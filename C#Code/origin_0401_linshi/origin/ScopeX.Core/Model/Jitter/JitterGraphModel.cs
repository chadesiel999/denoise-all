using NPOI.SS.Formula;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Channels;

namespace ScopeX.Core
{
    internal class JitterGraphModel : AdvancedMathModel
    {
        public JitterGraphModel(String formula, DrawMethod drawMethod) : base(formula, drawMethod)
        {
            InitChannelProperties(formula);
        }

        public JitterGraphModel(String formula) : base(formula)
        {
            InitChannelProperties(formula);
        }

        public override void InitChannelProperties(String formula)
        {
            switch (formula)
            {
                case Constants.JITTER_HISTOGRAM_FORMULA:
                    InitJitterHist();
                    break;
                case Constants.JITTER_TREND_FORMULA:
                    InitJitterTrend();
                    break;
                case Constants.JITTER_SPECTRUM_FORMULA:
                    InitJitterSpectrum();
                    break;
                case Constants.JITTER_QFACTOR_FORMULA:
                    InitJitterQWave();
                    break;
                case Constants.JITTER_BATHTUB_FORMULA:
                    InitJitterBathWave();
                    break;
                case Constants.JITTER_EYE_FORMULA:
                    InitJitterEye();
                    break;
                default:
                    break;
            }
        }

        private void InitJitterHist()
        {

        }
        private void InitJitterTrend()
        {

        }
        private void InitJitterSpectrum()
        {

        }
        private void InitJitterQWave()
        {

        }
        private void InitJitterBathWave()
        {

        }
        private void InitJitterEye()
        {

        }
        public override WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            switch (Formula)
            {
                case Constants.JITTER_HISTOGRAM_FORMULA:
                    return ConfigJitterHist(mch, exp, vec);
                case Constants.JITTER_TREND_FORMULA:
                    return ConfigJitterTrend(mch, exp, vec);
                case Constants.JITTER_SPECTRUM_FORMULA:
                    return ConfigJitterSpectrum(mch, exp, vec);
                case Constants.JITTER_QFACTOR_FORMULA:
                    return ConfigJitterQWave(mch, exp, vec);
                case Constants.JITTER_BATHTUB_FORMULA:
                    return ConfigJitterBathWave(mch, exp, vec);
                case Constants.JITTER_EYE_FORMULA:
                    return ConfigJitterEye(mch, exp, vec);
                default:
                    return new WfmProperties("null");
            }
        }
        private static Int32 GetLastNotZero(Double[,] value)
        {
            for (Int32 index = value.Length - 1; index >= 0; index--)
            {
                if (value[0, index] != 0) return index;
            }
            return -1;
        }

