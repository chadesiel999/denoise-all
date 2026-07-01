using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal class AbstractController_Decoder
    {
        protected Dictionary<SerialProtocolType, MethodInfo?> protocolConfigActionList = new Dictionary<SerialProtocolType, MethodInfo?>();
        public static void Config() => Hd.CurrProduct?.Ctrl_Decoder?.DoConfig();

        public static void DisableDecode() => Hd.CurrProduct?.Ctrl_Decoder?.DoDisableDecode();

        public void DoDisableDecode()
        {
            var trigType = Hd.UIMessage?.Trigger?.TrigType;
            if (trigType != TriggerType.Serial || Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType != SerialProtocolType.Close)
                return;
            
 //           Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, (UInt32)SerialProtocolType.Close);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.Close);//切换
        }

        public void DoConfig()
        {
            var trigType = Hd.UIMessage?.Trigger?.TrigType;
            SerialProtocolType protocolType = Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType;
            if (trigType != TriggerType.Serial || Hd.UIMessage!.Analog == null || Hd.UIMessage!.Trigger!.TrigDecoder == null || Hd.UIMessage.Decoder == null)
            {    //关闭协议触发
 //                       Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, (UInt32)SerialProtocolType.Close);
                HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, 0);
                return;
            }
            else if (protocolType == SerialProtocolType.Close)
            {
                //关闭协议触发
//                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, (UInt32)SerialProtocolType.Close);
                HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, 0);
                return;
            }
            else
            {

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 1);
                if (protocolType != SerialProtocolType.Close)
                {
                    if (protocolConfigActionList.ContainsKey(protocolType))
                    {
                        if (protocolType == SerialProtocolType.USB || protocolType == SerialProtocolType.SATA || protocolType == SerialProtocolType.PCIe)//采集板实现
                        {
                            //comment At 2023.06.01 Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (uint)protocolType);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (uint)protocolType);
                            //comment At 2023.06.01 Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigTypeSelect, (uint)protocolType);
                            //comment At 2023.06.01 HdIO.WriteReg((uint)AcqBdReg.W.Decoder_RamResetEnable, 0);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 0);              
                            //comment At 2023.06.01 HdIO.Sleep(1);
                            //comment At 2023.06.01 HdIO.WriteReg((uint)AcqBdReg.W.Decoder_RamResetEnable, 1);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 1);
                        }
                        else
                        {
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (uint)protocolType);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (uint)protocolType);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 0);
                            //comment At 2023.06.01 HdIO.Sleep(1);
                            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 1);
                        }
                        //打开的模拟通道，使能数据发送到触发模块
                        UInt16 opench = 0;

                        for (Int32 i = 0, l = Hd.UIMessage!.Analog!.Length; i < l; i++)
                        {
                            opench |= (UInt16)((Hd.UIMessage!.Analog[i].Active ? 1 : 0)<<i);
                        }

                        HdIO.WriteReg(ProcBdReg.W.Decoder_B1Type, opench);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, 1);
                        protocolConfigActionList[Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType!]?.Invoke(null, null);
                    }
                }
            }

            if (Hd.UIMessage?.Trigger?.TrigDecoder != null && Hd.UIMessage.Trigger.TrigDecoder.ProtocolType != SerialProtocolType.Close)
            {
                var thresholds = Hd.UIMessage.Trigger.TrigDecoder.ProtocolOptions?.GetThresholdInfos();
                if (Hd.UIMessage!.Decoder![0].Active)
                {
                    thresholds = Hd.UIMessage!.Decoder![0].ProtocolOptions?.GetThresholdInfos();
                }
                if (thresholds == null || thresholds.Count == 0) return;
                List<(AcqBdReg.W, AcqBdReg.W)> regs = new List<(AcqBdReg.W, AcqBdReg.W)>()
                {
//#if JiHe_MSO7000X
                (AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2,AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1),
                (AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level2,AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level1),
                (AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level2,AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level1),
                (AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level2,AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level1)
//#endif
                };
                for (int index = 0; index < regs.Count; index++)
                {

                    double threshold = 0;
                    int thindex = -1;
                    if (thresholds.Count > 0) thindex = thresholds.FindIndex(x => x.ChannelId == ChannelId.C1 + index);
                    if (thindex >= 0)
                    {
                        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)thresholds[thindex].ChannelId);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)EdgeSlope.Rise);
                        if (thresholds[thindex].Threshold != null && thresholds[thindex].Threshold.Length > 0)
                        {
                            threshold = thresholds[thindex].Threshold[0] / Hd.UIMessage!.Analog![index].ProbeGain;
                            UInt32 pos = Math.Clamp((UInt32)(Constants.SAMPS_PER_YDIV * (threshold * 1000 + Hd.UIMessage.Analog[index].Position) / Hd.UIMessage.Analog[index].Scale + AbstractController_Trigger.AdcCenterValue), 48, 4047);
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(regs[index].Item1, pos);

                            if (thresholds[thindex].Threshold.Length > 1)
                            {
                                threshold = thresholds[thindex].Threshold[1] / Hd.UIMessage!.Analog![index].ProbeGain;
                            }
                            else
                            {
                                threshold = 0;//如果没有则默认为0
                            }

                            pos = Math.Clamp((UInt32)(Constants.SAMPS_PER_YDIV * (threshold * 1000 + Hd.UIMessage.Analog[index].Position) / Hd.UIMessage.Analog[index].Scale + AbstractController_Trigger.AdcCenterValue), 48, 4047);
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(regs[index].Item2, pos);
                        }
                    }
                }
            }

            return;
            foreach (var decodeoption in Hd.UIMessage!.Decoder!)
            {
                if (decodeoption.Active && decodeoption.ProtocolType != SerialProtocolType.Close)
                {
                    if (protocolConfigActionList.ContainsKey(decodeoption.ProtocolType))
                    {
                        if (decodeoption.ProtocolType == SerialProtocolType.USB || decodeoption.ProtocolType == SerialProtocolType.SATA
                            || decodeoption.ProtocolType == SerialProtocolType.PCIe)//采集板实现
                        {
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamResetEnable, 0);
                            HdIO.Sleep(1);
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamResetEnable, 1);
                        }
                        protocolConfigActionList[decodeoption.ProtocolType]?.Invoke(null, null);

                        //设置比较电平
                        List<(ChannelId ChannelId, Double[] Threshold)>? ThresholdInfos = decodeoption.ProtocolOptions!.GetThresholdInfos();
                        if (ThresholdInfos.Count > 0)
                        {
                            ChannelId lastChannelID = ThresholdInfos[0].ChannelId;
                            foreach (var info in ThresholdInfos)
                            {
                                if(info.Threshold==null||info.Threshold.Length<=0)
                                    continue;

                                ChannelId currChannel = info.ChannelId;
                                if ((int)currChannel < ChannelIdExt.AnaChnlNum)
                                {
                                    var acqBd = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetChannelAcqBdAdcInputCorresponding((int)currChannel);
                                    if (acqBd != null)
                                    {
                                        double voltage = info.Threshold[0];
                                        double compVoltage = (Hd.UIMessage!.Analog![(int)currChannel].Position + voltage * 1000) / Hd.UIMessage!.Analog[(int)currChannel].Scale * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                                        int up = (int)(compVoltage + 50);
                                        int dn = (int)(compVoltage - 50);
                                        if (up < 0)
                                            up = 0;
                                        else if (up > (Constants.MAX_ADC_RES / 2 - 1))
                                            up = Constants.MAX_ADC_RES / 2 - 1;
                                        if (dn < 0)
                                            dn = 0;
                                        else if (dn > (Constants.MAX_ADC_RES / 2 - 1))
                                            dn = Constants.MAX_ADC_RES / 2 - 1;
                                        //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decoder_protocolcmplevelH1, acqBd.BdNo, (uint)up);
                                        //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decoder_protocolcmplevelL1, acqBd.BdNo, (uint)dn);

                                        if (decodeoption.ProtocolType == SerialProtocolType.ARINC429&&info.Threshold.Length>1)
                                        {
                                            double voltage2 = info.Threshold[1];
                                            double compVoltage2 = (Hd.UIMessage!.Analog[(int)currChannel].Position + voltage2 * 1000) / Hd.UIMessage!.Analog[(int)currChannel].Scale * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                                            int up2 = (int)(compVoltage2 + 50);
                                            int dn2 = (int)(compVoltage2 - 50);
                                            if (up2 < 0)
                                                up2 = 0;
                                            else if (up2 > (Constants.MAX_ADC_RES / 2 - 1))
                                                up2 = Constants.MAX_ADC_RES / 2 - 1;
                                            if (dn2 < 0)
                                                dn2 = 0;
                                            else if (dn2 > (Constants.MAX_ADC_RES / 2 - 1))
                                                dn2 = Constants.MAX_ADC_RES / 2 - 1;

                                            //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decoder_protocolcmplevelH2, acqBd.BdNo, (uint)up2);
                                            //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decoder_protocolcmplevelL2, acqBd.BdNo, (uint)dn2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }
    }
}
