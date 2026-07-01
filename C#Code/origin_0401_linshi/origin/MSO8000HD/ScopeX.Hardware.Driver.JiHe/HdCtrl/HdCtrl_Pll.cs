using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal static class HdCtrl_Pll
    {
        internal static void PllSync()
        {
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 0);
        }

        internal static void PllWrite7044(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
        }

        internal static void PllWrite7043(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
        }

        //cij_jiuyuan_new_clk
        internal static void PllSync_A()
        {
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044ASync, 0);
            HdIO.Sleep(10);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044ASync, 1);
            HdIO.Sleep(10);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044ASync, 0);
            HdIO.Sleep(10);
        }
        //cij_jiuyuan_new_clk

        //cij_jiuyuan_new_clk
        //HMC7044A
        internal static void PllWrite7044_A(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044AWriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044AWriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044AWriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044AWriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }
        //HMC7044B
        internal static void PllWrite7044_B(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044BWriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044BWriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044BWriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044BWriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }
        //HMC7044C
        internal static void PllWrite7044_C(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044CWriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044CWriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044CWriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044CWriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }
        //cij_jiuyuan_new_clk

        internal static void PllWrite7043_2(UInt32 addr, UInt32 data)
        {
            //UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            //HdIO.WriteReg(S6BdReg.W.PllConfig_7043_2WriteDataEffect, 0);
            //HdIO.WriteReg(S6BdReg.W.PllConfig_7043_2WriteData_L16, temp & 0xffff);
            //HdIO.WriteReg(S6BdReg.W.PllConfig_7043_2WriteData_H8, (temp >> 16) & 0xff);
            //HdIO.WriteReg(S6BdReg.W.PllConfig_7043_2WriteDataEffect, 1);
            //HdIO.WaitForSpiTransfer(1, 4);
            //HdIO.Sleep(1);
        }

        /// <summary>
        /// 设置7044 Pll的10M参考时钟的输入
        /// </summary>
        /// <param name="clkSrc">时钟源类型</param>
        internal static void SetPll10MSyncClk(AnaChnlClkSrc clkSrc)
        {
            //HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Ext_10m_Sel, Hd.UIMessage!.Timebase!.ClockSrc == AnaChnlClkSrc.Inner ? 0U : 1U);

            (UInt32 addr05Value, UInt32 addr14Value) Value = clkSrc switch
            {
                AnaChnlClkSrc.Outter => (0x82, 0x55),
                _ => (0x81, 0x00),   //默认使用内部时钟
            };
            PllWrite7044(0x0005, Value.addr05Value);
            PllWrite7044(0x0014, Value.addr14Value);
        }
    }
}
