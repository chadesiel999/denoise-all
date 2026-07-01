using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core.Decode;

[Obsolete("Please Use Class 'CANDecodeModelCPP'", true)]
internal partial class CANDecodeModel : ProtocolModel
{
    const Int32 _MinFrameBitCount = 11 + 3 + 4 + 15;
    const Int32 _MaxContinuousBitCount = 5;
    readonly Int32 _CRCBitCount = 15;
    readonly Int32 _DLCBitcount = 4;
    readonly Int32 _EOFBitCount = 7;
    readonly Int32 _ExtpandIDBitCount = 18;
    readonly Int32 _MinEndFrameBitCount = 7;
    //readonly Int32 _OneBitcount = 1;
    readonly Int32 _StandardIDBitCount = 11;
    //readonly Double _Tolerance = 0.05;
    readonly BitManger _BitManger;
    List<CANFrameSpan> _CANFrameSpan = new();
    readonly DecodeResultData _DecodeResultData = new();
    //private List<Int32> _PadBitErrorIndexs = new();
    List<CANPacketInfo> _PacketInfos = new();

    public override HdMessage.IDecoderOptions GetProtocolRecoder()
    {
        return new HdMessage.ProtocolCANOptions
        {
            SamplePoint = SamplePoint,
            SDAThreshold = _SDAThreshold,
            SignalInput1 = Source1,
            SignalInput2 = Source2,
            SignalRate = CustomSignalRate,
            SignalType = SignalType
        };
    }

    override internal Boolean SourceHasData()
    {
        if (DsoPrsnt.DefaultDsoPrsnt == null)
            return false;

        DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out IChnlPrsnt? prsnt);
        if (prsnt == null)
            return false;

        if (Source1.IsReference() && prsnt.VuDatabase.Current != null)
        {
            return DecodeDataHelper.ReferenceHasData(Source1, _SDAThreshold);
        }

        if (Source1.IsAnalog())
        {
            return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
        }

