using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Linq;

namespace ScopeX.Core
{
    internal class FunctionLimit
    {
        /// <summary>
        /// 顺序模式互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭互斥功能</param>
        /// <returns></returns>
        public static Boolean FastFrameFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.SegementIsNotSupportedInPowerAnalysis, MessageType.Asking))
                    {
                        foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                        {
                            item.Value.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.SegementIsNotSupportedInJitter, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.SegementIsNotSupportedInPassFail, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var decodelist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.SegementIsNotSupportedInDecode, MessageType.Asking))
                    {
                        foreach (var item in decodelist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// P/F互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭互斥功能</param>
        /// <returns></returns>
        public static Boolean PassFailFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PassFailIsNotSupportedInPowerAnalysis, MessageType.Asking))
                    {
                        foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                        {
                            item.Value.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PassFailIsNotSupportedInJitter, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            var decodelist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PassFailIsNotSupportedInDecode, MessageType.Asking))
                    {
                        foreach (var item in decodelist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PassFailIsNotSupportedInSegment, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Jitter互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭互斥功能</param>
        /// <returns></returns>
        public static Boolean JitterFunctionLimit(Boolean forceClose) => PlatformManager.Default.Platform.JitterFunctionLimit(forceClose);

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="forceClose">是否强制关闭</param>
        /// <returns></returns>
        public static Boolean DecodeFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return true;

            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInPowerAnalysis, MessageType.Asking))
                    {
                        foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                        {
                            item.Value.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInJitter, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInPassFail, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInSegment, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (forceClose)
                {
                    DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInLA, MessageType.Asking))
                    {
                        DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Math互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭</param>
        /// <returns></returns>
        public static Boolean MathFunctionLimit(Boolean forceClose)
        {
            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (forceClose)
                {
                    DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.MathIsNotSupportedInLA, MessageType.Asking))
                    {
                        DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static TimeSpan _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();

        private static Int32 _DigitalLimitTimeOut = 100;

        /// <summary>
        /// Digital互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭</param>
        /// <returns></returns>
        public static Boolean DigitalFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if ((TimeSpanUtility.GetTimestampSpan() - _DigitalLimitTime).TotalMilliseconds < _DigitalLimitTimeOut)
                {
                    return false;
                }
                if (forceClose)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalIsNotSupportedInPowerAnalysis, MessageType.Asking))
                    {
                        foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                        {
                            item.Value.Active = false;
                        }
                    }
                    else
                    {
                        _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();
                        return false;
                    }
                }
            }

            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if ((TimeSpanUtility.GetTimestampSpan() - _DigitalLimitTime).TotalMilliseconds < _DigitalLimitTimeOut)
                {
                    return false;
                }
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalIsNotSupportedInJitter, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                    }
                    else
                    {
                        _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();
                        return false;
                    }
                }
            }
            var decodelist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if ((TimeSpanUtility.GetTimestampSpan() - _DigitalLimitTime).TotalMilliseconds < _DigitalLimitTimeOut)
                {
                    return false;
                }
                if (forceClose)
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalIsNotSupportedInDecode, MessageType.Asking))
                    {
                        foreach (var item in decodelist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();
                        return false;
                    }
                }
            }

            var mathlist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsMath() && c.Active).Cast<MathPrsnt>();
            if (mathlist != null && mathlist.Count() > 0)
            {
                if ((TimeSpanUtility.GetTimestampSpan() - _DigitalLimitTime).TotalMilliseconds < _DigitalLimitTimeOut)
                {
                    return false;
                }
                if (forceClose)
                {
                    foreach (var item in mathlist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalIsNotSupportedInMath, MessageType.Asking))
                    {
                        foreach (var item in mathlist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();
                        return false;
                    }
                }
            }

            if (LissajousPrsnt.LissaLength > 0)
            {
                if ((TimeSpanUtility.GetTimestampSpan() - _DigitalLimitTime).TotalMilliseconds < _DigitalLimitTimeOut)
                {
                    return false;
                }
                if (forceClose)
                {
                    LissajousPrsnt.TryTryRemoveAll();//TOdo
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalIsNotSupportedInLissajous, MessageType.Asking))
                    {
                        LissajousPrsnt.TryTryRemoveAll();//TOdo
                    }
                    else
                    {
                        _DigitalLimitTime = TimeSpanUtility.GetTimestampSpan();
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Lissajous互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭</param>
        /// <returns></returns>
        public static Boolean LissajousFunctionLimit(Boolean forceClose)
        {
            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (forceClose)
                {
                    DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.LissajousIsNotSupportedInDigital, MessageType.Asking))
                    {
                        DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Pwr互斥检测
        /// </summary>
        /// <param name="forceClose">是否强制关闭</param>
        /// <returns></returns>
        public static Boolean PwrAnalysisFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.Jitter?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInJitter, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Jitter.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInPassFail, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var decodelist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInDecode, MessageType.Asking))
                    {
                        foreach (var item in decodelist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInSegment, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (forceClose)
                {
                    DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PwrAnalysisIsNotSupportedInLA, MessageType.Asking))
                    {
                        DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
