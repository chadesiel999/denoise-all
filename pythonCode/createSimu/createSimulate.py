import torch
# torch.backends.cudnn.enabled = False

import torch.nn as nn
import numpy as np
import os

from numpy import linalg as la

from scipy import signal
import matplotlib.pylab as plt
from torch.utils.data import Dataset, DataLoader
import time

# For running on GPU
# device = 'cuda:0' if torch.cuda.is_available() else 'cpu'
# print('Device state:', device)

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")# choose your device
# torch.cuda.set_per_process_memory_fraction(0.5, 0)
# a = torch.rand(5, 5, device=device)  # change by either using the device argument
# a = a.to(device)
####  data generate
fs = 128
f = 10
num_pulses = 10       # 偶发脉冲的个数
pulse_amplitude = 2   # 脉冲幅度
pulse_width = 10      # 每个脉冲的宽度

Amp = 5
x = np.linspace(0, 2 * np.pi * f * (1 - 1 / (f * fs)), fs * f)
y_sin = Amp * np.sin(x)
y_triangle = Amp * signal.sawtooth(x, 0.5)
y_saw = Amp * signal.sawtooth(x, 1)
y_pwm = Amp * signal.square(x)
y_occ = np.zeros(len(x))
pulse_positions = np.random.choice(len(x) - pulse_width, num_pulses, replace=False)
for pos in pulse_positions:
    y_occ[pos:pos + pulse_width] += pulse_amplitude
y_sin_occ = y_sin + y_occ
y_triangle_occ = y_triangle + y_occ
y_saw_occ = y_saw + y_occ
y_pwm_occ = y_pwm + y_occ

#plot original data
#plt.plot(x, y_sin)
#plt.show()
#plt.plot(x, y_sin_occ)
#plt.show()
#
# plt.plot(x, y_triangle)
# plt.xlabel('Phase [rad]')
# plt.ylabel('triangle(x)')
# plt.axis('tight')
# plt.show()
#
#
# plt.plot(x, y_saw)
# plt.xlabel('Phase [rad]')
# plt.ylabel('sawtooth(x)')
# plt.axis('tight')
# plt.show()
#
#
# plt.plot(x, y_pwm)
# plt.xlabel('Phase [rad]')
# plt.ylabel('pwm(x)')
# plt.axis('tight')
# plt.show()

noise = 0.5
# Add guassian noise
y_sin_n = y_sin_occ + noise * np.random.normal(size=len(x))
y_triangle_n = y_triangle_occ + noise * np.random.normal(size=len(x))
y_saw_n = y_saw_occ + noise * np.random.normal(size=len(x))
y_pwm_n = y_pwm_occ + noise * np.random.normal(size=len(x))

# SNR_sin_out       = float('inf')
# SNR_triangle_out   = float('inf')
# SNR_saw_out        = float('inf')
# SNR_pwm_out       = float('inf')


#plt.plot(x, y_sin_n)
#plt.show()
# plt.plot(x, y_sin_n)
# plt.xlabel('Angle [rad]')
# plt.ylabel('sin(x) + noise')
# plt.axis('tight')
# plt.show()
#
#
# plt.plot(x, y_triangle_n)
# plt.xlabel('Phase [rad]')
# plt.ylabel('triangle(x) + noise')
# plt.axis('tight')
# plt.show()
#
# plt.plot(x, y_saw_n)
# plt.xlabel('Phase [rad]')
# plt.ylabel('sawtooth(x) + noise')
# plt.axis('tight')
# plt.show()
#
# plt.plot(x, y_pwm_n)
# plt.xlabel('Phase [rad]')
# plt.ylabel('sawtooth(x) + noise')
# plt.axis('tight')
# plt.show()

# RNN model
input_dim = 1
hidden_size_1 = 32
input_snr = 1
hidden_snr = 16
# hidden_size_2 = 64
# hidden_size_3 = 32

output_size = 1
lr = 1e-4

def give_part_of_data(x, y, n_samples=10000, sample_size=500):

    data_inp = np.zeros((n_samples, sample_size))
    data_out = np.zeros((n_samples, sample_size))

    for i in range(n_samples):
        random_offset = np.random.randint(0, len(x) - sample_size)
        sample_inp = x[random_offset:random_offset + sample_size]
        sample_out = y[random_offset:random_offset + sample_size]
        data_inp[i, :] = sample_inp
        data_out[i, :] = sample_out

    return data_inp, data_out

