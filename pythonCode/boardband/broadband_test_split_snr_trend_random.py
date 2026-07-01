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
# 根据 broadband_dataset_split_random.py 生成的文件名
TEST_DATA_PATH = os.path.join(DATA_DIR, "broadband_test_ordered.pkl")
META_DATA_PATH = os.path.join(DATA_DIR, "broadband_metadata_random.pkl")

print(f"Loading test data from {TEST_DATA_PATH}...")
if not os.path.exists(TEST_DATA_PATH):
    print(f"Error: Test data file not found at {TEST_DATA_PATH}")
    exit()

with open(TEST_DATA_PATH, "rb") as f:
    test_data = pickle.load(f)

print(f"Loading metadata from {META_DATA_PATH}...")
if not os.path.exists(META_DATA_PATH):
    print("Error: Metadata file not found! Please re-run the dataset split script (broadband_dataset_split_random.py).")
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

# 确保这里的模型定义与训练时一致
# 在 broadband_train_split.py 中使用的是 GRUCNNModel
model = GRUCNNModel(input_dim, hidden_size_1, output_size).to(device)

# 根据 broadband_train_split.py 保存的模型路径
MODEL_WEIGHTS_PATH = "/home/uestcauto/cyy/boardband/saved_model/broadband_random_100M5G11G20G25M_240.pth"

if not os.path.exists(MODEL_WEIGHTS_PATH):
    print(f"Error: Model weights not found at {MODEL_WEIGHTS_PATH}")
    # 尝试寻找可能的备用模型 (仅供调试)
    fallback_dir = "/home/uestcauto/cyy/boardband/saved_model/"
    print(f"Available models in {fallback_dir}:")
    # os.system(f"ls {fallback_dir}")
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
start_time = time.time()
with torch.no_grad():
    for i in range(0, n_samples, batch_size):
        batch_input = test_input_tensor[i : i + batch_size]
        batch_output = model(batch_input)
        denoised_outputs.append(batch_output.cpu().numpy())
end_time = time.time()
print(f"Inference finished in {end_time - start_time:.2f} seconds.")

denoised_output_np = np.concatenate(denoised_outputs, axis=0)

# --- 步骤 4: 按 metadata 还原频率 SNR ---
test_counts = meta_info['test_counts']
# 新的 dataset 生成逻辑中 metadata 包含 'frequencies'
test_freqs = meta_info.get('frequencies', []) 

total_freqs = len(test_counts)
print(f"Total frequency points verified from metadata: {total_freqs}")

original_snr_list = []
denoised_snr_list = []
freq_indices = []
actual_frequencies = [] # 用于存储实际频率值

current_idx = 0

for i, count in enumerate(test_counts):
    # 如果该频点没有测试数据（count=0），则跳过
    if count == 0:
        continue
    
    start_idx = current_idx
    end_idx = current_idx + count
    
    # 确保不越界
    if end_idx > n_samples:
        print(f"Warning: Index mismatch at freq index {i}. Stopping.")
        break
        
    noisy_segment = X_test[start_idx:end_idx]
    clean_segment = Y_test[start_idx:end_idx]
    denoised_segment = denoised_output_np[start_idx:end_idx]
    
    # Update global index pointer
    current_idx = end_idx
    
    # 计算 SNR
    # 防止分母为0
    noise_power_orig = np.sum((noisy_segment - clean_segment)**2) + 1e-10
    signal_power_orig = np.sum(clean_segment**2) + 1e-10
    snr_orig = 10 * np.log10(signal_power_orig / noise_power_orig)
    
    noise_power_denoised = np.sum((denoised_segment - clean_segment)**2) + 1e-10
    signal_power_denoised = np.sum(clean_segment**2) + 1e-10
    snr_denoised = 10 * np.log10(signal_power_denoised / noise_power_denoised)
    
    original_snr_list.append(snr_orig)
    denoised_snr_list.append(snr_denoised)
    freq_indices.append(i) # Use the index sequence
    
    if test_freqs and i < len(test_freqs):
        actual_frequencies.append(test_freqs[i])

original_mean_snr = np.mean(original_snr_list)
denoised_mean_snr = np.mean(denoised_snr_list)

print(f"Original Mean SNR: {original_mean_snr:.2f} dB")
print(f"Denoised Mean SNR: {denoised_mean_snr:.2f} dB")
print(f"Valid frequencies with test data: {len(freq_indices)} / {total_freqs}")

# --- 步骤 5: 绘图 ---
FIGURES_DIR = '/home/uestcauto/cyy/boardband/figures_new'
SPLIT_FIGURES_DIR = '/home/uestcauto/cyy/boardband/figures_new/splitfigures_random'
os.makedirs(FIGURES_DIR, exist_ok=True)
os.makedirs(SPLIT_FIGURES_DIR, exist_ok=True)

# 绘图 1: 使用 Index 作为 X 轴
plt.figure(figsize=(12, 6))
plt.plot(freq_indices, original_snr_list, label=f'Original SNR (Mean: {original_mean_snr:.2f} dB)', color='red', alpha=0.6, linewidth=1)
plt.plot(freq_indices, denoised_snr_list, label=f'Denoised SNR (Mean: {denoised_mean_snr:.2f} dB)', color='blue', alpha=0.8, linewidth=1)

# Trend lines (Index based)
if len(freq_indices) > 0:
    z_orig = np.polyfit(freq_indices, original_snr_list, 1)
    p_orig = np.poly1d(z_orig)
    plt.plot(freq_indices, p_orig(freq_indices), "r--", alpha=0.8, linewidth=2, label="Original Trend")

    z_denoised = np.polyfit(freq_indices, denoised_snr_list, 1)
    p_denoised = np.poly1d(z_denoised)
    plt.plot(freq_indices, p_denoised(freq_indices), "b--", alpha=0.8, linewidth=2, label="Denoised Trend")

