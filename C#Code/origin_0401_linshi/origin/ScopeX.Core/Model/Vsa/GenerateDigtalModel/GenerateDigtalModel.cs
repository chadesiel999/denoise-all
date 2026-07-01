using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using ScopeX.MathExt;
using ScopeX.MathExt.Filter;
using ScopeX.Core.Tools;
using NPOI.POIFS.Properties;
using System.Diagnostics;

namespace ScopeX.Core
{

    public enum EqualizeOverSampling
    {
        OverSample1,
        OverSample2,
        OverSample3,
        OverSample4,
    }
    public class ParamStatistics
    {
        public Double? Value { get; set; }
        public Double? Mean { get; set; }
        public Double? Max { get; set; }
        public Double? Min { get; set; }
    }

    /// <summary>
    /// 包含所有通用数字解调的参数
    /// </summary>
    internal class GenerateDigtalModel : INotifyPropertyChanged
    {
        public GenerateDigtalModel()
        {
            FreqDetect = false;

            InphaseTimeGraph = new("InphaseTime()", DrawMethod.Plot);
            QuadratureTimeGraph = new("QuadratureTime()", DrawMethod.Plot);
            ConstellationGraph = new("Constellation()", DrawMethod.XYDots);
            IEyeGraph = new("VSAIEye()", DrawMethod.DPX);
            QEyeGraph = new("VSAQEye()", DrawMethod.DPX);
            VectorGraph = new("VectorGph()", DrawMethod.XYLines);
            EvmGraph = new("VSAEvm()", DrawMethod.Plot);
            SymbolErrorGraph = new("ErrParamList", DrawMethod.Plot);
            PhaseErrorTimeGraph = new("PhaseErrTime()", DrawMethod.Plot);
            AmplitudeErrorTimeGraph = new("AmplErrTime()", DrawMethod.Plot);

            ErrParamTable = new Dictionary<String, ParamStatistics>();
        }


        private VsaFormatOpt _FormatOpt = VsaFormatOpt.QAM16;
        public VsaFormatOpt FormatOpt
        {
            get => _FormatOpt;
            set
            {
                if (_FormatOpt != value)
                {
                    _FormatOpt = value;
                    OnPropertyChanged(nameof(FormatOpt));
                }
            }
        }

        public Double SymbolRateMax = 1e9;
        public Double SymbolRateMin = 10e6;

        private Double _SymbolRate = 200e6;
        /// <summary>
        /// 符号率
        /// </summary>
        public Double SymbolRate
        {
            get => _SymbolRate;
            set
            {
                _SymbolRate = value;
                OnPropertyChanged(nameof(SymbolRate));
            }
        }

        public Double RollOffFactorMax = 1;
        public Double RollOffFactorMin = 0;

        private Double _RollOffFactor = 0.8;
        /// <summary>
        /// 滚降因子
        /// </summary>
        public Double RollOffFactor
        {
            get => _RollOffFactor;
            set
            {
                _RollOffFactor = value;
                OnPropertyChanged(nameof(RollOffFactor));
            }
        }

        private Double _FilterPara = 0.8;
        /// <summary>
        /// 滤波器参数：0.001-1
        /// </summary>
        public Double FilterPara
        {
            get => _FilterPara;
            set
            {
                _FilterPara = value;
                OnPropertyChanged(nameof(FilterPara));
            }
        }

        public Double FilterParaMax = 1;
        public Double FilterParaMin = 0;


        private VsaMeasureFilterTypeOpt _MeasureFilterTypeOpt = VsaMeasureFilterTypeOpt.RootRaisedCosine;
        /// <summary>
        /// 测量滤波器
        /// </summary>
        public VsaMeasureFilterTypeOpt MeasureFilterType
        {
            get => _MeasureFilterTypeOpt;
            set
            {
                if (_MeasureFilterTypeOpt != value)
                {
                    _MeasureFilterTypeOpt = value;
                    OnPropertyChanged(nameof(MeasureFilterType));
                }
            }
        }

        private VsaRefFilterTypeOpt _RefFilterType = VsaRefFilterTypeOpt.RaisedCosine;
        /// <summary>
        /// 参考滤波器
        /// </summary>
        public VsaRefFilterTypeOpt RefFilterType
        {
            get => _RefFilterType;
            set
            {
                if (_RefFilterType != value)
                {
                    _RefFilterType = value;
                    OnPropertyChanged(nameof(RefFilterType));
                }
            }
        }

        /// <summary>
        /// 测量带宽
        /// </summary>
        public Double BandWidthMax = 20E9;
        public Double BandWidthMin = 1E8;

        private Double _BandWidth = 5E9;
        public Double BandWidth
        {
            get => _BandWidth;
            set
            {
                _BandWidth = value;
                OnPropertyChanged(nameof(BandWidth));
            }
        }

        /// <summary>
        /// 载波频率
        /// </summary>
        /// 
        public Double CarryFreqMax = 10E9;
        public Double CarryFreqMin = 1E6;

        private Double _CarryFreq = 300000000;
        public Double CarryFreq
        {
            get => _CarryFreq;
            set
            {
                _CarryFreq = value;
                OnPropertyChanged(nameof(CarryFreq));
            }

        }

        /// <summary>
        /// 载波频率误差
        /// </summary>
        public Double CarryFreqErrorMax = 1E6;
        public Double CarryFreqErrorMin = 0;

        private Double _CarryFreqError = 0;
        public Double CarryFreqError
        {
            get => _CarryFreqError;
            set
            {
                _CarryFreqError = value;
                OnPropertyChanged(nameof(CarryFreqError));
            }
        }

