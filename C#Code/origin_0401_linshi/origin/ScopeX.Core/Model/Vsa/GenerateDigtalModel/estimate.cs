using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace ScopeX.Core
{
    public struct CarrierEstimationResult
    {
        public double EstimatedFrequency;
        public int Iterations;
        public bool IsConverged;
        public double[] FrequencyHistory;
        public string Message;
    }

    public class CarrierEstimator
    {
        // --- 缓存成员变量 ---
        private double[] _window;
        private Complex[] _fftBuffer;     // FFT 计算缓冲
        private double[] _psdAccumulator; // PSD 累加器

        // 坐标轴数据 (只存单边谱：0 ~ Fs/2)
        private double[] _fAxis;
        private double[] _pAxis;

        private int _cachedNfft = -1;

        /// <summary>
        /// 针对示波器实数数据的载波频率估计
        /// </summary>
        /// <param name="realData">示波器采集的电压数据 (double)</param>
        /// <param name="fs">采样率</param>
        public CarrierEstimationResult Estimate(double[] realData, double fs, int maxIterations = 30, double ratioThreshold = 0.995)
        {
            // --- 1. 参数校验 ---
            if (realData == null || realData.Length < 128)
                return new CarrierEstimationResult { Message = "数据过短或为空" };

            // --- 2. 确定FFT点数 ---
            //int nFft = 4096;
            //if (realData.Length < nFft)
            //    nFft = (int)Math.Pow(2, Math.Floor(Math.Log(realData.Length, 2)));

            //// 初始化缓存 (如果点数变了)
            //if (nFft != _cachedNfft) InitializeBuffers(nFft);
            double targetWindowSize = realData.Length / 4.5;

            // 找到最接近且小于 targetWindowSize 的 2 的幂
            // 例如：数据 100k -> target 12.5k -> nFft 8192 或 16384
            int nFft = 4096; // 最小保底
            if (targetWindowSize > 4096)
            {
                nFft = (int)Math.Pow(2, Math.Floor(Math.Log(targetWindowSize, 2)));
            }

            // 限制最大 FFT 点数，防止内存溢出或计算太慢 (MathNet在大点数时依然很快，32k/64k通常没问题)
            // 如果你的数据非常长(如 10M点)，可能需要限制在 65536 或 131072
            if (nFft > 65536) nFft = 65536;

            // 如果数据实在太短，保持原逻辑
            if (realData.Length < nFft)
                nFft = (int)Math.Pow(2, Math.Floor(Math.Log(realData.Length, 2)));

            // 初始化缓存
            if (nFft != _cachedNfft) InitializeBuffers(nFft);

            // --- 3. 计算 Welch 单边功率谱 (One-Sided PSD) ---
            // 直接处理实数数据，生成 0 ~ Fs/2 的谱
            ComputeOneSidedWelchPsd(realData, nFft, fs);

            // --- 4. 迭代逻辑 ---
            double[] fc_history = new double[maxIterations];

            // 现在的频率范围是 0 (index 0) 到 Fs/2 (index nFft/2)
            // 所以我们只需要搜索前一半的 FFT 点数
            int oneSidedPoints = nFft / 2 + 1;

            int startIndex = 0;
            int endIndex = oneSidedPoints - 1;

            double fc_estimated = 0;
            double fc_prev = 0;
            int actualIterations = 0;
            bool converged = false;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                actualIterations = iter + 1;
                int currentLen = endIndex - startIndex + 1;

                if (currentLen < 5) break;

                // 4.1 统计 (Mean, Std)
                double sum = 0, sumSq = 0;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    double val = _pAxis[i];
                    sum += val;
                    sumSq += val * val;
                }
                double mean = sum / currentLen;
                double variance = (sumSq / currentLen) - (mean * mean);
                double stdDev = Math.Sqrt(Math.Max(0, variance));

                // 4.2 门限
                double threshold = mean + 5 * stdDev;

                // 4.3 重心法
                double numerator = 0;
                double denominator = 0;
                int validPoints = 0;

                // 第一次筛选
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (_pAxis[i] >= threshold)
                    {
                        numerator += _fAxis[i] * _pAxis[i];
                        denominator += _pAxis[i];
                        validPoints++;
                    }
                }

                // 回退机制
                if (validPoints < 3)
                {
                    threshold = mean + 1.0 * stdDev;
                    numerator = 0; denominator = 0; validPoints = 0;
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        if (_pAxis[i] >= threshold)
                        {
                            numerator += _fAxis[i] * _pAxis[i];
                            denominator += _pAxis[i];
                            validPoints++;
                        }
                    }
                }

                double fc_current;
                if (validPoints == 0 || denominator == 0)
                {
                    // 兜底：全段重心
                    double gN = 0, gD = 0;
                    for (int i = startIndex; i <= endIndex; i++) { gN += _fAxis[i] * _pAxis[i]; gD += _pAxis[i]; }
                    fc_current = (gD != 0) ? gN / gD : fc_prev;
                }
                else
                {
                    fc_current = numerator / denominator;
                }

                fc_history[iter] = fc_current;
                fc_estimated = fc_current;

                // 4.4 窗口截取 (针对单边谱的逻辑)
                double centerFreqCurrent = (_fAxis[startIndex] + _fAxis[endIndex]) / 2.0;
                double freqSpan = _fAxis[endIndex] - _fAxis[startIndex];
                double relativePos = (freqSpan > 1e-9) ? (fc_current - _fAxis[startIndex]) / freqSpan : 0.5;

                double retainRatio;
                if (fc_current <= centerFreqCurrent)
                    retainRatio = Math.Max(0.3, Math.Min(0.8, relativePos + 0.2));
                else
                    retainRatio = Math.Max(0.3, Math.Min(0.8, (1.0 - relativePos) + 0.2));

                int newPoints = (int)(currentLen * retainRatio);
                newPoints = Math.Max(50, Math.Min(currentLen - 2, newPoints));

                if (fc_current <= centerFreqCurrent)
                    endIndex = startIndex + newPoints - 1;
                else
                    startIndex = endIndex - newPoints + 1;

                // 4.5 收敛检查
                double lengthRatio = (double)newPoints / currentLen;
                if (iter > 2)
                {
                    double freqChangeRatio = (Math.Abs(fc_estimated) > 1e-9) ?
                        Math.Abs(fc_estimated - fc_prev) / Math.Abs(fc_estimated) : 0;

                    if (lengthRatio >= ratioThreshold && freqChangeRatio < 1e-6)
                    {
                        converged = true;
                        break;
                    }
                }
                fc_prev = fc_estimated;
            }

            return new CarrierEstimationResult
            {
                EstimatedFrequency = fc_estimated,
                Iterations = actualIterations,
                IsConverged = converged,
                FrequencyHistory = fc_history,
                Message = converged ? "Success" : "Max iterations reached"
            };
        }

        private void InitializeBuffers(int nFft)
        {
            _cachedNfft = nFft;
            _window = Window.Hamming(nFft);
            _fftBuffer = new Complex[nFft];

            // 对于单边谱，我们只需要存储 FFT 结果的一半 (0 ~ N/2)
            int oneSidedLen = nFft / 2 + 1;

            _psdAccumulator = new double[oneSidedLen];
            _fAxis = new double[oneSidedLen];
            _pAxis = new double[oneSidedLen];
        }

        /// <summary>
        /// 计算单边功率谱 (模仿 MATLAB pwelch 对实数输入的行为)
        /// 结果范围: 0 Hz 到 Fs/2
        /// </summary>
        //private void ComputeOneSidedWelchPsd(double[] realData, int nFft, double fs)
        //{
        //    int signalLen = realData.Length;
        //    int hopSize = nFft / 2;
        //    int numSegments = (signalLen - nFft) / hopSize + 1;
        //    if (numSegments < 1) numSegments = 1;

        //    // 清空累加器 (长度只有 nFft/2 + 1)
        //    Array.Clear(_psdAccumulator, 0, _psdAccumulator.Length);

        //    for (int seg = 0; seg < numSegments; seg++)
        //    {
        //        int startIdx = seg * hopSize;

        //        // 1. 填充 FFT buffer (实部为信号，虚部为0)
        //        for (int i = 0; i < nFft; i++)
        //        {
        //            if (startIdx + i < signalLen)
        //                _fftBuffer[i] = new Complex(realData[startIdx + i] * _window[i], 0);
        //            else
        //                _fftBuffer[i] = Complex.Zero;
        //        }

        //        // 2. 执行 FFT
        //        Fourier.Forward(_fftBuffer, FourierOptions.Matlab);

        //        // 3. 累加能量 (只累加前一半: 0 到 Nyquist)
        //        // index 0 是直流，index N/2 是 Nyquist
        //        int oneSidedLen = nFft / 2 + 1;
        //        for (int i = 0; i < oneSidedLen; i++)
        //        {
        //            double mag = _fftBuffer[i].Magnitude;

        //            // 单边谱校正：
        //            // 直流(i=0) 和 Nyquist(i=N/2) 保持不变
        //            // 其他频率(0 < i < N/2) 能量需要乘 2 (因为把负频率的能量加过来了)
        //            double factor = (i == 0 || i == oneSidedLen - 1) ? 1.0 : 2.0;

        //            _psdAccumulator[i] += (mag * mag) * factor;
        //        }
        //    }

        //    // 4. 平均并生成坐标轴
        //    int finalLen = _psdAccumulator.Length;
        //    double df = fs / nFft;
        //    double invSegs = 1.0 / numSegments;

        //    for (int i = 0; i < finalLen; i++)
        //    {
        //        _pAxis[i] = _psdAccumulator[i] * invSegs;
        //        _fAxis[i] = i * df; // 频率轴直接是 0, df, 2df ... Fs/2
        //    }
        //}
        private void ComputeOneSidedWelchPsd(double[] realData, int nFft, double fs)
        {
            int signalLen = realData.Length;
            // 50% Overlap
            int hopSize = nFft / 2;
            int numSegments = (signalLen - nFft) / hopSize + 1;
            if (numSegments < 1) numSegments = 1;

            // 计算窗口能量归一化系数 (Matlab pwelch 行为)
            double windowSumSq = 0;
            for (int i = 0; i < _window.Length; i++) windowSumSq += _window[i] * _window[i];

            Array.Clear(_psdAccumulator, 0, _psdAccumulator.Length);

            for (int seg = 0; seg < numSegments; seg++)
            {
                int startIdx = seg * hopSize;

                for (int i = 0; i < nFft; i++)
                {
                    // 边界保护，防止不足一段时越界
                    double val = (startIdx + i < signalLen) ? realData[startIdx + i] : 0;
                    _fftBuffer[i] = new Complex(val * _window[i], 0);
                }

                Fourier.Forward(_fftBuffer, FourierOptions.Matlab);

                int oneSidedLen = nFft / 2 + 1;
                for (int i = 0; i < oneSidedLen; i++)
                {
                    double mag = _fftBuffer[i].Magnitude;

                    // 单边谱能量补偿: 非DC且非Nyquist点能量乘2
                    double factor = (i == 0 || i == oneSidedLen - 1) ? 1.0 : 2.0;

                    // 累加 Magnitude Squared
                    _psdAccumulator[i] += (mag * mag) * factor;
                }
            }

            // 平均并生成坐标轴
            int finalLen = _psdAccumulator.Length;
            double df = fs / nFft;
            // 归一化因子: 1/段数 * 1/窗口能量和
            // 注意：这里没有除以 Fs，得到的是 Power Spectrum (V^2)，而不是 Density (V^2/Hz)
            // 只要 Matlab 端和 C# 端对齐即可。Matlab pwelch 默认除以了 Fs。
            // 但因为我们做比值门限，是否除以 Fs 不影响结果，为了数值稳定性，我们只做幅度平均。
            double scaleFactor = 1.0 / (numSegments * windowSumSq);

            for (int i = 0; i < finalLen; i++)
            {
                _pAxis[i] = _psdAccumulator[i] * scaleFactor;
                _fAxis[i] = i * df;
            }
        }
    }

    public class SymbolRateEstimator
    {
        private readonly int _fftLength;
        // MathNet 的 FFT 函数直接在 Complex[] 数组上原地操作
        private readonly Complex[] _fftBuffer;
        private readonly double[] _magnitudeDb;
        private readonly double[] _background;

        /// <summary>
        /// 初始化估算器
        /// </summary>
        /// <param name="fftLength">FFT点数，推荐 4096, 8192 等 (必须是2的幂)</param>
        public SymbolRateEstimator(int fftLength)
        {
            if ((fftLength & (fftLength - 1)) != 0)
                throw new ArgumentException("FFT Length must be a power of 2.");

            _fftLength = fftLength;

            // 预分配内存，避免 Estimate 循环中的 GC
            _fftBuffer = new Complex[fftLength];
            _magnitudeDb = new double[fftLength / 2];
            _background = new double[fftLength / 2];
        }

        /// <summary>
        /// 估计符号速率 (输入为 Complex[] 复基带信号)
        /// </summary>
        /// <param name="signal">复数信号 (I + jQ)</param>
        /// <param name="fs">采样率 (Hz)</param>
        /// <returns>估计的符号速率 (Hz)</returns>
        public double Estimate(Complex[] signal, double fs)
        {
            // ==========================================
            // 1. 预处理：非线性变换 |x|^2
            // ==========================================
            // O&M 算法核心：对于复数信号 s(n)，计算 p(n) = |s(n)|^2
            // p(n) 是实数序列，但在 MathNet 中为了做 FFT，我们将其存入 Complex 的实部，虚部设为 0

            int copyLength = Math.Min(signal.Length, _fftLength);

            // 循环展开或直接计算以提升性能
            for (int i = 0; i < copyLength; i++)
            {
                // 手动计算模平方：Re^2 + Im^2
                double magSq = signal[i].Real * signal[i].Real + signal[i].Imaginary * signal[i].Imaginary;

                // 存入 FFT Buffer
                _fftBuffer[i] = new Complex(magSq, 0);
            }

            // 补零：如果输入长度不足 FFT 长度，必须清空剩余部分
            // (这是复用 Buffer 必须做的，否则会残留上一次的数据)
            if (copyLength < _fftLength)
            {
                Array.Clear(_fftBuffer, copyLength, _fftLength - copyLength);
            }

            // ==========================================
            // 2. 执行 FFT (MathNet)
            // ==========================================
            // FourierOptions.Matlab 确保与 Matlab 行为一致
            Fourier.Forward(_fftBuffer, FourierOptions.Matlab);

            // ==========================================
            // 3. 计算对数幅度谱 (仅取正频率)
            // ==========================================
            // 结果的前半部分 (0 ~ N/2) 对应频率 0 ~ Fs/2
            int halfLen = _fftLength / 2;
            double maxMag = 1e-12; // 避免除零

            for (int i = 0; i < halfLen; i++)
            {
                // 获取幅度：FFT 结果是复数，取模
                double mag = _fftBuffer[i].Magnitude;

                if (mag > maxMag) maxMag = mag;
                _magnitudeDb[i] = mag;
            }

            // 转换为 dB
            // 预计算常数 20 / ln(10)
            const double logFactor = 8.685889638;

            for (int i = 0; i < halfLen; i++)
            {
                double val = _magnitudeDb[i] / maxMag;
                if (val < 1e-9) val = 1e-9; // 钳位底噪 (-180dB)
                _magnitudeDb[i] = Math.Log(val) * logFactor;
            }

            // ==========================================
            // 4. 背景噪声估计 (中值滤波)
            // ==========================================
            // 使用自定义的轻量级中值滤波，窗口大小 41 (左右各20)
            MedianFilter(_magnitudeDb, _background, 41);

            // ==========================================
            // 5. 寻峰策略 (关键修正)
            // ==========================================
            // 屏蔽低频 DC 附近 (0 ~ 1%)
            int minIdx = (int)(0.01 * _fftLength);
            // 屏蔽高频边缘 (45% ~ 50%) -> 修正了红线跑飞的问题
            int maxIdx = (int)(0.45 * _fftLength);

            double peakMaxVal = -9999;
            int peakMaxIdx = -1;
            double minPeakHeight = 3.0; // dB 阈值 (扣除背景后)

            for (int i = minIdx; i < maxIdx; i++)
            {
                // 核心逻辑：原始谱 - 背景谱
                double diff = _magnitudeDb[i] - _background[i];

                if (diff > minPeakHeight)
                {
                    // 寻找区域内最高峰
                    if (diff > peakMaxVal)
                    {
                        peakMaxVal = diff;
                        peakMaxIdx = i;
                    }
                }
            }

            // ==========================================
            // 6. 结果输出
            // ==========================================
            if (peakMaxIdx != -1)
            {
                // 频率计算：Index * Fs / N
                return (double)peakMaxIdx * fs / _fftLength;
            }

            return double.NaN;
        }

        /// <summary>
        /// 简单高效的一维中值滤波 (滑动窗口)
        /// 对于小窗口 (windowSize < 100)，Array.Sort 结合 L1 Cache 极其高效
        /// </summary>
        private void MedianFilter(double[] input, double[] output, int windowSize)
        {
            int halfWin = windowSize / 2;
            int len = input.Length;
            double[] tempWindow = new double[windowSize]; // 栈上分配开销极小

            for (int i = 0; i < len; i++)
            {
                int count = 0;
                // 计算窗口边界，处理数组边缘
                int start = i - halfWin;
                int end = i + halfWin;

                // 边界钳位
                if (start < 0) start = 0;
                if (end >= len) end = len - 1;

                // 填充窗口数据
                for (int j = start; j <= end; j++)
                {
                    tempWindow[count++] = input[j];
                }

                // 排序
                Array.Sort(tempWindow, 0, count);

                // 取中值
                output[i] = tempWindow[count / 2];
            }
        }
    }

    public static class DebugUtils
    {
        /// <summary>
        /// 将复数数组导出为 CSV 文件以便 MATLAB 分析
        /// </summary>
        /// <param name="data">复数数组</param>
        /// <param name="filePath">保存路径 (例如 "C:\\Temp\\debug_signal.csv")</param>
        public static void DumpComplexDataToCsv(Complex[] data, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.ASCII))
                {
                    // 写入表头 (可选，Matlab readmatrix 可以跳过)
                    // sw.WriteLine("Real,Imag");

                    foreach (var sample in data)
                    {
                        // 使用 "R" 或 "G17" 格式说明符确保双精度浮点数不丢失精度
                        sw.WriteLine($"{sample.Real:G17},{sample.Imaginary:G17}");
                    }
                }
                Console.WriteLine($"数据已成功导出到: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"导出数据失败: {ex.Message}");
            }
        }
        public static Complex[] LoadFromCsv(string filePath)
        {
            var list = new List<Complex>();

            // 读取所有行
            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 假设 CSV 用逗号分隔 (Matlab writematrix 默认行为)
                string[] parts = line.Split(',');

                if (parts.Length >= 2)
                {
                    // 使用 InvariantCulture 防止不同系统的小数点/逗号混淆
                    if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double re) &&
                        double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double im))
                    {
                        list.Add(new Complex(re, im));
                    }
                }
            }
            return list.ToArray();
        }
    }
}