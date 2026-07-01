using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using static ScopeX.ComModel.ProtocolCommon;

namespace ScopeX.Core.Decode
{
    public class NRZDecodePrsnt : ProtocolPrsnt
    {
        public NRZDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (NRZDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.NRZ);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        public UInt32 MinIdleTime => Model.MinIdleTime;
        public UInt32 MaxIdleTime => Model.MaxIdleTime;
        
        public ProtocolNRZ.Mode Mode
        {
            get => Model.Mode;
            set => Model.Mode = value.Clamp();
        }

        public ProtocolNRZ.MSB_LSB MSB_LSB
        {
            get => Model.MSB_LSB;
            set => Model.MSB_LSB = value.Clamp();
        }
        public ProtocolNRZ.SignalRate SignalRate
        {
            get => Model.SignalRate;
            set => Model.SignalRate = value.Clamp();
        }
        public Single MinThreshold1 => Model.MinThreshold1;
        //public Single MinThreshold2 => Model.MinThreshold2;
        public Single MaxThreshold1 => Model.MaxThreshold1;
        //public Single MaxThreshold2 => Model.MaxThreshold2;
        public Single Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold1, MaxThreshold1);
        }
        public String Unit => Model.Unit;

        public Int64 MinSignalRate => Model.MinSignalRate;
        public Int64 MaxSignalRate => Model.MaxSignalRate;
        public Int64 RealSignalRate
        {
            get => Model.RealSignalRate;
            set => Model.RealSignalRate = Math.Clamp(value, MinSignalRate, MaxSignalRate);
        }
        public Polarity Polarity
        {
            get => Model.Polarity;
            set => Model.Polarity = value;
        }
        //public ProtocolNRZ.SignalType SignalType
        //{
        //    get => Model.SignalType;
        //    set => Model.SignalType = value.Clamp();
        //}
        public ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = value.Clamp(ActivedChannels);
        }
        //public ChannelId Source2
        //{
        //    get => Model.Source2;
        //    set => Model.Source2 = value.Clamp(ActivedChannels);
        //}

        private protected override NRZDecodeModel Model { get; }
    }
}
