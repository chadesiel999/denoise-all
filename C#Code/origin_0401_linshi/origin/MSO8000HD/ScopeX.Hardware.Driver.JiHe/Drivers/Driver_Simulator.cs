using System;
using System.Collections.Generic;
using System.Text;

namespace ScopeX.Hardware.Driver
{
    class Driver_Simulator : IDriver
    {
        private bool bOpened = false;
        public bool bOpen
        {
            get => bOpened;
            set => bOpened = value;
        }
        public bool Open(string id)
        {
            return true;
        }
        public void Close()
        {

        }
        public void WriterRegister(UInt32 registerAddress, UInt32 data)
        {

        }
        public UInt32 ReadRegister(UInt32 registerAddress)
        {
            return 0xffffffff;
        }
        public bool DMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            return true;
        }

        //直接读Card数据,而非有上下的读取
        public Boolean RawDMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            return false;
        }

        public bool DMAWrite(byte[] data, UInt32 byteCount)
        {
            return true;
        }
        public bool DMAWrite(UInt32 startAddress, byte[] data, UInt32 byteCount)
        {
            return false;
        }
    }
}
