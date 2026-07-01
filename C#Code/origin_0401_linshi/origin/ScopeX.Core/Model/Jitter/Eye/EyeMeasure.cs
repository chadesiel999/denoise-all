using MathNet.Numerics.LinearAlgebra;
using ScopeX.Core.Model.Jitter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.Jitter.Eye
{
    internal static class EyeMeasure
    {

        public static EyePararmter NrzEyeMeasure(this NrzEyeGraphGenerator generator, NrzEyePrepare prepare)
        {
            GetEyeAmplitude(generator.EyeLevHist, out Int32 zerolevel, out Int32 onelevel, out Int32 eyeAmplitude, out Double eyeheight, out Double extinctionratio, out Double qFactor);
            GetEyeWidth(generator.EyeMatrix, generator.EyeLevHist, (Int32)Math.Round(prepare.AverageUILength * generator.EyeInterpRatio), zerolevel, onelevel, out var eyewidth, out var eyecrossratio);
            EyePararmter eye_pram = new EyePararmter();
            double height_scale = ((Double)ComModel.Constants.MAX_ADC_RES)/((Double)EyeCommon.EyeHight);
            eye_pram.ZeroLevel = (zerolevel* height_scale - generator.VirticalLevelPosition) * generator.VirticalScale / 1000;
            eye_pram.OneLevel = (onelevel* height_scale - generator.VirticalLevelPosition) * generator.VirticalScale / 1000;
            eye_pram.EyeAmplitude = eyeAmplitude * height_scale * generator.VirticalScale / 1000;
            eye_pram.EyeHeight = eyeheight * height_scale * generator.VirticalScale / 1000;
            eye_pram.EyeWidth = eyewidth * generator.EyeSampleInterval;
            eye_pram.EyeCrossRatio = eyecrossratio;
            eye_pram.ExtinctionRatio = extinctionratio;
            eye_pram.QFactor = qFactor;

            return eye_pram;
        }


        /// <summary>
        /// 获得眼幅度相关参数
        /// </summary>
        /// <param name="eyeLevHist">眼高直方图</param>
        /// <param name="zeroLevel">0电平</param>
        /// <param name="oneLevel">1电平</param>
        /// <param name="eyeAmplitude">眼幅度</param>
        /// <param name="eyeHeight">眼高</param>
        /// <param name="extincRatio">消光比</param>
        /// <param name="qFactor">Q因子</param>
        private static void GetEyeAmplitude(Double[] eyeLevHist, out Int32 zeroLevel, out Int32 oneLevel, out Int32 eyeAmplitude, out Double eyeHeight, out Double extincRatio, out Double qFactor)
        {
            zeroLevel = 0;
            oneLevel = 0;
            eyeAmplitude = 0;
            eyeHeight = 0;
            extincRatio = 0;
            qFactor = 0;

            if (eyeLevHist == null)
            {
                return;
            }

            #region 0电平，1电平，眼幅度

            var peaks = FindLevelIndexs(eyeLevHist);

            if (peaks == null || peaks.Count < 2)
                return;
            var one = peaks.First();
            var zero = peaks.Last();
            oneLevel = one.PaekIndex;
            zeroLevel = zero.PaekIndex;

            eyeAmplitude = oneLevel - zeroLevel;
            #endregion

            #region 眼高，消光比，Q因子
            Double sigma_zero = 0.0, sigma_one = 0.0;

            //去零点(异常) :当出现一个非零点后，往后4个点内，出现另一个非零点，则认为该非零点有效
            Int32 zero_eye_hight_start = zero.EndIndex;
            Int32 zero_eye_hight_end = zero.StartIndex;
            Int32 one_eye_hight_start = one.EndIndex;
            Int32 one_eye_hight_end = one.StartIndex;

            //求sigma时，默认0-1电平靠外的部分只受随机噪声的影响，可直接分析
            Double count = 0;
            Double level_sum = 0;
            for (Int32 i = 0; i < zeroLevel; ++i)//ZeroLevel 的sigma
            {
                if (level_sum > Double.MaxValue / 2)
                    break;
                level_sum += (i - zeroLevel) * (i - zeroLevel) * eyeLevHist[i];
                count += eyeLevHist[i];
            }
            sigma_zero = Math.Sqrt(level_sum / (count - 1));
            //  OneLevel的sigma
            count = 0;
            level_sum = 0;
            for (Int32 i = eyeLevHist.Length - 1; i > oneLevel; --i)
            {
                if (level_sum > Double.MaxValue / 2)
                    break;
                level_sum += (i - oneLevel) * (i - oneLevel) * eyeLevHist[i];
                count += eyeLevHist[i];
            }
            sigma_one = Math.Sqrt(level_sum / (count - 1));

            eyeHeight = oneLevel - zeroLevel - sigma_one * 3 - sigma_zero * 3;
            eyeHeight = Math.Max(eyeHeight, one_eye_hight_start - zero_eye_hight_end);//模拟自定义眼高

            extincRatio = 10 * Math.Log(oneLevel / (Double)zeroLevel);
            qFactor = (oneLevel - zeroLevel) / (sigma_one + sigma_zero);
            #endregion
        }

        private static void GetEyeAmplitude(Double[] eyeLevHist, out List<Int32> pam4Levels, out List<Int32> eyeAmplitude, out List<Double> eyeHeight)
        {
            pam4Levels = new List<Int32>() { 0, 0, 0, 0 };
            eyeAmplitude = new List<Int32>() { 0, 0, 0, 0 };
            eyeHeight = new List<Double>() { 0, 0, 0, 0 };

            var peaks = FindLevelIndexs(eyeLevHist);

            if (peaks == null || peaks.Count < 4)
                return;

            var threelevel = peaks[0];
            var twolevel = peaks[1];
            var onelevel = peaks[2];
            var zerolevel = peaks[3];

            pam4Levels[3] = threelevel.PaekIndex;
            pam4Levels[2] = twolevel.PaekIndex;
            pam4Levels[1] = onelevel.PaekIndex;
            pam4Levels[0] = zerolevel.PaekIndex;


            List<Double> sigma = new List<Double>();
           


        }

        private static List<(Int32 StartIndex, Int32 EndIndex, Int32 PaekIndex)> FindLevelIndexs(Double[] eyeLevHist)
        {
            // 存储所有最大值的索引
            List<(Int32 StartIndex, Int32 EndIndex, Int32 PaekIndex)> peak_indexs = new List<(Int32 StartIndex, Int32 EndIndex, Int32 PaekIndex)>();

            Int32 start_index = -1;
            Double max = -1;
            Int32 max_index = -1;
            Int32 zero_count = -1;

            // 从数组的末尾开始遍历
            for (Int32 i = eyeLevHist.Length - 1; i >= 0; i--)
            {
                // 1. 找到非零元素，记录 start_index
                if (start_index == -1 && eyeLevHist[i] != 0)
                {
                    start_index = i;
                }
                else if (start_index != -1 && eyeLevHist[i] == 0)
                {
                    Int32 ii = i;

                    while (eyeLevHist[ii] == 0)
                    {
                        zero_count++;
                        ii--;
                        if (ii < 0)
                        {
                            break;
                        }
                    }

                    if (zero_count >= 5)
                    {

                        for (Int32 iii = start_index; iii > i; iii--)
                        {
                            if (eyeLevHist[iii] > max)
                            {
                                max = eyeLevHist[iii];
                                max_index = iii;
                            }
                        }

                        if (max_index != -1)
                        {
                            peak_indexs.Add((start_index, i, max_index));
                            max_index = -1;
                            max = -1;
                        }

                        start_index = ii;

                    }

                    i = ii;
                    zero_count = 0;
                }

            }

            return peak_indexs;
        }

        /// <summary>
        /// 获得眼宽相关参数
        /// </summary>
        /// <param name="eyeDiagramMatrix">眼图矩阵</param>
        /// <param name="eyeLevHist">眼高直方图</param>
        /// <param name="one_ui_pixel">1个UI的像素点数</param>
        /// <param name="zeroLevel">0电平</param>
        /// <param name="oneLevel">1电平</param>
        /// <param name="eyeWidth">眼宽</param>
        /// <param name="crossRatio">眼交叉比</param>
        private static void GetEyeWidth(Double[,] eyeDiagramMatrix, Double[] eyeLevHist, Int32 one_ui_pixel, Int32 zeroLevel, Int32 oneLevel, out Double eyeWidth, out Double crossRatio)
        {
            if (eyeLevHist == null)
            {
                eyeWidth = 0;
                crossRatio = 0;
                return;
            }
            #region 眼宽
            //找垂直方向的中心位置
            //先从UI边沿的位置找交叉点起终范围：当左UI边沿和右UI边沿有一个非零即可
            Int32 eye_matrix_height = eyeDiagramMatrix.GetLength(1);
            Int32 eye_matrix_width = eyeDiagramMatrix.GetLength(0);
            Double[] eyeverhist = new Double[eye_matrix_width];
            one_ui_pixel = eye_matrix_width / 2;
            Int32 ui_left_side = (eye_matrix_width - one_ui_pixel) / 2;
            Int32 ui_right_side = (eye_matrix_width + one_ui_pixel) / 2;
            Int32 ui_side_start = 0, ui_side_end = 0;

            ui_side_start = zeroLevel;
            ui_side_end = oneLevel;
            //统计交叉范围内，每一行的0点个数，0点个数最多的一行即是垂直方向中心

            Int32[] eye_hist_over_eye_hight = new Int32[eye_matrix_height];
            for (Int32 j = ui_side_start; j <= ui_side_end; j++)
            {
                for (Int32 i = 0; i < eye_matrix_width; i++)
                {
                    if (eyeDiagramMatrix[i, j] != 0)
                        eye_hist_over_eye_hight[j] += 1;
                }
            }

            Int32 eye_width_line = 0;
            eye_width_line = ui_side_start;
            for (Int32 i = ui_side_start; i <= ui_side_end; ++i)
            {
                if (eye_hist_over_eye_hight[i] < eye_hist_over_eye_hight[eye_width_line])
                {
                    eye_width_line = i;
                }
            }


            Int32 verband_range = (Int32)(Math.Min(eye_width_line - zeroLevel, oneLevel - eye_width_line) * 0.01);//0.01这个值需根据数据量而定

            for (Int32 i = 0; i < eye_matrix_width; i++)
            {
                for (Int32 j = Math.Max(0, eye_width_line - verband_range); j <= Math.Min(eye_matrix_height - 1, eye_width_line + verband_range); j++)
                //for (Int32 j = 0; j < eyeMatrixHeight ; j++)
                {
                    eyeverhist[i] += eyeDiagramMatrix[i, j];
                }
            }

            // 去零点：找到眼宽的头尾 持续长度 
            Int32 left_eye_width_start = FindEfficientPoint(eyeverhist, 0, ui_left_side, 4);
            Int32 left_eye_width_end = FindEfficientPoint(eyeverhist, eye_matrix_width / 2, ui_left_side, 4);
            Int32 right_eye_width_start = FindEfficientPoint(eyeverhist, eye_matrix_width / 2, ui_right_side, 4);
            Int32 right_eye_width_end = FindEfficientPoint(eyeverhist, eye_matrix_width - 1, ui_right_side, 4);

            //找尾部      
            //直方图尾部是由随机抖动构成的高斯模型
            //由于眼图在绘制过程中进行了水平放缩，所以窄带直方图会出现栏栅式结构，这些由于放缩导致的空点会影响尾部的判断，并且会导致RMS过大

            Int32 range = 10;
            Int32 left_tail = FindTail(eyeverhist, left_eye_width_end, 0, range);
            Int32 right_tail = FindTail(eyeverhist, right_eye_width_start, eye_matrix_width - 1, range);

            //算sigma
            Double leye_width_cnt = 0;
            Double leye_width_rms = 0;
            for (Int32 i = left_eye_width_end; i >= left_tail; i--)
            {
                if (eyeverhist[i] > 100)                     //避免栅栏式直方图对RMS的影响，暂取100作为阈值
                {
                    leye_width_rms += (i - left_tail) * (i - left_tail);
                    leye_width_cnt++;
                }
            }
            if (leye_width_cnt != 0)
                leye_width_rms = Math.Sqrt(leye_width_rms / leye_width_cnt);

            Double reye_width_cnt = 0;
            Double reye_width_rms = 0;

            for (Int32 i = right_eye_width_start; i <= right_tail; i++)
            {
                if (eyeverhist[i] > 100)
                {
                    reye_width_rms += (i - right_tail) * (i - right_tail);
                    reye_width_cnt++;
                }
            }
            if (reye_width_cnt != 0)
                reye_width_rms = Math.Sqrt(reye_width_rms / reye_width_cnt);
            eyeWidth = right_tail - left_tail - 3 * leye_width_rms - 3 * reye_width_rms;
            eyeWidth = Math.Max(eyeWidth, right_eye_width_start - left_eye_width_end);

            if (eyeWidth < 0)
                eyeWidth = -1;
            #endregion

            //眼交叉比
            crossRatio = (eye_width_line - zeroLevel) / (Double)(oneLevel - zeroLevel) * 100;
        }


        /// <summary>
        /// 寻找大于零的有效点
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="start">开始寻找的索引值</param>
        /// <param name="end">截止的索引值</param>
        /// <param name="range">当找到一个大于零的点后，往后range个点内，出现另一个大于零的点，则认为该点有效 </param>
        /// <returns>从start索引开始寻找到的第一个有效点的索引</returns>
        private static Int32 FindEfficientPoint(Double[] data, Int32 start, Int32 end, Int32 range)
        {
            //去零点(异常) :当出现一个非零点后，往后range个点内，出现另一个非零点，则认为该非零点有效
            Int32 result = start;
            bool isok = false;
            if (start <= end)
            {
                for (; result < end; result++)
                {
                    if (data[result] > 0)
                    {
                        for (Int32 i = result + 1; i <= Math.Min(data.Length - 1, result + range); i++)
                        {
                            if (data[i] > 0)
                            {
                                isok = true;
                                break;
                            }
                        }
                        if (isok)
                            break;
                    }
                }
            }
            else
            {
                for (; result > end; result--)
                {
                    if (data[result] > 0)
                    {
                        for (Int32 i = result - 1; i >= Math.Max(0, result - range); i--)
                        {
                            if (data[i] > 0)
                            {
                                isok = true;
                                break;
                            }
                        }
                        if (isok)
                            break;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 直方图的尾部，也即峰值
        /// </summary>
        /// <param name="hist">直方图每个bin的值</param>
        /// <param name="start">开始寻找的索引值</param>
        /// <param name="end">截止的索引值</param>
        /// <param name="range">峰值之后range个点必须比峰值小</param>
        /// <returns>从start索引开始寻找到的第一个峰值的索引</returns>
        private static Int32 FindTail(Double[] hist, Int32 start, Int32 end, Int32 range)
        {
            start = start >= hist.Length ? hist.Length - 1 : start;
            end = end >= hist.Length ? hist.Length - 1 : end;
            Int32 result = start;
            bool isok;
            Double max = 0;
            if (start <= end)
            {
                for (Int32 i = start; i <= end; i++)
                {
                    if (hist[i] != 0 && hist[i] >= max)
                    {
                        max = hist[i];
                        isok = true;
                        for (Int32 j = i; j <= Math.Min(hist.Length - 1, i + range); j++)
                        {
                            if (hist[j] > hist[i])
                            {
                                isok = false;
                                break;
                            }
                        }
                        if (isok)
                        {
                            result = i;
                            break;
                        }

                    }
                }
            }
            else
            {
                for (Int32 i = start; i >= end; i--)
                {
                    if (hist[i] != 0 && hist[i] >= max)
                    {
                        max = hist[i];
                        isok = true;
                        for (Int32 j = i; j >= Math.Max(0, i - range); j--)
                        {
                            if (hist[j] > hist[i])
                            {
                                isok = false;
                                break;
                            }
                        }
                        if (isok)
                        {
                            result = i;
                            break;
                        }
                    }
                }
            }
            return result;

        }


    }
}
