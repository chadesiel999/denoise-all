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
DEPTH = 2048       # 原始信号生成长度 (降低深度，从 10k -> 2k，适配切片需求)
SAMPLE_LEN = 512   # 最终训练用的单样本长度
# NUM_SLICES 将作为动态参数，不再使用全局固定值
TIME_VECTOR = np.arange(DEPTH) / FS 

# 路径配置
SAVE_DIR = './generated_data_v2'
if not os.path.exists(SAVE_DIR):
    os.makedirs(SAVE_DIR)

print(f"配置: 采样率={FS/1e9} GSPS, 原始长度={DEPTH}, 切片长度={SAMPLE_LEN}")

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
        # 避免全0信号除零错误
        sig_power = np.mean(clean_sig ** 2)
        if sig_power == 0:
            noise_power = 0.04 # 默认噪声水平
        else:
            noise_power = sig_power / (10 ** (snr / 10))
            
        noise = np.random.normal(0, np.sqrt(noise_power), clean_sig.shape)
        # 原代码中使用固定std 0.04? 这里改为基于SNR计算，或保留原逻辑
        # 原代码: noise = np.random.normal(0, 0.04, clean_sig.shape)
        # 既然有snr_range参数，理应使用基于SNR的噪声，但为了保持幅度一致性，
        # 如果原始信号归一化了，噪声固定也是一种策略。
        # 这里遵循物理SNR定义生成:
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
        # y(t) = [1 + m * cos(wm*t)] * cos(wc*t)
        mod_sig = np.cos(2 * np.pi * mod_freq * self.t)
        carrier_sig = np.cos(2 * np.pi * carrier_freq * self.t)
        am_sig = (1 + depth * mod_sig) * carrier_sig
        # 归一化幅度
        return am_sig / (1 + depth)

    # --- 数字调制 (含成型滤波) ---
    def gen_digital(self, carrier_freq, symbol_rate, mod_type, alpha=0.5):
        # 1. 计算参数
        sps = int(self.fs / symbol_rate) # Samples per symbol
        if sps < 1: sps = 1
        num_symbols = int(len(self.t) / sps) + 10
        
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
            sqrt_M = int(np.sqrt(M))
            if mod_type == '128QAM': 
                # 128QAM非正方形，做特殊近似 (Generic Random Constellation points for sim)
                real = np.random.randint(-11, 12, num_symbols)
                imag = np.random.randint(-11, 12, num_symbols)
                symbols = real + 1j * imag
            else:
                real = 2 * (ints % sqrt_M) - (sqrt_M - 1)
                imag = 2 * (ints // sqrt_M) - (sqrt_M - 1)
                symbols = real + 1j * imag
        
        # 3. 脉冲成型 (RRC Filter)
        upsampled = np.zeros(len(symbols) * sps, dtype=complex)
        upsampled[::sps] = symbols
        
        ntaps = 8 * sps 
        t_rrc = np.arange(-ntaps//2, ntaps//2) / sps
        
        # 奇点处理
        mask_0 = np.isclose(t_rrc, 0)
        t_rrc[mask_0] = 1e-8 
        denom_zeros = np.isclose(np.abs(4*alpha*t_rrc), 1.0)
        t_rrc[denom_zeros] += 1e-6
        
        num = np.sin(np.pi * t_rrc * (1-alpha)) + 4*alpha*t_rrc * np.cos(np.pi * t_rrc * (1+alpha))
        denom = np.pi * t_rrc * (1 - (4*alpha*t_rrc)**2)
        rrc_filter = num / denom
        
        baseband = np.convolve(upsampled, rrc_filter, mode='same')
        baseband = baseband[:len(self.t)]
        if len(baseband) < len(self.t):
            baseband = np.pad(baseband, (0, len(self.t)-len(baseband)))
            
        # 4. 上变频
        carrier_i = np.cos(2 * np.pi * carrier_freq * self.t)
        carrier_q = np.sin(2 * np.pi * carrier_freq * self.t)
        
        rf_sig = baseband.real * carrier_i - baseband.imag * carrier_q
        
        scale = np.max(np.abs(rf_sig))
        if scale > 0:
            rf_sig /= scale
            
        return rf_sig

# ==========================================
# 3. 数据集生成逻辑
# ==========================================
def generate_dataset(mode='train'):
    gen = SignalCore()
    data_x = [] # Noisy
    data_y = [] # Clean
    
    print(f"\n>>> 开始生成 [{mode.upper()}] 数据集...")
    
    def get_range(start, end, step):
        return np.arange(start, end + step/1000.0, step)

    def process_and_append(clean_sig, num_slices=1):
        max_val = np.max(np.abs(clean_sig))
        if max_val > 0:
            clean_sig = clean_sig / max_val
            
        count = 0
        # 尝试多次切片，直到满足 num_slices
        # 如果长度不够切出非重叠，就允许重叠或随机
        for _ in range(num_slices):
            if len(clean_sig) <= SAMPLE_LEN:
                continue
            start_idx = np.random.randint(0, len(clean_sig) - SAMPLE_LEN)
            segment_clean = clean_sig[start_idx : start_idx + SAMPLE_LEN]
            
            segment_noisy = gen.add_noise(segment_clean)
            
            data_x.append(segment_noisy)
            data_y.append(segment_clean)

    # ==========================================
    # Train Data Generation
    # ==========================================
    if mode == 'train':
        # 1. 正弦信号：100M到20G；5M为间隔. 
        # Points ~3980. Slices=1 -> ~4000. Ratio ~16%
        print("1. 生成正弦波 (Train: 100M-20G, step 5M)...")
        freqs = get_range(100e6, 20e9, 5e6)
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean, num_slices=1)

        # 2. 方波：100M-20G, 50M为间隔.
        # Points ~399 * 2 = 798. Slices=5 -> ~4000. Ratio ~16%
        print("2. 生成方波 (Train: 100M-20G, step 50M)...")
        freqs = get_range(100e6, 20e9, 50e6)
        for f in freqs:
            for duty in [0.1, 0.5]:
                clean = gen.gen_square(f, duty)
                process_and_append(clean, num_slices=5)

        # 3. 三角波：100M-20G, 50M为间隔
        # Points ~399. Slices=5 -> ~2000. Ratio ~8%
        print("3. 生成三角波 (Train: 100M-20G, step 50M)...")
        freqs = get_range(100e6, 20e9, 50e6)
        for f in freqs:
            clean = gen.gen_triangle(f)
            process_and_append(clean, num_slices=5)

        # 4. 锯齿波：100M-20G, 50M为间隔
        # Points ~399. Slices=5 -> ~2000. Ratio ~8%
        print("4. 生成锯齿波 (Train: 100M-20G, step 50M)...")
        freqs = get_range(100e6, 20e9, 50e6)
        for f in freqs:
            clean = gen.gen_sawtooth(f)
            process_and_append(clean, num_slices=5)
            
        # 5. 其余周期信号 (Assuming Dual Tone)：100M到2G；10M为间隔
        # Points ~190. Slices=10 -> ~1900. Ratio ~8%
        print("5. 生成其余周期信号/DualTone (Train: 100M-2G, step 10M)...")
        freqs = get_range(100e6, 2e9, 10e6)
        for f in freqs:
            clean = gen.gen_dualtone(f)
            process_and_append(clean, num_slices=10)

        # 6. AM调制：1G-18G，间隔50M，调制波20M、100M，调制深度100%和10%
        # Carriers ~340. Total Seeds ~1360. Slices=8 -> ~10880. Ratio ~44%
        print("6. 生成AM信号 (Train: 1G-18G, step 50M)...")
        carriers = get_range(1e9, 18e9, 50e6)

        mod_freqs = [20e6, 100e6]
        depths = [1.0, 0.1]
        
        for fc in carriers:
            for fm in mod_freqs:
                for d in depths:
                    clean = gen.gen_am(fc, fm, d)
                    process_and_append(clean, num_slices=8)

    # ==========================================
    # Test Data Generation
    # ==========================================
    elif mode == 'test':
        # 1. 正弦信号：102.5M到20G；300M为间隔
        # Points ~67. Slices=20 -> ~1340
        print("1. 生成正弦波 (Test: 102.5M-20G, step 300M)...")
        freqs = get_range(102.5e6, 20e9, 300e6)
        for f in freqs:
            clean = gen.gen_sine(f)
            process_and_append(clean, num_slices=20)

        # 2. 方波：105M-20G, 300M为间隔, 占空比 0.1, 0.5, 0.3
        # Points ~67 * 3 = 201. Slices=20 -> ~4020
        print("2. 生成方波 (Test: 105M-20G, step 300M)...")
        freqs = get_range(105e6, 20e9, 300e6)
        for f in freqs:
            for duty in [0.1, 0.5, 0.3]:
                clean = gen.gen_square(f, duty)
                process_and_append(clean, num_slices=20)

        # 3. 三角波：105M-20G, 300M为间隔
        # Points ~67. Slices=20 -> ~1340
        print("3. 生成三角波 (Test: 105M-20G, step 300M)...")
        freqs = get_range(105e6, 20e9, 300e6)
        for f in freqs:
            clean = gen.gen_triangle(f)
            process_and_append(clean, num_slices=20)

        # 4. 锯齿波：105M-20G, 300M为间隔
        # Points ~67. Slices=20 -> ~1340
        print("4. 生成锯齿波 (Test: 105M-20G, step 300M)...")
        freqs = get_range(105e6, 20e9, 300e6)
        for f in freqs:
            clean = gen.gen_sawtooth(f)
            process_and_append(clean, num_slices=20)

        # 5. 双音信号：100M到20G，500M为间隔，幅度1:1，freq2 = 1.5freq1
        # Points ~40. Slices=20 -> ~800
        print("5. 生成双音信号 (Test: 100M-20G, step 500M)...")
        freqs = get_range(100e6, 20e9, 500e6)
        for f in freqs:
            # 确保 freq2 < FS/2 (Nyquist)
            if 1.5 * f < FS/2:
                clean = gen.gen_dualtone(f)
                process_and_append(clean, num_slices=20)

        # 6. 其余周期信号 (Rest of periodic signals)：100M到20G，400M为间隔
        # Points ~50. Slices=20 -> ~1000
        print("6. 生成其余周期信号 (Test: 100M-20G, step 400M)...")
        freqs = get_range(100e6, 20e9, 400e6)
        for f in freqs:
            if 1.5 * f < FS/2:
                clean = gen.gen_dualtone(f)
                process_and_append(clean, num_slices=20)

        # 7. 数字调制：载波2G-15G，500M为间隔，符号率200M，500M
        # Carriers ~27. Total seeds = 27*2*7 = 378. Slices=15 -> ~5670.
        print("7. 生成数字调制 (Test: 2G-15G, step 500M)...")
        carriers = get_range(2e9, 15e9, 500e6)
        sym_rates = [200e6, 500e6]
        mod_types = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM', '128QAM', '256QAM']
        
        for fc in carriers:
            for sym in sym_rates:
                for mod in mod_types:
                    clean = gen.gen_digital(fc, sym, mod, alpha=0.5)
                    process_and_append(clean, num_slices=15)

        # 8. AM调制：载波1G-19G，间隔500M，调制波20M，200M，400M，调制深度100%和50%
        # Carriers ~37. Total Seeds = 37*3*2 = 222. Slices=25 -> ~5550.
        print("8. 生成AM信号 (Test: 1G-19G, step 500M)...")
        carriers = get_range(1e9, 19e9, 500e6)
        mod_freqs = [20e6, 200e6, 400e6]
        depths = [1.0, 0.5]
        
        for fc in carriers:
            for fm in mod_freqs:
                for d in depths:
                    clean = gen.gen_am(fc, fm, d)
                    process_and_append(clean, num_slices=25)

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

    # (N, L, 1) to match Conv1d input requirement locally
    X = X[..., np.newaxis]
    Y = Y[..., np.newaxis]
    
    print(f"数据集 [{mode}] 生成完毕: Shape {X.shape}")
    
    torch.save((torch.from_numpy(X), torch.from_numpy(Y)), 
               os.path.join(SAVE_DIR, f'{mode}_dataset_v2.pth'))
    return X, Y

if __name__ == "__main__":
    X_train, Y_train = generate_dataset('train')
    X_test, Y_test = generate_dataset('test')
    
    # 验证图
    idx = np.random.randint(0, len(X_test))
    plt.figure()
    plt.plot(X_test[idx].flatten(), alpha=0.7, label='Noisy')
    plt.plot(Y_test[idx].flatten(), alpha=0.7, label='Clean')
    plt.legend()
    plt.title(f'Sample {idx}')
    plt.savefig(os.path.join(SAVE_DIR, 'verify_v2.png'))
    plt.close()
