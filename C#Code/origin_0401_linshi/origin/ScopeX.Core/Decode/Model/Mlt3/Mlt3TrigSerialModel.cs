using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class Mlt3TrigSerialModel : TriggerSerialModel
    {
        private ProtocolMlt3.Condition _Condition = ProtocolMlt3.Condition.Condition_NONE;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolMlt3.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigMlt3ConditionOptions(Condition) { };
        }
    }
}
