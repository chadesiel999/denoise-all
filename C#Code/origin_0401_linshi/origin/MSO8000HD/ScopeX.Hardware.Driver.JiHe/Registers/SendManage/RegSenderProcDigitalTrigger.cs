using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 处理板触发使能寄存器（ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro）的寄存器发送器
    /// </summary>
    internal class RegSenderProcDigitalTrigger : IRegSender
    {
        //要发送的寄存器
        private const ProcBdReg.W REGADDR = ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro;

        public UInt32 GetRegAddr()
        {
            return (UInt32)REGADDR;
        }

        public void Send()
        {
            UInt32 data;
            //根据当前条件决定
            if(ConditionManager.IsFromDDR)
            {
                if(ConditionManager.ToolProcDigitalTrigEn)
                {
                    data = ConditionManager.TriggerCtrlEn ? 1U : 0U;
                }
                else
                {
                    data = 0;
                }
            }
            else
            {
                data = 0;//FIFO数据关闭
            }
            HdIO.WriteReg(REGADDR, data);
        }
    }
}
