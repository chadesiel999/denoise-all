// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.U2.LanguageSupoort;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class CascadeSettingForm : FloatForm, ITriggerView
    {
        private readonly Control _EventPage;

        private readonly TriggerCascadePathSubPage _PathPage;
        private String _RecordKey = String.Empty;

        public CascadeSettingForm(ITriggerView itv, String name = "")
        {
            InitializeComponent();

            _EventPage = (Control)itv;
            _EventPage.BackColor = Color.Transparent;
            _EventPage.Dock = DockStyle.Fill;

            _PathPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Size = new(_EventPage.Size.Width, _EventPage.Size.Height + HeadHeight + NbgCascade.CurrentGroupNum * NbgCascade.NavBarHeight);

            NbgCascade.GroupItems[0].Title = name;
            NbgCascade.SetGroupContent(0, _EventPage);
            NbgCascade.SetGroupContent(1, _PathPage);
            _RecordKey = $"{this.Name}_{NbgCascade.Name}";
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(CascadeSettingForm)));
            };
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgCascade.CurrentGroupIndex;
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
            if (index > NbgCascade.CurrentGroupNum)
            {
                index = NbgCascade.CurrentGroupNum - 1;
            }
            NbgCascade.CurrentGroupIndex = index;
        }
        public TrigPathwayPrsnt PathPresenter
        {
            get;
            set;
        }

        public TriggerPrsnt Presenter
        {
            get;
            set;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TriggerPrsnt)value;
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

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            PathPresenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            SetGroupIndex();
            // LanguageFactory.CacheFormLanguageControls(this);
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
            (_EventPage as ITriggerView)?.UpdateView(prsnt, propertyName);

            _PathPage.UpdateView(prsnt, propertyName);
        }

        private void Stylize()
        {
            _PathPage.StylizeFlag = true;
            if (_EventPage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
            }
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

    }
}
