 clc; clear; close all;

% =============================================================
% 1. 主文件夹路径
% =============================================================
root_folder = 'D:\paper\mismatch\底噪新版\wave_acq\';

if ~exist(root_folder, 'dir')
    error('主文件夹不存在: %s', root_folder);
end

% =============================================================
% 2. 参数设置
% =============================================================
N_keep = 2000;   % 只保留前 2000 个点

% =============================================================
% 3. 获取所有子文件夹
% =============================================================
folder_list = dir(root_folder);
folder_list = folder_list([folder_list.isdir]);

% 去掉 . 和 ..
folder_names = {folder_list.name};
folder_names = folder_names(~ismember(folder_names, {'.', '..'}));

fprintf('共找到 %d 个子文件夹。\n', length(folder_names));

% =============================================================
% 4. 遍历子文件夹
% =============================================================
for i = 1:length(folder_names)

    curr_folder_name = folder_names{i};

    % 跳过带 _back 后缀的文件夹
    if endsWith(curr_folder_name, '_back') || strcmp(curr_folder_name, 'single')
        fprintf('[跳过] %s\n', curr_folder_name);
        continue;
    end

    curr_folder_path = fullfile(root_folder, curr_folder_name);

    % 查找当前文件夹下所有 csv 文件
    csv_files = dir(fullfile(curr_folder_path, '*.csv'));

    if isempty(csv_files)
        fprintf('[无csv] %s\n', curr_folder_name);
        continue;
    end

    fprintf('正在处理文件夹: %s, csv文件数: %d\n', curr_folder_name, length(csv_files));

    % =========================================================
    % 5. 遍历当前文件夹内 csv 文件
    % =========================================================
    for j = 1:length(csv_files)

        csv_name = csv_files(j).name;
        csv_path = fullfile(curr_folder_path, csv_name);

        try
            data = readmatrix(csv_path);
        catch ME
            fprintf('  [读取失败] %s: %s\n', csv_name, ME.message);
            continue;
        end

        % 去掉全 NaN 行
        data = data(~all(isnan(data), 2), :);

        if isempty(data)
            fprintf('  [空文件] %s\n', csv_name);
            continue;
        end

        % 如果 csv 有两列或多列，默认第 2 列是波形；
        % 如果只有一列，就取第 1 列。
        if size(data, 2) >= 2
            wave = data(:, 2);
        else
            wave = data(:, 1);
        end

        wave = wave(:);

        % 去掉 NaN / Inf
        wave = wave(isfinite(wave));

        if isempty(wave)
            fprintf('  [无有效数据] %s\n', csv_name);
            continue;
        end

        % 只取前 2000 个点
        if length(wave) >= N_keep
            wave_out = wave(1:N_keep);
        else
            wave_out = wave;
            fprintf('  [警告] %s 数据不足 %d 点，实际只有 %d 点\n', ...
                csv_name, N_keep, length(wave));
        end

        % 输出 txt 文件名：与 csv 同名，只改后缀
        [~, name_no_ext, ~] = fileparts(csv_name);
        txt_name = [name_no_ext, '.txt'];
        txt_path = fullfile(curr_folder_path, txt_name);

        % 保存为 txt
        fid = fopen(txt_path, 'w');
        if fid == -1
            fprintf('  [写入失败] %s\n', txt_name);
            continue;
        end

        fprintf(fid, '%.15g\n', wave_out);
        fclose(fid);

        fprintf('  已生成: %s\n', txt_name);
    end
end

fprintf('------------------------------------------------\n');
fprintf('全部处理完成。\n');