clear
clc
close all
%记得删掉文件夹里面的文件 名字写错了 sub2的  给noise tek mode
%% init
num = 50000;
fs = 100e9;
t = (0:num-1)/fs ;
fc = 33e9;          % 6G低通
filter_order = 50; % 滤波器阶数（越高越陡峭）
snr_db = 100;


%% rename
folderPath = 'D:\paper\mismatch\底噪新版\wave_acq\single\';  % 替换为您的文件夹路径
fileList = dir(fullfile(folderPath, '*'));  % 获取所有文件
fileNames = {fileList.name}; 
% 提取文件名中的频率信息并转换为数值，用于自然顺序排序
fileFrequencies = cellfun(@(x) str2double(regexp(x, '\d+(?=MHz)', 'match', 'once')), fileNames);

% 按频率进行自然升序排序，确保100MHz, 200MHz, ..., 9980MHz的顺序
[~, sortedIndices] = sort(fileFrequencies);
fileNames = fileNames(sortedIndices);  % 重新排序文件名列表

% 遍历文件
for i = 1:length(fileNames(3:end))
        oldName = fileNames{i+2};
        [~, name, ext] = fileparts(oldName);  % 分离文件名和扩展名
        
        % === 在此处定义您的重命名规则 ===
        newName = ['C1_Sub1_' sprintf('%04d', (i-1)*100+1000) 'MHz_am' ext];
        
        % 重命名文件
        oldFile = fullfile(folderPath, oldName);
        newFile = fullfile(folderPath, newName);
        movefile(oldFile, newFile,'f');
end



