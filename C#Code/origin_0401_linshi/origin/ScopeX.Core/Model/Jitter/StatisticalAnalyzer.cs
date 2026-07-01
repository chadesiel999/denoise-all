using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using ScopeX.ComModel;
using ScopeX.Core.Model.Jitter.Common;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public class StatisticalAnalyzer : IDisposable
    {
        public StatisticalAnalyzer()
        {
            MJSQ = new();
            NQScale = new();
        }

        private static readonly Object _Locker = new Object();
        public Histogram Hist;
        public Int32 Bins;
        public StatisticalConstructionMode ConstructionMode;
        public MJSQAnalyzer MJSQ { get; set; }

        public NQScaleAnalyzer NQScale { get; set; }
        public Histogram GetHist(JitterParameter jitterData)
        {
            lock (_Locker)
            {
                Histogram hist;
                switch (ConstructionMode)
                {
                    case StatisticalConstructionMode.Single:
                        hist = GetSingleHist(jitterData);
                        break;
                    case StatisticalConstructionMode.Accumulation:
                        hist = AccumulateHist(jitterData);
                        break;
                    default:
                        hist = GetSingleHist(jitterData);
                        break;
                }
                return hist;
            }
        }

        public Histogram GetSingleHist(JitterParameter jitterData)
        {
            return new Histogram(Bins, jitterData.TIEData);
        }

        public Histogram AccumulateHist(JitterParameter jitterData)
        {
            Histogram hist = new Histogram(Bins, jitterData.TIEData);
            hist.NumOfPoint += Hist.NumOfPoint;
            //
            return hist;
        }

        //public Vector GetHistogram(JitterData jitterData)
        //{
        //    jitterData.BinWidth = (jitterData.TIEData.Max() - jitterData.TIEData.Min()) / 100;
        //    var histVector = jitterData.TIEData.Hist(jitterData.BinWidth);
        //    Double biasByBinNum = (jitterData.TIEData.Max() + jitterData.TIEData.Min()) / jitterData.BinWidth;
        //    Double pixelsPerBin = 10000 / Math.Ceiling((jitterData.TIEData.Max() - jitterData.TIEData.Min()) / jitterData.BinWidth);
        //    Double rsp = 5000 - biasByBinNum * pixelsPerBin;
        //    //SDABuff.Default.Provide("SDAHist", new Vector(histVector, "ps"/*QuantityUnitExt.ToUnitString(QuantityUnit.Second)*/, QuantityUnitExt.ToUnitString(QuantityUnit.Constant), _BinWidth / fs * Constants.S_RELATIVE_TO_PS, rsp));
        //    return new Vector(histVector,
        //        "ps"/*QuantityUnitExt.ToUnitString(QuantityUnit.Second)*/,
        //        QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
        //        jitterData.BinWidth / jitterData.Fs * Constants.S_RELATIVE_TO_PS,
        //        rsp);
        //}

        /// <summary>
        /// 获取第一个局部峰值的索引值
        /// </summary>
        /// <param name="leftNum">局部峰值的左侧点数</param>
        /// <param name="rightNum">局部峰值的右侧点数</param>
        /// <returns></returns>
        public static Boolean GetLocalPeakIndex(Double[] dataArray, Int32 leftNum, Int32 rightNum, out Int32 index)
        {
            index = 0;
            Int32 windowWidth = leftNum + rightNum;
            if (dataArray.Length < windowWidth + 1)
                return false;

            List<Double> temp = new List<Double>();
            temp.AddRange(dataArray.Take(windowWidth));

            for (Int32 i = 0; i < dataArray.Length - windowWidth; i++)
            {
                temp.Add(dataArray[i + windowWidth]);
                if (temp[leftNum] == temp.Max())
                {
                    index = i + leftNum;
                    return true;
                }
                temp.RemoveAt(0);
            }
            return false;
        }

        /// <summary>
        /// TIE统计域分析处理
        /// </summary>
        /// <param name="TIE">TIE序列</param>
        /// <param name="TJ">总体抖动</param>
        /// <param name="DJ">确定性抖动</param>
        /// <param name="RJ">随机抖动</param>
        /// <param name="TJ_12">在误码率为1e-12水平下的总体抖动</param
        public static Boolean StatisticalProcessOld(JitterPrepare prepare, Double uiPoints, Double binWidth, out Double TJ, out Double DJ, out Double RJ, out Double TJ_12, out Double[,] bathWaveMatrix, out Double[,] QWaveMatrix)
        {

            Double[] leftFitFactor = new Double[3];
            Double[] rightFitFactor = new Double[3];
            TJ = 0;
            DJ = 0;
            RJ = 0;
            TJ_12 = 0;

            binWidth = 0.01;   //直方图bin的宽度，先默认为0.1个采样间隔，即0.1Ts

            //binWidth = (prepare.TIEData.Max - prepare.TIEData.Min) / 100;
            //binWidth=binWidth == 0?0.01:binWidth;
            Int32 leftHistBinNum = binWidth == 0 ? 0 : Math.Abs((Int32)(prepare.TIEData.Min / binWidth)) + 1;
            Int32 rightHistBinNum = binWidth == 0 ? 0 : Math.Abs((Int32)(prepare.TIEData.Max / binWidth)) + 1;
            Int32 leftTail = 0;
            Int32 rightTail = 0;


            #region Hist
            Double[] leftTIEHistDouble = prepare.TIEData.TIEs.Select(o => o < 0 ? -o : 0).Hist(binWidth).ToArray();
            Double[] rightTIEHistDouble = prepare.TIEData.TIEs.Select(o => o >= 0 ? o : 0).Hist(binWidth).ToArray();
            leftTIEHistDouble[0] -= prepare.TIEData.TIEs.SelectorSum(o => o >= 0 ? 1 : 0);
            rightTIEHistDouble[0] -= prepare.TIEData.TIEs.SelectorSum(o => o < 0 ? 1 : 0);

            if (leftTIEHistDouble.Length < 4 || rightTIEHistDouble.Length < 4)
            {
                //WeakTip.Default.Write("Print", MsgTipId.TIEResultError);
                TJ = DJ = RJ = TJ_12 = 0;
                QWaveMatrix = bathWaveMatrix = new Double[0, 0];
                return false;
            }


            //消除尾部断续点
            //if (leftHistBinNum * 0.05 > 5)
            //{              
            //    for (Int32 i = (Int32)(leftHistBinNum - 1 - leftHistBinNum * 0.1);  i < leftHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (leftTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            leftTIEHist.RemoveRange(i , leftTIEHist.Count - i);
            //            break;
            //        }                  
            //    }

            //}

            //if (rightHistBinNum * 0.05 > 5)
            //{
            //    for (Int32 i = (Int32)(rightHistBinNum - 1 - rightHistBinNum * 0.1); i < rightHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (rightTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            rightTIEHist.RemoveRange(i, rightTIEHist.Count - i);
            //            break;
            //        }
            //    }

            //}

            //消除过大值（可能删去最大值，暂时不取）

            //均值滤波：已删除        
            Int32 AveFilterCnt = (Int32)(Math.Min(leftTIEHistDouble.Length, rightTIEHistDouble.Length) / 5 + 0.5);//滤波系数
            AveFilterCnt = Math.Min(AveFilterCnt, 8);//当Count>40时,滤波系数为8

            if (AveFilterCnt == 0)
                AveFilterCnt = 1;

            Double totalValue = leftTIEHistDouble.Sum() + rightTIEHistDouble.Sum();
            #endregion

            #region PDF
            Double[] leftTIEPDF = leftTIEHistDouble.Select(o => o / totalValue).ToArray();
            Double[] rightTIEPDF = rightTIEHistDouble.Select(o => o / totalValue).ToArray();

            #endregion

            #region CDF 对数坐标
            Double[] leftTIECDFLinear = GetCDFByPDF(leftTIEPDF);
            Double[] leftTIECDFlog = leftTIECDFLinear.Select(o => System.Math.Log10(o)).ToArray();
            Double[] rightTIECDFLinear = GetCDFByPDF(rightTIEPDF);
            Double[] rightTIECDFlog = rightTIECDFLinear.Select(o => System.Math.Log10(o)).ToArray();

            #endregion

            //当抖动<0.3*采样周期，抖动过小（信号的幅度量化也可导致的噪声->抖动），粗略估计；          
            if (leftHistBinNum < 3 || rightHistBinNum < 3)
            {
                //总体抖动过小（TJ<Ts/2即TJ<0.1ns）             
                TJ = prepare.TIEData.Max - prepare.TIEData.Min;
                DJ = 0;
                RJ = TJ;

                rightFitFactor[0] = -0.05;
                rightFitFactor[1] = 0;
                rightFitFactor[2] = -0.3010;//lg(0.5)

                Buffer.BlockCopy(rightFitFactor, 0, leftFitFactor, 0, rightFitFactor.Length * sizeof(Double));
                Double[] BERtoTJ = GetTJArray(rightFitFactor);
                TJ_12 = BERtoTJ[12];
            }
            else
            {
                #region 求取尾部
                //尾部不必精准，但必须只包含RJ的部分，即范围可以小，但不能大
                //因为拟合是通过RJ的概率密度趋势来推导的
                Boolean leftFlag = GetLocalPeakIndex(leftTIEHistDouble, AveFilterCnt, AveFilterCnt, out leftTail);
                Boolean rightFlag = GetLocalPeakIndex(rightTIEHistDouble, AveFilterCnt, AveFilterCnt, out rightTail);

                //求取CDF的尾部，因为浴盆曲线实际上使CDF，所以这里不应该求Hist的尾部
                //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
                //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
                leftTail = Array.IndexOf(leftTIECDFlog, leftTIECDFlog.Max());
                rightTail = Array.IndexOf(rightTIECDFlog, rightTIECDFlog.Max());


                //当u1+u2<(sigma1+sigma2),左高斯对右尾部有影响，尾部减半   RightHistSize-rightTail=sigma1?  leftTail-LeftHistSize=sigma2?
                //尾部直接减半处理不太准确,删除
                //if (rightTail + leftTail < (leftHistBinNum + rightHistBinNum) / 2)
                //{
                //    rightTail = (Int32)((rightTail + rightHistBinNum) / 2 + 1);
                //    leftTail = (Int32)((leftTail + leftHistBinNum) / 2 + 1);
                //}
                #endregion

                if (rightHistBinNum - rightTail < 3 || leftHistBinNum - leftTail < 3)
                {
                    //随机抖动过小（RJ<0.1ns）???????????????????????????????         
                    TJ = prepare.TIEData.Max - prepare.TIEData.Min;
                    RJ = ((rightHistBinNum - rightTail) + (leftHistBinNum - leftTail)) / 2;
                    DJ = TJ - RJ;

                    leftFitFactor[0] = -0.05;
                    leftFitFactor[1] = -leftFitFactor[0] * 2 * (leftTail / 10);
                    leftFitFactor[2] = leftFitFactor[0] * (leftTail / 10) * (leftTail / 10) + leftTIECDFlog[leftTail];

                    rightFitFactor[0] = -0.05;
                    rightFitFactor[1] = -rightFitFactor[0] * 2 * (rightTail / 10);
                    rightFitFactor[2] = rightFitFactor[0] * (rightTail / 10) * (rightTail / 10) + rightTIECDFlog[rightTail];

                }
                else
                {
                    //尾部分别作一元二次拟合  即求二次曲线的系数
                    //左拟合（取leftTail~LeftHistSize）
                    if (leftTIECDFlog[leftHistBinNum - 1] > -15)  //误码率大于1e-15
                    {
                        Int32 leftLength = Math.Max(leftHistBinNum - leftTail, (Int32)(0.1 * leftHistBinNum));
                        Double[] leftX = new Double[leftLength];
                        Double[] leftY = new Double[leftLength];
                        for (Int32 i = 0; i < leftLength; i++)
                        {
                            //leftX[i] = -0.1 * (i + leftTail + 1);
                            leftX[i] = -binWidth * (i + leftHistBinNum - leftLength + 1);
                            leftY[i] = leftTIECDFlog[i + leftHistBinNum - leftLength];
                        }
                        //leftFitFactor = GetFitFactor(leftTIECDFlog,leftHistBinNum, leftTail);
                        leftFitFactor = SecondOrderFitting(leftX.Select(o => Math.Abs(o)).ToArray(), leftY, leftX.Length);
                    }

                    //右拟合（取RightTail~RightHistSize）
                    if (rightTIECDFlog[rightHistBinNum - 1] > -15)
                    {
                        Int32 rightLength = Math.Max(rightHistBinNum - rightTail, (Int32)(0.1 * rightHistBinNum));
                        Double[] rightX = new Double[rightLength];
                        Double[] rightY = new Double[rightLength];
                        for (Int32 i = 0; i < rightLength; i++)
                        {
                            //rightX[i] = 0.1 * (i + rightTail + 1);
                            rightX[i] = binWidth * (i + rightHistBinNum - rightLength + 1);
                            rightY[i] = rightTIECDFlog[i + rightHistBinNum - rightLength];
                        }
                        //rightFitFactor = GetFitFactor(rightTIECDFlog, rightHistBinNum, rightTail);
                        rightFitFactor = SecondOrderFitting(rightX, rightY, rightX.Length);
                    }
                }

                //拟合失败,二次曲线开口向上(尾部近似直线，补救措施)
                if (leftFitFactor[0] > 0)
                {
                    leftFitFactor[0] = -leftFitFactor[0];
                    leftFitFactor[1] = -leftFitFactor[0] * 2 * (leftTail / 10);
                    leftFitFactor[2] = leftFitFactor[0] * (leftTail / 10) * (leftTail / 10) + leftTIECDFlog[leftTail];
                }

                if (rightFitFactor[0] > 0)
                {
                    rightFitFactor[0] = -rightFitFactor[0];
                    rightFitFactor[1] = -rightFitFactor[0] * 2 * (rightTail / 10);
                    rightFitFactor[2] = rightFitFactor[0] * (rightTail / 10) * (rightTail / 10) + rightTIECDFlog[rightTail];
                }
            }

            BathWave(prepare, leftTIECDFlog, leftFitFactor, rightTIECDFlog, rightFitFactor, binWidth, out Double TJByBath, out Double TJ_12ByBath, out Double[] leftBathWave, out Double[] rightBathWave);
            bathWaveMatrix = CalcXYMatrix(leftBathWave, rightBathWave, uiPoints / 10, binWidth / 10);
            TJ = TJByBath;
            TJ_12 = TJ_12ByBath;
            //这里Q曲线的尾部需要用Hist来确定线性的部分，和浴盆曲线的尾部不一样，因此重新计算leftTail和rightTail
            //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
            leftTail = leftTIEHistDouble.MaxIndex();
            //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
            rightTail = rightTIEHistDouble.MaxIndex();
            QWave(leftTIECDFLinear, leftTail, rightTIECDFLinear, rightTail, binWidth, out Double DJByQ, out Double RJByQ, out List<Double> leftQWave, out List<Double> rightQWave);
            QWaveMatrix = CalcXYMatrix(leftQWave.Select(o => -o).ToArray(), rightQWave.Select(o => -o).ToArray(), uiPoints / 10, binWidth / 10);
            DJ = DJByQ;
            RJ = RJByQ;
            return true;
        }


        public static Boolean StatisticalProcess(JitterPrepare prepare, Double uiPoints, Double binWidth, out Double TJ, out Double DJ, out Double RJ, out Double TJ_12, out Double[,] bathWaveMatrix, out Double[,] QWaveMatrix)
        {

            Double[] leftFitFactor = new Double[3];
            Double[] rightFitFactor = new Double[3];
            TJ = Double.NaN;
            DJ = Double.NaN;
            RJ = Double.NaN;
            TJ_12 = Double.NaN;

            //binWidth = 0.01;   //直方图bin的宽度，先默认为0.1个采样间隔，即0.1Ts

            //binWidth = (prepare.TIEData.Max - prepare.TIEData.Min) / 100;
            //binWidth=binWidth == 0?0.01:binWidth;
            if (Math.Abs(prepare.TIEData.Max / binWidth) < 30 || Math.Abs(prepare.TIEData.Min / binWidth) < 30)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.HistBinNumberTooFew);
                //WeakTip.Default.Write("Print", MsgTipId.HistBinNumberTooFew);
                QWaveMatrix = new Double[0, 0];
                bathWaveMatrix = new Double[0, 0];
                return false;
            }

            Int32 leftHistBinNum = binWidth == 0 ? 0 : Math.Abs((Int32)(prepare.TIEData.Min / binWidth)) + 1;
            Int32 rightHistBinNum = binWidth == 0 ? 0 : Math.Abs((Int32)(prepare.TIEData.Max / binWidth)) + 1;
            Int32 leftTail = 0;
            Int32 rightTail = 0;


            #region Hist
            Double[] leftTIEHistDouble = prepare.TIEData.TIEs.Select(o => o < 0 ? -o : 0).Hist(binWidth).ToArray();
            Double[] rightTIEHistDouble = prepare.TIEData.TIEs.Select(o => o >= 0 ? o : 0).Hist(binWidth).ToArray();
            leftTIEHistDouble[0] -= prepare.TIEData.TIEs.SelectorSum(o => o >= 0 ? 1 : 0);
            rightTIEHistDouble[0] -= prepare.TIEData.TIEs.SelectorSum(o => o < 0 ? 1 : 0);

            leftHistBinNum = leftTIEHistDouble.Length;
            rightHistBinNum = rightTIEHistDouble.Length;


            List<Double> leftHist = leftTIEHistDouble.ToList();
            List<Double> rightHist = rightTIEHistDouble.ToList();

            if (leftHistBinNum > 30)
            {
                for (Int32 i = (Int32)(0.5 * leftHistBinNum); i < leftHistBinNum - 1; i++)//（从Hist的后半尾部处理,连5个0点即认为断续）
                {
                    if (leftHist.GetRange(i, 2).Sum() == 0)
                    {
                        leftHist.RemoveRange(i, leftHist.Count - i);
                        break;
                    }
                }
            }
            if (rightHistBinNum > 30)
            {
                for (Int32 i = (Int32)(0.5 * rightHistBinNum); i < rightHistBinNum - 1; i++)//（从Hist的后半尾部处理,连5个0点即认为断续）
                {
                    if (rightHist.RangeSum(i, 2) == 0)
                    {
                        rightHist.RemoveRange(i, rightHist.Count - i);
                        break;
                    }
                }
            }
            //移除直方图尾部连续的0，避免后续CDF中出现0值导致取log出现-inf
            for (Int32 i = 0; i < leftHist.Count - 1; i++)
            {
                if (leftHist.RangeSum(i, leftHist.Count - i) == 0)
                {
                    leftHist.RemoveRange(i, leftHist.Count - i);
                    break;
                }
                if (leftHist[i] == 0)
                    leftHist[i] = 1;
            }

            for (Int32 i = 0; i < rightHist.Count - 1; i++)
            {
                if (rightHist.RangeSum(i, rightHist.Count - i) == 0)
                {
                    rightHist.RemoveRange(i, rightHist.Count - i);
                    break;
                }
                if (rightHist[i] == 0)
                    rightHist[i] = 1;
            }


            leftTIEHistDouble = leftHist.ToArray();
            rightTIEHistDouble = rightHist.ToArray();
            leftHistBinNum = leftHist.Count;
            rightHistBinNum = rightHist.Count;

            if (leftTIEHistDouble.Length < 4 || rightTIEHistDouble.Length < 4)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.HistBinNumberTooFew);
                //WeakTip.Default.Write("Print", MsgTipId.HistBinNumberTooFew);
                TJ = DJ = RJ = TJ_12 = 0;
                QWaveMatrix = bathWaveMatrix = new Double[0, 0];
                return false;
            }

            //消除尾部断续点
            //if (leftHistBinNum * 0.05 > 5)
            //{              
            //    for (Int32 i = (Int32)(leftHistBinNum - 1 - leftHistBinNum * 0.1);  i < leftHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (leftTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            leftTIEHist.RemoveRange(i , leftTIEHist.Count - i);
            //            break;
            //        }                  
            //    }

            //}

            //if (rightHistBinNum * 0.05 > 5)
            //{
            //    for (Int32 i = (Int32)(rightHistBinNum - 1 - rightHistBinNum * 0.1); i < rightHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (rightTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            rightTIEHist.RemoveRange(i, rightTIEHist.Count - i);
            //            break;
            //        }
            //    }

            //}

            //消除过大值（可能删去最大值，暂时不取）

            //均值滤波：已删除        
            Int32 AveFilterCnt = (Int32)(Math.Min(leftTIEHistDouble.Length, rightTIEHistDouble.Length) / 5 + 0.5);//滤波系数
            AveFilterCnt = Math.Min(AveFilterCnt, 8);//当Count>40时,滤波系数为8

            if (AveFilterCnt == 0)
                AveFilterCnt = 1;

            Double totalValue = leftTIEHistDouble.Sum() + rightTIEHistDouble.Sum();
            #endregion

            #region PDF
            Double[] leftTIEPDF = leftTIEHistDouble.Select(o => o / totalValue).ToArray();
            Double[] rightTIEPDF = rightTIEHistDouble.Select(o => o / totalValue).ToArray();

            #endregion

            #region CDF 对数坐标
            Double[] leftTIECDFLinear = GetCDFByPDF(leftTIEPDF);
            Double[] leftTIECDFlog = leftTIECDFLinear.Select(o => System.Math.Log10(o)).ToArray();
            Double[] rightTIECDFLinear = GetCDFByPDF(rightTIEPDF);
            Double[] rightTIECDFlog = rightTIECDFLinear.Select(o => System.Math.Log10(o)).ToArray();

            #endregion

            //当抖动<0.3*采样周期，抖动过小（信号的幅度量化也可导致的噪声->抖动），粗略估计；          
            if (leftHistBinNum < 3 || rightHistBinNum < 3)
            {
                //总体抖动过小（TJ<Ts/2即TJ<0.1ns）             
                TJ = prepare.TIEData.Max - prepare.TIEData.Min;
                DJ = 0;
                RJ = TJ;

                rightFitFactor[0] = -0.05;
                rightFitFactor[1] = 0;
                rightFitFactor[2] = -0.3010;//lg(0.5)

                Buffer.BlockCopy(rightFitFactor, 0, leftFitFactor, 0, rightFitFactor.Length * sizeof(Double));
                Double[] BERtoTJ = GetTJArray(rightFitFactor);
                TJ_12 = BERtoTJ[12];
            }
            else
            {
                #region 求取尾部
                //尾部不必精准，但必须只包含RJ的部分，即范围可以小，但不能大
                //因为拟合是通过RJ的概率密度趋势来推导的
                Boolean leftFlag = GetLocalPeakIndex(leftTIEHistDouble, AveFilterCnt, AveFilterCnt, out leftTail);
                Boolean rightFlag = GetLocalPeakIndex(rightTIEHistDouble, AveFilterCnt, AveFilterCnt, out rightTail);

                //求取CDF的尾部，因为浴盆曲线实际上使CDF，所以这里不应该求Hist的尾部
                //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
                //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
                leftTail = leftTIECDFlog.MaxIndex();
                rightTail = rightTIECDFlog.MaxIndex();
                leftTail = (Int32)Math.Max(0.4 * leftTIECDFlog.Length, leftTail);
                rightTail = (Int32)Math.Max(0.4 * rightTIECDFlog.Length, rightTail);


                //当u1+u2<(sigma1+sigma2),左高斯对右尾部有影响，尾部减半   RightHistSize-rightTail=sigma1?  leftTail-LeftHistSize=sigma2?
                //尾部直接减半处理不太准确,删除
                //if (rightTail + leftTail < (leftHistBinNum + rightHistBinNum) / 2)
                //{
                //    rightTail = (Int32)((rightTail + rightHistBinNum) / 2 + 1);
                //    leftTail = (Int32)((leftTail + leftHistBinNum) / 2 + 1);
                //}
                #endregion

                if (rightHistBinNum - rightTail < 3 || leftHistBinNum - leftTail < 3)
                {
                    //随机抖动过小（RJ<0.1ns）???????????????????????????????         
                    TJ = prepare.TIEData.Max - prepare.TIEData.Min;
                    RJ = ((rightHistBinNum - rightTail) + (leftHistBinNum - leftTail)) / 2;
                    DJ = TJ - RJ;

                    leftFitFactor[0] = -0.05;
                    leftFitFactor[1] = -leftFitFactor[0] * 2 * (leftTail / 10);
                    leftFitFactor[2] = leftFitFactor[0] * (leftTail / 10) * (leftTail / 10) + leftTIECDFlog[leftTail];

                    rightFitFactor[0] = -0.05;
                    rightFitFactor[1] = -rightFitFactor[0] * 2 * (rightTail / 10);
                    rightFitFactor[2] = rightFitFactor[0] * (rightTail / 10) * (rightTail / 10) + rightTIECDFlog[rightTail];

                }
                else
                {

                    Int32 cnt = 0;
                    while ((leftFitFactor[0] >= 0 || rightFitFactor[0] >= 0) && cnt <= 9)
                    {
                        leftTail = (Int32)((0.9 - 0.1 * cnt) * leftTIECDFlog.Length);
                        rightTail = (Int32)((0.9 - 0.1 * cnt) * rightTIECDFlog.Length);
                        //尾部分别作一元二次拟合  即求二次曲线的系数
                        //左拟合（取leftTail~LeftHistSize）
                        if (leftTIECDFlog[leftHistBinNum - 1] > -15)  //误码率大于1e-15
                        {
                            Int32 leftLength = Math.Max(leftHistBinNum - leftTail, (Int32)(0.1 * leftHistBinNum));
                            Double[] leftX = new Double[leftLength];
                            Double[] leftY = new Double[leftLength];
                            for (Int32 i = 0; i < leftLength; i++)
                            {
                                //leftX[i] = -0.1 * (i + leftTail + 1);
                                leftX[i] = -binWidth * (i + leftHistBinNum - leftLength + 1);
                                leftY[i] = leftTIECDFlog[i + leftHistBinNum - leftLength];
                            }
                            //leftFitFactor = GetFitFactor(leftTIECDFlog,leftHistBinNum, leftTail);
                            leftFitFactor = SecondOrderFitting(leftX.Select(o => Math.Abs(o)).ToArray(), leftY, leftX.Length);
                        }

                        //右拟合（取RightTail~RightHistSize）
                        if (rightTIECDFlog[rightHistBinNum - 1] > -15)
                        {
                            Int32 rightLength = Math.Max(rightHistBinNum - rightTail, (Int32)(0.1 * rightHistBinNum));
                            Double[] rightX = new Double[rightLength];
                            Double[] rightY = new Double[rightLength];
                            for (Int32 i = 0; i < rightLength; i++)
                            {
                                //rightX[i] = 0.1 * (i + rightTail + 1);
                                rightX[i] = binWidth * (i + rightHistBinNum - rightLength + 1);
                                rightY[i] = rightTIECDFlog[i + rightHistBinNum - rightLength];
                            }
                            //rightFitFactor = GetFitFactor(rightTIECDFlog, rightHistBinNum, rightTail);
                            rightFitFactor = SecondOrderFitting(rightX, rightY, rightX.Length);
                        }

                        cnt++;
                    }

                    if (cnt == 10)
                    {
                        PrintBathWaveFitError();
                        QWaveMatrix = new Double[0, 0];
                        bathWaveMatrix = new Double[0, 0];
                        return false;
                    }
                }


                //拟合失败,二次曲线开口向上(尾部近似直线，补救措施)
                //if (leftFitFactor[0] > 0)
                //{
                //    leftFitFactor[0] = -leftFitFactor[0];
                //    leftFitFactor[1] = -leftFitFactor[0] * 2 * (leftTail / 10);
                //    leftFitFactor[2] = leftFitFactor[0] * (leftTail / 10) * (leftTail / 10) + leftTIECDFlog[leftTail];
                //}

                //if (rightFitFactor[0] > 0)
                //{
                //    rightFitFactor[0] = -rightFitFactor[0];
                //    rightFitFactor[1] = -rightFitFactor[0] * 2 * (rightTail / 10);
                //    rightFitFactor[2] = rightFitFactor[0] * (rightTail / 10) * (rightTail / 10) + rightTIECDFlog[rightTail];
                //}
            }

            BathWave(prepare, leftTIECDFlog, leftFitFactor, rightTIECDFlog, rightFitFactor, binWidth, out Double TJByBath, out Double TJ_12ByBath, out Double[] leftBathWave, out Double[] rightBathWave);
            bathWaveMatrix = CalcXYMatrix(leftBathWave, rightBathWave, uiPoints, binWidth);
            TJ = TJByBath;
            TJ_12 = TJ_12ByBath;
            //这里Q曲线的尾部需要用Hist来确定线性的部分，和浴盆曲线的尾部不一样，因此重新计算leftTail和rightTail
            //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
            leftTail = leftTIEHistDouble.MaxIndex();
            //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
            rightTail = rightTIEHistDouble.MaxIndex();
            QWave(leftTIECDFLinear, leftTail, rightTIECDFLinear, rightTail, binWidth, out Double DJByQ, out Double RJByQ, out List<Double> leftQWave, out List<Double> rightQWave);
            QWaveMatrix = CalcXYMatrix(leftQWave.Select(o => -o).ToArray(), rightQWave.Select(o => -o).ToArray(), uiPoints, binWidth);
            DJ = DJByQ;
            RJ = RJByQ;
            TJ_12 = DJByQ + RJByQ * 14.07;
            return true;
        }

        private static void PrintBathWaveFitError()
        {
            var bath = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Any(x =>
            {
                if (x is MathPrsnt math)
                {
                    if (math.Args.Occupier != null && math.Args.Occupier is JitterGraphModel jitter && jitter.Formula == Constants.JITTER_BATHTUB_FORMULA)
                    {
                        return true;
                    }
                }
                return false;
            });

            if (bath)
            {
                //if (tipsw == null || tipsw.ElapsedMilliseconds >= 10000)
                //{
                //    tipsw?.Stop(); // 如果 sw 不为 null，停止之前的 Stopwatch
                //    tipsw = Stopwatch.StartNew(); // 重新开始计时
                //}
                //else
                //{
                //    // 如果时间间隔小于 10秒，直接返回，不执行操作
                //    return;
                //}

                //WeakTip.Default.Write("Print", MsgTipId.BathWaveFitError);
                JitterCommon.LimitPrintJitterError(MsgTipId.BathWaveFitError);
            }
        }

        public static Boolean StatisticalProcessNew(List<(Double TimeTick, Int32 TotalNum)> jitterHist, Double fs, Double uiPoints, out Double tj, out Double dj, out Double rj, out Double tj_12, out Double[,] bathWaveMatrix, out Double[,] qWaveMatrix)
        {
            if (jitterHist.Count == 0)
            {
                tj = 0;
                dj = 0;
                rj = 0;
                tj_12 = 0;
                bathWaveMatrix = null;
                qWaveMatrix = null;
                return false;
            }

            Int32 max_bin_num = 500;
            List<(Double TimeTick, Int32 TotalNum)> hist = new();
            if (jitterHist.Count > max_bin_num)
            {
                Int32 dec_ratio = (Int32)Math.Ceiling(jitterHist.Count / (Double)max_bin_num);
                for (Int32 i = 0; i < Math.Floor(jitterHist.Count / (Double)dec_ratio); i++)
                {
                    Int32 count = 0;
                    for (Int32 j = 0; j < dec_ratio; j++)
                    {
                        count += jitterHist[dec_ratio * i + j].TotalNum;
                    }
                    hist.Add((jitterHist[dec_ratio * i + (Int32)Math.Ceiling((Double)dec_ratio / 2)].TimeTick * fs, count));
                }
            }
            else
            {
                hist = jitterHist.Copy();
                for (Int32 i = 0; i < hist.Count; i++)
                {
                    hist[i] = new(hist[i].TimeTick * fs, hist[i].TotalNum);
                }
            }


            Double bin_width = hist[1].TimeTick - hist[0].TimeTick;
            Double[] left_fit_factor = new Double[3];
            Double[] right_fit_factor = new Double[3];
            tj = Double.NaN;
            dj = Double.NaN;
            rj = Double.NaN;
            tj_12 = Double.NaN;
            Int32 posindex = 0;
            for (Int32 i = 0; i < hist.Count - 1; i++)
            {
                if (hist[i].TimeTick > 0)
                {
                    posindex = i;
                    break;
                }
            }


            if (hist.Count < 30)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.HistBinNumberTooFew);
                qWaveMatrix = new Double[0, 0];
                bathWaveMatrix = new Double[0, 0];
                return false;
            }

            Int32 left_hist_bin_num = posindex;
            Int32 right_hist_bin_num = hist.Count - left_hist_bin_num;
            Int32 left_tail = 0;
            Int32 right_tail = 0;


            #region Hist                   
            List<Double> left_tie_hist = hist.GetRange(0, left_hist_bin_num).Select(o => (Double)o.TotalNum).Reverse().ToList();
            List<Double> right_tie_hist = hist.GetRange(posindex, right_hist_bin_num).Select(o => (Double)o.TotalNum).ToList();
            left_hist_bin_num = left_tie_hist.Count;
            right_hist_bin_num = right_tie_hist.Count;

            //移除直方图首部和尾部连续的0，避免后续CDF中出现0值导致取log出现 - inf

            #region 移除前半部分为0的区域
            if (left_hist_bin_num > 30)
            {
                var zeroindex = left_tie_hist.FirstIndex(x => x == 0);
                var nonzeroindex = left_tie_hist.FirstIndex(x => x != 0);
                if (zeroindex != null && nonzeroindex != null && (nonzeroindex - zeroindex) >= 4)
                {
                    left_tie_hist.RemoveRange(zeroindex.Value, nonzeroindex.Value - zeroindex.Value + 1);
                }
            }
            if (right_hist_bin_num > 30)
            {
                var zeroindex = right_tie_hist.FirstIndex(x => x == 0);
                var nonzeroindex = right_tie_hist.FirstIndex(x => x != 0);
                if (zeroindex != null && nonzeroindex != null && (nonzeroindex - zeroindex) >= 4)
                {
                    right_tie_hist.RemoveRange(zeroindex.Value, nonzeroindex.Value - zeroindex.Value + 1);
                }
            }
            #endregion

            #region 移除后半部分为0的区域

            {
                left_tie_hist.Reverse();
                var zeroindex = left_tie_hist.FirstIndex(x => x == 0);
                var nonzeroindex = left_tie_hist.FirstIndex(x => x != 0);
                if (zeroindex != null && nonzeroindex != null && (nonzeroindex - zeroindex) >= 4)
                {
                    left_tie_hist.RemoveRange(zeroindex.Value, nonzeroindex.Value - zeroindex.Value + 1);
                }
                left_tie_hist.Reverse();
                for (var i = 0; i < left_tie_hist.Count; i++)
                {
                    if (left_tie_hist[i] == 0)
                    {
                        left_tie_hist[i] = 1;
                    }
                }
            }

            {
                right_tie_hist.Reverse();
                var zeroindex = right_tie_hist.FirstIndex(x => x == 0);
                var nonzeroindex = right_tie_hist.FirstIndex(x => x != 0);
                if (zeroindex != null && nonzeroindex != null && (nonzeroindex - zeroindex) >= 4)
                {
                    right_tie_hist.RemoveRange(zeroindex.Value, nonzeroindex.Value - zeroindex.Value + 1);
                }
                right_tie_hist.Reverse();
                for (var i = 0; i < right_tie_hist.Count; i++)
                {
                    if (right_tie_hist[i] == 0)
                    {
                        right_tie_hist[i] = 1;
                    }
                }
            }
            #endregion

            //移除断续点后的hist
            Double[] left_tie_hist_double = left_tie_hist.ToArray();
            Double[] right_tie_hist_double = right_tie_hist.ToArray();
            left_hist_bin_num = left_tie_hist_double.Length;
            right_hist_bin_num = right_tie_hist_double.Length;

            if (left_hist_bin_num < 15 || right_hist_bin_num < 15)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.HistBinNumberTooFew);
                tj = dj = rj = tj_12 = 0;
                qWaveMatrix = bathWaveMatrix = new Double[0, 0];
                return false;
            }

            //消除尾部断续点
            //if (leftHistBinNum * 0.05 > 5)
            //{              
            //    for (Int32 i = (Int32)(leftHistBinNum - 1 - leftHistBinNum * 0.1);  i < leftHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (leftTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            leftTIEHist.RemoveRange(i , leftTIEHist.Count - i);
            //            break;
            //        }                  
            //    }

            //}

            //if (rightHistBinNum * 0.05 > 5)
            //{
            //    for (Int32 i = (Int32)(rightHistBinNum - 1 - rightHistBinNum * 0.1); i < rightHistBinNum - 4; i++)//（只取10%处理,连5个0点即认为断续）
            //    {
            //        if (rightTIEHist.GetRange(i, 5).Sum() == 0)
            //        {
            //            rightTIEHist.RemoveRange(i, rightTIEHist.Count - i);
            //            break;
            //        }
            //    }

            //}

            //消除过大值（可能删去最大值，暂时不取）

            //均值滤波：已删除        
            Int32 ave_filter_cnt = (Int32)(Math.Min(left_tie_hist_double.Length, right_tie_hist_double.Length) / 5 + 0.5);//滤波系数
            ave_filter_cnt = Math.Min(ave_filter_cnt, 8);//当Count>40时,滤波系数为8

            if (ave_filter_cnt == 0)
                ave_filter_cnt = 1;

            Double total_value = left_tie_hist_double.Sum() + right_tie_hist_double.Sum();
            #endregion

            #region PDF
            Double[] left_tie_pdf = left_tie_hist_double.SelectorArray(o => o / total_value);
            Double[] right_tie_pdf = right_tie_hist_double.SelectorArray(o => o / total_value);

            #endregion

            #region CDF 对数坐标
            Double[] left_tie_cdf_linear = GetCDFByPDF(left_tie_pdf);
            Double[] left_tie_cdf_log = left_tie_cdf_linear.SelectorArray(o => System.Math.Log10(o));
            Double[] right_tie_cdf_linear = GetCDFByPDF(right_tie_pdf);
            Double[] right_tie_cdf_log = right_tie_cdf_linear.SelectorArray(o => System.Math.Log10(o));
            #endregion


            #region 求取尾部
            //尾部不必精准，但必须只包含RJ的部分，即范围可以小，但不能大
            //因为拟合是通过RJ的概率密度趋势来推导的
            Boolean left_flag = GetLocalPeakIndex(left_tie_hist_double, ave_filter_cnt, ave_filter_cnt, out left_tail);
            Boolean right_flag = GetLocalPeakIndex(right_tie_hist_double, ave_filter_cnt, ave_filter_cnt, out right_tail);

            //求取CDF的尾部，因为浴盆曲线实际上是CDF，所以这里不应该求Hist的尾部
            //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
            //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
            left_tail = left_tie_cdf_log.MaxIndex();
            right_tail = right_tie_cdf_log.MaxIndex();

            left_tail = (Int32)Math.Max(0.4 * left_tie_cdf_log.Length, left_tail);
            right_tail = (Int32)Math.Max(0.4 * right_tie_cdf_log.Length, right_tail);


            //当u1+u2<(sigma1+sigma2),左高斯对右尾部有影响，尾部减半   RightHistSize-rightTail=sigma1?  leftTail-LeftHistSize=sigma2?
            //尾部直接减半处理不太准确,删除
            //if (rightTail + leftTail < (leftHistBinNum + rightHistBinNum) / 2)
            //{
            //    rightTail = (Int32)((rightTail + rightHistBinNum) / 2 + 1);
            //    leftTail = (Int32)((leftTail + leftHistBinNum) / 2 + 1);
            //}
            #endregion

            if (right_hist_bin_num - right_tail < 3 || left_hist_bin_num - left_tail < 3)
            {
                //随机抖动过小（RJ<0.1ns）???????????????????????????????         
                tj = (left_hist_bin_num + right_hist_bin_num) * bin_width;
                rj = ((right_hist_bin_num - right_tail) + (left_hist_bin_num - left_tail)) / 2;
                dj = tj - rj;

                left_fit_factor[0] = -0.05;
                left_fit_factor[1] = -left_fit_factor[0] * 2 * (left_tail / 10);
                left_fit_factor[2] = left_fit_factor[0] * (left_tail / 10) * (left_tail / 10) + left_tie_cdf_log[left_tail];

                right_fit_factor[0] = -0.05;
                right_fit_factor[1] = -right_fit_factor[0] * 2 * (right_tail / 10);
                right_fit_factor[2] = right_fit_factor[0] * (right_tail / 10) * (right_tail / 10) + right_tie_cdf_log[right_tail];

            }
            else
            {
                Int32 cnt = 0;
                while ((left_fit_factor[0] >= 0 || right_fit_factor[0] >= 0) && cnt <= 9)
                {
                    left_tail = (Int32)((0.9 - 0.1 * cnt) * left_tie_cdf_log.Length);
                    right_tail = (Int32)((0.9 - 0.1 * cnt) * right_tie_cdf_log.Length);
                    //尾部分别作一元二次拟合  即求二次曲线的系数
                    //左拟合（取leftTail~LeftHistSize）
                    if (left_tie_cdf_log[left_hist_bin_num - 1] > -15)  //误码率大于1e-15
                    {
                        Int32 leftlength = Math.Max(left_hist_bin_num - left_tail, (Int32)(0.1 * left_hist_bin_num));
                        Double[] leftx = new Double[leftlength];
                        Double[] lefty = new Double[leftlength];
                        for (Int32 i = 0; i < leftlength; i++)
                        {
                            //leftX[i] = -0.1 * (i + leftTail + 1);
                            //leftX[i] = -binWidth * (i + leftHistBinNum - leftLength + 1);
                            leftx[i] = hist[posindex - 1 - (i + left_hist_bin_num - leftlength)].TimeTick;
                            lefty[i] = left_tie_cdf_log[i + left_hist_bin_num - leftlength];
                        }
                        //leftFitFactor = GetFitFactor(leftTIECDFlog,leftHistBinNum, leftTail);
                        left_fit_factor = SecondOrderFitting(leftx.SelectorArray(o => Math.Abs(o)), lefty, leftx.Length);
                    }

                    //右拟合（取RightTail~RightHistSize）
                    if (right_tie_cdf_log[right_hist_bin_num - 1] > -15)
                    {
                        Int32 rightlength = Math.Max(right_hist_bin_num - right_tail, (Int32)(0.1 * right_hist_bin_num));
                        Double[] rightx = new Double[rightlength];
                        Double[] righty = new Double[rightlength];
                        for (Int32 i = 0; i < rightlength; i++)
                        {
                            //rightX[i] = 0.1 * (i + rightTail + 1);
                            //rightX[i] = binWidth * (i + rightHistBinNum - rightLength + 1);
                            if (posindex + (i + left_hist_bin_num - rightlength) >= hist.Count || i + right_hist_bin_num - rightlength >= right_tie_cdf_log.Length)
                            {
                                break;
                            }
                            else
                            {
                                rightx[i] = hist[posindex + (i + left_hist_bin_num - rightlength)].TimeTick;
                                righty[i] = right_tie_cdf_log[i + right_hist_bin_num - rightlength];
                            }
                        }
                        //rightFitFactor = GetFitFactor(rightTIECDFlog, rightHistBinNum, rightTail);
                        right_fit_factor = SecondOrderFitting(rightx, righty, rightx.Length);
                    }

                    cnt++;
                }

                if (cnt == 10)
                {
                    PrintBathWaveFitError();
                    qWaveMatrix = new Double[0, 0];
                    bathWaveMatrix = new Double[0, 0];
                    return false;
                }
            }

            BathWaveNew(left_tie_cdf_log, left_fit_factor, right_tie_cdf_log, right_fit_factor, bin_width, out Double TJ_12ByBath, out Double[] leftBathWave, out Double[] rightBathWave);
            bathWaveMatrix = CalcXYMatrix(leftBathWave, rightBathWave, uiPoints, bin_width);
            tj = (left_hist_bin_num + right_hist_bin_num) * bin_width;
            tj_12 = TJ_12ByBath;
            //这里Q曲线的尾部需要用Hist来确定线性的部分，和浴盆曲线的尾部不一样，因此重新计算leftTail和rightTail
            //leftTail = Array.IndexOf(leftTIEHistDouble, leftTIEHistDouble.Max());
            left_tail = left_tie_hist_double.MaxIndex();
            //rightTail = Array.IndexOf(rightTIEHistDouble, rightTIEHistDouble.Max());
            right_tail = right_tie_hist_double.MaxIndex();
            QWave(left_tie_cdf_linear, left_tail, right_tie_cdf_linear, right_tail, bin_width, out Double DJByQ, out Double RJByQ, out List<Double> leftQWave, out List<Double> rightQWave);
            qWaveMatrix = CalcXYMatrix(leftQWave.SelectorArray(o => -o), rightQWave.SelectorArray(o => -o), uiPoints, bin_width);
            dj = DJByQ;
            rj = RJByQ;
            //TJ_12 = DJByQ + RJByQ * 14.07;
            return true;
        }




        private static Double[] GetTJArray(Double[] fitFactor)
        {
            Double[] ber_to_tj = new Double[17];
            Double ff = fitFactor[1] * fitFactor[1];
            Double f4 = 4 * fitFactor[0];
            for (Int32 i = 1, l = ber_to_tj.Count(); i < l; ++i)
                ber_to_tj[i] = Math.Abs(Math.Sqrt(ff - f4 * (fitFactor[2] + i)) / fitFactor[0]);
            return ber_to_tj;
        }
        /// <summary>
        /// 浴盆曲线
        /// </summary>
        /// <param name="TIE">TIE序列</param>
        /// <param name="leftTIECDFlog">左支CDF每个bin的值</param>
        /// <param name="leftFitFactor">左支二次曲线拟合系数</param>
        /// <param name="rightTIECDFlog">右支CDF每个bin的值</param>
        /// <param name="rightFitFactor">右支二次曲线拟合系数</param>
        /// <param name="TJ">总体抖动</param>
        /// <param name="TJ_12">误码率为1E-12水平下的总体抖动</param>
        public static void BathWave(JitterPrepare prepare, Double[] leftTIECDFlog, Double[] leftFitFactor, Double[] rightTIECDFlog, Double[] rightFitFactor, Double binWidth, out Double TJ, out Double TJ_12, out Double[] leftBathPieceInsrt, out Double[] rightBathPieceInsrt)
        {
            Double[] ber_to_tj = new Double[17];
            Double[] ber_to_tjr = new Double[17];
            Double[] ber_to_tjl = new Double[17];
            //外插
            leftBathPieceInsrt = GetPieceInsrt(leftTIECDFlog, leftTIECDFlog.Length, leftFitFactor, binWidth).ToArray();
            rightBathPieceInsrt = GetPieceInsrt(rightTIECDFlog, rightTIECDFlog.Length, rightFitFactor, binWidth).ToArray();

            //统计
            //解二次方程求根
            ber_to_tj[0] = 0;
            Int32 ir = 0;
            Int32 il = 0;
            Int32 iMin = (Int32)(Math.Min(rightTIECDFlog.Min(), leftTIECDFlog.Min()) - 0.5);
            for (Int32 i = -1; i > -17; --i)
            {
                if (i > iMin || leftFitFactor[1] * leftFitFactor[1] - 4 * leftFitFactor[0] * (leftFitFactor[2] - i) <= 0)
                {
                    while (ir < rightBathPieceInsrt.Length)
                    {
                        if (rightBathPieceInsrt[ir] < i)  //在添加了外插数据的CDF中找到第一个小于指定误码率的横坐标
                            break;
                        ir++;
                    }
                    while (il < leftBathPieceInsrt.Length)
                    {
                        if (leftBathPieceInsrt[il] < i)
                            break;
                        il++;
                    }
                    //在BER还未小到需要外插时，使用原始CDF计算抖动，因为数据量够所以直接用原始数据而不是拟合数据，结果更加准确
                    ber_to_tjr[-i] = (Double)ir * binWidth;
                    ber_to_tjl[-i] = (Double)il * binWidth;
                    ber_to_tj[-i] = ber_to_tjr[-i] + ber_to_tjl[-i];
                }
                else
                {   //使用外插数据解二次方程的根来估计总体抖动，这里不用再乘以binWidth，因为二次曲线拟合的时候横坐标已经包含的binwidth的信息
                    ber_to_tjl[-i] = (-leftFitFactor[1] - Math.Sqrt(leftFitFactor[1] * leftFitFactor[1] - 4 * leftFitFactor[0] * (leftFitFactor[2] - i))) / (2 * leftFitFactor[0]);   //取较大的那个根，因此-根号下4ac，较小根可能为负数
                    ber_to_tjr[-i] = (-rightFitFactor[1] - Math.Sqrt(rightFitFactor[1] * rightFitFactor[1] - 4 * rightFitFactor[0] * (rightFitFactor[2] - i))) / (2 * rightFitFactor[0]);
                    ber_to_tj[-i] = ber_to_tjl[-i] + ber_to_tjr[-i];
                }
            }

            TJ_12 = ber_to_tj[12];
            TJ = prepare.TIEData.Max - prepare.TIEData.Min;

        }

        public static void BathWaveNew(Double[] leftTIECDFlog, Double[] leftFitFactor, Double[] rightTIECDFlog, Double[] rightFitFactor, Double binWidth, out Double TJ_12, out Double[] leftBathPieceInsrt, out Double[] rightBathPieceInsrt)
        {
            Double[] BERtoTJ = new Double[17];
            Double[] BERtoTJR = new Double[17];
            Double[] BERtoTJL = new Double[17];
            //外插
            leftBathPieceInsrt = GetPieceInsrt(leftTIECDFlog, leftTIECDFlog.Length, leftFitFactor, binWidth).ToArray();
            rightBathPieceInsrt = GetPieceInsrt(rightTIECDFlog, rightTIECDFlog.Length, rightFitFactor, binWidth).ToArray();

            //统计
            //解二次方程求根
            BERtoTJ[0] = 0;
            Int32 ir = 0;
            Int32 il = 0;
            Int32 iMin = (Int32)(Math.Min(rightTIECDFlog.Min(), leftTIECDFlog.Min()) - 0.5);
            for (Int32 i = -1; i > -17; --i)
            {
                if (i > iMin || leftFitFactor[1] * leftFitFactor[1] - 4 * leftFitFactor[0] * (leftFitFactor[2] - i) <= 0)
                {
                    while (ir < rightBathPieceInsrt.Length)
                    {
                        if (rightBathPieceInsrt[ir] < i)  //在添加了外插数据的CDF中找到第一个小于指定误码率的横坐标
                            break;
                        ir++;
                    }
                    while (il < leftBathPieceInsrt.Length)
                    {
                        if (leftBathPieceInsrt[il] < i)
                            break;
                        il++;
                    }
                    //在BER还未小到需要外插时，使用原始CDF计算抖动，因为数据量够所以直接用原始数据而不是拟合数据，结果更加准确
                    BERtoTJR[-i] = (Double)ir * binWidth;
                    BERtoTJL[-i] = (Double)il * binWidth;
                    BERtoTJ[-i] = BERtoTJR[-i] + BERtoTJL[-i];
                }
                else
                {   //使用外插数据解二次方程的根来估计总体抖动，这里不用再乘以binWidth，因为二次曲线拟合的时候横坐标已经包含的binwidth的信息
                    BERtoTJL[-i] = (-leftFitFactor[1] - Math.Sqrt(leftFitFactor[1] * leftFitFactor[1] - 4 * leftFitFactor[0] * (leftFitFactor[2] - i))) / (2 * leftFitFactor[0]);   //取较大的那个根，因此-根号下4ac，较小根可能为负数
                    BERtoTJR[-i] = (-rightFitFactor[1] - Math.Sqrt(rightFitFactor[1] * rightFitFactor[1] - 4 * rightFitFactor[0] * (rightFitFactor[2] - i))) / (2 * rightFitFactor[0]);
                    BERtoTJ[-i] = BERtoTJL[-i] + BERtoTJR[-i];
                }
            }

            TJ_12 = BERtoTJ[12];
        }

        /// <summary>
        /// Q曲线
        /// </summary>
        /// <param name="leftTIECDFLinear">左支CDF的值（线性）</param>
        /// <param name="leftTail">左支的尾部</param>
        /// <param name="rightTIECDFLinear">右支CDF的值（线性）</param>
        /// <param name="rightTail">右支的尾部</param>
        /// <param name="DJ">确定性抖动</param>
        /// <param name="RJ">随机抖动</param>
        private static void QWave(Double[] leftTIECDFLinear, Int32 leftTail, Double[] rightTIECDFLinear, Int32 rightTail, Double binWidth, out Double DJ, out Double RJ, out List<Double> leftQPieceInsrt, out List<Double> rightQPieceInsrt)
        {
            //Double[] alpha = new Double[25];
            //for(Int32 i = 0; i < alpha.Length; i++)
            //{
            //    alpha[i] = QFunction(Math.Pow(10,-i),1e-50);
            //}
            Double[] alpha = { 0, 1.2816, 2.3263, 3.0902, 3.7190, 4.2649, 4.7534, 5.1993, 5.6120, 5.9978, 6.3613, 6.7060, 7.0345, 7.3488, 7.6506, 7.9413, 8.2221, 8.4938, 8.7573, 9.0133, 9.2623, 9.5050, 9.7418, 9.9730, 10.1992 };

            //用于拟合直线的点数
            Int32 leftFittingPointsNumber = Math.Min(leftTIECDFLinear.Length - leftTail, (Int32)(0.3 * leftTIECDFLinear.Length));
            Int32 rightFittingPointsNumber = Math.Min(rightTIECDFLinear.Length - rightTail, (Int32)(0.3 * rightTIECDFLinear.Length));

            //由CDF得到所有的Q的值
            //Double[] leftX = Enumerable.Range(0, leftTIECDFLinear.Length).Select(o => -0.1 * (o + 1)).ToArray();
            //Double[] leftX = Enumerable.Range(0, leftTIECDFLinear.Length).Select(o => -binWidth * (o + 1)).ToArray();
            Double[] leftX = leftTIECDFLinear.RangeSelectorArray(o => -binWidth * (o + 1));
            Double leftEps = leftTIECDFLinear.Min() / 50;
            Double[] leftQ = leftTIECDFLinear.SelectorArray(o => QFunction(o, leftEps));
            //Double[] rightX = Enumerable.Range(0, rightTIECDFLinear.Length).Select(o => 0.1 * (o + 1)).ToArray();
            Double rightEps = rightTIECDFLinear.Min() / 50;
            //Double[] rightX = Enumerable.Range(0, rightTIECDFLinear.Length).Select(o => binWidth * (o + 1)).ToArray();
            Double[] rightX = rightTIECDFLinear.RangeSelectorArray(o => binWidth * (o + 1));
            Double[] rightQ = rightTIECDFLinear.SelectorArray(o => QFunction(o, rightEps));

            //用于线性拟合的点的值
            Double[] leftX_fit = leftX.SkipArray(leftX.Length - leftFittingPointsNumber);
            Double[] leftQ_fit = leftQ.SkipArray(leftQ.Length - leftFittingPointsNumber);
            Double[] rightX_fit = rightX.SkipArray(rightX.Length - rightFittingPointsNumber);
            Double[] rightQ_fit = rightQ.SkipArray(rightQ.Length - rightFittingPointsNumber);

            (Double k, Double b) leftQFitFactor = LinearFitting(leftX_fit, leftQ_fit, leftFittingPointsNumber);
            (Double k, Double b) rightQFitFactor = LinearFitting(rightX_fit, rightQ_fit, rightFittingPointsNumber);


            DJ = Math.Abs(rightQFitFactor.b / rightQFitFactor.k) + Math.Abs(leftQFitFactor.b / leftQFitFactor.k);

            RJ = (Math.Abs(1.0 / rightQFitFactor.k) + Math.Abs(1.0 / leftQFitFactor.k)) / 2;

            //根据拟合直线推导至 精度为0.1Ts，Y范围为0~7.651，的区间 
            rightQPieceInsrt = new List<Double>();
            leftQPieceInsrt = new List<Double>();
            rightQPieceInsrt.AddRange(rightQ);
            leftQPieceInsrt.AddRange(leftQ);

            Double insertFit = 0;//BER   UILen
            Double xi = rightX[rightX.Length - 1];//jitter
            Double cnt = 0;
            while (insertFit < alpha[24])
            {
                try
                {
                    //xi = xi + 0.1;
                    xi = xi + binWidth;
                    insertFit = rightQFitFactor.k * (xi) + rightQFitFactor.b;
                    if (insertFit > 0)
                    {
                        rightQPieceInsrt.Add(insertFit);
                        cnt++;
                    }
                    if (cnt > 5000)
                    {
                        //WeakTip.Default.Write("Print", MsgTipId.QWaveFitError);
                        break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            insertFit = 0;//BER   UILen
            xi = leftX[leftX.Length - 1];//jitter
            cnt = 0;
            while (insertFit < alpha[24])
            {
                //xi = xi - 0.1;
                xi = xi - binWidth;
                insertFit = leftQFitFactor.k * (xi) + leftQFitFactor.b;
                if (insertFit > 0)
                {
                    leftQPieceInsrt.Add(insertFit);
                    cnt++;
                }
                if (cnt > 5000)
                {
                    //WeakTip.Default.Write("Print", MsgTipId.QWaveFitError);
                    break;
                }

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

            Double covariancesSum = 0, squareVariancesSum = 0;
            Double xAvg = x.Average(), yAvg = y.Average();
            for (Int32 i = 0; i < fittingPointsNumber; i++)
            {
                squareVariancesSum += (x[i] - xAvg) * (x[i] - xAvg);
                covariancesSum += (x[i] - xAvg) * (y[i] - yAvg);
            }

            if (squareVariancesSum == 0)
            {
                return (Double.PositiveInfinity, 0);
            }

            Double k = covariancesSum / squareVariancesSum;
            Double b = yAvg - k * xAvg;

            return (k, b);
        }

        /// <summary>
        /// 将二次曲线外插
        /// </summary>
        /// <param name="CDFlog">CDF的对数坐标值</param>
        /// <param name="histSize">bin的个数</param>
        /// <param name="fitFactor">二次曲线的系数，依次为二次项系数、一次项系数、常数项</param>
        /// <returns>外插后的序列值（包含原序列）</returns>
        public static List<Double> GetPieceInsrt(Double[] CDFlog, Int32 histSize, Double[] fitFactor, Double binWidth)
        {
            List<Double> pieceInsrt = new List<Double>();
            pieceInsrt.AddRange(CDFlog);//原始浴盆曲线数据
            //
            //两条线之间插一个点，
            //
            Double insertFit = 0;//BER   UILen
            //Double xi = (Double)(histSize - 1) / 10 + 0.1;//jitter 除10是因为拟合的方法中横坐标除以了10
            Double xi = (Double)(histSize - 1) / (1 / binWidth) + binWidth;
            Double cnt = 0;
            while (insertFit > -16)
            {
                //xi = xi + 0.1;     //步进0.1往更小的误码率情况下外插
                xi = xi + binWidth;
                insertFit = fitFactor[0] * (xi) * (xi) + fitFactor[1] * (xi) + fitFactor[2];     //二次曲线外插
                pieceInsrt.Add(insertFit);//添加外推的二次曲线值
                cnt++;
                if (cnt > 5000)
                {
                    //WeakTip.Default.Write("Print", MsgTipId.BathWaveFitError);
                    break;
                }
            }

            return pieceInsrt;
        }

        /// <summary>
        /// 求三阶方阵的逆矩阵
        /// </summary>
        /// <param name="thirdOrderSquare">输入的三阶方阵</param>
        /// <returns>输入矩阵的逆矩阵</returns>
        public static Double[,] GetInverseMatrix(Double[,] thirdOrderSquare)
        {
            //求行列式
            Double detA = thirdOrderSquare[0, 0] * (thirdOrderSquare[1, 1] * thirdOrderSquare[2, 2] - thirdOrderSquare[2, 1] * thirdOrderSquare[1, 2])
                - thirdOrderSquare[1, 0] * (thirdOrderSquare[0, 1] * thirdOrderSquare[2, 2] - thirdOrderSquare[2, 1] * thirdOrderSquare[0, 2])
                + thirdOrderSquare[2, 0] * (thirdOrderSquare[0, 1] * thirdOrderSquare[1, 2] - thirdOrderSquare[1, 1] * thirdOrderSquare[0, 2]);

            Double[,] inverseMatrix = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            //求代数余子式并转置得到伴随矩阵
            inverseMatrix[0, 0] = (thirdOrderSquare[1, 1] * thirdOrderSquare[2, 2] - thirdOrderSquare[1, 2] * thirdOrderSquare[2, 1]) / detA;
            inverseMatrix[0, 1] = (thirdOrderSquare[2, 1] * thirdOrderSquare[0, 2] - thirdOrderSquare[0, 1] * thirdOrderSquare[2, 2]) / detA;
            inverseMatrix[0, 2] = (thirdOrderSquare[0, 1] * thirdOrderSquare[1, 2] - thirdOrderSquare[1, 1] * thirdOrderSquare[0, 2]) / detA;

            inverseMatrix[1, 0] = (thirdOrderSquare[2, 0] * thirdOrderSquare[1, 2] - thirdOrderSquare[1, 0] * thirdOrderSquare[2, 2]) / detA;
            inverseMatrix[1, 1] = (thirdOrderSquare[0, 0] * thirdOrderSquare[2, 2] - thirdOrderSquare[0, 2] * thirdOrderSquare[2, 0]) / detA;
            inverseMatrix[1, 2] = (thirdOrderSquare[1, 0] * thirdOrderSquare[0, 2] - thirdOrderSquare[0, 0] * thirdOrderSquare[1, 2]) / detA;

            inverseMatrix[2, 0] = (thirdOrderSquare[1, 0] * thirdOrderSquare[2, 1] - thirdOrderSquare[2, 0] * thirdOrderSquare[1, 1]) / detA;
            inverseMatrix[2, 1] = (thirdOrderSquare[2, 0] * thirdOrderSquare[0, 1] - thirdOrderSquare[0, 0] * thirdOrderSquare[2, 1]) / detA;
            inverseMatrix[2, 2] = (thirdOrderSquare[0, 0] * thirdOrderSquare[1, 1] - thirdOrderSquare[0, 1] * thirdOrderSquare[1, 0]) / detA;

            return inverseMatrix;
        }

        /// <summary>
        /// Q函数
        /// </summary>
        /// <param name="x">自变量</param>
        /// <returns>函数值</returns>
        public static Double QFunction(Double x, Double eps = 1e-5)
        {
            return _Sqrt2 * Erfcinv(2 * x, eps);
        }

        private static readonly Double _Sqrt2 = 1.4142135623730950488016887242096980785696718753769480731766797379907324784621070388503875343276415727;

        /// <summary>
        /// 互补误差函数
        /// </summary>
        /// <param name="x">自变量</param>
        /// <returns>函数值</returns>
        public static Double Erfc(Double x)
        {
            return 1 - Erf(x);
        }

        private static Func<Double, Double> ErfSingleTransFunc = t => 2 / Math.Pow(Math.PI, 0.5) * Math.Pow(Math.E, -t * t);

        /// <summary>
        /// 误差函数
        /// </summary>
        /// <param name="x">自变量</param>
        /// <returns>函数值</returns>
        public static Double Erf(Double x)
        {
            //Func<Double, Double> func = t => 2 / Math.Pow(Math.PI, 0.5) * Math.Pow(Math.E, -t * t);
            //Double result = MathExt.Algorithm.Integeral(ErfSingleTransFunc, 0, x);
            //return result;
            return MathExt.Algorithm.Integeral(ErfSingleTransFunc, 0, x);
        }

        /// <summary>
        /// 互补误差函数的反函数
        /// </summary>
        /// <param name="x">自变量(需大于0小于等于1)</param>
        /// <param name="eps">误差限</param>
        /// <returns>函数值</returns>
        public static Double Erfcinv(Double x, Double eps = 1E-5)
        {
            Double low = 0;
            Double high = 30;
            Double mid = (low + high) / 2;
            Int32 count = 0;
            if (x >= 1)
            {
                return 0;
            }
            else if (x <= 0)
            {
                return Double.MaxValue;
            }
            else
            {
                while (Math.Abs(x - Erfc(mid)) > eps)
                {
                    if (x < Erfc(mid))
                    {
                        low = mid;
                        mid = (low + high) / 2;
                    }
                    else
                    {
                        high = mid;
                        mid = (low + high) / 2;
                    }
                    count++;
                    if (count > 30)
                        break;
                }
                return mid;
            }
        }

        /// <summary>
        /// 误差函数的反函数
        /// </summary>
        /// <param name="x">自变量(需大于等于0小于1)</param>
        /// <param name="eps">误差限</param>
        /// <returns>函数值</returns>
        public static Double Erfinv(Double x, Double eps = 1E-5)
        {
            Double low = 0;
            Double high = 30;
            Double mid = (low + high) / 2;
            Int32 count = 0;
            if (x <= 0)
            {
                return 0;
            }
            else if (x >= 1)
            {
                return Double.MaxValue;
            }
            else
            {
                while (Math.Abs(x - Erfc(mid)) > eps)
                {
                    if (x < Erfc(mid))
                    {
                        high = mid;
                        mid = (low + high) / 2;
                    }
                    else
                    {
                        low = mid;
                        mid = (low + high) / 2;
                    }
                    count++;
                    if (count > 30)
                        break;
                }
                return mid;
            }
        }

        public static void SymmetryArray(Double[] data)
        {
            if (data.Length < 1)
            {
                return;
            }
            Double TIEMean = data.Average();
            for (Int32 i = 0; i < data.Length; i++)
            {
                data[i] -= TIEMean;
            }
        }

        private static Double[] GetCDFByPDF(Double[] pdfData)
        {
            var length = pdfData.Length;
            Double[] cdf = new Double[length];
            cdf[length - 1] = pdfData[length - 1];
            for (Int32 i = length - 2; i >= 0; i--)
            {
                cdf[i] = cdf[i + 1] + pdfData[i];
            }
            return cdf;
        }

        private static Double[,] CalcXYMatrix(Double[] leftY, Double[] rightY, Double xMax, Double xStep)
        {
            Double[,] ans = new Double[1, (Int32)Math.Round(xMax / xStep)];
            var leftlength = leftY.Length;
            var rightlength = rightY.Length;
            var anslength = ans.Length;
            if (anslength - leftlength > Int32.MaxValue)
            {
                return ans;
            }
            if (rightlength + leftlength <= anslength)
            {
                Buffer.BlockCopy(rightY, 0, ans, 0, rightlength * sizeof(Double));
                Buffer.BlockCopy(leftY.ReverseArray(), 0, ans, (anslength - leftlength) * sizeof(Double), leftlength * sizeof(Double));
                for (Int32 i = rightlength; i < anslength - leftlength; i++)
                {
                    ans[0, i] = Double.NaN;
                }
            }
            else if (rightlength > anslength / 2 && leftlength < anslength / 2)
            {
                Buffer.BlockCopy(rightY.TakeArray(anslength - leftlength), 0, ans, 0, (anslength - leftlength) * sizeof(Double));
                Buffer.BlockCopy(leftY.ReverseArray(), 0, ans, (anslength - leftlength) * sizeof(Double), leftlength * sizeof(Double));
            }
            else if (rightY.Length < anslength / 2 && leftlength > anslength / 2)
            {
                Buffer.BlockCopy(rightY, 0, ans, 0, rightlength * sizeof(Double));
                Buffer.BlockCopy(leftY.TakeAndReverseArray(anslength - rightlength), 0, ans, rightlength * sizeof(Double), (anslength - rightlength) * sizeof(Double));
            }
            else
            {
                Buffer.BlockCopy(rightY.TakeArray((anslength + 1) / 2), 0, ans, 0, ((anslength + 1) / 2) * sizeof(Double));
                Buffer.BlockCopy(leftY.TakeAndReverseArray(anslength / 2), 0, ans, ((anslength + 1) / 2) * sizeof(Double), (anslength / 2) * sizeof(Double));
            }
            return ans;
        }




        /// <summary>
        /// 最小二乘法拟合CDF
        /// </summary>
        /// <param name="CDFlog">CDF每个bin的值</param>
        /// <param name="histSize">CDF bin的个数</param>
        /// <param name="tail">尾部的索引值</param>
        /// <returns>拟合得到的二次曲线系数</returns>
        public static Double[] GetFitFactor(Double[] CDFlog, Int32 histSize, Int32 tail)
        {
            //设置尾部权重
            Double[] tailWeight = new Double[histSize - tail];
            //y=x线性权重
            Double tempsum = (histSize - tail) * (histSize - tail + 1) / 2;
            for (Int32 i = 0; i < tailWeight.Length; i++)
            {
                tailWeight[i] = (i + 1) / tempsum;
            }


            Double sumx = 0, sumx2 = 0, sumx3 = 0, sumx4 = 0, Ncnt = 0;
            Double sumy = 0, sumxy = 0, sumx2y = 0;
            Double x2 = 0, x3 = 0, x4 = 0;
            for (Int32 i = tail; i < histSize; i++)
            {
                Double j = (Double)(i / 10.0);
                Ncnt += tailWeight[i - tail];

                sumx += tailWeight[i - tail] * j;

                x2 = tailWeight[i - tail] * j * j;
                sumx2 += x2;

                x3 = x2 * j;
                sumx3 += x3;

                x4 = x3 * j;
                sumx4 += x4;

                sumy += CDFlog[i] * tailWeight[i - tail];
                sumxy += (CDFlog[i] * tailWeight[i - tail] * j);
                sumx2y += (CDFlog[i] * x2);
            }

            Double[,] AMatrix ={{sumx4,sumx3,sumx2},
                              {sumx3,sumx2,sumx},
                              {sumx2,sumx,Ncnt}};

            Double[,] InveMatrix = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            InveMatrix = GetInverseMatrix(AMatrix);//逆矩阵

            Double[] fitFactor = new Double[3];

            fitFactor[0] = InveMatrix[0, 0] * sumx2y + InveMatrix[0, 1] * sumxy + InveMatrix[0, 2] * sumy;
            fitFactor[1] = InveMatrix[1, 0] * sumx2y + InveMatrix[1, 1] * sumxy + InveMatrix[1, 2] * sumy;
            fitFactor[2] = InveMatrix[2, 0] * sumx2y + InveMatrix[2, 1] * sumxy + InveMatrix[2, 2] * sumy;
            //FitFactor[0]为二次项系数，FitFactor[1]为一次项系数，FitFactor[2]为常数项
            return fitFactor;
        }

        public static Double[] SecondOrderFitting(Double[] x, Double[] y, Double fittingPointsNumber)
        {
            if (x.Length != y.Length)
                throw new ArgumentException("The length of x must be equal to the length of y.");

            if (x.Length <= 2)
                throw new ArgumentException("The length of x must be greater than 2.");

            if (fittingPointsNumber > x.Length || fittingPointsNumber <= 2)
                throw new ArgumentException("fittingPointsNumber must be less than or equal to the length of x and be greater than 2.", nameof(fittingPointsNumber));

            Double sumx = 0, sumx2 = 0, sumx3 = 0, sumx4 = 0, Ncnt = 0;
            Double sumy = 0, sumxy = 0, sumx2y = 0;
            Double x2 = 0, x3 = 0, x4 = 0;

            for (Int32 i = 0; i < fittingPointsNumber; i++)
            {
                Ncnt++;

                sumx += x[i];

                x2 = x[i] * x[i];
                sumx2 += x2;

                x3 = x2 * x[i];
                sumx3 += x3;

                x4 = x3 * x[i];
                sumx4 += x4;

                sumy += y[i];
                sumxy += (y[i] * x[i]);
                sumx2y += (y[i] * x2);
            }

            Double[,] AMatrix ={{sumx4,sumx3,sumx2},
                              {sumx3,sumx2,sumx},
                              {sumx2,sumx,Ncnt}};


            Double[,] InveMatrix = GetInverseMatrix(AMatrix);//逆矩阵

            Double[] fitFactor = new Double[3];

            fitFactor[0] = InveMatrix[0, 0] * sumx2y + InveMatrix[0, 1] * sumxy + InveMatrix[0, 2] * sumy;
            fitFactor[1] = InveMatrix[1, 0] * sumx2y + InveMatrix[1, 1] * sumxy + InveMatrix[1, 2] * sumy;
            fitFactor[2] = InveMatrix[2, 0] * sumx2y + InveMatrix[2, 1] * sumxy + InveMatrix[2, 2] * sumy;
            //FitFactor[0]为二次项系数，FitFactor[1]为一次项系数，FitFactor[2]为常数项
            return fitFactor;
        }

        public void Dispose()
        {
            Hist?.Dispose();
            MJSQ?.Dispose();
            NQScale?.Dispose();
        }
    }
}
