
import torch
import torch.nn as nn
import numpy as np
from scipy import signal
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import time
import pickle
import os
import sys
import random
from torch.utils.data import Dataset, DataLoader

# Add boardband to path to import models
current_dir = os.path.dirname(os.path.abspath(__file__))
boardband_dir = os.path.abspath(os.path.join(current_dir, '../boardband'))
sys.path.append(boardband_dir)

from models import GRUCNNmodModel

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# Parameters
BATCH_SIZE = 512 # Adjust based on GPU memory
NUM_EPOCHS = 1500
LEARNING_RATE = 1e-4
HIDDEN_SIZE = 32 # baseline:32
RANDOM_SEED = 42
FREQ_WEIGHT = 0.5 # Based solely on user's previous code it was variable, but usually 0.5 or similar. 
# Looking at provided broadband_denoise_new.py: freq_weight = 0 means only time loss? 
# Wait, previous code says: freq_weight = 0 #1e-3. 
# But then: loss = (1-freq_weight)*time_loss + freq_weight * freq_loss
# So if freq_weight = 0, it uses only time_loss.
# In broadband_denoise_new.py: freq_weight = 0.
# I will stick to 0 unless I see reason otherwise.
FREQ_WEIGHT = 0


def seed_everything(seed=42):
    os.environ["PYTHONHASHSEED"] = str(seed)
    random.seed(seed)
    np.random.seed(seed)
    torch.manual_seed(seed)
    if torch.cuda.is_available():
        torch.cuda.manual_seed(seed)
        torch.cuda.manual_seed_all(seed)

    torch.backends.cudnn.deterministic = True
    torch.backends.cudnn.benchmark = False
    torch.use_deterministic_algorithms(True, warn_only=True)


def seed_worker(worker_id):
    worker_seed = torch.initial_seed() % (2**32)
    np.random.seed(worker_seed)
    random.seed(worker_seed)


def count_parameters(model):
    total_params = sum(param.numel() for param in model.parameters())
    trainable_params = sum(param.numel() for param in model.parameters() if param.requires_grad)
    return total_params, trainable_params


def get_peak_memory_mb():
    if not torch.cuda.is_available():
        return None
    peak_bytes = torch.cuda.max_memory_allocated(device=device)
    return peak_bytes / (1024 ** 2)


def maybe_cuda_sync():
    if torch.cuda.is_available():
        torch.cuda.synchronize(device=device)


def to_db(values, eps=1e-12):
    values = np.asarray(values, dtype=np.float64)
    return 10.0 * np.log10(np.maximum(values, eps))


def plot_loss_comparison_db(results_dir):
    loss_0202_path = os.path.join(results_dir, 'loss_0202.txt')
    val_loss_0202_path = os.path.join(results_dir, 'val_loss_0202.txt')
    loss_0303_path = os.path.join(results_dir, 'loss_0303.txt')
    val_loss_0303_path = os.path.join(results_dir, 'val_loss_0303.txt')

    loss_0202 = np.loadtxt(loss_0202_path)
    val_loss_0202 = np.loadtxt(val_loss_0202_path)
    loss_0303 = np.loadtxt(loss_0303_path)
    val_loss_0303 = np.loadtxt(val_loss_0303_path)

    loss_0202_db = to_db(loss_0202)
    val_loss_0202_db = to_db(val_loss_0202)
    loss_0303_db = to_db(loss_0303)
    val_loss_0303_db = to_db(val_loss_0303)

    plt.figure(figsize=(10, 6))
    plt.plot(np.arange(1, len(loss_0202_db) + 1), loss_0202_db, label='loss_0202 (dB)', linewidth=1.5)
    plt.plot(np.arange(1, len(loss_0303_db) + 1), loss_0303_db, label='loss_0303 (dB)', linewidth=1.5)
    plt.xlabel('Epoch')
    plt.ylabel('Loss (dB)')
    plt.title('Train Loss Comparison (0202 vs 0303) in dB')
    plt.grid(True, alpha=0.3)
    plt.legend()
    plt.tight_layout()
    train_fig_path = os.path.join(results_dir, 'loss_compare_0202_0303_db.png')
    plt.savefig(train_fig_path, dpi=200)
    plt.close()

    plt.figure(figsize=(10, 6))
    plt.plot(np.arange(1, len(val_loss_0202_db) + 1), val_loss_0202_db, label='val_loss_0202 (dB)', linewidth=1.5)
    plt.plot(np.arange(1, len(val_loss_0303_db) + 1), val_loss_0303_db, label='val_loss_0303 (dB)', linewidth=1.5)
    plt.xlabel('Epoch')
    plt.ylabel('Validation Loss (dB)')
    plt.title('Validation Loss Comparison (0202 vs 0303) in dB')
    plt.grid(True, alpha=0.3)
    plt.legend()
    plt.tight_layout()
    val_fig_path = os.path.join(results_dir, 'val_loss_compare_0202_0303_db.png')
    plt.savefig(val_fig_path, dpi=200)
    plt.close()

    print(f"Saved train loss comparison figure: {train_fig_path}")
    print(f"Saved val loss comparison figure: {val_fig_path}")

