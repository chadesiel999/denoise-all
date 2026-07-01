import numpy as np
import os
import pickle
import re
import scipy.signal

# ================= Configuration =================
# 数据只读 Sub1
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"
SAVE_DIR = "/home/uestcauto/cyy/boardband/dataset"

# Parameters
SLICE_LEN = 240
READ_LINES = 10000 

# Split config
TEST_SLICES_PER_FREQ = 3   
VAL_RATIO = 0.05           

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
            for _ in range(num_values + 100): 
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

def normalize(data):
    d_max = np.max(data)
    d_min = np.min(data)
    if d_max == d_min:
        return np.zeros_like(data)
    return 2 * (data - d_min) / (d_max - d_min) - 1

def generate_clean_label_fft(noisy_signal):
    """
    Uses FFT to find the dominant frequency and reconstructs a clean signal.
    """
    n = len(noisy_signal)
    
    # 1. FFT
    fft_spectrum = np.fft.rfft(noisy_signal)
    freqs = np.fft.rfftfreq(n)
    
    # 2. Find Peak (Dominant Frequency)
    # Ignore DC component (index 0)
    mag = np.abs(fft_spectrum)
    mag[0] = 0 
    peak_idx = np.argmax(mag)
    
    # 3. Filter: Keep only the peak and immediate neighbors
    # This acts as an extremely narrow bandpass filter
    mask = np.zeros_like(fft_spectrum, dtype=bool)
    
    # Keep peak and +/- k neighbors
    k = 1 # Keep +/- 1 bin width
    start = max(0, peak_idx - k)
    end = min(len(fft_spectrum), peak_idx + k + 1)
    mask[start:end] = True
    
    clean_fft = np.zeros_like(fft_spectrum)
    clean_fft[mask] = fft_spectrum[mask]
    
    # 4. IFFT to get clean time-domain signal
    clean_signal = np.fft.irfft(clean_fft, n=n)
    
    return normalize(clean_signal)

def get_freq_from_name(filename):
    match = re.search(r'Sub1_(\d+)MHz', filename)
    if match:
        return int(match.group(1))
    return -1

def process_and_split():
    if not os.path.exists(SAVE_DIR):
        os.makedirs(SAVE_DIR)
        
    print(f"Loading data from: {DATA_DIR}")
    if not os.path.exists(DATA_DIR):
        print("Data directory not found!")
        return

    all_files = [f for f in os.listdir(DATA_DIR) if 'Sub1' in f and f.endswith('.txt')]
    # Sort files
    files = sorted(all_files, key=get_freq_from_name)
    total_files = len(files)
    
    train_val_pool = []
    test_chunks = []
    
    # Debug print first file processing
    first_run = True

    for i, file in enumerate(files):
        # Read
        file_path = os.path.join(DATA_DIR, file)
        wave = np.array(read_data(file_path, READ_LINES))
        if len(wave) < SLICE_LEN:
            continue
            
        # Normalize Input
        wave = normalize(wave)
        
        # --- KEY CHANGE: Generate Label via FFT Filtering ---
        # We do this for the WHOLE wave first to have better frequency resolution
        label_wave = generate_clean_label_fft(wave)
        
        # Debug Correlation
        if first_run:
            corr = np.corrcoef(wave, label_wave)[0,1]
            print(f"-- Debug First File: {file} --")
            print(f"   Shape: {wave.shape}")
            print(f"   FFT-Recon Correlation: {corr:.4f}")
            first_run = False

        # Slice
        num_chunks = len(wave) // SLICE_LEN
        if num_chunks == 0:
            continue
            
        wave = wave[:num_chunks * SLICE_LEN]
        label_wave = label_wave[:num_chunks * SLICE_LEN]
        
        w_slices = wave.reshape(-1, SLICE_LEN)
        l_slices = label_wave.reshape(-1, SLICE_LEN)
        
        combined = np.stack([w_slices, l_slices], axis=1)
        
        # Split (Test = Last 3)
        test_count = min(num_chunks, TEST_SLICES_PER_FREQ)
        if test_count > 0:
            test_chunks.append(combined[-test_count:])
            if num_chunks > test_count:
                train_val_pool.append(combined[:-test_count])
        
        if (i + 1) % 100 == 0:
            print(f"Processed {i + 1}/{total_files}...")

    # Save
    if train_val_pool:
        all_tv = np.concatenate(train_val_pool, axis=0)
        # Shuffle Train/Val
        np.random.shuffle(all_tv)
        val_size = int(len(all_tv) * VAL_RATIO)
        val_data = all_tv[:val_size]
        train_data = all_tv[val_size:]
    else:
        train_data = np.array([])
        val_data = np.array([])
        
    test_data = np.concatenate(test_chunks, axis=0) if test_chunks else np.array([])

    def save(d, n): 
        with open(os.path.join(SAVE_DIR, f"broadband_{n}.pkl"), "wb") as f:
            pickle.dump(d[..., np.newaxis], f, -1)
        print(f"Saved {n}: {d.shape}")

    save(train_data, "train_synth_fft")
    save(val_data, "val_synth_fft")
    save(test_data, "test_synth_fft")

if __name__ == "__main__":
    np.random.seed(42)
    process_and_split()