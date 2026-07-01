import torch
import torch.nn as nn
import numpy as np
import os
from scipy import signal
import matplotlib.pyplot as plt
from torch.utils.data import Dataset, DataLoader
import time

# ==========================================
# 1. 配置与环境 (Config & Setup)
# ==========================================
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# 路径设置
current_dir = os.path.dirname(os.path.abspath(__file__))
results_dir = os.path.join(current_dir, 'results')
models_dir = os.path.join(current_dir, 'saved_models')
figures_dir = os.path.join(current_dir, 'figures')

for d in [results_dir, models_dir, figures_dir]:
    if not os.path.exists(d):
        os.makedirs(d)
        print(f"Created directory: {d}")

# 超参数
BATCH_SIZE = 64
LR = 1e-4
NUM_EPOCHS = 200  # 演示用，可自行增加
SAMPLE_LEN = 240  # 根据 broadband 代码的习惯，或者设为 500
FREQ_WEIGHT = 0.0 # 频域 Loss 的权重 (改为0以消除吉布斯振荡)

# ==========================================
# 2. 数据生成部分 (来自 createSimulate.py)
# ==========================================
def generate_synthetic_data(total_len_factor=1000, sample_size=240):
    """
    生成合成数据：正弦、三角、锯齿、PWM
    去除 occ 脉冲，去除 SNR 计算
    """
    fs = 128
    f = 10
    Amp = 5
    
    # 基础时间轴
    # 为了生成足够多的数据，这里将长度拉长
    total_points = fs * f * total_len_factor 
    x = np.linspace(0, 2 * np.pi * f * (1 - 1 / (f * fs)) * total_len_factor, total_points)
    
    # 1. 生成纯净信号 (作为 Label / Ground Truth)
    y_sin = Amp * np.sin(x)
    y_triangle = Amp * signal.sawtooth(x, width=0.5)
    y_saw = Amp * signal.sawtooth(x, width=1)
    y_pwm = Amp * signal.square(x)
    
    # 2. 添加高斯噪声 (作为 Input)
    noise_level = 0.3
    y_sin_n = y_sin + noise_level * np.random.normal(size=len(x))
    y_triangle_n = y_triangle + noise_level * np.random.normal(size=len(x))
    y_saw_n = y_saw + noise_level * np.random.normal(size=len(x))
    y_pwm_n = y_pwm + noise_level * np.random.normal(size=len(x))
    
    return x, (y_sin, y_triangle, y_saw, y_pwm), (y_sin_n, y_triangle_n, y_saw_n, y_pwm_n)

def slice_data(clean_signals, noisy_signals, train_ratio=0.7, val_ratio=0.1, sample_size=240, n_samples_per_type=2000):
    """
    将长信号切片成短样本，并划分为 Train/Val/Test
    """
    X_all = []
    Y_all = []
    
    # 遍历四种波形
    for clean, noisy in zip(clean_signals, noisy_signals):
        # 随机切片
        for _ in range(n_samples_per_type):
            random_offset = np.random.randint(0, len(clean) - sample_size)
            sample_inp = noisy[random_offset:random_offset + sample_size]
            sample_out = clean[random_offset:random_offset + sample_size]
            
            X_all.append(sample_inp)
            Y_all.append(sample_out)
            
    X_all = np.array(X_all)
    Y_all = np.array(Y_all)
    
    # 增加维度以适应 PyTorch (N, L, 1)
    X_all = X_all[..., np.newaxis]
    Y_all = Y_all[..., np.newaxis]
    
    # 打乱数据
    indices = np.arange(len(X_all))
    np.random.shuffle(indices)
    X_all = X_all[indices]
    Y_all = Y_all[indices]
    
    # 划分数据集
    n_total = len(X_all)
    n_train = int(n_total * train_ratio)
    n_val = int(n_total * val_ratio)
    
    X_train, Y_train = X_all[:n_train], Y_all[:n_train]
    X_val, Y_val = X_all[n_train:n_train+n_val], Y_all[n_train:n_train+n_val]
    X_test, Y_test = X_all[n_train+n_val:], Y_all[n_train+n_val:]
    
    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)

print("正在生成数据...")
x_axis, cleans, noisys = generate_synthetic_data(total_len_factor=100, sample_size=SAMPLE_LEN)
(X_train, Y_train), (X_val, Y_val), (X_test, Y_test) = slice_data(cleans, noisys, sample_size=SAMPLE_LEN)

