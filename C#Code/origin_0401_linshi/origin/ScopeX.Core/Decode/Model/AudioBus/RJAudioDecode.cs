using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use AudioBusDecodeModelCPP", true)]
    internal class RJAudioDecode : AudioDecode
    {
        public override ProtocolAudioBus.SubType SubType => ProtocolAudioBus.SubType.RJ;
        public RJAudioDecode(AudioBusDecodeModel model) : base(model)
        {
        }
        public override void ParsingData(ChannelId busId, ref CancellationToken token, ref Boolean needclear, ref List<AudioPacketInfo> packetInfos)
        {
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(busId, _Model.SCL, ref datalen);
            Int32 wsendindex = 0;
            Int32 clkstartindex = 0;
            Int32 clkendindex = 0;
            Boolean datainvert = _Model.DataPolarity != ProtocolCommon.Polarity.Positive; //hight
            Boolean wsstartstatus = _Model.SyncPolarity != ProtocolCommon.Polarity.Positive; //nomal
            Boolean clkstatus = _Model.ClockEdge == ProtocolCommon.Edge.Rise; //rise
            TwoLevelEdgeInfo? wsnode = DecodeDataHelper.Instance.GetEdgeInfo(busId, 0, _Model.WS, ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
            TwoLevelEdgeInfo? clknode = DecodeDataHelper.Instance.GetEdgeInfo(busId, 0, _Model.SCL, ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
            if (wsnode == null || clknode == null) return;
            Int32 index = 0;
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
                if (wsnode.CurrentLevel != wsstartstatus)
                {
                    wsnode = wsnode.Child as TwoLevelEdgeInfo;
                }
                if (wsnode == null) break;
                clknode = clknode.GetEdgeInfoByIndex(wsnode.StartIndex) as TwoLevelEdgeInfo;
                if (clknode == null) break;
                if (clknode.CurrentLevel == clkstatus)
                {
                    clknode = clknode?.Child?.Child as TwoLevelEdgeInfo;
                }
                else clknode = clknode?.Child as TwoLevelEdgeInfo;
                if (clknode == null) break;
                var endwsnode = wsnode?.Child as TwoLevelEdgeInfo;
                packetInfo.Channels[0].Index = clknode.StartIndex;
                if (endwsnode == null)
                {
                    GetData(busId, ref packetInfo.Channels[0].Value, ref packetInfo.Channels[0].BitCount, packetInfo.Channels[0].SuccessBitCount, clknode, null, datainvert);
                    packetInfo.Channels[0].HasData = true;
                    packetInfo.Channels[0].Length = (Int32)datalen - packetInfo.Channels[0].Index;
                    packetInfos.Add(packetInfo);
                    break;
                }
                else
                {
                    wsnode = endwsnode;
                    var endclknode = clknode?.GetEdgeInfoByIndex(endwsnode.StartIndex)! as TwoLevelEdgeInfo;
                    if (endclknode != null && endclknode!.CurrentLevel == clkstatus)
                    {
                        endclknode = endclknode?.Child?.Child as TwoLevelEdgeInfo;
                    }
                    else endclknode = endclknode?.Child as TwoLevelEdgeInfo;
                    GetData(busId, ref packetInfo.Channels[0].Value, ref packetInfo.Channels[0].BitCount, packetInfo.Channels[0].SuccessBitCount, clknode, endclknode, datainvert);
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
                if (endwsnode == null)
                {
                    GetData(busId, ref packetInfo.Channels[1].Value, ref packetInfo.Channels[1].BitCount, packetInfo.Channels[1].SuccessBitCount, clknode, null, datainvert);
                    packetInfo.Channels[1].HasData = true;
                    packetInfo.Channels[1].Length = (Int32)datalen - packetInfo.Channels[1].Index;
                    packetInfos.Add(packetInfo);
                    break;
                }
                else
                {
                    wsnode = endwsnode;
                    var endclknode = clknode?.GetEdgeInfoByIndex(endwsnode.StartIndex)! as TwoLevelEdgeInfo;
                    if (endclknode != null && endclknode!.CurrentLevel == clkstatus)
                    {
                        endclknode = endclknode?.Child?.Child as TwoLevelEdgeInfo;
                    }
                    else endclknode = endclknode?.Child as TwoLevelEdgeInfo;
                    GetData(busId, ref packetInfo.Channels[1].Value, ref packetInfo.Channels[1].BitCount, packetInfo.Channels[1].SuccessBitCount, clknode, endclknode, datainvert);
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
        private void GetData(ChannelId busId, ref Byte[] data, ref Int32 realbitcount, Int32 bitcount, TwoLevelEdgeInfo? startclknode, TwoLevelEdgeInfo? endclknode, Boolean datainvert)
        {
            if (startclknode == null) return;
            data = new Byte[(Int32)Math.Ceiling(bitcount / 8.0)];
            var bitr = bitcount % 8;
            realbitcount = 0;
            for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
            {
                if (startclknode == null) break;
                if (endclknode != null && startclknode.StartIndex > endclknode.StartIndex) break;
                realbitcount++;
                Boolean status = DecodeDataHelper.Instance.GetLevel(busId, startclknode.StartIndex, _Model.SDAThreshold, _Model.SDA, datainvert);
                Int32 index = (bitindex + bitr) / 8;
                Int32 bit = bitindex % 8;
                if (_Model.MSB_LSB == ProtocolAudioBus.MSB_LSB.MSB)
                {
                    //data[index] = (Byte)(data[index] << 1);
                    //if (status) data[index] |= (Byte)1;
                    if (bitr == 0)
                    {
                        data[index] = (Byte)(data[index] << 1);
                        if (status) data[index] |= (Byte)1;
                    }
                    else
                    {
                        if (bitindex < bitr)
                        {
                            data[0] = (Byte)(data[0] << 1);
                            if (status) data[index] |= (Byte)1;
                        }
                        else
                        {
                            data[index] = (Byte)(data[index] << 1);
                            if (status) data[index] |= (Byte)1;
                        }
                    }
                }
                else
                {
                    if (status) data[index] |= (Byte)(1 << bit);
                }
                startclknode = startclknode?.Child?.Child as TwoLevelEdgeInfo;
            }
        }
    }
}
