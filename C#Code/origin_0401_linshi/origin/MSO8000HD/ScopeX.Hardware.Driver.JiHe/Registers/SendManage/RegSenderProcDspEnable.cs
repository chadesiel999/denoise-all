using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 处理板Dsp使能寄存器（ProcBdReg.W.Dsp_DspEnPro）的寄存器发送器
    /// </summary>
    internal class RegSenderProcDspEnable : IRegSender
    {
        //要发送的寄存器
        private const ProcBdReg.W REGADDR = ProcBdReg.W.Dsp_DspEnPro;

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
                {
                    if (ChannelIdExt.GetAnalogs().Contains(ConditionManager.TriggerSrc))
                        data = 15U;
                    else
                        data = 15;
                }
                else
                {
                    data = 0;
                }
            }
            else
            {
                data = 0;//抽取档关闭
            }
            HdIO.WriteReg(REGADDR, data);
        }
    }
}
