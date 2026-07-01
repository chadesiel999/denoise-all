// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/17</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;
    using ScopeX.Core.Hardware;

    /// <summary>
    /// Defines the <see cref="TrigEdgePrsnt" />.
    /// </summary>
    public class TrigEdgePrsnt : TrigSingleSrcPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigEdgePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigEdgePrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.Edge),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (_Model != null && Source != null)
            {
                if (Source == ChannelId.AuxIn)
                {
                    KeyLed.Default.SetTriggerSrc(ChannelId.Ext);
                }
                else
                {
                    KeyLed.Default.SetTriggerSrc(Source.Value);
                }
                _Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (_Model != null)
            {
                _Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        public void SetPosIndexCenterZero()
        {
            if (DsoModel.Default.Timebase.ScaleIndex >= AnaChnlTimebaseIndex.Lv50m)
            {
                // 滚动模式时，触发电平复位按钮失效
                return;
            }

            this.PosIndex = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigEdgePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerEdgeModel"/>.</param>
        internal TrigEdgePrsnt(IDsoPrsnt idp, ITriggerView view, TriggerEdgeModel model) : base(idp, view)
        {
            _Model = model;
            _Model.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the Coupling.
        /// </summary>
        public TriggerCoupling Coupling
        {
            get => Model.Coupling;
            set
            {
                Model.Coupling = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigCoupling);
            }
        }
        public Int32 SensitivityBymdiv  
        {
            get => Model.SensitivityBymdiv;
            set
            {
                Model.SensitivityBymdiv = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigSensitivity);
            }
        }
        public Int32 SensitivityMaxIndex
        {
            get => Model.SensitivityBymdivMax;
        }
        public Int32 SensitivityMinIndex
        {
            get => Model.SensitivityBymdivMin;
        }

        /// <summary>
        /// Gets or sets the Impedance.
        /// </summary>
        public TriggerImpedance Impedance
        {
            get => Model.Impedance;
            set
            {
                Model.Impedance = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigCoupling);
            }
        }

        //public String Name => Model.Name;
        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope Slope
        {
            get => Model.Slope;
            set
            {
                Model.Slope = value;
                Dispatcher.SoftReset();
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        
        private protected override TriggerEdgeModel Model => (TriggerEdgeModel)_Model!;
       
        public void ScpiResetPos()
        {
            base.ResetPosIndex();
            switch (Coupling)
            {
                case TriggerCoupling.AC:
                case TriggerCoupling.LFR:
                    SetPosIndexCenterZero();
                    break;
                case TriggerCoupling.HFR:
                default:
                    SetPosIndexCenter();
                    break;
            }
        }
    }
}
