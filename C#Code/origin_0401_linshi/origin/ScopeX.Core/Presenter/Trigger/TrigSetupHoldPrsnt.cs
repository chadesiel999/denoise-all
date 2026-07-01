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
    /// Defines the <see cref="TrigSetupHoldPrsnt" />.
    /// </summary>
    public class TrigSetupHoldPrsnt : TriggerPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigSetupHoldPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigSetupHoldPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerSetupHoldModel)DsoModel.Default.GetTriggerModel(TriggerType.SetupHold),
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
                // KeyLed.Default.SetTriggerSrc(DataSource);
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
        /// Gets or sets the ClkCompPosIndex.
        /// </summary>
        public Double ClkCompPosIndex
        {
            get => Model.ClkCompPosIndex;
            set
            {
                Model.ClkCompPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the ClkCompPosition.
        /// </summary>
        public Double ClkCompPosition
        {
            get => Model.ClkCompPosition;
            set
            {
                Model.ClkCompPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Double VuClkCompPosition
        {
            get
            {
                var id = ClkSource;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.ClkCompPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.ClkCompPosition;
            }
            set
            {
                var id = ClkSource;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.ClkCompPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);

                        return;
                    }

                }
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                Model.ClkCompPosition = value;
            }
        }
        public Double ClkCompPositionBymV
        {
            get => ClkCompPosition;
            set => ClkCompPosition = value;
        }


        /// <summary>
        /// Gets or sets the ClkPolarity.
        /// </summary>
        public EdgeSlope ClkPolarity
        {
            get => Model.ClkPolarity;
            set
            {
                Model.ClkPolarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the ClkPrefix.
        /// </summary>
        public Prefix ClkPrefix => Model.ClkPrefix;

        /// <summary>
        /// Gets the ClkRelPosIndex.
        /// </summary>
        public Double ClkRelPosIndex
        {
            get => Model.ClkRelPosIndex;
            set
            {
                Model.ClkRelPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the ClkSource.
        /// </summary>
        public ChannelId ClkSource
        {
            get => Model.ClkSource;
            set
            {
                if (DataSource != value)
                {
                    Model.ClkSource = value;
                    Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                    // KeyLed.Default.SetTriggerSrc(Model.ClkSource);
                }
            }
        }

        /// <summary>
        /// Gets the ClkUnit.
        /// </summary>
        public String ClkUnit => Model.ClkUnit;

        /// <summary>
        /// Gets the DataPrefix.
        /// </summary>
        public Prefix DataPrefix => Model.DataPrefix;

        /// <summary>
        /// Gets or sets the DataRelPosLowerIndex.
        /// </summary>
        public Double DataRelPosLowerIndex
        {
            get => Model.DataRelPosLowerIndex;
            set
            {
                Model.DataRelPosLowerIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        //public (Double Lower, Double Upper) DataRelPosIndex => Model.DataRelPosIndex;
        /// <summary>
        /// Gets or sets the DataRelPosUpperIndex.
        /// </summary>
        public Double DataRelPosUpperIndex
        {
            get => Model.DataRelPosUpperIndex;
            set
            {
                Model.DataRelPosUpperIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public EdgeSlope DataPosPolarity
        {
            get => Model.DataPosPolarity;
            set
            {
                Model.DataPosPolarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the DataSource.
        /// </summary>
        public ChannelId DataSource
        {
            get => Model.DataSource;
            set
            {
                if (ClkSource != value)
                {
                    Model.DataSource = value;
                    Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                    //KeyLed.Default.SetTriggerSrc(Model.DataSource);
                }
            }
        }

        /// <summary>
        /// Gets the DataUnit.
        /// </summary>
        public String DataUnit => Model.DataUnit;

        /// <summary>
        /// Gets or sets the LowerDataPosIndex.
        /// </summary>
        public Double LowerDataPosIndex
        {
            get => Model.LowerDataPosIndex;
            set
            {
                Model.LowerDataPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the LowerDataPosition.
        /// </summary>
        public Double LowerDataPosition
        {
            get => Model.LowerDataPosition;
            set
            {
                Model.LowerDataPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the MaxClkCompPosition.
        /// </summary>
        public Double MaxClkCompPosition => Model.MaxClkCompPosition;

        /// <summary>
        /// Gets the MaxDataCompPosition.
        /// </summary>
        public Double MaxDataCompPosition => Model.MaxDataCompPosition;

        /// <summary>
        /// Gets the MaxThd.
        /// </summary>
        public Int64 MaxThd => Model.MaxThd;

        /// <summary>
        /// Gets the MaxTsu.
        /// </summary>
        public Int64 MaxTsu => Model.MaxTsu;

        /// <summary>
        /// Gets the MinClkCompPosition.
        /// </summary>
        public Double MinClkCompPosition => Model.MinClkCompPosition;

        /// <summary>
        /// Gets the MinDataCompPosition.
        /// </summary>
        public Double MinDataCompPosition => Model.MinDataCompPosition;

        /// <summary>
        /// Gets the MinThd.
        /// </summary>
        public Int64 MinThd => Model.MinThd;

        /// <summary>
        /// Gets the MinTsu.
        /// </summary>
        public Int64 MinTsu => Model.MinTsu;

        /// <summary>
        /// Gets or sets the PosIndex.
        /// </summary>
        public override Double PosIndex //
        {
            get => Model.ClkCompPosIndex;
            set
            {
                Model.ClkCompPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the ThdByps.
        /// </summary>
        public Int64 ThdByps
        {
            get => Model.ThdByps;
            set
            {
                Model.ThdByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double ThdByus
        {
            get => ThdByps / 1000_000D;
            set => ThdByps = (Int64)(value * 1000_000D);
        }

        /// <summary>
        /// Gets or sets the TsuByps.
        /// </summary>
        public Int64 TsuByps
        {
            get => Model.TsuByps;
            set
            {
                Model.TsuByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double TsuByus
        {
            get => TsuByps / 1000_000D;
            set => TsuByps = (Int64)(value * 1000_000D);
        }

        /// <summary>
        /// Gets or sets the UpperDataPosIndex.
        /// </summary>
        public Double UpperDataPosIndex
        {
            get => Model.UpperDataPosIndex;
            set
            {
                Model.UpperDataPosIndex = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the UpperDataPosition.
        /// </summary>
        public Double UpperDataPosition
        {
            get => Model.UpperDataPosition;
            set
            {
                Model.UpperDataPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        public Double VuUpperDataPosition
        {
            get
            {
                var id = DataSource;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        return Model.UpperDataPosition + am.Conditioning.BiasByuV / 1e3;
                    }

                }
                return Model.UpperDataPosition;
            }
            set
            {
                var id = DataSource;
                if (DsoModel.Default.TryGetChannel((ChannelId)id, out var cm))
                {
                    if (cm is AnalogModel am)
                    {
                        Model.UpperDataPosition = value - am.Conditioning.BiasByuV / 1e3;
                        Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                        return;
                    }

                }
                Model.UpperDataPosition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }
        /// <summary>
        /// Gets or sets the ClkCompPosition.
        /// </summary>
        public Double DataCompPositionBymV
        {
            get => UpperDataPosition;
            set => UpperDataPosition = value;
        }

        //public String Name => Model.Name;
        /// <summary>
        /// Gets or sets the Violation.
        /// </summary>
        public SetupHoldViolation Violation
        {
            get => Model.Violation;
            set
            {
                Model.Violation = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerSetupHoldModel Model => (TriggerSetupHoldModel)_Model!;

        /// <summary>
        /// The AdjThd.
        /// </summary>
        /// <param name="step">The step<see cref="Int32"/>.</param>
        public void AdjThd(Int32 step)
        {
            //Model.ThdByps += step * Model.StpThd;
            if ((Model.ThdByps >= 1E6) && (Model.ThdByps < 1E9))
            {
                Model.ThdByps += step * (Int32)1E3;
            }
            else if ((Model.ThdByps >= 1E9) && (Model.ThdByps < 1E12))
            {
                Model.ThdByps += step * (Int32)1E6;
            }
            else if (Model.ThdByps >= 1E12)
            {
                Model.ThdByps += step * (Int32)1E9;
            }
            else
            {
                Model.ThdByps += step * Constants.STP_PULSEWIDTH_PS;//Model.ThdByps;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        /// <summary>
        /// The AdjTsu.
        /// </summary>
        /// <param name="step">The step<see cref="Int32"/>.</param>
        public void AdjTsu(Int32 step)
        {
            //Model.TsuByps += step * Model.StpTsu;
            if ((Model.TsuByps >= 1E6) && (Model.TsuByps < 1E9))
            {
                Model.TsuByps += step * (Int32)1E3;
            }
            else if ((Model.TsuByps >= 1E9) && (Model.TsuByps < 1E12))
            {
                Model.TsuByps += step * (Int32)1E6;
            }
            else if (Model.TsuByps >= 1E12)
            {
                Model.TsuByps += step * (Int32)1E9;
            }
            else
            {
                Model.TsuByps += step * Constants.STP_PULSEWIDTH_PS; //Model.TsuByps;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        /// <summary>
        /// The ResetPosIndex.
        /// </summary>
        public override void ResetPosIndex()
        {
            Model.ClkCompPosIndex = 0;
        }
        public Double SetCompPosCenter(ChannelId ts)
        {
            try
            {
                var pkg = DsoModel.Default.GetWfmPack(ts);
                Double[] buffer = pkg.Buffer.Cast<Double>().ToArray();
                Double centerpos = (buffer.Max() + buffer.Min()) / 2;
                return centerpos;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
