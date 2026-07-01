using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class SamplingPrsnt : ISampling
    {
        private protected virtual SamplingModel Model
        {
            get;
        }

        internal SamplingPrsnt(SamplingModel model)
        {
            Model = model;
        }

        public Double PositionByus => Model.Position;

        public Double MaxPosition => Model.MaxPosition;

        public Double MinPosition => Model.MinPosition;

        public Double PosIdxPerDiv => Model.PosIdxPerDiv;

        public Double Scale
        {
            get
            {
                return Model.Scale;
            }
            set
            {
                var oldscale = Model.Scale;
                var oldposindex = Model.PosIndex;
                Model.Scale = Math.Round(value,6);
                UpdateVCursorsPosByScale(oldscale, Model.Scale, oldposindex);

            }
        }

        public Double AutoScale
        {
            get
            {
                return Model.AutoScale;
            }
            set
            {
                Model.AutoScale = value;
            }
        }

        public Double MaxScale => Model.MaxScale;

        public Double MinScale => Model.MinScale;

        public Double QueryScaleValue(Int32 ScaleIndex)
        {
            return Model.GetScaleValue(ScaleIndex, 0);
        }

        #region ISampling

        public Double PosIndexBymDiv
        {
            get => Model.PosIndex;
            set
            {
                if (Model.PosIndex != value)
                {
                    var oldposindex = Model.PosIndex;
                    Model.PosIndex = value;
                    UpdateVCursorsPosByPoindex(oldposindex, Model.PosIndex);
                    Dispatcher.SoftReset();
                }
            }
        }

        public Double PosMaxIndex => Model.PosMaxIndex;

        public Double PosMinIndex => Model.PosMinIndex;

        //public Double PosIdxPerDiv => Model.PosIdxPerDiv;

        public void ResetPosIndex()
        {
            var oldposindex = Model.PosIndex;
            Model.PosIndex = Model.PosDefIndex;
            UpdateVCursorsPosByPoindex(oldposindex, Model.PosIndex);
            Dispatcher.SoftReset();
        }

        public Int32 ScaleIndex
        {
            get => Model.ScaleIndex;
            set
            {
                if (Model.ScaleIndex != value)
                {
                    var oldscale = Model.Scale;
                    var oldposindex = Model.PosIndex;
                    Model.ScaleIndex = value;
                    UpdateVCursorsPosByScale(oldscale, Model.Scale, oldposindex);
                    Dispatcher.SoftReset();
                }
            }
        }

        public Double ScaleMaxIndex => Model.ScaleMaxIndex;

        public Double ScaleMinIndex => Model.ScaleMinIndex;

        public Prefix Prefix
        {
            get
            {
                return Model.Prefix;
            }
            set
            {
                Model.Prefix = value;
            }
        }

        public String Unit
        {
            get => Model.Unit;
            set => Model.Unit = value;
        }
        #endregion

        public (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) GetCurrentTmbInfo()
        {
            return (PositionByus, Model.PosIndex, Model.Scale, Model.PosIdxPerDiv);
        }

        private void UpdateVCursorsPosByPoindex(Double oldPosIndex, Double currentPosIndex)
        {
            if (DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }
            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Int32)DsoModel.Default.Cursors.VCursor[0] + currentPosIndex - oldPosIndex);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Int32)DsoModel.Default.Cursors.VCursor[1] + currentPosIndex - oldPosIndex);
            }
        }

        private void UpdateVCursorsPosByScale(Double oldScale, Double currentScale, Double oldPosIndex)
        {
            if (DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }

            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Double)(DsoModel.Default.Cursors.VCursor[0] - oldPosIndex) * oldScale / currentScale + Model.PosIndex);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Double)(DsoModel.Default.Cursors.VCursor[1] - oldPosIndex) * oldScale / currentScale + Model.PosIndex);
            }
        }

        private void UpdateVCursorPosByTmbPosition(Double oldPosByus)
        {
            if (!DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }

            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Int32)DsoModel.Default.Cursors.VCursor[0] - (Model.Position - oldPosByus) * Constants.IDX_PER_XDIV / Model.Scale);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Int32)DsoModel.Default.Cursors.VCursor[1] - (Model.Position - oldPosByus) * Constants.IDX_PER_XDIV / Model.Scale);
            }
        }
    }

    public class TimebasePrsnt : MulticastPrsnt<ITimebaseView>, ITimebasePrsnt
    {
        private protected override TimebaseModel Model
        {
            get;
        }

        public TimebasePrsnt(IDsoPrsnt idp, ITimebaseView? view) : base(idp)
        {
            Model = DsoModel.Default.Timebase;
            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public AnaChnlAcqMode Mode
        {
            get => Model.Mode;
            set
            {
                if (StorageMode == AnaChnlStorageMode.Fast && (value == AnaChnlAcqMode.Average || value == AnaChnlAcqMode.Envelope))
                {
                    WeakTip.Default.Write("Timebase", MsgTipId.AcqModeCannotSet);
                }
                else
                {
                    Model.Mode = value;
                    Dispatcher.SoftReset();//采样模式改变时应该要重新刷新波形    Scan模式下尤为明显
                    Hardware.HdCmdFactory.Push(HdCmd.TmbPosition | HdCmd.TmbScaleIndex | HdCmd.TmbStorageLen);
                }
            }
        }

        public AnaChnlStorageMode StorageMode
        {
            get => Model.StorageMode;
            set
            {
                if (Model.StorageMode != value)
                {
                    if (/*IsScan*/ScaleIndex >= DsoModel.Default.Timebase.ScanMinIndex && value == AnaChnlStorageMode.Fast)
                    {
                        WeakTip.Default.Write("IsScan", MsgTipId.ScanIsNotSupportedInFast);
                        return;
                    }
                    if(DsoPrsnt.DefaultDsoPrsnt.IsDemoMode())
                    {
                        WeakTip.Default.Write("IsScan", MsgTipId.DemoIsNotSupportedInFast);
                        return;
                    }
                    Model.StorageMode = value;
                    Dispatcher.SoftReset();
                    Hardware.HdCmdFactory.Push(HdCmd.TmbStorageLen);
                    KeyLed.Default.SetFastAcq(value);
                }
            }
        }

        public Boolean FastEnable
        {
            get => StorageMode == AnaChnlStorageMode.Fast;
            set => StorageMode = value ? AnaChnlStorageMode.Fast : AnaChnlStorageMode.Long;
        }

        public Boolean EnhancedBitsActive
        {
            get => Model.EnhancedBitsActive;
            set
            {
                if (Model.EnhancedBitsActive != value)
                {
                    Model.EnhancedBitsActive = value;
                    Hardware.HdCmdFactory.Push(HdCmd.EnhancedBits);
                    //Hd.ConfigEResFIRCoefficients(EnhancedBits, value);
                }
            }
        }

        public Boolean ConstAutoStorageActive
        {
            get => Model.ConstAutoStorageActive;
            set
            {
                if (Model.ConstAutoStorageActive != value)
                {
                    Model.ConstAutoStorageActive = value;
                    //Hardware.HdCmdFactory.Push(HdCmd.EnhancedBits);
                }
            }
        }

        public Double EnhancedBits
        {
            get => Model.EnhancedBits;
            set
            {
                value = ValidateEnhancedBits(value);
                if (Model.EnhancedBits != value)
                {
                    Model.EnhancedBits = value;
                    Hardware.HdCmdFactory.Push(HdCmd.EnhancedBits);
                    //Hd.ConfigEResFIRCoefficients(value, Model.EnhancedBitsActive);
                }
            }
        }

        private static Double ValidateEnhancedBits(Double value)
        {
            value = Math.Round(value / Constants.StepEnhancedBit, MidpointRounding.AwayFromZero) * Constants.StepEnhancedBit;
            if (value > Constants.MaxEnhancedBits)
                value = Constants.MaxEnhancedBits;
            else if (value < Constants.MinEnhancedBits)
                value = Constants.MinEnhancedBits;
            return value;
        }

        public void AdjEnhancedBits(Int32 step)
        {
            EnhancedBits += step * Constants.StepEnhancedBit;
        }

        public Int32 AverageCnt
        {
            get => Model.AverageCnt;
            set => Model.AverageCnt = value;
        }

        public Int32 AverageMaxCnt => Model.AverageMaxCnt;

        public Int32 AverageMinCnt => Model.AverageMinCnt;

        public EvlpOpt EnvelopOpt
        {
            get => Model.EnvelopOpt;
            set => Model.EnvelopOpt = value;
        }

        public Int32 EnvelopeCnt
        {
            get => Model.EnvelopeCnt;
            set => Model.EnvelopeCnt = value;
        }

        public Int32 EnvelopeMaxCnt => Model.EnvelopeMaxCnt;

        public Int32 EnvelopeMinCnt => Model.EnvelopeMinCnt;

        public AnaChnlClkSrc ClockSrc
        {
            get => Model.ClockSrc;
            set
            {
                Model.ClockSrc = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TmbScaleIndex);
            }
        }
        public Boolean Ext10MHzLocked
        {
            get => Model.Ext10MHzLocked;
            set
            {
                Model.Ext10MHzLocked = value;
            }
        }
        public AdcInterleaveMode InterleaveMode
        {
            get => Model.InterleaveMode;
            set
            {
                Model.InterleaveMode = value;
                // 交织模式改变时
                // 1、刷新一下区域触发参数配置
                // 2、刷新一下通道延迟参数配置
                Hardware.HdCmdFactory.Push(HdCmd.InterleaveMode);
            }
        }
        public Double AnalogSamplingRate
        {
            get => Model.AnalogSamplingRate;
            set
            {
                Model.AnalogSamplingRate = value;
            }
        }
        public AnaChnlItplType InterplType
        {
            get => Model.InterplType;
            set => Model.InterplType = value;
        }

        public Boolean EnableRIS
        {
            get => Model.EnableRIS;
            set
            {
                Model.EnableRIS = value;

                //!!!Temp code, useless
                if (value)
                {
                    DsoModel.Default.Display.Persist = WfmPersist.Auto;
                    DsoModel.Default.Display.DrawMode = WfmDrawMode.Dot;
                }
                else
                {
                    DsoModel.Default.Display.Persist = WfmPersist.Close;
                    DsoModel.Default.Display.DrawMode = WfmDrawMode.Vector;
                }
            }
        }

        public Double PositionByus
        {
            get => Model.Position;
            set
            {
                if (ScaleIndex >= DsoModel.Default.Timebase.ScanMinIndex && TriggerModel.State != SysState.Stop)
                    return;
                var oldposbyus = Model.Position;
                Model.Position = value;

                UpdateVCursorPosByTmbPosition(oldposbyus);

                Dispatcher.SoftReset();
                Model.ScaleOrPosUpdateTime = DateTime.Now;
                Hardware.HdCmdFactory.Push(HdCmd.TmbPosition);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
            }
        }

        public Double MaxPosition => Model.MaxPosition;

        public Double MinPosition => Model.MinPosition;

        public Double PosIdxPerDiv => Model.PosIdxPerDiv;

        public Double ScaleByus
        {
            get => Model.Scale;
            set
            {
                var old = Model.ScaleIndex;
                var oldscalebyus = Model.Scale;
                var oldposindex = Model.PosIndex;

                Model.CheckAndChangedIndexRange(GetCurrentIndexLimit);

                var temp = value;
                var @new = Model.GetTimebaseIndexByValue(temp);
                @new = IndexLimit(@new);
                if (old != @new)
                {
                    Model.ScaleIndex = @new;
                    UpdateVCursorsPosByScale(oldscalebyus, Model.Scale, oldposindex);
                }
                Dispatcher.SoftReset();
                Model.ScaleOrPosUpdateTime = DateTime.Now;
                Hardware.HdCmdFactory.Push(HdCmd.TmbScaleIndex);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
            }
        }

        public Double MaxScale
        {
            get
            {
                Model.CheckAndChangedIndexRange(GetCurrentIndexLimit);
                return Model.MaxScale;
            }
        }

        public Double MinScale
        {
            get
            {
                Model.CheckAndChangedIndexRange(GetCurrentIndexLimit);
                return Model.MinScale;
            }
        }
        public Boolean IsZoom
        {
            get => Model.IsZoom;
            set => Model.IsZoom = value;
        }
        public Double ZoomCenterX
        {
            get => Model.ZoomCenterX;
            set => Model.ZoomCenterX = value;
        }
        public Double ZoomCenterY
        {
            get => Model.ZoomCenterY;
            set => Model.ZoomCenterY = value;
        }
        public Double ZoomScaleX
        {
            get => Model.ZoomScaleX;
            set => Model.ZoomScaleX = value;
        }
        public Double ZoomScaleY
        {
            get => Model.ZoomScaleY;
            set => Model.ZoomScaleY = value;
        }

        public Double ZoomScaleX2SCPI
        {
            get => 1D / Model.ZoomScaleX;
            set => Model.ZoomScaleX = 1D / value;
        }
        public Double ZoomScaleY2SCPI
        {
            get => 1D / Model.ZoomScaleY;
            set => Model.ZoomScaleY = 1D / value;
        }

        #region ITimebasePrsnt       
        //Default stride is 1000/div, so its name suffix "BymDiv", but the corresponding model name does not.
        public Double PosIndexBymDiv
        {
            get => Model.PosIndex;
            set
            {
                if (IsScan && TriggerModel.State != SysState.Stop)
                    return;
                var oldposindex = Model.PosIndex;
                Model.PosIndex = value;
                Dispatcher.SoftReset();
                Model.ScaleOrPosUpdateTime = DateTime.Now;
                DsoModel.Default?.MathChnls?.Where(x => x.Active && x.Args?.Type == MathType.Track).ToList().ForEach(x =>
                {
                    x.Sampling.PosIndex = value;
                });
                Hardware.HdCmdFactory.Push(HdCmd.TmbPosition);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
                UpdateVCursorsPosByPoindex(oldposindex, Model.PosIndex);
            }
        }

        private void UpdateVCursorsPosByPoindex(Double oldPosIndex, Double currentPosIndex)
        {
            if (!DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }
            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Int32)DsoModel.Default.Cursors.VCursor[0] + currentPosIndex - oldPosIndex);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Int32)DsoModel.Default.Cursors.VCursor[1] + currentPosIndex - oldPosIndex);
            }
        }

        private void UpdateVCursorsPosByScale(Double oldScale, Double currentScale, Double oldPosIndex)
        {
            if (!DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }

            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Double)(DsoModel.Default.Cursors.VCursor[0] - oldPosIndex) * oldScale / currentScale + Model.PosIndex);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Double)(DsoModel.Default.Cursors.VCursor[1] - oldPosIndex) * oldScale / currentScale + Model.PosIndex);
            }
        }

        private void UpdateVCursorPosByTmbPosition(Double oldPosByus)
        {
            if (!DsoModel.Default.Cursors.VCursor.Source.IsAnalog())
            {
                return;
            }

            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.Type != CursorType.Horizontal)
            {
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(0, (Int32)DsoModel.Default.Cursors.VCursor[0] - (Model.Position - oldPosByus) * Constants.IDX_PER_XDIV / Model.Scale);
                DsoModel.Default.Cursors.VCursor.SetPoxIndex(1, (Int32)DsoModel.Default.Cursors.VCursor[1] - (Model.Position - oldPosByus) * Constants.IDX_PER_XDIV / Model.Scale);
            }
        }

        public Double PosMaxIndex => Model.PosMaxIndex;

        public Double PosMinIndex => Model.PosMinIndex;

        public void ResetPosIndex()
        {
            Decode.DecodeProtocolShareParameter.Default.SetNeedReadData();
            PosIndexBymDiv = Model.PosDefIndex;
            //Hardware.HdCmdFactory.Push(HdCmd.TmbPosition);
        }

        public AnaChnlTimebaseIndex ScaleIndex
        {
            get => Model.ScaleIndex;
            set
            {
                AnaChnlTimebaseIndex old = Model.ScaleIndex;
                var oldscalebyus = Model.Scale;
                var oldposindex = Model.PosIndex;
                Model.CheckAndChangedIndexRange(GetCurrentIndexLimit);

                var temp = value;
                temp = IndexLimit(temp);

                DsoModel.Default?.MathChnls?.Where(x => x.Active && x.Args?.Type == MathType.Track).ToList().ForEach(x =>
                {
                    x.Sampling.ScaleIndex = (Int32)temp;
                });

                if (old != temp)
                {
                    Model.ScaleIndex = temp;
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag = true;
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag = true;
                    DsoModel.Default.Meas.Calc.ClearAllStat();
                    Dispatcher.SoftReset();
                    DsoPrsnt.DefaultDsoPrsnt.PwrModulationClear();
                    Model.ScaleOrPosUpdateTime = DateTime.Now;
                    Hardware.HdCmdFactory.Push(HdCmd.TmbScaleIndex);
                    if (TriggerPrsnt.Type == TriggerType.Serial)
                    {
                        if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                        {
                            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                        }
                    }
                    UpdateVCursorsPosByScale(oldscalebyus, Model.Scale, oldposindex);
                }
            }
        }

        private AnaChnlTimebaseIndex IndexLimit(AnaChnlTimebaseIndex value)
        {
            AnaChnlTimebaseIndex temp = value;
            if (TriggerModel.Mode == TriggerMode.Auto)
            {
                var mathlist = DsoModel.Default?.MathChnls?.Where(x => x.Active).ToList();
                if (mathlist != null && mathlist.Count > 0)
                {
                    if (value >= DsoModel.Default?.Timebase.ScanMinIndex)
                    {
                        temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
                        WeakTip.Default.Write("ScaleIndex", MsgTipId.MathIsNotSupportedInScan, false, "", 2);
                    }
                }
            }

            //if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            //{
            //    var items = DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(x => x.Value.Active && x.Value.Mode != PowerAnalysis.PowerAnalysisOpt.TurnOnOff).ToList();
            //    if (items.Count != 0)
            //    {
            //        if (value >= DsoModel.Default?.Timebase.ScanMinIndex)
            //        {
            //            temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
            //            WeakTip.Default.Write("ScaleIndex", MsgTipId.PowerAnalysisIsNotSupportedInScan, false, "", 2);
            //        }
            //    }
            //}
            //if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            //{
            //    if (value >= DsoModel.Default?.Timebase.ScanMinIndex)
            //    {
            //        temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
            //        WeakTip.Default.Write("ScaleIndex", MsgTipId.JitterIsNotSupportedInScan, false, "", 2);
            //    }
            //}

            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (value >= DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("ScaleIndex", MsgTipId.PassFailIsNotSupportedInScan, false, "", 2);
                }
            }
            var decodelist = DsoModel.Default?.DecodeChnls?.Where(x => x.Active).ToList();
            if (decodelist != null && decodelist.Count > 0)
            {
                if (value >= DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("ScaleIndex", MsgTipId.DecodeIsNotSupportedInScan, false, "", 2);
                }
                if (value < DsoModel.Default?.Timebase.DecodeMaxIndex)
                {
                    temp = DsoModel.Default.Timebase.DecodeMaxIndex;
                    WeakTip.Default.Write("ScaleIndex", MsgTipId.DecodeIsSupportedMinTimebase, false, "", 2);
                }
            }

            //if (DsoModel.Default.JitterModel.Active)
            //{
            //    if (value < DsoModel.Default?.Timebase.DecodeMaxIndex)
            //    {
            //        temp = DsoModel.Default.Timebase.DecodeMaxIndex;
            //        WeakTip.Default.Write("ScaleIndex", MsgTipId.JitterIsSupportedMinTimebase, false, "", 2);
            //    }
            //}

            if (DsoModel.Default.Timebase.SegmentActive)
            {
                if (value >= DsoModel.Default.Timebase.ScanMinIndex)
                {
                    temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("Scale", MsgTipId.SegementIsNotSupportedInScan, false, "", 2);
                }
            }

            if (StorageMode == AnaChnlStorageMode.Fast)
            {
                if (value >= DsoModel.Default.Timebase.ScanMinIndex)
                {
                    temp = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("IsScan", MsgTipId.FastIsNotSupportedInScan);
                }
            }
            return temp;
        }

        private (AnaChnlTimebaseIndex MaxIndex, AnaChnlTimebaseIndex MinIndex) GetCurrentIndexLimit((AnaChnlTimebaseIndex Max, AnaChnlTimebaseIndex Min) init)
        {
            AnaChnlTimebaseIndex max = init.Max;
            AnaChnlTimebaseIndex min = init.Min;
            if (TriggerModel.Mode == TriggerMode.Auto)
            {
                var mathlist = DsoModel.Default?.MathChnls?.Where(x => x.Active).ToList();
                if (mathlist != null && mathlist.Count > 0)
                {
                    if (max >= DsoModel.Default?.Timebase.ScanMinIndex)
                    {
                        max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    }
                }
            }

            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                var items = DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(x => x.Value.Active && x.Value.Mode != PowerAnalysis.PowerAnalysisOpt.TurnOnOff).ToList();
                if (items.Count != 0)
                {
                    if (max >= DsoModel.Default?.Timebase.ScanMinIndex)
                    {
                        max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if (max >= DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                }
            }

            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (max >= DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                }
            }
            var decodelist = DsoModel.Default?.DecodeChnls?.Where(x => x.Active).ToList();
            if (decodelist != null && decodelist.Count > 0)
            {
                if (max >= DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                }
            }

            if (DsoModel.Default.Timebase.SegmentActive)
            {
                if (max >= DsoModel.Default.Timebase.ScanMinIndex)
                {
                    max = DsoModel.Default.Timebase.ScanMinIndex - 1;
                }
            }

            //不同交织模式下的最小时基档定义
            AnaChnlTimebaseIndex vaildmin = PlatformManager.Default.Platform.GetTimebaseMinIndex(DsoModel.Default.Timebase.InterleaveMode);
            if ( min < vaildmin)
            {
                min = vaildmin;
            }

            return (max, min);
        }

        public void LimitScan(MsgTipId msgTipId, Boolean isDecode = false)
        {
            if (ScaleIndex >= DsoModel.Default.Timebase.ScanMinIndex)
            {
                ScaleIndex = DsoModel.Default.Timebase.ScanMinIndex - 1;
                WeakTip.Default.Write("ScaleIndex", msgTipId, false, "", 2);
            }
            if (isDecode && ScaleIndex < DsoModel.Default.Timebase.DecodeMaxIndex)
            {
                ScaleIndex = DsoModel.Default.Timebase.DecodeMaxIndex;
                WeakTip.Default.Write("ScaleIndex", msgTipId, false, "", 2);
            }
        }
        internal AnaChnlTimebaseIndex ScaleMaxIndex => Model.ScaleMaxIndex;

        internal AnaChnlTimebaseIndex ScaleMinIndex => Model.ScaleMinIndex;

        internal Double GetScale(AnaChnlTimebaseIndex index) => Model.GetScaleValue((Int32)index, 0);

        Int32 ISampling.ScaleIndex
        {
            get => (Int32)ScaleIndex;
            set => ScaleIndex = (AnaChnlTimebaseIndex)value;
        }

        public Prefix Prefix => Model.Prefix;

        public String Unit
        {
            get => Model.Unit;
            set => Model.Unit = value;
        }

        public Boolean IsScan => Model.IsScan;

        public Boolean IsItpl() => Model.IsItpl();

        #endregion

        public Int32 StorageDepthOpt
        {
            get => Model.StorageDepthOpt;
            set
            {
                if (AnaChnlLengthSource.Count == 0)
                    return;
                Int32 tempvalue = Math.Clamp(value, 0, AnaChnlLengthSource.Count - 1);
                if (Model.StorageDepthOpt == tempvalue)
                    return;
                Model.StorageDepthOpt = tempvalue;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TmbStorageLen);
            }
        }

        public Int64 TakeViewWaveDotsCnt
        {
            get => Model.TakeViewWaveDotsCnt;
        }
        public Int64 StorageWaveDotsCnt => Model.StorageWaveDotsCnt;

        public Int64 AcqedStorageWaveDotsCnt => Model.AcqedStorageWaveDotsCnt;

        public IReadOnlyList<KeyValuePair<String, Int32>> AnaChnlLengthSource => Model.AnaChnlLengthSource;
        #region 分段存储
        /// <summary>
        /// 显示参考帧
        /// </summary>
        public Boolean RefActive
        {
            get => Model.RefActive;
            set
            {
                if (Model.RefActive == value)
                    return;
                Model.RefActive = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 顺序模式的整体开关
        /// </summary>
        public Boolean SegmentActive
        {
            get => Model.SegmentActive;
            set
            {
                if (value && !Constants.ENABLE_Segement)
                {
                    WeakTip.Default.Write("SegmentActive", MsgTipId.FunctionDisabled);
                    return;
                }
                if (Model.SegmentActive == value)
                    return;
                if (value)
                {
                    if (Mode == AnaChnlAcqMode.Average || Mode == AnaChnlAcqMode.Envelope)
                    {
                        WeakTip.Default.Write("Segment", MsgTipId.SegementIsNotSupportedInThisAcqMode, false, "", 3);
                        return;
                    }
                    else if (TriggerPrsnt.Mode == TriggerMode.OneShot)
                    {
                        WeakTip.Default.Write("Segment", MsgTipId.SegementIsNotSupportedInThisTriggerMode, false, "", 3);
                        return;
                    }

                    if (FunctionLimit.FastFrameFunctionLimit(((DsoPrsnt)Dso).MutexFunctionFlag) == false)
                    {
                        return;
                    }

                    if (ScaleIndex >= DsoModel.Default.Timebase.ScanMinIndex)
                    {
                        ScaleIndex = DsoModel.Default.Timebase.ScanMinIndex - 1;
                        WeakTip.Default.Write("ScaleIndex", MsgTipId.SegementIsNotSupportedInScan, false, "", 2);
                    }

                    StorageMode = AnaChnlStorageMode.Fast;
                }

                Model.SegmentActive = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
                if (!Model.SegmentActive)
                {
                    if (TriggerPrsnt.State == SysState.Stop)
                    {
                        Dispatcher.Resume();
                    }
                    TriggerPrsnt.Mode = TriggerMode.Auto;
                }
            }
        }

        /// <summary>
        /// 重采
        /// </summary>
        public void ResetAcq()
        {
            Model.ResetAcq();
            Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
        }

        public Int32 MaxCallBackIntervalByms => Model.MaxCallBackIntervalByms;
        public Int32 MinCallBackIntervalByms => Model.MinCallBackIntervalByms;

        public Int32 CallBackIntervalByms
        {
            get => Model.CallBackIntervalByms;
            set => Model.CallBackIntervalByms = value;
        }

        public Boolean CallBack
        {
            get => Model.CallBack;
            set
            {
                if (!Model.SegmentComplete)
                {
                    return;
                }
                if (Model.CallBack == value)
                    return;
                Model.CallBack = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);

                if (value)
                {
                    _ = ExecuteCallbackAsync();
                }

            }
        }


        public AnaChnlLengthOpt LengthOpt
        {
            get => Model.LengthOpt;
            set
            {
                if (Model.LengthOpt == value)
                    return;
                Model.LengthOpt = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 已采集的帧数
        /// </summary>
        public Int32 CollectedFrameCount { get => Model.CollectedFrameCount; }

        public Int32 MaxFrameCount { get => Model.MaxFrameCount; }

        public Int32 MinFrameCount { get => Constants.SEGMENT_FRAME_COUNT_MIN; }

        /// <summary>
        /// 总帧数
        /// </summary>
        public Int32 FrameCount
        {
            get => Model.FrameCount;
            set
            {
                if (Model.FrameCount == value)
                    return;
                Model.FrameCount = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 分析模式
        /// </summary>
        public SegmentWorkMode WorkMode
        {
            get => Model.WorkMode;
            set
            {
                if (Model.WorkMode == value)
                {
                    return;
                }
                Model.WorkMode = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 单帧模式下的选定序号:1起
        /// </summary>
        public Int32 CurFrameId
        {
            get => Model.CurFrameId;
            set
            {
                if (Model.CurFrameId == value)
                    return;
                Model.CurFrameId = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 选定帧序号：1起
        /// </summary>
        public List<Int32> ChoseFrameIds
        {
            get => Model.ChoseFrameIds;
        }

        /// <summary>
        /// 参考帧序号：1起
        /// </summary>
        public Int32 ReferFrameIds
        {
            get => Model.ReferFrameIds;
            set
            {
                if (Model.ReferFrameIds == value)
                    return;
                Model.ReferFrameIds = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        /// <summary>
        /// 显示模式
        /// </summary>
        public PlotRenderType RenderType
        {
            get => Model.RenderType;
            set => Model.RenderType = value;
        }

        /// <summary>
        /// 连续模式下的起始帧号
        /// </summary>
        public Int32 SequentStartFrame
        {
            get => Model.SequentStartFrame;
            set
            {
                if (Model.SequentStartFrame == value)
                {
                    return;
                }
                Model.SequentStartFrame = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        public void SetChoseFrameId(Int32 posId, Int32 frameId)
        {
            if (posId < Model.ChoseFrameIds.Count)
                Model.ChoseFrameIds[posId] = frameId + 1;
            else
                Model.ChoseFrameIds.Add(frameId + 1);
        }

        /// <summary>
        /// 连续模式下的终止帧号
        /// </summary>
        public Int32 SequentEndFrame
        {
            get => Model.SequentEndFrame;
            set
            {
                if (Model.SequentEndFrame == value)
                {
                    return;
                }
                Model.SequentEndFrame = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }

        public Double CurFrameSecond
        {
            get => Model.CurFrameSecond;
        }
        public UInt32 BlankTime
        {
            get => Model.BlankTime;
            set
            {
                Model.BlankTime = value;
                Hardware.HdCmdFactory.Push(HdCmd.TmbLongStorage);
            }
        }
        #endregion
        private async Task ExecuteCallbackAsync()
        {
            for (Int32 i = CurFrameId; i <= FrameCount; i++)
            {
                CurFrameId = i;
                if (!Model.CallBack || !SegmentActive)
                {
                    break;
                }
                await Task.Delay(CallBackIntervalByms);
            }
            CurFrameId = CurFrameId == FrameCount ? 1 : CurFrameId;
            Model.CallBack = false;
        }
        public (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) GetCurrentTmbInfo()
        {
            return (PositionByus, Model.PosIndex, Model.Scale, Model.PosIdxPerDiv);
        }
    }
}
