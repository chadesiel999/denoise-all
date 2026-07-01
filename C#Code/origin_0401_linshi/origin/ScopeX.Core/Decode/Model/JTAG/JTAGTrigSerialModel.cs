using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal class JTAGTrigSerialModel : TriggerSerialModel
    {


        private ComModel.ProtocolJTAG.Condition _Condition = ProtocolJTAG.Condition.UPDAET_DR;

        public ComModel.ProtocolJTAG.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigJTAGConditionsOptions(Condition)
            {
            };
        }
    }
}
