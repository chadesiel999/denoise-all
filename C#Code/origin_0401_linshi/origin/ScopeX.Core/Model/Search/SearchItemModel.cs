using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class SearchItemModel : INotifyPropertyChanged
    {
        public SearchItemModel(String name)
        {
            _Name = name;
            _Id = (Int32)_Name.GetIdNumByString();
            _SearchTypeModel = GetSearchModel(_Type);
        }

        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged($"{Name}:{nameof(Active)}");
                }
            }
        }

        private Boolean _Focused = false;
        public Boolean Focused
        {
            get => _Focused;
            set
            {
                if (_Focused != value)
                {
                    _Focused = value;
                    OnPropertyChanged($"{Name}:{nameof(Focused)}");
                }
            }
        }

        private Boolean _Visiable = true;
        public Boolean Visible
        {
            get => _Visiable;
            set
            {
                if (_Visiable != value)
                {
                    _Visiable = value;
                    OnPropertyChanged($"{Name}:{nameof(Visible)}");
                }
            }
        }

        public ChannelId Source
        {
            get { return _SearchTypeModel.Source; }
            set
            {
                _SearchTypeModel.Source = value;
                OnPropertyChanged($"{Name}:{nameof(Source)}");
            }
        }

        private Boolean _OnceSearch = false;

        public Boolean OnceSearch
        {
            get => _OnceSearch;
            set
            {
                if (_OnceSearch != value)
                {
                    _OnceSearch = value;
                }
            }
        }

        private Boolean _EventEnable = false;
        public Boolean EventEnable
        {
            get => _EventEnable;
            set
            {
                if (_EventEnable != value)
                {
                    _EventEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _Name;
        public String Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    Id = (Int32)_Name.GetIdNumByString();
                    OnPropertyChanged();
                }
            }
        }

        private Color _DrawColor = Color.White;
        public Color DrawColor
        {
            get => _DrawColor;
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _Id = -1;

        public Int32 Id
        {
            get
            {
                return _Id;
            }
            private set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        public Int32 ResultCount
        {
            get => _SearchTypeModel.ResultCount;
            set
            {
                if (_SearchTypeModel.ResultCount != value)
                {
                    _SearchTypeModel.ResultCount = value;
                    OnPropertyChanged($"{Name}:{nameof(ResultCount)}");
                }
            }
        }

        private ISearchTypeModel _SearchTypeModel;
        public ISearchTypeModel SearchTypeModel
        {
            get => _SearchTypeModel;
            set
            {
                if (_SearchTypeModel != value)
                {
                    _SearchTypeModel = value;
                    OnPropertyChanged();
                }
            }
        }


        private SearchType _Type = SearchType.Edge;
        public SearchType Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    _SearchTypeModel = GetSearchModel(_Type);
                    OnPropertyChanged($"{Name}:{nameof(Type)}");
                }
            }
        }

        private (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) _SearchResultTmbInfo;
        public (Double PositionByus, Double PosIndex, Double Scale, Double PosIdxPerDiv) SearchResultTmbInfo
        {
            get => _SearchResultTmbInfo;
            set => _SearchResultTmbInfo = value;
        }

        /// <summary>
        /// 标识是否为最新数据
        /// </summary>
        private Boolean _IsNewResult;
        public Boolean IsNewResult
        {
            get => _IsNewResult;
            set => _IsNewResult = value;
        }


        private Prefix _ResultPrefix;
        public Prefix ResultPrefix
        {
            get => _ResultPrefix;
            set => _ResultPrefix = value;
        }

        private String _ResultTmbUnitName;
        public String ResultTmbUnitName
        {
            get => _ResultTmbUnitName;
            set => _ResultTmbUnitName = value;
        }

        private Double _ZoomRatio;
        public Double ZoomRatio
        {
            get => _ZoomRatio;
            set => _ZoomRatio = value;
        }

        private Double _Start;
        public Double Start
        {
            get => _Start;
            set => _Start = value;
        }

        public void SearchEventHandle()
        {

        }

        public void ChangeType(SearchType type)
        {
            _Type = type;
            _SearchTypeModel = GetSearchModel(_Type);
        }

        private ISearchTypeModel GetSearchModel(SearchType type)
        {
            switch (type)
            {
                case SearchType.Edge:
                    return new SearchEdgeModel();
                case SearchType.Pulse:
                    return new SearchPulseModel();
                case SearchType.Timeout:
                    return new SearchTimeoutModel();
                case SearchType.Runt:
                    return new SearchRuntModel();
                case SearchType.Window:
                    return new SearchWindowModel();
                case SearchType.Transition:
                    return new SearchTransitionModel();
                case SearchType.Pattern:
                    break;
                case SearchType.SetupHold:
                    return new SearchSetupHoldModel();
                case SearchType.Auto:
                    break;
                default:
                    break;
            }
            return new SearchEdgeModel();
        }

        public List<String> KeyInfos
        {
            get
            {
                return _SearchTypeModel.GetKeyInfos();
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
