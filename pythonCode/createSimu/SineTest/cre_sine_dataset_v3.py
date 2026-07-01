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
DEPTH = 10000      # 原始信号生成长度 (用户要求 10k)
SAMPLE_LEN = 512   # 最终训练用的单样本长度 (保持切片长度以便模型训练)
# 如果希望训练更长序列，可以增加 SAMPLE_LEN，这里保持 v2 的设置

# 路径配置
SAVE_DIR = './createSimu/SineTest'
if not os.path.exists(SAVE_DIR):
    os.makedirs(SAVE_DIR)

print(f"配置: 采样率={FS/1e9} GSPS, 原始长度={DEPTH}, 切片长度={SAMPLE_LEN}")

# ==========================================
# 2. 信号生成核心类 (Signal Generator)
# ==========================================
class SignalCore:
    def __init__(self, fs=FS, t=None):
        self.fs = fs
        if t is None:
            self.t = np.arange(DEPTH) / FS 
        else:
            self.t = t

    def add_noise(self, clean_sig, snr_range=(24, 26)):
        """添加高斯白噪声，SNR随机"""
        snr = np.random.uniform(*snr_range)
        # 避免全0信号除零错误
        sig_power = np.mean(clean_sig ** 2)
        if sig_power == 0:
            noise_power = 0.04 
        else:
            noise_power = sig_power / (10 ** (snr / 10))
            
        noise = np.random.normal(0, np.sqrt(noise_power), clean_sig.shape)
        return clean_sig + noise

    # --- 只保留正弦波 ---
    def gen_sine(self, freq):
        return np.sin(2 * np.pi * freq * self.t)

# ==========================================
# 3. 数据集生成逻辑
# ==========================================
def generate_dataset(mode='train'):
    gen = SignalCore()
    data_x = [] # Noisy
    data_y = [] # Clean
    
    print(f"\n>>> 开始生成 [{mode.upper()}] 数据集 (Sine Only)...")
    
    def get_range(start, end, step):
        return np.arange(start, end + step/1000.0, step)

    def process_and_append(clean_sig, num_slices=1):
        max_val = np.max(np.abs(clean_sig))
        if max_val > 0:
            clean_sig = clean_sig / max_val
            
        # 尝试切片
        for _ in range(num_slices):
            if len(clean_sig) <= SAMPLE_LEN:
                continue
            # 随机切片
            start_idx = np.random.randint(0, len(clean_sig) - SAMPLE_LEN)
            segment_clean = clean_sig[start_idx : start_idx + SAMPLE_LEN]
            
            # 加噪声
            segment_noisy = gen.add_noise(segment_clean)
            
            data_x.append(segment_noisy)
            data_y.append(segment_clean)

    # ==========================================
    # Train Data Generation
    # ==========================================
    if mode == 'train':
        # 正弦信号：100M到20G；5M为间隔。
        print("1. 生成正弦波 (Train: 100M-20G, step 5M, Depth 10k)...")
        freqs = get_range(100e6, 20e9, 5e6)
        # 频率点数量: ~4000
        # Depth=10000足够长。我们对每个频率切几个片？
        # 如果切片数=1，总共约4000样本。如果切2个，8000样本。
        # v2数据量级大概在 30k-50k 左右。
        # 这里为了保持数据量充足，我们每个频率切 5 个片 (4000 * 5 = 20000 样本)
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean, num_slices=5)

    # ==========================================
    # Test Data Generation
    # ==========================================
    elif mode == 'test':
        # 正弦信号：102.5M到20G；200M为间隔。
        # 频率点数量: ~100
        print("1. 生成正弦波 (Test: 102.5M-20G, step 200M, Depth 10k)...")
        freqs = get_range(102.5e6, 20e9, 200e6)
        # 测试集可以多切一点，比如每个频率 20 片 -> 2000 样本
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean, num_slices=20)

    # ==========================================
    # Save
    # ==========================================
    X = np.array(data_x, dtype=np.float32)
    Y = np.array(data_y, dtype=np.float32)
    
    if np.isnan(X).any() or np.isnan(Y).any():
        print(f"警告：检测到 NaN 值！在 {mode} 数据集中。正在清除...")
        valid_idx = ~(np.isnan(X).any(axis=1) | np.isnan(Y).any(axis=1))
        X = X[valid_idx]
        Y = Y[valid_idx]

    # Expand dims for Conv1d: (N, L) -> (N, L, 1)
    X = X[..., np.newaxis]
    Y = Y[..., np.newaxis]
    
    print(f"数据集 [{mode}] 生成完毕: Shape {X.shape}")
    
    torch.save((torch.from_numpy(X), torch.from_numpy(Y)), 
               os.path.join(SAVE_DIR, f'{mode}_dataset_v3.pth'))
    return X, Y

if __name__ == "__main__":
    X_train, Y_train = generate_dataset('train')
    X_test, Y_test = generate_dataset('test')
    
    # 验证图
    if len(X_test) > 0:
        idx = np.random.randint(0, len(X_test))
        plt.figure()
        plt.plot(X_test[idx].flatten(), alpha=0.7, label='Noisy')
        plt.plot(Y_test[idx].flatten(), alpha=0.7, label='Clean')
        plt.legend()
        plt.title(f'Sample {idx} (Sine Only v3)')
        plt.savefig(os.path.join(SAVE_DIR, 'verify_v3.png'))
        plt.close()
