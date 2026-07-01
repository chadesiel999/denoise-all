using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 寄存器发送管理器
    /// </summary>
    internal class RegSendManager
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static RegSendManager Default { get;} = new RegSendManager();

        private List<IRegSender> _RegSenders = new List<IRegSender>();  //寄存器发送器集合

        private RegSendManager()
        {
            RegisterRegSender();
        }

        /// <summary>
        /// 注册寄存器发送器
        /// </summary>
        private void RegisterRegSender()
        {
            _RegSenders.Add(new RegSenderAcqDigitalTrigger());
            _RegSenders.Add(new RegSenderProcDigitalTrigger());
            _RegSenders.Add(new RegSenderAcqDspEnable());
            _RegSenders.Add(new RegSenderProDspEnable());
            _RegSenders.Add(new RegSenderProcDspEnable());
            _RegSenders.Add(new RegSenderAcqInterpolateEnable());
            _RegSenders.Add(new RegSenderProcInterpolateEnable());
        }

        /// <summary>
        /// 往指定寄存器发送控制值，控制值是发送器根据条件自行决定的
        /// </summary>
        /// <param name="regAddr"></param>
        public void Send(UInt32 regAddr)
        {
            var sender = _RegSenders.FirstOrDefault(rs => rs.GetRegAddr() == regAddr);
            if(sender == null)
            {
                Hd.SysLogger!.Invoke("RegSendManager.Send(): sender not exist", "Error");
                return;
            }
            sender!.Send();
        }
    }
}
