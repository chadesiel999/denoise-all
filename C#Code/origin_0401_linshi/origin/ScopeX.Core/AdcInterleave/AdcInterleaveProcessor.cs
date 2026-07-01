using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core
{
    /// <summary>
    /// 交织处理器，单例
    /// </summary>
    public class AdcInterleaveProcessor
    {
        public static readonly AdcInterleaveProcessor Default = new();
        public DsoPrsnt Oscilloscope
        {
            get;
            set;
        }
        private AdcInterleaveProcessor()
        { }

        public void Process()
        {
            DsoModel.Default?.Timebase?.UpdateAnaChannel();
            DsoModel.Default?.Timebase?.UpdateParamsAndResetAcq();
        }

        /// <summary>
        /// 当前模拟通道点数的定义
        /// </summary>
        public IReadOnlyList<KeyValuePair<String, Int32>> AnaChnlLengthSource
        {
            get
            {
                if (DsoModel.Default != null && DsoModel.Default.AnalogChnls != null)
                {
                    ChannelId[] chnls = DsoModel.Default.AnalogChnls.Where((a) => a.Active).Select(o => o.Id).ToArray();
                    String paramstr = $"{AnalogParamEnum.StorageDotsCnt}_{String.Join('_', chnls)}";
                    Hd.TryGetData(ChannelType.Analog, paramstr, out Object? data);
                    if (data != null && data is List<Int32> datalist && datalist.Count > 0)
                    {
                        var storagedotscnt = (List<Int32>)data;
                        var result = storagedotscnt.Select(o => { return new KeyValuePair<String, Int32>(ValueChangeToSI(o, 1, "Pts"), o); }).ToList();
                        return PlatformManager.Default.Platform.GetAnaChnlLengthSource(result, DsoModel.Default.Timebase.Scale, AdcInterleaveMode);
                    }
                }
                //返回默认值
                return new List<KeyValuePair<String, Int32>>()
                {
                    new KeyValuePair<String, Int32>("Default0", 10_000),
                    new KeyValuePair<String, Int32>("Default1", 10_000),
                };
            }
        }

        /// <summary>
        /// 当前交织模式
        /// </summary>
        public AdcInterleaveMode AdcInterleaveMode
        {
            get
            {
                if (DsoModel.Default != null && DsoModel.Default.AnalogChnls != null)
                {
                    List<ChannelId> chnls = new List<ChannelId>();
                    chnls.AddRange(DsoModel.Default.AnalogChnls.Where((a) => a.Active).Select(o => o.Id).ToArray());
                    if (PlatformManager.Default.Platform.IncludeDigitalChnl)
                    {
                        chnls.AddRange(DsoModel.Default.DigitalChnls.Where((a) => a.Active).Select(o => o.Id).ToArray());
                    }
                    String paramstr = $"{AnalogParamEnum.AdcInterleaveMode}_{String.Join('_', chnls)}";
                    Hd.TryGetData(ChannelType.Analog, paramstr, out Object? data);
                    if (data != null && data is AdcInterleaveMode)
                    {
                        return (AdcInterleaveMode)data;
                    }
                }
                //返回默认值
                return AdcInterleaveMode.Mode1To1;
            }
        }

        private String _SICollection = "yzafpnμmDkMGTPEZY";
        /// <summary>
        /// 按SI单位制为数字添加后缀
        /// </summary>
        /// <param name="value">需要转换的数字</param>
        /// <param name="decimals">小数位数</param>
        /// <param name="unit">附加单位</param>
        /// <returns></returns>
        private String ValueChangeToSI(Double value, Int32 decimals = 1, String unit = "", Double hex = 1000, Boolean invalid = true)
        {
            if (hex <= 0) hex = 1000;
            if (Double.IsNaN(value)) return "NaN";
            if (value == 0)
            {
                return value + unit;
            }
            if (Math.Abs(value) < 1E-24 || Math.Abs(value) > 1E24) return Math.Round((Decimal)0f, decimals) + unit;
            String si = _SICollection;
            Double d = Math.Log(Math.Abs(value), hex);
            Decimal number = Math.Round((Decimal)(value / Math.Pow(hex, Math.Floor(d))), decimals);
            d += 8;
            String s = si.Substring((Int32)d, 1);
            if (s == "D") s = "";
            if (!invalid)
            {
                String formatstring = "#0";
                if (decimals > 0)
                {
                    formatstring += ".";
                    for (Int32 i = 0; i < decimals; i++) formatstring += "#";
                }
                return number.ToString(formatstring) + s + unit;
            }
            else
            {
                String formatstr = "{0:#." + String.Join("", Enumerable.Repeat("#", decimals)) + "}{1}{2}";
                return String.Format(formatstr, number, s, unit);
            }
        }
    }
}
