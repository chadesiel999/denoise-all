import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
from torch.utils.data import TensorDataset, DataLoader, random_split
import numpy as np
import matplotlib.pyplot as plt
import os

# ==========================================
# 1. 配置参数 (Config)
# ==========================================
BATCH_SIZE = 64
LR = 1e-4
EPOCHS = 80
DEVICE = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
# 注意：新数据集保存在 v2 目录，且文件名有 _v2 后缀
DATA_DIR = './generated_data_v2'
MODEL_SAVE_DIR = './generated_data_v2'
MODEL_PATH = os.path.join(MODEL_SAVE_DIR, 'best_denoise_model_grucnn_v2.pth')

if not os.path.exists(MODEL_SAVE_DIR):
    os.makedirs(MODEL_SAVE_DIR)

print(f"Using device: {DEVICE}")

# ==========================================
# 2. 网络结构定义 (GRU + CNN) - 保持一致
# ==========================================
class GRUCNNModel(nn.Module):
    def __init__(self, input_size, hidden_size, output_size):
        super(GRUCNNModel, self).__init__()
        # input_size: 输入通道数 (通常为1)
        # hidden_size: 隐层维度
        
        # 1. 前置 CNN 特征提取
        # 输入: (Batch, input_size, Length)
        self.conv1 = nn.Conv1d(input_size, hidden_size, kernel_size=3, padding=1)
        
        # 2. 双向 GRU 处理
        # GRU 输入需要 (Batch, Length, H_in)
        # Bidirectional=True, 所以 hidden_size 设为一半，拼接后恢复为 hidden_size
        self.GRU_layer_1 = nn.GRU(
            input_size=hidden_size, 
            hidden_size=int(hidden_size / 2), 
            num_layers=1, 
            batch_first=True, 
            dropout=0.0, 
            bidirectional=True
        )
        self.GRU_layer_2 = nn.GRU(
            input_size=hidden_size, # 上一层双向输出拼接后是 hidden_size
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
        
        # --- CNN ---
        x = self.conv1(x)  # -> (Batch, Hidden, Length)
        
        # --- Prepare for GRU ---
        # 维度转换: (B, C, L) -> (B, L, C)
        x = x.permute(0, 2, 1) # -> (Batch, Length, Hidden)
        
        # --- GRU Layers ---
        x, _ = self.GRU_layer_1(x)
        x = self.relu(x)
        x, _ = self.GRU_layer_2(x)
        
        # --- Prepare for Output CNN ---
        # 维度转换: (B, L, C) -> (B, C, L)
        x = x.permute(0, 2, 1) # -> (Batch, Hidden, Length)
        
        # --- Output ---
        x = self.conv2(x) # -> (Batch, 1, Length)
        
        return x

# ==========================================
# 3. 数据加载与训练流程
# ==========================================
def load_data():
    print("Loading data from generated_data_v2...")
    
    # Load Train V2
    train_path = os.path.join(DATA_DIR, 'train_dataset_v2.pth')
    if not os.path.exists(train_path):
        raise FileNotFoundError(f"Data file not found: {train_path}")
        
    X_train, Y_train = torch.load(train_path)
    
    # Load Test V2
    test_path = os.path.join(DATA_DIR, 'test_dataset_v2.pth')
    if not os.path.exists(test_path):
        raise FileNotFoundError(f"Data file not found: {test_path}")

    X_test, Y_test = torch.load(test_path)
    
    # 调整维度: 当前生成代码保存的维度是 (N, L, 1)
    # PyTorch Conv1d 需要 (Batch, Channel, Length) -> (N, 1, L)
    # 所以需要 permute(0, 2, 1)
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
    
    # num_workers=0 to avoid multiprocessing issues in some envs, adjust if needed
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
    
    print(f"Start Training GRU-CNN (V2) for {EPOCHS} epochs...")
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
    plt.title('Training & Validation Loss (V2 Dataset)')
    plt.savefig(os.path.join(MODEL_SAVE_DIR, 'training_loss_curve_v2.png'))
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
        plt.savefig(os.path.join(MODEL_SAVE_DIR, f'eval_result_v2_{i}.png'))
        plt.close()
    
    print(f"Evaluation images saved to {MODEL_SAVE_DIR}")

if __name__ == "__main__":
    train_model()
