using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Decode;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    /// <summary>
    /// 协议触发的基础model
    /// </summary>
    internal abstract class TriggerSerialModel : TriggerModel, INotifyPropertyChanged
    {
        public override String Name => TriggerType.Serial.ToString();

        public TriggerSerialModel(){ }

        private ChannelId _Source=ChannelId.None;

        public ChannelId Source
        {
            get => _Source;
            set
            {
                if(_Source!=value)
                {
                    if(value == ChannelId.None)
                    {
                        SerialType = SerialProtocolType.Close;
                    }
                    else
                    {
                        var model = DsoModel.Default.GetChannel(value);
                        if (model!=null&&model is DecodeModel dmodel && dmodel.Active)
                        {
                            SerialType = dmodel.ProtocolType;
                        }
                    }
                    TriggerSerialShareParameter.Default.Source= value;
                    _Source = value;
                    OnPropertyChanged(nameof(Source));
                }
            }
        }

        public SerialProtocolType SerialType
        {
            get => TriggerSerialShareParameter.Default.ProtocolType;
            set
            {
                if (TriggerSerialShareParameter.Default.ProtocolType != value)
                {
                    TriggerSerialShareParameter.Default.ProtocolType = value;
                    OnPropertyChanged(nameof(TriggerSerialShareParameter.ProtocolType));
                }
                if (SerialProtocolType.Close != value)
                {
                    Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                }
                else
                {
                    //Hardware.HdCmdFactory.Push(HdCmd.DecodeDisabled);//????
                }
            }
        }

        public SerialProtocolType GetSerialTypeByChannelId(ChannelId id, Boolean needactive)
        {
            var serialtype = SerialProtocolType.Close;
            if (id != ChannelId.None)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var p) && p is DecodePrsnt decodep && (decodep.Active || needactive))
                {
                    serialtype = decodep.ProtocolType;
                }
            }

            return serialtype;
        }

        public override void LeapPosIndex(){}

        public void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] String propertyName = "")
        {
            if (Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;
            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 返回协议触发的Recoder
        /// </summary>
        /// <returns></returns>
        public virtual HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return null;
        }
    }

    /// <summary>
    /// 协议触发的全局唯一参数
    /// </summary>
    internal class TriggerSerialShareParameter:INotifyPropertyChanged
    {
        private ChannelId _Source = ChannelId.None;

        public ChannelId Source
        {
            get
            {
                return _Source;
            }
            set
            {
                _Source= value;
            }
        }

        private SerialProtocolType _ProtocolType = SerialProtocolType.Close;
        /// <summary>
        /// 协议类型
        /// 为了保证触发中的协议类型与解码中的协议类型同步，此处使用了解码中的协议类型
        /// </summary>
        public SerialProtocolType ProtocolType
        {
            get => _ProtocolType;
            set
            {
                if(_ProtocolType != value)
                {
                    _ProtocolType = value;
                    OnPropertyChanged(nameof(ProtocolType));
                }
            }
        }
        /// <summary>
        /// 当前系统中所有的协议触发的Model
        /// 注意：本系统中为了代码的复用，协议触发的Model和协议触发解码的Model是分开的，同时Prsnt也是分开的
        /// </summary>
        private Dictionary<SerialProtocolType, TriggerSerialModel> _TrigSerialModels { get; } = new Dictionary<SerialProtocolType, TriggerSerialModel>();

        private Dictionary<SerialProtocolType, ProtocolModel> _TrigDecodeModels { get; } = new Dictionary<SerialProtocolType, ProtocolModel>();

        /// <summary>
        /// 获取指定协议的协议触发Model
        /// </summary>
        /// <param name="SerialProtocolType"></param>
        /// <returns></returns>
        public TriggerSerialModel GetTriggerSerial(SerialProtocolType protocolType)
        {
            if (_TrigSerialModels.TryGetValue(protocolType, out var decode))
                return decode;
            throw new NotImplementedException($"{protocolType} trigger is not supported!");
        }

        /// <summary>
        /// 获取系统当前的协议触发的Model
        /// </summary>
        /// <returns></returns>
        public TriggerSerialModel GetTriggerSerial()
        {
            return GetTriggerSerial(ProtocolType);
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] String propertyName = "")
        {
            if (Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;

            OnPropertyChanged(propertyName);
        }

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

        #endregion

        private TriggerSerialShareParameter()
        {
            _TrigSerialModels.Add(SerialProtocolType.Close, new CloseTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.RS232, new RS232TrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.I2C, new I2CTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.SPI, new SPITrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.SENT, new SENTTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.JTAG, new JTAGTrigSerialModel());
			_TrigSerialModels.Add(SerialProtocolType.CAN, new CANTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.ARINC429 , new ARINC429TrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.AudioBus , new AudioBusTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.CAN_FD , new CANFDTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.FlexRay , new FlexRayTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.LIN , new LINTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.MIL , new MILTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.PCIe , new PCIeTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.SATA , new SATATrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.SPMI , new SPMITrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.USB , new UsbTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.Ethernet, new EthernetTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.NRZ, new NRZTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.CPHY, new CPHYTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.Manchester, new ManchesterTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.I3C, new I3CTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.PSI5, new PSI5TrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.SMBus, new SMBusTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.Mlt3, new Mlt3TrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.Common_8b10b, new D8B10BTrigSerialModel());
            _TrigSerialModels.Add(SerialProtocolType.CXPI, new CXPITrigSerialModel());
        }
        public static TriggerSerialShareParameter Default { get; } = new Lazy<TriggerSerialShareParameter>(() => new TriggerSerialShareParameter()).Value;
    }
}
