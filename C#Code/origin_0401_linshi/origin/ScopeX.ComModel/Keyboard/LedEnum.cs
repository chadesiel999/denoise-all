using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 按键板中所有led的键值定义
    /// <para>
    /// 由于现在LED和按键是一一对应的，因此现定义为相同值
    /// </para>
    /// 同时为了方便扩展，这里对两个键值进行分开定义
    /// </summary>
    public enum LedEnum : Byte
    {
        LedRunStop = KeyEnum.RunStop,
        LedSingle = KeyEnum.Single,
        LedCH1 = KeyEnum.CH1,
        LedCH2 = KeyEnum.CH2,
        LedCH3 = KeyEnum.CH3,
        LedCH4 = KeyEnum.CH4,
        LedCH5 = KeyEnum.CH5,
        LedCH6 = KeyEnum.CH6,
        LedCH7 = KeyEnum.CH7,
        LedCH8 = KeyEnum.CH8,
        LedMath = KeyEnum.Math,
        LedReference = KeyEnum.Reference,
        LedDigital = KeyEnum.Digital,
        LedBus = KeyEnum.Bus,
        LedMultipupose = KeyEnum.Multipupose,
        LedTriggerLevel = KeyEnum.TriggerLevel,
        LedVerticalScale = KeyEnum.VerticalScale,
        LedVerticalPosition = KeyEnum.VerticalPosition,

        //8000HD 新增LED
        LedTrigger = 120,
        LedReady = 121,
        LedRising = 122,
        LedFalling = 123,
        LedVpushToFine = 129,

        LedDVM = KeyEnum.DVM,
        LedClear = KeyEnum.Clear,
        LedAWG = KeyEnum.AWG,
        LedUltraAcq = KeyEnum.UltraAcq,
        LedMenu = KeyEnum.Trigger,
        LedNomal = KeyEnum.Normal,
        LedApps = KeyEnum.Apps,
        LedAuto = KeyEnum.Auto,
        LedDefault = KeyEnum.Default,
        LedMeasure = KeyEnum.Measure,
        LedPrtsc = KeyEnum.PrtSc,
        LedCursor = KeyEnum.Cursor,
        LedForce = KeyEnum.Force,
        LedQuickMeasure = KeyEnum.QuickMeasure,
        LedUtility = KeyEnum.Utility,
        LedAutoset = KeyEnum.Autoset,
        LedTouchLock = KeyEnum.TouchLock,

        LedCH1AC = 150,
        LedCH150,
        LedCH1BW,
        LedCH2AC,
        LedCH250,
        LedCH2BW,
        LedCH3AC,
        LedCH350,
        LedCH3BW,
        LedCH4AC,
        LedCH450,
        LedCH4BW,
        LedSpare,
#if ES10G12BIT
        LedStop,
#else
        LedStop = LedRunStop,
#endif // ES10G12BIT

    }
}
