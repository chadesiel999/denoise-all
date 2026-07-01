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
        internal static uint GetTriggerSource_Video()
        {
            return (uint)(Hd.UIMessage?.Trigger?.Video?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Video()
        {
            uint trigmode = (uint)(Hd.UIMessage?.Trigger?.Video?.Sync ?? 0);
            uint trigstandard = (uint)(Hd.UIMessage?.Trigger?.Video?.Standard ?? 0);
            uint trigline = (uint)(Hd.UIMessage?.Trigger?.Video?.Line ?? 1);
            uint trigfield = (uint)(Hd.UIMessage?.Trigger?.Video?.Field ?? 1);
            uint trigType = (uint)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgVideoTriMode, trigmode);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgSynTriNum, trigline);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgvVideoMode, trigstandard);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, trigType);

        }
    }
}
#endif