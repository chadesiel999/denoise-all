using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.Jitter.Common
{
    internal static class JitterCommon
    {
        private static Stopwatch _StrongTipSW;
        private static Stopwatch _WeakTipSW;
        private static Int64 _StrongTipElapsedLitmit = 60000;
        private static Int64 _WeakTipElapsedLitmit = 10000;

        internal static void LimitPrintJitterError(MsgTipId errortip, Int32 type = 0)//0是若提示，1是强提示
        {
            switch (type)
            {
                case 0:
                    if (_WeakTipSW == null || _WeakTipSW.ElapsedMilliseconds >= _WeakTipElapsedLitmit)
                    {
                        _WeakTipSW?.Stop(); // 如果 sw 不为 null，停止之前的 Stopwatch
                        _WeakTipSW = Stopwatch.StartNew(); // 重新开始计时
                    }
                    else
                    {
                        // 如果时间间隔小于 10秒，直接返回，不执行操作
                        return;
                    }
                    WeakTip.Default.Write("Print", errortip, duration: 2);
                    break;
                case 1:
                    if (_StrongTipSW == null || _StrongTipSW.ElapsedMilliseconds >= _WeakTipElapsedLitmit)
                    {
                        _StrongTipSW?.Stop(); // 如果 sw 不为 null，停止之前的 Stopwatch
                        _StrongTipSW = Stopwatch.StartNew(); // 重新开始计时
                    }
                    else
                    {
                        // 如果时间间隔小于 10秒，直接返回，不执行操作
                        return;
                    }
                    StrongTip.Default.Show(MsgTipId.Warning, errortip, MessageType.Warning);
                    break;
            }
        }
    }
}
