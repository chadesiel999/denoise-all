print("正在启动脚本，加载依赖库中...", flush=True) # 添加这行调试信息
import torch
import torch.nn as nn
import numpy as np
import os
from scipy import signal
import matplotlib.pyplot as plt
from torch.utils.data import Dataset, DataLoader
import time
import random

# ==========================================
# 1. 配置与环境
# ==========================================
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# 路径设置
current_dir = os.path.dirname(os.path.abspath(__file__))
results_dir = os.path.join(current_dir, 'results')
models_dir = os.path.join(current_dir, 'saved_models')
figures_dir = os.path.join(current_dir, 'figures')

for d in [results_dir, models_dir, figures_dir]:
    os.makedirs(d, exist_ok=True)

# 超参数
BATCH_SIZE = 64
LR = 1e-4
NUM_EPOCHS = 200
SAMPLE_LEN = 512      # 网络输入长度，适当增加以捕获调制特征
FS = 80e9             # 80 GSPS
FREQ_WEIGHT = 0.5     # 频域Loss权重
INCLUDE_SINE = False  # 【重点】根据要求，训练集是否包含纯正弦波。设为False。

# ==========================================
# 2. 高级信号生成器
# ==========================================
class SignalGenerator:
    def __init__(self, fs=80e9, length=10000):
        self.fs = fs
        self.length = length
        self.t = np.arange(length) / fs

    def add_noise(self, y, snr_range=(5, 20)):
        """添加高斯白噪声"""
        snr = np.random.uniform(*snr_range)
        sig_power = np.mean(y ** 2)
        noise_power = sig_power / (10 ** (snr / 10))
        noise = np.random.normal(0, np.sqrt(noise_power), y.shape)
        return y + noise

    def get_freq_range(self, start, end, step):
        return np.arange(start, end + step, step)

    # --- 周期信号 ---
    def generate_periodic(self):
        data = []
        # 频率范围: 100M 到 2G, 间隔 50M (为了减少数据量，间隔设大一点，原图是10k深度)
        freqs = self.get_freq_range(100e6, 2e9, 50e6)
        
        for f in freqs:
            # 1. Square (方波) - 占空比 1:9 (0.1) 和 5:5 (0.5)
            for duty in [0.1, 0.5]:
                sig = signal.square(2 * np.pi * f * self.t, duty=duty)
                data.append({'type': 'square', 'data': sig})
            
            # 2. Sawtooth (锯齿波)
            sig = signal.sawtooth(2 * np.pi * f * self.t, width=1)
            data.append({'type': 'sawtooth', 'data': sig})
            
            # 3. Triangle (三角波)
            sig = signal.sawtooth(2 * np.pi * f * self.t, width=0.5)
            data.append({'type': 'triangle', 'data': sig})
        
        # 4. Dual Tone (双音信号)
        # 100M-2G, 50M间隔, freq2 = 1.5 freq1, 幅度 1:1
        for f1 in freqs:
            f2 = 1.5 * f1
            if f2 > self.fs / 2: continue # 奈奎斯特限制
            sig = 0.5 * np.sin(2 * np.pi * f1 * self.t) + 0.5 * np.sin(2 * np.pi * f2 * self.t)
            data.append({'type': 'dual_tone', 'data': sig})
            
        return data

    # --- 正弦信号 (按需生成) ---
    def generate_sine_only(self):
        data = []
        freqs = self.get_freq_range(100e6, 2e9, 50e6) # 间隔50M以节省时间
        for f in freqs:
            sig = np.sin(2 * np.pi * f * self.t)
            data.append({'type': 'sine', 'data': sig})
        return data

    # --- AM 调制 ---
    def generate_am(self):
        data = []
        # 载波 1G-2G, 间隔 100M
        carriers = self.get_freq_range(1e9, 2e9, 100e6)
        f_mod = 20e6 # 调制波 20M
        
        for fc in carriers:
            # AM modulation: (1 + m*cos(wm*t)) * cos(wc*t)
            # Depth 100% -> m=1
            mod_signal = np.cos(2 * np.pi * f_mod * self.t)
            carrier_signal = np.cos(2 * np.pi * fc * self.t)
            sig = (1 + 1.0 * mod_signal) * carrier_signal
            # 归一化幅度
            sig = sig / np.max(np.abs(sig))
            data.append({'type': 'am', 'data': sig})
        return data

    # --- 数字调制 (BPSK, QPSK... 256QAM) ---
    def generate_digital_mod(self):
        data = []
        fc = 1e9 # 载波 1G
        # 符号率 200M - 500M, 间隔 100M
        symbol_rates = self.get_freq_range(200e6, 500e6, 100e6)
        mod_types = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM', '128QAM', '256QAM']
        
        for sym_rate in symbol_rates:
            # 计算每个符号的采样数 (Upsample factor)
            sps = int(self.fs / sym_rate)
            if sps < 2: continue # 采样率不足

            # 生成足够的符号以填充时间长度
            num_symbols = int(self.length / sps) + 10
            
            for mod in mod_types:
                # 1. 生成随机整数符号
                M = 1
                if mod == 'BPSK': M=2
                elif mod == 'QPSK': M=4
                elif mod == '8PSK': M=8
                elif mod == '16QAM': M=16
                elif mod == '64QAM': M=64
                elif mod == '128QAM': M=128
                elif mod == '256QAM': M=256
                
                ints = np.random.randint(0, M, num_symbols)
                
                # 2. 星座图映射 (简化版，未做严格格雷码，主要为了波形特征)
                if 'PSK' in mod:
                    phase = 2 * np.pi * ints / M
                    symbols = np.exp(1j * phase)
                else: # QAM
                    # 简单的矩形QAM生成
                    sqrt_M = int(np.sqrt(M))
                    # 对于非平方数的QAM(如128)，这里做简化处理，统一用矩形网格近似
                    # 真实通信中128QAM是十字形，这里为了训练去噪，用正态分布复数近似高阶特征即可
                    # 或者严格映射:
                    if mod == '128QAM': # 特殊处理，或简化为随机复数
                        real = np.random.randint(-11, 12, num_symbols) # 粗略
                        imag = np.random.randint(-11, 12, num_symbols)
                        symbols = real + 1j*imag
                    else:
                        real = 2 * (ints % sqrt_M) - (sqrt_M - 1)
                        imag = 2 * (ints // sqrt_M) - (sqrt_M - 1)
                        symbols = real + 1j * imag
                
                # 3. 脉冲成型 (使用升余弦滚降系数 0.5)
                # 为了速度，使用 scipy.signal.resample_poly 或简单的插值+滤波
                # 这里模拟零阶保持后加平滑滤波来模拟成型
                symbols_upsampled = np.repeat(symbols, sps)
                # 截取所需长度
                symbols_upsampled = symbols_upsampled[:self.length]
                
                # 简单的低通滤波模拟成型 (截止频率 = 符号率 * (1+alpha)/2)
                # 归一化截止频率
                nyq = 0.5 * self.fs
                cutoff = (sym_rate * 1.5) / nyq
                if cutoff >= 1.0: cutoff = 0.99
                b, a = signal.butter(4, cutoff, btype='low')
                
                # 对 I 和 Q 分别滤波
                i_sig = signal.lfilter(b, a, symbols_upsampled.real)
                q_sig = signal.lfilter(b, a, symbols_upsampled.imag)
                
                # 4. 上变频 (IQ 调制) -> Real Signal
                # y(t) = I(t)cos(wt) - Q(t)sin(wt)
                carrier_i = np.cos(2 * np.pi * fc * self.t)
                carrier_q = np.sin(2 * np.pi * fc * self.t)
                
                sig = i_sig * carrier_i - q_sig * carrier_q
                
                # 归一化
                if np.max(np.abs(sig)) > 0:
                    sig = sig / np.max(np.abs(sig))
                
                data.append({'type': f'{mod}_{sym_rate}', 'data': sig})
                
        return data

# ==========================================
# 3. 数据集生成与切片
# ==========================================
def create_dataset(sample_len=SAMPLE_LEN, num_slices_per_sig=10):
    print(f"正在生成基础波形 (FS={FS/1e9} GHz)...")
    gen = SignalGenerator(fs=FS, length=10000) # 10k 深度
    
    clean_signals = []
    
    # 1. 周期信号 (方波、锯齿、三角、双音)
    print("- Generating Periodic signals...")
    clean_signals.extend(gen.generate_periodic())
    
    # 2. AM 调制
    print("- Generating AM signals...")
    clean_signals.extend(gen.generate_am())
    
    # 3. 数字调制
    print("- Generating Digital Mod signals (This may take time)...")
    clean_signals.extend(gen.generate_digital_mod())
    
    # 4. 正弦波 (根据 INCLUDE_SINE 决定是否加入)
    if INCLUDE_SINE:
        print("- Generating Sine signals...")
        clean_signals.extend(gen.generate_sine_only())
    else:
        print("- Skipping Sine signals for training set (as requested).")
    
    print(f"Total raw signal types generated: {len(clean_signals)}")
    
    X_list = []
    Y_list = []
    
    print(f"Slicing and adding noise (Target Sample Len: {sample_len})...")
    for item in clean_signals:
        raw_clean = item['data']
        # 归一化幅度到 [-1, 1]
        raw_clean = raw_clean / (np.max(np.abs(raw_clean)) + 1e-9)
        
        # 多次切片以增加数据量
        for _ in range(num_slices_per_sig):
            start_idx = np.random.randint(0, len(raw_clean) - sample_len)
            segment_clean = raw_clean[start_idx : start_idx + sample_len]
            
            # 添加噪声
            segment_noisy = gen.add_noise(segment_clean, snr_range=(0, 15)) # 噪声稍大一点以训练鲁棒性
            
            X_list.append(segment_noisy)
            Y_list.append(segment_clean)
            
    X = np.array(X_list, dtype=np.float32)
    Y = np.array(Y_list, dtype=np.float32)
    
    # 增加 Channel 维度 (N, L, 1)
    X = X[..., np.newaxis]
    Y = Y[..., np.newaxis]
    
    return X, Y

# 执行生成
X_data, Y_data = create_dataset(sample_len=SAMPLE_LEN, num_slices_per_sig=50)

# 打乱并划分
indices = np.arange(len(X_data))
np.random.shuffle(indices)
X_data, Y_data = X_data[indices], Y_data[indices]

split_idx = int(len(X_data) * 0.8)
X_train, Y_train = X_data[:split_idx], Y_data[:split_idx]
X_val, Y_val = X_data[split_idx:], Y_data[split_idx:]

print(f"Final Dataset Shapes: Train {X_train.shape}, Val {X_val.shape}")

# ==========================================
# 4. 模型定义 (保持原有的优秀结构)
# ==========================================
class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1, padding_mode='replicate')
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.0, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.0, bidirectional=True)
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1, padding_mode='replicate')

    def forward(self, x):
        # x: (batch, seq, dim) -> (batch, dim, seq) for Conv
        x = self.conv1(x.permute(0, 2, 1))
        x = x.transpose(1, 2)  # -> (batch, seq, hidden)
        
        x, _ = self.GRU_layer_1(x)
        x, _ = self.GRU_layer_2(x)
        
        # -> (batch, hidden, seq) for Conv
        x = self.conv2(x.permute(0, 2, 1))
        x = x.transpose(1, 2)
        return x

