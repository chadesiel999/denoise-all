using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class TemplateTriggerProcess
    {
        internal TemplateTriggerProcess()
        { 
            
        }

        private Dictionary<ChannelId, Int32> _SendTemplateCntTable = new();

        internal void Run()
        {
            ChannelId triggerchnl = ChannelId.C1;
            var templatetriggermsg = GetTemplateTriggerMsg(triggerchnl);

            if (templatetriggermsg != null)
            {
                ConfigEnable(templatetriggermsg.Enable);
            }
        }

        internal void TemplateDataChanged()
        {
            ChannelId triggerchnl = ChannelId.C1;
            var templatetriggermsg = GetTemplateTriggerMsg(triggerchnl);

            if (templatetriggermsg != null)
            {
                if ((_SendTemplateCntTable.ContainsKey(triggerchnl) == false) ||
                    (_SendTemplateCntTable[triggerchnl] != templatetriggermsg.SendTemplateCnt))
                {
                    switch (templatetriggermsg.SourceType)
                    {
                        case TemplateSourceEnum.Outside:
                            var framedata = ArtificialIntelligenceProcess.Default.EmdProcess.TryGetAbnormalData(triggerchnl, new List<UInt32>() { templatetriggermsg.FrameIdForTrig });
                            if (framedata.ContainsKey(templatetriggermsg.FrameIdForTrig))
                            {
                                SendFrameData(templatetriggermsg, framedata[templatetriggermsg.FrameIdForTrig]);
                            }
                            break;
                        case TemplateSourceEnum.UserDefine:
                            if ((Int32)triggerchnl < AcqedDataPool.AnalogChData.AllChannelData.Count)
                            {
                                SendFrameData(templatetriggermsg, AcqedDataPool.AnalogChData.AllChannelData[(Int32)triggerchnl]);
                            }
                            break;
                        default: 
                            break;
                    }
                    _SendTemplateCntTable[triggerchnl] = templatetriggermsg.SendTemplateCnt;
                }
            }
        }

        private TemplateTriggerRecord? GetTemplateTriggerMsg(ChannelId chnlId)
        {
            if ((Hd.UIMessage?.AiTable == null) || (Hd.UIMessage.AiTable.ContainsKey(chnlId) == false))
                return null;

            return Hd.UIMessage.AiTable[chnlId].TemplateTrigger;
        }

        private void ConfigEnable(Boolean enable)
        {
            UInt32 templateSet = 0;
            templateSet = templateSet | (1 << 13);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_test_data_mode, 0);
            if (enable)
            {
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro, 0);
                templateSet = templateSet | (1 << 14) | (1 << 13) | (200 << 4);//15-temp_load_en  14-rst_n  13-temp_rst_n
            }
            else
            {
                templateSet = templateSet | (0 << 14) | (1 << 13) | (200 << 4);//15-temp_load_en  14-rst_n  13-temp_rst_n
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
            //UInt64 procBdTrigCtrl_1st_PreDepth = 75;
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetL, (UInt32)procBdTrigCtrl_1st_PreDepth & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetM, (UInt32)(procBdTrigCtrl_1st_PreDepth >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetH, (UInt32)(procBdTrigCtrl_1st_PreDepth >> 32) & 0xfff);
        }

        private void SendFrameData(TemplateTriggerRecord templateTrigger, List<UInt16> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i] >>= 4;  //16bit转12——dyh
            }
            ChannelId triggerchnl = ChannelId.C1;
            var templatetriggermsg = GetTemplateTriggerMsg(triggerchnl);
            UInt32 threshold = (UInt32)(templatetriggermsg.FrameTrigDataLen * 0.25 * 0.85);
            UInt32 templateSet = 0;
            //UInt32 threshold = 160;
            templateSet = templateSet | (1 << 14) | (0 << 13) | (200 << 4);//15-temp_load_en  14-rst_n  13-temp_rst_n
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
            templateSet = templateSet | (1 << 13);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_CmpThresh, threshold);

            Int32 cnt = templateTrigger.UserDefinePosStart + (Int32)templateTrigger.FrameTrigDataLen;
            if (cnt > data.Count)
                cnt = data.Count;

            for (Int32 i = templateTrigger.UserDefinePosStart; i < cnt; i = i + 4)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write0, updata[i]);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write1, downdata[i]);
                //if (i >= 400)
                //{
                //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_UpData, 1);//up
                //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_DownData, 0);//down
                //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
                //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet | (1 << 15));
                //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet & (0x7fff));
                //}
                //else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_UpData, data[i] + templateTrigger.Offset);//up
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_DownData, data[i] - templateTrigger.Offset);//down

                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet | (1 << 15));
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.AnormTemplate_Set, templateSet & (0x7fff));
                }
            }
        }
    }
}
