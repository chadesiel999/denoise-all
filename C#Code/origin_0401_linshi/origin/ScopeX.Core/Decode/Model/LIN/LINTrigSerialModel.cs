using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class LINTrigSerialModel : TriggerSerialModel
    {

        private ProtocolLIN.Condition _Condition = ProtocolLIN.Condition.Data;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolLIN.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        private ProtocolLIN.DataRelation _DataRelation = ProtocolLIN.DataRelation.Eq;

        public ProtocolLIN.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }

        private Byte _ID;

        public Byte ID
        {
            get { return _ID; }
            set { UpdateProperty(ref _ID, value); }
        }
        public Byte MaxID => Byte.MaxValue;
        public Byte MinID => Byte.MinValue;

        private Int64 _Data;

        public Int64 Data
        {
            get { return _Data; }
            set 
            {
                _ByteCount = (Byte)Math.Ceiling(Convert.ToString(value, 16).Length / 2D);
                UpdateProperty(ref _Data, value);
                //_ByteCount = (Byte)Math.Ceiling(Convert.ToString(_Data, 16).Length / 2D);
            }
        }
        public Int64 MaxData = (Int64)Math.Pow(2, 8 * 8) - 1;//281474976710655;
        public Int64 MinData = 0;

        private Byte _ByteCount = 1;

        public Byte ByteCount
        {
            get { return _ByteCount; }
            set { UpdateProperty(ref _ByteCount, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigLINConditionsOptions(Condition)
            {
                Data = Data,
                DataRelation = DataRelation,
                ID = ID,
                ByteCount = ByteCount,
            };
        }
    }
}