def give_part_of_data1(x, y, n_samples=1, sample_size=128):

    data_inp = np.zeros((n_samples, sample_size))
    data_out = np.zeros((n_samples, sample_size))

    for i in range(n_samples):
        random_offset = np.random.randint(0, len(x) - sample_size)
        sample_inp = x[random_offset:random_offset + sample_size]
        sample_out = y[random_offset:random_offset + sample_size]
        data_inp[i, :] = sample_inp
        data_out[i, :] = sample_out

    return data_inp, data_out

def calculate_snr_for_samples(noisy_samples, signal_samples):
    """
    对每个样本计算 SNR（以 dB 为单位）
    :param signal_samples: 无噪声信号样本 (n_samples, sample_size)
    :param noisy_samples: 带噪声信号样本 (n_samples, sample_size)
    :return: SNR 数组 (n_samples,)
    """
    # 计算信号功率和噪声功率

    # 确保输入是二维
    if signal_samples.ndim == 1:
        signal_samples = np.expand_dims(signal_samples, axis=0)
    if noisy_samples.ndim == 1:
        noisy_samples = np.expand_dims(noisy_samples, axis=0)

    signal_power = np.mean(signal_samples ** 2, axis=1)  # 每个样本的信号功率
    noise_power = np.mean((noisy_samples - signal_samples) ** 2, axis=1) # 每个样本的噪声功率

    # 避免噪声功率为 0（防止除以零）
    noise_power = np.maximum(noise_power, 1e-12)

    # 计算 SNR（以 dB 为单位）
    snr_db = 10 * np.log10(signal_power / noise_power)

    #对snr归一并进行限制
    snr_db = np.clip(snr_db, None, 100)  # 最大值为 100
    snr_db = snr_db/100
    return snr_db

# Train, Validation, and Test
sin_train_in, sin_train_out = give_part_of_data(y_sin_n[:int(7 / 10 * len(x))], y_sin_occ[:int(7 / 10 * len(x))])

tri_train_in, tri_train_out = give_part_of_data(y_triangle_n[:int(7 / 10 * len(x))], y_triangle_occ[:int(7 / 10 * len(x))])

saw_train_in, saw_train_out = give_part_of_data(y_saw_n[:int(7 / 10 * len(x))], y_saw_occ[:int(7 / 10 * len(x))])

pwm_train_in, pwm_train_out = give_part_of_data(y_pwm_n[:int(7 / 10 * len(x))], y_pwm_occ[:int(7 / 10 * len(x))])

sin_val_in, sin_val_out = y_sin_n[int(7 / 10 * len(x)):int(8 / 10 * len(x))], y_sin_occ[int(7 / 10 * len(x)):int(8 / 10 * len(x))]
sin_val_in = sin_val_in[np.newaxis,...]
sin_val_out = sin_val_out[np.newaxis,...]
tri_val_in, tri_val_out = y_triangle_n[int(7 / 10 * len(x)):int(8 / 10 * len(x))], y_triangle_occ[int(7 / 10 * len(x)):int(
    8 / 10 * len(x))]
tri_val_in = tri_val_in[np.newaxis,...]
tri_val_out = tri_val_out[np.newaxis,...]
saw_val_in, saw_val_out = y_saw_n[int(7 / 10 * len(x)):int(8 / 10 * len(x))], y_saw_occ[
                                                           int(7 / 10 * len(x)):int(8 / 10 * len(x))]
saw_val_in = saw_val_in[np.newaxis,...]
saw_val_out = saw_val_out[np.newaxis,...]
pwm_val_in, pwm_val_out = y_pwm_n[int(7 / 10 * len(x)):int(8 / 10 * len(x))], y_pwm_occ[
                                                                              int(7 / 10 * len(x)):int(8 / 10 * len(x))]
pwm_val_in = pwm_val_in[np.newaxis,...]
pwm_val_out = pwm_val_out[np.newaxis,...]

sin_test_in, sin_test_out = y_sin_n[int(8 / 10 * len(x)):int(10 / 10 * len(x))], y_sin_occ[int(8 / 10 * len(x)):int(
    10 / 10 * len(x))]
tri_test_in, tri_test_out = y_triangle_n[int(8 / 10 * len(x)):int(10 / 10 * len(x))], y_triangle_occ[
                                                                                      int(8 / 10 * len(x)):int(
                                                                                          10 / 10 * len(x))]
