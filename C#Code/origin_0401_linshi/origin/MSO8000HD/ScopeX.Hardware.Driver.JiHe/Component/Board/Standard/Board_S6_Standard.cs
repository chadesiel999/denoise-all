using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class S6Bd_Standard : AbstractS6Bd
    {
      
        public override void Init()
        {
            //开机初始化，需使用内部10M时钟
           
            Config7044_A();
            HdIO.Sleep(200);
            Config7044_B();
            HdIO.Sleep(200);
            Config7044_C();
            HdIO.Sleep(200);

            //cij_jiuyuan_new_clk
            ReadFpgaVersion();
        }
        //cij_jiuyuan_new_clk
        //HMC7044A
        private void Config7044_A()
        {
            if (ConfigData7044A_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in ConfigData7044A_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_A(regAddrValuePair.address, regAddrValuePair.value);
            }
            HdIO.Sleep(50);
            HdCtrl_Pll.PllSync_A();
            HdIO.Sleep(200);

            if (SpecialConfigData7044A_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in SpecialConfigData7044A_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_A(regAddrValuePair.address, regAddrValuePair.value);
            }
        }
        //HMC7044B
        private void Config7044_B()
        {
            if (ConfigData7044B_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in ConfigData7044B_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_B(regAddrValuePair.address, regAddrValuePair.value);
            }
            HdIO.Sleep(50);
            HdCtrl_Pll.PllSync_A();
            HdIO.Sleep(200);

            if (SpecialConfigData7044B_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in SpecialConfigData7044B_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_B(regAddrValuePair.address, regAddrValuePair.value);
            }
        }
        //HMC7044C
        private void Config7044_C()
        {
            if (ConfigData7044C_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in ConfigData7044C_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_C(regAddrValuePair.address, regAddrValuePair.value);
            }
            HdIO.Sleep(50);
            HdCtrl_Pll.PllSync_A();
            HdIO.Sleep(200);

            if (SpecialConfigData7044C_Schame40G12BIT != null)
            {
                foreach (var regAddrValuePair in SpecialConfigData7044C_Schame40G12BIT)
                    HdCtrl_Pll.PllWrite7044_C(regAddrValuePair.address, regAddrValuePair.value);
            }
        }
        //cij_jiuyuan_new_clk
    }
}