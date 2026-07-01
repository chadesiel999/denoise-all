using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal class CPHYTrigSerialModel : TriggerSerialModel
    {
        public CPHYTrigSerialModel()
        {
            _DestMAC = new Byte[MACByteCount];
            _SrcMAC = new Byte[MACByteCount];
            MaxMACAddress = (Int64)(Math.Pow(2, MACByteCount * 8) - 1);
        }
        //private ComModel.ProtocolCPHY.Condition _Condition = ComModel.ProtocolCPHY.Condition.FrameHead;

        //public ComModel.ProtocolCPHY.Condition Condition
        //{
        //    get { return _Condition; }
        //    set { UpdateProperty(ref _Condition, value); }
        //}

        public Int64 MaxMACAddress { get; }
        public Int64 MinMACAddress { get; } = 0;
        public Int32 MACByteCount { get; } = 6;
        public Byte MinData => 0;
        public Byte MaxData => Byte.MaxValue;
        private Byte _Data;

        public Byte Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

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
        //private ProtocolCPHY.DataRelation _Relation = ProtocolCPHY.DataRelation.Eq;

        //public ProtocolCPHY.DataRelation Relation
        //{
        //    get { return _Relation; }
        //    set { UpdateProperty(ref _Relation, value); }
        //}


        //public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        //{
        //    return new HdMessage.TrigCPHYConditionsOptions(Condition)
        //    {
        //        Data = Data,
        //        DestMAC = DestMAC,
        //        SrcMAC = SrcMAC,
        //        Relation = Relation,
        //    };
        //}
    }
}
