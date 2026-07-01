# snrTrend 目录说明

本目录用于完成以下完整链路：

1. 从原始 `sub1/sub2` 文本波形生成训练/验证/测试数据集（`pkl`）。
2. 训练时域去噪模型并导出 `pth/onnx`。
3. 在测试集上按频点推理，统计并绘制 `SNR` 或 `ENOB` 随频率变化趋势。

---

## 1. 目录框架

```text
snrTrend/
├── data/                        # 原始数据与分割后的数据目录
│   ├── SplitData_80_20/
│   ├── SplitData_80_20_0402/
│   └── SplitData-0202_80_20/
│       ├── TrainVal/sub1, sub2
│       └── Test/sub1, sub2
├── dataset/                     # 生成后的 pkl 数据集与数据构建脚本
│   ├── create_dataset.py
│   ├── create_dataset_260401.py
│   ├── train_dataset_*.pkl
│   ├── val_dataset_*.pkl
│   └── test_dataset_*.pkl
├── denoise.py                   # 基线训练脚本（GRUCNNModel）
├── denoise_ablation.py          # 消融/改型训练脚本（GRUCNNmodModel）
├── snrtrend.py                  # 基于 ONNX 的 SNR 趋势评估与作图
├── enobtrend.py                 # 基于 ONNX 的 ENOB 趋势评估与作图
├── saved_model/                 # 训练得到的 pth/onnx 模型
├── results/                     # loss、val_loss 文本与对比图
├── figure100Middle*/            # SNR/ENOB 趋势图与代表频点波形图
└── README.md
```

说明：
- `sub1` 通常作为带噪输入，`sub2` 作为对应标签（参考信号）。
- 训练模型定义来自上级目录 `../boardband/models.py`。

---

## 2. 数据处理逻辑（dataset/create_dataset*.py）

核心流程一致：

1. 读取 `sub1/sub2` 同频点文本文件。
2. 逐文件归一化到 `[-1, 1]`。
3. 用互相关做时序对齐（`align_wave`）。
4. 采用滑窗切片：
- 窗长 `240`
- 步长 `100`
5. 形成样本格式：`(N, 2, 240)`，其中
- `[:, 0, :]` 为输入
- `[:, 1, :]` 为标签
6. 保存为 `pickle`：`train/val/test_dataset_*.pkl`。

两个脚本主要区别：
- `create_dataset.py`：按训练验证文件夹组织来生成 `*_0202.pkl`。
- `create_dataset_260401.py`：按 7:1:2 比例切分的另一版流程，生成 `*_260401.pkl`。

---

## 3. 训练逻辑

### 3.1 `denoise.py`（基线）

- 模型：`GRUCNNModel(input_size=1, hidden_size=32, output_size=1)`。
- 数据输入形状：`(batch, 240, 1)`。
- 损失：当前配置实际使用时域 `MSELoss`（`FREQ_WEIGHT=0`）。
- 训练策略：Adam + 早停（`patience_limit=50`）。
- 输出：
- `saved_model/best_model_0202.pth`
- `saved_model/denoise_model_0202.pth`
- `saved_model/denoise_model_0202.onnx`
- `results/loss_0202.txt`, `results/val_loss_0202.txt`

### 3.2 `denoise_ablation.py`（消融/结构变体）

- 模型：`GRUCNNmodModel(...)`。
- 增加了可复现实验设置（随机种子）、参数量统计、时间/显存统计。
- 也保留了频域损失接口（当前主路径仍为时域损失）。
- 会导出 `0414` 命名模型与 loss 文件，并可绘制 `0202 vs 0303` 的 loss dB 对比图。

---

## 4. 评估与趋势图逻辑

### 4.1 `snrtrend.py`

用途：加载 `ONNX` 模型，在测试集上逐频点推理并绘制 `SNR Trend vs Frequency`。

关键步骤：

1. 读取 `test_dataset_0202.pkl`，按频点分块（默认每频点 `18` 个窗口）。
2. ONNX 推理得到去噪输出。
3. 窗口拼接策略：每个 `240` 点窗口仅取中间 `100` 点（`[70:170]`）拼接，降低边界效应。
4. 以标签波形为参考，计算：
- 原始 SNR（输入 vs 标签）
- 去噪 SNR（输出 vs 标签）
5. 绘制：
- 全频段 SNR 趋势图
- 低/中/高频随机代表点的全波形与前 400 点局部放大图

### 4.2 `enobtrend.py`

逻辑与 `snrtrend.py` 类似，区别是指标变为 `ENOB`：

- 先计算 SNR
- 再按公式换算：`ENOB = (SNR - 1.76) / 6.02`
- 输出 `ENOB Trend vs Frequency` 及代表频点波形图

---

## 5. 脚本间依赖关系

```text
原始 txt(sub1/sub2)
    -> dataset/create_dataset*.py
        -> dataset/*.pkl
            -> denoise.py 或 denoise_ablation.py
                -> saved_model/*.pth + *.onnx, results/*.txt
                    -> snrtrend.py / enobtrend.py
                        -> figure*/趋势图与波形对比图
```

---

## 6. 推荐运行顺序

在 `snrTrend/` 目录下执行：

```bash
# 1) 生成数据集
python dataset/create_dataset.py

# 2) 训练（基线或消融二选一）
python denoise.py
# 或
python denoise_ablation.py

# 3) 趋势评估
python snrtrend.py
python enobtrend.py
```

---

## 7. 关键约定与注意事项

- `snrtrend.py` / `enobtrend.py` 默认读取 `saved_model/denoise_model_0303.onnx`，训练后若模型名称不同，需要同步修改脚本中的 `model_path`。
- 趋势脚本默认 `WINDOWS_PER_FREQ=18`，需与测试集构造方式一致，否则会触发长度告警并截断。
- 若更换数据源目录（如 `SplitData_80_20` 与 `SplitData-0202_80_20`），需要同时检查：
- `dataset/create_dataset*.py` 中 `base_dir`
- `snrtrend.py` / `enobtrend.py` 中 `sub1_path` 与 `test_pkl`
- 训练脚本依赖 `../boardband/models.py` 的网络定义，迁移目录时请保持相对路径或改为绝对导入。

---

## 8. 结果文件解读（快速）

- `results/loss_*.txt`：训练集 loss（每 epoch）。
- `results/val_loss_*.txt`：验证集 loss（每 epoch）。
- `results/*compare*.png`：不同实验版本的 loss 对比（dB）。
- `figure100Middle*/*snr_trend*.png`：SNR 全频趋势。
- `figure100Middle0202_ENOB/*enob_trend*.png`：ENOB 全频趋势。
- `compare_*MHz*.png`：典型频点波形去噪前后对比（含局部放大）。
