using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Drawing;
namespace ScopeX.Core
{
    public class AreaHistogramPrsnt : MulticastPrsnt<IAreaHistogramView>, IAreaHistogramPrsnt
    {
        private protected override AreaHistogramModel Model
        {
            get;
        }

        internal AreaHistogramPrsnt(AreaHistogramModel model, IDsoPrsnt idp, IView? view) : base(idp)
        {
            Model = model;

            Model.PropertyChanged += OnPropertyChanged;
        }

        public AreaHistogramPrsnt(IDsoPrsnt idp, IView? view = null) : this(DsoModel.Default.AreaHistModel, idp, view)
        { }

        public Boolean Enabled
        {
            get => Model.Enabled;
            set
            {
                Model.Enabled = value;
                if (!value)
                {
                    PositionOfHV = (PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty);
                }
                else
                {
                    var p = DsoPrsnt.DefaultDsoPrsnt;
                    CalulatePoints2Expect(p.Timebase.PosIndexBymDiv, p.Timebase.ScaleByus);
                }

            }
        }
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) RectanglePoints
        {
            get => Model.RectanglePoints;
            set
            {

                Model.RectanglePoints = value;
                var p = DsoPrsnt.DefaultDsoPrsnt;
                CalulatePoints2Expect(p.Timebase.PosIndexBymDiv, p.Timebase.ScaleByus);
            }
        }

        /// <summary>
        /// Get leftup and rightdown position
        /// </summary>
        public String Postiton
        {
            get
            {
                var pointleftx = Quantity.ConvertByPrefix(Convert.ToDouble(PositionOfHV.LeftUp.X), Prefix.Micro, Prefix.Empty);
                var pointlefty = Quantity.ConvertByPrefix(Convert.ToDouble(PositionOfHV.LeftUp.Y), Prefix.Milli, Prefix.Empty);
                var pointrightx = Quantity.ConvertByPrefix(Convert.ToDouble(PositionOfHV.RightDown.X), Prefix.Micro, Prefix.Empty);
                var pointrighty = Quantity.ConvertByPrefix(Convert.ToDouble(PositionOfHV.RightDown.Y), Prefix.Milli, Prefix.Empty);
                var resultstring = pointleftx.ToString("E5") + "," + pointlefty.ToString("E5") + "," + pointrightx.ToString("E5") + "," + pointrighty.ToString("E5");
                return resultstring;
            }
        }

        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) PositionOfHV
        {
            get => Model.PositionOfHV;
            set
            {
                Model.PositionOfHV = value;
            }
        }
        public Double VerticalScale
        {
            get => Model.VerticalScale;

        }

        public Double VerticalPosIndexBymDiv
        {
            get => Model.VerticalPosIndexBymDiv;

        }

        public void CalulatePoints2Expect(Double horiPos, Double horiScale)
        {
            var vertpos = VerticalPosIndexBymDiv;
            var vertscale = VerticalScale;
            var leftup = RectanglePoints.LeftUp;
            var rightup = RectanglePoints.RightUp;
            var leftdown = RectanglePoints.LeftDown;
            var rightdown = RectanglePoints.RightDown;

            if (!leftup.IsEmpty)
            {
                leftup = new PointF(VitrualValue2TimeBaseValueByvs(leftup.X), VirualValue2VerticalValueBymV(leftup.Y, vertpos, vertscale));
            }

            if (!rightup.IsEmpty)
            {
                rightup = new PointF(VitrualValue2TimeBaseValueByvs(rightup.X), VirualValue2VerticalValueBymV(rightup.Y, vertpos, vertscale));
            }

            if (!leftdown.IsEmpty)
            {
                leftdown = new PointF(VitrualValue2TimeBaseValueByvs(leftdown.X), VirualValue2VerticalValueBymV(leftdown.Y, vertpos, vertscale));
            }

            if (!rightdown.IsEmpty)
            {
                rightdown = new PointF(VitrualValue2TimeBaseValueByvs(rightdown.X), VirualValue2VerticalValueBymV(rightdown.Y, vertpos, vertscale));
            }

            PositionOfHV = (leftup, rightup, rightdown, leftdown);
        }

        /// <summary>
        /// 根据虚拟坐标点位，计算期望值，单位us
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Single VitrualValue2TimeBaseValueByvs(Single value) => (Single)((value - DsoModel.Default.Timebase.PosIndex) / Constants.IDX_PER_XDIV * DsoModel.Default.Timebase.Scale);

        /// <summary>
        /// 根据虚拟坐标，计算期望的垂直轴值，单位：mv
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Single VirualValue2VerticalValueBymV(Single value, Double vertpos, Double vertscale) => (Single)((value - vertpos) / Constants.IDX_PER_YDIV * vertscale);


        /// <summary>
        /// 根据期望值时间，单位us，计算虚拟坐标X轴，
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Single VitrualValue2PointValueByvs(Single value) => (Single)(value / DsoModel.Default.Timebase.Scale * Constants.IDX_PER_XDIV + DsoModel.Default.Timebase.PosIndex);//(value - h_pos) / Constants.IDX_PER_XDIV * h_scale);

        /// <summary>
        /// 根据期望值垂直刻度，单位：mv，计算虚拟坐标Y轴，
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Single VirualValue2PointValueBymV(Single value, Double vertpos, Double vertscale) => (Single)(value / vertscale * Constants.IDX_PER_YDIV + vertpos);//(value - v_pos) / Constants.IDX_PER_YDIV * v_scale);
    }
}
