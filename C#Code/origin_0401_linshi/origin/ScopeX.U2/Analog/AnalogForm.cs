// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/23</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.U2.LanguageSupoort;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class AnalogForm : FloatForm, IChnlView
    {
        //private readonly OtherPage _OtherPage;

        private readonly ProbePage _ProbePage;

        private readonly VerticalPage _VertPage;

        private String _RecordKey = String.Empty;

        public AnalogForm()
        {
            InitializeComponent();

            _VertPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            //_ProbePage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //};
            //_OtherPage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //};
            //First set Size property, then add control
            Size = new(_VertPage.Size.Width, _VertPage.Size.Height + HeadHeight + NbgAnalog.CurrentGroupNum * NbgAnalog.NavBarHeight);

            NbgAnalog.SetGroupContent(0, _VertPage);
            //NbgAnalog.SetGroupContent(1, _ProbePage);
            //NbgAnalog.SetGroupContent(2, _OtherPage);


            
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(AnalogForm)));
            };
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgAnalog.CurrentGroupIndex;
            base.OnHandleDestroyed(e);
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

        public AnalogPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (AnalogPrsnt)value;
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

            _VertPage.UpdateView(prsnt, propertyName);
            //_ProbePage.UpdateView(prsnt, propertyName);
            //_OtherPage.UpdateView(prsnt, propertyName);
        }

        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                TitleColor = AppStyleConfig.DefaultTitleForeColor;
                Title = Presenter.Name;
                //this.HeadBackColor = Presenter.DrawColor.GetBrightnessColor(-0.2);
                IndicatorColor = Presenter.DrawColor.GetBrightnessColor(-0.2);
                ActiveBorderColor = Presenter.DrawColor.GetBrightnessColor(-0.2);
                IsIndicatorShow = true;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Font != null)
            {
                Font = null;
            }
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _RecordKey = $"{Presenter.Id}_{this.Name}_{NbgAnalog.Name}";
            Stylize();
            UpdateView();
            SetGroupIndex();
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        private void SetGroupIndex()
        {
           var index = -1;
            if (index < 0)
            {
                if (!DsoPrsnt.NavBarGroupRecords.ContainsKey(_RecordKey))
                {
                    DsoPrsnt.NavBarGroupRecords.AddOrUpdate(_RecordKey, 0, (k, v) => 0);
                }
                index = DsoPrsnt.NavBarGroupRecords[_RecordKey];
            }
            if (index > NbgAnalog.CurrentGroupNum)
            {
                index = NbgAnalog.CurrentGroupNum - 1;
            }
            NbgAnalog.CurrentGroupIndex = index;
        }
        private void Stylize()
        {
            //_OtherPage.StylizeFlag = true;
            //_ProbePage.StylizeFlag = true;
            _VertPage.StylizeFlag = true;
            IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            IconSideDistance = 2;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }
    }
}
