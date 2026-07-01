using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class SPMITrigSerialModel : TriggerSerialModel
    {
        private ProtocolSPMI.Condition _Condition = ProtocolSPMI.Condition.WriteExRegister;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolSPMI.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigSPMIConditionsOptions(Condition) { };
        }
    }
}
