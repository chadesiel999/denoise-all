using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NativeLibraryLoader.Linux
{
    internal class ELFFileCheck : IFileCheck
    {
        public MachineType CheckFile(string path)
        {
            MachineType machineType = MachineType.NoSupport;
            var stream = System.IO.File.OpenRead(path);
            Byte[] bytes = new Byte[] { 0x7f, 0x45, 0x4c, 0x46 };
            if (bytes.All(x => stream.ReadByte() == x))
            {
                switch (stream.ReadByte())
                {
                    case 1:
                        machineType = MachineType.Bit32;
                        break;
                    case 2:
                        machineType = MachineType.Bit64;
                        break;
                    default:
                        machineType = MachineType.NoSupport;
                        break;
                }
            }
            stream.Close();
            stream.Dispose();
            return machineType;
        }
    }
}
