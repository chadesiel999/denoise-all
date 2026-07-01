using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ScopeX.Hardware.Driver
{
    internal class Driver_DCCardPcie : IDriver
    {
        private bool bOpened = false;
        public bool bOpen
        {
            get => bOpened;
            set => bOpened = value;
        }
        public bool Open(string id)
        {
            bOpened = PCIX_Open();
            return bOpened;
        }
        public void Close()
        {
            PCIX_Close();
        }
        StringBuilder stringBuilder = new StringBuilder();
        const int NewSpiDelayUs = 5;
        public void WriterRegister(UInt32 registerAddress, UInt32 data)
        {
            if (Hd.bPrintDebugInformation)
                stringBuilder.AppendLine("0x" + registerAddress.ToString("X") + "=" + data.ToString());
            PCIX_WriteRegister32(registerAddress, data);
#if NEW_SPI_MODE
            HdIO.DelayByUs(NewSpiDelayUs);
#endif
        }
        public UInt32 ReadRegister(UInt32 registerAddress)
        {
            UInt32 result = 0xffffffff;
            PCIX_ReadRegister32(registerAddress, ref result);
#if NEW_SPI_MODE
            HdIO.DelayByUs(NewSpiDelayUs);
            PCIX_ReadRegister32(registerAddress, ref result);
#endif
            return result;
        }
        public bool DMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            return PCIX_DMARead(fromAddress, receiveData, needReadBytes);
        }

        //直接读Card数据
        public Boolean RawDMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            return false;
        }

        public bool DMAWrite(byte[] data, UInt32 byteCount)
        {
            return PCIX_DMAWrite(data, byteCount);
        }
        public bool DMAWrite(UInt32 startAddress, byte[] data, UInt32 byteCount)
        {
            return false;
        }
        #region Private
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern void PCIX_Close();
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_Open();
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_WriteRegister32(UInt32 offset, UInt32 data);
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_ReadRegister32(UInt32 offset, ref UInt32 pdest);
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_DMARead(UInt32 addr, [MarshalAs(UnmanagedType.LPArray)] byte[] destBuffer, UInt32 bytecount);
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_DMAWrite([MarshalAs(UnmanagedType.LPArray)] byte[] sourceBuffer, UInt32 bytecount);
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_DMAOpenChannel(Byte dmachannel);
        [DllImport("libDCCardApi_x64D.dll", CharSet = CharSet.Auto)]
        private static extern Boolean PCIX_DMACloseChannel(Byte dmachannel);
        #endregion
    }
}
