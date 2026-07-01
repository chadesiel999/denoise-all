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
        internal static void Config_Timeout(ISearchTypeOptions searchTypeOption)
        {
            var option = (HdMessage.TrigTimeOutOptions)searchTypeOption;

            //下发使能
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en, 1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_type, 0b0000_0101_000);
            //源
            uint source = (uint)(option?.Source ?? ChannelId.C1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_source_sel, (uint)source);

            //电平
            //电平
            UInt32 compVolt;
            UInt32 compVoltUp;
            UInt32 compVoltDn;
            Int32 PerYDivAdcSamples = Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM;
            Int32 AdcCenterValue = Constants.MAX_ADC_RES / 2;
            int sourceYPos = AdcCenterValue;
            uint DefaultTrigSensitivity = Hd.CurrProduct!.Acquirer_AnalogChannel!.DefaultTrigSensitivity;
            bool isPositive = true;
            if (source < ChannelIdExt.AnaChnlNum)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![source];
                sourceYPos = (int)(Constants.SAMPS_PER_YDIV * analogParameters.Position / analogParameters.Scale + AdcCenterValue);
            }
            compVolt = (UInt32)((option!.PosIndex) / Constants.IDX_PER_YDIV * PerYDivAdcSamples + sourceYPos);
            isPositive = option!.Polarity == LevelPolarity.Positive;
            if (isPositive)
            {
                if ((compVolt - DefaultTrigSensitivity) < 0)
                    compVolt = DefaultTrigSensitivity;
                compVoltUp = compVolt + DefaultTrigSensitivity;
                compVoltDn = compVolt;
            }
            else
            {
                if ((compVolt + DefaultTrigSensitivity) > 4095)
                    compVolt = 4095 - DefaultTrigSensitivity;
                compVoltUp = compVolt;
                compVoltDn = compVolt - DefaultTrigSensitivity;
            }
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_h, compVoltUp);               //处理板二级触发迟滞高电平选择
            HdIO.WriteReg(ProcBdReg.W.search_pc_seaech_cmp1_level_l, compVoltDn);



            UInt64 DurationTime = (UInt64)(option?.DurationByps ?? 4000 * 96);
            UInt64 secondWidth = (UInt64)(DurationTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //设置二级触发脉宽
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setL, (uint)secondWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setM, (uint)(secondWidth >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_configure_data1_setH, (uint)(secondWidth >> 32) & 0xffff);
            //二级超时极性选择  0 负极性超时；1; 正极性超时
           
            uint secondPolarity = (uint)(option?.Polarity ?? 00);
            if (secondPolarity == 00)
            {
                HdIO.WriteReg((uint)ProcBdReg.W.search_pc_search_timeout_set, 1);
            }
            else
            {
                HdIO.WriteReg((uint)ProcBdReg.W.search_pc_search_timeout_set, 0);
            }

        }
    }
}
