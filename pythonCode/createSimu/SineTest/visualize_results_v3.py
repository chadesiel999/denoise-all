import torch
import numpy as np
import matplotlib.pyplot as plt
import os
import sys

# 动态调整路径以支持导入
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(current_dir)
# 将 createSimu 也可以作为包导入
parent_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
if parent_dir not in sys.path:
    sys.path.append(parent_dir)

try:
    # 尝试直接导入
    from cre_sine_dataset_v3 import SignalCore, SAMPLE_LEN, FS
    from train_sine_denoise_v3 import GRUCNNModel, DEVICE
except ImportError:
    # 尝试作为子模块导入
    try:
        from createSimu.SineTest.cre_sine_dataset_v3 import SignalCore, SAMPLE_LEN, FS
        from createSimu.SineTest.train_sine_denoise_v3 import GRUCNNModel, DEVICE
    except ImportError:
        # 最后尝试假设在 createSimu 目录下
        from cre_new_dataset import SignalCore as SignalCoreBase
        # 注意: 如果 v3 文件导入失败，这里不应回退到 v2，因为 SignalCore 定义不同
        print("Import Error: Could not find cre_sine_dataset_v3 or train_sine_denoise_v3")
        raise

# 配置
# 假设脚本位于 createSimu/SineTest/
BASE_DIR = os.path.join(current_dir) 
# 如果是从项目根目录运行，current_dir 已经是绝对路径了

MODEL_PATH = os.path.join(BASE_DIR, 'best_denoise_model_sine_v3.pth')
SAVE_DIR = os.path.join(BASE_DIR, 'figures_basic')
BOUND_SAVE_DIR = os.path.join(BASE_DIR, 'figures_bound')
FAILURE_SAVE_DIR = os.path.join(BASE_DIR, 'figures_failure')

for d in [SAVE_DIR, BOUND_SAVE_DIR, FAILURE_SAVE_DIR]:
    if not os.path.exists(d):
        os.makedirs(d)

def save_with_spectrum(save_path, label, clean, noisy, denoised, fs, mse_n, mse_d, ratio, color_denoised='blue', title_tag="Test Case"):
    """
    保存包含时域图和频域图(SNR)的结果图片
    """
    # 1. Calc SNR
    p_s = np.mean(clean**2)
    if p_s == 0: p_s = 1e-12
    snr_in = 10*np.log10(p_s/mse_n) if mse_n > 0 else 100
    snr_out = 10*np.log10(p_s/mse_d) if mse_d > 0 else 100

    # 2. Spectral
    n = len(clean)
    # FFT
    freqs = np.fft.rfftfreq(n, d=1/fs)
    freqs_ghz = freqs / 1e9
    
    def to_db(x):
        spec = np.abs(np.fft.rfft(x))
        return 20*np.log10(spec + 1e-12)

    clean_db = to_db(clean)
    noisy_db = to_db(noisy)
    denoised_db = to_db(denoised)

    # 3. Plot
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(12, 10))
    
    # Time
    t_ax = np.arange(n)
    ax1.plot(t_ax, noisy, color='gray', alpha=0.5, label='Noisy')
    ax1.plot(t_ax, clean, color='green', linestyle='--', label='Clean', alpha=0.8)
    ax1.plot(t_ax, denoised, color=color_denoised, label='Denoised', alpha=0.9)
    ax1.set_title(f"{title_tag}: {label}\nIn_SNR={snr_in:.2f}dB -> Out_SNR={snr_out:.2f}dB (Ratio={ratio:.4f})")
    ax1.legend(loc='upper right')
    ax1.set_xlabel("Time Index")
    ax1.set_ylabel("Amplitude")
    ax1.grid(True, alpha=0.3)

    # Freq
    ax2.plot(freqs_ghz, noisy_db, color='gray', alpha=0.5, label='Noisy')
    ax2.plot(freqs_ghz, clean_db, color='green', linestyle='--', label='Clean')
    ax2.plot(freqs_ghz, denoised_db, color=color_denoised, label='Denoised')
    ax2.set_title("Frequency Spectrum")
    ax2.set_xlabel("Frequency (GHz)")
    ax2.set_ylabel("Magnitude (dB)")
    ax2.legend(loc='upper right')
    ax2.grid(True, alpha=0.3)

    plt.tight_layout()
    plt.savefig(save_path, dpi=100)
    plt.close()

