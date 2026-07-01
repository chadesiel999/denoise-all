using ScopeX.ComModel;
using ScopeX.Updater.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using static ScopeX.Hardware.Driver.CtrlAnalogChannel_JiHe2d5G;

namespace ScopeX.Hardware.Driver
{
    public class McuComPortUpdater
    {
        private static Action<string>? AddLog;
        private static void WriteLog(string message)
        {
            AddLog?.Invoke(message);
        }
        public static void BindLogFunc(Action<string>? logFunc)
        {
            AddLog = logFunc;
        }

        //分段发送23.4.28 ljw
        private const int SPAN_SIZE = 2 * 1024;
        protected static ProductType productType = ProductType.Base;
        public enum Updater_ReqScopeXommands : byte
        {
            CMD0x01_Request_RunningAtWhere = 0x01,
            CMD0x02_Request_ReadMcuVersion = 0x02,
            CMD0x03_Request_ReadMcuBootVersion = 0x03,
            CMD0x04_Request_ReadUpdateTimeStamp = 0x04,

            CMD0x05_Request_UpdateStart = 0x05,
            CMD0x06_Request_RegisterAppStartTime = 0x06,
            CMD0x07_Request_ReadbackAppStartTime = 0x07,

            CMD0x08_Request_UpdateSend = 0x08,
            CMD0x0B_Request_UpdateVerifyStage1 = 0x0B,
            CMD0x0C_Request_UpdateSwitch = 0x0C,
            CMD0x0D_Request_UpdateVerifyStage2 = 0x0D,
            CMD0x11_Request_UpdateFinished = 0x11,
            CMD0x15_Request_WriteUpdateTimeStamp = 0x15,
            CMD0xC1_Request_CommunicateReset = 0xc1,
            /// <summary>
            /// 高压警告。主要包括：4个模拟通道和外部触发
            /// </summary>
            CMD0x60_HighVoltageWarning = 0x60,

            //////////// 探头更新  //////////// 
            CMD0x71_Request_Diff_UpdateSend_Stage1 = 0x71,
            CMD0x72_Request_Diff_UpdateVerify_Stage1 = 0x72,
            CMD0x73_Request_Diff_UpdateStart = 0x73,
            CMD0x74_Request_Diff_QueryRunAt = 0x74,
            CMD0x75_Request_Diff_UpdateSend_Stage2 = 0x75,
            CMD0x76_Request_Diff_UpdateVerify_Stage2 = 0x76,
            CMD0x77_Request_Diff_QueryVerify2 = 0x77,
            CMD0x78_Request_Diff_UpdateSwitch = 0x78,
            CMD0x79_Request_Diff_QueryVerify3 = 0x79,
            CMD0x7A_Request_Diff_UpdateFinished = 0x7A,

            CMD0x7B_Request_ProbeUpdateSupport_Chlboard = 0x7B,
            CMD0x7C_Request_ProbeUpdateSupport_Backend = 0x7C,
            CMD0x7D_Request_ReadProbeMCUVersion = 0x7D,
            CMD0x7E_Request_ReadProbeUpdateTimeStamp = 0x7E,
            CMD0x7F_Request_WriteProbeUpdateTimeStamp = 0x7F,

        }
        /*
        public enum Updater_ReadbackCommands
        {
            CMD0x01_Readback_RunningAtWhere = 0x01,
            CMD0x02_Readback_ReadMcuVersion = 0x02,
            CMD0xC0_Readback_CMDOK = 0xc0,
            CMD0xC2_Readback_UpdateWriteFlash = 0xc2,
            CMD0xFF_Readback_ERROR = 0xff,
        }
        */
        enum ReadbackErrorCode
        {
            Error0x01_CannotRunAtApp = 0x01,
            Error0x02_CannotRunAtBoot = 0x02,
            Error0x03_AlreadyAtBoot = 0x03,
            Error0x04_AlreadyAtApp = 0x04,

            Error0x05_CMD_ParaLentghError = 0x05,
            Error0x06_CMD_UpdateNotStart = 0x06,

            Error0x90_CMD_ThisVersionNotSupport = 0x90,
            Error0xFF_General = 0xff,
        };

        public event Action<int> HighVoltageWarningEvent;

        private object _highVoltageWarningLock = new object();

        private int _lastHighVoltageWarningvalue = 0;


        /// <summary>
        /// 高压警告。主要包括：4个模拟通道和外部触发
        /// </summary>
        internal int HighVoltageWarning { get; private set; }


        /// <summary>
        /// 重置高压警告
        /// </summary>
        internal void ResetHighVoltageWarning(IEnumerable<ChannelId> channelIds)
        {
            lock (_highVoltageWarningLock)
            {
                if (channelIds == null)
                {
                    HighVoltageWarning = 0;
                }
                else
                {
                    var annalogChannel = HighVoltageWarning >> 24;
                    var extTrigger = (HighVoltageWarning >> 16) & 0xFF;
                    foreach (var channelId in channelIds)
                    {
                        switch (channelId)
                        {
                            case ChannelId.C1:
                                annalogChannel &= ~(1 << 0);
                                break;
                            case ChannelId.C2:
                                annalogChannel &= ~(1 << 1);
                                break;
                            case ChannelId.C3:
                                annalogChannel &= ~(1 << 2);
                                break;
                            case ChannelId.C4:
                                annalogChannel &= ~(1 << 3);
                                break;
                            case ChannelId.Ext:
                            case ChannelId.Ext5:
                                extTrigger = 0;
                                break;
                        }
                    }

                    int mask_annalog = ~(0xFF << 24);
                    int mask_ext = ~(0xFF << 16);
                    HighVoltageWarning = (HighVoltageWarning & mask_annalog) | (annalogChannel << 24);
                    HighVoltageWarning = (HighVoltageWarning & mask_ext) | (extTrigger << 16);
                }

                _lastHighVoltageWarningvalue = HighVoltageWarning;
            }
        }

