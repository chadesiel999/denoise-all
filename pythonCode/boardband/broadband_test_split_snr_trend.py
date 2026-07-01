import numpy as np
import matplotlib.pylab as plt
import time
import torch
import torch.nn as nn
import os
import pickle
import re

from models import *

# --- 步骤 0: 初始化 ---
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# --- 步骤 1: 加载 SPLIT 测试数据集 和 元数据 ---
DATA_DIR = "/home/uestcauto/cyy/boardband/dataset"
TEST_DATA_PATH = os.path.join(DATA_DIR, "broadband_testno2.pkl")
META_DATA_PATH = os.path.join(DATA_DIR, "broadband_metadatano2.pkl")

print(f"Loading test data from {TEST_DATA_PATH}...")
with open(TEST_DATA_PATH, "rb") as f:
    test_data = pickle.load(f)

print(f"Loading metadata from {META_DATA_PATH}...")
if not os.path.exists(META_DATA_PATH):
    print("Error: Metadata file not found! Please re-run the dataset split script.")
    exit()

with open(META_DATA_PATH, "rb") as f:
    meta_info = pickle.load(f)

# Data shape: (N, 2, 240, 1) -> Split X (input), Y (label)
X_test = test_data[:, 0, :, :]
Y_test = test_data[:, 1, :, :]

print(f"Test data shape: {X_test.shape}")
n_samples = X_test.shape[0]

# --- 步骤 2: 加载模型 ---
input_dim = 1
hidden_size_1 = 32
output_size = 1

model = GRUCNNModel(input_dim, hidden_size_1, output_size).to(device)

MODEL_WEIGHTS_PATH = "/home/uestcauto/cyy/boardband/saved_model/broadband_split_100M20G25M_240.pth"

if not os.path.exists(MODEL_WEIGHTS_PATH):
    fallback_path = "/home/uestcauto/cyy/boardband/saved_model/broadband_split_2G10G15M_240.pth"
    if os.path.exists(fallback_path):
        MODEL_WEIGHTS_PATH = fallback_path
    else:
        print(f"Error: Model weights not found at {MODEL_WEIGHTS_PATH}")
        exit()

print(f"Loading weights from {MODEL_WEIGHTS_PATH}")
state_dict = torch.load(MODEL_WEIGHTS_PATH, map_location=device)
model.load_state_dict(state_dict)
model.eval()

# --- 步骤 3: 批量推理 ---
batch_size = 1000
test_input_tensor = torch.from_numpy(X_test).type(torch.FloatTensor).to(device)
denoised_outputs = []

print("Running inference...")
with torch.no_grad():
    for i in range(0, n_samples, batch_size):
        batch_input = test_input_tensor[i : i + batch_size]
        batch_output = model(batch_input)
        denoised_outputs.append(batch_output.cpu().numpy())

denoised_output_np = np.concatenate(denoised_outputs, axis=0)

# --- 步骤 4: 按 metadata 还原频率 SNR ---
test_counts = meta_info['test_counts']
total_freqs = len(test_counts)
print(f"Total frequency points verified from metadata: {total_freqs}")

original_snr_list = []
denoised_snr_list = []
freq_indices = []

current_idx = 0

for i, count in enumerate(test_counts):
    # 如果该频点没有测试数据（count=0），则跳过或标记为NaN
    if count == 0:
        continue
    
    start_idx = current_idx
    end_idx = current_idx + count
    
    # 确保不越界
    if end_idx > n_samples:
        print(f"Warning: Index mismatch at freq {i}. Stopping.")
        break
        
    noisy_segment = X_test[start_idx:end_idx]
    clean_segment = Y_test[start_idx:end_idx]
    denoised_segment = denoised_output_np[start_idx:end_idx]
    
    # Update global index pointer
    current_idx = end_idx
    
    # 计算 SNR
    noise_power_orig = np.sum((noisy_segment - clean_segment)**2) + 1e-10
    signal_power_orig = np.sum(clean_segment**2) + 1e-10
    snr_orig = 10 * np.log10(signal_power_orig / noise_power_orig)
    
    noise_power_denoised = np.sum((denoised_segment - clean_segment)**2) + 1e-10
    signal_power_denoised = np.sum(clean_segment**2) + 1e-10
    snr_denoised = 10 * np.log10(signal_power_denoised / noise_power_denoised)
    
    original_snr_list.append(snr_orig)
    denoised_snr_list.append(snr_denoised)
    freq_indices.append(i) # Use the real frequency index from metadata

