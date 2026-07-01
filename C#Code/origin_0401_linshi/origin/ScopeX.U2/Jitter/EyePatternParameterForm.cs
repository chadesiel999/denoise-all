using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Jitter;
using ScopeX.Core.Tools;
using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class EyePatternParameterForm :FloatForm, IJitterView, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Size _IndependentSize; //独立控件的大小
        private System.Timers.Timer _TimerUpdate = new System.Timers.Timer();
        public Control GetDataView => tableLayoutPanel1;

        public EyePatternParameterForm()
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += BtnExport_Click;
            _TimerUpdate.Elapsed += TmUpdate_Tick;
            tableLayoutPanel1.SizeChanged += TableLayoutPanel1_SizeChanged;
            _TimerUpdate.Start();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            this.DataExport();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
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

        public JitterPrsnt Presenter
        {
            get;
            set;
        }

        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            switch (propertyName)
            {
                case nameof(Presenter.EyeParamEnable):
                    Close();
                    break;
                default:
                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            _IndependentSize = LvEyePatternParameter.PreferredSize;
            base.OnLoad(e);
            InitTable();
            UpdateView();
            Stylize();
        }

        private String[] _Header = new string[8];
        private Semaphore _HeaderSemaphore = new Semaphore(1,1);
        private void InitTable()
        {
            LvEyePatternParameter.Columns[0].Width = 180;
            List<ListViewItem> items = new List<ListViewItem>();
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_ZeroLevel"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_OneLevel"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeAmplitude"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeHeight"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeWidth"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_ExtinctionRatio"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_QFactor"), "" }));
            items.Add(new ListViewItem(new String[2] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeCrossRatio"), "" }));
            LvEyePatternParameter.Items.AddRange(items.ToArray());
            InitTableHeader();
        }

        private void InitTableHeader()
        {
            if(_HeaderSemaphore.WaitOne())
            {
                _Header[0] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_ZeroLevel");
                _Header[1] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_OneLevel");
                _Header[2] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeAmplitude");
                _Header[3] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeHeight");
                _Header[4] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeWidth");
                _Header[5] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_ExtinctionRatio");
                _Header[6] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_QFactor");
                _Header[7] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("EyeParams_EyeCrossRatio");
                _HeaderSemaphore.Release();
            }
        }
        private void TableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            LvEyePatternParameter.Height = tableLayoutPanel1.Height;
            Value.Width= tableLayoutPanel1.Width-100-100;
        }
        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Parameter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiangXiang");
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangQianZhi");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanTuCanShu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanTuCanShu");
            InitTableHeader();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            LvEyePatternParameter.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
            LvEyePatternParameter.SelectedRowColor = LvEyePatternParameter.BackColor;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_TimerUpdate != null)
            {
                _TimerUpdate.Stop();
                _TimerUpdate.Elapsed -= TmUpdate_Tick;
                _TimerUpdate.Enabled = false;
            }
            Presenter.TryRemoveView(this);
            Presenter.EyeParamEnable = false;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            base.OnFormClosed(e);
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

        private void UpdateView()
        {
            if (!DesignMode&& _HeaderSemaphore.WaitOne())
            {
                LvEyePatternParameter.BeginUpdate();
                LvEyePatternParameter.Items[0].Text = _Header[0];
                LvEyePatternParameter.Items[0].SubItems[1].Text = Presenter.EyeParamTable["ZeroLevel"];

                LvEyePatternParameter.Items[1].Text = _Header[1];
                LvEyePatternParameter.Items[1].SubItems[1].Text = Presenter.EyeParamTable["OneLevel"];

                LvEyePatternParameter.Items[2].Text = _Header[2];
                LvEyePatternParameter.Items[2].SubItems[1].Text = Presenter.EyeParamTable["EyeAmplitude"];

                LvEyePatternParameter.Items[3].Text = _Header[3];
                LvEyePatternParameter.Items[3].SubItems[1].Text = Presenter.EyeParamTable["EyeHeight"];

                LvEyePatternParameter.Items[4].Text = _Header[4];
                LvEyePatternParameter.Items[4].SubItems[1].Text = Presenter.EyeParamTable["EyeWidth"];

                LvEyePatternParameter.Items[5].Text = _Header[5];
                LvEyePatternParameter.Items[5].SubItems[1].Text = Presenter.EyeParamTable["ExtinctionRatio"];

                LvEyePatternParameter.Items[6].Text = _Header[6];
                LvEyePatternParameter.Items[6].SubItems[1].Text = Presenter.EyeParamTable["QFactor"];

                LvEyePatternParameter.Items[7].Text = _Header[7];
                LvEyePatternParameter.Items[7].SubItems[1].Text = Presenter.EyeParamTable["EyeCrossRatio"];

                _HeaderSemaphore.Release();
                LvEyePatternParameter.EndUpdate();

            }
        }
        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            //((ScopeXListViewEx)control).IsIndependentWindow = true;
        }

        private void EyePatternParameterForm_HelpClick(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
        }

        public List<DataTable> GetDataTables()
        {
            DataTable dataTable = new DataTable() { TableName = this.Title };
            foreach (ColumnHeader column in LvEyePatternParameter.Columns)
                dataTable.Columns.Add(column.Text);
            DataRow row = null;
            foreach (ListViewItem item in LvEyePatternParameter.Items)
            {
                if (item == null || item.SubItems == null || item.SubItems.Count < 2)
                    continue;
                row = dataTable.NewRow();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    row[j] = item.SubItems[j]?.Text ?? String.Empty;
                }
                dataTable.Rows.Add(row);
            }

            return new List<DataTable> { dataTable };
        }
    }
}
