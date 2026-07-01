using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace ScopeX.Scpi
{
    public class ScpiManager
    {

        /// <summary>
        /// 其中服务。在主进程开始后其中。
        /// </summary>
        /// <returns>是否成功。当CmdTable.AllElements 中存在重复键值等问题时可能不正确。</returns>
        public static bool Start(DsoPrsnt _dsoPrsnt, bool autoStartLXI = true, bool autoStartWeb = true, bool autoStartUsb = false, string comPort_Name = "COM3", Int32 comPort_Baudrate = 9600, Parity comPort_Parity = Parity.None, Int32 comPort_Databits = 8, StopBits comPort_Stopbits = StopBits.One, char comPort_TerminationChar = '\n')
        {
           SCPIManager.SCPIManager.GetWaveDataFile += StubFunc.SaveWaveDataFile;

            StubFunc.Presenter = _dsoPrsnt;
            CmdTable.Presenter = _dsoPrsnt;
            CmdTable.ChnlPrsnt = (AnalogPrsnt)_dsoPrsnt.GetAllChnls().ToList().FirstOrDefault(ch => ch.Id == ComModel.ChannelId.C1);
            //CmdTable.ChnlPrsnt = (AnalogPrsnt)_dsoPrsnt.GetAllChnls().ToList()[0];
            CmdTable.InitDictionary();
            StubFunc.LoadManufacturerInfo();
            //return SCPIManager.Default.Start(CommunicationType.USBTMC | CommunicationType.LXI | CommunicationType.WEBSOCKET, CmdTable.AllElements);
            //var communicationType = CommunicationType.LXI;
            var communicationType = CommunicationType.None;
            if (autoStartLXI)
            {
                communicationType |= CommunicationType.LXI;
            }
            if (autoStartWeb)
            {
                communicationType |= CommunicationType.WEBSOCKET;
            }
            if (autoStartUsb)
            {
                communicationType |= CommunicationType.USBTMC;
            }
            communicationType |= CommunicationType.COM;

            (Func<Action<byte, int>>?, Func<Action<int, int>>?, Func<Action<uint, int>>?, Func<Func<string?>>?) actions = (WebSocketInterface.GetAction_KeyboradEventHandler, WebSocketInterface.GetAction_SetMousePosHandler, WebSocketInterface.GetAction_MouseEventHandler, WebSocketInterface.GetFunc_GetImageBase64String);
            return SCPIManager.SCPIManager.Default.Start(communicationType, CmdTable.AllElements, actions, comPort_Name, comPort_Baudrate, comPort_Parity, comPort_Databits, comPort_Stopbits, comPort_TerminationChar, DsoPrsnt.GetKeyBoardLocked);
        }
        /// <summary>
        /// 重置复位。目前没有太大用处。
        /// </summary>
        public static void Reset()
        {
            SCPIManager.SCPIManager.Default.Reset();
        }
        /// <summary>
        /// 关闭SCPI服务。在启动该服务的进程退出时，必须调用。
        /// </summary>
        public static void Close()
        {
            SCPIManager.SCPIManager.Default.Stop();
        }
        /// <summary>
        /// 界面配置 启动WEB模块
        /// </summary>
        /// <returns></returns>
        public static bool StartWebModel()
        {
            if (SCPIManager.SCPIManager.IsWebRunning)
            {
                return true;
            }
            SCPIManager.SCPIManager.Default.StartWebModel();
            return SCPIManager.SCPIManager.IsWebRunning;
        }
        /// <summary>
        /// 界面配置 检查WEB模块是否启动
        /// </summary>
        /// <returns></returns>
        public static bool IsWebModelRunning()
        {
            return SCPIManager.SCPIManager.IsWebRunning;
        }
        /// <summary>
        /// 界面配置 停止WEB模块
        /// </summary>
        public static void StopWebModel()
        {
            SCPIManager.SCPIManager.Default.Stop(true);
            Thread.Sleep(1000); //web关闭需要时间
        }

        public static void SetAdapter(ScpiAdapter.ScpiAdapter adapter)
        {
            SCPIManager.SCPIManager.Default.SetAdapter(adapter);
        }
    }
}
