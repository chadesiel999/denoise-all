using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    public abstract class SystemMonitor
    {
        private static SystemMonitor? _Instance = null;
        internal static SystemMonitor Default
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Hd.CurrProductType switch
                    {
                        ProductType.JiHe_MSO8000HD => new SysMonitor_8000HD(),
                        _ => new SysMonitor_8000HD()
                    };
                }
                return _Instance;
            }
        }

        internal ProductType Type { get; init; }

        internal const Double InvalidTemperature = 500D;//非法温度
        internal const Int32 HoldoffBymsAnalogTemperatureCompensate = 2 * 1000;
        internal const Int32 HoldoffBymsAutoFanControl = 5 * 1000;//间隔5秒读取一次温度 计算一次转速

        internal virtual AcqBdNo AcqBdNo { get; set; } = AcqBdNo.B5;

        internal PIDContoller _ChannelPID = new PIDContoller()
        {
            Kp = 200,
            Ki = 0,
            Kd = 0
        };

        internal PIDContoller _PciePID = new PIDContoller()
        {
            Kp = 500,
            Ki = 30,
            Kd = 0
        };

        private List<Double> _AnalogChannelTemperatures = new List<Double>() { 40.0, -40.0 };
        internal List<Double> AnalogChannelTemperatures => _AnalogChannelTemperatures;

        internal Double MinFanSpeed { get; set; } = 0;

        internal Double MaxFanSpeed { get; set; } = 4000;

        internal void ReadAnalogChannelTemperatures()
        {
            _AnalogChannelTemperatures[0] = AbstractController_AnalogChannel.ReadTemperatures();
        }

        internal String ReadAndGetAnalogChannelTemperatures(Int32 whichType)
        {
            ReadAnalogChannelTemperatures();
            return whichType switch
            {
                0 => $"{_AnalogChannelTemperatures[0]}",
                1 => $"{_AnalogChannelTemperatures[1]}",
                _ => "0.0"
            };
        }
        public Double GetAnalogChannelTemperature(Int32 whichType = 0)
        {
            ReadAnalogChannelTemperatures();
            return whichType switch
            {
                0 => _AnalogChannelTemperatures[0],
                1 => _AnalogChannelTemperatures[1],
                _ => 0.0,
            };
        }

        internal Int32 ComplementConvert(UInt32 data)
        {
            data &= 0x7ff;
            Int32 mark = 1;
            if ((data & 0x400) != 0)
            {
                mark = -1;
                data &= 0x3ff;
                data = ~data;
                data &= 0x3ff;
                data = +1;
            }
            //补码运算
            Int32 temprature = (Int32)data;
            temprature *= mark;
            return temprature;
        }

        internal abstract String Read();

        internal abstract String GetChannelTemperaturesByCelsius();

        internal virtual List<(String Description, Double Temperature)> GetPcieFpgaTemperatureBymCelsius()
        {
            var temprature = 0.0;
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            Thread.Sleep(5);
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
            temprature = (data * 501.3743 / 1024) - 273.15;
            temprature *= 1000;
            temprature = Math.Round(temprature);

            return new List<(String Description, Double Temperature)>() { ("Pcie_Fpga", temprature) };
        }

        internal abstract List<(String Description, Double Temperature)> GetPcieBoardTemperatureBymCelsius();

        internal abstract List<(String Description, Double Temperature)> GetAcqFpgaTemperatureBymCelsius(AcqBdNo acqBdNo);

        internal virtual List<(String Description, Double Temperature)> GetAcqFpgaTemperatureBymCelsius()
        {
            var temprature = 0.0;
            var data = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, AcqBdNo);
            data &= 0x3ff;
            temprature = (int)data;
            temprature = (temprature * 501.3743 / 1024) - 273.6777;
            temprature *= 1000;
            temprature = Math.Round(temprature);

            return new List<(String Description, Double Temperature)>() { ("Acq_Fpga", temprature) };
        }

        internal abstract List<(String Description, Double Temperature)> GetAcqBoardTemperatureBymCelsius(AcqBdNo acqBdNo);

        internal abstract List<(String Description, Double Temperature)> GetAcqBoardTemperatureBymCelsius();

        internal virtual void InitFanSpeed()
        {
            CtrlFanSpeed((MinFanSpeed + MaxFanSpeed) / 2);
            CtrlFanSpeed((MinFanSpeed + MaxFanSpeed) / 2, false);
        }

        /// <summary>
        /// 控制左右侧所有风扇  控制风扇寄存器可能不一致或者方式不同，必须重写此函数
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="isRight"></param>
        internal abstract void CtrlFanSpeed(Double speed, Boolean isRight = true);

        /// <summary>
        /// 控制左右侧单个风扇  控制风扇寄存器可能不一致或者方式不同，必须重写此函数
        /// </summary>
        /// <param name="fanIndex"></param>
        /// <param name="speed"></param>
        /// <param name="isRight"></param>
        internal abstract void CtrlFanSpeed(Double speed, Int32 fanIndex, Boolean isRight = true);

        internal virtual void CtrlFanSpeed(PcieBdReg.W register, UInt32 speed)
        {
            HdIO.WriteReg(register, speed);
        }

        /// <summary>
        /// 不同风扇型号的最大转速不一样，根据需要传入最大转速
        /// </summary>
        /// <param name="register"></param>
        /// <param name="speed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="minSpeed"></param>
        internal virtual void CtrlFanSpeed(PcieBdReg.W register, UInt32 speed, UInt32 maxSpeed, UInt32 minSpeed)
        {
            maxSpeed = maxSpeed <= 0 ? 4000 : maxSpeed;
            //hc20241009:value取值范围为[0,12500]，对应占空比为[0%,100%],对应转速speed为[0,MaxSpeed]RPM；
            //speed与value为线性关系
            var ratio = 12500 / maxSpeed;
            speed = Math.Clamp(speed, minSpeed, maxSpeed);
            var value = ratio * speed;
            HdIO.WriteReg(register, value);
        }

        /// <summary>
        /// Temperature read and temperature compensation time stamp
        /// </summary>
        internal DateTime _LastReadTemperatureTimestamp = DateTime.MinValue;

        /// <summary>
        /// Fan control time stamp
        /// </summary>
        internal DateTime _LastCtrlFanTimestamp = DateTime.MinValue;

        public static Boolean UsingUIParam { get; set; } = false;

        private List<Double> _TemperatureOffsetList = new();

        internal virtual Boolean IsTemperatureCompensated { get; set; } = false;

        internal void ResetReadTemperature()
        {
            _LastReadTemperatureTimestamp = DateTime.MinValue;
            _LastCtrlFanTimestamp = DateTime.MinValue;
        }

        internal void SystemTemperatureProcess()
        {
            AnalogTemperatureCompensate();
            AutoFanControl();
        }

        internal virtual Boolean GetChnlAvgTemperature(out Double temperature)
        {
            temperature = 0D;

            var list = new List<Double>();
            var runtimes = 0;
            for (int count = 0; count < 8;)
            {
                temperature = Double.Parse(SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0));
                if (temperature != 0)
                {
                    count++;
                    list.Add(temperature);
                }
                runtimes++;

                if (runtimes > 20)
                {
                    Hd.SysLogger?.Invoke($"[Temperature Calibration][{DateTime.Now.ToString("G")}] reading temperature error", "Debug");
                    return false;
                }
                Thread.Sleep(5);
            }
            list.Sort();
            list.RemoveAt(0);
            list.RemoveAt(1);
            list.RemoveAt(list.Count - 2);
            list.RemoveAt(list.Count - 1);
            temperature = list.Average();
            return true;
        }

        internal virtual void AnalogTemperatureCompensate()
        {
            if (!Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate && !UsingUIParam)
            {
                return;
            }
            if (_TemperatureOffsetList.Count > 5)//记录最近5次的温度差
            {
                _TemperatureOffsetList.RemoveAt(0);
            }
            if (DateTime.Now.AddMilliseconds(0 - HoldoffBymsAnalogTemperatureCompensate) >= _LastReadTemperatureTimestamp)
            {
                _LastReadTemperatureTimestamp = DateTime.Now;
                ReadAnalogChannelTemperatures();
                if (AnalogChannelTemperatures[0] >= InvalidTemperature)//非法温度不补偿
                {
                    //Hd.SysLogger?.Invoke($"温度读取异常，当前通道温度为非法温度：{AnalogChannelTemperatures[0]}℃", "Debug");
                    return;
                }
                var currDelta = AnalogChannelTemperatures[0] - AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius / 1000D;
                _TemperatureOffsetList.Add(currDelta);
                IsTemperatureCompensated = Math.Abs(currDelta - _TemperatureOffsetList.Average()) < 5D;
                if (IsTemperatureCompensated)// |温差-最近5次的平均差| < 5，进行补偿，否则放弃补偿
                {
                    CtrlAnalogChannel_JiHe2d5G.COMPort_AnalogChannelOffsetDAC();
                }
                else
                {
                    Hd.SysLogger?.Invoke($"温度读取异常，当前通道温度：{AnalogChannelTemperatures[0]}℃", "Debug");
                }
                ////当前温度与校准时的温度差 与上次补偿时的温度差 之差 大于补偿迟滞（目前时0.5摄氏度)
                //if (Math.Abs(temperatureOffsetByCeksius - currDelta) * 1000 > thresholdBymCeksius)
                //{
                //    temperatureOffsetByCeksius = currDelta;//补偿时的温度差
                //    CtrlAnalogChannel_JiHe2d5G.COMPort_AnalogChannelSet();
                //}
            }
        }

        #region Core调用参数 Pcie、Pro、Acq、Analog温度 风扇转速

        /// <summary>
        /// 获取所有温度
        /// </summary>
        /// <returns></returns>
        public abstract List<(String Description, Double Temperature)> GetAllTemperature();

        internal Double _Ch1Temperature = 0.0;
        internal Double _Ch2Temperature = 0.0;
        internal Double _Ch3Temperature = 0.0;
        internal Double _Ch4Temperature = 0.0;

        internal Double _HddTemperature = 50.5D;

        public virtual List<(String Description, Int32 Speed)> GetAllFanSpeed()
        {
            var alltemps = new List<(String Description, Int32 Temperature)>
            {
                ("Channel Speed", ChannelFanSpeed),
                ("Pcie Speed", PcieSpeed),
                ("Cpu Speed", CpuSpeed),
            };

            return alltemps;
        }

        public virtual Double CtrollerPcieFanSpeedTempertuare { get; internal set; }

        private Int32 _ChannelFanSpeed = 400;
        public Int32 ChannelFanSpeed
        {
            get => _ChannelFanSpeed;
            set
            {
                if (_ChannelFanSpeed != value)
                {
                    Volatile.Write(ref _ChannelFanSpeed, value);
                }
            }
        }

        private Int32 _PcieSpeed = (Int32)(1000 * 1.0);
        public Int32 PcieSpeed
        {
            get => _PcieSpeed;
            set
            {
                if (_PcieSpeed != value)
                {
                    Volatile.Write(ref _PcieSpeed, value);
                }
            }
        }

        private Int32 _CpuSpeed = 1000;
        public Int32 CpuSpeed
        {
            get => _CpuSpeed;
            set
            {
                if (_CpuSpeed != value)
                {
                    Volatile.Write(ref _CpuSpeed, value);
                }
            }
        }

        public void SettingFanSpeed(Int32 speed, Int32 fanIndex, Boolean isRight = true) => CtrlFanSpeed(speed, fanIndex, isRight);

        internal abstract void SetSysTemperatures();


        #endregion Core调用参数 Pcie、Pro、Acq、Analog温度 风扇转速

        internal abstract void AutoFanControl();

        /// <summary>
        /// 获取所有板卡温度
        /// </summary>
        /// <returns></returns>
        public String GetAllBoardTemperature() => Read();
    }
}

