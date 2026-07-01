using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using static NPOI.HSSF.Util.HSSFColor;

namespace ScopeX.Core
{
    internal class SystemCheckModel : INotifyPropertyChanged
    {
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 自检使能开关，可以去DisplayModel 关闭触摸使能TouchLock
        /// </summary>
        public Boolean _CheckEnable = false;
        public Boolean CheckEnable
        {
            get => _CheckEnable;
            set 
            {
                _CheckEnable = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// RunStop计数开关,按3次RunStop退出所有类型自测
        /// </summary>
        public UInt16 _ExitCount = 0;
        public UInt16 ExitCount
        {
            get => _ExitCount;
            set
            {
                _ExitCount = value;
                if (_ExitCount != 0)
                {
                    ResetValueOfExit();
                }
                if (ExitCount >= 3)
                {
                    OnPropertyChanged();
                }
            }
        }

        private async void ResetValueOfExit()
        {
            await Task.Delay(6000); // 等待6秒

            ExitCount = 0;
        }


        public CheckType _ScopeCheckType = CheckType.ScreenCheck;

        public CheckType ScopeCheckType
        {
            get => _ScopeCheckType;
            set
            {
                _ScopeCheckType = value;

            }
        }


        #region 屏幕检测模块

        public ScreenMaskColor _ScreenColorDisplay = ScreenMaskColor.Red;

        public ScreenMaskColor ScreenColorDisplay
        {
            get => _ScreenColorDisplay;
            set
            {
                _ScreenColorDisplay = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region 触摸检测模块

        public TouchTestTextColor _TextColorDisplay = TouchTestTextColor.Red;

        public TouchTestTextColor TextColorDisplay
        {
            get => _TextColorDisplay;
            set
            {
                _TextColorDisplay = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 按键板检测模块
        private Int32 _KeyCheckCode;
        public Int32 KeyCheckCode
        {
            get => _KeyCheckCode;
            set
            {
                _KeyCheckCode = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}
