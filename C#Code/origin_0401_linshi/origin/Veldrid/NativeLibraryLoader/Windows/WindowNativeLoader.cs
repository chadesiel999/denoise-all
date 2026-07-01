using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeLibraryLoader.Windows
{
    internal class WindowNativeLoader : INativeLoader
    {
        void INativeLoader.CoreFreeLibrary(IntPtr intPtr)
        {
            FreeLibrary(intPtr);
        }

        string INativeLoader.CoreGetLastError()
        {
            return Marshal.GetLastWin32Error()+"";
        }

        IntPtr INativeLoader.CoreGetProcAddress(IntPtr intPtr, string entryPoint)
        {
            return GetProcAddress(intPtr, entryPoint);
        }

        IntPtr INativeLoader.CoreLoadLibrary(string path)
        {
            return LoadLibrary(path);
        }
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);
    }
}
