using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    /// <summary>
    /// 
    /// Implements the <see cref="System.Windows.Forms.Control" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Control" />
    [DefaultEvent("ValueChanged")]
    public class ProgressBar : Control, IStylize, ILanguageControl
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public LanguagePattern LanguagePattern { get => LanguagePattern.Ignore; set => _ = value; }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        [Description("值变更事件"), Category(Const.Category)]
        public event EventHandler ValueChanged;

        Int32 _Value = 0;

        [Description("当前值"), Category(Const.Category)]
        public Int32 Value
        {
            get => _Value;
            set
            {
                if (value > _MaxValue)
                {
                    _Value = _MaxValue;
                }
                else if (value < MinValue)
                {
                    _Value = 0;
                }
                else
                {
                    _Value = value;
                }
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Refresh();
            }
        }

        private Int32 _MaxValue = 100;

        [Description("最大值"), Category(Const.Category)]
        public Int32 MaxValue
        {
            get => _MaxValue;
            set
            {
                if (value < _Value)
                    _MaxValue = _Value;
                else
                    _MaxValue = value;
                Refresh();
            }
        }

        [Description("透明度"), Category(Const.Category)]
        public Int32 Opacity { get; set; } = 80;

        [Description("最小值"), Category(Const.Category)]
        public Int32 MinValue => 0;

        Color _ValueColor = Color.FromArgb(255, 77, 59);

        [Description("值进度条颜色"), Category(Const.Category)]
        public Color ValueColor
        {
            get => _ValueColor;
            set
            {
                _ValueColor = value;
                Refresh();
            }
        }

        private Color _ValueBackgroundColor = Color.FromArgb(228, 231, 237);

        [Description("值背景色"), Category(Const.Category)]
        public Color ValueBackgroundColor
        {
            get => _ValueBackgroundColor;
            set
            {
                _ValueBackgroundColor = value;
                Refresh();
            }
        }

        private Color _BorderColor = Color.FromArgb(228, 231, 237);

        [Description("边框颜色"), Category(Const.Category)]
        public Color BorderColor
        {
            get => _BorderColor;
            set
            {
                _BorderColor = value;
                Refresh();
            }
        }

        private Int32 _BorderWidth = 1;

        [Description("边框粗细"), Category(Const.Category)]
        public Int32 BorderWidth
        {
            get => _BorderWidth;
            set
            {
                _BorderWidth = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取或设置控件显示的文字的字体。
        /// </summary>
        [Description("值字体"), Category(Const.Category)]
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
        [Description("值字体颜色"), Category(Const.Category)]
        public override System.Drawing.Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                Refresh();
            }
        }

        private ValueTextType _ValueTxtType = ValueTextType.Percent;

        [Description("值显示样式"), Category(Const.Category)]
        public ValueTextType ValueTxtType
        {
            get => _ValueTxtType;
            set
            {
                _ValueTxtType = value;
                Refresh();
            }
        }

        public String _DescriptionInfo = "";
        [Description("描述"), Category(Const.Category)]
        public String DescriptionInfo
        {
            get => _DescriptionInfo;
            set
            {
                _DescriptionInfo = value;
                Refresh();
            }
        }

        public ProgressBar()
        {
            Size = new Size(200, 15);
            ForeColor = Color.White;
            Font = new Font("Arial Unicode MS", 10);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }
        ~ProgressBar()
        {
            if(Font != null)
            {
                Font = null;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            #region SetGDIHigh

            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            #endregion

            Brush sb = new SolidBrush(Color.FromArgb((Int32)(Opacity / 100f * 255), _ValueBackgroundColor));
            g.FillRectangle(sb, new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width - 3, base.ClientRectangle.Height - 2));

            GraphicsPath pathone = CreateRoundedRectanglePath(new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y + 1, base.ClientRectangle.Width - 3, base.ClientRectangle.Height - 4), 2);
            g.DrawPath(new Pen(Color.FromArgb((Int32)(Opacity / 100f * 255), _BorderColor), _BorderWidth), pathone);

            LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, base.ClientRectangle.Height - 3), Color.FromArgb((Int32)(Opacity / 100f * 255), _ValueColor), Color.FromArgb((Int32)(Opacity / 100f * 255), _ValueColor.R, _ValueColor.G, _ValueColor.B));
            g.FillPath(lgb, CreateRoundedRectanglePath(new Rectangle(0, (base.ClientRectangle.Height - (base.ClientRectangle.Height - 3)) / 2, (base.ClientRectangle.Width - 3) * Value / _MaxValue, base.ClientRectangle.Height - 4), 2));

            String strvalue = String.Empty;
            strvalue = _ValueTxtType switch
            {
                ValueTextType.Percent => ((float)Value / (float)_MaxValue).ToString("0%"),
                ValueTextType.Absolute => Value + "/" + _MaxValue,
                _ => ""
            };
            if (!String.IsNullOrEmpty(_DescriptionInfo))
            {
                strvalue = $"{_DescriptionInfo}{strvalue}";
            }
            if (!String.IsNullOrEmpty(strvalue))
            {
                SizeF sizeF = g.MeasureString(strvalue, Font);
                g.DrawString(strvalue, Font, new SolidBrush(Color.FromArgb((Int32)(Opacity / 100f * 255), ForeColor)), new PointF((this.Width - sizeF.Width) / 2, (this.Height - sizeF.Height) / 2 + 1));
            }
        }

        /// <summary>
        /// 根据矩形和圆得到一个圆角矩形Path
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="cornerRadius">The corner radius.</param>
        /// <returns>GraphicsPath.</returns>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, Int32 cornerRadius)
        {
            GraphicsPath roundedrect = new GraphicsPath();
            roundedrect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedrect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedrect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedrect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedrect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedrect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedrect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedrect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedrect.CloseFigure();
            return roundedrect;
        }
    }

    /// <summary>
    /// Enum ValueTextType
    /// </summary>
    public enum ValueTextType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// 百分比
        /// </summary>
        Percent,
        /// <summary>
        /// 数值
        /// </summary>
        Absolute
    }
}