        public const byte RUNNING_AT_APP = 0x77; // App
        public const byte RUNNING_AT_BOOT = 0x11; // Boot        private static object _lock = new object();
        public const int versionStructLastModifiedDatetime_TotalBytes = 14;
        public const int versionStructLastModifierName_TotalBytes = 16;
        public const int versionStructModelName_TotalBytes = 32;
        public const int versionStructComment_TotalBytes = 64;

        public const int McuVersionInfoBytes = 129;
        const int PackageHeaderTailMinLength = 10;
        private object _lock = new object();
        private SerialPort? serialPort = null;

        private byte[] ReceivedBuffer = new byte[4096];
        private int ReceivedLength = 0;
        private byte[] packageHeaderBytes = { 0x55, 0xaa, 0xbb };
        private int packageHeader_TotalBytes;

        private byte[] packageEndBytes = { 0x0d, 0x0a };
        private int packageTail_TotalBytes;
        private ConcurrentDictionary<byte, KeyValuePair<object, ConcurrentQueue<List<byte>>>> ReceiveQueues = new ConcurrentDictionary<byte, KeyValuePair<object, ConcurrentQueue<List<byte>>>>();

        public McuComPortUpdater()
        {
            packageHeader_TotalBytes = packageHeaderBytes.Count();
            packageTail_TotalBytes = packageEndBytes.Count();
        }

        bool isPowerOn;
        public bool Connected => serialPort != null && serialPort.IsOpen;
        public Action? AfterPowerOnAction;
        // public static bool McuPowerOn()
        // {
        //     return false;
        // }

        public Boolean OpenChannelPower()
        {
            isPowerOn = McuPowerOn();
            return isPowerOn;
        }

