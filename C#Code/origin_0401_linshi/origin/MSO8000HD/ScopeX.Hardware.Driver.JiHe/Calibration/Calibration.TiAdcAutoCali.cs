using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Module;
using ScopeX.MathExt;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;
using Constants = ScopeX.ComModel.Constants;
using TiadcParamsKeyMap = ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X.TiadcParamsKeyMap;
namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        public Boolean CaliStatus = true;
        //cij_0810
        public Boolean CaliStatus_offset = true;

        private String _TiAdcCaliLog => $"TiAdcCaliLog{DateTime.Now.ToString("yyyyMMdd")}.txt";

        /// <summary>
        /// 打印校准日志
        /// </summary>
        /// <param name="msg"></param>
        internal void PrintTiAdcCaliLog(String msg)
        {
            if (!Directory.Exists(CaliDataPath))
            {
                Directory.CreateDirectory(CaliDataPath);
            }
            var filename = $"{CaliDataPath}{_TiAdcCaliLog}";
            File.AppendAllText(filename, $"{DateTime.Now.ToString("HH:mm:ss.ffff")}：{msg}{Environment.NewLine}");
        }

        /// <summary>
        /// 清除校准日志
        /// </summary>
        private void ClearTiAdcCaliLog()
        {
            String[] tiadccalilogpaths = Directory.GetFiles(CaliDataPath);
            foreach (var item in tiadccalilogpaths)
            {
                String filename = Path.GetFileName(item);
                if (filename.StartsWith("TiAdcCaliLog") && filename != _TiAdcCaliLog)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 删除波形日志
        /// </summary>
        private void DeleteWaveData()
        {
            var dirfile = Directory.GetFiles("./log");
            foreach (var dir in dirfile)
            {
                if (!dir.StartsWith("Log4net") && File.Exists(dir))
                {
                    File.Delete(dir);
                }
            }
        }

        /// <summary>
        /// tiAdc自动校准
        /// </summary>
        public void TiAdcAutoCali_Exec()
        {
            try
            {
                //bool result = Cali8GTiadc(new List<ChannelId> { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 });
                //if (result) { return; }
                //if (!result) { return; }



                ClearTiAdcCaliLog();
                if (AppConfig.GetIntance().AdcCalibrationModel == 0)//此模式不校准TiAdc
                {
                    return;
                }
                CaliStatus = true;
                Int32 acqcount = 0; //采集或校准次数
                CloseAdcRelatedImpacts();//关闭影响相位的条件，（触发和DSP）
                Switch100MGen();//输入100M信号
                HdIO.Sleep(50);
                SwitchSamplingMode(AdcInterleaveMode.Mode2To1);//切换到10G模式
                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;//获取当前交织信息
                AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;//获取当前采样模式
                ReSetAdcConfig(define);//初始化Adc参数 phase:32000 delay:128 //
                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                Thread.Sleep(2000);

                SetSyncSampleClock(define);//设置亚稳态区间
                DeleteWaveData();

                #region 判断采集器是否采满，用于取数据判断凭据
                if (!AbstractController_Misc.AcqIsFulled())
                {
                    //读使能复位
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    Acquisition.InitAcq(true);
                    Thread.Sleep(10);
                }
                #endregion

                #region 校准8片adc间同步

                acqcount = 0;
                var deltas = InitAdcDelta();

                #endregion


                #region 校准20G
                CaliStatus = true;
                acqcount = 0;
                SwitchSamplingMode(AdcInterleaveMode.Mode2To1);
                define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
                acqcount = 0;
                //deltas = InitAdcDelta();
                //Stopwatch ss = Stopwatch.StartNew();
                Thread.Sleep(1000);
                CalcTIofEachChannel(define, deltas, 1, acqcount == 10);
                Thread.Sleep(2000);

                SetSyncSampleClock(define, true);
                Thread.Sleep(2000);

                
                while (CaliStatus && !CalcAdcPhaseData(define, deltas, 0.85, acqcount == 50) && acqcount < 50)
                {
                    acqcount++;
                    PrintTiAdcCaliLog($"校准20G,第{acqcount}次");
                    SetSyncSampleClock(define, true);
                    Thread.Sleep(3000);
                }
                //CalcTIofEachChannel(define, deltas, 1, acqcount == 10);

                //bool result = Cali8GTiadc(new List<ChannelId> { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 });
                ////if (result) { return; }
                ////if (!result) { return; }

                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                SetSyncSampleClock(define);//设置亚稳态区间
                                           //ss.Stop();
                                           //var sss = ss.ElapsedMilliseconds.ToString();
                Thread.Sleep(1000);

                ///将C1C3-20G的数据拷贝到C1-20G和C3-20Gx //为什么
                CopyAll_20GData();
                //保存校准数据
                Helper.GetICaliData(CaliDataType.TiadcPhaseOffsetGainParams)?.SaveToFile();


                CaliStatus = false;

              //  CaliStatus_offset = true;

                //#region offset cali
                Close100MGen();
                ////保存校准数据
                SwitchSamplingMode(AdcInterleaveMode.Mode2To1);//50mv
                HdIO.Sleep(50);
                CloseAdcRelatedImpacts();//关闭影响相位的条件，（触发和DSP）
                TiAdcOffsetAndGainCali_Exec();//50mv

                //SwitchSamplingMode(AdcInterleaveMode.Mode2To1, 10);//10mv
                ////HdIO.Sleep(5000);
                ////CloseAdcRelatedImpacts();//关闭影响相位的条件，（触发和DSP）
                //HdIO.Sleep(50);
                ////SwitchSamplingMode(AdcInterleaveMode.Mode2To1);//10mv
                //TiAdcOffsetAndGainCali_Exec(true);//10mv

    //            CaliStatus_offset = false;
                //#endregion




                //打开触发
                OpenAdcRelatedImpacts();

                #endregion

            }
            catch (Exception ex)
            {
                //         Close100MGen();
                //打开触发
                OpenAdcRelatedImpacts();
                PrintTiAdcCaliLog($"TiAdc自校正异常，{ex.Message}");
                PrintTiAdcCaliLog($"堆栈信息，{ex.StackTrace}");
            }

            CaliStatus = false;

        }

        /// <summary>
        /// 初始化Adc校准参数实例
        /// </summary>
        internal List<AdcDelta> InitAdcDelta(Boolean isBoard = false)
        {
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine currdetail = analogacquiremodel.GetCurrentAcqModeInterleave()!;
            Int32 acqcount = 0;
            //Int32 model = currdetail.InterleaveMode == AdcInterleaveMode.Mode2To1 ? 1 : 0;
            Int32 model = 0;

            List<AdcDelta> adcdeltas = new List<AdcDelta>();
            foreach (var detail in currdetail.Details)
            {
                foreach (var item in detail.Value)
                {
                    foreach (var adc in item.AdcPorts)
                    {
                        if (isBoard && adc.Key == 0)
                        {
                            continue;
                        }
                        if (AppConfig.GetIntance().AdcCalibrationModel == 2 && (detail.Key == ChannelId.C3 || detail.Key == ChannelId.C4))//此模式不校准采集板2
                        {
                            continue;
                        }
                        if (AppConfig.GetIntance().AdcCalibrationModel == 3 && (detail.Key == ChannelId.C1 || detail.Key == ChannelId.C2))//此模式不校准采集板1
                        {
                            continue;
                        }
                        TiadcParamsKeyMapWithBoard itemkey = new(currdetail.Name, detail.Key, item.AcqBdNo, (UInt32)adc.Key);
                        TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
                        tiadcItem.AdcDelay_FPGA = 0;
                        AdcDelta adcdelta = new AdcDelta();
                        adcdelta.AcqBdNo = item.AcqBdNo;
                        adcdelta.ItemKey = itemkey;
                        adcdelta.ModelIndex = model;
                        adcdelta.ChanelIndex = (Int32)detail.Key;
                        adcdelta.AdcIndx = (UInt32)adc.Key;
                        adcdelta.Index = acqcount;
                        adcdelta.AdcInterleaveMode = currdetail.InterleaveMode;
                        acqcount++;
                        adcdeltas.Add(adcdelta);
                    }
                }

            }
            return adcdeltas;
        }

        /// <summary>
        /// 设置同步窗
        /// </summary>
        internal void SetSyncSampleClock(AcqModeAndInterleaveDefine define, Boolean isCheck = false)
        {
            //获取扫窗数据
            var adcscandatas = Hd.CurrProduct?.AcqBd?.ReadADC5200SyncWindowRegValue().Split(Environment.NewLine);
            if (adcscandatas.Length>8)
            {
                adcscandatas = adcscandatas.Take(8).ToArray();
            }
            Int32 fpgaindex = 0;
            Int32 adcindex = 0;
            if (adcscandatas == null)
            {
                return;
            }
            foreach (var item in adcscandatas)
            {
                if (String.IsNullOrEmpty(item))
                {
                    continue;
                }
                LogSyncSampleClock($"扫窗数据，value: {item}");
                var adcscandata = item.Split('>');
                //采集板
                String board = adcscandata[0];
                //扫窗数据
                String sacndata = adcscandata[1].Replace("_", "");

                Dictionary<Int32, Int32> steadystate = new Dictionary<Int32, Int32>();
                Int32 offset = 0;
                Int32 soffset = 0;
                for (Int32 i = sacndata.Length - 1; i > 7; i--)
                {
                    if (sacndata[i] == '0')
                    {
                        offset++;
                        if (!steadystate.TryAdd(soffset, offset))
                        {
                            steadystate[soffset] = offset;
                        }
                    }
                    else
                    {
                        offset = 0;
                        soffset = i;
                    }
                }
                var maxvalue = steadystate.Values.Max();
                var maxkeyvaluepair = steadystate.FirstOrDefault(kvp => kvp.Value == maxvalue);
                {
                    //设置采集板
                    switch (board.Replace("=", ""))
                    {
                        case "B8.Adc1": fpgaindex = 7; adcindex = 0; break;
                        case "B8.Adc2": fpgaindex = 7; adcindex = 1; break;
                        case "B7.Adc1": fpgaindex = 6; adcindex = 0; break;
                        case "B7.Adc2": fpgaindex = 6; adcindex = 1; break;
                        case "B6.Adc1": fpgaindex = 5; adcindex = 0; break;
                        case "B6.Adc2": fpgaindex = 5; adcindex = 1; break;
                        case "B5.Adc1": fpgaindex = 4; adcindex = 0; break;
                        case "B5.Adc2": fpgaindex = 4; adcindex = 1; break;
                        case "B4.Adc1": fpgaindex = 3; adcindex = 0; break;
                        case "B4.Adc2": fpgaindex = 3; adcindex = 1; break;
                        case "B3.Adc1": fpgaindex = 2; adcindex = 0; break;
                        case "B3.Adc2": fpgaindex = 2; adcindex = 1; break;
                        case "B2.Adc1": fpgaindex = 1; adcindex = 0; break;
                        case "B2.Adc2": fpgaindex = 1; adcindex = 1; break;
                        case "B1.Adc1": fpgaindex = 0; adcindex = 0; break;
                        case "B1.Adc2": fpgaindex = 0; adcindex = 1; break;
                        default:
                            break;
                    }
                }

                var postion = maxvalue / 2;
                postion += maxvalue % 2 == 1 ? 0 : 0;
                if (isCheck)
                {
                    UInt32 sampleclockdelay = define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay : TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay;
                    if (sampleclockdelay != (UInt32)((24 - maxkeyvaluepair.Key) + postion))
                    {
                        LogSyncSampleClock($"{define.InterleaveMode.ToString()}使用窗，board:{fpgaindex} adcindex:{adcindex} value: {sampleclockdelay}");
                        LogSyncSampleClock($"{define.InterleaveMode.ToString()}当前窗，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
                    }
                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
                    {
                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
                    }
                    else
                    {
                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
                    }
                }
                else
                {
                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
                    {
                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
                    }
                    else
                    {
                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
                    }
                    LogSyncSampleClock($"{define.InterleaveMode.ToString()}扫窗亚稳态区间校准，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
                }
            }

            //保存校准数据
            Helper.GetICaliData(CaliDataType.TiAdc_SyncSampleClock)?.SaveToFile();

            #region 亚稳态窗 SyncSampleClock
            Hd.CurrProduct?.AcqBd?.TiAdc_ApplayAdc_SyncSampleClock();
            #endregion
        }

        /// <summary>
        /// 记录同步窗日志
        /// </summary>
        private void LogSyncSampleClock(String log)
        {
            using (StreamWriter sw = new StreamWriter("SyncSampleClock.txt", true))
            {
                sw.WriteLine($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}】：{log}");
            }
        }

        /// <summary>
        /// 采集波形数据
        /// </summary>
        /// <param name="wavedata">波形数据</param>
        /// <param name="timeoutByMs">等待时长</param>
        /// <returns></returns>s
        private Boolean AcqWaveDataEx(out List<List<UInt16>> wavedata, Int16 timeoutByMs = 500)
        {
            wavedata = new List<List<UInt16>>();
            Dictionary<AcqDataType, Double> samplingrate = new();

            List<ReadInfo> readinfolist = new();
            WfmPkgInfo viewpkg = new(Hd.UIMessage!.Timebase!.StorageWaveDotsCnt, Hd.UIMessage!.Timebase!.TmbScale * 10, 0);
            ReadInfo viewinfo = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), viewpkg, "View");
            readinfolist.Add(viewinfo);

            Stopwatch stopwatcher = new();
            stopwatcher.Restart();
            var bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
            {
                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
            }
            stopwatcher.Restart();
            bok = false;
            //丢弃两次待通道稳定
            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
            {
                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
            }
            stopwatcher.Restart();
            bok = false;
            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
            {
                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
            }
            stopwatcher.Stop();

            var readinfo = readinfolist.FirstOrDefault(info => info.DataType == AcqDataType.AnalogChannel);
            Hd.AnalogChannel!.TakeAdcWaveform(out wavedata);
            return bok;
        }

        /// <summary>
        /// 切换模拟通道输入100MHz单频点信号
        /// </summary>
        private void Switch100MGen()
        {
            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
            //Hd.CurrDebugVarints.bEnable_OpenCrystal = true;
        }

        /// <summary>
        /// 关闭100M信号
        /// </summary>
        private void Close100MGen()
        {
            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000000, 0xa << 8);
            //Hd.CurrDebugVarints.bEnable_OpenCrystal = false;
        }

        /// <summary>
        /// 切换采样模式
        /// </summary>
        private void SwitchSamplingMode(AdcInterleaveMode adcInterleaveMode, Int32 scele = 50, Boolean status = true)
        {
            if (!status)
            {
                return;
            }
            var analog0ptions = new List<HdMessage.AnalogOptions>();
            Int32 storagewavedotscnt = 50 * 1000;
            //var coupling = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? AnaChnlCoupling.DC50 : AnaChnlCoupling.DC1M;
            var coupling =  AnaChnlCoupling.DC50;
            AnaChnlScaleIndex anaChnlScaleIndex = AnaChnlScaleIndex.Lv100m;
            switch (scele)
            {
                case 1: anaChnlScaleIndex = AnaChnlScaleIndex.Lv1m; break;
                case 2: anaChnlScaleIndex = AnaChnlScaleIndex.Lv2m; break;
                case 5: anaChnlScaleIndex = AnaChnlScaleIndex.Lv5m; break;
                case 10: anaChnlScaleIndex = AnaChnlScaleIndex.Lv10m; break;
                case 20: anaChnlScaleIndex = AnaChnlScaleIndex.Lv20m; break;
                case 50: anaChnlScaleIndex = AnaChnlScaleIndex.Lv50m; break;
                case 100: anaChnlScaleIndex = AnaChnlScaleIndex.Lv100m; break;
                case 200: anaChnlScaleIndex = AnaChnlScaleIndex.Lv200m; break;
                case 500: anaChnlScaleIndex = AnaChnlScaleIndex.Lv500m; break;
                case 1000: anaChnlScaleIndex = AnaChnlScaleIndex.Lv1; break;
                default:
                    break;
            }
            switch (adcInterleaveMode)
            {
                case AdcInterleaveMode.Mode1To1:
               
                case AdcInterleaveMode.Mode2To1:
                default:
                    List<ChannelId> needcali20gchannels = new()
                    {
                        ChannelId.C1, ChannelId.C2, ChannelId.C3,ChannelId.C4
                    };
                    foreach (var item in needcali20gchannels)
                    {
                        HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![(Int32)item] with
                        {
                            Active = true,
                            Bandwidth = 0,
                            IsInverted = false,
                            ScaleIndex = (Int32)anaChnlScaleIndex,
                            Scale = scele,
                            ScaleBymV = scele,
                            Coupling = coupling,
                            Bias = 0,
                            Position = 0,
                        };
                        analog0ptions.Add(ch);
                    }
                    storagewavedotscnt = 1000 * 1000;
                    break;
            }
            var mode = Hd.UIMessage!.Timebase! with
            {
                TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv100n].Scale,
                TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv100n,
                StorageWaveDotsCnt = storagewavedotscnt,
                NeedWaveDotsCnt = storagewavedotscnt,
                InterleaveMode = adcInterleaveMode,
                //AcqMode= AnaChnlAcqMode.Normal,
            };
            var display = Hd.UIMessage!.Display! with { IsFast = false };
            Hd.UIMessage = Hd.UIMessage! with { Analog = analog0ptions.ToArray(), Timebase = mode, Display = display };
            ConfigHardware(Hd.UIMessage, 1000);
        }

        /// <summary>
        /// 关闭影响两路ADC相位的功能
        /// </summary>
        private void CloseAdcRelatedImpacts()
        {
            //关闭数字触发
            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            //关闭触发丢点
            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = false;
            //关闭数字处理
            Hd.CurrDebugVarints.bEnable_Dsp = false;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;
            //cij_0425
            Hd.CurrDebugVarints.bEnable_IsOpenDDr = true;//true
            Hd.CurrDebugVarints.bEnable_ChannelSync = false;//true
            Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = false;//true
            Hd.CurrDebugVarints.bEnable_bandwidth = false;//true

            Hd.CurrDebugVarints.bEnable_ProbdInterpolation = false;
        }

        /// <summary>
        /// 打开影响两路ADC相位的功能
        /// </summary>
        private void OpenAdcRelatedImpacts()
        {
            //打开触发
            Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
            //打开触发丢点
            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
            //打开数字处理
            Hd.CurrDebugVarints.bEnable_Dsp = true;
            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
            Hd.CurrDebugVarints.bEnable_IsOpenDDr = true;//true
            Hd.CurrDebugVarints.bEnable_ChannelSync = true;//true
            Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = true;//true
            Hd.CurrDebugVarints.bEnable_bandwidth = true;//true

            Hd.CurrDebugVarints.bEnable_ProbdInterpolation = true;

        }

        #region 校准板内相位差

        /// <summary>
        /// 初始化Adc的配置
        /// </summary>
        internal void ReSetAdcConfig(AcqModeAndInterleaveDefine define)
        {
            if (define != null)
            {
                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
                List<TiadcParamsKeyMap> tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
                {
                    //new("C1C3-20G", (ChannelId.C1), 0),
                    //new("C1C3-20G", (ChannelId.C1), 1),
                    //new("C1C3-20G", (ChannelId.C3), 0),
                    //new("C1C3-20G", (ChannelId.C3), 1)
                };
                if (define!.InterleaveMode == AdcInterleaveMode.Mode2To1)
                {
                    tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
                    {
                        new("All-20G", (ChannelId.C1), 0),
                        new("All-20G", (ChannelId.C1), 1),
                        //new("All-20G", (ChannelId.C1), 2),
                        //new("All-20G", (ChannelId.C1), 3),
                        new("All-20G", (ChannelId.C2), 0),
                        new("All-20G", (ChannelId.C2), 1),
                        //new("All-20G", (ChannelId.C2), 2),
                        //new("All-20G", (ChannelId.C2), 3),
                        new("All-20G", (ChannelId.C3), 0),
                        new("All-20G", (ChannelId.C3), 1),
                        //new("All-20G", (ChannelId.C3), 2),
                        //new("All-20G", (ChannelId.C3), 3),
                        new("All-20G", (ChannelId.C4), 0),
                        new("All-20G", (ChannelId.C4), 1),
                        //new("All-20G", (ChannelId.C4), 2),
                        //new("All-20G", (ChannelId.C4), 3)
                    };
                }
                tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
                    {
                        new("All-20G", (ChannelId.C1), 0),
                        new("All-20G", (ChannelId.C1), 1),
                        //new("All-20G", (ChannelId.C1), 2),
                        //new("All-20G", (ChannelId.C1), 3),
                        new("All-20G", (ChannelId.C2), 0),
                        new("All-20G", (ChannelId.C2), 1),
                        //new("All-20G", (ChannelId.C2), 2),
                        //new("All-20G", (ChannelId.C2), 3),
                        new("All-20G", (ChannelId.C3), 0),
                        new("All-20G", (ChannelId.C3), 1),
                        //new("All-20G", (ChannelId.C3), 2),
                        //new("All-20G", (ChannelId.C3), 3),
                        new("All-20G", (ChannelId.C4), 0),
                        new("All-20G", (ChannelId.C4), 1),
                        //new("All-20G", (ChannelId.C4), 2),
                        //new("All-20G", (ChannelId.C4), 3)
                    };
                foreach (var item in tiadcparamskeymaps)
                {
                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(item)!.Value;//20G参数
                    //tiadcitem.Phase = 32000;
                    //tiadcitem.AdcDelay_FPGA = 128;
                    tiadcitem.Phase = 32000;
                    tiadcitem.AdcDelay_FPGA = 128;
                    if (tiadcitem.Reserved0 >= 230 || tiadcitem.Reserved0 <= 64)
                    {
                        tiadcitem.Reserved0 = 127;
                    }
                    tiadcitem.Reserved1 = 127;
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcitem);
                }
            }
        }

        /// <summary>
        /// 将C1C3_20G的配置拷贝C1-20G和C3-20G
        /// </summary>
        private void CopyAll_20GData() //从之前的C1C3改到All-20G
        {
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;
            List<TiadcParamsKeyMapWithBoard> tiadcparamskeymaps = new List<TiadcParamsKeyMapWithBoard>()
            {
              //new("All-20G", (ChannelId.C1), 0),
              new("All-20G", (ChannelId.C1),AcqBdNo.B1, 1),
              //new("All-20G", (ChannelId.C2), 0),
              new("All-20G", (ChannelId.C2),AcqBdNo.B3, 1),
             //new("All-20G", (ChannelId.C3), 0),
              new("All-20G", (ChannelId.C3),AcqBdNo.B5, 1),
             // new("All-20G", (ChannelId.C4), 0),
              new("All-20G", (ChannelId.C4),AcqBdNo.B7, 1)
            };

            //foreach (var item in tiadcparamskeymaps)
            //{
            //    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(item)!.Value;//20G参数
            //    tiadcitem.AdcDelay_FPGA += 1;
            //    ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(item, tiadcitem);
            //}
        }

        /// <summary>
        /// 校准同步
        /// </summary>
        internal Boolean CalcAdcPhaseData(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = true)
        {
            Boolean caliadcstatus = true;

            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
            for (Int32 i = 0; i < 1; i++)
            {
                adcdeltalist = GetAdcPhaseData(define, adcDeltas, delta_pS, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
            }
            #region 校准相位差
            if (!caliadcstatus)
            {
                int fpgaSampDeltaPS = 100;

                foreach (var item in adcdeltalist)
                {

                    Double ta0 = item.Delta % fpgaSampDeltaPS;
                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
                    }

                    //更新校准基数
                    item.Delta = ta0;
                    item.FpgaDelayer = -fpgadelay;
                }

                List<double> doubles = new List<double>();
                List<double> doublesdelay = new List<double>();
                foreach (var item in adcDeltas)
                {
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    doubles.Add(item.Delta);
                    doublesdelay.Add(item.FpgaDelayer);
                }

                ; int cnt = 0;
                foreach (var item in adcdeltalist)
                {
                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
                    {
                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcitem.Phase, item.Delta));
                        item.calc();
                        if (item.Delta != 0)
                        {
                            PrintTiAdcCaliLog($"粗调值：{item.CaliCase} 细调值：{item.CaliFine}");
                        }
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, false);
                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }
                    else
                    {
                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > 2 * delta_pS) != null ? false : true;
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, true, isDelay || isEnd);
                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }

                    cnt++;
                }
                //(Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).TiAdc_ApplyAdc_Phase_Offset_Gain();
                adcDeltas = adcdeltalist;
                //发送TiAdc参数
                return caliadcstatus;
            }

            #endregion 校准相位差

            return caliadcstatus;
        }

        internal Boolean CalcAdcPhaseDataDBI(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Double actualFreqByMHz,Int32 subband, Boolean isEnd = true)
        {
            Boolean caliadcstatus = true;

            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
            for (Int32 i = 0; i < 1; i++)
            {
                adcdeltalist = GetAdcPhaseDataDBI(define, adcDeltas, delta_pS, actualFreqByMHz,subband, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
            }
            #region 校准相位差
            if (!caliadcstatus)
            {
                int fpgaSampDeltaPS = 100;

                foreach (var item in adcdeltalist)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    Double ta0 = item.Delta % fpgaSampDeltaPS;
                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
                    }

                    //更新校准基数
                    item.Delta = ta0;
                    item.FpgaDelayer = -fpgadelay;
                }

                List<double> doubles = new List<double>();
                List<double> doublesdelay = new List<double>();
                foreach (var item in adcDeltas)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    doubles.Add(item.Delta);
                    doublesdelay.Add(item.FpgaDelayer);
                }

                ; int cnt = 0;
                foreach (var item in adcdeltalist)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
                    {
                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcitem.Phase, item.Delta));
                        item.calc();
                        if (item.Delta != 0)
                        {
                            PrintTiAdcCaliLog($"粗调值：{item.CaliCase} 细调值：{item.CaliFine}");
                        }
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, false);
                        (Hd.CurrProduct?.AcqBd as AcqBd_DBI13G).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }
                    else
                    {
                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > 2 * delta_pS) != null ? false : true;
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, true, isDelay || isEnd);
                        (Hd.CurrProduct?.AcqBd as AcqBd_DBI13G).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }

                    cnt++;
                }
                //(Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).TiAdc_ApplyAdc_Phase_Offset_Gain();
                adcDeltas = adcdeltalist;
                //发送TiAdc参数
                return caliadcstatus;
            }

            #endregion 校准相位差

            return caliadcstatus;
        }
        internal Boolean CalcTIofEachChannel(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = true)
        {
            Boolean caliadcstatus = true;

            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
            for (Int32 i = 0; i < 1; i++)
            {
                adcdeltalist = GetAdcPhaseData(define, adcDeltas, delta_pS, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
            }
            #region 校准相位差
            if (!caliadcstatus)
            {
                int fpgaSampDeltaPS = 100;

                foreach (var item in adcdeltalist)
                {

                    Double ta0 = item.Delta % fpgaSampDeltaPS;
                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
                    }
                    //更新校准基数

                    item.Delta = ta0;
                    item.FpgaDelayer = -fpgadelay;
                }
                List<double> doubles = new List<double>();
                List<double> doublesdelay = new List<double>();
                foreach (var item in adcDeltas)
                {
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    doubles.Add(item.Delta);
                    doublesdelay.Add(item.FpgaDelayer);
                }
                foreach (var item in adcdeltalist)
                {
                    //if (item.AcqBdNo == AcqBdNo.B0 || item.AcqBdNo == AcqBdNo.B1)
                    //{
                    //    continue;
                    //}
                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;

                    {
                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > delta_pS) != null ? false : true;
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, true, true);
                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }
                }
                adcDeltas = adcdeltalist;
                //发送TiAdc参数
                return true;
                return caliadcstatus;
            }

            #endregion 校准相位差

            return caliadcstatus;
        }

        internal Boolean CalcTIofEachChannelDBI(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Double actualFreqByMHz,Int32 subband, Boolean isEnd = true)
        {
            Boolean caliadcstatus = true;

            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
            for (Int32 i = 0; i < 1; i++)
            {
                adcdeltalist = GetAdcPhaseDataDBI(define, adcDeltas, delta_pS, actualFreqByMHz,subband, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
            }
            #region 校准相位差
            if (!caliadcstatus)
            {
                int fpgaSampDeltaPS = 100;

                foreach (var item in adcdeltalist)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    Double ta0 = item.Delta % fpgaSampDeltaPS;
                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
                    }
                    //更新校准基数

                    item.Delta = ta0;
                    item.FpgaDelayer = -fpgadelay;
                }
                List<double> doubles = new List<double>();
                List<double> doublesdelay = new List<double>();
                foreach (var item in adcDeltas)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    doubles.Add(item.Delta);
                    doublesdelay.Add(item.FpgaDelayer);
                }
                foreach (var item in adcdeltalist)
                {
                    if (((Int32)item.AcqBdNo) != subband)
                    {
                        continue;
                    }
                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;

                    {
                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > delta_pS) != null ? false : true;
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, true, true);
                        (Hd.CurrProduct?.AcqBd as AcqBd_DBI13G).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }
                }
                adcDeltas = adcdeltalist;
                //发送TiAdc参数
                return true;
                return caliadcstatus;
            }

            #endregion 校准相位差

            return caliadcstatus;
        }

        /// <summary>
        /// 获取Adc数据
        /// </summary>
        /// <param name="define"></param>
        /// <param name="adcDeltas"></param>
        /// <param name="delta_pS"></param>
        /// <param name="caliAdcStatus"></param>
        /// <returns></returns>
        private List<AdcDelta> GetAdcPhaseData(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, String nowCalitimes, out Boolean caliAdcStatus)
        {
            caliAdcStatus = true;
            Double inputsignalfreqbymhz = 100d;
            Double theorydelta_ps = 25;// define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
            #region 获取数据
            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphaseerr = new Dictionary<String, List<WaveOffsetGainPhase>>();
            Dictionary<String, List<WaveOffsetGainPhase>> tempwaveoffsetgainphases = GetPhaseData(define, nowCalitimes);
            String fintkey = String.Empty;
            if (!CaliStatus || tempwaveoffsetgainphases.Count == 0)
            {
                caliAdcStatus = false;
                return adcDeltas;
            }
            #endregion

            #region 计算三参数

            foreach (var item in tempwaveoffsetgainphases)
            {
                if (item.Key.Contains("C1"))
                    fintkey = "All-20G_C1_B1_Adc0";
                if (item.Key.Contains("C2"))
                    fintkey = "All-20G_C2_B3_Adc0";
                if (item.Key.Contains("C3"))
                    fintkey = "All-20G_C3_B5_Adc0";
                if (item.Key.Contains("C4"))
                    fintkey = "All-20G_C4_B7_Adc0";
                //else
                //    fintkey = item.Key;

                for (Int32 i = 0; i < item.Value.Count; i++)
                {
                    Int32 adcindex = fintkey == item.Key ? 0 : 1;
                    switch (item.Key)
                    {
                        case "All-20G_C1_B0_Adc0": adcindex = 1; break;
                        case "All-20G_C1_B0_Adc1": adcindex = 3; break;
                        case "All-20G_C1_B1_Adc0": adcindex = 0; break;
                        case "All-20G_C1_B1_Adc1": adcindex = 2; break;

                        case "All-20G_C2_B2_Adc0": adcindex = 1; break;
                        case "All-20G_C2_B2_Adc1": adcindex = 3; break;
                        case "All-20G_C2_B3_Adc0": adcindex = 0; break;
                        case "All-20G_C2_B3_Adc1": adcindex = 2; break;

                        case "All-20G_C3_B4_Adc0": adcindex = 1; break;
                        case "All-20G_C3_B4_Adc1": adcindex = 3; break;
                        case "All-20G_C3_B5_Adc0": adcindex = 0; break;
                        case "All-20G_C3_B5_Adc1": adcindex = 2; break;

                        case "All-20G_C4_B6_Adc0": adcindex = 1; break;
                        case "All-20G_C4_B6_Adc1": adcindex = 3; break;
                        case "All-20G_C4_B7_Adc0": adcindex = 0; break;
                        case "All-20G_C4_B7_Adc1": adcindex = 2; break;
                        default:
                            break;
                    }

                    Double phaseerror_ps = 25;
                    phaseerror_ps = ((item.Value[i].Phase - tempwaveoffsetgainphases[fintkey][i].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalfreqbymhz / (2 * Math.PI) - adcindex * theorydelta_ps;
                    if (phaseerror_ps > 1000_000 / inputsignalfreqbymhz / 2)
                        phaseerror_ps -= 1000_000 / inputsignalfreqbymhz;
                    else if (phaseerror_ps < -1000_000 / inputsignalfreqbymhz / 2)
                        phaseerror_ps += 1000_000 / inputsignalfreqbymhz;
                    
                    if (waveoffsetgainphaseerr.Keys.Contains(item.Key))
                    {
                        waveoffsetgainphaseerr[item.Key].Add(new WaveOffsetGainPhase() { Phase = phaseerror_ps, });
                    }
                    else
                    {
                        waveoffsetgainphaseerr.Add(item.Key, new List<WaveOffsetGainPhase>()
                                {
                                    new WaveOffsetGainPhase(){ Phase = phaseerror_ps, }
                                });
                    }
                }
                if (item.Key != fintkey)
                {
                    PrintTiAdcCaliLog($"{item.Key}与{fintkey}相位差：{waveoffsetgainphaseerr[item.Key].Average(p => p.Phase)}");
                }
            }

            #endregion

            #region 判断是否校准完成
            caliAdcStatus = true;
            List<double> doubles = new List<double>();
            foreach (var item in adcDeltas)
            {
                String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);
                doubles.Add(item.Delta);
            }
            foreach (var item in adcDeltas)
            {
                String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);
                //判断相位差是否小于设定的误差范围
                if (Math.Abs(item.Delta) > delta_pS)
                {
                    caliAdcStatus = false;
                    break;
                }
            }
            if (caliAdcStatus == true)
            {

            }
            #endregion
            return adcDeltas;
        }

        private List<AdcDelta> GetAdcPhaseDataDBI(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Double actualFreqByMHz,Int32 subband, String nowCalitimes, out Boolean caliAdcStatus)
        {
            caliAdcStatus = true;
            Double inputsignalfreqbymhz = actualFreqByMHz;
            Double theorydelta_ps = 50;// define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
            #region 获取数据
            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphaseerr = new Dictionary<String, List<WaveOffsetGainPhase>>();
            Dictionary<String, List<WaveOffsetGainPhase>> tempwaveoffsetgainphases = GetPhaseDataDBI(define, nowCalitimes,subband, actualFreqByMHz);
            String fintkey = String.Empty;
            if (!CaliStatus || tempwaveoffsetgainphases.Count == 0)
            {
                caliAdcStatus = false;
                return adcDeltas;
            }
            #endregion

            #region 计算三参数

            foreach (var item in tempwaveoffsetgainphases)
            {
                if (item.Key.Contains("B0"))
                    fintkey = "DBI_Disable_C1_B0_Adc0";
                if (item.Key.Contains("B1"))
                    fintkey = "DBI_Disable_C1_B1_Adc0";
                if (item.Key.Contains("B2"))
                    fintkey = "DBI_Disable_C1_B2_Adc0";
                if (item.Key.Contains("B3"))
                    fintkey = "DBI_Disable_C1_B3_Adc0";
                //else
                //    fintkey = item.Key;

                for (Int32 i = 0; i < item.Value.Count; i++)
                {
                    Int32 adcindex = fintkey == item.Key ? 0 : 1;
                    switch (item.Key)
                    {
                        case "DBI_Disable_C1_B0_Adc0": adcindex = 0; break;
                        case "DBI_Disable_C1_B0_Adc1": adcindex = 1; break;
                          
                        case "DBI_Disable_C1_B1_Adc0": adcindex = 0; break;
                        case "DBI_Disable_C1_B1_Adc1": adcindex = 1; break;
                            
                        case "DBI_Disable_C1_B2_Adc0": adcindex = 0; break;
                        case "DBI_Disable_C1_B2_Adc1": adcindex = 1; break;
                            
                        case "DBI_Disable_C1_B3_Adc0": adcindex = 0; break;
                        case "DBI_Disable_C1_B3_Adc1": adcindex = 1; break;
                        default:
                            break;
                    }

                    Double phaseerror_ps = 25;
                    phaseerror_ps = ((item.Value[i].Phase - tempwaveoffsetgainphases[fintkey][i].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalfreqbymhz / (2 * Math.PI) - adcindex * theorydelta_ps;
                    if (phaseerror_ps > 1000_000 / inputsignalfreqbymhz / 2)
                        phaseerror_ps -= 1000_000 / inputsignalfreqbymhz;
                    else if (phaseerror_ps < -1000_000 / inputsignalfreqbymhz / 2)
                        phaseerror_ps += 1000_000 / inputsignalfreqbymhz;

                    if (waveoffsetgainphaseerr.Keys.Contains(item.Key))
                    {
                        waveoffsetgainphaseerr[item.Key].Add(new WaveOffsetGainPhase() { Phase = phaseerror_ps, });
                    }
                    else
                    {
                        waveoffsetgainphaseerr.Add(item.Key, new List<WaveOffsetGainPhase>()
                                {
                                    new WaveOffsetGainPhase(){ Phase = phaseerror_ps, }
                                });
                    }
                }
                if (item.Key != fintkey)
                {
                    PrintTiAdcCaliLog($"{item.Key}与{fintkey}相位差：{waveoffsetgainphaseerr[item.Key].Average(p => p.Phase)}");
                }
            }

            #endregion

            #region 判断是否校准完成
            caliAdcStatus = true;
            List<double> doubles = new List<double>();
            foreach (var item in adcDeltas)
            {
                if (((Int32)item.AcqBdNo)==subband)
                {
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);
                    doubles.Add(item.Delta);
                }
                
            }
            int fpgaSampDeltaPS = 100;
            foreach (var item in adcDeltas)
            {
                if (((Int32)item.AcqBdNo) == subband)
                {
                    Double ta0 = item.Delta % fpgaSampDeltaPS;
                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
                    }

                    //更新校准基数
                    item.Delta = ta0;

                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    //item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);
                    //判断相位差是否小于设定的误差范围
                    if (Math.Abs(item.Delta) > delta_pS)
                    {
                        caliAdcStatus = false;
                        break;
                    }
                }
            }
            if (caliAdcStatus == true)
            {

            }
            #endregion
            return adcDeltas;
        }

        /// <summary>
        /// 设置Adc的相位和丢点
        /// </summary>
        /// <param name="tiadcParamsKeyMap"></param>
        /// <param name="data"></param>
        /// <param name="adcIndex"></param>
        /// <param name="isUpdatedelay"></param>
        /// <returns></returns>
        private void SetAcqData(TiadcParamsKeyMap tiadcParamsKeyMap, Int32[] data, UInt32 adcIndex, Boolean isUpdatedelay = true, Boolean isCali = true)
        {
            if (!isCali)
                return;
            TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(tiadcParamsKeyMap)!.Value;
            tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(tiadcParamsKeyMap)!.Value;
            if (!isUpdatedelay)
            {
                if (data[0] != 0)
                {
                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
                    if (tiadcitem.Phase - data[0] < 0)
                    {
                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
                        CaliStatus = false;
                    }
                    else if (tiadcitem.Phase - data[0] > 65535)
                    {
                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
                        CaliStatus = false;
                    }
                    else
                    {
                        tiadcitem.Phase -= data[0];
                    }
                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
                }
            }
            else
            {
                if (data[1] != 0)
                {
                    if (tiadcitem.AdcDelay_FPGA + data[1] > 255)
                    {
                        PrintTiAdcCaliLog($"超出FPGA丢点范围：当前：{tiadcitem.AdcDelay_FPGA} 校准值：{data[1]}");
                        CaliStatus = false;
                    }
                    else
                    {
                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
                        tiadcitem.AdcDelay_FPGA += data[1];
                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
                    }
                }
            }
            ProductDataTranslate_MSO8000X.SetTiadcParamsItem(tiadcParamsKeyMap, tiadcitem);
        }

        private void SetAcqDataWithBoard(TiadcParamsKeyMapWithBoard tiadcParamsKeyMap, Int32[] data, UInt32 adcIndex, Boolean isUpdatedelay = true, Boolean isCali = true)
        {
            if (!isCali)
                return;
            TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(tiadcParamsKeyMap)!.Value;
            if (!isUpdatedelay)
            {
                PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}- 粗调：{tiadcitem.Reserved0} 细调：{tiadcitem.Reserved1}");
                if (data[0] != 0)
                {
                    if (tiadcitem.Reserved0 - data[0] < 0)
                    {
                        PrintTiAdcCaliLog($"超出Adc粗调校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
                        CaliStatus = false;
                    }
                    else
                    {
                        tiadcitem.Reserved0 -= data[0];
                        if (tiadcitem.Reserved0 >= 255 || tiadcitem.Reserved0 <= 0)
                        {
                            tiadcitem.Reserved0 = 128;
                        }
                        tiadcitem.Reserved1 = 127;
                    }
                }
                if (data[1] != 0)
                {
                    if (tiadcitem.Reserved1 - data[1] < 0)
                    {
                        PrintTiAdcCaliLog($"超出Adc细调校准范围：当前：{tiadcitem.Phase} 校准值：{data[1]}");
                        CaliStatus = false;
                    }
                    else
                    {
                        tiadcitem.Reserved1 -= data[1];
                        if (tiadcitem.Reserved1 >= 255 && tiadcitem.Reserved1 <= 0)
                        {
                            tiadcitem.Reserved1 = 128;
                        }
                    }
                }
                PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}- 粗调：{tiadcitem.Reserved0} 细调：{tiadcitem.Reserved1}");
            }
            else
            {
                if (data[2] != 0)
                {
                    if (tiadcitem.AdcDelay_FPGA + data[2] > 255)
                    {
                        PrintTiAdcCaliLog($"超出FPGA丢点范围：当前：{tiadcitem.AdcDelay_FPGA} 校准值：{data[2]}");
                        CaliStatus = false;
                    }
                    else
                    {
                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
                        tiadcitem.AdcDelay_FPGA += data[2];
                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
                    }
                }
            }
            //if (!isUpdatedelay)
            //{
            //    if (data[0] != 0)
            //    {
            //        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
            //        if (tiadcitem.Phase - data[0] < 0)
            //        {
            //            PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
            //            CaliStatus = false;
            //        }
            //        else if (tiadcitem.Phase - data[0] > 65535)
            //        {
            //            PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
            //            CaliStatus = false;
            //        }
            //        else
            //        {
            //            tiadcitem.Phase -= data[0];
            //        }
            //        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
            //    }
            //}
            //else
            //{
            //    if (data[1] != 0)
            //    {
            //        if (tiadcitem.AdcDelay_FPGA + data[1] > 255)
            //        {
            //            PrintTiAdcCaliLog($"超出FPGA丢点范围：当前：{tiadcitem.AdcDelay_FPGA} 校准值：{data[1]}");
            //            CaliStatus = false;
            //        }
            //        else
            //        {
            //            PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
            //            tiadcitem.AdcDelay_FPGA += data[1];
            //            PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
            //        }
            //    }
            //}
            ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(tiadcParamsKeyMap, tiadcitem);
        }


        #endregion


        #region 校准通道三到通道一的相位差

        /// <summary>
        /// 将10G的数据拷贝20G //为什么？
        /// </summary>
        private void Copy10GTo20GData()
        {
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;
            List<TiadcParamsKeyMap> tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
            {
              new("C1C3-20G", (ChannelId.C1), 1),
              new("C1C3-20G", (ChannelId.C3), 1),
              new("C1-20G", (ChannelId.C1), 1),
              new("C3-20G", (ChannelId.C3), 1)
            };

            foreach (var item in tiadcparamskeymaps)
            {
                TiadcParamsKeyMap itemkey = new("All-20G", item.chnlId, item.adcId);
                TiadcPhaseOffsetGainItem_Base tiadcfineitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemkey)!.Value;//10G参数
                TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(item)!.Value;//20G参数
                tiadcItem.Phase = tiadcfineitem.Phase;
                tiadcItem.AdcDelay_FPGA = tiadcfineitem.AdcDelay_FPGA;
                ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcItem);
            }
        }

        /// <summary>
        /// 获取Adc数据
        /// </summary>
        /// <param name="define">交织模式</param>
        /// <param name="adcDeltas">adc校准数据</param>
        /// <param name="delta_pS">误差范围</param>
        /// <param name="caliAdcStatus">校准Adc状态</param>
        /// <returns></returns>
        private List<AdcDelta> GetAdcInterBoardSynchronizationPhaseDataC3ToC1(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, String nowCaliTimes, out Boolean caliAdcStatus)
        {
            caliAdcStatus = false;
            Double inputsignalfreqbymhz = 100d;
            //Double theorydelta_ps = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
            Double theorydelta_ps = 0;
            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphaseerr = new Dictionary<String, List<WaveOffsetGainPhase>>();
            Dictionary<String, List<WaveOffsetGainPhase>> tempwaveoffsetgainphases = GetPhaseData(define, nowCaliTimes);
            if (!CaliStatus || tempwaveoffsetgainphases.Count == 0)
            {
                caliAdcStatus = false;
                return adcDeltas;
            }
            String fintkey = String.Empty;
            #region 计算三参数
            //fintkey = "All-20G_C1_B0_Adc0";
            fintkey = "All-20G_C1_B1_Adc0";
            fintkey = "All-20G_C2_B3_Adc0";
            foreach (var item in tempwaveoffsetgainphases)
            {
                //theorydelta_ps = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
                theorydelta_ps = 0;


                for (Int32 i = 0; i < item.Value.Count; i++)
                {
                    Int32 adcIndex = fintkey == item.Key ? 0 : 1;
                    Double PhaseError_pS = 0;
                    PhaseError_pS = ((item.Value[i].Phase - tempwaveoffsetgainphases[fintkey][i].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalfreqbymhz / (2 * Math.PI) + theorydelta_ps;
                    if (PhaseError_pS > 1000_000 / inputsignalfreqbymhz / 2)
                        PhaseError_pS -= 1000_000 / inputsignalfreqbymhz;
                    else if (PhaseError_pS < -1000_000 / inputsignalfreqbymhz / 2)
                        PhaseError_pS += 1000_000 / inputsignalfreqbymhz;
                    if (waveoffsetgainphaseerr.Keys.Contains(item.Key))
                    {
                        waveoffsetgainphaseerr[item.Key].Add(new WaveOffsetGainPhase() { Phase = PhaseError_pS, });
                    }
                    else
                    {
                        waveoffsetgainphaseerr.Add(item.Key, new List<WaveOffsetGainPhase>()
                        {
                             new WaveOffsetGainPhase(){ Phase = PhaseError_pS, }
                        });
                    }
                }
                if (item.Key != fintkey)
                {
                    PrintTiAdcCaliLog($"{item.Key}与{fintkey}相位差：{waveoffsetgainphaseerr[item.Key].Average(p => p.Phase)}");
                }
            }

            #endregion

            #region 判断是否校准完成
            caliAdcStatus = true;
            foreach (var item in adcDeltas)
            {
                //if (item.AdcIndx == 1)
                {
                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
                    item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);

                    Double TA0 = Math.Abs(item.Delta) % 100;
                    if (Math.Abs(item.Delta) > delta_pS)
                    {
                        caliAdcStatus = false;
                    }
                }
            }
            #endregion

            return adcDeltas;
        }

        /// <summary>
        /// 校准同步
        /// </summary>
        private Boolean CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = false)



        {
            Boolean caliadcstatus = false;
            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
            for (Int32 i = 0; i < 1; i++)
            {
                adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
            }
            #region 校准相位差
            if (!caliadcstatus)
            {
                foreach (var item in adcdeltalist)
                {
                    Double ta0 = item.Delta % 100;
                    Int32 fpgadelay = (Int32)item.Delta / 100;
                    if (Math.Abs(ta0) >= 50 && Math.Abs(ta0) <= 100)
                    {
                        fpgadelay += ta0 > 0 ? 1 : -1;
                        ta0 = ta0 > 0 ? ta0 - 100 : ta0 + 100;

                    }
                    item.Delta = ta0;
                    item.FpgaDelayer = -fpgadelay;

                }
                foreach (var item in adcdeltalist)
                {
                    //if (item.ItemKey.chnlId != ChannelId.C2)
                    //{
                    //    continue;
                    //}
                    //if (item.AcqBdNo == AcqBdNo.B0 && item.AdcIndx == 0)
                    //{
                    //    continue;

                    //}
                    //if (item.AcqBdNo == AcqBdNo.B0 && item.AdcIndx == 1)
                    //{
                    //    continue;

                    //}
                    //if (item.AcqBdNo == AcqBdNo.B1 && item.AdcIndx == 1)
                    //{
                    //    continue;

                    //}
                    //if (item.AcqBdNo == AcqBdNo.B1 && item.AdcIndx == 0)
                    //{
                    //    continue;

                    //}
                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;

                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
                    {
                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcItem.Phase, item.Delta));
                        item.calc();
                        item.calc();
                        if (item.Delta != 0)
                        {
                            PrintTiAdcCaliLog($"粗调值：{item.CaliCase} 细调值：{item.CaliFine}");
                        }
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, false, true);
                        //if (item.Delta != 0)
                        //{
                        //    PrintTiAdcCaliLog($"斜率：{item.rate} 基数：{item.Delta}");
                        //}
                        //SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, false, true);
                        //SetAcqData(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
                        //AcqBdNo bn = keyValuePairs[(item.AcqBdNo, (Int32)item.AdcIndx)].Item1;
                        //Int32 adcindx = keyValuePairs[(item.AcqBdNo, (Int32)item.AdcIndx)].Item2;

                        //(Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, bn, adcindx, item.TiadcItem);
                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);

                        //              var adcdeltalist1 = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);

                    }
                    else
                    {
                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliCase, item.CaliFine, item.FpgaDelayer }, item.AdcIndx, true, true);
                        //AcqBdNo bn = keyValuePairs[(item.AcqBdNo, (Int32)item.AdcIndx)].Item1;
                        //Int32 adcindx = keyValuePairs[(item.AcqBdNo, (Int32)item.AdcIndx)].Item2;
                        //(Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, bn, adcindx, item.TiadcItem);
                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
                    }
                    //tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
                    //Hd.CurrProduct?.AcqBd.TiAdc_ApplyAdc_Phase_Offset_Gain();
                }
                //       adcDeltas = adcdeltalist;
                //       adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);

                return caliadcstatus;
            }

            //    adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);

            #endregion 校准相位差
            return caliadcstatus;
        }

        #endregion
        Dictionary<(AcqBdNo, Int32), (AcqBdNo, Int32)> keyValuePairs = new Dictionary<(AcqBdNo, int), (AcqBdNo, int)> {
            {(AcqBdNo.B0, 0), (AcqBdNo.B1, 0) },
            {(AcqBdNo.B0, 1), (AcqBdNo.B0, 0) },
            {(AcqBdNo.B1, 0), (AcqBdNo.B1, 1) },
            {(AcqBdNo.B1, 1), (AcqBdNo.B0, 1) },
            {(AcqBdNo.B2, 0), (AcqBdNo.B3, 0) },
            {(AcqBdNo.B2, 1), (AcqBdNo.B2, 0) },
            {(AcqBdNo.B3, 0), (AcqBdNo.B3, 1) },
            {(AcqBdNo.B3, 1), (AcqBdNo.B2, 1) },
            {(AcqBdNo.B4, 0), (AcqBdNo.B5, 0) },
            {(AcqBdNo.B4, 1), (AcqBdNo.B4, 0) },
            {(AcqBdNo.B5, 0), (AcqBdNo.B5, 1) },
            {(AcqBdNo.B5, 1), (AcqBdNo.B4, 1) },
            {(AcqBdNo.B6, 0), (AcqBdNo.B7, 0) },
            {(AcqBdNo.B6, 1), (AcqBdNo.B6, 0) },
            {(AcqBdNo.B7, 0), (AcqBdNo.B7, 1) },
            {(AcqBdNo.B7, 1), (AcqBdNo.B6, 1) },

        };

        #region 获取数据

        /// <summary>
        /// 获取相位数据
        /// </summary>
        /// <param name="define"></param>
        /// <returns></returns>
        private Dictionary<String, List<WaveOffsetGainPhase>> GetPhaseData(AcqModeAndInterleaveDefine define, String nowCaliTimes)
        {
            Int32 sgfrequencybyhz = 100;//单位MHz
            Double samplingrate = 10_000;//采样间隔，单位Sps
            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphases = new Dictionary<String, List<WaveOffsetGainPhase>>();
            String fintkey = String.Empty;
            for (Int32 i = 0; i < 3; i++)
            {
                var adcdata = new List<List<ushort>>();
                AcqWaveDataEx(out adcdata, 100);
                if (adcdata[0].ToArray().Length <= 4)
                {
                    PrintTiAdcCaliLog($"取Adc数据异常！");
                    CaliStatus = false;
                    return waveoffsetgainphases;
                }
                #region 调试代码
                using (StreamWriter sw = new StreamWriter($"./log/C1_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[0].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C2_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[1].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C3_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[2].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C4_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[3].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C5_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[4].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C6_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[5].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C7_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[6].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C8_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[7].ToArray()));
                }
                #endregion
                foreach (var item in define.Details)
                {
                    foreach (var eachinitem in item.Value)
                    {
                        foreach (var adc in eachinitem.AdcPorts)
                        {
                            fintkey = define.Name + "_" + item.Key + "_" + eachinitem.AcqBdNo + "_Adc" + adc.Key;
                            //当前两个采集板，固定通道1、2是采集板1，3、4是采集板2
                            //Int32 boardid = (item.Key) switch
                            //{
                            //    ChannelId.C1 => 0,
                            //    ChannelId.C2 => 0,
                            //    _ => 1,
                            //};

                            Int32 boardid = (item.Key) switch
                            {
                                ChannelId.C1 => 0,
                                ChannelId.C2 => 1,
                                ChannelId.C3 => 2,
                                ChannelId.C4 => 3
                            };
                            Int32 index = boardid * Constants.ADC_NUM * item.Value.Length + adc.Key;
                            switch (fintkey)
                            {
                                case "All-20G_C1_B0_Adc0": index = 0; break;
                                case "All-20G_C1_B0_Adc1": index = 1; break;
                                case "All-20G_C1_B1_Adc0": index = 2; break;
                                case "All-20G_C1_B1_Adc1": index = 3; break;

                                case "All-20G_C2_B2_Adc0": index = 4; break;
                                case "All-20G_C2_B2_Adc1": index = 5; break;
                                case "All-20G_C2_B3_Adc0": index = 6; break;
                                case "All-20G_C2_B3_Adc1": index = 7; break;

                                case "All-20G_C3_B4_Adc0": index = 8; break;
                                case "All-20G_C3_B4_Adc1": index = 9; break;
                                case "All-20G_C3_B5_Adc0": index = 10; break;
                                case "All-20G_C3_B5_Adc1": index = 11; break;

                                case "All-20G_C4_B6_Adc0": index = 12; break;
                                case "All-20G_C4_B6_Adc1": index = 13; break;
                                case "All-20G_C4_B7_Adc0": index = 14; break;
                                case "All-20G_C4_B7_Adc1": index = 15; break;
                                default:
                                    break;
                            }

                            var data = adcdata[index];
                            if (data != null && data.Count > 0)
                            {
                                Int32 startindex = data.Count / 10;
                                UInt16[] datas = data.GetRange(startindex, data.Count - startindex * 2).ToArray();
                                if (waveoffsetgainphases.Keys.Contains(fintkey))
                                {
                                    waveoffsetgainphases[fintkey].Add(SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz));
                                }
                                else
                                {
                                    waveoffsetgainphases.Add(fintkey, new List<WaveOffsetGainPhase>() { SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz) });
                                }
                            }
                        }
                    }

                }
            }

            return waveoffsetgainphases;
        }
        private Dictionary<String, List<WaveOffsetGainPhase>> GetPhaseDataDBI(AcqModeAndInterleaveDefine define, String nowCaliTimes,Int32 subband,double actualFreqByMHz)
        {
            Int32 sgfrequencybyhz = (Int32)actualFreqByMHz;//单位MHz
            Double samplingrate = 10_000;//采样间隔，单位Sps
            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphases = new Dictionary<String, List<WaveOffsetGainPhase>>();
            String fintkey = String.Empty;
            for (Int32 i = 0; i < 3; i++)
            {
                var adcdata = new List<List<ushort>>();
                AcqWaveDataEx(out adcdata, 100);
                if (adcdata[0].ToArray().Length <= 4)
                {
                    PrintTiAdcCaliLog($"取Adc数据异常！");
                    CaliStatus = false;
                    return waveoffsetgainphases;
                }
                #region 调试代码
                using (StreamWriter sw = new StreamWriter($"./log/C1_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[0].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C2_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[1].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C3_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[2].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C4_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[3].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C5_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[4].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C6_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[5].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C7_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[6].ToArray()));
                }
                using (StreamWriter sw = new StreamWriter($"./log/C8_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
                {
                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[7].ToArray()));
                }
                #endregion
                foreach (var item in define.Details)
                {
                
                    foreach (var eachinitem in item.Value)
                    {
                        if (((Int32)eachinitem.AcqBdNo) == subband)
                            foreach (var adc in eachinitem.AdcPorts)
                            {
                                fintkey = define.Name + "_" + item.Key + "_" + eachinitem.AcqBdNo + "_Adc" + adc.Key;
                               
                                Int32 boardid = (item.Key) switch
                                {
                                    ChannelId.C1 => 0,
                                    ChannelId.C2 => 1,
                                    ChannelId.C3 => 2,
                                    ChannelId.C4 => 3
                                };
                                Int32 index = boardid * Constants.ADC_NUM * item.Value.Length + adc.Key;
                                switch (fintkey)
                                {
                                    case "DBI_Disable_C1_B0_Adc0": index = 0; break;
                                    case "DBI_Disable_C1_B0_Adc1": index = 1; break;
                                    case "DBI_Disable_C1_B1_Adc0": index = 2; break;
                                    case "DBI_Disable_C1_B1_Adc1": index = 3; break;

                                    case "DBI_Disable_C1_B2_Adc0": index = 4; break;
                                    case "DBI_Disable_C1_B2_Adc1": index = 5; break;
                                    case "DBI_Disable_C1_B3_Adc0": index = 6; break;
                                    case "DBI_Disable_C1_B3_Adc1": index = 7; break;
                                    default:
                                        break;
                                }

                                var data = adcdata[index];
                                if (data != null && data.Count > 0)
                                {
                                    Int32 startindex = data.Count / 10;
                                    UInt16[] datas = data.GetRange(startindex, data.Count - startindex * 2).ToArray();
                                    if (waveoffsetgainphases.Keys.Contains(fintkey))
                                    {
                                        waveoffsetgainphases[fintkey].Add(SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz));
                                    }
                                    else
                                    {
                                        waveoffsetgainphases.Add(fintkey, new List<WaveOffsetGainPhase>() { SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz) });
                                    }
                                }
                            }
                    }

                }
            }

            return waveoffsetgainphases;
        }
        #endregion

        internal class AdcDelta
        {
            //20G 0:B0 1:A1 2:B0 3:A1
            //10G 0:A1 1:A0 2:A0 3:A1
            public TiadcPhaseOffsetGainItem_Base TiadcItem
            {
                get
                {
                    return ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(ItemKey!)!.Value;
                }
            }
            public ProductDataTranslate_MSO8000X.TiadcParamsKeyMapWithBoard ItemKey;
            public AcqBdNo AcqBdNo;
            public Int32 ChanelIndex;
            public Int32 ModelIndex;
            public UInt32 AdcIndx;
            public Int32 Index;//adc索引
            public Double Delta;//相位差
            public Int32 FpgaDelayer;//丢点
            public Int32 CaliDelta;
            public Int32 CaliCase;//粗调
            public Int32 CaliFine;//细调
            public AdcInterleaveMode AdcInterleaveMode;
            public List<Double> CaliCaseLogs = new List<double>();
            // 数据、偏差
            private List<KeyValuePair<Int32, Double>> _RegErrPairs = new List<KeyValuePair<Int32, Double>>();

            public void AddVaule(KeyValuePair<Int32, Double> infoValue)
            {
                _RegErrPairs.Add(infoValue);
            }

            /// <summary>
            /// 斜率
            /// </summary>
            public Double rate = 300;

            public void calc()
            {
                Boolean isrepeat = false;
                if (CaliCaseLogs.Count >= 3)
                {
                    isrepeat = CaliCaseLogs[0] == CaliCaseLogs[2];//当第一个值和第三个值一致，说明陷入粗调死循环，需要跳出粗调，采用细调校准
                    CaliCaseLogs.RemoveAt(0);
                }
                if (!isrepeat && Math.Abs(Delta) >= 1)//粗调步进为1.13ps 此处设置0.6ps是为尽可能用粗调，以保证细调的调整步进尽量小
                {
                    CaliCase = (Int32)Math.Round((Delta / 1.13), 0, MidpointRounding.ToNegativeInfinity);//向上取整
                    CaliCase = CaliCase > 4 ? 4 : CaliCase;
                    CaliCase = CaliCase < -3 ? -3 : CaliCase;
                    //CaliCase = - CaliCase;
                    CaliFine = 0;//粗调时，将细调设置为0

                }
                else
                {
                    CaliCase = 0;//细调时，粗调值设置为0
                    CaliFine = (Int32)(Delta * 1000 / 19);//细调步进为19fs 2595时钟抖动在130fs Adc抖动在65ps 理论组合误差为145fs 实际测试在400fs左右，所以我们校准指标在400fs以内
                }
                //if (_RegErrPairs != null && _RegErrPairs.Count > 1)
                //{
                //    var param = _RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value;
                //    if (param == 0)
                //    {
                //        if (Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) >= 2 && Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) < 3)
                //        {
                //            rate = 10;
                //        }
                //        else if (Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) < 2)
                //        {
                //            rate = 5;
                //        }
                //        else
                //        {
                //            rate = 100;
                //        }
                //    }
                //    else
                //    {
                //        Double rateparam = (_RegErrPairs[_RegErrPairs.Count - 1].Key - _RegErrPairs[_RegErrPairs.Count - 2].Key) / (_RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value);
                //        if (rateparam == rate)
                //        {
                //            rate /= 2;
                //        }
                //        else
                //        {
                //            rate = rateparam;
                //        }
                //    }
                //}
                //else
                //{
                //    rate = ChanelIndex == 3 || ChanelIndex == 2 ? 100 : 300;
                //    rate = 100;
                //    //if (Math.Abs(Delta) <10)
                //    //{
                //    //    rate = 20;
                //    //}
                //    if (Math.Abs(Delta) < 5)
                //    {
                //        rate = 10;
                //    }
                //    if (Math.Abs(Delta) < 2.5)
                //    {
                //        rate = 5;
                //    }
                //}
                //rate = Math.Abs(rate);
                //rate = rate > 200 ? 200 : rate;
                //rate = rate <= 3 ? 3 : rate;
                //CaliDelta = (Int32)(rate * Delta);
                //CaliDelta = CaliDelta >= 1000 ? 1000 : CaliDelta;
            }
        }


        private Boolean Cali8GTiadc(List<ChannelId> chnlIdTable)
        {
            Int32 maxCaliTimeByms = 60_000;//最大自校准用时，60s
            Int32 paramSendTimesByms = 1500;//参数发送所需时间，2s

            Double sampFreqByMHz = 40000.0;//40G采样率
            Double InputSignalFreqByMhz = 100.0;//输入信号（校准源）的频率MHz

            Int32 minPhaseCtrlWord = 512;//最小相位控制字
            Int32 maxPhaseCtrlWord = 0xffff;//最大相位控制字
            Int32 minGainCtrlWord = 512;//最小增益控制字
            Int32 maxGainCtrlWord = 0xffff;//最大增益控制字

            Double theoryGainStep = 5000;//理论增益
            Double theoryPhaseStep = 20;//理论1个控制字多少fs

            Double phaseLimitByfs = 1000;//相位误差容许范围 fs
            Double gainLimit = 0.006;//增益误差容许范围

            //参与拼合的tiadc与tool工具中三参数对应的采集板、adcid、adccore的编号index
            Dictionary<ChannelId, Dictionary<int, (int AcqBdNo, int AdcId, int CoreId)>> adcId2Index = new()
            {
                [ChannelId.C1] = new()
                {
                    {0 ,( 1 , 0 , 0 )},
                    {1 ,( 0, 0 , 0 )},
                    {2 ,( 1 , 1 , 0 )},
                    {3 ,( 0, 1 , 0 )},
                },
                [ChannelId.C2] = new()
                {
                    {0 ,( 3 , 0 , 0 )},
                    {1 ,( 2, 0 , 0 )},
                    {2 ,( 3 , 1 , 0 )},
                    {3 ,( 2, 1 , 0 )},
                },
                [ChannelId.C3] = new()
                {
                    {0 ,( 5 , 0 , 0 )},
                    {1 ,( 4 , 0 , 0 )},
                    {2 ,( 5 , 1 , 0 )},
                    {3 ,( 4 , 1 , 0 )},
                },
                [ChannelId.C4] = new()
                {
                    {0 ,( 7 , 0 , 0 )},
                    {1 ,( 6, 0 , 0 )},
                    {2 ,( 7 , 1 , 0 )},
                    {3 ,( 6, 1 , 0 )},
                },
            };
            //CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(InputSignalFreqByMhz, chnlId);//调用控制内部源的方法。
            Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();//调整ADC的增益和相位
            AbstractController_AnalogChannel.CtrlOffset();//调整通道偏置
            //以上属于校准前的准备，调整硬件
            Thread.Sleep(paramSendTimesByms);//等待控制命令发送完。

            Int32 baseAdcId = 0;
            List<Int32> adcIdList = new() { baseAdcId, 1, 2, 3 };

            Dictionary<ChannelId, Dictionary<Int32, Boolean>> gainOK = new();//增益校准完成标志 字典
            Dictionary<ChannelId, Dictionary<Int32, Boolean>> phaseOK = new();//相位校准完成标志 字典

            Dictionary<ChannelId, Dictionary<Int32, Int32>> phaseBaseCtrlWord = new();//相位控制字
            Dictionary<ChannelId, Dictionary<Int32, Int32>> phaseDeltaCtrlWord = new();//相位delta控制字

            Dictionary<ChannelId, Dictionary<Int32, Int32>> maxPhaseDelta = new();//最大delta相位控制
            Dictionary<ChannelId, Dictionary<Int32, Int32>> minPhaseDelta = new();//最小delta相位控制

            Dictionary<ChannelId, Dictionary<Int32, List<CtrlWordAndPhase>>> oldphasectrlwords = new();

            Dictionary<ChannelId, Dictionary<Int32, CtrlWordAndPhase>> negative = new();
            Dictionary<ChannelId, Dictionary<Int32, CtrlWordAndPhase>> positive = new();

            foreach (ChannelId chnlid in chnlIdTable)
            {
                gainOK[chnlid] = new();
                phaseOK[chnlid] = new();
                phaseBaseCtrlWord[chnlid] = new();
                phaseDeltaCtrlWord[chnlid] = new();
                maxPhaseDelta[chnlid] = new();
                minPhaseDelta[chnlid] = new();

                oldphasectrlwords[chnlid] = new();
                negative[chnlid] = new();
                positive[chnlid] = new();

                foreach (Int32 adcId in adcIdList)
                {
                    //TiadcPhaseOffsetGainItem_Base tmpItem = TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][adcId].AcqBdNo, adcId2Index[chnlid][adcId].AdcId, adcId2Index[chnlid][adcId].CoreId];
                    TiadcPhaseOffsetGainItem_Base tmpItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)adcId))!.Value;

                    if (tmpItem.Gain < minGainCtrlWord)
                        tmpItem.Gain = minGainCtrlWord;
                    //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][adcId].AcqBdNo, adcId2Index[chnlid][adcId].AdcId, adcId2Index[chnlid][adcId].CoreId] = tmpItem;
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)adcId), tmpItem);
                    // 从三维数组里取元素，[int channelIndex, int adcIndex, int coreIndex]，元素是 AdcPhaseOffsetGainItem：三个误差参数
                    // 小心数组越界//访问_AdcCaliBuffer三维数组具体的某个元素，该三维数组是AdcPhaseOffsetGainItem类型的。
                    //AdcPhaseOffsetGainItem tmpItem = TiAdc_PhaseOffsetGain.Default[0, 0, 0];
                    phaseBaseCtrlWord[chnlid][adcId] = tmpItem.Phase;
                    phaseDeltaCtrlWord[chnlid][adcId] = 0;
                }

                foreach (Int32 adcId in adcIdList)
                {
                    //有点儿绕
                    //最大delta相位控制字：最大相位控制字 - 基准相位控制字 + 基准adc控制字 - 最小相位控制字
                    maxPhaseDelta[chnlid][adcId] = (maxPhaseCtrlWord - phaseBaseCtrlWord[chnlid][adcId]) + (phaseBaseCtrlWord[chnlid][baseAdcId] - minPhaseCtrlWord);
                    minPhaseDelta[chnlid][adcId] = (minPhaseCtrlWord - phaseBaseCtrlWord[chnlid][adcId]) + (phaseBaseCtrlWord[chnlid][baseAdcId] - maxPhaseCtrlWord);
                }
            }

            //以上是准备工作，下面开始校准
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            while (sw.ElapsedMilliseconds < maxCaliTimeByms)
            {
                //得到ADC的三个误差，每个键代表当前通道的ADC编号，curTiadcError[0].phaseByfs代表当前通道的第一个ADC的相位误差。
                Dictionary<ChannelId, Dictionary<Int32, PhaseGainError>> curTiadcError = GetAllAdcAverageError(chnlIdTable, sampFreqByMHz, InputSignalFreqByMhz);
                Dictionary<ChannelId, TiadcPhaseOffsetGainItem_Base[]> adcItem = new();
                foreach (ChannelId chnlid in curTiadcError.Keys)
                {
                    adcItem[chnlid] = new TiadcPhaseOffsetGainItem_Base[]
                    {
                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][0].AcqBdNo, adcId2Index[chnlid][0].AdcId, adcId2Index[chnlid][0].CoreId],
                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][1].AcqBdNo, adcId2Index[chnlid][1].AdcId, adcId2Index[chnlid][1].CoreId],
                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][2].AcqBdNo, adcId2Index[chnlid][2].AdcId, adcId2Index[chnlid][2].CoreId],
                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][3].AcqBdNo, adcId2Index[chnlid][3].AdcId, adcId2Index[chnlid][3].CoreId],
                        ProductDataTranslate_MSO8000X.GetTiadcParamsItem( new("All-20G", (chnlid), 0))!.Value,
                        ProductDataTranslate_MSO8000X.GetTiadcParamsItem( new("All-20G", (chnlid), 1))!.Value,
                        ProductDataTranslate_MSO8000X.GetTiadcParamsItem( new("All-20G", (chnlid), 2))!.Value,
                        ProductDataTranslate_MSO8000X.GetTiadcParamsItem( new("All-20G", (chnlid), 3))!.Value,
                    };

                    foreach (Int32 adcId in curTiadcError[chnlid].Keys)
                    {
                        //校正相位
                        if ((!phaseOK.ContainsKey(chnlid)) || (!phaseOK[chnlid].ContainsKey(adcId)) || (!phaseOK[chnlid][adcId]))
                        {
                            if (Math.Abs(curTiadcError[chnlid][adcId].phaseByfs) < phaseLimitByfs)
                            {
                                phaseOK[chnlid][adcId] = true;
                            }//小于限制范围，该adc相位校正完成。
                            else//若相位校正未完成，则执行下列代码。
                            {
                                if (!oldphasectrlwords[chnlid].ContainsKey(adcId))
                                    oldphasectrlwords[chnlid][adcId] = new();

                                CtrlWordAndPhase curphase = new CtrlWordAndPhase(phaseDeltaCtrlWord[chnlid][adcId], curTiadcError[chnlid][adcId].phaseByfs);

                                Trace.WriteLine($"[Cali8GTiadc]********************************{chnlid}_{adcId} phaseCtrlWord：{phaseDeltaCtrlWord[chnlid][adcId]} curPhaseError:{curTiadcError[chnlid][adcId].phaseByfs.ToString("0.000")}fs******************");
                                HdIO.Sleep(5);
                                oldphasectrlwords[chnlid][adcId].Add(curphase);

                                if (curphase.PhaseError < 0)
                                    negative[chnlid][adcId] = curphase;
                                else
                                    positive[chnlid][adcId] = curphase;

                                CtrlWordAndPhase? nega = negative[chnlid].ContainsKey(adcId) ? negative[chnlid][adcId] : null;
                                CtrlWordAndPhase? posi = positive[chnlid].ContainsKey(adcId) ? positive[chnlid][adcId] : null;

                                phaseDeltaCtrlWord[chnlid][adcId] = GetPhaseDeltaCtrlWords(nega, posi, theoryPhaseStep, curphase);//方法写在下面

                                if (phaseDeltaCtrlWord[chnlid][adcId] < minPhaseDelta[chnlid][adcId] || phaseDeltaCtrlWord[chnlid][adcId] > maxPhaseDelta[chnlid][adcId])
                                {
                                    continue;
                                }

                                Int32 ctrlwordTmp = phaseBaseCtrlWord[chnlid][adcId] + phaseDeltaCtrlWord[chnlid][adcId];

                                if (ctrlwordTmp <= maxPhaseCtrlWord && ctrlwordTmp >= 0)
                                {
                                    adcItem[chnlid][adcId].Phase = ctrlwordTmp;
                                    adcItem[chnlid][baseAdcId].Phase = phaseBaseCtrlWord[chnlid][baseAdcId];
                                }
                                else if (ctrlwordTmp < 0)
                                {
                                    adcItem[chnlid][adcId].Phase = minPhaseCtrlWord;
                                    adcItem[chnlid][baseAdcId].Phase = phaseBaseCtrlWord[chnlid][baseAdcId] - (phaseDeltaCtrlWord[chnlid][adcId] + phaseBaseCtrlWord[chnlid][adcId] - minPhaseCtrlWord);
                                }
                                else
                                {
                                    adcItem[chnlid][adcId].Phase = maxPhaseCtrlWord;
                                    adcItem[chnlid][baseAdcId].Phase = phaseBaseCtrlWord[chnlid][baseAdcId] - (maxPhaseCtrlWord - phaseDeltaCtrlWord[chnlid][adcId]);
                                }
                            }
                        }
                        //校正增益
                        if ((!gainOK.ContainsKey(chnlid)) || (!gainOK[chnlid].ContainsKey(adcId)) || (!gainOK[chnlid][adcId]))
                        {
                            if (Math.Abs(curTiadcError[chnlid][adcId].gain) < gainLimit)//小于限制范围，则该ADC增益校正完成。
                            {
                                gainOK[chnlid][adcId] = true;
                            }
                            else
                            {
                                if (curTiadcError[chnlid][adcId].gain < 0)
                                {
                                    adcItem[chnlid][baseAdcId].Gain -= (Int32)(curTiadcError[chnlid][adcId].gain * theoryGainStep);
                                    if (adcItem[chnlid][baseAdcId].Gain < minGainCtrlWord || adcItem[chnlid][baseAdcId].Gain > maxGainCtrlWord)
                                        continue;
                                }
                                else
                                {
                                    adcItem[chnlid][adcId].Gain += (Int32)(curTiadcError[chnlid][adcId].gain * theoryGainStep);
                                    if (adcItem[chnlid][adcId].Gain < minGainCtrlWord || adcItem[chnlid][adcId].Gain > maxGainCtrlWord)
                                        continue;
                                }
                            }
                        }
                        TiadcPhaseOffsetGainItem_Base tmpItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)adcId))!.Value;
                        TiadcPhaseOffsetGainItem_Base tmpItemBase = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)baseAdcId))!.Value;
                        //ProductDataTranslate_MSO8000X.SetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)adcId), tmpItem);

                        //由于基础adc相位会被修改，故对其他两个adc进行相位补偿。
                        //int phaseModification = adcItem[chnlid][baseAdcId].Phase - TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][baseAdcId].AcqBdNo, adcId2Index[chnlid][baseAdcId].AdcId, adcId2Index[chnlid][baseAdcId].CoreId].Phase;
                        int phaseModification = adcItem[chnlid][baseAdcId].Phase - tmpItem.Phase;

                        //if (adcId == 2)
                        //{
                        //    adcItem[1].PhaseErr += phaseModification;
                        //    TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlId][1].AcqBdNo, adcId2Index[chnlId][1].AdcId, adcId2Index[chnlId][1].CoreId] = adcItem[1];
                        //}
                        //if (adcId == 3)
                        //{
                        //    adcItem[1].PhaseErr += phaseModification;
                        //    TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlId][1].AcqBdNo, adcId2Index[chnlId][1].AdcId, adcId2Index[chnlId][1].CoreId] = adcItem[1];

                        //    adcItem[2].PhaseErr += phaseModification;
                        //    TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlId][2].AcqBdNo, adcId2Index[chnlId][2].AdcId, adcId2Index[chnlId][2].CoreId] = adcItem[2];
                        //}

                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][adcId].AcqBdNo, adcId2Index[chnlid][adcId].AdcId, adcId2Index[chnlid][adcId].CoreId] = adcItem[chnlid][adcId];
                        //TiAdc_PhaseOffsetGain.Default[adcId2Index[chnlid][baseAdcId].AcqBdNo, adcId2Index[chnlid][baseAdcId].AdcId, adcId2Index[chnlid][baseAdcId].CoreId] = adcItem[chnlid][baseAdcId];

                        ProductDataTranslate_MSO8000X.SetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)adcId), adcItem[chnlid][adcId]);

                        ProductDataTranslate_MSO8000X.SetTiadcParamsItem(new("All-20G", (chnlid), (UInt32)baseAdcId), adcItem[chnlid][baseAdcId]);

                    }
                }

                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                AbstractController_AnalogChannel.CtrlOffset();

                Thread.Sleep(paramSendTimesByms);

                Boolean allOk = true;
                foreach (ChannelId chnlid in curTiadcError.Keys)
                {
                    foreach (Int32 adcId in curTiadcError[chnlid].Keys)
                    {
                        if ((!phaseOK.ContainsKey(chnlid)) || (!phaseOK[chnlid].ContainsKey(adcId)) || (!phaseOK[chnlid][adcId]))
                        {
                            allOk = false;
                        }

                        if ((!gainOK.ContainsKey(chnlid)) || (!gainOK[chnlid].ContainsKey(adcId)) || (!gainOK[chnlid][adcId]))
                        {
                            allOk = false;
                        }
                    }
                }


                if (allOk)
                {
                    Trace.WriteLine($"[CaliDbiTiadc]****************************************Tiadc OK (freq:{InputSignalFreqByMhz}MHz)************************");
                    //WeakTip.Default.Write("AutoCaliAtInit", $"{chnlId}子带{subbandId + 1}在内部源{signalFreqByMhz}MHz下TIADC误差校准成功", emergent: false, "", 5);
                    HdIO.Sleep(5);
                    return true;
                }

            }

            Trace.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Tiadc over time (freq:{InputSignalFreqByMhz}MHz)!!!!!!!!!!!!!!!!!!!!");
            return false;
        }

        record CtrlWordAndPhase(Int32 CtrlWord, Double PhaseError);

        private Int32 GetPhaseDeltaCtrlWords(CtrlWordAndPhase? negativePhase, CtrlWordAndPhase? positivePhase, Double theotryStep, CtrlWordAndPhase curPhase)
        {
            if (negativePhase != null && positivePhase != null)
            {
                Double phaseerror = positivePhase.PhaseError - negativePhase.PhaseError;
                Double ratio = (positivePhase.CtrlWord - negativePhase.CtrlWord) / phaseerror;
                return negativePhase.CtrlWord + (Int32)(-negativePhase.PhaseError * ratio);
            }

            if (Math.Abs(curPhase.PhaseError) < 150000)
                return (Int32)(curPhase.CtrlWord - curPhase.PhaseError * 3 / theotryStep);

            return (Int32)(curPhase.CtrlWord - curPhase.PhaseError * 0.75 / theotryStep);
        }

        private Int32 GetPhaseDeltaCtrlWords(List<CtrlWordAndPhase> oldctrlwordphase, Double theotryStep, CtrlWordAndPhase curPhase)
        {
            Int32 cnt = oldctrlwordphase.Count;
            if (cnt >= 2 && oldctrlwordphase[cnt - 1].PhaseError != oldctrlwordphase[cnt - 2].PhaseError)
            {
                Double ratio = (oldctrlwordphase[cnt - 1].CtrlWord - oldctrlwordphase[cnt - 2].CtrlWord) / (oldctrlwordphase[cnt - 1].PhaseError - oldctrlwordphase[cnt - 2].PhaseError);
                ratio = Math.Abs(ratio) > 2 * Math.Abs(theotryStep) ? theotryStep : ratio;
                return oldctrlwordphase.Last().CtrlWord + (Int32)(-oldctrlwordphase.Last().PhaseError * ratio * 0.5);
            }

            return (Int32)(curPhase.CtrlWord - curPhase.PhaseError * 1.5 / theotryStep);
        }

        record PhaseGainError(Double phaseByfs, Double gain);

        private Dictionary<ChannelId, Dictionary<Int32, PhaseGainError>> GetAllAdcAverageError(List<ChannelId> chnlIdTable, Double sampFreqByMHz, Double inputSignalFreqByMhz, Int32 baseAdcId = 0, Int32 staticTimes = 5)
        {
            Int32 adcCnt = 4;

            Dictionary<ChannelId, Dictionary<Int32, PhaseGainError>> ans = new();

            Dictionary<ChannelId, Dictionary<Int32, List<Double>>> gainError = new();
            Dictionary<ChannelId, Dictionary<Int32, List<Double>>> phaseError = new();

            foreach (ChannelId chnlid in chnlIdTable)
            {
                ans[chnlid] = new();
                gainError[chnlid] = new();
                phaseError[chnlid] = new();
                for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                {
                    gainError[chnlid][adcid] = new();
                    phaseError[chnlid][adcid] = new();
                }
            }

            for (Int32 i = 0; i < staticTimes; i++)//循环5次
            {
                for (Int32 j = 0; j < 10; j++)
                {
                    if (AbstractController_Misc.AcqIsFulled())//循环测10次（经验值），每次暂停1ms，这10次中若AcqIsFulled，代表采样数据准备好了，则跳出小循环。
                        break;
                    Thread.Sleep(1);
                }

                Dictionary<ChannelId, Dictionary<Int32, Double[]>> alladcdata = GetAllChnlAdcData(chnlIdTable);
                if (alladcdata.Count == 0)//没有数据，则进行下一次大循环，后续代码不执行。
                    continue;

                Boolean ampisok = true;
                foreach (ChannelId chnlid in chnlIdTable)
                {
                    for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                    {
                        if (alladcdata[chnlid][adcid].Length == 0)//检测adc采样数据是否正确获取。
                        {
                            ampisok = false;
                            Trace.WriteLine($"[GetAdcAverageError]chnlid{chnlid}_adcid({adcid}).Length = 0");
                            HdIO.Sleep(5);
                            continue;
                        }
                        var maxvalue = alladcdata[chnlid][adcid].Max();
                        var minvalue = alladcdata[chnlid][adcid].Min();
                        if (maxvalue - minvalue < 160 || maxvalue - minvalue > 4000 || maxvalue == 4095)//一些保护措施。|| minvalue == 0
                        {
                            Trace.WriteLine($"[GetAdcAverageError]chnlid{chnlid}_adcid({adcid}).amp is not ok(minvalue:{minvalue},maxvalue{maxvalue}).");
                            HdIO.Sleep(5);
                            ampisok = false;
                        }
                    }
                }

                if (!ampisok)
                {
                    continue;
                }
                //以上均为保护措施，以下开始计算增益和相位误差
                Dictionary<ChannelId, Dictionary<Int32, SinFitResult>> waveOffsetGainPhaseAdc = new();
                foreach (ChannelId chnlid in chnlIdTable)
                {
                    waveOffsetGainPhaseAdc[chnlid] = new();
                    for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                    {
                        waveOffsetGainPhaseAdc[chnlid][adcid] = SinFitClass.SinFit(alladcdata[chnlid][adcid], sampFreqByMHz / adcCnt, inputSignalFreqByMhz) ?? new SinFitResult(0, 0, 0);

                        if (!waveOffsetGainPhaseAdc[chnlid].Keys.Contains(baseAdcId))//如果没有基础ADC的三参数，则跳出最外层for循环
                            break;
                    }
                }

                if (i == 0)
                    continue;//第一次计算，直接跳出，再获得一组三参数.

                foreach (ChannelId chnlid in chnlIdTable)
                {
                    for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                    {
                        if (adcid == baseAdcId)
                            continue;

                        Double gainTmp = (waveOffsetGainPhaseAdc[chnlid][adcid].Gain - waveOffsetGainPhaseAdc[chnlid][baseAdcId].Gain) / waveOffsetGainPhaseAdc[chnlid][baseAdcId].Gain;
                        //相位: (phase_adc2 + 2pi - base) / 2pi
                        Double phaseTmp = (waveOffsetGainPhaseAdc[chnlid][adcid].Phase + Math.PI * 2 - waveOffsetGainPhaseAdc[chnlid][baseAdcId].Phase) % (Math.PI * 2);

                        gainError[chnlid][adcid].Add(gainTmp);
                        phaseError[chnlid][adcid].Add(phaseTmp);
                    }
                }
            }

            foreach (ChannelId chnlid in chnlIdTable)
            {
                for (Int32 adcid = 0; adcid < gainError[chnlid].Count; adcid++)
                {
                    if (gainError[chnlid][adcid].Count > 0)
                        Trace.WriteLine($"[GetAllAdcAverageError]chnlId({chnlid}) adcId({adcid}) gainError({String.Join(",", gainError[chnlid][adcid].Select(o => o.ToString("0.000")))})");
                }

                for (Int32 adcid = 0; adcid < phaseError[chnlid].Count; adcid++)
                {
                    if (phaseError[chnlid][adcid].Count > 0)
                        Trace.WriteLine($"[GetAllAdcAverageError]chnlId({chnlid}) adcId({adcid}) phaseError({String.Join(",", phaseError[chnlid][adcid].Select(o => o.ToString("0.000")))})");
                }
                HdIO.Sleep(5);
            }

            Double theoryDeltaByfs = 1_000_000_000d / sampFreqByMHz;

            foreach (ChannelId chnlid in chnlIdTable)
            {
                if (gainError.ContainsKey(chnlid) && phaseError.ContainsKey(chnlid))
                {
                    for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                    {
                        if (gainError[chnlid].ContainsKey(adcid) && phaseError[chnlid].ContainsKey(adcid))
                        {
                            if (gainError[chnlid][adcid].Count == 0 || phaseError[chnlid][adcid].Count == 0)
                                continue;
                            Double gainAvg = gainError[chnlid][adcid].Average();

                            Double sinAverage = phaseError[chnlid][adcid].Select(o => Math.Sin(o)).Average();
                            Double cosAverage = phaseError[chnlid][adcid].Select(o => Math.Cos(o)).Average();
                            Double phaseByfs = Math.Atan2(sinAverage, cosAverage) * 1000_000_000 / inputSignalFreqByMhz / (2 * Math.PI) - theoryDeltaByfs * adcid;

                            if (phaseByfs > 1000_000_000 / inputSignalFreqByMhz / 2)
                                phaseByfs -= 1000_000_000 / inputSignalFreqByMhz;
                            else if (phaseByfs < -1000_000_000 / inputSignalFreqByMhz / 2)
                                phaseByfs += 1000_000_000 / inputSignalFreqByMhz;

                            ans[chnlid][adcid] = new(phaseByfs, gainAvg);
                            Trace.WriteLine($"[GetAdcAverageError]chnlId({chnlid}) adcId({adcid}) phaseByfs({phaseByfs.ToString("0.000")}fs) gainAvg({gainAvg.ToString("0.000")})");
                            HdIO.Sleep(5);
                        }
                    }
                }
            }

            return ans;
        }

        private Dictionary<ChannelId, Dictionary<Int32, Double[]>> GetAllChnlAdcData(List<ChannelId> chnlIdTable)
        {
            Int32 dotCnt = 2048;
            Int32 adcCnt = 4;
            Dictionary<ChannelId, Dictionary<Int32, Double[]>> ans = new();

            var readinfo = new List<ReadInfo>//13G项目里面是怎么Acquire的？下面列表是干嘛的
            {
                new ReadInfo(AcqDataType.AnalogChannel,
                             new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
                             new WfmPkgInfo(10000, 0.1, 0.05),
                             ""),
            };
            Dictionary<AcqDataType, double> samplingRate = new Dictionary<AcqDataType, double>();
            //Boolean acqOk = Acquisition.Acquire(false, false, readinfo, ref samplingRate);
            Boolean acqOk = Hd.AcqWave(false, false, readinfo, ref samplingRate); ;
            if (!acqOk)
                return ans;

            foreach (ChannelId chnlid in chnlIdTable)
            {
                ans[chnlid] = new();
                for (Int32 adcid = 0; adcid < adcCnt; adcid++)
                {
                    ans[chnlid][adcid] = new Double[dotCnt];
                    for (Int32 dotid = 0; dotid < dotCnt; dotid++)
                    {
                        Int32 dotsid = dotid * adcCnt + adcid;
                        if (dotsid < AcqedDataPool.AnalogChData.AllChannelData[(int)chnlid].Count)
                        {
                            ans[chnlid][adcid][dotid] = AcqedDataPool.AnalogChData.AllChannelData[(int)chnlid][dotsid];
                        }
                    }
                }
            }
            return ans;
        }
    }
    public static class SinFitClass
    {
        /// <summary>
        /// 拟合公式：A*cos(w*tn)+B*sin(w*tn)+C
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="sourceSampleRateByMHz">采样率</param>
        /// <param name="sourceFreqByMHz">信号频率</param>
        /// <returns></returns>
        public static SinFitResult? SinFit(Double[] source, Double sourceSampleRateByMHz, Double sourceFreqByMHz)
        {
            Int32 len = source.Length;
            if (len == 0 || sourceSampleRateByMHz.Equals(0.0) || sourceFreqByMHz.Equals(0.0))
                return null;

            Double[] cos_data = new Double[len];
            Double[] sin_data = new Double[len];

            Double ratio = 2 * Math.PI * sourceFreqByMHz / sourceSampleRateByMHz;
            for (Int32 i = 0; i < len; i++)
            {
                cos_data[i] = Math.Cos(ratio * i);
                sin_data[i] = Math.Sin(ratio * i);
            }

            Double cos_sum = cos_data.Sum();
            Double sin_sum = sin_data.Sum();
            Double cos_avg = cos_sum / len;
            Double sin_avg = sin_sum / len;
            Double cos_cos_dp = cos_data.DotProd(cos_data);
            Double sin_sin_dp = sin_data.DotProd(sin_data);
            Double cos_sin_dp = cos_data.DotProd(sin_data);

            Double src_avg = source.Average();
            Double src_cos_dp = source.DotProd(cos_data);
            Double src_sin_dp = source.DotProd(sin_data);

            Double An = (src_cos_dp - src_avg * cos_sum) / (cos_sin_dp - sin_avg * cos_sum) - (src_sin_dp - src_avg * sin_sum) / (sin_sin_dp - sin_avg * sin_sum);
            Double Ad = (cos_cos_dp - cos_avg * cos_sum) / (cos_sin_dp - sin_avg * cos_sum) - (cos_sin_dp - cos_avg * sin_sum) / (sin_sin_dp - sin_avg * sin_sum);
            Double A = An / Ad;

            Double Bn = (src_cos_dp - src_avg * cos_sum) / (cos_cos_dp - cos_avg * cos_sum) - (src_sin_dp - src_avg * sin_sum) / (cos_sin_dp - cos_avg * sin_sum);
            Double Bd = (cos_sin_dp - sin_avg * cos_sum) / (cos_cos_dp - cos_avg * cos_sum) - (sin_sin_dp - sin_avg * sin_sum) / (cos_sin_dp - cos_avg * sin_sum);
            Double B = Bn / Bd;

            Double C = src_avg - A * cos_avg - B * sin_avg;

            return new SinFitResult(C, Math.Sqrt(A * A + B * B), Math.Atan2(A, B));
        }
    }

    public record SinFitResult(Double Offset, Double Gain, Double Phase);
}



