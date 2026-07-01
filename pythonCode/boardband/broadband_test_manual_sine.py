
import numpy as np
import matplotlib.pyplot as plt
import torch
import torch.nn as nn
import os
import sys

# Ensure we can import from the current directory and the boardband directory
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(current_dir)

# Import the model from models.py
try:
    from models import GRUCNNModel
except ImportError:
    # If running from root, models might be in boardband.models
    try:
        from boardband.models import GRUCNNModel
    except ImportError:
        print("Error: Could not import GRUCNNModel from models.py")
        sys.exit(1)

# def GRUCNNModel_removed():
#    pass


def generate_sine_wave(freq_hz, fs_hz, num_samples, snr_db=17):
    """
    Generates a noisy sine wave.
    freq_hz: Signal frequency
    fs_hz: Sampling frequency
    num_samples: Total points
    snr_db: Desired Signal-to-Noise Ratio in dB
    """
    t = np.arange(num_samples) / fs_hz
    clean_signal = np.sin(2 * np.pi * freq_hz * t)
    
    # Calculate signal power
    signal_power = np.mean(clean_signal ** 2)
    
    # Calculate noise power for desired SNR
    # SNR_dB = 10 * log10(P_signal / P_noise)
    # P_noise = P_signal / 10^(SNR_dB / 10)
    noise_power = signal_power / (10 ** (snr_db / 10))
    noise_std = np.sqrt(noise_power)
    
    # Generate noise
    noise = np.random.normal(0, noise_std, num_samples)
    
    noisy_signal = clean_signal + noise
    return t, clean_signal, noisy_signal

def normalize_signal(signal):
    """
    Normalize signal to [-1, 1] based on its own min/max.
    Matches the training preprocessing.
    """
    s_min = np.min(signal)
    s_max = np.max(signal)
    if s_max == s_min:
        return np.zeros_like(signal)
    return 2 * (signal - s_min) / (s_max - s_min) - 1

