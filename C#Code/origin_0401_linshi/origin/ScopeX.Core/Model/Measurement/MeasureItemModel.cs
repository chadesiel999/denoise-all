// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;

    internal class MeasureItemModel
    {
        public MeasureItemModel(ChannelId id, String name, PropertyChangedEventHandler? propertyChanged)
        {
            Id = id;
            RefLevel = new(this, propertyChanged);

            _Name = name;
            _PropertyChanged = propertyChanged;
        }

        public MeasureItemModel(String name, ChannelId source)
        {
            Id = ChannelIdExt.MaxPChId + (Int32)IdFactory.NextId;
            RefLevel = new(this);

            _Name = name;
            _Source = source;
        }

        internal Func<Boolean, ChannelId, MeasItemFigureType, Boolean>? OpenOrCloseFigure { get; set; } = null;

        public ChannelType Type
        {
            get;
        } = ChannelType.Parameter;

        private MeasureType _MeasureType = MeasureType.Single;

        public MeasureType MeasureType
        {
            get => _MeasureType;
            set
            {
                if (_MeasureType != value)
                {
                    _MeasureType = value;
                    if (_MeasureType == MeasureType.Single)
                    {
                        if (Source.IsMeasure())
                        {
                            Source = ChannelId.C1;
                        }
                        if (Source2nd.IsMeasure())
                        {
                            Source2nd = ChannelId.C2;
                        }
                    }
                    if (_MeasureType == MeasureType.Composite)
                    {
                        if (!Source.IsMeasure())
                        {
                            Source = Id == ChannelId.P1 ? ChannelId.P2 : ChannelId.P1;
                        }
                        if (!Source2nd.IsMeasure())
                        {
                            Source2nd = Id == ChannelId.P2 ? ChannelId.P3 : ChannelId.P2;
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        public ChannelId Id
        {
            get;
        }

        private String _Name;
        private String _CompositeName => $"{Source} {Operation.GetDescription()} {Source2nd}";

        public String Name
        {
            get => (MeasureType == MeasureType.Single ? _Name : _CompositeName);
            set
            {
                if (_Name != value)
                {
                    if (!MeasureProc.OperationDescriptions.Any(x => value.Contains(x)))
                    {
                        if (MeasureType != MeasureType.Single)
                        {
                            MeasureType = MeasureType.Single;
                        }
                        _Name = value;
                    }
                    else
                    {
                        _Name = nameof(MeasParameter.Pk2Pk); ;//保存参数运算后，重新启动需要重新保存为参数测量的项名称
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Color? _DrawColor = null;
        public Color DrawColor
        {
            get => _DrawColor ?? (DsoModel.Default.TryGetChannel(Source, out var ch) ? ch.DrawColor : Color.Gray);
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
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
                    OnPropertyChanged();
                }
            }
        }


        private Boolean _Visiable = false;
        public Boolean Visible
        {
            get => _Visiable;
            set
            {
                if (_Visiable != value)
                {
                    _Visiable = value;
                    OnPropertyChanged();
                }
            }
        }

        //private MeasureGate _Strobe = MeasureGate.Screen;
        //public MeasureGate Strobe
        //{
        //    get => _Strobe;
        //    set
        //    {
        //        if (_Strobe != value)
        //        {
        //            _Strobe = value;
        //            OnPropertyChanged?.Invoke(nameof(Strobe));
        //        }
        //    }
        //}

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value && ValidSource(value))
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }
        // 建立保持添加边沿类型 数据源
        private EdgeSlope _SourceEdgeSlope = EdgeSlope.Rise;
        public EdgeSlope SourceEdgeSlope
        {
            get => _SourceEdgeSlope;
            set
            {
                if (_SourceEdgeSlope != value)
                {
                    _SourceEdgeSlope = value;
                    OnPropertyChanged();
                }
            }
        }
        private ChannelId _Source2nd = ChannelId.C2;
        public ChannelId Source2nd
        {
            get => _Source2nd;
            set
            {
                if (_Source2nd != value && ValidSource(value))
                {
                    _Source2nd = value;
                    OnPropertyChanged();
                }
            }
        }
        // 建立保持添加边沿类型 时钟源
        private EdgeSlope _Source2ndEdgeSlope = EdgeSlope.Rise;
        public EdgeSlope Source2ndEdgeSlope
        {
            get => _Source2ndEdgeSlope;
            set
            {
                if (_Source2ndEdgeSlope != value)
                {
                    _Source2ndEdgeSlope = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean ValidSource(ChannelId id)
        {
            if (Active)
            {
                if (MeasureType == MeasureType.Single)
                {
                    return id.IsAnalog()||id.IsReference();
                }
                else if (MeasureType == MeasureType.Composite)
                {
                    return id.IsMeasure();
                }
            }

            return true;
        }

        private MeasureOperator _Operation = MeasureOperator.Add;
        public MeasureOperator Operation
        {
            get => _Operation;
            set
            {
                if (_Operation != value)
                {
                    _Operation = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly String[] _DualSrcMeasItems =
       {
            "Delay@lv",
            "Phase@lv",
            "Setup",
            "Hold",
            "Crossing",
            "Skew"
        };
        public Boolean Dualsrc
        {
            get
            {
                Boolean temp = false;
                foreach (var item in _DualSrcMeasItems)
                {
                    if (Name.Contains(item))
                    {
                        temp = true;
                        break;
                    }
                }

                return temp;
            }
        }

        private Boolean _IsStatActive = false;
        public Boolean IsStatActive
        {
            get => _IsStatActive;
            set
            {
                if (_IsStatActive != value)
                {
                    _IsStatActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _TrackEnable = false;
        public Boolean TrackEnable
        {
            get => _TrackEnable;
            set
            {
                if (_TrackEnable != value)
                {
                    _TrackEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Track) ?? false;
                    if (value)
                        TrackEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _TrendEnable = false;
        public Boolean TrendEnable
        {
            get => _TrendEnable;
            set
            {
                if (_TrendEnable != value)
                {
                    _TrendEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Trend) ?? false;
                    if (value)
                        TrendEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _HistgramEnable = false;
        public Boolean HistgramEnable
        {
            get => _HistgramEnable;
            set
            {
                if (_HistgramEnable != value)
                {
                    _HistgramEnable = value;
                    var res = OpenOrCloseFigure?.Invoke(value, Id, MeasItemFigureType.Histgram) ?? false;
                    if (value)
                        HistgramEnable = res;
                    OnPropertyChanged();
                }
            }
        }

        public MeasThresholdExProp RefLevel { get; }

        //public MeasItemExProp? ExProp 
        //{ 
        //    get;
        //    private set;
        //} = null;

        //private readonly String[] _DualSrcMeasItems =
        //{
        //    "Delay@lv",
        //    "Phase@lv",
        //    "Setup",
        //    "Hold",
        //    "Crossing"
        //};

        //private readonly String[] _PosIdxMeasItems =
        //{
        //    "Level@x",
        //    "Phase@frq",
        //    "Power@frq",
        //};

        //protected MeasItemExProp? InitExProp(String name)
        //{
        //    foreach (var item in _DualSrcMeasItems)
        //    {
        //        if (name.Contains(item))
        //        {
        //            return new MeasDualSrcExProp(this, _PropertyChanged);
        //        }
        //    }

        //    foreach (var item in _PosIdxMeasItems)
        //    {
        //        if (name.Contains(item))
        //        {
        //            return new MeasPosIndexExProp(this, _PropertyChanged);
        //        }
        //    }

        //    return null;
        //}

        public String Key => MeasureType == MeasureType.Single ? MeasureProc.GetKey(Name, Source, Source2nd) : MeasureProc.GetKey(Operation.GetDescription(), Source, Source2nd);

        private readonly PropertyChangedEventHandler? _PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    //Zhangqc
    //internal class MeasureSimpleCalc
    //{
    //    public MeasureSimpleCalc(Action<String>? onPropertyChanged = null)
    //    {
    //        OnPropertyChanged = onPropertyChanged;
    //    }

    //    private Int32 _LeftIndex = 0;
    //    public Int32 LeftIndex
    //    {
    //        get => _LeftIndex;
    //        set
    //        {
    //            if (_LeftIndex != value)
    //            {
    //                _LeftIndex = value;
    //                OnPropertyChanged?.Invoke(nameof(LeftIndex));
    //            }
    //        }
    //    }

    //    private Int32 _RightIndex = 1;
    //    public Int32 RightIndex
    //    {
    //        get => _RightIndex;
    //        set
    //        {
    //            if (_RightIndex != value)
    //            {
    //                _RightIndex = value;
    //                OnPropertyChanged?.Invoke(nameof(RightIndex));
    //            }
    //        }
    //    }

    //    private MathBinaryType _Operator = MathBinaryType.Add;
    //    public MathBinaryType Operator
    //    {
    //        get => _Operator;
    //        set
    //        {
    //            if (_Operator != value)
    //            {
    //                _Operator = value;
    //                OnPropertyChanged?.Invoke(nameof(Operator));
    //            }
    //        }
    //    }

    //    public override String ToString()
    //    {
    //        return $@"P{LeftIndex}{Operator.GetAlias()}P{RightIndex}";
    //    }

    //    protected Action<String>? OnPropertyChanged { get; }
    //}
}
