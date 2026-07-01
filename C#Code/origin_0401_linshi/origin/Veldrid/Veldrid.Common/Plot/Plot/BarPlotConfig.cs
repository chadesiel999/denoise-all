using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;
using Vulkan;

namespace Veldrid.Common.Plot
{
    public class BarPlotConfig:BaseProperty
    {
        private PointF _LostMousePoint; 
        private object locker = new object();
        private BarRender.BarRenderConfig renderConfig;
        private BarPlot _Parent;
        private TextInfo _Textinfo;
        private PointConfig _PointConfig;
        private ImageInfo _ImageInfo;
        private TextInfo _TitleTextInfo;
        private Color barColor;
        private Color borderColor;
        //private Double[,] datas = new Double[0,0];
        public event EventHandler<float> VerticalOffsetChanged;
        public event EventHandler<PointF> MouseDown;
        public event EventHandler<PointF> MouseUp;
        public event EventHandler<PointF> Dragged;
        public event EventHandler<IDropRender> SelectionChanged;

        [AllowNull]
        public object Tag { get; set; }

        private TextInfo[] _CategoryTextInfos = new TextInfo[0];
        private TextInfo[] _ValueTextInfos = new TextInfo[0];
        public String Label { get => _Textinfo.Text; set => _Textinfo.Text = value; }

