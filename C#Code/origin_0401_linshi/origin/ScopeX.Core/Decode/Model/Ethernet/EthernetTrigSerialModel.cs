using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal class EthernetTrigSerialModel : TriggerSerialModel
    {
        public EthernetTrigSerialModel()
        {
            _DestMAC = new Byte[MACByteCount];
            _SrcMAC = new Byte[MACByteCount];
            _MACLengthOrType = new Byte[MACLengthOrTypeByteCount];
            _QTagInfo = new Byte[QTagInfoByteCount];
            _Data = new Byte[MaxDataByteLength];
            MaxMACAddress = (Int64)(Math.Pow(2, MACByteCount * 8) - 1);
            MaxMACLengthOrType = (Int64)(Math.Pow(2, MACLengthOrTypeByteCount * 8) - 1);
            MaxQTagInfo = (Int64)(Math.Pow(2, QTagInfoByteCount * 8) - 1);
        }

        private ComModel.ProtocolEthernet.Condition _Condition = ComModel.ProtocolEthernet.Condition.FrameHead;
        public ComModel.ProtocolEthernet.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        public Int64 MinData
        {
            get
            {
                return (Int64)(0-(UInt64)(Math.Pow(2, DataByteLength * 8 - 1) - 1));
            }
        }
        public Int64 MaxData
        {
            get
            {
                if (DataByteLength * 8 == 64)
                {
                    return Int64.MaxValue;
                }
                else
                {
                    return (Int64)(Math.Pow(2, DataByteLength * 8 - 1) - 1);
                }
            }
        }

        private const Int32 _MINDATABYTELENGTH = 1;
        private const Int32 _MAXDATABYTELENGTH = 8;
        public Int32 MaxDataByteLength { get; } = _MAXDATABYTELENGTH;
        public Int32 MinDataByteLength { get; } = _MINDATABYTELENGTH;
        private Byte[] _Data;
        public Byte[] Data
        {
            get { return _Data; }
            set
            {
                if (value.Length > MaxDataByteLength)
                {
                    UpdateProperty(ref _Data, value.Take(MaxDataByteLength).ToArray());
                }
                else if (value.Length < MinDataByteLength)
                {
                    UpdateProperty(ref _Data, value.Take(MinDataByteLength).ToArray());
                }
                else
                {
                    UpdateProperty(ref _Data, value);
                }  
            }
        }

        public Int64 MaxMACAddress { get; }
        public Int64 MinMACAddress { get; } = 0;
        public Int32 MACByteCount { get; } = 6;
        private Byte[] _SrcMAC;
        public Byte[] SrcMAC
        {
            get { return _SrcMAC; }
            set { UpdateProperty(ref _SrcMAC, value); }
        }

        private Byte[] _DestMAC;
        public Byte[] DestMAC
        {
            get { return _DestMAC; }
            set { UpdateProperty(ref _DestMAC, value); }
        }

        public Int64 MaxMACLengthOrType { get; }
        public Int64 MinMACLengthOrType { get; } = 0;
        public Int32 MACLengthOrTypeByteCount { get; } = 2;
        private Byte[] _MACLengthOrType;
        public Byte[] MACLengthOrType
        {
            get { return _MACLengthOrType; }
            set { UpdateProperty(ref _MACLengthOrType, value); }
        }

        public Int64 MaxQTagInfo { get; }
        public Int64 MinQTagInfo { get; } = 0;
        public Int32 QTagInfoByteCount { get; } = 2;
        private Byte[] _QTagInfo;
        public Byte[] QTagInfo
        {
            get { return _QTagInfo; }
            set { UpdateProperty(ref _QTagInfo, value); }
        }

        private Int32 _DataByteLength = 1;
        public Int32 DataByteLength
        {
            get { return _DataByteLength; }
            set { UpdateProperty(ref _DataByteLength, value); }
        }

        public Int32 MaxDataOffset { get; } = 1499;
        public Int32 MinDataOffset { get; } = 0;
        private Int32 _DataOffset = 0;
        public Int32 DataOffset
        {
            get { return _DataOffset; }
            set { UpdateProperty(ref _DataOffset, value); }
        }

        private ProtocolEthernet.DataRelation _Relation = ProtocolEthernet.DataRelation.Eq;
        public ProtocolEthernet.DataRelation Relation
        {
            get { return _Relation; }
            set { UpdateProperty(ref _Relation, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigEthernetConditionsOptions(Condition)
            {
                Data = Data,
                DataByteLength = DataByteLength,    
                DataOffset = DataOffset,    
                DestMAC = DestMAC,
                SrcMAC = SrcMAC,
                MACLengthOrType = MACLengthOrType,
                QTagInfo = QTagInfo,
                Relation = Relation,
            };
        }
    }
}
