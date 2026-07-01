using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class SATATrigSerialModel : TriggerSerialModel
    {
        private ProtocolSATA.Condition _Condition = ProtocolSATA.Condition.DATA;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolSATA.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private Int64 _Data;

        public Int64 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }
        public Int64 MaxData => DataCount == 8 ? Int64.MaxValue : (Int64)Math.Pow(2, 8 * DataCount) - 1;
        public Int64 MinData => DataCount == 8 ? Int64.MinValue : 0;
        private ProtocolSATA.DataRelation _Relation = ProtocolSATA.DataRelation.Eq;

        public ProtocolSATA.DataRelation Relation
        {
            get { return _Relation; }
            set { UpdateProperty(ref _Relation, value); }
        }
        private ProtocolSATA.FISTypeFlag _FISType = ProtocolSATA.FISTypeFlag.R_H2D;

        public ProtocolSATA.FISTypeFlag FISType
        {
            get { return _FISType; }
            set { UpdateProperty(ref _FISType, value); }
        }

        public Byte MinDataCount { get; } = 1;
        public Byte MaxDataCount { get; } = 8;
        private Byte _DataCount = 1;

        public Byte DataCount
        {
            get { return _DataCount; }
            set { UpdateProperty(ref _DataCount, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigSATAConditionsOptions(Condition)
            {
                Data = Data,
                Relation = Relation,
                DataCount = DataCount,
                FISType = FISType,
            };
        }
    }
}
