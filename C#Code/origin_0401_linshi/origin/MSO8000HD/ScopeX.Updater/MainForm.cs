using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.Updater;
using ScopeX.Updater.Base;
using static ScopeX.USBBridge.Tmc;

#nullable enable

namespace WindowsDSO_Updater;

public partial class MainForm : Form
{
    BaseUpdater baseUpdater;
    readonly Stopwatch _processStopwatch = new();
    readonly Stopwatch _stopwatchProcessBar = new();
    bool _bSoftwareUpdateErrorFound;
    int _fpgaUpdateItemCount;
    long _needTotalSeconds = 10;
    HardwareVersionInfo? minPacketVersion;
    ErrorInfoCtrl? errorInfoCtrl;

    void ShowErrorInfo(string info)
    {
        _stopwatchProcessBar.Stop();
        UpdateStatusInfo(info);
        errorInfoCtrl = new ErrorInfoCtrl(info, LogHelper.LogFilePath);
        panelMain.Controls.Clear();
        panelMain.Controls.Add(errorInfoCtrl);
    }
    void MainForm_Load(object sender, EventArgs e)
    {


        minPacketVersion = UpdateBaseHelper.GetVersionFromStr(UpdaterBaseConstants.LAST_SUPPORT_PACKAGE_VERSION);

        FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        LableTitle.Text += $@"    {fileInfo.FileVersion}";
    }

