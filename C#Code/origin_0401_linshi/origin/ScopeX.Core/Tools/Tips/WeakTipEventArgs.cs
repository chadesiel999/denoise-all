using System;

namespace ScopeX.Core.Tools
{
    //public class TipEventArgs
    //{
    //    public TipEventArgs(Int32 msgContentId, params Object[] par)
    //    {
    //        MsgContentId = msgContentId;
    //        Par = par;
    //    }

    //    /// <summary>
    //    /// 消息内容Id
    //    /// </summary>
    //    public Int32 MsgContentId { get; set; }
    //    public Object[] Par { get; private set; }
    //}



    public record WeakTipEventArgs(String ContentName, String? Path = "", Int32 Duration = 5, Object? Mark = null, params Object[] Args);

}
