using ScopeX.Controls.Common.APIs;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal class ScopeXListPageDropDown : ScopeXDropDown<ScopeXListPage>
    {
        public ScopeXListPageDropDown(ScopeXListPage ScopeXListPage) : base(ScopeXListPage)
        {
        }
        /// <summary>
        /// 绘制控件内容
        /// </summary>
        /// <param name="graphics"></param>
        public override void DrawContext(Graphics graphics)
        {
            if (_owner.RunTimeDisplayItemCount == 0) return;
            ISvgRenderer svgRenderer = SvgRenderer.FromGraphics(graphics);
            List<(string, RectangleF, Color, TextFormatFlags, StringAlignment, StringAlignment, Font)> textinfo = new List<(string, RectangleF, Color, TextFormatFlags, StringAlignment, StringAlignment, Font)>();
            SvgGroup svgGroup = new SvgGroup();
            svgGroup.Children.Add(new SvgRectangle()
            {
                X = 0.5f * _owner.BorderThickness,
                Y = 0.5f * _owner.BorderThickness + HeightOffset,
                Width = _owner.Width - _owner.BorderThickness,
                Height = GetRealityHeight() - _owner.BorderThickness,
                Fill = new SvgColourServer(_owner.BackColor),
                FillOpacity = Backopacity / 100f,
                Stroke = new SvgColourServer(_owner.BorderColor),
                StrokeWidth = new SvgUnit(_owner.BorderThickness),
                StrokeOpacity = 1,
                StrokeDashArray = new SvgUnitCollection(),
            });
            if (_owner.DashArray != null && _owner.DashArray.Length > 0)
            {
                foreach (var val in _owner.DashArray) (svgGroup.Children[^1] as SvgRectangle).StrokeDashArray.Add(val);
            }

            //判断是否显示标头
            if(_owner.ShowHeader)
            {
                svgGroup.Children.Add(new SvgRectangle()
                {
                    X = 0,
                    Y = HeightOffset,
                    Height = _owner.DropedHeaderHeight,
                    Width = _owner.Height,
                    Fill = new SvgColourServer(_owner.HeaderBackColor),
                    FillOpacity = 1,
                });
                if (_owner.ShowIndicator)
                {
                    textinfo.Add(("↑", new RectangleF(1, HeightOffset - 3, 10, _owner.DropedHeaderHeight), Color.White, TextFormatFlags.Top | TextFormatFlags.VerticalCenter, StringAlignment.Center, StringAlignment.Center, _owner.DropedHeaderFont));
                }
                textinfo.Add((_owner.Header, new RectangleF(0, HeightOffset, _owner.Height, _owner.DropedHeaderHeight), _owner.HeaderForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, StringAlignment.Center, StringAlignment.Center, _owner.DropedHeaderFont));
            }

            textinfo.Add((_owner.MeasItemName, new RectangleF(0, HeightOffset, _owner.Width, _owner.MeasNameItemHeight), _owner.DisplayForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, StringAlignment.Center, StringAlignment.Center, _owner.DropedMeasNameItemFont));
            for (int i = 0; i < _owner.RunTimeDisplayItemCount; i++)
            {
                textinfo.Add((_owner.GetDisplayMemberValue(i) + ":", 
                    new RectangleF(0, _owner.DropedValueItemHeight * i + _owner.MeasNameItemHeight + HeightOffset, _owner.Width * _owner.Percentage, _owner.DropedValueItemHeight), 
                    _owner.DisplayForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter, StringAlignment.Far, StringAlignment.Center, _owner.DropedValueFont));
                textinfo.Add((_owner.GetValueMemberValue(i) + "", 
                    new RectangleF(_owner.Width * _owner.Percentage, _owner.DropedValueItemHeight * i + _owner.MeasNameItemHeight + HeightOffset, _owner.Width * (1 - _owner.Percentage), _owner.DropedValueItemHeight),
                    _owner.ValueForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter, StringAlignment.Near, StringAlignment.Center, _owner.DropedValueFont));
            }
            svgGroup.RenderElement(svgRenderer);
            textinfo.ForEach(x =>
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = x.Item5;
                stringFormat.LineAlignment = x.Item6;
                stringFormat.Trimming = StringTrimming.None;
                stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                graphics.DrawString(x.Item1, x.Item7, new SolidBrush(x.Item3), x.Item2, stringFormat);
                //TextRenderer.DrawText(graphics, x.Item1, _owner.Font, Rectangle.Round(x.Item2), x.Item3, x.Item4);
            });
        }

        int GetRealityHeight()
        {
            return _owner.DropedValueItemHeight * _owner.RunTimeDisplayItemCount + _owner.DropedMeasNameItemHeight + 4;
        }
        /// <summary>
        /// 开始重绘控件
        /// 在重绘控件前可以设置控件大小和位置
        /// </summary>
        public override void Refresh()
        {
            var form = _owner.FindForm();
            if (form == null) return;
            APIsStructs.RECT rect = new APIsStructs.RECT();
            APIsUser32.GetWindowRect(_owner.Handle, ref rect);
            var point = form.PointToClient(new Point(rect.left, rect.top));
            X = point.X;
            Y = point.Y + _owner.Height - GetRealityHeight();
            Width = _owner.Width;
            Height = GetRealityHeight();
            base.Refresh();
        }
    }

}
