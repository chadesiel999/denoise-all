using System;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public interface IWeakTip
    {
        void Write(String sender, MsgTipId tipId, Boolean emergent = false, String? path = "", Int32 duration = 5, Object? mark = null);
    }
}
