using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using ScopeX.ComModel;
using System.Reflection;
using System.ComponentModel;
namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= IEEE 488.2公用命令 =====================================================================================
        /// <summary>
        /// 将所有寄存器组中的事件寄存器清零，同时清除错误队列
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_CLS(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 设置或查询标准事件状态寄存器组的使能寄存器位
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_ESE(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 设置或查询标准事件状态寄存器组的使能寄存器位
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_ESE(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 查询并清除标准事件状态寄存器组的事件寄存器值
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_ESR(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 查询当前操作是否完成设置时，在当前操作完成后，将标准事件状态寄存器的“Operation Complete”位（位0）置1
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_OPC(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 获取仪器信息
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_IDN(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string serialNumber = "6455db7c7131";
            if (Presenter.OptionsManager != null)
            {
                serialNumber = Presenter.OptionsManager.SerialNumber;
            }

            var prodcut = Constants.PRODUCT switch
            {
                ProductType.JiHe_MSO7000X => "MSO7000X",
                ProductType.JiHe_UPO7000L or ProductType.JiHe_MSO7000A => "UPO7000L",
                ProductType.JiHe_MSO8000HD or ProductType.JiHe_MSO8000X => "MSO8000HD",
                _ => "MSO7000"
            };

            sendMessage.SendData = decodeStr($"UNI-T,{prodcut},{serialNumber},{Presenter.SoftWareVersion}");
            return true;
        }
        /// <summary>
        /// 将仪器复恢复到出厂默认值
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_RST(SCPICommandProcessFuncParam analyResult)
        {
            Presenter.Default(false);
            return true;
        }
        /// <summary>
        /// 为状态字节寄存器组设置使能寄存器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_SRE(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 为状态字节寄存器组设置使能寄存器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_SRE(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 保存当前仪器状态到所选寄存器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_SAV(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 从指定单元中恢复*SAV 命令保存的设定值
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_RCL(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 为状态字节寄存器组查询条件寄存器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_STB(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 执行一次自检并返回自检结果
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_IEEE4882_TST(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //throw new NotImplementedException();
            return true;
        }
        /// <summary>
        /// 等待操作完成
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_IEEE4882_WAI(SCPICommandProcessFuncParam analyResult)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
//================= 共14个方法 =
