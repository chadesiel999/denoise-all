import numpy as np
import onnxruntime as ort
import matplotlib.pyplot as plt
import pickle
import os
import re
import matplotlib.gridspec as gridspec
import random
from datetime import datetime

# Paths
current_dir = os.path.dirname(os.path.abspath(__file__))
dataset_dir = os.path.join(current_dir, 'dataset')
test_pkl = os.path.join(dataset_dir, 'test_dataset_0202.pkl')
model_path = os.path.join(current_dir, 'saved_model', 'denoise_model_0303.onnx')
figure_dir = os.path.join(current_dir, 'figure100Middle0202_ENOB') # Modified folder name
sub1_path = os.path.join(current_dir, 'data','SplitData-0202_80_20', 'Test', 'sub1')

if not os.path.exists(figure_dir):
    os.makedirs(figure_dir)

# Parameters
WINDOW_SIZE = 240
STRIDE = 100
WINDOWS_PER_FREQ = 18
SEED = 42
FILE_SUFFIX = "GRU1_ENOB" # Modified suffix


def seed_everything(seed=42):
    os.environ["PYTHONHASHSEED"] = str(seed)
    random.seed(seed)
    np.random.seed(seed)

def extract_frequency(filename):
    match = re.search(r'_(\d+)MHz\.txt', filename)
    if match:
        return int(match.group(1))
    return -1

def get_frequencies():
    files = os.listdir(sub1_path)
    files = [f for f in files if f.endswith('.txt')]
    files.sort(key=extract_frequency)
    freqs = [extract_frequency(f) for f in files]
    return freqs

def calculate_snr(clean, noisy):
    noise = noisy - clean
    p_signal = np.mean(clean ** 2)
    p_noise = np.mean(noise ** 2)
    if p_noise == 0:
        return 100 # Infinity
    return 10 * np.log10(p_signal / p_noise)

def calculate_enob(snr_val):
    """
    Calculate ENOB from SINAD (approximated by SNR here).
    Formula: ENOB = (SINAD - 1.76) / 6.02
    """
    if snr_val is None:
        return 0
    return (snr_val - 1.76) / 6.02

def stitch_windows_middle(windows, stride=100, window_size=240):
    # windows shape: (N_windows, 240)
    # Stride of 100.
    # We take middle 100 points from each window.
    # Center of 240 is 120. Middle 100 is indices [70:170] (length 100)
    
    start_idx = (window_size - stride) // 2
    end_idx = start_idx + stride
    
    stitched = []
    for w in windows:
        stitched.append(w[start_idx:end_idx])
        
    return np.concatenate(stitched)

