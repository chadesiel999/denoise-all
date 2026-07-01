using System;
using System.Collections.Generic;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.U2;

namespace ScopeX.Core.Model.Jitter.Common
{
    /// <summary>
    /// NRZ码元切割和时钟恢复
    /// </summary>
    public static partial class ClockRecovery
    {
        public static Boolean GetNRZClock(Double[] data, JitterParameter @params, JitterPrepare prepareData)
        {
            //在单个UI中点数小于固定值n时使用Cubic插值
            var clockedges = NRZExtractEdgesByCubicSpline(data, prepareData.HighLevel, prepareData.LowLevel, (@params.Threshold + @params.Hysteresis) / 100, (@params.Threshold - @params.Hysteresis) / 100, @params.Threshold / 100, JitterInterpolationType.Linear);
            if (clockedges == null || clockedges.Length < 8)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.ChannelUITooLess);
                return false;
            }

            var type = GetNRZSignalType(clockedges, out Double averageUILength);
            switch (type)
            {
                case SignalType.Clock:
                    @params.SignalType = SignalType.Clock;
                    break;
                case SignalType.PRBSCode:
                    clockedges = InsertEdges(clockedges, averageUILength, out Int32[] nonzeroIndex);
                    @params.SignalType = SignalType.PRBSCode;
                    prepareData.NonzeroIndex = nonzeroIndex;
                    break;
                default:
                    break;
            }

            prepareData.AverageUILength = averageUILength;
            prepareData.UICount = (Int32)(data.Length / prepareData.AverageUILength);
            prepareData.ClockEdges = clockedges;

