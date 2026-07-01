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

    public partial class PwrRippleInfoForm :FloatForm, IEmbeddableDataView, IDataExportView
    {
        private Size _IndependentSize;

        public Size LastSize { get; set; }

        public DataTableFigure TableForm { get; set; } = null;
        public PwrRippleInfoForm(PowerAnalysisPrsnt pap, PwrRipplePrsnt prp)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrRippleInfoForm_ToolClick;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            //CustomToolIconInfos = new List<ToolIconInfo>()
            //{
            //    new ToolIconInfo()
            //    {
            //        IsShow = true,
            //        Icon = Properties.Resources.Setting,
            //        ClickHandler = PwrRippleInfoForm_SettingClick
            //    }
            //};

            Presenter = pap;
            RipplePresenter = prp;
        }

        private void PwrRippleInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBoFenXi");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBoFenXi");
            this.Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi");
            this.Mean.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJunZhi");
            this.Max.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiDaZhi");
            this.Min.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZuiXiaoZhi");
        }

        protected override void OnClosed(EventArgs e)
        {
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            base.OnClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        public Control GetDataView => ScRipple;

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

        private PowerAnalysisPrsnt Presenter { get; }

        private PwrRipplePrsnt RipplePresenter { get; }

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
            Stylize();
            base.OnLoad(e);
            UpdateView();
            _IndependentSize = GetDataView.Size;
            LvRipple.IsIndependentWindow = true;
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
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
            if (TableForm != null)
            {
                TableForm.Close();
                TableForm = null;
            }

            base.OnFormClosed(e);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvRipple.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvRipple.SelectedRowColor = LvRipple.BackColor;

                LvRipple.BeginUpdate();

                if (LvRipple.Items.Count == 0)
                {
                    foreach (var item in RipplePresenter.RippleHearders)
                    {
                        var content = new ListViewItem(new String[LvRipple.Columns.Count]);
                        content.SubItems[0].Text = LanguageHelper.GetPowerAnalysisString($"Ripple{item}");
                        content.SubItems[0].Tag = item;
                        LvRipple.Items.Add(content);
                    }
                }

                //String name = LanguageHelper.GetPowerAnalysisString($"Ripple{RipplePresenter.Pk2Pk.Name}");
                //if (LvRipple.Items[0].SubItems[0].Text != name)
                //{
                //    LvRipple.Items[0].SubItems[0].Text = name;
                //}

                //name = LanguageHelper.GetPowerAnalysisString($"Ripple{RipplePresenter.RMS.Name}");
                //if (LvRipple.Items[1].SubItems[0].Text != name)
                //{
                //    LvRipple.Items[1].SubItems[0].Text = name;
                //}

                foreach (ListViewItem item in LvRipple.Items)
                {
                    var index = LvRipple.Items.IndexOf(item);
                    var key = item.SubItems[0].Tag.ToString();
                    var value = RipplePresenter[key];
                    LvRipple.Items[index].SubItems[0].Text = LanguageHelper.GetPowerAnalysisString($"Ripple{key}");
                    LvRipple.Items[index].SubItems[1].Text = new Quantity(value.Value, Prefix.Empty, value.Unit).ToString(6, true);
                    if (RipplePresenter.Statistics)
                    {
                        LvRipple.Items[index].SubItems[2].Text = new Quantity(value.Mean, Prefix.Empty, value.Unit).ToString(6, true);
                        LvRipple.Items[index].SubItems[3].Text = new Quantity(value.Max, Prefix.Empty, value.Unit).ToString(6, true);
                        LvRipple.Items[index].SubItems[4].Text = new Quantity(value.Min, Prefix.Empty, value.Unit).ToString(6, true);
                    }
                    else
                    {
                        LvRipple.Items[index].SubItems[2].Text = String.Empty;
                        LvRipple.Items[index].SubItems[3].Text = String.Empty;
                        LvRipple.Items[index].SubItems[4].Text = String.Empty;
                    }
                }

                //LvRipple.Items[0].SubItems[1].Text = new Quantity(RipplePresenter.Pk2Pk.Value, Prefix.Empty, RipplePresenter.Pk2Pk.Unit).ToString(6, true);


                //if (RipplePresenter.Statistics)
                //{
                //    LvRipple.Items[0].SubItems[2].Text = new Quantity(RipplePresenter.Pk2Pk.Mean, Prefix.Empty, RipplePresenter.Pk2Pk.Unit).ToString(6, true);
                //    LvRipple.Items[0].SubItems[3].Text = new Quantity(RipplePresenter.Pk2Pk.Max, Prefix.Empty, RipplePresenter.Pk2Pk.Unit).ToString(6, true);
                //    LvRipple.Items[0].SubItems[4].Text = new Quantity(RipplePresenter.Pk2Pk.Min, Prefix.Empty, RipplePresenter.Pk2Pk.Unit).ToString(6, true);
                //}
                //else
                //{
                //    LvRipple.Items[0].SubItems[2].Text = "";
                //    LvRipple.Items[0].SubItems[3].Text = "";
                //    LvRipple.Items[0].SubItems[4].Text = "";
                //}

                LvRipple.EndUpdate();
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

        private void PwrRippleInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrRippleInfoForm_SettingClick(object sender, EventArgs e)
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
           return new List<DataTable>() { this.GetDataTable(LvRipple, Title) };
            //List<DataTable> result = new List<DataTable>();

            //DataTable dataTable = new DataTable() { TableName = this.Title };
            //dataTable.Columns.Add(this.Title);
            //foreach (ColumnHeader item in LvRipple.Columns)
            //{
            //    if (item == null || string.IsNullOrEmpty(item.Text))
            //        continue;

            //    dataTable.Columns.Add(item.Text);
            //}

            //DataRow row;
            //foreach (ListViewItem item in LvRipple.Items)
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
