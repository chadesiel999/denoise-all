import torch
import numpy as np
import matplotlib.pyplot as plt
import os
import sys

# 假设当前工作目录是 /home/uestcauto/cyy
# 动态调整路径以支持导入
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(current_dir)

try:
    # 尝试直接导入 (当脚本直接运行时)
    from cre_new_dataset import SignalCore, SAMPLE_LEN, FS
    from train_denoise_v2 import GRUCNNModel, DEVICE
except ImportError:
    # 尝试包导入 (当作为模块运行时)
    from createSimu.cre_new_dataset import SignalCore, SAMPLE_LEN, FS
    from createSimu.train_denoise_v2 import GRUCNNModel, DEVICE

# 配置
MODEL_PATH = './generated_data_v2/best_denoise_model_grucnn_v2.pth'
SAVE_DIR = './createSimu/figures20G'
BOUND_SAVE_DIR = './createSimu/figures20Gbound'

if not os.path.exists(SAVE_DIR):
    os.makedirs(SAVE_DIR)
if not os.path.exists(BOUND_SAVE_DIR):
    os.makedirs(BOUND_SAVE_DIR)

def get_test_cases():
    """定义一系列具体的测试用例：信号类型和参数，覆盖高中低频"""
    cases = []
    
    # helper
    def add(name, func_name, *args, **kwargs):
        cases.append({
            'label': name,
            'func': func_name,
            'args': args,
            'kwargs': kwargs
        })

    # 1. 正弦 (Sine)
    add('Sine_Low_100M', 'gen_sine', 100e6)
    add('Sine_Mid_8G', 'gen_sine', 8e9)
    add('Sine_High_19G', 'gen_sine', 19e9)
    
    # 2. 方波 (Square)
    add('Square_Low_105M_Duty0.5', 'gen_square', 105e6, 0.5)
    add('Square_Mid_5G_Duty0.1', 'gen_square', 5e9, 0.1)
    add('Square_High_18G_Duty0.3', 'gen_square', 18e9, 0.3)
    
    # 3. 三角 (Triangle)
    add('Triangle_Low_200M', 'gen_triangle', 200e6)
    add('Triangle_Mid_8G', 'gen_triangle', 8e9)
    add('Triangle_High_18G', 'gen_triangle', 18e9)

    # 4. 锯齿 (Sawtooth)
    add('Sawtooth_Low_200M', 'gen_sawtooth', 200e6)
    add('Sawtooth_Mid_8G', 'gen_sawtooth', 8e9)
    add('Sawtooth_High_18G', 'gen_sawtooth', 18e9)
    
    # 5. 双音 (DualTone) - freq2 = 1.5 * freq1
    add('DualTone_Low_200M', 'gen_dualtone', 200e6)
    add('DualTone_Mid_8G', 'gen_dualtone', 8e9)
    add('DualTone_High_18G', 'gen_dualtone', 18e9)
    
    # 6. AM
    add('AM_Low_Carrier1G', 'gen_am', 1e9, 20e6, 1.0)
    add('AM_Mid_Carrier8G', 'gen_am', 8e9, 200e6, 0.5)
    add('AM_High_Carrier18G', 'gen_am', 18e9, 400e6, 0.5)
    
    # 7. Digital
    add('Digital_Low_Carrier2G_BPSK', 'gen_digital', 2e9, 200e6, 'BPSK', alpha=0.5)
    add('Digital_Mid_Carrier8G_16QAM', 'gen_digital', 8e9, 500e6, '16QAM', alpha=0.5)
    add('Digital_High_Carrier14G_256QAM', 'gen_digital', 14e9, 500e6, '256QAM', alpha=0.5)
    
    return cases

