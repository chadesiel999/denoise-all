using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class PCIeDecodePrsnt : ProtocolPrsnt
    {
        public PCIeDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (PCIeDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.PCIe);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }
        private protected override PCIeDecodeModel Model { get; }
        public Single MinThreshold1 => Model.MinThreshold1;
        public Single MinThreshold2 => Model.MinThreshold2;
        public Single MaxThreshold1 => Model.MaxThreshold1;
        public Single MaxThreshold2 => Model.MaxThreshold2;
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }
        public Single Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold1, MaxThreshold1);
        }
        public String Unit => Model.Unit;

        public ProtocolPCIe.PCIeVersion Version
        {
            get => Model.Version;
            set => Model.Version = value.Clamp();
        }
        public ProtocolPCIe.SignalType SignalType
        {
            get => Model.SignalType;
            set => Model.SignalType = value.Clamp();
        }
        public UInt16 MaxBytesCount => Model.MaxBytesCount;
        public UInt16 MinBytesCount => Model.MinBytesCount;
        public UInt16 BytesCount
        {
            get => Model.BytesCount;
            set => Model.BytesCount = Math.Clamp(value, MinBytesCount, MaxBytesCount);
        }

        public ChannelId SignalInput1
        {
            get => Model.SignalInput1;
            set => Model.SignalInput1 = value.Clamp(ActivedChannels);
        }
    }
}
