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

    public partial class PwrLoopAnalysisInfoForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize;
        public DataTableFigure TableForm { get; set; } = null;
        public PwrLoopAnalysisInfoForm(PowerAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += PwrLoopAnalysisInfoForm_ToolClick;
            Presenter = prsnt;
            LoopAnalysisPresenter = prsnt.LoopAnalysisPrsnt.Value;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void PwrLoopAnalysisInfoForm_ToolClick(object sender, EventArgs e) => this.DataExport();

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Frequency.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PinLv");
            Amplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FuDu");
            Gain.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZengYi");
            Phase.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangWei");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HuanLuFenXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HuanLuFenXi");
        }

        public Control GetDataView => ScLoopAnalysis;

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

        private PwrLoopAnalysisPrsnt LoopAnalysisPresenter { get; }

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
                if (!LoopAnalysisPresenter.CalcCompleted)
                    return;
                LvLoopAnalysis.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
                LvLoopAnalysis.SelectedRowColor = LvLoopAnalysis.BackColor;

                LvLoopAnalysis.BeginUpdate();

                Int32 row = 0;
                for (Int32 i = 0; i < LoopAnalysisPresenter.ScanNum; i++)
                {
                    if (i == LvLoopAnalysis.Items.Count)
                    {
                        LvLoopAnalysis.Items.Add(new ListViewItem(new String[LvLoopAnalysis.Columns.Count]));
                    }

                    var update_str = new String[5];
                    update_str[0] = (i + 1).ToString();
                    update_str[1] = ValueToString(LoopAnalysisPresenter.Data[i].Freq, Prefix.Empty, QuantityUnit.Hertz); ;
                    update_str[2] = (i + 1).ToString();
                    update_str[3] = (i + 1).ToString();
                    update_str[4] = (i + 1).ToString();

                    if (i < LoopAnalysisPresenter.DataCount)
                    {
                        update_str[2] = ValueToString(LoopAnalysisPresenter.Data[i].Amp, Prefix.Milli, QuantityUnit.Voltage);//LoopAnalysisPresenter.Data[i].Amp.ToString();
                        update_str[3] = LoopAnalysisPresenter.Data[i].Gain.ToString("##0.###");
                        update_str[4] = LoopAnalysisPresenter.Data[i].Phase.ToString("##0.###");
                    }
                    else
                    {
                        update_str[2] = "--";
                        update_str[3] = "--";
                        update_str[4] = "--";
                    }

                    if (LvLoopAnalysis.Items[i].Text != update_str[0])
                    {
                        LvLoopAnalysis.Items[i].Text = update_str[0];
                    }

                    for (Int32 ii = 1; ii <= 4; ii++)
                    {
                        if (!LvLoopAnalysis.Items[i].SubItems[ii].Text.Equals(update_str[ii]))
                        {
                            LvLoopAnalysis.Items[i].SubItems[ii].Text = update_str[ii];
                        }
                    }

                    row++;
                }
                LvLoopAnalysis.Items.Add(new ListViewItem(new String[LvLoopAnalysis.Columns.Count]));
                for (Int32 i = LoopAnalysisPresenter.ScanNum + 1; i < LvLoopAnalysis.Items.Count; i++)
                {
                    if (LvLoopAnalysis.Items.Count > i)
                    {
                        LvLoopAnalysis.Items.RemoveAt(i);
                    }
                }

                LvLoopAnalysis.EndUpdate();
            }
        }

        private void ScLoopAnalysis_SizeChanged(object sender, System.EventArgs e)
        {
            ScLoopAnalysis.HorizontalScroll.Enabled = false;
            ScLoopAnalysis.HorizontalScroll.Visible = false;
        }


        private void LvLoopAnalysis_SizeChanged(object sender, System.EventArgs e)
        {
            var sc_size = ScLoopAnalysis.Size;
            var lv_size = LvLoopAnalysis.Size;
            if (lv_size.Width != sc_size.Width - 5 && lv_size.Height != sc_size.Height - 5)
            {
                LvLoopAnalysis.Size = new Size(sc_size.Width - 5, sc_size.Height - 5);
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
        private void PwrLoopAnalysisInfoForm_EmbededClick(object sender, EventArgs e)
        {
            TableForm = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        private void PwrLoopAnalysisInfoForm_SettingClick(object sender, EventArgs e)
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
            return new List<DataTable>() { this.GetDataTable(LvLoopAnalysis, Title) };
        }
    }
}
