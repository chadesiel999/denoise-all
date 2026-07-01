using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using ScopeX.ComModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ScopeX.Core
{
    internal class LissajousModel : INotifyPropertyChanged
    {
        //private readonly ConcurrentDictionary<String, (Boolean Active, ChannelId X, ChannelId Y)> _LissajousList = new();

        private static Boolean IsSourceValid(ChannelId id)
        {
            //return id switch
            //{
            //    ChannelId.C1 or ChannelId.C2 or ChannelId.C3 or ChannelId.C4 => true,
            //    ChannelId.R1 or ChannelId.R2 or ChannelId.R3 or ChannelId.R4 => true,
            //    _ => false,
            //};
            return true;
        }

        private Int32 _ID = 0;
        public Int32 ID
        {
            get => _ID;
            set
            {
                _ID = value;
            }
        }

        private ChannelId _LissaSrcX = ChannelId.C1;
        public ChannelId SourceX
        {
            get => _LissaSrcX;
            set
            {
                if (IsSourceValid(value) && _LissaSrcX != value)
                {
                    _LissaSrcX = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _LissaSrcY = ChannelId.C2;
        public ChannelId SourceY
        {
            get => _LissaSrcY;
            set
            {
                if (IsSourceValid(value) && _LissaSrcY != value)
                {
                    _LissaSrcY = value;
                    OnPropertyChanged();
                }
            }
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
                    if (value == false)
                    {
                        _CursorActive = value;
                    }
                    else
                    {
                        if (_CursorType == LissajousCursorType.Wave)
                        {
                            DsoModel.Default.Cursors.Type = ComModel.CursorType.Vertical;
                            DsoModel.Default.Cursors.Active = _CursorActive;
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _CursorActive = false;
        public Boolean CursorActive
        {
            get => _CursorActive;
            set
            {
                if (_CursorActive == value)
                {
                    return;
                }
                _CursorActive = value;
                if (value)
                {
                    if (_CursorType == LissajousCursorType.Wave)
                    {
                        if (DsoModel.Default.TryGetChannel(DsoModel.Default.Cursors.VCursor.Source, out var am))
                        {
                            if (am.Active == false)
                            {
                                foreach (var id in ChannelIdExt.GetAnalogs())
                                {
                                    am = null;
                                    if (DsoModel.Default.TryGetChannel(id, out am))
                                    {
                                        if (am.Active)
                                        {
                                            DsoModel.Default.Cursors.VCursor.Source = DsoModel.Default.Cursors.HCursor.Source = id;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        DsoModel.Default.Cursors.Type = ComModel.CursorType.Vertical;
                        DsoModel.Default.Cursors.Active = true;
                    }
                }
                OnPropertyChanged();
            }
        }

        private LissajousCursorType _CursorType = LissajousCursorType.Wave;
        public LissajousCursorType CursorType
        {
            get => _CursorType;
            set
            {
                if (_CursorType != value)
                {
                    if (value == LissajousCursorType.Wave)
                    {
                        DsoModel.Default.Cursors.Type = ComModel.CursorType.Vertical;
                        if (_CursorActive == true)
                        {
                            DsoModel.Default.Cursors.Active = _CursorActive;
                        }
                    }
                    _CursorType = value;
                    OnPropertyChanged();
                }
            }
        }

        private LissajousDataType _DataType = LissajousDataType.Rectangle;
        public LissajousDataType DataType
        {
            get => _DataType;
            set
            {
                if (_DataType != value)
                {
                    _DataType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int64? _WindowId;
        public Int64? WindowId
        {
            get => _WindowId;
            set
            {
                if (_WindowId != value)
                {
                    _WindowId = value;
                    OnPropertyChanged();
                }
            }
        }

        private (Double Radius, Double Angle) _PosA = (0D, 0D);
        /// <summary>
        /// 单位 (V,°)
        /// </summary>
        public (Double Radius, Double Angle) PosA
        {
            get => _PosA;
            set => _PosA = value;
        }

        private (Double Radius, Double Angle) _PosB = (0D, 0D);
        /// <summary>
        /// 单位 (V,°)
        /// </summary>
        public (Double Radius, Double Angle) PosB
        {
            get => _PosB;
            set => _PosB = value;
        }

        public CursorBarModel HCursor
        {
            get;
        }

        //Vertical Cursor For Time and Amplitude Measurement
        public CursorBarModel VCursor
        {
            get;
        }

        public LissajousModel()
        {
            VCursor = new(Constants.MAX_HCURSOR_IDX, Constants.MIN_HCURSOR_IDX, OnPropertyChanged)
            {
                PosFormat = CursorPosFormat.Axis,
            };
            HCursor = new(Constants.MAX_VCURSOR_IDX, Constants.MIN_VCURSOR_IDX, OnPropertyChanged)
            {
                PosFormat = CursorPosFormat.Axis,
            };
            VCursor[0] = Constants.IDX_PER_XDIV * 2;
            VCursor[1] = Constants.IDX_PER_XDIV * 8;

            HCursor[0] = Constants.IDX_PER_YDIV * 2;
            HCursor[1] = 0 - Constants.IDX_PER_YDIV * 2;

            VCursor.Source = _LissaSrcX;
            HCursor.Source = _LissaSrcY;
        }

        //public Boolean TryAddLissa(String key) => _LissajousList.TryAdd(key, (Active, SourceX, SourceY));

        //public Boolean TryRemoveLissa(String key) => _LissajousList.TryRemove(key, out _);

        //public Boolean TryGetLissa(String key)
        //{
        //    if (_LissajousList.TryGetValue(key, out var value))
        //    {
        //        SourceX = value.X;
        //        SourceY = value.Y;
        //        return true;
        //    }
        //    return false;
        //}

        //public Int32 LissaLength => _LissajousList.Count;

        //public void ClearLissa() => _LissajousList.Clear();


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
    }
}
