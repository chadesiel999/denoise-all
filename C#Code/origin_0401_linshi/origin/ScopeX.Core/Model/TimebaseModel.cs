using ScopeX.ComModel;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using ScopeX.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ScopeX.Core
{
    public enum AnaChnlTimebaseIndex
    {
        Lv2p,
        Lv5p,
        Lv10p,
        Lv20p,
        Lv50p,
        Lv100p,
        Lv200p,
        Lv500p,
        Lv1n,
        Lv2n,
        Lv5n,
        Lv10n,
        Lv20n,
        Lv50n,
        Lv100n,
        Lv200n,
        Lv500n,
        Lv1u,
        Lv2u,
        Lv5u,
        Lv10u,
        Lv20u,
        Lv50u,
        Lv100u,
        Lv200u,
        Lv500u,
        Lv1m,
        Lv2m,
        Lv5m,
        Lv10m,
        Lv20m,
        Lv50m,
        Lv100m,
        Lv200m,
        Lv500m,
        Lv1,
        Lv2,
        Lv5,
        Lv10,
        Lv20,
        Lv50,
        Lv100,
        Lv200,
        Lv500,
        Lv1k,
        Lv2k,
        Lv5k,
        Lv10k,
        Lv20k,
        Lv50k,
        Lv100k,
        Lv200k,
        Lv500k,
        Lv1M,
        Lv2M,
        Lv5M,
        Lv10M,
        Lv20M,
        Lv50M,
        Lv100M,
        Lv200M,
        Lv500M,
        Lv1G,
        Lv2G,
        Lv5G,
        Lv10G,
        Lv20G,
        Lv50G,
        Lv100G,
        Lv200G,
        Lv500G,
    }

    internal class SamplingModel : HorzAxisModel
    {
        private protected Double TempPosition = 0;

        //public override Int32 ScaleIndex
        //{
        //    get => base.ScaleIndex;
        //    set
        //    {
        //        base.ScaleIndex = (Int32)value;

        //        base.PosIndex = PosDefIndex - TempPosition * PosIdxPerDiv / ScaleByus;
        //    }
        //}

        public override Double PosIndex
        {
            get => base.PosIndex;
            set
            {
                base.PosIndex = value;
                TempPosition = GetPosValue(base.PosIndex, Scale);
            }
        }

        public Double Position
        {
            get => GetPosValue(PosIndex, Scale);
            set => PosIndex = GetPosIndex(value, Scale);
        }

        public Double MaxPosition => GetPosValue(PosMinIndex, Scale);

        public Double MinPosition => GetPosValue(PosMaxIndex, Scale);

        public Double Scale
        {
            get => GetScaleValue(ScaleIndex, ScaleTick);
            set
            {
                //Int32 tempindex;
                (ScaleIndex, _) = GetScaleIndex(value);
                //ScaleIndex = ScaleLimit(tempindex);
            }
        }

        private Double _AutoScale;

        public Double AutoScale
        {
            get => _AutoScale;
            set
            {
                if (_AutoScale != value)
                {
                    _AutoScale = value;
                }
            }
        }

        private Int32 ScaleLimit(Int32 tempindex)
        {
            if (TriggerModel.Mode == TriggerMode.Auto)
            {
                var mathlist = DsoModel.Default?.MathChnls?.Where(x => x.Active).ToList();
                if (mathlist != null && mathlist.Count > 0)
                {
                    if (tempindex >= (Int32)DsoModel.Default.Timebase.ScanMinIndex)
                    {
                        tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
                        WeakTip.Default.Write("Scale", MsgTipId.MathIsNotSupportedInScan, false, "", 2);
                    }
                }
            }
            //if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            //{
            //    var items = DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(x => x.Value.Active && x.Value.Mode != PowerAnalysis.PowerAnalysisOpt.TurnOnOff).ToList();
            //    if (items.Count != 0)
            //    {
            //        if (tempindex >= (Int32)DsoModel.Default?.Timebase.ScanMinIndex)
            //        {
            //            tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
            //            WeakTip.Default.Write("Scale", MsgTipId.PowerAnalysisIsNotSupportedInScan, false, "", 2);
            //        }
            //    }
            //}
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter.Active)
            {
                if (tempindex >= (Int32)DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("Scale", MsgTipId.JitterIsNotSupportedInScan, false, "", 2);
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.PassFail.Active)
            {
                if (tempindex >= (Int32)DsoModel.Default?.Timebase.ScanMinIndex)
                {
                    tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("Scale", MsgTipId.PassFailIsNotSupportedInScan, false, "", 2);
                }
            }

            var decodelist = DsoModel.Default?.DecodeChnls?.Where(x => x.Active).ToList();
            if (decodelist != null && decodelist.Count > 0)
            {
                if (tempindex >= (Int32)DsoModel.Default.Timebase.ScanMinIndex)
                {
                    tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("Scale", MsgTipId.DecodeIsNotSupportedInScan, false, "", 2);
                }
                if (tempindex < (Int32)DsoModel.Default.Timebase.DecodeMaxIndex)
                {
                    tempindex = (Int32)DsoModel.Default.Timebase.DecodeMaxIndex;
                    WeakTip.Default.Write("Scale", MsgTipId.DecodeIsSupportedMinTimebase, false, "", 2);
                }
            }

            //if (DsoModel.Default.JitterModel.Active)
            //{
            //    if (tempindex < (Int32)DsoModel.Default.Timebase.DecodeMaxIndex)
            //    {
            //        tempindex = (Int32)DsoModel.Default.Timebase.DecodeMaxIndex;
            //        WeakTip.Default.Write("ScaleIndex", MsgTipId.JitterIsSupportedMinTimebase, false, "", 2);
            //    }
            //}

            if (DsoModel.Default.Timebase.SegmentActive)
            {
                if (tempindex >= (Int32)DsoModel.Default.Timebase.ScanMinIndex)
                {
                    tempindex = (Int32)DsoModel.Default.Timebase.ScanMinIndex - 1;
                    WeakTip.Default.Write("Scale", MsgTipId.SegementIsNotSupportedInScan, false, "", 2);
                }
            }
            if (TriggerModel.State == SysState.Stop && tempindex < MinScaleIndexAtStop)
            {
                tempindex = MinScaleIndexAtStop;
                WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 2);
            }
            return tempindex;
        }
        protected Int32 MinScaleIndexAtStop = (Int32)AnaChnlTimebaseIndex.Lv100p;

        public DateTime ScaleOrPosUpdateTime { get; set; }
        public Double MaxScale => GetScaleValue(ScaleMaxIndex, 0);

        public Double MinScale => GetScaleValue(ScaleMinIndex, 0);

        public Double CalcPosIndex(Double position, Double scale)
        {
            return GetPosIndex(position, scale);
        }
        public SamplingModel() : base("Sampling")
        {
            PosIndex = PosDefIndex;
        }

        public AnaChnlTimebaseIndex GetTimebaseIndexByValue(Double Scale)
        {
            Int32 index;
            (index, _) = GetScaleIndex(Scale);
            return (AnaChnlTimebaseIndex)index;
        }
    }

    internal class TimebaseModel : SamplingModel
    {
        protected static readonly Dictionary<AnaChnlTimebaseIndex, (Double Scale, Double MinPosIndex)> TimebaseTableByus = new()
        {
            [AnaChnlTimebaseIndex.Lv2p] = (2e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-12) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5p] = (5e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-12) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10p] = (10e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-11) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20p] = (20e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-11) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50p] = (50e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-11) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100p] = (100e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-10) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200p] = (200e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-10) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500p] = (500e-6, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-10) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1n] = (1e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-9) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2n] = (2e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-9) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5n] = (5e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-9) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10n] = (10e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-8) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20n] = (20e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-8) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50n] = (50e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-8) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100n] = (100e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200n] = (200e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500n] = (500e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1u] = (1e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2u] = (2e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5u] = (5e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10u] = (10e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20u] = (20e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50u] = (50e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100u] = (100e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200u] = (200e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500u] = (500e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1m] = (1e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2m] = (2e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5m] = (5e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10m] = (10e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.01) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20m] = (20e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.02) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50m] = (50e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.05) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100m] = (100e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.1) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200m] = (200e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.2) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500m] = (500e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1] = (1e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 1) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2] = (2e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 2) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5] = (5e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10] = (10e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 10) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20] = (20e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 20) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50] = (50e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 50) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100] = (100e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 100) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200] = (200e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 200) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500] = (500e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 500) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1k] = (1e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 1_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2k] = (2e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 2_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5k] = (5e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 5_000) * 1_000, MidpointRounding.AwayFromZero)),

            [AnaChnlTimebaseIndex.Lv10k] = (10e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 10_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20k] = (20e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 20_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50k] = (50e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 50_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100k] = (100e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 100_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200k] = (200e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 200_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500k] = (500e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 500_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1M] = (1e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 1_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2M] = (2e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 2_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5M] = (5e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 5_000_000) * 1_000, MidpointRounding.AwayFromZero)),

            [AnaChnlTimebaseIndex.Lv10M] = (10e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 10_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20M] = (20e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 20_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50M] = (50e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 50_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100M] = (100e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 100_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200M] = (200e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 200_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500M] = (500e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 500_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1G] = (1e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 1_000_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2G] = (2e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 2_000_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5G] = (5e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 5_000_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10G] = (10e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 10_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20G] = (20e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 20_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50G] = (50e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 50_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100G] = (100e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 100_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200G] = (200e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 200_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500G] = (500e15, Math.Round((5 - Constants.MIN_XPOS_TIME / 500_000_000_000) * 1_000, MidpointRounding.AwayFromZero)),
        };

        public static Double GetScaleByus(Int32 index, Int32 tick)
        {

            if (TimebaseTableByus.TryGetValue((AnaChnlTimebaseIndex)index, out var value))
            {
                return value.Scale;
            }
            else if (index > (Int32)TimebaseTableByus.Last().Key)
            {
                return TimebaseTableByus.Last().Value.Scale;
            }
            else
            {
                return TimebaseTableByus.First().Value.Scale;
            }
        }

        public static (Int32 index, Int32 tick) SetScaleByus(Double scaleValue)
        {
            AnaChnlTimebaseIndex key;
            if (scaleValue >= TimebaseTableByus.Last().Value.Scale)
            {
                key = TimebaseTableByus.Last().Key;
            }
            else if (scaleValue <= TimebaseTableByus.First().Value.Scale)
            {
                key = TimebaseTableByus.First().Key;
            }
            else
            {
                key = TimebaseTableByus.FirstOrDefault((kvp) => kvp.Value.Scale >= scaleValue).Key;
            }
            var tick = (Int32)(scaleValue - TimebaseTableByus[key].Scale);
            return ((Int32)key, tick);
        }

        protected static Double GetMinPosIndex(AnaChnlTimebaseIndex index)
        {
            return TimebaseTableByus[index].MinPosIndex;
        }

        //private Double _LastPosIndex = 0;
        //private Boolean _isfirstscan = false;
        public new AnaChnlTimebaseIndex ScaleIndex
        {
            get => (AnaChnlTimebaseIndex)base.ScaleIndex;
            set
            {
                if (base.ScaleIndex != (Int32)value)
                {
                    Int32 old = base.ScaleIndex;

                    if (TriggerModel.State != SysState.Stop)
                    {
                        base.ScaleIndex = (Int32)value;
                    }
                    else
                    {
                        if (value < (AnaChnlTimebaseIndex)MinScaleIndexAtStop)
                        {
                            base.ScaleIndex = MinScaleIndexAtStop;
                            WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 2);
                        }
                        else
                            base.ScaleIndex = (Int32)value;
                    }

                    //改变水平时基，导致最小触发深度和当前触发深度变化
                    base.PosMinIndex = GetMinPosIndex((AnaChnlTimebaseIndex)base.ScaleIndex);

                    base.PosIndex = PosDefIndex - TempPosition * PosIdxPerDiv / Scale;
                    AdcInterleaveProcessor.Default.Process();
                }
            }
        }

        public new AnaChnlTimebaseIndex ScaleMinIndex
        {
            get => (AnaChnlTimebaseIndex)base.ScaleMinIndex;
            protected set => base.ScaleMinIndex = (Int32)value;
        }

        /// <summary>
        /// 单通道，双通道和多通道的最小档位不一样
        /// </summary>
        /// <param name="limitindex"></param>
        public void CheckAndChangedIndexRange(Func<(AnaChnlTimebaseIndex, AnaChnlTimebaseIndex), (AnaChnlTimebaseIndex, AnaChnlTimebaseIndex)> limitfunc)
        {
            (var max, var min) = limitfunc((InitValue.maxScaleIndex, InitValue.minScaleIndex));
            if (min != ScaleMinIndex)
                ScaleMinIndex = min;

            if (max != ScaleMaxIndex)
                ScaleMaxIndex = max;
        }

        public new AnaChnlTimebaseIndex ScaleMaxIndex
        {
            get => (AnaChnlTimebaseIndex)base.ScaleMaxIndex;
            protected set => base.ScaleMaxIndex = (Int32)value;
        }

        public TimebaseModel(AnaChnlTimebaseIndex maxScaleIndex, AnaChnlTimebaseIndex minScaleIndex, AnaChnlTimebaseIndex minScanIndex, AnaChnlTimebaseIndex maxItplIndex) : base()
        {
            AnaChnlLengthSource = AdcInterleaveProcessor.Default.AnaChnlLengthSource;
            GetScaleValue = GetScaleByus;
            GetScaleIndex = SetScaleByus;
            InitValue = (maxScaleIndex, minScaleIndex, minScanIndex, maxItplIndex);
            ScaleMaxIndex = maxScaleIndex;
            ScaleMinIndex = minScaleIndex;
            ScanMinIndex = minScanIndex;
            ItplMaxIndex = maxItplIndex;
            ScaleIndex = AnaChnlTimebaseIndex.Lv1m;
            ScalePrefix = Prefix.Micro;
            Unit = "s";

            PosMinIndex = GetMinPosIndex(ScaleIndex);
            PosIndex = PosDefIndex;
            //PosPrefix = Prefix.Micro;
            //PosUnit = "s";
            ResetChoseFrameIds();
        }

        protected (AnaChnlTimebaseIndex maxScaleIndex, AnaChnlTimebaseIndex minScaleIndex, AnaChnlTimebaseIndex minScanIndex, AnaChnlTimebaseIndex maxItplIndex) InitValue;

        protected AnaChnlAcqMode _Mode = AnaChnlAcqMode.Normal;
        public AnaChnlAcqMode Mode
        {
            get => _Mode;
            set
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    OnPropertyChanged();
                }
            }
        }

        protected AnaChnlStorageMode _StorageMode = AnaChnlStorageMode.Long;
        public AnaChnlStorageMode StorageMode
        {
            get => _StorageMode;
            set
            {
                if (_StorageMode != value)
                {
                    _StorageMode = value;
                    //!!!Properties Interaction
                    if (_StorageMode != AnaChnlStorageMode.Fast)
                    {
                        SegmentActive = false;
                    }
                    HdCmdFactory.Push(HdCmd.DpxEnabled);
                    OnPropertyChanged();
                }
            }
        }
        private Dictionary<AnaChnlLengthOpt, Int32> MaxSegmentCnt = new Dictionary<AnaChnlLengthOpt, Int32>()
        {
            [AnaChnlLengthOpt.Of25KDots] = 40_000,
            [AnaChnlLengthOpt.Of250KDots] = 4_000,
            [AnaChnlLengthOpt.Of2_5MDots] = 400,
            [AnaChnlLengthOpt.Of25MDots] = 40,
            [AnaChnlLengthOpt.Of250MDots] = 4,
        };
        /// <summary>
        /// 不同存储深度下支持的最大分段数
        /// 20230419 此函数需要修改，已不使用<see cref="AnaChnlLengthOpt"/>
        /// </summary>
        public void UpdateMaxSegmentCnt(Dictionary<AnaChnlLengthOpt, Int32> newDict)
        {
            MaxSegmentCnt = newDict;
        }

        /// <summary>
        /// 根据存储深度的设置，将总段数有效化
        /// </summary>
        private void ValidFrameCount()
        {
            if (MaxSegmentCnt.ContainsKey(_LengthOpt) && _FrameCount > MaxSegmentCnt[_LengthOpt])
                _FrameCount = MaxSegmentCnt[_LengthOpt];
            OnPropertyChanged(nameof(FrameCount));
        }

        /// <summary>
        /// 根据总段数，将存储深度的设置有效化
        /// </summary>
        private void ValidLengthOpt()
        {
            while (MaxSegmentCnt.ContainsKey(_LengthOpt) && _FrameCount > MaxSegmentCnt[_LengthOpt])
                _LengthOpt--;
            OnPropertyChanged(nameof(LengthOpt));
        }


        /**********************
        软件里面计算的分段存储的段数最大值
            1、DDR总地址长度，这个FPGA里面是个常数：29'h1FFF_FFFF
            2、存触发地址的起始地址，这个FPGA里面是个常数：29'h1FC0_0000
            3、存储波形长度L个点
            4、通道数目n通道
	            软件计算出来的段数 = (29'h1FC0_0000/8*64)/L/n

        FPGA里面计算的分段存储的段数最大值
            1、DDR总地址长度，这个FPGA里面是个常数：29'h1FFF_FFFF
            2、存触发地址的起始地址，这个FPGA里面是个常数：29'h1FC0_0000
            3、存触发地址的最大数目，地址8突发
	            （29'h1FFF_FFFF - 29'h1FC0_0000）/8 = 7FFFF;

        如果（软件计算出来的段数 <存触发地址的最大数目） ，发送软件计算出来的段数
        如果（软件计算出来的段数 >=存触发地址的最大数目） ，发送存触发地址的最大数目
        *****************************/
        public Int32 MaxFrameCount
        {
            get
            {
                return PlatformManager.Default.Platform.GetDDrMaxFrameCount(StorageWaveDotsCnt, InterleaveMode); ;//最大存储深度下最后一段存在异常波形，需硬件排查
            }
        }

        protected AnaChnlLengthOpt _LengthOpt = AnaChnlLengthOpt.Full;
        public AnaChnlLengthOpt LengthOpt
        {
            get => _LengthOpt;
            set
            {
                if (_LengthOpt != value)
                {
                    _LengthOpt = value;
                    ValidFrameCount();
                    ResetAcq();
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _IsScan = false;
        public Boolean IsScan
        {
            get => _IsScan;
            set
            {
                if (_IsScan != value)
                {
                    _IsScan = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _IsZoom = false;
        public Boolean IsZoom
        {
            get => _IsZoom;
            set
            {
                if (_IsZoom != value)
                {
                    _IsZoom = value;
                    _ZoomChangedFlag = 1;
                    OnPropertyChanged();
                }
            }
        }

        private volatile Int32 _ZoomChangedFlag = 0;
        public Boolean ZoomChanged => Interlocked.CompareExchange(ref _ZoomChangedFlag, 0, 1) > 0;
        private Double _ZoomCenterX = (Int32)Constants.MAX_XPOS_IDX / 2;
        public Double ZoomCenterX
        {
            get => _ZoomCenterX;
            set
            {
                if (_ZoomCenterX != value)
                {
                    _ZoomCenterX = value;
                    _ZoomChangedFlag = 1;
                    OnPropertyChanged();
                }
            }
        }
        private Double _ZoomCenterY = 0;
        public Double ZoomCenterY
        {
            get => _ZoomCenterY;
            set
            {
                if (_ZoomCenterY != value)
                {
                    _ZoomCenterY = value;
                    OnPropertyChanged();
                }
            }
        }
        private Double _ZoomScaleX = 0.5;
        public Double ZoomScaleX
        {
            get => _ZoomScaleX;
            set
            {
                if (_ZoomScaleX != value)
                {
                    _ZoomScaleX = value;
                    _ZoomChangedFlag = 1;
                    if (_ZoomScaleX == 1)
                    {
                        _ZoomCenterX = (Int32)Constants.MAX_XPOS_IDX / 2;
                    }
                    OnPropertyChanged();
                }
            }
        }
        private Double _ZoomScaleY = 1;
        public Double ZoomScaleY
        {
            get => _ZoomScaleY;
            set
            {
                if (_ZoomScaleY != value)
                {
                    _ZoomScaleY = value;
                    if (_ZoomScaleY == 1)
                    {
                        _ZoomCenterY = 0;
                    }
                    OnPropertyChanged();
                }
            }
        }
        private IReadOnlyList<KeyValuePair<String, Int32>> AnaChnlLengthSource2 = new List<KeyValuePair<String, Int32>>()
        {
            new KeyValuePair<String, Int32>("Auto", 10_000),
            new KeyValuePair<String, Int32>("2MPts", 2_000_000),
            new KeyValuePair<String, Int32>("20MPts", 20_000_000),
            new KeyValuePair<String, Int32>("200MPts", 200_000_000),
            new KeyValuePair<String, Int32>("2GPts", 2_000_000_000),
        };
        /// <summary>
        /// 更新模拟通道的数据长度，交织模式
        /// </summary>
        internal void UpdateAnaChannel()
        {
            if (ConstAutoStorageActive&&(StorageDepthOpt==0))
            {
                AnaChnlLengthSource = AnaChnlLengthSource2;
            }
            else
            {
                AnaChnlLengthSource = AdcInterleaveProcessor.Default.AnaChnlLengthSource;
            }
            //AnaChnlLengthSource = AdcInterleaveProcessor.Default.AnaChnlLengthSource;
            InterleaveMode = AdcInterleaveProcessor.Default.AdcInterleaveMode;
            if (DsoPrsnt.DefaultDsoPrsnt != null)
            {
                DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex = DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex;
            }

            if (StorageDepthOpt >= AnaChnlLengthSource.Count)
                StorageDepthOpt = AnaChnlLengthSource.Count - 1;
            OnPropertyChanged(nameof(AnaChnlLengthSource));
        }

        public IReadOnlyList<KeyValuePair<String, Int32>> AnaChnlLengthSource 
        {
            get;
            private set;
        }
        protected Int32 _StorageDepthOpt = 1;
        public Int32 StorageDepthOpt
        {
            get => _StorageDepthOpt;
            set
            {
                if (_StorageDepthOpt != value)
                {
                    _StorageDepthOpt = value;
                 
                    AdcInterleaveProcessor.Default.Process();
                    OnPropertyChanged();
                }
            }
        }
        public Int64 StorageWaveDotsCnt
        {
            get 
            {
                if (ConstAutoStorageActive&& (StorageDepthOpt==0))
                {
                    return 10_000;
                }
                else
                {
                    return AnaChnlLengthSource[StorageDepthOpt].Value;
                }
            }
        
        }

        public Int64 AcqedStorageWaveDotsCnt = 0;

        protected Boolean _AdaptiveLength = true;
        public Boolean AdaptiveLength
        {
            get => _AdaptiveLength;
            set
            {
                if (_AdaptiveLength != value)
                {
                    _AdaptiveLength = value;
                    OnPropertyChanged();
                }
            }
        }
        private AdcInterleaveMode _InterleaveMode = AdcInterleaveMode.Mode4To1;
        public AdcInterleaveMode InterleaveMode
        {
            get => _InterleaveMode;
            set
            {
                if (value != _InterleaveMode)
                {
                    _InterleaveMode = value;

                    PlatformManager.Default.Platform.SetBandWidthByInterleaveMode(_InterleaveMode);
                    //zy:2023.10.10=>解决交织模式切换时，有一副波形闪的问题
                    if (TriggerModel.State != SysState.Stop)
                        Dispatcher.DoClear();

                    OnPropertyChanged();
                }
            }
        }
        private Double _AnalogSamplingRate = 10_000_000_000;
        public Double AnalogSamplingRate
        {
            get => _AnalogSamplingRate;
            set
            {
                if (value != _AnalogSamplingRate)
                {
                    _AnalogSamplingRate = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int32 _AveCnt = Constants.MIN_AVERAGE_CNT;
        public Int32 AverageCnt
        {
            get => _AveCnt;
            set
            {
                value = ValidateAveCnt(value);
                if (value != _AveCnt)
                {
                    _AveCnt = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Int32 ValidateAveCnt(Int32 value)
        {
            if (value < Constants.MIN_AVERAGE_CNT)
            {
                value = Constants.MIN_AVERAGE_CNT;
            }
            else if (value > Constants.MAX_AVERAGE_CNT)
            {
                value = Constants.MAX_AVERAGE_CNT;
            }

            return value;
        }

        public Int32 AverageMaxCnt { get; init; } = Constants.MAX_AVERAGE_CNT;

        public Int32 AverageMinCnt { get; init; } = Constants.MIN_AVERAGE_CNT;

        private EvlpOpt _EvlpOpt = EvlpOpt.Envlp;

        public EvlpOpt EnvelopOpt
        {
            get => _EvlpOpt;
            set
            {
                if (value != _EvlpOpt)
                {
                    _EvlpOpt = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _EvlpCnt = Constants.MIN_ENVELOPE_CNT;
        public Int32 EnvelopeCnt
        {
            get => _EvlpCnt;
            set
            {
                value = ValidateEvlpCnt(value);
                if (value != _EvlpCnt)
                {
                    _EvlpCnt = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Int32 ValidateEvlpCnt(Int32 value)
        {
            if (value < Constants.MIN_ENVELOPE_CNT)
            {
                value = Constants.MIN_ENVELOPE_CNT;
            }
            else if (value > Constants.MAX_ENVELOPE_CNT)
            {
                value = Constants.MAX_ENVELOPE_CNT;
            }

            return value;
        }

        public Int32 EnvelopeMaxCnt { get; init; } = Constants.MAX_ENVELOPE_CNT;

        public Int32 EnvelopeMinCnt { get; init; } = Constants.MIN_ENVELOPE_CNT;


        private AnaChnlClkSrc _ClkSrc = AnaChnlClkSrc.Inner;
        public AnaChnlClkSrc ClockSrc
        {
            get => _ClkSrc;
            set
            {
                if (value != _ClkSrc)
                {
                    _ClkSrc = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _Ext10MHzLocked = false;
        public Boolean Ext10MHzLocked
        {
            get => _Ext10MHzLocked;
            set
            {
                if (value != _Ext10MHzLocked)
                {
                    _Ext10MHzLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        private AnaChnlItplType _ItplType = AnaChnlItplType.Sinx;
        public AnaChnlItplType InterplType
        {
            get => _ItplType;
            set
            {
                if (value != _ItplType)
                {
                    _ItplType = value;
                    Dispatcher.SoftReset();
                    HdCmdFactory.Push(HdCmd.TmbInterpolateMode);
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EnableRIS = false; // Random Interleaved Sampling

        public Boolean EnableRIS
        {
            get => _EnableRIS;
            set
            {
                if (value != _EnableRIS)
                {
                    _EnableRIS = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly AnaChnlTimebaseIndex DecodeMaxIndex = AnaChnlTimebaseIndex.Lv100n;

        public AnaChnlTimebaseIndex ScanMinIndex
        {
            get;
            protected set;
        }

        public AnaChnlTimebaseIndex ItplMaxIndex
        {
            get;
            protected set;
        }

        //public Boolean IsScan()
        //{
        //    return ScaleIndex >= ScanMinIndex;
        //}

        public Boolean IsItpl()
        {
            return ScaleIndex <= ItplMaxIndex;
        }

        #region 分段存储
        private Boolean _RefActive = false;
        /// <summary>
        /// 参考帧的显示使能
        /// </summary>
        public Boolean RefActive
        {
            get => _RefActive;
            set
            {
                if (_RefActive == value)
                {
                    return;
                }
                value &= _SegmentActive;
                _RefActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 单帧模式下显示的选定帧
        /// </summary>
        private Int32 _CurFrameId = 1;
        public Int32 CurFrameId
        {
            get => _CurFrameId;
            set
            {
                if (_CurFrameId == value)
                    return;

                _CurFrameId = Math.Clamp(value, 1, _FrameCount);
                OnPropertyChanged();
            }
        }

        private Int32 _CurFrameSecond = 1;
        public Int32 CurFrameSecond
        {
            get => _CurFrameSecond;
            set
            {
                if (_CurFrameSecond == value)
                    return;
                _CurFrameSecond = Math.Clamp(value, 1, _FrameCount);
                OnPropertyChanged();
            }
        }

        private Int32 _ReferFrameIds = 1;
        /// <summary>
        /// 参考帧  0起
        /// </summary>
        public Int32 ReferFrameIds
        {
            get => _ReferFrameIds;
            set
            {
                if (_ReferFrameIds == value)
                {
                    return;
                }
                _ReferFrameIds = Math.Clamp(value, 1, _FrameCount);
                OnPropertyChanged();
            }
        }

        private Int32 ValidFrameSelcect(Int32 frameId)
        {
            if (frameId < 1)
                return 1;
            if (frameId > _FrameCount)
                return _FrameCount;
            return frameId;
        }

        private Int32 _FrameCount = Constants.SEGMENT_FRAME_COUNT_DEFAULT;
        /// <summary>
        /// 总帧数
        /// </summary>
        public Int32 FrameCount
        {
            get => _FrameCount;
            set
            {
                value = Math.Clamp(value, Constants.SEGMENT_FRAME_COUNT_MIN, MaxFrameCount);
                if (_FrameCount == value)
                    return;

                _FrameCount = value;

                //ValidLengthOpt();
                OnPropertyChanged();
                //总帧数发生变化时 强制刷新参数并重采
                UpdateFrameParams();
                ResetAcq();
            }
        }

        private SegmentWorkMode _WorkMode = SegmentWorkMode.Single;
        public SegmentWorkMode WorkMode
        {
            get => _WorkMode;
            set
            {
                if (_WorkMode == value)
                    return;
                _WorkMode = value;
                if (_WorkMode == SegmentWorkMode.Sequent)
                {
                    RefActive = false;
                    CallBack = false;
                }
                OnPropertyChanged();
            }
        }

        private Boolean _SegmentActive = false;
        public Boolean SegmentActive
        {
            get => _SegmentActive;
            set
            {
                if (_SegmentActive != value)
                {
                    _SegmentActive = value;

                    //!!!Properties Interaction

                    //if (_SegmentActive)
                    //{
                    //    if (Mode == AnaChnlAcqMode.Average || Mode == AnaChnlAcqMode.Envelope)
                    //    {
                    //        _SegmentActive = false;
                    //        WeakTip.Default.Write("Segment", MsgTipId.SegementIsNotSupportedInThisAcqMode, false, "", 3);
                    //    }
                    //    else if (TriggerPrsnt.Mode == TriggerMode.OneShot)
                    //    {
                    //        WeakTip.Default.Write("Segment", MsgTipId.SegementIsNotSupportedInThisTriggerMode, false, "", 3);
                    //        _SegmentActive = false;
                    //    }
                    //    else
                    //        StorageMode = AnaChnlStorageMode.Fast;
                    //}

                    if (_SegmentActive == false)
                    {
                        _RefActive = false;
                        OnPropertyChanged(nameof(RefActive));

                        _CollectedFrameCount = 0;
                        OnPropertyChanged(nameof(CollectedFrameCount));
                    }
                    OnPropertyChanged();
                    ResetAcq();
                    if (!value)
                    {
                        //if (TriggerPrsnt.State != SysState.Stop)
                        //{
                        //    Dispatcher.Stop();
                        //}
                        //else
                        //{
                        //    Dispatcher.Resume();
                        //}
                    }
                }
            }
        }

        public Int32 MaxCallBackIntervalByms => 5000;
        public Int32 MinCallBackIntervalByms => 1;

        private Int32 _CallBackIntervalByms = 50;
        public Int32 CallBackIntervalByms
        {
            get => _CallBackIntervalByms;
            set
            {
                value = ValidIntervalByms(value);
                if (_CallBackIntervalByms != value)
                {
                    _CallBackIntervalByms = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidIntervalByms(Int32 intervalByms)
        {
            return Math.Clamp(intervalByms, MinCallBackIntervalByms, MaxCallBackIntervalByms);
        }

        private Boolean _CallBack = false;

        public Boolean CallBack
        {
            get => _CallBack;
            set
            {
                if (!_SegmentComplete && value)
                {
                    return;
                }
                if (_CallBack == value)
                    return;

                _CallBack = value;
                if (value)
                {
                    _CurFrameId = 1;
                }
                OnPropertyChanged();
            }
        }
        public Int64 TakeViewWaveDotsCnt
        {
            get
            {
                //if (this.StorageDepthOpt != (Int32)AnaChnlLengthOpt.Auto)
                //    return AnaChnlLengthSource[1].Value;//25 * 1000;
                //else
                {
                    Int64 result = PlatformManager.Default.Platform.GetViewWaveDotsCnt(InterleaveMode);
                    if (result > AnaChnlLengthSource[0].Value)
                        result = AnaChnlLengthSource[0].Value;

                    return result;
                }
            }
        }
        /// <summary>
        /// 已经采集的帧数
        /// </summary>
        private volatile Int32 _CollectedFrameCount = 0;
        public Int32 CollectedFrameCount
        {
            get => _CollectedFrameCount;
            set
            {
                if (_CollectedFrameCount == value)
                    return;
                _CollectedFrameCount = value;
                OnPropertyChanged();
            }
        }

        private PlotRenderType _RenderType = PlotRenderType.Angle;
        public PlotRenderType RenderType
        {
            get => _WorkMode == SegmentWorkMode.Single ? PlotRenderType.None : _RenderType;
            set
            {
                if (_RenderType == value)
                    return;
                _RenderType = value;
                OnPropertyChanged();
            }
        }

        private UInt32 _BlankTime = 0;
        public UInt32 BlankTime
        {
            get => _BlankTime;
            set
            {
                value = Math.Max(Math.Min(value, UInt32.MaxValue), UInt32.MinValue);
                if (_BlankTime != value)
                {
                    _BlankTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _SequentStartFrame = 1;
        public Int32 SequentStartFrame
        {
            get => _SequentStartFrame;
            set
            {
                if (_SequentStartFrame == value)
                    return;
                _SequentStartFrame = Math.Clamp(value, 1, _FrameCount - 1);
                OnPropertyChanged();

                if (_SequentStartFrame >= _SequentEndFrame)
                {
                    _SequentEndFrame = _SequentStartFrame + 1;
                    OnPropertyChanged(nameof(SequentEndFrame));
                }
                if (_SequentEndFrame - _SequentStartFrame + 1 > Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT)
                {
                    _SequentEndFrame = Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT + _SequentStartFrame - 1;
                    OnPropertyChanged(nameof(SequentEndFrame));
                }
            }
        }

        private Int32 _SequentEndFrame = Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT;
        public Int32 SequentEndFrame
        {
            get => _SequentEndFrame;
            set
            {
                if (_SequentEndFrame == value||(value<2))
                    return;

                _SequentEndFrame = Math.Clamp(value, 2, _FrameCount);
                OnPropertyChanged();

                if (_SequentEndFrame <= _SequentStartFrame)
                {
                    _SequentStartFrame = _SequentEndFrame - 1;
                    OnPropertyChanged(nameof(SequentStartFrame));
                }

                if (_SequentEndFrame - _SequentStartFrame + 1 > Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT)
                {
                    _SequentStartFrame = _SequentEndFrame - Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT + 1;
                    OnPropertyChanged(nameof(SequentStartFrame));
                }
            }
        }


        /// <summary>
        /// 叠加模式下需要处理的帧集合
        /// </summary>
        private List<Int32> _ChoseFrameIds = new();
        /// <summary>
        /// 选定帧  0起
        /// </summary>
        public List<Int32> ChoseFrameIds
        {
            get => _ChoseFrameIds;
        }

        private void ResetChoseFrameIds()
        {
            _ChoseFrameIds.Clear();
            for (Int32 i = _SequentStartFrame; i <= _SequentEndFrame; i++)
                _ChoseFrameIds.Add(i);
        }

        internal void UpdateParamsAndResetAcq()
        {
            if (_SegmentComplete)
            {
                return;
            }
            UpdateFrameParams();
            ResetAcq();
        }

        /// <summary>
        /// 采集过程中 如果用户强制Stop 则需要强制刷新参数
        /// </summary>
        internal void ForceUpdateParams()
        {
            if (_SegmentActive == false)
            {
                return;
            }
            if (_CollectedFrameCount < 2)
            {
                return;
            }
            if (!_SegmentComplete)
            {
                _SegmentComplete = true;
                _FrameCount = _CollectedFrameCount;
                OnPropertyChanged(nameof(FrameCount));
                OnPropertyChanged(nameof(CollectedFrameCount));
                if (_CurFrameId > FrameCount)
                {
                    _CurFrameId = 1;//第一帧
                    OnPropertyChanged(nameof(CurFrameId));
                }

                if (_ReferFrameIds > FrameCount)
                {
                    _ReferFrameIds = 1;//第一帧
                    OnPropertyChanged(nameof(ReferFrameIds));
                }

                if (_SequentEndFrame > FrameCount)
                {
                    SequentEndFrame = FrameCount;//最后一帧
                }
            }
        }

        public void ResetAcq()
        {
            if (!_SegmentActive)
                return;
            //重采前再次更新参数 有可能在界面上点击重采
            UpdateFrameParams();
            _SegmentComplete = false;
            CollectedFrameCount = 0;//重采时已采集的清零
            CallBack = false;//重采时关闭回放
            TriggerModel.ResetState();
            Dispatcher.NeedUpdateSegmentFlag();
        }

        private Boolean _SegmentComplete = false;
        internal Boolean SegmentComplete => _SegmentComplete;

        private void UpdateCollectedCnt()
        {
            if (DsoModel.DataSrcOpt == DataSourceOpt.Simulator)
            {
                CollectedFrameCount = CollectedFrameCount < FrameCount ? CollectedFrameCount + 1 : CollectedFrameCount;
            }
            else
                CollectedFrameCount = Hd.AnalogChannel?.TryTakeCollectedSegmentCnt() ?? 0;

            _SegmentComplete = CollectedFrameCount >= FrameCount;
            CallBack = false;
            if (_SegmentComplete)
            {
                TriggerModel.State = SysState.Stop;
                CollectedFrameCount = Math.Min(CollectedFrameCount, FrameCount);
            }

        }

        /// <summary>
        /// 刷新参数
        /// </summary>
        /// <param name="isUpdate">是否需要更新参数，true --> 更新，false --> 不更新</param>
        /// <param name="resetCollectedCount">是否重置已采集到的帧数，true --> 重置，false --> 不重置</param>
        internal void UpdateFrameParams(Boolean isUpdate = true, Boolean resetCollectedCount = true)
        {
            if (!isUpdate)
            {
                return;
            }
            if (_FrameCount > MaxFrameCount)
            {
                _FrameCount = MaxFrameCount;
                _CollectedFrameCount = Math.Min(CollectedFrameCount, _FrameCount);
                OnPropertyChanged(nameof(CollectedFrameCount));
                OnPropertyChanged(nameof(FrameCount));
            }

            if (_CurFrameId > FrameCount)
            {
                _CurFrameId = 1;//第一帧
                OnPropertyChanged(nameof(CurFrameId));
            }

            if (_ReferFrameIds > FrameCount)
            {
                _ReferFrameIds = 1;//第一帧
                OnPropertyChanged(nameof(ReferFrameIds));
            }

            if (_SequentEndFrame > FrameCount)
            {
                SequentEndFrame = FrameCount;//最后一帧
            }
            if (resetCollectedCount)
                CollectedFrameCount = 0;//只有有参数更新已采集的清零
        }

        public void SegmentUpdate()
        {
            if (!_SegmentActive)
                return;

            if (TriggerModel.State != SysState.Stop)
            {
                UpdateFrameParams(true, false);
                UpdateCollectedCnt();
            }
            else
            {
                if (_WorkMode == SegmentWorkMode.Sequent)
                    ResetChoseFrameIds();
            }

            DsoModel.Default.Display.RenderType = RenderType;
        }

        #endregion
        public void CalcScaleLimitAtStop()
        {
            Double hardwareSampeIntervalBySecond = 1 / AnalogSamplingRate;
            for (AnaChnlTimebaseIndex scaleIndex = AnaChnlTimebaseIndex.Lv2p; scaleIndex < ScaleMaxIndex; scaleIndex++)
            {
                if ((GetScaleValue((Int32)scaleIndex, 0) * 1e-6) >= hardwareSampeIntervalBySecond)
                {
                    MinScaleIndexAtStop = (Int32)scaleIndex;
                    return;
                }
            }
            //error
        }

        private Boolean _EnhancedBitsActive = false;
        /// <summary>
        /// 位增强使能
        /// </summary>
        public Boolean EnhancedBitsActive
        {
            get
            {
                return _EnhancedBitsActive;
            }
            set
            {
                _EnhancedBitsActive = value;
                OnPropertyChanged();
            }
        }

        private Boolean _ConstAutoStorageActive = true;
        /// <summary>
        /// 固定AUTO的存储长度使能
        /// </summary>
        public Boolean ConstAutoStorageActive
        {
            get
            {
                //return _ConstAutoStorageActive;
                return false;
            }
            set
            {
                _ConstAutoStorageActive = value;
                AdcInterleaveProcessor.Default.Process();
                OnPropertyChanged();
            }
        }

        private Double _EnhancedBits = Constants.MinEnhancedBits;
        /// <summary>
        /// 位增强参数
        /// </summary>
        public Double EnhancedBits
        {
            get
            {
                return _EnhancedBits;
            }
            set
            {
                _EnhancedBits = value;
                OnPropertyChanged();
            }
        }
    }
}
