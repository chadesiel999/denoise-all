// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/1</date>

namespace ScopeX.U2
{
    using EventBus;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.Controls.Common.Structs;
    using System.Threading;

    public class PowerAnalysisApp
    {
        public PowerAnalysisApp()
        {

        }

        public static PowerAnalysisApp Default { get; internal set; }
        private ConcurrentDictionary<ChannelId, Form> _PwrInfoFormDictionary = new();

        //public PowerAnalysisForm MakeForm(PowerAnalysisPrsnt powerAnalysisPrsnt)
        //{
        //    var paf = new PowerAnalysisForm()
        //    {
        //        Presenter = powerAnalysisPrsnt,

        //        Anchor = AnchorStyles.Top | AnchorStyles.Right,
        //    };
        //    paf.Presenter.TryAddView(paf);
        //    powerAnalysisPrsnt.BoundMeasPrsnt = Program.Oscilloscope.Measure;
        //    if (Program.Oscilloscope.TryGetChannel(ComModel.ChannelId.M1, out var prsnt))
        //    {
        //        powerAnalysisPrsnt.BoundMathPrsnt = (MathPrsnt)prsnt;
        //    }
        //    return paf;
        //}
        public PowerAnalysisForm MakeForm()
        {
            var paf = new PowerAnalysisForm()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            return paf;
        }
        public void ShowDataTableForm(PowerAnalysisPrsnt prsnt)
        {
            if (_PwrInfoFormDictionary.ContainsKey(prsnt.Id) == false)
            {
                var form = TryMakeDataTableForm(prsnt);
                if (form is not null)
                {
                    _PwrInfoFormDictionary.TryAdd(prsnt.Id, form);
                    form.FormClosed += (a, b) =>
                    {
                        _PwrInfoFormDictionary.TryRemove(prsnt.Id, out _);
                    };
                    EventBus.EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = form, Type = FormType.InfoForm });

                    //CreatDataTableFif(form);
                }
            }
            else
            {
                Form form = _PwrInfoFormDictionary[prsnt.Id];
                if (form is PwrQualityInfoForm qform)
                {
                    qform.TableForm?.Activate();
                }
            }
        }

        private static void CreatDataTableFif(Form form)
        {
            Thread.Sleep(50);
            var datatablefig = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(form, true);
            if (form is PwrQualityInfoForm qform)
            {
                qform.TableForm = datatablefig;
            }
            else if (form is PwrHarmonicInfoForm hform)
            {
                hform.TableForm = datatablefig;
            }
            else if (form is PwrRippleInfoForm rform)
            {
                rform.TableForm = datatablefig;
            }
            else if (form is PwrPSRRInfoForm lform)
            {
                lform.TableForm = datatablefig;
            }
            else if (form is PwrSwitchingLossInfoForm sform)
            {
                sform.TableForm = datatablefig;
            }
            else if (form is PwrInrushCurrentInfoForm iform)
            {
                iform.TableForm = datatablefig;
            }
            else if(form is PwrRDSonInfoForm rdsform)
            {
                rdsform.TableForm = datatablefig;
            }
            else if(form is PwrOnOffTimeInfoForm ootform)
            {
                ootform.TableForm = datatablefig;
            }
            else if(form is PwrSlewRateInfoForm srform)
            {
                srform.TableForm = datatablefig;
            }
        }

        private Form TryMakeDataTableForm(PowerAnalysisPrsnt prsnt)
        {
            switch (prsnt.Mode)
            {
                case PowerAnalysisOpt.PowerQuality:
                    return new PwrQualityInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.Harmonic:
                    return new PwrHarmonicInfoForm(prsnt, prsnt.HarmonicPrsnt.Value)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.Ripple:
                    return new PwrRippleInfoForm(prsnt, prsnt.RipplePrsnt.Value)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.LoopAnalysis:
                    return new PwrLoopAnalysisInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.Modulation:
                    return new PwrModulationInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                    break;
                case PowerAnalysisOpt.SwitchingLoss:
                    return new PwrSwitchingLossInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.SafeOperationArea:
                    break;
                case PowerAnalysisOpt.InrushCurrent:
                    return new PwrInrushCurrentInfoForm(prsnt, prsnt.InrushCurrentPrsnt.Value)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.PowerEfficency:
                    return new PwrEfficiencyInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.Differ:
                    break;
                case PowerAnalysisOpt.Transient:
                    break;
                case PowerAnalysisOpt.RDSon:
                    return new PwrRDSonInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.TurnOnOff:
                    return new PwrOnOffTimeInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.PSRR:
                    return new PwrPSRRInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                case PowerAnalysisOpt.SlewRate:
                    return new PwrSlewRateInfoForm(prsnt)
                    {
                        Anchor = AnchorStyles.Top,
                        Location = new(100, 100),
                        Text = prsnt.Id.ToString() + "- " + prsnt.Mode.GetDescription_Lang()
                    };
                default:
                    break;
            }
            return null;
        }

        public void CloseDataTableForm(PowerAnalysisPrsnt prsnt)
        {
            if (_PwrInfoFormDictionary.ContainsKey(prsnt.Id))
            {
                _PwrInfoFormDictionary[prsnt.Id]?.Close();
            }
        }
        //public Form TryMakeDifferInfoForm()
        //{            
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.Differ))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var pdif = new PwrDifferInfoForm(Presenter, Presenter.DifferPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = pdif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.Differ);

        //        return pdif;
        //    }
        //    return null;
        //}

        //public Form TryMakeEfficiencyInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.PowerEfficency))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var peif = new PwrEfficiencyInfoForm(Presenter, Presenter.EfficiencyPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = peif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.PowerEfficency);

        //        return peif;
        //    }
        //    return null;
        //}

        //public Form TryMakeHarmonicInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.Harmonic))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var phif = new PwrHarmonicInfoForm(Presenter, Presenter.HarmonicPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = phif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.Harmonic);

        //        return phif;
        //    }
        //    return null;
        //}

        //public Form TryMakeInrushInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.InrushCurrent))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var piif = new PwrInrushCurrentInfoForm(Presenter)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = piif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.InrushCurrent);

        //        return piif;
        //    }
        //    return null;
        //}

        //public Form TryMakeQualityInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.PowerQuality))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var pqif = new PwrQualityInfoForm(Presenter, Presenter.QualityPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = pqif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.PowerQuality);

        //        return pqif;
        //    }
        //    return null;
        //}

        //public Form TryMakeRippleInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.Ripple))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var prif = new PwrRippleInfoForm(Presenter, Presenter.RipplePrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = prif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.Ripple);

        //        return prif;
        //    }
        //    return null;
        //}

        //public Form TryMakeSwitchingLossInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.SwitchingLoss))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var psif = new PwrSwitchingLossInfoForm(Presenter, Presenter.SwitchingLossPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = psif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.SwitchingLoss);

        //        return psif;
        //    }
        //    return null;
        //}

        //public Form TryMakeTransientInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.Transient))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var ptif = new PwrTransientInfoForm(Presenter)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = ptif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.Transient);

        //        return ptif;
        //    }
        //    return null;
        //}

        //public Form TryMakeModulationInfoForm()
        //{
        //    //if (InfoControl is not PwrModulationInfoForm)
        //    //{
        //    //    InfoControl?.Close();

        //    //    var pmif = new PwrModulationInfoForm(Presenter.ModulationPrsnt.Value)
        //    //    {
        //    //        Anchor = AnchorStyles.Top,
        //    //        Location = new(100, 100),
        //    //    };

        //    //    InfoControl = pmif;
        //    //}

        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.Modulation))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var pmif = new PwrModulationInfoForm(Presenter)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = pmif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.Modulation);

        //        return pmif;
        //    }
        //    return null;
        //}

        //public Form TryMakeSOAInfoForm()
        //{
        //    if ((String)InfoControl?.Tag != nameof(PowerAnalysisOpt.SafeOperationArea))
        //    {
        //        InfoControl?.FindForm()?.Close();

        //        var psoaif = new PwrSOAInfoForm(Presenter, Presenter.SOAPrsnt.Value)
        //        {
        //            Anchor = AnchorStyles.Top,
        //            Location = new(100, 100),
        //        };

        //        InfoControl = psoaif.GetDataView;
        //        InfoControl.Tag = nameof(PowerAnalysisOpt.SafeOperationArea);

        //        return psoaif;
        //    }
        //    return null;
        //}

        public static void TryShowDifferGuideForm() => new PwrDifferGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowEfficiencyGuideForm() => new PwrEfficiencyGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowHarmonicGuideForm() => new PwrHarmonicGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowLoopAnalysisGuideForm() => new PwrLoopAnalysisGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowInrushCurrentGuideForm() => new PwrInrushCurrentGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowRDSonGuideForm() => new PwrRDSonGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowPSRRGuideForm() => new PwrPSRRGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();
        public static void TryShowQualityGuideForm() => new PwrQualityGuideForm()
        {
            Location = new System.Drawing.Point(660, 100),
            StartPosition = FormStartPosition.Manual,
        }.ShowDialogByEvent();

        public static void TryShowRippleGuideForm() => new PwrRippleGuideForm()
        {
            Location = new System.Drawing.Point(660, 100),
            StartPosition = FormStartPosition.Manual,
        }.ShowDialogByEvent();

        public static void TryShowSwitchingLossGuideForm() => new PwrSwitchingLossGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowModulationGuideForm() => new PwrModulationGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowOnOffTimeGuideForm() => new PwrOnOffTimeGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowTransientGuideForm() => new PwrTransientGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowSOAGuideForm() => new PwrSOAGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();

        public static void TryShowSlewRateGuideForm() => new PwrSlewRateGuideForm()
        {
            StartPosition = FormStartPosition.CenterScreen,
        }.ShowDialogByEvent();
    }
}
