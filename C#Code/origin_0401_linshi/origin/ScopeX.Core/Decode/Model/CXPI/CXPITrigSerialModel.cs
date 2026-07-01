using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /*未做修改*/
    internal sealed class CXPITrigSerialModel : TriggerSerialModel
    {
        private ProtocolCXPI.Condition _Condition = ProtocolCXPI.Condition.Condition_NONE;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolCXPI.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigCXPIConditionsOptions(Condition) { };
        }
    }
}
