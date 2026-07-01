using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.AWG;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FVTForm : FloatForm, IChnlView
    {
        private readonly FVTPage _FVTPage;

        public FVTForm()
        {
            InitializeComponent();

            _FVTPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };

            Size = new(_FVTPage.Size.Width, _FVTPage.Size.Height + HeadHeight);
            Controls.Add(_FVTPage);
            Controls.SetChildIndex(_FVTPage, 0);
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

        public FrequencyVSTimePrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (FrequencyVSTimePrsnt)value;
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

            _FVTPage.UpdateView(prsnt, propertyName);
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
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void Stylize()
        {
            _FVTPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Presenter != null)
            {
                Presenter.TryRemoveView(this);
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
