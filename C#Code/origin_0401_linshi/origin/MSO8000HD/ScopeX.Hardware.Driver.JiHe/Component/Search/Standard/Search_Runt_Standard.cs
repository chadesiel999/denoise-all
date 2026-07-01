using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlSearch_Standard
    {
        internal static void Config_Runt(ISearchTypeOptions searchTypeOption)
        {
            var option = (HdMessage.TrigRuntOptions)searchTypeOption;
            if (option == null)
                return;

            //下发使能
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_type, 0b0000_0100_000);

            UInt64 runtWidth = (UInt64)(option?.WidthByps ?? 96000);

            uint source = (uint)(option!.Source);
            int sourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (source < ChannelIdExt.AnaChnlNum)
                sourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![source].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);

            //高低电平
            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;

            uint upper = (UInt32)((option?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);
            uint lower = (uint)((option?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);
           
            if (option!.Polarity != PulsePolarity.Positive)//正极性以下降沿判断，反之亦然
            {
                upperCompVolt.Up = upper;
                upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Up = lower;
                LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
            }
            else
            {
                upperCompVolt.Up = upper + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                upperCompVolt.Dn = upper;
                LowerCompVolt.Up = lower + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Dn = lower;
            }

            uint condition = (uint)(option?.Condition ?? 0) & 0x3;
            condition |= (uint)(option?.Polarity ?? 0) << 2;


            //源
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_source_sel, source);
            //二级欠幅触发条件,极性
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_runt_set, condition);
            //二级欠幅触发电平上限
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_h, upperCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_l, upperCompVolt.Up);
            //二级欠幅触发电平下限
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_l, LowerCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_h, LowerCompVolt.Up);
            //二级欠幅触发宽度
            UInt64 secondWidth = (UInt64)(runtWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setL, (uint)secondWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setM, (uint)(secondWidth >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setH, (uint)(secondWidth >> 32) & 0xffff);
            
            //二级欠幅触发极性
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 0x00);

        }
    }
}
