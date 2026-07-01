// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="DigitalModel" />.
    /// </summary>
    internal class DigitalModel : ChannelModel
    {
        /// <summary>
        /// Defines the _FocusBitId.
        /// </summary>
        private Int32 _FocusBitId = 0;

        /// <summary>
        /// The last data in LA
        /// </summary>
        public IEnumerable<Double>? LastBuffer = Enumerable.Empty<Double>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalModel"/> class.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="color">The color<see cref="Color"/>.</param>
        /// <param name="tmb">The tmb<see cref="TimebaseModel"/>.</param>
        public DigitalModel(ChannelId id, Color color, TimebaseModel tmb)
            : base(ChannelType.LogicGroup, id, color)
        {
            var num = ChannelIdExt.MaxDChId - ChannelIdExt.MinDChId + 1;
            Conditioning = new ConditioningModel(this, num, num / 4, num / 8);
            Sampling = tmb;
        }

        /// <summary>
        /// Gets the BitHeight.
        /// </summary>
        public Double BitHeight => Conditioning.Bits[0].BitHeight;

        /// <summary>
        /// Gets or sets the BitHeightOpt.
        /// </summary>
        public DigiHeightOpt BitHeightOpt
        {
            get => (DigiHeightOpt)Conditioning.Bits[0].ScaleIndex;
            set
            {
                Conditioning.Bits[0].ScaleIndex = (Int32)value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the Conditioning.
        /// </summary>
        public override ConditioningModel Conditioning { get; }


        public override void ClearBuffer()
        {
            base.ClearBuffer();
            LastBuffer = Enumerable.Empty<Double>();
        }

        public override Boolean Active
        {
            get => base.Active;
            set
            {
                if (value && !OptionsManager.Default.GetOptionAvailable(OptionType.LA))
                {
                    WeakTip.Default.Write("LA", MsgTipId.PurchaseOptions, duration: 4);
                    value = false;
                }

                if (base.Active != value)
                {
                    base.Active = value;
                    AdcInterleaveProcessor.Default.Process();
                    //Hardware.HdCmdFactory.Push(HdCmd.LASwitch);//????
                }
            }
        }

        /// <summary>
        /// Gets or sets the FocusBitId.
        /// </summary>
        public Int32 FocusBitId
        {
            get => _FocusBitId;
            set
            {
                if (value > Conditioning.Bits.Count - 1)
                {
                    value = Conditioning.Bits.Count - 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                if (_FocusBitId != value)
                {
                    _FocusBitId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the Sampling.
        /// </summary>
        public override TimebaseModel Sampling { get; }

        /// <summary>
        /// Defines the <see cref="ConditioningModel" />.
        /// </summary>
        internal sealed class ConditioningModel : VertAxisModel
        {
            /// <summary>
            /// Defines the Bits.
            /// </summary>
            public readonly ReadOnlyCollection<DigitalBitModel> Bits;

            /// <summary>
            /// Defines the Groups.
            /// </summary>
            public readonly ReadOnlyCollection<DigitalBitCtrlGrpModel> Groups;

            /// <summary>
            /// Defines the _Bits.
            /// </summary>
            private readonly DigitalBitModel[] _Bits;

            /// <summary>
            /// Defines the _Groups.
            /// </summary>
            private readonly DigitalBitCtrlGrpModel[] _Groups;

            //private List<DigitalBitGroupModel> _BitGroups;

            //public List<DigitalBitGroupModel> BitGroups
            //{
            //    get => _BitGroups;
            //    set => _BitGroups = value;
            //}
            /// <summary>
            /// Defines the _OuterModel.
            /// </summary>
            private readonly DigitalModel _OuterModel;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConditioningModel"/> class.
            /// </summary>
            /// <param name="dm">The dm<see cref="DigitalModel"/>.</param>
            /// <param name="bitLength">The bitLength<see cref="Int32"/>.</param>
            /// <param name="throldGrpLen">The throldGrpLen<see cref="Int32"/>.</param>
            /// <param name="hystGrpLen">The hystGrpLen<see cref="Int32"/>.</param>
            public ConditioningModel(DigitalModel dm, Int32 bitLength, Int32 throldGrpLen, Int32 hystGrpLen) : base("Conditioning")
            {
                _OuterModel = dm;

                GetPosValue = (_, _) => 0;
                GetPosIndex = (_, _) => 0;

                GetScaleValue = (_, _) => 1;
                GetScaleIndex = (_) => (1, 0);
                IsScaleTickOverflow = (_, tick) => tick > 0;
                IsScaleTickUnderflow = (_, tick) => tick < 0;

                InitialScale = (0, 1);
                ScaleMaxIndex = DigitalBitModel.ScaleMaxIndex;
                ScaleMinIndex = DigitalBitModel.ScaleMinIndex;
                Prefix = Prefix.Empty;
                Unit = "";

                PosMaxIndex = DigitalBitModel.PosMaxIndex;
                PosMinIndex = DigitalBitModel.PosMinIndex;
                PosDefIndex = DigitalBitModel.PosDefIndex;

                var hyst = new LimitedPosition<Int32>[hystGrpLen];
                for (Int32 i = 0; i < hystGrpLen; i++)
                {
                    hyst[i] = new("UserHyst");
                }

                _Groups = new DigitalBitCtrlGrpModel[throldGrpLen];

                for (Int32 i = 0; i < throldGrpLen; i++)
                {
                    _Groups[i] = new(hyst[i / 2], dm.OnPropertyChanged);
                }

                Groups = Array.AsReadOnly(_Groups);

                //_BitGroups = new();

                _Bits = new DigitalBitModel[bitLength];
                for (Int32 i = 0; i < bitLength; i++)
                {
                    var id = i + ChannelIdExt.MinDChId;
                    _Bits[i] = new(id, ColorLookup.Default[id.ToString()], _Groups[i / 4], dm, dm.OnPropertyChanged);
                }
                Bits = Array.AsReadOnly(_Bits);
            }

            public Int32 Count => _Groups.Length;

            /// <summary>
            /// Gets or sets the PosIndex.
            /// </summary>
            public override Double PosIndex
            {
                get => Bits[_OuterModel.FocusBitId].PosIndex;
                set
                {
                    Bits[_OuterModel.FocusBitId].PosIndex = value;
                    DsoModel.Default.DigitalChPositionUpdateTime = TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue);// DateTime.Now;
                }
            }

            /// <summary>
            /// Gets or sets the ScaleIndex.
            /// </summary>
            public override Int32 ScaleIndex { get => Bits[_OuterModel.FocusBitId].ScaleIndex; set => Bits[_OuterModel.FocusBitId].ScaleIndex = value; }
        }
    }
}
