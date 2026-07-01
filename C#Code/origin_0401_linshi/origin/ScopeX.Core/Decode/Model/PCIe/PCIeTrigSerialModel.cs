using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class PCIeTrigSerialModel : TriggerSerialModel
    {

        private ProtocolPCIe.DataRelation _DataRelation = ProtocolPCIe.DataRelation.Lt;

        public ProtocolPCIe.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }

        private ProtocolPCIe.Condition _Condition = ProtocolPCIe.Condition.Command;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolPCIe.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolPCIe.TLPType _TLPType = ProtocolPCIe.TLPType.MRD;

        public ProtocolPCIe.TLPType TLPType
        {
            get { return _TLPType; }
            set { UpdateProperty(ref _TLPType, value); }
        }

        public UInt16 MinSeqID => 0;
        public UInt16 MaxSeqID { get; } = (UInt16)(Math.Pow(2, 12) - 1);
        private UInt16 _SeqID;

        public UInt16 SeqID
        {
            get
            {
                _SeqID = (UInt16)((UInt16)(_BusNumber << 8) | (UInt16)(_DeviceNumber << 3) | (UInt16)_FunctionNumber);
                return _SeqID;
            }
            set
            {
                BusNumber = (Byte)(value >> 8);
                DeviceNumber = (Byte)((value & 0xFF) >> 3);
                FunctionNumber = (Byte)(value & 0b111);
                UpdateProperty(ref _SeqID, value);
            }
        }

        public Byte MinTCData => 0;
        public Byte MaxTCData { get; } = (Byte)(Math.Pow(2, 3) - 1);
        private Byte _TCData;

        public Byte TCData
        {
            get { return _TCData; }
            set { UpdateProperty(ref _TCData, value); }
        }

        public Byte MinATData => 0;
        public Byte MaxATData { get; } = (Byte)(Math.Pow(2, 2) - 1);
        private Byte _ATData;

        public Byte ATData
        {
            get { return _ATData; }
            set { UpdateProperty(ref _ATData, value); }
        }

        public Byte MinTagData => 0;
        public Byte MaxTagData { get; } = Byte.MaxValue;
        private Byte _TagData;

        public Byte TagData
        {
            get { return _TagData; }
            set { UpdateProperty(ref _TagData, value); }
        }
        public UInt16 MinReqIDData => 0;
        public UInt16 MaxReqIDData { get; } = (UInt16)(Math.Pow(2, 16) - 1);
        private UInt16 _ReqIDData;

        public UInt16 ReqIDData
        {
            get { return _ReqIDData; }
            set { UpdateProperty(ref _ReqIDData, value); }
        }

        public Byte MinMsgCodeData => 0;
        public Byte MaxMsgCodeData { get; } = Byte.MaxValue;
        private Byte _MsgCodeData;

        public Byte MsgCodeData
        {
            get { return _MsgCodeData; }
            set { UpdateProperty(ref _MsgCodeData, value); }
        }
        public Byte MinDataLenght => 1;
        public Byte MaxDataLenght => 8;
        private Byte _DataLenght = 1;

        public Byte DataLenght
        {
            get { return _DataLenght; }
            set
            {
                UpdateProperty(ref _DataLenght, value);
            }
        }
        public Int64 MinData => DataLenght == 8 ? Int64.MinValue : 0;
        public Int64 MaxData => DataLenght == 8 ? Int64.MaxValue : (Int64)(Math.Pow(2, DataLenght * 8) - 1);
        private Int64 _Data;

        public Int64 Data
        {
            get { return Math.Clamp(_Data, MinData, MaxData); }
            set { UpdateProperty(ref _Data, value); }
        }
        public Int64 MinAddressData => Int64.MinValue;
        public Int64 MaxAddressData { get; } = Int64.MaxValue;
        private Int64 _AddressData;

        public Int64 AddressData
        {
            get { return _AddressData; }
            set { UpdateProperty(ref _AddressData, value); }
        }


        public Byte MinBusNumber => 0;
        public Byte MaxBusNumber => Byte.MaxValue;
        private Byte _BusNumber;

        public Byte BusNumber
        {
            get { return _BusNumber; }
            set { UpdateProperty(ref _BusNumber, value); }
        }
        public Byte MinDeviceNumber => 0;
        public Byte MaxDeviceNumber { get; } = (Byte)(Math.Pow(2, 5) - 1);
        private Byte _DeviceNumber;

        public Byte DeviceNumber
        {
            get { return _DeviceNumber; }
            set { UpdateProperty(ref _DeviceNumber, value); }
        }

        public Byte MinFunctionNumber => 0;
        public Byte MaxFunctionNumber { get; } = (Byte)(Math.Pow(2, 3) - 1);
        private Byte _FunctionNumber;

        public Byte FunctionNumber
        {
            get { return _FunctionNumber; }
            set { UpdateProperty(ref _FunctionNumber, value); }
        }



        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigPCIeConditionsOptions(Condition)
            {
                AddressData = AddressData,
                ATData = ATData,
                Data = Data,
                DataLenght = DataLenght,
                MsgCodeData = MsgCodeData,
                ReqIDData = ReqIDData,
                SeqID = SeqID,
                TagData = TagData,
                TCData = TCData,
                TLPType = TLPType,
                DataRelation = DataRelation,
            };
        }
    }
}
