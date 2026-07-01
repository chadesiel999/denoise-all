using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.U2.BaseControl;
using ScopeX.U2.File;
using ScopeX.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class DecodeEventTableForm : FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }
        public Size WindowSize { get; set; }
        public Boolean IsClosed = true;
        private System.Threading.CancellationTokenSource _TokenSource = new System.Threading.CancellationTokenSource();
        private Boolean _CirculationIsRun = false;
        private Timer _ThreadCheckTimer;

        private DecodeEventTableForm()
        {
            PageControl = new BaseControl.PageControl(DecodeApp.Default.PageInfo);
            InitializeComponent();
            LangControl_Init();
            Content.SizeChanged += Content_SizeChanged;
            Table.Scroll += Table_Scroll;
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            FixedToolIconInfos[4].IsShow = true;
            HelpClick += LeftToolBtn_Click;
            ToolClick += ExtrToolBtn_Click;
            FourClick += FourToolBtn_Click;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            WindowSize = this.Size;
            EventTablePage.SelectedIndexChanged += EventTablePage_SelectedIndexChanged;
            EventTablePage.Click += EventTablePage_Click;
        }

        private void EventTablePage_Click(object sender, EventArgs e)
        {
            EventTablePage_SelectedIndexChanged(sender, e);
        }

        private void Table_Scroll(object sender, ScrollEventArgs e)
        {
            EventTablePage.Reload();
        }

        private void Content_SizeChanged(object sender, EventArgs e)
        {
            if (Content.Width > 0)
            {
                RbChannel.Size = new Size(Content.Width, 30);
                PageControl.Size = new Size(Content.Width, 45);
                PageControl.Location = new Point(0, Content.Height - 1 - PageControl.Height);
                Table.Size = new Size(Content.Width, PageControl.Top - RbChannel.Bottom);
                Table.Location = new Point(0, RbChannel.Bottom);
            }
        }


        private void Table_SizeChanged(object sender, EventArgs e)
        {
            if (Table.Height > 0)
            {
                Table.Location = new Point(0, RbChannel.Bottom);
                EventTablePage.Location = new Point(0, 0);
                EventTablePage.Height = Table.Height - (Table.AutoScroll ? 17 : 0);
                EventTablePage.Width = Table.Width;
                EventTablePage.LoadInitWidth(Table.Width);
            }
        }


        private void EventTablePage_SizeChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in EventTablePage.SelectedItems)
            {
                item.Selected = false;
            }
        }

        private void EventTablePage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EventTablePage.SelectedItems == null || EventTablePage.SelectedItems.Count <= 0)
            {
                return;
            }
            var selectitem = EventTablePage.SelectedItems[0];
            var infos = EventTablePage.DecodeEventIndexes;
            var index = -1;
            var ch = ChannelId.B1 + RbChannel.ChoosedButtonIndex;
            if (Int32.TryParse(selectitem.Text, out index) && infos.Count >= index && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ch, out var cp) && cp is DecodePrsnt dp && dp != null)
            {
                var source = dp.DecodeChPrsnt.GetDecodeSource()?.FirstOrDefault();
                if (source != null && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(source.Value, out var scp) && scp.VuDatabase != null)
                {
                    var indexes = EventTablePage.DecodeEventIndexes[index - 1];
                    if (!Program.Oscilloscope.Timebase.IsZoom)
                    {
                        Program.Oscilloscope.Timebase.IsZoom = true;
                    }
                    var tmbinfo = scp.Sampling.GetCurrentTmbInfo();
                    var start = indexes.StartPosition;
                    var end = indexes.EndPosition;
                    if (end > Constants.MAX_XPOS_IDX)
                    {
                        end = Constants.MAX_XPOS_IDX;
                    }
                    var center = (start + end) / 2;
                    var scale = (end - start) / Constants.MAX_XPOS_IDX;
                    if (DsoPrsnt.DefaultDsoPrsnt.Timebase.MinPosition < center)
                    {
                        Program.Oscilloscope.Timebase.ZoomCenterX = center;
                        Program.Oscilloscope.Timebase.ZoomScaleX = scale;
                    }
                    else
                    {
                        Program.Oscilloscope.Timebase.ZoomCenterX = (Int32)Constants.MAX_XPOS_IDX / 2;
                        Program.Oscilloscope.Timebase.ZoomScaleX = scale;
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.PositionByus = center;
                        //if (source!.Value.IsReference()&&scp is ReferencePrsnt rp&&rp!=null)
                        //{
                        //    (rp.Sampling as SamplingPrsnt).PositionByus = center;
                        //}
                    }

                }
            }
        }


        private void RbChannel_IndexChanged(object sender, EventArgs e)
        {
            EventTablePage.SwitchInfoSource(ChannelId.B1 + RbChannel.ChoosedButtonIndex);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void LangControl_Init()
        {
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianLieBiao");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiJianLieBiao");
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => LangControl_Init();

        private void LeftToolBtn_Click(object sender, EventArgs e)
        {
            Table.AutoScrollPosition = new Point(0, 0);
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true, (Properties.Resources.TriggerLocation, LocationTriggerInfo));
            RbChannel.Location = new Point(0, 0);
            Table.Location = new Point(0, RbChannel.Height);
            Table.Height = PageControl.Top - RbChannel.Bottom;
            EventTablePage.Location = new Point(0, 0);
        }

        private void FourToolBtn_Click(object sender, EventArgs e)
        {
            LocationTriggerInfo();
        }

        Boolean _IsLocationTrigger = false;

        private void LocationTriggerInfo()
        {
            if (!EventTablePage.IsLoationTrigger)
            {
                EventTablePage.IsLoationTrigger = true;
            }
        }

        private void ExtrToolBtn_Click(object sender, EventArgs e)
        {
            //List<DataTable> dataTables = new List<DataTable>();
            //if (EventTablePage != null)
            //{
            //    var tb = EventTablePage.GetDataTable();
            //    if (tb != null)
            //    {
            //        tb.TableName = EventTablePage.ChannelId.ToString();
            //        dataTables.Add(tb);
            //    }
            //}

            //if (dataTables.Count <= 0)
            //    return;


            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls"; // csv(*.csv)|*.csv|
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
            //    var bytes = DataExportHelper.ConvertTables2FileBytes(fileType, dataTables.ToArray());
            //    System.IO.File.WriteAllBytes(dialog.FileName, bytes);
            //    WeakTip.Default.Write("Export", MsgTipId.SavingSuccess, false, System.IO.Path.GetDirectoryName(dialog.FileName));
            //}
            //catch (Exception ex)
            //{
            //    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
            //    WeakTip.Default.Write("Export", MsgTipId.SavingFailed);
            //}
            this.DataExport();
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

        public static DecodeEventTableForm Default { get; } = new DecodeEventTableForm();

        public Control GetDataView => Content;// TbcEvent.SelectedTab.Controls.Cast<Control>().First();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            _TokenSource.Cancel();
            if (Content.Parent is Form f)
            {
                f.Visible = false;
                IsClosed = true;
            }
            else if (Content.Parent is Control ctl && ctl.Parent is Form cf)
            {
                cf.Close();
                IsClosed = true;
            }
            DecodeApp.Default.UpdateEventState?.Invoke(false);
            base.OnClosing(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            this.Size = WindowSize;

        }

        public Boolean IsCanShow => IsClosed;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LastSize = Table.Size;
            Stylize();
        }

        public void LoadSourceEventInfos(ChannelId id)
        {
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ChannelId.B1, out var prsntb1);
            RbChannel.ChoosedButtonIndex = id - ChannelId.B1;
            EventTablePage.SwitchInfoSource(ChannelId.B1 + RbChannel.ChoosedButtonIndex);
            if (!prsntb1.Active)
            {
                RbChannel.SetButtonEnable(0, false);
            }
            if (PlatformUIManager.Default.Platform.Attribute.MutiBus)
            {
                DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ChannelId.B2, out var prsntb2);
                if (!prsntb2.Active)
                {
                    RbChannel.SetButtonEnable(1, false);
                }
            }
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, UserControls.Style.StyleFlag.FontSize);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible)
            {
            }
            else if (!_CirculationIsRun && Visible)
            {
                IsClosed = false;
                _TokenSource = new System.Threading.CancellationTokenSource();
                if (_ThreadCheckTimer != null)
                {
                    _ThreadCheckTimer.Stop();
                    _ThreadCheckTimer = null;
                }
                _ThreadCheckTimer = new Timer();
                _ThreadCheckTimer.Interval = 1000;
                _ThreadCheckTimer.Tick += (_, _) =>
                {
                    if (_TokenSource.IsCancellationRequested)
                    {
                        _ThreadCheckTimer.Stop();
                        if (Content.Parent is Form f)
                        {
                            f.Close();
                            IsClosed = true;
                        }
                        else if (Content.Parent is Control ctl && ctl.Parent is Form cf)
                        {
                            cf.Close();
                            IsClosed = true;
                        }
                    }
                    if (!_CirculationIsRun)
                        RefreshData();
                };
                _ThreadCheckTimer.Start();
            }
        }

        private async void RefreshData()
        {
            _CirculationIsRun = true;
            try
            {
                while (!_TokenSource.IsCancellationRequested)
                {
                    IChnlPrsnt prsnt_1, prsnt_2 = null;
                    DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ChannelId.B1, out prsnt_1);
                    if (PlatformUIManager.Default.Platform.Attribute.MutiBus)
                    {
                        DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(ChannelId.B2, out prsnt_2);
                    }
                    if (prsnt_1 != null)
                    {
                        if (EventTablePage.Id == ChannelId.B1)
                        {
                            if (!prsnt_1.Active && (prsnt_2?.Active ?? false))
                            {
                                RbChannel.ChoosedButtonIndex = 1;
                            }
                            EventTablePage.RefreshData();
                        }
                        if (prsnt_1.Active)
                        {
                            RbChannel.SetButtonEnable(0, true);
                        }
                        else
                        {
                            RbChannel.SetButtonEnable(0, false);
                        }
                    }
                    if (prsnt_2 != null)
                    {
                        if (EventTablePage.Id == ChannelId.B2)
                        {
                            if (!prsnt_2.Active && prsnt_1?.Active == true)
                            {
                                RbChannel.ChoosedButtonIndex = 0;
                            }
                            EventTablePage.RefreshData();
                        }
                        if (prsnt_2.Active)
                        {
                            RbChannel.SetButtonEnable(1, true);
                        }
                        else
                        {
                            RbChannel.SetButtonEnable(1, false);
                        }
                    }
                    if (!prsnt_1.Active && !(prsnt_2?.Active ?? false))
                    {
                        _TokenSource.Cancel();
                    }
                    else
                    {
                        await Task.Delay(500);
                    }

                }
            }
            catch (Exception)
            {

            }
            finally
            {
                _CirculationIsRun = false;
            }
        }



        public void IndependentControl(Control control)
        {
            Table.AutoScrollPosition = new Point(0, 0);
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            RbChannel.Location = new Point(0, 45);
            RbChannel.Size = new Size(Content.Width, 30);
            PageControl.Size = new Size(Content.Width, 45);
            PageControl.Location = new Point(0, Content.Height - 1 - PageControl.Height);
            Table.Width = Content.Width;
            Table.Height = PageControl.Top - RbChannel.Bottom;
            Table.Location = new Point(0, RbChannel.Bottom);
        }

        List<DataTable> IDataExportView.GetDataTables()
        {
            List<DataTable> dataTables = new List<DataTable>();
            if (EventTablePage != null)
            {
                var tb = EventTablePage.GetDataTable();
                if (tb != null)
                {
                    tb.TableName = EventTablePage.Id.ToString();
                    dataTables.Add(tb);
                }
            }

            return dataTables;
        }
    }
}
