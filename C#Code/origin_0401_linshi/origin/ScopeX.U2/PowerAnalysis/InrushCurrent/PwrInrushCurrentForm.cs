using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class PwrInrushCurrentForm : FloatForm, IPwrAnalysisView
    {
        private PwrInrushCurrentPage _PwrInrushCurrentPage;
        public PwrInrushCurrentForm(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            Presenter = powerAnalysisPrsnt;
            _PwrInrushCurrentPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                PowerPresenter = Presenter,
                Presenter = Presenter.InrushCurrentPrsnt.Value
            };
            _PwrInrushCurrentPage.Presenter.TryAddView(_PwrInrushCurrentPage);
            Size = new(_PwrInrushCurrentPage.Size.Width, _PwrInrushCurrentPage.Size.Height + HeadHeight);

            Controls.Add(_PwrInrushCurrentPage);
            Controls.SetChildIndex(_PwrInrushCurrentPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PwrInrushCurrentForm)));
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
            _PwrInrushCurrentPage.UpdateView(prsnt, propertyName);
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
            base.OnLoad(e);
            Stylize();
            UpdateView();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            //Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _PwrInrushCurrentPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }
    }
}
