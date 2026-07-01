using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;
using System.Drawing.Imaging;
using Ivi.Visa;
using System.Threading.Channels;
using ScopeX.Hardware.Driver.Module;
using ScopeX.Hardware.Driver.Registers.SendManage;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_AnalogChannel_JiHe_MSO8000X : AbstractAcquirer_AnalogChannel
    {
        private SyncParams[] _SyncParams = new SyncParams[ChannelIdExt.AnaChnlNum];

        internal override SyncParams[] SyncParams()
        {
            return _SyncParams; 
        }


        private String _SyncParamsFileName = "SyncParams.txt";
        private Boolean LoadSyncParams()
        {
            if (!File.Exists(_SyncParamsFileName))
            {
                for (Int32 i = 0; i < _SyncParams.Length; i++)
                {
                    if (_SyncParams[i] == null)
                        _SyncParams[i] = new SyncParams(0, 0.0);
                }
                return false;
            }

            String datastr = File.ReadAllText(_SyncParamsFileName);
            String[] paramstr = datastr.Split('\n');

            for (Int32 chnlid = 0; chnlid < _SyncParams.Length && chnlid < paramstr.Length; chnlid++)
            {
                String[] syncparamstr = paramstr[chnlid].Split(",");
                if (syncparamstr.Length >= 2)
                {
                    Boolean dotcntflag = UInt32.TryParse(syncparamstr[0].Trim(), out UInt32 dotscnt);
                    Boolean farrowdelayflag = Double.TryParse(syncparamstr[1].Trim(), out Double farrowdelay);
                    if (dotcntflag && farrowdelayflag)
                    {
                        _SyncParams[chnlid] = new SyncParams(dotscnt, farrowdelay);
                    }
                    else if (_SyncParams[chnlid] == null)
                    {
                        _SyncParams[chnlid] = new SyncParams(0, 0.0);
                    }
                }
            }

            return true;
        }

        private void SaveSyncParams()
        {
            StringBuilder sb = new StringBuilder();
            for (Int32 i = 0; i < _SyncParams.Length; i++)
            {
                sb.Append($"{_SyncParams[i].DotsCnt},{_SyncParams[i].FarrowDelayByFs}\n");
            }
            StreamWriter sw = new StreamWriter(_SyncParamsFileName);
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }

        private Boolean CheckPhaseOK(Dictionary<ChannelId, Double> phaseErrorByfs, Double phaseLimitsByFs = 3000)
        {
            foreach (Double phase in phaseErrorByfs.Values)
            {
                if (Math.Abs(phase) >= phaseLimitsByFs)
                    return false;
            }
            return true;
        }

        private Boolean CheckDotCntOk(Dictionary<ChannelId, UInt32> discardDotsCnt)
        {
            Boolean flag = true;
            foreach (ChannelId chnlid in discardDotsCnt.Keys)
            {
                if (discardDotsCnt[chnlid] != 0 && (Int32)chnlid < _SyncParams.Length)
                {
                    UInt32 dotscnt = _SyncParams[(Int32)chnlid].DotsCnt + discardDotsCnt[chnlid];
                    _SyncParams[(Int32)chnlid] = _SyncParams[(Int32)chnlid] with { DotsCnt = dotscnt };
                    flag = false;
                }

            }
            return flag;
        }

        private void ValidDotsCnt(UInt32 maxDotsCnt, IEnumerable<ChannelId> chnlList)
        {
            UInt32 mindotscnt = UInt32.MaxValue;
            foreach (ChannelId chnlid in chnlList)
            {
                if (_SyncParams.Length > (Int32)chnlid && _SyncParams[(Int32)chnlid].DotsCnt < mindotscnt)
                {
                    mindotscnt = _SyncParams[(Int32)chnlid].DotsCnt;
                }
            }

            foreach (ChannelId chnlid in chnlList)
            {
                if (_SyncParams.Length > (Int32)chnlid)
                {
                    UInt32 dotscnt = (_SyncParams[(Int32)chnlid].DotsCnt - mindotscnt) % maxDotsCnt;
                    _SyncParams[(Int32)chnlid] = _SyncParams[(Int32)chnlid] with { DotsCnt = dotscnt };
                }
            }
        }

        private void ValidFarrowDelay(Dictionary<ChannelId, Double> phaseError, IEnumerable<ChannelId> chnlList, Double maxphasebyfs)
        {
            foreach (ChannelId chnlid in phaseError.Keys)
            {
                if ((Int32)chnlid < _SyncParams.Length)
                {
                    Double phase = _SyncParams[(Int32)chnlid].FarrowDelayByFs + phaseError[chnlid];
                    _SyncParams[(Int32)chnlid] = _SyncParams[(Int32)chnlid] with { FarrowDelayByFs = phase };
                }
            }

            Double minphase = Double.MaxValue;
            foreach (ChannelId chnlid in chnlList)
            {
                if (_SyncParams.Length > (Int32)chnlid && _SyncParams[(Int32)chnlid].FarrowDelayByFs < minphase)
                {
                    minphase = _SyncParams[(Int32)chnlid].FarrowDelayByFs;
                }
            }

            foreach (ChannelId chnlid in chnlList)
            {
                if (_SyncParams.Length > (Int32)chnlid)
                {
                    Double phase = (_SyncParams[(Int32)chnlid].FarrowDelayByFs - minphase) % maxphasebyfs;
                    Int32 dot = (int)Math.Floor((_SyncParams[(Int32)chnlid].FarrowDelayByFs - minphase)/ maxphasebyfs);
                    _SyncParams[(Int32)chnlid] = _SyncParams[(Int32)chnlid] with { FarrowDelayByFs = phase };
                    //_SyncParams[(Int32)chnlid] = _SyncParams[(Int32)chnlid] with { DotsCnt = _SyncParams[(Int32)chnlid].DotsCnt+1 };
                }
            }


        }

        private void SendFarrowDelay(IEnumerable<ChannelId> chnlList)
        {
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable      // reg_farrow_filter_en,reg_int_delay_en  
            foreach (Int32 chnlid in chnlList)
            {
                if (_SyncParams.Length> chnlid)
                {
                    var delay = _SyncParams[chnlid].FarrowDelayByFs;
                    //double[] delay = {  13000,0, 13000, 13000 };
                    //delay = 1000;
                    //比例关系,总控制字16bit,最大65535,相差按照比例转换成控制字 errorbyfs / 采样间隔(fs单位)                                  
                    UInt32 farrowDelay = ~(UInt32)(65535 * (delay / 25000)) + 1;//取负数
                    //farrowDelay =~(UInt32)1;
                    //发送分数延迟滤波器延迟值
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0b1u << chnlid);//通道选择
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyL16, farrowDelay);//low 
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyH1, farrowDelay >> 16);//high
                    Thread.Sleep(10);
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0);
                    Thread.Sleep(100);
                }
            }
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能
        }

        private void SendDiscardDotsCnt(IEnumerable<ChannelId> chnlList)
        {
            for (int i = 0; i < _SyncParams.Count(); i++)
            {
                if (_SyncParams[i].DotsCnt>200)
                {
                    for (int j = 0; j < _SyncParams.Count(); j++)
                    {
                        if (_SyncParams[j].DotsCnt<200)
                        {
                            _SyncParams[j] = _SyncParams[j] with { DotsCnt = _SyncParams[j].DotsCnt + 400 };
                        }
                       
                    }
                    uint min = 40000;
                    for (int j = 0; j < _SyncParams.Count(); j++)
                    {
                        if (_SyncParams[j].DotsCnt< min)
                        {
                            min = _SyncParams[j].DotsCnt;
                        }
                    }
                    for (int j = 0; j < _SyncParams.Count(); j++)
                    {
                        _SyncParams[j] = _SyncParams[j] with { DotsCnt = _SyncParams[j].DotsCnt-min};
                    }
                }

            }
            foreach (Int32 chnlid in chnlList)
            {
                if (_SyncParams.Length > chnlid)
                {
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0b1u << chnlid);//延迟丢点通道选择
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, _SyncParams[chnlid].DotsCnt);
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 0);
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 5);
                    HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);
        //            HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 1);
                    
                }
            }
        }

        private Dictionary<ChannelId, Int16[]> GetAllChnlData(IEnumerable<ChannelId> chnlList)
        {
            Int32 dataLen = 2048;//取点算三参数误差。

            Dictionary<ChannelId, Int16[]> ans = new();
            var readinfo = new List<ReadInfo>
            {
                new ReadInfo(AcqDataType.AnalogChannel,
                             new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
                             new WfmPkgInfo(10000, 0.1, 0),//参数
                             ""),
            };
            Dictionary<AcqDataType, double> samplingRate = new Dictionary<AcqDataType, double>();
            //Thread.Sleep(100);//因为有各种数字处理，采集速度慢，延迟保证正常取数。
            Boolean acqOk = Hd.AcqWave(false, false, readinfo, ref samplingRate);
            if ((!acqOk) || AcqedDataPool.AnalogChData.AllChannelData.Count < chnlList.Count())
            {
                System.Diagnostics.Trace.WriteLine($"获取数据失败acqOK = {acqOk}");
                HdIO.Sleep(5);
                return ans;
            }
            foreach (var chnlId in chnlList)
            {
                ans[chnlId] = new Int16[dataLen];
                for (Int32 i = 0; i < dataLen && i < AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Count; i++)
                {
                    ans[chnlId][i] = (Int16)AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId][i];
                }
            }
            return ans;
        }

        private Dictionary<ChannelId, Double> GetChnlPhaseByfs(IEnumerable<ChannelId> chnlList, double sampleFreqByMHz, double signalFreqByMHz)
        {
            int calcTimes = 10;
            Dictionary<ChannelId, List<Double>> phaseTempList = new();
            Dictionary<ChannelId, Double> phaseAvg = new();
            Dictionary<ChannelId, Double> result = new();

            foreach (var chnlId in chnlList)
            {
                result[chnlId] = new();
                phaseTempList[chnlId] = new();
                phaseAvg[chnlId] = new();
            }
            for (int i = 0; i <= calcTimes; i++)
            {
                for (Int32 j = 0; j < 50; j++)
                {
                    if (AbstractController_Misc.AcqIsFulled())
                    {
                        break;
                    }
                    HdIO.Sleep(1);
                }
                Dictionary<ChannelId, short[]> chnlData = GetAllChnlData(chnlList);
//                StringBuilder stringBuilder = new StringBuilder();
                //foreach(var v_k in chnlData)
                //{
                //    stringBuilder.Clear();
                //    foreach (var v_v in v_k.Value)
                //        stringBuilder.AppendLine(v_v.ToString());
                //    String fileName = $@"C:\Users\liqiang\Desktop\{v_k.Key}.txt";
                //    if (File.Exists(fileName))
                //        File.Delete(fileName);
                //    File.WriteAllText(fileName, stringBuilder.ToString());
                //}
                if (chnlData.Count == 0)//若没有数据，则进行下一次循环，后续代码不执行。
                {
                    System.Diagnostics.Trace.WriteLine($"未正确获取数据");
                    HdIO.Sleep(5);
                    continue;
                }
                if (i == 0)
                    continue;

                foreach (var chnlId in chnlData.Keys)
                {
                    var sinfitresult = SineFitFunc.SineFit(chnlData[chnlId], sampleFreqByMHz, signalFreqByMHz);
                    phaseTempList[chnlId].Add(sinfitresult.Phase);
                }
            }

            //double[] deltaerror = {18133.0, 20989,37029, 9914 };//内外部源相位补偿值，单位fs。内-外
            //foreach (var chnlId in phaseTempList.Keys)
            //{
            //    Double sinAverage = phaseTempList[chnlId].Select(o => Math.Sin(o)).Average();
            //    Double cosAverage = phaseTempList[chnlId].Select(o => Math.Cos(o)).Average();
            //    Double phaseByfs = Math.Atan2(sinAverage, cosAverage) * 1000_000_000 / signalFreqByMHz / (2 * Math.PI);

            //    if (phaseByfs > 1000_000_000 / signalFreqByMHz / 2)
            //        phaseByfs -= 1000_000_000 / signalFreqByMHz;
            //    else if (phaseByfs < -1000_000_000 / signalFreqByMHz / 2)
            //        phaseByfs += 1000_000_000 / signalFreqByMHz;

            //    phaseAvg[chnlId] = phaseByfs /*+ deltaerror[(int)chnlId]*/;
            //}

            //Double basephase = phaseAvg.Values.Max();
            //foreach (var chnlid in phaseAvg.Keys)
            //{
            //    result[chnlid] = Math.Abs(phaseAvg[chnlid] - basephase);
            //    System.Diagnostics.Trace.WriteLine($"{chnlid}:{String.Join(',', phaseTempList[chnlid].Select(o => o.ToString("0.0000")).ToArray())}");
            //    HdIO.Sleep(5);
            //}
            Dictionary<ChannelId, List<Double>> phasedelta = new();
            foreach (var chnlId in chnlList)
            {
                phasedelta[chnlId] = new();
            }
            for (int i = 0; i < calcTimes; i++)
            {
                phasedelta[ChannelId.C1].Add(getphase((double)phaseTempList[ChannelId.C1][i]) - getphase(phaseTempList[ChannelId.C1][i]));
                phasedelta[ChannelId.C2].Add(getphase((double)phaseTempList[ChannelId.C2][i]) - getphase(phaseTempList[ChannelId.C1][i]));
                phasedelta[ChannelId.C3].Add(getphase((double)phaseTempList[ChannelId.C3][i]) - getphase(phaseTempList[ChannelId.C1][i]));
                phasedelta[ChannelId.C4].Add(getphase((double)phaseTempList[ChannelId.C4][i]) - getphase(phaseTempList[ChannelId.C1][i]));
            }
            double[] phaseerror = { 0, 0, 0, 0 };
            double[] deltaerror = {0, 0, 0, 0 };
            foreach (var chnlId in chnlList)
            {
                phaseerror[(Int32)chnlId] = phasedelta[chnlId].Average()+ deltaerror[(Int32)chnlId];
            }
            Double basephase = phaseerror.Max();
            foreach (var chnlid in phaseAvg.Keys)
            {
                result[chnlid] = Math.Abs(phaseerror[(Int32)chnlid]- basephase);
                System.Diagnostics.Trace.WriteLine($"{chnlid}:{String.Join(',', phaseTempList[chnlid].Select(o => o.ToString("0.0000")).ToArray())}");
                HdIO.Sleep(5);
            }



            //double[] deltaerror = { 0, 0, 0, 0 };
            //foreach (var chnlId in phaseTempList.Keys)
            //{
            //    Double sinAverage = phaseTempList[chnlId].Select(o => Math.Sin(o)).Average();
            //    Double cosAverage = phaseTempList[chnlId].Select(o => Math.Cos(o)).Average();
            //    Double phaseByfs = Math.Atan2(sinAverage, cosAverage) * 1000_000_000 / signalFreqByMHz / (2 * Math.PI);

            //    if (phaseByfs > 1000_000_000 / signalFreqByMHz / 2)
            //        phaseByfs -= 1000_000_000 / signalFreqByMHz;
            //    else if (phaseByfs < -1000_000_000 / signalFreqByMHz / 2)
            //        phaseByfs += 1000_000_000 / signalFreqByMHz;

            //    phaseAvg[chnlId] = phaseByfs /*+ deltaerror[(int)chnlId]*/;
            //}

            //Double basephase = phaseAvg[ChannelId.C1];
            //foreach (var chnlid in phaseAvg.Keys)
            //{
            //    result[chnlid] = Math.Abs(phaseAvg[chnlid] - basephase);
            //    System.Diagnostics.Trace.WriteLine($"{chnlid}:{String.Join(',', phaseTempList[chnlid].Select(o => o.ToString("0.0000")).ToArray())}");
            //    HdIO.Sleep(5);
            //}
            double getphase(double value)
            {
                Double sinAverage = Math.Sin(value);
                Double cosAverage = Math.Cos(value);
                Double phaseByfs = Math.Atan2(sinAverage, cosAverage) * 1000_000_000 / 100 / (2 * Math.PI);

                if (phaseByfs > 1000_000_000 / 100 / 2)
                    phaseByfs -= 1000_000_000 / 100;
                else if (phaseByfs < -1000_000_000 / 100 / 2)
                    phaseByfs += 1000_000_000 / 100;

                return phaseByfs;
            }
            return result;
        }

        


        public Dictionary<ChannelId, UInt32> Phase2DotsNumber(Dictionary<ChannelId, Double> phaseErrorByfs, double sampleFreqByMHz)
        {
            Double TsByfs = 1_000_000_000d / sampleFreqByMHz;//采样点时间间隔,单位fs。
            Dictionary<ChannelId, UInt32> result = new();
            Dictionary<ChannelId, Int32> tmp = new();
            foreach (ChannelId chnlId in phaseErrorByfs.Keys)
            {
                tmp[chnlId] = ((Int32)Math.Floor(phaseErrorByfs[chnlId] / TsByfs));
            }

            Int32 minvalue = tmp.Values.Min();

            foreach (ChannelId chnlId in tmp.Keys)
            {
                result[chnlId] = (UInt32)(tmp[chnlId] - minvalue);
            }
            return result;
        }

        internal override void ChnlSyncDiscardDotsEx(/*HdMessage? hdMessage*/)
        {
            ChannelId[] chnlsarray = new ChannelId[] { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 };

            double sampleFreqByMHz = 40000.0;//示波器基础的采样率（不经抽取和插值）
            double signalFreqByMHz = 100.0;//输入通道的信号的频率(内部源频率100MHz)

            LoadSyncParams();

            if (Hd.UIMessage == null)//加保护
                return;

            HdMessage backHdMessage = Hd.UIMessage with { };//备份校准前示波器的各项设置           
            //var storagedotscnt = Hd.GetStorageLengthSource(chnlsarray.ToArray());
            var storagedotscnt = Hd.CurrProduct?.Acquirer_AnalogChannel?.TryGetStorageDotsCnt("C1_C2_C3_C4",'_').ToArray() ?? new Int32[] { 0 };
            AbstractController_AnalogChannel.CtrlAnalogChannelSet();
            if (Hd.UIMessage == null || Hd.UIMessage.Timebase == null || Hd.UIMessage.Analog == null || Hd.UIMessage.Trigger == null)
                return;

            int _maxCaliTimeByms = 60_000;

            HdMessage Message = Hd.UIMessage with
            {
                Display = Hd.UIMessage.Display with
                {
                    IsFast = false,
                },
                Trigger = Hd.UIMessage!.Trigger! with
                {
                    Edge = Hd.UIMessage!.Trigger!.Edge! with
                    {
                        Position = 0,
                    }
                },
                Timebase = Hd.UIMessage.Timebase with
                {
                    //StorageMode = AnaChnlStorageMode.Long,
                    //StorageWaveDotsCnt = storagedotscnt.Min(),
                    StorageWaveDotsCnt = 1000000,
                    SegmentActive = 0,
                    //StorageMethod = AnaChnlStorageMethod.Manual,
                    TmbScale = 0.01,
                    TmbScaleIndex = (int)AnaChnlTimebaseIndex.Lv10n,
                },
                Analog = new HdMessage.AnalogOptions[]
                {
                    Hd.UIMessage.Analog[0] with{ ScaleIndex =(Int32) AnaChnlScaleIndex.Lv100m ,Position = 0/*, CoeffType = coeffType.AAA*/ },
                    Hd.UIMessage.Analog[1] with{ ScaleIndex =(Int32) AnaChnlScaleIndex.Lv100m, Position = 0 },
                    Hd.UIMessage.Analog[2] with{ ScaleIndex =(Int32) AnaChnlScaleIndex.Lv100m, Position = 0 },
                    Hd.UIMessage.Analog[3] with{ ScaleIndex =(Int32) AnaChnlScaleIndex.Lv100m, Position = 0 },
                }
            };

            Hd.CurrDebugVarints.bEnableAutoFanControl = false;
            Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_ProbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;
            Hd.CurrDebugVarints.bEnable_Dsp = true;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
            Hd.CurrDebugVarints.bEnable_ChannelSync = true;//分开绑定
            Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = true;//分开绑定
            Hd.CurrDebugVarints.bEnable_bandwidth = false;//分开绑定
            Hd.CurrDebugVarints.bEnable_analog_signal = true;//分开绑定

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;

            //ChannelSync_IntDelayEn
            //ChannelSync_FarrowFilterEn
            #region MyRegion
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 1);//int delay enable
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable
            //int[]? dataArray = Misc.ReadCaliCoefDataFronmFile($"C:\\Users\\9Y\\Desktop\\Y9_FreqDetect_V5.0_0526_dpodecoder_Commented\\farrow_10x32_ploy_5ps.txt");
            int[]? dataArray = Misc.ReadCaliCoefDataFronmFile($@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\CoeFiles\coe_farrow.txt");
            
            if (dataArray == null)
                return;
            int dataCount = dataArray.Length;

            for (uint i = 0; i < dataCount; i++)
            {
                uint data = (uint)dataArray[i];
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable      // reg_farrow_filter_en,reg_int_delay_en             
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    // reg_farrow_factor_wen           
                //HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, i);//coef address         //reg_farrow_factor_wa                         
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWa, ((uint)(i / 108) << 7) + i % 108);//coef address         //reg_farrow_factor_wa    20250806 htf                     
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdL16, data & 0xffff);//coef data Low16bit       //reg_farrow_factor_wd_l16        
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWdH3, (data >> 16) & 0x7);//coef data High3bit  //reg_farrow_factor_wd_h3
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 1);//coef write enable    //reg_farrow_factor_wen           
                HdIO.DelayByUs(10);
            }
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFactorWen, 0);//coef write enable    //reg_farrow_factor_wen       
            #endregion




            //Hd.LocalCommands |= (long)HdCmd.DpxEnabled;
            //Hd.LocalCommands |= (long)HdCmd.TmbStorageLen;//fast快采模式切换成正常状态。

            Hd.LocalCommands |= (long)HdCmd.TrigTypeAndParameters;
            Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;
            Hd.LocalCommands |= (long)HdCmd.ChnlGain;
            Hd.Execute(Message);
            Thread.Sleep(10);

            uint InterpolationNum = AcquingParameters.InterplotNumToDMA;//获取时基档对应的插值参数
            sampleFreqByMHz = sampleFreqByMHz * InterpolationNum;//经过插值后的实际采样率
            double double_N = sampleFreqByMHz / 100;//100MHz信号一个周期对应的点数

            //SendDiscardDotsCnt(chnlsarray);
            //SendFarrowDelay(chnlsarray);

            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);//打开内部源使能,这个控制字同时将通路切换成了内部源
            Thread.Sleep(1000);//10
            Stopwatch _sw = Stopwatch.StartNew();
            _sw.Start();//计时开始

            UInt32 okcnt = 0;
            _maxCaliTimeByms = 60_000;
            //开始校准
            while (_sw.ElapsedMilliseconds < _maxCaliTimeByms)
            {
                Dictionary<ChannelId, Double> phaseErrorByfs = GetChnlPhaseByfs(chnlsarray, sampleFreqByMHz, signalFreqByMHz);//获取通道的相位误差。
                System.Diagnostics.Trace.WriteLine($"phase:{String.Join(',', phaseErrorByfs.Select(o => $"{o.Key}_{o.Value.ToString("0.000")}").ToArray())}");
                HdIO.Sleep(5);
                if (CheckPhaseOK(phaseErrorByfs,15000))
                {

                    for (int j = 0; j < _SyncParams.Count(); j++)
                    {
                        _SyncParams[j] = _SyncParams[j] with { DotsCnt = _SyncParams[j].DotsCnt + 2 };
                    }
                    SendDiscardDotsCnt(chnlsarray);



                    okcnt++;
                    if (okcnt < 2)
                        continue;
                    System.Diagnostics.Trace.WriteLine("*************channel sync ok*************");
                    HdIO.Sleep(5);
                    RecoverMsg(backHdMessage);
                    SaveSyncParams();



                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0b1u << 0);//延迟丢点通道选择
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, _SyncParams[0].DotsCnt+1);
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0b1u << 3);//延迟丢点通道选择
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, _SyncParams[3].DotsCnt+1);
                    //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);

                    return;
                }
                okcnt = 0;
                Dictionary<ChannelId, UInt32> dotsNumber = Phase2DotsNumber(phaseErrorByfs, sampleFreqByMHz);//根据相位误差计算丢点数   
                bool overperiod = false;
                for (int i = 0; i < _SyncParams.Count(); i++)
                {
                    if (_SyncParams[i].DotsCnt>200)
                    {
                        overperiod = true;
                        break;
                    }
                }
                if (!CheckDotCntOk(dotsNumber)|| overperiod)
                {
                    ValidDotsCnt((UInt32)(sampleFreqByMHz / signalFreqByMHz), chnlsarray);
                    SendDiscardDotsCnt(chnlsarray);
                    continue;
                }

                ValidFarrowDelay(phaseErrorByfs, chnlsarray, 25_000);
                SendFarrowDelay(chnlsarray);

                SendDiscardDotsCnt(chnlsarray);
            }

          

            RecoverMsg(backHdMessage);
            System.Diagnostics.Trace.WriteLine("*************channel sync over time*************");
            HdIO.Sleep(5);
        }

        private void RecoverMsg(HdMessage msg)
        {
            //Hd.CurrProduct?.Acquirer_AnalogChannel?.InitDebugVarints();//配置各项数字信号处理开关。
            ConfigDebugBooleanVariantState();
            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000000, 0xa << 8);//关闭内部源使能
            //Hd.CurrDebugVarints[DebugVariants.bEnable_Inner_Source] = false;//??????
            Thread.Sleep(100);//留时间给够通道单片机响应关闭内部源命令

            //还原丢点前的各项设置。
            Hd.LocalCommands |= (long)HdCmd.DpxEnabled;
            Hd.LocalCommands |= (long)HdCmd.TmbStorageLen;//fast快采模式切换成正常状态。
            Hd.LocalCommands |= (long)HdCmd.ChnlBandwidth;//切换通路
            Hd.LocalCommands |= (long)HdCmd.TrigTypeAndParameters;
            Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;
            Hd.LocalCommands |= (long)HdCmd.ChnlGain;
            Hd.Execute(msg);
        }

        void ConfigDebugBooleanVariantState()
        {
            Hd.CurrDebugVarints.bEnableAutoFanControl = false;
            Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_ProbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;
            Hd.CurrDebugVarints.bEnable_Dsp = true;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
            Hd.CurrDebugVarints.bEnable_ChannelSync = true;
            Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = true;
            Hd.CurrDebugVarints.bEnable_bandwidth = true;
            Hd.CurrDebugVarints.bEnable_analog_signal = false;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;
        }
    }

    internal record SyncParams(UInt32 DotsCnt, Double FarrowDelayByFs);
}
