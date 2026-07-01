using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ScopeX.Core
{
    public class VisualTriggerPrsnt : MulticastPrsnt<IVisualTriggerView>, IVisualTriggerPrsnt
    {
        public Action<(PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown)>? ShowHistogram;

        private protected override VisualTriggerModel Model { get; }

        public VisualTriggerPrsnt(IDsoPrsnt idp, IVisualTriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.VisualTrigger,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            var vip = new VisualTriggerItemPrsnt[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                vip[i] = new VisualTriggerItemPrsnt(Model.SelectedItems[i]);
            }
            SelectedItems = Array.AsReadOnly(vip);

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public readonly IList<VisualTriggerItemPrsnt> SelectedItems;

        public VisualTriggerItemPrsnt this[Int32 index] => SelectedItems[index];

        public Int32 Length => Model.SelectedItems.Length;

        public Boolean Enabled
        {
            get => Model.Enabled;
            set => Model.Enabled = value;
        }
        /// <summary>
        /// 当前选中项索引
        /// </summary>
        public Int32 CurrentSelected { get => Model.CurrentSelected; set => Model.CurrentSelected = value; }
        /// <summary>
        /// 区域关系
        /// </summary>
        public VisualTriggerRelation Relation
        {
            get => Model.Relation;
            set => Model.Relation = value;
        }
    }

    public class VisualTriggerItemPrsnt
    {
        internal VisualTriggerItemPrsnt(VisualTriggerItemModel m)
        {
            Model = m;
        }

        /// <summary>
        /// 区域名称，用于用户在界面上区分
        /// </summary>
        public String? Name => Model.Name;

        public Color PolygonsDrawColor
        {
            get => Model.PolygonsDrawColor;
            set => Model.PolygonsDrawColor = value;
        }

        public ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(value, out var channel))
                {
                    if (channel is AnalogPrsnt aprsnt)
                    {
                        VerticalScale = aprsnt.ScaleBymV;
                        VerticalPosIndexBymDiv = aprsnt.PosIndexBymDiv;
                    }
                }
                Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public Boolean Success
        {
            get => Model.Success;
        }
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
                    CalulatePoints2Expect();
                }
                Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) RectanglePoints
        {
            get => Model.RectanglePoints;
            set
            {
                Model.RectanglePoints = value;
                CalulatePoints2Expect();
            }
        }

        public List<List<PointF>> Polygons
        {
            get => Model.Polygons;
            set
            {
                Model.Polygons = value;
                // Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        /// <summary>
        /// 用于记录区域想要表达的以时基档和垂直档位为单位的数值，(x us,y mv)，
        /// 使得缩放和移动时，根据此想要表达的数值来确定新的形状大小和位置，
        /// 直接缩放会有精度问题
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) PositionOfHV
        {
            get => Model.PositionOfHV;
            private set
            {
                Model.PositionOfHV = value;
            }
        }

        /// <summary>
        /// Get leftup and rightdown position
        /// </summary>
        public String Position
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

        /// <summary>
        /// 形状点位改变了
        /// </summary>
        public void PolygonsChanged() => Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);

        /// <summary>
        /// 根据数据点位计算出形状想要表达的期望值(x us,y mv)
        /// </summary>
        public void CalulatePoints2Expect()
        {
            var vertpos = this.VerticalPosIndexBymDiv;
            var vertscale = this.VerticalScale;
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
        private Single VirualValue2VerticalValueBymV(Single value, Double vertPos, Double vertScale) => (Single)((value - vertPos) / Constants.IDX_PER_YDIV * vertScale);


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
        public Single VirualValue2PointValueBymV(Single value, Double vertPos, Double vertScale) => (Single)(value / vertScale * Constants.IDX_PER_YDIV + vertPos);//(value - v_pos) / Constants.IDX_PER_YDIV * v_scale);

        public VisualTriggerState TriggerState
        {
            get => Model.TriggerState;
            set
            {
                Model.TriggerState = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public Int32 PolygonEdgeNumber
        {
            get => Model.PolygonEdgeNumber;
            set => Model.PolygonEdgeNumber = value;
        }

        /// <summary>
        /// The unit of polygonrenter is virtual coordinate
        /// </summary>
        public PointF PolygonCenter
        {
            get => Model.PolygonCenter;
            set => Model.PolygonCenter = value;
        }

        /// <summary>
        /// The unit of polygonradius is pixel coordinate
        /// </summary>
        public Single PolygonRadius
        {
            get => Model.PolygonRadius;
            set => Model.PolygonRadius = value;
        }

        public VisualTriggerShape TriggerShape
        {
            get => Model.TriggerShape;
            set
            {
                Model.TriggerShape = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public Double TimebaseScale
        {
            get => Model.TimebaseScale;
            set
            {
                Model.TimebaseScale = value;
                //Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public Double VerticalScale
        {
            get => Model.VerticalScale;
            set
            {
                Model.VerticalScale = value;
                //Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
            }
        }

        public Double VerticalPosIndexBymDiv
        {
            get => Model.VerticalPosIndexBymDiv;
            set
            {
                Model.VerticalPosIndexBymDiv = value;
            }
        }

        /// <summary>
        /// 距离触发点的偏移时间 mdiv
        /// </summary>
        public Double TimeBasePosIndexBymDiv
        {
            get => Model.TimeBasePosIndexBymDiv;
            set
            {
                Model.TimeBasePosIndexBymDiv = value;
            }
        }

        public Boolean ReSet
        {
            get => Model.ReSet;
            set
            {
                Model.ReSet = value;
            }
        }
        private protected VisualTriggerItemModel Model { get; }
    }
}
