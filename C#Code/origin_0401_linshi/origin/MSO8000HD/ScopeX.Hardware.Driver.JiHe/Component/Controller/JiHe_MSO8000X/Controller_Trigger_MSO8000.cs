#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Registers.SendManage;

namespace ScopeX.Hardware.Driver
{
    internal class Controller_Trigger_MSO8000 : Controller_Trigger_Standard
    {
        public Controller_Trigger_MSO8000(Dictionary<AnaChnlTimebaseIndex, int> interpolationLevelDiscardNumTable) : base(interpolationLevelDiscardNumTable)
        {
        }

        protected override void ourConfigTriggerSource()
        {
            ChannelId trigchannel = (ChannelId)CurrentTrigSource();
            Boolean trigen = true;
            ConditionManager.TriggerSrc = trigchannel;

            switch (trigchannel)
            {
                case ChannelId chn when chn >= ChannelId.C1 && chn <= ChannelId.C4:
                    #region 模拟通道触发源配置
                    if (Hd.UIMessage!.bAcquireStopped)
                        return;
                    //trigen = Hd.UIMessage!.Analog![(int)trigchannel].Active;
                    //if (!trigen)
                    //    break;
                    UInt32 trigchnlcode = (UInt32)trigchannel;
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq,0);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectAcq_Pro, trigchnlcode);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, trigchnlcode);
                    #endregion 模拟通道触发源配置
                    break;
                case ChannelId.AuxIn:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectAcq_Pro, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b10);
                    break;
                case ChannelId.Ext:
                case ChannelId.Ext5:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectAcq_Pro, 0x10);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b01);
                    break;
                case ChannelId.AC:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectAcq_Pro, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b00);
                    break;
                case ChannelId chnl when chnl >= ChannelId.D0 && chnl <= ChannelId.D15:
                    Int32 dgtindex = trigchannel - ChannelId.D0;
                    trigen = Hd.UIMessage!.Digital![dgtindex].Active;
                    trigchnlcode = (UInt32)(dgtindex + 1) << 8; //0x0100-0x1000对应D0-D15
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectAcq_Pro, trigchnlcode);
                    break;
            }

            ConditionManager.TriggerCtrlEn = trigen;
            ConditionManager.IsFromDDR = !(Hd.UIMessage?.Timebase?.IsScan ?? false);
            RegSendManager.Default.Send((UInt32)AcqBdReg.W.TrigCtrl_DigitalTrigEn);
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);


            Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_IFC_Delay(false);
        }

        protected override void ourConfigTriggerMode()
        {
            UInt32 trigMode = 0;
            if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Auto)
                trigMode = 0;
            else if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Normal)
                trigMode = 1;
            else
            {
                if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.OneShot)
                    trigMode = 1;
                else
                    trigMode = 0;// Hd.CurrHdMessage?.Trigger?.Mode == TriggerMode.Auto ? 2 : (uint)4;
            }

            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Mode_Pro, trigMode+0x8000);//有1走JH，没有走自己
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);
        }

        /// <summary>
        /// 配置数字触发相关参数。如丢点数，搜索宽度等。
        /// </summary>
        internal override void ourConfigDgtParameter()
        {
            //临时代码
            int trigSource = (int)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int toFpgaTrigPos = AbstractController_Trigger.AdcCenterValue;

            if ((trigSource < ChannelIdExt.AnaChnlNum))
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![trigSource];
                int trigType = (int)Hd.UIMessage?.Trigger?.TrigType;//触发类型
                if (trigType == (int)TriggerType.Delay || trigType == (int)TriggerType.SetupHold || trigType == (int)TriggerType.SustainTime)
                    return;
                var probeGain = analogParameters.ProbeIndex switch
                {
                    AnaChnlProbe.x1 => 1,
                    AnaChnlProbe.x10 => 10,
                    AnaChnlProbe.x100 => 100,
                    _ => 1,
                };
                switch (trigType)
                {
                    case (int)TriggerType.Edge:
                        toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Edge?.Position / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                        break;
                    case (int)TriggerType.PulseWidth:
                        toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Pulse?.Position / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                        break;
                }

                toFpgaTrigPos = Math.Max(0, Math.Min(4095, toFpgaTrigPos));

                //toFpgaTrigPos = toFpgaTrigPos > 255 ? 255 : toFpgaTrigPos;
                //toFpgaTrigPos = toFpgaTrigPos < 25 ? 25 : toFpgaTrigPos;
                if (Hd.UIMessage?.Trigger?.TrigType != TriggerType.Serial && (!Hd.UIMessage?.bAcquireStopped ?? false))
                {
                    switch (trigSource)//低电平
                    {
                        case (int)ChannelId.C1:
                            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, (uint)(toFpgaTrigPos));
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C2:
                            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, (uint)(toFpgaTrigPos));
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C3:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C4:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(toFpgaTrigPos));
                            break;

                    }
                    HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve17, (uint)(toFpgaTrigPos));
                }

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, (uint)(toFpgaTrigPos - 80));
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, (uint)toFpgaTrigPos);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, 2000);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, 2200);
            }
        }
    }
}
#endif
