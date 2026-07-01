using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeLibraryLoader.MAC
{
    internal class BSDNativeLoader : INativeLoader
    {
        public void CoreFreeLibrary(IntPtr intPtr)
        {
            dlclose(intPtr);
        }

        public string CoreGetLastError()
        {
            return dlerror();
        }

        public IntPtr CoreGetProcAddress(IntPtr intPtr, string entryPoint)
        {
            return dlsym(intPtr, entryPoint);
        }

        public IntPtr CoreLoadLibrary(string path)
        {
            return dlopen(path,RtldNow);
        }

        private const string LibName = "libc";

        const int RtldNow = 0x002;

        [DllImport(LibName)]
        static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibName)]
        static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibName)]
        static extern IntPtr dlclose(IntPtr handle);

        [DllImport(LibName)]
        static extern string dlerror();
    }
}
