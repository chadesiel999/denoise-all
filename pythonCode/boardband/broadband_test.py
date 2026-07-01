import numpy as np
from numpy import linalg as la
import matplotlib.pylab as plt
from scipy import signal
import time
import pickle
import torch.nn as nn
import torch
import torch.nn.functional as F
# import zelin_dataset

import torch

device = torch.device("cuda")# choose your device

def load_broadband_denoise(filename):
    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx=[]
    val_idx=[]
    sum_samples=0

    n_samples = Xd.shape[0]
    X.append(Xd[:, 0, :]) # 0 ??????2??????
    Y.append(Xd[:, 1, :])

    train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
    val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
    sum_samples += n_samples
    X = np.vstack(X)
    Y = np.vstack(Y)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train),(X_val,Y_val),(X_test,Y_test)

# X_test = np.loadtxt('D:/paper/mismatch/底噪/py/ZZY/dataset/wideband/wideband/C1_Sub1_1000MHz.txt')
# X_test = (X_test-2048)/300

# Y_test = np.loadtxt('D:/paper/mismatch/底噪/py/ZZY/dataset/wideband/wideband/C1_Sub2_1000MHz.txt')
# Y_test = (Y_test-2048)/300

(X_train, Y_train), (X_val, Y_val), (X_test, Y_test) = load_broadband_denoise("/home/uestcauto/cyy/boardband/dataset/broadband_dataset_200ps.pkl")



# ort_session = ort.InferenceSession("./saved_model/broadband_denoise_idledata.onnx")

n_samples = len(X_test)

input_dim = 1
hidden_size_1 = 64
output_size = 1
lr = 1e-3

class MultiHeadAttention(nn.Module):
    """多头自注意力机制（优化计算效率）"""
    def __init__(self, embed_dim, num_heads):
        super().__init__()
        self.embed_dim = embed_dim
        self.num_heads = num_heads
        self.head_dim = embed_dim // num_heads

        assert self.head_dim * num_heads == embed_dim, "embed_dim需被num_heads整除"

        # 合并投影减少计算量
        self.qkv_proj = nn.Linear(embed_dim, 3 * embed_dim)
        self.out_proj = nn.Linear(embed_dim, embed_dim)

    def forward(self, x):
        batch_size, seq_len, _ = x.size()
        # 合并计算QKV [B, L, 3*D]
        qkv = self.qkv_proj(x)
        # 拆分为多头 [B, L, H, 3*D_h] -> [B, H, L, D_h]
        qkv = qkv.reshape(batch_size, seq_len, self.num_heads, 3 * self.head_dim)
        qkv = qkv.permute(0, 2, 1, 3)
        q, k, v = qkv.chunk(3, dim=-1)  # 各[B, H, L, D_h]

        # 注意力得分 [B, H, L, L]
        attn_scores = torch.matmul(q, k.transpose(-2, -1)) / torch.sqrt(
            torch.tensor(self.head_dim, dtype=torch.float32))
        attn_weights = F.softmax(attn_scores, dim=-1)

        # 加权值 [B, H, L, D_h]
        attended = torch.matmul(attn_weights, v)
        # 拼接多头 [B, L, D]
        attended = attended.permute(0, 2, 1, 3).contiguous().view(batch_size, seq_len, -1)
        return self.out_proj(attended)

class LiteAttnGRU(nn.Module):
    """轻量版（参数量减少60%）"""
    def __init__(self, input_dim=1, embed_dim=32, num_heads=2):
        super().__init__()
        self.embed = nn.Conv1d(input_dim, embed_dim, kernel_size=3, padding=1)
        self.attn = MultiHeadAttention(embed_dim, num_heads)
        self.fc = nn.Linear(embed_dim, embed_dim)
        self.out = nn.Conv1d(embed_dim, 1, kernel_size=3, padding=1)

    def forward(self, x):
        x = self.embed(x.permute(0, 2, 1))  # [B, D, L]
        x = x.permute(0, 2, 1)  # [B, L, D]
        x = self.attn(x) + x  # 残差注意力
        x = self.fc(x)
        x = x.permute(0, 2, 1)  # [B, D, L]
        x = self.out(x).permute(0, 2, 1)
        return x

GRU_model = LiteAttnGRU(input_dim, hidden_size_1, num_heads=2)

state_dict  = torch.load("./saved_model/broadband_denoise_multiattention.pth")
GRU_model.load_state_dict(state_dict)

# ground_truth = Y_test[[0]][:, :, 0, None]
# test_input = X_test[[0]][:, :, 0, None].astype(np.float32)

choice_idx = np.random.choice(list(range(0, n_samples)), size=100, replace=False)
# choice_idx = range(n_samples)
ground_truth = Y_test[choice_idx].astype(np.float32)
test_inputnp = X_test[choice_idx].astype(np.float32)

# ground_truth = Y_test.astype(np.float32)
# test_inputnp = X_test.astype(np.float32)

X_test = torch.Tensor(X_test).type(torch.FloatTensor).to(device)
pth_input = X_test[choice_idx]

