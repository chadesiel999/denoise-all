import numpy as np
import torch
from scipy import signal
import matplotlib.pyplot as plt
import os
from torch.utils.data import TensorDataset, DataLoader
import torch.nn as nn

# ==========================================
# 1. 全局配置 (Global Config)
# ==========================================
FS = 80e9          # 采样率 80 GSPS
DEPTH = 10000      # 原始信号生成长度 (用于切片的长序列)
SAMPLE_LEN = 512   # 最终训练用的单样本长度 (参考 complex 代码)
NUM_SLICES = 50    # 每个长信号切出的样本数
TIME_VECTOR = np.arange(DEPTH) / FS # 预计算时间轴

# 路径配置
SAVE_DIR = './generated_data'
if not os.path.exists(SAVE_DIR):
    os.makedirs(SAVE_DIR)

print(f"配置: 采样率={FS/1e9} GSPS, 原始长度={DEPTH}, 切片长度={SAMPLE_LEN}, 切片数={NUM_SLICES}")

# ==========================================
# 2. 信号生成核心类 (Signal Generator)
# ==========================================
class SignalCore:
    def __init__(self, fs=FS, t=TIME_VECTOR):
        self.fs = fs
        self.t = t

    def add_noise(self, clean_sig, snr_range=(24, 26)):
        """添加高斯白噪声，SNR随机"""
        snr = np.random.uniform(*snr_range)
        sig_power = np.mean(clean_sig ** 2)
        noise_power = sig_power / (10 ** (snr / 10))
        noise = np.random.normal(0, 0.04, clean_sig.shape)
        return clean_sig + noise

    # --- 基础波形 ---
    def gen_sine(self, freq):
        return np.sin(2 * np.pi * freq * self.t)

    def gen_square(self, freq, duty):
        return signal.square(2 * np.pi * freq * self.t, duty=duty)

    def gen_triangle(self, freq):
        # width=0.5 对应三角波
        return signal.sawtooth(2 * np.pi * freq * self.t, width=0.5)

    def gen_sawtooth(self, freq):
        # width=1.0 对应锯齿波
        return signal.sawtooth(2 * np.pi * freq * self.t, width=1.0)
    
    def gen_dualtone(self, freq1):
        freq2 = 1.5 * freq1
        # 幅度 1:1
        return 0.5 * np.sin(2 * np.pi * freq1 * self.t) + 0.5 * np.sin(2 * np.pi * freq2 * self.t)

    # --- AM 调制 ---
    def gen_am(self, carrier_freq, mod_freq, depth):
        # depth: 0.0 - 1.0 (例如 100% -> 1.0)
        # y(t) = [1 + m * cos(wm*t)] * cos(wc*t)
        # 注意：通常调制波用cos或sin皆可，这里用cos
        mod_sig = np.cos(2 * np.pi * mod_freq * self.t)
        carrier_sig = np.cos(2 * np.pi * carrier_freq * self.t)
        am_sig = (1 + depth * mod_sig) * carrier_sig
        # 归一化幅度
        return am_sig / (1 + depth)

    # --- 数字调制 (含成型滤波) ---
    def gen_digital(self, carrier_freq, symbol_rate, mod_type, alpha=0.5):
        # 1. 计算参数
        sps = int(self.fs / symbol_rate) # Samples per symbol
        num_symbols = int(len(self.t) / sps) + 5
        
        # 2. 生成符号映射
        M_map = {
            'BPSK': 2, 'QPSK': 4, '8PSK': 8, 
            '16QAM': 16, '64QAM': 64, '128QAM': 128, '256QAM': 256
        }
        M = M_map.get(mod_type, 4)
        ints = np.random.randint(0, M, num_symbols)
        
        # 星座图映射
        if 'PSK' in mod_type or mod_type == 'BPSK':
            phase = 2 * np.pi * ints / M
            symbols = np.exp(1j * phase)
        else: # QAM
            # 简化的矩形QAM生成
            sqrt_M = int(np.sqrt(M))
            if mod_type == '128QAM': # 128QAM非正方形，做特殊近似
                real = np.random.randint(-11, 12, num_symbols)
                imag = np.random.randint(-11, 12, num_symbols)
                symbols = real + 1j * imag
            else:
                real = 2 * (ints % sqrt_M) - (sqrt_M - 1)
                imag = 2 * (ints // sqrt_M) - (sqrt_M - 1)
                symbols = real + 1j * imag
        
        # 3. 脉冲成型 (RRC Filter 近似)
        # 由于 80GSPS 下点数极多，我们使用上采样+卷积的方法
        upsampled = np.zeros(len(symbols) * sps, dtype=complex)
        upsampled[::sps] = symbols
        
        # 生成 RRC 滤波器抽头
        # 长度取 8 个符号周期足够
        ntaps = 8 * sps 
        # 使用 scipy 的 rrc 设计或者用 windowed sinc 近似
        # 这里为了运行效率，构建一个升余弦滤波器
        t_rrc = np.arange(-ntaps//2, ntaps//2) / sps
        # 避免分母为0
        # t_rrc[t_rrc == 0] = 1e-8 
        # t_rrc[np.abs(4*alpha*t_rrc) == 1] = 1/(4*alpha) # 处理特殊点
        # 改进的奇点处理
        # 1. 处理 t=0
        mask_0 = np.isclose(t_rrc, 0)
        t_rrc[mask_0] = 1e-8 
        
        # 2. 处理分母由 (1 - (4*alpha*t)**2) 导致的零点 => |4*alpha*t| = 1 => |t| = 1/(4*alpha)
        # 对于 alpha=0.5, |t|=0.5
        denom_zeros = np.isclose(np.abs(4*alpha*t_rrc), 1.0)
        # 对这些点，稍微偏移一点点避免除零，或者直接赋一个理论极限值（这里采用偏移法更简单）
        t_rrc[denom_zeros] += 1e-6
        
        # RRC 公式 (简化版)
        num = np.sin(np.pi * t_rrc * (1-alpha)) + 4*alpha*t_rrc * np.cos(np.pi * t_rrc * (1+alpha))
        denom = np.pi * t_rrc * (1 - (4*alpha*t_rrc)**2)
        rrc_filter = num / denom
        
        # 滤波
        baseband = np.convolve(upsampled, rrc_filter, mode='same')
        
        # 截断到目标长度
        baseband = baseband[:len(self.t)]
        if len(baseband) < len(self.t):
            baseband = np.pad(baseband, (0, len(self.t)-len(baseband)))
            
        # 4. 上变频
        carrier_i = np.cos(2 * np.pi * carrier_freq * self.t)
        carrier_q = np.sin(2 * np.pi * carrier_freq * self.t)
        
        # I*cos - Q*sin
        rf_sig = baseband.real * carrier_i - baseband.imag * carrier_q
        
        # 归一化
        scale = np.max(np.abs(rf_sig))
        if scale > 0:
            rf_sig /= scale
            
        return rf_sig

# ==========================================
# 3. 数据集生成逻辑
# ==========================================
def generate_dataset(mode='train'):
    """
    mode: 'train' 或 'test'
    完全依照需求列表进行参数遍历，但采用了切片策略和更稀疏的频率步进以适配数据量
    """
    gen = SignalCore()
    data_x = [] # Noisy
    data_y = [] # Clean
    
    print(f"\n>>> 开始生成 [{mode.upper()}] 数据集...")
    
    # 辅助函数：生成频率列表
    def get_range(start, end, step):
        # 包含 end
        return np.arange(start, end + step/1000.0, step)

    # 辅助函数：切片与处理
    def process_and_append(clean_sig):
        # 归一化幅度到 [-1, 1]
        max_val = np.max(np.abs(clean_sig))
        if max_val > 0:
            clean_sig = clean_sig / max_val
            
        for _ in range(NUM_SLICES):
            if len(clean_sig) <= SAMPLE_LEN:
                continue
            # 随机切片
            start_idx = np.random.randint(0, len(clean_sig) - SAMPLE_LEN)
            segment_clean = clean_sig[start_idx : start_idx + SAMPLE_LEN]
            
            # 添加噪声
            segment_noisy = gen.add_noise(segment_clean)
            
            data_x.append(segment_noisy)
            data_y.append(segment_clean)

    # ----------------------------------------
    # A. 训练/验证数据生成 (Strict Training Params)
    # ----------------------------------------
    if mode == 'train':
        # 1. 正弦信号: 100M-2G, 50M间隔 (原5M->50M)
        print("1. 生成正弦波 (Train)...")
        freqs = get_range(100e6, 2e9, 50e6)
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean)

        # 2. 方波: 100M-2G, 50M间隔 (原10M->50M), 占空比 1:9(0.1) 和 5:5(0.5)
        print("2. 生成方波 (Train)...")
        freqs = get_range(100e6, 2e9, 50e6)
        for f in freqs:
            for duty in [0.1, 0.5]:
                clean = gen.gen_square(f, duty)
                process_and_append(clean)

        # 3. 三角波: 100M-2G, 50M间隔 (原10M->50M)
        print("3. 生成三角波 (Train)...")
        freqs = get_range(100e6, 2e9, 50e6)
        for f in freqs:
            clean = gen.gen_triangle(f)
            process_and_append(clean)

        # 4. 锯齿波: 100M-2G, 50M间隔 (原10M->50M)
        print("4. 生成锯齿波 (Train)...")
        freqs = get_range(100e6, 2e9, 50e6)
        for f in freqs:
            clean = gen.gen_sawtooth(f)
            process_and_append(clean)

        # 5. AM调制: 载波1G-2G(50M), 调波20M-100M(20M), 深度100%和10%
        # (原载波10M/调波5M -> 50M/20M)
        print("5. 生成AM信号 (Train)...")
        carriers = get_range(1e9, 2e9, 50e6)
        mod_freqs = get_range(20e6, 100e6, 20e6)
        depths = [1.0, 0.1]
        
        count = 0
        for fc in carriers:
            for fm in mod_freqs:
                for d in depths:
                    clean = gen.gen_am(fc, fm, d)
                    process_and_append(clean)
                    count += 1
        print(f"   AM 基础波形数: {count}, 总切片数: {count * NUM_SLICES}")

    # ----------------------------------------
    # B. 测试数据生成 (Strict Test Params)
    # ----------------------------------------
    elif mode == 'test':
        # 1. 正弦: 102.5M - 5G, 100M间隔 (原50M->100M)
        print("1. 生成正弦波 (Test)...")
        freqs = get_range(102.5e6, 5e9, 100e6)
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean)

        # 2. 方波: 105M - 2G, 100M间隔 (原50M->100M)
        print("2. 生成方波 (Test)...")
        freqs = get_range(105e6, 2e9, 100e6)
        for f in freqs:
            for duty in [0.1, 0.5, 0.3]:
                clean = gen.gen_square(f, duty)
                process_and_append(clean)

        # 3. 三角波: 105M - 2G, 100M间隔
        print("3. 生成三角波 (Test)...")
        freqs = get_range(105e6, 2e9, 100e6)
        for f in freqs:
            clean = gen.gen_triangle(f)
            process_and_append(clean)
            
        # 4. 锯齿波: 105M - 2G, 100M间隔
        print("4. 生成锯齿波 (Test)...")
        freqs = get_range(105e6, 2e9, 100e6)
        for f in freqs:
            clean = gen.gen_sawtooth(f)
            process_and_append(clean)

        # 5. 双音信号: 100M-5G, 200M间隔 (原100M->200M)
        print("5. 生成双音信号 (Test)...")
        freqs = get_range(100e6, 5e9, 200e6)
        for f in freqs:
            if 1.5 * f < FS/2: # 确保不混叠
                clean = gen.gen_dualtone(f)
                process_and_append(clean)

        # 6. 数字调制: 载波1G, 3G; 符号率200M-500M(100M); 
        print("6. 生成数字调制 (Test)...")
        carriers = [1e9, 3e9]
        sym_rates = get_range(200e6, 500e6, 100e6)
        mod_types = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM', '128QAM', '256QAM']
        
        for fc in carriers:
            for sym in sym_rates:
                for mod in mod_types:
                    clean = gen.gen_digital(fc, sym, mod, alpha=0.5)
                    process_and_append(clean)

        # 7. AM调制: 载波1G-5G(200M), 调波20M-400M(100M) (原100M->200M)
        print("7. 生成AM信号 (Test)...")
        carriers = get_range(1e9, 5e9, 200e6)
        mod_freqs = get_range(20e6, 400e6, 100e6)
        depths = [1.0, 0.5]
        
        for fc in carriers:
            for fm in mod_freqs:
                for d in depths:
                    clean = gen.gen_am(fc, fm, d)
                    process_and_append(clean)

    # ----------------------------------------
    # C. 格式转换与保存
    # ----------------------------------------
    X = np.array(data_x, dtype=np.float32)
    Y = np.array(data_y, dtype=np.float32)
    
    # Check for NaNs
    if np.isnan(X).any() or np.isnan(Y).any():
        print(f"警告：检测到 NaN 值！在 {mode} 数据集中。正在清除...")
        valid_idx = ~(np.isnan(X).any(axis=1) | np.isnan(Y).any(axis=1))
        X = X[valid_idx]
        Y = Y[valid_idx]
        print(f"清除后剩余样本数: {len(X)}")

    # 增加 Channel 维度 (N, Length, 1) 
    # 适配 PyTorch Conv1d 输入要求 (Batch, Channel, Length) 或 (Batch, Length, Channel)
    # 这里保持 (N, L, 1)
    X = X[..., np.newaxis]
    Y = Y[..., np.newaxis]
    
    print(f"数据集 [{mode}] 生成完毕: Shape {X.shape}")
    
    # 保存 Tensor
    torch.save((torch.from_numpy(X), torch.from_numpy(Y)), 
               os.path.join(SAVE_DIR, f'{mode}_dataset.pth'))
    return X, Y