        return false;
    }
    override internal Boolean CheckUpdate(ref Int64 laststamp)
    {
        //if(SignalInput1.IsAnalog())
        //{
        //    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
        //}
        //if (SignalInput1.IsReference())
        //{
        //    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[SignalInput1 - ChannelIdExt.MinRChId].TimeStamp;
        //}

        if (Source1.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
        {
            laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
            return true;
        }
        if (Source1.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].TimeStamp)
        {
            laststamp = DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].TimeStamp;
            return true;
        }

        return false;
    }

    public override void UpdateReferenceDataStatus()
    {
        if (_Source1.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels != null
            && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels[0] == _Source1)
        {
            DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].HasData = false;
        }
    }

    //24.6 ljw
    Boolean DecodeEOFFrame(ref Int32 edgeindex, ref CancellationToken token, ref Boolean needclear, ref CANPacketInfo packettmp)
    {
        packettmp.EOF = (byte)_BitManger.GetBits(BusId, edgeindex, _MinEndFrameBitCount, ref token, ref needclear,
            out Int32 len, out Boolean success, false);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.EOFIndex = edgeindex;
        packettmp.EOFLen = len;
        packettmp.HasEOF = true;

        return true;
    }

    Boolean DecodeCRCACKFrame(Int32 bitLenByTime, ref Int32 edgeindex, ref CancellationToken token, ref Boolean needclear, ref CANPacketInfo packettmp)
    {
        packettmp.SuccessCRC = BitConverter.GetBytes(_BitManger.CRC).Reverse().ToArray();
        packettmp.CRC = BitConverter.GetBytes((UInt16)_BitManger.GetBits(BusId, edgeindex, _CRCBitCount, ref token, ref needclear,
            out Int32 len, out Boolean success)).Reverse().ToArray();
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.CRCIndex = edgeindex;
        packettmp.CRCLen = len;
        packettmp.HasCRC = true;
        edgeindex += len;
        edgeindex += bitLenByTime;

        packettmp.ACK = _BitManger.GetBit(BusId, edgeindex, out success, ref token, ref needclear);
        if (!success)
        {
            return false;
        }
        packettmp.ACKIndex = edgeindex;
        packettmp.ACKLen = bitLenByTime;
        packettmp.HasACK = true;
        edgeindex += bitLenByTime;

        return true;
    }
    //解码  控制字段DLC + 数据字段 24.6 ljw
    Boolean DecodeCtrlDataFrame(Int32 bitLenByTime, ref Int32 edgeindex, ref CancellationToken token, ref Boolean needclear, ref CANPacketInfo packettmp)
    {
        packettmp.DLC = (byte)_BitManger.GetBits(BusId, edgeindex, _DLCBitcount, ref token, ref needclear, out Int32 len, out Boolean success);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.DLCIndex = edgeindex;
        packettmp.DLCLen = len;
        packettmp.HasDLC = true;
        edgeindex += len;
        //RTR 数据帧
        if (!packettmp.RTR)
        {
            if (packettmp.IDE)
            {
                packettmp.FrameType = FrameType.ExtendedDataFrame;
            }
            else
            {
                packettmp.FrameType = FrameType.StandardDataFrame;
            }
            List<DataInfo> dataInfos = new();
            for (Int32 dataindex = 0; dataindex < packettmp.DLC; dataindex++)
            {
                DataInfo dataInfo = new();
                //_BitManger.GetBits(edgeindex, 8, ref token, ref needclear, out len, out success);
                //if (len > (9 * bitLenByTime))
                //{
                //    return false;
                //}

                dataInfo.Data = (Byte)_BitManger.GetBits(BusId, edgeindex, 8, out List<Int32> padBitErrors, ref token, ref needclear, out len, out success);
                if (!success) break;
                if (padBitErrors.Count > 0)
                {
                    packettmp.FrameType = FrameType.PadBitError;
                    packettmp.FramePaddingErrorIndexs = padBitErrors;
                    return false;
                }
                if (len > (10 * bitLenByTime))
                {
                    return false;
                }
                if (len > (8 * bitLenByTime))
                {
                    edgeindex = _BitManger.BitIndex;
                    dataInfo.Index = edgeindex;
                    dataInfo.Len = len + 1;
                    edgeindex += len;
                    dataInfos.Add(dataInfo);
                    //edgeindex += bitLenByTime;
                }
                else
                {
                    edgeindex = _BitManger.BitIndex;
                    dataInfo.Index = edgeindex;
                    dataInfo.Len = len;
                    edgeindex += len;
                    dataInfos.Add(dataInfo);
                }

            }
            if (!success)
            {
                return false;
            }
            packettmp.DataInfos = dataInfos.ToArray();
            packettmp.HasData = packettmp.DataInfos.Length > 0;

        }
        else
        {
            packettmp.DLC = 0;
            if (packettmp.IDE)
            {
                packettmp.FrameType = FrameType.ExtendedRemoteFrame;
            }
            else
            {
                packettmp.FrameType = FrameType.StandardRemoteFrame;
            }
        }
        return true;
    }
    //解码/验证 扩展帧 24.6 ljw
    Boolean DecodeExtFrame(Int32 bitLenByTime, ref Int32 edgeindex, ref CancellationToken token, ref Boolean needclear, ref CANPacketInfo packettmp)
    {
        packettmp.HasRTR = false;
        packettmp.HasSRR = true;
        packettmp.SRR = packettmp.RTR;
        packettmp.SRRIndex = packettmp.RTRIndex;
        packettmp.SRRLen = packettmp.RTRLen;
        UInt64 tempid = packettmp.TempStandardID;
        _BitManger.GetBits(BusId, ref tempid, edgeindex, _ExtpandIDBitCount, ref token, ref needclear, out Int32 len, out Boolean success);
        packettmp.ExtID = BitConverter.GetBytes((UInt32)tempid).Reverse().ToArray();
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;

        packettmp.ExtIDIndex = edgeindex;
        packettmp.ExtIDLen = len;
        packettmp.HasExtID = true;
        edgeindex += len;

        packettmp.RTR = _BitManger.GetBit(BusId, edgeindex, out success, ref token, ref needclear);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.RTRIndex = edgeindex;
        packettmp.RTRLen = bitLenByTime;
        packettmp.HasRTR = true;
        edgeindex += bitLenByTime;

        packettmp.R1 = _BitManger.GetBit(BusId, edgeindex, out success, ref token, ref needclear);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.R1Index = edgeindex;
        packettmp.R1Len = bitLenByTime;
        packettmp.HasR1 = true;
        edgeindex += bitLenByTime;

        packettmp.R0 = _BitManger.GetBit(BusId, edgeindex, out success, ref token, ref needclear);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.R0Index = edgeindex;
        packettmp.R0Len = bitLenByTime;
        packettmp.HasR0 = true;
        edgeindex += bitLenByTime;

        return true;
    }

    //解码/验证 标准帧 24.6 ljw
    Boolean DecodeStandardFrame(Int32 bitLenByTime, Int32 edgeindex, ref CancellationToken token, ref Boolean needclear, ref CANPacketInfo packettmp)
    {
        packettmp.R0 = _BitManger.GetBit(BusId, edgeindex, out Boolean success, ref token, ref needclear);
        if (!success)
        {
            return false;
        }
        edgeindex = _BitManger.BitIndex;
        packettmp.R0Index = edgeindex;
        packettmp.R0Len = bitLenByTime;
        packettmp.HasR0 = true;

        return true;
    }

    //解码 / 纠错  ljw 24.6
    void DecodeAll(Int32 bitLenByTime, UInt32 datalenTotal, ref CancellationToken token, ref Boolean needclear)
    {
        if (!FindAllFrameSpans(bitLenByTime, datalenTotal, ref token))
        {
            return;
        }
        DecodeProcess(BusId, bitLenByTime, datalenTotal, ref token, ref needclear);
    }
    // ljw 24.6
    Boolean FindAllFrameSpans(Int32 bitLenByTime, UInt32 datalenTotal, ref CancellationToken token)
    {
        _CANFrameSpan = new List<CANFrameSpan>();
        Int32 edgeindex = 0;
        List<Int32> eopstarts = new();
        List<Int32> eopends = new();
        Boolean needclear = false;
        if (bitLenByTime <= 0 || datalenTotal == 0)
            return false;
        Int32 firstendindex;
        Int32 firststartindex;
        var eofbittimelen = bitLenByTime * _EOFBitCount;
        var starttime = TimeSpanUtility.GetTimestampSpan();
        if (_SignalType == ProtocolCAN.SignalType.CAN_H)
        {
            //初始显性
            if (DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source1, ref token, ref needclear)?.Edge == Edge.Rise)
            {
                firststartindex = DecodeDataHelper.Instance.FindNextRisingEdge(BusId, 0, Source1, ref token, ref needclear);
                firstendindex = DecodeDataHelper.Instance.FindNextFallingEdge(BusId, firststartindex + 1, Source1, ref token, ref needclear);
            }
            //初始隐性
            else
            {
                firststartindex = 0;
                //firstendindex = DecodeDataHelper.Instance.FindNextFallingEdge(1, SignalInput1, ref token, ref needclear) - (1 * bitLenByTime);
                firstendindex = DecodeDataHelper.Instance.FindNextRisingEdge(BusId, 1, Source1, ref token, ref needclear);
            }

        }
        else
        {
            //初始显性
            if (DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source1, ref token, ref needclear)?.Edge == Edge.Rise)
            {
                firststartindex = DecodeDataHelper.Instance.FindNextFallingEdge(BusId, 0, Source1, ref token, ref needclear);
                firstendindex = DecodeDataHelper.Instance.FindNextRisingEdge(BusId, firststartindex + 1, Source1, ref token, ref needclear);
            }
            //初始隐性
            else
            {
                firststartindex = 0;
                //firstendindex = DecodeDataHelper.Instance.FindNextRisingEdge(1, SignalInput1, ref token, ref needclear) - (1 * bitLenByTime);
                firstendindex = DecodeDataHelper.Instance.FindNextFallingEdge(BusId, 1, Source1, ref token, ref needclear);
            }

        }
        if ((firstendindex - firststartindex) > eofbittimelen)
        {
            eopstarts.Add(firststartindex);
            eopends.Add(firstendindex);
            edgeindex = firstendindex + 1;
        }
        if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
        {
            return false;
        }
        while (true)
        {
            //edgeindex = FindStartIndex(edgeindex, bitLenByTime, datalenTotal, ref token, ref needclear);//检测帧头
            //if (edgeindex < 0) break;
            Int32 tmpstartindex;

            if (_SignalType == ProtocolCAN.SignalType.CAN_H)
            {
                tmpstartindex = DecodeDataHelper.Instance.FindNextFallingEdge(BusId, edgeindex, Source1, ref token, ref needclear);
            }
            else
            {
                tmpstartindex = DecodeDataHelper.Instance.FindNextRisingEdge(BusId, edgeindex, Source1, ref token, ref needclear);
            }

            if (tmpstartindex < 0)
                break;
            Int32 tmpstopindex = DecodeDataHelper.Instance.FindNextEdge(BusId, tmpstartindex + 1, Source1, ref token, ref needclear);
            if (tmpstopindex < 0)
            {
                break;
            }
            Int32 tmpEdgeLen = (tmpstopindex - tmpstartindex) / bitLenByTime;
            if (tmpEdgeLen >= (_EOFBitCount + 2))
            {
                eopstarts.Add(tmpstartindex);
                eopends.Add(tmpstopindex);
            }
            edgeindex = tmpstartindex + 1;
            if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
            {
                return false;
            }
        }
        //找出最后一个EOP
        if (edgeindex < (datalenTotal - eofbittimelen))
        {
            //Int32 lasteopstartindex = -1;

            //if (_SignalType == ProtocolCAN.SignalType.CAN_H)
            //{

            //}
            //else
            //{

            //}
            Int32 lasteopstartindex = DecodeDataHelper.Instance.FindLastEdge(BusId, edgeindex, Source1, ref token, ref needclear);
            if (lasteopstartindex > 0 && (datalenTotal - lasteopstartindex) > eofbittimelen)
            {
                eopstarts.Add(lasteopstartindex);
                eopends.Add((Int32)(datalenTotal - 1));
            }
        }

        if (eopends.Count > 1)
        {
            for (Int32 i = 0; i < (eopends.Count - 1); i++)
            {
                if (_CANFrameSpan.Count > 0 && _CANFrameSpan[_CANFrameSpan.Count - 1].BitLen <= _MaxContinuousBitCount)
                {
                    _CANFrameSpan[_CANFrameSpan.Count - 1].EndIndex = eopends[i];
                }
                else
                {
                    _CANFrameSpan.Add(new CANFrameSpan(eopends[i], eopstarts[i + 1], bitLenByTime));
                }

            }
        }
        return _CANFrameSpan.Count > 0;
    }
    ////解码 
    void DecodeProcess(ChannelId busId, Int32 bitLenByTime, UInt32 datalenTotal, ref CancellationToken token, ref Boolean needclear)
    {
        _PacketInfos = new List<CANPacketInfo>();
        //_PadBitErrorIndexs = new();
        foreach (CANFrameSpan? frameSpan in _CANFrameSpan)
        {
            CANPacketInfo packettmp = new();
            _BitManger.Clear(busId, Source1, bitLenByTime);
            Int32 edgeindex = frameSpan.StartIndex;
            ///////////////// 检测到显著的低电平信号（SOF）后，开始解析仲裁字段。 /////////////////
            Int32 sofstart = 0;
            Int32 sofstop = 0;
            Boolean success;
            if (frameSpan.BitLen < _MinFrameBitCount)
            {
                packettmp.SOFIndex = edgeindex;
                packettmp.SOFLen = bitLenByTime;
                packettmp.FrameType = FrameType.ErrorFrame;
                packettmp.ErrorIndex = edgeindex + bitLenByTime;
                packettmp.ErrorLen = frameSpan.FrameSpanIndexLen - bitLenByTime;
                _PacketInfos.Add(packettmp);
                continue;
            }
            if (FindSOFIndex(edgeindex, bitLenByTime, datalenTotal, ref token, ref needclear, ref sofstart, ref sofstop) != -1)
            {
                edgeindex = sofstart;
                packettmp.SOF = _BitManger.GetBit(busId, edgeindex, out success, ref token, ref needclear);
                if (!success)
                {
                    break;
                }

                packettmp.SOFIndex = _BitManger.BitIndex;
                packettmp.SOFLen = sofstop - sofstart;
                edgeindex = sofstop;
            }
            else
            {
                //帧头不对 
                continue;
            }

            if ((edgeindex + (3 * bitLenByTime)) >= datalenTotal)
            {
                //数据不全
                break;
            }
            Int32 lenTmp;
            ///////////////// 仲裁字段 读取11位标识符 ///////////////// 
            packettmp.TempStandardID = _BitManger.GetBits(busId, edgeindex, _StandardIDBitCount, out List<Int32> padBitErrors, ref token, ref needclear, out lenTmp, out success);
            if (!success)
            {
                //解码错误 
                _PacketInfos.Add(packettmp);

                continue;
            }
            //ID填充错误
            if (padBitErrors.Count > 0)
            {
                foreach (Int32 bitError in padBitErrors)
                {
                    packettmp.FrameType = FrameType.PadBitError;

                    packettmp.FramePaddingErrorIndexs?.Add(bitError);

                    _PacketInfos.Add(packettmp);
                }

                continue;
            }
            packettmp.StandardID = BitConverter.GetBytes((UInt16)packettmp.TempStandardID).Reverse().ToArray();

            packettmp.HasStandardID = true;
            edgeindex = _BitManger.BitIndex;
            packettmp.StandardIDIndex = edgeindex;
            packettmp.StandardIDLen = lenTmp;
            edgeindex += lenTmp;
            if (edgeindex >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            ///////////////// 仲裁字段 检查第12位（RTR位）以确定数据帧或远程帧  ///////////////// 
            packettmp.RTR = _BitManger.GetBit(busId, edgeindex, out success, ref token, ref needclear);
            if (!success)
            {
                //解码错误 
                _PacketInfos.Add(packettmp);

                continue;
            }
            edgeindex = _BitManger.BitIndex;
            packettmp.RTRIndex = edgeindex;
            packettmp.RTRLen = bitLenByTime;
            packettmp.HasRTR = true;
            edgeindex += bitLenByTime;
            if (edgeindex >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            ///////////////// IDE位  标准帧 / 扩展帧 ///////////////// 
            packettmp.IDE = _BitManger.GetBit(busId,edgeindex, out success, ref token, ref needclear);
            if (!success)
            {
                //解码错误  
                _PacketInfos.Add(packettmp);

                continue;
            }
            edgeindex = _BitManger.BitIndex;
            packettmp.IDEIndex = edgeindex;
            packettmp.IDELen = bitLenByTime;
            packettmp.HasIDE = true;
            edgeindex += bitLenByTime;
            if (edgeindex >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            ///////////////// 解码标准帧  ///////////////// 
            if (!packettmp.IDE)
            {
                if ((edgeindex + bitLenByTime) > datalenTotal)
                {
                    //数据不全 部分显示
                    _PacketInfos.Add(packettmp);
                    break;
                }
                success = DecodeStandardFrame(bitLenByTime, edgeindex, ref token, ref needclear, ref packettmp);
                if (!success)
                {
                    //解码错误 
                    _PacketInfos.Add(packettmp);

                    continue;
                }
                edgeindex += bitLenByTime;
            }
            ///////////////// 解码扩展帧   ///////////////// 
            else
            {
                //扩展帧长度
                Int32 extframelen = (_ExtpandIDBitCount + 1) * bitLenByTime;
                if ((edgeindex + extframelen) >= datalenTotal)
                {
                    //数据不全 部分显示
                    _PacketInfos.Add(packettmp);
                    break;
                }
                success = DecodeExtFrame(bitLenByTime, ref edgeindex, ref token, ref needclear, ref packettmp);
                if (!success)
                {
                    //解码错误 回溯
                    _PacketInfos.Add(packettmp);
                    //edgeindex += 1;
                    continue;
                }
            }
            ///////////////// 解析  除IDE的控制字段 + 数据字段///////////////// 
            //控制字段DLC长度
            Int32 ctrlframelen = _DLCBitcount * bitLenByTime;
            if ((edgeindex + ctrlframelen) >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            success = DecodeCtrlDataFrame(bitLenByTime, ref edgeindex, ref token, ref needclear, ref packettmp);
            if (!success || packettmp.FrameType == FrameType.PadBitError)
            {
                //解码错误 
                _PacketInfos.Add(packettmp);

                continue;
            }

            ///////////////// CRC/ACK ///////////////// 
            //CRC/ACK 字段长度
            Int32 crcacklen = 3 * bitLenByTime;
            if ((edgeindex + crcacklen) >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            success = DecodeCRCACKFrame(bitLenByTime, ref edgeindex, ref token, ref needclear, ref packettmp);
            if (!success)
            {
                //解码错误 回溯
                _PacketInfos.Add(packettmp);
                //edgeindex += 1;
                continue;
            }
            ///////////////// 确认帧结束（EOF） ///////////////// 

            //EOF 字段长度
            Int32 eoflen = _EOFBitCount * bitLenByTime;
            if ((edgeindex + eoflen) >= datalenTotal)
            {
                //数据不全 部分显示
                _PacketInfos.Add(packettmp);
                break;
            }
            success = DecodeEOFFrame(ref edgeindex, ref token, ref needclear, ref packettmp);
            if (!success)
            {
                edgeindex += eoflen;
            }
            _PacketInfos.Add(packettmp);

        }

    }
    override internal void ParsingData(ref CancellationToken token)
    {
        Int32 chindex = GetChIndex(Source1);
        Double samplerate = 0;
        DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
        UInt32 datalen = 0;
        DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
        if (chindex == -1)
        {
            _NeedDecodeData = false;
            _NeedUpdateViewInfo = true;
            _PacketInfos.Clear();
        }
        Boolean needclear = false;
        try
        {
            if (_NeedDecodeData)
            {
                Int32 bitDataCount = (Int32)Math.Round((1d / CustomSignalRate) * samplerate, 0);

                _NeedDecodeData = false;
                _PacketInfos.Clear();
                if (bitDataCount >= 2)
                {
                    DecodeAll(bitDataCount, datalen, ref token, ref needclear);
                }
            }
        }
        catch
        {
            // ignored
        }

        ///////////////// 解码展示 /////////////////  ljw 24.6
        if (!_NeedUpdateViewInfo) return;
        _NeedUpdateViewInfo = false;
        List<DecodeResultData> buffer = GetDecodeBuffer();
        buffer.Clear();
        _EventInfos.Clear();
        if (_PacketInfos.Count == 0)
        {
            _DecodeResultData.DecodeViewInfos = Array.Empty<IDecodeViewInfo>();
            buffer.Add(_DecodeResultData);
            ChangeBuffer();
            return;
        }
        try
        {
            _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
            {
                String errorstr = "";

                _EventInfos.Add(new ProtocolEventInfo
                {
                    Index = _EventInfos.Count
                });
                _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(_ => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                List<CANDecodePacket> packets = new();
                {
                    CANSOFDecodePacket packet = new(CalcPosition(x.SOFIndex, Source1, chindex), CalcBitLenght(x.SOFLen, Source1, chindex));
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packet.Start, chindex);
                    //_EventInfos[^1].EventInofs[0] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }

                if (x.FrameType == FrameType.StandardRemoteFrame || x.FrameType == FrameType.StandardDataFrame)
                {
                    if (x.HasStandardID)
                    {
                        CANStandardIDDecodePacket packet = new(CalcPosition(x.StandardIDIndex, Source1, chindex), CalcBitLenght(x.StandardIDLen, Source1, chindex))
                        {
                            Data = x.StandardID
                        };
                        _EventInfos[^1].EventInofs[0] = (x.StandardID, (UInt32)_StandardIDBitCount);
                        packets.Add(packet);
                    }
                }
                else
                {
                    if (x.HasExtID)
                    {
                        CANExtandIDDecodePacket packet = new(CalcPosition(x.StandardIDIndex, Source1, chindex), CalcBitLenght((x.ExtIDIndex - x.StandardIDIndex) + x.ExtIDLen, Source1, chindex))
                        {
                            Data = x.ExtID
                        };
                        _EventInfos[^1].EventInofs[1] = (x.ExtID, (UInt32)(_ExtpandIDBitCount + 8));
                        packets.Add(packet);
                    }
                }
                switch (x.FrameType)
                {
                    case FrameType.ErrorFrame:
                        {
                            CANErrorFrameDecodePacket packet = new(CalcPosition(x.ErrorIndex, Source1, chindex), CalcBitLenght(x.ErrorLen, Source1, chindex))
                            {
                                BitCount = (UInt32)(x.ErrorLen / x.SOFLen)
                            };
                            errorstr += "Frame,";
                            //_EventInfos[^1].EventInofs[2] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                            return packets;
                        }
                    case FrameType.PadBitError:
                        {
                            packets.AddRange(x.FramePaddingErrorIndexs.Select(errorIndex => new CANErrorPadBitDecodePacket(CalcPosition(errorIndex, Source1, chindex), CalcBitLenght(x.SOFLen, Source1, chindex)) { BitCount = 0 }).Cast<CANDecodePacket>());
                            break;
                        }
                }
                if (x.HasDLC)
                {
                    CANDLCDecodePacket packet = new(CalcPosition(x.DLCIndex, Source1, chindex), CalcBitLenght(x.DLCLen, Source1, chindex))
                    {
                        Data = new[] { x.DLC }
                    };
                    _EventInfos[^1].EventInofs[2] = (packet.Data, (UInt32)_DLCBitcount);
                    packets.Add(packet);
                }

                if (x.HasData)
                {
                    packets.AddRange(x.DataInfos.Select(data => new CANDataDecodePacket(CalcPosition(data.Index, Source1, chindex), CalcBitLenght(data.Len, Source1, chindex)) { Data = new[] { data.Data } }).Cast<CANDecodePacket>());
                    _EventInfos[^1].EventInofs[3] = (x.DataInfos.Select(info => info.Data).ToArray(), (UInt32)x.DataInfos.Length * 8);
                }

                if (x.HasCRC)
                {
                    CANCRCDecodePacket packet = new(CalcPosition(x.CRCIndex, Source1, chindex), CalcBitLenght(x.CRCLen, Source1, chindex))
                    {
                        SuccessCRC = x.SuccessCRC,
                        Data = x.CRC
                    };
                    if (!packet.Success)
                    {
                        errorstr += "CRC,";
                    }
                    _EventInfos[^1].EventInofs[4] = (x.CRC, (UInt32)_CRCBitCount);
                    packets.Add(packet);
                }
                if (x.HasACK)
                {
                    CANACKDeocdePacket packet = new(CalcPosition(x.ACKIndex, Source1, chindex), CalcBitLenght(x.ACKLen, Source1, chindex))
                    {
                        Data = Encoding.Default.GetBytes(x.ACK ? "ACK1" : "ACK0"),
                        BitCount = 4
                    };
                    _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes(x.ACK ? "0" : "1"), 2);
                    if (x.ACK)
                    {
                        errorstr += "ACK,";
                    }
                    //_EventInfos[^1].EventInofs[6] = (Encoding.Default.GetBytes(x.ACK ? "False" : "True"), 5);
                    packets.Add(packet);
                }
                if (x.HasEOF)
                {
                    CANEOFDecodePacket packet = new(CalcPosition(x.EOFIndex, Source1, chindex), CalcBitLenght(x.EOFLen, Source1, chindex))
                    {
                        Data = new[] { x.EOF }
                    };
                    _EventInfos[^1].EventInofs[6] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }
                if (errorstr.Length > 0)
                {
                    errorstr = errorstr.Remove(errorstr.Length - 1, 1);
                    byte[] errorData = Encoding.Default.GetBytes(errorstr);
                    _EventInfos[^1].EventInofs[7] = (errorData, 0);
                }

                return packets;
            }).ToArray();
            buffer.Add(_DecodeResultData);
        }
        catch
        {
            // ignored
        }
        ChangeBuffer();
    }

    //24.6 ljw
    // Int32 FindStartIndex(Int32 dataindex, Int32 bitLenByTime, UInt32 datalen, ref CancellationToken token, ref Boolean needclear)
    // {
    //     if (bitLenByTime <= 0) return -1;
    //     if ((dataindex + bitLenByTime) >= datalen) return -1;
    //     Boolean invert = SignalType == ProtocolCAN.SignalType.CAN_H || SignalType == ProtocolCAN.SignalType.Diff;
    //     DateTime starttime = TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue); // DateTime.Now;
    //
    //     while (true)
    //     {
    //         Int32 tmpstartindex;
    //         //找到第一个边沿
    //         if (invert)
    //         {
    //             tmpstartindex = DecodeDataHelper.Instance.FindNextRisingEdge(dataindex, SignalInput1, ref token, ref needclear);
    //         }
    //         else
    //         {
    //             tmpstartindex = DecodeDataHelper.Instance.FindNextFallingEdge(dataindex, SignalInput1, ref token, ref needclear);
    //         }
    //
    //         if (tmpstartindex == -1) return -1;
    //         if ((tmpstartindex - dataindex) >= (_MinEndFrameBitCount * bitLenByTime))
    //         {
    //             Int32 nextedge = DecodeDataHelper.Instance.FindNextEdge(dataindex, SignalInput1, ref token, ref needclear);
    //             if (nextedge == -1) return -1;
    //             if (nextedge == tmpstartindex && (tmpstartindex - dataindex) >= (_MinEndFrameBitCount * bitLenByTime)) //如果下一个边沿是下降沿，表明波形第一个眼就是下降沿，且为帧间间隔
    //                 return tmpstartindex;
    //             if ((tmpstartindex - nextedge) >= (_MinEndFrameBitCount * bitLenByTime)) //如果下一个沿和下一个下降沿不是一个边沿，则说明下一个沿是上升沿，则判定两个边沿之间隔间是否满足帧间间隔
    //                 return tmpstartindex;
    //         }
    //         dataindex = tmpstartindex + 1;
    //         if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue) - starttime).TotalMilliseconds > 2000)
    //         {
    //             return -1;
    //         }
    //     }
    //
    // }

    //24.6 ljw
    Int32 FindSOFIndex(Int32 startIndex, Int32 bitLenByTime, UInt32 dataLen, ref CancellationToken token, ref Boolean needclear,
        ref Int32 sofstartIndex, ref Int32 sofstopIndex)
    {
        if (bitLenByTime <= 0)
            return -1;
        if ((startIndex + bitLenByTime) >= dataLen)
            return -1;
        //if (_SignalType == ProtocolCAN.SignalType.CAN_L)
        //{
        //    sofstartindex = DecodeDataHelper.Instance.FindNextFallingEdge(startIndex, SignalInput1, ref token, ref needclear);
        //}
        //else
        //{
        //    sofstartindex = DecodeDataHelper.Instance.FindNextRisingEdge(startIndex, SignalInput1, ref token, ref needclear);
        //}
        //if (sofstartindex == -1)
        //    return -1;
        //sofstartindex = startIndex + bitLenByTime; //step SOF
        sofstartIndex = startIndex; //step SOF
        sofstopIndex = startIndex + bitLenByTime;
        //仲裁段结尾
        Int32 arbitrationphaseendindex = -1;
        //check id len
        arbitrationphaseendindex = _SignalType == ProtocolCAN.SignalType.CAN_H
            ? DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startIndex, Source1, ref token, ref needclear)
            : DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startIndex, Source1, ref token, ref needclear);
        if (arbitrationphaseendindex > sofstartIndex + bitLenByTime * 12)
        {
            return -1;
        }
        else
        {
            return sofstartIndex;
        }

        //stopindex = DecodeDataHelper.Instance.FindNextEdge(sofstartindex + 1, SignalInput1, ref token, ref needclear);
        //if (stopindex == -1)
        //    return -1;
        //if ((stopindex - sofstartindex) >= (bitLenByTime * (1 - 0.05)) && (stopindex - sofstartindex) < (bitLenByTime * (1 + 0.05)))
        //{
        //    startIndex = sofstartindex;
        //    return startIndex;
        //}
        //return -1;
    }

    class BitManger
    {
        readonly UInt16 _Mask = 0x8000;
        readonly Int32 _MaxIdenticalBitCount = 5;
        readonly UInt16 _Poly = 0x4599;
        Int32 _Count;
        UInt16 _CRC;
        UInt32 _DataLength;
        Boolean _LastState;
        Int32 _OffsetSamplePoint;
        readonly CANDecodeModel _Parent;
        Double _SampleRate;

        internal BitManger(CANDecodeModel parent)
        {
            _Parent = parent;
        }
        public UInt16 CRC => (UInt16)(_CRC >> 1);
        [AllowNull] public ChannelId CurrentID { get; private set; }
        public Int32 BitDataCount { get; private set; }
        public Int32 BitIndex { get; private set; }
        void CalcCRC(Boolean state)
        {
            if (((_CRC ^ (state ? _Mask : 0)) & _Mask) != 0)
            {
                _CRC ^= _Poly;
            }
            _CRC <<= 1;
        }
        public void Clear(ChannelId busId,ChannelId ch, Int32 bitdatacount)
        {
            _CRC = 0;
            CurrentID = ch;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(busId, ch, ref _DataLength);
            DecodeDataHelper.Instance.TryGetSampleRate(busId, ch, ref _SampleRate);
            BitDataCount = bitdatacount;
            _Count = 0;
            _OffsetSamplePoint = (BitDataCount * _Parent.SamplePoint) / 100;
        }
        public Boolean GetBit(ChannelId busId, Int32 dataindex, out Boolean success, ref CancellationToken token, ref Boolean needclear)
        {
            if (_Count == _MaxIdenticalBitCount)
            {
                Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex, _Parent.SDAThreshold, CurrentID, _Parent.SignalType == ProtocolCAN.SignalType.CAN_H || _Parent.SignalType == ProtocolCAN.SignalType.Diff);
                if (tempstaus == _LastState)
                {
                    dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                }
                else
                {
                    dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                }
                if (dataindex == -1 || token.IsCancellationRequested || needclear)
                {
                    success = false;
                    return false;
                }
                _Count = 1;

                _LastState = !_LastState;
                dataindex += BitDataCount;
            }
            BitIndex = dataindex;
            Boolean state = GetPointBit(busId,dataindex, out success);
            if (!success) return false;
            if (state != _LastState)
            {
                _Count = 1;
                _LastState = state;
            }
            else _Count++;
            CalcCRC(state);
            return _LastState;
        }

        public void GetBits(ChannelId busId, ref UInt64 initialvalue, Int32 dataindex, Int32 bitcount, ref CancellationToken token, ref Boolean needclear, out Int32 len, out Boolean success, Boolean check = true)
        {
            if (bitcount > 64)
                throw new ArgumentOutOfRangeException(nameof(bitcount) + "max value is 64");
            success = false;
            len = 0;
            for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
            {
                if (_Count == _MaxIdenticalBitCount && check)
                {
                    Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex, _Parent.SDAThreshold, CurrentID, _Parent.SignalType == ProtocolCAN.SignalType.CAN_H || _Parent.SignalType == ProtocolCAN.SignalType.Diff);
                    if (tempstaus == _LastState)
                    {
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                    else
                    {
                        dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                    if (dataindex == -1 || token.IsCancellationRequested || needclear)
                    {
                        return;
                    }
                    _Count = 1;
                    _LastState = !_LastState;
                    dataindex += BitDataCount;
                    //_CRCBits.Add(!_LastState);
                }
                if (bitindex == 0)
                {
                    BitIndex = dataindex;
                }
                Boolean state = GetPointBit(busId,dataindex, out success);
                if (!success)
                {
                    return;
                }
                if (state != _LastState)
                {
                    _LastState = state;
                    _Count = 1;
                }
                else _Count++;
                CalcCRC(state);
                dataindex += BitDataCount;
                initialvalue <<= 1;
                initialvalue |= (UInt32)(state ? 1 : 0);
            }
            len = dataindex - BitIndex;
            success = len > 0;
        }
        public UInt64 GetBits(ChannelId busId, Int32 dataindex, Int32 bitcount, ref CancellationToken token, ref Boolean needclear, out Int32 len, out Boolean success, Boolean check = true)
        {
            return GetBits(busId,dataindex, bitcount, out List<Int32> _, ref token, ref needclear, out len, out success, check);
        }

        public UInt64 GetBits(ChannelId busId, Int32 dataindex, Int32 bitcount, out List<Int32> padBitErrors, ref CancellationToken token, ref Boolean needclear, out Int32 len, out Boolean success, Boolean check = true)
        {
            padBitErrors = new List<Int32>();
            if (bitcount > 64)
                throw new ArgumentOutOfRangeException(nameof(bitcount) + "max value is 64");
            success = false;
            len = 0;
            UInt64 val = 0;
            for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
            {
                if (_Count == _MaxIdenticalBitCount && check)
                {
                    Int32 tempNextStartDataIndex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    Int32 difDataCnt = tempNextStartDataIndex - dataindex;
                    if (difDataCnt < BitDataCount / 4)
                    {
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                    else
                    {
                        dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }

                    if (dataindex == -1 || token.IsCancellationRequested || needclear)
                    {
                        return val;
                    }
                    _Count = 1;
                    Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex + BitDataCount / 4,
                        _Parent.SDAThreshold,
                        CurrentID,
                        _Parent.SignalType == ProtocolCAN.SignalType.CAN_H || _Parent.SignalType == ProtocolCAN.SignalType.Diff);

                    _LastState = tempstaus;
                    // _CRC.AddBit(tempstaus);
                    dataindex += BitDataCount;
                }
                Int32 tempNextEdgeStartIndex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                if (tempNextEdgeStartIndex > 0)
                {
                    //若当前索引在边沿脉宽末尾，则当前索引应在下一个边沿开始
                    Int32 difCntFromNextEdge = tempNextEdgeStartIndex - dataindex;
                    if (difCntFromNextEdge < BitDataCount / 10)
                    {
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                }
                if (bitindex == 0)
                {
                    BitIndex = dataindex;
                }
                Boolean state = GetPointBit(busId,dataindex, out success);
                if (!success)
                {
                    return val;
                }
                if (state != _LastState)
                {
                    _LastState = state;
                    _Count = 1;
                }
                else _Count++;
                CalcCRC(state);
                dataindex += BitDataCount;
                val <<= 1;
                val |= (UInt32)(state ? 1 : 0);
            }

            len = dataindex - BitIndex;
            success = len > 0;
            return val;
        }
        Boolean GetPointBit(ChannelId busId, Int32 startindex, out Boolean success)
        {
            success = false;
            Int32 sampleindex = startindex + _OffsetSamplePoint;
            if (sampleindex >= _DataLength)
            {
                success = false;
                return false;
            }
            Boolean invert = _Parent.SignalType == ProtocolCAN.SignalType.CAN_H || _Parent.SignalType == ProtocolCAN.SignalType.Diff;

            if (_DataLength <= sampleindex) return false;
            Boolean result = DecodeDataHelper.Instance.GetLevel(busId, sampleindex, _Parent.SDAThreshold, CurrentID, invert);
            success = true;
            return result;
        }

        // Int32 GetNextEdge(Int32 startindex, ref CancellationToken token, ref Boolean needclear)
        // {
        //     DecodeDataHelper.Instance.GetLevel(startindex, _Parent.SDAThreshold, CurrentID);
        //     Int32 index = DecodeDataHelper.Instance.FindNextEdge(startindex, CurrentID, ref token, ref needclear);
        //     if (index == -1) return -1;
        //     return index;
        // }
    }
    #region 公有属性

    public CANDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.CAN, isTrigDecode)
    {
        _DecodeResultData.Name = "CAN";
        _BitManger = new BitManger(this);
    }


    public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>
    {
        "Index",
        "Start Time",
        //"SOF",
        "StandardID",
        "ExpandID",
        "DLC",
        "Data",
        "CRC",
        "ACK",
        "EOF",
        "ERROR"
    };


    public Double MaxThreshold => (Single)(12 * TryGetChannelGain(_Source1));
    public Double MinThreshold => -MaxThreshold;
    public Int64 MaxSignalRate => 1000_1000;
    public Int64 MinSignalRate => 10_000;


    public override Double BitRateByPs => (1f / CustomSignalRate) * 1E+12;
    //信号速率
    ProtocolCAN.SignalRate _SignalRate = ProtocolCAN.SignalRate.SignalRate_100k;
    public ProtocolCAN.SignalRate SignalRate
    {
        get => _SignalRate;
        set
        {
            if (value != _SignalRate)
            {
                if (ProtocolCAN.SignalRate.SignalRate_custom == value)
                {
                    UpdateProperty(ref _SignalRate, value);
                }
                _SignalRate = value;
                switch (value)
                {
                    case ProtocolCAN.SignalRate.SignalRate_10k:
                        CustomSignalRate = 10_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_20k:
                        CustomSignalRate = 20_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_33_3k:
                        CustomSignalRate = 33_300;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_50k:
                        CustomSignalRate = 50_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_62_5k:
                        CustomSignalRate = 62_500;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_83_3k:
                        CustomSignalRate = 83_300;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_100k:
                        CustomSignalRate = 100_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_125k:
                        CustomSignalRate = 125_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_1M:
                        CustomSignalRate = 1_000_000;
                        break;
                    case ProtocolCAN.SignalRate.SignalRate_custom:
                        break;
                }


            }
        }
    }

    //自定义的信号速率（当SignalRate == TriggerCANSignalRate.CANSignalRate_custom时使用）
    Int64 _CustomSignalRate = 250000;
    public Int64 CustomSignalRate
    {
        get => _CustomSignalRate;
        set
        {
            if (_CustomSignalRate != value)
            {
                switch (value)
                {
                    case 10_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_10k;
                        break;
                    case 20_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_20k;
                        break;
                    case 33_300:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_33_3k;
                        break;
                    case 50_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_50k;
                        break;
                    case 62_500:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_62_5k;
                        break;
                    case 83_300:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_83_3k;
                        break;
                    case 100_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_100k;
                        break;
                    case 125_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_125k;
                        break;
                    case 1_000_000:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_1M;
                        break;
                    default:
                        SignalRate = ProtocolCAN.SignalRate.SignalRate_custom;
                        break;
                }
                UpdateProperty(ref _CustomSignalRate, value);
            }
        }
    }

    //信号类型
    ProtocolCAN.SignalType _SignalType = ProtocolCAN.SignalType.CAN_H;
    public ProtocolCAN.SignalType SignalType
    {
        get => _SignalType;
        set => UpdateProperty(ref _SignalType, value);
    }

    //输入1
    ChannelId _Source1 = ChannelId.C1;
    public ChannelId Source1
    {
        get => _Source1;
        set => UpdateProperty(ref _Source1, value);
    }

    //输入2(信号类型选择"差分"时使用)
    ChannelId _Source2 = ChannelId.C1;
    public ChannelId Source2
    {
        get => _Source2;
        set => UpdateProperty(ref _Source2, value);
    }

    public Int32 MinSamplePoint => 30;


    public Int32 MaxSamplePoint => 90;
    //采样点
    Int32 _SamplePoint = 30;
    public Int32 SamplePoint
    {
        get => _SamplePoint;
        set => UpdateProperty(ref _SamplePoint, value);
    }
    Double _SDAThreshold = 1;
    /// <summary>
    ///     数据源的阈值
    /// </summary>
    public Double SDAThreshold
    {
        get => _SDAThreshold * TryGetChannelGain(Source1);
        set => UpdateProperty(ref _SDAThreshold, value / TryGetChannelGain(Source1));
    }

    public String SDAUnit => GetChannelUnit(Source1);

    #endregion
}
