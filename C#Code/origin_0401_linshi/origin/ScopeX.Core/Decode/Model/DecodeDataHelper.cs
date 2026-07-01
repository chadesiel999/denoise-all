using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using System.Net.Sockets;
using NPOI.POIFS.NIO;
using System.Reflection.Metadata.Ecma335;
using NPOI.XWPF.UserModel;
using System.Threading.Channels;
using ScopeX.Core.Tools;
using NPOI.Util.ArrayExtensions;
using SixLabors.ImageSharp.Memory;
using System.Runtime.InteropServices;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.DecoderTypes;
using NPOI.POIFS.Crypt.Dsig;
using MathNet.Numerics;

namespace ScopeX.Core.Decode
{
    public sealed class DecodeDataHelper
    {

        private class EdgeBuffer
        {
            [AllowNull]
            public BaseEdgeInfo First { get; set; }
            [AllowNull]
            public BaseEdgeInfo Last { get; set; }
            public void Clear()
            {
                First = null;
                Last = null;
                TimeStamp = 0;
            }
            public ChannelId Id;
            public Int32 StatusCount;
            public Int64 TimeStamp;
        }
        private class EdgeInfoBuffer
        {
            public List<EdgeBuffer> EdgeBuffers { get; } = new List<EdgeBuffer>();
            public Object Locker = new Object();
        }
        private DecodeDataHelper()
        {
            _AnalogEdgeBuffer = new EdgeInfoBuffer();
            _AnalogEdgeBuffer.Locker = new Object();

            _LAEdgeBuffer = new EdgeInfoBuffer();
            _LAEdgeBuffer.Locker = new Object();

            _ReferenceEdgeBuffer = new EdgeInfoBuffer();
            _ReferenceEdgeBuffer.Locker = new Object();

            _DifferEdgeBuffer = new EdgeInfoBuffer();
            _DifferEdgeBuffer.Locker = new Object();
        }

        private EdgeInfoBuffer _AnalogEdgeBuffer;
        private EdgeInfoBuffer _LAEdgeBuffer;
        private EdgeInfoBuffer _ReferenceEdgeBuffer;
        private EdgeInfoBuffer _DifferEdgeBuffer;

        public (Int32 SDAIndex, Int32 SCKIndex) FindI2CStartIndex(ChannelId busId, Int32 startindex, ChannelId clk, ChannelId sda, ref CancellationToken token, ref Boolean needclear)
        {
            FindNextEdge(ref DecodeDataSource.Instance.AnalogDataSources[busId - ChannelId.B1], ref _AnalogEdgeBuffer, startindex, clk, ref token, ref needclear);

            FindNextEdge(ref DecodeDataSource.Instance.AnalogDataSources[busId - ChannelId.B1], ref _AnalogEdgeBuffer, startindex, sda, ref token, ref needclear);
            if (_AnalogEdgeBuffer.EdgeBuffers.FindIndex(x => x.Id == clk && x.StatusCount == 2) >= 0 && _AnalogEdgeBuffer.EdgeBuffers.FindIndex(x => x.Id == sda && x.StatusCount == 2) >= 0)
            {
                var clkbuffer = _AnalogEdgeBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == clk && x.StatusCount == 2)?.First;
                var sdabuffer = _AnalogEdgeBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == sda && x.StatusCount == 2)?.First;
                if (clkbuffer == null || sdabuffer == null)
                    return (-1, -1);
                //var starttime = DateTime.Now;
                var starttime = TimeSpanUtility.GetTimestampSpan();
                while (true)
                {
                    sdabuffer = sdabuffer.GetEdgeInfoByIndex(startindex);
                    if (sdabuffer == null)
                        return (-1, -1);
                    if (sdabuffer.StartIndex < startindex)
                        sdabuffer = sdabuffer.Child;
                    if (sdabuffer == null)
                        return (-1, -1);
                    if (sdabuffer.Edge != Edge.Falling)
                        sdabuffer = sdabuffer.Child;
                    if (sdabuffer == null)
                        return (-1, -1);
                    clkbuffer = clkbuffer.GetEdgeInfoByIndex(sdabuffer.StartIndex);
                    if (clkbuffer == null)
                        return (-1, -1);
                    if (sdabuffer.StartIndex > clkbuffer.StartIndex)
                        clkbuffer = clkbuffer.Child;
                    if (clkbuffer == null)
                        return (-1, -1);
                    if (clkbuffer.Edge == Edge.Falling)
                    {
                        if (sdabuffer.StartIndex <= clkbuffer.StartIndex)
                            return (sdabuffer.StartIndex, clkbuffer.StartIndex);
                        //return (sdabuffer.StartIndex, clkbuffer.StartIndex);
                    }

                    startindex = clkbuffer.EndIndex + 1;
                    if ((TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                    {
                        return (-1, -1);
                    }
                }
            }
            else
            {
                return (-1, -1);
            }
        }
        public (Int32 SDAIndex, Int32 CLKIndex) FindI2CStopIndex(ChannelId busId, Int32 startindex, ChannelId clk, ChannelId sda, ref CancellationToken token, ref Boolean needclear)
        {
            FindNextEdge(ref DecodeDataSource.Instance.AnalogDataSources[busId - ChannelId.B1], ref _AnalogEdgeBuffer, startindex, clk, ref token, ref needclear);

            FindNextEdge(ref DecodeDataSource.Instance.AnalogDataSources[busId - ChannelId.B1], ref _AnalogEdgeBuffer, startindex, sda, ref token, ref needclear);
            if (_AnalogEdgeBuffer.EdgeBuffers.FindIndex(x => x.Id == clk && x.StatusCount == 2) >= 0 && _AnalogEdgeBuffer.EdgeBuffers.FindIndex(x => x.Id == sda && x.StatusCount == 2) >= 0)
            {
                var clkbuffer = _AnalogEdgeBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == clk && x.StatusCount == 2)?.First;
                var sdabuffer = _AnalogEdgeBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == sda && x.StatusCount == 2)?.First;
                if (clkbuffer == null || sdabuffer == null)
                    return (-1, -1);
                var starttime = TimeSpanUtility.GetTimestampSpan();
                while (true)
                {
                    sdabuffer = sdabuffer.GetEdgeInfoByIndex(startindex);
                    if (sdabuffer == null)
                        return (-1, -1);
                    if (sdabuffer.StartIndex < startindex)
                        sdabuffer = sdabuffer.Child;
                    if (sdabuffer == null)
                        return (-1, -1);
                    if (sdabuffer.Edge != Edge.Rise)
                        sdabuffer = sdabuffer.Child;
                    if (sdabuffer == null)
                        return (-1, -1);
                    clkbuffer = clkbuffer.GetEdgeInfoByIndex(sdabuffer.StartIndex);
                    if (clkbuffer == null)
                        return (-1, -1);
                    if (sdabuffer.StartIndex > clkbuffer.StartIndex)
                        clkbuffer = clkbuffer.Child;
                    if (clkbuffer == null)
                        return (-1, -1);
                    if (clkbuffer.Edge == Edge.Falling)
                    {
                        if (sdabuffer.StartIndex <= clkbuffer.StartIndex)
                            return (sdabuffer.StartIndex, clkbuffer.StartIndex);
                        //return (sdabuffer.StartIndex, clkbuffer.StartIndex);
                    }

                    startindex = clkbuffer.EndIndex + 1;
                    if ((TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                    {
                        return (-1, -1);
                    }
                }

            }
            else
            {
                return (-1, -1);
            }
        }
        public ComModel.DeocodeDataSourcePacket[] AnalogDataSources => DecodeDataSource.Instance.AnalogDataSources;
        public ComModel.DeocodeDataSourcePacket? LADataSource => DecodeDataSource.Instance.LADataSource;
        public ComModel.DeocodeDataSourcePacket[] ReferenceDataSource => DecodeDataSource.Instance.ReferenceDataSource;
        public ComModel.DeocodeDataSourcePacket[] DiffDataSource => DecodeDataSource.Instance.DiffDataSource;
        public static DecodeDataHelper Instance { get; } = new DecodeDataHelper();

