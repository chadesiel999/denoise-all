using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Transforms.Onnx;
using Microsoft.ML;
using NPOI.POIFS.Properties;
using Uestc.Auto6.Dso.ComModel;
using Uestc.Auto6.Dso.MathExt;

namespace Uestc.Auto6.Dso.Core
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

        internal List<Double> Run(List<Double> source, NoiseRedutionMethod method)
        {
            Int32 usercnt = UserResetCnt;
            if (usercnt != _ResetCnt)
            {
                _AverageNoiseReduction.Reset();
                _ResetCnt = usercnt;
            }

            if (_NoiseReductionMethodDefine.ContainsKey(method))
            {
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


    internal class NeuralNetworkNoiseReduction : INoiseReductio
    {

        //public List<Double> Run(List<Double> source)
        //{
        //    return source.ToList();
        //}
        private readonly DeNoise _denoiseProcessor;

        public NeuralNetworkNoiseReduction()
        {
            _denoiseProcessor = new DeNoise();
            // 如果需要，你可以在这里配置 _denoiseProcessor，
            // 例如：_denoiseProcessor.ModelPath = "path/to/your/model.onnx";
            // 目前它将使用 DeNoise 中定义的默认路径。
            // 另外，考虑是否需要将 DeNoise.Enable 设置为 true。
            _denoiseProcessor.Enable = true; // 确保去噪逻辑已启用
        }

        public List<Double> Run(List<Double> source)
        {
            if (source == null || source.Count == 0)
            {
                return new List<Double>();
            }
            // 1. 将 List<Double> 转换为 float[]，供 DeNoise 类使用
            float[] floatData = source.Select(d => (float)d).ToArray();

            // 2. 调用 DeNoise 实例的 GetDeNoiseData 方法
            // GetDeNoiseData 方法将处理其内部的归一化/反正归一化和 ONNX 推理。
            List<float> denoisedFloatSignal = _denoiseProcessor.GetDeNoiseData(floatData);
            //_denoiseProcessor.SaveDenoiseResult(denoisedFloatSignal.ToArray());

            // 3. 将结果 List<float> 转换回 List<Double>
            List<Double> denoisedDoubleSignal = denoisedFloatSignal.Select(f => (Double)f).ToList();

            return denoisedDoubleSignal;
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
            for (int i = 0; i < 2000; i++)
            {
                data[i] = 2 * (data[i] - min) / (max - min) - 1;
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

            var res = GetDeNoiseData(data); // 调用新的 GetDeNoiseData，它现在返回 List<float>
        }


        // 已修改为返回 List<float> 而不是 List<DisposableNamedOnnxValue>
        // 并删除了 OccupierBuffer 的调用，使其成为一个用于外部使用的纯函数。
        public List<float> GetDeNoiseData(float[] dataInput)
        {
            if (dataInput == null || dataInput.Length == 0)
            {
                return new List<float>();
            }

            var session = new InferenceSession(_ModelPath);
            List<float> denoisedSignal = new List<float>();

            //var sessionOptions = new SessionOptions();
            //sessionOptions.AppendExecutionProvider_DML(0);

            //var session = new InferenceSession(_ModelPath, sessionOptions);
            //List<float> denoisedSignal = new List<float>();





            // 根据输入数据计算用于归一化的最大值和最小值
            // 这对于后续的反正归一化至关重要。
            float currentMax = dataInput.Max();
            float currentMin = dataInput.Min();

            // 归一化输入数据
            float[] normalizedData = new float[dataInput.Length];
            for (int i = 0; i < dataInput.Length; i++)
            {
                normalizedData[i] = 2 * (dataInput[i] - currentMin) / (currentMax - currentMin) - 1;
            }

            int signalLength = normalizedData.Length;
            const int windowSize = 240;
            int outputCount = 20;
            // 使用滑动窗口处理信号
            for (int i = 0; i <= signalLength - windowSize; i = i + outputCount)
            {
                float[] windowData = new float[windowSize];
                for (int j = 0; j < windowSize; j++)
                {
                    windowData[j] = normalizedData[i + j];
                }

                ONNXParam onnxParm = GetDenseBuffer(windowData, false);
                DenseTensor<float> denseBuffer = onnxParm.DenseTensor;
                NamedOnnxValue named = NamedOnnxValue.CreateFromTensor(onnxParm.InputName, denseBuffer);
                var inputs = new List<NamedOnnxValue> { named };

                using (var outputs = session.Run(inputs)) // 使用 using 确保 DisposableNamedOnnxValue 被正确释放
                {
                    var res = (DenseTensor<float>)outputs.First().Value;
                    int startIndex = windowSize - outputCount;
                    for (int k = 0; k < outputCount; k++)
                    {
                        // res[0, 230, 0] ... res[0, 239, 0]
                        denoisedSignal.Add(res[0, startIndex + k, 0]);
                    }
                }
            }

            // 处理信号长度不是 windowSize 倍数时剩余的部分
            if (signalLength % windowSize != 0)
            {
                int remainingPoints = signalLength % windowSize;
                float[] lastWindow = new float[windowSize];

                // 用剩余的数据填充 lastWindow 的开头
                for (int i = 0; i < remainingPoints; i++)
                {
                    lastWindow[i] = normalizedData[signalLength - remainingPoints + i];
                }

                // 用最后一个有效数据点填充窗口的其余部分，如果没有有效点则用第一个点填充
                float fillValue = remainingPoints > 0 ? lastWindow[remainingPoints - 1] : normalizedData[0];
                for (int i = remainingPoints; i < windowSize; i++)
                {
                    lastWindow[i] = fillValue;
                }

                ONNXParam lastOnnxParm = GetDenseBuffer(lastWindow, false);
                DenseTensor<float> lastDenseBuffer = lastOnnxParm.DenseTensor;
                NamedOnnxValue lastNamed = NamedOnnxValue.CreateFromTensor(lastOnnxParm.InputName, lastDenseBuffer);
                var lastInputs = new List<NamedOnnxValue> { lastNamed };

                using (var lastOutputs = session.Run(lastInputs)) // 使用 using 确保 DisposableNamedOnnxValue 被正确释放
                {
                    var lastRes = (DenseTensor<float>)lastOutputs.First().Value;
                    denoisedSignal.Add(lastRes[0, windowSize - 1, 0]);
                }
            }

            // 将去噪后的信号反归一化回原始尺度
            for (int i = 0; i < denoisedSignal.Count; i++)
            {
                denoisedSignal[i] = (denoisedSignal[i] + 1) * (currentMax - currentMin) / 2 + currentMin;
            }

            // 下面这行关于 OccupierBuffer 的代码是你的 DSO 应用程序内部绘图系统特有的。
            // 它不应该作为通用的 INoiseReduction 实现的一部分。
            // OccupierBuffer.Default.Provide(DsoModel.Default.ArtificialIntelligence.DeNoiseDataGraph.Formula, new Vector(denoisedSignal.Select(o => (Double)(1000000 * o - 1191200)), "S", "V", 1.0, 1.0));

             //SaveDenoiseResult(denoisedSignal.ToArray()); // 可选：用于调试

            
            return denoisedSignal; // 返回实际的去噪信号
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

    #endregion
}
