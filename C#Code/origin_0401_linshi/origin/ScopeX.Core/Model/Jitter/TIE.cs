using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.Util;
using ScopeX.ComModel;
using ScopeX.Core.Model.Jitter.Common;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public static class TIE
    {

        public static List<(Double TimeTick, Int32 TotalNum)> JitterHist = new List<(Double TimeTick, Int32 TotalNum)>();

        public static Double BinTick = 1;

        public static Boolean GetTIE(JitterParameter jitterData, JitterPrepare prepare, JitterResult result)
        {
            if (prepare.TIEData.Length < 1 || prepare.TIEData.Average == Double.NaN)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.SDADataCyclesTooFew);
                //WeakTip.Default.Write("Print", MsgTipId.SDADataCyclesTooFew);
                return false;
            }
            if (prepare.TIEData.Max - prepare.TIEData.Min > prepare.AverageUILength)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.TIEResultError);
                return false;
            }
            Double[] TIEResultList;
            Double[] tieafterinterp;
            Double[] tiewithoutddjafterinterp;
            if (jitterData.SignalType == SignalType.PRBSCode)
            {
                Double[] tiewithoutddj = GetDDJandTIEWithoutDDJNew(prepare, (Int32)jitterData.PatternLength, prepare.NonzeroIndex, out var ddj, out var dcd, out var isi, out tieafterinterp, out tiewithoutddjafterinterp);
                TIEResultList = tiewithoutddj;
                result.DDJ = ddj;
                result.DCD = dcd;
                result.ISI = isi;
            }
            else
            {
                TIEResultList = prepare.TIEData.TIEs;
                tieafterinterp = prepare.TIEData.TIEs;
                tiewithoutddjafterinterp = prepare.TIEData.TIEs;
                result.DDJ = Double.NaN;
                result.DCD = Double.NaN;
                result.ISI = Double.NaN;
            }
            result.TIE = prepare.TIEData.Max - prepare.TIEData.Min;
            Double interpratio = 1;
            Double[] tieresult = TIEResultList.SelectorArray(o => o / interpratio);
            if (tieresult.Length < 1)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.SDADataCyclesTooFew);
                return false;
            }
            SymmetryArray(tieresult);
            jitterData.TIEData = tieresult;
            jitterData.TIEDataAfterInterp = tieafterinterp;
            jitterData.TIEDataWithoutDDJAfterInterp = tiewithoutddjafterinterp;

            UpdateHist(prepare);

            return true;
        }
        /// <summary>
        /// 更新直方图数据
        /// </summary>
        /// <param name="jitterData"></param>
        private static void UpdateHist(JitterPrepare prepare)
        {
            for (Int32 i = 0; i < JitterHist.Count; i++)
            {
                if (JitterHist[i].TotalNum > Constants.JITTER_HIST_MAX_RECORD_NUMBER)
                {
                    return;
                }

            }
            InitHistPerTimes(prepare);
            Double maxtick = prepare.TIEData.Max / prepare.Fs;
            Double mintick = prepare.TIEData.Min / prepare.Fs;
            Double tickrange = maxtick - mintick;
            Int32 ticknumincurrentdata = (Int32)Math.Ceiling(tickrange / BinTick);
            var data = prepare.TIEData.TIEs.Select(o => o / prepare.Fs);
            //获取新的一组Hist
            var current_hist_vector = data.Hist(tickrange / ticknumincurrentdata).ToArray();

            if (current_hist_vector != null)
            {
                //从指定索引累加直方图数据
                var firstbinindex = JitterHist.FirstIndex(o => (o.TimeTick + BinTick) > mintick);
                if (firstbinindex != null)
                {
                    Double tick = 0;
                    Int32 totalnum = 0;
                    for (Int32 i = 0; i < current_hist_vector.Length; i++)
                    {
                        if ((Int32)firstbinindex + i < JitterHist.Count)
                        {
                            tick = JitterHist[(Int32)firstbinindex + i].TimeTick;//找到索引
                            totalnum = (Int32)current_hist_vector[i] + JitterHist[(Int32)firstbinindex + i].TotalNum;//累计计数
                            JitterHist[(Int32)firstbinindex + i] = (tick, totalnum);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 初始化或扩容
        /// </summary>
        /// <param name="jitterData"></param>
        private static void InitHistPerTimes(JitterPrepare prepare)
        {
            Double maxtick = prepare.TIEData.Max / prepare.Fs;
            Double mintick = prepare.TIEData.Min / prepare.Fs;
            Double tickrange = maxtick - mintick;
            if (JitterHist.Count == 0)
            {
                JitterHist = new();
                for (Int32 i = 0; i < Constants.DEFAULT_HIST_BIN_CNT; i++)
                {
                    BinTick = tickrange / Constants.DEFAULT_HIST_BIN_CNT;
                    JitterHist.Add((mintick + BinTick * i, 0));
                }
            }
            else
            {
                if (JitterHist.First().TimeTick > mintick)
                {
                    while (JitterHist.First().TimeTick > mintick)
                    {
                        JitterHist.Insert(0, (JitterHist.First().TimeTick - BinTick, 0));
                    }
                    Int32 minadditionaltick = JitterHist.Count / 20;
                    for (Int32 i = 0; i < minadditionaltick; i++)
                    {
                        JitterHist.Insert(0, (JitterHist.First().TimeTick - BinTick, 0));
                    }
                }
                if (JitterHist.Last().TimeTick < maxtick)
                {
                    while (JitterHist.Last().TimeTick < maxtick)
                    {
                        JitterHist.Add((JitterHist.Last().TimeTick + BinTick, 0));
                    }
                    Int32 maxadditionaltick = JitterHist.Count / 20;
                    for (Int32 i = 0; i < maxadditionaltick; i++)
                    {
                        JitterHist.Add((JitterHist.Last().TimeTick + BinTick, 0));
                    }
                }
            }
        }

        public static Vector GetTrend(JitterParameter jitterData, JitterPrepare prepareData)
        {
            var trenddata = jitterData.TIEDataAfterInterp.Select(o => (Double)o / jitterData.Fs * Constants.S_RELATIVE_TO_PS / 1000).ToMatrix(1, jitterData.TIEDataAfterInterp.Length);//ps
            return new Vector(trenddata,
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                1.0 / jitterData.Fs * prepareData.AverageUILength,
                Constants.DEF_XPOS_IDX);
        }

        public static Vector GetTIEHist(JitterParameter jitterData)
        {
            //var data = jitterData.TIEData.Select(o => o / jitterData.Fs);
            ////var histVector = jitterData.TIEData.Hist(jitterData.BinWidth);
            //var histVector= data.Hist(jitterData.BinWidth/ jitterData.Fs);
            ////Double biasByBinNum = (jitterData.TIEData.Max() + jitterData.TIEData.Min()) / jitterData.BinWidth;
            ////Double pixelsPerBin = 10000 / Math.Ceiling((jitterData.TIEData.Max() - jitterData.TIEData.Min()) / jitterData.BinWidth);
            ////Double rsp = 5000 - biasByBinNum * pixelsPerBin;

            Int32 maxbinnum = JitterHist.Count;
            switch (jitterData.UIBinNum)
            {
                case MaxBinNum.Num25:
                    maxbinnum = 25;
                    break;
                case MaxBinNum.Num50:
                    maxbinnum = 50;
                    break;
                case MaxBinNum.Num100:
                    maxbinnum = 100;
                    break;
                case MaxBinNum.Num250:
                    maxbinnum = 250;
                    break;
                case MaxBinNum.Num500:
                    maxbinnum = 500;
                    break;
                case MaxBinNum.Num2000:
                    maxbinnum = 2000;
                    break;
                case MaxBinNum.Max:
                    maxbinnum = JitterHist.Count;
                    break;
                default:
                    maxbinnum = JitterHist.Count;
                    break;
            }

            //目前结果数量略小于设置的bin如需导出数据，则需要修改
            List<(Double TimeTick, Int32 TotalNum)> hist = new();
            if (JitterHist.Count != maxbinnum)
            {
                Int32 decratio = (Int32)Math.Ceiling(JitterHist.Count / (Double)maxbinnum);
                for (Int32 i = 0; i < Math.Floor(JitterHist.Count / (Double)decratio); i++)
                {
                    Int32 count = 0;
                    for (Int32 j = 0; j < decratio; j++)
                    {
                        count += JitterHist[decratio * i + j].TotalNum;
                    }
                    hist.Add((JitterHist[decratio * i + (Int32)Math.Ceiling((Double)decratio / 2)].TimeTick, count));
                }
            }
            else
            {
                hist = JitterHist.Copy();
            }

            Double[] histvector = new Double[hist.Count];
            for (Int32 i = 0; i < hist.Count; i++)
            {
                histvector[i] = hist[i].TotalNum;
            }
            var min = hist.First();
            var max = hist.Last();
            //var min = hist.First(o => o.TotalNum != 0);
            //var max = hist.FindLast(o => o.TotalNum != 0);
            //var minIndex = hist.FirstIndex(o => o.TotalNum != 0);
            //var maxIndex = hist.FindLastIndex(o => o.TotalNum != 0);


            return new Vector(histvector.ToArray(),
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
                max.TimeTick,
                min.TimeTick);
        }

        /// <summary>
        /// 获取数据相关性抖动以及剥离出DDJ的TIE序列
        /// </summary>
        /// <param name="TIE">原始的TIE</param>
        /// <param name="cycleLength">一个周期的数据bit位数</param>
        /// <param name="nonzeroIndex">TIE非0的索引值</param>
        /// <param name="DDJ">数据相关性抖动</param>
        /// <param name="DCD">占空比失真</param>
        /// <param name="ISI">符号间干扰</param>
        /// <returns>剥离出DDJ的TIE序列</returns>
        public static Double[] GetDDJandTIEWithoutDDJ(Double[] TIE, Int32 cycleLength, Int32[] nonzeroIndex, out Double DDJ, out Double DCD, out Double ISI)
        {
            Int32 cycle = TIE.Length / (cycleLength * 2);
            Double[] averagetie = new Double[cycleLength * 2];
            Double[] tiewithoutddj = new Double[TIE.Length];
            List<Double> validtieonlyddj = new List<Double>();
            List<Double> postie = new List<Double>();
            List<Double> negtie = new List<Double>();
            List<Double> validtiewithoutddj = new List<Double>();
            for (Int32 i = 0; i < 2 * cycleLength; i++)
            {
                for (Int32 j = 0; j < cycle; j++)
                {
                    averagetie[i] += TIE[i + j * cycleLength * 2];
                }
                averagetie[i] /= cycle;
            }

            foreach (Int32 i in nonzeroIndex)
            {
                if (i < 2 * cycleLength)
                {
                    validtieonlyddj.Add(averagetie[i]);
                }
            }

            for (Int32 i = 0; i < validtieonlyddj.Count; i++)
            {
                if (i % 2 == 0)
                {
                    postie.Add(validtieonlyddj[i]);
                }
                else
                {
                    negtie.Add(validtieonlyddj[i]);
                }
            }

            DDJ = validtieonlyddj.Count != 0 ? validtieonlyddj.Max() - validtieonlyddj.Min() : 0;
            DCD = (postie.Count != 0 && negtie.Count != 0) ? postie.Average() - negtie.Average() : 0;
            ISI = (postie.Count != 0 && negtie.Count != 0) ? Math.Max(postie.Max() - postie.Min(), negtie.Max() - negtie.Min()) : 0;

            for (Int32 i = 0; i < tiewithoutddj.Length; i++)
            {
                tiewithoutddj[i] = TIE[i] - averagetie[i % (cycleLength * 2)];
            }

            foreach (Int32 i in nonzeroIndex)
            {
                if (i < tiewithoutddj.Length)
                {
                    validtiewithoutddj.Add(tiewithoutddj[i]);
                }
            }

            return validtiewithoutddj.ToArray();
        }
        public static Double[] GetDDJandTIEWithoutDDJNew(JitterPrepare prepare, Int32 cycleLength, Int32[] nonzeroIndex, out Double ddj, out Double dcd, out Double isi, out Double[] tieAfterInterp, out Double[] tieWithoutDDJAfterInterp)
        {
            Int32 cycle = prepare.TIEData.Length / cycleLength;
            Double[] averagetie = new Double[cycleLength];
            Double[] tiewithoutddj = new Double[prepare.TIEData.Length];
            List<Double> validtieonlyddj = new List<Double>();
            List<Double> postie = new List<Double>();
            List<Double> negtie = new List<Double>();
            List<Double> validtiewithoutddj = new List<Double>();
            List<Double> validtiewithddj = new List<Double>();
            for (Int32 i = 0; i < cycleLength; i++)
            {
                for (Int32 j = 0; j < cycle; j++)
                {
                    averagetie[i] += prepare.TIEData.TIEs[i + j * cycleLength];
                }
                averagetie[i] /= cycle;
            }

            foreach (Int32 i in nonzeroIndex.Take(cycleLength).ToArray())
            {
                if (i < cycleLength)
                {
                    validtieonlyddj.Add(averagetie[i]);
                }
            }

            for (Int32 i = 0; i < validtieonlyddj.Count; i++)
            {
                if (i % 2 == 0)
                {
                    postie.Add(validtieonlyddj[i]);
                }
                else
                {
                    negtie.Add(validtieonlyddj[i]);
                }
            }

            ddj = validtieonlyddj.Count != 0 ? validtieonlyddj.Max() - validtieonlyddj.Min() : 0;
            dcd = (postie.Count != 0 && negtie.Count != 0) ? postie.Average() - negtie.Average() : 0;
            isi = (postie.Count != 0 && negtie.Count != 0) ? Math.Max(postie.Max() - postie.Min(), negtie.Max() - negtie.Min()) : 0;

            for (Int32 i = 0; i < tiewithoutddj.Length; i++)
            {
                tiewithoutddj[i] = prepare.TIEData.TIEs[i] - averagetie[i % cycleLength];
            }

            foreach (Int32 i in nonzeroIndex)
            {
                if (i < tiewithoutddj.Length)
                {
                    validtiewithoutddj.Add(tiewithoutddj[i]);
                    validtiewithddj.Add(prepare.TIEData.TIEs[i]);
                }
            }
            tieAfterInterp = TIELinearInterpolation(prepare.TIEData.TIEs, nonzeroIndex);
            tieWithoutDDJAfterInterp = TIELinearInterpolation(tiewithoutddj, nonzeroIndex);
            //Double[] TIEWithZero = TIEAddZero(TIE, nonzeroIndex);
            //Double tmpMax = validTIEWithDDJ.Max();
            //Double tmpMin = validTIEWithDDJ.Min();

            return validtiewithddj.ToArray();
        }

        public static Double[] TIELinearInterpolation(Double[] tie, Int32[] nonzeroIndex)
        {
            Double[] interpolatedtie = new Double[tie.Length];
            for (Int32 i = 0; i < nonzeroIndex.Length - 1; i++)
            {
                for (Int32 j = nonzeroIndex[i]; j < nonzeroIndex[i + 1]; j++)
                {
                    Double intervals = nonzeroIndex[i + 1] - nonzeroIndex[i];
                    interpolatedtie[j] = tie[nonzeroIndex[i]] * (nonzeroIndex[i + 1] - j) / intervals + tie[nonzeroIndex[i + 1]] * (j - nonzeroIndex[i]) / intervals;
                }
            }
            interpolatedtie[^1] = tie[^1];
            return interpolatedtie;
        }

        public static Double[] TIEAddZero(Double[] tie, Int32[] nonzeroIndex)
        {
            Double[] rst = new Double[tie.Length];
            foreach (Int32 i in nonzeroIndex)
            {
                rst[i] = tie[i];
            }
            return rst;
        }

        public static void SymmetryArray(Double[] data)
        {
            if (data.Length < 1)
            {
                return;
            }
            Double tiemean = data.Average();
            for (Int32 i = 0; i < data.Length; i++)
            {
                data[i] -= tiemean;
            }
        }

        /// <summary>
        /// 最小二乘法拟合直线
        /// </summary>
        /// <param name="x">横坐标序列</param>
        /// <param name="y">纵坐标序列</param>
        /// <param name="fittingPointsNumber">拟合的点数</param>
        /// <returns>直线的斜率k和纵截距b</returns>
        public static (Double k, Double b) LinearFitting(Double[] x, Double[] y, Int32 fittingPointsNumber)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("The length of x must be equal to the length of y.");
            }

            if (x.Length < 2)
            {
                throw new ArgumentException("The length of x must be greater than 1.");
            }

            if (fittingPointsNumber > x.Length || fittingPointsNumber < 2)
            {
                throw new ArgumentException($"Parameter fittingPointsNumber must be less than or equal to the  length of x and be greater than 1.", nameof(fittingPointsNumber));
            }

            Double covarianceSum = 0, squareVarianceSum = 0;
            Double xAvg = x.Average(), yAvg = y.Average();

            for (Int32 i = 0; i < fittingPointsNumber; i++)
            {
                Double xDiff = x[i] - xAvg;
                Double yDiff = y[i] - yAvg;

                squareVarianceSum += xDiff * xDiff;
                covarianceSum += xDiff * yDiff;
            }

            if (Math.Abs(squareVarianceSum) < 1e-10)  // 防止除以零
            {
                return (Double.PositiveInfinity, 0);
            }

            Double k = covarianceSum / squareVarianceSum;  // 斜率
            Double b = yAvg - k * xAvg;  // 截距

            return (k, b);
        }



        /// <summary>
        /// 计算时钟信号的时间间隔误差
        /// </summary>
        /// <param name="actualEdge">插入虚拟边沿后的边沿位置序列</param>
        /// <param name="refEdge">参考信号的边沿位置序列</param>
        /// <returns>TIE序列</returns>
        public static (Double[] TIEs, Int32 Length, Double Max, Double Min, Double Average) CalculateClockTIE(Double[] actualEdge, Double[] refEdge)
        {
            Int32 length = Math.Min(actualEdge.Length, refEdge.Length);
            Double[] ties = new Double[length];
            Double sum = 0;
            Double average = Double.NaN;
            Double max = 0;
            Double min = 0;
            for (Int32 i = 0; i < length; i++)
            {
                ties[i] = actualEdge[i] - refEdge[i];
                if (ties[i] > max)
                {
                    max = ties[i];
                }
                if (ties[i] < min)
                {
                    min = ties[i];
                }
                sum += ties[i];
            }

            if (length != 0)
            {
                average = sum / length;
            }

            return (ties, length, max, min, average);
        }

        public static Boolean GetCyleToCycle(JitterPrepare data, JitterResult result)
        {
            Double[] cyc2cyc = GetCyleToCycles(data.ClockEdges);

            if (cyc2cyc.Length > 0)
            {
                result.CC = cyc2cyc.Max() - cyc2cyc.Min();
                return true;
            }
            return false;
        }


        public static Double[] GetCyleToCycles(Double[] actualEdge)
        {
            List<Double> cyc2cyc = new List<Double>();
            List<Double> cycles = new List<Double>();
            Int32 index = 0;
            if (actualEdge != null && actualEdge!.Length > 2)
            {
                for (Int32 i = 2, l = actualEdge.Length; i < l - 1; i += 2)
                {
                    cycles.Add(actualEdge[i + 1] - actualEdge[i - 2]);
                    index++;
                    if (index >= 2)
                    {
                        cyc2cyc.Add(cycles[index - 1] - cycles[index - 2]);
                    }
                }
            }

            return cyc2cyc.ToArray();
        }

        public static void Dispose()
        {
            JitterHist = new();
            BinTick = 1;
        }

    }
}
