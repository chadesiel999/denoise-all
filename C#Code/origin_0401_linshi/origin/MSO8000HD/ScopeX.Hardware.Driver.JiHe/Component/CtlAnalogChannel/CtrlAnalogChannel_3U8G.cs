using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class CtrlAnalogChannel_3U8G : CtrlAnalogChannel_JiHe2d5G
    {
        /// <summary>
        /// 阻抗补偿系数
        /// </summary>
        private static Dictionary<(ChannelId ChId, AnaChnlScaleIndex Scale), (UInt16 Fgp, UInt16 Fdp)>? ImpCompensateDacSetting;
        
        /// <summary>
        /// 初始化阻抗补偿系数
        /// </summary>
        private static void InitImpCompensateDacSetting()
        {
            //ImpCompensateDacSetting初始化
            String settingfilepath = @"CaliData\ImpCompensateDacSetting.json";
            ImpCompensateDacSetting = new Dictionary<(ChannelId, AnaChnlScaleIndex), (UInt16, UInt16)>();
            try
            {
                using (FileStream fs = new FileStream(settingfilepath, FileMode.Open, FileAccess.Read))
                {
                    JsonDocument jd = JsonDocument.Parse(fs);
                    JsonElement root = jd.RootElement;
                    //通道循环
                    foreach (var chid in ChannelIdExt.GetAnalogs())
                    {
                        JsonElement chelement = root.GetProperty(chid.ToString());
                        //挡位循环
                        for (int scale = (Int32)AnaChnlScaleIndex.Lv10m; scale <= (Int32)AnaChnlScaleIndex.Lv1; scale++)
                        {
                            var scaleenum = (AnaChnlScaleIndex)scale;
                            JsonElement scaleelement = chelement.GetProperty(scaleenum.ToString());
                            String fgp = scaleelement.GetProperty("Fgp").GetString()!;
                            String fdp = scaleelement.GetProperty("Fdp").GetString()!;
                            UInt16 fgpuint16 = UInt16.Parse(fgp.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            UInt16 fdpuint16 = UInt16.Parse(fdp.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            ImpCompensateDacSetting.Add((chid, scaleenum), (fgpuint16, fdpuint16));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Hd.SysLogger?.Invoke(ex.Message,"Warning");
            }
        }

        /// <summary>
        /// 获取指定通道，指定垂直挡位的阻抗补偿系数
        /// </summary>
        /// <param name="chId"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static (UInt16 Fgp, UInt16 Fdp) GetImpCompensateDacSetting(ChannelId chId, AnaChnlScaleIndex scale)
        {
            if (ImpCompensateDacSetting == null)
                InitImpCompensateDacSetting();
            (UInt16, UInt16) ret = (0x0800, 0x2200);//默认值
            foreach (var setting in ImpCompensateDacSetting!)
            {
                if(setting.Key.ChId == chId && setting.Key.Scale == scale)
                {
                    ret = setting.Value; 
                    break;
                }
            }
            return ret;
        }

        #region RegFPGA_ChnlCtrl
        private static Dictionary<ChannelId, Int32[]> acqbddefine = new()
        {
            [ChannelId.C1] =new int[2]{ 0, 1 },
            [ChannelId.C2] = new int[2] { 2,3 },
            [ChannelId.C3] = new int[2] { 4,5 },
            [ChannelId.C4] = new int[2] { 6, 7 },
        };

        private const UInt32 inputImpedance = 0;//调节输入阻抗
        private const UInt32 offsetVoltage = 1;//调节偏置电压  

        static AnalogChannelDacPortInfo[,] dacPortCtrlInfo_FD =
        {   //FD1      FD2
            {new(7,7),new(7,6)},  //ch4
            {new(7,2),new(7,3)},  //ch3
            {new(6,7),new(6,6)},  //ch2
            {new(6,2),new(6,3)},  //ch1
        };
        //+14dB通路需要额外配置FG
        static AnalogChannelDacPortInfo[,] dacPortCtrlInfo_FG =
        {   //FG1     FG2   
            {new(7,5),new(7,4)},  //ch4
            {new(7,0),new(7,1)},  //ch3
            {new(6,5),new(6,4)},  //ch2
            {new(6,0),new(6,1)},  //ch1
        };

        private static void SendConfigDataToDAC_FD(UInt32 ctrlWords, UInt32 channelId, UInt32 ctrlType)
        {
            short dacIndex = (Int16)((dacPortCtrlInfo_FD[channelId, ctrlType].dacIndex) | 0x10);
            short portIndex = dacPortCtrlInfo_FD[channelId, ctrlType].portIndex;
            SendDataTo5668(dacIndex, (uint)portIndex, ctrlWords);
        }
        private static void SendConfigDataToDAC_FG(UInt32 ctrlWords, UInt32 channelId, UInt32 ctrlType)
        {
            short dacIndex_FG = (Int16)((dacPortCtrlInfo_FG[channelId, ctrlType].dacIndex) | 0x10);
            short portIndex_FG = dacPortCtrlInfo_FG[channelId, ctrlType].portIndex;
            SendDataTo5668(dacIndex_FG, (uint)portIndex_FG, ctrlWords);
        }

        private static void SendConfigDataToDSA(UInt32 ctrlWords, UInt32 channelId)
        {
            UInt32 EnableWord = 0x0008;
            EnableWord = EnableWord >> (int)channelId;
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0X10);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_reg_ch_dsa_data_8g, ctrlWords);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, EnableWord);
            Thread.Sleep(5);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_CH_DSA_EN, 0x0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0);
        }



        private static void CtrlOffsetBias()
        {
            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog[channelindex];
              
                (Int32 ScaleIndex, Int32 ScaleValueByuV, Int32 GainFineByFpgaThousand) bestscaleindexfpgafine = GetCurrentScaleIndex(channelindex);

                Int32 impedance_h_is0 = 1;// analogparas.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                Int32 yscaleindex = bestscaleindexfpgafine.ScaleIndex;
                AnalogChannelItem_Base chnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelindex, impedance_h_is0 == 0, (UInt32)(bestscaleindexfpgafine.ScaleValueByuV / 1000)))!.Value;

                ///获取衰减挡位
                Int32 att = 2;//默认为衰减档
                if (yscaleindex <= (Int32)AnaChnlScaleIndex.Lv20m)
                {
                    //放大档
                    att = 1;
                }
                else if (yscaleindex <= (Int32)AnaChnlScaleIndex.Lv100m)
                {
                    //直通档
                    att = 0;
                }
                if (Hd.CurrDebugVarints.bEnable_OpenCrystal)
                    att = 3;

                ///DAC控制
                //说明：1）当前调整位移，偏执只用设置一个DAC(0div由通道参数Offset指定，斜率由通道参数Offset_Pos3Div指定)；
                //         另一个DAC为定值(值由通道参数Bias指定)；
                //      2）放大档，设置DAC使用fgn；fdn为定值；
                //      3）衰减档,直通档，设置DAC使用fdn；fgn为定值；
                Int32 offpos = (Int32)(Constants.SAMPS_PER_YDIV * (analogparas.Position) * 1000 / bestscaleindexfpgafine.ScaleValueByuV);
                Int32 biaspos = (Int32)(Constants.SAMPS_PER_YDIV * (double)analogparas.Bias / bestscaleindexfpgafine.ScaleValueByuV);
                Int32 offbias = 0;
                double positionLimit = 3.8;

                var autocaliparams = AutoCaliParams.Default![channelindex, impedance_h_is0, yscaleindex];

                if ((offpos - biaspos) > (Constants.SAMPS_PER_YDIV * positionLimit))//偏置设为-5.5div以外
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPreceding_N3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else if ((offpos - biaspos) > 0)//偏置设为负0~5.5div以内
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPosterior_3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else if ((offpos - biaspos) > -(Constants.SAMPS_PER_YDIV * positionLimit))//偏置设为0~5.5div以内
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPosterior_N3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else //偏置设为正5.5div以外
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPreceding_3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }

                Int32 fdn, fgn;
                //放大档
                if (att == 1)
                {
                    fgn = Math.Min((Math.Max(autocaliparams.OffsetPosterior + offbias, 0)), 65535);
                    fdn = Math.Min((Math.Max(chnlparams.Bias, 0)), 65535);
                }
                else
                {
                    //衰减档,直通档
                    fdn = Math.Min((Math.Max(autocaliparams.OffsetPosterior + offbias, 0)), 65535);
                    fgn = Math.Min((Math.Max(chnlparams.Bias, 0)), 65535);
                }

                //wcj: 8G通道V2，DAC调整效果是反的，这里反过来发，保证上层控制逻辑一致
                fgn = 65535 - fgn;
                fdn = 65535 - fdn;

                SendConfigDataToDAC_FG((uint)fgn, (UInt32)channelindex, offsetVoltage);
                SendConfigDataToDAC_FD((uint)fdn, (UInt32)channelindex, offsetVoltage);
               
            }
                //    //senddata.Add((Byte)(fgn & 0xff));
                //    //senddata.Add((Byte)(fgn >> 8 & 0xff));

                //    //senddata.Add((Byte)(fgp & 0xff));
                //    //senddata.Add((Byte)(fgp >> 8 & 0xff));

                //    //senddata.Add((Byte)(fdn & 0xff));
                //    //senddata.Add((Byte)(fdn >> 8 & 0xff));

                //    //senddata.Add((Byte)(fdp & 0xff));
                //    //senddata.Add((Byte)(fdp >> 8 & 0xff));
                //}

                ////********//////

                //for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
                //{
                //    HdMessage.AnalogOptions analogParameters = Hd.UIMessage.Analog[channelIndex];
                //    int Impedance_H_is0 = 1;//只有低阻 analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                //    int yScaleIndex = analogParameters.ScaleIndex;

                //    ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparams = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap((ChannelId)channelIndex, Impedance_H_is0 == 0 ? true : false, (UInt32)(analogParameters.ScaleBymV));
                //    var chnlparamsItems = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparams)!.Value;

                //    //用tool工具上的前级偏来调节基线
                //    UInt32 offsetctrlwords = 65535 - (UInt32)chnlparamsItems.Offset;
                //    UInt32 offsetctrlwords_fg = 65535 - (UInt32)chnlparamsItems.Reserved0; //没有OffsetPreceding参数用reverse补上空


                //AnalogChannelItem_Base chnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                //    new((ChannelId)channelIndex, false, (UInt32)(analogParameters.ScaleBymV )))!.Value;              
                //UInt32 offsetctrlwords = 65535 - ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
                //UInt32 offsetctrlwords_fg = 65535 - ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPreceding;
                //    UInt32 offsetctrlwords = ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
                //     UInt32 offsetctrlwords_fg = ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPreceding;


                //Double limitedPosition_uV = analogParameters.Scale * 1000 * 2;
                //Double UIBias = Hd.UIMessage?.Analog?[channelIndex].Bias ?? 0;//单位uV
                //Double posPosition = (Double)(analogParameters.Position * 1000);//单位uV?
                //Double bias = Math.Abs(posPosition) > limitedPosition_uV ? (posPosition > 0 ? (posPosition - limitedPosition_uV) : (posPosition + limitedPosition_uV)) : 0;//取大于n格以后的部分
                //posPosition -= bias;
                //Double TotalBias = UIBias + bias;
                //Double volt2DacCtrlWords;

                //if (TotalBias >= 0)
                //{
                //    volt2DacCtrlWords = chnlparamsItems.Bias_Pos3Div;
                //    //volt2DacCtrlWords = ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPreceding_3Div;
                //}
                //else
                //{
                //    volt2DacCtrlWords = chnlparamsItems.Bias_Neg3Div;
                //volt2DacCtrlWords = ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_3Div;
                //}
                //if (yScaleIndex > 8)//200mV以上时，两位小数。
                //{
                //    volt2DacCtrlWords = volt2DacCtrlWords / 100;
                //}
                //else
                //    volt2DacCtrlWords = volt2DacCtrlWords / 1000;
                //Double BiasCtrlWords;
                //if (yScaleIndex == 4 || yScaleIndex == 5 || yScaleIndex == 3)//14dB通路时(10~20mV档),FG的控制字从tool工具上的前级偏来控制
                //{
                //    UInt32 tmp = offsetctrlwords_fg;
                //    offsetctrlwords_fg = offsetctrlwords;
                //    offsetctrlwords = tmp;

                //    BiasCtrlWords = TotalBias / volt2DacCtrlWords;//从uV转成控制字的系数
                //    offsetctrlwords_fg -= (uint)BiasCtrlWords;
                //    SendConfigDataToDAC_FG(offsetctrlwords_fg, (UInt32)channelIndex, offsetVoltage);
                //}
                //else
                //{
                //    BiasCtrlWords = TotalBias / volt2DacCtrlWords;//从uV转成控制字的系数
                //    offsetctrlwords -= (uint)BiasCtrlWords;
                //}
                //SendConfigDataToDAC_FD(offsetctrlwords, (UInt32)channelIndex, offsetVoltage);
                //}
        }

        public static void CtrlOffset()
        {
            CtrlOffsetBias();
        }
        public static void CtrlBias()
        {
            CtrlOffsetBias();
        }

        internal static void CtrlGainByFpga()
        {

            for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![channelIndex];
                int Impedance_H_is0 = 1;//只有低阻 analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                int yScaleIndex = analogParameters.ScaleIndex;

                ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparamsGain = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap((ChannelId)channelIndex, Impedance_H_is0 == 0 ? true : false, (UInt32)(analogParameters.Scale));//ScaleBymV cij_0810
                var chnlItemsGain = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparamsGain)!.Value;

                uint gain_FineByFpgaThousand = (uint)(chnlItemsGain.Gain_FineByFpgaThousand * 1.0 / 1000f * 2048); //乘数什么意思？
                //uint gain_FineByFpgaThousand = (uint)(ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].Gain_FineByFpgaThousand * 1.0 / 1000f * 2048);

                //float fpgaGainBytxt = (float)(GetFPGAGain(channelIndex) * 1.0 / 1000f);
                //gain_FineByFpgaThousand = fpgaGainBytxt;
                //var FloatHighLowPair = AbstractAnalogChannel.Convert2HighLowShortPair(gain_FineByFpgaThousand);

                //        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_DigitalZoomEnable, acqbddefine[(ChannelId)channelIndex], 0x01);// dig_en  //CHAI
                //     Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_DigitalZoomNumH16, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.High);// num_H16  //CHAI
                //   Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_DigitalZoomNumL16, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.Low);// num_L16   //CHAI

                ////Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch1_H, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.High);
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, acqbddefine[(ChannelId)channelIndex], gain_FineByFpgaThousand);
                ////Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch2_H, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.High);
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, acqbddefine[(ChannelId)channelIndex], gain_FineByFpgaThousand);
                ////Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch3_H, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.High);
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, acqbddefine[(ChannelId)channelIndex], gain_FineByFpgaThousand);
                ////Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch4_H, acqbddefine[(ChannelId)channelIndex], FloatHighLowPair.High);
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, acqbddefine[(ChannelId)channelIndex], gain_FineByFpgaThousand);
                foreach (var item in acqbddefine[(ChannelId)channelIndex])
                {
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, (AcqBdNo)item, gain_FineByFpgaThousand);
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, (AcqBdNo)item, gain_FineByFpgaThousand);
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, (AcqBdNo)item, gain_FineByFpgaThousand);
                    Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, (AcqBdNo)item, gain_FineByFpgaThousand);

                }

            }
        }

        public static void CtrlGain()
        {
            for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![channelIndex];

                #region 调节点1：粗调增益,DSA
                int Impedance_H_is0 = 1;//只有低阻 analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                int yScaleIndex = analogParameters.ScaleIndex;

                ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparamsDSA = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap((ChannelId)channelIndex, Impedance_H_is0 == 0 ? true : false, (UInt32)(analogParameters.Scale));//ScaleBymV cij_0810
                var chnlItemsDSA = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparamsDSA)!.Value;

                int GainCtrlow = chnlItemsDSA.Gain;//gain的cw哪里算？直接用gain的参数可以吗？
                int GainCtrlhigh_14dB = chnlItemsDSA.Gain << 8;
                //int GainCtrlow = (Int32)ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].Gain_CoarseCtrlWord;
                //int GainCtrlhigh_14dB = (Int32)ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, yScaleIndex].Gain_CoarseCtrlWord << 8;
                int GainCtr = GainCtrlow | GainCtrlhigh_14dB;
                SendConfigDataToDSA((UInt32)GainCtr, (UInt32)channelIndex);
                #endregion
            }
            AnalogChannelMiscSet();
            #region 调节点2：Adc_Fine
            //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
            #endregion

            #region 调节点3：FPGA_Fine
            CtrlGainByFpga();
            #endregion
            CtrlGainByFpgaUpo();

        }
        private static void GeneratedAndSend4094CtrlWord()
        {
            UInt32 hc595Bits = 0;
            List<AnaChnlScaleIndex> anaChnlScalesList = new List<AnaChnlScaleIndex>();
            for (int channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                if (Hd.UIMessage?.Analog?[(int)channelIndex] == null)
                    return;
                anaChnlScalesList.Add((AnaChnlScaleIndex)Hd.UIMessage.Analog[channelIndex].ScaleIndex);
            }

            bool retVal = GetSingnalConditionCtrlWords(anaChnlScalesList.ToArray(), ref hc595Bits);//anaChnlScalesList存的是当前四个通道的垂直档位。
            if (retVal)
            {
                //CIJ_0515
                Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(hc595Bits, 0xa << 8);

                if (Hd.Calibration.CaliStatus)
                {
                    Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
 //                   Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(hc595Bits, 0xa << 8);
                }
                else
                {
                    Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(hc595Bits, 0xa << 8);
                    if (Hd.CurrDebugVarints.bEnable_analog_signal == true)
                    {
                        Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
                    }
              //      Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);


                    //                Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
                }
                return;
            }
        }
        //通路选择40bit控制字。例：1C_00_00_00为通道四+14dB放大控制字。
        static UInt32[,] HC595CtrlInfo = {
            //  +14dB       0dB       NC       -20dB
            { 0x1c <<24, 0x08 <<24, 0x06 <<24, 0x12 <<24}, // ch4 
            { 0x1c <<16, 0x08 <<16, 0x06 <<16, 0x12 <<16}, // ch3
            { 0x1c << 8, 0x08 << 8, 0x06 << 8, 0x12 <<8 }, // ch2
            { 0x1c << 0, 0x08 << 0, 0x06 << 0, 0x12 <<0 }, // ch1
        };

        //垂直档位与通路选择对照表。
        private const UInt16 frst_GainCtrl_14dB = 0;    //14dB           
        private const UInt16 frst_GainCtrl_0dB = 1;     //0dB  直通                      
        private const UInt16 frst_GainCtrl_protect = 2; //过压保护,ref参考信号通路
        private const UInt16 frst_GainCtrl_neg20dB = 3; //-20dB
        static Dictionary<AnaChnlScaleIndex, UInt16> yScalTableTodB = new Dictionary<AnaChnlScaleIndex, UInt16>()
        {
            {AnaChnlScaleIndex.Lv1m,    frst_GainCtrl_14dB },    //10mV 
            {AnaChnlScaleIndex.Lv2m,    frst_GainCtrl_14dB },    //10mV 
            {AnaChnlScaleIndex.Lv5m,    frst_GainCtrl_14dB },    //10mV 
            {AnaChnlScaleIndex.Lv10m,   frst_GainCtrl_14dB },    //10mV 
            {AnaChnlScaleIndex.Lv20m,   frst_GainCtrl_14dB },    //20mV 
            {AnaChnlScaleIndex.Lv50m,   frst_GainCtrl_0dB },     //50mV 
            {AnaChnlScaleIndex.Lv100m,  frst_GainCtrl_0dB },     //100mV 
            {AnaChnlScaleIndex.Lv200m,  frst_GainCtrl_neg20dB }, //200mV 
            {AnaChnlScaleIndex.Lv500m,  frst_GainCtrl_neg20dB }, //500mV 
            {AnaChnlScaleIndex.Lv1,     frst_GainCtrl_neg20dB }, //1V 
        };
        static bool GetSingnalConditionCtrlWords(AnaChnlScaleIndex[] chnlScaleTable, ref UInt32 ctrlWords)
        {
            UInt32 signalContionTmp;
            ctrlWords = 0;
            for (int channelIndex = 0; channelIndex < chnlScaleTable.Length; channelIndex++)
            {
                if (!yScalTableTodB.ContainsKey(chnlScaleTable[channelIndex]))
                {
                    return false;
                }
                signalContionTmp = yScalTableTodB[chnlScaleTable[channelIndex]];

                if (signalContionTmp >= HC595CtrlInfo.GetLength(1) || channelIndex > HC595CtrlInfo.GetLength(0))
                {
                    return false;
                }

                ctrlWords |= HC595CtrlInfo[channelIndex, signalContionTmp];

                HdMessage.AnalogOptions analogParameters = Hd.UIMessage.Analog[channelIndex];
                int yScaleIndex = analogParameters.ScaleIndex;

                ProductDataTranslate_MSO8000X.ChnlParamsKeyMap chnlparams595 = new ProductDataTranslate_MSO8000X.ChnlParamsKeyMap((ChannelId)channelIndex, false, (UInt32)(analogParameters.Scale));////ScaleBymV cij_0810
                var chnlItems595 = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlparams595)!.Value;


                uint inputImpedence_fd = (uint)chnlItems595.Reserved1;
                uint inputImpedence_fg = (uint)chnlItems595.Reserved2;

                //uint inputImpedence_fd = ChannelParamsModel2.Default[(int)channelIndex, 1, (int)yScaleIndex].DCTrigZero;
                //uint inputImpedence_fg = ChannelParamsModel2.Default[(int)channelIndex, 1, (int)yScaleIndex].DCTrigZero_3Div;

                #region 新阻抗调节24-1-25
                SendConfigDataToDAC_FD(inputImpedence_fd, (UInt32)channelIndex, inputImpedance);
                //14dB通路需另配置FG
                if (signalContionTmp == frst_GainCtrl_14dB)
                {
                    SendConfigDataToDAC_FG(inputImpedence_fg, (UInt32)channelIndex, inputImpedance);
                }
                #endregion

            }
            return true;
        }


        static bool flag = true;

        #region 0617
        private enum DACIndex
        {
            DAC0, DAC1, DAC2, DAC3, DAC4, DAC5, DAC6, DAC7
        }

        static Dictionary<TriggerCoupling, UInt16> couplingCtrlWords = new Dictionary<TriggerCoupling, UInt16>
        {
            { TriggerCoupling.DC, 0b110 },
            { TriggerCoupling.AC, 0b100 },
            { TriggerCoupling.LFR, 0b010 },
            { TriggerCoupling.HFR, 0b001 },
            { TriggerCoupling.NR, 0b000 },
        };

        private static void WriteExtTrigDAC(byte dacAddress, ushort dacCtrlWord)
        {
            Thread.Sleep(5);
            const byte WRITE_AND_UPDATE_DAC_CHANNEL = 3;

            byte dacCmdAndAddr = (byte)((WRITE_AND_UPDATE_DAC_CHANNEL << 4 | dacAddress) & 0xff);
            //HdIO.WriteReg(ProcBdReg.W.reverse_Write2, dacCmdAndAddr);
            Thread.Sleep(5);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, (UInt32)(dacCtrlWord & 0xff00 | dacCmdAndAddr));
            Thread.Sleep(5);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_1, (byte)(dacCtrlWord & 0xff));

            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_4, 0x0);
            Thread.Sleep(5);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_4, 0xb);
            Thread.Sleep(5);
        }

        public static void SetExtTrigCoupling(TriggerCoupling couplingType)
        {
            WriteExtTrigDAC((byte)DACIndex.DAC0, 0xc000/*(ushort)(couplingCtrlWords[couplingType] >> 1 & 0x1)*/);
            WriteExtTrigDAC((byte)DACIndex.DAC1, 0xc000/*(ushort)(couplingCtrlWords[couplingType] >> 2 & 0x1)*/);
            WriteExtTrigDAC((byte)DACIndex.DAC2, (ushort)(couplingCtrlWords[couplingType] & 0x1));
        }

        public static void SetExtTrigSource(bool isExtTrig, bool isHighImpedance, ushort slope)
        {

            //     uint trigExtSetting = (slope == 0 ? 0xc0b0 : 0x80b0);
            uint trigExtSetting = 0x03c0;
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, 0);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_5, trigExtSetting); //DBI_DBI_DBIPROAUTOTRIGNUM
                                                                                     // HdIO.WriteReg(ProcBdReg.W.DBI_DBI_DBIPROAUTOTRIGNUM, trigExtSetting); //DBI_DBI_DBIPROAUTOTRIGNUM

            //WriteExtTrigDAC((byte)DACIndex.DAC3, isExtTrig ? (ushort)0x0000 : (ushort)0);
            WriteExtTrigDAC((byte)DACIndex.DAC3, isExtTrig ? (ushort)0xC000 : (ushort)0);

            if (isExtTrig)
                WriteExtTrigDAC((byte)DACIndex.DAC5, isHighImpedance ? (ushort)0x0 : (ushort)0xC000);

        }
        public static void SetExtTrigLevel(uint idx, double voltageV)
       {
            ushort ctrlWord = (ushort)(voltageV * 1000 * (65535 / 4096.0));

            Double volbase = 0;
            Double volpositive = 360;
            Double volnegative = -380;

            Double dac6base = 0;
            Double dac6positive = 1000;
            Double dac6negative = 0;

            Double dac7base = 300;
            Double dac7positive = 0;
            Double dac7negative = 1400;

            Double dac6ctrlword = dac6base;
            Double dac7ctrlword = dac7base;

            if (voltageV < volbase)
            {
                dac6ctrlword += (UInt32)Math.Round(voltageV * 1000 * (dac6negative - dac6base) / (volnegative - volbase));
                
                dac7ctrlword += (UInt32)Math.Round(voltageV * 1000 * (dac7negative - dac7base) / (volnegative - volbase));
                dac6ctrlword = 0;
            }

            if (voltageV > volbase)
            {
                dac6ctrlword += (UInt32)Math.Round(voltageV * 1000 * (dac6positive - dac6base) / (volpositive - volbase));
                
                dac7ctrlword += (UInt32)Math.Round(voltageV * 1000 * (dac7positive - dac7base) / (volpositive - volbase));
                dac7ctrlword = 0;

            }

            if (dac6ctrlword > 65535)
                dac6ctrlword = 65535;
            if (dac6ctrlword < 0)
                dac6ctrlword = 0;

            if (dac7ctrlword > 65535)
                dac7ctrlword = 65535;
            if (dac7ctrlword < 0)
                dac7ctrlword = 0;

            //WriteExtTrigDAC((byte)(idx == 0 ? DACIndex.DAC6 : DACIndex.DAC7), ctrlWord);
            WriteExtTrigDAC((byte)DACIndex.DAC6, (UInt16)Math.Round(dac6ctrlword));
            WriteExtTrigDAC((byte)DACIndex.DAC7, (UInt16)Math.Round(dac7ctrlword));//fu 65535 -1.04V 

            //WriteExtTrigDAC((byte)DACIndex.DAC6, 1000);
            //WriteExtTrigDAC((byte)DACIndex.DAC7, 0);//fu 65535 -1.04V 
        }

        //0V DAC6 2000  DAC7 0              0V      DAC6 0    DAC7 300
        //-300MV DAC6 12000  DAC7 0         360MV   DAC6 1000  DAC7 0
        //480MV DAC6 0  DAC7 18000          -380MV  DAC6 0    DAC7 2000 
        public static void CtrlTrigVolt()
        {
            var source = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;

            if (source != ChannelId.Ext&& source != ChannelId.Ext5)
            {
        //        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, 0x0000);
                return;
            }
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);
            //Thread.Sleep(10);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x1);
            //Thread.Sleep(10);
            //HdIO.WriteReg(ProcBdReg.W.Exttrig_SoftRst, 0x0);



            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, 0x0300);

            var coupling = Hd.UIMessage?.Trigger?.Edge?.Coupling ?? TriggerCoupling.DC;
            var imp = Hd.UIMessage?.Trigger?.Edge?.Impedance?? TriggerImpedance.Low50;
            bool ishigh= imp==TriggerImpedance.High1M?true:false;
            SetExtTrigCoupling(coupling);

            // var slopel = Hd.UIMessage?.Trigger?.Edge?.Slope ?? EdgeSlope.Rise;

            SetExtTrigSource(true, ishigh, 0);

            var volt = Hd.UIMessage?.Trigger?.Edge?.Position / 1000 ?? 0;//mv
                                                                         //      var volt = Hd.UIMessage?.Trigger?.Edge?.Position ?? 0;//mv

            //foreach (var chnlid in chnllist)
            //    if (trigLevel > 0)
            //    {
            //        SetExtTrigLevel(0, trigLevel);
            //    }
            //    else
            //    {
            //        SetExtTrigLevel(1, trigLevel);
            //    }
            // 转换公式 2/7*volt_level1*(1+5.1/20) - 5.1/20*volt_level2 = volt;
            //if (volt > 0)
            //{
            //    double compare_level1 = volt / (1 + 5.1 / 20.0) / (2 / 7.0);
            //    compare_level1 = (compare_level1 > 4.095) ? 4.095 : compare_level1;
            //    SetExtTrigLevel(0, compare_level1 * 0.07); //DAC6
            //    HdIO.Sleep(5);
            //    SetExtTrigLevel(1, 0); //DAC7

            //}
            //else
            //{
            //    double compare_level2 = volt / (-5.1 / 20.0);
            //    compare_level2 = (compare_level2 > 4.095) ? 4.095 : compare_level2;
            //    SetExtTrigLevel(0, 0); //DAC6
            //    HdIO.Sleep(5);
            //    SetExtTrigLevel(1, compare_level2 * 0.45); //DAC7
            //}
            SetExtTrigLevel(1, volt); //DAC7
        }

        #endregion

        public static void FPGAReg_AnalogChannelSet()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            CtrlOffset();
            CtrlGain();
            GeneratedAndSend4094CtrlWord();
            ConfigBandwidth();
            stopwatch.Stop();
            var sss = stopwatch.ElapsedMilliseconds;




        }
        public static void CtrlADCOffset()
        {
            bool ifneedchangeoffset = false;
            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            { 
                HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![channelindex];
                var scale = analogparas.ScaleBymV;
                var oldscale= Hd.CurrProduct.Acquirer_AnalogChannel?.AcquedParameters?.HdMessage?.Analog?[channelindex].ScaleBymV ?? scale;
                if (scale<25 && oldscale>25)
                {
                    ifneedchangeoffset = true;
                }
                if (scale > 25 && oldscale < 25)
                {
                    ifneedchangeoffset = true;
                }
            }
            if (ifneedchangeoffset)
            {
                //Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();

            }

        }

        public static void Send_IFCCoefficientsToAcqBoardByRegisterMode()
        {
            CoefficientsTableSender_8000X.Send_IFCCoefficientsToAcqBoardByRegisterMode(true);
        }

        private static Dictionary<ChannelId, Int32> SendScaleCoe = new();
        
        public static void Send_IFCCoefficientsSelect()
        {
            if (Hd.UIMessage?.Analog != null && !Hd.UIMessage.bAcquireStopped)
            {
                Dictionary<ChannelId, ProcBdReg.W> regdefine = new()
                {
                    [ChannelId.C1] = ProcBdReg.W.Dsp_CoeSelectCh1,
                    [ChannelId.C2] = ProcBdReg.W.Dsp_CoeSelectCh2,
                    [ChannelId.C3] = ProcBdReg.W.Dsp_CoeSelectCh3,
                    [ChannelId.C4] = ProcBdReg.W.Dsp_CoeSelectCh4,
                };
                for (Int32 chnlid = 0; chnlid < Hd.UIMessage.Analog.Length; chnlid++)
                {
                    if (!regdefine.ContainsKey((ChannelId)chnlid))
                    {
                        continue;
                    }
                    if (!SendScaleCoe.ContainsKey((ChannelId)chnlid) || SendScaleCoe[(ChannelId)chnlid] != Hd.UIMessage.Analog[chnlid].ScaleIndex)
                    {
                        HdIO.WriteReg(regdefine[(ChannelId)chnlid], (UInt32)(Hd.UIMessage.Analog[chnlid].ScaleIndex - 2));
                        SendScaleCoe[(ChannelId)chnlid] = Hd.UIMessage.Analog[chnlid].ScaleIndex;
                    }
                }
            }

            //Int32 index = Hd.UIMessage!.Analog[(Int32)ChannelId.C1].ScaleIndex - 2;
            //HdIO.WriteReg(ProcBdReg.W.Dsp_CoeSelectCh1, (UInt32)index);
            //index = Hd.UIMessage!.Analog[(Int32)ChannelId.C2].ScaleIndex - 2;
            //HdIO.WriteReg(ProcBdReg.W.Dsp_CoeSelectCh2, (UInt32)index);
            //index = Hd.UIMessage!.Analog[(Int32)ChannelId.C3].ScaleIndex - 2;
            //HdIO.WriteReg(ProcBdReg.W.Dsp_CoeSelectCh3, (UInt32)index);
            //index = Hd.UIMessage!.Analog[(Int32)ChannelId.C4].ScaleIndex - 2;
            //HdIO.WriteReg(ProcBdReg.W.Dsp_CoeSelectCh4, (UInt32)index);
        }
        #endregion
        /// <summary>
        /// 发送通道设置给单片机
        /// </summary>
        public new static void COMPort_AnalogChannelSet()
        {
            ///物理通道设置
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;

            #region 根据协议准备数据
            Byte channelbits = 0b1111;  //默认打开4个通道
            List<Byte> senddata = new List<Byte>
            {
                channelbits
            };

            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                HdMessage.AnalogOptions analogparas = Hd.UIMessage.Analog[channelindex];
                (Int32 ScaleIndex, Int32 ScaleValueByuV, Int32 GainFineByFpgaThousand) bestscaleindexfpgafine = GetCurrentScaleIndex(channelindex);

                Int32 impedance_h_is0 = analogparas.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                Int32 yscaleindex = bestscaleindexfpgafine.ScaleIndex;
                AnalogChannelItem_Base chnlparams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
                    new((ChannelId)channelindex, impedance_h_is0 == 0, (UInt32)(bestscaleindexfpgafine.ScaleValueByuV / 1000)))!.Value;

                ///获取衰减挡位
                Int32 att = 2;//默认为衰减档
                if (yscaleindex <= (Int32)AnaChnlScaleIndex.Lv20m)
                {
                    //放大档
                    att = 1;
                }
                else if (yscaleindex <= (Int32)AnaChnlScaleIndex.Lv100m)
                {
                    //直通档
                    att = 0;
                }
                if (Hd.CurrDebugVarints.bEnable_OpenCrystal)
                    att = 3;

                ///增益控制
                //说明：1）当前调整增益有2个DSA；
                //      2）放大档，Dsa1 = Dsa2;
                //      3）衰减档,直通档，Dsa2默认设到63，调整Dsa1;
                Int32 dsa1ctr = Math.Min((Math.Max(chnlparams.Gain, 0)), 63);
                Int32 dsa2ctr = att == 1 ? dsa1ctr : 63;

                senddata.Add((Byte)dsa1ctr);
                senddata.Add((Byte)dsa2ctr);

                ///衰减控制
                senddata.Add((Byte)att);

                ///DAC控制
                //说明：1）当前调整位移，偏执只用设置一个DAC(0div由通道参数Offset指定，斜率由通道参数Offset_Pos3Div指定)；
                //         另一个DAC为定值(值由通道参数Bias指定)；
                //      2）放大档，设置DAC使用fgn；fdn为定值；
                //      3）衰减档,直通档，设置DAC使用fdn；fgn为定值；
                Int32 offpos = (Int32)(Constants.SAMPS_PER_YDIV * (analogparas.Position) * 1000 / bestscaleindexfpgafine.ScaleValueByuV);
                Int32 biaspos = (Int32)(Constants.SAMPS_PER_YDIV * (double)analogparas.Bias / bestscaleindexfpgafine.ScaleValueByuV);
                Int32 offbias = 0;

                var autocaliparams = AutoCaliParams.Default![channelindex, impedance_h_is0, yscaleindex];

                if ((offpos - biaspos) > (Constants.SAMPS_PER_YDIV * Constants.MAX_YDIVS_NUM / 2))
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPreceding_N3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else if ((offpos - biaspos) > 0)
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPosterior_3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else if ((offpos - biaspos) > -(Constants.SAMPS_PER_YDIV * Constants.MAX_YDIVS_NUM / 2))
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPosterior_N3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }
                else // (offPos - biasPos) <= -(Constants.SAMPS_PER_YDIV * Constants.MAX_YDIVS_NUM / 2)
                {
                    offbias = (Int32)((offpos - biaspos) * (autocaliparams.OffsetPreceding_3Div / (Constants.SAMPS_PER_YDIV * 3)));
                }

                Int32 fdn, fgn;
                //放大档
                if (att == 1)//10 20 mV
                {
                    fgn = Math.Min((Math.Max(autocaliparams.OffsetPosterior + offbias, 0)), 65535);
                    fdn = Math.Min((Math.Max(chnlparams.Bias, 0)), 65535);
                }
                else
                {
                    //衰减档,直通档
                    fdn = Math.Min((Math.Max(autocaliparams.OffsetPosterior + offbias, 0)), 65535);
                    fgn = Math.Min((Math.Max(chnlparams.Bias, 0)), 65535);
                }

                //wcj: 8G通道V2，DAC调整效果是反的，这里反过来发，保证上层控制逻辑一致
                fgn = 65535 - fgn;
                fdn = 65535 - fdn;

                ///固定值发送
                var compensatedac = GetImpCompensateDacSetting((ChannelId)channelindex, (AnaChnlScaleIndex)yscaleindex);
                Int32 fgp = Math.Min((Math.Max((Int32)compensatedac.Fgp, 0)), 65535);//补偿电压,定值
                Int32 fdp = Math.Min((Math.Max((Int32)compensatedac.Fdp, 0)), 65535);//补偿电压，定值

                senddata.Add((Byte)(fgn & 0xff));
                senddata.Add((Byte)(fgn >> 8 & 0xff));

                senddata.Add((Byte)(fgp & 0xff));
                senddata.Add((Byte)(fgp >> 8 & 0xff));

                senddata.Add((Byte)(fdn & 0xff));
                senddata.Add((Byte)(fdn >> 8 & 0xff));

                senddata.Add((Byte)(fdp & 0xff));
                senddata.Add((Byte)(fdp >> 8 & 0xff));
            }
            #endregion 根据协议准备数据

            //发送串口数据
            baseObj1.ClearSpecialReceiveQueue((Byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet);
            baseObj1.SendData(true, (Int32)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, senddata);

            ///带宽设置
            ConfigBandwidth();
        }

        /// <summary>
        /// 配置带宽
        /// </summary>
        private static void ConfigBandwidth()
        {
            var define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave();
            for (Int32 channelindex = (Int32)ChannelId.C1; channelindex < ChannelIdExt.AnaChnlNum; channelindex++)
            {
                Byte enbits = 3;
                Byte selbits = 0;
                HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![channelindex];

                if (!define!.Details.Keys.Contains((ChannelId)channelindex))
                    continue;
                var chnldtl = define!.Details[(ChannelId)channelindex];
                if (chnldtl == null)
                    continue;

                AcqBdNo board = chnldtl[0].AcqBdNo;//??????????
                foreach (var chn in chnldtl)
                {
                    board = chn.AcqBdNo;
                    if (analogparas.Active)
                    {
/*
                        if (analogparas.Bandwidth == 3) //20MHz带宽限制
                        {
                            selbits = 0;
                        }
                        else */if (analogparas.Bandwidth == 1 || analogparas.Bandwidth == 2)
                        {
                            selbits = 7;
                            var coe = GetBandwidthCoe(analogparas.Bandwidth);

                            for (Int32 i = 0; i < coe.High.Count; i++)
                            {
                                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteEn, board, 0);
                                //需要区分采集板上的通道号，地址不一样
                                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteAddr, board, (UInt32)(i + chidinacq * 32));
                                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteAddr, board, (UInt32)(i));
                                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteDataH, board, coe.High[i]);
                                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteDataL, board, coe.Low[i]);
                                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitCoeffWriteEn, board, 1);
                                HdIO.Sleep(2);
                            }
                        }
                        else //全带宽
                        {
                            enbits = 0;
                        }
                        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitEn, board, enbits);
                        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Channel_BandLimitSelect, board, selbits);
                    }
                }
                if (!Hd.Calibration.CaliStatus)
                    SetDelayByBandwidth((ChannelId)channelindex, analogparas.Bandwidth);
            }
        }
        
        private static void SetDelayByBandwidth(ChannelId channelid,Int32 bandwidthIndex)
        {
            double[] delays500M=new double[4] { 0,0,0,0};
            double[] delays4G=new double[4] { 0,0,0,0};
            double[] delays8G=new double[4] { 0,0,0,0};

           UInt32[] delays500MDot = new UInt32[4] { 1, 1, 1, 1 };
            UInt32[] delays4GDot = new UInt32[4] {1, 1, 1,1 };
            UInt32[] delays8GDot = new UInt32[4] { 0, 0, 0, 0 };

            var parms = Hd.CurrProduct?.Acquirer_AnalogChannel?.SyncParams();
            if (parms==null||parms.Count()<(Int32)channelid)
            {
                return;
            }
            if (parms[3]==null)
            {
                return;
            }
            if ((Int32)channelid < parms.Count())
            {
                for (int i = 0; i < parms.Count(); i++)
                {
                    delays500M[i] = delays500M[i]+ parms[i].FarrowDelayByFs;
                    delays4G[i] = delays4G[i]+ parms[i].FarrowDelayByFs;
                    delays8G[i] = parms[i].FarrowDelayByFs;

                    delays500MDot[i] = parms[i].DotsCnt==0?0: parms[i].DotsCnt- delays500MDot[i];
                    delays4GDot[i] = parms[i].DotsCnt == 0 ? 0 : parms[i].DotsCnt - delays4GDot[i] ;
                    delays8GDot[i] = parms[i].DotsCnt;
                }
            }
            switch (bandwidthIndex)
            {
                case 0:
                    SendFarrowDelay(channelid, delays8G);
                    SendDiscardDotsCnt(channelid, delays8GDot);
                    break;
                case 1:
                    SendFarrowDelay(channelid, delays4G);
                    SendDiscardDotsCnt(channelid, delays4GDot);
                    break;
                case 2:
                    SendFarrowDelay(channelid, delays500M);
                    SendDiscardDotsCnt(channelid, delays500MDot);
                    break;

                default:
                    break;
            }
           

        }

        private static void SendFarrowDelay(ChannelId channelId, double[] delays)
        {
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//farrow enable      // reg_farrow_filter_en,reg_int_delay_en  
           
            if (delays.Length > (Int32)channelId)
            {
                var delay = delays[(Int32)channelId];
                //double[] delay = {  13000,0, 13000, 13000 };
                //delay = 1000;
                //比例关系,总控制字16bit,最大65535,相差按照比例转换成控制字 errorbyfs / 采样间隔(fs单位)                                  
                UInt32 farrowDelay = ~(UInt32)(65535 * (delay / 25000)) + 1;//取负数
                //farrowDelay =~(UInt32)1;
                //发送分数延迟滤波器延迟值
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0b1u << (Int32)channelId);//通道选择
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyL16, farrowDelay);//low 
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFracNumberDlyH1, farrowDelay >> 16);//high
                Thread.Sleep(10);
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能
                //HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectFarrow, 0);
                Thread.Sleep(100);
            }
            
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);//打开farrow使能
        }

        private static void SendDiscardDotsCnt(ChannelId channelId, UInt32[] delays)
        {
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0b1u <<(Int32) channelId);//延迟丢点通道选择
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, delays[(Int32)channelId]);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 0);
            //HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayNum, 5);
            HdIO.WriteReg(ProcBdReg.W.ChannelSync_ChannelSelectInt, 0);
        
        }

        /// <summary>
        /// 获取带宽系数
        /// </summary>
        /// <param name="bandwidth">=1:20G-4G,10G-2G;=2:20G-2G,10G-1G;=3:20G-1G,10G-500M</param>
        private static (List<UInt32> High, List<UInt32> Low) GetBandwidthCoe(Int32 bandwidth)
        {
            List<UInt32> high;
            List<UInt32> low;
            if (bandwidth == 1)//4G
            {
                high = new List<UInt32>() { 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0 };
                low = new List<UInt32>() {0x048b,0x0328,0xfad3,0xf90e,0x0475,0x0c8b,0xfef8,0xea66,0xf65f,0x2e2e,0x6581,
                                          0x6581,0x2e2e,0xf65f,0xea66,0xfef8,0x0c8b,0x0475,0xf90e,0xfad3,0x0328,0x048b };
            }
            else if (bandwidth == 2)//1G
            {
                high = new List<UInt32>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                low = new List<UInt32>() { 0x033C, 0x053E, 0x073B, 0x0928, 0x0AF7, 0x0C9E, 0x0E11, 0x0F46, 0x1035, 0x10D8, 0x112B,
                                           0x112B, 0x10D8, 0x1035, 0x0F46, 0x0E11, 0x0C9E, 0x0AF7, 0x0928, 0x073B, 0x053E, 0x033C};
                /*
            //6G——bw
               high = new List<UInt32>() { 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0 };
               low = new List<UInt32>() { 0x039A, 0xFF62, 0xFABE, 0x0619, 0x0264, 0xF3DD, 0x0851, 0x0C74, 0xE1B3, 0x099F,
                                          0x85DD, 0x85DD, 0x099F, 0xE1B3, 0x0C74, 0x0851, 0xF3DD, 0x0264, 0x0619, 0xFABE, 0xFF62, 0x039A };
                */
            }
            else//bandwidth == 3
            {
                high = new List<UInt32>() { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };
                low = new List<UInt32>() {0xfbd2,0xfbc9,0xfced,0xff7f,0x038c,0x08dd,0x0efc,0x153d,0x1adc,0x1f1a,0x2162,
                                          0x2162,0x1f1a,0x1adc,0x153d,0x0efc,0x08dd,0x038c,0xff7f,0xfced,0xfbc9, 0xfbd2 };
            }
            return (high, low);
        }
    }
}
