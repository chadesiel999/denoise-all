using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class DigitalForm : FloatForm, IChnlView
    {
        private readonly DigitalPage _DisplayPage;

        private readonly DigiThroldPage _ThroldPage;

        //private readonly GroupPage _GroupPage;

        private String _RecordKey = String.Empty;
        public DigitalForm()
        {
            InitializeComponent();

            _DisplayPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _ThroldPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Size = new(_DisplayPage.Size.Width + 4, _DisplayPage.Size.Height + HeadHeight + NbgDigital.CurrentGroupNum * NbgDigital.NavBarHeight + 4);

            NbgDigital.SetGroupContent(0, _DisplayPage);
            NbgDigital.SetGroupContent(1, _ThroldPage);
            //NbgDigital.SetGroupContent(2, _GroupPage);
            _RecordKey = $"{this.Name}_{NbgDigital.Name}";
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(DigitalForm)));
            };
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgDigital.CurrentGroupIndex;
            base.OnHandleDestroyed(e);
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
            if (index > NbgDigital.CurrentGroupNum)
            {
                index = NbgDigital.CurrentGroupNum - 1;
            }
            NbgDigital.CurrentGroupIndex = index;
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

        public DigitalPrsnt Presenter
        {
            get => _DisplayPage.Presenter;
            set => _DisplayPage.Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (DigitalPrsnt)value;
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

            _DisplayPage.UpdateView(prsnt, propertyName);
            _ThroldPage.UpdateView(prsnt, propertyName);
            //_GroupPage.UpdateView(prsnt, propertyName);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                //HeadBackColor = ControlPaint.Dark(Presenter.DrawColor, 0.1F);
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
            base.OnFormClosed(e);
            Presenter.TryRemoveView(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            SetGroupIndex();
        }

        private void Stylize()
        {
            _DisplayPage.StylizeFlag = true;
            _ThroldPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