%% recover qam am
 myfilename = sprintf('C1_Sub1_%04dMHz_am.txt', 0);
 n = 10000;
 % 设计 FIR 低通滤波器（Hamming 窗）
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
for k = 1:4
  myfilename = sprintf('C1_Sub1_%04dMHz_am.txt', (k-1)*100+1000);
  idealname = sprintf('C1_Sub2_%04dMHz_am.txt', (k-1)*100+1000);
  filename=strcat("D:\Correction\0-Denoise\data1125\ShiShi5ns-80GSPS-20M\C1_Sub1_1000MHz_am\",myfilename);
  idealdataname=strcat("D:\Correction\0-Denoise\data1125\ShiShi5ns-80GSPS-20M\C1_Sub1_1000MHz_am\",idealname);
  mydata{k} = readmatrix(filename);
  idlewave{k} = readmatrix(idealdataname);
%   idlewave{k} = idlewave{k}(1:2:end);
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  clear trs;
      if size(mydata{k}, 2) >= 2
        mydata{k} = mydata{k}(:, 2);     
      end
  mydata{k} = mydata{k}(1:n);
  mydata{k} = (mydata{k} - min(mydata{k})) / (max(mydata{k}) - min(mydata{k}));
  writematrix( mydata{k}, filename);
%   idlewave{k} = idlewave{k}(1:n);
  idlewave{k} = (idlewave{k} - min(idlewave{k})) / (max(idlewave{k}) - min(idlewave{k}));
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
  writematrix( idlewave{k}, idealdataname);
%   figure;
%   subplot(2,1,1);
%   sfdr(mydata{k},fs);
%   subplot(2,1,2);
%   sfdr(idlewave{k},fs);
end


%% zhengxian (5ns 实时档 - 最终修正版 Fs=80G) - 20G滤波+自动扫描文件版
% 自动遍历文件夹下所有 C1_Sub1_*.txt 文件，无需手动设置频率范围
% 含 20GHz 零相移低通滤波

clc; clear; close all;

% =============================================================
% 1. 关键参数设置
% =============================================================
fs_real = 80e9;   % 采样率
n = 10000;        % 截取长度

% --- 滤波器设置 (20GHz) ---
cutoff_freq = 20e9;
nyquist_freq = fs_real / 2;
Wn = cutoff_freq / nyquist_freq;
[b_filt, a_filt] = butter(6, Wn, 'low'); % 6阶巴特沃斯

% =============================================================
% 2. 路径设置
% =============================================================
% 请确保路径结尾带斜杠，或者使用 fullfile
working_folder = 'D:\Correction\0-Denoise\data0202-100M20G25M20mv\';

if ~exist(working_folder, 'dir')
    error('文件夹路径不存在: %s', working_folder);
end

% =============================================================
% 3. 自动获取文件列表 (关键修改)
% =============================================================
% 查找所有符合 C1_Sub1_*.txt 模式的文件
file_structs = dir(fullfile(working_folder, 'C1_Sub1_*.txt'));
num_files = length(file_structs);

fprintf('在文件夹中找到 %d 个 Sub1 原始文件。\n', num_files);
fprintf('开始处理...\n');

% 记录失败的文件
failed_files = {}; 

% =============================================================
% 4. 循环处理
% =============================================================
for i = 1:num_files
    
    input_name = file_structs(i).name;
    full_input_path = fullfile(working_folder, input_name);
    
    % --- 从文件名中提取频率 ---
    % 使用正则表达式提取数字，例如从 "C1_Sub1_100MHz.txt" 提取 "100"
    tokens = regexp(input_name, 'C1_Sub1_(\d+)MHz.txt', 'tokens');
    
    if isempty(tokens)
        fprintf('  [跳过] 文件名格式无法解析频率: %s\n', input_name);
        failed_files{end+1} = [input_name, ' (文件名格式错误)']; %#ok<SAGROW>
        continue;
    end
    
    current_freq_mhz = str2double(tokens{1}{1});
    current_freq_hz = current_freq_mhz * 1e6;
    
    % --- 读取数据 ---
    try
        raw_temp = readmatrix(full_input_path);
    catch ME
        fprintf('  [读取失败] %s: %s\n', input_name, ME.message);
        failed_files{end+1} = [input_name, ' (无法读取)']; %#ok<SAGROW>
        continue;
    end

    % 处理列
    if size(raw_temp, 2) >= 2
        raw_signal = raw_temp(:, 2);
    else
        raw_signal = raw_temp;
    end
    
    % 截取
    if length(raw_signal) > n
        raw_signal = raw_signal(1:n);
    end
    
    % --- 滤波 (20GHz) ---
    try
        % 如果数据太短，filtfilt会报错
        if length(raw_signal) > 18 % 6阶滤波器至少需要一点长度
            curr_data = filtfilt(b_filt, a_filt, raw_signal);
        else
            curr_data = raw_signal; % 数据太短不滤波
        end
    catch ME
        fprintf('  [滤波警告] %s: %s (使用原始数据)\n', input_name, ME.message);
        curr_data = raw_signal;
    end
    
    % --- 拟合 ---
    len = length(curr_data);
    t_vec = (0:len-1)' / fs_real; 
    omega = 2 * pi * current_freq_hz;
    
    M = [ones(len, 1), cos(omega * t_vec), sin(omega * t_vec)];
    
    try
        % 警告：如果频率极低或数据全为0，这里可能会警告矩阵奇异
        lastwarn(''); % 清空警告
        coeffs = M \ curr_data;
        [~, msgid] = lastwarn;
        if strcmp(msgid, 'MATLAB:rankDeficientMatrix') || strcmp(msgid, 'MATLAB:nearlySingularMatrix')
             % 可以在这里选择是否跳过，或者接受这种拟合
             % fprintf('  [拟合警告] 矩阵奇异: %s\n', input_name);
        end
    catch ME
        fprintf('  [拟合失败] %s: %s\n', input_name, ME.message);
        failed_files{end+1} = [input_name, ' (拟合失败)']; %#ok<SAGROW>
        continue;
    end
    
    % 生成波形
    ideal_signal = round(M * coeffs);
    
    % --- 保存 ---
    output_name = sprintf('C1_Sub2_%dMHz.txt', current_freq_mhz);
    full_output_path = fullfile(working_folder, output_name);
    
    fid = fopen(full_output_path, 'w');
    if fid == -1
        fprintf('  [写入失败] %s\n', output_name);
        failed_files{end+1} = [input_name, ' (无法写入)']; %#ok<SAGROW>
        continue;
    end
    fprintf(fid, '%d\n', ideal_signal);
    fclose(fid);
    
    % 进度
    if mod(i, 100) == 0
        fprintf('进度: %d / %d\n', i, num_files);
    end
end

% =============================================================
% 5. 结果汇总
% =============================================================
fprintf('------------------------------------------------\n');
fprintf('处理完成。\n');
fprintf('原始文件数: %d\n', num_files);
fprintf('失败文件数: %d\n', length(failed_files));

if ~isempty(failed_files)
    fprintf('以下文件处理失败:\n');
    for k = 1:length(failed_files)
        fprintf('  %s\n', failed_files{k});
    end
else
    fprintf('所有文件处理成功！应有总文件数: %d\n', num_files * 2);
end

%% zhengxian (5ns 实时档 - 最终修正版 Fs=80G) - 鲁棒性处理版
% 频率范围：2000MHz - 20000MHz，步进 15MHz

clc; clear; close all;

% =============================================================
% 1. 关键参数设置
% =============================================================
fs_real = 80e9;   % 采样率 80 GSa/s
n = 10000;        % 截取长度

% =============================================================
% 2. 路径设置
% =============================================================
input_folder = 'D:\Correction\0-Denoise\data0105-100M2G-5M\';
output_folder = 'D:\Correction\0-Denoise\data0105-100M2G-5M\'; % 输出在同一文件夹，注意文件名区分
if ~exist(output_folder, 'dir'), mkdir(output_folder); end

% =============================================================
% 3. 生成理论频率列表 (关键修改)
% =============================================================
% 不要写死，而是根据物理意义生成完整列表
start_freq = 100;
end_freq = 2000;
step_freq = 5;
freq_list = start_freq:step_freq:end_freq; % 这是一个包含1201个频率点的向量

% 预分配 cell 数组以提高内存效率 (按最大理论长度分配)
% 注意：缺失频率对应的位置将为空
mydata = cell(1, length(freq_list));
idlewave = cell(1, length(freq_list));

fprintf('开始处理数据，频率范围 %d-%d MHz，共 %d 个理论频点...\n', ...
    start_freq, end_freq, length(freq_list));

% =============================================================
% 4. 循环处理
% =============================================================
% 使用 idx 作为索引，遍历理论频率列表
for idx = 1:length(freq_list)
    
    % 获取当前应该处理的频率
    current_freq_mhz = freq_list(idx);
    current_freq_hz = current_freq_mhz * 1e6;
    
    % 构造输入文件名
    myfilename = sprintf('C1_Sub1_%dMHz.txt', current_freq_mhz);
    full_input_path = fullfile(input_folder, myfilename);
    
    % --- 关键检查 ---
    % 检查文件是否存在，如果不存在则跳过，但 idx 依然会增加，
    % 保证了下一个循环处理的频率是正确的。
    if ~isfile(full_input_path)
        fprintf('  [跳过] 缺失文件: %d MHz\n', current_freq_mhz);
        continue; 
    end
    
    % 读取数据
    try
        raw_temp = readmatrix(full_input_path);
    catch
        fprintf('  [错误] 无法读取文件: %s\n', myfilename);
        continue;
    end

    % 处理数据格式 (兼容单列或多列)
    if size(raw_temp, 2) >= 2
        mydata{idx} = raw_temp(:, 2);
    else
        mydata{idx} = raw_temp;
    end
    
    % 截取长度
    if length(mydata{idx}) > n
        mydata{idx} = mydata{idx}(1:n);
    else
        % 如果数据长度不够 n，为了防止报错，可以做一个简单的处理
        % 比如补零或者只取现有长度，这里假设数据够长
        % disp(['警告: 数据长度不足 n，频率: ' num2str(current_freq_mhz)]);
    end
    
    % =============================================================
    % 核心对齐算法：最小二乘法拟合 (Least Squares Fitting)
    % =============================================================
    % 既然数据读进来了，mydata{idx} 肯定非空
    curr_data = mydata{idx}; 
    len = length(curr_data);
    
    t_vec = (0:len-1)' / fs_real; 
    omega = 2 * pi * current_freq_hz;
    
    % 构建方程：Data = Offset + A*cos(wt) + B*sin(wt)
    M = [ones(len, 1), cos(omega * t_vec), sin(omega * t_vec)];
    
    % 解方程
    % 使用 try-catch 防止矩阵奇异导致崩溃
    try
        coeffs = M \ curr_data;
    catch ME
        fprintf('  [拟合失败] 频率 %d MHz: %s\n', current_freq_mhz, ME.message);
        continue;
    end
    
    % 生成理想波形
    ideal_signal = M * coeffs;
    
    % 取整并存入 cell
    idlewave{idx} = round(ideal_signal);
    
    % 保存结果
    output_filename = sprintf('C1_Sub2_%dMHz.txt', current_freq_mhz);
    full_output_path = fullfile(output_folder, output_filename);
    
    fileID = fopen(full_output_path, 'w');
    if fileID == -1
        fprintf('  [错误] 无法写入文件: %s\n', output_filename);
        continue;
    end
    fprintf(fileID, '%d\n', idlewave{idx});
    fclose(fileID);
    
    % 进度提示 (每50个提示一次)
    if mod(idx, 50) == 0
        fprintf('已处理进度: %.1f%% (%d MHz)\n', (idx/length(freq_list))*100, current_freq_mhz);
    end
end

fprintf('------------------------------------------------\n');
fprintf('全部处理完成。\n');






%% zhengxian
%预分配内存
 myfilename = sprintf('C1_Sub1_%dMHz.txt', 0);
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
 n = 10000;
for k = 1:381
  myfilename = sprintf('C1_Sub1_%04dMHz.txt', (k-1)*5+100);
  filename=strcat("D:\Correction\0-Denoise\data1125\ShiShi5ns\",myfilename);
  mydata{k} = load(filename);
  mydata{k} = mydata{k}(1:n);
  % writematrix( mydata{k}, filename); %不要覆盖掉原文件，故注释

  clean_signal = wdenoise(mydata{k}, 'Wavelet', 'sym8', ... 
                        'DenoisingMethod', 'UniversalThreshold');
  peakdistance = round(fs/(((k-1)*5+100)*1e6));
  pks = findpeaks(clean_signal,'MinPeakDistance',peakdistance-5);
  trs = findpeaks(-clean_signal,'MinPeakDistance',peakdistance-5);
  mean_pks = mean(pks);
  mean_trs = mean(-trs);  
  amp = mean_pks - mean_trs;
  idlewave{k} = amp*1/2*sin(2*pi*((k-1)*5+100)*1e6*t)+mean(mydata{k});    
  idlewave{k} = round(idlewave{k});
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
  output_filename = sprintf('C1_Sub2_%04dMHz.txt',(k-1)*5+100);
  output_folder = "D:\Correction\0-Denoise\data1125\ShiShi5ns-pre\";
  full_output_path = fullfile(output_folder, output_filename);
  fileID = fopen(full_output_path, 'w');
  fprintf(fileID,'%d\n',idlewave{k});
end

%% zhengxian (5ns 实时档 - 最终修正版 Fs=80G)
% 频率范围：100MHz - 2000MHz，步进 5MHz

% =============================================================
% 1. 关键参数设置
% 根据频谱验证，采样率确定为 80 GSa/s
% =============================================================
fs_real = 80e9;   % 【修正】改为 80e9
n = 10000;        % 截取长度

% 2. 路径设置
input_folder = 'D:\Correction\0-Denoise\data0126-100M6G11G20G25M\';
output_folder = 'D:\Correction\0-Denoise\data0126-100M6G11G20G25M\';
if ~exist(output_folder, 'dir'), mkdir(output_folder); end

% 3. 循环处理
for k = 1:797
    % 计算频率
    current_freq_mhz = (k-1)*25 + 100;
    current_freq_hz = current_freq_mhz * 1e6;
    
    % 构造文件名
    myfilename = sprintf('C1_Sub1_%dMHz.txt', current_freq_mhz);
    full_input_path = fullfile(input_folder, myfilename);
    
    if ~isfile(full_input_path), continue; end
    
    % 读取数据
    raw_temp = readmatrix(full_input_path);
    if size(raw_temp, 2) >= 2
        mydata{k} = raw_temp(:, 2);
    else
        mydata{k} = raw_temp;
    end
    
    % 截取长度
    if length(mydata{k}) > n
        mydata{k} = mydata{k}(1:n);
    end
    
    % =============================================================
    % 核心对齐算法：最小二乘法拟合 (Least Squares Fitting)
    % =============================================================
    len = length(mydata{k});
    t_vec = (0:len-1)' / fs_real; % 使用 80e9
    omega = 2 * pi * current_freq_hz;
    
    % 构建方程：Data = Offset + A*cos(wt) + B*sin(wt)
    M = [ones(len, 1), cos(omega * t_vec), sin(omega * t_vec)];
    
    % 解方程
    coeffs = M \ mydata{k};
    
    % 生成理想波形 (自动对齐相位、幅度、偏置)
    idlewave{k} = M * coeffs;
    
    % 取整
    idlewave{k} = round(idlewave{k});
    
    % 保存结果
    output_filename = sprintf('C1_Sub2_%dMHz.txt', current_freq_mhz);
    full_output_path = fullfile(output_folder, output_filename);
    fileID = fopen(full_output_path, 'w');
    fprintf(fileID, '%d\n', idlewave{k});
    fclose(fileID);
    
    if mod(k, 50) == 0
        fprintf('已处理: %d MHz (Fs=80G)\n', current_freq_mhz);
    end
end
fprintf('5ns 档位 (80GSa/s) 处理完成。\n');

%% sin
% 200ps 插值档处理 (极简拟合版 - 无滤波，数学级对齐)
% 频率范围：100MHz - 2000MHz，步进 5MHz

% 1. 【修改】输入输出路径
input_folder = 'D:\Correction\0-Denoise\data1203\'; 
output_folder = 'D:\Correction\0-Denoise\data1203\';

% 自动创建输出文件夹
if ~exist(output_folder, 'dir'), mkdir(output_folder); end

n = 10000; % 截取长度 (200ps档 100MHz 约8000个点一周期，10000点足够拟合)

for k = 1:401
    % 计算频率
    current_freq_mhz = (k-1)*5 + 100;
    current_freq_hz = current_freq_mhz * 1e6;
    
    % 构造文件名
    myfilename = sprintf('C1_Sub1_%04dMHz.txt', current_freq_mhz);
    full_input_path = fullfile(input_folder, myfilename);
    
    if ~isfile(full_input_path)
        % 偶尔可能有没有的文件，跳过不报错
        continue; 
    end
    
    % 读取数据
    raw_temp = readmatrix(full_input_path);
    if size(raw_temp, 2) >= 2
        mydata{k} = raw_temp(:, 2);
    else
        mydata{k} = raw_temp;
    end
    
    % 长度限制
    if length(mydata{k}) > n
        mydata{k} = mydata{k}(1:n);
    end
    
    % ====================================================
    % 核心：直接拟合 (Least Squares Fit)
    % ====================================================
    len = length(mydata{k});
    t_vec = (0:len-1)'/fs; % 使用当前的 fs (800e9)
    omega = 2 * pi * current_freq_hz;
    
    % 构建方程: Data = Offset + A*cos + B*sin
    M = [ones(len, 1), cos(omega * t_vec), sin(omega * t_vec)];
    
    % 求解系数
    coeffs = M \ mydata{k};
    
    % 生成理想波形 (Ideal = M * coeffs)
    idlewave{k} = M * coeffs;
    
    % 取整
    idlewave{k} = round(idlewave{k});
    
    % 保存
    output_filename = sprintf('C1_Sub2_%04dMHz.txt', current_freq_mhz);
    full_output_path = fullfile(output_folder, output_filename);
    fileID = fopen(full_output_path, 'w');
    fprintf(fileID, '%d\n', idlewave{k});
    fclose(fileID);
    
    if mod(k, 50) == 0
        fprintf('已处理: %d MHz\n', current_freq_mhz);
    end
end
fprintf('200ps 档位数据处理完成。\n');

%% 方波 50
 myfilename = sprintf('C1_Sub1_%04dMHz_square50.txt', 0);
 n = 10000;
 % 设计 FIR 低通滤波器（Hamming 窗）
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
for k = 1:19
  myfilename = sprintf('C1_Sub1_%04dMHz_square50.txt', (k-1)*50+100);
  filename=strcat("D:\paper\mismatch\底噪新版\采集数据\2U_data\方波50 - 副本\",myfilename);
  mydata{k} = readmatrix(filename);
  clear trs;
      if size(mydata{k}, 2) >= 2
        mydata{k} = mydata{k}(:, 2);     
      end
  mydata{k} = mydata{k}(1:n);
  writematrix( mydata{k}, filename);
  f = (0:n-1)*(fs/n);         % 频率轴
  fft_signal{k} = abs(fft(mydata{k}));
  harmonic_ratios = [1, 3, 5, 7]; % 方波的奇次谐波比例
  [peaks, locs] = findpeaks(fft_signal{k}(2:floor(n/2)), 'SortStr', 'descend');
   main_freq{k} = f(locs(1)+1);

  clean_signal = wdenoise(mydata{k}, 'Wavelet', 'sym8', ... 
                        'DenoisingMethod', 'UniversalThreshold');
  peakdistance = round(fs/(((k-1)*50+100)*1e6));
  pks = findpeaks(clean_signal,'MinPeakDistance',peakdistance-5);
  trs = findpeaks(-clean_signal,'MinPeakDistance',peakdistance-5);
  mean_pks = mean(pks);
  mean_trs = mean(-trs);  
  amp = mean_pks - mean_trs;
%   idlewave{k} = amp*1/2*square(2*pi*main_freq{k}*t)+mean(mydata{k});  
  idlewave{k} = amp*1/2*square(2*pi*((k-1)*50+100)*1e6*t)+mean(mydata{k}); 
  % 应用 FIR 滤波器
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
  idlewave{k} = awgn(idlewave{k}, snr_db, 'measured');
%   idlewave{k} = round(idlewave{k});
  output_filename = sprintf('C1_Sub2_%04dMHz_square50.txt',(k-1)*50+100);
  output_folder = "D:\paper\mismatch\底噪新版\采集数据\2U_data\方波50 - 副本\";
  full_output_path = fullfile(output_folder, output_filename);
  fileID = fopen(full_output_path, 'w');
  fprintf(fileID,'%f\n',idlewave{k});
end

%% 方波10
 myfilename = sprintf('C1_Sub1_%04dMHz_square10.txt', 0);
 n = 10000;
 % 设计 FIR 低通滤波器（Hamming 窗）
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
for k = 1:19
  myfilename = sprintf('C1_Sub1_%04dMHz_square10.txt', (k-1)*50+100);
  filename=strcat("D:\paper\mismatch\底噪新版\采集数据\2U_data\方波10 - 副本\",myfilename);
  mydata{k} = readmatrix(filename);
  clear trs;
      if size(mydata{k}, 2) >= 2
        mydata{k} = mydata{k}(:, 2);
      end
  mydata{k} = mydata{k}(1:n);
  writematrix( mydata{k}, filename);
  f = (0:n-1)*(fs/n);         % 频率轴
  fft_signal{k} = abs(fft(mydata{k}));
  harmonic_ratios = [1, 3, 5, 7]; % 方波的奇次谐波比例
  [peaks, locs] = findpeaks(fft_signal{k}(2:floor(n/2)), 'SortStr', 'descend');
   main_freq{k} = f(locs(1)+1);

  clean_signal = wdenoise(mydata{k}, 'Wavelet', 'sym8', ... 
                        'DenoisingMethod', 'UniversalThreshold');
  peakdistance = round(fs/(((k-1)*50+100)*1e6));
  pks = findpeaks(clean_signal,'MinPeakDistance',peakdistance-5);
  trs = findpeaks(-clean_signal,'MinPeakDistance',peakdistance-5);
  mean_pks = mean(pks);
  mean_trs = mean(-trs);  
  amp = mean_pks - mean_trs;
%   idlewave{k} = amp*0.85*1/2*square(2*pi*main_freq{k}*t,10)+(max(mydata{k})+min(mydata{k}))/2;  
  idlewave{k} = amp*1/2*0.9*square(2*pi*((k-1)*50+100)*1e6*t,10)+(max(mydata{k})+min(mydata{k}))/2;  
  % 应用 FIR 滤波器
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
%   idlewave{k} = awgn(idlewave{k}, snr_db, 'measured');
% 
%   figure
%   plot(idlewave{k});
%   hold on
%   plot(mydata{k});

%   idlewave{k} = round(idlewave{k});
  output_filename = sprintf('C1_Sub2_%04dMHz_square10.txt',(k-1)*50+100);
  output_folder = "D:\paper\mismatch\底噪新版\采集数据\2U_data\方波10 - 副本\";
  full_output_path = fullfile(output_folder, output_filename);
  fileID = fopen(full_output_path, 'w');
  fprintf(fileID,'%f\n',idlewave{k});
end
%% 三角
 myfilename = sprintf('C1_Sub1_%04dMHz_saw50.txt', 0);
 n = 10000;
 % 设计 FIR 低通滤波器（Hamming 窗）
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
for k = 1:19
  myfilename = sprintf('C1_Sub1_%04dMHz_saw50.txt', (k-1)*50+100);
  filename=strcat("D:\paper\mismatch\底噪新版\采集数据\2U_data\三角波 - 副本\",myfilename);
  mydata{k} = readmatrix(filename);
  clear trs;
      if size(mydata{k}, 2) >= 2
        mydata{k} = mydata{k}(:, 2);
      end
  mydata{k} = mydata{k}(1:n);
  writematrix( mydata{k}, filename);

  f = (0:n-1)*(fs/n);         % 频率轴
  fft_signal{k} = abs(fft(mydata{k}));
  harmonic_ratios = [1, 3, 5, 7]; % 方波的奇次谐波比例
  [peaks, locs] = findpeaks(fft_signal{k}(2:floor(n/2)), 'SortStr', 'descend');
   main_freq{k} = f(locs(1)+1);

  clean_signal = wdenoise(mydata{k}, 'Wavelet', 'sym8', ... 
                        'DenoisingMethod', 'UniversalThreshold');
  peakdistance = round(fs/(((k-1)*50+100)*1e6));
  pks = findpeaks(clean_signal,'MinPeakDistance',peakdistance-5);
  trs = findpeaks(-clean_signal,'MinPeakDistance',peakdistance-5);
  mean_pks = mean(pks);
  mean_trs = mean(-trs);  
  amp = mean_pks - mean_trs;
  idlewave{k} = amp*1/2*sawtooth(2*pi*((k-1)*50+100)*1e6*t,0.5)+mean(mydata{k});  
  % 应用 FIR 滤波器
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
  idlewave{k} = awgn(idlewave{k}, snr_db, 'measured');
%   idlewave{k} = round(idlewave{k});
  output_filename = sprintf('C1_Sub2_%04dMHz_saw50.txt',(k-1)*50+100);
  output_folder = "D:\paper\mismatch\底噪新版\采集数据\2U_data\三角波 - 副本\";
  full_output_path = fullfile(output_folder, output_filename);
  fileID = fopen(full_output_path, 'w');
  fprintf(fileID,'%f\n',idlewave{k});
end
%% 锯齿
 myfilename = sprintf('C1_Sub1_%04dMHz_saw0.txt', 0);
 n = 10000;
 % 设计 FIR 低通滤波器（Hamming 窗）
 fir_filter = fir1(filter_order, fc/(fs/2), 'low', hamming(filter_order+1));
for k = 1:19
  myfilename = sprintf('C1_Sub1_%04dMHz_saw0.txt', (k-1)*50+100);
  filename=strcat("D:\paper\mismatch\底噪新版\采集数据\2U_data\锯齿波 - 副本\",myfilename);
  mydata{k} = readmatrix(filename);
  clear trs;
      if size(mydata{k}, 2) >= 2
        mydata{k} = mydata{k}(:, 2);
      end
  mydata{k} = mydata{k}(1:n);
  writematrix( mydata{k}, filename);

  f = (0:n-1)*(fs/n);         % 频率轴
  fft_signal{k} = abs(fft(mydata{k}));
  harmonic_ratios = [1, 3, 5, 7]; % 方波的奇次谐波比例
  [peaks, locs] = findpeaks(fft_signal{k}(2:floor(n/2)), 'SortStr', 'descend');
   main_freq{k} = f(locs(1)+1);

  clean_signal = wdenoise(mydata{k}, 'Wavelet', 'sym8', ... 
                        'DenoisingMethod', 'UniversalThreshold');
  peakdistance = round(fs/(((k-1)*50+100)*1e6));
  pks = findpeaks(clean_signal,'MinPeakDistance',peakdistance-5);
  trs = findpeaks(-clean_signal,'MinPeakDistance',peakdistance-5);
  mean_pks = mean(pks);
  mean_trs = mean(-trs);  
  amp = mean_pks - mean_trs;
  idlewave{k} = amp*1/2*sawtooth(2*pi*((k-1)*50+100)*1e6*t,0)+(max(mydata{k})+min(mydata{k}))/2;  
  % 应用 FIR 滤波器
  idlewave{k} = filter(fir_filter, 1, idlewave{k});
  idlewave{k} = idlewave{k}(501:length(mydata{k})+500);
  idlewave{k} = awgn(idlewave{k}, snr_db, 'measured');
  idlewave{k} = round(idlewave{k});
  output_filename = sprintf('C1_Sub2_%04dMHz_saw0.txt',(k-1)*50+100);
  output_folder = "D:\paper\mismatch\底噪新版\采集数据\2U_data\锯齿波 - 副本\";
  full_output_path = fullfile(output_folder, output_filename);
  fileID = fopen(full_output_path, 'w');
  fprintf(fileID,'%f\n',idlewave{k});
end




%% 7GHz-9GHz 终极修正版 (Hilbert 相位回归法)
% 原理：直接提取瞬时相位计算频率和初始相角，彻底解决相位对齐问题
% 采样率：400 GSa/s

% 1. 基础设置
fs_new = 80e9;    
n_points = 10000;  
folder_path = 'D:\Correction\0-Denoise\data1204\'; 

fprintf('开始处理 7GHz - 9GHz 数据 (Hilbert 相位锁定模式)...\n');

% 2. 循环处理
for k = 1:401
    % 理论频率 (仅用于文件名，计算完全依赖原始数据)
    center_freq_mhz = (k-1)*5 + 7000; 
    
    % 构造文件名
    filename_in = sprintf('C1_Sub1_%04dMHz.txt', center_freq_mhz); 
    full_input_path = fullfile(folder_path, filename_in);
    
    if ~isfile(full_input_path)
        continue; 
    end
    
    % 读取数据
    raw_temp = readmatrix(full_input_path);
    if size(raw_temp, 2) >= 2
        mydata_curr = raw_temp(:, 2);
    else
        mydata_curr = raw_temp;
    end
    
    % 截取长度
    len = length(mydata_curr);
    use_len = min(len, n_points);
    mydata_curr = mydata_curr(1:use_len);
    
    % 时间轴
    t_vec = (0:use_len-1)' / fs_new; 
    
    % =============================================================
    % 核心算法：Hilbert 相位回归 (Phase Regression)
    % =============================================================
    
    % 1. 去除直流偏置
    dc_offset = mean(mydata_curr);
    y_ac = mydata_curr - dc_offset;
    
    % 2. 计算解析信号 (Analytic Signal)
    % 这一步能获取信号的包络和瞬时相位
    z = hilbert(y_ac); 
    
    % 3. 提取幅度 (Envelope)
    % 取包络的均值作为 Ideal 的幅度，这能自动忽略噪声毛刺
    inst_amp = abs(z);
    % 为了避免边界效应影响，去掉头尾各100点再求平均
    if use_len > 200
        A_est = mean(inst_amp(100:end-100));
    else
        A_est = mean(inst_amp);
    end
    
    % 4. 提取相位 (Instantaneous Phase)
    inst_phase = unwrap(angle(z));
    
    % 5. 线性拟合相位 (Linear Fit on Phase)
    % 相位 phi(t) = w*t + phi_0
    % 同样去掉头尾各100点以消除 Hilbert 变换的边界效应
    valid_idx = 100: (use_len-100);
    if isempty(valid_idx), valid_idx = 1:use_len; end
    
    p = polyfit(t_vec(valid_idx), inst_phase(valid_idx), 1);
    
    w_est = p(1); % 拟合出的角频率 (rad/s)
    phi_0 = p(2); % 拟合出的初始相位 (rad)
    
    % 6. 重构完美正弦波
    % Ideal = A * cos(w*t + phi) + DC
    ideal_wave = A_est * cos(w_est * t_vec + phi_0) + dc_offset;
    
    % =============================================================
    
    % 数据取整 (针对ADC码值)
    ideal_wave = round(ideal_wave); 
    
    % 保存结果
    filename_out = sprintf('C1_Sub2_%04dMHz.txt', center_freq_mhz);
    full_output_path = fullfile(folder_path, filename_out);
    
    fileID = fopen(full_output_path, 'w');
    fprintf(fileID, '%d\n', ideal_wave); 
    fclose(fileID);
    
    if mod(k, 50) == 0
        % 计算一下实际频率打印出来看看
        real_freq_mhz = (w_est / (2*pi)) / 1e6;
        fprintf('处理: %d MHz | 实测频率: %.2f MHz | 幅度: %.1f\n', ...
            center_freq_mhz, real_freq_mhz, A_est);
    end
end

fprintf('全部完成！Hilbert 算法已生成相位完美对齐的 Ideal 文件。\n');


%% 7GHz-9GHz 终极对齐版 (FFT引导 + 变投影优化)
% 修复问题：解决相位严重错位和幅度不匹配
% 采样率：400 GSa/s

% 1. 基础设置
fs_new = 400e9;    
n_points = 10000;  
folder_path = 'D:\Correction\0-Denoise\data1202\'; 

fprintf('开始处理 7GHz - 9GHz 数据 (FFT + 变投影优化模式)...\n');

% 2. 循环处理
for k = 1:401
    % 理论频率 (文件名参考)
    center_freq_mhz = (k-1)*5 + 7000; 
    
    % 构造文件名
    filename_in = sprintf('C1_Sub1_%04dMHz.txt', center_freq_mhz); 
    full_input_path = fullfile(folder_path, filename_in);
    
    if ~isfile(full_input_path)
        continue; 
    end
    
    % 读取数据
    raw_temp = readmatrix(full_input_path);
    if size(raw_temp, 2) >= 2
        mydata_curr = raw_temp(:, 2);
    else
        mydata_curr = raw_temp;
    end
    
    % 截取长度
    len = length(mydata_curr);
    use_len = min(len, n_points);
    mydata_curr = mydata_curr(1:use_len);
    
    % 时间轴
    t_vec = (0:use_len-1)' / fs_new; 
    
    % =============================================================
    % 步骤1: FFT 粗略寻找主频 (防止频率搜索陷入局部极值)
    % =============================================================
    L = use_len;
    Y = fft(mydata_curr - mean(mydata_curr));
    P2 = abs(Y/L);
    P1 = P2(1:floor(L/2)+1);
    f_axis = fs_new*(0:(L/2))/L;
    [~, max_idx] = max(P1);
    fft_freq_est = f_axis(max_idx); % FFT 估计的频率
    
    % 如果FFT估计太离谱(比如偏离理论值超过5%)，强制归位到理论值
    if abs(fft_freq_est - (center_freq_mhz*1e6)) > (center_freq_mhz*1e6 * 0.05)
         fft_freq_est = center_freq_mhz * 1e6;
    end

    % =============================================================
    % 步骤2: 变投影优化 (Variable Projection)
    % 我们只需要搜索频率 f，对于给定的 f，相位和幅度通过矩阵求逆直接得到最优解
    % =============================================================
    
    % 定义目标函数：输入频率 freq，返回拟合残差
    % 搜索范围：FFT频率 ±1%
    fit_func = @(f) get_residual(f, t_vec, mydata_curr);
    
    options = optimset('TolX', 1e-4, 'Display', 'off');
    best_freq = fminbnd(fit_func, fft_freq_est * 0.99, fft_freq_est * 1.01, options);
    
    % =============================================================
    % 步骤3: 使用最佳频率生成最终波形
    % =============================================================
    omega = 2 * pi * best_freq;
    % 构建基矩阵 [直流, cos, sin]
    M = [ones(use_len, 1), cos(omega * t_vec), sin(omega * t_vec)];
    % 求解最佳系数 (coeffs包含 Offset, A, B)
    coeffs = M \ mydata_curr;
    
    % 生成 Ideal Wave
    ideal_wave = M * coeffs;
    
    % 数据取整
    ideal_wave = round(ideal_wave); 
    
    % 保存结果
    filename_out = sprintf('C1_Sub2_%04dMHz.txt', center_freq_mhz);
    full_output_path = fullfile(folder_path, filename_out);
    
    fileID = fopen(full_output_path, 'w');
    fprintf(fileID, '%d\n', ideal_wave); 
    fclose(fileID);
    
    if mod(k, 50) == 0
        fprintf('已处理: %d MHz | 锁定频率: %.2f MHz\n', ...
            center_freq_mhz, best_freq/1e6);
    end
end

fprintf('全部完成！采用 FFT+变投影法，相位应已完美对齐。\n');





%% 7GHz-9GHz 终极对齐版 (FFT引导 + 变投影优化)
% 针对 400G 采样率，7G-9G 范围，5MHz 步进
% 输入: C1_Sub1_xxxxMHz.txt
% 输出: C1_Sub2_xxxxMHz.txt (保存在同一文件夹)

clear %以此行开头，避免误删变量，也可直接 clear
clc

% 1. 基础设置 (根据你的要求修改)
fs_new = 400e9;     % 采样率 400G
n_points = 10000;   % 处理的数据点数
folder_path = 'D:\Correction\0-Denoise\data1202\'; % 数据路径

fprintf('开始处理 7GHz - 9GHz 数据 (FFT + 变投影优化模式)...\n');

% 2. 循环处理
% 范围: 7000MHz 到 9000MHz，步进 5MHz
% 计算次数: (9000 - 7000) / 5 + 1 = 401 次
for k = 1:401
    % 理论频率 (文件名参考)
    center_freq_mhz = (k-1)*5 + 7000; 
    
    % 构造输入文件名
    filename_in = sprintf('C1_Sub1_%04dMHz.txt', center_freq_mhz); 
    full_input_path = fullfile(folder_path, filename_in);
    
    % 检查文件是否存在
    if ~isfile(full_input_path)
        fprintf('跳过缺失文件: %s\n', filename_in);
        continue; 
    end
    
    % 读取数据
    try
        raw_temp = readmatrix(full_input_path);
        % 兼容多列格式，如果只有一列则直接用，如果有两列(时间,电压)取第二列
        if size(raw_temp, 2) >= 2
            mydata_curr = raw_temp(:, 2);
        else
            mydata_curr = raw_temp;
        end
        
        % 截取长度
        len = length(mydata_curr);
        if len < n_points
             warning('文件 %s 数据长度不足 %d 点', filename_in, n_points);
             use_len = len;
        else
             use_len = n_points;
        end
        mydata_curr = mydata_curr(1:use_len);
        
        % 时间轴
        t_vec = (0:use_len-1)' / fs_new; 
        
        % =============================================================
        % 步骤1: FFT 粗略寻找主频
        % =============================================================
        L = use_len;
        Y = fft(mydata_curr - mean(mydata_curr));
        P2 = abs(Y/L);
        P1 = P2(1:floor(L/2)+1);
        f_axis = fs_new*(0:(L/2))/L;
        [~, max_idx] = max(P1);
        fft_freq_est = f_axis(max_idx); 
        
        % 容错: 如果FFT跑偏超过 5%，强制归位到理论值
        if abs(fft_freq_est - (center_freq_mhz*1e6)) > (center_freq_mhz*1e6 * 0.05)
             fft_freq_est = center_freq_mhz * 1e6;
        end
    
        % =============================================================
        % 步骤2: 变投影优化 (Variable Projection) - 寻找最佳拟合频率
        % =============================================================
        fit_func = @(f) get_residual(f, t_vec, mydata_curr);
        options = optimset('TolX', 1e-4, 'Display', 'off');
        % 在 FFT 频率 ±1% 范围内搜索精确频率
        best_freq = fminbnd(fit_func, fft_freq_est * 0.99, fft_freq_est * 1.01, options);
        
        % =============================================================
        % 步骤3: 生成最终波形 (最小二乘法解幅度和相位)
        % =============================================================
        omega = 2 * pi * best_freq;
        % 构建基矩阵 [直流分量, cos分量, sin分量]
        M = [ones(use_len, 1), cos(omega * t_vec), sin(omega * t_vec)];
        coeffs = M \ mydata_curr;
        
        % 生成 Ideal Wave
        ideal_wave = M * coeffs;
        
        % 数据取整 (模拟ADC输出格式)
        ideal_wave = round(ideal_wave); 
        
        % 保存结果 (Sub2)
        filename_out = sprintf('C1_Sub2_%04dMHz.txt', center_freq_mhz);
        full_output_path = fullfile(folder_path, filename_out);
        
        fileID = fopen(full_output_path, 'w');
        fprintf(fileID, '%d\n', ideal_wave); 
        fclose(fileID);
        
        if mod(k, 50) == 0 || k == 1
            fprintf('已处理: %d MHz | 锁定频率: %.4f MHz\n', center_freq_mhz, best_freq/1e6);
        end
        
    catch ME
        fprintf('处理文件 %s 时出错: %s\n', filename_in, ME.message);
    end
end

fprintf('全部完成！Ideal文件已生成在 %s\n', folder_path);

% =============================================================
% 辅助函数：计算残差 (需放在脚本最后)
% =============================================================
function err = get_residual(f, t, y)
    w = 2 * pi * f;
    A = [ones(length(t),1), cos(w*t), sin(w*t)];
    c = A \ y; 
    y_fit = A * c;
    err = sum((y - y_fit).^2);
end



% % =============================================================
% % 辅助函数：计算残差
% % =============================================================
% function err = get_residual(f, t, y)
%     w = 2 * pi * f;
%     % 构造线性方程组矩阵: y = c1 + c2*cos(wt) + c3*sin(wt)
%     A = [ones(length(t),1), cos(w*t), sin(w*t)];
%     % 最小二乘求解线性参数
%     c = A \ y; 
%     % 计算拟合波形
%     y_fit = A * c;
%     % 返回误差平方和
%     err = sum((y - y_fit).^2);
% end


