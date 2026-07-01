using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using EventBus;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    internal class ComPort
    {
        Action<Byte[]> _DataReceived;
        private readonly SerialPort _SerialPort;

        public ComPort(String name, Action<Byte[]> dataReceived, Int32 baudrate = 19200, Parity parity = Parity.None, Int32 databits = 8, StopBits stopbits = StopBits.One)
        {
            _SerialPort = new()
            {
                PortName = name,
                BaudRate = baudrate,
                Parity = parity,
                DataBits = databits,
                StopBits = stopbits,
                ReadBufferSize = 1024,
                ReadTimeout = 2500,
                WriteTimeout = 5000,
                ReceivedBytesThreshold = 1,
            };

            _SerialPort.DataReceived += SerialPort_DataReceived;
            _SerialPort.ErrorReceived += _SerialPort_ErrorReceived;
            _DataReceived = dataReceived;
            EventBroker.Instance.GetEvent<SerialPortRecord>().Subscrip(SerialChangeHandler);
            Open();
        }

        private void _SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"键盘板串口收到异常,错误类型为：{e?.EventType}", LogLevel.Debug));
        }

        /// <summary>
        /// 串口连接改变事件处理程序
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg"></param>
        private void SerialChangeHandler(object arg1, EventArgs<SerialPortRecord> arg)
        {
            if (arg == null || arg.Data == null || string.IsNullOrEmpty(arg.Data.SerialPortName) || arg.Data.SerialPortName.Contains(_SerialPort.PortName))
                return;

            if (!_SerialPort.IsOpen && arg.Data.Direction == SerialPortChangeType.Insert)
            {
                // 串口已断开，并重新插入时，自动重连。
                try
                {
                    _SerialPort.Open();
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Serial Port({_SerialPort.PortName}) Insert and reopen success!", EventBus.LogLevel.Debug));
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Serial Port({_SerialPort.PortName}) Insert and reopen faild! \r\n{e.Message}", EventBus.LogLevel.Debug));
                }
            }
        }

        private void SerialPort_DataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            Int32 count = _SerialPort.BytesToRead;
            Byte[] buffer = new Byte[count];
            _SerialPort.Read(buffer, 0, count);
            _DataReceived?.Invoke(buffer);
        }

        public void Open()
        {
            if (_SerialPort == null) return;
            DateTime dateTime = DateTime.Now;
            Int32 timeout = 10 * 1000;
            while (!_SerialPort.IsOpen)
            {
                Thread.Sleep(100);
                try
                {
                    _SerialPort?.Open();
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Serial Port({_SerialPort.PortName}) open success!", EventBus.LogLevel.Debug));
                }
                catch (Exception ex)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Serial Port cannot open!\r\n" + ex.Message, EventBus.LogLevel.Error));
                }
                if ((DateTime.Now - dateTime).TotalMilliseconds >= timeout)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Serial Port  open Timeout!", EventBus.LogLevel.Error));
                    break;
                }
            }
        }

        public void Close()
        {
            if (_SerialPort.IsOpen)
            {
                _SerialPort.Close();
            }
        }

        public Boolean WriteBytes(Byte[] bytes)
        {
            if (_SerialPort == null)
                return false;

            if (bytes == null || bytes.Length <= 0)
                return true;

            try
            {
                if (!_SerialPort.IsOpen)
                {
                    try
                    {
                        _SerialPort.Open();
                    }
                    catch { }
                }

                if (_SerialPort.IsOpen)
                {
                    _SerialPort.Write(bytes, 0, bytes.Length);
                    return true;
                }
                else
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"The serial port({_SerialPort.PortName}) reopen fails!", EventBus.LogLevel.Error));
                    return false;
                }
            }
            catch (TimeoutException e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("The write operation of serial port fails!" + e.ToString(), EventBus.LogLevel.Error));
            }
            return false;
        }
    }
}