def run_manual_test():
    # --- Configuration ---
    # Parameters to simulate "100MHz" based on typical high-speed sampling
    # Assumption: Sampling Rate ~ 20-50 GHz based on context (240 points captures ~1-2 cycles of 100MHz?)
    # If 100MHz has period 10ns.
    # If we want 1 cycle in 240 samples: sample_period = 10ns/240 approx 40ps -> 25GHz.
    # If we assume the scope adjusts Fs so we see a few cycles.
    # 100MHz period = 10ns. To see ~5 cycles in 240 points:
    # 5 full cycles = 50ns. 
    # 240 points covers 50ns -> dt = 50/240 ~ 0.2ns = 200ps.
    # Fs = 1/200ps = 5GHz.
    FS = 5e9  
    FREQ = 100e6 # 100 MHz
    NUM_SAMPLES = 240 * 20 # Generate enough for 20 batches
    INPUT_SNR_DB = 17 # User mentioned ~17dB input quality
    
    MODEL_PATH = "/home/uestcauto/cyy/boardband/saved_model/broadband_split_100M20G25M_240.pth"
    # Fallback model path if specific one doesn't exist
    if not os.path.exists(MODEL_PATH):
        # Try finding *any* .pth file in saved_model
        saved_model_dir = "/home/uestcauto/cyy/boardband/saved_model"
        models = [f for f in os.listdir(saved_model_dir) if f.endswith('.pth')]
        if models:
            MODEL_PATH = os.path.join(saved_model_dir, models[0])
            print(f"Requested model not found. Using alternative: {MODEL_PATH}")
        else:
            print("No model weights found!")
            return

    print(f"--- Broadband Manual Sine Wave Test ---")
    print(f"Simulating Frequency: {FREQ/1e6} MHz")
    print(f"Sampling Frequency: {FS/1e9} GHz")
    print(f"Target Input SNR: {INPUT_SNR_DB} dB")
    
    # 1. Generate Synthetic Data
    t, clean_raw, noisy_raw = generate_sine_wave(FREQ, FS, NUM_SAMPLES, snr_db=INPUT_SNR_DB)
    
    # 2. "Save to .txt" (Simulating pipeline)
    print("Saving synthetic data to 'manual_test_data.txt'...")
    np.savetxt("manual_test_data_noisy.txt", noisy_raw)
    np.savetxt("manual_test_data_clean.txt", clean_raw) # For reference
    
    # 3. Load and Preprocess (Mimicking dataset pipeline)
    # Read back (to be pedantic about user request)
    noisy_loaded = np.loadtxt("manual_test_data_noisy.txt")
    clean_loaded = np.loadtxt("manual_test_data_clean.txt")
    
    # Normalize INDEPENDENTLY (Critical: Model expects inputs in [-1, 1])
    # Note: Training normalizes clean label by ITS OWN min/max too.
    noisy_norm = normalize_signal(noisy_loaded)
    clean_norm = normalize_signal(clean_loaded)
    
    # Reshape to (N, 240, 1)
    # Truncate to multiple of 240
    n_batches = len(noisy_norm) // 240
    noisy_norm = noisy_norm[:n_batches*240]
    clean_norm = clean_norm[:n_batches*240]
    
    X_input = noisy_norm.reshape(n_batches, 240, 1)
    Y_label = clean_norm.reshape(n_batches, 240, 1)
    
    # 4. Inference
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    # Initialize with the same hyperparameters as used in training/testing
    model = GRUCNNModel(input_size=1, hidden_size=32, output_size=1).to(device)
    
    checkpoint = torch.load(MODEL_PATH, map_location=device)
    model.load_state_dict(checkpoint)
    model.eval()
    
    input_tensor = torch.from_numpy(X_input).float().to(device)
    
    print("Running inference...")
    with torch.no_grad():
        output_tensor = model(input_tensor)
        output_np = output_tensor.cpu().numpy()
        
    # 5. Evaluate
    # Flatten for global calculation
    clean_flat = Y_label.flatten()
    noisy_flat = X_input.flatten()
    denoised_flat = output_np.flatten()
    
    # Calculate SNR (on normalized data, as that's what the model works with)
    def calc_snr(signal, noise_residue):
        p_signal = np.sum(signal ** 2)
        p_noise = np.sum(noise_residue ** 2)
        return 10 * np.log10(p_signal / p_noise)
    
    input_snr = calc_snr(clean_flat, noisy_flat - clean_flat)
    output_snr = calc_snr(clean_flat, denoised_flat - clean_flat)
    gain = output_snr - input_snr
    
    print("\n--- Results ---")
    print(f"Input SNR (Normalized scale): {input_snr:.2f} dB")
    print(f"Output SNR (Normalized scale): {output_snr:.2f} dB")
    print(f"Denoising Gain: {gain:.2f} dB")
    
    if gain > 5:
        print("\nSUCCESS: Significant denoising observed on completely synthetic data.")
        print("This strongly suggests the model has learned the transformation and is not overfitting to file artifacts.")
    else:
        print("\nWARNING: Low denoising gain. Model might be overfitting or frequency/Fs mismatch.")

    # 6. Visualize
    idx = 0 # Plot first window
    plt.figure(figsize=(12, 6))
    
    plt.subplot(1, 2, 1)
    plt.title("Sample Window (Time Domain)")
    plt.plot(noisy_flat[:240], label='Noisy Input (Simulated)', color='green', alpha=0.5)
    plt.plot(denoised_flat[:240], label='Denoised Output', color='blue', linewidth=2)
    plt.plot(clean_flat[:240], label='Clean GT (Simulated)', color='red', linestyle='--', alpha=0.7)
    plt.legend()
    plt.grid(True)
    
    # Spectrum
    plt.subplot(1, 2, 2)
    plt.title("Spectrum (FFT)")
    def get_spectrum(x):
        return np.abs(np.fft.rfft(x))
    
    # Use longer segment for better fft resolution if available, else first window
    fft_len = 240
    freqs = np.fft.rfftfreq(fft_len, d=1/FS)
    
    input_spec = get_spectrum(noisy_flat[:fft_len])
    output_spec = get_spectrum(denoised_flat[:fft_len])
    clean_spec = get_spectrum(clean_flat[:fft_len])
    
    # Plot log spectrum
    plt.plot(freqs/1e6, 20*np.log10(input_spec + 1e-9), label='Input', color='green', alpha=0.5)
    plt.plot(freqs/1e6, 20*np.log10(output_spec + 1e-9), label='Output', color='blue')
    plt.plot(freqs/1e6, 20*np.log10(clean_spec + 1e-9), label='Clean', color='red', linestyle='--')
    plt.xlabel("Frequency (MHz)")
    plt.ylabel("Magnitude (dB)")
    plt.legend()
    plt.grid(True)
    
    save_fig_path = "broadband_manual_test_result.png"
    plt.savefig(save_fig_path)
    print(f"\nResult plot saved to: {save_fig_path}")

if __name__ == "__main__":
    run_manual_test()
