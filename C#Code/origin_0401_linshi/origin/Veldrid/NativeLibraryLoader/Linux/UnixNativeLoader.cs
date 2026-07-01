using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeLibraryLoader.Linux
{
    internal class UnixNativeLoader : INativeLoader
    {
        void INativeLoader.CoreFreeLibrary(IntPtr intPtr)
        {
            dlclose(intPtr);
        }

        IntPtr INativeLoader.CoreGetProcAddress(IntPtr intPtr, string entryPoint)
        {
            return dlsym(intPtr, entryPoint);
        }

        IntPtr INativeLoader.CoreLoadLibrary(string path)
        {
            return dlopen(path, RTLD_NOW);
        }
        private const string LibName = "libdl";

        const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibName)]
        static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibName)]
        static extern int dlclose(IntPtr handle);

        [DllImport(LibName)]
        static extern string dlerror();

        public string CoreGetLastError()
        {
            return dlerror();
        }
    }
}
