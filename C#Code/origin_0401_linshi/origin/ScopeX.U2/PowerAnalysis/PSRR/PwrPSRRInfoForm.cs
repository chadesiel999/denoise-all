namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;

    public partial class PwrPSRRInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;
        public DataTableFigure TableForm { get; set; } = null;
        public PwrPSRRInfoForm(PowerAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrPSRRInfoForm_ToolClick;
            Presenter = prsnt;
            PSRRPresenter = prsnt.PSRRPrsnt.Value;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void PwrPSRRInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Frequency.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv");
            Amplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuDu");
        }

        public Control GetDataView => ScPSRR;

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

        private PwrPSRRPrsnt PSRRPresenter { get; }

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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            base.OnClosing(e);
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
                if (!PSRRPresenter.CalcCompleted)
                    return;
                LvPSRR.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvPSRR.SelectedRowColor = LvPSRR.BackColor;

                LvPSRR.BeginUpdate();

                Int32 row = 0;
                for (Int32 i = 0; i < PSRRPresenter.ScanNum; i++)
                {
                    if (i == LvPSRR.Items.Count)
                    {
                        LvPSRR.Items.Add(new ListViewItem(new String[LvPSRR.Columns.Count]));
                    }

                    String name = (i + 1).ToString();
                    if (LvPSRR.Items[i].Text != name)
                    {
                        LvPSRR.Items[i].Text = name;
                    }

                    LvPSRR.Items[i].SubItems[1].Text = ValueToString(PSRRPresenter.Data[i].Freq, Prefix.Empty, QuantityUnit.Hertz);// PSRRPresenter.Data[i].Freq.ToString();

                    if (i < PSRRPresenter.DataCount)
                    {
                        LvPSRR.Items[i].SubItems[2].Text = ValueToString(PSRRPresenter.Data[i].Amp, Prefix.Milli, QuantityUnit.Voltage);//PSRRPresenter.Data[i].Amp.ToString();
                        LvPSRR.Items[i].SubItems[3].Text = new Quantity(PSRRPresenter.Data[i].PSRR, Prefix.Empty, "dB").ToString(6, true);
                    }
                    else
                    {
                        LvPSRR.Items[i].SubItems[2].Text = "--";
                        LvPSRR.Items[i].SubItems[3].Text = "--";
                    }

                    row++;
                }

                LvPSRR.Items.Add(new ListViewItem(new String[LvPSRR.Columns.Count]));
                for (Int32 i = PSRRPresenter.ScanNum + 1; i < LvPSRR.Items.Count + 1; i++)
                {
                    if (LvPSRR.Items.Count > i)
                    {
                        LvPSRR.Items.RemoveAt(i);
                    }
                }

                LvPSRR.EndUpdate();
            }
        }

        private String ValueToString(Double value, Prefix prefix, QuantityUnit unit)
        {
            return new Quantity(value, prefix, unit).ToString("##0.###", true);
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
        private void PwrPSRRInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrPSRRInfoForm_SettingClick(object sender, EventArgs e)
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
            return new List<DataTable>() { this.GetDataTable(LvPSRR, Title) };
        }
    }
}
