using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.Visa;
using Ivi.Visa;
using ScopeX.Hardware.Calibration.Data.Base;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;
using System.Reflection;
using System.Text.Json;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class MainForm : Form
    {
        private Button? _buttonAiSetScpiTest;

        public MainForm()
        {
            var version = Application.ProductVersion.Split("+")[0];
            //striVersion = version.ToString();
            var lastWriteDateTime = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToString();
            String Version = $" [{version.ToString()}]({lastWriteDateTime})";
            InitializeComponent();
            Text = this.Text + Version;
            toolStripMenuItemLoadRemoteCaliFileData.Click += ToolStripMenuItemLoadRemoteCaliFileData_Click;
            toolStripMenuItemSaveCaliData2LocalFile.Click += ToolStripMenuItemSaveCaliData2LocalFile_Click;
            toolStripMenuItemLoadRemoteUsingData.Click += ToolStripMenuItemLoadRemoteUsingData_Click;
            toolStripMenuItemLoadLocalCaliFileDataAndUsing.Click += ToolStripMenuItemLoadLocalCaliFileDataAndUsing_Click;
            toolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile.Click += ToolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile_Click;
            toolStripMenuItemLoadCaliDataFromFlash.Click += ToolStripMenuItemLoadCaliDataFromFlash_Click;
            toolStripMenuItemWriteCaliData2Flash.Click += ToolStripMenuItemWriteCaliData2Flash_Click;
            BtnLoadFromFile.Click += BtnLoadFromFile_Click;
            Application.AddMessageFilter(ControlMsgFilter.Instance);

            Init();
            timer1AutoRefreshCaliData.Start();
            //临时代码
            tabControl1.TabPages.Remove(tabPageSoftLA);

            buttonWaveRegionCtrl_Click(buttonWaveRegionCtrl, new EventArgs());

            InstallProcessXMLAndPartTask_UIFunctions();
            InitAiSetScpiTestUi();
        }

        private void InitAiSetScpiTestUi()
        {
            if (_buttonAiSetScpiTest != null)
                return;

            _buttonAiSetScpiTest = new Button()
            {
                Name = "buttonAiSetScpiTest",
                Text = "AISet SCPI测试",
                Size = new Size(108, 28),
                Location = new Point(791, 4),
                UseVisualStyleBackColor = true
            };
            _buttonAiSetScpiTest.Click += ButtonAiSetScpiTest_Click;
            panel1.Controls.Add(_buttonAiSetScpiTest);
            _buttonAiSetScpiTest.BringToFront();
        }

        private void ButtonAiSetScpiTest_Click(object? sender, EventArgs e)
        {
            //if (currInstrument == null)
            //{
            //    MessageBox.Show("请先连接仪器！");
            //    return;
            //}

            using var dlg = new AiSetScpiTestForm(currInstrument);
            dlg.ShowDialog(this);
        }

        private void BtnLoadFromFile_Click(object? sender, EventArgs e)
        {
            ToolStripMenuItemLoadRemoteCaliFileData_Click(sender, e);
        }

        private Boolean oldInstrumentStateIsOK = false;
        internal void RefreshInstrumentState(bool bOK)
        {
            if (this.oldInstrumentStateIsOK != bOK)
            {
                oldInstrumentStateIsOK = bOK;
                if (labelInstrumentState.InvokeRequired)
                    labelInstrumentState.Invoke(new Action(() => this.labelInstrumentState.Text = bOK ? "√" : "×"));
                else
                    labelInstrumentState.Text = bOK ? "√" : "×";
                if (labelInstrumentState.InvokeRequired)
                    labelInstrumentState.Invoke(new Action(() => this.labelInstrumentState.BackColor = bOK ? Color.White : Color.Red));
                else
                    labelInstrumentState.BackColor = bOK ? Color.Lime : Color.Red;
                if (!bOK)
                {
                    if (buttonConnectInstrument.InvokeRequired)
                        this.Invoke(new Action(() => buttonConnectInstrument_Click(buttonConnectInstrument, new EventArgs())));
                    else
                        buttonConnectInstrument_Click(buttonConnectInstrument, new EventArgs());
                }
            }
        }
        private void ToolStripMenuItemWriteCaliData2Flash_Click(object? sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            //传递到远端
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_Send(this.currInstrument, dataType);
                }
            }
            //烧写到FLASH
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " FlashCaliData";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(5000);
            MessageBox.Show("Ok!");
        }

        private void ToolStripMenuItemLoadCaliDataFromFlash_Click(object? sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show($"该操作将覆盖远端原有校准数据及保存的数据，最好在此之前，先把现有的校准数据做备份{System.Environment.NewLine}。你确认要进行此操作吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? FlashCaliData";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(2000);
            //刷新根据端的数据
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }
        internal void UI_RefreshData()
        {
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
        }
        private void ToolStripMenuItemLoadLocalCaliFileDataAndSave2RemoteFile_Click(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("该操作将覆盖远端原有校准数据及保存的数据，你确认要进行此操作吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            string folder = folderBrowserDialog1.SelectedPath;
            if (folder.Last<char>() != '\\')
                folder += '\\';

            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    caliData.LoadFromFile(folder);

                    InstrumentInteract.CaliData_Send(this.currInstrument, dataType);
                    InstrumentInteract.CaliData_SaveData(this.currInstrument, dataType);
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }
        private void ToolStripMenuItemLoadLocalCaliFileDataAndUsing_Click(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("该操作将覆盖远端原有校准数据，你确认要进行此操作吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            string folder = folderBrowserDialog1.SelectedPath;
            if (folder.Last<char>() != '\\')
                folder += '\\';

            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    caliData.LoadFromFile(folder);

                    InstrumentInteract.CaliData_Send(this.currInstrument, dataType);
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }

        private void ToolStripMenuItemLoadRemoteUsingData_Click(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("该操作将覆盖本地校准数据，你确认要进行此操作吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }

        private void ToolStripMenuItemSaveCaliData2LocalFile_Click(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            string folder = folderBrowserDialog1.SelectedPath;
            if (folder.Last<char>() != '\\')
                folder += '\\';
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                    caliData.SaveToFile(folder);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }

        private void ToolStripMenuItemLoadRemoteCaliFileData_Click(object? sender, EventArgs? e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("该操作将覆盖本地校准数据、远端运行态校准数据，你确认要进行此操作吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_LoadFromFile(this.currInstrument, dataType);
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }
            MessageBox.Show("Ok!");
        }

        private void Init()
        {
            currInstrument = null;
            waveViewer1.CurrInstrument = currInstrument;
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && (tabPage.Controls[0] is IMainFormTabPage))
                    (tabPage.Controls[0] as IMainFormTabPage)?.SetInstrumentInteract(currInstrument);
            }
            InitUIInfoByProductType();
        }
        private IInstrumentSession? currInstrument = null;

        private void RefreshConstDataFromServer()
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "GetComModelConstData";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string recvStr = currInstrument.ReadString();
            if (recvStr == "")
                return;
            ServerDomainConstants.Convert(recvStr);
        }

        private void GetOscilloscopeInfo()
        {
            string scpiCmd = "*IDN?";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string recvStr = currInstrument.ReadString();
            CalibrationOscilloscopeInfo.SetCalibrationOscilloscopeInfo(recvStr);
            Utilities.Logger.RefreshLogger();
        }

        private record OscilloscopeInfo(string productType, string serialNumber, string softVersion);
  		private void GetAnalogChannelItemsDefine()
        {
            string cmd = $":FACT:CALI:SPEC:DATA? GetAnalogChannelParamsDefine";
            currInstrument.WriteString(cmd);
            string readback = currInstrument.ReadString();
            //if (readback=="")
            //{
            //    return;
            //}
            GlobalData.CurrentAnalogChannelItemsDefine = JsonSerializer.Deserialize<AnalogChannelItemsDefine>(readback);
        }
        private void GetAnalogChannelAllDefaultData()
        {
            string cmd = $":FACT:CALI:SPEC:DATA? GetAnalogChannelAllDefaultData";
            currInstrument.WriteString(cmd);

            string readback = currInstrument.ReadString();
            //if (readback == "")
            //{
            //    return;
            //}
            GlobalData.CurrentAnalogChannelAllDefault = JsonSerializer.Deserialize<AnalogChannelAllDefault>(readback);
        }
        private void buttonConnectInstrument_Click(object sender, EventArgs e)
        {
            if (buttonConnectInstrument.Tag.ToString() == "0")
            {
                string message = "";
                if (currInstrument != null)
                    currInstrument.Close();
                if (comboBoxVisaResource.Items.Count == 0)
                {
                    MessageBox.Show("没有可连接的仪器，请点击[刷新]按钮或启动示波器软件！");
                    return;
                }
                if (comboBoxVisaResource.SelectedItem == null)
                {
                    MessageBox.Show("没有可连接的仪器，请点击[刷新]按钮或启动示波器软件！");
                    return;
                }
                string selectedResource = comboBoxVisaResource?.SelectedItem?.ToString() ?? "";
                currInstrument = InstrumentSessionEngine.TryGetSession(selectedResource, "500", RefreshInstrumentState, out message);
                if (currInstrument == null)
                {
                    // 避免设备刚上电/网络刚连通时的瞬时失败，补一次更长超时的重试。
                    Thread.Sleep(200);
                    currInstrument = InstrumentSessionEngine.TryGetSession(selectedResource, "2000", RefreshInstrumentState, out message);
                }
                //currInstrument = InstrumentSessionEngine.TryGetSession("TCPIP0::192.168.2.110::inst0::INSTR", "20", out message);
                if (currInstrument == null)
                {
                    RefreshInstrumentState(false);
                    MessageBox.Show($"不能连接到指定的仪器：{selectedResource}\r\n请确认：\r\n1、仪器地址是否正确；\r\n2、仪器是否开机并完成启动；\r\n3、NI VISA/网络连接是否正常。", "错误提示");
                    return;
                }
                else
                {
                    RefreshInstrumentState(true);
                }
                waveViewer1.CurrInstrument = currInstrument;
                Cursor = Cursors.WaitCursor;
                RefreshConstDataFromServer();
                GetOscilloscopeInfo();
      			//GetAnalogChannelItemsDefine();
         //       GetAnalogChannelAllDefaultData();
                ServerSpecailData.Load(currInstrument);
                waveViewer1.Wave_ViewInit();
                this.tabPageMatlabSourceCode1.SetInstrument(currInstrument);
                //currInstrument.WriteString("*CLS");
                foreach (TabPage tabPage in this.tabControl1.TabPages)
                {
                    if (tabPage.Controls.Count > 0 && (tabPage.Controls[0] is IMainFormTabPage))
                        (tabPage.Controls[0] as IMainFormTabPage)?.SetInstrumentInteract(currInstrument);
                }
                if (ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI16G ||
                    ServerDomainConstants.ProductType == ComModel.ProductType.B21_DBI20G ||
                    ServerDomainConstants.ProductType == ComModel.ProductType.B24_AI20G)
                {
                    if (!tabControl1.TabPages.Contains(tabPageDbiAnalogChannel))
                        tabControl1.TabPages.Add(tabPageDbiAnalogChannel);
                    if (!tabControl1.TabPages.Contains(tabPageDbiCoefficientsTable))
                        tabControl1.TabPages.Add(tabPageDbiCoefficientsTable);
                    if (!tabControl1.TabPages.Contains(tabPageDbiLocalOscillators))
                        tabControl1.TabPages.Add(tabPageDbiLocalOscillators);

                    if (tabControl1.TabPages.Contains(tabPageAnalogChannel))
                        tabControl1.TabPages.Remove(tabPageAnalogChannel);
                }
                else
                {
                    if (tabControl1.TabPages.Contains(tabPageDbiAnalogChannel))
                        tabControl1.TabPages.Remove(tabPageDbiAnalogChannel);
                    if (tabControl1.TabPages.Contains(tabPageDbiCoefficientsTable))
                        tabControl1.TabPages.Remove(tabPageDbiCoefficientsTable);
                    if (tabControl1.TabPages.Contains(tabPageDbiLocalOscillators))
                        tabControl1.TabPages.Remove(tabPageDbiLocalOscillators);

                    if (!tabControl1.TabPages.Contains(tabPageAnalogChannel))
                        tabControl1.TabPages.Add(tabPageAnalogChannel);
                }
                ConstructUIByProductType();
                tabPageCoefficientsTable1.RefreshData();
                Cursor = Cursors.Default;

                var msg = MessageBox.Show("是否从Flash中装载校准数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    foreach (CaliDataType dataType in ServerDomainConstants.CurrProductIncludeCaliDataTypes!)
                    {
                        var caliData = CalibrationData.Helper.GetICaliData(dataType);
                        if (caliData != null)
                        {
                            if (dataType == CaliDataType.DbiAnalogParams)
                            {
                            }
                            InstrumentInteract.CaliData_LoadFromFile(this.currInstrument, dataType);
                            InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                        }
                    }
                    foreach (TabPage tabPage in this.tabControl1.TabPages)
                    {
                        if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                        {
                            if (MainFormTabPage.CaliDataType != CaliDataType.None)
                                MainFormTabPage.RefreshData();
                        }
                    }
                    Cursor = Cursors.Default;
                }
                //if (MessageBox.Show("需要从远端装载校准文件数据吗？", "提示", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                //{
                //    Cursor = Cursors.WaitCursor;
                //    foreach (CaliDataType dataType in ServerDomainConstants.CurrProductIncludeCaliDataTypes!)
                //    {
                //        ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                //        if (caliData != null)
                //        {
                //            if (dataType == CaliDataType.DbiAnalogParams)
                //            {
                //            }
                //            InstrumentInteract.CaliData_LoadFromFile(this.currInstrument, dataType);
                //            InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                //        }
                //    }
                //    foreach (TabPage tabPage in this.tabControl1.TabPages)
                //    {
                //        if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                //        {
                //            if (MainFormTabPage.CaliDataType != CaliDataType.None)
                //                MainFormTabPage.RefreshData();
                //        }
                //    }
                //    Cursor = Cursors.Default;
                //}
                buttonConnectInstrument.Text = "断开仪器";
            }
            else
            {
                currInstrument?.Close();
                currInstrument = null;
                waveViewer1.CurrInstrument = currInstrument;
                this.tabPageMatlabSourceCode1.SetInstrument(null);
                foreach (TabPage tabPage in this.tabControl1.TabPages)
                {
                    if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage)
                        (tabPage.Controls[0] as IMainFormTabPage)?.SetInstrumentInteract(currInstrument);
                }
                RefreshInstrumentState(false);

                buttonConnectInstrument.Text = "连接仪器";
            }
            buttonConnectInstrument.Tag = buttonRefresh.Enabled ? "1" : "0";
            this.buttonRefresh.Enabled = !this.buttonRefresh.Enabled;
            comboBoxVisaResource!.Enabled = this.buttonRefresh.Enabled;
        }
        private void buttonWaveRegionCtrl_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
            waveViewer1.Run(!splitContainer1.Panel1Collapsed);
            buttonWaveRegionCtrl.Text = splitContainer1.Panel1Collapsed ? "显示波形区" : "隐藏波形区";
            buttonCtrlRegionCtrl.Text = splitContainer1.Panel2Collapsed ? "显示控制区" : "隐藏控制区";
        }

        private void buttonCtrlRegionCtrl_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            waveViewer1.Run(!splitContainer1.Panel1Collapsed);
            buttonWaveRegionCtrl.Text = splitContainer1.Panel1Collapsed ? "显示波形区" : "隐藏波形区";
            buttonCtrlRegionCtrl.Text = splitContainer1.Panel2Collapsed ? "显示控制区" : "隐藏控制区";
        }
        private void buttonLoadAllCalibrationData_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            foreach (CaliDataType dataType in ServerDomainConstants.CurrProductIncludeCaliDataTypes!)
            {
                ICaliData? caliData = CalibrationData.Helper.GetICaliData(dataType);
                if (caliData != null)
                {
                    InstrumentInteract.CaliData_Get(this.currInstrument, dataType);
                }
            }
            foreach (TabPage tabPage in this.tabControl1.TabPages)
            {
                if (tabPage.Controls.Count > 0 && tabPage.Controls[0] is IMainFormTabPage MainFormTabPage)
                {
                    if (MainFormTabPage.CaliDataType != CaliDataType.None)
                        MainFormTabPage.RefreshData();
                }
            }

            MessageBox.Show("Ok!");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            waveViewer1.SaveSetting();
            BatchTaskPart_TxtFileStream.ForceCloseFile();
        }
        private void buttonGotoExePath_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(Application.ExecutablePath)!);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                comboBoxVisaResource.Items.Clear();
                List<String> definedInstrument = InstrumentSessionEngine.GetReachableResourceEx(400);
                foreach (String instrument in definedInstrument)
                {
                    comboBoxVisaResource.Items.Add(instrument);
                }
                if (comboBoxVisaResource.Items.Count > 0)
                    comboBoxVisaResource.SelectedIndex = 0;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private List<Action> TimerAutoActionList = new List<Action>();
        private object TimerAutoActionList_Locker = new object();
        private void timer1AutoRefreshCaliData_Tick(object sender, EventArgs e)
        {
            timer1AutoRefreshCaliData.Stop();
            lock (TimerAutoActionList_Locker)
            {
                foreach (Action action in TimerAutoActionList)
                    action.Invoke();
                TimerAutoActionList.Clear();
            }
            timer1AutoRefreshCaliData.Start();
        }

        private sealed class AiSetScpiTestForm : Form
        {
            private readonly IInstrumentSession _instrument;
            private readonly TextBox _tbSignalType = new() { Width = 220 };
            private readonly TextBox _tbResult = new()
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Width = 560,
                Height = 260
            };
            private readonly Button _btnRun = new() { Text = "触发AISet", Width = 100 };
            private readonly Button _btnQuery = new() { Text = "查询状态", Width = 100 };
            private readonly Button _btnRunAndPoll = new() { Text = "触发并轮询", Width = 100 };

            public AiSetScpiTestForm(IInstrumentSession instrument)
            {
                _instrument = instrument;
                Text = "AISet SCPI流程验证";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterParent;
                ClientSize = new Size(590, 340);

                var lbSignal = new Label()
                {
                    Text = "信号类型(可空):",
                    AutoSize = true,
                    Location = new Point(15, 15)
                };
                _tbSignalType.Location = new Point(120, 12);
                _tbSignalType.PlaceholderText = "如: SINE / QPSK";

                _btnRun.Location = new Point(15, 45);
                _btnQuery.Location = new Point(125, 45);
                _btnRunAndPoll.Location = new Point(235, 45);
                _tbResult.Location = new Point(15, 78);

                _btnRun.Click += (_, _) => TriggerAiSet();
                _btnQuery.Click += (_, _) => QueryAiSetStatus();
                _btnRunAndPoll.Click += async (_, _) => await TriggerAndPollAsync();

                Controls.Add(lbSignal);
                Controls.Add(_tbSignalType);
                Controls.Add(_btnRun);
                Controls.Add(_btnQuery);
                Controls.Add(_btnRunAndPoll);
                Controls.Add(_tbResult);
            }

            private void TriggerAiSet()
            {
                var signalType = _tbSignalType.Text.Trim();
                var cmd = string.IsNullOrWhiteSpace(signalType) ? ":SYST:AISE" : $":SYST:AISE {signalType}";
                _instrument.WriteString(cmd);
                AppendLog($"> {cmd}");
                AppendLog("< OK");
            }

            private bool QueryAiSetStatus()
            {
                const string cmd = ":SYST:AISE:STAT?";
                _instrument.WriteString(cmd);
                var response = _instrument.ReadString().Trim();  
                AppendLog($"> {cmd}");
                AppendLog($"< {response}");
                return response.Contains("\"status\":true", StringComparison.OrdinalIgnoreCase);
            }

            private async Task TriggerAndPollAsync()
            {
                try
                {
                    SetButtonsEnabled(false);
                    TriggerAiSet();

                    var start = DateTime.Now;
                    while ((DateTime.Now - start).TotalSeconds < 30)
                    {
                        await Task.Delay(300);
                        if (QueryAiSetStatus())
                        {
                            AppendLog("< 流程完成");
                            return;
                        }
                    }
                    AppendLog("< 轮询超时(30s)");
                }
                catch (Exception ex)
                {
                    AppendLog($"< ERROR: {ex.Message}");
                }
                finally
                {
                    SetButtonsEnabled(true);
                }
            }

            private void SetButtonsEnabled(bool enabled)
            {
                _btnRun.Enabled = enabled;
                _btnQuery.Enabled = enabled;
                _btnRunAndPoll.Enabled = enabled;
            }

            private void AppendLog(string line)
            {
                _tbResult.AppendText($"[{DateTime.Now:HH:mm:ss.fff}] {line}{Environment.NewLine}");
            }
        }
    }
}