saw_test_in, saw_test_out = y_saw_n[int(8 / 10 * len(x)):int(10 / 10 * len(x))], y_saw_occ[int(8 / 10 * len(x)):int(
    10 / 10 * len(x))]

pwm_test_in, pwm_test_out = y_pwm_n[int(8 / 10 * len(x)):int(10 / 10 * len(x))], y_pwm_occ[int(8 / 10 * len(x)):int(
    10 / 10 * len(x))]

SNR_train_sin_in = calculate_snr_for_samples(sin_train_in, sin_train_out)
SNR_train_triangle_in = calculate_snr_for_samples(tri_train_in, tri_train_out)
SNR_train_saw_in = calculate_snr_for_samples(saw_train_in, saw_train_out)
SNR_train_pwm_in = calculate_snr_for_samples(pwm_train_in, pwm_train_out)

SNR_sin_out        = calculate_snr_for_samples(sin_train_out, sin_train_out)
SNR_triangle_out   = calculate_snr_for_samples(tri_train_out, tri_train_out)
SNR_saw_out        = calculate_snr_for_samples(saw_train_out, saw_train_out)
SNR_pwm_out        = calculate_snr_for_samples(pwm_train_out, pwm_train_out)

SNR_val_sin_in        = calculate_snr_for_samples(sin_val_in, sin_val_out)
SNR_val_triangle_in   = calculate_snr_for_samples(tri_val_in, tri_val_out)
SNR_val_saw_in        = calculate_snr_for_samples(saw_val_in, saw_val_out)
SNR_val_pwm_in        = calculate_snr_for_samples(pwm_val_in, pwm_val_out)

X_train = np.concatenate([sin_train_in, tri_train_in, saw_train_in, pwm_train_in])[..., np.newaxis]
Y_train = np.concatenate([sin_train_out, tri_train_out, saw_train_out, pwm_train_out])[..., np.newaxis]

SNR_train_in = np.concatenate([SNR_train_sin_in,SNR_train_triangle_in,SNR_train_saw_in,SNR_train_pwm_in])[:, np.newaxis]
SNR_train_out = np.concatenate([SNR_sin_out,SNR_pwm_out,SNR_saw_out,SNR_triangle_out])[:, np.newaxis]

# X_val = np.vstack([sin_val_in, tri_val_in, saw_val_in, pwm_val_in])[..., np.newaxis]
# Y_val = np.vstack([sin_val_out, tri_val_out, saw_val_out, pwm_val_out])[..., np.newaxis]

X_val = np.concatenate([sin_val_in, tri_val_in, saw_val_in, pwm_val_in])[..., np.newaxis]
Y_val = np.concatenate([sin_val_out, tri_val_out, saw_val_out, pwm_val_out])[..., np.newaxis]

SNR_val_in = np.concatenate([SNR_val_sin_in, SNR_val_triangle_in, SNR_val_saw_in, SNR_val_pwm_in])[:, np.newaxis]
SNR_val_out = np.concatenate([SNR_sin_out,SNR_pwm_out,SNR_saw_out,SNR_triangle_out])[:, np.newaxis]

train_len = X_train.shape[0]
val_len = X_val.shape[0]

# Normalization
# la_norm = np.zeros((train_len))
# la_val_norm = np.zeros((val_len))
#
# for i in range(train_len):
#     X_train[i, :, 0] = X_train[i, :, 0] / la.norm(X_train[i, :, 0], 2)
#     Y_train[i, :, 0] = Y_train[i, :, 0] / la.norm(Y_train[i, :, 0], 2)
#
#
# for i in range(val_len):
#     X_val[i, :, 0] = X_val[i, :, 0] / la.norm(X_val[i, :, 0], 2)
#     Y_val[i, :, 0] = Y_val[i, :, 0] / la.norm(Y_val[i, :, 0], 2)

train_idx = list(range(train_len))

np.random.shuffle(train_idx)
X_train = X_train[train_idx]
Y_train = Y_train[train_idx]
SNR_train_in = SNR_train_in[train_idx]
SNR_train_out = SNR_train_out[train_idx]

X_train = torch.Tensor(X_train).type(torch.FloatTensor)
Y_train = torch.Tensor(Y_train).type(torch.FloatTensor)
SNR_train = torch.Tensor(SNR_train_in).type(torch.FloatTensor)
X_val = torch.Tensor(X_val).type(torch.FloatTensor)
Y_val = torch.Tensor(Y_val).type(torch.FloatTensor)
SNR_val = torch.Tensor(SNR_val_in).type(torch.FloatTensor)

