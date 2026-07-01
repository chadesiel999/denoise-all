using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    #region PrintEnums
    public enum PrintOrient
    {
        Hor = 0,
        Ver = 1,
    };

    public enum PrintFunction
    {
        Screen = 0,
        File = 1,
    };

    public enum PrintColor
    {
        Standard = 0,
        BlackWhite = 1,
        Reverse = 2,
    };

    public enum PrintSaveArea
    {
        Screen = 0,
        Grid = 1,
    }
    #endregion

    internal class PrintModel : INotifyPropertyChanged
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

        private PrintFunction _Function = PrintFunction.Screen;
        public PrintFunction Function
        {
            get => _Function;
            set
            {
                if (_Function != value)
                {
                    _Function = value;
                    OnPropertyChanged();
                }
            }
        }

        //设置方向
        private PrintOrient _Orient = PrintOrient.Hor;
        public PrintOrient Orient
        {
            get => _Orient;
            set
            {
                if (_Orient != value)
                {
                    _Orient = value;
                    OnPropertyChanged();
                }
            }

        }

        private PrintColor _PrintColor = PrintColor.Standard;
        public PrintColor PrintColor
        {
            get => _PrintColor;
            set
            {
                if (_PrintColor != value)
                {
                    _PrintColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private PrintSaveArea _PrintSaveArea = PrintSaveArea.Screen;
        public PrintSaveArea PrintArea
        {
            get => _PrintSaveArea;
            set
            {
                if (_PrintSaveArea != value)
                {
                    _PrintSaveArea = value;
                    OnPropertyChanged();
                }
            }
        }

        public String PrntFileName
        {
            get;
            set;
        } = "";

        //默认路径
        private String _PrntFilePath = Constants.PRNT_DEF_PATH;
        public String PrntFilePath
        {
            get => _PrntFilePath;
            set
            {
                if (_PrntFilePath != value)
                {
                    _PrntFilePath = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
