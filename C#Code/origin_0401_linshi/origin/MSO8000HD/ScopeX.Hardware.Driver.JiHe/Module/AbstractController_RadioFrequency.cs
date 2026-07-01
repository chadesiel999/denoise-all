using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal abstract class AbstractController_RadioFrequency
    {
        protected Action? _CtrlConfig;
        protected Action? _CtrlConfigWindow;
        protected Action? _CtrlSpan;
        protected Action? _CtrlSTFTSource;
        public static ChannelId CurrentRFChannel
        {
            get;
            set;
        } = ChannelId.RF1;
        public static Int64 RBWHardware
        {
            get;
            set;
        } = 1024;
        public static Int64 SpanHardware
        {
            get;
            set;
        } = 1024;

        public static void CtrlConfig() => Hd.CurrProduct?.Ctrl_RadioFrequency?._CtrlConfig?.Invoke();
        public static void CtrlConfigWindow() => Hd.CurrProduct?.Ctrl_RadioFrequency?._CtrlConfigWindow?.Invoke();
        public static void CtrlSpan() => Hd.CurrProduct?.Ctrl_RadioFrequency?._CtrlSpan?.Invoke();
        public static void CtrlSTFTSource() => Hd.CurrProduct?.Ctrl_RadioFrequency?._CtrlSTFTSource?.Invoke();
    }
}
