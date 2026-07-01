// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using NPOI.SS.Formula.Functions;
    using System;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Decode;
    using ScopeX.Core.Sda;
    using ScopeX.Core.Tools;
    using ScopeX.Hardware.Driver;
    using System.Runtime.Intrinsics.Arm;
    using System.Collections.Generic;

    public abstract class TriggerPrsnt : MulticastPrsnt<ITriggerView>, ITriggerPrsnt
    {
        private static TriggerPrsnt? _Current;

        private static ChannelId? TriggerSerialSource;

        protected TriggerPrsnt(IDsoPrsnt idp, ITriggerView? view) : base(idp)
        {
            //TriggerModel.GetAnalogAxisInfo = GetAxisInfo;
            TriggerModel.GetDigiModel = GetDigiModel;
            //需要关注DigitalModel的变化
            ((DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0)).PropertyChanged += OnPropertyChanged;
                        
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public virtual void LoadEvent()
        {

        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public virtual void DisposeEvent()
        {

        }

        public static DelayOpt HoldoffType
        {
            get => TriggerModel.HoldoffType;
            set
            {
                TriggerModel.HoldoffType = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigHoldoff);
            }
        }

        public static Int64 HoldoffByps
        {
            get => TriggerModel.HoldoffByps;
            set
            {
                TriggerModel.HoldoffByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigHoldoff);
            }
        }

        public static Double HoldoffByus
        {
            get => HoldoffByps / 1000_000D;
            set => HoldoffByps = (Int64)(value * 1000_000D);
        }

        public static Int64 MaxHoldoffTime => TriggerModel.MaxHoldoffTime;

        public static Int64 MinHoldoffTime => TriggerModel.MinHoldoffTime;

        public static void AdjHoldoff(Int64 step)
        {
            //TriggerModel.HoldoffByps += step * TriggerModel.StpHoldoffTime;
            TriggerModel.AdjHoldoffTime(step);
            Hardware.HdCmdFactory.Push(HdCmd.TrigHoldoff);
        }

        public static Int32 HoldoffByCnt
        {
            get => TriggerModel.HoldoffByCnt;
            set
            {
                TriggerModel.HoldoffByCnt = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigHoldoff);
            }
        }

        public static Int32 MaxHoldoffCnt => TriggerModel.MaxHoldoffCnt;

        public static Int32 MinHoldoffCnt => TriggerModel.MinHoldoffCnt;

        public static TriggerMode Mode
        {
            get => TriggerModel.Mode;
            set
            {
                if (value == TriggerMode.OneShot && DsoModel.Default.Timebase.SegmentActive)
                {
                    WeakTip.Default.Write("Segment", MsgTipId.SingleTriggerIsNotSupportedInSegement, false, "", 3);
                    return;
                }

                KeyLed.Default.SetTriggerModel(value);//bug 5494 此处通知按键板切换LED，避免在Dispather中跨线程通知导致下发按键板报文粘包
                Dispatcher.SetNewTriggerMode(value);
                //TriggerModel.Mode = value;
                //Dispatcher.SoftReset();
                //Hardware.HdCmdFactory.Push(HdCmd.TrigMode);
                //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
            }
        }

        public static SysState State => TriggerModel.State;

        internal static void ResetState()
        {
            TriggerModel.ResetState();
        }

        public static TriggerType Type => TriggerModel.Type;

        public static Boolean EnableExtAtten
        {
            get => TriggerModel.EnableExtAtten;
            set
            {
                TriggerModel.EnableExtAtten = value;

                Hardware.HdCmdFactory.Push(HdCmd.TrigSource);
            }
        }
        public static void Force()
        {
            Hd.DoForceTrigger();
        }
        public String Name => Model.Name;

        public abstract Double PosIndex { get; set; }

        public abstract void ResetPosIndex();
        internal static TriggerPrsnt? GetCurrentTrigger() => _Current;
        public static TriggerPrsnt GetOrMakeTrigger(IDsoPrsnt idp, TriggerType tt, ITriggerView? tv = null)
        {
            if (_Current?.Name != tt.ToString())
            {
                //Backup Views
                var tvs = _Current?.GetViewList().ToList() ?? new();
                if (tv is not null)
                {
                    tvs.Add(tv);
                }

                //Dispose
                _Current?.Dispose();
                _Current?.DisposeEvent();
                //Make
                if (tt == TriggerType.Serial)
                {
                    _Current = TriggerTools.GetSerialPrsnt(idp, TriggerSerialShareParameter.Default.Source, null);
                }
                else 
                {
                    _Current = TriggerTools.MakeTrigPrsnt(idp, tt);
                    tvs = tvs.Where(x => x is not ITriggerSerialView).ToList();
                    if (tt == TriggerType.Edge)
                    {
                        var edgePst = _Current as TrigEdgePrsnt;
                        PlatformManager.Default.Platform.SetEdgeTriggerLed(edgePst.Slope);
                    }
                }

                //Attach
                _Current.AddViewList(tvs);
                foreach (var v in tvs)
                {
                    v.Presenter = _Current;
                }

                TriggerModel.Type = tt;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                if(TriggerModel.Type == TriggerType.Serial)
                {
                    Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                }
            }
            //TriggerModel.Type = tt;
            return _Current;
        }


        //private static TriggerPrsnt MakeTrigPrsnt(IDsoPrsnt idp, TriggerType tt)
        //{

        //    TriggerPrsnt prsnt;

        //    if (TiggerTools.TriggerPrsnts.ContainsKey(tt))
        //    {
        //        prsnt = TiggerTools.TriggerPrsnts[tt];
        //    }
        //    else
        //    {
        //        prsnt = tt switch
        //        {
        //            TriggerType.Edge => new TrigEdgePrsnt(idp),
        //            TriggerType.PulseWidth => new TrigWidthPrsnt(idp),
        //            TriggerType.Video => new TrigVideoPrsnt(idp),
        //            TriggerType.Pattern => new TrigPatPrsnt(idp),
        //            TriggerType.State => new TrigStatePrsnt(idp),
        //            TriggerType.Delay => new TrigDelayPrsnt(idp),
        //            TriggerType.TimeOut => new TrigTimeOutPrsnt(idp),
        //            TriggerType.SustainTime => new TrigSustainTimePrsnt(idp),
        //            TriggerType.SetupHold => new TrigSetupHoldPrsnt(idp),
        //            TriggerType.NEdge => new TrigNEdgePrsnt(idp),
        //            TriggerType.Runt => new TrigRuntPrsnt(idp),
        //            TriggerType.Transition => new TrigTransPrsnt(idp),
        //            TriggerType.Glitch => new TrigGlitchPrsnt(idp),
        //            TriggerType.Window => new TrigWindowPrsnt(idp),
        //            TriggerType.Interval => new TrigIntervalPrsnt(idp),
        //            TriggerType.MultiQulified => new TrigMultiQualifiedPrsnt(idp),
        //            TriggerType.Serial => TiggerTools.TrigSerialPrsnts[SerialProtocolType.Close],

        //            _ => throw new NotImplementedException(),
        //        };
        //        TiggerTools.TriggerPrsnts.Add(tt, prsnt);
        //    }

        //    return prsnt;

        //    //return tt switch
        //    //{
        //    //    TriggerType.Edge => new TrigEdgePrsnt(idp),
        //    //    TriggerType.PulseWidth => new TrigWidthPrsnt(idp),
        //    //    TriggerType.Video => new TrigVideoPrsnt(idp),
        //    //    TriggerType.Pattern => new TrigPatPrsnt(idp),
        //    //    TriggerType.State => new TrigStatePrsnt(idp),
        //    //    TriggerType.Delay => new TrigDelayPrsnt(idp),
        //    //    TriggerType.TimeOut => new TrigTimeOutPrsnt(idp),
        //    //    TriggerType.SustainTime => new TrigSustainTimePrsnt(idp),
        //    //    TriggerType.SetupHold => new TrigSetupHoldPrsnt(idp),
        //    //    TriggerType.NEdge => new TrigNEdgePrsnt(idp),
        //    //    TriggerType.Runt => new TrigRuntPrsnt(idp),
        //    //    TriggerType.Transition => new TrigTransPrsnt(idp),
        //    //    TriggerType.Glitch => new TrigGlitchPrsnt(idp),
        //    //    TriggerType.Window => new TrigWindowPrsnt(idp),
        //    //    TriggerType.Interval => new TrigIntervalPrsnt(idp),
        //    //    TriggerType.MultiQulified => new TrigMultiQualifiedPrsnt(idp),
        //    //    TriggerType.Serial => new CloseTrigSerialPrsnt(idp),
        //    //    _ => throw new NotImplementedException(),
        //    //};
        //}

        protected virtual ChannelId? TriggerSource()
        {
            return null;
        }

        public static ChannelId? GetTriggerSource()
        {
            if (_Current != null)
            {
                if(_Current is TrigSerialPrsnt tsp)
                {
                    return TriggerSerialShareParameter.Default.Source;
                }
                else
                {
                    return _Current.TriggerSource();
                }
            }

            return null;
        }

        public static Boolean SetTrigSerialSource(ChannelId? id)
        {
            if(_Current is TrigSerialPrsnt tsp)
            {
                if (id != null && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id.Value, out var p) && p is DecodePrsnt dp)
                {
                    tsp.Source = id.Value;
                    UpdateTrigSerialType(dp.ProtocolType);
                }
                else
                {
                    tsp.Source = ChannelId.None;
                    UpdateTrigSerialType(SerialProtocolType.Close);
                }
                return true;
            }
            return false;
        }

        public static Boolean UpdateTrigSerialType(SerialProtocolType type)
        {
            if(_Current is TrigSerialPrsnt)
            {
                TriggerSerialShareParameter.Default.ProtocolType = type;
                _Current = TriggerTools.GetSerialPrsnt(DsoPrsnt.DefaultDsoPrsnt, TriggerSerialShareParameter.Default.Source, null);
                Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                return true;
            }
            else
            {
                return false;
            }
        }


        public void TriggerSourceChanged()
        {
            TriggerShareParameter.Default.TriggerSourceChanged();
        }

        public static TrigSerialPrsnt GetOrMakeTriggerSerial(IDsoPrsnt idp, SerialProtocolType tt, ITriggerSerialView? view = null)
        {
            if (_Current == null)
            {
                return GetTrigSerialPrsnt(idp, tt, view);
            }

            if (_Current is TrigSerialPrsnt tsp)
            {
                if (tsp.SerialType != tt)
                {
                    return GetTrigSerialPrsnt(idp, tt, view);
                }
                else
                {
                    return tsp;
                }
            }
            else
            {
                return GetTrigSerialPrsnt(idp, tt, view);
            }
        }

        public static void TryRemoveTriggerView(ITriggerView view)
        {
            TriggerTools.TryRemoveTriggerView(view);
        }


        //public static TrigSerialPrsnt GetSerialPrsnt(IDsoPrsnt idp, ChannelId id, ITriggerSerialView? view)
        //{
        //    var serialtype = SerialProtocolType.Close;
        //    if (id != ChannelId.None)
        //    {
        //        if (idp.TryGetChannel(id, out var p) && p is DecodePrsnt decodep && decodep.Active)
        //        {
        //            serialtype = decodep.ProtocolType;
        //        }
        //    }


        //    TrigSerialPrsnt prsnt;

        //    if (TiggerTools.TrigSerialPrsnts.ContainsKey(serialtype))
        //    {
        //        prsnt= TiggerTools.TrigSerialPrsnts[serialtype];
        //    }
        //    else
        //    {
        //        prsnt = serialtype switch
        //        {
        //            SerialProtocolType.Close => new CloseTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.RS232 => new RS232TrigSerialPrsnt(idp, view),
        //            SerialProtocolType.I2C => new I2CTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.SPI => new SPITrigSerialPrsnt(idp, view),
        //            SerialProtocolType.SENT => new SENTTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.JTAG => new JTAGTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.CAN => new CANTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.CAN_FD => new CANFDTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.ARINC429 => new ARINC429TrigSerialPrsnt(idp, view),
        //            SerialProtocolType.LIN => new LINTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.SPMI => new SPMITrigSerialPrsnt(idp, view),
        //            SerialProtocolType.FlexRay => new FlexRayTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.AudioBus => new AudioBusTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.Ethernet => new EthernetTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.PCIe => new PCIeTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.USB => new USBTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.SATA => new SATATrigSerialPrsnt(idp, view),
        //            SerialProtocolType.MIL => new MILTrigSerialPrsnt(idp, view),
        //            SerialProtocolType.NRZ => new NRZTrigSerialPrsnt(idp, view),

        //            _ => throw new NotImplementedException(),
        //        };
        //        TiggerTools.TrigSerialPrsnts.Add(serialtype, prsnt);
        //    }

        //    prsnt.Source = id;

        //    return prsnt;
        //}

        private static TrigSerialPrsnt GetTrigSerialPrsnt(IDsoPrsnt idp, SerialProtocolType tt, ITriggerSerialView? view = null)
        {
            System.Collections.Generic.List<ITriggerView> views = new System.Collections.Generic.List<ITriggerView>();
            if (_Current != null)
            {
                var curviews = _Current.GetViewList().Where(x=>view!=null&&x.GetType()!=view.GetType());

                views.AddRange(curviews);
            }
            if (view != null)
            {
                views.Add(view);
            }
            _Current?.Dispose();
            _Current?.DisposeEvent();

            TrigSerialPrsnt trigserial;

            if (TriggerTools.TrigSerialPrsnts.ContainsKey(tt))
            {
                trigserial = TriggerTools.TrigSerialPrsnts[tt];
            }
            else
            {
                trigserial = tt switch
                {
                    SerialProtocolType.Close => new CloseTrigSerialPrsnt(idp, view),
                    SerialProtocolType.RS232 => new RS232TrigSerialPrsnt(idp, view),
                    SerialProtocolType.I2C => new I2CTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SPI => new SPITrigSerialPrsnt(idp, view),
                    SerialProtocolType.SENT => new SENTTrigSerialPrsnt(idp, view),
                    SerialProtocolType.JTAG => new JTAGTrigSerialPrsnt(idp, view),
                    SerialProtocolType.CAN => new CANTrigSerialPrsnt(idp, view),
                    SerialProtocolType.CAN_FD => new CANFDTrigSerialPrsnt(idp, view),
                    SerialProtocolType.ARINC429 => new ARINC429TrigSerialPrsnt(idp, view),
                    SerialProtocolType.LIN => new LINTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SPMI => new SPMITrigSerialPrsnt(idp, view),
                    SerialProtocolType.FlexRay => new FlexRayTrigSerialPrsnt(idp, view),
                    SerialProtocolType.AudioBus => new AudioBusTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Ethernet => new EthernetTrigSerialPrsnt(idp, view),
                    SerialProtocolType.PCIe => new PCIeTrigSerialPrsnt(idp, view),
                    SerialProtocolType.USB => new USBTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SATA => new SATATrigSerialPrsnt(idp, view),
                    SerialProtocolType.MIL => new MILTrigSerialPrsnt(idp, view),
                    SerialProtocolType.NRZ => new NRZTrigSerialPrsnt(idp, view),
                    SerialProtocolType.I3C => new I3CTrigSerialPrsnt(idp, view),
                    SerialProtocolType.SMBus => new SMBusTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Common_8b10b => new D8B10BTrigSerialPrsnt(idp, view),
                    SerialProtocolType.Mlt3 => new Mlt3TrigSerialPrsnt(idp, view),
                    SerialProtocolType.CXPI => new CXPITrigSerialPrsnt(idp, view),

                    _ => throw new NotImplementedException(),
                };
                TriggerTools.TrigSerialPrsnts.TryAdd(tt, trigserial);
            }
            //TrigSerialPrsnt trigserial = tt switch
            //{
            //    SerialProtocolType.Close => new CloseTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.RS232 => new RS232TrigSerialPrsnt(idp, view),
            //    SerialProtocolType.I2C => new I2CTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.SPI => new SPITrigSerialPrsnt(idp, view),
            //    SerialProtocolType.SENT => new SENTTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.JTAG => new JTAGTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.CAN => new CANTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.CAN_FD => new CANFDTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.ARINC429 => new ARINC429TrigSerialPrsnt(idp, view),
            //    SerialProtocolType.LIN => new LINTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.SPMI => new SPMITrigSerialPrsnt(idp, view),
            //    SerialProtocolType.FlexRay => new FlexRayTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.AudioBus => new AudioBusTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.Ethernet => new EthernetTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.PCIe => new PCIeTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.USB => new USBTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.SATA => new SATATrigSerialPrsnt(idp, view),
            //    SerialProtocolType.MIL => new MILTrigSerialPrsnt(idp, view),
            //    SerialProtocolType.NRZ => new NRZTrigSerialPrsnt(idp, view),

            //    _ => throw new NotImplementedException(),
            //};
            trigserial.AddViewList(views);
            trigserial.SerialType = tt;
            _Current = trigserial;
            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            return trigserial;
        }

        //private static (Double Scale, Double Pos0, Prefix Pfx, String Unit) GetAxisInfo(ChannelId ts)
        //{
        //    switch (ts)
        //    {
        //        case ChannelId when ts.IsAnalog():
        //            var ach = (AnalogModel)DsoModel.Default.GetChannel(ts);
        //            return (ach.Conditioning.ScaleBymV / ach.Conditioning.PosIdxPerDiv, ach.Conditioning.PosIndex, ach.Conditioning.Prefix, ach.Conditioning.Unit);
        //        case ChannelId.Ext:
        //            return (Constants.EXT_TRIGGER_RES_MV * 1/*(TriggerPrsnt.EnableExtAtten ? 5 : 1)*/, 0, Prefix.Milli, "V");
        //        //!!!Is ChannelId.Ext5 reserved? 
        //        case ChannelId.Ext5:
        //            return (Constants.EXT_TRIGGER_RES_MV * 5, 0, Prefix.Milli, "V");
        //        //!!!What is the range of AC Line trigger's level?
        //        case ChannelId.AC:
        //            return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");
        //        case ChannelId.AuxIn:
        //            return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");
        //        default:
        //            return (Constants.EXT_TRIGGER_RES_MV, 0, Prefix.Milli, "V");

        //    }
        //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Argument {nameof(ts)} of function {nameof(GetAxisInfo)} is not a valide trigger source.", EventBus.LogLevel.Error));
        //    throw new ArgumentException($"Argument {nameof(ts)} of function {nameof(GetAxisInfo)} is not a valide trigger source.");
        //}

        private static DigitalBitCtrlGrpModel GetDigiModel(ChannelId ts)
        {
            if (ts.IsDigital())
            {
                var dch = (DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0);

                var src = ts - ChannelId.D0;

                return dch.Conditioning.Bits[src].CtrlGroup;
            }

            throw new ArgumentException($"Trigger source '{ts}' is not digital source.");
        }

        private protected TriggerModel? _Model;
        private protected override TriggerModel Model => _Model!;
    }

    public abstract class TrigMultiLevelPrsnt : TriggerPrsnt
    {
        protected TrigMultiLevelPrsnt(IDsoPrsnt idp, ITriggerView? view) : base(idp, view)
        { }

        public Double MaxCompPosition => Model.MaxCompPosition;

        public Double MinCompPosition => Model.MinCompPosition;

        public override Double PosIndex
        {
            get => PosSwitch == 0 ? Model.PosUpperIndex : Model.PosLowerIndex;
            set
            {
                if (PosSwitch == 0)
                {
                    Model.PosUpperIndex = value;
                }
                else
                {
                    Model.PosLowerIndex = value;
                }
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Prefix PosPrefix => Model.PosPrefix;

        public Int32 PosSwitch { get; set; }

        public String PosUnit => Model.PosUnit;

        public Double PosUpperIndex
        {
            get => Model.PosUpperIndex;
            set
            {
                Model.PosUpperIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double PosLowerIndex
        {
            get => Model.PosLowerIndex;
            set
            {
                Model.PosLowerIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double RelPosLowerIndex
        {
            get => Model.RelPosLowerIndex;
            set
            {
                Model.RelPosLowerIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double RelPosUpperIndex
        {
            get => Model.RelPosUpperIndex;
            set
            {
                Model.RelPosUpperIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double UpperCompPosition
        {
            get => Model.UpperCompPosition;
            set
            {
                Model.UpperCompPosition = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double LowerCompPosition
        {
            get => Model.LowerCompPosition;
            set
            {
                Model.LowerCompPosition = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigPosition);
            }
        }
        public Double VuUpperCompPosition
        {
            get
            {
                var id = Source;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.UpperCompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.UpperCompPosition;
            }
            set
            {
                var id = Source;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.UpperCompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                        return;
                    }

                }
                Model.UpperCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                KeyLed.Default.SetTriggerSrc(Source);
                TriggerSourceChanged();
            }
        }
        public Double VuLowerCompPosition
        {
            get
            {
                var id = Source;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.LowerCompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.LowerCompPosition;
            }
            set
            {
                var id = Source;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.LowerCompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigPosition);
                        return;
                    }

                }
                Model.LowerCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigPosition);
            }
        }
        public Int64 WidthByps
        {
            get => Model.WidthByps;
            set
            {
                Model.WidthByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);

            }
        }

        public Double WidthByus
        {
            get => WidthByps / 1000_000D;
            set => WidthByps = (Int64)(value * 1000_000D);
        }

        public Int64 UpperWidthByps
        {
            get => Model.UpperWidthByps;
            set
            {
                Model.UpperWidthByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double UpperWidthByus
        {
            get => UpperWidthByps / 1000_000D;
            set => UpperWidthByps = (Int64)(value * 1000_000D);
        }

        public (Int64 min, Int64 max) GetWidthRange()
        {
            return Model.GetWidthRange(0);
        }

        public (Int64 min, Int64 max) GetUpperWidthRange()
        {
            return Model.GetWidthRange(1);
        }

        public void AdjWidth(Int64 step)
        {
            if ((Model.WidthByps >= 1E6) && (Model.WidthByps < 1E9))
            {
                Model.WidthByps += step * (Int32)1E3;
            }
            else if ((Model.WidthByps >= 1E9) && (Model.WidthByps < 1E12))
            {
                Model.WidthByps += step * (Int32)1E6;
            }
            else if (Model.WidthByps >= 1E12)
            {
                Model.WidthByps += step * (Int32)1E9;
            }
            else
            {
                Model.WidthByps += step * Model.StpWidth;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public void AdjUpperWidth(Int64 step)
        {
            if ((Model.UpperWidthByps >= 1E6) && (Model.UpperWidthByps < 1E9))
            {
                Model.UpperWidthByps += step * (Int32)1E3;
            }
            else if ((Model.UpperWidthByps >= 1E9) && (Model.UpperWidthByps < 1E12))
            {
                Model.UpperWidthByps += step * (Int32)1E6;
            }
            else if (Model.UpperWidthByps >= 1E12)
            {
                Model.UpperWidthByps += step * (Int32)1E9;
            }
            else
            {
                Model.UpperWidthByps += step * Model.StpWidth;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public PulseCondition WidthCompCondition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        private protected override TriggerMultiLevelModel Model => (TriggerMultiLevelModel)_Model!;

        public override void ResetPosIndex()
        {
            if (PosSwitch == 0)
            {
                Model.PosUpperIndex = 0;
            }
            else
            {
                Model.PosLowerIndex = 0;
            }
        }
        public Double SetPosIndexCenter(ChannelId ts)
        {
            return Model.SetCompPosIndexCenter(ts);
        }

        protected override ChannelId? TriggerSource()
        {
            return this.Source;
        }
    }

    public abstract class TrigSingleSrcPrsnt : TriggerPrsnt
    {
        protected TrigSingleSrcPrsnt(IDsoPrsnt idp, ITriggerView? view) : base(idp, view)
        {
        }

        /// <summary>
        /// 电平值
        /// </summary>
        public Double CompPosition
        {
            get => Model.CompPosition;
            set
            {
                var chmodel = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Id == Source);
                if (chmodel != null)
                {
                    if ((value / chmodel.Conditioning.Scale * 1000 + chmodel.Conditioning.PosIndex) <= Model.MaxPosIndex &&
                        (value / chmodel.Conditioning.Scale * 1000 + chmodel.Conditioning.PosIndex) >= Model.MinPosIndex)
                    {
                        Model.ScreenPosIndex = value / chmodel.Conditioning.Scale * 1000;
                    }
                }
                Model.CompPosition = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Double VuCompPosition
        {
            get
            {
                var id = Source;
                if (id != null && DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.CompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.CompPosition;
            }

            set
            {
                var id = Source;
                if (id != null && DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.CompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                        return;
                    }

                }
                Model.CompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        /// <summary>
        /// 电平值
        /// </summary>
        public Double CompPositionBymV
        {
            get => CompPosition;
            set => CompPosition = value;
        }

        /// <summary>
        /// 最大电平值
        /// </summary>
        public Double MaxCompPosition => Model.MaxCompPosition;

        /// <summary>
        /// 最小电平值
        /// </summary>
        public Double MinCompPosition => Model.MinCompPosition;

        /// <summary>
        /// 电平值虚拟坐标 （相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标）
        /// </summary>
        public override Double PosIndex
        {
            get => Model.CompPosIndex;
            set
            {
                var chmodel = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Id == Source);
                if (chmodel != null)
                {
                    if ((value + chmodel.Conditioning.PosIndex) <= Model.MaxPosIndex && (value + chmodel.Conditioning.PosIndex) >= Model.MinPosIndex)
                    {
                        Model.ScreenPosIndex = value;
                    }
                }
                Model.CompPosIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Prefix PosPrefix => Model.PosPrefix;

        public String PosUnit => Model.PosUnit;

        public Double RelPosIndex
        {
            get => Model.RelPosIndex;
            set
            {
                var chmodel = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Id == Source);
                if (chmodel != null)
                {
                    if (value <= Model.MaxPosIndex && value >= Model.MinPosIndex)
                    {
                        Model.ScreenPosIndex = value - chmodel.Conditioning.PosIndex;
                    }
                }
                Model.RelPosIndex = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public ChannelId? Source
        {
            get => Model?.Source;
            set
            {
                Model.Source = value!.Value;
                if (value == ChannelId.Ext)
                {
                    EnableExtAtten = false;
                }
                else if (value == ChannelId.Ext5)
                {
                    EnableExtAtten = true;
                }
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                Hardware.HdCmdFactory.Push(HdCmd.TrigSource);
                if (value == ChannelId.AuxIn)
                {
                    // 开启辅助输入的“触发同步”功能
                    if (DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal != AuxInputType.Trigger)
                        DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal = AuxInputType.Trigger;

                    KeyLed.Default.SetTriggerSrc(ChannelId.Ext);
                }
                else
                {
                    // 关闭触发同步
                    if (DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal == AuxInputType.Trigger)
                        DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal = AuxInputType.Close;

                    KeyLed.Default.SetTriggerSrc(Source!.Value);
                }
                TriggerSourceChanged();
                Dispatcher.SoftReset();
            }
        }

        private protected override TriggerSingleSrcModel Model => (TriggerSingleSrcModel)_Model!;

        public override void ResetPosIndex()
        {
            //if (DsoModel.Default.Timebase.ScaleIndex >= AnaChnlTimebaseIndex.Lv50m)
            //{
            //    // 滚动模式时，触发电平复位按钮失效
            //    return;
            //}
            Dispatcher.SoftReset();

            Model.ResetCompPosIndex();
            Hardware.HdCmdFactory.Push(HdCmd.TrigPosition);
        }

        public void SetPosIndexCenter()
        {
            Dispatcher.SoftReset();

            //if (DsoModel.Default.Timebase.ScaleIndex >= AnaChnlTimebaseIndex.Lv50m)
            //{
            //    // 滚动模式时，触发电平复位按钮失效
            //    return;
            //}

            Model.SetCompPosIndexCenter();
            Hardware.HdCmdFactory.Push(HdCmd.TrigPosition);
        }

        protected override ChannelId? TriggerSource()
        {
            return this.Source;
        }     
    }
}