/// <summary>
/// 调整𝐾𝑝：
/// 增大𝐾𝑝:会使系统响应更快，但也可能导致过冲和振荡。
///       减小𝐾𝑝:会减慢响应速度，但增加系统稳定性。
/// 调整𝐾𝑖：
///       增大𝐾𝑖:可以消除稳态误差，但也可能导致振荡和反应过度。
///       减小𝐾𝑖:可以降低系统的敏感性，减少振荡。
/// 调整𝐾𝑑:
///       增大𝐾𝑑:可以减少过冲，增强系统的稳定性。
///       减小 𝐾𝑑:会使微分项的影响减小，可能导致系统振荡。
/// 
/// 比例项(P)：快速响应当前误差。比例项能直接作用于输出，误差越大，比例项的修正越强。
/// 积分项(I)：消除稳态误差。积分项通过累积误差来消除长时间存在的小误差，确保系统能准确达到目标。
/// 微分项(D)：减少误差变化引起的剧烈响应。它预测误差的变化趋势，提前对变化进行调整，从而减少系统的振荡或过冲。
/// </summary>
internal class PIDContoller
{
    /// <summary>
    /// 比例系数Proportional
    /// </summary>
    public Double Kp { get; set; } = 1000;

    /// <summary>
    /// 积分系数Integral
    /// </summary>
    public Double Ki { get; set; } = 30;

