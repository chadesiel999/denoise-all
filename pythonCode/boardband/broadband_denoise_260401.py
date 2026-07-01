import torch

import torch.nn as nn
import numpy as np
from numpy import linalg as la

from scipy import signal
import matplotlib.pylab as plt
from torch.utils.data import Dataset, DataLoader
import time
import zelin_dataset
# from thop import profile, clever_format

from models import *
torch.autograd.set_detect_anomaly(True)

# For running on GPU
device = torch.device("cuda")# choose your device
# torch.cuda.set_per_process_memory_fraction(0.5, 0)


(X_train, Y_train), (X_val, Y_val), (X_test, Y_test) = zelin_dataset.load_broadband_denoise("/home/uestcauto/cyy/boardband/dataset/broadband_dataset_260401.pkl")

X_train = torch.Tensor(X_train).type(torch.FloatTensor)
Y_train = torch.Tensor(Y_train).type(torch.FloatTensor)
X_val = torch.Tensor(X_val).type(torch.FloatTensor)
Y_val = torch.Tensor(Y_val).type(torch.FloatTensor)

dummy_input = (torch.randn(1, 240, 1, device='cuda'))
input_dim = 1
hidden_size_1 = 32
output_size = 1
lr = 1e-4 #1e-3
freq_weight = 0 #1e-3

# GRU_model = TinyCNNModel(input_dim, hidden_size_1, output_size)
GRU_model = GRUCNNModel(input_dim, hidden_size_1, output_size)