        internal BarPlotConfig(BarPlot parent,
            VeldridRender.BarRender.BarRenderConfig config,
            TextInfo textInfo,
            TextInfo titleTextInfo,
            PointConfig pointConfig,
            ImageInfo imageInfo)
        {
            _TitleTextInfo = titleTextInfo;
            _Parent = parent;
            renderConfig = config;
            _Textinfo = textInfo;
            _PointConfig = pointConfig;
            _ImageInfo = imageInfo;
            Title = "BarPlot";
            TitleVisibily = false;
        }
        public Color BarColor
        {
            get => barColor;
            set
            {
                barColor = value;
                renderConfig.Color = value.ColorConverToRGBA();
                _Textinfo.Color = value;
                _PointConfig.Color = value;
            }
        }
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                renderConfig.BorderColor = value.ColorConverToRGBA();
            }
        }
        private Single[] _TempData = new Single[0];
        public int Index => renderConfig.Index;
        public Double[,] Datas
        {
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(Datas));
                _TempData = value.Cast<Double>().Select(x => (Single)x).ToArray();
                renderConfig.Data = _TempData;
            }
        }
        public Single YPerValue { get; set; } = 1;
        internal Boolean MouseIn { get; set; }
        public String ValueUnit { get; set; }
        public String Unit { get; set; }
        internal Int32 NBinsCount => _TempData.Length;
        public Int32 FixedCount { get; set; }
        internal Int32 GetPositionIndex(out Single value)
        {
            value = 0;
            if (Double.IsNaN(Width)||Width==0) return -1;
            if (_LostMousePoint.X < Start || _LostMousePoint.X > Start + FixedCount * Width) return -1;
            Int32 index = (Int32)Math.Floor((_LostMousePoint.X - Start) / Width);
            if (index > _TempData.Length - 1)
                return -1;
            value = _TempData[index];
            if (_LostMousePoint.Y > value || _LostMousePoint.Y < BaseValue)
            {
                value = 0;
                return -1;
            }
            value = MathF.Round((value - BaseValue) * YPerValue, 0);
            return index;
        }
        internal Int32 GetPositionIndex(PointF point,out Single value)
        {
            MouseIn = true;
            _LostMousePoint = point;
            return GetPositionIndex(out value);
            
        }
        public List<CustomTextInfo> CustomTextInfos { get; } = new List<CustomTextInfo>();
        public Boolean CustomTextVisibily { get; set; } = true;
        public List<String> Infos { get; } = new List<String>();
        public Color InfoColor { get; set; } = Color.White;
        public Boolean InfoVisibily { get; set; } = true;
        public PointF InfoLocal { get; set; }
        public float Width
        {
            get => renderConfig.Width;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Width));
                if (renderConfig.Width != value)
                {
                    renderConfig.Width = value;
                    CalcAbsWidth(value);
                }
            }
        }
        private void CalcAbsWidth(Single width)
        {

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    renderConfig.AbsWidth = AxesTransfer.VirtualSizeToLocalSize(_Parent.BarRender.FixedMargin,_Parent.LineRange,new Vector2(_Parent.BarRender.TextureWidth,_Parent.BarRender.TextureHeight),0, width).Y;
                    break;
                case Orientation.Vertical:
                    renderConfig.AbsWidth = AxesTransfer.VirtualSizeToLocalSize(_Parent.BarRender.FixedMargin,_Parent.LineRange,new Vector2(_Parent.BarRender.TextureWidth,_Parent.BarRender.TextureHeight),width, 0).X;
                    break;
            }
        }
        protected override void Dispose(bool disposing)
        {
            this.ClearEventHandle();
            CursorImages.Clear();
            //datas = new Double[0,0];
            DashData = new Vector2[0];
            base.Dispose(disposing);
        }
        public float Start
        {
            get => renderConfig.Start;
            set
            {
                if (renderConfig.Start != value)
                {
                    renderConfig.Start = value;
                }
            }
        }
        public List<int> CursorImages { get; } = new List<int>();
        private int cursorIndex;

        private CursorLineStyle cursorLineStyle;
        private string[] category = new string[0];

        public int CursorIndex
        {
            get => cursorIndex;
            set
            {
                if (cursorIndex != value)
                {
                    cursorIndex = value;
                    _ImageInfo.ImageIndex = SelectedIndex;
                }
            }
        }

        public Boolean IsFocused { get; set; }
        public Boolean IsDragged { get; set; }

        internal Boolean Selected { get; set; }
        internal int SelectedIndex => (CursorIndex < 0 || CursorIndex >= CursorImages.Count) ? -1 : CursorImages[CursorIndex];
        public CursorLineStyle CursorLineStyle
        {
            get => cursorLineStyle;
            set
            {
                if (value != cursorLineStyle)
                {
                    cursorLineStyle = value;
                    SetDashData();
                }
            }
        }
        internal Vector2[] DashData { get; private set; } = new Vector2[0];
        private void SetDashData()
        {
            var size = _Parent.LocalSizeToVirtualSize(new Vector2(24, 24));
            DashData = Tools.DashedHelper.CalcDashedPoints(_Parent, CursorLineStyle, VerticalOffset, size);
            _PointConfig.PointCounts = new PointVisibily[] { DashData.Length };
        }
        public Orientation Orientation
        {
            get => renderConfig.Orientation;
            set
            {
                if (renderConfig.Orientation != value)
                {
                    renderConfig.Orientation = value;
                    _Textinfo.Rotation = value == Orientation.Horizontal ? 90 : 0;
                }
            }
        }
        [AllowNull]
        public string[] Category 
        {
            get => category;
            set
            {
                if (value == null) category = new string[0];
                else category = value;
                lock(locker)
                {
                    if(category.Length==0)
                    {
                        _CategoryTextInfos = new TextInfo[0];
                    }
                    else  
                    {
                        _CategoryTextInfos = category.Select(x => new TextInfo()
                        {
                            Color = BarColor,
                            Text = x,
                            Rotation = Orientation == Orientation.Horizontal ? 90 : 0,
                        }).ToArray();
                    }
                }
            }
        }

        internal Int32 ToolTipIndex { get; set; }
        public Boolean CategoryVisibily { get; set; }
        [AllowNull]
        public string[] Values { get; set; }
        public Boolean ValueVisibily { get; set; }
        public int DrawIndex { get => renderConfig.DrawIndex; set => renderConfig.DrawIndex = value; }
        public int DrawLength { get => renderConfig.DrawLength; set => renderConfig.DrawIndex = value; }
        public float VerticalOffset 
        { 
            get => renderConfig.VerticalOffset;
            set
            {
                if (renderConfig.VerticalOffset != value)
                {
                    renderConfig.VerticalOffset = value;
                    _ImageInfo.Position = new Vector2(_Parent.LineRange.MinX, value);
                    SetDashData();
                }
                _Textinfo.Local = new PointF(_Parent.LineRange.MinX + 206, value + 300/*VerticalOffset + 50*/);//new PointF(_Parent.LineRange.MinX + 120, value+200);
               
            }
        }
        public float HorizontalOffset
        {
            get => renderConfig.HorizontalOffset;
            set
            {
                if (value != renderConfig.HorizontalOffset)
                {
                    renderConfig.HorizontalOffset = value;
                }
            }
        }
        public float XValueOffset { get => renderConfig.XValueOffset; set => renderConfig.XValueOffset = value; }
        public float YValueOffset { get => renderConfig.YValueOffset; set => renderConfig.YValueOffset = value; }
        public string Title { get => _TitleTextInfo.Text; set => _TitleTextInfo.Text = value; }
        public Color TitleColor { get => _TitleTextInfo.Color; set => _TitleTextInfo.Color = value; }
        public Boolean TitleVisibily { get => _TitleTextInfo.Visibily; set => _TitleTextInfo.Visibily = value; }
        public int ZIndex
        {
            get => renderConfig.ZIndex;
            set
            {
                renderConfig.ZIndex = value;
                _ImageInfo.ZIndex = value;
            }
        }
        public Double MaxValue { get; set; }
        public Double MinValue { get; set; }
        public bool Visibily
        {
            get => renderConfig.Visibily;
            set
            {
                renderConfig.Visibily = value;
                _Textinfo.Visibily = value;
                _ImageInfo.Visibily = value;
                _PointConfig.PointCounts[0].Visibily = value;
            }
        }
        public PointF TitleLocal { get => _TitleTextInfo.Local; set => _TitleTextInfo.Local = value; }

        public float BaseValue
        {
            get => renderConfig.BaseValue;
            set
            {
                if (renderConfig.BaseValue != value)
                {
                    renderConfig.BaseValue = value;
                    CalcAbsBaseValue(value);
                }
            }
        }
        public void RefreshAbsValue()
        {
            CalcAbsWidth(renderConfig.Width);
            CalcAbsBaseValue(renderConfig.BaseValue);
        }
        private void CalcAbsBaseValue(Single basevalue)
        {
            var view = _Parent.BarRender.Camera.GetLineMatrix(_Parent.BarRender.TextureWidth,_Parent.BarRender.TextureHeight, _Parent.BarRender.FixedMargin, _Parent.LineRange,_Parent.BarRender.GraphicsDevice.IsClipSpaceYInverted);
            switch (renderConfig.Orientation)
            {
                case Orientation.Horizontal:
                    renderConfig.AbsBaseValue = AxesTransfer.VirtualPointToLocalPoint(_Parent.BarRender.Orth,view,basevalue, 0).X;
                    break;
                case Orientation.Vertical:
                    renderConfig.AbsBaseValue = AxesTransfer.VirtualPointToLocalPoint(_Parent.BarRender.Orth,view,0, basevalue).Y;
                    break;
            }
        }
        internal void OnMouseUp(PointF point) => MouseUp?.Invoke(this, point);
        internal void OnDragged(PointF point) => Dragged?.Invoke(this, point);
        internal void OnMouseDown(PointF point) => MouseDown?.Invoke(this, point);
        internal void OnSelectionChanged(DigitalPlot plot) => SelectionChanged?.Invoke(this, plot);
    }

    public class CustomTextInfo
    {
        private TextInfo textInfo = new TextInfo();
        public PointF Local { get=>textInfo.Local; set=>textInfo.Local= value; }
        public String Text { get=>textInfo.Text; set=>textInfo.Text =value; }
        public Color Color { get=>textInfo.Color; set=>textInfo.Color =value; }
        public float Rotation { get=>textInfo.Rotation; set=>textInfo.Rotation = value; }
        public VerticalAlignment VerticalAlignment { get=>textInfo.VerticalAlignment; set=>textInfo.VerticalAlignment = value; }
        public HorizontalAlignment HorizontalAlignment { get=>textInfo.HorizontalAlignment; set=>textInfo.HorizontalAlignment = value;}
        internal TextInfo TextInfo=>textInfo;
    }
}
