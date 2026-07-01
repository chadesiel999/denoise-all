using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ScopeX.Controls.Common.Helper;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    public class BarPlot : BasePlot
    {
        internal BarRender BarRender => barRender;
        private BarRender barRender;
        private ImagesRender imagesRender;
        private DataRender dataRender;
        private MutiText labelText;
        private MutiText otherText;
        private MutiText titleText;
        private MutiText infoText;
        private MutiText customText;
        private VeldridText tooltipText;
        public BarPlot(IVeldridContent control, BitmapData bitmap, int maxDataCount = 10000, int plotCount = 1) : base(control)
        {
            barRender = new BarRender(control, maxDataCount, plotCount);
            barRender.CreateResources();
            imagesRender = new ImagesRender(control, bitmap, new SizeF(36, 24), new SizeF(40, 30));
            for (int i = 0; i < plotCount; i++) imagesRender.ImageInfos.Add(new ImageInfo()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            });
            imagesRender.CreateResources();
            dataRender = new DataRender(control);
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs =  Enumerable.Range(0, plotCount).Select(x=>
                    {
                        return new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{ 0},
                            Brightness = 100,
                        };
                    }).ToArray(),
                    Primitive = PrimitiveTopology.LineList,
                }
            };
            dataRender.CreateResources();
            titleText = new MutiText(control);
            titleText.TextInfos = Enumerable.Range(0, plotCount).Select(x => new VeldridRender.TextRender.TextInfo()).ToArray();
            titleText.CreateResources();
            titleText.Visibily = false;
            titleText.FontSize = 16;
            labelText = new VeldridRender.TextRender.MutiText(control);
            labelText.TextInfos = Enumerable.Range(0, plotCount).Select(x => new VeldridRender.TextRender.TextInfo()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            }).ToArray();
            labelText.Visibily = true;
            labelText.CreateResources();
            BarPlots = Enumerable.Range(0, plotCount).Select(x => new BarPlotConfig(this, barRender.BarRenders[x], labelText.TextInfos[x], titleText.TextInfos[x], dataRender.DataRenderConfigs[0].PointConfigs[x], imagesRender.ImageInfos[x])).ToList().AsReadOnly();
            otherText = new MutiText(control);
            otherText.CreateResources();
            infoText = new MutiText(control);
            infoText.CreateResources();
            customText = new MutiText(control);
            customText.CreateResources();
            tooltipText = new VeldridText(control,true,true);
            tooltipText.CreateResources();
            (this as IRender).Children.Add(imagesRender);
            (this as IRender).Children.Add(dataRender);
            (this as IRender).Children.Add(labelText);
            (this as IRender).Children.Add(otherText);
            (this as IRender).Children.Add(titleText);
            (this as IRender).Children.Add(infoText);
            (this as IRender).Children.Add(customText);
            (this as IRender).Children.Add(tooltipText);
        }
        public override float Brightness { get => barRender.Brightness; set => barRender.Brightness = value; }
        public IReadOnlyList<BarPlotConfig> BarPlots { get; }
        public override void Draw()
        {
            if (!Visibily) return;
            if (barRender[nameof(BarRender.Margin), nameof(BarRender.Range)] || barRender.WindowSizeState)
            {
                foreach (var val in BarPlots)
                {
                    val.RefreshAbsValue();
                }
            }
            var datas = BarPlots.SelectMany(x => x.DashData).ToArray();
            dataRender.WriteData(0, datas);
            dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)datas.Length;
            dataRender.DataRenderConfigs[0].DataLenght = (uint)datas.Length;
            otherText.Visibily = false;
            titleText.Visibily = false;
            imagesRender.Visibily = false;
            infoText.TextInfos = BarPlots.Where(x => x.InfoVisibily && x.Infos.Count() > 0).Select(x => new VeldridRender.TextRender.TextInfo()
            {
                BackColor = Color.FromArgb(0,0,0,0),
                Color = x.InfoColor,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Local =new PointF(x.InfoLocal.X, x.InfoLocal.Y-labelText.GetVirtualSize(x.Infos.First()).Y),
                Text = String.Join('\n', x.Infos),
                Visibily = true,
            }).ToArray();
            infoText.Visibily = infoText.TextInfos.Length > 0;
            var color = BarPlots.First().InfoColor;
            customText.TextInfos = BarPlots.Where(x => x.CustomTextVisibily && x.CustomTextInfos.Count > 0).SelectMany(x => x.CustomTextInfos).Select(x => x.TextInfo).ToArray();
            customText.Visibily = customText.TextInfos.Length > 0;
            var lables = customText.TextInfos.ToList();
            RemoveOverlapXLableStr(ref lables);
            foreach (var lable in lables)
            {
                lable.Color = color;
            }
            customText.TextInfos = lables.ToArray();
            if (tooltipText.Visibily)
            {
                if (BarPlots.Any(x => x.Visibily && x.ToolTipIndex >= 0 && x.MouseIn))
                {
                    var plot = BarPlots.First(x => x.Visibily && x.ToolTipIndex >= 0 && x.MouseIn);
                    plot.ToolTipIndex = plot.GetPositionIndex(out var val);
                    if (plot.ToolTipIndex >= 0)
                    {
                        var x1 = ((Decimal)plot.MaxValue - (Decimal)plot.MinValue) / (Decimal)plot.NBinsCount * (Decimal)plot.ToolTipIndex + (Decimal)plot.MinValue;
                        var x2 = ((Decimal)plot.MaxValue - (Decimal)plot.MinValue) / (Decimal)plot.NBinsCount * (Decimal)(plot.ToolTipIndex + 1) + (Decimal)plot.MinValue;
                        SIHelper.ValueChangeToSI((Double)x1, out var unitstr1, out var x1str, 8, plot.Unit);
                        SIHelper.ValueChangeToSI((Double)x2, out var unitstr2, out var x2str, 8, plot.Unit);
                        tooltipText.Text = $"Bin:{plot.ToolTipIndex}\r\nx:{x1str}{unitstr1}-{x2str}{unitstr2}\r\ny:{val}{plot.ValueUnit}";
                        //tooltipText.Text = $"Bin:{plot.ToolTipIndex}\r\nx:{x1}{plot.Unit}-{x2}{plot.Unit}\r\ny:{val}{plot.ValueUnit}";
                    }
                    else
                    {
                        tooltipText.Visibily = false;
                    }
                }
            }
            else
            {
                foreach (var plot in BarPlots.Where(x => x.Visibily&&x.MouseIn).OrderBy(x => x.ZIndex))
                {
                    var pos = imagesRender.ImageInfos[plot.Index].Position;
                    plot.ToolTipIndex = plot.GetPositionIndex(out var val);
                    tooltipText.Visibily = plot.ToolTipIndex >= 0;
                    if (tooltipText.Visibily)
                    {
                        tooltipText.Color = Color.Black;
                        tooltipText.BackColor = Color.FromArgb(150, Color.Gray);
                        tooltipText.Text = $"Bin:{plot.ToolTipIndex}\r\nx:{(plot.MaxValue - plot.MinValue) / plot.NBinsCount * plot.ToolTipIndex + plot.MinValue}{plot.Unit}-{(plot.MaxValue - plot.MinValue) / plot.NBinsCount * (plot.ToolTipIndex+1) + plot.MinValue}{plot.Unit}\r\ny:{val}{plot.ValueUnit}";
                        break;
                    }
                };
            }
            base.Draw();
            DrawTitles();
        }


        /// <summary>
        /// 清除重叠X坐标标签
        /// </summary>
        /// <param name="lables"></param>
        private void RemoveOverlapXLableStr(ref List<VeldridRender.TextRender.TextInfo> lables)
        {
            List<float> xsizes = lables.Select(x =>
            {
                return customText.GetVirtualSize(x.Text).X;
            }).ToList();
            List<float> xend = lables.Zip(xsizes, (a, b) => a.Local.X + b).ToList();

            int num = lables.Where(x => !string.IsNullOrEmpty(x.Text)).Count();
            float Expansion = LineRange.XLenght / xsizes.Sum();
            float OverlapCoef = 1.0f;
            var sum = xsizes.Sum();
            if (sum== 0.0)
                return;

            //总体长度不超过水平坐标绘制区域
            //检查水平坐标标签互相之间是否存在重叠（针对少量密集绘制）

            for (int i = 1; i < xend.Count; i++)
            {
                if (xend[i - 1] > lables[i].Local.X)
                {
                    var overlap = (xend[i - 1] - lables[i].Local.X) / xsizes[i];
                    if (overlap < OverlapCoef)
                    {
                        OverlapCoef = overlap;
                    }
                }
            }

            if (OverlapCoef >= 1.0&& Expansion>=1.2)
                return;

            //1.2以上为完整展示系数
            //1.2以下，0.6以上剔除一半标签

            if ((Expansion < 1.2 && Expansion >= 0.6)|| (OverlapCoef<1.0&& OverlapCoef>=0.6))
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 2 != 0)
                    {
                        lables[i].Text = string.Empty;
                    }
                }
                return;
            }

            if ((Expansion < 0.6 && Expansion >= 0.3)|| (OverlapCoef < 0.6 && OverlapCoef >= 0.3))
            {
                for (int i = 0; i < num; i++)
                {
                    if (i % 4 != 0)
                    {
                        lables[i].Text = string.Empty;
                    }
                }
                return;
            }

            if ((Expansion < 0.3 && Expansion >= 0.1)|| (OverlapCoef < 0.3 && OverlapCoef >= 0.1))
            {
                if((xsizes[0] + xsizes[xsizes.Count - 1])*1.2 < LineRange.XLenght )
                {
                    for (int i = 1; i < num - 1; i++)
                    {
                        lables[i].Text = string.Empty;
                    }
                    return;
                }
                else if (xsizes[xsizes.Count / 2]* 1.2 < LineRange.XLenght)
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (i != xsizes.Count / 2)
                        {
                            lables[i].Text = string.Empty;
                        }
                    }
                    return;
                }

            }

            for (int i = 0; i < num; i++)
            {
                lables[i].Text = string.Empty;
            }
            return;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var val in BarPlots) val.Dispose();
            base.Dispose(disposing);
        }

        public string TitleFontName { get => titleText.FontName; set => titleText.FontName = value; }
        public string TitleFontStyle { get => titleText.FontStyle; set => titleText.FontStyle = value; }
        public float TitleFontSize { get => titleText.FontSize; set => titleText.FontSize = value; }
        private void DrawTitles()
        {

        }
        int selectindex = -1;
        internal override void OnMouseDown(PointF point, ref bool handle)
        {
            return;
            if (!Visibily) return;
            var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
            foreach (var plot in BarPlots.Where(x => x.Visibily).OrderBy(x => x.ZIndex))
            {
                if (handle) return;
                var pos = imagesRender.ImageInfos[plot.Index].Position;
                pos.Y -= size.Height;
                RectangleF rec = new RectangleF(Unsafe.As<Vector2, PointF>(ref pos), size);
                handle = rec.Contains(point);
                if (handle)
                {
                    selectindex = plot.Index;
                    plot.OnMouseDown(point);
                }
            };
        }
        protected override void ActiveMouseDown(object sender, PointF point)
        {
            if (selectindex < 0)
            {
                base.ActiveMouseDown(sender, point);
            }
            else
            {
                BarPlots[selectindex].OnMouseDown(point);
            }
        }
        internal override void OnMouseLeave(ref bool handle)
        {
            foreach(var plot in BarPlots)
            {
                plot.MouseIn = false;
            }
            base.OnMouseLeave(ref handle);
        }
        internal override void OnMouseMove(PointF point, ref bool handle)
        {
            tooltipText.Visibily = false;
            if (!Visibily) return;
            var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
            foreach (var plot in BarPlots.Where(x => x.Visibily).OrderBy(x => x.ZIndex))
            {
                if (handle) return;
                var pos = imagesRender.ImageInfos[plot.Index].Position;
                plot.ToolTipIndex = plot.GetPositionIndex(point,out var val);
                handle = plot.ToolTipIndex>=0;
                if (handle)
                {
                    tooltipText.Color = Color.Black;
                    tooltipText.BackColor = Color.FromArgb(150, Color.Gray);
                    tooltipText.Text = $"Bin:{plot.ToolTipIndex}\r\nx:{((Decimal)plot.MaxValue - (Decimal)plot.MinValue) / (Decimal)plot.NBinsCount * (Decimal)plot.ToolTipIndex + (Decimal)plot.MinValue}{plot.Unit}-{((Decimal)plot.MaxValue - (Decimal)plot.MinValue) / (Decimal)plot.NBinsCount * (Decimal)(plot.ToolTipIndex + 1) + (Decimal)plot.MinValue}{plot.Unit}\r\ny:{val}{plot.ValueUnit}";
                }
                tooltipText.Local = point;
                tooltipText.Visibily = handle;
            };
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            return;
            if (selectindex < 0)
            {
                base.ActiveDragged(sender, point);
            }
            else
            {
                BarPlots[selectindex].OnDragged(point);
            }
        }
        internal override void OnMouseUp(PointF point, ref bool handle)
        {
            selectindex = -1;
            return;
            if (handle)
            {
                return;
            }

            var size = this.LocalSizeToVirtualSize(GraphicsManger.CursorFixedSize.X, GraphicsManger.CursorFixedSize.Y);
            foreach (var plot in BarPlots.Where(x => x.Visibily).OrderBy(x => x.ZIndex))
            {
                var pos = imagesRender.ImageInfos[plot.Index].Position;
                pos.Y -= size.Height;
                RectangleF rec = new RectangleF(Unsafe.As<Vector2, PointF>(ref pos), size);
                handle = rec.Contains(point);
                plot.OnMouseUp(point);
            }
        }

        public String CustomTextFontName { get => customText.FontName; set => customText.FontName = value; }
        public String CustomTextFontStyle { get => customText.FontStyle; set => customText.FontStyle = value; }
        public Single CustomTextFontSize { get => customText.FontSize; set => customText.FontSize = value; }
        public String InfoFontName { get => infoText.FontName; set => infoText.FontName = value; }
        public String InfoFontStyle { get => infoText.FontStyle; set => infoText.FontStyle = value; }
        public Single InfoFontSize { get => infoText.FontSize; set => infoText.FontSize = value; }
        private protected override BaseVeldridRender Renderer => barRender;
        public override string FontName { get => labelText.FontName; set => labelText.FontName = value; }
        public override float FontSize { get => labelText.FontSize; set => labelText.FontSize = value; }
        public override string FontStyle { get => labelText.FontStyle; set => labelText.FontStyle = value; }
    }
}
