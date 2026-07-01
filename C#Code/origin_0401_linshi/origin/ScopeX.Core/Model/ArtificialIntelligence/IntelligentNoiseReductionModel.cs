using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Transforms.Onnx;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class IntelligentNoiseReductionModel
    {
        internal IntelligentNoiseReductionModel()
        {
            _NoiseReductionMethodDefine.Add(NoiseRedutionMethod.NeuralNetwork, new NeuralNetworkNoiseReduction());
            _NoiseReductionMethodDefine.Add(NoiseRedutionMethod.Average, _AverageNoiseReduction);
            _NoiseReductionMethodDefine.Add(NoiseRedutionMethod.FreqDomainFilter, new FreqFilterNoiseReduction());
            _NoiseReductionMethodDefine.Add(NoiseRedutionMethod.TimeDomainFilter, new TimeFilterNoiseReduction());
        }

        private readonly Dictionary<NoiseRedutionMethod, INoiseReductio> _NoiseReductionMethodDefine = new();
        private AverageNoiseReduction _AverageNoiseReduction = new();

        internal NoiseRedutionMethod SelectMethod(List<Double> source)
        {
            return NoiseRedutionMethod.Close;
        }

        internal List<Double> Run(List<Double> source, NoiseRedutionMethod method, String modelPath)
        {
            Int32 usercnt = UserResetCnt;
            if (usercnt != _ResetCnt)
            {
                _AverageNoiseReduction.Reset();
                _ResetCnt = usercnt;
            }

            if (_NoiseReductionMethodDefine.ContainsKey(method))
            {
                if (method == NoiseRedutionMethod.NeuralNetwork)
                {
                    NeuralNetworkNoiseReduction neuralNetworkNoiseReduction = (NeuralNetworkNoiseReduction)_NoiseReductionMethodDefine[method];
                    neuralNetworkNoiseReduction.LoadModel(modelPath);
                }
                return _NoiseReductionMethodDefine[method].Run(source);
            }

            return source.ToList();
        }

        internal Int32 MaxAverageCount
        {
            get => _AverageNoiseReduction.MaxAvgCnt;
            set => _AverageNoiseReduction.MaxAvgCnt = value;
        }

        internal Int32 UserResetCnt = 0;
        private Int32 _ResetCnt = 0;
    }

    internal static class JudgePeriod
    {
        internal static Boolean CheckSignalIsPeriod(List<Double> source, Double ratio)
        {
            if (source.Count == 0)
            {
                return false;
            }

            Double max = source.Max();
            Double min = source.Min();
            Double vpp = max - min;
            Double hlevel = max - ratio * vpp;
            Double llevel = min + ratio * vpp;

            Byte[] shapedata = Shape(source, hlevel, llevel);

            var lowcnttable = GetCntTable(shapedata, _ValueL);

            var highcnttable = GetCntTable(shapedata, _ValueH);

            return true;
        }

        private const Byte _ValueH = 1;
        private const Byte _ValueL = 0;
        private const Byte _ValueDefault = 0xff;

        /// <summary>
        /// 整形：迟滞比较，前期没有超过迟滞范围时赋值为默认值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="levelH"></param>
        /// <param name="levelL"></param>
        /// <returns></returns>
        internal static Byte[] Shape(List<Double> source, Double levelH, Double levelL, Byte defaultvalue = _ValueDefault)
        {
            Int32 sourcelen = source.Count;
            Byte[] shapedata = new Byte[sourcelen];

            if (sourcelen == 0)
                return shapedata;

            Boolean? preishigh = null;

            for (Int32 i = 0; i < sourcelen; i++)
            {
                if (source[i] <= levelL)
                {
                    preishigh = false;
                    shapedata[i] = _ValueL;
                    continue;
                }

                if (source[i] >= levelH)
                {
                    preishigh = true;
                    shapedata[i] = _ValueH;
                    continue;
                }

                if (preishigh == null)
                {
                    shapedata[i] = defaultvalue;
                    continue;
                }

                shapedata[i] = ((Boolean)preishigh) ? _ValueH : _ValueL;
            }

            return shapedata;
        }

        internal static List<Int32> GetCntTable(Byte[] source, Byte peakValue)
        {
            Int32 datalen = source.Length;
            List<Int32> cnttable = new();

            Int32 curcnt = 0;
            for (Int32 i = 0; i < datalen; i++)
            {
                if (source[i] == peakValue)
                {
                    curcnt++;
                }
                else
                {
                    if (curcnt != 0)
                        cnttable.Add(curcnt);
                    curcnt = 0;
                }
            }
            if (curcnt != 0)
                cnttable.Add(curcnt);

            return cnttable;
        }

        internal static Boolean CheckUniform(List<Int32> source, Double thresholdRatio)
        {
            if (source.Count < 2)
            {
                return false;
            }
            Double avg = source.Average();
            Int32 highlimit = (Int32)Math.Ceiling(avg + thresholdRatio * avg);
            Int32 lowlimit = (Int32)Math.Floor(avg - thresholdRatio * avg);

            for (Int32 i = 0; i < source.Count; i++)
            {
                if (source[i] > highlimit || source[i] < lowlimit)
                    return false;
            }
            return true;
        }
    }

    #region 去噪方法
    public interface INoiseReductio
    {
        public List<Double> Run(List<Double> source);
    }

    /// <summary>
    /// 神经网络降噪类 - 使用ONNX模型进行信号降噪
    /// </summary>
    internal class NeuralNetworkNoiseReduction : INoiseReductio
    {
        // ================= 通信管理器 =================
        private NeuralNetworkCommunicator _communicator;

        // ================= ONNX 推理部分 =================
        private InferenceSession? _onnxSession;
        private object _modelLock = new object();
        private bool _isModelReady = false;
        private string _modelPath = "";

        // ================= 性能记录部分（新增） =================
        private Int32 _InferenceCallCount = 0;
        private readonly HashSet<Int32> _RecordCheckpoints = new HashSet<Int32> { 50, 100, 150, 200, 250, 300 };

        public NeuralNetworkNoiseReduction()
        {
            _communicator = new NeuralNetworkCommunicator();

            // 注册模型接收完成事件
            _communicator.OnModelReceived += (modelPath) =>
            {
                Console.WriteLine($"通信器收到新模型，准备加载: {modelPath}");
                LoadModel(modelPath);
            };

            // 尝试初始化连接（不阻塞）
            Task.Run(() =>
            {
                try
                {
                    _communicator.InitializeConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"初始化连接失败: {ex.Message}");
                }
            });

            // 尝试加载一个默认的本地模型(如果有)
            //string defaultModel = "AI/TensorModels/broadband_dataset_7G9G5ns.onnx";
            //if (File.Exists(defaultModel))
            //{
            //    LoadModel(defaultModel);
            //}
        }

        /// <summary>
        /// 加载ONNX模型
        /// </summary>
        public void LoadModel(string modelPath)
        {
            lock (_modelLock)
            {
                try
                {
                    if (modelPath == _modelPath)
                        return;
                    else
                        ResetModel();
                    if (_onnxSession != null)
                    {
                        _onnxSession.Dispose();
                    }

                    var options = new SessionOptions();
                    // options.AppendExecutionProvider_DML(0); // 如果支持显卡加速可开启

                    _onnxSession = new InferenceSession(modelPath, options);
                    _isModelReady = true;
                    _modelPath = modelPath;
                    Console.WriteLine($"模型已加载: {modelPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载模型失败: {ex.Message}");
                    _isModelReady = false;
                }
            }
        }

        // max和min现在会在GetDeNoiseData方法内部根据传入数据计算
        private float max = 0;
        private float min = 0;

        /// <summary>
        /// Double数据处理 - 用于实时推理降噪
        /// </summary>
        public List<Double> Run(List<Double> source)
        {
            //if (_modelPath == ArtificialIntelligenceModel.DefaultModelPath)
            //{
            // 将 List<Double> 转换为 double[] 并调用 GetDeNoiseData
            return GetDeNoiseData(source.ToArray());
            //}
            //// 如果模型未就绪，直接返回原数据
            //if (!_isModelReady || _onnxSession == null || source.Count == 0)
            //{
            //    return source;
            //}

            //try
            //{
            //    // 对于显示数据，直接进行推理
            //    return RunInferenceDouble(source);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"推理异常: {ex.Message}");
            //    return source; // 出错时返回原数据
            //}
        }

        public List<Double> GetDeNoiseData(double[] dataInput)
        {
            if (dataInput == null || dataInput.Length == 0)
            {
                return new List<Double>();
            }

            // ===== 新增：本次整条波形推理调用计数 +1 =====
            Int32 currentInferenceIndex = Interlocked.Increment(ref _InferenceCallCount);

            // ===== 新增：总耗时计时 =====
            var totalWatch = System.Diagnostics.Stopwatch.StartNew();

            // ===== 新增：纯推理时间累计 =====
            Double pureInferenceTimeMs = 0.0;

            // ===== 新增：记录资源占用（前） =====
            var process = System.Diagnostics.Process.GetCurrentProcess();
            process.Refresh();
            Double workingSetBeforeMB = process.WorkingSet64 / 1024.0 / 1024.0;
            Double privateMemoryBeforeMB = process.PrivateMemorySize64 / 1024.0 / 1024.0;

            List<float> denoisedSignal = new List<float>();

            // 根据输入数据计算用于归一化的最大值和最小值
            // 这对于后续的反正归一化至关重要。
            float currentMax = (float)dataInput.Max();
            float currentMin = (float)dataInput.Min();

            // 归一化输入数据
            float[] normalizedData = new float[dataInput.Length];
            for (int i = 0; i < dataInput.Length; i++)
            {
                normalizedData[i] = 2 * ((float)dataInput[i] - currentMin) / (currentMax - currentMin) - 1;
            }

            int signalLength = normalizedData.Length;
            const int windowSize = 240;
            const int outputCount = 100;
            int pad = (windowSize - outputCount) / 2; // 70

            // 使用滑动窗口处理信号
            // 改为：每次滑窗取中间的 outputCount (100) 个点
            // 并自动对两端进行 Padding (复制边缘值)，确保覆盖整个信号且不丢数据
            for (int i = 0; i < signalLength; i += outputCount)
            {
                float[] windowData = new float[windowSize];

                // 计算当前窗口对应的起始数据索引
                // 我们希望 output 的第 k 个点对应 normalizedData[i + k]
                // 而 output 的第 k 个点来源于 window 的 pad + k 位置
                // 所以 window 的 0 位置应该对应 normalizedData[i - pad]
                int windowStartIndex = i - pad;

                for (int j = 0; j < windowSize; j++)
                {
                    int realIndex = windowStartIndex + j;
                    if (realIndex < 0)
                        windowData[j] = normalizedData[0];
                    else if (realIndex >= signalLength)
                        windowData[j] = normalizedData[signalLength - 1];
                    else
                        windowData[j] = normalizedData[realIndex];
                }

                ONNXParam onnxParm = GetDenseBuffer(windowData, false);
                DenseTensor<float> denseBuffer = onnxParm.DenseTensor;
                NamedOnnxValue named = NamedOnnxValue.CreateFromTensor(onnxParm.InputName, denseBuffer);
                var inputs = new List<NamedOnnxValue> { named };

                // ===== 新增：单窗纯推理计时 =====
                var inferenceWatch = System.Diagnostics.Stopwatch.StartNew();
                using (var outputs = _onnxSession.Run(inputs)) // 使用 using 确保 DisposableNamedOnnxValue 被正确释放
                {
                    inferenceWatch.Stop();
                    pureInferenceTimeMs += inferenceWatch.Elapsed.TotalMilliseconds;

                    var res = (DenseTensor<float>)outputs.First().Value;

                    // 取窗口中间的数据
                    int pointsToTake = Math.Min(outputCount, signalLength - i);

                    for (int k = 0; k < pointsToTake; k++)
                    {
                        denoisedSignal.Add(res[0, pad + k, 0]);
                    }
                }
            }

            // 将去噪后的信号反归一化回原始尺度，并转换为 Double 类型
            List<Double> result = new List<Double>(denoisedSignal.Count);
            for (int i = 0; i < denoisedSignal.Count; i++)
            {
                float denoisedValue = (denoisedSignal[i] + 1) * (currentMax - currentMin) / 2 + currentMin;
                result.Add((Double)denoisedValue);
            }

            // ===== 新增：总耗时结束 =====
            totalWatch.Stop();

            // ===== 新增：记录资源占用（后） =====
            process.Refresh();
            Double workingSetAfterMB = process.WorkingSet64 / 1024.0 / 1024.0;
            Double privateMemoryAfterMB = process.PrivateMemorySize64 / 1024.0 / 1024.0;

            // ===== 新增：只在第50/100/150/200/250/300次推理时保存 =====
            if (_RecordCheckpoints.Contains(currentInferenceIndex))
            {
                SavePerformanceToCsv(
                    currentInferenceIndex,
                    dataInput.Length,
                    totalWatch.Elapsed.TotalMilliseconds,
                    pureInferenceTimeMs,
                    workingSetBeforeMB,
                    workingSetAfterMB,
                    privateMemoryBeforeMB,
                    privateMemoryAfterMB
                );

                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理总耗时：{totalWatch.Elapsed.TotalMilliseconds:F4} ms");
                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理纯推理耗时：{pureInferenceTimeMs:F4} ms");
                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理工作集内存前：{workingSetBeforeMB:F4} MB");
                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理工作集内存后：{workingSetAfterMB:F4} MB");
                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理私有内存前：{privateMemoryBeforeMB:F4} MB");
                System.Diagnostics.Debug.WriteLine($"第{currentInferenceIndex}次推理私有内存后：{privateMemoryAfterMB:F4} MB");
                System.Diagnostics.Debug.WriteLine("--------------------------------------------------");
            }

            // 下面这行关于 OccupierBuffer 的代码是你的 DSO 应用程序内部绘图系统特有的。
            // 它不应该作为通用的 INoiseReduction 实现的一部分。
            // OccupierBuffer.Default.Provide(DsoModel.Default.ArtificialIntelligence.DeNoiseDataGraph.Formula, new Vector(denoisedSignal.Select(o => (Double)(1000000 * o - 1191200)), "S", "V", 1.0, 1.0));

            //SaveDenoiseResult(denoisedSignal.ToArray()); // 可选：用于调试

            return result; // 返回实际的去噪信号
        }

        private ONNXParam GetDenseBuffer(float[] data, Boolean testMode = true)
        {
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 240; // 假设模型的 windowSize 始终为 30
            onnxParam.InputCount = 1;

            // 创建输入张量 [1, 30, 1]
            DenseTensor<float> denseBuffer = new(new[] { 1, 240, 1 });
            for (int i = 0; i < data.Length; i++)
            {
                denseBuffer[0, i, 0] = data[i];
            }
            onnxParam.DenseTensor = denseBuffer;
            return onnxParam;
        }

        // ===== 新增：保存性能统计到CSV =====
        private void SavePerformanceToCsv(
            Int32 inferenceCallIndex,
            Int32 pointCount,
            Double totalMs,
            Double pureInferenceMs,
            Double workingSetBeforeMB,
            Double workingSetAfterMB,
            Double privateMemoryBeforeMB,
            Double privateMemoryAfterMB)
        {
            try
            {
                string directory = Directory.GetCurrentDirectory() + "\\PerformanceLog\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = Path.Combine(directory, "inference_stats.csv");

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(
                        filePath,
                        "Timestamp,InferenceCallIndex,DataPoints,TotalTime(ms),PureInference(ms),WorkingSetBefore(MB),WorkingSetAfter(MB),PrivateMemoryBefore(MB),PrivateMemoryAfter(MB)\n"
                    );
                }

                string logLine =
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}," +
                    $"{inferenceCallIndex}," +
                    $"{pointCount}," +
                    $"{totalMs:F4}," +
                    $"{pureInferenceMs:F4}," +
                    $"{workingSetBeforeMB:F4}," +
                    $"{workingSetAfterMB:F4}," +
                    $"{privateMemoryBeforeMB:F4}," +
                    $"{privateMemoryAfterMB:F4}\n";

                File.AppendAllText(filePath, logLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存性能统计失败: {ex.Message}");
            }
        }

        public void SaveDenoiseResult(float[] denoisedSignal)
        {
            try
            {
                string directory = Directory.GetCurrentDirectory() + "\\DataExport\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string fileName = $"去噪结果_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string fullPath = Path.Combine(directory, fileName);

                using (StreamWriter sw = new StreamWriter(fullPath, false))
                {
                    foreach (float value in denoisedSignal)
                    {
                        sw.WriteLine(value.ToString("F6"));
                    }
                }

                Console.WriteLine($"去噪结果已保存到: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存去噪结果出错: {ex.Message}");
            }
        }

        /// <summary>
        /// Double类型数据的ONNX推理
        /// </summary>
        private List<Double> RunInferenceDouble(List<Double> inputData)
        {
            // 1. 预处理：Double -> float
            float[] dataFloat = new float[inputData.Count];
            for (int i = 0; i < inputData.Count; i++)
            {
                dataFloat[i] = (float)inputData[i];
            }

            // 2. 归一化 (Normalization)
            // 计算当前帧的最大最小值，将数据映射到 [-1, 1] 区间
            float currentMax = dataFloat.Max();
            float currentMin = dataFloat.Min();

            // 避免除以0
            if (Math.Abs(currentMax - currentMin) < 1e-6)
                return inputData;

            float[] normalizedData = new float[dataFloat.Length];
            for (int i = 0; i < dataFloat.Length; i++)
            {
                normalizedData[i] = 2 * (dataFloat[i] - currentMin) / (currentMax - currentMin) - 1;
            }

            // 3. ONNX 推理
            List<float> denoisedFloat = new List<float>(inputData.Count);
            const int windowSize = 240;
            int signalLength = normalizedData.Length;
            int outputCount = 1;

            lock (_modelLock) // 锁住模型防止推理中途被替换
            {
                if (_onnxSession == null) return inputData;

                // 滑动窗口推理
                for (int i = 0; i <= signalLength - windowSize; i += outputCount)
                {
                    var inputTensor = new DenseTensor<float>(new[] { 1, windowSize, 1 });
                    for (int j = 0; j < windowSize; j++)
                    {
                        inputTensor[0, j, 0] = normalizedData[i + j];
                    }

                    var inputs = new List<NamedOnnxValue>
                    {
                        NamedOnnxValue.CreateFromTensor("input", inputTensor)
                    };

                    using (var outputs = _onnxSession.Run(inputs))
                    {
                        var outputTensor = outputs.First().Value as DenseTensor<float>;
                        // 取窗口末尾的点作为去噪结果
                        denoisedFloat.Add(outputTensor[0, windowSize - 1, 0]);
                    }
                }

                // 简单的 Padding 处理剩余点 (不够一个窗口的数据)
                int remaining = signalLength - denoisedFloat.Count;
                for (int k = 0; k < remaining; k++)
                {
                    denoisedFloat.Add(normalizedData[signalLength - remaining + k]);
                }
            }

            // 4. 后处理：反归一化 (float -> Double)
            // 将 [-1, 1] 映射回 [Min, Max]
            List<Double> result = new List<Double>(denoisedFloat.Count);
            for (int i = 0; i < denoisedFloat.Count; i++)
            {
                double val = (denoisedFloat[i] + 1) * (currentMax - currentMin) / 2 + currentMin;
                result.Add(val);
            }

            return result;
        }

        /// <summary>
        /// 手动触发模型重置
        /// </summary>
        public void ResetModel()
        {
            lock (_modelLock)
            {
                _onnxSession?.Dispose();
                _onnxSession = null;
                _isModelReady = false;
                Console.WriteLine("模型已重置");
            }
        }
    }
    #endregion
    #region 其他降噪方法

    /// <summary>
    /// 平均降噪类
    /// </summary>
    /*已废弃的通信代码已删除 - 现在使用 NeuralNetworkCommunicator 类处理*/
    internal class AverageNoiseReduction : INoiseReductio
    {
        internal AverageNoiseReduction()
        {

        }

        public void Reset()
        {
            _AvgData.Clear();
            _OriginDataQueue.Clear();
        }

        internal Int32 MaxAvgCnt = 100;
        private Queue<List<Double>> _OriginDataQueue = new();
        private List<Double> _AvgData = new();

        public List<Double> Run(List<Double> source)
        {
            if (_AvgData.Count != source.Count)
            {
                _AvgData.Clear();
                _OriginDataQueue.Clear();
                _AvgData.AddRange(source);
                _OriginDataQueue.Enqueue(source.ToList());
                return _AvgData.ToList();
            }

            Int32 queuecnt = _OriginDataQueue.Count;
            if (queuecnt < MaxAvgCnt)
            {
                Int32 avgcnt = queuecnt + 1;
                for (Int32 i = 0; i < _AvgData.Count; i++)
                {
                    Double tmp = _AvgData[i] * queuecnt + source[i];
                    _AvgData[i] = tmp / avgcnt;
                }
                _OriginDataQueue.Enqueue(source.ToList());
                return _AvgData.ToList();
            }

            List<Double> topdata = _OriginDataQueue.Dequeue();
            for (Int32 i = 0; i < _AvgData.Count; i++)
            {
                Double tmp = _AvgData[i] * queuecnt - topdata[i] + source[i];
                _AvgData[i] = tmp / queuecnt;
            }
            _OriginDataQueue.Enqueue(source.ToList());

            return _AvgData.ToList();
        }
    }

    internal class FreqFilterNoiseReduction : INoiseReductio
    {
        public List<Double> Run(List<Double> source)
        {
            return source.ToList();
        }
    }

    internal class TimeFilterNoiseReduction : INoiseReductio
    {
        public List<Double> Run(List<Double> source)
        {
            return source.ToList();
        }
    }

    internal class DeNoise
    {
        public Boolean Enable
        {
            get;
            set;
        } = false;
        private MLContext _MLContext = new MLContext();
        private OnnxScoringEstimator? _Estimator;

        // max和min现在会在GetDeNoiseData方法内部根据传入数据计算
        private float max = 0;
        private float min = 0;

        private String _ModelPath = @"AI/TensorModels/broadband_dataset_7G9G5ns.onnx";
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

        // 注意：原始的 Run() 方法似乎是用于内部测试/演示的。
        // 对于 INoiseReduction 接口，我们将主要使用 GetDeNoiseData 方法。
        public void Run()
        {
            var DataPath = Directory.GetCurrentDirectory() + "\\denoisedata\\";
            var data_ = ReadDataFromFile(DataPath + "C1_Sub1_400MHz.txt");
            var clean = ReadDataFromFile(DataPath + "C1_Sub2_400MHz.txt");
            float[] data = new float[2000];  // 创建正确大小的数组
            float[] data2 = new float[2000];
            for (int i = 0; i < 2000; i++)
            {
                data[i] = (float)(data_[i][0] / 20.48 - 100);
                data2[i] = (float)(clean[i][0] / 20.48 - 100);
            }
            max = data.Max();
            min = data.Min();
            // SaveDataToTxt(data2, "cleanData.txt");
            // 将 float[] 转换为 double[]
            double[] dataDouble = new double[2000];
            for (int i = 0; i < 2000; i++)
            {
                dataDouble[i] = 2 * (data[i] - min) / (max - min) - 1;
            }
            //ChannelId child = DsoModel.Default.ArtificialIntelligence.SignalRecognizeChnlId1;
            //if (DsoModel.Default.TryGetChannel(child, out var Ich))
            //{
            //    if (Ich.VuDatabase.Current != null)
            //    {
            //        Double[,] buffer = Ich.VuDatabase.Current.Buffer;
            //        int rows = buffer.GetLength(1);
            //        if (rows > 2000)
            //        {
            //            rows = 2000;
            //        }
            //        //float [] secondvumdata = new float[rows];
            //        for (int i = 0; i < rows; i++)
            //        {
            //            data[i] = (float)buffer[0, i];
            //        }
            //    }
            //}

            var res = GetDeNoiseData(dataDouble); // 调用新的 GetDeNoiseData，它现在返回 List<Double>
        }


        // 已修改为返回 List<Double> 而不是 List<DisposableNamedOnnxValue>
        // 并删除了 OccupierBuffer 的调用，使其成为一个用于外部使用的纯函数。
        public List<Double> GetDeNoiseData(double[] dataInput)
        {
            if (dataInput == null || dataInput.Length == 0)
            {
                return new List<Double>();
            }

            var session = new InferenceSession(_ModelPath);
            List<float> denoisedSignal = new List<float>();

            //var sessionOptions = new SessionOptions();
            //sessionOptions.AppendExecutionProvider_DML(0);

            //var session = new InferenceSession(_ModelPath, sessionOptions);
            //List<float> denoisedSignal = new List<float>();





            // 根据输入数据计算用于归一化的最大值和最小值
            // 这对于后续的反正归一化至关重要。
            float currentMax = (float)dataInput.Max();
            float currentMin = (float)dataInput.Min();

            // 归一化输入数据
            float[] normalizedData = new float[dataInput.Length];
            for (int i = 0; i < dataInput.Length; i++)
            {
                normalizedData[i] = 2 * ((float)dataInput[i] - currentMin) / (currentMax - currentMin) - 1;
            }

            int signalLength = normalizedData.Length;
            const int windowSize = 240;
            const int outputCount = 100;
            int pad = (windowSize - outputCount) / 2; // 70

            // 使用滑动窗口处理信号
            // 改为：每次滑窗取中间的 outputCount (100) 个点
            // 并自动对两端进行 Padding (复制边缘值)，确保覆盖整个信号且不丢数据
            for (int i = 0; i < signalLength; i += outputCount)
            {
                float[] windowData = new float[windowSize];

                // 计算当前窗口对应的起始数据索引
                // 我们希望 output 的第 k 个点对应 normalizedData[i + k]
                // 而 output 的第 k 个点来源于 window 的 pad + k 位置
                // 所以 window 的 0 位置应该对应 normalizedData[i - pad]
                int windowStartIndex = i - pad;

                for (int j = 0; j < windowSize; j++)
                {
                    int realIndex = windowStartIndex + j;
                    if (realIndex < 0)
                        windowData[j] = normalizedData[0];
                    else if (realIndex >= signalLength)
                        windowData[j] = normalizedData[signalLength - 1];
                    else
                        windowData[j] = normalizedData[realIndex];
                }

                ONNXParam onnxParm = GetDenseBuffer(windowData, false);
                DenseTensor<float> denseBuffer = onnxParm.DenseTensor;
                NamedOnnxValue named = NamedOnnxValue.CreateFromTensor(onnxParm.InputName, denseBuffer);
                var inputs = new List<NamedOnnxValue> { named };

                using (var outputs = session.Run(inputs)) // 使用 using 确保 DisposableNamedOnnxValue 被正确释放
                {
                    var res = (DenseTensor<float>)outputs.First().Value;

                    // 取窗口中间的数据
                    int pointsToTake = Math.Min(outputCount, signalLength - i);

                    for (int k = 0; k < pointsToTake; k++)
                    {
                        denoisedSignal.Add(res[0, pad + k, 0]);
                    }
                }
            }

            // 将去噪后的信号反归一化回原始尺度，并转换为 Double 类型
            List<Double> result = new List<Double>(denoisedSignal.Count);
            for (int i = 0; i < denoisedSignal.Count; i++)
            {
                float denoisedValue = (denoisedSignal[i] + 1) * (currentMax - currentMin) / 2 + currentMin;
                result.Add((Double)denoisedValue);
            }

            // 下面这行关于 OccupierBuffer 的代码是你的 DSO 应用程序内部绘图系统特有的。
            // 它不应该作为通用的 INoiseReduction 实现的一部分。
            // OccupierBuffer.Default.Provide(DsoModel.Default.ArtificialIntelligence.DeNoiseDataGraph.Formula, new Vector(denoisedSignal.Select(o => (Double)(1000000 * o - 1191200)), "S", "V", 1.0, 1.0));

            //SaveDenoiseResult(denoisedSignal.ToArray()); // 可选：用于调试

            return result; // 返回实际的去噪信号
        }


        private ONNXParam GetDenseBuffer(float[] data, Boolean testMode = true)
        {
            ONNXParam onnxParam = new ONNXParam();
            onnxParam.InputName = "input";
            onnxParam.DataLength = 240; // 假设模型的 windowSize 始终为 30
            onnxParam.InputCount = 1;

            // 创建输入张量 [1, 30, 1]
            DenseTensor<float> denseBuffer = new(new[] { 1, 240, 1 });
            for (int i = 0; i < data.Length; i++)
            {
                denseBuffer[0, i, 0] = data[i];
            }
            onnxParam.DenseTensor = denseBuffer;
            return onnxParam;
        }

        // 添加数据导出方法
        private void SaveDataToTxt(float[] data, string fileName)
        {
            try
            {
                string directory = Directory.GetCurrentDirectory() + "\\DataExport\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string fullPath = Path.Combine(directory, fileName);
                using (StreamWriter sw = new StreamWriter(fullPath, false))
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        sw.WriteLine(data[i].ToString("F6"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存数据出错: {ex.Message}");
            }
        }
        public void SaveDenoiseResult(float[] denoisedSignal)
        {
            try
            {
                string directory = Directory.GetCurrentDirectory() + "\\DataExport\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string fileName = $"去噪结果_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string fullPath = Path.Combine(directory, fileName);

                using (StreamWriter sw = new StreamWriter(fullPath, false))
                {
                    foreach (float value in denoisedSignal)
                    {
                        sw.WriteLine(value.ToString("F6"));
                    }
                }

                Console.WriteLine($"去噪结果已保存到: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存去噪结果出错: {ex.Message}");
            }
        }

        #region 接口实现
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    // 定义统计量计算类
    public static class StatsCalculator
    {
        // 计算均值
        public static float Mean(float[] data)
        {
            double sum = 0.0;
            foreach (float value in data)
            {
                sum += value;
            }
            return (float)(sum / data.Length);
        }

        // 计算标准差（总体标准差，与 NumPy 默认行为一致）
        public static float Std(float[] data)
        {
            float mean = Mean(data);
            double sumSquares = 0.0;
            foreach (float value in data)
            {
                sumSquares += Math.Pow(value - mean, 2);
            }
            return (float)Math.Sqrt(sumSquares / data.Length);
        }
    }
    #endregion
}