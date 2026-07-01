import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
from torch.utils.data import TensorDataset, DataLoader, random_split
import numpy as np
import matplotlib.pyplot as plt
import os
import sys

# ==========================================
# 1. 配置参数 (Config)
# ==========================================
BATCH_SIZE = 64
LR = 1e-4
EPOCHS = 80
DEVICE = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

# 动态调整路径以支持导入 (如果需要导入同目录下的 dataset)
# 但是训练通常只读取 .pth 文件，不需要 dataset 脚本
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(current_dir)

# 路径配置
DATA_DIR = './createSimu/SineTest'
MODEL_SAVE_DIR = './createSimu/SineTest'
MODEL_PATH = os.path.join(MODEL_SAVE_DIR, 'best_denoise_model_sine_v3.pth')

if not os.path.exists(MODEL_SAVE_DIR):
    os.makedirs(MODEL_SAVE_DIR)

print(f"Using device: {DEVICE}")

# ==========================================
# 2. 网络结构定义 (GRU + CNN) - 保持与 v2 一致
# ==========================================
class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        # 1. 前置 CNN 特征提取
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        
        # 2. 双向 GRU 处理
        self.GRU_layer_1 = nn.GRU(
            input_size=hidden_size, 
            hidden_size=int(hidden_size / 2), 
            num_layers=1, 
            batch_first=True, 
            dropout=0.0, 
            bidirectional=True
        )
        self.GRU_layer_2 = nn.GRU(
            input_size=hidden_size, 
            hidden_size=int(hidden_size / 2), 
            num_layers=1, 
            batch_first=True, 
            dropout=0.0, 
            bidirectional=True
        )
        
        self.relu = nn.ReLU()
        
        # 3. 后置 CNN 恢复/输出
        self.conv2 = nn.Conv1d(hidden_size, output_size, kernel_size=3, padding=1)

    def forward(self, x):
        # x: (Batch, 1, Length)
        x = self.conv1(x)  # -> (Batch, Hidden, Length)
        
        # (B, C, L) -> (B, L, C)
        x = x.permute(0, 2, 1) 
        
        x, _ = self.GRU_layer_1(x)
        x = self.relu(x)
        x, _ = self.GRU_layer_2(x)
        
        # (B, L, C) -> (B, C, L)
        x = x.permute(0, 2, 1) 
        
        x = self.conv2(x) # -> (Batch, 1, Length)
        return x

# ==========================================
# 3. 数据加载与训练流程
# ==========================================
def load_data():
    print(f"Loading data from {DATA_DIR} ...")
    
    # Load Train V3
    train_path = os.path.join(DATA_DIR, 'train_dataset_v3.pth')
    if not os.path.exists(train_path):
        raise FileNotFoundError(f"Data file not found: {train_path}. Please run cre_sine_dataset_v3.py first.")
        
    X_train, Y_train = torch.load(train_path)
    
    # Load Test V3
    test_path = os.path.join(DATA_DIR, 'test_dataset_v3.pth')
    if not os.path.exists(test_path):
        raise FileNotFoundError(f"Data file not found: {test_path}. Please run cre_sine_dataset_v3.py first.")

    X_test, Y_test = torch.load(test_path)
    
    # 调整维度: (N, L, 1) -> (N, 1, L) for Conv1d
    X_train = X_train.permute(0, 2, 1)
    Y_train = Y_train.permute(0, 2, 1)
    X_test = X_test.permute(0, 2, 1)
    Y_test = Y_test.permute(0, 2, 1)
    
    print(f"Train Shape: {X_train.shape}, Test Shape: {X_test.shape}")
    
    full_train_ds = TensorDataset(X_train, Y_train)
    test_ds = TensorDataset(X_test, Y_test)
    
    # Split Train -> Train (90%) / Val (10%)
    train_size = int(0.9 * len(full_train_ds))
    val_size = len(full_train_ds) - train_size
    train_ds, val_ds = random_split(full_train_ds, [train_size, val_size])
    
    print(f"Splitting Train set: {train_size} for training, {val_size} for validation.")
    
    train_loader = DataLoader(train_ds, batch_size=BATCH_SIZE, shuffle=True,  num_workers=4, pin_memory=True)
    val_loader   = DataLoader(val_ds,   batch_size=BATCH_SIZE, shuffle=False, num_workers=4, pin_memory=True)
    test_loader  = DataLoader(test_ds,  batch_size=BATCH_SIZE, shuffle=False, num_workers=4, pin_memory=True)
    
    return train_loader, val_loader, test_loader

