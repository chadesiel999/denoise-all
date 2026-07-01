using EventBus;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using NPOI.SS.Formula.Functions;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Histgram))]
    public class MathHistArg : MathArgPrsnt
    {
        public MathHistArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.Histgram)
        {
            _Args = ParseFormula(formula);
        }

        private HistArgs _Args;

        public HistParamter HistParamter => _Args.HistParamter;


        public ChannelId Source
        {
            get => _Args.Source;
            set
            {
                if (_Args.Source != value)
                {
                    _Args.Source = value;

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        public Int32 NBins
        {
            get => _Args.HistParamter.NBins;
            set
            {
                value = ValidateNBins(value);
                if (_Args.HistParamter.NBins != value)
                {
                    _Args.HistParamter.NBins = value;

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        private static Int32 ValidateNBins(Int32 value)
        {
            if (value < MinNBins)
                value = MinNBins;
            else if (value > MaxNBins)
                value = MaxNBins;

            return value;
        }

        public static readonly Int32 MinNBins = Constants.MIN_HIST_BIN_CNT;

        public static readonly Int32 MaxNBins = Constants.MAX_HIST_BIN_CNT;

        private Int64 _TimeStamp = 0;

        private ScopeX.Measure.Histogram? _Parameters;

        public ScopeX.Measure.Histogram? CalcParameters()
        {
            //!!!Ambiguity Histogram
            //var pkg = DsoModel.Default.GetWfmPack(Model.Id);
            //if (pkg is not null)
            //{
            //    if (pkg.Properties.Stamp > _TimeStamp)
            //    {
            //        _TimeStamp = pkg.Properties.Stamp;
            //        _Parameters = new(NBins, pkg.Buffer.GetRow(0));
            //    }
            //    return _Parameters;
            //}

            if (Source.IsMeasure())
            {
                Int32 index = DsoModel.Default.Meas.SelectedItems.ToList().FindIndex(x => x.Id == Source);
                if (index >= 0)
                {
                    var datas = DsoModel.Default.Meas.Calc.StatBuffer[index].ToArray();
                    if (datas == null || datas.Length == 0)
                    {
                        _Parameters = null;
                        return null;
                    }
                    _Parameters = new Measure.Histogram(NBins, datas);
                    return _Parameters;
                }
            }
            else if (Source == ChannelId.DVM)
            {
                var datas = DsoPrsnt.DefaultDsoPrsnt.Voltmeter.StaBuffer.ToArray();
                if (datas == null || datas.Length == 0)
                {
                    _Parameters = null;
                    return null;
                }
                _Parameters = new Measure.Histogram(NBins, datas);
                return _Parameters;
            }
            else if (Source == ChannelId.CYM)
            {
                var datas = DsoPrsnt.DefaultDsoPrsnt.Cymometer.StaBuffer.ToArray();
                if (datas == null || datas.Length == 0)
                {
                    _Parameters = null;
                    return null;
                }
                _Parameters = new Measure.Histogram(NBins, datas);
                return _Parameters;
            }
            else
            {
                var pkg = DsoModel.Default.GetWfmPack(Source);
                if (pkg is not null)
                {
                    if (pkg.Properties.Stamp > _TimeStamp)
                    {
                        _TimeStamp = pkg.Properties.Stamp;
                        _Parameters = new(NBins, pkg.Buffer.GetRow(0));
                    }
                    return _Parameters;
                }
            }
            return null;
        }

        public override String Description
        {
            get
            {
                if (ChannelIdExt.IsMeasure(Source))
                {
                    var selected = DsoModel.Default.Meas.SelectedItems[Source - ChannelIdExt.MinPChId];
                    var pfxunit = DsoModel.Default.Meas.Calc.GetPfxUnitString(Source - ChannelIdExt.MinPChId);

                    //return $"直方图({Source}:{selected.Name},{selected.Source})";
                    return $"({Source})";
                }
                else
                {
                    return $"({Source})";
                }

            }
        }

        public override String MakeFormula()
        {
            return $"{MathType.Histgram}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        internal sealed class HistArgs
        {
            public ChannelId Source { get; set; }
            [NotNull]
            public HistParamter HistParamter { get; set; } = new HistParamter();
        }

        internal static HistArgs ParseFormula(String formula)
        {
            var exp = formula;
            if (MathArgPrsnt.TryParse(exp, out var arg))
            {
                if (MathType.Histgram != arg.Value.ExpType)
                    return new HistArgs()
                    {
                        Source = ChannelId.C1,
                        HistParamter = new HistParamter()
                        {
                            NBins = 100,
                        }
                    };
                exp = arg.Value.Exp;
            }

            String name = "Execute.Hist(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new HistArgs()
                {
                    Source = Enum.Parse<ChannelId>(substr[1]),
                    HistParamter = new HistParamter()
                    {
                        NBins = Int32.Parse(substr[2]),
                    },
                };
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct Hist(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct Hist(...) expression.");
        }

        internal static String MakeFormula(HistArgs ha)
        {
            return $"Execute.Hist({ha.Source}, {ha.HistParamter.NBins})";
        }
        private static Int32 GetLastNotZero(Double[,] value)
        {
            for (Int32 index = value.Length - 1; index >= 0; index--)
            {
                if (value[0, index] != 0) return index;
            }
            return -1;
        }


        internal static WfmProperties Config(MathModel mch, String exp, Vector? res)
        {
            Int32 minbins = 20;
            mch.Sampling.Prefix = Prefix.Micro;
            Double si = res?.SampInterval ?? 1;
            if (mch.Args is MathHistArg histarg && (histarg.Source.IsMeasure() || histarg.Source == ChannelId.DVM || histarg.Source == ChannelId.CYM) && MathVecBuffer.Default.TryGetVector(((MathHistArg)mch.Args).Source.ToString(), out var val) && res != null)
            {
                var tempbuffer = val.Elements.Cast<Double>().Where(x => !Double.IsNaN(x)).ToList();
                switch (res.XUnit)
                {
                    case "%":
                        tempbuffer = tempbuffer.Select(x => x * 1E8).ToList();
                        si = si * 1E8;
                        break;
                    case "s":
                    case "Hz":
                    case " ":
                        if (histarg.Source == ChannelId.CYM)
                        {
                            tempbuffer = tempbuffer.Select(x => x * 1E3).ToList();
                            si = si * 1E3;
                        }
                        else
                        {
                            tempbuffer = tempbuffer.Select(x => x * 1E6).ToList();
                            si = si * 1E6;
                        }
                        break;
                }
                if (tempbuffer.Count > 0)
                {
                    var max = tempbuffer.Max();
                    var min = tempbuffer.Min();

                    if (Math.Abs((((Decimal)max - (Decimal)min) / histarg.HistParamter.NBins / (Decimal)max)) < Algorithm.HistEpsilon)
                    {
                        histarg.HistParamter.MaxValue = (Double)((Decimal)si * (Decimal)histarg.HistParamter.NBins * (Decimal)2.0 - (Decimal)si);
                        histarg.HistParamter.MinValue = si;
                    }
                    else
                    {
                        histarg.HistParamter.MaxValue = max;
                        histarg.HistParamter.MinValue = min;
                    }
                }
                else
                {
                    histarg.HistParamter.MaxValue = 0;
                    histarg.HistParamter.MinValue = 0;
                }
                histarg.HistParamter.Total = 0;
                for (Int32 bin = 0, l = res.Elements.GetLength(1); bin < l; bin++)
                {
                    histarg.HistParamter.Total += (Int32)res.Elements[0, bin];
                }
                histarg.HistParamter.BinZoomRatio = 1;
                Int32 index = histarg.HistParamter.NBins;
                histarg.HistParamter.FixedBins = index + 1;
                if (mch.AutoScale && res.Elements.Length > 0)
                {
                    Double width = 0;
                    if (index <= minbins)
                    {
                        width = (histarg.HistParamter.MaxValue - histarg.HistParamter.MinValue) * res.Elements.Length;
                    }
                    else
                    {
                        width = (histarg.HistParamter.MaxValue - histarg.HistParamter.MinValue) / (Double)res.Elements.Length * (index + 1);
                    }
                    Double autoscale = 0;
                    if (mch.VuDatabase != null && mch.VuDatabase.Current != null)
                    {
                        autoscale = (Double)(((Decimal)histarg.HistParamter.MaxValue - (Decimal)histarg.HistParamter.MinValue) * (Decimal)mch.VuDatabase.Current.ZoomRatio / (Decimal)histarg.HistParamter.FixedBins * (Decimal)1E9);
                    }
                    var scaleinfo = mch.GetHistSampleScale(width);
                    mch.Sampling.InitialScale = (0, scaleinfo.ScaleValue);
                    mch.Sampling.ScaleMaxIndex = scaleinfo.MaxScaleIndex;
                    mch.Sampling.ScaleMinIndex = scaleinfo.MinScaleIndex;
                    mch.Sampling.Scale = scaleinfo.ScaleValue;
                    mch.Sampling.AutoScale = autoscale == 0 ? scaleinfo.ScaleValue : autoscale;
                    mch.Sampling.PosMaxIndex = 1E200;
                    mch.Sampling.PosMinIndex = -1E200;
                    if (index < minbins)
                    {
                        mch.Sampling.PosIndex = (Double)((Decimal)Constants.MIN_XPOS_TIME - ((Decimal)width / ((Decimal)scaleinfo.ScaleValue / (Decimal)1E6 * (Decimal)Constants.VIS_XDIVS_NUM) * ((Decimal)Constants.VIS_XDIVS_NUM * (Decimal)Constants.IDX_PER_XDIV) / (Decimal)2));
                    }
                    else
                    {
                        mch.Sampling.PosIndex = (Double)((Decimal)Constants.MIN_XPOS_TIME - ((((Decimal)histarg.HistParamter.MaxValue - (Decimal)histarg.HistParamter.MinValue) / (Decimal)res.Elements.Length * (Decimal)(index + 1)) / ((Decimal)scaleinfo.ScaleValue / (Decimal)1E6 * (Decimal)Constants.VIS_XDIVS_NUM) * ((Decimal)Constants.VIS_XDIVS_NUM * (Decimal)Constants.IDX_PER_XDIV) / (Decimal)2));
                    }
                }
            }
            mch.Conditioning.PosIndex = (Double)((Decimal)Constants.IDX_PER_YDIV * (Decimal)Constants.VIS_YDIVS_NUM * -(Decimal)0.5);
            if (mch.AutoScale)
            {
                mch.Conditioning.InitialScale = (0, 1_000_000);
                mch.Conditioning.ScaleMinIndex = -10;
                mch.Conditioning.ScaleMaxIndex = 10;
                mch.Conditioning.Prefix = Prefix.Milli;

                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, res);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.InitFlag = false;

                mch.Conditioning.Unit = mch.IsAutoUnit ? (res?.YUnit ?? "?") : mch.CustomUnit;
            }
            mch.Sampling.Unit = res?.XUnit ?? "?";
            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = mch.Conditioning.InitialScale,
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, 0),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),

                DrawMethod = DrawMethod.Bar,
            };

            prop.SampInterval = si;

            return prop;
        }
        #endregion
    }
}
