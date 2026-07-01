
import numpy as np
import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import random
import torch.distributions as distributions
import os

device = torch.device("cuda")# choose your device

class SequentialMaskLayer(nn.Module):
    def __init__(self):
        super(SequentialMaskLayer, self).__init__()
    def forward(self, x, mask_ratio):
        batch_size, seq_len, feature_dim = x.shape
        mask_length = int(feature_dim * mask_ratio)  # 计算需要掩码的长度

        # 从序列末尾开始掩码
        if mask_length > 0:
            mask = torch.ones_like(x)  # 初始化全为1的掩码
            mask[:, :, -mask_length:] = 0.0  # 将最后mask_length部分设置为0
            output = x * mask
            return output[:, :, :-mask_length]
        else:
            return x


class GRUModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size, mask_ratio = 0):
        super(GRUModel, self).__init__()
        # 这里设置了 batch_first=True, 所以应该 inputs = inputs.view(inputs.shape[0], -1, inputs.shape[1])
        # 针对时间序列预测问题，相当于将时间步（seq_len）设置为 1。

        self.mask_ratio = mask_ratio
        hidden_size = int(hidden_size * (1 - mask_ratio))

        self.fc_input = nn.Linear(input_size, hidden_size)
        # self.mask_layer = SequentialMaskLayer()

        self.GRU_layer = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, num_layers=3, batch_first=True, dropout=0.2)
        # self.GRU_layer_1 = nn.GRU(input_size=input_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_2 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        # self.GRU_layer_3 = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, batch_first=True)
        self.fc_output = nn.Linear(hidden_size, output_size)
        # self.act = nn.Tanh()

    def forward(self, input):
        # h_n of shape (num_layers * num_directions, batch, hidden_size)
        # 这里不用显式地传入隐层状态 self.hidden
        output = self.fc_input(input)
        # output = self.mask_layer(output, self.mask_ratio)  # 传入动态掩码比例

        output, _ = self.GRU_layer(output)
        # output, _ = self.GRU_layer_1(input)
        # output, _ = self.GRU_layer_2(output)
        # output, _ = self.GRU_layer_3(output)
        output = self.fc_output(output)

        # output = self.fc(output).view(output.data.shape[0], -1, 1)
        return output


# class GRUCNNModel(nn.Module):
#     def __init__(self, input_size, hidden_size, output_size, mask_ratio = 0.5):
#         super(GRUCNNModel, self).__init__()
#
#         self.mask_ratio = mask_ratio
#         hidden_size = int(hidden_size * (1 - mask_ratio))
#
#         self.fc_input = nn.Linear(input_size, hidden_size)
#         # self.mask_layer = SequentialMaskLayer()
#
#         self.conv1 = nn.Conv1d(in_channels=hidden_size, out_channels=hidden_size, kernel_size=16, padding=1, stride=3)
#         self.GRU_layer = nn.GRU(input_size=hidden_size, hidden_size=hidden_size, num_layers=2, batch_first=True, dropout=0.2)
#         self.fc = nn.Linear(hidden_size, output_size)
#         self.conv2 = nn.ConvTranspose1d(in_channels=hidden_size, out_channels=1, kernel_size=16, padding=0, stride=3)
#
#     def forward(self, x):
#         x = self.fc_input(x)
#         # x = self.mask_layer(x, self.mask_ratio)  # 传入动态掩码比例
#
#         x = self.conv1(x.permute(0, 2, 1))
#         x = x.transpose(1, 2)  # 调整维度
#         x, _ = self.GRU_layer(x)
#         # x = self.fc(x).view(x.data.shape[0], -1, 1) # 特征维度归一
#         x = self.conv2(x.permute(0, 2, 1))
#         x = x.transpose(1, 2)  # 调整维度
#         return x

class DualModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(DualModel, self).__init__()

        self.GRU_layer = nn.GRU(input_size=input_size, hidden_size=hidden_size, num_layers=3, batch_first=True, dropout=0.2)

        self.fc = nn.Linear(hidden_size, output_size)

        self.conv1 = nn.Conv1d(in_channels=2, out_channels=hidden_size, kernel_size=16, padding=1, stride=3)
        self.conv2 = nn.ConvTranspose1d(in_channels=hidden_size, out_channels=2, kernel_size=16, padding=1, stride=3)

    def forward(self, x):

        x_0, _ = self.GRU_layer(x)
        x_0 = self.fc(x_0)

        x_flat = torch.squeeze(x, dim=2)
        x_freq = torch.fft.rfft(x_flat)

        x_freq = torch.dstack([x_freq.real, x_freq.imag])
        x_freq = self.conv1(x_freq.permute(0, 2, 1))
        x_freq = self.conv2(x_freq).permute(0, 2, 1)
        x_freq = torch.complex(x_freq[:, :, 0], x_freq[:, :, 1])
        x_1 = torch.fft.irfft(x_freq, x_flat.shape[1])
        x_1 = torch.unsqueeze(x_1, 2)

        x = x_0 + x_1

        return x


def calculate_entropy(x):
    """
    计算每个信号样本的每个维度的信息熵
    输入 x: (batch, 信号长度, 信号维数)
    输出 entropy: (batch, 信号维数)
    """
    eps = 1e-10  # 防止对数操作时除以0


    batch_size, signal_length, signal_dim = x.size()
    entropy = torch.zeros(1, signal_dim, device=x.device)  # 初始化熵张量
    for i in range(signal_dim):
        # 提取第 i 个样本的第 j 个维度
        sample_dim = x[:, :, i].view(-1)  # 将样本的维度展平为一维
        hist = torch.histc(sample_dim, bins=100, min=sample_dim.min(), max=sample_dim.max())  # 计算直方图
        prob = hist / hist.sum()  # 概率分布
        prob = prob[prob > eps]  # 过滤掉小于eps的值，避免log(0)
        entropy[0, i] = -(prob * torch.log(prob)).sum()  # 计算熵


    # entropy = torch.zeros(batch_size, 1, signal_dim, device=x.device)  # 初始化熵张量
    # for i in range(batch_size):
    #     for j in range(signal_dim):
    #         # 提取第 i 个样本的第 j 个维度
    #         sample_dim = x[i, :, j].view(-1)  # 将样本的维度展平为一维
    #         hist = torch.histc(sample_dim, bins=100, min=sample_dim.min(), max=sample_dim.max())  # 计算直方图
    #         prob = hist / hist.sum()  # 概率分布
    #         prob = prob[prob > eps]  # 过滤掉小于eps的值，避免log(0)
    #         entropy[i, 0, j] = -(prob * torch.log(prob)).sum()  # 计算熵

    return entropy


class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.fc = nn.Linear(hidden_size, hidden_size)
        self.relu = nn.ReLU()
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        x = self.conv1(x.permute(0, 2, 1))
        # residual = x
        x = x.transpose(1, 2)  # 调整维度
        x, _ = self.GRU_layer_1(x)
        x, _ = self.GRU_layer_2(x)
        x = self.conv2(x.permute(0, 2, 1))
        x = x.transpose(1, 2)  # 调整维度
        return x 
    
# class GRUCNNmodModel(nn.Module):
#     def __init__(self, input_size, hidden_size, output_size):
#         super(GRUCNNmodModel, self).__init__()
#         self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
#         self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
#         self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
#         self.fc = nn.Linear(hidden_size, hidden_size)
#         self.relu = nn.ReLU()
#         self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

#     def forward(self, x):
#         x = self.conv1(x.permute(0, 2, 1))
#         x = x.transpose(1, 2)  # 调整维度
#         x, _ = self.GRU_layer_1(x)
#         x, _ = self.GRU_layer_2(x)
#         x = self.conv2(x.permute(0, 2, 1))
#         x = x.transpose(1, 2)  # 调整维度
#         return x
    
