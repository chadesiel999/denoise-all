using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using static ScopeX.Core.FilterPrsnt;

namespace ScopeX.Core
{
    public class FilterPrsnt :MulticastPrsnt<IFilterView>, IFilterPrsnt
    {
        private protected override FilterModel Model
        {
            get;
        }

        public FilterPrsnt(IDsoPrsnt idp, IFilterView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {

            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.Filter,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            DesignerConfigs = DesignerConfig.InitConfigs(FreqFactor);
            Model.PropertyChanged += OnPropertyChanged;

            Model.Design();

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        private List<DesignerConfig> DesignerConfigs;

        public DesignerConfig _CurrentDesignerConfig;

        public DesignerConfig CurrentDesignerConfig
        {
            get
            {
                var config= DesignerConfigs.First(x => x.DesignerType == (Int32)FilterType &&
                ((x.DesignerType == (Int32)FilterType.FIRFilter && x.MethodType == (Int32)FIRMethod) || (x.DesignerType == (Int32)FilterType.IIRFilter && x.MethodType == (Int32)IIRMethod)));
               if(config!=_CurrentDesignerConfig)
                {
                    Model.LowPassFreq = config[(Int32)RespType, (Int32)OrderMode].Fp1;
                    Model.LowStopFreq = config[(Int32)RespType, (Int32)OrderMode].Fs1;
                    Model.HighPassFreq = config[(Int32)RespType, (Int32)OrderMode].Fp2;
                    Model.HighStopFreq = config[(Int32)RespType, (Int32)OrderMode].Fs2;
                    Model.PassMag = config[(Int32)RespType, (Int32)OrderMode].PassMag;
                    Model.StopMag = config[(Int32)RespType, (Int32)OrderMode].StopMag;
                    Model.Order = config.Order;
                    _CurrentDesignerConfig = config;
                }
                return _CurrentDesignerConfig;
            }
        }

        public Double FreqFactor => Model.FreqFactor;

        public Double MagFactor => Model.MagFactor;

        public Int32 MaxMagnitude => Model.MaxMagnitude;

        public Int32 MinMagnitude => Model.MinMagnitude;

        public FilterResponseType RespType
        {
            get => Model.RespType;
            set
            {
                if(Model.RespType != value)
                {
                    Model.LowPassFreq = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].Fp1;
                    Model.LowStopFreq = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].Fs1;
                    Model.HighPassFreq = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].Fp2;
                    Model.HighStopFreq = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].Fs2;
                    Model.PassMag = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].PassMag;
                    Model.StopMag = CurrentDesignerConfig[(Int32)value, (Int32)OrderMode].StopMag;
                    Model.RespType = value;
                }
            }
        }

        public FilterType FilterType
        {
            get => Model.FilterType;
            set => Model.FilterType = value;
        }

        public FIRType FIRMethod
        {
            get => Model.FIRMethod;
            set => Model.FIRMethod = value;
        }

        public IIRType IIRMethod
        {
            get => Model.IIRMethod;
            set => Model.IIRMethod = value;
        }

        public FilterOrderMode OrderMode
        {
            get => Model.OrderMode;
            set
            {
                Model.LowPassFreq = CurrentDesignerConfig[(Int32)RespType, (Int32)value].Fp1;
                Model.LowStopFreq = CurrentDesignerConfig[(Int32)RespType, (Int32)value].Fs1;
                Model.HighPassFreq = CurrentDesignerConfig[(Int32)RespType, (Int32)value].Fp2;
                Model.HighStopFreq = CurrentDesignerConfig[(Int32)RespType, (Int32)value].Fs2;
                Model.PassMag = CurrentDesignerConfig[(Int32)RespType, (Int32)value].PassMag;
                Model.StopMag = CurrentDesignerConfig[(Int32)RespType, (Int32)value].StopMag;
                Model.OrderMode = value;
            }
        }

        public Int32 Order
        {
            get
            {
                if(CurrentDesignerConfig.Order!=Model.Order)
                {
                    CurrentDesignerConfig.Order = Model.Order;
                }
               return CurrentDesignerConfig.Order;
            }
            set
            {
                CurrentDesignerConfig.Order = value;
                Model.Order = value;
            }
        }

        public static readonly Int32 FIRMaxOrder = FilterModel.FIRMaxOrder;
        public static readonly Int32 IIRMaxOrder = FilterModel.IIRMaxOrder;

        public static readonly Int32 MinOrder = FilterModel.MinOrder;

        public Double SamplingFreq
        {
            get => Model.SamplingFreq;
            set => Model.SamplingFreq = value / MagFactor;
        }

        public Int32 LowPassFreq
        {
            get => Model.LowPassFreq;
            set
            {
                if(Model.LowPassFreq!=value)
                {
                    var limit = GetFreqLimitValue(CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp1Limit);
                    Int32 max = (Int32)(limit.max * FreqFactor);
                    Int32 min = (Int32)(limit.min * FreqFactor);
                    if (value> max)
                    {
                        value = max;
                        WeakTip.Default.Write(nameof(LowPassFreq), MsgTipId.GreatethanMax, false, "", 1);
                    }
                    if (value < min)
                    {
                        value = min;
                        WeakTip.Default.Write(nameof(LowPassFreq), MsgTipId.LessthanMin, false, "", 1);
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp1 = value;
                    Model.LowPassFreq = value;
                }
            }
        }
        public Int32 LowStopFreq
        {
            get => Model.LowStopFreq;
            set
            {
                if (Model.LowStopFreq != value)
                {
                    var limit = GetFreqLimitValue(CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs1Limit);
                    Int32 max = (Int32)(limit.max * FreqFactor);
                    Int32 min = (Int32)(limit.min * FreqFactor);
                    if (value > max)
                    {
                        value = max;
                        WeakTip.Default.Write(nameof(LowStopFreq), MsgTipId.GreatethanMax, false, "", 1);
                    }
                    if (value < min)
                    {
                        value = min;
                        WeakTip.Default.Write(nameof(LowStopFreq), MsgTipId.LessthanMin, false, "", 1);
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs1 = value;
                    Model.LowStopFreq = value; 
                }
            }
        }

        public Int32 HighPassFreq
        {
            get => Model.HighPassFreq;
            set
            {
                if (Model.HighPassFreq != value)
                {
                    var limit = GetFreqLimitValue(CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp2Limit);
                    Int32 max = (Int32)(limit.max * FreqFactor);
                    Int32 min = (Int32)(limit.min * FreqFactor);
                    if (value > max)
                    {
                        value = max;
                        WeakTip.Default.Write(nameof(HighPassFreq), MsgTipId.GreatethanMax, false, "", 1);
                    }
                    if (value < min)
                    {
                        value = min;
                        WeakTip.Default.Write(nameof(HighPassFreq), MsgTipId.LessthanMin, false, "", 1);
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp2 = value;
                    Model.HighPassFreq = value; 
                }
            }
        }

        public Int32 HighStopFreq
        {
            get => Model.HighStopFreq;
            set
            {
                if (Model.HighStopFreq != value)
                {
                    var limit = GetFreqLimitValue(CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs2Limit);
                    Int32 max = (Int32)(limit.max * FreqFactor);
                    Int32 min = (Int32)(limit.min * FreqFactor);
                    if (value > max)
                    {
                        value = max;
                        WeakTip.Default.Write(nameof(HighStopFreq), MsgTipId.GreatethanMax, false, "", 1);
                    }
                    if (value < min)
                    {
                        value = min;
                        WeakTip.Default.Write(nameof(HighStopFreq), MsgTipId.LessthanMin, false, "", 1);
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs2 = value;
                    Model.HighStopFreq = value; 
                }
            }
        }

        /// <summary>
        /// Passband Ripple Magnitude, in 0.01dB
        /// </summary>
        public Int32 PassMag
        {
            get => Model.PassMag;
            set
            {
                if (Model.PassMag != value)
                {
                    var limit = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode];
                    if (limit.IsShowStopMag)
                    {
                        if (value > StopMag)
                        {
                            value = StopMag;
                            WeakTip.Default.Write(nameof(PassMag), MsgTipId.GreatethanMax, false, "", 1);
                        } 
                    }
                    else
                    {
                        if (value >= MaxMagnitude)
                        {
                            value = MaxMagnitude-1;
                            WeakTip.Default.Write(nameof(PassMag), MsgTipId.GreatethanMax, false, "", 1);
                        }
                    }
                    if (value < MinMagnitude)
                    {
                        value = MinMagnitude;
                        WeakTip.Default.Write(nameof(PassMag), MsgTipId.LessthanMin, false, "", 1);
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].PassMag = value;
                    Model.PassMag = value; 
                }
            }
        }

        /// <summary>
        /// Stopband Attenuation Magnitude, in 0.01dB
        /// </summary>

        public Int32 StopMag
        {
            get => Model.StopMag;
            set
            {
                var limit = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode];
                if (Model.PassMag != value)
                {
                    if (value > MaxMagnitude)
                    {
                        value = MaxMagnitude;
                        WeakTip.Default.Write(nameof(StopMag), MsgTipId.GreatethanMax, false, "", 1);
                    }
                    if (limit.IsShowPassMag)
                    {
                        if (value < PassMag)
                        {
                            value = PassMag+1;
                            WeakTip.Default.Write(nameof(StopMag), MsgTipId.LessthanMin, false, "", 1);
                        } 
                    }
                    else
                    {
                        if (value <= MinMagnitude)
                        {
                            value = MinMagnitude;
                            WeakTip.Default.Write(nameof(StopMag), MsgTipId.LessthanMin, false, "", 1);
                        }
                    }
                    CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].StopMag = value;
                    Model.StopMag = value; 
                }
            }
        }

        public void AdjPassMag(Int32 delta) => Model.AdjPassMag(delta);

        public void AdjStopMag(Int32 delta) => Model.AdjStopMag(delta);

        public WindowType Window
        {
            get => Model.Window;
            set => Model.Window = value;
        }

        public UInt32 DensityFactor
        {
            get => Model.DensityFactor;
            set => Model.DensityFactor = value;
        }

        public Double[]? Numerator => Model.Numerator;

        public Double[]? Denominator => Model.Denominator;

        public Double[]? Zeros => Model.Zeros;

        public Double[]? Poles => Model.Poles;

        public void Design() => Model.Design();

        public IReadOnlyList<Double>? DesignFor(ChannelId src)
        {
            Design();

            if (Numerator is not null)
                MathVecBuffer.Default.Provide(MathFilterArg.GetCoeffKey(src.ToString()), new Vector(Numerator));

            return Numerator;
        }

        private String MakeFileName()
        {
            var name = FilterType.ToString().Substring(0, 3);
            if (FilterType == FilterType.FIRFilter)
            {
                name += FIRMethod.ToString();
            }
            else
            {
                name += IIRMethod.ToString();
            }

            return name + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public Boolean Save(String path, String name = "")
        {
            if (Model.Numerator is not null)
            {
                if (String.IsNullOrWhiteSpace(name))
                    name = MakeFileName();
                //!!!Notice: Add some parameters
                //return FilePrsnt.SaveToText(path, name, Model.Coefficient);

                return FilePrsnt.SaveToText(path, name, (sw) =>
                {
                    sw.WriteLine(FilterType);
                    sw.WriteLine(RespType);
                    sw.WriteLine(FilterType == FilterType.FIRFilter ? FIRMethod : IIRMethod);
                   if(FilterType == FilterType.FIRFilter&& FIRMethod== FIRType.Window)
                    {
                        sw.WriteLine(Window);
                    }
                    sw.WriteLine(OrderMode);
                   if(OrderMode== FilterOrderMode.UserDefined)
                    {
                        sw.WriteLine(Order);
                    }
                   if(CurrentDesignerConfig!=null&&CurrentDesignerConfig.Limit!=null&& CurrentDesignerConfig.Limit.Count>0)
                    {
                        var infos = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode];
                        if(infos!=null)
                        {
                            if(infos.IsShowFp1)
                            {
                                sw.WriteLine(LowPassFreq / FreqFactor);
                            }

                            if (infos.IsShowFs1)
                            {
                                sw.WriteLine(LowStopFreq / FreqFactor);
                            }

                            if (infos.IsShowFp2)
                            {
                                sw.WriteLine(HighPassFreq / FreqFactor);
                            }

                            if (infos.IsShowFs2)
                            {
                                sw.WriteLine(HighStopFreq / FreqFactor);
                            }

                            if (infos.IsShowPassMag)
                            {
                                sw.WriteLine(PassMag / MagFactor);
                            }

                            if (infos.IsShowStopMag)
                            {
                                sw.WriteLine(StopMag / MagFactor);
                            }
                        }
                        
                    }
                    //sw.WriteLine(LowPassFreq/ FreqFactor);
                    //sw.WriteLine(LowStopFreq/ FreqFactor);
                    //sw.WriteLine(HighPassFreq/ FreqFactor);
                    //sw.WriteLine(HighStopFreq/ FreqFactor);
                    //sw.WriteLine(PassMag/ MagFactor);
                    //sw.WriteLine(StopMag/ MagFactor);
                    //sw.WriteLine(Window);
                    //sw.WriteLine(DensityFactor);
                    sw.WriteLine();
                    sw.WriteLine(nameof(Model.Numerator));
                    foreach (var d in Model.Numerator)
                    {
                        sw.WriteLine(d);
                    }

                    if (Model.Denominator is not null)
                    {
                        sw.WriteLine();
                        sw.WriteLine(nameof(Model.Denominator));
                        foreach (var d in Model.Denominator)
                        {
                            sw.WriteLine(d);
                        }
                    }
                    return true;
                });

            }
            return false;
        }

        public Boolean Load(String fullName)
        {
            return FilePrsnt.LoadFromText(fullName, (sr) =>
            {
                if (Enum.TryParse<FilterType>(sr.ReadLine(), out var ft))
                {
                    FilterType = ft;
                }
                if (Enum.TryParse<FilterResponseType>(sr.ReadLine(), out var rt))
                {
                    RespType = rt;
                }
                if(FilterType== FilterType.FIRFilter)
                {
                    if (Enum.TryParse<FIRType>(sr.ReadLine(), out var fm))
                    {
                        FIRMethod = fm;
                    }
                    if(FIRMethod== FIRType.Window)
                    {
                        if (Enum.TryParse<WindowType>(sr.ReadLine(), out var win))
                        {
                            Window = win;
                        }
                    }
                }
                else
                {
                    if (Enum.TryParse<IIRType>(sr.ReadLine(), out var im))
                    {
                        IIRMethod = im;
                    }
                }
               
               
                if (Enum.TryParse<FilterOrderMode>(sr.ReadLine(), out var om))
                {
                    OrderMode = om;
                }
               if(OrderMode== FilterOrderMode.UserDefined)
                {
                    if (Int32.TryParse(sr.ReadLine(), out var order))
                    {
                        Order = order;
                    }
                }

                if (CurrentDesignerConfig != null && CurrentDesignerConfig.Limit != null && CurrentDesignerConfig.Limit.Count > 0)
                {
                    var infos = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode];
                    if(infos!=null)
                    {
                        if (infos.IsShowFp1)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var lpf))
                            {
                                LowPassFreq =(Int32)(lpf * FreqFactor);
                            }

                        }

                        if (infos.IsShowFs1)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var lsf))
                            {
                                LowStopFreq = (Int32)(lsf * FreqFactor);
                            }
                        }

                        if (infos.IsShowFp2)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var hpf))
                            {
                                HighPassFreq = (Int32)(hpf * FreqFactor);
                            }
                        }

                        if (infos.IsShowFs2)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var hsf))
                            {
                                HighStopFreq = (Int32)(hsf * FreqFactor);
                            }
                        }

                        if (infos.IsShowPassMag)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var pm))
                            {
                                PassMag = (Int32)(pm * MagFactor);
                            }
                        }

                        if (infos.IsShowStopMag)
                        {
                            if (Double.TryParse(sr.ReadLine(), out var sm))
                            {
                                StopMag = (Int32)(sm * MagFactor);
                            }
                        }
                    }
                }
                Model.Design();
                //if (Int32.TryParse(sr.ReadLine(), out var lpf))
                //{
                //    LowPassFreq = lpf;
                //}
                //if (Int32.TryParse(sr.ReadLine(), out var lsf))
                //{
                //    LowStopFreq = lsf;
                //}
                //if (Int32.TryParse(sr.ReadLine(), out var hpf))
                //{
                //    HighPassFreq = hpf;
                //}
                //if (Int32.TryParse(sr.ReadLine(), out var hsf))
                //{
                //    HighStopFreq = hsf;
                //}
                //if (Int32.TryParse(sr.ReadLine(), out var pm))
                //{
                //    PassMag = pm;
                //}
                //if (Int32.TryParse(sr.ReadLine(), out var sm))
                //{
                //    StopMag = sm;
                //}
                //if (Double.TryParse(sr.ReadLine(), out var ff))
                //{
                //    FreqFactor = ff;
                //}
                //if (Double.TryParse(sr.ReadLine(), out var ff))
                //{
                //    MagFactor = ff;
                //}
                //if (Enum.TryParse<WindowType>(sr.ReadLine(), out var w))
                //{
                //    Window = w;
                //}
                //if (UInt32.TryParse(sr.ReadLine(), out var df))
                //{
                //    DensityFactor = df;
                //}
                //sr.ReadLine();
                //sr.ReadLine();
                //String? line;
                //while (!String.IsNullOrEmpty((line = sr.ReadLine())))
                //{
                //    numerator.Add(Double.Parse(line));
                //}
                //sr.ReadLine();
                //denominator = new();
                //while ((line = sr.ReadLine()) != null)
                //{
                //    denominator.Add(Double.Parse(line));
                //}
                return true;
            });
        }

        public System.Numerics.Complex[] Roots(Double[] num,Int32 n)
        {
            if (num == null || num.Length != n+1)
                return new System.Numerics.Complex[n];
            else
                return MathToolAPI.ConvertIntPtrToComplexDoubleArray(MathToolAPI.Roots(num,n),n);
        }

        public (Double max, Double min) GetFreqLimitValue(ValueLimit limit)
        {
            Double max = 0.999*Model.FreqFactor, min = 0.001 * Model.FreqFactor;
            switch (limit.MaxLimit)
            {
                case LimitObject.Fp1:
                    max = CurrentDesignerConfig[(Int32)RespType,(Int32)OrderMode].Fp1 - 1;
                    break;
                case LimitObject.Fs1:
                    max = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs1 - 1;
                    break;
                case LimitObject.Fp2:
                    max = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp2 - 1;
                    break;
                case LimitObject.Fs2:
                    max = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs2 - 1;
                    break;
                case LimitObject.None:
                default:
                    break;
            }

            switch (limit.MinLimit)
            {
                case LimitObject.Fp1:
                    min = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp1 + 1;
                    break;
                case LimitObject.Fs1:
                    min = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs1 + 1;
                    break;
                case LimitObject.Fp2:
                    min = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fp2 + 1;
                    break;
                case LimitObject.Fs2:
                    min = CurrentDesignerConfig[(Int32)RespType, (Int32)OrderMode].Fs2 + 1;
                    break;
                case LimitObject.None:
                default:
                    break;
            }

            return (max/Model.FreqFactor, min / Model.FreqFactor);
        }


        public class DesignerConfig
        {
            public Int32 DesignerType;//IIR ,FIR
            public Int32 MethodType;//IIRMethod,FIRMethod
            public Boolean IsShowWindowMethod;
            public Boolean IsShowMinOrder;
            public Int32 Order=2;//初始默认为2

            public List<FilterLimit> Limit;

            public static List<DesignerConfig> InitConfigs(Double FreqFactor)
            {
                List<DesignerConfig> configs = new List<DesignerConfig>();

                #region FIR配置
                //频率采样法
                var fs = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.FIRFilter,
                    MethodType = (Int32)FIRType.FreqSampling,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = false,
                    Limit = new List<FilterLimit>()
                };
                fs.Limit.AddRange(FilterLimit.InitLimits(fs.DesignerType, fs.MethodType, FreqFactor));
                configs.Add(fs);
                var window = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.FIRFilter,
                    MethodType = (Int32)FIRType.Window,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = false,
                    Limit = new List<FilterLimit>()
                };
                window.Limit.AddRange(FilterLimit.InitLimits(window.DesignerType, window.MethodType, FreqFactor));
                configs.Add(window);
                var remez = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.FIRFilter,
                    MethodType = (Int32)FIRType.Remez,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = true,
                    Limit = new List<FilterLimit>()
                };
                remez.Limit.AddRange(FilterLimit.InitLimits(remez.DesignerType, remez.MethodType, FreqFactor));
                configs.Add(remez);

                #endregion

                #region IIR配置
                //巴特沃斯
                var butter = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.IIRFilter,
                    MethodType = (Int32)IIRType.Butterworth,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = true,
                    Limit = new List<FilterLimit>()
                };
                butter.Limit.AddRange(FilterLimit.InitLimits(butter.DesignerType, butter.MethodType, FreqFactor));
                configs.Add(butter);

                //切比雪夫Ⅰ型
                var cheby1 = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.IIRFilter,
                    MethodType = (Int32)IIRType.ChebyshevI,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = true,
                    Limit = new List<FilterLimit>()
                };
                cheby1.Limit.AddRange(FilterLimit.InitLimits(cheby1.DesignerType, cheby1.MethodType, FreqFactor));
                configs.Add(cheby1);
                //切比雪夫Ⅱ型
                var cheby2 = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.IIRFilter,
                    MethodType = (Int32)IIRType.ChebyshevII,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = true,
                    Limit = new List<FilterLimit>()
                };
                cheby2.Limit.AddRange(FilterLimit.InitLimits(cheby2.DesignerType, cheby2.MethodType, FreqFactor));
                configs.Add(cheby2);

                //椭圆
                var elliptic = new DesignerConfig()
                {
                    DesignerType = (Int32)FilterType.IIRFilter,
                    MethodType = (Int32)IIRType.Elliptic,
                    IsShowWindowMethod = false,
                    IsShowMinOrder = true,
                    Limit = new List<FilterLimit>()
                };
                elliptic.Limit.AddRange(FilterLimit.InitLimits(elliptic.DesignerType, elliptic.MethodType, FreqFactor));
                configs.Add(elliptic);
                #endregion

                return configs;
            }

            public FilterLimit this[Int32 resptype, Int32 ordermode]
            {
                get
                {
                    var limit= Limit.FirstOrDefault(x => x.RespType == resptype && x.OrderMode == ordermode);
                    if(limit == null)
                    {
                        limit = Limit.First(x => x.RespType == resptype && x.OrderMode != ordermode);
                    }
                    return limit;
                }
            }
        }

        public class FilterLimit
        {
            public FilterLimit()
            {
                Fs1Limit = new ValueLimit();
                Fp1Limit = new ValueLimit();
                Fs2Limit = new ValueLimit();
                Fp2Limit = new ValueLimit();
            }
            public Int32 RespType;//LP,HP,BP,BS
            public Int32 OrderMode;//Minimum,UserDefined
            public Boolean IsShowFs1 = false;
            public Int32 Fs1;
            public ValueLimit Fs1Limit;
            public Boolean IsShowFp1 = false;
            public Int32 Fp1;
            public ValueLimit Fp1Limit;
            public Boolean IsShowFs2 = false;
            public Int32 Fs2;
            public ValueLimit Fs2Limit;
            public Boolean IsShowFp2 = false;
            public Int32 Fp2;
            public ValueLimit Fp2Limit;
            public Boolean IsShowMag = false;//是否显示幅度限制
            public Boolean IsShowPassMag = false;
            public Int32 PassMag;
            public Boolean IsShowStopMag = false;
            public Int32 StopMag;

            public static List<FilterLimit> InitLimits(Int32 DesignerType, Int32 MethodType, Double FreqFactor)
            {
                List<FilterLimit> limits = new List<FilterLimit>();

                if (DesignerType == (Int32)FilterType.FIRFilter)
                {
                    switch (MethodType)
                    {

                        case (Int32)FIRType.FreqSampling:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            ;

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            break;
                        case (Int32)FIRType.Window:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            ;

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            break;
                        case (Int32)FIRType.Remez:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1},
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1,MaxLimit=LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1, MaxLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,//Wp＜Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,//Wp>Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFp1 = true,
                                IsShowFs1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1, MaxLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            ;
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,//Wp（1）＜Ws（1）＞Ws（2）＜Wp（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1, MaxLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            break;
                    }

                }
                else if (DesignerType == (Int32)FilterType.IIRFilter)
                {
                    switch (MethodType)
                    {
                        case (Int32)IIRType.Butterworth:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero)
                            });
                            ;

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero)
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,//Wp＜Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,//Wp>Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFp1 = true,
                                IsShowFs1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1, MaxLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fs1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            ;
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,//Wp（1）＜Ws（1）＞Ws（2）＜Wp（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1, MaxLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fs1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            break;
                        case (Int32)IIRType.ChebyshevI:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                 Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowPassMag = true,
                                PassMag = 1
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowPassMag = true,
                                PassMag = 1
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                PassMag = 1
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,//Wp＜Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,//Wp>Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,//Ws（1）＜Wp（1）＞Wp（2）＜Ws（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fs1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fp1 },
                                Fp1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,//Wp（1）＜Ws（1）＞Ws（2）＜Wp（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MaxLimit = LimitObject.Fp2, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MinLimit = LimitObject.Fs2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            break;
                        case (Int32)IIRType.ChebyshevII:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowStopMag = true,
                                PassMag = 1
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowStopMag = true,
                                StopMag = 5000
                            });

                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowStopMag = true,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowStopMag = true,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,//Wp＜Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,//Wp>Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,//Ws（1）＜Wp（1）＞Wp（2）＜Ws（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fs1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fp1 },
                                Fp1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,//Wp（1）＜Ws（1）＞Ws（2）＜Wp（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MaxLimit = LimitObject.Fp2, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MinLimit = LimitObject.Fs2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            break;
                        case (Int32)IIRType.Elliptic:
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs2 = true,
                                 Fs2 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,
                                OrderMode = (Int32)FilterOrderMode.UserDefined,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.LowPass,//Wp＜Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.HighPass,//Wp>Ws
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFp1 = true,
                                IsShowFs1 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MinLimit = LimitObject.Fp1 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandPass,//Ws（1）＜Wp（1）＞Wp（2）＜Ws（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fs1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fp1 },
                                Fp1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MinLimit = LimitObject.Fp2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            limits.Add(new FilterLimit()
                            {
                                RespType = (Int32)FilterResponseType.BandStop,//Wp（1）＜Ws（1）＞Ws（2）＜Wp（2）
                                OrderMode = (Int32)FilterOrderMode.Minimum,
                                IsShowFs1 = true,
                                IsShowFp1 = true,
                                IsShowFs2 = true,
                                IsShowFp2 = true,
                                Fp1 = (Int32)Math.Round(0.1 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs1 },
                                Fs1 = (Int32)Math.Round(0.2 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs1Limit = new ValueLimit() { MaxLimit = LimitObject.Fs2, MinLimit = LimitObject.Fp1 },
                                Fs2 = (Int32)Math.Round(0.8 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fs2Limit = new ValueLimit() { MaxLimit = LimitObject.Fp2, MinLimit = LimitObject.Fp2 },
                                Fp2 = (Int32)Math.Round(0.9 * FreqFactor, MidpointRounding.AwayFromZero),
                                Fp2Limit = new ValueLimit() { MinLimit = LimitObject.Fs2 },
                                IsShowMag = true,
                                IsShowPassMag = true,
                                IsShowStopMag = true,
                                PassMag = 1,
                                StopMag = 5000
                            });
                            break;
                    }
                }
                return limits;
            }

        }

        /// <summary>
        /// 归一化频率数值限定（默认不等于）
        /// </summary>
        public class ValueLimit
        {
            public LimitObject MaxLimit = LimitObject.None;
            public LimitObject MinLimit = LimitObject.None;
            public Double MaxValue = 1.0;
            public Double MinValue = 0.0;
        }

        public enum LimitObject
        {
            None = 0,
            Fp1,
            Fs1,
            Fp2,
            Fs2,
        }
    }
}
