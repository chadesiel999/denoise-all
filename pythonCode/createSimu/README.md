# createSimu 目录说明

本目录用于搭建并验证“合成信号去噪”实验闭环，核心目标是：

1. 构造多类型射频/基带合成信号数据集（含噪与无噪配对）。
2. 训练 GRU-CNN 去噪模型并保存最佳权重。
3. 通过可视化、边界样本搜索、失效扫描分析模型鲁棒性。

---

## 1. 目录框架

```text
createSimu/
├── cre_new_dataset.py           # v2 主数据生成脚本（80GSPS，丰富波形类型）
├── train_denoise_v2.py          # v2 训练脚本（GRUCNN，读取 generated_data_v2）
├── visualize_results_v2.py      # v2 可视化/边界样本搜索/失效扫描
├── train_denoise.py             # v1 训练脚本（读取 generated_data）
├── custom_test.py               # v1 泛化测试脚本（大量调制组合）
├── creComCom.py                 # v1 数据生成脚本（generated_data）
├── createSimulate.py            # 更早期的一体化实验脚本（数据+训练）
├── cre.py / creComplex.py       # 更早期原型脚本（含其他网络尝试）
├── generated_data/              # v1 数据、模型、测试结果
├── results/                     # loss 文本等训练日志
├── saved_model/                 # 旧版模型存档
├── saved_models/                # 一体化脚本保存的 best_model
├── figures*/                    # 可视化结果目录
├── failure_cases/               # 失效样本报告
└── SineTest/                    # v3：纯正弦专用实验分支
    ├── cre_sine_dataset_v3.py
    ├── train_sine_denoise_v3.py
    ├── visualize_results_v3.py
    ├── train_dataset_v3.pth / test_dataset_v3.pth
    ├── figures_basic/ figures_bound/ figures_failure/
    └── best_denoise_model_sine_v3.pth
```

---

## 2. 主流程逻辑

### 2.1 数据生成

`cre_new_dataset.py`（v2）负责生成 `train_dataset_v2.pth` 与 `test_dataset_v2.pth`，流程为：

1. 生成多类干净信号：
- 正弦、方波、三角、锯齿、双音
- AM 调制
- 数字调制（BPSK/QPSK/8PSK/16QAM/64QAM/128QAM/256QAM）
2. 幅值归一化到接近 `[-1, 1]`。
3. 随机截取固定长度片段（默认 `SAMPLE_LEN=512`）。
4. 按给定 SNR 范围加入高斯噪声。
5. 保存为 `(X, Y)`，形状约为 `(N, 512, 1)`。

其中：
- `X` 为带噪输入。
- `Y` 为干净标签。

补充：`creComCom.py` 是 v1 版本，逻辑类似，但数据路径和覆盖参数更偏旧实验配置。

### 2.2 模型训练

`train_denoise_v2.py`（v2）主流程：

1. 读取 `generated_data_v2/train_dataset_v2.pth` 与 `test_dataset_v2.pth`。
2. 维度变换：`(N, L, 1) -> (N, 1, L)`，适配 `Conv1d`。
3. 划分训练/验证（90%/10%）。
4. 使用 `GRUCNNModel` 训练：
- `Conv1d -> BiGRU -> BiGRU -> Conv1d`
- 损失函数：`MSELoss`
- 优化器：`Adam`
5. 保存最佳模型到：
- `generated_data_v2/best_denoise_model_grucnn_v2.pth`
6. 输出训练曲线与样例可视化图。

`train_denoise.py` 是 v1 对应训练脚本，默认读取 `generated_data/train_dataset.pth` 与 `test_dataset.pth`。

### 2.3 评估与诊断

`visualize_results_v2.py` 提供三类分析：

1. `run_visualization()`：
- 固定测试用例可视化（高中低频、多调制类型）。
- 输出时域对比图，统计 `MSE_noisy`、`MSE_denoised`、`ratio`。

