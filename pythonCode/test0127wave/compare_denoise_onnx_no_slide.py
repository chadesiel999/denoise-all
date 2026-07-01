
import numpy as np
import matplotlib.pyplot as plt
import onnxruntime as ort
import os
import scipy.signal

def safe_float_convert(x):
    try:
        return float(x.strip())
    except ValueError:
        return None

def read_data(file_path):
    data = []
    with open(file_path, 'r', encoding='utf-8', errors='ignore') as file:
        lines = file.readlines()
        for line in lines:
            val = safe_float_convert(line)
            if val is not None:
                data.append(val)
    return np.array(data)

def normalize_signal(data):
    data_min = np.min(data)
    data_max = np.max(data)
    if data_max == data_min:
        return np.zeros_like(data), 0, 1
    # Normalized to [-1, 1]
    norm_data = 2 * (data - data_min) / (data_max - data_min) - 1
    return norm_data, data_min, data_max

def denormalize_signal(norm_data, data_min, data_max):
    return (norm_data + 1) / 2 * (data_max - data_min) + data_min

def align_signals(ref_signal, target_signal):
    """
    Align target_signal to ref_signal using cross-correlation.
    Returns aligned_target (trimmed/padded) to match ref_signal length,
    or returns the valid overlapping parts of both.
    Here we try to find where target_signal starts within ref_signal or vice versa.
    """
    # Normalize locally for correlation to avoid amplitude bias
    ref_n = (ref_signal - np.mean(ref_signal)) / (np.std(ref_signal) + 1e-9)
    tar_n = (target_signal - np.mean(target_signal)) / (np.std(target_signal) + 1e-9)
    
    correlation = scipy.signal.correlate(ref_n, tar_n, mode='full')
    lags = scipy.signal.correlation_lags(len(ref_n), len(tar_n), mode='full')
    lag = lags[np.argmax(correlation)]
    
    print(f"Detected lag: {lag}")

    # lag is (index in ref) - (index in target)
    # if lag > 0: target starts 'lag' samples before ref? 
    # Let's visualize: 
    # if ref is [0, 1, 2], target is [1, 2] -> target matches ref at index 1.
    # correlate result max will be when target slides to match.
    
    # If lag is positive, it means the target signal needs to be shifted RIGHT by `lag` to match ref?
    # Or ref starts `lag` samples after target.
    
    # Simplified approach:
    # return the overlapping segments
    
    if lag > 0:
        # Ref starts ahead of Target (Target is delayed? No, Ref is delayed relative to Target start)
        # We slice Ref from lag onwards, and Target from 0
        # Actually let's just use the lag to slice arrays
        # aligned_ref = ref_signal[lag:]
        # aligned_tar = target_signal
        # But we also need to handle length differences
        
        # If we want to align visual plots, we shift target.
        # Let's truncate to common overlap
        start_ref = lag
        start_tar = 0
    else:
        start_ref = 0
        start_tar = -lag
        
    end_ref = len(ref_signal)
    end_tar = len(target_signal)
    
    # Calculate common length
    len_ref = end_ref - start_ref
    len_tar = end_tar - start_tar
    common_len = min(len_ref, len_tar)
    
    return ref_signal[start_ref : start_ref+common_len], target_signal[start_tar : start_tar+common_len]

def calculate_snr(signal, noise):
    # Assumes signal is the clean reference, noise is the difference
    signal_power = np.mean(signal ** 2)
    noise_power = np.mean(noise ** 2)
    if noise_power == 0:
        return float('inf')
    return 10 * np.log10(signal_power / noise_power)

def calculate_consistency_snr(reference, target):
    # Treats reference as "Signal" and (reference - target) as "Noise"
    diff = reference - target
    return calculate_snr(reference, diff)

def calculate_spectral_snr(signal):
    """
    Calculate SNR based on frequency spectrum (assuming single tone signal).
    Returns freqs, magnitude_db, snr_value
    """
    # Remove DC
    sig = signal - np.mean(signal)
    n = len(sig)
    # Hanning window to reduce leakage
    window = np.hanning(n)
    
    # FFT
    fft_spec = np.fft.rfft(sig * window)
    mag_spec = np.abs(fft_spec)
    
    # Find peak (Signal)
    peak_idx = np.argmax(mag_spec)
    
    # Define signal region (peak +/- some bins)
    # Determine width based on peak width roughly
    bin_width = 5
    low = max(0, peak_idx - bin_width)
    high = min(len(mag_spec), peak_idx + bin_width + 1)
    
    signal_energy = np.sum(mag_spec[low:high]**2)
    total_energy = np.sum(mag_spec**2)
    noise_energy = total_energy - signal_energy
    
    if noise_energy <= 1e-10:
        snr_val = 100.0
    else:
        snr_val = 10 * np.log10(signal_energy / noise_energy)
        
    # Convert to dB for plotting (Normalize to peak 0dB for better visual comparison of shape, 
    # or keep relative amplitude to show noise floor drop? 
    # Usually relative amplitude is better to show noise suppression)
    # Let's use 20log10(mag)
    mag_db = 20 * np.log10(mag_spec + 1e-12)
    
    freqs = np.fft.rfftfreq(n)
    return freqs, mag_db, snr_val

