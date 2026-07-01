using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class MathChartModel : AdvancedMathModel
    {
        public MathChartModel(string formula,ChannelId source, DrawMethod drawMethod) : base(formula, drawMethod)
        {
            _Source = source;
            InitChannelProperties(formula);
        }

        public MathChartModel(string formula, ChannelId source) : base(formula)
        {
            _Source = source;
            InitChannelProperties(formula);
        }

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get { return _Source; }
            set {
                if (value != _Source)
                {
                    _Source = value;
                }
            }
        
        }

        public override void InitChannelProperties(string formula)
        {
            switch (formula)
            {
                case "ChartHist()":
                    InitJitterHist();
                    break;
                case "JitterTrend()":
                    InitJitterTrend();
                    break;
                case "JitterSpectrum()":
                    InitJitterSpectrum();
                    break;
                case "JitterQWave()":
                    InitJitterQWave();
                    break;
                case "JitterBathWave()":
                    InitJitterBathWave();
                    break;
                case "JitterEye()":
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
        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            switch (Formula)
            {
                case "IChartHistogram()":
                    return ConfigHistogram(mch, exp, vec);
                case "IChartTimeDomain()":
                    return ConfigTimeDomain(mch, exp, vec);
                case "IChartSpectrum()":
                    return ConfigSpectrum(mch, exp, vec);
                case "IChart()":
                    return ConfigQFactor(mch, exp, vec);
                case "IChartBath()":
                    return ConfigBath(mch, exp, vec);
                case "IChartEyeDiagram()":
                    return ConfigEyeDiagram(mch, exp, vec);
                case "IChartConstellation()":
                    return ConfigConstellation(mch, exp, vec);
                case "IChartIDiagram()":
                    return ConfigIDiagram(mch, exp, vec);
                case "IChartQDiagram()":
                    return ConfigQDiagram(mch, exp, vec);
                case "IChartXY()":
                    return ConfigXY(mch, exp, vec);
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

        internal WfmProperties ConfigHistogram(MathModel mch, string exp, Vector? res)
        {

            Int32 minbins = 20;
            mch.Sampling.Prefix = Prefix.Micro;

            if (mch.Args is MathCustomArg histArg && res != null)
            {
                var id = mch.Id.ToString();
                if (id != null && MathVecBuffer.Default.TryGetVector(id, out var val))
                {
                    var tempbuffer = val.Elements.Cast<Double>().Where(x => !Double.IsNaN(x)).ToList();
                    if (tempbuffer.Count > 0)
                    {

                        //histArg.HistParamter.MaxValue = tempbuffer.Max()/* * val.SampInterval*/;
                        //histArg.HistParamter.MinValue = tempbuffer.Min()/* * val.SampInterval*/;
                        //histArg.HistParamter.MaxValue = DsoModel.Default.JitterModel.HistRangeMax/* * val.SampInterval*/;
                        //histArg.HistParamter.MinValue = DsoModel.Default.JitterModel.HistRangeMin/* * val.SampInterval*/;
                        histArg.HistParamter.MaxValue = val.SampInterval;
                        histArg.HistParamter.MinValue = val.RefSampPos;
                    }
                    else
                    {
                        histArg.HistParamter.MaxValue = 0;
                        histArg.HistParamter.MinValue = 0;
                    }
                    histArg.HistParamter.BinZoomRatio = 1;
                    Int32 index = GetLastNotZero(res.Elements);
                    histArg.HistParamter.FixedBins = index + 1;
                    histArg.HistParamter.NBins = tempbuffer.Count;
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
                            width = (histArg.HistParamter.MaxValue - histArg.HistParamter.MinValue) / (Double)res.Elements.Length * (index + 1);
                        }
                        var scaleinfo = mch.GetHistSampleScale(width);
                        mch.Sampling.InitialScale = (0, scaleinfo.ScaleValue);
                        mch.Sampling.ScaleMaxIndex = scaleinfo.MaxScaleIndex;
                        mch.Sampling.ScaleMinIndex = scaleinfo.MinScaleIndex;
                        mch.Sampling.Scale = scaleinfo.ScaleValue;
                        if (index > minbins)
                        {
                            mch.Sampling.PosIndex = Constants.MIN_XPOS_TIME - (width / (scaleinfo.ScaleValue / 1E6 * Constants.VIS_XDIVS_NUM) * (Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV) / 2);
                        }
                        else
                        {
                            mch.Sampling.PosIndex = Constants.MIN_XPOS_TIME - (((histArg.HistParamter.MaxValue - histArg.HistParamter.MinValue) / (Double)res.Elements.Length * (index + 1)) / (scaleinfo.ScaleValue / 1E6 * Constants.VIS_XDIVS_NUM) * (Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV) / 2);
                        }
                    }

                }

            }
            mch.Conditioning.PosIndex = Constants.IDX_PER_YDIV * Constants.VIS_YDIVS_NUM * -0.5;
            if (mch.AutoScale)
            //if (mch.AutoScale)
            {
                mch.Conditioning.InitialScale = (12, 1_000_000);
                mch.Conditioning.ScaleMinIndex = 9;
                mch.Conditioning.ScaleMaxIndex = 20;
                mch.Conditioning.Prefix = Prefix.Milli;

                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, res);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.InitFlag = false;

                if (mch.Conditioning.IsAutoUnit)
                {
                    mch.Conditioning.Unit = res?.YUnit ?? "?";
                }
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
            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (12, 1_000_000);

                mch.Conditioning.ScaleMaxIndex = 30;
                mch.Conditioning.ScaleMinIndex = 3;
            }

            prop.SampInterval = si;

            return prop;
            
        }
        internal WfmProperties ConfigTimeDomain(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 80;
            mch.Conditioning.ScaleMinIndex = -80;
            if (mch.Args?.Occupier is not null)
            {
                //mch.Conditioning.Prefix = Prefix.Pico;
                mch.Conditioning.Prefix = Prefix.Milli;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 2;
            mch.Sampling.ScaleMinIndex = -5;
            mch.Sampling.Prefix = Prefix.Micro;

            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;
            if (mch.InitFlag)
            {
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = -35;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigSpectrum(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigQFactor(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (1, cscale);
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
                (int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                //mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                mch.Conditioning.PosIndex = 4000;

                //mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                //mch.Sampling.PosIndex = 0;// mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                //ScaleFit(mch, vec);
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigBath(MathModel mch, string exp, Vector? vec)
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
                (int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigEyeDiagram(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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
            if (exp == "Execute.JitterEye()")
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);

                mch.Conditioning.ScaleIndex = (Int32)ach.Conditioning.ScaleIndex - 10;

                //档位限制，眼图下禁止手动调节
                mch.Conditioning.ScaleMaxIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Conditioning.ScaleMinIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigConstellation(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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
            if (exp == "Execute.JitterEye()")
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);

                mch.Conditioning.ScaleIndex = (Int32)ach.Conditioning.ScaleIndex - 10;

                //档位限制，眼图下禁止手动调节
                mch.Conditioning.ScaleMaxIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Conditioning.ScaleMinIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigIDiagram(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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
            if (exp == "Execute.JitterEye()")
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);

                mch.Conditioning.ScaleIndex = (Int32)ach.Conditioning.ScaleIndex - 10;

                //档位限制，眼图下禁止手动调节
                mch.Conditioning.ScaleMaxIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Conditioning.ScaleMinIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigQDiagram(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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
            if (exp == "Execute.JitterEye()")
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);

                mch.Conditioning.ScaleIndex = (Int32)ach.Conditioning.ScaleIndex - 10;

                //档位限制，眼图下禁止手动调节
                mch.Conditioning.ScaleMaxIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Conditioning.ScaleMinIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        internal WfmProperties ConfigXY(MathModel mch, string exp, Vector? vec)
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
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
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
            if (exp == "Execute.JitterEye()")
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(DsoModel.Default.JitterModel.Source);

                mch.Conditioning.ScaleIndex = (Int32)ach.Conditioning.ScaleIndex - 10;

                //档位限制，眼图下禁止手动调节
                mch.Conditioning.ScaleMaxIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Conditioning.ScaleMinIndex = (Int32)ach.Conditioning.ScaleIndex - 10;
                mch.Sampling.ScaleMaxIndex = 0;
                mch.Sampling.ScaleMinIndex = 0;
            }
            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
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
        public override Vector? Take()
        {
            return null;
        }
    }
}