original_mean_snr = np.mean(original_snr_list)
denoised_mean_snr = np.mean(denoised_snr_list)

print(f"Original Mean SNR: {original_mean_snr:.2f} dB")
print(f"Denoised Mean SNR: {denoised_mean_snr:.2f} dB")
print(f"Valid frequencies with test data: {len(freq_indices)} / {total_freqs}")

# --- 步骤 5: 绘图 ---
FIGURES_DIR = '/home/uestcauto/cyy/boardband/figures_new'
SPLIT_FIGURES_DIR = '/home/uestcauto/cyy/boardband/figures_new/splitfigures'
os.makedirs(FIGURES_DIR, exist_ok=True)
os.makedirs(SPLIT_FIGURES_DIR, exist_ok=True)

plt.figure(figsize=(12, 6))
# Plot scatter or line? Line is fine if we skip NaNs, but indices are explicit now.
plt.plot(freq_indices, original_snr_list, label=f'Original SNR (Mean: {original_mean_snr:.2f} dB)', color='red', alpha=0.6, linewidth=1)
plt.plot(freq_indices, denoised_snr_list, label=f'Denoised SNR (Mean: {denoised_mean_snr:.2f} dB)', color='blue', alpha=0.8, linewidth=1)

# Trend lines
z_orig = np.polyfit(freq_indices, original_snr_list, 1)
p_orig = np.poly1d(z_orig)
plt.plot(freq_indices, p_orig(freq_indices), "r--", alpha=0.8, linewidth=2, label="Original Trend")

z_denoised = np.polyfit(freq_indices, denoised_snr_list, 1)
p_denoised = np.poly1d(z_denoised)
plt.plot(freq_indices, p_denoised(freq_indices), "b--", alpha=0.8, linewidth=2, label="Denoised Trend")

plt.title("SNR vs Frequency Index (Aligned)")
plt.xlabel(f"Frequency Point Index (Total {total_freqs})")
plt.ylabel("SNR (dB)")
plt.legend(loc="best")
plt.grid(True, which="both", linestyle='--', linewidth=0.5)

save_path = os.path.join(FIGURES_DIR, "broadband_snr_trend_split_aligned.png")
plt.savefig(save_path, dpi=300, bbox_inches='tight')
print(f"SNR trend plot saved to {save_path}")
plt.show()

# --- 步骤 6: 附加功能 - 频谱绘制与分析 ---
print("\n--- Generating additional spectrum plots ---")

# 1. 建立索引映射 (Freq Index -> Array Slice Range)
slice_map = {}
curr = 0
for idx, count in enumerate(test_counts):
    if count > 0:
        slice_map[idx] = (curr, curr + count)
        curr += count

# 2. 定义选取范围
valid_indices = list(slice_map.keys())
max_idx = max(valid_indices)
min_idx = min(valid_indices)

# 低频部分: 取前50个有效点中的随机5个
low_freq_candidates = [i for i in valid_indices if i < min_idx + 50]
# 高频部分: 取最后50个有效点中的随机5个
high_freq_candidates = [i for i in valid_indices if i > max_idx - 50]
# 跌落区域 1: 210~270
drop_range_1_candidates = [i for i in valid_indices if 210 <= i <= 270]
# 跌落区域 2: 390~440
drop_range_2_candidates = [i for i in valid_indices if 390 <= i <= 440]

def safe_sample(candidates, k=5):
    if len(candidates) < k:
        return candidates
    return np.random.choice(candidates, k, replace=False)

