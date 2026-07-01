import os
import numpy as np

# Configuration
DATA_DIR = "/home/uestcauto/zelin_code/Code/Denoise/datacyy/data0123-100M5G11G20G-25M"
TARGET_FILE = "C1_Sub2_1725MHz.txt"

def analyze_file():
    file_path = os.path.join(DATA_DIR, TARGET_FILE)
    if not os.path.exists(file_path):
        print(f"File not found: {file_path}")
        return

    print(f"--- Inspecting {TARGET_FILE} ---")
    
    try:
        with open(file_path, 'r') as f:
            lines = f.readlines()
            
        print(f"Total lines: {len(lines)}")
        
        # Helper to extract values
        vals = []
        for line in lines:
            try:
                vals.append(float(line.strip()))
            except:
                pass
        vals = np.array(vals)
        
        if len(vals) == 0:
            print("No valid float data found.")
            return

        print(f"\nStats:")
        print(f"  Max: {np.max(vals)}")
        print(f"  Min: {np.min(vals)}")
        print(f"  Mean: {np.mean(vals)}")
        
        print("\nFirst 20 values:")
        print(vals[:20])
        
        print("\nMiddle 20 values (around index 5000):")
        mid = len(vals) // 2
        print(vals[mid:mid+20])
        
        print("\nLast 20 values:")
        print(vals[-20:])
        
        # Check if it is a simple counter
        diffs = np.diff(vals)
        print("\nFirst 20 Differences (Derivative):")
        print(diffs[:20])
        
        if np.allclose(diffs, -1) or np.allclose(diffs, 1):
            print("\n!!! CONCLUSION: This file is just a COUNTER/INDEX, not a signal wave! !!!")
        elif np.allclose(diffs, 0):
             print("\n!!! CONCLUSION: This file is CONSTANT values! !!!")
        else:
             print("\nConclusion: Data varies, but check if it looks like a sine wave.")

    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    analyze_file()