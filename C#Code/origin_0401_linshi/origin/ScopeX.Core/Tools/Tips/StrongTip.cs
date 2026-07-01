using System;

namespace ScopeX.Core.Tools
{
    public class StrongTip
    {
        public Func<StrongTipEventArgs, Boolean>? ShowHandler
        {
            get;
            set;
        }

        public Boolean Show(MsgTipId MsgTitleId, MsgTipId MsgContentId, MessageType MsgType, params Object[] args)
        {
            StrongTipEventArgs stea = new StrongTipEventArgs(
                EnumOperateMethod.GetEnumFullName(MsgTitleId),
                EnumOperateMethod.GetEnumFullName(MsgContentId),
                MsgType, args);
            return ShowHandler?.Invoke(stea) ?? false;
        }

        private StrongTip()
        { }

        public void Init(Func<StrongTipEventArgs, Boolean> sh)
        {
            ShowHandler = sh;
        }

        public static readonly StrongTip Default = new();
    }
}
