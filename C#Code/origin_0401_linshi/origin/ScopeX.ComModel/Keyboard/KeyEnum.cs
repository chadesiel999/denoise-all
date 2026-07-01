using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 按键板中所有按键值定义
    /// </summary>
    public enum KeyEnum : Byte
    {
        Apps = 0,
        TouchLock,
        RunStop,
        Normal,
        PrtSc,
        Utility,
        Autoset,
        Auto,
        DVM,
        AWG,
        Clear,
        Default,
        Single,
        Force,
        Math,
        Reference,
        Digital,
        Bus,
        Trigger,
        Measure,
        CH1,
        CH2,
        CH3,
        CH4,
        CH5,
        CH6,
        CH7,
        CH8,
        Cursor,
        QuickMeasure,
        UltraAcq,
        //ES10G12Bit按键板中需要
        Analysis,
        Save,
        View,
        Help,
        TimeBaseMenu,
        Spare,

        TriggerLevel = 100,
        Multipupose,
        HorizontalScale,
        VerticalScale,
        VerticalPosition,
        HorizontalPosition,

        //ES10G12Bit按键板中需要
        MultipuposeA,
        MultipuposeB,
        CH1VerticalScale,
        CH1VerticalPosition,
        CH2VerticalScale,
        CH2VerticalPosition,
        CH3VerticalScale,
        CH3VerticalPosition,
        CH4VerticalScale,
        CH4VerticalPosition,

    };
}
