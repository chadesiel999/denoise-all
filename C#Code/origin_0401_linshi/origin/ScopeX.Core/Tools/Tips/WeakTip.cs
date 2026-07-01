using System;

namespace ScopeX.Core.Tools
{
    public class WeakTip : IWeakTip
    {
        private String _MessageName;
        public String MessageName
        {
            get => _MessageName;
            private set
            {
                _MessageName = value;
               // ShowHandler?.Invoke(new(value, Path));
            }
        }
        private Object? _Mark = null;
        public Object? Mark
        {
            get => _Mark;
            private set
            {
                _Mark = value;
               // ShowHandler?.Invoke(new(MessageName, Path, Mark: value));
            }
        }
        private String? _Path;
        public String? Path
        {
            get => _Path;
            private set
            {
                _Path = value;
                //ShowHandler?.Invoke(new(value));
            }
        }

        public Action<WeakTipEventArgs>? ShowHandler
        {
            get;
            set;
        }

        public Boolean Enabled
        {
            get;
            set;
        } = true;

        public Int32 HoldoffByms
        {
            get;
            set;
        }

        public Boolean Emergent
        {
            get;
            private set;
        } = false;

        public Int64 Timestamp
        {
            get;
            private set;
        }

        private Int32 _Duration;
        public Int32 Duration
        {
            get => _Duration;
            private set
            {
                _Duration = value;
                // ShowHandler?.Invoke(new(MessageName, Path, value));
            }
        }

        private static readonly Object _Lock = new();

        public void Write(String sender, MsgTipId tipId, Boolean emergent = false, String? path = "", Int32 duration = 5, Object? mark = null)
        {
            String tipname = EnumOperateMethod.GetEnumFullName(tipId);

            Write(sender, tipname, emergent, path, duration, mark);
        }

        public void Write(String sender, String message, Boolean emergent, String? path, Int32 duration, Object? mark = null)
        {
            if (Enabled)
            {
                lock (_Lock)
                {
                    if (emergent)
                    {
                        if (!Emergent)
                        {
                            Emergent = true;

                            Path = path;
                            MessageName = message;
                            Duration = duration;
                            Mark = mark;
                            Timestamp = ComModel.TimeSpanUtility.GetTimesLongByms();// Stopwatch.GetTimestamp();// DateTime.Now.Ticks;
                            ShowHandler?.Invoke(new(MessageName, Path, Duration, Mark));
                        }
                    }
                    else if (/*DateTime.Now.Ticks*/ComModel.TimeSpanUtility.GetTimesLongByms() > Timestamp + HoldoffByms)
                    {
                        Emergent = false;

                        Path = path;
                        MessageName = message;
                        Duration = duration;
                        Mark = mark;
                        Timestamp = ComModel.TimeSpanUtility.GetTimesLongByms();// Stopwatch.GetTimestamp();// DateTime.Now.Ticks;
                        ShowHandler?.Invoke(new(MessageName, Path, Duration, Mark));
                    }

                    //输出到Log文件
                }
            }
        }

        public void Close()
        {
            ShowHandler?.Invoke(new(String.Empty, String.Empty, 0, String.Empty));
        }

        private WeakTip(Int32 holdoffByms = 500)
        {
            HoldoffByms = holdoffByms;
            _MessageName = EnumOperateMethod.GetEnumFullName(MsgTipId.None);
        }

        public void Init(Action<WeakTipEventArgs> sh)
        {
            ShowHandler = sh;
        }

        public static readonly WeakTip Default = new();
    }
}
