// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    /// <summary>
    /// Defines the <see cref="TrigDelayPrsnt" />.
    /// </summary>
    public class TrigDelayPrsnt : TrigMultiLevelPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigDelayPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigDelayPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerDelayModel)DsoModel.Default.GetTriggerModel(TriggerType.Delay),
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
            if (_Model != null)
            {
                KeyLed.Default.SetLedColor(LedEnum.LedTriggerLevel, System.Drawing.Color.Black);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigDelayPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerDelayModel"/>.</param>
        internal TrigDelayPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerDelayModel model) : base(idp, view)
        {
            _Model = model;
            _Model.PropertyChanged += OnPropertyChanged;
        }

        public ChannelId SourceOne
        {
            get => Model.SourceOne;
            set
            {
                if (SourceTwo == value)
                {
                    return;
                }
                Model.SourceOne = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                // 2024年6月4日 sx：触发方式具有两个及其以上的源时，出发旋钮不显示任何的颜色
                //KeyLed.Default.SetTriggerSrc(Model.SourceOne);
            }
        }

        public ChannelId SourceTwo
        {
            get => Model.SourceTwo;
            set
            {
                if (SourceOne == value)
                {
                    return;
                }
                Model.SourceTwo = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                // 2024年6月4日 sx：触发方式具有两个及其以上的源时，出发旋钮不显示任何的颜色
                //KeyLed.Default.SetTriggerSrc(Model.SourceTwo);
            }
        }

        public EdgeSlope SourceOneSlope
        {
            get => Model.SourceOneSlope;
            set
            {
                Model.SourceOneSlope = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public EdgeSlope SourceTwoSlope
        {
            get => Model.SourceTwoSlope;
            set
            {
                Model.SourceTwoSlope = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Double DataCompPosIndex
        {
            get => Model.DataCompPosIndex;
            set
            {
                Model.DataCompPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public new Double VuUpperCompPosition
        {
            get
            {
                var id = SourceOne;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.UpperCompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.UpperCompPosition;
            }
            set
            {
                var id = SourceOne;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.UpperCompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                        return;
                    }

                }
                Model.UpperCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double VuDataCompPosition
        {
            get
            {
                var id = SourceTwo;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.DataCompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.DataCompPosition;
            }
            set
            {
                var id = SourceTwo;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.DataCompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                        return;
                    }

                }
                Model.DataCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Double DataCompPosition
        {
            get => Model.DataCompPosition;
            set
            {
                Model.DataCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double MaxDataCompPosition => Model.MaxDataCompPosition;

        public Double MinDataCompPosition => Model.MinDataCompPosition;

        public Double DataRelPosIndex
        {
            get => Model.DataRelPosIndex;
            set
            {
                Model.DataRelPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Prefix DataPrefix => Model.PosPrefix;

        public String DataUnit => Model.PosUnit;
        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerDelayModel Model => (TriggerDelayModel)_Model!;
    }
}
