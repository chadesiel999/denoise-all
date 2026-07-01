using MathWorks.MATLAB.NET.Arrays;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Transforms.Onnx;
using NPOI.POIFS.Properties;
using PreProcess;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using NPOI.POIFS.Properties;
//using NPOI.HPSF;
//using NPOI.POIFS.Crypt.Dsig;

namespace ScopeX.Core
{
    internal class IntelligentChartManager
    {
        public Boolean Enable
        {
            get;
            set;
        } = false;
        private MLContext _MLContext = new MLContext();
        private OnnxScoringEstimator? _Estimator;

        private String _ModelPath = "./onnx_model.onnx";

        public String ModelPath
        {
            get { return _ModelPath; }
            set
            {
                if (_ModelPath != value)
                {
                    _ModelPath = value;
                }
            }
        }
        private ChannelId _Source = ChannelId.C1;

        public ChannelId Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                }
            }
        }

        private List<float[]> ReadDataFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new List<float[]>();
            }
            StreamReader sr = new StreamReader(fileName);
            List<float> data = new List<float>();
            List<float[]> datas = new List<float[]>();
            Int32 times = 0;
            while ((!sr.EndOfStream))
            {
                var newline = sr.ReadLine();
                if (newline != null)
                {
                    var arr = newline.Split(' ');
                    foreach (var item in arr)
                    {
                        data.Add(float.Parse(item));
                    }
                    datas.Add(data.ToArray());
                    data.Clear();
                }
                times++;
            }
            sr.Close();
            return datas;
        }

        private double[] ReadSingleColumnDoubleData(string fileName)
        {
            // 1. 检查文件是否存在
            if (!System.IO.File.Exists(fileName))
            {
                return new double[0];
            }

            List<double> dataList = new List<double>();

            // 2. 使用 StreamReader 读取文件
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    // 3. 校验非空并解析
                    // 使用 TryParse 比 Parse 更安全，能防止因空行或非数字字符导致的崩溃
                    if (!string.IsNullOrWhiteSpace(line) && double.TryParse(line.Trim(), out double val))
                    {
                        dataList.Add(val);
                    }
                }
            }

            // 4. 转为数组返回
            return dataList.ToArray();
        }

        private float[] ReadSingleColumnFloatData(string fileName)
        {
            // 1. 检查文件是否存在
            if (!System.IO.File.Exists(fileName))
            {
                return new float[0];
            }

            List<float> dataList = new List<float>();

            // 2. 使用 StreamReader 读取文件
            using (System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    // 3. 校验非空并解析
                    // 使用 TryParse 比 Parse 更安全，能防止因空行或非数字字符导致的崩溃
                    if (!string.IsNullOrWhiteSpace(line) && float.TryParse(line.Trim(), out float val))
                    {
                        dataList.Add(val);
                    }
                }
            }

            // 4. 转为数组返回
            return dataList.ToArray();
        }
        private Double[] _DataReading = new Double[0];
        private Double _DataReadingSampleFreq = 0.1;
        private volatile Boolean _IsUpdateReading = false;
        private Object _Lock = new Object();

        public void SetSampleData(UInt16[] data, Double sampleInterval)
        {
            if (sampleInterval.Equals(0))
                return;
            lock (_Lock)
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel((ChannelId)Source);
                var pos0 = ach.Conditioning.PosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                var ratio = ach.Conditioning.Scale / Constants.SAMPS_PER_YDIV;
                _DataReading = data.Select(o => (Double)(o - pos0) * ratio).ToArray();
                _DataReadingSampleFreq = 1 / sampleInterval;
                _IsUpdateReading = true;
            }
        }
        private Double[] _DataReaded;
        private Double _DataReadedSampleFreq = 0.1;
        private void SwitchData()
        {
            lock (_Lock)
            {
                if (!_IsUpdateReading)
                    return;
                _DataReaded = _DataReading.ToArray();
                _DataReadedSampleFreq = _DataReadingSampleFreq;
                _IsUpdateReading = false;
            }
        }

        internal String MatchTypeStr = String.Empty;
        public void Run()
        {
            try
            {
                double[,] data = new double[0, 0];
                MatchTypeStr = GetMatchType(data);
                //List<ChartType> chartTypes = GetChartList(new List<string> { matchType });
                List<ChartType> chartTypes = GetChartList(new List<string> { "" });
                ChannelId child1 = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;

                if (DsoModel.Default.TryGetChannel(child1, out var Ich))
                {
                    if (DsoModel.Default.ArtificialIntelligence.CurAiParamsEnable)
                    {
                       // SwitchMeasureParameters(child1);
                    }
                }
                OpenCharts(chartTypes);
                UpdateWaveData(data);
                //Thread.Sleep(1000);
            }
            catch
            { 
                
            }
        }

        public ONNXModelType oNNXModelType { get; set; } = ONNXModelType.AMR;

        public void SetModelType(ONNXModelType modelType)
        {
            oNNXModelType = modelType;
        }

        private InferenceSession _session;
        private ONNXModelType _currentLoadedType; // 记录当前加载的模型类型
        private readonly Object _PeriodHistRawLock = new();
        private readonly Object _InferenceLock = new();
        private UInt16[] _PrevPeriodHistRaw = Array.Empty<UInt16>();
        private Double _PrevPeriodHistSampleRate = 0;
        private Int32 _LongWindowCooldownFrames = 0;
        private String _StablePeriodHistMatchType = String.Empty;
        private String _PendingPeriodHistMatchType = String.Empty;
        private Int32 _PendingPeriodHistHitCount = 0;
        private const Single _LowConfidenceGapThreshold = 0.08f;
        private const Single _ShortWindowWeight = 0.55f;
        private const Single _LongWindowWeight = 0.45f;
        private const Single _SignalChangeRmsRatioThreshold = 0.35f;
        private const Single _SignalChangeP2PRatioThreshold = 0.45f;
        private const Int32 _LongWindowCooldownFrameCount = 2;
        private const Int32 _SwitchStableFrameRequired = 2;

        public String GetMatchType(double[,] data)
        {
            ONNXModelType oNNXModelType = ONNXModelType.PeriodHist;

            String modelPath = GetPath(oNNXModelType);
            //var gpuSessionOptions = SessionOptions.MakeSessionOptionWithCudaProvider(0);
            //var session = new InferenceSession(modelPath, gpuSessionOptions);
            // 性能优化：只有当 session 为空，或者模型类型发生改变时才重新加载
            if (true || _session == null || _currentLoadedType != oNNXModelType)
            {
                // 释放旧的 session
                _session?.Dispose();

                try
                {
                    // 加载新模型 (这是一个耗时操作，只做一次)
                    _session = new InferenceSession(modelPath);
                    _currentLoadedType = oNNXModelType;
                }
                catch (Exception ex)
                {
                    // 处理模型加载失败的情况，防止崩溃
                    return "Error";
                }
            }

            //if (oNNXModelType == ONNXModelType.PeriodHist)
            //{
            //    return GetPeriodHistMatchTypeAdaptive();
            //}

            //if (oNNXModelType == ONNXModelType.PeriodHist)
            //    return GetMatchTypeByMajorityVote(oNNXModelType, data, 7, 20);

            return PredictOnce(oNNXModelType, data);
       }

        private String GetMatchTypeByMajorityVote(ONNXModelType modelType, double[,] data, Int32 voteCount, Int32 voteIntervalMs)
        {
            Dictionary<String, Int32> counts = new(StringComparer.OrdinalIgnoreCase);
            String lastValid = String.Empty;

            for (Int32 i = 0; i < voteCount; i++)
            {
                String matchType = PredictOnce(modelType, data);
                if (!String.IsNullOrWhiteSpace(matchType) && !String.Equals(matchType, "Error", StringComparison.OrdinalIgnoreCase))
                {
                    lastValid = matchType;
                    if (!counts.TryAdd(matchType, 1))
                        counts[matchType]++;
                }

                if (i < voteCount - 1 && voteIntervalMs > 0)
                    Thread.Sleep(voteIntervalMs);
            }

            if (counts.Count == 0)
                return lastValid;

            return counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
                .First()
                .Key;
        }

        private String PredictOnce(ONNXModelType oNNXModelType, double[,] data)
        {
            ONNXParam onnxParm = GetDenseBuffer(oNNXModelType, data);
            DenseTensor<float> denseBuffer = onnxParm.DenseTensor;
            if (denseBuffer == null || onnxParm.MatchType == null || onnxParm.MatchType.Length == 0)
                return String.Empty;

            NamedOnnxValue named = NamedOnnxValue.CreateFromTensor(onnxParm.InputName, denseBuffer);
            var inputs = new List<NamedOnnxValue> { named };
            List<DisposableNamedOnnxValue> outputs;
            lock (_InferenceLock)
            {
                outputs = _session.Run(inputs).ToList();
            }

            Int32 index = GetIndexOfMaxvalue(outputs);
            if (index < 0 || index >= onnxParm.MatchType.Length)
                return String.Empty;

            return onnxParm.MatchType[index];
        }

        private String GetPeriodHistMatchTypeAdaptive()
        {
            ONNXParam onnxParam = CreatePeriodHistParam();
            ChannelId chnl = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;
            var pkg = DsoModel.Default.ArtificialIntelligence.GetData(chnl);
            if (pkg?.data == null || pkg.data.Length == 0)
                return "";

            Double fs = DsoModel.Default.Timebase.AnalogSamplingRate;
            if (_PrevPeriodHistSampleRate > 0)
            {
                Double fsRatio = Math.Abs(fs - _PrevPeriodHistSampleRate) / Math.Max(_PrevPeriodHistSampleRate, 1.0);
                if (fsRatio > 0.01)
                {
                    ResetPeriodHistWindowState();
                }
            }
            _PrevPeriodHistSampleRate = fs;

            var (shortWindow, longWindow) = PreparePeriodHistWindows(pkg.data);
            DenseTensor<float>? shortTensor = BuildPeriodHistTensorFromRaw(shortWindow, fs, dumpCsv: true);
            if (shortTensor == null)
                return "";

            Single[] shortLogits = RunOnnxAndGetLogits(shortTensor, onnxParam.InputName);
            if (shortLogits.Length == 0)
                return "";

            Single[] merged = shortLogits;
            Single shortGap = GetTopGap(shortLogits);
            Single mergedGap = shortGap;
            if (longWindow != null && longWindow.Length > 0)
            {
                DenseTensor<float>? longTensor = BuildPeriodHistTensorFromRaw(longWindow, fs, dumpCsv: false);
                if (longTensor != null)
                {
                    Single[] longLogits = RunOnnxAndGetLogits(longTensor, onnxParam.InputName);
                    if (longLogits.Length == shortLogits.Length && longLogits.Length > 0)
                    {
                        Single longGap = GetTopGap(longLogits);
                        (Single shortWeight, Single longWeight) = GetAdaptiveFusionWeights(shortGap, longGap);
                        merged = MergeLogits(shortLogits, longLogits, shortWeight, longWeight);
                        mergedGap = GetTopGap(merged);
                    }
                }
            }
            else
            {
                mergedGap = shortGap;
            }

            Int32 index = GetTop1Index(merged);
            if (index < 0 || index >= onnxParam.MatchType.Length)
                return "";

            String candidate = onnxParam.MatchType[index];
            if (mergedGap < _LowConfidenceGapThreshold)
            {
                if (!String.IsNullOrWhiteSpace(_StablePeriodHistMatchType))
                {
                    return _StablePeriodHistMatchType;
                }

                Int32 shortTop = GetTop1Index(shortLogits);
                if (shortTop >= 0 && shortTop < onnxParam.MatchType.Length)
                    candidate = onnxParam.MatchType[shortTop];
            }

            return ApplyPeriodHistHysteresis(candidate, mergedGap);
        }

        private (UInt16[] shortWindow, UInt16[]? longWindow) PreparePeriodHistWindows(UInt16[] currentRaw)
        {
            lock (_PeriodHistRawLock)
            {
                if (_PrevPeriodHistRaw.Length > 0 && IsSignalChangedAbruptly(_PrevPeriodHistRaw, currentRaw))
                {
                    _LongWindowCooldownFrames = _LongWindowCooldownFrameCount;
                    _PrevPeriodHistRaw = currentRaw.ToArray();
                    return (currentRaw, null);
                }

                if (_LongWindowCooldownFrames > 0)
                {
                    _LongWindowCooldownFrames--;
                    _PrevPeriodHistRaw = currentRaw.ToArray();
                    return (currentRaw, null);
                }

                UInt16[]? longWindow = null;
                if (_PrevPeriodHistRaw.Length > 0)
                {
                    longWindow = new UInt16[_PrevPeriodHistRaw.Length + currentRaw.Length];
                    Buffer.BlockCopy(_PrevPeriodHistRaw, 0, longWindow, 0, _PrevPeriodHistRaw.Length * sizeof(UInt16));
                    Buffer.BlockCopy(currentRaw, 0, longWindow, _PrevPeriodHistRaw.Length * sizeof(UInt16), currentRaw.Length * sizeof(UInt16));
                }

                _PrevPeriodHistRaw = currentRaw.ToArray();
                return (currentRaw, longWindow);
            }
        }

        private void ResetPeriodHistWindowState()
        {
            lock (_PeriodHistRawLock)
            {
                _PrevPeriodHistRaw = Array.Empty<UInt16>();
                _LongWindowCooldownFrames = 0;
            }
        }

        private static (Double rms, Double p2p) GetRawStats(UInt16[] data)
        {
            if (data.Length == 0)
                return (0, 0);

            Double sum = 0;
            UInt16 min = UInt16.MaxValue;
            UInt16 max = UInt16.MinValue;
            for (Int32 i = 0; i < data.Length; i++)
            {
                UInt16 v = data[i];
                sum += v;
                if (v < min) min = v;
                if (v > max) max = v;
            }
            Double mean = sum / data.Length;
            Double sq = 0;
            for (Int32 i = 0; i < data.Length; i++)
            {
                Double d = data[i] - mean;
                sq += d * d;
            }
            Double rms = Math.Sqrt(sq / Math.Max(data.Length, 1));
            Double p2p = max - min;
            return (rms, p2p);
        }

        private static Boolean IsSignalChangedAbruptly(UInt16[] prev, UInt16[] current)
        {
            if (prev.Length == 0 || current.Length == 0)
                return false;

            var prevStats = GetRawStats(prev);
            var currStats = GetRawStats(current);

            Double rmsBase = Math.Max(prevStats.rms, 1.0);
            Double p2pBase = Math.Max(prevStats.p2p, 1.0);
            Double rmsRatio = Math.Abs(currStats.rms - prevStats.rms) / rmsBase;
            Double p2pRatio = Math.Abs(currStats.p2p - prevStats.p2p) / p2pBase;
            return rmsRatio > _SignalChangeRmsRatioThreshold || p2pRatio > _SignalChangeP2PRatioThreshold;
        }

        private static (Single shortWeight, Single longWeight) GetAdaptiveFusionWeights(Single shortGap, Single longGap)
        {
            if (shortGap <= 0 && longGap <= 0)
                return (_ShortWindowWeight, _LongWindowWeight);

            Single shortWeight = 0.5f;
            Single longWeight = 0.5f;
            Single sum = shortGap + longGap;
            if (sum > 0)
            {
                shortWeight = shortGap / sum;
                longWeight = longGap / sum;
            }

            if (longGap < _LowConfidenceGapThreshold)
                longWeight *= 0.5f;

            Single norm = Math.Max(shortWeight + longWeight, 1e-6f);
            shortWeight /= norm;
            longWeight /= norm;
            return (shortWeight, longWeight);
        }

        private String ApplyPeriodHistHysteresis(String candidate, Single confidenceGap)
        {
            if (String.IsNullOrWhiteSpace(candidate))
                return _StablePeriodHistMatchType;

            if (String.IsNullOrWhiteSpace(_StablePeriodHistMatchType))
            {
                _StablePeriodHistMatchType = candidate;
                _PendingPeriodHistMatchType = String.Empty;
                _PendingPeriodHistHitCount = 0;
                return candidate;
            }

            if (candidate == _StablePeriodHistMatchType)
            {
                _PendingPeriodHistMatchType = String.Empty;
                _PendingPeriodHistHitCount = 0;
                return _StablePeriodHistMatchType;
            }

            if (confidenceGap < _LowConfidenceGapThreshold * 1.2f)
                return _StablePeriodHistMatchType;

            if (_PendingPeriodHistMatchType == candidate)
            {
                _PendingPeriodHistHitCount++;
            }
            else
            {
                _PendingPeriodHistMatchType = candidate;
                _PendingPeriodHistHitCount = 1;
            }

            if (_PendingPeriodHistHitCount >= _SwitchStableFrameRequired)
            {
                _StablePeriodHistMatchType = candidate;
                _PendingPeriodHistMatchType = String.Empty;
                _PendingPeriodHistHitCount = 0;
            }

            return _StablePeriodHistMatchType;
        }

        private ONNXParam CreatePeriodHistParam()
        {
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 512;
            onnxParam.InputCount = 1;
            onnxParam.MatchType = new string[] { "64QAM", "Sine", "QPSK", "BPSK", "8PSK", "Tri", "SFM", "16QAM", "Square" };
            return onnxParam;
        }

        private DenseTensor<float>? BuildPeriodHistTensorFromRaw(UInt16[] raw, Double sampleRate, Boolean dumpCsv)
        {
            if (raw.Length == 0)
                return null;

            Double[] bufferDouble = Array.ConvertAll(raw, o => (Double)o);
            MWNumericArray data = new(bufferDouble);
            MWArray[] res = signalPreProcess.PreProcess(1, data, sampleRate);
            MWNumericArray tmp = (MWNumericArray)res[0];
            Double[] features = (Double[])tmp.ToVector(MWArrayComponent.Real);
            if (features.Length == 0)
                return null;

            if (dumpCsv)
            {
                Double[] dataCopy = features.ToArray();
                Task.Run(() =>
                {
                    try
                    {
                        SaveUpdateDataAsCsv(ChannelId.C1, dataCopy);
                    }
                    catch (Exception ex)
                    {
                    }
                });
            }

            DenseTensor<float> denseBuffer = new(new[] { 1, 512, 1 });
            Int32 copyLength = Math.Min(512, features.Length);
            for (Int32 i = 0; i < copyLength; i++)
                denseBuffer[0, i, 0] = (Single)features[i];
            return denseBuffer;
        }

        private Single[] RunOnnxAndGetLogits(DenseTensor<float> denseBuffer, String inputName)
        {
            NamedOnnxValue named = NamedOnnxValue.CreateFromTensor(inputName, denseBuffer);
            var inputs = new List<NamedOnnxValue> { named };
            using var outputs = _session.Run(inputs);
            if (outputs.Count == 0)
                return Array.Empty<Single>();

            DenseTensor<Single> tensor = (DenseTensor<Single>)outputs.First().Value;
            return tensor.ToArray();
        }

        private static Single[] MergeLogits(Single[] shortLogits, Single[] longLogits, Single shortWeight, Single longWeight)
        {
            var merged = new Single[shortLogits.Length];
            for (Int32 i = 0; i < merged.Length; i++)
            {
                merged[i] = shortLogits[i] * shortWeight + longLogits[i] * longWeight;
            }
            return merged;
        }

        private static Int32 GetTop1Index(Single[] values)
        {
            if (values.Length == 0)
                return -1;

            Int32 bestIndex = 0;
            Single bestValue = values[0];
            for (Int32 i = 1; i < values.Length; i++)
            {
                if (values[i] > bestValue)
                {
                    bestValue = values[i];
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        private static Int32 GetSecondIndex(Single[] values, Int32 top1Index)
        {
            if (values.Length < 2 || top1Index < 0 || top1Index >= values.Length)
                return -1;

            Int32 second = top1Index == 0 ? 1 : 0;
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (i == top1Index)
                    continue;
                if (values[i] > values[second])
                    second = i;
            }
            return second;
        }

        private static Single GetTopGap(Single[] values)
        {
            Int32 top1 = GetTop1Index(values);
            Int32 top2 = GetSecondIndex(values, top1);
            if (top1 < 0 || top2 < 0)
                return 0;
            return Math.Abs(values[top1] - values[top2]);
        }
        public enum ONNXModelType
        {
            AMR,
            Anomaly,
            Period,
            PeriodHist,  //通过直方图识别信号
        }
        private Int32 GetIndexOfMaxvalue(List<DisposableNamedOnnxValue> outputs)
        {
            var max = ((DenseTensor<float>)outputs[0].Value).Max();
            Int32 index = -1;
            var valueList = ((DenseTensor<float>)outputs[0].Value).ToList();
            for (int i = 0; i < valueList.Count; i++)
            {
                if (max == valueList[i])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private String GetPath(ONNXModelType oNNXModelType)
        {
            string amrPath = @"AI/TensorModels/AMR_CNN_torch_best.onnx";
            string anomalyPath = @"AI/TensorModels/class_model_real.onnx";
            string periodPath = @"AI/TensorModels/classification_torch.onnx";
            string periodHistPath = @"AI/TensorModels/all_best_0401.onnx"; //@"AI/TensorModels/all_best.onnx";

            String modelPath = "";
            switch (oNNXModelType)
            {
                case ONNXModelType.AMR:
                    modelPath = amrPath;
                    break;
                case ONNXModelType.Anomaly:
                    modelPath = anomalyPath;
                    break;
                case ONNXModelType.Period:
                    modelPath = periodPath;
                    break;
                case ONNXModelType.PeriodHist: 
                    modelPath = periodHistPath;
                    break;
                default:
                    break;
            }
            return modelPath;
        }

        private ONNXParam GetDenseBuffer(ONNXModelType oNNXModelType, double[,] data)
        {
            switch (oNNXModelType)
            {
                case ONNXModelType.AMR:
                    return GetAMRDenseBuffer(new float[128], new float[128], false);
                case ONNXModelType.Anomaly:
                    return GetAnomalyDenseBuffer(new float[100], true);
                case ONNXModelType.Period:
                    return GetPeriodDenseBuffer(new float[128], false);
                case ONNXModelType.PeriodHist: // [新增]
                    return GetPeriodHistDenseBuffer();
                default:
                    break;
            }
            return new ONNXParam();
        }

        private static Int32 targetLength = 128;

        private ONNXParam GetAnomalyDenseBuffer(float[] data, Boolean testMode = false)
        {
            var DataPath = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\AnomalyData\\";
            ONNXParam onnxParam = new ONNXParam();
            Int32 SNR = 30;
            onnxParam.InputName = "input";
            onnxParam.DataLength = 100;
            onnxParam.InputCount = 1;
            onnxParam.MatchType = new string[] { "VoltInc", "ShutDown", "FreqDec", "VoltDec", "FreqInc" };
            onnxParam.DetectType = new string[] { "volt_inc", "shut_down", "freq_dec", "volt_dec", "freq_inc" };
            onnxParam.Path = @"AI/TensorModels/class_model_real.onnx";
            if (testMode)
            {
                //var d = ReadDataFromFile(Directory.GetCurrentDirectory() + "..\\..\\..\\..\\AnomalyData\\" + onnxParam.DetectType[4] + ".txt");
                var d = ReadDataFromFile( DataPath + onnxParam.DetectType[4] + ".txt");
                if (d.Count > 0)
                {
                    data = d[0];
                }
            }
            for (int i = 0; i < data.Length; i++)
            {
                data = Normalize(data);
            }

            DenseTensor<float> denseBuffer = new(new[] { 1, 100, 1 });

            for (int i = 0; i < data.Length; i++)
                denseBuffer[0, i, 0] = (float)data[i];

            onnxParam.DenseTensor = denseBuffer;
            return onnxParam;
        }
        private ONNXParam GetAMRDenseBuffer(float[] datai, float[] dataq, Boolean testMode = false)
        {
            var DataPath = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\AnomalyData\\mode_data\\";
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 996;
            onnxParam.InputCount = 2;
            onnxParam.MatchType = new string[] { "BPSK", "QPSK", "PSK8", "QAM16", "QAM32", "QAM64", "QAM128", "QAM256" };
            onnxParam.DetectType = new string[] { "BPSK", "QPSK", "PSK8", "QAM16", "QAM32", "QAM64", "QAM128", "QAM256" };
            //onnxParam.Path = @"AI/TensorModels/classification_torch.onnx";

            if (testMode)
            {
                var data = ReadDataFromFile(DataPath + onnxParam.DetectType[4] + "\\" + "data.txt");


                for (int i = 0; i < data.Count; i++)
                {
                    datai[i] = data[i][0];
                    dataq[i] = data[i][1];
                }
                //datai = ExtractPeaks(Inums, targetLength);
                //dataq = ExtractPeaks(Qnums, targetLength);
                datai = Normalize(datai);
                if (dataq[0] != 0 )
                {
                    dataq = Normalize(dataq);
                }

                // var q = ReadDataFromFile(Directory.GetCurrentDirectory() + "\\AI\\AnomalyData\\" + onnxParam.DetectType[0] + "_Q");

            }
            else
            {
                ChannelId child1 = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;

                if (DsoModel.Default.TryGetChannel(child1, out var Ich))
                {
                    if (Ich.VuDatabase.Current != null)
                    {
                        Double[,] bufferIQ = Ich.VuDatabase.Current.Buffer;
                        int rows = bufferIQ.GetLength(1);
                        double[] secondvumdata = new double[rows];
                        for (int i = 0; i < rows; i++)
                        {
                            secondvumdata[i] = (double)bufferIQ[0, i];
                        }
                        //var fileName_RF = "BPSK.txt";
                        //StreamWriter sw_RF = new StreamWriter(fileName_RF, true);
                        //for (Int32 i = 0; i < secondvumdata.Length; i++)
                        //{
                        //    sw_RF.WriteLine(secondvumdata[i]);
                        //}
                        //sw_RF.Flush();
                        //sw_RF.Close();

                        var demodulated = DataIQ(secondvumdata, out float[] secondvumdatai, out float[] secondvumdataq);
                        float[] secondvumdatai1 = new float[rows - 42];
                        float[] secondvumdataq1 = new float[rows - 42];
                        for (int i = 0; i < rows - 42; i++)
                        {
                            secondvumdatai1[i] = secondvumdatai[i + 42];
                            secondvumdataq1[i] = secondvumdataq[i + 42];
                        }
                        datai = ExtractData(secondvumdatai1, 10);
                        dataq = ExtractData(secondvumdataq1, 10);
                        Normalize(datai);
                        Normalize(dataq);
                    }
                }
            }
            //var fileNameI = "AMR_QAM256_I.txt";
            //var fileNameQ = "AMR_QAM256_Q.txt";
            //StreamWriter swI = new StreamWriter(fileNameI, true);
            //StreamWriter swQ = new StreamWriter(fileNameQ, true);
            //for (Int32 i = 0; i < datai.Length; i++)
            //{
            //    swI.WriteLine(datai[i]);
            //    swQ.WriteLine(dataq[i]);
            //}
            //swI.Flush();
            //swI.Close();
            //swQ.Flush();
            //swQ.Close();

            //if (datai.Length != dataq.Length)
            //{
            //    return onnxParam;
            //}

            Int32 length = datai.Length;
            //Int32 length = 1024;
            DenseTensor<float> denseBuffer = new(new[] { 1, length, 2 });

            for (int i = 0; i < length; i++)
            {
                
                denseBuffer[0, i, 0] = (float)datai[i];
                denseBuffer[0, i, 1] = (float)dataq[i];
            }
            onnxParam.DenseTensor = denseBuffer;
            return onnxParam;
        }
        private ONNXParam GetPeriodDenseBuffer(float[] datai, Boolean testMode = false)
        {
            var DataPath = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\AnomalyData\\period_";
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 128;
            onnxParam.InputCount = 1;
            onnxParam.MatchType = new string[] { "sin", "square", "tri" };
            if (testMode)
            {
                var data = ReadDataFromFile(DataPath + onnxParam.MatchType[0] + "\\" + "data.txt");
                for (int i = 0; i < data.Count; i++)
                {
                    datai[i] = data[i][0];
                }
                //datai = ExtractPeaks(Inums, targetLength);
                //dataq = ExtractPeaks(Qnums, targetLength);
                datai = Normalize(datai);

                // var q = ReadDataFromFile(Directory.GetCurrentDirectory() + "\\AI\\AnomalyData\\" + onnxParam.DetectType[0] + "_Q");
            }
            else
            {
                ChannelId child1 = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;
                float[] siganldata = new float[targetLength];
                if (DsoModel.Default.TryGetChannel(child1, out var Ich))
                {
                    if (Ich.VuDatabase.Current != null)
                    {
                        Double[,] bufferI = Ich.VuDatabase.Current.Buffer;
                        int rows = bufferI.GetLength(1);
                        float[] secondvumdata = new float[rows];
                        for (int i = 0; i < rows; i++)
                        {
                            secondvumdata[i] = (float)bufferI[0, i];
                        }
                        siganldata = ExtractPeaks(secondvumdata, targetLength);
                        datai = Normalize(siganldata);
                    }
                }
            }
            //var fileName = "Square30_200.txt";
            //StreamWriter sw = new StreamWriter(fileName, true);
            //for (Int32 i = 0; i < datai.Length; i++)
            //{
            //    sw.WriteLine(datai[i]);
            //}
            //sw.Flush();
            //sw.Close();

            DenseTensor<float> denseBuffer = new(new[] { 1, 128, 1 });

            for (int i = 0; i < datai.Length; i++)
                denseBuffer[0, i, 0] = (float)datai[i];

            onnxParam.DenseTensor = denseBuffer;
            return onnxParam;
        }

        SignalPreProcess signalPreProcess = new();

        private ONNXParam GetPeriodHistDenseBuffer() 
        {
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 512;
            onnxParam.InputCount = 1;
            onnxParam.MatchType = new string[] { "SFM", "LFM", "AM", "QPSK", "BPSK", "8PSK", "16QAM", "Tri", "Sine", "Square" }; ; //0401
            //{ "SFM", "LFM", "AM", "QPSK", "8PSK", "16QAM", "Tri", "Sine", "Square" }; ; //0401
            //{ "QPSK", "BPSK", "8PSK", "16QAM", "Tri", "SFM", "Sine", "Square" }; //0328
            //{ "SFM", "LFM", "AM", "QPSK", "BPSK", "8PSK", "16QAM", "Tri", "Sine", "Square" }; 0331
            //{ "32QAM", "SFM", "8PSK", "Sine", "QPSK", "AM", "BPSK", "Tri", "64QAM", "16QAM",  "128QAM" };//0227
            //{ "64QAM", "SFM", "Sine", "QPSK", "BPSK","8PSK", "16QAM", };//0302
            //{ "16QAM", "square", "BPSK" };
            //            { "64QAM", "SFM", "LFM", "Pulse", "QPSK", "AM", "32QAM", "BPSK", "8PSK", "16QAM", "Tri", "128QAM", "Sine", "Square"};

            ChannelId child1 = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;
            var pkg = DsoModel.Default.ArtificialIntelligence.GetData(child1);
            if (pkg != null)
            {
             
                ushort[] buffer = pkg.data;
                double[] bufferDouble = Array.ConvertAll(buffer, o => (double)o);
                MWNumericArray data = new(bufferDouble);
                MWArray[] res = signalPreProcess.PreProcess(1, data, DsoModel.Default.Timebase.AnalogSamplingRate);
                MWNumericArray tmp = (MWNumericArray)res[0];
                double[] tmp1 = (double[])tmp.ToVector(MWArrayComponent.Real);
                double[] dataCopy = tmp1.ToArray();
                // 将新的数据保存为 CSV（异步，避免阻塞采集线程）
                //Task.Run(() =>
                //{
                //    try
                //    {
                //        SaveUpdateDataAsCsv(ChannelId.C1, dataCopy);
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //});

                DenseTensor<float> denseBuffer = new(new[] { 1, 512, 1 });

                for (int i = 0; i < tmp1.Length; i++)
                    denseBuffer[0, i, 0] = (float)tmp1[i];

                onnxParam.DenseTensor = denseBuffer;
            }

            return onnxParam;
        }

        private Int64 _UpdateDataCsvSeq = 0;
        private void SaveUpdateDataAsCsv(ChannelId chnlId, Double[] data)
        {
            // 默认落盘目录：我的文档\ScopeX\AI\UpdateDataCsv\<ChannelId>\
            String rootDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ScopeX",
                "AI",
                "UpdateDataCsv",
                chnlId.ToString());
            Directory.CreateDirectory(rootDir);

            Int64 seq = Interlocked.Increment(ref _UpdateDataCsvSeq);
            String fileName = $"UpdateData_{chnlId}_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{seq}.csv";
            String filePath = Path.Combine(rootDir, fileName);

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                // 统一用 InvariantCulture，避免小数点受本地化影响
                writer.WriteLine("Index,Time,Value");
                for (Int32 i = 0; i < data.Length; i++)
                {
                    writer.WriteLine(
                        $"{i}," +
                        $"{data[i]}");
                }
            }
        }

        //private ONNXParam GetPeriodHistDenseBuffer(Boolean testMode = false)
        //{
        //    ONNXParam onnxParam = new ONNXParam();
        //    onnxParam.InputName = "input";
        //    onnxParam.DataLength = 256;
        //    onnxParam.InputCount = 1;
        //    onnxParam.MatchType = new string[] { "sine", "square", "triangle" };

        //    // 1. 获取原始波形数据 (Double 数组)
        //    double[] rawData = new double[0];

        //    if (testMode)
        //    {
        //        // 测试模式逻辑...
        //        rawData = ReadSingleColumnDoubleData(@"D:\vsc#program\testsignal\ai\ai_signal\square_15000M_sequence.txt");
        //    }
        //    else
        //    {
        //        //ChannelId child1 = DsoModel.Default.ArtificialIntelligence.AiSetChnlId;
        //        //if (DsoModel.Default.TryGetChannel(child1, out var Ich))
        //        //{
        //        //    if (Ich.VuDatabase.Current != null)
        //        //    {
        //        //        Double[,] bufferI = Ich.VuDatabase.Current.Buffer;
        //        //        int rows = bufferI.GetLength(1);
        //        //        rawData = new double[rows];
        //        //        Buffer.BlockCopy(Ich.VuDatabase.Current.Buffer, 0, rawData, 0, rows * sizeof(double));
        //        //    }
        //        //}
        //        SwitchData();
        //        int rows = _DataReaded.GetLength(1);
        //        rawData = new double[rows];
        //        Buffer.BlockCopy(_DataReaded, 0, rawData, 0, rows * sizeof(double));
        //    }

        //    // 2. 复用 Histogram 类生成直方图
        //    // Histogram(Int32 nbins, Double[] buffer)
        //    // 注意：使用 using 语句或手动调用 Dispose() 以确保资源释放
        //    float[] histogramData = new float[256];

        //    if (rawData.Length > 0)
        //    {
        //        using (Histogram hist = new Histogram(256, rawData))
        //        {
        //            // Histogram 类会自动计算 Min, Max, BinWidth 并填充 HistBuffer
        //            // HistBuffer 是 Int32[]，我们需要将其转换为 float[] 并在此时做归一化

        //            // 获取总样本数 (TotalPop 或 rawData.Length) 用于归一化
        //            float hRange = (float)(hist.HistBuffer.Max() - hist.HistBuffer.Min());
        //            if (hRange == 0) hRange = 1; // 防止除零

        //            for (int i = 0; i < 256; i++)
        //            {
        //                // 如果直方图数据不足 256 (比如数据量极少)，hist.HistBuffer 长度仍是 nbins (256)
        //                // 取出计数值并归一化 (根据模型训练时的要求，通常是除以总点数)
        //                histogramData[i] = (float)(hist.HistBuffer[i] - hist.HistBuffer.Min()) / hRange;
        //            }
        //        }
        //    }
        //    //histogramData = ReadSingleColumnFloatData(@"D:\vsc#program\testsignal\ai\ai_signal\sine_1M_histogram.txt");
        //    // 3. 构建 Tensor (1, 256, 1)
        //    DenseTensor<float> denseBuffer = new(new[] { 1, 256, 1 });
        //    for (int i = 0; i < histogramData.Length; i++)
        //        denseBuffer[0, i, 0] = histogramData[i];

        //    onnxParam.DenseTensor = denseBuffer;
        //    return onnxParam;
        //}

        public static Boolean DataIQ(double[] data, out float[] dmodSignalI, out float[] dmodSignalQ)
        {

            Double sampleRateResample = 20E9;
            Double freqCarrier = 1E9;
            Boolean freqDownFlag = VSA.MixFreqDown(data, sampleRateResample, freqCarrier, out var dmodSignal);
            Double beta = 0.8;
            Double symBaudRate = 2E8;

            if (freqDownFlag)
            {
                Double[] dataI = dmodSignal.Select(o => o.Real).ToArray();
                Double[] dataQ = dmodSignal.Select(o => o.Imaginary).ToArray();

                Double[] dmodSignalI1 = ApplyLowPassFilterWithWindow(dataI, (1 + beta) * symBaudRate, sampleRateResample);
                Double[] dmodSignalQ1 = ApplyLowPassFilterWithWindow(dataQ, (1 + beta) * symBaudRate, sampleRateResample);
                dmodSignalI = dmodSignalI1.Select(o => (float)o).ToArray();
                dmodSignalQ = dmodSignalQ1.Select(o => (float)o).ToArray();
                return true;
            }
            else
            {
                dmodSignalI = null;
                dmodSignalQ = null;
                return false;
            }
        }
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
        public enum AnomalyType
        {
            Normal,
            ShutDown,
            Transient,
            TurnOn,
            VoltDec,
            VoltInc
        }
        public enum AMRType
        {

            PSK8,
            QAM16,
            QAM64,
            BPSK,
            QPSK
        }
        public enum PeriodType
        {

            AM,
            Sine,
            Square,
            Triangle
        }

        private (float[] Amp, float[] Phase) GetPhaseAmpFromIQ(float[] iData, float[] qData)
        {
            if (iData.Length != qData.Length)
            {
                return new();
            }
            Int32 length = iData.Length;
            float[] Amp = new float[length];
            float[] Phase = new float[length];
            for (int i = 0; i < length; i++)
            {
                System.Numerics.Complex complex = new System.Numerics.Complex(iData[i], qData[i]);
                Amp[i] = (float)complex.Magnitude;
                Phase[i] = (float)GetPhase(iData[i], qData[i]);
            }
            return (Amp, Phase);
        }
        private Double GetPhase(Double i, Double q)
        {
            Double phase = 0;
            if (i > 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i > 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i < 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180 + 180;
            else if (i < 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180 - 180;
            return phase;
        }
        public float[] Normalize(float[] inputData)
        {
            float sum = 0;
            float norm = 0;
            for (int i = 0; i < inputData.Length; i++)
                sum += inputData[i] * inputData[i];
            norm = (float)Math.Sqrt(Math.Abs(sum));
            if (norm == 0)
            {
                return new float[0];

            }
            for (int i = 0; i < inputData.Length; i++)
            {
                inputData[i] = inputData[i] / norm;
            }
            return inputData;
        }
        public static float[] ExtractPeaks(float[] signal, int targetLength)
        {
            float[] result = new float[targetLength];
            int groupSize = signal.Length * 2 / targetLength;

            if (signal.Length <= targetLength)
            {
                return result;
            }

            List<float> peaks = new List<float>();

            for (int i = 0; i < targetLength; i++)
            {
                int start = i * groupSize;
                int end = Math.Min(start + groupSize, signal.Length);

                if (start >= end)
                    break;

                int maxIndex = start;
                int minIndex = start;
                for (int j = start; j < end; j++)
                {
                    if (signal[j] > signal[maxIndex])
                        maxIndex = j;
                    if (signal[j] < signal[minIndex])
                        minIndex = j;
                }

                if (minIndex < maxIndex)
                {
                    peaks.Add(signal[minIndex]);
                    peaks.Add(signal[maxIndex]);
                }
                else
                {
                    peaks.Add(signal[maxIndex]);
                    peaks.Add(signal[minIndex]);
                }

                if (peaks.Count >= targetLength)
                {
                    break;
                }
            }
            if (peaks.Count > targetLength)
            {
                peaks = peaks.GetRange(0, targetLength);
            }

            for (int i = 0; i < peaks.Count; i++)
            {
                result[i] = peaks[i];
            }

            return result;
        }

        public static float[] ExtractData(float[] signal, int extractionFactor)
        {
            // 检查抽取倍率是否有效
            //if (extractionFactor <= 0 || extractionFactor > signal.Length)
            //{
            //    WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
            //}

            // 计算抽取后的数组长度
            int extractedLength = (signal.Length + extractionFactor - 1) / extractionFactor;//向上取整
            //extractedLength = 128;

            // 初始化抽取后的结果数组
            float[] extractedData = new float[extractedLength];

            // 按抽取倍率从信号数据中抽取数据
            for (int i = 0; i < extractedLength; i++)
            {
                int index = i * extractionFactor;

                // 确保索引在信号数据范围内
                if (index < signal.Length)
                {
                    extractedData[i] = signal[index];
                }
            }
            return extractedData;
        }
        public class HousingData
        {
            [LoadColumn(0)]
            public float Size { get; set; }

            [LoadColumn(1, 3)]
            [VectorType(3)]
            public float[] HistoricalPrices { get; set; }

            [LoadColumn(4)]
            [ColumnName("Label")]
            public float CurrentPrice { get; set; }
        }

        public const Int32 PredictLength = 100;
        public class SamplesToBePredicted
        {
            [LoadColumn(0, PredictLength - 1)]
            [VectorType(PredictLength)]
            [ColumnName("Label")]
            public double[] Data { get; set; }
        }

        private List<ChartType> GetChartList(List<String> predictedResults)
        {
            List<ChartType> charts = new List<ChartType>();
            for (int i = 0; i < predictedResults.Count; i++)
            {
                if (StringToChartTypes.ContainsKey(predictedResults[i]))
                {
                    ChartType[] type = StringToChartTypes[predictedResults[i]];
                    for (int j = 0; j < type.Length; j++)
                    {
                        if (!charts.Contains(type[j]))
                        {
                            charts.Add(type[j]);
                        }
                    }
                }
            }
            return charts;
        }
        public enum ChartType
        {
            Constellation,
            IDiagram,
            QDiagram,
            Spectrum,
            TimeDomain,
            Histogram,
            EyeDiagram,
            Bath,
            QFactor,
            XY,
        }

        private Dictionary<String, ChartType[]> StringToChartTypes = new Dictionary<String, ChartType[]>() {
            { "PSK",new ChartType[]{ChartType.Constellation, ChartType.IDiagram, ChartType.QDiagram } },
            { "FSK",new ChartType[]{ChartType.Spectrum, ChartType.IDiagram, ChartType.QDiagram } },
            { "QAM",new ChartType[]{ChartType.Constellation } },
        };

        private Dictionary<ChartType, DrawMethod> ChartTypeToDrawMethod = new Dictionary<ChartType, DrawMethod>() {
            { ChartType.Constellation,DrawMethod.XYDots },
            { ChartType.IDiagram,DrawMethod.Plot },
            { ChartType.QDiagram,DrawMethod.Plot },
            { ChartType.Spectrum,DrawMethod.Plot },
            { ChartType.TimeDomain,DrawMethod.Plot },
            { ChartType.Histogram,DrawMethod.Bar },
            { ChartType.EyeDiagram,DrawMethod.DPX },
            { ChartType.Bath,DrawMethod.Plot },
            { ChartType.QFactor,DrawMethod.Plot },
            { ChartType.XY,DrawMethod.XYLines },
        };



        private Dictionary<ChannelId, MathChartModel> _ChartTable = new();
        private static readonly object _Locker = new object();
        public Boolean CloseCharts()
        {
            lock (_Locker)
            {
                foreach (var item in _ChartTable)
                {
                    item.Value.Enabled = false;
                }
                _ChartTable.Clear();
                return true;
            }
        }

        private Boolean OpenCharts(List<ChartType> chartTypes)
        {
            lock (_Locker)
            {
                for (int i = 0; i < chartTypes.Count; i++)
                {
                    if (GetMathItems(out var id))
                    {
                        var chartExist = false;
                        foreach (var item in _ChartTable)
                            if (item.Value.Source == _Source && item.Value.Formula == "IChart" + chartTypes[i].ToString() + "()")
                            {
                                chartExist = true;
                                break;
                            }
                        if (!chartExist)
                            Active((ChannelId)id, true, _Source, chartTypes[i]);
                        //Active((ChannelId)id, true, _Source, ChartTypeToDrawMethod[chartTypes[i]]);
                    }
                }
            }

            return false;
        }

        private Boolean GetMathItems([NotNullWhen(true)] out ChannelId? channelId)
        {
            channelId = null;
            foreach (var channel in ChannelIdExt.GetIChartMaths())
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(channel, out var mathPrsnt))
                {
                    if (mathPrsnt is MathPrsnt && (mathPrsnt as MathPrsnt)!.Args!.Occupier == null)
                    {
                        (mathPrsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                        channelId = channel;
                        return true;
                    }
                }
            }
            return false;
        }

        private void Active(ChannelId id, Boolean state, ChannelId source, ChartType chartType)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var mathPrsnt))
            {
                MathChartModel mathChartModel = new("IChart" + chartType.ToString() + "()", source);
                mathChartModel.MathChannelId = id;
                if (mathPrsnt is MathPrsnt && mathPrsnt as MathPrsnt != null)
                {
                    MathPrsnt prsnt = (mathPrsnt as MathPrsnt)!;
                    (mathPrsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                    mathPrsnt.Active = state;
                    mathChartModel.Enabled = state;
                    _ChartTable.Add(id, mathChartModel);
                }
            }
        }

        public void UpdateWaveData(double[,] data)
        {
            //var IChart = new Vector();
            var constellationVector = GetConstellation(data);
            OccupierBuffer.Default.Provide("IChartConstellation", constellationVector);

            var IDiagramVector = GetIDiagram(data);
            OccupierBuffer.Default.Provide("IChartIDiagram", IDiagramVector);

            var QDiagramVector = GetQDiagram(data);
            OccupierBuffer.Default.Provide("IChartQDiagram", QDiagramVector);

            var spectrumVector = GetSpectrum(data);
            OccupierBuffer.Default.Provide("IChartSpectrum", spectrumVector);

            var timeDomainVector = GetTimeDomain(data);
            OccupierBuffer.Default.Provide("IChartTimeDomain", timeDomainVector);

            var histogramVector = GetHistogram(data);
            OccupierBuffer.Default.Provide("IChartHistogram", histogramVector);

            var eyeDiagramector = GetEyeDiagram(data);
            OccupierBuffer.Default.Provide("IChartEyeDiagram", eyeDiagramector);

            var bathVector = GetBath(data);
            OccupierBuffer.Default.Provide("IChartBath", bathVector);

            var QFactorVector = GetQFactor(data);
            OccupierBuffer.Default.Provide("IChartQFactor", QFactorVector);

            var XYVector = GetXY(data);
            OccupierBuffer.Default.Provide("IChartXY", XYVector);
        }//????

        private Vector GetConstellation(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(QWaveMatrix,
            //  QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //  QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
            //  1,
            //  Constants.DEF_XPOS_IDX);
        }
        private Vector GetIDiagram(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(trenddata,
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    1.0 / jitterData.Fs * jitterData.AverageUILength,
            //    Constants.DEF_XPOS_IDX);
        }
        private Vector GetQDiagram(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(trenddata,
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    1.0 / jitterData.Fs * jitterData.AverageUILength,
            //    Constants.DEF_XPOS_IDX);
        }
        private Vector GetSpectrum(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new MathExt.Vector(TIESpectrum.Select(o => o / jitterData.Fs * Constants.S_RELATIVE_TO_PS).ToMatrix(1, TIESpectrum.Length),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Hertz),
            //    /*"ps"*/QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    (jitterData.Fs / 2 / jitterData.AverageUILength) / TIESpectrum.Length,
            //    /*Constants.DEF_XPOS_IDX*/0);
        }
        private Vector GetTimeDomain(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(trenddata,
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    1.0 / jitterData.Fs * jitterData.AverageUILength,
            //    Constants.DEF_XPOS_IDX);
        }
        private Vector GetHistogram(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(histVector,
            //    /*"ps"*/QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
            //    jitterData.TIEData.Max() / jitterData.Fs,
            //    jitterData.TIEData.Min() / jitterData.Fs);
        }
        private Vector GetEyeDiagram(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(eyeMatrix,
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Voltage),
            //    Data.EyeSampleInterval,
            //    Constants.DEF_XPOS_IDX);
        }
        private Vector GetBath(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(BathWaveMatrix,
            //    QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //    QuantityUnitExt.ToUnitString(QuantityUnit.BER),
            //    1,
            //    Constants.DEF_XPOS_IDX);
        }
        private Vector GetQFactor(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(QWaveMatrix,
            //   QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //   QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
            //   1,
            //   Constants.DEF_XPOS_IDX);
        }
        private Vector GetXY(double[,] data)
        {
            return new Vector(new double[1000],
             QuantityUnitExt.ToUnitString(QuantityUnit.Second),
             QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
             1,
             Constants.DEF_XPOS_IDX);
            //return new Vector(QWaveMatrix,
            //   QuantityUnitExt.ToUnitString(QuantityUnit.Second),
            //   QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
            //   1,
            //   Constants.DEF_XPOS_IDX); 
        }

        public void SwitchMeasureParameters(ChannelId channelId, string MatchTypeStr)
        {
            var measureModel = DsoModel.Default.Meas;
            if (measureModel == null) return;
            // 定义当前信号类型需要的测量参数
            string[] measureItems = MatchTypeStr switch
            {
                "AM" => new[] { "Period", "Freq", "PWidth", "Duty", "Rise", "Fall" },
                "Sine" => new[] { "Period", "Freq", "Amplitude" },
                "Square" => new[] { "PWidth", "NWidth", "Duty", "NDuty", "Amplitude" },
                "tri" => new[] { "Average", "RMS", "Amplitude", "POverShoot", "NOverShoot", "Top", "Base" },
                _ => Array.Empty<string>()
            };

            // 检查当前激活的测量参数是否与需要的一致
            bool needsUpdate = false;
            var activeItems = measureModel.SelectedItems.Where(x => x.Active).ToList();

            if (activeItems.Count != measureItems.Length)
            {
                needsUpdate = true;
            }
            else
            {
                for (int i = 0; i < measureItems.Length; i++)
                {
                    if (activeItems[i].Name != measureItems[i] || activeItems[i].Source != channelId)
                    {
                        needsUpdate = true;
                        break;
                    }
                }
            }

            // 只在需要更新时执行开关操作
            if (needsUpdate)
            {
                foreach (var item in measureModel.SelectedItems)
                {
                    item.Active = false;
                }

                for (int i = 0; i < Math.Min(measureItems.Length, measureModel.SelectedItems.Length); i++)
                {
                    measureModel.SelectedItems[i].Name = measureItems[i];
                    measureModel.SelectedItems[i].Source = channelId;
                    measureModel.SelectedItems[i].Active = true;
                }

                measureModel.Active = measureItems.Length > 0;
            }
        }
    }
}
