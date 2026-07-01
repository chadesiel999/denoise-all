using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use AudioBusDecodeModelCPP", true)]
    internal class I2SAudioDecode :AudioDecode
    {
        public override ProtocolAudioBus.SubType SubType => ProtocolAudioBus.SubType.I2S;
        public I2SAudioDecode(AudioBusDecodeModel model) : base(model)
        {
        }
        public override void ParsingData(ChannelId busId, ref CancellationToken token, ref Boolean needclear, ref List<AudioPacketInfo> packetInfos)
        {
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(busId, _Model.SCL, ref datalen);
            Boolean datainvert = _Model.DataPolarity != ProtocolCommon.Polarity.Positive; //hight
            Boolean wsstartstatus = _Model.SyncPolarity == ProtocolCommon.Polarity.Positive; //nomal
            Boolean clkstatus = _Model.ClockEdge == ProtocolCommon.Edge.Rise; //rise
            TwoLevelEdgeInfo? wsnode = DecodeDataHelper.Instance.GetEdgeInfo(busId, 0, _Model.WS, ref token, ref needclear) as TwoLevelEdgeInfo;
            TwoLevelEdgeInfo? clknode = DecodeDataHelper.Instance.GetEdgeInfo(busId, 0, _Model.SCL, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (wsnode == null || clknode == null) return;

            while (true)
            {
                AudioPacketInfo packetInfo = new AudioPacketInfo();
                packetInfo.Channels = new AudioChannelPacket[]
                {
                    new AudioChannelPacket()
                    {
                        Value = new Byte[(Int32)Math.Ceiling(_Model.DataBitCount / 8.0)],
                        SuccessBitCount = _Model.DataBitCount,
                        ClkBitCount = _Model.DataBitCount,
                        SuccessClkBitCount = _Model.DataBitCount,
                    },
                    new AudioChannelPacket()
                    {
                        Value = new Byte[(Int32)Math.Ceiling(_Model.DataBitCount / 8.0)],
                        SuccessBitCount = _Model.DataBitCount,
                        ClkBitCount = _Model.DataBitCount,
                        SuccessClkBitCount = _Model.DataBitCount,
                    }
                };
                if (wsnode == null || clknode == null) break;

                #region 寻找帧起始（I2S帧起始标识，在 WS 下降沿/上升沿之后第二个 CLK 上升沿/下降沿有效）
                if (wsnode.CurrentLevel != wsstartstatus)
                {
                    wsnode = wsnode.Child as TwoLevelEdgeInfo;
                }
                if (wsnode == null)
                    break;
                clknode = clknode.GetEdgeInfoByIndex(wsnode.EndIndex) as TwoLevelEdgeInfo;//找到对于的时钟node
                if (clknode == null)
                    break;
                if (clknode.CurrentLevel==clkstatus)//如果时钟node和需要的有效时钟电平相同
                {
                    clknode = clknode?.Child?.Child?.Child?.Child as TwoLevelEdgeInfo;
                }
                else
                {
                    clknode = clknode?.Child?.Child?.Child as TwoLevelEdgeInfo;
                }
                if (clknode == null)
                    break;
                var endwsnode = wsnode?.Child as TwoLevelEdgeInfo;
                if (endwsnode==null||endwsnode.Length < 0)
                    break;
                #endregion
                Int32 startindex= clknode.StartIndex;

                packetInfo.Channels[0].Index = startindex;
                if (endwsnode == null||endwsnode.Length<0)
                {
                   Int64 endindex=  GetData(busId, ref token,ref needclear, ref packetInfo.Channels[0].Value, ref packetInfo.Channels[0].BitCount, packetInfo.Channels[0].SuccessBitCount, clknode, null, datainvert);
                    packetInfo.Channels[0].HasData = true;
                    packetInfo.Channels[0].Length = (Int32)datalen - packetInfo.Channels[0].Index;
                    packetInfos.Add(packetInfo);
                    break;
                }
                else
                {
                    wsnode = endwsnode;
                    var endclknode = clknode?.GetEdgeInfoByIndex(endwsnode.EndIndex)! as TwoLevelEdgeInfo;
                    if (endclknode == null||endclknode.Length<0)
                        break;
                    //结束时钟信号node
                    if (endclknode.CurrentLevel == clkstatus)
                    {
                        endclknode = endclknode?.Child?.Child?.Child?.Child as TwoLevelEdgeInfo;
                    }
                    else
                    {
                        endclknode = endclknode?.Child?.Child?.Child as TwoLevelEdgeInfo;
                    }
                    GetData(busId, ref token, ref needclear,ref packetInfo.Channels[0].Value, ref packetInfo.Channels[0].BitCount, packetInfo.Channels[0].SuccessBitCount, clknode, endclknode, datainvert);
                    packetInfo.Channels[0].HasData = true;
                    if (endclknode == null)
                    {
                        packetInfo.Channels[0].Length = endwsnode.StartIndex - packetInfo.Channels[0].Index;
                        packetInfos.Add(packetInfo);
                        break;
                    }
                    packetInfo.Channels[0].Length = endclknode.StartIndex - packetInfo.Channels[0].Index;
                    clknode = endclknode;
                }
                endwsnode = wsnode?.Child as TwoLevelEdgeInfo;
                packetInfo.Channels[1].Index = clknode.StartIndex;
                if (endwsnode == null || endwsnode.Length < 0)
                {
                    GetData(busId, ref token, ref needclear, ref packetInfo.Channels[1].Value, ref packetInfo.Channels[1].BitCount, packetInfo.Channels[1].SuccessBitCount, clknode, null, datainvert);
                    packetInfo.Channels[1].HasData = true;
                    packetInfo.Channels[1].Length = (Int32)datalen - packetInfo.Channels[1].Index;
                    packetInfos.Add(packetInfo);
                    break;
                }
                else
                {
                    wsnode = endwsnode;
                    var endclknode = clknode?.GetEdgeInfoByIndex(endwsnode.EndIndex)! as TwoLevelEdgeInfo;
                    if (endclknode != null && endclknode.CurrentLevel == clkstatus)
                    {
                        endclknode = endclknode?.Child?.Child?.Child?.Child as TwoLevelEdgeInfo;
                    }
                    else endclknode = endclknode?.Child?.Child?.Child as TwoLevelEdgeInfo;
                    GetData(busId, ref token, ref needclear, ref packetInfo.Channels[1].Value, ref packetInfo.Channels[1].BitCount, packetInfo.Channels[1].SuccessBitCount, clknode, endclknode, datainvert);
                    packetInfo.Channels[1].HasData = true;
                    if (endclknode == null)
                    {
                        packetInfo.Channels[1].Length = endwsnode.StartIndex - packetInfo.Channels[1].Index;
                        packetInfos.Add(packetInfo);
                        break;
                    }
                    packetInfo.Channels[1].Length = endclknode.StartIndex - packetInfo.Channels[1].Index;
                    clknode = endclknode;
                }
                packetInfos.Add(packetInfo);
            }
        }
        private Int64 GetData(ChannelId busId, ref CancellationToken token, ref Boolean needclear,ref Byte[] data, ref Int32 realbitcount, Int32 bitcount, TwoLevelEdgeInfo? startclknode, TwoLevelEdgeInfo? endclknode, Boolean datainvert)
        {
            if (startclknode == null) return -1;
            data = new Byte[(Int32)Math.Ceiling(bitcount / 8.0)];
            realbitcount = 0;
            Int64 endindex = startclknode.StartIndex;
            for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
            {
                if (startclknode == null)
                    return -1;
                if (endclknode != null && startclknode.StartIndex > endclknode.StartIndex)
                {
                    break;
                }
                realbitcount++;
                Boolean status = DecodeDataHelper.Instance.GetLevel(busId, startclknode.StartIndex+startclknode.Length/2, _Model.SDAThreshold, _Model.SDA, datainvert);
                Int32 index = bitindex / 8;
                Int32 bit = bitindex % 8;
                if (_Model.MSB_LSB == ProtocolAudioBus.MSB_LSB.MSB)
                {
                    data[index] = (Byte)(data[index] << 1);
                    if (status) data[index] |= (Byte)1;
                }
                else
                {
                    if (status) data[index] |= (Byte)(1 << bit);
                }
                endindex = startclknode.StartIndex;
                startclknode = startclknode?.Child?.Child as TwoLevelEdgeInfo;
            }

            return endindex;
        }

    }
}
