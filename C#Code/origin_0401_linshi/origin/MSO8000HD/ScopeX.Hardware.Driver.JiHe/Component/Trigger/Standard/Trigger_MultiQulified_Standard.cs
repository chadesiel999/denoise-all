#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlTrigger_Standard
    {
        internal static uint GetTriggerSource_MultiQulified()
        {
            return (uint)ChannelId.C1;
        }
        //使用internal  是为了共用
        static Dictionary<string, KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>> MultiQulifiedDefine = new Dictionary<string, KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>>()
        {
            ["Edge"]=new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(0, CtrlTrigger_Standard.Config_Edge_MultiQulified),
            ["PulseWidth"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(1, CtrlTrigger_Standard.Config_PulseWidth_MultiQulified),
            ["Transition"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(2, CtrlTrigger_Standard.Config_Transition_MultiQulified),
            ["Runt"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(3, CtrlTrigger_Standard.Config_Runt_MultiQulified),
            ["TimeOut"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(4, CtrlTrigger_Standard.Config_TimeOut_MultiQulified),
        };
        internal static void Config_MultiQulified()
        {
            throw new NotImplementedException($" this type not Implemented");
        }
    }
}
#endif
