using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class FileModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
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
        #endregion

        private ChannelId _WfmSource = ChannelId.C1;
        public ChannelId WfmSource
        {
            get => _WfmSource;
            set
            {
                if (_WfmSource != value)
                {
                    _WfmSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private WfmFormat _WfmFormat = WfmFormat.Binary;
        public WfmFormat WfmFormat
        {
            get => _WfmFormat;
            set
            {
                if (_WfmFormat != value)
                {
                    _WfmFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        private WfmFormat _LongWfmFormat = WfmFormat.Binary;
        public WfmFormat LongWfmFormat
        {
            get => _LongWfmFormat;
            set
            {
                if (_LongWfmFormat != value)
                {
                    _LongWfmFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        private PicFormat _PicFormat = PicFormat.Png;
        public PicFormat PicFormat
        {
            get => _PicFormat;
            set
            {
                if (_PicFormat != value)
                {
                    _PicFormat = value;
                    OnPropertyChanged();
                }
            }
        }

        private PicArea _PicRegion = PicArea.Application;
        public PicArea PicRegion
        {
            get => _PicRegion;
            set
            {
                if (_PicRegion != value)
                {
                    _PicRegion = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _IfAppendDatetime = false;
        public Boolean IfAppendDatetime
        {
            get => _IfAppendDatetime;
            set
            {
                if (_IfAppendDatetime != value)
                {
                    _IfAppendDatetime = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _FileName = "Uni-t000";
        public String FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _LongStroageFileName = "Uni-t000";
        public String LongStroageFileName
        {
            get => _LongStroageFileName;
            set
            {
                if (_LongStroageFileName != value)
                {
                    _LongStroageFileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public String DefaultPrefixName
        {
            get;
            init;
        } = "Uni-t";


        private String _WfmPath = Constants.WFM_DEF_PATH;
        public String WfmPath
        {
            get => _WfmPath;
            set
            {
                if (_WfmPath != value)
                {
                    _WfmPath = value;
                    OnPropertyChanged();
                }

                //if (!AccessToFolder(_WfmPath))
                //{
                //    WeakTip.Default.Write("WfmPath", MsgTipId.AccessToFolderFailed, false, "", 2);
                //    _WfmPath = Constants.WFM_DEF_PATH;
                //    OnPropertyChanged();
                //}
            }
        }

        private String _PicPath = Constants.PIC_DEF_PATH;
        public String PicPath
        {
            get => _PicPath;
            set
            {
                if (_PicPath != value)
                {
                    _PicPath = value;
                    OnPropertyChanged();
                }

                //if (!AccessToFolder(_PicPath))
                //{
                //    WeakTip.Default.Write("PicPath", MsgTipId.AccessToFolderFailed, false, "", 2);
                //    _PicPath = Constants.PIC_DEF_PATH;
                //    OnPropertyChanged();
                //}
            }
        }

        private String _SettingLoadPath = Constants.SET_DEF_PATH;
        public String SettingLoadFullPath
        {
            get => _SettingLoadPath;
            set
            {
                if (_SettingLoadPath != value)
                {
                    _SettingLoadPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _SettingSavePath = Constants.SET_DEF_PATH;
        public String SettingSavePath
        {
            get => _SettingSavePath;
            set
            {
                if (_SettingSavePath != value)
                {
                    _SettingSavePath = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _IsDefaultSetting = false;
        public Boolean IsDefaultSetting
        {
            get => _IsDefaultSetting;
            set
            {
                if (_IsDefaultSetting != value)
                {
                    _IsDefaultSetting = value;
                    OnPropertyChanged();
                }
            }
        }

        private PicColor _PicColor = PicColor.Standard;
        public PicColor PicColor
        {
            get => _PicColor;
            set
            {
                if (_PicColor != value)
                {
                    _PicColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private TxtFormat _WfmTxtFormat = TxtFormat.UTF8;
        public TxtFormat WfmTxtFormat
        {
            get => _WfmTxtFormat;
            set
            {
                if (_WfmTxtFormat != value)
                {
                    _WfmTxtFormat = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean AccessToFolder(String Path)
        {
            try
            {
                var result = new DirectoryInfo(Path)
                .GetFiles($"*{_WfmTxtFormat.GetAlias()}", SearchOption.TopDirectoryOnly)
                .Where(x => Regex.IsMatch(x.Name, $"^{DefaultPrefixName}{""}[0-9]{"{3}"}{_WfmTxtFormat.GetAlias()}$", RegexOptions.IgnoreCase));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