            if (@params.ClockType == ClockTypeOpt.Constant)
            {
                prepareData.RecoveredEdges = ConstantClock(prepareData.ClockEdges);
            }
            else
            {
                Double[] output_of_pll;
                switch (@params.PLLType)
                {
                    case PllTypeOpt.SecondOrder:
                        output_of_pll = SPLL_SecondOrder(prepareData.ClockEdges, @params.Fs, @params.NaturalFreq, @params.DamplingFactor);//1150ms
                        break;
                    case PllTypeOpt.Golden:
                    default:
                        output_of_pll = SPLL_FCGolden(prepareData.ClockEdges, @params.Fs, @params.CutoffDivisor);
                        break;
                }
                prepareData.RecoveredEdges = output_of_pll;
            }
            return true;
        }

        /// <summary>
        /// 提取信号的边沿
        /// </summary>
        /// <param name="samples">样点序列</param>
        /// <param name="highLevel">高电平值</param>
        /// <param name="lowLevel">低电平值</param>
        /// <param name="hysteresis">迟滞电平，与幅度的比值，范围：0-1</param>
        /// <param name="threshold">比较阈值，与幅度的比值，范围：0-1</param>
        /// <returns>返回信号边沿序列</returns>
        public static Double[] ExtractEdges(Double[] samples, Double highLevel, Double lowLevel, Double topThreshold, Double baseThreshold, Double threshold = 0.5)
        {
            //1. 求边界（1/2-1/3 ~ 1/2+1/3）即(1/6 ~ 5/6)  边界能否更窄从而增加寻找速度?
            Int32 sampleindex = 0, i = 0;
            Int32 xresult0 = 0, xresult1 = 0; //高低边界点的横坐标
            Double result = 0;
            Int32 resultcnt = 0;
            Double falldot = 0, riseDot = 0;
            Boolean riseedgenow = true;
            Double amplitude = highLevel - lowLevel;
            List<Double> originaledges = new List<Double>();
            if (threshold > 1 || threshold < 0)
                return originaledges.ToArray();
            Double mid = lowLevel + amplitude * threshold;
            //默认从第一个上升沿开始寻找，若输入信号第一个边沿为下降沿则会跳过该下降沿寻找该下降沿之后的第一个上升沿
            //即该边沿寻找算法总是先找到第一个上升沿
            while (sampleindex < samples.Length - 1)
            {
                //从第一个上升沿开始寻找
                if (riseedgenow)
                {
                    while (sampleindex < samples.Length - 1 && samples[sampleindex] >= baseThreshold * amplitude + lowLevel)
                    {
                        xresult0 = sampleindex;
                        sampleindex++;
                    }
                    while (sampleindex < samples.Length - 1 && samples[sampleindex] < topThreshold * amplitude + lowLevel)
                    {
                        if (samples[sampleindex] < baseThreshold * amplitude + lowLevel)
                        {
                            xresult0 = sampleindex;
                        }
                        sampleindex++;
                    }
                    xresult1 = sampleindex;

                    result = 0;
                    resultcnt = 0;

                    for (i = xresult0 + 1; i <= xresult1; ++i)
                    {
                        if (samples[i - 1] <= mid && samples[i] > mid)
                        {
                            result = result + i - 1 + (mid - samples[i - 1]) / (samples[i] - samples[i - 1]);  //目前暂时采用线性插值，后面考虑三次样条插值
                            resultcnt++;
                        }
                    }
                    if (resultcnt != 0)
                    {
                        riseDot = result / resultcnt;
                        originaledges.Add(riseDot);
                        riseedgenow = !riseedgenow;
                    }
                }

                else    //下降沿
                {
                    while (sampleindex < samples.Length - 1 && samples[sampleindex] <= topThreshold * amplitude + lowLevel)
                    {
                        xresult1 = sampleindex;
                        sampleindex++;
                    }
                    while (sampleindex < samples.Length - 1 && samples[sampleindex] > baseThreshold * amplitude + lowLevel)
                    {
                        if (samples[sampleindex] > topThreshold * amplitude + lowLevel)
                        {
                            xresult1 = sampleindex;
                        }
                        sampleindex++;
                    }
                    xresult0 = sampleindex;

                    result = 0;
                    resultcnt = 0;

                    for (i = xresult1 + 1; i <= xresult0; ++i)
                    {
                        if (samples[i - 1] >= mid && samples[i] < mid)
                        {
                            result = result + i - 1 + (samples[i - 1] - mid) / (samples[i - 1] - samples[i]);
                            resultcnt++;
                        }

                    }
                    if (resultcnt != 0)
                    {
                        falldot = result / resultcnt;
                        originaledges.Add(falldot);
                        riseedgenow = !riseedgenow;
                    }
                }
            }

            return originaledges.ToArray();
        }

        public static Double[] NRZExtractEdgesByCubicSpline(Double[] samples, Double highLevel, Double lowLevel, Double topThreshold, Double baseThreshold, Double threshold = 0.5, JitterInterpolationType interopType = JitterInterpolationType.CubicSpline)
        {
            if (threshold > 1 || threshold < 0)
                return new Double[0];

            Int32 samplecount = samples.Length;
            var originaledges = new List<Double>();
            var types = new List<Int32>();

            Double amplitude = highLevel - lowLevel;
            Double mid = lowLevel + amplitude * threshold;

            Boolean riseedgenow = true;
            Int32 sampleindex = 0;

            Double @base = Math.Floor(baseThreshold * amplitude + lowLevel);
            Double @top = Math.Ceiling(topThreshold * amplitude + lowLevel);

            while (sampleindex < samplecount - 1)
            {
                // 根据当前是寻找上升沿还是下降沿来处理
                var (StartIndex, EndIndex, InterpolateDirection) = GetEdgeIndices(sampleindex, samples, samplecount, @base, @top, riseedgenow);
                if (StartIndex >= 0 && EndIndex >= 0&&EndIndex< samples.Length-1)
                {
                    Double edgeValue = CalculateEdgeValue(samples, StartIndex, EndIndex, interopType, mid, InterpolateDirection);
                    if (!Double.IsNaN(edgeValue))
                    {
                        originaledges.Add(edgeValue);
                        types.Add(InterpolateDirection == 1 ? 1 : -1);
                        riseedgenow = !riseedgenow;
                    }
                }
                sampleindex = EndIndex + 1;
            }

            return originaledges.ToArray();
        }

        #region PAM4
        public static Double[] PAM4ExtractEdgesByCubicSpline(Double[] samples, Double highLevel, Double lowLevel, Double threshold01, Double threshold12, Double threshold23, Double hysteresis, JitterInterpolationType interopType = JitterInterpolationType.CubicSpline)
        {
            if (hysteresis > 0.3 || hysteresis < 0)
                return new Double[0];
            if ((threshold01 > 1 || threshold01 < 0) || (threshold12 > 1 || threshold12 < 0) || (threshold23 > 1 || threshold23 < 0))
                return new Double[0];

            Int32 samplecount = samples.Length;
            var originaledges = new List<Double>();

            Double amplitude = highLevel - lowLevel;
            Double mid01 = lowLevel + amplitude * threshold01;
            Double mid12 = lowLevel + amplitude * threshold12;
            Double mid23 = lowLevel + amplitude * threshold23;

            Boolean riseedgenow = true;
            Int32 sampleindex = 0;

            Double top01 = lowLevel + amplitude * (threshold01 + hysteresis);
            Double base01 = lowLevel + amplitude * (threshold01 - hysteresis);

            Double top12 = lowLevel + amplitude * (threshold12 + hysteresis);
            Double base12 = lowLevel + amplitude * (threshold12 - hysteresis);

            Double top23 = lowLevel + amplitude * (threshold23 + hysteresis);
            Double base23 = lowLevel + amplitude * (threshold23 - hysteresis);

            #region 01沿
            while (sampleindex < samplecount - 1)
            {
                // 根据当前是寻找上升沿还是下降沿来处理
                var (StartIndex, EndIndex, InterpolateDirection) = GetEdgeIndices(sampleindex, samples, samplecount, base01, top01, riseedgenow);
                if (StartIndex >= 0 && EndIndex >= 0)
                {
                    Double edgeValue = CalculateEdgeValue(samples, StartIndex, EndIndex, interopType, mid01, InterpolateDirection);
                    if (!Double.IsNaN(edgeValue))
                    {
                        originaledges.Add(edgeValue);
                        riseedgenow = !riseedgenow;
                    }
                }
                sampleindex = EndIndex + 1;
            }
            #endregion

            #region 12沿
            riseedgenow = true;
            sampleindex = 0;

            while (sampleindex < samplecount - 1)
            {
                // 根据当前是寻找上升沿还是下降沿来处理
                var (StartIndex, EndIndex, InterpolateDirection) = GetEdgeIndices(sampleindex, samples, samplecount, base12, top12, riseedgenow);
                if (StartIndex >= 0 && EndIndex >= 0)
                {
                    Double edgeValue = CalculateEdgeValue(samples, StartIndex, EndIndex, interopType, mid12, InterpolateDirection);
                    if (!Double.IsNaN(edgeValue))
                    {
                        originaledges.Add(edgeValue);
                        riseedgenow = !riseedgenow;
                    }
                }
                sampleindex = EndIndex + 1;
            }
            #endregion

            #region 23沿
            riseedgenow = true;
            sampleindex = 0;

            while (sampleindex < samplecount - 1)
            {
                // 根据当前是寻找上升沿还是下降沿来处理
                var (StartIndex, EndIndex, InterpolateDirection) = GetEdgeIndices(sampleindex, samples, samplecount, base23, top23, riseedgenow);
                if (StartIndex >= 0 && EndIndex >= 0)
                {
                    Double edgeValue = CalculateEdgeValue(samples, StartIndex, EndIndex, interopType, mid23, InterpolateDirection);
                    if (!Double.IsNaN(edgeValue))
                    {
                        originaledges.Add(edgeValue);
                        riseedgenow = !riseedgenow;
                    }
                }
                sampleindex = EndIndex + 1;
            }
            #endregion

            return originaledges.OrderBy(x => x).ToArray();
        }
        #endregion
        private static (Int32 startIndex, Int32 endIndex, Int32 interpolateDirection) GetEdgeIndices(Int32 startSampleIndex, Double[] samples, Int32 sampleCount, Double baseThresholdValue, Double topThresholdValue, Boolean isRiseEdge)
        {
            Int32 xresult0 = 0, xresult1 = 0;
            if (isRiseEdge)
            {
                while (startSampleIndex < sampleCount - 1 && samples[startSampleIndex] >= baseThresholdValue)
                {
                    xresult0 = startSampleIndex;
                    startSampleIndex++;
                }
                while (startSampleIndex < sampleCount - 1 && samples[startSampleIndex] < topThresholdValue)
                {
                    if (samples[startSampleIndex] < baseThresholdValue)
                    {
                        xresult0 = startSampleIndex;
                    }
                    startSampleIndex++;
                }
                xresult1 = startSampleIndex;
                return (xresult0, xresult1, 1);
            }
            else
            {
                while (startSampleIndex < sampleCount - 1 && samples[startSampleIndex] <= topThresholdValue)
                {
                    xresult1 = startSampleIndex;
                    startSampleIndex++;
                }
                while (startSampleIndex < sampleCount - 1 && samples[startSampleIndex] > baseThresholdValue)
                {
                    if (samples[startSampleIndex] > topThresholdValue)
                    {
                        xresult1 = startSampleIndex;
                    }
                    startSampleIndex++;
                }
                xresult0 = startSampleIndex;
                return (xresult1, xresult0, -1);
            }
        }

        private static Double CalculateEdgeValue(Double[] samples, Int32 startIndex, Int32 endIndex, JitterInterpolationType interopType, Double mid, Int32 interpolateDirection)
        {
            Double result = 0;
            Int32 resultCnt = 0;
            switch (interopType)
            {
                case JitterInterpolationType.Linear:
                    for (Int32 i = startIndex + 1; i <= endIndex; ++i)
                    {
                        if (interpolateDirection == 1 && samples[i - 1] <= mid && samples[i] > mid ||
                            interpolateDirection == -1 && samples[i - 1] >= mid && samples[i] < mid)
                        {
                            result += i - 1 + (interpolateDirection == 1 ? (mid - samples[i - 1]) / (samples[i] - samples[i - 1]) : (samples[i - 1] - mid) / (samples[i - 1] - samples[i]));
                            resultCnt++;
                        }
                    }
                    break;
                case JitterInterpolationType.CubicSpline:
                    // 优化CubicSpline插值计算逻辑，提前判断一些边界情况，避免不必要的循环
                    if (endIndex - startIndex < 3)
                    {
                        // 如果数据点过少，无法进行有效的CubicSpline插值，采用线性插值替代
                        for (Int32 i = startIndex + 1; i <= endIndex; ++i)
                        {
                            if (interpolateDirection == 1 && samples[i - 1] <= mid && samples[i] > mid ||
                                interpolateDirection == -1 && samples[i - 1] >= mid && samples[i] < mid)
                            {
                                result += i - 1 + (interpolateDirection == 1 ? (mid - samples[i - 1]) / (samples[i] - samples[i - 1]) : (samples[i - 1] - mid) / (samples[i - 1] - samples[i]));
                                resultCnt++;
                            }
                        }
                    }
                    else
                    {
                        for (Int32 i = startIndex + 1; i <= endIndex; ++i)
                        {
                            if (i - 2 > 0 && i + 1 < samples.Length - 1)
                            {
                                if (interpolateDirection == 1 && samples[i - 1] <= mid && samples[i] > mid ||
                                    interpolateDirection == -1 && samples[i - 1] >= mid && samples[i] < mid)
                                {
                                    Int32 ratio = 10;
                                    Int32 fitLength = 4;
                                    Int32 totalLength = fitLength * ratio;
                                    Double midValuePos = 0;
                                    var x = new Single[4] { i - 2, i - 1, i, i + 1 };
                                    var y = new Single[4] { (Single)samples[i - 2], (Single)samples[i - 1], (Single)samples[i], (Single)samples[i + 1] };
                                    var xs = new Single[totalLength];
                                    Single stepSize = (x[x.Length - 1] - x[0]) / (totalLength - 1);
                                    for (Int32 k = 0; k < totalLength; k++)
                                    {
                                        xs[k] = x[0] + k * stepSize;
                                    }
                                    Single[] ys = new CubicSpline().FitAndEval(x, y, xs);
                                    for (Int32 l = 0; l < totalLength - 1; l++)
                                    {
                                        if (interpolateDirection == 1 && ys[l] <= mid && ys[l + 1] > mid ||
                                            interpolateDirection == -1 && ys[l] >= mid && ys[l + 1] < mid)
                                        {
                                            midValuePos = stepSize * (1 + (interpolateDirection == 1 ? (mid - ys[l]) / (ys[l + 1] - ys[l]) : (mid - ys[l]) / (ys[l + 1] - ys[l])));
                                        }
                                    }
                                    result += i - 1 + midValuePos;
                                    resultCnt++;
                                }
                            }
                        }
                    }
                    break;
                case JitterInterpolationType.Sinc:
                    break;
                default:
                    for (Int32 i = startIndex + 1; i <= endIndex; ++i)
                    {
                        if (interpolateDirection == 1 && samples[i - 1] <= mid && samples[i] > mid ||
                            interpolateDirection == -1 && samples[i - 1] >= mid && samples[i] < mid)
                        {
                            result += i - 1 + (interpolateDirection == 1 ? (mid - samples[i - 1]) / (samples[i] - samples[i - 1]) : (samples[i - 1] - mid) / (samples[i - 1] - samples[i]));
                            resultCnt++;
                        }
                    }
                    break;
            }

            return resultCnt > 0 ? result / resultCnt : Double.NaN;
        }



        private static void ExtractEdgeInit(Double[] samples, Double @top, Double @base, Boolean isRaise, ref Int32 sampleIndex, ref Int32 xResult0, ref Int32 xResult1)
        {
            //找到满足条件的周期
            //1.周期左半部分
            //上升沿找到小于底值的索引
            //下降沿找到大于顶值的索引
            while (sampleIndex < samples.Length - 1 && (isRaise ? samples[sampleIndex] >= @base : samples[sampleIndex] <= @top))
            {
                xResult0 = sampleIndex;
                sampleIndex++;
            }
            //2.周期右半部分
            //上升沿找到再次回到底值的索引
            //下降沿找到再次回到顶值的索引
            while (sampleIndex < samples.Length - 1 && (isRaise ? samples[sampleIndex] < @top : samples[sampleIndex] > @base))
            {
                if (isRaise ? samples[sampleIndex] < @base : samples[sampleIndex] > @top)
                {
                    xResult0 = sampleIndex;
                }
                sampleIndex++;
            }
            xResult1 = sampleIndex;
        }


        public static SignalType GetNRZSignalType(Double[] edges, out Double averageUILength)
        {
            Boolean isdatasignal = IsDataSignal(edges, out averageUILength);

            return isdatasignal ? SignalType.PRBSCode : SignalType.Clock;
        }

        /// <summary>
        /// 判断信号是否为数据信号，输出平均码元长度
        /// </summary>
        /// <param name="edges">边沿序列</param>
        /// <param name="averageUILength">平均UI长度</param>
        /// <returns>若信号为数据信号，则返回true；否则返回false</returns>
        public static Boolean IsDataSignal(Double[] edges, out Double averageUILength)
        {
            Boolean is_data_signal = false;
            Int32 cnt = edges.Length - 1;
            if (cnt < 0)
            {
                averageUILength = 0;
                return false;
            }
            Int32[] pulse_length = new Int32[cnt];   //直方图找峰值使用
            Double[] pulse_length_double = new Double[cnt];  //求平均脉宽使用
            Int32 max_pulse_length = Int32.MinValue;
            Int32 min_pulse_length = Int32.MaxValue;
            for (Int32 id = 0; id < cnt; id++)
            {
                pulse_length[id] = (Int32)(edges[id + 1] - edges[id] + 0.5); //默认向下取整因此加0.5实现四舍五入
                pulse_length_double[id] = edges[id + 1] - edges[id];

                if (pulse_length[id] > max_pulse_length)
                {
                    max_pulse_length = pulse_length[id];
                }
                if (pulse_length[id] < min_pulse_length)
                {
                    min_pulse_length = pulse_length[id];
                }
            }
            if (pulse_length.Length < 1 || max_pulse_length < 0)
            {
                averageUILength = 0;
                return false;
            }

            //找1UI的脉宽    
            //作脉宽直方图
            if (max_pulse_length < 0)
            {
                averageUILength = 0;
                return false;
            }
            Int32[] pulse_len_hist = new Int32[max_pulse_length + 1];
            for (Int32 i = 0; i < pulse_length.Count(); ++i)
            {
                if (pulse_length[i] < pulse_len_hist.Length && pulse_length[i] > 0)
                {
                    pulse_len_hist[pulse_length[i]]++;

                }
            }
            //分析脉宽直方图
            //①判断minPulse是否是异常值，如果minPulse异常，说明信号不连续
            //为了测量准确，输入信号脉宽最少为50；
            //锁相环能够处理的抖动，最大为脉宽的1/10(估计);
            //如果信号连续，即脉宽直方图minPulse在1UI附近，数量较多
            Int32 min_pulse = min_pulse_length;
            Int32 first_crestval = 0;
            Int32 first_crestpos = 0;
            Boolean find_first_crest = false;
            Int32 first_not_zero_pos = 0;

            for (Int32 i = 1; i < max_pulse_length && !find_first_crest; ++i)
            {
                if (pulse_len_hist[i] == 0)
                    first_not_zero_pos = i + 1;
                else if (pulse_len_hist[i] >= pulse_len_hist[i - 1] && pulse_len_hist[i] >= pulse_len_hist[i + 1])
                {
                    first_crestval = pulse_len_hist[i];//第一个小尖峰，不一定是1UI脉宽峰的峰值，可能由抖动导致
                    first_crestpos = i;//直方图1UI脉宽
                    find_first_crest = true;
                    //如果这个小尖峰是峰值，那么同时它是这个峰内最大值
                    for (Int32 j = i - (i - first_not_zero_pos); j < Math.Min(i + (i - first_not_zero_pos), pulse_len_hist.Length - 1); ++j)
                    {
                        if (first_crestval < pulse_len_hist[j])//FirstCrestIsOneUILen不是1UI脉宽峰的峰值，重新找
                        {
                            find_first_crest = false;
                            break;
                        }
                    }
                }
            }
            if (find_first_crest)
                min_pulse = first_crestpos;  //最小脉宽也即1个UI的宽度

            //统计计算平均UI长度
            Int32 pulse_cnt = 0;
            Double total_length = 0;

            if (max_pulse_length > 1.5 * min_pulse)
            {
                is_data_signal = true;
            }

            if (min_pulse_length < min_pulse * 0.5)
            {
                is_data_signal = false;//数据乱码,不能分析
            }

            for (Int32 i = 0; i < cnt; ++i)
            {
                total_length += pulse_length_double[i];//freq用点数表示
                pulse_cnt += (Int32)Math.Round(pulse_length_double[i] / min_pulse);
            }
            averageUILength = total_length / pulse_cnt;
            return is_data_signal;
        }

        /// <summary>
        /// 为数据信号两个跳变点之间插入虚拟边沿（实际插入的为正弦样点）
        /// </summary>
        /// <param name="edges">边沿序列</param>
        /// <param name="averageUILength">平均UI长度</param>
        /// <param name="nonzeroIndex">TIE不为0的索引，即真实边沿的索引</param>
        /// <returns>插入虚拟边沿后的样点序列</returns>
        public static Double[] InsertEdges(Double[] samples, Double[] edges, Double averageUILength, out Int32[] nonzeroIndex)
        {
            List<Int32> tie_index = new List<Int32>();
            List<Double> inserted_samples = new List<Double>();
            //对于数据信号，将连续0或1出现的时候的时钟边沿加上，时钟信号可以直接复制            
            //insertedSamples.Add(samples[(Int32)edges[0]]);
            Double one_ui_length = averageUILength;    //1UI的点数
            Double current_ui_length;
            Int32 cnt;
            Int32 index = 0;
            if (edges.Length < 2)
            {
                nonzeroIndex = new Int32[0];
                return new Double[0];
            }

            for (Int32 i = 0; i < edges.Length - 1; i++)
            {
                if (edges[i + 1] - edges[i] >= 0.5 * one_ui_length)
                {
                    cnt = (Int32)((edges[i + 1] - edges[i]) / one_ui_length + 0.5);
                    current_ui_length = (edges[i + 1] - edges[i]) / cnt;
                    for (Int32 j = 0; j < (Int32)edges[i + 1] - (Int32)edges[i]; j++)
                    {
                        inserted_samples.Add(Math.Sin(2 * Math.PI / current_ui_length * j));
                    }
                    index = index + 2 * cnt;
                }
                else
                {
                    Int16 m = 0;
                    while (edges[i + 1 + m] - edges[i] < 0.5 * one_ui_length)
                    {
                        m++;
                    }
                    cnt = (Int32)((edges[i + 1 + m] - edges[i]) / one_ui_length + 0.5);
                    current_ui_length = (edges[i + 1 + m] - edges[i]) / cnt;
                    for (Int32 j = 0; j < (Int32)edges[i + 1 + m] - (Int32)edges[i]; j++)
                    {
                        inserted_samples.Add(Math.Sin(2 * Math.PI / current_ui_length * j));
                    }
                    index = index + 2 * cnt;
                }
                tie_index.Add(index);
            }

            nonzeroIndex = tie_index.Select(o => o - 2).ToArray();
            return inserted_samples.ToArray();

        }
        public static Double[] InsertEdges(Double[] edges, Double averageUILength, out Int32[] nonzeroIndex)
        {
            List<Int32> tie_index = new List<Int32>();
            List<Double> inserted_edges = new List<Double>();
            //对于数据信号，将连续0或1出现的时候的时钟边沿加上，时钟信号可以直接复制            
            Double one_ui_length = averageUILength;    //1UI的点数
            Double current_ui_length;
            Int32 cnt;
            Int32 index = 0;
            Int32 i = 0;

            while (i <= edges.Length - 2)
            {
                cnt = (Int32)Math.Round((edges[i + 1] - edges[i]) / one_ui_length);
                Int16 m = 0;
                while (cnt == 0)
                {
                    m++;
                    if (i + 1 + m > edges.Length - 1)
                        break;
                    cnt = (Int32)Math.Round((edges[i + 1 + m] - edges[i]) / one_ui_length);
                }
                if (i + 1 + m > edges.Length - 1)
                    break;
                current_ui_length = (edges[i + 1 + m] - edges[i]) / cnt;
                for (Int32 j = 0; j < cnt; j++)
                {
                    inserted_edges.Add(edges[i] + current_ui_length * j);
                }
                tie_index.Add(index);
                index += cnt;
                i += m + 1;
            }

            nonzeroIndex = tie_index.ToArray();
            return inserted_edges.ToArray();

        }

        /// <summary>
        /// 用户自定义锁相环
        /// </summary>
        /// <param name="input">锁相环输入序列</param>
        /// <param name="fs">采样率</param>
        /// <param name="oneUILength">平均UI长度</param>
        /// <param name="filterCoefficients">滤波器系数</param>
        /// <returns>经过PLL后的序列</returns>
        public static Double[] CustomPLL(Double[] input, Double fs, Double oneUILength, Double[][] filterCoefficients)
        {
            Double pdgain = 1.0;
            Double lfgain = 1.0;
            Double vcogain = 1.0;
            Double vcoinitialfreq = fs / (oneUILength * 2);
            Int32 pole_length = filterCoefficients[0].Length;
            Int32 zero_length = filterCoefficients[1].Length;
            Double[] a = new Double[pole_length];
            Double[] b = new Double[zero_length];
            for (Int32 i = 0; i < filterCoefficients.Length; i++)
            {
                for (Int32 j = 0; j < filterCoefficients[i].Length; j++)
                {
                    a[j] = filterCoefficients[0][j];
                    b[j] = filterCoefficients[1][j];
                }
            }

            Double[] pdout = new Double[input.Length];
            Double[] lfout = new Double[input.Length];
            Double[] vcophase = new Double[input.Length];
            Double[] vcoout = new Double[input.Length];

            Double input_max_value = input.Max();
            Double input_min_value = input.Min();
            Double offset = (input_max_value + input_min_value) / 2;
            Double ratio = (input_max_value - input_min_value) / 2;
            Double[] normalization_data = input.Select(o => (o - offset) / ratio).ToArray();
            Double initial_phase = Math.Asin(normalization_data[0]);
            Boolean isposedge = IsPosEdge(normalization_data, oneUILength);
            vcophase[0] = isposedge == true ? initial_phase : Math.PI - initial_phase;
            vcoout[0] = Math.Sin(vcophase[0]);
            pdout[0] = 0;
            lfout[0] = lfgain * (b[0] * pdout[0]);

            for (Int32 i = 1; i < normalization_data.Length - 1; i++)
            {
                vcophase[i] = vcophase[i - 1] + (2 * Math.PI * vcoinitialfreq + vcogain * lfout[i - 1]) / fs;
                if (vcophase[i] > 2 * Math.PI)
                {
                    vcophase[i] -= 2 * Math.PI;
                }
                vcoout[i] = Math.Sin(vcophase[i]);
                pdout[i] = pdgain * normalization_data[i - 1] * vcoout[i - 1];
                for (Int32 j = 0; j < zero_length - 1; j++)
                {
                    lfout[i] += a[j] * (i >= j ? lfout[i - j] : 0);
                }
                for (Int32 k = 0; k < pole_length - 1; k++)
                {
                    lfout[i] += b[k] * (i >= k ? pdout[i - k] : 0);
                }
                lfout[i] *= lfgain;
            }
            return vcoout;
        }

        /// <summary>
        /// 黄金锁相环
        /// </summary>
        /// <param name="input">锁相环输入序列</param>
        /// <param name="fs">采样率</param>
        /// <param name="oneUILength">平均UI长度</param>
        /// <param name="cutoffDivisor">截止因子（默认为1667）</param>
        /// <returns>经过PLL后的序列</returns>
        public static Double[] FCGoldenPLL(Double[] input, Double fs, Double oneUILength, Double cutoffDivisor = 1667)
        {
            Double pdgain = 1.0;
            Double lfgain = 1.0;
            Double vcogain = 1.0;
            Double totalgain = pdgain * lfgain * vcogain;
            Double vcoinitialfreq = fs / (oneUILength * 2);
            Double nwc = cutoffDivisor;

            Double[] pdout = new Double[input.Length];
            Double[] lfout = new Double[input.Length];
            Double[] vcophase = new Double[input.Length];
            Double[] vcoout = new Double[input.Length];
            Double inputmaxvalue = input.Max();
            Double inputMinValue = input.Min();
            Double offset = (inputmaxvalue + inputMinValue) / 2;
            Double ratio = (inputmaxvalue - inputMinValue) / 2;
            Double[] normalizationdata = input.Select(o => (o - offset) / ratio).ToArray();

            Double initialphase = Math.Asin(normalizationdata[0]);
            Boolean isposedge = IsPosEdge(normalizationdata, oneUILength);
            vcophase[0] = isposedge == true ? initialphase : Math.PI - initialphase;
            vcoout[0] = Math.Sin(vcophase[0]);
            pdout[0] = 0;
            lfout[0] = lfgain * (1 / nwc * pdout[0]);
            Double freqinit = 2 * Math.PI * vcoinitialfreq;
            Double pdcoe = 1 / nwc;
            Double lfcoe = 1 - pdcoe;

            for (Int32 i = 1; i < normalizationdata.Length; i++)
            {
                vcophase[i] = vcophase[i - 1] + (freqinit + vcogain * lfout[i - 1]) / fs;
                vcoout[i] = Math.Sin(vcophase[i]);
                pdout[i] = pdgain * normalizationdata[i - 1] * vcoout[i - 1];

                lfout[i] = lfgain * (pdcoe * pdout[i] + lfcoe * lfout[i - 1]);
            }
            return vcoout;
        }

        private static Double[] SPLL_FCGolden(Double[] input, Double fs, Double cutoffDivisor = 1667)
        {
            Double pdcurrent = 0;
            Double lfcurrent = 0;
            Double vcocurrent = input[0];
            //Double pdLast;
            //Double lfLast;
            Double vcolast;

            Double nwc = cutoffDivisor;
            Double baseperiod = (input[^1] - input[0]) / (input.Length - 1);
            Double vcoinitialfreq = fs / (baseperiod * 2);
            Double lfgain = vcoinitialfreq / nwc / (2 * Math.PI);
            Double t = 1 / (fs / baseperiod);
            List<Double> output = new()
            {
                vcocurrent
            };

            for (Int32 i = 0; i < input.Length - 1; i++)
            {
                //pdLast = pdCurrent;
                //lfLast = lfCurrent;
                vcolast = vcocurrent;

                pdcurrent = input[i] - vcolast;
                lfcurrent = pdcurrent;
                vcocurrent = lfgain * lfcurrent * t + baseperiod + vcolast;
                output.Add(vcocurrent);
            }

            return output.ToArray();
        }

        /// <summary>
        /// 二阶锁相环
        /// </summary>
        /// <param name="input">锁相环输入序列</param>
        /// <param name="fs">采样率</param>
        /// <param name="oneUILength">平均UI长度</param>
        /// <param name="naturalFreq">自然频率</param>
        /// <param name="damplingFactor">阻尼因子</param>
        /// <returns>经过PLL后的序列</returns>
        public static Double[] SecondOrderPLL(Double[] input, Double fs, Double oneUILength, Double naturalFreq, Double damplingFactor)
        {
            Double pdgain = 1.0;
            Double lfgain = 1.0;
            Double vcogain = 1.0;
            Double vcoinitialfreq = fs / (oneUILength * 2);
            Double wn = naturalFreq;       //自然频率
            Double sigma = damplingFactor;//阻尼因子

            Double tau_1 = vcogain / (wn * wn);
            Double tau_2 = 2 * sigma / wn - 1 / vcogain;
            Double ts = 1 / fs;

            Double a1 = -(ts - 2 * tau_1) / (ts + 2 * tau_1);
            Double b0 = (ts + 2 * tau_2) / (ts + 2 * tau_1);
            Double b1 = (ts - 2 * tau_2) / (ts + 2 * tau_1);

            Double[] pdout = new Double[input.Length];
            Double[] lfout = new Double[input.Length];
            Double[] vcophase = new Double[input.Length];
            Double[] vcoout = new Double[input.Length];

            Double inputmaxvalue = input.Max();
            Double inputminvalue = input.Min();

            Double offset = (inputmaxvalue + inputminvalue) / 2;
            Double ratio = (inputmaxvalue - inputminvalue) / 2;
            Double[] normalizationdata = input.Select(o => (o - offset) / ratio).ToArray();

            Double initialphase = Math.Asin(normalizationdata[0]);
            Boolean isposedge = IsPosEdge(normalizationdata, oneUILength);
            vcophase[0] = isposedge == true ? initialphase : Math.PI - initialphase;
            vcoout[0] = Math.Sin(vcophase[0]);
            pdout[0] = 0;
            lfout[0] = lfgain * (1 / b0 * pdout[0]);

            for (Int32 i = 1; i < normalizationdata.Length - 1; i++)
            {
                vcophase[i] = vcophase[i - 1] + (2 * Math.PI * vcoinitialfreq + vcogain * lfout[i - 1]) * ts;
                if (vcophase[i] > 2 * Math.PI)
                {
                    vcophase[i] -= 2 * Math.PI;
                }
                vcoout[i] = Math.Sin(vcophase[i]);
                pdout[i] = pdgain * normalizationdata[i - 1] * vcoout[i - 1];
                lfout[i] = lfgain * (a1 * lfout[i - 1] + 1 / b0 * pdout[i] + b1 * pdout[i - 1]);
            }
            return vcoout;
        }

        private static Double[] SPLL_SecondOrder(Double[] input, Double fs, Double naturalFreq, Double damplingFactor = 0.707, Double lfGain = 1)
        {
            Double k = lfGain;
            Double tau1 = k / (naturalFreq * naturalFreq);
            Double tau2 = 2 * damplingFactor / naturalFreq;

            Double baseperiod = (input[^1] - input[0]) / (input.Length - 1);
            Double t = 1 / (fs / baseperiod);
            Double a1 = -1;
            Double b0 = (t + 2 * tau2) / (2 * tau1);
            Double b1 = (t - 2 * tau2) / (2 * tau1);

            Double pdcurrent = 0;
            Double lfcurrent = 0;
            Double vcocurrent = input[0];
            Double pdlast;
            Double lflast;
            Double vcolast;

            List<Double> output = new()
            {
                vcocurrent
            };

            for (Int32 i = 0; i < input.Length - 1; i++)
            {
                pdlast = pdcurrent;
                lflast = lfcurrent;
                vcolast = vcocurrent;

                pdcurrent = input[i] - vcolast;
                lfcurrent = -a1 * lflast + b0 * pdcurrent + b1 * pdlast;
                vcocurrent = lfGain * lfcurrent * t + baseperiod + vcolast;
                output.Add(vcocurrent);
            }

            return output.ToArray();
        }
        /// <summary>
        /// 判断第一个点位于上升沿还是下降沿
        /// </summary>
        /// <param name="input">样点序列</param>
        /// <param name="oneUILength">平均UI长度</param>
        /// <returns>若第一个点位于上升沿则返回true；否则返回false</returns>
        private static Boolean IsPosEdge(Double[] input, Double oneUILength)
        {
            Boolean flag;
            Double temp1 = input[(Int32)(oneUILength / 2 + 0.5)];//根据初始点加PI/2相位的值来判断
            //Double temp2 = Math.Cos(Math.Asin((input[0] - (input.Max() + input.Min()) / 2) / ((input.Max() - input.Min()) / 2)));
            if (temp1 >= 0)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public static Double[] ConstantClock(Double[] input)
        {
            if (input.Length < 1)
            {
                return Array.Empty<Double>();
            }
            Double[] x = new Double[input.Length];
            for (Int32 i = 0; i < x.Length; i++)
                x[i] = i;
            (Double k, Double b) = TIE.LinearFitting(x, input, input.Length);
            Double[] output = new Double[input.Length];
            for (Int32 i = 0; i < x.Length; i++)
                output[i] = k * x[i] + b;
            return output;
        }

        public static Double[] ConstantClockCubicSpline(Double[] input)
        {
            if (input.Length < 1)
            {
                return new Double[0];
            }
            Double[] x = new Double[input.Length];
            for (Int32 i = 0; i < x.Length; i++)
                x[i] = i;
            (Double k, Double b) = TIE.LinearFitting(x, input, input.Length);
            Double[] output = new Double[input.Length];
            for (Int32 i = 0; i < x.Length; i++)
                output[i] = k * x[i] + b;
            return output;
        }
    }

    /// <summary>
    /// PAM4边沿切割
    /// </summary>
    public static partial class ClockRecovery
    {
        /// <summary>
        /// 切割PAM4边沿
        /// </summary>
        /// <param name="sig">信号数组（码值）</param>
        /// <param name="lev0">0电平值（需要提前转换成码值）</param>
        /// <param name="lev1">1电平值（需要提前转换成码值）</param>
        /// <param name="lev2">2电平值（需要提前转换成码值）</param>
        /// <param name="lev3">3电平值（需要提前转换成码值）</param>
        /// <param name="UIPoints"></param>
        /// <returns>切割后的实际边沿和插值后的边沿数组</returns>
        public static (List<Double> InsertedEdge, List<Double> FinalEdge) FindPAM4Edge(Double[] sig, Double lev0, Double lev1, Double lev2, Double lev3, Double UIPoints)
        {
            Double[] levels = new Double[] { lev0, lev1, lev2, lev3 };
            List<Double> threholds = GetThresholds(levels);
            Double noiseThres = (lev3 - lev0) * 0.35;
            List<Double> signal = new List<Double>();
            if (sig.Length != 1)
            {
                signal = sig.Select(x => x).ToList();
            }

            Int32 addedPoints = (Int32)Math.Round(UIPoints, 0);
            signal = new List<Double>(Enumerable.Repeat(signal[0], addedPoints)
                                   .Concat(sig)
                                   .Concat(Enumerable.Repeat(signal[signal.Count - 1], addedPoints)));

            Int32 searchedPoints = (Int32)Math.Round(UIPoints / 2);

            var pL = FindCrossingPoints(signal, threholds[0]);//穿过低阈值的点
            var pM = FindCrossingPoints(signal, threholds[1]);//穿过中阈值的点
            var pH = FindCrossingPoints(signal, threholds[2]);//穿过高阈值的点

            //对穿过低阈值点分类
            List<Int32> validIndex01L = new List<Int32>();
            List<Int32> validIndex02L = new List<Int32>();
            List<Int32> validIndex03L = new List<Int32>();

            //对穿过中阈值点分类
            List<Int32> validIndex02M = new List<Int32>();
            List<Int32> validIndex03M = new List<Int32>();
            List<Int32> validIndex12M = new List<Int32>();
            List<Int32> validIndex13M = new List<Int32>();

            //对穿过高阈值点分类
            List<Int32> validIndex23H = new List<Int32>();
            List<Int32> validIndex13H = new List<Int32>();
            List<Int32> validIndex03H = new List<Int32>();

            foreach (var p in pL)
            {
                var rang = signal.GetRange(p - searchedPoints, 2 * searchedPoints);
                if (rang.All(x => x < threholds[1]))
                    validIndex01L.Add(p);
                else if (rang.All(x => x < threholds[2]))
                    validIndex02L.Add(p);
                else
                    validIndex03L.Add(p);
            }

            foreach (var p in pM)
            {
                var rang = signal.GetRange(p - searchedPoints, 2 * searchedPoints);

                if (rang.All(x => x < threholds[2]))
                {
                    if (rang.All(x => x > threholds[0]))
                        validIndex12M.Add(p);
                    else
                        validIndex02M.Add(p);
                }
                else
                {
                    if (rang.All(x => x > threholds[0]))
                        validIndex13M.Add(p);
                    else
                        validIndex03M.Add(p);
                }
            }

            foreach (var p in pH)
            {
                var rang = signal.GetRange(p - searchedPoints, 2 * searchedPoints);

                if (rang.All(x => x > threholds[1]))
                    validIndex23H.Add(p);
                else if (rang.All(x => x > threholds[0]))
                    validIndex13H.Add(p);
                else
                    validIndex03H.Add(p);
            }

            List<Int32> validindex01 = validIndex01L.OrderBy(x => x).ToList();
            List<Int32> validIndex03 = validIndex03M.OrderBy(x => x).ToList();
            List<Int32> validIndex12 = validIndex12M.OrderBy(x => x).ToList();
            List<Int32> validIndex23 = validIndex23H.OrderBy(x => x).ToList();
            List<Int32> validIndex02 = validIndex02L.Concat(validIndex02M).OrderBy(x => x).ToList();
            List<Int32> validIndex13 = validIndex13M.Concat(validIndex13H).OrderBy(x => x).ToList();

            validIndex02 = GetValidIndexes(validIndex02, signal, searchedPoints, noiseThres, lev1);

            validIndex13 = GetValidIndexes(validIndex13, signal, searchedPoints, noiseThres, lev2);

            List<Double> final01 = CalcFinalEdges(signal, validindex01, threholds[0], noiseThres, searchedPoints);
            List<Double> result03 = CalcFinalEdges(signal, validIndex03, threholds[1], noiseThres, searchedPoints);
            List<Double> result12 = CalcFinalEdges(signal, validIndex12, threholds[1], noiseThres, searchedPoints);
            List<Double> result23 = CalcFinalEdges(signal, validIndex23, threholds[2], noiseThres, searchedPoints);
            List<Double> result02 = CalcFinalEdges(signal, validIndex02, lev1, noiseThres, searchedPoints);
            List<Double> result13 = CalcFinalEdges(signal, validIndex13, lev2, noiseThres, searchedPoints);

            List<Double> finalEdge = new List<Double>(final01.Concat(result03).Concat(result12).Concat(result23).Concat(result02).Concat(result13));
            finalEdge.Sort();

            List<Double> diffEdge = new List<Double>();
            for (Int32 i = 0; i < finalEdge.Count; i++)
            {
                finalEdge[i] -= addedPoints;
            }

            for (Int32 i = 1; i < finalEdge.Count; i++)
            {
                diffEdge.Add(finalEdge[i] - finalEdge[i - 1]);
            }

            List<Double> insertedEdge = new List<Double>();
            for (Int32 i = 0; i < finalEdge.Count - 1; i++)
            {
                Double current_ui_points = finalEdge[i + 1] - finalEdge[i];
                Int32 insertEdgeNum = (Int32)Math.Round(current_ui_points / UIPoints) - 1;

                if (insertEdgeNum >= 1)
                {
                    List<Double> insertEdge = Enumerable.Range(1, insertEdgeNum)
                        .Select(x => finalEdge[i] + (x * current_ui_points / (insertEdgeNum + 1)))
                        .ToList();

                    insertedEdge.AddRange(insertEdge);
                }
                insertedEdge.Add(finalEdge[i]);
            }
            insertedEdge.Add(finalEdge.Last());
            insertedEdge = insertedEdge.OrderBy(x => x).ToList();
            // Returning result
            return (insertedEdge, finalEdge);
        }

        //根据已有电平计算阈值（按照电平间50%为阈值）
        private static List<Double> GetThresholds(Double[] levels)
        {
            List<Double> thresholds = new List<Double>();
            for (Int32 i = 0; i < levels.Length - 1; i++)
            {
                thresholds.Add((levels[i] + levels[i + 1]) / 2);
            }

            return thresholds;
        }

        //寻找跨越阈值的穿越点
        private static List<Int32> FindCrossingPoints(List<Double> sig, Double threshold)
        {
            List<Int32> crossingPoints = new List<Int32>();
            for (Int32 i = 1; i < sig.Count; i++)
            {
                if ((sig[i - 1] - threshold) * (sig[i] - threshold) < 0)
                {
                    crossingPoints.Add(i);
                }
            }
            return crossingPoints;
        }

        private static List<Int32> GetValidIndexes(List<Int32> validIndex, List<Double> sig, Double searchedPoints, Double noiseThres, Double level)
        {
            List<Int32> result = new List<Int32>();
            // Compute differences
            List<Double> tmpDiff = new List<Double>();
            for (Int32 i = 1; i < validIndex.Count; i++)
            {
                tmpDiff.Add(validIndex[i] - validIndex[i - 1]);
            }

            // Find split positions
            List<Int32> splitPos = new List<Int32> { 0 };
            for (Int32 i = 0; i < tmpDiff.Count; i++)
            {
                if (tmpDiff[i] > searchedPoints)
                {
                    splitPos.Add(i + 1);
                }
            }
            splitPos.Add(validIndex.Count);

            for (Int32 i = 0; i < splitPos.Count - 1; i++)
            {
                Int32 index = splitPos[i];

                if (Math.Abs(sig[validIndex[i] - 1] - sig[validIndex[i] - 2]) < noiseThres &&
                    Math.Abs(sig[validIndex[i] - 1] - sig[validIndex[i]]) < noiseThres)
                {
                    List<Double> segment = sig.GetRange(validIndex[index] - 1, validIndex[splitPos[i + 1] - 1] - validIndex[index] + 1);

                    segment = segment.Select(x => x - level).ToList();

                    for (Int32 j = 0; j < segment.Count - 1; j++)
                    {
                        if ((segment[j]) * (segment[j + 1]) < 0)
                        {
                            result.Add(validIndex[index] + j);
                        }
                    }
                }
            }

            return result;
        }

        //用于寻找跳变沿
        private static List<Double> CalcFinalEdges(List<Double> sig, List<Int32> validIndices, Double threshold, Double noiseThres, Int32 searchedPoInt32s)
        {
            List<Double> result = new List<Double>();
            for (Int32 i = 0; i < validIndices.Count; i++)
            {
                Int32 index = validIndices[i];

                // 确保索引不越界
                if (index > 0 && index < sig.Count - 1)
                {
                    // 判断前后相邻的差值是否小于噪声阈值
                    if (Math.Abs(sig[index] - sig[index - 1]) < noiseThres &&
                        Math.Abs(sig[index] - sig[index + 1]) < noiseThres)
                    {
                        // 计算 result01 中的值，并添加到列表中
                        Double value = index + (threshold - sig[index - 1]) / (sig[index] - sig[index - 1]);
                        result.Add(value);
                    }
                }
            }

            // 计算差值
            List<Double> tmp = new List<Double>();
            for (Int32 i = 1; i < result.Count; i++)
            {
                tmp.Add(result[i] - result[i - 1]);
            }

            // 查找分割位置
            List<Int32> splitPos = new List<Int32> { 0 }; // 初始化splitPos并加入0
            for (Int32 i = 0; i < tmp.Count; i++)
            {
                if (Math.Abs(tmp[i]) > searchedPoInt32s)
                {
                    splitPos.Add(i + 1);
                }
            }
            splitPos.Add(result.Count); // 在最后添加list的长度

            // 计算final01
            List<Double> final = new List<Double>();
            for (Int32 i = 0; i < splitPos.Count - 1; i++)
            {
                Int32 start = splitPos[i];
                Int32 end = splitPos[i + 1];
                Double sum = 0;
                for (Int32 j = start; j < end; j++)
                {
                    sum += result[j];
                }
                Double average = sum / (end - start);
                final.Add(average);
            }

            return final;
        }
    }

}
