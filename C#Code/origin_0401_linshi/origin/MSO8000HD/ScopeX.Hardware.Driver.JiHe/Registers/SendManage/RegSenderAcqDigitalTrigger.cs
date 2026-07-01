using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 采集板触发使能寄存器（AcqBdReg.W.TrigCtrl_DigitalTrigEn）的寄存器发送器
    /// </summary>
    internal class RegSenderAcqDigitalTrigger : IRegSender
    {
        //要发送的寄存器
        private const AcqBdReg.W REGADDR = AcqBdReg.W.TrigCtrl_DigitalTrigEn;

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
                data = ConditionManager.ToolAcqDigitalTrigEn ? 1U : 0U;
            }
            else
            {
                data = 0;//FIFO数据关闭
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(REGADDR, data);
        }
    }
}
