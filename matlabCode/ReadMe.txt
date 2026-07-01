generate_scan_freq ：数据采集扫频文件生成。

calSNR_SINAD ： 画频谱。同时画snr和sinad。

snrTrenddataCut ： 对应vscode中snrTrend文件夹下面的数据预处理，前 80% 作为训练，后 20% 作为测试，可自行改ratio调整此比例。只负责切割。使用这个之前先去creat_idldata_mode将raw数据生成label标签sub2。

creat_idldata_mode ：负责标签生成。其中主要使用的部分是%% zhengxian (5ns 实时档 - 最终修正版 Fs=80G)。

verify7G9G1：上个代码标签生成后，该代码用于查看标签的效果，绘图。改代码中想看的频率值即可。

Verify7G9G：和上个绘图的区别是本代码将raw（sub1）和ideal（sub2）画同一张图上对比，上一个是分开画。