# ==========================================
# 4. 执行入口
# ==========================================
if __name__ == "__main__":
    # 生成训练集
    X_train, Y_train = generate_dataset('train')
    
    # 生成测试集
    X_test, Y_test = generate_dataset('test')
    
    # 画图验证（随机抽取几个看波形）
    print("正在保存验证波形图...")
    idx_list = np.random.choice(len(X_test), 3, replace=False)
    
    for i, idx in enumerate(idx_list):
        plt.figure(figsize=(10, 4))
        plt.plot(X_test[idx].flatten(), color='lightgray', label='Noisy')
        plt.plot(Y_test[idx].flatten(), color='red', linestyle='--', label='Clean')
        plt.title(f"Sample Index {idx}")
        plt.legend()
        plt.tight_layout()
        plt.savefig(f'{SAVE_DIR}/sample_waveform_{i}.png')
        plt.close()
        
    print(f"完成。数据已保存至 {SAVE_DIR}")

# ==========================================
# 5. 模型定义 (Model Definition)
# ==========================================
class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        # Conv1d 期望输入 (Batch, input_size, Length)
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        
        # GRU 期望输入 (Batch, Length, H_in) 因为 batch_first=True
        # H_in 这里就是 conv1 的输出通道数 hidden_size
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        
        self.fc = nn.Linear(hidden_size, hidden_size)
        self.relu = nn.ReLU()
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        # x shape: (Batch, 1, Length)
        
        x = self.conv1(x) 
        # x shape: (Batch, hidden_size, Length)
        
        # 调整为 GRU 需要的 (Batch, Length, Hidden)
        x = x.permute(0, 2, 1) 
        
        x, _ = self.GRU_layer_1(x)
        # x shape: (Batch, Length, hidden_size)  (双向导致输出维度是 2 * hidden/2 = hidden)
        
        x, _ = self.GRU_layer_2(x)
        # x shape: (Batch, Length, hidden_size)
        
        # 调整回 Conv1d 需要的 (Batch, Hidden, Length)
        x = x.permute(0, 2, 1)
        
        x = self.conv2(x)
        # x shape: (Batch, output_size, Length)
        
        return x