#dummy_input = torch.randn(1, 1000, 1, device='cuda')
dummy_input = torch.randn(1, 1000, 1, device="cuda" if torch.cuda.is_available() else "cpu")


# Networks

class DenoisingNetwork(nn.Module):
    def __init__(self, input_size, hidden_size, input_snr, hidden_snr, output_size):
        super(DenoisingNetwork, self).__init__()

        # 全连接网络处理 SNR 输入
        self.fcn = nn.Sequential(
            nn.Linear(input_snr, hidden_snr),
            nn.ReLU(),
            nn.Linear(hidden_snr, hidden_snr),
            nn.ReLU()
        )

        self.LSTM_layer = nn.LSTM(input_size=input_size, hidden_size=hidden_size, num_layers=3, batch_first=True,
                                  dropout=0.2)

        self.fc = nn.Sequential(
            nn.Linear(hidden_size + hidden_snr, 64),
            nn.ReLU(),
            nn.Linear(64, output_size)
        )


    def forward(self, signal_input, snr_input):
        """
        前向传播
        :param signal_input: (batch_size, seq_len, input_dim_signal)
        :param snr_input: (batch_size, input_dim_snr)
        :return: 降噪后的信号 (batch_size, seq_len, output_dim)
        """
        # SNR 输入通过 FCN 提取特征
        snr_features = self.fcn(snr_input)  # (batch_size, hidden_dim_snr)

        # 信号输入通过 LSTM 提取特征
        lstm_out, _ = self.LSTM_layer(signal_input)  # (batch_size, seq_len, hidden_dim_signal)

        # 将 SNR 特征广播到每个时间步
        snr_features = snr_features.unsqueeze(1).repeat(1, signal_input.shape[1], 1)  # (batch_size, seq_len, hidden_snr)

        # 融合 LSTM 输出和 SNR 特征
        combined_features = torch.cat([lstm_out, snr_features],
                                      dim=-1)  # (batch_size, seq_len, hidden_dim_signal + hidden_dim_snr)

        # 输出降噪后的信号
        output = self.fc(combined_features)  # (batch_size, output_dim)
        return output

class CustomRNN(nn.Module):
    def __init__(self, input_size, hidden_size_1, hidden_size_2, output_size):
        super(CustomRNN, self).__init__()
        self.rnn = nn.RNN(input_size=input_size, hidden_size=hidden_size_1, batch_first=True)
        self.linear = nn.Linear(hidden_size_1, hidden_size_2, )
        self.act = nn.Tanh()
        self.linear = nn.Linear(hidden_size_2, output_size, )
        self.act = nn.Tanh()

    def forward(self, x):
        pred, hidden = self.rnn(x, None)
        pred = self.act(self.linear(pred)).view(pred.data.shape[0], -1, 1)
        return pred

class GRUModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUModel, self).__init__()
        self.hidden_size = hidden_size
        # 这里设置了 batch_first=True, 所以应该 inputs = inputs.view(inputs.shape[0], -1, inputs.shape[1])
        # 针对时间序列预测问题，相当于将时间步（seq_len）设置为 1。
        self.GRU_layer = nn.GRU(input_size=input_size, hidden_size=hidden_size, num_layers=3, batch_first=True, dropout=0.2)
        # self.GRU_layer_1 = nn.GRU(input_size=input_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_2 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_3 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        self.fc = nn.Linear(hidden_size, output_size)
        # self.act = nn.Tanh()

    def forward(self, input):
        # h_n of shape (num_layers * num_directions, batch, hidden_size)
        # 这里不用显式地传入隐层状态 self.hidden
        output, _ = self.GRU_layer(input)
        # output, _ = self.GRU_layer_1(input)
        # output, _ = self.GRU_layer_2(output)
        # output, _ = self.GRU_layer_3(output)
        output = self.fc(output).view(output.data.shape[0], -1, 1)
        return output

class LSTMModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(LSTMModel, self).__init__()
        self.hidden_size = hidden_size
        # 这里设置了 batch_first=True, 所以应该 inputs = inputs.view(inputs.shape[0], -1, inputs.shape[1])
        # 针对时间序列预测问题，相当于将时间步（seq_len）设置为 1。
        self.LSTM_layer = nn.LSTM(input_size=input_size, hidden_size=hidden_size, num_layers=3, batch_first=True, dropout=0.2)
        # self.GRU_layer_1 = nn.GRU(input_size=input_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_2 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_3 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        self.fc = nn.Linear(hidden_size, output_size)
        # self.act = nn.Tanh()

    def forward(self, input):
        # h_n of shape (num_layers * num_directions, batch, hidden_size)
        # 这里不用显式地传入隐层状态 self.hidden
        output, _ = self.LSTM_layer(input)
        # output, _ = self.GRU_layer_1(input)
        # output, _ = self.GRU_layer_2(output)
        # output, _ = self.GRU_layer_3(output)
        output = self.fc(output).view(output.data.shape[0], -1, 1)
        return output

class TransformerModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size, num_layers=3, num_heads=4, dropout=0.2):
        super(TransformerModel, self).__init__()
        self.hidden_size = hidden_size
        # Embedding layer: 将 input_size 映射到 hidden_size 维度
        self.embedding = nn.Linear(input_size, hidden_size)
        # Transformer Encoder
        encoder_layer = nn.TransformerEncoderLayer(d_model=hidden_size, nhead=num_heads, dropout=dropout,
                                                   batch_first=True)
        self.transformer = nn.TransformerEncoder(encoder_layer, num_layers=num_layers)

        # 全连接层，将 Transformer 的输出映射到目标维度
        self.fc = nn.Linear(hidden_size, output_size)

    def forward(self, x):
        # x: (batch_size, seq_len, input_size)

        # Step 1: Embedding
        x = self.embedding(x)  # (batch_size, seq_len, hidden_size)

        # Step 2: Transformer Encoder
        x = self.transformer(x)  # (batch_size, seq_len, hidden_size)

        # Step 3: 输出最后一个时间步的预测结果
        # 如果需要对每个时间步进行预测，可以直接 return self.fc(x)
        output = self.fc(x[:, -1, :])  # (batch_size, output_size)

        return output



# # RNN for Sin
# model = CustomRNN(input_dim, hidden_size_1, hidden_size_2, output_size)
# model = model.to(device)
# optimizer = torch.optim.Adam(model.parameters())
# loss_func = nn.MSELoss()
#
# for t in range(100):
#     inp = X_train
#     inp.requires_grad = True
#     inp = inp.to(device)
#
#     out = Y_train
#     out = out.to(device)
#
#     pred = model(inp)
#     optimizer.zero_grad()
#     loss = loss_func(pred, out)
#     if t % 20 == 0:
#         print(t, loss.data.item())
#
#     # lr = lr / 1.0001
#     optimizer.param_groups[0]['lr'] = lr
#     loss.backward()
#     optimizer.step()
#
# test_in = sin_test_in
# inp = torch.Tensor(test_in[np.newaxis, ..., np.newaxis])
# inp = inp.to(device)
# pred = model(inp).cpu().detach().numpy()
# plt.plot(range(len(sin_test_in)), test_in)
# plt.plot(range(len(sin_test_in)), pred[0, :, 0])
# plt.show()
#
# orginal_SNR = np.sum(np.abs(sin_test_out) ** 2) / np.sum(np.abs(sin_test_in - sin_test_out) ** 2)
# orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original SNR : ', orginal_SNR)
# print('Original SNR DB : ', orginal_SNR_db)
#
# network_SNR = np.sum(np.abs(sin_test_out) ** 2) / np.sum(np.abs(pred[0, :, 0] - sin_test_out) ** 2)
# network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('Network SNR : ', network_SNR)
# print('Network SNR DB : ', network_SNR_db)

