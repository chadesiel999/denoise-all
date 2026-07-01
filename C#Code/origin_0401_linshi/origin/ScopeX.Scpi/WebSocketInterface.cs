using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core;

namespace ScopeX.Scpi
{
    internal class WebSocketInterface
    {
        public static Action<byte, int>? GetAction_KeyboradEventHandler()
        {
            return WidgetPrsnt.KeyboradEventHandler;
        }
        public static Action<int, int>? GetAction_SetMousePosHandler()
        {
            return WidgetPrsnt.SetMousePosHandler;
        }
        public static Action<uint, int>? GetAction_MouseEventHandler()
        {
            return WidgetPrsnt.MouseEventHandler;
        }
        private static string? _GetImageBase64String()
        {
            return FilePrsnt.GetImageBase64String();
        }
        public static Func<string?> GetFunc_GetImageBase64String()
        {
            return _GetImageBase64String;
        }
    }
}
