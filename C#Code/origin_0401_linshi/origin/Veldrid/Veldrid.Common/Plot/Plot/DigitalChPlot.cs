using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public unsafe class DigitalChPlot : BaseProperty
    {
        private float verticalOffset;
        private string name = string.Empty;
        private Boolean nameVisility=false;
        private Color color;
        private int cursorIndex=-1;
        private int zIndex;
        private CursorLineStyle cursorLineStyle;
        private DigitalChConfig chConfig;
        private TextInfo _textInfo;
        private ImageInfo _imageInfo;

        public event EventHandler<float> VerticalOffsetChanged;
        public event EventHandler<PointF> MouseDown;
        public event EventHandler<PointF> MouseUp;
        public event EventHandler<PointF> Dragged;
        public event EventHandler<IDropRender> SelectionChanged;
        internal Boolean Selected { get; set; }
        internal DigitalChPlot(DigitalChConfig config,TextInfo textInfo,ImageInfo imageInfo)
        {
            chConfig= config;
            _imageInfo= imageInfo;
            _textInfo= textInfo;
        }
        internal void OnVerticalOffsetChanged(float value) => VerticalOffsetChanged?.Invoke(this, value);
        public int ChIndex => chConfig.Index;
        public String Name 
        { 
            get => name;
            set
            {
                if(value!=name)
                {
                    name= value;
                    _textInfo.Text = value;
                }
            }
        }

        public Boolean NameVisility
        {
            get => nameVisility;
            set
            {
                
                   nameVisility = value;
                   _textInfo.Visibily = value;
            }
            
        }
        public int SkipCount { get => chConfig.SkipCount; set => chConfig.SkipCount = value; }
        public int DrawLength { get => chConfig.DrawLength; set => chConfig.DrawLength = value; }
        public float VerticalOffset
        {
            get => verticalOffset;
            set
            {
                if (verticalOffset != value)
                {
                    verticalOffset = value;
                    OnPropertyChanged(this, nameof(VerticalOffset));
                    OnVerticalOffsetChanged(value);
                }
            }
        }
        public float ValueOffset
        {
            get => chConfig.VerticalOffset;
            set=> chConfig.VerticalOffset = value;
        }
        public float VerticalPos
        {
            get => chConfig.VerticalPos;
            set => chConfig.VerticalPos = value;
        }
        [AllowNull]
        public Object Tag { get; set; }
        public Color Color
        { 
            get => color;
            set
            {
                if(value!=color)
                {
                    color = value;
                    _textInfo.Color = color;
                    chConfig.Color = color.ColorConverToRGBA();
                }
            }
        }
        public Boolean Visibility 
        {
            get => chConfig.Visibily;
            set
            {
                if(value!=chConfig.Visibily)
                {
                    _textInfo.Visibily = value;
                    _imageInfo.Visibily = value;
                    chConfig.Visibily = value;
                }
            }
        }
        public Boolean IsFocused { get; set; }
        public Boolean IsDragged { get; set; }
        public int ZIndex 
        {
            get => zIndex;
            set
            {
                if(value!=zIndex)
                {
                    zIndex = value;
                    chConfig.ZIndex = value;
                    _imageInfo.ZIndex = value;
                }
            }
        }
        public List<int> CursorImages { get; } = new List<int>();
        public int CursorIndex 
        {
            get => cursorIndex;
            set
            {
                Set(ref cursorIndex, value);
            }
        }
        internal int SelectedIndex => (CursorIndex < 0 || CursorIndex >= CursorImages.Count) ? -1 : CursorImages[CursorIndex];
        public CursorLineStyle CursorLineStyle { get => cursorLineStyle; set => Set(ref cursorLineStyle, value); }
        internal void OnDragged(PointF point)=>Dragged?.Invoke(this, point);
        internal void OnMouseDown(PointF point)=>MouseDown?.Invoke(this, point);
        internal void OnMouseUp(PointF point)=> MouseUp?.Invoke(this, point);
        internal void OnSelectionChanged(DigitalPlot plot) => SelectionChanged?.Invoke(this, plot);
    }
}
