using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class RuntimePlot : BasePlot
    {
        private RuntimeRender _RuntimeRender;
        private VeldridText _VeldridText;
        private CursorPlot _CursorPlot;
        public RuntimePlot(IVeldridContent control) : base(control)
        {
            _RuntimeRender = new RuntimeRender(control);
            _RuntimeRender.CreateResources();
            _VeldridText= new VeldridText(control,true);
            _VeldridText.CreateResources();
            (this as IRender).Children.Add(_VeldridText);
            _CursorPlot= new CursorPlot(control);
            _CursorPlot.VerticalAlignment = VerticalAlignment.Center;
            _CursorPlot.HorizontalAlignment= HorizontalAlignment.Left;
            cursors.Add(_CursorPlot);
        }

        public Color Color
        {
            get => _RuntimeRender.Color;
            set
            {
                if (_RuntimeRender.Color == value) return;
                _RuntimeRender.Color = value;
                _VeldridText.Color = value;
                _CursorPlot.Color = value;
            }
        }
        public Single SampleRate { get => _RuntimeRender.SampleRate; set => _RuntimeRender.SampleRate = value; }
        public CursorLineStyle CursorStyle { get => _CursorPlot.LineStyle; set => _CursorPlot.LineStyle = value; }
        private protected override BaseVeldridRender Renderer => _RuntimeRender;
        public override Single Brightness { get => _RuntimeRender.Brightness; set => _RuntimeRender.Brightness = value; }
        public override String FontName { get => _VeldridText.FontName; set => _VeldridText.FontName = value; }
        public override Single FontSize { get => _VeldridText.FontSize; set => _VeldridText.FontSize = value; }
        public override String FontStyle { get => _VeldridText.FontStyle; set => _VeldridText.FontStyle = value; }
        public override String Label { get => _VeldridText.Text; set => _VeldridText.Text = value; }
        public override Boolean LabelVisibility { get => _VeldridText.Visibily; set => _VeldridText.Visibily = value; }
        public override Single HorizontalOffset { get => _RuntimeRender.HorizontalOffset; set => _RuntimeRender.HorizontalOffset = value; }
        public override Single VerticalOffset 
        { 
            get => _CursorPlot.Local;
            set
            {
                if (_CursorPlot.Local == value) return;
                _CursorPlot.Local = value;
                _VeldridText.Local = new PointF(LineRange.MinX + 120, VerticalOffset - 10);
            }
        }
        public CursorImageCollection CursorImages => _CursorPlot.CursorImages;
        public Int32 CursorIndex { get => _CursorPlot.CursorIndex; set => _CursorPlot.CursorIndex = value; }

        public override LineStyle LineStyle 
        { 
            get => base.LineStyle;
            set
            {
                if (base.LineStyle == value) return;
                base.LineStyle = value;
                _RuntimeRender.PrimitiveTopology = value == LineStyle.Point ? PrimitiveTopology.PointList : PrimitiveTopology.LineStrip;
                _RuntimeRender.Visibily = value != LineStyle.None;
            }
        }
        public UInt32 CacheLenght { get => _RuntimeRender.CacheLenght; set => _RuntimeRender.CacheLenght = value; }
        public void SetData(Single data)=>_RuntimeRender.SetData(data);
        public void ClearCache()=>_RuntimeRender.ClearCache();
    }
}