2. `run_boundary_search()`：
- 在随机样本中搜索性能临界区间（`ratio` 落在指定范围）。
- 用于定位“效果接近失效边界”的输入。

3. `run_failure_scan()`：
- 系统扫描失效样本（例如 `ratio > threshold`）。
- 输出 `failure_report.txt` 与对应波形图，便于回溯。

说明：
- `ratio = MSE_denoised / MSE_noisy`。
- `ratio < 1` 表示去噪有效，越小越好。
- `ratio > 1` 通常视为去噪失败（比输入更差）。

---

## 3. SineTest 子项目（v3）

`SineTest/` 是专门针对“纯正弦场景”的独立分支：

1. `cre_sine_dataset_v3.py`：只生成正弦信号数据集（覆盖 100MHz 到 20GHz）。
2. `train_sine_denoise_v3.py`：同结构 GRUCNN，仅在正弦数据上训练。
3. `visualize_results_v3.py`：
- 基础用例图（`figures_basic`）
- 边界样本图（`figures_bound`）
- 失效样本与报告（`figures_failure`）
- 同时给出时域+频域联合图（含 SNR 变化）

适用场景：
- 先在单一信号族验证模型上限。
- 对比多类型训练（v2）与单类型训练（v3）的泛化差异。

---

## 4. 数据与模型依赖关系

```text
合成信号脚本 (cre_new_dataset.py / creComCom.py / cre_sine_dataset_v3.py)
    -> *.pth 数据集 (train/test)
        -> 训练脚本 (train_denoise*.py / train_sine_denoise_v3.py)
            -> best_model*.pth + loss曲线
                -> 可视化脚本 (visualize_results_v2.py / visualize_results_v3.py / custom_test.py)
                    -> figures* + failure_report + all_test_results
```

---

## 5. 推荐运行顺序

在工作区根目录 `/home/uestcauto/cyy` 下执行。

### 5.1 v2 主线（推荐）

```bash
# 1) 生成 v2 数据
python createSimu/cre_new_dataset.py

# 2) 训练 v2 模型
python createSimu/train_denoise_v2.py

# 3) 可视化与边界/失效分析
python createSimu/visualize_results_v2.py
```

### 5.2 v1 兼容线

```bash
# 1) 生成 v1 数据
python createSimu/creComCom.py

# 2) 训练 v1 模型
python createSimu/train_denoise.py

# 3) 泛化测试
python createSimu/custom_test.py
```

### 5.3 v3 纯正弦线

```bash
# 1) 生成 v3 数据
python createSimu/SineTest/cre_sine_dataset_v3.py

# 2) 训练 v3 模型
python createSimu/SineTest/train_sine_denoise_v3.py

# 3) 可视化/边界/失效
python createSimu/SineTest/visualize_results_v3.py
```

---

## 6. 关键注意事项

- 路径一致性：
- `train_denoise_v2.py` 默认使用 `./generated_data_v2`。
- 当前目录里常见的是 `generated_data/`，如果你未生成 `generated_data_v2`，训练会报找不到文件。

- 输入维度一致性：
- 数据保存常用 `(N, L, 1)`，训练前需转为 `(N, 1, L)` 供 `Conv1d` 使用。

- 训练结果判读：
- 只看训练损失不足够，建议结合 `ratio`、失效样本与频谱图综合判断。

- 不同分支不要混用模型：
- v1/v2/v3 的数据分布和保存路径不同，评估脚本应加载对应分支模型。

---

## 7. 结果文件快速索引

- `generated_data/all_test_results.txt`：v1 批量测试指标记录。
- `results/training_loss.txt`、`results/val_loss.txt`：训练过程日志。
- `failure_cases/failure_report.txt`：v2 失效样本报告（若执行失效扫描）。
- `figures20G/`、`figures20Gbound/`：v2 可视化与边界样本图。
- `SineTest/figures_basic/`：v3 常规测试图。
- `SineTest/figures_bound/`：v3 边界样本图。
- `SineTest/figures_failure/failure_report.txt`：v3 失效报告。
