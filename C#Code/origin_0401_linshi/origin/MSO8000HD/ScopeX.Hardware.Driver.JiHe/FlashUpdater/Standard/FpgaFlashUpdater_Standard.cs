using Microsoft.Win32;
using PCIeControl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExt;
using ScopeX.Updater.Base;
using ScopeX.Updater.Base.Infos;
using ScopeX.USBBridge;
using System.Reflection.Metadata;
using System.IO;
using System.Text.RegularExpressions;
using ScopeX.USBTMC;

namespace ScopeX.Hardware.Driver
{
    public class FpgaFlashUpdaterStandard : IFpgaFlashUpdater
    {
        private String LogfilePath = String.Empty;


        private Action<String>? AddLog; //日志输出
        public void WriteLog(String message)
        {
            AddLog?.Invoke(message);
        }

        private Action<String, Boolean>? InnoSetupTip;//安装弹出
        public void WriteInnoSetupMessageInfos(String boardName, Boolean updateOk)
        {
            InnoSetupTip?.Invoke(boardName, updateOk);
        }

        #region old
        private Boolean _bIncludeAwgBoard = false;
        private readonly ProductType _currProductType;
        RootFpgaFlash? _rootFpgaFlash = null;
        private List<Int32> _packetBoardIndexList { init; get; }
        private List<Int32>? _packetBoardIndexListOfOldUpdater = null;
        public void SetPacketBoardListOnOldUpdater(List<Int32>? boardList)
        {
            _packetBoardIndexListOfOldUpdater = boardList;
        }

        //private PCIe pcieCtrl { get; set; }
        public UInt32 FLASHID => _flashId;
        private Dictionary<SystemFPGAConstituteDefine, FpgaFlash> _fpgaFlashDefine { get; }

        public Int32 PerWriteRegUsedNs { get; set; }

