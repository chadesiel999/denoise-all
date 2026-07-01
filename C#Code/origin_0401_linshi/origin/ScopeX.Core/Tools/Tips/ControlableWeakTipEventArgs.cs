using System;

namespace ScopeX.Core.Tools.Tips
{
    /// <summary>
    /// 可控弱提示框的事件消息参数
    /// </summary>
    /// <param name="type">控制类型</param>
    /// <param name="TipId">唯一ID标识</param>
    /// <param name="ContentName">提示内容多语言ID</param>
    /// <param name="Path"></param>
    /// <param name="Args"></param>
    public record ControlableWeakTipEventArgs(ControlableWeakTipEventControlType type, Guid TipId, String ContentName, String? Path = "", params Object[] Args);
}
