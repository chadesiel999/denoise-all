import numpy as np
import os
import pickle
import re

# Configuration
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0105-100M5G11G20G-25M"
SAVE_DIR = "/home/uestcauto/cyy/boardband/dataset"

# Slicing parameters
SLICE_LEN = 240

# Target distribution
TRAIN_SLICES_PER_FREQ = 36 # Keeps ~20k train slices (36 * 556 = 20016)
TEST_SLICES_PER_FREQ = 3   # Remaining budget: 41 - 36 - 2 = 3
VAL_SLICES_PER_FREQ = 2    # Reduced to fit: 10000 lines -> ~41 chunks

READ_LINES = 10000         # Actual file length is 10000 lines 

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

# --- FIXED: Sorting Logic for MHz files ---
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
        print(f"First 3 files (Lowest Freq): {files[:3]}") # Should look like 100MHz, 125MHz...
        print(f"Last 3 files (Highest Freq): {files[-3:]}") # Should look like 19975MHz, 20000MHz...
    print("-------------------------")

    train_chunks = []
    val_chunks = []
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
            meta_info['files'].append(file)
            meta_info['test_counts'].append(0)
            meta_info['frequencies'].append(parsed_freq)
            continue

        wave = np.array(read_data(file_path, READ_LINES))
        refer_wave = np.array(read_data(refer_path, READ_LINES))

        if len(wave) < SLICE_LEN or len(refer_wave) < SLICE_LEN:
            meta_info['files'].append(file)
            meta_info['test_counts'].append(0)
            meta_info['frequencies'].append(parsed_freq)
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
            meta_info['files'].append(file)
            meta_info['test_counts'].append(0)
            meta_info['frequencies'].append(parsed_freq)
            continue
            
        wave = wave[:num_chunks * SLICE_LEN]
        refer_wave = refer_wave[:num_chunks * SLICE_LEN]
        
        w_slices = wave.reshape(-1, SLICE_LEN)
        l_slices = refer_wave.reshape(-1, SLICE_LEN)

        idx = 0
        
        # Train
        count = TRAIN_SLICES_PER_FREQ
        if idx + count <= num_chunks:
            train_chunks.append(np.stack([w_slices[idx:idx+count], l_slices[idx:idx+count]], axis=1))
            idx += count
        else:
            train_chunks.append(np.stack([w_slices[idx:], l_slices[idx:]], axis=1))
            idx = num_chunks
        
        # Val
        count = VAL_SLICES_PER_FREQ
        if idx < num_chunks:
            take = min(count, num_chunks - idx)
            val_chunks.append(np.stack([w_slices[idx:idx+take], l_slices[idx:idx+take]], axis=1))
            idx += take
            
        # Test
        count = TEST_SLICES_PER_FREQ
        actual_test_take = 0
        if idx < num_chunks:
            actual_test_take = min(count, num_chunks - idx)
            if actual_test_take > 0:
                test_chunks.append(np.stack([w_slices[idx:idx+actual_test_take], l_slices[idx:idx+actual_test_take]], axis=1))
            idx += actual_test_take
            
        meta_info['files'].append(file)
        meta_info['test_counts'].append(actual_test_take)
        meta_info['frequencies'].append(parsed_freq)

        if (i + 1) % 100 == 0:
            print(f"Processed {i + 1}/{total_files} frequencies")

    def save_dataset(chunks, name):
        if not chunks:
            print(f"Dataset {name} is empty.")
            return
        data = np.concatenate(chunks, axis=0)
        data = data[..., np.newaxis]
        save_path = os.path.join(SAVE_DIR, f"broadband_{name}.pkl")
        with open(save_path, "wb") as f:
            pickle.dump(data, f, -1)
        print(f"Saved {name} dataset. Shape: {data.shape} to {save_path}")

    save_dataset(train_chunks, "trainno2")
    save_dataset(val_chunks, "valno2")
    save_dataset(test_chunks, "testno2")
    
    meta_path = os.path.join(SAVE_DIR, "broadband_metadatano2.pkl")
    with open(meta_path, "wb") as f:
        pickle.dump(meta_info, f)
    print(f"Saved metadata to {meta_path}")

if __name__ == "__main__":
    process_and_split()
