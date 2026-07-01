using ScopeX.ComModel;
using ScopeX.Core.Model.Jitter.Eye;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.Jitter.Common
{
    internal class EyeCommon
    {
        //眼图的垂直分别率
        //internal readonly static Int32 EyeHight = Constants.MAX_ADC_RES;
        internal readonly static Int32 EyeHight = 1024;
        //眼图的水平分别率
        internal readonly static Int32 EyeWidth = 1920;

        internal readonly static Double CreateEyeByUICount = 2;//眼图绘制的UI数

        internal const Int32 ParallelNumber = 50_000;

        private static object _Lockobj = new object();

        internal static (Double[,] Matrix, Double[] Hist) GetEyeDiagramMatrixInterpInOriginalDataInParallel(FastEyeParams fastParams, Int32 eyeHeight, Int32 eyeWidth, Int32 dataHeight, Int32 dataWidth, Double low, Double step, Double[] samples, Double[] recoveredClockEdges, short taskNum = 4)
        {
            var edge_list = new List<Double[]>();
            //数据分段
            var data_len = (Int32)Math.Floor((Double)recoveredClockEdges.Length / taskNum);//每份数据单独处理
            for (var i = 0; i < taskNum; i++)
            {
                Int32 start_index = i * data_len - 1;
                if (start_index < 0) start_index = 0;
                Int32 end_index = start_index + data_len + 1;
                if (end_index >= recoveredClockEdges.Length) end_index = recoveredClockEdges.Length - 1;
                Double[] data = new Double[end_index - start_index + 1];
                Array.Copy(recoveredClockEdges, start_index, data, 0, end_index - start_index + 1);
                edge_list.Add(data);
            }

            List<(Double[,] Matrix, Double[] Hist)> results = new();

            Parallel.For(0, taskNum, (i) =>
            {
                var result = GetEyeDiagramMatrixTask(fastParams, eyeWidth, eyeHeight, dataHeight, dataWidth, low, step, samples, edge_list[i]);
                lock (_Lockobj)
                {
                    results.Add(result);
                }
            });

            //汇总聚合
            for (var i = 1; i < taskNum; i++)
            {
                for (var ii = 0; ii < eyeWidth; ii++)
                {
                    for (var iii = 0; iii < eyeHeight; iii++)
                    {
                        results[0].Matrix[ii, iii] += results[i].Matrix[ii, iii];
                    }
                }

                for (var ii = 0; ii < dataHeight; ii++)
                {
                    results[0].Hist[ii] += results[i].Hist[ii];
                }
            }
            return (results[0].Matrix, results[0].Hist);
        }

        private static (Double[,] Matrix, Double[] Hist) GetEyeDiagramMatrixTask(FastEyeParams fastParams, Int32 eyeHeight, Int32 eyeWidth, Int32 dataHeight, Int32 dataWidth, Double low, Double step, Double[] samples, Double[] recoveredClockEdges)
        {
            var matrix = GetEyeDiagramMatrixInterpInOriginalData(fastParams,eyeWidth, eyeHeight, dataHeight, dataWidth, 0, 1, samples, recoveredClockEdges, out Double[] eyeLevHist);

            return (matrix, eyeLevHist);
        }

        internal static Double[,] GetEyeDiagramMatrixInterpInOriginalData(FastEyeParams fastParams,Int32 eyeHeight, Int32 eyeWidth, Int32 dataHeight, Int32 dataWidth, Double low, Double step, Double[] samples, Double[] recoveredClockEdges, /*Double averageUIPoints, Double UILengthOnScreen,*/ out Double[] eyeLevHist)
        {
            Double[,] eye_diagram_matrix = new Double[eyeWidth, eyeHeight];
            eyeLevHist = new Double[dataHeight];
            try
            {
                Int32 points_on_screen = dataWidth;
                Int32 ui_number = recoveredClockEdges.Length - 1;
                Double[] current_ui_length = new Double[ui_number];
                Int32[] start_index = new Int32[ui_number];
                Int32[] end_index = new Int32[ui_number];
                Double points_each_side = 0;
                Int32 temp_x = 0;
                Int32 temp_y = 0;
                Boolean is_ui_first_piont = true;
                Double scale_ratio = 0;
                Int32 position_x = 0;
                Int32 position_y = 0;
                Int32 origi_x = 0;
                Int32 delta_x = 0;
                Int32 delta_y = 0;
                Int32 interp_num = 0;
                Double slope = 0;
                List<Int32> index = new List<Int32>();
                for (Int32 i = 0; i < ui_number; i++)
                {
                    if (fastParams != null && fastParams.IsFastEye && fastParams.CreateEyeUIIndex != null && fastParams.CreateEyeUIIndex.Length > 0 && !fastParams.CreateEyeUIIndex.Contains(i))
                    {
                        continue;
                    }

                    current_ui_length[i] = recoveredClockEdges[i + 1] - recoveredClockEdges[i];
                    points_each_side = Math.Round(points_on_screen - (current_ui_length[i] + 1));  //可以为负,为负是眼图绘制不到1个UI
                    points_each_side /= 2;
                    start_index[i] = (Int32)Math.Round(recoveredClockEdges[i] - points_each_side);//向左扩展
                    end_index[i] = (Int32)Math.Round(recoveredClockEdges[i + 1] + points_each_side);//向右扩展
                    temp_x = 0;
                    temp_y = 0;
                    is_ui_first_piont = true;
                    for (Int32 j = start_index[i]; j < end_index[i] + 3; j++)//+3为了防止眼图最右边出现空白
                    {
                        scale_ratio = points_on_screen / (Double)(end_index[i] - start_index[i] + 1);
                        position_x = (Int32)Math.Round((j - start_index[i]) * scale_ratio * ((eyeWidth - 1) / (Double)(dataWidth - 1)));
                        origi_x = (Int32)Math.Round((j - start_index[i]) * scale_ratio);
                        if (j < samples.Length && j >= 0 && position_x >= 0 && position_x < eyeWidth)
                        {
                            position_y = (Int32)((samples[j] - low) / step * (eyeHeight / (Double)dataHeight));

                            #region 路径插值
                            if (position_y >= 0 && position_y < dataHeight)
                            {
                                if (current_ui_length[i] > 1000)
                                {
                                    eye_diagram_matrix[position_x, position_y]++;
                                }
                                else
                                {
                                    if (is_ui_first_piont)
                                    {
                                        temp_x = position_x;
                                        temp_y = position_y;
                                        is_ui_first_piont = false;
                                        eye_diagram_matrix[position_x, position_y/**(eyeHeight/ (Double)dataHeight)*/]++;
                                    }
                                    else
                                    {
                                        delta_x = position_x - temp_x;
                                        delta_y = position_y - temp_y;
                                        interp_num = Math.Max(Math.Abs(delta_x), Math.Abs(delta_y));
                                        slope = (Double)(position_y - temp_y) / (position_x - temp_x);

                                        if (delta_x > Math.Abs(delta_y))
                                        {
                                            if (delta_y > 0)
                                            {
                                                for (Int32 k = 0; k < interp_num; k++)
                                                {
                                                    eye_diagram_matrix[temp_x + k, temp_y + (Int32)(k * slope)]++;
                                                }
                                            }
                                            else
                                            {
                                                for (Int32 k = 0; k < interp_num; k++)
                                                {
                                                    eye_diagram_matrix[temp_x + k, temp_y + (Int32)(k * slope)]++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (delta_y > 0)
                                            {
                                                for (Int32 k = 0; k < interp_num; k++)
                                                {
                                                    eye_diagram_matrix[temp_x + (Int32)(k / slope), temp_y + k]++;
                                                }
                                            }
                                            else
                                            {
                                                for (Int32 k = 0; k < interp_num; k++)
                                                {
                                                    eye_diagram_matrix[temp_x + (Int32)(k / Math.Abs(slope)), temp_y - k]++;
                                                }
                                            }

                                        }
                                        temp_x = position_x;
                                        temp_y = position_y;

                                    }
                                }
                            }

                            #endregion
                            if (origi_x >= dataWidth * 0.4 && origi_x <= dataWidth * 0.6)
                            {
                                eyeLevHist[position_y]++;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{ex.Message}\n{ex.StackTrace}", EventBus.LogLevel.Debug));
            }
            return eye_diagram_matrix;
        }


        internal static Double[,] ConvertToNBitMatrix(Double[,] source, uint colorStep)
        {
            if (colorStep < 8)
            {
                colorStep = 8;
            }
            if (colorStep > 256)
            {
                colorStep = 256;
            }
            Double max = source[0, 0];
            var l0 = source.GetLength(0);
            var l1 = source.GetLength(1);
            for (Int32 i = 0; i < l0; i++)
            {
                for (Int32 j = 0; j < l1; j++)
                {
                    max = source[i, j] > max ? source[i, j] : max;
                }
            }
            if (max == 0)
            {
                return source;
            }
            for (Int32 i = 0; i < l0; i++)
            {
                for (Int32 j = 0; j < l1; j++)
                {
                    source[i, j] = Math.Ceiling(source[i, j] / (max / colorStep)) * (max / colorStep);
                }
            }
            return source;
        }

        internal static Double[,] CutMatrix(Double[,] source)
        {
            //if (source.GetLength(1)<2)
            //{
            //    return source;
            //}
            //Double[,] dest = new Double[source.GetLength(0), source.GetLength(1)-1];
            //for (Int32 i = 0; i < source.GetLength(0); i++)
            //{
            //    for (Int32 j = 1; j < source.GetLength(1); j++)
            //    {
            //        dest[i, j-1] = source[i,j];
            //    }
            //}
            //return dest;

            for (Int32 i = 0; i < source.GetLength(0); i++)
            {
                source[i, 0] = 0;
            }
            return source;
        }

        internal static Double[,] CorrectMatrix(Double[,] eyeMatrix)
        {
            Double[,] temp = null;
            var l0 = eyeMatrix.GetLength(0);
            var l1 = eyeMatrix.GetLength(1);
            if (eyeMatrix != null && l0 > 0 && l1 > 0)
            {
                Double max = 0;

                for (Int32 i = 0; i < l0; i++)
                {
                    for (Int32 j = 0; j < l1; j++)
                    {
                        if (eyeMatrix[i, j] > max)
                        {
                            max = eyeMatrix[i, j];
                        }
                    }
                }

                if (max != 0)
                {
                    Int32 first_nonzero_row = FirstNonZeroRow(eyeMatrix, l0, l1);

                    Int32 last_nonzero_row = LastNonZeroRow(eyeMatrix, l0, l1);

                    if (first_nonzero_row != last_nonzero_row && first_nonzero_row != -1 && last_nonzero_row != -1)
                    {
                        Int32 total = last_nonzero_row - first_nonzero_row + 1;
                        Int32 correct = (Int32)Math.Ceiling(total * 1.25);
                        Int32 half;
                        if ((correct - total) % 2 != 0)//保证在眼图均匀居中
                        {
                            correct++;
                        }
                        temp = new Double[correct, l1];
                        half = (correct - total) / 2;
                        CopyRows(eyeMatrix, temp, first_nonzero_row, last_nonzero_row, half);
                    }
                }
            }
            if (temp == null)
            {
                temp = eyeMatrix;
            }

            return temp;
        }

        private static Int32 FirstNonZeroRow(Double[,] matrix, Int32 l0, Int32 l1)
        {
            Int32 first_nonzero_row = -1;
            for (Int32 i = 0; i < l0; i++)
            {
                Boolean found_nonzero = false;

                for (Int32 j = 0; j < l1; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        found_nonzero = true;
                        break; // 找到非零元素，退出内层循环
                    }
                }

                if (found_nonzero)
                {
                    first_nonzero_row = i;
                    break; // 找到第一个非零元素的行，退出外层循环
                }
            }

            return first_nonzero_row;
        }

        private static Int32 LastNonZeroRow(Double[,] matrix, Int32 l0, Int32 l1)
        {
            Int32 last_nonzero_row = -1;
            for (Int32 i = l0 - 1; i >= 0; i--)
            {
                Boolean found_nonzero = false;

                for (Int32 j = 0; j < l1; j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        found_nonzero = true;
                        break; // 找到非零元素，退出内层循环
                    }
                }

                if (found_nonzero)
                {
                    last_nonzero_row = i;
                    break; // 找到最后一个非零元素的行，退出外层循环
                }
            }

            return last_nonzero_row;
        }

        private static void CopyRows(Double[,] source, Double[,] destination, Int32 sourceStartRow, Int32 sourceEndRow, Int32 destinationStartRow)
        {
            Int32 row_count = sourceEndRow - sourceStartRow + 1;
            Int32 col_count = source.GetLength(1);

            for (Int32 i = 0; i < row_count; i++)
            {
                for (Int32 j = 0; j < col_count; j++)
                {
                    destination[destinationStartRow + i, j] = source[sourceStartRow + i, j];
                }
            }
        }



    }
}