    /// <summary>
    ///微分系数Derivative
    /// </summary>
    public Double Kd { get; set; } = 0;

    private Double P, I, D;

    /// <summary>
    /// 当前误差
    /// </summary>
    private Double Error { get; set; } = 0D;

    /// <summary>
    /// 上一次误差
    /// </summary>
    private Double Lasterror { get; set; } = 0D;

    //internal static PIDContoller Default = new PIDContoller();

    /// <summary>
    /// 死区，当目标值与实际值小于死区时不进行调节
    /// </summary>
    public Double Deadband { get; init; } = 1D;

    /// <summary>
    /// 上一次输出值
    /// </summary>
    public Double LastOutPut { get; internal set; }

    /// <summary>
    /// 最大输出变化幅度
    /// </summary>
    public Double OutputChangeLimit { get; init; } = 500;

    private Boolean _IsFirstCalc = true;

    public PIDContoller()
    {
        //Kd = Kd == 0 ? 3 * Kp : Kd;
    }

    /// <summary>
    /// 计算函数
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="targetValue">目标值</param>
    /// <param name="targetMinValue">目标最小值</param>
    /// <param name="targetMaxValue">目标最大值</param>
    /// <param name="integralMinValue">积分增量最小值</param>
    /// <param name="integralMaxValue">积分增量最大值</param>
    /// <param name="defaultValue">默认值（目标温度与实际温度一直时）</param>
    /// <returns>期望值</returns>
    public Double Caculate(Double currentValue, Double targetValue, Double targetMinValue, Double targetMaxValue, Double integralMinValue, Double integralMaxValue, Double defaultValue = 0)
    {
        if (targetMinValue > targetMaxValue || integralMinValue > integralMaxValue)
            throw new Exception("最大值不能小于最小值");
        Error = targetValue - currentValue;

        // 死区处理
        if (Math.Abs(Error) < Deadband)
        {
            return LastOutPut; // 在死区内保持上次输出
        }

        P = Kp * Error;
        //I += Ki * Error;
        if (Error < 0)
        {
            I -= Ki * Math.Sqrt(-Error);
        }
        else
        {
            I += Ki * Math.Sqrt(Error);
        }
        I = Math.Clamp(I, integralMinValue, integralMaxValue);
        D = Kd * (Error - Lasterror);
        Lasterror = Error;
        defaultValue = defaultValue == 0 ? (targetMaxValue + targetMinValue) / 2 : defaultValue;
        var result = defaultValue - P - I - D;
        result = Math.Clamp(result, targetMinValue, targetMaxValue);

        // 限制输出变化
        if (!_IsFirstCalc)
        {
            result = Math.Clamp(result, LastOutPut - OutputChangeLimit, LastOutPut + OutputChangeLimit);
        }
        else
            _IsFirstCalc = false;

        LastOutPut = result; // 保存当前输出
        return result;
    }