GRU_model = GRU_model.to(device)
optimizer = torch.optim.Adam(GRU_model.parameters(), lr=lr, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
criterion = nn.MSELoss()


class MyDataset(Dataset):
    # 初始化函数，得到数据
    def __init__(self, data_root, data_label):
        self.data = data_root.to(device)
        self.data_label = data_label.to(device)

    # index是根据batchsize划分数据后得到的索引，最后将data和对应的labels进行一起返回
    def __getitem__(self, index):
        data = self.data[index]
        labels = self.data_label[index]
        return data, labels

    # 该函数返回数据大小长度，目的是DataLoader方便划分，如果不知道大小，DataLoader会一脸懵逼
    def __len__(self):
        return len(self.data)

train_data = MyDataset(X_train, Y_train)
val_data = MyDataset(X_val, Y_val)
train_len = X_train.shape[0]
val_len = X_val.shape[0]
train_loader = DataLoader(dataset=train_data, batch_size=int(train_len/50), shuffle=True, num_workers=0)
val_loader = DataLoader(dataset=val_data, batch_size=int(val_len), shuffle=True, num_workers=0)


num_epochs = 1500
patience = 0

losses = []
val_losses = []
min_val_loss = np.inf


for i in range(num_epochs):
    print("Epoch {}/{}".format(i + 1, num_epochs))
    start_time = time.time()
    epoch_loss = []
    epoch_efficiency = []
    for data in train_loader:
        GRU_model.train()
        X_train, Y_train = data
        Y_pred = GRU_model(X_train)
        time_loss = criterion(Y_pred, Y_train)
        # time_loss = time_loss/torch.max(time_loss)

        window = signal.get_window('hann', X_train.shape[1])
        window = window / np.sqrt(np.mean(window**2))
        window = torch.tensor(window, dtype=torch.float32, device=device)
        windowed_noise = window * Y_train
        windowed_pred = window * Y_pred
        
        pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
        noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
        
        magnitude_pred = torch.abs(pred_fft)
            # 取对数幅度谱 (dB)
        log_pred = 20 * torch.log10(magnitude_pred+ 1e-10)
        magnitude_noise = torch.abs(noise_fft)
            # 取对数幅度谱 (dB)
        log_noise = 20 * torch.log10(magnitude_noise+ 1e-10)

        # pred_fft = torch.abs(torch.fft.rfft(Y_pred, dim=1))
        # noise_fft = torch.abs(torch.fft.rfft(Y_train, dim=1))
        extra_loss = pred_fft - noise_fft
        # extra_loss = log_pred - log_noise

        # baseline_noise = torch.mean(noise_fft)
        baseline_noise = torch.mean(log_noise)

        # freq_loss = torch.mean(torch.square(torch.where(noise_fft >= 0.2 * baseline_noise, extra_loss, pred_fft)))
        # freq_loss = torch.mean(torch.abs(torch.where(noise_fft >= 0.5 * baseline_noise, extra_loss, pred_fft)))
        freq_loss = torch.mean(torch.abs(log_pred - log_noise))
        # freq_loss = torch.mean(torch.abs(torch.where(log_noise >= 0.5 * baseline_noise, extra_loss, log_pred)))
        # freq_loss = freq_loss/torch.max(freq_loss)

        loss = (1-freq_weight)*time_loss + freq_weight * freq_loss  # 调节权重λ
        # rewards = -loss

        optimizer.zero_grad()
        loss.backward()
        optimizer.step()
        epoch_loss.append(loss.item())

    end_time = time.time()
    print("Epoch time: {}s".format(end_time - start_time))
    losses.append(sum(epoch_loss)/len(epoch_loss))
    print("Epoch avg loss: {}".format(sum(epoch_loss)/len(epoch_loss)))

    GRU_model.eval()  # 在验证状态
    valid_total_loss = 0
    mask_ratios = None
    loss = np.inf
    with torch.no_grad():  # 验证的部分，不是训练所以不要带入梯度
        for valid_data in val_loader:
            X_val, Y_val = valid_data
            Y_pred = GRU_model(X_val)
            time_loss = criterion(Y_pred, Y_val)
            # time_loss = time_loss/torch.max(time_loss)
            
            window = signal.get_window('hann', X_val.shape[1])
            window = window / np.sqrt(np.mean(window**2))
            window = torch.tensor(window, dtype=torch.float32, device=device)
            windowed_noise = window * Y_val
            windowed_pred = window * Y_pred
            
            pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
            noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
            
            magnitude_pred = torch.abs(pred_fft)
            # 取对数幅度谱 (dB)
            log_pred = 20 * torch.log10(magnitude_pred+ 1e-10)
            magnitude_noise = torch.abs(noise_fft)
                # 取对数幅度谱 (dB)
            log_noise = 20 * torch.log10(magnitude_noise+ 1e-10)

            # pred_fft = torch.abs(torch.fft.rfft(Y_pred, dim=1))
            # noise_fft = torch.abs(torch.fft.rfft(Y_val, dim=1))
            extra_loss = pred_fft - noise_fft
            # extra_loss = log_pred - log_noise

            # baseline_noise = torch.mean(noise_fft)
            baseline_noise = torch.mean(log_noise)

            # freq_loss = torch.mean(torch.abs(torch.where(noise_fft >= 0.5 * baseline_noise, extra_loss, pred_fft)))
            freq_loss = torch.mean(torch.abs(log_pred - log_noise))
            # freq_loss = torch.mean(torch.abs(torch.where(log_noise >= 0.5 * baseline_noise, extra_loss, log_pred)))
            # freq_loss = freq_loss/torch.max(freq_loss)
            
            val_loss = (1-freq_weight)*time_loss + freq_weight * freq_loss  # 调节权重λ
        print("val_loss: {}".format(val_loss.item()))
        val_losses.append(val_loss.item())

    # early stopping
    if i % 10 == 0:
        patience += 10
        if val_loss.item() < min_val_loss:
            min_val_loss = val_loss.item()
            print("update min validation loss")
            patience = 0
        if patience == 50:
            print("Early stopping...")
            break


GRU_model.eval()
print(GRU_model)
# if Is_transfered:
#     torch.save(GRU_model.state_dict(), "./saved_model/transfered_modulation.pth")
#     np.savetxt('results/transfer_training_loss', losses)
#     np.savetxt('./results/transfer_training_val_loss', val_losses)
# else:
torch.save(GRU_model.state_dict(), "/home/uestcauto/cyy/boardband/saved_model/broadband_dataset_260401.pth")
np.savetxt('/home/uestcauto/cyy/boardband/results/broadband_260401_240_loss.txt', losses)
np.savetxt('/home/uestcauto/cyy/boardband/results/broadband_260401_240_val_loss.txt', val_losses)
# np.savetxt('./results/broadband_efficiency_32', efficiency)
#

# torch.save(model.state_dict(), "models/class_model_{}dB.pth".format(SNR))

input_names = ['input']  # the model's input names
output_names = ['output']  # the model's output names
dynamic_axes = {'input': {0: 'batch_size', 1: 'signal_length', 2: 'data'}, 'output': {0: 'batch_size', 1: 'signal_length', 2: 'data'}}

torch.onnx.export(GRU_model, dummy_input, '/home/uestcauto/cyy/boardband/saved_model/broadband_dataset_260401_240.onnx', input_names=input_names, output_names=output_names, dynamic_axes=dynamic_axes)
print("Model saved...")
#



# pretrained_dict = torch.load(weight_path)
# model.load_state_dict(pretrained_dict, strict=False)