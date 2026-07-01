using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ScopeX.ComModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.CodeDom;
using EventBus;
using System.IO.Ports;
using ScopeX.Core;
using System.Text;
using static ScopeX.Controls.Common.APIs.APIsStructs;

namespace ScopeX.U2
{
    internal class Keyboard
    {
        //public static String FindKeyBoardSerialPort(out String msg)
        //{
        //    //String path = "c:\\test.txt";
        //    //if(System.IO.File.Exists(path))System.IO.File.Delete(path);
        //    List<String> msgs = new List<String>();
        //    msg = String.Empty;
        //    String tempmsg = String.Empty;
        //    var serialport = System.IO.Ports.SerialPort.GetPortNames().FirstOrDefault(x =>
        //    {
        //        if (x == "COM2")
        //        {
        //            Thread.Sleep(5000);
        //            return false;
        //        }

        //        try
        //        {
        //            System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort(x, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        //            serialPort.DtrEnable = false;
        //            serialPort.RtsEnable = false;
        //            serialPort.Open();
        //            Thread.Sleep(2500);//由于按键板在打开后在某些情况下时会导致按键板重启，按键板重启后会有1+s左右的无法通信的窗口时间故增加延迟
        //            try
        //            {
        //                serialPort.WriteLine("*IDN?");
        //                DateTime dateTime = DateTime.Now;
        //                Int32 bytecount = 0;
        //                while (true)
        //                {
        //                    bytecount = serialPort.BytesToRead;
        //                    Thread.Sleep(10);
        //                    if (bytecount > 10 || (DateTime.Now - dateTime).TotalMilliseconds >= 2000) break;
        //                }
        //                if (bytecount > 0)
        //                {
        //                    Byte[] buffer = new Byte[bytecount];
        //                    serialPort.Read(buffer, 0, bytecount);
        //                    String result = System.Text.Encoding.Default.GetString(buffer);
        //                    msgs.Add($"{x}:{result}");
        //                    if (result.IndexOf("Keyboard") >= 0)//兼容旧版的固件，旧版固件使用'.'作为分隔符，新版的兼容标准SCPI指令改为','为分隔符
        //                    {
        //                        tempmsg = result;
        //                        return true;
        //                    }
        //                }
        //                msgs.Add($"{x}:读取超时");
        //            }
        //            catch (Exception ex)
        //            {
        //                msgs.Add($"{x}通信错误:{ex.Message}");
        //            }
        //            finally
        //            {
        //                serialPort.Close();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            msgs.Add($"{x}打开失败:{ex.Message}");
        //        }
        //        return false;
        //    });
        //    if (msgs.Count > 0)
        //    {
        //        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(String.Join("\r\n", msgs), LogLevel.Info));
        //    }
        //    else
        //    {
        //        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("系统中不存在任何串口", LogLevel.Error));
        //    }
        //    //System.IO.File.WriteAllLines(path, msgs);
        //    if (!String.IsNullOrEmpty(serialport))
        //    {
        //        msg = tempmsg;
        //    }
        //    else
        //    {
        //        serialport = $"COM{Constants.COMPORTNUM_KEYBOARD}";
        //        if (Array.FindIndex(SerialPort.GetPortNames(), x => x.Equals(serialport)) == -1) serialport = String.Empty;
        //    }

        //    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"键盘板使用的串口号为：{serialport},其版本号为：{msg}", LogLevel.Debug));
        //    return serialport;
        //}


