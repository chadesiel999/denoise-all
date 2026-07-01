using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal static class SignalIntegrity
    {
        #region Matlab
        public static Boolean ChannelSim(Double[] inputData, String s2pFilePath, out Double[] outputData)
        {
            if (SignalIntegrityMatlab.TryExcuteCS(inputData, s2pFilePath, out var outData))
            {
                outputData = outData.ToJagged()[0];
                return true;
            }
            outputData = new Double[0];
            return false;
        }

        public static Boolean RxFFE(Double[] inputData, String tapPath, Int32 delay, out Double[] outputData)
        {
            if (SignalIntegrityMatlab.TryExcuteRE(inputData.ToMatrix(1, inputData.Count()), tapPath, delay, out var outData))
            {
                outputData = outData.ToJagged()[0];
                return true;
            }
            outputData = new Double[0];
            return false;
        }
    
        #endregion

#if MATLAB
        //#region C#
        //public static Boolean ChannelSim(Double[] inputData, String s2pFilePath, out Double[] outputData)
        //{
        //    if (!File.Exists(s2pFilePath))
        //    {
        //        outputData = inputData;
        //        return false;
        //    }
        //    String[] strArray = File.ReadAllLines(s2pFilePath);

        //    Complex[] S21_pos_1 = new Complex[strArray.Length];
        //    Double _real, _imag;
        //    Int32 j = 0;
        //    foreach (var str in strArray)
        //    {
        //        String[] strline = str.Split(" ");
        //        String[] sParameter = new String[strArray.Length];
        //        Int32 i = 0;
        //        foreach (var line in strline)
        //        {
        //            if (line != "")
        //            {
        //                sParameter[i] = line;
        //                i++;
        //            }
        //        }
        //        if (i == 9 && sParameter[0] != "!freq")
        //        {
        //            _real = Convert.ToDouble(sParameter[3]);
        //            _imag = Convert.ToDouble(sParameter[4]);
        //            S21_pos_1[j] = new Complex(_real, _imag);
        //            j++;

        //        }
        //    }
        //    Complex[] S21_pos = new Complex[j];
        //    Array.Copy(S21_pos_1, S21_pos, j);
        //    Complex[] S21_neg = new Complex[S21_pos.Length - 2];
        //    for (Int32 i = 0; i < S21_pos.Length - 2; i++)
        //    {
        //        S21_neg[S21_pos.Length - 3 - i] = Complex.Conjugate(S21_pos[i + 1]);
        //    }

        //    Complex[] S21_F = new Complex[S21_pos.Length * 2 - 2];


        //    Array.Copy(S21_pos, S21_F, S21_pos.Length);

        //    Array.Copy(S21_neg, 0, S21_F, S21_pos.Length, S21_neg.Length);

        //    var S21_T = S21_F.IFFT().Real().ToArray();

        //    outputData = Algorithm.Convolve(S21_T, inputData, ConvModeOpt.Full).Select(o => o / 2).ToArray();

        //    return true;
        //}

        //public static Boolean RxFFE(Double[] data, String tapPath, Int32 delay, out Double[] result)
        //{
        //    if (!File.Exists(tapPath))
        //    {
        //        result = data;
        //        return false;
        //    }

        //    String[] strArray = File.ReadAllLines(tapPath);
        //    String[] firstLine = strArray[0].Split(' ');
        //    String[] secondLine = strArray[1].Split(' ');
        //    Double[] preCursor = new Double[firstLine.Length];
        //    Double[] postCursor = new Double[secondLine.Length];
        //    Int32 preLen = preCursor.Length;
        //    Int32 postLen = postCursor.Length;
        //    Int32 dataLen = data.Length;
        //    for (Int32 i = 0; i < preLen; i++)
        //    {
        //        preCursor[i] = Convert.ToDouble(firstLine[i]);
        //    }
        //    for (Int32 i = 0; i < postLen; i++)
        //    {
        //        postCursor[i] = Convert.ToDouble(secondLine[i]);
        //    }

        //    //Int32 delay = (Int32)Math.Round(UIPoints);
        //    Stopwatch stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    Int32 resultLen = dataLen - (preLen + postLen - 1) * delay;
        //    result = new Double[resultLen];

        //    for (Int32 i = 0; i < preLen; i++)
        //    {
        //        Double[] tmp = data.Skip(dataLen - i * delay - resultLen).Take(resultLen).Select(o => o * preCursor[preLen - 1 - i]).ToArray();
        //        result = AddArrays(result, tmp);
        //    }

        //    for (Int32 i = 0; i < postLen; i++)
        //    {
        //        Double[] tmp = data.Skip(i * delay).Take(resultLen).Select(o => o * postCursor[postLen - 1 - i]).ToArray();
        //        result = AddArrays(result, tmp);
        //    }
        //    var time1 = stopwatch.ElapsedMilliseconds;
        //    return true;

        //}

        //private static Double[] AddArrays(Double[] a, Double[] b)
        //{
        //    if (a.Length != b.Length)
        //        throw new ArgumentException("Input lengths are not equal!");
        //    Double[] result = new Double[a.Length];
        //    for (Int32 i = 0; i < a.Length; i++)
        //    {
        //        result[i] = a[i] + b[i];
        //    }

        //    return result;
        //}



        //#endregion
#endif
    }
    
    public static class SignalIntegrityMatlab
    {
        private static readonly Task _InitMatlabEngineTask = new(Init);

        private const BindingFlags _DEFAULT_BINDING_FLAGS = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static;

        private static Object? _MatlabInstance = null;
        private static Type? _MatlabType = null;

        private static Boolean _IsInitOk = false;
        private static String _WorkFolder = "";

        private static void Init()
        {
            if (_IsInitOk)
                return;
            try
            {
                _MatlabType = Type.GetTypeFromProgID("Matlab.Application");
                if (_MatlabType == null)
                {
                    return;
                }
                _MatlabInstance = Activator.CreateInstance(_MatlabType);
                if (_MatlabInstance == null)
                {
                    return;
                }
                _MatlabType.InvokeMember("Visible", BindingFlags.SetProperty, null, _MatlabInstance, new Object[] { false });
                _IsInitOk = true;
            }
            catch
            {
                _IsInitOk = false;
            }
        }

        private static Boolean CheckMatlabState()
        {
            _InitMatlabEngineTask.Wait();
            if (!_IsInitOk || _MatlabType == null || _MatlabInstance == null)
                return false;
            return true;
        }

        public static Boolean TryExcuteCS(Double[] Data, String SPath, [NotNullWhen(true)] out Double[,]? result)
        {
            result = null;
            if (!CheckMatlabState())
                return false;

            String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Matlab");

            if (!Directory.Exists(path))
                return false;

            Object? matlabresult = _MatlabType!.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { $"cd('{path}')" });
            matlabresult = _MatlabType!.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "clear all" });

            _MatlabType!.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "Data", "base", new Object[] { Data } });
            _MatlabType!.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "SPath", "base", new Object[] { SPath } });

            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "data = cell2mat(Data);" });
            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "spath = cell2mat(SPath);" });
            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "resultData = ChannelSim(data,spath );" });

            // try-catch必须留着，因为没法确保用户写的函数一定能运行成功，如果用户代码报错，matlab工作区将没有resultData，直接获取会导致程序崩溃
            try
            {
                Object? resultdata = _MatlabType.InvokeMember("GetVariable", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "resultData", "base" });
                result = resultdata as Double[,];
            }
            catch
            {
                return false;
            }
            return result == null ? false : true;
        }

        public static Boolean TryExcuteRE(Double[,] Data, String SPath, Int32 UIPoints, [NotNullWhen(true)]out Double[,]? result)
        {
            result = null;
            if (!CheckMatlabState())
                return false;


            String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Matlab");

            if (!Directory.Exists(path))
                return false;
            Object? matlabresult = _MatlabType!.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { $"cd('{path}')" });
            matlabresult = _MatlabType!.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "clear all" });

            _MatlabType!.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "Data", "base", new Object[] { Data } });
            _MatlabType!.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "SPath", "base", new Object[] { SPath } });
            _MatlabType!.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "UIPoints", "base", new Object[] { UIPoints } });

            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "data = cell2mat(Data);" });
            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "spath = cell2mat(SPath);" });
            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "UIPoints = cell2mat(UIPoints);" });
            matlabresult = _MatlabType.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "resultData = RxEqualisation(data,spath,UIPoints );" });

            try
            {
                Object? resultdata = _MatlabType.InvokeMember("GetVariable", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new Object[] { "resultData", "base" });
                result = resultdata as Double[,];
            }
            catch
            {
                return false;
            }

            return result == null ? false : true;
        }

        public static void AsyncInit()
        {
            if (_InitMatlabEngineTask.Status != TaskStatus.Running)
                _InitMatlabEngineTask.Start();
        }

    }
    
}
