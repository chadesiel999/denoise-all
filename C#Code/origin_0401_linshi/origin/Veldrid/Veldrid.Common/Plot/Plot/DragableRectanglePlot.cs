using Clipper2Lib;
using MatterHackers.Agg.VertexSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Vulkan;

namespace Veldrid.Common.Plot.Plot
{
    /// <summary>
    /// 形状的填充类型
    /// </summary>
    public enum FillType
    {
        /// <summary>
        /// 不填充
        /// </summary>
        None,

        /// <summary>
        /// 点阵
        /// </summary>
        Point,

        /// <summary>
        /// 线段
        /// </summary>
        LineSegment,
    }


    /// <summary>
    /// 可拖拽矩形的模式
    /// </summary>
    public enum RectangleMode
    {
        /// <summary>
        /// 正常矩形模式
        /// </summary>
        Noraml,

        /// <summary>
        /// 拖拽创建模式
        /// </summary>
        DragCreat,
    }

    /// <summary>
    /// 可拖拽矩形的绘制
    /// </summary>
    public class DragableRectanglePlot : BasePlot
    {
        #region Fields

        private IVeldridContent _Control;

        /// <summary>
        /// 填充区域绘制层
        /// </summary>
        private DataRender _FillRender;
        private DataRenderConfig _FillConfig;
        private DataRenderConfig _FillConfig_Point;
        private DataRenderConfig _FillConfig_Line;

        /// <summary>
        /// 边框绘制层
        /// </summary>
        private DataRender _BorderRender;
        private DataRenderConfig _BorderConfig;

        /// <summary>
        /// 可拖拽顶点绘制层
        /// </summary>
        private DataRender _VetextRender;
        private DataRenderConfig _VetextConfig;

        // 顶点计算辅助集合
        private List<Vector2> _Vertexts = new List<Vector2>();
        private List<bool> _VertextVisible = new List<bool>();

        /// <summary>
        /// 触摸识别区域像素大小
        /// </summary>
        private int _AreaTouchExpand = 25;

        // 事件处理标志
        private PointF? _LastMouseDown;
        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown)? _LastMouseDownRect;
        private PointF? _LastDragMove;
        private Boolean _StartDrawRect = false; // 指示是否开始拖拽绘制矩形
        private PolygonBoundary _BoundaryType = PolygonBoundary.Out;
        private VertexType? _SelectedVertex = null;

        #endregion

        #region Events

        /// <summary>
        /// 在矩形内点击事件
        /// </summary>
        public event Action<DragableRectanglePlot, PointF> Clicked;

        /// <summary>
        /// 拖拽绘制矩形完成
        /// </summary>
        public event Action<DragableRectanglePlot> DrawComplete;

        /// <summary>
        /// 矩形改变事件
        /// </summary>
        public event Action<DragableRectanglePlot> RectangleChanged;

        /// <summary>
        /// 模式改变事件
        /// </summary>
        public event Action<DragableRectanglePlot> ModeChanged;

        #endregion

        #region Properties

        private Boolean _Visibily = true;
        /// <summary>
        /// 矩形是否可见
        /// </summary>
        public new Boolean Visibily
        {
            get { return _Visibily; }
            set
            {
                _Visibily = value;
                _BorderRender.Visibily = _Visibily;
                _VetextRender.Visibily = _Visibily;
                if (_FillRender != null)
                    _FillRender.Visibily = _Visibily;

                if (_Visibily)
                    Recalculate();
            }
        }

        private RectangleMode _Mode = RectangleMode.Noraml;

