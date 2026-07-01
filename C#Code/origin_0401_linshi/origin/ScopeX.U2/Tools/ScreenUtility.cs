using EventBus;
using ScopeX.Core.Tools;
using ScopeX.Touch;
using System;
using System.Drawing;
using System.Linq;
using System.Management;

namespace ScopeX.U2
{
    internal class ScreenUtility
    {
        private readonly ManagementScope _Scope = new ManagementScope("\\\\.\\ROOT\\WMI");
        private readonly ObjectQuery _ObjectQuery = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");
        private readonly SelectQuery _SelectQuery = new SelectQuery("WmiMonitorBrightnessMethods");
        private ManagementObjectSearcher _ObjectSearcher;
        private ManagementObjectSearcher _SelectSearcher;
        private Byte[] _BrightnessLevels;//缓存亮度级别

        internal static ScreenUtility Default = new ScreenUtility();

        private ScreenUtility()
        {
            Initialize();
        }

        private void Initialize()
        {
            _ObjectSearcher = new ManagementObjectSearcher(_Scope, _ObjectQuery);
            _SelectSearcher = new ManagementObjectSearcher(_Scope, _SelectQuery);
            _BrightnessLevels = GetBrightnessLevels();
        }

        #region 亮度调节

        /// <summary>
        /// 获取当前亮度
        /// </summary>
        public Int32 GetCurrentBrigtness()
        {
            try
            {
                var querycollection = _ObjectSearcher.Get();
                var lightvalue = 5; //屏幕亮度最小值是5
                foreach (var m in querycollection)
                {
                    lightvalue = Convert.ToInt32(m["CurrentBrightness"].ToString());
                }
                return lightvalue < 5 ? 5 : lightvalue;
            }
            catch (Exception ex)
            {
                //WeakTip.Default.Write("Brigtness setting", MsgTipId.GetBrigtnessFail);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                return 90;//屏幕亮度最小值是5
            }
        }

        /// <summary>
        /// 设置亮度值 默认最大
        /// </summary>
        /// <param name="targetBrightness"></param>
        public void SetBrigtness(Int32 targetBrightness = 100)
        {
            try
            {
                if (_BrightnessLevels.Length == 0) //"WmiMonitorBrightness" is not supported by the system
                {
                    //WeakTip.Default.Write("Brigtness setting", MsgTipId.SetBrigtnessFail);
                    return;
                }

                byte value = 100;
                if (targetBrightness <= _BrightnessLevels[^1])
                {
                    foreach (var item in _BrightnessLevels)
                    {
                        if (item >= targetBrightness)
                        {
                            value = item;
                            break;
                        }
                    }
                }

                using (var objectCollection = _SelectSearcher.Get())
                {
                    foreach (ManagementObject mObj in objectCollection)
                    {
                        mObj.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, value });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WeakTip.Default.Write("Brigtness setting", MsgTipId.SetBrigtnessFail);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Error));
            }
        }

        private Byte[] GetBrightnessLevels()
        {
            Byte[] levels = Array.Empty<Byte>();
            try
            {
                using ManagementObjectCollection m = _ObjectSearcher.Get();
                foreach (ManagementObject mObj in m)
                {
                    levels = (Byte[])mObj.GetPropertyValue("Level");
                    // Only work on the first object
                    break;
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Error));
            }
            return levels;
        }

        #endregion

        #region 对比度

        private NativeMethods.RAMP _Ramp = new() { Blue = new UInt16[256], Green = new UInt16[256], Red = new UInt16[256] };
        private IntPtr _Hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
        private Int32 _LastGama = -1;

        public Boolean SetContrast(Int32 gamma)
        {
            if (_LastGama == gamma)
            {
                return false;
            }
            _LastGama = gamma;

            var scalefactor = gamma / 100.0 * 255;
            for (var i = 1; i < 256; i++)
            {
                var value = (UInt16)Math.Clamp(i * scalefactor, UInt16.MinValue, UInt16.MaxValue);
                _Ramp.Red[i] = _Ramp.Green[i] = _Ramp.Blue[i] = value;
            }

            if (!NativeMethods.SetDeviceGammaRamp(Graphics.FromHwnd(IntPtr.Zero).GetHdc(), ref _Ramp))
            {
                //WeakTip.Default.Write("Brigtness setting", MsgTipId.SetContrastFail);
                return false;
            }
            return true;
        }

        public (Boolean, Int32) GetCurrentContrast()
        {
            if (NativeMethods.GetDeviceGammaRamp(Graphics.FromHwnd(IntPtr.Zero).GetHdc(), ref _Ramp))
            {
                var gamma = (CalColorGammaVal(_Ramp.Red) + CalColorGammaVal(_Ramp.Green) + CalColorGammaVal(_Ramp.Blue)) / 3;
                return (true, Math.Min(100, (Int32)(gamma * 100)));
            }
            else
            {
                //WeakTip.Default.Write("Brigtness setting", MsgTipId.GetContrastFail);
                return (false, 70);//最大对比度
            }
        }

        private Double CalColorGammaVal(UInt16[] line)
        {
            var max = 0;
            var min = line[0];
            var index = 1;
            for (var i = 1; i < line.Length; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                    index = i;
                }
            }
            var gamma = Math.Round((((Double)(max - min) / index) / 255), 2);
            return gamma;
        }

        #endregion

        #region 触摸锁定

        /// <summary>
        /// 尝试设置锁定屏幕
        /// </summary>
        /// <param name="touchable">ture ---> 支持触摸，false ---> 触摸锁定</param>
        /// <returns></returns>
        public void TrySetTouchable(Boolean touchable)
        {
            try
            {
                //if (touchable)
                //    WeakTip.Default.Write("ChkTouch", MsgTipId.EnableTouch);
                //else
                //    WeakTip.Default.Write("ChkTouch", MsgTipId.DisableTouch);

                //if (!TouchController.EnableTouch(touchable))
                //{
                //    WeakTip.Default.Write("ChkTouch", MsgTipId.AdministrtorAuthorityRequired);
                //}
            }
            catch (Exception ex)
            {
                WeakTip.Default.Write("ChkTouch", MsgTipId.AdministrtorAuthorityRequired);
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
            }

            // 键盘板的状态更改在DsoForm.cs 的InitOnLoad中注册的 TouchController.TouchChanged += TouchController_TouchChanged;事件来处理的，这里不再重复处理。
            // KeyboardLed.Default.SetTouchSceen(!TouchController.IsTouchable());
        }

        /// <summary>
        /// 尝试获取锁定是否锁定屏幕
        /// </summary>
        /// <returns>true ---> 支持触摸，false ---> 触摸锁定</returns>
        public Boolean TryGetTouchable()
        {
            try
            {
                return TouchController.IsTouchable();
            }
            catch (Exception ex)
            {
                //// 获取触摸屏触摸启用状态失败。 设置为默认启用
                //if (!ChkTouch.Checked)
                //{
                //    _ArgToCtrl = true;
                //    ChkTouch.Checked = true; //???为什么获取失败了 默认Checked
                //    _ArgToCtrl = false;
                //}
                WeakTip.Default.Write("ChkTouch", MsgTipId.AdministrtorAuthorityRequired);
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                return false;
            }
        }

        #endregion
    }
}
