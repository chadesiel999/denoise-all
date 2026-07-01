#if DBI

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ScopeX.Hardware.Driver
{
    internal static partial class CoefficientsTableSender_DBI
    {
        /// <summary>
        /// 是否需要开启文件下发的CRC检查
        /// </summary>
        internal static Boolean CheckCrcEnable = true;

        private static readonly Dictionary<DbiCoefficientsTablesType, AcqDbiCoefficientsRecord> _AcqDbiCoefficientsRecordTables = new()
        { 
            {DbiCoefficientsTablesType.InterpolationCoefficients_2fold,    new(0xE0, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level1_InterpolationCoefficients,   new(0x1, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients, new(0x2,  8, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level1_AntiImageCoefficients,       new(0x4, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level2_InterpolationCoefficients,   new(0x41, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients, new(0x42,  8, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Level2_AntiImageCoefficients,       new(0x44, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.Sub_AmpCoefficientFile,             new(0xC1, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},


            {DbiCoefficientsTablesType.InterpolationCoefficients,           new(0x81, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.LocalOscillatorCoefficients,         new(0x82,  8, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.AntiImageCoefficients,               new(0x84, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.FractionaryDelayCoefficients,        new(0x88, 10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients,   new(0xA0,10, HdCtrl_Coefficient.Send2AcqBoardByRegister, HdCtrl_Coefficient.Send2AcqBoardByDMA)},
            {DbiCoefficientsTablesType.TiAdc,                               new(0x6,  7, HdCtrl_Coefficient.SendTiadcByRegister, null/* 没有DMA方式 */)},
        };

        private static readonly Dictionary<DbiCoefficientsTablesType, ProcDbiCoefficientsRecord> _ProcDbiCoefficientsRecordTables = new()
        {
            {DbiCoefficientsTablesType.AmpFreqCoefficients,                 new(0x1, 10, HdCtrl_Coefficient.Send2ProcBoardByRegister, HdCtrl_Coefficient.Send2ProcBoardByDMA)},
            {DbiCoefficientsTablesType.PhaseFreqCoefficients,               new(0x2, 12, HdCtrl_Coefficient.Send2ProcBoardByRegister, HdCtrl_Coefficient.Send2ProcBoardByDMA)},
            {DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients, new(0x0, 10, HdCtrl_Coefficient.SendMultiRadioInterpolationByRegister, null/* 没有DMA方式 */)},
        };

        /// <summary>
        /// Key 格式为$"AcqBd_{acqBdNo}_{DbiCoefficientsTablesType}";value = crcCode
        /// </summary>
        private static Dictionary<string, int> SendCoefficientsTablesHistory = new();

        internal static void ClearSendHistory()
        {
            AmpFreqCoefficientsSentHistory[ChannelId.C1] = -1;
            AmpFreqCoefficientsSentHistory[ChannelId.C2] = -1;
            AmpFreqCoefficientsSentHistory[ChannelId.C3] = -1;
            AmpFreqCoefficientsSentHistory[ChannelId.C4] = -1;

            SendCoefficientsTablesHistory.Clear();
        }
        internal class Sub_AmpCoefficientFileInfo
        {
            public string FileName = "";
            public int CRCCode = 0;
            public bool bOk = false;
        }

        internal class FreqRangeItem
        {
            public UInt64 StartFreq { get; set; } // 起始频率 (包含)
            public UInt64 EndFreq { get; set; }   // 结束频率 (不包含)
            public Sub_AmpCoefficientFileInfo FileInfo { get; set; } // 对应的文件信息
        }

        #region acq board
        private const int DMAWriteMultipleBytes = 8192;//512;
        private static void CorrectDmaLength(ref List<byte> array, int lastData)
        {
            ////补齐，使用最后一个数据作为补齐数据
            //int remain = (array.Count % DMAWriteMultipleBytes);
            //if (remain > 0)
            //{
            //    remain = (DMAWriteMultipleBytes - remain) / 4;//4=int32是4字节
            //    if (remain > 0)
            //    {
            //        for (int i = 0; i < remain; i++)
            //            array.AddRange(BitConverter.GetBytes(lastData));
            //    }
            //}
            ////补齐，使用最后一个数据作为补齐数据
            int remain = (DMAWriteMultipleBytes - array.Count);
            if (remain > 0)
            {
                remain = (remain) / 4;//4=int32是4字节
                if (remain > 0)
                {
                    for (int i = 0; i < remain; i++)
                        array.AddRange(BitConverter.GetBytes(lastData));
                }
            }
        }
        private static byte[] GenerateDmaDataByContinueAddressAndPerData(int[] dataArray, int dataCount, int addressBitCount)
        {
            List<byte> dataList = new List<byte>();
            Int32 data = 0;
            for (int i = 0; i < dataCount; i++)
            {
                data = (dataArray[i] << addressBitCount) | i;
                dataList.AddRange(BitConverter.GetBytes(data));
            }
            CorrectDmaLength(ref dataList, data);
            return dataList.ToArray();
        }
        private static byte[] GenerateDmaDataByTiAdcFormat(int[] dataArray, int dataCount, int addressBitCount, int ImaginaryPartBitCount)
        {
            List<byte> dataList = new List<byte>();
            Int32 data = 0;
            for (int i = 0; i < dataCount / 2; i++)
            {
                data = dataArray[i] << (addressBitCount + ImaginaryPartBitCount);//实部
                data |= (dataArray[i + 1] << addressBitCount);//虚部
                data |= i;
                dataList.AddRange(BitConverter.GetBytes(data));
            }
            CorrectDmaLength(ref dataList, data);
            return dataList.ToArray();
        }
        /// <summary>
        /// 如MultiRadioInterpolationCoefficients
        /// </summary>
        /// <param name="dataArray"></param>
        /// <param name="dataCount"></param>
        /// <param name="addressBitCount"></param>
        /// <param name="firstTableCount"></param>
        /// <param name="secondTableAddressMark"></param>
        /// <returns></returns>
        private static byte[] GenerateDmaDataByTwoPartFormat(int[] dataArray, int dataCount, int addressBitCount, int firstTableCount, int secondTableAddressMark)
        {
            List<byte> dataList = new List<byte>();
            Int32 data = 0;
            for (int i = 0; i < dataCount; i++)
            {
                data = dataArray[i] << (addressBitCount);
                if (i < firstTableCount)
                    data |= i;
                else
                    data |= (secondTableAddressMark | (i - firstTableCount));
                dataList.AddRange(BitConverter.GetBytes(data));
            }
            CorrectDmaLength(ref dataList, data);
            return dataList.ToArray();
        }

        private static DBI_CoefTableSendItem? GetAcqDefineItem(DBI_CoefTableSendItem sendItem, DbiCoefficientsTablesType dbiType)
        {
            Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
            if (acqBoardCoefficientsTablesSendDefine == null || (!acqBoardCoefficientsTablesSendDefine.ContainsKey(dbiType)))
                return null;
            foreach (DBI_CoefTableSendItem defineItem in acqBoardCoefficientsTablesSendDefine[dbiType])
            { 
                if (sendItem.Equal(defineItem)) return defineItem;
            }
            return null;
        }

        private static Dictionary<DbiCoefficientsTablesType, Int32> _DbiCoefficientsDataLengthDefine = new()
        {
            [DbiCoefficientsTablesType.LocalOscillatorCoefficients] = 160,
        };

        internal static void SaveCoefToFile(DbiCoefficientsTablesType sendType, DBI_CoefTableSendItem sendItem)
        {
            DBI_CoefTableSendItem? defineItem = GetAcqDefineItem(sendItem, sendType);
            if (defineItem == null)
            {
                PublicFunc.WriteLog("[SaveCoefToFile]AcqBdCoefTablesSendDefine no define!!!");
                return;
            }
            if (_DbiCoefficientsDataLengthDefine.ContainsKey(sendType))
            {
                Int32[] dataarray = new Int32[_DbiCoefficientsDataLengthDefine[sendType]*5];
                Int32[] calcdata = new Int32[160];
                for (Int32 i = 0; i < _DbiCoefficientsDataLengthDefine[sendType]; i++)
                {
                    calcdata[i] = DbiCoefficientsTables.Default[sendType, i, (Int32)defineItem.BandMode, (Int32)defineItem.ChannelID, (Int32)defineItem.SubbandIndex, (Int32)(Int32)defineItem.FilterbandMode];
                }
                for (Int32 i = 0; i < 5; i++)//将值复制5次，使长度扩展成5倍
                { 
                    for(Int32 j = 0; j < 160; j++)
                    {
                        dataarray[i * 160 + j] = calcdata[j];
                    }
                }
                Misc.WriteCaliCoefDataToFile(defineItem.DataFileName, dataarray);
            }
        }

        private static Int32[]? GenerateLocalFreqCoeff(UInt64 localFreqByHz)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\dbi_factor");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)localFreqByHz, "localFreqByHz");

            List<String> codelist = new List<String>()
            {
                "localFreq = cell2mat(localFreqByHz);",
                $"lo_signal = lo_data_gen(localFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("lo_signal", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }
        private static Int32[]? GenerateLocalFreqCoeff_Level1(UInt64 localFreqByHz)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\dbi_factor");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)localFreqByHz, "localFreqByHz");


            List<String> codelist = new List<String>()
            {
                "localFreq = cell2mat(localFreqByHz);",
                $"lo_signal = lo_data_gen_level1(localFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("lo_signal", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }
        private static Int32[]? GenerateLocalFreqCoeff_Level2(UInt64 localFreqByHz)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\dbi_factor");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)localFreqByHz, "localFreqByHz");


            List<String> codelist = new List<String>()
            {
                "localFreq = cell2mat(localFreqByHz);",
                $"lo_signal = lo_data_gen_level2(localFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("lo_signal", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }

        private static Int32[]? GenerateAntImageCoeff(AntImageFreq freqRange)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\anti");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)freqRange.LeftFreqByHz, "LeftFreqByHz");
            ans = Matlab.Default.PutData((Double)freqRange.RightFreqByHz, "RightFreqByHz");

            List<String> codelist = new List<String>()
            {
                "LeftFreq = cell2mat(LeftFreqByHz);",
                "RightFreq = cell2mat(RightFreqByHz);",
                $"anti_coe_sub = design_fir_bandpass_filter_digital(LeftFreq, RightFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("anti_coe_sub", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }
        private static Int32[]? GenerateAntImageCoeff_Level1(AntImageFreq freqRange)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\anti");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)freqRange.LeftFreqByHz, "LeftFreqByHz");
            ans = Matlab.Default.PutData((Double)freqRange.RightFreqByHz, "RightFreqByHz");

            List<String> codelist = new List<String>()
            {
                "LeftFreq = cell2mat(LeftFreqByHz);",
                "RightFreq = cell2mat(RightFreqByHz);",
                $"anti_coe_sub = design_fir_bandpass_filter_digital_level1(LeftFreq, RightFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("anti_coe_sub", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }
        private static Int32[]? GenerateAntImageCoeff_Level2(AntImageFreq freqRange)
        {
            var ans = Matlab.Default.SetWorkFolder($"C:\\matlab\\anti");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData((Double)freqRange.LeftFreqByHz, "LeftFreqByHz");
            ans = Matlab.Default.PutData((Double)freqRange.RightFreqByHz, "RightFreqByHz");

            List<String> codelist = new List<String>()
            {
                "LeftFreq = cell2mat(LeftFreqByHz);",
                "RightFreq = cell2mat(RightFreqByHz);",
                $"anti_coe_sub = design_fir_bandpass_filter_digital_level2(LeftFreq, RightFreq);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));


            ans = Matlab.Default.TryGetData("anti_coe_sub", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return ((Double[])data).Select(o => (Int32)o).ToArray();
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Int32[] result = new Int32[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = (Int32)tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    return result;
                }
            }
            return null;
        }

        private static readonly Dictionary<Int32, UInt64> _DefaultLocalFreqByHz = new()
        {
            [0] = 0,
            [1] = 10_000_000_000,
            [2] = 15_000_000_000,
            [3] = 22_500_000_000,
        };

        private static readonly Dictionary<Int32, UInt64> _DefaultLocalFreqByHz_Level1 = new()
        {
            [0] = 0,
            [1] = 1_000_000_000,
            [2] = 1_000_000_000,
            [3] = 1_000_000_000,
        };

        private static readonly Dictionary<Int32, UInt64> _DefaultLocalFreqByHz_Level2 = new()
        {
            [0] = 0,
            [1] = 3_000_000_000,
            [2] = 3_000_000_000,
            [3] = 3_000_000_000,
        };

        private static readonly Dictionary<Int32, AntImageFreq> _DefaultAntImageFreq = new()
        {
            [0] = new AntImageFreq(0,              6_000_000_000),
            [1] = new AntImageFreq(6_000_000_000,  9_500_000_000),
            [2] = new AntImageFreq(9_500_000_000, 14_500_000_000),
            [3] = new AntImageFreq(14_500_000_000, 22_000_000_000),
        };


        private static readonly Dictionary<Int32, AntImageFreq> _DefaultAntImageFreq_Level1 = new()
        {
            [0] = new AntImageFreq(0_750_000_000,   1_350_000_000),
            [1] = new AntImageFreq(0_750_000_000,   1_350_000_000),
            [2] = new AntImageFreq(0_750_000_000,   1_350_000_000),
            [3] = new AntImageFreq(0_750_000_000,   1_350_000_000),
        };

        private static readonly Dictionary<Int32, AntImageFreq> _DefaultAntImageFreq_Level2 = new()
        {
            [0] = new AntImageFreq(3_750_000_000, 4_350_000_000),
            [1] = new AntImageFreq(3_750_000_000, 4_350_000_000),
            [2] = new AntImageFreq(3_750_000_000, 4_350_000_000),
            [3] = new AntImageFreq(3_750_000_000, 4_350_000_000),
        };

        private static readonly List<FreqRangeItem> _configList = new List<FreqRangeItem>
        {
            new FreqRangeItem{StartFreq = 11_000_000_000,EndFreq = 11_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_115G.txt" } },
            new FreqRangeItem{StartFreq = 10_500_000_000,EndFreq = 11_000_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_110G.txt" } },
            new FreqRangeItem{StartFreq = 10_000_000_000,EndFreq = 10_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_105G.txt" } },
            new FreqRangeItem{StartFreq = 9_500_000_000, EndFreq = 10_000_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_100G.txt" } },
            new FreqRangeItem{StartFreq = 9_000_000_000, EndFreq = 9_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_95G.txt" } },
            new FreqRangeItem{StartFreq = 8_500_000_000, EndFreq = 9_000_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_90G.txt" } },
            new FreqRangeItem{StartFreq = 8_000_000_000, EndFreq = 8_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_85G.txt" } },
            new FreqRangeItem{StartFreq = 7_500_000_000, EndFreq = 8_000_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_80G.txt" } },
            new FreqRangeItem{StartFreq = 7_000_000_000, EndFreq = 7_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_75G.txt" } },
            new FreqRangeItem{StartFreq = 6_500_000_000, EndFreq = 7_000_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_70G.txt" } },
            new FreqRangeItem{StartFreq = 6_000_000_000, EndFreq = 6_500_000_000, FileInfo  = new Sub_AmpCoefficientFileInfo{ FileName =  $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\amp\sub2\AFC_sub2_65G.txt" } },
        };


        internal static void SendAmpSubCoff(Int32 sub,UInt64 localFreqByHz)
        {
            foreach (Int32 subbandid in _DefaultLocalFreqByHz.Keys)
            {
                string DataFileName;

                
                DataFileName = _configList.FirstOrDefault(x => localFreqByHz > x.StartFreq && localFreqByHz <= x.EndFreq)?.FileInfo.FileName??"";
                Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(DataFileName);
                if (dataArray != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.Sub_AmpCoefficientFile, ChannelId.C1, subbandid, dataArray);
                    PublicFunc.WriteLog($"[Sub_AmpCoefficientFile]{subbandid} send  ok");
                }
                else
                {
                    PublicFunc.WriteLog($"[Sub_AmpCoefficientFile]{subbandid} send  fail");
                }
            }
        }

        internal static void SendLocalFreqCoeff(Dictionary<Int32, UInt64> localFreqByHz)
        {
            foreach (Int32 subbandid in _DefaultLocalFreqByHz.Keys)
            {
                UInt64 localfreq = localFreqByHz.ContainsKey(subbandid) ? localFreqByHz[subbandid] : _DefaultLocalFreqByHz[subbandid];
                Int32[]? coeff = GenerateLocalFreqCoeff(localfreq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.LocalOscillatorCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"[SendLocalFreqCoeff]subband{subbandid} SendCoeff {localfreq}");
                }
                else
                {
                    Trace.WriteLine($"[SendLocalFreqCoeff]subband{subbandid} Generate {localfreq} Hz coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null && 
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.LocalOscillatorCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.LocalOscillatorCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.LocalOscillatorCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.LocalOscillatorCoefficients][subbandid] });
                    }
                }
            }
        }
        internal static void SendLocalFreqCoeff_Level1(Dictionary<Int32, UInt64> localFreqByHz)
        {
            foreach (Int32 subbandid in _DefaultLocalFreqByHz_Level1.Keys)
            {
                UInt64 localfreq = localFreqByHz.ContainsKey(subbandid) ? localFreqByHz[subbandid] : _DefaultLocalFreqByHz_Level1[subbandid];
                Int32[]? coeff = GenerateLocalFreqCoeff_Level1(localfreq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"[SendLocalFreqCoeff_Level1]subband{subbandid} SendCoeff {localfreq}");
                }
                else
                {
                    Trace.WriteLine($"[SendLocalFreqCoeff_Level1]subband{subbandid} Generate {localfreq} Hz coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null &&
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level1_LocalOscillatorCoefficients][subbandid] });
                    }
                }
            }
        }
        internal static void SendLocalFreqCoeff_Level2(Dictionary<Int32, UInt64> localFreqByHz)
        {
            foreach (Int32 subbandid in _DefaultLocalFreqByHz_Level2.Keys)
            {
                UInt64 localfreq = localFreqByHz.ContainsKey(subbandid) ? localFreqByHz[subbandid] : _DefaultLocalFreqByHz_Level2[subbandid];
                Int32[]? coeff = GenerateLocalFreqCoeff_Level2(localfreq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"[SendLocalFreqCoeff_Level2]subband{subbandid} SendCoeff {localfreq}");
                }
                else
                {
                    Trace.WriteLine($"[SendLocalFreqCoeff_Level2]subband{subbandid} Generate {localfreq} Hz coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null &&
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients][subbandid] });
                    }
                }
            }
        }

        internal static void SendAntiImageCoeff(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            foreach (Int32 subbandid in _DefaultAntImageFreq.Keys)
            {
                AntImageFreq freq = freqByHz.ContainsKey(subbandid) ? freqByHz[subbandid] : _DefaultAntImageFreq[subbandid];
                Int32[]? coeff = GenerateAntImageCoeff(freq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.AntiImageCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"*********************************subband {subbandid + 1} config Anti Image freq {freq.LeftFreqByHz} Hz - {freq.RightFreqByHz} Hz");
                }
                else
                {
                    Trace.WriteLine($"[SendAntiImageCoeff]Generate coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null && 
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.AntiImageCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.AntiImageCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.AntiImageCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.AntiImageCoefficients][subbandid] });
                    }
                }
            }
        }
        internal static void SendAntiImageCoeff_Level1(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            foreach (Int32 subbandid in _DefaultAntImageFreq_Level1.Keys)
            {
                AntImageFreq freq = freqByHz.ContainsKey(subbandid) ? freqByHz[subbandid] : _DefaultAntImageFreq_Level1[subbandid];
                Int32[]? coeff = GenerateAntImageCoeff_Level1(freq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.Level1_AntiImageCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"*********************************subband {subbandid + 1} config_Level1 Anti Image freq  {freq.LeftFreqByHz} Hz - {freq.RightFreqByHz} Hz");
                }
                else
                {
                    Trace.WriteLine($"[SendAntiImageCoeff]Level1_Generate coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null &&
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.Level1_AntiImageCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level1_AntiImageCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.Level1_AntiImageCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level1_AntiImageCoefficients][subbandid] });
                    }
                }
            }
        }
        internal static void SendAntiImageCoeff_Level2(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            foreach (Int32 subbandid in _DefaultAntImageFreq_Level2.Keys)
            {
                AntImageFreq freq = freqByHz.ContainsKey(subbandid) ? freqByHz[subbandid] : _DefaultAntImageFreq_Level2[subbandid];
                Int32[]? coeff = GenerateAntImageCoeff_Level2(freq);
                if (coeff != null)
                {
                    SendCoeff(DbiCoefficientsTablesType.Level2_AntiImageCoefficients, ChannelId.C1, subbandid, coeff);
                    Trace.WriteLine($"*********************************subband {subbandid + 1} config_Level2 Anti Image freq  {freq.LeftFreqByHz} Hz - {freq.RightFreqByHz} Hz");
                }
                else
                {
                    Trace.WriteLine($"[SendAntiImageCoeff]Level2_Generate coeff failed.");
                    Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
                    if (acqBoardCoefficientsTablesSendDefine != null &&
                        acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.Level2_AntiImageCoefficients) &&
                        acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level2_AntiImageCoefficients].Count > subbandid)
                    {
                        Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.Level2_AntiImageCoefficients, new List<DBI_CoefTableSendItem>() { acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.Level2_AntiImageCoefficients][subbandid] });
                    }
                }
            }
        }
        internal static void SendInterpolationCoefficients_Level1(Int32 Ts_mode,Int32 sub )
        {
            string DataFileName_level1;
            if (Ts_mode == 1)
            {
                DataFileName_level1 = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode1_low.txt";
            }
            else
            {
                if(sub == 0 )
                    DataFileName_level1 = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub1_interp4_mode2_low.txt";
                else
                    DataFileName_level1 = $@".\CaliData\CoeFiles\dbi_ch1_coe_gen\ts\level1_m1_ch1_sub2_interp4_mode2_pass.txt";
            }
            Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(DataFileName_level1); //系数发送
        
                foreach (Int32 subbandid in _DefaultAntImageFreq.Keys)
                {
                    if(dataArray != null)
                    {
                    SendCoeff(DbiCoefficientsTablesType.Level1_InterpolationCoefficients, ChannelId.C1, subbandid, dataArray);
                    PublicFunc.WriteLog($"[SendInterpolationCoefficients_Level1]{subbandid} send  ok");
                    }

                else
                    {
                        PublicFunc.WriteLog($"[SendInterpolationCoefficients_Level1]{subbandid} send  fail");
                    }
                }
        }
        internal static void SendInterpolationCoefficients(Dictionary<Int32, Int32> subbandInfo)
        {
            Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
            if (acqBoardCoefficientsTablesSendDefine == null || !acqBoardCoefficientsTablesSendDefine.ContainsKey(DbiCoefficientsTablesType.InterpolationCoefficients))
                return;

            foreach (Int32 subbandid in subbandInfo.Keys)
            {
                foreach (DBI_CoefTableSendItem senditem in acqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.InterpolationCoefficients])
                {
                    if (senditem.SubbandIndex == subbandInfo[subbandid])
                    {
                        Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(senditem.DataFileName); //系数发送
                        if (dataArray != null)
                        {
                            SendCoeff(DbiCoefficientsTablesType.InterpolationCoefficients, ChannelId.C1, subbandid, dataArray);
                            PublicFunc.WriteLog($"[SendInterpolationCoefficients]{subbandid} send {subbandInfo[subbandid]} ok");
                        }
                        break;
                    }
                }
            }
        }

        private static void SendCoeff(DbiCoefficientsTablesType coeType, ChannelId chnlId, Int32 subbandId, Int32[] dataArray)
        {
            var acqBdNo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, chnlId, subbandId)?.AcqBdNo;
            if (acqBdNo != null && _AcqDbiCoefficientsRecordTables.ContainsKey(coeType))
            {
                _AcqDbiCoefficientsRecordTables[coeType].SendByRegister?.Invoke(_AcqDbiCoefficientsRecordTables[coeType].TypeCodeDefine, (AcqBdNo)acqBdNo, dataArray, dataArray.Length);
            }
        }

        /// <summary>
        /// 根据AcqBdCoefTablesSendDefine里面的定义进行发送
        /// </summary>
        /// <param name="sendType"></param>
        /// <param name="sendItems"></param>
        internal static void Send2AcqBoardByDefineItem(DbiCoefficientsTablesType sendType, List<DBI_CoefTableSendItem> sendItems)
        {
            if (!_AcqDbiCoefficientsRecordTables.ContainsKey(sendType))
            {
                PublicFunc.WriteLog("[Send2AcqBoardByDefineItem]_AcqDbiCoefficientsRecordTables no define!!!");
                return;
            }

            foreach (DBI_CoefTableSendItem sendItem in sendItems)
            {
                //DBI_CoefTableSendItem? defineItem = GetAcqDefineItem(sendItem, sendType);
                //if (defineItem == null)
                //{
                //    PublicFunc.WriteLog("[Send2AcqBoardByDefineItem]AcqBdCoefTablesSendDefine no define!!!");
                //    continue;
                //}

                //Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(defineItem.DataFileName); //系数发送
                //if (dataArray == null)
                //{
                //    PublicFunc.WriteLog($"[Send2AcqBoardByDefineItem]{defineItem.DataFileName} not exit!!!");
                //    continue;
                //}

                if (sendItem == null)
                {
                    PublicFunc.WriteLog("[Send2AcqBoardByDefineItem]AcqBdCoefTablesSendDefine no define!!!");
                    continue;
                }

                Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(sendItem.DataFileName); //系数发送
                if (dataArray == null)
                {
                    PublicFunc.WriteLog($"[Send2AcqBoardByDefineItem]{sendItem.DataFileName} not exit!!!");
                    continue;
                }

                int dataCount = dataArray.Length;

                //var tmp = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, sendItem.ChannelID, sendItem.SubbandIndex);      
                var tmp = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(1, sendItem.ChannelID, sendItem.SubbandIndex);//HTF 10313  DBI_DEBUG
                if (tmp == null)
                    continue;

                AcqBdNo acqBdNo = tmp.AcqBdNo;

                if (CheckCrcEnable)
                {
                    int crcCode = Misc.GenerateCRCCode(dataArray);
                    string HistoryKey = $"AcqBd_{acqBdNo}_{sendType}";
                    if (SendCoefficientsTablesHistory.ContainsKey(HistoryKey))
                    {
                        if (SendCoefficientsTablesHistory[HistoryKey] == crcCode)
                        {
                            PublicFunc.WriteLog($"[Send2AcqBoardByDefineItem]CRC({crcCode}) is same!!!");
                            continue;//已经发送，不需要重新发送
                        }

                        SendCoefficientsTablesHistory.Remove(HistoryKey);
                    }
                    SendCoefficientsTablesHistory.Add(HistoryKey, crcCode);
                }

                //DownloadBlockDataMode downMode = DbiCoefficientsTables.Default[sendType];
                //if (downMode == DownloadBlockDataMode.Register)
                _AcqDbiCoefficientsRecordTables[sendType].SendByRegister?.Invoke(_AcqDbiCoefficientsRecordTables[sendType].TypeCodeDefine, acqBdNo, dataArray, dataCount);
                //else
                //{
                //    Byte[] dmaByes = GenerateDmaDataByContinueAddressAndPerData(dataArray, dataCount, _AcqDbiCoefficientsRecordTables[sendType].AddressBitCount);
                //    _AcqDbiCoefficientsRecordTables[sendType].SendByDma?.Invoke(_AcqDbiCoefficientsRecordTables[sendType].TypeCodeDefine, acqBdNo, dmaByes);
                //}
            }
        }
        #endregion

        #region proc board

        private static void SendProcBoardCoefficientsByRegister((UInt32 TypeCode, UInt32 wen, UInt32 addr, UInt32 lowData, UInt32 HighData, UInt32 DspReset) registers, uint dataTypeCode, Int32 chnlId, int[] dataArray, int dataCount)
        {
            if (registers.DspReset != 0)
            {
                HdIO.WriteReg(registers.DspReset, 0);
                HdIO.WriteReg(registers.DspReset, 1);
                HdIO.WriteReg(registers.DspReset, 0);
            }
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, 0x1u << chnlId);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, dataTypeCode);
            for (int dataIndex = 0; dataIndex < dataCount; dataIndex++)
            {
                //UInt32 data = (UInt32)DbiCoefficientsTables.Default[dataChanged.Key, dataIndex, (int)sendItem.BandMode, (int)sendItem.ChannelID, sendItem.SubbandIndex, (int)sendItem.FilterbandMode];
                UInt32 data = (UInt32)dataArray![dataIndex];
                HdIO.WriteReg(registers.wen, 0);
                HdIO.WriteReg(registers.addr, (UInt32)dataIndex);
                HdIO.WriteReg(registers.lowData, (UInt32)data & 0xffff);
                HdIO.WriteReg(registers.HighData, (UInt32)(data >> 16) & 0xffff);
                HdIO.WriteReg(registers.wen, 1);
            }
            HdIO.WriteReg(registers.wen, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProFactorSelect, 0);
        }

        private static void SendProcBoardCoefficientsByDMA(DbiCoefficientsTablesType dbiCoefficientsTablesType, uint dataType, int[] dataArray, int dataCount)
        {
            byte[]? dmaWriteData = dbiCoefficientsTablesType switch
            {
                DbiCoefficientsTablesType.PhaseFreqCoefficients => GenerateDmaDataByContinueAddressAndPerData(dataArray, dataCount, 12),
                DbiCoefficientsTablesType.AmpFreqCoefficients => GenerateDmaDataByContinueAddressAndPerData(dataArray, dataCount, 10),
                _ => null,
            };
            if (dmaWriteData == null)
                return;
            //需要验证
            //HdIO.WriteReg(S6BdReg.W.DBI_DBI_FACTOR_SELSECT_S6, 0U);

            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 1U);
            HdIO.WriteReg(ProcBdReg.W.DBI_DMA_RST_PRO, 0);
            HdIO.DelayByUs(500);
            //if (dataType == 32)
            //    dataType = 16;
            //if (dataType == 128)
            //    dataType = 64;
            HdIO.WriteReg(ProcBdReg.W.DBI_DBI_FACTOR_SELSECT_PRO, dataType);
            HdIO.DelayByUs(500);
            bool bOK = HdIO.DMAWrite(0, dmaWriteData, (UInt32)dmaWriteData.Length);
        }

        private static DBI_CoefTableSendItem? GetProcDefineItem(DBI_CoefTableSendItem sendItem, DbiCoefficientsTablesType dbiType)
        {
            Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>? acqBoardCoefficientsTablesSendDefine = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcqBdCoefTablesSendDefine ?? null;
            if (acqBoardCoefficientsTablesSendDefine == null || acqBoardCoefficientsTablesSendDefine.Count == 0)
                return null;

            foreach (DBI_CoefTableSendItem defineItem in acqBoardCoefficientsTablesSendDefine[dbiType])
            {
                if (dbiType == DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients)
                {
                    if (sendItem.BandMode == defineItem.BandMode)
                        return defineItem;
                }
                else
                {
                    if (sendItem.ChannelID == defineItem.ChannelID && sendItem.BandMode == defineItem.BandMode && sendItem.ChnlScaleIndex == defineItem.ChnlScaleIndex)
                        return defineItem;
                } 
            }

            return null;
        }

        private static void Send2ProcBoardByDefine(DbiCoefficientsTablesType sendType, List<DBI_CoefTableSendItem> sendItems)
        {
            if (!_ProcDbiCoefficientsRecordTables.ContainsKey(sendType))
            {
                PublicFunc.WriteLog("[Send2ProcBoardByDefine]_ProcDbiCoefficientsRecordTables no define!!!");
                return;
            }

            foreach (DBI_CoefTableSendItem sendItem in sendItems)
            {
                DBI_CoefTableSendItem? defineItem = GetProcDefineItem(sendItem, sendType);
                if (defineItem == null) 
                {
                    PublicFunc.WriteLog("[Send2ProcBoardByDefine]AcqBdCoefTablesSendDefine no define!!!");
                    continue;
                }

                Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(defineItem.DataFileName);
                if (dataArray == null)
                {
                    PublicFunc.WriteLog($"[Send2ProcBoardByDefine]{defineItem.DataFileName} not exit!!!");
                    continue;
                }

                int dataCount = dataArray.Length;

                if (CheckCrcEnable)
                {
                    int crcCode = Misc.GenerateCRCCode(dataArray);
                    string HistoryKey = $"ProcBd_{sendType}";
                    if (SendCoefficientsTablesHistory.ContainsKey(HistoryKey))
                    {
                        if (SendCoefficientsTablesHistory[HistoryKey] == crcCode)
                        {
                            PublicFunc.WriteLog($"[Send2ProcBoardByDefine]CRC({crcCode}) is same!!!");
                            continue;
                        }
                        SendCoefficientsTablesHistory.Remove(HistoryKey);
                    }
                    SendCoefficientsTablesHistory.Add(HistoryKey, crcCode);
                }

                //DownloadBlockDataMode downMode = DbiCoefficientsTables.Default[sendType];
                //if (downMode == DownloadBlockDataMode.Register)
                    _ProcDbiCoefficientsRecordTables[sendType].SendByRegister?.Invoke(_ProcDbiCoefficientsRecordTables[sendType].TypeCodeDefine, dataArray, dataCount, sendItem.ChannelID);
                //else
                //{
                //    Byte[] dmaByes = GenerateDmaDataByContinueAddressAndPerData(dataArray, dataCount, _ProcDbiCoefficientsRecordTables[sendType].AddressBitCount);
                //    _ProcDbiCoefficientsRecordTables[sendType].SendByDma?.Invoke(_ProcDbiCoefficientsRecordTables[sendType].TypeCodeDefine, dmaByes);
                //}
            }
        }

        private static void Send2ProcBoardByDefine(DbiCoefficientsTablesType sendType, List<DBI_CoefTableSendItem> sendItems,AnaChnlScaleIndex anaChnlScaleIndex)
        {
            if (!_ProcDbiCoefficientsRecordTables.ContainsKey(sendType))
            {
                PublicFunc.WriteLog("[Send2ProcBoardByDefine]_ProcDbiCoefficientsRecordTables no define!!!");
                return;
            }

            foreach (DBI_CoefTableSendItem sendItem in sendItems)
            {
                DBI_CoefTableSendItem? defineItem = GetProcDefineItem(sendItem, sendType);
                if (defineItem == null)
                {
                    PublicFunc.WriteLog("[Send2ProcBoardByDefine]AcqBdCoefTablesSendDefine no define!!!");
                    continue;
                }

                Int32[]? dataArray = Misc.ReadCaliCoefDataFronmFile(defineItem.DataFileName);
                if (dataArray == null)
                {
                    PublicFunc.WriteLog($"[Send2ProcBoardByDefine]{defineItem.DataFileName} not exit!!!");
                    continue;
                }

                int dataCount = dataArray.Length;

                if (CheckCrcEnable)
                {
                    int crcCode = Misc.GenerateCRCCode(dataArray);
                    string HistoryKey = $"ProcBd_{sendType}";
                    if (SendCoefficientsTablesHistory.ContainsKey(HistoryKey))
                    {
                        if (SendCoefficientsTablesHistory[HistoryKey] == crcCode)
                        {
                            PublicFunc.WriteLog($"[Send2ProcBoardByDefine]CRC({crcCode}) is same!!!");
                            continue;
                        }
                        SendCoefficientsTablesHistory.Remove(HistoryKey);
                    }
                    SendCoefficientsTablesHistory.Add(HistoryKey, crcCode);
                }

                //DownloadBlockDataMode downMode = DbiCoefficientsTables.Default[sendType];
                //if (downMode == DownloadBlockDataMode.Register)
                _ProcDbiCoefficientsRecordTables[sendType].SendByRegister?.Invoke(_ProcDbiCoefficientsRecordTables[sendType].TypeCodeDefine, dataArray, dataCount, sendItem.ChannelID);
                //else
                //{
                //    Byte[] dmaByes = GenerateDmaDataByContinueAddressAndPerData(dataArray, dataCount, _ProcDbiCoefficientsRecordTables[sendType].AddressBitCount);
                //    _ProcDbiCoefficientsRecordTables[sendType].SendByDma?.Invoke(_ProcDbiCoefficientsRecordTables[sendType].TypeCodeDefine, dmaByes);
                //}
            }
        }
        #endregion

        #region AmpFreqCoefficients 改为随档位变化
        private static Dictionary<ChannelId, int/*crcCode*/> AmpFreqCoefficientsSentHistory = new()
        {
            [ChannelId.C1] = -1,
            [ChannelId.C2] = -1,
            [ChannelId.C3] = -1,
            [ChannelId.C4] = -1,
        };//幅频特性系数发送历史
        private static void SendAmpFreqCoefficients()
        {
            if (Hd.CurrProduct!.Acquirer_AnalogChannel!.ChannelPerScaleAmpFreqCoefficientsDefine == null)
                return;
            int channlCount = Hd.CurrProductType == ProductType.B24_AI20G ? 2 : 4;
            for (int channelIndex = 0; channelIndex < channlCount; channelIndex++)
            {
                if (Hd.UIMessage!.Analog![channelIndex].Active)
                {
                    string bandMode = (Hd.CurrProduct.Acquirer_AnalogChannel.AcquingParameters.CurrChBWModeAndActiveState & 0x100) != 0x100 ? "Full" : "Other";
                    string key = $"{(ChannelId)channelIndex}_{((int)Hd.UIMessage!.Analog![channelIndex].Scale).ToString()}_{bandMode}";
                    if (Hd.CurrProduct!.Acquirer_AnalogChannel!.ChannelPerScaleAmpFreqCoefficientsDefine.ContainsKey(key))
                    {
                        var info = Hd.CurrProduct!.Acquirer_AnalogChannel!.ChannelPerScaleAmpFreqCoefficientsDefine[key];
                        if (!info.bOk)
                            continue;

                        if (AmpFreqCoefficientsSentHistory[(ChannelId)channelIndex] != info.CRCCode)
                        {
                            int[]? dataArray = Misc.ReadCaliCoefDataFronmFile(info.FileName);

                            if (Hd.CurrProduct!.HardwareConfig!.DownloadBlockDataMode == DownloadBlockDataMode.Register && dataArray != null)
                            {
                           //     HdCtrl_Coefficient.SendProcTiadcByRegisterProcTi(dataArray, dataArray.Length, (ChannelId)channelIndex);
                                (UInt32 TypeCode, UInt32 wen, UInt32 addr, UInt32 lowData, UInt32 HighData, UInt32 DspReset) registers = (1, (UInt32)ProcBdReg.W.DBI_ProFactorWen,
                                                (UInt32)ProcBdReg.W.DBI_ProFactorWa, (UInt32)ProcBdReg.W.DBI_ProFactorWdLow, (UInt32)ProcBdReg.W.DBI_ProFactorWdHigh, 0);
                                SendProcBoardCoefficientsByRegister(registers, registers.TypeCode, channelIndex, dataArray, dataArray.Length);//2 见上面的注释
                            }
                            //else
                            //{
                            //    //由于DBI16G没有DMA模式，如果存在，dataType的定义可能与DBI20G不同
                            //    SendProcBoardCoefficientsByDMA(DbiCoefficientsTablesType.AmpFreqCoefficients, (uint)Math.Pow(2, (double)channelIndex), dataArray, dataArray.Length);//1 + (uint)channelIndex * 2  LYL修改
                            //}
                            AmpFreqCoefficientsSentHistory[(ChannelId)channelIndex] = info.CRCCode;
                        }
                    }
                }
            }
        }
        #endregion

        internal static void SendCoefficientsTables()
        {
            int totalGroupCount = 0;
            AnaChnlScaleIndex scale = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![(int)ChannelId.C1].ScaleIndex;
            foreach (var dataChanged in CaliDataManager.DataChangedDbiCoefficientsTablesType_Running)
            {
                switch (dataChanged.Key)
                {
                    case DbiCoefficientsTablesType.InterpolationCoefficients_2fold
                        or DbiCoefficientsTablesType.Level1_InterpolationCoefficients
                        or DbiCoefficientsTablesType.Level1_AntiImageCoefficients
                   
                        or DbiCoefficientsTablesType.Level2_InterpolationCoefficients
                        or DbiCoefficientsTablesType.Level2_AntiImageCoefficients
                        or DbiCoefficientsTablesType.Level2_LocalOscillatorCoefficients
                        or DbiCoefficientsTablesType.InterpolationCoefficients
                        or DbiCoefficientsTablesType.AntiImageCoefficients
                        or DbiCoefficientsTablesType.OverlapPhaseFreqDelayCoefficients
                        or DbiCoefficientsTablesType.FractionaryDelayCoefficients
                        or DbiCoefficientsTablesType.TiAdc:
                        Send2AcqBoardByDefineItem(dataChanged.Key, dataChanged.Value);
                        totalGroupCount += dataChanged.Value.Count;
                        break;
                    case DbiCoefficientsTablesType.LocalOscillatorCoefficients://
                        Send2AcqBoardByDefineItem(dataChanged.Key, dataChanged.Value.Where(o => o.ChnlScaleIndex == scale).ToList());
                        totalGroupCount += dataChanged.Value.Count;
                        break;

                    case DbiCoefficientsTablesType.PhaseFreqCoefficients://
                        //Send2ProcBoardByDefine(dataChanged.Key, dataChanged.Value);
                        Send2ProcBoardByDefine(dataChanged.Key, dataChanged.Value.Where(o => o.ChnlScaleIndex == scale).ToList());
                        totalGroupCount += dataChanged.Value.Count;
                        break;
                    case DbiCoefficientsTablesType.MultiRadioInterpolationCoefficients:
                        Send2ProcBoardByDefine(dataChanged.Key, dataChanged.Value);
                        totalGroupCount += dataChanged.Value.Count;
                        break;
                    case DbiCoefficientsTablesType.AmpFreqCoefficients:
                        Send2ProcBoardByDefine(dataChanged.Key, dataChanged.Value.Where(o => o.ChnlScaleIndex == scale).ToList());         //HTF    1031   DBI_DEBUG
                        totalGroupCount += dataChanged.Value.Count;
                        //SendAmpFreqCoefficients();
                        break;
                }
            }
        }

        internal static void SendCoefficientsTablesByAnalogScale(ChannelId channelIndex)
        {
            AnaChnlScaleIndex scale = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![(int)channelIndex].ScaleIndex;
            var dic = Acquirer_AnalogChanel_DBI13G.AcqBoardCoefficientsTablesSendDefine;
            if (dic.ContainsKey(DbiCoefficientsTablesType.LocalOscillatorCoefficients))
                Send2AcqBoardByDefineItem(DbiCoefficientsTablesType.LocalOscillatorCoefficients, dic[DbiCoefficientsTablesType.LocalOscillatorCoefficients].Where(o => o.ChnlScaleIndex == scale).ToList()) ;
            if (dic.ContainsKey(DbiCoefficientsTablesType.PhaseFreqCoefficients))
                Send2ProcBoardByDefine(DbiCoefficientsTablesType.PhaseFreqCoefficients, dic[DbiCoefficientsTablesType.PhaseFreqCoefficients].Where(o => o.ChnlScaleIndex == scale).ToList());
            if (dic.ContainsKey(DbiCoefficientsTablesType.AmpFreqCoefficients))
                Send2ProcBoardByDefine(DbiCoefficientsTablesType.AmpFreqCoefficients, dic[DbiCoefficientsTablesType.AmpFreqCoefficients].Where(o => o.ChnlScaleIndex == scale).ToList());
       
        }
    }

    internal record AcqDbiCoefficientsRecord(UInt32 TypeCodeDefine, Int32 AddressBitCount, Action<UInt32, AcqBdNo, Int32[], Int32>? SendByRegister, Func<UInt32, AcqBdNo, Byte[], Boolean>? SendByDma);

    internal record ProcDbiCoefficientsRecord(UInt32 TypeCodeDefine, Int32 AddressBitCount, Action<UInt32, Int32[], Int32, ChannelId>? SendByRegister, Func<UInt32, Byte[], Boolean>? SendByDma);
}
#endif //DBI
