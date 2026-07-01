%% 初始化设置
clc; clear; close all;

% ================= 配置区域 =================
% 原始数据所在的那个大文件夹路径 (请确保路径正确，不要最后带反斜杠)
sourceDir = 'D:\paper\mismatch\底噪新版\DestData0608\DestData\ch1';

% 输出根目录 (会自动创建)
outputDir = 'D:\paper\mismatch\底噪新版\DestData0608\DestData\ch1\SplitData-0608_80_20';

% 训练集/验证集占比 (0.8 表示 80%)
trainRatio = 0.8;
% ===========================================

% 定义输出的具体路径 (输出时帮你把 sub1 和 sub2 分开存放，方便训练)
dstTrainSub1 = fullfile(outputDir, 'TrainVal', 'sub1');
dstTrainSub2 = fullfile(outputDir, 'TrainVal', 'sub2');
dstTestSub1  = fullfile(outputDir, 'Test', 'sub1');
dstTestSub2  = fullfile(outputDir, 'Test', 'sub2');

% 检查源文件夹是否存在
if ~isfolder(sourceDir)
    error('错误: 源文件夹不存在: %s', sourceDir);
end

% 创建输出文件夹结构
if ~isfolder(dstTrainSub1), mkdir(dstTrainSub1); end
if ~isfolder(dstTrainSub2), mkdir(dstTrainSub2); end
if ~isfolder(dstTestSub1),  mkdir(dstTestSub1); end
if ~isfolder(dstTestSub2),  mkdir(dstTestSub2); end

% --- 核心修改：匹配文件名模式 ---
% 获取所有文件名包含 "Sub1" 的 txt 文件
% 假设文件名格式如截图所示：C1_Sub1_100MHz.txt (注意大小写)
fileList = dir(fullfile(sourceDir, '*Sub1*.txt')); 

numFiles = length(fileList);

if numFiles == 0
    % 如果没找到，尝试一下小写 sub1
    fileList = dir(fullfile(sourceDir, '*sub1*.txt'));
    numFiles = length(fileList);
end

if numFiles == 0
    error('错误: 在文件夹中未找到包含 Sub1 或 sub1 的txt文件。请检查文件名格式！');
end

fprintf('开始处理，共找到 %d 组配对文件...\n', numFiles);

%% 循环处理每个文件
for i = 1:numFiles
    fileName1 = fileList(i).name;
    
    % --- 核心修改：生成对应的 Sub2 文件名 ---
    % 将文件名中的 "Sub1" 替换为 "Sub2" (根据截图，首字母是大写S)
    if contains(fileName1, 'Sub1')
        fileName2 = strrep(fileName1, 'Sub1', 'Sub2');
    else
        fileName2 = strrep(fileName1, 'sub1', 'sub2'); % 兼容小写
    end
    
    % 构建完整路径
    path1 = fullfile(sourceDir, fileName1);
    path2 = fullfile(sourceDir, fileName2);
    
    % 检查对应的标签文件是否存在
    if ~isfile(path2)
        warning('文件 %s 对应的标签文件 %s 不存在，跳过！', fileName1, fileName2);
        continue;
    end
    
    % --- 读取数据 ---
    try
        % 读取数据 (假设数据是纯数字矩阵)
        data1 = readmatrix(path1); 
        data2 = readmatrix(path2);
    catch ME
        warning('读取文件 %s 出错: %s', fileName1, ME.message);
        continue;
    end
    
    % --- 数据校验 ---
    [rows1, cols1] = size(data1);
    [rows2, cols2] = size(data2);
    
    % 确保行数一致 (防止示波器丢点导致长度不一)
    if rows1 ~= rows2
        % warning('文件对 (%s) 长度不一致: %d vs %d，将自动截断对齐。', fileName1, rows1, rows2);
        minRows = min(rows1, rows2);
        data1 = data1(1:minRows, :);
        data2 = data2(1:minRows, :);
        rows1 = minRows;
    end
    
    % --- 计算切分点 ---
    % 前 80% 作为训练，后 20% 作为测试
    splitIdx = floor(rows1 * trainRatio);
    
    if splitIdx == 0 || splitIdx == rows1
        warning('文件 %s 数据行数过少，无法切分，跳过。', fileName1);
        continue;
    end
    
    % 切分数据
    trainData1 = data1(1:splitIdx, :);
    trainData2 = data2(1:splitIdx, :);
    
    testData1 = data1(splitIdx+1:end, :);
    testData2 = data2(splitIdx+1:end, :);
    
    % --- 保存数据 ---
    % 注意：输出时，我们把 sub1 和 sub2 放到了不同的文件夹里，文件名保持不变
    % 这样你的神经网络代码读取时会更方便
    
    % 保存 TrainVal
    writematrix(trainData1, fullfile(dstTrainSub1, fileName1), 'Delimiter', ' ');
    writematrix(trainData2, fullfile(dstTrainSub2, fileName2), 'Delimiter', ' ');
    
    % 保存 Test
    writematrix(testData1, fullfile(dstTestSub1, fileName1), 'Delimiter', ' ');
    writematrix(testData2, fullfile(dstTestSub2, fileName2), 'Delimiter', ' ');
    
    % 进度提示
    if mod(i, 50) == 0
        fprintf('已处理: %d / %d (%.1f%%)\n', i, numFiles, (i/numFiles)*100);
    end
end

fprintf('处理完成！\n数据已保存至: %s\n', outputDir);
fprintf('结构如下：\n');
fprintf('  - TrainVal (前80%%) -> sub1 / sub2\n');
fprintf('  - Test     (后20%%) -> sub1 / sub2\n');