//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Channels;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using Microsoft.VisualBasic;
//using ScopeX.ComModel;
//using ScopeX.Hardware.Calibration.Data.Base;
//using ScopeX.Hardware.Driver.Module;
//using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
//using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;
//using Constants = ScopeX.ComModel.Constants;
//using TiadcParamsKeyMap = ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X.TiadcParamsKeyMap;
//namespace ScopeX.Hardware.Driver
//{
//    public partial class Cali
//    {
//        public Boolean CaliStatus = true;
//        private String _TiAdcCaliLog => $"TiAdcCaliLog{DateTime.Now.ToString("yyyyMMdd")}.txt";

//        /// <summary>
//        /// 打印校准日志
//        /// </summary>
//        /// <param name="msg"></param>
//        private void PrintTiAdcCaliLog(String msg)
//        {
//            if (!Directory.Exists(CaliDataPath))
//            {
//                Directory.CreateDirectory(CaliDataPath);
//            }
//            var filename = $"{CaliDataPath}{_TiAdcCaliLog}";
//            //File.AppendAllText(filename, $"{DateTime.Now.ToString("HH:mm:ss.ffff")}：{msg}{Environment.NewLine}");
//        }

//        /// <summary>
//        /// 清除校准日志
//        /// </summary>
//        private void ClearTiAdcCaliLog()
//        {
//            String[] tiadccalilogpaths = Directory.GetFiles(CaliDataPath);
//            foreach (var item in tiadccalilogpaths)
//            {
//                String filename = Path.GetFileName(item);
//                if (filename.StartsWith("TiAdcCaliLog") && filename != _TiAdcCaliLog)
//                {
//                    File.Delete(item);
//                }
//            }
//        }

