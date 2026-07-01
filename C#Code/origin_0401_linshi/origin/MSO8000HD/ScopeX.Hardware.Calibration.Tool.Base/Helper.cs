using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CSScriptLib;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using Microsoft.Office.Interop.Excel;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public delegate BatchTaskPartResult ProcessXMLAndPartTask_UIFunc(XmlScpiCmd xmlScpiCmd, IInstrumentSession? instrumentSession, out string message, Action<int, string, string>? updateAction, CancellationToken? cancellationToken);
    public class BaseHelper
    {
        #region matlabInstalled
        private static bool matlabInstalled = false;
        public static void InitSystemInfo()
        {
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine;//注册表用于存储系统和应用程序的设置信息
            matlabInstalled = true;// (regkey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\matlab.exe", false) != null);
        }
        public static bool bMatlabInstalled
        {
            get => matlabInstalled;
            private set
            {
                matlabInstalled = value;
            }
        }
        #endregion
        #region Reflection Defined Class
        public static List<Type>? AllBatchTaskClass = null;
        public static List<Type>? AllBatchTaskPartClass = null;
        public static List<Type>? AllMatlabSourceCodePrePosProcessorClass = null;
        public static void InitDomainClass(Assembly mainAssembly)
        {
            Type[] mainProgramTypes = mainAssembly.GetTypes();
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass
                        && (t.BaseType?.Name ?? "") == "BatchTaskBase"
                    select t;
            AllBatchTaskClass = q.ToList();
            var q2 = from t in mainProgramTypes
                     where t.IsClass
                         && (t.BaseType?.Name ?? "") == "BatchTaskBase"
                     select t;
            if (q2 != null)
                AllBatchTaskClass.AddRange(q2.ToArray());
            //============
            var m = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass
                        && (t.BaseType?.Name ?? "") == "BatchTaskPartBase"
                    select t;
            AllBatchTaskPartClass = m.ToList();
            var m2 = from t in mainProgramTypes
                     where t.IsClass
                         && (t.BaseType?.Name ?? "") == "BatchTaskPartBase"
                     select t;
            if (m2 != null)
                AllBatchTaskPartClass.AddRange(m2.ToArray());
            //===========
            var n = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass
                        && t.Name.IndexOf("MatlabSourceCodePrePosProcessor") == 0
                    select t;

            AllMatlabSourceCodePrePosProcessorClass = n.ToList();

            var n2 = from t in mainProgramTypes
                     where t.IsClass
                         && t.Name.IndexOf("MatlabSourceCodePrePosProcessor") == 0
                     select t;
            if (n2 != null)
                AllMatlabSourceCodePrePosProcessorClass.AddRange(n2.ToArray());
        }
        #endregion

        #region
        public static string TryConvertToString(string value)
        {
            if (value == null)
                return "";
            return value;
        }
        public static int TryConvertToInt(string value)
        {
            if (int.TryParse(value, out int int_value))
                return int_value;
            else
                return 0;
        }
        public static double TryConvertToDouble(string value)
        {
            if (double.TryParse(value, out double double_value))
                return double_value;
            else
                return 0;
        }
        public static long TryConvertToLong(string value)
        {
            if (long.TryParse(value, out long int_value))
                return int_value;
            else
                return 0;
        }
        public static bool TryConvertToBoolean(string value)
        {
            return value switch
            {
                "true" => true,
                "TRUE" => true,
                "1" => true,
                _ => false,
            };
        }
        #endregion

        public static string[]? SplitClassNameAndParameter(string nameAndParameter)
        {
            string tmpNameAndParameter = nameAndParameter.Trim();
            int firstSpacePos = tmpNameAndParameter.IndexOf(' ');
            if (firstSpacePos < 0)
                return null;
            string[] returnValue = new string[2];
            returnValue[0] = tmpNameAndParameter.Substring(0, firstSpacePos).Trim();
            returnValue[1] = tmpNameAndParameter.Substring(firstSpacePos, tmpNameAndParameter.Length - firstSpacePos).Trim();
            return returnValue;
        }
        public static bool CheckAndTipInstrumentSession(IInstrumentSession? instrumentSession)
        {
            if (instrumentSession == null)
            {
                MessageBox.Show("请先连接待测试的示波器！");
                return false;
            }
            return true;
        }
        public static MethodInfo? GetGenerateCoefficientsMatlabDllFunc(Type type, int parameterCount)
        {
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo m in methodInfos)
            {
                //if (m.ReturnType.Name == "MWArray")
                {
                    if (m.GetParameters().Length == parameterCount)
                        return m;
                }
            }
            return null;
        }
        public static bool CalcBooleanFormula(string exp, string paramStr)
        {
            string newFormula = exp.Replace("@result", paramStr);
            String script =
                    $@"using System;
                    public class CSharpCode
				    {{
	 					public bool Exec()
	                    {{
							return {newFormula};
				        }}
				    }}";
            try
            {
                dynamic _Code = CSScript.Evaluator.LoadCode(script);
                return (bool)_Code!.Exec();
            }
            catch
            {
                return false;
            }
        }
        public static object? CalcCSharpFormat(string exp)
        {
            string newFormula = exp;
            String script =
                    $@"using System;
                    public class CSharpCode
				    {{
	 					public object Exec()
	                    {{
							return {newFormula};
				        }}
				    }}";
            try
            {
                dynamic _Code = CSScript.Evaluator.LoadCode(script);
                return (object)_Code!.Exec();
            }
            catch
            {
                return null;
            }
        }
        private static Dictionary<string, string> SpecaialCharMarkList = new Dictionary<string, string>()
        {
            {"@char_commat@","@" },
            {"@char_colon@",":" },
            {"@char_comma@","," },
            {"@char_verbar@","|" },
            {"@char_num@","#" },
            {"@char_lt@","<" },
            {"@char_gt@",">" },
        };
        public static string ReplaceESCChar(string sourceStr)
        {
            string result = sourceStr;
            foreach (var kvp in SpecaialCharMarkList)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }
            return result;
        }

        public static List<ProcessXMLAndPartTask_UIFunc> ProcessXMLAndPartTask_UIFuncList = new List<ProcessXMLAndPartTask_UIFunc>();
    }
    public class SineFitFunc
    {
        private static Double AVG1(Int16[] a)
        {
            Double avg = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                avg += a[i];
            avg /= M;
            return (avg);
        }
        private static Double AVG1(UInt16[] a)
        {
            Double avg = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                avg += a[i];
            avg /= M;
            return (avg);
        }

        private static Double AVG2(Double[] a)
        {
            Double avg = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                avg += a[i];
            avg /= M;
            return (avg);
        }

        private static Double SUM(ref Double[] a)
        {
            Int32 M = a.Length;
            Double sum = 0;
            Int32 i;
            for (i = 0; i < M; i++)
                sum += a[i];
            return (sum);
        }

        private static Double SPFH(ref Double[] a) //平方和
        {
            Double spfh = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                spfh += a[i] * a[i];
            return (spfh);
        }

        private static Double DCJ1(ref Int16[] a, ref Double[] b) //两数先相乘，再相加
        {
            Double dcj = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                dcj += a[i] * b[i];
            return (dcj);
        }
        private static Double DCJ1(ref UInt16[] a, ref Double[] b) //两数先相乘，再相加
        {
            Double dcj = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                dcj += a[i] * b[i];
            return (dcj);
        }

        private static Double DCJ2(ref Double[] a, ref Double[] b) //两数先相乘，再相加
        {
            Double dcj = 0;
            Int32 i;
            Int32 M = a.Length;
            for (i = 0; i < M; i++)
                dcj += a[i] * b[i];
            return (dcj);
        }

        private static Double[]? m;
        private static Double[]? n;
        private static Double a, b, c, d, e, f, g, h, p, q;

        private static Double parameterA(Int16[] y) //经验公式A*cos(w*tn)+B*sin(w*tn)+C,求系数A
        {
            Double An, Ad, A1;

            a = AVG1(y);
            b = AVG2(m!);
            c = AVG2(n!);
            d = DCJ1(ref y, ref m!);
            e = DCJ2(ref m, ref n!);
            f = DCJ1(ref y, ref n);
            g = SUM(ref m);
            h = SUM(ref n);
            p = SPFH(ref m);
            q = SPFH(ref n);

            An = (d - a * g) / (e - c * g) - (f - a * h) / (q - c * h);
            Ad = (p - b * g) / (e - c * g) - (e - b * h) / (q - c * h);
            A1 = An / Ad;
            return (A1);
        }
        private static Double parameterA(UInt16[] y) //经验公式A*cos(w*tn)+B*sin(w*tn)+C,求系数A
        {
            Double An, Ad, A1;

            a = AVG1(y);
            b = AVG2(m!);
            c = AVG2(n!);
            d = DCJ1(ref y, ref m!);
            e = DCJ2(ref m, ref n!);
            f = DCJ1(ref y, ref n);
            g = SUM(ref m);
            h = SUM(ref n);
            p = SPFH(ref m);
            q = SPFH(ref n);

            An = (d - a * g) / (e - c * g) - (f - a * h) / (q - c * h);
            Ad = (p - b * g) / (e - c * g) - (e - b * h) / (q - c * h);
            A1 = An / Ad;
            return (A1);
        }

        private static Double parameterB(Int16[] y) //求系数B
        {
            Double Bn, Bd, B1;

            Bn = (d - a * g) / (p - b * g) - (f - a * h) / (e - b * h);
            Bd = (e - c * g) / (p - b * g) - (q - c * h) / (e - b * h);
            B1 = Bn / Bd;
            return (B1);
        }
        private static Double parameterB(UInt16[] y) //求系数B
        {
            Double Bn, Bd, B1;

            Bn = (d - a * g) / (p - b * g) - (f - a * h) / (e - b * h);
            Bd = (e - c * g) / (p - b * g) - (q - c * h) / (e - b * h);
            B1 = Bn / Bd;
            return (B1);
        }

        private static Double A1, B1;

        private static Double parameterC(Int16[] y, double sampleByM_Sps, double signalFreqByMHz, Int32 M) //求系数C
        {
            Int32 i;
            Double C1;
            for (i = 0; i < M; i++)
            {
                m![i] = Math.Cos(2 * 3.141592 * i * signalFreqByMHz / sampleByM_Sps);                  //fx为输入正弦波频率，MHz为单位，每个AD1.25G采样率 2*Pi*fx/fs
                n![i] = Math.Sin(2 * 3.141592 * i * signalFreqByMHz / sampleByM_Sps);
            }
            A1 = parameterA(y);
            B1 = parameterB(y);
            C1 = a - A1 * b - B1 * c;
            return (C1);
        }
        private static Double parameterC(UInt16[] y, double sampleByM_Sps, double signalFreqByMHz, Int32 M) //求系数C
        {
            Int32 i;
            Double C1;
            for (i = 0; i < M; i++)
            {
                m![i] = Math.Cos(2 * 3.141592 * i * signalFreqByMHz / sampleByM_Sps);                  //fx为输入正弦波频率，MHz为单位，每个AD1.25G采样率 2*Pi*fx/fs
                n![i] = Math.Sin(2 * 3.141592 * i * signalFreqByMHz / sampleByM_Sps);
            }
            A1 = parameterA(y);
            B1 = parameterB(y);
            C1 = a - A1 * b - B1 * c;
            return (C1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databuffer"></param>
        /// <param name="estationresult">返回的计算结果0-offset,1-gain,2-phase</param>
        /// <param name="sampleByM_Sps">以Msps表示的当前数据的采样率</param>
        /// <param name="signalFreqByMHz">MHz表示当前输入信号的频率</param>
        public static WaveOffsetGainPhase SineFit(Int16[] databuffer, double sampleByM_Sps, double signalFreqByMHz)
        {
            m = new Double[databuffer.Length];
            n = new Double[databuffer.Length];

            WaveOffsetGainPhase waveOffsetGainPhase = new();
            //正弦拟合计算偏置和增益
            waveOffsetGainPhase.Offset = parameterC(databuffer, sampleByM_Sps, signalFreqByMHz, databuffer.Length);//输入100MHz正弦波,偏置
            waveOffsetGainPhase.Gain = Math.Sqrt(A1 * A1 + B1 * B1);//增益

            //if (A1 < 0)
            //    waveOffsetGainPhase.Phase = (Math.Atan(-B1 / A1));//相位
            //else
            //    waveOffsetGainPhase.Phase = (Math.Atan(-B1 / A1) + 3.141592);
            waveOffsetGainPhase.Phase = (Math.Atan2(A1, B1));//相位
            return waveOffsetGainPhase;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databuffer"></param>
        /// <param name="estationresult">返回的计算结果0-offset,1-gain,2-phase</param>
        /// <param name="sampleByM_Sps">以Msps表示的当前数据的采样率</param>
        /// <param name="signalFreqByMHz">MHz表示当前输入信号的频率</param>
        public static WaveOffsetGainPhase SineFit(UInt16[] databuffer, double sampleByM_Sps, double signalFreqByMHz)
        {
            m = new Double[databuffer.Length];
            n = new Double[databuffer.Length];

            WaveOffsetGainPhase waveOffsetGainPhase = new();
            //正弦拟合计算偏置和增益
            waveOffsetGainPhase.Offset = parameterC(databuffer, sampleByM_Sps, signalFreqByMHz, databuffer.Length);//输入100MHz正弦波,偏置
            waveOffsetGainPhase.Gain = Math.Sqrt(A1 * A1 + B1 * B1);//增益

            if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X||ServerDomainConstants.ProductType == ProductType.B24_AI20G)
            {
                waveOffsetGainPhase.Phase = (Math.Atan2(A1, B1));//相位
            }
            else //ProductType.JiHe_MSO7000X
            {
                if (A1 < 0)
                    waveOffsetGainPhase.Phase = (Math.Atan(-B1 / A1)) * 1000000;//相位
                else
                    waveOffsetGainPhase.Phase = (Math.Atan(-B1 / A1) + 3.141592) * 1000000;
            }
            return waveOffsetGainPhase;
        }

    }
    public class WaveOffsetGainPhase
    {
        public double Offset;
        public double Gain;
        public double Phase;
    }
}
