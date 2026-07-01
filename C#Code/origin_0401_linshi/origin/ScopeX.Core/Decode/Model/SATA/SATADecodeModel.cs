using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class SATADecodeModel : ProtocolModel
    {
        public SATADecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.SATA, isTrigDecode)
        {
        }


        public override Double BitRateByPs => 1f / (Version switch
        {
            ProtocolSATA.SATAVersion.SATA1_0 => 1.5E9,
            ProtocolSATA.SATAVersion.SATA2_0 => 3.0E9,
            ProtocolSATA.SATAVersion.SATA3_0 => 6.0E9,
            _ => 1.5E9,
        }) * 1E12;


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Align",
            "Sync",
            "X Ready",
            "SOF",
            "Hold",
            "FIS Type",
            "Data",
            "CRC",
            "EOF",
            "WTRM",
            "Error",
        };


        private ComModel.ChannelId _Source1 = ChannelId.C1;

        public ComModel.ChannelId Source1
        {
            get { return _Source1; }
            set { UpdateProperty(ref _Source1, value); }
        }
        private ComModel.ChannelId _Source2 = ChannelId.C2;

        public ComModel.ChannelId Source2
        {
            get { return _Source2; }
            set { UpdateProperty(ref _Source2, value); }
        }

        private Single _Threshold=1;

        public Single Threshold
        {
            get { return (Single)(_Threshold*TryGetChannelGain(Source1)); }
            set { UpdateProperty(ref _Threshold, (Single)(value / TryGetChannelGain(Source1))); }
        }

        public String Unit => GetChannelUnit(Source1);
        private ComModel.ProtocolSATA.SATAVersion _Version;

        public ComModel.ProtocolSATA.SATAVersion Version
        {
            get { return _Version; }
            set { UpdateProperty(ref _Version, value); }
        }
        public UInt16 MinBytesCount => 1;
        public UInt16 MaxBytesCount => 1023;
        private UInt16 _BytesCount = 1;
        public UInt16 BytesCount
        {
            get => _BytesCount;
            set => UpdateProperty(ref _BytesCount, value);
        }
        private ComModel.ProtocolSATA.SignalType _SignalType = ProtocolSATA.SignalType.Single;

        public ComModel.ProtocolSATA.SignalType SignalType
        {
            get { return _SignalType; }
            set { UpdateProperty(ref _SignalType, value); }
        }
        public Single MaxThreshold { get; } = 20;
        public Single MinThreshold { get; } = -20;

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolSATAOptions()
            {
                Source = Source1,
                Source1 = Source1,
                Threshold = _Threshold,
                Version = Version,
                BytesCount = BytesCount,
            };
        }

        internal override void ParsingData(ref CancellationToken token)
        {

        }
    }
}
