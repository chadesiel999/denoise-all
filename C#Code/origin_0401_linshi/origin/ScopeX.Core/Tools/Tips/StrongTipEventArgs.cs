using System;

namespace ScopeX.Core.Tools
{
    //public class MsgEventArgs
    //{
    //    public MsgEventArgs(Int32 msgTitleId, Int32 msgContentId, MessageType msgType,params Object[] par)
    //    {
    //        MsgTitleId = msgTitleId;
    //        MsgContentId = msgContentId;
    //        MsgType = msgType;
    //        Par = par;
    //    }
    //    /// <summary>
    //    /// 消息标题Id
    //    /// </summary>
    //    public Int32 MsgTitleId { get; set; }
    //    /// <summary>
    //    /// 消息内容Id
    //    /// </summary>
    //    public Int32 MsgContentId { get; set; }
    //    /// <summary>
    //    /// 消息类型
    //    /// </summary>
    //    public MessageType MsgType { get; set; }
    //    public Object[] Par{ get;private set; }

    //}


    public record StrongTipEventArgs(String MsgTitleName, String MsgContentName, MessageType MsgType, params Object[] Args);
}
