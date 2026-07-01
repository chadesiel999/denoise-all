// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    internal class TriggerShareParameter : INotifyPropertyChanged
    {
        public TriggerShareParameter()
        {
            _HoldoffByps = new(3, () => OnPropertyChanged(nameof(HoldoffByps)))
            {
                Max = MaxHoldoffTime,
                Min = MinHoldoffTime,
                Stp = StpHoldoffTime,
            };
        }

        private Int32 _State = (Int32)SysState.Auto;

        public SysState State
        {
            get => (SysState)_State;
            set
            {
                if (SetState(value))
                {
                    OnPropertyChanged();
                    if (value == 0)
                    {
                        //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
                        Hardware.HdCmdFactory.Push(HdCmd.Run);
                        KeyLed.Default.SetRunStopState(value);
                        Dispatcher.SoftReset();
                        DsoModel.Default.Timebase.ForceUpdateParams();
                    }
                    else
                    {
                        KeyLed.Default.SetRunStopState(value);
                    }
                }               
            }
        }

        private Boolean SetState(SysState value)
        {
            Int32 currentval = _State, startval;
            Int32 desiredval = (Int32)value;

            do
            {
                startval = currentval;

                if (desiredval == startval)
                {
                    return false;
                }

                if (startval == 0)
                {
                    return false;
                }

                currentval = System.Threading.Interlocked.CompareExchange(ref _State, desiredval, startval);

            } while (startval != currentval);
            return true;
        }

        public void ResetState()
        {
            
            if (System.Threading.Interlocked.CompareExchange(ref _State, 1, 0) == 0)
            {
                
                Hardware.HdCmdFactory.Push(HdCmd.Run);
                Dispatcher.SoftReset();
                OnPropertyChanged(nameof(State));
                KeyLed.Default.SetRunStopState(State);
            }
        }

        private TriggerType _Type = TriggerType.Edge;
        public TriggerType Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    TriggerSourceChanged();
                    OnPropertyChanged();
                    Dispatcher.SoftReset();
                }

                PlatformManager.Default.Platform.TriggerTypeChanged(_Type);
            }
        }

        public void  TriggerSourceChanged()
        {
            if (DsoPrsnt.DefaultDsoPrsnt?.Cymometer != null)
            {
                if (_Type == TriggerType.SetupHold || _Type == TriggerType.Pattern || _Type == TriggerType.SustainTime || _Type == TriggerType.Delay || _Type == TriggerType.Serial)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Cymometer.Source = null;
                }
                else
                {
                    DsoPrsnt.DefaultDsoPrsnt.Cymometer.Source = TriggerPrsnt.GetTriggerSource();
                }
            }
        }


        private TriggerMode _Mode = TriggerMode.Auto;
        public TriggerMode Mode
        {
            get => _Mode;
            set
            {
                if(value == TriggerMode.OneShot /*|| value == TriggerMode.Normal*/)
                {
                    Dispatcher.DoClear();
                }
                if (_Mode != value)
                {
                    _Mode = value;
                    ResetState();

                    OnPropertyChanged();
                    KeyLed.Default.SetTriggerModel(value);
                }
                else if (value == TriggerMode.OneShot)
                {
                    Thread.Sleep(100);
                    ResetState();
                }
            }
        }


        private DelayOpt _DelayType = DelayOpt.Time;
        public DelayOpt HoldoffType
        {
            get => _DelayType;
            set
            {
                if (value != _DelayType)
                {
                    _DelayType = value;
                    OnPropertyChanged();
                }
            }
        }

        //private Int64 _HoldoffByps;
        //public Int64 HoldoffByps
        //{
        //    get => _HoldoffByps;
        //    set
        //    {
        //        value = ValidateHoldoff(value);
        //        if (value != _HoldoffByps)
        //        {
        //            _HoldoffByps = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private readonly AdaptNum _HoldoffByps;
        public Int64 HoldoffByps
        {
            get => _HoldoffByps.Value;
            set => _HoldoffByps.Value = value;
        }

        //public void AdjHoldoffTime(Int64 delta) => _HoldoffByps.AdjValue(delta);
        public void AdjHoldoffTime(Int64 delta)
        {
            _HoldoffByps.AdjValue(delta);
            OnPropertyChanged();
        }

        //private static Int64 ValidateHoldoff(Int64 value)
        //{
        //    Int64 holdoff = (Int64)Math.Round((Double)value / StpHoldoffTime, MidpointRounding.AwayFromZero) * StpHoldoffTime;

        //    if (holdoff < MinHoldoffTime)
        //    {
        //        holdoff = MinHoldoffTime;
        //    }
        //    else if (holdoff > MaxHoldoffTime)
        //    {
        //        holdoff = MaxHoldoffTime;
        //    }

        //    return holdoff;
        //}

        public static readonly Int64 MaxHoldoffTime = Constants.MAX_HOLDOFF_PS;

        public static readonly Int64 MinHoldoffTime = Constants.MIN_HOLDOFF_PS;

        public static readonly Int64 StpHoldoffTime = Constants.STP_HOLDOFF_PS;

        private Int32 _HoldoffByCnt = Constants.MIN_HOLDOFF_EVENT;
        public Int32 HoldoffByCnt
        {
            get => _HoldoffByCnt;
            set
            {
                if (value < MinHoldoffCnt)
                {
                    value = MinHoldoffCnt;
                }
                else if (value > MaxHoldoffCnt)
                {
                    value = MaxHoldoffCnt;
                }

                if (value != _HoldoffByCnt)
                {
                    _HoldoffByCnt = value;
                    OnPropertyChanged();
                }
            }
        }

        public static readonly Int32 MaxHoldoffCnt = Constants.MAX_HOLDOFF_EVENT;

        public static readonly Int32 MinHoldoffCnt = Constants.MIN_HOLDOFF_EVENT;

        private Int32 _ExtAttenFactor = 1;
        public Int32 ExtAttenFactor
        {
            get => _ExtAttenFactor;
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }

                if (value != _ExtAttenFactor)
                {
                    _ExtAttenFactor = value;
                    OnPropertyChanged();
                }
            }
        }
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly TriggerShareParameter Default = new();
    }
}
