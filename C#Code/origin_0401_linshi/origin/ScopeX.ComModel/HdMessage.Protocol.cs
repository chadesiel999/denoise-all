using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public partial record HdMessage
    {
        #region 解码触发结构参数

        public record TrigDecoderOptions(ChannelId id, SerialProtocolType ProtocolType)
        {
            public ITrigDecoderConditionsOptions? DecoderConditionsOptions
            {
                get;
                init;
            }
            public IDecoderOptions? ProtocolOptions
            {
                get;
                init;
            }
        }
        public interface ITrigDecoderConditionsOptions
        {
        }
        public record TrigCloseConditionsOptions() : ITrigDecoderConditionsOptions
        {
        }


        public record TrigNRZConditionOptions(ProtocolNRZ.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolNRZ.DataRelation Relation { get; init; }
            public UInt32 Data { get; init; }
        }
        public record TrigFlexRayConditionOptions(ProtocolFlexRay.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolFlexRay.Realtion Relation
            {
                get;
                init;
            }
            public UInt16 IndicatorData
            {
                get;
                init;
            }
            public UInt16 ID
            {
                get;
                init;
            }
            public UInt16 Payload
            {
                get;
                init;
            }
            public UInt16 HeaderCRC
            {
                get;
                init;
            }
            public Byte ByteCount
            {
                get; init;
            }
            public Byte[]? Data
            {
                get;
                init;
            }

            public Boolean HasDataOffset
            {
                get;
                init;
            }

            public UInt16 ByteOffset
            {
                get;
                init;
            }

            public Byte CycleData
            {
                get;
                init;
            }
            
            public ProtocolFlexRay.Indicator Indicator
            {
                get;
                init;
            }

            public ProtocolFlexRay.FrameError FrameError
            {
                get; init;
            }
            public ProtocolFlexRay.FrameTail FrameTail
            {
                get; init;
            }
        }
        public record TrigEthernetConditionsOptions(ProtocolEthernet.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public Byte[] Data
            {
                get;
                init;
            }
            public Int32 DataByteLength
            {
                get;
                init;
            }
            public Int32 DataOffset
            {
                get;
                init;
            }
            public Byte[]? SrcMAC
            {
                get;
                init;
            }
            public Byte[]? DestMAC
            {
                get;
                init;
            }

            public Byte[]? MACLengthOrType
            {
                get;
                init;
            }
            public Byte[]? QTagInfo
            {
                get;
                init;
            }

            public ProtocolEthernet.DataRelation Relation
            {
                get;
                init;
            }
        }


        public record TrigLINConditionsOptions(ProtocolLIN.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolLIN.DataRelation DataRelation
            {
                get;
                init;
            }

            public Byte ID
            {
                get;
                init;
            }
            public Int64 Data
            {
                get; init;
            }

            public Byte ByteCount
            {
                get; init;
            }
        }
        public record TrigRS232ConditionsOptions(ProtocolRS232.Conditions Conditions) : ITrigDecoderConditionsOptions
        { 

            public PulseCondition Compare
            {
                get;
                init;
            }
            public UInt32 DataLength
            {
                get;
                init;
            }

            public UInt64 Data
            {
                get;
                init;
            }
            public Char EOPChar
            {
                get;
                init;
            }
        }
        public record TrigPCIeConditionsOptions(ProtocolPCIe.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolPCIe.TLPType TLPType
            {
                get;
                init;
            }
            public UInt16 SeqID
            {
                get;
                init;
            }


            public Byte TCData
            {
                get;
                init;
            }


            public Byte ATData
            {
                get;
                init;
            }


            public Byte TagData
            {
                get;
                init;
            }

            public UInt16 ReqIDData
            {
                get;
                init;
            }


            public Byte MsgCodeData
            {
                get;
                init;
            }

            public Byte DataLenght
            {
                get;
                init;
            }
            public Int64 Data
            {
                get;
                init;
            }

            public Int64 AddressData
            {
                get;
                init;
            }

            public ProtocolPCIe.DataRelation DataRelation
            {
                get; init;
            }
        }
        public record TrigMILConditionsOptions(ProtocolMIL.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public Int32 Data
            {
                get;
                init;
            }
            public Int32 Pairty
            {
                get;
                init;
            }
            public Int32 RTA
            {
                get;
                init;
            }

            public ProtocolMIL.ErrorType ErrorType
            {
                get; init;
            }
        }
        public record TrigI2SConditionsOptions(ProtocolAudioBus.Condition Condition) : ITrigDecoderConditionsOptions
        {

            public ProtocolAudioBus.SoundChannel SoundChannel
            {
                get; init;
            }

            public UInt32 Data
            {
                get; init;
            }

            public Byte ByteCount
            {
                get; init;
            }

            public ProtocolAudioBus.DataRelation DataRelation
            {
                get; init;
            }

            /// <summary>
            /// TDM时，选择的条件
            /// </summary>
            public ProtocolAudioBus.Condition4TDM ConditionTDM
            {
                get; init;
            }

            /// <summary>
            /// TDM时，选择的音频通道
            /// </summary>
            public UInt32 TDMChannelID
            {
                get; init;
            }
        }

        // SPMI触发条件
        public record TrigSPMIConditionsOptions(ProtocolSPMI.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }

        //I3C触发条件
        public record TrigI3CConditionsOptions(ProtocolI3C.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }
        //CXPI触发条件
        public record TrigCXPIConditionsOptions(ProtocolCXPI.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }
        public record TrigManchesterConditionOptions(ProtocolManchester.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }
        //Mlt3触发条件
        public record TrigMlt3ConditionOptions(ProtocolMlt3.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }
        //8b10b触发条件
        public record Trig8B10BConditionOptions(Protocol8B10B.Condition Condition) : ITrigDecoderConditionsOptions
        {

        }
        public record TrigI2CConditionsOptions(ProtocolI2C.Condition Condition) : ITrigDecoderConditionsOptions
        {
            /// <summary>
            /// 数据方向
            /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
            /// </summary>
            public ProtocolI2C.DataDirection Direction
            {
                get;
                init;
            }

            /// <summary>
            /// 地址值
            /// </summary>
            public UInt16 AddressData
            {
                get;
                init;
            }

            /// <summary>
            /// 比较方式
            /// </summary>
            public ProtocolI2C.DataRelation Relation
            {
                get;
                init;
            }

            /// <summary>
            /// 字节数
            /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
            /// </summary>
            public UInt32 DataBytesCount
            {
                get;
                init;
            }

            /// <summary>
            /// 字节号，从0开始
            /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
            /// </summary>
            public Int32 DataByteIndex
            {
                get;
                init;
            }

            /// <summary>
            /// 数据
            /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
            /// </summary>
            public Int64 Data
            {
                get;
                init;
            }
        }
        public record TrigSATAConditionsOptions(ProtocolSATA.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public Int64 Data
            {
                get; init;
            }
            public ProtocolSATA.DataRelation Relation
            {
                get; init;
            }
            public Byte DataCount { get; init; }
            public ProtocolSATA.FISTypeFlag FISType
            {
                get; init;
            }
        }
        public record TrigSPIConditionsOptions(ComModel.ProtocolSPI.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ComModel.ProtocolSPI.DataTriggerSource DataSource
            {
                get;
                init;
            }


            public Int32 FrameCount
            {
                get;
                init;
            }
            public Int64 FrameData
            {
                get;
                init;
            }
            public Int64 FrameDataHigh
            {
                get;
                init;
            }
            public UInt64 DataBitWidth
            {
                get;
                init;
            }
        }

        public record TrigARINC429ConditionsOptions(ProtocolARINC429.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public UInt32 SSM
            {
                get;
                init;
            }

            public UInt32 SDI
            {
                get;
                init;
            }

            public UInt32 Label
            {
                get;
                init;
            }

            public UInt32 Data
            {
                get;
                init;
            }

            public ProtocolARINC429.ErrorType ErrorType
            {
                get;
                init;
            }

            public ProtocolARINC429.DataRelation DataRelation
            {
                get;
                init;
            }
        }

        public record TrigCANFDConditionsOptions(ProtocolCANFD.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolCANFD.FrameType FrameType
            {
                get;
                init;
            }

            public ProtocolCANFD.IDStandard IDStandard
            {
                get;
                init;
            }
            public ProtocolCANFD.IDFrameDirection IDFrameDirection
            {
                get;
                init;
            }

            public Int32 StandardID
            {
                get;
                init;
            }

            public Int32 ExtendedID
            {
                get; init;
            }

            public Int32 ByteCount
            {
                get; init;
            }

            public Int32 ByteIndex
            {
                get; init;
            }

            public UInt64 Data
            {
                get; init;
            }

            /// <summary>
            /// 数据偏移使能
            /// </summary>
            public Boolean DataOffsetEnabled
            {
                get; init;
            }

            /// <summary>
            /// 数据偏移量
            /// </summary>
            public Int32 DataOffset
            {
                get; init;
            }

            public ProtocolCANFD.DataRelation DataRelation
            {
                get; init;
            }
            //public ProtocolCANFD.ErrorPacketType ErrorPacketType
            //{
            //    get; init;
            //}
            public ProtocolCANFD.ErrorType ErrorType
            {
                get; init;
            }
        }

        public record TrigUSBConditionsOptions(ProtocolUSB.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolUSB.DataRelation DataRelation
            {
                get; init;
            }
            public UInt16 Data
            {
                get; init;
            }
            public ProtocolUSB.TokenPackageType TokenPackageType
            {
                get; init;
            }
            public ProtocolUSB.HandshakePackageType HandshakePackageType
            {
                get; init;
            }
            public ProtocolUSB.DataPackageType DataPackageType
            {
                get; init;
            }
            public ProtocolUSB.ErrorPackageType ErrorPackageType
            {
                get; init;
            }
            public ProtocolUSB.SpecialPacketType SpecialPacketType
            {
                get; init;
            }
        }
        public record TrigCANConditionsOptions(ProtocolCAN.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ProtocolCAN.FrameType FrameType
            {
                get;
                init;
            }

            public ProtocolCAN.IDStandard IDStandard
            {
                get;
                init;
            }
            public ProtocolCAN.IDFrameDirection IDFrameDirection
            {
                get;
                init;
            }

            public Int32 StandardID
            {
                get; init;
            }

            public Int32 ExtendedID
            {
                get; init;
            }
            public ProtocolCAN.DataRelation DataRelation
            {
                get; init;
            }

            public Int32 ByteIndex
            {
                get; init;
            }
            public Int32 ByteCount
            {
                get; init;
            }

            public UInt64 Data
            {
                get; init;
            }
            public ProtocolCAN.ErrorPacketType ErrorPacketType
            {
                get; init;
            }
        }

        public record TrigJTAGConditionsOptions(ComModel.ProtocolJTAG.Condition Condition) : ITrigDecoderConditionsOptions
        {
            public ComModel.ProtocolJTAG.DataRelation DataRelation
            {
                get;
                init;
            }

            public Byte[]? Data
            {
                get;
                init;
            }
        }

        public record TrigSENTConditionsOptions(ProtocolSENT.ChannelMode channelMode, ProtocolSENT.FastCondition fastCondition, ProtocolSENT.SlowCondition slowCondition) : ITrigDecoderConditionsOptions
        {
            #region 快速通道参数
            public ProtocolSENT.DataLength DataLength
            {
                get;
                init;
            }

            public Int64 FastChannelData
            {
                get;
                init;
            }

            public Int32 FastChannelStatus
            {
                get;
                init;
            }

            public Int32 FastChannelCRC
            {
                get;
                init;
            }

            public ProtocolSENT.FastError FastError
            {
                get;
                init;
            }

            #endregion

            #region 慢速通道参数
            public ProtocolSENT.SlowMessageCondition SlowMessageCondition
            {
                get;
                init;
            }

            public ProtocolSENT.SlowEnhancedMessageType SlowEnhancedMessageType
            {
                get;
                init;
            }

            public Int32 SlowChannelID
            {
                get;
                init;
            }

            public Int32 SlowChannelData
            {
                get;
                init;
            }

            public Int32 SlowChannelCRC
            {
                get;
                init;
            }
            #endregion


            public ProtocolSENT.DataRelation DataRelation
            {
                get;
                init;
            }
        }
        public record ProtocolManchesterOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();

                tempinfos.Add((Source1, new Double[1] { Threshold }));

                return tempinfos;
            }
            /*通道选择*/
            public ChannelId Source1 { get; init; }

            /*阈值门限*/
            public Double Threshold { get; init; }

            /*信号速率*/
            public Int64 BaudRate { get; init; }

            /*边沿选择*/
            public ProtocolManchester.Polarity Polarity { get; init; }

            /*位顺序*/
            public ProtocolManchester.MSB_LSB MSB_LSB { get; init; }

            /*奇偶性*/
            public ProtocolManchester.OddEvenCheck OddEvenCheck { get; init; }

            /*数据包视图flag*/
            public ProtocolManchester.DataView Flag { get; init; }

            /*Sync_size*/
            public Byte SyncSize { get; init; }

            /*Header_Size*/
            public Byte HeaderSize { get; init; }

            /*Data_Size*/
            public Byte DataSize { get; init; }

            /*Trailer_Size*/
            public Byte TrailerSize { get; init; }

            /*Idle_Size*/
            public Double Idle_Size { get; init; }

            /*Data_num*/
            public Byte Data_Num { get; init; }



            /*开始索引*/
            public Byte StartEdge { get; init; }

            /*容限*/
            public Double Tolerance { get; init; }
        }


        public record TrigPSI5ConditionsOptions(ProtocolPSI5.Condition Condition) : ITrigDecoderConditionsOptions
        {
            /// <summary>
            /// 数据域A
            /// </summary>
            public Int64 DataAValue
            {
                get;
                init;
            }

            /// <summary>
            /// 数据域B
            /// </summary>
            public Int64 DataBValue
            {
                get;
                init;
            }


            /// <summary>
            /// BlockID
            /// </summary>
            public Int64 BlockID
            {
                get;
                init;
            }


            /// <summary>
            /// Sensor Status
            /// </summary>
            private Int64 _SensorStatus;
            public Int64 SensorStatus
            {
                get;
                init;
            }

        }

        #endregion 解码触发结构参数                

        #region 解码结构参数

        public enum PotocalPAMLevel
        {
            DECODE_PAM2 = 0,
            DECODE_PAM3 = 1,


            NO_DEFINE = 99
        }

        public record DecoderOptions(Boolean Active, SerialProtocolType ProtocolType)
        {
            public Double Position
            {
                get;
                init;
            }

            public IDecoderOptions? ProtocolOptions
            {
                get;
                init;
            }
           // PotocalPAMLevel DecodePamLevel
        }

        public interface IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos();
        }

        public record ProtocolCloseOptions() : IDecoderOptions
        {
            //无参数
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>();
            }
        }
        public record ProtocolEthernetOptions() : IDecoderOptions
        {
            public ProtocolEthernet.EthernetVersion Version
            {
                get;
                init;
            }

            /// <summary>
            /// 数据源的阈值
            /// </summary>
            public Double Signal1ThresholdH
            {
                get;
                init;
            }
            /// <summary>
            /// 数据源的阈值
            /// </summary>
            public Double Signal1ThresholdL
            {
                get;
                init;
            }

            public ChannelId SignalInput1
            {
                get;
                init;
            }

            public ChannelId SignalInput2
            {
                get;
                init;
            }

            public ProtocolEthernet.SignalType SignalType
            {
                get;
                init;
            }

            public ProtocolEthernet.EthernetSpeed Speed
            {
                get;
                init;
            }
            //public Double Signal1Threshold
            //{
            //    get;
            //    init;
            //}
            //public Double Signal2Threshold
            //{
            //    get;
            //    init;
            //}
            public Byte QFlag
            {
                get;
                init;
            }

            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (SignalType == ProtocolEthernet.SignalType.Single)
                {
                    tempinfos.Add((SignalInput1, new Double[1] { Signal1ThresholdH }));
                    tempinfos.Add((SignalInput2, new Double[1] { Signal1ThresholdL }));
                }
                else
                {
                    tempinfos.Add((SignalInput1, new Double[2] { Signal1ThresholdH, Signal1ThresholdL }));
                }

                return tempinfos;
            }
        }

        public record ProtocolCPHYOptions() : IDecoderOptions
        {
            public Double AThreshold
            {
                get;
                init;
            }

            public Double BThreshold
            {
                get;
                init;
            }

            public Double CThreshold
            {
                get;
                init;
            }

            public Double LPAThreshold
            {
                get;
                init;
            }

            public Double LPCThreshold
            {
                get;
                init;
            }

            public ChannelId SignalInputA
            {
                get;
                init;
            }

            public ChannelId SignalInputB
            {
                get;
                init;
            }

            public ChannelId SignalInputC
            {
                get;
                init;
            }

            public ProtocolCPHY.SignalType SignalType
            {
                get;
                init;
            }

            public ProtocolCPHY.SubType SubType
            {
                get;
                init;
            }
            public Int64 BitRate
            {
                get;
                init;
            }

            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((SignalInputA, new Double[] { AThreshold, LPAThreshold }));
                tempinfos.Add((SignalInputB, new Double[] { BThreshold, 0 }));
                tempinfos.Add((SignalInputC, new Double[] { CThreshold, LPCThreshold }));
                return tempinfos;
            }
        }

        public record ProtocolUSBOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (InputType == ProtocolUSB.USBInputType.DIFF_INPUT)
                {
                    tempinfos.Add((Source1, new Double[2] { Source1ThresholdH, Source1ThresholdL }));

                }
                else
                {
                    tempinfos.Add((Source1, new Double[1] { Source1ThresholdH }));
                    tempinfos.Add((Source2, new Double[1] { Source2Threshold }));
                }

                return tempinfos;
            }

            public ChannelId Source1
            {
                get; init;
            }
            public ChannelId Source2
            {
                get; init;
            }
            public Double Source1ThresholdH
            {
                get; init;
            }
            public Double Source1ThresholdL
            {
                get; init;
            }

            public Double Source2Threshold
            {
                get; init;
            }
            public ProtocolUSB.SignalRate SignalRate
            {
                get; init;
            }
            public ProtocolUSB.USBInputType InputType
            {
                get; init;
            }

            public ProtocolUSB.USBSpeed BaudMode
            {
                get; init;
            }
            public UInt16 ByteCount
            {
                get; init;
            }
            /// <summary>
            /// 采样频率
            /// </summary>
            public Double SamplingFrequency;
            /// <summary>
            /// 自动时钟
            /// </summary>
            public bool AutoClock;
        }
        public record ProtocolRS232Options() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (SignalType == ProtocolRS232.SignalType.Single)
                {
                    tempinfos.Add((Source, new Double[1] { Threshold }));
                }
                else
                {
                    tempinfos.Add((Source, new Double[1] { Threshold }));
                    tempinfos.Add((SourceL, new Double[1] { Threshold }));
                }
                return tempinfos;
            }
            public ChannelId Source
            {
                get;
                init;
            }

            public ChannelId SourceL
            {
                get;
                init;
            }

            public ProtocolRS232.DataBitWidth DataBitWidth
            {
                get;
                init;
            }

            public ProtocolRS232.OddEvenCheck OddEvenCheck
            {
                get;
                init;
            }

            public ProtocolRS232.StopBit StopBit
            {
                get;
                init;
            }

            public ProtocolRS232.MSB_LSB BitSeq
            {
                get;
                init;
            }

            public ProtocolCommon.Polarity Polarity
            {
                get;
                init;
            }

            public UInt32 Baud
            {
                get;
                init;
            }

            public ProtocolRS232.SignalType SignalType
            {
                get;
                init;
            }
            public Double Threshold
            {
                get; init;
            }
        }
        public record ProtocolPCIeOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (SignalType == ProtocolPCIe.SignalType.Single)
                {
                    tempinfos.Add((SignalInput, new Double[1] { Threshold }));
                }
                else
                {
                    tempinfos.Add((SignalInput, new Double[1] { Threshold }));
                    tempinfos.Add((SignalIutput1, new Double[1] { Threshold }));
                }
                return tempinfos;
            }
            public ChannelId SignalInput
            {
                get; init;
            }
            public ChannelId SignalIutput1
            {
                get; init;
            }
            public ProtocolPCIe.PCIeVersion Version
            {
                get; init;
            }
            public ProtocolPCIe.SignalType SignalType
            {
                get; init;
            }
            public Double Threshold
            {
                get; init;
            }
            public UInt16 ByetsCount
            {
                get; init;
            }
        }
        public record ProtocolI2SOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>()
                {
                    (SCL,new Double[1] { SCLThreshold }),
                    (WS,new Double[1] { WSThreshold }),
                    (SDA,new Double[1] { SDAThreshold }),
                };
            }
            public ProtocolAudioBus.SubType SubType
            {
                get;
                init;
            }

            public ProtocolCommon.Polarity SyncPolarity
            {
                get;
                init;
            }
            public ProtocolCommon.Edge ClockEdge
            {
                get; init;
            }
            public ProtocolCommon.Polarity DataPolarity
            {
                get; init;
            }
            public ProtocolAudioBus.MSB_LSB MSB_LSB
            {
                get; init;
            }
            public ProtocolAudioBus.SoundChannel SoundChannel
            {
                get; init;

            }

            public Int32 BitDelayCount
            {
                get; init;
            }
            /// <summary>
            /// 每通道时钟位
            /// </summary>
            public Int32 ClockBitNumberPerChannel { get; init; }

            /// <summary>
            /// 每帧通道数量
            /// </summary>
            public Int32 ChannelNumberPerFream { get; init; }

            public Int32 DataBitCount
            {
                get; init;
            }

            public Int32 ClockBitCount
            {
                get; init;
            }
            public Int32 SoundChannelCount
            {
                get; init;
            }
            public ChannelId SCL
            {
                get; init;
            }
            public ChannelId WS
            {
                get; init;
            }
            public ChannelId SDA
            {
                get; init;
            }
            public Double SCLThreshold
            {
                get; init;
            }
            public Double WSThreshold
            {
                get; init;
            }
            public Double SDAThreshold
            {
                get; init;
            }
        }
        public record ProtocolI2COptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>()
                {
                    (SCLK,new Double[1]{SCLKThreshold }),
                    (SDA,new Double[1]{SDAThreshold }),
                };
            }
            public ChannelId SCLK
            {
                get;
                init;
            }
            public ChannelId SDA
            {
                get;
                init;
            }

            public Double SCLKThreshold
            {
                get;
                init;
            }

            public Double SDAThreshold
            {
                get;
                init;
            }
            public ProtocolI2C.AddrBitWidth BitWidth
            {
                get; init;
            }
        }

        public record ProtocolSPIOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((CLK, new Double[]{ CLKThreshold, 0}));
                if (FramingMode == ProtocolSPI.FramingMode.CS)
                {
                    tempinfos.Add((CS, new Double[] { CSThreshold, 0 }));
                }

                tempinfos.Add((MOSI, new Double[] { MOSIThreshold, 0 }));
                 
                return tempinfos;
            }
            public ComModel.ProtocolSPI.FramingMode FramingMode
            {
                get;
                init;
            }

            public ComModel.ProtocolSPI.DecodeChannel DecodeChannel
            {
                get;
                init;
            }

            public Int32 FrameCount
            {
                get;
                init;
            }

            public ComModel.ChannelId CLK
            {
                get;
                init;
            }

            public ComModel.ChannelId MOSI
            {
                get;
                init;
            }

            //public ComModel.ChannelId MISO
            //{
            //    get;
            //    init;
            //}

            public ComModel.ChannelId CS
            {
                get;
                init;
            }

            public Double IdleTime
            {
                get;
                init;
            }

            public Double CLKThreshold
            {
                get;
                init;
            }

            public Double MOSIThreshold
            {
                get;
                init;
            }

            public Double MISOThreshold
            {
                get;
                init;
            }

            public ComModel.ProtocolSPI.MSB_LSB ByteOrder
            {
                get;
                init;
            }

            public ComModel.ProtocolCommon.Polarity MISOPolarity
            {
                get;
                init;
            }

            public ComModel.ProtocolCommon.Polarity MOSIPolarity
            {
                get;
                init;
            }

            public ComModel.ProtocolSPI.LevelState CSLevelState
            {
                get;
                init;
            }

            public ComModel.ProtocolCommon.Edge CLKState
            {
                get;
                init;
            }

            public Double CSThreshold
            {
                get;
                init;
            }
        }
        public record ProtocolSATAOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((Source, new Double[1] { Threshold }));
                return tempinfos;
            }
            public ProtocolSATA.SATAVersion Version
            {
                get; init;
            }
            public ChannelId Source
            {
                get; init;
            }
            public ChannelId Source1
            {
                get; init;
            }
            public Double Threshold
            {
                get; init;
            }
            public UInt16 BytesCount
            {
                get; init;
            }
        }
        public record ProtocolARIN429Options() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((SignalInputA, new Double[2] { ThresholdH, ThresholdL }));
                if (InputMode == ProtocolARINC429.InputMode.Diff)
                {
                    tempinfos.Add((SignalInputB, new Double[2] { ThresholdH, ThresholdL }));
                }
                return tempinfos;
            }
            public ProtocolARINC429.DecodeMode DecodeMode
            {
                get;
                init;
            }

            public ChannelId SignalInputA
            {
                get; init;
            }
            public ProtocolARINC429.InputMode InputMode
            {
                get; init;
            }

            public ChannelId SignalInputB
            {
                get; init;
            }

            public Int32 Baud//自定义速率
            {
                get; init;
            }
            /// <summary>
            /// 数据源的阈值
            /// </summary>
            public Double ThresholdH
            {
                get; init;
            }
            public Double ThresholdL
            {
                get; init;
            }
        }

        public record ProtocolSPMIOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>()
                {
                    (SCLK,new Double[1] { CLKThreshold } ),
                    (SData,new Double[1] { DataThreshold }),
                };
            }
            public Double CLKThreshold { get; init; }
            public Double DataThreshold { get; init; }
            public ChannelId SData { get; init; }
            public ChannelId SCLK { get; init; }
            public ProtocolSPMI.Version Version { get; init; }
            public ProtocolSPMI.CheckType CheckType { get; init; }
        }
        public record ProtocolMILOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>()
                {
                    (Source,new Double[2] { HighThreshold,LowThreshold }),
                };
            }
            public Int32 SignalRate { get; init; }
            public Double HighThreshold { get; init; }
            public Double LowThreshold { get; init; }
            public ChannelId Source { get; init; }
            public ProtocolCommon.Polarity Polarity { get; init; }
        }
        public record ProtocolNRZOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();

                tempinfos.Add((Source1, new Double[1] { Threshold }));

                return tempinfos;
            }
            public Int64 SignalRate { get; init; }
            //public ProtocolNRZ.SignalType SignalType { get; init; }
            public ChannelId Source1 { get; init; }
            //public ChannelId Source2 { get; init; }
            public Double Threshold { get; init; }
            public ProtocolNRZ.MSB_LSB MSB_LSB { get; init; }
            //public UInt32 IdleTime { get; init; }
            //public ProtocolNRZ.IdleLevel IdleLevel { get; init; }
            public ProtocolNRZ.Mode Mode { get; init; }

        }
        public record ProtocolCANOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (SignalType == ProtocolCAN.SignalType.Diff)
                {
                    tempinfos.Add((SignalInput1, new Double[1] { SDAThreshold }));
                    tempinfos.Add((SignalInput2, new Double[1] { SDAThreshold }));
                }
                else
                {
                    tempinfos.Add((SignalInput1, new Double[1] { SDAThreshold }));
                }

                return tempinfos;
            }
            public Int64 SignalRate
            {
                get;
                init;
            }

            //信号类型
            public ProtocolCAN.SignalType SignalType
            {
                get;
                init;
            }

            //输入1
            public ChannelId SignalInput1
            {
                get; init;
            }

            //输入2(信号类型选择"差分"时使用)
            public ChannelId SignalInput2
            {
                get; init;
            }

            //采样点
            public Int32 SamplePoint
            {
                get; init;
            }
            /// <summary>
            /// 数据源的阈值
            /// </summary>
            public Double SDAThreshold
            {
                get; init;
            }
        }


        public record ProtocolCANFDOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (SignalType == ProtocolCANFD.SignalType.Diff)
                {
                    tempinfos.Add((SignalInput1, new Double[1] { SDAThreshold }));
                    tempinfos.Add((SignalInput2, new Double[1] { SDAThreshold }));
                }
                else
                {
                    tempinfos.Add((SignalInput1, new Double[1] { SDAThreshold }));
                }

                return tempinfos;
            }
            public Int64 SDSignalRate
            {
                get;
                init;
            }
            public Int64 FDSignalRate
            {
                get;
                init;
            }
            //信号类型
            public ProtocolCANFD.SignalType SignalType
            {
                get;
                init;
            }

            //输入1
            public ChannelId SignalInput1
            {
                get; init;
            }

            //输入2(信号类型选择"差分"时使用)
            public ChannelId SignalInput2
            {
                get; init;
            }

            /// <summary>
            /// 仲裁域采样点
            /// </summary>
            public Int32 SamplePoint
            {
                get; init;
            }

            /// <summary>
            /// 数据域采样点
            /// </summary>
            public Int32 DataSamplePoint
            {
                get; init;
            }

            /// <summary>
            /// 数据源的阈值
            /// </summary>
            public Double SDAThreshold
            {
                get; init;
            }
        }


        public record ProtocolJTAGOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((TCK, new Double[1] { TCKThreshold }));
                tempinfos.Add((TDI, new Double[1] { TDIThreshold }));
                tempinfos.Add((TDO, new Double[1] { TDOThreshold }));
                tempinfos.Add((TMS, new Double[1] { TMSThreshold }));
                return tempinfos;
            }
            public ChannelId TCK
            {
                get;
                init;
            }

            public Double TCKThreshold
            {
                get;
                init;
            }

            public ChannelId TMS
            {
                get; init;
            }


            public Double TMSThreshold
            {
                get; init;
            }

            public ChannelId TDI
            {
                get; init;
            }

            public Double TDIThreshold
            {
                get; init;
            }


            public ChannelId TDO
            {
                get; init;
            }


            public Double TDOThreshold
            {
                get; init;
            }


            public ComModel.ProtocolJTAG.DecodeChannel DecodeChannel
            {
                get; init;
            }

            public UInt32 BitRate
            {
                get; init;
            }
        }

        public record ProtocolLINOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((Source, new Double[1] { Threshold }));

                return tempinfos;
            }
            public ChannelId Source
            {
                get;
                init;
            }

            public ProtocolLIN.Standard Standard
            {
                get;
                init;
            }


            public ProtocolCommon.Polarity Polarity
            {
                get; init;
            }

            public ProtocolLIN.PIncludeOddEven PIncludeOddEven
            {
                get; init;
            }

            public Double Threshold
            {
                get; init;
            }




            public Int32 BPS
            {
                get; init;
            }
            public Int32 DataCount
            {
                get; init;
            }
        }

        public record ProtocolFlexRayOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                if (/*SourceType ==  ProtocolFlexRay.SourceType.BDiff*/ false)
                {
                    tempinfos.Add((Source, new Double[1] { Threshold }));
                    tempinfos.Add((SourceL, new Double[1] { Threshold }));
                }
                else
                {
                    tempinfos.Add((Source, new Double[1] { Threshold }));
                }

                return tempinfos;
            }
            public ChannelId Source
            {
                get;
                init;
            }
            public ChannelId SourceL
            {
                get; init;
            }

            public ProtocolFlexRay.ChannelType ChannelType
            {
                get;
                init;
            }


            public ProtocolFlexRay.SourceType SourceType
            {
                get; init;
            }

            public Int64 SignalRate
            {
                get; init;
            }


            public Double Threshold
            {
                get; init;
            }
        }
        public record ProtocolSENTOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((Source, new Double[1] { Threshold }));
                return tempinfos;
            }
            public ComModel.ChannelId Source
            {
                get;
                init;
            }


            public ProtocolSENT.PauseBit PauseBit
            {
                get;
                init;
            }


            public ProtocolSENT.ChannelMode ChannelMode
            {
                get; init;
            }


            public ProtocolSENT.FastChannelMode FastChannelMode
            {
                get; init;
            }

            public ProtocolSENT.DataLength DataLength
            {
                get; init;
            }


            public ProtocolCommon.Polarity Polarity
            {
                get; init;
            }



            public Double ClockTick
            {
                get; init;
            }

            public Int32 Tolerance
            {
                get; init;
            }

            public Double Threshold
            {
                get; init;
            }
        }

        public record ProtocolPSI5Options : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((Source, new Double[1] { Threshold }));
                return tempinfos;
            }
            public ComModel.ChannelId Source
            {
                get;
                init;
            }
            public Double Threshold
            {
                get; init;
            }


            public ProtocolPSI5.Psi5BaudMode Psi5BaudMode
            {
                get;
                init;
            }


            public ProtocolPSI5.Psi5FrameControl Psi5FrameControl
            {
                get; init;
            }


            public ProtocolPSI5.Psi5Status Psi5Status
            {
                get; init;
            }
            public ProtocolPSI5.Psi5Status Psi5SerialMessage
            {
                get; init;
            }

            public UInt64 DataASize
            {
                get; init;
            }

            public UInt64 DataBSize
            {
                get; init;
            }

        }

        public record ProtocolSMBusOptions() : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                return new List<(ChannelId ChannelId, Double[] Threshold)>()
                {
                    (SMBClK,new Double[1] { CLKThreshold } ),
                    (SMBData,new Double[1] { DataThreshold }),
                };
            }
            public Double CLKThreshold { get; init; }
            public Double DataThreshold { get; init; }
            public ChannelId SMBData { get; init; }
            public ChannelId SMBClK { get; init; }
            public ProtocolSMBus.PECByte PECByte { get; init; }
        }
        public record Protocol8b10bOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();

                tempinfos.Add((Source, new Double[1] { Threshold }));

                return tempinfos;
            }

            /*通道选择*/
            public ChannelId Source { get; init; }

            /*阈值门限*/
            public Double Threshold { get; init; }

            /*信号速率*/
            public Int64 BaudRate { get; init; }

        }
        public record ProtocolMlt3Options : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();
                tempinfos.Add((Source, new Double[2] { HighThreshold, LowThreshold }));

                return tempinfos;
            }

            /*通道选择*/
            public ChannelId Source { get; init; }

            /*阈值门限*/
            public Double HighThreshold { get; init; }

            /*阈值门限*/
            public Double LowThreshold { get; init; }

            /*信号速率*/
            public Int64 BaudRate { get; init; }

            /*零电平保持个数*/
            public UInt32 ZeroCount { get; init; }

        }
        public record ProtocolCXPIOptions : IDecoderOptions
        {
            public List<(ChannelId ChannelId, Double[] Threshold)> GetThresholdInfos()
            {
                var tempinfos = new List<(ChannelId ChannelId, Double[] Threshold)>();

                tempinfos.Add((Source, new Double[1] { Threshold }));

                return tempinfos;
            }

            /*通道选择*/
            public ChannelId Source { get; init; }

            /*阈值门限*/
            public Double Threshold { get; init; }

            /*信号速率*/
            public Int64 BaudRate { get; init; }

        }
        #endregion 解码结构参数
    }
}
