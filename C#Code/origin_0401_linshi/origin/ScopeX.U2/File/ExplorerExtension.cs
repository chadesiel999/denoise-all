using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ScopeX.U2
{
    /// <summary>
    /// 资源管理器扩展类
    /// </summary>
    public static class ExplorerExtension
    {
        /// <summary>
        /// 打开路径并定位文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        public static void ExplorerFile(String filePath)
        {
            if (!System.IO.File.Exists(filePath) && !Directory.Exists(filePath))
            {
                return;
            }

            if (Directory.Exists(filePath))
            {
                Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");
            }
            else
            {
                IntPtr pidlList = NativeMethods.ILCreateFromPathW(filePath);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0);
                    }
                    catch { }
                    finally
                    {
                        NativeMethods.ILFree(pidlList);
                    }
                }
            }
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="Path"></param>
        public static void ExplorerDic(String Path)
        {
            if (!System.IO.File.Exists(Path) && !Directory.Exists(Path))
            {
                return;
            }

            if (Directory.Exists(Path))
            {
                Process.Start(@"explorer.exe", Path);
            }
            else
            {
                IntPtr pidlList = NativeMethods.ILCreateFromPathW(Path);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        Marshal.ThrowExceptionForHR(NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                    }
                    finally
                    {
                        NativeMethods.ILFree(pidlList);
                    }
                }
            }
        }

        /// <summary>
        /// 获取最近保存的文件
        /// </summary>
        /// <param name="Dir"></param>
        /// <param name="Postfix"></param>
        /// <returns></returns>
        public static String GetLatestFile(String Dir, String Postfix = ".set")
        {
            var list = new List<FileTimeInfo>();
            var dicInfo = new DirectoryInfo(Dir);
            foreach (var file in dicInfo.GetFiles())
            {
                if (String.IsNullOrEmpty(Postfix))
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        LastWriteTime = file.LastWriteTime,
                    });
                }
                else if (file.Extension.ToUpper() == Postfix.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        LastWriteTime = file.LastWriteTime,
                    });
                }
            }
            var f = from x in list
                    orderby x.LastWriteTime
                    select x;
            if (f.Count() == 0)
            {
                return String.Empty;
            }
            return f.LastOrDefault().FileName;
        }

        /// <summary>
        /// 检查磁盘空间是否足够
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="requiredFreeSpace">需要空间</param>
        /// <returns></returns>
        public static Boolean IsDiskSpaceSufficient(String filePath, Double requiredFreeSpace)
        {
            try
            {
                // 获取文件所在磁盘根目录的路径
                var driveRoot = Path.GetPathRoot(filePath);

                // 获取磁盘信息
                DriveInfo driveInfo = new DriveInfo(driveRoot);

                // 检查磁盘剩余空间是否足够
                if (driveInfo.AvailableFreeSpace >= requiredFreeSpace)
                {
                    return true;
                }
                else
                {
                    WeakTip.Default.Write("Save Waveform", MsgTipId.NoMoreDiskSpace);
                    return false;
                }
            }
            catch (Exception e)
            {
                WeakTip.Default.Write("Save Waveform", MsgTipId.CheckDiskSpaceError);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Debug));
                return false;
            }
        }
    }
    internal class FileTimeInfo
    {
        public String FileName { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