def main():
    seed_everything(SEED)

    run_tag = datetime.now().strftime("%Y%m%d_%H%M%S")
    run_figure_dir = os.path.join(figure_dir, f"{FILE_SUFFIX}_{run_tag}")
    os.makedirs(run_figure_dir, exist_ok=True)

    # 1. Load Data and Frequencies
    print("Loading test data...")
    with open(test_pkl, 'rb') as f:
        data = pickle.load(f) # Shape (N_total, 2, 240)
        
    freqs = get_frequencies()
    num_freqs = len(freqs)
    
    print(f"Found {num_freqs} frequencies.")
    print(f"Data shape: {data.shape}")
    
    expected_samples = num_freqs * WINDOWS_PER_FREQ
    if len(data) != expected_samples:
        print(f"Warning: Data length {len(data)} does not match expected {expected_samples} (18 windows * {num_freqs} freqs)")
        # If mismatch, use minimum
        num_freqs = min(num_freqs, len(data) // WINDOWS_PER_FREQ)
        data = data[:num_freqs * WINDOWS_PER_FREQ]
        freqs = freqs[:num_freqs]

    # 2. Init Model
    print("Loading ONNX model...")
    ort_session = ort.InferenceSession(model_path)
    input_name = ort_session.get_inputs()[0].name
    
    enob_in_list = []
    enob_out_list = []
    
    # Store data for visualization (indices)
    vis_indices = {
        'low': 0, # First freq
        'mid': num_freqs // 2, # Middle freq
        'high': num_freqs - 1 # Last freq
    }
    
    # Randomly jitter indices
    low_range = list(range(0, int(num_freqs * 0.3)))
    mid_range = list(range(int(num_freqs * 0.35), int(num_freqs * 0.65)))
    high_range = list(range(int(num_freqs * 0.7), num_freqs))
    
    if low_range: vis_indices['low'] = random.choice(low_range)
    if mid_range: vis_indices['mid'] = random.choice(mid_range)
    if high_range: vis_indices['high'] = random.choice(high_range)
    
    vis_data = {}

    print("Processing frequencies for ENOB...")
    for i in range(num_freqs):
        start_idx = i * WINDOWS_PER_FREQ
        end_idx = start_idx + WINDOWS_PER_FREQ
        
        chunk = data[start_idx:end_idx] # (18, 2, 240)
        
        x_in = chunk[:, 0, :]
        y_label = chunk[:, 1, :]
        
        # ONNX Inference
        x_in_onnx = x_in[:, :, np.newaxis].astype(np.float32)
        
        outputs = ort_session.run(None, {input_name: x_in_onnx})
        y_pred = outputs[0] # (18, 240, 1)
        y_pred = y_pred.squeeze(-1) # (18, 240)
        
        # Reconstruct / Stitch
        stitched_input = stitch_windows_middle(x_in)
        stitched_label = stitch_windows_middle(y_label)
        stitched_output = stitch_windows_middle(y_pred)
        
        # Calculate SNR first
        snr_in = calculate_snr(stitched_label, stitched_input)
        snr_out = calculate_snr(stitched_label, stitched_output)

        # Calculate ENOB
        enob_in = calculate_enob(snr_in)
        enob_out = calculate_enob(snr_out)
        
        enob_in_list.append(enob_in)
        enob_out_list.append(enob_out)
        
        if i in vis_indices.values():
            key = [k for k, v in vis_indices.items() if v == i][0]
            vis_data[key] = {
                'freq': freqs[i],
                'input': stitched_input,
                'output': stitched_output,
                'label': stitched_label,
                'enob_in': enob_in,
                'enob_out': enob_out
            }
            
    # 3. Plot ENOB Trend
    print("Plotting results...")
    avg_enob_in = np.mean(enob_in_list)
    avg_enob_out = np.mean(enob_out_list)
    
    plt.figure(figsize=(12, 6))
    plt.plot(freqs, enob_in_list, label=f'Original ENOB (Avg: {avg_enob_in:.2f} bits)', alpha=0.7)
    plt.plot(freqs, enob_out_list, label=f'Denoised ENOB (Avg: {avg_enob_out:.2f} bits)', alpha=0.9)
    plt.xlabel('Frequency (MHz)')
    plt.ylabel('ENOB (bits)')
    plt.title('ENOB Trend vs Frequency')
    plt.legend()
    plt.grid(True)
    plt.savefig(os.path.join(run_figure_dir, f'enob_trend_{FILE_SUFFIX}.png'))
    plt.close()
    
    # 4. Plot Visualizations (Waveforms with ENOB labels)
    for key, item in vis_data.items():
        freq = item['freq']
        inp = item['input']
        out = item['output']
        # label = item['label']
        e_in = item['enob_in']
        e_out = item['enob_out']
        
        fig = plt.figure(figsize=(12, 8))
        gs = gridspec.GridSpec(2, 1, height_ratios=[1, 1])
        
        # Full Waveform
        ax0 = plt.subplot(gs[0])
        ax0.plot(inp, label=f'Original (ENOB={e_in:.2f})', color='gray', alpha=0.7)
        ax0.plot(out, label=f'Denoised (ENOB={e_out:.2f})', color='blue', alpha=0.8)
        ax0.set_title(f'Frequency: {freq} MHz - Full Waveform')
        ax0.legend()
        ax0.grid(True)
        
        # Zoomed (First 400 points)
        ax1 = plt.subplot(gs[1])
        zoom_len = min(400, len(inp))
        ax1.plot(inp[:zoom_len], label='Original', color='gray', alpha=0.7)
        ax1.plot(out[:zoom_len], label='Denoised', color='blue', alpha=0.8)
        ax1.set_title(f'Frequency: {freq} MHz - First {zoom_len} Points Zoom')
        ax1.legend()
        ax1.grid(True)
        
        plt.tight_layout()
        plt.savefig(os.path.join(run_figure_dir, f'compare_enob_{key}_{freq}MHz_{FILE_SUFFIX}.png'))
        plt.close()
        
    print(f"All figures saved to {run_figure_dir}")

if __name__ == "__main__":
    main()
