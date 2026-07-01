using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace NativeLibraryLoader
{
    public class NativeLoader
    {
        private IntPtr intPtr;
        public string DLLName { get; }
        public string Extension { get; }
        public Boolean Loaded => intPtr != IntPtr.Zero;
        public string SearchPattern => $"*{DLLName}*.{Extension}";
        public string LoadedFile =>loadedFile;
        private string loadedFile;
        public List<String> SearchDirectories { get; } = new List<string>();
        private IFileCheck fileCheck;
        private INativeLoader nativeLoader;
        public NativeLoader(string dllname)
        {
            fileCheck = GetPlatformFileCheck();
            nativeLoader = GetPlatformLoader();
            Extension = GetPlatformExtension();
            if (System.IO.File.Exists(dllname))
            {
                intPtr = nativeLoader.CoreLoadLibrary(dllname);
                loadedFile = dllname;
            }
            else
            {
                DLLName = dllname;
                SearchDirectories.Add(AppContext.BaseDirectory);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) SearchDirectories.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
            }
            Init();
        }
        public void Init()
        {
            if (Loaded) return;
            if (SearchDirectories.Count == 0) SearchDirectories.Add(AppContext.BaseDirectory);
            string path = SearchDirectories.SelectMany(x =>
            {
                try
                {
                    string[] files = System.IO.Directory.GetFiles(x, SearchPattern, System.IO.SearchOption.AllDirectories);
                    return files;
                }
                catch 
                {
                    return System.IO.Directory.GetFiles(x,SearchPattern, System.IO.SearchOption.TopDirectoryOnly);
                }
            }).FirstOrDefault(x =>
            {
                MachineType machineType = fileCheck.CheckFile(x);
                if (Environment.Is64BitProcess) return machineType == MachineType.Bit64;
                else return machineType == MachineType.Bit32;
            });
            if (!string.IsNullOrEmpty(path))
            {
                intPtr = nativeLoader.CoreLoadLibrary(path);
                loadedFile = path;
            }
        }
        public T LoadFunction<T>(string enterpoint)
        {
            if (intPtr == IntPtr.Zero) return default;
            IntPtr proc = nativeLoader.CoreGetProcAddress(intPtr, enterpoint);
            if (proc != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer<T>(proc);
            }
            else
            {
                string s = nativeLoader.CoreGetLastError();
            }
            return default;
        }
        public IntPtr LoadFunction(string enterpoint)
        {
            if (intPtr == IntPtr.Zero) return intPtr;
            return nativeLoader.CoreGetProcAddress(intPtr, enterpoint);
        }
        public void Unload()
        {
            if(intPtr!=IntPtr.Zero)
            {
                nativeLoader.CoreFreeLibrary(intPtr);
            }
            intPtr = IntPtr.Zero;
        }
        private IFileCheck GetPlatformFileCheck()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { 
                return new Windows.PEFileCheck();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new Linux.ELFFileCheck();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MAC.MachOFileCheck();
            }
            throw new PlatformNotSupportedException("This platform cannot load native libraries.");
        }
        private INativeLoader GetPlatformLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new Windows.WindowNativeLoader();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new Linux.UnixNativeLoader();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)|| RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
            {
                return new MAC.BSDNativeLoader();
            }
            throw new PlatformNotSupportedException("This platform cannot load native libraries.");

        }
        private string GetPlatformExtension()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "dylib";
            }
            throw new PlatformNotSupportedException("This platform cannot load native libraries.");

        }
    }
}
