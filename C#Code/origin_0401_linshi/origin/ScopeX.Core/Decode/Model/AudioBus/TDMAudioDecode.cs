using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use AudioBusDecodeModelCPP", true)]
    internal class TDMAudioDecode :AudioDecode
    {
        public override ProtocolAudioBus.SubType SubType => ProtocolAudioBus.SubType.TDM;
        public TDMAudioDecode(AudioBusDecodeModel model) : base(model)
        {
        }
        public override void ParsingData(ChannelId busId, ref CancellationToken token, ref Boolean needclear, ref List<AudioPacketInfo> packetInfos)
        {

            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(busId, _Model.SCL, ref datalen);
            Int32 wsendindex1 = 0;
            Int32 wsendindex2 = 0;
            Int32 clkstartindex = 0;
            Int32 clkendindex = 0;
            Boolean datainvert = _Model.DataPolarity != ProtocolCommon.Polarity.Positive; //hight
            Boolean wsstartstatus = _Model.SyncPolarity != ProtocolCommon.Polarity.Positive; //nomal
            Boolean clkstatus = _Model.ClockEdge == ProtocolCommon.Edge.Rise; //rise

            Int32 index = 0;
            Int32 next = 0;
            var starttime = TimeSpanUtility.GetTimestampSpan();
            while (true)
            {
                AudioPacketInfo packetInfo = new AudioPacketInfo();
                packetInfo.Channels = Enumerable.Range(0, _Model.SoundChannelCount).Select(x =>
                {
                    return new AudioChannelPacket()
                    {
                        SuccessBitCount = _Model.DataBitCount,
                        SuccessClkBitCount = _Model.ClockBitCount,
                        Value = new Byte[(Int32)Math.Ceiling(_Model.DataBitCount / 8.0)],
                    };
                }).ToArray();
                if (next == -1)
                    break;
                if(wsstartstatus)
                {
                    index = DecodeDataHelper.Instance.FindNextFallingEdge(busId, next, _Model.WS, ref token, ref needclear);
                    next = DecodeDataHelper.Instance.FindNextFallingEdge(busId, index + 1, _Model.WS, ref token, ref needclear);

                }
                else
                {
                    index = DecodeDataHelper.Instance.FindNextRisingEdge(busId, next, _Model.WS, ref token, ref needclear);
                    next = DecodeDataHelper.Instance.FindNextRisingEdge(busId, index +1, _Model.WS, ref token, ref needclear);
                }
                wsendindex1 = index;//ws帧起始位置
                wsendindex2 = next != -1 ? next : (Int32)datalen;

                if (DecodeDataHelper.Instance.GetLevel(busId, index, _Model.SCLThreshold, _Model.SCL) == clkstatus)
                {
                    index = DecodeDataHelper.Instance.FindNextEdge(busId, index, _Model.SCL, ref token, ref needclear);
                }
                index = DecodeDataHelper.Instance.FindNextEdge(busId, index, _Model.SCL, ref token, ref needclear);
                if (index == -1)
                    break;
                clkstartindex = index;
                clkendindex = wsendindex2;
                for (Int32 chindex = 0; chindex < packetInfo.Channels.Length; chindex++)
                {
                    clkstartindex = GetData(busId, ref packetInfo.Channels[chindex], ref token, ref needclear, clkstartindex, clkendindex, datainvert, _Model.BitDelayCount);
                }
#if DEBUG
                if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                {
                    return;
                }
#endif
                index = wsendindex2;
                packetInfos.Add(packetInfo);
            }
        }

        private Int32 GetData(ChannelId busId, ref AudioChannelPacket data, ref CancellationToken token, ref Boolean needclear, Int32 startindex, Int32 endindex, Boolean datainvert, Int32 skipbitcount)
        {

            data.Value = new Byte[(Int32)Math.Ceiling(data.SuccessBitCount / 8.0)];
            data.BitCount = 0;
            data.ClkBitCount = 0;
            for (Int32 bitindex = 0; bitindex < data.SuccessClkBitCount; bitindex++)
            {
                if (startindex > endindex || startindex == -1)
                {
                    if (data.Length == 0)
                    {
                        data.Length = endindex - data.Index;
                    }
                    break;
                }
                data.ClkBitCount++;
                if (bitindex < data.SuccessBitCount && bitindex + 1 >= skipbitcount)
                {
                    if (data.BitCount == 0)
                    {
                        data.Index = startindex;
                    }
                    data.BitCount++;
                    //if (data.BitCount == data.SuccessBitCount)
                    //{
                    //    data.Length = startindex - data.Index;
                    //}
                    Boolean status = DecodeDataHelper.Instance.GetLevel(busId, startindex, _Model.SDAThreshold, _Model.SDA, datainvert);
                    Int32 index = bitindex / 8;
                    Int32 bit = bitindex % 8;
                    data.HasData = true;
                    if (_Model.MSB_LSB == ProtocolAudioBus.MSB_LSB.MSB)
                    {
                        data.Value[index] = (Byte)(data.Value[index] << 1);
                        if (status) data.Value[index] |= (Byte)1;
                    }
                    else
                    {
                        if (status) data.Value[index] |= (Byte)(1 << bit);
                    }
                }
                startindex = DecodeDataHelper.Instance.FindNextEdge(busId, startindex, _Model.SCL, ref token, ref needclear);
                if (data.BitCount == data.SuccessBitCount)
                {
                    data.Length = startindex - data.Index;
                }
                startindex = DecodeDataHelper.Instance.FindNextEdge(busId, startindex, _Model.SCL, ref token, ref needclear);
            }
            return startindex;
        }

    }
}
