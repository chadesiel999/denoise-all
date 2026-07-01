using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;

namespace ScopeX.Core
{
    //该类作为一个适配器，用来适配P层对象的频率刻度及位置到FFT子页面；
    public class FrequencyAdapter
    {
        private MathModel _MathModel;                    //对应的数学P层实例

        Double _ValueCenter;                             //中心频率（对应P层的prefix和unit）
        public Double ValueCenter
        {
            get => GetVolueCenter();
            set
            {
                if (value < ValueCenterMin(Prefix.Empty))
                {
                    WeakTip.Default.Write(nameof(ValueCenter), MsgTipId.LessthanMin, false, "", 1);
                    value = ValueCenterMin(Prefix.Empty);
                }
                else if (value > ValueCenterMax(Prefix.Empty))
                {
                    WeakTip.Default.Write(nameof(ValueCenter), MsgTipId.GreatethanMax, false, "", 1);
                    value = ValueCenterMax(Prefix.Empty);
                }
                _ValueCenter = value;
                _MathModel.Sampling.PosIndex = (Constants.VIS_XDIVS_NUM / 2.0 - _ValueCenter / _MathModel.Sampling.Scale) * _MathModel.Sampling.PosIdxPerDiv;
                //FreshPrsnt();
                OnPropertyChanged?.Invoke(nameof(ValueCenter));
            }
        }

        public Double ValueCenterBymHz
        {
            get => ValueCenter / 1000D;
            set => ValueCenter = value * 1000D;
        }

        Double _ValueSpan;
        public Double ValueSpan
        {
            get => GetVolueSpan();
            set
            {
                if (value < ValueSpanMin(Prefix.Empty))
                {
                    WeakTip.Default.Write(nameof(ValueSpan), MsgTipId.LessthanMin, false, "", 1);
                    value = ValueSpanMin(Prefix.Empty);
                }
                else if (value > ValueSpanMax(Prefix.Empty))
                {
                    WeakTip.Default.Write(nameof(ValueSpan), MsgTipId.GreatethanMax, false, "", 1);
                    value = ValueSpanMax(Prefix.Empty);
                }
                _ValueSpan = value;
                _MathModel.Sampling.ScaleIndex = GetSuitableScaleIndex(_ValueSpan / Constants.VIS_XDIVS_NUM);
                OnPropertyChanged?.Invoke(nameof(ValueSpan));
            }
        }

        public Double ValueSpanBymHz
        {
            get => ValueSpan / 1000D;
            set => ValueSpan = value * 1000D;
        }

        public Int32 ScaleIndexSpan
        {
            get => _MathModel.Sampling.ScaleIndex;
            set
            {
                if (value != _MathModel.Sampling.ScaleIndex)
                {
                    if (value < _MathModel.Sampling.ScaleMinIndex)
                    {
                        WeakTip.Default.Write(nameof(ScaleIndexSpan), MsgTipId.LessthanMin, false, "", 1);
                        value = (Int32)_MathModel.Sampling.ScaleMinIndex;
                    }
                    else if (value > _MathModel.Sampling.ScaleMaxIndex)
                    {
                        WeakTip.Default.Write(nameof(ScaleIndexSpan), MsgTipId.GreatethanMax, false, "", 1);
                        value = (Int32)_MathModel.Sampling.ScaleMaxIndex;
                    }
                    _MathModel.Sampling.ScaleIndex = value;
                    GetVolueSpan();
                }
            }
        }

        private Double GetVolueSpan()
        {
            _ValueSpan = Quantity.ConvertByPrefix((Double)((decimal)_MathModel.Sampling.Scale * Constants.VIS_XDIVS_NUM), Prefix.Empty, Prefix.Empty);
            if (_ValueSpan < ValueSpanMin(Prefix.Empty))
                _ValueSpan = ValueSpanMin(Prefix.Empty);
            else if (_ValueSpan > ValueSpanMax(Prefix.Empty))
                _ValueSpan = ValueSpanMax(Prefix.Empty);
            return _ValueSpan;
        }

        private Double GetVolueCenter()
        {
            if (_ValueCenter < ValueCenterMin(Prefix.Empty))
                _ValueCenter = ValueCenterMin(Prefix.Empty);
            else if (_ValueCenter > ValueCenterMax(Prefix.Empty))
                _ValueCenter = ValueCenterMax(Prefix.Empty);
            return _ValueCenter;
        }

