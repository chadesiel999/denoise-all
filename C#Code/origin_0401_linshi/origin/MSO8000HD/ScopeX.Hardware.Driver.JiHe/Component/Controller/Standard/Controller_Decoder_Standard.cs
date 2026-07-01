using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal class Controller_Decoder_Standard : AbstractController_Decoder
    {
        public Controller_Decoder_Standard()
        {
            protocolConfigActionList.Clear();
            MethodInfo[] methodInfos = typeof(CtrlDecoder).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Where(o => o.Name.StartsWith($"Config_Standard_")).ToArray<MethodInfo>();
            foreach (SerialProtocolType serialProtocolType in Enum.GetValues(typeof(SerialProtocolType)))
            {
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (methodInfo.Name == $"Config_Standard_{serialProtocolType.ToString()}")
                    {
                        if (!protocolConfigActionList.ContainsKey(serialProtocolType))
                        {
                            protocolConfigActionList.Add(serialProtocolType, methodInfo);
                            break;
                        }
                    }
                }
            }
            //思路：缺省使用通用的方法
            //protocolConfigActionList.Add(SerialProtocolType.RS232, CtrlDecoder.Config_RS232_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.NRZ, CtrlDecoder.Config_NRZ_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.I2C, CtrlDecoder.Config_I2C_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.SPI, CtrlDecoder.Config_SPI_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.USB, CtrlDecoder.Config_USB_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.CAN, CtrlDecoder.Config_CAN_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.CAN_FD, CtrlDecoder.Config_CAN_FD_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.LIN, CtrlDecoder.Config_LIN_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.FlexRay, CtrlDecoder.Config_Flexray_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.AudioBus, CtrlDecoder.Config_I2S_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.MIL, CtrlDecoder.Config_MIL_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.SENT, CtrlDecoder.Config_SENT_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.SATA, CtrlDecoder.Config_Sata_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.JTAG, CtrlDecoder.Config_JTAG_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.PCIe, CtrlDecoder.Config_PCIe_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.SPMI, CtrlDecoder.Config_SPMI_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.ARINC429, CtrlDecoder.Config_ARINC429_Standard);
            //protocolConfigActionList.Add(SerialProtocolType.Ethernet, CtrlDecoder.Config_Ether_Standard);
        }
    }
}
