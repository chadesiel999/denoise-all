// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;

    internal class VoltmeterModel : INotifyPropertyChanged
    {
        public ChannelType Type
        {
            get;
        } = ChannelType.Voltmeter;

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
                    KeyLed.Default.SetLed(LedEnum.LedDVM, value);
                }
            }
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

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private VoltmeterMode _Mode = VoltmeterMode.DC;
        public VoltmeterMode Mode
        {
            get => _Mode;
            set
            {
                if (_Mode != value)
                {
                    var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source == ChannelId.DVM).ToList();
                    maths?.ForEach(x => x.ClearFlag = true);
                    StaBuffer.Clear();
                    _Mode = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AutoRange = true;
        public Boolean AutoRange
        {
            get => _AutoRange;
            set
            {
                if (_AutoRange != value)
                {
                    _AutoRange = value;
                    OnPropertyChanged();
                }
            }
        }

        //public Double GetVolt()
        //{            
        //    return new Random().NextDouble() * 1000;
        //}

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

        public VoltmeterModel(ChannelId id)
        {
            Id = id;
            Name = id.ToString();
            StaBuffer = new(10000);
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

        public String Unit => DsoModel.Default.GetChannel(Source).Conditioning.Unit;

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

        public void CalcVoltBymV()
        {
            if (!IsTriggerSource() && AutoRange)
            {
                GetRange();
            }

            if (IsACCoupling(Source))
            {
                Mode = VoltmeterMode.ACrms;
            }

            //Double dcacrms = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("RMS", Source) ?? Double.NaN;
            //Double dc = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Average", Source) ?? Double.NaN;
            //Double acrms = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Stddev", Source) ?? Double.NaN;

            Current = Mode switch
            {
                VoltmeterMode.DC => DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Average", Source) ?? Double.NaN,
                VoltmeterMode.ACrms => DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Stddev", Source) ?? Double.NaN,
                VoltmeterMode.DCACrms => DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("RMS", Source) ?? Double.NaN,
                _ => Double.NaN,
            };
        }

        public void GetRange()
        {
            Double max = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Max", Source) ?? Double.NaN;
            Double min = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Min", Source) ?? Double.NaN;
            Double curscale = DsoModel.Default.GetChannel(Source).Conditioning.Scale;

            if (!DsoModel.Default.GetChannel(Source).Active && (max - min < 2 * curscale || max >= 4 * curscale || min <= -4 * curscale))
            {
                //幅度不在20%-80%调节
                Double zoom = Math.Round((max - min) / curscale);

                if ((curscale == DsoModel.Default.GetChannel(Source).Conditioning.MaxScale && zoom > 8) ||
                    (curscale == DsoModel.Default.GetChannel(Source).Conditioning.MinScale && zoom < 3))
                {
                    return;
                }

                Int32 scaleindex = DsoModel.Default.GetChannel(Source).Conditioning.ScaleIndex;
                Int32 minindex = DsoModel.Default.GetChannel(Source).Conditioning.ScaleMinIndex;
                Int32 maxindex = DsoModel.Default.GetChannel(Source).Conditioning.ScaleMaxIndex;
                if (DsoPrsnt.DefaultDsoPrsnt != null)
                {
                    if (Source.IsAnalog() && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var channel))
                    {
                        channel.ScaleIndex = zoom switch
                        {
                            0 => scaleindex - 3 < minindex ? minindex : scaleindex - 3,//波形在屏幕外测量不准时，出现0
                            1 => scaleindex - 2 < minindex ? minindex : scaleindex - 2,
                            2 => scaleindex - 1 < minindex ? minindex : scaleindex - 1,
                            9 or 10 => scaleindex + 1 > maxindex ? maxindex : scaleindex + 1,
                            11 => scaleindex + 2 > maxindex ? maxindex : scaleindex + 2,
                            _ => maxindex,
                        };
                    }
                }

                //DsoModel.Default.GetChannel(Source).Conditioning.ScaleIndex = zoom switch
                //{
                //    0 => scaleindex - 3 < minindex ? minindex : scaleindex - 3,//波形在屏幕外测量不准时，出现0
                //    1 => scaleindex - 2 < minindex ? minindex : scaleindex - 2,
                //    2 => scaleindex - 1 < minindex ? minindex : scaleindex - 1,
                //    9 or 10 => scaleindex + 1 > maxindex ? maxindex : scaleindex + 1,
                //    11 => scaleindex + 2 > maxindex ? maxindex : scaleindex + 2,
                //    _ => maxindex,
                //};
            }
        }

        public static Boolean IsACCoupling(ChannelId source)
        {
            var chn = DsoModel.Default.GetChannel(source);
            if (chn is AnalogModel achn)
            {
                return achn.Conditioning.Coupling == AnaChnlCoupling.AC1M;
            }
            return false;
        }

        public Boolean IsTriggerSource()
        {
            var trigger = DsoModel.Default.GetTrigger();
            return trigger switch
            {
                TriggerSingleSrcModel tssm => tssm.Source == Source,
                TriggerMultiLevelModel tmlm => tmlm.Source == Source,
                TriggerStateModel tsm => tsm.ClkSource == Source || tsm.Bits.GetCondition(Source) != PatLevelCondition.Any,
                TriggerPatternModel tpm => tpm.Bits.GetCondition(Source) != PatLevelCondition.Any,
                TriggerSetupHoldModel tshm => tshm.ClkSource == Source || tshm.DataSource == Source,
                _ => false,
            };
        }


        public String VoltageToString(Double value)
        {
            return new Quantity(value, Prefix.Milli, Unit).ToString("##0.000", true, 6);
        }

    }
}
