// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="TriggerVideoModel" />.
    /// </summary>
    internal class TriggerVideoModel : TriggerSingleSrcModel
    {
        /// <summary>
        /// Defines the _CustomFrameRate.
        /// </summary>
        private VideoFrameRate _CustomFrameRate = VideoFrameRate.FM25;

        /// <summary>
        /// Defines the _CustomMux.
        /// </summary>
        private VideoMux _CustomMux = VideoMux.OneByOne;

        /// <summary>
        /// Defines the _Field.
        /// </summary>
        private Int16 _Field = 1;

        /// <summary>
        /// Defines the _Line.
        /// </summary>
        private Int16 _Line = 1;

        /// <summary>
        /// Defines the _Polarity.
        /// </summary>
        private VideoPolarity _Polarity = VideoPolarity.Positive;//极性

        /// <summary>
        /// Defines the _SpecifiedLine.
        /// </summary>
        private Int16 _SpecifiedLine = 1;

        /// <summary>
        /// Defines the _Standard.
        /// </summary>
        private VideoStandard _Standard = VideoStandard.NTSC;

        /// <summary>
        /// Defines the _Sync.
        /// </summary>
        private VideoSync _Sync = VideoSync.Specified;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerVideoModel"/> class.
        /// </summary>
        public TriggerVideoModel()
        {
            Reset();
        }

        /// <summary>
        /// Gets or sets the Field.
        /// </summary>
        public Int16 Field
        {
            get => _Field;
            set
            {
                value = ValidateField(value);
                if (value != _Field)
                {
                    _Field = value;
                    ResetLine();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the FrameRate.
        /// </summary>
        public VideoFrameRate FrameRate
        {
            get => _CustomFrameRate;
            set
            {
                if (value != _CustomFrameRate)
                {
                    _CustomFrameRate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Line.
        /// </summary>
        public Int16 Line
        {
            get => _Line;
            set
            {
                value = ValidateLine(value);
                if (value != _Line)
                {
                    _Line = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the MaxField.
        /// </summary>
        public Int16 MaxField { get; private set; }

        /// <summary>
        /// Gets the MaxLine.
        /// </summary>
        public Int16 MaxLine { get; private set; }

        /// <summary>
        /// Gets the MinLine.
        /// </summary>
        public Int16 MinLine { get; private set; }

        /// <summary>
        /// Gets or sets the Multiplexer.
        /// </summary>
        public VideoMux Multiplexer
        {
            get => _CustomMux;
            set
            {
                if (value != _CustomMux)
                {
                    _CustomMux = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.Video.ToString();

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public VideoPolarity Polarity
        {
            get => _Polarity;
            set
            {
                if (value != _Polarity)
                {
                    _Polarity = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the SpecifiedLine.
        /// </summary>
        public Int16 SpecifiedLine
        {
            get => _SpecifiedLine;
            set
            {
                if (value != _SpecifiedLine)
                {
                    _SpecifiedLine = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Standard.
        /// </summary>
        public VideoStandard Standard
        {
            get => _Standard;
            set
            {
                if (value != _Standard)
                {
                    _Standard = value;
                    Reset();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Sync.
        /// </summary>
        public VideoSync Sync
        {
            get => _Sync;
            set
            {
                if (value != _Sync)
                {
                    _Sync = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The Reset.
        /// </summary>
        private void Reset()
        {
            switch (Standard)
            {
                case VideoStandard.NTSC:
                    MaxField = 2;
                    Field = 1;
                    MaxLine = 525;
                    MinLine = 1;
                    Line = 1;
                    break;
                case VideoStandard.EDTV480I60:
                case VideoStandard.PAL:
                case VideoStandard.EDTV576I50:
                    MaxField = 2;
                    Field = 1;
                    MaxLine = 625;
                    MinLine = 1;
                    Line = 1;
                    break;
                case VideoStandard.HDTV720P60:
                    MaxField = 1;
                    Field = 1;
                    MaxLine = 750;
                    MinLine = 1;
                    Line = 1;
                    break;
                case VideoStandard.HDTV480P60:
                    MaxField = 1;
                    Field = 1;
                    MaxLine = 525;
                    MinLine = 1;
                    Line = 1;
                    break;
                case VideoStandard.HDTV1080P60:
                    MaxField = 1;
                    Field = 1;
                    MaxLine = 1125;
                    MinLine = 1;
                    Line = 1;
                    break;
                case VideoStandard.HDTV1080I60:
                    MaxField = 2;
                    Field = 1;
                    MaxLine = 563;
                    MinLine = 1;
                    Line = 1;
                    //if (Field % 2 == 0)
                    //{
                    //    _MaxLine = 1125;
                    //    _MinLine = 564;
                    //}
                    //else
                    //{
                    //    _MaxLine = 563;
                    //    _MinLine = 1;
                    //}
                    break;
                case VideoStandard.HDTV576P50:
                    MaxField = 1;
                    Field = 1;
                    MaxLine = 525;
                    MinLine = 1;
                    Line = 1;
                    break;
            }
        }

        /// <summary>
        /// The ResetLine.
        /// </summary>
        private void ResetLine()
        {
            switch (Standard)
            {
                case VideoStandard.NTSC:
                case VideoStandard.EDTV480I60:
                case VideoStandard.PAL:
                case VideoStandard.EDTV576I50:
                    if (Field % 2 == 0)
                    {
                        MaxLine = 525;
                        MinLine = 1;
                        Line = 1;
                    }
                    else
                    {
                        MaxLine = 266;
                        MinLine = 4;
                        Line = 4;
                    }
                    break;
                case VideoStandard.HDTV720P60:
                    break;
                case VideoStandard.HDTV480P60:
                    break;
                case VideoStandard.HDTV1080P60:
                    break;
                case VideoStandard.HDTV1080I60:
                    if (Field % 2 == 0)
                    {
                        MaxLine = 1125;
                        MinLine = 564;
                        Line = 564;
                    }
                    else
                    {
                        MaxLine = 563;
                        MinLine = 1;
                        Line = 1;
                    }
                    break;
                case VideoStandard.HDTV576P50:
                    break;
            }
        }

        /// <summary>
        /// The ValidateField.
        /// </summary>
        /// <param name="field">The field<see cref="Int16"/>.</param>
        /// <returns>The <see cref="Int16"/>.</returns>
        private Int16 ValidateField(Int16 field)
        {
            if (field < 1)
            {
                WeakTip.Default.Write("Field", MsgTipId.LessthanMin, false, "", 1);
                field = 1;
            }
            else if (field > MaxField)
            {
                WeakTip.Default.Write("Field", MsgTipId.GreatethanMax, false, "", 1);
                field = MaxField;
            }

            return field;
        }

        /// <summary>
        /// The ValidateLine.
        /// </summary>
        /// <param name="line">The line<see cref="Int16"/>.</param>
        /// <returns>The <see cref="Int16"/>.</returns>
        private Int16 ValidateLine(Int16 line)
        {
            if (line < MinLine)
            {
                WeakTip.Default.Write("Line", MsgTipId.LessthanMin, false, "", 1);
                line = MinLine;
            }
            else if (line > MaxLine)
            {
                WeakTip.Default.Write("Line", MsgTipId.GreatethanMax, false, "", 1);
                line = MaxLine;
            }

            return line;
        }
    }
}
