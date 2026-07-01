import numpy as np
from numpy import linalg as la
import matplotlib.pylab as plt
import os
import pickle

def safe_float_convert(x):
    try:
        return float(x)
    except ValueError as e:
        print(f"Error converting '{x}' to float: {e}")
        return None

def read_data(file_path, num_values):
    data = []
    with open(file_path, 'r') as file:
        for _ in range(num_values):
            line = file.readline()
            if not line:
                break
            value = safe_float_convert(line.strip())
            if value is not None:
                data.append(value)
    return data

def align_wave(wave, label_wave):
    cross_correlation = np.correlate(wave, label_wave, mode='full')
    delay = np.argmax(cross_correlation) - len(wave) + 1

    if delay >= 0:
        wave = wave[delay:]
    else:
        label_wave = label_wave[-delay:]

    return wave, label_wave


def normalize(data, data_max, data_min):
    for i in range(len(data)):
        data[i] = 2 * (data[i] - data_min) / (data_max - data_min) - 1
    return data

waves = []
label_waves = []
baseline_file_flags = []

# def normalize(data, data_min):
#     for i in range(len(data)):
#         data[i] = (data[i] - data_min) / 2**16
#     return data

# waves = []
# label_waves = []

# current_dir = os.getcwd()
# os.chdir(current_dir + r"/data101401/data")
#files = os.listdir("/home/uestcauto/zelin_code/Code/Denoise/data101401/data_zhineng")

# files = os.listdir()

data_dir = "/home/uestcauto/cyy/boardband/data0401/"
files = os.listdir(data_dir)



for file in files:
    os.chdir("/home/uestcauto/cyy/boardband/data0401/")

    if file.split('_')[1] == 'Sub1':
        # 提取频率，按照 25MHz 间隔采样 (基线 0M 也会被保留因为 0%25==0)
        try:
            freq_str = file.split('_')[2].split('M')[0]
            freq = int(freq_str)
            if freq % 25 != 0:
                continue
        except ValueError:
            pass  # 如果解析频率失败，则默认保留不跳过
            
        wave = read_data(file, 10000)
        refer_file = 'C1_Sub2_' + file[8:]
        refer_wave = read_data(refer_file, 10000)
        data_max = np.max(wave)
        data_min = np.min(wave)
        data_max_ref = np.max(refer_wave)
        data_min_ref = np.min(refer_wave)      

        if '0000MHz' in file:
            # 基线信号：不对齐。为了匹配正常数据[-1, 1]范围，且AD是16位，需要去直流并除以32768
            scale_factor = 32768.0
            wave = list((np.array(wave) - np.mean(wave)) / scale_factor)
            label_wave = list((np.array(refer_wave) - np.mean(refer_wave)) / scale_factor)
            baseline_file_flags.append(True)
        else:
            wave, label_wave = normalize(wave, data_max, data_min), normalize(refer_wave, data_max_ref, data_min_ref)
            wave, label_wave = align_wave(wave, label_wave) # 通道噪声
            baseline_file_flags.append(False)

        waves.append(wave[:9600])
        label_waves.append(label_wave[:9600])
        
        # 找到所有处理后波形的最小长度
min_len = min(min(len(w) for w in waves), min(len(lw) for lw in label_waves))

# 确保所有波形都截取到这个最小长度，以保证形状一致
waves = [w[:min_len] for w in waves]
label_waves = [lw[:min_len] for lw in label_waves]

if min_len % 240 != 0:
    new_len = (min_len // 240) * 240
    waves = [w[:new_len] for w in waves]
    label_waves = [lw[:new_len] for lw in label_waves]
else:
    new_len = min_len

data = np.stack((waves, label_waves))

data = np.reshape(data, (2, -1, 240, 1))
data = np.swapaxes(data, 0, 1)


file_to_save = open("/home/uestcauto/cyy/boardband/dataset/broadband_dataset_260401.pkl", "wb")
pickle.dump(data, file_to_save, -1)
file_to_save.close()
print("dataset generate successful")

# 生成基线掩码
segments_per_file = new_len // 240
is_baseline_segment = np.repeat(baseline_file_flags, segments_per_file)

# 拆分正常数据和基线数据
normal_data = data[~is_baseline_segment]
baseline_data = data[is_baseline_segment]

# 仅对正常信号计算 SNR
orginal_SNR = np.sum(np.abs(normal_data[:, 1, :, :] ** 2)) / np.sum(np.abs(normal_data[:, 0, :, :] - normal_data[:, 1, :, :]) ** 2)
orginal_SNR_db = 10 * np.log10(orginal_SNR)
print('Original sin SNR dB (excluding baseline): ', orginal_SNR_db)

# 寻找需要展示的样本进行画图
# 展示正常信号
t = 10
if len(normal_data) > t:
    t_idx = np.where(~is_baseline_segment)[0][t]
else:
    t_idx = np.where(~is_baseline_segment)[0][0] if len(normal_data) > 0 else 0

if len(normal_data) > 0:
    real_trigger = data[t_idx, 0, :, :]
    label_trigger = data[t_idx, 1, :, :]

    plt.figure(1)
    plt.clf()  # 清除当前图形
    plt.ylabel('Normalized Signal Amplitude') # y轴标签：归一化后的信号幅度（-1~1）
    plt.xlabel('Signal Sample Point Index') # x轴标签：信号样本点索引（0~479）
    plt.plot(np.arange(len(real_trigger)), real_trigger, color='darkblue', linestyle='-', label='real')
    plt.plot(np.arange(len(label_trigger)), label_trigger, color='red', linestyle='-.', label='label')

    # 定义保存路径
    save_path = '/home/uestcauto/cyy/boardband/figures/dataset_260401.png'
    os.makedirs(os.path.dirname(save_path), exist_ok=True)
    plt.savefig(save_path, transparent=True, bbox_inches="tight", pad_inches=0)
    plt.legend()
    plt.grid()

    orginal_SNR_single = np.sum(np.abs(label_trigger) ** 2) / np.sum(np.abs(real_trigger - label_trigger) ** 2)
    orginal_SNR_single_db = 10 * np.log10(orginal_SNR_single)
    print("Normal Signal Sample SNR:", orginal_SNR_single_db)

# 展示基线信号并画图
if len(baseline_data) > 0:
    t_base_idx = np.where(is_baseline_segment)[0][0] # 取第一张基线波形图
    real_baseline = data[t_base_idx, 0, :, :]
    label_baseline = data[t_base_idx, 1, :, :]

    plt.figure(2)
    plt.clf()
    plt.ylabel('Baseline Signal Amplitude') 
    plt.xlabel('Signal Sample Point Index') 
    plt.plot(np.arange(len(real_baseline)), real_baseline, color='darkblue', linestyle='-', label='baseline_real')
    plt.plot(np.arange(len(label_baseline)), label_baseline, color='red', linestyle='-.', label='baseline_label')

    save_path_base = '/home/uestcauto/cyy/boardband/figures/dataset_260401_baseline.png'
    os.makedirs(os.path.dirname(save_path_base), exist_ok=True)
    plt.savefig(save_path_base, transparent=True, bbox_inches="tight", pad_inches=0)
    plt.legend()
    plt.grid()

plt.show()