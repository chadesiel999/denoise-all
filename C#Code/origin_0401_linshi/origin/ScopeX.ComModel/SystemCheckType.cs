using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public enum CheckType
    {
        Close = 0,
        ScreenCheck,
        TouchCheck,
        KeyboardCheck,
        LEDCheck
    }

    #region 屏幕检测模块
    public enum ScreenMaskColor
    {
        Red = 0,
        Green,
        Blue,
        Black,
        White
    }

    #endregion

    #region 触摸检测模块
    public enum TouchTestTextColor
    {
        Red = 0,
        White,
        Black

    }

    #endregion
}
