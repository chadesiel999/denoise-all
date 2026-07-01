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

data_dir = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0105-10G20G-15M/"
files = os.listdir(data_dir)



for file in files:
    # freq = int(file.split('_')[2][:-7])
    # if freq % 10 == 0:
    #     print(freq)
    os.chdir("/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0105-10G20G-15M")

    if file.split('_')[1] == 'Sub1':
        wave = read_data(file, 10000)
        refer_file = 'C1_Sub2_' + file[8:]
        refer_wave = read_data(refer_file, 10000)
        data_max = np.max(wave)
        data_min = np.min(wave)
        data_max_ref = np.max(refer_wave)
        data_min_ref = np.min(refer_wave)      

        wave, label_wave = normalize(wave, data_max, data_min), normalize(refer_wave, data_max_ref, data_min_ref)
        wave, label_wave = align_wave(wave, label_wave) # 通道噪声

        waves.append(wave[:9600])
        label_waves.append(label_wave[:9600])
        
        # 找到所有处理后波形的最小长度
min_len = min(min(len(w) for w in waves), min(len(lw) for lw in label_waves))

# 确保所有波形都截取到这个最小长度，以保证形状一致
waves = [w[:min_len] for w in waves]
label_waves = [lw[:min_len] for lw in label_waves]

data = np.stack((waves, label_waves))
# data = normalize(data)
# 注意：你可能需要根据新的min_len来调整reshape的参数
# 确保 min_len 可以被 60 整除
if min_len % 240 != 0:
    # 如果不能整除，需要调整截取长度或reshape的维度
    new_len = (min_len // 240) * 240
    waves = [w[:new_len] for w in waves]
    label_waves = [lw[:new_len] for lw in label_waves]
    data = np.stack((waves, label_waves))

# min_len_waves = min(len(sublist) for sublist in waves)
# min_len_label = min(len(sublist) for sublist in label_waves)
# data = np.stack((waves, label_waves))

# data = normalize(data)
data = np.reshape(data, (2, -1, 240, 1))
data = np.swapaxes(data, 0, 1)


file_to_save = open("/home/uestcauto/cyy/boardband/dataset/broadband_dataset_10G20G15M.pkl", "wb")
pickle.dump(data, file_to_save, -1)
file_to_save.close()
print("dataset generate successful")


orginal_SNR = np.sum(np.abs(data[:, 1, :, :] ** 2)) / np.sum(np.abs(data[:, 0, :, :] - data[:, 1, :, :]) ** 2)
orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original sin SNR: ', orginal_SNR)
print('Original sin SNR dB: ', orginal_SNR_db)

# low_SNR = np.sum(np.abs(data[:, 2, :, :] ** 2)) / np.sum(np.abs(data[:, 0, :, :] - data[:, 2, :, :]) ** 2)
# low_SNR_db = 10 * np.log(low_SNR) / np.log(10)
# # print('Original sin SNR: ', orginal_SNR)
# print('Low sin SNR dB: ', low_SNR_db)

t = 10

real_trigger = data[t, 0, :, :]
label_trigger = data[t, 1, :, :]


plt.figure(1)
plt.clf()  # 清除当前图形
# plt.ylim(0, 1)
# plt.xlim(-1, 102)
plt.ylabel('Normalized Signal Amplitude') # y轴标签：归一化后的信号幅度（-1~1）
plt.xlabel('Signal Sample Point Index') # x轴标签：信号样本点索引（0~479）
plt.plot(np.arange(len(real_trigger)), real_trigger, color='darkblue', linestyle='-', label='real')
plt.plot(np.arange(len(label_trigger)), label_trigger, color='red', linestyle='-.', label='label')
# plt.savefig('/home/uestcauto/cyy/BoardBand/figures/dataset.png', transparent=True, bbox_inches="tight", pad_inches=0)


# 定义保存路径
save_path = '/home/uestcauto/cyy/boardband/figures/dataset_10G20G15M.png'
# 获取目录部分
save_dir = os.path.dirname(save_path)
# 如果目录不存在，则创建它
os.makedirs(save_dir, exist_ok=True)

plt.savefig(save_path, transparent=True, bbox_inches="tight", pad_inches=0)

plt.legend()
plt.grid()
plt.show()
orginal_SNR = np.sum(np.abs(label_trigger) ** 2) / np.sum(np.abs(real_trigger - label_trigger) ** 2)
orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
print("SNR:", orginal_SNR_db)