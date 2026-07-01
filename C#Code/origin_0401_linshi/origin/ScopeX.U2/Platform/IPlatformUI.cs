using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.U2.KeyboardLed;

namespace ScopeX.U2
{
    /// <summary>
    /// 各型号在界面层的差异基类
    /// </summary>
    internal interface IPlatformUI
    {
        ProductType ProductType { get; }

        /// <summary>
        /// 通道高压报警
        /// </summary>
        void HardwareWarningEventHandler(HardwareWarningEventMessageArgs eventArgs);

        /// <summary>
        /// Utility按键差异处理
        /// </summary>
        void KeyEnumUtilityHandler() { }

        /// <summary>
        /// Trigger按键差异处理
        /// </summary>
        void KeyEnumTriggerHandler() { }

        public void KeyEnumTrigForceHandler()
        {
            TriggerPrsnt.Force();
        }

        /// <summary>
        /// 用于按键板初始化平台化
        /// 对datas的补充
        /// </summary>
        /// <param name="datas"></param>
        void KeyboardInit(List<IKeyData> datas);

        /// <summary>
        /// 获取触发源，包含了模拟通道、数字通道、Ext、Ext5、AC、Auxin通过条件过滤
        /// </summary>
        /// <param name="hasDigitalChnl">是否包含数字通道</param>
        /// <param name="hasExtChnl">是否包含外触发通道</param>
        /// <param name="hasAcChnl">是否包含Ac通道</param>
        /// <param name="hasAuxin">是否包含Auxin通道</param>
        /// <returns></returns>
        IReadOnlyList<ChannelId> GetTriggerSource(Boolean hasDigitalChnl = false, Boolean hasExtChnl = false, Boolean hasAcChnl = false, Boolean hasAuxin = false);

        /// <summary>
        /// 获取支持的选件
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<OptionType, (String FunctionName, String Description)>> GetOptionInfo();

        List<ChannelId> GetEditableColorsChannel();

        ILanguage GetLanguage(Language language);

        public String HelperXmlName => $"{ProductType}.xml";

        GeneralAttribute Attribute { get; }

        String[] GetAwgTriggerSource();

        IkeyBoardDetetionView KeyboardDetectionView { get; }

        List<FFTNumber> GetFFTNumbers();

        /// <summary>
        /// FFT是否与Jitter互斥
        /// </summary>
        /// <returns></returns>
        Boolean FFTFunctionLimitWithJitter();

        /// <summary>
        /// 获取温控UI参数
        /// </summary>
        /// <returns></returns>
        (Int32 MaxFanSpeed, Int32 Scale, Int32 MaxTemperature) GetFanControlParams();
    }

    /// <summary>
    /// 通用属性
    /// </summary>
    /// <param name="MutiBus">是否支持多通道Bus</param>
    /// <param name="MutiAwg">是否支持多通道AWG</param>
    /// <param name="Touchable">是否可触屏</param>
    internal record GeneralAttribute(Boolean MutiBus, Boolean MutiAwg, Boolean Touchable)
    {
        /// <summary>
        /// 是否支持Digital
        /// </summary>
        public Boolean SupportDigital { get; init; }

        /// <summary>
        /// 是否支持自定义按键
        /// </summary>
        public Boolean SupportUtilityKey { get; init; }

        /// <summary>
        /// 是否支持功能裁剪
        /// </summary>
        public Boolean FunctionCropping { get; init; }

        /// <summary>
        /// 是否有按键板
        /// </summary>
        public Boolean SupportKeyBoard { get; init; }

        /// <summary>
        /// 是否支持获取或设置屏幕亮度
        /// </summary>
        public Boolean SupportGetOrSetBrightness { get; init; }

        /// <summary>
        /// 是否支持辅助输入
        /// </summary>
        public Boolean SupportAuxIn { get; init; }

        /// <summary>
        /// 是否支持旋钮条件
        /// </summary>
        public Boolean SupportKnodAdjust { get; init; }

        /// <summary>
        /// 是否有细调按钮
        /// </summary>
        public Boolean SupportKnobFine { get; init; }

        /// <summary>
        /// 是否Autode等待模式
        /// </summary>
        public Boolean AutosetWaitingMode { get; init; }

        /// <summary>
        /// 是否支持高阻
        /// </summary>
        public Boolean SupportHighImpedance { get; init; }
    }
}
