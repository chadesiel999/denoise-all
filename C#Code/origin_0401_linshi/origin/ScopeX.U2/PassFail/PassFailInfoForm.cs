// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/24</date>

namespace ScopeX.U2.PassFail
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using EventBus;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.Core.Tools.DataExport;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using static System.Windows.Forms.ListViewItem;

    public partial class PassFailInfoForm :FloatForm, IEmbeddableDataView, IDataExportView
    {
        public Size LastSize { get; set; }

        public PassFailInfoForm(PassFailPrsnt prsnt)
        {
            InitializeComponent();
            FixedToolIconInfos[2].Icon = U2.Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = U2.Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            ToolClick += BtnExport_Click;
            Presenter = prsnt;
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
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            base.OnHandleDestroyed(e);

            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }
        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => InitControlLang();

        public Control GetDataView => tableLayoutPanel1; //LvPassFail;

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

        private PassFailPrsnt Presenter { get; }

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
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("P_FZhuangTai");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("P_FZhuangTai");

            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] { new System.Windows.Forms.ListViewItem.ListViewSubItem(null, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TestStatus"), System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(41, 42, 45), AppStyleConfig.DefaultLabelFont), new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "OFF", System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(41, 42, 45), AppStyleConfig.DefaultLabelFont) }, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingZongShu"), "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ViolationNumber"), "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TestingTime"), "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] { ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MingZhongGeShu"), "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] { new System.Windows.Forms.ListViewItem.ListViewSubItem(null, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CeShiJieGuo"),
                System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(41, 42, 45), AppStyleConfig.DefaultLabelFont), new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "Fail", System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(41, 42, 45), AppStyleConfig.DefaultLabelFont) }, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MeiDuanMingZhongShu_"));
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] { "1", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] { "2", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] { "3", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] { "4", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] { "5", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] { "6", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] { "7", "0" }, -1);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] { "8", "0" }, -1);

            LvPassFail.Items.Clear();
            LvPassFail.Items.AddRange(new System.Windows.Forms.ListViewItem[] { listViewItem1, listViewItem2, listViewItem3, listViewItem4, listViewItem5, listViewItem6, listViewItem7, listViewItem8, listViewItem9, listViewItem10, listViewItem11, listViewItem12, listViewItem13, listViewItem14, listViewItem15 });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitControlLang();
            Stylize();
            UpdateView();
            TmUpdate.Enabled = true;
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
            DefaultStyleManager.Instance.RegisterControlRecursion(LvPassFail, StyleFlag.FontSize);

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            PassFailApp.Default.InfoControl = null;
            if (Presenter.Active)
            {
                Presenter.Active = false;
            }
            //!!!Close embeded figure
            var pfif = GetDataView?.FindForm();
            if (pfif != this)
            {
                pfif?.Close();
            }

            TmUpdate.Elapsed -= TmUpdate_Tick;
            TmUpdate.Enabled = false;

            base.OnFormClosed(e);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (!Presenter.Active)
                {
                    Close();
                    return;
                }

                LvPassFail.BeginUpdate();

                if (Presenter.Mode == PFTestMode.LimitMode)
                {
                    UpdateLimitInfo();
                }
                else
                {
                    UpdateStdInfo();
                }

                LvPassFail.EndUpdate();
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

        private void UpdateLimitInfo()
        {
            LvPassFail.Items[0].SubItems[1].Text = Presenter.Running ? @"Running" : @"OFF";
            LvPassFail.Items[1].SubItems[1].Text = Presenter.Results.TotalWfms[0].ToString();
            LvPassFail.Items[2].SubItems[1].Text = Presenter.Results.FailWfms[0].ToString();
            LvPassFail.Items[3].SubItems[1].Text = new Quantity(Presenter.Results.RunningTime[0], Prefix.Milli, QuantityUnit.Second).ToString();

            LvPassFail.Items[4].SubItems[1].Text = Presenter.Results.TotalHits[0].ToString();
            LvPassFail.Items[5].SubItems[1].Text = Presenter.Results.FailWfms[0] > 0 ? @"Fail" : @"Pass";
            LvPassFail.Items[7].SubItems[1].Text = Presenter.Results.SegHits[0, 0].ToString();
            LvPassFail.Items[8].SubItems[1].Text = Presenter.Results.SegHits[1, 0].ToString();
        }

        private void UpdateStdInfo()
        {
            LvPassFail.Items[0].SubItems[1].Text = Presenter.Running ? @"Running" : @"OFF";
            LvPassFail.Items[1].SubItems[1].Text = Presenter.Results.TotalWfms[0].ToString();
            LvPassFail.Items[2].SubItems[1].Text = Presenter.Results.FailWfms[0].ToString();
            LvPassFail.Items[3].SubItems[1].Text = new Quantity(Presenter.Results.RunningTime[0], Prefix.Milli, QuantityUnit.Second).ToString();

            LvPassFail.Items[4].SubItems[1].Text = Presenter.Results.TotalHits[0].ToString();
            LvPassFail.Items[5].SubItems[1].Text = Presenter.Results.FailWfms[0] > 0 ? @"Fail" : @"Pass"; 
            LvPassFail.Items[7].SubItems[1].Text = Presenter.Results.SegHits[0, 0].ToString();
            LvPassFail.Items[8].SubItems[1].Text = Presenter.Results.SegHits[1, 0].ToString();

            LvPassFail.Items[9].SubItems[1].Text = Presenter.Results.SegHits[2, 0].ToString();
            LvPassFail.Items[10].SubItems[1].Text = Presenter.Results.SegHits[3, 0].ToString();
            LvPassFail.Items[11].SubItems[1].Text = Presenter.Results.SegHits[4, 0].ToString();
            LvPassFail.Items[12].SubItems[1].Text = Presenter.Results.SegHits[5, 0].ToString();
            LvPassFail.Items[13].SubItems[1].Text = Presenter.Results.SegHits[6, 0].ToString();
            LvPassFail.Items[14].SubItems[1].Text = Presenter.Results.SegHits[7, 0].ToString();
        }

        public void IndependentControl(Control control)
        {
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
        }

        private void PassFailInfoForm_EmbededClick(Object sender, EventArgs e)
        {
            (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this,true);
        }

        private void PassFailInfoForm_SettingClick(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PASSFAIL);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            this.DataExport();
        }

        public List<DataTable> GetDataTables()
        {
            DataTable dataTable = new DataTable() { TableName = this.Title };
            dataTable.Columns.Add(this.Title);
            dataTable.Columns.Add("  ");
            DataRow row = null;
            foreach (ListViewItem item in LvPassFail.Items)
            {
                if (item == null || item.SubItems == null || item.SubItems.Count < 2)
                    continue;

                row = dataTable.NewRow();
                row[dataTable.Columns[0]] = item.SubItems[0]?.Text ?? "";
                row[dataTable.Columns[1]] = item.SubItems[1]?.Text ?? "";
                dataTable.Rows.Add(row);
            }

            return new List<DataTable> { dataTable };
        }
    }
}
