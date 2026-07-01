using ScopeX.ComModel;
using ScopeX.Core.Decode.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal partial class EthernetDecodeModel
    {
        public enum EthernetEventType
        {
            NoData = 0,
            PhyLayerData, // 物理层数据
            LinkLayerData // 链路层数据
        }

        // 协议解码状态
        public enum StatusType
        {
            EthernetNoError = 0, //解码无错误

            PhyLayerMLT3SameAsPreLevel, //MLT3电平与上上一个电平值相等
            PhyLayerMLT3TooManyZeroLevel, //MLT3电平逻辑0过多
            PhyLayer5B4BDecodeError,     //5B->4B数据包编码错误

            LinkLayerFCSError,  //MAC帧校验错误

            NetLayerHeaderCheckSumError, //IP网络数据包头部校验错误

            TransLayerHeaderCheckSumError // 传输层数据包头部校验错误
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct EventField
        {
            public UInt64 StartIndex; // 事件字段起始索引
            public UInt64 Len;         // 事件字段长度
            public Byte HasData;            // 该字段是否有效
            public StatusType EventErorType;  // 字段错误类型，0-无错误
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct EventDataInfo
        {
            public UInt64 StartIndex; // 事件字段起始索引
            public UInt64 Len;         // 事件字段长度
            public Byte HasData;            // 该字段是否有效
            public StatusType event_error_type;  // 字段错误类型，0-无错误
            public Byte Data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EthernetEvent
        {
            public EventField Preamble;  // 前导码
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public Byte[] PreambleData;
            public EventField SSD;  // 帧起始符
            public Byte SSDData;
            public EventField DestMacAddress; // 目的MAC地址
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] DestMacAddressData;
            public EventField SrcMacAddress;  // 源MAC地址
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] SrcMacAddressData;
            public EventField CVlanTag; // 用户私网标签信息，内层，单层VLAN和双层VLAN有效
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] CVlanTagData;
            public EventField SVlanTag; // 用户公网标签信息，外层，双层VLAN有效
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] SVlanTagData;
            public EventField EthernetType; // 用户公网标签信息，外层，双层VLAN有效
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] EthernetTypeData;
            public EventField MacFrameLen_802_3;  //802.3帧有效，长度字段值
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] MacFrameLen_802_3Data;
            public EventField MacFrameInfo_802_3; // 802.3帧有效，控制信息，包含LLC子层的信息
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Byte[] MacFrameInfo_802_3Data;
            public EventField FcsCheckSum;                           // MAC帧校验和
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Byte[] FcsCheckSumData;
            public IntPtr FrameDatas; // [1530];MAC帧原始数据，最大长度包含前导码和帧开始符而计算
            public UInt32 FrameDatasCnt;  // 数据个数
            public EventField Esd; // 帧结束符
            public UInt32 ActualFcsCheckSum; // 通过计算得出的MAC帧校验和

            public EventDataInfo PhylayerInfos; // 物理层信息，无法进行链路层解码时会返回物理层的信息

            public EthernetEventType EventType; // 结果事件类型

            public void GetDataList(out List<EventDataInfo> dataList)
            {
                Int32 dataSize = Marshal.SizeOf(typeof(EventDataInfo));
                dataList = new List<EventDataInfo>();
                for (UInt32 pindex = 0; pindex < FrameDatasCnt; pindex++)
                {
                    IntPtr pdataptr = new IntPtr(FrameDatas.ToInt64() + pindex * dataSize);
                    EventDataInfo pdata = (EventDataInfo)Marshal.PtrToStructure(pdataptr, typeof(EventDataInfo));
                    dataList.Add(pdata);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EthernetResult
        {
            public IntPtr EventInfosPtr; // 结果事件列表
            public UInt32 EventCount; // 结果事件个数
            public SerialProtocolType ProtocolType; //协议类型
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EthernetOptions
        {
            public IntPtr CancelFlag;//取消解码标志
            public ProtocolEthernet.EthernetSpeed SignalRatelType;//以太网协议类型
            public ProtocolEthernet.SignalType SignalType;//信号类型
            public Byte Ipv4Flag; //ipv4数据包处理标志,true-处理，false-不处理
        };

        internal class EthernetResultPacket
        {
            public EthernetEvent PaketInfo;
            public List<EventDataInfo> EthDataInfos;
        };
    }
}
