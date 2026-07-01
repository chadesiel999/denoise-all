using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventBus;
using ScopeX.Controls.Common.Structs;
using ScopeX.UserControls;

namespace ScopeX.U2.BaseControl
{
    public class FlashBorderForm :FloatForm
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        public bool HasFlashChild = false;
        private LowLevelMouseProc hookProc;
        private IntPtr hookID = IntPtr.Zero;
        private Boolean IsFlashBorder = false;
        Color _BorderColorBack;

        public FlashBorderForm()
        {
            // EventBroker.Instance.GetEvent<FormShowDialogEventArgs>().Publish(this, new() { Current = this });
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0312)
            {
                // 按下了热键
                int id = m.WParam.ToInt32();
            }
        }
        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            IntPtr moduleHandle = currentModule.BaseAddress;
            return SetWindowsHookEx((int)HookType.WH_MOUSE_LL, proc, moduleHandle, 0);
        }
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && (int)wParam == 0x0201 || (int)wParam == 0x0246) // 鼠标左键按下
            {
                // 处理鼠标左键按下事件
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                Point mousepoint = new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y);
                Rectangle ret = new Rectangle(Location.X, Location.Y, Width, Height);
                if (!ret.Contains(mousepoint))
                {
                    if (this.IsHandleCreated && !IsFlashBorder && !HasFlashChild)
                    {
                        IsFlashBorder = true;
                        Task.Run(() =>
                        {
                            if (!this.IsDisposed)
                            {
                                (Program.Oscilloscope.View as DsoForm)?.Invoke(() =>
                                {
                                    HeadBackColor = _BorderColorBack.GetBrightnessColor(0.3);
                                    BorderBackColor = HeadBackColor;
                                });
                                Thread.Sleep(100);
                                (Program.Oscilloscope.View as DsoForm)?.Invoke(() =>
                                {
                                    HeadBackColor = _BorderColorBack;
                                    BorderBackColor = HeadBackColor;
                                });
                            }
                            Thread.Sleep(100);
                            IsFlashBorder = false;
                        });
                    }
                }
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookID);
                hookID = IntPtr.Zero;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            hookProc = HookCallback;
            hookID = SetHook(hookProc);
            _BorderColorBack = HeadBackColor;
        }
        private void Timer_tick(object sender, EventArgs e)
        {

        }
    }


}
