using ScopeX.Core;
using ScopeX.SCPIManager;
using System.Text;
using ScopeX.ComModel;
using System.Linq;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 系统命令 ===============================================================================================
        public static bool scpiQuy_IDN(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string resultStr = "UNI-T,MSO7000CS,6455db7c7131,1.00";
            PrintDebug(ConvertInputData(analyResult), ConvertOutputData(resultStr));

            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(resultStr);
            return true;
        }
        /*  {协议版本号},{软件版本号},{硬件版本号},{仪器类型},{机型内部代号},{带宽},{采样率},
            {通道数量},{ 显示名称},{ 渠道信息},{ 序列号},{ 生产日期},{ 自定义信息}  */
        public static bool scpiQuy_SystemInfo(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string serialNumber = "6455db7c7131";
            if (Presenter.OptionsManager != null)
            {
                serialNumber = Presenter.OptionsManager.SerialNumber;
            }
            var is2GHz = Presenter.OptionsManager.Is2GHz;
            var bandwidth = is2GHz ? "2GHz" : "1GHz";
            var devicetype = "mso";
            var privatetype = "mso-7x";
            var samplingrate = "10GSa/s";
            var showname = Presenter.GetProductModel();     //Constants.ProductModel.Where(p => p.Is2GHz == is2GHz).FirstOrDefault().Model;
            var hardwareversion = string.IsNullOrEmpty(Presenter.HardWareVersion) ? "NULL" : Presenter.HardWareVersion;

            switch (Constants.PRODUCT)
            {
                case ProductType.JiHe_MSO7000X:
                    break;
                case ProductType.JiHe_MSO8000HD or ProductType.JiHe_MSO8000X:
                    devicetype = "mso";
                    privatetype = "mso-8x";
                    break;
                case ProductType.JiHe_UPO7000L or ProductType.JiHe_MSO7000A:
                    devicetype = "upo";
                    privatetype = "upo-7l";
                    break;
                default:
                    break;
            }

            string resultStr = $"20190929,{Presenter.SoftWareVersion},{hardwareversion},{devicetype},{privatetype},{bandwidth},{samplingrate},4,{showname},EN,{serialNumber},0/0/0,*";
            sendMessage.SendData = Encoding.UTF8.GetBytes(resultStr);
            return true;
        }
        //================= 系统命令 ===============================================================================================
        public static bool scpiQuy_Ext10MHzLockedState(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string resultStr = WidgetPrsnt.HardwareMiscFunc("Ext10MHzLocked", "");
            PrintDebug(ConvertInputData(analyResult), ConvertOutputData(resultStr));

            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(resultStr);
            return true;
        }


    }
}
//================= 共1个方法 =
