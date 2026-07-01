using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class SATADecodePrsnt : ProtocolPrsnt
    {
        public SATADecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (SATADecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.SATA);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1,Source2 };
        }
        public ComModel.ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = value.Clamp(ActivedChannels);
        }

        public ComModel.ChannelId Source2
        {
            get => Model.Source2;
            set => Model.Source2 = value.Clamp(ActivedChannels);
        }


        public Single Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String Unit => Model.Unit;
        public Single MaxThreshold => Model.MaxThreshold;
        public Single MinThreshold => Model.MinThreshold;


        public ComModel.ProtocolSATA.SATAVersion Version
        {
            get => Model.Version;
            set => Model.Version = value.Clamp();
        }
        public UInt16 MaxBytesCount => Model.MaxBytesCount;
        public UInt16 MinBytesCount => Model.MinBytesCount;
        public UInt16 BytesCount
        {
            get => Model.BytesCount;
            set => Model.BytesCount = Math.Clamp(value, MinBytesCount, MaxBytesCount);
        }

        public ComModel.ProtocolSATA.SignalType SignalType
        {
            get => Model.SignalType;
            set => Model.SignalType = value.Clamp();
        }
        private protected override SATADecodeModel Model { get; }
    }
}