def run_visualization():
    print(f"Loading model from {MODEL_PATH} ...")
    if not os.path.exists(MODEL_PATH):
        print("Error: Model file not found!")
        return

    # 加载模型
    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    model.eval()
    
    # 初始化信号生成器
    # 注意：FS 等全局变量在 cre_new_dataset 导入时已定义
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
            
            # 3. 截取适合的片段 (确保非空且有信号)
            # 尝试随机取其中的一段，长度为 SAMPLE_LEN
            valid_sample = False
            for _ in range(10): # 尝试10次找到非全0段
                if len(clean_long) <= SAMPLE_LEN:
                    break
                start_idx = np.random.randint(0, len(clean_long) - SAMPLE_LEN)
                clean_sample = clean_long[start_idx : start_idx + SAMPLE_LEN]
                if np.max(np.abs(clean_sample)) > 1e-6:
                    valid_sample = True
                    break
            
            if not valid_sample:
                print(f"Skipping {label}: Signal too short or empty")
                continue
                
            # 4. 加噪声
            noisy_sample = gen.add_noise(clean_sample, snr_range=(20, 25)) # 稍微大一点噪声范围以便观察
            
            # 5. 准备模型输入
            # (Length) -> (1, 1, Length)
            inp_tensor = torch.from_numpy(noisy_sample).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
            
            # 6. 推理
            denoised_tensor = model(inp_tensor)
            denoised_sample = denoised_tensor.squeeze().cpu().numpy()
            
            # 7. 计算指标
            # MSE
            mse_noisy = np.mean((noisy_sample - clean_sample) ** 2)
            mse_denoised = np.mean((denoised_sample - clean_sample) ** 2)
            
            # Ratio (降噪后 MSE / 原始带噪 MSE) -> 值越小，效果越好 (例如 0.1 代表误差降为原来的 10%)
            ratio = mse_denoised / mse_noisy if mse_noisy > 0 else 0
            
            # dB 提升 (Optional, but nice to haee)
            snr_gain = 10 * np.log10(mse_noisy / mse_denoised) if mse_denoised > 0 else 0

            # 8. 绘图
            plt.figure(figsize=(12, 6))
            
            t_axis = np.arange(SAMPLE_LEN)
            
            # Plot Noisy (在底层，灰色)
            plt.plot(t_axis, noisy_sample, color='gray', alpha=0.5, label='Noisy Input', linewidth=1)
            
            # Plot Clean (理想，绿色虚线)
            plt.plot(t_axis, clean_sample, color='green', linestyle='--', label='Clean Ideal', linewidth=1.5, alpha=0.8)
            
            # Plot Denoised (结果，红色/蓝色)
            plt.plot(t_axis, denoised_sample, color='blue', label='Denoised Output', linewidth=1.2, alpha=0.9)
            
            plt.title(f"Test Case: {label}\n"
                      f"MSE_Noisy={mse_noisy:.6f} | MSE_Denoised={mse_denoised:.6f} | Ratio (Den/Noisy)={ratio:.4f}", fontsize=12)
            plt.legend(loc='upper right')
            plt.grid(True, alpha=0.3)
            plt.xlabel("Sample Index")
            plt.ylabel("Amplitude")
            
            # 保存
            save_path = os.path.join(SAVE_DIR, f"{i:02d}_{label}.png")
            plt.savefig(save_path, dpi=150)
            plt.close()
            
            print(f"[{i+1}/{len(test_cases)}] {label} -> Saved. Ratio: {ratio:.4f}")

    print(f"\nAll figures saved to {SAVE_DIR}")

def get_random_case(gen):
    """随机生成一个测试样本及其标签"""
    type_idx = np.random.randint(0, 7)
    freq = np.random.uniform(0.1e9, 20e9) # 100M - 20G
    
    if type_idx == 0: # Sine
        return gen.gen_sine(freq), f"Sine_{freq/1e9:.2f}G"
    
    elif type_idx == 1: # Square
        duty = np.random.choice([0.1, 0.3, 0.5, 0.8])
        return gen.gen_square(freq, duty), f"Square_{freq/1e9:.2f}G_Duty{duty}"
        
    elif type_idx == 2: # Triangle
        return gen.gen_triangle(freq), f"Triangle_{freq/1e9:.2f}G"
        
    elif type_idx == 3: # Sawtooth
        return gen.gen_sawtooth(freq), f"Sawtooth_{freq/1e9:.2f}G"
        
    elif type_idx == 4: # DualTone
        return gen.gen_dualtone(freq), f"DualTone_{freq/1e9:.2f}G"
        
    elif type_idx == 5: # AM
        mod_freq = np.random.choice([20e6, 100e6, 400e6])
        depth = np.random.choice([0.5, 1.0])
        return gen.gen_am(freq, mod_freq, depth), f"AM_Car{freq/1e9:.2f}G_Mod{mod_freq/1e6:.0f}M"
        
    elif type_idx == 6: # Digital
        # carrier 2G - 15G
        c_freq = np.random.uniform(2e9, 15e9)
        sym_rate = np.random.choice([200e6, 500e6])
        mod = np.random.choice(['BPSK', 'QPSK', '16QAM', '64QAM', '256QAM'])
        return gen.gen_digital(c_freq, sym_rate, mod, alpha=0.5), f"Dig_{mod}_Car{c_freq/1e9:.2f}G"
    
    return None, None

