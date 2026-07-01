// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/3/31</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls;

    public partial class VsaErrParamInfoForm : FloatForm, IEmbeddableDataView
    {
        public Size LastSize { get; set; }
        public VsaErrParamInfoForm(GenerateDigtalPrsnt prsnt)
        {
            InitializeComponent();

            Presenter = prsnt;
        }

        public Control GetDataView => LvQuality;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_COMPOSITED
                cp.ExStyle |= 0x02000000;
                //Turn off ALT+F4
                cp.ClassStyle |= 0x200;
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

        private GenerateDigtalPrsnt Presenter { get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            LvQuality.IsIndependentWindow = true;
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        private void UpdateView()
        {
            if (!DesignMode)
            {
                LvQuality.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvQuality.SelectedRowColor = LvQuality.BackColor;

                LvQuality.BeginUpdate();

                Int32 row = 0;
                foreach (var o in Presenter.ErrParamTable)
                {
                    if (row == LvQuality.Items.Count)
                    {
                        LvQuality.Items.Add(new ListViewItem(new String[LvQuality.Columns.Count]));
                    }

                    var name = o.Key;
                    if (LvQuality.Items[row].SubItems[0].Text != name)
                    {
                        LvQuality.Items[row].SubItems[0].Text = name;
                    }

                    //LvQuality.Items[row].SubItems[1].Text = o.Value.ToString();
                    LvQuality.Items[row].SubItems[1].Text = o.Value.Value?.ToString() ?? "";
                    LvQuality.Items[row].SubItems[2].Text = o.Value.Mean?.ToString() ?? "";
                    LvQuality.Items[row].SubItems[3].Text = o.Value.Max?.ToString() ?? "";
                    LvQuality.Items[row].SubItems[4].Text = o.Value.Min?.ToString() ?? "";
                    row++;
                }

                LvQuality.EndUpdate();
            }
        }

        //private void LvQuality_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LvQuality.SelectedItems.Clear();
        //    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        //}

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void PwrQualityInfoForm_LeftIconClick(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this);
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            ((ScopeXListViewEx)control).IsIndependentWindow = true;
        }
    }
}
