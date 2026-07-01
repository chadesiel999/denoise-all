using System;
using System.Collections.Generic;
using System.Text;

namespace NativeLibraryLoader.MAC
{
    internal class MachOFileCheck : IFileCheck
    {
        private Dictionary<uint, bool> magicToMachOType = new Dictionary<uint, bool>()
        {

            { 0xFEEDFACE, false },
            { 0xFEEDFACF, true  },
            { 0xCEFAEDFE, false },
            { 0xCFFEEDFE, true },
        };
        public MachineType CheckFile(string path)
        {
            MachineType machineType = MachineType.NoSupport;
            var stream = new System.IO.BinaryReader(System.IO.File.OpenRead(path));
            if (magicToMachOType.TryGetValue(stream.ReadUInt32(), out var val))
            {
                if (val) machineType = MachineType.Bit64;
                else machineType = MachineType.Bit32;
            }
            else machineType = MachineType.NoSupport;
            stream.Close();
            stream.Dispose();
            return machineType;
        }
    }
}