def get_test_cases():
    """定义一系列具体的测试用例：仅包含正弦信号，覆盖不同频段"""
    cases = []
    
    # helper
    def add(name, func_name, *args, **kwargs):
        cases.append({
            'label': name,
            'func': func_name,
            'args': args,
            'kwargs': kwargs
        })

    # 正弦 (Sine) - 覆盖 100M 到 20G 的多个频点
    freqs_to_test = [
        100e6, 500e6, 
        1e9, 2e9, 5e9, 8e9, 
        10e9, 12e9, 15e9, 18e9, 19.5e9
    ]
    
    for f in freqs_to_test:
        add(f'Sine_{f/1e9:.2f}G', 'gen_sine', f)
    
    return cases

def run_visualization():
    print(f"Loading model from {MODEL_PATH} ...")
    if not os.path.exists(MODEL_PATH):
        print("Error: Model file not found! Please run train_sine_denoise_v3.py first.")
        return

    # 加载模型
    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    model.eval()
    
    # 初始化信号生成器
    gen = SignalCore(fs=FS)
    
    test_cases = get_test_cases()
    
    print(f"Starting visualization for {len(test_cases)} cases...")
    
    with torch.no_grad():
        for i, case in enumerate(test_cases):
            func_name = case['func']
            args = case['args']
            kwargs = case['kwargs']
            label = case['label']
            
            # 1. 生成长信号
            gen_func = getattr(gen, func_name)
            clean_long = gen_func(*args, **kwargs)
            
            # 2. 归一化
            max_val = np.max(np.abs(clean_long))
            if max_val > 0:
                clean_long /= max_val
            
            # 3. 截取适合的片段
            valid_sample = False
            for _ in range(10): 
                if len(clean_long) <= SAMPLE_LEN:
                    break
                start_idx = np.random.randint(0, len(clean_long) - SAMPLE_LEN)
                clean_sample = clean_long[start_idx : start_idx + SAMPLE_LEN]
                if np.max(np.abs(clean_sample)) > 1e-6:
                    valid_sample = True
                    break
            
            if not valid_sample:
                continue
                
            # 4. 加噪声
            noisy_sample = gen.add_noise(clean_sample, snr_range=(20, 25))
            
            # 5. 推理
            inp_tensor = torch.from_numpy(noisy_sample).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
            denoised_tensor = model(inp_tensor)
            denoised_sample = denoised_tensor.squeeze().cpu().numpy()
            
            # 6. 指标
            mse_noisy = np.mean((noisy_sample - clean_sample) ** 2)
            mse_denoised = np.mean((denoised_sample - clean_sample) ** 2)
            ratio = mse_denoised / mse_noisy if mse_noisy > 0 else 0
            
            # 7. 绘图 (Combined Time & Freq)
            save_path = os.path.join(SAVE_DIR, f"{i:02d}_{label}.png")
            save_with_spectrum(save_path, label, clean_sample, noisy_sample, denoised_sample, 
                               FS, mse_noisy, mse_denoised, ratio, 
                               color_denoised='blue', title_tag="Test Case")
            
            print(f"[{i+1}/{len(test_cases)}] {label} -> Saved. Ratio: {ratio:.4f}")

    print(f"\nAll basic figures saved to {SAVE_DIR}")

def get_random_case(gen):
    """随机生成一个正弦测试样本"""
    freq = np.random.uniform(0.1e9, 20e9) # 100M - 20G
    # v3 只支持 gen_sine
    return gen.gen_sine(freq), f"Sine_{freq/1e9:.2f}G"

