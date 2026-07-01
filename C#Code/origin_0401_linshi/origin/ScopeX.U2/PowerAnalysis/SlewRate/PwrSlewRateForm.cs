using ScopeX.U2;
using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class PwrSlewRateForm : FloatForm, IPwrAnalysisView
    {
        private PwrSlewRatePage _PwrSlewRatePage;

        public PwrSlewRateForm(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            Presenter = powerAnalysisPrsnt;
            _PwrSlewRatePage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                PowerPresenter = Presenter,
                Presenter = Presenter.SlewRatePrsnt.Value
            };
            Size = new(_PwrSlewRatePage.Size.Width, _PwrSlewRatePage.Size.Height + HeadHeight);
            _PwrSlewRatePage.Presenter.TryAddView(_PwrSlewRatePage);
            Controls.Add(_PwrSlewRatePage);
            Controls.SetChildIndex(_PwrSlewRatePage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PwrSlewRateForm)));
            };
        }

        public PowerAnalysisPrsnt Presenter { get; set; }

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
            _PwrSlewRatePage.UpdateView(prsnt, propertyName);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                TitleColor = AppStyleConfig.DefaultTitleForeColor;
                //HeadBackColor = Color.FromArgb(255, 77, 77, 77);
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _PwrSlewRatePage.Presenter.TryRemoveView(_PwrSlewRatePage);
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _PwrSlewRatePage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