# ...existing code...
class GRUCNNmodModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size, no_conv2=True):
        super(GRUCNNmodModel, self).__init__()
        self.no_conv2 = no_conv2

        # CNN 保持原结构
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)

        # 两层 GRU 都改成单向，维度保持 hidden_size，方便做残差
        self.GRU_layer_1 = nn.GRU(
            input_size=hidden_size,
            hidden_size=hidden_size,
            num_layers=1,
            batch_first=True,
            bidirectional=False
        )
        self.GRU_layer_2 = nn.GRU(
            input_size=hidden_size,
            hidden_size=hidden_size,
            num_layers=1,
            batch_first=True,
            bidirectional=False
        )

        if self.no_conv2:
            self.head = nn.Linear(hidden_size, output_size)
        else:
            self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        # x: [B, L, C]
        x = self.conv1(x.permute(0, 2, 1)).transpose(1, 2)  # [B, L, H]
        residual = x

        x, _ = self.GRU_layer_1(x)  # [B, L, H]
        x, _ = self.GRU_layer_2(x)  # [B, L, H]

        # 残差连接
        x = x + residual

        if self.no_conv2:
            x = self.head(x)  # [B, L, output_size]
        else:
            x = self.conv2(x.permute(0, 2, 1)).transpose(1, 2)  # [B, L, output_size]
        return x
# ...existing code...


class nobiGRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(nobiGRUCNNModel, self).__init__()
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        # self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size), num_layers=1, batch_first=True, dropout=0.2, bidirectional=False)
        self.fc = nn.Linear(hidden_size, hidden_size)
        self.relu = nn.ReLU()
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        x = self.conv1(x.permute(0, 2, 1))
        # residual = x
        x = x.transpose(1, 2)  # 调整维度
        x, _ = self.GRU_layer_1(x)
        # x, _ = self.GRU_layer_2(x)
        x = self.conv2(x.permute(0, 2, 1))
        x = x.transpose(1, 2)  # 调整维度
        return x
    
class TinyCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(TinyCNNModel, self).__init__()
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        x = x.permute(0, 2, 1)   # (batch, input_size, seq_len)
        x = self.conv1(x)
        x = self.conv2(x)
        x = x.transpose(1, 2)    # (batch, seq_len, output_size)
        return x

