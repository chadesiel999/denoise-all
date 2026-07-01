using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2.AWG
{
    public partial class AWGForm : FloatForm, IWfmGenView
    {
        private readonly AWGPage _AWGPage;

        public AWGForm()
        {
            InitializeComponent();

            _AWGPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };
            Size = new(_AWGPage.Size.Width, _AWGPage.Size.Height + HeadHeight);

            Controls.Add(_AWGPage);
            Controls.SetChildIndex(_AWGPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(AWGForm)));
            };
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

        public ArbWfmGenPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArbWfmGenPrsnt)value;
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
                return;
            }

            _AWGPage.UpdateView(prsnt, propertyName);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                TitleColor = AppStyleConfig.DefaultTitleForeColor;
                Title = Presenter.Name.Replace("AW", "");
                IndicatorColor = Presenter.DrawColor;
                ActiveBorderVisiable = true;
                ActiveBorderColor = Presenter.DrawColor;
                IsIndicatorShow = true;
                //HeadBackColor = Presenter.DrawColor;
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
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            Dispose();
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _AWGPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