//        /// <summary>
//        /// 删除波形日志
//        /// </summary>
//        private void DeleteWaveData()
//        {
//            var dirfile = Directory.GetFiles("./log");
//            foreach (var dir in dirfile)
//            {
//                if (!dir.StartsWith("Log4net") && File.Exists(dir))
//                {
//                    File.Delete(dir);
//                }
//            }
//        }

//        /// <summary>
//        /// tiAdc自动校准
//        /// </summary>
//        public void TiAdcAutoCali_Exec()
//        {
//            try
//            {
//                ClearTiAdcCaliLog();
//                if (AppConfig.GetIntance().AdcCalibrationModel == 0)//此模式不校准TiAdc
//                {
//                    return;
//                }
//                CaliStatus = true;
//                Int32 acqcount = 0; //采集或校准次数
//                CloseAdcRelatedImpacts();//关闭影响相位的条件，（触发和DSP）
//                Switch100MGen();//输入100M信号
//                HdIO.Sleep(50);
//                SwitchSamplingMode(AdcInterleaveMode.Mode2To1);//切换到10G模式 
//                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;//获取当前交织信息
//                AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;//获取当前采样模式
//                ReSetAdcConfig(define);//初始化Adc参数 phase:32000 delay:128
//                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
//                SetSyncSampleClock(define);//设置亚稳态区间
//                DeleteWaveData();

