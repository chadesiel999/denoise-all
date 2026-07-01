using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class ARINC429TrigSerialModel : TriggerSerialModel
    {
        private ProtocolARINC429.Condition _Condition = ProtocolARINC429.Condition.FrameHead;

        private ProtocolARINC429.DecodeMode _DecodeMode;

        public ProtocolARINC429.DecodeMode DecodeMode
        {
            get { return _DecodeMode; }
            set { UpdateProperty(ref _DecodeMode, value); }
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        private UInt32 _SSM = 0;
        public UInt32 SSM
        {
            get { return _SSM; }
            set { UpdateProperty(ref _SSM, value); }
        }
        public UInt32 MinSSM => 0;
        public UInt32 MaxSSM => 3;
        public UInt32 MinLabel => 0;
        public UInt32 MaxLabel => Byte.MaxValue;
        public UInt32 MinSDI => 0;
        public UInt32 MaxSDI => 3;
        public UInt32 MinData => 0;
        public UInt32 MaxData => (UInt32)0x7_FFFF; //(UInt32)(Math.Pow(2, 23) - 1);
        private UInt32 _SDI = 0;
        public UInt32 SDI
        {
            get { return _SDI; }
            set { UpdateProperty(ref _SDI, value); }
        }

        private UInt32 _Label = 0;
        public UInt32 Label
        {
            get { return _Label; }
            set { UpdateProperty(ref _Label, value); }
        }

        private UInt32 _Data = 0;
        public UInt32 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

        private ProtocolARINC429.DataRelation _DataRelation;
        public ProtocolARINC429.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }


        public ProtocolARINC429.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolARINC429.ErrorType _ErrorType = ProtocolARINC429.ErrorType.AnyError;
        public ProtocolARINC429.ErrorType ErrorType
        {
            get { return _ErrorType; }
            set { UpdateProperty(ref _ErrorType, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigARINC429ConditionsOptions(Condition)
            {
                Label = Label,
                Data = Data,
                DataRelation = DataRelation,
                SDI = SDI,
                SSM = SSM,
                ErrorType = ErrorType
            };
        }
    }
}
