using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal class NRZTrigSerialModel : TriggerSerialModel
    {
        private ProtocolNRZ.Condition _Condition = ProtocolNRZ.Condition.StartFrame;

        public ProtocolNRZ.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public Byte MinData => Byte.MinValue;
        public Byte MaxData => Byte.MaxValue;
        private Byte _Data;

        public Byte Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }
        private ProtocolNRZ.DataRelation _Relation = ProtocolNRZ.DataRelation.Eq;

        public ProtocolNRZ.DataRelation Relation
        {
            get { return _Relation; }
            set { UpdateProperty(ref _Relation, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigNRZConditionOptions(Condition)
            {
                Data = Data,
                Relation = Relation,
            };
        }
    }
}
