import numpy as np
from numpy import linalg as la
import os
import pickle
import copy
import matplotlib.pylab as plt

max_len = 1980
align_len = 1980
test_len = 2000
def safe_float_convert(x):
    try:
        return float(x)
    except ValueError as e:
        print(f"Error converting '{x}' to float: {e}")
        return None

def l2_normalize(X_train):
    for i in range(X_train.shape[0]):
        X_train[i, :, 0] = X_train[i, :, 0] / la.norm(X_train[i, :, 0], 2)
        X_train[i, :, 1] = X_train[i, :, 1] / la.norm(X_train[i, :, 1], 2)

    return X_train

def align_wave(wave, label_wave):
    aligned_wave = []
    aligned_label_wave = []
    max_delay = 0
    # 798
    # 1980
    for i in range(wave.shape[0]):
        cross_correlation = np.correlate(wave[i, :, 0], label_wave[i, :, 0], mode='full')
        delay = np.argmax(cross_correlation) - wave.shape[1] + 1
        if np.abs(delay) > max_delay:
            max_delay = np.abs(delay)
        if delay >= 0:
            aligned_wave.append(wave[i, delay:delay+align_len, :])
            aligned_label_wave.append(label_wave[i, :align_len, :])
        else:
            aligned_wave.append(wave[i, :align_len, :])
            aligned_label_wave.append(label_wave[i, -delay:align_len-delay, :])
    wave = np.array(aligned_wave)
    label_wave = np.array(aligned_label_wave)
    return wave, label_wave


def read_data(file_path, num_values):
    data = []
    with open(file_path, 'r') as file:
        for _ in range(num_values):
            line = file.readline()
            if not line:
                break
            value = safe_float_convert(line.strip())
            if value is not None:
                data.append(value)
    return data

def make_dataset_from_txt():
    mode_data = {}
    current_dir = os.getcwd()
    modes = ["BPSK", "QPSK", "16QAM", "64QAM"]
    BPSK_files = os.listdir(r'./dataset/BPSK')
    QPSK_files = os.listdir(r'./dataset/QPSK')
    QAM16_files = os.listdir(r'./dataset/16QAM')
    QAM64_files = os.listdir(r'./dataset/64QAM')

    files_set = [BPSK_files, QPSK_files, QAM16_files, QAM64_files]
    data = np.zeros((4, 5, 1000, 1280, 2))

    SNRs = [0, 10, 20, 100]
    length = 1280

    for i in range(len(files_set)):
        os.chdir(current_dir + r"/dataset/" + modes[i])
        print("Current directory: " + current_dir + r"/dataset/" + modes[i])
        for file in files_set[i]:
            number = int(file.split('_')[0][4:])
            IQ = file.split('_')[1]
            snr = int(file.split('_')[2][5:-4])
            for j in range(len(SNRs)):
                if snr == SNRs[j]: # snr in the file name
                    if IQ == "I":
                        data[i, j, number - 1, :, 0] = np.array(read_data(file, length)) # index[mod, snr, number, length, IQ]
                    elif IQ == "Q":
                        data[i, j, number - 1 , :, 1] = np.array(read_data(file, length))
                mode_data[(modes[i], SNRs[j])] = data[i, j, :, :, :]
        mode_data[(modes[i], 'AWGN')] = data[i, 3, : , :, :] + 1 * np.max(data[i, 3, : , :, :]) * np.random.rand() * np.random.randn(1000, 1280, 2)

    os.chdir(current_dir)
    file_to_save = open("Denoise_dataset.pkl", "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")


