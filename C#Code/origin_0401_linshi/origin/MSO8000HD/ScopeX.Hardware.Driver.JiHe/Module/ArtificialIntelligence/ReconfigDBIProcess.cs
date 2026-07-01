using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace ScopeX.Hardware.Driver
{
    internal class ReconfigDBIProcess
    {
        internal ReconfigDBIProcess()
        {

        }

        internal void Run()
        {
            ConfigSubbandState();
            ConfigDBIParams();
        }

        internal String TryGetData(Object param, out Object? data)
        {
            data = null;
            if (param is not String)
                return "";
            if (String.Equals((String)param, "AnaChnlBitWidthDefine"))
            {
                data = Hd.CurrProduct?.AnaChnlBitWidthDefine ?? new Int32[1] { 12 };
                return String.Empty;
            }

            if (String.Equals((String)param, "BitWidth"))
            {
                Int32 enableId = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key).First() ?? 0;
                if (enableId != 0)
                {
                    if (Math.Abs(_CurSigFreq[enableId].FreqMax - _CurSigFreq[enableId].FreqMin) < 1.6e9)
                    {
                        data = 16;
                    }
                    else if (Math.Abs(_CurSigFreq[enableId].FreqMax - _CurSigFreq[enableId].FreqMin) < 3.6e9)
                    {
                        data = 15;
                    }
                    else
                    {
                        data = 14;
                    }
                }
                else
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0b0000);     //信号位于第1子带
                    if (_CurSigFreq[enableId].FreqMax < 1.2e9)
                    {
                        data = 16;
                    }
                    else if (_CurSigFreq[enableId].FreqMax < 3.2e9)
                    {
                        data = 15;
                    }
                    else
                    {
                        data = 14;
                    }
                }
                return String.Empty;
            }

            if (String.Equals((String)param, "SetFinish"))
            {
                data = SetFinish;
                return String.Empty;
            }

            if (String.Equals((String)param, "EnableIds"))
            {
                data = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key).ToArray() ?? new int[] { 0,};
                return String.Empty;
            }

            if (String.Equals((String)param, "FrequencyMeter"))
            {
                Int32 enableId = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key).LastOrDefault() ?? 0;
                if (_CurSigFreq[enableId].FreqMax > 90E6)
                {
                    data = _CurSigFreq[enableId].FreqMax;
                }
                else 
                {
                    data = ReadFrequencyMeterData();
                }
                return String.Empty;
            }

            if (String.Equals((String)param, "FrequencyMin"))
            {
                Int32 enableId = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key).FirstOrDefault() ?? 0;
                if (_CurSigFreq[enableId].FreqMax > 90E6)
                {
                    data = _CurSigFreq[enableId].FreqMin;
                }
                return String.Empty;
            }
            return "";
        }

        internal const Int32 SubbandCnt = 4;

        private readonly Double[] _DefaultLoFreq = new Double[]
        {
            0,
            10e9, 
            15e9,
            22.5e9,
        };
        
        private readonly Double[] _DefaultAnalogLoFreq = new Double[]
        {
            0,
            10e9,
            15e9,
            22.5e9,
        };
        private readonly Double[] _DefaultLoFreq_Level1 = new Double[]
        {
            0,
            1e9,
            1e9,
            1e9,
        };
        private readonly Double[] _DefaultLoFreq_Level2 = new Double[]
        {
            0,
            3e9,
            3e9,
            3e9,
        };

        internal UInt64[] CurLoFreq = new UInt64[SubbandCnt];
        internal UInt64[] CurAnalogLoFreq = new UInt64[SubbandCnt];
        internal UInt64[] CurLoFreq_Level1 = new UInt64[SubbandCnt];
        internal UInt64[] CurLoFreq_Level2 = new UInt64[SubbandCnt];

        internal record SigFreqRange(Double FreqMax, Double FreqMin);
        private SigFreqRange[] _CurSigFreq = new SigFreqRange[SubbandCnt]
        {
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0)
        };
        private SigFreqRange[] _LastSigFreq = new SigFreqRange[SubbandCnt]
        {
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0),
            new SigFreqRange(0,0)
        };
        private List<Double>[] _FreqMaxList = new List<Double>[]
        {
            new List<Double>(),
            new List<Double>(),
            new List<Double>(),
            new List<Double>()
        };
        private List<Double>[] _FreqMinList = new List<Double>[]
        {
            new List<Double>(),
            new List<Double>(),
            new List<Double>(),
            new List<Double>()
        };

        private Boolean _InitFlag = false;
        private Boolean _FullDBI = true;

        internal void InitParams()
        {
            for (Int32 subid = 0; subid < CurLoFreq.Length && subid < _DefaultLoFreq.Length; subid++)
            {
                CurLoFreq[subid] = (UInt64)_DefaultLoFreq[subid];
                CurLoFreq_Level1[subid] = (UInt64)_DefaultLoFreq_Level1[subid];
                CurLoFreq_Level2[subid] = (UInt64)_DefaultLoFreq_Level2[subid];
                CurAnalogLoFreq[subid] = (UInt64)_DefaultAnalogLoFreq[subid];
            }
            for (Int32 subid = 0; subid < _LastSigFreq.Length; subid++)
            {
                _LastSigFreq[subid] = new(0, 0);
            }
            for (Int32 subid = 0; subid < _CurSigFreq.Length; subid++)
            {
               // _CurSigFreq[subid] = new(0, 0);
            }
            _LastSubbandInterplotCoe.Clear();
            SubbandValid = 0xf;
            BitWidth = 12;
            LowFreqMode = false;
            _LastTs_mode = 2;
            Trace.WriteLine("[InitParams]12Bit");
            _InitFlag = true;
            SetFinish = false;
        }

        public SigFreqRange[] GetSignalFreq() 
        {
            return _CurSigFreq;
        }

        internal void ConfigDefaultDBI()
        {
            //ConfigSubbandBandwidth(new());

            //ConfigLocalFreq(new(), new());

            //ConfigAntImageFreq(new());
            var aimsg = Hd.UIMessage?.AiTable;
            var chnlid = ChannelId.C1;
            var recfgDbimsg = aimsg[chnlid].RecfgDbi;
            ConfigSubbandBandwidth(recfgDbimsg.BandFreqLimitByHz);

            ConfigLocalFreq(recfgDbimsg.LocalFreqByHz, recfgDbimsg.LocalFreqByHz,false);

            ConfigAntImageFreq(recfgDbimsg.AntImageFreqByHz);

            Dictionary<Int32, Int32> subbandinfo = new()
            {
                [0] = 0,
                [1] = 1,
                [2] = 2,
                [3] = 3,
            };
            ConfigInterplotCoe(subbandinfo);
            HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0);//hcj_c
            LowFreqMode = false;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(0x404F8, 0);
            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
            _FullDBI = true;

        }

        private Int32 _NarrowFreqSigCnt = 0;
        private Int32 _MoreNarrowFreqSigCnt = 0;
        private Int32 _LowFreqSigCnt = 0;
        private Int32 _MoreLowFreqSigCnt = 0;
        private Int32 _frequency_low_flag = 0;
        private Int32 _Ts_mode_low = 0;

        internal void ConfigCurDBI()
        {
            Dictionary<Int32, UInt64> localfreq = new();
            Dictionary<Int32, UInt64> analoglocalfreq = new();
            Dictionary<Int32, UInt64> localfreq_level1 = new();
            Dictionary<Int32, UInt64> localfreq_level2 = new();
            Dictionary<Int32, AntImageFreq> antimagfreq = new();
            Dictionary<Int32, AntImageFreq> antimagfreq_level1 = new();
            Dictionary<Int32, AntImageFreq> antimagfreq_level2 = new();
            Dictionary<Int32, UInt64> bandwidth = new();
            Dictionary<Int32, Int32> interplotsubbandinfo = new();

            _FullDBI = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;

            if (_CurSigFreq[0].FreqMax<=400_000_000 && _CurSigFreq[0].FreqMax>0) 
            {
                _frequency_low_flag = 0;        //1
            }
            else if(Math.Abs(_CurSigFreq[1].FreqMax - _CurSigFreq[1].FreqMin) <= 400_000_000 && Math.Abs(_CurSigFreq[1].FreqMax - _CurSigFreq[1].FreqMin)>0)
            {
                _frequency_low_flag = 0;        //1
            }
            else if(Math.Abs(_CurSigFreq[2].FreqMax - _CurSigFreq[2].FreqMin) <= 400_000_000 && Math.Abs(_CurSigFreq[2].FreqMax - _CurSigFreq[2].FreqMin) > 0)
            {
                _frequency_low_flag = 0;        //1
            }
            else if(Math.Abs(_CurSigFreq[3].FreqMax - _CurSigFreq[3].FreqMin) <= 400_000_000 && Math.Abs(_CurSigFreq[3].FreqMax - _CurSigFreq[3].FreqMin) > 0)
            {
                _frequency_low_flag = 0;        //1
            }
            else
            {
                _frequency_low_flag = 0;        //0
            }

            if(_frequency_low_flag == 1)
            {
                _Ts_mode_low = 1;
                LowFreqMode = true;
                #region 子带一
                if (_CurSigFreq[0].FreqMin == 0 && _CurSigFreq[0].FreqMax == 0)
                {

                }
                else
                {
                    Int64 analoglocal =0;
                    analoglocalfreq[0] = (UInt64)Math.Max((Int64)0, analoglocal);
                    CurAnalogLoFreq[0] = (UInt64)analoglocal;
                    //第一级配置
                    Int64 local_level1 = 0;
                    localfreq_level1[0] = (UInt64)Math.Max((Int64)0, local_level1);
                    CurLoFreq_Level1[0] = (UInt64)local_level1;//实际中生成,但是FPGA模块使能关闭
                    antimagfreq_level1[0] = new AntImageFreq((UInt64)21_000_000, 500_000_000);//实际中生成,但是FPGA模块使能关闭
                    //第二级配置
                    Int64 local_level2 = 0;
                    localfreq_level2[0] = (UInt64)Math.Max((Int64)0, local_level2);
                    CurLoFreq_Level2[0] = (UInt64)local_level2;//实际中生成,但是FPGA模块使能关闭
                    antimagfreq_level2[0] = new AntImageFreq((UInt64)3_750_000_000, 4_350_000_000);//实际中生成,但是FPGA模块使能关闭
                    //第三级配置
                    Int64 local = 0;
                    localfreq[0] = (UInt64)Math.Max((Int64)0, local);
                    CurLoFreq[0] = (UInt64)local;//实际中生成,但是FPGA模块使能关闭
                    antimagfreq[0] = new AntImageFreq((UInt64)2_500_000_000, 6_500_000_000);//实际中生成,但是FPGA模块使能关闭
                    bandwidth[0] = 6_000_000_000;
                }
                #endregion
                #region 子带二
                if (_CurSigFreq[1].FreqMin == 0 && _CurSigFreq[1].FreqMax == 0)
                {

                }
                else
                {
                    Int64 analoglocal = (Int64)Math.Round(_CurSigFreq[1].FreqMax / 10_000_000) * 10_000_000 + 60_000_000;//10M to 20M
                    analoglocalfreq[1] = (UInt64)Math.Max((Int64)0, analoglocal);
                    CurAnalogLoFreq[1] = (UInt64)analoglocal;
                    //获取本振的十M级的数值
                    Int64 value_10M = analoglocal - (Int64)Math.Floor(analoglocal / 100_000_000.0) * 100_000_000;
                    Int64 local_level1;
                    //第一级配置
                    if (Math.Abs(_CurSigFreq[1].FreqMax - _CurSigFreq[1].FreqMin) + 20_000_000>=250_000_000)
                         local_level1 = (Int64)Math.Round(((Math.Abs(_CurSigFreq[1].FreqMax - _CurSigFreq[1].FreqMin)) + 60_000_000 - 250_000_000) / 100_000_000.0) * 100_000_000 + value_10M + 1_000_000_000;
                    else
                         local_level1 = value_10M + 1_000_000_000;

                    localfreq_level1[1] = (UInt64)Math.Max((Int64)0, local_level1);
                    CurLoFreq_Level1[1] = (UInt64)local_level1;
                    antimagfreq_level1[1] = new AntImageFreq((UInt64)750_000_000, (UInt64)local_level1 -60_000_000);//10M to 20M
                    //第二级配置
                    Int64 local_level2 = 3_000_000_000;
                    localfreq_level2[1] = (UInt64)Math.Max((Int64)0, local_level2);
                    CurLoFreq_Level2[1] = (UInt64)local_level2;
                    antimagfreq_level2[1] = new AntImageFreq((UInt64)3_750_000_000, (UInt64)local_level1 - 60_000_000+ (UInt64)local_level2);//10M to 20M
                    //第三级配置
                    Int64 local = analoglocal- local_level1- local_level2;
                    localfreq[1] = (UInt64)Math.Max((Int64)0, local);
                    CurLoFreq[1] = (UInt64)local;
                    //antimagfreq[1] = new AntImageFreq((UInt64)6_000_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2+ (UInt64)local);//10M to 20M
                    antimagfreq[1] = new AntImageFreq((UInt64)local_level1 - 60_000_000 + (UInt64)local_level2 + (UInt64)local - (UInt64)500_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2+ (UInt64)local);//10M to 20M

                    bandwidth[1] = 6_000_000_000;
                }
                #endregion
                #region 子带三
                if (_CurSigFreq[2].FreqMin == 0 && _CurSigFreq[2].FreqMax == 0)
                {

                }
                else
                {

                    Int64 analoglocal = (Int64)Math.Round(_CurSigFreq[2].FreqMax / 10_000_000) * 10_000_000 + 60_000_000;//10M to 20M
                    analoglocalfreq[2] = (UInt64)Math.Max((Int64)0, analoglocal);
                    CurAnalogLoFreq[2] = (UInt64)analoglocal;
                    //获取本振的十M级的数值
                    Int64 value_10M = analoglocal - (Int64)Math.Floor(analoglocal / 100_000_000.0) * 100_000_000;
                    Int64 local_level1;
                    //第一级配置
                    if (Math.Abs(_CurSigFreq[2].FreqMax - _CurSigFreq[2].FreqMin) + 20_000_000 >= 250_000_000)
                        local_level1 = (Int64)Math.Round(((Math.Abs(_CurSigFreq[2].FreqMax - _CurSigFreq[2].FreqMin)) + 60_000_000 - 250_000_000) / 100_000_000.0) * 100_000_000 + value_10M + 1_000_000_000;
                    else
                        local_level1 = value_10M + 1_000_000_000;

                    localfreq_level1[2] = (UInt64)Math.Max((Int64)0, local_level1);
                    CurLoFreq_Level1[2] = (UInt64)local_level1;
                    antimagfreq_level1[2] = new AntImageFreq((UInt64)750_000_000, (UInt64)local_level1 - 60_000_000);//10M to 20M
                    //第二级配置
                    Int64 local_level2 = 3_000_000_000;
                    localfreq_level2[2] = (UInt64)Math.Max((Int64)0, local_level2);
                    CurLoFreq_Level2[2] = (UInt64)local_level2;
                    antimagfreq_level2[2] = new AntImageFreq((UInt64)3_750_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2);//10M to 20M
                    //第三级配置
                    Int64 local = analoglocal - local_level1 - local_level2;
                    localfreq[2] = (UInt64)Math.Max((Int64)0, local);
                    CurLoFreq[2] = (UInt64)local;
                    //antimagfreq[1] = new AntImageFreq((UInt64)6_000_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2+ (UInt64)local);//10M to 20M
                    antimagfreq[2] = new AntImageFreq((UInt64)local_level1 - 60_000_000 + (UInt64)local_level2 + (UInt64)local - (UInt64)500_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2 + (UInt64)local);//10M to 20M

                    bandwidth[2] = 6_000_000_000;


                }
                #endregion

                #region 子带四
                if (_CurSigFreq[3].FreqMin == 0 && _CurSigFreq[3].FreqMax == 0)
                {

                }
                else
                {
                    Int64 analoglocal = (Int64)Math.Round(_CurSigFreq[3].FreqMax / 10_000_000) * 10_000_000 + 60_000_000;//10M to 20M
                    analoglocalfreq[3] = (UInt64)Math.Max((Int64)0, analoglocal);
                    CurAnalogLoFreq[3] = (UInt64)analoglocal;
                    //获取本振的十M级的数值
                    Int64 value_10M = analoglocal - (Int64)Math.Floor(analoglocal / 100_000_000.0) * 100_000_000;
                    Int64 local_level1;
                    //第一级配置
                    if (Math.Abs(_CurSigFreq[3].FreqMax - _CurSigFreq[3].FreqMin) + 20_000_000 >= 250_000_000)
                        local_level1 = (Int64)Math.Round(((Math.Abs(_CurSigFreq[3].FreqMax - _CurSigFreq[3].FreqMin)) + 60_000_000 - 250_000_000) / 100_000_000.0) * 100_000_000 + value_10M + 1_000_000_000;
                    else
                        local_level1 = value_10M + 1_000_000_000;

                    localfreq_level1[3] = (UInt64)Math.Max((Int64)0, local_level1);
                    CurLoFreq_Level1[3] = (UInt64)local_level1;
                    antimagfreq_level1[3] = new AntImageFreq((UInt64)750_000_000, (UInt64)local_level1 - 60_000_000);//10M to 20M
                    //第二级配置
                    Int64 local_level2 = 3_000_000_000;
                    localfreq_level2[3] = (UInt64)Math.Max((Int64)0, local_level2);
                    CurLoFreq_Level2[3] = (UInt64)local_level2;
                    antimagfreq_level2[3] = new AntImageFreq((UInt64)3_750_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2);//10M to 20M
                    //第三级配置
                    Int64 local = analoglocal - local_level1 - local_level2;
                    localfreq[3] = (UInt64)Math.Max((Int64)0, local);
                    CurLoFreq[3] = (UInt64)local;
                    //antimagfreq[1] = new AntImageFreq((UInt64)6_000_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2+ (UInt64)local);//10M to 20M
                    antimagfreq[3] = new AntImageFreq((UInt64)local_level1 - 60_000_000 + (UInt64)local_level2 + (UInt64)local - (UInt64)500_000_000, (UInt64)local_level1 - 60_000_000 + (UInt64)local_level2 + (UInt64)local);//10M to 20M

                    bandwidth[3] = 6_000_000_000;

                }
                #endregion
                if (SubbandId != 0)
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0b1111);     //信号位于第234子带

                    if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) + 10_000_000 < 100_000_000)
                    {
                        BitWidth = 16;
                    }
                    else if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) + 10_000_000 < 200_000_000)
                    {
                        BitWidth = 15;
                    }
                    else 
                    {
                        BitWidth = 14;
                    }
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                        {
                            analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                            localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                            antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                            localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                            antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                            bandwidth[subbandid] = bandwidth[SubbandId];
                            localfreq[subbandid] = localfreq[SubbandId];
                            antimagfreq[subbandid] = antimagfreq[SubbandId];
                        }
                    }
                    Trace.WriteLine($"[ConfigCurDBI]Low_NarrowFreqSig,BitWidth++({BitWidth})");

                    //switch (BitWidth)
                    //{
                    //    case 12:
                    //        break;
                    //    case 13:
                    //        break;
                    //    case 14:
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin)+10_000_000 < 200_000_000)
                    //        {
                    //            _NarrowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _NarrowFreqSigCnt = 0;
                    //        }
                    //        if (_NarrowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]Low_NarrowFreqSig,BitWidth++({BitWidth})");
                    //            _NarrowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 15:
                    //        _NarrowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) + 10_000_000 < 100_000_000)
                    //        {
                    //            _MoreNarrowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _MoreNarrowFreqSigCnt = 0;
                    //        }
                    //        if (_MoreNarrowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]Low_MoreNarrowFreqSigCnt,BitWidth++({BitWidth})");
                    //            _MoreNarrowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 16:
                    //        _MoreNarrowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        break;
                    //}
                }
                else
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0b0000);     //信号位于第一子带
                    if (_CurSigFreq[SubbandId].FreqMax < 100_000_000)
                    {
                        BitWidth = 16;
                    }
                    else if (_CurSigFreq[SubbandId].FreqMax < 200_000_000)
                    {
                        BitWidth = 15;
                    }
                    else 
                    {
                        BitWidth = 14;
                    }
                    Trace.WriteLine($"[ConfigCurDBI]Low_LowFreqSig,BitWidth++({BitWidth})");
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                        {
                            analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                            localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                            antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                            localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                            antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                            bandwidth[subbandid] = bandwidth[SubbandId];
                            localfreq[subbandid] = localfreq[SubbandId];
                            antimagfreq[subbandid] = antimagfreq[SubbandId];
                        }
                    }
                    //    switch (BitWidth)
                    //    {
                    //        case 12:
                    //            break;
                    //        case 13:
                    //            break;
                    //        case 14:
                    //            for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //            {
                    //                if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //                {
                    //                    analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                    localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                    antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                    localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                    antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                    bandwidth[subbandid] = bandwidth[SubbandId];
                    //                    localfreq[subbandid] = localfreq[SubbandId];
                    //                    antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //                }
                    //            }
                    //            if (_CurSigFreq[SubbandId].FreqMax < 200_000_000)
                    //            {
                    //                _LowFreqSigCnt++;
                    //            }
                    //            else
                    //            {
                    //                _LowFreqSigCnt = 0;
                    //            }
                    //            if (_LowFreqSigCnt > 10)
                    //            {
                    //                BitWidth++;
                    //                Trace.WriteLine($"[ConfigCurDBI]Low_LowFreqSig,BitWidth++({BitWidth})");
                    //                _LowFreqSigCnt = 0;
                    //            }
                    //            break;
                    //        case 15:
                    //            _LowFreqSigCnt = 0;
                    //            for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //            {
                    //                if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //                {
                    //                    analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                    localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                    antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                    localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                    antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                    bandwidth[subbandid] = bandwidth[SubbandId];
                    //                    localfreq[subbandid] = localfreq[SubbandId];
                    //                    antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //                }
                    //            }
                    //            if (_CurSigFreq[SubbandId].FreqMax < 100_000_000)
                    //            {
                    //                //_MoreLowFreqSigCnt++;
                    //            }
                    //            else
                    //            {
                    //                _MoreLowFreqSigCnt = 0;
                    //            }
                    //            if (_MoreLowFreqSigCnt > 10)
                    //            {
                    //                BitWidth++;
                    //                Trace.WriteLine($"[ConfigCurDBI]Low_MoreLowFreqSigCnt,BitWidth++({BitWidth})");
                    //                _MoreLowFreqSigCnt = 0;
                    //            }
                    //            break;
                    //        case 16:
                    //            _MoreLowFreqSigCnt = 0;
                    //            for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //            {
                    //                if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //                {
                    //                    analoglocalfreq[subbandid] = analoglocalfreq[SubbandId];
                    //                    localfreq_level1[subbandid] = localfreq_level1[SubbandId];
                    //                    antimagfreq_level1[subbandid] = antimagfreq_level1[SubbandId];

                    //                    localfreq_level2[subbandid] = localfreq_level2[SubbandId];
                    //                    antimagfreq_level2[subbandid] = antimagfreq_level2[SubbandId];

                    //                    bandwidth[subbandid] = bandwidth[SubbandId];
                    //                    localfreq[subbandid] = localfreq[SubbandId];
                    //                    antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //                }
                    //            }
                    //            break;
                    //    }
                }

                for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                {
                    interplotsubbandinfo[subbandid] = 0;
                }
 
                ConfigSubbandBandwidth(bandwidth);
                ConfigLocalFreq(localfreq, analoglocalfreq,true);
                ConfigAntImageFreq(antimagfreq);
                ConfigInterplotCoe(interplotsubbandinfo);

                ConfigLocalFreq_Level1(localfreq_level1);
                ConfigLocalFreq_Level2(localfreq_level2);
                ConfigAntImageFreq_Level1(antimagfreq_level1);
                ConfigAntImageFreq_Level2(antimagfreq_level2);
                ConfigInterpolationCoefficients_Level1(_Ts_mode_low, SubbandId);
            }
            else
            {
                _Ts_mode_low = 0;
                LowFreqMode = false;
                #region 子带一
                if (_CurSigFreq[0].FreqMin == 0 && _CurSigFreq[0].FreqMax == 0)
                {

                }
                else
                {
                    //Int64 local = (Int64)Math.Round(_CurSigFreq[0].FreqMin / 100_000_000) * 100_000_000 - 500_000_000;
                    Int64 local = 0;
                    localfreq[0] = (UInt64)Math.Max((Int64)0, local);
                    CurAnalogLoFreq[0] = (UInt64)local;
                    CurLoFreq[0] = (UInt64)local;
                    antimagfreq[0] = new AntImageFreq((UInt64)local, 6_000_000_000);
                    bandwidth[0] = 6_000_000_000;
                }
                #endregion

                #region 子带二
                if (_CurSigFreq[1].FreqMin == 0 && _CurSigFreq[1].FreqMax == 0)
                {

                }
                else
                {
                    Int64 local = (Int64)Math.Round(_CurSigFreq[1].FreqMax / 100_000_000) * 100_000_000 + 500_000_000;//+500 000 000
                    localfreq[1] = (UInt64)Math.Max(0, local);
                    CurAnalogLoFreq[1] = (UInt64)local;
                    CurLoFreq[1] = (UInt64)local;
                    //antimagfreq[1] = new AntImageFreq(5_500_000_000, (UInt64)local- 500_000_000);
                    //antimagfreq[1] = new AntImageFreq((UInt64)Math.Round(_CurSigFreq[1].FreqMax), (UInt64)local- 500_000_000);
                    antimagfreq[1] = new AntImageFreq((UInt64)Math.Floor(_CurSigFreq[1].FreqMin / 100_000_000) * 100_000_000, (UInt64)local- 500_000_000);
                    bandwidth[1] = 6_000_000_000;
                }
                #endregion

                #region 子带三
                if (_CurSigFreq[2].FreqMin == 0 && _CurSigFreq[2].FreqMax == 0)
                {

                }
                else
                {
                    Int64 local = (Int64)Math.Round(_CurSigFreq[2].FreqMax / 100_000_000) * 100_000_000 + 500_000_000;
                    CurAnalogLoFreq[2] = (UInt64)local;
                    localfreq[2] = (UInt64)Math.Max(0, local);
                    CurLoFreq[2] = (UInt64)local;
                    //antimagfreq[2] = new AntImageFreq((UInt64)local+ 500_000_000, 16_500_000_000);
                    //antimagfreq[2] = new AntImageFreq((UInt64)local + 500_000_000 ,(UInt64)Math.Round(_CurSigFreq[2].FreqMax));
                    antimagfreq[2] = new AntImageFreq((UInt64)Math.Floor(_CurSigFreq[2].FreqMin / 100_000_000) * 100_000_000, (UInt64)local - 500_000_000);
                    bandwidth[2] = 6_000_000_000;
                }
                #endregion

                #region 子带四
                if (_CurSigFreq[3].FreqMin == 0 && _CurSigFreq[3].FreqMax == 0)
                {

                }
                else
                {
                    //Int64 local = (Int64)Math.Round(_CurSigFreq[3].FreqMax / 100_000_000) * 100_000_000 + 500_000_000;
                    //CurAnalogLoFreq[3] = (UInt64)local;
                    //localfreq[3] = (UInt64)Math.Max(0, local);
                    //CurLoFreq[3] = (UInt64)local;
                    ////antimagfreq[3] = new AntImageFreq((UInt64)local+ 500_000_000, 20_500_000_000);
                    ////antimagfreq[3] = new AntImageFreq((UInt64)local + 500_000_000 ,(UInt64)Math.Round(_CurSigFreq[3].FreqMax));
                    //antimagfreq[3] = new AntImageFreq((UInt64)Math.Floor(_CurSigFreq[3].FreqMin / 100_000_000) * 100_000_000, (UInt64)local - 500_000_000);
                    //bandwidth[3] = 6_000_000_000;

                    Int64 local = (Int64)Math.Round(_CurSigFreq[3].FreqMin  / 100_000_000) * 100_000_000 - 500_000_000;
                    CurAnalogLoFreq[3] = (UInt64)local;
                    localfreq[3] = (UInt64)Math.Max(0, local);
                    CurLoFreq[3] = (UInt64)local;
                    //antimagfreq[3] = new AntImageFreq((UInt64)local+ 500_000_000, 20_500_000_000);
                    //antimagfreq[3] = new AntImageFreq((UInt64)local + 500_000_000 ,(UInt64)Math.Round(_CurSigFreq[3].FreqMax));
                    antimagfreq[3] = new AntImageFreq((UInt64)local + 500_000_000, (UInt64)Math.Ceiling(_CurSigFreq[3].FreqMax / 100_000_000) * 100_000_000);
                    bandwidth[3] = 6_000_000_000;
                }

                #endregion
                if (SubbandId != 0)
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0b1111);     //信号位于第234子带
                    if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) < 1.6e9)
                    {
                        BitWidth = 16;
                    }
                    else if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) <3.6e9)
                    {
                        BitWidth = 15;
                    }
                    else 
                    {
                        BitWidth = 14;
                    }
                    Trace.WriteLine($"[ConfigCurDBI]NarrowFreqSig,BitWidth++({BitWidth})");
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                        {
                            bandwidth[subbandid] = bandwidth[SubbandId];
                            localfreq[subbandid] = localfreq[SubbandId];
                            antimagfreq[subbandid] = antimagfreq[SubbandId];
                        }
                    }

                    //switch (BitWidth)
                    //{
                    //    case 12:
                    //        break;
                    //    case 13:
                    //        break;
                    //    case 14:
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) < 3e9)
                    //        {
                    //            _NarrowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _NarrowFreqSigCnt = 0;
                    //        }
                    //        if (_NarrowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]NarrowFreqSig,BitWidth++({BitWidth})");
                    //            _NarrowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 15:
                    //        _NarrowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (Math.Abs(_CurSigFreq[SubbandId].FreqMax - _CurSigFreq[SubbandId].FreqMin) < 1e9)
                    //        {
                    //            _MoreNarrowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _MoreNarrowFreqSigCnt = 0;
                    //        }
                    //        if (_MoreNarrowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]MoreNarrowFreqSigCnt,BitWidth++({BitWidth})");
                    //            _MoreNarrowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 16:
                    //        _MoreNarrowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        break;
                    //}
                }
                else
                {
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0b0000);     //信号位于第1子带
                    if (_CurSigFreq[SubbandId].FreqMax < 1.2e9)
                    {
                        BitWidth = 16;
                    }
                    else if (_CurSigFreq[SubbandId].FreqMax < 3.2e9)
                    {
                        BitWidth = 15;
                    }
                    else 
                    {
                        BitWidth = 14;
                    }
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                        {
                            bandwidth[subbandid] = bandwidth[SubbandId];
                            localfreq[subbandid] = localfreq[SubbandId];
                            antimagfreq[subbandid] = antimagfreq[SubbandId];
                        }
                    }
                    Trace.WriteLine($"[ConfigCurDBI]LowFreqSig,BitWidth++({BitWidth})");
                    //switch (BitWidth)
                    //{
                    //    case 12:
                    //        break;
                    //    case 13:
                    //        break;
                    //    case 14:
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (_CurSigFreq[SubbandId].FreqMax < 3e9)
                    //        {
                    //            _LowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _LowFreqSigCnt = 0;
                    //        }
                    //        if (_LowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]LowFreqSig,BitWidth++({BitWidth})");
                    //            _LowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 15:
                    //        _LowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        if (_CurSigFreq[SubbandId].FreqMax < 1e9)
                    //        {
                    //            _MoreLowFreqSigCnt++;
                    //        }
                    //        else
                    //        {
                    //            _MoreLowFreqSigCnt = 0;
                    //        }
                    //        if (_MoreLowFreqSigCnt > 10)
                    //        {
                    //            BitWidth++;
                    //            Trace.WriteLine($"[ConfigCurDBI]MoreLowFreqSigCnt,BitWidth++({BitWidth})");
                    //            _MoreLowFreqSigCnt = 0;
                    //        }
                    //        break;
                    //    case 16:
                    //        _MoreLowFreqSigCnt = 0;
                    //        for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    //        {
                    //            if (subbandid != SubbandId && bandwidth.ContainsKey(SubbandId) && localfreq.ContainsKey(SubbandId) && antimagfreq.ContainsKey(SubbandId))
                    //            {
                    //                bandwidth[subbandid] = bandwidth[SubbandId];
                    //                localfreq[subbandid] = localfreq[SubbandId];
                    //                antimagfreq[subbandid] = antimagfreq[SubbandId];
                    //            }
                    //        }
                    //        break;
                    //}
                }
                if(BitWidth > 12)
                {
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        interplotsubbandinfo[subbandid] = 0;
                    }

                }
                else
                {
                    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
                    {
                        interplotsubbandinfo[subbandid] = subbandid;
                    }
                }

                ConfigSubbandBandwidth(bandwidth);
                ConfigLocalFreq(localfreq, localfreq,true);//非低频模式下，模拟本振与数字本振频率一致
                ConfigAntImageFreq(antimagfreq);
                ConfigInterplotCoe(interplotsubbandinfo);

                ConfigInterpolationCoefficients_Level1(_Ts_mode_low, SubbandId);
            }

            //ConfigSub_AmpCoefficientFileInfo(SubbandId, localfreq);//目前针对的是一个子带，下发一样的系数，后续应该要改成不同子带下发不一样的幅频系数

            SetFinish = true;


            //if (BitWidth >= 12)
            //{
            //    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
            //    {
            //        interplotsubbandinfo[subbandid] = 0;
            //    }
            //}
            //else
            //{
            //    for (Int32 subbandid = 0; subbandid < 4; subbandid++)
            //    {
            //        interplotsubbandinfo[subbandid] = subbandid;
            //    }
            //}

            //if(LowFreqMode)
            //{
            //    HdIO.WriteReg(ProcBdReg.W.reverse_Write5, 0xf);

            //}
            //else
            //{
            //    HdIO.WriteReg(ProcBdReg.W.reverse_Write6, 0);
            //    HdIO.WriteReg(ProcBdReg.W.reverse_Write5,0x0);
            //}
            //ConfigSubbandBandwidth(bandwidth);
            //ConfigLocalFreq(localfreq, localfreq);
            //ConfigAntImageFreq(antimagfreq);
            //ConfigInterplotCoe(interplotsubbandinfo);

            //ConfigLocalFreq_Level1(localfreq_level1);
            //ConfigLocalFreq_Level2(localfreq_level2);
            //ConfigAntImageFreq_Level1(antimagfreq_level1);
            //ConfigAntImageFreq_Level2(antimagfreq_level2);
            //ConfigInterpolationCoefficients_Level1(0);
        }

        private Dictionary<Int32, Int32> _LastSubbandInterplotCoe = new();
        private void ConfigInterplotCoe(Dictionary<Int32, Int32> subbandInfo)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastSubbandInterplotCoe, subbandInfo))
            {
                CoefficientsTableSender_DBI.SendInterpolationCoefficients(subbandInfo);
                PublicFunc.CopyDictionary(subbandInfo, _LastSubbandInterplotCoe);
            }
        }


        private Dictionary<Int32, UInt64> _LastLocalFreq = new Dictionary<Int32, UInt64>();
        private Dictionary<Int32, UInt64> _LastAnalogLocalFreq = new Dictionary<Int32, UInt64>();
        private Dictionary<Int32, UInt64> _LastLocalFreq_Level1 = new Dictionary<Int32, UInt64>();
        private Dictionary<Int32, UInt64> _LastLocalFreq_Level2 = new Dictionary<Int32, UInt64>();
        private void ConfigLocalFreq(Dictionary<Int32, UInt64> localFreqByHz, Dictionary<Int32, UInt64>analogFreqByHz,bool rec_local)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastLocalFreq, localFreqByHz))
            {
                // 生成并下发数字本振系数
                CoefficientsTableSender_DBI.SendLocalFreqCoeff(localFreqByHz);

                PublicFunc.CopyDictionary(localFreqByHz, _LastLocalFreq);

                for (Int32 subbandid = 0; subbandid < CurLoFreq.Length; subbandid++)
                {
                    CurLoFreq[subbandid] = _LastLocalFreq.ContainsKey(subbandid) ? _LastLocalFreq[subbandid] : (UInt64)_DefaultLoFreq[subbandid];
                }
            }

            if(!PublicFunc.CheckDictionaryEqual(_LastAnalogLocalFreq, analogFreqByHz))
            {
                // 配置模拟通道的本振频率
                CtrlAnalogChannel_DBI20G.ConfigLocalFreq(analogFreqByHz, rec_local);
                PublicFunc.CopyDictionary(analogFreqByHz, _LastAnalogLocalFreq);

                for (Int32 subbandid = 0; subbandid < CurAnalogLoFreq.Length; subbandid++)
                {
                    CurAnalogLoFreq[subbandid] = _LastAnalogLocalFreq.ContainsKey(subbandid) ? _LastAnalogLocalFreq[subbandid] : (UInt64)_DefaultLoFreq[subbandid];
                }
            }
        }

        private void ConfigLocalFreq_Level1(Dictionary<Int32, UInt64> localFreqByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastLocalFreq_Level1, localFreqByHz))
            {
                // 生成并下发数字本振系数
                CoefficientsTableSender_DBI.SendLocalFreqCoeff_Level1(localFreqByHz);

                PublicFunc.CopyDictionary(localFreqByHz, _LastLocalFreq_Level1);

                for (Int32 subbandid = 0; subbandid < CurLoFreq_Level1.Length; subbandid++)
                {
                    CurLoFreq_Level1[subbandid] = _LastLocalFreq_Level1.ContainsKey(subbandid) ? _LastLocalFreq_Level1[subbandid] : (UInt64)_DefaultLoFreq_Level1[subbandid];
                }
            }
        }

        private void ConfigLocalFreq_Level2(Dictionary<Int32, UInt64> localFreqByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastLocalFreq_Level2, localFreqByHz))
            {
                // 生成并下发数字本振系数
                CoefficientsTableSender_DBI.SendLocalFreqCoeff_Level2(localFreqByHz);

                PublicFunc.CopyDictionary(localFreqByHz, _LastLocalFreq_Level2);

                for (Int32 subbandid = 0; subbandid < CurLoFreq_Level2.Length; subbandid++)
                {
                    CurLoFreq_Level2[subbandid] = _LastLocalFreq_Level2.ContainsKey(subbandid) ? _LastLocalFreq_Level2[subbandid] : (UInt64)_DefaultLoFreq_Level2[subbandid];
                }
            }
        }

        private Dictionary<Int32, AntImageFreq> _LastAntImageFrq = new();
        private Dictionary<Int32, AntImageFreq> _LastAntImageFrq_Level1 = new();
        private Dictionary<Int32, AntImageFreq> _LastAntImageFrq_Level2 = new();
        private void ConfigAntImageFreq(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastAntImageFrq, freqByHz))
            {
                CoefficientsTableSender_DBI.SendAntiImageCoeff(freqByHz);
                PublicFunc.CopyDictionary(freqByHz, _LastAntImageFrq);
            }
        }

        private void ConfigAntImageFreq_Level1(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastAntImageFrq_Level1, freqByHz))
            {
                CoefficientsTableSender_DBI.SendAntiImageCoeff_Level1(freqByHz);
                PublicFunc.CopyDictionary(freqByHz, _LastAntImageFrq_Level1);
            }
        }

        private void ConfigAntImageFreq_Level2(Dictionary<Int32, AntImageFreq> freqByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastAntImageFrq_Level2, freqByHz))
            {
                CoefficientsTableSender_DBI.SendAntiImageCoeff_Level2(freqByHz);
                PublicFunc.CopyDictionary(freqByHz, _LastAntImageFrq_Level2);
            }
        }

        private Dictionary<Int32, UInt64> _LastSubbandBandwidth = new Dictionary<Int32, UInt64>();
        private void ConfigSubbandBandwidth(Dictionary<Int32, UInt64> bandwidthByHz)
        {
            if (!PublicFunc.CheckDictionaryEqual(_LastSubbandBandwidth, bandwidthByHz))
            {
                CtrlAnalogChannel_DBI20G.ConfigSubbandBandWidth(bandwidthByHz);
                PublicFunc.CopyDictionary(bandwidthByHz, _LastSubbandBandwidth);
            }
        }
        private Int32 _LastTs_mode = new();
        private void ConfigInterpolationCoefficients_Level1(Int32 Ts_mode, Int32 sub)
        {
            if (_LastTs_mode!= Ts_mode)
            {
                CoefficientsTableSender_DBI.SendInterpolationCoefficients_Level1(Ts_mode,sub);
                _LastTs_mode = Ts_mode;
            }
        }

        private Int32 _LastTs_sub = new();
        private void ConfigSub_AmpCoefficientFileInfo(Int32 Sub, Dictionary<Int32, UInt64> analogFreqByHz)
        {
            if (_LastTs_sub != Sub &&(!PublicFunc.CheckDictionaryEqual(_LastAnalogLocalFreq, analogFreqByHz)))
            {
                CoefficientsTableSender_DBI.SendAmpSubCoff(Sub, analogFreqByHz[Sub]);
                _LastTs_mode = Sub;

            }
        }

        private Int32 _SigCheckCnt = 0;

        internal void ConfigLast_CurSigFreqParams()
        {
            for (Int32 i = 0; i < _CurSigFreq.Length && i < _LastSigFreq.Length; i++)
            {
                Trace.WriteLine($"subband{i} sig changed{_LastSigFreq[i]} -> {_CurSigFreq[i]}");
                _LastSigFreq[i] = _CurSigFreq[i];
            }
        }

        internal Boolean CheckSigChanged()
        {
            //if (_SigCheckCnt < 1)
            //{
            //    _SigCheckCnt++;
            //    return false;
            //}
            for (Int32 subid = 0; subid < SubbandCnt; subid++)
            {
                if (Math.Abs(_CurSigFreq[subid].FreqMin - _LastSigFreq[subid].FreqMin) > 200e6 ||
                    Math.Abs(_CurSigFreq[subid].FreqMax - _LastSigFreq[subid].FreqMax) > 200e6)
                {
                    //for (Int32 i = 0; i < _CurSigFreq.Length && i < _LastSigFreq.Length; i++)
                    //{
                    //    Trace.WriteLine($"subband{i} sig changed{_LastSigFreq[i]} -> {_CurSigFreq[i]}");
                    //    _LastSigFreq[i] = _CurSigFreq[i];
                    //}
                    _SigCheckCnt = 0;
                    //if (_InitFlag)
                    //{
                    //    _InitFlag = false;
                    //    return false;
                    //} 
                    return true;
                }
            }

            return false;
        }

        private RecfgDbiStateEnum _RecfgDbiStateEnum = RecfgDbiStateEnum.DefaultState;

        private void ConfigDBIParams()
        {
            var aimsg = Hd.UIMessage?.AiTable;
            if (aimsg == null)
                return;

            var chnlid = ChannelId.C1;
            Boolean curenable = aimsg[chnlid].RecfgDbi?.Enable ?? false;
            SubbandCtrlMethod method = aimsg[chnlid].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
            var recfgDbimsg = aimsg[chnlid].RecfgDbi;

            if (curenable)
            {
                switch (_RecfgDbiStateEnum)
                {
                    case RecfgDbiStateEnum.DefaultState:
                        InitParams();
                        ConfigDefaultDBI();
                        _RecfgDbiStateEnum = RecfgDbiStateEnum.DetectSigState;
                        break;

                    case RecfgDbiStateEnum.DetectSigState:
                        ReadSubbandInfo();
                        if (CheckSigChanged())
                        {
                            _RecfgDbiStateEnum = RecfgDbiStateEnum.AutoCfgState;
                            _NarrowFreqSigCnt = 0;
                            _MoreNarrowFreqSigCnt = 0;
                            _LowFreqSigCnt = 0;
                            _MoreLowFreqSigCnt = 0;
                        }
                        break;

                    case RecfgDbiStateEnum.AutoCfgState:
                        ConfigCurDBI();
                        _RecfgDbiStateEnum = RecfgDbiStateEnum.DetectFakeSigState;
                        break;

                    case RecfgDbiStateEnum.DetectFakeSigState:
                        ReadSubbandInfo();
                        if (CheckSigChanged())
                        {
                            _RecfgDbiStateEnum = RecfgDbiStateEnum.DefaultState;
                        }
                        break;
                }
            }
            else if (recfgDbimsg != null && method != SubbandCtrlMethod.BitWidthAdaptive)
            {
                ConfigSubbandBandwidth(recfgDbimsg.BandFreqLimitByHz);

                ConfigLocalFreq(recfgDbimsg.LocalFreqByHz, recfgDbimsg.LocalFreqByHz,false);

                ConfigAntImageFreq(recfgDbimsg.AntImageFreqByHz);

                _RecfgDbiStateEnum = RecfgDbiStateEnum.DefaultState;
                HdIO.WriteReg(ProcBdReg.W.DBI_DbiSignSub, 0x0); //hcj_c
                BitWidth = 12;
                _FullDBI = true;
                //Trace.WriteLine($"[ConfigDBIParams]method is {method}, Bitwidth = 12");
            }
        }

        internal void InitDetectModel() //需要在什么时候下发回读这些内容？eg：在开机时？初始化时？ //需要在开机时
        {
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00000);//rst //正常下发
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00001);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00000);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_threshold, 22);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_change_threshold_max, 20);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_change_threshold_min, 20);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_config_tdata_l16, 0b100001100);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_config_tdata_h16, 0b0);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b01100);//config_v_pos
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b11100);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b01100);


            UInt32 freq_change_flag_1 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_flag, AcqBdNo.B11); //cyy_10.31 //莫名其妙4个flag都是0，但有波形，疑似寄存器不再起作用
            UInt32 freq_change_cnt_1 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_cnt, AcqBdNo.B11);
            UInt32 freq_change_flag_2 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_flag, AcqBdNo.B12);
            UInt32 freq_change_cnt_2 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_cnt, AcqBdNo.B12);
            UInt32 freq_change_flag_3 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_flag, AcqBdNo.B7);
            UInt32 freq_change_cnt_3 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_cnt, AcqBdNo.B7);
            UInt32 freq_change_flag_4 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_flag, AcqBdNo.B10);
            UInt32 freq_change_cnt_4 = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_change_cnt, AcqBdNo.B10);
        }

        internal Int32 BitWidth
        {
            get;
            private set;
        } = 12;

        internal Boolean SetFinish
        {
            get;
            set;
        } = false;

        internal Int32 SubbandId = 0;

        /// <summary>
        /// 独热码：每个子带的信号是否是真正有效的
        /// </summary>
        internal Int32 SubbandValid
        {
            get;
            set;
        } = 0xf;

        internal Boolean LowFreqMode
        {
            get;
            set;
        } = false;

        private UInt32 _LastSubbandEnable = 0xf;

        private void AutoCfgSubbandEnable(IEnumerable<Int32> enableSubbandIdArray)
        {
            UInt32 bitvalue = PublicFunc.ConvertToUniqueHotCode(enableSubbandIdArray);
            if (bitvalue == 0)
            {
                bitvalue = _LastSubbandEnable;
            }
            HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, bitvalue); //hcj_c
            if (bitvalue != _LastSubbandEnable)
            {
                Trace.WriteLine($"**************DBI_SelectSub changed {_LastSubbandEnable} -> {bitvalue}**************");
                HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, 0xf); //hcj_c
            }
            _LastSubbandEnable = bitvalue;
        }

        //internal void ConfigSubbandEnabel(ChannelId chnlId)
        internal Boolean ConfigSubbandEnabel(ChannelId chnlId)
        {
            Int32 enablecnt = 0;
            var enableids = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key) ?? new Int32[] { 0 };
            foreach (Int32 subbandid in enableids)
            {
                if (((0x1 << subbandid) & SubbandValid) != 0)
                {
                    enablecnt++;
                }
            }
            if (enablecnt == 1)
            {
                if (enableids.Count() == 1)
                    SubbandId = enableids.FirstOrDefault();
                SubbandValid = 0x1 << SubbandId;
                HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, 0xf); //hcj_c
                if (BitWidth <= 14)
                {
                    BitWidth = 14;
                    Trace.WriteLine($"[ConfigSubbandEnabel]subband enablecnt = 1, Bitwidth = 14");
                }
                return false;
            }
            else
            {
                SubbandValid = 0xf;
                AutoCfgSubbandEnable(enableids);
                BitWidth = 12;
                Trace.WriteLine($"[ConfigSubbandEnabel]subband enablecnt = {enablecnt}, Bitwidth = 12");
                return true;
            }
        }

        internal void ConfigSubbandState()
        {
            var aimsg = Hd.UIMessage?.AiTable;
            if (aimsg != null)
            {
                ChannelId chnlid = ChannelId.C1;
                SubbandCtrlMethod ctrlmethod = aimsg[chnlid].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
                var enableids = Hd.CurrProduct.Acquirer_AnalogChannel?.SubbandEnergyTable.Where(o => o.Value).Select(o => o.Key) ?? new Int32[] { 0 };
                switch (ctrlmethod)
                {
                    case SubbandCtrlMethod.UserManual:
                        //HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, aimsg[chnlid].RecfgDbi?.SubbandEnable ?? 0xf);
                        HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, 0xf);
                        SubbandValid = (Int32)(aimsg[chnlid].RecfgDbi?.SubbandEnable ?? 0xf);
                        break;

                    case SubbandCtrlMethod.SubbandAdaptive:
                        AutoCfgSubbandEnable(enableids);
                        BitWidth = 12;
                        Trace.WriteLine($"[ConfigSubbandState]SubbandCtrlMethod.SubbandAdaptive, Bitwidth = 12");
                        break;

                    case SubbandCtrlMethod.BitWidthAdaptive:
                        //Int32 enablecnt = 0;
                        //foreach (Int32 subbandid in enableids)
                        //{
                        //    if (((0x1 << subbandid) & SubbandValid) != 0)
                        //    {
                        //        enablecnt++;
                        //    }
                        //}
                        //if (enablecnt == 1)
                        //{
                        //    SubbandId = enableids.FirstOrDefault();
                        //    SubbandValid = 0x1 << SubbandId;
                        //    HdIO.WriteReg(ProcBdReg.W.DBI_SelectSub, 0xf);
                        //    BitWidth = 14;
                        //}
                        //else
                        //{
                        //    SubbandValid = 0xf;
                        //    AutoCfgSubbandEnable(enableids);
                        //    BitWidth = 12;
                        //}

                        break;
                }
            }
        }

        internal record SubbandInfo(AcqBdNo AcqBd, Func<Double, UInt32, Double> CalcFreqFunc);
        private const Int32 _FreqCountMax = 20;//100;修改为20与这个_ExcuteCnt保持一致，用于清空每次可重构时上一轮的值。
        internal void UpdateSubbandInfo()
        {
            for ( Int32 subbandid = 0; subbandid < _FreqMaxList.Length && subbandid < _FreqMinList.Length; subbandid++)
            {
                if (_FreqMaxList[subbandid].Count > 0 && _FreqMinList[subbandid].Count > 0)
                {
                     _CurSigFreq[subbandid] = new SigFreqRange(_FreqMaxList[subbandid].Max(), _FreqMinList[subbandid].Min());
                    //Trace.WriteLine($"[UpdateSubbandInfo]subband{subbandid + 1},FreqMax:{_CurSigFreq[subbandid].FreqMax},FreqMin:{_CurSigFreq[subbandid].FreqMin}");
                }
                else
                {
                    _CurSigFreq[subbandid] = new SigFreqRange(0, 0);
                }
            }
        }
        internal void ReadSubbandInfo()
        {
            SubbandInfo[] subbanddefines = new SubbandInfo[] { };
            if (_FullDBI == true)
            {
                 subbanddefines = new SubbandInfo[]
               {
                    new(AcqBdNo.B0, RightFreqBand),
                    new(AcqBdNo.B1, LeftFreqBand ),
                    new(AcqBdNo.B2,  LeftFreqBand),
                    new(AcqBdNo.B3, LeftFreqBand),
               };
            }
            else
            {
                 subbanddefines = new SubbandInfo[]
               {
                    new(AcqBdNo.B0, RightFreqBand),
                    new(AcqBdNo.B1, LeftFreqBand ),
                    new(AcqBdNo.B2,  LeftFreqBand),
                    new(AcqBdNo.B3, RightFreqBand),
               };
            }


            Dictionary<Int32, UInt32> ratiodefine = new()
            {
                [12] = 1,
                [13] = 2,
                [14] = 1,
                [15] = 2,
                [16] = 4,
            };

            Dictionary<Int32, UInt32> ratiodefine_low = new()
            {
                [12] = 1,
                [13] = 16,
                [14] = 8,
                [15] = 16,
                [16] = 32,
            };

            for (Int32 subid = 0; subid < subbanddefines.Length; subid++)
            {
                if ((SubbandValid & (0x1 << subid)) == 0)
                {
                    //_CurSigFreq[subid] = new SigFreqRange(0, 0);
                    _FreqMaxList[subid].Clear();
                    _FreqMinList[subid].Clear();
                    continue;
                }
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00011);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b01100);
                //Thread.Sleep(1000);
                var tp = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_out_valid, subbanddefines[subid].AcqBd);

                if (Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_out_valid, subbanddefines[subid].AcqBd) == 0x1u)
                {
                    UInt32 max = 0, min = 0;
                    for (int i = 0; i < 20; i++)
                    {
                        max = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_max, subbanddefines[subid].AcqBd);
                        min = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_min, subbanddefines[subid].AcqBd);
                    }
                    //if (_frequency_low_flag == 1)
                    //{
                    //    if (ratiodefine_low.ContainsKey(BitWidth))
                    //    {
                    //        max = max / ratiodefine_low[BitWidth];
                    //        min = min / ratiodefine_low[BitWidth];

                    //    }
                    //}
                    //else
                    //{
                        if (ratiodefine.ContainsKey(BitWidth))
                        {
                            max = max / ratiodefine[BitWidth];
                            min = min / ratiodefine[BitWidth];
                        }
                    //}
                    var calcfunc = subbanddefines[subid].CalcFreqFunc; 
                    //var tmp = new SigFreqRange(calcfunc(CurLoFreq[subid], max), calcfunc(CurLoFreq[subid], min));
                    var tmp = new SigFreqRange(new(),new());
                    if(subid == 0)
                    {
                        tmp = new SigFreqRange(calcfunc(CurAnalogLoFreq[subid], max), calcfunc(CurAnalogLoFreq[subid], min));
                    }
                    else if(subid == 1 || subid == 2)
                    {
                        tmp = new SigFreqRange(calcfunc(CurAnalogLoFreq[subid], min), calcfunc(CurAnalogLoFreq[subid], max));
                    }
                    else
                    {
                        if(_FullDBI == true)
                        {
                            tmp = new SigFreqRange(calcfunc(CurAnalogLoFreq[subid], min), calcfunc(CurAnalogLoFreq[subid], max));
                        }
                        else
                        {
                            tmp = new SigFreqRange(calcfunc(CurAnalogLoFreq[subid], max), calcfunc(CurAnalogLoFreq[subid], min));
                        }
                    }

                    //Trace.WriteLine($"[ReadSubbandInfo]subband{subid + 1},Max:{tmp.FreqMax},Min:{tmp.FreqMin}");
                    //直接对应不同子带是高侧还是低侧混频
                    _FreqMaxList[subid].Add(tmp.FreqMax);
                    _FreqMinList[subid].Add(tmp.FreqMin);
                    if (subid == 2 && tmp.FreqMax > 13000000000) 
                    {
                        ;
                        //_FreqMaxList[subid].Add(13.5e9 + 20e6);
                        //_FreqMinList[subid].Add(13.5e9 - 20e6);
                    }

                    if (_FreqMaxList[subid].Count > _FreqCountMax)
                    {
                        _FreqMaxList[subid].RemoveAt(0);
                    }
                    if (_FreqMinList[subid].Count > _FreqCountMax)
                    {
                        _FreqMinList[subid].RemoveAt(0);
                    }
                }
                else
                {
                    _FreqMaxList[subid].Clear();
                    _FreqMinList[subid].Clear();
                }
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00011);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b01100);
        }

        private double ReadFrequencyMeterData(int whichChannel = 0)
        {
            //频率计测试结果
            uint n_l = 0;
            uint n_h = 0;
            uint t_l = 0;
            uint t_h = 0;

            //获取当前触发通道
            ChannelId trigChannel = (ChannelId)Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource();

            if (trigChannel == ChannelId.AC ||
                trigChannel == ChannelId.Ext ||
                trigChannel == ChannelId.Ext5)
            {
                t_l = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1StandardFrequenceCountL);
                t_h = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1StandardFrequenceCountH);
                n_l = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1FrequenceCountL);
                n_h = HdIO.ReadReg(ProcBdReg.R.Cymometer_Pro_Ch1FrequenceCountH);
            }
            else // 模拟通道
            {
                var channelAcqBdAdcInputCorresponding = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetChannelAcqBdAdcInputCorresponding((int)trigChannel);
                if (channelAcqBdAdcInputCorresponding != null)
                {
                    AcqBdNo acqBdNo = channelAcqBdAdcInputCorresponding.BdNo;
                    (AcqBdReg.R FrequenceCountL, AcqBdReg.R FrequenceCountH, AcqBdReg.R StandardFrequenceCountL, AcqBdReg.R StandardFrequenceCountH)[] registers =
                    {
                    (AcqBdReg.R.Cymometer_Ch1FrequenceCountL, AcqBdReg.R.Cymometer_Ch1FrequenceCountH, AcqBdReg.R.Cymometer_Ch1StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch1StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch2FrequenceCountL, AcqBdReg.R.Cymometer_Ch2FrequenceCountH, AcqBdReg.R.Cymometer_Ch2StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch2StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch3FrequenceCountL, AcqBdReg.R.Cymometer_Ch3FrequenceCountH, AcqBdReg.R.Cymometer_Ch3StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch3StandardFrequenceCountH),
                    (AcqBdReg.R.Cymometer_Ch4FrequenceCountL, AcqBdReg.R.Cymometer_Ch4FrequenceCountH, AcqBdReg.R.Cymometer_Ch4StandardFrequenceCountL, AcqBdReg.R.Cymometer_Ch4StandardFrequenceCountH),
                    };

                    n_l = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].FrequenceCountL, acqBdNo));
                    n_h = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].FrequenceCountH, acqBdNo));
                    t_l = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].StandardFrequenceCountL, acqBdNo));
                    t_h = (uint)(Hd.CurrProduct!.AcqBd!.ReadReg(registers[whichChannel].StandardFrequenceCountH, acqBdNo));
                }
            }

            uint n = (n_h << 16) | n_l;
            uint t = (t_h << 16) | t_l;
            if (t != 0 && n != 0xffffffff)
            {
                return n * 312.5 * 1e6 / t;
            }
            else
            {
                return 0.0;
            }
        }
        private Double LeftFreqBand(Double localFreq, UInt32 fftid)
        {
            if(LowFreqMode)
            {
                return localFreq - fftid * 2.5 * 1e9 / 4096;
            }
            else
            {
                return localFreq - fftid * 20 * 1e9 / 4096;
            }
        }

        private Double RightFreqBand(Double localFreq, UInt32 fftid)
        {
            if(LowFreqMode)
            {
                return localFreq + fftid * 2.5 * 1e9 / 4096;
            }
            else
            {
                return localFreq + fftid * 20 * 1e9 / 4096;
            }
            
        }

        internal Dictionary<ChannelId, UInt16[]?> DataPool = new();

        internal enum RecfgDbiStateEnum
        {
            /// <summary>
            /// 开机初始化或者重新检测信号频率
            /// </summary>
            DefaultState,

            /// <summary>
            /// 检测真实的信号频率
            /// </summary>
            DetectSigState,

            /// <summary>
            /// 根据真实的信号频率重配DBI
            /// </summary>
            AutoCfgState,

            /// <summary>
            /// 过渡态，表示需要重新初始化
            /// </summary>
            DetectFakeSigState,
        }
    }
}
