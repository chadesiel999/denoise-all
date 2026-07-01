// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/23</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.U2.Demo;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class DemoSetForm : FloatForm
    {
        private readonly PowerAnalysisPage _PowerAnalysisPage;
        public DemoSetForm()
        {
            InitializeComponent();
            _PowerAnalysisPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            IsShowHelp = false;
            NbgDemo.SetGroupContent(0, _PowerAnalysisPage);
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
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            _PowerAnalysisPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
