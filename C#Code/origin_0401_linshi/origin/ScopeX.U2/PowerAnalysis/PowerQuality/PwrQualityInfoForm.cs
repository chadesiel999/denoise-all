namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;

    public partial class PwrQualityInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;
        public DataTableFigure TableForm { get; set; } = null;

        public PwrQualityInfoForm(PowerAnalysisPrsnt prsnt)
        {
            this.FormClosed += PwrQualityInfoForm_FormClosed;
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += BtnExport_Click;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            Presenter = prsnt;
            QualityPresenter = prsnt.QualityPrsnt.Value;
        }

        private void PwrQualityInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => InitControlLang();

        public Control GetDataView => tableLayoutPanel1;

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

        private PwrQualityPrsnt QualityPresenter { get; }

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
            var title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYuanZhiLiang");
            title = $"{Presenter.Id} - {title}";
            Text = title;
            Title = title;
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
            Mean.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJunZhi");
            Max.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            Min.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            InitControlLang();
            UpdateView();
            LvQuality.Columns[0].Width = 180;
            LvQuality.Columns[4].Width = 140;
            _IndependentSize = GetDataView.Size;
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
            //HeadBackColor = AppStyleConfig.DefaultTitleBackColor;// Color.FromArgb(62, 62, 62);
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
            if (TableForm != null)
            {
                TableForm.Close();
                TableForm = null;
            }
            base.OnFormClosed(e);
        }

        private void UpdateView()
        {
            if (!DesignMode)
            {
                //!!!!!How to response the state change from the SCPI command
                //if (!Presenter.Active || PowerAnalysisForm.InfoControl != GetDataView)
                //{
                //    Close();
                //    return;
                //}


                void Handler()
                {
                    LvQuality.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                    LvQuality.SelectedRowColor = LvQuality.BackColor;
                    var qualities = QualityPresenter.Qualities;
                    LvQuality.BeginUpdate();

                    Int32 row = 0;
                    foreach (PwrQualityPrsnt.QualityItems o in qualities)
                    {
                        if (row == LvQuality.Items.Count)
                        {
                            LvQuality.Items.Add(new ListViewItem(new String[LvQuality.Columns.Count]));
                        }

                        String name = LanguageHelper.GetPowerAnalysisString($"Quality{o.Name}");
                        if (LvQuality.Items[row].Text != name)
                        {
                            LvQuality.Items[row].Text = name;
                        }

                        LvQuality.Items[row].SubItems[1].Text = new Quantity(o.Value, Prefix.Empty, o.Unit).ToString(6, true);

                        if (QualityPresenter.Statistics)
                        {
                            LvQuality.Items[row].SubItems[2].Text = new Quantity(o.Mean, Prefix.Empty, o.Unit).ToString(6, true);
                            LvQuality.Items[row].SubItems[3].Text = new Quantity(o.Max, Prefix.Empty, o.Unit).ToString(6, true);
                            LvQuality.Items[row].SubItems[4].Text = new Quantity(o.Min, Prefix.Empty, o.Unit).ToString(6, true);
                        }
                        else
                        {
                            LvQuality.Items[row].SubItems[2].Text = "";
                            LvQuality.Items[row].SubItems[3].Text = "";
                            LvQuality.Items[row].SubItems[4].Text = "";
                        }

                        row++;
                    }

                    LvQuality.EndUpdate();
                }

                if (this.InvokeRequired)
                {
                    // 有跨线程访问问题
                    this.BeginInvoke(Handler);
                }
                else
                {
                    Handler();
                }

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
        private void PwrQualityInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrQualityInfoForm_SettingClick(object sender, EventArgs e)
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

        private void BtnExport_Click(object sender, EventArgs e)
        {
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls"; //csv(*.csv)|*.csv|
            //dialog.SupportMultiDottedExtensions = false;
            //dialog.OverwritePrompt = true;
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //dialog.SetWindowInCenter();
            //if (dialog.ShowDialog() != DialogResult.OK || String.IsNullOrWhiteSpace(dialog.FileName))
            //    return;

            //var ext = System.IO.Path.GetExtension(dialog.FileName).ToUpper();
            //FileType fileType;
            //switch (ext)
            //{
            //    case ".TXT":
            //        fileType = FileType.Text;
            //        break;
            //    //case ".CSV":
            //    //    fileType = FileType.CSV;
            //    //    break;
            //    case ".XLS":
            //        fileType = FileType.Excel;
            //        break;
            //    default:
            //        StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.UnSupportedFormat, MessageType.Warning);
            //        return;
            //}



            //try
            //{
            //    var bytes = DataExportHelper.ConvertTables2FileBytes(fileType, dataTable);
            //    System.IO.File.WriteAllBytes(dialog.FileName, bytes);
            //    WeakTip.Default.Write("Export", MsgTipId.SavingSuccess, false, System.IO.Path.GetDirectoryName(dialog.FileName));
            //}
            //catch (Exception ex)
            //{
            //    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
            //    WeakTip.Default.Write("Export", MsgTipId.SavingFailed);
            //}

            DataExportTool.DataExport(this);
        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            ScQuality.Width = tableLayoutPanel1.Width - 15;
            ScQuality.Height = tableLayoutPanel1.Height;

            LvQuality.Width = tableLayoutPanel1.Width - 15;
            LvQuality.Height = tableLayoutPanel1.Height;
        }

        public List<DataTable> GetDataTables()
        {
           return new List<DataTable>() { this.GetDataTable(LvQuality, Title) };
            //DataTable dataTable = new DataTable() { TableName = this.Title };

            //dataTable.Columns.Add(new DataColumn(this.Title)); // 补充标题列
            //foreach (ColumnHeader item in LvQuality.Columns)
            //{
            //    if (item == null || string.IsNullOrEmpty(item.Text))
            //        continue;

            //    dataTable.Columns.Add(item.Text);
            //}

            //DataRow row = null;
            //foreach (ListViewItem item in LvQuality.Items)
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

            //return new List<DataTable>() { dataTable };
        }
    }
}
