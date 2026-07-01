using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_LissajousCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetLissajousPrsnt(analyResult, out LissajousPrsnt lissajousPrsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(lissajousPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(lissajousPrsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UsingScientificNotation = usingScientific;
                        returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_LissajousCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetLissajousPrsnt(analyResult, out LissajousPrsnt lissajousPrsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(lissajousPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (scpiTagObj.PropertyName == nameof(lissajousPrsnt.Active) && (param[0].ToString().ToUpper() == "ON" || param[0].ToString() == "1"))
                        {
                            Presenter.SetMutexFunctionFlag();
                        }
                        if (TrySetPropertyValue(lissajousPrsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiQuy_LissajousCursorPos(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return returnResult;
            }
            if (TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.Active == false)
                {
                    return returnResult;
                }
            }
            double? pos = 0.0;
            if (scpiTagObj.PropertyName == nameof(LissajousPrsnt.VCursor))
            {
                pos = prsnt.VCursor[index];
            }
            if (scpiTagObj.PropertyName == nameof(LissajousPrsnt.HCursor))
            {
                pos = prsnt.HCursor[index];
            }
            sendMessage.SendData = decodeStr(Quantity.ConvertByPrefix((double)pos, Prefix.Milli).ToString());
            returnResult = true;
            return returnResult;
        }
        public static bool scpiSet_LissajousCursorPos(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            //CursorPrsnt prsnt = Presenter.Cursor;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return returnResult;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return returnResult;
            }
            if (TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.Active == false)
                {
                    return returnResult;
                }
            }
            List<string> param = ParamListToStrList(analyResult.Params);
            var cursorbarprsnt = scpiTagObj.PropertyName == nameof(LissajousPrsnt.VCursor) ? prsnt.VCursor : prsnt.HCursor;
            if (param.Count > 0 && double.TryParse(param[0], out double value))
            {
                cursorbarprsnt[index] = Quantity.ConvertByPrefix(value, Prefix.Empty, Prefix.Milli);
                returnResult = true;
            }

            return returnResult;
        }
        public static bool scpiQuy_LissajousWaverPos(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return returnResult;
            }
            if (TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.Active == false)
                {
                    return returnResult;
                }
            }
            double? pos = 0.0;
            if (scpiTagObj.PropertyName == nameof(LissajousPrsnt.VCursor))
            {
                pos = Presenter.Cursor.VCursor[index];
            }
            sendMessage.SendData = decodeStr(Quantity.ConvertByPrefix((double)pos, Prefix.Milli).ToString());
            returnResult = true;
            return returnResult;
        }
        public static bool scpiSet_LissajousWavePos(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            //CursorPrsnt prsnt = Presenter.Cursor;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return returnResult;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return returnResult;
            }
            if (TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.Active == false)
                {
                    return returnResult;
                }
            }
            List<string> param = ParamListToStrList(analyResult.Params);
            var cursorbarprsnt = scpiTagObj.PropertyName == nameof(CursorPrsnt.VCursor) ? Presenter.Cursor.VCursor : Presenter.Cursor.HCursor;
            if (param.Count > 0 && double.TryParse(param[0], out double value))
            {
                cursorbarprsnt[index] = Quantity.ConvertByPrefix(value, Prefix.Empty, Prefix.Milli);
                returnResult = true;
            }

            return returnResult;
        }

        public static bool scpiQuy_LissajousMeasureValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return false;
            }
            if (!TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                return false;
            }
            var cursorbarprsnt = scpiTagObj.PropertyName == nameof(LissajousPrsnt.VCursor) ? prsnt.VCursor : prsnt.HCursor;

            switch (scpiTagObj.PropertyName)
            {
                case nameof(LissajousPrsnt.VCursor):
                    VCursorToString(prsnt, (double)cursorbarprsnt[index], prsnt.SourceX, out var vvalue);
                    var sendxvalue = Quantity.ConvertByPrefix((double)vvalue.v, vvalue.p, Prefix.Empty);
                    sendMessage.SendData = decodeStr($"{sendxvalue}");
                    returnResult = true;
                    break;
                case nameof(LissajousPrsnt.HCursor):
                    HCursorToString(prsnt, (double)cursorbarprsnt[index], prsnt.SourceY, out var yvalue);
                    var sendyvalue = Quantity.ConvertByPrefix((double)yvalue.v, yvalue.p, Prefix.Empty);
                    sendMessage.SendData = decodeStr($"{sendyvalue}");
                    returnResult = true;
                    break;
                default:
                    break;
            }

            return returnResult;
        }

        public static bool scpiQuy_LissajousPolarValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                return false;
            }

            switch (scpiTagObj.Tag)
            {
                case "RA":
                    sendMessage.SendData = decodeStr($" {prsnt.PosA.Radius}");
                    returnResult = true;
                    break;
                case "RB":
                    sendMessage.SendData = decodeStr($" {prsnt.PosB.Radius}");
                    returnResult = true;
                    break;
                case "AA":
                    sendMessage.SendData = decodeStr($" {prsnt.PosA.Angle}");
                    returnResult = true;
                    break;
                case "AB":
                    sendMessage.SendData = decodeStr($" {prsnt.PosB.Angle}");
                    returnResult = true;
                    break;
                default:
                    break;
            }

            return returnResult;
        }

        public static bool scpiQuy_LissajousWaveMeasureValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

            if (!TryGetLissajousPrsnt(analyResult, out var prsnt))
            {
                return false;
            }
            IChnlPrsnt xchannel = null;
            if (Presenter.TryGetChannel(prsnt.SourceX, out xchannel) == false)
            {
                return false;
            }

            IChnlPrsnt ychannel = null;
            if (Presenter.TryGetChannel(prsnt.SourceY, out ychannel) == false)
            {
                return false;
            }
            if (xchannel.VuDatabase.Current.Buffer == null || ychannel.VuDatabase.Current.Buffer == null)
            {
                return false;
            }

            var xbuffer = xchannel.VuDatabase.Current.Buffer.Cast<Double>().Select(x => x + 5000).ToArray();
            var ybuffer = ychannel.VuDatabase.Current.Buffer.Cast<Double>().ToArray();

            var start = xchannel.VuDatabase.Current.Start!;
            var posa = ((double)Presenter.Cursor.VCursor[0] - start - Presenter.Cursor.VCursor.MinPosIndex) * xchannel.VuDatabase.Current.ZoomRatio!;
            var posb = ((double)Presenter.Cursor.VCursor[1] - start - Presenter.Cursor.VCursor.MinPosIndex) * xchannel.VuDatabase.Current.ZoomRatio!;

            var aindex = Math.Clamp((Int32)Math.Round(posa), 0, xbuffer.Length);
            var bindex = Math.Clamp((Int32)Math.Round(posb), 0, xbuffer.Length);

            (double X, double Y) posA = (xbuffer[aindex], ybuffer[aindex]);
            (double X, double Y) posB = (xbuffer[bindex], ybuffer[bindex]);

            var value = 0.0D;
            switch (scpiTagObj.Tag.ToString())
            {
                case "0_X":
                    VCursorToString(prsnt, posA.X, prsnt.SourceX, out var ax);
                    value = Quantity.ConvertByPrefix((double)ax.v, ax.p, Prefix.Empty);
                    returnResult = true;
                    break;
                case "0_Y":
                    HCursorToString(prsnt, posA.Y, prsnt.SourceY, out var ay);
                    value = Quantity.ConvertByPrefix((double)ay.v, ay.p, Prefix.Empty);
                    returnResult = true;
                    break;
                case "1_X":
                    VCursorToString(prsnt, posB.X, prsnt.SourceX, out var bx);
                    value = Quantity.ConvertByPrefix((double)bx.v, bx.p, Prefix.Empty);
                    returnResult = true;
                    break;
                case "1_Y":
                    HCursorToString(prsnt, posB.Y, prsnt.SourceY, out var by);
                    value = Quantity.ConvertByPrefix((double)by.v, by.p, Prefix.Empty);
                    returnResult = true;
                    break;
                default:
                    return false;
            }
            sendMessage.SendData = decodeStr($"{value}");

            return returnResult;
        }

        public static bool TryGetLissajousPrsnt(SCPICommandProcessFuncParam analyResult, out LissajousPrsnt lissajousPrsnt)
        {
            lissajousPrsnt = null;
            var id = analyResult.ChannelIndex;
            if (id <= 0 || id > LissajousPrsnt.MAX_LISSA_CNT)
            {
                return false;
            }
            if (LissajousPrsnt.GetorMakeLissajousPrsnt(id, out var xyprsnt))
            {
                if (xyprsnt is LissajousPrsnt prsnt)
                {
                    lissajousPrsnt = prsnt;
                    return true;
                }
            }
            return true;
        }
        private static String VCursorToString(LissajousPrsnt _XYPrsnt, Double vpos, ChannelId sx, out (Double? v, ScopeX.Core.Tools.Prefix p, String u) y)
        {
            var (ay, ayp, ayu) = _XYPrsnt.GetVValueAxisInfo(vpos, sx);
            y = (ay, ayp, ayu);
            var xstr = new ScopeX.Core.Tools.Quantity(ay, ayp, ayu).ToString("##0.###", true);
            return $"{xstr}";
        }

        private static String HCursorToString(LissajousPrsnt _XYPrsnt, Double hpos, ChannelId sy, out (Double? v, ScopeX.Core.Tools.Prefix p, String u) x)
        {
            var (by, byp, byu) = _XYPrsnt.GetHValueAxisInfo(hpos, sy);
            x = (by, byp, byu);
            var ystr = new ScopeX.Core.Tools.Quantity(by, byp, byu).ToString("##0.###", true);
            return $"{ystr}";
        }
    }
}