        public Boolean OpenMcuSerialPort(string name, int baudrate, Parity parity, int databits, StopBits stopbits, char terminationChar = '\n')
        {
            if (Connected)
            {
                Hd.SysLogger?.Invoke("通道板串口已被打开!", "Warning");
                return false;
            }
            if (serialPort != null)
                return true;
            serialPort = new SerialPort
            {
                PortName = name,
                BaudRate = baudrate,
                Parity = parity,
                ReadTimeout = 100,
                DataBits = databits,
                StopBits = stopbits,
                ReceivedBytesThreshold = 1,
                WriteTimeout = 1000,
                WriteBufferSize = 32 * 1024 + 16,
                ReadBufferSize = 16 * 2 * 1024 + 16,
            };
            try
            {
                serialPort?.Open();
                serialPort!.DataReceived += new SerialDataReceivedEventHandler(Receive);
                serialPort!.ErrorReceived += McuComPortUpdater_ErrorReceived;
                for (int i = 0; i < packageHeader_TotalBytes; i++)
                    localSendBuffer[i] = packageHeaderBytes[i];

                SendData(true, (byte)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);

                return true;
            }
            catch
            {
                serialPort = null;
                return false;
            }
        }
        public static void UpdateForceSetProductType(ProductType type)
        {
            productType = type;
        }
        public static void SetOuterLedByUpdate(ErrorType type)
        {
            
        }
        public bool Open(string name = "COM1", int baudrate = 115200, Parity parity = Parity.None, int databits = 8, StopBits stopbits = StopBits.One, char terminationChar = '\n')
        {
            if (!CtrlAnalogChannel_JiHe2d5G.McuPowerOn())
            {
                return false;
            }
            if (serialPort != null)
                return true;
            serialPort = new SerialPort
            {
                PortName = name,
                BaudRate = baudrate,
                Parity = parity,
                ReadTimeout = 100,
                DataBits = databits,
                StopBits = stopbits,
                ReceivedBytesThreshold = 1,
                WriteTimeout = 1000,
                WriteBufferSize = 32 * 1024 + 16,
                ReadBufferSize = 16 * 2 * 1024 + 16,
            };
            try
            {
                serialPort?.Open();
                serialPort!.DataReceived += new SerialDataReceivedEventHandler(Receive);
                serialPort!.ErrorReceived += McuComPortUpdater_ErrorReceived;
                for (int i = 0; i < packageHeader_TotalBytes; i++)
                    localSendBuffer[i] = packageHeaderBytes[i];

                SendData(true, (byte)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);

                return true;
            }
            catch
            {
                serialPort = null;
                return false;
            }
        }
        private void McuComPortUpdater_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.WriteLine(e.EventType.ToString());
            ; // throw new NotImplementedException();
        }
        public bool Opened
        {
            get => serialPort != null;
        }
        public void Close()
        {
            serialPort?.Close();
            serialPort = null;
        }
        public static Dictionary<byte, bool> CommandCommunicationSendFormats = new Dictionary<byte, bool>()
        {
            [(byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x03_Request_ReadMcuBootVersion] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x04_Request_ReadUpdateTimeStamp] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x05_Request_UpdateStart] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x06_Request_RegisterAppStartTime] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x07_Request_ReadbackAppStartTime] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x08_Request_UpdateSend] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x0B_Request_UpdateVerifyStage1] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x0C_Request_UpdateSwitch] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x0D_Request_UpdateVerifyStage2] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x11_Request_UpdateFinished] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x15_Request_WriteUpdateTimeStamp] = true,


            [(byte)AnalogChannelReqScopeXommands.CMD0x20_Request_PowerCtrl] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x21_Request_RefVlotage] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x23_Request_ExternalChannelSet] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x32_Request_CtrlProbeDifferenceOrSingle] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x33_Request_ReadProbeFactoryInfo] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x34_Request_CtrlProbeLed] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x41_Request_AnalogChannelOffset] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x42_Request_AnalogChannelGain] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x43_Request_CtrlChannel4094] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x48_Request_CtrlOuterLed] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x50_Request_WriteProbeFactoryInfo] = true,

            [(byte)AnalogChannelReqScopeXommands.CMD0x90_Request_ReadProbeCaliInfo] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x91_Request_ReadProbeCaliReadDnREG] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x92_Request_ReadProbeCaliSendDnREG] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x93_Request_ReadProbeCaliReadDnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x94_Request_ReadProbeCaliSendDnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x95_Request_ReadProbeCaliReadFnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x96_Request_ReadProbeCaliSendFnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x97_Request_ReadProbeCaliReadDate] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x98_Request_ReadProbeCaliSendDate] = false,

            [(byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication] = false,
        };
        public static Dictionary<byte, bool> CommandCommunicationRecvFormats = new Dictionary<byte, bool>()
        {
            [(byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x03_Request_ReadMcuBootVersion] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x04_Request_ReadUpdateTimeStamp] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x05_Request_UpdateStart] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x06_Request_RegisterAppStartTime] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x07_Request_ReadbackAppStartTime] = true,

            [(byte)Updater_ReqScopeXommands.CMD0x08_Request_UpdateSend] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x0B_Request_UpdateVerifyStage1] = false,
            [(byte)Updater_ReqScopeXommands.CMD0x0C_Request_UpdateSwitch] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x0D_Request_UpdateVerifyStage2] = true,
            [(byte)Updater_ReqScopeXommands.CMD0x11_Request_UpdateFinished] = false,

            [(byte)Updater_ReqScopeXommands.CMD0x15_Request_WriteUpdateTimeStamp] = false,


            [(byte)AnalogChannelReqScopeXommands.CMD0x20_Request_PowerCtrl] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x21_Request_RefVlotage] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x23_Request_ExternalChannelSet] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x30_Request_ReadTemperature] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x32_Request_CtrlProbeDifferenceOrSingle] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x33_Request_ReadProbeFactoryInfo] = true,
            [(byte)AnalogChannelReqScopeXommands.CMD0x34_Request_CtrlProbeLed] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x41_Request_AnalogChannelOffset] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x42_Request_AnalogChannelGain] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x43_Request_CtrlChannel4094] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x48_Request_CtrlOuterLed] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x50_Request_WriteProbeFactoryInfo] = false,


            [(byte)AnalogChannelReqScopeXommands.CMD0x90_Request_ReadProbeCaliInfo] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x91_Request_ReadProbeCaliReadDnREG] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x92_Request_ReadProbeCaliSendDnREG] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x93_Request_ReadProbeCaliReadDnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x94_Request_ReadProbeCaliSendDnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x95_Request_ReadProbeCaliReadFnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x96_Request_ReadProbeCaliSendFnROM] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x97_Request_ReadProbeCaliReadDate] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0x98_Request_ReadProbeCaliSendDate] = false,

            [(byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData] = false,
            [(byte)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication] = false,
        };
        public void ClearSpecialReceiveQueue(byte cmd)
        {
            if (ReceiveQueues.ContainsKey(cmd))
            {
                lock (ReceiveQueues[cmd].Key)
                {
                    ReceiveQueues[cmd].Value.Clear();
                }
            }
        }

        private void Receive(object sender, EventArgs e)
        {
            lock (_lock)
            {
                SerialPort serialPort = (SerialPort)sender;
                //判断串口是否打开
                if (!serialPort.IsOpen)
                    return;
                var bytesCount = serialPort.BytesToRead;
                if (bytesCount == 0)
                {
                    return;
                }
                byte[] newContent = new byte[bytesCount];
                Int32 recvedBytes = 0;
                try
                {
                    recvedBytes = serialPort.Read(newContent, 0, bytesCount);
                    if (recvedBytes == 0)
                        return;
                    Array.Copy(newContent, 0, ReceivedBuffer, ReceivedLength, recvedBytes);

                    String recvstr = Encoding.UTF8.GetString(ReceivedBuffer.ToArray());
                    Trace.WriteLine($"{serialPort.PortName} recv ({recvedBytes}) : {recvstr}");
                }
                catch
                {
                    Hd.SysLogger?.Invoke($"at McuComPortUpdater->Receive", "Info");
                    ReceivedLength = 0;
                    return;
                }
                ReceivedLength += recvedBytes;
                if (!(ReceivedLength >= 8 && ReceivedBuffer[ReceivedLength - 2] == 0x0d && ReceivedBuffer[ReceivedLength - 1] == 0x0a))
                    return;
                //var oldCount = bytesCount;
                //var readTimesCount = 0;
                //do
                //{
                //    Thread.Sleep(10);
                //    oldCount = bytesCount;
                //    bytesCount = serialPort.BytesToRead;
                //    readTimesCount++;

                //} while (bytesCount != oldCount);

                //for (int bytesToRead = serialPort.BytesToRead; bytesToRead > 0; bytesToRead = serialPort.BytesToRead)
                //{
                //    serialPort.Read(ReceivedBuffer, 0, bytesToRead);
                //    ReceivedLength = bytesToRead;
                //    Thread.Sleep(1);
                //}

                if (ReceivedLength < packageHeader_TotalBytes + packageTail_TotalBytes + 3)
                {
                    return;
                }

                // 是否继续解包
                int handedlength = 0;

                // 粘包处理，需要一个包一个包的解
                do
                {
                    bool checkHead = ReceivedBuffer.Take(packageHeader_TotalBytes).SequenceEqual(packageHeaderBytes);

                    // 当前包的数据长度
                    ushort currentpackdatalength = BitConverter.ToUInt16(ReceivedBuffer.Skip(packageHeader_TotalBytes + 1).Take(2).ToArray());

                    // 当前包的总长度，包含所有数据的总长度。
                    int fullpackagetotallength = packageHeader_TotalBytes + packageTail_TotalBytes + 3 + currentpackdatalength;
                    handedlength += fullpackagetotallength;

                    bool checkTail = ReceivedBuffer.Skip(fullpackagetotallength - packageTail_TotalBytes).Take(packageTail_TotalBytes).SequenceEqual(packageEndBytes);

                    /*for (int i = 0; i < packageHeader_TotalBytes; i++)
                    {
                        if (packageHeaderBytes[i] != ReceivedBuffer[i])
                        {
                            checkHead = false;
                            break;
                        }
                    }
                    for (int i = 0; i < packageTail_TotalBytes; i++)
                    {
                        if (packageEndBytes[i] != ReceivedBuffer[fullpackagetotallength - packageTail_TotalBytes + i])
                        {
                            checkTail = false;
                            break;
                        }
                    }*/

                    if (checkHead && checkTail)
                    {
                        List<byte> newItem = new List<byte>();
                        for (int i = packageHeader_TotalBytes; i < fullpackagetotallength - 1; i++)
                            newItem.Add(ReceivedBuffer[i]);
                        if (newItem[0] == 0x33 || /*探头信息*/
                            newItem[0] == 0x31 || /*探头状态*/
                            newItem[0] == 0x90 || /*校正信息*/
                            newItem[0] == 0x7D || /*版本信息*/
                            newItem[0] == 0x9E || /*版本信息*/
                            false
                            )
                        {
                            ProbeManager.Default.ReceiveQueue.Enqueue(newItem);
                        }
                        else if (newItem[0] == (byte)Updater_ReqScopeXommands.CMD0x07_Request_ReadbackAppStartTime)
                        {
                            AfterPowerOnAction?.Invoke();
                        }
                        else if (newItem[0] == (byte)Updater_ReqScopeXommands.CMD0x60_HighVoltageWarning)
                        {
                            // 通道xx 输入了高压，可能损害器件，软键已自动切换为高阻，请断开输入
                            int dataLength = newItem[1];
                            dataLength |= newItem[2] << 8;
                            if (dataLength >= 4 && newItem.Count >= dataLength + 3)
                            {
                                var intbytes = newItem.Skip(3).Take(dataLength);
                                var warningbytes = intbytes.Take(4).ToArray();
                                var tempbytes = warningbytes.Reverse().ToArray();
                                lock (_highVoltageWarningLock)
                                {
                                    HighVoltageWarning = BitConverter.ToInt32(tempbytes, 0);
                                    if (_lastHighVoltageWarningvalue != HighVoltageWarning)
                                    {
                                        _lastHighVoltageWarningvalue = HighVoltageWarning;
                                        HighVoltageWarningEvent?.Invoke(HighVoltageWarning);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int dataLength = newItem[1];
                            dataLength |= newItem[2] << 8;
                            if (newItem.Count >= dataLength + 3)
                            {
                                if (!ReceiveQueues.ContainsKey(newItem[0]))
                                {
                                    ConcurrentQueue<List<byte>> q = new();
                                    ReceiveQueues.TryAdd(newItem[0], new KeyValuePair<object, ConcurrentQueue<List<byte>>>(new object(), q));
                                }
                                lock (ReceiveQueues[newItem[0]].Key)
                                {
                                    ReceiveQueues[newItem[0]].Value.Enqueue(newItem);
                                }
                            }
                            else
                                ;
                        }


                        //ReceivedLength = 0;
                        ReceivedBuffer = ReceivedBuffer.Skip(fullpackagetotallength).Take(ReceivedBuffer.Length - fullpackagetotallength).ToArray();
                    }
                    else
                    {
                        ReceivedBuffer = ReceivedBuffer.Skip(fullpackagetotallength).Take(ReceivedBuffer.Length - fullpackagetotallength).ToArray();
                    }
                }
                while (handedlength < ReceivedLength);

                ReceivedLength = 0;
                ReceivedBuffer = new byte[4096];
            }
        }

        private const int localSendBufferSize = 1024 * 16;
        private byte[] localSendBuffer = new byte[localSendBufferSize];

        public bool SendData(Boolean bDiscardInOutBuffer, byte cmd, List<byte>? dataList)
        {
            if (serialPort == null)
                return false;

            if (dataList != null && dataList.Count > 0)
            {
                String sendscmd = Encoding.UTF8.GetString(dataList.ToArray());
                Trace.WriteLine($"{serialPort.PortName} send({dataList.Count}) : {sendscmd}");
            }

            int len = (dataList == null ? 0 : dataList.Count);
            if (len + PackageHeaderTailMinLength > localSendBufferSize)
                return false;
            int validLength = len;
            bool bNeedInvert = CheckNeedInvert(CommandCommunicationSendFormats, cmd);
            if (bNeedInvert)
                validLength *= 2;
            lock (_lock)
            {
                if (bDiscardInOutBuffer)
                {
                    serialPort!.DiscardOutBuffer();
                    serialPort!.DiscardInBuffer();
                }

                //localSendBuffer[packageHeader_TotalBytes + 0] = (byte)cmd;
                //localSendBuffer[packageHeader_TotalBytes + 1] = (byte)((validLength & 0x00ff));
                //localSendBuffer[packageHeader_TotalBytes + 2] = (byte)((validLength & 0xff00) >> 8);
                //if (len != 0 && dataList != null)
                //    Array.Copy(dataList.ToArray(), 0, localSendBuffer, packageHeader_TotalBytes + 3, len);
                //if (bNeedInvert && dataList != null)
                //{

                //    for (int i = 0; i < len; i++)
                //    {
                //        localSendBuffer[packageHeader_TotalBytes + 3 + len + i] = (byte)(~dataList![i]);
                //    }
                //}
                //Array.Copy(packageHeaderBytes, 0, localSendBuffer, 0, packageHeader_TotalBytes);
                //Array.Copy(packageEndBytes, 0, localSendBuffer, packageHeader_TotalBytes + 3 + validLength, packageTail_TotalBytes);
                //Array.Copy(packageHeaderBytes, 0, localSendBuffer, 0, packageHeader_TotalBytes);
                //try
                //{
                //    serialPort.Write(localSendBuffer, 0, packageHeader_TotalBytes + validLength + packageTail_TotalBytes + 3);
                //}
                //catch
                //{

                //}
                //Thread.Sleep(1);
                /////HTF_1112
                if (len != 0 && dataList != null)
                    Array.Copy(dataList.ToArray(), 0, localSendBuffer, 0, len);
                try
                {
                    serialPort.Write(localSendBuffer, 0, len);
                }
                catch
                {

                }
                ////HTF_END
                return true;
            }
        }
        public bool ReadSpecailMessage(byte specailCmdID, int maxWaitMilliseconds, bool bForceClear, [NotNullWhen(true)] out (byte cmd, int dataLength, List<byte> Data)? result)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool bRecvInverted = CheckNeedInvert(CommandCommunicationRecvFormats, specailCmdID);

            //Debug.WriteLine("");

            while (stopwatch.ElapsedMilliseconds < maxWaitMilliseconds)
            {
                if (ReceiveQueues.ContainsKey(specailCmdID))
                {
                    var curr = ReceiveQueues[specailCmdID];
                    lock (curr.Key)
                    {
                        if (curr.Value.Count > 0)
                        {
                            if (curr.Value.TryDequeue(out var item))
                            {
                                byte cmd = item[0];
                                //Debug.WriteLine($"cmd:{cmd}  Time:{stopwatch.ElapsedMilliseconds} mm");
                                int dataLength = item[1];
                                dataLength |= item[2] << 8;
                                //Debug.WriteLine($"dataLength:{dataLength}  Time:{stopwatch.ElapsedMilliseconds} mm");
                                List<Byte> data = new List<byte>();

                                bool bOk = true;
                                if (dataLength > 0)
                                {
                                    int validLength = dataLength;
                                    if (bRecvInverted)
                                    {
                                        validLength = dataLength / 2;
                                        for (int i = 0; i < validLength; i++)
                                        {
                                            if (item[3 + i] != (byte)(~item[3 + i + validLength]))
                                            {
                                                bOk = false;
                                                continue;
                                            }
                                        }
                                    }
                                    if (bOk)
                                    {
                                        for (int i = 0; i < dataLength; i++)
                                            data.Add(item[3 + i]);
                                    }
                                }
                                if (bOk)
                                {
                                    result = (cmd, dataLength, data);
                                    if (bForceClear)
                                        curr.Value.Clear();
                                    stopwatch.Stop();
                                    //Debug.WriteLine($"L545: ReadSpecailMessage OK:dataLen:{dataLength} crc:0x{data[0]:X1} 0x{data[1]:X1}");
                                    return true;
                                }
                            }
                            else
                            {
                                ;
                            }
                        }
                        else;
                    }
                }
                else;
                Thread.Sleep(1);
            }

            stopwatch.Stop();
            result = null;
            return false;
        }

        public void McuUpdate_Start(byte where, byte atWhitchBoot)
        {
            var sendData = new List<byte>();
            sendData.Add(where);
            sendData.Add(atWhitchBoot);
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x05_Request_UpdateStart, sendData);
        }
        public bool CheckNeedInvert(Dictionary<byte, bool> define, byte currCMD)
        {
            return define.ContainsKey(currCMD) && define[currCMD];
            //return false;
        }
        public bool McuUpdate_SendData(List<byte> data)
        {
            int errorTimes = 0;
            int alwarySendCount = 0;
            int tmpSendTimes = 0;
            while (alwarySendCount < data.Count && errorTimes < 100)
            {
                Debug.WriteLine($"L588: SendTimes:{tmpSendTimes}");
                var sendData = new List<byte>();
                int nowCount = ((data.Count - alwarySendCount) > SPAN_SIZE) ? SPAN_SIZE : (data.Count - alwarySendCount);

                for (int i = 0; i < nowCount; i++)
                    sendData.Add(data[alwarySendCount + i]);

                #region WriteAtAddress

                sendData.Add((byte)((alwarySendCount & 0x00_00_00_ff) >> 0));
                sendData.Add((byte)((alwarySendCount & 0x00_00_ff_00) >> 8));
                sendData.Add((byte)((alwarySendCount & 0x00_ff_00_00) >> 16));
                sendData.Add((byte)((alwarySendCount & 0xff_00_00_00) >> 24));

                #endregion
                #region CRC Code

                UInt32 crcCode = CRC32.GetCRC32Code(sendData);
                sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
                sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
                sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
                sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));

                #endregion


                ClearSpecialReceiveQueue((byte)Updater_ReqScopeXommands.CMD0x08_Request_UpdateSend);
                SendData(true, (byte)Updater_ReqScopeXommands.CMD0x08_Request_UpdateSend, sendData);
                tmpSendTimes++;
                int maxWaitMilliseconds = (int)((sendData.Count + 8) * 1000.0 / (115200 / 10) * 2) * 10; //ljw 24.3.11 之前时间不够
                if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x08_Request_UpdateSend, maxWaitMilliseconds, true, out var result))
                {
                    if (result.Value.dataLength >= 1)
                    {
                        if (result.Value.Data[0] == 0x00)
                        {
                            //OK
                            alwarySendCount += nowCount;
                            continue;
                        }
                        else
                        {
                            Debug.WriteLine("L612: error:result.Value.Data[0] != 0x00");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("L626: error:result.Value.dataLength == 0x00");
                    }
                }
                else
                {
                    Debug.WriteLine("L631: error: ReadSpecailMessage failed");
                }
                errorTimes++;
                Debug.WriteLine($"L631: errorTimes:{errorTimes}");
            }
            return (errorTimes < 100);
        }

        public void McuUpdate_Finished()
        {
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x11_Request_UpdateFinished, null);
        }
        public bool McuUpdate_IsRunAtBoot(int maxWaitMicroseconds)
        {

            for (int i = 0; i < maxWaitMicroseconds / 50; i++)
            {
                SendData(true, (byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
                if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, 1000, true, out var readbackResult))
                {
                    if (readbackResult.Value.Data[0] == RUNNING_AT_BOOT)
                    {
                        return true;
                    }
                }
                else
                    Thread.Sleep(50);
            }
            return false;
        }
        public bool McuUpdate_IsRunAtApp(int maxWaitMicroseconds)
        {
            /*
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            SendData((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
            */
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < maxWaitMicroseconds)
            {
                SendData(true, (byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, null);
                if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x01_Request_RunningAtWhere, 500, true, out var readbackResult))
                {
                    if (readbackResult != null && readbackResult.Value.Data != null && readbackResult.Value.Data.Count > 0 && readbackResult.Value.Data[0] == RUNNING_AT_APP)
                    {
                        return true;
                    }
                }
                else
                    Thread.Sleep(10);
            }
            return false;
        }
        private bool Mcu_CheckVersion(List<byte> datas, out HardwareVersionInfo? hardwareVersionInfo)
        {
            hardwareVersionInfo = null;
            if (datas == null || datas.Count < 10)
            {
                return false;
            }
            int majorVersion = datas[0];
            int minorVersion = datas[1];
            int build = datas[2];
            List<byte> newData = new();
            bool gotZero = false;
            int splitCount = 0;
            int commentStartIndex = 0;
            string lastModifierName, modelName, comment;
            for (int i = 3; i < datas.Count; i++)
            {
                if (gotZero)
                {
                    if (datas[i] != 0)
                    {
                        newData.Add(datas[i]);
                        gotZero = false;
                    }
                }
                else
                {

                    if (datas[i] == 0)
                    {
                        gotZero = true;
                        splitCount++;
                        if (splitCount == 3)
                        {
                            newData.Add((byte)'^');
                        }
                        else
                        {
                            newData.Add((byte)',');
                        }
                    }
                    else
                    {
                        newData.Add(datas[i]);
                    }
                }
            }
            if (splitCount < 3)
            {
                return false;
            }
            string resutlStr = Encoding.UTF8.GetString(newData.ToArray()).TrimEnd(',');

            if (!DateTime.TryParse(resutlStr.Substring(0, resutlStr.IndexOf(',')), out DateTime lastModifiedDatetime))
            {
                return false;
            }

            var infoStrs = resutlStr.Substring(commentStartIndex).TrimStart(',').Split('^')[0].Split(',');
            comment = resutlStr.Substring(commentStartIndex).TrimStart(',').Split('^')[1];

            hardwareVersionInfo = new HardwareVersionInfo()
            {
                Major = majorVersion,
                Minor = minorVersion,
                Build = build,
                LastBuildDateTime = lastModifiedDatetime,
                LastModifier = infoStrs[1],
                ModelName = infoStrs[2],
                LastComment = comment
            };
            return true;
        }
        public HardwareVersionInfo? Mcu_GetVersionInfo()
        {
            if (serialPort == null)
                return null;
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x02_Request_ReadMcuVersion, 1000, false, out var readbackResult))
            {
                if (readbackResult.Value.Data == null)
                {
                    return null;
                }
                ushort length = (ushort)readbackResult.Value.Data.Count;
                if (Mcu_CheckVersion(readbackResult.Value.Data, out HardwareVersionInfo? hardwareVersionInfo))
                {
                    return hardwareVersionInfo;
                }
                else if (length == McuVersionInfoBytes)
                {
                    List<byte> item = readbackResult.Value.Data;
                    int byteIndex = 0;
                    int majorVersion = item[byteIndex++];
                    int minorVersion = item[byteIndex++];
                    int build = item[byteIndex++];
                    string lastModifiedDatetimeStr = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifiedDatetime_TotalBytes);
                    DateTime lastModifiedDatetime;
                    try
                    {
                        lastModifiedDatetime = new DateTime
                            (int.Parse(lastModifiedDatetimeStr.Substring(0, 4)),
                                int.Parse(lastModifiedDatetimeStr.Substring(4, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(6, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(8, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(10, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(12, 2)))
                            ;
                    }
                    catch
                    {
                        return null;
                    }
                    byteIndex += versionStructLastModifiedDatetime_TotalBytes;
                    string LastModifierName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifierName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructLastModifierName_TotalBytes;
                    string ModelName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructModelName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructModelName_TotalBytes;
                    string Comment = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructComment_TotalBytes).Replace('\0', ' ').Trim();
                    return new HardwareVersionInfo() { Major = majorVersion, Minor = minorVersion, Build = build, LastBuildDateTime = lastModifiedDatetime, LastModifier = LastModifierName, ModelName = ModelName, LastComment = Comment };
                }

            }
            return null;
        }
        public HardwareVersionInfo? Mcu_GetBootVersionInfo()
        {
            if (serialPort == null)
                return null;
            serialPort!.DiscardOutBuffer();
            serialPort!.DiscardInBuffer();
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x03_Request_ReadMcuBootVersion, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x03_Request_ReadMcuBootVersion, 1000, false, out var readbackResult))
            {
                ushort length = (ushort)readbackResult.Value.Data.Count;
                if (length == McuVersionInfoBytes)
                {
                    List<byte> item = readbackResult.Value.Data;
                    int byteIndex = 0;
                    int majorVersion = item[byteIndex++];
                    int minorVersion = item[byteIndex++];
                    int build = item[byteIndex++];
                    string lastModifiedDatetimeStr = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifiedDatetime_TotalBytes);
                    DateTime lastModifiedDatetime;
                    try
                    {
                        lastModifiedDatetime = new DateTime
                            (int.Parse(lastModifiedDatetimeStr.Substring(0, 4)),
                                int.Parse(lastModifiedDatetimeStr.Substring(4, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(6, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(8, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(10, 2)),
                                int.Parse(lastModifiedDatetimeStr.Substring(12, 2)))
                            ;
                    }
                    catch
                    {
                        return null;
                    }
                    byteIndex += versionStructLastModifiedDatetime_TotalBytes;
                    string LastModifierName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructLastModifierName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructLastModifierName_TotalBytes;
                    string ModelName = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructModelName_TotalBytes).Replace('\0', ' ').Trim();
                    byteIndex += versionStructModelName_TotalBytes;
                    string Comment = Encoding.UTF8.GetString(item.ToArray(), byteIndex, versionStructComment_TotalBytes).Replace('\0', ' ').Trim();
                    return new HardwareVersionInfo()
                    {
                        Major = majorVersion,
                        Minor = minorVersion,
                        Build = build,
                        LastBuildDateTime = lastModifiedDatetime,
                        LastModifier = LastModifierName,
                        ModelName = ModelName,
                        LastComment = Comment
                    };
                }
            }
            return null;
        }
        public enum McuComPortObjects
        {
            Keyboard,
            AnalogChannelBaseplate1,
            AnalogChannelBaseplate2,
        }
        private bool Update_VerifyStage1(List<byte> sendAllData, uint fillCount = 0)
        {
            var sendData = new List<byte>();

            #region totalBytes

            sendData.Add((byte)((sendAllData.Count & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((sendAllData.Count & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((sendAllData.Count & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((sendAllData.Count & 0xff_00_00_00) >> 24));

            #endregion
            #region CRC Code

            UInt32 crcCode = CRC32.GetCRC32Code(sendAllData);
            sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));

            #endregion
            serialPort!.DiscardOutBuffer();
            serialPort!.DiscardInBuffer();
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x0B_Request_UpdateVerifyStage1, sendData);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x0B_Request_UpdateVerifyStage1, 5000, true, out var result))
            {
                int readback_Length = 0;
                readback_Length |= result.Value.Data[3];
                readback_Length <<= 8;
                readback_Length |= result.Value.Data[2];
                readback_Length <<= 8;
                readback_Length |= result.Value.Data[1];
                readback_Length <<= 8;
                readback_Length |= result.Value.Data[0];

                UInt32 readbackCrcCode = 0;
                readbackCrcCode |= result.Value.Data[4 + 3];
                readbackCrcCode <<= 8;
                readbackCrcCode |= result.Value.Data[4 + 2];
                readbackCrcCode <<= 8;
                readbackCrcCode |= result.Value.Data[4 + 1];
                readbackCrcCode <<= 8;
                readbackCrcCode |= result.Value.Data[4 + 0];
                if (readback_Length - fillCount != sendAllData.Count)
                {
                    return false;
                }
                //尾巴CRC结果计算有点问题，实际内容无误 ljw 24.3
                //if (readback_Length - fillCount != sendAllData.Count || readbackCrcCode != crcCode)
                //{
                //	Debug.WriteLine($"Error:CRC1:{readbackCrcCode:X4} CRC2:{crcCode:X4}");
                //	return false;
                //}

                return true;
            }
            return false;
        }
        private bool Update_Switch()
        {
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x0C_Request_UpdateSwitch, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x0C_Request_UpdateSwitch, 2000, true, out var result))
            {
                return result.Value.Data[0] == 0;
            }
            return false;
        }
        private bool Update_VerifyStage2()
        {
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x0D_Request_UpdateVerifyStage2, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x0D_Request_UpdateVerifyStage2, 2000, true, out var result))
            {
                return result.Value.Data[0] == 0;
            }
            return false;
        }
        private bool Update_SendNowTimestamp()
        {
            DateTime nowTime = DateTime.Now;
            string nowTimestamp =
                $"{nowTime.Year.ToString()}{nowTime.Month.ToString().PadLeft(2, '0')}{nowTime.Day.ToString().PadLeft(2, '0')}{nowTime.Hour.ToString().PadLeft(2, '0')}{nowTime.Minute.ToString().PadLeft(2, '0')}{nowTime.Second.ToString().PadLeft(2, '0')}";
            List<byte> sendData = new List<byte>();
            foreach (char c in nowTimestamp)
                sendData.Add((byte)c);
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x15_Request_WriteUpdateTimeStamp, sendData);
            Thread.Sleep(500);
            return true;
        }
        public static bool DoUpdate(UpdateItem updateItem, Action<string>? debugWriter)
        {
            bool bOk = true;
            McuComPortUpdater? mcuComPortUpdater = CtrlAnalogChannel_JiHe2d5G.baseObj1;
            #region step1:打开串口

            if (mcuComPortUpdater == null)
            {
                debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName}对用的Mcu通信接口还没有对应实现！");
                return false;
            }
            string BoardName = updateItem.BoardName.Trim();
            string comPortNum = BoardName.Substring(BoardName.Length - 1, 1);

            bool bOpened = mcuComPortUpdater.Open($"COM{comPortNum}");
            if (!bOpened)
            {
                debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName}对用的Mcu通信接口还没有对应实现！");
                return false;
            }

            #endregion


            #region step2:读取app的版本

            HardwareVersionInfo? appVersionInfo = mcuComPortUpdater.Mcu_GetVersionInfo();
            HardwareVersionInfo? bootVersionInfo = mcuComPortUpdater.Mcu_GetBootVersionInfo();
            WriteLog($"通道底板BOOT版本: {bootVersionInfo?.ToString()}");
            WriteLog($"通道底板App 版本: {appVersionInfo?.ToString()}");
            #endregion
            #region Step3:跳转到boot ,准备更新

            //mcuComPortUpdater.McuUpdate_Start((byte)updateItem.TypeID,0x00);

            mcuComPortUpdater.McuUpdate_Start((byte)0x77, 0x01); //目前暂时只支持App的更新
            Thread.Sleep(500);
            if (!mcuComPortUpdater.McuUpdate_IsRunAtBoot(2500))
            {
                debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} 未成功进入BOOT");
                if (!mcuComPortUpdater.McuUpdate_IsRunAtBoot(2500))
                {
                    debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} 未工作在APP，可能通道板未上电");
                }
                else
                {
                    debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} 目前工作在APP");
                }
                mcuComPortUpdater.Close();
                return false;
            }

            #endregion
            #region Step4:初始化Boot中的更新变量

            //mcuComPortUpdater.McuUpdate_Start((byte)updateItem.TypeID, 0x01);//目前暂时怒支持更新
            mcuComPortUpdater.McuUpdate_Start((byte)0x77, 0x01); //目前暂时只支持App的更新
            Thread.Sleep(500);

            #endregion
            #region Step5:Send All Content,Write at backup temp area

            var dataRealLen = updateItem.Content.Length;
            //test
            //if (dataRealLen > 0x4800)
            //{
            //	dataRealLen = 0x4800;
            //}
            if (dataRealLen == 0)
            {
                debugWriter?.Invoke($"Error: updateItem.Content.Length == 0");
                return false;
            }
            List<byte> dataToWrite = new List<byte>();

            //最后一包补位
            uint fillInLastLen = 0;
            if (dataRealLen % SPAN_SIZE != 0)
            {
                fillInLastLen = (uint)(SPAN_SIZE - (dataRealLen % SPAN_SIZE));
                Debug.WriteLine($"fillInLastLen:{fillInLastLen}");
                for (int i = 0; i < dataRealLen; i++)
                {
                    dataToWrite.Add(updateItem.Content[i]);
                }
                for (int i = dataRealLen; i < fillInLastLen + dataRealLen; i++)
                {
                    dataToWrite.Add(0xFF);
                }
            }
            else
            {
                //dataToWrite = updateItem.Content.ToList();
                for (int i = 0; i < dataRealLen; i++)
                {
                    dataToWrite.Add(updateItem.Content[i]);
                }
            }
            bOk = mcuComPortUpdater.McuUpdate_SendData(dataToWrite);

            #endregion

            #region Step6:Verify backup content

            if (bOk)
            {
                Thread.Sleep(4000);
                bOk = mcuComPortUpdater.Update_VerifyStage1(dataToWrite);
                //bOk = mcuComPortUpdater.Update_VerifyStage1(updateItem.Content.ToList(), fillInLastLen);
                if (!bOk)
                {
                    debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} Error at VerifyStage1");
                }
            }

            #endregion

            #region Step7:move content from backup area to App area

            if (bOk)
            {
                Thread.Sleep(300);
                bOk = mcuComPortUpdater.Update_Switch();
                if (!bOk)
                {
                    Thread.Sleep(500);
                    bOk = mcuComPortUpdater.Update_Switch();
                    if (!bOk)
                    {
                        debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} Error at Switch");
                    }
                }
            }

            #endregion

            #region Step8:VerifyStage2,compare app content and backup content

            if (bOk)
            {
                bOk = mcuComPortUpdater.Update_VerifyStage2();
                if (!bOk)
                {
                    debugWriter?.Invoke($"updaterItemType={updateItem.Type},BoardName={updateItem.BoardName} Error at VeriftStage2");
                }
            }

            #endregion
            #region Step9:Write Update timestamp

            if (bOk)
            {
                mcuComPortUpdater.Update_SendNowTimestamp();
            }

            #endregion
            #region Setp10: finished

            if (bOk)
            {
                mcuComPortUpdater.McuUpdate_Finished();
            }

            #endregion

            mcuComPortUpdater.Close();
            return bOk;
        }
        public void RegisterAppRunStartTime()
        {
            DateTime nowTime = DateTime.Now;
            string nowTimeStr = $"{nowTime.Year}-{nowTime.Month.ToString().PadLeft(2, '0')}-{nowTime.Day.ToString().PadLeft(2, '0')} {nowTime.Hour.ToString().PadLeft(2, '0')}:{nowTime.Minute.ToString().PadLeft(2, '0')}:{nowTime.Second.ToString().PadLeft(2, '0')}";
            List<byte> data = new List<byte>();
            foreach (char c in nowTimeStr)
                data.Add((byte)c);
            data.Add(0);
            //SendData(true, (byte)Updater_ReqScopeXommands.CMD0x06_Request_RegisterAppStartTime, data);
            Hd.SysLogger?.Invoke($"AppRunStartTime:{nowTimeStr}", "Info");
        }
        public String ReadAppRegistedRunStartTime()
        {
            //SendData(true, (byte)Updater_ReqScopeXommands.CMD0x07_Request_ReadbackAppStartTime, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x07_Request_ReadbackAppStartTime, 500, true, out var result))
            {
                return Encoding.UTF8.GetString(result.Value.Data.ToArray(), 0, result.Value.Data.Count);
            }
            return "";
        }
        public String ReadUpdatetimeStamp()
        {
            SendData(true, (byte)Updater_ReqScopeXommands.CMD0x04_Request_ReadUpdateTimeStamp, null);
            if (ReadSpecailMessage((byte)Updater_ReqScopeXommands.CMD0x04_Request_ReadUpdateTimeStamp, 500, true, out var result))
            {
                return Encoding.UTF8.GetString(result.Value.Data.ToArray(), 0, result.Value.Data.Count);
            }
            return "";
        }


    }
}
