using ScopeX.Controls.Language;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
     public partial class PwrSlewRateInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        private Size _IndependentSize;

        public Size LastSize { get; set; }

        public DataTableFigure TableForm { get; set; } = null;
        public PwrSlewRateInfoForm(PowerAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrSlewRateInfoForm_ToolClick;
            _Presenter = prsnt;
            _SlewRatePresenter = prsnt.SlewRatePrsnt.Value;
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void PwrSlewRateInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        protected override void OnClosed(EventArgs e)
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
            base.OnClosed(e);
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, ILanguage e) => InitControlLang();

        public Control GetDataView => ScSlewRate;

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

        private PowerAnalysisPrsnt _Presenter { get; }

        private PwrSlewRatePrsnt _SlewRatePresenter { get; }

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

        private void InitControlLang()
        {
            var title = LanguageManger.Instance.GetIDMessage("ZhuanHuanSuLv");
            title = $"{_Presenter.Id} - {title}";
            Text = title;
            Title = title;

            Value.Text = LanguageManger.Instance.GetIDMessage("Zhi");
            Mean.Text = LanguageManger.Instance.GetIDMessage("PingJunZhi");
            Max.Text = LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            Min.Text = LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            InitControlLang();
            UpdateView();
            _IndependentSize = GetDataView.Size;
        }

        private void Stylize()
        {
            UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvSlewRate.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvSlewRate.SelectedRowColor = LvSlewRate.BackColor;

                LvSlewRate.BeginUpdate();

                Int32 row = 0;
                foreach (var o in _SlewRatePresenter.SlewRates)
                {
                    if (row == LvSlewRate.Items.Count)
                    {
                        LvSlewRate.Items.Add(new ListViewItem(new String[LvSlewRate.Columns.Count]));
                    }

                    var name = LanguageHelper.GetPowerAnalysisString($"SlewRate{o.Name}");
                    if (LvSlewRate.Items[row].SubItems[0].Text != name)
                    {
                        LvSlewRate.Items[row].SubItems[0].Text = name;
                    }

                    LvSlewRate.Items[row].SubItems[1].Text = new Quantity(o.Value, Prefix.Empty, o.Unit).ToString(6, true);

                    if (_SlewRatePresenter.Statistics)
                    {
                        LvSlewRate.Items[row].SubItems[2].Text = new Quantity(o.Mean, Prefix.Empty, o.Unit).ToString(6, true);
                        LvSlewRate.Items[row].SubItems[3].Text = new Quantity(o.Max, Prefix.Empty, o.Unit).ToString(6, true);
                        LvSlewRate.Items[row].SubItems[4].Text = new Quantity(o.Min, Prefix.Empty, o.Unit).ToString(6, true);
                    }
                    else
                    {
                        LvSlewRate.Items[row].SubItems[2].Text = "";
                        LvSlewRate.Items[row].SubItems[3].Text = "";
                        LvSlewRate.Items[row].SubItems[4].Text = "";
                    }

                    row++;
                }

                LvSlewRate.EndUpdate();
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

        private void PwrSlewRateInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrSlewRateInfoForm_SettingClick(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            control.Size = _IndependentSize;
            TableForm = null;
        }

        public List<DataTable> GetDataTables()
        {
           return new List<DataTable>() { this.GetDataTable(LvSlewRate, Title) };
            //List<DataTable> result = new List<DataTable>();

            //DataTable dataTable = new DataTable() { TableName = this.Title };
            //dataTable.Columns.Add(this.Title);
            //foreach (ColumnHeader item in LvSlewRate.Columns)
            //{
            //    if (item == null || string.IsNullOrEmpty(item.Text))
            //        continue;

            //    dataTable.Columns.Add(item.Text);
            //}

            //DataRow row;
            //foreach (ListViewItem item in LvSlewRate.Items)
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
