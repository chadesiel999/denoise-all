
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
figure_dir = os.path.join(current_dir, 'figure100Middle0202')
sub1_path = os.path.join(current_dir, 'data','SplitData-0202_80_20', 'Test', 'sub1')

if not os.path.exists(figure_dir):
    os.makedirs(figure_dir)

# Parameters
WINDOW_SIZE = 240
STRIDE = 100
WINDOWS_PER_FREQ = 18
SEED = 42
FILE_SUFFIX = "deConvandoneunidirectional"


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
    
    snr_in_list = []
    snr_out_list = []
    
    # Store data for visualization (indices)
    vis_indices = {
        'low': 0, # First freq
        'mid': num_freqs // 2, # Middle freq
        'high': num_freqs - 1 # Last freq
    }
    
    # Randomly jitter indices to pick random ones if desired, but fixed low/mid/high is often better for "low mid high" request
    # User said "randomly extract low mid high". 
    # Let's define ranges: Low (0-30%), Mid (35-65%), High (70-100%)
    low_range = list(range(0, int(num_freqs * 0.3)))
    mid_range = list(range(int(num_freqs * 0.35), int(num_freqs * 0.65)))
    high_range = list(range(int(num_freqs * 0.7), num_freqs))
    
    vis_indices['low'] = random.choice(low_range)
    vis_indices['mid'] = random.choice(mid_range)
    vis_indices['high'] = random.choice(high_range)
    
    vis_data = {}

    print("Processing frequencies...")
    for i in range(num_freqs):
        start_idx = i * WINDOWS_PER_FREQ
        end_idx = start_idx + WINDOWS_PER_FREQ
        
        chunk = data[start_idx:end_idx] # (18, 2, 240)
        
        # Prepare input
        # Input X: chunk[:, 0, :] -> (18, 240)
        # Label Y: chunk[:, 1, :] -> (18, 240)
        
        x_in = chunk[:, 0, :]
        y_label = chunk[:, 1, :]
        
        # ONNX Inference expects ( Batch, 240, 1 ) based on previous training export
        # Check export code: dynamic_axes={'input': {0: 'batch_size'}, ...}
        # Model input size: (N, 240, 1)
        
        x_in_onnx = x_in[:, :, np.newaxis].astype(np.float32)
        
        outputs = ort_session.run(None, {input_name: x_in_onnx})
        y_pred = outputs[0] # (18, 240, 1)
        y_pred = y_pred.squeeze(-1) # (18, 240)
        
        # Reconstruct / Stitch
        # Stitch Input (Noisy), Label (Clean), Output (Denoised)
        stitched_input = stitch_windows_middle(x_in)
        stitched_label = stitch_windows_middle(y_label)
        stitched_output = stitch_windows_middle(y_pred)
        
        # Calculate SNR
        # SNR_in: comparison between Noisy Input and Clean Label
        snr_in = calculate_snr(stitched_label, stitched_input)
        
        # SNR_out: comparison between Denoised Output and Clean Label
        snr_out = calculate_snr(stitched_label, stitched_output)
        
        snr_in_list.append(snr_in)
        snr_out_list.append(snr_out)
        
        if i in vis_indices.values():
            key = [k for k, v in vis_indices.items() if v == i][0]
            vis_data[key] = {
                'freq': freqs[i],
                'input': stitched_input,
                'output': stitched_output,
                'label': stitched_label,
                'snr_in': snr_in,
                'snr_out': snr_out
            }
            
    # 3. Plot SNR Trend
    print("Plotting results...")
    avg_snr_in = np.mean(snr_in_list)
    avg_snr_out = np.mean(snr_out_list)
    
    plt.figure(figsize=(12, 6))
    plt.plot(freqs, snr_in_list, label=f'Original SNR (Avg: {avg_snr_in:.2f}dB)', alpha=0.7)
    plt.plot(freqs, snr_out_list, label=f'Denoised SNR (Avg: {avg_snr_out:.2f}dB)', alpha=0.9)
    plt.xlabel('Frequency (MHz)')
    plt.ylabel('SNR (dB)')
    plt.title('SNR Trend vs Frequency')
    plt.legend()
    plt.grid(True)
    plt.savefig(os.path.join(run_figure_dir, f'snr_trend_{FILE_SUFFIX}.png'))
    plt.close()
    
    # 4. Plot Visualizations
    for key, item in vis_data.items():
        freq = item['freq']
        inp = item['input']
        out = item['output']
        # label = item['label']
        snr_in = item['snr_in']
        snr_out = item['snr_out']
        
        fig = plt.figure(figsize=(12, 8))
        gs = gridspec.GridSpec(2, 1, height_ratios=[1, 1])
        
        # Full Waveform
        ax0 = plt.subplot(gs[0])
        ax0.plot(inp, label=f'Original (SNR={snr_in:.2f}dB)', color='gray', alpha=0.7)
        ax0.plot(out, label=f'Denoised (SNR={snr_out:.2f}dB)', color='blue', alpha=0.8)
        # ax0.plot(label, label='Clean Label', color='green', alpha=0.5, linestyle='--')
        ax0.set_title(f'Frequency: {freq} MHz - Full Waveform')
        ax0.legend()
        ax0.grid(True)
        
        # Zoomed (First 400 points)
        ax1 = plt.subplot(gs[1])
        zoom_len = min(400, len(inp))
        ax1.plot(inp[:zoom_len], label='Original', color='gray', alpha=0.7)
        ax1.plot(out[:zoom_len], label='Denoised', color='blue', alpha=0.8)
        # ax1.plot(label[:zoom_len], label='Clean', color='green', alpha=0.5, linestyle='--')
        ax1.set_title(f'Frequency: {freq} MHz - First {zoom_len} Points Zoom')
        ax1.legend()
        ax1.grid(True)
        
        plt.tight_layout()
        plt.savefig(os.path.join(run_figure_dir, f'compare_{key}_{freq}MHz_{FILE_SUFFIX}.png'))
        plt.close()
        
    print(f"All figures saved to {run_figure_dir}")

if __name__ == "__main__":
    main()
