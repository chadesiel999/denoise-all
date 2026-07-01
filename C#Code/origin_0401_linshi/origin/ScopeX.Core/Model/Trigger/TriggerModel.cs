// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    internal abstract class TriggerModel : INotifyPropertyChanged
    {
        public abstract String Name
        {
            get;
        }

        public static SysState State
        {
            get => TriggerShareParameter.Default.State;
            set
            {
                if (TriggerShareParameter.Default.State != value)
                {
                    TriggerShareParameter.Default.State = value;
                    if (value == SysState.Stop)
                        DsoModel.Default.Timebase.CalcScaleLimitAtStop();
                }
            }
        }

        public static void ResetState()
        {
            TriggerShareParameter.Default.ResetState();
            //DsoModel.Default.Timebase.CollectedFrameCount = 0;
        }

        public static TriggerType Type
        {
            get => TriggerShareParameter.Default.Type;
            set => TriggerShareParameter.Default.Type = value;
        }

        public static TriggerMode Mode
        {
            get => TriggerShareParameter.Default.Mode;
            set
            {
                if (TriggerShareParameter.Default.Mode != value && value == TriggerMode.Auto)
                {
                    var mathlist = DsoModel.Default?.MathChnls?.Where(x => x.Active).ToList();
                    if (mathlist != null && mathlist.Count > 0)
                    {
                        if (DsoModel.Default?.Timebase.ScaleIndex >= DsoModel.Default?.Timebase.ScanMinIndex)
                        {
                            DsoModel.Default.Timebase.ScaleIndex = DsoModel.Default.Timebase.ScanMinIndex - 1;
                            WeakTip.Default.Write("ScaleIndex", MsgTipId.MathIsNotSupportedInScan, false, "", 2);
                        }
                    }
                }
                TriggerShareParameter.Default.Mode = value;
            }
        }

        /// <summary>
        /// 释抑
        /// </summary>
        public static DelayOpt HoldoffType
        {
            get => TriggerShareParameter.Default.HoldoffType;
            set => TriggerShareParameter.Default.HoldoffType = value;
        }

        public static Int64 HoldoffByps
        {
            get => TriggerShareParameter.Default.HoldoffByps;
            set => TriggerShareParameter.Default.HoldoffByps = value;
        }

        public static Int64 MaxHoldoffTime => TriggerShareParameter.MaxHoldoffTime;

        public static Int64 MinHoldoffTime => TriggerShareParameter.MinHoldoffTime;

        public static Int64 StpHoldoffTime => TriggerShareParameter.StpHoldoffTime;

        public static void AdjHoldoffTime(Int64 delta) => TriggerShareParameter.Default.AdjHoldoffTime(delta);

        public static Int32 HoldoffByCnt
        {
            get => TriggerShareParameter.Default.HoldoffByCnt;
            set => TriggerShareParameter.Default.HoldoffByCnt = value;
        }



        public static Int32 MaxHoldoffCnt => TriggerShareParameter.MaxHoldoffCnt;

        public static Int32 MinHoldoffCnt => TriggerShareParameter.MinHoldoffCnt;

        public static Boolean EnableExtAtten
        {
            get => TriggerShareParameter.Default.ExtAttenFactor > 1;
            set => TriggerShareParameter.Default.ExtAttenFactor = value ? Constants.EXT_TRIGGER_ATTEN_FAC : 1;
        }

        protected virtual Double ValidatePosIndex(Double posIndex, Boolean showtip = true)
        {
            // posIndex = Math.Round(posIndex / StpPosIndex, MidpointRounding.AwayFromZero) * StpPosIndex;
            posIndex = posIndex / StpPosIndex * StpPosIndex;
            if (posIndex > MaxPosIndex)
            {
                posIndex = MaxPosIndex;
                if (showtip)
                    WeakTip.Default.Write(nameof(StpPosIndex), MsgTipId.GreatethanMax, false, "", 1);
            }
            else if (posIndex < MinPosIndex)
            {
                posIndex = MinPosIndex;
                if (showtip)
                    WeakTip.Default.Write(nameof(StpPosIndex), MsgTipId.LessthanMin, false, "", 1);
            }
            return posIndex;
        }

        public virtual Double MaxPosIndex
        {
            get;
            init;
        } = Constants.MAX_TRIGGER_IDX;

        public virtual Double MinPosIndex
        {
            get;
            init;
        } = Constants.MIN_TRIGGER_IDX;

        public Double StpPosIndex
        {
            get;
            init;
        } = Constants.STP_TRIGGER_IDX;

        public abstract void LeapPosIndex();

        public static (Double Scale, Double Pos0, Prefix Pfx, String Unit) GetAnalogAxisInfo(ChannelId channel) => GetAxisInfo(channel);

        //public static Func<ChannelId, (Double Scale, Double Pos0, Prefix Pfx, String Unit)> GetAnalogAxisInfo
        //{
        //    get;
        //    set;
        //} = ts => (1, 0, Prefix.Empty, "?");

        public static Func<ChannelId, DigitalBitCtrlGrpModel?> GetDigiModel
        {
            get;
            set;
        } = ts => null;

        protected static Double PosIndexToValue(ChannelId ts, Double posIndex)
        {
            return GetAnalogAxisInfo(ts).Scale * posIndex;
        }

        protected static Double ValueToPosIndex(ChannelId ts, Double posValue)
        {
            return posValue / GetAnalogAxisInfo(ts).Scale;
        }

        public static String GetPosUnit(ChannelId ts)
        {
            return ts.IsDigital() ? QuantityUnit.Voltage.ToUnitString() : GetAnalogAxisInfo(ts).Unit;
        }

        public static Prefix GetPosPrefix(ChannelId ts)
        {
            return ts.IsDigital() ? Prefix.Milli : GetAnalogAxisInfo(ts).Pfx;
        }

        internal static (Double Scale, Double Pos0, Prefix Pfx, String Unit) GetAxisInfo(ChannelId ts)
        {
            switch (ts)
            {
                case ChannelId when ts.IsAnalog():
                    var ach = (AnalogModel)DsoModel.Default.GetChannel(ts);
                    return (ach.Conditioning.ScaleBymV / ach.Conditioning.PosIdxPerDiv, ach.Conditioning.PosIndex, ach.Conditioning.Prefix, ach.Conditioning.Unit);
                case ChannelId.Ext:
                    return (Constants.EXT_TRIGGER_RES_MV * 1/*(TriggerPrsnt.EnableExtAtten ? 5 : 1)*/, 0, Prefix.Milli, "V");
                //!!!Is ChannelId.Ext5 reserved? 
                case ChannelId.Ext5:
                    return (Constants.EXT_TRIGGER_RES_MV * 5, 0, Prefix.Milli, "V");
                //!!!What is the range of AC Line trigger's level?
                case ChannelId.AC:
                    return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");
                case ChannelId.AuxIn:
                    return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");
                default:
                    return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");

            }
            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Argument {nameof(ts)} of function {nameof(GetAxisInfo)} is not a valide trigger source.", EventBus.LogLevel.Error));
            throw new ArgumentException($"Argument {nameof(ts)} of function {nameof(GetAxisInfo)} is not a valide trigger source.");
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public virtual event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                TriggerShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                TriggerShareParameter.Default.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
