using EventBus;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Jitter;
using ScopeX.Core.Tools;
using ScopeX.Core.Tools.DataExport;
using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class MeasureResultForm :FloatForm, IEmbeddableDataView, IJitterView, IDataExportView
    {
        private Timer _TimerUpdate = new Timer();
        public MeasureResultForm()
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += BtnExport_Click;
            tableLayoutPanel1.SizeChanged += TableLayoutPanel1_SizeChanged;
            _TimerUpdate.Tick += TmUpdate_Tick;
            _TimerUpdate.Start();
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
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Presenter?.TryRemoveView(this);
        }
        private void TableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            LvMeasureResult.Height = tableLayoutPanel1.Height ;
            Detail.Width = tableLayoutPanel1.Width - 240 - 100;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => InitControlLang();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public JitterPrsnt Presenter
        {
            get;
            set;
        }

        public Control GetDataView => tableLayoutPanel1;

        public Size LastSize { get; set; }
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
                case nameof(Presenter.JitterParamEnable):
                    Close();
                    break;
                default:
                    break;
            }
        }
        private void InitControlLang()
        {
            Parameter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeLiangXiang");
            Value.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DangQianZhi");
            Detail.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BeiZhu");

            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DouDongFenXiCanShu");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DouDongFenXiCanShu");
        }

        private void InitDataTable()
        {
            List<ListViewItem> items = new List<ListViewItem>();
            items.Add(new ListViewItem(new String[3] { "TIE", "", "" }));
            items.Add(new ListViewItem(new String[3] { "TJ@BER=10e-12", "", "" }));
            items.Add(new ListViewItem(new String[3] { "RJ", "", "" }));
            items.Add(new ListViewItem(new String[3] { "DJ", "", "" }));
            items.Add(new ListViewItem(new String[3] { "PJ", "", "" }));
            items.Add(new ListViewItem(new String[3] { "DDJ", "", "" }));
            items.Add(new ListViewItem(new String[3] { "DCD", "", "" }));
            items.Add(new ListViewItem(new String[3]{ "ISI", "",""}));
            items.Add(new ListViewItem(new String[3]{ "CCJ", "",""}));
            LvMeasureResult.Items.AddRange(items.ToArray());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitControlLang();
            InitDataTable();
            UpdateView();
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            LvMeasureResult.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
            LvMeasureResult.SelectedRowColor = LvMeasureResult.BackColor;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _TimerUpdate.Stop();
            Presenter.TryRemoveView(this);
            Presenter.JitterParamEnable = false;
            base.OnFormClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
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
            if (!DesignMode && Presenter != null)
            {
                LvMeasureResult.BeginUpdate();
                //LvMeasureResult.Items[0].Text = "TIE";
                //LvMeasureResult.Items[0].SubItems[1].Text = new Quantity(Presenter.TIEPeak.Value, Prefix.Pico, Presenter.TIEPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[0].SubItems[2].Text = Properties.Resources.JitterParams_TIE;

                //LvMeasureResult.Items[1].Text = "TIEPeak";
                //LvMeasureResult.Items[1].SubItems[1].Text = new Quantity(Presenter.TIEPeak.Value, Prefix.Pico, Presenter.TIEPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[1].SubItems[2].Text = Properties.Resources.JitterParams_TIEPeak;

                //LvMeasureResult.Items[2].Text = "Tj";
                //LvMeasureResult.Items[2].SubItems[1].Text = new Quantity(Presenter.HistTj.Value, Prefix.Pico, Presenter.HistTj.Unit).ToString(6, true);
                //LvMeasureResult.Items[2].SubItems[2].Text = Properties.Resources.JitterParams_Tj;

                //LvMeasureResult.Items[3].Text = "Tj BER";
                //LvMeasureResult.Items[3].SubItems[1].Text = new Quantity(Presenter.HistTjBER.Value, Prefix.Pico, Presenter.HistTj.Unit).ToString(6, true);
                //LvMeasureResult.Items[3].SubItems[2].Text = Properties.Resources.JitterParams_TjBER;

                //LvMeasureResult.Items[4].Text = "Rj";
                //LvMeasureResult.Items[4].SubItems[1].Text = new Quantity(Presenter.HistRj.Value, Prefix.Pico, Presenter.HistRj.Unit).ToString(6, true);
                //LvMeasureResult.Items[4].SubItems[2].Text = Properties.Resources.JitterParams_Rj;

                //LvMeasureResult.Items[5].Text = "Pj";
                //LvMeasureResult.Items[5].SubItems[1].Text = new Quantity(Presenter.PjPeak.Value, Prefix.Pico, Presenter.PjPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[5].SubItems[2].Text = Properties.Resources.JitterParams_PjPeak;

                //LvMeasureResult.Items[6].Text = "Pj";
                //LvMeasureResult.Items[6].SubItems[1].Text = new Quantity(Presenter.PjPeak.Value, Prefix.Pico, Presenter.PjPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[6].SubItems[2].Text = Properties.Resources.JitterParams_PjPeak;


                //LvMeasureResult.Items[0].SubItems[0].Text = "TIE";
                LvMeasureResult.Items[0].SubItems[1].Text = new Quantity(Presenter.TIEPeak.Value, Prefix.Empty, Presenter.TIEPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[0].SubItems[2].Text = Properties.Resources.JitterParams_TIE;
                LvMeasureResult.Items[0].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_TIE");

                //LvMeasureResult.Items[1].SubItems[0].Text = "TJ@BER=10e-12";
                LvMeasureResult.Items[1].SubItems[1].Text = new Quantity(Presenter.HistTjBER.Value, Prefix.Empty, Presenter.HistTjBER.Unit).ToString(6, true);
                //LvMeasureResult.Items[1].SubItems[2].Text = Properties.Resources.JitterParams_TjBER;
                LvMeasureResult.Items[1].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_TjBER");

                //LvMeasureResult.Items[2].SubItems[0].Text = "RJ";
                LvMeasureResult.Items[2].SubItems[1].Text = new Quantity(Presenter.HistRj.Value, Prefix.Empty, Presenter.HistRj.Unit).ToString(6, true);
                //LvMeasureResult.Items[2].SubItems[2].Text = Properties.Resources.JitterParams_Rj;
                LvMeasureResult.Items[2].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_Rj");

                //LvMeasureResult.Items[3].SubItems[0].Text = "DJ";
                LvMeasureResult.Items[3].SubItems[1].Text = new Quantity(Presenter.HistDj.Value, Prefix.Empty, Presenter.HistDj.Unit).ToString(6, true);
                //LvMeasureResult.Items[3].SubItems[2].Text = Properties.Resources.JitterParams_Dj;
                LvMeasureResult.Items[3].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_Dj");

                //LvMeasureResult.Items[4].SubItems[0].Text = "PJ";
                LvMeasureResult.Items[4].SubItems[1].Text = new Quantity(Presenter.PjPeak.Value, Prefix.Empty, Presenter.PjPeak.Unit).ToString(6, true);
                //LvMeasureResult.Items[4].SubItems[2].Text = Properties.Resources.JitterParams_PjPeak;
                LvMeasureResult.Items[4].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_PjPeak");

                //LvMeasureResult.Items[5].SubItems[0].Text = "DDJ";
                LvMeasureResult.Items[5].SubItems[1].Text = new Quantity(Presenter.SpecDDj.Value, Prefix.Empty, Presenter.SpecDDj.Unit).ToString(6, true);
                //LvMeasureResult.Items[5].SubItems[2].Text = Properties.Resources.JitterParams_DDJ;
                LvMeasureResult.Items[5].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_DDJ");

                //LvMeasureResult.Items[6].SubItems[0].Text = "DCD";
                LvMeasureResult.Items[6].SubItems[1].Text = new Quantity(Presenter.SpecDCD.Value, Prefix.Empty, Presenter.SpecDCD.Unit).ToString(6, true);
                //LvMeasureResult.Items[6].SubItems[2].Text = Properties.Resources.JitterParams_DCD;
                LvMeasureResult.Items[6].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_DCD");

                //LvMeasureResult.Items[7].SubItems[0].Text = "ISI";
                LvMeasureResult.Items[7].SubItems[1].Text = new Quantity(Presenter.SpecISI.Value, Prefix.Empty, Presenter.SpecISI.Unit).ToString(6, true);
                //LvMeasureResult.Items[7].SubItems[2].Text = Properties.Resources.JitterParams_ISI;
                LvMeasureResult.Items[7].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_ISI");

                LvMeasureResult.Items[8].SubItems[1].Text = new Quantity(Presenter.CCjPeak.Value, Prefix.Empty, Presenter.SpecISI.Unit).ToString(6, true);

                LvMeasureResult.Items[8].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JitterParams_CC");

                LvMeasureResult.EndUpdate();

                //LvMeasureResult.BeginUpdate();

                //Int32 row = 0;
                //foreach (var o in Presenter.JitterParamTable)
                //{
                //    if (row == LvMeasureResult.Items.Count)
                //    {
                //        LvMeasureResult.Items.Add(new ListViewItem(new String[LvMeasureResult.Columns.Count]));
                //    }

                //    var name = o.Key;
                //    if (LvMeasureResult.Items[row].SubItems[0].Text != name)
                //    {
                //        LvMeasureResult.Items[row].SubItems[0].Text = name;
                //    }

                //    LvMeasureResult.Items[row].SubItems[1].Text = o.Value.ToString();
                //    row++;
                //}

                //LvMeasureResult.EndUpdate();
            }
        }

        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            //((ScopeXListViewEx)control).IsIndependentWindow = true;
        }

        private void MeasureResultForm_HelpClick(object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this,true);
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExport_Click(object sender, EventArgs e)
        {

            this.DataExport();
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls";//csv(*.csv)|*.csv|
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
        }

        public List<DataTable> GetDataTables()
        {
            DataTable dataTable = new DataTable() { TableName = this.Title };

            foreach (ColumnHeader item in LvMeasureResult.Columns)
            {
                if (item == null || string.IsNullOrEmpty(item.Text))
                    continue;

                dataTable.Columns.Add(item.Text);
            }

            DataRow row = null;
            foreach (ListViewItem item in LvMeasureResult.Items)
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
