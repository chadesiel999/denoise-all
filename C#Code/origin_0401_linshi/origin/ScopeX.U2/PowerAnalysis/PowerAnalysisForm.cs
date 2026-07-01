using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class PowerAnalysisForm : FloatForm, IPwrAnalysisView
    {
        private Control _OptionPage;

        private Boolean _ArgToCtrl;

        public PowerAnalysisForm()
        {
            InitializeComponent();
            TlpOptions.RowStyles[1] = new RowStyle(SizeType.Absolute, 480);
            BtnAdd.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TianJia");
            LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            LblMode.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanFenXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanFenXi");
            InitComboxList();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PowerAnalysisForm)));
            };
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();
            CbxVoltageSrc.DataSource = dss;
            CbxVoltageSrc.SelectValue = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList()[0].Item2;
            CbxCurrentSrc.DataSource = dss;
            CbxCurrentSrc.SelectValue = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList()[1].Item2;
            CbxMode.DataSource = Enum.GetValues<PowerAnalysisOpt>().
               Where(o => o != PowerAnalysisOpt.Differ && o != PowerAnalysisOpt.Transient).
               Select(o => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(o.GetDescription()), o, null)).ToList();
            CbxMode.SelectedIndexChanged += CbxMode_SelectedIndexChanged;

            //var ds = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
            //    Select(x => new KeyValuePair<String, ChannelId>(x.GetDescription(), x)).ToList();
            //CbxVoltageSrc.DataSource = ds;
            //CbxVoltageSrc.DisplayMember = "Key";
            //CbxVoltageSrc.ValueMember = "Value";

            //CbxCurrentSrc.DataSource = ds;
            //CbxCurrentSrc.DisplayMember = "Key";
            //CbxCurrentSrc.ValueMember = "Value";
            //CbxCurrentSrc.SelectedIndex = 1;
            ////CbxMode.DataSource = Enum.GetValues<PowerAnalysisOpt>().
            ////    Where(o => o != PowerAnalysisOpt.TurnOnOff && o != PowerAnalysisOpt.DynamicRes && o != PowerAnalysisOpt.Transient).Select(o => new KeyValuePair<String, PowerAnalysisOpt>(o.GetDescription(), o)).ToList();
            //CbxMode.DataSource = Enum.GetValues<PowerAnalysisOpt>().
            //   Where(o => o == PowerAnalysisOpt.PowerQuality || o == PowerAnalysisOpt.Harmonic || o == PowerAnalysisOpt.SafeOperationArea
            //   || o == PowerAnalysisOpt.SwitchingLoss || o == PowerAnalysisOpt.Ripple || o == PowerAnalysisOpt.LoopAnalysis).
            //   Select(o => new KeyValuePair<String, PowerAnalysisOpt>(o.GetDescription(), o)).ToList();

            //CbxMode.DisplayMember = "Key";
            //CbxMode.ValueMember = "Value";
            //CbxMode.SelectedIndexChanged += CbxMode_SelectedIndexChanged;
        }

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private Control GetOptionPage(PowerAnalysisOpt pao)
        {
            return pao switch
            {
                PowerAnalysisOpt.PowerQuality => MakeQualityPage(),
                PowerAnalysisOpt.Harmonic => MakeHarmonicPage(),
                PowerAnalysisOpt.Ripple => MakeRipplePage(),
                PowerAnalysisOpt.Modulation => MakeModulationPage(),
                PowerAnalysisOpt.SwitchingLoss => MakeSwitchingLossPage(),
                PowerAnalysisOpt.SafeOperationArea => MakeSOAPage(),
                PowerAnalysisOpt.InrushCurrent => MakeInrushCurrentPage(),
                PowerAnalysisOpt.PowerEfficency => MakeEfficiencyPage(),
                PowerAnalysisOpt.RDSon => MakeRDSonPage(),
                PowerAnalysisOpt.TurnOnOff => MakeOnOffTimePage(),
                PowerAnalysisOpt.Differ => MakeDifferPage(),
                PowerAnalysisOpt.Transient => MakeTransientPage(),
                PowerAnalysisOpt.LoopAnalysis => MakeLoopAnalysisPage(),
                PowerAnalysisOpt.PSRR => MakePSRRPage(),
                PowerAnalysisOpt.SlewRate => MakeSlewRatePage(),
                _ => throw new NotImplementedException(),
            };

            Control MakeQualityPage()
            {
                var qp = new PowerQualityGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return qp;
            }

            Control MakeHarmonicPage()
            {
                var hp = new PwrHarmonicGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return hp;
            }

            Control MakeEfficiencyPage()
            {
                var ep = new PwrEfficiencyGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return ep;
            }

            Control MakeRipplePage()
            {
                var rp = new PwrRippleGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return rp;
            }

            Control MakeDifferPage()
            {
                var dp = new PwrDifferPage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return dp;
            }

            Control MakeInrushCurrentPage()
            {
                var ip = new PwrInrushCurrentGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0)
                };
                return ip;
            }

            Control MakeSwitchingLossPage()
            {
                var sp = new PwrSwitchingLossGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return sp;
            }

            Control MakeTransientPage()
            {
                var tp = new PwrTransientPage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return tp;
            }

            Control MakeModulationPage()
            {
                var mp = new PwrModulationGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return mp;
            }

            Control MakeSOAPage()
            {
                var mp = new PwrSOAGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return mp;
            }
            Control MakeLoopAnalysisPage()
            {
                var lp = new PwrLoopAnalysisGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return lp;
            }

            Control MakeRDSonPage()
            {
                var lp = new PwrRDSonGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return lp;
            }

            Control MakePSRRPage()
            {
                var lp = new PwrPSRRGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return lp;
            }

            Control MakeOnOffTimePage()
            {
                var tp = new PwrOnOffTimeGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return tp;
            }

            Control MakeSlewRatePage()
            {
                var tp = new PwrSlewRateGuidePage
                {
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return tp;
            }
        }

        private void ChangeOptionPage(PowerAnalysisOpt pao)
        {
            TlpOptions.Controls.Remove(_OptionPage);
            _OptionPage.Dispose();
            _OptionPage = GetOptionPage(pao);

            TlpOptions.Controls.Add(_OptionPage, 0, 1);

            ChangeInputOptState(pao);

            DefaultStyleManager.Instance.RegisterControl(_OptionPage);
        }

        private void LoadOptionPage()
        {
            CbxMode.SelectValue = Enum.GetValues<PowerAnalysisOpt>().
               Where(o => o == PowerAnalysisPrsnt.LastSelect).
               Select(o => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(o.GetDescription()), o, null)).ToList()[0].Item2;
            _OptionPage = GetOptionPage((PowerAnalysisOpt)CbxMode.SelectValue);

            TlpOptions.Controls.Add(_OptionPage, 0, 1);

            ChangeInputOptState((PowerAnalysisOpt)CbxMode.SelectValue);
        }

        private void ChangeInputOptState(PowerAnalysisOpt pao)
        {
            if(pao == PowerAnalysisOpt.PowerEfficency)
            {
                LblVoltageSrc.Visible = LblCurrentSrc.Visible = CbxCurrentSrc.Visible = CbxVoltageSrc.Visible = false;
            }
            else if(pao == PowerAnalysisOpt.Ripple)
            {
                LblVoltageSrc.Visible = CbxVoltageSrc.Visible = true;
                LblCurrentSrc.Visible = CbxCurrentSrc.Visible = false;
            }
            else if (pao == PowerAnalysisOpt.InrushCurrent)
            {
                LblVoltageSrc.Visible = CbxVoltageSrc.Visible = false;
                LblCurrentSrc.Visible = CbxCurrentSrc.Visible = true;
            }
            else
            {
                LblVoltageSrc.Visible = LblCurrentSrc.Visible = CbxCurrentSrc.Visible = CbxVoltageSrc.Visible = true;
            }
        }

        public static Control InfoControl
        {
            get;
            set;
        }

        private void ShowTypeForm(PowerAnalysisOpt pao)
        {
            switch (pao)
            {
                case PowerAnalysisOpt.PowerQuality:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PowerQualityInfo pqinfo = new()
                        {
                            Name = "PowerQualityInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        Presenter.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Harmonic:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrHarmonicInfo pqinfo = new()
                        {
                            Name = "PwrHarmonicInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "PwrHarmonicNum",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("PwrHarmonicNum"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddPwrHarmonicUI(Presenter);
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Ripple:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrRippleInfo pqinfo = new()
                        {
                            Name = "PwrRippleInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Modulation:
                    break;
                case PowerAnalysisOpt.SwitchingLoss:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrSwitchingLossInfo pqinfo = new()
                        {
                            Name = "PwrSwitchingLossInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        Presenter.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.SafeOperationArea:
                    {
                        if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                        {
                            PwrSOAInfo pqinfo = new()
                            {
                                Name = "PwrSOAInfo",
                                NameWidth = 150,
                                Text = p.Id.ToString(),
                                Dock = DockStyle.None,
                                Presenter = p,
                                Row1LangKey = "PwrVoltageSource",
                                Row2LangKey = "PwrCurrentSource",
                                Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                                Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            };
                            return pqinfo;
                        }))
                        {
                            (Program.Oscilloscope.View as DsoForm).TryAddSOAUI(Presenter);
                            return;
                        }
                        WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    }
                    break;
                case PowerAnalysisOpt.LoopAnalysis:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrLoopAnalysisInfo pqinfo = new()
                        {
                            Name = "PwrLoopAnalysisInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "LoopAnalysisInputSource",
                            Row2LangKey = "LoopAnalysisOutputSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("LoopAnalysisInputSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("LoopAnalysisOutputSource"),
                            //Row3rdName = LanguageHelper.GetPowerAnalysisString("PwrHarmonicNum"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddLoopAnalysisUI(Presenter);
                        //PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.InrushCurrent:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrInrushCurrentInfo pqinfo = new()
                        {
                            Name = "PwrInrushCurrentInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        Presenter.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.PowerEfficency:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrEfficiencyInfo pqinfo = new()
                        {
                            Name = "PowerEfficiencyInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.RDSon:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrRDSonInfo pqinfo = new()
                        {
                            Name = "PwrRDSonInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        Presenter.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.TurnOnOff: break;
                case PowerAnalysisOpt.Differ:
                    break;
                case PowerAnalysisOpt.Transient:
                    break;
                case PowerAnalysisOpt.PSRR:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(Presenter, (p) =>
                    {
                        PwrPSRRInfo pqinfo = new()
                        {
                            Name = "PwrPSRRInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PSRRInputSource",
                            Row2LangKey = "PSRROutputSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PSRRInputSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PSRROutputSource"),
                            //Row3rdName = LanguageHelper.GetPowerAnalysisString("PwrHarmonicNum"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(Presenter);
                        //PowerAnalysisApp.Default.ShowDataTableForm(Presenter);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
            };
        }

        public PowerAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => (IBadge)Presenter;
            set => Presenter = (PowerAnalysisPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    break;
                case nameof(Presenter.Mode):
                    //CbxMode.SelectedValue = Presenter.Mode;
                    CbxMode.SelectValue = Presenter.Mode;
                    ChangeOptionPage(Presenter.Mode);
                    break;
                case nameof(Presenter.VoltageSrc1):
                    //CbxVoltageSrc.SelectedIndex = (Int32)Presenter.VoltageSrc;
                    CbxVoltageSrc.SelectValue = Presenter.VoltageSrc1;
                    break;
                case nameof(Presenter.CurrentSrc1):
                    //CbxCurrentSrc.SelectedIndex = (Int32)Presenter.CurrentSrc;
                    CbxCurrentSrc.SelectValue = Presenter.CurrentSrc1;
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                //ChkActive.Checked = Presenter.Active;
                //CbxMode.SelectedValue = Presenter.Mode;
                CbxMode.SelectValue = Presenter.Mode;
                //CbxVoltageSrc.SelectedIndex = (Int32)Presenter.VoltageSrc;
                CbxVoltageSrc.SelectValue = Presenter.VoltageSrc1;
                //CbxCurrentSrc.SelectedIndex = (Int32)Presenter.CurrentSrc;
                CbxCurrentSrc.SelectValue = Presenter.CurrentSrc1;
                _ArgToCtrl = false;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadOptionPage();
            base.OnLoad(e);

            Stylize();
        }

        private void Stylize()
        {
            //DefaultStyleManager.Instance.RegisterControl(this, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControl(LblMode, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControl(LblVoltageSrc, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControl(LblCurrentSrc, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControl(BtnAdd, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {

            //if (!DsoPrsnt.DefaultDsoPrsnt.CheckLAMutex(true))
            //    return;

            //var id = ChannelIdExt.GetPowers().Where(o => !Program.Oscilloscope.PwrAnalysisDictionary.ContainsKey(o)).ToList().FirstOrDefault();
            //if (id.IsPowers())
            //{
            //    Presenter = new PowerAnalysisPrsnt(Program.Oscilloscope, null, id);
            //    Program.Oscilloscope.PwrAnalysisDictionary.Add(Presenter.Id, Presenter);
            //    Presenter.BoundMeasPrsnt = Program.Oscilloscope.Measure;
            //    foreach (var item in ChannelIdExt.GetPowerAnalysisMaths())
            //    {
            //        if (Program.Oscilloscope.TryGetChannel(item, out var prsnt))
            //        {
            //            if (prsnt.Active == false &&
            //                Program.Oscilloscope.PwrAnalysisDictionary.Where((a) => { return a.Value.BoundMathPrsnt?.Id == item; }).Count() < 1)
            //            {
            //                Presenter.BoundMathPrsnt = (MathPrsnt)prsnt;
            //                break;
            //            }
            //        }
            //    }
            //    if (Presenter.BoundMathPrsnt == null)
            //    {
            //        WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
            //        Program.Oscilloscope.PwrAnalysisDictionary.Remove(Presenter.Id);
            //        Presenter.Dispose();
            //        Presenter = null;
            //        return;
            //    }

            //    Program.Oscilloscope.Timebase.LimitScan(MsgTipId.PowerAnalysisIsNotSupportedInScan);
            //}
            //else
            //{
            //    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
            //    return;
            //}
            //Presenter.VoltageSrc = (ChannelId)CbxVoltageSrc.SelectValue;
            //Presenter.CurrentSrc = (ChannelId)CbxCurrentSrc.SelectValue;
            //Presenter.Mode = (PowerAnalysisOpt)CbxMode.SelectValue;
            //Presenter.Active = true;

            if (PowerAnalysisPrsnt.TryAddPowerAnalysis((PowerAnalysisOpt)CbxMode.SelectValue, out var paprsnt, (ChannelId)CbxVoltageSrc.SelectValue, (ChannelId)CbxCurrentSrc.SelectValue))
            {
                Presenter = paprsnt;
                if (PowerAnalysisPrsnt.LastSelect != (PowerAnalysisOpt)CbxMode.SelectValue)
                {
                    PowerAnalysisPrsnt.LastSelect = (PowerAnalysisOpt)CbxMode.SelectValue;
                }

            }
        }

        private Boolean FunctionLimit()
        {
            if (Program.Oscilloscope.Jitter.Active)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInJitter, MessageType.Asking))
                {
                    Program.Oscilloscope.Jitter.Active = false;
                }
                else
                {
                    return false;
                }
            }
            if (Program.Oscilloscope.PassFail.Active)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInPassFail, MessageType.Asking))
                {
                    Program.Oscilloscope.PassFail.Active = false;
                }
                else
                {
                    return false;
                }
            }

            var decodelist = Program.Oscilloscope.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInDecode, MessageType.Asking))
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (Program.Oscilloscope.Timebase.SegmentActive)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.PowerAnalysisIsNotSupportedInSegment, MessageType.Asking))
                {
                    Program.Oscilloscope.Timebase.SegmentActive = false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private void CbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_OptionPage != null)
            {
                TlpOptions.Controls.Remove(_OptionPage);
                _OptionPage.Dispose();
            }

            var select = (PowerAnalysisOpt)CbxMode.SelectValue;

            _OptionPage = GetOptionPage(select);
            _OptionPage.Dock = DockStyle.Fill;
            TlpOptions.Controls.Add(_OptionPage, 0, 1);

            if (select == PowerAnalysisOpt.LoopAnalysis || select == PowerAnalysisOpt.TurnOnOff || select == PowerAnalysisOpt.PSRR)
            {
                LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuYuan");// ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
                LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuYuan");// ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            }
            else
            {
                LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
                LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            }

            ChangeInputOptState(select);
        }
    }
}