# ==== 数据归一化 (Normalization) ====
# 信号幅度大约是 5，噪声加成后大约在 [-6, 6] 之间。
# 我们将其统一除以 6，归一化到 [-1, 1] 附近
# NORM_FACTOR = 6.0
# X_train /= NORM_FACTOR
# Y_train /= NORM_FACTOR
# X_val /= NORM_FACTOR
# Y_val /= NORM_FACTOR
# X_test /= NORM_FACTOR
# Y_test /= NORM_FACTOR

# 转为 Tensor
X_train = torch.tensor(X_train, dtype=torch.float32)
Y_train = torch.tensor(Y_train, dtype=torch.float32)
X_val = torch.tensor(X_val, dtype=torch.float32)
Y_val = torch.tensor(Y_val, dtype=torch.float32)
X_test = torch.tensor(X_test, dtype=torch.float32)
Y_test = torch.tensor(Y_test, dtype=torch.float32)

print(f"数据生成完毕: Train shape: {X_train.shape}, Val shape: {X_val.shape}")

# ==========================================
# 3. 模型定义 (来自 createSimulate.py，替代外部 model)
# ==========================================
class GRUModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUModel, self).__init__()
        self.hidden_size = hidden_size
        # 3层 GRU，dropout防止过拟合
        self.GRU_layer = nn.GRU(input_size=input_size, hidden_size=hidden_size, num_layers=3, batch_first=True, dropout=0.2)
        self.fc = nn.Linear(hidden_size, output_size)

    def forward(self, input):
        # input shape: (batch, seq_len, input_size)
        output, _ = self.GRU_layer(input)
        # output shape: (batch, seq_len, hidden_size)
        output = self.fc(output)
        # output shape: (batch, seq_len, output_size)
        return output
    
class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        # 使用 replicate 填充模式解决边缘效应
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1, padding_mode='replicate')
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.fc = nn.Linear(hidden_size, hidden_size)
        self.relu = nn.ReLU()
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1, padding_mode='replicate')

    def forward(self, x):
        # x: (batch, seq, dim) -> permute -> (batch, dim, seq)
        # 移除 ReLU，卷积后直接进 RNN
        x = self.conv1(x.permute(0, 2, 1))
        # x = self.relu(x) 
        
        x = x.transpose(1, 2)  # 调整维度 (batch, seq, hidden)
        x, _ = self.GRU_layer_1(x)
        x, _ = self.GRU_layer_2(x)
        
        # permute back for conv2: (batch, hidden, seq)
        x = self.conv2(x.permute(0, 2, 1))
        x = x.transpose(1, 2)  # 调整维度
        return x

# 初始化模型
input_dim = 1
hidden_size = 64
output_size = 1

model = GRUCNNModel(input_dim, hidden_size, output_size)
model = model.to(device)