def main():
    # Paths
    base_dir = "/home/uestcauto/cyy/test0127wave/test0127/800M"
    raw_file = os.path.join(base_dir, "新建 文本文档.txt")
    machine_file = os.path.join(base_dir, "新建 文本文档 (2).txt")
    model_path = "/home/uestcauto/cyy/boardband/saved_model/broadband_random_100M5G11G20G25M_240.onnx"
    output_dir = "/home/uestcauto/cyy/test0127wave/figure"
    
    os.makedirs(output_dir, exist_ok=True)
    
    # 1. Load Data
    print("Loading data...")
    raw_data = read_data(raw_file)
    machine_data = read_data(machine_file)
    print(f"Raw data shape: {raw_data.shape}")
    print(f"Machine data shape: {machine_data.shape}")
    
    if len(raw_data) == 0 or len(machine_data) == 0:
        print("Error: Empty data files.")
        return

    # 2. Preprocess Raw Data for Model
    print("Running Python Simulation (ONNX Batch Inference - No Sliding Window)...")
    # Normalize
    # Note: C# code uses per-file max/min for normalization, consistent with our logic here.
    raw_norm, raw_min, raw_max = normalize_signal(raw_data)
    
    # --- Logic: No Sliding Window (Batch Inference) ---
    # Just reshape to [N, 240, 1]
    
    if not os.path.exists(model_path):
        print(f"Error: Model file not found at {model_path}")
        return
    session = ort.InferenceSession(model_path)
    input_name = session.get_inputs()[0].name
    
    SEQ_LEN = 240
    num_samples = len(raw_norm)
    num_batches = num_samples // SEQ_LEN
    
    if num_batches == 0:
        print("Data too short")
        return

    # Prepare batch input
    input_tensor = raw_norm[:num_batches*SEQ_LEN].reshape(num_batches, SEQ_LEN, 1).astype(np.float32)
    
    outputs = session.run(None, {input_name: input_tensor})
    denoised_norm = outputs[0] # [N, 240, 1]
    
    denoised_norm_flat = denoised_norm.flatten()
    denoised_py = denormalize_signal(denoised_norm_flat, raw_min, raw_max)
    
    print(f"Python Denoised shape: {denoised_py.shape}")
    
    # 3. Handling Trimmed/Aligned Data
    raw_trimmed = raw_data 

    # 4. Compare with Machine Output
    print("Aligning signals...")
    
    # Batch inference logic means Python output starts at index 0 of Raw Data.
    offset_py = 0
    
    # Now we want to align everything.
    # Let's align Machine Data to Raw Data first (physically measured lag).
    
    ref_n = (raw_trimmed - np.mean(raw_trimmed)) / (np.std(raw_trimmed) + 1e-9)
    tar_n = (machine_data - np.mean(machine_data)) / (np.std(machine_data) + 1e-9)
    correlation = scipy.signal.correlate(ref_n, tar_n, mode='full')
    lags = scipy.signal.correlation_lags(len(ref_n), len(tar_n), mode='full')
    lag = lags[np.argmax(correlation)]
    
    print(f"Machine to Raw Lag: {lag}")
    
    # lag > 0 => Raw starts 'lag' samples before Machine (Machine is delayed by 'lag' relative to Raw[0])
    # DenoisedPy is delayed by 'offset_py' (0) relative to Raw[0].
    
    start_t = max(0, lag, offset_py)
    end_t = min(len(raw_trimmed), len(machine_data) + lag, len(denoised_py) + offset_py)
    
    if end_t <= start_t:
        print("Error: No overlapping segment found.")
        print(f"Lag: {lag}, Offset: {offset_py}")
        return

    common_len = end_t - start_t
    
    final_raw = raw_trimmed[start_t : end_t]
    final_mach = machine_data[start_t - lag : end_t - lag]
    final_py = denoised_py[start_t - offset_py : end_t - offset_py]
    
    print(f"Aligned Length: {common_len}")
    
    # 5. Metrics
    # Calculate independent SNR for each signal using spectral method
    freqs_raw, mag_raw, snr_raw = calculate_spectral_snr(final_raw)
    freqs_py, mag_py, snr_py = calculate_spectral_snr(final_py)
    freqs_mach, mag_mach, snr_mach = calculate_spectral_snr(final_mach)
    
    # MSE
    mse = np.mean((final_py - final_mach) ** 2)
    
    print("-" * 30)
    print(f"Comparison Results (No Sliding Window):")
    print(f"Normalization Pars: Min={raw_min:.4f}, Max={raw_max:.4f}")
    print(f"Aligned Length: {common_len}")
    print(f"Independent SNR - Raw Input: {snr_raw:.2f} dB")
    print(f"Independent SNR - Python Output: {snr_py:.2f} dB")
    print(f"Independent SNR - Machine Output: {snr_mach:.2f} dB")
    print(f"MSE (Sim vs Machine): {mse:.4f}")
    print("-" * 30)
    
    # 6. Plotting
    plt.figure(figsize=(15, 12))
    
    # Time domain
    plt.subplot(4, 1, 1)
    plt.plot(final_raw, label=f'Raw Input (SNR: {snr_raw:.2f} dB)', alpha=0.5, color='gray')
    plt.plot(final_py, label=f'Python Sim Output (NoSlide) (SNR: {snr_py:.2f} dB)', alpha=0.8, color='blue')
    plt.legend()
    plt.title(f'Simulation: Input vs Output (Batch Mode)')
    plt.grid(True)

    plt.subplot(4, 1, 2)
    plt.plot(final_mach, label=f'Machine Output (SNR: {snr_mach:.2f} dB)', alpha=0.8, color='red')
    plt.plot(final_py, label=f'Python Sim Output (NoSlide) (SNR: {snr_py:.2f} dB)', alpha=0.8, color='blue', linestyle='--')
    plt.legend()
    plt.title(f'Implementation vs Simulation')
    plt.grid(True)
    
    # Detail view (zoom in)
    plt.subplot(4, 1, 3)
    zoom_len = min(400, common_len)
    plt.plot(final_mach[:zoom_len], label='Machine Output', color='red')
    plt.plot(final_py[:zoom_len], label='Python Sim Output', color='blue', linestyle='--')
    plt.legend()
    plt.title(f'Zoomed View (First {zoom_len} points)')
    plt.grid(True)
    
    # Difference
    plt.subplot(4, 1, 4)
    diff = final_py - final_mach
    plt.plot(diff, label='Error (Python - Machine)', color='green')
    plt.legend()
    plt.title(f'Implementation Error (MSE: {mse:.4f})')
    plt.grid(True)
    
    save_path = os.path.join(output_dir, 'comparison_800M_no_slide.png')
    plt.tight_layout()
    plt.savefig(save_path)
    print(f"Figure saved to {save_path}")
    
    # --- New Feature: Spectral Analysis & SNR Comparison ---
    print("Performing Spectral Analysis...")
    # Values already calculated in step 5
    
    plt.figure(figsize=(12, 8))
    plt.plot(freqs_raw, mag_raw, label=f'Raw Input (SNR: {snr_raw:.2f} dB)', alpha=0.6, color='gray')
    plt.plot(freqs_mach, mag_mach, label=f'Machine Output (SNR: {snr_mach:.2f} dB)', alpha=0.8, color='red')
    
    plt.title('Spectrum Comparison: Raw Input vs Machine Output')
    plt.xlabel('Normalized Frequency')
    plt.ylabel('Magnitude (dB)')
    plt.legend()
    plt.grid(True, which='both', linestyle='--', alpha=0.7)
    
    spectrum_save_path = os.path.join(output_dir, 'spectrum_comparison_800M_no_slide.png')
    plt.savefig(spectrum_save_path)
    print(f"Spectrum figure saved to {spectrum_save_path}")
    # -------------------------------------------------------

    # Save data for closer inspection if needed
    save_txt_path = os.path.join(output_dir, 'comparison_stats_no_slide.txt')
    with open(save_txt_path, 'w') as f:
        f.write(f"Raw Min: {raw_min}\nRaw Max: {raw_max}\n")
        f.write(f"Input SNR: {snr_raw:.2f} dB\n")
        f.write(f"Machine Output SNR: {snr_mach:.2f} dB\n")
        f.write(f"Python Output SNR: {snr_py:.2f} dB\n")
        f.write(f"MSE: {mse:.4f}\n")
if __name__ == "__main__":
    main()
