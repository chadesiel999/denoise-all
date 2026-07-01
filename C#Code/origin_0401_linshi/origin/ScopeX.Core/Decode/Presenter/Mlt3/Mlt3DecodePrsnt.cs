using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolCommon;

namespace ScopeX.Core.Decode
{
    public class Mlt3DecodePrsnt : ProtocolPrsnt
    {
        public Mlt3DecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (Mlt3DecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.Mlt3);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }

        private protected override Mlt3DecodeModel Model { get; }
        public Double MaxHighThreshold => Model.MaxHighThreshold;
        public Double MinHighThreshold => Model.MinHighThreshold;
        public Double MaxLowThreshold => Model.MaxLowThreshold;
        public Double MinLowThreshold => Model.MinLowThreshold;
        public Int64 MaxBaudRate => Model.MaxBaudrate;
        public Int64 MinBaudRate => Model.MinBaudrate;
        public UInt32 MaxZeroCount => Model.MaxZeroCount;
        public UInt32 MinZeroCount => Model.MinxZeroCount;
        public String Unit => Model.Unit;
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }

        /*门限阈值输入*/
        public Double HighThreshold
        {
            get => Model.HighThreshold;
            set => Model.HighThreshold = Math.Clamp(value, MinHighThreshold, MaxHighThreshold);
        }

        /*门限阈值输入*/
        public Double LowThreshold
        {
            get => Model.LowThreshold;
            set => Model.LowThreshold = Math.Clamp(value, MinHighThreshold, MaxHighThreshold);
        }

        /*数据速率输入*/
        public Int64 Baudrate
        {
            get => Model.Baudrate;
            set => Model.Baudrate = Math.Clamp(value, MinBaudRate, MaxBaudRate);
        }

        public UInt32 ZeroCount
        {
            get => Model.ZeroCount;
            set => Model.ZeroCount = Math.Clamp(value, MinZeroCount, MaxZeroCount);
        }
    }
}
