using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public enum SerialProtocolType
    {
        [Alias("Close")]
        [Display("Close")]
        Close,
        [Alias("RS232")]
        [Display("RS232")]
        RS232,//422、485、UART
        [Alias("I2C")]
        [Display("I2C")]
        I2C,
        [Alias("SPI")]
        [Display("SPI")]
        SPI,
        [Alias("CAN")]
        [Display("CAN")]
        CAN,
        [Alias("CAN-FD")]
        [Display("CAN-FD")]
        CAN_FD,
        [Alias("LIN")]
        [Display("LIN")]
        LIN,
        [Alias("FlexRay")]
        [Display("FlexRay")]
        FlexRay,
        [Alias("AudioBus")]
        [Display("AudioBus")]
        AudioBus,// I2S（I2S、LJ、RJ）
        [Alias("MIL-STD-1553")]
        [Display("MIL")]
        MIL = 10,
        [Alias("ARINC429")]
        [Display("ARINC429")]
        ARINC429,
        [Alias("USB")]
        [Display("USB")]
        USB,//1.X、2.0
        [Alias("SENT")]
        [Display("SENT")]
        SENT,
        [Alias("SPMI")]
        [Display("SPMI")]
        SPMI,
        [Alias("Ethernet")]
        [Display("Ethernet")]
        Ethernet,
        [Alias("CXPI")]
        [Display("CXPI")]
        CXPI,
        [Alias("NFC")]
        [Display("NFC")]
        NFC,
        [Alias("PD")]
        [Display("PD")]
        PD,
        [Alias("NRZ")]
        [Display("NRZ")]
        NRZ,
        [Alias("Manchester")]
        [Display("Manchester")]
        Manchester,
        [Alias("DigRF-3G")]
        [Display("DigRF-3G")]
        DigRF_3G,
        [Alias("DigRF-V4")]
        [Display("DigRF-V4")]
        DigRF_V4,
        [Alias("8b10b")]
        [Display("8b10b")]
        Common_8b10b,
        [Alias("JTAG")]
        [Display("JTAG")]
        JTAG,
        [Alias("SATA")]
        [Display("SATA")]
        SATA,
        [Alias("PCIe")]
        [Display("PCIe")]
        PCIe,
        [Alias("PSI5")]
        [Display("PSI5")]
        PSI5,
        [Alias("CPHY")]
        [Display("CPHY")]
        CPHY,
        [Alias("I3C")]
        [Display("I3C")]
        I3C,
        [Alias("SMBus")]
        [Display("SMBus")]
        SMBus,
        [Alias("Mlt3")]
        [Display("Mlt3")]
        Mlt3


    }

    public class DecodeEvent
    {
        public Int64 EventIndex { get; init; }

        public Double StartTime { get; init; }

        public Double EndTime { get; init; }

        public String Name { get; init; } = "";

        public UInt16 Data { get; init; }

    }


    public enum DecodeDisplayMode
    {
        //Close = 0,
        [Display("Hex")]
        Hex = 0,
        [Display("Dec")]
        Dec,
        [Display("Bin")]
        Binary,
        [Display("ASCII")]
        ASCII,
        [Display("Auto")]
        Auto,
    }

    #region TriggerSerial

    //public enum PulseCondition
    //{
    //    GreaterThan,
    //    LessThan,
    //    Equal,
    //    NotEqual,
    //}

    public enum ByteOrder
    {
        LSB,
        MSB,
    }
    #endregion

    public class ProtocolCommon
    {
        public enum Edge
        {
            Rise,
            Fall,
        }

        public enum Polarity
        {
            Positive,
            Negative
        }
    }

    public class ProtocolRS232
    {
        public enum Conditions
        {
            FrameStart = 0,
            PackageEnd = 1,
            CheckError = 2,
            SpecialData = 3

        }

        //信号类型
        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }

        public enum DataBitWidth
        {
            DataBitWidth_5Bit = 5,
            DataBitWidth_6Bit = 6,
            DataBitWidth_7Bit = 7,
            DataBitWidth_8Bit = 8
        }

        //停止位
        public enum StopBit
        {
            StopBit_1bit = 0,
            StopBit_2bit = 1,

        }
        //奇偶校验
        public enum OddEvenCheck
        {
            None = 0,
            Odd = 1, //奇校验
            Even = 2, //偶校验

        }

        //位顺序
        public enum MSB_LSB
        {
            MSB = 0,
            LSB = 1
        }

        //波特率
        public enum BPSList
        {
            [Description("ZiDingYi")]
            BPSList_Custom = 7,
            BPSList_2400 = 2400,
            BPSList_4800 = 4800,
            BPSList_9600 = 9600,
            BPSList_19200 = 19200,
            BPSList_38400 = 38400,
            BPSList_57600 = 57600,
            BPSList_115200 = 115200,
        }

        //数据包结束标志（触发条件选择“包结束”时使用）
        public enum PackageEndFlag
        {
            NULL,
            LF,
            CR,
            SP,
            FFH
        }
    }

    public class ProtocolI2C
    {
        public enum Condition
        {
            Start,
            ReStart,
            Stop,
            Lost,
            Address,
            Data,
            AddressAndData
        }

        //数据方向（触发条件选择“地址”时使用）
        public enum DataDirection
        {
            Write,
            Read,
        }

        //地址位宽（触发条件选择“地址”时使用）
        public enum AddrBitWidth
        {
            AddrBitWidth_7 = 7,
            AddrBitWidth_10 = 10
        }

        //数据限定（触发条件选择“数据”时使用）
        public enum DataRelation
        {
            EQ,
            GT,
            LT,
            NEQ
        }
    }

    public class ProtocolSPI
    {
        public enum DecodeChannel
        {
            MISO,
            MOSI,
            BOTH
        }
        //位顺序
        public enum MSB_LSB
        {
            LSB,
            MSB,
        }
       

        public enum DataTriggerSource
        {
            //MISO = DecodeChannel.MISO,
            MOSI = DecodeChannel.MOSI,
            //MOMI = DecodeChannel.MOMI,
        }
        public enum FramingMode
        {
            TIMEOUT,
            CS
        }
 
        public enum LevelState
        {
            Low,
            Hight,
        }

        public enum Condition
        {
            CS,
            Data,
        }
    }

    public class ProtocolSENT
    {
        //触发条件
        public enum FastCondition
        {
            Sync,          //同步
            Status,         //状态
            Data,          //数据
            CRC,           //校验
            StatusData,    //状态+数据
            StatusDataCRC,  //状态+数据+CRC
            Error,         //CRC错误
        };

        public enum FastError
        {
            CRCError,     //快速CRC错误
            ConPulsError  //连续脉冲错误
        }

        public enum SlowCondition
        {
            ID=1,                  //ID
            Data,                //数据
            CRC,                 //CRC
            IDData,              //ID+数据
            CRCError = 9,             //CRC错误
        }

        public enum SlowMessageCondition
        {
            EnhancedMessage,      //增强型消息
            ShortMessage,         //简短型消息
        }

        public enum SlowEnhancedMessageType
        {
            Enhanced8BitID, //增强8bitID+12bit数据
            Enhanced4BitID, //增强4bitID+16bit数据
        }

        //数据限定
        public enum DataRelation
        {
            Lt,
            gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };

        //附加间隙
        public enum PauseBit
        {
            No,  //无
            Yes, //有
        };

        public enum ChannelMode
        {
            FastChannel,
            SlowChannel
        }

        public enum FastChannelMode
        {
            Nibbles,
            FastChannel
        }

        //数据长度
        public enum DataLength
        {
            //Nibbles_5,
            //Nibbles_6,
            //Nibbles_7,
            //Nibbles_8,
            [Description("1Nibbles")]
            Nibbles_1,
            [Description("2Nibbles")]
            Nibbles_2,
            [Description("3Nibbles")]
            Nibbles_3,
            [Description("4Nibbles")]
            Nibbles_4,
            [Description("5Nibbles")]
            Nibbles_5,
            [Description("6Nibbles")]
            Nibbles_6,
        };

        //信号速率（类似于CAN总线的速率）
        public enum ClockTick
        {
            [Description("ZiDingYi")]
            ClockTick_Custom,   //自定义速率
            [Description("1μs")]
            MicroSecond_1,
            [Description("3μs")]
            MicroSecond_3,
            [Description("10μs")]
            MicroSecond_10,
            [Description("30μs")]
            MicroSecond_30,
            [Description("100μs")]
            MicroSecond_100,
            [Description("300μs")]
            MicroSecond_300,
        };
    }

    public class ProtocolARINC429
    {

        //ARINC429
        //触发条件
        /*
        public enum Condition
        {
            FrameHead,         //帧头
            Label = 2,         //LABEL
            SDI = 3,           //SDI
            SSM = 4,           //SSM
            Data = 5,          //DATA
            LabelAndData = 6,  //标签和数据
            Error = 7,         //错误
            FrameTail = 8      //结束
        }*/

        public enum Condition
        {
            FrameHead = 0,         //帧头
            FrameTail,      //结束
            Label,         //LABEL
            SDI,           //SDI
            Data,          //DATA
            SSM,           //SSM
            LabelAndData,  //标签和数据
            Error,         //错误
        };

        //数据限定
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };

        //输入模式
        public enum InputMode
        {
            ABDiff,   //AB通道差分
            Diff      //差分输入
        };

        //数据格式
        public enum DecodeMode
        {
            Mode_8_2_19_1,  //8-2-19-1
            Mode_8_21_2_1,
            Mode_8_23_1         //8-23-1
        };

        //信号速率（类似于CAN总线的速率）
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_custom,   //自定义速率
            SignalRate_12_5k,
            SignalRate_100k,
        };
        public enum ErrorType
        {
            CRCError = 0,
            WordError,
            GapError,
            AnyError,
        };
    }

    public class ProtocolEthernet
    {
        public enum EthernetSpeed
        {
            EthernetSpeed_10M,
            EthernetSpeed_100M,
            EthernetSpeed_1000M,
        }
        public enum EthernetVersion
        {
            IPv4,
            IPv6,
        }
        //信号类型
        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }
        /*
         * 触发现在只做了3个，其他的先屏蔽
         */
        public enum Condition
        {
            FrameHead, // 帧开头
            MACAddress, // MAC地址
            MACLenghtOrType, // MAC长度/类型          
            //IPHeader,            
            //TCPHeader,
            QTagInfo, // Q标记控制信息
            ClientData, // 客户端数据
            DataEndPos, // 数据包的结尾
            Idle, // 空闲
            CRCError // CRC错误
        };

        //数据限定
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };
    }
    public class ProtocolCPHY
    {
        public enum SubType
        {
            CSI,
            DSI,
            Symbol,
            Word
        }
        //信号类型
        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }

        //具体数据类型
        public enum DataType
        {
            SyncShortPacketData = 0x00,    //同步短包,datatype:0x00-0x07
            GenericShortPacketData = 0x08, //通用短包,datatype:0x08-0x0f
            NullData = 0x10,
            BlankData = 0x11,
            EmbeddedBit8NonImageData = 0x12, //0x12,嵌入式8位非图像数据
            GenericLongPacketDataType1 = 0x13,
            GenericLongPacketDataType2 = 0x14,
            GenericLongPacketDataType3 = 0x15,
            GenericLongPacketDataType4 = 0x16,
            Yuv420Bit8 = 0x18,
            Yuv420Bit10 = 0x19,
            LegacyYuv420Bit8 = 0x1A,
            Yuv420Bit8ChromaShiftedPixelSampling = 0x1C,
            Yuv420Bit10ChromaShiftedPixelSampling = 0x1D,
            Yuv422Bit8 = 0x1E,
            Yuv422Bit10 = 0x1F,
            Rgb444 = 0x20,
            Rgb555 = 0x21,
            Rgb565 = 0x22,
            Rgb666 = 0x23,
            Rgb888 = 0x24,
            Raw24 = 0x27,
            Raw6 = 0x28,
            Raw7 = 0x29,
            Raw8 = 0x2A,
            Raw10 = 0x2B,
            Raw12 = 0x2C,
            Raw14 = 0x2D,
            Raw16 = 0x2E,
            Raw20 = 0x2F,
            UserDefinedBit8DataType_1 = 0x30, //用户自定义类型
            UserDefinedBit8DataType_2 = 0x31,
            UserDefinedBit8DataType_3 = 0x32,
            UserDefinedBit8DataType_4 = 0x33,
            UserDefinedBit8DataType_5 = 0x34,
            UserDefinedBit8DataType_6 = 0x35,
            UserDefinedBit8DataType_7 = 0x36,
            UserDefinedBit8DataType_8 = 0x37,
            UslCommands = 0x38,
            OtherReserved //其他预留类型
        };
    }

    public class ProtocolPCIe
    {
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };
        public enum PCIeVersion
        {
            PCIeV1_0,
            PCIeV2_0,
        }

        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }

        public enum Condition
        {
            Command,
            STP,
            SDP,
            SeqID,
            TLPType,
            TC,
            AT,
            TransID,
            MsgCode,
            Address,
            Data,
            Decode_Error,
            EP,
            FormatError,
            ECRCError,
            LCRCError,
            End,
            EDB,
        }
        public enum TLPType
        {
            MRD = 1,
            MRDLK,
            MWr,
            IORd,
            IOWr,
            CfgRd0,
            CfgWr0,
            CfgRd1,
            CfgWr1,
            TCfgRd,
            TCfgWr,
            Msg,
            MsgD,
            Cpl,
            CplD,
            CplLK,
            CplDLK,
        }
    }
    public class ProtocolMIL
    {
        public enum Condition
        {
            CmdOrStatus = 1,
            Data = 2,
            Error = 3,
            Sync = 4,
        };
        //public enum Condition
        //{
          
        //   Condition_NONE,
        //};

    //数据限定（触发条件:命令字、状态字、数据字）
    public enum Relation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };

        //位设置（用于所有需要选择0,1，x三个值中的一个的参数选项）
        public enum BitSetting
        {
            Bit_0,
            Bit_1,
            Bit_X
        };
        //奇偶性（用于所有需要选择0,1，x:无三个值中的一个的参数选项）
        public enum Parity
        {
            Parity_0 = 0,
            Parity_1,
            Parity_X
        };
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            Custom,
            SignalRate_1M,
            SignalRate_10M,
        }

        //数据限定（触发条件:空闲时间）
        public enum IdleTimeDataRelation
        {
            Lt,
            Gt,
            In,
            Out
        };

        //错误类型（触发条件：错误类型）
        public enum ErrorType
        {
            Oddevent,  //奇偶校验
            Sync,      //同步
            Manchester,    //曼彻斯特
            Notcontinue    //非连续数
        };


    }

    public class ProtocolUSB
    {
        public enum Condition
        {
            Sync,
            Reset,
            Pause,
            Resume,
            PackageEnd,
            TokenPackage,
            HandshakePackage,
            DataPackage,
            //Sof,
            Special,
            Error,
        }
        //信号速率
        public enum SignalRate
        {
            LowRate,
            FullRate,
            HighRate,
        }

        //令牌类型（触发条件选择“令牌包”时使用，PID之一）
        public enum TokenPackageType
        {
            Out = 0b0001,
            SOF = 0b0101,
            IN = 0b1001,
            SETUP = 0b1101,
        }

        public enum SpecialPacketType
        {
            //Pre=0b1100,
            Err = 0b1100,
            Split = 0b1000,
            Ping = 0b0100,
            Remain = 0b000,
        }
        //握手包类型（触发条件选择“握手包”时使用，PID之一）
        public enum HandshakePackageType
        {
            ACK = 0b0010,
            NAK = 0b1010,
            STALL = 0b1110,
            Nyet = 0b0110,
        }

        //数据包类型（触发条件选择“数据包”时使用，PID之一）
        public enum DataPackageType
        {
            Data0 = 0b0011,
            Data1 = 0b1011,
            Data2 = 0b0111,
            MData = 0b1111,
        }

        //错误类型（触发条件选择“错误”时使用）
        public enum ErrorPackageType
        {
            PIDCRC,
            CRC5,
            CRC16,
            BitFill
        }

        //数据限定（触发条件选择“数据包”、“SOF”、“令牌包”时使用）
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        }
        /// <Author>
        /// ZXL
        /// </Author>
        public enum USBInputType
        {
            DOUBLE_INPUT = 0,
            DIFF_INPUT

        };
        public enum USBSpeed
        {
            NONE = 0,
            LOW_SPEED,       //1.5M/s
            FULL_SPEED,      //12M/s
            HIGH_SPEED     //480M/s
        };

        public enum USBFiledType
        {
            NO_DEFINE,
            SOP,
            SYNC,
            PID,
            ADDR,
            ENDP,   //端口号
            FRAME,
            DATA,
            CRC,
            EOP,

            //START_SPLIT
            START_SPLIT_SC,
            START_SPLIT_PORT,
            START_SPLIT_S,
            START_SPLIT_E,
            START_SPLIT_ET,


            //COMPLETE_SPLIT
            COMPLETE_SPLIT_SC,
            COMPLETE_SPLIT_PORT,
            COMPLETE_SPLIT_S,
            COMPLETE_SPLIT_U,
            COMPLETE_SPLIT_ET,
        };



    }

    public class ProtocolCAN
    {
        public enum Condition
        {
            FrameStart,     //帧起始
            FrameType,      //帧类型
            ID,             //ID
            Data,               //数据
            IDandData,      //ID和数据
            FrameEnd,            //帧结束
            Error,
        }

        //帧类型(触发条件选择"帧类型"时使用)
        public enum FrameType
        {
            Data,
            Remote,
            Error,
            Overload,
        }

        //ID帧类型(触发条件选择"ID"或"ID和数据"时使用)
        public enum IDFrameDirection
        {
            Write = 0,
            Read,
            Both
        }

        //信号类型
        public enum SignalType
        {
            CAN_L,
            CAN_H,
            RXTX,
            Diff
        }
        public enum ErrorPacketType
        {
            AckLose,            //ACK丢失
            BitFillError,       //位填充错
            CRCError,           //crc错误
            AllError            //所有错误

        }

        //ID标准(触发条件选择"ID"或"ID和数据"时使用)
        public enum IDStandard
        {
            Standard,
            Extended
        }

        ////数据限定(触发条件选择"数据"或"ID和数据"时使用)
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
        }
        /*
        //old 信号速率
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_custom,    //自定义速率
            SignalRate_10k,
            SignalRate_19_2k,
            SignalRate_20k,
            SignalRate_33_3k,
            SignalRate_50k,
            SignalRate_62_5k,
            SignalRate_83_3k,
            SignalRate_100k,
            SignalRate_125k,
            SignalRate_1m,
        }
        */
        //new 信号速率
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_custom,    //自定义速率
            SignalRate_10k,
            SignalRate_19_2k,
            SignalRate_20k,
            SignalRate_33_3k,
            SignalRate_38_4k,
            SignalRate_50k,
            SignalRate_57_6k,
            SignalRate_62_5k,
            SignalRate_83_3k,
            SignalRate_100k,
            SignalRate_115_2k,
            SignalRate_125k,
            SignalRate_230_4k,
            SignalRate_250k,
            SignalRate_490_8k,
            SignalRate_500k,
            SignalRate_800k,
            SignalRate_921_6k,
            SignalRate_1M,
            SignalRate_2M,
            SignalRate_3M,
            SignalRate_4M,
            SignalRate_5M
        }
    }

    public class ProtocolCANFD
    {
        public enum Condition
        {
            FrameStart,     //帧起始
            FrameType,      //帧类型
            ID,             //ID
            Data,           //数据
            IDandData,      //ID和数据
            FrameEnd,       //帧结束
            Error          //错误
        }

        //帧类型(触发条件选择"帧类型"时使用)
        public enum FrameType
        {
            /// <summary>
            /// 数据帧
            /// </summary>
            Data,
            /// <summary>
            /// 远程帧
            /// </summary>
            Remote,
            /// <summary>
            /// 变速帧
            /// </summary>
            // VarFrame,
            /// <summary>
            /// 错误帧
            /// </summary>
            Error,
            /// <summary>
            /// 超载帧
            /// </summary>
            Overload
        }
        //public enum ErrorPacketType
        //{
        //    AckLose = 10,            //ACK丢失
        //    BitFillError,       //位填充错
        //}
        //ID帧数据方向(触发条件选择"ID"或"ID和数据"时使用)
        public enum IDFrameDirection
        {
            Read = 0b00,
            Write = 0b01,
            Both = 0b11,
        }

        //信号类型
        public enum SignalType
        {
            [Display("CAN-FD_L")]
            CAN_FDL,
            [Display("CAN-FD_H")]
            CAN_FDH,
            RXTX,
            Diff
        }

        //ID标准(触发条件选择"ID"或"ID和数据"时使用)
        public enum IDStandard
        {
            [Description("BiaoZhun")]
            Standard,
            [Description("KuoZhan")]
            Extended,
            [Description("FDBiaoZhun")]
            FDStandard,
            [Description("FDKuoZhan")]
            FDExtended
        }

        ////数据限定(触发条件选择"数据"或"ID和数据"时使用)
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
        }

        //信号速率
        public enum SDSignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_custom,    //自定义速率
            SignalRate_10k,
            SignalRate_19_2k,
            SignalRate_20k,
            SignalRate_33_3k,
            SignalRate_38_4k,
            SignalRate_50k,
            SignalRate_57_6k,
            SignalRate_62_5k,
            SignalRate_83_3k,
            SignalRate_100k,
            SignalRate_115_2k,
            SignalRate_125k,
            SignalRate_230_4k,
            SignalRate_250k,
            SignalRate_490_8k,
            SignalRate_500k,
            SignalRate_800k,
            SignalRate_921_6k,
            SignalRate_1M,
            SignalRate_2M,
            SignalRate_3M,
            SignalRate_4M,
            SignalRate_5M
        }

        //old FDSignalRate
        /*        
        public enum FDSignalRate
        {
            [Description("ZiDingYi")]
        SignalRate_custom,    //自定义速率
            SignalRate_1M,
            SignalRate_2M,
            SignalRate_3M,
            SignalRate_4M,
            SignalRate_5M,
            SignalRate_6M,
            SignalRate_7M,
            SignalRate_8M,
        }*/

        public enum FDSignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_custom,    //自定义速率
            SignalRate_250k,
            SignalRate_500k,
            SignalRate_800k,
            SignalRate_1M,
            SignalRate_1_5M,
            SignalRate_2M,
            SignalRate_3M,
            SignalRate_4M,
            SignalRate_5M,
            SignalRate_6M,
            SignalRate_7M,
            SignalRate_8M,
        }

        public enum ErrorType
        {
            /// <summary>
            /// 确认丢失
            /// </summary>
            LostError,
            /// <summary>
            /// 位填充错误
            /// </summary>
            BitFillError,
            /// <summary>
            /// CRC错误
            /// </summary>
            CRCError,
            /// <summary>
            /// 任意错误
            /// </summary>
            AnyError
        }
    }

    public class ProtocolLIN
    {
        //LIN
        //触发条件
        public enum Condition
        {
            Start,
            ID,
            Data,
            ID_DATA,
            WAKE_UP,
            SLEEP,
            SyncError,
            IDCRCError,
            SUMCRCError
        }

        //数据限定(触发条件选择"数据"或"ID和数据"时使用)
        public enum DataRelation
        {
            //Lt,
            //Gt,
            //Lteq,
            //Gteq,
            Eq
            //Neq,
            //In,
            //Out
        }

        //LIN协议版本
        public enum Standard
        {
            V1 = 1,
            V2 = 2,
            VBOTH = 3,
        }

        //位速率
        public enum BPS_ID
        {
            [Description("ZiDingYi")]
            BPS_Special,
            BPS_2400,
            BPS_4800,
            BPS_9600,
            BPS_19200,
        }

        //奇偶位
        public enum PIncludeOddEven
        {
            Y,
            N
        }
    }

    public class ProtocolFlexRay
    {
        //FlexRay
        //触发条件
        public enum Condition
        {
            FrameHead,
            Indicator,
            ID,
            Circulate,
            Header,
            Data = 5,
            IDAndData,
            FrameTail,
            Error
        }

        //源类型
        public enum SourceType
        {
            // Tx_Rx,
            BP,
            BM
            //BDiff
        }

        //信号速率
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            SignalRate_Custom,
            SignalRate_1Mbps,
            SignalRate_5Mbps,
            SignalRate_10Mbps,
        }

        //通道类型
        public enum ChannelType
        {
            A,
            B
        }

        //指示符（触发条件：指示符/包头）
        public enum Indicator
        {
            NormalFrame,   //正常帧
            PayloadFrame,  //净荷帧
            EmptyFrame,  //空帧
            SyncFrame,     //同步帧
            StartFrame,       //启动帧
            //HeaderCRCError = 0,   //正常帧
            //PayloadFrame = 1,  //净荷帧
            //StaticEmptyFrame = 2,  //静空帧
            //DynamicEmptyFrame = 3, //动空帧
            //SyncFrame = 4,     //同步帧
            //StartFram = 5      //启动帧
        }

        //限定符（触发条件：标识/数据/标识和数据）
        public enum Realtion
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        }

        //帧尾（触发条件：帧尾）
        public enum FrameTail
        {
            Static, //= 7,
            Dynamic,
            Any
        }

        //错误（触发条件：错误）
        public enum FrameError
        {
            /// <summary>
            /// 标头CRC错误
            /// </summary>
            PkgHeadCRC,// = 10,
            /// <summary>
            /// 帧尾CRC错误
            /// </summary>
            PkgTail,
            /// <summary>
            /// 空帧静态
            /// </summary>
            StaticEmptyFrame,
            /// <summary>
            /// 空帧动态
            /// </summary>
            DynamicEmptyFrame,
            /// <summary>
            /// 同步帧
            /// </summary>
            SyncFrame,
            /// <summary>
            /// 启动帧无同步
            /// </summary>
            StartFrame
        }
    }

    public class ProtocolAudioBus
    {
        //I2S
        //触发条件
        public enum Condition
        {
            Sync,
            Data,
        };

        /// <summary>
        /// TDM时的触发方式
        /// </summary>
        public enum Condition4TDM
        {
            /// <summary>
            /// 帧同步
            /// </summary>
            Sync,

            /// <summary>
            /// 数据
            /// </summary>
            Data,

            /// <summary>
            /// 通道+数据
            /// </summary>
            ChannelAndData,
        }
        //协议类型
        public enum SubType
        {
            I2S,
            LJ,
            RJ,
            TDM
        };
        //位顺序
        public enum MSB_LSB
        {
            MSB,
            LSB,

        };

        //数据限定(触发条件选择"数据"时使用)
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };

        //声道选择
        public enum SoundChannel
        {
            LeftOrRight,
            Left,
            Right
        };
    }

    public class ProtocolNRZ
    {
        public enum Condition
        {
            StartFrame,
            Data,
            EndFrame,
        }
        public enum MSB_LSB
        {
            MSB,
            LSB,
        };
        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };
        public enum Mode
        {
            NRZ,
            _8_10B,
            _64_66B,
        }
        public enum IdleLevel
        {
            Low,
            High,
        }
        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }
        public enum SignalRate
        {
            [Description("ZiDingYi")]
            Custom,
            Speed_650M,
            Speed_1G,
            Speed_6_5G,
        }

    }
    public class ProtocolSATA
    {
        //触发条件
        public enum Condition
        {
            ALIGN = 1,
            SYNC,
            X_RDY,
            SOF,
            FIS_TYPE,
            DATA,
            Hold,
            CRC,
            EOF,
            WTRM,
        }
        public enum SignalType
        {
            Single,//单端
            Difference//差分
        }
        public enum SATAVersion
        {
            SATA1_0,
            SATA2_0,
            SATA3_0,
        }

        //基元触发标志（触发条件选择“基元触发”时使用）
        public enum PrimitiveFlag
        {
            CONTp,
            DMATp,
            HOLDp,
            HOLDAp,
            PMACKp,
            PMNACKp,
            PMREQ_Pp,
            PMREQ_Sp,
            R_ERRp,
            R_IPp,
            R_OKp,
            R_RDYp,
            SYNCp,
            WTRMp,
            X_RDp
        }

        //类型触发标志（触发条件选择“类型触发”时使用）
        public enum FISTypeFlag
        {
            R_H2D = 1,
            R_D2H,
            DMA_Act,
            DMA_Set,
            Data,
            BIST,
            PIO,
            SDB,
            R_SATA,
            V_S,
        }

        public enum DataRelation
        {
            Lt,
            Gt,
            Lteq,
            Gteq,
            Eq,
            Neq,
            In,
            Out
        };

        public enum DecodeType
        {
            IDLE_state,
            D10_2_state,
            ALIGNp_state,
            SOFp_state,
            FIS_TYPE_state,
            FIS_DATA_state,
            CRC_state,
            EOFp_state,
            CONTp,
            DMATp,
            HOLDp,
            HOLDAp,
            PMACKp,
            PMNACKp,
            PMREQ_Pp,
            PMREQ_Sp,
            R_ERRp,
            R_IPp,
            R_OKp,
            R_RDYp,
            SYNCp,
            WTRMp,
            X_RDp
        }
    }

    public class ProtocolJTAG
    {
        //Jtag
        public enum Condition
        {
            RUN_TEST_IDLE = 1,
            SELECT_DR_SCAN,
            CAPTURE_DR,
            SHIFT_DR,
            EXIT1_DR,
            PAUSE_DR,
            EXIT2_DR,
            UPDAET_DR,
            SELECT_IR_SCAN,
            CAPTURE_IR,
            SHIFT_IR,
            EXIT1_IR,
            PAUSE_IR,
            EXIT2_IR,
            UPDATE_IR,
            //TEST_LOGIC_REST,
            //RESET = 0,
            //RUN_TEST_IDLE = 1,
            //SELECT_DR_SCAN = 2,
            //SELECT_IR_SCAN = 3,
            //CAPTURE_DR = 4,
            //CAPTURE_IR = 5,
            //SHIFT_DR = 6,
            //SHIFT_IR = 7,
            //EXIT1_DR = 8,
            //EXIT1_IR = 9,
            //PAUSE_DR = 10,
            //PAUSE_IR = 11,
            //EXIT2_DR = 12,
            //EXIT2_IR = 13,
            //UPDATE_DR = 14,
            //UPDATE_IR = 15
        }
        public enum BaudRateList
        {
            //待修改
            BaudRateList_0,
            BaudRateList_1
        }

        ////数据通道(触发条件选择"数据"触发时使用)（触发数据通道）
        //public enum   TriggerJtagDataChannel
        //{
        //	JtagDataChannel_TDI = 0,
        //	JtagDataChannel_TDO = 1
        //};
        //解码通道（解码数据通道）
        public enum DecodeChannel
        {
            TDI,
            TDO
        }

        //数据限定(触发条件选择"数据"或"ID和数据"时使用)
        public enum DataRelation
        {
            Eq,
            Gt,
            Lt,
        }
    }



    public class ProtocolSPMI
    {
        public enum Condition
        {
            WriteExRegister,
            ReadExRegister,
            WriteExRegisterLong,
            ReadExRegisterLong,
            WriteRegister,
            ReadRegister,
            MasterWrite,
            MasterRead,
            MasterWriteDD,
            MasterReadDD,
            WriteRegister0,
            Reset,
            Dormancy,
            Stop,
            Wakeup,
            Identify,
            BusRightTransfer,
            Parity_Error
        }

        public enum CheckType
        {
            Odd,
            Even
        }

        public enum Version
        {
            SMPI1,
            SMPI2
        }
    }
    public class ProtocolPSI5
    {
        public enum Condition
        {
            Start,
            DataA,
            DataB,
            Crc,
            Parity,
            Block_ID,
            Sensor_Status
        }
        //信号速率
        public enum Psi5BaudMode
        {
            Standard = 125000, // 125kbps
            Fast = 189000  // 189kbps
        }

        public enum Psi5FrameControl
        {
            Frame_0_Bit = 0,  // 0 bit
            Frame_1_Bit,     // 1 bit
            Frame_2_Bit,     // 2 bit
            Frame_3_Bit,     // 3 bit
            Frame_4_Bit,     // 4 bit
        }

        public enum Psi5Status
        {
            Status_0_Bit = 0, // 0 bit
            Status_1_Bit,     // 1 bit
            Status_2_Bit      // 2 bit
        }
        //串行通道是否打开
        public enum Psi5SerialMessage
        {
            OFF = 0,
            ON
        }

        //校验方式
        public enum Psi5CheckType
        {
            Check_Crc = 0, // 3 bit
            Check_P      // 1 bit
        };

        public enum Psi5FieldErrorStatus
        {
            FIELD_NO_ERROR = 0,
            FIELD_ERROR,
        };
    }
    public class ProtocolManchester
    {
        public enum MSB_LSB
        {
            MSB,
            LSB,
        };

        public enum OddEvenCheck
        {
            None = 0,
            Odd = 1, //奇校验
            Even = 2, //偶校验
        }
        public enum Polarity
        {
            Rising,
            Falling,
        }
        public enum DataView
        {
            Open,
            Close,
        }
        public enum Condition
        {
            Condition_NONE,
        }
    }

    public class ProtocolI3C
    {
        public enum Condition
        {
            Condition_NONE,
        }
    }
    public class ProtocolSMBus
    {
        public enum Condition
        {
            QuickCommand,
            SendByte,
            ReceiveByte,

            WriteByte,
            WriteWord,
            Write32,
            Write64,
            BlockWrite,

            ReadByteCommand,
            ReadByteResponse,
            ReadWordCommand,
            ReadWordResponse,
            Read32Command,
            Read32Response,
            Read64Command,
            Read64Response,
            BlockReadCommand,
            BlockReadResponse,

            BlockWirteBlockReadProcessCallCommand,
            BlockWirteBlockReadProcessCallResponse,
            ProcessCallCommand,
            ProcessCallResponse,

            HostNotifyProtocol,
            NotifyARPMaster,

            PrepareToARP,
            ResetDeviceGeneral,
            ResetDeviceDirected,
            AssignAddress,

            GetUdidGeneralCommand,
            GetUdidGeneralResponse,
            GetUdidDirectedCommand,
            GetUdidDirectedResponse,

            //Start,         // 开始条件
            //ReStart,       // 重复开始
            //Address,       // 地址
            //HostAddress,   // 主机地址
            //DeviceAddress, // 设备地址
            //CommandCode,   // 命令代码
            //Data,          // 数据
            //DataBytes,     // 数据字节数量 1-8 byte
            //DomainBytes,  //  UDID数据的域字节
            //UdidData,     //  UDID数据
            //ErrorType,    //  ANY ACK NACK PEC错误 PEC设置为True时 PEC搜索才可用
            //Stop,         //  停止
            //Idle,         //  空闲事件
        }

        public enum PECByte
        {
            Open,
            Close,
        }
    }
    public class ProtocolCXPI
    { 
        public enum Condition
        {
            Condition_NONE,
        }
    }
    public class Protocol8B10B
    {
        public enum Condition
        {
            Condition_NONE,
        }
    }
    public class ProtocolMlt3
    {
        public enum Condition
        {
            Condition_NONE,
        }
    }
    public class ProtocolEventInfo
    {
        public Int32 Index { get; set; }
        public Double StartTimeByPs { get; set; }
        public Double EndTimeByPs { get; set; }
        /// <summary>
        /// 开始位置的虚拟位置
        /// </summary>
        public Double StartPosition { get; set; }
        /// <summary>
        /// 开始位置的虚拟位置
        /// </summary>
        public Double EndPosition { get; set; }
       
        
        public List<(Byte[] Data, UInt32 BitCount)> EventInofs { get; } = new List<(Byte[] Data, UInt32 BitCount)>();

        public List<(UInt32 InfoIndex, Byte[] Data, UInt32 BitCount)> ExtraInfos { get; } = new List<(UInt32 InfoIndex, Byte[] Data, UInt32 BitCount)>();

        public override Boolean Equals(Object? obj)
        {
            if (obj is ProtocolEventInfo info)
            {
                return info.Index == Index && StartTimeByPs == info.StartTimeByPs && EventInofs.Except(info.EventInofs).Count() == 0;
            }
            else
            {
                return false;
            }
        }
        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