model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(device)
optimizer = torch.optim.Adam(model.parameters(), lr=LR)
criterion_mse = nn.MSELoss()

# ==========================================
# 5. 训练循环
# ==========================================
class SignalDataset(Dataset):
    def __init__(self, x, y):
        self.x = torch.from_numpy(x).float()
        self.y = torch.from_numpy(y).float()
    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]
    def __len__(self):
        return len(self.x)

train_loader = DataLoader(SignalDataset(X_train, Y_train), batch_size=BATCH_SIZE, shuffle=True)
val_loader = DataLoader(SignalDataset(X_val, Y_val), batch_size=BATCH_SIZE, shuffle=False)

print("Starting training...")
train_losses, val_losses = [], []
min_val_loss = float('inf')

for epoch in range(NUM_EPOCHS):
    model.train()
    batch_losses = []
    
    for bx, by in train_loader:
        bx, by = bx.to(device), by.to(device)
        
        pred = model(bx)
        
        # Time Domain Loss
        loss_time = criterion_mse(pred, by)
        
        # Frequency Domain Loss (简化版，增强高频细节恢复)
        # 使用 FFT
        pred_fft = torch.fft.rfft(pred, dim=1)
        target_fft = torch.fft.rfft(by, dim=1)
        loss_freq = torch.mean(torch.abs(torch.abs(pred_fft) - torch.abs(target_fft)))
        
        loss = (1 - FREQ_WEIGHT) * loss_time + FREQ_WEIGHT * loss_freq * 0.1 # 缩放频域Loss
        
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        batch_losses.append(loss.item())
        
    avg_train_loss = np.mean(batch_losses)
    train_losses.append(avg_train_loss)
    
    # Validation
    model.eval()
    val_batch_losses = []
    with torch.no_grad():
        for bx, by in val_loader:
            bx, by = bx.to(device), by.to(device)
            pred = model(bx)
            loss_time = criterion_mse(pred, by)
            val_batch_losses.append(loss_time.item()) # Val只看MSE
            
    avg_val_loss = np.mean(val_batch_losses)
    val_losses.append(avg_val_loss)
    
    print(f"Epoch {epoch+1}/{NUM_EPOCHS} | Train Loss: {avg_train_loss:.5f} | Val MSE: {avg_val_loss:.5f}")
    
    if avg_val_loss < min_val_loss:
        min_val_loss = avg_val_loss
        torch.save(model.state_dict(), os.path.join(models_dir, 'best_model.pth'))