        public RectangleMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; ModeChange(); }
        }


        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _Bound;
        /// <summary>
        /// 矩形定义
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) Bound
        {
            get { return _Bound; }
            set
            {
                _Bound = value;
                Recalculate();
                RectangleChanged?.Invoke(this);
            }
        }

        private FillType _Fill = FillType.None;
        /// <summary>
        /// 矩形填充类型,默认为 None(不填充)
        /// </summary>
        public FillType Fill
        {
            get { return _Fill; }
            set { _Fill = value; }
        }

        /// <summary>
        /// 是否显示顶点
        /// </summary>
        public Boolean ShowVertex
        {
            get { return _VetextRender.Visibily; }
            set
            {
                _VetextRender.Visibily = value;
            }
        }

        private Color _BorderColor = Color.White;

        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        private Color _FillColor = Color.White;
        /// <summary>
        /// 填充色
        /// </summary>
        public Color FillColor
        {
            get { return _FillColor; }
            set { _FillColor = value; }
        }


        private Color _VertexColor = Color.White;
        /// <summary>
        /// 顶点颜色
        /// </summary>
        public Color VertexColor
        {
            get { return _VertexColor; }
            set { _VertexColor = value; }
        }


        #endregion


        public DragableRectanglePlot(IVeldridContent control) : base(control)
        {
            _Control = control;
            _BorderConfig = new DataRenderConfig()
            {
                FixedDataLenght = 5,
                DataLenght = 5,
                PointConfigs = new PointConfig[]
                {
                    new PointConfig()
                    {
                        PointCounts = null,
                        Brightness = 50,
                        Color = BorderColor,
                        VertexCount = 5,
                    }
                },
                Primitive = PrimitiveTopology.LineStrip,
            };
            _BorderRender = new DataRender(control, 5);
            _BorderRender.DataRenderConfigs = new DataRenderConfig[] { _BorderConfig };
            _BorderRender.CreateResources();

            _VetextRender = new DataRender(control, 20);
            _VetextConfig = new DataRenderConfig() { Primitive = PrimitiveTopology.LineStrip };
            _VetextRender.DataRenderConfigs = new DataRenderConfig[] { _VetextConfig };
            _VetextRender.CreateResources();

            _FillRender = new DataRender(_Control, 500);
            _FillRender.CreateResources();

            _FillConfig_Point = new DataRenderConfig()
            {
                PointConfigs = new PointConfig[]
                {
                    new PointConfig()
                    {
                        PointCounts = null,
                        Brightness = 50,
                        Color = FillColor
                    }
                },
                Primitive = PrimitiveTopology.PointList,
            };

            _FillConfig_Line = new DataRenderConfig()
            {
                PointConfigs = new PointConfig[]
                {
                    new PointConfig()
                    {
                        PointCounts = null,
                        Brightness = 50,
                        Color = FillColor
                    }
                },
                Primitive = PrimitiveTopology.LineList,
            };

            var defaultpoint = new PointF(0, 0);
            _Bound = (defaultpoint, defaultpoint, defaultpoint, defaultpoint);

            Recalculate();
        }

        private protected override BaseVeldridRender Renderer => _BorderRender;

        protected override void Dispose(bool disposing)
        {
            Clear();
            _FillRender?.DisposeResources();
            _VetextRender?.DisposeResources();
            base.Dispose(disposing);
        }

        public override void Draw()
        {
            base.Draw();
            _FillRender?.Draw();
            _VetextRender?.Draw();
        }

        #region Private Method

        /// <summary>
        /// 获取矩形的x、y的最大最小值
        /// </summary>
        /// <returns></returns>
        private (float MinX, float MaxX, float MinY, float MaxY) GetBoundary()
        {
            (float MinX, float MaxX, float MinY, float MaxY) result = default;

            List<float> xvals = new List<float>() { Bound.LeftUp.X, Bound.RightUp.X, Bound.RightDown.X, Bound.LeftDown.X };
            List<float> yvals = new List<float>() { Bound.LeftUp.Y, Bound.RightUp.Y, Bound.RightDown.Y, Bound.LeftDown.Y };

            result.MinX = xvals.Min();
            result.MaxX = xvals.Max();
            result.MinY = yvals.Min();
            result.MaxY = yvals.Max();

            return result;
        }

        private bool IsValideFloat(float v) => !float.IsInfinity(v) && !float.IsNaN(v);

        /// <summary>
        /// 矩形定义是否有效
        /// </summary>
        /// <returns></returns>
        private bool IsValide() => IsValideFloat(Bound.LeftUp.X) && IsValideFloat(Bound.LeftUp.Y) && IsValideFloat(Bound.RightUp.X) && IsValideFloat(Bound.RightUp.Y) && IsValideFloat(Bound.RightDown.X) && IsValideFloat(Bound.RightDown.Y) && IsValideFloat(Bound.LeftDown.X) && IsValideFloat(Bound.LeftDown.Y);

        private Vector2 PointF2Vector2(PointF p) => new Vector2(p.X, p.Y);

        /// <summary>
        /// 获取矩形的顶点拖拽点的定义
        /// </summary>
        /// <param name="pointf"></param>
        /// <param name="pixel"></param>
        /// <returns></returns>
        private RectangleF GetVertexRectangle(PointF pointf, Int32 pixel = 5)
        {
            RectangleF rect = new();
            rect.Width = pixel * LineRange.XLenght / Rectangle.Width;
            rect.Height = pixel * LineRange.YLenght / Rectangle.Height;
            rect.Location = new PointF(pointf.X - rect.Width / 2, pointf.Y + rect.Height / 2);
            return rect;
        }

        /// <summary>
        /// 获取指定位置的顶点矩形集合
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private IEnumerable<Vector2> GetVertex(PointF p)
        {
            var rect = GetVertexRectangle(p, 10);
            var leftup = new Vector2(rect.X, rect.Y);
            var rightup = new Vector2(rect.X + rect.Width, rect.Y);
            var rightdown = new Vector2(rect.X + rect.Width, rect.Y - rect.Height);
            var leftdown = new Vector2(rect.X, rect.Y - rect.Height);

            yield return leftup;
            yield return rightup;
            yield return rightdown;
            yield return leftdown;
            yield return leftup;
        }

        /// <summary>
        /// 判断顶点是否超出了可视区域
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private bool IsOutOfRange(IEnumerable<Vector2> vertex) => vertex.All(c => c.X > LineRange.MaxX) || vertex.All(c => c.X < LineRange.MinX);

        /// <summary>
        /// 重算
        /// </summary>
        private void Recalculate()
        {
            if (!Visibily || !IsValide())
                return;

            Clear();

            var boundary = GetBoundary();

            #region 填充逻辑

            switch (Fill)
            {
                case FillType.Point:
                    var intervalx = 65;
                    var intervalY = intervalx * 2 - 10;

                    // 限制填充点，超出可视区域的部分不显示
                    var minx = Math.Clamp(boundary.MinX, LineRange.MinX, LineRange.MaxX);
                    var maxx = Math.Clamp(boundary.MaxX, LineRange.MinX, LineRange.MaxX);
                    var miny = Math.Clamp(boundary.MinY, LineRange.MinY, LineRange.MaxY);
                    var maxy = Math.Clamp(boundary.MaxY, LineRange.MinY, LineRange.MaxY);

                    var fillPoints = ShapeFillHelper.FillPoint(new RectangleF(minx, maxy, (maxx - minx), (maxy - miny)), intervalx, intervalY).ToList();

                    _FillConfig_Point.DataLenght = (uint)fillPoints.Count;
                    _FillConfig_Point.FixedDataLenght = (uint)fillPoints.Count;
                    _FillConfig_Point.PointConfigs[0].VertexCount = (uint)fillPoints.Count;
                    _FillConfig_Point.PointConfigs[0].Color = FillColor;
                    _FillRender.DataRenderConfigs = new DataRenderConfig[] { _FillConfig_Point };
                    _FillRender.Visibily = Visibily;
                    _FillRender.WriteData(0, fillPoints.ToArray());
                    break;
                case FillType.LineSegment:
                    // 先不显示
                    _FillRender.Visibily = false;
                    break;
                case FillType.None:
                default:
                    // 先不显示
                    _FillRender.Visibily = false;
                    break;
            }

            #endregion

            #region 边框逻辑

            var border_points = new Vector2[] { PointF2Vector2(Bound.LeftUp), PointF2Vector2(Bound.RightUp), PointF2Vector2(Bound.RightDown), PointF2Vector2(Bound.LeftDown), PointF2Vector2(Bound.LeftUp) };
            var isbordervisible = IsOutOfRange(border_points);
            _BorderConfig.PointConfigs[0].Color = BorderColor;
            _BorderConfig.PointConfigs[0].Visibily = !isbordervisible;
            _BorderRender.WriteData(0, border_points);

            #endregion

            #region 顶点逻辑

            var leftup_vt = GetVertex(Bound.LeftUp);
            _VertextVisible.Add(!IsOutOfRange(leftup_vt));
            _Vertexts.AddRange(leftup_vt);

            var rightup_vt = GetVertex(Bound.RightUp);
            _VertextVisible.Add(!IsOutOfRange(rightup_vt));
            _Vertexts.AddRange(rightup_vt);

            var righdown = GetVertex(Bound.RightDown);
            _VertextVisible.Add(!IsOutOfRange(righdown));
            _Vertexts.AddRange(righdown);

            var leftdown_vt = GetVertex(Bound.LeftDown);
            _VertextVisible.Add(!IsOutOfRange(leftdown_vt));
            _Vertexts.AddRange(leftdown_vt);

            _VetextConfig.PointConfigs = new PointConfig[4];
            for (int i = 0; i < 4; i++)
            {
                _VetextConfig.PointConfigs[i] = new PointConfig()
                {
                    PointCounts = new PointVisibily[] { new PointVisibily(5, _VertextVisible[i] && ShowVertex) },
                    Brightness = 50,
                    Color = VertexColor
                };
            }

            _VetextConfig.DataLenght = (uint)_Vertexts.Count;
            _VetextConfig.FixedDataLenght = (uint)_Vertexts.Count;

            _VetextRender.WriteData(0, _Vertexts.ToArray());

            #endregion

        }

        private void Clear()
        {
            _Vertexts.Clear();
            _VertextVisible.Clear();
        }

        /// <summary>
        /// 模式改变
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ModeChange()
        {
            switch (Mode)
            {
                case RectangleMode.Noraml:
                default:
                    _StartDrawRect = false;
                    break;
                case RectangleMode.DragCreat:
                    Clear();
                    _LastMouseDown = null;
                    _LastMouseDownRect = null;
                    break;
            }

            ModeChanged?.Invoke(this);
        }


        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;

            _LastMouseDown = point;
            _LastMouseDownRect = _Bound;
            var t = CheckPosition(point);
            _BoundaryType = t.Item1;
            _SelectedVertex = t.Item2;
            switch (Mode)
            {
                case RectangleMode.Noraml:
                    handle = t.Item1 != PolygonBoundary.Out;
                    if (handle)
                        Clicked?.Invoke(this, point);
                    break;
                case RectangleMode.DragCreat:
                    Bound = (point, point, point, point);
                    _StartDrawRect = true;
                    handle = true;
                    break;
            }

            base.OnMouseDown(point, ref handle);
        }

        internal override void OnMouseUp(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;

            if (Mode == RectangleMode.DragCreat)
            {
                _StartDrawRect = false;

                if (_LastMouseDown != null)
                    DrawComplete?.Invoke(this);
            }

            // 鼠标抬起后，始终是正常模式
            Mode = RectangleMode.Noraml;
            _LastMouseDown = null;
            _LastMouseDownRect = null;
            _SelectedVertex = null;
            base.OnMouseUp(point, ref handle);
        }
        internal override void OnMouseLeave(ref bool handle)
        {
            if (!Visibily)
                return;

            if (Mode == RectangleMode.DragCreat)
            {
                _StartDrawRect = false;

                if (_LastMouseDown != null)
                    DrawComplete?.Invoke(this);
            }

            // 鼠标抬起后，始终是正常模式
            Mode = RectangleMode.Noraml;
            _LastMouseDown = null;
            _LastMouseDownRect = null;
            _SelectedVertex = null;
            base.OnMouseLeave(ref handle);
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            if (!Visibily)
                return;

            switch (Mode)
            {
                case RectangleMode.Noraml:
                    if (_BoundaryType == PolygonBoundary.In && _LastMouseDown != null && _LastMouseDownRect != null)
                    {
                        // 拖拽移动操作
                        var offsetX = point.X - _LastMouseDown.Value.X;
                        var offsetY = point.Y - _LastMouseDown.Value.Y;

                        var tp = _LastMouseDownRect.Value;

                        Bound = (new PointF(tp.LeftUp.X + offsetX, tp.LeftUp.Y + offsetY),
                            new PointF(tp.RightUp.X + offsetX, tp.RightUp.Y + offsetY),
                            new PointF(tp.RightDown.X + offsetX, tp.RightDown.Y + offsetY),
                            new PointF(tp.LeftDown.X + offsetX, tp.LeftDown.Y + offsetY));
                    }
                    else if (_BoundaryType == PolygonBoundary.Vertex && _SelectedVertex != null && _LastMouseDown != null && _LastMouseDownRect != null)
                    {
                        var offsetX = point.X - _LastMouseDown.Value.X;
                        var offsetY = point.Y - _LastMouseDown.Value.Y;

                        float p0_x = 0, p0_y = 0;
                        float p1_x = 0, p1_y = 0;
                        float p2_x = 0, p2_y = 0;
                        float p3_x = 0, p3_y = 0;

                        switch (_SelectedVertex.Value)
                        {
                            case VertexType.LeftUp:
                                p0_x = offsetX;
                                p0_y = offsetY;
                                p1_x = 0;
                                p1_y = offsetY;
                                p2_x = 0;
                                p2_y = 0;
                                p3_x = offsetX;
                                p3_y = 0;
                                break;
                            case VertexType.RightUp:
                                p0_x = 0;
                                p0_y = offsetY;
                                p1_x = offsetX;
                                p1_y = offsetY;
                                p2_x = offsetX;
                                p2_y = 0;
                                p3_x = 0;
                                p3_y = 0;
                                break;
                            case VertexType.RightDown:
                                p0_x = 0;
                                p0_y = 0;
                                p1_x = offsetX;
                                p1_y = 0;
                                p2_x = offsetX;
                                p2_y = offsetY;
                                p3_x = 0;
                                p3_y = offsetY;
                                break;
                            case VertexType.LeftDown:
                                p0_x = offsetX;
                                p0_y = 0;
                                p1_x = 0;
                                p1_y = 0;
                                p2_x = 0;
                                p2_y = offsetY;
                                p3_x = offsetX;
                                p3_y = offsetY;
                                break;
                        }

                        var tp = _LastMouseDownRect.Value;
                        Bound = (new PointF(tp.LeftUp.X + p0_x, tp.LeftUp.Y + p0_y),
                            new PointF(tp.RightUp.X + p1_x, tp.RightUp.Y + p1_y),
                            new PointF(tp.RightDown.X + p2_x, tp.RightDown.Y + p2_y),
                            new PointF(tp.LeftDown.X + p3_x, tp.LeftDown.Y + p3_y));
                    }
                    break;
                case RectangleMode.DragCreat:
                    if (_StartDrawRect)
                        Bound = (Bound.LeftUp, new PointF(point.X, Bound.LeftUp.Y), point, new PointF(Bound.LeftUp.X, point.Y));
                    break;
            }
            base.ActiveDragged(sender, point);
        }

        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            if (!Visibily)
                return;

            base.OnMouseMove(point, ref handle);
        }

        /// <summary>
        /// 点point是否在点target的可识别区域内
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <param name="touchExpand">可识别区域像素</param>
        /// <returns></returns>
        private bool InIdentificationZone(PointF point, PointF target, int touchExpand) => Math.Abs(target.X - point.X) < touchExpand * LineRange.XLenght / Rectangle.Width && Math.Abs(target.Y - point.Y) < touchExpand * LineRange.YLenght / Rectangle.Height;

        /// <summary>
        /// 判断点和矩形的关系，以及点在哪个顶点识别区内
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private (PolygonBoundary, VertexType?) CheckPosition(PointF p)
        {
            if (!Visibily)
                return (PolygonBoundary.Out, null);

            VertexType? vt = null;
            if (InIdentificationZone(p, Bound.LeftUp, _AreaTouchExpand))
                vt = VertexType.LeftUp;

            if (InIdentificationZone(p, Bound.RightUp, _AreaTouchExpand))
                vt = VertexType.RightUp;

            if (InIdentificationZone(p, Bound.RightDown, _AreaTouchExpand))
                vt = VertexType.RightDown;

            if (InIdentificationZone(p, Bound.LeftDown, _AreaTouchExpand))
                vt = VertexType.LeftDown;

            if (vt != null)
                return (PolygonBoundary.Vertex, vt);

            List<PointF> temp = new List<PointF>() { Bound.LeftUp, Bound.RightUp, Bound.RightDown, Bound.LeftDown };
            var _Paths64 = Clipper.Union(temp, FillRule.NonZero);
            var res = PointInPolygonResult.IsOutside;
            foreach (var path in _Paths64)
            {
                res = Clipper.PointInPolygon(new Point64(p.X, p.Y), path);
                if (res == PointInPolygonResult.IsInside)
                {
                    return (PolygonBoundary.In, null);
                }
                else if (res == PointInPolygonResult.IsOn)
                {
                    return (PolygonBoundary.Edge, null);
                }
            }

            return (PolygonBoundary.Out, null);
        }

        #endregion

        #region Public Method

        /// <summary>
        /// 判定给定点是否在矩形内
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Boolean IsInRectangle(PointF p) => CheckPosition(p).Item1 != PolygonBoundary.Out;

        #endregion

        #region Inner Object

        enum VertexType
        {
            LeftUp,
            RightUp,
            LeftDown,
            RightDown,
        }

        #endregion
    }
}
