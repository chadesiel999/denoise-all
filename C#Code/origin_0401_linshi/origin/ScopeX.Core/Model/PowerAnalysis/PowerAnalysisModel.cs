namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using EventBus;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    internal class PowerAnalysisModel : INotifyPropertyChanged
    {
        public override String ToString()
        {
            return "Power Analysis";
        }

        public PowerAnalysisModel(MeasureModel mm)
        {
            Quality = new(this, mm);
            Harmonic = new(this, mm);
            Efficiency = new(this, mm);
            Ripple = new(this, mm);
            Differ = new(this, mm);
            InrushCurrent = new(this, mm);
            SwitchingLoss = new(this, mm);
            Transient = new(this, mm);
            Modulation = new(this, mm);
            SafeOpArea = new(this, mm);
            LoopAnalysis = new(this, mm);
            RDSon=new(this, mm);
            PSRR=new(this, mm);
            OnOffTime=new(this, mm);
            SlewRate=new(this, mm);
        }

        private PowerAnalysisOpt _Mode = PowerAnalysisOpt.PowerQuality;
        public PowerAnalysisOpt Mode
        {
            get => _Mode;
            set
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Active;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (value && !OptionsManager.Default.GetOptionAvailable(OptionType.Pwr))
                {
                    WeakTip.Default.Write("Pwr", MsgTipId.PurchaseOptions, duration: 4);
                    value = false;
                }

                if (_Active != value)
                {
                    _Active = value;
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }
        
        private ChannelId _VoltageSrc1 = ChannelId.C1;
        public ChannelId VoltageSrc1
        {
            get => _VoltageSrc1;
            set
            {
                if (_VoltageSrc1 != value)
                {
                    _VoltageSrc1 = value;
                    UpdateSource();
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _CurrentSrc1 = ChannelId.C2;
        public ChannelId CurrentSrc1
        {
            get => _CurrentSrc1;
            set
            {
                if (_CurrentSrc1 != value)
                {
                    _CurrentSrc1 = value;
                    UpdateSource();
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }


        private ChannelId _VoltageSrc2 = ChannelId.C3;
        public ChannelId VoltageSrc2
        {
            get => _VoltageSrc2;
            set
            {
                if (_VoltageSrc2 != value)
                {
                    _VoltageSrc2 = value;
                    UpdateSource();
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _CurrentSrc2 = ChannelId.C4;
        public ChannelId CurrentSrc2
        {
            get => _CurrentSrc2;
            set
            {
                if (_CurrentSrc2 != value)
                {
                    _CurrentSrc2 = value;
                    UpdateSource();
                    InitHandler?.Invoke();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateSource()
        {
            switch(Mode)
            {
                case PowerAnalysisOpt.SlewRate:
                    SlewRate.UpdateSource();
                    break;
                case PowerAnalysisOpt.PowerEfficency:
                    Efficiency.UpdateSource();
                    break;
                case PowerAnalysisOpt.RDSon:
                    RDSon.UpdateSource();
                    break;
            }
        }


        public readonly PwrQualityModel Quality;

        public readonly PwrHarmonicModel Harmonic;

        public readonly PwrEfficiencyModel Efficiency;

        public readonly PwrRippleModel Ripple;

        public readonly PwrDifferModel Differ;

        public readonly PwrInrushCurrentModel InrushCurrent;

        public readonly PwrSwitchingLossModel SwitchingLoss;

        public readonly PwrRDSonModel RDSon;

        public readonly PwrTransientModel Transient;

        public readonly PwrModulationModel Modulation;

        public readonly PwrSOAModel SafeOpArea;

        public readonly PwrLoopAnalysisModel LoopAnalysis;

        public readonly PwrPSRRModel PSRR;

        public readonly PwrOnOffTimeModel OnOffTime;

        public readonly PwrSlewRateModel SlewRate;

        public void Run(Boolean wfmTaken)
        {
            try
            {
                if ((wfmTaken || DsoPrsnt.DefaultDsoPrsnt.IsDemoMode()) && Active)
                {
                    switch (Mode)
                    {
                        case PowerAnalysisOpt.PowerQuality:
                            Quality.Run();
                            break;
                        case PowerAnalysisOpt.Harmonic:
                            Harmonic.Run();
                            break;
                        case PowerAnalysisOpt.Ripple:
                            Ripple.Run();
                            break;
                        case PowerAnalysisOpt.SwitchingLoss:
                            SwitchingLoss.Run();
                            break;
                        case PowerAnalysisOpt.PowerEfficency:
                            Efficiency.Run();
                            break;
                        case PowerAnalysisOpt.InrushCurrent:
                            InrushCurrent.Run();
                            break;
                        case PowerAnalysisOpt.RDSon:
                            RDSon.Run();
                            break;
                        case PowerAnalysisOpt.TurnOnOff:
                            OnOffTime.Run();
                            break;
                        case PowerAnalysisOpt.Transient:
                            Transient.Run();
                            break;
                        case PowerAnalysisOpt.Differ:
                            Differ.Run();
                            break;
                        case PowerAnalysisOpt.Modulation:
                            Modulation.Run();
                            break;
                        case PowerAnalysisOpt.SafeOperationArea:
                            SafeOpArea.Run();
                            break;
                        case PowerAnalysisOpt.LoopAnalysis:
                            LoopAnalysis.Run();
                            break;
                        case PowerAnalysisOpt.SlewRate:
                            SlewRate.Run();
                            break;
                        case PowerAnalysisOpt.PSRR:
                            PSRR.Run();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"{ex.Message}\n{ex.StackTrace}", LogLevel.Error));
            }
        }

        public void Reset(PowerAnalysisOpt pam)
        {
            switch (pam)
            {
                case PowerAnalysisOpt.PowerQuality:
                    Quality.Reset();
                    break;
                case PowerAnalysisOpt.Ripple:
                    Ripple.Reset();
                    break;
                case PowerAnalysisOpt.SwitchingLoss:
                    SwitchingLoss.Reset();
                    break;
                case PowerAnalysisOpt.Harmonic:
                    Harmonic.Reset();
                    break;
                case PowerAnalysisOpt.SafeOperationArea:
                    SafeOpArea.Reset();
                    break;
                case PowerAnalysisOpt.LoopAnalysis:
                    LoopAnalysis.Reset();
                    break;
                case PowerAnalysisOpt.Modulation:
                    Modulation.Reset();
                    break;
                case PowerAnalysisOpt.PowerEfficency:
                    Efficiency.Reset();
                    break;
                case PowerAnalysisOpt.InrushCurrent:
                    InrushCurrent.Reset();
                    break;
                case PowerAnalysisOpt.RDSon:
                    RDSon.Reset();
                    break;
                case PowerAnalysisOpt.TurnOnOff:
                    OnOffTime.Reset();
                    break;
                case PowerAnalysisOpt.SlewRate:
                    SlewRate.Reset();
                    break;
                case PowerAnalysisOpt.Transient:
                    break;
                case PowerAnalysisOpt.Differ:
                    Differ.Reset();
                    break;
            }
        }

        public Action? InitHandler
        {
            get;
            set;
        }


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
    }
}
