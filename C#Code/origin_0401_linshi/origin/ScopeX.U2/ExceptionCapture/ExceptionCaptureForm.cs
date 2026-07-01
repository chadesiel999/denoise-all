using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ExceptionCaptureForm : FloatForm, IExceptionCaptureView
    {
        private ExceptionCapturePage _ExceptionCapturePage;

        public ExceptionCaptureForm(ExceptionCapturePrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
            _ExceptionCapturePage = new ExceptionCapturePage()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Presenter = Presenter,
            };
            Size = new(_ExceptionCapturePage.Size.Width, _ExceptionCapturePage.Size.Height + HeadHeight);
            Controls.Add(_ExceptionCapturePage);
            Controls.SetChildIndex(_ExceptionCapturePage, 0);
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

        public ExceptionCapturePrsnt Presenter
        {
            get;
            set;
        }

        IExceptionCapturePrsnt IView<IExceptionCapturePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ExceptionCapturePrsnt)value;
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
            if (Presenter != null)
            {
                if (_ExceptionCapturePage != null)
                {
                    _ExceptionCapturePage.UpdateView(prsnt, propertyName);
                }
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Presenter.TryRemoveView(this);
            _ExceptionCapturePage?.Dispose();
            base.OnClosing(e);
        }

        private void Stylize()
        {
            _ExceptionCapturePage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
