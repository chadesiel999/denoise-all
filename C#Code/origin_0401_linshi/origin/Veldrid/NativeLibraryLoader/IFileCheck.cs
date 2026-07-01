using System;
using System.Collections.Generic;
using System.Text;

namespace NativeLibraryLoader
{
    internal interface IFileCheck
    {
        MachineType CheckFile(string path);
    }
    
}