    /// <summary>
    /// 计算函数
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="targetValue">目标值</param>
    /// <param name="targetMinValue">目标最小值</param>
    /// <param name="targetMaxValue">目标最大值</param>
    /// <param name="integralMinValue">积分增量最小值</param>
    /// <param name="integralMaxValue">积分增量最大值</param>
    /// <param name="defaultValue">默认值（目标温度与实际温度一直时）</param>
    /// <returns>期望值</returns>
    public Double CaculateOriginal(Double currentValue, Double targetValue, Double targetMinValue, Double targetMaxValue, Double integralMinValue, Double integralMaxValue, Double defaultValue = 0)
    {
        if (targetMinValue > targetMaxValue || integralMinValue > integralMaxValue)
            throw new Exception("最大值不能小于最小值");
        Error = targetValue - currentValue;

        P = Kp * Error;
        I += Ki * Error;

        I = Math.Clamp(I, integralMinValue, integralMaxValue);
        D = Kd * (Error - Lasterror);
        Lasterror = Error;
        defaultValue = defaultValue == 0 ? (targetMaxValue + targetMinValue) / 2 : defaultValue;
        var result = defaultValue - P - I - D;
        result = Math.Clamp(result, targetMinValue, targetMaxValue);

        LastOutPut = result;
        return result;
    }
}