        /// <summary>
        /// 频率检测
        /// </summary>
        private Boolean _FreqDetect = false;
        public Boolean FreqDetect
        {
            get => _FreqDetect;
            set
            {
                if (_FreqDetect != value)
                {
                    _FreqDetect = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 均衡模式使能开关
        /// </summary>
        private Boolean _EqualizerEnabled = false;
        public Boolean EqualizerEnabled
        {
            get => _EqualizerEnabled;
            set
            {
                if (_EqualizerEnabled != value)
                {
                    _EqualizerEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 均衡复位标志位
        /// </summary>
        private Boolean _EqualizerReset = false;
        public Boolean EqualizerReset
        {
            get => _EqualizerReset;
            set
            {
                if (_EqualizerReset != value)
                {
                    _EqualizerReset = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 模式：训练，保持
        /// </summary>

        private VsaEqualizeMode _EqualizeMode = VsaEqualizeMode.Tranning;
        public VsaEqualizeMode EqualizeMode
        {
            get => _EqualizeMode;
            set
            {
                if (_EqualizeMode != value)
                {
                    _EqualizeMode = value;
                    OnPropertyChanged(nameof(EqualizeMode));
                }
            }
        }

        /// <summary>
        /// 均衡收敛系数
        /// </summary>
        public Double ConvergenceCoefficientMax = 10;
        public Double ConvergenceCoefficientMin = 0;

        private Double _ConvergenceCoefficient = 4;
        public Double ConvergenceCoefficient
        {
            get => _ConvergenceCoefficient;
            set
            {
                _ConvergenceCoefficient = value;
                OnPropertyChanged(nameof(ConvergenceCoefficient));
            }
        }



        /// <summary>
        /// 过采样率：1/2/4/8
        /// </summary>
        private EqualizeOverSampling _OverSample = EqualizeOverSampling.OverSample1;
        public EqualizeOverSampling OverSample
        {
            get => _OverSample;
            set
            {
                if (_OverSample != value)
                {
                    _OverSample = value;
                    OnPropertyChanged(nameof(OverSample));
                }
            }
        }


        /// <summary>
        /// 滤波器系数个数：3-100
        /// </summary>
        public Int32 FilterCofficientCntMax = 300;
        public Int32 FilterCofficientCntMin = 3;

        private Int32 _FilterCofficientCnt = 100;
        public Int32 FilterCofficientCnt
        {
            get => _FilterCofficientCnt;
            set
            {
                _FilterCofficientCnt = value;
                OnPropertyChanged(nameof(FilterCofficientCnt));
            }
        }

        /// <summary>
        /// 符号长度,10-1000
        /// </summary>
        public Int32 SymbolLengthMax = 1000;
        public Int32 SymbolLengthMin = 10;

        private Int32 _SymbolLength = 200;
        public Int32 SymbolLength
        {
            get => _SymbolLength;
            set
            {
                _SymbolLength = value;
                OnPropertyChanged(nameof(SymbolLength));
            }
        }

        private VsaTimeSyncType _TimeSyncType = VsaTimeSyncType.Gardner;

        /// <summary>
        /// 定时同步算法的选择
        /// </summary>
        public VsaTimeSyncType TimeSyncType
        {
            get => _TimeSyncType;
            set
            {
                if (_TimeSyncType != value)
                {
                    _TimeSyncType = value;
                    OnPropertyChanged(nameof(TimeSyncType));
                }
            }
        }
        /// <summary>
        /// 载波同步算法选择
        /// </summary>

        public VsaCarrySyncType _CarrySyncType = VsaCarrySyncType.DD;
        public VsaCarrySyncType CarrySyncType
        {
            get => _CarrySyncType;
            set
            {
                if (_CarrySyncType != value)
                {
                    _CarrySyncType = value;
                    OnPropertyChanged(nameof(CarrySyncType));
                }
            }
        }

        public Dictionary<String, ParamStatistics> ErrParamTable
        {
            get;
            private set;
        }

        /// <summary>
        /// I路时域图
        /// </summary>
        public VsaGenerateDigtalGraphModel InphaseTimeGraph;

        /// <summary>
        /// Q路时域图
        /// </summary>
        public VsaGenerateDigtalGraphModel QuadratureTimeGraph;

        /// <summary>
        /// 星座图
        /// </summary>
        public VsaGenerateDigtalGraphModel ConstellationGraph;

        /// <summary>
        /// I路眼图
        /// </summary>
        public VsaGenerateDigtalGraphModel IEyeGraph;

        /// <summary>
        /// Q路眼图
        /// </summary>
        public VsaGenerateDigtalGraphModel QEyeGraph;

        /// <summary>
        /// 矢量图
        /// </summary>
        public VsaGenerateDigtalGraphModel VectorGraph;

        /// <summary>
        /// 符号表
        /// </summary>
        public VsaGenerateDigtalGraphModel EvmGraph;

        /// <summary>
        /// 符号误差表
        /// </summary>
        public VsaGenerateDigtalGraphModel SymbolErrorGraph;

        /// <summary>
        /// 相位误差时间图
        /// </summary>
        public VsaGenerateDigtalGraphModel PhaseErrorTimeGraph;

        /// <summary>
        /// 幅度误差时间图
        /// </summary>
        public VsaGenerateDigtalGraphModel AmplitudeErrorTimeGraph;


        public static double[] ApplyLowPassFilterWithWindow(double[] input, double cutoffFrequency, double samplingRate)
        {
            int n = input.Length;
            double[] output = new double[n];

            if (cutoffFrequency <= 0 || cutoffFrequency >= samplingRate / 2)
            {
                throw new ArgumentException("截止频率必须大于0且小于采样率的一半");
            }

            double alpha = cutoffFrequency / (samplingRate / 2);
            double[] h = CalculateFilterCoefficientsWithWindow(alpha);

            for (int i = 0; i < n; i++)
            {
                output[i] = 0;
                for (int j = 0; j < h.Length; j++)
                {
                    if (i - j >= 0)
                    {
                        output[i] += h[j] * input[i - j];
                    }
                }
            }

            return output;
        }

        private static double[] CalculateFilterCoefficientsWithWindow(double alpha)
        {
            int N = 81; // Filter order (adjust as needed)
            double[] h = new double[N + 1];
            double window;

            for (int n = 0; n <= N; n++)
            {
                window = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * n / N)); // Hanning window
                if (n == N / 2)
                {
                    h[n] = 2 * alpha * window;
                }
                else
                {
                    h[n] = (Math.Sin(2 * Math.PI * alpha * (n - N / 2)) / (Math.PI * (n - N / 2))) * window;
                }
            }

            return h;
        }

        public static void writeFiledata(string path, double[] data)
        {
            if (data == null)
                return;
            //将data中的内容写入test.txt中
            StreamWriter sw = new StreamWriter(path);
            for (int i = 0; i < data.Length; i++)
            {
                sw.WriteLine(data[i]);
            }
            sw.Flush();
            sw.Close();
        }
        private Double[] ReadDataFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new Double[0];
            }
            StreamReader sr = new StreamReader(fileName);
            List<Double> data = new List<Double>();

            while (!sr.EndOfStream)
            {
                data.Add(Double.Parse(sr.ReadLine()!));
            }
            sr.Close();
            return data.ToArray();
        }
        public static Double[] readFiledata(String path)
        {

            List<Double> list = new List<Double>();


            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            //读取文件的一行数据到字符串中
            StreamReader sr = new StreamReader(fs);
            string str;
            Console.WriteLine("read....");
            while ((str = sr.ReadLine()) != null)
            {
                //list.Add(Convert.ToSingle(str));
                list.Add(Convert.ToSingle(str));
            }

            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();

            Double[] data = list.ToArray();
            Console.WriteLine("Read done!!!!");
            return data;
        }

        private Double[,] ReadMatrixFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"错误: 文件未找到 - {filePath}");
                return null;
            }

            try
            {
                // 1. 读取文件的所有行
                string[] lines = File.ReadAllLines(filePath);

                // 如果文件为空，则返回null
                if (lines.Length == 0)
                {
                    Console.WriteLine($"错误: 文件 '{filePath}' 为空。");
                    return null;
                }

                // 2. 确定矩阵的维度
                int numRows = lines.Length;
                // 使用第一行来确定列数
                int numCols = lines[0].Split('\t').Length;

                // 3. 创建结果二维数组
                Double[,] matrix = new Double[numRows, numCols];

                // 4. 逐行解析并填充矩阵
                for (int i = 0; i < numRows; i++)
                {
                    // 使用制表符分割当前行
                    string[] values = lines[i].Split('\t');

                    // 检查每行的列数是否一致
                    if (values.Length != numCols)
                    {
                        Console.WriteLine($"错误: 文件 '{filePath}' 的第 {i + 1} 行格式不一致。");
                        return null;
                    }

                    for (int j = 0; j < numCols; j++)
                    {
                        // 将字符串解析为double，使用 InvariantCulture 确保小数点'.'被正确处理
                        matrix[i, j] = Double.Parse(values[j], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                return matrix;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取或解析矩阵文件时发生错误: {ex.Message}");
                return null;
            }
        }
        public void Run(Double[] data, Double sampleRate)
        {
            //#if UESTC_DIST
            // 数据相关参数
            //Int32 M = 1024;
            Int32 M = _FormatOpt switch
            {
                VsaFormatOpt.BPSK => 2,
                VsaFormatOpt.QPSK => 4,
                VsaFormatOpt.PSK8 => 8,
                VsaFormatOpt.QAM16 => 16,
                VsaFormatOpt.QAM64 => 64,
                VsaFormatOpt.QAM128 => 128,
                VsaFormatOpt.QAM256 => 256,
                VsaFormatOpt.QAM1024 => 1024,
                _ => 16,
            };
            //Double beta = _FilterPara;
            Double beta = _FilterPara;
            ;
            Double sampleRateDataIn = sampleRate;
            // Double symBaudRate = _SymbolRate;
            Double symBaudRate = 8000;
            Int32 sps = 16;
            Int32 span = 40;
            Double sampleRateResample = symBaudRate * sps;

            ConvModeOpt mode = ConvModeOpt.Full;
            Double freqCarrier = _CarryFreq;

            // 采样率转换模块：将原始采样率的数据转换为符号率的整数倍            
            //_coeff = ReadDataFromFile(@"Coeff.txt");

            //if (_coeff.Length.Equals(0.0))
            //{
            //    return;
            //}

            //Int32 farrowfilterorder = 39;
            //Int32 farrowsubfilternum = 4;
            //Double[,] farrowCoeff = _coeff.ToMatrix(farrowsubfilternum, farrowfilterorder + 1);

            //Boolean signalresamplevalid = VSA.ReSample(data, sampleRateDataIn, sampleRateResample, farrowCoeff, out Double[]? dataoriginresample);
            //if (!signalresamplevalid || dataoriginresample == null)
            //{
            //    return;
            //}
            string dataoriginname = _FormatOpt switch
            {
                VsaFormatOpt.BPSK => "BPSK.txt",
                VsaFormatOpt.QPSK => "QPSK.txt",
                VsaFormatOpt.PSK8 => "8PSK.txt",
                VsaFormatOpt.QAM16 => "16QAM.txt",
                VsaFormatOpt.QAM64 => "64QAM.txt",
                VsaFormatOpt.QAM128 => "128QAM.txt",
                VsaFormatOpt.QAM256 => "256QAM.txt",
                VsaFormatOpt.QAM1024 => "1024QAM.txt",
                _ => "",
            };
            Double[] data2 = ReadDataFromFile(dataoriginname);
            // 数据相关参数

            // 数字下变频：原始数据，原始采样率，载波频率得到下低频信号结果
            //Double[] data2 = readFiledata("C:\\Users\\Asus\\Desktop\\test_vsa\\chanOut.txt");
            Boolean freqDownFlag = VSA.MixFreqDown(data2, sampleRateResample, freqCarrier, out var dmodSignal);
            Double[] dmodSignalI = dmodSignal.Select(o => o.Real).ToArray();
            Double[] dmodSignalI_LowPass = ApplyLowPassFilterWithWindow(dmodSignalI, (1 + beta) * symBaudRate, sampleRateResample);

            Double[] dmodSignalQ = dmodSignal.Select(o => o.Imaginary).ToArray();
            Double[] dmodSignalQ_LowPass = ApplyLowPassFilterWithWindow(dmodSignalQ, (1 + beta) * symBaudRate, sampleRateResample);

            Double coe = Math.Sqrt(4);
            Double[] dmodSignalI_1 = dmodSignalI.Select(o => o * coe).ToArray();// 需对比
            Double[] dmodSignalQ_1 = dmodSignalQ.Select(o => o * coe).ToArray();// 需对比
            if (!freqDownFlag || dmodSignal == null)
            {
                return;
            }

            // 获取匹配滤波器（根升余弦滤波器）系数：滚降系数，窗个数（这个值怎么来），过采样率（计算得来：采样率/符号速率）


            Int32 receiversps = sps;
            if (!_VsaMeasureFilter.ContainsKey(MeasureFilterType))
            {
                return;
            }

            IEnumerable<Complex> measureFilterOut;
            // 考虑不需要匹配滤波器的情况，无需卷积
            if (MeasureFilterType == VsaMeasureFilterTypeOpt.NoFilter)
            {
                measureFilterOut = dmodSignal;
            }
            IEnumerable<Double> measureFilterCoeff = _VsaMeasureFilter[MeasureFilterType].Invoke(beta, span, receiversps);

            // 将匹配滤波器系数和下变频以后的结果进行卷积，并丢掉卷积后多出的点，得到真正的基带信号


            IEnumerable<Double> measureFilterOutI = Algorithm.Convolve(dmodSignalI_1, measureFilterCoeff, mode);  // 卷积
            IEnumerable<Double> measureFilterOutQ = Algorithm.Convolve(dmodSignalQ_1, measureFilterCoeff, mode);

            measureFilterOut = measureFilterOutI.Zip(measureFilterOutQ, (i, q) => new Complex(i, q));
            Int32 interceptLength = measureFilterOutI.Count() - receiversps * span;
            IEnumerable<Complex> RemoveGroupDelay = measureFilterOut.Take(interceptLength);// 将后面数据去掉


            //if (!_VsaAmplidCoe.ContainsKey(_FormatOpt))
            //{
            //    return;
            //}
            // Double vsaamplidcoe = _VsaAmplidCoe[_FormatOpt];

            Complex[] symSynIn = RemoveGroupDelay.ToArray();
            // 定时同步：C1和C2有专门的算法进行产生
            // 1、Gardner算法进行时间误差估计（找到最大值与采样时刻点的偏差）
            // 2、分数延时（根据时间偏差进行插值，找到最大值）
            // 3、数控振荡器（NCO）：找到基于哪个点进行延时多少分数倍周期
            // 4、环路滤波器：负责滤除Gardner算法带来的噪声，并保持Gardner算法的输出向后级传输，得到每个符号的最值
            Double symbolsynbandWidth = 0.01;
            Double kesi = Math.Sqrt(2) / 2;

            VsaPllUseScne vsaPllUseScne = _FormatOpt switch
            {
                VsaFormatOpt.QPSK => VsaPllUseScne.MPSKsymbolsyn,
                VsaFormatOpt.QAM16 => VsaPllUseScne.MQAMsymbolsyn,
                VsaFormatOpt.PSK8 => VsaPllUseScne.MPSKsymbolsyn,
                VsaFormatOpt.BPSK => VsaPllUseScne.MPSKsymbolsyn,
                VsaFormatOpt.QAM64 => VsaPllUseScne.MQAMsymbolsyn,
                VsaFormatOpt.QAM128 => VsaPllUseScne.MQAMsymbolsyn,
                VsaFormatOpt.QAM256 => VsaPllUseScne.MQAMsymbolsyn,
                VsaFormatOpt.QAM1024 => VsaPllUseScne.MQAMsymbolsyn,
                _ => VsaPllUseScne.MPSKsymbolsyn,
            };


            Boolean symbolSynLoopFilterValid = VSA.LoopFilter(symbolsynbandWidth, kesi, sps, beta, M, VsaPllUseScne.MQAMsymbolsyn, out Double[]? symbolsyncoeff);
            if (symbolSynLoopFilterValid == false || symbolsyncoeff == null)
            {
                return;
            }

            // 定时同步算法


            Boolean symbolSynHpreFilterValid = VSA.hPrefilter(beta, sps, span, out var hprefilter);


            IEnumerable<Double> hprefilterOutI = Algorithm.Convolve(symSynIn.Select(o => o.Real).ToArray(), hprefilter, mode);  // 卷积
            IEnumerable<Double> hprefilterOutQ = Algorithm.Convolve(symSynIn.Select(o => o.Imaginary).ToArray(), hprefilter, mode);
            IEnumerable<Complex> hprefilterOut = hprefilterOutI.Zip(hprefilterOutQ, (i, q) => new Complex(i, q));
            Int32 interceptLength2 = hprefilterOutI.Count() - sps * span;

            IEnumerable<Complex> RemoveGroupDelay2 = hprefilterOut.Take(interceptLength2);// 对群时延进行补偿，并且将后一半数据去掉
            Complex[] symSynIn3 = RemoveGroupDelay2.ToArray();
            Complex[,] symSynIn4 = new Complex[symSynIn.Length, 1];
            Complex[,] symSynIn5 = new Complex[symSynIn.Length, 1];
            for (int i = 0; i < symSynIn.Length; i++)
            {
                symSynIn4[i, 0] = symSynIn[i];
                symSynIn5[i, 0] = symSynIn3[i];
            }
            Complex[] symSynOutComplex;
            Double[] pte;
            (symSynOutComplex, pte) = VSA.SymSynPreFilter(sps, -symbolsyncoeff[0], -symbolsyncoeff[1], symSynIn5, symSynIn4, VSA.ModFormat.MQAM);


            // 载波同步：核心目标是为了解决载波偏差，算法部分增加一个close选项
            //  载波同步环路滤波器系数生成
            Double carriersynbandWidth = 0.01;
            Double carriersynsps = 1;

            // 载波同步算法
            Double k0 = 1;
            Boolean carrierSynLoopFilterValid = VSA.LoopFilter(carriersynbandWidth, kesi, carriersynsps, beta, M, VsaPllUseScne.carriersyn, out Double[]? carriersyncoeff);
            if (carrierSynLoopFilterValid == false || carriersyncoeff == null)
            {
                return;
            }

            var carrierSyn = VSA.CarrierSyncDD(symSynOutComplex.ToArray(), carriersyncoeff[0], carriersyncoeff[1], k0, M, _FormatOpt);
            //string pathI = "C:\\Users\\Asus\\Desktop\\test_vsa\\pathI.txt";
            //string pathQ = "C:\\Users\\Asus\\Desktop\\test_vsa\\pathQ.txt";

            //var CI = carrierSyn.Select(o => o.Real).ToArray();
            //var CQ = carrierSyn.Select(o => o.Imaginary).ToArray();
            //writeFiledata(pathI, CI);
            //writeFiledata(pathQ, CQ);

            //均衡(待写)，抵消非理想信道（例如多径水声信道）以及非理想滤波器干扰
            if (!_OverSampleRateTable.ContainsKey(_OverSample))
            {
                return;
            }
            var oversamplerate = _OverSampleRateTable[_OverSample];


            // 获取参考滤波器（根升余弦滤波器）系数：滚降系数，窗个数（这个值怎么来），过采样率（计算得来：采样率/符号速率）

            Int32 refFilterSps = 16; // 面模板参数
            if (!_VsaRefFilter.ContainsKey(RefFilterType))
            {
                return;
            }

            Double[] refI = new double[5000];
            Double[] refQ = new double[5000];


            // 考虑不需要参考滤波器的情况，无需卷积

            //refI = carrierSyn.Select( o => o.Real).Take(400);       // I路参考波形
            //refQ = carrierSyn.Select( o => o.Imaginary).Take(400);  // Q路参考波形
            for (int i = 0; i < 5000; i++)
            {
                refI[i] = carrierSyn[i + 15000].Real;
                refQ[i] = carrierSyn[i + 15000].Imaginary;
            }
            IEnumerable<Complex> carrier = refI.Zip(refQ, (i, q) => new Complex(i, q));
            // 眼图
            Int32 symbolNumPlay = 8;
            UInt32 high = 4;
            UInt32 width = 1000;// (UInt32)symbolNumPlay;

            //Double[,]? IexyXY = VSA.GetEyeDiagramVSA(refI.Take(500).Select(o => o * 0.001), symbolNumPlay, refFilterSps, width, high);
            //Double[,]? QexyXY = VSA.GetEyeDiagramVSA(refQ.Take(500).Select(o => o * 0.001), symbolNumPlay, refFilterSps, width, high);
            Double[,]? IexyXY = VSA.GetEyeDiagramVSA(refI.Take(500), symbolNumPlay, refFilterSps, width, high);
            Double[,]? QexyXY = VSA.GetEyeDiagramVSA(refQ.Take(500), symbolNumPlay, refFilterSps, width, high);
            if (IexyXY == null || QexyXY == null)
            {
                return;
            }

            OccupierBuffer.Default.Provide("VSAIEye", new MathExt.Vector(IexyXY, "", "", 1.0, 1.0));
            OccupierBuffer.Default.Provide("VSAQEye", new MathExt.Vector(QexyXY, "", "", 1.0 / (Constants.IDX_PER_XDIV), 1.0));

            // VsaBuffer.Default.Provide("VSAIEye", new MathExt.Vector(IexyXY, "", "", 1.0 , 1.0));
            // VsaBuffer.Default.Provide("VSAQEye", new MathExt.Vector(QexyXY, "", "", 1.0 , 1.0));
            // IQ时域图
            OccupierBuffer.Default.Provide("InphaseTime", new MathExt.Vector(refI.Take(500).Select(o => o * 0.001), "", "", 1.0 / (symBaudRate), 1.0));
            OccupierBuffer.Default.Provide("QuadratureTime", new MathExt.Vector(refQ.Take(500).Select(o => o * 0.001), "", "", 1.0 / (symBaudRate), 1.0));

            // VsaBuffer.Default.Provide("InphaseTime", new MathExt.Vector(refI.Select(o => o*0.001), "s", "V", 1.0 / (symBaudRate), 1.0));
            // VsaBuffer.Default.Provide("QuadratureTime", new MathExt.Vector(refQ, "s", "V", 1.0 / (symBaudRate), 1.0));
            //星座图
            Double[] carrierSynI = refI.Select(o => o * 0.001).ToArray();
            Double[] carrierSynQ = refQ.Select(o => o * 0.001).ToArray();

            // Double[] carrierSynI = refI.ToArray();
            // Double[] carrierSynQ = refQ.ToArray();
            Double[,] constellationXY = new Double[2, carrierSynI.Length];
            Buffer.BlockCopy(carrierSynI.ToArray(), 0, constellationXY, 0, carrierSynI.Count() * sizeof(Double));
            Buffer.BlockCopy(carrierSynQ.ToArray(), 0, constellationXY, carrierSynI.Count() * sizeof(Double), carrierSynQ.Count() * sizeof(Double));
            // （-1，1）映射到水平方向的6格
            OccupierBuffer.Default.Provide("Constellation", new MathExt.Vector(constellationXY, "", "", 1.0, 1.0));

            // 矢量图
            Double[,] vectorXY = new Double[2, refI.Count()];
            Buffer.BlockCopy(refI.ToArray(), 0, vectorXY, 0, refI.Count() * sizeof(Double));
            Buffer.BlockCopy(refQ.ToArray(), 0, vectorXY, refI.Count() * sizeof(Double), refQ.Count() * sizeof(Double));
            OccupierBuffer.Default.Provide("VectorGph", new MathExt.Vector(vectorXY, "", "", 2.0 / (Constants.IDX_PER_XDIV * 6), 1.0));



            // 解调结果参数表征           
            //if (!_FindTheoryPosTable.ContainsKey(_FormatOpt))
            //{
            //    return;
            //}
            //var idealSignal1= _FindTheoryPosArrayTable[_FormatOpt].Invoke(carrier);
            VSA.PhaseDiscParam phasediscqpsk = new();
            // PD鉴相器
            //if (modFormat == ModFormat.MQAM)
            //{
            var carrierArray = carrier.ToArray();
            Complex[] idealSignal = new Complex[refI.Length];
            for (int i = 0; i < refI.Length; i++)
            {
                phasediscqpsk = VSA.DDPhaseDiscQam(carrierArray[i], M, _FormatOpt);
                idealSignal[i] = phasediscqpsk.CarrierSynSign;
            }

            //EVM
            IEnumerable<Double> EVM = idealSignal.Zip(carrier, (o, w) => (o - w).Magnitude);
            OccupierBuffer.Default.Provide("VSAEvm", new MathExt.Vector(EVM.Take(500).Select(o => o * 0.1), "", "", 1.0 / (symBaudRate), 1.0));

            // 相位误差图
            IEnumerable<Double> PhaseErr = carrier.Zip(idealSignal, (r, i) => r.Phase - i.Phase);
            OccupierBuffer.Default.Provide("PhaseErr&&Time", new MathExt.Vector(PhaseErr.Select(o => o * 0.1), "", "", 1.0 / (symBaudRate), 1.0));

            // 幅度误差图
            IEnumerable<Double> AmplErr = carrier.Zip(idealSignal, (r, i) => r.Magnitude - i.Magnitude);
            OccupierBuffer.Default.Provide("AmplErr&&Time", new MathExt.Vector(AmplErr.Select(o => o * 0.1), "", "", 1.0 / (symBaudRate), 1.0));

            // 误差参数
            //var errparam = ErrCaculate.CalcParam(carrier, idealSignal, EVM, PhaseErr, AmplErr);
            //if (errparam == null)
            //{
            //    return;
            //}

            //foreach (var prop in errparam.GetType().GetProperties())
            //{
            //    if (ErrParamTable.ContainsKey(prop.Name))
            //    {
            //        ErrParamTable[prop.Name] = (Double)prop.GetValue(errparam)!;
            //    }
            //    else
            //    {
            //        ErrParamTable.Add(prop.Name, (Double)prop.GetValue(errparam)!);
            //    }
            //}
            //#else
            //                       return;
            //#endif
            return;
        }
        #region 廖先强软件解调
        #region 辅助函数
        public static double[] GenerateHanningWindow(int windowLength)
        {
            double[] window = new double[windowLength];
            for (int n = 0; n < windowLength; n++)
            {
                window[n] = 0.5 * (1 - Math.Cos(2 * Math.PI * n / (windowLength - 1)));
            }
            return window;
        }
        public static double[] IdealLowPassCoefficients(int order, double cutoffFreq, double sampleRate)
        {
            int N = order + 1; // 系数数量
            double[] coefficients = new double[N];
            double center = (N - 1) / 2.0;

            for (int n = 0; n < N; n++)
            {
                double arg = 2 * Math.PI * cutoffFreq / sampleRate * (n - center);
                if (n == center)
                {
                    coefficients[n] = 2 * cutoffFreq / sampleRate;
                }
                else
                {
                    coefficients[n] = Math.Sin(arg) / arg * 2 * cutoffFreq / sampleRate;
                }
            }
            return coefficients;
        }
        public static double[] ApplyWindow(double[] coefficients, double[] window)
        {
            if (coefficients.Length != window.Length)
                throw new ArgumentException("数组长度需一致");

            double[] windowedCoefficients = new double[coefficients.Length];
            for (int i = 0; i < coefficients.Length; i++)
            {
                windowedCoefficients[i] = coefficients[i] * window[i];
            }
            return windowedCoefficients;
        }
        public static Double[] ConvolveMatlabFilter(Double[] x, Double[] h)
        {
            // 1. Get the lengths of the input signal and the filter
            int len_x = x.Length;
            int len_h = h.Length;

            // The length of the full convolution result
            int len_full = len_x + len_h - 1;

            // Create an array to store the full convolution result
            Double[] y_full = new Double[len_full];

            // 2. Perform the full convolution
            // This loop calculates the convolution sum for each output point
            for (int n = 0; n < len_full; n++)
            {
                double sum = 0;
                // Determine the start and end points for k in the sum
                int k_start = Math.Max(0, n - len_x + 1);
                int k_end = Math.Min(len_h - 1, n);

                for (int k = k_start; k <= k_end; k++)
                {
                    // Ensure indices are within bounds
                    if ((n - k) < len_x)
                    {
                        sum += h[k] * x[n - k];
                    }
                }
                y_full[n] = sum;
            }

            // 3. Truncate the result to match the length of the input signal 'x'
            // This is the key step that mimics MATLAB's filter() behavior.
            Double[] y_truncated = new Double[len_x];
            Array.Copy(y_full, 0, y_truncated, 0, len_x);

            return y_truncated;
        }
        public static (double c1, double c2) CalculateCarrierLoopCoefficients(double BT, double kesi, int SPS, int M)
        {
            // 1. Calculate Phase Detector Gain (kd) as in MATLAB
            // kd = 1/log2(M);
            double kd = 1.0 / Math.Log2(M);

            // 2. Calculate intermediate variable theta
            // theta = (BT/SPS) / (kesi + 1/(kesi*4));
            double theta = (BT / SPS) / (kesi + 1.0 / (kesi * 4.0));

            // 3. Calculate the common denominator part of the formula
            // common_denom = (1 + 2*kesi*theta + theta^2)
            double common_denom = 1.0 + 2.0 * kesi * theta + theta * theta;

            // 4. Calculate the final divisor
            // divisor = kd^2 / 2
            double divisor = (kd * kd) * 2.0;

            // 5. Calculate coefficients c1 and c2
            // c1 = (4*kesi*theta) / common_denom / divisor;
            // c2 = (4*theta^2) / common_denom / divisor;
            double c1 = (4.0 * kesi * theta) / common_denom / divisor;
            double c2 = (4.0 * theta * theta) / common_denom / divisor;

            return (c1, c2);
        }
        public static Boolean TimeLoopFilter(Double bandWidth, Double kesi, Double sps, Double beta, Int32 M, out Double[]? coeff)
        {

            // if (!(M % 16).Equals(0) || bandWidth > 0.1 || beta < 0 || beta > 1 || sps.Equals(0.0) || kesi.Equals(0.0) || kesi < 0 || kesi > 1)
            if (bandWidth > 0.1 || beta < 0 || beta > 1 || sps.Equals(0.0) || kesi.Equals(0.0) || kesi < 0 || kesi > 1)
            {
                coeff = null;
                return false;
            }

            coeff = new Double[2];

            // 鉴相器增益
            var kd = 2 * Math.Sin(Math.PI * beta / 2) / (1 - Math.Pow(beta, 2) / 4) / Math.Log2(M);
            Double theta = (bandWidth / sps) / (kesi + 1 / (kesi * 4));

            // 环路滤波器系数
            Double c1 = (4 * kesi * theta) / (1 + 2 * kesi * theta + Math.Pow(theta, 2)) / kd;
            Double c2 = (4 * Math.Pow(theta, 2)) / (1 + 2 * kesi * theta + Math.Pow(theta, 2)) / kd;

            coeff[0] = c1;
            coeff[1] = c2;

            return true;
        }
        #endregion
        /// <summary>
        /// 2025.8.8 软件解调，逻辑和Matlab仿真的top_logic_samewith_fpga.m一致
        /// </summary>
        /// <param name="adcData">输入的采样信号</param>
        /// <param name="sampleRate_original">输入信号的采样率</param>
        public void RunSoftwareDemodulation(Double[] adcData, Double sampleRate_original)
        {
            //================================================================================
            // STEP 0: 参数设置
            //================================================================================
            Double baudRate = SymbolRate;                             //符号率
            Double beta = RollOffFactor;                              //滚降因子
            Double freqCarrier = CarryFreq;
            //Double freqCarrier = 1e9;                               //载波频率
            Double fs_sys = sampleRate_original;                    //采样率
            int filterOrder = 80;                                   //FIR低通滤波器阶数
            double fs_receive = 2.5e9;                              // signalReceiver 当前的采样率
            int sps_after_resample = 16;                            // 我们固定的内部工作SPS（每个符号点的采样点数）
            int span = 40;                                          // 根升余弦滤波器跨度，与MATLAB一致



            //================================================================================
            // STEP 1: 数字下变频 (Digital Down-Conversion)
            //================================================================================
            Boolean freqDownFlag = VSA.MixFreqDown(adcData, fs_sys, freqCarrier, out var dmodSignal);
            if (!freqDownFlag || dmodSignal == null)
            {
                return;
            }



            //================================================================================
            // STEP 2: 低通滤波 (Low-Pass Filtering)
            //================================================================================
            Double cutoffFrequency = (1 + beta) * baudRate;         //截至频率
            double[] idealCoeffs = IdealLowPassCoefficients(filterOrder, cutoffFrequency, fs_sys);

            // 2. 生成汉宁窗
            double[] hanningWindow = GenerateHanningWindow(filterOrder + 1);

            // 3. 将窗函数应用到理想系数上，得到最终的FIR滤波器系数
            double[] firFilterCoeffs = ApplyWindow(idealCoeffs, hanningWindow);
            Double[] dmodSignalI = dmodSignal.Select(o => o.Real).ToArray();
            Double[] dmodSignalQ = dmodSignal.Select(o => o.Imaginary).ToArray();

            Double[] dmodSignalI_LowPass = ConvolveMatlabFilter(dmodSignalI, firFilterCoeffs);
            Double[] dmodSignalQ_LowPass = ConvolveMatlabFilter(dmodSignalQ, firFilterCoeffs);



            //================================================================================
            // STEP 3: 降采样 (Decimation by 8)
            //================================================================================
            // 使用LINQ的Where方法可以非常简洁地实现降采样
            // (sample, index) => index % decimationFactor == 0 的意思是：只保留那些索引号能被8整除的点
            //Double[] decimatedI = dmodSignalI_LowPass
            //                        .Where((sample, index) => index % decimationFactor == 0)
            //                        .ToArray();

            //Double[] decimatedQ = dmodSignalQ_LowPass
            //                        .Where((sample, index) => index % decimationFactor == 0)
            //                        .ToArray();
            int decimatedLength = dmodSignalI_LowPass.Length / 8;
            Double[] decimatedI = new Double[decimatedLength];
            Double[] decimatedQ = new Double[decimatedLength];
            for (int i = 0; i < decimatedLength; i++)
            {
                decimatedI[i] = dmodSignalI_LowPass[i * 8];
                decimatedQ[i] = dmodSignalQ_LowPass[i * 8];
            }
            //================================================================================
            // STEP 4: 组装最终的复基带信号
            //================================================================================
            // 将降采样后的I路和Q路数据组合成复数数组。
            Complex[] signalReceiver = decimatedI.Zip(decimatedQ, (i, q) => new Complex(i, q)).ToArray();
            // *** 重采样 ***
            double fs_after_resample = baudRate * sps_after_resample;   // 重采样后的目标采样率
            Complex[] data_after_resample;                              // 用于存放重采样结果
            if (Math.Abs(fs_receive - fs_after_resample) < 1.0)         // 使用一个小的容差来比较浮点数
            {
                // 如果采样率几乎一样，直接跳过，不做任何处理
                data_after_resample = signalReceiver;
            }
            else
            {
                // 2a. 从文件加载Farrow滤波器系数
                string farrowCoeffFilePath = @"D:\Matlab_VSA\MATLAB_0417\farrow_coeffs.txt";
                Double[,] farrowCoeffs = ReadMatrixFromFile(farrowCoeffFilePath);

                if (farrowCoeffs == null)
                {
                    Console.WriteLine("错误：Farrow系数加载失败，已终止重采样。");
                    return;
                }

                // 2b. 分别对I路和Q路进行重采样 (这部分代码不变)
                VSA.ReSample(
                    signalReceiver.Select(c => c.Real).ToArray(),
                    fs_receive,
                    fs_after_resample,
                    farrowCoeffs,
                    out Double[]? resampledI
                );
                VSA.ReSample(
                    signalReceiver.Select(c => c.Imaginary).ToArray(),
                    fs_receive,
                    fs_after_resample,
                    farrowCoeffs,
                    out Double[]? resampledQ
                );

                if (resampledI == null || resampledQ == null)
                {
                    Console.WriteLine("错误：重采样执行失败。");
                    return;
                }

                // 2c. 组合重采样后的信号 
                data_after_resample = resampledI.Zip(resampledQ, (i, q) => new Complex(i, q)).ToArray();
            }



            //================================================================================
            // STEP 5: 接收滤波 (匹配滤波)
            //================================================================================
            // 目标: 应用一个根升余弦(RRC)滤波器来最大化信噪比并抑制噪声。
            // 1. 设计RRC滤波器系数
            //    调用 RootRaisedCosFilter.cs 中的函数
            Double[] rrcFilterCoeffs = VSA.RootRaisedCosFilter(beta, span, sps_after_resample).ToArray();

            // 2. 应用滤波器
            //    分别对I路和Q路进行滤波
            Double[] filteredI = ConvolveMatlabFilter(
                data_after_resample.Select(c => c.Real).ToArray(),
                rrcFilterCoeffs
            );

            Double[] filteredQ = ConvolveMatlabFilter(
                data_after_resample.Select(c => c.Imaginary).ToArray(),
                rrcFilterCoeffs
            );

            // 3. 组合滤波后的信号
            //    这个 rxSample 变量就对应MATLAB中的 rxSample
            Complex[] rxSample = filteredI.Zip(filteredQ, (i, q) => new Complex(i, q)).Skip(640).ToArray();



            //================================================================================
            // STEP 6: 定时同步前归一化 (Normalization)
            //================================================================================
            // 目标: 将信号幅度调整到一个稳定的范围，为后续的定时同步做准备。
            // 1. 定义用于寻找最大幅度的窗口大小
            int timing_maxnum = 3000;

            // 检查 rxSample 的长度是否足够
            if (rxSample.Length <= timing_maxnum)
            {
                Console.WriteLine("错误：信号长度不足以进行归一化。");
                return;
            }

            // 2. 截取信号的前 timing_maxnum 个点用于分析
            Complex[] analysisWindow = rxSample.Take(timing_maxnum).ToArray();

            // 3. 计算窗口内的最大平方幅度 (I² + Q²)
            double maxSquaredAmplitude = analysisWindow.Select(c => c.Magnitude * c.Magnitude).Max();

            // 4. 计算归一化因子（最大幅度）
            double normalizationFactor = Math.Sqrt(maxSquaredAmplitude);

            // 5. 应用归一化并丢弃用于分析的窗口部分
            Complex[] rxSample_nor;

            if (normalizationFactor > 1e-9) // 避免除以零
            {
                rxSample_nor = rxSample
                                    .Skip(timing_maxnum) // 丢弃前3000个点
                                    .Select(c => c / normalizationFactor) // 将剩余的点逐个进行归一化
                                    .ToArray();
            }
            else
            {
                // 如果归一化因子接近零（信号几乎为零），则直接跳过归一化，只丢弃窗口
                rxSample_nor = rxSample.Skip(timing_maxnum).ToArray();
            }

            //================================================================================
            // STEP 7: 定时同步预滤波 (Prefiltering for TED)
            //================================================================================
            // 目标: 生成用于定时误差检测器(TED)的辅助信号。

            // 1. 定义预滤波器的参数
            int prefilterSpan = span / 4;

            // 2. 调用 hPrefilter.cs 中的函数来设计滤波器系数
            Boolean prefilterValid = VSA.hPrefilter(beta, sps_after_resample, prefilterSpan, out Double[]? hprefilterCoeffs);

            if (!prefilterValid || hprefilterCoeffs == null)
            {
                Console.WriteLine("错误：预滤波器系数生成失败。");
                return;
            }

            // 3. 应用预滤波器
            //    输入是归一化后的信号 rxSample_nor
            Double[] prefilteredI = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Real).ToArray(), hprefilterCoeffs);
            Double[] prefilteredQ = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Imaginary).ToArray(), hprefilterCoeffs);

            // 4. 组合成复数信号
            Complex[] rxSamplePrefilter = prefilteredI.Zip(prefilteredQ, (i, q) => new Complex(i, q)).ToArray();



            //================================================================================
            // STEP 8: 计算定时同步环路滤波器系数
            //================================================================================
            // 目标: 为 TimingSynchronizer 提供所需的比例和积分增益。
            // 我们将调用 VSA.LoopFilter 来计算这些系数。
            Double symbolsynbandWidth = 0.008;              // 归一化环路带宽，对应 BT
            Double kesi = Math.Sqrt(2) / 2.0;               // 阻尼系数
            Int32 M = _FormatOpt switch
            {
                VsaFormatOpt.BPSK => 2,
                VsaFormatOpt.QPSK => 4,
                VsaFormatOpt.PSK8 => 8,
                VsaFormatOpt.QAM16 => 16,
                VsaFormatOpt.QAM64 => 64,
                VsaFormatOpt.QAM128 => 128,
                VsaFormatOpt.QAM256 => 256,
                VsaFormatOpt.QAM1024 => 1024,
                _ => 16,
            };                                    // 调制阶数
            ModulationFormat modFormat;

            switch (_FormatOpt)
            {
                case VsaFormatOpt.BPSK:
                case VsaFormatOpt.QPSK:
                case VsaFormatOpt.PSK8:
                    modFormat = ModulationFormat.PSK;
                    break;
                case VsaFormatOpt.QAM16:
                case VsaFormatOpt.QAM64:
                case VsaFormatOpt.QAM128:
                case VsaFormatOpt.QAM256:
                case VsaFormatOpt.QAM1024:
                    modFormat = ModulationFormat.QAM;
                    break;
                default:
                    // 提供一个默认值或抛出异常，以处理未预期的格式
                    modFormat = ModulationFormat.QAM;
                    // throw new NotSupportedException($"Modulation format {_FormatOpt} is not supported.");
                    break;
            }

            // 调用系数生成函数
            Boolean symbolSynLoopFilterValid = TimeLoopFilter(symbolsynbandWidth, kesi, sps_after_resample, beta, M, out Double[]? symbolsyncoeff);

            if (!symbolSynLoopFilterValid || symbolsyncoeff == null || symbolsyncoeff.Length < 2)
            {
                Console.WriteLine("错误：定时同步环路滤波器系数生成失败。");
                return;
            }

            double c1_loop = -symbolsyncoeff[0];
            double c2_loop = -symbolsyncoeff[1];



            //================================================================================
            // STEP 9: 执行定时同步
            //================================================================================
            // 目标: 使用新的 TimingSynchronizer 类来恢复出符号点。

            // 1. 准备输入数据
            //    在MATLAB脚本中，为了处理边界效应，对输入信号进行了截断：
            //    rxSamplePrefilter_short = rxSamplePrefilter(order+1:testnum_sync+order);
            //    rxSample_short = rxSample_nor(order/2+1:testnum_sync+order/2);
            //    这里的 order 是160。这是因为预滤波器 hprefilter 的长度是 161 (span/4 * sps + 1 = 10 * 16 + 1)。
            //    群延迟是 (161-1)/2 = 80。
            //    `filter`函数又额外引入了80的延迟。总共大约160。

            int order = (prefilterSpan * sps_after_resample); // 10 * 16 = 160
            int testnum_sync = rxSample_nor.Length - order;

            if (rxSamplePrefilter.Length <= order || rxSample_nor.Length <= (order / 2) || testnum_sync <= 0)
            {
                Console.WriteLine("错误：信号长度不足以进行定时同步处理。");
                return;
            }

            // 截取预滤波后的信号，跳过前面 order 个点
            ReadOnlySpan<Complex> prefiltered_short = rxSamplePrefilter.AsSpan().Slice(order, testnum_sync);
            // 截取原始信号，跳过前面 order/2 个点
            ReadOnlySpan<Complex> original_short = rxSample_nor.AsSpan().Slice(order / 2, testnum_sync);

            // 2. 实例化 TimingSynchronizer
            var synchronizer = new TimingSynchronizer(
                sps: sps_after_resample,
                c1: c1_loop,
                c2: c2_loop,
                c3: c1_loop, // 跟踪阶段也用相同的系数
                c4: c2_loop, // 跟踪阶段也用相同的系数
                beta: beta,
                modFormat: modFormat // 因为我们是16QAM
            );

            // 3. 执行同步处理
            var (synchronizedSymbols, timingErrors) = synchronizer.SynchronizeBlock(
                prefiltered_short,
                original_short
            );

            //================================================================================
            // STEP 10: 计算载波同步环路滤波器系数
            //================================================================================
            // 目标: 使用新创建的 CalculateCarrierLoopCoefficients 函数来生成系数。

            // 从MATLAB脚本中获取载波环路参数
            double carriersynbandWidth = 0.03;          // 对应 BT_car
            int carrierSps = 1;                         // 载波环路在每个符号点上操作一次，所以SPS是1

            (double c5, double c6) = CalculateCarrierLoopCoefficients(carriersynbandWidth, kesi, carrierSps, M);

            int discardnum_after_timing = 1000;
            if (synchronizedSymbols.Count <= discardnum_after_timing)
            {
                Console.WriteLine("错误：定时同步后的符号点数量不足以进行载波同步。");
                return;
            }
            Complex[] carrierSyncInput = synchronizedSymbols.Skip(discardnum_after_timing).ToArray();
            Complex[] normalizedSignal = CarrierSynchronizer.NormalizeInputSignal(carrierSyncInput, 1);
            Complex[] refConstellation = VSA.GetStandardConstellation(_FormatOpt, M);
            double k0 = 1; // NCO增益，与MATLAB一致
            CarrierSynchronizer carrierSynchronizer = new CarrierSynchronizer(
                loopGainKp: c5,
                loopGainKi: c6,
                ncoGain: k0,
                referenceConstellation: refConstellation,
                modulationFormat: modFormat.ToString(), // 根据您的设置
                modulationOrder: M
            );
            // 4. 执行同步处理
            Complex[] synchronizedOutput = carrierSynchronizer.Process(normalizedSignal, out double[] phaseEstimates, out double[] phaseErrors);
            //var linesToSave_carrier = synchronizedOutput.Select(c => $"{c.Real.ToString(System.Globalization.CultureInfo.InvariantCulture)},{c.Imaginary.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            //System.IO.File.WriteAllLines("cs_carrierSynchronized.csv", linesToSave_carrier);
            Double[] carrierSynI = synchronizedOutput.Real().ToArray();
            Double[] carrierSynQ = synchronizedOutput.Imaginary().ToArray();
        }
        private readonly CarrierEstimator _carrierEstimator = new CarrierEstimator();
        private readonly SymbolRateEstimator _symbolRateEstimator = new SymbolRateEstimator(8192);

        /// <summary>
        /// 估计输入信号的载波频率和符号率。
        /// </summary>
        /// <param name="adcData">输入信号（实数采样）</param>
        /// <param name="sampleRate">输入信号采样率</param>
        /// <returns>估计结果：(carrierFreq, symbolRate, evm, snr)</returns>
        public (Double carrierFreq, Double symbolRate, Double evm, Double snr) EstimateCarrierAndSymbolRate(Double[] adcData, Double sampleRate)
        {
            return (CarryFreq, SymbolRate, errparam.EvmRms, errparam.SNR);
        }

        private ErrParam errparam = new();
        //public (Double carrierFreq, Double symbolRate, Double evm, Double snr) EstimateCarrierAndSymbolRate(Double[] adcData, Double sampleRate)
        //{
        //    if (adcData == null || adcData.Length == 0)
        //    {
        //        throw new ArgumentException("输入信号不能为空。", nameof(adcData));
        //    }

        //    if (sampleRate <= 0 || Double.IsNaN(sampleRate) || Double.IsInfinity(sampleRate))
        //    {
        //        throw new ArgumentException("采样率必须是有效的正数。", nameof(sampleRate));
        //    }

        //    Double estimatedCarrier = CarryFreq;
        //    Double estimatedSymbolRate = SymbolRate;
        //    Double estimatedEvm = Double.NaN;
        //    Double estimatedSnr = Double.NaN;
        //    int decimationFactor = 8;
        //    Double fs_sys = sampleRate;
        //    double fs_decimated = fs_sys / decimationFactor;

        //    void TryFillEvmAndSnrFromErrParamTable()
        //    {
        //        if (ErrParamTable.TryGetValue("EVM(%)", out var evmPercentStat))
        //        {
        //            if (evmPercentStat.Mean.HasValue && Double.IsFinite(evmPercentStat.Mean.Value))
        //            {
        //                estimatedEvm = evmPercentStat.Mean.Value;
        //            }
        //            else if (evmPercentStat.Value.HasValue && Double.IsFinite(evmPercentStat.Value.Value))
        //            {
        //                estimatedEvm = evmPercentStat.Value.Value;
        //            }
        //        }
        //        else if (ErrParamTable.TryGetValue("EvmRms", out var evmRmsStat)
        //            && evmRmsStat.Value.HasValue && Double.IsFinite(evmRmsStat.Value.Value))
        //        {
        //            estimatedEvm = evmRmsStat.Value.Value;
        //        }

        //        if (ErrParamTable.TryGetValue("SNRdB", out var snrDbStat)
        //            && snrDbStat.Value.HasValue && Double.IsFinite(snrDbStat.Value.Value))
        //        {
        //            estimatedSnr = snrDbStat.Value.Value;
        //            return;
        //        }

        //        if (ErrParamTable.TryGetValue("SNR", out var snrLinearStat)
        //            && snrLinearStat.Value.HasValue && Double.IsFinite(snrLinearStat.Value.Value)
        //            && snrLinearStat.Value.Value > 0)
        //        {
        //            estimatedSnr = 10.0 * Math.Log10(snrLinearStat.Value.Value);
        //        }
        //    }

        //    bool TryApplyEstimatedSymbolRate(double candidateRate)
        //    {
        //        if (double.IsNaN(candidateRate) || double.IsInfinity(candidateRate) || candidateRate <= 0)
        //        {
        //            return false;
        //        }

        //        double maxReasonableRate = fs_decimated * 0.45;
        //        if (candidateRate >= maxReasonableRate)
        //        {
        //            return false;
        //        }

        //        estimatedSymbolRate = candidateRate;
        //        return true;
        //    }

        //    // 实采数据通常存在直流分量，先去直流再做估计会更稳。
        //    Double[] centeredSignal = adcData;
        //    double dcOffset = centeredSignal.Average();
        //    if (Math.Abs(dcOffset) > 1e-12)
        //    {
        //        centeredSignal = centeredSignal.Select(x => x - dcOffset).ToArray();
        //    }

        //    var carrierResult = _carrierEstimator.Estimate(centeredSignal, fs_sys);
        //    if (!double.IsNaN(carrierResult.EstimatedFrequency) && !double.IsInfinity(carrierResult.EstimatedFrequency) && carrierResult.EstimatedFrequency > 0)
        //    {
        //        estimatedCarrier = carrierResult.EstimatedFrequency;
        //    }

        //    Boolean freqDownFlag = VSA.MixFreqDown(centeredSignal, fs_sys, estimatedCarrier, out var dmodSignal);
        //    if (!freqDownFlag || dmodSignal == null)
        //    {
        //        TryFillEvmAndSnrFromErrParamTable();
        //        return (estimatedCarrier, estimatedSymbolRate, estimatedEvm, estimatedSnr);
        //    }
        //    Complex[] dmodSignalArray = dmodSignal.ToArray();
        //    if (dmodSignalArray.Length == 0)
        //    {
        //        TryFillEvmAndSnrFromErrParamTable();
        //        return (estimatedCarrier, estimatedSymbolRate, estimatedEvm, estimatedSnr);
        //    }

        //    Double[] dmodSignalI = dmodSignalArray.Select(o => o.Real).ToArray();
        //    Double[] dmodSignalQ = dmodSignalArray.Select(o => o.Imaginary).ToArray();

        //    // 粗符号率估计
        //    int snapshotLen = 32768;
        //    int actualSnapshotLen = Math.Min(dmodSignalI.Length, snapshotLen);
        //    if (actualSnapshotLen == 0)
        //    {
        //        TryFillEvmAndSnrFromErrParamTable();
        //        return (estimatedCarrier, estimatedSymbolRate, estimatedEvm, estimatedSnr);
        //    }

        //    Double[] dmodI_snapshot = new Double[actualSnapshotLen];
        //    Double[] dmodQ_snapshot = new Double[actualSnapshotLen];
        //    Array.Copy(dmodSignalI, dmodI_snapshot, actualSnapshotLen);
        //    Array.Copy(dmodSignalQ, dmodQ_snapshot, actualSnapshotLen);

        //    Double coarseCutoff = Math.Min(1.0e9, fs_sys * 0.45);
        //    if (coarseCutoff <= 0)
        //    {
        //        TryFillEvmAndSnrFromErrParamTable();
        //        return (estimatedCarrier, estimatedSymbolRate, estimatedEvm, estimatedSnr);
        //    }

        //    int coarseOrder = 60;
        //    double[] coarseIdealCoeffs = IdealLowPassCoefficients(coarseOrder, coarseCutoff, fs_sys);
        //    double[] coarseWindow = GenerateHanningWindow(coarseOrder + 1);
        //    double[] coarseFirCoeffs = ApplyWindow(coarseIdealCoeffs, coarseWindow);
        //    Double[] snapI_filtered = ConvolveMatlabFilter(dmodI_snapshot, coarseFirCoeffs);
        //    Double[] snapQ_filtered = ConvolveMatlabFilter(dmodQ_snapshot, coarseFirCoeffs);

        //    List<Complex> snap_decimated_list = new List<Complex>();
        //    for (int i = 0; i < snapI_filtered.Length; i += decimationFactor)
        //    {
        //        snap_decimated_list.Add(new Complex(snapI_filtered[i], snapQ_filtered[i]));
        //    }
        //    Complex[] snap_decimated = snap_decimated_list.ToArray();
        //    if (snap_decimated.Length > 0)
        //    {
        //        double coarseRate = _symbolRateEstimator.Estimate(snap_decimated, fs_decimated);
        //        TryApplyEstimatedSymbolRate(coarseRate);
        //    }

        //    // 精细符号率估计
        //    int filterOrder = 80;
        //    Double cutoffFrequency = (1 + RollOffFactor) * estimatedSymbolRate;
        //    cutoffFrequency = Math.Min(cutoffFrequency, fs_sys * 0.45);

        //    if (cutoffFrequency > 0)
        //    {
        //        double[] idealCoeffs = IdealLowPassCoefficients(filterOrder, cutoffFrequency, fs_sys);
        //        double[] hanningWindow = GenerateHanningWindow(filterOrder + 1);
        //        double[] firFilterCoeffs = ApplyWindow(idealCoeffs, hanningWindow);
        //        Double[] dmodSignalI_LowPass = ConvolveMatlabFilter(dmodSignalI, firFilterCoeffs);
        //        Double[] dmodSignalQ_LowPass = ConvolveMatlabFilter(dmodSignalQ, firFilterCoeffs);

        //        Double[] decimatedI = dmodSignalI_LowPass
        //                                .Where((sample, index) => index % decimationFactor == 0)
        //                                .ToArray();

        //        Double[] decimatedQ = dmodSignalQ_LowPass
        //                                .Where((sample, index) => index % decimationFactor == 0)
        //                                .ToArray();

        //        Complex[] signalReceiver = decimatedI.Zip(decimatedQ, (i, q) => new Complex(i, q)).ToArray();
        //        if (signalReceiver.Length > 0)
        //        {
        //            double refinedRate = _symbolRateEstimator.Estimate(signalReceiver, fs_decimated);
        //            TryApplyEstimatedSymbolRate(refinedRate);
        //        }
        //    }

        //    TryFillEvmAndSnrFromErrParamTable();
        //    return (estimatedCarrier, estimatedSymbolRate, estimatedEvm, estimatedSnr);
        //}

        public void Run_New(Double[] adcData, Double sampleRate_original)
        {
            int maxDataPoints = 300000;
            if (adcData != null && adcData.Length > maxDataPoints)
            {
                Double[] truncatedData = new Double[maxDataPoints];
                Array.Copy(adcData, truncatedData, maxDataPoints); // 复制前 maxDataPoints 点
                adcData = truncatedData;
            }

            // 当外部未提供有效数据时，才尝试从文件读取（兼容旧调试流程）
            string workingDirectory = Directory.GetCurrentDirectory();
            string dataFilePath = Path.Combine(workingDirectory, "chanOut_data.txt");
            if ((adcData == null || adcData.Length == 0) && File.Exists(dataFilePath))
            {
                adcData = ReadDataFromFile(dataFilePath);
                if (adcData == null || adcData.Length == 0)
                {
                    Console.WriteLine("错误：文件读取到的数据为空。");
                    return;
                }
            }
            if (adcData == null || adcData.Length == 0)
            {
                Console.WriteLine("错误：输入采样数据为空，且未找到可用的文件数据。");
                return;
            }

            //================================================================================
            // STEP 0: 参数设置
            //================================================================================
            Double baudRate = SymbolRate;
            Double beta = RollOffFactor;
            Double freqCarrier = CarryFreq;
            int decimationFactor = 8;
            bool hasEstimatedBaudRate = false;
            // 优先使用外部输入的采样率；异常时回退到 20 GHz（兼容历史逻辑）
            Double fs_sys = sampleRate_original > 0 ? sampleRate_original : 20e9;
            double fs_decimated = fs_sys / decimationFactor;

            // 符号率估计结果保护：避免异常峰值破坏后续重采样和滤波参数。
            bool TryApplyEstimatedBaudRate(double candidateRate)
            {
                if (double.IsNaN(candidateRate) || double.IsInfinity(candidateRate) || candidateRate <= 0)
                {
                    return false;
                }

                // 经验约束：符号率应明显小于当前采样率（留一点裕量，避免贴近奈奎斯特边缘）。
                double maxReasonableRate = fs_decimated * 0.45;
                if (candidateRate >= maxReasonableRate)
                {
                    return false;
                }

                baudRate = candidateRate;
                hasEstimatedBaudRate = true;
                return true;
            }

            // 实采数据通常存在直流分量，先去直流再做频偏估计，提高估计稳定性
            //double dcOffset = adcData.Average();
            //if (Math.Abs(dcOffset) > 1e-12)
            //{
            //    adcData = adcData.Select(x => x - dcOffset).ToArray();
            //}

            var result = _carrierEstimator.Estimate(adcData, fs_sys);
            if (!double.IsNaN(result.EstimatedFrequency) && result.EstimatedFrequency > 0)
            {
                //freqCarrier = result.EstimatedFrequency;
                //CarryFreq = result.EstimatedFrequency;

                var tmpRes = result.FrequencyHistory.Where(o => o != 0).ToArray();
                var freq = Math.Round(tmpRes.OrderBy(o => o).Skip(2).Take(tmpRes.Count() - 4).Average() / 1e8) * 1e8;
                Trace.WriteLine($" CarryFreq:{freq})************************");
                freqCarrier = freq;
                CarryFreq = freq;
            }
            //freqCarrier = 1001500000;
            //================================================================================
            // STEP 1: 数字下变频 (Digital Down-Conversion)
            //================================================================================
            // 输入是 adcData，采样率是 fs_sys
            Boolean freqDownFlag = VSA.MixFreqDown(adcData, fs_sys, freqCarrier, out var dmodSignal);
            if (!freqDownFlag || dmodSignal == null)
            {
                return;
            }
            #region 粗符号速率估计
            Double[] dmodSignalI = dmodSignal.Select(o => o.Real).ToArray();
            Double[] dmodSignalQ = dmodSignal.Select(o => o.Imaginary).ToArray();
            int snapshotLen = 32768;
            int actualSnapshotLen = Math.Min(dmodSignalI.Length, snapshotLen);

            Double[] dmodI_snapshot = new Double[actualSnapshotLen];
            Double[] dmodQ_snapshot = new Double[actualSnapshotLen];
            Array.Copy(dmodSignalI, dmodI_snapshot, actualSnapshotLen);
            Array.Copy(dmodSignalQ, dmodQ_snapshot, actualSnapshotLen);

            Double coarseCutoff = Math.Min(1.0e9, fs_sys * 0.45);
            int coarseOrder = 60; // 阶数较低，追求速度

            double[] coarseIdealCoeffs = IdealLowPassCoefficients(coarseOrder, coarseCutoff, fs_sys);
            double[] coarseWindow = GenerateHanningWindow(coarseOrder + 1); // 或使用 Window.Hann 如果引入了 MathNet
            double[] coarseFirCoeffs = ApplyWindow(coarseIdealCoeffs, coarseWindow);
            // 3. 对快照进行滤波
            Double[] snapI_filtered = ConvolveMatlabFilter(dmodI_snapshot, coarseFirCoeffs);
            Double[] snapQ_filtered = ConvolveMatlabFilter(dmodQ_snapshot, coarseFirCoeffs);

            // 4. 8倍降采样 (快照)
            // 使用简单的索引抽取
            List<Complex> snap_decimated_list = new List<Complex>();
            for (int i = 0; i < snapI_filtered.Length; i += decimationFactor)
            {
                snap_decimated_list.Add(new Complex(snapI_filtered[i], snapQ_filtered[i]));
            }
            Complex[] snap_decimated = snap_decimated_list.ToArray();

            // 5. 调用估计器
            // 注意：采样率现在是 fs_sys / decimationFactor
            double estimatedRate = _symbolRateEstimator.Estimate(snap_decimated, fs_decimated);
            TryApplyEstimatedBaudRate(estimatedRate);
            #endregion
            //DebugUtils.DumpComplexDataToCsv(dmodSignal.ToArray(), @"D:\Matlab_VSA\MATLAB_0417\debug_ddc.csv");
            //================================================================================
            // STEP 2: 低通滤波 (Low-Pass Filtering)
            //================================================================================
            // 同样在 fs_sys (20 GHz) 下进行滤波
            Double cutoffFrequency = (1 + beta) * baudRate;
            //cutoffFrequency = Math.Min(cutoffFrequency, fs_sys * 0.45);
            //if (cutoffFrequency <= 0)
            //{
            //    Console.WriteLine("错误：低通截止频率无效。");
            //    return;
            //}
            int filterOrder = 80; // 滤波器阶数，需要为偶数。80阶对应81个系数。
                                  // 1. 生成理想低通滤波器系数
            double[] idealCoeffs = IdealLowPassCoefficients(filterOrder, cutoffFrequency, fs_sys);

            // 2. 生成汉宁窗
            double[] hanningWindow = GenerateHanningWindow(filterOrder + 1);

            // 3. 将窗函数应用到理想系数上，得到最终的FIR滤波器系数
            double[] firFilterCoeffs = ApplyWindow(idealCoeffs, hanningWindow);
            Double[] dmodSignalI_LowPass = ConvolveMatlabFilter(dmodSignalI, firFilterCoeffs);
            Double[] dmodSignalQ_LowPass = ConvolveMatlabFilter(dmodSignalQ, firFilterCoeffs);

            // 此时，dmodSignalI_LowPass 和 dmodSignalQ_LowPass 对应 MATLAB 的 imodI1 和 imodQ1
            // 临时组合一下方便导出
            //Complex[] lpfOutput = dmodSignalI_LowPass.Zip(dmodSignalQ_LowPass, (i, q) => new Complex(i, q)).ToArray();
            //// 【导出点 B】：把 lpfOutput 导出为 debug_lpf.csv
            //DebugUtils.DumpComplexDataToCsv(lpfOutput, @"D:\Matlab_VSA\MATLAB_0417\debug_lpf.csv");
            //================================================================================
            // STEP 3: 降采样 (Decimation by 8) - 新增的关键步骤
            //================================================================================
            // 目标: 复现 MATLAB 的 imodI = imodI1(1:8:end)
            // 使用LINQ的Where方法可以非常简洁地实现降采样
            // (sample, index) => index % decimationFactor == 0 的意思是：只保留那些索引号能被8整除的点
            Double[] decimatedI = dmodSignalI_LowPass
                                    .Where((sample, index) => index % decimationFactor == 0)
                                    .ToArray();

            Double[] decimatedQ = dmodSignalQ_LowPass
                                    .Where((sample, index) => index % decimationFactor == 0)
                                    .ToArray();

            //================================================================================
            // STEP 4: 组装最终的复基带信号
            //================================================================================
            // 将降采样后的I路和Q路数据组合成复数数组。
            Complex[] signalReceiver = decimatedI.Zip(decimatedQ, (i, q) => new Complex(i, q)).ToArray();

            // 现在，signalReceiver 的长度应该是 384000 / 8 = 48000，与 MATLAB 一致
            // 它的有效采样率也变成了 fs_sys / 8 = 2.5 GHz
            //DebugUtils.DumpComplexDataToCsv(signalReceiver, @"D:\Matlab_VSA\MATLAB_0417\debug_data.csv");
            // 1. 设置路径 (请修改为你刚才 MATLAB 导出的文件路径)
            //string csvPath = @"D:\Matlab_VSA\MATLAB_0417\golden_data_matlab.csv"; // 或者是 golden_data_qpsk.csv

            //if (!File.Exists(csvPath))
            //{
            //    Console.WriteLine("错误：找不到测试文件！");
            //    return;
            //}

            //// 2. 加载数据
            //Complex[] goldenData = DebugUtils.LoadFromCsv(csvPath);
            double symbolRate = _symbolRateEstimator.Estimate(signalReceiver, fs_sys / decimationFactor);
            TryApplyEstimatedBaudRate(symbolRate);

            // 将最终使用的符号率回写到模型，确保后续流程/界面一致使用估计值。
            if (hasEstimatedBaudRate)
            {
                SymbolRate = baudRate;
            }

            //--------------------------------------------------------------------------------
            // 验证点 1: 完成DDC、低通滤波和降采样
            //--------------------------------------------------------------------------------
            // 此时，C#中的 'signalReceiver' 变量的长度和内容应该都与MATLAB中的 'signalReceiver' 高度匹配。
            // 请重新生成 .csv 文件并用之前的MATLAB代码进行验证。
            //--------------------------------------------------------------------------------

            // *** 下一步将会在这里添加：重采样 ***
            int sps_after_resample = 16; // 我们固定的内部工作SPS
            double fs_receive = fs_sys / decimationFactor; // signalReceiver 当前采样率
            double fs_after_resample = baudRate * sps_after_resample; // 重采样后的目标采样率
            Complex[] data_after_resample; // 用于存放重采样结果
            if (Math.Abs(fs_receive - fs_after_resample) < 1.0) // 使用一个小的容差来比较浮点数
            {
                // 如果采样率几乎一样，直接跳过，不做任何处理
                data_after_resample = signalReceiver;
            }
            else
            {
                // 2a. 从文件加载Farrow滤波器系数
                //     将 'farrow_coeffs.txt' 文件放到您的C#项目可以访问的路径下
                //     例如，放在 "bin/Debug/netX.X/" 目录下，或者提供一个绝对路径
                //string farrowCoeffFilePath = @"D:\Matlab_VSA\MATLAB_0417\farrow_coeffs.txt"; ; // 或者 @"D:\YourProject\farrow_coeffs.txt"
                string farrowCoeffFilePath = Path.Combine(workingDirectory, "farrow_coeffs.txt");
                Double[,] farrowCoeffs = ReadMatrixFromFile(farrowCoeffFilePath);

                if (farrowCoeffs == null)
                {
                    Console.WriteLine("错误：Farrow系数加载失败，已终止重采样。");
                    // 如果系数加载失败，可以选择直接返回，或者用原始信号继续（可能会在后续步骤出错）
                    return;
                }

                // 2b. 分别对I路和Q路进行重采样 (这部分代码不变)
                VSA.ReSample(
                    signalReceiver.Select(c => c.Real).ToArray(),
                    fs_receive,
                    fs_after_resample,
                    farrowCoeffs,
                    out Double[]? resampledI
                );
                VSA.ReSample(
                    signalReceiver.Select(c => c.Imaginary).ToArray(),
                    fs_receive,
                    fs_after_resample,
                    farrowCoeffs,
                    out Double[]? resampledQ
                );

                if (resampledI == null || resampledQ == null)
                {
                    Console.WriteLine("错误：重采样执行失败。");
                    return;
                }

                // 2c. 组合重采样后的信号 (这部分代码不变)
                data_after_resample = resampledI.Zip(resampledQ, (i, q) => new Complex(i, q)).ToArray();
            }
            //================================================================================
            // STEP 5: 接收滤波 (匹配滤波)
            //================================================================================
            // 目标: 应用一个根升余弦(RRC)滤波器来最大化信噪比并抑制噪声。
            // 这对应MATLAB中的 rcosdesign 和 filter(h1,...) 部分。

            int sps = 16; // 我们当前的工作SPS (sps_after_resample)
            int span = 40; // 滤波器跨度，与MATLAB一致
                           // beta (滚降系数) 在STEP 0中已经定义

            // 1. 设计RRC滤波器系数
            //    调用 RootRaisedCosFilter.cs 中的函数
            //    注意：这个函数返回的是 IEnumerable<Double>，我们需要将其转为数组
            Double[] rrcFilterCoeffs = VSA.RootRaisedCosFilter(beta, span, sps).ToArray();

            // 2. 应用滤波器
            //    输入信号是 signalReceiver，滤波器系数是 rrcFilterCoeffs
            //    我们需要对复数信号进行滤波，因此需要一个复数卷积的实现。
            //    假设我们有一个Complex版本的Convolve函数。

            //    分别对I路和Q路进行滤波
            Double[] filteredI = ConvolveMatlabFilter(
                data_after_resample.Select(c => c.Real).ToArray(),
                rrcFilterCoeffs
            );

            Double[] filteredQ = ConvolveMatlabFilter(
                data_after_resample.Select(c => c.Imaginary).ToArray(),
                rrcFilterCoeffs
            );

            // 3. 组合滤波后的信号
            //    这个 rxSample 变量就对应MATLAB中的 rxSample
            int rrcGroupDelay = (span * sps) / 2;
            Complex[] rxSample = filteredI.Zip(filteredQ, (i, q) => new Complex(i, q)).Skip(rrcGroupDelay).ToArray();

            //--------------------------------------------------------------------------------
            // 验证点 2: 完成接收滤波
            //--------------------------------------------------------------------------------
            // 此时，C#中的 'rxSample' 变量应该与您MATLAB工作空间中的 'rxSample' 变量相对应。
            // (注意：MATLAB中可能需要调整discard的长度来与C#的'Same'模式对齐)
            // 您可以再次保存文件进行对比。
            //var linesToSave_rxSample = rxSample.Select(c => $"{c.Real.ToString(System.Globalization.CultureInfo.InvariantCulture)},{c.Imaginary.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            //System.IO.File.WriteAllLines("cs_rxSample.csv", linesToSave_rxSample);
            //--------------------------------------------------------------------------------

            // *** 下一步：定时同步归一化预处理 ***
            //================================================================================
            // STEP 6: 定时同步前归一化 (Normalization)
            //================================================================================
            // 目标: 将信号幅度调整到一个稳定的范围，为后续的定时同步做准备。
            // 这对应MATLAB中的 "timing sync normalization" 部分。

            // 1. 定义用于寻找最大幅度的窗口大小
            int timing_maxnum = Math.Min(3000, Math.Max(512, rxSample.Length / 10));

            // 检查 rxSample 的长度是否足够
            if (rxSample.Length <= timing_maxnum)
            {
                // 如果信号太短，无法执行此步骤，可以在此报错或直接返回
                Console.WriteLine("错误：信号长度不足以进行归一化。");
                return;
            }

            // 2. 截取信号的前 timing_maxnum 个点用于分析
            Complex[] analysisWindow = rxSample.Take(timing_maxnum).ToArray();

            // 3. 计算窗口内的最大平方幅度 (I² + Q²)
            //    c.Magnitude * c.Magnitude 是最直接的计算方式
            double maxSquaredAmplitude = analysisWindow.Select(c => c.Magnitude * c.Magnitude).Max();

            // 4. 计算归一化因子（最大幅度）
            double normalizationFactor = Math.Sqrt(maxSquaredAmplitude);

            // 5. 应用归一化并丢弃用于分析的窗口部分
            Complex[] rxSample_nor;

            if (normalizationFactor > 1e-9) // 避免除以零
            {
                rxSample_nor = rxSample
                                    .Skip(timing_maxnum) // 丢弃前3000个点
                                    .Select(c => c / normalizationFactor) // 将剩余的点逐个进行归一化
                                    .ToArray();
            }
            else
            {
                // 如果归一化因子接近零（信号几乎为零），则直接跳过归一化，只丢弃窗口
                rxSample_nor = rxSample.Skip(timing_maxnum).ToArray();
            }

            //================================================================================
            // STEP 7: 定时同步预滤波 (Prefiltering for TED)
            //================================================================================
            // 目标: 生成用于定时误差检测器(TED)的辅助信号。

            // 1. 定义预滤波器的参数
            //    MATLAB脚本中使用的是 span/4
            int prefilterSpan = span / 4;

            // 2. 调用 hPrefilter.cs 中的函数来设计滤波器系数
            Boolean prefilterValid = VSA.hPrefilter(beta, sps, prefilterSpan, out Double[]? hprefilterCoeffs);

            if (!prefilterValid || hprefilterCoeffs == null)
            {
                Console.WriteLine("错误：预滤波器系数生成失败。");
                return;
            }

            // 3. 应用预滤波器
            //    输入是归一化后的信号 rxSample_nor
            //    使用我们之前写的 ConvolveMatlabFilter 来精确匹配 MATLAB 的 filter() 行为
            Double[] prefilteredI = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Real).ToArray(), hprefilterCoeffs);
            Double[] prefilteredQ = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Imaginary).ToArray(), hprefilterCoeffs);

            // 4. 组合成复数信号
            Complex[] rxSamplePrefilter = prefilteredI.Zip(prefilteredQ, (i, q) => new Complex(i, q)).ToArray();

            //================================================================================
            // STEP 8: 计算定时同步环路滤波器系数
            //================================================================================
            // 目标: 为 TimingSynchronizer 提供所需的比例和积分增益。
            // 我们将调用 VSA.LoopFilter 来计算这些系数。
            Double symbolsynbandWidth = 0.008; // 归一化环路带宽，对应 BT
            Double kesi = Math.Sqrt(2) / 2.0;   // 阻尼系数
                                                // 根据 VsaPllUseScne 的定义，为 MQAM symbol syn 选择正确的鉴相器增益模型
            Int32 M = _FormatOpt switch
            {
                VsaFormatOpt.BPSK => 2,
                VsaFormatOpt.QPSK => 4,
                VsaFormatOpt.PSK8 => 8,
                VsaFormatOpt.QAM16 => 16,
                VsaFormatOpt.QAM64 => 64,
                VsaFormatOpt.QAM128 => 128,
                VsaFormatOpt.QAM256 => 256,
                VsaFormatOpt.QAM1024 => 1024,
                _ => 16,
            };                                    // 调制阶数
            ModulationFormat modFormat;

            switch (_FormatOpt)
            {
                case VsaFormatOpt.BPSK:
                case VsaFormatOpt.QPSK:
                case VsaFormatOpt.PSK8:
                    modFormat = ModulationFormat.PSK;
                    break;
                case VsaFormatOpt.QAM16:
                case VsaFormatOpt.QAM64:
                case VsaFormatOpt.QAM128:
                case VsaFormatOpt.QAM256:
                case VsaFormatOpt.QAM1024:
                    modFormat = ModulationFormat.QAM;
                    break;
                default:
                    // 提供一个默认值或抛出异常，以处理未预期的格式
                    modFormat = ModulationFormat.QAM;
                    // throw new NotSupportedException($"Modulation format {_FormatOpt} is not supported.");
                    break;
            }

            // 调用系数生成函数
            Boolean symbolSynLoopFilterValid = TimeLoopFilter(symbolsynbandWidth, kesi, sps, beta, M, out Double[]? symbolsyncoeff);

            if (!symbolSynLoopFilterValid || symbolsyncoeff == null || symbolsyncoeff.Length < 2)
            {
                Console.WriteLine("错误：定时同步环路滤波器系数生成失败。");
                return;
            }

            // MATLAB 脚本中传入的是 -c3, -c4。LoopFilter 返回的 coeff[0] 对应 c3, coeff[1] 对应 c4。
            double c1_loop = -symbolsyncoeff[0];
            double c2_loop = -symbolsyncoeff[1];

            //================================================================================
            // STEP 9: 执行定时同步
            //================================================================================
            // 目标: 使用新的 TimingSynchronizer 类来恢复出符号点。

            // 1. 准备输入数据
            //    在MATLAB脚本中，为了处理边界效应，对输入信号进行了截断：
            //    rxSamplePrefilter_short = rxSamplePrefilter(order+1:testnum_sync+order);
            //    rxSample_short = rxSample_nor(order/2+1:testnum_sync+order/2);
            //    这里的 order 是160。这是因为预滤波器 hprefilter 的长度是 161 (span/4 * sps + 1 = 10 * 16 + 1)。
            //    群延迟是 (161-1)/2 = 80。
            //    `filter`函数又额外引入了80的延迟。总共大约160。
            //    
            //    我们的 ConvolveMatlabFilter 也引入了 80 的延迟。
            //    为了精确匹配，我们也需要进行类似的截断。

            int order = (prefilterSpan * sps); // 10 * 16 = 160
            int testnum_sync = rxSample_nor.Length - order;

            if (rxSamplePrefilter.Length <= order || rxSample_nor.Length <= (order / 2) || testnum_sync <= 0)
            {
                Console.WriteLine("错误：信号长度不足以进行定时同步处理。");
                return;
            }

            // 截取预滤波后的信号，跳过前面 order 个点
            ReadOnlySpan<Complex> prefiltered_short = rxSamplePrefilter.AsSpan().Slice(order, testnum_sync);
            // 截取原始信号，跳过前面 order/2 个点
            ReadOnlySpan<Complex> original_short = rxSample_nor.AsSpan().Slice(order / 2, testnum_sync);

            // 2. 实例化 TimingSynchronizer
            //    我们暂时让捕获和跟踪系数使用同一套
            var synchronizer = new TimingSynchronizer(
                sps: sps,
                c1: c1_loop,
                c2: c2_loop,
                c3: c1_loop, // 跟踪阶段也用相同的系数
                c4: c2_loop, // 跟踪阶段也用相同的系数
                beta: beta,
                modFormat // 因为我们是16QAM
            );

            // 3. 执行同步处理
            var (synchronizedSymbols, timingErrors) = synchronizer.SynchronizeBlock(
                prefiltered_short,
                original_short
            );

            //================================================================================
            // STEP 10: 计算载波同步环路滤波器系数
            //================================================================================
            // 目标: 使用新创建的 CalculateCarrierLoopCoefficients 函数来生成系数。

            // 从MATLAB脚本中获取载波环路参数
            double carriersynbandWidth = 0.05; // 对应 BT_car
            int carrierSps = 1; // 载波环路在每个符号点上操作一次，所以SPS是1

            // 为了与已验证稳定的旧流程一致，这里使用 VSA.LoopFilter + VSA.CarrierSyncDD。
            //Boolean carrierSynLoopFilterValid = VSA.LoopFilter(
            //    carriersynbandWidth,
            //    kesi,
            //    carrierSps,
            //    beta,
            //    M,
            //    VsaPllUseScne.carriersyn,
            //    out Double[]? carriersyncoeff
            //);
            //if (!carrierSynLoopFilterValid || carriersyncoeff == null || carriersyncoeff.Length < 2)
            //{
            //    Console.WriteLine("错误：载波同步环路滤波器系数生成失败。");
            //    return;
            //}
            (double c5, double c6) = CalculateCarrierLoopCoefficients(carriersynbandWidth, kesi, carrierSps, M);

            int discardnum_after_timing = Math.Min(1000, Math.Max(100, synchronizedSymbols.Count / 10));
            if (synchronizedSymbols.Count <= discardnum_after_timing)
            {
                Console.WriteLine("错误：定时同步后的符号点数量不足以进行载波同步。");
                return;
            }
            Complex[] carrierSyncInput = synchronizedSymbols.Skip(discardnum_after_timing).ToArray();
            double k0 = 1; // NCO增益，与MATLAB一致
            Complex[] normalizedSignal = CarrierSynchronizer.NormalizeInputSignal(carrierSyncInput, 1);
            Complex[] refConstellation = VSA.GetStandardConstellation(_FormatOpt, M);
            CarrierSynchronizer carrierSynchronizer = new CarrierSynchronizer(
                loopGainKp: c5,
                loopGainKi: c6,
                ncoGain: k0,
                referenceConstellation: refConstellation,
                modulationFormat: modFormat.ToString(), // 根据您的设置
                modulationOrder: M
            );
            // 4. 执行同步处理
            Complex[] synchronizedOutput = carrierSynchronizer.Process(normalizedSignal, out double[] phaseEstimates, out double[] phaseErrors);
            synchronizedOutput = synchronizedOutput.Skip(100).ToArray();
            //var linesToSave_carrier = synchronizedOutput.Select(c => $"{c.Real.ToString(System.Globalization.CultureInfo.InvariantCulture)},{c.Imaginary.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            //System.IO.File.WriteAllLines("cs_carrierSynchronized.csv", linesToSave_carrier);
            #region 星座图
            //星座图
            Double[] carrierSynI = synchronizedOutput.Real().Select(o => o * 0.01).ToArray();
            Double[] carrierSynQ = synchronizedOutput.Imaginary().Select(o => o * 0.01).ToArray();

            Double[,] constellationXY = new Double[2, carrierSynI.Length];
            Buffer.BlockCopy(carrierSynI.ToArray(), 0, constellationXY, 0, carrierSynI.Count() * sizeof(Double));
            Buffer.BlockCopy(carrierSynQ.ToArray(), 0, constellationXY, carrierSynI.Count() * sizeof(Double), carrierSynQ.Count() * sizeof(Double));
            OccupierBuffer.Default.Provide("Constellation()", new MathExt.Vector(constellationXY, "", "", 1.0, 1.0));
            #endregion
            #region EVM
            Int32 synchronizedOutputSize = synchronizedOutput.Length;
            Complex[] normalizedSignalEvm = NormalizeSignal(synchronizedOutput, M, _FormatOpt);
            var evmCarrier = normalizedSignalEvm.Take(synchronizedOutputSize < 500 ? synchronizedOutputSize : 500);
            VSA.PhaseDiscParam phaseDiscParam = new();
            var evmCarrierArray = evmCarrier.ToArray();
            Complex[] idealSignal = new Complex[evmCarrier.Count()];
            for (int i = 0; i < evmCarrier.Count(); i++)
            {
                phaseDiscParam = VSA.DDPhaseDiscQam(evmCarrierArray[i], M, _FormatOpt);
                idealSignal[i] = phaseDiscParam.CarrierSynSign;
            }
            //Double sumOfSquares = idealSignal.Select(c => Math.Pow(c.Real, 2) + Math.Pow(c.Imaginary, 2)).Sum();
            //Double meanOfSquares = sumOfSquares / idealSignal.Length;
            var standardConstellation = VSA.GetStandardConstellation(_FormatOpt, M);
            double meanOfSquares = standardConstellation.Select(c => Math.Pow(c.Real, 2) + Math.Pow(c.Imaginary, 2)).Average();
            Double res = Math.Sqrt(meanOfSquares);
            IEnumerable<Double> EVM = idealSignal.Zip(evmCarrierArray, (o, w) => ((o - w).Magnitude / res) * 100);
            OccupierBuffer.Default.Provide("VSAEvm", new MathExt.Vector(EVM.Select(o => o), "", "", 1.0, 1.0));
            Double evmRms = Math.Sqrt(EVM.Average(x => x * x)) / 10;
            Double evmMax = EVM.Max() / 10;
            Double evmMin = EVM.Min() / 10;

            if (!ErrParamTable.ContainsKey("EVM(%)"))
            {
                ErrParamTable["EVM(%)"] = new ParamStatistics
                {
                    Mean = evmRms,
                    Max = evmMax,
                    Min = evmMin
                };
            }
            else
            {
                var stats = ErrParamTable["EVM(%)"];
                stats.Mean = evmRms;
                stats.Max = evmMax;
                stats.Min = evmMin;
            }
            static Complex[] NormalizeSignal(Complex[] signal, int M, VsaFormatOpt formatOpt)
            {
                bool isQAM = formatOpt == VsaFormatOpt.QAM16 || formatOpt == VsaFormatOpt.QAM64
                            || formatOpt == VsaFormatOpt.QAM128 || formatOpt == VsaFormatOpt.QAM256;
                if (isQAM)
                {
                    double normalRefPoint = 0.5 * Math.Sqrt(2) * (Math.Sqrt(M) - 2) / (Math.Sqrt(M) - 1);
                    var filteredData = signal.Where(c => c.Real > normalRefPoint && c.Imaginary > normalRefPoint).ToArray();
                    if (filteredData.Length > 0)
                    {
                        double averageValue = filteredData.Average(c => c.Magnitude);
                        double scaleFactor = (Math.Sqrt(M) - 1) * Math.Sqrt(2) / averageValue;
                        return signal.Select(c => c * scaleFactor).ToArray();
                    }
                    else
                    {
                        double averageValue = signal.Average(c => c.Magnitude);
                        double scaleFactor = (Math.Sqrt(M) - 1) * Math.Sqrt(2) / averageValue;
                        return signal.Select(c => c * scaleFactor).ToArray();
                    }
                }
                else
                {
                    double averageValue = signal.Average(c => c.Magnitude);
                    return signal.Select(c => c / averageValue).ToArray();
                }
            }
            // 相位误差图
            IEnumerable<Double> PhaseErr = evmCarrier.Zip(idealSignal, (r, i) => r.Phase - i.Phase);
            OccupierBuffer.Default.Provide("PhaseErrTime()", new MathExt.Vector(PhaseErr.Select(o => o * 0.1), "", "", 1.0, 1.0));

            // 幅度误差图
            IEnumerable<Double> AmplErr = evmCarrier.Zip(idealSignal, (r, i) => r.Magnitude - i.Magnitude);
            OccupierBuffer.Default.Provide("AmplErrTime()", new MathExt.Vector(AmplErr.Select(o => o * 0.1), "", "", 1.0, 1.0));
            errparam = ErrCaculate.CalcParam(evmCarrier, idealSignal, EVM, PhaseErr, AmplErr);
            if (errparam == null)
            {
                return;
            }
            foreach (var prop in errparam.GetType().GetProperties())
            {
                if (ErrParamTable.ContainsKey(prop.Name))
                {
                    ErrParamTable[prop.Name].Value = (Double)prop.GetValue(errparam)!;
                }
                else
                {
                    ErrParamTable.Add(prop.Name, new ParamStatistics
                    {
                        Value = (Double)prop.GetValue(errparam)!
                    });
                }
            }
            #endregion
            #region IQ时域图
            OccupierBuffer.Default.Provide("IChartIDiagram", new MathExt.Vector(carrierSynI.Take(500).Select(o => o * 100), "", "", 1.0 / (156.25e6), 1.0));
            OccupierBuffer.Default.Provide("IChartQDiagram", new MathExt.Vector(carrierSynQ.Take(500).Select(o => o * 100), "", "", 1.0 / (156.25e6), 1.0));
            #endregion
            #region 矢量图
            //double phaseCorrection = -phaseEstimates.LastOrDefault();
            //Complex correctionPhasor = Complex.Exp(Complex.ImaginaryOne * phaseCorrection);
            //// 3. 选择数据源：使用匹配滤波后的 rxSample，并应用相位校正
            //int vectorPointsToShow = Math.Min(rxSample.Length, 8000);

            //// 关键步骤：在选择数据点的同时，应用旋转校正
            ////var vectorData = rxSample
            ////                    .Take(vectorPointsToShow)
            ////                    .Select(c => c * correctionPhasor); // 对每个点进行旋转
            ////                                                        // 4. 准备数据格式
            //var vectorData = rxSample
            //                    .Take(vectorPointsToShow)
            //                    ;
            //Double[] vectorI = vectorData.Select(c => c.Real).ToArray();
            //Double[] vectorQ = vectorData.Select(c => c.Imaginary).ToArray();
            //Double[,] vectorXY = new Double[2, vectorPointsToShow];
            //Buffer.BlockCopy(vectorI, 0, vectorXY, 0, vectorPointsToShow * sizeof(Double));
            //Buffer.BlockCopy(vectorQ, 0, vectorXY, vectorPointsToShow * sizeof(Double), vectorPointsToShow * sizeof(Double));

            //// 5. 发送数据给UI
            //OccupierBuffer.Default.Provide("VectorGph()", new MathExt.Vector(vectorXY, "", "", 1.0, 1.0));
            Complex[] idealWaveform = ReconstructIdealWaveform(synchronizedOutput, sps, beta, span);

            int pointsToShow = Math.Min(idealWaveform.Length, 4000); // 显示的点数
            var vectorData = idealWaveform.Take(pointsToShow);

            Double[] vectorI = vectorData.Select(c => c.Real).ToArray();
            Double[] vectorQ = vectorData.Select(c => c.Imaginary).ToArray();

            Double[,] vectorXY = new Double[2, pointsToShow];
            Buffer.BlockCopy(vectorI, 0, vectorXY, 0, pointsToShow * sizeof(Double));
            Buffer.BlockCopy(vectorQ, 0, vectorXY, pointsToShow * sizeof(Double), pointsToShow * sizeof(Double));

            OccupierBuffer.Default.Provide("VectorGph()", new MathExt.Vector(vectorXY, "", "", 1.0, 1.0));
            #endregion
            #region 眼图
            //var iEyeMatrix = GenerateVsaEyeDiagramMatrix(vectorI, sps, 4);
            //var qEyeMatrix = GenerateVsaEyeDiagramMatrix(vectorQ, sps, 4);
            //if (iEyeMatrix.GetLength(0) > 0)
            //{
            //    // 注意：您的UI框架似乎需要旋转后的矩阵，并且可能需要垂直翻转
            //    // EyeGraphGenerator.GetEyeMartix() 内部有 .Reverse() 和 .RotateAnticlockwise()
            //    // 我们在这里模拟这个过程以确保兼容性
            //    iEyeMatrix = iEyeMatrix.Reverse(false, true).RotateAnticlockwise();
            //    qEyeMatrix = qEyeMatrix.Reverse(false, true).RotateAnticlockwise();

            //    // 计算采样间隔
            //    double eyeSampleInterval = (1 / fs_after_resample) / 1600; // 2个UI

            //    OccupierBuffer.Default.Provide("VSAIEye()", new MathExt.Vector(iEyeMatrix, "s", "V", eyeSampleInterval, 1.0));
            //    OccupierBuffer.Default.Provide("VSAQEye()", new MathExt.Vector(qEyeMatrix, "s", "V", eyeSampleInterval, 1.0));
            //}
            #endregion
        }
        //public void Run_New(Double[] adcData, Double sampleRate_original)
        //{
        //    int maxDataPoints = 300000;
        //    if (adcData != null && adcData.Length > maxDataPoints)
        //    {
        //        Double[] truncatedData = new Double[maxDataPoints];
        //        Array.Copy(adcData, truncatedData, maxDataPoints); // 复制前30k点
        //        adcData = truncatedData;
        //    }
        //    // 1. 定义数据文件的完整路径
        //    //    注意：在C#中，路径里的反斜杠'\'需要写成'\\'，或者在字符串前加上'@'符号。
        //    //string dataFilePath = @"D:\Matlab_VSA\MATLAB_0417\chanOut_data.txt";
        //    // 获取当前工作目录
        //    string workingDirectory = Directory.GetCurrentDirectory();
        //    // 拼接文件路径
        //    string dataFilePath = Path.Combine(workingDirectory, "chanOut_data.txt");


        //    // 2. 调用您已有的辅助函数来读取文件
        //    //adcData = ReadDataFromFile(dataFilePath);
        //    //================================================================================
        //    // STEP 0: 参数设置
        //    //================================================================================
        //    Double baudRate = SymbolRate;
        //    Double beta = RollOffFactor;
        //    Double freqCarrier = CarryFreq;
        //    //Double freqCarrier = 1e9;

        //    // 注意：MATLAB中是在 Fs_sys (20 GHz) 下处理然后降采样的。
        //    // 我们传入的 adcData 就是 Fs_sys 下的数据。
        //    Double fs_sys = 20e9; // 对应 MATLAB 的 Fs_sys

        //    //================================================================================
        //    // STEP 1: 数字下变频 (Digital Down-Conversion)
        //    //================================================================================
        //    // 输入是 adcData (chanOut)，采样率是 fs_sys (20 GHz)
        //    Boolean freqDownFlag = VSA.MixFreqDown(adcData, fs_sys, freqCarrier, out var dmodSignal);
        //    if (!freqDownFlag || dmodSignal == null)
        //    {
        //        return;
        //    }

        //    //================================================================================
        //    // STEP 2: 低通滤波 (Low-Pass Filtering)
        //    //================================================================================
        //    // 同样在 fs_sys (20 GHz) 下进行滤波
        //    Double cutoffFrequency = (1 + beta) * baudRate;
        //    int filterOrder = 80; // 滤波器阶数，需要为偶数。80阶对应81个系数。
        //                          // 1. 生成理想低通滤波器系数
        //    double[] idealCoeffs = IdealLowPassCoefficients(filterOrder, cutoffFrequency, fs_sys);

        //    // 2. 生成汉宁窗
        //    double[] hanningWindow = GenerateHanningWindow(filterOrder + 1);

        //    // 3. 将窗函数应用到理想系数上，得到最终的FIR滤波器系数
        //    double[] firFilterCoeffs = ApplyWindow(idealCoeffs, hanningWindow);
        //    Double[] dmodSignalI = dmodSignal.Select(o => o.Real).ToArray();
        //    Double[] dmodSignalQ = dmodSignal.Select(o => o.Imaginary).ToArray();

        //    Double[] dmodSignalI_LowPass = ConvolveMatlabFilter(dmodSignalI, firFilterCoeffs);
        //    Double[] dmodSignalQ_LowPass = ConvolveMatlabFilter(dmodSignalQ, firFilterCoeffs);

        //    // 此时，dmodSignalI_LowPass 和 dmodSignalQ_LowPass 对应 MATLAB 的 imodI1 和 imodQ1

        //    //================================================================================
        //    // STEP 3: 降采样 (Decimation by 8) - 新增的关键步骤
        //    //================================================================================
        //    // 目标: 复现 MATLAB 的 imodI = imodI1(1:8:end)
        //    int decimationFactor = 8;

        //    // 使用LINQ的Where方法可以非常简洁地实现降采样
        //    // (sample, index) => index % decimationFactor == 0 的意思是：只保留那些索引号能被8整除的点
        //    Double[] decimatedI = dmodSignalI_LowPass
        //                            .Where((sample, index) => index % decimationFactor == 0)
        //                            .ToArray();

        //    Double[] decimatedQ = dmodSignalQ_LowPass
        //                            .Where((sample, index) => index % decimationFactor == 0)
        //                            .ToArray();

        //    //================================================================================
        //    // STEP 4: 组装最终的复基带信号
        //    //================================================================================
        //    // 将降采样后的I路和Q路数据组合成复数数组。
        //    Complex[] signalReceiver = decimatedI.Zip(decimatedQ, (i, q) => new Complex(i, q)).ToArray();

        //    // 现在，signalReceiver 的长度应该是 384000 / 8 = 48000，与 MATLAB 一致
        //    // 它的有效采样率也变成了 fs_sys / 8 = 2.5 GHz

        //    //--------------------------------------------------------------------------------
        //    // 验证点 1: 完成DDC、低通滤波和降采样
        //    //--------------------------------------------------------------------------------
        //    // 此时，C#中的 'signalReceiver' 变量的长度和内容应该都与MATLAB中的 'signalReceiver' 高度匹配。
        //    // 请重新生成 .csv 文件并用之前的MATLAB代码进行验证。
        //    //--------------------------------------------------------------------------------

        //    // *** 下一步将会在这里添加：重采样 ***
        //    double fs_receive = 2.5e9; // signalReceiver 当前的采样率
        //    int sps_after_resample = 16; // 我们固定的内部工作SPS
        //    double fs_after_resample = baudRate * sps_after_resample; // 重采样后的目标采样率
        //    Complex[] data_after_resample; // 用于存放重采样结果
        //    if (Math.Abs(fs_receive - fs_after_resample) < 1.0) // 使用一个小的容差来比较浮点数
        //    {
        //        // 如果采样率几乎一样，直接跳过，不做任何处理
        //        data_after_resample = signalReceiver;
        //    }
        //    else
        //    {
        //        // 2a. 从文件加载Farrow滤波器系数
        //        //     将 'farrow_coeffs.txt' 文件放到您的C#项目可以访问的路径下
        //        //     例如，放在 "bin/Debug/netX.X/" 目录下，或者提供一个绝对路径
        //        //string farrowCoeffFilePath = @"D:\Matlab_VSA\MATLAB_0417\farrow_coeffs.txt"; ; // 或者 @"D:\YourProject\farrow_coeffs.txt"
        //        string farrowCoeffFilePath = Path.Combine(workingDirectory, "farrow_coeffs.txt");
        //        Double[,] farrowCoeffs = ReadMatrixFromFile(farrowCoeffFilePath);

        //        if (farrowCoeffs == null)
        //        {
        //            Console.WriteLine("错误：Farrow系数加载失败，已终止重采样。");
        //            // 如果系数加载失败，可以选择直接返回，或者用原始信号继续（可能会在后续步骤出错）
        //            return;
        //        }

        //        // 2b. 分别对I路和Q路进行重采样 (这部分代码不变)
        //        VSA.ReSample(
        //            signalReceiver.Select(c => c.Real).ToArray(),
        //            fs_receive,
        //            fs_after_resample,
        //            farrowCoeffs,
        //            out Double[]? resampledI
        //        );
        //        VSA.ReSample(
        //            signalReceiver.Select(c => c.Imaginary).ToArray(),
        //            fs_receive,
        //            fs_after_resample,
        //            farrowCoeffs,
        //            out Double[]? resampledQ
        //        );

        //        if (resampledI == null || resampledQ == null)
        //        {
        //            Console.WriteLine("错误：重采样执行失败。");
        //            return;
        //        }

        //        // 2c. 组合重采样后的信号 (这部分代码不变)
        //        data_after_resample = resampledI.Zip(resampledQ, (i, q) => new Complex(i, q)).ToArray();
        //    }
        //    //================================================================================
        //    // STEP 5: 接收滤波 (匹配滤波)
        //    //================================================================================
        //    // 目标: 应用一个根升余弦(RRC)滤波器来最大化信噪比并抑制噪声。
        //    // 这对应MATLAB中的 rcosdesign 和 filter(h1,...) 部分。

        //    int sps = 16; // 我们当前的工作SPS (sps_after_resample)
        //    int span = 40; // 滤波器跨度，与MATLAB一致
        //                   // beta (滚降系数) 在STEP 0中已经定义

        //    // 1. 设计RRC滤波器系数
        //    //    调用 RootRaisedCosFilter.cs 中的函数
        //    //    注意：这个函数返回的是 IEnumerable<Double>，我们需要将其转为数组
        //    Double[] rrcFilterCoeffs = VSA.RootRaisedCosFilter(beta, span, sps).ToArray();

        //    // 2. 应用滤波器
        //    //    输入信号是 signalReceiver，滤波器系数是 rrcFilterCoeffs
        //    //    我们需要对复数信号进行滤波，因此需要一个复数卷积的实现。
        //    //    假设我们有一个Complex版本的Convolve函数。

        //    //    分别对I路和Q路进行滤波
        //    Double[] filteredI = ConvolveMatlabFilter(
        //        data_after_resample.Select(c => c.Real).ToArray(),
        //        rrcFilterCoeffs
        //    );

        //    Double[] filteredQ = ConvolveMatlabFilter(
        //        data_after_resample.Select(c => c.Imaginary).ToArray(),
        //        rrcFilterCoeffs
        //    );

        //    // 3. 组合滤波后的信号
        //    //    这个 rxSample 变量就对应MATLAB中的 rxSample
        //    Complex[] rxSample = filteredI.Zip(filteredQ, (i, q) => new Complex(i, q)).Skip(640).ToArray();

        //    //--------------------------------------------------------------------------------
        //    // 验证点 2: 完成接收滤波
        //    //--------------------------------------------------------------------------------
        //    // 此时，C#中的 'rxSample' 变量应该与您MATLAB工作空间中的 'rxSample' 变量相对应。
        //    // (注意：MATLAB中可能需要调整discard的长度来与C#的'Same'模式对齐)
        //    // 您可以再次保存文件进行对比。
        //    //var linesToSave_rxSample = rxSample.Select(c => $"{c.Real.ToString(System.Globalization.CultureInfo.InvariantCulture)},{c.Imaginary.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        //    //System.IO.File.WriteAllLines("cs_rxSample.csv", linesToSave_rxSample);
        //    //--------------------------------------------------------------------------------

        //    // *** 下一步：定时同步归一化预处理 ***
        //    //================================================================================
        //    // STEP 6: 定时同步前归一化 (Normalization)
        //    //================================================================================
        //    // 目标: 将信号幅度调整到一个稳定的范围，为后续的定时同步做准备。
        //    // 这对应MATLAB中的 "timing sync normalization" 部分。

        //    // 1. 定义用于寻找最大幅度的窗口大小
        //    int timing_maxnum = 3000;

        //    // 检查 rxSample 的长度是否足够
        //    if (rxSample.Length <= timing_maxnum)
        //    {
        //        // 如果信号太短，无法执行此步骤，可以在此报错或直接返回
        //        Console.WriteLine("错误：信号长度不足以进行归一化。");
        //        return;
        //    }

        //    // 2. 截取信号的前 timing_maxnum 个点用于分析
        //    Complex[] analysisWindow = rxSample.Take(timing_maxnum).ToArray();

        //    // 3. 计算窗口内的最大平方幅度 (I² + Q²)
        //    //    c.Magnitude * c.Magnitude 是最直接的计算方式
        //    double maxSquaredAmplitude = analysisWindow.Select(c => c.Magnitude * c.Magnitude).Max();

        //    // 4. 计算归一化因子（最大幅度）
        //    double normalizationFactor = Math.Sqrt(maxSquaredAmplitude);

        //    // 5. 应用归一化并丢弃用于分析的窗口部分
        //    Complex[] rxSample_nor;

        //    if (normalizationFactor > 1e-9) // 避免除以零
        //    {
        //        rxSample_nor = rxSample
        //                            .Skip(timing_maxnum) // 丢弃前3000个点
        //                            .Select(c => c / normalizationFactor) // 将剩余的点逐个进行归一化
        //                            .ToArray();
        //    }
        //    else
        //    {
        //        // 如果归一化因子接近零（信号几乎为零），则直接跳过归一化，只丢弃窗口
        //        rxSample_nor = rxSample.Skip(timing_maxnum).ToArray();
        //    }

        //    //================================================================================
        //    // STEP 7: 定时同步预滤波 (Prefiltering for TED)
        //    //================================================================================
        //    // 目标: 生成用于定时误差检测器(TED)的辅助信号。

        //    // 1. 定义预滤波器的参数
        //    //    MATLAB脚本中使用的是 span/4
        //    int prefilterSpan = span / 4;

        //    // 2. 调用 hPrefilter.cs 中的函数来设计滤波器系数
        //    Boolean prefilterValid = VSA.hPrefilter(beta, sps, prefilterSpan, out Double[]? hprefilterCoeffs);

        //    if (!prefilterValid || hprefilterCoeffs == null)
        //    {
        //        Console.WriteLine("错误：预滤波器系数生成失败。");
        //        return;
        //    }

        //    // 3. 应用预滤波器
        //    //    输入是归一化后的信号 rxSample_nor
        //    //    使用我们之前写的 ConvolveMatlabFilter 来精确匹配 MATLAB 的 filter() 行为
        //    Double[] prefilteredI = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Real).ToArray(), hprefilterCoeffs);
        //    Double[] prefilteredQ = ConvolveMatlabFilter(rxSample_nor.Select(c => c.Imaginary).ToArray(), hprefilterCoeffs);

        //    // 4. 组合成复数信号
        //    Complex[] rxSamplePrefilter = prefilteredI.Zip(prefilteredQ, (i, q) => new Complex(i, q)).ToArray();

        //    //================================================================================
        //    // STEP 8: 计算定时同步环路滤波器系数
        //    //================================================================================
        //    // 目标: 为 TimingSynchronizer 提供所需的比例和积分增益。
        //    // 我们将调用 VSA.LoopFilter 来计算这些系数。
        //    Double symbolsynbandWidth = 0.008; // 归一化环路带宽，对应 BT
        //    Double kesi = Math.Sqrt(2) / 2.0;   // 阻尼系数
        //                                        // 根据 VsaPllUseScne 的定义，为 MQAM symbol syn 选择正确的鉴相器增益模型
        //    Int32 M = _FormatOpt switch
        //    {
        //        VsaFormatOpt.BPSK => 2,
        //        VsaFormatOpt.QPSK => 4,
        //        VsaFormatOpt.PSK8 => 8,
        //        VsaFormatOpt.QAM16 => 16,
        //        VsaFormatOpt.QAM64 => 64,
        //        VsaFormatOpt.QAM128 => 128,
        //        VsaFormatOpt.QAM256 => 256,
        //        VsaFormatOpt.QAM1024 => 1024,
        //        _ => 16,
        //    };                                    // 调制阶数
        //    ModulationFormat modFormat;

        //    switch (_FormatOpt)
        //    {
        //        case VsaFormatOpt.BPSK:
        //        case VsaFormatOpt.QPSK:
        //        case VsaFormatOpt.PSK8:
        //            modFormat = ModulationFormat.PSK;
        //            break;
        //        case VsaFormatOpt.QAM16:
        //        case VsaFormatOpt.QAM64:
        //        case VsaFormatOpt.QAM128:
        //        case VsaFormatOpt.QAM256:
        //        case VsaFormatOpt.QAM1024:
        //            modFormat = ModulationFormat.QAM;
        //            break;
        //        default:
        //            // 提供一个默认值或抛出异常，以处理未预期的格式
        //            modFormat = ModulationFormat.QAM;
        //            // throw new NotSupportedException($"Modulation format {_FormatOpt} is not supported.");
        //            break;
        //    }

        //    // 调用系数生成函数
        //    Boolean symbolSynLoopFilterValid = TimeLoopFilter(symbolsynbandWidth, kesi, sps, beta, M, out Double[]? symbolsyncoeff);

        //    if (!symbolSynLoopFilterValid || symbolsyncoeff == null || symbolsyncoeff.Length < 2)
        //    {
        //        Console.WriteLine("错误：定时同步环路滤波器系数生成失败。");
        //        return;
        //    }

        //    // MATLAB 脚本中传入的是 -c3, -c4。LoopFilter 返回的 coeff[0] 对应 c3, coeff[1] 对应 c4。
        //    double c1_loop = -symbolsyncoeff[0];
        //    double c2_loop = -symbolsyncoeff[1];

        //    //================================================================================
        //    // STEP 9: 执行定时同步
        //    //================================================================================
        //    // 目标: 使用新的 TimingSynchronizer 类来恢复出符号点。

        //    // 1. 准备输入数据
        //    //    在MATLAB脚本中，为了处理边界效应，对输入信号进行了截断：
        //    //    rxSamplePrefilter_short = rxSamplePrefilter(order+1:testnum_sync+order);
        //    //    rxSample_short = rxSample_nor(order/2+1:testnum_sync+order/2);
        //    //    这里的 order 是160。这是因为预滤波器 hprefilter 的长度是 161 (span/4 * sps + 1 = 10 * 16 + 1)。
        //    //    群延迟是 (161-1)/2 = 80。
        //    //    `filter`函数又额外引入了80的延迟。总共大约160。
        //    //    
        //    //    我们的 ConvolveMatlabFilter 也引入了 80 的延迟。
        //    //    为了精确匹配，我们也需要进行类似的截断。

        //    int order = (prefilterSpan * sps); // 10 * 16 = 160
        //    int testnum_sync = rxSample_nor.Length - order;

        //    if (rxSamplePrefilter.Length <= order || rxSample_nor.Length <= (order / 2) || testnum_sync <= 0)
        //    {
        //        Console.WriteLine("错误：信号长度不足以进行定时同步处理。");
        //        return;
        //    }

        //    // 截取预滤波后的信号，跳过前面 order 个点
        //    ReadOnlySpan<Complex> prefiltered_short = rxSamplePrefilter.AsSpan().Slice(order, testnum_sync);
        //    // 截取原始信号，跳过前面 order/2 个点
        //    ReadOnlySpan<Complex> original_short = rxSample_nor.AsSpan().Slice(order / 2, testnum_sync);

        //    // 2. 实例化 TimingSynchronizer
        //    //    我们暂时让捕获和跟踪系数使用同一套
        //    var synchronizer = new TimingSynchronizer(
        //        sps: sps,
        //        c1: c1_loop,
        //        c2: c2_loop,
        //        c3: c1_loop, // 跟踪阶段也用相同的系数
        //        c4: c2_loop, // 跟踪阶段也用相同的系数
        //        beta: beta,
        //        modFormat // 因为我们是16QAM
        //    );

        //    // 3. 执行同步处理
        //    var (synchronizedSymbols, timingErrors) = synchronizer.SynchronizeBlock(
        //        prefiltered_short,
        //        original_short
        //    );

        //    //================================================================================
        //    // STEP 10: 计算载波同步环路滤波器系数
        //    //================================================================================
        //    // 目标: 使用新创建的 CalculateCarrierLoopCoefficients 函数来生成系数。

        //    // 从MATLAB脚本中获取载波环路参数
        //    double carriersynbandWidth = 0.03; // 对应 BT_car
        //    int carrierSps = 1; // 载波环路在每个符号点上操作一次，所以SPS是1

        //    // 调用我们刚刚创建的、与MATLAB完全匹配的系数生成函数
        //    (double c5, double c6) = CalculateCarrierLoopCoefficients(carriersynbandWidth, kesi, carrierSps, M);

        //    int discardnum_after_timing = 1000;
        //    if (synchronizedSymbols.Count <= discardnum_after_timing)
        //    {
        //        Console.WriteLine("错误：定时同步后的符号点数量不足以进行载波同步。");
        //        return;
        //    }
        //    Complex[] carrierSyncInput = synchronizedSymbols.Skip(discardnum_after_timing).ToArray();
        //    Complex[] normalizedSignal = CarrierSynchronizer.NormalizeInputSignal(carrierSyncInput, 1);
        //    Complex[] refConstellation = VSA.GetStandardConstellation(_FormatOpt, M);
        //    double k0 = 1; // NCO增益，与MATLAB一致
        //    CarrierSynchronizer carrierSynchronizer = new CarrierSynchronizer(
        //        loopGainKp: c5,
        //        loopGainKi: c6,
        //        ncoGain: k0,
        //        referenceConstellation: refConstellation,
        //        modulationFormat: modFormat.ToString(), // 根据您的设置
        //        modulationOrder: M
        //    );
        //    // 4. 执行同步处理
        //    Complex[] synchronizedOutput = carrierSynchronizer.Process(normalizedSignal, out double[] phaseEstimates, out double[] phaseErrors);
        //    synchronizedOutput = synchronizedOutput.Skip(100).ToArray();
        //    //var linesToSave_carrier = synchronizedOutput.Select(c => $"{c.Real.ToString(System.Globalization.CultureInfo.InvariantCulture)},{c.Imaginary.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        //    //System.IO.File.WriteAllLines("cs_carrierSynchronized.csv", linesToSave_carrier);
        //    #region 星座图
        //    //星座图
        //    Double[] carrierSynI = synchronizedOutput.Real().Select(o => o * 0.01).ToArray();
        //    Double[] carrierSynQ = synchronizedOutput.Imaginary().Select(o => o * 0.01).ToArray();

        //    Double[,] constellationXY = new Double[2, carrierSynI.Length];
        //    Buffer.BlockCopy(carrierSynI.ToArray(), 0, constellationXY, 0, carrierSynI.Count() * sizeof(Double));
        //    Buffer.BlockCopy(carrierSynQ.ToArray(), 0, constellationXY, carrierSynI.Count() * sizeof(Double), carrierSynQ.Count() * sizeof(Double));
        //    OccupierBuffer.Default.Provide("Constellation()", new MathExt.Vector(constellationXY, "", "", 1.0, 1.0));
        //    #endregion
        //    #region EVM
        //    Int32 synchronizedOutputSize = synchronizedOutput.Length;
        //    Complex[] normalizedSignalEvm = NormalizeSignal(synchronizedOutput, M, _FormatOpt);
        //    var evmCarrier = normalizedSignalEvm.Take(synchronizedOutputSize < 500 ? synchronizedOutputSize : 500);
        //    VSA.PhaseDiscParam phaseDiscParam = new();
        //    var evmCarrierArray = evmCarrier.ToArray();
        //    Complex[] idealSignal = new Complex[evmCarrier.Count()];
        //    for (int i = 0; i < evmCarrier.Count(); i++)
        //    {
        //        phaseDiscParam = VSA.DDPhaseDiscQam(evmCarrierArray[i], M, _FormatOpt);
        //        idealSignal[i] = phaseDiscParam.CarrierSynSign;
        //    }
        //    //Double sumOfSquares = idealSignal.Select(c => Math.Pow(c.Real, 2) + Math.Pow(c.Imaginary, 2)).Sum();
        //    //Double meanOfSquares = sumOfSquares / idealSignal.Length;
        //    var standardConstellation = VSA.GetStandardConstellation(_FormatOpt, M);
        //    double meanOfSquares = standardConstellation.Select(c => Math.Pow(c.Real, 2) + Math.Pow(c.Imaginary, 2)).Average();
        //    Double res = Math.Sqrt(meanOfSquares);
        //    IEnumerable<Double> EVM = idealSignal.Zip(evmCarrierArray, (o, w) => ((o - w).Magnitude / res) * 100);
        //    OccupierBuffer.Default.Provide("VSAEvm", new MathExt.Vector(EVM.Select(o => o), "", "", 1.0, 1.0));
        //    Double evmRms = Math.Sqrt(EVM.Average(x => x * x)) / 10;
        //    Double evmMax = EVM.Max() / 10;
        //    Double evmMin = EVM.Min() / 10;

        //    if (!ErrParamTable.ContainsKey("EVM(%)"))
        //    {
        //        ErrParamTable["EVM(%)"] = new ParamStatistics
        //        {
        //            Mean = evmRms,
        //            Max = evmMax,
        //            Min = evmMin
        //        };
        //    }
        //    else
        //    {
        //        var stats = ErrParamTable["EVM(%)"];
        //        stats.Mean = evmRms;
        //        stats.Max = evmMax;
        //        stats.Min = evmMin;
        //    }
        //    static Complex[] NormalizeSignal(Complex[] signal, int M, VsaFormatOpt formatOpt)
        //    {
        //        bool isQAM = formatOpt == VsaFormatOpt.QAM16 || formatOpt == VsaFormatOpt.QAM64
        //                    || formatOpt == VsaFormatOpt.QAM128 || formatOpt == VsaFormatOpt.QAM256;
        //        if (isQAM)
        //        {
        //            double normalRefPoint = 0.5 * Math.Sqrt(2) * (Math.Sqrt(M) - 2) / (Math.Sqrt(M) - 1);
        //            var filteredData = signal.Where(c => c.Real > normalRefPoint && c.Imaginary > normalRefPoint).ToArray();
        //            if (filteredData.Length > 0)
        //            {
        //                double averageValue = filteredData.Average(c => c.Magnitude);
        //                double scaleFactor = (Math.Sqrt(M) - 1) * Math.Sqrt(2) / averageValue;
        //                return signal.Select(c => c * scaleFactor).ToArray();
        //            }
        //            else
        //            {
        //                double averageValue = signal.Average(c => c.Magnitude);
        //                double scaleFactor = (Math.Sqrt(M) - 1) * Math.Sqrt(2) / averageValue;
        //                return signal.Select(c => c * scaleFactor).ToArray();
        //            }
        //        }
        //        else
        //        {
        //            double averageValue = signal.Average(c => c.Magnitude);
        //            return signal.Select(c => c / averageValue).ToArray();
        //        }
        //    }
        //    // 相位误差图
        //    IEnumerable<Double> PhaseErr = evmCarrier.Zip(idealSignal, (r, i) => r.Phase - i.Phase);
        //    OccupierBuffer.Default.Provide("PhaseErrTime()", new MathExt.Vector(PhaseErr.Select(o => o * 0.1), "", "", 1.0, 1.0));

        //    // 幅度误差图
        //    IEnumerable<Double> AmplErr = evmCarrier.Zip(idealSignal, (r, i) => r.Magnitude - i.Magnitude);
        //    OccupierBuffer.Default.Provide("AmplErrTime()", new MathExt.Vector(AmplErr.Select(o => o * 0.1), "", "", 1.0, 1.0));
        //    var errparam = ErrCaculate.CalcParam(evmCarrier, idealSignal, EVM, PhaseErr, AmplErr);
        //    if (errparam == null)
        //    {
        //        return;
        //    }
        //    foreach (var prop in errparam.GetType().GetProperties())
        //    {
        //        if (ErrParamTable.ContainsKey(prop.Name))
        //        {
        //            ErrParamTable[prop.Name].Value = (Double)prop.GetValue(errparam)!;
        //        }
        //        else
        //        {
        //            ErrParamTable.Add(prop.Name, new ParamStatistics
        //            {
        //                Value = (Double)prop.GetValue(errparam)!
        //            });
        //        }
        //    }
        //    #endregion
        //    #region IQ时域图
        //    OccupierBuffer.Default.Provide("IChartIDiagram", new MathExt.Vector(carrierSynI.Take(500).Select(o => o * 100), "", "", 1.0 / (156.25e6), 1.0));
        //    OccupierBuffer.Default.Provide("IChartQDiagram", new MathExt.Vector(carrierSynQ.Take(500).Select(o => o * 100), "", "", 1.0 / (156.25e6), 1.0));
        //    #endregion
        //    #region 矢量图
        //    //double phaseCorrection = -phaseEstimates.LastOrDefault();
        //    //Complex correctionPhasor = Complex.Exp(Complex.ImaginaryOne * phaseCorrection);
        //    //// 3. 选择数据源：使用匹配滤波后的 rxSample，并应用相位校正
        //    //int vectorPointsToShow = Math.Min(rxSample.Length, 8000);

        //    //// 关键步骤：在选择数据点的同时，应用旋转校正
        //    ////var vectorData = rxSample
        //    ////                    .Take(vectorPointsToShow)
        //    ////                    .Select(c => c * correctionPhasor); // 对每个点进行旋转
        //    ////                                                        // 4. 准备数据格式
        //    //var vectorData = rxSample
        //    //                    .Take(vectorPointsToShow)
        //    //                    ;
        //    //Double[] vectorI = vectorData.Select(c => c.Real).ToArray();
        //    //Double[] vectorQ = vectorData.Select(c => c.Imaginary).ToArray();
        //    //Double[,] vectorXY = new Double[2, vectorPointsToShow];
        //    //Buffer.BlockCopy(vectorI, 0, vectorXY, 0, vectorPointsToShow * sizeof(Double));
        //    //Buffer.BlockCopy(vectorQ, 0, vectorXY, vectorPointsToShow * sizeof(Double), vectorPointsToShow * sizeof(Double));

        //    //// 5. 发送数据给UI
        //    //OccupierBuffer.Default.Provide("VectorGph()", new MathExt.Vector(vectorXY, "", "", 1.0, 1.0));
        //    Complex[] idealWaveform = ReconstructIdealWaveform(synchronizedOutput, sps, beta, span);

        //    int pointsToShow = Math.Min(idealWaveform.Length, 4000); // 显示的点数
        //    var vectorData = idealWaveform.Take(pointsToShow);

        //    Double[] vectorI = vectorData.Select(c => c.Real).ToArray();
        //    Double[] vectorQ = vectorData.Select(c => c.Imaginary).ToArray();

        //    Double[,] vectorXY = new Double[2, pointsToShow];
        //    Buffer.BlockCopy(vectorI, 0, vectorXY, 0, pointsToShow * sizeof(Double));
        //    Buffer.BlockCopy(vectorQ, 0, vectorXY, pointsToShow * sizeof(Double), pointsToShow * sizeof(Double));

        //    OccupierBuffer.Default.Provide("VectorGph()", new MathExt.Vector(vectorXY, "", "", 1.0, 1.0));
        //    #endregion
        //    #region 眼图
        //    #endregion
        //}

        public enum ModulationFormat
        {
            PSK,
            QAM
        }

        public class TimingSynchronizer
        {
            // Parameters
            private readonly int _sps; // Samples Per Symbol
            private readonly double _c1, _c2, _c3, _c4; // 环路滤波器系数 (比例和积分器增益)
            private readonly double _beta; // Rolloff factor
            private readonly ModulationFormat _modFormat;
            private readonly Complex[,] _interpFilterCoeff; // 插值滤波器系数 (Farrow)

            // State Variables
            private double _loopFilterState;              // 环路滤波器积分器状态
            private double _loopPreviousInput;            // 环路滤波器前次输入
            private bool _strobe;                         // 符号采样标志位
            private Queue<bool> _strobeHistory; // 符号采样历史记录（用于TED缓冲控制）
            private double _mu; // 分数间隔插值点（范围0~1）
            private double _ncoCounter; // 数控振荡器(NCO)计数器
            private Complex[] _interpFilterState; // State for the pre-filtered signal interpolator
            private Complex[] _interpFilterState2; // State for the non-pre-filtered signal interpolator
            private Queue<Complex> _tedBuffer; // Buffer for TED calculation

            // Threshold for switching loop filter coefficients
            private const int LoopCoeffSwitchThreshold = 10000;

            /// <summary>
            /// Initializes a new instance of the TimingSynchronizer class.
            /// </summary>
            /// <param name="sps">Samples Per Symbol.</param>
            /// <param name="c1">Loop filter coefficient (初始阶段比例增益).</param>
            /// <param name="c2">Loop filter coefficient (初始阶段积分增益).</param>
            /// <param name="c3">Loop filter coefficient (跟踪阶段比例增益).</param>
            /// <param name="c4">Loop filter coefficient (跟踪阶段积分增益).</param>
            /// <param name="beta">Rolloff factor for pulse shaping filter.</param>
            /// <param name="modFormat">Modulation format (PSK or QAM).</param>
            /// <param name="alpha">Interpolator coefficient抛物线插值系数(typically 0.5 for piecewise parabolic).</param>
            public TimingSynchronizer(int sps, double c1, double c2, double c3, double c4, double beta, ModulationFormat modFormat, double alpha = 0.5)
            {
                if (sps % 2 != 0)
                {
                    throw new ArgumentException("Samples Per Symbol (sps) must be an even integer.", nameof(sps));
                }

                _sps = sps;
                _c1 = c1; // Note: MATLAB code uses -c3, -c4. Assuming inputs here are the final desired values. Adjust if needed.
                _c2 = c2;
                _c3 = c3;
                _c4 = c4;
                _beta = beta;
                _modFormat = modFormat;

                // 初始化Farrow插值器系数（分段抛物线型）
                _interpFilterCoeff = new Complex[3, 4];
                _interpFilterCoeff[0, 0] = 0; _interpFilterCoeff[0, 1] = 0; _interpFilterCoeff[0, 2] = 1; _interpFilterCoeff[0, 3] = 0;      // Constant
                _interpFilterCoeff[1, 0] = -alpha; _interpFilterCoeff[1, 1] = 1 + alpha; _interpFilterCoeff[1, 2] = -(1 - alpha); _interpFilterCoeff[1, 3] = -alpha; // Linear
                _interpFilterCoeff[2, 0] = alpha; _interpFilterCoeff[2, 1] = -alpha; _interpFilterCoeff[2, 2] = -alpha; _interpFilterCoeff[2, 3] = alpha;  // 二次项系数

                // Initialize state variables
                Reset();
            }

            /// <summary>
            /// Resets the internal state of the synchronizer.
            /// </summary>
            public void Reset()
            {
                _loopFilterState = 0.0;
                _loopPreviousInput = 0.0;
                _strobe = false;
                _mu = 0.0;
                _ncoCounter = 0.0;

                _interpFilterState = new Complex[3]; // Needs 3 previous samples
                _interpFilterState2 = new Complex[3];
                Array.Clear(_interpFilterState, 0, _interpFilterState.Length);
                Array.Clear(_interpFilterState2, 0, _interpFilterState2.Length);


                _strobeHistory = new Queue<bool>(Enumerable.Repeat(false, _sps)); // Initialize with SPS false values
                _tedBuffer = new Queue<Complex>(Enumerable.Repeat(Complex.Zero, _sps)); // Initialize with SPS zeros
            }


            /// <summary>
            /// Processes a block of input samples to perform timing synchronization.
            /// </summary>
            /// <param name="prefilteredSamples">Input samples after pre-filtering (used for TED).</param>
            /// <param name="originalSamples">Input samples before pre-filtering (used for symbol output).</param>
            /// <returns>A tuple containing the list of synchronized output symbols and 各采样点的定时误差mu值.</returns>
            public (List<Complex> synchronizedSymbols, List<double> timingError) SynchronizeBlock(
                ReadOnlySpan<Complex> prefilteredSamples,
                ReadOnlySpan<Complex> originalSamples)
            {
                if (prefilteredSamples.Length != originalSamples.Length)
                {
                    throw new ArgumentException("Input sample arrays must have the same length.");
                }

                int inputFrameLen = prefilteredSamples.Length;
                // Estimate max output length conservatively (adjust MaxOutputExpansionFactor if needed)
                // double maxExpansionFactor = 1.1; // Corresponds to [11, 10] in MATLAB
                // int maxOutputFrameLen = (int)Math.Ceiling(inputFrameLen * maxExpansionFactor / _sps);
                // Use List which dynamically resizes
                var synchronizedSymbols = new List<Complex>();
                var timingError = new List<double>(inputFrameLen); // Pre-allocate for efficiency

                int numStrobe = 0;

                for (int sampleIdx = 0; sampleIdx < inputFrameLen; sampleIdx++)
                {
                    // Stop if output buffer likely full and strobe is set (prevent excessive growth)
                    // This check might need refinement based on expected output size.
                    // if (numStrobe >= maxOutputFrameLen && _strobe) {
                    //     Console.WriteLine("Warning: Output buffer potentially full.");
                    //     break;
                    // }

                    // Update strobe count based on previous iteration's strobe迭代 
                    numStrobe += _strobe ? 1 : 0;

                    // Record timing error (fractional interval from previous step)
                    timingError.Add(_mu);

                    // Interpolation (Farrow structure)
                    Complex intOut = Interpolate(prefilteredSamples[sampleIdx], ref _interpFilterState);
                    Complex intOut2 = Interpolate(originalSamples[sampleIdx], ref _interpFilterState2);

                    // Store symbol output if strobe is active
                    if (_strobe)
                    {
                        synchronizedSymbols.Add(intOut2);
                    }

                    // Stop if output buffer is potentially full AFTER adding a symbol
                    // if (numStrobe >= maxOutputFrameLen && _strobe) {
                    //      Console.WriteLine("Warning: Output buffer potentially full after adding symbol.");
                    //      break;
                    // }

                    // Timing Error Detector (Gardner)
                    double error = GardnerTED(intOut);

                    // Update TED Buffer based on strobe history (handles skipping/stuffing)
                    UpdateTedBuffer(intOut); // Must happen before LoopFilter/InterpControl updates strobe history

                    // Loop Filter (PI Controller)
                    // Select coefficients based on sample index
                    double currentC1 = (sampleIdx < LoopCoeffSwitchThreshold) ? _c1 : _c3;
                    double currentC2 = (sampleIdx < LoopCoeffSwitchThreshold) ? _c2 : _c4;
                    double v = LoopFilter(error, currentC1, currentC2);


                    // Interpolation Control (NCO) - Updates _mu, _ncoCounter, _strobeHistory, _strobe for the *next* iteration
                    InterpolationControl(v);
                }

                return (synchronizedSymbols, timingError);
            }


            /// <summary>
            /// 基于 ​​Farrow结构的分段抛物线插值器，用于在非整数采样点（由 pMu 控制）对输入信号进行插值
            /// Updates the filter state in place.
            /// </summary>
            private Complex Interpolate(Complex inputSample, ref Complex[] filterState)
            {
                // Input sequence for Farrow: [current_sample, state1, state2, state3]
                Complex x0 = inputSample;
                Complex x1 = filterState[0];
                Complex x2 = filterState[1];
                Complex x3 = filterState[2];

                // Calculate contributions for constant, linear, and quadratic terms
                Complex y_const = _interpFilterCoeff[0, 0] * x0 + _interpFilterCoeff[0, 1] * x1 + _interpFilterCoeff[0, 2] * x2 + _interpFilterCoeff[0, 3] * x3;
                Complex y_lin = _interpFilterCoeff[1, 0] * x0 + _interpFilterCoeff[1, 1] * x1 + _interpFilterCoeff[1, 2] * x2 + _interpFilterCoeff[1, 3] * x3;
                Complex y_quad = _interpFilterCoeff[2, 0] * x0 + _interpFilterCoeff[2, 1] * x1 + _interpFilterCoeff[2, 2] * x2 + _interpFilterCoeff[2, 3] * x3;

                // Combine terms weighted by mu
                Complex output = y_const + y_lin * _mu + y_quad * _mu * _mu; // Equivalent to sum((coeff * xSeq) .* [1; mu; mu^2])

                // Update state for next iteration: Shift state and add new input
                filterState[2] = filterState[1];
                filterState[1] = filterState[0];
                filterState[0] = inputSample;

                return output;
            }

            /// <summary>
            /// Updates the TED buffer considering strobe history for skipping/stuffing.
            /// This should be called *before* InterpolationControl updates the strobe history queue.
            /// </summary>
            private void UpdateTedBuffer(Complex interpolatedSample)
            {
                // Count strobes in the relevant history window (excluding the oldest, including the current potential strobe)
                int strobeCount = _strobeHistory.Skip(1).Count(s => s) + (_strobe ? 1 : 0);


                switch (strobeCount)
                {
                    case 0:
                        // Skip: No strobe across N samples. Buffer doesn't necessarily need update
                        // unless we always want it full, but MATLAB logic implies just skipping TED update.
                        // However, the buffer needs *some* update to keep sliding. Let's just enqueue/dequeue.
                        //if (_tedBuffer.Count >= _sps) _tedBuffer.Dequeue();
                        //_tedBuffer.Enqueue(interpolatedSample);
                        break;
                    case 1:
                        // Regular update: Shift buffer, add new sample.
                        if (_tedBuffer.Count >= _sps) _tedBuffer.Dequeue();
                        _tedBuffer.Enqueue(interpolatedSample);
                        break;
                    default: // > 1
                             // Stuff: Two or more strobes. Shift buffer by 2, add zero, add new sample.
                        if (_tedBuffer.Count >= _sps) _tedBuffer.Dequeue(); // Remove oldest
                        if (_tedBuffer.Count >= _sps - 1) _tedBuffer.Dequeue(); // Remove second oldest
                        _tedBuffer.Enqueue(Complex.Zero); // Stuff with zero
                        _tedBuffer.Enqueue(interpolatedSample); // Add current
                                                                // Ensure buffer doesn't exceed SPS size due to stuffing logic edge cases
                        while (_tedBuffer.Count > _sps) _tedBuffer.Dequeue();
                        break;
                }
                // Ensure buffer has exactly _sps elements after update if starting empty or smaller
                while (_tedBuffer.Count < _sps)
                {
                    // This padding might be needed if the buffer starts empty,
                    // but the constructor initializes it. Add padding logic if Reset behavior changes.
                    // For now, assume it's always kept at size _sps by enqueue/dequeue.
                    // If stuffing logic causes issues, review padding here. Let's assume standard enqueue/dequeue keeps size.
                    // Pad with zero if initial state is problematic - check constructor init first.
                    _tedBuffer.Enqueue(Complex.Zero); // Example padding if needed
                }
            }


            /// <summary>
            /// 比例积分（PI）型环路滤波器​​，用于处理定时误差信号 e，生成控制信号 v，从而调整插值控制器。其核心作用是通过 ​​比例项（快速响应）​​ 和 ​​积分项（消除稳态误差）​​ 实现环路稳定。
            /// </summary>
            private double LoopFilter(double error, double c1, double c2)
            {
                // c1 = Proportional gain, c2 = Integrator gain
                double loopFiltOut = _loopPreviousInput + _loopFilterState; // Integrator component
                double v = error * c1 + loopFiltOut; // P + I output

                // Update state for next iteration
                _loopFilterState = loopFiltOut; // Store integrator state
                _loopPreviousInput = error * c2; // Store scaled error for next integrator input

                return v;
            }

            /// <summary>
            /// Interpolation control using a modulo-1 counter (NCO).
            /// Updates strobe, mu, NCO counter, and strobe history.
            /// </summary>
            private void InterpolationControl(double loopFilterOutput)
            {
                double W = loopFilterOutput + 1.0 / _sps; // NCO control word (should be small when locked)

                // Update strobe history queue: Dequeue oldest, Enqueue current strobe state
                _strobeHistory.Dequeue();
                _strobeHistory.Enqueue(_strobe); // Enqueue the strobe value from the *start* of the main loop iteration

                // Determine strobe for the *next* sample based on current NCO state
                bool nextStrobe = (_ncoCounter < W);

                // Update fractional interval (mu) if a strobe will occur *next*
                if (nextStrobe)
                {
                    // Avoid division by zero or very small W if loop becomes unstable
                    _mu = (W == 0) ? 0 : _ncoCounter / W;
                    // Clamp mu to valid range [0, 1) - Farrow interpolator might be sensitive outside this
                    _mu = Math.Max(0.0, Math.Min(_mu, 1.0));
                }
                // else: keep the old mu if no strobe - MATLAB doesn't explicitly show this,
                // but mu is only relevant when strobe=true for symbol output,
                // and the interpolator uses the current _mu regardless. Let's update only on strobe.

                // Update NCO counter (Modulo-1)
                _ncoCounter = (_ncoCounter - W) % 1.0;
                if (_ncoCounter < 0) // Ensure positive result for C# % operator
                {
                    _ncoCounter += 1.0;
                }

                // Set the strobe state for the *next* iteration
                _strobe = nextStrobe;
            }

            /// <summary>
            /// Gardner Timing Error Detector (TED).
            /// Calculates timing error based on interpolated samples.
            /// </summary>
            private double GardnerTED(Complex currentInterpolated)
            {
                // TED calculation only happens if a strobe occurred in the *current* iteration
                // AND the strobe history indicates this isn't immediately after a skip (all previous non-strobes)
                // Check _strobe (set by previous iteration's InterpControl) and relevant part of _strobeHistory
                bool canUpdateTED = _strobe && !_strobeHistory.Skip(1).Any(s => s); // Check if all but the oldest history entries are false

                if (canUpdateTED) // Use _strobe determined at the start of the loop iteration
                {
                    // Access samples from the TED buffer
                    // Ensure the buffer has enough elements before accessing
                    if (_tedBuffer.Count < _sps)
                    {
                        Console.WriteLine("Warning: TED buffer not full during calculation.");
                        // Handle this case: return 0 error or throw exception? Let's return 0 for now.
                        return 0.0;
                    }
                    Complex earlySample = _tedBuffer.ElementAt(0); // Corresponds to pTEDBuffer(1) in MATLAB
                    Complex lateSample = currentInterpolated;    // Corresponds to x(sampleIdx) used in TED calc in MATLAB (which is intOut)

                    // Mid-sample calculation depends on SPS parity
                    Complex midSample;
                    //int midIndex1 = _sps / 2 - 1; // 0-based index for mid-point sample 1
                    int midIndex1 = _sps / 2; // 0-based index for mid-point sample 1
                    int midIndex2 = _sps / 2; // 0-based index for mid-point sample 2 (for even SPS)


                    Complex t1 = _tedBuffer.ElementAt(midIndex1); // Corresponds to pTEDBuffer(end/2 + 1 - rem(pSPS,2))
                    if (_sps % 2 == 0)
                    {
                        Complex t2 = _tedBuffer.ElementAt(midIndex2); // Corresponds to pTEDBuffer(end/2 + 1) for even SPS
                        midSample = (t1 + t2) * 0.5;
                    }
                    else
                    {
                        midSample = t1; // For odd SPS, mid-point is just one sample
                    }

                    Complex pTEDBufferFirst = _tedBuffer.ElementAt(0); // 对应 pTEDBuffer(1)
                    double signRealT = -Math.Sign(pTEDBufferFirst.Real) * Math.Sign(currentInterpolated.Real);
                    double signImagT = -Math.Sign(pTEDBufferFirst.Imaginary) * Math.Sign(currentInterpolated.Imaginary);


                    double error = 0;
                    switch (_modFormat)
                    {
                        case ModulationFormat.PSK:
                            // Original MATLAB PSK formula:
                            // e = signRealt * real(midSample) * (real(pTEDBuffer(1)) - real(x)) + ...;
                            //     signImagt * imag(midSample) * (imag(pTEDBuffer(1)) - imag(x));
                            // where signRealt = -sign(real(pTEDBuffer(1))) * sign(real(x));
                            // Let's use the simpler version first (commented out in MATLAB):
                            error = midSample.Real * (earlySample.Real - lateSample.Real) +
                                    midSample.Imaginary * (earlySample.Imaginary - lateSample.Imaginary);
                            break;

                        case ModulationFormat.QAM:
                            // Original MATLAB QAM formula:
                            // e = (real(midSample) - real(sigMean)) * (real(pTEDBuffer(1)) - real(x)) + ...;
                            //     (imag(midSample) - imag(sigMean)) * (imag(pTEDBuffer(1)) - imag(x));
                            // where sigMean = (pTEDBuffer(1) + x )/2;
                            Complex sigMean = (earlySample + lateSample) * 0.5;
                            error = (midSample.Real - sigMean.Real) * (earlySample.Real - lateSample.Real) +
                                    (midSample.Imaginary - sigMean.Imaginary) * (earlySample.Imaginary - lateSample.Imaginary);
                            break;
                    }
                    return error;
                }
                else
                {
                    return 0.0; // No error calculation if no valid strobe
                }
            }
        }
        public class CarrierSynchronizer
        {
            private readonly double _loopGainKp; // Proportional gain (c3 in MATLAB)
            private readonly double _loopGainKi; // Integral gain (c4 in MATLAB)
            private readonly double _ncoGain;    // NCO gain (k0 in MATLAB)
            private readonly Complex[] _referenceConstellationNormalized; // Normalized reference constellation (refSig_nomal)
            private readonly string _modulationFormat; // "QAM" or "PSK"
            private readonly int _modulationOrder; // M

            private double _phaseEstimate = 0.0; // NCO phase accumulator (Phase(i) in MATLAB)
            private double _loopFilterState = 0.0; // Loop filter state (w1(i) in MATLAB)
            private double _previousPhaseError = 0.0; // Previous phase error for loop filter calculation (P_error(i-1))

            /// <summary>
            /// Initializes a new instance of the <see cref="CarrierSynchronizer"/> class.
            /// </summary>
            /// <param name="loopGainKp">Proportional gain of the loop filter (c3).</param>
            /// <param name="loopGainKi">Integral gain of the loop filter (c4).</param>
            /// <param name="ncoGain">Gain of the Numerically Controlled Oscillator (k0).</param>
            /// <param name="referenceConstellation">The ideal reference constellation points (refSig).</param>
            /// <param name="modulationFormat">Modulation format ("QAM" or "PSK"). Needed for correct normalization.</param>
            /// <param name="modulationOrder">Modulation order (M). Needed for correct normalization.</param>
            public CarrierSynchronizer(double loopGainKp, double loopGainKi, double ncoGain, Complex[] referenceConstellation, string modulationFormat, int modulationOrder)
            {
                _loopGainKp = loopGainKp;
                _loopGainKi = loopGainKi;
                _ncoGain = ncoGain;

                // Normalize reference constellation based on modulation format (matches refSig_nomal logic)
                if (modulationFormat.Equals("QAM", StringComparison.OrdinalIgnoreCase))
                {
                    // Normalization specific to QAM as in MATLAB line 44
                    double factor = (Math.Sqrt(modulationOrder / 4.0) * 2.0 - 1.0) * Math.Sqrt(2.0);
                    // Handle potential division by zero if factor is zero (e.g., M=4 for QPSK treated as QAM)
                    if (Math.Abs(factor) < 1e-9)
                    {
                        // QPSK case treated as QAM? Use average power normalization or specific QPSK handling.
                        // Using average power normalization as a fallback for M=4 QAM.
                        double avgPower = referenceConstellation.Average(c => c.Magnitude * c.Magnitude);
                        factor = Math.Sqrt(avgPower);
                    }
                    if (Math.Abs(factor) > 1e-9)
                    {
                        _referenceConstellationNormalized = referenceConstellation.Select(c => c / factor).ToArray();
                    }
                    else
                    {
                        // Fallback if factor is still zero or very small
                        _referenceConstellationNormalized = (Complex[])referenceConstellation.Clone();
                    }

                }
                else // PSK
                {
                    // For PSK, MATLAB code uses refSig directly (line 42), implying they are already normalized (e.g., on unit circle)
                    // We clone it to ensure it's a separate copy.
                    _referenceConstellationNormalized = (Complex[])referenceConstellation.Clone();
                }
                // Previous normalization logic (commented out)
                // #region AI
                // // ... (previous normalization attempts removed for clarity) ...
                // #endregion
                // double factor = (Math.Sqrt(modulationOrder / 4.0) * 2.0 - 1.0) * Math.Sqrt(2.0);
                // _referenceConstellationNormalized = referenceConstellation.Select(c => c / factor).ToArray();
            }
            public static Complex[] NormalizeInputSignal(Complex[] complexSignal, double gain = 1.0)
            {
                int n = complexSignal.Length;
                int carrierMaxnum = 200; // The window size to find the max amplitude

                if (n <= carrierMaxnum)
                {
                    // Not enough data to perform normalization and have remaining signal
                    // You might want to throw an exception or return an empty array
                    return new Complex[0];
                }

                // 1. Find the maximum squared amplitude in the analysis window
                double maxAmpSquared = 0;
                for (int i = 0; i < carrierMaxnum; i++)
                {
                    double ampSquared = complexSignal[i].Magnitude * complexSignal[i].Magnitude;
                    if (ampSquared > maxAmpSquared)
                    {
                        maxAmpSquared = ampSquared;
                    }
                }

                // 2. Calculate the normalization factor
                double normalizationFactor = Math.Sqrt(maxAmpSquared);

                // If signal is nearly zero, avoid division by zero. Return the signal as is, but skip the analysis window.
                if (normalizationFactor < 1e-10)
                {
                    return complexSignal.Skip(carrierMaxnum).ToArray();
                }

                // 3. Create the output array for the normalized signal (length n - carrierMaxnum)
                Complex[] normalizedSignal = new Complex[n - carrierMaxnum];

                // 4. Apply normalization to the rest of the signal
                for (int i = 0; i < normalizedSignal.Length; i++)
                {
                    // The source index is i + carrierMaxnum
                    normalizedSignal[i] = complexSignal[i + carrierMaxnum] / normalizationFactor;
                }

                return normalizedSignal;
            }

            /// <summary>
            /// Normalizes the input signal based on the method in carrier_normalization_ori.m (lines 31-41).
            /// Applies a fixed gain and then normalizes based on the maximum amplitude.
            /// </summary>
            /// <param name="inputSignalI">Real part of the input signal (yI).</param>
            /// <param name="inputSignalQ">Imaginary part of the input signal (yQ).</param>
            /// <param name="gain">Gain factor to apply before normalization (default 0.2).</param>
            /// <returns>Normalized complex signal.</returns>
            public static Complex[] NormalizeInputSignal(double[] inputSignalI, double[] inputSignalQ, double gain = 0.2)
            {
                int n = inputSignalI.Length;
                if (n != inputSignalQ.Length)
                {
                    throw new ArgumentException("Input I and Q arrays must have the same length.");
                }

                int carrierMaxnum = 200;
                Complex[] normalizedSignal = new Complex[n - 200];
                double maxAmpSquared = 0;

                // Apply gain and find max squared amplitude
                for (int i = 0; i < n; i++)
                {
                    double iGain = inputSignalI[i] * gain;
                    double qGain = inputSignalQ[i] * gain;
                    if (i < carrierMaxnum)
                    {
                        double ampSquared = iGain * iGain + qGain * qGain;
                        if (ampSquared > maxAmpSquared)
                        {
                            maxAmpSquared = ampSquared;
                        }
                    }
                    else
                    {
                        // Store the gained signal temporarily
                        normalizedSignal[i - 200] = new Complex(iGain, qGain);
                    }
                }

                // Normalize by sqrt of max squared amplitude
                double normalizationFactor = Math.Sqrt(maxAmpSquared);
                if (normalizationFactor < 1e-10) // Avoid division by zero or near-zero
                {
                    // Handle case of zero or very small signal - return zero signal or throw error
                    // Returning zero signal for now
                    return Enumerable.Repeat(Complex.Zero, n).ToArray();
                    // Or: throw new InvalidOperationException("Signal magnitude is too small for normalization.");
                }


                for (int i = 0; i < n - 200; i++)
                {
                    normalizedSignal[i] /= normalizationFactor;
                }

                return normalizedSignal;
            }


            /// <summary>
            /// Performs symbolic judgement (decision) on a received symbol.
            /// Finds the closest point in the normalized reference constellation.
            /// Corresponds to SymbolicJudement in carrier_normalization.m.
            /// </summary>
            /// <param name="receivedSymbol">The complex symbol after phase correction.</param>
            /// <returns>The closest complex symbol from the normalized reference constellation.</returns>
            private Complex SymbolicJudement(Complex receivedSymbol)
            {
                double minDistanceSquared = double.MaxValue;
                Complex decidedSymbol = _referenceConstellationNormalized.Length > 0 ? _referenceConstellationNormalized[0] : Complex.Zero; // Handle empty constellation

                foreach (Complex refSymbol in _referenceConstellationNormalized)
                {
                    double distSq = Complex.Abs(receivedSymbol - refSymbol);
                    distSq *= distSq; // Use squared distance to avoid sqrt

                    if (distSq < minDistanceSquared)
                    {
                        minDistanceSquared = distSq;
                        decidedSymbol = refSymbol;
                    }
                }
                return decidedSymbol;
            }

            /// <summary>
            /// Processes the input signal to perform carrier synchronization.
            /// Corresponds to the main loop in carrier_normalization_ori.m (lines 48-73).
            /// Assumes the input signal is already normalized.
            /// </summary>
            /// <param name="normalizedInputSignal">The normalized complex input signal.</param>
            /// <param name="phaseOutput">Output array to store the NCO phase estimate for each symbol.</param>
            /// <param name="phaseErrorOutput">Output array to store the phase detector error for each symbol.</param>
            /// <returns>The complex signal after carrier synchronization (phase correction).</returns>
            public Complex[] Process(Complex[] normalizedInputSignal, out double[] phaseOutput, out double[] phaseErrorOutput)
            {
                int n = normalizedInputSignal.Length;
                Complex[] outputSignal = new Complex[n]; // Corresponds to 'r' in MATLAB
                phaseOutput = new double[n];        // Corresponds to 'Phase' in MATLAB
                phaseErrorOutput = new double[n];   // Corresponds to 'P_error' in MATLAB
                double[] loopFilterOutput = new double[n]; // Corresponds to 'w1' in MATLAB


                // Initialize state variables (redundant if Process is called multiple times on the same instance,
                // but good for clarity or if creating a new instance per signal block).
                // _phaseEstimate = 0.0;
                // _loopFilterState = 0.0; // Represents w1(i)
                // _previousPhaseError = 0.0;

                for (int i = 0; i < n; i++)
                {
                    // Store current phase estimate before NCO update
                    phaseOutput[i] = _phaseEstimate;
                    loopFilterOutput[i] = _loopFilterState; // Store w1(i)


                    // #1 Phase Compensation (MATLAB line 51)
                    // r(i) = input(i) * exp(j * -Phase(i))
                    Complex correctedSymbol = normalizedInputSignal[i] * Complex.Exp(Complex.ImaginaryOne * -_phaseEstimate);
                    outputSignal[i] = correctedSymbol;

                    // #2 Symbol Judgement (MATLAB line 54)
                    Complex decidedSymbol = SymbolicJudement(correctedSymbol);

                    // #3 Phase Detector (PD) (MATLAB lines 56-59)
                    // P_error(i) = abs(r(i)) * imag(r(i)/Symjud(i))/sqrt(2)
                    double phaseError = 0;
                    if (decidedSymbol.Magnitude > 1e-10) // Avoid division by zero
                    {
                        Complex errorSignal = correctedSymbol / decidedSymbol;
                        // Use magnitude of corrected symbol as in MATLAB line 58 `r_A(i) = abs(r(i))`
                        phaseError = correctedSymbol.Magnitude * errorSignal.Imaginary / Math.Sqrt(2.0);
                    }
                    phaseErrorOutput[i] = phaseError;


                    // #4 Loop Filter (MATLAB lines 62-66)
                    // w1(i+1) = w1(i) + c3*(P_error(i)-P_error(i-1)) + c4*P_error(i)
                    // Update loop filter state for the *next* iteration (calculating w1(i+1))
                    // Note: MATLAB uses w1(i) for NCO update, so we update _loopFilterState *after* the NCO step.
                    double proportionalTerm = _loopGainKp * (phaseError - _previousPhaseError);
                    double integralTerm = _loopGainKi * phaseError;
                    double nextLoopFilterState = _loopFilterState + proportionalTerm + integralTerm;


                    // #5 Numerically Controlled Oscillator (NCO) (MATLAB line 69)
                    // Phase(i+1) = Phase(i) + k0 * w1(i)
                    // Use the *current* loop filter state (_loopFilterState which is w1(i)) before updating it.
                    _phaseEstimate += _ncoGain * _loopFilterState;

                    // Update state for next iteration
                    _loopFilterState = nextLoopFilterState; // w1(i+1)
                    _previousPhaseError = phaseError;       // P_error(i)

                    // Optional: Wrap phase estimate to keep it within [-pi, pi] or [0, 2pi]
                    _phaseEstimate = (_phaseEstimate + Math.PI) % (2 * Math.PI) - Math.PI;

                    // Handle last iteration edge case if output arrays need N+1 elements like MATLAB's Phase/w1
                    if (i == n - 1 && n > 0)
                    {
                        // If needed, could store the final Phase(n) and w1(n) based on the last loop calculations
                        // phaseOutput[n] = _phaseEstimate;
                        // loopFilterOutput[n] = _loopFilterState;
                    }
                }


                // Reset state if the instance will be reused for unrelated signals?
                // Or assume a new instance is created for each signal block.

                return outputSignal; // Return the phase-corrected signal 'r'
            }


            // TODO: Implement CalculateLoopFilterCoefficients if needed, or take coefficients as input.
            // public static (double kp, double ki) CalculateLoopFilterCoefficients(double loopBandwidth, double dampingFactor, int samplesPerSymbol, int modulationOrder) { ... }
        }
        //private double[,] GenerateVsaEyeDiagramMatrix(double[] signalData, int sps, double symbolsToDisplay = 3.0)
        //{
        //    if (signalData == null || signalData.Length < sps * 2 || sps <= 0)
        //    {
        //        return new double[0, 0]; // 数据不足，返回空矩阵
        //    }

        //    // 1. 创建一个完美的、无抖动的“恢复时钟边沿”数组
        //    int numSymbols = signalData.Length / sps;
        //    double[] perfectClockEdges = new double[numSymbols + 1];
        //    for (int i = 0; i <= numSymbols; i++)
        //    {
        //        perfectClockEdges[i] = i * sps;
        //    }

        //    // 2. 准备调用核心矩阵生成函数所需的参数
        //    int height = 2048; // 垂直分辨率，可以根据需要调整
        //    double low = signalData.Min();
        //    double high = signalData.Max();
        //    double step = (high - low) / height;
        //    if (step == 0) step = 1; // 避免除以零

        //    // 将信号数据归一化到 [0, height-1] 的整数范围
        //    // 这是因为 GetEyeDiagramMatrix1 内部处理的是ADC码值，我们模拟这个过程
        //    var scaledSamples = signalData.Select(d => (d - low) / step).ToArray();

        //    // 3. 直接调用您现有的、强大的静态方法来生成眼图矩阵
        //    // 我们复用 EyeGraphGenerator 里的核心算法，但给它提供了来自VSA的理想时钟
        //    var eyeMatrix = EyeGraphGenerator.GetEyeDiagramMatrix1(
        //        height,
        //        0, // low-level for scaled data is 0
        //        1, // step for scaled data is 1
        //        scaledSamples,
        //        perfectClockEdges,
        //        sps,
        //        symbolsToDisplay, // 在屏幕上显示2个UI
        //        out double[] eyeLevHist,
        //        out double compressRatio
        //    );

        //    return eyeMatrix;
        //}
        private Complex[] ReconstructIdealWaveform(Complex[] symbols, int upsampleFactor, double beta, int span)
        {
            if (symbols == null || symbols.Length == 0)
            {
                return new Complex[0];
            }

            // 1. 升采样：在每个符号点之间插入 (upsampleFactor - 1) 个零
            int reconstructedLength = symbols.Length * upsampleFactor;
            Complex[] upsampledSignal = new Complex[reconstructedLength];
            for (int i = 0; i < symbols.Length; i++)
            {
                // 将原始符号点放置到新数组的对应位置
                upsampledSignal[i * upsampleFactor] = symbols[i];
                // 其他位置默认为 Complex(0, 0)
            }

            // 2. 设计第一级根升余弦 (RRC) 滤波器
            //    这个滤波器将作为脉冲成型滤波器
            Double[] rrcFilterCoeffs = VSA.RootRaisedCosFilter(beta, span, upsampleFactor).ToArray();

            // 3. 应用第一级RRC滤波
            //    分别对I路和Q路进行滤波
            Double[] filteredI_stage1 = ConvolveMatlabFilter(
                upsampledSignal.Select(c => c.Real).ToArray(),
                rrcFilterCoeffs
            );
            Double[] filteredQ_stage1 = ConvolveMatlabFilter(
                upsampledSignal.Select(c => c.Imaginary).ToArray(),
                rrcFilterCoeffs
            );

            // 4. 应用第二级RRC滤波 (模拟接收端的匹配滤波器)
            //    输入是第一级滤波的输出
            Double[] filteredI_stage2 = ConvolveMatlabFilter(filteredI_stage1, rrcFilterCoeffs);
            Double[] filteredQ_stage2 = ConvolveMatlabFilter(filteredQ_stage1, rrcFilterCoeffs);

            // 5. 组合最终的重建波形
            //    注意：两级滤波会引入群延迟，我们需要补偿它。
            //    每个滤波器的延迟大约是 (滤波器长度 - 1) / 2。
            //    滤波器长度 = span * sps + 1。
            int filterLength = (span * upsampleFactor) + 1;
            int totalDelay = filterLength - 1; // 两个filter()函数的总延迟

            Complex[] reconstructedWaveform = filteredI_stage2
                .Zip(filteredQ_stage2, (i, q) => new Complex(i, q))
                .Skip(totalDelay) // 跳过由两级滤波引入的总延迟
                .ToArray();

            return reconstructedWaveform;
        }

        /// <summary>
        /// 消除离散调制的相位歧义（例如 QPSK/QAM 的 90° 等价旋转），
        /// 避免出现“每次锁定角度不同导致星座时对时错”的现象。
        /// </summary>
        private static Complex[] ResolvePhaseAmbiguity(Complex[] input, Complex[] referenceConstellation, VsaFormatOpt formatOpt)
        {
            if (input == null || input.Length == 0 || referenceConstellation == null || referenceConstellation.Length == 0)
            {
                return input ?? Array.Empty<Complex>();
            }

            int rotationCount = formatOpt switch
            {
                VsaFormatOpt.BPSK => 2,
                VsaFormatOpt.PSK8 => 8,
                _ => 4
            };

            int evalCount = Math.Min(1000, input.Length);
            double bestCost = double.MaxValue;
            int bestK = 0;

            for (int k = 0; k < rotationCount; k++)
            {
                double angle = 2.0 * Math.PI * k / rotationCount;
                Complex rot = Complex.Exp(Complex.ImaginaryOne * angle);
                double cost = 0.0;

                for (int i = 0; i < evalCount; i++)
                {
                    Complex v = input[i] * rot;
                    double minDist2 = double.MaxValue;
                    for (int j = 0; j < referenceConstellation.Length; j++)
                    {
                        Complex d = v - referenceConstellation[j];
                        double dist2 = d.Real * d.Real + d.Imaginary * d.Imaginary;
                        if (dist2 < minDist2)
                        {
                            minDist2 = dist2;
                        }
                    }
                    cost += minDist2;
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestK = k;
                }
            }

            if (bestK == 0)
            {
                return input;
            }

            double bestAngle = 2.0 * Math.PI * bestK / rotationCount;
            Complex bestRot = Complex.Exp(Complex.ImaginaryOne * bestAngle);
            Complex[] output = new Complex[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] * bestRot;
            }
            return output;
        }
        #endregion
        //#if UESTC_DIST
        /// <summary>
        /// 计算各个调制格式下的理想坐标点
        /// </summary>
        //private readonly Dictionary<VsaFormatOpt, Func<IEnumerable<Complex>, IEnumerable<Complex>>> _FindTheoryPosArrayTable = new Dictionary<VsaFormatOpt, Func<IEnumerable<Complex>, IEnumerable<Complex>>>()
        //        {
        //            {VsaFormatOpt.QPSK, TheroryPosion.QPSKArray},
        //            {VsaFormatOpt.QAM16, TheroryPosion.QAM16 },
        //            {VsaFormatOpt.PSK8,  TheroryPosion.Psk8Array},
        //            {VsaFormatOpt.BPSK,  TheroryPosion.BPSKArray},
        //        };
        //private readonly Dictionary<VsaFormatOpt, Func<Complex, Complex>> _FindTheoryPosTable = new Dictionary<VsaFormatOpt, Func<Complex, Complex>>()
        //        {
        //            {VsaFormatOpt.QPSK, TheroryPosion.QPSK},
        //            {VsaFormatOpt.QAM16,TheroryPosion.QAM16},
        //            {VsaFormatOpt.PSK8,  TheroryPosion.Psk8},
        //            {VsaFormatOpt.BPSK,  TheroryPosion.BPSK},
        //        };
        //private readonly Dictionary<VsaFormatOpt, Func<Complex, Boolean, Complex>> _FindTheoryPosPfdTable = new Dictionary<VsaFormatOpt, Func<Complex, Boolean, Complex>>()
        //        {
        //            {VsaFormatOpt.QAM16,TheroryPosion.QAM16PFD},
        //        };

        /// <summary>
        /// 不同调制格式对应的增益归一化系数
        /// </summary>
        private readonly Dictionary<VsaFormatOpt, Double> _VsaAmplidCoe = new Dictionary<VsaFormatOpt, Double>()
                {
                    {VsaFormatOpt.QPSK,  Math.Pow(2, 0.5)},
                    {VsaFormatOpt.QAM16,  3.0},
                    {VsaFormatOpt.PSK8,  Math.Pow(2, 0.5)},
                    {VsaFormatOpt.BPSK,  1.0},
                };

        //        /// <summary>
        //        /// 不同调制格式下的PFD判决区域表
        //        /// </summary>

        private readonly Dictionary<VsaFormatOpt, Double[]> _PfdThresholdAmplTable = new Dictionary<VsaFormatOpt, Double[]>()
        {
            { VsaFormatOpt.QAM16,  new Double[2]{Math.Sqrt(5.0), Math.Sqrt(13.0)} },
        };


        /// <summary>
        /// 计算不同需求下的关于VSA的测量滤波器
        /// </summary>
        private readonly Dictionary<VsaMeasureFilterTypeOpt, Func<Double, Int32, Int32, IEnumerable<Double>>> _VsaMeasureFilter = new Dictionary<VsaMeasureFilterTypeOpt, Func<Double, Int32, Int32, IEnumerable<Double>>>()
        {
            {VsaMeasureFilterTypeOpt.RootRaisedCosine,  VSA.RootRaisedCosFilter},
            {VsaMeasureFilterTypeOpt.RaisedCosine,  VSA.RaisedCosFilter},
        };

        /// <summary>
        /// 计算不同需求下的关于VSA的参考滤波器
        /// </summary>
        private readonly Dictionary<VsaRefFilterTypeOpt, Func<Double, Int32, Int32, IEnumerable<Double>>> _VsaRefFilter = new Dictionary<VsaRefFilterTypeOpt, Func<Double, Int32, Int32, IEnumerable<Double>>>()
        {
            {VsaRefFilterTypeOpt.RaisedCosine,  VSA.RaisedCosFilter},
        };

        /// <summary>
        /// 均衡对应的不同过采样率表
        /// </summary>
        private readonly Dictionary<EqualizeOverSampling, Int32> _OverSampleRateTable = new Dictionary<EqualizeOverSampling, Int32>()
        {
            {EqualizeOverSampling.OverSample1,  1},
            {EqualizeOverSampling.OverSample2,  2},
            {EqualizeOverSampling.OverSample3,  4},
            {EqualizeOverSampling.OverSample4,  8},
        };

        //#endif
        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add => _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            remove => _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

