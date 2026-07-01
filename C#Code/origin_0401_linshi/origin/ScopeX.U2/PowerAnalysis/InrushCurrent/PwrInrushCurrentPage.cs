namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Protocol;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.UserControls;

    public partial class PwrInrushCurrentPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PwrInrushCurrentPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.InrushCurrent;

        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }

        public PwrInrushCurrentPrsnt Presenter
        {
            get;
            set;
        }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => Presenter;
            set
            {
                Presenter = (PwrInrushCurrentPrsnt)value;
            }
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
                case nameof(PowerPresenter.Active):
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(Presenter.PeakCurrent):
                    SetPeakCurrent();
                    break;
                case nameof(Presenter.RunFlag):
                    BtnSingleRun.Enabled = !Presenter.RunFlag;
                    break;

            }
            _ArgToCtrl = false;
        }


        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                InitView();
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            Presenter.VuSingleRun = SingleRun;
            this.Refresh();
        }

        private void InitView()
        {
            InitCurrent();
            InitComboxList();
            ChkActive.Checked = PowerPresenter.Active;
            CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
        }

        private void InitComboxList()
        {
            var chsrc = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new SelectComboBox.ComboBoxItem(x.GetDescription(), x, null)).ToList();
            CbxCurrentSrc.DataSource = chsrc;
            CbxCurrentSrc.SelectedIndexChanged -= CbxCurrentSrc_SelectedIndexChanged;
            CbxCurrentSrc.SelectedIndexChanged += CbxCurrentSrc_SelectedIndexChanged;
        }

        private void CbxCurrentSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
                SetPeakCurrent();
            }
        }

        private void InitCurrent()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnPeakCurrent);
            BtnPeakCurrent.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnPeakCurrent, nameof(Presenter.PeakCurrent), 0.02);
            };
            SetPeakCurrent();
        }

        private void SetPeakCurrent()
        {
            this.BtnPeakCurrent.Text = SIHelper.ValueChangeToSI(Presenter.PeakCurrent, 3, Presenter.Unit);
        }

        
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }
        
        private void BtnPeakCurrent_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {                
                Presenter.PeakCurrent = this.ShowNumberKeyboard(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FengZhiDianLiu"), Presenter.Unit, Presenter.PeakCurrent, Presenter.MaxPeakCurrent, Presenter.MinPeakCurrent);
            }
        }
        private void SingleRun()
        {
            if (!Presenter.RunFlag)
            {
                Presenter.RunFlag = true;
                BtnSingleRun.Enabled = false;
                if (TriggerPrsnt.Mode == TriggerMode.OneShot || TriggerPrsnt.Mode == TriggerMode.Normal)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTO);
                }
                else if (TriggerPrsnt.State == SysState.Stop)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.RUNSTOP);
                }
            }
        }
        private void BtnSingleRun_Click(object sender, EventArgs e)
        {
            if (!Presenter.RunFlag)
            {
                Presenter.RunFlag = true;
                BtnSingleRun.Enabled = false;
                if (TriggerPrsnt.Mode == TriggerMode.OneShot || TriggerPrsnt.Mode == TriggerMode.Normal)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTO);
                }
                else if (TriggerPrsnt.State == SysState.Stop)
                {
                    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.RUNSTOP);
                }
            }
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowInrushCurrentGuideForm();
        }
        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }
    }
}