//                #region 判断采集器是否采满，用于取数据判断凭据
//                if (!AbstractController_Misc.AcqIsFulled())
//                {
//                    //读使能复位
//                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
//                    Acquisition.InitAcq(true);
//                    Thread.Sleep(10);
//                }
//                #endregion
//                Stopwatch sw = Stopwatch.StartNew();   


//                #region 校准8片adc间同步
//                bool isparallel = true;//并行校准
//                acqcount = 0;
//                var deltas = InitAdcDelta();
//                //if (AppConfig.GetIntance().AdcCalibrationModel == 0)

//                //校准adc同步
//                while (CaliStatus && !CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0(define, deltas, 15, acqcount == 40) && acqcount < 40)
//                    //while (CaliStatus && !CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0parallel(define, deltas, 15, acqcount == 40) && acqcount < 40)
//                {
//                    acqcount++;
//                    PrintTiAdcCaliLog($"校准20G通道间延时,第{acqcount}次");
//                    SetSyncSampleClock(define, true);
//                    Thread.Sleep(100);
//                }

//                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
//                //保存校准数据
//                Helper.GetICaliData(CaliDataType.TiadcPhaseOffsetGainParams)?.SaveToFile();

//                #endregion


//                #region 校准20G
//                CaliStatus = true;
//                acqcount = 0;
//                SwitchSamplingMode(AdcInterleaveMode.Mode2To1);
//                define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
//                //ReSetAdcConfig(define);
//                //Copy10GTo20GData();
//                //CopyAll_20GData();

