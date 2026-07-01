using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using EventBus;
using ScopeX.Core.Tools;
using System.Reflection;

namespace ScopeX.U2
{
    internal static class KeyBoardStateManager
    {
        public static KeyBoardState CurrentKeyBoardState = KeyBoardState.None;

        public static Boolean IsInKeyBoardState(KeyBoardState state)
        {
            return state == CurrentKeyBoardState;
        }

        /// <summary>
        /// 按键板当前调节的按键状态
        /// </summary>
        internal enum KeyBoardState
        {
            None,
            VerticalPos,
            VerticalScale,
            TimebaseScale,
            TriggerDelay,
            TriggerCompPos,
        }

    }
}
