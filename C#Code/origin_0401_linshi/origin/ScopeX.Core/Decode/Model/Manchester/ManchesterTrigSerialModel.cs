using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class ManchesterTrigSerialModel : TriggerSerialModel
    {
        private ProtocolManchester.Condition _Condition = ProtocolManchester.Condition.Condition_NONE;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolManchester.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigManchesterConditionOptions(Condition) { };
        }
    }
}
