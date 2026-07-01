using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using PCIeControl;
using System.Net.WebSockets;

namespace ScopeX.Hardware.Driver
{
    class Flash_MX25U12832F : RootFpgaFlash
    {

        #region FLASH_CMD Define
        const Byte M25P_CMD_WREN = 0x06;  //Write Enable
        const Byte M25P_CMD_WRDI = 0x04;  //Write disable
        const Byte M25P_CMD_RDID = 0x9f;  //Read Identification
        const Byte M25P_CMD_RDSR = 0x05;  //Read Status Register
        const Byte M25P_CMD_WRSR = 0x01;  //Write Status Register
        const Byte M25P_CMD_READ = 0x03;  //Read Data Bytes
        const Byte M25P_CMD_PP = 0x02;  //Page Program
        const Byte M25P_CMD_BE = 0xc7;  //Block Erase
        const Byte M25P_CMD_B4K_E = 0x20;  //Block 4K Erase
        const Byte M25P_CMD_SECTOR_E = 0xd8;  //SECTOR ERASE Erase
        const Byte M25P_CMD_B32K_E = 0x52;  //Block 32K Erase
        const Byte M25P_CMD_ID = 0x9f;   //Read ID
        #endregion FLASH_CMD_Define

        private const Int32 MAX_WAITTIME = 60;
        private const UInt32 SIZE_64K = 64 * 1024;
        private const Int64 REFBASECLOKE_MHZ = 125;//125MHz
        //写入占进度权重
        private const Int32 WRITE_WEIGHT = 50;
        private const Int32 MAX_PROGRESS = 100;