//                // Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
//                acqcount = 0;
//                //SetSyncSampleClock(define);//设置亚稳态区间
//                //PrintTiAdcCaliLog($"开启All-20G");
//                deltas = InitAdcDelta();
//                while (CaliStatus && !CalcAdcPhaseData(define, deltas, 1, acqcount == 60) && acqcount < 60)
//                    //while (CaliStatus && !CalcAdcPhaseDataparallel(define, deltas,1, acqcount ==60) && acqcount < 60)
//                    {
//                    acqcount++;
//                    PrintTiAdcCaliLog($"校准20G,第{acqcount}次");
//                    SetSyncSampleClock(define, true);
//                    Thread.Sleep(100);
//                }
//                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
//                SetSyncSampleClock(define);//设置亚稳态区间

//                ///将C1C3-20G的数据拷贝到C1-20G和C3-20Gx //为什么
//                CopyAll_20GData();
//                //保存校准数据
//                Helper.GetICaliData(CaliDataType.TiadcPhaseOffsetGainParams)?.SaveToFile();
//                //打开触发
//                //OpenAdcRelatedImpacts();
//                sw.Stop();
//                var ss = sw.ElapsedMilliseconds.ToString();
//                #endregion

//            }
//            catch (Exception ex)
//            {
//       //         Close100MGen();
//                //打开触发
//                OpenAdcRelatedImpacts();
//                PrintTiAdcCaliLog($"TiAdc自校正异常，{ex.Message}");
//                PrintTiAdcCaliLog($"堆栈信息，{ex.StackTrace}");
//            }

