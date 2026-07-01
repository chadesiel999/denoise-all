using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// rs232协议触发的基础model
    /// </summary>
    internal sealed class RS232TrigSerialModel : TriggerSerialModel
    {

        private ProtocolRS232.Conditions _Conditions = ProtocolRS232.Conditions.FrameStart;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolRS232.Conditions Conditions
        {
            get { return _Conditions; }
            set { UpdateProperty(ref _Conditions, value); }
        }

        private PulseCondition _Compare = PulseCondition.Equal;
        /// <summary>
        /// 比较方式
        /// </summary>
        public PulseCondition Compare
        {
            get { return _Compare; }
            set { UpdateProperty(ref _Compare, value); }
        }

        public UInt32 MinData => UInt32.MinValue;
        public UInt32 MaxData => (UInt32)(Math.Pow(2, 10) - 1);
        private UInt32 _Data = 0;
        /// <summary>
        /// 数据
        /// </summary>
        public UInt32 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

        public UInt32 MinDataLength => 1;
        public UInt32 MaxDataLength => 8;
        public UInt32 _DataLength = 1;
        /// <summary>
        /// 数据
        /// </summary>
        public UInt32 DataLength
        {
            get { return _DataLength; }
            set { UpdateProperty(ref _DataLength, value); }
        }


        public Char MaxEOP => Char.MaxValue;
        public Char MinEOP => Char.MinValue;

        private Char _EOPChar = (char)0x00;
        /// <summary>
        /// EOP数据
        /// </summary>
        public Char EOPChar
        {
            get { return _EOPChar; }
            set { UpdateProperty(ref _EOPChar, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigRS232ConditionsOptions(Conditions)
            {
                Compare = Compare,
                DataLength = DataLength,
                Data = Data,
                EOPChar = EOPChar,
            };
        }
    }
}