def run_boundary_search(target_count=10, max_tries=1000, ratio_range=(0.8, 1.25)):
    """
    搜索 Ratio 在 ratio_range 范围内的样本并保存到 figures20Gbound
    """
    print(f"\n>>> Starting Boundary Search (Target: {target_count} images with Ratio in {ratio_range}) ...")
    
    if not os.path.exists(MODEL_PATH):
        print("Error: Model file not found!")
        return

    # 加载模型
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
            
            # 归一化 & 截取
            max_val = np.max(np.abs(clean_long))
            if max_val > 0: clean_long /= max_val
            
            # 随机截取
            if len(clean_long) <= SAMPLE_LEN: continue
            start_idx = np.random.randint(0, len(clean_long) - SAMPLE_LEN)
            clean_sample = clean_long[start_idx : start_idx + SAMPLE_LEN]
            
            # 忽略太小的信号
            if np.max(np.abs(clean_sample)) < 1e-4: continue

            # 加噪
            noisy_sample = gen.add_noise(clean_sample, snr_range=(20, 25))
            
            # 推理
            inp_tensor = torch.from_numpy(noisy_sample).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
            denoised_tensor = model(inp_tensor)
            denoised_sample = denoised_tensor.squeeze().cpu().numpy()
            
            # 指标
            mse_noisy = np.mean((noisy_sample - clean_sample) ** 2)
            mse_denoised = np.mean((denoised_sample - clean_sample) ** 2)
            
            ratio = mse_denoised / mse_noisy if mse_noisy > 0 else 0
            
            # 判断是否在目标区间
            if ratio_range[0] <= ratio <= ratio_range[1]:
                found_count += 1
                print(f"  [Found {found_count}] Ratio={ratio:.4f} | {label}")
                
                # 绘图保存
                plt.figure(figsize=(12, 6))
                t_axis = np.arange(SAMPLE_LEN)
                plt.plot(t_axis, noisy_sample, color='gray', alpha=0.5, label='Noisy', linewidth=1)
                plt.plot(t_axis, clean_sample, color='green', linestyle='--', label='Clean', linewidth=1.5, alpha=0.8)
                plt.plot(t_axis, denoised_sample, color='red', label='Denoised', linewidth=1.2, alpha=0.9) # 用红色突出
                
                plt.title(f"Bound Case: {label}\nRatio={ratio:.4f} (MSE_Den={mse_denoised:.6f} / MSE_Noisy={mse_noisy:.6f})", fontsize=12)
                plt.legend()
                plt.grid(True, alpha=0.3)
                
                save_path = os.path.join(BOUND_SAVE_DIR, f"Bound_{found_count:02d}_{label}_R{ratio:.2f}.png")
                plt.savefig(save_path, dpi=100)
                plt.close()

    print(f"Boundary search finished. Found {found_count} cases saved to {BOUND_SAVE_DIR}")