# GRU FNN init
GRU_model = GRUModel(input_dim, hidden_size_1, output_size)
GRU_model = GRU_model.to(device)
LSTM_model = LSTMModel(input_dim, hidden_size_1, output_size)
LSTM_model = LSTM_model.to(device)
Transformer_model = TransformerModel(input_dim, hidden_size_1, output_size)
Transformer_model = Transformer_model.to(device)
DenoisingNetwork_model = DenoisingNetwork(input_dim, hidden_size_1,input_snr, hidden_snr,output_size)
DenoisingNetwork_model = DenoisingNetwork_model.to(device)
#optimizer = torch.optim.Adam(GRU_model.parameters(), lr=lr, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
optimizer = torch.optim.Adam(DenoisingNetwork_model.parameters(), lr=lr, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
# optimizer = torch.optim.Adam(Transformer_model.parameters(), lr=lr, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
criterion = nn.MSELoss()
# 如何定义联合损失函数？
#criterion_SNR =

def _save_checkpoint(epoch,model):
    ckp = model.state_dict()
    PATH = "tf_resnet_checkpoint.pt"
    torch.save(ckp, PATH)
    print(f"Epoch {epoch} | Training checkpoint saved at {PATH}")

class MyDataset(Dataset):
    # 初始化函数，得到数据
    def __init__(self, data_root, data_label):
        self.data = data_root.to(device)
        self.label = data_label.to(device)

    # index是根据batchsize划分数据后得到的索引，最后将data和对应的labels进行一起返回
    def __getitem__(self, index):
        data = self.data[index]
        labels = self.label[index]
        return data, labels

    # 该函数返回数据大小长度，目的是DataLoader方便划分，如果不知道大小，DataLoader会一脸懵逼
    def __len__(self):
        return len(self.data)

class MyDataset1(Dataset):
    def __init__(self, signal_data, target_data, snr_data):
        self.signal_data = signal_data
        self.target_data = target_data
        self.snr_data = snr_data

    def __getitem__(self, index):
        # 返回信号输入、目标输出和 SNR
        return self.signal_data[index], self.target_data[index], self.snr_data[index]

    def __len__(self):
        # 数据集总长度
        return len(self.signal_data)

# train_data = MyDataset(X_train, Y_train)
# val_data = MyDataset(X_val, Y_val)

train_data = MyDataset1(X_train, Y_train, SNR_train)
val_data = MyDataset1(X_val, Y_val, SNR_val)


train_loader = DataLoader(dataset=train_data, batch_size=int(train_len/50), shuffle=True, num_workers=0)
val_loader = DataLoader(dataset=val_data, batch_size=val_len, shuffle=True, num_workers=0)

train_SNR_loader = DataLoader(dataset=train_data, batch_size=int(train_len/50), shuffle=True, num_workers=0)
val_SNR_loader = DataLoader(dataset=val_data, batch_size=val_len, shuffle=True, num_workers=0)

# 获取当前脚本所在目录
current_dir = os.path.dirname(os.path.abspath(__file__))
results_dir = os.path.join(current_dir, 'results')
saved_model_dir = os.path.join(current_dir, 'saved_model')

if not os.path.exists(results_dir):
    os.makedirs(results_dir)
if not os.path.exists(saved_model_dir):
    os.makedirs(saved_model_dir)

num_epochs = 1000
losses = []
val_losses = []
min_val_loss = np.inf
patience = 0
save_every = 1

# for i in range(num_epochs):
#     print("Epoch {}/{}".format(i + 1, num_epochs))
#     start_time = time.time()
#     for data in train_loader:
#         GRU_model.train()
#         X_train, Y_train = data
#         Y_pred = GRU_model(X_train)
#
#         loss = criterion(Y_pred, Y_train)
#         optimizer.zero_grad()
#         loss.backward()
#         optimizer.step()
#     end_time = time.time()
#     print("Epoch time: {}s".format(end_time - start_time))
#     losses.append(loss.item())
#     print("loss: {}".format(loss.item()))
#     if i % save_every == 0:
#         _save_checkpoint(i,GRU_model)
#
#     GRU_model.eval()  # 在验证状态
#     valid_total_loss = 0
#     with torch.no_grad():  # 验证的部分，不是训练所以不要带入梯度
#         for valid_data in val_loader:
#             X_val, Y_val = valid_data
#             Y_pred = GRU_model(X_val)
#             val_loss = criterion(Y_pred, Y_val)
#         print("val_loss: {}".format(val_loss.item()))
#         val_losses.append(val_loss.item())
#
#     # early stopping
#     if i % 5 == 0:
#         if val_loss.item() < min_val_loss:
#             min_val_loss = val_loss.item()
#             print("update min validation loss")
#             patience = 0
#         else:
#             patience += 5
#             if patience == 20:
#                 print("Early stopping...")
#                 break

# for i in range(num_epochs):
#     print("Epoch {}/{}".format(i + 1, num_epochs))
#     start_time = time.time()
#     for data in train_loader:
#         LSTM_model.train()
#         X_train, Y_train = data
#         Y_pred = LSTM_model(X_train)
#
#         loss = criterion(Y_pred, Y_train)
#         optimizer.zero_grad()
#         loss.backward()
#         optimizer.step()
#     end_time = time.time()
#     print("Epoch time: {}s".format(end_time - start_time))
#     losses.append(loss.item())
#     print("loss: {}".format(loss.item()))
#     if i % save_every == 0:
#         _save_checkpoint(i,LSTM_model)
#
#     LSTM_model.eval()  # 在验证状态
#     valid_total_loss = 0
#     with torch.no_grad():  # 验证的部分，不是训练所以不要带入梯度
#         for valid_data in val_loader:
#             X_val, Y_val = valid_data
#             Y_pred = LSTM_model(X_val)
#             val_loss = criterion(Y_pred, Y_val)
#         print("val_loss: {}".format(val_loss.item()))
#         val_losses.append(val_loss.item())
#
#     # early stopping
#     if i % 5 == 0:
#         if val_loss.item() < min_val_loss:
#             min_val_loss = val_loss.item()
#             print("update min validation loss")
#             patience = 0
#         else:
#             patience += 5
#             if patience == 20:
#                 print("Early stopping...")
#                 break

for i in range(num_epochs):
    print("Epoch {}/{}".format(i + 1, num_epochs))
    start_time = time.time()
    for data in train_loader:
        DenoisingNetwork_model.train()
        X_train, Y_train, SNR_train = data
        X_train = X_train.to(device)
        Y_train = Y_train.to(device)
        SNR_train = SNR_train.to(device)
        Y_pred = DenoisingNetwork_model(X_train,SNR_train)

        loss = criterion(Y_pred, Y_train)
        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
    end_time = time.time()
    print("Epoch time: {}s".format(end_time - start_time))
    losses.append(loss.item())
    print("loss: {}".format(loss.item()))
    if i % save_every == 0:
        _save_checkpoint(i,DenoisingNetwork_model)

    DenoisingNetwork_model.eval()  # 在验证状态
    valid_total_loss = 0
    with torch.no_grad():  # 验证的部分，不是训练所以不要带入梯度
        for valid_data in val_loader:
            X_val, Y_val , SNR_val= valid_data
            X_val = X_val.to(device)
            Y_val = Y_val.to(device)
            SNR_val = SNR_val.to(device)
            Y_pred = DenoisingNetwork_model(X_val, SNR_val)
            val_loss = criterion(Y_pred, Y_val)
        print("val_loss: {}".format(val_loss.item()))
        val_losses.append(val_loss.item())

    # early stopping
    if i % 5 == 0:
        if val_loss.item() < min_val_loss:
            min_val_loss = val_loss.item()
            print("update min validation loss")
            patience = 0
        else:
            patience += 5
            if patience == 20:
                print("Early stopping...")
                break

# for i in range(num_epochs):
#     print("Epoch {}/{}".format(i + 1, num_epochs))
#     start_time = time.time()
#     for data in train_loader:
#         Transformer_model.train()
#         X_train, Y_train = data
#         Y_pred = Transformer_model(X_train)
#
#         loss = criterion(Y_pred, Y_train)
#         optimizer.zero_grad()
#         loss.backward()
#         optimizer.step()
#     end_time = time.time()
#     print("Epoch time: {}s".format(end_time - start_time))
#     losses.append(loss.item())
#     print("loss: {}".format(loss.item()))
#     if i % save_every == 0:
#         _save_checkpoint(i,Transformer_model)
#
#     Transformer_model.eval()  # 在验证状态
#     valid_total_loss = 0
#     with torch.no_grad():  # 验证的部分，不是训练所以不要带入梯度
#         for valid_data in val_loader:
#             X_val, Y_val = valid_data
#             Y_pred = Transformer_model(X_val)
#             val_loss = criterion(Y_pred, Y_val)
#         print("val_loss: {}".format(val_loss.item()))
#         val_losses.append(val_loss.item())
#
#     # early stopping
#     if i % 5 == 0:
#         if val_loss.item() < min_val_loss:
#             min_val_loss = val_loss.item()
#             print("update min validation loss")
#             patience = 0
#         else:
#             patience += 5
#             if patience == 20:
#                 print("Early stopping...")
#                 break

# np.savetxt(os.path.join(results_dir, 'LSTM_pre_training_loss'), losses)
# np.savetxt(os.path.join(results_dir, 'LSTM_pre_training_val_loss'), val_losses)
np.savetxt(os.path.join(results_dir, 'SNR_pre_training_loss'), losses)
np.savetxt(os.path.join(results_dir, 'SNR_pre_training_val_loss'), val_losses)
# np.savetxt(os.path.join(results_dir, 'Transformer_pre_training_loss'), losses)
# np.savetxt(os.path.join(results_dir, 'Transformer_pre_training_val_loss'), val_losses)

# GRU_model.eval()
# print(GRU_model)
# torch.save(GRU_model, "./saved_model/denoise.pth")
# # torch.save(model.state_dict(), "models/class_model_{}dB.pth".format(SNR))

# LSTM_model.eval()
# print(LSTM_model)
# torch.save(LSTM_model, os.path.join(saved_model_dir, "LSTM_denoise.pth"))

DenoisingNetwork_model.eval()
print(DenoisingNetwork_model)
torch.save(DenoisingNetwork_model, os.path.join(saved_model_dir, "SNR_denoise.pth"))

# Transformer_model.eval()
# print(Transformer_model)
# torch.save(Transformer_model, os.path.join(saved_model_dir, "Transformer_denoise.pth"))

input_names = ['input']  # the model's input names
output_names = ['output']  # the model's output names
dynamic_axes = {'input': {0: 'batch_size', 1: 'signal_length', 2: 'data_IQ'}, 'output': {0: 'batch_size', 1: 'signal_length', 2: 'data_IQ'}}

# torch.onnx.export(GRU_model, dummy_input, os.path.join(saved_model_dir, 'denoise.onnx'), input_names=input_names, output_names=output_names, dynamic_axes=dynamic_axes)
# torch.onnx.export(LSTM_model, dummy_input, os.path.join(saved_model_dir, 'denoise.onnx'), input_names=input_names, output_names=output_names, dynamic_axes=dynamic_axes)
# torch.onnx.export(Transformer_model, dummy_input, os.path.join(saved_model_dir, 'denoise.onnx'), input_names=input_names, output_names=output_names, dynamic_axes=dynamic_axes)
print("Model saved...")
#
# test sin
# print("Testing...")
# test_in = sin_test_in
# inp = torch.Tensor(test_in[np.newaxis, ..., np.newaxis])
# inp = inp.to(device)
# pred = GRU_model(inp).cpu().detach().numpy()
# plt.plot(range(len(sin_test_in)), test_in)
# plt.plot(range(len(sin_test_in)), pred[0, :, 0])
# plt.show()
#
# orginal_SNR = np.sum(np.abs(sin_test_out) ** 2) / np.sum(np.abs(sin_test_in - sin_test_out) ** 2)
# orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original sin SNR : ', orginal_SNR)
# print('Original sin SNR DB : ', orginal_SNR_db)
#
# network_SNR = np.sum(np.abs(sin_test_out) ** 2) / np.sum(np.abs(pred[0, :, 0] - sin_test_out) ** 2)
# network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('GRU sin SNR : ', network_SNR)
# print('GRU sin SNR DB : ', network_SNR_db)
#
# # test tri
# test_in = tri_test_in
# inp = torch.Tensor(test_in[np.newaxis, ..., np.newaxis])
# inp = inp.to(device)
# pred = GRU_model(inp).cpu().detach().numpy()
# plt.plot(range(len(tri_test_in)), test_in)
# plt.plot(range(len(tri_test_in)), pred[0, :, 0])
# plt.show()
#
# orginal_SNR = np.sum(np.abs(tri_test_out) ** 2) / np.sum(np.abs(tri_test_in - tri_test_out) ** 2)
# orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original tri SNR : ', orginal_SNR)
# print('Original tri DB : ', orginal_SNR_db)
#
# network_SNR = np.sum(np.abs(tri_test_out) ** 2) / np.sum(np.abs(pred[0, :, 0] - tri_test_out) ** 2)
# network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('GRU tri SNR : ', network_SNR)
# print('GRU tri SNR DB : ', network_SNR_db)
#
# # test saw
# test_in = saw_test_in
# inp = torch.Tensor(test_in[np.newaxis, ..., np.newaxis])
# inp = inp.to(device)
# pred = GRU_model(inp).cpu().detach().numpy()
# plt.plot(range(len(saw_test_in)), test_in)
# plt.plot(range(len(saw_test_in)), pred[0, :, 0])
# plt.show()
#
# orginal_SNR = np.sum(np.abs(saw_test_out) ** 2) / np.sum(np.abs(saw_test_in - saw_test_out) ** 2)
# orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original saw SNR : ', orginal_SNR)
# print('Original saw DB : ', orginal_SNR_db)
#
# network_SNR = np.sum(np.abs(saw_test_out) ** 2) / np.sum(np.abs(pred[0, :, 0] - saw_test_out) ** 2)
# network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('GRU saw SNR : ', network_SNR)
# print('GRU saw SNR DB : ', network_SNR_db)
#
