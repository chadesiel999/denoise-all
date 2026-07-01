// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="TrigSustainTimePrsnt" />.
    /// </summary>
    public class TrigSustainTimePrsnt : TrigMultiLevelPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigSustainTimePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigSustainTimePrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerSustainTimeModel)DsoModel.Default.GetTriggerModel(TriggerType.SustainTime),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="TrigSustainTimePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerSustainTimeModel"/>.</param>
        internal TrigSustainTimePrsnt(IDsoPrsnt idp, ITriggerView view, TriggerSustainTimeModel model) : base(idp, view)
        {
            _Model = model;
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
        /// Gets the BitLength.
        /// </summary>
        public Int32 BitLength => Model.Length;

        /// <summary>
        /// Gets or sets the CurrentSource.
        /// </summary>
        public ChannelId CurrentSource { get; set; }

        public PulseCondition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerSustainTimeModel Model => (TriggerSustainTimeModel)_Model!;

        

        /// <summary>
        /// The GetCompPosition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        public Double GetCompPosition(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetPosition(ts);
        }
        public Double GetVuCompPosition(ChannelId ts)
        {
            Model.Bits.Source = ts;
            if (DsoModel.Default.TryGetChannel(ts, out var cm))
            {
                if (cm is AnalogModel am)
                {
                    return Model.Bits.GetPosition(ts) + am.Conditioning.BiasByuV / 1e3;
                }

            }
            return Model.Bits.GetPosition(ts);
        }
   
        /// <summary>
        /// The GetMaxCompPosition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        public Double GetMaxCompPosition(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetMaxPosition(ts);
        }

        /// <summary>
        /// The GetMinCompPosition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        public Double GetMinCompPosition(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetMinPosition(ts);
        }

        /// <summary>
        /// The GetPosCompCondition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="PatLevelCondition"/>.</returns>
        public SustainTimeLevelCondition GetPosCompCondition(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetCondition(ts);
        }

        /// <summary>
        /// The GetPosIndex.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        public Double GetPosIndex(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetPosIndex(ts);
        }


        /// <summary>
        /// The GetPosPrefix.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Prefix"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Prefix GetPosPrefix(ChannelId ts)
        {
            return TriggerModel.GetPosPrefix(ts);
        }


        /// <summary>
        /// The GetPosUnit.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="String"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public String GetPosUnit(ChannelId ts)
        {
            return TriggerModel.GetPosUnit(ts);
        }

        /// <summary>
        /// The ResetPosIndex.
        /// </summary>
        public override void ResetPosIndex()
        {
            SetPosIndex(CurrentSource, 0);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        /// <summary>
        /// The SetCompPosition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <param name="value">The value<see cref="Double"/>.</param>
        public void SetCompPosition(ChannelId ts, Double value)
        {
            Model.Bits.Source = ts;
            Model.Bits.SetPosition(ts, value);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }
        public void SetVuCompPosition(ChannelId ts, Double value)
        {
            Model.Bits.Source = ts;
            var compos = 0D;
            if (DsoModel.Default.TryGetChannel(ts, out var cm))
            {
                if (cm is AnalogModel am)
                {
                    compos = value - am.Conditioning.BiasByuV / 1e3;
                    Model.Bits.SetPosition(ts, compos);
                }
            }
            else
            {
                Model.Bits.SetPosition(ts, value);
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }
        /// <summary>
        /// The SetPosCompCondition.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <param name="plc">The plc<see cref="PatLevelCondition"/>.</param>
        public void SetPosCompCondition(ChannelId ts, SustainTimeLevelCondition plc)
        {
            Model.Bits.Source = ts;
            Model.Bits.SetCondition(ts, plc);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        /// <summary>
        /// The SetPosIndex.
        /// </summary>
        /// <param name="ts">The ts<see cref="ChannelId"/>.</param>
        /// <param name="value">The value<see cref="Double"/>.</param>
        public void SetPosIndex(ChannelId ts, Double value)
        {
            Model.Bits.Source = ts;
            Model.Bits.SetPosIndex(ts, value);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public void SetRelPosIndex(ChannelId ts, Double value)
        {
            Model.Bits.Source = ts;
            Model.Bits.SetRelPosIndex(ts, value);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public Double GetRelPosIndex(ChannelId ts)
        {
            Model.Bits.Source = ts;
            return Model.Bits.GetRelPosIndex(ts);
        }

        public void SetCompPosCenter(ChannelId ts)
        {
            try
            {
                var pkg = DsoModel.Default.GetWfmPack(ts);
                Double[] buffer = pkg.Buffer.Cast<Double>().ToArray();
                Double centerpos = (buffer.Max() + buffer.Min()) / 2;
                Model.Bits.Source = ts;
                Model.Bits.SetPosition(ts, centerpos);
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
            catch (Exception ex)
            {
                Model.Bits.Source = ts;
                Model.Bits.SetPosition(ts, 0);
            }
        }

    }
}