def make_dataset_from_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/baseline")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    mode_data = {}
    for file in files:
        if file.endswith(".bin"):
            mode = file.split("_")[0]
            freq = file.split("_")[1]
            bias = file.split("_")[2][:-4]
        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.fromfile(file, dtype=np.int16)

            mode_data[(mode, freq, bias)] = data

    file_to_save = open("Real_baseline_dataset.pkl", "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def make_dataset_from_sim_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/simdata")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    mode_data = {}
    for file in files:
        if file.endswith(".bin") and file.startswith("16QAM") and file.split("_")[3] == 'noise':
            mode = file.split("_")[0]
            freq_low = file.split("_")[1]
            freq_high = file.split("_")[2]

        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.fromfile(file, dtype=np.double)

            if (mode, freq_low, freq_high) in mode_data:
                mode_data[(mode, freq_low, freq_high)] = np.hstack((mode_data[(mode, freq_low, freq_high)], data))
            else:
                mode_data[(mode, freq_low, freq_high)] = data

    file_to_save = open("Sim_{}_{}_{}_dataset_noise.pkl".format(mode, freq_low, freq_high), "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def make_dataset_from_floor_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/simdata")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    mode_data = {}
    for file in files:
        if file.endswith(".bin") and file.startswith("Noise"):
            mode = file.split("_")[0]
        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.fromfile(file, dtype=np.double)

            if mode in mode_data:
                mode_data[mode] = np.hstack((mode_data[mode], data))
            else:
                mode_data[mode] = data

    file_to_save = open("Floor_dataset.pkl", "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def make_dataset_from_Tek_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/Tek_data")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    mode_data = {}
    for file in files:
        if file.endswith(".bin") and len(file.split("_")) == 5:
            mode = file.split("_")[1]
            freq = file.split("_")[2]
            bias = file.split("_")[3]
        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.fromfile(file, dtype=np.int16)

            if (mode, freq, bias) in mode_data:
                mode_data[(mode, freq, bias)] = np.hstack((mode_data[(mode, freq, bias)], data))
            else:
                mode_data[(mode, freq, bias)] = data

    file_to_save = open("Tek_baseline_dataset.pkl", "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def frequency_transfer_Tek_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/Tek_data")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    for file in files:
        if file.endswith(".bin") and len(file.split("_")) == 5:
            mode = file.split("_")[1]
            freq = file.split("_")[2]
            bias = file.split("_")[3]
            num = file.split("_")[4]
            data = np.fromfile(file, dtype=np.int16)

            if freq == '1M':
                transfer_freq = 8000
                length = data.shape[0] // transfer_freq * transfer_freq
                data = data[:length].reshape((-1, transfer_freq)).T.flatten()
                # plt.plot(data[:1000])
                # plt.show()
                data.tofile("LongStorageWfm_{}_8000M_{}_{}".format(mode, bias, num))
    print("data processing successful")

def make_dataset_from_TCP_bin():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/TCP_data")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)

    mode_data = {}
    for file in files:
        if file.endswith(".bin") and len(file.split("_")) == 5:
            data_type = file.split("_")[1]
            data_length = file.split("_")[2]
            date = file.split("_")[3]
            data_id = file.split("_")[4][:-4]
        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.fromfile(file, dtype=np.int16)

            if (data_type, data_id) in mode_data:
                mode_data[(data_type, data_id)] = np.hstack((mode_data[(data_type, data_id)], data))
            else:
                mode_data[(data_type, data_id)] = data

    file_to_save = open("TCP_dataset_for_{}.pkl".format(data_id), "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def normalize(data, mean = 2048, std = 1200):
    data = (data - mean) / std
    return data

def load_baseline_denoise(filename):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    mode = '直流'
    freq = '0MHz'
    bias = ['0mV', '10mV', '-10mV']

    noise = []
    real = []
    for i in range(len(bias)):
        noise.append(Xd[(mode, freq, bias[i])])
        real.append(np.zeros_like(Xd[(mode, freq, bias[i])]) + Xd[(mode, freq, bias[i])].mean())
    noise = np.array(noise)
    real = np.array(real)

    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]
    real = real[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))
    real = np.reshape(real, (-1, max_len, 1))
    noise = normalize(noise)
    real = normalize(real)

    X.append(noise)
    Y.append(real)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)


def load_transfer_denoise(filename, mode, freq, bias):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    noise = []

    noise.append(Xd[(mode, freq, bias)])
    noise = np.array(noise)
    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))
    noise = normalize(noise)

    # noise = np.random.permutation(noise)
    # real = np.random.permutation(noise)

    # noise, real = align_wave(noise, real)  # 通道噪声

    X.append(noise)
    Y.append(noise)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)

