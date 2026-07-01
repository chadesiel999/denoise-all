// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/23</date>

namespace ScopeX.U2.PassFail
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class PassFailForm : FloatForm, IPassFailView
    {
        private readonly PassFailPage _PassFailFormPage;
        private readonly PassFailSavePage _PassFailSavePage;

        public PassFailForm()
        {
            InitializeComponent();

            _PassFailFormPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                StylizeFlag = true
            };
            _PassFailSavePage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                StylizeFlag = true
            };


            Size = new(_PassFailFormPage.Size.Width, _PassFailFormPage.Size.Height + HeadHeight + NbgAnalog.CurrentGroupNum * NbgAnalog.NavBarHeight);
            DefaultStyleManager.Instance.RegisterControlRecursion(_PassFailFormPage, StyleFlag.FontSize);
            NbgAnalog.SetGroupContent(0, _PassFailFormPage);
            NbgAnalog.SetGroupContent(1, _PassFailSavePage);

            //Controls.Add(_PassFailFormPage);
            //Controls.SetChildIndex(_PassFailFormPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PassFailForm)));
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

        public PassFailPrsnt Presenter { get; set; }

        IPassFailPrsnt Core.IView<IPassFailPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PassFailPrsnt)value;
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

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _PassFailFormPage.UpdateView(presenter, propertyName);
            _PassFailSavePage.UpdateView(presenter, propertyName);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            _PassFailFormPage.StylizeFlag = true;
            _PassFailSavePage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }
    }
}
