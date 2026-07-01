import numpy as np
import matplotlib.pylab as plt
import time
import torch
import torch.nn as nn
import os  # 导入 os 模块
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches  # 导入patches模块
import pickle  # 导入 pickle 模块

# 从 models.py 导入你训练时使用的模型
from models import *
# 导入 zelin_dataset.py 以使用其中的数据加载函数
import zelin_dataset


# --- 步骤 0: 初始化 ---
# 确保使用 GPU 进行计算
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# --- 步骤 1: 加载与训练时相同的数据集 ---
# 使用 zelin_dataset.py 中的函数来加载并划分数据。
# 这个路径必须指向你之前生成的 .pkl 文件。
DATASET_PATH = "/home/uestcauto/cyy/boardband/dataset/broadband_dataset_2G10G15M.pkl"
try:
    (X_train, Y_train), (X_val, Y_val), (X_test, Y_test) = zelin_dataset.load_broadband_denoise(DATASET_PATH)
except FileNotFoundError:
    print(f"Error: Dataset file not found at {DATASET_PATH}")
    print("Please make sure the path is correct and the file exists.")
    exit()

print(f"Test data shape: {X_test.shape}")
n_samples = X_test.shape[0]

# --- 步骤 2: 构建与训练时完全相同的模型 ---
# 定义与训练时相同的模型超参数
input_dim = 1
hidden_size_1 = 32  # 必须与训练时的 32 保持一致
output_size = 1

# 实例化正确的模型，并将其移动到 GPU
# model = TinyCNNModel(input_dim, hidden_size_1, output_size).to(device)

model = GRUCNNModel(input_dim, hidden_size_1, output_size).to(device)

# --- 步骤 3: 加载训练好的模型权重 ---
# 这个路径必须指向你之前训练生成的 .pth 文件。
MODEL_WEIGHTS_PATH = "/home/uestcauto/cyy/boardband/saved_model/broadband_dataset_2G10G15M_240.pth"
try:
    state_dict = torch.load(MODEL_WEIGHTS_PATH, map_location=device)
    model.load_state_dict(state_dict)
except FileNotFoundError:
    print(f"Error: Model weights file not found at {MODEL_WEIGHTS_PATH}")
    print("Please make sure you have trained the model and the path is correct.")
    exit()

# 将模型设置为评估模式（这会关闭 dropout 等训练特有的层）
model.eval()
print("Model loaded successfully.")

# --- 步骤 4: 准备测试数据并进行推理 ---
# 从测试集中随机选择 100 个样本进行评估
# 如果测试样本总数少于100，则选择所有样本
num_to_test = min(100, n_samples)
choice_idx = np.random.choice(range(n_samples), size=num_to_test, replace=False)

ground_truth_np = Y_test[choice_idx].astype(np.float32)
test_input_np = X_test[choice_idx].astype(np.float32)

# 将 Numpy 数组转换为 PyTorch 张量并移动到 GPU
test_input_tensor = torch.from_numpy(test_input_np).to(device)

print(f"\nRunning inference on {num_to_test} samples...")
start_time = time.time()
# 使用模型进行推理（降噪）
# 在评估模式下，我们不需要计算梯度，这样可以节省计算资源
with torch.no_grad():
    denoised_output_tensor = model(test_input_tensor)
end_time = time.time()
print(f'Inference time: {end_time - start_time:.4f}s')

# 将结果从 GPU 转回 CPU，并转换为 Numpy 数组，以便后续计算和绘图
denoised_output_np = denoised_output_tensor.cpu().numpy()

# --- 步骤 5: 可视化结果与计算信噪比 ---

# 1. 计算并打印所有被选中测试样本的平均信噪比（SNR）
print("\n--- Average SNR for all chosen samples ---")
# 原始带噪信号的信噪比
original_snr_all = 10 * np.log10(np.sum(ground_truth_np**2) / np.sum((test_input_np - ground_truth_np)**2))
print(f'Original Average SNR (dB): {original_snr_all:.4f}')

# 经过模型降噪后的信号的信噪比
denoised_snr_all = 10 * np.log10(np.sum(ground_truth_np**2) / np.sum((denoised_output_np - ground_truth_np)**2))
print(f'Denoised Average SNR (dB): {denoised_snr_all:.4f}')
snr_improvement = denoised_snr_all - original_snr_all
print(f'SNR Improvement (dB): {snr_improvement:.4f}')


# 2. 选择第一个样本进行可视化
t = 4
sample_noisy = test_input_np[t, :, 0]
sample_denoised = denoised_output_np[t, :, 0]
sample_clean = ground_truth_np[t, :, 0]

