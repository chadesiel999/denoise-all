using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PwrSOAForm : FloatForm, IPwrAnalysisView
    {
        private PwrSOAPage _PwrSOAPageEx;
        public PwrSOAForm(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            InitializeComponent();
            Presenter = powerAnalysisPrsnt;
            _PwrSOAPageEx = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                PowerPresenter = Presenter,
                Presenter = Presenter.SOAPrsnt.Value
            };
            Size = new(_PwrSOAPageEx.Size.Width, _PwrSOAPageEx.Size.Height + HeadHeight);
            _PwrSOAPageEx.Presenter.TryAddView(_PwrSOAPageEx);
            Controls.Add(_PwrSOAPageEx);
            Controls.SetChildIndex(_PwrSOAPageEx, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PwrSOAForm)));
            };
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Power1_AnQuanGongZuoQu_");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Power1_AnQuanGongZuoQu_");
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
            _PwrSOAPageEx.UpdateView(prsnt, propertyName);
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
            _PwrSOAPageEx.Presenter.TryAddView(_PwrSOAPageEx);
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _PwrSOAPageEx.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