internal class HDDMonitor
{
    public static HDDMonitor? Default = new HDDMonitor();

    public HDDMonitor()
    {
        ConnectServer();
    }

    //创建 1个客户端套接字 和1个负责监听服务端请求的线程  
    private Socket? _SocketClient = null;
    private Thread? _ThreadClient = null;
    private void ConnectServer()
    {
        //定义一个套字节监听  包含3个参数(IP4寻址协议,流式连接,TCP协议)
        _SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //需要获取文本框中的IP地址
        System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("127.0.0.1");
        //将获取的ip地址和端口号绑定到网络节点endpoint上
        IPEndPoint endpoint = new IPEndPoint(ipaddress, 8888);
        //这里客户端套接字连接到网络节点(服务端)用的方法是Connect 而不是Bind
        try
        {
            _SocketClient.Connect(endpoint);
            //创建一个线程 用于监听服务端发来的消息
            _ThreadClient = new Thread(RecMsg);
            //将窗体线程设置为与后台同步
            _ThreadClient.IsBackground = true;
            //启动线程
            _ThreadClient.Start();
        }
        catch
        {
        }
    }

    /// <summary>
    /// 接收服务端发来信息的方法
    /// </summary>
    public void RecMsg()
    {
        while (true) //持续监听服务端发来的消息
        {
            try
            {
                //定义一个1M的内存缓冲区 用于临时性存储接收到的信息
                var arrRecMsg = new byte[1024 * 1024];
                //将客户端套接字接收到的数据存入内存缓冲区, 并获取其长度
                Int32? length = _SocketClient?.Receive(arrRecMsg);
                //将套接字获取到的字节数组转换为人可以看懂的字符串
                String strRecMsg = Encoding.UTF8.GetString(arrRecMsg, 0, length ?? 0);
                //将发送的信息追加到聊天内容文本框中
                var bOK = Double.TryParse(strRecMsg, out var temperature);
                temperature = bOK ? temperature : 55.5;
                temperature = temperature <= 0 ? 55.5 : temperature;
                Volatile.Write(ref SystemMonitor.Default._HddTemperature, temperature);
            }
            catch
            {
            }
            Thread.Sleep(500);
        }
    }

    /// <summary>
    /// 发送字符串信息到服务端的方法
    /// </summary>
    /// <param name="sendMsg">发送的字符串信息</param>
    public void SendMsg(String sendMsg = "HDDTemperature")
    {
        try
        {
            //将输入的内容字符串转换为机器可以识别的字节数组
            var arrclientsendmsg = Encoding.UTF8.GetBytes(sendMsg);
            //调用客户端套接字发送字节数组
            _SocketClient?.Send(arrclientsendmsg);
            //将发送的信息追加到聊天内容文本框中
        }
        catch
        {
        }
    }
}

