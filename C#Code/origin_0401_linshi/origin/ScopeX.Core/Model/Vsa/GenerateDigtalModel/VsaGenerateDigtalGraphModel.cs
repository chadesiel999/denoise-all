using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class VsaGenerateDigtalGraphModel : AdvancedMathModel
    {
        public VsaGenerateDigtalGraphModel(String formula) :base(formula)
        { 
            
        }
        public VsaGenerateDigtalGraphModel(String formula, DrawMethod drawMethod) : base(formula, drawMethod)
        { 
            
        }
        public override WfmProperties Config(MathModel mch, string exp, Vector? vec)
        {
            switch (Formula)
            {
                //case "VSAIEye()":
                   // return ConfigConstellation(mch, exp, vec);
                case "InphaseTime()":
                    return ConfigTime(mch, exp, vec);
                case "QuadratureTime()":
                    return ConfigTime(mch, exp, vec);
                case "Constellation()":
                    return ConfigConstellation(mch, exp, vec);
                case "VSAEvm()":
                    return ConfigTime(mch, exp, vec);
                case "ErrParamList":
                    return ConfigTime(mch, exp, vec);
                case "PhaseErrTime()":
                    return ConfigTime(mch, exp, vec);
                case "AmplErrTime()":
                    return ConfigTime(mch, exp, vec);
                case "VectorGph()":
                    return ConfigConstellation(mch, exp, vec);

                default:
                    return new WfmProperties("null");
            }
        }
        internal WfmProperties ConfigTime(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;//采样间隔，每两个点之间的时间
            var cscale = 12;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            var cscale1 = vec.MaxSubVector;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale =  Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            if (vec != null )
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
        internal WfmProperties ConfigConstellation(MathModel mch, string exp, Vector? vec)
        {
            var si = vec?.SampInterval ?? 1;//采样间隔，每两个点之间的时间
            var cscale = 12.0;
            //var cscale = (vec.MaxSubVector * 1000 + 2) / 4;
            //var cscale1 = vec.MaxSubVector;
            //var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = 10000;// Math.Round(si * 1E6 * length / Constants.VIS_XDIVS_NUM, 7);
            if (vec!=null&& vec.Elements.GetLength(0) == 2) 
            {
                var XYLength = vec.Elements.GetLength(1);
                double[] X  = new double[XYLength];
                double[] Y = new double[XYLength];
                for (int i = 0; i < vec?.Elements.GetLength(1); i++)
                {
                    X[i] = vec.Elements[0,i];
                    Y[i] = vec.Elements[1,i];
                }
                if (Y.Max() * 1000 < 1.5)
                {
                    cscale = 0.25;
                }
                else
                {
                    cscale = (int)(Y.Max() * 1000 / 3) ;
                }
                
                tscale = (int)(X.Max() *1000/30*100);
            }


            //垂直调节档位
            mch.Conditioning.InitialScale = (0, 0.002);
            mch.Conditioning.ScaleMaxIndex = 30;//正向调节  30次
            mch.Conditioning.ScaleMinIndex = -20;//负向调节  -20次


            if (mch.Args?.Occupier is not null)//当前通道被高级数学通道占用，basic +-*/ FFT custom，  VSA Jitter  电源分析
            {
                mch.Conditioning.Prefix = Prefix.Empty;
            }

            //水平调节档位
            mch.Sampling.InitialScale = (0, 15);
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
        internal WfmProperties ConfigVsaEye(MathModel mch, string exp, Vector? vec)
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

            mch.Sampling.InitialScale = (0, tscale * 100000);
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
    }
}
