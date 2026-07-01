using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class RadioFrequencyModel : ChannelModel
    {
        public RadioFrequencyModel(ChannelId id, Color color, Boolean active, FrequencyModel frequencyModel,AmpVSTimeModel ampVSTime,PhaseVSTimeModel phaseVSTime,PhaseVSFrequencyModel phaseVSFrequency,TimeVSFrequencyModel timeVSFrequency,FrequencyVSTimeModel frequencyVSTime)
            : base(ChannelType.RadioFrequency, id, color)
        {
            Active = active;

            Conditioning = new AmplitudeModel();
            Sampling = frequencyModel;
            AmpVSTime = ampVSTime;
            PhaseVSTime = phaseVSTime;
            PhaseVSFrequency = phaseVSFrequency;
            TimeVSFrequency = timeVSFrequency;
            FrequencyVSTime = frequencyVSTime;

            SyncMDParm();
        }

        public override Boolean Active
        {
            get => base.Active;
            set {
                if (value == true)
                {
                    Sampling.Init = true;
                }
                base.Active = value;
            }
        }

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (value != _Source)
                {
                    _Source = value;
                    Sampling.Source = value;
                    OnPropertyChanged();
                }
            }
        }

        public override AmplitudeModel Conditioning
        {
            get;
        }

        public override FrequencyModel Sampling
        {
            get;
        }

        #region Frequency
        public Int32 FFTLength
        {
            get
            {
                return Sampling.FFTLength;
            }
            set
            {
                if (Sampling.FFTLength != value)
                {
                    Sampling.FFTLength = value;
                    //TimeVSFrequency.Sampling.FFTLength = value;
                    //PhaseVSFrequency.Sampling.FFTLength = value;
                    //FrequencyVSTime.Conditioning.FFTLength = value;
                  
                    SyncMDParm();
                }
            }
        }
        public Int32 STFTLength
        {
            get => Sampling.STFTLength;
            set
            {
                if (value != Sampling.STFTLength)
                {
                    Sampling.STFTLength = value;
                    SyncMDParm();
                }
            }
        }

        public Int32 STFTStep
        {
            get => Sampling.STFTStep;
            set
            {
                if (value != Sampling.STFTStep)
                {
                    Sampling.STFTStep = value;
                    SyncMDParm();
                }
            }
        }
        public Int64 StartFrequency
        {
            get
            {
                return Sampling.StartFrequency;
            }
            set
            {
                if (Sampling.StartFrequency != value)
                {
                    Sampling.StartFrequency = value;
                    //TimeVSFrequency.Sampling.StartFrequency = value;
                    //PhaseVSFrequency.Sampling.StartFrequency = value;
                    SyncMDParm();
                }
            }
        }

        public Int64 CenterFrequency
        {
            get
            {
                return Sampling.CenterFrequency;
            }
            set
            {
                if (Sampling.CenterFrequency != value)
                {
                    Sampling.CenterFrequency = value;
                    //TimeVSFrequency.Sampling.CenterFrequency = value;
                    //PhaseVSFrequency.Sampling.CenterFrequency = value;
                    SyncMDParm();
                }
            }
        }

        public Int64 EndFrequency
        {
            get
            {
                return Sampling.EndFrequency;
            }
            set
            {
                if (Sampling.EndFrequency != value)
                {
                    Sampling.EndFrequency = value;
                    //TimeVSFrequency.Sampling.EndFrequency = value;
                    //PhaseVSFrequency.Sampling.EndFrequency = value;
                    SyncMDParm();
                }
            }
        }

        public Int64 Span
        {
            get
            {
                return Sampling.Span;
            }
            set
            {
                if (Sampling.Span != value)
                {
                    Sampling.Span = value;
                    //TimeVSFrequency.Sampling.Span = value;
                    //PhaseVSFrequency.Sampling.Span = value;
                    SyncMDParm();
                }
            }
        }

        public Double RBW
        {
            get
            {
                return Sampling.RBW;
            }
            set
            {
                if (Sampling.RBW != value)
                {
                    Sampling.RBW = value;
                    //TimeVSFrequency.Sampling.RBW = value;
                    //PhaseVSFrequency.Sampling.RBW = value;
                    SyncMDParm();
                }
            }
        }

        private void SyncMDParm()
        {
            //AmpVSTime.Sampling.TimeScale = Sampling.STFTLength * (1000000000000 / Sampling.TranslateSampleRate) / Constants.VIS_XDIVS_NUM ;
            //PhaseVSTime.Sampling.TimeScale = Sampling.STFTLength * (1000000000000 / Sampling.TranslateSampleRate) / Constants.VIS_XDIVS_NUM ;
            //PhaseVSTime.Conditioning.TimeScale = Sampling.FFTLength * (1000000 / Sampling.TranslateSampleRate) / Constants.VIS_YDIVS_NUM;
            //PhaseVSFrequency.Conditioning.TimeScale = 1000000000000 / (Sampling.TranslateSampleRate / Sampling.FFTLength) / (Constants.VIS_YDIVS_NUM / 2);
            //TimeVSFrequency.Conditioning.TimeScale = Sampling.DataLength * (1000000 / Sampling.TranslateSampleRate) / Constants.VIS_YDIVS_NUM;

            //AmpVSTime.Sampling.TranslateSamplerate =  Sampling.TranslateSampleRate;
            //PhaseVSTime.Sampling.TranslateSamplerate =  Sampling.TranslateSampleRate;
        }
        #endregion
        #region 多域

        public AmpVSTimeModel AmpVSTime
        {
            get;
        }

        public PhaseVSTimeModel PhaseVSTime
        {
            get;
        }

        public PhaseVSFrequencyModel PhaseVSFrequency
        {
            get;
        }

        public TimeVSFrequencyModel TimeVSFrequency
        {
            get;
        }

        public FrequencyVSTimeModel FrequencyVSTime
        {
            get;
        }

        public Boolean ThreeD
        {
            get => TimeVSFrequency.ThreeD;
            set
            {
                if (value != TimeVSFrequency.ThreeD)
                {
                    TimeVSFrequency.ThreeD = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region 谱线
        public delegate void Init();
        public Init? ProcessInit
        {
            get;
            set;
        }
        public ProcessHandler<Double[,]>? ProcessNormalSamples
        {
            get;
            set;
        } 
        public ProcessHandler<Double[,]>? ProcessAverageSamples
        {
            get;
            set;
        } 
        public ProcessHandler<Double[,]>? ProcessMaxHoldSamples
        {
            get;
            set;
        } 
        public ProcessHandler<Double[,]>? ProcessMinHoldSamples
        {
            get;
            set;
        } 
        public Func<Object, MDVirticalType, RFWaveType, WfmVuBlock?>? MakeVuSamplesIQFFT
        {
            get;
            set;
        }

        private Boolean _ResetLine = false;

        #region 正常
        private Boolean _NormalLine = false;
        public Boolean NormalLine
        {
            get { return _NormalLine; }
            set {
                if (_NormalLine != value)
                { 
                    _NormalLine = value;
                    _ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        private PickMode _NormalLinePickMode = PickMode.Sample;
        public PickMode NormalLinePickMode
        {
            get { return _NormalLinePickMode; }
            set
            {
                if (_NormalLinePickMode != value)
                {
                    _NormalLinePickMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color NormalLineColor = Color.Yellow;

        public WfmPack? PackNormal
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseNormal
        {
            get;
        } = new();

        #endregion

        #region 最大值保持

        private Boolean _MaxHoldLine = false;
        public Boolean MaxHoldLine
        {
            get { return _MaxHoldLine; }
            set
            {
                if (_MaxHoldLine != value)
                {
                    _MaxHoldLine = value;
                    _ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        private PickMode _MaxHoldLinePickMode = PickMode.Sample;
        public PickMode MaxHoldLinePickMode
        {
            get { return _MaxHoldLinePickMode; }
            set
            {
                if (_MaxHoldLinePickMode != value)
                {
                    _MaxHoldLinePickMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color MaxHoldLineColor = Color.Red;

        public WfmPack? PackMaxHold
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseMaxHold
        {
            get;
        } = new();

        #endregion

        #region 最小值保持


        private Boolean _MinHoldLine = false;
        public Boolean MinHoldLine
        {
            get { return _MinHoldLine; }
            set
            {
                if (_MinHoldLine != value)
                {
                    _MinHoldLine = value;
                    _ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        private PickMode _MinHoldLinePickMode = PickMode.Sample;
        public PickMode MinHoldLinePickMode
        {
            get { return _MinHoldLinePickMode; }
            set
            {
                if (_MinHoldLinePickMode != value)
                { 
                    _MinHoldLinePickMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color MinHoldLineColor = Color.Blue;

        public WfmPack? PackMinHold
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseMinHold
        {
            get;
        } = new();

        #endregion

        #region 平均

        private Int32 _AverageTimes = Constants.RF_AVERAGE_TIMES_MAX;
        public Int32 AverageTimes
        {
            get { return _AverageTimes; }
            set
            {
                value = value < Constants.RF_AVERAGE_TIMES_MIN ? Constants.RF_AVERAGE_TIMES_MIN : value;
                value = value > Constants.RF_AVERAGE_TIMES_MAX ? Constants.RF_AVERAGE_TIMES_MAX : value;

                if (_AverageTimes != value)
                { 
                    _AverageTimes = value;
                    _ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AverageLine = false;
        public Boolean AverageLine
        {
            get { return _AverageLine; }
            set
            {
                if (_AverageLine != value)
                {
                    _AverageLine = value;
                    _ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        private PickMode _AverageLinePickMode = PickMode.Sample;
        public PickMode AverageLinePickMode
        {
            get { return _AverageLinePickMode; }
            set
            {
                if (_AverageLinePickMode != value)
                {
                    _AverageLinePickMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color AverageLineColor = Color.LightGreen;

        public WfmPack? PackAverage
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseAverage
        {
            get;
        } = new();

        #endregion

        #endregion
        #region 窗

        private RFWindowType _Window = RFWindowType.Rectangle;
        public RFWindowType Window
        {
            get
            {
                return _Window;
            }
            set
            {
                if (_Window != value)
                {
                    _Window = value;
                    Sampling.Init = true;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
        public override Boolean Take(Boolean init, CancellationToken ct, CancellationToken? softResetToken = null)
        {
            var args = PrepareSamples?.Invoke(init, Id, ct);
            var buffer = ReadSamples?.Invoke(args);

            if (buffer != null)
            {
                Pack = ProcessSamples?.Invoke(((Double[,], Object))buffer, args);

                if (TimeVSFrequency.RecordHistoricalData)
                {
                    if (Pack != null)
                    {
                        List<Double[]> data = new List<Double[]>();
                        for (Int32 i = 0; i < Pack.Buffer.GetLength(0); i++)
                        {
                            data.Add(new Double[Pack.Buffer.GetLength(1)]);
                            for (Int32 j = 0; j < Pack.Buffer.GetLength(1); j++)
                            {
                                data[i][j] = Pack.Buffer[i, j];
                            }
                        }
                        TimeVSFrequency.TFHistoryData.AddData(data);
                    }
                }

                if (_ResetLine || Sampling.Init)
                {
                    ProcessInit?.Invoke();
                    _ResetLine = false;
                    Sampling.Init = false;
                }

                PackNormal  = ProcessNormalSamples?.Invoke(((Double[,], Object))buffer, args);
                PackAverage = ProcessAverageSamples?.Invoke(((Double[,], Object))buffer, args);
                PackMaxHold = ProcessMaxHoldSamples?.Invoke(((Double[,], Object))buffer, args);
                PackMinHold = ProcessMinHoldSamples?.Invoke(((Double[,], Object))buffer, args);

                return true;
            }
            return false;
        }
    }
}
