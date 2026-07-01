// // ******************************************************************
// //       /\ /|       @File         BaseUpdater.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-07-04
// //      /  \\        @Modified      2024-07-04
// //    *(__\_\
// // ******************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Hardware.Driver;
using ScopeX.Updater.Base;
using WindowsDSO_Updater;
using static ScopeX.USBBridge.Tmc;

namespace ScopeX.Updater;

public class BaseUpdater
{
    #region 常量

    public const int PCIE_BOARD_INDEX = 0;
    public const int AWG_BOARD_INDEX = 11;
    public const int DELAY_GETID = 300;
    public const int ESTIMATED_TIME_ERASE = 300;
    public const int SECONDS_PER_MINUTE = 60;

    #endregion 常量

    #region 成员属性

    public IFpgaFlashUpdater? FpgaUpdater
    {
        get => fpgaUpdater;
    }
    IFpgaFlashUpdater? fpgaUpdater;
    readonly ReaderWriterLockSlim writerLogLockSlim = new();
    List<ImageBlock>? boardsVersion;
    UpdatePackage? updatePackage;
    public UpdatePackage? UpdatePackage
    {
        get => updatePackage;
        set => updatePackage = value;
    }

    bool isUpdatePackageValid
    {
        get => updatePackage is { Items.Count: > 0 };
    }

    /// <summary>
    /// 强制禁用外面版 LED
    /// </summary>
    public bool IsForceDisableOuterLed = false;
    private bool isNeedRefreshOuterLed
    {
        get
        {
            return !IsForceDisableOuterLed && isUpdatePackageValid && updatePackage?.ProductType == ProductType.JiHe_UPO7000L;
        }
    }

    /// <summary
    ///     更新中
    /// </summary>

    #endregion 成员属性

    #region 基础功能方法

    static string ConvertNowTimeToString()
    {
        DateTime now = DateTime.Now;
        return string.Format(now.Year + "." + now.Month.ToString().PadLeft(2, '0') + "." + now.Day.ToString().PadLeft(2, '0') + " " + now.Hour.ToString().PadLeft(2, '0') + ":" + now.Minute.ToString().PadLeft(2, '0') + ":" +
                             now.Second.ToString().PadLeft(2, '0'));
    }

    public void WriteLog(string message)
    {
        try
        {
            writerLogLockSlim.EnterWriteLock();
            File.AppendAllText(LogHelper.LogFilePath, ConvertNowTimeToString() + "=>" + message + Environment.NewLine);
        }
        catch
        {
            // ignored
        }
        finally
        {
            writerLogLockSlim.ExitWriteLock();
        }
    }
    public void AddInfo(string info)
    {
        Debug.WriteLine(info);
        WriteLog(info);
    }

    public void WriteInnoSetupMessageInfos(string boardName, bool updateOk)
    {
        string msg = string.Empty;
        if (updateOk)
        {
            msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateSuccessFormat"), boardName);
        }
        else
        {
            msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateFailedFormat"), boardName);
        }
        LogHelper.WriteInnoSetupMessageInfos(msg);
    }

    #endregion 基础功能方法

    #region UI接口

    public Action<string>? ShowErrorInfo;
    public volatile ErrorType LedShowErrorState = ErrorType.F_FireWare_Update_Failed_0002;
    volatile UpdateProcessState processState;
    public UpdateProcessState ProcessState
    {
        get => processState;
        set => processState = value;
    }
    public Task LedShowStateTask()
    {
        return Task.Run(() =>
        {
            if (!isNeedRefreshOuterLed)
            {
                return;
            }
            while (processState == UpdateProcessState.Updating)
            {
                McuComPortUpdater.SetOuterLedByUpdate(ErrorType.F_FireWare_Updating_0003);
                Thread.Sleep(750);
                McuComPortUpdater.SetOuterLedByUpdate(ErrorType.F_FireWare_Updating_0004);
            }
            if (processState == UpdateProcessState.Done)
            {
                McuComPortUpdater.SetOuterLedByUpdate(LedShowErrorState);
            }
        });
    }

    #endregion UI接口

    #region 升级核心