def train_model():
    train_loader, val_loader, test_loader = load_data()
    
    # 初始化模型
    model = GRUCNNModel(input_size=1, hidden_size=64, output_size=1).to(DEVICE)
    
    optimizer = optim.Adam(model.parameters(), lr=LR)
    criterion = nn.MSELoss()
    
    best_loss = float('inf')
    train_losses = []
    val_losses = []
    
    print(f"Start Training GRU-CNN (V3 - Sine) for {EPOCHS} epochs...")
    for epoch in range(EPOCHS):
        model.train()
        batch_losses = []
        
        for i, (inputs, targets) in enumerate(train_loader):
            inputs, targets = inputs.to(DEVICE), targets.to(DEVICE)
            
            optimizer.zero_grad()
            outputs = model(inputs)
            loss = criterion(outputs, targets)
            loss.backward()
            optimizer.step()
            
            batch_losses.append(loss.item())
            
        avg_train_loss = np.mean(batch_losses)
        train_losses.append(avg_train_loss)
        
        # Validation
        model.eval()
        val_batch_losses = []
        with torch.no_grad():
            for inputs, targets in val_loader:
                inputs, targets = inputs.to(DEVICE), targets.to(DEVICE)
                outputs = model(inputs)
                v_loss = criterion(outputs, targets)
                val_batch_losses.append(v_loss.item())
        
        avg_val_loss = np.mean(val_batch_losses)
        val_losses.append(avg_val_loss)
        
        print(f"Epoch {epoch+1:03d}/{EPOCHS} | Train Loss: {avg_train_loss:.8f} | Val Loss: {avg_val_loss:.8f}")
        
        # Save best
        if avg_val_loss < best_loss:
            best_loss = avg_val_loss
            torch.save(model.state_dict(), MODEL_PATH)
            print(f"  >>> Model Saved! (Best: {best_loss:.8f})")
            
    # Visualize Loss
    plt.figure()
    plt.plot(train_losses, label='Train MSE')
    plt.plot(val_losses, label='Val MSE')
    plt.xlabel('Epoch')
    plt.ylabel('Loss')
    plt.yscale('log')
    plt.legend()
    plt.title('Training & Validation Loss (V3 Sine Only)')
    plt.savefig(os.path.join(MODEL_SAVE_DIR, 'training_loss_curve_v3.png'))
    print(f"Training finished. Best Val Loss: {best_loss}")
    
    # Simple Eval Check
    evaluate_and_visualize(model, test_loader)

def evaluate_and_visualize(model, test_loader):
    print("Visualizing some results...")
    model.eval()
    
    # Load best weights
    model.load_state_dict(torch.load(MODEL_PATH, map_location=DEVICE))
    
    inputs, targets = next(iter(test_loader))
    inputs = inputs.to(DEVICE)
    
    with torch.no_grad():
        preds = model(inputs)
    
    inputs = inputs.cpu().numpy()
    targets = targets.cpu().numpy()
    preds = preds.cpu().numpy()
    
    # Plot first 3 samples
    for i in range(min(3, inputs.shape[0])):
        plt.figure(figsize=(10, 5))
        
        plt.subplot(2, 1, 1)
        plt.plot(inputs[i, 0], color='lightgray', label='Noisy Input')
        plt.plot(targets[i, 0], color='red', linestyle='--', label='Ground Truth')
        plt.title(f"Test Sample {i} - Input")
        plt.legend(loc='upper right')
        
        plt.subplot(2, 1, 2)
        plt.plot(preds[i, 0], color='blue', label='Denoised Output')
        plt.plot(targets[i, 0], color='red', linestyle='--', label='Truth', alpha=0.6)
        plt.title(f"Test Sample {i} - Output")
        plt.legend(loc='upper right')
        
        plt.tight_layout()
        plt.savefig(os.path.join(MODEL_SAVE_DIR, f'eval_result_v3_{i}.png'))
        plt.close()
    
    print(f"Evaluation images saved to {MODEL_SAVE_DIR}")

if __name__ == "__main__":
    train_model()
