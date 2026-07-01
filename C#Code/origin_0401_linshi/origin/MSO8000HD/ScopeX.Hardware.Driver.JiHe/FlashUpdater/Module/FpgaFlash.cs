using PCIeControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Updater.Base;
using System.Text.Json;
using System.Reflection.Metadata;

namespace ScopeX.Hardware.Driver
{
    internal abstract class FpgaFlash
    {
        internal Int32 FLASH_PROTECTED_MARK = 0b111100;

        public Action<String>? AddLog;

        public void WriteLog(String message)
        {
            AddLog?.Invoke(message);
        }

        public static UInt32 FlashSize { get; }

        public static UInt32 FlashMarkID { get; }

        public Int32 FlashProtectedMark { get; }

        public static UInt32 FpgaVarsionInfoStartAtBytes { get; }

        public static UInt32 FpgaVarsionInfoTotalBytes { get; }

        public static UInt32 ProductInfoStartAtBytes { get; }

        public static UInt32 ProductInfoTotalBytes { get; }

        public static UInt32 CaliDataStartAtBytes { get; }

        public static UInt32 CaliDataTotalBytes { get; }

        public static UInt32 CaliDataUsedSectorCount { get; }

        internal FpgaFlash_Registers Registers { init; get; } = new FpgaFlash_Registers();
        protected virtual UInt32 ReadID() => 0;

        protected virtual FpgaFlashErrorCode QueryMultipleBootStatus(out UInt32? status)
        {
            status = null;
            return FpgaFlashErrorCode.NotFound;
        }
        protected virtual FpgaFlashErrorCode EraseOneSector(UInt32 Flash_Address, Int32 maxWaitSecond) => FpgaFlashErrorCode.NotFound;
        protected virtual FpgaFlashErrorCode EraseMultiSectors(UInt32 start_address, UInt32 erase_length, Int32 maxWaitSecond, Action<TaskProgress>? updateProcessTip = null) => FpgaFlashErrorCode.NotFound;
        internal virtual FpgaFlashErrorCode ReadBlock(UInt32 Flash_Address, ref Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null) => FpgaFlashErrorCode.NotFound;
        protected virtual FpgaFlashErrorCode WriteBlock(UInt32 Flash_Address, Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null) => FpgaFlashErrorCode.NotFound;
        internal virtual FpgaFlashErrorCode WriteBlockOneSector(UInt32 Flash_Address, Byte[] content, UInt32 offset, UInt32 bytes)
        {
            var result = EraseOneSector(Flash_Address, 1 * 60);
            //test
            if (result == FpgaFlashErrorCode.Succeed)
                result = WriteBlock(Flash_Address, content, 0, bytes);
            return result;
        }
        protected virtual void AddLogInfo(String msg)
        {
            Debug.WriteLine($"FLASH BoardID-{Registers.BoardID} :{msg}");
            if (AddLog != null)
            {
                AddLog($"FLASH BoardID-{Registers.BoardID} :{msg}");
            }
        }
        public virtual Boolean LockFlashProtect() => true;
        public virtual Boolean UnLockFlashProtect() => true;
        public virtual UInt32 QueryStatus() => 0xFF;

        public virtual FpgaFlashErrorCode VerifyFlashID(ref UInt32 flashID)
        {
            //无论新旧都保障此处始终设置
            HdIO.WriteReg(Registers.SpiClock, (UInt32)Registers.SpiClockSetValue);

            //如果是新模式就返回；因为老路已经走不通
            if (FpgaFlashUpdaterMod.FFU_FastMod == CheckFpgaFlashUpdaterMod())
            {
                AddLogInfo($"Read FlashID: skip current in FFU_FastMod");
                return FpgaFlashErrorCode.Succeed;
            }

            //旧模式继续安装既有逻辑
            UInt32 readBackID = ReadID() & 0xffffff;
            flashID = readBackID;
            AddLogInfo($"Read FlashID:0x{flashID:X}");

            if (readBackID == (Registers.FlashMarkID & 0xffffff))
            {
                return FpgaFlashErrorCode.Succeed;
            }
            else
            {
                return FpgaFlashErrorCode.FLashIDMismatching;
            }
        }