        private Double GetVolueSpanByScaleIndex()
        {
            _ValueSpan = Quantity.ConvertByPrefix(_MathModel.Sampling.Scale * Constants.VIS_XDIVS_NUM, Prefix.Empty, Prefix.Empty);
            if (_ValueSpan < ValueSpanMin(Prefix.Empty))
                _ValueSpan = ValueSpanMin(Prefix.Empty);
            else if (_ValueSpan > ValueSpanMax(Prefix.Empty))
                _ValueSpan = ValueSpanMax(Prefix.Empty);
            return _ValueSpan;
        }

        private Double GetVolueCenterByPosIndex()
        {
            var valueCenter = (Constants.VIS_XDIVS_NUM / 2.0 - _MathModel.Sampling.PosIndex / _MathModel.Sampling.PosIdxPerDiv) * _MathModel.Sampling.Scale;
            return valueCenter;

        }

        public static readonly Double CenterMin = 0;
        public static readonly Double CenterMax = 8E15;
        public static readonly Double SpanMin = 5E8;
        public static readonly Double SpanMax = 1E15;

        public Double ValueCenterMin(Prefix prefix)
        {
            return Quantity.ConvertByPrefix(CenterMin, prefix);
        }

        public Double ValueCenterMax(Prefix prefix)
        {
            return Quantity.ConvertByPrefix(2.5*CenterMax, prefix);
        }

        public Double ValueSpanMin(Prefix prefix)
        {
            return Quantity.ConvertByPrefix(SpanMin * Constants.VIS_XDIVS_NUM, prefix);
        }

        public Double ValueSpanMax(Prefix prefix)
        {
            return Quantity.ConvertByPrefix(2*SpanMax * Constants.VIS_XDIVS_NUM, prefix);
        }

        internal FrequencyAdapter(MathModel mModel, Action<String>? onPropertyChanged)
        {
            _MathModel = mModel;
            OnPropertyChanged = onPropertyChanged;
        }

        //设置合适的垂直刻度及位置
        public void FreshVerticalValue()
        {
            //设置Y轴值范围
            var data = _MathModel.Pack?.Buffer;
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

                Double range = max - min;
                Double average = (max + min) / 2;

                //从小到大找到合适的刻度
                for (Int32 i = _MathModel.Conditioning.ScaleMinIndex; i < _MathModel.Conditioning.ScaleMaxIndex; i++)
                {
                    Double scale = _MathModel.Conditioning.GetScaleValue(i, 0);
                    if (range <= (scale * Constants.VIS_YDIVS_NUM * 2 / 3) ||
                        i == _MathModel.Conditioning.ScaleMaxIndex)
                    {
                        _MathModel.Conditioning.ScaleIndex = i;
                        break;
                    }
                }

                //设置到合适的中间位置
                _MathModel.Conditioning.PosIndex = -1 * average / _MathModel.Conditioning.Scale * _MathModel.Conditioning.PosIdxPerDiv;
            }

        }

        //初始化合适的频率范围
        public void FreshHorizonValue()
        {
            if (_MathModel.Pack != null)
            {
                //设置频率范围
                ValueSpan = Quantity.ConvertByPrefix(_MathModel.Sampling.MaxScale * Constants.VIS_XDIVS_NUM, Prefix.Empty, Prefix.Micro);
            }
        }

        //刷新P层的值
        public void FreshPrsnt()
        {
            _MathModel.Sampling.ScaleIndex = GetSuitableScaleIndex(_ValueSpan / Constants.VIS_XDIVS_NUM);
            _MathModel.Sampling.PosIndex = (Constants.VIS_XDIVS_NUM / 2.0 - ValueCenter / _MathModel.Sampling.Scale) * _MathModel.Sampling.PosIdxPerDiv;
        }



        //获取一个最合适的ScalIndex,使得desireScale与Scale最为匹配
        private Int32 GetSuitableScaleIndex(Double desireScale)
        {
            if (desireScale > _MathModel.Sampling.MaxScale)
                desireScale = _MathModel.Sampling.MaxScale;
            if (desireScale < _MathModel.Sampling.MinScale)
                desireScale = _MathModel.Sampling.MinScale;

            for (Int32 i = (Int32)_MathModel.Sampling.ScaleMinIndex; i <= (Int32)_MathModel.Sampling.ScaleMaxIndex; i++)
            {
                if (_MathModel.Sampling.GetScaleValue(i, 0) >= desireScale)
                    return i;
            }

            return (Int32)_MathModel.Sampling.ScaleMaxIndex;
        }

        protected Action<String>? OnPropertyChanged
        {
            get;
        }
    }
}