//            CaliStatus = false;

//        }

//        /// <summary>
//        /// 初始化Adc校准参数实例
//        /// </summary>
//        private List<AdcDelta> InitAdcDelta(Boolean isBoard = false)
//        {
//            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
//            AcqModeAndInterleaveDefine currdetail = analogacquiremodel.GetCurrentAcqModeInterleave()!;
//            Int32 acqcount = 0;
//            //Int32 model = currdetail.InterleaveMode == AdcInterleaveMode.Mode2To1 ? 1 : 0;
//            Int32 model = 0;

//            List<AdcDelta> adcdeltas = new List<AdcDelta>();
//            foreach (var detail in currdetail.Details)
//            {
//                foreach (var item in detail.Value)
//                {
//                    foreach (var adc in item.AdcPorts)
//                    {
//                        if (isBoard && adc.Key == 0)
//                        {
//                            continue;
//                        }
//                        if (AppConfig.GetIntance().AdcCalibrationModel == 2 && (detail.Key == ChannelId.C3 || detail.Key == ChannelId.C4))//此模式不校准采集板2
//                        {
//                            continue;
//                        }
//                        if (AppConfig.GetIntance().AdcCalibrationModel == 3 && (detail.Key == ChannelId.C1 || detail.Key == ChannelId.C2))//此模式不校准采集板1
//                        {
//                            continue;
//                        }
//                        TiadcParamsKeyMapWithBoard itemkey = new(currdetail.Name, detail.Key,item.AcqBdNo, (UInt32)adc.Key);
//                        TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
//                        tiadcItem.AdcDelay_FPGA = 0;
//                        AdcDelta adcdelta = new AdcDelta();
//                        adcdelta.AcqBdNo = item.AcqBdNo;
//                        adcdelta.ItemKey = itemkey;
//                        adcdelta.ModelIndex = model;
//                        adcdelta.ChanelIndex = (Int32)detail.Key;
//                        adcdelta.AdcIndx = (UInt32)adc.Key;
//                        adcdelta.Index = acqcount;
//                        adcdelta.AdcInterleaveMode = currdetail.InterleaveMode;
//                        acqcount++;
//                        adcdeltas.Add(adcdelta);
//                    }
//                }

//            }
//            return adcdeltas;
//        }

//        /// <summary>
//        /// 设置同步窗
//        /// </summary>
//        private void SetSyncSampleClock(AcqModeAndInterleaveDefine define, Boolean isCheck = false,Boolean isparallel =true)
//        {
//            if (!isparallel)
//            {
//                SetSyncSampleClockparallel(define, isCheck);
//                return;
//            }
//            //获取扫窗数据
//            var adcscandatas = Hd.CurrProduct?.AcqBd?.ReadADC5200SyncWindowRegValue().Split(Environment.NewLine);

//            Int32 fpgaindex = 0;
//            Int32 adcindex = 0;
//            if (adcscandatas == null)
//            {
//                return;
//            }
//            foreach (var item in adcscandatas)
//            {
//                if (String.IsNullOrEmpty(item))
//                {
//                    continue;
//                }
//                LogSyncSampleClock($"扫窗数据，value: {item}");
//                var adcscandata = item.Split('>');
//                //采集板
//                String board = adcscandata[0];
//                //扫窗数据
//                String sacndata = adcscandata[1].Replace("_", "");

//                Dictionary<Int32, Int32> steadystate = new Dictionary<Int32, Int32>();
//                Int32 offset = 0;
//                Int32 soffset = 0;
//                for (Int32 i = sacndata.Length - 1; i > 7; i--)
//                {
//                    if (sacndata[i] == '0')
//                    {
//                        offset++;
//                        if (!steadystate.TryAdd(soffset, offset))
//                        {
//                            steadystate[soffset] = offset;
//                        }
//                    }
//                    else
//                    {
//                        offset = 0;
//                        soffset = i;
//                    }
//                }
//                var maxvalue = steadystate.Values.Max();
//                var maxkeyvaluepair = steadystate.FirstOrDefault(kvp => kvp.Value == maxvalue);
//                {
//                    //设置采集板
//                    switch (board.Replace("=", ""))
//                    {
//                        case "B8.Adc1": fpgaindex = 7; adcindex = 0; break;
//                        case "B8.Adc2": fpgaindex = 7; adcindex = 1; break;
//                        case "B7.Adc1": fpgaindex = 6; adcindex = 0; break;
//                        case "B7.Adc2": fpgaindex = 6; adcindex = 1; break;
//                        case "B6.Adc1": fpgaindex = 5; adcindex = 0; break;
//                        case "B6.Adc2": fpgaindex = 5; adcindex = 1; break;
//                        case "B5.Adc1": fpgaindex = 4; adcindex = 0; break;
//                        case "B5.Adc2": fpgaindex = 4; adcindex = 1; break;
//                        case "B4.Adc1": fpgaindex = 3; adcindex = 0; break;
//                        case "B4.Adc2": fpgaindex = 3; adcindex = 1; break;
//                        case "B3.Adc1": fpgaindex = 2; adcindex = 0; break;
//                        case "B3.Adc2": fpgaindex = 2; adcindex = 1; break;
//                        case "B2.Adc1": fpgaindex = 1; adcindex = 0; break;
//                        case "B2.Adc2": fpgaindex = 1; adcindex = 1; break;
//                        case "B1.Adc1": fpgaindex = 0; adcindex = 0; break;
//                        case "B1.Adc2": fpgaindex = 0; adcindex = 1; break;
//                        default:
//                            break;
//                    }
//                }

//                var postion = maxvalue / 2;
//                postion += maxvalue % 2 == 1 ? 0 : 0;
//                if (isCheck)
//                {
//                    UInt32 sampleclockdelay = define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay : TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay;
//                    if (sampleclockdelay != (UInt32)((24 - maxkeyvaluepair.Key) + postion))
//                    {
//                        LogSyncSampleClock($"{define.InterleaveMode.ToString()}使用窗，board:{fpgaindex} adcindex:{adcindex} value: {sampleclockdelay}");
//                        LogSyncSampleClock($"{define.InterleaveMode.ToString()}当前窗，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
//                    }
//                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    else
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                }
//                else
//                {
//                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    else
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    LogSyncSampleClock($"{define.InterleaveMode.ToString()}扫窗亚稳态区间校准，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
//                }
//            }

//            //保存校准数据
//            Helper.GetICaliData(CaliDataType.TiAdc_SyncSampleClock)?.SaveToFile();

//            #region 亚稳态窗 SyncSampleClock
//            Hd.CurrProduct?.AcqBd?.TiAdc_ApplayAdc_SyncSampleClock();
//            #endregion
//        }
//        private void SetSyncSampleClockparallel(AcqModeAndInterleaveDefine define, Boolean isCheck = false)
//        {
//            //获取扫窗数据
//            var adcscandatas = Hd.CurrProduct?.AcqBd?.ReadADC5200SyncWindowRegValue().Split(Environment.NewLine);

//            Int32 fpgaindex = 0;
//            Int32 adcindex = 0;
//            if (adcscandatas == null)
//            {
//                return;
//            }

//            adcscandatas.AsParallel().ForAll(item =>
//            {
//                if (String.IsNullOrEmpty(item))
//                {
//                    return;
//                }
//                //LogSyncSampleClock($"扫窗数据，value: {item}");
//                var adcscandata = item.Split('>');
//                //采集板
//                String board = adcscandata[0];
//                //扫窗数据
//                String sacndata = adcscandata[1].Replace("_", "");

//                Dictionary<Int32, Int32> steadystate = new Dictionary<Int32, Int32>();
//                Int32 offset = 0;
//                Int32 soffset = 0;
//                for (Int32 i = sacndata.Length - 1; i > 7; i--)
//                {
//                    if (sacndata[i] == '0')
//                    {
//                        offset++;
//                        if (!steadystate.TryAdd(soffset, offset))
//                        {
//                            steadystate[soffset] = offset;
//                        }
//                    }
//                    else
//                    {
//                        offset = 0;
//                        soffset = i;
//                    }
//                }
//                var maxvalue = steadystate.Values.Max();
//                var maxkeyvaluepair = steadystate.FirstOrDefault(kvp => kvp.Value == maxvalue);
//                {
//                    //设置采集板
//                    switch (board.Replace("=", ""))
//                    {
//                        case "B8.Adc1": fpgaindex = 7; adcindex = 0; break;
//                        case "B8.Adc2": fpgaindex = 7; adcindex = 1; break;
//                        case "B7.Adc1": fpgaindex = 6; adcindex = 0; break;
//                        case "B7.Adc2": fpgaindex = 6; adcindex = 1; break;
//                        case "B6.Adc1": fpgaindex = 5; adcindex = 0; break;
//                        case "B6.Adc2": fpgaindex = 5; adcindex = 1; break;
//                        case "B5.Adc1": fpgaindex = 4; adcindex = 0; break;
//                        case "B5.Adc2": fpgaindex = 4; adcindex = 1; break;
//                        case "B4.Adc1": fpgaindex = 3; adcindex = 0; break;
//                        case "B4.Adc2": fpgaindex = 3; adcindex = 1; break;
//                        case "B3.Adc1": fpgaindex = 2; adcindex = 0; break;
//                        case "B3.Adc2": fpgaindex = 2; adcindex = 1; break;
//                        case "B2.Adc1": fpgaindex = 1; adcindex = 0; break;
//                        case "B2.Adc2": fpgaindex = 1; adcindex = 1; break;
//                        case "B1.Adc1": fpgaindex = 0; adcindex = 0; break;
//                        case "B1.Adc2": fpgaindex = 0; adcindex = 1; break;
//                        default:
//                            break;
//                    }
//                }

//                var postion = maxvalue / 2;
//                postion += maxvalue % 2 == 1 ? 0 : 0;
//                if (isCheck)
//                {
//                    UInt32 sampleclockdelay = define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay : TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay;
//                    if (sampleclockdelay != (UInt32)((24 - maxkeyvaluepair.Key) + postion))
//                    {
//                        //LogSyncSampleClock($"{define.InterleaveMode.ToString()}使用窗，board:{fpgaindex} adcindex:{adcindex} value: {sampleclockdelay}");
//                        //LogSyncSampleClock($"{define.InterleaveMode.ToString()}当前窗，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
//                    }
//                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    else
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                }
//                else
//                {
//                    if (define.InterleaveMode == AdcInterleaveMode.Mode2To1)
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample20GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    else
//                    {
//                        TiAdc_SyncSampleClock.Default[fpgaindex][adcindex].Sample10GClockDelay = (UInt32)((24 - maxkeyvaluepair.Key) + postion);
//                    }
//                    //LogSyncSampleClock($"{define.InterleaveMode.ToString()}扫窗亚稳态区间校准，board:{fpgaindex} adcindex:{adcindex} value: {(UInt32)((24 - maxkeyvaluepair.Key) + postion)}");
//                }
//            });

//            //保存校准数据
//            Helper.GetICaliData(CaliDataType.TiAdc_SyncSampleClock)?.SaveToFile();

//            #region 亚稳态窗 SyncSampleClock
//            Hd.CurrProduct?.AcqBd?.TiAdc_ApplayAdc_SyncSampleClock();
//            #endregion
//        }
//        /// <summary>
//        /// 记录同步窗日志
//        /// </summary>
//        private void LogSyncSampleClock(String log)
//        {
//            using (StreamWriter sw = new StreamWriter("SyncSampleClock.txt", true))
//            {
//                sw.WriteLine($"【{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}】：{log}");
//            }
//        }

//        /// <summary>
//        /// 采集波形数据
//        /// </summary>
//        /// <param name="wavedata">波形数据</param>
//        /// <param name="timeoutByMs">等待时长</param>
//        /// <returns></returns>s
//        private Boolean AcqWaveDataEx(out List<List<UInt16>> wavedata, Int16 timeoutByMs = 500)
//        {
//            wavedata = new List<List<UInt16>>();
//            Dictionary<AcqDataType, Double> samplingrate = new();

//            List<ReadInfo> readinfolist = new();
//            WfmPkgInfo viewpkg = new(Hd.UIMessage!.Timebase!.StorageWaveDotsCnt, Hd.UIMessage!.Timebase!.TmbScale * 10, 0);
//            ReadInfo viewinfo = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), viewpkg, "View");
//            readinfolist.Add(viewinfo);

//            Stopwatch stopwatcher = new();
//            stopwatcher.Restart();
//            var bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
//            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
//            {
//                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
//            }
//            stopwatcher.Restart();
//            bok = false;
//            //丢弃两次待通道稳定
//            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
//            {
//                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
//            }
//            stopwatcher.Restart();
//            bok = false;
//            while (!bok && stopwatcher.ElapsedMilliseconds < timeoutByMs)
//            {
//                bok = Hd.AcqWave(false, false, readinfolist, ref samplingrate);
//            }
//            stopwatcher.Stop();

//            var readinfo = readinfolist.FirstOrDefault(info => info.DataType == AcqDataType.AnalogChannel);
//            Hd.AnalogChannel!.TakeAdcWaveform(out wavedata);
//            return bok;
//        }

//        /// <summary>
//        /// 切换模拟通道输入100MHz单频点信号
//        /// </summary>
//        private void Switch100MGen()
//        {
//            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
//            //Hd.CurrDebugVarints.bEnable_OpenCrystal = true;
//        }

//        /// <summary>
//        /// 关闭100M信号
//        /// </summary>
//        private void Close100MGen()
//        {
//            Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000000, 0xa << 8);
//            //Hd.CurrDebugVarints.bEnable_OpenCrystal = false;
//        }

//        /// <summary>
//        /// 切换采样模式
//        /// </summary>
//        private void SwitchSamplingMode(AdcInterleaveMode adcInterleaveMode, Boolean status = true)
//        {
//            if (!status)
//            {
//                return;
//            }
//            var analog0ptions = new List<HdMessage.AnalogOptions>();
//            Int32 storagewavedotscnt = 50 * 1000;
//            var coupling = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? AnaChnlCoupling.DC50 : AnaChnlCoupling.DC1M;

