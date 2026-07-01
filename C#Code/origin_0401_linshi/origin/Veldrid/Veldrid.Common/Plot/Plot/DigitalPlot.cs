using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;
using Vulkan;

namespace Veldrid.Common.Plot
{
    public class DigitalPlot : BasePlot
    {
        private List<DigitalChPlot> plots = new List<DigitalChPlot>();
        DigitalRender dataRender;
        MutiText veldridText;
        ImagesRender imagesRender;
        readonly SizeF gridSize = new SizeF(30, 30);
        public unsafe DigitalPlot(IVeldridContent control, BitmapData bitmap, int maxDiagitalCount = 16, int chdataCount = 10000) : base(control)
        {
            dataRender = new DigitalRender(control, chdataCount, maxDiagitalCount);
            dataRender.CreateResources();
            veldridText = new MutiText(control);
            veldridText.TextInfos = Enumerable.Range(0, maxDiagitalCount).Select(x => new TextInfo()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Local = new PointF(LineRange.MinX + 120, 200)
            }).ToArray();
            veldridText.CreateResources();
            (this as IRender).Children.Add(veldridText);

            imagesRender = new ImagesRender(control, bitmap, new SizeF(24, 24), gridSize);
            for (int i = 0; i < maxDiagitalCount; i++)
            {
                imagesRender.ImageInfos.Add(new ImageInfo()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Visibily = false,
                    ImageIndex = -1,
                });
            }
            imagesRender.CreateResources();

            (this as IRender).Children.Add(imagesRender);
            for (int i = 0; i < maxDiagitalCount; i++)
            {
                plots.Add(new DigitalChPlot(dataRender.DigitalCh[i], veldridText.TextInfos[i], imagesRender.ImageInfos[i]));
            }
           // plots = Enumerable.Range(0, maxDiagitalCount).Select(x => new DigitalChPlot(dataRender.DigitalCh[x], veldridText.TextInfos[x], imagesRender.ImageInfos[x])).ToList();
            Brightness = 100;
            foreach (var val in plots)
            {
                val.PropertyChanged += Val_PropertyChanged;
            }
        }

        private void Val_PropertyChanged(object sender, string e)
        {
            if (sender is DigitalChPlot plot)
            {
                switch (e)
                {
                    case nameof(DigitalChPlot.Name):
                        veldridText.TextInfos[plot.ChIndex].Text = plot.Name;
                        break;
                    case nameof(DigitalChPlot.Color):
                        veldridText.TextInfos[plot.ChIndex].Color = plot.Color;
                        dataRender.DigitalCh[plot.ChIndex].Color = plot.Color.ColorConverToRGBA();
                        break;
                    case nameof(DigitalChPlot.VerticalOffset):
                        var pointf = new PointF(LineRange.MinX + 120, plot.VerticalOffset + 200);
                        pointf = new PointF(pointf.X, pointf.Y > LineRange.MaxY - 200 ? LineRange.MaxY - 300 : pointf.Y);
                        veldridText.TextInfos[plot.ChIndex].Local = pointf;
                        imagesRender.ImageInfos[plot.ChIndex].Position = new Vector2(LineRange.MinX, plot.VerticalOffset);
                        break;
                    case nameof(DigitalChPlot.Visibility):
                        veldridText.TextInfos[plot.ChIndex].Visibily = plot.Visibility;
                        imagesRender.ImageInfos[plot.ChIndex].Visibily = plot.Visibility;
                        dataRender.DigitalCh[plot.ChIndex].Visibily = plot.Visibility;
                        break;
                    case nameof(DigitalChPlot.CursorIndex):
                        imagesRender.ImageInfos[plot.ChIndex].ImageIndex = plot.SelectedIndex;
                        break;
                    case nameof(DigitalChPlot.ZIndex):
                        imagesRender.ImageInfos[plot.ChIndex].ZIndex = plot.ZIndex;
                        dataRender.DigitalCh[plot.ChIndex].ZIndex = plot.ZIndex;
                        break;
                }
            }
        }
        public void SetData(float[] data) => dataRender.SetData(data);
        public float ValueScale { get => dataRender.ValueScale; set => dataRender.ValueScale = value; }

        public override float HorizontalOffset { get => dataRender.HorizontalOffset; set => dataRender.HorizontalOffset = value; }

        public float SampleRate { get => dataRender.SampleRate; set => dataRender.SampleRate = value; }
        private protected override BaseVeldridRender Renderer => dataRender;
        public override string FontName { get => veldridText.FontName; set => veldridText.FontName = value; }
        public override string FontStyle { get => veldridText.FontStyle; set => veldridText.FontStyle = value; }
        public override float FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }
        public IReadOnlyList<DigitalChPlot> Plots => plots;

        private LineStyle lineStyle = LineStyle.Line;
        public override float Brightness { get => dataRender.Brightness; set => dataRender.Brightness = value; }

        public int ChdataCount
        {
            get => dataRender.ChdataCount;
            set => dataRender.ChdataCount = value;
        }

        public override LineStyle LineStyle
        {
            get => lineStyle;
            set
            {
                lineStyle = value;
                dataRender.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
            }
        }
        protected override void Dispose(bool disposing)
        {
            plots.ForEach(x => x.Dispose());
            plots.Clear();
            base.Dispose(disposing);
        }

        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }
        int selectindex = -1;
        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            if (!Visibily) return;
            var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
            foreach (var plot in Plots.Where(x => x.Visibility).OrderByDescending(x => x.ZIndex))
            {
                if (handle) return;
                var pos = MovePosY(imagesRender.ImageInfos[plot.ChIndex].Position, size);
                RectangleF rec = new RectangleF(Unsafe.As<Vector2, PointF>(ref pos), size);
                handle = rec.Contains(point);
                if (handle)
                {
                    selectindex = plot.ChIndex;
                    plot.OnMouseDown(point);
                }
            };
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            if (selectindex < 0)
            {
                base.ActiveDragged(sender, point);
            }
            else
            {
                Plots[selectindex].OnDragged(point);
            }
        }
        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            if (!Visibily) return;
            var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
            foreach (var plot in Plots.Where(x => x.Visibility).OrderByDescending(x => x.ZIndex))
            {
                if (handle) return;
                var pos = MovePosY(imagesRender.ImageInfos[plot.ChIndex].Position, size);
                RectangleF rec = new RectangleF(Unsafe.As<Vector2, PointF>(ref pos), size);
                handle = rec.Contains(point);
                if (handle)
                {
                    SetCursor(Sdl2.SDL_SystemCursor.Hand);
                }
            };
        }
        internal override void OnMouseUp(PointF point, ref bool handle)
        {
            if (selectindex > -1)
            {
                Plots[selectindex].OnMouseUp(point);
                selectindex = -1;
            }
            else
            {
                var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
                foreach (var plot in Plots.Where(x => x.Visibility).OrderByDescending(x => x.ZIndex))
                {
                    if (handle) return;
                    var pos = MovePosY(imagesRender.ImageInfos[plot.ChIndex].Position, size);
                    RectangleF rec = new RectangleF(Unsafe.As<Vector2, PointF>(ref pos), size);
                    handle = rec.Contains(point);
                    if (handle)
                    {
                        plot.OnMouseUp(point);
                    }
                }
            }
        }

        private Vector2 MovePosY(Vector2 originPos, SizeF cursorFixedSize)
        {
            var cursorGridSize = this.LocalSizeToVirtualSize(gridSize.Width, gridSize.Height);
            return new Vector2(originPos.X, originPos.Y - (cursorFixedSize.Height - cursorGridSize.Height) / 2);
        }
    }

}