class DynamicMaskGRU_CNN_Model(nn.Module):
    def __init__(self, input_size, hidden_size, output_size, mask_ratio = 0.5):
        super(DynamicMaskGRU_CNN_Model, self).__init__()

        # 超参数统一定义
        self.input_size = input_size  # 输入维度（特征数）
        self.hidden_size = hidden_size  # GRU隐藏层维度
        self.cnn_filters = hidden_size  # CNN滤波器数量
        self.num_layers = 3  # GRU层数
        self.output_size = output_size  # 输出维度
        self.mask_ratio = 0.5  # 初始掩码比例

        # GRU 层
        # self.gru = nn.GRU(self.hidden_size, self.hidden_size, self.num_layers, batch_first=True)
        self.GRU_layer_1 = nn.GRU(input_size=self.cnn_filters, hidden_size=int(self.hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=int(self.hidden_size), hidden_size=int(self.hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)


        # CNN 层
        self.conv1 = nn.Conv1d(self.hidden_size, self.cnn_filters, kernel_size=3, padding=1)
        self.conv2 = nn.Conv1d(self.hidden_size, self.cnn_filters, kernel_size=3, padding=1)

        # 输入映射层
        self.fc_input = nn.Linear(self.input_size, self.hidden_size)

        # 全连接层
        # self.fc = nn.Linear(self.hidden_size + self.cnn_filters, self.output_dim)
        self.fc = nn.Linear(self.hidden_size, self.output_size)


    def forward(self, x, mask_ratio):
        x = self.fc_input(x)

        x = self.apply_mask(x, mask_ratio)
        x = self.conv1(x.permute(0, 2, 1))
        x = x.transpose(1, 2)  # 调整维度
        x, _ = self.GRU_layer_1(x)
        x, _ = self.GRU_layer_2(x)
        # GRU 处理时序信息
        x = self.conv2(x.permute(0, 2, 1))
        output = x.transpose(1, 2)  # 调整维度
        output = self.fc(output)
        return output

        # x = self.fc_input(x)
        #
        # x = self.apply_mask(x, mask_ratio)
        #
        # # GRU 处理时序信息
        # gru_out, _ = self.GRU_layer_1(x)
        # gru_out, _ = self.GRU_layer_2(gru_out)
        #
        # output = self.fc(gru_out)


    def apply_mask(self, x, mask_ratios):
        batch_size, seq_len, feature_dim = x.shape
        mask_length = (feature_dim * mask_ratios).int()

        for i in range(batch_size):        # 从序列末尾开始掩码
            if mask_length[i] > 0:
                mask = torch.ones_like(x)  # 初始化全为1的掩码
                mask[:, :, -mask_length[i]:] = 0.0  # 将最后mask_length部分设置为0
                output = x * mask
                # return output[:, :, :-mask_length[i]]
                return output
            else:
                return x
        # 根据动态掩码比例应用掩码
        # mask = torch.zeros_like(x)
        # for i in range(x.size(0)):
        #     mask[i] = torch.rand_like(x[i]) < mask_ratio[i]
        # return x * mask.float()

    def mask_and_topk(self, data, alpha, is_freq=False):
        num_elements = data.numel() // data.size(0)
        num_topk = int(num_elements * alpha)
        if is_freq:
            # 频域数据按幅度大小排序
            data_abs = torch.abs(data.squeeze())
            _, topk_indices = torch.topk(data_abs, num_topk, dim=1)
        else:
            # 时域数据按信息量（信息熵）排序
            entropy = calculate_entropy(data)
            _, topk_indices = torch.topk(entropy, num_topk, dim=1)
        mask = torch.zeros((data.size(0), data.size(1)), dtype=torch.bool, device=device)
        mask.scatter_(1, topk_indices, True)
        mask = mask.unsqueeze(-1).expand_as(data)
        data_masked = data * mask.float()
        # 创建掩码标记通道
        mask_channel = mask
        return data_masked, mask_channel


# 1. 定义策略网络（Policy Network）
class PolicyNetwork(nn.Module):
    def __init__(self, state_dim, action_dim, hidden_dim):
        super(PolicyNetwork, self).__init__()
        self.net = nn.Sequential(
            nn.Linear(state_dim, hidden_dim),
            nn.ReLU(),
        )
        self.mu = nn.Linear(hidden_dim, action_dim)
        self.sigma = nn.Linear(hidden_dim, action_dim)

    def forward(self, x):
        x = x.view(x.size(0), -1)  # 调整输入张量的形状为 (batch, state_dim)
        x = self.net(x)
        mu = torch.sigmoid(self.mu(x))  # 使用 sigmoid 激活函数
        sigma = F.softplus(self.sigma(x)) / 10 + 0.0001  # 确保 sigma 是正的
        return mu, sigma


# 2. 定义价值网络（Value Network）
class ValueNetwork(nn.Module):
    def __init__(self, state_dim, hidden_dim):
        super(ValueNetwork, self).__init__()
        self.net = nn.Sequential(
            nn.Linear(state_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, 1)
        )
    def forward(self, state):
        state = state.view(state.size(0), -1)  # 调整输入张量的形状为 (batch, state_dim)
        value = self.net(state)
        return value

# 3. 定义 PPO 智能体
class PPOAgent:
    def __init__(self, state_dim=3, action_dim=1, hidden_dim=32, lr=1e-4, gamma=0.99, epsilon=0.2, c1=0.5, c2=0.01):
        self.action_dim = action_dim
        self.gamma = gamma
        self.epsilon = epsilon
        self.c1 = c1  # Value loss coefficient
        self.c2 = c2  # Entropy regularization coefficient

        self.policy_network = PolicyNetwork(state_dim, action_dim, hidden_dim).to(device)
        self.value_network = ValueNetwork(state_dim, hidden_dim).to(device)
        self.policy_optimizer = optim.Adam(self.policy_network.parameters(), lr=lr)
        self.value_optimizer = optim.Adam(self.value_network.parameters(), lr=lr)
        self.memory = PGReplay()
        self.k_epochs = 2


    def get_states(self, x, loss):
        # 计算信号的信息熵

        time_entropy = calculate_entropy(x)
        freq_entropy = calculate_entropy(torch.fft.rfft(x).abs())
        loss = loss.unsqueeze(-1).unsqueeze(-1)
        # states = torch.cat((x, entropy), dim=1)
        states = torch.cat((time_entropy, freq_entropy, loss), dim=1)
        return states

    def calculate_mask_ratio(self, states):
        actions, log_probs = self.select_action(states)
        return actions, log_probs

    def select_action(self, states):
        # states: (batch, state_dim)
        mu, sigma = self.policy_network(states)  # shape: (batch, action_dim)
        dist = distributions.normal.Normal(mu, sigma)        #构建分布
        actions = torch.clamp(dist.sample(), 0, 0.9)  # shape: (batch,)
        log_probs = dist.log_prob(actions)  # shape: (batch,)
        return actions, log_probs

    def evaluate(self, states, actions):
        # states: (batch, state_dim)
        # actions: (batch,)
        mu, sigma = self.policy_network(states)  # shape: (batch, action_dim)
        dist = distributions.normal.Normal(mu, sigma)        #构建分布
        log_probs = dist.log_prob(actions)  # shape: (batch,)
        values = self.value_network(states)  # shape: (batch, 1)
        return log_probs, values

    def update(self, states, actions, rewards, next_states, log_probs_old):
        # states, next_states: (batch, state_dim)
        # actions: (batch,)
        # rewards: (batch,)
        # log_probs_old: (batch,)
        self.policy_optimizer.zero_grad()
        self.value_optimizer.zero_grad()

        # Calculate returns and advantages
        values = self.value_network(states)  # shape: (batch, 1)
        next_values = self.value_network(next_states)  # shape: (batch, 1)
        returns = rewards.unsqueeze(-1).unsqueeze(-1) + self.gamma * next_values  # shape: (batch, 1)
        advantages = returns - values  # shape: (batch, 1)
        # Normalize advantages
        advantages = (advantages - advantages.mean()) / (advantages.std() + 1e-8)

        # Evaluate current policy and value
        log_probs, _ = self.evaluate(states, actions)  # shape: (batch,)
        ratio = torch.exp(log_probs - log_probs_old)  # shape: (batch,)
        clipped_ratio = torch.clamp(ratio, 1 - self.epsilon, 1 + self.epsilon)  # shape: (batch,)
        policy_loss = -torch.min(ratio * advantages.squeeze(-1), clipped_ratio * advantages.squeeze(-1)).mean()

        # Evaluate value network
        _, values = self.evaluate(states, actions)  # shape: (batch, 1)
        value_loss = F.mse_loss(values, returns)

        # Add entropy regularization
        mu, sigma = self.policy_network(states)  # shape: (batch, action_dim)

        dist = distributions.normal.Normal(mu, sigma)  # shape: (batch,)
        entropy_loss = -self.c2 * dist.entropy().mean()

        # Total loss
        total_loss = policy_loss + self.c1 * value_loss + entropy_loss

        # Backward pass
        total_loss.backward(retain_graph=True)

        # Step optimizers
        self.policy_optimizer.step()
        self.value_optimizer.step()

        return total_loss.item()

    def learn(self):
        old_states, old_actions, old_log_probs, old_rewards, old_dones = self.memory.sample()
        # convert to tensor
        old_states = torch.tensor(old_states, device=device, dtype=torch.float32)
        old_actions = torch.tensor(old_actions, device=device, dtype=torch.float32)
        old_log_probs = torch.tensor(old_log_probs, device=device, dtype=torch.float32)
        # monte carlo estimate of state rewards
        returns = []
        discounted_sum = 0
        for reward, done in zip(reversed(old_rewards), reversed(old_dones)):
            if done:
                discounted_sum = 0
            discounted_sum = reward + (self.gamma * discounted_sum)
            returns.insert(0, discounted_sum)
        # Normalizing the rewards:
        returns = torch.tensor(returns, device=device, dtype=torch.float32)
        returns = (returns - returns.mean()) / (returns.std() + 1e-8)  # 1e-5 to avoid division by zero

        old_states = torch.flatten(old_states, start_dim=0, end_dim=1)
        old_actions = torch.flatten(old_actions, start_dim=0, end_dim=1)
        old_log_probs = torch.flatten(old_log_probs, start_dim=0, end_dim=1)

        Loss = []
        for _ in range(self.k_epochs):
            # compute advantage
            values = self.value_network(old_states)  # detach to avoid backprop through the critic
            advantage = returns - values.detach()
            # get action probabilities
            # probs = self.policy_network(old_states)
            # dist = Categorical(probs)

            mu, sigma = self.policy_network(old_states)  # shape: (batch, action_dim)
            dist = distributions.normal.Normal(mu, sigma)  # shape: (batch,)
            # get new action probabilities
            new_probs = dist.log_prob(old_actions)
            # compute ratio (pi_theta / pi_theta__old):
            ratio = torch.exp(new_probs - old_log_probs)  # old_log_probs must be detached
            # compute surrogate loss
            surr1 = ratio * advantage
            surr2 = torch.clamp(ratio, 1 - self.epsilon, 1 + self.epsilon) * advantage
            # compute actor loss
            actor_loss = - torch.min(surr1, surr2).mean() - self.c2 * dist.entropy().mean()
            # compute critic loss
            critic_loss = self.c1 * (returns - values).pow(2).mean()
            # take gradient step
            self.policy_optimizer.zero_grad()
            self.value_optimizer.zero_grad()
            actor_loss.backward()
            critic_loss.backward()
            self.policy_optimizer.step()
            self.value_optimizer.step()
        self.memory.clear()
        return  [- torch.min(surr1, surr2).mean().item(), - self.c2 * dist.entropy().mean().item(), self.c1 * (returns - values).pow(2).mean().item()]

    def save_models(self, save_dir="./saved_model/"):
        """保存策略网络和价值网络的参数及其优化器状态"""
        # 构造保存路径
        policy_path = os.path.join(save_dir, "policy_network.pth")
        value_path = os.path.join(save_dir, "value_network.pth")
        policy_opt_path = os.path.join(save_dir, "policy_optimizer.pth")
        value_opt_path = os.path.join(save_dir, "value_optimizer.pth")

        # 保存网络参数和优化器状态
        torch.save({
            'model_state_dict': self.policy_network.state_dict(),
            'optimizer_state_dict': self.policy_optimizer.state_dict()
        }, policy_path)

        torch.save({
            'model_state_dict': self.value_network.state_dict(),
            'optimizer_state_dict': self.value_optimizer.state_dict()
        }, value_path)

        print(f"Models saved to {save_dir}")

    def load_models(self, load_dir="./saved_model/"):
        """加载已保存的模型参数和优化器状态"""
        # 构造加载路径
        policy_path = os.path.join(load_dir, "policy_network.pth")
        value_path = os.path.join(load_dir, "value_network.pth")

        # 检查文件是否存在
        if not os.path.exists(policy_path) or not os.path.exists(value_path):
            raise FileNotFoundError("Model files not found in the specified directory")

        # 加载策略网络
        policy_checkpoint = torch.load(policy_path)
        self.policy_network.load_state_dict(policy_checkpoint['model_state_dict'])
        self.policy_optimizer.load_state_dict(policy_checkpoint['optimizer_state_dict'])

        # 加载价值网络
        value_checkpoint = torch.load(value_path)
        self.value_network.load_state_dict(value_checkpoint['model_state_dict'])
        self.value_optimizer.load_state_dict(value_checkpoint['optimizer_state_dict'])

        print(f"Models loaded from {load_dir}")


import random
from collections import deque


class ReplayBufferQue:
    '''DQN的经验回放池，每次采样batch_size个样本'''

    def __init__(self, capacity: int) -> None:
        self.capacity = capacity
        self.buffer = deque(maxlen=self.capacity)

    def push(self, transitions):
        '''_summary_
        Args:
            trainsitions (tuple): _description_
        '''
        self.buffer.append(transitions)

    def sample(self, batch_size: int, sequential: bool = False):
        if batch_size > len(self.buffer):
            batch_size = len(self.buffer)
        if sequential:  # sequential sampling
            rand = random.randint(0, len(self.buffer) - batch_size)
            batch = [self.buffer[i] for i in range(rand, rand + batch_size)]
            return zip(*batch)
        else:
            batch = random.sample(self.buffer, batch_size)
            return zip(*batch)

    def clear(self):
        self.buffer.clear()

    def __len__(self):
        return len(self.buffer)


class PGReplay(ReplayBufferQue):
    '''PG的经验回放池，每次采样所有样本，因此只需要继承ReplayBufferQue，重写sample方法即可
    '''

    def __init__(self):
        self.capacity = 100
        self.buffer = deque()

    def sample(self):
        ''' sample all the transitions
        '''
        batch = list(self.buffer)
        return zip(*batch)

class QNetwork(nn.Module):
    def __init__(self, state_dim=2, action_dim=8, hidden_dim=32):
        super(QNetwork, self).__init__()
        self.net = nn.Sequential(
            nn.Linear(state_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, action_dim)
        )

    def forward(self, state):
        state = state.view(state.size(0), -1)
        return self.net(state).unsqueeze(1)

# 定义 RL 智能体
class RLAgent:
    def __init__(self, state_dim=2, action_dim=8, hidden_dim=32, lr=1e-4, gamma=0.99, epsilon=0.1):
        self.action_dim = action_dim
        self.gamma = gamma
        self.epsilon = epsilon
        self.q_network = QNetwork(state_dim, action_dim, hidden_dim).to(device)
        self.optimizer = optim.Adam(self.q_network.parameters(), lr=lr)
        self.loss_fn = nn.MSELoss()

    def get_states(self, x, loss):
        # 计算信号的信息熵
        time_entropy = calculate_entropy(x)
        freq_entropy = calculate_entropy(torch.fft.rfft(x).abs())
        # loss = loss.view(x.size(0), 1, -1)
        # states = torch.cat((x, entropy), dim=1)
        states = torch.cat((time_entropy, freq_entropy), dim=1)
        return states

    def calculate_mask_ratio(self, states):
        actions, _ = self.select_action(states)
        mask_ratios = 1 / self.action_dim * actions

        return mask_ratios, _

    def select_action(self, states):
        # states: (batch, state_dim)
        if random.random() < self.epsilon:
            # 随机选择动作（探索）
            actions = torch.randint(0, self.action_dim, (states.size(0), 1), device=device)
            log_probs = None
        else:
            # 根据 Q 网络选择最优动作（利用）
            with torch.no_grad():
                q_values = self.q_network(states)  # shape: (batch, action_dim)
                actions = torch.argmax(q_values, dim=2)
        return actions, None

    def update(self, states, actions, rewards, next_states, _):
        # states, next_states: (batch, state_dim)
        # actions: (batch,)
        # rewards: (batch,)
        actions = actions / (1 / self.action_dim)
        self.optimizer.zero_grad()
        q_values = self.q_network(states)  # shape: (batch, action_dim)
        q_value = q_values.gather(2, actions.type(torch.int64).unsqueeze(-1)).squeeze(-1)  # shape: (batch,)
        with torch.no_grad():
            q_next = self.q_network(next_states)  # shape: (batch, action_dim)
            max_q_next = torch.max(q_next, dim=2)[0]  # shape: (batch,)
        targets = rewards.unsqueeze(-1) + self.gamma * max_q_next  # shape: (batch,)
        loss = self.loss_fn(q_value, targets)
        loss.backward(retain_graph=True)
        self.optimizer.step()
        return loss.item()


class Residual(nn.Module):
    def __init__(self, input_channels, num_channels, use_1x1conv=False, strides=1):
        super().__init__()
        self.conv1 = nn.Conv1d(input_channels, num_channels, kernel_size=3, padding=1)
        self.conv2 = nn.Conv1d(input_channels, num_channels, kernel_size=3, padding=1)
        if use_1x1conv:
            self.conv3 = nn.Conv1d(input_channels, num_channels,
                                   kernel_size=3, padding=1)
        else:
            self.conv3 = None
        self.bn1 = nn.BatchNorm1d(num_channels)
        self.bn2 = nn.BatchNorm1d(num_channels)

    def forward(self, X):
        Y = F.relu(self.bn1(self.conv1(X)))
        Y = self.bn2(self.conv2(Y))
        if self.conv3:
            X = self.conv3(X)
        Y += X
        return F.relu(Y)


def resnet_block(input_channels, num_channels, num_residuals,
                 first_block=False):
    blk = []
    for i in range(num_residuals):
        if i == 0 and not first_block:
            blk.append(Residual(input_channels, num_channels,
                                use_1x1conv=True, strides=2))
        else:
            blk.append(Residual(num_channels, num_channels))
    return blk


class ResNet(nn.Module):
    def __init__(self, input_dim, hidden_size, output_size):
        super().__init__()
        self.conv1 = nn.Conv1d(input_dim, hidden_size, kernel_size=3, padding=1)
        self.bn1 = nn.BatchNorm1d(hidden_size)
        self.activation = nn.ReLU()
        self.Pooling1 = nn.MaxPool1d(kernel_size=3, stride=2, padding=1)
        self.b2 = nn.Sequential(*resnet_block(hidden_size, hidden_size, 2, first_block=True))
        self.b3 = nn.Sequential(*resnet_block(hidden_size, hidden_size, 2))
        self.GRU_layer_1 = nn.GRU(input_size=hidden_size, hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)
        self.GRU_layer_2 = nn.GRU(input_size=int(hidden_size), hidden_size=int(hidden_size / 2), num_layers=1, batch_first=True, dropout=0.2, bidirectional=True)

        # self.Pool = nn.AdaptiveAvgPool1d((1))
        self.FC = nn.Linear(hidden_size, hidden_size)
        self.out = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        x = self.conv1(x.permute(0, 2, 1))
        x = self.bn1(x)
        x = self.activation(x)
        # x = self.Pooling1(x)
        x = self.b2(x)
        x = self.b3(x)
        # x = self.Pool(x)
        x = x.transpose(1, 2)
        x, _ = self.GRU_layer_1(x)
        x, _ = self.GRU_layer_2(x)
        x = self.FC(x)
        x = x.transpose(1, 2)
        x = self.out(x)
        x = x.transpose(1, 2)
        return x

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
        self.relu = nn.ReLU()
        self.out = nn.Conv1d(embed_dim, 1, kernel_size=3, padding=1)

    def forward(self, x):
        x = self.embed(x.permute(0, 2, 1))  # [B, D, L]
        x = x.permute(0, 2, 1)  # [B, L, D]
        x = self.attn(x) + x  # 残差注意力
        x = self.fc(x)
        # x = self.relu(x)
        x = x.permute(0, 2, 1)  # [B, D, L]
        x = self.out(x).permute(0, 2, 1)
        return x

