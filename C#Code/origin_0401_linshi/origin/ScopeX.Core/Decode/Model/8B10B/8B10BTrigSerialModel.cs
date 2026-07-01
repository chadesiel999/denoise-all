using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /*未做修改*/
    internal sealed class D8B10BTrigSerialModel : TriggerSerialModel
    {
        private Protocol8B10B.Condition _Condition = Protocol8B10B.Condition.Condition_NONE;
        /// <summary>
        /// 事件类型
        /// </summary>
        public Protocol8B10B.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.Trig8B10BConditionOptions(Condition) { };
        }
    }
}
