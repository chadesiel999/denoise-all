using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Updater.Base;

namespace ScopeX.Updater;

/// <summary>
///     增加命令模式，以便能通过Dos命令模式被第三方程序调用来安装固件。代码从FullScreenForm拷贝
/// </summary>
internal class CommandLineMode
{
    static internal CommandLineMode Instance = new();

    HardwareVersionInfo _minPacketVersion;

    readonly BaseUpdater baseUpdater;
    int FpgaUpdateItemCount;

    /// <summary>
    ///     是否是静默执行模式(不展示UI界面)
    /// </summary>
    internal bool IsQuiet = false;

    /// <summary>
    /// 强制禁用外面版 LED
    /// </summary>
    internal bool IsForceDisableOuterLed = false;
    /// <summary>
    ///     命令行模式下的upd文件集合
    /// </summary>
    internal List<string> UpdFilePaths;
    CommandLineMode()
    {
        baseUpdater = new BaseUpdater();

    }

    HardwareVersionInfo minPacketVersion
    {
        get
        {
            if (_minPacketVersion == null)
                _minPacketVersion = UpdateBaseHelper.GetVersionFromStr(UpdaterBaseConstants.LAST_SUPPORT_PACKAGE_VERSION);

            return _minPacketVersion;
        }
    }

    /// <summary>
    ///     执行安装。
    /// </summary>
    internal bool Install()
    {
        if (UpdFilePaths == null)
            return false;

        // 将相对路径转换为绝对路径
        for (int i = 0; i < UpdFilePaths.Count; i++)
        {
            string file = UpdFilePaths[i];
            if (!Path.IsPathRooted(file))
                if (file != null)
                    UpdFilePaths[i] = Path.Combine(Directory.GetCurrentDirectory(), file);
            LogHelper.WriteLog($"检测到upd文件：{UpdFilePaths[i]}");
        }

        List<string> updFiles = UpdFilePaths.Where(File.Exists).ToList();
        if (!updFiles.Any())
        {
            LogHelper.WriteLog("检测到0个有效upd文件");
            return false;
        }

        foreach (string updfile in updFiles)
        {
            try
            {
                InstallUpdAsync(updfile).Wait();
            }
            catch (Exception ex)
            {
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("InstallUpdFailed", defaultval: "安装：{0}失败"), updfile);
                LogHelper.WriteInnoSetupMessageInfos(msg);
                LogHelper.WriteLog($"安装Upd文件：{updfile}时异常：{ex}");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///     安装单个UPD文件
    /// </summary>
    /// <param name="updfile"></param>
    /// <exception cref="Exception"></exception>
    async Task InstallUpdAsync(string updfile)
    {
        LogHelper.WriteLog($"读取文件:({updfile})");
        baseUpdater.UpdatePackage = UpdatePackage.Load(updfile);
        baseUpdater.IsForceDisableOuterLed = IsForceDisableOuterLed;
        if (baseUpdater.UpdatePackage == null || baseUpdater.UpdatePackage.Items == null || baseUpdater.UpdatePackage.Items.Count == 0)
            throw new Exception("[更新包文件]有误！");

        if (baseUpdater.UpdatePackage.PackageVersion.CompareTo(minPacketVersion) < 0)
            throw new Exception($"[更新包文件]版本过低，支持的最低版本：[{minPacketVersion}]！");

        FpgaUpdateItemCount = 0;
        List<UpdateItem> mcuUpdateItems_AnalogChannel = new();
        List<UpdateItem> mcuUpdateItems_Keyboard = new();
        List<UpdateItem> usbUpdateItems_Bridge = new();
        foreach (UpdateItem item in baseUpdater.UpdatePackage.Items)
        {
            if (item.Type == UpdaterItemType.Fpga)
                FpgaUpdateItemCount++;
            else if (item.Type == UpdaterItemType.Mcu_AnalogChannel)
                mcuUpdateItems_AnalogChannel.Add(item);
            else if (item.Type == UpdaterItemType.Mcu_Keyboard)
                mcuUpdateItems_Keyboard.Add(item);
            else if (item.Type == UpdaterItemType.USBBridge)
                usbUpdateItems_Bridge.Add(item);
        }

        LogHelper.WriteLog("初始化FPGA");
        List<UpdateItem> updateFpgaItems = null;
        bool mustHaveFpga = FpgaUpdateItemCount > 0 || mcuUpdateItems_AnalogChannel.Count > 0;
        bool success = await Task.Run(() => baseUpdater.CheckHardwave(mustHaveFpga, out updateFpgaItems));
        if (!success || updateFpgaItems == null)
        {
            baseUpdater.FpgaUpdater!.Close();
            LogHelper.WriteLog("初始化上电失败！");
            LogHelper.WriteInnoSetupMessageInfos(LanguageManger.Instance.GetIDMessage("TheInitialPowerOnFailed", defaultval: "初始化上电失败！"));
            return;
        }

        if (updateFpgaItems.Count > 0)
        {
            try
            {
                bool bFpgaUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateFpgaFirmware(updateFpgaItems));
                if (!bFpgaUpdateErrorFound)
                {
                    LogHelper.WriteLog("[FPGA]更新出错");
                    baseUpdater.ProcessState = UpdateProcessState.HaveError;
                    baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog($"[FPGA]更新出错:{e}");
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), "FPGA");
                LogHelper.WriteInnoSetupMessageInfos(msg);
                baseUpdater.ProcessState = UpdateProcessState.HaveError;
                baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
            }
            finally
            {
                baseUpdater.FpgaUpdater?.Close();
            }
        }

        if (mcuUpdateItems_AnalogChannel.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_AnalogChannelFirmware(mcuUpdateItems_AnalogChannel));
                if (!bUpdateErrorFound)
                {
                    LogHelper.WriteLog("[通道]更新出错");
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), LanguageManger.Instance.GetIDMessage("Channel"));
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                    baseUpdater.ProcessState = UpdateProcessState.HaveError;
                    baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
                }
                else
                {
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateSuccessFormat"), LanguageManger.Instance.GetIDMessage("Channel"));
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog($"[通道]更新出错:{e}");
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), LanguageManger.Instance.GetIDMessage("Channel"));
                LogHelper.WriteInnoSetupMessageInfos(msg);
                baseUpdater.ProcessState = UpdateProcessState.HaveError;
                baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
            }
        }

        if (mcuUpdateItems_Keyboard.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_KeyboardFirmware(mcuUpdateItems_Keyboard));
                if (!bUpdateErrorFound)
                {
                    LogHelper.WriteLog("[键盘板]更新出错");
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), LanguageManger.Instance.GetIDMessage("KeyBoard"));
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                    baseUpdater.ProcessState = UpdateProcessState.HaveError;
                    baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
                }
                else
                {
                    LogHelper.WriteLog("[键盘板]更新完毕");
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateSuccessFormat"), LanguageManger.Instance.GetIDMessage("KeyBoard"));
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog($"[键盘板]更新出错:{e}");
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), LanguageManger.Instance.GetIDMessage("KeyBoard"));
                LogHelper.WriteInnoSetupMessageInfos(msg);
                baseUpdater.ProcessState = UpdateProcessState.HaveError;
                baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
            }
        }
        if (usbUpdateItems_Bridge.Count > 0)
        {
            try
            {
                bool bUpdateErrorFound = await Task.Run(() => baseUpdater.UpdateMcu_USBBridges(usbUpdateItems_Bridge));
                if (!bUpdateErrorFound)
                {
                    LogHelper.WriteLog("[USB]更新出错");
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), "USB");
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                    baseUpdater.ProcessState = UpdateProcessState.HaveError;
                    baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
                }
                else
                {
                    string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateSuccessFormat"), "USB");
                    LogHelper.WriteInnoSetupMessageInfos(msg);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog($"[USB]更新出错：{e}");
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), "USB");
                LogHelper.WriteInnoSetupMessageInfos(msg);
                baseUpdater.ProcessState = UpdateProcessState.HaveError;
                baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
            }
        }
        baseUpdater.ProcessState = UpdateProcessState.Done;
        baseUpdater.LedShowErrorState = ErrorType.F_FireWare_Update_Success_0001;

        if (!baseUpdater.IsForceDisableOuterLed && Constants.PRODUCT == ProductType.JiHe_UPO7000L)
        {
            //等待灯变色
            Thread.Sleep(1500);
        }

        LogHelper.WriteLog("本次更新完毕");
    }
}
