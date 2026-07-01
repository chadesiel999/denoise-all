using ScopeX.ComModel;
using ScopeX.Core.Model.Jitter.Common;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.Jitter.Eye
{
    internal class EyeMethod
    {
        internal EyeMethod()
        {
            _NrzEyeGenerator = new NrzEyeGraphGenerator();
            _Pam4EyeGenerator=new Pam4EyeGraphGenerator();
        }

        public ChannelId Source { get; set; }

        public Boolean EyeEnable = false;

        public Boolean FastEye = true;

        public Boolean EyeParamEnable = false;

        public Dictionary<String, String> EyeParamTable = new Dictionary<String, String>() {
            {"ZeroLevel",MeasureHelper.MeasureEmpty },
            {"OneLevel",MeasureHelper.MeasureEmpty},
            {"EyeAmplitude",MeasureHelper.MeasureEmpty },
            {"EyeHeight",MeasureHelper.MeasureEmpty },
            {"EyeWidth",MeasureHelper.MeasureEmpty},
            {"ExtinctionRatio",MeasureHelper.MeasureEmpty },
            {"QFactor",MeasureHelper.MeasureEmpty },
            {"EyeCrossRatio",MeasureHelper.MeasureEmpty },
        };

        internal Boolean EyeAnalysisCompleted = true;

        internal Boolean CancelUpdateEyeData = false;

        private NrzEyeGraphGenerator _NrzEyeGenerator { get; set; }
        private Pam4EyeGraphGenerator _Pam4EyeGenerator { get; set; }

        #region Limit
        /// <summary>
        /// 最大支持的UI个数
        /// </summary>
        private readonly Double MaxUICount = 1_000_000;//一百万

        /// <summary>
        /// 最小支持的UI个数
        /// </summary>
        private readonly Double MinUICount = 8;

        /// <summary>
        /// 最小支持的UI长度
        /// </summary>
        private readonly Double MinUILength = 2.5;

        #endregion

        internal async Task EyeAnalysisTask(NrzEyePrepare prepare)
        {
            try
            {
                await Task.Run(() => EyeAnalysis(prepare));
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        internal (Double StepValue, Double Offset)? GetEyeVerticalParams(Double step, Double StepLength)
        {
            if (_NrzEyeGenerator.Amplitude != 0)
            {
                //占比80%
                Double stepValue = Math.Round(_NrzEyeGenerator.Amplitude * 1.25 / step, 3);

                Double center = Math.Round((_NrzEyeGenerator.OneLevel + _NrzEyeGenerator.ZeroLevel) / 2, 3);

                Double offset = Math.Ceiling(-center / stepValue * StepLength);

                return (stepValue, offset);
            }
            else
            {
                return null;
            }
        }

        private void EyeAnalysis(NrzEyePrepare prepare)
        {
            if (!EyeAnalysisCompleted)
            {
                return;
            }

            try
            {
                EyeAnalysisCompleted = false;
                Boolean run_eye = EyeEnable;
                if (EyeEnable)
                {
                    if (prepare.AverageUILength < MinUILength)
                    {
                        run_eye = false;
                    }

                    if (prepare.AverageUILength < MinUILength)
                    {
                        JitterCommon.LimitPrintJitterError(MsgTipId.CreateEyeError, 0);
                        run_eye = false;
                    }

                    if (prepare.UICount > MaxUICount)
                    {
                        JitterCommon.LimitPrintJitterError(MsgTipId.CreateEyeError, 0);
                        run_eye = false;
                    }

                    if (prepare.UICount < MinUICount)
                    {
                        JitterCommon.LimitPrintJitterError(MsgTipId.CreateEyeError, 0);
                        run_eye = false;
                    }
                }
                if (run_eye)
                {
                    run_eye = run_eye && _NrzEyeGenerator.CheckSysParams(prepare, Source);
                }

                if (run_eye)
                {
                    NrzEyeGraphGenerator.FastEyeParams = new FastEyeParams()
                    {
                        IsFastEye = FastEye
                    };
                    NrzEyeGraphGenerator.FastEyeParams.CreateEyeUIIndex = _NrzEyeGenerator.GetFastEyeUiIndexs(prepare);

                    var eyematrix = _NrzEyeGenerator.GetEyeMatrix(prepare);//耗时
                    if (!CancelUpdateEyeData)
                    {
                        JitterBuff.Default.Provide(Constants.DATA_JITTER_EYE,
                                      new Vector(eyematrix, QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                                      QuantityUnitExt.ToUnitString(QuantityUnit.Voltage),
                                      _NrzEyeGenerator.EyeSampleInterval,
                                      5000));

                        EyeMeasure();
                    }
                }
                else
                {
                    //清除眼图数据
                    ClearEyeUiBuffer();
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
            }
            finally
            {
                EyeAnalysisCompleted = true;
                CancelUpdateEyeData = false;
            }
        }

        internal async Task EyeAnalysisTask(Pam4EyePrepare prepare)
        {
            try
            {
                await Task.Run(() => EyeAnalysis(prepare));
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void EyeAnalysis(Pam4EyePrepare prepare)
        {
            if (!EyeAnalysisCompleted)
            {
                return;
            }

            try
            {
                EyeAnalysisCompleted = false;
                if (EyeEnable)
                {
                    var edges = ClockRecovery.FindPAM4Edge(prepare.SampleData, 85.99717285156251, 103.9971728515625, 121.9971728515625, 139.99717285156251, prepare.UILength);
                    prepare.InsertEdges = edges.InsertedEdge;
                    var eyematrix = _Pam4EyeGenerator.GetEyeMatrix(prepare);//耗时
                    if (!CancelUpdateEyeData)
                    {
                        JitterBuff.Default.Provide(Constants.DATA_JITTER_EYE,
                                      new Vector(eyematrix, QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                                      QuantityUnitExt.ToUnitString(QuantityUnit.Voltage),
                                      _NrzEyeGenerator.EyeSampleInterval,
                                      5000));

                        //EyeMeasure();
                    }
                }
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
            }
            finally
            {
                EyeAnalysisCompleted = true;
                CancelUpdateEyeData = false;
            }
        }

        private void EyeMeasure()
        {
            var eyeparam = GetEyePararmter();
            EyeParamTable["ZeroLevel"] = new Quantity(eyeparam.ZeroLevel, Prefix.Empty, QuantityUnit.Voltage).ToString("##0.###", true);
            EyeParamTable["OneLevel"] = new Quantity(eyeparam.OneLevel, Prefix.Empty, QuantityUnit.Voltage).ToString("##0.###", true);
            EyeParamTable["EyeAmplitude"] = new Quantity(eyeparam.EyeAmplitude, Prefix.Empty, QuantityUnit.Voltage).ToString("##0.###", true);
            EyeParamTable["EyeHeight"] = new Quantity(eyeparam.EyeHeight, Prefix.Empty, QuantityUnit.Voltage).ToString("##0.###", true);
            EyeParamTable["EyeWidth"] = new Quantity(eyeparam.EyeWidth, Prefix.Empty, QuantityUnit.Second).ToString("##0.###", true); ;
            EyeParamTable["ExtinctionRatio"] = eyeparam.ExtinctionRatio.ToString("##0.###");
            EyeParamTable["QFactor"] = eyeparam.QFactor.ToString("##0.###");
            EyeParamTable["EyeCrossRatio"] = new Quantity(eyeparam.EyeCrossRatio, Prefix.Empty, QuantityUnit.Percent).ToString("##0.##", true);
        }

        private EyePararmter GetEyePararmter()
        {
            EyePararmter ep = new();

            ep.ZeroLevel = _NrzEyeGenerator.ZeroLevel;
            ep.OneLevel = _NrzEyeGenerator.OneLevel;
            ep.EyeAmplitude = _NrzEyeGenerator.Amplitude;
            ep.EyeHeight = _NrzEyeGenerator.Height;
            ep.EyeWidth = _NrzEyeGenerator.Width;
            ep.QFactor = _NrzEyeGenerator.QFactor;
            ep.ExtinctionRatio = _NrzEyeGenerator.ExtinctionRatio;
            ep.EyeCrossRatio = _NrzEyeGenerator.CrossRatio;

            return ep;
        }

        internal void ClearEyeUiBuffer()
        {
            if (_NrzEyeGenerator?.ConstructionMode == StatisticalConstructionMode.Accumulation)
            {
                _NrzEyeGenerator?.ClearAccumulateEyeMatrix();
            }
            //清除眼图数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_EYE, new Vector(new Double[0, 0],
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Voltage),
                1, Constants.DEF_XPOS_IDX));

            ClearEyeMeasure();
        }

        private void ClearEyeMeasure()
        {
            EyeParamTable["ZeroLevel"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["OneLevel"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["EyeAmplitude"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["EyeHeight"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["EyeWidth"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["ExtinctionRatio"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["QFactor"] = MeasureHelper.MeasureEmpty;
            EyeParamTable["EyeCrossRatio"] = MeasureHelper.MeasureEmpty;
        }

        public void Dispose()
        {
            _NrzEyeGenerator?.Dispose();
        }
    }
}