# # --- 修改开始：使用绝对路径 ---
# # 获取当前脚本文件所在的目录
# script_dir = os.path.dirname(os.path.abspath(__file__))
# # 构造绝对路径
# FIGURES_DIR = os.path.join(script_dir, 'figures')
# os.makedirs(FIGURES_DIR, exist_ok=True)
# # --- 修改结束 ---

# --- 新增代码：自动创建保存图片的文件夹 ---
FIGURES_DIR = './figures'
os.makedirs(FIGURES_DIR, exist_ok=True)
# --- 结束新增代码 ---

# 创建要显示的SNR文本-1127
snr_text = (f'Original Average SNR: {original_snr_all:.2f} dB\n'
            f'Denoised Average SNR: {denoised_snr_all:.2f} dB\n'
            f'SNR Improvement: {snr_improvement:.2f} dB')

# 绘制时域信号对比图
plt.figure(figsize=(12, 6))
plt.title("Time Domain Signal Comparison (Sample 0)")
plt.plot(sample_noisy, color='red', linestyle='-', label="Noisy Signal", alpha=0.7)
plt.plot(sample_denoised, color='blue', linestyle='-', label="Denoised Signal (Model Output)")
plt.plot(sample_clean, color='black', linestyle='--', label="Ground Truth (Clean Signal)")
plt.xlabel("Sample Index")
plt.ylabel("Normalized Amplitude")
plt.legend(loc='upper right')
plt.grid(True)

# 在左上角添加SNR信息文本框
props = dict(boxstyle='round', facecolor='white', alpha=0.8)
plt.gca().text(0.02, 0.98, snr_text, transform=plt.gca().transAxes, fontsize=10,
               verticalalignment='top', bbox=props)

plt.savefig(os.path.join(FIGURES_DIR, "broadband_denoise_signal_2G10G15M_240_comparison.png"), transparent=False, bbox_inches="tight")
plt.show()

# 绘制频域信号对比图
plt.figure(figsize=(12, 6))
plt.title("Frequency Domain Signal Comparison (Sample 0)")
# 计算FFT并取绝对值
length = len(sample_noisy)
window = np.hanning(length)
noisy_window = np.fft.rfft(sample_noisy * window)
denoised_window = np.fft.rfft(sample_denoised * window)
clean_window = np.fft.rfft(sample_clean * window)
fft_noisy = 20*np.log10(np.abs(noisy_window))
fft_denoised = 20*np.log10(np.abs(denoised_window))
fft_clean = 20*np.log10(np.abs(clean_window))
plt.plot(fft_noisy, color='red', linestyle='-', label="Noisy Signal Spectrum", alpha=0.7)
plt.plot(fft_denoised, color='blue', linestyle='-', label="Denoised Signal Spectrum")
plt.plot(fft_clean, color='black', linestyle='--', label="Ground Truth Spectrum")
plt.xlabel("Frequency Bin")
plt.ylabel("Magnitude")
plt.legend(loc='best')
plt.grid(True)
plt.savefig(os.path.join(FIGURES_DIR, 'broadband_denoise_freq_2G10G15M_240_comparison.png'), transparent=False, bbox_inches="tight")
plt.show()


# 在左上角添加SNR信息文本框 (为频域图也添加)
# plt.gca().text(0.02, 0.98, snr_text, transform=plt.gca().transAxes, fontsize=10,
#                verticalalignment='top', bbox=props)

# --- 修改开始 ---
# # 1. 获取当前图表的所有句柄和标签
# handles, labels = plt.gca().get_legend_handles_labels()

# # 2. 创建一个不可见的句柄，但给它附上SNR信息的标签
# snr_text_for_legend = (f'\n'  # 加一个换行符来分隔
#                        f'Original Avg SNR: {original_snr_all:.2f} dB\n'
#                        f'Denoised Avg SNR: {denoised_snr_all:.2f} dB\n'
#                        f'SNR Improvement: {snr_improvement:.2f} dB')
# extra_handle = mpatches.Patch(color='none', label=snr_text_for_legend)

# # 3. 将新句柄和它的标签（空字符串，因为标签在句柄里）添加到列表中
# handles.append(extra_handle)
# labels.append("") # 对应的label设为空

# # 4. 使用更新后的句柄和标签列表创建图例
# plt.legend(handles=handles, labels=labels, loc='best', fontsize='small')
# # --- 修改结束 ---

# plt.savefig(os.path.join(FIGURES_DIR, 'broadband_denoise_freq_200ps_comparison.png'), transparent=False, bbox_inches="tight")
# plt.show()