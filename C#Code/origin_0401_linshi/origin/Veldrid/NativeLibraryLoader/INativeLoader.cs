using System;
using System.Collections.Generic;
using System.Text;

namespace NativeLibraryLoader
{
    internal interface INativeLoader
    {
        IntPtr CoreLoadLibrary(string path);
        IntPtr CoreGetProcAddress(IntPtr intPtr, string entryPoint);
        void CoreFreeLibrary(IntPtr intPtr);
        string CoreGetLastError();
    }
}
