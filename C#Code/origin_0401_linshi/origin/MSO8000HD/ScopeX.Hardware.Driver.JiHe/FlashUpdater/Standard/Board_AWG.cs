using System;
using System.Diagnostics;
using System.Threading;

namespace ScopeX.Hardware.Driver;

internal class BoardAwg : RootFpgaFlash
{
    Object _eraseAllLocker = new();
    public UInt32 TransferPerByteUsedNs { get; init; }

    public BoardAwg(FpgaFlash_Registers fpgaFlashRegisters)
    {
        Registers = fpgaFlashRegisters;

        TransferPerByteUsedNs = (UInt32)((10 * 1000L) / (BaseclokeMhz / ((SpiClockSetValue + 1) * 2))) + 320;
    }

    #region 寄存器/常量

    const UInt32 Size64K = 64 * 1024;
    const Int32 MaxWaittime = 30;
    const UInt16 ReadBackVer1 = 0xF2;
    const UInt16 ReadBackVer2 = 0xF3;

    const UInt16 RegDivNum = 0xF5;
    const UInt16 RegChipSelect = 0xF6;
    const UInt16 RegWriteStart = 0xF7;
    const UInt16 RegWriteData = 0xF8;

    const UInt16 RegReadStart = 0xF9;
    const UInt16 RegReadData = 0xFA;
    const UInt16 RegFlashBusy = 0xFB;

    const UInt16 BaseclokeMhz = 250;
    const UInt16 SpiClockSetValue = 12;
    const Int32 PageBytes = 256;
    //写入占进度权重
    const Int32 WriteWeight = 50;
    const Int32 MaxProgress = 100;

    #endregion 寄存器/常量

    #region FlashCMD

    const Byte W25QCmdWren = 0x06; //Write Enable
    const Byte W25QCmdWrdi = 0x04; //Write disable
    const Byte W25QCmdRdid = 0x9f; //Read Identification
    const Byte W25QCmdRdsr = 0x05; //Read Status Register
    //const Byte W25Q_CMD_RD_FLAG_SR = 0x70;  //Read Flag Register
    //const Byte W25Q_CMD_RD_NONV_SR = 0xB5;  //Read nonvo Register
    //const Byte W25Q_CMD_WR_NONV_SR = 0xB1;  //Write nonvo Register
    const Byte W25QCmdWrsr = 0x01; //Write Status Register
    const Byte W25QCmdRead = 0x03; //Read Data Bytes 

    //const Byte W25Q_CMD_READ = 0x13;  //Read Data Bytes 
    //const Byte W25Q_CMD_PP = 0x12;  //Page Program
    const Byte W25QCmdPp = 0x02; //Page Program
    const Byte W25QCmdBe = 0xc7; //Block Erase
    const Byte W25QCmdId = 0x90; //Read ID
    const Byte W25QCmdB4KE = 0x20; //Block 4K Erase
    const Byte W25QCmdSectorE = 0xd8; //SECTOR ERASE Erase

    #endregion FlashCMD

    #region SPI Base

    Object _readerLocker = new();
    Object _writerLocker = new();
    Object _queryStatusLocker = new();

