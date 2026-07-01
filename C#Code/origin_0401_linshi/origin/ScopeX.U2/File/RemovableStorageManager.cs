using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System;
using System.IO;
using System.Linq;
using System.Management;

namespace ScopeX.U2
{
    public sealed class RemovableStorageManager : IDisposable
    {
        private FilePrsnt Presenter { get; set; }
        //private String PicPath => $"{Program.Oscilloscope.GetProductModel()}\\Picture";
        //private String WfmPath => $"{Program.Oscilloscope.GetProductModel()}\\WaveForm";
        private String PicPath => $"ScopeFile\\Picture";//????????
        private String WfmPath => $"ScopeFile\\WaveForm";

        /// <summary>
        /// Gets or sets the Default.
        /// </summary>
        public static RemovableStorageManager Default
        {
            get;
            internal set;
        }

        public RemovableStorageManager(FilePrsnt prsnt)
        {
            Presenter = prsnt;
            AddUsbEventWatcher(UsbEventHandler, UsbEventHandler, new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// USB插入事件监视
        /// </summary>
        private ManagementEventWatcher OnInsertWatcher = null;

        /// <summary>
        /// USB拔出事件监视
        /// </summary>
        private ManagementEventWatcher OnRemoveWatcher = null;

        /// <summary>
        /// 添加USB事件监视器
        /// </summary>
        /// <param name="usbInsertHandler">USB插入事件处理器</param>
        /// <param name="usbRemoveHandler">USB拔出事件处理器</param>
        /// <param name="timeInterval">发送通知允许的滞后时间</param>
        private Boolean AddUsbEventWatcher(EventArrivedEventHandler usbInsertHandler, EventArrivedEventHandler usbRemoveHandler, TimeSpan timeInterval)
        {
            try
            {
                ManagementScope scope = new("root\\CIMV2");
                scope.Options.EnablePrivileges = true;

                // USB插入监视
                if (usbInsertHandler != null)
                {
                    WqlEventQuery insertquery = new("__InstanceCreationEvent",
                        timeInterval,
                        "TargetInstance isa 'Win32_DiskDrive'");
                    OnInsertWatcher = new ManagementEventWatcher(scope, insertquery);
                    OnInsertWatcher.EventArrived += usbInsertHandler;
                    OnInsertWatcher.Start();
                }

                // USB拔出监视
                if (usbRemoveHandler != null)
                {
                    WqlEventQuery removequery = new("__InstanceDeletionEvent",
                        timeInterval,
                        "TargetInstance isa 'Win32_DiskDrive'");
                    OnRemoveWatcher = new ManagementEventWatcher(scope, removequery);
                    OnRemoveWatcher.EventArrived += usbRemoveHandler;
                    OnRemoveWatcher.Start();
                }
                return true;
            }
            catch (Exception e)
            {
                RemoveUsbEventWatcher();
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
            return false;
        }

        /// <summary>
        /// 移去USB事件监视器
        /// </summary> 
        private void RemoveUsbEventWatcher()
        {
            try
            {
                if (OnInsertWatcher != null)
                {
                    OnInsertWatcher.Stop();
                    OnInsertWatcher = null;
                }

                if (OnRemoveWatcher != null)
                {
                    OnRemoveWatcher.Stop();
                    OnRemoveWatcher = null;
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void UsbEventHandler(Object sender, EventArrivedEventArgs e)
        {
            var watcher = sender as ManagementEventWatcher;
            watcher.Stop();//等待业务逻辑处理完 再开启监听事件
            DiskDevice diskdevice = GetDiskDevice(e);
            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")//插入USB
            {
                if (diskdevice != null)
                {
                    DriveInfo usbdisk = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable).LastOrDefault();
                    if (usbdisk == null)
                    {
                        WeakTip.Default.Write("Removable Media", MsgTipId.UnknownRemovableStorage);
                        watcher.Start();
                        return;
                    }
                    diskdevice.DiskName = usbdisk.VolumeLabel;
                    diskdevice.RootDirectory = usbdisk.RootDirectory.FullName;

                    (Program.Oscilloscope.View as DsoForm).Invoke(() =>
                    {
                        if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.RemovableStorageDiscovered, MessageType.Asking))
                        {
                            var path = Path.Combine(diskdevice.RootDirectory, WfmPath);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            Presenter.WfmPath = path;

                            path = Path.Combine(diskdevice.RootDirectory, PicPath);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            Presenter.PicPath = path;
                        }
                    });
                }
                else
                {
                    WeakTip.Default.Write("Removable Media", MsgTipId.UnknownRemovableStorage);
                }
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")//拔出USB
            {
                //<Remark>作者：彭博 创建日期：2024/2/26 14:14:00 创建原因：主动关闭提示窗口，保证只有一个提示窗口</Remark>
                (Program.Oscilloscope.View as DsoForm).CloseFormsManagerMessageForm();
                WeakTip.Default.Write("Removable Media", MsgTipId.RemovableStorageRemoved);
                if (!Directory.Exists(Presenter.WfmPath))
                {
                    Presenter.WfmPath = Constants.WFM_DEF_PATH;
                }
                if (!Directory.Exists(Presenter.PicPath))
                {
                    Presenter.PicPath = Constants.PIC_DEF_PATH;
                }
            }

            watcher.Start();
        }

        private static DiskDevice GetDiskDevice(EventArrivedEventArgs e)
        {
            try
            {
                if (e.NewEvent["TargetInstance"] is ManagementBaseObject mbo && (mbo["MediaType"] as String) == "Removable Media")
                {
                    String mediatype = mbo["MediaType"] as String;
                    String description = mbo["Description"] as String;
                    return new DiskDevice { DeviceType = mediatype, Description = description };
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        //private Boolean _DisposedValue = false;

        public void Dispose()
        {
            //if (!_DisposedValue)
            //{
            RemoveUsbEventWatcher();
            //_DisposedValue = true;
            //}
        }
    }

    internal class DiskDevice
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public String DeviceType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 磁盘名
        /// </summary>
        public String DiskName { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public String RootDirectory { get; set; }
    }
}