# ==========================================
# 6. 可视化验证
# ==========================================
# 生成几个包含正弦波的测试数据（即使训练没用，测试也要看看泛化能力）
gen = SignalGenerator(fs=FS, length=SAMPLE_LEN)
test_sigs = []

# 手动构建时间轴用于测试信号
t_test = np.arange(SAMPLE_LEN) / FS

# 测试1: 正弦波 - 500MHz (可见约3个周期)
sig_sine = np.sin(2 * np.pi * 500e6 * t_test)
test_sigs.append(('Sine (Unseen, 500MHz)', sig_sine))

# 测试2: 16QAM - 选取 500MHz 符号率以显示更多变化
all_dig_mods = gen.generate_digital_mod()
qam_candidates = [x for x in all_dig_mods if '16QAM' in x['type'] and '500000000' in x['type']]
if qam_candidates:
    test_sigs.append(('16QAM (500Msps)', qam_candidates[0]['data']))
else:
    test_sigs.append(('16QAM', [x for x in all_dig_mods if '16QAM' in x['type']][-1]['data']))

# 测试3: 锯齿波 - 500MHz (可见约3个周期)
sig_saw = signal.sawtooth(2 * np.pi * 500e6 * t_test, width=1)
test_sigs.append(('Sawtooth (500MHz)', sig_saw))

model.load_state_dict(torch.load(os.path.join(models_dir, 'best_model.pth')))
model.eval()

plt.figure(figsize=(15, 10))
for i, (name, sig) in enumerate(test_sigs):
    # 处理数据
    sig = sig[:SAMPLE_LEN]
    sig = sig / np.max(np.abs(sig))
    noisy = gen.add_noise(sig, snr_range=(5, 5))
    
    inp_tensor = torch.tensor(noisy[np.newaxis, :, np.newaxis], dtype=torch.float32).to(device)
    with torch.no_grad():
        denoised = model(inp_tensor).cpu().numpy().flatten()
        
    plt.subplot(3, 1, i+1)
    plt.plot(noisy, color='lightgray', label='Noisy Input', alpha=0.7)
    plt.plot(sig, color='blue', linestyle='--', label='Clean Target', alpha=0.6)
    plt.plot(denoised, color='red', label='Denoised Output', linewidth=1.5)
    plt.title(f"Test on {name}")
    plt.legend(loc='upper right')
    plt.grid(True, alpha=0.3)

plt.tight_layout()
plt.savefig(os.path.join(figures_dir, 'final_verification.png'))
print(f"Done! Results saved to {figures_dir}")