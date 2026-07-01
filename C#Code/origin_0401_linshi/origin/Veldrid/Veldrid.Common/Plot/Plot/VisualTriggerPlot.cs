using Clipper2Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using Veldrid.Common.Poly2Tri;
using Veldrid.Common.Poly2Tri.Polygons;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;
using Veldrid.Sdl2;

namespace Veldrid.Common.Plot
{
    public enum PolygonBoundary
    {
        In,
        Out,
        Edge,
        Vertex
    };
    public enum PolygonShapes
    {
        Rectangle,
        Triangle,
        Polygon
    }
    public class VisualTriggerPlot : BasePlot
    {
        public event Action<int> ReSetCompleted;
        public event Action<int, (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown)> RectangleChanged;
        public event Action<int> DoubleClick;
        /// <summary>
        /// 用户主动拖拽区域结束
        /// </summary>
        public event Action<int> DraggedOver;
        public event Action<int> DraggedMove;
        public event Action<int> Clicked;
        private DataRender _DataRender;
        private PolygonsConfig[] _PolygonsConfig;
        public VisualTriggerPlot(IVeldridContent control, Int32 Count) : base(control)
        {
            _DataRender = new DataRender(control, 100);
            _DataRender.CreateResources();
            _PolygonsConfig = new PolygonsConfig[Count];
            for (int i = 0; i < _PolygonsConfig.Length; i++)
            {
                _PolygonsConfig[i] = new PolygonsConfig(control, LineRange)
                {
                    ZIndex = i,
                };

                _PolygonsConfig[i].DraggedOver += s => DraggedOver?.Invoke(GetIndex(s));
                _PolygonsConfig[i].DraggedMove += s => DraggedMove?.Invoke(GetIndex(s));
                _PolygonsConfig[i].RectangleChanged += (s, a) => RectangleChanged?.Invoke(GetIndex(s), a);
                _PolygonsConfig[i].ReSetCompleted += s => ReSetCompleted?.Invoke(GetIndex(s));
                _PolygonsConfig[i].DoubleClick += s => DoubleClick?.Invoke(GetIndex(s));
                _PolygonsConfig[i].Clicked += s => Clicked?.Invoke(GetIndex(s));
            }
        }

        private int GetIndex(PolygonsConfig config)
        {
            if (_PolygonsConfig.Contains(config))
                return Array.IndexOf(_PolygonsConfig, config);

            return 0;
        }

        private protected override BaseVeldridRender Renderer => _DataRender;

        public PolygonsConfig this[Int32 index] => _PolygonsConfig[index];

        public Int32 Length => _PolygonsConfig.Length;

