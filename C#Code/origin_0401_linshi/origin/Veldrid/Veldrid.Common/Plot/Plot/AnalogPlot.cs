using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Veldrid.Common.Plot.Plot;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class AnalogPlot : BasePlot
    {
        private AnalogRender _AnalogRender;
        private NoDataPlot _NoDataPlot;
        //private VeldridText _VeldridText;
        private CursorPlot _CursorPlot;
        private DataRender _DataRender;
        private DashedStyle _DashedStyle;
        public AnalogPlot(IVeldridContent control, uint maxframelenght = 10000, uint maxframeCount = 1, int width = 1000, int height = 200) : base(control)
        {
            _DashedStyle = new DashedStyle();
            _AnalogRender = new AnalogRender(control, maxframelenght, maxframeCount);
            _AnalogRender.CreateResources();
            _AnalogRender.UseCache = false;
            //_VeldridText = new VeldridText(control, false);
            //_VeldridText.CreateResources();
            //(this as IRender).Children.Add(_VeldridText);
            LabelPlot = new LabelPlot(control);

            _CursorPlot = new CursorPlot(control);
            _CursorPlot.HorizontalAlignment = HorizontalAlignment.Left;
            _CursorPlot.VerticalAlignment = VerticalAlignment.Center;
            cursors.Add(_CursorPlot);
            _NoDataPlot = new NoDataPlot(control);
            _NoDataPlot.Visibily = false;
            _DataRender = new DataRender(control, 2000);
            _DataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{ 0},
                            Color = _AnalogRender.Color,
                            Brightness= _AnalogRender.Brightness,
                        },

                    },
                    Primitive = PrimitiveTopology.LineList,
                },
            };
            _DataRender.CreateResources();
            (this as IRender).Children.Add(_DataRender);
            Color = _AnalogRender.Color;
        }
        public StackConfig StackConfig => _AnalogRender.StackConfig;
        public override float Brightness
        {
            get => _AnalogRender.Brightness;
            set
            {
                _AnalogRender.Brightness = value;
                _DataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
            }
        }

        /// <summary>
        /// 帧绘制长度
        /// </summary>
        public Int32 DrawLength { get => _AnalogRender.DrawLength; set => _AnalogRender.DrawLength = value; }

        public uint SkipCount { get => _AnalogRender.SkipCount; set => _AnalogRender.SkipCount = value; }

        //public IReadOnlyList<AnalogPlotConfig> AnalogPlots { get; } = new List<AnalogPlotConfig>();

        ///// <summary>
        ///// 帧长度
        ///// </summary>
        //public uint FrameLength { get => _AnalogRender.FrameLength; set => _AnalogRender.FrameLength = value; }

        /// <summary>
        /// 最大帧长度
        /// </summary>
        public uint MaxFrameLength { get => _AnalogRender.MaxFrameLength;/* set => _AnalogRender.MaxFrameLength = value;*/ }

        /// <summary>
        /// 最大帧数
        /// </summary>
        public uint MaxFrameCount { get => _AnalogRender.MaxFrameCount; set => _AnalogRender.MaxFrameCount = value; }

        public void ClearCache() => _AnalogRender.ClearCache();

        public override String FontName { get => LabelPlot.FontName; set => LabelPlot.FontName = value; }

        public override String FontStyle { get => LabelPlot.FontStyle; set => LabelPlot.FontStyle = value; }

        public override float FontSize { get => LabelPlot.FontSize; set => LabelPlot.FontSize = value; }

        public Boolean UseCache { get => _AnalogRender.UseCache; set => _AnalogRender.UseCache = value; }

        public Boolean SegementActive { private get; set; } = false;

        public Color Color
        {
            get => _AnalogRender.Color;
            set
            {
                _AnalogRender.Color = value;
                _NoDataPlot.Color = value;
                LabelPlot.Color = value;
                _CursorPlot.Color = value;
                _DataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
            }
        }
        public override float VerticalOffset
        {
            get => _CursorPlot.Local;
            set
            {
                if (_CursorPlot.Local != value)
                {
                    _CursorPlot.Local = value;
                    _NoDataPlot.Local = value;
                    OnVerticalOffsetChanged(value);
                }
              //  _VeldridText.Local = new PointF((float)(LineRange.MinX + (36.0 / _VeldridText.Rectangle.Width * LineRange.XLenght)), VerticalOffset - 10);
            }
        }
        public float ValueOffset { get => _AnalogRender.VerticalOffset; set => _AnalogRender.VerticalOffset = value; }
        public CursorLineStyle CursorLineStyle { get => _CursorPlot.LineStyle; set => _CursorPlot.LineStyle = value; }
        public CursorImageCollection CursorImages => _CursorPlot.CursorImages;
        public int CursorIndex { get => _CursorPlot.CursorIndex; set => _CursorPlot.CursorIndex = value; }
        public override String Label { get => LabelPlot.Label; set => LabelPlot.Label = value; }
        public override bool LabelVisibility { get => LabelPlot.Visibily; set => LabelPlot.Visibily = value; }
        public override float HorizontalOffset
        {
            get => _AnalogRender.HorizontalOffset;
            set
            {
                if (_AnalogRender.HorizontalOffset != value)
                {
                    _AnalogRender.HorizontalOffset = value;
                    OnHorizontalOffsetChanged(value);
                }
            }
        }
        private protected override BaseVeldridRender Renderer => _AnalogRender;
        public Boolean NoDataVisibily { get => _NoDataPlot.Visibily; set => _NoDataPlot.Visibily = value; }
        public float SampleRate { get => _AnalogRender.SampleRate; set => _AnalogRender.SampleRate = value; }
        public override LineStyle LineStyle
        {
            get => base.LineStyle;
            set
            {
                base.LineStyle = value;
                _AnalogRender.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
            }
        }
        public float ValueScale { get => _AnalogRender.ValueScale; set => _AnalogRender.ValueScale = value; }
        public float[] Points
        {
            set
            {
                _AnalogRender.Datas = value;
                if (value == null || value.Length == 0)
                {
                    _NoDataPlot.Visibily = true;
                    return;
                }
                _NoDataPlot.Visibily = false;
                CalcDashData(value);
            }
        }
        private void CalcDashData(float[] data)
        {
            float max = data.Max();
            float min = data.Min();
            var framecount = data.Length / _AnalogRender.DrawLength;
            if (framecount <= 1) return;
            if (max == min)
            {
                max += 100;
                min -= 100;
            }
            else
            {
            }
            List<Vector2> points = new List<Vector2>();

            for (int i = 1; i < framecount; i++)
            {
                Vector2 start = new Vector2(LineRange.XLenght / framecount * i, min);
                points.AddRange(Enumerable.Range(0, (int)Math.Ceiling(((max - min) / (_DashedStyle.LineLength + _DashedStyle.DashedLength)))).SelectMany(x =>
                {
                    Vector2[] temps = new Vector2[2];
                    temps[0] = start;

                    temps[1].Y = start.Y + _DashedStyle.LineLength;
                    start.Y = temps[1].Y + _DashedStyle.DashedLength;
                    temps[1].X = temps[0].X;
                    return temps;
                }).ToArray());
            }
            _DataRender.WriteData(0, points.ToArray());
            _DataRender.DataRenderConfigs[0].DataLenght = (uint)points.Count;
            _DataRender.DataRenderConfigs[0].FixedDataLenght = (uint)points.Count;
            _DataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].PointCount = (uint)points.Count;
            _DataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts[0].FixedPointCount = (uint)points.Count;
        }
        public override void Draw()
        {
            if (!Visibily) return;
            if (_NoDataPlot.Visibily)
            {
                _NoDataPlot.Draw();
            }
            _DataRender.Visibily = (_AnalogRender.StackConfig.StackingMode == StackingMode.Stitching) && SegementActive;
            base.Draw();
        }
        protected override void Dispose(bool disposing)
        {
            _NoDataPlot.Dispose();
            base.Dispose(disposing);
        }
    }
}
