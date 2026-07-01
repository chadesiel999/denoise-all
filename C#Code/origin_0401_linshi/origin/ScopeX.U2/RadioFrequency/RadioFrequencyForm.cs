using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class RadioFrequencyForm : FloatForm, IChnlView
    {
        private readonly RadioFrequencyPage _RadioFrequencyPage;

        private readonly HorizontalPage _HoriPage;
        private readonly OtherWfmPage _LinePage;
        private readonly FrequencyVSTimePage _FrequencyVSTimePage;

        private String _RecordKey = String.Empty;
        public RadioFrequencyForm()
        {
            InitializeComponent();

            _RadioFrequencyPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _HoriPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _LinePage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _FrequencyVSTimePage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Size = new(_RadioFrequencyPage.Size.Width, _RadioFrequencyPage.Size.Height + HeadHeight + (NbgRadio.CurrentGroupNum * NbgRadio.NavBarHeight));

            NbgRadio.SetGroupContent(0, _RadioFrequencyPage);
            NbgRadio.SetGroupContent(1, _HoriPage);
            NbgRadio.SetGroupContent(2, _LinePage);
            NbgRadio.SetGroupContent(3, _FrequencyVSTimePage);
            _RecordKey = $"{this.Name}_{NbgRadio.Name}";

        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgRadio.CurrentGroupIndex;
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
            if (index > NbgRadio.CurrentGroupNum)
            {
                index = NbgRadio.CurrentGroupNum - 1;
            }
            NbgRadio.CurrentGroupIndex = index;
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

        public RadioFrequencyPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (RadioFrequencyPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { propertyName });
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
                UpdateView();
                return;
            }

            _RadioFrequencyPage.UpdateView(prsnt, propertyName);

            _HoriPage.UpdateView(prsnt, propertyName);
            _LinePage.UpdateView(prsnt, propertyName);
            _FrequencyVSTimePage.UpdateView(prsnt, propertyName);

        }

        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                //Title = Presenter.Name;
                HeadBackColor = ControlPaint.Dark(Presenter.DrawColor, 0.1F);
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
            SetGroupIndex();
        }

        private void Stylize()
        {
            _RadioFrequencyPage.StylizeFlag = true;
            _HoriPage.StylizeFlag = true;
            _LinePage.StylizeFlag = true;
            _FrequencyVSTimePage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Presenter != null)
            {
                Presenter.TryRemoveView(this);
                Presenter.AmpVSTime.TryRemoveView(this);
                Presenter.PhaseVSTime.TryRemoveView(this);
                Presenter.PhaseVSFrequency.TryRemoveView(this);
            }
            base.OnFormClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }
    }
}
