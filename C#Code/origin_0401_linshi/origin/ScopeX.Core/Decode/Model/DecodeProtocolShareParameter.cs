using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using System.Linq;
using NPOI.Util;

namespace ScopeX.Core.Decode
{
    internal class DecodeProtocolShareParameter : INotifyPropertyChanged
    {
        private Object _Locker = new Object();
        static DecodeProtocolShareParameter() { }

        private DecodeProtocolShareParameter()
        {
        }
        public void SetNeedReadData()
        {
            lock (_Locker)
            {
                if (NeedReadDecodeData) return;
                NeedReadDecodeData = true;
            }
        }
        public void ClearNeedReadData()
        {
            lock (_Locker)
            {
                NeedReadDecodeData = false;
            }
        }
        public Boolean NeedReadDecodeData { get; private set; } = false;

        public static DecodeProtocolShareParameter Default { get; } = new Lazy<DecodeProtocolShareParameter>(() => new DecodeProtocolShareParameter()).Value;

        #region 方法

        ///// <summary>
        ///// 获取指定通道、指定协议的Prsnt
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="view"></param>
        ///// <param name="SerialProtocolType"></param>
        ///// <returns></returns>
        //public ProtocolPrsnt GetChDecodePrsnt(ChannelId id, IDsoPrsnt idp, SerialProtocolType SerialProtocolType, IProtocolView? view)
        //{
        //    return SerialProtocolType switch
        //    {
        //        SerialProtocolType.Close => new CloseDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.I2C => new I2CDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.RS232 => new RS232DecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.SPI => new SPIDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.SENT => new SENTDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.JTAG => new JTAGDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.CAN => new CANDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.ARINC429 => new ARINC429DecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.AudioBus => new AudioBusDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.CAN_FD => new CANFDDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.FlexRay => new FlexRayDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.LIN => new LINDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.MIL => new MILDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.PCIe => new PCIeDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.SATA => new SATADecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.SPMI => new SPMIDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.USB => new USBDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.Ethernet => new EthernetDecodePrsnt(id, idp, view, false),
        //        SerialProtocolType.NRZ => new NRZDecodePrsnt(id, idp, view, false),

        //        _ => throw new NotSupportedException(),
        //    };
        //}

        //public ProtocolModel GetChannelDecodeModel(SerialProtocolType serialProtocolType)
        //{
        //    return serialProtocolType switch
        //    {
        //        SerialProtocolType.Close => new CloseDecodeModel(false),
        //        SerialProtocolType.RS232 => new RS232DecodeModel(false),
        //        SerialProtocolType.I2C => new I2CDecodeModel(false),
        //        SerialProtocolType.SPI => new SPIDecodeModel(false),
        //        SerialProtocolType.SENT => new SENTDecodeModelCPP(false),
        //        SerialProtocolType.JTAG => new JTAGDecodeModel(false),
        //        SerialProtocolType.CAN => new CANDecodeModelCPP(false),
        //        SerialProtocolType.ARINC429 => new ARINC429DecodeModelCPP(false),
        //        SerialProtocolType.AudioBus => new AudioBusDecodeModelCPP(false),
        //        SerialProtocolType.CAN_FD => new CANFDDecodeModelCPP(false),
        //        SerialProtocolType.FlexRay => new FlexRayDecodeModelCPP(false),
 
        //        SerialProtocolType.LIN => new LINDecodeModelCPP(false),
        //        SerialProtocolType.MIL => new MILDecodeModelCPP(false),
        //        SerialProtocolType.PCIe => new PCIeDecodeModel(false),
        //        SerialProtocolType.SATA => new SATADecodeModel(false),
        //        SerialProtocolType.SPMI => new SPMIDecodeModel(false),
        //        SerialProtocolType.USB => new USBDecodeModel(false),
        //        SerialProtocolType.Ethernet => new EthernetDecodeModel(false),
        //        SerialProtocolType.NRZ => new NRZDecodeModel(false),
        //        _ => throw new NotSupportedException(),
        //    };
        //}

        #endregion 方法

        #region INotifyPropertyChanged

        protected PropertyChangedEventHandler? _PropertyChanged;
        public void OnPropertyChanged(ProtocolModel model, [CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(model, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateProperty<T>(ProtocolModel model, ref T properValue, T newValue, [CallerMemberName] String propertyName = "")
        {
            if (Equals(properValue, newValue))
                return;

            properValue = newValue;

            OnPropertyChanged(model, propertyName);
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

        #endregion INotifyPropertyChanged

    }
}
