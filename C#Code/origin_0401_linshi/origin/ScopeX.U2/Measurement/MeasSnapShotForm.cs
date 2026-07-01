using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.Core.Tools.DataExport;
using ScopeX.Measure;
using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using static ScopeX.Controls.Common.APIs.APIsStructs;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class MeasSnapShotForm : FloatForm, IMeasView, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        private Boolean _ArgToCtrl;
        private Size _ListSize;
        private Size _IndependentSize; //独立控件的大小
        private IEnumerable<MeasureItemProperties> showitems;//需要显示的项目
        private Boolean _MovingFlag = false;
        private Boolean _UpdateSnapshotFlag = false;
        internal MeasSnapShotForm()
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            LvContent.Font = AppStyleConfig.DefaultMeasureFontPlus;
            LvContent.HeaderFont = AppStyleConfig.DefaultMeasureFontPlus;
            LblTip.Font = AppStyleConfig.DefaultMeasureFontPlus;
            //LvContent内容初始化
            showitems = MeasureApp.Default.MeasCandidates.Values.Where(v => v.Level < 2 && !v.Name.Contains("@lv"));
            LvContent.Columns.AddRange(Enumerable.Range(0, 5).SelectMany(x =>
            {
                return new ListViewEx.ColumnInfo[]
                {
                    new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingCheng"),138),//todo
                    new ListViewEx.ColumnInfo(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhi"),110),
                };
            }));
            Int32 rowcount = (Int32)Math.Ceiling(showitems.Count() * 2.0D / LvContent.Columns.Count) + 2;
            LvContent.Items.AddRange(Enumerable.Range(0, rowcount).Select(x => new String[LvContent.Columns.Count]).ToList());
            LvContent.SizeChanged += LvContent_SizeChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            ToolClick += ExtrToolBtn_Click;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            LangChangeHandleUI();
            showitems = MeasureApp.Default.MeasCandidates.Values.Where(v => v.Level < 2 && !v.Name.Contains("@lv"));
            this.Invalidate();
        }

        private void LvContent_SizeChanged(Object sender, EventArgs e)
        {
            LvContent.SetScrollPosition(new Point(0, 0));
            PlSource.Top = LvContent.Bottom;
            PlSource.Height = PlSource.Parent.Height - LvContent.Height;
            CbxSource.Height = 33;
            CbxSource.Top = PlSource.Height - CbxSource.Height - 20;
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public MeasPrsnt Presenter
        {
            get;
            set;
        }

        IMeasPrsnt IView<IMeasPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (MeasPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.SnapshotActive):
                    if (!Presenter.SnapshotActive)
                    {
                        if (this.InvokeRequired)
                        {
                            if (!this.IsDisposed && !this.Disposing)
                            {
                                // 如果不在 UI 线程上，则使用 Invoke 方法执行代码
                                Invoke(new Action(() => Close()));
                            }
                        }
                        else
                        {
                            Close();
                        }
                    }
                    break;
                case nameof(MeasPrsnt.SnapshotSource):
                    LvContent.ForeColor = new Color[] { Presenter.SnapshotColor, AppStyleConfig.DefaultContextForeColor };
                    break;
            }
        }

        protected void UpdateView()
        {
            if (_MovingFlag)
            {
                _MovingFlag = false;
                return;
            }
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                if (_UpdateSnapshotFlag == false)
                {
                    _UpdateSnapshotFlag = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            UpdateSnapshot();
                        }
                        finally
                        {
                            _UpdateSnapshotFlag = false;
                        }
                    });
                }

                _ArgToCtrl = false;
            }
        }

        static readonly Regex regex = new Regex(@"(?<number>[\d\.]+)(?<unit>\D+)");

        private Boolean _FirstLoad = true;

        private void UpdateSnapshot()
        {
            //if (!DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure && TriggerPrsnt.State == SysState.Stop && !_FirstLoad)
            //    return;

            (Int32 row, Int32 column) currentindex = (0, 0);
            Boolean isactive = true;
            if (Program.Oscilloscope.TryGetChannel(Presenter.SnapshotSource, out IChnlPrsnt channel))
            {
                isactive = channel.Active;
            }

            String tip = String.Empty;
            if (isactive)
            {
                if (channel.Pack != null)
                {
                    tip = channel.Pack.Properties.Clipping != Clipping.None ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(channel.Pack.Properties.Clipping.GetType().UnderlyingSystemType.FullName + "." + channel.Pack.Properties.Clipping.ToString()) : String.Empty;

                }
            }
            else
            {
                tip = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChannelNotOpen");
            }
            foreach (var item in showitems)
            {
                if (currentindex.row >= LvContent.Items.Count)
                    break;
                //给data内容赋值
                LvContent.Items[currentindex.row][currentindex.column] = item.Text;
                currentindex.column++;
                _FirstLoad = false;
                if (!_FirstLoad || TriggerPrsnt.State != SysState.Stop)
                {
                    string newvalue = (isactive ? Presenter.CalcResultStringNow(item.Name, Presenter.SnapshotSource) : MeasureHelper.MeasureEmpty).ToFormat(item.Format);
                    LvContent.Items[currentindex.row][currentindex.column] = newvalue;
                }
                currentindex.column++;
                //换行
                if (currentindex.column == LvContent.Columns.Count)
                {
                    currentindex.column = 0;
                    currentindex.row++;
                }
            }
            LvContent.Refresh();

            if (!this.IsDisposed && this.IsHandleCreated)
            {
                (Program.Oscilloscope.View as DsoForm).Invoke(() =>
                {
                    if (!String.IsNullOrEmpty(tip))
                    {
                        LblTip.Visible = true;
                        LblTip.Text = tip;
                    }
                    else
                    {
                        LblTip.Visible = false;
                        LblTip.Text = tip;
                    }

                    if (this.IsHandleCreated)
                    {
                        CbxSource.SelectValue = Presenter.SnapshotSource;
                    }
                });
            }
            //var idx = Math.Max(0, CbxSource.FindStringExact(Presenter.SnapshotSource.ToString()));
            //if (CbxSource.SelectedIndex != idx)
            //{
            //    CbxSource.SelectedIndex = idx;
            //}

        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        private void LangChangeHandleUI()
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuKuaiZhao");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuKuaiZhao");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.DoubleBuffered = true;
            LangChangeHandleUI();
            LvContent.ForeColor = new Color[] { Presenter.SnapshotColor, AppStyleConfig.DefaultContextForeColor };
            LvContent.BackColor = new Color[] { Color.FromArgb(255, 41, 42, 45), AppStyleConfig.DefaultContextDarkBackColor };
            LoadSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
            Stylize();
            _MovingFlag = false;
            UpdateView();
            if (DsoPrsnt.FocusId.IsAnalog())
                Presenter.SnapshotSource = DsoPrsnt.FocusId;
            else
                CbxSource.SelectValue = (int)Presenter.SnapshotSource;
            _IndependentSize = TlpSnapshot.PreferredSize;
            KeyboardLed.Default.LedStateControl(LedEnum.LedQuickMeasure, true);
            _ListSize = LvContent.Size;

            TmUpdate.Enabled = true;