def run_boundary_search(target_count=10, max_tries=1000, ratio_range=(0.8, 1.25)):
    """搜索 Ratio 在指定范围的样本"""
    print(f"\n>>> Starting Boundary Search (Target: {target_count} images with Ratio in {ratio_range}) ...")
    
    if not os.path.exists(MODEL_PATH):
        return

    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    model.eval()
    
    gen = SignalCore(fs=FS)
    found_count = 0
    
    with torch.no_grad():
        for i in range(max_tries):
            if found_count >= target_count:
                break
                
            clean_long, label = get_random_case(gen)
            if clean_long is None: continue
            
            max_val = np.max(np.abs(clean_long))
            if max_val > 0: clean_long /= max_val
            
            if len(clean_long) <= SAMPLE_LEN: continue
            start_idx = np.random.randint(0, len(clean_long) - SAMPLE_LEN)
            clean_sample = clean_long[start_idx : start_idx + SAMPLE_LEN]
            
            if np.max(np.abs(clean_sample)) < 1e-4: continue

            noisy_sample = gen.add_noise(clean_sample, snr_range=(20, 25))
            
            inp_tensor = torch.from_numpy(noisy_sample).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
            denoised_tensor = model(inp_tensor)
            denoised_sample = denoised_tensor.squeeze().cpu().numpy()
            
            mse_noisy = np.mean((noisy_sample - clean_sample) ** 2)
            mse_denoised = np.mean((denoised_sample - clean_sample) ** 2)
            ratio = mse_denoised / mse_noisy if mse_noisy > 0 else 0
            
            if ratio_range[0] <= ratio <= ratio_range[1]:
                found_count += 1
                print(f"  [Found {found_count}] Ratio={ratio:.4f} | {label}")
                
                save_path = os.path.join(BOUND_SAVE_DIR, f"Bound_{found_count:02d}_{label}_R{ratio:.2f}.png")
                save_with_spectrum(save_path, label, clean_sample, noisy_sample, denoised_sample, 
                                   FS, mse_noisy, mse_denoised, ratio, 
                                   color_denoised='red', title_tag="Bound Case")

    print(f"Boundary search finished. Found {found_count} cases saved to {BOUND_SAVE_DIR}")

def run_failure_scan(samples_per_type=200, threshold=1.0):
    """扫描 Failure Cases (Ratio > 1.0)"""
    LOG_FILE = os.path.join(FAILURE_SAVE_DIR, 'failure_report.txt')
    
    print(f"\n>>> Starting Failure Scan (Target: Ratio > {threshold}) ...")
    
    if not os.path.exists(MODEL_PATH):
        return

    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    model.eval()
    
    gen = SignalCore(fs=FS)

    with open(LOG_FILE, 'w') as f_log:
        f_log.write(f"Timestamp: {os.times()}\n")
        f_log.write(f"Type\tFreq(GHz)\tRatio\tMSE_Noisy\tMSE_Denoised\n")
        f_log.write("-" * 80 + "\n")

    failure_count = 0
    
    # 因为只有 Sine，我们加大采样力度
    print(f"Scanning Sine ({samples_per_type} samples)...")
    
    for _ in range(samples_per_type):
        freq_val = np.random.uniform(0.1e9, 20e9)
        clean_long = gen.gen_sine(freq_val)
        label = f"Sine_{freq_val/1e9:.2f}G"
        
        # Norm & Crop
        max_v = np.max(np.abs(clean_long))
        if max_v > 0: clean_long /= max_v
        
        if len(clean_long) <= SAMPLE_LEN: continue
        start_idx = np.random.randint(0, len(clean_long) - SAMPLE_LEN)
        clean = clean_long[start_idx : start_idx + SAMPLE_LEN]
        
        if np.max(np.abs(clean)) < 1e-4: continue

        # Noise
        noisy = gen.add_noise(clean, snr_range=(20, 25))
        
        # Predict
        inp = torch.from_numpy(noisy).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
        with torch.no_grad():
            out = model(inp).squeeze().cpu().numpy()
            
        mse_n = np.mean((noisy - clean) ** 2)
        mse_d = np.mean((out - clean) ** 2)
        ratio = mse_d / mse_n if mse_n > 0 else 0
        
        if ratio > threshold:
            failure_count += 1
            print(f"  [Failure #{failure_count}] Ratio={ratio:.4f} | {label}")
            
            with open(LOG_FILE, 'a') as f_log:
                f_log.write(f"Sine\t{freq_val/1e9:.4f}\t{ratio:.4f}\t{mse_n:.6f}\t{mse_d:.6f}\n")
            
            save_path = os.path.join(FAILURE_SAVE_DIR, f"Fail_{failure_count:03d}_{label}_R{ratio:.2f}.png")
            save_with_spectrum(save_path, label, clean, noisy, out, 
                               FS, mse_n, mse_d, ratio, 
                               color_denoised='red', title_tag="FAILURE CASE")

    print(f"\nFailure scan complete. Found {failure_count} cases. Check {LOG_FILE}")

if __name__ == "__main__":
    run_visualization()
    run_boundary_search(target_count=10, max_tries=500, ratio_range=(0.8, 1.25))
    run_failure_scan(samples_per_type=200, threshold=1.0)