def load_transfer_denoise_test(filename, mode, freq, bias):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    noise = []

    noise.append(Xd[(mode, freq, bias)])
    noise = np.array(noise)
    total_length = test_len * (noise.shape[1] // test_len)
    noise = noise[:, :total_length]

    noise = np.reshape(noise, (-1, test_len, 1))
    noise = normalize(noise)
    real = noise

    # noise = np.random.permutation(noise)
    # real = np.random.permutation(noise)

    X.append(noise)
    Y.append(real)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)


np.random.seed(88)

mode_data = {}
maxlen = 1280

def load_data_denoise(filename):
    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    mods, snrs = [list(set([k[j] for k in Xd.keys()])) for j in [0, 1]]
    X = []
    Y = []
    train_idx=[]
    val_idx=[]
    sum_samples=0

    for mod in mods:
        n_samples = Xd[(mod, 0)].shape[0]
        X.append(Xd[(mod, 'AWGN')])
        Y.append(Xd[(mod, 100)])
        train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
        val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
        sum_samples += n_samples
    X = np.vstack(X)
    Y = np.vstack(Y)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    # X_train,X_val,X_test = to_amp_phase(X_train,X_val,X_test,100)

    X_train = X_train[:,:maxlen,:]
    X_val = X_val[:,:maxlen,:]
    X_test = X_test[:,:maxlen,:]
    Y_train = Y_train[:,:maxlen]
    Y_val = Y_val[:,:maxlen]
    Y_test = Y_test[:,:maxlen]

    # X_train = l2_normalize(X_train)
    # X_val = l2_normalize(X_val)
    # X_test = l2_normalize(X_test)
    # Y_train = l2_normalize(Y_train)
    # Y_val = l2_normalize(Y_val)
    # Y_test = l2_normalize(Y_test)

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train),(X_val,Y_val),(X_test,Y_test)


def load_broadband_denoise(filename):
    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx=[]
    val_idx=[]
    sum_samples=0

    n_samples = Xd.shape[0]
    X.append(Xd[:, 0, :]) # 0 ??????2??????
    Y.append(Xd[:, 1, :])

    train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
    val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
    sum_samples += n_samples
    X = np.vstack(X)
    Y = np.vstack(Y)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train),(X_val,Y_val),(X_test,Y_test)


def load_data_classification(filename):
    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    mods, snrs = [list(set([k[j] for k in Xd.keys()])) for j in [0, 1]]
    X = []
    Y = []
    lbl = []
    train_idx=[]
    val_idx=[]
    sum_samples=0

    for mod in mods:
        n_samples = Xd[(mod, 'AWGN')].shape[0]
        X.append(Xd[(mod, 'AWGN')])
        Y.append(Xd[(mod, 100)])
        for i in range(Xd[(mod, 'AWGN')].shape[0]):
            lbl.append((mod))
        train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
        val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
        sum_samples += n_samples
    X = np.vstack(X)
    Y = np.vstack(Y)
    lbl = np.vstack(lbl)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    lbl_train = list(map(lambda x: mods.index(lbl[x]), train_idx))
    lbl_val = list(map(lambda x: mods.index(lbl[x]), val_idx))
    lbl_test = list(map(lambda x: mods.index(lbl[x]), test_idx))

    # X_train,X_val,X_test = to_amp_phase(X_train,X_val,X_test,100)

    X_train = X_train[:,:maxlen,:]
    X_val = X_val[:,:maxlen,:]
    X_test = X_test[:,:maxlen,:]
    Y_train = Y_train[:,:maxlen,:]
    Y_val = Y_val[:,:maxlen,:]
    Y_test = Y_test[:,:maxlen,:]

    # X_train = l2_normalize(X_train)
    # X_val = l2_normalize(X_val)
    # X_test = l2_normalize(X_test)
    # Y_train = l2_normalize(Y_train)
    # Y_val = l2_normalize(Y_val)
    # Y_test = l2_normalize(Y_test)

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train,lbl_train),(X_val,Y_val,lbl_val),(X_test,Y_test,lbl_test)

