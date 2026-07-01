using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 采集板Dsp使能寄存器（AcqBdReg.W.TIADC_Enable）的寄存器发送器
    /// </summary>
    internal class RegSenderAcqDspEnable : IRegSender
    {
        //要发送的寄存器
        private const AcqBdReg.W REGADDR = AcqBdReg.W.TIADC_Enable;

        public UInt32 GetRegAddr()
        {
            return (UInt32)REGADDR;
        }

        public void Send()
        {
            UInt32 data;
            //根据当前条件决定
            if (!ConditionManager.IsExtractEn)
            {
                if (ConditionManager.ToolDspEn)
                    data = 1U;
                else
                    data = 0;
            }
            else
            {
                data = 0;//抽取档关闭
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(REGADDR, data);
        }
    }

    internal class RegSenderProDspEnable : IRegSender
    {
        //要发送的寄存器
        private const ProcBdReg.W REGADDR = ProcBdReg.W.TIADC_Enable;

        public UInt32 GetRegAddr()
        {
            return (UInt32)REGADDR;
        }

        public void Send()
        {
            UInt32 data;
            //根据当前条件决定
            if (!ConditionManager.IsExtractEn)
            {
                if (ConditionManager.ToolDspProEn)
                    data = 1U;
                else
                    data = 0;
            }
            else
            {
                data = 0;//抽取档关闭
            }
            //Hd.CurrProduct?.ProcBd?.WriteToAllFpga(REGADDR, data);
            HdIO.WriteReg(REGADDR, data);
        }
    }
}
