using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class DbiCoefficientsTables : ICaliData/*, ICoefficientsTable*/
    {
        public const Int32 BandModeCount = 2;
        public const Int32 ChannelCount = 4;
        public const Int32 SubbandCount = 4;
        public const Int32 FilterbandModeCount = 2;

        public static DbiCoefficientsTables Default = new DbiCoefficientsTables();
        //acq board
        private Int32[/*20G/16G=2*/,/*低通、带通=2*/,/*Data*/] InterpolationCoefficients = new Int32[BandModeCount, FilterbandModeCount, 1000];
        private Int32[/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/] LocalOscillatorCoefficients = new Int32[BandModeCount, ChannelCount, SubbandCount, 800];//ok
        //抗镜像
        private Int32[/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/] AntiImageCoefficients = new Int32[BandModeCount, ChannelCount, SubbandCount, 300];
        //分数延时
        private Int32[/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/] FractionaryDelayCoefficients = new Int32[BandModeCount, ChannelCount, SubbandCount, 100];
        private Int32[/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/] OverlapPhaseFreqDelayCoefficients = new Int32[BandModeCount, ChannelCount, SubbandCount, 20];

        private Int32[/*20G/16G*/,/*channel*/,/*subBand*/,/*data*/] TiAdcCoefficients = new Int32[BandModeCount, ChannelCount, SubbandCount, 4096];

        //proc board
        private Int32[/*20G/16G=2*/,/*通道=4*/,/*data*/] AmpFreqCoefficients = new Int32[BandModeCount, ChannelCount, 1000];
        private Int32[/*20G/16G=2*/,/*通道=4*/,/*data*/] PhaseFreqCoefficients = new Int32[BandModeCount, ChannelCount, 4000];

        private Int32[/*20G/16G=2*/,/*data*/] MultiRadioInterpolationCoefficients = new Int32[BandModeCount, 3000];

        private DbiCoefficientsTables()
        {
        }

        #region
        public CaliDataType DataType { get => CaliDataType.DbiCoefficientsTables; }
        public Int32 TotalBytes
        {
            get
            {
                Int32 totalBytes = 0;
                foreach (DbiCoefficientsTablesType dbiCoefficientsTablesType in Enum.GetValues(typeof(DbiCoefficientsTablesType)))
                {
                    totalBytes += TotalBytesOfType(dbiCoefficientsTablesType);
                }
                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public Int32 PerDataCount(DbiCoefficientsTablesType dbiCoefficientsTablesType)
        {
            int result = dbiCoefficientsTablesType switch
            {
                DbiCoefficientsTablesType.InterpolationCoefficients => InterpolationCoefficients.GetLength(InterpolationCoefficients.Rank-1),
                DbiCoefficientsTablesType.LocalOscillatorCoefficients => LocalOscillatorCoefficients.GetLength(LocalOscillatorCoefficients.Rank-1),
                DbiCoefficientsTablesType.AntiImageCoefficients => AntiImageCoefficients.GetLength(AntiImageCoefficients.Rank-1),
                DbiCoefficientsTablesType.FractionaryDelayCoefficients => FractionaryDelayCoefficients.GetLength(FractionaryDelayCoefficients.Rank-1),
                DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients => OverlapPhaseFreqDelayCoefficients.GetLength(OverlapPhaseFreqDelayCoefficients.Rank-1),
                DbiCoefficientsTablesType.AmpFreqCoefficients => AmpFreqCoefficients.GetLength(AmpFreqCoefficients.Rank-1),
                DbiCoefficientsTablesType.PhaseFreqCoefficients => PhaseFreqCoefficients.GetLength(PhaseFreqCoefficients.Rank-1),
                DbiCoefficientsTablesType.TiAdc => TiAdcCoefficients.GetLength(TiAdcCoefficients.Rank - 1),
                DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients => MultiRadioInterpolationCoefficients.GetLength(MultiRadioInterpolationCoefficients.Rank-1),
                _ => 0,

            };
            return result;
        }
        public Int32 TotalBytesOfType(DbiCoefficientsTablesType dbiCoefficientsTablesType)
        {
            int result = dbiCoefficientsTablesType switch
            {
                DbiCoefficientsTablesType.InterpolationCoefficients => InterpolationCoefficients.Length,
                DbiCoefficientsTablesType.LocalOscillatorCoefficients => LocalOscillatorCoefficients.Length,
                DbiCoefficientsTablesType.AntiImageCoefficients => AntiImageCoefficients.Length,
                DbiCoefficientsTablesType.FractionaryDelayCoefficients => FractionaryDelayCoefficients.Length,
                DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients => OverlapPhaseFreqDelayCoefficients.Length,
                DbiCoefficientsTablesType.AmpFreqCoefficients => AmpFreqCoefficients.Length,
                DbiCoefficientsTablesType.PhaseFreqCoefficients => PhaseFreqCoefficients.Length,
                DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients => MultiRadioInterpolationCoefficients.Length,
                DbiCoefficientsTablesType.TiAdc=>TiAdcCoefficients.Length,
                _ => 0,

            };
            return (Int32)result * sizeof(Int32);
        }
        //仅仅用于保存和装载
        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            foreach (DbiCoefficientsTablesType dbiCoefficientsTablesType in Enum.GetValues(typeof(DbiCoefficientsTablesType)))
                memoryStream.Write(Serialize(dbiCoefficientsTablesType));
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
        public void Deserialize(byte[] content)
        {
            Int32 maxTotalBytes = 0;
            int tmpBytes = 0;
            foreach (DbiCoefficientsTablesType dbiCoefficientsTablesType in Enum.GetValues(typeof(DbiCoefficientsTablesType)))
            {
                tmpBytes = TotalBytesOfType(dbiCoefficientsTablesType);
                if (tmpBytes > maxTotalBytes)
                    maxTotalBytes = tmpBytes;
            }
            int startIndex = 0;
            byte[] tmpArray = new byte[maxTotalBytes];
            foreach (DbiCoefficientsTablesType dbiCoefficientsTablesType in Enum.GetValues(typeof(DbiCoefficientsTablesType)))
            {
                tmpBytes = TotalBytesOfType(dbiCoefficientsTablesType);
                if (content.Length < startIndex + tmpBytes) return;
                Array.Copy(content, startIndex, tmpArray, 0, tmpBytes);
                Deserialize(dbiCoefficientsTablesType, tmpArray);
                startIndex += tmpBytes;
            }
        }

        #endregion
        public Int32 this[DbiCoefficientsTablesType type, int dataIndex, int bandMode, int channelIndex, int subBandIndex, int FilterBandMode]
        {
            get
            {
                return type switch
                {
                    DbiCoefficientsTablesType.InterpolationCoefficients => InterpolationCoefficients[bandMode, FilterBandMode, dataIndex],
                    DbiCoefficientsTablesType.LocalOscillatorCoefficients => LocalOscillatorCoefficients[bandMode, channelIndex, subBandIndex, dataIndex],
                    DbiCoefficientsTablesType.AntiImageCoefficients => AntiImageCoefficients[bandMode, channelIndex, subBandIndex, dataIndex],
                    DbiCoefficientsTablesType.FractionaryDelayCoefficients => FractionaryDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex],
                    DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients => OverlapPhaseFreqDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex],
                    DbiCoefficientsTablesType.TiAdc => TiAdcCoefficients[bandMode, channelIndex, subBandIndex, dataIndex],
                    DbiCoefficientsTablesType.AmpFreqCoefficients => AmpFreqCoefficients[bandMode, channelIndex, dataIndex],
                    DbiCoefficientsTablesType.PhaseFreqCoefficients => PhaseFreqCoefficients[bandMode, channelIndex, dataIndex],
                    DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients => MultiRadioInterpolationCoefficients[bandMode, dataIndex],
                    _ => 0,
                };
            }
            set
            {
                switch (type)
                {
                    case DbiCoefficientsTablesType.InterpolationCoefficients: InterpolationCoefficients[bandMode, FilterBandMode, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.LocalOscillatorCoefficients: LocalOscillatorCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.AntiImageCoefficients: AntiImageCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.FractionaryDelayCoefficients: FractionaryDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients: OverlapPhaseFreqDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.TiAdc: TiAdcCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.AmpFreqCoefficients: AmpFreqCoefficients[bandMode, channelIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.PhaseFreqCoefficients: PhaseFreqCoefficients[bandMode, channelIndex, dataIndex] = value; break;
                    case DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients: MultiRadioInterpolationCoefficients[bandMode, dataIndex] = value; break;
                }
            }
        }
        //用于单个子表的处理
        public byte[] Serialize(DbiCoefficientsTablesType dbiCoefficientsTablesType)
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            int dataCount = 0;
            switch (dbiCoefficientsTablesType)
            {
                case DbiCoefficientsTablesType.InterpolationCoefficients:
                    dataCount = InterpolationCoefficients.GetLength(InterpolationCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int FilterBandMode = 0; FilterBandMode < FilterbandModeCount; FilterBandMode++)
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                memoryStream.Write(Helper.StructToBytes(InterpolationCoefficients[bandMode, FilterBandMode, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.LocalOscillatorCoefficients:
                    dataCount = LocalOscillatorCoefficients.GetLength(LocalOscillatorCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                    memoryStream.Write(Helper.StructToBytes(LocalOscillatorCoefficients[bandMode, channelIndex, subBandIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.AntiImageCoefficients:
                    dataCount = AntiImageCoefficients.GetLength(AntiImageCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                    memoryStream.Write(Helper.StructToBytes(AntiImageCoefficients[bandMode, channelIndex, subBandIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.FractionaryDelayCoefficients:
                    dataCount = FractionaryDelayCoefficients.GetLength(FractionaryDelayCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                    memoryStream.Write(Helper.StructToBytes(FractionaryDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients:
                    dataCount = OverlapPhaseFreqDelayCoefficients.GetLength(OverlapPhaseFreqDelayCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                    memoryStream.Write(Helper.StructToBytes(OverlapPhaseFreqDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.TiAdc:
                    dataCount = TiAdcCoefficients.GetLength(TiAdcCoefficients.Rank - 1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                    memoryStream.Write(Helper.StructToBytes(TiAdcCoefficients[bandMode, channelIndex, subBandIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.AmpFreqCoefficients:
                    dataCount = AmpFreqCoefficients.GetLength(AmpFreqCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                memoryStream.Write(Helper.StructToBytes(AmpFreqCoefficients[bandMode, channelIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.PhaseFreqCoefficients:
                    dataCount = PhaseFreqCoefficients.GetLength(PhaseFreqCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                memoryStream.Write(Helper.StructToBytes(PhaseFreqCoefficients[bandMode, channelIndex, dataIndex]));
                    break;
                case DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients:
                    dataCount = MultiRadioInterpolationCoefficients.GetLength(MultiRadioInterpolationCoefficients.Rank-1);
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                            memoryStream.Write(Helper.StructToBytes(MultiRadioInterpolationCoefficients[bandMode, dataIndex]));
                    break;
            }

            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
        public void Deserialize(DbiCoefficientsTablesType dbiCoefficientsTablesType, byte[] content)
        {
            int perBandModeBytes = 0;
            int perChannelBytes = 0;
            int perFilterBandModeBytes = 0;
            int perSubBandBytes = 0;
            int byteIndex = 0;
            int dataCount = PerDataCount(dbiCoefficientsTablesType);
            int totalBytes = TotalBytesOfType(dbiCoefficientsTablesType);
            switch (dbiCoefficientsTablesType)
            {
                case DbiCoefficientsTablesType.InterpolationCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perFilterBandModeBytes = totalBytes / BandModeCount / FilterbandModeCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int FilterBandMode = 0; FilterBandMode < FilterbandModeCount; FilterBandMode++)
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                            {
                                byteIndex = bandMode * perBandModeBytes + FilterBandMode * perFilterBandModeBytes + dataIndex * sizeof(Int32);
                                if (content.Length < byteIndex + 4)
                                    break;
                                InterpolationCoefficients[bandMode, FilterBandMode, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                            }
                    break;
                case DbiCoefficientsTablesType.LocalOscillatorCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    perSubBandBytes = totalBytes / BandModeCount / ChannelCount / SubbandCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                {
                                    byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + subBandIndex * perSubBandBytes + dataIndex * sizeof(Int32);
                                    if (content.Length < byteIndex + 4)
                                        break;
                                    LocalOscillatorCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                                }
                    break;
                case DbiCoefficientsTablesType.AntiImageCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    perSubBandBytes = totalBytes / BandModeCount / ChannelCount / SubbandCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                {
                                    byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + subBandIndex * perSubBandBytes + dataIndex * sizeof(Int32);
                                    if (content.Length < byteIndex + 4)
                                        break;
                                    AntiImageCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));

                                }
                    break;
                case DbiCoefficientsTablesType.FractionaryDelayCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    perSubBandBytes = totalBytes / BandModeCount / ChannelCount / SubbandCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                {
                                    byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + subBandIndex * perSubBandBytes + dataIndex * sizeof(Int32);
                                    if (content.Length < byteIndex + 4)
                                        break;
                                    FractionaryDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                                }
                    break;

                case DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    perSubBandBytes = totalBytes / BandModeCount / ChannelCount / SubbandCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                {
                                    byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + subBandIndex * perSubBandBytes + dataIndex * sizeof(Int32);
                                    if (content.Length < byteIndex + 4)
                                        break;
                                    OverlapPhaseFreqDelayCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                                }
                    break;
                case DbiCoefficientsTablesType.TiAdc:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    perSubBandBytes = totalBytes / BandModeCount / ChannelCount / SubbandCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int subBandIndex = 0; subBandIndex < SubbandCount; subBandIndex++)
                                for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                                {
                                    byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + subBandIndex * perSubBandBytes + dataIndex * sizeof(Int32);
                                    if (content.Length < byteIndex + 4)
                                        break;
                                    TiAdcCoefficients[bandMode, channelIndex, subBandIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                                }
                    break;
                case DbiCoefficientsTablesType.AmpFreqCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                            {
                                byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + dataIndex * sizeof(Int32);
                                if (content.Length < byteIndex + 4)
                                    break;
                                AmpFreqCoefficients[bandMode, channelIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                            }
                    break;
                case DbiCoefficientsTablesType.PhaseFreqCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    perChannelBytes = totalBytes / BandModeCount / ChannelCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                    {
                        for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                        {
                            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                            {
                                byteIndex = bandMode * perBandModeBytes + channelIndex * perChannelBytes + dataIndex * sizeof(Int32);
                                if (content.Length < byteIndex + 4)
                                    break;
                                PhaseFreqCoefficients[bandMode, channelIndex, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                            }
                        }
                    }
                    break;
                case DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients:
                    perBandModeBytes = totalBytes / BandModeCount;
                    for (int bandMode = 0; bandMode < BandModeCount; bandMode++)
                        for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
                        {
                            byteIndex = bandMode * perBandModeBytes + dataIndex * sizeof(Int32);
                            if (content.Length < byteIndex + 4)
                                break;
                            MultiRadioInterpolationCoefficients[bandMode, dataIndex] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));

                        }
                    break;
            }
        }
    }
}
