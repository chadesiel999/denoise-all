using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class UestcLoadingCircle : Control
    {
        #region 常数

        private const Double NUMBER_OF_DEGREES_IN_CIRCLE = 360;//圆圈
        private const Double NUMBER_OF_DEGREES_IN_HARF_CIRCLE = NUMBER_OF_DEGREES_IN_CIRCLE / 2;//半圈
        private const Int32 DEFAULT_INNER_RADUIS = 8;//默认的内圈半径
        private const Int32 DEFAULT_OUTER_RADIUS = 10;//默认的外圈半径
        private const Int32 DEFAULT_NUMBER_OF_SPOKE = 10;//默认的辐条数量
        private const Int32 DEFAULT_SPOKE_THICKNESS = 4;//默认的辐条粗细
        private readonly Color DEFAULT_COLOR = Color.DarkGray;//系统定义的颜色

        private const Int32 MACOSX_INNER_RADUIS = 5;//MacOSX浏览器等待加载圆环
        private const Int32 MACOSX_OUTER_RADIUS = 11;
        private const Int32 MACOSX_NUMBER_OF_SPOKE = 12;
        private const Int32 MACOSX_SPOKE_THICKNESS = 2;

        private const Int32 FIREFOX_INNER_RADUIS = 6;//FireFox浏览器等待加载圆环
        private const Int32 FIREFOX_OUTER_RADIUS = 7;
        private const Int32 FIREFOX_NUMBER_OF_SPOKE = 9;
        private const Int32 FIREFOX_SPOKE_THICKNESS = 4;

        private const Int32 IE7_INNER_RADUIS = 8;//IE7浏览器等待加载圆环
        private const Int32 IE7_OUTER_RADIUS = 9;
        private const Int32 IE7_NUMBER_OF_SPOKE = 24;
        private const Int32 IE7_SPOKE_THICKNESS = 4;

        #endregion 常数

        #region 枚举

        public enum StylePresets
        {
            [Description("MacOs系统风格")]
            MacOSX,
            [Description("Firefox浏览器系统风格")]
            Firefox,
            [Description("ie7浏览器系统风格")]
            IE7,
            [Description("普通风格")]
            Custom
        }

        #endregion 枚举

        #region 局部变量

        private Boolean _IsTimerRunning;
        private System.Threading.Timer _Timer;//计时器
        private Int32 _ProgressValue;//------------------------
        private PointF _CenterPoint;//二维平面的点
        private Color[] _Colors;
        private Double[] _Angles;

        #endregion 局部变量

        #region 属性

        private Color _SpokeColor;
        /// <summary>
        /// 获取和设置控件高亮色
        /// </summary>
        /// <value>高亮色</value>
        [TypeConverter("System.Drawing.ColorConverter"), Category("LoadingCircle"), Description("获取和设置控件高亮色")]
        public Color SpokeColor
        {
            get => _SpokeColor;
            set
            {
                if (_SpokeColor != value)
                {
                    _SpokeColor = value;
                    GenerateColorsPallet();
                    Invalidate();
                }
            }
        }

        private Int32 _OuterCircleRadius;//外圈
        /// <summary>
        /// 获取和设置外围半径
        /// </summary>
        /// <value>外围半径</value>
        [System.ComponentModel.Description("获取和设置外围半径"), System.ComponentModel.Category("LoadingCircle")]
        public Int32 OuterCircleRadius
        {
            get
            {
                if (_OuterCircleRadius <= 0)
                {
                    _OuterCircleRadius = DEFAULT_OUTER_RADIUS;
                }
                return _OuterCircleRadius;
            }
            set
            {
                if (_OuterCircleRadius != value)
                {
                    _OuterCircleRadius = value;
                    Invalidate();
                }
            }
        }

        private Int32 _InnerCircleRadius;//内圈
        /// <summary>
        /// 获取和设置内圆半径
        /// </summary>
        /// <value>内圆半径</value>
        [System.ComponentModel.Description("获取和设置内圆半径"), System.ComponentModel.Category("LoadingCircle")]
        public Int32 InnerCircleRadius
        {
            get
            {
                if (_InnerCircleRadius <= 0)
                {
                    _InnerCircleRadius = DEFAULT_INNER_RADUIS;
                }
                return _InnerCircleRadius;
            }
            set
            {
                if (_InnerCircleRadius != value)
                {
                    _InnerCircleRadius = value;
                    Invalidate();
                }
            }
        }


        private Int32 _NumberOfSpoke;//辐条数量
        /// <summary>
        /// 获取和设置辐条数量
        /// </summary>
        /// <value>辐条数量</value>
        [System.ComponentModel.Description("获取和设置辐条数量"), System.ComponentModel.Category("LoadingCircle")]
        public Int32 NumberSpoke
        {
            get
            {
                if (_NumberOfSpoke <= 0)
                {
                    _NumberOfSpoke = DEFAULT_NUMBER_OF_SPOKE;
                }
                return _NumberOfSpoke;
            }
            set
            {
                if (_NumberOfSpoke != value && _NumberOfSpoke > 0)
                {
                    _NumberOfSpoke = value;
                    GenerateColorsPallet();
                    GetSpokesAngles();
                    Invalidate();
                }
            }
        }

        private Boolean _Active;
        /// <summary>
        /// 获取和设置一个布尔值，表示当前控件<see cref="T:LoadingCircle"/>是否激活。
        /// </summary>
        /// <value><c>true</c> 表示激活；否则，为<c>false</c>。</value>
        [System.ComponentModel.Description("获取和设置一个布尔值，表示当前控件是否激活。"), System.ComponentModel.Category("LoadingCircle")]
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    StartTimer();
                }
            }
        }

        private Int32 _SpokeThickness;//辐条粗细
        /// <summary>
        /// 获取和设置辐条粗细程度。
        /// </summary>
        /// <value>辐条粗细值</value>
        [System.ComponentModel.Description("获取和设置辐条粗细程度。"), System.ComponentModel.Category("LoadingCircle")]
        public Int32 SpokeThickness
        {
            get
            {
                if (_SpokeThickness <= 0)
                {
                    _SpokeThickness = DEFAULT_SPOKE_THICKNESS;
                }
                return _SpokeThickness;
            }
            set
            {
                if (_SpokeThickness != value)
                {
                    _SpokeThickness = value;
                    Invalidate();
                }
            }
        }

        private Int32 _RotationSpeedms = 100;
        /// <summary>
        /// 获取和设置旋转速度。
        /// </summary>
        /// <value>旋转速度</value>
        [System.ComponentModel.Description("获取和设置旋转速度。"), System.ComponentModel.Category("LoadingCircle")]
        public Int32 RotationSpeedms
        {
            get => _RotationSpeedms;
            set
            {
                if (value > 0 && _RotationSpeedms != value)
                {
                    _RotationSpeedms = value;
                }
            }
        }

        private StylePresets _StylePreset;//枚举的浏览器
        /// <summary>
        /// 快速设置预定义风格。
        /// </summary>
        /// <value>风格的值</value>
        [Category("LoadingCircle"), Description("快速设置预定义风格。"), DefaultValue(typeof(StylePresets), "Custom")]
        public StylePresets StylePreset
        {
            get => _StylePreset;
            set
            {
                if (_StylePreset != value)
                {
                    _StylePreset = value;
                    switch (_StylePreset)
                    {
                        case StylePresets.MacOSX:
                            SetCircleAppearance(MACOSX_NUMBER_OF_SPOKE, MACOSX_SPOKE_THICKNESS, MACOSX_INNER_RADUIS, MACOSX_OUTER_RADIUS);
                            break;
                        case StylePresets.Firefox:
                            SetCircleAppearance(FIREFOX_NUMBER_OF_SPOKE, FIREFOX_SPOKE_THICKNESS, FIREFOX_INNER_RADUIS, FIREFOX_OUTER_RADIUS);
                            break;
                        case StylePresets.IE7:
                            SetCircleAppearance(IE7_NUMBER_OF_SPOKE, IE7_SPOKE_THICKNESS, IE7_INNER_RADUIS, IE7_OUTER_RADIUS);
                            break;
                        case StylePresets.Custom:
                            SetCircleAppearance(DEFAULT_NUMBER_OF_SPOKE, DEFAULT_SPOKE_THICKNESS, DEFAULT_INNER_RADUIS, DEFAULT_OUTER_RADIUS);
                            break;
                    }
                }
            }
        }

        #endregion 属性

        #region 构造函数及事件处理

        public UestcLoadingCircle()
        {
            // 启用双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            _SpokeColor = DEFAULT_COLOR;
            GenerateColorsPallet();
            GetSpokesAngles();
            GetControlCenterPoint();
            //_Timer = new(TimerCallback, null, 0, _RotationSpeedms);
            Resize += new EventHandler(LoadingCircle_Resize);
        }

        public override void Refresh()
        {
            //base.Refresh();
            GenerateColorsPallet();
            Invalidate();
        }

        private void LoadingCircle_Resize(Object sender, EventArgs e)
        {
            GetControlCenterPoint();
        }

        private void TimerCallback(Object sender)
        {
            if (this.IsDisposed || this.Disposing)
            {
                return;
            }
            try
            {
                // 在 UI 线程上更新进度条
                if (InvokeRequired)
                {
                    if (this.IsDisposed || this.Disposing)
                    {
                        return; // 再次确认，避免销毁后操作
                    }
                    Invoke(new Action(() => { UpdateProgress(); }));
                }
                else
                {
                    UpdateProgress();
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        // 更新进度的方法
        private void UpdateProgress()
        {
            _ProgressValue = ++_ProgressValue % _NumberOfSpoke;
            Invalidate(); // 刷新控件
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_NumberOfSpoke > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

                var intposition = _ProgressValue;
                for (var intcounter = 0; intcounter < _NumberOfSpoke; intcounter++)
                {
                    intposition = intposition % _NumberOfSpoke;
                    DrawLine(e.Graphics,
                             GetCoordinate(_CenterPoint, _InnerCircleRadius, _Angles[intposition]),
                             GetCoordinate(_CenterPoint, _OuterCircleRadius, _Angles[intposition]),
                             _Colors[intcounter], _SpokeThickness);
                    intposition++;
                }
            }

            base.OnPaint(e);
        }

        #endregion

        #region 局部方法

        private Color Darken(Color objColor, Int32 intPercent)
        {
            Int32 intred = objColor.R;
            Int32 intgreen = objColor.G;
            Int32 intblue = objColor.B;
            return Color.FromArgb(intPercent, Math.Min(intred, Byte.MaxValue), Math.Min(intgreen, Byte.MaxValue), Math.Min(intblue, Byte.MaxValue));
        }

        private void GenerateColorsPallet()
        {
            _Colors = GenerateColorsPallet(_SpokeColor, Active, _NumberOfSpoke);
        }

        private Color[] GenerateColorsPallet(Color objColor, Boolean ShadeColor, Int32 numberofSpoke)
        {
            Color[] objcolors = new Color[NumberSpoke];
            Byte bytincrement = (Byte)(Byte.MaxValue / NumberSpoke);
            Byte percentage_of_darken = 0;
            for (var intCursor = 0; intCursor < NumberSpoke; intCursor++)
            {
                if (ShadeColor)
                {
                    if (intCursor == 0 || intCursor < NumberSpoke - numberofSpoke)
                    {
                        objcolors[intCursor] = objColor;
                    }
                    else
                    {
                        percentage_of_darken += bytincrement;
                        if (percentage_of_darken > Byte.MaxValue)
                        {
                            percentage_of_darken = Byte.MaxValue;
                        }
                        objcolors[intCursor] = Darken(objColor, percentage_of_darken);
                    }
                }
                else
                {
                    objcolors[intCursor] = objColor;
                }
            }
            return objcolors;
        }

        private void GetControlCenterPoint()
        {
            _CenterPoint = GetControlCenterPoint(this);
        }

        private PointF GetControlCenterPoint(Control control)
        {
            return new PointF(control.Width / 2, control.Height / 2 - 1);
        }

        private void DrawLine(Graphics graphics, PointF pointOne, PointF pointTwo, Color color, Int32 lineThickness)
        {
            using (Pen objpen = new Pen(new SolidBrush(color), lineThickness))
            {
                objpen.StartCap = LineCap.Round;
                objpen.EndCap = LineCap.Round;
                graphics.DrawLine(objpen, pointOne, pointTwo);
            }
        }

        private PointF GetCoordinate(PointF circleCenter, Int32 radius, Double angle)
        {
            Double currentangle = Math.PI * angle / NUMBER_OF_DEGREES_IN_HARF_CIRCLE;
            return new PointF(circleCenter.X + radius * (float)Math.Cos(currentangle), circleCenter.Y + radius * (float)Math.Sin(currentangle));
        }

        private void GetSpokesAngles()
        {
            if (_Angles == null || _Angles.Length != NumberSpoke)
            {
                _Angles = GetSpokesAngles(NumberSpoke);
            }
        }

        private Double[] GetSpokesAngles(Int32 numberSpoke)
        {
            Double[] angles = new Double[numberSpoke];
            Double dblangle = NUMBER_OF_DEGREES_IN_CIRCLE / numberSpoke;
            for (var shtCounter = 0; shtCounter < numberSpoke; shtCounter++)
            {
                angles[shtCounter] = (shtCounter == 0 ? dblangle : angles[shtCounter - 1] + dblangle);
            }
            return angles;
        }

        private void StartTimer()
        {
            if (_Active)
            {
                if (_IsTimerRunning)
                    return;
                _Timer = new(TimerCallback, null, 0, _RotationSpeedms); // 每秒执行一次
                _IsTimerRunning = true;
            }
            else
            {
                StopTimer();
            }
            GenerateColorsPallet();
            Invalidate();
        }

        private void StopTimer()
        {
            if (!_IsTimerRunning)
                return;  // 如果定时器已经停止，不做任何操作
            _Timer.Change(Timeout.Infinite, Timeout.Infinite); // 停止定时器
            _IsTimerRunning = false;
            _ProgressValue = 0;
            if (_Timer != null)
            {
                _Timer.Dispose();
                _Timer = null;
            }
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (_Timer != null)
                {
                    StopTimer();
                    _Timer?.Dispose();
                    _Timer = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region 全局方法

        /// <summary>
        /// 获取适合控件区域的矩形大小。
        /// </summary>
        /// <param name="proposedSize">The custom-sized area for a control.</param>
        /// <returns>
        /// An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.
        /// </returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Width = (_OuterCircleRadius + _SpokeThickness) * 2;
            return proposedSize;
        }

        /// <summary>
        /// 设置控件的外观
        /// </summary>
        /// <param name="numberSpoke">条数</param>
        /// <param name="spokeThickness">粗细</param>
        /// <param name="innerCircleRadius">内圆半径</param>
        /// <param name="outerCircleRadius">外圆半径</param>
        public void SetCircleAppearance(int numberSpoke, int spokeThickness, int innerCircleRadius, int outerCircleRadius)
        {
            NumberSpoke = numberSpoke;
            SpokeThickness = spokeThickness;
            InnerCircleRadius = innerCircleRadius;
            OuterCircleRadius = outerCircleRadius;

            Invalidate();
        }

        #endregion
    }
}