using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolMIL;

namespace ScopeX.Core.Decode
{
    internal sealed class MILTrigSerialModel : TriggerSerialModel
    {
        public MILTrigSerialModel()
        {
            //_DestMAC = new Byte[MACByteCount];
            //_SrcMAC = new Byte[MACByteCount];
            //_MACLengthOrType = new Byte[MACLengthOrTypeByteCount];
            //_QTagInfo = new Byte[QTagInfoByteCount];
            //_Data = new Byte[MaxDataByteLength];
            //MaxMACAddress = (Int64)(Math.Pow(2, MACByteCount * 8) - 1);
            //MaxMACLengthOrType = (Int64)(Math.Pow(2, MACLengthOrTypeByteCount * 8) - 1);
            //MaxQTagInfo = (Int64)(Math.Pow(2, QTagInfoByteCount * 8) - 1);
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        //private ProtocolMIL.Condition _Condition = ProtocolMIL.Condition.Condition_NONE;
        private ProtocolMIL.Condition _Condition = ProtocolMIL.Condition.Sync;

        public ProtocolMIL.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        public Int32 MaxRTA { get; } = 31;
        public Int32 MinRTA { get; } = 0;
        private Int32 _RTA = 0;
        public Int32 RTA
        {
            get { return _RTA; }
            set { UpdateProperty(ref _RTA, value); }
        }

        public Int32 MaxData { get; } = (Int32)(Math.Pow(2, 16) - 1);
        public Int32 MinData { get; } = 0;
        private Int32 _Data = 0;
        public Int32 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

        public Int32 MaxPairty { get; } = 1;
        public Int32 MinPairty { get; } = 0;
        private Int32 _Pairty = 0;
        public Int32 Pairty
        {
            get { return _Pairty; }
            set { UpdateProperty(ref _Pairty, value); }
        }

        private ProtocolMIL.ErrorType _ErrorType = ProtocolMIL.ErrorType.Notcontinue;
        public ProtocolMIL.ErrorType ErrorType
        {
            get { return _ErrorType; }
            set { UpdateProperty(ref _ErrorType, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigMILConditionsOptions(Condition)
            {
                Data = Data,
                Pairty = Pairty,
                RTA = RTA,
                ErrorType = ErrorType
            };
        }
    }
}
