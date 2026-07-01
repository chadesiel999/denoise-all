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
        internal static void Config_Edge(ISearchTypeOptions searchTypeOption)
        {

            var option = (HdMessage.TrigEdgeOptions)searchTypeOption;

            //下发使能
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.search_pc_search_pro_en,1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_type, 0b0000_0000_000);
            //源
            uint source = (uint)(option?.Source ?? ChannelId.C1);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_source_sel, (uint)source);

            //边沿
            uint slope = (uint)(option?.Slope == 0 ? 1 :0);
            HdIO.WriteReg(ProcBdReg.W.search_pc_search_edge_sel,(uint)(slope == 0 ? 1 : 0));

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
            isPositive = option!.Slope == EdgeSlope.Rise;
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

        }
    }


    //每个寄存器都要改



}
    

