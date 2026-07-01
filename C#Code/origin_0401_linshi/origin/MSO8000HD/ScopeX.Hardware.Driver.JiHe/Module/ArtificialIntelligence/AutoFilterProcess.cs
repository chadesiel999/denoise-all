using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.Hardware.Driver.ReconfigDBIProcess;

namespace ScopeX.Hardware.Driver
{
    internal class AutoFilterProcess
    {
        internal AutoFilterProcess()
        { 
            
        }

        private Dictionary<String, List<Double>> _AllNoiseTable = new();

        private const String _NoiseFilePath = "Noise_File";

        internal void Init()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write5, 0x01);//rst
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write5, 0x00);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, 0x00);//update
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, 0x00);//en
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write4, 0x02);//mod

            if (!Directory.Exists(_NoiseFilePath))
            {
                Directory.CreateDirectory(_NoiseFilePath);
            }

            String[] noisefiles = Directory.GetFiles(_NoiseFilePath, "*.txt");
            foreach (String filename in noisefiles)
            {
                if (PublicFunc.LoadDataFormFile(filename, out List<Double> data))
                {
                    _AllNoiseTable[filename] = data;
                }
            }
        }

        internal void PropertyChanged()
        {
            if (!Hd.CurrDebugVarints.bEnable_AutoFilter_Fpga)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, 0x00);//update
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, 0x00);//en
                return;
            }
            if (Hd.UIMessage?.AiTable == null)
                return;

            ChannelId chnlid = ChannelId.C1;
            {
                var recfgdbi = Hd.UIMessage.AiTable[chnlid].RecfgDbi;
                if (recfgdbi == null)
                    return;

                switch (recfgdbi.AutoFilterMode)
                {
                    case AutoFilterMode.Closed:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, 0x00);//update
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, 0x00);//en
                        break;
                    case AutoFilterMode.Open:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, 0x00);//update
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, 0x01);//en
                        break;
                    case AutoFilterMode.CalcNoise:
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write5, 0x01);//rst
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write5, 0x00);
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write6, 1020);//fifo
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, 0x01);//update
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, 0x00);//en
                        //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write4, 0x02);//mod
                        break;
                }
            }
        }//????

        private Double _Noise = Double.NaN;
        private List<Double> _NoiseTable = new List<Double>();

        internal Boolean TryExcuteBySoftware(List<UInt16> source, Double perDataByfs_AtDMA, out List<UInt16> result)
        {
            result = new List<UInt16>();
            if (Hd.CurrDebugVarints.bEnable_AutoFilter_Fpga)
            {
                return false;
            }

            if (Hd.UIMessage?.AiTable == null)
                return false;

            ChannelId chnlid = ChannelId.C1;
            {
                var recfgdbi = Hd.UIMessage.AiTable[chnlid].RecfgDbi;
                if (recfgdbi == null)
                    return false;

                switch (recfgdbi.AutoFilterMode)
                {
                    case AutoFilterMode.Closed:
                        _NoiseTable.Clear();
                        return false;
                    case AutoFilterMode.Open:
                        Double[] tmp = ExcuteBySoftware(source, perDataByfs_AtDMA, 1024);
                        if (tmp.Length > 0)
                        {
                            result.AddRange(tmp.Select(o => (UInt16)o));
                            return true;
                        }
                        break;
                    case AutoFilterMode.CalcNoise:
                        CalcNoise(source, perDataByfs_AtDMA);
                        break;
                }
            }

            return false;
        }

        private Double _LastPerDataByfs_AtDMA = Double.NaN;

        private void CalcNoise(List<UInt16> source, Double perDataByfs_AtDMA)
        {
            if (!_LastPerDataByfs_AtDMA.Equals(perDataByfs_AtDMA))
            {
                _LastPerDataByfs_AtDMA = perDataByfs_AtDMA;
                _NoiseTable.Clear();
            }

            var ans = Matlab.Default.SetWorkFolder("C:\\Matlab\\");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData(source.Select(o => (Double)o).ToArray(), "data");

            List<String> codelist = new List<String>()
            {
                "source = cell2mat(data);",
                "resultData = calcNoise(source, 10240);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));

            ans = Matlab.Default.TryGetData("resultData", out Object? data);
            if (data != null)
            {
                if (data is Double)
                {
                    _Noise = (Double)data;
                }
                else if (data is Double[])
                {
                    _NoiseTable.Clear();
                    _NoiseTable.AddRange((Double[])data);
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Double[] result = new Double[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = tmp[rowid, columnid];
                            if (_NoiseTable.Count <= dotid)
                            {
                                _NoiseTable.Add(result[dotid]);
                            }
                            else
                            {
                                _NoiseTable[dotid] = result[dotid];
                            }
                            dotid++;
                        }
                    }
                }

                String filename = $"{_NoiseFilePath}\\{_NoiseTable.Count}_{perDataByfs_AtDMA.ToString("0.00")}fs_Noise.txt";
                _AllNoiseTable[filename] = _NoiseTable.ToList();
                PublicFunc.SaveDataToFile(filename, _NoiseTable);
            }

        }

        private Double[] ExcuteBySoftware(List<UInt16> source, Double perDataByfs_AtDMA, Int32 noiseCnt)
        {
            var ans = Matlab.Default.SetWorkFolder("C:\\Matlab\\");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData(source.Select(o => (Double)o).ToArray(), "data");
            String filename = $"{_NoiseFilePath}\\{10240}_{perDataByfs_AtDMA.ToString("0.00")}fs_Noise.txt";
            if (_AllNoiseTable.ContainsKey(filename))
            {
                ans = Matlab.Default.PutData(_AllNoiseTable[filename].ToArray(), "noise");
                List<String> codelist = new List<String>()
                {
                    "noise = cell2mat(noise);",
                    "source = cell2mat(data);",
                    $"resultData = AutoFilterEx(source, 10240, noise);",
                };

                ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));
            }
            

            ans = Matlab.Default.TryGetData("resultData", out Object? data);
            if (data is not null)
            {
                if (data is Double[])
                {
                    return (Double[])data;
                }

                if (data is Double[,])
                {
                    Double[,] tmp = (Double[,])data;
                    Double[] result = new Double[tmp.GetLength(0) * tmp.GetLength(1)];
                    Int32 dotid = 0;
                    for (Int32 rowid = 0; rowid < tmp.GetLength(0); rowid++)
                    {
                        for (Int32 columnid = 0; columnid < tmp.GetLength(1); columnid++)
                        {
                            result[dotid] = tmp[rowid, columnid];
                            dotid++;
                        }
                    }
                    //Array.Copy(tmp, result, result.Length * sizeof(Double));
                    return result;
                }
            }
            return new Double[0];
        }

        #region 迭代平均滤波
        internal const Int32 SubbandCnt = 4;
        private UInt64[] _CurLoFreq = new UInt64[SubbandCnt];
        private UInt64[,] _CurFc = new UInt64[SubbandCnt, 2];

        private SigFreqRange[] _CurFreq = new SigFreqRange[SubbandCnt]
     {
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0)
     };
        private SigFreqRange[] _LastFreq = new SigFreqRange[SubbandCnt]
        {
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0)
        };

        private void ReadSubbandInfo()//????
        {
            SubbandInfo[] subbanddefines = new SubbandInfo[]
            {
                new(AcqBdNo.B11, RightFreqBand),
                new(AcqBdNo.B12, LeftFreqBand ),
                new(AcqBdNo.B7,  RightFreqBand),
                new(AcqBdNo.B10, RightFreqBand),
            };

            for (Int32 subid = 0; subid < subbanddefines.Length; subid++)
            {
                UInt32 validreg = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_out_valid, subbanddefines[subid].AcqBd);

                if (validreg == 1)
                {
                    UInt32 max = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_max, subbanddefines[subid].AcqBd);
                    UInt32 min = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_min, subbanddefines[subid].AcqBd);

                    var calcfunc = subbanddefines[subid].CalcFreqFunc;
                    _CurFreq[subid] = new SigFreqRange(calcfunc(_CurLoFreq[subid], max), calcfunc(_CurLoFreq[subid], min));
                }
            }
        }

        private Double LeftFreqBand(Double localFreq, UInt32 fftid)
        {
            return localFreq - fftid * 20 * 1e9 / 4096;
        }

        private Double RightFreqBand(Double localFreq, UInt32 fftid)
        {
            return localFreq + fftid * 20 * 1e9 / 4096;
        }

        private Boolean CheckSigChanged()
        {
            // 补充信号是否变化，直接用频点（Max，Nin）
            for (Int32 subid = 0; subid < SubbandCnt; subid++)
            {
                if (Math.Abs(_CurFreq[subid].FreqMin - _LastFreq[subid].FreqMin) > 50e6 ||
                    Math.Abs(_CurFreq[subid].FreqMax - _LastFreq[subid].FreqMax) > 50e6)
                {
                    for (Int32 i = 0; i < _CurFreq.Length && i < _LastFreq.Length; i++)
                    {
                        _LastFreq[i] = _CurFreq[i];
                    }
                    return true;
                }
            }

            return false;
        }

        //internal static void ConfigAverageCnt(Int32 averageCnt)
        //{
        //    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, AcquingParameters.bIsLongStorageMode ? 1 : 0U);
        //    if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
        //    {
        //        HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
        //    }
        //    else
        //    {
        //        Double timescale_AVE_1 = (Hd.UIMessage?.Timebase?.TmbScale ?? 0.02);//us单位
        //        if (timescale_AVE_1 > 0.02)//平均功能
        //        {
        //            HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
        //        }
        //        else
        //        {
        //            HdIO.WriteReg(ProcBdReg.W.Average_Enable, Hd.CurrDebugVarints.bEnable_ProcBd_Average ? 1u : 0);
        //        }

        //        HdIO.WriteReg(ProcBdReg.W.Average_Number, (UInt32)averageCnt);
        //        HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
        //        HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
        //        HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
        //    }
        //}

        internal Boolean UiChanged = false;
        internal Boolean SigChanged = false;

        private UInt32 _AvgCnt = 0;
        private const UInt32 _MaxAvgCnt = 4;


        internal void IterAvgFilter()//????
        {
            //if (Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.IterFilterEnable ?? false)
            //{
            //    ReadSubbandInfo();
            //    if (UiChanged || CheckSigChanged())
            //    {
            //        HdIO.WriteReg(ProcBdReg.W.Average_Enable, 1);
            //        //HdIO.WriteReg(ProcBdReg.W.Average_Number, 0);
            //        _AvgCnt = 0;
            //        UiChanged = false;
            //    }
            //    else
            //    {
            //        _AvgCnt++;
            //        if (_AvgCnt == 10)
            //        {
            //            HdIO.WriteReg(ProcBdReg.W.Average_Enable, 1);
            //        }

            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0, (UInt32)400);
            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0Cnt, (UInt32)5000);
            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1, (UInt32)400);
            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1Cnt, (UInt32)5000);
            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2, (UInt32)(4 << 12) + 4096);
            //        HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2Cnt, (UInt32)1000);
            //        HdIO.WriteReg(ProcBdReg.W.Average_Number, 3);
            //        HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
            //        HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
            //        HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
            //    }

            //}
            //else
            //{
            //    HdIO.WriteReg(ProcBdReg.W.Average_Enable, Hd.CurrDebugVarints.bEnable_ProcBd_Average ? 1u : 0);

            //    _AvgCnt = 0;
            //    //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 1);
            //    //     HdIO.WriteReg(ProcBdReg.W.Average_Enable, 1);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0, (UInt32)400);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0Cnt, (UInt32)5000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1, (UInt32)400);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1Cnt, (UInt32)5000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2, (UInt32)(4 << 12) + 4096);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2Cnt, (UInt32)1000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_Number, 3);
            //    HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
            //    HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
            //}



        }

        internal async void IterativeAverageFilter()//????
        {
            bool userModifiedTimebase = false;
            UInt32 maxAverageCnt = 4;
            UInt32 curAverageCnt = 0;

            ReadSubbandInfo();

            //HdMessage backHdMessage = Hd.UIMessage! with { };
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, AcquingParameters.bIsLongStorageMode ? 1 : 0U);

            if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            {
                //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
            }
            else
            {
                //Double timescale_AVE_1 = (Hd.UIMessage?.Timebase?.TmbScale ?? 0.02);//us单位

                if (userModifiedTimebase || CheckSigChanged())//将平均功能关闭
                {
                    //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
                    curAverageCnt = 0;
                }
                while (CheckSigChanged())
                {
                    await Task.Delay(500);
                }
                if (!userModifiedTimebase && !CheckSigChanged())
                {
                    //HdIO.WriteReg(ProcBdReg.W.Average_Enable, Hd.CurrDebugVarints.bEnable_ProcBd_Average ? 1u : 0); //开启
                    while (curAverageCnt < maxAverageCnt) //逐步累加averageCnt
                    {
                        curAverageCnt++;
                        //HdIO.WriteReg(ProcBdReg.W.Average_Number, maxAverageCnt);
                        //HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
                        //HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
                        //HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
                    }
                }

                //HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
                //HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
                //HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
            }

        }
        #endregion

    }
}
