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
        internal static uint GetTriggerSource_Serial()
        {
            return (uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Serial()
        {
            //throw new NotImplementedException($" this type not Implemented");
        }
    }
}
#endif
