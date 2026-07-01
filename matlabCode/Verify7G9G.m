%% 修复版：波形对比校验脚本
% 这里的 clear 会清空工作区，确保变量重新加载
clear; 
% clc; 
close all; 

% ================= 用户设置区域 =================
% 1. 请确保这里定义了 target_freq
target_freq = 2500;  % <--- 关键变量！修改这里查看不同频率 (例如 7000, 8000, 9000)

% 2. 文件夹路径
folder_path = 'D:\paper\mismatch\底噪新版\DestData0608\DestData\ch1\';

% 3. 采样率
fs = 80e9; 

% 4. 绘图显示的长度 (点数)
view_points = 1000; 
% ==============================================

%% 检查文件路径
fprintf('正在读取 %d MHz 的数据...\n', target_freq);

filename_raw = sprintf('C1_Sub1_%dMHz.txt', target_freq);
filename_ideal = sprintf('C1_Sub2_%dMHz.txt', target_freq);

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

%% 开始绘图
% 这里就是你报错的第 52 行附近，现在 target_freq 肯定已经定义了
figure('Name', sprintf('Signal Check: %d MHz', target_freq), ...
       'Color', 'w', ...
       'Position', [100, 100, 1000, 700]);

% --- 子图 1: 波形对比 ---
subplot(2,1,1);
plot(t_axis, data_raw_plot, 'b', 'LineWidth', 1.2, 'DisplayName', 'Raw (Input)');
hold on;
plot(t_axis, data_ideal_plot, 'r--', 'LineWidth', 1.5, 'DisplayName', 'Ideal (Label)');
hold off;

title(sprintf('波形对比 - 频率: %d MHz', target_freq), 'FontSize', 12);
xlabel('Time (ns)');
ylabel('Amplitude');
legend('Location', 'best');
grid on;
xlim([0, max(t_axis)]);

% --- 子图 2: 误差残差 ---
subplot(2,1,2);
error_signal = data_raw_plot - data_ideal_plot;
plot(t_axis, error_signal, 'k', 'LineWidth', 1);

title('残差 (噪声) = Raw - Ideal', 'FontSize', 12);
xlabel('Time (ns)');
ylabel('Error Amplitude');
grid on;
xlim([0, max(t_axis)]);

fprintf('绘图完成！请查看弹出的窗口。\n');