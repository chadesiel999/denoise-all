#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class HdCtrl_Trigger
    {
        internal static void Config_Video()
        {
            uint trigmode = (uint)(Hd.UIMessage?.Trigger?.Video?.Sync ?? 0);
            uint trigstandard = (uint)(Hd.UIMessage?.Trigger?.Video?.Standard ?? 0);
            uint trigline = (uint)(Hd.UIMessage?.Trigger?.Video?.Line ?? 1);
            uint trigfield = (uint)(Hd.UIMessage?.Trigger?.Video?.Field ?? 1);
            uint videostandard = 0;

            switch (trigstandard)
            {
                case 0:
                case 2:
                    videostandard = 0x00;
                    break;
                case 1:
                case 3:
                    videostandard = 0x01;
                    break;
                case 4:
                    videostandard = 0x02;
                    break;
                case 5:
                    videostandard = 0x03;
                    break;
                case 6:
                    videostandard = 0x04;
                    break;
                case 7:
                    videostandard = 0x07;
                    break;
                case 8:
                    videostandard = 0x06;
                    break;
                default:
                    break;
            }

              //HdIO.WriteReg(PcieBdReg.W.TrigCtrl_viedo_TrigMode, trigmode);     
              //HdIO.WriteReg(PcieBdReg.W.TrigCtrl_viedo_SyncNumber, trigline);     //
              //HdIO.WriteReg(PcieBdReg.W.TrigCtrl_viedo_CustomHorizontal, trigfield);
              //HdIO.WriteReg(PcieBdReg.W.TrigCtrl_viedo_VideoMode, videostandard);  //



           

        }
    }
}
#endif