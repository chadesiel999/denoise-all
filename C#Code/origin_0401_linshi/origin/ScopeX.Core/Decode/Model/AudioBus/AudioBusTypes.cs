using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolAudioBus;

namespace ScopeX.Core.Decode
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioBusTDMOption
    {
        public Int32 ClockBitNumberPreChannel;  // 每个通道时钟位个数
        public Int32 ChannelNumberPreFrame;   // 每个帧的通道数
        public Int32 BitDelay;      // 位延迟
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioBusOptions
    {
        public ProtocolAudioBus.SubType  AudioBusMode;   // audio bus 模式
        public ProtocolCommon.Polarity WsSyncPolarity; // 位选择极性
        public ProtocolCommon.Polarity DataPolarity;   // 数据极性
        public ProtocolCommon.Polarity ValidClockPolarity; // 时钟极性

        public ProtocolAudioBus.MSB_LSB ByteOrder; // 位顺序
        public ProtocolAudioBus.SoundChannel SoundChannel; // 声道类型

        public Int32 BitNumberPreChannel;

        public AudioBusTDMOption tDMOption;

        public IntPtr CancelFlag; //取消解码标志
    };

    public enum DecodeChannel
    {
        DECODECHANNEL_NONE,
        DECODECHANNEL_LEFT,  // 左声道
        DECODECHANNEL_RIGHT, // 右声道
        DECODECHANNEL_MULTI // 多通道
    };

    public struct AudioChannelCPP
    {
        public DecodeChannel DecodeChannel = DecodeChannel.DECODECHANNEL_NONE;
        public Int32 MultiChannelNum = 0;

        public Int32 DecodeDataCount = 0; // 实际解码数据
        public IntPtr DecodePtr = (IntPtr)0;

        public Int32 PaddingDataCount = 0;
        public IntPtr PaddingData = (IntPtr)0;

        public UInt64 ChannelStartIndex = 0; // packet起始位置
        public UInt64 ChannelEndIndex = 0;   // 结束位置

        public AudioChannelCPP()
        {
            DecodeChannel = DecodeChannel.DECODECHANNEL_NONE;
            MultiChannelNum = 0;
            DecodeDataCount = 0; // 实际解码数据
            DecodePtr = IntPtr.Zero;

            PaddingDataCount = 0;
            PaddingData = IntPtr.Zero;

            ChannelStartIndex = 0; // packet起始位置
            ChannelEndIndex = 0;   // 结束位置

        }
    }

    public struct AudioBusEventCPP
    {
        public UInt64 PacketStartIndex;
        public UInt64 PacketEndIndex;

        public IntPtr AudioChannelPtr;
        public Int32 ChannelNum;

        public AudioBusEventCPP()
        {
            PacketStartIndex = 0;
            PacketEndIndex = 0;

            AudioChannelPtr = IntPtr.Zero;
            ChannelNum = 0;
        }
    }

    public struct AudioBusResultInfoCPP
    {
        public IntPtr AudioBusEvent;
        public Int32 EventCount;
        public Int32 ProtocolType; // 协议类型
    };
}
