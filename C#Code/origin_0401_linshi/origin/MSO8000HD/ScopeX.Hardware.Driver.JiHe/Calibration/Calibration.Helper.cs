using ScopeX.ComModel;
using ScopeX.MathExt.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    internal enum Coupling
    {
        HighImpedance = AnaChnlCoupling.DC1M,
        LowImpedance = AnaChnlCoupling.DC50
    }

    internal enum PosDiv
    {
        Pos0Div = 0,
        Pos3Div_N = -3,
        Pos3Div_P = 3,
    }
    internal enum AutoCaliOffsetStatus
    {
        Working = 1,
        Failure = 2,
        Ok = 3
    }
    public partial class Cali
    {

        String BasePath = AppDomain.CurrentDomain.BaseDirectory;
        String CaliResultSavedFileName = @"{0}CaliData\AcqProcBdLoopScanResult_{1}.txt";
        String SendError = "BoardInteractionDelay_Send To {0} Error:{1} ";
        Int32 BoardInteractionDelay_Data = 8;
        Int32 BoardInteractionDelay_Ctrl = 7;

        private void ConfigHardware(HdMessage currHdMessage, Int16 DelayMsAfterHardwareChannged)
        {
            Hd.Execute(currHdMessage);
            Thread.Sleep(DelayMsAfterHardwareChannged);
        }


        public Boolean AcqWaveData(out List<UInt16[]> wavedata, Int16 timeoutByMs = 2000, HdMessage? currHdMessage = null)
        {
            wavedata = new();
            Dictionary<AcqDataType, Double> _SamplingRate = new();

            List<ReadInfo> readinfolist = new();
            WfmPkgInfo viewpkg = new(Hd.UIMessage!.Timebase!.NeedWaveDotsCnt, Hd.UIMessage!.Timebase!.TmbScale * Constants.VIS_XDIVS_NUM, Hd.UIMessage!.Timebase!.TmbPosition);
            ReadInfo viewinfo = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), viewpkg, "View");
            readinfolist.Add(viewinfo);

            Stopwatch _StopWatcher = new();
            _StopWatcher.Restart();
            _ = Hd.AcqWave(false, true, readinfolist, ref _SamplingRate);
            var bok = false;//多才集几次
            while (!bok && _StopWatcher.ElapsedMilliseconds < timeoutByMs)
            {
                bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
            }
            _StopWatcher.Stop();

            if (!bok)
            {
                if (currHdMessage != null)
                {
                    ConfigHardware(currHdMessage, 400);
                }
                _StopWatcher.Restart();
                _ = Hd.AcqWave(false, true, readinfolist, ref _SamplingRate);
                while (!bok && _StopWatcher.ElapsedMilliseconds < timeoutByMs)
                {
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                }
                _StopWatcher.Stop();
            }

            if (!bok)
            {
                if (currHdMessage != null)
                {
                    ConfigHardware(currHdMessage, 400);
                }
                _StopWatcher.Restart();
                _ = Hd.AcqWave(false, true, readinfolist, ref _SamplingRate);
                while (!bok && _StopWatcher.ElapsedMilliseconds < timeoutByMs)
                {
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                    bok = Hd.AcqWave(false, false, readinfolist, ref _SamplingRate);
                }
                _StopWatcher.Stop();

            }
            if (!bok)
            {
                Hd.SysLogger?.Invoke("校准时数据采集错误！！！", "Debug");
            }
#if DEBUG
            Debug.WriteLine($"[AutoCalibration] [{DateTime.Now.ToString("G")}]:AcqWaveData Time = {_StopWatcher.ElapsedMilliseconds}ms");
#endif
            _StopWatcher.Stop();

            var readinfo = readinfolist.FirstOrDefault(info => info.DataType == AcqDataType.AnalogChannel);

            if (bok)
            {
                for (Int16 channel = 0; channel < ChannelIdExt.AnaChnlNum; channel++)
                {
                    Hd.AnalogChannel!.TryTakeWave((ChannelId)channel, readinfo!, out var d, out _, null);
                    wavedata.Add(d.ToArray());
                }
            }
            return bok;
        }

        private static Boolean GetAcqBdDescription(Enum value, out String result)
        {
            try
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                result = attributes.Length > 0 ? attributes[0].Description : value.ToString();
                return true;
            }
            catch (Exception e)
            {
                result = $"{e.Message}\n{e.StackTrace}";
            }
            return false;
        }

        private static Boolean _WriteScanResultToFile => Constants.ENABLE_DEBUG;
        private static void FileWrite(String fileName,String content)
        {
            if(_WriteScanResultToFile)
            {
                String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CaliData", $"{fileName}.txt");
                var dir = Path.GetDirectoryName(fileName);
                if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(path, content);
            }
        }
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
            waveOffsetGainPhase.Phase = (Math.Atan2(A1, B1));//相位

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