//            switch (adcInterleaveMode)
//            {
//                case AdcInterleaveMode.Mode1To1:
//                    //List<ChannelId> needcali10gchannels = new()
//                    //{
//                    //    ChannelId.C1, ChannelId.C2, ChannelId.C3,  ChannelId.C4
//                    //};
//                    //for (var chnl = 0; chnl < needcali10gchannels.Count; chnl++)
//                    //{
//                    //    HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![chnl] with
//                    //    {
//                    //        Active = true,
//                    //        Bandwidth = 1,
//                    //        IsInverted = false,
//                    //        ScaleIndex = (Int32)AnaChnlScaleIndex.Lv100m,
//                    //        Scale = 100,
//                    //        Coupling = coupling,
//                    //        Bias = 0,
//                    //        Position = 0,
//                    //    };
//                    //    analog0ptions.Add(ch);
//                    //}
//                    ////Hd.UIMessage!.Analog.Where(p => p.Active = true);
//                    //break;
//                case AdcInterleaveMode.Mode2To1:
//                default:
//                    List<ChannelId> needcali20gchannels = new()
//                    {
//                        ChannelId.C1, ChannelId.C2, ChannelId.C3,ChannelId.C4
//                    };
//                    foreach (var item in needcali20gchannels)
//                    {
//                        Boolean active = true;// (item == ChannelId.C1 || item == ChannelId.C3) ? true : false;
//                        HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![(Int32)item] with
//                        {
//                            Active = active,
//                            Bandwidth = 1,
//                            IsInverted = false,
//                            ScaleIndex = (Int32)AnaChnlScaleIndex.Lv50m,
//                            //Scale = 100,
//                            Scale = 50,
//                            Coupling = coupling,
//                            Bias = 0,
//                            Position = 0,
//                        };
//                        analog0ptions.Add(ch);
//                    }
//                    storagewavedotscnt = 100 * 1000;
//                    break;
//            }
//            var mode = Hd.UIMessage!.Timebase! with
//            {
//                TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv100n].Scale,
//                TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv100n,
//                StorageWaveDotsCnt = storagewavedotscnt,
//                NeedWaveDotsCnt = storagewavedotscnt,
//                InterleaveMode = adcInterleaveMode
//            };
//            var display = Hd.UIMessage!.Display! with { IsFast = false };
//            Hd.UIMessage = Hd.UIMessage! with { Analog = analog0ptions.ToArray(), Timebase = mode, Display = display };
//            ConfigHardware(Hd.UIMessage, 100);
//        }

//        /// <summary>
//        /// 关闭影响两路ADC相位的功能
//        /// </summary>
//        private void CloseAdcRelatedImpacts()
//        {
//            //关闭数字触发
//            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
//            //关闭触发丢点
//            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = false;
//            //关闭数字处理
//            Hd.CurrDebugVarints.bEnable_Dsp = false;
//            Hd.CurrDebugVarints.bEnable_Dsp_Pro = false;
//            //cij_0425
//            Hd.CurrDebugVarints.bEnable_IsOpenDDr = false;//true
//        }

//        /// <summary>
//        /// 打开影响两路ADC相位的功能
//        /// </summary>
//        private void OpenAdcRelatedImpacts()
//        {
//            //打开触发
//            Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
//            //打开触发丢点
//            Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
//            //打开数字处理
//            Hd.CurrDebugVarints.bEnable_Dsp = true;
//            Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
//            Hd.CurrDebugVarints.bEnable_IsOpenDDr = true;//true
//        }

//        #region 校准板内相位差

//        /// <summary>
//        /// 初始化Adc的配置
//        /// </summary>
//        private void ReSetAdcConfig(AcqModeAndInterleaveDefine define)
//        {
//            if (define != null)
//            {
//                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
//                List<TiadcParamsKeyMap> tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
//                {
//                    //new("C1C3-20G", (ChannelId.C1), 0),
//                    //new("C1C3-20G", (ChannelId.C1), 1),
//                    //new("C1C3-20G", (ChannelId.C3), 0),
//                    //new("C1C3-20G", (ChannelId.C3), 1)
//                };
//                if (define!.InterleaveMode == AdcInterleaveMode.Mode2To1)
//                {
//                    tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
//                    {
//                        new("All-20G", (ChannelId.C1), 0),
//                        new("All-20G", (ChannelId.C1), 1),
//                        new("All-20G", (ChannelId.C1), 2),
//                        new("All-20G", (ChannelId.C1), 3),
//                        new("All-20G", (ChannelId.C2), 0),
//                        new("All-20G", (ChannelId.C2), 1),
//                        new("All-20G", (ChannelId.C2), 2),
//                        new("All-20G", (ChannelId.C2), 3),
//                        new("All-20G", (ChannelId.C3), 0),
//                        new("All-20G", (ChannelId.C3), 1),
//                        new("All-20G", (ChannelId.C3), 2),
//                        new("All-20G", (ChannelId.C3), 3),
//                        new("All-20G", (ChannelId.C4), 0),
//                        new("All-20G", (ChannelId.C4), 1),
//                        new("All-20G", (ChannelId.C4), 2),
//                        new("All-20G", (ChannelId.C4), 3)
//                    };
//                }

//                foreach (var item in tiadcparamskeymaps)
//                {
//                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(item)!.Value;//20G参数
//                    tiadcitem.Phase = 32000;
//                    tiadcitem.AdcDelay_FPGA = 128;
//                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcitem);
//                }
//            }
//        }

//        /// <summary>
//        /// 将C1C3_20G的配置拷贝C1-20G和C3-20G
//        /// </summary>
//        private void CopyAll_20GData() //从之前的C1C3改到All-20G
//        {
//            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
//            AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;
//            List<TiadcParamsKeyMapWithBoard> tiadcparamskeymaps = new List<TiadcParamsKeyMapWithBoard>()
//            {
//              //new("All-20G", (ChannelId.C1), 0),
//              new("All-20G", (ChannelId.C1),AcqBdNo.B1, 1),
//              //new("All-20G", (ChannelId.C2), 0),
//              new("All-20G", (ChannelId.C2),AcqBdNo.B3, 1),
//             //new("All-20G", (ChannelId.C3), 0),
//              new("All-20G", (ChannelId.C3),AcqBdNo.B5, 1),
//             // new("All-20G", (ChannelId.C4), 0),
//              new("All-20G", (ChannelId.C4),AcqBdNo.B7, 1) 
//            };

//            //foreach (var item in tiadcparamskeymaps)
//            //{
//            //    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(item)!.Value;//20G参数
//            //    tiadcitem.AdcDelay_FPGA += 1;
//            //    ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(item, tiadcitem);
//            //}
//        }

//        /// <summary>
//        /// 校准同步
//        /// </summary>
//        private Boolean CalcAdcPhaseData(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = true, Boolean isparallel = true)
//        {
//            //if (!isparallel)
//            //{
//            //    return CalcAdcPhaseDataparallel(define,  adcDeltas,  delta_pS,  isEnd);
//            //}
//            Boolean caliadcstatus = true;

//            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
//            for (Int32 i = 0; i < 1; i++)
//            {
//                adcdeltalist = GetAdcPhaseData(define, adcDeltas, delta_pS, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
//            }
//            #region 校准相位差
//            if (!caliadcstatus)
//            {
//                int fpgaSampDeltaPS = 100;


//                foreach (var item in adcdeltalist)
//                {

//                    Double ta0 = item.Delta % fpgaSampDeltaPS;
//                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
//                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
//                    {
//                        fpgadelay += ta0 > 0 ? 1 : -1;
//                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
//                    }
//                    //更新校准基数
//                    item.Delta = ta0;
//                    item.FpgaDelayer = -fpgadelay;
//                }
//                foreach (var item in adcdeltalist)
//                {
//                    if (item.AcqBdNo == AcqBdNo.B0 || item.AcqBdNo == AcqBdNo.B1)
//                    {
//                        continue;
//                    }
//                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex,item.AcqBdNo, item.AdcIndx);
//                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
//                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
//                    {
//                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
//                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcitem.Phase, item.Delta));
//                        item.calc();
//                        if (item.Delta != 0)
//                        {
//                            PrintTiAdcCaliLog($"斜率：{item.rate} 基数：{item.Delta}");
//                        }
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, false);
//                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                    else
//                    {
//                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > delta_pS) != null ? false : true;
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, isDelay || isEnd);
//                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                }
//                adcDeltas = adcdeltalist;
//                //发送TiAdc参数
//                return caliadcstatus;
//            }

//            #endregion 校准相位差

//            return caliadcstatus;
//        }
//        private Boolean CalcAdcPhaseDataparallel(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = true)
//        {
//            Boolean caliadcstatus = true;

//            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
//            for (Int32 i = 0; i < 1; i++)
//            {
//                adcdeltalist = GetAdcPhaseData(define, adcDeltas, delta_pS, define.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "20G" : "10G" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
//            }
//            #region 校准相位差
//            if (!caliadcstatus)
//            {
//                int fpgaSampDeltaPS = 100;


//                //foreach (var item in adcdeltalist)
//                adcdeltalist.AsParallel().ForAll(item =>
//                {

//                    Double ta0 = item.Delta % fpgaSampDeltaPS;
//                    Int32 fpgadelay = (Int32)item.Delta / fpgaSampDeltaPS;
//                    if (Math.Abs(ta0) >= fpgaSampDeltaPS / 2.0 && Math.Abs(ta0) <= fpgaSampDeltaPS)
//                    {
//                        fpgadelay += ta0 > 0 ? 1 : -1;
//                        ta0 = ta0 > 0 ? ta0 - fpgaSampDeltaPS : ta0 + fpgaSampDeltaPS;
//                    }
//                    //更新校准基数
//                    item.Delta = ta0;
//                    item.FpgaDelayer = -fpgadelay;

//                    //if (item.AcqBdNo == AcqBdNo.B0 || item.AcqBdNo == AcqBdNo.B1)
//                    //{
//                    //    continue;
//                    //}
//                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
//                    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
//                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
//                    {
//                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
//                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcitem.Phase, item.Delta));
//                        item.calc();
//                        if (item.Delta != 0)
//                        {
//                            PrintTiAdcCaliLog($"斜率：{item.rate} 基数：{item.Delta}");
//                        }
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, false);
//                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                    else
//                    {
//                        bool isDelay = adcdeltalist.Find(p => Math.Abs(p.Delta) > delta_pS) != null ? false : true;
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, isDelay || isEnd);
//                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                });
//                adcDeltas = adcdeltalist;
//                //发送TiAdc参数
//                return caliadcstatus;
//            }

//            #endregion 校准相位差

//            return caliadcstatus;
//        }
//        /// <summary>
//        /// 获取Adc数据
//        /// </summary>
//        /// <param name="define"></param>
//        /// <param name="adcDeltas"></param>
//        /// <param name="delta_pS"></param>
//        /// <param name="caliAdcStatus"></param>
//        /// <returns></returns>
//        private List<AdcDelta> GetAdcPhaseData(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, String nowCalitimes, out Boolean caliAdcStatus)
//        {
//            caliAdcStatus = true;
//            Double inputsignalfreqbymhz = 100d;
//            Double theorydelta_ps = 25;// define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
//            #region 获取数据
//            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphaseerr = new Dictionary<String, List<WaveOffsetGainPhase>>();
//            Dictionary<String, List<WaveOffsetGainPhase>> tempwaveoffsetgainphases = GetPhaseData(define, nowCalitimes);
//            String fintkey = String.Empty;
//            if (!CaliStatus || tempwaveoffsetgainphases.Count == 0)
//            {
//                caliAdcStatus = false;
//                return adcDeltas;
//            }
//            #endregion

//            #region 计算三参数

//            foreach (var item in tempwaveoffsetgainphases)
//            {
//                if (item.Key.Contains("C1"))
//                    fintkey ="All-20G_C1_B1_Adc0";
//                if (item.Key.Contains("C2"))
//                    fintkey = "All-20G_C2_B3_Adc0";
//                if (item.Key.Contains("C3"))
//                    fintkey = "All-20G_C3_B5_Adc0";
//                if (item.Key.Contains("C4"))
//                    fintkey = "All-20G_C4_B7_Adc0";
//                //else
//                //    fintkey = item.Key;

//                for (Int32 i = 0; i < item.Value.Count; i++)
//                {
//                    Int32 adcindex = fintkey == item.Key ? 0 : 1;
//                    switch (item.Key)
//                    {
//                        case "All-20G_C1_B0_Adc0": adcindex = 1; break;
//                        case "All-20G_C1_B0_Adc1": adcindex = 3; break;
//                        case "All-20G_C1_B1_Adc0": adcindex = 0; break;
//                        case "All-20G_C1_B1_Adc1": adcindex = 2; break;

//                        case "All-20G_C2_B2_Adc0": adcindex = 1; break;
//                        case "All-20G_C2_B2_Adc1": adcindex = 3; break;
//                        case "All-20G_C2_B3_Adc0": adcindex = 0; break;
//                        case "All-20G_C2_B3_Adc1": adcindex = 2; break;

//                        case "All-20G_C3_B4_Adc0": adcindex = 1; break;
//                        case "All-20G_C3_B4_Adc1": adcindex = 3; break;
//                        case "All-20G_C3_B5_Adc0": adcindex = 0; break;
//                        case "All-20G_C3_B5_Adc1": adcindex = 2; break;

//                        case "All-20G_C4_B6_Adc0": adcindex = 1; break;
//                        case "All-20G_C4_B6_Adc1": adcindex = 3; break;
//                        case "All-20G_C4_B7_Adc0": adcindex = 0; break;
//                        case "All-20G_C4_B7_Adc1": adcindex = 2; break;
//                        default:
//                            break;
//                    }

//                    Double phaseerror_ps = 25;
//                    phaseerror_ps = ((item.Value[i].Phase - tempwaveoffsetgainphases[fintkey][i].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalfreqbymhz / (2 * Math.PI) - adcindex * theorydelta_ps;
//                    if (phaseerror_ps > 1000_000 / inputsignalfreqbymhz / 2)
//                        phaseerror_ps -= 1000_000 / inputsignalfreqbymhz;
//                    else if (phaseerror_ps < -1000_000 / inputsignalfreqbymhz / 2)
//                        phaseerror_ps += 1000_000 / inputsignalfreqbymhz;
//                    if (waveoffsetgainphaseerr.Keys.Contains(item.Key))
//                    {
//                        waveoffsetgainphaseerr[item.Key].Add(new WaveOffsetGainPhase() { Phase = phaseerror_ps, });
//                    }
//                    else
//                    {
//                        waveoffsetgainphaseerr.Add(item.Key, new List<WaveOffsetGainPhase>()
//                                {
//                                    new WaveOffsetGainPhase(){ Phase = phaseerror_ps, }
//                                });
//                    }
//                }
//                if (item.Key != fintkey)
//                {
//                    PrintTiAdcCaliLog($"{item.Key}与{fintkey}相位差：{waveoffsetgainphaseerr[item.Key].Average(p => p.Phase)}");
//                }
//            }

//            #endregion

//            #region 判断是否校准完成
//            caliAdcStatus = true;
//            foreach (var item in adcDeltas)
//            {
//                String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_"+ item.AcqBdNo + "_Adc" + item.AdcIndx;
//                item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);
//                //判断相位差是否小于设定的误差范围
//                if (Math.Abs(item.Delta) > delta_pS)
//                {
//                    caliAdcStatus = false;
//                    break;
//                }
//            }
//            if (caliAdcStatus == true)
//            {

//            }
//            #endregion
//            return adcDeltas;
//        }

//        /// <summary>
//        /// 设置Adc的相位和丢点
//        /// </summary>
//        /// <param name="tiadcParamsKeyMap"></param>
//        /// <param name="data"></param>
//        /// <param name="adcIndex"></param>
//        /// <param name="isUpdatedelay"></param>
//        /// <returns></returns>
//        private void SetAcqData(TiadcParamsKeyMap tiadcParamsKeyMap, Int32[] data, UInt32 adcIndex, Boolean isUpdatedelay = true, Boolean isCali = true)
//        {
//            if (!isCali)
//                return;
//            TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(tiadcParamsKeyMap)!.Value;
//            tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(tiadcParamsKeyMap)!.Value;
//            if (!isUpdatedelay)
//            {
//                if (data[0] != 0)
//                {
//                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
//                    if (tiadcitem.Phase - data[0] < 0)
//                    {
//                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
//                        CaliStatus = false;
//                    }
//                    else if (tiadcitem.Phase - data[0] > 65535)
//                    {
//                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
//                        CaliStatus = false;
//                    }
//                    else
//                    {
//                        tiadcitem.Phase -= data[0];
//                    }
//                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
//                }
//            }
//            else
//            {
//                if (data[1] != 0)
//                {
//                    if (tiadcitem.AdcDelay_FPGA + data[1] > 255)
//                    {
//                        PrintTiAdcCaliLog($"超出FPGA丢点范围：当前：{tiadcitem.AdcDelay_FPGA} 校准值：{data[1]}");
//                        CaliStatus = false;
//                    }
//                    else
//                    {
//                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
//                        tiadcitem.AdcDelay_FPGA += data[1];
//                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
//                    }
//                }
//            }
//            ProductDataTranslate_MSO8000X.SetTiadcParamsItem(tiadcParamsKeyMap, tiadcitem);
//        }

//        private void SetAcqDataWithBoard(TiadcParamsKeyMapWithBoard tiadcParamsKeyMap, Int32[] data, UInt32 adcIndex, Boolean isUpdatedelay = true, Boolean isCali = true)
//        {
//            if (!isCali)
//                return;
//            TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(tiadcParamsKeyMap)!.Value;
//            if (!isUpdatedelay)
//            {
//                if (data[0] != 0)
//                {
//                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
//                    if (tiadcitem.Phase - data[0] < 0)
//                    {
//                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
//                        CaliStatus = false;
//                    }
//                    else if (tiadcitem.Phase - data[0] > 65535)
//                    {
//                        PrintTiAdcCaliLog($"超出Adc校准范围：当前：{tiadcitem.Phase} 校准值：{data[0]}");
//                        CaliStatus = false;
//                    }
//                    else
//                    {
//                        tiadcitem.Phase -= data[0];
//                    }
//                    PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，PhaseErrAD{adcIndex}：{tiadcitem.Phase}");
//                }
//            }
//            else
//            {
//                if (data[1] != 0)
//                {
//                    if (tiadcitem.AdcDelay_FPGA + data[1] > 255)
//                    {
//                        PrintTiAdcCaliLog($"超出FPGA丢点范围：当前：{tiadcitem.AdcDelay_FPGA} 校准值：{data[1]}");
//                        CaliStatus = false;
//                    }
//                    else
//                    {
//                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准前，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
//                        tiadcitem.AdcDelay_FPGA += data[1];
//                        PrintTiAdcCaliLog($"{tiadcParamsKeyMap.interleaveName}校准后，通道：{tiadcParamsKeyMap.chnlId}，AdcDelayErr_FPGAAD{adcIndex}：{tiadcitem.AdcDelay_FPGA}");
//                    }
//                }
//            }
//            ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(tiadcParamsKeyMap, tiadcitem);
//        }


//        #endregion


//        #region 校准通道三到通道一的相位差

//        /// <summary>
//        /// 将10G的数据拷贝20G //为什么？
//        /// </summary>
//        private void Copy10GTo20GData()
//        {
//            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
//            AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;
//            List<TiadcParamsKeyMap> tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
//            {
//              new("C1C3-20G", (ChannelId.C1), 1),
//              new("C1C3-20G", (ChannelId.C3), 1),
//              new("C1-20G", (ChannelId.C1), 1),
//              new("C3-20G", (ChannelId.C3), 1)
//            };

