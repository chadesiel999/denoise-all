import numpy as np
import os
import pickle
import re
import random

# ================= Configuration =================
# 数据只读 Sub1，忽略 Sub2
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"
SAVE_DIR = "/home/uestcauto/cyy/boardband/dataset"

# Parameters
SLICE_LEN = 240
READ_LINES = 10000 
FS = 80e9  # 80 GSa/s

# Split config
TEST_SLICES_PER_FREQ = 3   
VAL_RATIO = 0.05           

# ================= Helper Functions =================
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
            for _ in range(num_values + 100): # Read a bit extra buffer
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
    # Norm to [-1, 1]
    return 2 * (data - d_min) / (d_max - d_min) - 1

def synthesize_ideal_wave(noisy_wave, freq_hz, fs):
    """
    Fits a pure sine wave to the noisy input using Least Squares.
    Model: y = C + A*cos(wt) + B*sin(wt)
    Returns: Constructed pure sine wave normalized to [-1, 1]
    """
    n = len(noisy_wave)
    t = np.arange(n) / fs
    omega = 2 * np.pi * freq_hz
    
    # Design Matrix: [1, cos(wt), sin(wt)]
    # Use float64 for precision during fitting
    cos_term = np.cos(omega * t)
    sin_term = np.sin(omega * t)
    
    # Stack columns
    A_mat = np.column_stack([np.ones(n), cos_term, sin_term])
    
    # Least Squares Solution: Minimize ||Ax - y||
    # coeffs = [bias, a, b]
    coeffs, residuals, rank, s = np.linalg.lstsq(A_mat, noisy_wave, rcond=None)
    
    # Reconstruct ideal wave
    # ideal = bias + a*cos + b*sin
    ideal_wave = np.dot(A_mat, coeffs)
    
    # Normalize ideal wave strictly to [-1, 1] for label
    # This removes amplitude fluctuations and bias, giving a perfect target
    return normalize(ideal_wave)

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

    # 1. Get List (Only Sub1 needed)
    all_files = [f for f in os.listdir(DATA_DIR) if 'Sub1' in f and f.endswith('.txt')]
    # Sort by numerical frequency
    files = sorted(all_files, key=get_freq_from_name)
    
    total_files = len(files)
    print(f"Found {total_files} files.")

    train_val_pool = []
    test_chunks = []
    
    meta_info = {'files': [], 'frequencies': []}

    for i, file in enumerate(files):
        freq_mhz = get_freq_from_name(file)
        if freq_mhz <= 0:
            continue
            
        file_path = os.path.join(DATA_DIR, file)
        
        # 1. Read Raw Input
        wave = np.array(read_data(file_path, READ_LINES))
        if len(wave) < SLICE_LEN:
            continue
            
        # 2. Normalize Input first (important for fitting stability)
        wave = normalize(wave)
        
        # 3. Create Synthetic Label
        freq_hz = freq_mhz * 1e6
        label_wave = synthesize_ideal_wave(wave, freq_hz, FS)
        
        # 4. Slice
        # Discard last incomplete chunk
        num_chunks = len(wave) // SLICE_LEN
        if num_chunks == 0:
            continue
            
        wave = wave[:num_chunks * SLICE_LEN]
        label_wave = label_wave[:num_chunks * SLICE_LEN]
        
        w_slices = wave.reshape(-1, SLICE_LEN)
        l_slices = label_wave.reshape(-1, SLICE_LEN)
        
        # Stack: (NumChunks, 2, SLICE_LEN) -> [Input, Label]
        combined = np.stack([w_slices, l_slices], axis=1)
        
        # 5. Split Logic
        test_count = min(num_chunks, TEST_SLICES_PER_FREQ)
        
        if test_count > 0:
            test_chunks.append(combined[-test_count:])
            
            if num_chunks > test_count:
                train_val_pool.append(combined[:-test_count])
        
        meta_info['files'].append(file)
        meta_info['frequencies'].append(freq_mhz)

        if (i + 1) % 100 == 0:
            print(f"Processed {i + 1}/{total_files} | Last: {freq_mhz} MHz")

    # --- Finalize Datasets ---
    print("Shuffling and splitting Train/Val...")
    
    if train_val_pool:
        all_train_val = np.concatenate(train_val_pool, axis=0)
        
        # Shuffle
        indices = np.arange(all_train_val.shape[0])
        np.random.shuffle(indices)
        all_train_val = all_train_val[indices]
        
        # Split
        val_size = int(len(all_train_val) * VAL_RATIO)
        val_data = all_train_val[:val_size]
        train_data = all_train_val[val_size:]
    else:
        train_data = np.array([])
        val_data = np.array([])

    if test_chunks:
        test_data = np.concatenate(test_chunks, axis=0)
    else:
        test_data = np.array([])

    # Helper to save
    def save_dataset(data, name):
        if data.size == 0:
            print(f"Skipping empty {name}")
            return
        # Add channel dim: (N, 2, 240) -> (N, 2, 240, 1) to match Model input
        data_to_save = data[..., np.newaxis]
        path = os.path.join(SAVE_DIR, f"broadband_{name}.pkl")
        with open(path, "wb") as f:
            pickle.dump(data_to_save, f, -1)
        print(f"Saved {name}: {data_to_save.shape}")

    # Use NEW names so we don't mix up with the bad data
    save_dataset(train_data, "train_synth")
    save_dataset(val_data, "val_synth")
    save_dataset(test_data, "test_synth")
    
    print("Done.")

if __name__ == "__main__":
    np.random.seed(42)
    process_and_split()