static class ErrCaculate
{
    public static ErrParam CalcParam(IEnumerable<Complex> dataIn, IEnumerable<Complex> idealSignal, IEnumerable<double> EVM, IEnumerable<double> PhaseErr, IEnumerable<double> amplErr)
    {
        ErrParam errParam = new ErrParam();
        IEnumerable<double> source = EVM.Select((double o) => Math.Pow(o, 2.0));
        IEnumerable<double> source2 = dataIn.Select((Complex s) => Math.Pow(s.Real, 2.0) + Math.Pow(s.Imaginary, 2.0));
        errParam.EvmRms = Math.Sqrt(source.Average()) / Math.Sqrt(source2.Average()) * 100.0;
        IEnumerable<double> enumerable = dataIn.Select((Complex s) => s.Phase);
        errParam.PERms = Math.Sqrt(PhaseErr.Select((double o) => Math.Pow(o, 2.0)).Average()) * 180.0 / Math.PI;
        errParam.AmplErrRms = amplErr.Average() / idealSignal.Select((Complex o) => o.Magnitude).Average();
        errParam.imbalanceIQ = (dataIn.Select((Complex o) => o.Imaginary).Average() / dataIn.Select((Complex o) => o.Real).Average() - 1.0) * 100.0;
        errParam.SNR = 1.0 / Math.Pow(errParam.EvmRms, 2.0);
        errParam.SNRdB = -20.0 * Math.Log10(errParam.EvmRms);
        return errParam;
    }
}

public class ErrParam
{
    public double EvmRms { get; set; }

    public double PERms { get; set; }

    public double AmplErrRms { get; set; }

    public double offsetI { get; set; }

    public double offsetQ { get; set; }

    public double imbalanceIQ { get; set; }

    public double SNR { get; set; }

    public double SNRdB { get; set; }
}
