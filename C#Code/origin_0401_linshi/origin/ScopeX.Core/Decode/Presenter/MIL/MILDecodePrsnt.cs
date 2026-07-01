using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class MILDecodePrsnt : ProtocolPrsnt
    {
        public MILDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (MILDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.MIL);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }
        public String Unit => Model.Unit;

        public Double HighThreshold
        {
            get => Model.HighThreshold;
            set => Model.HighThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public Double HighThresholdBymV
        {
            get => HighThreshold * 1_000D;
            set => HighThreshold = value / 1000D;
        }
        public Double LowThreshold
        {
            get => Model.LowThreshold;
            set => Model.LowThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public Double LowThresholdBymV
        {
            get => LowThreshold * 1_000D;
            set => LowThreshold = value / 1000D;
        }
        public ProtocolMIL.SignalRate SignalRate
        {
            get => Model.SignalRate;
            set => Model.SignalRate = value.Clamp();
        }
        public Int32 RealSignalRate
        {
            get => Model.RealSignalRate;
            set => Model.RealSignalRate = Math.Clamp(value, MinSignalRate, MaxSignalRate);
        }
        public ProtocolCommon.Polarity Polarity
        {
            get => Model.Polarity;
            set => Model.Polarity = value.Clamp();
        }
        public Int32 MaxSignalRate => Model.MaxSignalRate;
        public Int32 MinSignalRate => Model.MinSignalRate;
        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;
        private protected override MILDecodeModelCPP Model { get; }
    }
}
