using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace ScopeX.U2.Tools
{
    internal static class SystemSoftKeyboard
    {
        /// <summary>
        /// TabTip.exe键盘所在目录
        /// </summary>
        private static String _TabTipDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "microsoft shared", "ink");

        private const String TABTIP_NAME = "TabTip.exe";

        /// <summary>
        /// TabTip.exe键盘全路径
        /// </summary>
        private static String _TabTipPath = Path.Combine(_TabTipDir, TABTIP_NAME);

        private static Boolean _IsTabTipExist = false;

        static SystemSoftKeyboard()
        {
            _IsTabTipExist = System.IO.File.Exists(_TabTipPath);
        }
        /// <summary>
        /// 显示系统软键盘，osk.exe是单进程程序，只会有一个实例。
        /// </summary>
        public static void Show()
        {
            if (_IsTabTipExist)
            {
                if (!TabTipHelper.IsInputPaneOpen())
                    TabTipHelper.ToggleTouchKeyboard();
            }
            else
            {
                ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.SystemDirectory;
                startInfo.FileName = "osk.exe";
                startInfo.Verb = "runas";
                Process.Start(startInfo);
            }
        }

        /// <summary>
        ///  关闭系统软键盘
        /// </summary>
        public static void Close()
        {
            if (_IsTabTipExist)
            {
                TabTipHelper.ToggleTouchKeyboard();
            }
            else
            {
                var oskexe = Process.GetProcessesByName("osk");
                if (oskexe != null)
                {
                    foreach (var p in oskexe)
                        p?.Kill();
                }
            }
        }

        /// <summary>
        /// TabTip软键盘接口封装
        /// 
        /// 使用com组件的方式，强制调用Windows未公开的API，但是可能在后续的Windows版本中被更改从而不适用
        /// </summary>
        private class TabTipHelper
        {
            /// <summary>
            /// 切换TabTip软键盘状态
            /// </summary>
            /// <returns></returns>
            public static Boolean ToggleTouchKeyboard()
            {
                try
                {
                    UIHostNoLaunch uiHostNoLaunch = new UIHostNoLaunch();
                    ((ITipInvocation)uiHostNoLaunch).Toggle(GetDesktopWindow());
                    Marshal.ReleaseComObject(uiHostNoLaunch);
                }
                catch (COMException exc)
                {
                    if (exc.HResult == unchecked((Int32)0x80040154)) // REGDB_E_CLASSNOTREG
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo(TABTIP_NAME)
                        {
                            UseShellExecute = true
                        };
                        using (Process? process = Process.Start(processStartInfo))
                        {
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
            private class UIHostNoLaunch
            {
            }

            [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface ITipInvocation
            {
                void Toggle(IntPtr hwnd);
            }

            [DllImport("user32.dll", SetLastError = false)]
            private static extern IntPtr GetDesktopWindow();

            /// <summary>
            /// TabTip软键盘是否已打开
            /// </summary>
            /// <returns></returns>
            public static Boolean IsInputPaneOpen()
            {
                FrameworkInputPane frameworkInputPane = new FrameworkInputPane();
                Rectangle rect;
                ((IFrameworkInputPane)frameworkInputPane).Location(out rect);
                Marshal.ReleaseComObject(frameworkInputPane);
                return !rect.IsEmpty;
            }

            [ComImport, Guid("d5120aa3-46ba-44c5-822d-ca8092c1fc72")]
            public class FrameworkInputPane
            {
            }

            [ComImport, Guid("5752238b-24f0-495a-82f1-2fd593056796")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IFrameworkInputPane
            {
                Int32 Advise([MarshalAs(UnmanagedType.IUnknown)] Object pWindow, [MarshalAs(UnmanagedType.IUnknown)] Object pHandler, out Int32 pdwCookie);
                Int32 AdviseWithHWND(IntPtr hwnd, [MarshalAs(UnmanagedType.IUnknown)] Object pHandler, out Int32 pdwCookie);
                Int32 Unadvise(Int32 pdwCookie);
                Int32 Location(out Rectangle prcInputPaneScreenLocation);
            }
        }
    }
}