        private UInt32 _flashId;
        private Boolean ExistsFlash(Int32 boardIndex)
        {
            return (_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)boardIndex));
        }
        #region 接口实现

        public Boolean ReadFpgaInsideVersionInfo(Int32 boardIndex, out String? infos)
        {
            infos = null;
            if (!ExistsFlash(boardIndex))
                return false;
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            return flash.ReadFpgaInsideVersionInfo(out infos);
        }
        internal FpgaFlashUpdaterStandard(ProductType productType, List<Int32> packetBoardList, Dictionary<SystemFPGAConstituteDefine, FpgaFlash> inFpgaFlashDefine)
        {
            _fpgaFlashDefine = inFpgaFlashDefine;
            _currProductType = productType;
            foreach (var flash in _fpgaFlashDefine)
            {
                if (flash.Value is RootFpgaFlash)
                {
                    _rootFpgaFlash = (RootFpgaFlash)flash.Value;
                    break;
                }
            }
            _packetBoardIndexList = packetBoardList;
        }
        public void BindLogFunc(Action<String>? logFunc, Action<String, Boolean>? setupTip = null)
        {
            AddLog = logFunc;
            InnoSetupTip = setupTip;

            foreach (var flash in _fpgaFlashDefine)
            {
                if (flash.Value is RootFpgaFlash)
                {
                    _rootFpgaFlash = (RootFpgaFlash)flash.Value;
                    if (_rootFpgaFlash.AddLog == null)
                    {
                        _rootFpgaFlash.AddLog += logFunc;
                    }
                }
            }
        }
        public void BindLogConf(String logfilePath)
        {
            LogfilePath = logfilePath;
        }

        public Boolean WriteUSBTMCSN(String sn)
        {
            //7000 或2.0硬件
            if (Tmc.Default != null)
            {
                return Tmc.Default.UsbtmcBridge_WriteSN(sn);
            }
            //8000 3.0硬件 
             if (TMC.Default != null)
            {
                return TMC.Default.WriteTMCSN(sn);
            }
            return true;
        }
        public Boolean Open(UpdatePackage? updatePackage, String key, out String errorMsg)
        {
            errorMsg = "";

            if (updatePackage == null)
            {
                return false;
            }

            Boolean bOk = Hd.Open(key, true);
            if (!bOk)
            {
                errorMsg = "打开PCIE驱动失败";
                return false;
            }

            Hd.HardwareWorkMode = HardwareWorkMode.HdWorkOnUpdater;
            Hd.CurrProductType = _currProductType;
            Hd.CreateProduct();
            Hd.SysLogger = new Action<String, String>((String msg, String lvl) =>
            {
                WriteLog($"[{lvl}] {msg}");
            });
            Boolean bIncludePcieBoard = true;
            Boolean bIncludeProcBoard = false;
            Boolean bIncludeAcqBoard = false;
            Boolean bIncludeAnalogChannel = false;
            foreach (var item in updatePackage.Items)
            {
                if (item.Type == UpdaterItemType.Fpga)
                {
                    if (item.TypeID == (Int32)SystemFPGAConstituteDefine.PCIE)
                        bIncludePcieBoard = true;
                    else if (item.TypeID == (Int32)SystemFPGAConstituteDefine.ProcessBoard_K7 || item.TypeID == (Int32)SystemFPGAConstituteDefine.ProcessBoard_S6)
                    {
                        bIncludePcieBoard = true;
                        bIncludeProcBoard = Hd.CurrProduct!.ProcBd != null;
                    }
                    else if (item.TypeID >= (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_1 && item.TypeID <= (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_8)
                    {
                        bIncludePcieBoard = true;
                        bIncludeProcBoard = Hd.CurrProduct!.ProcBd != null;
                        bIncludeAcqBoard = true;
                    }
                    else if (item.TypeID == (Int32)SystemFPGAConstituteDefine.AWG)
                    {
                        bIncludePcieBoard = true;
                        bIncludeProcBoard = Hd.CurrProduct!.ProcBd != null;
                        _bIncludeAwgBoard = true;

                    }
                }
                else if (item.Type == UpdaterItemType.Mcu_Keyboard)
                {
                }
                else if (item.Type == UpdaterItemType.Mcu_AnalogChannel)
                {
                    bIncludePcieBoard = true;
                    bIncludeProcBoard = true;
                    bIncludeAnalogChannel = true;
                }
                else
                {
                    bIncludePcieBoard = true;
                }

            }
            if (!bIncludePcieBoard)
            {
                return true;
            }

            //PCIE初始化
            {
                Hd.CurrProduct?.PcieBd?.Init();
                WriteLog($"PCIE 升级前版本{Hd.CurrProduct?.PcieBd?.FpgaVersion?.ToString()}");
            }
            if (bIncludeProcBoard)
            {
                Hd.CurrProduct?.ProcBd?.Init();
                WriteLog($"PROC 升级前版本{Hd.CurrProduct?.ProcBd?.FpgaVersion?.ToString()}");
            }

            if (bIncludeAcqBoard)
            {
                Hd.CurrProduct?.AcqBd?.Init();
                if (Hd.CurrProduct?.AcqBd?.AllFpgaVerionStr != null)
                {
                    WriteLog($"ACQx 升级前版本");
                    WriteLog(Hd.CurrProduct?.AcqBd?.AllFpgaVerionStr!);
                }
                else
                {
                    WriteLog($"ACQx 升级前版本 未检测到");
                }

            }

            //上电结果检查
            if (bIncludePcieBoard && !Hd.CurrProduct!.PcieBd!.IsPowerOk())
            {
                errorMsg = "Pcie Board Power Error!";
                return false;
            }
            if (bIncludeProcBoard && !Hd.CurrProduct!.ProcBd!.IsPowerOk)
            {
                errorMsg = "Proc Board Power Error!";
                return false;
            }
            if (bIncludeAcqBoard && !Hd.CurrProduct!.AcqBd!.IsAllPowerOk())
            {
                errorMsg = "Acq Board Power Error!";
                return false;
            }
            if (bIncludeAnalogChannel && !AbstractController_AnalogChannel.PowerOn(true))
            {
                errorMsg = "Chl Board Power Error!";
                return false;
            }

            if (_bIncludeAwgBoard)
            {
                AWG.PowerOn();
            }

            //22.9.17 MX25U128 PCIE第二次查状态才准确
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            flash.QueryStatus();
            return true;
        }


        public FpgaFlashErrorCode VerifyIDCode(Int32 boardIndex, Byte[] content)
        {
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;
            UInt32 searchHeaderBytes = 1024; //通过观察得到的，并没有找到文档支持
            if (searchHeaderBytes > content.Length)
            {
                searchHeaderBytes = (UInt32)content.Length;
            }
            String HeaderStr = "";
            for (Int32 i = 0; i < searchHeaderBytes; i++)
                HeaderStr += $"_{content[i].ToString("X").PadLeft(2, '0')}";
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            return HeaderStr.IndexOf(flash.Registers.IDCodeVerify, StringComparison.Ordinal) > 0 ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.InfoError;
        }

        public List<BaseDataBlock>? FpgaContent_ReadBoardData(UInt32 boardID, List<InfoIndex> infoIndexs)
        {
            foreach (var flashIndex in _packetBoardIndexList)
            {
                if (boardID == flashIndex && _fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)flashIndex))
                {
                    var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)flashIndex];

                    return flash.DataBlock_Read(infoIndexs);
                }
            }
            return null;
        }
        public List<ImageBlock> FpgaContent_ReadImageInfo()
        {
            List<ImageBlock> dataBlock = new();

            foreach (var flashIndex in _packetBoardIndexList)
            {
                if (_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)flashIndex))
                {
                    var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)flashIndex];
                    List<ImageBlock>? blocks = flash.ImageInfo_Read();
                    if (blocks != null)
                    {
                        for (Int32 i = 0; i < blocks.Count; i++)
                        {
                            dataBlock.Add(blocks[i]);
                        }
                    }
                }
            }
            return dataBlock;
        }

        public Boolean UnLockFlashProtect(List<UpdateItem> updateItem)
        {
            var list = new List<Int32>();
            updateItem.ForEach(item => { list.Add(item.TypeID); });
            return UnLockFlashProtect(list);
        }

        public Boolean LockFlashProtect(List<Int32>? fpgaIndexs)
        {
            if (fpgaIndexs == null)
            {
                return false;
            }

            foreach (var flashIndex in fpgaIndexs)
            {
                if (false == _fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)flashIndex))
                {
                    continue;
                }
                FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)flashIndex];
                if (flash == null)
                {
                    continue;
                }

                if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
                {
                    continue;
                }

                Boolean status = flash.LockFlashProtect();
                if (!status)
                {
                    return false;
                }
            }
            return true;
        }
        public Boolean UnLockFlashProtect(List<Int32>? fpgaIndexs)
        {
            if (fpgaIndexs == null)
            {
                return false;
            }

            foreach (var flashIndex in fpgaIndexs)
            {
                if (false == _fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)flashIndex))
                {
                    continue;
                }

                FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)flashIndex];
                if (flash == null)
                {
                    continue;
                }

                if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
                {
                    continue;
                }

                Boolean status = flash.UnLockFlashProtect();
                if (!status)
                {
                    return false;
                }
            }
            return true;
        }

        public Boolean StatusIsLocked(Int32 BoardIndex, UInt32 state)
        {
            if (!ExistsFlash(BoardIndex))
            {
                return false;
            }

            FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)BoardIndex];
            if (flash == null)
            {
                return false;
            }


            return (flash.GetFlashProtectMaskID() & state) != 0;
        }


        public Dictionary<Int32, UInt32> QueryFlashsStatus()
        {
            Dictionary<Int32, UInt32>? result = new Dictionary<Int32, UInt32>();
            foreach (var flashIndex in _packetBoardIndexList)
            {
                if (false == _fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)flashIndex))
                {
                    continue;
                }
                FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)flashIndex];
                if (flash == null)
                {
                    continue;
                }

                if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
                {
                    //
                }
                else
                {
                    UInt32 status = flash.QueryStatus();
                    result.Add(flashIndex, status);
                }
            }
            return result;
        }
        private async Task<Boolean> DoEraseSectors()
        {
            if (_packetBoardIndexList == null)
            {
                return true;
            }
            List<Task<FpgaFlashErrorCode>> allEraseTasks = new();
            foreach (var boardIndex in _packetBoardIndexList)
            {
                if (_packetBoardIndexListOfOldUpdater != null && !_packetBoardIndexListOfOldUpdater.Contains(boardIndex))
                {
                    //或者走新更新路线的 PCIE PROC ACQ 也不需要擦除
                    continue;
                }

                IEnumerable<KeyValuePair<SystemFPGAConstituteDefine, FpgaFlash>>? flash = _fpgaFlashDefine.Where(fpga => fpga.Key == (SystemFPGAConstituteDefine)boardIndex);
                IEnumerable<KeyValuePair<SystemFPGAConstituteDefine, FpgaFlash>> keyValuePairs = flash.ToList();
                if (!keyValuePairs.Any())
                {
                    continue;
                }
                allEraseTasks.Add(keyValuePairs.ToList()[0].Value.Content_EraseSectorsAsync(5 * 60));
            }

            IEnumerable<FpgaFlashErrorCode> eraseResults = await Task.WhenAll(allEraseTasks);
            Int32 errorCount = eraseResults.Count(m => m != FpgaFlashErrorCode.Succeed);
            return errorCount == 0;
        }
        public async Task<Boolean> FpgaContent_EraseSectors()
        {
            var doErase = await DoEraseSectors();
            return doErase;
        }

        public FpgaFlashErrorCode FpgaContent_OnlyWriteBoardData(Int32 boardIndex, List<ImageBlock> blocks)
        {
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            FpgaFlashErrorCode result = flash.BoardData_Write(blocks);
            return result;
        }
        public FpgaFlashErrorCode FpgaContent_Write(Int32 boardIndex, Byte[] content, List<ImageBlock> versionInfo)
        {
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            FpgaFlashErrorCode result =
                flash.Content_Write(content);
            return result;
        }
        public Byte[]? FpgaContent_Read(Int32 boardIndex, Int32 maxSize = 0)
        {
            if (_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)boardIndex))
            {
                FpgaFlash fpgaFlash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

                return fpgaFlash.Content_Read(fpgaFlash.Registers.FlashImageAppStartAddr, (UInt32)maxSize);
            }
            return null;
        }
        //public ProductBaseInfo? ProductInfo_Read()
        //{
        //    return rootFpgaFlash?.ProductInfo_Read();
        //}
        //public FpgaFlashErrorCode ProductInfo_Write(ProductBaseInfo productInfo)
        //{
        //    return rootFpgaFlash?.ProductInfo_Write(productInfo) ?? FpgaFlashErrorCode.NotFound;
        //}

        public void Close()
        {
            try
            {
                if (_bIncludeAwgBoard)
                {
                    AWG.PowerOff();
                }
                Hd.Close();
            }
            catch (SEHException)
            {
                // Handle catch here.
            }

            //if (pcieCtrl != null)
            //{
            //    pcieCtrl.Close();
            //}
        }

        public Boolean VerifyFlashID(Int32 BoardIndex)
        {
            if (!ExistsFlash(BoardIndex))
            {
                return false;
            }


            FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)BoardIndex];
            if (flash == null)
            {
                return false;
            }

            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {
                return true;
            }

            return flash.VerifyFlashID(ref _flashId) == FpgaFlashErrorCode.Succeed;
        }

        public FpgaFlashUpdaterMod CheckFpgaFlashUpdaterMod(Int32 BoardIndex)
        {
            if (!ExistsFlash(BoardIndex))
            {
                return FpgaFlashUpdaterMod.FFU_Unknow;
            }

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)BoardIndex];

            return flash.CheckFpgaFlashUpdaterMod();
        }

        public Boolean CheckIamgeIsGoldenAndApp(Int32 boardIndex, Byte[] content)
        {
            if (!ExistsFlash(boardIndex))
            {
                return false;
            }
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            return flash.CheckIamgeIsGoldenAndApp(content);
        }

        public void QueryPerWriteRegUsedNs()
        {
            //if (pcieCtrl == null)
            //{
            //    return;
            //}
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (Int32 i = 0; i < 10_000; i++)
                //pcieCtrl.UserStream.WriteFile(0x8000, 0);
                HdIO.WriteReg(0x8000, 0);
            stopwatch.Stop();
            PerWriteRegUsedNs = (Int32)(stopwatch.ElapsedMilliseconds * 1000 * 1000 / 10_000);
        }
        public Boolean ResetFlashStatusReg(Int32 BoardIndex)
        {
            if (!ExistsFlash(BoardIndex))
                return false;
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)BoardIndex];
            return flash.ResetFlashStatusReg();
        }

        public UInt32? QueryMultipleBootStatus(Int32 boardIndex)
        {
            if (!ExistsFlash(boardIndex))
                return null;
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            return flash.QueryMultipleBootStatus();
        }

        public InfoZone? ReadBoardInfos(Int32 boardIndex)
        {
            if (!ExistsFlash(boardIndex))
                return null;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            Debug.WriteLine($"ReadBoardInfos - ID:{boardIndex}");
            var infozong = flash.InfoZone_Read();
            if (infozong != null)
            {
                infozong.BoardID = (UInt32)boardIndex;
                return infozong;
            }
            return null;
        }

        public FpgaFlashErrorCode FlashInfoZone_Write(Int32 boardIndex, InfoZone infoZone)
        {
            FpgaFlashErrorCode result;
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            infoZone.BoardID = (UInt32)boardIndex;
            result = flash.InfoZone_Write(infoZone);
            return result;
        }

        public FpgaFlashErrorCode FlashInfoZone_Init(Int32 boardIndex)
        {
            FpgaFlashErrorCode result;
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            result = flash.InfoZone_Init();
            return result;
        }

        public FpgaFlashErrorCode FlashInfoZone_Clear(Int32 boardIndex)
        {
            FpgaFlashErrorCode result;
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            result = flash.InfoZone_Clear();
            return result;
        }

        public InfoZone? FlashInfoZone_Read(Int32 boardIndex)
        {
            if (!ExistsFlash(boardIndex))
                return null;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            var result = flash.InfoZone_Read();
            if (result == null)
            {
                return null;
            }
            result.BoardID = (UInt32)boardIndex;
            return result;
        }

        public Byte[]? ReadDataForTest(Int32 boardIndex, UInt32 lengh, UInt32 addr)
        {
            Byte[] bytes = new Byte[lengh];
            if (!ExistsFlash(boardIndex))
                return null;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            var result = flash.ReadBlock(addr, ref bytes, 0, lengh);
            if (result == FpgaFlashErrorCode.Succeed)
                return bytes;
            else
            {
                Debug.WriteLine($"ReadDataForTest ERROR:{result}");
                return null;
            }
        }

        public FpgaFlashErrorCode RandomCorruptedImageForTest(Int32 boardIndex, out Int32 length)
        {
            length = 0;
            var random = new Random();
            length = random.Next(1, 10) * 64 * 1024;
            var randomContent = new Byte[length];
            for (Int32 i = 0; i < length; i++)
            {
                randomContent[i] = (Byte)random.Next(0, 0xf0);
            }
            if (!ExistsFlash(boardIndex))
                return FpgaFlashErrorCode.NotFound;

            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            return flash.Content_Write(randomContent);
        }

        public Boolean ReadProductInfo(out ProductInfo productInfo, Int32 boardIndex = 0)
        {

            productInfo = new ProductInfo();


            //找到产品信息存放Flash
            var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];

            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {//选件信息按照新的途径读取

                UInt32 fpgaAddr = flash.Registers.ProductInfoStartAtBytes;
                UInt32 optbytes = flash.Registers.ProductInfoTotalBytes;
                Byte[] content = new Byte[flash.Registers.ProductInfoTotalBytes];
                if (FpgaFlashErrorCode.Succeed != FastMode_ReadContent(flash, fpgaAddr, optbytes, ref content, 1 * 60 * 1000))
                {
                    WriteLog("RPinfo fail");
                    return false;
                }
                Int32 realLengh = flash.GetDataRealLength(content);
                if (realLengh == 0)
                {
                    WriteLog("RPinfo length = 0");
                    return false;
                }
                else
                {
                    var realContent = new Byte[realLengh];
                    Array.Copy(content, realContent, realLengh);
                    try
                    {
                        productInfo.BytesToInfos(content);
                    }
                    catch (Exception ex)
                    {
                        WriteLog("parse Fial");
                        WriteLog(Encoding.ASCII.GetString(realContent));
                        WriteLog(ex.ToString());
                        return false;
                    }
                    WriteLog("RPinfo succ");
                    return true;
                }
            }
            else
            {
                var result = flash.ProductInfo_Read();
                if (result == null)
                {
                    WriteLog("RPinfo: fail");
                    return false;
                }
                else
                {
                    WriteLog("RPinfo: succ");
                    productInfo = result;
                    return true;
                }
            }
        }

        public Boolean WriteProductInfo(ProductInfo productInfo, Int32 boardIndex = 0)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //找到些选件的Flash
            if (!ExistsFlash(boardIndex))
            {
                return false;
            }
            FpgaFlash? flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }

            Boolean isOnlySN = true;
            if (boardIndex == 0)
            {
                isOnlySN = false;
            }

            sw.Restart();
            //判断固件支持的FLASH访问模式
            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {//使用新路径写

                //数据走到DDR
                Byte[]? content = productInfo.InfosToBytes(isOnlySN);
                if (content == null) return false;
                UInt32 writeBytes = content.Length < flash.Registers.ProductInfoTotalBytes
                    ? (UInt32)content.Length
                    : flash.Registers.ProductInfoTotalBytes;
                UInt32 fpgaAddr = flash.Registers.ProductInfoStartAtBytes;

                Dictionary<Int32, FlashContentAndImageBlock> flashInfo = new Dictionary<Int32, FlashContentAndImageBlock>();
                FlashContentAndImageBlock flashContentAndImageBlock = new FlashContentAndImageBlock()
                {
                    TypeID = (Int32)flash.BoardID,
                    boardName = String.Empty,
                    content = content,
                    ImageBlocks = new List<ImageBlock>(),
                    contentAtPcieDrrOffset = 0
                };

                flashInfo.Add(flashContentAndImageBlock.TypeID, flashContentAndImageBlock);
                if (false == FastMode_TransmitAllContentToPcieDdrByDMA(flashInfo))
                {
                    WriteLog("WPinfo data to ddr");
                    return false;
                }

                WriteLog($"WPinfo {sw.ElapsedMilliseconds} data to ddr"); sw.Restart();

                if (false == FastMode_StartEraseEx(flash, fpgaAddr, (UInt32)flashInfo[flashContentAndImageBlock.TypeID].content.Length))
                {
                    WriteLog("WPinfo Erase fail");
                    return false;
                }
                WriteLog($"WPinfo {sw.ElapsedMilliseconds} Flash Erase"); sw.Restart();

                //数据转移到目标
                if (FpgaFlashErrorCode.Succeed != FastMode_WriteContent(flash, fpgaAddr
                    , flashInfo[flashContentAndImageBlock.TypeID].content
                    , flashInfo[flashContentAndImageBlock.TypeID].contentAtPcieDrrOffset))
                {
                    WriteLog("WPinfo send fail");
                    return false;
                }
                WriteLog($"WPinfo {sw.ElapsedMilliseconds} send time"); sw.Restart();

                //回读检查
                Int32 baclreadlen = flashInfo[flashContentAndImageBlock.TypeID].content.Length;
                Byte[] backreadbuf = new Byte[baclreadlen];
                if (FpgaFlashErrorCode.Succeed != FastMode_ReadContent(flash, fpgaAddr, (UInt32)baclreadlen, ref backreadbuf, 1 * 60 * 1000))
                {
                    WriteLog("WPinfo backread fail");
                    return false;
                }
                for (Int32 i = 0; i < baclreadlen; i++)
                {
                    if (backreadbuf[i] != flashInfo[flashContentAndImageBlock.TypeID].content[i])
                    {
                        WriteLog($"WPinfo backread check fail index={i}");
                        return false;
                    }
                }
                WriteLog($"WPinfo {sw.ElapsedMilliseconds} backread check"); sw.Restart();
                return true;
            }
            else
            {//既有逻辑
                //写pcie
                var result = flash.ProductInfo_Write(productInfo, isOnlySN);
                return result == FpgaFlashErrorCode.Succeed;

            }
        }

        public Boolean ReadOptionsInfo(out OptionsInfoBase optionsInfo)
        {
            optionsInfo = new();

            //选件信息存放PCIE板卡
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }

            //从PCIE板卡读取成功
            if (ReadOptionsInfoEx(flash, ref optionsInfo))
            {
                return true;
            }
            //从PCIE板卡读取失败，就尝试从处理板读取(选件数据从处理板迁移而来)
            else
            {
                flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.ProcessBoard_K7];
                return ReadOptionsInfoEx(flash, ref optionsInfo);
            }       
        }

        private Boolean ReadOptionsInfoEx(FpgaFlash flash, ref OptionsInfoBase optionsInfo)
        {
            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {//选件信息按照新的途径读取

                UInt32 fpgaAddr = flash.Registers.OptionsInfoStartAtBytes;
                UInt32 optbytes = flash.Registers.OptionsInfoTotalBytes;
                Byte[] content = new Byte[flash.Registers.OptionsInfoTotalBytes];
                if (FpgaFlashErrorCode.Succeed != FastMode_ReadContent(flash, fpgaAddr, optbytes, ref content, 1 * 60 * 1000))
                {
                    return false;
                }

                Int32 realLengh = flash.GetDataRealLength(content);
                if (realLengh == 0)
                {
                    return false;
                }
                var realContent = new Byte[realLengh];
                Array.Copy(content, realContent, realLengh);
                try
                {
                    optionsInfo.BytesToInfos(realContent);
                }
                catch (Exception ex)
                {
                    WriteLog("parse Fial");
                    WriteLog(Encoding.ASCII.GetString(realContent));
                    WriteLog(ex.ToString());
                    return false;
                }

                return true;
            }
            else
            {//选件信息按照老的途径读取

                OptionsInfoBase? result = flash.OptionsInfo_Read();
                if (result == null)
                {
                    return false;
                }
                optionsInfo = result;
                return true;
            }
        }

        public Boolean WriteOptionsInfo(OptionsInfoBase info)
        {
            Stopwatch sw = Stopwatch.StartNew();

            //找到些选件的Flash
            //FpgaFlash? flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.ProcessBoard_K7];
            //if (_currProductType == ProductType.JiHe_UPO7000L)
            //{
            //    flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            //}

            //选件信息统一写PCIE板卡
            FpgaFlash? flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }

            sw.Restart();
            //判断固件支持的FLASH访问模式
            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {
                Byte[]? content = info.InfosToBytes();
                UInt32 writeBytes = content.Length < flash.Registers.OptionsInfoTotalBytes
                    ? (UInt32)content.Length
                    : flash.Registers.OptionsInfoTotalBytes;
                UInt32 fpgaAddr = flash.Registers.OptionsInfoStartAtBytes;

                //数据走到DDR
                Dictionary<Int32, FlashContentAndImageBlock> flashInfo = new Dictionary<Int32, FlashContentAndImageBlock>();
                FlashContentAndImageBlock flashContentAndImageBlock = new FlashContentAndImageBlock()
                {
                    TypeID = (Int32)flash.BoardID,
                    boardName = String.Empty,
                    content = content,
                    ImageBlocks = new List<ImageBlock>(),
                    contentAtPcieDrrOffset = 0
                };
                flashInfo.Add(flashContentAndImageBlock.TypeID, flashContentAndImageBlock);
                if (false == FastMode_TransmitAllContentToPcieDdrByDMA(flashInfo))
                {
                    return false;
                }
                WriteLog($"wOinfo {sw.ElapsedMilliseconds} data to DDR"); sw.Restart();

                if (false == FastMode_StartEraseEx(flash, fpgaAddr, (UInt32)flashInfo[flashContentAndImageBlock.TypeID].content.Length))
                {
                    return false;
                }
                WriteLog($"wOinfo {sw.ElapsedMilliseconds} Flash Erase"); sw.Restart();

                //数据转移到目标
                if (FpgaFlashErrorCode.Succeed != FastMode_WriteContent(flash, fpgaAddr
                    , flashInfo[flashContentAndImageBlock.TypeID].content
                    , flashInfo[flashContentAndImageBlock.TypeID].contentAtPcieDrrOffset))
                {
                    return false;
                }
                WriteLog($"wOinfo {sw.ElapsedMilliseconds} data send"); sw.Restart();

                //回读检查
                Int32 baclreadlen = flashInfo[flashContentAndImageBlock.TypeID].content.Length;
                Byte[] backreadbuf = new Byte[baclreadlen];
                if (FpgaFlashErrorCode.Succeed != FastMode_ReadContent(flash, fpgaAddr, (UInt32)baclreadlen, ref backreadbuf, 1 * 60 * 1000))
                {
                    WriteLog($"wOinfo backread fail");
                    return false;
                }
                for (Int32 i = 0; i < baclreadlen; i++)
                {
                    if (backreadbuf[i] != flashInfo[flashContentAndImageBlock.TypeID].content[i])
                    {
                        WriteLog($"wOinfo backread check fial index={i}");
                        return false;
                    }
                }
                WriteLog($"wOinfo {sw.ElapsedMilliseconds}  backread check ok"); sw.Restart();
                return true;
            }
            else
            {
                FpgaFlashErrorCode result = flash.OptionsInfo_Write(info);
                return result == FpgaFlashErrorCode.Succeed;
            }
        }

        /// <summary>
        /// Flash数据读取 数据格式为#9000001000.... 表示 9 个字节描述数据的长度， 000001000 表示波形数据的长度，即1000 字节
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="startAddress">读取起始地址</param>
        /// <param name="length">读取长度</param>
        /// <returns>是否读取成功</returns>
        private Boolean FlashCaliData_Read(out Byte[] buffer, UInt32 startAddress, UInt32 length)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null || startAddress == 0 || length == 0)
            {
                return false;
            }

            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {
                var content = new List<Byte>();
                var readsize = 2 * 64 * 1024U;
                var contentlength = -1L;
                var headlength = 11;//数据头 固定11个字节
                var datalengh = 0L;
                var hashead = false;
                do
                {
                    var thiscontent = new Byte[readsize];
                    var res = FastMode_ReadContent(flash, startAddress, readsize, ref thiscontent);
                    if (res != FpgaFlashErrorCode.Succeed)
                    {
                        return false;
                    }

                    if (contentlength <= 0)
                    {
                        //解析长度
                        var header = Encoding.ASCII.GetString(thiscontent, 0, 11);
                        // 正则匹配：#9 + 9位数字 严格匹配格式
                        if (Regex.IsMatch(header, @"^#9\d{9}$"))
                        {
                            //解析长度（此时已确保是数字）
                            var lengthtr = header.Substring(2, 9); // 跳过#9，取9位数字
                            if (!Int64.TryParse(lengthtr, out datalengh) || datalengh <= 0)
                            {
                                contentlength = length;
                            }
                            else
                            {
                                contentlength = datalengh + headlength;
                                hashead = true;//解析成功 认为含有数据头头
                            }
                        }
                        else
                            contentlength = length;
                        content.AddRange(thiscontent);
                    }
                    else
                    {
                        content.Clear();
                        content.AddRange(thiscontent);
                    }

                    if (content.Count >= contentlength)
                    {
                        break;
                    }
                    readsize = (UInt32)contentlength;

                } while (true);

                var databuffer = hashead ? content.Skip(headlength).ToArray() : content.ToArray();
                var reallengh = datalengh == 0 ? flash.GetDataRealLength(databuffer) : datalengh;
                if (reallengh == 0)
                {
                    return false;
                }
                buffer = new Byte[reallengh];
                Array.Copy(databuffer, buffer, reallengh);
                return true;
            }
            else
            {
                var content = new Byte[length];
                var result = flash.ReadBlock(startAddress, ref content, 0, length);
                var reallengh = flash.GetDataRealLength(content);
                if (reallengh == 0)
                {
                    return false;
                }
                buffer = new Byte[reallengh];
                Array.Copy(content, buffer, reallengh);
                return true;
            }
        }

        /// <summary>
        /// 数据写入Flash 数据格式为#9000001000.... 表示 9 个字节描述数据的长度， 000001000 表示波形数据的长度，即1000 字节
        /// </summary>
        /// <param name="buffer">数据buffer</param>
        /// <param name="startAddress">写入起始地址</param>
        /// <param name="eraesSize">需要擦除的大小</param>
        /// <returns></returns>
        private Boolean FlashCaliData_Write(Byte[] buffer, UInt32 startAddress, UInt32 eraesSize)
        {
            var sw = Stopwatch.StartNew();
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null || buffer == null || buffer.Length <= 0 || startAddress <= 0 || eraesSize <= 0)
            {
                return false;
            }
            var headbytes = Encoding.UTF8.GetBytes("#9" + buffer.Length.ToString().PadLeft(9, '0'));
            var newbuffer = new byte[buffer.Length + headbytes.Length];
            headbytes.CopyTo(newbuffer, 0);
            buffer.CopyTo(newbuffer, headbytes.Length);

            sw.Restart();
            //判断固件支持的FLASH访问模式
            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {
                //数据走到DDR
                var flashinfo = new Dictionary<Int32, FlashContentAndImageBlock>();
                var flashcontentandimageBlock = new FlashContentAndImageBlock()
                {
                    TypeID = (Int32)flash.BoardID,
                    boardName = String.Empty,
                    content = newbuffer,
                    ImageBlocks = new List<ImageBlock>(),
                    contentAtPcieDrrOffset = 0
                };
                flashinfo.Add(flashcontentandimageBlock.TypeID, flashcontentandimageBlock);
                if (FastMode_TransmitAllContentToPcieDdrByDMA(flashinfo) == false)
                {
                    return false;
                }
                WriteLog($"wCaliData {sw.ElapsedMilliseconds} data to DDR"); sw.Restart();

                if (FastMode_StartEraseEx(flash, startAddress, (UInt32)flashinfo[flashcontentandimageBlock.TypeID].content.Length) == false)
                {
                    return false;
                }
                WriteLog($"wCaliData {sw.ElapsedMilliseconds} Flash Erase"); sw.Restart();

                //数据转移到目标
                if (FpgaFlashErrorCode.Succeed != FastMode_WriteContent(flash, startAddress, flashinfo[flashcontentandimageBlock.TypeID].content, flashinfo[flashcontentandimageBlock.TypeID].contentAtPcieDrrOffset))
                {
                    return false;
                }
                WriteLog($"wCaliData {sw.ElapsedMilliseconds} data send"); sw.Restart();

                //回读检查
                Int32 baclreadlen = flashinfo[flashcontentandimageBlock.TypeID].content.Length;
                Byte[] backreadbuf = new Byte[baclreadlen];
                if (FpgaFlashErrorCode.Succeed != FastMode_ReadContent(flash, startAddress, (UInt32)baclreadlen, ref backreadbuf))
                {
                    WriteLog($"wCaliData backread fail");
                    return false;
                }
                for (Int32 i = 0; i < baclreadlen; i++)
                {
                    if (backreadbuf[i] != flashinfo[flashcontentandimageBlock.TypeID].content[i])
                    {
                        WriteLog($"wCaliData backread check fial index={i}");
                        return false;
                    }
                }
                WriteLog($"wCaliData {sw.ElapsedMilliseconds}  backread check ok"); sw.Restart();
                return true;
            }
            else
            {
                var writebytes = buffer.Length < eraesSize ? (UInt32)buffer.Length : eraesSize;

                var result = flash.WriteBlockOneSector(startAddress, buffer, 0, writebytes);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    WriteLog($"UserData_Write Error:Addr:0x{startAddress:X},Bytes:{writebytes}");
                }
                return result == FpgaFlashErrorCode.Succeed;
            }
        }

        public Boolean AwgCaliDataRead(out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_AWG;
            var length = flash.Registers.CaliDataTotalAtBytes_AWG;

            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Read(out buffer, startaddress, length);
            WriteLog($"AwgCaliDataRead {sw.ElapsedMilliseconds} ms {ret}"); 
            return ret;
        }

        public Boolean AwgCaliDataWrite(Byte[] buffer)
        {
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_AWG;
            var length = flash.Registers.CaliDataTotalAtBytes_AWG + 128 * 1024;//128K的隔离带

            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Write(buffer, startaddress, length);
            WriteLog($"AwgCaliDataWrite {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean MiscCaliDataRead(out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_MISC;
            var length = flash.Registers.CaliDataTotalAtBytes_MISC;
            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Read(out buffer, startaddress, length);
            WriteLog($"MiscCaliDataRead {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean MiscCaliDataWrite(Byte[] buffer)
        {
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_MISC;
            var length = flash.Registers.CaliDataTotalAtBytes_MISC + 128 * 1024;//128K的隔离带

            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Write(buffer, startaddress, length);
            WriteLog($"MiscCaliDataWrite {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean DspCaliDataRead(out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_DSP;
            var length = flash.Registers.CaliDataTotalAtBytes_DSP;
            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Read(out buffer, startaddress, length);
            WriteLog($"DspCaliDataRead {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean DspCaliDataWrite(Byte[] buffer)
        {
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_DSP;
            var length = flash.Registers.CaliDataTotalAtBytes_DSP + 128 * 1024;//128K的隔离带

            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Write(buffer, startaddress, length);
            WriteLog($"DspCaliDataWrite {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean ChannelCaliDataRead(out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_Channel;
            var length = flash.Registers.CaliDataTotalAtBytes_Channel;
            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Read(out buffer, startaddress, length);
            WriteLog($"ChannelCaliDataRead {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean ChannelCaliDataWrite(Byte[] buffer)
        {
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_Channel;
            var length = flash.Registers.CaliDataTotalAtBytes_Channel + 128 * 1024;//128K的隔离带
            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Write(buffer, startaddress, length);
            WriteLog($"ChannelCaliDataWrite {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean TiadcCaliDataRead(out Byte[] buffer)
        {
            buffer = new Byte[0];
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return false;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_TiAdc;
            var length = flash.Registers.CaliDataTotalAtBytes_TiAdc;
            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Read(out buffer, startaddress, length);
            WriteLog($"TiadcCaliDataRead {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public Boolean TiadcCaliDataWrite(Byte[] buffer)
        {
            var flash = _fpgaFlashDefine[SystemFPGAConstituteDefine.PCIE];
            if (flash == null)
            {
                return true;
            }
            var startaddress = flash.Registers.CaliDataStartAtBytes_TiAdc;
            var length = flash.Registers.CaliDataTotalAtBytes_TiAdc + 128 * 1024;//128K的隔离带

            var sw = Stopwatch.StartNew();
            var ret = FlashCaliData_Write(buffer, startaddress, length);
            WriteLog($"TiadcCaliDataWrite {sw.ElapsedMilliseconds} ms {ret}");
            return ret;
        }

        public FpgaFlashErrorCode ProductInfo_Clear(int boardIndex = 0)
        {
            //找到些选件的Flash
            if (!ExistsFlash(boardIndex))
            {
                return FpgaFlashErrorCode.NotFound;
            }
            FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            if (flash == null)
            {
                return FpgaFlashErrorCode.NotFound;
            }

            //判断固件支持的FLASH访问模式
            FpgaFlashErrorCode result = FpgaFlashErrorCode.Succeed;
            if (flash.CheckFpgaFlashUpdaterMod() == FpgaFlashUpdaterMod.FFU_FastMod)
            {
                FastMode_StartEraseEx(flash, flash.Registers.ProductInfoStartAtBytes, flash.Registers.ProductInfoTotalBytes);
            }
            else
            {
                result = flash.ProductInfo_Clear();
            }

            return result;
        }

        #endregion

        #region FPGA_Register Define

        internal enum SystemFPGAConstituteDefine
        {
            PCIE = 0,
            ProcessBoard_S6 = 1,
            ProcessBoard_K7 = 2,
            AcquireBoard_K7_1 = 3,
            AcquireBoard_K7_2 = 4,
            AcquireBoard_K7_3 = 5,
            AcquireBoard_K7_4 = 6,
            AcquireBoard_K7_5 = 7,
            AcquireBoard_K7_6 = 8,
            AcquireBoard_K7_7 = 9,
            AcquireBoard_K7_8 = 10,
            AWG = 11,
            Probe = 12,
            TMC_FPGA = 13,
            TMC_MCU = 14,
        }

        #endregion

        #endregion

        #region 2024 New Schame

        private Boolean? IsFastMode = null;                    //是否是快速模式  
        private const Int32 One64KBytes = 64 * 1024;        //64K凑整
        private const Int32 PerDMABytes = 16 * 1024 * 1024; //16M单次
        private Dictionary<Int32, Int32> BoardSpiCorrespondence = new Dictionary<Int32, Int32>()
        {
            { (Int32)SystemFPGAConstituteDefine.PCIE             /**/, 0 },
            { (Int32)SystemFPGAConstituteDefine.ProcessBoard_K7  /**/, 1 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_1/**/, 2 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_2/**/, 3 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_3/**/, 4 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_4/**/, 5 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_5/**/, 6 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_6/**/, 7 },
            { (Int32)SystemFPGAConstituteDefine.AcquireBoard_K7_7/**/, 8 },
        };

        private void FastMode_InitUpdate(FpgaFlash fpgaFlash)
        {
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, 0);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, 1); Thread.Sleep(1);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, 0);
        }

        enum FlashMode_Action
        {
            FlashActionNormal = 0x00, //常规
            EraseFlash = 0x01, //擦除
            WriteFlashContent = 0x02, //写入
            ReadFlashContent = 0x03, //读取
        }

        public FpgaFlashErrorCode FastMode_VerifyFlashID(Int32 boardIndex)
        {
            //FpgaFlash fpgaFlash = fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            //FastMode_ResetFlashSideFpgaAction(fpgaFlash);
            //HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0);
            //HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)1);
            //Thread.Sleep(1);
            //HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0);
            //HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, (UInt32)FlashMode_Action.ReadFlashID);
            ////HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x00);
            ////HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x01);
            ////HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x00);
            ////Thread.Sleep(2);
            //UInt32 readbackFlashID = HdIO.ReadReg(fpgaFlash.Registers.FastMode_FlashID_H16);
            //readbackFlashID <<= 16;
            //readbackFlashID |= HdIO.ReadReg(fpgaFlash.Registers.FastMode_FlashID_L16);
            //if (readbackFlashID != fpgaFlash.FlashID)
            //{
            //    fpgaFlash.AddLog?.Invoke($"FLASH BoardID-{fpgaFlash.BoardID} :read Flash ID Action=>verify error");
            //    return FpgaFlashErrorCode.FLashIDMismatching;
            //}
            return FpgaFlashErrorCode.Succeed;
        }

        public FpgaFlashErrorCode FastMode_VerifyFpgaIDCode(Int32 boardIndex, Byte[] content)
        {
            return FpgaFlashErrorCode.Succeed;
            //if (!ExistsFlash(boardIndex))
            //    return FpgaFlashErrorCode.NotFound;

            //Byte[] readBackData = new Byte[PerDMABytes];
            //var flash = fpgaFlashDefine[(SystemFPGAConstituteDefine)boardIndex];
            //FpgaFlashErrorCode result = FastMode_ReadContent(flash, ref readBackData);
            //if (result != FpgaFlashErrorCode.Succeed)
            //    return result;

            //UInt32 searchHeaderBytes = 256;//通过观察得到的，并没有找到文档支持
            //if (searchHeaderBytes > content.Length)
            //{
            //    searchHeaderBytes = (UInt32)content.Length;
            //}
            //String HeaderStr = "";
            //for (Int32 i = 0; i < searchHeaderBytes; i++)
            //    HeaderStr += $"_{content[i].ToString("X").PadLeft(2, '0')}";
            //return HeaderStr.IndexOf(flash.Registers.IDCodeVerify, StringComparison.Ordinal) > 0 ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.InfoError;
        }

        /// <summary>
        /// 给指定FPGA发送Flash擦除指令
        /// </summary>
        private void FastMode_StartErase(FpgaFlash fpgaFlash, UInt32 flashStartAddr, UInt32 eraseTotalBytes, UInt32 aligned = 64 * 1024)
        {
            Int32 spiIndex = BoardSpiCorrespondence[(Int32)fpgaFlash.BoardID];
            UInt32 spiMark = (UInt32)(1 << spiIndex);
            UInt32 totalBytes = (UInt32)((eraseTotalBytes + aligned - 1) / aligned) * aligned;//必须是64k的整数倍

            //为什么要 -4K? 因为FPGA是从结束地址多擦4K！！！！ 在上层做的修正(lhj)
            totalBytes = totalBytes - 4 * 1024;

            #region Flash Side Fpga
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x01); Thread.Sleep(1);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);

            //二级 FPGA 动作配置
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressL16, (UInt32)(flashStartAddr >> 0x00) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressH16, (UInt32)(flashStartAddr >> 0x10) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressL16, (UInt32)((flashStartAddr + totalBytes) >> 0x00) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressH16, (UInt32)((flashStartAddr + totalBytes) >> 0x10) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_WhichFlash, (UInt32)0);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, (UInt32)FlashMode_Action.EraseFlash);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x00);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x01);

            #endregion
        }
        //
        private Boolean FastMode_StartEraseEx(FpgaFlash fpgaFlash, UInt32 flashStartAddr, UInt32 eraseTotalBytes)
        {
            Boolean rsult = false;
            FastMode_StartErase(fpgaFlash, flashStartAddr, eraseTotalBytes);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < 1 * 60 * 1000)
            {
                UInt32 eraseFinish = HdIO.ReadReg(fpgaFlash.Registers.FastMode_ActionStatus);
                if (eraseFinish == 1)
                {
                    rsult = true;
                    break;
                }
            }

            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DoWhat, 0x00);                 //复位2级传输的动作
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, 0x00);           //复位2级FPGA的动作
            return rsult;
        }

        private FpgaFlashErrorCode FastMode_WriteContent(FpgaFlash fpgaFlash, UInt32 flashStartAddr, Byte[] content, UInt32 DDRStartAddress)
        {
            Int32 spiIndex = BoardSpiCorrespondence[(Int32)fpgaFlash.BoardID];
            UInt32 spiMark = (UInt32)(1 << spiIndex);
            UInt32 totalBytes = 0;
            totalBytes = (UInt32)((content.Length + 64 * 1024 - 1) / (64 * 1024)) * (64 * 1024);//必须是64k的整数倍
            //totalBytes = (UInt32)(content.Length);//转写实际大小

            #region Flash Side Fpga
            //二级FPGA开启快速模式
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x01); Thread.Sleep(1);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);

            //二级Flash操作的参数配置
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressL16, (UInt32)(flashStartAddr >> 0x00) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressH16, (UInt32)(flashStartAddr >> 0x10) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressL16, (UInt32)((flashStartAddr + totalBytes) >> 0x00) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressH16, (UInt32)((flashStartAddr + totalBytes) >> 0x10) & 0xffff);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_WhichFlash, (UInt32)0);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, (UInt32)FlashMode_Action.WriteFlashContent);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x00);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x01);
            #endregion

            #region Pcie Side
            //一级 FPGA 动作配置
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_WhichSpi, (UInt32)spiIndex);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);

            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DoWhat, (UInt32)(UInt32)FlashMode_Action.WriteFlashContent);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DDR_StartAddress_L16, DDRStartAddress & 0xffff);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DDR_StartAddress_H16, (DDRStartAddress >> 16) & 0xffff);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DDR_TotalBytes_L16, (UInt32)totalBytes & 0xffff);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DDR_TotalBytes_H16, (UInt32)(totalBytes >> 16) & 0xffff);

            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)1);

            #endregion

            //等待传输完成
            //flash datasheet say,use 100ms per page(256byte),wo max wait 200ms per sector
            Int64 maxWaitMilliseconds = (content.Length + One64KBytes - 1) / 256 * 200;
            UInt32 status = 0x1;//忙中
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < maxWaitMilliseconds)
            {
                status = HdIO.ReadReg(PcieBdReg.R.Level2Transmit_ActionStatus) & 0x03;
                if ((status & 0x03) == 0x01)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DoWhat, 0x00);                 //复位2级传输的动作
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, 0x00);           //复位2级FPGA的动作

            if ((status & 0x01) != 0x01)
            {
                fpgaFlash.AddLog?.Invoke($"FLASH BoardID-{fpgaFlash.BoardID} :Write App Image Action=>over time!");
                return FpgaFlashErrorCode.ErrorOvertime;
            }
            else if ((status & 0x02) == 0x02)
            {
                fpgaFlash.AddLog?.Invoke($"FLASH BoardID-{fpgaFlash.BoardID} :Write App Image Action:Fpga return Error!");
                return FpgaFlashErrorCode.GeneralError;
            }
            return FpgaFlashErrorCode.Succeed;
        }

        private Object Lock_Pcie_DdrOperation = new Object();
        private FpgaFlashErrorCode FastMode_ReadContent(FpgaFlash fpgaFlash, UInt32 flashStartAddr, UInt32 readbackLength, ref Byte[] readbackContent, UInt32 timeout = 5 * 60 * 1000)
        {
            Int32 spiIndex = BoardSpiCorrespondence[(Int32)fpgaFlash.BoardID];
            UInt32 spiMark = (UInt32)(1 << spiIndex);
            UInt32 totalBytes = (UInt32)((readbackLength + 64 * 1024 - 1) / (64 * 1024)) * (64 * 1024);//必须是64k的整数倍
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);

            #region 回读配置 Side Fpga
            {
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x01); Thread.Sleep(1);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionReset, (UInt32)0x00);

                HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressL16, (UInt32)(flashStartAddr >> 0x00) & 0xffff);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashStartAddressH16, (UInt32)(flashStartAddr >> 0x10) & 0xffff);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressL16, (UInt32)((flashStartAddr + totalBytes) >> 0) & 0xffff);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_FlashEndAddressH16, (UInt32)((flashStartAddr + totalBytes) >> 16) & 0xffff);
                HdIO.WriteReg(fpgaFlash.Registers.FastMode_WhichFlash, (UInt32)0);
            }
            #endregion

            #region 回读配置 Pcie Side
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_WhichSpi, (UInt32)spiIndex);

            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, totalBytes * 8);       //注意此处需要乘8 FPGA使用bit计数的
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset, (UInt32)1);

            #endregion

            //通知二级FPGA开始传输
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, (UInt32)FlashMode_Action.ReadFlashContent);
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x00);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DoWhat, 0X02);                 //设置2级传输的动作
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionStart, (UInt32)0x01);

            //等待二级FPGA传输结束
            Stopwatch stopwatch = Stopwatch.StartNew();
            UInt32 ReadFinish = 0;
            while (ReadFinish != 1 && stopwatch.ElapsedMilliseconds < timeout)
            {
                ReadFinish = HdIO.ReadReg(PcieBdReg.R.Xdma_XdmaWrFinish);
            }
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_DoWhat, 0x00);                 //复位2级传输的动作
            HdIO.WriteReg(fpgaFlash.Registers.FastMode_ActionCode, 0x00);           //复位2级FPGA的动作

            if (ReadFinish != 1)
            {
                WriteLog($"dataclean：board ID={fpgaFlash.BoardID} PcieBdReg.R.Xdma_XdmaWrFinish = {ReadFinish}  timeout");
                return FpgaFlashErrorCode.ErrorOvertime;
            }
            else
            {
                WriteLog($"dataclean：board ID={fpgaFlash.BoardID} PcieBdReg.R.Xdma_XdmaWrFinish = {ReadFinish}");
            }

            //回读二级回传数据
            Byte[] readbackBuffer = new Byte[totalBytes];
            HdIO.CurrDriver?.DMARead(0, totalBytes, ref readbackBuffer);

            //返回要求读取的数据
            var lastsize = Math.Min(Math.Min(totalBytes, readbackLength), readbackContent.Length);
            Array.Copy(readbackBuffer, readbackContent, lastsize);

            return FpgaFlashErrorCode.Succeed;
        }

        private FpgaFlashErrorCode DoFastMode_FlashUpdate(FpgaFlash fpgaFlash, FlashContentAndImageBlock flashContentAndImageBlock)
        {
            FpgaFlashErrorCode returnCode;

            //参数检查
            if (flashContentAndImageBlock.content == null || flashContentAndImageBlock.content.Length == 0)
            {
                WriteLog($"data send：skip board ID={fpgaFlash.BoardID} context is empty");
                return FpgaFlashErrorCode.GeneralError;
            }

            //如果是Gloden和app的合并bing，则从0地址开始
            UInt32 imageStartAddress = fpgaFlash.Registers.FlashImageAppStartAddr;
            if (fpgaFlash.CheckIamgeIsGoldenAndApp(flashContentAndImageBlock.content!))
            {
                WriteLog($"data send：data board ID={fpgaFlash.BoardID} contain golden");
                imageStartAddress = 0;
            }

            //转写
            returnCode = FastMode_WriteContent(fpgaFlash, imageStartAddress, flashContentAndImageBlock.content!, flashContentAndImageBlock.contentAtPcieDrrOffset);
            if (returnCode != FpgaFlashErrorCode.Succeed)
            {
                return returnCode;
            }

            //回读检查
            Byte[] readbackContent = new Byte[flashContentAndImageBlock.content!.Length];
            returnCode = FastMode_ReadContent(fpgaFlash, imageStartAddress, (UInt32)flashContentAndImageBlock.content!.Length, ref readbackContent);
            if (returnCode != FpgaFlashErrorCode.Succeed)
            {
                return returnCode;
            }

            FastMode_SaveCacheData($"{fpgaFlash.BoardID}.FormFlash.bin", readbackContent);

            //检查
            for (Int32 i = 0; i < flashContentAndImageBlock.content.Length; i++)
            {
                if (flashContentAndImageBlock.content[i] != readbackContent[i])
                {
                    WriteLog($"data send：fail board ID={fpgaFlash.BoardID} backread error index={i}");
                    return FpgaFlashErrorCode.WriteVerifyDefeated;
                }
            }
            return FpgaFlashErrorCode.Succeed;
        }

        private Boolean FastMode_TransmitAllContentToPcieDdrByDMA(Dictionary<Int32/*boardIndex*/, FlashContentAndImageBlock> flashInfo)
        {
            UInt32 position = 200 * 1024 * 1024;//0;
            WriteLog($"data down: ddr buff addr {position}");
            Dictionary<Int32, UInt32> currUpdateContentAtDdrPosition = new Dictionary<Int32, UInt32>();

            Byte[] perDMAWriteBuffer = new Byte[PerDMABytes]; //每次写入16M Bytes
            Int32 OneMBytes = 1 * 1024 * 1024;                //零头凑整成M Bytes

            #region 通过DMA传递到PCIE的DDR中
            foreach (KeyValuePair<Int32, FlashContentAndImageBlock> board in flashInfo)
            {
                if (!_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)board.Key))
                {
                    WriteLog($"data down：skip board ID={board.Key} not exist on current product");
                    continue;
                }

                FastMode_SaveCacheData($"{board.Value.TypeID}.FormUdppk.bin", board.Value.content);

                //记录当前更新项在DDR的数据位置
                currUpdateContentAtDdrPosition.Add(board.Key, position);
                board.Value.contentAtPcieDrrOffset = position;

                Int32 writedByteCount = 0;
                Int32 currDMAByteCount = 0;
                while (writedByteCount < board.Value.content.Length)
                {
                    //每次写入要么是PerDMABytes 要么是n*One64KBytes
                    currDMAByteCount = (board.Value.content.Length - writedByteCount) > PerDMABytes
                        ? PerDMABytes
                        : (((board.Value.content.Length - writedByteCount + OneMBytes - 1) / One64KBytes) * One64KBytes);

                    //拷贝本次写入数据
                    if ((board.Value.content.Length - writedByteCount) < currDMAByteCount)
                    {
                        for (Int32 i = 0; i < perDMAWriteBuffer.Length; i++)
                        {   //先把缓冲区初始化为FF
                            perDMAWriteBuffer[i] = 0xFF;
                        }
                        Array.Copy(board.Value.content, writedByteCount, perDMAWriteBuffer, 0, board.Value.content.Length - writedByteCount);
                    }
                    else
                    {
                        Array.Copy(board.Value.content, writedByteCount, perDMAWriteBuffer, 0, currDMAByteCount);
                    }

                    //本次写入
                    if (false == HdIO.DMAWrite((UInt32)(position + writedByteCount), perDMAWriteBuffer, (UInt32)currDMAByteCount))
                    {
                        WriteLog($"data down：fail board ID={board.Key} dma write error");
                        return false;
                    }
                    else
                    {
                        writedByteCount += currDMAByteCount;
                        WriteLog($"data down：write board ID={board.Key}  Totals = {board.Value.content.Length} Current write ={currDMAByteCount} ");
                    }
                }

                //计算出下个更新项的DDR开始地址
                Int32 contentBytes = board.Value.content.Length;
                position += (UInt32)(((contentBytes + OneMBytes - 1) / OneMBytes) * OneMBytes);
            }
            #endregion


            #region 通过DMA方式读回各个区域，进行验证
            //统计出更新项中最大的项用于开辟接收缓冲区
            Int32 maxContentBytes = 0;
            foreach (KeyValuePair<Int32, FlashContentAndImageBlock> board in flashInfo)
            {
                if (!_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)board.Key))
                {
                    continue;
                }

                if (board.Value.content.Length > maxContentBytes)
                {
                    maxContentBytes = board.Value.content.Length;
                }
            }
            maxContentBytes = ((maxContentBytes + One64KBytes - 1) / One64KBytes) * One64KBytes;

            Byte[] readBackData = new Byte[maxContentBytes];
            foreach (KeyValuePair<Int32, FlashContentAndImageBlock> board in flashInfo)
            {
                if (!_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)board.Key))
                {
                    continue;
                }
                Int32 currContentBytes = board.Value.content.Length;
                currContentBytes = ((currContentBytes + One64KBytes - 1) / One64KBytes) * One64KBytes;
                if (false == HdIO.RawDMARead(currUpdateContentAtDdrPosition[board.Key], (UInt32)currContentBytes, ref readBackData))
                {
                    WriteLog($"data down：fail board ID={board.Key} backread fail");
                    return false;
                }

                for (Int32 i = 0; i < board.Value.content.Length; i++)
                {
                    if (board.Value.content[i] == readBackData[i]) continue;
                    WriteLog($"data down：fail board ID={board.Key} check fail，Index={i}");
                    return false;
                }
                WriteLog($"data down：succ board ID={board.Key}");
            }
            #endregion

            return true;
        }

        public Boolean FastMode_UpdateIncludeFlash(Dictionary<Int32, FlashContentAndImageBlock> flashInfo)
        {
            #region step1 通过DMA把要烧写的bin内容写入PCIE的DDR中
            if (false == FastMode_TransmitAllContentToPcieDdrByDMA(flashInfo))
            {
                return false;
            }
            #endregion

            #region step2 复位二级传输，使其处于指令状态
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);
            #endregion

            #region step3 并行擦除-发出所有擦除命令
            List<Int32> TMC_Items = new List<Int32>();
            List<UInt32> eraseActionStatusFlashType = new List<UInt32>();
            List<UInt32> eraseActionStatusRegisters = new List<UInt32>();//记录需要擦除的FPGA中读取擦除状态的寄存器
            List<Boolean> eraseActionStatusIsFinishs = new List<Boolean>();
            foreach (var board in flashInfo)
            {
                if (!_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)board.Key))
                {
                    WriteLog($"dataclean：skip board ID={board.Key} not exist on current product");
                    continue;
                }

                if ((SystemFPGAConstituteDefine)board.Key == SystemFPGAConstituteDefine.TMC_FPGA || (SystemFPGAConstituteDefine)board.Key == SystemFPGAConstituteDefine.TMC_MCU)
                {
                    WriteLog($"dataclean：skip board ID={board.Key} not support");
                    TMC_Items.Add(board.Key);
                    continue;
                }
                WriteLog($"dataclean：start board ID={board.Key}");
                var fpgaFlash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)board.Key];
                eraseActionStatusRegisters.Add(fpgaFlash.Registers.FastMode_ActionStatus);
                eraseActionStatusFlashType.Add(fpgaFlash.BoardID);
                eraseActionStatusIsFinishs.Add(false);

                //如果是Gloden和app的合并bing，则从0地址开始
                UInt32 imageStartAddress = fpgaFlash.Registers.FlashImageAppStartAddr;
                if (fpgaFlash.CheckIamgeIsGoldenAndApp(board.Value.content))
                {
                    WriteLog($"dataclean：notice board ID={fpgaFlash.BoardID} contain golden");
                    imageStartAddress = 0;
                }
                FastMode_StartErase(fpgaFlash, imageStartAddress, (UInt32)board.Value.content.Length);
            }
            #endregion

            #region step4 并行擦除-等待所有查出完成
            Stopwatch stopwatch = Stopwatch.StartNew();
            Boolean bAllOk = false;
            while (stopwatch.ElapsedMilliseconds < 5 * 60 * 1000)
            {
                Int32 OkCount = 0;
                for (Int32 i = 0; i < eraseActionStatusRegisters.Count; i++)
                {
                    if (!eraseActionStatusIsFinishs[i])
                    {
                        UInt32 eraseFinish = HdIO.ReadReg(eraseActionStatusRegisters[i]);
                        if (eraseFinish == 1)
                        {
                            eraseActionStatusIsFinishs[i] = true;
                            OkCount++;
                        }
                    }
                    else
                    {
                        OkCount++;
                    }
                }
                if (OkCount == eraseActionStatusRegisters.Count)
                {
                    bAllOk = true;
                    break;
                }
            }
            for (Int32 i = 0; i < eraseActionStatusIsFinishs.Count; i++)
            {
                if (eraseActionStatusIsFinishs[i])
                {
                    WriteLog($"dataclean：succ board ID={eraseActionStatusFlashType[i]} ");
                }
                else
                {
                    WriteLog($"dataclean：fail board ID={eraseActionStatusFlashType[i]} erase timeout");
                }
            }
            if (!bAllOk)
            {
                return false;
            }
            #endregion

            #region 擦除后回读检查是否擦干净
            if (true)
            {
                FpgaFlashErrorCode returnCode;
                foreach (var board in flashInfo)
                {
                    SystemFPGAConstituteDefine name = (SystemFPGAConstituteDefine)board.Key;
                    FpgaFlash? flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)board.Key];
                    UInt32 imageStartAddress = flash.Registers.FlashImageAppStartAddr;
                    if (flash.CheckIamgeIsGoldenAndApp(board.Value.content!))
                    {
                        imageStartAddress = 0;
                    }
                    Byte[] readbackContent = new Byte[board.Value.content!.Length];
                    returnCode = FastMode_ReadContent(flash, imageStartAddress, (UInt32)board.Value.content!.Length, ref readbackContent);
                    if (returnCode != FpgaFlashErrorCode.Succeed)
                    {
                        WriteLog($"dataclean：board ID={board.Key}-{name.ToString()} backread fail on erase after");
                        continue;
                    }

                    for (Int32 i = 0; i < readbackContent.Length; i++)
                    {
                        if (readbackContent[i] != 0XFF)
                        {
                            WriteLog($"dataclean：board ID={board.Key}-{name.ToString()} index={i} != 0XFF on erase after");
                            break;
                        }
                    }
                }
            }
            #endregion

            #region step5 串行烧写-各个待升级的内容
            foreach (var board in flashInfo)
            {

                if (!_fpgaFlashDefine.ContainsKey((SystemFPGAConstituteDefine)board.Key))
                {
                    WriteLog($"data send：skip board ID={board.Key} not exist on current product");
                    continue;
                }

                if ((SystemFPGAConstituteDefine)board.Key == SystemFPGAConstituteDefine.TMC_FPGA
                    || (SystemFPGAConstituteDefine)board.Key == SystemFPGAConstituteDefine.TMC_MCU)
                {
                    continue;
                }
                var flash = _fpgaFlashDefine[(SystemFPGAConstituteDefine)board.Key];
                var result = DoFastMode_FlashUpdate(flash, board.Value);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    WriteInnoSetupMessageInfos(board.Value.boardName, false);
                    return false;
                }
                else
                {
                    WriteInnoSetupMessageInfos(board.Value.boardName, true);
                }
            }
            #endregion

            #region step6 TMC的处理
            if (TMC_Items.Count > 1)
            {

            }
            #endregion

            return true;
        }

        /// <summary>
        /// 保存缓存数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        private void FastMode_SaveCacheData(String name, Byte[] content)
        {
            if (true)
            {
                try
                {
                    String cachePath = LogfilePath + ".Cache";
                    if (!Directory.Exists(cachePath))
                    {
                        Directory.CreateDirectory(cachePath);
                    }

                    String cacheFile = Path.Combine(cachePath, name);
                    FileStream fStream = new FileStream(cacheFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write); ;
                    fStream?.Write(content, 0, content.Length);
                    fStream?.Close();
                }
                catch
                {
                    AddLog?.Invoke($"保存缓存文件[{name}]失败");
                }
            }
        }

        #endregion
    }
}