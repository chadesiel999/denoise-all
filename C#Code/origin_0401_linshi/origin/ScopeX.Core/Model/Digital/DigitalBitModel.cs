// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using System.Drawing;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="DigitalBitModel" />.
    /// </summary>
    internal class DigitalBitModel
    {
        /// <summary>
        /// Defines the PosDefIndex.
        /// </summary>
        public static readonly Double PosDefIndex = PosMinIndex;

        /// <summary>
        /// Defines the PosIdxPerDiv.
        /// </summary>
        public static readonly Double PosIdxPerDiv = Constants.IDX_PER_YDIV;

        /// <summary>
        /// Defines the PosMaxIndex.
        /// </summary>
        public static readonly Double PosMaxIndex = Constants.DEF_YPOS_IDX + Constants.VIS_YDIVS_NUM / 2.0 * PosIdxPerDiv;

        /// <summary>
        /// Defines the PosMinIndex.
        /// </summary>
        public static readonly Double PosMinIndex = Constants.DEF_YPOS_IDX - Constants.VIS_YDIVS_NUM / 2.0 * PosIdxPerDiv;

        /// <summary>
        /// Defines the ScaleMaxIndex.
        /// </summary>
        public static readonly Int32 ScaleMaxIndex = 2;

        /// <summary>
        /// Defines the ScaleMinIndex.
        /// </summary>
        public static readonly Int32 ScaleMinIndex = 0;

        //private static readonly Double _MaxHeight = PosIdxPerDiv * Constants.VIS_YDIVS_NUM / ChannelIdExt.DigiChnlNum;
        /// <summary>
        /// Defines the _MaxHeight.
        /// </summary>
        private static readonly Double _MaxHeight = PosIdxPerDiv / 2;

        /// <summary>
        /// Defines the _Container.
        /// </summary>
        private readonly DigitalModel _Container;

        /// <summary>
        /// Defines the _ScaleIndex.
        /// </summary>
        private static Int32 _ScaleIndex;

        /// <summary>
        /// Defines the _Active.
        /// </summary>
        

        /// <summary>
        /// Defines the _Label.
        /// </summary>
        private String _Label = String.Empty;

        /// <summary>
        /// Defines the _Label.
        /// </summary>
        private Boolean _LabelVisiblity = false;

        /// <summary>
        /// Defines the _PosIndex.
        /// </summary>
        private Double _PosIndex = PosDefIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalBitModel"/> class.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="color">The color<see cref="Color"/>.</param>
        /// <param name="bcg">The bcg<see cref="DigitalBitCtrlGrpModel"/>.</param>
        /// <param name="dm">The dm<see cref="DigitalModel"/>.</param>
        /// <param name="onPropertyChanged">The onPropertyChanged<see cref="Action{String}?"/>.</param>
        public DigitalBitModel(ChannelId id, Color color, DigitalBitCtrlGrpModel bcg, DigitalModel dm, Action<String>? onPropertyChanged = null)
        {
            Type = ChannelType.Logic;
            Id = id;
            Name = Id.ToString();
            DrawColor = color;
            CtrlGroup = bcg;
            _Container = dm;
            _LabelVisiblity = dm.LabelVisibility;
            OnPropertyChanged = onPropertyChanged;
        }

        private Boolean _ActiveBit = false;
        public Boolean ActiveBit
        {
            get => _ActiveBit;
            set
            {
                if (_ActiveBit != value)
                {
                    _ActiveBit = value;
                    OnPropertyChanged?.Invoke(nameof(ActiveBit));
                }

                _Container.Active = _Container.Conditioning.Bits.Any(o => o.ActiveBit);
            }
        }

        //public void AdjPosIndex(Double step) => PosIndex += step * BitHeight;

        /// <summary>
        /// Gets the BitHeight.
        /// </summary>
        public Double BitHeight => GetBitHeight(ScaleIndex);

        /// <summary>
        /// Gets the CtrlGroup.
        /// </summary>
        public DigitalBitCtrlGrpModel CtrlGroup { get; }

        /// <summary>
        /// Gets or sets the DrawColor.
        /// </summary>
        public Color DrawColor { get; set; }


        
        /// <summary>
        /// Gets the Id.
        /// </summary>
        public ChannelId Id { get; }

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        public String Label
        {
            get => _Label;
            set
            {
                if (_Label != value)
                {
                    _Label = value;
                    OnPropertyChanged?.Invoke(nameof(Label));
                }
            }
        }
        /// <summary>
        /// Gets or sets the LabelVisiblity.
        /// </summary>
        public Boolean LabelVisiblity
        {
            get => _LabelVisiblity;
            set
            {
                if (_LabelVisiblity != value)
                {
                    _LabelVisiblity = value;
                    OnPropertyChanged?.Invoke(nameof(LabelVisiblity));
                }
            }
        }

        /// <summary>
        /// Gets or sets the LogicalGroup.
        /// </summary>
        public String? LogicalGroup { get; set; } = null;

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Gets or sets the PosIndex.
        /// </summary>
        public Double PosIndex
        {
            get => _PosIndex;
            set
            {
                value = ValidatePosIndex(value);
                if (_PosIndex != value)
                {
                    _PosIndex = value;
                    DsoModel.Default.DigitalChPositionUpdateTime = TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue);// DateTime.Now;
                    OnPropertyChanged?.Invoke(nameof(PosIndex));
                }
            }
        }

        /// <summary>
        /// Gets or sets the ScaleIndex.
        /// </summary>
        public Int32 ScaleIndex
        {
            get => _ScaleIndex;
            set
            {
                value = ValidateScaleIndex(value);
                if (_ScaleIndex != value)
                {
                    _ScaleIndex = value;
                    OnPropertyChanged?.Invoke(nameof(ScaleIndex));
                }
            }
        }

        /// <summary>
        /// Gets the Type.
        /// </summary>
        public ChannelType Type { get; }

        /// <summary>
        /// Gets the OnPropertyChanged.
        /// </summary>
        protected Action<String>? OnPropertyChanged { get; }

        /// <summary>
        /// The GetBitHeight.
        /// </summary>
        /// <param name="scaleIndex">The scaleIndex<see cref="Int32"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        public static Double GetBitHeight(Int32 scaleIndex)
        {
            return _MaxHeight / 4 * (1 << scaleIndex); /*PosIdxPerDiv / 8 * (1 << scaleIndex);*/
        }

        /// <summary>
        /// The ValidateScaleIndex.
        /// </summary>
        /// <param name="scaleIndex">The scaleIndex<see cref="Int32"/>.</param>
        /// <returns>The <see cref="Int32"/>.</returns>
        protected static Int32 ValidateScaleIndex(Int32 scaleIndex)
        {
            if (scaleIndex > ScaleMaxIndex)
            {
                return ScaleMaxIndex;
            }
            else if (scaleIndex < ScaleMinIndex)
            {
                return ScaleMinIndex;
            }

            return scaleIndex;
        }

        /// <summary>
        /// The ValidatePosIndex.
        /// </summary>
        /// <param name="posIndex">The posIndex<see cref="Double"/>.</param>
        /// <returns>The <see cref="Double"/>.</returns>
        protected Double ValidatePosIndex(Double posIndex)
        {
            var value = Math.Round(posIndex / BitHeight, MidpointRounding.AwayFromZero) * BitHeight;
            if (value + BitHeight > PosMaxIndex)
            {
                value = PosMaxIndex - BitHeight;
            }
            else if (value < PosMinIndex)
            {
                value = PosMinIndex;
            }

            return value;
        }
    }
}
