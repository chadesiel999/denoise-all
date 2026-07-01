// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class NormalViewSettingForm : FloatForm, IDisplayView
    {
        private readonly NormalViewSettingPage _SettingPage;

        public NormalViewSettingForm(Boolean visible = false)
        {
            InitializeComponent();

            _SettingPage = new(visible)
            {
                BackColor = System.Drawing.Color.Transparent,
                Dock = DockStyle.Fill
            };
            Size = new(_SettingPage.Size.Width, _SettingPage.Size.Height + HeadHeight);

            Controls.Add(_SettingPage);
            Controls.SetChildIndex(_SettingPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(NormalViewSettingForm)));
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

        public DisplayPrsnt Presenter { get; set; }

        IDisplayPrsnt IView<IDisplayPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (DisplayPrsnt)value;
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
            IsShowHelp = false;
            _SettingPage.StylizeFlag = true;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="presenter">The presenter<see cref="Object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="String"/>.</param>
        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _SettingPage.UpdateView(presenter, propertyName);
        }
    }
}
