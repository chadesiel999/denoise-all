// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/31</date>

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;

    public partial class PwrDifferInfoForm : FloatForm, IEmbeddableDataView,IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;

        public PwrDifferInfoForm(PowerAnalysisPrsnt pap, PwrDifferPrsnt pdp)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += BtnExport_Click;

            Presenter = pap;
            DifferPresenter = pdp;
        }

      

        public Control GetDataView => ScDiffer;

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

        private PowerAnalysisPrsnt Presenter { get; }

        private PwrDifferPrsnt DifferPresenter { get; }

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
            if (PowerAnalysisForm.InfoControl == GetDataView)
            {
                PowerAnalysisForm.InfoControl = null;
                if (Presenter.Active)
                {
                    Presenter.Active = false;
                }
            }

            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }

            base.OnFormClosed(e);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvDiffer.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvDiffer.SelectedRowColor = LvDiffer.BackColor;

                LvDiffer.BeginUpdate();

                if (0 == LvDiffer.Items.Count)
                {
                    LvDiffer.Items.Add(new ListViewItem(new String[LvDiffer.Columns.Count]));
                }

                var name = LanguageHelper.GetPowerAnalysisString($"Differ{DifferPresenter.SlewRate.Name}");
                if (LvDiffer.Items[0].SubItems[0].Text != name)
                {
                    LvDiffer.Items[0].SubItems[0].Text = name;
                }

                if (DifferPresenter.ValidExp)
                {
                    LvDiffer.Items[0].SubItems[1].Text = new Quantity(DifferPresenter.SlewRate.Max, Prefix.Empty, DifferPresenter.SlewRate.Unit).ToString(6, true);
                    LvDiffer.Items[0].SubItems[2].Text = new Quantity(DifferPresenter.SlewRate.Min, Prefix.Empty, DifferPresenter.SlewRate.Unit).ToString(6, true);

                }
                else
                {
                    LvDiffer.Items[0].SubItems[1].Text = MeasureHelper.MeasureEmpty;
                    LvDiffer.Items[0].SubItems[2].Text = MeasureHelper.MeasureEmpty;
                }

                LvDiffer.EndUpdate();
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

        private void PwrDifferInfoForm_EmbededClick(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this,true);
        }

        private void PwrDifferInfoForm_SettingClick(object sender, EventArgs e)
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
        private void BtnExport_Click(object sender, EventArgs e)
        {
            DataExportTool.DataExport(this);
        }
        public List<DataTable> GetDataTables()
        {
            DataTable dataTable = new DataTable() { TableName = this.Title };

            foreach (ColumnHeader item in LvDiffer.Columns)
            {
                if (item == null || string.IsNullOrEmpty(item.Text))
                    continue;

                dataTable.Columns.Add(item.Text);
            }

            DataRow row = null;
            foreach (ListViewItem item in LvDiffer.Items)
            {
                if (item == null || item.SubItems == null || item.SubItems.Count <= 0)
                    continue;

                row = dataTable.NewRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i >= item.SubItems.Count)
                        continue;
                    row[i] = item.SubItems[i]?.Text.ToString();
                }
                dataTable.Rows.Add(row);
            }

            return new List<DataTable>() { dataTable };
        }
    }
}
