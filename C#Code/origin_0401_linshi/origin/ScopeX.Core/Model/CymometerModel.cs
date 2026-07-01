using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.Measure;

namespace ScopeX.Core
{
    internal class CymometerModel : INotifyPropertyChanged
    {
        public ChannelType Type
        {
            get;
        } = ChannelType.Cymometer;

        public ChannelId Id
        {
            get;
        }

        public String Name
        {
            get;
        }

        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged();
                }
            }

        }

        public Boolean _ShowPeriod = false;
        public Boolean ShowPeriod
        {
            get => _ShowPeriod;
            set
            {
                if (_ShowPeriod != value)
                {
                    _ShowPeriod = value;
                    var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source == ChannelId.CYM).ToList();
                    maths?.ForEach(x => x.ClearFlag = true);
                    StaBuffer.Clear();
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId? _Source = ChannelId.C1;
        public ChannelId? Source
        {
            //get => _Source;
            get => ChannelId.C1;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public CymometerModel(ChannelId id)
        {
            Id = id;
            Name = id.ToString();
            StaBuffer = new(10000);
        }

        private Boolean _IsStatActive = false;
        public Boolean IsStatActive
        {
            get => _IsStatActive;
            set
            {
                if (_IsStatActive != value)
                {
                    _IsStatActive = value;
                    OnPropertyChanged();
                }
            }
        }
        private Double _Current;

        public Double Current
        {
            get => _Current;

            set
            {
                _Current = value;
                if (!Double.IsNaN(value))
                {
                    StaBuffer.Insert(value);
                }
            }
        }

        public readonly StatisticBuffer StaBuffer;

        private Boolean _HistgramEnable = false;
        public Boolean HistgramEnable
        {
            get => _HistgramEnable;
            set
            {
                if (_HistgramEnable != value)
                {
                    _HistgramEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Histgram) ?? false;
                    if (value)
                        _HistgramEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        internal Func<Boolean, ChannelId, MeasItemFigureType, Boolean>? OpenOrCloseFigure { get; set; } = null;


        private Boolean _TrackEnable = false;
        public Boolean TrackEnable
        {
            get => _TrackEnable;
            set
            {
                if (_TrackEnable != value)
                {
                    _TrackEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Track) ?? false;
                    if (value)
                        _TrackEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _TrendEnable = false;
        public Boolean TrendEnable
        {
            get => _TrendEnable;
            set
            {
                if (_TrendEnable != value)
                {
                    _TrendEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Trend) ?? false;
                    if (value)
                        _TrendEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        public String Unit => _ShowPeriod ? QuantityUnitExt.ToUnitString(QuantityUnit.Second) : QuantityUnitExt.ToUnitString(QuantityUnit.Hertz);

        public String FrequencyToString(Double value)
        {
            return _ShowPeriod ? new Quantity(value, Prefix.Empty, QuantityUnit.Second).ToString("##0.0000000", true, 9).Replace(Unit, String.Empty)
                              : new Quantity(value, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.0000000", true, 9).Replace(Unit, String.Empty);
        }
        //Random rand = new Random();

        internal Double CurrentCym = Double.NaN;
        public void GetFrequency()
        {
            Double result = Double.NaN;

            if (Active && Source != null)
            {
                result = Hd.Cymometer?.GetFrequencyByHz((Int32)ChannelId.C1) ?? Double.NaN;
                if (result == 0)
                {
                    result = Double.NaN;
                }
            }
            //仿真测试数据
            //10Hz
            //double min = 9.888800073;
            //double max = 10.1140007301;
            //100Hz
            //double min = 99.888800073;
            //double max = 100.1140007301;
            //1k
            //double min = 999.888800073;
            //double max = 1000.1140007301;
            //10k
            //double min = 9999.888800073;
            //double max = 10000.1140007301;
            //100k
            //double min = 99999.888800073;
            //double max = 100000.1140007301;
            //1M
            //double min = 999999.888800073;
            //double max = 1000000.1140007301;
            //10M
            //double min = 9999999.888800073;
            //double max = 10000000.1140007301;
            //100M
            //double min = 99988888.888800073;
            //double max = 100001140.1140007301;
            //1G
            //double min = 999988888.888800073;
            //double max = 1000001140.1140007301;
            //10G
            //double min = 9999999999.888800073;
            //double max = 10000000000.1140007301;

            //result = min + (max - min) * rand.NextDouble();
            if (_ShowPeriod && result != 0D)
            {
                result = 1 / result;
            }

            Current = result;
            if (result == Double.NaN)
                result = 0;
            CurrentCym = result;
            //if (!Active)
            //{
            //    return 0D;
            //}
            //if (Source == null)
            //{
            //    return 0D;
            //}

            ///！！！读频率计只能从C1读
            //return Hd.Cymometer?.GetFrequencyByHz((Int32)ChannelId.C1) ?? 0D;


            //if (Source != null)
            //{
            //    //return Hd.Cymometer?.GetFrequencyByHz() ?? 0D;
            //    return Hd.Cymometer?.GetFrequencyByHz((Int32)Source.Value) ?? 0D;
            //}
            //else
            //{
            //    return 0D;
            //}
        }
    }
}
