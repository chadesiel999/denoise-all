using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using Keysight.Calibration.U9193x;
using Keysight.USBDevice.WinUsbDevice;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal class BatchTaskPart_OpenCombGenerator : BatchTaskPartBase
    {
        public override String FuncionDescription
        {
            get => $"打开梳状波发生器";
        }
        public override String ParametersDescription
        {
            get
            {
                Int32 argindex = 1;
                return $"第{argindex++}个参数：通道组合构造成的交织方式，请使用Driver端代码中的AnalogAcquireModel.cs 之 AcqModeInterleaveDefines 之InterleaveName{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，需要校准的通道，Ch1、Ch2、Ch3、Ch4{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，阻抗，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，频点文件路径{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，保存该次扫频数据的文件夹{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，信号源VISA地址{System.Environment.NewLine}";
            }
        }
        public override String Example
        {
            get => "BatchTaskPart_SendCoefficientParams C1C3_20G,Ch1,HIGH,50,USB0::0x1AB1::0x0641::DG4E183302205::INSTR";
        }
        //参数样式为：
        //第1个参数，CoefficientType ，系数类型，应该与CreateProduct中PFC定义的一致。如Coefficients1，Coefficients2，Coefficients3
        //第2个参数，SaveCoefficientPathAndFileName:保存系数文件路径及名称。可以是相对路径或绝对路径。用于保存系数，供示波器软件使用。名称应该与软件中使用的名称一致
        private String _MatLabDLLName = "";
        private String _CoefficientType = "";
        private ChannelId _CurrChannelID = ChannelId.C1;
        private String _SaveCoefficientPath = "";
        private String _SaveCoefficientPathAndFileName = "";
        private String _InterleaveName = "";
        private Int32 _CoeQuantiBit = 0;
        private IInstrumentSession? _SigInstrumentSession;
        private String _CoefficientKey = String.Empty;
        private Int32 _MagCompOrder = 0;
        private Boolean Open = true;

        private Boolean AnalyParameter(String parameter)
        {
            if (parameter == "")
                return false;
            String[] paramlist = parameter.Split(',');
            Open = paramlist[0].Trim() == "ON" ? true : false;
            return true;
        }
        public override Boolean SetParameter(XmlScpiCmd? xmlScpiCmd, String parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            String[]? myname_parameterpair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myname_parameterpair == null)
                return false;
            return AnalyParameter(myname_parameterpair[1]);
        }

        // Token: 0x04000001 RID: 1
        private WinUsbDevice m_winUsbDevice;

        // Token: 0x04000002 RID: 2
        private U9391FFactory m_U9391FFactory;

        // Token: 0x04000003 RID: 3
        private U9391F m_U9391F;

        public override BatchTaskPartResult Exec(Double overtimeBySec, out String outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            outMsg = String.Empty;
            BatchTaskPartResult batchtaskpartresult = BatchTaskPartResult.ErrorFatal;
            Boolean checkstatus = false;
            List<Double> alldata = new List<Double>();
            bool flag = this.m_winUsbDevice == null;
            if (flag)
            {
                this.m_winUsbDevice = new WinUsbDevice();
            }
            bool flag2 = this.m_U9391FFactory == null;
            if (flag2)
            {
                this.m_U9391FFactory = new U9391FFactory(this.m_winUsbDevice);
            }
            int numConnectedDevices = this.m_U9391FFactory.NumConnectedDevices;
            bool flag3 = numConnectedDevices == 0;
            while (flag3)
            {
                string info = "没发现U9391(梳状波)设备,请检查电源或数据线路连接状态！";
                DialogResult dialogresult = MessageBox.Show(info,
                                        "Confirmation",             // Title
                                        MessageBoxButtons.YesNo,    // Buttons to display
                                        MessageBoxIcon.Question     // Icon to display
                                        );
                if (dialogresult == DialogResult.Yes)
                {
                    batchtaskpartresult = BatchTaskPartResult.Succeed;
                    checkstatus = false;
                    this.m_U9391FFactory = new U9391FFactory(this.m_winUsbDevice);
                    numConnectedDevices = this.m_U9391FFactory.NumConnectedDevices;
                    flag3 = numConnectedDevices == 0;
                }
                else if (dialogresult == DialogResult.No)
                {
                    checkstatus = true;
                    outMsg = "未打开U9391(梳状波)设备！";
                    batchtaskpartresult = BatchTaskPartResult.ErrorFatal;
                }
            }

            bool flag4 = this.m_U9391F == null;
            if (flag4)
            {
                this.m_U9391F = this.m_U9391FFactory.CreateDriver(0);
                outMsg = "已打开U9391(梳状波)设备！";
                batchtaskpartresult = BatchTaskPartResult.Succeed;
            }
            U9391F.StatusCode result = this.m_U9391F.startPegasus("pegasus12345");
            this.m_U9391F?.Dispose();
            this.m_U9391F = null;
            this.m_U9391FFactory = null;
            this.m_winUsbDevice = null;
            return batchtaskpartresult;
        }
    }
}
