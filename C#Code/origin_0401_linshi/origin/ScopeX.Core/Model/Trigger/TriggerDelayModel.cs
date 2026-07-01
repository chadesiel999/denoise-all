// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    /// <summary>
    /// Defines the <see cref="TriggerDelayModel" />.
    /// </summary>
    internal class TriggerDelayModel : TriggerMultiLevelModel
    {

        private ChannelId _SourceOne = ChannelId.C1;
        public ChannelId SourceOne
        {
            get => Source;
            set
            {
                Source = value;
                OnPropertyChanged();
            }
            //get => _SourceOne;
            //set
            //{
            //    if (_SourceOne != value)
            //    {
            //        _SourceOne = value;
            //        OnPropertyChanged();
            //    }
            //}
        }

        private ChannelId _SourceTwo = ChannelId.C2;
        public ChannelId SourceTwo
        {
            get => Data.Source;
            set
            {
                Data.Source = value;
                OnPropertyChanged();
            }
            //get => _SourceTwo;
            //set
            //{
            //    if (_SourceTwo != value)
            //    {
            //        _SourceTwo = value;
            //        OnPropertyChanged();
            //    }
            //}
        }
        private TriggerEdgeModel Data
        {
            get;
        } = new() { Source = ChannelId.C2 };

        public Double DataCompPosIndex
        {
            get => Data.CompPosIndex;
            set => Data.CompPosIndex = value;
        }

        public Double DataCompPosition
        {
            get => Data.CompPosition;
            set => Data.CompPosition = value;
        }

        public Double MaxDataCompPosition => Data.MaxCompPosition;

        public Double MinDataCompPosition => Data.MinCompPosition;

        public Double MaxDataCompPositionIndex => Data.MaxPosIndex;

        public Double MinDataCompPositionIndex => Data.MinPosIndex;

        public Double DataRelPosIndex
        {
            get => Data.RelPosIndex;
            set => Data.RelPosIndex = value;
        }

        public Prefix DataPrefix => Data.PosPrefix;

        public String DataUnit => Data.PosUnit;

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.Delay.ToString();

        /// <summary>
        /// Defines the Slope.
        /// </summary>
        private EdgeSlope _SourceOneSlope = EdgeSlope.Rise;
        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope SourceOneSlope
        {
            get => _SourceOneSlope;
            set
            {
                if (value != _SourceOneSlope)
                {
                    _SourceOneSlope = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Defines the Slope.
        /// </summary>
        private EdgeSlope _SourceTwoSlope = EdgeSlope.Rise;
        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope SourceTwoSlope
        {
            get => _SourceTwoSlope;
            set
            {
                if (value != _SourceTwoSlope)
                {
                    _SourceTwoSlope = value;
                    OnPropertyChanged();
                }
            }
        }

        public override event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
                Data.PropertyChanged += value;
            }
            remove
            {
                base.PropertyChanged -= value;
                Data.PropertyChanged -= value;
            }
        }
    }
}
