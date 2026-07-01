using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Xml;
using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        public Dictionary<ChannelId, Int32> OffsetBy10mv_Dic = new Dictionary<ChannelId, int>();
        /// <summary>
        /// tiAdc自动校准
        /// </summary>
        internal void TiAdcOffsetAndGainCali_Exec(Boolean is10mv = false)
        {
            //modify by lhj with 7000HD
            if (Hd.CurrProductType == ProductType.JiHe_MSO7000HD)
            {
                return;
            }

            try
            {
                OffsetBy10mv_Dic = new Dictionary<ChannelId, int>();

                AcqModeAndInterleaveDefine define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;//获取当前采样模式
                String interleavename = define.Name.ToString();
                CaliStateManager calistatemanager = new CaliStateManager(define.Details.Count, 4);
                List<ChannelId> channelids = new List<ChannelId>();
                foreach (var adcuserinfo in define.Details)
                {
                    channelids.Add(adcuserinfo.Key);
                  
                    foreach (var adc in adcuserinfo.Value)
                    {
                        foreach (var adcport in adc.AdcPorts)
                        {
                            TiadcParamsKeyMapWithBoard itemKey = new(interleavename, adcuserinfo.Key, adc.AcqBdNo, (uint)adcport.Key);
                            TiadcPhaseOffsetGainItem_Base currItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemKey)!.Value;
                            //currItem.Gain = 40960;
                       
                            if (is10mv)
                            {
                                currItem.Offset_FPGA_10mv = 100;
                            }
                            else
                            {
                                currItem.Offset_FPGA = 100;
                            }
                          
                            ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(itemKey, currItem);
                        }
                    }
                }
                //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                TiAdc_ApplyAdc_Offset();
                Thread.Sleep(500);
                Double inputsignalfreqbymhz = 100;
                Double samplebym_sps = 10000;
                Double times=0;
                while (!calistatemanager.IsAllCompleted()&& times < 10)
                //while (false)
                {
                    //InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
                   

                    //获取多次数据做分析
                    List<List<UInt16[]>> datas = new List<List<UInt16[]>>();
                    for (Int32 i = 0; i <7; i++)
                    {
                        var adcdata = new List<List<UInt16>>();
                        AcqWaveDataEx(out adcdata);
                        if (adcdata == null)
                        {
                            //LogInfo($"(ExecuteId={_UniqueId};)采集数据错误!");
                            break;
                        }
                        List<UInt16[]> addata = new List<UInt16[]>();
                        foreach (var item in adcdata)
                        {
                            addata.Add(item.ToArray());
                        }
                        datas.Add(addata);
                        Thread.Sleep(500);
                    }

                    DataManager dataManager = new DataManager(datas, samplebym_sps, inputsignalfreqbymhz);

                    //下发值校准
                    for (var i = 0; i < channelids.Count; i++)
                    {
                        ChannelId channelId = channelids[i];
                        Int32 channelIndex = (Int32)i;
                        var currentDefineDetail = define.Details[channelId];
                        if (currentDefineDetail == null)
                            continue;

                        //校准gain,phase
                        if (calistatemanager.IsChnlCompleted(channelIndex))
                            continue;
                        foreach (var item in currentDefineDetail)
                        {
                            foreach (var adc in item.AdcPorts)
                            {
                                if (calistatemanager.IsAdcCompleted(channelIndex, (((Int32)item.AcqBdNo) % 2) * 2 + adc.Key))
                                    continue;
                                StringBuilder caliMsg = new StringBuilder();
                                TiadcParamsKeyMapWithBoard itemKey = new(interleavename, channelId, item.AcqBdNo, (uint)adc.Key);
                                TiadcPhaseOffsetGainItem_Base currItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemKey)!.Value;

                                var caliStateGain = calistatemanager.GetCaliState(channelIndex, (((Int32)item.AcqBdNo) % 2) * 2 + adc.Key, CaliStateManager.CaliItem.Gain);
                                caliStateGain.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;
                                //if (!caliStateGain.IsItemCompleted())
                                //    CaliGain(caliStateGain, dataManager, ref currItem, caliMsg, currentDefineDetail.First(), adc.Key);

                                var caliStateOffset = calistatemanager.GetCaliState(channelIndex, (((Int32)item.AcqBdNo) % 2) * 2 + adc.Key, CaliStateManager.CaliItem.Offset);
                                if (!caliStateOffset.IsItemCompleted())
                                    CaliOffset(caliStateOffset, dataManager, ref currItem, caliMsg, item, adc.Key,(Int32)channelId);

                                //var caliStatePhase = caliStateManager.GetCaliState(channelIndex, acqUnitIndex, CaliStateManager.CaliItem.Phase);
                                //if (!caliStatePhase.IsItemCompleted())
                                //    CaliPhase(caliStatePhase, dataManager, ref currItem, caliMsg, currentDefineDetail.First(), acqUnitIndex);
                                //更新寄存器值
                                ProductDataTranslate_MSO8000X.SetTiadcParamsItemWithBoard(itemKey, currItem);
                                //TiAdcPhaseOffsetGain_MSO8000X.Default.SetItem(currModeDefIndex, currChnlDefIndex, acqUnitIndex, currItem);
                            }
                        }

                        //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                        if (is10mv)
                        {
                            TiAdc_ApplyAdc_Offset(true);
                        }
                        else
                        {
                            TiAdc_ApplyAdc_Offset();
                        }
                      
                        Thread.Sleep(500);
                    }

                        times++;

                }
                if (!is10mv)
                {
                    //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                    TiAdc_ApplyAdc_Offset();
                    //保存校准数据
                    (TiadcPhaseOffsetGainParams.Default as ICaliData).SaveToFile();
                }
                else
                {
                    //foreach (var channelId in channelids)
                    //{
                    //    var currentDefineDetail = define.Details[channelId];
                    //    foreach (var item in currentDefineDetail)
                    //    {
                    //        TiadcParamsKeyMapWithBoard itemc1key = new(interleavename, channelId, item.AcqBdNo, 0);
                    //        var ParamsItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemc1key);
                    //        int offset = ParamsItem != null ? ParamsItem.Value.Offset_FPGA_10mv : 0;
                    //        //OffsetBy10mv_Dic.Add(channelId, offset);

                    //        TiadcPhaseOffsetGainItem_Base currItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemc1key)!.Value;
                    //        currItem.Offset_FPGA_10mv = offset;
                    //    }

                    //}
                    //(TiadcPhaseOffsetGainParams.Default as ICaliData).LoadFromFile();
                    ////Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                    //TiAdc_ApplyAdc_Offset(true);

                    TiAdc_ApplyAdc_Offset(true);
                    //保存校准数据
                    (TiadcPhaseOffsetGainParams.Default as ICaliData).SaveToFile();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void TiAdc_ApplyAdc_Offset(bool is10mv = false)
        {
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain Start!");
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interdefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            stopwatch2.Start();
            foreach (var dtl in interdefine.Details)
            {
                var adcinfo = dtl.Value[0];
                foreach (var item in dtl.Value)
                {
                    adcinfo = item;
                    var usedadcs = analogAcquireModel.GetUsedAdcs(adcinfo);
                    foreach (var adcId in usedadcs)
                    {
                        var tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(new(interdefine.Name, dtl.Key, item.AcqBdNo, adcId % 2))!.Value;
                        UInt32 offset = is10mv ? (UInt32)tiadcItem.Offset_FPGA_10mv : (UInt32)tiadcItem.Offset_FPGA;
                        //uint uintPosition = (uint)((UInt32)tiadcItem.Offset_FPGA * 16);
                        uint uintPosition = (uint)((UInt32)offset * 16);
                            if (((Int32)adcId % 2) == 1)
                        {
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreA, item.AcqBdNo, uintPosition);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreB, item.AcqBdNo, uintPosition);
                        }
                        else
                        {
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreC, item.AcqBdNo, uintPosition);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreD, item.AcqBdNo, uintPosition);
                        }
                    }
                }

            }
            stopwatch2.Stop();
            var sss = stopwatch2.ElapsedMilliseconds;
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain End!");
        }
        /// <summary>
        /// 将C1C3_20G的配置拷贝C1-20G和C3-20G
        /// </summary>
        private void CopyC1C3_20GOffsetData()
        {
            //var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            //AcqModeAndInterleaveDefine define = analogacquiremodel.GetCurrentAcqModeInterleave()!;
            //List<TiadcParamsKeyMapWithBoard> tiadcparamskeymaps = new List<TiadcParamsKeyMapWithBoard>()
            //{
            //  new("C1-20G", (ChannelId.C1), 0),
            //  new("C1-20G", (ChannelId.C1), 1),
            //  new("C3-20G", (ChannelId.C3), 0),
            //  new("C3-20G", (ChannelId.C3), 1)
            //};

            //foreach (var item in tiadcparamskeymaps)
            //{
            //    TiadcParamsKeyMapWithBoard itemkey = new("All-20G", item.chnlId, item.adcId);
            //    TiadcPhaseOffsetGainItem_Base tiadcfineitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(itemkey)!.Value;//20G参数
            //    TiadcPhaseOffsetGainItem_Base tiadcitem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(item)!.Value;//20G参数
            //    tiadcitem.Offset_FPGA = tiadcfineitem.Offset_FPGA;
            //    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcitem);
            //}
        }

        private void CaliGain(CaliStateManager.CaliState caliState, DataManager dataManager, ref TiadcPhaseOffsetGainItem_Base currItem
     , StringBuilder caliMsg, AdcUsedInfo currentDefineDetail, Int32 acqUnitIndex)
        {
            var fixedacqunitinfo = 1;
            fixedacqunitinfo += currentDefineDetail.AcqBdNo == AcqBdNo.B1 ? 2 : 0;
            var currentacqunitinfo = currentDefineDetail.AcqBdNo == AcqBdNo.B1 ? 2 : 0;
            currentacqunitinfo += acqUnitIndex;

            Double avgErrGain = dataManager.GetAvgErrGain(currentacqunitinfo, fixedacqunitinfo);
            String msg = String.Empty;/* $"Gain Calibration(caliTimes:{caliState.GetCaliCount()}): 0, {currentAcqUnitInfo}, {fixedAcqUnitInfo}, {currItem.Gain}, {avgErrGain}"*/ ;

            if (Math.Abs(avgErrGain) > 0.0005)//0.0005=>0.5
            {
                caliState.AddRegErr(currItem.Gain, avgErrGain);
                Int32 step = 1000;
                if (Math.Abs(avgErrGain) < 0.01)
                {
                    step = 100;
                }
                caliState.AdjustStep = (Int32)(step * 0.9 * (avgErrGain > 0 ? 1 : -1));
                currItem.Gain = caliState.CalculateReg();
                if (currItem.Gain <= 0 || Math.Abs(currItem.Gain) > 65535)
                {
                    caliState.CurrFlag = CaliStateManager.CaliState.Flag.Failed;
                }
            }
            else
                caliState.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;

            msg += $" - workFlag({caliState.CurrFlag})";
        }

        private void CaliOffset(CaliStateManager.CaliState caliState, DataManager dataManager, ref TiadcPhaseOffsetGainItem_Base currItem
           , StringBuilder caliMsg, AdcUsedInfo currentDefineDetail, Int32 acqUnitIndex,Int32  channelididx)
        {
            //var fixedacqunitinfo = 1;
            //fixedacqunitinfo += ((Int32)currentDefineDetail.AcqBdNo)*2;
            var fixedacqunitinfo = channelididx*4;
            var currentacqunitinfo = ((Int32)currentDefineDetail.AcqBdNo) * 2;
            currentacqunitinfo += acqUnitIndex;

            Double avgerroffset = dataManager.GetAvgErrOffset(currentacqunitinfo, fixedacqunitinfo);
            String msg = String.Empty;/* $"Offset Calibration(caliTimes:{caliState.GetCaliCount()}): 0, {currentAcqUnitInfo}, {fixedAcqUnitInfo}, {currItem.Offset_FPGA}, {avgErrOffset}";*/
            //AppendCaliLog($"Offset Calibration(caliTimes:{caliState.GetCaliCount()}): 0, CH:{currentacqunitinfo}, {fixedacqunitinfo}, Offset_FPGA:{currItem.Offset_FPGA}, avgErrOffset:{avgerroffset}");
            System.Diagnostics.Trace.WriteLine($"Offset Calibration(caliTimes:{caliState.GetCaliCount()}): 0, CH:{currentacqunitinfo}, {fixedacqunitinfo}, Offset_FPGA:{currItem.Offset_FPGA}, avgErrOffset:{avgerroffset}");
            if (Math.Abs(avgerroffset) > 2)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![channelididx];
                var scale = analogparas.ScaleBymV;
                if (scale<50)
                {
                    caliState.AddRegErr(currItem.Offset_FPGA_10mv, avgerroffset);
                    //caliState.AdjustStep = (Int32)(0.5 * avgerroffset) * (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? -1 : 1);
                    caliState.AdjustStep = (Int32)(1 * avgerroffset) * (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? -1 : 1);
                    currItem.Offset_FPGA_10mv = caliState.CalculateReg();
                    if (Math.Abs(currItem.Offset_FPGA_10mv) > 65535)
                    {
                        caliState.CurrFlag = CaliStateManager.CaliState.Flag.Failed;
                    }
                }
                else
                {
                    caliState.AddRegErr(currItem.Offset_FPGA, avgerroffset);
                    //caliState.AdjustStep = (Int32)(0.5 * avgerroffset) * (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? -1 : 1);
                    caliState.AdjustStep = (Int32)(1 * avgerroffset) * (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? -1 : 1);
                    currItem.Offset_FPGA = caliState.CalculateReg();
                    if (Math.Abs(currItem.Offset_FPGA) > 65535)
                    {
                        caliState.CurrFlag = CaliStateManager.CaliState.Flag.Failed;
                    }
                }

             
            }
            else
            {
                caliState.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;
            }
            AppendCaliLog($" workFlag({caliState.CurrFlag})");
        }

    }
}