    void ButtonOpenUpdatePackageFile_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() != DialogResult.OK)
            return;
        textBoxUpdatePackageFileName.Text = openFileDialog1.FileName;
        BtnStartUpdate.Enabled = true;
    }
    void StopUpdate(string message)
    {
        Ctrl_Enable(true);
        _stopwatchProcessBar.Stop();

        if (message == "") return;
        baseUpdater.WriteLog(message);
        MessageBox.Show(this, message);
    }

    void UpdateTotalProgressBar()
    {
        progressBarStep.Invoke(() =>
        {
            int elapsedSeconds = (int)(_processStopwatch.ElapsedMilliseconds / 1000);
            if (elapsedSeconds > progressBarStep.Maximum)
                elapsedSeconds = progressBarStep.Maximum;
            progressBarStep.Value = elapsedSeconds;
            progressBarStep.Update();
        });
        int currValue = (int)(_stopwatchProcessBar.ElapsedMilliseconds / 1000);
        labelActualUsedTime.Text = (currValue / BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0') + @":" + (currValue % BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0');
    }

    void Ctrl_Enable(bool enable)
    {
        BeginInvoke(() =>
        {
            foreach (object? ctrl in Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = enable;
                }
            }
            progressBarStep.Visible = !enable;
            panelTime.Visible = !enable;
            if (!enable)
            {
                progressBarStep.Maximum = (int)_needTotalSeconds;
                _stopwatchProcessBar.Restart();
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

    async void BtnStartUpdate_Click(object sender, EventArgs e)
    {
        #region 界面重置与数据检查
        UpdateStatusInfo();
        progressBarStep.Value = 0;

        if (textBoxUpdatePackageFileName.Text == "")
        {
            MessageBox.Show(this, @"请先选择[更新包文件]！");
            return;
        }

        string currPackageFileName = textBoxUpdatePackageFileName.Text.Trim();
        baseUpdater.UpdatePackage = UpdatePackage.Load(currPackageFileName);
        if (baseUpdater.UpdatePackage?.Items == null || baseUpdater.UpdatePackage.Items.Count == 0)
        {
            MessageBox.Show(this, @"[更新包文件]有误！");
            return;
        }

        if (minPacketVersion != null && baseUpdater.UpdatePackage.PackageVersion.CompareTo(minPacketVersion) < 0)
        {
            MessageBox.Show(this, @$"[更新包文件]版本过低，支持的最低版本：[{minPacketVersion}]！");
            return;
        }
        toolStripStatusLabelUsedSecond.Text = "";
        _bSoftwareUpdateErrorFound = false;
        #endregion

        #region 更新统计与硬件检查
        _fpgaUpdateItemCount = 0;
        List<UpdateItem> mcuUpdateItems_AnalogChannel = new();
        List<UpdateItem> mcuUpdateItems_Probe = new();
        List<UpdateItem> McuUpdateItems_Keyboard = new();
        List<UpdateItem> USBUpdateItems_Bridge = new();
        foreach (UpdateItem? item in baseUpdater.UpdatePackage.Items)
        {
            if (item.Type == UpdaterItemType.Fpga)
            {
                _fpgaUpdateItemCount++;
            }
            else if (item.Type == UpdaterItemType.Mcu_AnalogChannel)
            {
                mcuUpdateItems_AnalogChannel.Add(item);
            }
            else if (item.Type == UpdaterItemType.Mcu_Keyboard)
            {
                McuUpdateItems_Keyboard.Add(item);
            }
            else if (item.Type == UpdaterItemType.USBBridge)
            {
                USBUpdateItems_Bridge.Add(item);
            }
            else if (item.Type == UpdaterItemType.Probe)
            {
                mcuUpdateItems_Probe.Add(item);
            }
        }

        labelCalculateNeedUsingTime.Text = @"计算中...";
        BtnStartUpdate.Enabled = false;

        List<UpdateItem> updateFpgaItems;
        bool mustHaveFpga = _fpgaUpdateItemCount > 0 || mcuUpdateItems_AnalogChannel.Count > 0;
        {
            if (!baseUpdater.CheckHardwave(mustHaveFpga, out updateFpgaItems))
            {
                baseUpdater.FpgaUpdater!.Close();
                baseUpdater.AddInfo("初始化上电失败！");
                ShowErrorInfo("更新出错");
                BtnStartUpdate.Enabled = true;
                return;
            }
        }
        #endregion

        #region 更新时间与耗时确认
        _needTotalSeconds = 0;

        if (updateFpgaItems.Count > 0)
        {
            _needTotalSeconds = 5;
            _needTotalSeconds += BaseUpdater.ESTIMATED_TIME_ERASE;
            foreach (UpdateItem item in updateFpgaItems)
            {
                long pages = ((item.TotalBytes + 256) - 1) / 256;
                int baseTimeNeed = baseUpdater.FpgaUpdater!.PerWriteRegUsedNs == 0 ? 3800 : baseUpdater.FpgaUpdater!.PerWriteRegUsedNs;
                _needTotalSeconds += (pages * ((256 * 4) + 28) * baseTimeNeed) / 1000_000_000; //1000_000_000,ns=>s,写入需要的时间(seconds)
                _needTotalSeconds += (item.TotalBytes * 3L * baseTimeNeed) / 1000_000_000; //1000_000_000,ns=>s,验证需要的时间
                if (item.TypeID != BaseUpdater.AWG_BOARD_INDEX)
                {
                    _needTotalSeconds += 60; //烧写版本信息花费的时间
                    _needTotalSeconds += 70; //修正
                }
            }
        }
        if (McuUpdateItems_Keyboard.Count > 0)
        {
            _needTotalSeconds += 35;
        }
        if (USBUpdateItems_Bridge.Count > 0)
        {
            _needTotalSeconds += 240 * USBUpdateItems_Bridge.Count;
        }
        if (mcuUpdateItems_AnalogChannel.Count > 0)
        {
            _needTotalSeconds += 90;
        }
        _needTotalSeconds += baseUpdater.UpdatePackage.Items.Count - _fpgaUpdateItemCount;
        if (_needTotalSeconds > 0 && baseUpdater.UpdatePackage.ProductType == ProductType.JiHe_UPO7000L)
        {
            _needTotalSeconds = (int)(_needTotalSeconds * 1.4);
        }
        progressBarStep.Maximum = (int)_needTotalSeconds;
        labelCalculateNeedUsingTime.Text = (_needTotalSeconds / BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0') + @":" + (_needTotalSeconds % BaseUpdater.SECONDS_PER_MINUTE).ToString().PadLeft(2, '0');
        labelCalculateNeedUsingTime.Visible = true;
        panelTime.Visible = true;

        string message = "在更新期间，请确保不要断电！";
        if (_fpgaUpdateItemCount > 0)
        {
            message += Environment.NewLine + "(本次更新需要较长的时间！)";
        }
        message += Environment.NewLine + "您确认需要更新吗？";
        if (MessageBox.Show(message, @"提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            if (_fpgaUpdateItemCount > 0)
            {
                baseUpdater.FpgaUpdater!.Close();
            }
            baseUpdater.AddInfo("取消了更新！");
            Ctrl_Enable(true);
            BtnStartUpdate.Enabled = true;
            return;
        }
        UpdateStatusInfo("更新中...请勿[关闭程序]或[使设备断电]");
        _processStopwatch.Restart();
        Ctrl_Enable(false);
        #endregion

        #region 更新固件
        if (false == _bSoftwareUpdateErrorFound && updateFpgaItems.Count > 0)
        {
            try
            {
                bool bFpgaUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateFpgaFirmware(updateFpgaItems));
                if (!bFpgaUpdateErrorFound)
                {
                    _bSoftwareUpdateErrorFound = true;
                    BtnStartUpdate.Enabled = true;

                    baseUpdater.AddInfo("[FPGA]更新出错");
                    ShowErrorInfo("[FPGA]更新出错");
                }
                else
                {
                    baseUpdater.AddInfo("[FPGA]更新成功");
                }
            }
            catch (Exception ex)
            {
                _bSoftwareUpdateErrorFound = true;
                baseUpdater.AddInfo("[FPGA]更新异常");
                baseUpdater.AddInfo(ex.ToString());
                ShowErrorInfo("[FPGA]更新异常");
            }
        }

        if (false == _bSoftwareUpdateErrorFound && mcuUpdateItems_Probe.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await baseUpdater.UpdateMcu_Probe(mcuUpdateItems_Probe);
                if (!bUpdateErrorFound)
                {
                    _bSoftwareUpdateErrorFound = true;
                    BtnStartUpdate.Enabled = true;
                    baseUpdater.AddInfo("[探头]更新出错");
                    ShowErrorInfo("[探头]更新出错");
                }
                else
                {
                    baseUpdater.AddInfo("[探头]更新成功");
                }
            }
            catch (Exception ex)
            {
                _bSoftwareUpdateErrorFound = true;
                baseUpdater.AddInfo("[探头]更新异常");
                baseUpdater.AddInfo(ex.ToString());
                ShowErrorInfo("[探头]更新异常");
            }
        }

        if (false == _bSoftwareUpdateErrorFound && mcuUpdateItems_AnalogChannel.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_AnalogChannelFirmware(mcuUpdateItems_AnalogChannel));
                if (!bUpdateErrorFound)
                {
                    _bSoftwareUpdateErrorFound = true;
                    BtnStartUpdate.Enabled = true;
                    baseUpdater.AddInfo("[通道]更新出错");
                    ShowErrorInfo("[通道]更新出错");
                    return;
                }
                else
                {
                    baseUpdater.AddInfo("[通道]更新成功");
                }
            }
            catch (Exception ex)
            {
                baseUpdater.AddInfo("[通道]更新异常");
                baseUpdater.AddInfo(ex.ToString());
                ShowErrorInfo("[通道]更新出错");
                _bSoftwareUpdateErrorFound = true;
            }
        }

        if (false == _bSoftwareUpdateErrorFound && McuUpdateItems_Keyboard.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(async () => await baseUpdater.UpdateMcu_KeyboardFirmware(McuUpdateItems_Keyboard));
                if (!bUpdateErrorFound)
                {
                    _bSoftwareUpdateErrorFound = true;
                    BtnStartUpdate.Enabled = true;
                    baseUpdater.AddInfo("[键盘板]更新出错");
                    ShowErrorInfo("[键盘板]更新出错");
                    return;
                }
                else
                {
                    baseUpdater.AddInfo("[键盘板]更新成功");
                }
            }
            catch (Exception ex)
            {
                _bSoftwareUpdateErrorFound = true;
                baseUpdater.AddInfo("[键盘板]更新异常");
                baseUpdater.AddInfo(ex.ToString());
                ShowErrorInfo("[键盘板]更新异常");
            }
        }

        if (false == _bSoftwareUpdateErrorFound && USBUpdateItems_Bridge.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_USBBridges(USBUpdateItems_Bridge));
                if (!bUpdateErrorFound)
                {
                    _bSoftwareUpdateErrorFound = true;
                    BtnStartUpdate.Enabled = true;
                    baseUpdater.AddInfo("[USB]更新出错");
                    ShowErrorInfo("[USB]更新出错");
                    return;
                }
                else
                {
                    baseUpdater.AddInfo("[USB]更新成功");
                }
            }
            catch (Exception ex)
            {
                _bSoftwareUpdateErrorFound = true;
                baseUpdater.AddInfo("[USB]更新异常");
                baseUpdater.AddInfo(ex.ToString());
                ShowErrorInfo("[USB]更新异常");
            }
        }
        #endregion

        #region 更新主程序
        _bSoftwareUpdateErrorFound = false;
        if ((baseUpdater.UpdatePackage.Items.Count
            - _fpgaUpdateItemCount
            - mcuUpdateItems_AnalogChannel.Count
            - McuUpdateItems_Keyboard.Count
            - USBUpdateItems_Bridge.Count
            - mcuUpdateItems_Probe.Count) > 0)
        {
            _bSoftwareUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateSoftware());
        }

        string timeInfoStr = $"本次更新花费时间 {_processStopwatch.ElapsedMilliseconds / 1000} s.";
        baseUpdater.AddInfo(timeInfoStr);
        UpdateStatusInfo(timeInfoStr);
        baseUpdater.AddInfo($"本次更新结束于 {DateTime.Now} .");
        if (_bSoftwareUpdateErrorFound)
        {
            ShowErrorInfo("更新出错");
        }
        else
        {
            StopUpdate("本次更新结束，请重新启动仪器！");
        }
        if (!baseUpdater.IsForceDisableOuterLed && Constants.PRODUCT == ProductType.JiHe_UPO7000L)
        {
            //等待灯变色
            Thread.Sleep(1500);
        }
        BtnStartUpdate.Enabled = true;
        #endregion
    }

    void BtnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    void Timer1_Tick(object sender, EventArgs e)
    {
        UpdateTotalProgressBar();
    }

    void MainForm_KeyPress(object sender, KeyPressEventArgs e)
    {
    }


    #region 成员


    #endregion 成员
    public MainForm()
    {
        InitializeComponent();
        baseUpdater = new BaseUpdater();
        baseUpdater.ShowErrorInfo += ShowErrorInfo;
    }

    #region 初始化  读版本

    // static string GetVersionStr(HardwareVersionInfo version)
    // {
    //     return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    // }
    // void PrintVersionInfo()
    // {
    //     if (boardsVersion == null)
    //     {
    //         return;
    //     }
    //     baseUpdater.AddInfo("版本信息:");
    //     baseUpdater.AddInfo("=====================");
    //     foreach (ImageBlock boardVer in boardsVersion)
    //     {
    //         baseUpdater.AddInfo($"{boardVer.FirmwareType} - {GetVersionStr(boardVer.Version)}");
    //     }
    //     baseUpdater.AddInfo("=====================");
    //
    // }


    #endregion 初始化  读版本

    #region 窗口移动

    bool _mouseIsDown;
    Point _mouseOff; //鼠标移动位置变量

    void PanelTitle_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {

            _mouseOff = new Point(-e.X, -e.Y);
            _mouseIsDown = true;
        }
    }

    void PanelTitle_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _mouseIsDown = false;
        }
    }

    void PanelTitle_MouseMove(object sender, MouseEventArgs e)
    {
        if (_mouseIsDown)
        {
            Point mouseSet = MousePosition;
            mouseSet.Offset(_mouseOff.X, _mouseOff.Y);
            Location = mouseSet;
        }
    }

    #endregion

    // void textBoxbaseUpdater.UpdatePackageFileName_KeyDown(object sender, KeyEventArgs e)
    // {
    //     if (e.KeyCode == Keys.F10)
    //     {
    //         labelLastModifyDate.Visible = !labelLastModifyDate.Visible;
    //     }
    // }
}
