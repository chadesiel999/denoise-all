using ScopeX.Core.Tools;
using System;
using ScopeX.ComModel;
using System.Linq;
using ScopeX.Measure;
using NPOI.SS.Formula.Functions;

namespace ScopeX.Core
{
    internal class HorzAxisModel : PositionScaleTuple
    {   
        public HorzAxisModel(String name) : base(name)
        {
            GetPosValue = PosIndexToValue;
            GetPosIndex = PosIndexFromValue;

            GetScaleValue = ScaleIndexToValue;
            GetScaleIndex = ScaleIndexFromValue;

            PosMaxIndex = Constants.MAX_XPOS_IDX;
            PosMinIndex = 0; 
            PosStpIndex = Constants.STP_XPOS_IDX;
            PosDefIndex = Constants.DEF_XPOS_IDX;
            PosIdxPerDiv = Constants.IDX_PER_XDIV;
            PosPrefix = Prefix.Empty;
            PosUnit = "";
        }

        internal Double PosIndexToValue(Double posIndex, Double scale)
        {
            return (PosDefIndex- posIndex) * scale / PosIdxPerDiv;
        }

        internal Double ScaleIndexToValue(Int32 scaleIndex, Int32 _)
        {
            //var (initindex, _) = TimebaseModel.SetScaleByus(InitialScale.Value);
            //return TimebaseModel.GetScaleByus(scaleIndex - InitialScale.Index + initindex, _);
            return InitialScale.Value * ScaleFactory.Default.GetScale(scaleIndex - InitialScale.Index, InitialScale.Value);
        }

        internal Double PosIndexFromValue(Double posValue, Double scale)
        {
            return ValidatePosIndex(PosDefIndex - posValue * PosIdxPerDiv / scale);
        }

        internal (Int32 index, Int32 tick) ScaleIndexFromValue(Double scaleValue)
        {
            //获取水平各自绝对档位，然后计算相对档位
            //var (initindex, inittick) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value);
            //var (index, tick) = ScaleFactory.Default.TryGetScaleIndex(scaleValue);
            var (initindex, inittick) = TimebaseModel.SetScaleByus(InitialScale.Value);
            var (index, tick) = TimebaseModel.SetScaleByus(scaleValue);
            index -= initindex-InitialScale.Index;

            return (index, tick);
        }

        public override void SetInitScaleValue(Int32 index, Double value, Int32 minindex, Int32 maxindex,Boolean synchronize)
        {
            var (initindex, _) = TimebaseModel.SetScaleByus(InitialScale.Value);
            var (newinitindex, _) = TimebaseModel.SetScaleByus(value);
            var delta = (InitialScale.Index - initindex) - (index - newinitindex);
            var curindex = synchronize ? this.ScaleIndex : this.ScaleIndex - delta;

            this.InitialScale = (index, value);
            this.ScaleMinIndex = minindex < curindex ? minindex : curindex;
            this.ScaleMaxIndex = maxindex > curindex ? maxindex : curindex;
            if(this.ScaleIndex != curindex)
            {
                var tmp = this.PosIndex;
                this.ScaleIndex = curindex;
                this.PosIndex = tmp;
            }
        }


        public Prefix Prefix
        {
            get => ScalePrefix;
            set => ScalePrefix = value;
        }

        public String Unit
        {
            get => ScaleUnit;
            set => ScaleUnit = value;
        }
    }

}
