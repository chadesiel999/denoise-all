using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 采集板插值使能寄存器（AcqBdReg.W.Interpolate_EnableAcq）的寄存器发送器
    /// </summary>
    internal class RegSenderAcqInterpolateEnable : IRegSender
    {
        //要发送的寄存器
        private const AcqBdReg.W REGADDR = AcqBdReg.W.Interpolate_EnableAcq;

        public UInt32 GetRegAddr()
        {
            return (UInt32)REGADDR;
        }

        public void Send()
        {
            UInt32 data;
            //根据当前条件决定
            if (ConditionManager.InterpEn)
                data = 1;
            else 
                data = 0;

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(REGADDR, data);
        }
    }
}
