using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal class JTAGDecodeModel : ProtocolModel
    {
        public JTAGDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.JTAG, isTrigDecode)
        {
        }

        public override Double BitRateByPs => 1f / BitRate * 1E12;


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Sync",
            "Run Test Idle",
            "Select DR Scan",
            "Capture DR",
            "Shift DR",
            "Exit1 DR",
            "Pause DR",
            "Exit2 DR",
            "Update DR",
            "Select IR Scan",
            "Capture IR",
            "Shift IR",
            "Exit1 IR",
            "Pause IR",
            "Exit2 IR",
            "Update IR",
        };


        private ChannelId _TCK = ChannelId.C1;

        public ChannelId TCK
        {
            get { return _TCK; }
            set { UpdateProperty(ref _TCK, value); }
        }

        private Single _TCKThreshold=1;

        public Single TCKThreshold
        {
            get { return (Single)(_TCKThreshold * TryGetChannelGain(TCK)); }
            set { UpdateProperty(ref _TCKThreshold, value); }
        }
        public String TCKUnit => GetChannelUnit(TCK);


        private ChannelId _TMS = ChannelId.C2;

        public ChannelId TMS
        {
            get { return _TMS; }
            set { UpdateProperty(ref _TMS, value); }
        }

        private Single _TMSThreshold=1;

        public Single TMSThreshold
        {
            get { return (Single)(_TMSThreshold*TryGetChannelGain(TMS)); }
            set { UpdateProperty(ref _TMSThreshold, (Single)(value/ TryGetChannelGain(TMS))); }
        }
        public String TMSUnit => GetChannelUnit(TMS);

        private ChannelId _TDI = ChannelId.C3;

        public ChannelId TDI
        {
            get { return _TDI; }
            set { UpdateProperty(ref _TDI, value); }
        }

        private Single _TDIThreshold=1;

        public Single TDIThreshold
        {
            get { return (Single)(_TDIThreshold * TryGetChannelGain(TMS)); }
            set { UpdateProperty(ref _TDIThreshold, (Single)(value / TryGetChannelGain(TMS))); }
        }

        public String TDIUnit => GetChannelUnit(TDI);

        private ChannelId _TDO = ChannelId.C4;

        public ChannelId TDO
        {
            get { return _TDO; }
            set { UpdateProperty(ref _TDO, value); }
        }

        private Single _TDOThreshold=1;

        public Single TDOThreshold
        {
            get { return (Single)(_TDOThreshold * TryGetChannelGain(TDO)); }
            set { UpdateProperty(ref _TDOThreshold, (Single)(value / TryGetChannelGain(TDO))); }
        }
        public String TDOUnit => GetChannelUnit(TDO);

        private ComModel.ProtocolJTAG.DecodeChannel _DecodeChannel;

        public ComModel.ProtocolJTAG.DecodeChannel DecodeChannel
        {
            get { return _DecodeChannel; }
            set { UpdateProperty(ref _DecodeChannel, value); }
        }
        public Single MaxThresholdTCK { get => (Single)(12 * TryGetChannelGain(_TCK)); }
        public Single MaxThresholdTMS { get => (Single)(12 * TryGetChannelGain(_TMS)); }
        public Single MaxThresholdTDI { get => (Single)(12 * TryGetChannelGain(_TDI)); }
        public Single MaxThresholdTDO { get => (Single)(12 * TryGetChannelGain(_TDO)); }
        public Single MinThresholdTCK { get => -MaxThresholdTCK; }
        public Single MinThresholdTMS { get => -MaxThresholdTMS; }
        public Single MinThresholdTDI { get => -MaxThresholdTDI; }
        public Single MinThresholdTDO { get => -MaxThresholdTDO; }

        public UInt32 MinBitRate { get; } = 1000;
        public UInt32 MaxBitRate { get; } = 100_000_000;
        private UInt32 _BitRate = 31250000;

        public UInt32 BitRate
        {
            get { return _BitRate; }
            set { UpdateProperty(ref _BitRate, value); }
        }

        internal override void ParsingData(ref CancellationToken token)
        {

        }
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolJTAGOptions()
            {
                BitRate = BitRate,
                DecodeChannel = DecodeChannel,
                TCK = TCK,
                TCKThreshold = _TCKThreshold,
                TDI = TDI,
                TDIThreshold = _TDIThreshold,
                TDO = TDO,
                TDOThreshold = _TDOThreshold,
                TMS = TMS,
                TMSThreshold = _TMSThreshold,
            };
        }
    }
}