        public virtual List<BaseDataBlock>? DataBlock_Read(List<InfoIndex> infoIndexs)
        {
            if (infoIndexs == null || infoIndexs.Count == 0)
            {
                return null;
            }
            List<BaseDataBlock> dataBlocks = new();
            foreach (var infoIndex in infoIndexs)
            {
                var size = (UInt32)infoIndex.InfoSize * 1024;
                Byte[] content = new Byte[size];
                var result = ReadBlock(infoIndex.InfoAddr, ref content, 0, size);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    AddLogInfo($"DataBlock_Read Error:index[{infoIndex.ID}] result:{result}");
                    continue;
                }
                var dataStr = Encoding.UTF8.GetString(content);
                if (String.IsNullOrWhiteSpace(dataStr))
                {
                    AddLogInfo($"DataBlock_Read Error:dataStr is null");
                    continue;
                }
                try
                {
                    var block = JsonSerializer.Deserialize<ImageBlock>(dataStr);
                    if (block != null)
                        dataBlocks.Add(block);
                }
                catch
                {
                    AddLogInfo($"DataBlock_Read Error:Data error:");
                    AddLogInfo($"{dataStr}");
                    continue;
                }

            }
            return dataBlocks;
        }
        public virtual List<ImageBlock>? ImageInfo_Read()
        {
            #region 索引检查
            var infoZone = InfoZone_Read();
            if (infoZone == null || !infoZone.IndexsCheck())
            {
                AddLogInfo("VersionInfo_Read Error:infoZone not enable");
                return null;
            }

            #endregion 索引检查

            List<ImageBlock> imageBlocks = new();
            for (Int32 i = 0; i < 2; i++)
            {
                ImageBlock imageBlock;
                var infoIndex = infoZone.InfoIndexs[i];
                var length = (UInt32)infoIndex.InfoSize;
                if (length < Registers.FlashMinInfoSizeByte)
                {
                    length = Registers.FlashMinInfoSizeByte;
                }
                Byte[] content = new Byte[length];
                var result = ReadBlock(infoIndex.InfoAddr, ref content, 0, length);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    AddLogInfo($"ImageInfo_Read Error:index[{i}] result:{result}");
                    continue;
                }
                var realLengh = GetDataRealLength(content);
                if (realLengh == 0)
                {
                    AddLogInfo($"ImageInfo_Read Error:datalen is 0");
                    continue;
                }
                var realContent = new Byte[realLengh];
                Array.Copy(content, realContent, realLengh);
                var dataStr = Encoding.UTF8.GetString(realContent);
                if (String.IsNullOrWhiteSpace(dataStr))
                {
                    AddLogInfo($"ImageInfo_Read Error:dataStr is null");
                    continue;
                }
                try
                {
                    imageBlock = JsonSerializer.Deserialize<ImageBlock>(dataStr);
                    imageBlocks.Add(imageBlock);
                }
                catch
                {
                    AddLogInfo($"ImageInfo_Read Error:Data error:");
                    AddLogInfo($"{dataStr}");
                    continue;
                }

            }