    public bool UpdateMcu_USBBridges(List<UpdateItem> updateUSBItems)
    {
        Init();
        Default.UsbtmcBridge_Open(null, null, null, true);

        int _USBInsideIndex = -1;
        int _USBOutIndex = -1;
        string? defultImgFileName = ConfigurationManager.AppSettings["USBDefaultImgFileName"];
        if (string.IsNullOrWhiteSpace(defultImgFileName))
        {
            defultImgFileName = "CyBootProgrammer3.0.img";
        }
#pragma warning disable CS8602
        string defaultImgPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, defultImgFileName);
#pragma warning restore CS8602
        if (!File.Exists(defaultImgPath))
        {
            AddInfo($"文件不存在:{defaultImgPath}");
            return false;
        }
        for (int i = 0; i < updateUSBItems.Count; i++)
        {
            if (updateUSBItems[i].TypeID == 0)
            {
                _USBInsideIndex = i;
            }
            if (updateUSBItems[i].TypeID == 1)
            {
                _USBOutIndex = i;
            }
        }
        //Action<bool, string> getResult = updateMsg;
        if (_USBInsideIndex != -1)
        {
            bool result = Default.StartUpdateImage(defaultImgPath, updateUSBItems[_USBInsideIndex].Content.ToList(), false, out string msg);
            if (result)
            {
                AddInfo("USB内-更新成功");
            }
            else
            {
                AddInfo($"USB内-更新失败:{msg}");
                return false;
            }
        }
        if (_USBOutIndex != -1)
        {
            bool result = Default.StartUpdateImage(defaultImgPath, updateUSBItems[_USBOutIndex].Content.ToList(), true, out string msg);
            if (result)
            {
                AddInfo("USB外-更新成功");
            }
            else
            {
                AddInfo($"USB外-更新失败:{msg}");
                return false;
            }
        }