        public Flash_MX25U12832F(FpgaFlash_Registers FpgaFlash_Registers)
        {
            Registers = FpgaFlash_Registers;

            TransferPerByteUsedNs = (UInt32)(10 * 1000L / (REFBASECLOKE_MHZ / ((Registers.SpiClockSetValue + 1) * 2))) + 320;
        }
        private UInt32 TransferPerByteUsedNs { get; init; }
        private Object writerLocker = new Object();
        private void WriteDataWithSpi(UInt32 data, Boolean switchSS = false)
        {
            lock (writerLocker)
            {
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 1);

                HdIO.WriteReg(Registers.WriteData, data);
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 0);
                HdIO.WriteReg(Registers.WriteStart, 1);
                HdIO.WriteReg(Registers.WriteStart, 0);
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 1);
            }
        }
        private Object readerLocker = new Object();
        private UInt32 ReadDataWithSpi()
        {
            lock (readerLocker)
            {
                HdIO.WriteReg(Registers.ReadStart, 1);
                UInt32 result = 0xFF;
                result = HdIO.ReadReg(Registers.ReadData);
                HdIO.WriteReg(Registers.ReadStart, 0);
                return result;
            }
        }

        private Object readIDLocker = new Object();
        private Object queryStatusLocker = new Object();
        private void ResetInitStatus()
        {
            HdIO.WriteReg(Registers.SS, 0x01);
            HdIO.WriteReg(Registers.ReadStart, 0x00);
            HdIO.WriteReg(Registers.WriteStart, 0x00);
            Thread.Sleep(1);
        }
        public override Boolean UnLockFlashProtect()
        {
            var state = QueryStatus() & FLASH_PROTECTED_MARK;
            if (state == 0)
            {
                return true;
            }
            if (!ResetFlashStatusReg()) return false;
            state = QueryStatus() & FLASH_PROTECTED_MARK;
            return state == 0;
        }
        public override UInt32 QueryStatus()
        {
            lock (queryStatusLocker)
            {
                UInt32 status = 0;
                HdIO.WriteReg(Registers.SS, 0x00);

                WriteDataWithSpi(M25P_CMD_RDSR, false);
                status = ReadDataWithSpi();
                HdIO.WriteReg(Registers.SS, 0x01);
                return status;
            }
        }
        public override Boolean ResetFlashStatusReg()
        {
            const Int32 testTimes = 20;
            Int32 count = 0;
            UInt32 status = 0;
            UInt32 WEL = 0;
            do
            {
                HdIO.WriteReg(Registers.SS, 0x00);
                WriteDataWithSpi(M25P_CMD_WREN, false);
                HdIO.WriteReg(Registers.SS, 0x01);
                status = QueryStatus();
                WEL = (status & 0b10) >> 1;
                count++;
                Thread.Sleep(1);
            } while (WEL != 1 && count < testTimes); //WEL == 1
            if (count == testTimes)
            {
                HdIO.WriteReg(Registers.SS, 0x00);
                WriteDataWithSpi(M25P_CMD_WRDI, false);
                HdIO.WriteReg(Registers.SS, 0x01);
                return false;
            }
            count = 0;
            #region 写寄存器
            HdIO.WriteReg(Registers.SS, 0x00);
            WriteDataWithSpi(M25P_CMD_WRSR, false);
            WriteDataWithSpi(0, false);
            WriteDataWithSpi(0, false);
            HdIO.WriteReg(Registers.SS, 0x01);
            #endregion 写寄存器
            do
            {
                status = QueryStatus();
                WEL = (status & 0b10) >> 1;
                count++;
                Thread.Sleep(1);
            } while (WEL != 0 && count < testTimes); //WEL == 0
            if (count == testTimes)
            {
                HdIO.WriteReg(Registers.SS, 0x00);
                WriteDataWithSpi(M25P_CMD_WRDI, false);
                HdIO.WriteReg(Registers.SS, 0x01);
                return false;
            }
            status = QueryStatus();

            //Thread.Sleep(1);
            //HdIO.WriteReg(Registers.SS, 0x00);
            //HdIO.WriteReg(M25P_CMD_WRSR, 0x0);
            //HdIO.WriteReg(Registers.SS, 0x01);
            Thread.Sleep(1);
            ResetInitStatus();

            return status == 0;
        }
        private Object eraseAllLocker = new Object();
        private FpgaFlashErrorCode EraseAll(UInt32 MaxWaitSecond)
        {
            ResetInitStatus();

            lock (eraseAllLocker)
            {
                WriteDataWithSpi(M25P_CMD_WREN, true);
                WriteDataWithSpi(M25P_CMD_BE, true);
            }
            #region check erase finished
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            UInt32 status = QueryStatus();
            while (status != 0)
            {
                if (stopwatch.ElapsedMilliseconds > MaxWaitSecond * 1000)
                {
                    break;
                }
                Thread.Sleep(10);
                status = QueryStatus();
            }
            stopwatch.Stop();
            #endregion
            WriteDataWithSpi(M25P_CMD_WRDI, true);
            ResetInitStatus();
            return (status == 0) ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.EraseOvertime;
        }
        const Int32 PAGE_BYTES = 256;
        private Object locker = new Object();

        protected override UInt32 ReadID()
        {
            lock (readIDLocker)
            {
                HdIO.WriteReg(Registers.SS, 0x00);
                HdIO.WriteReg(Registers.SS, 0x01);
                Thread.Sleep(1);
                HdIO.WriteReg(Registers.ReadStart, 0x00);
                Thread.Sleep(1);
                HdIO.WriteReg(Registers.ReadStart, 0x01);
                Thread.Sleep(1);
                HdIO.WriteReg(Registers.ReadStart, 0x00);
                Thread.Sleep(1);
                HdIO.WriteReg(Registers.WriteStart, 0x00);
                Thread.Sleep(1);
                //HdIO.WriteReg (currFPGA_Register.SS, 0x00);

                HdIO.WriteReg(Registers.SS, 0x00);
                Thread.Sleep(1);
                WriteDataWithSpi(M25P_CMD_ID, false);
                Thread.Sleep(1);
                UInt32 result = 0;
                result = ReadDataWithSpi();
                result <<= 8;
                result |= ReadDataWithSpi();
                result <<= 8;
                result |= ReadDataWithSpi();
                HdIO.WriteReg(Registers.SS, 0x01);

                return result;
            }
        }
        protected override FpgaFlashErrorCode WriteBlock(UInt32 Flash_Address, Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null)
        {
            TaskProgress taskProgress = new(Registers.BoardID);
            UInt32 status = 0;
            UInt32 pageCount = (bytes + PAGE_BYTES - 1) / PAGE_BYTES;
            UInt32 pageAddress = Flash_Address;
            UInt32 writedCount = 0;
            UInt32 pageIndex;
            Int32 byteIndex = (Int32)offset;

            Stopwatch stopwatch = new Stopwatch();
            ResetInitStatus();
            AddLogInfo($"Call WriteBlock:Flash_Address {Flash_Address},offset:{offset},bytes:{bytes}");
            taskProgress.Total = pageCount;
            //进度5%
            UInt32 persentsFive = pageCount / 20;
            for (pageIndex = 0; pageIndex < pageCount; pageIndex++)
            {
                pageAddress = Flash_Address + pageIndex * PAGE_BYTES;
                if (pageIndex == pageCount - 1 || pageIndex % persentsFive == 0)
                {
                    if (updateProcessTip != null)
                    {
                        taskProgress.Now = pageIndex / 2;
                        updateProcessTip(taskProgress);
                    }
                    AddLogInfo($"Call WriteBlock - Writing::{pageIndex + 1}/{pageCount}");
                }
                //if (updateProcessTip != null && (pageIndex == pageCount - 1 || pageIndex % persentsFive == 0))
                //{
                //    taskProgress.Now = pageIndex / 2;
                //    updateProcessTip(taskProgress);
                //    AddLogInfo($"Call WriteBlock - Writing::{pageIndex + 1}/{pageCount}");
                //}
                lock (locker)
                {
                    //step1:
                    WriteDataWithSpi(M25P_CMD_WREN, true);

                    HdIO.WriteReg(Registers.SS, 0);

                    WriteDataWithSpi(M25P_CMD_PP, false);
                    WriteDataWithSpi(((pageAddress & 0xFF0000) >> 16), false);
                    WriteDataWithSpi(((pageAddress & 0x00FF00) >> 8), false);
                    WriteDataWithSpi(((pageAddress & 0x0000ff)), false);

                    #region 256 bytes Data Per Page 
                    for (Int32 len = 0; len < PAGE_BYTES; len++)
                    {
                        if (writedCount < bytes)
                            WriteDataWithSpi(content[byteIndex++], false);
                        else
                            WriteDataWithSpi(0xff, false);//写满一页
                        writedCount++;
                    }
                    #endregion

                }
                #region 等待写完成
                HdIO.WriteReg(Registers.SS, 0x01);
                Thread.Sleep(1);
                HdIO.WriteReg(Registers.SS, 0x0);
                WriteDataWithSpi(M25P_CMD_WRDI, true);
                ResetInitStatus();
                stopwatch.Restart();
                Int32 count = 0;
                do
                {
                    status = QueryStatus() & 0xf;
                    if (status != 0x00)
                    {
                        Thread.Sleep(1);
                        if (stopwatch.ElapsedMilliseconds > (3 * 1000))
                        {
                            break;
                        }
                    }
                    count++;
                } while (status != 0x00);
                if (status != 0x00)
                {
                    ResetInitStatus();
                    return FpgaFlashErrorCode.WriteOvertime;
                }
                #endregion
            }

            #region Verify
            Byte[] readBackData = new Byte[bytes];
            var error = ReadBlock(Flash_Address, ref readBackData, 0, bytes, new Action<TaskProgress>(progress =>
            {
                if (updateProcessTip != null)
                {
                    taskProgress.Now = pageCount * (WRITE_WEIGHT + (MAX_PROGRESS - WRITE_WEIGHT) * progress.Now / progress.Total) / MAX_PROGRESS;
                    updateProcessTip(taskProgress);
                }
            }));
            Boolean bErrorFound = false;
            if (error == FpgaFlashErrorCode.Succeed)
            {
                for (Int32 i = 0; i < bytes; i++)
                {
                    if (readBackData[i] != content[i])
                    {
                        bErrorFound = true;
                        break;
                    }
                }
            }
            else
                bErrorFound = true;
            #endregion  Verify

            #region Verify old before 22.5.15
            //Boolean bErrorFound = false;
            //ResetInitStatus();

            //lock (locker)
            //{
            //    HdIO.WriteReg(Registers.SS, 0);
            //    WriteDataWithSpi(M25P_CMD_READ, false);
            //    //WriteDataWithSpi(0, false);
            //    //WriteDataWithSpi(0, false);
            //    //WriteDataWithSpi(0, false);
            //    WriteDataWithSpi(((Flash_Address & 0xFF0000) >> 16), false);
            //    WriteDataWithSpi(((Flash_Address & 0x00FF00) >> 8), false);
            //    WriteDataWithSpi(((Flash_Address & 0x0000ff)), false);
            //    Byte readbackByte = 0;

            //    for (UInt32 i = 0; i < bytes; i++)
            //    {
            //        readbackByte = (Byte)(ReadDataWithSpi() & 0xff);
            //        if (readbackByte != content[i])
            //        {
            //            bErrorFound = true;
            //            break;
            //        }
            //    }
            //}
            #endregion  Verify old

            ResetInitStatus();
            return (!bErrorFound) ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.WriteVerifyDefeated;
        }
        internal override FpgaFlashErrorCode ReadBlock(UInt32 Flash_Address, ref Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null)
        {
            TaskProgress taskProgress = new(Registers.BoardID);
            HdIO.WriteReg(Registers.SS, 0);

            WriteDataWithSpi(M25P_CMD_READ, false);
            WriteDataWithSpi(((Flash_Address & 0xFF0000) >> 16), false);
            WriteDataWithSpi(((Flash_Address & 0x00FF00) >> 8), false);
            WriteDataWithSpi(((Flash_Address & 0x0000ff)), false);
            taskProgress.Total = bytes;
            //进度5%
            UInt32 persentsFive = bytes / 20;

            for (UInt32 i = 0; i < bytes; i++)
            {
                content[i] = (Byte)(ReadDataWithSpi() & 0xff);
                if (bytes > 3500000 && i % 3500000 == 0)
                {
                    AddLogInfo($"Read:{i}/{bytes}");
                }
                if (updateProcessTip != null && (i == bytes - 1 || i % persentsFive == 0))
                {
                    taskProgress.Now = i;
                    updateProcessTip(taskProgress);
                }
            }
            AddLogInfo($"Read:{bytes}/{bytes}");
            HdIO.WriteReg(Registers.SS, 1);
            if (updateProcessTip != null)
            {
                taskProgress.Now = bytes;
                updateProcessTip(taskProgress);
            }
            return FpgaFlashErrorCode.Succeed;
        }
        protected override FpgaFlashErrorCode EraseOneSector(UInt32 Flash_Address, Int32 MaxWaitSecond)
        {
            ResetInitStatus();

            lock (eraseAllLocker)
            {
                WriteDataWithSpi(M25P_CMD_WREN, true);

                HdIO.WriteReg(Registers.SS, 0);
                WriteDataWithSpi(M25P_CMD_SECTOR_E, false);
                WriteDataWithSpi(((Flash_Address & 0xFF0000) >> 16), false);
                WriteDataWithSpi(((Flash_Address & 0x00FF00) >> 8), false);
                WriteDataWithSpi(((Flash_Address & 0x0000ff)), false);
                HdIO.WriteReg(Registers.SS, 1);

            }
            #region check erase finished
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            UInt32 status = QueryStatus();
            while ((status & 0x3) != 0)
            {
                if (stopwatch.ElapsedMilliseconds > MaxWaitSecond * 1000)
                {
                    break;
                }
                Thread.Sleep(10);
                status = QueryStatus();
            }
            stopwatch.Stop();
            #endregion
            WriteDataWithSpi(M25P_CMD_WRDI, true);
            ResetInitStatus();
            if ((status & 0x3) == 0)
            {
                return FpgaFlashErrorCode.Succeed;
            }
            else
            {
                AddLogInfo($"BoradId:{Registers.BoardID},State:{status}");
                return FpgaFlashErrorCode.EraseOvertime;
            }
            //return ((status & 0x3) == 0) ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.EraseOvertime;
        }
        protected override FpgaFlashErrorCode EraseMultiSectors(UInt32 start_address, UInt32 erase_length, Int32 maxWaitSecond, Action<TaskProgress>? updateProcessTip)
        {
            TaskProgress taskProgress = new(Registers.BoardID);
            if (erase_length == 0 || erase_length % SIZE_64K != 0)
            {
                return FpgaFlashErrorCode.InfoError;
            }
            UInt32 sectorsCount = erase_length / SIZE_64K;
            taskProgress.Total = sectorsCount;
            //进度5%
            UInt32 persentsFive = sectorsCount / 20;
            for (UInt32 i = 0; i < sectorsCount; i++)
            {
                //test
                //AddLogInfo($"Test I:{i}/{sectorsCount}");
                var address = (UInt32)(start_address + i * SIZE_64K);
                var result = EraseOneSector(address, MAX_WAITTIME);
                if (updateProcessTip != null && (i == sectorsCount - 1 || i % persentsFive == 0))
                {
                    taskProgress.Now = i;
                    updateProcessTip(taskProgress);
                }
                if (result != FpgaFlashErrorCode.Succeed)
                {
                    AddLogInfo($"EraseMultiSectors False:{result} , Time:{DateTime.Now}");
                    return result;
                }

            }
            if (updateProcessTip != null)
            {
                taskProgress.Now = sectorsCount;
                updateProcessTip(taskProgress);
            }
            AddLogInfo($"EraseMultiSectors succeed!");
            return FpgaFlashErrorCode.Succeed;
        }
    }
}