def make_dataset_from_txt():
    current_dir = os.getcwd()
    os.chdir(current_dir + r"/dataset/rf_modulated_signals_with_snr")
    current_dir = os.getcwd()
    files = os.listdir(current_dir)
    length = 800000

    mode_data = {}
    for file in files:
        if file.endswith(".txt"):
            fc = file.split("_")[0][2:3]
            rs = file.split("_")[1][2:-1]
            a = file.split("_")[2][1:]
            mode = file.split("_")[3]
            snr = file.split("_")[4][3:-6]
        # with open(file, 'rb') as f:
        #     data = f.read()
            data = np.float32(np.loadtxt(file))
            data = data.reshape(-1, 2000, 1)
            mode_data[(mode, snr)] = data

    file_to_save = open("rf_modulated_signals_with_snr.pkl", "wb")
    pickle.dump(mode_data, file_to_save, -1)
    file_to_save.close()
    print("data processing successful")

def load_rf_data(filename):
    mode = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM']

    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    mods = [k for k in Xd.keys()]
    X = []
    Y = []
    lbl = []
    train_idx=[]
    val_idx=[]
    sum_samples=0
    for mod in mods:
        if mod[1] == '10':
            print("processing mod" + str(mod))
            n_samples = Xd[(mod)].shape[0]
            X.append(Xd[(mod)])     #ndarray(6000,2,128)
            Y.append(Xd[(mod[0], '100')])
            # for i in range(n_samples):
            #     label = [mode.index(mod[0])] + list(mod[1:])
            #     lbl.append(label)
            train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
            val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
            sum_samples += n_samples

    X = np.vstack(X)
    Y = np.vstack(Y)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    # lbl = np.array(lbl, dtype=float)

    # Y_train = np.array(list(map(lambda x: lbl[x], train_idx)))
    # Y_val= np.array(list(map(lambda x: lbl[x], val_idx)))
    # Y_test = np.array(list(map(lambda x: lbl[x], test_idx)))

    # X_train = X_train[:,:maxlen,:]
    # X_val = X_val[:,:maxlen,:]
    # X_test = X_test[:,:maxlen,:]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train),(X_val,Y_val),(X_test,Y_test)

def load_rf_data_unsupervised(filename):
    mode = ['BPSK', 'QPSK', '8PSK', '16QAM', '64QAM']

    Xd =pickle.load(open(filename,'rb')) #Xd(120W,2,128) 10calss*20SNR*6000samples
    mods = [k for k in Xd.keys()]
    X = []
    Y = []
    lbl = []
    train_idx=[]
    val_idx=[]
    sum_samples=0
    for mod in mods:
        if mod[1] == '10':
            print("processing mod" + str(mod))
            n_samples = Xd[(mod)].shape[0]
            X.append(Xd[(mod)])     #ndarray(6000,2,128)
            Y.append(Xd[(mod)])
            # for i in range(n_samples):
            #     label = [mode.index(mod[0])] + list(mod[1:])
            #     lbl.append(label)
            train_idx+=list(np.random.choice(range(sum_samples, sum_samples+n_samples), size=int(n_samples*0.8), replace=False))
            val_idx+=list(np.random.choice(list(set(range(sum_samples, sum_samples+n_samples))-set(train_idx)), size=int(n_samples*0.1), replace=False))
            sum_samples += n_samples

    X = np.vstack(X)
    Y = np.vstack(Y)
    n_examples=X.shape[0]
    test_idx = list(set(range(0,n_examples))-set(train_idx)-set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    # lbl = np.array(lbl, dtype=float)

    # Y_train = np.array(list(map(lambda x: lbl[x], train_idx)))
    # Y_val= np.array(list(map(lambda x: lbl[x], val_idx)))
    # Y_test = np.array(list(map(lambda x: lbl[x], test_idx)))

    # X_train = X_train[:,:maxlen,:]
    # X_val = X_val[:,:maxlen,:]
    # X_test = X_test[:,:maxlen,:]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train,Y_train),(X_val,Y_val),(X_test,Y_test)

def load_floor_denoise(filename):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    mode = 'Noise'

    noise = []
    real = []
    noise.append(Xd[(mode)])
    real.append(np.zeros_like(Xd[(mode)]) + Xd[(mode)].mean())
    noise = np.array(noise)
    real = np.array(real)

    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]
    real = real[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))
    real = np.reshape(real, (-1, max_len, 1))
    # noise = normalize(noise)
    # real = normalize(real)

    X.append(noise)
    Y.append(real)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)

