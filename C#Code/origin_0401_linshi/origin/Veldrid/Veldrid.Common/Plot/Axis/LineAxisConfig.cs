using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Veldrid.Common.Plot
{
    public class LineAxisConfig : BaseProperty
    {
        private Func<float, string> labelFormatter;
        private Position position = Position.Left;
        private Color lableColor = Color.White;
        private Color color = Color.White;
        private int majorTick = 40;
        private float step = 1000;
        private float brightness = 100;
        private float lableFontSize = 10;
        private bool lableVisibily = true;
        private float pointOffset = 0;
        private bool visibily = true;
        private int minjorTick = 20;
        private int minjorTickCount = 5;
        private double lableStep;
        private string unit = string.Empty;
        private float offsetValue;

        /// <summary>
        /// 标签格式
        /// </summary>
        [AllowNull]

        public Func<float, String> LabelFormatter
        {
            get => labelFormatter;
            set => Set(ref labelFormatter, value);
        }
        /// <summary>
        /// 位置参数校验
        /// </summary>
        [AllowNull]
        internal Func<Position, Position> PositionVerify { get; set; }
        /// <summary>
        /// 坐标轴位置
        /// </summary>
        public Position Position
        {
            get => position;
            set
            {
                Position temp = value;
                if (PositionVerify != null) temp = PositionVerify(value);
                Set(ref position, temp);
            }
        }
        public LineAxisType Type { get; set; } = LineAxisType.Linear;

        /// <summary>
        /// 标签颜色
        /// </summary>
        public Color LableColor
        {
            get => lableColor; set => Set(ref lableColor, value);
        }
        /// <summary>
        /// 坐标轴颜色
        /// </summary>
        public Color Color
        {
            get => color; set => Set(ref color, value);
        }
        /// <summary>
        /// 主间隔高度
        /// </summary>
        public int MajorTick
        {
            get => majorTick; set => Set(ref majorTick, value);
        }
        /// <summary>
        /// 次间隔高度
        /// </summary>
        public int MinjorTick
        {
            get => minjorTick; set => Set(ref minjorTick, value);
        }
        /// <summary>
        /// 次间隔数量
        /// </summary>
        public int MinjorTickCount
        {
            get => minjorTickCount; set => Set(ref minjorTickCount, value);
        }
        /// <summary>
        /// 主间隔
        /// </summary>
        public float Step
        {
            get => step; set => Set(ref step, value);
        }
        public Double LableStep
        {
            get => lableStep; set => Set(ref lableStep, value);
        }
        /// <summary>
        /// 坐标轴亮度
        /// </summary>
        public float Brightness
        {
            get => brightness; set => Set(ref brightness, Math.Clamp(value, 0, 100));
        }
        /// <summary>
        /// 标签字体大小
        /// </summary>
        public float LableFontSize
        {
            get => lableFontSize; set => Set(ref lableFontSize, value);
        }
        /// <summary>
        /// 标签可见
        /// </summary>
        public Boolean LableVisibily
        {
            get => IsHistparmX? false:lableVisibily;
            set
            {
                Set(ref lableVisibily, value);
            }
        }

        public Boolean HistparmLableVisibily => IsHistparmX ? lableVisibily : false;

        public Boolean IsHistparmX = false;

        private Boolean _ShowGrid = false;

        public Boolean ShowGrid
        {
            get { return _ShowGrid; }
            set { Set(ref _ShowGrid,value); }
        }

        /// <summary>
        /// 偏移数
        /// </summary>
        public float PointOffset
        {
            get => pointOffset; set => Set(ref pointOffset, value);
        }
        public Single OffsetValue { get => offsetValue; set =>Set(ref offsetValue,value); }
        /// <summary>
        /// 坐标轴可见
        /// </summary>
        public Boolean Visibily
        {
            get => visibily; set => Set(ref visibily, value);
        }
        public String Unit { get => unit; set => Set(ref unit, value); }
        public int Decimal { get; set; } = 2;
        ///// <summary>
        ///// 间隔绘制位置
        ///// </summary>
        //public TickStyle TickStyle
        //{
        //    get => tickStyle; set => Set(ref tickStyle, value);
        //}
    }
}