selected_low = safe_sample(low_freq_candidates, 5)
selected_high = safe_sample(high_freq_candidates, 5)
selected_drop_1 = safe_sample(drop_range_1_candidates, 5)
selected_drop_2 = safe_sample(drop_range_2_candidates, 5)

print(f"Selected Low Freq Indices: {selected_low}")
print(f"Selected High Freq Indices: {selected_high}")
print(f"Selected Drop Region 1 Indices: {selected_drop_1}")
print(f"Selected Drop Region 2 Indices: {selected_drop_2}")

def plot_and_save_details(freq_idx, category):
    try:
        start, end = slice_map[freq_idx]
        # 取该频点的第1个切片进行绘图 (或者随机一个)
        slice_offset = 0 
        
        # 数据 shape: (N, 240, 1) -> flatten to (240,)
        s_noisy = X_test[start + slice_offset, :, 0]
        s_clean = Y_test[start + slice_offset, :, 0]
        s_denoised = denoised_output_np[start + slice_offset, :, 0]
        
        # 获取该点的 SNR 以标注
        idx_in_list = freq_indices.index(freq_idx)
        snr_orig = मूल_list = original_snr_list[idx_in_list] # Original (with bug kept for context but overwritten)
        snr_orig = original_snr_list[idx_in_list] # Corrected
        snr_den = denoised_snr_list[idx_in_list]
        
        # --- 1. 频谱图 ---
        # 计算 FFT
        window = np.hanning(len(s_noisy))
        fft_noisy = 20 * np.log10(np.abs(np.fft.rfft(s_noisy * window)) + 1e-10)
        fft_clean = 20 * np.log10(np.abs(np.fft.rfft(s_clean * window)) + 1e-10)
        fft_denoised = 20 * np.log10(np.abs(np.fft.rfft(s_denoised * window)) + 1e-10)
        
        plt.figure(figsize=(10, 6))
        plt.plot(fft_noisy, label='Original Noisy', color='red', alpha=0.5)
        plt.plot(fft_denoised, label='Denoised', color='blue', alpha=0.8)
        plt.plot(fft_clean, label='Clean Ref', color='black', linestyle='--', alpha=0.5)
        
        plt.title(f"Spectrum Analysis - {category} - Freq Index {freq_idx}\nOriginal SNR: {snr_orig:.2f}dB | Denoised SNR: {snr_den:.2f}dB")
        plt.xlabel("Frequency Bin")
        plt.ylabel("Magnitude (dB)")
        plt.legend()
        plt.grid(True, alpha=0.3)
        
        filename_spec = f"{category}_idx{freq_idx}_spectrum.png"
        plt.savefig(os.path.join(SPLIT_FIGURES_DIR, filename_spec), dpi=150)
        plt.close()

        # --- 2. 时域图 ---
        plt.figure(figsize=(10, 6))
        plt.plot(s_noisy, label='Original Noisy', color='red', alpha=0.5)
        plt.plot(s_denoised, label='Denoised', color='blue', alpha=0.8)
        plt.plot(s_clean, label='Clean Ref', color='black', linestyle='--', alpha=0.5)
        
        plt.title(f"Time Domain - {category} - Freq Index {freq_idx}\nOriginal SNR: {snr_orig:.2f}dB | Denoised SNR: {snr_den:.2f}dB")
        plt.xlabel("Sample Index")
        plt.ylabel("Amplitude")
        plt.legend()
        plt.grid(True, alpha=0.3)
        
        filename_time = f"{category}_idx{freq_idx}_time.png"
        plt.savefig(os.path.join(SPLIT_FIGURES_DIR, filename_time), dpi=150)
        plt.close()
        
    except Exception as e:
        print(f"Error plotting {freq_idx}: {e}")

# 执行绘图
for idx in selected_low:
    plot_and_save_details(idx, "Low_Freq")

for idx in selected_high:
    plot_and_save_details(idx, "High_Freq")

for idx in selected_drop_1:
    plot_and_save_details(idx, "Drop_Region_1")

for idx in selected_drop_2:
    plot_and_save_details(idx, "Drop_Region_2")

print(f"All detail plots saved to {SPLIT_FIGURES_DIR}")

