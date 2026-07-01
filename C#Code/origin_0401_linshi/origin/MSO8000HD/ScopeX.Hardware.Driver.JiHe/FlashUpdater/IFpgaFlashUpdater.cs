using PCIeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Updater.Base;
using Microsoft.VisualBasic;

namespace ScopeX.Hardware.Driver
{
    //板卡FLASH支持的升级模式枚举
    public enum FpgaFlashUpdaterMod
    {
        FFU_Normal,
        FFU_FastMod,
        FFU_Unknow = 0xFF
    }

    public class FlashContentAndImageBlock
    {
        /// <summary>
        /// 起始板卡ID
        /// </summary>
        public Int32 TypeID { get; set; }

        /// <summary>
        /// 办卡名字
        /// </summary>
        public String boardName { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public Byte[] content { get; set; }

        /// <summary>
        /// 数据DMA下传后 DDR中偏移地址
        /// </summary>
        public UInt32 contentAtPcieDrrOffset { get; set; }

        public List<ImageBlock> ImageBlocks { get; set; }

    };

    public interface IFpgaFlashUpdater
    {
        Boolean ReadFpgaInsideVersionInfo(Int32 boardIndex, out String? infos);
        Boolean Open(UpdatePackage? updatePackage, String key, out String errorMsg);

        Boolean ReadProductInfo(out ProductInfo productInfo, Int32 boardIndex = 0);
        Boolean WriteProductInfo(ProductInfo productInfo, Int32 boardIndex = 0);

        Boolean AwgCaliDataWrite(Byte[] buffer);
        Boolean AwgCaliDataRead(out Byte[] buffer);

        Boolean MiscCaliDataWrite(Byte[] buffer);
        Boolean MiscCaliDataRead(out Byte[] buffer);

        Boolean TiadcCaliDataWrite(Byte[] buffer);
        Boolean TiadcCaliDataRead(out Byte[] buffer);

        Boolean ChannelCaliDataWrite(Byte[] buffer);
        Boolean ChannelCaliDataRead(out Byte[] buffer);

        Boolean DspCaliDataWrite(Byte[] buffer);
        Boolean DspCaliDataRead(out Byte[] buffer);

        Boolean ReadOptionsInfo(out OptionsInfoBase optionsInfo);
        Boolean WriteOptionsInfo(OptionsInfoBase info);

        Boolean WriteUSBTMCSN(String sn);
        FpgaFlashErrorCode ProductInfo_Clear(Int32 boardIndex = 0);
        UInt32 FLASHID { get; }

        Boolean VerifyFlashID(Int32 BoardIndex);
        FpgaFlashErrorCode VerifyIDCode(Int32 BoardIndex, Byte[] content);
        public List<BaseDataBlock>? FpgaContent_ReadBoardData(UInt32 boardID, List<InfoIndex> infoIndexs);

        List<ImageBlock>? FpgaContent_ReadImageInfo();
        Task<Boolean> FpgaContent_EraseSectors();
        Boolean StatusIsLocked(Int32 BoardIndex, UInt32 state);
        /// <summary>
        /// 初始化信息区
        /// </summary>
        /// <param name="boardID"></param>
        /// <returns></returns>
        FpgaFlashErrorCode FlashInfoZone_Init(Int32 boardID);
        /// <summary>
        /// 擦除信息区
        /// </summary>
        /// <param name="BoardIndex"></param>
        /// <returns></returns>
        FpgaFlashErrorCode FlashInfoZone_Clear(Int32 boardID);

        public InfoZone? FlashInfoZone_Read(Int32 boardID);

        FpgaFlashErrorCode FpgaContent_OnlyWriteBoardData(Int32 boardID, List<ImageBlock> versionInfo);

        FpgaFlashErrorCode FpgaContent_Write(Int32 FlashIndex, Byte[] content, List<ImageBlock> versionInfo);
        Byte[]? FpgaContent_Read(Int32 FlashIndex, Int32 MaxSize = 0);

        //ProductBaseInfo? ProductInfo_Read();
        //FpgaFlashErrorCode ProductInfo_Write(ProductBaseInfo productInfo);

        //Byte[]? CaliData_Read();
        //FpgaFlashErrorCode CaliData_Write(Byte[] content);

        void Close();

        //Int32 PerWriteRegUsedNs => HdIO.PerWriteRegUsedNs;
        Boolean ResetFlashStatusReg(Int32 boardID);
        Int32 PerWriteRegUsedNs { get; set; }
        void QueryPerWriteRegUsedNs();

        Boolean UnLockFlashProtect(List<Int32>? fpgaIndexs);

        Boolean UnLockFlashProtect(List<UpdateItem> updateItem);

        Boolean LockFlashProtect(List<Int32>? fpgaIndexs);

        public Dictionary<Int32, UInt32> QueryFlashsStatus();

        public UInt32? QueryMultipleBootStatus(Int32 boardID);

        public void BindLogFunc(Action<String>? logFunc, Action<String, Boolean>? setupTip = null);
        public void BindLogConf(String logfilePath);

        public InfoZone? ReadBoardInfos(Int32 boardID);

        public FpgaFlashErrorCode FlashInfoZone_Write(Int32 boardID, InfoZone infoZone);

        #region 测试用
        public Byte[]? ReadDataForTest(Int32 boardIndex, UInt32 lengh, UInt32 addr);
        FpgaFlashErrorCode RandomCorruptedImageForTest(Int32 boardIndex, out Int32 length);
        #endregion 测试用


        public void WriteInnoSetupMessageInfos(String boardName, Boolean updateOk);

        public void SetPacketBoardListOnOldUpdater(List<Int32>? boardList);
        #region FastMode
        public FpgaFlashUpdaterMod CheckFpgaFlashUpdaterMod(Int32 BoardIndex);
        public FpgaFlashErrorCode FastMode_VerifyFlashID(Int32 boardIndex);
        public FpgaFlashErrorCode FastMode_VerifyFpgaIDCode(Int32 boardIndex, Byte[] content);
        public Boolean CheckIamgeIsGoldenAndApp(Int32 boardIndex, Byte[] content);
        public Boolean FastMode_UpdateIncludeFlash(Dictionary<Int32/*boardIndex*/, FlashContentAndImageBlock> flashInfo);
        #endregion
    }
}