def load_sim_denoise(filename, mode, freq_low, freq_high):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    noise = []

    noise.append(Xd[(mode, freq_low, freq_high)])
    noise = np.array(noise)
    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))
    # noise = normalize(noise)

    # noise = np.random.permutation(noise)
    # real = np.random.permutation(noise)

    # noise, real = align_wave(noise, real)  # 通道噪声

    X.append(noise)
    Y.append(noise)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)

def load_sim_denoise_test(filename_noise, filename_ideal, mode, freq_low, freq_high):
    Xd_noise = pickle.load(open(filename_noise, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    Xd_ideal = pickle.load(open(filename_ideal, 'rb'))
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    noise = []
    ideal = []

    noise.append(Xd_noise[(mode, freq_low, freq_high)])
    ideal.append(Xd_ideal[(mode, freq_low, freq_high)])

    noise = np.array(noise)
    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))

    ideal = np.array(ideal)
    ideal = ideal[:, :total_length]

    ideal = np.reshape(ideal, (-1, max_len, 1))
    # noise = normalize(noise)

    # noise = np.random.permutation(noise)
    # real = np.random.permutation(noise)

    # noise, real = align_wave(noise, real)  # 通道噪声

    X.append(noise)
    Y.append(ideal)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)

def load_sin_denoise(filename, mode, freq):
    Xd = pickle.load(open(filename, 'rb'))  # Xd(120W,2,128) 10calss*20SNR*6000samples
    # mods,snrs = [sorted(list(set([k[j] for k in Xd.keys()]))) for j in [0,1]]
    X = []
    Y = []
    train_idx = []
    val_idx = []
    sum_samples = 0

    noise = []

    noise.append(Xd[(mode, freq)])
    noise = np.array(noise)
    total_length = max_len * (noise.shape[1] // max_len)
    noise = noise[:, :total_length]

    noise = np.reshape(noise, (-1, max_len, 1))
    # noise = normalize(noise)

    # noise = np.random.permutation(noise)
    # real = np.random.permutation(noise)

    # noise, real = align_wave(noise, real)  # 通道噪声

    X.append(noise)
    Y.append(noise)

    n_samples = noise.shape[0]

    train_idx += list(
        np.random.choice(range(n_samples), size=int(n_samples * 0.8), replace=False))
    val_idx += list(np.random.choice(list(set(range(n_samples)) - set(train_idx)),
                                     size=int(n_samples * 0.1), replace=False))
    X = np.vstack(X)
    Y = np.vstack(Y)
    test_idx = list(set(range(n_samples)) - set(train_idx) - set(val_idx))
    np.random.shuffle(train_idx)
    np.random.shuffle(val_idx)
    np.random.shuffle(test_idx)
    X_train = X[train_idx]
    X_val = X[val_idx]
    X_test = X[test_idx]
    Y_train = Y[train_idx]
    Y_val = Y[val_idx]
    Y_test = Y[test_idx]

    print(X_train.shape)
    print(X_val.shape)
    print(X_test.shape)

    return (X_train, Y_train), (X_val, Y_val), (X_test, Y_test)


if __name__ == '__main__':
    # frequency_transfer_Tek_bin()
    # make_dataset_from_Tek_bin()
    # make_dataset_from_sim_bin()
    # make_dataset_from_floor_bin()
    # make_dataset_from_txt()
    # load_rf_data("rf_modulated_signals_with_snr.pkl")
    make_dataset_from_TCP_bin()
    # load_data_classification("Denoise_dataset.pkl")
    # load_data_denoise("Denoise_dataset.pkl")
    # load_broadband_denoise(r"./dataset/broadband_dataset.pkl")
    # (X_train, Y_train), (X_val, Y_val), (X_test, Y_test) = load_data()
