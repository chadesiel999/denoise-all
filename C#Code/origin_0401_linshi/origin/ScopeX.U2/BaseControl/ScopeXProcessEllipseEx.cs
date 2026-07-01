using ScopeX.Controls.Common.Default;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class ScopeXProcessEllipseEx : Control, IStylize, ILanguageControl
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public LanguagePattern LanguagePattern { get => LanguagePattern.Ignore; set => _ = value; }

        public Int32 HeaderHeight { get; set; } = 30;

        public Int32 FirstColumnWidth { get; set; } = 70;

        private Color _HeaderBackColor = Color.SteelBlue;
        public Color HeaderBackColor
        {
            get => _HeaderBackColor;
            set
            {
                _HeaderBackColor = value;
                Invalidate();
            }
        }
        private Color _HeaderForeColor = Color.White;
        public Color HeaderForeColor
        {
            get => _HeaderForeColor;
            set
            {
                _HeaderForeColor = value;
                Invalidate();
            }
        }

        private Color _ValueInfoColor = Color.White;
        public Color ValueInfoColor
        {
            get => _ValueInfoColor;
            set
            {
                _ValueInfoColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        [Description("值改变事件"), Category(Const.Category)]
        public event EventHandler ValueChanged;

        /// <summary>
        /// The m back ellipse color
        /// </summary>
        private Color _BackEllipseColor = Color.FromArgb(228, 231, 237);
        /// <summary>
        /// 圆背景色
        /// </summary>
        /// <value>The color of the back ellipse.</value>
        [Description("圆背景色"), Category(Const.Category)]
        public Color BackEllipseColor
        {
            get => _BackEllipseColor;
            set
            {
                _BackEllipseColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m core ellipse color
        /// </summary>
        private Color _InnerEllipseColor = Color.FromArgb(228, 231, 237);
        /// <summary>
        /// 内圆颜色，ShowType=Ring 有效
        /// </summary>
        /// <value>The color of the core ellipse.</value>
        [Description("内圆颜色，ShowType=Ring 有效"), Category(Const.Category)]
        public Color InnerEllipseColor
        {
            get => _InnerEllipseColor;
            set
            {
                _InnerEllipseColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m value color
        /// </summary>
        private Color _ValueColor = Color.FromArgb(255, 77, 59);

        /// <summary>
        /// Gets or sets the color of the value.
        /// </summary>
        /// <value>The color of the value.</value>
        [Description("值圆颜色"), Category(Const.Category)]
        public Color ValueColor
        {
            get => _ValueColor;
            set
            {
                _ValueColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m is show core ellipse border
        /// </summary>
        private Boolean _IsShowInnerEllipseBorder = true;
        /// <summary>
        /// 内圆是否显示边框，ShowType=Ring 有效
        /// </summary>
        /// <value><c>true</c> if this instance is show core ellipse border; otherwise, <c>false</c>.</value>
        [Description("内圆是否显示边框，ShowType=Ring 有效"), Category(Const.Category)]
        public Boolean IsShowInnerEllipseBorder
        {
            get => _IsShowInnerEllipseBorder;
            set
            {
                _IsShowInnerEllipseBorder = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m value type
        /// </summary>
        private ProcessBarValueType _ValueType = ProcessBarValueType.Percent;
        /// <summary>
        /// 值文字类型
        /// </summary>
        /// <value>The type of the value.</value>
        [Description("值文字类型"), Category(Const.Category)]
        public ProcessBarValueType ValueType
        {
            get => _ValueType;
            set
            {
                _ValueType = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m value width
        /// </summary>
        private Int32 _ValueWidth = 30;
        /// <summary>
        /// 外圆值宽度
        /// </summary>
        /// <value>The width of the value.</value>
        [Description("外圆值宽度，ShowType=Ring 有效"), Category(Const.Category)]
        public Int32 ValueWidth
        {
            get => _ValueWidth;
            set
            {
                if (value <= 0 || value > Math.Min(this.Width, this.Height))
                    return;
                _ValueWidth = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m value margin
        /// </summary>
        private Int32 _ValueMargin = 5;
        /// <summary>
        /// 外圆值间距
        /// </summary>
        /// <value>The value margin.</value>
        [Description("外圆值间距"), Category(Const.Category)]
        public Int32 ValueMargin
        {
            get => _ValueMargin;
            set
            {
                if (value < 0 || _ValueMargin >= _ValueWidth)
                    return;
                _ValueMargin = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m maximum value
        /// </summary>
        private Int32 _MaxValue = 100;
        /// <summary>
        /// 最大值
        /// </summary>
        /// <value>The maximum value.</value>
        [Description("最大值"), Category(Const.Category)]
        public Int32 MaxValue
        {
            get => _MaxValue;
            set
            {
                if (value <= 0 || value == _MaxValue)
                    return;
                _MaxValue = value;
                _Value = Math.Clamp(_Value, 0, _MaxValue);//判断当前值是否有效
                Refresh();
            }
        }

        /// <summary>
        /// The m value
        /// </summary>
        private Int32 _Value = 20;
        /// <summary>
        /// 当前值
        /// </summary>
        /// <value>The value.</value>
        [Description("当前值"), Category(Const.Category)]
        public Int32 Value
        {
            get => _Value;
            set
            {
                if (_Value != value)
                {
                    _Value = Math.Clamp(value, 0, _MaxValue);
                    ValueChanged?.Invoke(this, null);
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件显示的文字的字体。
        /// </summary>
        [Description("文字字体"), Category(Const.Category)]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                Refresh();
            }
        }
        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Description("文字颜色"), Category(Const.Category), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Localizable(true)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// The m show type
        /// </summary>
        private ShowType _ShowType = ShowType.Ring;

        /// <summary>
        /// Gets or sets the type of the show.
        /// </summary>
        /// <value>The type of the show.</value>
        [Description("显示类型"), Category(Const.Category)]
        public ShowType ShowType
        {
            get { return _ShowType; }
            set
            {
                _ShowType = value;
                Refresh();
            }
        }

        public override String Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Refresh();
            }
        }

        private Color _EllipseBackColor = Color.White;
        public Color EllipseBackColor
        {
            get => _EllipseBackColor;
            set
            {
                if (_EllipseBackColor != value)
                {
                    _EllipseBackColor = value;
                    Refresh();
                }
            }
        }

        private Color _ValueInfoBackColor = Color.White;
        public Color ValueInfoBackColor
        {
            get => _ValueInfoBackColor;
            set
            {
                if (_ValueInfoBackColor != value)
                {
                    _ValueInfoBackColor = value;
                    Refresh();
                }
            }
        }

        public ScopeXProcessEllipseEx()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            ForeColor = Color.White;
            Size = new Size(260, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // 绘制标题区域
            Rectangle headerrect = new Rectangle(0, 0, Width, HeaderHeight);
            using (SolidBrush brush = new SolidBrush(_HeaderBackColor))
            {
                g.FillRectangle(brush, headerrect);
            }
            headerrect = new Rectangle(0, 1, Width, HeaderHeight - 1);
            // 绘制标题文本
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(g, Text, Font, headerrect, HeaderForeColor, flags);


            // 绘制环形背景区域
            Rectangle circlerect = new Rectangle(0, HeaderHeight - 1, Size.Width, Size.Height - HeaderHeight + 1);
            using (SolidBrush brush = new SolidBrush(EllipseBackColor))
            {
                g.FillRectangle(brush, circlerect);
            }

            // 计算进度条区域
            var intwidth = Math.Min(FirstColumnWidth, Size.Height - HeaderHeight) - 1;
            //底圆
            g.FillEllipse(new SolidBrush(_BackEllipseColor), new Rectangle(new Point(0, HeaderHeight), new Size(intwidth, intwidth)));
            if (_ShowType == ShowType.Ring)
            {
                //中心圆
                var intcore = intwidth - _ValueWidth * 2;
                g.FillEllipse(new SolidBrush(_InnerEllipseColor), new Rectangle(new Point(_ValueWidth, _ValueWidth + HeaderHeight), new Size(intcore, intcore)));
                //中心圆边框
                if (_IsShowInnerEllipseBorder)
                {
                    g.DrawEllipse(new Pen(_ValueColor, 2), new Rectangle(new Point(_ValueWidth + 1, _ValueWidth + HeaderHeight + 1), new Size(intcore - 1, intcore - 1)));
                }
                if (_Value >= 0 && _MaxValue > 0)
                {
                    float fltpercent = _Value / (float)_MaxValue;
                    if (fltpercent > 1)
                    {
                        fltpercent = 1;
                    }
                    var sweepangle = fltpercent * 360;
                    sweepangle = sweepangle < 1f ? 1f : sweepangle;
                    g.DrawArc(new Pen(_ValueColor, _ValueWidth - _ValueMargin * 2), new RectangleF(new Point(_ValueWidth / 2 + _ValueMargin / 4, HeaderHeight + _ValueWidth / 2 + _ValueMargin / 4), new SizeF(intwidth - _ValueWidth - _ValueMargin / 2 + (_ValueMargin == 0 ? 0 : 1), intwidth - _ValueWidth - _ValueMargin / 2 + (_ValueMargin == 0 ? 0 : 1))), -90, sweepangle);

                    var strValueText = _ValueType == ProcessBarValueType.Percent ? fltpercent.ToString("0.0%") : _Value.ToString();
                    SizeF _txtSize = g.MeasureString(strValueText, this.Font);
                    g.DrawString(strValueText, Font, new SolidBrush(this.ForeColor), new PointF((intwidth - _txtSize.Width) / 2 + 1, HeaderHeight + (intwidth - _txtSize.Height) / 2 + 1));
                }
            }
            else
            {
                if (_Value >= 0 && _MaxValue > 0)
                {
                    float fltpercent = _Value / (float)_MaxValue;
                    if (fltpercent > 1)
                    {
                        fltpercent = 1;
                    }

                    g.FillPie(new SolidBrush(_ValueColor), new Rectangle(_ValueMargin, _ValueMargin + HeaderHeight, intwidth - _ValueMargin * 2, intwidth - _ValueMargin * 2), -90, fltpercent * 360);

                    string strValueText = _ValueType == ProcessBarValueType.Percent ? fltpercent.ToString("0%") : _Value.ToString();
                    System.Drawing.SizeF _txtSize = g.MeasureString(strValueText, this.Font);
                    g.DrawString(strValueText, this.Font, new SolidBrush(this.ForeColor), new PointF((intwidth - _txtSize.Width) / 2 + 1, HeaderHeight + (intwidth - _txtSize.Height) / 2 + 1));
                }
            }

            // 绘制进度文本
            var progresstext = $"{Value}/{MaxValue}";
            Rectangle textrect = new Rectangle(
                FirstColumnWidth + 5,
                HeaderHeight,
                Width - FirstColumnWidth - 5,
                Height - HeaderHeight);

            TextRenderer.DrawText(g, progresstext, Font, textrect, ValueInfoColor, flags);
        }
    }
}