def load_data(pickle_path):
    print(f"Loading {pickle_path}...")
    with open(pickle_path, 'rb') as f:
        data = pickle.load(f)
    print(f"Loaded shape: {data.shape}")
    # data is (N, 2, 240)
    # X: data[:, 0, :] -> (N, 240)
    # Y: data[:, 1, :] -> (N, 240)
    X = data[:, 0, :]
    Y = data[:, 1, :]
    
    # Reshape for model: (N, 240, 1)
    X = X[:, :, np.newaxis]
    Y = Y[:, :, np.newaxis]
    
    return X, Y

class MyDataset(Dataset):
    def __init__(self, data_x, data_y):
        self.data_x = torch.from_numpy(data_x).float()
        self.data_y = torch.from_numpy(data_y).float()
        
    def __getitem__(self, index):
        return self.data_x[index], self.data_y[index]

    def __len__(self):
        return len(self.data_x)

def train():
    seed_everything(RANDOM_SEED)

    # Paths
    dataset_dir = os.path.join(current_dir, 'dataset')
    train_pkl = os.path.join(dataset_dir, 'train_dataset_0202.pkl')
    val_pkl = os.path.join(dataset_dir, 'val_dataset_0202.pkl')
    # test_pkl = os.path.join(dataset_dir, 'test_dataset_0202.pkl') 
    
    save_model_dir = os.path.join(current_dir, 'saved_model')
    results_dir = os.path.join(current_dir, 'results')
    os.makedirs(save_model_dir, exist_ok=True)
    os.makedirs(results_dir, exist_ok=True)

    # Load Data
    X_train, Y_train = load_data(train_pkl)
    X_val, Y_val = load_data(val_pkl)
    
    # Create DataLoaders
    # Note: user's code put all data to device in __init__. 
    # For large datasets this might OOM. I'll move to device in loop.
    # User's code: self.data = data_root.to(device)
    # 36k * 240 float32 is small (~33MB). It fits in VRAM. I'll follow user pattern for speed.
    
    # Actually, let's stick to standard practice: move to device in loop
    # unless user insists on exact copy. User said "仿照".
    # User code used `MyDataset` which moved to device in `__init__`.
    # I'll try to stick to efficient loading.
    
    dataset_train = MyDataset(X_train, Y_train)
    dataset_val = MyDataset(X_val, Y_val)

    g = torch.Generator()
    g.manual_seed(RANDOM_SEED)
    
    train_loader = DataLoader(
        dataset_train,
        batch_size=BATCH_SIZE,
        shuffle=True,
        num_workers=0,
        worker_init_fn=seed_worker,
        generator=g,
    )
    # Val batch size: all of it? user code: batch_size=int(val_len)
    val_loader = DataLoader(
        dataset_val,
        batch_size=BATCH_SIZE,
        shuffle=False,
        num_workers=0,
        worker_init_fn=seed_worker,
    )

    # Model
    model = GRUCNNmodModel(input_size=1, hidden_size=HIDDEN_SIZE, output_size=1)
    model = model.to(device)
    total_params, trainable_params = count_parameters(model)
    print(f"Model params - total: {total_params:,}, trainable: {trainable_params:,}")
    
    optimizer = torch.optim.Adam(model.parameters(), lr=LEARNING_RATE, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
    criterion = nn.MSELoss()
    
    losses = []
    val_losses = []
    min_val_loss = np.inf
    patience = 0
    patience_limit = 50
    
    print("Starting training...")
    
    for epoch in range(NUM_EPOCHS):
        maybe_cuda_sync()
        epoch_start_time = time.perf_counter()
        if torch.cuda.is_available():
            torch.cuda.reset_peak_memory_stats(device=device)

        train_data_time = 0.0
        train_compute_time = 0.0
        val_data_time = 0.0
        val_compute_time = 0.0
        
        # Training
        model.train()
        epoch_loss = []
        
        for batch_x, batch_y in train_loader:
            data_start = time.perf_counter()
            batch_x = batch_x.to(device)
            batch_y = batch_y.to(device)
            train_data_time += time.perf_counter() - data_start
            
            optimizer.zero_grad()
            maybe_cuda_sync()
            compute_start = time.perf_counter()
            
            y_pred = model(batch_x)
            
            # Loss calculation matching broadband_denoise_new.py
            time_loss = criterion(y_pred, batch_y)
            
            # # Frequency loss
            # # Windowing
            # # Note: signal.get_window returns numpy, need to handle batch size
            # # In user code: window = signal.get_window('hann', X_train.shape[1])
            # # X_train.shape[1] is seq_len (240).
            
            # # Create window once per batch (or cached)
            # # Efficient way is to create it outside loop, but let's follow logic
            # window_np = signal.get_window('hann', batch_x.shape[1])
            # window_np = window_np / np.sqrt(np.mean(window_np**2))
            # window = torch.tensor(window_np, dtype=torch.float32, device=device)
            # # window needs to broadcast. batch_x is (B, 240, 1). Window is (240,)
            # # window should be (1, 240, 1) or similar.
            # window = window.view(1, -1, 1)
            
            # windowed_noise = window * batch_y
            # windowed_pred = window * y_pred
            
            # # FFT (dim=1 is time dimension)
            # pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
            # noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
            
            # magnitude_pred = torch.abs(pred_fft)
            # log_pred = 20 * torch.log10(magnitude_pred + 1e-10)
            
            # magnitude_noise = torch.abs(noise_fft)
            # log_noise = 20 * torch.log10(magnitude_noise + 1e-10)
            
            # freq_loss = torch.mean(torch.abs(log_pred - log_noise))
            
            # loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
            loss = time_loss
            
            loss.backward()
            optimizer.step()

            maybe_cuda_sync()
            train_compute_time += time.perf_counter() - compute_start
            
            epoch_loss.append(loss.item())
            
        avg_epoch_loss = sum(epoch_loss) / len(epoch_loss)
        losses.append(avg_epoch_loss)
        maybe_cuda_sync()
        train_end_time = time.perf_counter()
        
        # Validation
        model.eval()
        epoch_val_loss = []
        with torch.no_grad():
            for batch_x, batch_y in val_loader:
                data_start = time.perf_counter()
                batch_x = batch_x.to(device)
                batch_y = batch_y.to(device)
                val_data_time += time.perf_counter() - data_start
                maybe_cuda_sync()
                compute_start = time.perf_counter()
                
                y_pred = model(batch_x)
                time_loss = criterion(y_pred, batch_y)
                
                # window_np = signal.get_window('hann', batch_x.shape[1])
                # window_np = window_np / np.sqrt(np.mean(window_np**2))
                # window = torch.tensor(window_np, dtype=torch.float32, device=device).view(1, -1, 1)
                
                # windowed_noise = window * batch_y
                # windowed_pred = window * y_pred
                
                # pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
                # noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
                
                # log_pred = 20 * torch.log10(torch.abs(pred_fft) + 1e-10)
                # log_noise = 20 * torch.log10(torch.abs(noise_fft) + 1e-10)
                
                # freq_loss = torch.mean(torch.abs(log_pred - log_noise))
                
                # val_loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
                val_loss = time_loss
                maybe_cuda_sync()
                val_compute_time += time.perf_counter() - compute_start
                epoch_val_loss.append(val_loss.item())
        
        avg_val_loss = sum(epoch_val_loss) / len(epoch_val_loss)
        val_losses.append(avg_val_loss)

        maybe_cuda_sync()
        epoch_end_time = time.perf_counter()
        train_time = train_end_time - epoch_start_time
        epoch_time = epoch_end_time - epoch_start_time
        val_time = epoch_end_time - train_end_time
        peak_memory_mb = get_peak_memory_mb()
        peak_memory_text = f"{peak_memory_mb:.2f} MB" if peak_memory_mb is not None else "N/A (CPU)"
        
        print(
            f"Epoch {epoch+1}/{NUM_EPOCHS}, "
            f"Train Time: {train_time:.2f}s, Val Time: {val_time:.2f}s, Epoch Time: {epoch_time:.2f}s, "
            f"Train(Data/Compute): {train_data_time:.2f}/{train_compute_time:.2f}s, "
            f"Val(Data/Compute): {val_data_time:.2f}/{val_compute_time:.2f}s, "
            f"Peak Mem: {peak_memory_text}, "
            f"Loss: {avg_epoch_loss:.6f}, Val Loss: {avg_val_loss:.6f}"
        )
        
        # Early Stopping
        if (epoch + 1) % 10 == 0 or epoch == 0: # Check periodically or every epoch? User code says % 10.
            # But wait, logic in user code:
            # if i % 10 == 0: patience += 10; if val < min: ...
            # Let's clean up logic
            pass
        
        # Checking early stopping every epoch is better but I will stick to user pattern roughly
        # User code:
        # if i % 10 == 0:
        #     patience += 10
        #     if val_loss.item() < min_val_loss: ...
        # My implementation: check every epoch for 'min_val_loss' update, but check 'patience' logic.
        
        if avg_val_loss < min_val_loss:
            min_val_loss = avg_val_loss
            patience = 0
            # Save best model? User saves at end. Better to save best here? 
            # User code: "update min validation loss". But it doesn't seem to save intermediate?
            # It saves at very end. 
            # I will save best model just in case.
            torch.save(model.state_dict(), os.path.join(save_model_dir, 'best_model_0303.pth'))
            print("  New best validation loss, model saved.")
        else:
            patience += 1
            
        if patience >= patience_limit:
            print("Early stopping...")
            break
            
    # Save final model
    final_model_path = os.path.join(save_model_dir, 'denoise_model_0414.pth')
    torch.save(model.state_dict(), final_model_path)
    print(f"Final model saved to {final_model_path}")
    
    # Save losses
    np.savetxt(os.path.join(results_dir, 'loss_0414.txt'), losses)
    np.savetxt(os.path.join(results_dir, 'val_loss_0414.txt'), val_losses)

    # Plot loss curves (0202 vs 0303) in dB
    plot_loss_comparison_db(results_dir)
    
    # Export ONNX
    dummy_input = torch.randn(1, 240, 1, device=device)
    onnx_path = os.path.join(save_model_dir, 'denoise_model_0414.onnx')
    torch.onnx.export(model, dummy_input, onnx_path, 
                      input_names=['input'], output_names=['output'], 
                      dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}})
    print(f"ONNX model saved to {onnx_path}")

if __name__ == "__main__":
    train()
