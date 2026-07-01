using EventBus;
using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Linq;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Zoom))]
    public class MathZoomArg :MathArgPrsnt
    {
        public MathZoomArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.Zoom)
        {
            _Args = ParseFormula(formula);
        }

        private ZoomArgs _Args;

        public ChannelId Source
        {
            get => _Args.Source;
            set
            {
                if (_Args.Source != value)
                {
                    _Args = _Args with { Source = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public override String Description => $"Zoom({Source})";

        public override String MakeFormula() =>
            //Int32 width = 1;
            //Int32 start = 0;

            //if (DsoModel.Default.TryGetChannel(_Args.Source, out var cm))
            //{
            //    width = (Int32)(Model.Sampling.ScaleByus / cm!.Pack!.Properties.TmbScale.Value * cm.Pack.Buffer.Length);

            //    start = (Int32)(Model.Sampling.PosIndex / Constants.MAX_XPOS_IDX * cm.Pack.Buffer.Length - width / 2);
            //}

            ////Model.Formula = $"{MathType.Zoom}:{ToFormula(_Parameters, start, width)}";
            //return $"{MathType.Zoom}:{ToFormula(_Args, start, width)}";

            //!!!Notice: Zoom function is implemented by copying the source channel's buffer, rather than copying a certain segment of its buffer.
            $"{MathType.Zoom}:{MakeFormula(_Args, 1, 0, 0)}";

        #region Validity And Configuration
        internal sealed record ZoomArgs(ChannelId Source);

        internal static ZoomArgs ParseFormula(String formula)
        {
            //var exp = formula;
            //var sep = exp.IndexOf(":");
            //if (sep >= 0)
            //{
            //    if (MathType.Zoom != Enum.Parse<MathType>(exp[0..sep]))
            //        return new(ChannelId.C1);
            //    exp = exp[(sep + 1)..];
            //}

            var exp = formula;
            if (MathArgPrsnt.TryParse(exp, out var arg))
            {
                if (MathType.Zoom != arg.Value.ExpType)
                {
                    return new(ChannelId.C1);
                }

                exp = arg.Value.Exp;
            }

            String name = "Execute.SubVector(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new ZoomArgs(
                    Enum.Parse<ChannelId>(substr[1]));
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct Zoom(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct Zoom(...) expression.");
        }

        internal static String MakeFormula(ZoomArgs za, Int32 stride, Int32 start, Int32 width) => $"Execute.SubVector({za.Source}, {stride}, {start}, {width})";

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            var za = ParseFormula(exp);

            Double cscale = 1;
            Double tscale = 1;
            Double start = 0;
            if (DsoModel.Default.TryGetChannel(za.Source, out var sch))
            {
                if (sch.Pack is not null)
                {
                    cscale = sch.Pack.Properties.ChnlScale.Value;
                    tscale = sch.Pack.Properties.TmbScale.Value;
                    start = sch.Pack.Properties.VuStartIndex;
                }
                else
                {
                    cscale = sch.Conditioning.Scale;
                    tscale = sch.Sampling.Scale;
                }
            }

            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (0, cscale);
                mch.Conditioning.ScaleMinIndex = -10;
                mch.Conditioning.ScaleMaxIndex = 10;
                mch.Conditioning.Prefix = Prefix.Milli;

                mch.Sampling.InitialScale = (0, tscale);
                mch.Sampling.ScaleMinIndex = -5;
                mch.Sampling.ScaleMaxIndex = 0;
                //mch.Sampling.PosDefIndex = Constants.DEF_XPOS_IDX;
                mch.Sampling.Prefix = Prefix.Micro;
            }
            else
            {
                mch.Conditioning.SetInitScaleValue(0, cscale, -10, 10, false);
                mch.Sampling.SetInitScaleValue(0, tscale, -5, 0, mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId);
            }

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                //mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                //mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;

            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),

                VuStartIndex = start,
            };

            prop.SampInterval = vec?.SampInterval ?? 1;
            //prop.VuFactor = prop.TmbScale.Value * 1E-6 / mch.Sampling.PosIdxPerDiv / prop.SampInterval;

            //Boolean init = false;
            //if (mch.Pack is not null)
            //{
            //    if (mch.Pack.Properties.Stamp < wfmpkg?.Properties.Stamp)
            //        init = true;
            //}
            //else
            //    init = true;

            //if (!init)
            //    return;

            return prop;
        }


        #endregion
    }
}