#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            CbxSource.DataSource = sources.Select(x => new ComboBoxItem(x.ToString(), x)).ToList();
            CbxSource.SelectValue = Presenter.SnapshotSource;
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.SnapshotSource = (ChannelId)CbxSource.SelectValue;
                }
            };
        }

        private void Stylize()
        {
            IsShowHelp = false;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            Presenter.SnapshotActive = false;
            LvContent.SizeChanged -= LvContent_SizeChanged;
            KeyboardLed.Default.LedStateControl(LedEnum.LedQuickMeasure, false);
            //!!!Close embeded figure
            var ef = GetDataView?.FindForm();
            if (ef != this)
            {
                ef?.Close();
            }

            if (TmUpdate != null)
            {
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
                TmUpdate.Dispose();
                TmUpdate = null;
            }

            base.OnFormClosed(e);
        }

        //!!!Route WM_KEYDOWN to main form, KeyPreview property of this form must set.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }

        private void TmUpdate_Tick(Object sender, EventArgs e)
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


        private void MeasSnapShotForm_LeftIconClick(object sender, EventArgs e)
        {
            _ArgToCtrl = true;
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
            _ArgToCtrl = false;
        }

        private void ExtrToolBtn_Click(object sender, EventArgs e)
        {
            this.DataExport();
        }

        public Control GetDataView => TlpSnapshot;

        public void IndependentControl(Control control)
        {
            _ArgToCtrl = true;
            control.Dock = DockStyle.Fill;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            LvContent.SetScrollPosition(Point.Empty);
            LvContent.Size = _ListSize;
            _ArgToCtrl = false;
        }

        private void MeasSnapShotForm_Move(object sender, EventArgs e)
        {
            _MovingFlag = true;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {



            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls";//csv(*.csv)|*.csv|
            //dialog.SupportMultiDottedExtensions = false;
            //dialog.OverwritePrompt = true;
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //dialog.SetWindowInCenter();
            //if (dialog.ShowDialog() != DialogResult.OK || String.IsNullOrWhiteSpace(dialog.FileName))
            //    return;

            //var ext = System.IO.Path.GetExtension(dialog.FileName).ToUpper();
            //FileType fileType = FileType.CSV;
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
            //    var bytes = DataExportHelper.ConvertTables2FileBytes(fileType, dt);
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
            DataTable dt = new DataTable() { TableName = this.Title };
            int totalcolumns = LvContent.Items.Max(c => c.Length);
            var chnel = (ChannelId)CbxSource.SelectValue;
            string chneltitle = $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TongDao")}：{chnel}";
            for (int i = 0; i < totalcolumns; i++)
            {
                if (i == 0)
                {
                    dt.Columns.Add(new DataColumn(chneltitle));
                }
                else
                {
                    dt.Columns.Add(new DataColumn(string.Join(' ', Enumerable.Repeat(" ", i))));
                }
            }

            DataRow dataRow = null;
            int columnindex = 0;
            foreach (string[] d in LvContent.Items)
            {
                if (d == null)
                    continue;

                columnindex = 0;
                dataRow = dt.NewRow();
                foreach (var item in d)
                {
                    dataRow[dt.Columns[columnindex++]] = item;
                }
                dt.Rows.Add(dataRow);
            }

            return new List<DataTable>() { dt };
        }
    }
}
