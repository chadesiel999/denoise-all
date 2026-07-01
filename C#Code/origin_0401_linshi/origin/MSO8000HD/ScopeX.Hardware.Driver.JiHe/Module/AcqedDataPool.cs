using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal class AcquiredData
    {
        internal long TimestampInitedAcq
        {
            get;
            set;
        } = 0;
        internal long TimestampAcquired
        {
            get;
            set;
        }
        internal WfmSampleInfo WfmSampleInfo
        {
            get;
        } = new WfmSampleInfo();
    }
    internal class AnalogChannelData : AcquiredData
    {
        internal List<List<ushort>> AllChannelData
        {
            get;
        } = new List<List<ushort>>();
    }
    internal class LAData : AcquiredData
    {
        internal List<ushort> Data
        {
            get;
        } = new List<ushort>();
        internal List<byte> DMAData
        {
            get;
        } = new List<byte>();
    }
    internal class DPXData : AcquiredData
    {
        internal DPXData()
        {
        }
        internal List<Byte[,]> Data
        {
            get;
            init;
        }
        internal byte[] DMAData = new byte[Constants.UPO_HEIGHT* Constants.UPO_WIDTH];//
    }
    internal class RFData : AcquiredData
    {
        internal static object UpdateDataLock = new object();
        internal RFDataType AcquiredDataType
        {
            get;
            set;
        } = RFDataType.Frequency;
        internal List<Double> Data
        {
            get;
        } = new List<Double>();

        internal List<Double> PhaseVSFrequencyData
        {
            get;
        } = new List<Double>();
        internal List<Double> PhaseVSTimeData
        {
            get;
        } = new List<Double>();
        internal List<Double> SpectrogramData
        {
            get;
        } = new List<Double>();//lhc
        internal List<Double> AmpVSTimeData
        {
            get;
        } = new List<Double>();
        internal List<Double> FrequencyVSTimeData
        {
            get;
        } = new List<Double>();

        //只有Raw 有DMAData
        internal List<byte> DMAData
        {
            get;
        } = new List<byte>();

        internal List<Double> DataI
        {
            get;
        } = new List<Double>();
        internal List<Double> DataQ
        {
            get;
        } = new List<Double>();
        internal List<Double> FFTDataI
        {
            get;
        } = new List<Double>();
        internal List<Double> FFTDataQ
        {
            get;
        } = new List<Double>();
        internal List<Double> RoughSpecData
        {
            get;
        } = new List<Double>();

        internal List<Double> FineSpecData
        {
            get;
        } = new List<Double>();//lhc

        internal List<List<Double>> WaterFallsData
        {
            get;
        } = new List<List<Double>>();
    }

    internal class SearchData : AcquiredData
    {
        internal static object UpdateDataLock = new object();

        internal Dictionary<long, (Double[,] Result, Int32 ResultCount)> Data
        {
            get;
        } = new Dictionary<long, (Double[,] Result, Int32 ResultCount)>();
    }

    internal static class AcqedDataPool
    {
        internal static object UpdateDataLock = new object();

        internal static AnalogChannelData AnalogChData
        {
            get;
        } = new AnalogChannelData();
        internal static LAData LAData
        {
            get;
        } = new LAData();
        internal static DPXData DpxData
        {
            get;
        } = new DPXData();
        internal static RFData RFData
        {
            get;
        } = new RFData();
        internal static SearchData SearchData
        {
            get;
        } = new SearchData();
    }
}
