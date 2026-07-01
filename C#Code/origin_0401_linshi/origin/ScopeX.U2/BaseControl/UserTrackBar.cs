using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    /// <summary>
    /// Class UserTrackBar.
    /// Implements the <see cref="System.Windows.Forms.Control" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Control" />
    [DefaultEvent("ValueChanged")]
    public class UserTrackBar : Control
    {
        #region 私有字段

        /// <summary>
        /// The m line rectangle
        /// </summary>
        private RectangleF _LineRectangle;
        /// <summary>
        /// The m track rectangle
        /// </summary>
        private RectangleF _TrackRectangle;

        /// <summary>
        /// The BLN down
        /// </summary>
        private Boolean _BlnDown = false;

        /// <summary>
        /// The FRM tips
        /// </summary>
        private ScopeX.U2.FrmAnchorTips _FrmTips = null;

        #endregion

        #region 属性

        public override Cursor Cursor { get; set; } = Cursors.Hand;

        /// <summary>
        /// The dcimal digits
        /// </summary>
        private Int32 _DcimalDigits = 0;

        /// <summary>
        /// Gets or sets the dcimal digits.
        /// </summary>
        /// <value>The dcimal digits.</value>
        [Description("值小数精确位数"), Category("自定义")]
        public Int32 DcimalDigits
        {
            get { return _DcimalDigits; }
            set { _DcimalDigits = value; }
        }

        /// <summary>
        /// The line width
        /// </summary>
        private float _LineWidth = 10;

        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        /// <value>The width of the line.</value>
        [Description("线宽度"), Category("自定义")]
        public float LineWidth
        {
            get { return _LineWidth; }
            set { _LineWidth = value; }
        }

        /// <summary>
        /// The minimum value
        /// </summary>
        private float _MinValue = 0;

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        [Description("最小值"), Category("自定义")]
        public float MinValue
        {
            get { return _MinValue; }
            set
            {
                if (_MinValue > _Value)
                    return;
                _MinValue = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// The maximum value
        /// </summary>
        private float _MaxValue = 100;

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        [Description("最大值"), Category("自定义")]
        public float MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (value < _Value)
                    return;
                _MaxValue = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// The m value
        /// </summary>
        private float _Value = 0;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Description("值"), Category("自定义")]
        public float Value
        {
            get { return _Value; }
            set
            {
                if (value > _MaxValue || value < _MinValue || value == _Value)
                    return;
                var v = (float)Math.Round((Double)value, DcimalDigits);
                if (_Value == v)
                    return;
                _Value = v;
                Invoke(() => Refresh());
                _Stopwatch.Restart();
                if (!_IsRunning)
                {
                    _IsRunning = true;
                    Task.Run(() =>
                    {
                        while (_Stopwatch.ElapsedMilliseconds < 50)
                        {
                            Thread.Sleep(10);
                        }
                        ValueChanged?.Invoke(this, EventArgs.Empty);
                        _IsRunning = false;
                        _Stopwatch.Stop();
                    });
                }
            }
        }

        /// <summary>
        /// The m line color
        /// </summary>
        private Color _LineColor = Color.FromArgb(228, 231, 237);

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        [Description("线颜色"), Category("自定义")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                _LineColor = value;
                this.Refresh();
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
        [Description("值颜色"), Category("自定义")]
        public Color ValueColor
        {
            get { return _ValueColor; }
            set
            {
                _ValueColor = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// The is show tips
        /// </summary>
        private Boolean _IsShowTips = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is show tips.
        /// </summary>
        /// <value><c>true</c> if this instance is show tips; otherwise, <c>false</c>.</value>
        [Description("点击滑动时是否显示数值提示"), Category("自定义")]
        public Boolean IsShowTips
        {
            get { return _IsShowTips; }
            set { _IsShowTips = value; }
        }

        /// <summary>
        /// Gets or sets the tips format.
        /// </summary>
        /// <value>The tips format.</value>
        [Description("显示数值提示的格式化形式"), Category("自定义")]
        public String TipsFormat { get; set; }

        #endregion

        #region 事件

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        [Description("值改变事件"), Category("自定义")]
        public event EventHandler ValueChanged;

        #endregion

        private Stopwatch _Stopwatch = new Stopwatch();
        private Boolean _IsRunning = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTrackBar" /> class.
        /// </summary>
        public UserTrackBar()
        {
            this.Size = new Size(250, 30);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            //this.MouseDown += UCTrackBar_MouseDown;
            //this.MouseMove += UCTrackBar_MouseMove;
            //this.MouseUp += UCTrackBar_MouseUp;
        }

        #region private

        /// <summary>
        /// Handles the MouseDown event of the UCTrackBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void UCTrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (_LineRectangle.Contains(e.Location) || _TrackRectangle.Contains(e.Location))
            {
                _BlnDown = true;
                Value = _MinValue + (e.Location.X / (float)this.Width) * (_MaxValue - _MinValue);
                ShowTips();
            }
        }
        /// <summary>
        /// Handles the MouseMove event of the UCTrackBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void UCTrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_BlnDown)
            {
                Value = _MinValue + (e.Location.X / (float)this.Width) * (_MaxValue - _MinValue);
                ShowTips();
            }
        }
        /// <summary>
        /// Handles the MouseUp event of the UCTrackBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void UCTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _BlnDown = false;

            if (_FrmTips != null && !_FrmTips.IsDisposed)
            {
                _FrmTips.Close();
                _FrmTips = null;
            }
        }

        /// <summary>
        /// Shows the tips.
        /// </summary>
        private void ShowTips()
        {
            if (IsShowTips)
            {
                String strValue = Value.ToString();
                if (!String.IsNullOrEmpty(TipsFormat))
                {
                    try
                    {
                        strValue = Value.ToString(TipsFormat);
                    }
                    catch { }
                }
                var p = this.PointToScreen(new Point((Int32)_TrackRectangle.X, (Int32)_TrackRectangle.Y));

                if (_FrmTips == null || _FrmTips.IsDisposed || !_FrmTips.Visible)
                {
                    _FrmTips = ScopeX.U2.FrmAnchorTips.ShowTips(new Rectangle(p.X, p.Y, (Int32)_TrackRectangle.Width, (Int32)_TrackRectangle.Height), strValue, ScopeX.U2.AnchorTipsLocation.TOP, ValueColor, autoCloseTime: -1);
                }
                else
                {
                    _FrmTips.RectControl = new Rectangle(p.X, p.Y, (Int32)_TrackRectangle.Width, (Int32)_TrackRectangle.Height);
                    _FrmTips.StrMsg = strValue;
                }
            }
        }

        #endregion

        #region override

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_BlnDown)
            {
                Int32 locationx = 0;
                if (e.Location.X < 0)
                {
                    locationx = 0;
                }
                else if (e.Location.X > Width)
                {
                    locationx = Width;
                }
                else
                {
                    locationx = e.Location.X;
                }
                Value = _MinValue + (locationx / (float)this.Width) * (_MaxValue - _MinValue);
                ShowTips();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _BlnDown = true;
            if (_LineRectangle.Contains(e.Location) || _TrackRectangle.Contains(e.Location))
            {
                Value = _MinValue + (e.Location.X / (float)this.Width) * (_MaxValue - _MinValue);
                ShowTips();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _BlnDown = false;

            if (_FrmTips != null && !_FrmTips.IsDisposed)
            {
                _FrmTips.Close();
                _FrmTips = null;
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handles the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            _LineRectangle = new RectangleF(LineWidth, (this.Size.Height - LineWidth) / 2, this.Size.Width - LineWidth * 2, LineWidth);
            GraphicsPath pathLine = CreateRoundedRectanglePath(_LineRectangle, 5);
            g.FillPath(new SolidBrush(LineColor), pathLine);

            GraphicsPath valueLine = CreateRoundedRectanglePath(new RectangleF(LineWidth, (this.Size.Height - LineWidth) / 2, ((float)(_Value - _MinValue) / (float)(_MaxValue - _MinValue)) * _LineRectangle.Width, LineWidth), 5);
            g.FillPath(new SolidBrush(ValueColor), valueLine);

            _TrackRectangle = new RectangleF(_LineRectangle.Left - LineWidth + (((float)(_Value - _MinValue) / (float)(_MaxValue - _MinValue)) * (this.Size.Width - LineWidth * 2)), (this.Size.Height - LineWidth * 2) / 2, LineWidth * 2, LineWidth * 2);
            g.FillEllipse(new SolidBrush(ValueColor), _TrackRectangle);
            g.FillEllipse(Brushes.White, new RectangleF(_TrackRectangle.X + _TrackRectangle.Width / 4, _TrackRectangle.Y + _TrackRectangle.Height / 4, _TrackRectangle.Width / 2, _TrackRectangle.Height / 2));
        }
        /// <summary>
        /// Creates the rounded rectangle path.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="cornerRadius">The corner radius.</param>
        /// <returns>GraphicsPath.</returns>
        private GraphicsPath CreateRoundedRectanglePath(RectangleF rect, Int32 cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        #endregion
    }
}