        public int IndexOf(PolygonsConfig config) => Array.IndexOf(_PolygonsConfig, config);

        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;
            base.OnMouseDown(point, ref handle);
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, len = _PolygonsConfig.Length; i < len; i++)
            {
                source[i].OnMouseDown(point, ref handle);
                if (handle)
                {
                    if (source.First() != source[i])
                    {
                        source[i].ZIndex = source.Max(c => c.ZIndex) + 1;
                    }
                    // source[i].ZIndex += 1;
                    return;
                }
            }
        }

        internal override void OnRightMouseDown(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;
            base.OnRightMouseDown(point, ref handle);
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, len = _PolygonsConfig.Length; i < len; i++)
            {
                source[i].OnRightMouseDown(point, ref handle);
                if (handle)
                {
                    if (source.First() != source[i])
                    {
                        source[i].ZIndex = source.Max(c => c.ZIndex) + 1;
                    }
                    // source[i].ZIndex += 1;
                    return;
                }
            }
        }

        private PolygonsConfig[] GetOrderdPolygonsConfig() => _PolygonsConfig?.OrderByDescending(c => c.ZIndex).ToArray();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            var source = GetOrderdPolygonsConfig().Reverse(); // 绘制时，最后绘制顶层
            source.ToList().ForEach(config => config.Dispose());
            _PolygonsConfig = null;
        }

        internal override void OnMouseUp(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;
            base.OnMouseUp(point, ref handle);
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, ct = _PolygonsConfig.Length; i < ct; i++)
            {
                source[i].OnMouseUp(point, ref handle);
                if (handle)
                {
                    return;
                }
            }
        }

        internal override void OnDoubleClick(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;

            base.OnDoubleClick(point, ref handle);
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, ct = _PolygonsConfig.Length; i < ct; i++)
            {
                source[i].OnDoubleClick(point, ref handle);
                if (handle)
                {
                    return;
                }
            }
        }

        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;
            base.OnMouseMove(point, ref handle);
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, ct = _PolygonsConfig.Length; i < ct; i++)
            {
                source[i].OnMouseMove(point, ref handle);
                if (handle)
                {
                    return;
                }
            }
            //handle = boundary != Boundary.Out;
            //if (handle)
            //{
            //    switch (boundary)
            //    {
            //        case Boundary.In:
            //            SetCursor(Sdl2.SDL_SystemCursor.Hand);
            //            break;
            //        case Boundary.Out:
            //            break;
            //        case Boundary.Edge:
            //            SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
            //            break;
            //        case Boundary.Vertex:
            //            SetCursor(Sdl2.SDL_SystemCursor.SizeAll);
            //            break;
            //        default:
            //            break;
            //    }
            //}

        }

        protected override void SetWindowSizeState(bool state)
        {
            base.SetWindowSizeState(state);
            GetOrderdPolygonsConfig().ToList().ForEach(config => config.SetWindowSizeState());
            //UpdateRenderPoints();
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            base.ActiveDragged(sender, point);
            GetOrderdPolygonsConfig()?.First()?.OnActiveDragged(point);
        }

        public override void Draw()
        {
            if (!Visibily)
                return;
            var source = GetOrderdPolygonsConfig().Reverse(); // 绘制时，最后绘制顶层
            source.ToList().ForEach(config => config.Render());
        }

        public Boolean IsInPlotArea()
        {
            var source = GetOrderdPolygonsConfig();
            for (int i = 0, ct = _PolygonsConfig.Length; i < ct; i++)
            {
                if (source[i].IsInPlotArea())
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class PolygonsConfig : BaseProperty
    {
        private Sdl2Window _Window;
        private bool _isDragged = false;
        //pixel
        private const Int32 _AreaTouchExpand = 25;

        // virtual size
        private const Int32 _TxtTouchExpand = 30;

        private Int32 _SelectedVertexIndex = -1;
        private Int32 _RectagleMinLength = 5;

        /// <summary>
        /// 框的上下边界margion
        /// </summary>
        private int _VerticalMargion = 250;

        private PointF _LastPoint = new PointF();
        private PolygonBoundary PlotBoundary { get; set; }
        private DataRender _DataRender;
        private DataRender _DataRender_Border;
        private DataRender _DataRender_Vertex;
        private VeldridText _VeldridText;
        private TimeSpan _LastMouseDown = TimeSpan.Zero;
        public event Action<PolygonsConfig> ReSetCompleted;
        public event Action<PolygonsConfig> DoubleClick;
        public event Action<PolygonsConfig, (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown)> RectangleChanged;
        public event Action<PolygonsConfig> DraggedOver;
        public event Action<PolygonsConfig> DraggedMove;
        public event Action<PolygonsConfig> Clicked;

        public PolygonsConfig(IVeldridContent control, LineRange linerange)
        {
            LineRange = linerange;
            _Window = control.Window;

            _DataRender = new DataRender(control, 6);
            _DataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = null,
                            Brightness = 40
                        }
                    },
                    Primitive = PrimitiveTopology.TriangleList,
                },
            };
            _DataRender.CreateResources();

            _DataRender_Border = new DataRender(control, 5);
            _DataRender_Border.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs=new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = null,
                            Brightness = 100,
                            Color=BorderColor
                        }
                    },
                    Primitive = PrimitiveTopology.LineStrip,
                },
            };
            _DataRender_Border.CreateResources();

            _DataRender_Vertex = new DataRender(control, 20);
            _DataRender_Vertex.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig() { Primitive = PrimitiveTopology.LineStrip, }
            };
            _DataRender_Vertex.CreateResources();

            _VeldridText = new VeldridText(control, false);
            _VeldridText.CreateResources();
        }
        #region 文本属性

        public String Name_FontName { get => _VeldridText.FontName; set => _VeldridText.FontName = value; }

        public String Name_FontStyle { get => _VeldridText.FontStyle; set => _VeldridText.FontStyle = value; }

        public float Name_FontSize { get => _VeldridText.FontSize; set => _VeldridText.FontSize = value; }
        public String Name_Label { get => _VeldridText.Text; set => _VeldridText.Text = value; }
        public bool Name_LabelVisibility { get => _VeldridText.Visibily; set => _VeldridText.Visibily = value; }
        public Color Name_Color { get => _VeldridText.Color; set => _VeldridText.Color = value; }
        public PointF Name_TextLocal { get => _VeldridText.Local; set => _VeldridText.Local = value; }
        #endregion

        public int ZIndex { get; set; }

        public Color Color
        {
            get => _DataRender.DataRenderConfigs[0].PointConfigs[0].Color;
            set => _DataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
        }

        private Color _BorderColor = Color.White;
        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                if (_BorderColor != value)
                {
                    _BorderColor = value;
                    UpdateRenderPoints();
                }
            }
        }

        public float Brightness
        {
            get => _DataRender.DataRenderConfigs[0].PointConfigs[0].Brightness;
            set
            {
                /*if (value < 50)
                {
                    _DataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
                    _DataRender_Border.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
                }*/
            }
        }

        public Boolean Visibily
        {
            get => _DataRender.Visibily;
            set
            {
                _DataRender.Visibily = value;
                _DataRender_Border.Visibily = value;
                _DataRender_Vertex.Visibily = value;
                _VeldridText.Visibily = value;
            }
        }

        private Int32 _PolygonEdgeNumber;
        public Int32 PolygonEdgeNumber
        {
            get => _PolygonEdgeNumber;
            set
            {
                if (_PolygonEdgeNumber != value)
                {
                    _PolygonEdgeNumber = value;
                    ReCalaPoints();
                }
            }
        }

        private PointF _PolygonCenter;
        public PointF PolygonCenter
        {
            get => _PolygonCenter;
            set
            {
                if (_PolygonCenter != value)
                {
                    _PolygonCenter = value;
                    ReCalaPoints();
                }
            }
        }

        private float _PolygonRadius;
        /// <summary>
        /// The unit of radius is pixel
        /// </summary>
        public float PolygonRadius
        {
            get => _PolygonRadius;
            set
            {
                if (_PolygonRadius != value)
                {
                    _PolygonRadius = value;
                    ReCalaPoints();
                }
            }
        }

        private PolygonShapes _PolygonShape;
        public PolygonShapes PolygonShape
        {
            get => _PolygonShape;
            set
            {
                if (_PolygonShape != value)
                {
                    _PolygonShape = value;
                    ReCalaPoints();
                }
            }
        }

        private Boolean _ReSet;

        public Boolean Reset
        {
            get => _ReSet;
            set
            {
                if (_ReSet != value)
                {
                    _ReSet = value;
                    if (_ReSet)
                    {
                        SetDefaultPosition();
                        RectangleChanged?.Invoke(this, _RectaglePoints);
                        ReSetCompleted?.Invoke(this);
                        ReCalaPoints();
                        //ReSetCompleted?.Invoke(new Object(), EventArgs.Empty);
                    }
                }
            }
        }

        private void SetDefaultPosition() => RectaglePoints = (new PointF(4000f, 1000f), new PointF(6000f, 1000f), new PointF(6000f, -1000f), new PointF(4000f, -1000f));

        private String _Name;

        public String Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    ReCalaPoints();
                }
            }
        }

        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _RectaglePoints;

        /// <summary>
        /// 矩形的四个点
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) RectaglePoints
        {
            get { return _RectaglePoints; }
            set
            {
                if (_RectaglePoints != value)
                {
                    _RectaglePoints = value;
                    UpdateRenderPoints();
                    RectangleChanged?.Invoke(this, _RectaglePoints);
                }
            }
        }

        private List<PointF> _Points = new List<PointF>();
        public List<PointF> Points
        {
            get => _Points;
            set
            {
                if (value != null)
                {
                    _Points = value;
                    UpdateRenderPoints();
                }
            }
        }

        public List<List<PointF>> Polygons { get; private set; } = new();

        public LineRange LineRange { private get; set; }

        public Boolean IsInPlotArea(PointF point)
        {
            if (!Visibily)
                return false;

            var in_text = InTextRegion(point);
            if (in_text)
                return true;

            if (_PolygonShape == PolygonShapes.Rectangle)
            {
                if (RectaglePoints.LeftDown.IsEmpty || RectaglePoints.LeftUp.IsEmpty || RectaglePoints.RightDown.IsEmpty || RectaglePoints.RightUp.IsEmpty)
                    return false;
            }
            else
            {
                if (Polygons == null || Polygons.Count <= 0)
                    return false;
            }

            var boundary = CheckPosition(point, ref _SelectedVertexIndex);
            return boundary != PolygonBoundary.Out;
        }

        private void ReCalaPoints()
        {
            Points.Clear();
            /*var centerx = (PolygonCenter.X - LineRange.MinX) / LineRange.XLenght * _DataRender.Rectangle.Width;
            var centery = (PolygonCenter.Y - LineRange.MinY) / LineRange.YLenght * _DataRender.Rectangle.Height;
            switch (_PolygonShape)
            {
                case PolygonShapes.Triangle:
                    Points.Add(new PointF(4000f, -1000f));
                    Points.Add(new PointF(5000f, 1000f));
                    Points.Add(new PointF(6000f, -1000f));
                    break;
                case PolygonShapes.Rectangle:
                    RectaglePoints = (new PointF(4000f, 1000f), new PointF(6000f, 1000f), new PointF(6000f, -1000f), new PointF(4000f, -1000f));
                    RectangleChanged?.Invoke(this, _RectaglePoints);
                    ReSetCompleted?.Invoke(null, new EventArgs());
                    break;
                case PolygonShapes.Polygon:
                    var newpoint = new List<PointF>();
                    newpoint = GetPolygon(new PointF(centerx, centery), PolygonRadius, PolygonEdgeNumber);
                    Points = newpoint.ConvertAll(new Converter<PointF, PointF>(PixelToPointF));
                    break;
                default:
                    break;
            }*/
        }

        private List<Vector2> _Result = new List<Vector2>(5000);
        private List<Vector2> _PolygonVector2 = new List<Vector2>();
        private List<List<Vector2>> _Borderes = new();
        private List<Vector2> _Vertexrects = new List<Vector2>();
        private List<Polygon> _Polygons = new();
        private List<bool> _IsVertexVisible = new();
        private Object _Lock = new object();
        private void ClearList()
        {
            _Result.Clear();
            _PolygonVector2.Clear();
            _Borderes.Clear();
            _Vertexrects.Clear();
            _Polygons.Clear();
            _IsVertexVisible.Clear();
        }
        protected override void Dispose(bool disposing)
        {
            _Result.Clear();
            _PolygonVector2.Clear();
            _Borderes.Clear();
            _Vertexrects.Clear();
            _Polygons.Clear();
            _IsVertexVisible.Clear();
            _DataRender.DisposeResources();
            _DataRender_Border.DisposeResources();
            _DataRender_Vertex.DisposeResources();
            base.Dispose(disposing);
        }

        private void UpdateRenderPoints()
        {
            lock (_Lock)
            {
                // 暂时只支持矩形形状
                if (!Visibily || _PolygonShape != PolygonShapes.Rectangle || _RectaglePoints.LeftUp.IsEmpty || _RectaglePoints.RightUp.IsEmpty || _RectaglePoints.LeftDown.IsEmpty || _RectaglePoints.RightDown.IsEmpty)
                    return;

                ClearList();
                var borderlist = new List<Vector2>()
                {
                    new Vector2(_RectaglePoints.LeftUp.X, _RectaglePoints.LeftUp.Y),
                    new Vector2(_RectaglePoints.RightUp.X, _RectaglePoints.RightUp.Y),
                    new Vector2(_RectaglePoints.RightDown.X, _RectaglePoints.RightDown.Y),
                    new Vector2(_RectaglePoints.LeftDown.X, _RectaglePoints.LeftDown.Y),
                    new Vector2(_RectaglePoints.LeftUp.X, _RectaglePoints.LeftUp.Y),
                };

                var xarrary = new List<float>()
                {
                    _RectaglePoints.LeftUp.X,
                    _RectaglePoints.RightUp.X,
                    _RectaglePoints.RightDown.X,
                    _RectaglePoints.LeftDown.X,
                };

                var yarrary = new List<float>()
                {
                    _RectaglePoints.LeftUp.Y,
                    _RectaglePoints.RightUp.Y,
                    _RectaglePoints.RightDown.Y,
                    _RectaglePoints.LeftDown.Y,
                };

                var startX = xarrary.Min(c => c);
                var startY = yarrary.Max(c => c);
                var endX = xarrary.Max(c => c);
                var endY = yarrary.Min(c => c);

                /*var minx_p = Math.Clamp(startX, LineRange.MinX, LineRange.MaxX);
                var maxx_p = Math.Clamp(endX, LineRange.MinX, LineRange.MaxX);
                var miny_p = Math.Clamp(endY, LineRange.MinY, LineRange.MaxY);
                var maxy_p = Math.Clamp(startY, LineRange.MinY, LineRange.MaxY);
                // 使用间隔点填充矩形
                // 生成间隔点
                // 生成横平竖直的点集合
                var interval = 65;
                var intervalY = interval * 2 - 10;
                List<Vector2> fillPoints = ShapeFillHelper.FillPoint(new RectangleF(minx_p, maxy_p, maxx_p - minx_p, maxy_p - miny_p), interval, intervalY).ToList();
                _PolygonVector2.Capacity = fillPoints.Capacity + 100;
                _PolygonVector2.AddRange(fillPoints);*/

                // 使用实心矩形填充
                _PolygonVector2.Add(new Vector2(_RectaglePoints.LeftUp.X, _RectaglePoints.LeftUp.Y));
                _PolygonVector2.Add(new Vector2(_RectaglePoints.RightUp.X, _RectaglePoints.RightUp.Y));
                _PolygonVector2.Add(new Vector2(_RectaglePoints.RightDown.X, _RectaglePoints.RightDown.Y));
                _PolygonVector2.Add(new Vector2(_RectaglePoints.RightDown.X, _RectaglePoints.RightDown.Y));
                _PolygonVector2.Add(new Vector2(_RectaglePoints.LeftDown.X, _RectaglePoints.LeftDown.Y));
                _PolygonVector2.Add(new Vector2(_RectaglePoints.LeftUp.X, _RectaglePoints.LeftUp.Y));

                List<Vector2> temp;
                void GetVertex(PointF point)
                {
                    var rect = GetRectangle(point, 10);
                    var leftup = new Vector2(rect.X, rect.Y);
                    var rightup = new Vector2(rect.X + rect.Width, rect.Y);
                    var rightdown = new Vector2(rect.X + rect.Width, rect.Y - rect.Height);
                    var leftdown = new Vector2(rect.X, rect.Y - rect.Height);

                    _Vertexrects.Add(leftup);
                    _Vertexrects.Add(rightup);
                    _Vertexrects.Add(rightdown);
                    _Vertexrects.Add(leftdown);
                    _Vertexrects.Add(leftup);

                    temp = new List<Vector2>() { leftup, rightup, rightdown, leftdown };
                    _IsVertexVisible.Add(!(temp.All(c => c.X > LineRange.MaxX) ||
                        temp.All(c => c.X < LineRange.MinX) ||
                        temp.All(c => c.Y < LineRange.MinY) ||
                        temp.All(c => c.Y > LineRange.MaxY)));
                }

                GetVertex(_RectaglePoints.LeftUp);
                GetVertex(_RectaglePoints.RightUp);
                GetVertex(_RectaglePoints.RightDown);
                GetVertex(_RectaglePoints.LeftDown);

                /*
                 * 
                 System.ArgumentException:“Source array was not long enough. Check the source index, length, and the array's lower bounds. Arg_ParamName_Name”
                 */
                //_Result.Capacity = _PolygonVector2.Capacity + borderlist.Capacity + _Vertexrects.Capacity + 500;
                var fillpointsarrary = _PolygonVector2.ToArray();
                var borderlistArrary = borderlist.ToArray();
                var vertextArrary = _Vertexrects.ToArray();
                /*_Result.AddRange(fillpointsarrary);
                _Result.AddRange(borderlistArrary);
                _Result.AddRange(vertextArrary);*/

                var all_out_of_screen = borderlistArrary.All(c => c.X > LineRange.MaxX) ||
                   borderlistArrary.All(c => c.X < LineRange.MinX) ||
                   borderlistArrary.All(c => c.Y < LineRange.MinY) ||
                   borderlistArrary.All(c => c.Y > LineRange.MaxY);

                //填充形状
                var pointcount = (uint)fillpointsarrary.Length;
                _DataRender.DataRenderConfigs[0].PointConfigs[0].VertexCount = pointcount;
                _DataRender.DataRenderConfigs[0].PointConfigs[0].Color = Color;
                _DataRender.DataRenderConfigs[0].FixedDataLenght = pointcount;
                _DataRender.DataRenderConfigs[0].DataLenght = pointcount;
                _DataRender.DataRenderConfigs[0].PointConfigs[0].Visibily = !all_out_of_screen;

                //边框
                _DataRender_Border.DataRenderConfigs[0].PointConfigs[0].VertexCount = (uint)borderlistArrary.Length;
                _DataRender_Border.DataRenderConfigs[0].PointConfigs[0].Color = BorderColor;
                _DataRender_Border.DataRenderConfigs[0].FixedDataLenght = (uint)borderlistArrary.Length;
                _DataRender_Border.DataRenderConfigs[0].DataLenght = (uint)borderlistArrary.Length;
                if (all_out_of_screen)
                {
                    _DataRender_Border.DataRenderConfigs[0].PointConfigs[0].Visibily = false;
                }
                else
                {
                    _DataRender_Border.DataRenderConfigs[0].PointConfigs[0].Visibily = true;
                }

                //顶点
                _DataRender_Vertex.DataRenderConfigs[0].PointConfigs = new PointConfig[4];
                var vertexct = vertextArrary.Length / 4;
                for (int i = 0; i < 4; i++)
                {
                    _DataRender_Vertex.DataRenderConfigs[0].PointConfigs[i] = new PointConfig()
                    {
                        PointCounts = new PointVisibily[]
                        {
                            new PointVisibily((uint)vertexct, _IsVertexVisible[i])
                        },
                        Brightness = 100,
                        Color = BorderColor
                    };
                }

                _DataRender_Vertex.DataRenderConfigs[0].FixedDataLenght = (uint)vertextArrary.Length;
                _DataRender_Vertex.DataRenderConfigs[0].DataLenght = (uint)vertextArrary.Length;

                var txtx = (startX + endX) / 2 - 100;
                var txty = startY + 80;

                var txt_size = _VeldridText.GetVirtualSize();
                var txt_size_x = txt_size.X > LineRange.MaxX ? 0 : txt_size.X;
                txtx = Math.Clamp(txtx,Math.Min( LineRange.MinX, LineRange.MaxX - txt_size_x), Math.Max(LineRange.MinX, LineRange.MaxX - txt_size_x));

                float miny = LineRange.MinY + txt_size.Y;
                float maxy = LineRange.MaxY - txt_size.Y;
                txty = Math.Clamp(txty,Math.Min(miny, maxy),Math.Max(miny, maxy) );

                // 区域名称文本绘制
                Name_TextLocal = new PointF(txtx, txty);

                _DataRender.WriteData(0, fillpointsarrary);
                _DataRender_Border.WriteData(0, borderlistArrary);
                _DataRender_Vertex.WriteData(0, vertextArrary);
            }
        }

        private PolygonBoundary CheckPosition(PointF point, ref Int32 index)
        {
            if (!Visibily)
                return PolygonBoundary.Out;

            var points = new List<PointF>();
            switch (_PolygonShape)
            {
                case PolygonShapes.Rectangle:
                    points = new List<PointF>()
                    {
                        _RectaglePoints.LeftUp,
                        _RectaglePoints.RightUp,
                        _RectaglePoints.RightDown,
                        _RectaglePoints.LeftDown,
                    };
                    break;
                case PolygonShapes.Triangle:
                    break;
                case PolygonShapes.Polygon:
                    break;
                default:
                    break;
            }
            if (points.Count <= 0)
                return PolygonBoundary.Out;

            var res = PointInPolygonResult.IsOutside;
            index = points.FindIndex(p => Math.Abs(p.X - point.X) < _AreaTouchExpand * LineRange.XLenght / _DataRender.Rectangle.Width && Math.Abs(p.Y - point.Y) < _AreaTouchExpand * LineRange.YLenght / _DataRender.Rectangle.Height);
            if (index != -1)
            {
                return PolygonBoundary.Vertex;
            }

            var _Paths64 = Clipper2Lib.Clipper.Union(points, Clipper2Lib.FillRule.NonZero);
            foreach (var path in _Paths64)
            {
                res = Clipper.PointInPolygon(new Point64(point.X, point.Y), path);
                if (res == PointInPolygonResult.IsInside)
                {
                    return PolygonBoundary.In;
                }
                else if (res == PointInPolygonResult.IsOutside)
                {
                    //return Boundary.Out;
                }
                else if (res == PointInPolygonResult.IsOn)
                {
                    return PolygonBoundary.Edge;
                }
            }
            return PolygonBoundary.Out;
        }

        /// <summary>
        /// 使用外切圆的方法绘制一个正多边形
        /// </summary>
        /// <param name="center">正多边形外切圆的圆心</param>
        /// <param name="radius">正多边形外切圆的半径</param>
        /// <param name="sideCount">正多边形的边数</param>
        private List<PointF> GetPolygon(PointF center, float radius, Int32 sideCount = 3)
        {
            // 多边形至少要有3条边，边数不达标就返回.
            if (sideCount < 3)
            {
                sideCount = 3;
            }
            // 每条边对应的圆心角角度，精确为浮点数。使用弧度制，360度角为2派
            var arc = 2 * Math.PI / sideCount;
            // 为多边形创建所有的顶点列表
            var pointList = new List<PointF>();
            for (var i = 0; i < sideCount; i++)
            {
                var curArc = arc * i; // 当前点对应的圆心角角度
                var pt = new PointF();
                // 就是简单的三角函数正余弦根据圆心角和半径算点坐标。这里都取整就行
                pt.X = (float)(center.X + Math.Round(radius * Math.Cos(curArc), 2));
                pt.Y = (float)(center.Y + Math.Round(radius * Math.Sin(curArc), 2));
                pointList.Add(pt);
            }

            return pointList;
        }

        private RectangleF GetRectangle(PointF pointf, Int32 pixel = 5)
        {
            RectangleF rect = new();
            rect.Width = pixel * LineRange.XLenght / _DataRender.Rectangle.Width;
            rect.Height = pixel * LineRange.YLenght / _DataRender.Rectangle.Height;
            rect.Location = new PointF(pointf.X - rect.Width / 2, pointf.Y + rect.Height / 2);
            return rect;
        }

        private PointF PixelToPointF(PointF pixel) => new PointF(pixel.X / _DataRender.Rectangle.Width * LineRange.XLenght + LineRange.MinX, pixel.Y / _DataRender.Rectangle.Height * LineRange.YLenght + LineRange.MinY);

        /// <summary>
        /// 获取时间戳，避免DateTime.Now由于修改了系统时间导致的程序错误。
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetTimestampSpan()
        {
            double timestampSeconds = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 1000d;
            TimeSpan timestampTimeSpan = TimeSpan.FromMilliseconds(timestampSeconds);
            return timestampTimeSpan;
        }

        public void OnRightMouseDown(PointF point, ref Boolean handle)
        {
            if (!Visibily)
                return;
            _LastPoint = point;
            PlotBoundary = CheckPosition(point, ref _SelectedVertexIndex);
            var in_text = InTextRegion(point);
            handle = PlotBoundary != PolygonBoundary.Out || in_text;

            if (handle)
                DoubleClick?.Invoke(this);
        }

        /// <summary>
        /// 判定指定点是否在文本矩形内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool InTextRegion(PointF point)
        {
            var size = _VeldridText.VirtualSize;

            var wm = _AreaTouchExpand * LineRange.XLenght / _DataRender.Rectangle.Width;
            var hm = _AreaTouchExpand * LineRange.YLenght / _DataRender.Rectangle.Height;

            var minx = _VeldridText.Local.X - wm;
            var maxx = _VeldridText.Local.X + size.X + wm;
            var maxy = _VeldridText.Local.Y + hm;
            var miny = _VeldridText.Local.Y - size.Y - wm;

            return point.X >= minx && point.X <= maxx && point.Y >= miny && point.Y <= maxy;
        }

        public void OnMouseDown(PointF point, ref Boolean handle)
        {
            if (!Visibily)
                return;
            _LastPoint = point;
            PlotBoundary = CheckPosition(point, ref _SelectedVertexIndex);
            var in_text = InTextRegion(point);
            handle = PlotBoundary != PolygonBoundary.Out || in_text;
            if (PlotBoundary != PolygonBoundary.Out)
                Clicked?.Invoke(this);

            /*if (PlotBoundary == PolygonBoundary.In)
            {
                // 在文本区域内
                var currenttimespan = GetTimestampSpan();
                var offset = (currenttimespan - _LastMouseDown).TotalMilliseconds;
                if (offset >= 100 && offset <= 400)
                {
                    _LastMouseDown = TimeSpan.Zero;
                    DoubleClick?.Invoke(this);
                }
                else
                {
                    _LastMouseDown = currenttimespan;
                }
            }*/
        }

        /// <summary>
        /// 双击和右击是一个效果
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handle"></param>
        public void OnDoubleClick(PointF point, ref Boolean handle) => OnRightMouseDown(point, ref handle);

        public void OnMouseUp(PointF point, ref Boolean handle)
        {
            if (!Visibily)
                return;
            if (_isDragged)
            {
                DraggedOver?.Invoke(this);
                _LastMouseDown = TimeSpan.Zero;
                _isDragged = false;
            }
        }

        public void OnMouseMove(PointF point, ref Boolean handle)
        {
            if (!Visibily)
                return;
            var boundary = CheckPosition(point, ref _SelectedVertexIndex);
            handle = boundary != PolygonBoundary.Out;
            if (handle && Visibily)
            {
                switch (boundary)
                {
                    case PolygonBoundary.In:
                        SetCursor(Sdl2.SDL_SystemCursor.Hand);
                        break;
                    case PolygonBoundary.Out:
                        break;
                    case PolygonBoundary.Edge:
                        SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
                        break;
                    case PolygonBoundary.Vertex:
                        SetCursor(Sdl2.SDL_SystemCursor.SizeAll);
                        break;
                    default:
                        break;
                }

                if (_isDragged)
                {
                    DraggedMove?.Invoke(this);
                }
            }
        }

        public void OnActiveDragged(PointF point)
        {
            if (!Visibily)
                return;
            var xoffset = point.X - _LastPoint.X;
            var yoffset = point.Y - _LastPoint.Y;
            var npoint = Point.Round(new PointF(xoffset, yoffset));
            var in_text = InTextRegion(point);
            if (PlotBoundary == PolygonBoundary.In || (in_text && PlotBoundary != PolygonBoundary.Vertex))
            {
                if (_PolygonShape == PolygonShapes.Rectangle)
                {
                    var leftup = new PointF(_RectaglePoints.LeftUp.X + npoint.X, _RectaglePoints.LeftUp.Y + npoint.Y);
                    var rightup = new PointF(_RectaglePoints.RightUp.X + npoint.X, _RectaglePoints.RightUp.Y + npoint.Y);
                    var rightdown = new PointF(_RectaglePoints.RightDown.X + npoint.X, _RectaglePoints.RightDown.Y + npoint.Y);
                    var leftdown = new PointF(_RectaglePoints.LeftDown.X + npoint.X, _RectaglePoints.LeftDown.Y + npoint.Y);


                    // 限制垂直方向上的边界，使得垂直方向上预留一定距离可以触摸拖动，否则太靠近边界无法拖动
                    if (leftdown.Y > LineRange.MaxY - _VerticalMargion)
                    {
                        // 整体向下移动
                        var offset = leftdown.Y - (LineRange.MaxY - _VerticalMargion);
                        leftup.Y -= offset;
                        rightup.Y -= offset;
                        rightdown.Y -= offset;
                        leftdown.Y -= offset;
                    }
                    else if (leftup.Y < LineRange.MinY + _VerticalMargion)
                    {
                        // 整体向上移动
                        var offset = leftup.Y - (LineRange.MinY + _VerticalMargion);
                        leftup.Y -= offset;
                        rightup.Y -= offset;
                        rightdown.Y -= offset;
                        leftdown.Y -= offset;
                    }

                    _RectaglePoints = (leftup, rightup, rightdown, leftdown);

                    RectangleChanged?.Invoke(this, _RectaglePoints);
                }
                else
                {
                    for (int i = 0; i < Points.Count; i++)
                    {
                        Points[i] = new PointF(Points[i].X + npoint.X, Points[i].Y + npoint.Y);
                    }
                }
                UpdateRenderPoints();
            }
            else if (PlotBoundary == PolygonBoundary.Vertex)
            {
                var offsetX = point.X - _LastPoint.X;
                var offsetY = point.Y - _LastPoint.Y;

                if (PolygonShape == PolygonShapes.Rectangle)
                {
                    // 必须保持为矩形，不能拖拽成四边形。

                    //float p0_x = 0, p0_y = 0;
                    //float p1_x = 0, p1_y = 0;
                    //float p2_x = 0, p2_y = 0;
                    //float p3_x = 0, p3_y = 0;

                    switch (_SelectedVertexIndex)
                    {
                        case 0:
                            //p0_x = offsetX;
                            //p0_y = offsetY;
                            //p1_x = 0;
                            //p1_y = offsetY;
                            //p2_x = 0;
                            //p2_y = 0;
                            //p3_x = offsetX;
                            //p3_y = 0;

                            Single leftupx = _RectaglePoints.LeftUp.X + offsetX;
                            Single leftupy = _RectaglePoints.LeftUp.Y + offsetY;
                            leftupx = (leftupx + _RectagleMinLength) > _RectaglePoints.RightUp.X ? _RectaglePoints.RightUp.X - _RectagleMinLength : leftupx;
                            leftupy = (leftupy - _RectagleMinLength) < _RectaglePoints.LeftDown.Y ? _RectaglePoints.LeftDown.Y + _RectagleMinLength : leftupy;

                            RectaglePoints = (new PointF(leftupx, leftupy),
                                              new PointF(_RectaglePoints.RightUp.X, leftupy),
                                              new PointF(_RectaglePoints.RightDown.X, _RectaglePoints.RightDown.Y),
                                              new PointF(leftupx, _RectaglePoints.LeftDown.Y));
                            break;
                        case 1:
                            //p0_x = 0;
                            //p0_y = offsetY;
                            //p1_x = offsetX;
                            //p1_y = offsetY;
                            //p2_x = offsetX;
                            //p2_y = 0;
                            //p3_x = 0;
                            //p3_y = 0;

                            Single rightupx = _RectaglePoints.RightUp.X + offsetX;
                            Single rightupy = _RectaglePoints.RightUp.Y + offsetY;

                            rightupx = (rightupx - _RectagleMinLength) < _RectaglePoints.LeftUp.X ? _RectaglePoints.LeftUp.X + _RectagleMinLength : rightupx;
                            rightupy = (rightupy - _RectagleMinLength) < _RectaglePoints.RightDown.Y ? _RectaglePoints.RightDown.Y + _RectagleMinLength : rightupy;

                            RectaglePoints = (new PointF(_RectaglePoints.LeftUp.X, rightupy),
                                              new PointF(rightupx, rightupy),
                                              new PointF(rightupx, _RectaglePoints.RightDown.Y),
                                              new PointF(_RectaglePoints.LeftDown.X, _RectaglePoints.LeftDown.Y));

                            break;
                        case 2:
                            //p0_x = 0;
                            //p0_y = 0;
                            //p1_x = offsetX;
                            //p1_y = 0;
                            //p2_x = offsetX;
                            //p2_y = offsetY;
                            //p3_x = 0;
                            //p3_y = offsetY;
                            Single rightdownx = _RectaglePoints.RightDown.X + offsetX;
                            Single rightdowny = _RectaglePoints.RightDown.Y + offsetY;
                            rightdownx = (rightdownx - _RectagleMinLength) < _RectaglePoints.LeftDown.X ? _RectaglePoints.LeftDown.X + _RectagleMinLength : rightdownx;
                            rightdowny = (rightdowny + _RectagleMinLength) > _RectaglePoints.RightUp.Y ? _RectaglePoints.RightUp.Y - _RectagleMinLength : rightdowny;

                            RectaglePoints = (new PointF(_RectaglePoints.LeftUp.X, _RectaglePoints.LeftUp.Y),
                                              new PointF(rightdownx, _RectaglePoints.RightUp.Y),
                                              new PointF(rightdownx, rightdowny),
                                              new PointF(_RectaglePoints.LeftDown.X, rightdowny));
                            break;
                        case 3:
                            //p0_x = offsetX;
                            //p0_y = 0;
                            //p1_x = 0;
                            //p1_y = 0;
                            //p2_x = 0;
                            //p2_y = offsetY;
                            //p3_x = offsetX;
                            //p3_y = offsetY;
                            Single leftdownx = _RectaglePoints.LeftDown.X + offsetX;
                            Single leftdowny = _RectaglePoints.LeftDown.Y + offsetY;
                            leftdownx = (leftdownx + _RectagleMinLength) > _RectaglePoints.RightDown.X ? _RectaglePoints.RightDown.X + _RectagleMinLength : leftdownx;
                            leftdowny = (leftdowny + _RectagleMinLength) > _RectaglePoints.LeftUp.Y ? _RectaglePoints.LeftUp.Y - _RectagleMinLength : leftdowny;

                            RectaglePoints = (new PointF(leftdownx, _RectaglePoints.LeftUp.Y),
                                              new PointF(_RectaglePoints.RightUp.X, _RectaglePoints.RightUp.Y),
                                              new PointF(_RectaglePoints.RightDown.X, leftdowny),
                                              new PointF(leftdownx, leftdowny));
                            break;
                    }
                }
                else
                {
                    Points[_SelectedVertexIndex] = new PointF(Points[_SelectedVertexIndex].X + offsetX, Points[_SelectedVertexIndex].Y + offsetY);
                    UpdateRenderPoints();
                }
            }
            _LastPoint = point;
            _isDragged = true;
            //PolygonsChanged?.Invoke(this, Polygons);
        }

        public void Render()
        {
            if (!Visibily)
                return;
            lock (_Lock)
            {
                _DataRender.Draw();
                _DataRender_Border.Draw();
                _DataRender_Vertex.Draw();
                _VeldridText.Draw();
            }
        }

        public void SetWindowSizeState()
        {
            UpdateRenderPoints();
        }
        private void SetCursor(SDL_SystemCursor cursor) => _Window.Cursor = cursor;

        public Boolean IsInPlotArea() => PlotBoundary != PolygonBoundary.Out;
    }
}
