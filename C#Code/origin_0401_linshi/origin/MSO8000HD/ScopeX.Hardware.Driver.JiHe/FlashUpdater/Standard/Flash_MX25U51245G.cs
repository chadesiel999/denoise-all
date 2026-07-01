using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    class Flash_MX25U51245G : RootFpgaFlash
    {
        public Flash_MX25U51245G(FpgaFlash_Registers FpgaFlash_Registers)
        {
            Registers = FpgaFlash_Registers;
            TransferPerByteUsedNs = (UInt32)(10 * 1000L / (REFBASECLOKE_MHZ / ((Registers.SpiClockSetValue + 1) * 2))) + 320;
        }

        private const Int32 SIZE_16MB = 16777216;
        private const Int32 SIZE_48MB = SIZE_16MB * 3;
        private const Int32 SIZE_32MB = SIZE_16MB * 2;

        private const UInt32 SIZE_64K = 64 * 1024;
        private const Int32 MAX_WAITTIME = 30;

        //写入占进度权重
        private const Int32 WRITE_WEIGHT = 50;
        private const Int32 MAX_PROGRESS = 100;

        #region FLASH_CMD Define
        const Byte M25P_CMD_WREN = 0x06;  //Write Enable
        const Byte M25P_CMD_WRDI = 0x04;  //Write disable
        const Byte M25P_CMD_RDID = 0x9f;  //Read Identification
        const Byte M25P_CMD_RDSR = 0x05;  //Read Status Register
                                          //const Byte M25P_CMD_RD_FLAG_SR = 0x70;  //Read Flag Register
                                          //const Byte M25P_CMD_RD_NONV_SR = 0xB5;  //Read nonvo Register
                                          //const Byte M25P_CMD_WR_NONV_SR = 0xB1;  //Write nonvo Register
        const Byte M25P_CMD_WRSR = 0x01;  //Write Status Register
        const Byte M25P_CMD_READ = 0x03;  //Read Data Bytes 
        const Byte M25P_CMD_WR_EAR_SR = 0xC5;  //Write EAR
        const Byte M25P_CMD_RD_EAR_SR = 0xC8;  //Read EAR
                                               //const Byte M25P_CMD_READ = 0x13;  //Read Data Bytes 
                                               //const Byte M25P_CMD_PP = 0x12;  //Page Program
        const Byte M25P_CMD_PP = 0x02;  //Page Program
        const Byte M25P_CMD_BE = 0xc7;  //Block Erase
        const Byte M25P_CMD_ID = 0x9f;   //Read ID
        const Byte M25P_CMD_B4K_E = 0x20;  //Block 4K Erase
        const Byte M25P_CMD_SECTOR_E = 0xd8;  //SECTOR ERASE Erase
                                              //const Byte M25P_CMD_EN_4BYTE_ADDR = 0xb7;  // 4字节使能 
                                              //const Byte M25P_CMD_DI_4BYTE_ADDR = 0xe9;   //  4字节退出
        #endregion FLASH_CMD_Define

        private const Int64 REFBASECLOKE_MHZ = 125;//125MHz

        public UInt32 TransferPerByteUsedNs { get; init; }

        private Object writerLocker = new Object();
        public void WriteDataWithSpi(UInt32 data, Boolean switchSS = false)
        {
            lock (writerLocker)
            {
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 1);

                HdIO.WriteReg(Registers.WriteData, data);
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 0);
                HdIO.WriteReg(Registers.WriteStart, 1, (Int32)TransferPerByteUsedNs / 1000);
                HdIO.WriteReg(Registers.WriteStart, 0, 0);
                if (switchSS)
                    HdIO.WriteReg(Registers.SS, 1);
            }
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

        public override Boolean LockFlashProtect()
        {
            var state = QueryStatus() & FLASH_PROTECTED_MARK;
            if (state != 0)
            {
                return true;
            }
            if (!SetFlashToLock()) return false;
            state = QueryStatus() & FLASH_PROTECTED_MARK;
            return state != 0;
        }
        private Object readerLocker = new Object();
        private UInt32 ReadDataWithSpi()
        {
            lock (readerLocker)
            {
                HdIO.WriteReg(Registers.ReadStart, 1, (Int32)TransferPerByteUsedNs / 1000);
                UInt32 result = (UInt16)(HdIO.ReadReg(Registers.ReadData) & 0xff);
                HdIO.WriteReg(Registers.ReadStart, 0, 0);
                return result;
            }
        }
        private Object readIDLocker = new Object();
        private Object queryStatusLocker = new Object();
        private void ResetInitStatus()
        {
            HdIO.WriteReg(Registers.SS, 0x01);
            //test 11.15
            //WriteDataWithSpi(M25P_CMD_WRDI, true);
            HdIO.WriteReg(Registers.ReadStart, 0x00);
            HdIO.WriteReg(Registers.WriteStart, 0x00);

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
            ResetInitStatus();
            return (status == 0) ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.EraseOvertime;
        }
        const Int32 PAGE_BYTES = 256;
        private Object locker = new Object();

        private Byte GetEarAddrReg()
        {
            HdIO.WriteReg(Registers.SS, 0x00);
            WriteDataWithSpi(M25P_CMD_RD_EAR_SR, false);
            var status = (Byte)ReadDataWithSpi();
            HdIO.WriteReg(Registers.SS, 0x01);
            Thread.Sleep(1);
            ResetInitStatus();
            Debug.WriteLine($"Read EAR: 0x{status:X2}");
            return status;
        }
        private void WaitWritingBusy()
        {
            while ((QueryStatus() & 0x01) == 0x01) ;
        }

        private void ChangeAddrMode(UInt32 pageAddress, ref Byte earAddr)
        {

            var needChange = false;
            if (pageAddress < SIZE_16MB)
            {
                needChange = earAddr != 0;
                earAddr = 0;
            }
            else if (pageAddress >= SIZE_16MB && pageAddress < SIZE_32MB)
            {
                needChange = earAddr != 1;
                earAddr = 1;
            }
            else if (pageAddress >= SIZE_32MB && pageAddress < SIZE_48MB)
            {
                needChange = earAddr != 2;
                earAddr = 2;
            }
            else if (pageAddress >= SIZE_48MB)
            {
                needChange = earAddr != 3;
                earAddr = 3;
            }

            if (needChange)
            {
                Debug.WriteLine($"Change Addr:0x{pageAddress:X8} ,To EAR:{earAddr}");
                //AddLogInfo($"Addr:{pageAddress} --- 切换到高地址");
                WriteDataWithSpi(M25P_CMD_WREN, true);
                HdIO.WriteReg(Registers.SS, 0x00);
                WriteDataWithSpi(M25P_CMD_WR_EAR_SR, false);
                WriteDataWithSpi(earAddr, false);
                HdIO.WriteReg(Registers.SS, 0x01);
                //test 11.15
                //WriteDataWithSpi(M25P_CMD_WRDI, true);
                //Thread.Sleep(1);
                WaitWritingBusy();

            }
        }

        private void CloseAddrMode(ref Byte earAddrReg)
        {
            //AddLogInfo($"完成 --- 切换到低地址");
            WriteDataWithSpi(M25P_CMD_WREN, true);
            HdIO.WriteReg(Registers.SS, 0x00);
            WriteDataWithSpi(M25P_CMD_WR_EAR_SR, false);
            WriteDataWithSpi(0, false);
            HdIO.WriteReg(Registers.SS, 0x01);
            earAddrReg = 0;
            //test 11.15
            //WriteDataWithSpi(M25P_CMD_WRDI, true);
            //Thread.Sleep(1);
            WaitWritingBusy();
        }
        protected override UInt32 ReadID()
        {
            lock (readIDLocker)
            {
                HdIO.WriteReg(Registers.SS, 0x00);
                HdIO.WriteReg(Registers.SS, 0x01);

                HdIO.WriteReg(Registers.ReadStart, 0x00);
                HdIO.WriteReg(Registers.ReadStart, 0x01);

                HdIO.WriteReg(Registers.ReadStart, 0x00);
                HdIO.WriteReg(Registers.WriteStart, 0x00);

                //HdIO.WriteReg (currFPGA_Register.SS, 0x00);

                HdIO.WriteReg(Registers.SS, 0x00);

                WriteDataWithSpi(M25P_CMD_ID, false);
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
            Byte earHighAddr = 0;
            Stopwatch stopwatch = new();
            //for (Int32 i = 0; i < 256; i++)
            //{
            //    content[i] = (Byte)i;
            //}
            //ljw 22.4.2
            ResetInitStatus();

            AddLogInfo($"Call WriteBlock:Flash_Address 0x{Flash_Address:X0},offset:{offset},bytes:{bytes}");

            #region 22.5.11 ljw
            earHighAddr = GetEarAddrReg();
            #endregion 22.5.11 ljw

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
                if (pageAddress % 300000 == 0)
                {
                    AddLogInfo($"烧写进度:{pageIndex}/{pageCount}");
                }
                if (updateProcessTip != null && (pageIndex == pageCount - 1 || pageIndex % persentsFive == 0))
                {
                    taskProgress.Now = pageIndex / 2;
                    updateProcessTip(taskProgress);
                }
                pageAddress = Flash_Address + pageIndex * PAGE_BYTES;

                #region 22.5.11 ljw
                ChangeAddrMode(pageAddress, ref earHighAddr);
                #endregion 22.5.11 ljw

                lock (locker)
                {
                    //step1:

                    WriteDataWithSpi(M25P_CMD_WREN, true);
                    //WriteDataWithSpi(M25P_CMD_EN_4BYTE_ADDR, true);

                    HdIO.WriteReg(Registers.SS, 0);

                    WriteDataWithSpi(M25P_CMD_PP, false);
                    //WriteDataWithSpi(((pageAddress & 0xFF000000) >> 24), false);
                    WriteDataWithSpi(((pageAddress & 0x00FF0000) >> 16), false);
                    WriteDataWithSpi(((pageAddress & 0x0000FF00) >> 8), false);
                    WriteDataWithSpi(((pageAddress & 0x000000FF)), false);

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
                #region 检查页写完成
                ResetInitStatus();
                stopwatch.Restart();
                do
                {
                    status = (QueryStatus());
                    if ((status & 0x3) != 0x00)
                    {
                        Thread.Sleep(1);
                        if (stopwatch.ElapsedMilliseconds > (1000))
                        {
                            break;
                        }
                    }
                } while ((status & 0x3) != 0x00);
                if ((status & 0x3) != 0x00)
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
                        AddLogInfo($"bErrorFound: i:{i} data:0x{readBackData[i]:X2}/0x{content[i]:X2}");
                        bErrorFound = true;
                        break;
                    }
                }
            }
            else
                bErrorFound = true;
            #endregion  Verify

            #region 22.5.11 ljw
            CloseAddrMode(ref earHighAddr);
            #endregion 22.5.11 ljw

            ResetInitStatus();
            return (!bErrorFound) ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.WriteVerifyDefeated;
        }
        internal override FpgaFlashErrorCode ReadBlock(UInt32 Flash_Address, ref Byte[] content, UInt32 offset, UInt32 bytes, Action<TaskProgress>? updateProcessTip = null)
        {
            AddLogInfo($"ReadBlock:0x{Flash_Address:X}");
            TaskProgress taskProgress = new(Registers.BoardID);
            #region 22.5.11 ljw
            var earAddrReg = GetEarAddrReg();
            ChangeAddrMode(Flash_Address, ref earAddrReg);
            #endregion 22.5.11 ljw

            #region test 22.11.15
            AddLogInfo($"State: 0x{QueryStatus():X2}");
            #endregion  test 22.11.15

            HdIO.WriteReg(Registers.SS, 0);

            WriteDataWithSpi(M25P_CMD_READ, false);
            // WriteDataWithSpi(0, false);
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
                    AddLogInfo($"Read Data:{i}/{bytes}");
                }
                if (persentsFive != 0 && updateProcessTip != null && (i == bytes - 1 || i % persentsFive == 0))
                {
                    taskProgress.Now = i;
                    updateProcessTip(taskProgress);
                }
            }

            AddLogInfo($"Read Data:{bytes}/{bytes}");
            HdIO.WriteReg(Registers.SS, 1);
            #region 22.5.11 ljw
            CloseAddrMode(ref earAddrReg);
            ResetInitStatus();
            #endregion 22.5.11 ljw
            if (updateProcessTip != null)
            {
                taskProgress.Now = bytes;
                updateProcessTip(taskProgress);
            }
            return FpgaFlashErrorCode.Succeed;
        }

        protected override FpgaFlashErrorCode EraseOneSector(UInt32 Flash_Address, Int32 MaxWaitSecond)
        {
            Debug.WriteLine($"擦除：0x{Flash_Address:X}");
            ResetInitStatus();
            #region 22.5.11 ljw
            var earAddrReg = GetEarAddrReg();
            ChangeAddrMode(Flash_Address, ref earAddrReg);
            #endregion 22.5.11 ljw

            lock (eraseAllLocker)
            {
                WriteDataWithSpi(M25P_CMD_WREN, true);
                HdIO.WriteReg(Registers.SS, 0x00);
                WriteDataWithSpi(M25P_CMD_SECTOR_E, false);
                WriteDataWithSpi(((Flash_Address & 0xFF0000) >> 16), false);
                WriteDataWithSpi(((Flash_Address & 0x00FF00) >> 8), false);
                WriteDataWithSpi(((Flash_Address & 0x0000ff)), false);
                HdIO.WriteReg(Registers.SS, 1);
            }
            #region check erase finished
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var status = QueryStatus();
            while ((status & 0x03) != 0)
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
            CloseAddrMode(ref earAddrReg);
            ResetInitStatus();

            return (status & 0x03) == 0 ? FpgaFlashErrorCode.Succeed : FpgaFlashErrorCode.EraseOvertime;
        }
        protected override FpgaFlashErrorCode EraseMultiSectors(UInt32 start_address, UInt32 erase_length, Int32 maxWaitSecond, Action<TaskProgress>? updateProcessTip)
        {
            TaskProgress taskProgress = new(Registers.BoardID);
            if (erase_length == 0 || erase_length % SIZE_64K != 0)
            {
                return FpgaFlashErrorCode.ErrorEraseLength;
            }
            ResetInitStatus();

            #region 22.5.11 ljw
            //var earAddrReg = GetEarAddrReg();
            #endregion 22.5.11 ljw
            Stopwatch stopwatch = new Stopwatch();
            UInt32 sectorsCount = erase_length / SIZE_64K;

            taskProgress.Total = sectorsCount;
            //进度5%
            UInt32 persentsFive = sectorsCount / 20;
            stopwatch.Start();
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
            UInt32 WEL = 0;
            if (status == 0)
            {
                return true;
            }
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
            WriteDataWithSpi((UInt32)FLASH_PROTECTED_MARK, false);
            //WriteDataWithSpi(0, false);
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

            return (status & FLASH_PROTECTED_MARK) != 0;
        }
        public override Boolean ResetFlashStatusReg()
        {
            const Int32 testTimes = 20;
            Int32 count = 0;
            UInt32 status = QueryStatus();
            UInt32 WEL = 0;
            if (status == 0)
            {
                return true;
            }
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
            //return true;
        }
    }
}


