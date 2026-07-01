import numpy as np
import os
import re

# Configuration
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"
READ_LINES = 10000

def safe_float_convert(x):
    try:
        return float(x)
    except ValueError:
        return None

def read_data(file_path, num_values):
    data = []
    if not os.path.exists(file_path):
        return []
    try:
        with open(file_path, 'r') as file:
            for i in range(num_values + 10): # Read a bit more to be safe
                line = file.readline()
                if not line:
                    break
                value = safe_float_convert(line.strip())
                if value is not None:
                    data.append(value)
                if len(data) >= num_values:
                    break
    except Exception as e:
        print(f"Error reading {file_path}: {e}")
        return []
    return data

def align_wave(wave, label_wave):
    # Normalize for correlation calculation to avoid scale issues
    w_norm = (wave - np.mean(wave)) / (np.std(wave) + 1e-10)
    l_norm = (label_wave - np.mean(label_wave)) / (np.std(label_wave) + 1e-10)
    
    cross_correlation = np.correlate(w_norm, l_norm, mode='full')
    delay = np.argmax(cross_correlation) - len(wave) + 1
    
    max_corr = np.max(cross_correlation) / len(wave)
    print(f"  [Align Debug] Detected delay: {delay}, Max Norm Corr: {max_corr:.4f}")

    if delay >= 0:
        aligned_wave = wave[delay:]
        aligned_label = label_wave[:len(aligned_wave)]
    else:
        aligned_label = label_wave[-delay:]
        aligned_wave = wave[:len(aligned_label)]
    
    min_l = min(len(aligned_wave), len(aligned_label))
    return aligned_wave[:min_l], aligned_label[:min_l]

def main():
    if not os.path.exists(DATA_DIR):
        print(f"Directory not found: {DATA_DIR}")
        return

    all_files = [f for f in os.listdir(DATA_DIR) if 'Sub1' in f and f.endswith('.txt')]
    all_files.sort()
    
    if not all_files:
        print("No Sub1 files found.")
        return

    # Pick a file from the middle to avoid edge cases
    target_file = all_files[len(all_files)//2] if len(all_files) > 1 else all_files[0]
    
    if target_file.startswith('C1_Sub1_'):
        refer_file = 'C1_Sub2_' + target_file[8:]
    else:
        refer_file = target_file.replace('Sub1', 'Sub2')

    file_path = os.path.join(DATA_DIR, target_file)
    refer_path = os.path.join(DATA_DIR, refer_file)

    print(f"Analyzing Pair:\n  Input: {target_file}\n  Label: {refer_file}")
    
    if not os.path.exists(refer_path):
        print("  Label file NOT FOUND!")
        return

    # 1. Read Raw
    wave = np.array(read_data(file_path, READ_LINES))
    refer_wave = np.array(read_data(refer_path, READ_LINES))
    
    print(f"  Lengths -> Wave: {len(wave)}, LABEL: {len(refer_wave)}")
    print(f"  First 5 Wave: {wave[:5]}")
    print(f"  First 5 Label: {refer_wave[:5]}")
    
    if len(wave) == 0 or len(refer_wave) == 0:
        print("  Error: Data is empty.")
        return

    # 2. Pre-alignment Correlation
    min_len = min(len(wave), len(refer_wave))
    corr_raw = np.corrcoef(wave[:min_len], refer_wave[:min_len])[0, 1]
    print(f"  Raw Correlation (First {min_len} pts): {corr_raw:.4f}")

    # 3. Align
    aw, al = align_wave(wave, refer_wave)
    
    if len(aw) == 0:
        print("  Alignment resulted in empty arrays!")
        return

    # 4. Post-alignment Correlation
    corr_aligned = np.corrcoef(aw, al)[0, 1]
    print(f"  Aligned Correlation ({len(aw)} pts): {corr_aligned:.4f}")
    
    # Check if we are just aligning noise
    if corr_aligned < 0.1:
        print("\n  !!! WARNING: Correlation is still extremely low after alignment. !!!")
        print("  Possible reasons:")
        print("  1. Sub1 and Sub2 file content does not match (wrong pair).")
        print("  2. Data is purely noise.")
        print("  3. Frequency is so high that sampling rate creates aliasing/phase issues.")

if __name__ == "__main__":
    main()