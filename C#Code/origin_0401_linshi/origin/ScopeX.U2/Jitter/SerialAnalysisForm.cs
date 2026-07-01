// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.Jitter;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class SerialAnalysisForm : FloatForm, IJitterView
    {
        private IntegrityAnalysisPage _IntegrityAnalysisPage;

        private JitterPage _JitterPage;

        public SerialAnalysisForm(JitterPrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
            LoadOptionPage();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(SerialAnalysisForm)));
            };
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
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Presenter?.TryRemoveView(_JitterPage);
            Presenter?.TryRemoveView(_IntegrityAnalysisPage);
            Presenter?.TryRemoveView(this);
        }

        private void LoadOptionPage()
        {
            _IntegrityAnalysisPage = new()
            {
                Presenter = Presenter,
            };
            _IntegrityAnalysisPage.Presenter.TryAddView(_IntegrityAnalysisPage);

            _JitterPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Presenter = Presenter
            };
            _JitterPage.Presenter.TryAddView(_JitterPage);
            Size = new(_JitterPage.Size.Width, _JitterPage.Size.Height + HeadHeight);
            Controls.Add(_JitterPage);
            Controls.SetChildIndex(_JitterPage, 0);

        }

        public JitterPrsnt Presenter { get; set; }

        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_JitterPage.NeedPrsnt == true)
            {
                e.Cancel = true;
            }
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Presenter.TryRemoveView(this);
            Presenter.TryRemoveView(_IntegrityAnalysisPage);
            _JitterPage?.Dispose();
        }

        protected override void OnLoad(EventArgs e)
        {           
            Stylize();
            base.OnLoad(e);
        }
        private void Stylize()
        {
            _IntegrityAnalysisPage.StylizeFlag = true;
            _JitterPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (_JitterPage != null && _IntegrityAnalysisPage != null)
            {
                _JitterPage.UpdateView(prsnt, propertyName);
                _IntegrityAnalysisPage.UpdateView(prsnt, propertyName);
            }

        }

        //private void NbgSda_CurrentGroupIndexChanged(object sender, int previousIndex)
        //{
        //    var page = NbgSda.GroupItems[NbgSda.CurrentGroupIndex];
        //    Size = new(page.GroupSize.Width, page.GroupSize.Height + HeadHeight + (NbgSda.CurrentGroupNum * NbgSda.NavBarHeight));
        //}
    }
}
