using PdfSharpCore.Pdf;
using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PVTForm : FloatForm, IChnlView
    {
        private readonly PVTPage _PVTPage;


        public PVTForm()
        {
            InitializeComponent();

            _PVTPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Size = new(_PVTPage.Size.Width, _PVTPage.Size.Height + HeadHeight );
            Controls.Add(_PVTPage);
            Controls.SetChildIndex(_PVTPage, 0);
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

        public PhaseVSTimePrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (PhaseVSTimePrsnt)value;
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

            _PVTPage.UpdateView(prsnt, propertyName);
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
        }

        private void Stylize()
        {
            _PVTPage.StylizeFlag = true;
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
