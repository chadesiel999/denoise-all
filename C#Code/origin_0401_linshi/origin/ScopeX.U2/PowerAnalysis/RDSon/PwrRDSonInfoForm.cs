using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.Controls.Language;

namespace ScopeX.U2
{
    public partial class PwrRDSonInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;
        public DataTableFigure TableForm { get; set; } = null;
        public PwrRDSonInfoForm(PowerAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrRDSonInfoForm_ToolClick;
            _Presenter = prsnt;
            _RDSonPresenter = prsnt.RDSonPrsnt.Value;
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }
        private void PwrRDSonInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        private void Instance_LanguageChanged(object sender, ILanguage e) => InitControlLang();

        private void InitControlLang()
        {
            var title = "Rds(on)";
            title = $"{_Presenter.Id} - {title}";
            Text = title;
            Title = title;
            Value.Text = LanguageManger.Instance.GetIDMessage("Zhi");
            Mean.Text = LanguageManger.Instance.GetIDMessage("PingJunZhi");
            Max.Text = LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            Min.Text = LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
        }

        public Control GetDataView => ScRDSon;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
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

        private PowerAnalysisPrsnt _Presenter { get; }

        private PwrRDSonPrsnt _RDSonPresenter { get; }

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
            InitControlLang();
            UpdateView();
            _IndependentSize = GetDataView.Size;
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (PowerAnalysisForm.InfoControl == GetDataView)
            {
                PowerAnalysisForm.InfoControl = null;
                if (_Presenter.Active)
                {
                    _Presenter.Active = false;
                }
            }
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            if (TableForm != null)
            {
                TableForm.Close();
                TableForm = null;
            }
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            base.OnFormClosed(e);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvRDSon.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvRDSon.SelectedRowColor = LvRDSon.BackColor;

                LvRDSon.BeginUpdate();

                Int32 row = 0;
                foreach (var o in _RDSonPresenter.RDSons)
                {
                    if (row == LvRDSon.Items.Count)
                    {
                        LvRDSon.Items.Add(new ListViewItem(new String[LvRDSon.Columns.Count]));
                    }

                    var name = "Rds(on)";
                    if (LvRDSon.Items[row].SubItems[0].Text != name)
                    {
                        LvRDSon.Items[row].SubItems[0].Text = name;
                    }

                    LvRDSon.Items[row].SubItems[1].Text = new Quantity(o.Value, Prefix.Empty, o.Unit).ToString(6, true);
                    LvRDSon.Items[row].SubItems[2].Text = new Quantity(o.Mean, Prefix.Empty, o.Unit).ToString(6, true);
                    LvRDSon.Items[row].SubItems[3].Text = new Quantity(o.Max, Prefix.Empty, o.Unit).ToString(6, true);
                    LvRDSon.Items[row].SubItems[4].Text = new Quantity(o.Min, Prefix.Empty, o.Unit).ToString(6, true);

                    row++;
                }

                LvRDSon.EndUpdate();
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

        private void PwrRDSonInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrRDSonInfoForm_SettingClick(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Top;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            TableForm = null;
        }

        public List<DataTable> GetDataTables()
        {
            return new List<DataTable>() { this.GetDataTable(LvRDSon, Title) };
            //List<DataTable> result = new List<DataTable>();

            //DataTable dataTable = new DataTable() { TableName = this.Title };
            //dataTable.Columns.Add(this.Title);
            //foreach (ColumnHeader item in LvRDSon.Columns)
            //{
            //    if (item == null || string.IsNullOrEmpty(item.Text))
            //        continue;

            //    dataTable.Columns.Add(item.Text);
            //}

            //DataRow row;
            //foreach (ListViewItem item in LvRDSon.Items)
            //{
            //    if (item == null || item.SubItems == null || item.SubItems.Count <= 0)
            //        continue;

            //    row = dataTable.NewRow();
            //    for (Int32 i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        if (i >= item.SubItems.Count)
            //            continue;
            //        row[i] = item.SubItems[i]?.Text.ToString();
            //    }
            //    dataTable.Rows.Add(row);
            //}

            //result.Add(dataTable);

            //return result;
        }
    }
}
