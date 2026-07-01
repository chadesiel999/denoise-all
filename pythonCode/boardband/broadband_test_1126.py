import torch
import numpy as np
import matplotlib.pyplot as plt
import os
import random
from models import GRUCNNModel
from scipy.signal import fftconvolve

# --- Helper Functions (from your data processing script) ---

def calculate_snr(signal, noise):
    """以dB为单位计算信噪比"""
    signal_power = np.mean(np.square(signal))
    noise_power = np.mean(np.square(noise))
    if noise_power == 0:
        return float('inf')  # 噪声为0时，信噪比为无穷大
    snr_db = 10 * np.log10(signal_power / noise_power)
    return snr_db

def read_data(file_path, num_points):
    with open(file_path, 'r') as file:
        lines = file.readlines()
    
    data = []
    for line in lines:
        try:
            # Split the line by whitespace
            parts = line.split()
            # Try to convert the last part of the split to a float.
            # This is more robust if there are one or two columns.
            if parts:
                value = float(parts[-1])
                data.append(value)
        except (ValueError, IndexError):
            # If conversion fails (e.g., it's a header line like "Time, Ampl"), just skip it.
            continue
    
    if not data:
        return []

    # Resample the data to the desired number of points using interpolation
    current_points = len(data)
    x_original = np.linspace(0, 1, current_points)
    x_resampled = np.linspace(0, 1, num_points)
    data_resampled = np.interp(x_resampled, x_original, data)
    
    return data_resampled.tolist()

def align_wave(wave, label_wave):
    wave = np.asarray(wave)
    label_wave = np.asarray(label_wave)
    # Use FFT for fast cross-correlation
    cross_correlation = fftconvolve(wave, label_wave[::-1], mode='full')
    delay = np.argmax(cross_correlation) - len(wave) + 1

    if delay >= 0:
        wave = wave[delay:]
        min_len = min(len(wave), len(label_wave))
        wave = wave[:min_len]
        label_wave = label_wave[:min_len]
    else:
        label_wave = label_wave[-delay:]
        min_len = min(len(wave), len(label_wave))
        wave = wave[:min_len]
        label_wave = label_wave[:min_len]

    return wave, label_wave

def normalize(wave):
    wave = np.array(wave)
    min_val = np.min(wave)
    max_val = np.max(wave)
    if max_val - min_val > 0:
        return (wave - min_val) / (max_val - min_val), min_val, max_val
    return wave, min_val, max_val

def denormalize(wave, min_val, max_val):
    return wave * (max_val - min_val) + min_val

# --- Main Test Logic ---

def test_model():
    # 1. Setup paths and parameters
    data_dir = '/home/uestcauto/zelin_code/Code/Denoise/data-1126/'
    model_path = 'boardband/saved_model/broadband_dataset_1126_combined.pth'
    output_fig_dir = 'boardband/figures/'
    num_points = 10000
    num_test_samples = 5  # Number of random files to test

    if not os.path.exists(output_fig_dir):
        os.makedirs(output_fig_dir)

    # 2. Load Model
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model = GRUCNNModel(input_size=1, hidden_size=32, output_size=1).to(device)
    model.load_state_dict(torch.load(model_path, map_location=device))
    model.eval()
    print(f"Model loaded from {model_path}")

    # 3. Select random files for testing
    all_files = [f for f in os.listdir(data_dir) if f.startswith('C1_Sub1_') and f.endswith('.txt')]
    if not all_files:
        print(f"Error: No 'C1_Sub1_' files found in {data_dir}")
        return
    
    test_files = random.sample(all_files, min(num_test_samples, len(all_files)))
    print(f"Selected {len(test_files)} files for testing...")

    # 4. Loop through test files
    for raw_filename in test_files:
        ideal_filename = raw_filename.replace('C1_Sub1_', 'C1_Sub2_')
        raw_filepath = os.path.join(data_dir, raw_filename)
        ideal_filepath = os.path.join(data_dir, ideal_filename)

        if not os.path.exists(ideal_filepath):
            print(f"Warning: Corresponding ideal file not found for {raw_filename}. Skipping.")
            continue

        print(f"  Testing with: {raw_filename}")

        # 5. Load and preprocess data
        raw_wave_orig = read_data(raw_filepath, num_points)
        ideal_wave_orig = read_data(ideal_filepath, num_points)
        
        if not raw_wave_orig or not ideal_wave_orig:
            print(f"Warning: Failed to read data for {raw_filename}. Skipping.")
            continue

        # Align waves. Use these aligned waves for SNR calculation and plotting.
        raw_wave, ideal_wave = align_wave(raw_wave_orig, ideal_wave_orig)
        
        norm_wave, min_val, max_val = normalize(raw_wave)

        # 6. Model Inference
        with torch.no_grad():
            # Prepare tensor in (batch, seq_len, input_size) format
            input_tensor = torch.from_numpy(norm_wave).float().unsqueeze(0).unsqueeze(-1).to(device)
            output_tensor = model(input_tensor)
            denoised_norm_wave = output_tensor.squeeze().cpu().numpy()

        # Denormalize the output to match original scale
        denoised_wave = denormalize(denoised_norm_wave, min_val, max_val)

        # 7. Calculate SNR
        # Ensure all waves for SNR have the same length after potential alignment/processing
        min_len = min(len(raw_wave), len(ideal_wave), len(denoised_wave))
        raw_wave = raw_wave[:min_len]
        ideal_wave = ideal_wave[:min_len]
        denoised_wave = denoised_wave[:min_len]

        input_noise = raw_wave - ideal_wave
        output_noise = denoised_wave - ideal_wave
        
        snr_input = calculate_snr(ideal_wave, input_noise)
        snr_output = calculate_snr(ideal_wave, output_noise)
        
        print(f"    -> Input SNR: {snr_input:.2f} dB, Output SNR: {snr_output:.2f} dB")

        # 8. Plotting
        plt.figure(figsize=(15, 7))
        plt.plot(raw_wave, label='Original Noisy Signal', color='blue', alpha=0.6)
        plt.plot(denoised_wave, label='Denoised Signal (Model Output)', color='red', linewidth=2)
        plt.plot(ideal_wave, label='Ideal Signal', color='green', linestyle='--', linewidth=2)
        
        plot_title = (f'Denoising Result for {raw_filename}\n'
                      f'Input SNR: {snr_input:.2f} dB | Output SNR: {snr_output:.2f} dB')
        plt.title(plot_title)
        plt.xlabel('Sample Points')
        plt.ylabel('Amplitude')
        plt.legend()
        plt.grid(True)
        
        output_filename = os.path.join(output_fig_dir, f"test_result_{os.path.splitext(raw_filename)[0]}.png")
        plt.savefig(output_filename)
        plt.close()
        print(f"    -> Result saved to {output_filename}")

if __name__ == '__main__':
    test_model()