        return true;
    }
    public Task<bool> UpdateMcu_Probe(List<UpdateItem> updateMcuItems)
    {
        return Task.Run(() =>
        {
            foreach (UpdateItem item in updateMcuItems)
            {
                AddInfo($"开始更新Mcu_Probe Vesion:{((ImageBlock)item.BaseInfo).Version}");
                bool bOK = ProbeMcuUpdater.DoUpdate(item, AddInfo);
                if (!bOK)
                    return false;
            }
            return true;
        });
    }
    public bool UpdateMcu_AnalogChannelFirmware(List<UpdateItem> updateMcuItems)
    {
        foreach (UpdateItem item in updateMcuItems)
        {
            AddInfo($"开始更新Mcu_AnalogChannel.Vesion:{((ImageBlock)item.BaseInfo).Version}");
            McuComPortUpdater.BindLogFunc(AddInfo);
            if (!McuComPortUpdater.DoUpdate(item, AddInfo))
                return false;
        }
        return true;
    }
    public async Task<bool> UpdateMcu_KeyboardFirmware(List<UpdateItem> updateMcuItems)
    {
        AddInfo($"开始更新Mcu_Keyboard.Vesion:{((ImageBlock)updateMcuItems[0].BaseInfo).Version}");
        Mcu_KeyboardFirmwareUpdater.BindLogFunc(AddInfo);
        //return Mcu_KeyboardFirmwareUpdater.DoUpdate(updateMcuItems, AddInfo);
        return await Mcu_KeyboardFirmwareUpdater.UpdateFormBinFile(updateMcuItems, AddInfo);
    }
    public bool UpdateFpgaFirware_AtFastMode(List<UpdateItem> updateFpgaItems)
    {
        //更新包转换
        Dictionary<Int32, FlashContentAndImageBlock> flashInfo = new Dictionary<Int32, FlashContentAndImageBlock>();
        foreach (UpdateItem item in updateFpgaItems)
        {
            FlashContentAndImageBlock flashContentAndImageBlock = new FlashContentAndImageBlock()
            {
                TypeID = item.TypeID,
                boardName = item.BoardName,
                content = item.Content,
                ImageBlocks = new List<ImageBlock>(),
                contentAtPcieDrrOffset = 0
            };
            flashInfo.Add(item.TypeID, flashContentAndImageBlock);
        }

        //执行快速更新
        return fpgaUpdater?.FastMode_UpdateIncludeFlash(flashInfo) ?? false;

    }
    public async Task<bool> UpdateFpgaFirmware(List<UpdateItem> updateFpgaItems)
    {
        if (fpgaUpdater == null)
        {
            return false;
        }

        #region 更新模式分类
        List<UpdateItem> newupdatermods = new List<UpdateItem>();
        List<UpdateItem> oldupdatermods = new List<UpdateItem>();
        List<int> oldupdatermodboardlist = new List<int>();
        if (FpgaFlashUpdaterMod.FFU_FastMod != fpgaUpdater.CheckFpgaFlashUpdaterMod(PCIE_BOARD_INDEX))
        {
            //PCIE非快速模式时,所有都必须安装低速模式进行
            oldupdatermods = updateFpgaItems;
            for (int i = 0; i < updateFpgaItems.Count; i++)
            {
                oldupdatermodboardlist.Add(updateFpgaItems[i].TypeID);
            }
        }
        else
        {
            for (int i = 0; i < updateFpgaItems.Count; i++)
            {
                if (updateFpgaItems[i].Type == UpdaterItemType.Fpga)
                {
                    //PCIE是快速模式时,WAG仍采用低速模式进行
                    if (updateFpgaItems[i].TypeID == 11/*AWG*/)
                    {
                        oldupdatermods.Add(updateFpgaItems[i]);
                        oldupdatermodboardlist.Add(updateFpgaItems[i].TypeID);
                    }
                    //PCIE是快速模式时,二级非快速模式FPGA仍采用低速模式进行
                    else if (FpgaFlashUpdaterMod.FFU_FastMod != fpgaUpdater.CheckFpgaFlashUpdaterMod(updateFpgaItems[i].TypeID))
                    {
                        if (fpgaUpdater.CheckIamgeIsGoldenAndApp(updateFpgaItems[i].TypeID, updateFpgaItems[i].Content))
                        {
                            AddInfo($"板卡{updateFpgaItems[i].TypeID} 普通模式下, 不允许更新含有Golden的固件, 跳过...");
                        }
                        else
                        {
                            oldupdatermods.Add(updateFpgaItems[i]);
                            oldupdatermodboardlist.Add(updateFpgaItems[i].TypeID);
                        }
                    }
                    else
                    {
                        newupdatermods.Add(updateFpgaItems[i]);
                    }
                }
            }
        }
        #endregion
        oldupdatermods = oldupdatermods.OrderBy(o => o.TypeID).ToList();
        fpgaUpdater.SetPacketBoardListOnOldUpdater(oldupdatermodboardlist);
        bool bOk = true;

        if (bOk && oldupdatermods.Count > 0)
        {
            AddInfo("普通更新模式...");

            #region 擦除

            AddInfo($"开始擦除...  StartTime: {DateTime.Now}");
            bOk = await Task.Run(() => fpgaUpdater!.FpgaContent_EraseSectors());


            if (!bOk)
            {
                fpgaUpdater?.Close();
                AddInfo("擦除失败! ");
                return false;
            }
            AddInfo($"擦除完成  EndTime: {DateTime.Now}");

            #endregion

            #region 烧写

            for (int itemIndex = 0; itemIndex < oldupdatermods.Count; itemIndex++)
            {
                UpdateItem pktItem = oldupdatermods[itemIndex];
                // del 23.3.8
                //SoftwareVersionInfo softwareVersionInfo = new()
                //{
                //    MaxSoftVersion = pktItem.MaxDevVer,
                //    MinSoftVersion = pktItem.MinDevVer
                //};
                //var version = pktItem.VersionStr.Split('.');
                //FpgaFlashVersionInfo fpgaFlashVersionInfo = new(softwareVersionInfo)
                //{
                //    BuildTime = pktItem.LastWriteDateTime,
                //    Comment = pktItem.Comment,
                //    Designer = pktItem.Designer,
                //    MaxNum = int.Parse(version[0]),
                //    SubNum = int.Parse(version[1]),
                //    MinNum = int.Parse(version[2]),
                //    SoftwareVersionInfo = softwareVersionInfo,
                //    WriteTime = DateTime.Now,
                //};
                AddInfo($"[{pktItem.BoardName}] 开始更新,大小:{pktItem.Content.Length}");
                Stopwatch writeOneStopwatch = new();
                writeOneStopwatch.Start();
                FpgaFlashErrorCode result = fpgaUpdater!.FpgaContent_Write(pktItem.TypeID, pktItem.Content, new List<ImageBlock> { (ImageBlock)pktItem.BaseInfo });

                writeOneStopwatch.Stop();
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    AddInfo($"[{pktItem.BoardName}] 更新失败,错误:{result}");
                    bOk = false;
                    break;
                }
                string msg = string.Format(LanguageManger.Instance.GetIDMessage("UpdateSuccessFormat"), pktItem.BoardName);
                LogHelper.WriteInnoSetupMessageInfos(msg);

                AddInfo($"[{pktItem.BoardName}]更新完成，花费时间:{writeOneStopwatch.ElapsedMilliseconds / 1000}s。");

                Thread.Sleep(500);
                if (updatePackage.ProductType == ProductType.JiHe_UPO7000L)
                {
                    Thread.Sleep(1500);
                }
            }
            #endregion
        }

        if (bOk && newupdatermods.Count > 0)
        {
            AddInfo("快速更新模式...");
            bOk = UpdateFpgaFirware_AtFastMode(newupdatermods);
        }

        fpgaUpdater!.Close();
        return bOk;
    }
    public async Task<bool> UpdateSoftware()
    {
        return await Task.Run(() =>
        {
            foreach (UpdateItem? item in updatePackage!.Items)
            {
                if (item.Type == UpdaterItemType.Software)
                {
                    Thread.Sleep(100);
                }
            }
            return true;
        });
    }

    /// <summary>
    ///     是否需要更新
    /// </summary>
    /// <param name="pkgVer">包版本</param>
    /// <param name="hardVer">硬件版本</param>
    /// <returns></returns>
    bool IsVersionNeedUpDate(HardwareVersionInfo pkgVer, HardwareVersionInfo? hardVer)
    {
        return pkgVer.CompareTo(hardVer) > 0;
    }

    bool InitHardware()
    {
        if (updatePackage == null)
        {
            AddInfo("升级包为空");
            return false;
        }

        List<int> pkgBoardsList = new();
        foreach (UpdateItem? item in updatePackage.Items)
        {
            if (item.Type == UpdaterItemType.Fpga)
            {
                pkgBoardsList.Add(item.TypeID);
            }
        }

        pkgBoardsList = pkgBoardsList.Distinct().ToList();

        IFpgaFlashUpdater? _fpgaUpdater = FpgaFlashUpdaterFactory.FindUpdater(updatePackage.ProductType, pkgBoardsList);
        if (_fpgaUpdater == null)
        {
            AddInfo("硬件初始化 失败；未能创建FpgaFlashUpdater对象}");
            return false;
        }

        fpgaUpdater = _fpgaUpdater;
        fpgaUpdater.BindLogFunc(AddInfo, WriteInnoSetupMessageInfos);
        fpgaUpdater.BindLogConf(LogHelper.LogFilePath);

        bool result = fpgaUpdater.Open(updatePackage, "", out string msg);
        if (!result)
        {
            AddInfo($"硬件初始化 失败,{msg}");
            return false;
        }
        else
        {
            AddInfo("硬件初始化 成功");
            return true;
        }
    }

    bool ReadFPGAVersion(int boardIndex, out string? fpgaVersion)
    {
        if (fpgaUpdater != null) return fpgaUpdater.ReadFpgaInsideVersionInfo(boardIndex, out fpgaVersion);
        fpgaVersion = null;
        return false;
    }

    public bool CheckHardwave(bool mustHaveFpga, out List<UpdateItem> updateFpgaItems)
    {
        #region 硬件初始化

        bool skipAwg = false;
        AddInfo("========初始化....");

        updateFpgaItems = new List<UpdateItem>();

        if (mustHaveFpga)
        {
            bool result = InitHardware();

            if (!result)
            {
                AddInfo("InitHardware 初始化上电失败！");
                ShowErrorInfo?.Invoke("更新出错");
                processState = UpdateProcessState.HaveError;
                return false;
            }

            if (fpgaUpdater == null)
            {
                AddInfo("硬件初始化 失败；未能创建FpgaFlashUpdater对象}");
                return false;
            }

            result = fpgaUpdater!.VerifyFlashID(PCIE_BOARD_INDEX);
            if (!result)
            {
                AddInfo("PCIe FLASHID 验证失败！");
                ShowErrorInfo?.Invoke("更新出错");
                processState = UpdateProcessState.HaveError;
                return false;
            }
        }
        else
        {

            if (updatePackage.ProductType == ProductType.JiHe_UPO7000L)
            {
                McuComPortUpdater.UpdateForceSetProductType(ProductType.JiHe_UPO7000L);
            }
            AddInfo("本次更新不包含FPGA");
        }
        AddInfo("========初始化完成");

        #endregion

        processState = UpdateProcessState.Updating;

        LedShowStateTask();

        #region 验证Flash
        foreach (UpdateItem item in updatePackage!.Items)
        {
            if (item.Type == UpdaterItemType.Fpga)
            {
                if (item.TypeID == AWG_BOARD_INDEX)
                {
                    if (!ReadFPGAVersion(AWG_BOARD_INDEX, out string? fpgaVersion))
                    {
                        continue;
                    }

                    if (fpgaVersion != null)
                    {
                        string[] versions = fpgaVersion.Split(".");
                        if (versions.Count() < 2)
                        {
                            continue;
                        }

                        if (boardsVersion == null)
                        {
                            boardsVersion = new List<ImageBlock>();
                        }
                        boardsVersion.Add(
                            new ImageBlock
                            {
                                BoardID = AWG_BOARD_INDEX,
                                FirmwareType = FirmwareType.AppImage,
                                Version = new HardwareVersionInfo
                                {
                                    Major = int.Parse(versions[0]),
                                    Minor = int.Parse(versions[1])
                                }
                            });
                    }
                }

                Thread.Sleep(2000);

                if (!fpgaUpdater!.VerifyFlashID(item.TypeID))
                {
                    if (item.TypeID == AWG_BOARD_INDEX)
                    {
                        skipAwg = true;
                        AddInfo(@$"Warning: AWG上电失败，更新 PCIE 后再试");
                        continue;
                    }
                    AddInfo($"FLASHID-{item.TypeID} 验证失败！");
                    ShowErrorInfo?.Invoke("更新出错");
                    processState = UpdateProcessState.HaveError;
                    return false;
                }
            }
        }
        #endregion

        if (mustHaveFpga)
        {
            if (boardsVersion == null || boardsVersion.Count == 0)
            {
                AddInfo("读取版本失败！重写版本信息");
            }

            AddInfo("开始版本验证");
            AddInfo("========版本比较....");
            #region 检测在线板卡
            foreach (UpdateItem item in updatePackage.Items)
            {
                if (item.Type == UpdaterItemType.Fpga)
                {
                    Thread.Sleep(DELAY_GETID);
                    //此处所有在线板卡
                    if (updateFpgaItems.FirstOrDefault(obj => obj.TypeID == item.TypeID) == null)
                    {
                        if (boardsVersion == null || boardsVersion.FirstOrDefault(board => board.BoardID == item.TypeID) != null)
                        {
                            if (item.TypeID == AWG_BOARD_INDEX && skipAwg)
                            {
                                continue;
                            }
                            updateFpgaItems.Add(item);
                        }
                        else if (item.BaseInfo is ImageBlock imageInfo)
                        {
                            //板卡信息无效
                            List<ImageBlock> boardVers = boardsVersion.Where(board => board.BoardID == imageInfo.BoardID
                                                                                      && board.FirmwareType == imageInfo.FirmwareType
                                                                                      && imageInfo.BoardID != AWG_BOARD_INDEX).ToList();

                            if (boardVers.Count == 0)
                            {
                                //除AWG外其他板卡
                                updateFpgaItems.Add(item);

                                continue;
                            }
                            updateFpgaItems.AddRange(from boardVersion in boardVers where IsVersionNeedUpDate(imageInfo.Version, boardVersion.Version) select item);
                        }
                    }
                }
                //else if (item.Type == UpdaterItemType.AWG)
                //{
                //	updateFpgaItems.Add(item);
                //}
            }
            #endregion 检测在线板卡


            AddInfo("========查询FLASH状态....");


            #region 查询解除写保护状态(一次)

            AddInfo("查询FLASH状态");
            Dictionary<int, uint>? status = fpgaUpdater?.QueryFlashsStatus();
            if (status == null)
            {
                return false;
            }

            List<int> protectedFlashId = new();
            foreach (KeyValuePair<int, uint> flashState in status)
            {
                AddInfo($"FLASH状态:Id:{flashState.Key}   State:0x{flashState.Value}");
                if (fpgaUpdater != null && fpgaUpdater.StatusIsLocked(flashState.Key, flashState.Value))
                {
                    AddInfo($"FLASH状态:Id:{flashState.Key}  已写保护，准备解除");
                    protectedFlashId.Add(flashState.Key);
                }
            }
            fpgaUpdater?.UnLockFlashProtect(protectedFlashId);

            #endregion

            Thread.Sleep(1000);

            #region 查询解除写保护状态(二次)
            AddInfo("========查询FLASH状态....");
            status = fpgaUpdater?.QueryFlashsStatus();
            if (status == null)
            {
                return false;
            }
            bool flashProtected = false;
            foreach (KeyValuePair<int, uint> flashState in status)
            {
                AddInfo($"FLASH状态:Id:{flashState.Key}   State:0x{flashState.Value}");
                if (fpgaUpdater != null && fpgaUpdater.StatusIsLocked(flashState.Key, flashState.Value))
                {
                    AddInfo($"FLASH状态:Id:{flashState.Key}  已写保护，解除失败");
                    flashProtected = true;
                }
            }
            if (flashProtected)
            {
                return false;
            }
            AddInfo("========查询FLASH状态 成功");

            #endregion
        }
        return true;
    }

    #endregion 升级核心
}