        public static Boolean ReferenceHasData(ChannelId id, params Double[] thresholds)
        {
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt);
            if (DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].Channels == null
                || DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].Channels[0] != id
                || !DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].HasData
                || (prsnt as ReferencePrsnt).TriggerIndex != DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].TriggerIndex)
            {
                UpdaterReferenceData(id, thresholds);
            }

            if (!prsnt.Active && DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].Channels[0] == id)
            {
                DecodeDataSource.Instance.RemoveReferenceData(id);
            }
            return DecodeDataSource.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].HasData;
        }

        public static void UpdaterReferenceData(ChannelId id, params Double[] thresholds)
        {
            if (!id.IsReference())
            {
                return;
            }
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt);
            if (prsnt == null || prsnt.VuDatabase == null || prsnt.VuDatabase.Current == null || prsnt is not ReferencePrsnt)
            {
                return;
            }

            ReferencePrsnt refprsnt = prsnt as ReferencePrsnt;
            var index = id - ChannelIdExt.MinRChId;
            DeocodeDataSourcePacket @ref = DecodeDataSource.Instance.ReferenceDataSource[index];
            //var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(id, true);
            //var Length = 0;
            //var range = datarange.End.Value - datarange.Start.Value;
            //if (range < 0)
            //{
            //    Length = 0;
            //}
            //else
            //{
            //    Length = range;
            //}

            if (@ref.Channels == null || @ref.Channels[0] != id)
            {
                @ref = new DeocodeDataSourcePacket() { Channels = new ChannelId[1] { id } };
            }
            @ref.SampleRate = 1 / refprsnt!.Pack!.Properties.SampInterval;
            @ref.TriggerIndex = refprsnt!.TriggerIndex;
            @ref.MaxByteCount = (UInt32)refprsnt!.Pack!.Buffer.GetLength(1);
            @ref.PerChannelDataLength = (UInt32)refprsnt!.Pack!.Buffer.GetLength(1);
            @ref.TimeStamp = DateTime.Now.Ticks;
            @ref.HasData = true;
            DecodeDataSource.Instance.ReferenceDataSource[index] = @ref;

            Double[] data = new Double[@ref.PerChannelDataLength];
            Buffer.BlockCopy(refprsnt!.Pack!.Buffer, 0, data, 0, data.Length * sizeof(Double));
            for (Int32 i = 0; i < thresholds.Length; i++)
            {
                thresholds[i] = Quantity.ConvertByPrefix(thresholds[i], Prefix.Empty, refprsnt!.Pack!.Properties.ChnlUnit.Prefix);
            }
            @ref.ChannelDataSource = thresholds.Length switch
            {
                1 => SignalShaping(data, thresholds[0]).ToArray(),
                2 => SignalShaping(data, thresholds[0], thresholds[1]).ToArray(),
                3 => SignalShaping(data, thresholds[0], thresholds[1], thresholds[2]).ToArray(),
                _ => SignalShaping(data, 0).ToArray()
            };
            DecodeDataSource.Instance.ReferenceDataSource[index] = @ref;
            Instance._ReferenceEdgeBuffer = new EdgeInfoBuffer();
        }

        public static void UpdaterCPHYDiffData(ChannelId idA, ChannelId idB, ChannelId idC, params Double[] thresholds)
        {
            if (!idA.IsReference() || !idB.IsReference() || !idC.IsReference() || thresholds.Length != 3) return;
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(idA, out var prsnta);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(idB, out var prsntb);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(idC, out var prsntc);
            if (prsnta == null || prsnta.VuDatabase == null || prsnta.VuDatabase.Current == null || prsnta is not ReferencePrsnt) return;
            if (prsntb == null || prsntb.VuDatabase == null || prsntb.VuDatabase.Current == null || prsntb is not ReferencePrsnt) return;
            if (prsntc == null || prsntc.VuDatabase == null || prsntc.VuDatabase.Current == null || prsntc is not ReferencePrsnt) return;
            ReferencePrsnt refprsnta = prsnta as ReferencePrsnt;
            ReferencePrsnt refprsntb = prsntb as ReferencePrsnt;
            ReferencePrsnt refprsntc = prsntc as ReferencePrsnt;
            var adata = refprsnta!.Pack!.Buffer;
            var adatb = refprsntb!.Pack!.Buffer;
            var adatc = refprsntc!.Pack!.Buffer;
            if (adata.Length != adatb.Length || adata.Length != adatc.Length)
            {
                return;
            }

            //var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(idA, true);
            //var length = 0;
            //var range = datarange.End.Value - datarange.Start.Value;
            //if (range < 0)
            //{
            //    length = 0;
            //}
            //else
            //{
            //    length = range;
            //}

            Int32 length = adata.Length;
            Double[][] datadiff = new Double[3][];
            for (Int32 i = 0; i < datadiff.Length; i++)
            {
                datadiff[i] = new Double[length];
            }
            Parallel.For(0, length, i =>
            {
                datadiff[0][i] = adata[0, i] - adatb[0, i];
                datadiff[1][i] = adatb[0, i] - adatc[0, i];
                datadiff[2][i] = adatc[0, i] - adata[0, i];
            });
            for (Int32 i = 0; i < thresholds.Length; i++)
            {
                thresholds[i] = Quantity.ConvertByPrefix(thresholds[i], Prefix.Empty, refprsnta!.Pack!.Properties.ChnlUnit.Prefix);
            }
            ChannelId[] chs = { idA, idB, idC };
            Parallel.For(0, 3, i =>
            {
                DeocodeDataSourcePacket @diff = DecodeDataSource.Instance.ReferenceDataSource[chs[i] - ChannelIdExt.MinRChId];
                if (@diff.Channels == null || @diff.Channels[0] != chs[i])
                {
                    @diff = new DeocodeDataSourcePacket() { Channels = new ChannelId[1] { chs[i] } };
                    @diff.SampleRate = 1 / refprsnta!.Pack!.Properties.SampInterval;
                }
                @diff.TriggerIndex = (length + 1) / 2;
                @diff.MaxByteCount = (UInt32)length;
                @diff.PerChannelDataLength = (UInt32)length;
                @diff.TimeStamp = DateTime.Now.Ticks;
                @diff.HasData = true;
                DecodeDataSource.Instance.DiffDataSource[chs[i] - ChannelIdExt.MinRChId] = @diff;

                @diff.ChannelDataSource = SignalShaping(datadiff[i], thresholds[i], 0).ToArray();
                DecodeDataSource.Instance.DiffDataSource[chs[i] - ChannelIdExt.MinRChId] = @diff;
            });

        }

        public static void UpdaterReferenceDataByRange(ChannelId id, Double thresholds, Double toleranceRange = 0.6)
        {
            if (!id.IsReference())
            {
                return;
            }
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt);
            if (prsnt == null || prsnt.VuDatabase == null || prsnt.VuDatabase.Current == null || prsnt is not ReferencePrsnt)
            {
                return;
            }

            ReferencePrsnt refprsnt = prsnt as ReferencePrsnt;
            var index = id - ChannelIdExt.MinRChId;
            DeocodeDataSourcePacket @ref = DecodeDataSource.Instance.ReferenceDataSource[index];
            var datarange = DsoModel.Default.Cursors.VCursor.GetRangeBetweenAllIndexs(id, true);
            var length = 0;
            var range = datarange.End.Value - datarange.Start.Value;
            if (range < 0)
            {
                length = 0;
            }
            else
            {
                length = range;
            }

            if (@ref.Channels == null || @ref.Channels[0] != id)
            {
                @ref = new DeocodeDataSourcePacket() { Channels = new ChannelId[1] { id } };
                @ref.SampleRate = 1 / refprsnt!.Pack!.Properties.SampInterval;
            }
            @ref.TriggerIndex = refprsnt!.TriggerIndex;
            @ref.MaxByteCount = (UInt32)length;
            @ref.PerChannelDataLength = (UInt32)length;
            @ref.TimeStamp = DateTime.Now.Ticks;
            @ref.HasData = true;
            DecodeDataSource.Instance.ReferenceDataSource[index] = @ref;

            Double[] data = new Double[length];
            Buffer.BlockCopy(refprsnt!.Pack!.Buffer, 0, data, 0, data.Length * sizeof(Double));

            thresholds = Quantity.ConvertByPrefix(thresholds, Prefix.Empty, refprsnt!.Pack!.Properties.ChnlUnit.Prefix);

            @ref.ChannelDataSource = SignalShapingByRange(data, thresholds, toleranceRange).ToArray();
            DecodeDataSource.Instance.ReferenceDataSource[index] = @ref;
        }

        /// <summary>
        /// 波形信号数据整形（双电平）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold">阈值数组</param>
        /// <returns></returns>
        public static List<Byte> SignalShaping(Double[] data, Double threshold)
        {
            List<Byte> result = new List<Byte>();
            foreach (Double item in data)
            {
                if (item >= threshold)
                {
                    result.Add(255);
                }
                else
                {
                    result.Add(0);
                }
            }
            return result;
        }
        /// <summary>
        /// 波形信号数据宽范围整形（双电平）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold">阈值数组</param>
        /// <returns></returns>
        public static List<Byte> SignalShapingByRange(Double[] data, Double threshold, Double toleranceRange = 0.6)
        {
            List<Byte> result = new List<Byte>();
            Byte lastData = 0;
            result.Add(0);
            if (data[0] <= threshold - toleranceRange)
            {
                lastData = 1;
                result.Add(1);
            }
            foreach (Double item in data)
            {
                if (item >= threshold + toleranceRange)
                {
                    result.Add(255);
                    lastData = 255;
                }
                else if (item < threshold + toleranceRange && item > threshold - toleranceRange)
                {
                    result.Add(lastData);
                }
                else // item <= threshold- range
                {
                    result.Add(0);
                    lastData = 0;
                }
            }
            return result;
        }

        /// <summary>
        /// 波形信号数据整形（三电平）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold">阈值数组</param>
        /// <returns></returns>
        public static List<Byte> SignalShaping(Double[] data, Double highthreshold, Double lowthreshold)
        {
            List<Byte> result = new List<Byte>();
            foreach (Double item in data)
            {
                if (item >= highthreshold)
                {
                    result.Add(255);
                }
                else if (item < highthreshold && item >= lowthreshold)
                {
                    result.Add(127);
                }
                else
                {
                    result.Add(0);
                }
            }
            return result;
        }

        /// <summary>
        /// 波形信号数据整形（四电平）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold">阈值数组</param>
        /// <returns></returns>
        public static List<Byte> SignalShaping(Double[] data, Double highthreshold, Double midthreshold, Double lowthreshold)
        {
            List<Byte> result = new List<Byte>();
            foreach (Double item in data)
            {
                if (item >= highthreshold)
                {
                    result.Add(255);
                }
                else if (item < highthreshold && item >= midthreshold)
                {
                    result.Add(169);//169-255
                }
                else if (item < midthreshold && item >= lowthreshold)
                {
                    result.Add(84);//84-169
                }

                else if (item < midthreshold && item >= lowthreshold)
                {
                    result.Add(0);//0-83
                }
            }
            return result;
        }



        private Int32 FindNextEdge(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer edgeInfoBuffer, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            //if(token.IsCancellationRequested || needclear)
            //{
            //    needclear = true;
            //    return -1;
            //}
            if (!datasource.Channels.Contains(id) || startindex >= datasource.PerChannelDataLength)
                return -1;
            if (!datasource.HasData)
            {
                needclear = true;
                return -1;
            }
            lock (edgeInfoBuffer.Locker)
            {
                Int64 time = datasource.TimeStamp;
                Int32 index = edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == time && x.First != null && x.Last != null && x.StatusCount == 2);
                if (index >= 0)
                {

                }
                else
                {
                    AnalysisDecodeData(ref datasource, ref edgeInfoBuffer, id, ref token, ref needclear);
                    index = edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == time && x.First != null && x.Last != null && x.StatusCount == 2);
                }
                if (index == -1)
                    return -1;
                if (edgeInfoBuffer.EdgeBuffers[index].First != null && edgeInfoBuffer.EdgeBuffers[index].Last != null)
                {
                    var temp = edgeInfoBuffer.EdgeBuffers[index].First.GetEdgeInfoByIndex(startindex);
                    if (temp != null && temp.Child != null)
                        return temp.Child.StartIndex;
                    return -1;
                }
                else
                {
                    return -1;
                }
            }
        }

        private Int32 FindLastEdge(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer edgeInfoBuffer, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            //if(token.IsCancellationRequested || needclear)
            //{
            //    needclear = true;
            //    return -1;
            //}
            if (!datasource.Channels.Contains(id) || startindex >= datasource.PerChannelDataLength)
                return -1;
            if (!datasource.HasData)
            {
                needclear = true;
                return -1;
            }
            lock (edgeInfoBuffer.Locker)
            {
                Int64 time = datasource.TimeStamp;
                Int32 index = edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == time && x.First != null && x.Last != null && x.StatusCount == 2);
                if (index >= 0)
                {

                }
                else
                {
                    AnalysisDecodeData(ref datasource, ref edgeInfoBuffer, id, ref token, ref needclear);
                    index = edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == time && x.First != null && x.Last != null && x.StatusCount == 2);
                }
                if (index == -1)
                    return -1;
                if (edgeInfoBuffer.EdgeBuffers[index].First != null && edgeInfoBuffer.EdgeBuffers[index].Last != null)
                {
                    var temp = edgeInfoBuffer.EdgeBuffers[index].First.GetEdgeInfoByIndex(startindex);
                    if (temp != null)
                        return temp.StartIndex;
                    return -1;
                }
                else
                {
                    return -1;
                }
            }
        }
        private void AnalysisThreeLevelDecodeData(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer buffer, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            lock (buffer.Locker)
            {
                //var datasource = AnalogDataSource;// ;

                if (!datasource.HasData)
                {
                    buffer.EdgeBuffers.Clear();
                    return;
                }
                Int32 index = Array.FindIndex(datasource.Channels, x => x == id);
                if (index == -1)
                {
                    buffer.EdgeBuffers.RemoveAll(x => x.Id == id && x.StatusCount == 3);
                    return;
                }
                if (datasource.Channels.Contains(id))
                {
                    Int32 bufferindex = buffer.EdgeBuffers.FindIndex(x => x.Id == id && x.StatusCount == 3);
                    if (bufferindex == -1)
                    {
                        buffer.EdgeBuffers.Add(new EdgeBuffer()
                        {
                            Id = id,
                        });
                        AnalysisThreeLevelData(ref datasource, id, ref Unsafe.AsRef(buffer.EdgeBuffers[^1]), ref token, ref needclear);
                    }
                    else if (buffer.EdgeBuffers[bufferindex].TimeStamp != datasource.TimeStamp && bufferindex >= 0)
                    {
                        buffer.EdgeBuffers[bufferindex].Clear();
                        AnalysisThreeLevelData(ref datasource, id, ref Unsafe.AsRef(buffer.EdgeBuffers[bufferindex]), ref token, ref needclear);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        private void AnalysisThreeLevelData(ref ComModel.DeocodeDataSourcePacket datasource, ChannelId id, ref EdgeBuffer edgeBuffer, ref CancellationToken token, ref Boolean needclear)
        {
            //var datasource = AnalogDataSource;
            edgeBuffer.First = null;
            edgeBuffer.Last = null;
            UInt32 maxbytecount = datasource.MaxByteCount;
            Int32 chindex = Array.FindIndex(datasource.Channels, x => x == id);
            if (chindex == -1)
            {
                return;
            }
            Int32 skipcount = 8;// / 8 * datasource.Channels.Length;
            Int32 startbyteindex = 0;// chindex * (datasource.InterwovenBitCount / 8);
            Int32 channelLen = 1;
            var bytecount = 8;//按8字节为一组解析数据 datasource.InterwovenBitCount / 8;
            if (id.IsAnalog())
            {
                switch (DsoModel.Default.Timebase.InterleaveMode)
                {
                    case AdcInterleaveMode.Mode1To1:
                    default:
                        //除去以下模式的所有情况：即开启三通道，开启不连续的两通道，开启四通道等
                        //连续许两字节为一通道数据，按双bit存储
                        //例如：第4-5字节-CH1,第6-7字节-CH2，第0-1字节-CH3，第2-3字节-CH4
                        startbyteindex = 6 - (chindex <= 1 ? 0 : 4) - ((chindex + 1) % 2 != 0 ? 2 : 0);
                        channelLen = 4;
                        bytecount = 2;
                        break;
                    case AdcInterleaveMode.Mode2To1:
                        if (chindex == 0 || chindex == 2)
                        {
                            //Mode2To1模式有两种情况：即同时开启CH1、CH2，或者同时开启CH3、CH4
                            //连续4字节数据为一个通道数据，且通道序号小取数据在后，按双bit存储
                            //如:整形数据数组的第4-7字节为CH1的数据,第0-3字字节为CH2的数据，后续依次内推
                            startbyteindex = 4;
                        }
                        else
                        {
                            startbyteindex = 0;
                        }
                        channelLen = 2;
                        bytecount = 4;
                        break;
                    case AdcInterleaveMode.Mode4To1:
                        //只开启一个通道的模式，按顺序、双bit存储数据
                        startbyteindex = 0;
                        break;
                }
            }
            ThreeLevelEdgeInfo.Status curstatus = ThreeLevelEdgeInfo.Status.None;
            ThreeLevelEdgeInfo.Status prestatus = ThreeLevelEdgeInfo.Status.None;
            ThreeLevelEdgeInfo root = ThreeLevelEdgeInfo.CreateRoot(curstatus);
            ThreeLevelEdgeInfo tempnode = root;
            if (id.IsAnalog())
            {
                Int32 nodeindex = 0;
                for (Int32 index = startbyteindex; index < maxbytecount; index += skipcount)
                {
                    for (Int32 byteindex = 0; byteindex < bytecount; byteindex++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        if (index + bytecount - 1 - byteindex >= datasource.ChannelDataSource.Length)
                        {
                            continue;
                        }
                        Byte byteval = datasource.ChannelDataSource[index + bytecount - 1 - byteindex];
                        if (AdcInterleaveMode.Mode4To1 == DsoModel.Default.Timebase.InterleaveMode)
                        {
                            Int32 tempIndex = index + 3 - byteindex % 4 + (byteindex > 3 ? 4 : 0);
                            if (tempIndex >= datasource.ChannelDataSource.Length)
                            {
                                continue;
                            }
                            byteval = datasource.ChannelDataSource[tempIndex];
                        }
                        else if (AdcInterleaveMode.Mode2To1 == DsoModel.Default.Timebase.InterleaveMode)
                        {
                            if (byteindex > 4)
                            {
                                break;
                            }
                        }
                        else if (AdcInterleaveMode.Mode1To1 == DsoModel.Default.Timebase.InterleaveMode)
                        {
                            if (byteindex > 2)
                            {
                                break;
                            }
                        }
                        for (Int32 byteIntraBitIndex = 0; byteIntraBitIndex < 8; byteIntraBitIndex += 2)
                        {
                            curstatus = ThreeLevelEdgeInfo.ConverToStatusExtend((Byte)((byteval >> (6 - 2 * (byteIntraBitIndex / 2)) & 0b11)));
                            if ((startbyteindex == index) && (0 == byteindex) && (0 == byteIntraBitIndex))
                            {
                                prestatus = curstatus;
                                root = ThreeLevelEdgeInfo.CreateRoot(curstatus);
                                tempnode = root;
                                var rootnodeindex = (index + byteindex - startbyteindex) * 4 / channelLen + byteIntraBitIndex / 2;
                                tempnode = tempnode.AddChild(rootnodeindex, curstatus);
                            }
                            if (AdcInterleaveMode.Mode2To1 == DsoModel.Default.Timebase.InterleaveMode)
                            {
                                nodeindex = ((index - startbyteindex) / channelLen + byteindex % 4) * 4 + byteIntraBitIndex / 2;
                            }
                            else
                            {
                                nodeindex = (index + byteindex - startbyteindex) * 4 / channelLen + byteIntraBitIndex / 2;
                            }
                            if (curstatus != prestatus)
                            {
                                // var nodeindex = (index + byteIndex - startbyteindex) * 4 / channelLen + byteIntraBitIndex / 2;
                                tempnode = tempnode.AddChild(nodeindex, curstatus);
                                prestatus = curstatus;
                            }
                        }
                    }
                }
                tempnode.SetNodeEndIndex(nodeindex);
            }
            else if (id.IsReference() && datasource.ChannelDataSource != null)
            {
                Int32 startindex = 0;
                Byte currentlevel = 0;
                Byte preLevel = 0;
                for (Int32 bitindex = startbyteindex; bitindex < maxbytecount; bitindex++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    currentlevel = datasource.ChannelDataSource[bitindex];
                    if (bitindex == startbyteindex)
                    {
                        startindex = bitindex;
                        preLevel = currentlevel;
                    }

                    if (currentlevel != preLevel)
                    {
                        var thelevel = preLevel switch
                        {
                            > 127 => ThreeLevelEdgeInfo.Status.High,
                            127 => ThreeLevelEdgeInfo.Status.Middle,
                            _ => ThreeLevelEdgeInfo.Status.Low,
                        };

                        tempnode = tempnode.AddChild(startindex, thelevel);
                        startindex = bitindex;
                        preLevel = currentlevel;
                    }

                    if (bitindex == maxbytecount - 1)
                    {
                        //tempnode.SetNodeEndIndex(startindex);
                        var thelevel = currentlevel switch
                        {
                            > 127 => ThreeLevelEdgeInfo.Status.High,
                            127 => ThreeLevelEdgeInfo.Status.Middle,
                            _ => ThreeLevelEdgeInfo.Status.Low,
                        };
                        tempnode = tempnode.AddChild(startindex, thelevel);
                        tempnode.SetNodeEndIndex(bitindex);
                    }
                }
            }
            edgeBuffer.First = root;
            edgeBuffer.Last = tempnode;
            edgeBuffer.StatusCount = 3;
            edgeBuffer.TimeStamp = datasource.TimeStamp;
            edgeBuffer.Id = id;
        }
        private void AnalysisDecodeData(ref DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer buffer, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            lock (buffer.Locker)
            {
                if (!datasource.HasData)
                {
                    buffer.EdgeBuffers.Clear();
                    return;
                }
                Int32 index = Array.FindIndex(datasource.Channels, x => x == id);
                if (index == -1)
                {
                    buffer.EdgeBuffers.RemoveAll(x => x.Id == id && x.StatusCount == 2);
                    return;
                }
                if (datasource.Channels.Contains(id))
                {
                    Int32 bufferindex = buffer.EdgeBuffers.FindIndex(x => x.Id == id && x.StatusCount == 2);
                    if (bufferindex == -1)
                    {
                        buffer.EdgeBuffers.Add(new EdgeBuffer()
                        {
                            Id = id,
                        });
                        AnalysisData(id, ref Unsafe.AsRef(buffer.EdgeBuffers[^1]), ref datasource, ref token, ref needclear);
                    }
                    else if (buffer.EdgeBuffers[bufferindex].TimeStamp != datasource.TimeStamp && bufferindex >= 0)
                    {
                        buffer.EdgeBuffers[bufferindex].Clear();
                        AnalysisData(id, ref Unsafe.AsRef(buffer.EdgeBuffers[bufferindex]), ref datasource, ref token, ref needclear);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        private void AnalysisData(ChannelId id, ref EdgeBuffer edgeBuffer, ref DeocodeDataSourcePacket datasource, ref CancellationToken token, ref Boolean needclear)
        {
            Int32 bitcount = 8;//单个Byte=8个bit
            edgeBuffer.First = null;
            edgeBuffer.Last = null;
            UInt32 maxbytecount = datasource.MaxByteCount;
            Int32 chindex = Array.FindIndex(datasource.Channels, x => x == id);
            if (chindex == -1)
            {
                return;
            }
            Int32 skipcount = 4;// datasource.InterwovenBitCount;// / 8 * datasource.Channels.Length;
            Int32 startbyteindex = 0;// chindex * (datasource.InterwovenBitCount / 8);
            Int32 bytecount = 0;
            if (id.IsAnalog())
            {
                bytecount = datasource.InterwovenBitCount / bitcount;
                switch (datasource.InterwovenBitCount)
                {
                    case 8:
                    default:
                        skipcount = 4;
                        startbyteindex = chindex/* * skipcount*/;
                        break;
                    case 16://同时开启1、2或者3、4通道
                        if (chindex == 0 || chindex == 2)
                        {
                            startbyteindex = 0;
                        }
                        else
                        {
                            startbyteindex = 2;
                        }
                        break;
                    case 32://开启一个通道
                        startbyteindex = 0;
                        break;
                }
            }
            //Int32 perchbytecount = datasource.InterwovenBitCount / 8;
            Int32 laststatus = (datasource.ChannelDataSource[startbyteindex] >> (bitcount - 1)) & 0x01;
            Int32 currentstatus = laststatus;
            var root = TwoLevelEdgeInfo.CreateRoot(currentstatus);
            var tempnode = root;
            if (id.IsAnalog())
            {
                Int32 nodeindex = -1;
                PlatformManager.Default.Platform.ProcessDecodeData(id, bitcount, ref datasource, ref root, ref tempnode, ref nodeindex, token);
                if (nodeindex != -1)
                {
                    tempnode.SetNodeEndIndex(nodeindex);
                }
            }
            else if (id.IsReference())
            {
                for (Int32 index = startbyteindex; index < maxbytecount; index++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    currentstatus = (datasource.ChannelDataSource[index] >> 7) & 0x01;
                    if (currentstatus != laststatus)
                    {
                        tempnode = tempnode.AddChild(index, currentstatus);
                        laststatus = currentstatus;
                    }
                    if (index == maxbytecount - 1)
                    {
                        tempnode.SetNodeEndIndex(index);
                    }
                }
            }
            edgeBuffer.First = root;
            edgeBuffer.Last = tempnode;
            edgeBuffer.StatusCount = 2;
            edgeBuffer.TimeStamp = datasource.TimeStamp;
            edgeBuffer.Id = id;
        }
        public static Boolean GetPAM2EdgePulseSequence(TwoLevelEdgeInfo node, UInt32 bufferlength,
            ref List<PAM2EdgePulse> edgePulses, Double sampleRate, out PAM2EdgePulseSequence pulseData)
        {
            pulseData = new();

            if (node == null || node.IsLast)
            {
                return false;
            }

            Edge lastEdge = node.Edge;
            UInt64 startIndex = 0;

            edgePulses = new();
            try
            {
                while (node != null && node.Child != null)
                {
                    DecoderTypes.PAM2StatusType curValue = node.CurrentLevel ? DecoderTypes.PAM2StatusType.High : DecoderTypes.PAM2StatusType.Low;
                    if (node.IsRoot)
                    {
                        //lastEdge = node.Edge;
                        startIndex = (UInt64)node.StartIndex;
                        node = (TwoLevelEdgeInfo)node.Child;
                        continue;
                    }
                    else if (node.EdgeIndex == 1)
                    {
                        lastEdge = node.Edge;
                        startIndex = (UInt64)node.StartIndex;
                        if (edgePulses.Count() == 0 && node.StartIndex > 0)
                        {
                            edgePulses.Add(new(0, (UInt32)node.StartIndex, node.Edge == Edge.Rise ? Edge.Falling : Edge.Rise, curValue, node.EdgeIndex));
                        }
                        edgePulses.Add(new((UInt32)node.StartIndex, (UInt32)node.EndIndex, node.Edge, curValue, node.EdgeIndex));
                        node = (TwoLevelEdgeInfo)node.Child;
                        continue;
                    }
                    else if (lastEdge != node.Edge)
                    {
                        lastEdge = node.Edge;
                        edgePulses.Add(new((UInt32)node.StartIndex, (UInt32)node.EndIndex, node.Edge, curValue, node.EdgeIndex));
                    }
                    else
                    {
                        edgePulses[^1].SetEndIndex((UInt32)node.EndIndex);
                    }
                    node = (TwoLevelEdgeInfo)node.Child;
                    if (node.IsLast)
                    {
                        edgePulses.Add(new((UInt32)node.StartIndex, bufferlength, node.Edge, curValue, node.EdgeIndex));
                        break;
                    }
                }
            }
            catch
            {
                return false;
            }
            if (edgePulses.Count == 0)
            {
                return false;
            }
            //pulseData.WaveformDataCount = (UInt64)edgePulses[^1].EndIndex - startIndex;
            pulseData.WaveformDataCount = (UInt64)edgePulses[^1].EndIndex;
            if (pulseData.WaveformDataCount > bufferlength)
            {
                pulseData.WaveformDataCount = bufferlength;
            }
            pulseData.EdgePulsesCount = (UInt64)edgePulses.Count;
            pulseData.SampleRateByHz = sampleRate;

            return true;
        }
        public BaseEdgeInfo? GetCPHYDiffEdgeInfo(ChannelId busId, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return null;
            Int32 index = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == id);
            if (index >= 0)
            {
                return GetEdgeInfo(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, id, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                index = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == id);
                if (index >= 0)
                {
                    return GetEdgeInfo(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, id, ref token, ref needclear);
                }
            }

            if (!id.IsReference() || DecodeDataSource.Instance.DiffDataSource[id - ChannelId.R1].ChannelDataSource == null)
            {
                return null;
            }

            var @ref = DecodeDataSource.Instance.DiffDataSource.Where(x => x.Channels != null && x.Channels[0] == id);
            if (@ref != null && @ref.Count() > 0)
            {
                return GetEdgeInfo(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _DifferEdgeBuffer, startindex, id, ref token, ref needclear);
            }
            return null;
        }

        public BaseEdgeInfo? GetEdgeInfo(ChannelId busId, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return null;
            Int32 index = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == id);
            if (index >= 0)
            {
                return GetEdgeInfo(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, id, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                index = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == id);
                if (index >= 0)
                {
                    return GetEdgeInfo(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, id, ref token, ref needclear);
                }
            }

            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == id);
            if (@ref != null && @ref.Count() > 0)
            {
                return GetEdgeInfo(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, id, ref token, ref needclear);
            }

            var flag = 1;
            if (flag == 2)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "BitData.txt")))
                {
                    foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSources[0].ChannelDataSource)
                    {
                        sw.WriteLine(((b >> 7) & 0x01).ToString());
                    }
                }
            }

            return null;
        }

        public BaseEdgeInfo? GetThreeLevelEdgeInfo(ChannelId busId, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return null;
            Int32 index = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == id);
            if (index >= 0)
            {
                return GetThreeLevelEdgeInfo(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, id, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                index = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == id);
                if (index >= 0)
                {
                    return GetThreeLevelEdgeInfo(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, id, ref token, ref needclear);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == id);
            if (@ref != null && @ref.Count() > 0)
            {
                return GetThreeLevelEdgeInfo(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, id, ref token, ref needclear);
            }

            //var flag = 1;
            //if (flag == 2)
            //{
            //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Data.txt")))
            //    {
            //        foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSource.ChannelDataSource)
            //        {
            //            sw.WriteLine(b.ToString());
            //        }
            //    }
            //}

            return null;
        }

        private BaseEdgeInfo? GetThreeLevelEdgeInfo(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer buffer,
    Int32 startindex,
    ChannelId id,
    ref CancellationToken token,
    ref Boolean needclear)
        {
            Int64 sourcetime = datasource.TimeStamp;
            AnalysisThreeLevelDecodeData(ref datasource, ref buffer, id, ref token, ref needclear);
            Int32 index = buffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == sourcetime && x.First != null && x.Last != null && x.StatusCount == 3);
            if (index == -1)
                return null;
            return buffer.EdgeBuffers[index].First.GetEdgeInfoByIndex(startindex);
        }
        private BaseEdgeInfo? GetEdgeInfo(ref ComModel.DeocodeDataSourcePacket datasource,
            ref EdgeInfoBuffer buffer,
            Int32 startindex,
            ChannelId id,
            ref CancellationToken token,
            ref Boolean needclear)
        {
            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt) || prsnt == null || !prsnt.Active)
                return null;
            Int64 sourcetime = datasource.TimeStamp;
            AnalysisDecodeData(ref datasource, ref buffer, id, ref token, ref needclear);
            Int32 index = buffer.EdgeBuffers.FindIndex(x => x.Id == id && x.TimeStamp == sourcetime && x.First != null && x.Last != null && x.StatusCount == 2);
            if (index == -1)
                return null;
            return buffer.EdgeBuffers[index].First.GetEdgeInfoByIndex(startindex);
        }
        public Int32 FindNextEdge(ChannelId busId, Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return -1;
            Int32 index = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (index >= 0)
            {
                return FindNextEdge(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                index = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (index >= 0)
                {
                    return FindNextEdge(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, ch, ref token, ref needclear);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                return FindNextEdge(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            //index = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (index >= 0)
            //{
            //    return FindNextEdge(ref Unsafe.AsRef(ReferenceDataSource), ref ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            //}
            return -1;
        }
        // ljw 24.6
        private Int32 FindNextRisingEdge(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer edgeInfoBuffer, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            FindNextEdge(ref datasource, ref edgeInfoBuffer, startindex, id, ref token, ref needclear);
            if (edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.StatusCount == 2) >= 0)
            {
                var idbuffer = edgeInfoBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == id && x.StatusCount == 2);
                if (idbuffer == null || idbuffer.First == null || idbuffer.Last == null)
                    return -1;

                var idindex = idbuffer.First.GetEdgeInfoByIndex(startindex);
                if (idindex == null)
                    return -1;
                if (startindex > idindex.StartIndex)
                    idindex = idindex.Child;
                if (idindex == null)
                    return -1;

                while (true)
                {
                    if (idindex.Edge == Edge.Rise)
                    {
                        return idindex.StartIndex;
                    }
                    idindex = idindex.Child;
                    if (idindex == null)
                        return -1;
                }

            }
            else
            {
                return -1;
            }
        }
        //ljw 24.6
        private Int32 FindNextFallingEdge(ref ComModel.DeocodeDataSourcePacket datasource, ref EdgeInfoBuffer edgeInfoBuffer, Int32 startindex, ChannelId id, ref CancellationToken token, ref Boolean needclear)
        {
            FindNextEdge(ref datasource, ref edgeInfoBuffer, startindex, id, ref token, ref needclear);
            if (edgeInfoBuffer.EdgeBuffers.FindIndex(x => x.Id == id && x.StatusCount == 2) >= 0)
            {
                var idbuffer = edgeInfoBuffer.EdgeBuffers.FirstOrDefault(x => x.Id == id && x.StatusCount == 2);
                if (idbuffer == null || idbuffer.First == null || idbuffer.Last == null)
                    return -1;

                var idindex = idbuffer.First.GetEdgeInfoByIndex(startindex);
                if (idindex == null)
                    return -1;
                if (startindex > idindex.StartIndex)
                    idindex = idindex.Child;
                if (idindex == null)
                    return -1;

                while (true)
                {
                    if (idindex.Edge == Edge.Falling)
                    {
                        return idindex.StartIndex;
                    }
                    idindex = idindex.Child;
                    if (idindex == null)
                        return -1;
                }

            }
            else
            {
                return -1;
            }
        }
        //ljw 24.6
        public Int32 FindNextRisingEdge(ChannelId busId, Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return -1;
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                return FindNextRisingEdge(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    return FindNextRisingEdge(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, ch, ref token, ref needclear);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                return FindNextRisingEdge(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            }

            return -1;
        }
        public Int32 FindNextFallingEdge(ChannelId busId, Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return -1;
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                return FindNextFallingEdge(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    return FindNextFallingEdge(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, ch, ref token, ref needclear);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                return FindNextFallingEdge(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    return FindNextFallingEdge(ref Unsafe.AsRef(ReferenceDataSource), ref ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            //}
            return -1;
        }

        public Int32 FindLastEdge(ChannelId busId, Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex == -1)
                return -1;
            Int32 index = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (index >= 0)
            {
                return FindLastEdge(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), ref _AnalogEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            if (LADataSource != null)
            {
                index = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (index >= 0)
                {
                    return FindLastEdge(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), ref _LAEdgeBuffer, startindex, ch, ref token, ref needclear);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                return FindLastEdge(ref Unsafe.AsRef(@ref.FirstOrDefault()), ref _ReferenceEdgeBuffer, startindex, ch, ref token, ref needclear);
            }
            //index = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (index >= 0)
            //{
            //    return FindLastEdge(ref Unsafe.AsRef(ReferenceDataSource), ref LAEdgeBuffer, startindex, ch, ref token, ref needclear);
            //}
            return -1;
        }
        public Boolean GetLevel(ChannelId busId, Int32 index, Double threshold, ComModel.ChannelId ch, Boolean inverse = false)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                return GetLevel(ref Unsafe.AsRef(AnalogDataSources[busId - ChannelId.B1]), index, chindex, inverse);
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    return GetLevel(ref Unsafe.AsRef(((DeocodeDataSourcePacket)LADataSource)), index, chindex, inverse);
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                return GetLevel(ref Unsafe.AsRef(@ref.FirstOrDefault()), index, 0, inverse);
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    return GetLevel(ref Unsafe.AsRef(LADataSource), index, chindex, inverse);
            //}

            return false;
        }
        public Boolean TryGetHasData(ChannelId busId, ComModel.ChannelId ch, ref Boolean hasData)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                hasData = AnalogDataSources[busId - ChannelId.B1].HasData;
                return true;
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    hasData = ((DeocodeDataSourcePacket)LADataSource).HasData;
                    return true;
                }
            }

            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                hasData = @ref.FirstOrDefault().HasData;
                return true;
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    hasData = ReferenceDataSource.HasData;
            //    return true;
            //}
            return false;
        }
        public Boolean TryGetTriggerIndex(ChannelId busId, ComModel.ChannelId ch, ref Int64 triggerIndex)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                triggerIndex = AnalogDataSources[busId - ChannelId.B1].TriggerIndex;
                return true;
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    triggerIndex = ((DeocodeDataSourcePacket)LADataSource).TriggerIndex;
                    return true;
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                triggerIndex = @ref.FirstOrDefault().TriggerIndex;
            }
            return false;
        }
        public Boolean TryGetPerChannelDataLength(ChannelId busId, ComModel.ChannelId ch, ref UInt32 rerChannelDataLength)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                rerChannelDataLength = AnalogDataSources[busId - ChannelId.B1].PerChannelDataLength;
                return true;
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    rerChannelDataLength = ((DeocodeDataSourcePacket)LADataSource).PerChannelDataLength;
                    return true;
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                rerChannelDataLength = @ref.FirstOrDefault().PerChannelDataLength;
                return true;
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    rerChannelDataLength = ReferenceDataSource.PerChannelDataLength;
            //    return true;
            //}
            return false;
        }
        public Boolean TryGetTimeStamp(ChannelId busId, ComModel.ChannelId ch, ref Int64 timeStamp)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                timeStamp = AnalogDataSources[busId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    timeStamp = ((DeocodeDataSourcePacket)LADataSource).TimeStamp;
                    return true;
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                timeStamp = @ref.FirstOrDefault().TimeStamp;
                return true;
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    timeStamp = ReferenceDataSource.TimeStamp;
            //    return true;
            //}
            return false;
        }

        public Boolean TryGetSampleRate(ChannelId busId, ComModel.ChannelId ch, ref Double sampleRate)
        {
            Int32 chindex = Array.FindIndex(AnalogDataSources[busId - ChannelId.B1].Channels, x => x == ch);
            if (chindex >= 0)
            {
                sampleRate = AnalogDataSources[busId - ChannelId.B1].SampleRate;
                return true;
            }
            if (LADataSource != null)
            {
                chindex = Array.FindIndex(((DeocodeDataSourcePacket)LADataSource).Channels, x => x == ch);
                if (chindex >= 0)
                {
                    sampleRate = ((DeocodeDataSourcePacket)LADataSource).SampleRate;
                    return true;
                }
            }
            var @ref = DecodeDataSource.Instance.ReferenceDataSource.Where(x => x.Channels != null && x.Channels[0] == ch);
            if (@ref != null && @ref.Count() > 0)
            {
                sampleRate = @ref.FirstOrDefault().SampleRate;
                return true;
            }
            //chindex = Array.FindIndex(ReferenceDataSource.Channels, x => x == ch);
            //if (chindex >= 0)
            //{
            //    sampleRate = ReferenceDataSource.SampleRate;
            //    return true;
            //}
            return false;
        }
        private Boolean GetLevel(ref ComModel.DeocodeDataSourcePacket dataSource, Int32 index, Int32 chindex, Boolean inverse = false)
        {
            Boolean status = false;
            Int32 startbyteindex = 0;// chindex * (datasource.InterwovenBitCount / 8);
            if (chindex >= dataSource.Channels.Length || index >= dataSource.PerChannelDataLength)
                return false;
            if (dataSource.Channels[chindex].IsAnalog())
            {
                switch (DsoModel.Default.Timebase.InterleaveMode)
                {
                    case AdcInterleaveMode.Mode1To1:
                    default:
                        startbyteindex = chindex /** dataSource.InterwovenBitCount*/;
                        break;
                    case AdcInterleaveMode.Mode2To1:
                        if (chindex == 0 || chindex == 2)
                        {
                            startbyteindex = 0;
                        }
                        else
                        {
                            startbyteindex = 2;
                        }
                        break;
                    case AdcInterleaveMode.Mode4To1:
                        startbyteindex = 0;
                        break;
                }
                Int32 currentindex = (index / dataSource.InterwovenBitCount) * dataSource.Channels.Length * (dataSource.InterwovenBitCount / 8) + startbyteindex/*chindex * dataSource.InterwovenBitCount / 8*/;
                Int32 currentbitindex = index % dataSource.InterwovenBitCount;
                //status = Convert.ToBoolean((dataSource.ChannelDataSource[currentbitindex / 8 + currentindex] >> (7 - (currentbitindex % 8))) & 0x01);
                status = Convert.ToBoolean((dataSource.ChannelDataSource[dataSource.InterwovenBitCount / 8 - 1 - currentbitindex / 8 + currentindex] >> (7 - (currentbitindex % 8))) & 0x01);
            }
            else if (dataSource.Channels[chindex].IsReference())
            {
                status = Convert.ToBoolean((dataSource.ChannelDataSource[index] >> 7) & 0x01);
            }

            //Int32 startbyteindex = 0;// chindex * (datasource.InterwovenBitCount / 8);
            //switch (DsoModel.Default.Timebase.InterleaveMode)
            //{
            //    case AdcInterleaveMode.Mode1To1:
            //    default:
            //        startbyteindex = chindex /** dataSource.InterwovenBitCount*/;
            //        break;
            //    case AdcInterleaveMode.Mode2To1:
            //        if (chindex == 0 || chindex == 2)
            //        {
            //            startbyteindex = 0;
            //        }
            //        else
            //        {
            //            startbyteindex = dataSource.InterwovenBitCount;
            //        }
            //        break;
            //    case AdcInterleaveMode.Mode4To1:
            //        startbyteindex = 0;
            //        break;
            //}
            //Double n = (Double)index * dataSource.Channels.Length / 8;
            ////Boolean status = Convert.ToBoolean((dataSource.ChannelDataSource[(Int32)Math.Ceiling(n) + startbyteindex] >>7) & 0x01);
            //Boolean status = Convert.ToBoolean((dataSource.ChannelDataSource[(Int32)n + startbyteindex] >> 7) & 0x01);
            return inverse ? !status : status;
        }

        public void GetTwoLevelPulses(ref TwoLevelEdgeInfo node, ref List<PAM2EdgePulse> edgePulsesList)
        {
            Int32 edgepulseIndex = 0;
            while (node != null)
            {
                PAM2EdgePulse epules = new PAM2EdgePulse((UInt32)node.StartIndex, (UInt32)node.EndIndex, node.Edge, node.CurrentLevel ? DecoderTypes.PAM2StatusType.High : DecoderTypes.PAM2StatusType.Low, (UInt32)edgepulseIndex);
                node = node?.Child as TwoLevelEdgeInfo;
                edgepulseIndex++;
                edgePulsesList.Add(epules);
            }
        }

        public void GetThreeLevelPulses(ref ThreeLevelEdgeInfo node, ref List<PAM3EdgePulse> edgePulsesList)
        {
            Int32 edgepulseIndex = 0;
            while (node != null)
            {
                PAM3EdgePulse epules = new PAM3EdgePulse((UInt32)node.StartIndex, (UInt32)node.EndIndex, node.Edge,
                    (PAM3StatusType)node.CurrentLevel, (UInt32)edgepulseIndex);
                node = node?.Child as ThreeLevelEdgeInfo;
                edgepulseIndex++;
                edgePulsesList.Add(epules);
            }
        }
    }
}
