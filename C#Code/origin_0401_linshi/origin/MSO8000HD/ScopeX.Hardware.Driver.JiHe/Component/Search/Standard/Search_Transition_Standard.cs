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
        internal static void Config_Transition(ISearchTypeOptions searchTypeOption)
        {
            var option = (HdMessage.TrigTransOptions)searchTypeOption;
            if (option == null)
                return;
            //下发使能
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_type, 0b0000_0010_000);

            UInt64 runtTime = (UInt64)(option?.WidthByps ?? 96000);
            //斜率触发条件和极性
            uint polarityAndCondition = 0;
            polarityAndCondition = (uint)(option!.Condition);
            if (option!.Slope == EdgeSlope.Fall)
                polarityAndCondition |= 1 << 2;
            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;

            uint source = (uint)(option?.Source ?? ChannelId.C1);
            int sourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (source < ChannelIdExt.AnaChnlNum)
                sourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![source].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);

            uint upper = (UInt32)((option?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);
            uint lower = (UInt32)((option?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);
            if ((option?.Slope ?? EdgeSlope.Rise) == EdgeSlope.Rise)
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

            //源
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_source_sel, source);
            //二级斜率触发条件和极性
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_slope_set, polarityAndCondition);
            //设置斜率触发下限电平
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_l, upperCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_h, upperCompVolt.Up);
            //二级斜率触发上限电平
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_l, LowerCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_h, LowerCompVolt.Up);
            //设置斜率触发二级触发时间
            UInt64 secondTime = (UInt64)(runtTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setL, (uint)secondTime & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setM, (uint)(secondTime >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setH, (uint)(secondTime >> 32) & 0xffff);
            //throw new NotImplementedException($" this type not Implemented");
        }
    }
}
