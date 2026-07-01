using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Model
{
    internal class PageInfoModel :INotifyPropertyChanged
    {
        private Int64 _TotalNum;

        //总项数
        public Int64 TotalNum
        {
            get
            {
                return _TotalNum;
            }
            set
            {
                if (_TotalNum != value)
                {
                    _TotalNum = value;
                    OnPropertyChanged(nameof(TotalNum));
                }
            }
        }

        private Int64 _PageNum;

        //每页项数
        public Int64 PageNum
        {
            get
            {
                return _PageNum;
            }
            set
            {
                if (_PageNum != value)
                {
                    _PageNum = value;
                    OnPropertyChanged(nameof(PageNum));
                }
            }
        }

        private (Int64 StartIndex, Int64 EndIndex) _ShowIndexs = (-1, -1);

        /// <summary>
        /// 当前页显示项索引
        /// </summary>
        public (Int64 StartIndex, Int64 EndIndex) ShowIndexs
        {
            get
            {
                return _ShowIndexs;
            }
            set
            {
                if (_ShowIndexs != value)
                {
                    _ShowIndexs = value;
                    OnPropertyChanged(nameof(ShowIndexs));
                }
            }
        }

        private Int64 _TotalPageNum;
        //总页数
        public Int64 TotalPageNum
        {
            get
            {
                return _TotalPageNum;
            }
            set
            {
                if (_TotalPageNum != value)
                {
                    _TotalPageNum = value;
                    OnPropertyChanged(nameof(TotalPageNum));
                }
            }
        }

        private Int64 _CurrentPageNum;
        //当前页数
        public Int64 CurrentPageNum
        {
            get
            {
                return _CurrentPageNum;
            }
            set
            {
                if (_CurrentPageNum != value)
                {
                    _CurrentPageNum = value;
                    OnPropertyChanged(nameof(CurrentPageNum));
                }
            }
        }
        private Boolean _CanModifyCurrentPage = false;

        /// <summary>
        /// 是否可以执行第一页指令
        /// </summary>
        public Boolean CanModifyCurrentPage
        {
            get
            {
                return _CanModifyCurrentPage;
            }
            set
            {
                if (_CanModifyCurrentPage != value)
                {
                    _CanModifyCurrentPage = value;
                    OnPropertyChanged(nameof(CanModifyCurrentPage));
                }
            }
        }


        private Boolean _CanFirst = false;

        /// <summary>
        /// 是否可以执行第一页指令
        /// </summary>
        public Boolean CanFirst
        {
            get
            {
                return _CanFirst;
            }
            set
            {
                if (_CanFirst != value)
                {
                    _CanFirst = value;
                    OnPropertyChanged(nameof(CanFirst));
                }
            }
        }

        private Boolean _CanLast = false;

        /// <summary>
        /// 是否可以执行最后一页指令
        /// </summary>
        public Boolean CanLast
        {
            get
            {
                return _CanLast;
            }
            set
            {
                if (_CanLast != value)
                {
                    _CanLast = value;
                    OnPropertyChanged(nameof(CanLast));
                }
            }
        }

        private Boolean _CanPrevious = false;
        /// <summary>
        /// 是否可以执行上一页指令
        /// </summary>
        public Boolean CanPrevious
        {
            get
            {
                return _CanPrevious;
            }
            set
            {
                if (_CanPrevious != value)
                {
                    _CanPrevious = value;
                    OnPropertyChanged(nameof(CanPrevious));
                }
            }
        }

        private Boolean _CanNext = false;

        /// <summary>
        /// 是否可以执行下一页指令
        /// </summary>
        public Boolean CanNext
        {
            get
            {
                return _CanNext;
            }
            set
            {
                if (_CanNext != value)
                {
                    _CanNext = value;
                    OnPropertyChanged(nameof(CanNext));
                }
            }
        }

        

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
    }
}
