using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 寄存器发送器
    /// </summary>
    internal interface IRegSender
    {
        /// <summary>
        /// 获取寄存器地址
        /// </summary>
        /// <returns></returns>
        UInt32 GetRegAddr();

        /// <summary>
        /// 发送值到寄存器
        /// </summary>
        void Send();
    }
}
