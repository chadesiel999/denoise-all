using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MultiDomainForm : FloatForm, IMultiDomainView
    {
        public MultiDomainForm()
        {
            InitializeComponent();
        }

        private MultiDomainViewPage _ViewPage;
        private ParameterPage _ParameterPage;

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

        public MultiDomainPrsnt Presenter
        {
            get;
            set;
        }

        IMultiDomainPrsnt IView<IMultiDomainPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (MultiDomainPrsnt)value;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadOptionPage();
            base.OnLoad(e);
            Stylize();
        }

        private void LoadOptionPage()
        {
            _ViewPage = new();
            _ParameterPage = new();

            NbgRadio.SetGroupContent(0, _ViewPage);
            NbgRadio.SetGroupContent(1, _ParameterPage);
            Size = new(_ViewPage.Size.Width, _ViewPage.Size.Height + HeadHeight + NbgRadio.CurrentGroupNum * NbgRadio.NavBarHeight);
        }

        private void Stylize()
        {
            _ViewPage.StylizeFlag = true;
            _ParameterPage.StylizeFlag = true;

            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
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
            _ViewPage.UpdateView(prsnt, propertyName);
            _ParameterPage.UpdateView(prsnt, propertyName);

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }
    }
}
