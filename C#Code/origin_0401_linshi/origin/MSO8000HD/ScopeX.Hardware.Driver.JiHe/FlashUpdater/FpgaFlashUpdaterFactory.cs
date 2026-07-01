using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Updater.Base;

namespace ScopeX.Hardware.Driver;

public class FpgaFlashUpdaterFactory
{

    /// <summary>
    /// 与8000HD硬件结构一致
    /// </summary>
    /// <returns></returns>
    private static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateMSO7000HDProduct()
    {
        return CreateMSO8000HDProduct();
    }

    /// <summary>
    /// MSO8000HD 相关配置 24.8.13 ljw
    /// </summary>
    /// <returns></returns>
    private static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateMSO8000HDProduct()
    {
        //const Int32 MX25U12832F_MarkID = 0xC22538;
        //const Int32 MP25P128_MarkID = 0x012018;
        const Int32 MX66U1G45G_MarkID = 0xC2253B;
        const Int32 MX25U51245G_MarkID = 0xC2253A;
		const Int32 W25Q64_MarkID = 0x4017A;
		const UInt32 SIZE_64K_SECTOR = UpdaterBaseConstants.SIZE_64K_SECTOR;
		//const UInt32 MX25U12832F_FLASHSIZE = 16777216;
		const UInt32 MT25U51245G_FLASHSIZE = 67_108_864;
        const UInt32 MX66U1G45G_FLASHSIZE = MT25U51245G_FLASHSIZE * 2;

        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE / 2 - SIZE_64K_SECTOR;
        const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;
		//const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = SIZE_64K_SECTOR * 4;

		const UInt32 PROC_FPGA_CONTENT_START_ADDR = 0;
		const UInt32 PROC_FPGA_CONTENT_TOTAL_BYTES = MT25U51245G_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
		const UInt32 PROC_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

		const UInt32 ACQBD_FPGA_CONTENT_START_ADDR = 0;
		const UInt32 ACQBD_FPGA_CONTENT_TOTAL_BYTES = MX66U1G45G_FLASHSIZE - SIZE_64K_SECTOR - ACQBD_FPGA_CONTENT_START_ADDR;
		const UInt32 ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

		const UInt32 FLASH_DATA_STARTAT_BYTES = 0x0270_0000;
		const UInt32 FLASH_DATA_TOTAL_BYTES = MT25U51245G_FLASHSIZE - FLASH_DATA_STARTAT_BYTES;
		const UInt32 FLASH_MIN_INFO_SIZE_BYTE = UpdaterBaseConstants.FLASH_MIN_INFO_SIZE_BYTE;

		const UInt32 FLASH_IMAGE_APP_START_ADDR = 0x0120_0000;
		const UInt32 FLASH_IMAGE_APP_TOTAL_BYTES = UpdaterBaseConstants.FLASH_IMAGE_APP_MAX_SIZE_BYTE;

		const UInt32 ACQ_FLASH_IMAGE_APP_START_ADDR = 0x0284_0000;
		const UInt32 ACQ_FLASH_IMAGE_APP_TOTAL_BYTES = 0x0284_0000;

		const UInt32 FPGA_INFO_INDEX_TOTAL_BYTES = UpdaterBaseConstants.FLASH_TOTAL_INFO_SIZE_BYTE;

        // Product(PCIE) 在第一批试产机器 选件信息存放在处理板的这个位置，后续搬移到PCIE的选件位置 
        // = 0x25F_0000
        const UInt32 PRODUCT_INFO_STARTAT_BYTES = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES - SIZE_64K_SECTOR;
		const UInt32 PRODUCT_INFO_TOTAL_BYTES = SIZE_64K_SECTOR;

        // Options(PCIE)
        const UInt32 OPTIONS_INFO_STARTAT_BYTES = 0x26F_0000;
        const UInt32 OPTIONS_INFO_TOTAL_BYTES = SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_TOTAL_BYTES = 0 * SIZE_64K_SECTOR;

		const UInt32 CALI_DATA_STARTAT_BYTES = PRODUCT_INFO_STARTAT_BYTES - CALI_DATA_TOTAL_BYTES - SIZE_64K_SECTOR * 2;

		const UInt32 CALI_DATA_USED_SECTOR_COUNT = 0 * SIZE_64K_SECTOR;

		const Int16 SPI_CLOCK = 6;//6;
        return new Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>()
        {
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE] = new Flash_MX25U51245G(new()
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)PcieBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenTotalBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataStartAtBytes   /**/= 0x2700000,//CALI_DATA_STARTAT_BYTES,
                CaliDataTotalBytes     /**/= 23 * 1024 * 1024,//CALI_DATA_TOTAL_BYTES,
                CaliDataUsedSectorCount/**/= CALI_DATA_USED_SECTOR_COUNT,
                FpgaInfoZoneStartAddr  /**/= PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes /**/= FPGA_INFO_INDEX_TOTAL_BYTES,
                                       /**/
                ProductInfoStartAtBytes/**/= PRODUCT_INFO_STARTAT_BYTES,
                ProductInfoTotalBytes  /**/= PRODUCT_INFO_TOTAL_BYTES,
                OptionsInfoStartAtBytes/**/= OPTIONS_INFO_STARTAT_BYTES,
                OptionsInfoTotalBytes  /**/= OPTIONS_INFO_TOTAL_BYTES,
                                       /**/
                FlashImageAppStartAddr /**/= FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes/**/= FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr     /**/= FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes    /**/= FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte   /**/= FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag  /**/= (UInt32)PcieBdReg.R.FPGAFlashUpdater_UpdateFlag,
                //AWG 2M
                CaliDataStartAtBytes_AWG = 0x270_0000,
                CaliDataTotalAtBytes_AWG = 2 * 1024 * 1024,

                //Misc 2M
                CaliDataStartAtBytes_MISC = 0x292_0000,
                CaliDataTotalAtBytes_MISC = 2 * 1024 * 1024,

                //Dsp 4M
                CaliDataStartAtBytes_DSP = 0x2B4_0000,
                CaliDataTotalAtBytes_DSP = 4 * 1024 * 1024,

                //Channel 1M
                CaliDataStartAtBytes_Channel = 0x2F6_0000,
                CaliDataTotalAtBytes_Channel = 1 * 1024 * 1024,

                //TiAdc 1M
                CaliDataStartAtBytes_TiAdc = 0x308_0000,
                CaliDataTotalAtBytes_TiAdc = 1 * 1024 * 1024,

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)PcieBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= (UInt32)PcieBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= (UInt32)PcieBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= (UInt32)PcieBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= (UInt32)PcieBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= (UInt32)PcieBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= (UInt32)PcieBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= (UInt32)PcieBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= (UInt32)PcieBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= (UInt32)PcieBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= (UInt32)PcieBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion			
			

			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7] = new Flash_MX25U51245G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
				//FlashMarkID = MP25P128_MarkID,
				FlashMarkID = MX25U51245G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
				SpiClockSetValue = SPI_CLOCK,
				SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
				WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
				ReadData = (UInt32)ProcBdReg.R.FPGAFlashUpdater_ReadData,
				ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
				WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
				FlashImageGoldenStartAddr = PROC_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = PROC_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                OptionsInfoStartAtBytes = PRODUCT_INFO_STARTAT_BYTES,
                OptionsInfoTotalBytes = PRODUCT_INFO_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag,

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)ProcBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= (UInt32)ProcBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= (UInt32)ProcBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= (UInt32)ProcBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= (UInt32)ProcBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= (UInt32)ProcBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= (UInt32)ProcBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= (UInt32)ProcBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= (UInt32)ProcBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= (UInt32)ProcBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= (UInt32)ProcBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[0],

				FlashImageGoldenStartAddr   = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes  = ACQBD_FPGA_CONTENT_TOTAL_BYTES,

				FpgaInfoZoneStartAddr       = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes      = FPGA_INFO_INDEX_TOTAL_BYTES,

				FlashImageAppStartAddr      = ACQ_FLASH_IMAGE_APP_START_ADDR,
				//FlashImageAppTotalBytes     = /*ACQ_FLASH_IMAGE_APP_TOTAL_BYTES*/0X80_0000,
                FlashImageAppTotalBytes = ACQ_FLASH_IMAGE_APP_TOTAL_BYTES,

                FlashDataStartAddr          = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes         = FLASH_DATA_TOTAL_BYTES,

				FlashMinInfoSizeByte        = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag       = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[0],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = ACQ_FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = ACQ_FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[1],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[2],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/= (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                SpiClockSetValue /**/= 8,
                FlashID          /**/= MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[3],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_00_80_01_00_00_00_13",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = ACQ_FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = ACQ_FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[4],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/= (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                SpiClockSetValue /**/= 8,
                FlashID          /**/= MX25U51245G_MarkID,
            },
            #endregion			
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[5],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/= (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                SpiClockSetValue /**/= 8,
                FlashID          /**/= MX25U51245G_MarkID,
            },
            #endregion
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_00_80_01_00_00_00_13",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = ACQ_FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = ACQ_FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[6],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8] = new Flash_MX66U1G45G(new()
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
				FlashMarkID = MX66U1G45G_MarkID,
				IDCodeVerify = "30_01_80_01_03_82_30_93",
				SpiClock = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				SpiClockSetValue = SPI_CLOCK,
				SS = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_SS) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				WriteData = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				ReadData = ((UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				ReadStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				WriteStart = ((UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart) | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[7],

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                #endregion
            })
            #region FastMode
            {
                BoardID          /**/ = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                SpiClockSetValue /**/ = 8,
                FlashID          /**/ = MX25U51245G_MarkID,
            },
            #endregion
			
			[FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AWG] = new BoardAwg(new() //8000 AWG功能转移到处理板了 
			{
				BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
				//FlashMarkID              /**/ = MP25P128_MarkID,
				FlashMarkID                /**/ = W25Q64_MarkID,
				IDCodeVerify               /**/ = "20_00_00_00_20_00_00_00",
				SpiClock                   /**/ = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
				SpiClockSetValue           /**/ = SPI_CLOCK,
				SS                         /**/ = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
				WriteData                  /**/ = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
				ReadData                   /**/ = (UInt32)ProcBdReg.R.Awg_data_awg_rd,
				ReadStart                  /**/ = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
				WriteStart                 /**/ = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
				FlashImageGoldenStartAddr  /**/ = PROC_FPGA_CONTENT_START_ADDR,
				FlashImageGoldenTotalBytes /**/ = PROC_FPGA_CONTENT_TOTAL_BYTES,
				FpgaInfoZoneStartAddr      /**/ = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
				FpgaInfoZoneTotalBytes     /**/ = FPGA_INFO_INDEX_TOTAL_BYTES,
				FlashImageAppStartAddr     /**/ = FLASH_IMAGE_APP_START_ADDR,
				FlashImageAppTotalBytes    /**/ = FLASH_IMAGE_APP_TOTAL_BYTES,
				FlashDataStartAddr         /**/ = FLASH_DATA_STARTAT_BYTES,
				FlashDataTotalBytes        /**/ = FLASH_DATA_TOTAL_BYTES,
				FlashMinInfoSizeByte       /**/ = FLASH_MIN_INFO_SIZE_BYTE,
				FlashUpDateGoldenFlag      /**/ = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag,
			}),
		};
	}
    /// <summary>
    /// UPO7000L 相关配置 24.3.11
    /// </summary>
    /// <returns></returns>
    static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateUPO7000LProduct()
    {
        //const Int32 MX25U12832F_MarkID = 0xC22538;
        //const Int32 MP25P128_MarkID = 0x012018;
        const Int32 MX25U51245G_MarkID = 0xC2253A;
        const Int32 W25Q64_MarkID = 0xEF4017;
        const UInt32 SIZE_64K_SECTOR = UpdaterBaseConstants.SIZE_64K_SECTOR;
        //const UInt32 MX25U12832F_FLASHSIZE = 16777216;
        const UInt32 MT25U51245G_FLASHSIZE = 67_108_864;
        const UInt32 W25Q64_FLASHSIZE = 0x80_0000;

        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE / 2 - SIZE_64K_SECTOR;
        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;
        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = SIZE_64K_SECTOR * 4;
        const UInt32 PCIE_FPGA_VARSION_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 4; //256K
        const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE - PCIE_FPGA_VARSION_INFO_TOTAL_BYTES;

        const UInt32 PRO_FPGA_VARSION_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 4; //256K
        const UInt32 PRO_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE - PRO_FPGA_VARSION_INFO_TOTAL_BYTES;

        const UInt32 AWG_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 AWG_FPGA_CONTENT_TOTAL_BYTES = W25Q64_FLASHSIZE - AWG_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        //const UInt32 AWG_FPGA_APP_START_ADDR = 0x35_0000;
        const UInt32 AWG_FPGA_APP_START_ADDR = 0x40_0000; //23.10
        const UInt32 AWG_FPGA_VARSION_INFO_STARTAT_BYTES = 0x076_0000;
        const UInt32 AWG_FPGA_APP_TOTAL_BYTES = AWG_FPGA_VARSION_INFO_STARTAT_BYTES - AWG_FPGA_APP_START_ADDR;

        const UInt32 PROC_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 PROC_FPGA_CONTENT_TOTAL_BYTES = MT25U51245G_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        //const UInt32 PROC_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

        const UInt32 ACQBD_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 ACQBD_FPGA_CONTENT_TOTAL_BYTES = MT25U51245G_FLASHSIZE - SIZE_64K_SECTOR - ACQBD_FPGA_CONTENT_START_ADDR;
        //const UInt32 ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

        const UInt32 FLASH_DATA_STARTAT_BYTES = 0x0270_0000;
        const UInt32 FLASH_DATA_TOTAL_BYTES = MT25U51245G_FLASHSIZE - FLASH_DATA_STARTAT_BYTES;
        const UInt32 FLASH_MIN_INFO_SIZE_BYTE = UpdaterBaseConstants.FLASH_MIN_INFO_SIZE_BYTE;
        const UInt32 FLASH_IMAGE_APP_START_ADDR = 0x0120_0000;
        const UInt32 FLASH_IMAGE_APP_TOTAL_BYTES = UpdaterBaseConstants.FLASH_IMAGE_APP_MAX_SIZE_BYTE;
        const UInt32 FPGA_INFO_INDEX_TOTAL_BYTES = UpdaterBaseConstants.FLASH_TOTAL_INFO_SIZE_BYTE;

        const UInt32 OPTIONS_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 8; //512K
        const UInt32 OPTIONS_INFO_STARTAT_BYTES = 0x0250_0000;


        const UInt32 PRODUCT_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 8; //512K

        const UInt32 PRODUCT_INFO_STARTAT_BYTES = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES - (SIZE_64K_SECTOR * 2) - PRODUCT_INFO_TOTAL_BYTES;

        const UInt32 CALI_DATA_TOTAL_BYTES = 0 * SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_STARTAT_BYTES = PRODUCT_INFO_STARTAT_BYTES - CALI_DATA_TOTAL_BYTES - (SIZE_64K_SECTOR * 2);

        const UInt32 CALI_DATA_USED_SECTOR_COUNT = 0 * SIZE_64K_SECTOR;

        const Int16 SPI_CLOCK = 6; //6;
        return new Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>()
        {
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
               
                FlashMarkID      /**/ = MX25U51245G_MarkID,
                IDCodeVerify     /**/ = "02_2A_3F_E5_30_01_C0_01",
                SpiClock         /**/ = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue /**/ = SPI_CLOCK,
                SS               /**/ = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SS,
                WriteData        /**/ = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData         /**/ = (UInt32)PcieBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart        /**/ = (UInt32)PcieBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart       /**/ = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenTotalBytes /**/ = CALI_DATA_STARTAT_BYTES,
                CaliDataStartAtBytes       /**/ = CALI_DATA_STARTAT_BYTES,
                CaliDataTotalBytes         /**/ = CALI_DATA_TOTAL_BYTES,
                CaliDataUsedSectorCount    /**/ = CALI_DATA_USED_SECTOR_COUNT,
                FpgaInfoZoneStartAddr      /**/ = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes     /**/ = FPGA_INFO_INDEX_TOTAL_BYTES,
                ProductInfoStartAtBytes    /**/ = PRODUCT_INFO_STARTAT_BYTES,
                ProductInfoTotalBytes      /**/ = PRODUCT_INFO_TOTAL_BYTES,
                OptionsInfoStartAtBytes    /**/ = OPTIONS_INFO_STARTAT_BYTES,
                OptionsInfoTotalBytes      /**/ = OPTIONS_INFO_TOTAL_BYTES,
                FlashImageAppStartAddr     /**/ = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes    /**/ = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr         /**/ = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes        /**/ = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte       /**/ = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag      /**/ = (UInt32)PcieBdReg.R.FPGAFlashUpdater_UpdateFlag,

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= (UInt32)0, //ProcBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= (UInt32)0, //PcieBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= (UInt32)0, //PcieBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion

            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = PROC_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = PROC_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                OptionsInfoStartAtBytes = OPTIONS_INFO_STARTAT_BYTES,
                OptionsInfoTotalBytes = OPTIONS_INFO_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag,

                #region FastMode
                FastMode_ActionReset          /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= (UInt32)0, //ProcBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= (UInt32)0, //PcieBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= (UInt32)0, //PcieBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= (UInt32)0, //PcieBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_20_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[0],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[1],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[2],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[3],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[4],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[5],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[6],

                #region FastMode
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                //常量值
                BoardID          /**/= (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                FlashMarkID      /**/= MX25U51245G_MarkID,
                IDCodeVerify     /**/= "30_01_80_01_03_82_30_93",

                //配置项
                SpiClockSetValue              /**/= SPI_CLOCK,                           //
                FlashImageGoldenStartAddr     /**/= ACQBD_FPGA_CONTENT_START_ADDR,       //
                FlashImageGoldenTotalBytes    /**/= ACQBD_FPGA_CONTENT_TOTAL_BYTES,      //
                FpgaInfoZoneStartAddr         /**/= PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,//
                FpgaInfoZoneTotalBytes        /**/= FPGA_INFO_INDEX_TOTAL_BYTES,         //
                FlashImageAppStartAddr        /**/= FLASH_IMAGE_APP_START_ADDR,          //
                FlashImageAppTotalBytes       /**/= FLASH_IMAGE_APP_TOTAL_BYTES,         //
                FlashDataStartAddr            /**/= FLASH_DATA_STARTAT_BYTES,            //
                FlashDataTotalBytes           /**/= FLASH_DATA_TOTAL_BYTES,              //
                FlashMinInfoSizeByte          /**/= FLASH_MIN_INFO_SIZE_BYTE,            //

                #region Old Mode（Flash更新旧方案使用到的寄存器）
                SpiClock                      /**/= (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock        /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                SS                            /**/= (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS              /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteData                     /**/= (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadData                      /**/= (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData        /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadStart                     /**/= (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteStart                    /**/= (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FlashUpDateGoldenFlag         /**/= (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                #endregion

                #region FastMode（Flash更新新方案使用到的寄存器）
                //FastMode_ActionReset          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_ActionCode           /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_ActionStart          /**/= (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_ActionStatus         /**/= (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashStartAddressL16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashStartAddressH16 /**/= (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashEndAddressL16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashEndAddressH16   /**/= (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_WhichFlash           /**/= (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashID_L16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                //FastMode_FlashID_H16          /**/= (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AWG] = new BoardAwg(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AWG,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = W25Q64_MarkID,
                IDCodeVerify = "20_00_00_00_20_00_00_00",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.Awg_data_awg_rd,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = AWG_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = AWG_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = W25Q64_FLASHSIZE - AWG_FPGA_VARSION_INFO_STARTAT_BYTES,
                FlashImageAppStartAddr = AWG_FPGA_APP_START_ADDR,
                FlashImageAppTotalBytes = AWG_FPGA_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag
            })
        };
    }
    /// <summary>
    /// MSO7000X 相关配置 23.3 新机箱
    /// </summary>
    /// <returns></returns>
    static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateMSO7000XProduct()
    {
        //const Int32 MX25U12832F_MarkID = 0xC22538;
        //const Int32 MP25P128_MarkID = 0x012018;
        const Int32 MX25U51245G_MarkID = 0xC2253A;
        const Int32 W25Q64_MarkID = 0xEF4017;
        const UInt32 SIZE_64K_SECTOR = UpdaterBaseConstants.SIZE_64K_SECTOR;
        //const UInt32 MX25U12832F_FLASHSIZE = 16777216;
        const UInt32 MT25U51245G_FLASHSIZE = 67_108_864;
        const UInt32 W25Q64_FLASHSIZE = 0x80_0000;

        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE / 2 - SIZE_64K_SECTOR;
        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;
        //const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = SIZE_64K_SECTOR * 4;
        const UInt32 PCIE_FPGA_VARSION_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 4; //256K
        const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE - PCIE_FPGA_VARSION_INFO_TOTAL_BYTES;

        const UInt32 PRO_FPGA_VARSION_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 4; //256K
        const UInt32 PRO_FPGA_VARSION_INFO_STARTAT_BYTES = MT25U51245G_FLASHSIZE - PRO_FPGA_VARSION_INFO_TOTAL_BYTES;

        const UInt32 AWG_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 AWG_FPGA_CONTENT_TOTAL_BYTES = W25Q64_FLASHSIZE - AWG_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        //const UInt32 AWG_FPGA_APP_START_ADDR = 0x35_0000;
        const UInt32 AWG_FPGA_APP_START_ADDR = 0x40_0000; //23.10
        const UInt32 AWG_FPGA_VARSION_INFO_STARTAT_BYTES = 0x076_0000;
        const UInt32 AWG_FPGA_APP_TOTAL_BYTES = AWG_FPGA_VARSION_INFO_STARTAT_BYTES - AWG_FPGA_APP_START_ADDR;

        const UInt32 PROC_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 PROC_FPGA_CONTENT_TOTAL_BYTES = MT25U51245G_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        //const UInt32 PROC_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

        const UInt32 ACQBD_FPGA_CONTENT_START_ADDR = 0;
        const UInt32 ACQBD_FPGA_CONTENT_TOTAL_BYTES = MT25U51245G_FLASHSIZE - SIZE_64K_SECTOR - ACQBD_FPGA_CONTENT_START_ADDR;
        //const UInt32 ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES = 0x0260_0000;

        const UInt32 FLASH_DATA_STARTAT_BYTES = 0x0270_0000;
        const UInt32 FLASH_DATA_TOTAL_BYTES = MT25U51245G_FLASHSIZE - FLASH_DATA_STARTAT_BYTES;
        const UInt32 FLASH_MIN_INFO_SIZE_BYTE = UpdaterBaseConstants.FLASH_MIN_INFO_SIZE_BYTE;
        const UInt32 FLASH_IMAGE_APP_START_ADDR = 0x0120_0000;
        const UInt32 FLASH_IMAGE_APP_TOTAL_BYTES = UpdaterBaseConstants.FLASH_IMAGE_APP_MAX_SIZE_BYTE;
        const UInt32 FPGA_INFO_INDEX_TOTAL_BYTES = UpdaterBaseConstants.FLASH_TOTAL_INFO_SIZE_BYTE;

        const UInt32 OPTIONS_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 8; //512K
        const UInt32 OPTIONS_INFO_STARTAT_BYTES = PRO_FPGA_VARSION_INFO_STARTAT_BYTES - (SIZE_64K_SECTOR * 2) - OPTIONS_INFO_TOTAL_BYTES;


        const UInt32 PRODUCT_INFO_TOTAL_BYTES = SIZE_64K_SECTOR * 8; //512K

        const UInt32 PRODUCT_INFO_STARTAT_BYTES = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES - (SIZE_64K_SECTOR * 2) - PRODUCT_INFO_TOTAL_BYTES;

        const UInt32 CALI_DATA_TOTAL_BYTES = 0 * SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_STARTAT_BYTES = PRODUCT_INFO_STARTAT_BYTES - CALI_DATA_TOTAL_BYTES - (SIZE_64K_SECTOR * 2);

        const UInt32 CALI_DATA_USED_SECTOR_COUNT = 0 * SIZE_64K_SECTOR;

        const Int16 SPI_CLOCK = 6; //6;
        return new Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>()
        {
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)PcieBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenTotalBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataStartAtBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataTotalBytes = CALI_DATA_TOTAL_BYTES,
                CaliDataUsedSectorCount = CALI_DATA_USED_SECTOR_COUNT,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                ProductInfoStartAtBytes = PRODUCT_INFO_STARTAT_BYTES,
                ProductInfoTotalBytes = PRODUCT_INFO_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)PcieBdReg.R.FPGAFlashUpdater_UpdateFlag,

                #region FastMode
                FastMode_ActionReset          /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= 0, //(UInt32)PcieBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= 0, //(UInt32)PcieBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= 0, //(UInt32)PcieBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= 0, //(UInt32)PcieBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = PROC_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = PROC_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                OptionsInfoStartAtBytes = OPTIONS_INFO_STARTAT_BYTES,
                OptionsInfoTotalBytes = OPTIONS_INFO_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag,

                #region FastMode
                FastMode_ActionReset          /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_ActionReset,
                FastMode_ActionCode           /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_ActionCode,
                FastMode_ActionStart          /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_ActionStart,
                FastMode_ActionStatus         /**/= 0, //(UInt32)ProcBdReg.R.FlashOperator_ActionStatus,
                FastMode_FlashStartAddressL16 /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_StartAddress_L16,
                FastMode_FlashStartAddressH16 /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_StartAddress_H16,
                FastMode_FlashEndAddressL16   /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_EndAddress_L16,
                FastMode_FlashEndAddressH16   /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_EndAddress_H16,
                FastMode_WhichFlash           /**/= 0, //(UInt32)ProcBdReg.W.FlashOperator_WhichFlash,
                FastMode_FlashID_L16          /**/= 0, //(UInt32)ProcBdReg.R.FlashOperator_FlashID_L16,
                FastMode_FlashID_H16          /**/= 0, //(UInt32)ProcBdReg.R.FlashOperator_FlashID_H16,
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[0],

                #region FastMode
                FastMode_ActionReset          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionCode           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionStart          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_ActionStatus         /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashStartAddressL16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashStartAddressH16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashEndAddressL16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashEndAddressH16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_WhichFlash           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashID_L16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FastMode_FlashID_H16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                
				#region FastMode
                FastMode_ActionReset          /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionCode           /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionStart          /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_ActionStatus         /**/ = 0, //(UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashStartAddressL16 /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashStartAddressH16 /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashEndAddressL16   /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashEndAddressH16   /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_WhichFlash           /**/ = 0, //(UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashID_L16          /**/ = 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FastMode_FlashID_H16          /**/ = 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[2],

                #region FastMode
                FastMode_ActionReset          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionCode           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionStart          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_ActionStatus         /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashStartAddressL16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashStartAddressH16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashEndAddressL16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashEndAddressH16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_WhichFlash           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashID_L16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FastMode_FlashID_H16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[3],

                #region FastMode
                FastMode_ActionReset          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionCode           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionStart          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_ActionStatus         /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashStartAddressL16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashStartAddressH16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashEndAddressL16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashEndAddressH16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_WhichFlash           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashID_L16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FastMode_FlashID_H16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
				
                #region FastMode
                FastMode_ActionReset          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionCode           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionStart          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_ActionStatus         /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashStartAddressL16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashStartAddressH16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashEndAddressL16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashEndAddressH16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_WhichFlash           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashID_L16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FastMode_FlashID_H16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
				
                #region FastMode
                FastMode_ActionReset          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionCode           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionStart          /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_ActionStatus         /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashStartAddressL16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashStartAddressH16 /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashEndAddressL16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashEndAddressH16   /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_WhichFlash           /**/= 0, //(UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashID_L16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FastMode_FlashID_H16          /**/= 0, //(UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                #region FastMode
				
                FastMode_ActionReset          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionReset      | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionCode           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionCode       | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionStart          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionStart      | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_ActionStatus         /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashStartAddressL16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashStartAddressH16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashEndAddressL16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashEndAddressH16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_WhichFlash           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashID_L16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FastMode_FlashID_H16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8] = new Flash_MX25U51245G(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                FlashMarkID = MX25U51245G_MarkID,
                IDCodeVerify = "30_01_80_01_03_82_30_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_INFO_INDEX_TOTAL_BYTES,
                FlashImageAppStartAddr = FLASH_IMAGE_APP_START_ADDR,
                FlashImageAppTotalBytes = FLASH_IMAGE_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)AcqBdReg.R.FPGAFlashUpdater_UpdateFlag | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
				
                #region FastMode
                FastMode_ActionReset          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionReset      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionCode           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionCode       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionStart          /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_ActionStart      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_ActionStatus         /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_ActionStatus     /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashStartAddressL16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_L16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashStartAddressH16 /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_StartAddress_H16 /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashEndAddressL16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_L16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashEndAddressH16   /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_EndAddress_H16   /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_WhichFlash           /**/= 0, // (UInt32)AcqBdReg.W.FlashOperator_WhichFlash       /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashID_L16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_L16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FastMode_FlashID_H16          /**/= 0, // (UInt32)AcqBdReg.R.FlashOperator_FlashID_H16      /**/| AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                #endregion
            })
            #region FastMode
            {
                //BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                //SpiClockSetValue = 8,
                //FlashID = MX25U51245G_MarkID,
            },
            #endregion
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AWG] = new BoardAwg(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AWG,
                //FlashMarkID = MP25P128_MarkID,
                FlashMarkID = W25Q64_MarkID,
                IDCodeVerify = "20_00_00_00_20_00_00_00",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.Awg_data_awg_rd,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = AWG_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = AWG_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = W25Q64_FLASHSIZE - AWG_FPGA_VARSION_INFO_STARTAT_BYTES,
                FlashImageAppStartAddr = AWG_FPGA_APP_START_ADDR,
                FlashImageAppTotalBytes = AWG_FPGA_APP_TOTAL_BYTES,
                FlashDataStartAddr = FLASH_DATA_STARTAT_BYTES,
                FlashDataTotalBytes = FLASH_DATA_TOTAL_BYTES,
                FlashMinInfoSizeByte = FLASH_MIN_INFO_SIZE_BYTE,
                FlashUpDateGoldenFlag = (UInt32)ProcBdReg.R.FPGAFlashUpdater_UpdateFlag
            })
        };
    }
    /// <summary>
    /// 浅机箱 相关配置
    /// </summary>
    /// <returns></returns>
    static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateShallowModeProduct()
    {
        const Int32 MP25P128_MarkID = 0x012018;
        const Int32 MT25QU256_MarkID = 0x20BB19;

        const UInt32 SIZE_64K_SECTOR = 64 * 1024;
        const UInt32 MP25P128_FLASHSIZE = 16777216;
        const UInt32 MT25QU256_FLASHSIZE = 33554432;

        const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MP25P128_FLASHSIZE - SIZE_64K_SECTOR;

        const UInt32 PROC_FPGA_CONTENT_START_ADDR = 0x70_0000;
        const UInt32 PROC_FPGA_CONTENT_TOTAL_BYTES = MP25P128_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        const UInt32 PROC_FPGA_VARSION_INFO_STARTAT_BYTES = MP25P128_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - PROC_FPGA_CONTENT_TOTAL_BYTES;

        const UInt32 ACQBD_FPGA_CONTENT_START_ADDR = 0x80_0000;
        const UInt32 ACQBD_FPGA_CONTENT_TOTAL_BYTES = MT25QU256_FLASHSIZE - SIZE_64K_SECTOR - ACQBD_FPGA_CONTENT_START_ADDR;
        ;
        const UInt32 ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES = MT25QU256_FLASHSIZE - ACQBD_FPGA_CONTENT_START_ADDR - ACQBD_FPGA_CONTENT_TOTAL_BYTES;

        const UInt32 FPGA_SIGN_INFO_SIZE_BYTES = SIZE_64K_SECTOR;

        const UInt32 PRODUCT_INFO_STARTAT_BYTES = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES - SIZE_64K_SECTOR;

        const UInt32 PRODUCT_INFO_TOTAL_BYTES = SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_TOTAL_BYTES = 64 * SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_STARTAT_BYTES = PRODUCT_INFO_STARTAT_BYTES - CALI_DATA_TOTAL_BYTES - (SIZE_64K_SECTOR * 2);

        const UInt32 CALI_DATA_USED_SECTOR_COUNT = 64 * SIZE_64K_SECTOR;

        const Int16 SPI_CLOCK = 6;
        return new Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>()
        {
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE] = new Flash_MX25U12832F(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                FlashMarkID = MP25P128_MarkID,
                IDCodeVerify = "30_01_80_01_03_65_10_93",
                SpiClock = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)PcieBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenTotalBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataStartAtBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataTotalBytes = CALI_DATA_TOTAL_BYTES,
                CaliDataUsedSectorCount = CALI_DATA_USED_SECTOR_COUNT,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES,
                ProductInfoStartAtBytes = PRODUCT_INFO_STARTAT_BYTES,
                ProductInfoTotalBytes = PRODUCT_INFO_TOTAL_BYTES
            }),

            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7] = new Flash_MX25U12832F(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                FlashMarkID = MP25P128_MarkID,
                IDCodeVerify = "30_01_80_01_03_65_10_93",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = PROC_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = PROC_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PROC_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_SIGN_INFO_SIZE_BYTES
            })
        };
    }
    /// <summary>
    /// 深机箱 相关配置
    /// </summary>
    /// <returns></returns>
    static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash> CreateDeepModeProduct()
    {
        const Int32 MP25P128_MarkID = 0x012018;
        const Int32 MT25QU256_MarkID = 0x20BB19;

        const UInt32 SIZE_64K_SECTOR = 64 * 1024;
        const UInt32 MP25P128_FLASHSIZE = 0x100_0000; // 16777216;
        const UInt32 MT25QU256_FLASHSIZE = 0x200_0000; // 33554432;

        const UInt32 PCIE_FPGA_VARSION_INFO_STARTAT_BYTES = MP25P128_FLASHSIZE - SIZE_64K_SECTOR;

        const UInt32 S6_FPGA_CONTENT_START_ADDR = 0x6F_FFFF;
        const UInt32 S6_FPGA_CONTENT_TOTAL_BYTES = MP25P128_FLASHSIZE - S6_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        const UInt32 S6_FPGA_VARSION_INFO_STARTAT_BYTES = MP25P128_FLASHSIZE - S6_FPGA_CONTENT_START_ADDR - S6_FPGA_CONTENT_TOTAL_BYTES;

        const UInt32 PROC_FPGA_CONTENT_START_ADDR = 0x70_0000;
        const UInt32 PROC_FPGA_CONTENT_TOTAL_BYTES = MT25QU256_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        const UInt32 PROC_FPGA_VARSION_INFO_STARTAT_BYTES = MT25QU256_FLASHSIZE - PROC_FPGA_CONTENT_START_ADDR - PROC_FPGA_CONTENT_TOTAL_BYTES;

        const UInt32 ACQBD_FPGA_CONTENT_START_ADDR = 0x80_0000;
        const UInt32 ACQBD_FPGA_CONTENT_TOTAL_BYTES = MT25QU256_FLASHSIZE - ACQBD_FPGA_CONTENT_START_ADDR - SIZE_64K_SECTOR;
        const UInt32 ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES = MT25QU256_FLASHSIZE - ACQBD_FPGA_CONTENT_START_ADDR - ACQBD_FPGA_CONTENT_TOTAL_BYTES;

        const UInt32 FPGA_VARSION_INFO_TOTAL_BYTES = SIZE_64K_SECTOR;

        const UInt32 PRODUCT_INFO_STARTAT_BYTES = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES - SIZE_64K_SECTOR;

        const UInt32 PRODUCT_INFO_TOTAL_BYTES = SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_TOTAL_BYTES = 64 * SIZE_64K_SECTOR;

        const UInt32 CALI_DATA_STARTAT_BYTES = PRODUCT_INFO_STARTAT_BYTES - CALI_DATA_TOTAL_BYTES - (SIZE_64K_SECTOR * 2);

        const UInt32 CALI_DATA_USED_SECTOR_COUNT = 64 * SIZE_64K_SECTOR;

        const Int16 SPI_CLOCK = 6;

        return new Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>()
        {
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE] = new Flash_MX25U12832F(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.PCIE,
                FlashMarkID = MP25P128_MarkID,
                IDCodeVerify = "30_01_80_01_03_65_10_93",
                SpiClock = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)PcieBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)PcieBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)PcieBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenTotalBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataStartAtBytes = CALI_DATA_STARTAT_BYTES,
                CaliDataTotalBytes = CALI_DATA_TOTAL_BYTES,
                CaliDataUsedSectorCount = CALI_DATA_USED_SECTOR_COUNT,
                FpgaInfoZoneStartAddr = PCIE_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES,
                ProductInfoStartAtBytes = PRODUCT_INFO_STARTAT_BYTES,
                ProductInfoTotalBytes = PRODUCT_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_S6] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_S6,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "31_C2_04_00_80_93",
                //SpiClock = (UInt32)S6BdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                //SS = (UInt32)S6BdReg.W.FPGAFlashUpdater_SS,
                //WriteData = (UInt32)S6BdReg.W.FPGAFlashUpdater_WriteData,
                //ReadData = (UInt32)S6BdReg.R.FPGAFlashUpdater_ReadData,
                //ReadStart = (UInt32)S6BdReg.W.FPGAFlashUpdater_ReadStart,
                //WriteStart = (UInt32)S6BdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = S6_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = S6_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = S6_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.ProcessBoard_K7,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_69_10_93",
                SpiClock = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SpiClock,
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)ProcBdReg.W.FPGAFlashUpdater_SS,
                WriteData = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteData,
                ReadData = (UInt32)ProcBdReg.R.FPGAFlashUpdater_ReadData,
                ReadStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_ReadStart,
                WriteStart = (UInt32)ProcBdReg.W.FPGAFlashUpdater_WriteStart,
                FlashImageGoldenStartAddr = PROC_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = PROC_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = PROC_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_1,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[0],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_2,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[1],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_3,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[2],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_4,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[3],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_5,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[4],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_6,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[5],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_7,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[6],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            }),
            [FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8] = new Flash_MT25QU256(new FpgaFlash_Registers
            {
                BoardID = (UInt32)FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine.AcquireBoard_K7_8,
                FlashMarkID = MT25QU256_MarkID,
                IDCodeVerify = "30_01_80_01_03_90_D0_93",
                SpiClock = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SpiClock | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                SpiClockSetValue = SPI_CLOCK,
                SS = (UInt32)AcqBdReg.W.FPGAFlashUpdater_SS | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteData = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadData = (UInt32)AcqBdReg.R.FPGAFlashUpdater_ReadData | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                ReadStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_ReadStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                WriteStart = (UInt32)AcqBdReg.W.FPGAFlashUpdater_WriteStart | AbstractAcqBd.AcqFpgaAddrMarkTable[7],
                FlashImageGoldenStartAddr = ACQBD_FPGA_CONTENT_START_ADDR,
                FlashImageGoldenTotalBytes = ACQBD_FPGA_CONTENT_TOTAL_BYTES,
                FpgaInfoZoneStartAddr = ACQBD_FPGA_VARSION_INFO_STARTAT_BYTES,
                FpgaInfoZoneTotalBytes = FPGA_VARSION_INFO_TOTAL_BYTES
            })
        };
    }

    static Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>? CreateProductFPGAConstituteDefine(ProductType productType)
    {
        switch (productType)
        {
            case ProductType.B21_MS2G:
            case ProductType.B24_AI20G:
            case ProductType.B21_MD8G:
            case ProductType.B21_HB8G:
            case ProductType.B21_DBI20G:
            case ProductType.B21_DBI16G:
                return CreateDeepModeProduct();
            case ProductType.B21_HD4G:
            case ProductType.B21_HR1G:
            case ProductType.Base:
            case ProductType.ForTest:
                return CreateShallowModeProduct();
            case ProductType.JiHe_MSO7000X:
            case ProductType.JiHe_MSO7000A:
                return CreateMSO7000XProduct();
            case ProductType.JiHe_MSO8000HD:
            case ProductType.JiHe_MSO8000X:
                return CreateMSO8000HDProduct();
            case ProductType.JiHe_MSO7000HD:
                return CreateMSO7000HDProduct();
            case ProductType.JiHe_UPO7000L:
                return CreateUPO7000LProduct();
            default:
                return null;
        }
    }
    public static IFpgaFlashUpdater? FindUpdater(ProductType productType, List<Int32> boardIndexList)
    {
        Dictionary<FpgaFlashUpdaterStandard.SystemFPGAConstituteDefine, FpgaFlash>? fpgaFlashDefine = CreateProductFPGAConstituteDefine(productType);
        if (fpgaFlashDefine == null)
            return null;

        return productType switch
        {
            _ => new FpgaFlashUpdaterStandard(productType, boardIndexList, fpgaFlashDefine)
        };
    }
}
