#nullable enable
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.Updater.Base;
using ScopeX.USBBridge;
using WindowsDSO_Updater;

namespace ScopeX.Updater;

/// <summary>
///     全屏实现更新，强制更新，目前没有检查固件版本。
/// </summary>
public partial class FullScreenForm : Form
{
    BaseUpdater baseUpdater;
    void FullScreenForm_Load(object sender, EventArgs e)
    {

        minPacketVersion = UpdateBaseHelper.GetVersionFromStr(UpdaterBaseConstants.LAST_SUPPORT_PACKAGE_VERSION);

        FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        lb_Title.Text += @$"    {fileInfo.FileVersion}";
        MSG_panle.Controls.Clear();
    }

    void ShowErrorInfo(string info)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => ShowErrorInfo(info));
        }
        else
        {
            if (!groupBox1.Visible)
                groupBox1.Visible = true;
            stopwatchProcessBar.Stop();
            UpdateStatusInfo(info);
            //errorInfoCtrl = new ErrorInfoCtrl(info, logFilePath)
            //{
            //    BorderStyle = BorderStyle.FixedSingle,
            //};
            Label msg = new()
            {
                Text = info
            };

            MSG_panle.Controls.Add(msg);
            //MSG_panle.Controls.Clear();
            //MSG_panle.Controls.Add(errorInfoCtrl);
        }
    }

    // void OpenLog()
    // {
    //     ProcessStartInfo psi = new("Explorer.exe");
    //     psi.Arguments = "/e,/select," + logFilePath;
    //     Process.Start(psi);
    // }

    void UpdateStatusInfo(string str = "")
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => { UpdateStatusInfo(str); });
        }
        else
        {
            StatusStripBar.Text = str;
        }
    }
 
    void Ctrl_Enable(bool enable)
    {
        BeginInvoke(() =>
        {
            foreach (object ctrl in Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = enable;
                }
            }
            //panelTime.Visible = !enable;
            if (!enable)
            {
                progressBar.Maximum = (int)NeedTotalSeconds;
                stopwatchProcessBar.Restart();
                Timer1.Enabled = true;
                Timer1.Start();
            }
            else
            {
                Timer1.Stop();
                Timer1.Enabled = false;
            }
        });
    }

    void StopUpdate(string message)
    {
        Ctrl_Enable(true);
        stopwatchProcessBar.Stop();

        if (message == "") return;
        baseUpdater.WriteLog(message);
        MessageBox.Show(message);
    }


    /// <summary>
    ///     是否需要更新
    /// </summary>
    /// <param name="pkgVer">包版本</param>
    /// <param name="hardVer">硬件版本</param>
    /// <returns></returns>
    void UpdateTotalProgressBar()
    {
        progressBar.Invoke(() =>
        {
            int elapsedSeconds = (int)(ProcessStopwatch.ElapsedMilliseconds / 1000);
            if (elapsedSeconds > progressBar.Maximum)
                elapsedSeconds = progressBar.Maximum;
            progressBar.Value = elapsedSeconds;
            progressBar.Update();
        });
        int currValue = (int)(stopwatchProcessBar.ElapsedMilliseconds / 1000);
        lb_UsingTime.Text = (currValue / BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0') + @":" + (currValue % BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0');
    }

    /// <summary>
    ///     启动更新流程。
    /// </summary>
    async void StartUpdate()
    {
        _isuupdate = true;
        //timer.Stop();
        UpdateStatusInfo();
        progressBar.Value = 0;
        string? appDir = Path.GetDirectoryName(Application.ExecutablePath);

        string[]? updFiles = null;
        if (CommandLineMode.Instance.UpdFilePaths != null && CommandLineMode.Instance.UpdFilePaths.Any())
        {
            // 优先以命令行参数为准
            updFiles = CommandLineMode.Instance.UpdFilePaths.ToArray();
        }
        else
        {
            if (appDir != null) updFiles = Directory.GetFiles(appDir, "*.upd");
        }

        if (updFiles != null && !updFiles.Any())
        {
            MessageBox.Show(@"未找到更新包，请确认！");
            return;
        }

        if (updFiles != null)
        {
            string currPackageFileName = updFiles.First(); // UPD文件路径
            baseUpdater.UpdatePackage = UpdatePackage.Load(currPackageFileName);
        }
        if (baseUpdater.UpdatePackage?.Items == null || baseUpdater.UpdatePackage.Items.Count == 0)
        {
            MessageBox.Show(@"[更新包文件]有误！");
            return;
        }
        if (baseUpdater.UpdatePackage.PackageVersion.CompareTo(minPacketVersion) < 0)
        {
            MessageBox.Show(@$"[更新包文件]版本过低，支持的最低版本：[{minPacketVersion}]！");
            return;
        }

        FpgaUpdateItemCount = 0;
        List<UpdateItem> mcuUpdateItems_AnalogChannel = new();
        List<UpdateItem> mcuUpdateItems_Probe = new();
        List<UpdateItem> McuUpdateItems_Keyboard = new();
        List<UpdateItem> USBUpdateItems_Bridge = new();
        foreach (UpdateItem item in baseUpdater.UpdatePackage.Items)
        {
            if (item.Type == UpdaterItemType.Fpga)
                FpgaUpdateItemCount++;
            else if (item.Type == UpdaterItemType.Mcu_AnalogChannel)
                mcuUpdateItems_AnalogChannel.Add(item);
            else if (item.Type == UpdaterItemType.Mcu_Keyboard)
                McuUpdateItems_Keyboard.Add(item);
            else if (item.Type == UpdaterItemType.USBBridge)
            {
                USBUpdateItems_Bridge.Add(item);
            }
            else if (item.Type == UpdaterItemType.Probe)
            {
                mcuUpdateItems_Probe.Add(item);
            }
        }

        lb_EstTime.Text = @"计算中...";
        bool mustHaveFpga = FpgaUpdateItemCount > 0 || mcuUpdateItems_AnalogChannel.Count > 0;
        //if (FpgaUpdateItemCount > 0)//注释原因是，必须依赖FPGA（虽然是PCIE)上电通道板。此处的逻辑有些不完美！！！
        //{
        List<UpdateItem>? updateFpgaItems = null;
        bool success = await Task.Run(() => baseUpdater.CheckHardwave(mustHaveFpga, out updateFpgaItems));
        if (!success || updateFpgaItems == null)
        {
            baseUpdater.FpgaUpdater!.Close();
            baseUpdater.AddInfo("初始化上电失败！");
            ShowErrorInfo("更新出错");
            //StopUpdate("遇到严重的问题，请与开发商联系！");
            return;
        }
        //}

        NeedTotalSeconds = 5;
        if (updateFpgaItems.Count > 0)
        {
            NeedTotalSeconds += BaseUpdater.ESTIMATED_TIME_ERASE;
            foreach (UpdateItem item in updateFpgaItems)
            {
                long pages = ((item.TotalBytes + 256) - 1) / 256;
                int baseTimeNeed = baseUpdater.FpgaUpdater!.PerWriteRegUsedNs == 0 ? 3800 : baseUpdater.FpgaUpdater!.PerWriteRegUsedNs;
                NeedTotalSeconds += (pages * ((256 * 4) + 28) * baseTimeNeed) / 1000_000_000; //1000_000_000,ns=>s,写入需要的时间(seconds)
                NeedTotalSeconds += (item.TotalBytes * 3L * baseTimeNeed) / 1000_000_000; //1000_000_000,ns=>s,验证需要的时间
                if (item.TypeID != BaseUpdater.AWG_BOARD_INDEX)
                {
                    NeedTotalSeconds += 60; //烧写版本信息花费的时间
                    NeedTotalSeconds += 70; //修正
                }
            }
        }
        if (USBUpdateItems_Bridge.Count > 0)
        {
            NeedTotalSeconds += 240 * USBUpdateItems_Bridge.Count;
        }
        if (mcuUpdateItems_AnalogChannel.Count > 0)
        {
            NeedTotalSeconds += 90;
        }
        NeedTotalSeconds += baseUpdater.UpdatePackage.Items.Count - FpgaUpdateItemCount;
        progressBar.Maximum = (int)NeedTotalSeconds;
        lb_EstTime.Text = (NeedTotalSeconds / BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0') + @":" + (NeedTotalSeconds % BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0');
        lb_EstTime.Visible = true;
        panelTime.Visible = true;
        string message = "在更新期间，请确保不要断电！";
        if (FpgaUpdateItemCount > 0)
            message += Environment.NewLine + "(本次更新需要较长的时间！)";
        message += Environment.NewLine + "您确认需要更新吗？";
        if (MessageBox.Show(message, @"提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            if (FpgaUpdateItemCount > 0)
                baseUpdater.FpgaUpdater!.Close();
            baseUpdater.AddInfo("取消了更新！");
            Ctrl_Enable(true);
            return;
        }
        UpdateStatusInfo("更新中...请勿[关闭程序]或[使设备断电]");
        ProcessStopwatch.Restart();
        Ctrl_Enable(false);

        bSoftwareUpdateErrorFound = false;
        if (updateFpgaItems.Count > 0)
        {
            try
            {
                bool bFpgaUpdateErrorFound = await baseUpdater.UpdateFpgaFirmware(updateFpgaItems); //await Task.Run(() =>  UpdateFpgaFirmware(updateFpgaItems));
                if (!bFpgaUpdateErrorFound)
                {
                    bSoftwareUpdateErrorFound = true;
                    ShowErrorInfo("[FPGA]更新出错");
                    //StopUpdate("遇到严重的问题，请与开发商联系！");
                    //return;
                }
            }
            catch (Exception e)
            {
                baseUpdater.AddInfo(e.ToString());
                ShowErrorInfo("[FPGA]更新出错");
                bSoftwareUpdateErrorFound = true;
            }
        }
        if (mcuUpdateItems_Probe.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_Probe(mcuUpdateItems_AnalogChannel));
                if (!bUpdateErrorFound)
                {
                    ShowErrorInfo("[探头]更新出错");
                    bSoftwareUpdateErrorFound = true;
                    //StopUpdate("遇到严重的问题，请与开发商联系！");
                    //return;
                }
            }
            catch (Exception e)
            {
                baseUpdater.AddInfo(e.ToString());
                ShowErrorInfo("[探头]更新出错");
                bSoftwareUpdateErrorFound = true;
            }
        }
        if (mcuUpdateItems_AnalogChannel.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_AnalogChannelFirmware(mcuUpdateItems_AnalogChannel));
                if (!bUpdateErrorFound)
                {
                    ShowErrorInfo("[通道]更新出错");
                    bSoftwareUpdateErrorFound = true;
                    //StopUpdate("遇到严重的问题，请与开发商联系！");
                    //return;
                }
            }
            catch (Exception e)
            {
                baseUpdater.AddInfo(e.ToString());
                ShowErrorInfo("[通道]更新出错");
                bSoftwareUpdateErrorFound = true;
            }
        }
        if (McuUpdateItems_Keyboard.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await baseUpdater.UpdateMcu_KeyboardFirmware(McuUpdateItems_Keyboard);
                if (!bUpdateErrorFound)
                {
                    ShowErrorInfo("[键盘板]更新出错");
                    bSoftwareUpdateErrorFound = true;
                    //StopUpdate("遇到严重的问题，请与开发商联系！");
                    //return;
                }
            }
            catch (Exception e)
            {
                baseUpdater.AddInfo(e.ToString());
                ShowErrorInfo("[键盘板]更新出错");
                bSoftwareUpdateErrorFound = true;
            }
        }
        if (USBUpdateItems_Bridge.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_USBBridges(USBUpdateItems_Bridge));
                if (!bUpdateErrorFound)
                {
                    ShowErrorInfo("[USB]更新出错");
                    bSoftwareUpdateErrorFound = true;
                    //return;
                }
            }
            catch (Exception e)
            {
                baseUpdater.AddInfo(e.ToString());
                ShowErrorInfo("[USB]更新出错");
                bSoftwareUpdateErrorFound = true;
            }
        }

        if ((baseUpdater.UpdatePackage.Items.Count - FpgaUpdateItemCount - mcuUpdateItems_AnalogChannel.Count - McuUpdateItems_Keyboard.Count - USBUpdateItems_Bridge.Count) > 0)
        {
            bSoftwareUpdateErrorFound = await baseUpdater.UpdateSoftware();
        }
        string timeInfoStr = $"本次更新花费时间 {ProcessStopwatch.ElapsedMilliseconds / 1000} s.";
        baseUpdater.AddInfo(timeInfoStr);
        UpdateStatusInfo(timeInfoStr);
        baseUpdater.AddInfo(@$"本次更新结束于 {DateTime.Now} .");
        if (bSoftwareUpdateErrorFound)
        {
            ShowErrorInfo("更新出错");
            //StopUpdate("遇到严重的问题，请与开发商联系！");
        }
        else
            StopUpdate("本次更新结束，请重新启动仪器！");
    }

    void Timer1_Tick(object sender, EventArgs e)
    {
        UpdateTotalProgressBar();
    }

    void FullScreenForm_Shown(object sender, EventArgs e)
    {
        StartUpdate();
        _isuupdate = false;
    }

    /// <summary>
    ///     关闭程序按钮。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void button1_Click(object sender, EventArgs e)
    {
        if (_isuupdate)
            return;

        Application.Exit();
    }

    #region 成员

    HardwareVersionInfo? minPacketVersion;

    readonly Stopwatch stopwatchProcessBar = new();
    readonly Stopwatch ProcessStopwatch = new();
    long NeedTotalSeconds = 10;
    int FpgaUpdateItemCount;
    bool bSoftwareUpdateErrorFound;
    bool _isuupdate;

    /// <summary
    ///     更新中
    /// </summary>

    #endregion 成员
    public FullScreenForm()
    {
        //没实际使用 ljw
        // this.errorInfoCtrl = errorInfoCtrl;
        InitializeComponent();
        baseUpdater = new();
        baseUpdater.ShowErrorInfo += ShowErrorInfo;

    }
}
