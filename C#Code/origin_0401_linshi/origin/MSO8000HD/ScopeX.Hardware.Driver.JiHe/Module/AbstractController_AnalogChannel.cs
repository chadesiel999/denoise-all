using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道
    /// </summary>
    internal abstract class AbstractController_AnalogChannel
    {
        protected Action? _CtrlOffset;
        protected Action? _CtrlADCOffset;
        protected Action? _CtrlBias;
        protected Action? _CtrlGain;
        protected Action? _CtrlExtTrig;
        protected Action? _CtrlAnalogChannelSet;
        protected Action? _CtrlAnalogChannelCoefficientsParams;
        protected Action? _CtrlChannelDelay;

        protected Action? _Ctrl4094;
        protected Action? _Init;
        protected Action? _PowerOff;
        protected Func<Boolean, Boolean>? _PowerOn;
        protected Action? _SwitchDBI_ASCII;
        protected Action? _ActiveChannged;
        protected Action? _CtrlGainByFpga;
        protected Action? _SoftwareBandwidthProcess;
        protected Func<string>? _GetCaliMemo;
        protected Func<double>? _ReadTemperatures;
        protected Func<string>? _ReadAppRegistedRunStartTime;
        protected AnalogChannelType _ChannelModel = AnalogChannelType.BW1G2G4G;
        public static AnalogChannelType ChannelModel => Hd.CurrProduct?.Ctrl_AnalogChannel?._ChannelModel ?? AnalogChannelType.BW1G2G4G;
        public static void CtrlOffset() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlOffset?.Invoke();
        public static void CtrlADCOffset() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlADCOffset?.Invoke();
        public static void CtrlAnalogChannelSet() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlAnalogChannelSet?.Invoke();

        public static void Send_IFCCoefficientsToAcqBoardByRegisterMode() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlAnalogChannelCoefficientsParams?.Invoke();

        /// <summary>
        /// 通道延迟配置
        /// </summary>
        public static void CtrlChannelDelay() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlChannelDelay?.Invoke();

        public static void CtrlBias() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlBias?.Invoke();
        public static void CtrlGain() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlGain?.Invoke();
        public static void CtrlExtTrig() => Hd.CurrProduct?.Ctrl_AnalogChannel?._CtrlExtTrig?.Invoke();
        public static void Ctrl4094() => Hd.CurrProduct?.Ctrl_AnalogChannel?._Ctrl4094?.Invoke();
        public static void ActiveChannged() => Hd.CurrProduct?.Ctrl_AnalogChannel?._ActiveChannged?.Invoke();
        public static void SwitchDBI_ASCII() => Hd.CurrProduct.Ctrl_AnalogChannel?._SwitchDBI_ASCII?.Invoke();
        public static void CtrlGainByFpga() => Hd.CurrProduct.Ctrl_AnalogChannel?._CtrlGainByFpga?.Invoke();
        public static void SoftwareBandwidthProcess() => Hd.CurrProduct?.Ctrl_AnalogChannel?._SoftwareBandwidthProcess?.Invoke();
        public static string GetCaliMemo() => Hd.CurrProduct.Ctrl_AnalogChannel?._GetCaliMemo?.Invoke() ?? "";
        public static double ReadTemperatures() => Hd.CurrProduct.Ctrl_AnalogChannel?._ReadTemperatures?.Invoke() ?? 0.0;
        public static String ReadAppRegistedRunStartTime() => Hd.CurrProduct.Ctrl_AnalogChannel?._ReadAppRegistedRunStartTime?.Invoke() ?? "";
        /// <summary>
        /// 同时打开电源并配置参考电压
        /// </summary>
        public static void Init() => Hd.CurrProduct?.Ctrl_AnalogChannel?._Init?.Invoke();
        public static void PowerOff() => Hd.CurrProduct?.Ctrl_AnalogChannel?._PowerOff?.Invoke();

        public static Boolean PowerOn(bool isUpdate = false) => Hd.CurrProduct?.Ctrl_AnalogChannel?._PowerOn?.Invoke(isUpdate) ?? false;
        public static void AdjustGainByUI()
        {
            if (Hd.UIMessage!.Command == 0xff_ff_ff_ff_ff_ff_ff_ff)
                return;
            int direction = (Hd.UIMessage!.ComboBits & 0x04) == 0 ? 1 : -1;

            for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![channelIndex];
                int Impedance_H_is0 = analogParameters.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                int yScaleIndex = analogParameters.ScaleIndex;
                if (Hd.UIMessage.Analog[channelIndex].InputSource == AnaChnlIpnutSource.BNC)
                {
                    ChnlParamsKeyMap chnlParamKey = new((ChannelId)channelIndex, Impedance_H_is0 == 0,
                    (uint)(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[yScaleIndex] / 1000));
                    AnalogChannelItem_Base chnlParams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParamKey)!.Value;

                    chnlParams.Gain_FineByFpgaThousand = chnlParams.Gain_FineByFpgaThousand + chnlParams.Gain_FineByFpgaThousand * direction * 2 / 1000;//每次改变千分之2
                    ProductDataTranslate_MSO8000X.SetChnlParamsItem(chnlParamKey, chnlParams);
                }
                else
                {
                    int old_value = (int)ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, (int)analogParameters.ScaleIndex].Gain_FineByFpgaThousand;
                    int new_value = old_value + old_value * direction * 2 / 1000;//每次改变千分之2
                    ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, (int)analogParameters.ScaleIndex] = ChannelParamsModel2.Default[(int)channelIndex, Impedance_H_is0, (int)analogParameters.ScaleIndex] with { Gain_FineByFpgaThousand = (uint)new_value };
                }
            }
            CtrlGainByFpga();
        }
    }
}