# # test_input = (torch.from_numpy(X_test[choice_idx].astype(np.float32)).float()).unsqueeze(0).unsqueeze(-1)
# # test_input = (torch.from_numpy(X_test[choice_idx].astype(np.float32)).float())
# test_input = (torch.from_numpy(X_test.astype(np.float32)).float())
# # test_input = test_input[:490]
# test_input = np.reshape(test_input, (-1, 500, 1))
# # test_input[:, 30:33, :] = 1.1

start_time = time.time()

x_module_out = GRU_model(pth_input)
x_module_out = x_module_out.detach().numpy()
# x_module_out = x_module_out[:500]
# x_module_out = x_module_out.reshape(-1)
# onnx_outputs = ort_session.run(None, {'input': test_input})[0]
end_time = time.time()
# 823513 882000
# onnx_outputs = np.reshape(onnx_outputs,(len(choice_idx),-1, 1))

print('Testing time: {}s'.format(end_time - start_time))

t = 0

# test sin
# plt.ylim(-330, 330)
plt.figure(1)
plt.plot(range(len(test_inputnp[t, :, 0])), test_inputnp[t, :, 0] * 300,  color='red', linestyle='-.', label="noise_signal")
# plt.plot(range(len(test_input[t, :, 0])), onnx_outputs[t, :, 0] * 300,  color='blue', linestyle='-', label="denoise_signal")
plt.plot(range(len(test_inputnp[t, :, 0])), x_module_out[t, :, 0] * 300,  color='blue', linestyle='-', label="denoise_signal")
# plt.plot(range(len(test_input[t, :, 0])), ground_truth[t, :, 0] * 300, color='k', linestyle='-', label="ground truth")
# plt.axis("off")
plt.legend(loc='upper right')
plt.savefig('./figures/broadband_denoise_signal.png', transparent=True, bbox_inches="tight", pad_inches=0)
plt.show()

plt.figure(2)
plt.plot(range(len(torch.fft.rfft(torch.Tensor(test_inputnp[t, :, 0])).numpy())), torch.fft.rfft(torch.Tensor(test_inputnp[t, :, 0])).numpy(),  color='red', linestyle='-.', label="noise_signal")
# plt.plot(range(len(torch.fft.rfft(torch.Tensor(test_input[t, :, 0])).numpy())), torch.fft.rfft(torch.Tensor(onnx_outputs[t, :, 0])).numpy(),  color='blue', linestyle='-', label="denoise_signal")
plt.plot(range(len(torch.fft.rfft(torch.Tensor(test_inputnp[t, :, 0])).numpy())), torch.fft.rfft(torch.Tensor(x_module_out[t, :, 0])).numpy(),  color='blue', linestyle='-', label="denoise_signal")
# plt.plot(range(len(torch.fft.rfft(torch.Tensor(test_input[t, :, 0])).numpy())), torch.fft.rfft(torch.Tensor(ground_truth[t, :, 0])).numpy(), color='k', linestyle='-', label="ground truth")
# plt.axis("off")
plt.legend(loc='best')
plt.savefig('./figures/broadband_denoise_freq.png', transparent=True, bbox_inches="tight", pad_inches=0)
plt.show()

#
orginal_SNR = np.sum(np.abs(ground_truth[t, :, 0] * 300) ** 2) / np.sum(np.abs(test_inputnp[t, :, 0] * 300 - ground_truth[t, :, 0] * 300) ** 2)
orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original sin SNR: ', orginal_SNR)
print('Original sin SNR dB: ', orginal_SNR_db)

# network_SNR = np.sum(np.abs(ground_truth[t, :, 0] * 300) ** 2) / np.sum(np.abs(onnx_outputs[t, :, 0] * 300 - ground_truth[t, :, 0] * 300) ** 2)
network_SNR = np.sum(np.abs(ground_truth[t, :, 0] * 300) ** 2) / np.sum(np.abs(x_module_out[t, :, 0] * 300 - ground_truth[t, :, 0] * 300) ** 2)
network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('GRU sin SNR: ', network_SNR)
print('Denoise sin SNR dB: ', network_SNR_db)


orginal_SNR = np.sum(np.abs(ground_truth) ** 2) / np.sum(np.abs(test_inputnp - ground_truth) ** 2)
orginal_SNR_db = 10 * np.log(orginal_SNR) / np.log(10)
# print('Original sin SNR: ', orginal_SNR)
print('Original sin SNR dB: ', orginal_SNR_db)

# network_SNR = np.sum(np.abs(ground_truth) ** 2) / np.sum(np.abs(onnx_outputs - ground_truth) ** 2)
network_SNR = np.sum(np.abs(ground_truth) ** 2) / np.sum(np.abs(x_module_out - ground_truth) ** 2)
network_SNR_db = 10 * np.log(network_SNR) / np.log(10)
# print('GRU sin SNR: ', network_SNR)
print('Denoise sin SNR dB: ', network_SNR_db)
