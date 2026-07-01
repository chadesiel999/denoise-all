using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ScopeX.ComModel;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    public abstract class MathArgPrsnt : INotifyPropertyChanged
    {
        private protected readonly MathModel Model;

        public MathArgPrsnt(MathPrsnt mp, ChannelId id, MathType mt, Object? occupier = null)
        {
            Type = mt;
            Model = (MathModel)DsoModel.Default.GetChannel(id);
            Occupier = occupier;
            RunState = RunStateType.Repeat;
            PropertyChanged += mp.OnPropertyChanged;
        }

        public Object? Occupier
        {
            get { return Model.Occupier; }
            internal set
            {
                Model.Occupier = value;
                if (value is JitterGraphModel jitter)
                {
                    if (jitter.Formula == Constants.JITTER_HISTOGRAM_FORMULA)
                        Model.AutoScale = true;
                    Model.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                    Model.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                    Model.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                    Model.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.Both;

                    jitter.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                    jitter.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                    jitter.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                    jitter.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                }
                OnPropertyChanged();
            }
        }

        public MathType Type
        {
            get;
        }

        public abstract String Description
        {
            get;
        }
        private Boolean _IsAutoUnit;
        public virtual Boolean IsAutoUnit
        {
            get => _IsAutoUnit;
            set
            {
                if (_IsAutoUnit != value)
                {
                    _IsAutoUnit = value;
                    OnPropertyChanged();
                }
            }
        }
        private String _CustomUnit;

        public virtual String CustomUnit
        {
            get => _CustomUnit;
            set
            {
                if (!String.Equals(_CustomUnit, value))
                {
                    _CustomUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public abstract String MakeFormula();

        public static Boolean TryParse(String formula, [NotNullWhen(true)] out (MathType ExpType, String Exp)? arg)
        {
            var sep = formula.IndexOf(":");
            if (sep > 0)
            {
                if (Enum.TryParse<MathType>(formula[0..sep], out var exptype))
                {
                    arg = (exptype, formula[(sep + 1)..]);

                    return true;
                }
            }

            arg = null;
            return false;
        }

        /// <summary>
        /// save formula as file;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public virtual Boolean SaveFormula(String path, String name, String formula)
        {
            return false;
        }

        /// <summary>
        /// load formula by file;
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public virtual Boolean LoadFormula(String fullName, out String formula)
        {
            formula = "";
            return false;
        }


        /// <summary>
        /// 运行次数的状态标记：循环运行、运行一次，停止运行；默认为循环运行
        /// </summary>
        public RunStateType RunState
        {
            get;
            set;
        }


        internal static void ScaleFit(MathModel mch, Vector? res)
        {
            if (res == null)
            {
                return;
            }
            //=res.XUnit;
            var unx = res.XUnit;
            var uny = res.YUnit;

            //设置Y轴值范围
            var data = res.Elements;
            if (data != null && data.Length > 0)
            {
                Double max = data[0, 0];
                Double min = data[0, 0];
                for (Int32 i = 0; i < data.GetLength(0); i++)
                {
                    for (Int32 j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j] > max)
                            max = data[i, j];
                        if (data[i, j] < min)
                            min = data[i, j];
                    }
                }

                Double range = (max == min) ? max : max - min;
                Double average = (max == min) ? max / 2 : (max + min) / 2;

                var prefix = uny == "V" ? Prefix.Milli : mch.Conditioning.Prefix;
                range = Quantity.ConvertByPrefix(range, Prefix.Empty, prefix);
                average = Quantity.ConvertByPrefix(average, Prefix.Empty, prefix);
                //从小到大找到合适的刻度
                for (Int32 i = mch.Conditioning.ScaleMinIndex; i < mch.Conditioning.ScaleMaxIndex; i++)
                {
                    Double scale = mch.Conditioning.GetScaleValue(i, 0);
                    if (range <= (scale * Constants.VIS_YDIVS_NUM * 2 / 3) ||
                        i == mch.Conditioning.ScaleMaxIndex)
                    {
                        mch.Conditioning.ScaleIndex = i;
                        break;
                    }
                }

                //设置到合适的中间位置
                mch.Conditioning.PosIndex = -1 * average / mch.Conditioning.Scale * mch.Conditioning.PosIdxPerDiv;
            }

        }

        public static String GetName(MathArgPrsnt prsnt)
        {
            var type = prsnt.GetType();
            var srcname = GetSource(prsnt);
            String name = type.Name switch
            {
                nameof(MathHistArg) => "Hist",
                nameof(MathTrendArg) => "Trd",
                nameof(MathTrackArg) => "Trk",
                _ => String.Empty
            };
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(srcname))
            {
                var src = DsoPrsnt.DefaultDsoPrsnt.Measure.SelectedItems.FirstOrDefault(x => x.Id.ToString() == srcname);
                if (src != null)
                {
                    srcname = src.MeasureType switch
                    {
                        MeasureType.Single => src.Name.Length > 7 ? Regex.Replace(src.Name, "[a-z]", "") : src.Name,
                        MeasureType.Composite => $"{src.Source} {src.Operation.GetDescription()} {src.Source2nd}",
                        _ => String.Empty
                    };
                    name = $"{src.Id.ToString()}:{srcname}";
                }
                else
                {
                    name += $"{srcname}";
                }
            }
            if (prsnt.Occupier != null)
            {
                if (prsnt.Occupier is JitterGraphModel jgm)
                {
                    name = "Jitter";
                }
                else if (prsnt.Occupier is PowerAnalysisModel pa)
                {
                    name = pa.Mode.GetDescription();
                }
            }

            if (String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(srcname))
            {
                name += $"({srcname})";
            }
            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(srcname))
            {
                name = "y=f(x)";
            }

            return name;
        }

        public static String GetSource(MathArgPrsnt prsnt)
        {
            var type = prsnt.GetType();
            var info = type.GetProperty("Source");
            var value = info?.GetValue(prsnt);
            return value?.ToString();
        }

        public Boolean IsJitterTypeOccupier(String Formula = null)
        {
            if (String.IsNullOrEmpty(Formula) && Occupier != null && Occupier is JitterGraphModel)//如果Formula为null则判定是否为抖动相关
            {
                return true;
            }

            if (Occupier != null && Occupier is JitterGraphModel model && model.Formula == Formula)// 如果Formula不为null，则判定是否为抖动的对应图形
            {
                return true;
            }

            return false;
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                ChannelShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                ChannelShareParameter.Default.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
