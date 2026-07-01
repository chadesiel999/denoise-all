using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// Close协议触发的基础model
    /// </summary>
    internal sealed class CloseTrigSerialModel : TriggerSerialModel
    {

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigCloseConditionsOptions() { };
        }
    }
}
