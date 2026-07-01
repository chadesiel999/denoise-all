using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventBus;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public static class HelpProcessManager
    {
        private const Int32 WM_COPYDATA = 0x004A;
        private const Int32 WM_LANGUAGE = 0x04A0;
        private const String HELP_PROCESS_NAME = "help";
        private const String PDFVIEWER_PROCESS_NAME = "PDFViewer";

        //private static Process _HelpProcess;

        public static void StartProcess()
        {
            try
            {
                CloseProcess();
                //运行之前清除缓存文件
                ClearHelpCache();
                Process.Start(HELP_PROCESS_NAME + ".exe");
                Process.Start($"ScopeXSysMonitor.exe");
            }
            catch
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====Fail to call Help.exe! =====", EventBus.LogLevel.Error));
#if DEBUG
                Trace.WriteLine($"=====Fail to call Help.exe! =====");
#endif

            }

            //if (!System.IO.File.Exists("scHelp.pdf"))
            //{
            //    WeakTip.Default.Write("File", MsgTipId.FileExisted);
            //    return;
            //}

            //ProcessStartInfo psi = new()
            //{
            //    FileName = "scHelp.pdf",
            //    UseShellExecute = true,
            //};
            //_HelpProcess = Process.Start(psi);
        }

        /// <summary>
        /// 清除help缓存
        /// </summary>
        public static void ClearHelpCache()
        {
            var applicationdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var helprunpath = @$"{applicationdata}\ScopeX";
            DelectDir(helprunpath);
        }

        private static void DelectDir(String srcPath)
        {
            try
            {
                if (string.IsNullOrEmpty(srcPath) || !Directory.Exists(srcPath))
                    return;

                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    try
                    {
                        if (i is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            System.IO.File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                    catch (Exception ex)
                    {
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish("HelpProcess", new LogEventArgs(ex.Message, LogLevel.Warn));
                    }
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish("HelpProcess", new LogEventArgs(ex.Message, LogLevel.Warn));
            }
        }

        public static void CloseProcess()
        {
            RunningInstance();
        }

        private static Process GetPDFViewerProcess()
        {
            Process[] processes = Process.GetProcessesByName(PDFVIEWER_PROCESS_NAME);
            if (processes.Length == 0)
            {
                return null;
            }
            else
            {
                return processes[0];
            }
        }

        private static void RunningInstance()
        {
            var prolist = new List<Process>();
            Process[] processes = Process.GetProcessesByName(HELP_PROCESS_NAME);
            if (processes.Length > 0)
            {
                prolist.AddRange(processes.ToList());
            }

            processes = null;
            processes = Process.GetProcessesByName(PDFVIEWER_PROCESS_NAME);
            if (processes.Length > 0)
            {
                prolist.AddRange(processes.ToList());
            }
            prolist.ForEach(p => p.Kill());

            processes = null;
            processes = Process.GetProcessesByName($"ScopeXSysMonitor");
            if (processes.Length > 0)
            {
                prolist.AddRange(processes.ToList());
            }
            prolist.ForEach(p => p.Kill());
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public Int32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public String lpData;
        }


        public static void SendCommand(String command = "")
        {
#if DEBUG
            Debug.WriteLine(command);
#endif
            if (GetPDFViewerProcess() == null)
            {
                CloseProcess();
                StartProcess();
                Thread.Sleep(1000);//等待进程开启
            }
            var windowhandle = ProcessHandleUtil.GetHandleByProcessName(PDFVIEWER_PROCESS_NAME + ".exe");
            if (windowhandle != IntPtr.Zero)
            {
                var sarr = System.Text.Encoding.Default.GetBytes(command);
                var len = sarr.Length;
                NativeMethods.CopyDataStruct cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = command;
                cds.cbData = len + 1;
                _ = NativeMethods.SendMessage(windowhandle, WM_COPYDATA, 0, ref cds);
            }

        }

        public static void SendCommand(Int32 cmd)
        {
#if DEBUG

            Debug.WriteLine(cmd);
#endif
            //if (GetPDFViewerProcess() == null)
            //{
            //    CloseProcess();
            //    StartProcess();
            //    Thread.Sleep(1000);//等待进程开启
            //}
            var windowhandle = ProcessHandleUtil.GetHandleByProcessName(PDFVIEWER_PROCESS_NAME + ".exe");
            if (windowhandle != IntPtr.Zero)
            {
                _ = NativeMethods.PostMessage(windowhandle, WM_LANGUAGE, 0, cmd);
            }
        }
    }

    /// <summary>
    /// 工具类  根据进程名获取主窗口句柄
    /// </summary>
    internal class ProcessHandleUtil
    {
        private static Hashtable _ProcessWnd = null;
        static ProcessHandleUtil()
        {
            if (_ProcessWnd == null)
            {
                _ProcessWnd = new();
            }
        }

        /// <summary>
        /// 根据进程名获取主窗口句柄
        /// </summary>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public static IntPtr GetHandleByProcessName(String ProcessName)
        {
            var list = new List<NativeMethods.ProcessEntry32>();
            var handle = NativeMethods.CreateToolhelp32Snapshot(0x2, 0);
            var hh = IntPtr.Zero;
            if ((Int32)handle > 0)
            {
                var pe32 = new NativeMethods.ProcessEntry32();
                pe32.dwSize = (uint)Marshal.SizeOf(pe32);
                var more = NativeMethods.Process32First(handle, ref pe32);
                while (more == 1)
                {
                    var temp = Marshal.AllocHGlobal((int)pe32.dwSize);
                    Marshal.StructureToPtr(pe32, temp, true);
                    NativeMethods.ProcessEntry32 pe = (NativeMethods.ProcessEntry32)Marshal.PtrToStructure(temp, typeof(NativeMethods.ProcessEntry32));
                    Marshal.FreeHGlobal(temp);
                    list.Add(pe);
                    if (pe.szExeFile == ProcessName)
                    {
                        more = 2;
                        hh = GetCurrentWindowHandle(pe.th32ProcessID);
                        break;
                    }
                    more = NativeMethods.Process32Next(handle, ref pe32);
                }
            }
            return hh;
        }

        private static IntPtr GetCurrentWindowHandle(uint proid)
        {
            var ptrWnd = IntPtr.Zero;
            var uiPid = proid;
            var objWnd = _ProcessWnd[uiPid];
            if (objWnd != null)
            {
                ptrWnd = (IntPtr)objWnd;
                if (ptrWnd != IntPtr.Zero && NativeMethods.IsWindow(ptrWnd))  // 从缓存中获取句柄
                {
                    return ptrWnd;
                }
                else
                {
                    ptrWnd = IntPtr.Zero;
                }
            }
            var bResult = NativeMethods.EnumWindows(new NativeMethods.WNDENUMPROC(EnumWindowsProc), uiPid);
            // 枚举窗口返回 false 并且没有错误号时表明获取成功
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = _ProcessWnd[uiPid];
                if (objWnd != null)
                {
                    ptrWnd = (IntPtr)objWnd;
                }
            }
            return ptrWnd;
        }

        private static Boolean EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint uiPid = 0;
            if (NativeMethods.GetParent(hwnd) == IntPtr.Zero)
            {
                NativeMethods.GetWindowThreadProcessId(hwnd, ref uiPid);
                if (uiPid == lParam)    // 找到进程对应的主窗口句柄
                {
                    if (_ProcessWnd.ContainsKey(uiPid))
                    {
                        _ProcessWnd.Remove(uiPid);
                    }
                    _ProcessWnd.Add(uiPid, hwnd);   // 把句柄缓存起来
                    NativeMethods.SetLastError(0);    // 设置无错误
                    return false;   // 返回 false 以终止枚举窗口
                }
            }
            return true;
        }
    }
}
