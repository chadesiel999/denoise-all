using System;
using ScopeX.Core.Tools;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class VertAxisModel : PositionScaleTuple
    {
        public VertAxisModel(String name) : base(name)
        {
            GetPosValue = GetPosValueFromIndex;
            GetPosIndex = GetPosIndexFromValue;

            GetScaleValue = GetScaleValueFromIndex;
            GetScaleIndex = GetScaleIndexFromValue;
            IsScaleTickOverflow = (index, tick) => tick >= ScaleFactory.Default.GetPosTicks(index);
            IsScaleTickUnderflow = (index, tick) => tick <= ScaleFactory.Default.GetNegTicks(index);

            PosMaxIndex = Constants.MAX_YPOS_IDX;
            PosMinIndex = Constants.MIN_YPOS_IDX;
            PosStpIndex = Constants.STP_YPOS_IDX;
            PosDefIndex = Constants.DEF_YPOS_IDX;
            PosIdxPerDiv = Constants.IDX_PER_YDIV;
            PosPrefix = Prefix.Milli;
            PosUnit = "div";
        }

        private Double GetPosValueFromIndex(Double posIndex, Double scale)
        {
            return posIndex / PosIdxPerDiv * scale;
        }

        private Double GetScaleValueFromIndex(Int32 scaleIndex, Int32 scaleTick)
        {
            var (initindex, _) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
            return ScaleFactory.Default.GetScale(scaleIndex - InitialScale.Index + initindex, 0);
        }

        private Double GetPosIndexFromValue(Double posValue, Double scale)
        {
            return ValidatePosIndex(posValue * PosIdxPerDiv / scale);
        }

        private (Int32 Index, Int32 Tick) GetScaleIndexFromValue(Double scaleValue)
        {
            //获取各自据对档位，然后计算相对档位
            var (initindex, _) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
            var (index, tick) = ScaleFactory.Default.TryGetScaleIndex(scaleValue, InitialScale.Value);
            index -= initindex - InitialScale.Index;
            return (index, tick);
        }

        public override void SetInitScaleValue(Int32 index, Double value, Int32 minindex, Int32 maxindex, Boolean synchronize)
        {
            var (initindex, _) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
            var (newinitindex, _) = ScaleFactory.Default.TryGetScaleIndex(value, value);
            var delta = (InitialScale.Index - initindex) - (index - newinitindex);
            var curindex = synchronize ? this.ScaleIndex : this.ScaleIndex - delta;

            this.InitialScale = (index, value);
            this.ScaleMinIndex = minindex < curindex ? minindex : curindex;
            this.ScaleMaxIndex = maxindex > curindex ? maxindex : curindex;
            if (this.ScaleIndex != curindex)
                this.ScaleIndex = curindex;
        }

        public Prefix Prefix
        {
            get => ScalePrefix;
            set => ScalePrefix = value;
        }

        public virtual String Unit
        {
            get => ScaleUnit;
            set => ScaleUnit = value;
        }

        public Double Position
        {
            get => GetPosValue(base.PosIndex, Scale);
            set => base.PosIndex = GetPosIndex(value, Scale);
        }

        //for future
        public Double MaxPosition => GetPosValue(PosMaxIndex, Scale);

        //for future
        public Double MinPosition => GetPosValue(PosMinIndex, Scale);

        public Double Scale
        {
            get => GetScaleValue(ScaleIndex, ScaleTick);
            set
            {
                (ScaleIndex, _) = GetScaleIndex(value);
            }
        }

        private Double _ScaleBymVAdd = 0;
        /// <summary>
        /// 细调幅度值
        /// </summary>
        public Double ScaleBymVAdd
        {
            get { return _ScaleBymVAdd; }//Math.Round(_ScaleBymVAdd, 4);
            set
            {
                _ScaleBymVAdd = value;
                OnPropertyChanged("ConditioningScale");
                DsoModel.Default.GetTrigger().LeapPosIndex();
                if (DsoModel.Default.Timebase.IsScan)
                {
                    Dispatcher.SoftReset();
                }
            }
        }

        /// <summary>
        /// 当前界面设置的幅度为
        /// Conditioning.Scale:由档位得到的幅度
        /// ScaleBymVAdd=设置的幅度-由档位得到的幅度
        /// </summary>
        public Double ScaleBymV
        {
            get => IsUpdateScale ? OldScaleBymV : Scale + ScaleBymVAdd;
        }

        private Double OldScaleBymV = 0;
        private Boolean _IsUpdateScale = false;
        public Boolean IsUpdateScale
        {
            get
            {
                return _IsUpdateScale;
            }
            set
            {
                if (_IsUpdateScale != value)
                {
                    OldScaleBymV = Scale + ScaleBymVAdd;
                    _IsUpdateScale = value;
                }
            }
        }

        public Double MaxScale => GetScaleValue(ScaleMaxIndex, 0);

        public Double MinScale => GetScaleValue(ScaleMinIndex, 0);
    }

}