//            foreach (var item in tiadcparamskeymaps)
//            {
//                TiadcParamsKeyMap itemkey = new("All-20G", item.chnlId, item.adcId);
//                TiadcPhaseOffsetGainItem_Base tiadcfineitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemkey)!.Value;//10G参数
//                TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(item)!.Value;//20G参数
//                tiadcItem.Phase = tiadcfineitem.Phase;
//                tiadcItem.AdcDelay_FPGA = tiadcfineitem.AdcDelay_FPGA;
//                ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcItem);
//            }
//        }

//        /// <summary>
//        /// 获取Adc数据
//        /// </summary>
//        /// <param name="define">交织模式</param>
//        /// <param name="adcDeltas">adc校准数据</param>
//        /// <param name="delta_pS">误差范围</param>
//        /// <param name="caliAdcStatus">校准Adc状态</param>
//        /// <returns></returns>
//        private List<AdcDelta> GetAdcInterBoardSynchronizationPhaseDataC3ToC1(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, String nowCaliTimes, out Boolean caliAdcStatus)
//        {
//            caliAdcStatus = false;
//            Double inputsignalfreqbymhz = 100d;
//            //Double theorydelta_ps = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
//            Double theorydelta_ps = 0;
//            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphaseerr = new Dictionary<String, List<WaveOffsetGainPhase>>();
//            Dictionary<String, List<WaveOffsetGainPhase>> tempwaveoffsetgainphases = GetPhaseData(define, nowCaliTimes);
//            if (!CaliStatus || tempwaveoffsetgainphases.Count == 0)
//            {
//                caliAdcStatus = false;
//                return adcDeltas;
//            }
//            String fintkey = String.Empty;
//            #region 计算三参数
//            //fintkey = "All-20G_C1_B0_Adc0";
//            fintkey = "All-20G_C1_B1_Adc0";
//            fintkey = "All-20G_C2_B3_Adc0";
//            foreach (var item in tempwaveoffsetgainphases)
//            {
//                //theorydelta_ps = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0 : 1000d / 20;
//                theorydelta_ps = 0;


//                for (Int32 i = 0; i < item.Value.Count; i++)
//                {
//                    Int32 adcIndex = fintkey == item.Key ? 0 : 1;
//                    Double PhaseError_pS = 0;
//                    PhaseError_pS = ((item.Value[i].Phase - tempwaveoffsetgainphases[fintkey][i].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalfreqbymhz / (2 * Math.PI) + theorydelta_ps;
//                    if (PhaseError_pS > 1000_000 / inputsignalfreqbymhz / 2)
//                        PhaseError_pS -= 1000_000 / inputsignalfreqbymhz;
//                    else if (PhaseError_pS < -1000_000 / inputsignalfreqbymhz / 2)
//                        PhaseError_pS += 1000_000 / inputsignalfreqbymhz;
//                    if (waveoffsetgainphaseerr.Keys.Contains(item.Key))
//                    {
//                        waveoffsetgainphaseerr[item.Key].Add(new WaveOffsetGainPhase() { Phase = PhaseError_pS, });
//                    }
//                    else
//                    {
//                        waveoffsetgainphaseerr.Add(item.Key, new List<WaveOffsetGainPhase>()
//                        {
//                             new WaveOffsetGainPhase(){ Phase = PhaseError_pS, }
//                        });
//                    }
//                }
//                if (item.Key != fintkey)
//                {
//                    PrintTiAdcCaliLog($"{item.Key}与{fintkey}相位差：{waveoffsetgainphaseerr[item.Key].Average(p => p.Phase)}");
//                }
//            }

//            #endregion

//            #region 判断是否校准完成
//            caliAdcStatus = true;
//            foreach (var item in adcDeltas)
//            {
//                //if (item.AdcIndx == 1)
//                {
//                    String key = define.Name + "_" + (ChannelId)item.ChanelIndex + "_" + item.AcqBdNo + "_Adc" + item.AdcIndx;
//                    item.Delta = waveoffsetgainphaseerr[key].Average(p => p.Phase);

//                    Double TA0 = Math.Abs(item.Delta) % 100;
//                    if (Math.Abs(item.Delta) > delta_pS)
//                    {
//                        caliAdcStatus = false;
//                    }
//                }
//            }
//            #endregion

//            return adcDeltas;
//        }

//        /// <summary>
//        /// 校准同步
//        /// </summary>
//        private Boolean CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = false, Boolean isparallel = true)
//        {
//            //if (!isparallel)
//            //{
//            //    return CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0parallel(define, adcDeltas,  delta_pS,  isEnd);
//            //}
//            Boolean caliadcstatus = false;
//            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
//            for (Int32 i = 0; i < 1; i++)
//            {
//                adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
//            }
//            #region 校准相位差
//            if (!caliadcstatus)
//            {
//                foreach (var item in adcdeltalist)
//                {
//                    Double ta0 = item.Delta % 100;
//                    Int32 fpgadelay = (Int32)item.Delta / 100;
//                    if (Math.Abs(ta0) >= 50 && Math.Abs(ta0) <= 100)
//                    {
//                        fpgadelay += ta0 > 0 ? 1 : -1;
//                        ta0 = ta0 > 0 ? ta0 - 100 : ta0 + 100;

//                    }
//                    item.Delta = ta0;
//                    item.FpgaDelayer = -fpgadelay;

//                }
//                foreach (var item in adcdeltalist)
//                {
//                    if (item.ItemKey.chnlId == ChannelId.C4)
//                    {
//                        continue;
//                    }
//                    //if (item.AcqBdNo == AcqBdNo.B0 && item.AdcIndx == 0)
//                    //{
//                    //    continue;

//                    //}
//                    ////if (item.AcqBdNo == AcqBdNo.B0 && item.AdcIndx == 1)
//                    ////{
//                    ////    continue;

//                    ////}
//                    //if (item.AcqBdNo == AcqBdNo.B1 && item.AdcIndx == 1)
//                    //{
//                    //    continue;

//                    //}
//                    //if (item.AcqBdNo == AcqBdNo.B1 && item.AdcIndx == 0)
//                    //{
//                    //    continue;

//                    //}
//                    TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
//                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
//                    //if (item.FpgaDelayer != 0)
//                    //{
//                    //    SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
//                    //    (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);

//                    //}
//                    if (Math.Abs(item.Delta) > delta_pS && !isEnd)
//                    {
//                        //if (item.FpgaDelayer != 0)
//                        //{
//                        //    SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
//                        //    (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                        //    continue;
//                        //}
//                        var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
//                        item.AddVaule(new KeyValuePair<Int32, Double>(tiadcItem.Phase, item.Delta));
//                        item.calc();
//                        if (item.Delta != 0)
//                        {
//                            PrintTiAdcCaliLog($"斜率：{item.rate} 基数：{item.Delta}");
//                        }
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, false, true);
//                       (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                    else
//                    {
//                        SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
//                        (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                    }
//                    //tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;
//                    //Hd.CurrProduct?.AcqBd.TiAdc_ApplyAdc_Phase_Offset_Gain();
//                }
//         //       adcDeltas = adcdeltalist;
//         //       adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);

//                return caliadcstatus;
//            }

//        //    adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);

//            #endregion 校准相位差
//            return caliadcstatus;
//        }
//        private Boolean CalcAdcInterBoardSynchronizationPhaseAdc7ToAdc0parallel(AcqModeAndInterleaveDefine define, List<AdcDelta> adcDeltas, Double delta_pS, Boolean isEnd = false)
//        {
//            Boolean caliadcstatus = false;
//            List<AdcDelta> adcdeltalist = new List<AdcDelta>();
//            for (Int32 i = 0; i < 1; i++)
//            {
//                adcdeltalist = GetAdcInterBoardSynchronizationPhaseDataC3ToC1(define, adcDeltas, delta_pS, "board" + DateTime.Now.ToString("hhmmssfff"), out caliadcstatus);
//            }
//            #region 校准相位差
//            if (!caliadcstatus)
//            {
//                //foreach (var item in adcdeltalist)
//                //Parallel.For(0, adcdeltalist.Count, adcdeltalist, item =>
//                adcdeltalist.AsParallel().ForAll( item =>
//                    {
//                        Double ta0 = item.Delta % 100;
//                        Int32 fpgadelay = (Int32)item.Delta / 100;
//                        if (Math.Abs(ta0) >= 50 && Math.Abs(ta0) <= 100)
//                        {
//                            fpgadelay += ta0 > 0 ? 1 : -1;
//                            ta0 = ta0 > 0 ? ta0 - 100 : ta0 + 100;

//                        }
//                        item.Delta = ta0;
//                        item.FpgaDelayer = -fpgadelay;

//                        TiadcParamsKeyMapWithBoard itemkey = new(define!.Name, (ChannelId)item.ChanelIndex, item.AcqBdNo, item.AdcIndx);
//                        TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;

//                        if (Math.Abs(item.Delta) > delta_pS && !isEnd)
//                        {
//                            //if (item.FpgaDelayer != 0)
//                            //{
//                            //    SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
//                            //    (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);

//                            //}
//                            //else
//                            { 
//                                var fintdelta = adcdeltalist.Find(p => p.AdcIndx == 1 && (p.ChanelIndex == item.ChanelIndex || p.ChanelIndex == item.ChanelIndex - 1));
//                                item.AddVaule(new KeyValuePair<Int32, Double>(tiadcItem.Phase, item.Delta));
//                                item.calc();
//                                if (item.Delta != 0)
//                                {
//                                    //PrintTiAdcCaliLog($"斜率：{item.rate} 基数：{item.Delta}");
//                                }
//                                SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, false, true);
//                                (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                            }

//                        }
//                        else
//                        {
//                            SetAcqDataWithBoard(itemkey, new Int32[] { item.CaliDelta, item.FpgaDelayer }, item.AdcIndx, true, true);
//                            (Hd.CurrProduct?.AcqBd as Boadr_Acq_JiHe_MSO8000X).SendAdc_Phase_Offset_Gain((ChannelId)item.ChanelIndex, item.AcqBdNo, (Int32)item.AdcIndx, item.TiadcItem);
//                        }
//                    });
//                return caliadcstatus;
//            }
//            #endregion 校准相位差
//            return caliadcstatus;
//        }

//        #endregion
//        Dictionary<(AcqBdNo, Int32), (AcqBdNo, Int32)> keyValuePairs = new Dictionary<(AcqBdNo, int), (AcqBdNo, int)> {
//            {(AcqBdNo.B0, 0), (AcqBdNo.B1, 0) },
//            {(AcqBdNo.B0, 1), (AcqBdNo.B0, 0) },
//            {(AcqBdNo.B1, 0), (AcqBdNo.B1, 1) },
//            {(AcqBdNo.B1, 1), (AcqBdNo.B0, 1) },
//            {(AcqBdNo.B2, 0), (AcqBdNo.B3, 0) },
//            {(AcqBdNo.B2, 1), (AcqBdNo.B2, 0) },
//            {(AcqBdNo.B3, 0), (AcqBdNo.B3, 1) },
//            {(AcqBdNo.B3, 1), (AcqBdNo.B2, 1) },
//            {(AcqBdNo.B4, 0), (AcqBdNo.B5, 0) },
//            {(AcqBdNo.B4, 1), (AcqBdNo.B4, 0) },
//            {(AcqBdNo.B5, 0), (AcqBdNo.B5, 1) },
//            {(AcqBdNo.B5, 1), (AcqBdNo.B4, 1) },
//            {(AcqBdNo.B6, 0), (AcqBdNo.B7, 0) },
//            {(AcqBdNo.B6, 1), (AcqBdNo.B6, 0) },
//            {(AcqBdNo.B7, 0), (AcqBdNo.B7, 1) },
//            {(AcqBdNo.B7, 1), (AcqBdNo.B6, 1) },

//        };

//        #region 获取数据

//        /// <summary>
//        /// 获取相位数据
//        /// </summary>
//        /// <param name="define"></param>
//        /// <returns></returns>
//        private Dictionary<String, List<WaveOffsetGainPhase>> GetPhaseData(AcqModeAndInterleaveDefine define, String nowCaliTimes)
//        {
//            Int32 sgfrequencybyhz = 100;//单位MHz
//            Double samplingrate = 10_000;//采样间隔，单位Sps
//            Dictionary<String, List<WaveOffsetGainPhase>> waveoffsetgainphases = new Dictionary<String, List<WaveOffsetGainPhase>>();
//            String fintkey = String.Empty;
//            for (Int32 i = 0; i < 1; i++)
//            {
//                var adcdata = new List<List<ushort>>();
//                AcqWaveDataEx(out adcdata, 100);
//                if (adcdata[0].ToArray().Length <= 4)
//                {
//                    PrintTiAdcCaliLog($"取Adc数据异常！");
//                    CaliStatus = false;
//                    return waveoffsetgainphases;
//                }
//                #region 调试代码
//                using (StreamWriter sw = new StreamWriter($"./log/C1_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[0].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C2_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[1].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C3_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[2].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C4_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[3].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C5_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[4].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C6_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[5].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C7_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[6].ToArray()));
//                }
//                using (StreamWriter sw = new StreamWriter($"./log/C8_{nowCaliTimes}_{DateTime.Now.ToFileTime()}.txt", true))
//                {
//                    sw.WriteLine(String.Join(Environment.NewLine, adcdata[7].ToArray()));
//                }
//                #endregion
//                foreach (var item in define.Details)
//                {
//                    foreach (var eachinitem in item.Value)
//                    {
//                        foreach (var adc in eachinitem.AdcPorts)
//                        {
//                            fintkey = define.Name + "_" + item.Key + "_" + eachinitem.AcqBdNo + "_Adc" + adc.Key;
//                            //当前两个采集板，固定通道1、2是采集板1，3、4是采集板2
//                            //Int32 boardid = (item.Key) switch
//                            //{
//                            //    ChannelId.C1 => 0,
//                            //    ChannelId.C2 => 0,
//                            //    _ => 1,
//                            //};

//                            Int32 boardid = (item.Key) switch
//                            {
//                                ChannelId.C1 => 0,
//                                ChannelId.C2 => 1,
//                                ChannelId.C3 => 2,
//                                ChannelId.C4 => 3
//                            };
//                            Int32 index = boardid * Constants.ADC_NUM * item.Value.Length + adc.Key;
//                            switch (fintkey)
//                            {
//                                case "All-20G_C1_B0_Adc0": index = 0; break;
//                                case "All-20G_C1_B0_Adc1": index = 1; break;
//                                case "All-20G_C1_B1_Adc0": index = 2; break;
//                                case "All-20G_C1_B1_Adc1": index = 3; break;

//                                case "All-20G_C2_B2_Adc0": index = 4; break;
//                                case "All-20G_C2_B2_Adc1": index = 5; break;
//                                case "All-20G_C2_B3_Adc0": index = 6; break;
//                                case "All-20G_C2_B3_Adc1": index = 7; break;

//                                case "All-20G_C3_B4_Adc0": index = 8; break;
//                                case "All-20G_C3_B4_Adc1": index = 9; break;
//                                case "All-20G_C3_B5_Adc0": index = 10; break;
//                                case "All-20G_C3_B5_Adc1": index = 11; break;

//                                case "All-20G_C4_B6_Adc0": index = 12; break;
//                                case "All-20G_C4_B6_Adc1": index = 13; break;
//                                case "All-20G_C4_B7_Adc0": index = 14; break;
//                                case "All-20G_C4_B7_Adc1": index = 15; break;
//                                default:
//                                    break;
//                            }

//                            var data = adcdata[index];
//                            if (data != null && data.Count > 0)
//                            {
//                                Int32 startindex = data.Count / 10;
//                                UInt16[] datas = data.GetRange(startindex, data.Count - startindex * 2).ToArray();
//                                if (waveoffsetgainphases.Keys.Contains(fintkey))
//                                {
//                                    waveoffsetgainphases[fintkey].Add(SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz));
//                                }
//                                else
//                                {
//                                    waveoffsetgainphases.Add(fintkey, new List<WaveOffsetGainPhase>() { SineFitFunc.SineFit(datas.ToArray(), samplingrate, sgfrequencybyhz) });
//                                }
//                            }
//                        }
//                    }

//                }
//            }

//            return waveoffsetgainphases;
//        }

//        #endregion

//        private class AdcDelta
//        {
//            //20G 0:B0 1:A1 2:B0 3:A1
//            //10G 0:A1 1:A0 2:A0 3:A1
//            public TiadcPhaseOffsetGainItem_Base TiadcItem
//            {
//                get
//                {
//                    return ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(ItemKey!)!.Value;
//                }
//            }
//            public ProductDataTranslate_MSO8000X.TiadcParamsKeyMapWithBoard ItemKey;
//            public AcqBdNo AcqBdNo;
//            public Int32 ChanelIndex;
//            public Int32 ModelIndex;
//            public UInt32 AdcIndx;
//            public Int32 Index;//adc索引
//            public Double Delta;//相位差
//            public Int32 FpgaDelayer;//丢点
//            public Int32 CaliDelta;
//            public AdcInterleaveMode AdcInterleaveMode;
//            // 数据、偏差
//            private List<KeyValuePair<Int32, Double>> _RegErrPairs = new List<KeyValuePair<Int32, Double>>();

//            public void AddVaule(KeyValuePair<Int32, Double> infoValue)
//            {
//                _RegErrPairs.Add(infoValue);
//            }

//            /// <summary>
//            /// 斜率
//            /// </summary>
//            public Double rate = 300;

//            public void calc()
//            {
//                if (_RegErrPairs != null && _RegErrPairs.Count > 1)
//                {
//                    var param = _RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value;
//                    if (param == 0)
//                    {
//                        if (Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) >= 2 && Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) < 3)
//                        {
//                            rate = 10;
//                        }
//                        else if (Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) < 2)
//                        {
//                            rate = 5;
//                        }
//                        else
//                        {
//                            rate = 100;
//                        }
//                    }
//                    else
//                    {
//                        Double rateparam = (_RegErrPairs[_RegErrPairs.Count - 1].Key - _RegErrPairs[_RegErrPairs.Count - 2].Key) / (_RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value);
//                        if (rateparam == rate)
//                        {
//                            rate /= 2;
//                        }
//                        else
//                        {
//                            rate = rateparam;
//                        }
//                    }
//                }
//                else
//                {
//                    rate = ChanelIndex == 3 || ChanelIndex == 2 ? 100 : 300;
//                    rate = 100;
//                    //if (Math.Abs(Delta) <10)
//                    //{
//                    //    rate = 20;
//                    //}
//                    if (Math.Abs(Delta) < 5)
//                    {
//                        rate = 10;
//                    }
//                    if (Math.Abs(Delta) < 2.5)
//                    {
//                        rate = 5;
//                    }
//                }
//                rate = Math.Abs(rate);
//                rate = rate > 200 ? 200 : rate;
//                rate = rate <= 3 ? 3 : rate;
//                CaliDelta = (Int32)(rate * Delta);
//                CaliDelta = CaliDelta >= 3000 ? 3000 : CaliDelta;
//            }
//        }
//    }

//}
