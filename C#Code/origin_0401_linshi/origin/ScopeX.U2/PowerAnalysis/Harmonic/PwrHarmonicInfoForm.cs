namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class PwrHarmonicInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;

        public DataTableFigure TableForm { get; set; } = null;
        public PwrHarmonicInfoForm(PowerAnalysisPrsnt pap, PwrHarmonicPrsnt php)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrHarmonicInfoForm_ToolClick;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            Presenter = pap;
            HarmonicPresenter = php;
            SetDistortionSize();
            TlpHarmonic.SizeChanged += TlpHarmonic_SizeChanged;
            //HelpClick += (_, _) =>
            //{
            //    var res = Int32.TryParse(HelpLabel, out var index);
            //    if (!res)
            //    {
            //        HelpProcessManager.SendCommand();
            //        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
            //        return;
            //    }
            //    HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(index));
            //};
        }

        private void PwrHarmonicInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

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

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Text = Presenter.Id.ToString() + "- " + Presenter.Mode.GetDescription_Lang();
            Title = Text;
        }

        private void SetDistortionSize()
        {
            LvHarmonicTable.Width = _IndependentSize.Width - LvHarmonicTable.Margin.Left - LvHarmonicTable.Margin.Right;
            LvDistortion.Width = _IndependentSize.Width - LvDistortion.Margin.Left - LvDistortion.Margin.Right;
            TlpHarmonic.RowStyles[0].Height = HarmonicPresenter.Distortions.Count() * (LvDistortion.ItemHeight + 10) + LvDistortion.Margin.Top + LvDistortion.Margin.Bottom;
            LvDistortion.Columns[0].Width = 180;
            LvDistortion.Height = HarmonicPresenter.Distortions.Count() * (LvDistortion.ItemHeight + 20) + LvDistortion.Margin.Top + LvDistortion.Margin.Bottom;
            //LvHarmonicTable.Top = LvDistortion.Height;
            LvHarmonicTable.Height = _IndependentSize.Height - LvDistortion.Height - LvHarmonicTable.Margin.Top - LvHarmonicTable.Margin.Bottom;
        }

        private void TlpHarmonic_SizeChanged(object sender, EventArgs e)
        {
            //在嵌入后尺寸发生了改变，改回去
            SetDistortionSize();
        }

        public Control GetDataView => TlpHarmonic;

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

        private PwrHarmonicPrsnt HarmonicPresenter { get; }

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

        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
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
                UpdateInfo();
            }
        }

        private String GetHarmonicFreq(Int32 i)
        {
            return new Quantity(HarmonicPresenter.GetHarmonicFreq(i), Prefix.Empty, QuantityUnit.Hertz).ToString("G3", true);
        }

        private String GetMagRatio(Int32 i)
        {
            return new Quantity(HarmonicPresenter.GetMagRatio(i), Prefix.Empty, QuantityUnit.Percent).ToString("G3", true);
        }

        private String GetMagRMS(Int32 i, QuantityUnit typeunit, QuantityUnit unit)
        {
            if(HarmonicPresenter.Unit== SweepType.Linear)
            {
                return new Quantity(HarmonicPresenter.GetMagRMS(i), Prefix.Empty, unit).ToString("G3", true);
            }
            else
            {
                return new Quantity(HarmonicPresenter.GetMagRMS(i), Prefix.Micro, typeunit, unit).ToString("G3", true);
            }
        }

        private String GetPhase(Int32 i)
        {
            return new Quantity(HarmonicPresenter.GetPhase(i), Prefix.Empty, QuantityUnit.Angle).ToString("G3", true);
        }

        private void UpdateInfo()
        {
            var columnname1 = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv_Hz_");
            var columnName2 = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LiangZhi___");
            var columnName3 = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LiangZhiJunFangGen");
            var columnName4 = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWei_°_");
            UpdateDistortion();
            UpdateHarmonicTab();
            void UpdateDistortion()
            {
                Int32 j = 0;

                foreach (PwrHarmonicPrsnt.DistortionItem o in HarmonicPresenter.Distortions)
                {
                    if (j == LvDistortion.Items.Count)
                    {
                        LvDistortion.Items.Add(new String[LvDistortion.Columns.Count]);
                    }

                    //String name = LanguageHelper.GetPowerAnalysisString($"Harmonic{o.Name}");
                    String name = LanguageHelper.GetPowerAnalysisString($"Harmonic{o.Name}");
                    if (LvDistortion.Items[j][0] != name)
                    {
                        LvDistortion.Items[j][0] = name;
                    }

                    LvDistortion.Items[j][1] = new Quantity(o.Value, Prefix.Empty, o.Unit).ToString(6, true);

                    j++;
                }

                LvDistortion.Refresh();
            }

            void UpdateHarmonicTab()
            {
                QuantityUnit magrmsunit = QuantityUnit.Voltage;
                QuantityUnit typeunit= HarmonicPresenter.Unit == SweepType.Linear? QuantityUnit.Variant: QuantityUnit.Decibel;

                LvHarmonicTable.Columns[1].Text = columnname1;
                LvHarmonicTable.Columns[2].Text = columnName2;
                if (HarmonicPresenter.Source == VIType.V)
                {
                    LvHarmonicTable.Columns[3].Text = columnName3 + (HarmonicPresenter.Unit== SweepType.Linear? @"(V)": @"(dBV)");
                }
                else
                {
                    LvHarmonicTable.Columns[3].Text = columnName3 + (HarmonicPresenter.Unit == SweepType.Linear ? @"(A)" : @"(dBA)");
                    magrmsunit = QuantityUnit.Ampere;
                }
                LvHarmonicTable.Columns[4].Text = columnName4;

                var temp = HarmonicPresenter.HarmonicIndexes;

                for (Int32 i = 0; i < temp.Count; i++)
                {
                    if (LvHarmonicTable.Items.Count <= i)
                    {
                        LvHarmonicTable.Items.Add(Enumerable.Repeat(MeasureHelper.MeasureEmpty, LvHarmonicTable.Columns.Count).ToArray());
                    }

                    LvHarmonicTable.Items[i][0] = temp[i].ToString();

                    LvHarmonicTable.Items[i][1] = GetHarmonicFreq(temp[i]-1);
                    LvHarmonicTable.Items[i][2] = GetMagRatio(temp[i]-1);
                    LvHarmonicTable.Items[i][3] = GetMagRMS(temp[i]-1, typeunit, magrmsunit);
                    LvHarmonicTable.Items[i][4] = GetPhase(temp[i]-1);
                }

                for (Int32 i = temp.Count; i< LvHarmonicTable.Items.Count;i++)
                {
                    LvHarmonicTable.Items.RemoveAt(i);
                }

                LvHarmonicTable.Refresh();
            }
        }

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateInfo));
            }
            else
            {
                UpdateInfo();
            }
        }

        private void PwrHarmonicInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this,true);
        }

        private void PwrHarmonicInfoForm_SettingClick(object sender, EventArgs e)
        {
           // _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        public void IndependentControl(Control control)
        {
            SetDistortionSize();
            control.Dock = DockStyle.Top;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            //SetDistortionSize();
            TableForm = null;
        }

        public List<DataTable> GetDataTables()
        {
            DataTable dataTable_t = new DataTable() { TableName = " Harmonic " };
            dataTable_t.Columns.Add(" ");
            dataTable_t.Columns.Add("  ");

            DataRow row = null;
            for (Int32 i = 0, ct = LvDistortion.Items.Count; i < ct; i++)
            {
                row = dataTable_t.NewRow();
                var rowitem = LvDistortion.Items[i];
                for (Int32 j = 0, ct_c = rowitem.Length; j < ct_c; j++)
                    row[dataTable_t.Columns[j]] = rowitem[j];

                dataTable_t.Rows.Add(row);
            }

            DataTable dataTable = new DataTable() { TableName = this.Title };
            dataTable.Columns.Add(this.Title);
            foreach (var item in LvHarmonicTable.Columns)
            {
                if (item == null || string.IsNullOrEmpty(item.Text))
                    continue;

                dataTable.Columns.Add(item.Text);
            }

            foreach (var rowData in LvHarmonicTable.Items)
            {
                row = dataTable.NewRow();
                for (Int32 i = 0; i < rowData.Length; i++)
                    row[dataTable.Columns[i]] = rowData[i];

                dataTable.Rows.Add(rowData);
            }

            return new List<DataTable>() { dataTable_t, dataTable };
        }
    }
}