        public static String FindKeyBoardSerialPort(out String msg)
        {
            string port = null;
            msg = String.Empty;
            var serialport = $"COM{Constants.COMPORTNUM_KEYBOARD}";
            Int32 index = 5;
            while (index > 0)//按键板失效，通信前，强制开关5次，解决偶尔按键板异常
            {
                index--;
                try
                {
                    System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort(serialport, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    serialPort.DtrEnable = true;
                    serialPort.RtsEnable = true;
                    serialPort.ErrorReceived += SerialPort_ErrorReceived;
                    serialPort.Open();
                    Thread.Sleep(2500);
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Open_Error_0001.GetDescription()} {serialport}打开失败:{ex.Message}", LogLevel.Info));
                    Thread.Sleep(50);
                }
            }
            index = 5;
            while (index > 0)
            {
                index--;
                try
                {
                    System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort(serialport, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    serialPort.ReadTimeout = 5000;
                    serialPort.WriteTimeout = -1;
                    serialPort.ErrorReceived += SerialPort_ErrorReceived;
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;
                    serialPort.Open();
                    Thread.Sleep(5000);//由于按键板在打开后在某些情况下时会导致按键板重启，按键板重启后会有1+s左右的无法通信的窗口时间故增加延迟
                    try
                    {
                        serialPort.WriteLine("*IDN?");
                        DateTime dateTime = DateTime.Now;
                        Int32 bytecount = 0;
                        while (true)
                        {
                            bytecount = serialPort.BytesToRead;
                            Thread.Sleep(10);
                            if (bytecount > 10 || (DateTime.Now - dateTime).TotalMilliseconds >= 2000) break;
                        }
                        if (bytecount > 0)
                        {
                            Byte[] buffer = new Byte[bytecount];
                            serialPort.Read(buffer, 0, bytecount);
                            String result = System.Text.Encoding.Default.GetString(buffer);
                            if (result.IndexOf("Keyboard") >= 0)//兼容旧版的固件，旧版固件使用'.'作为分隔符，新版的兼容标准SCPI指令改为','为分隔符
                            {
                                msg = result;
                                port = serialport;
                                break;
                            }
                        }
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Read_Timeout_0002.GetDescription()} {serialport}:读取超时", LogLevel.Info));
                    }
                    catch (Exception ex)
                    {
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Exception_0003.GetDescription()} {serialport}通信错误:{ex.Message}", LogLevel.Info));
                    }
                    finally
                    {
                        serialPort.Close();
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Open_Error_0001.GetDescription()} {serialport}打开失败:{ex.Message}", LogLevel.Info));
                }
            }
            if (string.IsNullOrEmpty(port))
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Open_Error_0001.GetDescription()} 错误信息:键盘板初始化失败", LogLevel.Debug));
            }
            else
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"键盘板使用的串口号为：{serialport},其版本号为：{msg}", LogLevel.Debug));
            }
            return port;
        }

        private static void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"查找键盘板串口号时收到串口异常,错误类型为：{e?.EventType}", LogLevel.Debug));
        }

        /// <summary>
        /// 包头定义
        /// </summary>
        public const UInt16 PACKET_HEADER = 0xAA55;
        /// <summary>
        /// 包尾定义
        /// </summary>
        public const UInt16 PACKET_ENDER = 0x55AA;
        /// <summary>
        /// 按键字
        /// </summary>
        public const Byte KEY_CODE_COMMAND = 0x00;
        private System.Timers.Timer _Timer = new System.Timers.Timer(50);
        private KeySpeed<KeyEnum> _KeySpeed;
        private List<Byte> _Buffer = new List<Byte>();

        private Keyboard()
        {
            _KeySpeed = new KeySpeed<KeyEnum>(_SupportSpeedUpKey, 500, 1, 0.50);
            _Timer.Stop();
            _Timer.AutoReset = false;
            _Timer.Elapsed += _Timer_Elapsed;
        }

        private void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            KeyboardLed.Default.EnbleWrite = true;
        }

        private readonly ConcurrentQueue<KeyCode> _KeyQueue = new();


        internal void PreProcess(Byte buffer, Int16 step = 0) => _KeyQueue.Enqueue(new KeyCode(buffer, step));

        public void Receive(Byte[] buffer)
        {
            _Buffer.AddRange(buffer);
            AnalysisKeyCode();
        }

        /// <summary>
        /// 对接收到的数据包进行拆包
        /// </summary>
        private void AnalysisKeyCode()
        {
            Int32 size = Marshal.SizeOf<KeyCodeData>();
            lock (_Buffer)
            {
                while (true)
                {
                    if (size > _Buffer.Count)
                    {
                        return;
                    }

                    Boolean findheader = false;
                    for (Int32 i = 0; i < _Buffer.Count - 1; i++)
                    {
                        if (BitConverter.ToUInt16(_Buffer.ToArray(), i) == PACKET_HEADER)
                        {
                            _Buffer.RemoveRange(0, i);
                            findheader = true;
                            break;
                        }
                        else
                        {

                        }
                    }

                    if (!findheader)
                    {
                        _Buffer.Clear();
                    }
                    else if (_Buffer.Count >= size && findheader)
                    {
                        var bytes = _Buffer.Take(size).ToArray();
                        try
                        {
                            PreKey(bytes);
                        }
                        catch (Exception ex)
                        {
                            // 处理掉异常，
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"错误码:{ErrorType.K_Open_Error_0001.GetDescription()} 解析键盘板消息[{Bytes2Hex(bytes)}]时异常(已被丢弃)：{ex}", LogLevel.Error));
                        }
                        finally
                        {
                            // 无论是否异常，都需要将本次数据清掉，假如异常后不清除，就会导致收到一个正确数据包，但是消息处理时异常，_Buffer中始终存在这个包结构正常，但无法处理的消息，从而阻塞后续包解析。
                            _Buffer.RemoveRange(0, size);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        private string Bytes2Hex(byte[] bytes)
        {
            if (bytes == null)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (var item in bytes)
                sb.Append($"{item.ToString("X2")} ");

            return sb.ToString().TrimEnd(' ');
        }


        /// <summary>
        /// 处理数据包
        /// </summary>
        /// <param name="buffer">数据包</param>
        private void PreKey(Byte[] buffer)
        {
            if (buffer.Length < Marshal.SizeOf<KeyCodeData>())
            {
                return;
            }
            KeyCodeData keycodedata = Unsafe.As<Byte, KeyCodeData>(ref buffer[0]);

            //验证数据,此处需要注意高低字节
            if (keycodedata.PacketHeader != PACKET_HEADER //验证包头
                || keycodedata.PacketEnder != PACKET_ENDER //验证包尾
                || keycodedata.Command != KEY_CODE_COMMAND //验证控制字
                || !Enum.GetValues<KeyEnum>().Any(x => x == keycodedata.Key))//验证按键值是否合法
            {
                return;
            }
            if (keycodedata.Step != 0)
            {
                KeyboardLed.Default.EnbleWrite = false;
                if (_Timer.Enabled)
                {
                    _Timer.Stop();
                }
                _Timer.Start();
            }
            switch (keycodedata.Key)
            {
                case KeyEnum.Autoset:
                    PreProcess(KeyCode.AUTOSET);
                    break;
                case KeyEnum.Apps:
                    PreProcess(KeyCode.VK_APPS);
                    break;
                case KeyEnum.TouchLock:
                    PreProcess(KeyCode.TOUCH);
                    break;
                case KeyEnum.RunStop:
                    PreProcess(KeyCode.RUNSTOP);
                    break;
                case KeyEnum.Single:
                    PreProcess(KeyCode.SINGLE);
                    break;
                case KeyEnum.Normal:
                    PreProcess(KeyCode.NORMAL);
                    break;
                case KeyEnum.Auto:
                    PreProcess(KeyCode.AUTO);
                    break;
                case KeyEnum.Force:
                    PreProcess(KeyCode.TRIG_FORCE);
                    break;
                case KeyEnum.PrtSc:
                    PreProcess(KeyCode.VK_SCREENSHOT);
                    break;
                case KeyEnum.Utility:
                    PlatformUIManager.Default.Platform.KeyEnumUtilityHandler();
                    //PreProcess(KeyCode.SETTING);
                    break;

                case KeyEnum.DVM:
                    PreProcess(KeyCode.VK_VOLTMETER);
                    break;
                case KeyEnum.AWG:

                    PreProcess(KeyCode.VK_AWGALL);
                    break;
                case KeyEnum.Clear:
                    PreProcess(KeyCode.VK_CLEAR);
                    break;
                case KeyEnum.Default:
                    PreProcess(KeyCode.DEFAULT);
                    break;

                case KeyEnum.Math:
                    PreProcess(KeyCode.MATH);
                    break;
                case KeyEnum.Reference:
                    PreProcess(KeyCode.REF);
                    break;
                case KeyEnum.Digital:
                    PreProcess(KeyCode.LOGIC);
                    break;
                case KeyEnum.Bus:
                    PreProcess(KeyCode.DECODE);
                    break;
                case KeyEnum.Trigger:
                    PlatformUIManager.Default.Platform.KeyEnumTriggerHandler();
                    //PreProcess(KeyCode.TRIGGER);
                    break;
                case KeyEnum.Measure:
                    PreProcess(KeyCode.MEASURE);
                    break;
                case KeyEnum.CH1:
                    PreProcess(KeyCode.CH1);
                    break;
                case KeyEnum.CH2:
                    PreProcess(KeyCode.CH2);
                    break;
                case KeyEnum.CH3:
                    PreProcess(KeyCode.CH3);
                    break;
                case KeyEnum.CH4:
                    PreProcess(KeyCode.CH4);
                    break;
                case KeyEnum.CH5:
                    PreProcess(KeyCode.CH5);
                    break;
                case KeyEnum.CH6:
                    PreProcess(KeyCode.CH6);
                    break;
                case KeyEnum.CH7:
                    PreProcess(KeyCode.CH7);
                    break;
                case KeyEnum.CH8:
                    PreProcess(KeyCode.CH8);
                    break;
                case KeyEnum.Cursor:
                    PreProcess(KeyCode.CURSOR);
                    break;
                case KeyEnum.QuickMeasure:
                    PreProcess(KeyCode.VK_SNAPSHOT);
                    break;
                case KeyEnum.UltraAcq:
                    PreProcess(KeyCode.FASTACQ);
                    break;
                case KeyEnum.TriggerLevel:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_TRIG_YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        Int16 step = (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step));
                        //System.IO.File.AppendAllLines($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\11.txt",new[] { $"{DateTime.Now}:{step}"});
                        PreProcess(KeyCode.KNOB_TRIG_YPOS_LEFT, step);
                    }
                    else
                    {
                        Int16 step = (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step));
                        PreProcess(KeyCode.KNOB_TRIG_YPOS_RIGHT, step);
                        //System.IO.File.AppendAllLines($"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\11.txt", new[] { $"{DateTime.Now}:{step}" });
                    }

                    break;
                case KeyEnum.MultipuposeA:
                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_UPMULTI_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_UPMULTI_LEFT, (Byte)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_UPMULTI_RIGHT, (Byte)Math.Abs(keycodedata.Step));
                    }
                    break;
                case KeyEnum.Multipupose:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_MULTI_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_MULTI_LEFT, (Byte)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_MULTI_RIGHT, (Byte)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.MultipuposeB:
                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_DNMULTI_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_DNMULTI_LEFT, (Byte)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_DNMULTI_RIGHT, (Byte)Math.Abs(keycodedata.Step));
                    }
                    break;

                case KeyEnum.HorizontalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_XLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_XLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_XLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.VerticalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_YLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_YLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_YLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;

                case KeyEnum.CH1VerticalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH1YLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH1YLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH1YLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.CH2VerticalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH2YLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH2YLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH2YLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.CH3VerticalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH3YLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH3YLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH3YLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.CH4VerticalScale:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH4YLEVEL_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH4YLEVEL_LEFT, (Int16)Math.Abs(keycodedata.Step));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH4YLEVEL_RIGHT, (Int16)Math.Abs(keycodedata.Step));
                    }

                    break;
                case KeyEnum.VerticalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_YPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_YPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;

                case KeyEnum.CH1VerticalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH1YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH1YPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH1YPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;
                case KeyEnum.CH2VerticalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH2YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH2YPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH2YPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;
                case KeyEnum.CH3VerticalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH3YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH3YPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH3YPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;
                case KeyEnum.CH4VerticalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_CH4YPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_CH4YPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_CH4YPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;
                case KeyEnum.HorizontalPosition:

                    if (keycodedata.Step == 0)
                    {
                        PreProcess(KeyCode.KNOB_XPOS_SELECT);
                    }
                    else if (keycodedata.Step < 0)
                    {
                        PreProcess(KeyCode.KNOB_XPOS_LEFT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }
                    else
                    {
                        PreProcess(KeyCode.KNOB_XPOS_RIGHT, (Int16)Math.Abs(_KeySpeed.GetStep(keycodedata.Key, keycodedata.Step)));
                    }

                    break;
                case KeyEnum.Analysis:
                    break;
                case KeyEnum.Save:
                    PreProcess(KeyCode.STORAGE);
                    break;
                case KeyEnum.View:
                    PreProcess(KeyCode.DISPLAY);
                    break;
                case KeyEnum.Help:
                    PreProcess(KeyCode.HELP);
                    break;
                case KeyEnum.TimeBaseMenu:
                    PreProcess(KeyCode.TIMEBASE);
                    break;
                default:
                    return;
            }

            OnKeyReady();
        }

        private readonly IReadOnlyList<KeyEnum> _SupportSpeedUpKey = new List<KeyEnum>()
        {
            KeyEnum.CH1VerticalPosition,
            KeyEnum.CH2VerticalPosition,
            KeyEnum.CH3VerticalPosition,
            KeyEnum.CH4VerticalPosition,
            KeyEnum.HorizontalPosition,
            KeyEnum.VerticalPosition,
            KeyEnum.TriggerLevel,
        };

        public Action<KeyCode> ProcessKey { get; set; } = null;

        private void OnKeyReady()
        {
            while (_KeyQueue.TryDequeue(out KeyCode keycode) && keycode != null && ProcessKey != null)
            {
                ProcessKey(keycode);
            }
        }


        public static readonly Keyboard Default = new();
    }
}
