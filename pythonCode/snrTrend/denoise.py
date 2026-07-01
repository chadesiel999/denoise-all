
import torch
import torch.nn as nn
import numpy as np
from scipy import signal
import time
import pickle
import os
import sys
from torch.utils.data import Dataset, DataLoader

# Add boardband to path to import models
current_dir = os.path.dirname(os.path.abspath(__file__))
boardband_dir = os.path.abspath(os.path.join(current_dir, '../boardband'))
sys.path.append(boardband_dir)

from models import GRUCNNModel

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"Using device: {device}")

# Parameters
BATCH_SIZE = 512 # Adjust based on GPU memory
NUM_EPOCHS = 1500
LEARNING_RATE = 1e-4
HIDDEN_SIZE = 32
FREQ_WEIGHT = 0.5 # Based solely on user's previous code it was variable, but usually 0.5 or similar. 
# Looking at provided broadband_denoise_new.py: freq_weight = 0 means only time loss? 
# Wait, previous code says: freq_weight = 0 #1e-3. 
# But then: loss = (1-freq_weight)*time_loss + freq_weight * freq_loss
# So if freq_weight = 0, it uses only time_loss.
# In broadband_denoise_new.py: freq_weight = 0.
# I will stick to 0 unless I see reason otherwise.
FREQ_WEIGHT = 0

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
    
    train_loader = DataLoader(dataset_train, batch_size=BATCH_SIZE, shuffle=True, num_workers=0)
    # Val batch size: all of it? user code: batch_size=int(val_len)
    val_loader = DataLoader(dataset_val, batch_size=BATCH_SIZE, shuffle=False, num_workers=0)

    # Model
    model = GRUCNNModel(input_size=1, hidden_size=HIDDEN_SIZE, output_size=1)
    model = model.to(device)
    
    optimizer = torch.optim.Adam(model.parameters(), lr=LEARNING_RATE, betas=(0.9, 0.999), eps=1e-8, amsgrad=False)
    criterion = nn.MSELoss()
    
    losses = []
    val_losses = []
    min_val_loss = np.inf
    patience = 0
    patience_limit = 50
    
    print("Starting training...")
    
    for epoch in range(NUM_EPOCHS):
        start_time = time.time()
        
        # Training
        model.train()
        epoch_loss = []
        
        for batch_x, batch_y in train_loader:
            batch_x = batch_x.to(device)
            batch_y = batch_y.to(device)
            
            optimizer.zero_grad()
            
            y_pred = model(batch_x)
            
            # Loss calculation matching broadband_denoise_new.py
            time_loss = criterion(y_pred, batch_y)
            
            # Frequency loss
            # Windowing
            # Note: signal.get_window returns numpy, need to handle batch size
            # In user code: window = signal.get_window('hann', X_train.shape[1])
            # X_train.shape[1] is seq_len (240).
            
            # Create window once per batch (or cached)
            # Efficient way is to create it outside loop, but let's follow logic
            window_np = signal.get_window('hann', batch_x.shape[1])
            window_np = window_np / np.sqrt(np.mean(window_np**2))
            window = torch.tensor(window_np, dtype=torch.float32, device=device)
            # window needs to broadcast. batch_x is (B, 240, 1). Window is (240,)
            # window should be (1, 240, 1) or similar.
            window = window.view(1, -1, 1)
            
            windowed_noise = window * batch_y
            windowed_pred = window * y_pred
            
            # FFT (dim=1 is time dimension)
            pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
            noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
            
            magnitude_pred = torch.abs(pred_fft)
            log_pred = 20 * torch.log10(magnitude_pred + 1e-10)
            
            magnitude_noise = torch.abs(noise_fft)
            log_noise = 20 * torch.log10(magnitude_noise + 1e-10)
            
            freq_loss = torch.mean(torch.abs(log_pred - log_noise))
            
            loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
            
            loss.backward()
            optimizer.step()
            
            epoch_loss.append(loss.item())
            
        avg_epoch_loss = sum(epoch_loss) / len(epoch_loss)
        losses.append(avg_epoch_loss)
        end_time = time.time()
        
        # Validation
        model.eval()
        epoch_val_loss = []
        with torch.no_grad():
            for batch_x, batch_y in val_loader:
                batch_x = batch_x.to(device)
                batch_y = batch_y.to(device)
                
                y_pred = model(batch_x)
                time_loss = criterion(y_pred, batch_y)
                
                window_np = signal.get_window('hann', batch_x.shape[1])
                window_np = window_np / np.sqrt(np.mean(window_np**2))
                window = torch.tensor(window_np, dtype=torch.float32, device=device).view(1, -1, 1)
                
                windowed_noise = window * batch_y
                windowed_pred = window * y_pred
                
                pred_fft = torch.abs(torch.fft.rfft(windowed_pred, dim=1))
                noise_fft = torch.abs(torch.fft.rfft(windowed_noise, dim=1))
                
                log_pred = 20 * torch.log10(torch.abs(pred_fft) + 1e-10)
                log_noise = 20 * torch.log10(torch.abs(noise_fft) + 1e-10)
                
                freq_loss = torch.mean(torch.abs(log_pred - log_noise))
                
                val_loss = (1 - FREQ_WEIGHT) * time_loss + FREQ_WEIGHT * freq_loss
                epoch_val_loss.append(val_loss.item())
        
        avg_val_loss = sum(epoch_val_loss) / len(epoch_val_loss)
        val_losses.append(avg_val_loss)
        
        print(f"Epoch {epoch+1}/{NUM_EPOCHS}, Time: {end_time - start_time:.2f}s, Loss: {avg_epoch_loss:.6f}, Val Loss: {avg_val_loss:.6f}")
        
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
            torch.save(model.state_dict(), os.path.join(save_model_dir, 'best_model_0202.pth'))
            print("  New best validation loss, model saved.")
        else:
            patience += 1
            
        if patience >= patience_limit:
            print("Early stopping...")
            break
            
    # Save final model
    final_model_path = os.path.join(save_model_dir, 'denoise_model_0202.pth')
    torch.save(model.state_dict(), final_model_path)
    print(f"Final model saved to {final_model_path}")
    
    # Save losses
    np.savetxt(os.path.join(results_dir, 'loss_0202.txt'), losses)
    np.savetxt(os.path.join(results_dir, 'val_loss_0202.txt'), val_losses)
    
    # Export ONNX
    dummy_input = torch.randn(1, 240, 1, device=device)
    onnx_path = os.path.join(save_model_dir, 'denoise_model_0202.onnx')
    torch.onnx.export(model, dummy_input, onnx_path, 
                      input_names=['input'], output_names=['output'], 
                      dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}})
    print(f"ONNX model saved to {onnx_path}")

if __name__ == "__main__":
    train()
