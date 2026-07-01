using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class SENTDecodePrsnt : ProtocolPrsnt
    {
        public SENTDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (SENTDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.SENT);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }

        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }
        // <summary>
        /// Model
        /// </summary>
        private protected override SENTDecodeModelCPP Model
        {
            get;
        }


        public ComModel.ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }


        public ProtocolSENT.PauseBit PauseBit
        {
            get => Model.PauseBit;
            set => Model.PauseBit = value.Clamp();
        }


        public ProtocolSENT.ChannelMode ChannelMode
        {
            get => Model.ChannelMode;
            set => Model.ChannelMode = value.Clamp();
        }

        public ProtocolSENT.FastChannelMode FastChannelMode
        {
            get => Model.FastChannelMode;
            set => Model.FastChannelMode = value.Clamp();
        }


        public ProtocolSENT.DataLength DataLength
        {
            get => Model.DataLength;
            set => Model.DataLength = value.Clamp();
        }


        public ProtocolCommon.Polarity Polarity
        {
            get => Model.Polarity;
            set => Model.Polarity = value.Clamp();
        }


        public ProtocolSENT.ClockTick ClockTick
        {
            get => Model.ClockTick;
            set => Model.ClockTick = value.Clamp();
        }

        public Double MaxClockTick => Model.MaxClockTick;
        public Double MinClockTick => Model.MinClockTick;

        public Double RealClockTick
        {
            get => Model.RealClockTick;
            set => Model.RealClockTick = Math.Clamp(value, MinClockTick, MaxClockTick);
        }
        public Int32 MaxTolerance => Model.MaxTolerance;
        public Int32 MinTolerance => Model.MinTolerance;
        public Int32 Tolerance
        {
            get => Model.Tolerance;
            set => Model.Tolerance = Math.Clamp(value, MinTolerance, MaxTolerance);
        }

        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;

        public Double Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String Unit => Model.Unit;

        public Double ThresholdBymV
        {
            get => Threshold * 1_000D;
            set => Threshold = value / 1000D;
        }
    }
}