optimizer = torch.optim.Adam(model.parameters(), lr=LR, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
criterion_mse = nn.MSELoss()

# ==========================================
# 4. 数据加载器 (Dataset & DataLoader)
# ==========================================
class MyDataset(Dataset):
    def __init__(self, data_root, data_label):
        self.data = data_root.to(device)
        self.data_label = data_label.to(device)

    def __getitem__(self, index):
        return self.data[index], self.data_label[index]

    def __len__(self):
        return len(self.data)

train_data = MyDataset(X_train, Y_train)
val_data = MyDataset(X_val, Y_val)

train_loader = DataLoader(dataset=train_data, batch_size=BATCH_SIZE, shuffle=True)
val_loader = DataLoader(dataset=val_data, batch_size=BATCH_SIZE, shuffle=False) # Val通常不需要shuffle

# ==========================================
# 5. 训练循环 (来自 broadband_denoise_new.py)
# ==========================================
losses = []
val_losses = []
min_val_loss = np.inf
patience_counter = 0
PATIENCE_LIMIT = 20

print("开始训练...")
for i in range(NUM_EPOCHS):
    start_time = time.time()
    epoch_loss = []
    
    # --- Training ---
    model.train()
    for data in train_loader:
        x_batch, y_batch = data
        
        # 前向传播
        y_pred = model(x_batch)
        
        # 1. 时域 Loss (MSE)
        time_loss = criterion_mse(y_pred, y_batch)
        
        # 2. 频域 Loss (FFT based) - 这是 broadband 代码的核心逻辑
        # 加窗防止频谱泄漏
        window = signal.get_window('hann', x_batch.shape[1])
        window = window / np.sqrt(np.mean(window**2))
        window = torch.tensor(window, dtype=torch.float32, device=device)
        
        # 广播 window 到 batch 维度: (Batch, Len, 1)
        window = window.view(1, -1, 1) 
        
        windowed_noise = window * y_batch
        windowed_pred = window * y_pred
        
        # 计算 FFT (只取实部 rfft)
        pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
        noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
        
        # 转为对数谱 (dB)
        log_pred = 20 * torch.log10(pred_fft + 1e-10)
        log_noise = 20 * torch.log10(noise_fft + 1e-10)
        
        # 频域 Loss 计算
        freq_loss = torch.mean(torch.abs(log_pred - log_noise))
        
        # 总 Loss
        loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
        
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        
        epoch_loss.append(loss.item())

    avg_train_loss = sum(epoch_loss) / len(epoch_loss)
    losses.append(avg_train_loss)
    
    end_time = time.time()
    
    # --- Validation ---
    model.eval()
    valid_total_loss = 0
    val_batch_count = 0
    
    with torch.no_grad():
        for valid_data in val_loader:
            x_val_batch, y_val_batch = valid_data
            y_pred_val = model(x_val_batch)
            
            # 这里的 Val Loss 计算逻辑同 Train
            time_loss = criterion_mse(y_pred_val, y_val_batch)
            
            window = signal.get_window('hann', x_val_batch.shape[1])
            window = window / np.sqrt(np.mean(window**2))
            window = torch.tensor(window, dtype=torch.float32, device=device).view(1, -1, 1)
            
            windowed_noise = window * y_val_batch
            windowed_pred = window * y_pred_val
            
            pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
            noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
            
            log_pred = 20 * torch.log10(pred_fft + 1e-10)
            log_noise = 20 * torch.log10(noise_fft + 1e-10)
            
            freq_loss = torch.mean(torch.abs(log_pred - log_noise))
            
            val_batch_loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
            valid_total_loss += val_batch_loss.item()
            val_batch_count += 1
            
    avg_val_loss = valid_total_loss / val_batch_count
    val_losses.append(avg_val_loss)
    
    print(f"Epoch {i+1}/{NUM_EPOCHS} | Time: {end_time - start_time:.2f}s | Train Loss: {avg_train_loss:.6f} | Val Loss: {avg_val_loss:.6f}")

    # Early Stopping
    if avg_val_loss < min_val_loss:
        min_val_loss = avg_val_loss
        patience_counter = 0
        # 保存最佳模型
        torch.save(model.state_dict(), os.path.join(models_dir, "best_denoise_model.pth"))
    else:
        patience_counter += 1
        if patience_counter >= PATIENCE_LIMIT:
            print("Early stopping triggered.")
            break

# ==========================================
# 6. 结果保存与可视化
# ==========================================
print("训练结束，保存结果...")
np.savetxt(os.path.join(results_dir, 'training_loss.txt'), losses)
np.savetxt(os.path.join(results_dir, 'val_loss.txt'), val_losses)

# 简单测试并绘图
model.load_state_dict(torch.load(os.path.join(models_dir, "best_denoise_model.pth")))
model.eval()

# 随机抽取几个测试样本并画图
num_test_samples = 8
print(f"正在生成 {num_test_samples} 张测试结果对比图...")
indices = np.random.choice(len(X_test), num_test_samples, replace=False)

for i, idx in enumerate(indices):
    test_input = X_test[idx:idx+1].to(device) # shape (1, 240, 1)
    test_target = Y_test[idx:idx+1].cpu().numpy().flatten()
    
    with torch.no_grad():
        test_pred = model(test_input).cpu().numpy().flatten()
    
    test_input_np = test_input.cpu().numpy().flatten()

    plt.figure(figsize=(12, 6))
    # 将三条线画在同一个坐标系中
    plt.plot(test_input_np, color='lightgray', label='Noisy Input', linewidth=1)
    plt.plot(test_target, color='blue', label='Clean Target (Ground Truth)', linewidth=2, linestyle='--')
    plt.plot(test_pred, color='red', label='Denoised Output', linewidth=1.5, alpha=0.8)
    
    plt.title(f"Denoising Result - Sample Index {idx}")
    plt.legend()
    plt.grid(True, alpha=0.3)
    
    save_path = os.path.join(figures_dir, f'denoise_result_{i}.png')
    plt.savefig(save_path)
    plt.close()

print(f"所有结果图已保存至 {figures_dir}")