plt.title("SNR vs Frequency Index (Random Split Dataset)")
plt.xlabel(f"Frequency Point Index (Total {total_freqs})")
plt.ylabel("SNR (dB)")
plt.legend(loc="best")
plt.grid(True, which="both", linestyle='--', linewidth=0.5)

save_path_idx = os.path.join(FIGURES_DIR, "broadband_snr_trend_random_index.png")
plt.savefig(save_path_idx, dpi=300, bbox_inches='tight')
print(f"SNR trend plot (Index) saved to {save_path_idx}")
# plt.show() # 如果在服务器上运行，通常不直接show

# 绘图 2: 如果有实际频率值，使用 Frequency (MHz) 作为 X 轴
if actual_frequencies:
    plt.figure(figsize=(12, 6))
    
    # 频率可能是整数或浮点数
    freqs_mhz = np.array(actual_frequencies)
    
    plt.plot(freqs_mhz, original_snr_list, label=f'Original SNR', color='red', alpha=0.6, linewidth=1)
    plt.plot(freqs_mhz, denoised_snr_list, label=f'Denoised SNR', color='blue', alpha=0.8, linewidth=1)
    
    # Trend lines (Freq based)
    z_orig_f = np.polyfit(freqs_mhz, original_snr_list, 1)
    p_orig_f = np.poly1d(z_orig_f)
    plt.plot(freqs_mhz, p_orig_f(freqs_mhz), "r--", alpha=0.8, linewidth=2)

    z_denoised_f = np.polyfit(freqs_mhz, denoised_snr_list, 1)
    p_denoised_f = np.poly1d(z_denoised_f)
    plt.plot(freqs_mhz, p_denoised_f(freqs_mhz), "b--", alpha=0.8, linewidth=2)

    plt.title("SNR vs Frequency (MHz)")
    plt.xlabel("Frequency (MHz)")
    plt.ylabel("SNR (dB)")
    plt.legend(loc="best")
    plt.grid(True, which="both", linestyle='--', linewidth=0.5)
    
    save_path_freq = os.path.join(FIGURES_DIR, "broadband_snr_trend_random_freq.png")
    plt.savefig(save_path_freq, dpi=300, bbox_inches='tight')
    print(f"SNR trend plot (Freq) saved to {save_path_freq}")


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
if not valid_indices:
    print("No valid indices found for plotting details.")
    exit()

max_idx = max(valid_indices)
min_idx = min(valid_indices)

# 随机策略：从整体中随机选取几个点，以及头部、尾部、中间
def safe_sample(candidates, k=5):
    if len(candidates) < k:
        return candidates
    return np.random.choice(candidates, k, replace=False)

# 低频部分
low_freq_candidates = [i for i in valid_indices if i < min_idx + 50]
# 高频部分
high_freq_candidates = [i for i in valid_indices if i > max_idx - 50]
# 中间部分
mid_candidates = [i for i in valid_indices if (min_idx + 50) <= i <= (max_idx - 50)]

selected_low = safe_sample(low_freq_candidates, 3)
selected_high = safe_sample(high_freq_candidates, 3)
selected_mid = safe_sample(mid_candidates, 3)

print(f"Selected Low Freq Indices: {selected_low}")
print(f"Selected Mid Freq Indices: {selected_mid}")
print(f"Selected High Freq Indices: {selected_high}")

def plot_and_save_details(freq_idx, category):
    try:
        start, end = slice_map[freq_idx]
        # 取该频点的第1个切片进行绘图
        slice_offset = 0 
        
        # 数据 shape: (N, 240, 1) -> flatten to (240,)
        s_noisy = X_test[start + slice_offset, :, 0]
        s_clean = Y_test[start + slice_offset, :, 0]
        s_denoised = denoised_output_np[start + slice_offset, :, 0]
        
        # 获取该点的 SNR 以标注
        if freq_idx in freq_indices:
            idx_in_list = freq_indices.index(freq_idx)
            snr_orig = original_snr_list[idx_in_list]
            snr_den = denoised_snr_list[idx_in_list]
            freq_val = test_freqs[freq_idx] if test_freqs and freq_idx < len(test_freqs) else "N/A"
        else:
            snr_orig = 0
            snr_den = 0
            freq_val = "N/A"
        
        # --- 1. 频谱图 ---
        window = np.hanning(len(s_noisy))
        fft_noisy = 20 * np.log10(np.abs(np.fft.rfft(s_noisy * window)) + 1e-10)
        fft_clean = 20 * np.log10(np.abs(np.fft.rfft(s_clean * window)) + 1e-10)
        fft_denoised = 20 * np.log10(np.abs(np.fft.rfft(s_denoised * window)) + 1e-10)
        
        plt.figure(figsize=(10, 6))
        plt.plot(fft_noisy, label='Original Noisy', color='red', alpha=0.5)
        plt.plot(fft_denoised, label='Denoised', color='blue', alpha=0.8)
        plt.plot(fft_clean, label='Clean Ref', color='black', linestyle='--', alpha=0.5)
        
        plt.title(f"Spectrum - {category} - Index {freq_idx} (Freq: {freq_val} MHz)\nOrig SNR: {snr_orig:.2f}dB | Denoised SNR: {snr_den:.2f}dB")
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
        
        plt.title(f"Time Domain - {category} - Index {freq_idx} (Freq: {freq_val} MHz)\nOrig SNR: {snr_orig:.2f}dB | Denoised SNR: {snr_den:.2f}dB")
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

for idx in selected_mid:
    plot_and_save_details(idx, "Mid_Freq")

print(f"All detail plots saved to {SPLIT_FIGURES_DIR}")
print("Test script completed.")
