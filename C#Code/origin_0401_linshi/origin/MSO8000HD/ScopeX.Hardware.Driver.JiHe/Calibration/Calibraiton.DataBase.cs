using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        //private String AnalogChannelTemperaturesCoefficientFile => $@"AnalogChannelTemperaturesCoefficientFile.txt";
        //private String CaliDataPath => $@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\";

        //private String TemperatureCaliLog => $"TemperatureCaliLog{DateTime.Now.ToString("yyyyMMdd")}.txt";

        //internal Boolean IsCaliTemperatureOffset { get; private set; } = false;

        //private String BaselineCaliLogFileName => $"BaselineCaliLog{DateTime.Now.ToString("yyyy-MM-dd")}.txt";

        //private volatile Int32 _FinishedItemCount = 0;

        //private const Int32 MaxIterationCount = 20;

        //private const Double CaliError = 0.5D;//偏差

        //private const Double CaliRMS = 0.015D;//误差

        //private const Double DefaultTemperatureCoeffcient = 0D;

        //private const Double TemperatureDiff = 10D;

        //private const Double CoeffcientError = 0.05D;

        //private Double MaxppByADC = Constants.SAMPS_PER_YDIV * 2;

        //private Double MaxOffsetByADC = Constants.SAMPS_PER_YDIV * 3;

        //internal Boolean IsCalibration { get; private set; } = false;

        //private Boolean AverageAcqData(UInt16[] source, out Double average)
        //{
        //    if (source == null || source.Length <= 0)
        //    {
        //        average = 0D;
        //        return false;
        //    }
        //    var buffer = source.Select(x => (Double)x).ToList();
        //    average = buffer.Average();
        //    return true;
        //}

        ////幅度自校正
        //private Int32 GainCaliCount = (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv1m + 1);

        ////计算基线自校正 0Div、±3Div
        //private Int32 BaseLineCaliCount = 3 * (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv10m + 1);

        ////计算外触发校正 {AC,DC,LFR,HFR}{Ext,Ext5}{1MΩ,50Ω}
        //private Int32 ExtCaliCount = 4 * 2 * 2;

        //public Int32 GetTotalItemCount()
        //{
        //    var count = 0;

        //    if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)//只有低阻
        //    {
        //        //幅度自校正 ±3Div
        //        GainCaliCount = (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv1m + 1);
        //        count += GainCaliCount;

        //        //计算基线自校正 0Div、±3Div
        //        BaseLineCaliCount = 3 * (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv10m + 1);
        //        count += BaseLineCaliCount;
        //    }

        //    else//高低阻
        //    {
        //        //幅度自校正 ±3Div
        //        GainCaliCount = (AnaChnlScaleIndex.Lv10 - AnaChnlScaleIndex.Lv1m + 1) + (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv1m + 1);
        //        count += GainCaliCount;

        //        //计算基线自校正 0Div、±3Div
        //        BaseLineCaliCount = 3 * (AnaChnlScaleIndex.Lv10 - AnaChnlScaleIndex.Lv10m + 1) + 3 * (AnaChnlScaleIndex.Lv1 - AnaChnlScaleIndex.Lv10m + 1);
        //        count += BaseLineCaliCount;
        //    }
        //    //计算外触发校正 {AC,DC,LFR,HFR}{Ext,Ext5}{1MΩ,50Ω}
        //    count += ExtCaliCount;
        //    return count;
        //}

        //public Int32 GetFinishedCount() => _FinishedItemCount;

        //public void ClearFinishedCount() => _FinishedItemCount = 0;

        private String CaliLogPath { get; set; } = String.Empty;

        private void AppendCaliLog(String info)
        {
            //using (var sw = new StreamWriter(CaliLogPath, true))
            //{
            //    sw.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")}：{info}");
            //}
        }

        /// <summary>
        /// 清除校准日志
        /// </summary>
        private void CleanUpOldLogs(String Filter, String logFileName)
        {
            try
            {
                var logdirectory = Path.GetDirectoryName(logFileName);
                var logfilepattern = $"{Filter}*.txt";
                var alllogfiles = Directory.GetFiles(logdirectory, logfilepattern);

                // 按日期排序（最新的文件在前）
                var sortedLogFiles = alllogfiles
                    .Select(file => new FileInfo(file))
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                // 动态计算保留的时间范围
                DateTime cutoffDate = DateTime.Now;
                var weekstocheck = 1;
                var maxweekstocheck = 12; // 最大检查12周（约3个月）

                while (weekstocheck <= maxweekstocheck)
                {
                    cutoffDate = DateTime.Now.AddDays(-7 * weekstocheck);
                    var hasLogsInRange = sortedLogFiles.Any(f => f.CreationTime >= cutoffDate);

                    if (hasLogsInRange)
                    {
                        break; // 找到有效日志，停止扩展
                    }
                    weekstocheck++;
                }

                // 删除超出时间范围的文件
                foreach (var file in sortedLogFiles)
                {
                    if (file.CreationTime < cutoffDate)
                    {
                        try
                        {
                            file.Delete();
                            Hd.SysLogger?.Invoke($"Deleted old log: {file.Name} (Older than {weekstocheck} weeks)", "Info");
                        }
                        catch (Exception ex)
                        {
                            Hd.SysLogger?.Invoke($"Delete failed: {file.Name}, Error: {ex.Message}", "Info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Hd.SysLogger?.Invoke($"Log cleanup error: {ex.Message}", "Info");
            }
        }
    }

    internal class CaliStateManager
    {
        public enum CaliItem
        {
            Gain=0,
            Offset=1,
        }

        public class CaliState
        {
            public enum Flag
            {
                Continue,
                Failed,
                Succeed,
            }

            private readonly (int min, int max) AdcRange = (-65535, 65535);        //校准值取值范围

            //使用过的寄存器值,误差对的集合
            private List<KeyValuePair<int, double>> _RegErrPairs = new List<KeyValuePair<int, double>>();

            /// <summary>
            /// 当前状态标识
            /// </summary>
            public Flag CurrFlag { set; get; } = Flag.Continue;

            public Boolean IsFirstAdjust { get; set; } = true;

            public int AdjustStep { set; get; } = 1;

            public Boolean IsItemCompleted() => !(CurrFlag == CaliState.Flag.Continue);

            public int GetCaliCount() => _RegErrPairs.Count;

            public void AddRegErr(int regValue, double errValue)
            {
                _RegErrPairs.Add(new KeyValuePair<int, double>(regValue, errValue));
            }

            public int CalculateReg() => CalulateBase();

            private int CalulateBase()
            {
                int tempReg = _RegErrPairs.Last().Key + AdjustStep;

                return Math.Min(AdcRange.max, Math.Max(AdcRange.min, tempReg));
            }
        }

        //校准工作状态的三维数组：1）通道;2）acqUnit;3) 校准项(Gain,Phase);
        private CaliState[,,] CaliStates;
        public CaliStateManager(Int32 chnlCount, Int32 acqUnitCount)
        {
            int caliItemCount = Enum.GetValues(typeof(CaliItem)).Length;
            CaliStates = new CaliState[chnlCount, acqUnitCount, caliItemCount];
            for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
            {
                for (int adcId = 0; adcId < CaliStates.GetLength(1); adcId++)
                {
                    for (int caliItemId = 0; caliItemId < CaliStates.GetLength(2); caliItemId++)
                    {
                        CaliStates[chnlId, adcId, caliItemId] = new CaliState();
                    }
                }
            }
        }

        public CaliState GetCaliState(Int32 chnlIndex, Int32 acqUnitIndex, CaliItem caliItem)
        {
            return CaliStates[chnlIndex, acqUnitIndex, (int)caliItem];
        }

        #region flag相关

        public Boolean IsAllSucceed()
        {
            for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
            {
                for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
                {
                    foreach (CaliItem item in Enum.GetValues(typeof(CaliItem)))
                    {
                        if (GetCaliState(chnlId, acqUnitId, item).CurrFlag != CaliState.Flag.Succeed)
                            return false;
                    }
                }
            }
            return true;
        }

        public Boolean IsAllCompleted()
        {
            for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
            {
                if (!IsChnlCompleted(chnlId))
                    return false;
            }
            return true;
        }

        public Boolean IsChnlCompleted(Int32 chnlIndex)
        {
            for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
            {
                if (!IsAdcCompleted(chnlIndex, acqUnitId))
                    return false;
            }
            return true;
        }

        public Boolean IsChnlGainCompleted(Int32 chnlIndex)
        {
            for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
            {
                if (!GetCaliState(chnlIndex, acqUnitId, (int)CaliItem.Gain).IsItemCompleted())
                    return false;
            }
            return true;
        }

        public Boolean IsAdcCompleted(Int32 chnlIndex, Int32 acqUnitIndex)
        {
            foreach (CaliItem item in Enum.GetValues(typeof(CaliItem)))
            {
                if (!GetCaliState(chnlIndex, acqUnitIndex, item).IsItemCompleted())
                    return false;
            }
            return true;
        }

        #endregion flag相关
    }

    internal class DataManager
    {
        //List<ushort[]>为一次获取的数据,包含了Adc的采集数据
        private List<List<ushort[]>> Datas;
        //List<WaveOffsetGainPhase>为一次分析的结果数据,包含了Adc的结果数据
        private List<List<WaveOffsetGainPhase>> waveOffsetGainPhases = new List<List<WaveOffsetGainPhase>>();
        /// <summary>
        /// 说明：把32位bit的数值，转化为数组，数组的值为位的下标；
        /// 例如：0b01000011 => {1，2，7}；
        /// </summary>
        /// <param name="bitSet"></param>
        /// <returns></returns>
        public static Int32[] BitsToArray(Int32 bitSet)
        {
            List<Int32> result = new List<Int32>();
            for (int i = 0; i < 32; i++)
            {
                if ((bitSet & (1 << i)) != 0)
                    result.Add(i + 1);
            }
            return result.ToArray();
        }

        /// <summary>
        /// 中值过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="reserveNum"></param>
        /// <returns></returns>
        public static List<T> MiddleDataFilter<T>(List<T> data, int reserveNum)
            where T : IComparable<T>
        {
            List<T> objectList = new List<T>();
            data.Sort();

            int startIndex = (data.Count - reserveNum) / 2;
            for (int i = startIndex; i < startIndex + reserveNum; i++)
                objectList.Add(data[i]);

            //打印信息
            //StringBuilder msgSb = new StringBuilder();
            //msgSb.Append("srcData:");
            //data.ForEach(d => msgSb.Append(d.ToString() + ","));
            //msgSb.Append(";objectList:");
            //objectList.ForEach(d => msgSb.Append(d.ToString() + ","));
            //Logger.WriteLine($"MiddleDataFilter:{msgSb.ToString()};");

            return objectList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas">多次的adc核数据</param>
        /// <param name="sampleByMHz"></param>
        /// <param name="signalByMHz"></param>
        public DataManager(List<List<ushort[]>> datas, double sampleByMHz, double signalByMHz)
        {
            Datas = datas;
            foreach (var timeData in Datas)
            {
                List<WaveOffsetGainPhase> offsetGainPhases = new List<WaveOffsetGainPhase>();
                foreach (var coreData in timeData)
                    offsetGainPhases.Add(SineFitFunc.SineFit(coreData, sampleByMHz, signalByMHz));
                waveOffsetGainPhases.Add(offsetGainPhases);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currCoreID"></param>
        /// <param name="relativeCoreID"></param>
        /// <returns>Gain差异，比例</returns>
        public double GetAvgErrGain(int currCoreID, int relativeCoreID)
        {
            List<double> errs = new List<double>();
            foreach (var timeOGP in waveOffsetGainPhases)
                errs.Add((timeOGP[currCoreID].Gain - timeOGP[relativeCoreID].Gain) / timeOGP[relativeCoreID].Gain);
            return MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
        }

        /// <summary>
        /// 相对core0的相位差异
        /// </summary>
        /// <param name="currCoreID"></param>
        /// <param name="RelativeID"></param>
        /// <returns>Phase差异，弧度</returns>
        public double GetAvgErrPhase(int currCoreID, int RelativeID)
        {
            List<double> errs = new List<double>();
            foreach (var timeOGP in waveOffsetGainPhases)
            {
                double err = timeOGP[currCoreID].Phase - timeOGP[RelativeID].Phase;
                if (err < 0)
                    err += (Math.PI * 2 * 1000_000);
                errs.Add(err);
            }
            return MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
        }

        /// <summary>
        /// 相对core0的相位差异
        /// </summary>
        /// <param name="currCoreID"></param>
        /// <param name="RelativeID"></param>
        /// <returns>Offset偏置</returns>
        public double GetAvgErrOffset(int currCoreID, int RelativeID)
        {
            List<double> errs = new List<double>();
            foreach (var timeOGP in waveOffsetGainPhases)
            {
                double err = timeOGP[currCoreID].Offset - timeOGP[RelativeID].Offset;
                errs.Add(err);
            }
            return MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
        }
    }

}