        internal WfmProperties ConfigJitterHist(MathModel mch, String exp, Vector? res)
        {

            Int32 minbins = 20;
            mch.Sampling.Prefix = Prefix.Micro;
            (Single ScaleValue, Int32 MinScaleIndex, Int32 MaxScaleIndex)? sampingscaleinfo = null;
            if (mch.Args is MathCustomArg histarg && res != null)
            {
                var id = histarg.Source?.ToString().ToString();
                if (id != null && MathVecBuffer.Default.TryGetVector(id, out var val))
                {
                    var tempbuffer = val.Elements.Cast<Double>().Where(x => !Double.IsNaN(x)).ToList();
                    if (tempbuffer.Count > 0)
                    {
                        //histArg.HistParamter.MaxValue = tempbuffer.Max()/* * val.SampInterval*/;
                        //histArg.HistParamter.MinValue = tempbuffer.Min()/* * val.SampInterval*/;
                        //histArg.HistParamter.MaxValue = DsoModel.Default.JitterModel.HistRangeMax/* * val.SampInterval*/;
                        //histArg.HistParamter.MinValue = DsoModel.Default.JitterModel.HistRangeMin/* * val.SampInterval*/;
                        histarg.HistParamter.MaxValue = val.SampInterval;
                        histarg.HistParamter.MinValue = val.RefSampPos;
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
                    Int32 index = GetLastNotZero(res.Elements);
                    histarg.HistParamter.FixedBins = index + 1;

                    mch.Sampling.ScaleMaxIndex = 80;
                    mch.Sampling.ScaleMinIndex = -80;

                    //if (!mch.AutoScale && res.Elements.Length > 0)
                    if (mch.AutoScale)
                    {
                        Double width = 0;
                        if (index <= minbins)
                        {
                            width = res.SampInterval * res.Elements.Length;
                        }
                        else
                        {
                            width = (histarg.HistParamter.MaxValue - histarg.HistParamter.MinValue) / (Double)res.Elements.Length * (index + 1);
                        }
                        sampingscaleinfo = mch.GetHistSampleScale(width);
                        mch.Sampling.InitialScale = (0, sampingscaleinfo.Value.ScaleValue);
                        mch.Sampling.ScaleMaxIndex = sampingscaleinfo.Value.MaxScaleIndex;
                        mch.Sampling.ScaleMinIndex = sampingscaleinfo.Value.MinScaleIndex;
                        mch.Sampling.Scale = sampingscaleinfo.Value.ScaleValue;
                        if (index > minbins)
                        {
                            mch.Sampling.PosIndex = Constants.MIN_XPOS_TIME - (width / (sampingscaleinfo.Value.ScaleValue / 1E6 * Constants.VIS_XDIVS_NUM) * (Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV) / 2);
                        }
                        else if (res.Elements.Length != 0)
                        {
                            mch.Sampling.PosIndex = Constants.MIN_XPOS_TIME - (((histarg.HistParamter.MaxValue - histarg.HistParamter.MinValue) / (Double)res.Elements.Length * (index + 1)) / (sampingscaleinfo.Value.ScaleValue / 1E6 * Constants.VIS_XDIVS_NUM) * (Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV) / 2);
                        }
                    }

                }

            }
            mch.Conditioning.PosIndex = Constants.IDX_PER_YDIV * Constants.VIS_YDIVS_NUM * -0.5;
            if (mch.AutoScale)
            //if (mch.AutoScale)
            {
                mch.Conditioning.InitialScale = (0, 1_000_000);
                mch.Conditioning.ScaleMinIndex = -20;
                mch.Conditioning.ScaleMaxIndex = 20;
                mch.Conditioning.Prefix = Prefix.Milli;

                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, res);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.InitFlag = false;

                mch.Conditioning.Unit = res?.YUnit ?? "?";
            }
            Double si = res?.SampInterval ?? 1;
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
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            prop.SampInterval = si;


            if (mch.Args is MathCustomArg arg && arg.HistParamter != null)
            {
                if (Int32.TryParse(DsoModel.Default.JitterModel.CurrentBinNum.GetDescription(), out Int32 binnum))
                {
                    arg.HistParamter.NBins = binnum;
                }
                if (res != null)
                {
                    arg.HistParamter.MaxValue = res.SampInterval;
                    arg.HistParamter.MinValue = res.RefSampPos;
                    if (mch.Args is MathCustomArg hisargs && hisargs != null && mch.AutoScale && mch.VuDatabase != null && mch.VuDatabase.Current != null && sampingscaleinfo.HasValue)
                    {
                        Double autoscale = (hisargs.HistParamter.MaxValue - hisargs.HistParamter.MinValue) * mch.VuDatabase.Current.Buffer.Length / (Constants.IDX_PER_XDIV * Constants.VIS_XDIVS_NUM * 0.6f * hisargs.HistParamter.FixedBins) * 1E9;
                        mch.Sampling.AutoScale = autoscale == 0 ? sampingscaleinfo.Value.ScaleValue : autoscale;
                    }

                }
            }

            return prop;
            //var si = vec?.SampInterval ?? 1;
            //var cscale = 1000;
            //var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            //var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            //mch.Conditioning.InitialScale = (0, cscale);
            //mch.Conditioning.ScaleMaxIndex = 30;
            //mch.Conditioning.ScaleMinIndex = -20;
            //if (mch.Args?.Occupier is not null)
            //{
            //    mch.Conditioning.Prefix = Prefix.Milli;
            //}

            //mch.Sampling.InitialScale = (0, tscale);
            //mch.Sampling.ScaleMaxIndex = 2;
            //mch.Sampling.ScaleMinIndex = -5;
            //mch.Sampling.Prefix = Prefix.Pico;

            //mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            //if (mch.InitFlag)
            //{
            //    //(Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
            //    //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
            //    //mch.Sampling.ScaleIndex = scale.HScaleIndex;
            //    mch.Conditioning.ScaleIndex = 0;
            //    mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

            //    mch.Sampling.ScaleIndex = 0;
            //    mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
            //    //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
            //    mch.InitFlag = false;
            //    //ScaleFit(mch, vec);
            //}

            //if (mch.Conditioning.IsAutoUnit)
            //{
            //    mch.Conditioning.Unit = vec?.YUnit ?? "?";
            //}
            //mch.Sampling.Unit = vec?.XUnit ?? "?";

            //var prop = new WfmProperties(mch.Name)
            //{
            //    ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
            //    ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
            //    ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

            //    //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
            //    TmbPosition = (vec?.RefSampPos ?? Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(vec?.RefSampPos ?? Constants.DEF_XPOS_IDX)),
            //    TmbScale = mch.Sampling.InitialScale,
            //    TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            //};

            //if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            //{
            //    prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            //}

            //prop.SampInterval = si;

            //return prop;
        }
        internal WfmProperties ConfigJitterTrend(MathModel mch, String exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Pico;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 2;
            mch.Sampling.ScaleMinIndex = -5;
            mch.Sampling.Prefix = Prefix.Micro;

            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                //(Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 2;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = vec?.YUnit ?? "?";
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = si;

            return prop;
        }
        internal WfmProperties ConfigJitterSpectrum(MathModel mch, String exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (-10, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Pico;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -50;
            mch.Sampling.Prefix = Prefix.Micro;

            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                //(Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = vec?.YUnit ?? "?";
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbPosition = (vec?.RefSampPos ?? Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(vec?.RefSampPos ?? Constants.DEF_XPOS_IDX)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = si;

            return prop;
        }
        internal WfmProperties ConfigJitterQWave(MathModel mch, String exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Milli;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 2;
            mch.Sampling.ScaleMinIndex = -5;
            mch.Sampling.Prefix = Prefix.Micro;

            //mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                //mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                //mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = vec?.YUnit ?? "?";
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = si;

            return prop;
        }
        internal WfmProperties ConfigJitterBathWave(MathModel mch, String exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * (length == 0 ? 1 : length) / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 7;
            mch.Conditioning.ScaleMinIndex = -7;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Milli;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 0;
            mch.Sampling.ScaleMinIndex = 0;
            mch.Sampling.Prefix = Prefix.Micro;

            //mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 1;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                mch.Conditioning.PosIndex = 3000;



                //mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = vec?.YUnit ?? "?";
            //mch.Sampling.Unit = vec?.XUnit ?? "?";
            mch.Sampling.Unit = "UI";//固定为UI单位

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = si;

            return prop;
        }
        internal WfmProperties ConfigJitterEye(MathModel mch, String exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = (Double)Math.Round((Decimal)si * (Decimal)1E6 * (Decimal)length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Milli;
            }

            if (mch.Sampling.InitialScale.Value == 0 || (mch.Sampling.InitialScale.Value - tscale) / (mch.Sampling.InitialScale.Value) > 0.05)
            {
                mch.Sampling.InitialScale = (0, (Double)tscale);
                mch.Sampling.ScaleMaxIndex = 2;
                mch.Sampling.ScaleMinIndex = -5;
                mch.Sampling.Prefix = Prefix.Micro;
            }

            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                //(Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
                //ScaleFit(mch, vec);
            }
            if (exp == "Execute.JitterEye()")
            {
                ChannelModel chm;
                if (DsoModel.Default.JitterModel.Source.IsAnalog())
                {
                    chm = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);
                    mch.Conditioning.ScaleIndex = (Int32)chm.Conditioning.ScaleIndex - 10;

                    //档位限制，眼图下禁止手动调节
                    mch.Conditioning.ScaleMaxIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                    mch.Conditioning.ScaleMinIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                }
                else if (DsoModel.Default.JitterModel.Source.IsReference())
                {
                    if(DsoModel.Default.TryGetChannel(DsoModel.Default.JitterModel.Source,out var chnl)&&chnl is ReferenceModel @ref&&@ref!=null)
                    {
                        chm = @ref;
                        mch.Conditioning.ScaleIndex = (Int32)chm.Conditioning.ScaleIndex - 10;

                        //档位限制，眼图下禁止手动调节
                        mch.Conditioning.ScaleMaxIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                        mch.Conditioning.ScaleMinIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                    }
                }


                //mch.Conditioning.ScaleIndex = (Int32)chm.Conditioning.ScaleIndex - 10;

                ////档位限制，眼图下禁止手动调节
                //mch.Conditioning.ScaleMaxIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                //mch.Conditioning.ScaleMinIndex = (Int32)chm.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            mch.Conditioning.Unit = vec?.YUnit ?? "?";
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbPosition = (vec?.RefSampPos ?? Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(vec?.RefSampPos ?? Constants.DEF_XPOS_IDX)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }
            mch.Sampling.Scale = si * Constants.EYE_PATTERN_DEFAULT_WIDTH / Constants.VIS_XDIVS_NUM;
            prop.SampInterval = si;

            return prop;
        }


        public override Vector? Take()
        {
            return null;
        }
    }
}