    Byte ReadDataWithSpi()
    {
        lock (_readerLocker)
        {
            WriteAwgRegBySpi(RegReadStart, 0x00);
            WriteAwgRegBySpi(RegReadStart, 0x01);
            WriteAwgRegBySpi(RegReadData, 1);
            HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x06);

            Byte result = (Byte)(HdIO.ReadReg(Registers.ReadData) & 0xff);
            WriteAwgRegBySpi(RegReadStart, 0x00);
            return result;
        }
    }
    Byte ReadAwgRegBySpi(UInt16 regAddr, Int32 delayus = 0)
    {
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x04);
        // 低8位地址
        UInt16 addrCtrl = (UInt16)(regAddr & 0xffff);
        // cs为0时，发送一次，赋初值
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
        // cs置1，再发送一次
        addrCtrl = (UInt16)(regAddr | (0x0001 << 8));
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
        Thread.Sleep(1);
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x06);
        Byte data = (Byte)HdIO.ReadReg(Registers.ReadData);
        //HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
        return data;
    }

    void WriteAwgRegBySpi(UInt16 regAddr, UInt16 data, Int32 delayus = 0)
    {
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x04);
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_data_in, data, delayus);
        // 低8位地址
        UInt16 addrCtrl = (UInt16)(regAddr & 0xffff);
        //// cs为0时，发送一次，赋初值
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
        //// cs置1，再发送一次
        addrCtrl = (UInt16)(regAddr | (0x0001 << 8));
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
        //HdIO.Sleep(1);
        HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
    }

    public void WriteDataWithSpi(UInt32 data, Boolean switchSs = false)
    {
        WriteDataWithSpi((UInt16)data, switchSs);
    }
    public void WriteDataWithSpi(UInt16 data, Boolean switchSs = false)
    {
        lock (_writerLocker)
        {
            if (switchSs)
                WriteAwgRegBySpi(RegChipSelect, 1);

            WriteAwgRegBySpi(RegWriteData, data);
            if (switchSs)
                WriteAwgRegBySpi(RegChipSelect, 0);
            WriteAwgRegBySpi(RegWriteStart, 1);
            WriteAwgRegBySpi(RegWriteStart, 0);
            if (switchSs)
                WriteAwgRegBySpi(RegChipSelect, 1);
        }
    }

    #endregion SPI Base

    #region 业务逻辑

    void SetSpiClock()
    {
        WriteAwgRegBySpi(RegChipSelect, 0x00);
        WriteAwgRegBySpi(RegChipSelect, 0x01);
        WriteAwgRegBySpi(RegDivNum, SpiClockSetValue);
        WriteAwgRegBySpi(RegChipSelect, 0x00);
    }
    public override FpgaFlashErrorCode VerifyFlashID(ref UInt32 flashId)
    {
        SetSpiClock();

        UInt32 readBackId = ReadID() & 0xffffff;
        if (readBackId != (Registers.FlashMarkID & 0xffffff))
        {
            readBackId = ReadID() & 0xffffff; //目前是问题,需要读二次才正确
        }
        flashId = readBackId;
        AddLogInfo($"Read FlashID:0x{flashId:X}");

        if (readBackId == (Registers.FlashMarkID & 0xffffff))
            return FpgaFlashErrorCode.Succeed;
        else
            return FpgaFlashErrorCode.FLashIDMismatching;
    }
    public override Boolean ReadFpgaInsideVersionInfo(out String? infos)
    {
        infos = $"{ReadAwgRegBySpi(ReadBackVer1)}.{ReadAwgRegBySpi(ReadBackVer2)}";
        //return infos != "0.0";
        return true;
    }
    override protected UInt32 ReadID()
    {
        lock (_readerLocker)
        {
            Int32 count = 3;
            while (count > 0)
            {
                WriteAwgRegBySpi(RegChipSelect, 0x01);
                WriteAwgRegBySpi(RegChipSelect, 0x00);

                WriteDataWithSpi(W25QCmdRdid, false);

                UInt32 result = 0;
                result = ReadDataWithSpi();
                result <<= 8;

                result |= ReadDataWithSpi();
                result <<= 8;

                result |= ReadDataWithSpi();

                WriteAwgRegBySpi(RegChipSelect, 0x01);
                ////test
                //if (result != 0x00ef4017)
                //{
                //    return 0;
                //}
                if (result != 0)
                {
                    return result;
                }
                count--;
            }
            return 0;
        }
    }
    override protected void AddLogInfo(String msg)
    {
        Debug.WriteLine($"FLASH Board-AWG :{msg}");
        if (AddLog != null)
        {
            AddLog($"FLASH BoardID-AWG :{msg}");
        }
    }
    void ResetInitStatus()
    {
        WriteAwgRegBySpi(RegChipSelect, 0x01);
        WriteAwgRegBySpi(RegReadStart, 0x00);
        WriteAwgRegBySpi(RegWriteStart, 0x00);
    }
    public override UInt32 QueryStatus()
    {
        lock (_queryStatusLocker)
        {
            UInt32 status = 0;
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdRdsr, false);

            WriteAwgRegBySpi(RegWriteStart, 0x01);
            WriteAwgRegBySpi(RegWriteStart, 0x00);

            status = ReadDataWithSpi();
            status <<= 8;
            status = ReadDataWithSpi();
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            return status;
        }
    }

    override protected FpgaFlashErrorCode WriteBlock(UInt32 flashAddress, Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null)
    {
        TaskProgress taskProgress = new();
        UInt32 status = 0;
        UInt32 pageCount = ((bytes + PageBytes) - 1) / PageBytes;
        UInt32 pageAddress = flashAddress;
        UInt32 writedCount = 0;
        UInt32 pageIndex;
        Int32 byteIndex = (Int32)offset;

        Stopwatch stopwatch = new();

        ResetInitStatus();

        AddLogInfo($"Call WriteBlock:Flash_Address 0x{flashAddress:X0},offset:{offset},bytes:{bytes}");

        taskProgress.Total = pageCount;
        //进度5%
        UInt32 persentsFive = pageCount / 20;
        for (pageIndex = 0; pageIndex < pageCount; pageIndex++)
        {
            //ljw 22.4.2
            //if (pageAddress % 16777216 == 0)
            //{
            //    ResetInitStatus();
            //}
            if ((pageAddress % 1500) == 0)
            {
                AddLogInfo($"烧写进度:{pageIndex}/{pageCount}");
            }
            if (updateProcessTip != null && (pageIndex == (pageCount - 1) || (pageIndex % persentsFive) == 0))
            {
                taskProgress.Now = pageIndex / 2;
                updateProcessTip(taskProgress);
            }
            pageAddress = flashAddress + (pageIndex * PageBytes);


            lock (_writerLocker)
            {

                //WriteDataWithSpi(W25Q_CMD_EN_4BYTE_ADDR, true);
                WriteDataWithSpi(W25QCmdWren, true);

                WriteAwgRegBySpi(RegChipSelect, 0);

                WriteDataWithSpi(W25QCmdPp, false);

                WriteDataWithSpi((UInt16)((pageAddress & 0x00FF0000) >> 16), false);

                WriteDataWithSpi((UInt16)((pageAddress & 0x0000FF00) >> 8), false);

                WriteDataWithSpi((UInt16)(pageAddress & 0x000000FF), false);

                #region 256 bytes Data Per Page

                for (Int32 len = 0; len < PageBytes; len++)
                {
                    if (writedCount < bytes)
                    {
                        WriteDataWithSpi(content[byteIndex++], false);
                    }
                    else
                    {
                        //test
                        WriteDataWithSpi(0xff, false); //写满一页
                    }
                    writedCount++;
                }

                #endregion

            }
            #region 检查页写完成

            ResetInitStatus();
            stopwatch.Restart();
            do
            {
                status = QueryStatus();
                if ((status & 0x3) != 0x00)
                {
                    Thread.Sleep(1);
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        break;
                    }
                }
            }
            while ((status & 0x3) != 0x00);
            if ((status & 0x3) != 0x00)
            {
                ResetInitStatus();
                return FpgaFlashErrorCode.WriteOvertime;
            }

            #endregion
        }

        #region Verify
        //awg 回读可能有问题，不做检查 ljw 24.7
        // Byte[] readBackData = new Byte[bytes];
        // FpgaFlashErrorCode error = ReadBlock(flashAddress, ref readBackData, 0, bytes, new Action<TaskProgress>(progress =>
        // {
        //     if (updateProcessTip != null)
        //     {
        //         taskProgress.Now = (pageCount * (WriteWeight + (((MaxProgress - WriteWeight) * progress.Now) / progress.Total))) / MaxProgress;
        //         updateProcessTip(taskProgress);
        //     }
        // }));
        // Boolean bErrorFound = false;
        // if (error == FpgaFlashErrorCode.Succeed)
        // {
        //     for (Int32 i = 0; i < bytes; i++)
        //     {
        //         if (readBackData[i] != content[i])
        //         {
        //             AddLogInfo($"bErrorFound: i:{i} data:0x{readBackData[i]:X2}/0x{content[i]:X2}");
        //             bErrorFound = true;
        //             break;
        //         }
        //     }
        // }
        // else
        //     bErrorFound = true;

        #endregion Verify

        ResetInitStatus();
        //return !bErrorFound ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.WriteVerifyDefeated;
        return FpgaFlashErrorCode.Succeed;
    }
    override internal FpgaFlashErrorCode ReadBlock(UInt32 flashAddress, ref Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null)
    {
        AddLogInfo($"ReadBlock:0x{flashAddress:X}");
        TaskProgress taskProgress = new();

        #region test 22.11.15

        AddLogInfo($"State: 0x{QueryStatus():X2}");

        #endregion test 22.11.15

        WriteAwgRegBySpi(RegChipSelect, 0);

        WriteDataWithSpi(W25QCmdRead, false);
        // WriteDataWithSpi(0, false);
        WriteDataWithSpi((UInt16)((flashAddress & 0xFF0000) >> 16), false);
        WriteDataWithSpi((UInt16)((flashAddress & 0x00FF00) >> 8), false);
        WriteDataWithSpi((UInt16)(flashAddress & 0x0000ff), false);
        taskProgress.Total = bytes;
        //进度5%
        UInt32 persentsFive = bytes / 20;

        for (UInt32 i = 0; i < bytes; i++)
        {
            content[i] = (Byte)(ReadDataWithSpi() & 0xff);
            if (bytes > 350000 && (i % 350000) == 0)
            {
                AddLogInfo($"Read Data:{i}/{bytes}");
            }
            if (updateProcessTip != null && (i == (bytes - 1) || (i % persentsFive) == 0))
            {
                taskProgress.Now = i;
                updateProcessTip(taskProgress);
            }
        }

        AddLogInfo($"Read Data:{bytes}/{bytes}");
        WriteAwgRegBySpi(RegChipSelect, 1);

        if (updateProcessTip != null)
        {
            taskProgress.Now = bytes;
            updateProcessTip(taskProgress);
        }
        return FpgaFlashErrorCode.Succeed;
    }

    override protected FpgaFlashErrorCode EraseOneSector(UInt32 flashAddress, Int32 maxWaitSecond)
    {
        Debug.WriteLine($"擦除：0x{flashAddress:X}");
        ResetInitStatus();

        lock (_eraseAllLocker)
        {
            WriteDataWithSpi(W25QCmdWren, true);
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdSectorE, false);
            WriteDataWithSpi((flashAddress & 0xFF0000) >> 16, false);
            WriteDataWithSpi((flashAddress & 0x00FF00) >> 8, false);
            WriteDataWithSpi(flashAddress & 0x0000FF, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
        }
        #region check erase finished

        Stopwatch stopwatch = new();
        stopwatch.Start();
        UInt32 status = QueryStatus();
        while ((status & 0x03) != 0)
        {
            if (stopwatch.ElapsedMilliseconds > (maxWaitSecond * 1000))
            {
                break;
            }
            Thread.Sleep(10);
            status = QueryStatus();
        }
        stopwatch.Stop();

        #endregion

        ResetInitStatus();

        return (status & 0x03) == 0 ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.EraseOvertime;
    }
    override protected FpgaFlashErrorCode EraseMultiSectors(UInt32 startAddress, UInt32 eraseLength, Int32 maxWaitSecond, Action<TaskProgress>? updateProcessTip)
    {
        TaskProgress taskProgress = new(Registers.BoardID);
        if (eraseLength == 0 || (eraseLength % Size64K) != 0)
        {
            return FpgaFlashErrorCode.ErrorEraseLength;
        }
        ResetInitStatus();

        #region 22.5.11 ljw

        //var earAddrReg = GetEarAddrReg();

        #endregion 22.5.11 ljw
        Stopwatch stopwatch = new();
        UInt32 sectorsCount = eraseLength / Size64K;

        taskProgress.Total = sectorsCount;
        //进度5%
        UInt32 persentsFive = sectorsCount / 20;
        stopwatch.Start();
        for (UInt32 i = 0; i < sectorsCount; i++)
        {
            //test
            //AddLogInfo($"Test I:{i}/{sectorsCount}");
            UInt32 address = (UInt32)(startAddress + (i * Size64K));
            FpgaFlashErrorCode result = EraseOneSector(address, MaxWaittime);
            if (updateProcessTip != null && (i == (sectorsCount - 1) || (i % persentsFive) == 0))
            {
                taskProgress.Now = i;
                updateProcessTip(taskProgress);
            }
            if (result != FpgaFlashErrorCode.Succeed)
            {
                return result;
            }
        }
        if (updateProcessTip != null)
        {
            taskProgress.Now = sectorsCount;
            updateProcessTip(taskProgress);
        }

        return FpgaFlashErrorCode.Succeed;
    }

    public Boolean SetFlashToLock()
    {
        const Int32 testTimes = 20;
        Int32 count = 0;
        UInt32 status = QueryStatus();
        UInt32 wel = 0;
        if (status == 0)
        {
            return true;
        }
        do
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWren, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            status = QueryStatus();
            wel = (status & 0b10) >> 1;
            count++;
            Thread.Sleep(1);
        }
        while (wel != 1 && count < testTimes); //WEL == 1
        if (count == testTimes)
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWrdi, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            return false;
        }
        count = 0;
        #region 写寄存器

        WriteAwgRegBySpi(RegChipSelect, 0x00);
        WriteDataWithSpi(W25QCmdWrsr, false);
        WriteDataWithSpi((UInt32)FLASH_PROTECTED_MARK, false);
        //WriteDataWithSpi(0, false);
        WriteAwgRegBySpi(RegChipSelect, 0x01);

        #endregion 写寄存器
        do
        {
            status = QueryStatus();
            wel = (status & 0b10) >> 1;
            count++;
            Thread.Sleep(1);
        }
        while (wel != 0 && count < testTimes); //WEL == 0
        if (count == testTimes)
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWrdi, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            return false;
        }
        status = QueryStatus();

        //Thread.Sleep(1);
        //WriteAwgRegBySpi(REG_CHIP_SELECT, 0x00);
        //HdIO.WriteReg(M25P_CMD_WRSR, 0x0);
        //WriteAwgRegBySpi(REG_CHIP_SELECT, 0x01);
        Thread.Sleep(1);
        ResetInitStatus();

        return (status & FLASH_PROTECTED_MARK) != 0;
    }
    public override Boolean ResetFlashStatusReg()
    {
        const Int32 testTimes = 20;
        Int32 count = 0;
        UInt32 status = QueryStatus();
        UInt32 wel = 0;
        if (status == 0)
        {
            return true;
        }
        do
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWren, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            status = QueryStatus();
            wel = (status & 0b10) >> 1;
            count++;
            Thread.Sleep(1);
        }
        while (wel != 1 && count < testTimes); //WEL == 1
        if (count == testTimes)
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWrdi, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            return false;
        }
        count = 0;
        #region 写寄存器

        WriteAwgRegBySpi(RegChipSelect, 0x00);
        WriteDataWithSpi(W25QCmdWrsr, false);
        WriteDataWithSpi(0, false);
        WriteDataWithSpi(0, false);
        WriteAwgRegBySpi(RegChipSelect, 0x01);

        #endregion 写寄存器
        do
        {
            status = QueryStatus();
            wel = (status & 0b10) >> 1;
            count++;
            Thread.Sleep(1);
        }
        while (wel != 0 && count < testTimes); //WEL == 0
        if (count == testTimes)
        {
            WriteAwgRegBySpi(RegChipSelect, 0x00);
            WriteDataWithSpi(W25QCmdWrdi, false);
            WriteAwgRegBySpi(RegChipSelect, 0x01);
            return false;
        }
        status = QueryStatus();

        //Thread.Sleep(1);
        //WriteAwgRegBySpi(REG_CHIP_SELECT, 0x00);
        //HdIO.WriteReg(M25P_CMD_WRSR, 0x0);
        //WriteAwgRegBySpi(REG_CHIP_SELECT, 0x01);
        Thread.Sleep(1);
        ResetInitStatus();

        return status == 0;
        //return true;
    }

    #endregion

}
