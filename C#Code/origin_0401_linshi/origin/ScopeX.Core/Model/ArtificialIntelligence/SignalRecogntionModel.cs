using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace ScopeX.Core
{
    internal class SignalRecogntionModel
    {
        private SignalRecogntionModel()
        { 
            
        }

        internal static SignalRecogntionModel Default = new();

        private Dictionary<ChannelId, Int32> _ChannelId = new();

        internal Dictionary<ChannelId, String> ResultOut = new();
        private Dictionary<ChannelId, Int32> _UpdateBuildCnt = new();
        private int MAX_INPUT = 100;

        
        internal void RunPSignalRecognition(ChannelId chnlid, Int32 needCalcCnt)
        {
            //if (_UpdateBuildCntTable.ContainsKey(chnlid)== false) _UpdateBuildCntTable[chnlid] = 0;
            //Int32 buildUpdatecnttable = _UpdateBuildCntTable[chnlid];
            //if (_UpdateBuildCnt.ContainsKey(chnlid)== false) _UpdateBuildCnt[chnlid] = 0;
            //Int32 buildUpdatecnt = _UpdateBuildCnt[chnlid];
            int buildUpdatecnt = _UpdateBuildCnt.ContainsKey(chnlid) ? _UpdateBuildCnt[chnlid] : 0;

            if (!_ChannelId.ContainsKey(chnlid)) _ChannelId[chnlid] = 0;
            if (_ChannelId[chnlid] == 0 || needCalcCnt != buildUpdatecnt)
            {
                double[] bufferdoubles = new double[MAX_INPUT];
                //String filePath = "python/data.txt";
                String filePath = "C:\\Users\\Admin\\AppData\\Local\\Programs\\Python\\Python38\\Lib\\data.txt";
                if (DsoModel.Default.TryGetChannel(chnlid, out var ach))
                {
                    if (ach.VuDatabase.Current != null)
                    {
                        Double[,] buffer = ach.VuDatabase.Current.Buffer;
                        String result;
                        if (buffer.GetLength(1) * buffer.GetLength(0) > MAX_INPUT)
                        {
                            var buffer123= buffer.Cast<Double>().ToArray().Take(MAX_INPUT);
                            //bufferdoubles = buffer123.Select(o=>(o-256)*12).ToArray();
                            bufferdoubles = buffer123.ToArray();
                            result = String.Join(",", bufferdoubles);
                        }
                        else
                        {
                            bufferdoubles = buffer.Cast<Double>().ToArray();
                            result = String.Join(",", bufferdoubles);
                        }
                        File.WriteAllText(filePath, result);
                    }
                }
                //ResultOut[chnlid] = RunPythonScript("-u", filePath);
                Dictionary<int, string> signalTypeMapping = new Dictionary<int, string>
                {
                    { 0, "AM" },
                    { 1, "chirp" },
                    { 2, "sin" },
                    { 3, "triangle" },
                    { 4, "square" }
                };
                //Int32 index = RunPythondef(bufferdoubles);
                // Example usage: retrieving the string associated with a number
                
                var index = RunScript("-u", filePath);
                int number = int.Parse(index);
                if (signalTypeMapping.TryGetValue(number, out string value))
                {
                    ResultOut[chnlid] = value;
                }
                else
                {
                    ResultOut[chnlid] = "Data is wrong";
                }

                _ChannelId[chnlid] = 1;
                _UpdateBuildCnt[chnlid] = needCalcCnt;
            }
        }
        internal String RunPythonScript(String args = "", params String[] teps)
        {
            Process p = new Process();

            String path = "python/waveformrecognition_plus_plus_plus(1).py";
            p.StartInfo.FileName = @"D:\VoiceRecognition\python\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            String sArguments = path;
            foreach (String sigstr in teps)
            {
                sArguments += " " + sigstr;//传递参数
            }

            sArguments += " " + args;

            p.StartInfo.Arguments = sArguments;

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            String output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();//关键，等待外部程序退出后才能往下执行}
            p.Close();
            return output;

        }

        //internal Int32 RunPythondef(double[] dataI)
        //{
        //    string pathToVirtualEnv = "C:\\Users\\Asus\\anaconda3\\envs\\deeplearning";    //python环境路径
        //    Runtime.PythonDLL = Path.Combine(pathToVirtualEnv, "python39.dll");
        //    PythonEngine.PythonHome = Path.Combine(pathToVirtualEnv, "python.exe");
        //    PythonEngine.PythonPath = $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib;{pathToVirtualEnv}\\DLLs;";
        //    PythonEngine.Initialize();


        //    using (Py.GIL())
        //    {

        //        //dynamic np = Py.Import("numpy");
        //        //PyObject dataListI = new PyList(dataI.Select(d => (PyObject)new PyFloat(d)).ToArray());
        //        ////PyObject dataListQ = new PyList(dataQ.Select(d => (PyObject)new PyFloat(d)).ToArray());
        //        //// PyObject dataListI = (PyObject)dataI.Select(d => (PyObject)new PyFloat(d)).ToArray();
        //        //// PyObject dataListQ = (PyObject)dataQ.Select(d => (PyObject)new PyFloat(d)).ToArray();

        //        //var data2Dimension = np.expand_dims(dataListI, -1);
        //        //var data3Dimension = np.expand_dims(data2Dimension, 0);
        //        //dynamic np1 = Py.Import("load_model");
        //        //var predicted_index = np1.predicted(data3Dimension);
        //        //return predicted_index;
        //        dynamic np = Py.Import("numpy");
        //        PyObject dataListI = new PyList(dataI.Select(d => (PyObject)new PyFloat(d)).ToArray());
        //        var data2Dimension = np.expand_dims(dataListI, 0);
        //        var data3Dimension = np.expand_dims(data2Dimension, -1);
        //        dynamic np1 = Py.Import("load_model");
        //        var predicted_index = np1.predicted(data3Dimension);
        //        Int32 result = (int)predicted_index.AsManagedObject(typeof(int));
        //        return result;
        //    }
        //}
        public static string RunScript(string args = "", params string[] teps)
        {
            Process p = new Process();
            //string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + sArgName;// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
            //string path = @"C:\Users\Asus\PycharmProjects\pythonProject\main.py";//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
            string path = @"C:\Users\Admin\AppData\Local\Programs\Python\Python38\Lib\load_model.py";
            p.StartInfo.FileName = @"C:\Users\Admin\AppData\Local\Programs\Python\Python38\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            string sArguments = path;
            foreach (string sigstr in teps)
            {
                sArguments += " " + sigstr;//传递参数
            }
            sArguments += " " + args;

            p.StartInfo.Arguments = sArguments;

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            string[] lines = output.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string lastLine = lines[lines.Length - 2];
            p.Close();
            return lastLine;

        }
        //private Dictionary<ChannelId, Int32> _ChannelId = new();

        //internal Dictionary<ChannelId, String> ResultOut = new();
        //private Dictionary<ChannelId, Int32> _UpdateBuildCnt = new();
        //private int MAX_INPUT = 2500;
        //internal void RunPSignalRecognition(ChannelId chnlid, Int32 needCalcCnt)
        //{
        //    //if (_UpdateBuildCntTable.ContainsKey(chnlid)== false) _UpdateBuildCntTable[chnlid] = 0;
        //    //Int32 buildUpdatecnttable = _UpdateBuildCntTable[chnlid];
        //    //if (_UpdateBuildCnt.ContainsKey(chnlid)== false) _UpdateBuildCnt[chnlid] = 0;
        //    //Int32 buildUpdatecnt = _UpdateBuildCnt[chnlid];
        //    int buildUpdatecnt = _UpdateBuildCnt.ContainsKey(chnlid) ? _UpdateBuildCnt[chnlid] : 0;

        //    if (!_ChannelId.ContainsKey(chnlid)) _ChannelId[chnlid] = 0;
        //    if (_ChannelId[chnlid] == 0 || needCalcCnt != buildUpdatecnt)
        //    {

        //        String filePath = "python/data.txt";
        //        if (DsoModel.Default.TryGetChannel(chnlid, out var ach))
        //        {
        //            if (ach.VuDatabase.Current != null)
        //            {
        //                Double[,] buffer = ach.VuDatabase.Current.Buffer;
        //                String result;
        //                if (buffer.GetLength(1) * buffer.GetLength(0) > MAX_INPUT)
        //                {
        //                    var bufferdoubles = buffer.Cast<Double>().ToArray().Take(MAX_INPUT);
        //                    result = String.Join(",", bufferdoubles);
        //                }
        //                else
        //                {
        //                    var bufferdoubles = buffer.Cast<Double>().ToArray();
        //                    result = String.Join(",", bufferdoubles);
        //                }
        //                File.WriteAllText(filePath, result);
        //            }
        //        }
        //        ResultOut[chnlid] = RunPythonScript("-u", filePath);
        //        _ChannelId[chnlid] = 1;
        //        _UpdateBuildCnt[chnlid] = needCalcCnt;
        //    }
        //}
        //调制信号
        private Dictionary<(ChannelId, ChannelId), Int32> _ChannelIdM = new();

        internal Dictionary<(ChannelId, ChannelId), string> ResultOutM = new();
        private Dictionary<(ChannelId, ChannelId), Int32> _UpdateBuildCntM = new();
        private int MAX_INPUT_M = 100;
        internal void RunMSignalRecognition(ChannelId chnlidI, ChannelId chnlidQ,Int32 needCalcCnt)
        {
            int buildUpdatecnt = _UpdateBuildCntM.ContainsKey((chnlidI, chnlidQ)) ? _UpdateBuildCntM[(chnlidI, chnlidQ)] : 0;

            if (!_ChannelIdM.ContainsKey((chnlidI, chnlidQ))) _ChannelIdM[(chnlidI, chnlidQ)] = 0;
            if (_ChannelIdM[(chnlidI, chnlidQ)] == 0 || needCalcCnt != buildUpdatecnt)
            {
               // Double[] dataI, dataQ;
                
                if (DsoModel.Default.TryGetChannel(chnlidI, out var Ich)&& DsoModel.Default.TryGetChannel(chnlidQ, out var Qch))
                {
                    if (Ich.VuDatabase.Current != null&& Qch.VuDatabase.Current!= null)
                    {
                        Double[,] bufferI = Ich.VuDatabase.Current.Buffer;
                        Double[,] bufferQ = Qch.VuDatabase.Current.Buffer;
                        
                        if (bufferI.GetLength(1) * bufferI.GetLength(0) > MAX_INPUT)
                        {
                            var dataI= bufferI.Cast<Double>().ToArray();
                            var dataQ =bufferQ.Cast<Double>().ToArray();
                            //result = String.Join(",", bufferdoubles);
                        }
                        else
                        {
                            var dataI = bufferI.Cast<Double>().ToArray();
                            var dataQ = bufferQ.Cast<Double>().ToArray();
                            //result = String.Join(",", bufferdoubles);
                        }

                        IntelligentChartManager intelligentChartManager = new();
                        var matchType = intelligentChartManager.GetMatchType(bufferI);
                        //ResultOutM[(chnlidI, chnlidQ)] = RunPythonModel(dataI, dataQ);
                        ResultOutM[(chnlidI, chnlidQ)] = matchType;
                        _ChannelIdM[(chnlidI, chnlidQ)] = 1;
                        _UpdateBuildCntM[(chnlidI, chnlidQ)] = needCalcCnt;

                    }
                }
                
            }
        }
    }


#pragma warning disable CA1416

#pragma warning restore CA1416
    #region 识别是否是周期信号
    public class SignalAnalyzerForPeriod
    {
        private static SignalAnalyzerForPeriod _instance;
        public static SignalAnalyzerForPeriod Default => _instance ??= new SignalAnalyzerForPeriod();

        public string MatchTypeStr { get; private set; }
        public string SpecificSignalType { get; private set; }
        private IntelligentChartManager ChartManager => DsoModel.Default.IntelligentChartManager;

        private SignalAnalyzerForPeriod()
        {
            MatchTypeStr = "未知";
            SpecificSignalType = "未知";
        }
        // 迟滞比较
        private static List<int> CompareLevels(List<double> data, double levelH, double levelL)
        {
            var resultCompare = new List<int>();
            int prevValue = -1;

            foreach (var num in data)
            {
                if (num >= levelH && num >= levelL)
                {
                    resultCompare.Add(1);
                    prevValue = 1;
                }
                else if (num <= levelL && num <= levelH)
                {
                    resultCompare.Add(0);
                    prevValue = 0;
                }
                else
                {
                    resultCompare.Add(prevValue != -1 ? prevValue : 1);
                }
            }
            return resultCompare;
        }

        private static double CalculateThreshold(List<int> lengths)
        {
            if (lengths.Count <= 2)
            {
                return 0.0; // 如果长度小于等于2，无法去掉最大值和最小值，返回0
            }

            //int maxLength = lengths.Max();
            //int minLength = lengths.Min();

            //var filteredLengths = lengths.Where(length => length != maxLength && length != minLength);
            double meanLength = lengths.Skip(1).Average();

            return meanLength * 0.05; // 相对阈值
        }

        private static bool CheckConsecutiveOnesWidth(List<int> sequence, int n)
        {
            var lengths = new List<int>();   // 用来保存每段连续1的长度
            int currentCount = 0;            // 当前连续1的计数

            foreach (var num in sequence)
            {
                if (num == 1)
                {
                    currentCount++;
                }
                else if (currentCount > 0)
                {
                    lengths.Add(currentCount);
                    currentCount = 0;
                }
            }

            if (currentCount > 0)
            {
                lengths.Add(currentCount);
            }

            if (lengths.Count == 0)
            {
                return false;
            }

            int consecutiveCount = 1;
            double threshold = CalculateThreshold(lengths);
            int flag = 0;
            for (int i = 1; i < lengths.Count; i++)
            {
                if ((Math.Abs(lengths[i] - lengths[i - 1]) / lengths[i - 1]) < 0.2)
                {
                    if(flag == 1)
                    {
                        consecutiveCount++;
                    }
                    
                    flag = 1;
                    if (consecutiveCount == n)
                    {
                        return true;
                    }
                }
                else
                {
                    flag = 0;
                    consecutiveCount = 1;
                }
            }
            return false;
        }

        private static bool CheckConsecutiveWidth(List<int> sequence1, List<int> sequence2, List<int> sequence3, int n)
        {
            bool a = CheckConsecutiveOnesWidth(sequence1, n);
            bool b = CheckConsecutiveOnesWidth(sequence2, n);
            bool c = CheckConsecutiveOnesWidth(sequence3, n);
            return a &&
                   b &&
                   c;
        }

        // 信号分类函数：0非周期、1周期
        public static int SignalClassSimplePeriod(List<double> inputData, double levelH, double levelL)
        {
            //var ls = CompareLevels(inputData, levelH, levelL);
            //var ls20 = CompareLevels(inputData, levelH - 0.1 * (levelH - levelL), levelL + 0.1 * (levelH - levelL));
            var ls80 = CompareLevels(inputData, levelH - 0.05 * (levelH - levelL), levelL + 0.05 * (levelH - levelL));


            return CheckConsecutiveWidth(ls80, ls80, ls80, 15) ? 1 : 0;
        }

        internal void RunSignalAnalyzerForPeriod(ChannelId chnlid)
        {
            int result = -1;
            if (DsoModel.Default.TryGetChannel(chnlid, out var data) && data.VuDatabase.Current != null)
            {   
                Double[,] buffer = data.VuDatabase.Current.Buffer;
                List<double> input = new List<double>(buffer.Cast<double>());
                double levelH = input.Max() - 0.05 * (input.Max() - input.Min());
                double levelL = input.Min() + 0.05 * (input.Max() - input.Min());
                result = SignalClassSimplePeriod(input, levelH, levelL);
                MatchTypeStr = result == 1 ? "周期信号" : "非周期信号";
                // 切换测量参数
                //if(DsoModel.Default.ArtificialIntelligence.ParameterDisplay)
                //{
                //    SwitchMeasureParameters(chnlid);
                //}
                //SwitchMeasureParameters(chnlid);
            }
            else
            {
                Console.WriteLine($"Channel {chnlid} 数据不可用或为空。");
                MatchTypeStr = "数据无效";
            }
        }

        //public void SwitchMeasureParameters(ChannelId channelId)
        //{
        //    Task.Run(() =>
        //    {
        //        var measureModel = DsoModel.Default.Meas;
        //        if (measureModel == null) return;

        //        // 定义当前信号类型需要的测量参数
        //        string[] measureItems = MatchTypeStr switch
        //        {
        //            "周期信号" => new[] { "Period", "Freq", "PWidth", "Duty", "Rise", "Fall" },
        //            "非周期信号" => new[] { "Average", "RMS", "Amplitude", "POverShoot", "NOverShoot", "Top", "Base" },
        //            _ => Array.Empty<string>()
        //        };

        //        // 检查当前激活的测量参数是否与需要的一致
        //        bool needsUpdate = false;
        //        var activeItems = measureModel.SelectedItems.Where(x => x.Active).ToList();

        //        if (activeItems.Count != measureItems.Length)
        //        {
        //            needsUpdate = true;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < measureItems.Length; i++)
        //            {
        //                if (activeItems[i].Name != measureItems[i] || activeItems[i].Source != channelId)
        //                {
        //                    needsUpdate = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // 只在需要更新时执行开关操作
        //        if (needsUpdate)
        //        {
        //            foreach (var item in measureModel.SelectedItems)
        //            {
        //                item.Active = false;
        //            }

        //            for (int i = 0; i < Math.Min(measureItems.Length, measureModel.SelectedItems.Length); i++)
        //            {
        //                measureModel.SelectedItems[i].Name = measureItems[i];
        //                measureModel.SelectedItems[i].Source = channelId;
        //                measureModel.SelectedItems[i].Active = true;
        //            }

        //            measureModel.Active = measureItems.Length > 0;
        //        }
        //    });
        //}

        public void RunSignalAnalyzerForSpecificType(ChannelId chnlid)
        {
            RunSignalAnalyzerForPeriod(chnlid);

            if (DsoModel.Default.TryGetChannel(chnlid, out var data) && data.VuDatabase.Current != null)
            {   
                Double[,] buffer = data.VuDatabase.Current.Buffer;
            
                // 根据 MatchTypeStr 的结果选择使用的模型
                IntelligentChartManager.ONNXModelType modelType = MatchTypeStr == "周期信号" ? IntelligentChartManager.ONNXModelType.Period : IntelligentChartManager.ONNXModelType.AMR;
            
                ChartManager.SetModelType(modelType);
            }
            else
            {
                Console.WriteLine($"Channel {chnlid} 数据不可用或为空。");
                SpecificSignalType = "数据无效";
            }
        }

    }
    #endregion



}
