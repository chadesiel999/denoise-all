import numpy as np
import os
import pickle
import re
import random

# Configuration
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0126-100M6G11G20G25M"
SAVE_DIR = "/home/uestcauto/cyy/boardband/dataset"

# Slicing parameters
SLICE_LEN = 240

# Test configuration
TEST_SLICES_PER_FREQ = 3   # Keep 3 slices per frequency for detailed testing
READ_LINES = 10000         # Actual file length is 10000 lines 
VAL_RATIO = 0.05           # 5% for Validation, 95% for Train (roughly matches 2/38 ratio)

def safe_float_convert(x):
    try:
        return float(x)
    except ValueError as e:
        return None

def read_data(file_path, num_values):
    data = []
    if not os.path.exists(file_path):
        return []
    try:
        with open(file_path, 'r') as file:
            for _ in range(num_values):
                line = file.readline()
                if not line:
                    break
                value = safe_float_convert(line.strip())
                if value is not None:
                    data.append(value)
    except Exception as e:
        print(f"Error reading {file_path}: {e}")
        return []
    return data

def align_wave(wave, label_wave):
    cross_correlation = np.correlate(wave, label_wave, mode='full')
    delay = np.argmax(cross_correlation) - len(wave) + 1

    if delay >= 0:
        wave = wave[delay:]
    else:
        label_wave = label_wave[-delay:]
    
    min_l = min(len(wave), len(label_wave))
    return wave[:min_l], label_wave[:min_l]

def normalize(data, data_max, data_min):
    if data_max == data_min:
        return np.zeros_like(data)
    return 2 * (data - data_min) / (data_max - data_min) - 1

def get_freq_from_name(filename):
    # Matches "C1_Sub1_19175MHz.txt" -> extracts "19175"
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

    # 1. Get List
    all_files = [f for f in os.listdir(DATA_DIR) if 'Sub1' in f and f.endswith('.txt')]
    
    # 2. Sort by NUMERICAL FREQUENCY
    files = sorted(all_files, key=get_freq_from_name)
    
    total_files = len(files)
    print(f"Found {total_files} Sub1 files.")
    
    print("--- Sort Verification ---")
    if total_files > 0:
        print(f"First 3 files (Lowest Freq): {files[:3]}") 
        print(f"Last 3 files (Highest Freq): {files[-3:]}")
    print("-------------------------")

    train_val_pool = []
    test_chunks = []
    
    meta_info = {
        'files': [],
        'test_counts': [],
        'frequencies': [] 
    }

    for i, file in enumerate(files):
        parsed_freq = get_freq_from_name(file)
        
        if file.startswith('C1_Sub1_'):
            refer_file = 'C1_Sub2_' + file[8:]
        else:
            refer_file = file.replace('Sub1', 'Sub2')
            
        file_path = os.path.join(DATA_DIR, file)
        refer_path = os.path.join(DATA_DIR, refer_file)
        
        if not os.path.exists(refer_path):
            print(f"Refer file not found: {refer_file}")
            continue

        wave = np.array(read_data(file_path, READ_LINES))
        refer_wave = np.array(read_data(refer_path, READ_LINES))

        if len(wave) < SLICE_LEN or len(refer_wave) < SLICE_LEN:
            continue

        data_max = np.max(wave)
        data_min = np.min(wave)
        data_max_ref = np.max(refer_wave)
        data_min_ref = np.min(refer_wave)      

        wave = normalize(wave, data_max, data_min)
        refer_wave = normalize(refer_wave, data_max_ref, data_min_ref)

        wave, refer_wave = align_wave(wave, refer_wave)

        curr_len = len(wave)
        num_chunks = curr_len // SLICE_LEN
        
        if num_chunks == 0:
            continue
            
        wave = wave[:num_chunks * SLICE_LEN]
        refer_wave = refer_wave[:num_chunks * SLICE_LEN]
        
        # Reshape to slices
        w_slices = wave.reshape(-1, SLICE_LEN)
        l_slices = refer_wave.reshape(-1, SLICE_LEN)
        
        # Combine (N, 2, SLICE_LEN) for easier handling? 
        # Original code stacked them later. Here we can stack slice-by-slice.
        # But efficiently: 
        combined_slices = np.stack([w_slices, l_slices], axis=1) # Shape: (NumChunks, 2, SLICE_LEN)
        
        # Split logic:
        # Take Test from the END (last N slices)
        test_count = min(num_chunks, TEST_SLICES_PER_FREQ)
        
        if test_count > 0:
            # Test takes the LAST chunks
            file_test = combined_slices[-test_count:]
            test_chunks.append(file_test)
            
            # Remaining goes to Train/Val pool
            if num_chunks > test_count:
                file_train_val = combined_slices[:-test_count]
                # Add to pool (list of arrays, will concat later)
                train_val_pool.append(file_train_val)
        else:
            # Should not happen given num_chunks > 0 check
            pass

        meta_info['files'].append(file)
        meta_info['test_counts'].append(test_count)
        meta_info['frequencies'].append(parsed_freq)

        if (i + 1) % 100 == 0:
            print(f"Processed {i + 1}/{total_files} frequencies")

    print("Processing split logic...")
    
    # Flatten pool
    if train_val_pool:
        all_train_val = np.concatenate(train_val_pool, axis=0)
        print(f"Total Train/Val samples: {all_train_val.shape[0]}")
        
        # Shuffle randomly
        # Use a fixed seed for reproducibility if needed, but 'randomly split' usually implies shuffle
        indices = np.arange(all_train_val.shape[0])
        np.random.shuffle(indices)
        all_train_val = all_train_val[indices]
        
        # Split ratio
        val_size = int(len(all_train_val) * VAL_RATIO)
        train_size = len(all_train_val) - val_size
        
        val_data = all_train_val[:val_size]
        train_data = all_train_val[val_size:]
        
        print(f"Train samples: {train_data.shape[0]}")
        print(f"Val samples: {val_data.shape[0]}")
    else:
        train_data = np.array([])
        val_data = np.array([])
        print("Warning: No train/val data found!")

    # Process Test
    if test_chunks:
        # Don't shuffle test chunks to preserve frequency order
        test_data = np.concatenate(test_chunks, axis=0)
        print(f"Test samples: {test_data.shape[0]}")
    else:
        test_data = np.array([])
        print("Warning: No test data found!")

    def save_dataset(data, name):
        if data.size == 0:
            print(f"Dataset {name} is empty.")
            return
        # Original shape: (N, 2, 240). Original save code added newaxis: data[..., np.newaxis]
        # Shape becomes (N, 2, 240, 1)
        data_to_save = data[..., np.newaxis]
        save_path = os.path.join(SAVE_DIR, f"broadband_{name}.pkl")
        with open(save_path, "wb") as f:
            pickle.dump(data_to_save, f, -1)
        print(f"Saved {name} dataset. Shape: {data_to_save.shape} to {save_path}")

    # Using distinct names to avoid confusion with original strict split
    save_dataset(train_data, "train_random")
    save_dataset(val_data, "val_random")
    save_dataset(test_data, "test_ordered")
    
    meta_path = os.path.join(SAVE_DIR, "broadband_metadata_random.pkl")
    with open(meta_path, "wb") as f:
        pickle.dump(meta_info, f)
    print(f"Saved metadata to {meta_path}")

if __name__ == "__main__":
    # Optional: Seed for reproducibility
    np.random.seed(42)
    process_and_split()
