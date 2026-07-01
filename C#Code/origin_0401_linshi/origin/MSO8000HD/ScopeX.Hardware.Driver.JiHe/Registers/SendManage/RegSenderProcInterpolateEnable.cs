using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 处理板插值使能寄存器（ProcBdReg.W.Interpolate_InterpEnPro）的寄存器发送器
    /// </summary>
    internal class RegSenderProcInterpolateEnable : IRegSender
    {
        //要发送的寄存器
        private const ProcBdReg.W REGADDR = ProcBdReg.W.Interpolate_InterpEnPro;

        public UInt32 GetRegAddr()
        {
            return (UInt32)REGADDR;
        }

        public void Send()
        {
            UInt32 data;
            //根据当前条件决定
            if (ConditionManager.InterpEn)
            {
                if (ChannelIdExt.GetAnalogs().Contains(ConditionManager.TriggerSrc))
                    data = 1U;
                else
                    data = 1U;
            }
            else
                data = 0;

            HdIO.WriteReg(REGADDR, data);
        }
    }
}