def run_failure_scan(samples_per_type=50, threshold=1.0):
    """
    扫描所有信号类型，专门寻找 Ratio > threshold (默认为1.0) 的失效案例。
    记录详细参数并保存日志和图片。
    """
    FAILURE_DIR = './createSimu/failure_cases'
    if not os.path.exists(FAILURE_DIR):
        os.makedirs(FAILURE_DIR)
        
    LOG_FILE = os.path.join(FAILURE_DIR, 'failure_report.txt')
    
    print(f"\n>>> Starting Failure Scan (Target: Ratio > {threshold}) ...")
    print(f"    Scanning {samples_per_type} samples per signal type. Saving to {FAILURE_DIR}")

    if not os.path.exists(MODEL_PATH):
        print("Error: Model file not found!")
        return

    # Load Model
    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    model.eval()
    
    gen = SignalCore(fs=FS)

    # Header for log
    with open(LOG_FILE, 'w') as f_log:
        f_log.write(f"Timestamp: {os.times()}\n")
        f_log.write(f"Type\tFreq(GHz)\tParams\tRatio\tMSE_Noisy\tMSE_Denoised\n")
        f_log.write("-" * 80 + "\n")

    failure_count = 0
    
    # Define generators for systematic scanning
    # Generators return: (signal, label_str, freq_val, param_str)
    
    def try_case(signal_data, label, freq_val, param_str):
        nonlocal failure_count
        # Normalize
        max_v = np.max(np.abs(signal_data))
        if max_v > 0: signal_data /= max_v
        
        # Crop
        if len(signal_data) <= SAMPLE_LEN: return
        start_idx = np.random.randint(0, len(signal_data) - SAMPLE_LEN)
        clean = signal_data[start_idx : start_idx + SAMPLE_LEN]
        
        if np.max(np.abs(clean)) < 1e-4: return

        # Add Noise
        noisy = gen.add_noise(clean, snr_range=(20, 25))
        
        # Predict
        inp = torch.from_numpy(noisy).float().unsqueeze(0).unsqueeze(0).to(DEVICE)
        with torch.no_grad():
            out = model(inp).squeeze().cpu().numpy()
            
        # Calc
        mse_n = np.mean((noisy - clean) ** 2)
        mse_d = np.mean((out - clean) ** 2)
        ratio = mse_d / mse_n if mse_n > 0 else 0
        
        if ratio > threshold:
            failure_count += 1
            print(f"  [Failure #{failure_count}] Ratio={ratio:.4f} | {label}")
            
            # Log to file
            with open(LOG_FILE, 'a') as f_log:
                f_log.write(f"{label.split('_')[0]}\t{freq_val/1e9:.4f}\t{param_str}\t{ratio:.4f}\t{mse_n:.6f}\t{mse_d:.6f}\n")
            
            # Plot
            plt.figure(figsize=(10, 5))
            t = np.arange(SAMPLE_LEN)
            plt.plot(t, noisy, color='gray', alpha=0.5, label='Noisy')
            plt.plot(t, clean, color='green', linestyle='--', label='Clean')
            plt.plot(t, out, color='red', label='Denoised')
            plt.title(f"FAILURE CASE: {label}\nRatio={ratio:.4f} > {threshold}", color='red')
            plt.legend()
            plt.savefig(os.path.join(FAILURE_DIR, f"Fail_{failure_count:03d}_{label}_R{ratio:.2f}.png"))
            plt.close()

    # --- Scanning Loops ---
    
    # 1. Sine
    print("Scanning Sine...")
    for _ in range(samples_per_type):
        f = np.random.uniform(0.1e9, 20e9)
        try_case(gen.gen_sine(f), f"Sine_{f/1e9:.2f}G", f, "None")

    # 2. Square
    print("Scanning Square...")
    for _ in range(samples_per_type):
        f = np.random.uniform(0.1e9, 20e9)
        d = np.random.choice([0.1, 0.3, 0.5, 0.8])
        try_case(gen.gen_square(f, d), f"Square_{f/1e9:.2f}G_D{d}", f, f"Duty={d}")

    # 3. Triangle
    print("Scanning Triangle...")
    for _ in range(samples_per_type):
        f = np.random.uniform(0.1e9, 20e9)
        try_case(gen.gen_triangle(f), f"Triangle_{f/1e9:.2f}G", f, "None")

    # 4. Sawtooth
    print("Scanning Sawtooth...")
    for _ in range(samples_per_type):
        f = np.random.uniform(0.1e9, 20e9)
        try_case(gen.gen_sawtooth(f), f"Sawtooth_{f/1e9:.2f}G", f, "None")

    # 5. DualTone
    print("Scanning DualTone...")
    for _ in range(samples_per_type):
        f = np.random.uniform(0.1e9, 20e9) # Base freq
        try_case(gen.gen_dualtone(f), f"DualTone_{f/1e9:.2f}G", f, "f2=1.5f1")

    # 6. AM
    print("Scanning AM...")
    for _ in range(samples_per_type):
        fc = np.random.uniform(0.1e9, 20e9)
        fm = np.random.choice([20e6, 100e6, 400e6])
        depth = np.random.choice([0.5, 1.0])
        try_case(gen.gen_am(fc, fm, depth), f"AM_C{fc/1e9:.2f}G_M{fm/1e6:.0f}M", fc, f"Mod={fm/1e6}M,Depth={depth}")

    # 7. Digital
    print("Scanning Digital...")
    for _ in range(samples_per_type):
        fc = np.random.uniform(2e9, 15e9)
        sr = np.random.choice([200e6, 500e6])
        mod = np.random.choice(['BPSK', 'QPSK', '16QAM', '64QAM', '256QAM'])
        try_case(gen.gen_digital(fc, sr, mod, 0.5), f"Dig_{mod}_C{fc/1e9:.2f}G", fc, f"Sym={sr/1e6}M,Mod={mod}")

    print(f"\nFailure scan complete. Found {failure_count} cases. Check {FAILURE_DIR}/failure_report.txt")

if __name__ == "__main__":
    # run_visualization()
    # run_boundary_search(target_count=15, max_tries=2000, ratio_range=(0.8, 1.25))
    run_failure_scan(samples_per_type=100, threshold=1.0)
