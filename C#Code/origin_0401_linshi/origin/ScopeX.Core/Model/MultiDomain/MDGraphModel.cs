using NPOI.SS.Formula.Functions;
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
    internal class MDGraphModel : AdvancedMathModel
    {
        public MDGraphModel(String formula) : base(formula)
        {

        }

        public MDGraphModel(String formula, DrawMethod drawMethod) : base(formula, drawMethod)
        {
            
        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;//采样间隔，每两个点之间的时间
            var cscale = 12;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            var cscale1 = vec.MaxSubVector;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 16384 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            if (vec != null)
            {
                var XYLength = vec.Elements.GetLength(1);
                double[] X = new double[XYLength];
                double[] Y = new double[XYLength];
                for (int i = 0; i < vec?.Elements.GetLength(1); i++)
                {
                    X[i] = vec.Elements[0, i];

                }
                cscale = (int)(X.Max() * 1000 / 3);

            }


            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 9);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            //水平调节档位
            mch.Sampling.InitialScale = (0, si * length);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Micro;

            mch.Sampling.PosMinIndex = 0;
            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;//虚拟坐标
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
            // mch.Sampling.Unit = " ";
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
    }

    internal class PVFGraphModel : MDGraphModel
    { 
        public PVFGraphModel(String formula) : base(formula)
        {
            
        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 3);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            double hscale = DsoModel.Default.MultiDomain.SpanFreqByHz / 10;

            //水平调节档位
            mch.Sampling.InitialScale = (0, hscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Empty;
            mch.Sampling.PosIdxPerDiv = 1000;
            //mch.Sampling.PosIdxPerDiv = ((hscale / 1e3) * 10) / ((vec?.Elements.GetLength(1) ?? 1024) - 1);
            mch.Sampling.PosDefIndex = 0;// DsoModel.Default.MultiDomain.CenterFreqByHz;

            //double test = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            mch.Sampling.PosIndex = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            //mch.Sampling.PosStpIndex = -((Double)DsoModel.Default.MultiDomain.EndFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            if (mch.InitFlag)
            {
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
            mch.Sampling.Unit = vec?.XUnit ?? "?";
            // mch.Sampling.Unit = " ";
            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, 0),
                //TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = (((vec?.SampInterval ?? 1) / 1e6) / ((vec?.Elements.GetLength(1) ?? 1024) - 1));
            prop.VuStartIndex = ((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            return prop;
        }
    }

    internal class AVFGraphModel : MDGraphModel
    {
        public AVFGraphModel(String formula) : base(formula)
        {

        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 3);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            double hscale = DsoModel.Default.MultiDomain.SpanFreqByHz / 10;

            //水平调节档位
            mch.Sampling.InitialScale = (0, hscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Empty;
            mch.Sampling.PosIdxPerDiv = 1000;
            //mch.Sampling.PosIdxPerDiv = ((hscale / 1e3) * 10) / ((vec?.Elements.GetLength(1) ?? 1024) - 1);
            mch.Sampling.PosDefIndex = 0;// DsoModel.Default.MultiDomain.CenterFreqByHz;

            //double test = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            mch.Sampling.PosMinIndex = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            mch.Sampling.PosIndex = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            //mch.Sampling.PosStpIndex = -((Double)DsoModel.Default.MultiDomain.EndFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            if (mch.InitFlag)
            {
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
            mch.Sampling.Unit = vec?.XUnit ?? "?";
            // mch.Sampling.Unit = " ";
            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, 0),
                //TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = (((vec?.SampInterval ?? 1) / 1e6) / ((vec?.Elements.GetLength(1) ?? 1024) - 1));
            prop.VuStartIndex = ((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            return prop;
        }
    }

    internal class WaterFallsGraphModel : MDGraphModel
    {
        public WaterFallsGraphModel(String formula) : base(formula, DrawMethod.DPX)
        {
            
        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            var cscale = 12;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            //var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            //if (vec != null)
            //{
            //    var XYLength = vec.Elements.GetLength(1);
            //    double[] X = new double[XYLength];
            //    double[] Y = new double[XYLength];
            //    for (int i = 0; i < vec?.Elements.GetLength(1); i++)
            //    {
            //        X[i] = vec.Elements[0, i];

            //    }
            //    cscale = -(int)(X.Max() * 1000 / 3);
            //}

            //mch.AutoScale = false;
            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 3);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            double hscale = DsoModel.Default.MultiDomain.SpanFreqByHz / 10;

            //水平调节档位
            mch.Sampling.InitialScale = (0, hscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Empty;
            mch.Sampling.PosIdxPerDiv = 1000;
            //mch.Sampling.PosIdxPerDiv = ((hscale / 1e3) * 10) / ((vec?.Elements.GetLength(1) ?? 1024) - 1);
            mch.Sampling.PosDefIndex = 0;// DsoModel.Default.MultiDomain.CenterFreqByHz;

            //double test = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            mch.Sampling.PosIndex = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            //mch.Sampling.PosStpIndex = -((Double)DsoModel.Default.MultiDomain.EndFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            if (mch.InitFlag)
            {
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
            mch.Sampling.Unit = vec?.XUnit ?? "?";
            // mch.Sampling.Unit = " ";
            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, 0),
                //TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = (((vec?.SampInterval ?? 1) / 1e6) / ((vec?.Elements.GetLength(1) ?? 1024) - 1));
            prop.VuStartIndex = ((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            return prop;
        }
    }

    internal class SpectrogramGraphModel : MDGraphModel
    {
        public SpectrogramGraphModel(String formula) : base(formula, DrawMethod.DPX)
        {

        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            var cscale = 12;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            //var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            //if (vec != null)
            //{
            //    var XYLength = vec.Elements.GetLength(1);
            //    double[] X = new double[XYLength];
            //    double[] Y = new double[XYLength];
            //    for (int i = 0; i < vec?.Elements.GetLength(1); i++)
            //    {
            //        X[i] = vec.Elements[0, i];

            //    }
            //    cscale = -(int)(X.Max() * 1000 / 3);
            //}

            //mch.AutoScale = false;
            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 3);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            double hscale = DsoModel.Default.MultiDomain.SpanValueForTimeFreq / 10;

            //水平调节档位
            mch.Sampling.InitialScale = (0, hscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Empty;
            mch.Sampling.PosIdxPerDiv = 1000;
            //mch.Sampling.PosIdxPerDiv = ((hscale / 1e3) * 10) / ((vec?.Elements.GetLength(1) ?? 1024) - 1);
            mch.Sampling.PosDefIndex = 0;

            //double test = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;
            mch.Sampling.PosIndex = -((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanValueForTimeFreq) * 1e4;
            //mch.Sampling.PosStpIndex = -((Double)DsoModel.Default.MultiDomain.EndFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanFreqByHz) * 1e4;

            if (mch.InitFlag)
            {
                //(int VScaleIndex, int HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                //mch.Conditioning.ScaleIndex = scale.VScaleIndex;
                //mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
            }

            if (mch.Conditioning.IsAutoUnit)
            {
                mch.Conditioning.Unit = vec?.YUnit ?? "?";
            }
            mch.Sampling.Unit = vec?.XUnit ?? "?";
            // mch.Sampling.Unit = " ";
            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, 0),
                //TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = (((vec?.SampInterval ?? 1) / 1e6) / ((vec?.Elements.GetLength(1) ?? 1024) - 1));
            prop.VuStartIndex = ((Double)DsoModel.Default.MultiDomain.StartFreqByHz / (Double)DsoModel.Default.MultiDomain.SpanValueForTimeFreq) * 1e4;

            return prop;
        }
    }

    /// <summary>
    /// 需要完善
    /// </summary>
    internal class ThreeDGraphModel : MDGraphModel
    {
        public ThreeDGraphModel(String formula) : base(formula, DrawMethod.Stair)
        {

        }

        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;//采样间隔，每两个点之间的时间
            var cscale = 12;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            var cscale1 = vec.MaxSubVector;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            if (vec != null)
            {
                var XYLength = vec.Elements.GetLength(1);
                double[] X = new double[XYLength];
                double[] Y = new double[XYLength];
                for (int i = 0; i < vec?.Elements.GetLength(1); i++)
                {
                    X[i] = vec.Elements[0, i];

                }
                cscale = (int)(X.Max() * 1000 / 3);

            }


            //垂直调节档位
            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            //水平调节档位
            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 20;
            mch.Sampling.ScaleMinIndex = -20;
            mch.Sampling.Prefix = Prefix.Empty;

            mch.Sampling.PosIndex = vec?.RefSampPos ?? 5000;//虚拟坐标
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
            // mch.Sampling.Unit = " ";
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
    }
}
