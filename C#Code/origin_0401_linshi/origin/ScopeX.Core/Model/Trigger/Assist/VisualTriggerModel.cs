using ScopeX.ComModel;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ScopeX.Core
{
    internal class VisualTriggerModel : INotifyPropertyChanged
    {
        public VisualTriggerModel()
        {
            SelectedItems = new VisualTriggerItemModel[]
            {
                new VisualTriggerItemModel(ChannelId.C1, ItemPropertyChanged,"A"),
                new VisualTriggerItemModel(ChannelId.C2, ItemPropertyChanged,"B"),
            };
        }
        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    if (!value && SelectedItems != null)
                    {
                        foreach (var item in SelectedItems)
                            item.Enabled = false;
                    }
                    OnPropertyChanged();
                }
            }
        }


        private Int32 _CurrentSelected = 0;
        public Int32 CurrentSelected
        {
            get => _CurrentSelected;
            set
            {
                if (value != _CurrentSelected)
                {
                    _CurrentSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private VisualTriggerRelation _Relation;
        /// <summary>
        /// 区域关系
        /// </summary>
        public VisualTriggerRelation Relation
        {
            get { return _Relation; }
            set
            {
                if (_Relation != value)
                {
                    _Relation = value;
                    Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
                    OnPropertyChanged();
                }
            }
        }

        public VisualTriggerItemModel[] SelectedItems
        {
            get;
        }

        public void Run(Boolean newwfm)
        {
            if (!newwfm || !_Enabled)
            {
                return;
            }

            for (var i = 0; i < SelectedItems.Length; i++)
            {
                var bsucceed = SelectedItems[i].Test();
            }

        }


        protected PropertyChangedEventHandler? _PropertyChanged;

        public virtual event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                TriggerShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                TriggerShareParameter.Default.PropertyChanged -= value;
            }
        }


        protected void ItemPropertyChanged(Object? sender, PropertyChangedEventArgs e)
        {
            _PropertyChanged?.Invoke(sender, e);
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VisualTriggerItemModel
    {
        public VisualTriggerItemModel(ChannelId source, PropertyChangedEventHandler? propertyChanged, String? name = null)
        {
            _Name = name;
            _Source = source;
            _PropertyChanged = propertyChanged;
        }

        private String? _Name;
        public String? Name
        {
            get => _Name;
            init => _Name = value;
        }

        private Boolean _Success = false;
        public Boolean Success
        {
            get => _Success;
        }

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    if (value)
                    {
                        DsoModel.Default.VisualTrigger.Enabled = value;
                    }
                    //Dispatcher.DoClear();
                    OnPropertyChanged();
                }
            }
        }

        private Color? _PolygonsDrawColor = null;
        public Color PolygonsDrawColor
        {
            get => _PolygonsDrawColor ?? (DsoModel.Default.TryGetChannel(Source, out var ch) ? ch.DrawColor : Color.Gray);
            set
            {
                if (_PolygonsDrawColor != value)
                {
                    _PolygonsDrawColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _rectanglePoints;
        /// <summary>
        /// 矩形的四个点
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) RectanglePoints
        {
            get { return _rectanglePoints; }
            set
            {
                if (_rectanglePoints != value)
                {
                    _rectanglePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<List<PointF>> _Polygons = new List<List<PointF>>();
        public List<List<PointF>> Polygons
        {
            get => _Polygons;
            set
            {
                if (value != null)
                {
                    _Polygons = value;
                    ReadMask(_Polygons, _Source);
                    MakeMask();
                }
            }
        }

        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _PositionOfHV;
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) PositionOfHV
        {
            get => _PositionOfHV;
            set
            {
                if (value != _PositionOfHV)
                {
                    _PositionOfHV = value;
                    OnPropertyChanged();
                }
            }
        }

        private VisualTriggerState _TriggerState = VisualTriggerState.Overlap;
        public VisualTriggerState TriggerState
        {
            get => _TriggerState;
            set
            {
                if (_TriggerState != value)
                {
                    _TriggerState = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _PolygonEdgeNumber = 5;
        public Int32 PolygonEdgeNumber
        {
            get => _PolygonEdgeNumber;
            set
            {
                if (value < 5)
                {
                    value = 5;
                }
                if (_PolygonEdgeNumber != value)
                {
                    _PolygonEdgeNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private PointF _PolygonCenter = new PointF((Single)(Constants.MAX_XPOS_IDX / 2), 0);
        /// <summary>
        /// The unit of polygonrenter is virtual coordinate
        /// </summary>
        public PointF PolygonCenter
        {
            get => _PolygonCenter;
            set
            {
                if (_PolygonCenter != value)
                {
                    _PolygonCenter = value;
                    OnPropertyChanged();
                }
            }
        }

        private Single _PolygonRadius = 200f;
        /// <summary>
        /// The unit of polygonradius is pixel coordinate
        /// </summary>
        public Single PolygonRadius
        {
            get => _PolygonRadius;
            set
            {
                if (_PolygonRadius != value)
                {
                    _PolygonRadius = value;
                    OnPropertyChanged();
                }
            }
        }

        private VisualTriggerShape _TriggerShape = VisualTriggerShape.Rectangle;
        public VisualTriggerShape TriggerShape
        {
            get => _TriggerShape;
            set
            {
                if (_TriggerShape != value)
                {
                    _TriggerShape = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _ReSet = false;
        public Boolean ReSet
        {
            get => _ReSet;
            set
            {
                if (_ReSet != value)
                {
                    _ReSet = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _TimebaseScale;
        /// <summary>
        /// 当前形状所属的时基缩放值，用于记录和计算形状缩放使用
        /// </summary>
        public Double TimebaseScale
        {
            get => _TimebaseScale;
            set
            {
                if (_TimebaseScale != value)
                {
                    _TimebaseScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _VerticalScale;
        /// <summary>
        /// 当前形状所属的垂直缩放值，用于记录和计算形状缩放使用
        /// </summary>
        public Double VerticalScale
        {
            get { return _VerticalScale; }
            set
            {
                if (_VerticalScale != value)
                {
                    _VerticalScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _VerticalPosIndexBymDiv;
        /// <summary>
        /// 垂直档位偏移div数量
        /// </summary>
        public Double VerticalPosIndexBymDiv
        {
            get { return _VerticalPosIndexBymDiv; }
            set
            {
                if (_VerticalPosIndexBymDiv != value)
                {
                    _VerticalPosIndexBymDiv = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _TimeBasePosIndexBymDiv;

        public Double TimeBasePosIndexBymDiv
        {
            get { return _TimeBasePosIndexBymDiv; }
            set
            {
                if (_TimeBasePosIndexBymDiv != value)
                {
                    _TimeBasePosIndexBymDiv = value;
                    OnPropertyChanged();
                }
            }
        }

        private StdMaskTest.StdMaskPkg StdMask = new()
        {
            Amplitude = 0,
            VScale = 0,
            VPos = 0,
            VOffset = 0,
            HScale = 0,
            HPos = 0,
            HWidth = 0,
            TrigToSamp = 0,
            Percent = 0,
            RecordLength = 0,
            Segments = new List<PointF>[]
           {
                new List<PointF>(), new List<PointF>(), new List<PointF>(), new List<PointF>(),
                new List<PointF>(), new List<PointF>(), new List<PointF>(), new List<PointF>()
           },

            IsVaild = false,

            SegPaths = new List<(Double x, Double y)>[]
           {
                new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(),
                new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>(), new List<(Double x, Double y)>()
           },
        };

        public Boolean MaskCreated
        {
            get;
            set;
        } = false;

        public Boolean ReadMask(List<List<PointF>> points, ChannelId id)
        {
            String[] segstrs =
            {
                    "SEG1:POINTS",
                    //"SEG2:POINTS",
                    //"SEG3:POINTS",
                    //"SEG4:POINTS",
                    //"SEG5:POINTS",
                    //"SEG6:POINTS",
                    //"SEG7:POINTS",
                    //"SEG8:POINTS",
            };
            if (StdMask.Segments.Length != points.Count)
            {
                StdMask.Segments = new List<PointF>[points.Count];
            }

            for (Int32 j = 0; j < points.Count; j++)
            {
                if (StdMask.Segments[j] == null)
                {
                    StdMask.Segments[j] = new List<PointF>();
                }
                else
                {
                    StdMask.Segments[j].Clear();
                }
                for (Int32 i = 0; i < points[j].Count; i++)
                {
                    StdMask.Segments[j].Add(points[j][i]);
                }
            }

            StdMask.IsVaild = true;

            return true;
        }

        public Boolean MakeMask()
        {
            ChannelId id = _Source;
            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl is null)
            {
                return false;
            }

            lock (StdMask.SegPaths.SyncRoot)
            {
                if (StdMask.SegPaths.Length != StdMask.Segments.Length)
                {
                    StdMask.SegPaths = new List<(Double x, Double y)>[StdMask.Segments.Length];
                }

                for (Int32 i = 0; i < StdMask.SegPaths.Length; i++)
                {
                    if (StdMask.SegPaths[i] == null)
                    {
                        StdMask.SegPaths[i] = new List<(Double x, Double y)>();
                    }
                    else
                    {
                        StdMask.SegPaths[i].Clear();
                    }
                }

                //分别处理每一个封闭区域
                for (Int32 i = 0; i < StdMask.Segments.Length; i++)
                {
                    //平面上至少3个点才能定义一个封闭区域
                    if (StdMask.Segments[i].Count >= 3)
                    {
                        var points = new (Double, Double)[StdMask.Segments[i].Count];

                        Double x, y;
                        for (Int32 j = 0; j < StdMask.Segments[i].Count; j++)
                        {
                            x = StdMask.Segments[i][j].X;
                            y = StdMask.Segments[i][j].Y;

                            points[j] = (x, y);
                        }

                        StdMask.SegPaths[i].AddRange(points);
                    }
                }
            }

            MaskCreated = true;

            return true;
        }

        public Boolean Test() => Test(_Source);

        private Boolean Test(ChannelId id)
        {
            var chnl = DsoModel.Default.GetChannel(id);
            if (chnl?.Pack is null)
            {
                return false;
            }
            var pbuffer = chnl.Pack.Buffer.ToJagged()[0];
            var offsety = chnl.Conditioning.Position;


            Boolean bpassed = true;

            var hits = new List<(Double x, Double y)>();
            lock (StdMask.SegPaths.SyncRoot)
            {
                for (Int32 i = 0; i < pbuffer.Length; i++)
                {
                    for (Int32 j = 0; j < StdMask.SegPaths.Length; j++)
                    {
                        if (StdMask.SegPaths[j].Count < 3)
                        {
                            continue;
                        }

                        var pt = (i, Convert.ToSingle(pbuffer[i]));

                        if (StdMask.SegPaths[j].InPolygon((i, (Convert.ToSingle(pt.Item2 + offsety)) / chnl.Conditioning.Scale * chnl.Conditioning.PosIdxPerDiv)))
                        {
                            hits.Add(pt);
                            bpassed = false;
                        }
                    }
                }
                _Success = TriggerState == VisualTriggerState.Overlap ? !bpassed : bpassed;
                return _Success;
            }
        }

        private readonly PropertyChangedEventHandler? _PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
