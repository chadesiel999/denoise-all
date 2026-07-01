using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class CircularProgressBar : Panel
    {

        public CircularProgressBar()
        {
            this.DoubleBuffered = true; // 双缓冲以减少闪烁  
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private Int32 _Progress = 10;
        public Int32 Progress
        {
            get { return _Progress; }
            set
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                if (_Progress != value)
                {
                    _Progress = value;
                    Invalidate(); // 触发重绘  
                }   
            }
        }

        private Int32 _Thickness = 10;
        public Int32 Thickness
        {
            get => _Thickness;
            set 
            { 
                if (_Thickness != value)
                {
                    _Thickness = value;
                    Invalidate();
                }
            }
        }

        private Color _ProgressColor = Color.Green;
        public Color ProgressColor
        {
            get => _ProgressColor;
            set
            {
                if (_ProgressColor != value)
                {
                    _ProgressColor = value;
                    Invalidate();
                }
            }
        }
        private Color _ProgressBackColor = Color.Gray;
        public Color ProgressBackColor
        {
            get => _ProgressBackColor;
            set
            {
                if (_ProgressBackColor != value)
                {
                    _ProgressBackColor = value;
                    Invalidate();
                }
            }
        }
        private Color _GroundBackColor = Color.Gray;
        public Color GroundBackColor
        {
            get => _GroundBackColor;
            set
            {
                if (_GroundBackColor != value)
                {
                    _GroundBackColor = value;
                    Invalidate();
                }
            }
        }
        private Int32 _StartAngle = -90;
        public Int32 StartAngle
        {
            get => _StartAngle;
            set
            {
                if (_StartAngle != value)
                {
                    _StartAngle = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Int32 diameter = Math.Min(Width, Height); // 直径等于宽度和高度的最小值减一  
            Int32 radius = diameter / 2; // 半径等于直径的一半  
            Int32 centerX = Width / 2; // 圆心X坐标  
            Int32 centerY = Height / 2; // 圆心Y坐标  
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // 计算绘制区域
            var rect = new Rectangle(
                _Thickness / 2 + 2,
                _Thickness / 2 + 2,
                Width - _Thickness - 6,
                Height - _Thickness - 6);
            
            // 绘制进度条背景  
            using (Pen backgroundPen = new Pen(_ProgressBackColor, _Thickness))
            {
                e.Graphics.DrawArc(backgroundPen, rect, 0, 360);
            }
           
            // 绘制进度  
            Int32 sweepAngle = 360 * _Progress / 100; // 计算扫过的角度  
            using (Pen progressPen = new Pen(_ProgressColor, _Thickness))
            {
                e.Graphics.DrawArc(progressPen, rect, _StartAngle, sweepAngle);
            }
            SolidBrush groundbrush = new SolidBrush(_GroundBackColor);
            e.Graphics.FillEllipse(groundbrush, _Thickness + 2, _Thickness + 2, diameter - _Thickness * 2 - 6, diameter - _Thickness * 2 - 6);
            //// 绘制进度文本（可选）  
            //using (Font textFont = new Font("Arial", 12, FontStyle.Bold))
            //using (Brush textBrush = new SolidBrush(Color.Black))
            //{
            //    string text = Progress + "%";
            //    e.Graphics.DrawString(text, textFont, textBrush, centerX - e.Graphics.MeasureString(text, textFont).Width / 2, centerY - e.Graphics.MeasureString(text, textFont).Height / 2);
            //}
        }
    }

}
