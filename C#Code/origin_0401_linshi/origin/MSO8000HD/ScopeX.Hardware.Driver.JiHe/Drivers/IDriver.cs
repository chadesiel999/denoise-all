using System;
using System.Collections.Generic;
using System.Text;

namespace ScopeX.Hardware.Driver
{
    interface IDriver
    {
        bool bOpen
        {
            get;
            set;
        }
        bool Open(string id);
        void Close();
        void WriterRegister(UInt32 registerAddress, UInt32 data);
        UInt32 ReadRegister(UInt32 registerAddress);
        bool DMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData);

        //直接DMA读PCIE中DDR的数据
        Boolean RawDMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData);

        bool DMAWrite(byte[] data, UInt32 byteCount);
        Boolean DMAWrite(UInt32 startAddr, Byte[] data, UInt32 byteCount);
    }
}
