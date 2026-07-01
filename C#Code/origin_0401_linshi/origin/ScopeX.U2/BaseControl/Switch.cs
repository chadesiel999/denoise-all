using ScopeX.Controls.Common.Default;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScopeX.U2
{
    [DefaultEvent("CheckedChanged")]
    public partial class Switch : UserControl, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        /// <summary>
        /// Occurs when [checked changed].
        /// </summary>
        [Description("选中改变事件"), Category("Uni-T")]
        public event EventHandler CheckedChanged;

        private Color _TrueColor = AppStyleConfig.DefaultCheckedBackColor;
        /// <summary>
        /// Gets or sets the color of the true.
        /// </summary>
        /// <value>The color of the true.</value>
        [Description("选中时颜色"), Category("Uni-T")]
        public Color TrueColor
        {
            get => _TrueColor;
            set
            {
                _TrueColor = value;
                Refresh();
            }
        }

        private Color _TrueTextColr = Color.White;
        [Description("选中时文本颜色"), Category("Uni-T")]
        public Color TrueTextColr
        {
            get => _TrueTextColr;
            set
            {
                _TrueTextColr = value;
                Refresh();
            }
        }

        private Color _FalseColor = Color.Black;
        /// <summary>
        /// Gets or sets the color of the false.
        /// </summary>
        /// <value>The color of the false.</value>
        [Description("没有选中时颜色"), Category("Uni-T")]
        public Color FalseColor
        {
            get => _FalseColor;
            set
            {
                _FalseColor = value;
                Refresh();
            }
        }

        private Color _FalseTextColr = Color.White;
        [Description("没有选中时文本颜色"), Category("Uni-T")]

        public Color FalseTextColr
        {
            get => _FalseTextColr;
            set
            {
                _FalseTextColr = value;
                Refresh();
            }
        }

        private Boolean _Checked;
        /// <summary>
        /// Gets or sets a value indicating whether this is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [Description("是否选中"), Category("Uni-T")]
        public Boolean Checked
        {
            get => _Checked;
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    Refresh();
                    CheckedChanged?.Invoke(this, null);
                }
            }
        }

        private String[] _Texts = new String[] { "", "" };
        /// <summary>
        /// Gets or sets the texts.
        /// </summary>
        /// <value>The texts.</value>
        [Description("文本值，当选中或没有选中时显示，必须是长度为2的数组"), Category("Uni-T")]
        public String[] Texts
        {
            get { return _Texts; }
            set
            {
                if (value.Length != 2)
                {
                    return;
                }
                if (_Texts != value)
                {
                    _Texts = value;
                    Refresh();
                }
            }
        }

        private SwitchType _SwitchType = SwitchType.Ellipse;
        /// <summary>
        /// Gets or sets the type of the switch.
        /// </summary>
        /// <value>The type of the switch.</value>
        [Description("显示类型"), Category("Uni-T")]
        public SwitchType SwitchType
        {
            get => _SwitchType;
            set
            {
                if (_SwitchType != value)
                {
                    _SwitchType = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件显示的文字的字体
        /// </summary>
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                Refresh();
            }
        }

        private Cursor _Cursor = Cursors.Hand;
        public override Cursor Cursor
        {
            get => _Cursor;
            set
            {
                if (_Cursor != value)
                {
                    _Cursor = value;
                    base.Cursor = value;
                }
            }
        }

        private Brush _DrawCircleBrush = Brushes.White;

        new public Boolean Enabled
        {
            get => base.Enabled;
            set
            {
                if (base.Enabled != value)
                {
                    base.Enabled = value;
                    if (!base.Enabled)
                    {
                        _FalseColor = _FalseColor.GetBrightnessColor(-0.5);
                        _TrueColor = _TrueColor.GetBrightnessColor(-0.5);
                        _TrueTextColr = _TrueTextColr.GetBrightnessColor(-0.5);
                        _FalseTextColr = _FalseTextColr.GetBrightnessColor(-0.5);
                        _DrawCircleBrush = Brushes.Gray;
                    }
                    else
                    {
                        _FalseColor = _FalseColor.GetBrightnessColor(0.5);
                        _TrueColor = _TrueColor.GetBrightnessColor(0.5);
                        _TrueTextColr = _TrueTextColr.GetBrightnessColor(0.5);
                        _FalseTextColr = _FalseTextColr.GetBrightnessColor(0.5);
                        _DrawCircleBrush = Brushes.White;
                    }
                    Refresh();
                }
            }
        }

        public Switch()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.MouseDown += Switch_MouseDown;
        }

        private void Switch_MouseDown(Object sender, MouseEventArgs e)
        {
            Checked = !Checked;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            switch (_SwitchType)
            {
                case SwitchType.Ellipse:
                    DrawEllipse(g);
                    return;
                case SwitchType.Quadrilateral:
                    DrawQuadrilateral(g);
                    return;
                case SwitchType.Line:
                    DrawLine(g);
                    return;
                default:
                    break;
            }
        }

        private void DrawEllipse(Graphics g)
        {
            Color fillColor = _Checked ? _TrueColor : _FalseColor;
            GraphicsPath path = new GraphicsPath();
            path.AddLine(new Point(this.Height / 2, 1), new Point(this.Width - this.Height / 2, 1));
            path.AddArc(new Rectangle(this.Width - this.Height - 1, 1, this.Height - 2, this.Height - 2), -90, 180);
            path.AddLine(new Point(this.Width - this.Height / 2, this.Height - 1), new Point(this.Height / 2, this.Height - 1));
            path.AddArc(new Rectangle(1, 1, this.Height - 2, this.Height - 2), 90, 180);
            g.FillPath(new SolidBrush(fillColor), path);

            string strText = string.Empty;
            if (_Texts != null && _Texts.Length == 2)
            {
                strText = _Checked ? _Texts[0] : _Texts[1];
            }
            var radius = this.Height - 2 - 4;
            if (_Checked)
            {
                g.FillEllipse(_DrawCircleBrush, new Rectangle(this.Width - this.Height - 1 + 2, 1 + 2, radius, radius));
                if (string.IsNullOrEmpty(strText))
                {
                    g.DrawEllipse(new Pen(Color.White, 2), new Rectangle(radius / 2 - (radius / 2) / 2, (this.Height - 2 - radius / 2) / 2 + 1, radius / 2, radius / 2));
                }
                else
                {
                    System.Drawing.SizeF sizeF = g.MeasureString(strText.Replace(" ", "A"), Font);
                    int intTextY = (this.Height - (int)sizeF.Height) / 2 + 1;
                    g.DrawString(strText, Font, new SolidBrush(_TrueTextColr), new Point(radius / 4, intTextY));
                }
            }
            else
            {
                g.FillEllipse(_DrawCircleBrush, new Rectangle(1 + 2, 1 + 2, radius, radius));
                if (string.IsNullOrEmpty(strText))
                {
                    g.DrawEllipse(new Pen(Color.White, 2), new Rectangle(this.Width - 2 - radius / 2 - (radius / 2) / 2, (this.Height - 2 - radius / 2) / 2 + 1, radius / 2, radius / 2));
                }
                else
                {
                    System.Drawing.SizeF sizeF = g.MeasureString(strText.Replace(" ", "A"), Font);
                    int intTextY = (this.Height - (int)sizeF.Height) / 2 + 1;
                    g.DrawString(strText, Font, new SolidBrush(_FalseTextColr), new Point(this.Width - radius / 4 - (int)sizeF.Width, intTextY));
                }
            }
        }
        private void DrawQuadrilateral(Graphics g)
        {
            var fillColor = _Checked ? _TrueColor : _FalseColor;
            GraphicsPath path = new GraphicsPath();
            int intRadius = 5;
            path.AddArc(0, 0, intRadius, intRadius, 180f, 90f);
            path.AddArc(this.Width - intRadius - 1, 0, intRadius, intRadius, 270f, 90f);
            path.AddArc(this.Width - intRadius - 1, this.Height - intRadius - 1, intRadius, intRadius, 0f, 90f);
            path.AddArc(0, this.Height - intRadius - 1, intRadius, intRadius, 90f, 90f);

            g.FillPath(new SolidBrush(fillColor), path);

            string strText = string.Empty;
            if (_Texts != null && _Texts.Length == 2)
            {
                strText = _Checked ? _Texts[0] : _Texts[1];
            }

            if (_Checked)
            {
                GraphicsPath path2 = new GraphicsPath();
                path2.AddArc(this.Width - this.Height - 1 + 2, 1 + 2, intRadius, intRadius, 180f, 90f);
                path2.AddArc(this.Width - 1 - 2 - intRadius, 1 + 2, intRadius, intRadius, 270f, 90f);
                path2.AddArc(this.Width - 1 - 2 - intRadius, this.Height - 2 - intRadius - 1, intRadius, intRadius, 0f, 90f);
                path2.AddArc(this.Width - this.Height - 1 + 2, this.Height - 2 - intRadius - 1, intRadius, intRadius, 90f, 90f);
                g.FillPath(_DrawCircleBrush, path2);

                if (string.IsNullOrEmpty(strText))
                {
                    g.DrawEllipse(new Pen(Color.White, 2), new Rectangle((this.Height - 2 - 4) / 2 - ((this.Height - 2 - 4) / 2) / 2, (this.Height - 2 - (this.Height - 2 - 4) / 2) / 2 + 1, (this.Height - 2 - 4) / 2, (this.Height - 2 - 4) / 2));
                }
                else
                {
                    System.Drawing.SizeF sizeF = g.MeasureString(strText.Replace(" ", "A"), Font);
                    int intTextY = (this.Height - (int)sizeF.Height) / 2 + 2;
                    g.DrawString(strText, Font, new SolidBrush(_TrueTextColr), new Point((this.Height - 2 - 4) / 2, intTextY));
                }
            }
            else
            {
                GraphicsPath path2 = new GraphicsPath();
                path2.AddArc(1 + 2, 1 + 2, intRadius, intRadius, 180f, 90f);
                path2.AddArc(this.Height - 2 - intRadius, 1 + 2, intRadius, intRadius, 270f, 90f);
                path2.AddArc(this.Height - 2 - intRadius, this.Height - 2 - intRadius - 1, intRadius, intRadius, 0f, 90f);
                path2.AddArc(1 + 2, this.Height - 2 - intRadius - 1, intRadius, intRadius, 90f, 90f);
                g.FillPath(_DrawCircleBrush, path2);

                //g.FillEllipse(Brushes.White, new Rectangle(1 + 2, 1 + 2, this.Height - 2 - 4, this.Height - 2 - 4));
                if (string.IsNullOrEmpty(strText))
                {
                    g.DrawEllipse(new Pen(Color.White, 2), new Rectangle(this.Width - 2 - (this.Height - 2 - 4) / 2 - ((this.Height - 2 - 4) / 2) / 2, (this.Height - 2 - (this.Height - 2 - 4) / 2) / 2 + 1, (this.Height - 2 - 4) / 2, (this.Height - 2 - 4) / 2));
                }
                else
                {
                    System.Drawing.SizeF sizeF = g.MeasureString(strText.Replace(" ", "A"), Font);
                    int intTextY = (this.Height - (int)sizeF.Height) / 2 + 2;
                    g.DrawString(strText, Font, new SolidBrush(_FalseTextColr), new Point(this.Width - 2 - (this.Height - 2 - 4) / 2 - ((this.Height - 2 - 4) / 2) / 2 - (int)sizeF.Width / 2, intTextY));
                }
            }
        }
        private void DrawLine(Graphics g)
        {
            var fillColor = _Checked ? _TrueColor : _FalseColor;
            int intLineHeight = (this.Height - 2 - 4) / 2;

            GraphicsPath path = new GraphicsPath();
            path.AddLine(new Point(this.Height / 2, (this.Height - intLineHeight) / 2), new Point(this.Width - this.Height / 2, (this.Height - intLineHeight) / 2));
            path.AddArc(new Rectangle(this.Width - this.Height / 2 - intLineHeight - 1, (this.Height - intLineHeight) / 2, intLineHeight, intLineHeight), -90, 180);
            path.AddLine(new Point(this.Width - this.Height / 2, (this.Height - intLineHeight) / 2 + intLineHeight), new Point(this.Width - this.Height / 2, (this.Height - intLineHeight) / 2 + intLineHeight));
            path.AddArc(new Rectangle(this.Height / 2, (this.Height - intLineHeight) / 2, intLineHeight, intLineHeight), 90, 180);
            g.FillPath(new SolidBrush(fillColor), path);

            if (_Checked)
            {
                g.FillEllipse(new SolidBrush(fillColor), new Rectangle(this.Width - this.Height - 1 + 2, 1 + 2, this.Height - 2 - 4, this.Height - 2 - 4));
                g.FillEllipse(_DrawCircleBrush, new Rectangle(this.Width - 2 - (this.Height - 2 - 4) / 2 - ((this.Height - 2 - 4) / 2) / 2 - 4, (this.Height - 2 - (this.Height - 2 - 4) / 2) / 2 + 1, (this.Height - 2 - 4) / 2, (this.Height - 2 - 4) / 2));
            }
            else
            {
                g.FillEllipse(new SolidBrush(fillColor), new Rectangle(1 + 2, 1 + 2, this.Height - 2 - 4, this.Height - 2 - 4));
                g.FillEllipse(_DrawCircleBrush, new Rectangle((this.Height - 2 - 4) / 2 - ((this.Height - 2 - 4) / 2) / 2 + 4, (this.Height - 2 - (this.Height - 2 - 4) / 2) / 2 + 1, (this.Height - 2 - 4) / 2, (this.Height - 2 - 4) / 2));
            }
        }
    }
    /// <summary>
    /// Enum SwitchType
    /// </summary>
    public enum SwitchType
    {
        /// <summary>
        /// 椭圆
        /// </summary>
        Ellipse,
        /// <summary>
        /// 四边形
        /// </summary>
        Quadrilateral,
        /// <summary>
        /// 横线
        /// </summary>
        Line
    }
}
