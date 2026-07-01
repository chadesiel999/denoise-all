using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class I3CTrigSerialModel : TriggerSerialModel
    {
        private ProtocolI3C.Condition _Condition = ProtocolI3C.Condition.Condition_NONE;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolI3C.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigI3CConditionsOptions(Condition) { };
        }
    }
}
