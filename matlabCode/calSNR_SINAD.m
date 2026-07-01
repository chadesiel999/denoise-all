clc; clear; close all;

%% 1. 参数与数据输入
Fs = 80e9; % 采样率 80 GHz (80 GS/s)

input = [ 
    % --- 在下方粘贴 降噪前(Input) 数据 ---
    
]; 

output = [
    % --- 在下方粘贴 降噪后(Output) 数据 ---
    
];

%% 2. 自动计算与画图
% 防止数据格式错误，强制转为列向量
input = input(:); 
output = output(:);

% % 如果没有填数据，生成一点随机数据防止报错（实际使用时请忽略此段）
% if isempty(input) || isempty(output)
%     disp('【提示】检测到数据为空，正在使用模拟信号演示...');
%     t = 0:1/Fs:1e-9*100; % 100个点
%     input = sin(2*pi*1e9*t)' + 0.1*randn(size(t))';
%     output = sin(2*pi*1e9*t)' + 0.01*randn(size(t))';
% end

% === 图1：SNR (信噪比) 频域对比 ===
figure('Name', 'SNR Comparison', 'Color', 'w');

subplot(2, 1, 1);
r_snr_in = snr(input, Fs);  % 自动画图
title(['Input SNR: ', num2str(r_snr_in, '%.2f'), ' dB']);

subplot(2, 1, 2);
r_snr_out = snr(output, Fs); % 自动画图
title(['Output SNR: ', num2str(r_snr_out, '%.2f'), ' dB']);

% === 图2：SINAD (信纳比) 频域对比 ===
figure('Name', 'SINAD Comparison', 'Color', 'w');

subplot(2, 1, 1);
r_sinad_in = sinad(input, Fs);  % 自动画图
title(['Input SINAD: ', num2str(r_sinad_in, '%.2f'), ' dB']);

subplot(2, 1, 2);
r_sinad_out = sinad(output, Fs); % 自动画图
title(['Output SINAD: ', num2str(r_sinad_out, '%.2f'), ' dB']);

% === 命令行打印数值结果 ===
fprintf('\n=========== 计算结果 (Fs = 80G) ===========\n');
fprintf('Input  => SNR: %6.2f dB | SINAD: %6.2f dB\n', r_snr_in, r_sinad_in);
fprintf('Output => SNR: %6.2f dB | SINAD: %6.2f dB\n', r_snr_out, r_sinad_out);
fprintf('提升量 => SNR: %+6.2f dB | SINAD: %+6.2f dB\n', r_snr_out - r_snr_in, r_sinad_out - r_sinad_in);
fprintf('===========================================\n');