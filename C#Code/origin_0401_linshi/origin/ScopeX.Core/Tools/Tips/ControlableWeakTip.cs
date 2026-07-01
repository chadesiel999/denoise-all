using System;

namespace ScopeX.Core.Tools.Tips
{
    /// <summary>
    /// 可控弱提示
    /// </summary>
    public class ControlableWeakTip
    {
        private String _MessageName;
        public String MessageName
        {
            get => _MessageName;
            private set
            {
                _MessageName = value;
                ShowHandler?.Invoke(new(Type, MessageFormId, value, Path));
            }
        }

        public String Path { get; private set; }

        public Action<ControlableWeakTipEventArgs>? ShowHandler
        {
            get;
            set;
        }

        public ControlableWeakTipEventControlType Type { get; private set; }

        public Guid MessageFormId { get; private set; }

        private static readonly Object _Lock = new();

        public void Write(String sender, ControlableWeakTipEventControlType controlType, Guid msgFormID, MsgTipId tipId, String? path = "")
        {
            String tipname = EnumOperateMethod.GetEnumFullName(tipId);
            Write(sender, controlType, msgFormID, tipname, path);
        }

        public void Write(String sender, ControlableWeakTipEventControlType controlType, Guid guid, String message, String? path)
        {
            lock (_Lock)
            {
                Path = path;
                MessageFormId = guid;
                Type = controlType;
                MessageName = message;
            }
        }

        private ControlableWeakTip()
        {
            _MessageName = EnumOperateMethod.GetEnumFullName(MsgTipId.None);
        }

        public void Init(Action<ControlableWeakTipEventArgs> sh)
        {
            ShowHandler = sh;
        }

        public static readonly ControlableWeakTip Default = new();
    }
}
