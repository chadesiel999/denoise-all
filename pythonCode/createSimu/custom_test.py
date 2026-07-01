import torch
import numpy as np
import matplotlib.pyplot as plt
import os
import sys

# Ensure we can import modules
sys.path.append(os.getcwd())

# Import Signal Generator and Model
# Attempt to import creComCom. If createSimu is not a package, add it to path
try:
    from createSimu.creComCom import SignalCore, FS, SAMPLE_LEN
except ImportError:
    sys.path.append(os.path.join(os.getcwd(), 'createSimu'))
    from creComCom import SignalCore, FS, SAMPLE_LEN

from train_denoise import GRUCNNModel, DEVICE, SAVE_DIR, MODEL_PATH

def generate_and_test():
    print("Loading Model...")
    # Initialize Model
    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    if os.path.exists(MODEL_PATH):
        model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    else:
        print("Model not found!")
        return
    model.eval()

    gen = SignalCore()
    
    # 2. Define the specific scenarios to test (Generalization Test)
    # 训练集中只包含了 100M-2G 的基础波形。
    # 这里测试 3G 的波形 + 双音信号 + 3:7占空比方波
    test_cases = [
        {
            'name': 'Sine 3GHz', 
            'file_name': 'test_gen_sine_3G',
            'func': gen.gen_sine, 
            'args': {'freq': 3e9}
        },
        {
            'name': 'Square 3.5GHz (Duty 0.1)', 
            'file_name': 'test_gen_square_3.5G_duty0.1',
            'func': gen.gen_square, 
            'args': {'freq': 3.5e9, 'duty': 0.1}
        },
        {
            'name': 'Square 8GHz (Duty 0.5)', 
            'file_name': 'test_gen_square_8G_duty0.5',
            'func': gen.gen_square, 
            'args': {'freq': 8e9, 'duty': 0.5}
        },
        {
            'name': 'Square 10GHz (Duty 0.3)', 
            'file_name': 'test_gen_square_10G_duty0.3',
            'func': gen.gen_square, 
            'args': {'freq': 10e9, 'duty': 0.3}
        },
        {
            'name': 'Triangle 2GHz', 
            'file_name': 'test_gen_triangle_2G',
            'func': gen.gen_triangle, 
            'args': {'freq': 2e9}
        },
        {
            'name': 'Sawtooth 6GHz', 
            'file_name': 'test_gen_sawtooth_6G',
            'func': gen.gen_sawtooth, 
            'args': {'freq': 6e9}
        },
        {
            'name': 'Dual Tone 1.8GHz', 
            'file_name': 'test_gen_dualtone_1.8G',
            'func': gen.gen_dualtone, 
            'args': {'freq1': 1.8e9}
        }
    ]

    # --- Add Digital Modulation Cases ---
    # Digital: Carrier 1G, 3G; Sym Rate 200M-500M (step 100M); Roll-off 0.5
    digi_carriers = [1e9, 3e9]
    digi_sym_rates = np.arange(200e6, 501e6, 100e6)
    digi_mods = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM', '128QAM', '256QAM']
    
    for fc in digi_carriers:
        for sym in digi_sym_rates:
            for mod in digi_mods:
                test_cases.append({
                    'name': f'Digital {mod} fc={fc/1e9}G sym={sym/1e6}M',
                    'file_name': f'test_gen_digital_{mod}_fc{int(fc/1e6)}M_sym{int(sym/1e6)}M',
                    'func': gen.gen_digital,
                    'args': {'carrier_freq': fc, 'symbol_rate': sym, 'mod_type': mod, 'alpha': 0.5}
                })

    # --- Add AM Modulation Cases ---
    # AM: Carrier 1G-5G (step 100M); Mod Freq 20M-400M (step 100M); Depth 100%, 50%
    am_carriers = np.arange(1e9, 5.01e9, 100e6) 
    am_mod_freqs = np.arange(20e6, 401e6, 100e6)
    am_depths = [1.0, 0.5]

    for fc in am_carriers:
        for fm in am_mod_freqs:
            for depth in am_depths:
                test_cases.append({
                    'name': f'AM fc={fc/1e9:.1f}G fm={fm/1e6:.0f}M d={depth}',
                    'file_name': f'test_gen_am_fc{int(fc/1e6)}M_fm{int(fm/1e6)}M_d{depth}',
                    'func': gen.gen_am,
                    'args': {'carrier_freq': fc, 'mod_freq': fm, 'depth': depth}
                })

    print(f"Total Test Cases: {len(test_cases)}")
    print("Running Generalization Tests...")

    mse_records = []
    plot_probability = 0.05  # 对于大量重复的测试用例，只有5%的概率画图

    for i, case in enumerate(test_cases):
        # 1. Generate clean signal (Long)
        clean_long = case['func'](**case['args'])
        
        # 2. Normalize (Crucial Step: Training data was normalized)
        max_val = np.max(np.abs(clean_long))
        if max_val > 0:
            clean_long = clean_long / max_val
            
        # 3. Slice a standard segment (avoid beginning/end)
        # 随机取中间一段，或者固定取一段
        start_idx = 4096 
        if start_idx + SAMPLE_LEN > len(clean_long):
            start_idx = 0
            
        clean_sig = clean_long[start_idx : start_idx + SAMPLE_LEN]
        
        # 4. Add Noise (Using the generator's method)
        # Using a fixed reasonable SNR for visualization clarity, e.g., 25dB
        # We set the range to (25, 25) to get an SNR very close to 25dB
        noisy_sig = gen.add_noise(clean_sig, snr_range=(25, 25))
        
        # 5. Prepare for Model
        # (1, 1, 512)
        inp_tensor = torch.tensor(noisy_sig, dtype=torch.float32).view(1, 1, -1).to(DEVICE)
        
        # 6. Inference
        with torch.no_grad():
            denoised_sig = model(inp_tensor).cpu().numpy().flatten()
            
        # Calculate MSE
        mse = np.mean((denoised_sig - clean_sig)**2)
        mse_noise = np.mean((noisy_sig - clean_sig)**2)
        
        # Calculate Ratio (MSE / InputMSE)
        # Ratio 越接近 1，说明降噪越无效（边界情况）；Ratio 越小说明效果越好
        ratio = mse / mse_noise if mse_noise > 1e-9 else 0.0
        
        # Record result
        mse_records.append(f"{case['name']}: MSE={mse:.4e}, InputMSE={mse_noise:.4e}, Ratio={ratio:.4f}")
        
        # 判断是否为边界情况 (Boundary Case)
        # 定义：当降噪后的MSE仍然保留了超过 60% 的噪声能量时，认为接近边界
        is_boundary = ratio > 0.6 
        
        tag = " [BOUNDARY]" if is_boundary else ""
        print(f"[{i+1}/{len(test_cases)}] {case['name']} | MSE: {mse:.4e} | Ratio: {ratio:.2f}{tag}")

        # 7. Plotting Strategy
        # 策略更改：
        # 1. 基础波形 (前7个) -> 必画
        # 2. 边界情况 (Performance limit) -> 必画，用于观察特征
        # 3. 其他情况 -> 极低概率 (1%) 随机画一张作为参照
        is_basic_case = i < 7
        should_plot = is_basic_case or is_boundary or (np.random.rand() < 0.01)

        if should_plot:
            plt.figure(figsize=(12, 6))
            
            # Plot Noisy
            plt.plot(noisy_sig, color='lightgray', label='Noisy Input', alpha=0.9, linewidth=1.5)
            
            # Plot Clean (Ground Truth)
            plt.plot(clean_sig, color='limegreen', linestyle='--', label='Clean (Ground Truth)', linewidth=2, alpha=0.9)
            
            # Plot Denoised
            plt.plot(denoised_sig, color='blue', label='Denoised Output', linewidth=1.5, alpha=0.8)
            
            # Title with Ratio info
            title_str = f"Generalization Test - {case['name']}\nMSE: {mse:.4e} | InputMSE: {mse_noise:.4e} | Ratio: {ratio:.2f}"
            if is_boundary:
                title_str += " [BOUNDARY]"
                
            plt.title(title_str)
            plt.xlabel("Sample Point")
            plt.ylabel("Amplitude")
            plt.legend(loc='best')
            plt.grid(True, alpha=0.3)
            plt.ylim(-1.5, 1.5) # Limit y-axis for clarity
            
            # 文件名中标注 Boundary 方便查找
            suffix = "_BOUNDARY" if is_boundary else ""
            out_path = os.path.join(SAVE_DIR, f"{case['file_name']}{suffix}.png")
            plt.savefig(out_path)
            plt.close()
            print(f"   -> Plot Saved: {out_path}")

    # Save all MSE results to text file
    results_path = os.path.join(SAVE_DIR, "all_test_results.txt")
    with open(results_path, "w") as f:
        f.write("\n".join(mse_records))
    print(f"\nAll MSE results saved to {results_path}")

if __name__ == "__main__":
    generate_and_test()
