using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using static ScopeX.UserControls.SelectComboBox;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class PwrModulationPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrModulationPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxVoltageSrc.DataSource = dss;
            //selectTouch1.SelectValue = PowerPresenter.VoltageSrc;
            //selectTouch1.Text = PowerPresenter.VoltageSrc.ToString();

            CbxVoltageSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.VoltageSrc1 = (ChannelId)CbxVoltageSrc.SelectValue;
                }
            };

            CbxCurrentSrc.DataSource = dss;
            //selectTouch2.SelectValue = PowerPresenter.CurrentSrc;
            //selectTouch2.Text = PowerPresenter.CurrentSrc.ToString();

            CbxCurrentSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
                }
            };

            CbxHistogramSource.DataSource = Enum.GetValues<ModulationType>().
               Select(o => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(o.GetDescription()), o, null)).ToList();
            CbxHistogramSource.SelectedIndexChanged += CbxHistogramSource_SelectedIndexChanged;

            CbxTrendSource.DataSource = Enum.GetValues<ModulationType>().
               Select(o => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(o.GetDescription()), o, null)).ToList();
            CbxTrendSource.SelectedIndexChanged += CbxTrendSource_SelectedIndexChanged; ;
        }
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }
        public PwrModulationPrsnt Presenter
        {
            get;
            set;
        }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => Presenter;
            set
            {
                Presenter = (PwrModulationPrsnt)value;
            }
        }
        public PowerAnalysisOpt Mode => PowerAnalysisOpt.PowerQuality;

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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.SourceType):
                    SourceType.ChoosedButtonIndex = (Int32)Presenter.SourceType;
                    break;
                case nameof(PowerPresenter.Active):
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(PowerPresenter.VoltageSrc1):
                    //CbxVoltageSrc.SelectedIndex = (Int32)PowerPresenter.VoltageSrc;
                    CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                    break;
                case nameof(PowerPresenter.CurrentSrc1):
                    //CbxCurrentSrc.SelectedIndex = (Int32)PowerPresenter.CurrentSrc;
                    CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                    break;
                case nameof(Presenter.HistogramSource):
                    CbxHistogramSource.SelectIndex = (Int32)Presenter.HistogramSource;
                    break;
                case nameof(Presenter.TrendSource):
                    CbxTrendSource.SelectIndex = (Int32)Presenter.TrendSource;
                    break;
            }
            _ArgToCtrl = false;

        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                SourceType.ChoosedButtonIndex = (Int32)Presenter.SourceType;
                ChkActive.Checked = PowerPresenter.Active;
                //CbxVoltageSrc.SelectedIndex = (Int32)PowerPresenter.VoltageSrc;
                CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                //CbxCurrentSrc.SelectedIndex = (Int32)PowerPresenter.CurrentSrc;
                CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                CbxHistogramSource.SelectIndex = (Int32)Presenter.HistogramSource;
                CbxTrendSource.SelectIndex = (Int32)Presenter.TrendSource;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            Presenter.VuAddHistogram = AddHistogramUI;
            Presenter.VuAddTrend = AddTrendUI;
            ContorlTextRefresh();
            UpdateView();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void ContorlTextRefresh()
        {
            LblDisplay.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            ChkActive.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            ChkActive.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            LblVoltageSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            LblCurrentSrc.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            LblSourceSelection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinYuan");
            SourceType.ButtonItems[0].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaYuan");
            SourceType.ButtonItems[1].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuYuan");
            BtnGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XinHaoLianJieShiYi");
            BtnResultTable.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JieGuoBiao");
            //BtnPowerPic.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu");
            LblHistogram.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiFangTu");
            BtnHistogramAdd.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TianJia");
            LblTrend.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuShiTu");
            BtnTrendAdd.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TianJia");
        }

        private void AddTrendUI()
        {
            switch (Presenter.TrendSource)
            {
                case ModulationType.Period:
                    Presenter.MathId = ChannelId.M9;
                    Presenter.Items[ModulationType.Period].TrendActive = true;
                    if(!Presenter.ItemType.ContainsKey(ChannelId.M9))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M9, ModulationType.Period);
                    }
                   (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.Period);
                    break;
                case ModulationType.Frequency:
                    Presenter.MathId = ChannelId.M10;
                    Presenter.Items[ModulationType.Frequency].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M10))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M10, ModulationType.Frequency);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.Frequency);
                    break;
                case ModulationType.PDuty:
                    Presenter.MathId = ChannelId.M11;
                    Presenter.Items[ModulationType.PDuty].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M11))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M11, ModulationType.PDuty);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.PDuty);
                    break;
                case ModulationType.NDuty:
                    Presenter.MathId = ChannelId.M12;
                    Presenter.Items[ModulationType.NDuty].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M12))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M12, ModulationType.NDuty);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.NDuty);
                    break;
                case ModulationType.PWidth:
                    Presenter.MathId = ChannelId.M13;
                    Presenter.Items[ModulationType.PWidth].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M13))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M13, ModulationType.PWidth);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.PWidth);
                    break;
                case ModulationType.NWidth:
                    Presenter.MathId = ChannelId.M14;
                    Presenter.Items[ModulationType.NWidth].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M14))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M14, ModulationType.NWidth);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.NWidth);
                    break;
                case ModulationType.RiseTime:
                    Presenter.MathId = ChannelId.M15;
                    Presenter.Items[ModulationType.RiseTime].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M15))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M15, ModulationType.RiseTime);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.RiseTime);
                    break;
                case ModulationType.FallTime:
                    Presenter.MathId = ChannelId.M16;
                    Presenter.Items[ModulationType.FallTime].TrendActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M16))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M16, ModulationType.FallTime);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Trend, ModulationType.FallTime);
                    break;
                default:
                    break;
            }
        }

        private void AddHistogramUI()
        {
            switch (Presenter.HistogramSource)
            {
                case ModulationType.Period:
                    Presenter.MathId = ChannelId.M17;
                    Presenter.Items[ModulationType.Period].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M17))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M17, ModulationType.Period);
                    }
                   (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.Period);
                    break;
                case ModulationType.Frequency:
                    Presenter.MathId = ChannelId.M18;
                    Presenter.Items[ModulationType.Frequency].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M18))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M18, ModulationType.Frequency);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.Frequency);
                    break;
                case ModulationType.PDuty:
                    Presenter.MathId = ChannelId.M19;
                    Presenter.Items[ModulationType.PDuty].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M19))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M19, ModulationType.PDuty);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.PDuty);
                    break;
                case ModulationType.NDuty:
                    Presenter.MathId = ChannelId.M20;
                    Presenter.Items[ModulationType.NDuty].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M20))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M20, ModulationType.NDuty);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.NDuty);
                    break;
                case ModulationType.PWidth:
                    Presenter.MathId = ChannelId.M21;
                    Presenter.Items[ModulationType.PWidth].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M21))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M21, ModulationType.PWidth);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.PWidth);
                    break;
                case ModulationType.NWidth:
                    Presenter.MathId = ChannelId.M22;
                    Presenter.Items[ModulationType.NWidth].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M22))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M22, ModulationType.NWidth);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.NWidth);
                    break;
                case ModulationType.RiseTime:
                    Presenter.MathId = ChannelId.M23;
                    Presenter.Items[ModulationType.RiseTime].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M23))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M23, ModulationType.RiseTime);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.RiseTime);
                    break;
                case ModulationType.FallTime:
                    Presenter.MathId = ChannelId.M24;
                    Presenter.Items[ModulationType.FallTime].HistgramActive = true;
                    if (!Presenter.ItemType.ContainsKey(ChannelId.M24))
                    {
                        Presenter.ItemType.TryAdd(ChannelId.M24, ModulationType.FallTime);
                    }
                    (Program.Oscilloscope.View as DsoForm).TryAddPwrModulationUI(PowerPresenter, MathType.Histgram, ModulationType.FallTime);
                    break;
                default:
                    break;
            }
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowModulationGuideForm();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }

        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }

        private void SourceType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SourceType = (VIType)SourceType.ChoosedButtonIndex;
            }
        }

        private void CbxHistogramSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.HistogramSource = (ModulationType)CbxHistogramSource.SelectIndex;
            }
        }

        private void CbxTrendSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TrendSource = (ModulationType)CbxTrendSource.SelectIndex;
            }
        }

        private void BtnPowerPic_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }

        private void BtnHistogramAdd_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AddHistogramUI();
            }
        }

        private void BtnTrendAdd_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AddTrendUI();
            }
        }
    }
}
