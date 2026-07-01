import pickle
import numpy as np
import os
import sys

DATA_DIR = "/home/uestcauto/cyy/boardband/dataset"

def check_pkl(filename):
    path = os.path.join(DATA_DIR, filename)
    print(f"\n--- Checking {filename} ---")
    if not os.path.exists(path):
        print(f"File not found: {path}")
        return

    with open(path, "rb") as f:
        data = pickle.load(f)
    
    # Expected shape: (N, 2, 240, 1) or (N, 2, 240)
    print(f"Shape: {data.shape}")
    
    if data.ndim == 4:
        # (N, Channels, Length, 1)
        X = data[:, 0, :, 0]
        Y = data[:, 1, :, 0]
    elif data.ndim == 3:
        # (N, Channels, Length)
        X = data[:, 0, :]
        Y = data[:, 1, :]
    else:
        print("Unexpected dimensions")
        return

    print(f"X (Input)  - Mean: {np.mean(X):.4f}, Std: {np.std(X):.4f}, Min: {np.min(X):.4f}, Max: {np.max(X):.4f}")
    print(f"Y (Label)  - Mean: {np.mean(Y):.4f}, Std: {np.std(Y):.4f}, Min: {np.min(Y):.4f}, Max: {np.max(Y):.4f}")

    # Check for zeros
    zeros_x = np.sum(X == 0)
    total_x = X.size
    print(f"Zeros in X: {zeros_x}/{total_x} ({zeros_x/total_x*100:.2f}%)")
    
    if zeros_x == total_x:
        print("!! ALERT: Input X is all ZEROS !!")
    
    # Check correlation of first few samples
    print("\nSample Correlation (First 5):")
    for i in range(min(5, len(X))):
        corr = np.corrcoef(X[i], Y[i])[0, 1]
        print(f"  Sample {i}: Corr={corr:.4f}")
        # print(f"    X sample stats: min={np.min(X[i]):.3f}, max={np.max(X[i]):.3f}")

if __name__ == "__main__":
    check_pkl("broadband_train_synth.pkl")
    check_pkl("broadband_val_synth.pkl")
    check_pkl("broadband_test_synth.pkl")