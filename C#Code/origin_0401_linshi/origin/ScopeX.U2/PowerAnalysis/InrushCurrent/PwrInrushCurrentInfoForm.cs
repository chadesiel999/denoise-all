namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Controls.Language;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;

    public partial class PwrInrushCurrentInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;
        public DataTableFigure TableForm { get; set; } = null;
        public PwrInrushCurrentInfoForm(PowerAnalysisPrsnt pap, PwrInrushCurrentPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrInrushCurrentInfoForm_ToolClick;
            _Presenter = pap;
            _InrushCurrentPresenter = prsnt;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }
        private void PwrInrushCurrentInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        private void Instance_LanguageChanged(object sender, ILanguage e) => InitControlLang();

        private void InitControlLang()
        {
            var title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LangYongDianLiu");
            title = $"{_Presenter.Id} - {title}";
            Text = title;
            Title = title;
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
            Mean.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJunZhi");
            Max.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            Min.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
        }

        public Control GetDataView => ScInrushCurrent;

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

        private PowerAnalysisPrsnt _Presenter { get; }

        private PwrInrushCurrentPrsnt _InrushCurrentPresenter { get; }

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
                LvInrushCurrent.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvInrushCurrent.SelectedRowColor = LvInrushCurrent.BackColor;

                LvInrushCurrent.BeginUpdate();

                Int32 row = 0;
                foreach (var o in _InrushCurrentPresenter.InrushCurrents)
                {
                    if (row == LvInrushCurrent.Items.Count)
                    {
                        LvInrushCurrent.Items.Add(new ListViewItem(new String[LvInrushCurrent.Columns.Count]));
                    }

                    var name = LanguageHelper.GetPowerAnalysisString($"InrushCurrent{o.Name}");
                    if (LvInrushCurrent.Items[row].SubItems[0].Text != name)
                    {
                        LvInrushCurrent.Items[row].SubItems[0].Text = name;
                    }

                    LvInrushCurrent.Items[row].SubItems[1].Text = new Quantity(o.Value, Prefix.Empty, o.Unit).ToString(6, true);
                    LvInrushCurrent.Items[row].SubItems[2].Text = new Quantity(o.Mean, Prefix.Empty, o.Unit).ToString(6, true);
                    LvInrushCurrent.Items[row].SubItems[3].Text = new Quantity(o.Max, Prefix.Empty, o.Unit).ToString(6, true);
                    LvInrushCurrent.Items[row].SubItems[4].Text = new Quantity(o.Min, Prefix.Empty, o.Unit).ToString(6, true);
                    
                    row++;
                }

                LvInrushCurrent.EndUpdate();
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

        private void PwrInrushCurrentInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrInrushCurrentInfoForm_SettingClick(object sender, EventArgs e)
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
            return new List<DataTable>() { this.GetDataTable(LvInrushCurrent, Title) };
            //List<DataTable> result = new List<DataTable>();

            //DataTable dataTable = new DataTable() { TableName = this.Title };
            //dataTable.Columns.Add(this.Title);
            //foreach (ColumnHeader item in LvInrushCurrent.Columns)
            //{
            //    if (item == null || string.IsNullOrEmpty(item.Text))
            //        continue;

            //    dataTable.Columns.Add(item.Text);
            //}

            //DataRow row;
            //foreach (ListViewItem item in LvInrushCurrent.Items)
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
