// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    public class DisplayPrsnt : MulticastPrsnt<IDisplayView>, IDisplayPrsnt
    {
        private protected override DisplayModel Model { get; }

        public DisplayPrsnt(IDsoPrsnt idp, IDisplayView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.Display,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };

            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Boolean AxisTickVisible
        {
            get => Model.AxisTickVisible;
            set => Model.AxisTickVisible = value;
        }

        public WfmDrawMode DrawMode
        {
            get => Model.DrawMode;
            set
            {
                Model.DrawMode = value;
                Hardware.HdCmdFactory.Push(HdCmd.DpxVectorized);
            }
        }

        public Action<Int32>? SetBrightness
        {
            set => Model.SetBrightness = value;
        }

        public Func<Int32>? GetBrightness
        {
            set => Model.GetBrightness = value;
        }

        public Func<Int32, Boolean>? SetContrast
        {
            set => Model.SetContrast = value;
        }
        public Func<(Boolean, Int32)>? GetContrast
        {
            set => Model.GetContrast = value;
        }

        public Int32 ScreenBrightness
        {
            get => Model.ScreenBrightness;
            set => Model.ScreenBrightness = value;
        }

        public Int32 ScreenContrast
        {
            get => Model.ScreenContrast;
            set => Model.ScreenContrast = value;
        }

        public Int32 GridIntensity
        {
            get => Model.GridIntensity;
            set => Model.GridIntensity = value;
        }

        public GridType GridStyle
        {
            get => Model.GridStyle;
            set => Model.GridStyle = value;
        }

        public WfmPersist Persist
        {
            get => Model.Persist;
            set
            {
                Model.Persist = value;
                Hardware.HdCmdFactory.Push(HdCmd.DpxVectorized);
            }
        }

        public Int32 WfmIntensity
        {
            get => Model.WfmIntensity;
            set => Model.WfmIntensity = value;
        }

        public static readonly Int32 MaxIntensity = DisplayModel.MaxIntensity;

        public static readonly Int32 MinIntensity = DisplayModel.MinIntensity;

        public PlotRenderType RenderType
        {
            get => Model.RenderType;
            set => Model.RenderType = value;
        }

        public MultiWfmsLayout WfmLayout
        {
            get => Model.WfmLayout;
            set => Model.WfmLayout = value;
        }

        public Boolean XAxisTickBottom
        {
            get => Model.XAxisTickBottom;
            set => Model.XAxisTickBottom = value;
        }

        public Boolean YAxisTickRight
        {
            get => Model.YAxisTickRight;
            set => Model.YAxisTickRight = value;
        }

        public void ResetWfmIntensity()
        {
            Model.WfmIntensity = DisplayModel.DefIntensity;
        }

        public Int32 AnalogZIndex
        {
            get => Model.AnalogZIndex;
            set => Model.AnalogZIndex = value;
        }

        public Func<Boolean>? GetTouchable
        {
            set => Model.GetTouchable = value;
        }
        public Action<Boolean>? SetTouchable
        {
            set => Model.SetTouchable = value;
        }

        /// <summary>
        /// 触摸使能 True --> 使能打开表示锁定（不支持触摸），false --> 使能关闭表示未锁定（支持触摸）
        /// </summary>
        public Boolean TouchLock
        {
            get => Model.TouchLock;
            set => Model.TouchLock = value;
        }

        /// <summary>
        /// 系统消息禁用和开启（主要是键盘和鼠标）
        /// </summary>
        public Boolean SystemMessageLock
        {
            get => Model.SystemMessageLock;
            set => Model.SystemMessageLock = value;
        }

        public Func<String>? GetLocalTime
        {
            set => Model.GetLocalTime = value;
        }
        public Func<String, Boolean>? SetLocalTime
        {
            set => Model.SetLocalTime = value;
        }

        public String SystemTime
        {
            get => Model.SystemTime;
            set => Model.SystemTime = value;
        }
    }
}
