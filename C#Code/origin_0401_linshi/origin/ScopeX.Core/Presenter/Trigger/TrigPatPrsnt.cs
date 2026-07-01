// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Linq;
    using NPOI.POIFS.NIO;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="TrigPatPrsnt" />.
    /// </summary>
    public class TrigPatPrsnt : TriggerPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigPatPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigPatPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerPatternModel)DsoModel.Default.GetTriggerModel(TriggerType.Pattern),
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
                // 码型触发时，触发电平旋钮灯不需要亮。
                KeyLed.Default.SetLedColor(LedEnum.LedTriggerLevel, System.Drawing.Color.Black);
                /*if (DsoPrsnt.FocusId.IsAnalog() || DsoPrsnt.FocusId.IsDigital())
                {
                    KeyLed.Default.SetTriggerSrc(DsoPrsnt.FocusId);
                }
                else
                {
                    KeyLed.Default.SetTriggerSrc(ChannelId.C1);
                }*/
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

        public Int64 DurationByps
        {
            get => Model.DurationByps;
            set
            {
                Model.DurationByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double DurationByus
        {
            get => DurationByps / 1000_000D;
            set => DurationByps = (Int64)(value * 1000_000D);
        }

        public Int64 UpperDurationByps
        {
            get => Model.UpperDurationByps;
            set
            {
                Model.UpperDurationByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public (Int64 min, Int64 max) GetDurationRange() => Model.GetDurationRange(0);

        public (Int64 min, Int64 max) GetUpperDurationRange() => Model.GetDurationRange(1);

        //public String Name => Model.Name;
        /// <summary>
        /// Gets or sets the Operator.
        /// </summary>
        public PatOperator Operator
        {
            get => Model.Operator;
            set
            {
                Model.Operator = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the PosIndex.
        /// </summary>
        public override Double PosIndex
        {
            get => GetPosIndex(CurrentSource);
            set
            {
                SetPosIndex(CurrentSource, value);
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the TimeCondition.
        /// </summary>
        public PatTimeCondition TimeCondition
        {
            get => Model.TimeCondition;
            set
            {
                Model.TimeCondition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerPatternModel Model => (TriggerPatternModel)_Model!;

        public void AdjDuration(Int64 step)
        {
            Model.DurationByps += step * Model.StpDuration;
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public void AdjUpperDuration(Int64 step)
        {
            Model.UpperDurationByps += step * Model.StpDuration;
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

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
        public PatLevelCondition GetPosCompCondition(ChannelId ts)
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
        public void SetPosCompCondition(ChannelId ts, PatLevelCondition plc)
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
            }
            catch (Exception ex)
            {
                Model.Bits.Source = ts;
                Model.Bits.SetPosition(ts, 0);
            }
            finally
            {
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
    }
}
