namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Drawing;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public class PowerAnalysisPrsnt : MulticastPrsnt<IPwrAnalysisView>, IPwrAnalysisPrsnt
    {

        public static PowerAnalysisOpt LastSelect = PowerAnalysisOpt.PowerQuality;
        public readonly Lazy<PwrDifferPrsnt> DifferPrsnt;

        public readonly Lazy<PwrEfficiencyPrsnt> EfficiencyPrsnt;

        public readonly Lazy<PwrHarmonicPrsnt> HarmonicPrsnt;

        public readonly Lazy<PwrInrushCurrentPrsnt> InrushCurrentPrsnt;

        public readonly Lazy<PwrQualityPrsnt> QualityPrsnt;

        public readonly Lazy<PwrRipplePrsnt> RipplePrsnt;

        public readonly Lazy<PwrModulationPrsnt> ModulationPrsnt;

        public readonly Lazy<PwrSwitchingLossPrsnt> SwitchingLossPrsnt;

        public readonly Lazy<PwrTransientPrsnt> TransientPrsnt;

        public readonly Lazy<PwrSOAPrsnt> SOAPrsnt;

        public readonly Lazy<PwrLoopAnalysisPrsnt> LoopAnalysisPrsnt;

        public readonly Lazy<PwrRDSonPrsnt> RDSonPrsnt;

        public readonly Lazy<PwrPSRRPrsnt> PSRRPrsnt;

        public readonly Lazy<PwrOnOffTimePrsnt> OnOffTimePrsnt;

        public readonly Lazy<PwrSlewRatePrsnt> SlewRatePrsnt;

        public static Boolean TryAddPowerAnalysis(PowerAnalysisOpt type, out PowerAnalysisPrsnt? pwprsnt, ChannelId VoltageSrc = ChannelId.C1, ChannelId CurrentSrc = ChannelId.C2)
        {
            pwprsnt = null;
            if (FunctionLimit.PwrAnalysisFunctionLimit(DsoPrsnt.DefaultDsoPrsnt?.MutexFunctionFlag ?? false) == false)
            {
                return false;
            }

            var id = ChannelIdExt.GetPowers().Where(o => !DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.ContainsKey(o)).ToList().FirstOrDefault();
            if (id.IsPowers())
            {
                pwprsnt = new PowerAnalysisPrsnt(DsoPrsnt.DefaultDsoPrsnt, null, id);
                DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Add(pwprsnt.Id, pwprsnt);
                pwprsnt.BoundMeasPrsnt = DsoPrsnt.DefaultDsoPrsnt.Measure;

                pwprsnt.BoundMathPrsnt1 = GetPowerMathPrsnt();

                if (pwprsnt.BoundMathPrsnt1 == null)
                {
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Remove(pwprsnt.Id);
                    pwprsnt.Dispose();
                    pwprsnt = null;
                    return false;
                }

                if (type == PowerAnalysisOpt.PowerEfficency || type == PowerAnalysisOpt.SlewRate)//需要双数学通道
                {
                    pwprsnt.BoundMathPrsnt2 = GetPowerMathPrsnt();

                    if (pwprsnt.BoundMathPrsnt2 == null)
                    {
                        WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                        DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Remove(pwprsnt.Id);
                        pwprsnt.Dispose();
                        pwprsnt = null;
                        return false;
                    }
                }
            }
            else
            {
                WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                return false;
            }

            if (DsoPrsnt.DefaultDsoPrsnt.IsDemoMode())
            {
                if (TriggerPrsnt.Mode == TriggerMode.OneShot || TriggerPrsnt.Mode == TriggerMode.Normal)
                {
                    TriggerPrsnt.Mode = TriggerMode.Auto;
                }
                if (TriggerPrsnt.State == SysState.Stop)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Resume();
                }
            }
            pwprsnt.VoltageSrc1 = VoltageSrc;
            pwprsnt.CurrentSrc1 = CurrentSrc;
            pwprsnt.Mode = type;
            pwprsnt.Active = true;

            return true;
        }

        public PowerAnalysisPrsnt(IDsoPrsnt idp, IPwrAnalysisView? view = null, ChannelId id = ChannelId.POWER1) : base(idp)
        {
            Model = new(DsoModel.Default.Meas);
            DsoModel.Default.AddPowerChannel(id, Model);
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = (IBadge)this;

                TryAddView(view);
            }
            Id = id;
            QualityPrsnt = new Lazy<PwrQualityPrsnt>(() => new(Dso, id));
            HarmonicPrsnt = new Lazy<PwrHarmonicPrsnt>(() => new(Dso, id));
            EfficiencyPrsnt = new Lazy<PwrEfficiencyPrsnt>(() => new(Dso, id));
            RipplePrsnt = new Lazy<PwrRipplePrsnt>(() => new(Dso, id));
            DifferPrsnt = new Lazy<PwrDifferPrsnt>(() => new(Dso));
            InrushCurrentPrsnt = new Lazy<PwrInrushCurrentPrsnt>(() => new(Dso, id));
            SwitchingLossPrsnt = new Lazy<PwrSwitchingLossPrsnt>(() => new(Dso, id));
            TransientPrsnt = new Lazy<PwrTransientPrsnt>(() => new(Dso));
            ModulationPrsnt = new Lazy<PwrModulationPrsnt>(() => new(Dso, id));
            SOAPrsnt = new Lazy<PwrSOAPrsnt>(() => new(Dso, id));
            LoopAnalysisPrsnt = new Lazy<PwrLoopAnalysisPrsnt>(() => new(Dso, id));
            RDSonPrsnt = new Lazy<PwrRDSonPrsnt>(() => new(Dso, id));
            PSRRPrsnt = new Lazy<PwrPSRRPrsnt>(() => new(Dso, id));
            OnOffTimePrsnt = new Lazy<PwrOnOffTimePrsnt>(() => new(Dso, id));
            SlewRatePrsnt = new Lazy<PwrSlewRatePrsnt>(() => new(Dso, id));

            Model.InitHandler = Init;
        }


        internal static MathPrsnt? GetPowerMathPrsnt()
        {
            MathPrsnt? result = null;
            foreach (var item in ChannelIdExt.GetPowerAnalysisMaths())
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(item, out var prsnt))
                {
                    if (prsnt.Active == false &&
                        DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where((a) => { return a.Value.BoundMathPrsnt1?.Id == item || (a.Value.Mode == PowerAnalysisOpt.PowerEfficency && a.Value.BoundMathPrsnt2?.Id == item); }).Count() < 1)
                    {
                        result = (MathPrsnt)prsnt;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the Active.
        /// </summary>
        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_PowerAs && value)
                {
                    WeakTip.Default.Write("Power Analysis", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                if (!value)
                {
                    ((DsoPrsnt)Dso).PwrAnalysisDictionary.Remove(Id);
                }
                Model.Active = value;

                //var mch = (MathModel)DsoModel.Default.GetChannel(ChannelId.M1);

                //if (!Model.Active && mch.Args?.Occupier == Model)
                //{
                //    mch.Args.Occupier = null;
                //    mch.Label = "";
                //    mch.Active = false;
                //    mch.Formula = $"{nameof(MathType.Binary)}:C1+C2";
                //}
            }
        }

        public ChannelId CurrentSrc1 { get => Model.CurrentSrc1; set => Model.CurrentSrc1 = value; }

        public ChannelId VoltageSrc1 { get => Model.VoltageSrc1; set => Model.VoltageSrc1 = value; }

        public ChannelId CurrentSrc2 { get => Model.CurrentSrc2; set => Model.CurrentSrc2 = value; }

        public ChannelId VoltageSrc2 { get => Model.VoltageSrc2; set => Model.VoltageSrc2 = value; }
        public PowerAnalysisOpt Mode { get => Model.Mode; set => Model.Mode = value; }



        protected void Init()
        {
            if (Active)
            {
                switch (Mode)
                {
                    case PowerAnalysisOpt.PowerQuality:
                        QualityPrsnt.Value.TryShowPowerWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.Harmonic:
                        HarmonicPrsnt.Value.TryShowHarmonicWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.Ripple:
                        RipplePrsnt.Value.TryShowRippleWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.SwitchingLoss:
                        SwitchingLossPrsnt.Value.TryShowPowerWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.PowerEfficency:
                        EfficiencyPrsnt.Value.TryShowEfficiencyWfmPower1(BoundMathPrsnt1);
                        EfficiencyPrsnt.Value.TryShowEfficiencyWfmPower2(BoundMathPrsnt2);
                        break;
                    case PowerAnalysisOpt.InrushCurrent:
                        break;
                    case PowerAnalysisOpt.RDSon:
                        RDSonPrsnt.Value.TryShowRDSonWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.TurnOnOff:
                        //TurnOnOff.Run();
                        break;
                    case PowerAnalysisOpt.Transient:
                        TransientPrsnt.Value.TryShowTransientWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.Differ:
                        DifferPrsnt.Value.TryShowDifferWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.Modulation:
                        //TryShowModulationWfm(MathType.Histgram, ModulationType.Period);
                        //TryShowModulationWfm(MathType.Trend, ModulationType.Period);
                        break;
                    case PowerAnalysisOpt.SafeOperationArea:
                        SOAPrsnt.Value.TryShowSOAWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.LoopAnalysis:
                        LoopAnalysisPrsnt.Value.TryShowLoopAnalysisWfm(BoundMathPrsnt1);
                        break;
                    case PowerAnalysisOpt.PSRR:
                        break;
                    case PowerAnalysisOpt.SlewRate:
                        SlewRatePrsnt.Value.TryShowSlewRateWfmVoltageRate(BoundMathPrsnt1);
                        SlewRatePrsnt.Value.TryShowSlewRateWfmCurrentRate(BoundMathPrsnt2);
                        break;
                }
            }
            else
            {

            }
        }

        public MathPrsnt? BoundMathPrsnt1
        {
            get;
            set;
        }

        public MathPrsnt? BoundMathPrsnt2
        {
            get;
            set;
        }

        public MeasPrsnt? BoundMeasPrsnt
        {
            get;
            set;
        }


        private protected override PowerAnalysisModel Model { get; }

        public ChannelType Type { get; set; }

        public ChannelId Id { get; set; }

        public String Name { get; set; }

        public Color DrawColor { get; set; }
    }
}
