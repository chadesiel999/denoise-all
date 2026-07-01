using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using static ScopeX.Core.Decode.DecoderTypes;

namespace ScopeX.Core.Decode;

public class EdgeInfoCPP
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PAM2EdgePulse
    {
        public UInt64 StartIndex;
        public UInt64 EndIndex;
        public Edge Edge;
        public UInt64 EdgePulseIndex;//需确定边沿脉宽对象的在列表中的索引
        public PAMType PAMType;
        public DecoderTypes.PAM2StatusType CurrentLevel = DecoderTypes.PAM2StatusType.None;
        public PAM2EdgePulse(UInt64 startIndex, UInt64 endIndex, Edge edge, DecoderTypes.PAM2StatusType Level, UInt64 edgePulseIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Edge = edge;
            EdgePulseIndex = edgePulseIndex;
            PAMType = PAMType.PAM2;
            CurrentLevel = Level;
            //if (Edge == Edge.None)
            //{
            //    return;
            //}
            //CurrentLevel = Edge == Edge.Rise ? DecoderTypes.PAM2StatusType.High : DecoderTypes.PAM2StatusType.Low;
        }
        public void SetEndIndex(UInt32 endIndex)
        {
            EndIndex = endIndex;
        }
        public PAM2EdgePulse(PAM2EdgePulse edgePulse)
        {
            StartIndex = edgePulse.StartIndex;
            EndIndex = edgePulse.EndIndex;
            Edge = edgePulse.Edge;
            EdgePulseIndex = edgePulse.EdgePulseIndex;
            PAMType = PAMType.PAM2;
            if (Edge == Edge.None)
            {
                return;
            }
            CurrentLevel = Edge == Edge.Rise ? DecoderTypes.PAM2StatusType.High : DecoderTypes.PAM2StatusType.Low;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PAM3EdgePulse
    {
        public UInt64 StartIndex;
        public UInt64 EndIndex;
        public Edge Edge;
        public UInt64 EdgePulseIndex;//需确定边沿脉宽对象的在列表中的索引
        public PAMType PAMType;
        public DecoderTypes.PAM3StatusType CurrentLevel = DecoderTypes.PAM3StatusType.None;
        public PAM3EdgePulse(UInt64 startIndex, UInt64 endIndex, Edge edge, DecoderTypes.PAM3StatusType Level, UInt64 edgePulseIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Edge = edge;
            EdgePulseIndex = edgePulseIndex;
            PAMType = PAMType.PAM3;
            CurrentLevel = Level;
        }
        public PAM3EdgePulse(PAM3EdgePulse edgePulse)
        {
            StartIndex = edgePulse.StartIndex;
            EndIndex = edgePulse.EndIndex;
            EdgePulseIndex = edgePulse.EdgePulseIndex;
            PAMType = PAMType.PAM3;
            Edge = edgePulse.Edge;
            CurrentLevel = edgePulse.CurrentLevel;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAM2EdgePulseSequence
    {
        public UInt64 WaveformDataCount;
        public UInt64 EdgePulsesCount;
        public Double SampleRateByHz;
        IntPtr EdgePulsesPtr;
        public PAM2EdgePulseSequence(IntPtr edgePulses, UInt64 edgePulsesCount, UInt64 waveformDataCount, Double sampleRateByHz)
        {
            WaveformDataCount = waveformDataCount;
            EdgePulsesPtr = edgePulses;
            SampleRateByHz = sampleRateByHz;
            EdgePulsesCount = edgePulsesCount;
        }
        public void SetDataPtr(IntPtr intPtr)
        {
            EdgePulsesPtr = intPtr;
        }
        public void SetDataPtr(PAM2EdgePulse[] edgePulses)
        {
            EdgePulsesPtr = Marshal.UnsafeAddrOfPinnedArrayElement(edgePulses, 0);
        }
        public void SetDataPtr(List<PAM2EdgePulse> edgePulses)
        {
            EdgePulsesPtr = Marshal.UnsafeAddrOfPinnedArrayElement(edgePulses.ToArray(), 0);
        }
        public static void Allocate(ref List<PAM2EdgePulse> edgePulsesList, UInt64 waveformDataCount, Double sampleRate,
            out IntPtr edgePulseSequencePtr, out GCHandle edgePulsesHandle)
        {
            PAM2EdgePulse[] edgepulsearr = edgePulsesList.ToArray();
            edgePulsesHandle = GCHandle.Alloc(edgepulsearr, GCHandleType.Pinned);
            IntPtr edgepulseptr = Marshal.UnsafeAddrOfPinnedArrayElement(edgepulsearr, 0);

            PAM2EdgePulseSequence edgepulsedata = new(edgepulseptr, (UInt64)edgePulsesList.Count, waveformDataCount, sampleRate);
            edgePulseSequencePtr = Marshal.AllocHGlobal(Marshal.SizeOf(edgepulsedata));
            Marshal.StructureToPtr(edgepulsedata, edgePulseSequencePtr, false);
        }

        public static void Free(ref IntPtr edgePulseSequencePtr, ref GCHandle edgePulsesHandle)
        {
            edgePulsesHandle.Free();
            Marshal.FreeHGlobal(edgePulseSequencePtr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAM3EdgePulseSequence
    {
        public UInt64 WaveformDataCount;
        public UInt64 EdgePulsesCount;
        public Double SampleRateByHz;
        public IntPtr EdgePulsesPtr;
        public PAM3EdgePulseSequence(IntPtr edgePulses, UInt64 edgePulsesCount, UInt64 waveformDataCount, Double sampleRate)
        {
            WaveformDataCount = waveformDataCount;
            EdgePulsesPtr = edgePulses;
            SampleRateByHz = sampleRate;
            EdgePulsesCount = edgePulsesCount;
        }

        public static void Allocate(ref List<PAM3EdgePulse> edgePulsesList, UInt64 waveformDataCount, Double sampleRate,
            out IntPtr edgePulseSequencePtr, out GCHandle edgePulsesHandle)
        {
            PAM3EdgePulse[] edgepulseArr = edgePulsesList.ToArray();
            edgePulsesHandle = GCHandle.Alloc(edgepulseArr, GCHandleType.Pinned);
            IntPtr edgepulseptr = Marshal.UnsafeAddrOfPinnedArrayElement(edgepulseArr, 0);

            PAM3EdgePulseSequence edgepulsedata = new(edgepulseptr, (UInt64)edgePulsesList.Count, waveformDataCount, sampleRate);
            edgePulseSequencePtr = Marshal.AllocHGlobal(Marshal.SizeOf(edgepulsedata));
            Marshal.StructureToPtr(edgepulsedata, edgePulseSequencePtr, false);
        }

        public static void Free(ref IntPtr edgePulseSequencePtr, ref GCHandle edgePulsesHandle)
        {
            edgePulsesHandle.Free();
            Marshal.FreeHGlobal(edgePulseSequencePtr);
        }
    }
}
