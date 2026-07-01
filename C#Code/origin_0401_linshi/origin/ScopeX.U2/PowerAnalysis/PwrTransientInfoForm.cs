using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class PwrTransientInfoForm : FloatForm, IEmbeddableDataView
    {
        private Size _IndependentSize;

        public Size LastSize { get; set; }
        public PwrTransientInfoForm(PowerAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;

            Presenter = prsnt;
            TransientPresenter = prsnt.TransientPrsnt.Value;
        }

        public Control GetDataView => ScTransient;

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
                //Turn on WS_EX_COMPOSITED
                cp.ExStyle |= 0x02000000;
                //Turn off ALT+F4
                cp.ClassStyle |= 0x200;
                return cp;
            }
        }

        private PowerAnalysisPrsnt Presenter { get; }

        public PwrTransientPrsnt TransientPresenter
        {
            get;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            _IndependentSize = GetDataView.Size;
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            TmUpdate.Enabled = false;

            if (PowerAnalysisForm.InfoControl == GetDataView)
            {
                PowerAnalysisForm.InfoControl = null;
                if (Presenter.Active)
                {
                    Presenter.Active = false;
                }
            }

            base.OnFormClosed(e);
        }

        private void UpdateView()
        {
            if (!DesignMode)
            {
                LvTransient.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvTransient.SelectedRowColor = LvTransient.BackColor;

                LvTransient.BeginUpdate();
                if (0 == LvTransient.Items.Count)
                {
                    LvTransient.Items.Add(new ListViewItem(new String[LvTransient.Columns.Count]));
                }

                var name = LanguageHelper.GetPowerAnalysisString($"Transient");
                if (LvTransient.Items[0].SubItems[0].Text != name)
                {
                    LvTransient.Items[0].SubItems[0].Text = name;
                }

                LvTransient.Items[0].SubItems[1].Text = new Quantity(TransientPresenter.Transient.Value, Prefix.Empty, TransientPresenter.Transient.Unit).ToString(6, true);

                LvTransient.EndUpdate();
            }
        }

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateView));
            }
            else
            {
                UpdateView();
            }
        }

        private void PwrTransientInfoForm_EmbededClick(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrTransientInfoForm_SettingClick(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Top;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
        }
    }
}