using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core.Model.Jitter.Common;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Core.Model.Jitter.Eye
{
    public class NrzEyeGraphGenerator
    {
        public NrzEyeGraphGenerator(Double jitterError = 0.1)
        {
            _JitterError = jitterError;
        }
        private static readonly object _Locker = new object();

        private Boolean _FirstAfterReset = false;

        public Double EyeSampleInterval { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }
        public Int32 EyeInterpRatio { get; set; }
        public Double ZeroLevel { get; set; }
        public Double OneLevel { get; set; }
        public Double Amplitude { get; set; }
        public Double QFactor { get; set; }
        public Double ExtinctionRatio { get; set; }
        public Double CrossRatio { get; set; }

        public Double SameSignalTolerate = 0.1;

        private ChannelId _Source = ChannelId.None;
        private Double _VerPosIndex = Double.NaN;
        private Double _VerBias = Double.NaN;
        private Double _HorScale = Double.NaN;
        private Double _VerScale = Double.NaN;

        public Int32 UICount;
        public StatisticalConstructionMode ConstructionMode = StatisticalConstructionMode.Accumulation;
        private Double _JitterError;
        public static FastEyeParams FastEyeParams;

        internal Double VirticalLevelPosition { get; set; }

        internal Double VirticalScale { get; set; }

        internal Double[,] EyeMatrix { get; set; } = new Double[EyeCommon.EyeWidth, EyeCommon.EyeHight];

        internal Double[] EyeLevHist { get; set; } = new Double[EyeCommon.EyeHight];

        public Boolean CheckSysParams(NrzEyePrepare prepare, ChannelId source)
        {
            var flag = true;
            if (_Source != ChannelId.None && source != _Source)
            {
                flag = false;
            }
            else
            {
                if (_VerPosIndex != Double.NaN && _VerPosIndex != prepare.DataParams.VerPosIndex
                    || _VerBias != Double.NaN && _VerBias != prepare.DataParams.VerBias
                    || _HorScale != Double.NaN && _HorScale != prepare.DataParams.HorScale
                    || _VerScale != Double.NaN && _VerScale != prepare.DataParams.VerScale)
                {
                    flag = false;
                }
            }

            _VerPosIndex = prepare.DataParams.VerPosIndex;
            _VerBias = prepare.DataParams.VerBias;
            _VerScale = prepare.DataParams.VerScale;
            _HorScale = prepare.DataParams.HorScale;
            return flag;
        }

        public Int32[] GetFastEyeUiIndexs(NrzEyePrepare prepare)
        {
            if (!FastEyeParams.IsFastEye || prepare.SampleData == null || prepare.SampleData.Length == 0 || prepare.TIEData.TIEs == null || prepare.TIEData.Length == 0)
                return new Int32[0];

            var edges_length = prepare.RecoveredEdges.Length;
            List<Int32> indexs = new List<Int32>();
            //获取最大\最小\绝对值最小的TIE的索引
            Int32 max_tie_index = -1;
            Int32 min_tie_index = -1;
            Int32 zero_tie_index = -1;

            Double max_tie = 0;
            Double min_tie = Double.MaxValue;
            Double abs_min_tie = Double.MaxValue;

            //找到数据最大值和最小值所在边沿的索引
            if (prepare.SampleData != null && prepare.SampleData.Length > 1)
            {
                Double max = prepare.SampleData[0];
                Double min = prepare.SampleData[0];
                Int32 max_data_index = 0;
                Int32 min_data_index = 0;

                for (Int32 i = 0; i < prepare.SampleData.Length; i++)
                {
                    if (prepare.SampleData[i] > max)
                    {
                        max = prepare.SampleData[i];
                        max_data_index = i;
                    }
                    if (prepare.SampleData[i] < min)
                    {
                        min = prepare.SampleData[i];
                        min_data_index = i;
                    }
                }


                if (prepare.RecoveredEdges[0] > max_data_index)
                {
                    indexs.Add(0);
                }
                else
                {
                    for (Int32 i = 0; i < edges_length - 1; i++)
                    {
                        if (prepare.RecoveredEdges[i] <= max_data_index && prepare.RecoveredEdges[i + 1] > max_data_index)
                        {
                            indexs.Add(i);
                            break;
                        }
                    }
                }
                if (prepare.RecoveredEdges[0] > min_data_index)
                {
                    indexs.Add(0);
                }
                else
                {
                    for (Int32 i = 0; i < edges_length - 1; i++)
                    {
                        if (prepare.RecoveredEdges[i] <= min_data_index && prepare.RecoveredEdges[i + 1] > min_data_index)
                        {
                            indexs.Add(i);
                            break;
                        }
                    }
                }
            }

            //找到构建眼图UI中心上升沿的最高点和最低点，下降沿的最高点和最低点
            var data_width = (Int32)Math.Ceiling(prepare.AverageUILength * EyeCommon.CreateEyeByUICount + 1);
            Double ui_length, points_each_side;
            //第一个边沿目前固定为上升沿，然后下降沿，依次交替
            Double raise_max = 0, raise_min = 0;
            Int32 raise_max_index = -1, raise_min_index = -1;
            Double fall_max = 0, fall_min = 0;
            Int32 fall_max_index = -1, fall_min_index = -1;
            for (Int32 i = 0; i < edges_length - 1; i++)
            {
                ui_length = prepare.RecoveredEdges[i + 1] - prepare.RecoveredEdges[i];
                points_each_side = Math.Round(data_width - (ui_length + 1));  //可以为负
                points_each_side /= 2;

                var position = (Int32)Math.Round(prepare.RecoveredEdges[i] + points_each_side);
                if (i % 2 == 0)//上升沿
                {
                    if (raise_max < prepare.SampleData[position])
                    {
                        raise_max = prepare.SampleData[position];
                        raise_max_index = i;
                    }
                    if (raise_min > prepare.SampleData[position])
                    {
                        raise_min = prepare.SampleData[position];
                        raise_min_index = i;
                    }
                }
                else//下降沿
                {
                    if (fall_max < prepare.SampleData[position])
                    {
                        fall_max = prepare.SampleData[position];
                        fall_max_index = i;
                    }
                    if (fall_min > prepare.SampleData[position])
                    {
                        fall_min = prepare.SampleData[position];
                        fall_min_index = i;
                    }
                }

                //找TIE
                var tie = prepare.TIEData.TIEs[i];
                var tie_abs = Math.Abs(tie);

                // 查找最大值
                if (tie > max_tie)
                {
                    max_tie = tie;
                    max_tie_index = i;
                }

                // 查找最小值
                if (tie < min_tie)
                {
                    min_tie = tie;
                    min_tie_index = i;
                }

                // 查找绝对最小值
                if (tie_abs < abs_min_tie)
                {
                    abs_min_tie = tie_abs;
                    zero_tie_index = i;
                }

            }

            if (raise_max_index != -1)
            {
                indexs.Add(raise_max_index);
            }
            if (raise_min_index != -1)
            {
                indexs.Add(raise_min_index);
            }
            if (fall_max_index != -1)
            {
                indexs.Add(fall_max_index);
            }
            if (fall_min_index != -1)
            {
                indexs.Add(fall_min_index);
            }

            if (max_tie_index != -1)
            {
                indexs.Add(max_tie_index);
            }
            if (min_tie_index != -1)
            {
                indexs.Add(min_tie_index);
            }
            if (zero_tie_index != -1)
            {
                indexs.Add(zero_tie_index);
            }

            var must_length = edges_length > 8 ? 8 : edges_length;

            for (int i = 0; i < must_length; i++)
            {
                indexs.Add(i);
            }

            return indexs.Distinct().OrderBy(x => x).ToArray();
        }

        public Double[,] GetEyeMatrix(NrzEyePrepare prepare)
        {
            lock (_Locker)
            {
                Double[,] eye_unit_matrix;
                var result = GetSingleEyeMatrix(prepare);

                if (ConstructionMode == StatisticalConstructionMode.Accumulation)
                {
                    AccumulateEyeMatrix(prepare, result.Matrix, result.Hist);
                }
                else
                {
                    EyeMatrix = result.Matrix;
                    EyeLevHist = result.Hist;
                }

                UpdateEyeParameter(prepare);

                var reversed_matrix = EyeMatrix.RotateAnticlockwise();

                reversed_matrix = EyeCommon.ConvertToNBitMatrix(reversed_matrix, 25);//色阶问题
                //reversedMatrix = ConvertToHigherMatrix(reversedMatrix);
                //reversedMatrix = ConvertToTestMatrix(reversedMatrix);
                reversed_matrix = EyeCommon.CutMatrix(reversed_matrix);

                return EyeCommon.CorrectMatrix(reversed_matrix);
            }
        }

        /// <summary>
        /// NRZ眼图叠加
        /// </summary>
        /// <param name="prepare"></param>
        /// <returns></returns>
        public (Double[,] Matrix, Double[] Hist) GetSingleEyeMatrix(NrzEyePrepare prepare)
        {
            Double tie_mean = prepare.TIEData.Average;
            // 计算一个UI的点数
            //Boolean isData = ClockRecovery.IsDataSignal(recoveredEdges, out Double uiPoints);
            //ref edge
            Double[] eye_ref_edges_array = prepare.RecoveredEdges.Select(o => o + tie_mean).ToArray();

            // 实际计算出来的时基档和对应的UI长度
            Double[] interpolated_data = prepare.SampleData;
            Double interpolated_ui_length = prepare.AverageUILength;
            EyeInterpRatio = 1;

            Int32 data_width = (Int32)Math.Ceiling(interpolated_ui_length * EyeCommon.CreateEyeByUICount + 1);
            Int32 data_height = Constants.MAX_ADC_RES;

            Double[,] matrix = new Double[EyeCommon.EyeWidth, EyeCommon.EyeHight];
            Double[] levhist = new Double[EyeCommon.EyeHight];

            #region 耗时部分，眼图矩阵的叠加
            var mid = (prepare.HighLevel + prepare.LowLevel) * prepare.Threshold / 100;
            PlatformManager.Default.Platform.GetEyeGraphParams(ref FastEyeParams, prepare.AverageUILength, data_height, data_width, interpolated_data, eye_ref_edges_array, out matrix, out levhist);
           
            #endregion


            EyeSampleInterval = 1 / prepare.Fs / EyeCommon.EyeWidth * EyeCommon.CreateEyeByUICount * interpolated_ui_length / EyeInterpRatio;

            return (matrix, levhist);
        }

        public void AccumulateEyeMatrix(NrzEyePrepare prepare, Double[,] new_matrix, Double[] levHist)
        {
            if (_FirstAfterReset == true)
            {
                EyeMatrix = new_matrix;
                EyeLevHist = levHist;
                _FirstAfterReset = false;
            }
            else
            {
                UICount += prepare.UICount;

                for (Int32 i = 0; i < EyeMatrix.GetLength(0); i++)
                {
                    for (Int32 j = 0; j < EyeMatrix.GetLength(1); j++)
                    {
                        EyeMatrix[i, j] += new_matrix[i, j];
                    }
                }

                for (Int32 i = 0; i < EyeLevHist.Length; i++)
                {
                    EyeLevHist[i] += levHist[i];
                }
            }
        }

        public void ClearAccumulateEyeMatrix()
        {
            UICount = 0;
            Array.Clear(EyeMatrix, 0, EyeMatrix.Length);
            Array.Clear(EyeLevHist, 0, EyeLevHist.Length);
            _FirstAfterReset = true;
        }

        private void UpdateEyeParameter(NrzEyePrepare prepare)
        {
            //EyeMeasure
            VirticalLevelPosition = _VerPosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            VirticalScale = _VerScale / Constants.SAMPS_PER_YDIV;
            var result = this.NrzEyeMeasure(prepare);
            ZeroLevel = result.ZeroLevel;
            OneLevel = result.OneLevel;
            Amplitude = result.EyeAmplitude;
            Height = result.EyeHeight;
            Width = result.EyeWidth;
            CrossRatio = result.EyeCrossRatio;
            ExtinctionRatio = result.ExtinctionRatio;
            QFactor = result.QFactor;
        }

        public void Dispose()
        {
            Array.Clear(EyeMatrix, 0, EyeMatrix.Length);
            Array.Clear(EyeLevHist, 0, EyeLevHist.Length);
        }

    }

    public class Pam4EyeGraphGenerator
    {
        private static readonly object _Locker = new object();

        private Boolean _FirstAfterReset = false;

        internal Double[,] EyeMatrix { get; set; } = new Double[EyeCommon.EyeWidth, EyeCommon.EyeHight];
        internal Double[] EyeLevHist { get; set; } = new Double[EyeCommon.EyeHight];

        public Int32 UICount;

        public Double EyeSampleInterval { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }
        public Int32 EyeInterpRatio { get; set; }
        public Double ZeroLevel { get; set; }
        public Double OneLevel { get; set; }
        public Double Amplitude { get; set; }
        public Double QFactor { get; set; }
        public Double ExtinctionRatio { get; set; }
        public Double CrossRatio { get; set; }

        public StatisticalConstructionMode ConstructionMode = StatisticalConstructionMode.Single;

        public Double[,] GetEyeMatrix(Pam4EyePrepare prepare)
        {
            lock (_Locker)
            {
                Double[,] eye_unit_matrix;
                var result = GetSingleEyeMatrix(prepare);

                if (ConstructionMode == StatisticalConstructionMode.Accumulation)
                {
                    AccumulateEyeMatrix(prepare, result.Matrix, result.Hist);
                }
                else
                {
                    EyeMatrix = result.Matrix;
                    EyeLevHist = result.Hist;
                }

                var flag = false;

                if (flag)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "EyeLevHist.txt")))
                    {
                        foreach (Double h in EyeLevHist)
                        {
                            sw.WriteLine(h);
                        }
                    }
                }

                //进行眼图测量然后更新结果
                //EyeCommon.UpdateEyeParameter(prepare);

                var reversed_matrix = EyeMatrix.RotateAnticlockwise();

                reversed_matrix = EyeCommon.ConvertToNBitMatrix(reversed_matrix, 25);//色阶问题
                reversed_matrix = EyeCommon.CutMatrix(reversed_matrix);

                return EyeCommon.CorrectMatrix(reversed_matrix);
            }
        }

        public void AccumulateEyeMatrix(Pam4EyePrepare prepare, Double[,] new_matrix, Double[] levHist)
        {
            if (_FirstAfterReset == true)
            {
                EyeMatrix = new_matrix;
                EyeLevHist = levHist;
                _FirstAfterReset = false;
            }
            else
            {
                UICount += (Int32)Math.Round(prepare.SampleData.Length / prepare.UILength, 0);

                for (Int32 i = 0; i < EyeMatrix.GetLength(0); i++)
                {
                    for (Int32 j = 0; j < EyeMatrix.GetLength(1); j++)
                    {
                        EyeMatrix[i, j] += new_matrix[i, j];
                    }
                }

                for (Int32 i = 0; i < EyeLevHist.Length; i++)
                {
                    EyeLevHist[i] += levHist[i];
                }
            }
        }

        public (Double[,] Matrix, Double[] Hist) GetSingleEyeMatrix(Pam4EyePrepare prepare)
        {
            // 实际计算出来的时基档和对应的UI长度
            Double[] interpolated_data = prepare.SampleData;
            Double interpolated_ui_length = prepare.UILength;
            EyeInterpRatio = 1;

            Int32 data_width = (Int32)Math.Ceiling(interpolated_ui_length * EyeCommon.CreateEyeByUICount + 1);
            Int32 data_height = Constants.MAX_ADC_RES;

            Double[,] matrix = new Double[EyeCommon.EyeWidth, EyeCommon.EyeHight];
            Double[] levhist = new Double[EyeCommon.EyeHight];

            var eye_ref_edges_array = prepare.InsertEdges.ToArray();

            #region 耗时部分，眼图矩阵的叠加
            if (eye_ref_edges_array.Length * prepare.UILength < EyeCommon.ParallelNumber)
            {
                matrix = EyeCommon.GetEyeDiagramMatrixInterpInOriginalData(null, EyeCommon.EyeHight, EyeCommon.EyeWidth, data_height, data_width, 0, 1, interpolated_data, eye_ref_edges_array, /*interpolatedUILength, actualUILength,*/ out levhist);
            }
            else
            {
                var task = EyeCommon.GetEyeDiagramMatrixInterpInOriginalDataInParallel(null, EyeCommon.EyeHight, EyeCommon.EyeWidth, data_height, data_width, 0, 1, interpolated_data, eye_ref_edges_array);
                matrix = task.Matrix;
                levhist = task.Hist;
            }
            #endregion


            EyeSampleInterval = 1 / prepare.Fs / EyeCommon.EyeWidth * EyeCommon.CreateEyeByUICount * interpolated_ui_length / EyeInterpRatio;

            return (matrix, levhist);
        }

        public void Dispose()
        {
            Array.Clear(EyeMatrix, 0, EyeMatrix.Length);
            Array.Clear(EyeLevHist, 0, EyeLevHist.Length);
        }


    }

    public class FastEyeParams
    {
        public Boolean IsFastEye { get; set; }
        public Int32[] CreateEyeUIIndex { get; set; }
    }
}
