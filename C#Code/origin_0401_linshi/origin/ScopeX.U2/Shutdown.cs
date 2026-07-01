using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core;

namespace ScopeX.U2
{
    class Shutdown
    {
        private static Boolean EnableShutdownPrivilege(Int32 doFlag)
        {
            IntPtr htok = IntPtr.Zero;
            if (NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, ref htok))
            {
                NativeMethods.TokPriv1Luid tp;
                tp.Count = 1;
                tp.Luid = 0;
                tp.Attr = NativeMethods.SE_PRIVILEGE_ENABLED;
                if (NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_SHUTDOWN_NAME, ref tp.Luid))
                {
                    NativeMethods.AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                }
                NativeMethods.CloseHandle(htok);
            }

            return NativeMethods.ExitWindowsEx(doFlag, 0);
        }

        public static void PowerOff(DsoPrsnt prsnt)
        {
            try
            {
                //Process p = new();
                //p.StartInfo.CreateNoWindow = true;
                //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //p.StartInfo.FileName = "Shutdown.exe";
                //p.StartInfo.Arguments = " -s -t 0";
                //p.Start();
                if (EnableShutdownPrivilege(NativeMethods.EWX_FORCE | NativeMethods.EWX_POWEROFF | NativeMethods.EWX_SHUTDOWN) && prsnt != null)
                {
                    prsnt.HardwarePowerOff();
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Warn));
            }

        }

        public static void LogOff()
        {
            try
            {
                //Process p = new();
                //p.StartInfo.CreateNoWindow = true;
                //p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //p.StartInfo.FileName = "Shutdown.exe";
                //p.StartInfo.Arguments = " -s -t 0";
                //p.Start();
                EnableShutdownPrivilege(NativeMethods.EWX_LOGOFF);
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Warn));
            }
        }

        public static void Reboot(DsoPrsnt prsnt)
        {
            try
            {
				//Process p = new();
				//p.StartInfo.CreateNoWindow = true;
				//p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				//p.StartInfo.FileName = "Shutdown.exe";
				//p.StartInfo.Arguments = " -s -t 0";
				//p.Start();
				if (EnableShutdownPrivilege(NativeMethods.EWX_FORCE | NativeMethods.EWX_REBOOT))
				{
					prsnt.HardwarePowerOff();
				}
			}
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Warn));
            }

        }
    }
}
