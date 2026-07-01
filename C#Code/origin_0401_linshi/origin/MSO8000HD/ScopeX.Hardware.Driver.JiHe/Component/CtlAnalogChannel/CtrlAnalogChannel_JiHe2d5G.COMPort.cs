using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlAnalogChannel_JiHe2d5G : McuComPortUpdater
    {

        internal static McuComPortUpdater baseObj1 = new McuComPortUpdater();
        internal static McuComPortUpdater baseObj2 = new McuComPortUpdater();
        /// <summary>
        /// 通道的控制参数包括：
        /// 1、电源开关
        /// 2、DAC的参考电压，16位的DAC，用2个字节来表示
        /// 3、衰减控制，实际上是倍率，可以是直接数买入1,5，10,20等，分别表1倍衰减（不衰减）、5倍衰减、10倍衰减、20倍衰减等。。1个字节能否足够表示（1~255）--1
        /// 4、放大控制，实际上是倍率，可以是直接数，1,10,20等，分别表示1倍放大（不放大）、10倍放大，20倍放大等。1个字节能否足够表示（1~255）---1
        /// 5、BiasDAC控制，DAC的值。16位的DAC，用2个字节来表示---2
        /// 6、OffsetDAC控制，DAC的值。16位的DAC，用2个字节来表示--2
        /// 7、DSA 粗调衰减控制。一般1个字节够表示。但8G通道的低阻需要16位来表示，建议用16位来表示。--2
        /// 8、耦合控制（DC1M，AC1M，50欧姆），用枚举来表示--1
        /// 9、带宽限制控制，可以用枚举来约定，或者以MHz为单位的频率值来表示--1/2
        /// 
        /// 从以上可以看出，如果同时控制一个通道的所有参数（不包括电源控制和参考电压的设置），需要10/11个字节。如果4个通道同时发送，业务数据需要40/44个字节。
        /// 如果通信格式为55_AA_CMD_LEN_业务数据_CRC1_CRC2_66_AA,通信格式占用8个字节。需要48/52个字节。以115200的bps来计算，按每个字符格式需要包含10位来计算，则有11.520KBytes的传输率，
        ///    传输4通道，需要4ms的理论传输时间。比现有的方案（至少大于60ms）节约不少的时间。如果加上智能的不重复发送，所需的时间更少。
        /// 其他的功能：
        /// A、读取通道温度
        /// B、读取探头连接及按钮情况
        /// C、读取探头的厂家信息
        /// D、控制探头灯
        /// E、控制探头单端或差分
        /// F、读取MCU程序的版本、型号等信息
        /// G、更新所需要的功能。通知、擦除、烧写、完成。
        /// 约定如下
        /// 1、数据格式采用little-endian的字节序.（0x1234abcd，cd-ab-34-12)
        /// 2、双向通行的包格式为:55-AA-CMD-LEN-业务数据-66-AA,格式描述中'-'是字节分割符，不是实际内容。没有特殊说明字节长度的，表示一个字节。多个字节用AABBCCDD来表示。AA、BB、CC、DD分别表示一个字节。
        ///    LEN表示业务数据的字节长度。不包含包头和包尾
        /// 3、通道指示：使用占位来表示包含通道。   1表示有该通道的数据，0表示不包含该通道的数据，从bit0到bit7,分别表示通道1,通道2...通道8.在下面的描述中用CC特指
        /// 3、请求命令（PC->MCU)
        ///  Request_MCU_Version:55-AA-CMD-00-66-AA 请求读取MCU的版本信息及信号信息。
        ///  Request_PowerCtrl:55-AA-CMD-XX-66-AA,   其中XX表示可控的8个通道电源开关，1表示PowerON，0表示PowerOff，从bit0到bit7,分别表示通道1,通道2...通道8
        ///  Request_RefVlotage:55-AA-CMD-LEN-XX-{YY-ZZ,}-66-AA，其中XX表示数据所在的通道（见3条），后续的YYZZ表示DAC的值，XX中包含多少个为1的bit位，就有多少个YYZZ数据。第一个YYZZ数据，是从bit0开始的第一个bit为1的数据。以此类推。LEN=1+bit为1个个数 * 2
        ///  Request_AnalogChannelSetting:55-AA-CMD-LEN-CC-{衰减控制-放大控制-BiasDAC[16位数]-OffsetDAC[16位数]-DSA-Coupling-BandwidthLimit[16位，以MHz为单位的数据]}-66-AA
        ///  Request_ExtTriggerSetting:
        ///  Request_ReadTemperature:
        ///  Request_ProbeState:
        ///  Request_ReadProbeFactoryInfo:
        ///  Requst_CtrlProbeLed:
        ///  Requst_CtrlProbeDifferenceOrSingle:
        ///  Requst_UpdateStart
        ///  Requst_UpdateSendData
        ///  Requst_UpdateFinished
        ///  
        ///  Readback_MCU_Version:55-AA-CMD-LEN-MAX-MID-MIN-MODEL(16字节,字符串，后续填充0)-序列号（16字节，字符串，后续填充0）-66-AA.
        ///  Readback_Temperature
        ///  Readback_ProbeState
        ///  Readback_ProbeFactoryInfo
        ///  Readback_UpdateFinished
        ///  =======有关通道的通信定义2023.05.04 zy write
        ///  CMD0x20_Request_PowerCtrl:
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道电源开关位（1表示打开，0表示关闭）
        ///  CMD0x21_Request_RefVlotage：
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道阻抗指定位（指定位为0表示高阻，1表示低阻）
        ///         +指定的第一个通道的offset 16Bit DAC数据
        ///         +指定的第二个通道的offset 16Bit DAC数据  最多8个通道
        ///         ....
        ///  CMD0x41_Request_AnalogChannelOffset: 
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道阻抗指定位（指定位为0表示高阻，1表示低阻）
        ///         +指定的第一个通道的offset 16Bit DAC数据
        ///         +指定的第二个通道的offset 16Bit DAC数据  最多8个通道
        ///         .....
        ///  CMD0x42_Request_AnalogChannelGain:
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道阻抗指定位（指定位为0表示高阻，1表示低阻）
        ///         +指定的第一个通道的16Bit 增益控制字数据
        ///         +1Byte指定的第一个通道的衰减倍率
        ///         +1Byte指定的第一个通道的放大倍率
        ///         +指定的第二个通道的16Bit 增益控制字数据
        ///         .....
        ///  CMD0x43_Request_AnalogChannelBias:
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道阻抗指定位（指定位为0表示高阻，1表示低阻）
        ///         +指定的第一个通道的16Bit DAC数据
        ///         +指定的第二个通道的16Bit DAC数据
        ///         ......
        ///  CMD0x46_Request_AnalogChannelBandwidth:       
        ///         1Byte的Channel按位指定Bit（指定位为1表示包含该通道）
        ///         +1Byte的通道阻抗指定位（指定位为0表示高阻，1表示低阻）
        ///         +2Byte指定的第一个通道带宽(MHz为单位)
        ///         +2Byte指定的第二个通道带宽(MHz为单位)
        /// </summary>
        internal enum AnalogChannelReqScopeXommands
        {
            CMD0x20_Request_PowerCtrl = 0x20,
            CMD0x21_Request_RefVlotage = 0x21,
            CMD0x22_Request_AnalogChannelSet = 0x22,
            CMD0x23_Request_ExternalChannelSet = 0x23,

            CMD0x30_Request_ReadTemperature = 0x30,

            CMD0x32_Request_CtrlProbeDifferenceOrSingle = 0x32,
            CMD0x33_Request_ReadProbeFactoryInfo = 0x33,
            CMD0x34_Request_CtrlProbeLed = 0x34,

            CMD0x40_Request_AnalogChannelCalcOffset = 0x40,
            CMD0x41_Request_AnalogChannelOffset = 0x41,
            CMD0x42_Request_AnalogChannelGain = 0x42,
            CMD0x43_Request_CtrlChannel4094 = 0x43,
            //CMD0x44_Request_AnalogChannelAttenuationRate = 0x44,
            //CMD0x45_Request_AnalogChannelAmplificationFactor = 0x45,
            CMD0x46_Request_AnalogChannelBandwidth = 0x46,
            CMD0x48_Request_CtrlOuterLed = 0x48,
            CMD0x50_Request_WriteProbeFactoryInfo = 0x50,


            CMD0x90_Request_ReadProbeCaliInfo = 0x90,
            CMD0x91_Request_ReadProbeCaliReadDnREG = 0x91,
            CMD0x92_Request_ReadProbeCaliSendDnREG = 0x92,
            CMD0x93_Request_ReadProbeCaliReadDnROM = 0x93,
            CMD0x94_Request_ReadProbeCaliSendDnROM = 0x94,
            CMD0x95_Request_ReadProbeCaliReadFnROM = 0x95,
            CMD0x96_Request_ReadProbeCaliSendFnROM = 0x96,
            CMD0x97_Request_ReadProbeCaliReadDate = 0x97,
            CMD0x98_Request_ReadProbeCaliSendDate = 0x98,
            CMD0x7D_Request_ReadProbeVersion = 0x7D,

            CMD0xE3_Request_GetDebugData = 0xe3,
            CMD0xE4_Request_Communication = 0xe4,


        }
        /*
        internal enum AnalogChannelReadbackCommands
        {
            CMD0x22_Readback_AnalogExecOKSerailNo = 0x22,

            CMD0x30_Readback_Temperature = 0x30,
            CMD0x31_Readback_ProbeStatus = 0x31,
            CMD0x33_Readback_ReadProbeInfo = 0x33,

            CMD0xC1_Readback_DebugData = 0xc1,
        }
        */
        private const uint OUTER_PANNEL_LED_COUNT = 18;

        public static void InitAll()
        {
            outerPannelLed = new();
            for (int i = 0; i < OUTER_PANNEL_LED_COUNT; i++)
            {
                outerPannelLed.Add(0);
            }
        }
        #region 业务实现
        private BitArray probeLed = new BitArray(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        private static List<byte> outerPannelLed = new();
        internal static void COMPort_CtrlHeadLight(UInt32 data)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;

            List<Byte> sendData = new List<byte>
            {
                (byte)((data)&0xFF),
            };
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x34_Request_CtrlProbeLed, sendData);

        }
        internal static bool COMPort_Check()
        {
            if (baseObj1 == null || baseObj1.Connected)
            {
                return true;
            }
            return false;
        }

        internal static void COMPort_OuterPannelLEDSet()
        {
            if (Constants.PRODUCT != ProductType.JiHe_MSO7000A)
                return;
            for (Int32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                HdMessage.AnalogOptions analogParas = Hd.UIMessage!.Analog![channelIndex];
                OuterPannelLEDType ledType;
                bool ledEnable = true;
                switch (channelIndex)
                {
                    case 0:
                        ledType = OuterPannelLEDType.CH1_50;
                        break;
                    case 1:
                        ledType = OuterPannelLEDType.CH2_50;
                        break;
                    case 2:
                        ledType = OuterPannelLEDType.CH3_50;
                        break;
                    case 3:
                        ledType = OuterPannelLEDType.CH4_50;
                        break;
                    default:
                        return;
                }
                switch (analogParas.Coupling)
                {
                    case AnaChnlCoupling.DC1M:
                        COMPort_OuterLEDSet(ledType, false);
                        ledType += 1;
                        break;
                    case AnaChnlCoupling.AC1M:
                        COMPort_OuterLEDSet(ledType, false);
                        ledType += 1;
                        break;
                    case AnaChnlCoupling.DC50:
                        COMPort_OuterLEDSet(ledType + 1, false);
                        break;
                    case AnaChnlCoupling.Gnd:
                        ledEnable = false;
                        break;
                    default:
                        continue;
                }

                COMPort_OuterLEDSet(ledType, ledEnable, Constants.OUTER_PANNEL_LED_BLUE, Constants.OUTER_PANNEL_LED_GREEN, Constants.OUTER_PANNEL_LED_RED);
            }
        }
        internal static void COMPort_OuterLEDSet(OuterPannelLEDType pannelLED, bool enable, byte BlueOrLight = 0xff, byte Green = 0xff, byte Red = 0xff)
        {
            if (Constants.PRODUCT != ProductType.JiHe_MSO7000A)
            {
                return;
            }
            if (outerPannelLed.Count() == 0)
            {
                InitAll();
            }
            var ledIndex = 0;
            if (!enable)
            {
                Red = 0;
                BlueOrLight = 0;
                Green = 0;
            }
            switch (pannelLED)
            {
                case OuterPannelLEDType.LAN:
                    ledIndex = 0;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    outerPannelLed[ledIndex + 1] = Green;
                    outerPannelLed[ledIndex + 2] = Red;
                    break;
                case OuterPannelLEDType.ACQ:
                    ledIndex = 3;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    outerPannelLed[ledIndex + 1] = Green;
                    outerPannelLed[ledIndex + 2] = Red;
                    break;
                case OuterPannelLEDType.RUN:
                    ledIndex = 6;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    outerPannelLed[ledIndex + 1] = Green;
                    outerPannelLed[ledIndex + 2] = Red;
                    break;
                case OuterPannelLEDType.POWER:
                    ledIndex = 9;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH1_50:
                    ledIndex = 10;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH1_1M:
                    ledIndex = 11;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH2_50:
                    ledIndex = 12;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH2_1M:
                    ledIndex = 13;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH3_50:
                    ledIndex = 14;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH3_1M:
                    ledIndex = 15;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH4_50:
                    ledIndex = 16;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                case OuterPannelLEDType.CH4_1M:
                    ledIndex = 17;
                    outerPannelLed[ledIndex] = BlueOrLight;
                    break;
                default:
                    return;
            }
            COMPort_OuterLEDSetAll(outerPannelLed);
        }
        internal static void COMPort_OuterLEDSetAll(List<byte> datas)
        {
            if (datas == null || datas.Count == 0)
            {
                return;
            }
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x48_Request_CtrlOuterLed, datas);
        }
        public static Boolean bTestComm = false;
        public static Boolean bAnalogChannelComm = false;
        public static List<List<byte>> lastSendData = new List<List<byte>>();
        public static byte[] last409 = new byte[8] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
        public static ushort CmdSerailNo = 0;

        /// <summary>
        /// 温补单独发送（不组包）
        /// </summary>
        internal static void COMPort_AnalogChannelOffsetDAC()
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 0b1111;
            List<Byte> sendData = new List<byte>
            {
                channelBits
            };
            //sendData.Add(channelBits);
            for (var channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                var analogParas = Hd.UIMessage!.Analog[channelIndex];
                int Impedance_H_is0 = analogParas.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                (int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
                int yScaleIndex = bestScaleIndexFpgaFine.ScaleIndex;
                Int32 pos0Div = (Int32)AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
                Double pos3Div_P = AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_3Div;
                Double pos3div_N = AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_N3Div;
                /******************************温补***********************/
                var offsetByTemprtrBymV = 0D;
                if (Hd.Calibration.IsCaliTemperatureOffset || !Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate || !SystemMonitor.Default.IsTemperatureCompensated)
                {
                    offsetByTemprtrBymV = 0D;
                }
                else
                {
                    AnaChnlCoupling anaChnlCoupling = AnaChnlCoupling.DC1M;

                    if (analogParas.Coupling == AnaChnlCoupling.DC50)
                    {
                        anaChnlCoupling = AnaChnlCoupling.DC50;
                    }
                    var coeff = Hd.Calibration.TemperatureCoeffcientTable.Count == 0 ? 0 : Hd.Calibration.TemperatureCoeffcientTable[(ChannelId)channelIndex][anaChnlCoupling].Coeffcient;
                    if (SystemMonitor.Default.AnalogChannelTemperatures[0] > SystemMonitor.InvalidTemperature)
                    {
                        offsetByTemprtrBymV = 0;
                    }
                    else
                    {
                        double TemperatureOffsetByCeksius = SystemMonitor.Default.AnalogChannelTemperatures[0] - AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius / 1000D;
                        offsetByTemprtrBymV = coeff * TemperatureOffsetByCeksius;
                    }
                }
                /******************************温补***********************/
                Int32 posPosition = (Int32)(Constants.SAMPS_PER_YDIV * (analogParas.Position - offsetByTemprtrBymV) * 1000 / bestScaleIndexFpgaFine.ScaleValueByuV);
                Int32 posPositionCtrl = 0;
                if (posPosition > 0)
                {
                    posPositionCtrl = (Int32)(posPosition * pos3Div_P / (Constants.SAMPS_PER_YDIV * 3));
                }
                else
                {
                    posPositionCtrl = (Int32)(posPosition * pos3div_N / (Constants.SAMPS_PER_YDIV * 3));
                }

                if (analogParas.IsInverted)
                    posPositionCtrl = pos0Div - posPositionCtrl;
                else
                    posPositionCtrl = pos0Div + posPositionCtrl;
                sendData.Add(1);
                sendData.Add(0x0);
                sendData.Add((byte)(posPositionCtrl & 0xFF));
                sendData.Add((byte)((posPositionCtrl >> 8) & 0xFF));
            }

            baseObj1.ClearSpecialReceiveQueue((byte)AnalogChannelReqScopeXommands.CMD0x40_Request_AnalogChannelCalcOffset);
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x40_Request_AnalogChannelCalcOffset, sendData);
        }

        internal static void COMPort_AnalogChannelSet()
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 0b1111;
            List<Byte> sendData = new List<byte>
            {
                (byte)(Hd.CurrDebugVarints.bEnable_OpenCrystal ? 0x01 : 0x00),
                channelBits
            };

            List<ChannelId> resetResetHighVoltageWarnings = new List<ChannelId>();
            for (Int32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                HdMessage.AnalogOptions analogParas = Hd.UIMessage.Analog[channelIndex];
                int Impedance_H_is0 = analogParas.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                (int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
                int yScaleIndex = bestScaleIndexFpgaFine.ScaleIndex;
                AnalogChannelItem_Base chnlParams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelIndex, Impedance_H_is0 == 0, (uint)(bestScaleIndexFpgaFine.ScaleValueByuV / 1000)))!.Value;

                #region 是否过FG10(0-不过，1-过)
                //当前垂直档位<=20mv时为高增益(过FG),其他为低增益(不过FG)
                if (yScaleIndex <= (int)AnaChnlScaleIndex.Lv20m)
                    sendData.Add(1);
                else
                    sendData.Add(0);
                #endregion

                #region 放大倍数
                //说明：MSO8000HD有两个DSA，每个DSA的寄存器设置范围为[0,127]
                int GainCtr = Math.Min((Math.Max(chnlParams.Gain, 0)), 254);
                var attPair = Get5GDsaCtrlWord(Impedance_H_is0 == 0, (AnaChnlScaleIndex)yScaleIndex, GainCtr);

                sendData.Add((byte)attPair.Dsa1);
                sendData.Add((byte)attPair.Dsa2);
                #endregion 放大倍数

                #region 衰减倍数           
                HdMessage.AnalogOptions phychannel = Hd.UIMessage!.Analog![channelIndex];
                bool result;

                // 默认使用高阻的幅度衰减表，如果是低阻，显式地进行更换
                Dictionary<AnaChnlScaleIndex, UInt32> attTable = (phychannel.Coupling == AnaChnlCoupling.DC50) ? newAttTableLz : newAttTableHz;
                result = attTable.TryGetValue((AnaChnlScaleIndex)bestScaleIndexFpgaFine.ScaleIndex, out UInt32 att);
                if (!result)
                    return;
                sendData.Add((byte)att);
                #endregion 衰减倍数

                #region 耦合 阻抗
                //DC-0/AC-1
                byte coupling = 0;
                //高阻-0/低阻-1
                byte impedance = 0;
                switch (phychannel.Coupling)
                {
                    case AnaChnlCoupling.DC1M:
                        coupling = 0;
                        impedance = 0;
                        break;
                    case AnaChnlCoupling.AC1M:
                        coupling = 1;
                        impedance = 0;
                        break;
                    case AnaChnlCoupling.DC50:
                        coupling = 0;
                        impedance = 1;
                        resetResetHighVoltageWarnings.Add((ChannelId)channelIndex);
                        break;
                    case AnaChnlCoupling.Gnd:
                        coupling = 1;
                        impedance = 0;
                        break;
                    default:
                        return;
                }
                sendData.Add(coupling);
                sendData.Add(impedance);
                #endregion 耦合 阻抗

                #region 偏置DAC
                int Pre0Div = chnlParams.Bias;
                uint moveFactorScaleMv = GetDACScaleMv((AnaChnlScaleIndex)yScaleIndex, attTable);
                double MoveFactor = ProductDataTranslate_MSO8000X.GetChnlParamsItem(new((ChannelId)channelIndex, Impedance_H_is0 == 0, moveFactorScaleMv))!
                    .Value.Bias_Pos3Div / 100_000_000D;  //Bias_Pos3Div表示：斜率放大100_000_000倍的值。该值的大小可以理解为最大输入电平的值(以uV为单位)
                MoveFactor = analogParas.Bias >= 0 ? MoveFactor : ProductDataTranslate_MSO8000X.GetChnlParamsItem(new((ChannelId)channelIndex, Impedance_H_is0 == 0, moveFactorScaleMv))!
    .Value.Bias_Neg3Div / 100_000_000D;
                Int32 Bias = Pre0Div - (Int32)((double)analogParas.Bias * MoveFactor);
                sendData.Add((byte)(Bias & 0xFF));
                sendData.Add((byte)((Bias >> 8) & 0xFF));
                #endregion 偏置DAC

                #region offset DAC
                Int32 pos0Div = (Int32)AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
                Double pos3Div_P = AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_3Div;
                Double pos3div_N = AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_N3Div;

                #region 自校准+温补
                //Int32 pos0Div = (Int32)AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
                //Double pos3Div_P = (Int32)AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_3Div;
                //Double pos3div_N = AutoCaliParams.Default![(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_N3Div;
                ///******************************温补***********************/
                //var offsetByTemprtrBymV = 0D;
                //if (Hd.Calibration.IsCaliTemperatureOffset || !Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate || !SysMonitor.Default.IsTemperatureCompensated)
                //{
                //    offsetByTemprtrBymV = 0D;
                //}
                //else
                //{
                //    AnaChnlCoupling anaChnlCoupling = AnaChnlCoupling.DC1M;

                //    if (analogParas.Coupling == AnaChnlCoupling.DC50)
                //    {
                //        anaChnlCoupling = AnaChnlCoupling.DC50;
                //    }
                //    var coeff = Hd.Calibration.TemperatureCoeffcientTable.Count == 0 ? 0 : Hd.Calibration.TemperatureCoeffcientTable[(ChannelId)channelIndex][anaChnlCoupling].Coeffcient;
                //    if (SysMonitor.Default.AnalogChannelTemperatures[0] > SysMonitor.InvalidTemperature)
                //    {
                //        offsetByTemprtrBymV = 0;
                //    }
                //    else
                //    {
                //        double TemperatureOffsetByCeksius = SysMonitor.Default.AnalogChannelTemperatures[0] - AutoCaliParams.Default!.TemperatureAtCaliBaseline_mCelsius / 1000D;
                //        offsetByTemprtrBymV = coeff * TemperatureOffsetByCeksius;
                //    }
                //}
                ///******************************温补***********************/
                #endregion
                Int32 posPosition = (Int32)(Constants.SAMPS_PER_YDIV * (analogParas.Position) * 1000 / bestScaleIndexFpgaFine.ScaleValueByuV);
                Int32 posPositionCtrl = 0;
                if (posPosition > 0)
                {
                    posPositionCtrl = (Int32)(posPosition * pos3Div_P / (Constants.SAMPS_PER_YDIV * 3));
                }
                else
                {
                    posPositionCtrl = (Int32)(posPosition * pos3div_N / (Constants.SAMPS_PER_YDIV * 3));
                }

                //反向
                if (analogParas.IsInverted)
                    posPositionCtrl = pos0Div - posPositionCtrl;
                else
                    posPositionCtrl = pos0Div + posPositionCtrl;

                sendData.Add((byte)(posPositionCtrl & 0xFF));
                sendData.Add((byte)((posPositionCtrl >> 8) & 0xFF));
                //CtrlOffset(false); //test
                #endregion offset DAC

                #region 带宽
                // 带宽
                UInt16 bandwidthByMHz;
                switch (phychannel.Bandwidth)
                {
                    case 0: // LZ HZ FULL
                        bandwidthByMHz = 0xFFFF;
                        break;
                    case 1: // LZ 500M
                        bandwidthByMHz = 500;
                        break;
                    case 2: // LZ 200M
                        bandwidthByMHz = 200;
                        break;
                    case 3: // LZ HZ 20M
                        bandwidthByMHz = 20;
                        break;
                    default:
                        return;
                }
                sendData.Add((byte)(bandwidthByMHz & 0xFF));
                sendData.Add((byte)((bandwidthByMHz >> 8) & 0xFF));
                #endregion 带宽

            }

            Hd.ResetHighVoltageWarning(resetResetHighVoltageWarnings);
            UInt32 crcCode = CRC32.GetCRC32Code(sendData);
            sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));

            ushort sum = 0;
            for (int i = 0; i < sendData.Count - 4; i++)
                sum += sendData[i];
            sendData.Add((byte)((sum & 0x00_ff) >> 0));
            sendData.Add((byte)((sum & 0xff_00) >> 8));

            CmdSerailNo++;
            sendData.Add((byte)((CmdSerailNo & 0x00_ff) >> 0));
            sendData.Add((byte)((CmdSerailNo & 0xff_00) >> 8));
            baseObj1.ClearSpecialReceiveQueue((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet);
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
            #region 强制握手
            if (Constants.PRODUCT != ProductType.JiHe_MSO7000A)
            {
                bool execOK = false;
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, 60, true, out var readbackResult_execOK))
                {
                    if (readbackResult_execOK.Value.dataLength >= 2)
                    {
                        ushort readbackSerialNo = readbackResult_execOK.Value.Data[1];
                        readbackSerialNo <<= 8;
                        readbackSerialNo |= readbackResult_execOK.Value.Data[0];
                        if (readbackSerialNo == CmdSerailNo || readbackSerialNo == CmdSerailNo - 1)
                        {
                            execOK = true;
                        }
                    }
                }
                else
                    ;
                if (!execOK)
                {
                    baseObj1.SendData(true, (int)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);
                    baseObj1.SendData(true, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
                }
            }
            #endregion

            if (CmdSerailNo == ushort.MaxValue)
                CmdSerailNo = 0;

            #region 测试代码
            if (bAnalogChannelComm)
            {

                //通道控制测试
                Thread.Sleep(50);
                List<byte> readBackSendData = new List<byte> { 0 };
                baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, readBackSendData);
                string errorStr = "";
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, 1000, true, out var readbackResult))
                {
                    if (readbackResult.Value.dataLength >= 41)
                    {
                        for (int i = 0; i < 41; i++)
                        {
                            if (readbackResult.Value.Data[i] != sendData[i])
                            {
                                errorStr = errorStr + $"at index={i},sourceData={sendData[i]} != readbackData={readbackResult.Value.Data[i]}";
                            }

                        }
                    }
                    else
                        ;
                }
                else
                    ;
                if (errorStr.Trim() != "")
                    Hd.SysLogger?.Invoke($"AnalogChannel Ctrl Error:{errorStr}", "Warnning");
                else
                    ;

                if (lastSendData.Count > 1)
                {
                    lastSendData.RemoveAt(0);
                }
                lastSendData.Add(sendData);



                List<byte> readBackSendData2 = new List<byte> { 2 };
                baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, readBackSendData2);
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, 1000, true, out var readbackResult2))
                {
                    if (readbackResult2.Value.dataLength >= 8)
                    {
                        if (last409[0] == 0xff)
                        {
                            for (int i = 0; i < 8; i++)
                                last409[i] = readbackResult2.Value.Data[i];
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            if (last409[i] != readbackResult2.Value.Data[i])
                                ;
                        }
                    }
                    else
                        ;
                }
            }
            if (bTestComm)
            {
                //单独通信测试
                List<byte> testData = new List<byte>();
                for (byte i = 0; i < 128; i++)
                    testData.Add((byte)(i & 0xFF));
                for (int testTimes = 0; testTimes < 1000; testTimes++)
                {
                    baseObj1.SendData(true, (int)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication, testData);
                    if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication, 1000, true, out var readbackResult_test))
                    {
                        if (readbackResult_test.Value.dataLength >= 128)
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (readbackResult_test.Value.Data[i] != testData[i])
                                    ;
                            }
                        }
                    }
                }
            }
            #endregion

        }

        internal static void COMPort_ExitTrigChannelSet()
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 1; // 外触发通道
            List<Byte> sendData = new List<byte>
            {
                channelBits
            };
            sendData.Add(0);
            HdMessage.TriggerOptions? triggerParas = Hd.UIMessage.Trigger;
            if (triggerParas == null)
            {
                return;
            }
            var triggerEdge = triggerParas.Edge;
            if (triggerEdge == null)
            {
                return; // ljw 23.6.28 先仅支持边沿触发
            }

            byte extAtt = (byte)(triggerParas.EnableExtAtten ? 5 : 1);//Ext5 \ Ext
            sendData.Add(extAtt);
            byte extCoupling = (byte)(triggerEdge.Coupling - TriggerCoupling.DC);
            if (triggerEdge.Coupling == TriggerCoupling.NR)
            {
                extCoupling = 0;
            }
            sendData.Add(extCoupling);
            byte extImp = (byte)(triggerEdge.Impedance - TriggerImpedance.High1M);
            sendData.Add(extImp);

            UInt32 trigsource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();

            /// 输入粗调衰减 暂时不用
            //int attenu = trigsource == (UInt32)ChannelId.Ext ? 2 : 10;

            ///不区分 Ext和Ext5
            var offsetByuV = ((ChannelId)trigsource, Hd.UIMessage!.Trigger!.Edge!.Impedance, Hd.UIMessage!.Trigger!.Edge!.Coupling) switch
            {
                //Ext
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.DC) => MiscData.Default[(int)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.DC) => MiscData.Default[(int)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.AC) => MiscData.Default[(int)MiscDefine.ExtTrigger_HighImp_AC_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.AC) => MiscData.Default[(int)MiscDefine.ExtTrigger_LowImp_AC_ZeroDelta_uV],


                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.LFR) => MiscData.Default[(int)MiscDefine.ExtTrigger_HighImp_LFR_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.LFR) => MiscData.Default[(int)MiscDefine.ExtTrigger_LowImp_LFR_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.HFR) => MiscData.Default[(int)MiscDefine.ExtTrigger_HighImp_HFR_ZeroDelta_uV],
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.HFR) => MiscData.Default[(int)MiscDefine.ExtTrigger_LowImp_HFR_ZeroDelta_uV],

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.NR) => MiscData.Default[(int)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV] - 88000,//chenyan:噪声抑制未校准，特殊处理，用实测值，下同
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.NR) => MiscData.Default[(int)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV] - 880000,


                //Ext5  使用Ext校准值
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.DC) => MiscData.Default[(int)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.DC) => MiscData.Default[(int)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.AC) => MiscData.Default[(int)MiscDefine.Ext5Trigger_HighImp_AC_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.AC) => MiscData.Default[(int)MiscDefine.Ext5Trigger_LowImp_AC_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.LFR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_HighImp_LFR_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.LFR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_LowImp_LFR_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.HFR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_HighImp_HFR_ZeroDelta_uV],
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.HFR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_LowImp_HFR_ZeroDelta_uV],

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.NR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV] - 90000 * 5,
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.NR) => MiscData.Default[(int)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV] - 90000 * 5,

                (_, _, _) => 0,
            };

            var offset = offsetByuV / 1e3; //uV to mV
            var ratio = (trigsource == (UInt32)ChannelId.Ext) ? 1D : 5D;
            var smtGate_HFRorOther = (triggerEdge.Coupling == TriggerCoupling.HFR) ? 70D : 55D;
            var smtGateBymV = (triggerEdge.Coupling == TriggerCoupling.NR) ? 280 * ratio : smtGate_HFRorOther * ratio;
            if (!Hd.Calibration.IsCaliExttrigger)
            {
                {
                    if (triggerEdge.Slope == EdgeSlope.Rise)
                    {
                        offset -= smtGateBymV / 4;
                    }
                    else if (triggerEdge.Slope == EdgeSlope.Fall)
                    {
                        offset += smtGateBymV / 4;
                    }
                    else
                    {
                        //offset += smtGateBymV * 0.375;
                    }
                }
            }
            else
            {
                offset = 0;
            }
            var triggerPosBymV = offset + triggerEdge.Position * 0.6D;
            //归一化到0xFFFF
            int dacValue = (int)((Constants.EXT_TRIGGER_MAX_MV * ratio - triggerPosBymV) / ((Constants.EXT_TRIGGER_MAX_MV - Constants.EXT_TRIGGER_MIN_MV) * ratio) * 0xFFFF);
            dacValue = Math.Clamp(dacValue, 0, 0xFFFF); //limit（v, min,max）

            sendData.Add((byte)(dacValue & 0xFF));
            sendData.Add((byte)((dacValue >> 8) & 0xFF));

            //ljw 23.11.1
            //ljw 归一化到0xFFFF
            //uint positionDac = (uint)((triggerEdge.Position + 1000) / 2000 * 0xFFFF);
            uint positionDac = triggerEdge.Coupling == TriggerCoupling.NR ? Constants.EXT_TRIGGER_NR_POSITION_DEFAULT : Constants.EXT_TRIGGER_ACDC_POSITION_DEFAULT;

            sendData.Add((byte)(positionDac & 0xFF));
            sendData.Add((byte)((positionDac >> 8) & 0xFF));
            //var position = 0x7D00;
            //sendData.Add((byte)(position & 0xFF));
            //sendData.Add((byte)((position >> 8) & 0xFF));

            //todo 高频抑制开关 core层没有对应,默认关
            sendData.Add(0);
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x23_Request_ExternalChannelSet, sendData);
        }
        internal static void COMPort_SetRefVoltage(ushort refVoltage_DacValue)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 0;
            List<Byte> sendData = new List<byte>
            {
                channelBits
            };
            for (Int32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                channelBits |= (byte)(1 << channelIndex);
                sendData.AddRange(BitConverter.GetBytes(refVoltage_DacValue));
            }
            sendData[0] = channelBits;
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x21_Request_RefVlotage, sendData);
        }

        internal static void COMPort_SendCmdToDSA(UInt32 gainCtr, ChannelId channelIndex)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            UInt32 EnableWord = 0x0001;
            EnableWord = EnableWord << (int)channelIndex;

            List<Byte> sendData = new List<byte>
            {
                (byte)EnableWord,
                (byte)(gainCtr & 0xFF),
            };

            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x42_Request_AnalogChannelGain, sendData);
        }
        internal static void COMPort_SendDataTo5675(short DAC_Select, UInt32 Port, uint dacindexEx, UInt32 Data)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            UInt32 ConfigWord = 0x1030_0000;
            ConfigWord |= dacindexEx << 24;
            ConfigWord |= Port << 16;
            ConfigWord |= Data;

            List<Byte> sendData = new List<byte>
            {
                (byte)DAC_Select,

                (byte)((ConfigWord >> 24) & 0xFF),
                (byte)((ConfigWord >> 16) & 0xFF),
                (byte)((ConfigWord >> 8) & 0xFF),
                (byte)(ConfigWord & 0xFF),
            };


            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x41_Request_AnalogChannelOffset, sendData);
        }

        internal static void COMPort_Ctrl4094(UInt32 low32word, UInt32 high8word)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<Byte> sendData = new List<byte>
            {
                (byte)(high8word >> 8),
                (byte)(high8word & 0xFF),
                (byte)((low32word >> 24) & 0xFF),
                (byte)((low32word >> 16) & 0xFF),
                (byte)((low32word >> 8) & 0xFF),
                (byte)(low32word & 0xFF),
            };
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x43_Request_CtrlChannel4094, sendData);
        }
        internal static void COMPort_PowerCtrl(bool bPowerOn)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<Byte> sendData = new List<byte>
            {
                (byte)(bPowerOn ? 1 : 0)
            };
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x20_Request_PowerCtrl, sendData);
        }

        internal static void COMPort_RequestFactoryInfo(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, 0x33, sendData);
            //Thread.Sleep(50);
        }
        internal static void COMPort_RequestProbeFreqData(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, 0x9E, sendData);
            //Thread.Sleep(50);
        }


        /// <summary>
        /// 读取探头校准信息
        /// </summary>
        /// <param name="channelID"></param>
        internal static void COMPort_RequestProbeCaliInfo(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, 0x90, sendData);
            //Thread.Sleep(50);
        }

        internal static void COMPort_RequestProbeVersion(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, 0x7D, sendData);
            //Thread.Sleep(50);
        }

        internal static void COMPort_RequestProbeDnRegSet(int channelID, UInt16 dacA, UInt16 dacB)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;

            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID),
                (byte)(dacA & 0xFF),
                (byte)((dacA & 0xFF00) >> 8),
                (byte)(dacB & 0xFF),
                (byte)((dacB & 0xFF00) >> 8),
            };
            baseObj1.SendData(false, (byte)AnalogChannelReqScopeXommands.CMD0x92_Request_ReadProbeCaliSendDnREG, sendData);
        }
        internal static byte[] COMPort_RequestProbeDnRegGet(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return new byte[0];

            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, (byte)AnalogChannelReqScopeXommands.CMD0x91_Request_ReadProbeCaliReadDnREG, sendData);
            return baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x91_Request_ReadProbeCaliReadDnREG, 1000, true, out var result)
                ? result!.Value.Data.ToArray()
                : (new byte[0]);
        }

        internal static void COMPort_RequestProbeDnRomSet(int channelID, UInt16 dacA, UInt16 dacB)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;

            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID),
                (byte)(dacA & 0xFF),
                (byte)((dacA & 0xFF00) >> 8),
                (byte)(dacB & 0xFF),
                (byte)((dacB & 0xFF00) >> 8),
            };
            baseObj1.SendData(false, (byte)AnalogChannelReqScopeXommands.CMD0x94_Request_ReadProbeCaliSendDnROM, sendData);
        }
        internal static byte[] COMPort_RequestProbeDnRomGet(int channelID)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return new byte[0];

            List<byte> sendData = new List<byte>
            {
                (byte)(1 << channelID)
            };
            baseObj1.SendData(false, (byte)AnalogChannelReqScopeXommands.CMD0x93_Request_ReadProbeCaliReadDnROM, sendData);
            return baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x93_Request_ReadProbeCaliReadDnROM, 5000, true, out var result)
                ? result!.Value.Data.ToArray()
                : (new byte[0]);
        }

        internal static void COMPort_WriteFactoryInfo(List<byte> sendData)
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            baseObj1.SendData(false, (byte)AnalogChannelReqScopeXommands.CMD0x50_Request_WriteProbeFactoryInfo, sendData);
        }

        #endregion
    }
    internal static class CRC32
    {
        private static UInt32[]? Crc32Table;
        //生成CRC32码表
        public static void GetCRC32Table()
        {
            UInt32 Crc;
            Crc32Table = new UInt32[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                Crc = (UInt32)i;
                for (j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }
        }

        //获取字符串的CRC32校验值
        public static UInt32 GetCRC32Code(List<byte> buffer)
        {
            if (Crc32Table == null)
                GetCRC32Table();

            UInt32 value = 0xffffffff;
            int len = buffer.Count;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }
    }
}
