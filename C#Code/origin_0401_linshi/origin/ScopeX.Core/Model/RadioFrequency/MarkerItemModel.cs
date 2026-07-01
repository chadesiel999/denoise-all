using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Model.RadioFrequency
{
    internal class MarkerItemModel :INotifyPropertyChanged
    {

        public Boolean ManualMarkerActive
        {
            get => MaunalMarker.Active;
            set
            {
                MaunalMarker.Active = value;
                OnPropertyChanged();
            }
        }

        public Boolean AtuoMarkerActive
        {
            get => AutoMarker.Active;
            set
            {
                if(AutoMarker.Active != value)
                {
                    AutoMarker.Active = value;
                    MarkerResultsTableEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _MarkerResultsTableEnable = false;
        public Boolean MarkerResultsTableEnable
        {
            get => _MarkerResultsTableEnable;
            set
            {
                if(_MarkerResultsTableEnable != value)
                {
                    _MarkerResultsTableEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private MarkerReadMode _ReadMode = MarkerReadMode.Absolute;
        public MarkerReadMode ReadMode
        {
            get => _ReadMode;
            set
            {
                _ReadMode = value;
                OnPropertyChanged();
            }
        }

        private SortOption _SortMode = SortOption.Frequency;
        public SortOption SortMode
        {
            get => _SortMode;
            set
            {
                _SortMode = value;
                OnPropertyChanged();
            }
        }

        private Int32 _FocusId = 0;
        /// <summary>
        /// 光标焦点Id
        /// </summary>
        public Int32 FocusId
        {
            get => _FocusId;
            set
            {
                if (_FocusId != value)
                {
                    _FocusId = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _Id = ChannelId.MAKER1;
        /// <summary>
        /// Id
        /// </summary>
        public ChannelId Id
        {
            get => _Id;
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }


        private ChannelId _Source = ChannelId.M1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    MaunalMarker.Source = value;
                    AutoMarker.Source = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 阈值
        /// </summary>
        public Double RFThreshold
        {
            get { return AutoMarker.RFThreshold; }
            set
            {
                AutoMarker.RFThreshold = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 振幅
        /// </summary>
        public Double RFExcursion
        {
            get { return AutoMarker.RFExcursion; }
            set
            {
                AutoMarker.RFExcursion = value;
                OnPropertyChanged();
            }
        }


        public Int32 MaxMarkerCount
        {
            get { return AutoMarker.MaxMarkerCount; }
            set
            {
                AutoMarker.MaxMarkerCount = value;
                OnPropertyChanged();
            }
        }
        public String Unit
        {
            get
            {
                return DsoModel.Default.GetChannel(_Source).Conditioning.Unit;
            }
        }
        public Prefix Prefix
        {
            get
            {
                return DsoModel.Default.GetChannel(_Source).Conditioning.Prefix;
            }
        }

        //Horizontal Cursor For Amplitude Measurment
        public RFMarkerBarModel MaunalMarker
        {
            get;
        }
        public RFMarkerBarModel AutoMarker
        {
            get;
        }

        public MarkerItemModel(ChannelId source, ChannelId id = ChannelId.MAKER1)
        {
            MaunalMarker = new(this, Constants.MAX_HCURSOR_IDX, Constants.MIN_HCURSOR_IDX);
            AutoMarker = new(this, Constants.MAX_HCURSOR_IDX, Constants.MIN_HCURSOR_IDX);
            Source = source;
            Id = id;
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                MaunalMarker.PropertyChanged += value;
                AutoMarker.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                MaunalMarker.PropertyChanged -= value;
                AutoMarker.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
