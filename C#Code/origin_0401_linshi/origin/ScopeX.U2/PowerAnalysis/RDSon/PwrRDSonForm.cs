using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class PwrRDSonForm : FloatForm, IPwrAnalysisView
    {
        private PwrRDSonPage _PwrRDSonPage;
        public PwrRDSonForm(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            Presenter = powerAnalysisPrsnt;
            _PwrRDSonPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                PowerPresenter = Presenter,
                Presenter = Presenter.RDSonPrsnt.Value
            };
            Size = new(_PwrRDSonPage.Size.Width, _PwrRDSonPage.Size.Height + HeadHeight);
            _PwrRDSonPage.Presenter.TryAddView(_PwrRDSonPage);
            Controls.Add(_PwrRDSonPage);
            Controls.SetChildIndex(_PwrRDSonPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PwrRDSonForm)));
            };
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Power1_XieBoFenXi_");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Power1_XieBoFenXi_");
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
            _PwrRDSonPage.UpdateView(prsnt, propertyName);
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
            _PwrRDSonPage.Presenter.TryAddView(_PwrRDSonPage);
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _PwrRDSonPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
    }
}
