#if JiHe_MSO7000X || JiHe_MSO8000X
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class Driver_JiHePcie : IDriver
    {
        PCIeControl.PCIe? control;
        private Int32 ReadDMATimeout => 200;
        public Boolean bOpen
        {
            get
            {
                if (control == null) return false;
                return control.IsOpen;
            }
            set
            {

            }
        }

        public void Close()
        {
            control?.Close();
        }
        private object WriteReadLocker = new object();
        public Boolean DMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            if (control == null)
            {
                return false;
            }
            int timeOut = 4000;
            if (Hd.UIMessage?.Timebase != null)
            {
                if (Hd.UIMessage.Timebase.StorageWaveDotsCnt < 5e8)
                {
                    timeOut = 4000;
                }
                else if (Hd.UIMessage.Timebase.StorageWaveDotsCnt < 1e9)
                {
                    timeOut = 8000;
                }
                else if (Hd.UIMessage.Timebase.StorageWaveDotsCnt <= 2e9)
                {
                    timeOut = 16000;
                }
            }
            if (!HdIO.CheckRegisterValue((uint)PcieBdReg.R.Xdma_XdmaWrFinish, 1, 0x01, timeOut))
            {
                if (Hd.CurrProduct?.AcqBd != null)
                {
                    //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ChannelMode, 0);
                    //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ChannelMode, 1);
                    //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ChannelMode, 0);

                    UInt32 CountH16 = HdIO.ReadReg(PcieBdReg.R.Debug_PcieDataCountH16);
                    UInt32 CountL16 = HdIO.ReadReg(PcieBdReg.R.Debug_PcieDataCountL16);
                    HdIO.WriteReg(PcieBdReg.W.RegMonitor_RegAddress, 0X00A4);

                    UInt32 ReadbackValue = HdIO.ReadReg(PcieBdReg.R.RegMonitor_ReadbackValue);

                    Hd.SysLogger?.Invoke(String.Format("Debug_PcieDataCountH16：{0}，Debug_PcieDataCountL16：{1}，DataNum：{2}",
                        CountH16, CountL16, ReadbackValue), "Info");

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("From Driver: HdIO.CheckRegisterValue((uint)PcieBdReg.R.Xdma_XdmaWrFinish, 1, 0x01, 4000)) is false");

                    //stringBuilder.AppendLine("=======Begin  Acq Board Register Readback Values==========");
                    //stringBuilder.AppendLine(Hd.CurrProduct!.AcqBd!.GetRegMonitorResult());
                    //stringBuilder.AppendLine("=======end Acq Board Register Readback Values==========");
                    Hd.SysLogger?.Invoke(stringBuilder.ToString(), "Warning!!!!");
                }

                return false;
            }
            else
            {
                //Dma完成后即可关闭处理板读使能
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            }
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.ReadDMAStream.ReadFile(fromAddress, (Int32)needReadBytes, ref receiveData);
            }
            if (bOK)
            {
                WriterRegister((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
                WriterRegister((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);
            }
            return bOK;
        }

        //直读DMA(PCIE的DDR的数据)
        public Boolean RawDMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.ReadDMAStream.ReadFile(fromAddress, (Int32)needReadBytes, ref receiveData);
            }
            return bOK;
        }

        public Boolean DMAWrite(Byte[] data, UInt32 byteCount)
        {
            if (control == null)
            {
                return false;
            }
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.WriteDMAStream.WriteFile(0, data);
            }
            return bOK;
        }

        public Boolean DMAWrite(UInt32 startAddr, Byte[] data, UInt32 byteCount)
        {
            if (control == null)
            {
                return false;
            }
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.WriteDMAStream.WriteFile(startAddr, data);
            }
            return bOK;
        }

        public Boolean Open(String id)
        {
            var pcies = PCIeControl.PCIe.GetDeviceInfos();
            if (pcies == null || pcies.Count == 0)
            {
                ComModel.ErrorCode.ErrorType = ComModel.ErrorType.S_PCIE_NotFound_0004;
                return false;
            }
            control = pcies[0].Open();
            bOpen = control.IsOpen;
            if (!bOpen)
            {
                ComModel.ErrorCode.ErrorType = ComModel.ErrorType.S_PCIE_Open_Error_0005;
            }
            return bOpen;
        }

        public UInt32 ReadRegister(UInt32 registerAddress)
        {
            if (control == null)
            {
                return 0;
            }
            UInt32 value = 0;
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.UserStream.ReadFile(registerAddress, ref value);
            }
            if (!bOK)
            {
                throw new Exception($"Register {registerAddress:X4} Read Error");
            }
            if (value == 0xffff_ffff)
            {
                Hd.ErrorMsgbox?.Invoke(-1);
            }
            return value;
        }

        public void WriterRegister(UInt32 registerAddress, UInt32 data)
        {
            if (control == null)
            {
                return;
            }
            Boolean bOK = false;
            lock (WriteReadLocker)
            {
                bOK = control.UserStream.WriteFile(registerAddress, data);
                UInt32 readbackData = ReadRegister(0x0000);
                if (readbackData==0)
                {
                    readbackData++;
                }
            }
            if (!bOK)
            {
                throw new Exception($"Register {registerAddress:X4} Writer Error");
            }
        }
    }
}
#endif