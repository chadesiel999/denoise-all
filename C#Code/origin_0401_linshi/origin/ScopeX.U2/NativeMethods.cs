using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal static class NativeMethods
    {
        /// <summary>
        /// 设备改变
        /// </summary>
        public const int WM_DEVICECHANGE = 0x219; 
        /// <summary>
        /// 插入设备
        /// </summary>
        public const int DBT_DEVICEARRIVAL = 0x8000; 
        /// <summary>
        /// 移除设备
        /// </summary>
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        /// <summary>
        /// 获得焦点
        /// </summary>
        public const int WM_SETFOCUS = 0x0007;

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public const int SC_CLOSE = 0xF060;

        /// <summary>
        /// 系统命令消息，当点击窗口的最大化、最小化、关闭等命令时，收到这个消息
        /// </summary>
        public const int WM_SYSCOMMAND = 0x0112;

        public const Int32 WM_USERKEYDOWN = 0x0400;
        public const Int32 WM_USERCLICK = 0x0401;

        public const Int32 WM_KEYDOWN = 0x0100;

        public const Int32 WM_PARENTNOTIFY = 0x210;
        public const Int32 WM_LBUTTONDOWN = 0x201;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SendMessage(IntPtr wnd, Int32 msg, Int32 wP, Int32 lP);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 PostMessage(
          IntPtr hWnd,          // 信息发往的窗口的句柄
          Int32 Msg,            // 消息ID
          Int32 wParam,         // 参数1
          Int32 lParam          // 参数2
        );

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern Int32 SendMessage(
          IntPtr hWnd,        // 信息发往的窗口的句柄
          Int32 Msg,            // 消息ID
          Int32 wParam,         // 参数1
          ref CopyDataStruct lParam  // 参数2
        );

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public Int32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public String lpData;
        }
        [DllImport("psapi.dll")]
        public static extern int EmptyWorkingSet(IntPtr hwProc);
        [DllImport("user32.dll", EntryPoint = "SetCursorPos", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetCursorPos(Int32 x, Int32 y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "mouse_event", CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 mouse_event(UInt32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokPriv1Luid
        {
            public Int32 Count;
            public Int64 Luid;
            public Int32 Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean OpenProcessToken(IntPtr h, Int32 acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern Boolean LookupPrivilegeValue(String host, String name, ref Int64 pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean AdjustTokenPrivileges(IntPtr htok, Boolean disall,
            ref TokPriv1Luid newst, Int32 len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Boolean ExitWindowsEx(Int32 DoFlag, Int32 rea);

        public const Int32 SE_PRIVILEGE_ENABLED = 0x00000002;
        public const Int32 TOKEN_QUERY = 0x00000008;
        public const Int32 TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const String SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const Int32 EWX_LOGOFF = 0x00000000;
        public const Int32 EWX_SHUTDOWN = 0x00000001;
        public const Int32 EWX_REBOOT = 0x00000002;
        public const Int32 EWX_FORCE = 0x00000004;
        public const Int32 EWX_POWEROFF = 0x00000008;
        public const Int32 EWX_FORCEIFHUNG = 0x00000010;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean MessageBeep(Int32 uType);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(String pszPath, UInt32 dwFileAttributes, ref SHFILEINFO psfi, UInt32 cbSizeFileInfo, UInt32 uFlags);

        [DllImport("gdi32.dll")]
        public static extern Boolean BitBlt(IntPtr hdcDest, Int32 xDest, Int32 yDest, Int32
        wDest, Int32 hDest, IntPtr hdcSource, Int32 xSrc, Int32 ySrc, CopyPixelOperation rop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, Int32 nWidth, Int32 nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);
        [DllImport("user32.dll")]
        public static extern Boolean PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
        [DllImport("user32.dll")]
        public static extern Boolean ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #region 设备监视

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DEV_BROADCAST_HDR
        {
            /// <summary>
            /// 此结构的大小（以字节为单位）。
            /// 如果这是用户定义的事件，则此成员必须是此标头的大小以及 _DEV_BROADCAST_USERDEFINED 结构中可变长度数据的大小。
            /// </summary>
            public int Dbch_Size;
            /// <summary>
            /// 设备类型，它确定前三个成员后面的特定于事件的信息
            /// </summary>
            public DBCH_DEVICETYPE Dbch_DeviceType;
            /// <summary>
            /// 保留值；请勿使用
            /// </summary>
            public int Dbch_Reserved;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        public enum DBCH_DEVICETYPE : int
        {
            /// <summary>
            /// 设备类别。结构是 DEV_BROADCAST_DEVICEINTERFACE 结构
            /// </summary>
            DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,

            /// <summary>
            /// 文件系统句柄。 此结构是 一个DEV_BROADCAST_HANDLE 结构
            /// </summary>
            DBT_DEVTYP_HANDLE = 0x00000006,
            /// <summary>
            /// OEM 或 IHV 定义的设备类型。 此结构是 DEV_BROADCAST_OEM 结构。
            /// </summary>
            DBT_DEVTYP_OEM = 0x00000000,
            /// <summary>
            /// 将设备 (串行或并行) 。 此结构是 一个DEV_BROADCAST_PORT 结构。
            /// </summary>
            DBT_DEVTYP_PORT = 0x00000003,
            /// <summary>
            /// 逻辑卷。 此结构是 一个DEV_BROADCAST_VOLUME 结构
            /// </summary>
            DBT_DEVTYP_VOLUME = 0x00000002
        }

        #endregion

        #region 资源管理器

        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr ILCreateFromPathW(String pszPath);

        [DllImport("shell32.dll", ExactSpelling = true)]
        public static extern Int32 SHOpenFolderAndSelectItems(IntPtr pidlList, UInt32 cild, IntPtr children, UInt32 dwFlags);

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;        //文件的图标句柄
            public IntPtr iIcon;        //图标的系统索引号
            public UInt32 dwAttributes;   //文件的属性值 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String szDisplayName;    //文件的显示名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public String szTypeName;       //文件的类型名
        }

        public enum SHGFI
        {
            SHGFI_ICON = 0x100,
            SHGFI_LARGEICON = 0x0,
            SHGFI_USEFILEATTRIBUTES = 0x10
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmIsCompositionEnabled(ref Int32 enabledptr);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margin);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public Int32 Left;
            public Int32 Right;
            public Int32 Top;
            public Int32 Bottom;
        }

        [DllImport("user32", EntryPoint = "HideCaret")]
        public static extern Boolean HideCaret(IntPtr hWnd);

        public const UInt32 SWP_NOSIZE = 0x0001;//忽略 cx、cy, 保持大小
        public const UInt32 SWP_NOMOVE = 0x0002;//忽略 X、Y, 不改变位置
        public const UInt32 SWP_NOACTIVATE = 0x0010;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 x, Int32 y, Int32 Width, Int32 Height, UInt32 flags);

        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
        public static extern Int32 GetFileInfo(String pszPath, Int32 dwFileAttributes, ref FileInfomation psfi, Int32 cbFileInfo, Int32 uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct FileInfomation
        {
            public IntPtr hIcon;
            public Int32 iIcon;
            public Int32 dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public String szTypeName;
        }

        #region  get or set time

        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern Boolean SetLocalTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern void GetSystemTime(ref SystemTime lpSystemTime);

        [DllImport("Kernel32.dll")]
        public static extern Boolean SetSystemTime(ref SystemTime lpSystemTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            [MarshalAs(UnmanagedType.U2)]
            internal ushort Year; // 年

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Month; // 月

            [MarshalAs(UnmanagedType.U2)]
            internal ushort DayOfWeek; // 星期

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Day; // 日

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Hour; // 时

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Minute; // 分

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Second; // 秒

            [MarshalAs(UnmanagedType.U2)]
            internal ushort Milliseconds; // 毫秒
        }

        #endregion

        #region  get or set brigtness

        [DllImport("gdi32.dll")]
        public static extern Boolean GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);


        [DllImport("gdi32.dll")]
        public static extern Boolean SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        #endregion

        #region get process handle

        public delegate Boolean WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern Boolean EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern Boolean IsWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(uint dwErrCode);

        [DllImport("KERNEL32.DLL ")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);

        [DllImport("KERNEL32.DLL ")]
        public static extern Int32 Process32First(IntPtr handle, ref ProcessEntry32 pe);

        [DllImport("KERNEL32.DLL ")]
        public static extern Int32 Process32Next(IntPtr handle, ref ProcessEntry32 pe);

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessEntry32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public Int32 pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String szExeFile;
        }

        #endregion get process handle

        public enum FileAttributeFlags : Int32
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000
        }
        public enum GetFileInfoFlags : Int32
        {
            SHGFI_ICON = 0x000000100, //get icon
            SHGFI_DISPLAYNAME = 0x000000200, //get display name
            SHGFI_TYPENAME = 0x000000400, //get type name
            SHGFI_ATTRIBUTES = 0x000000800, //get attributes
            SHGFI_ICONLOCATION = 0x000001000, //get icon location
            SHGFI_EXETYPE = 0x000002000, //return exe type
            SHGFI_SYSICONINDEX = 0x000004000, //get system icon index
            SHGFI_LINKOVERLAY = 0x000008000,  //put a link overlay on icon
            SHGFI_SELECTED = 0x000010000, //show icon in selected state
            SHGFI_ATTR_SPECIFIED = 0x000020000, //get only specified attributes
            SHGFI_LARGEICON = 0x000000000, //get large icon
            SHGFI_SMALLICON = 0x000000001, //get small icon
            SHGFI_OPENICON = 0x000000002, //get open icon
            SHGFI_SHELLICONSIZE = 0x000000004, //get shell size icon
            SHGFI_PIDL = 0x000000008, //pszPath is a pidl
            SHGFI_USEFILEATTRIBUTES = 0x000000010, //use passed dwFileAttribute
            SHGFI_ADDOVERLAYS = 0x000000020, //apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040  //Get the index of the overlay
        }

        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 wndproc);

        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);

        public const Int32 GWL_STYLE = -16;
        public const Int32 GWL_EXSTYLE = -20;
        public const Int32 WS_DISABLED = 0x8000000;

        internal static void SetControlEnabled(Control c, Boolean enabled)
        {
            if (enabled)
            {
                _ = SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE));
            }
            else
            {
                _ = SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + GetWindowLong(c.Handle, GWL_STYLE));
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetFocus();

        internal static Control GetFocusedControl()
        {
            Control fc = null;

            // To get hold of the focused control:
            IntPtr handle = GetFocus();

            if (handle != IntPtr.Zero)
            {
                //focusedControl = Control.FromHandle(focusedHandle);
                fc = Control.FromChildHandle(handle);
            }
            return fc;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr SetActiveWindow(IntPtr hwnd);

        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(String lpPathName, Int32 iReadWrite);
               
        private const Int32 OF_READWRITE = 2;

        private const Int32 OF_SHARE_DENY_NONE = 0x40;

        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
               
        public static Boolean IsFileOccupied(String fileFullName)
        {
            IntPtr handle = _lopen(fileFullName, OF_READWRITE | OF_SHARE_DENY_NONE);
            CloseHandle(handle);

            return handle == HFILE_ERROR;
        }

    }
}
