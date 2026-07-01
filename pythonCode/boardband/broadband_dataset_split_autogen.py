import numpy as np
import os
import pickle
import re

# Configuration
# DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"

SAVE_DIR = "/home/uestcauto/cyy/boardband/dataset"

# Slicing parameters
SLICE_LEN = 240
SAMPLING_RATE = 80e9  # 80 GSa/s

# Test configuration
TEST_SLICES_PER_FREQ = 3
VAL_RATIO = 0.05
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
            for _ in range(num_values + 50): # Read a bit buffer
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
    return data[:num_values]

def normalize(data, data_max, data_min):
    if data_max == data_min:
        return np.zeros_like(data)
    return 2 * (data - data_min) / (data_max - data_min) - 1

def get_freq_from_name(filename):
    match = re.search(r'Sub1_(\d+)MHz', filename)
    if match:
        return int(match.group(1))
    return -1

def generate_ideal_wave(raw_wave, freq_mhz, fs=SAMPLING_RATE):
    """
    Uses Least Squares to fit: y = C + A*cos(wt) + B*sin(wt)
    Returns the reconstructed ideal wave.
    """
    n = len(raw_wave)
    t = np.arange(n) / fs
    omega = 2 * np.pi * (freq_mhz * 1e6)
    
    # Design Matrix: [1, cos(wt), sin(wt)]
    # Note: For high frequency, omega*t can be large, leading to precision loss in float32.
    # float64 (default in numpy) usually handles this capable up to many periods.
    cos_wt = np.cos(omega * t)
    sin_wt = np.sin(omega * t)
    
    A_matrix = np.vstack([np.ones(n), cos_wt, sin_wt]).T
    
    # Solve A * [C, A, B]^T = raw_wave
    # lstsq returns: coeffs, residuals, rank, s
    coeffs, residuals, _, _ = np.linalg.lstsq(A_matrix, raw_wave, rcond=None)
    
    C_term, A_term, B_term = coeffs
    
    # Reconstruct
    ideal_wave = C_term + A_term * cos_wt + B_term * sin_wt
    
    # Calculate quality of fit (R-squared or similar implication)
    # If residuals are huge relative to signal energy, the fit is bad (wrong freq?)
    # But here we just return the wave.
    
    amplitude = np.sqrt(A_term**2 + B_term**2)
    return ideal_wave, amplitude

def process_and_split():
    if not os.path.exists(SAVE_DIR):
        os.makedirs(SAVE_DIR)
        
    print(f"Loading data from: {DATA_DIR}")
    if not os.path.exists(DATA_DIR):
        print("Data directory not found!")
        return

    # 1. Get List
    all_files = [f for f in os.listdir(DATA_DIR) if 'Sub1' in f and f.endswith('.txt')]
    files = sorted(all_files, key=get_freq_from_name)
    
    total_files = len(files)
    print(f"Found {total_files} Sub1 files.")

    train_val_pool = []
    test_chunks = []
    
    meta_info = {'files': [], 'frequencies': []}

    print("Starting generation and split...")
    for i, file in enumerate(files):
        freq = get_freq_from_name(file)
        if freq <= 0:
            print(f"Skipping weird file: {file}")
            continue

        file_path = os.path.join(DATA_DIR, file)
        
        # Read RAW Sub1
        wave = np.array(read_data(file_path, READ_LINES))
        if len(wave) < SLICE_LEN:
            continue
            
        # --- KEY STEP: Generate Label ---
        label_wave, amplitude = generate_ideal_wave(wave, freq)

        # Sanity Check: If fitted amplitude is extremely small, something is wrong
        # (e.g., input was pure noise, or frequency mismatch)
        if amplitude < 1e-3: # Threshold depends on raw data scale
             print(f"WARNING: File {file} (Freq {freq}MHz) yielded near-zero amplitude ({amplitude:.4f}). Skipping.")
             continue

        # Normalize BOTH
        data_max = np.max(wave)
        data_min = np.min(wave)
        # For label, we usually use its own max/min, or the input's max/min if we want to preserve rough scale ratio.
        # Standard practice in this denoise task: Normalize label to [-1, 1] as well.
        label_max = np.max(label_wave)
        label_min = np.min(label_wave)
        
        wave_norm = normalize(wave, data_max, data_min)
        label_norm = normalize(label_wave, label_max, label_min)
        
        # In this approach, alignment is IMPLICITLY handled by the Least Squares fit!
        # The fit finds the phase that best matches the input.
        # No extra 'align_wave' function needed.

        curr_len = len(wave_norm)
        num_chunks = curr_len // SLICE_LEN
        
        if num_chunks == 0:
            continue
            
        wave_norm = wave_norm[:num_chunks * SLICE_LEN]
        label_norm = label_norm[:num_chunks * SLICE_LEN]
        
        # Reshape to (NumChunks, SLICE_LEN)
        w_slices = wave_norm.reshape(-1, SLICE_LEN)
        l_slices = label_norm.reshape(-1, SLICE_LEN)
        
        # Stack: (NumChunks, 2, SLICE_LEN)
        combined_slices = np.stack([w_slices, l_slices], axis=1)
        
        # Split Logic
        # Test: Last 3 chunks
        test_count = min(num_chunks, TEST_SLICES_PER_FREQ)
        
        if test_count > 0:
            # Test takes the LAST chunks
            file_test = combined_slices[-test_count:]
            test_chunks.append(file_test)
            
            # Train/Val takes the rest
            if num_chunks > test_count:
                file_train_val = combined_slices[:-test_count]
                train_val_pool.append(file_train_val)
        
        meta_info['files'].append(file)
        meta_info['frequencies'].append(freq)

        if (i + 1) % 100 == 0:
            print(f"Processed {i + 1}/{total_files} frequencies | Last: {freq}MHz")

    # --- Finalize Dataset ---
    print("\nMixing and saving...")

    if train_val_pool:
        all_train_val = np.concatenate(train_val_pool, axis=0)
        print(f"Total Train/Val samples: {all_train_val.shape[0]}")
        
        # Shuffle
        indices = np.arange(all_train_val.shape[0])
        np.random.shuffle(indices)
        all_train_val = all_train_val[indices]
        
        val_size = int(len(all_train_val) * VAL_RATIO)
        val_data = all_train_val[:val_size]
        train_data = all_train_val[val_size:]
        
        print(f"Train samples: {train_data.shape[0]}")
        print(f"Val samples: {val_data.shape[0]}")
    else:
        print("Error: No training data generated!")
        return

    if test_chunks:
        test_data = np.concatenate(test_chunks, axis=0)
        print(f"Test samples: {test_data.shape[0]}")
    else:
        test_data = np.array([])
        print("Warning: No test data.")

    def save_dataset(data, name):
        if data.size == 0: return
        # Add channel dim: (N, 2, 240, 1)
        data_to_save = data[..., np.newaxis]
        save_path = os.path.join(SAVE_DIR, f"broadband_{name}.pkl")
        with open(save_path, "wb") as f:
            pickle.dump(data_to_save, f, -1)
        print(f"Saved {name} to {save_path}")

    # Save with NEW names to distinguish
    save_dataset(train_data, "train_autogen")
    save_dataset(val_data, "val_autogen")
    save_dataset(test_data, "test_autogen") 
    
    meta_path = os.path.join(SAVE_DIR, "broadband_metadata_autogen.pkl")
    with open(meta_path, "wb") as f:
        pickle.dump(meta_info, f)
    print("Done.")

if __name__ == "__main__":
    process_and_split()