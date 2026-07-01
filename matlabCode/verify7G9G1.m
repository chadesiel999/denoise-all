%% 修复版：波形对比校验脚本 (分离绘图模式)
clear; clc; close all; 

% ================= 用户设置区域 =================
% 1. 目标频率
target_freq = 1500;  % 修改这里查看不同频率 (例如 7000, 7005, ... 9000)

% 2. 文件夹路径
folder_path = 'D:\Correction\0-Denoise\data0126-100M6G11G20G25M\';

% 3. 采样率 (注意：根据你之前的描述，这里应该是 80G，如果不是请手动改为 400e9)
fs = 80e9; 

% 4. 绘图显示的长度 (点数)
view_points = 500; 
% ==============================================

%% 检查文件路径
fprintf('正在读取 %d MHz 的数据...\n', target_freq);

filename_raw = sprintf('C1_Sub1_%04dMHz.txt', target_freq);   % 原始
filename_ideal = sprintf('C1_Sub2_%04dMHz.txt', target_freq); % 标签

path_raw = fullfile(folder_path, filename_raw);
path_ideal = fullfile(folder_path, filename_ideal);

% 检查文件是否存在
if ~isfile(path_raw)
    error('错误：找不到原始文件 -> %s', path_raw);
end
if ~isfile(path_ideal)
    error('错误：找不到理想文件 -> %s', path_ideal);
end

%% 读取并处理数据
% 读取原始数据
data_raw = readmatrix(path_raw);
if size(data_raw, 2) >= 2
    data_raw = data_raw(:, 2); 
end

% 读取理想数据
data_ideal = readmatrix(path_ideal);
if size(data_ideal, 2) >= 2
    data_ideal = data_ideal(:, 2);
end

% 截取长度 (取三者最小值，防止报错)
len = min([length(data_raw), length(data_ideal), view_points]);
data_raw_plot = data_raw(1:len);
data_ideal_plot = data_ideal(1:len);

% 生成时间轴
t_axis = (0:len-1) / fs * 1e9; % 单位 ns

%% 开始绘图 (分离显示)
figure('Name', sprintf('Signal Check: %d MHz', target_freq), ...
       'Color', 'w', ...
       'Position', [100, 50, 800, 800]); % 调整了窗口高度以便容纳3个图

% --- 子图 1: 原始波形 (Raw) ---
ax1 = subplot(3,1,1);
plot(t_axis, data_raw_plot, 'b', 'LineWidth', 1.2);
title(sprintf('原始输入 (Raw Input) - %d MHz', target_freq), 'FontSize', 11, 'FontWeight', 'bold');
ylabel('Amplitude');
grid on;
xlim([0, max(t_axis)]);

% --- 子图 2: 理想波形 (Ideal) ---
ax2 = subplot(3,1,2);
plot(t_axis, data_ideal_plot, 'r', 'LineWidth', 1.2);
title('理想标签 (Ideal Label)', 'FontSize', 11, 'FontWeight', 'bold');
ylabel('Amplitude');
grid on;
xlim([0, max(t_axis)]);

% --- 子图 3: 误差残差 (Error) ---
% 如果波形完美对齐，这里应该只剩下无规则的噪声
ax3 = subplot(3,1,3);
error_signal = data_raw_plot - data_ideal_plot;
plot(t_axis, error_signal, 'k', 'LineWidth', 1);
title('残差 (Residual) = Raw - Ideal', 'FontSize', 11);
xlabel('Time (ns)');
ylabel('Error');
grid on;
xlim([0, max(t_axis)]);

% --- 关键功能：联动坐标轴 ---
% 这样当你用鼠标放大(Zoom)任何一个子图时，其他两个子图的X轴会自动同步缩放
linkaxes([ax1, ax2, ax3], 'x');

fprintf('绘图完成！\n提示：使用工具栏的放大镜框选波形，三个图会同步放大，方便检查相位对齐情况。\n');