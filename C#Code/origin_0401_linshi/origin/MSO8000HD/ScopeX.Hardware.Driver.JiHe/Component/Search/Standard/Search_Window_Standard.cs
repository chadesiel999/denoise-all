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
        internal static void Config_Window(ISearchTypeOptions searchTypeOption)
        {
            var option = (HdMessage.TrigWindowOptions)searchTypeOption;

            //下发使能
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_type, 0b0000_0110_000);
            //源
            uint source = (uint)(option?.Source ?? ChannelId.C1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_source_sel, (uint)source);

            ////选择二级脉宽触发条件,窗口范围(极性)
            WindowTimeCondition condition = option?.Condition ?? WindowTimeCondition.GreaterThan;
            WindowRange range = option?.Range ?? WindowRange.Inside;
            uint value = (uint)condition & 0x3;
            value |= (uint)range << 2;
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_window_set, (uint)value);

            //设置二级触发脉宽
            long widthByps = option?.WidthByps ?? 0;
            UInt64 secondWidth = (UInt64)(widthByps / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000)); //宽度以逻辑采样间隔ps数计算           
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setL, (uint)secondWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setM, (uint)(secondWidth >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setH, (uint)(secondWidth >> 32) & 0xffff);


            //高低电平
            int sourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (source < ChannelIdExt.AnaChnlNum)
                sourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![source].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);

            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;

            uint upper = (UInt32)((option?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);
            uint lower = (uint)((option?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + sourceYPos);

            if (option!.Range != WindowRange.Inside)//正极性以下降沿判断，反之亦然
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

            //二级欠幅触发电平上限
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_h, upperCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp2_level_l, upperCompVolt.Up);
            //二级欠幅触发电平下限
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_l, LowerCompVolt.Dn);
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_h, LowerCompVolt.Up);



        }
    }
}
