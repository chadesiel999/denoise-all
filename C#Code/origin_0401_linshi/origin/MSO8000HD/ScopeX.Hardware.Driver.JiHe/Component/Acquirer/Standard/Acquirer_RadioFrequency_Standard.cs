using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExt;
using System.Numerics;
using System.Threading.Channels;
using CyUSB;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集
    /// </summary>
    public class Acquirer_RadioFrequency_Standard : AbstractAcquirer_RadioFrequency
    {
#if FrequencyDomain
        internal static byte[] DMAData = new byte[0];
        internal static byte[] DMATotalData = new byte[0];
        private byte[] IQDMAData = new byte[4 * 16 * 8 * 1024];

        /// <summary>
        /// 此值由硬件方案决定，包括ADC的位数，DDR颗粒数。对目前的硬件，采用12位的ADC，有3个DDR颗粒，故（3*16)=48,48/12=4
        /// </summary>
        private UInt32 DDR_PerAddrStoreDots { get => 4; }
        private UInt32 DDR_PerAddrBytes { get => 6; }//48bit,48/8=6;
        private UInt32 DDR_PingPangAddr { get => 1024 * 1024 * 1024 / DDR_PerAddrStoreDots; }
        private UInt32 DDR_AddrBurstConvert(UInt32 addr)
        {
            return addr & 0xffffffc;
        }

        public Acquirer_RadioFrequency_Standard()
        {
            ConfigFunc = null;
            //DMAData = new byte[Constants.CHNL_DATA_NUM / 1000 * (4 * Constants.ADC_BITS + 16) / 8 * 1024];
            DMAData = new byte[16 * 8 * 1024];
            DMATotalData = new byte[16 * 1024 * 1024];
        }

        internal override void CreateAcquireAttribute()
        {
            AcquingParameters.AcqStorageMode = Hd.UIMessage?.Timebase?.AcqLength ?? AnaChnlStorageMode.Normal;
        }

        internal override void Init()
        {

        }
        internal override void InitAcq()
        {

        }

        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
        }
        //private UInt32 FifoMaxDepth = 16 * 1024;
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            return true;
        }

        protected virtual bool AcqAnalogChannelSimulateWaveform()
        {
            Monitor.Enter(AllChannelWaveDataLock);
            AllChannelWaveData.Clear();
            int Length = Constants.CHNL_DATA_NUM;
            double SampIntByns = Hd.UIMessage?.Timebase?.TmbScale * Constants.VIS_XDIVS_NUM * 1000 / Constants.CHNL_DATA_NUM ?? 0.5;
            var cycles = Length * (SampIntByns * 1E-9) * (Constants.AWG_SIN_FRQ_DEF * 1E-6);
            double NoiseByPercent = 0.05;
            ArbWfmType[] allChannelArbWfmType = { ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.Haversine, ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.Haversine };
            for (int channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            {
                double anaChannelPosition = 0;// Constants.IDX_PER_YDIV * 5;// Hd.CurrHdMessage?.Analog?[channelID].PositionIndex ?? 0;
                double amplitude = Constants.IDX_PER_YDIV * 4;// (Hd.CurrHdMessage?.Analog?[channelID].Scale ?? 0) * 6;
                ArbWfmType arbWfmType = allChannelArbWfmType[channelID];
                IEnumerable<Double> y = arbWfmType switch
                {
                    ArbWfmType.Pulse or ArbWfmType.Square => Generator.Rectangular(anaChannelPosition, amplitude, cycles / Length, Length, 0.05, NoiseByPercent, 0.1),
                    ArbWfmType.DC => Generator.DirectCurrent(anaChannelPosition, amplitude, Length, 0.05),
                    ArbWfmType.Haversine => Generator.Haversine(anaChannelPosition, amplitude, cycles / Length, Length, NoiseByPercent, 0.05),
                    _ => Generator.Sinc(anaChannelPosition, amplitude, cycles / Length, Length, NoiseByPercent, 0.5, 0),
                };
                var pos0 = (Hd.UIMessage?.Analog?[channelID].PositionIndex ?? 0) / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                y = y.Select((o) => o /*(Hd.CurrHdMessage?.Analog?[channelID].Scale ?? 100)*/ / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + pos0);
                Double[] data = y.ToArray();// .ToRowVector();
                AllChannelWaveData.Add(new List<ushort>());
                for (int i = 0; i < Length; i++)
                    AllChannelWaveData[channelID].Add((ushort)data[i]);
            }
            //AcquedParameters.PerDataByfs_ToCore = (long)(Hd.CurrHdMessage?.Timebase?.TmbScale * Constants.VIS_XDIVS_NUM * 1000_000_000 / Constants.CHNL_DATA_NUM ?? 50 * 1000);
            Monitor.Exit(AllChannelWaveDataLock);
            return true;
        }

        private List<UInt32> DDR_ReadbackTrigAddrTable = new List<uint>();
        class DDRReadParams
        {
            public ulong AcqedTimeStamp { get; set; }
            public UInt32 SegmentStartAddr { get; set; }
            public UInt32 StartAddr { get; set; }
            public UInt32 ExtractNum { get; set; }
            public ulong ResultPerData_fs { get; set; }
            public UInt32 DDRAddrCount { get; set; }
            public bool bNeedReRead(DDRReadParams old)
            {
                if (this.AcqedTimeStamp == old.AcqedTimeStamp && this.SegmentStartAddr == old.SegmentStartAddr && this.StartAddr == old.StartAddr && this.ExtractNum == old.ExtractNum && this.DDRAddrCount == old.DDRAddrCount && this.ResultPerData_fs == old.ResultPerData_fs)
                    return false;
                return true;
            }
            public void Clone(DDRReadParams source)
            {
                this.AcqedTimeStamp = source.AcqedTimeStamp;
                this.SegmentStartAddr = source.SegmentStartAddr;
                this.StartAddr = source.StartAddr;
                this.ExtractNum = source.ExtractNum;
                this.DDRAddrCount = source.DDRAddrCount;
                this.ResultPerData_fs = source.ResultPerData_fs;
            }
        }
        private DDRReadParams lastDDRReadParams = new DDRReadParams { AcqedTimeStamp = 0, SegmentStartAddr = 0, StartAddr = 0, ExtractNum = 0, ResultPerData_fs = 0, DDRAddrCount = 0 };
        private UInt32 getValidPostExtractNum(UInt32 extractNum)
        {
            return extractNum;
        }
#endif
    }
}