            return imageBlocks;
        }

        public virtual FpgaFlashErrorCode InfoZone_Init()
        {
            var result = InfoZone_Clear();
            if (result != FpgaFlashErrorCode.Succeed)
            {
                AddLogInfo($"InfoZone_Init - InfoZone_Clear ERROR:{result}");
                return result;
            }
            InfoZone infoZone = new InfoZone();
            infoZone.Init();
            result = InfoZone_Write(infoZone);
            if (result != FpgaFlashErrorCode.Succeed)
            {
                AddLogInfo($"InfoZone_Init - InfoZone_Write ERROR:{result}");
            }
            return result;
        }
        public virtual FpgaFlashErrorCode InfoZone_Clear()
        {
            var fpgaAddr = Registers.FpgaInfoZoneStartAddr;

            return EraseMultiSectors(fpgaAddr, Registers.FpgaInfoZoneTotalBytes, 60);
        }
        public virtual FpgaFlashErrorCode InfoZone_Write(InfoZone infoZone)
        {
            if (infoZone == null || infoZone.InfoIndexs == null)
            {
                return FpgaFlashErrorCode.InfoError;
            }
            FpgaFlashErrorCode result = FpgaFlashErrorCode.NotFound;
            var fpgaAddr = Registers.FpgaInfoZoneStartAddr;
            for (Int32 i = 0; i < infoZone.InfoIndexs.Count; i++)
            {
                Byte[]? content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(infoZone.InfoIndexs[i]));
                if (content == null)
                    return FpgaFlashErrorCode.InfoError;
                UInt32 writeBytes = content.Length < Registers.FpgaInfoZoneTotalBytes ? (UInt32)content.Length : Registers.FpgaInfoZoneTotalBytes;
                result = WriteBlockOneSector(fpgaAddr, content, 0, writeBytes);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    AddLogInfo($"InfoZone_Write Error:Addr:0x{fpgaAddr:X},Bytes:{writeBytes}");
                    return result;
                }
                fpgaAddr += UpdaterBaseConstants.FLASH_MIN_INFO_SIZE_BYTE;
            }
            return result;
            //Byte[]? content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(infoZone));
            //if (content == null)
            //    return FpgaFlashErrorCode.InfoError;
            //UInt32 writeBytes = content.Length < Registers.FpgaVarsionInfoTotalBytes ? (UInt32)content.Length : Registers.FpgaVarsionInfoTotalBytes;
            //return WriteBlockOneSector(Registers.FpgaVarsionInfoStartAtBytes, content, 0, writeBytes);
        }

        public virtual FpgaFlashErrorCode ProductInfo_Clear()
        {
            var fpgaAddr = Registers.ProductInfoStartAtBytes;

            return EraseMultiSectors(fpgaAddr, Registers.ProductInfoTotalBytes, 60);
        }

        public virtual FpgaFlashErrorCode ProductInfo_Write(ProductInfo infoData, Boolean isOnlySN)
        {
            if (infoData == null || infoData.SerialNumber == null)
            {
                return FpgaFlashErrorCode.InfoError;
            }
            FpgaFlashErrorCode result = FpgaFlashErrorCode.NotFound;
            var fpgaAddr = Registers.ProductInfoStartAtBytes;
            if (fpgaAddr == 0)
            {
                return FpgaFlashErrorCode.ErrorAddr;
            }
            Byte[]? content = infoData.InfosToBytes(isOnlySN);
            if (content == null)
                return FpgaFlashErrorCode.InfoError;
            UInt32 writeBytes = content.Length < Registers.ProductInfoTotalBytes ? (UInt32)content.Length : Registers.ProductInfoTotalBytes;
            result = WriteBlockOneSector(fpgaAddr, content, 0, writeBytes);
            if (result != FpgaFlashErrorCode.Succeed)
            {
                AddLogInfo($"ProductInfo_Write Error:Addr:0x{fpgaAddr:X},Bytes:{writeBytes}");
                return result;
            }

            return result;

        }

        public virtual FpgaFlashErrorCode OptionsInfo_Write(OptionsInfoBase infoData)
        {
            if (infoData == null)
            {
                return FpgaFlashErrorCode.InfoError;
            }
            FpgaFlashErrorCode result = FpgaFlashErrorCode.NotFound;
            var fpgaAddr = Registers.OptionsInfoStartAtBytes;
            if (fpgaAddr == 0)
            {
                return FpgaFlashErrorCode.ErrorAddr;
            }
            Byte[]? content = infoData.InfosToBytes();
            if (content == null)
            {
                return FpgaFlashErrorCode.InfoError;
            }

            UInt32 writeBytes = content.Length < Registers.OptionsInfoTotalBytes ? (UInt32)content.Length : Registers.OptionsInfoTotalBytes;
            result = WriteBlockOneSector(fpgaAddr, content, 0, writeBytes);
            if (result != FpgaFlashErrorCode.Succeed)
            {
                AddLogInfo($"OptionsInfo_Write Error:Addr:0x{fpgaAddr:X},Bytes:{writeBytes}");
                return result;
            }

            return result;

        }

        public UInt32 QueryMultipleBootStatus()
        {
            return HdIO.ReadReg(Registers.FlashUpDateGoldenFlag);
        }

        internal Int32 GetDataRealLength(Byte[] content)
        {
            if (content.Length == 0 || content[0] == 0xFF)
            {
                return 0;
            }
            Int32 realLengh = 0;
            for (Int32 index = content.Length - 1; index >= 0; index--)
            {
                if (content[index] == 0xFF || content[index] == 0x00)
                {

                }
                else
                //    if (content[index] != 0xFF)
                {
                    realLengh = index + 1;
                    break;
                }
            }
            return realLengh;
        }
        public virtual InfoZone? InfoZone_Read()
        {
            InfoZone infoZone = new InfoZone();
            infoZone.InfoIndexs = new();
            var fpgaAddr = Registers.FpgaInfoZoneStartAddr;
            var totalCount = UpdaterBaseConstants.FLASH_TOTAL_INFO_COUNT;
            for (Int32 i = 0; i < totalCount; i++)
            {
                Byte[] content = new Byte[Registers.FlashMinInfoSizeByte];
                var errorCode = ReadBlock(fpgaAddr, ref content, 0, Registers.FlashMinInfoSizeByte);

                if (errorCode == FpgaFlashErrorCode.Succeed && content != null)
                {
                    try
                    {
                        var realLengh = GetDataRealLength(content);
                        if (realLengh == 0)
                        {
                            continue;
                        }
                        var realContent = new Byte[realLengh];
                        Array.Copy(content, realContent, realLengh);
                        var test = Encoding.UTF8.GetString(realContent).Trim();
                        var infoIndex = JsonSerializer.Deserialize<InfoIndex?>(Encoding.UTF8.GetString(realContent));
                        if (infoIndex != null)
                        {
                            infoZone.InfoIndexs.Add((InfoIndex)infoIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddLogInfo($"InfoZone_Read Error: Addr:0x{fpgaAddr:X}");
                        break;
                    }
                }
                fpgaAddr += Registers.FlashMinInfoSizeByte;
            }
            if (infoZone.InfoIndexs != null)
            {
                return infoZone;
            }
            return null;
        }
        public virtual ProductInfo? ProductInfo_Read()
        {
            ProductInfo info = new();

            var fpgaAddr = Registers.ProductInfoStartAtBytes;
            if (fpgaAddr == 0)
            {
                return null;
            }
            Byte[] content = new Byte[Registers.ProductInfoTotalBytes];
            var errorCode = ReadBlock(fpgaAddr, ref content, 0, Registers.ProductInfoTotalBytes);

            if (errorCode == FpgaFlashErrorCode.Succeed && content != null)
            {
                try
                {
                    var realLengh = GetDataRealLength(content);
                    if (realLengh == 0)
                    {
                        return null;
                    }
                    var realContent = new Byte[realLengh];
                    Array.Copy(content, realContent, realLengh);
                    //var test = Encoding.UTF8.GetString(realContent).Trim();
                    info.BytesToInfos(content);
                }
                catch (Exception ex)
                {
                    AddLogInfo($" ProductInfo_Read Error: Addr:0x{fpgaAddr:X}");
                    return null;
                }
            }

            return info;
        }

        public virtual OptionsInfoBase? OptionsInfo_Read()
        {
            OptionsInfoBase info = new();

            var fpgaAddr = Registers.OptionsInfoStartAtBytes;

            if (fpgaAddr == 0)
            {
                return null;
            }
            Byte[] content = new Byte[Registers.OptionsInfoTotalBytes];
            var errorCode = ReadBlock(fpgaAddr, ref content, 0, Registers.OptionsInfoTotalBytes);
            if (errorCode == FpgaFlashErrorCode.Succeed && content != null)
            {
                try
                {
                    Int32 realLengh = GetDataRealLength(content);
                    if (realLengh == 0)
                    {
                        return null;
                    }
                    var realContent = new Byte[realLengh];
                    Array.Copy(content, realContent, realLengh);
                    //var test = Encoding.UTF8.GetString(realContent).Trim();
                    info.BytesToInfos(content);
                }
                catch (Exception ex)
                {
                    AddLogInfo($" OptionsInfo_Read Error: Addr:0x{fpgaAddr:X}");
                    return null;
                }
            }

            return info;
        }
        public virtual Boolean ReadFpgaInsideVersionInfo(out String? infos)
        {
            infos = null;
            return false;
        }

        public virtual FpgaFlashErrorCode BoardData_Write(List<ImageBlock> blocks, Action<TaskProgress>? updateProcessBar = null)
        {
            #region 检查参数
            if (blocks == null || blocks.Count == 0)
            {
                return FpgaFlashErrorCode.InfoError;
            }
            #endregion  检查参数

            #region 索引检查
            var infoZone = InfoZone_Read();
            if (infoZone == null)
            {
                return FpgaFlashErrorCode.NullInfoZone;
            }
            if (!infoZone.IndexsCheck())
            {
                return FpgaFlashErrorCode.ErrorInfoZone;
            }

            #endregion 索引检查

            foreach (var imageBlock in blocks)
            {
                var forInfoType = imageBlock.FirmwareType == FirmwareType.GoldenImage ? InfoType.GoldenInfo : InfoType.AppInfo;
                var infoIndex = infoZone.InfoIndexs.FirstOrDefault(info => info.InfoType == forInfoType);
                if (infoIndex == null)
                {
                    AddLogInfo($"VersionInfo_Write Error:InfoType:{forInfoType} ");
                    return FpgaFlashErrorCode.ErrorInfoZone;
                }
                var addr = infoIndex.InfoAddr;
                if (addr < Registers.FlashDataStartAddr)
                {
                    AddLogInfo($"VersionInfo_Write Waring:flashAddr:0x{infoIndex.InfoAddr:X} not enable");
                    addr = Registers.FlashDataStartAddr;
                }
                Byte[]? content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(imageBlock));
                if (content == null)
                    return FpgaFlashErrorCode.InfoError;
                UInt32 writeBytes = content.Length < Registers.FlashDataTotalBytes ? (UInt32)content.Length : Registers.FlashDataTotalBytes;
                var result = WriteBlockOneSector(addr, content, 0, writeBytes);
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    return result;
                }
            }
            return FpgaFlashErrorCode.Succeed;
        }
        public virtual async Task<FpgaFlashErrorCode> Content_EraseSectorsAsync(Int32 maxWaitSecond, Boolean isGoldenImage = false, Action<TaskProgress>? updateProcessBar = null)
        {
            if (isGoldenImage)
            {

                return await Task.Run(() => EraseMultiSectors(Registers.FlashImageGoldenStartAddr, Registers.FlashImageGoldenTotalBytes, maxWaitSecond, updateProcessBar));

            }
            else
            {
                return await Task.Run(() => EraseMultiSectors(Registers.FlashImageAppStartAddr, Registers.FlashImageAppTotalBytes, maxWaitSecond, updateProcessBar));
            }
        }

        public virtual Byte[]? Content_Read(UInt32 startAddress, UInt32 readSize)
        {
            Byte[]? content = new Byte[readSize];
            if (ReadBlock(startAddress, ref content, 0, readSize) == FpgaFlashErrorCode.Succeed)
                return content;
            return null;
        }
        public virtual FpgaFlashErrorCode Content_Write(Byte[] content, Boolean isGoldenImage = false, Action<TaskProgress>? updateProcessBar = null)
        {
            UInt32 imageStartAddress = Registers.FlashImageAppStartAddr;
            //如果是Gloden和app的合并bing，则从0地址开始
            if (CheckIamgeIsGoldenAndApp(content))
            {
                imageStartAddress = 0;
                WriteLog($"固件写入：失败 不允许更新Golden区域");
                return FpgaFlashErrorCode.GeneralError;
            }
            return WriteBlock(imageStartAddress, content, 0, (UInt32)content.Length, updateProcessBar);
        }
        public virtual Boolean ResetFlashStatusReg() => false;
        public virtual Int32 GetFlashProtectMaskID()
        {
            return FLASH_PROTECTED_MARK;
        }

        #region 2024 Fast Mode
        public virtual UInt32 BoardID { get; init; }
        public virtual Int32 SpiClockSetValue { get; init; }
        public virtual Int32 FlashID { get; init; }
        #endregion

        public Boolean CheckIamgeContain(Byte[] content)
        {
            ///检查content中是否含有Golden
            ///
            return false;
        }

        public Boolean CheckIamgeIsGoldenAndApp(Byte[] content)
        {
            if (content.Length < Registers.FlashImageAppStartAddr + 0x300)
            {
                return false;
            }

            #region FlashImageAppStartAddr开始的前256字节开始的128字节一定为0xff，之后的256字节字符的256字节一定会不全部是0xff
            Boolean bIsGoldenAndApp = true;
            for (UInt32 i = Registers.FlashImageAppStartAddr - 0x100; i < Registers.FlashImageAppStartAddr - 0x80; i++)
            {
                if (content[i] != 0xff)
                {
                    bIsGoldenAndApp = false;
                    break;
                }
            }

            if (bIsGoldenAndApp)
            {
                Int32 value_0xffCount = 0;
                for (UInt32 i = Registers.FlashImageAppStartAddr + 0x100; i < Registers.FlashImageAppStartAddr + 0x100 + 0x80; i++)
                {
                    if (content[i] == 0xff)
                    {
                        value_0xffCount++;
                    }
                }
                if (value_0xffCount == 0x80)
                {
                    bIsGoldenAndApp = false;
                }
            }
            #endregion

            #region 查找IDCodeVerify,如果是Gloden,必须在 FlashImageAppStartAddr 开始后的512字节内还有一次IDCodeVerify
            if (bIsGoldenAndApp)
            {
                String HeaderStr = "";
                for (Int32 i = 0; i < 0x200; i++)
                {
                    HeaderStr += $"_{content[Registers.FlashImageAppStartAddr + i].ToString("X").PadLeft(2, '0')}";
                }
                bIsGoldenAndApp = HeaderStr.IndexOf(Registers.IDCodeVerify, StringComparison.Ordinal) > 0 ? true : false;
            }
            #endregion
            return bIsGoldenAndApp;
        }

        //读ID
        public FpgaFlashUpdaterMod CheckFpgaFlashUpdaterMod()
        {
            UInt32 flashid_l16 = 0;
            UInt32 flashid_h16 = 0;

            HdIO.WriteReg(Registers.FastMode_ActionReset, (UInt32)0x00);
            HdIO.WriteReg(Registers.FastMode_ActionReset, (UInt32)0x01); Thread.Sleep(1);
            HdIO.WriteReg(Registers.FastMode_ActionReset, (UInt32)0x00);

            Thread.Sleep(1000);//延时一秒 避免板卡来不及

            //读三次避免回路未同步到位
            flashid_l16 = HdIO.ReadReg(Registers.FastMode_FlashID_L16);
            flashid_l16 = HdIO.ReadReg(Registers.FastMode_FlashID_L16);
            flashid_l16 = HdIO.ReadReg(Registers.FastMode_FlashID_L16);

            //读三次避免回路未同步到位
            flashid_h16 = HdIO.ReadReg(Registers.FastMode_FlashID_H16);
            flashid_h16 = HdIO.ReadReg(Registers.FastMode_FlashID_H16);
            flashid_h16 = HdIO.ReadReg(Registers.FastMode_FlashID_H16);

            var flashid = (flashid_l16) | ((flashid_h16 & 0x00FF) << 16);

            if (flashid == 00)
            {
                return FpgaFlashUpdaterMod.FFU_Normal;
            }
            else if (flashid == Registers.FlashMarkID)
            {
                return FpgaFlashUpdaterMod.FFU_FastMod;
            }
            else
            {
                return FpgaFlashUpdaterMod.FFU_Normal;
            }
        }
    }
    internal abstract class RootFpgaFlash : FpgaFlash
    {


        //public virtual Byte[]? CaliData_Read()
        //{
        //    Byte[]? content = new Byte[Registers.CaliDataTotalBytes];
        //    if (ReadBlock(Registers.CaliDataStartAtBytes, ref content, 0, Registers.CaliDataTotalBytes) == FpgaFlashErrorCode.Succeed)
        //        return content;
        //    return null;
        //}
        //public virtual FpgaFlashErrorCode CaliData_Write(Byte[] caliData)
        //{
        //    UInt32 perSectorBytes = Registers.CaliDataTotalBytes / Registers.CaliDataUsedSectorCount;
        //    for (Int32 sectorIndex = 0; sectorIndex < Registers.CaliDataUsedSectorCount; sectorIndex++)
        //    {
        //        var result = EraseOneSector((UInt32)(Registers.CaliDataStartAtBytes + sectorIndex * perSectorBytes), 1 * 60);
        //        if (result != FpgaFlashErrorCode.Succeed)
        //            return result;
        //    }
        //    UInt32 writeBytes = caliData.Length < Registers.CaliDataTotalBytes ? (UInt32)caliData.Length : Registers.CaliDataTotalBytes;
        //    return WriteBlock(Registers.CaliDataStartAtBytes, caliData, 0, writeBytes);
        //}

        //public virtual ProductBaseInfo? ProductInfo_Read()
        //{
        //    Byte[] content = new Byte[Registers.ProductInfoTotalBytes];
        //    if (ReadBlock(Registers.ProductInfoStartAtBytes, ref content, 0, Registers.ProductInfoTotalBytes) == FpgaFlashErrorCode.Succeed)
        //    {
        //        //test 22.11.15
        //        AddLogInfo("ProductInfo:");
        //        Int32 i = 0;
        //        foreach (Byte data in content)
        //        {
        //            Debug.Write($"{data:X2} ");
        //            i++;
        //            if (i > 50)
        //            {
        //                break;
        //            }
        //        }
        //        AddLogInfo("\n");
        //        return ProductBaseInfo.Deserialize(content);
        //    }
        //    return null;
        //}
        //public virtual FpgaFlashErrorCode ProductInfo_Write(ProductBaseInfo productInfo)
        //{
        //    Byte[]? content = ProductBaseInfo.Serialize(productInfo);
        //    if (content == null)
        //        return FpgaFlashErrorCode.InfoError;
        //    UInt32 writeBytes = content.Length < Registers.ProductInfoTotalBytes ? (UInt32)content.Length : Registers.ProductInfoTotalBytes;
        //    return WriteBlockOneSector(Registers.ProductInfoStartAtBytes, content, 0, writeBytes);
        //}

    }


}
