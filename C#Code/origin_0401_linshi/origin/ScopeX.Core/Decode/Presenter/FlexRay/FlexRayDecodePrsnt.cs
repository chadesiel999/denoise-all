using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class FlexRayDecodePrsnt : ProtocolPrsnt
    {
        public FlexRayDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (FlexRayDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.FlexRay);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        private protected override FlexRayDecodeModelCPP Model { get; }

        //源类型
        //  private ProtocolFlexRay.SourceType _SourceType = ProtocolFlexRay.SourceType.Tx_Rx;
        public ProtocolFlexRay.SourceType SourceType
        {
            get => Model.SourceType;
            set
            {
                Model.SourceType = value.Clamp();
            }
        }
        //源1
        //  private ChannelId _Source1 = ChannelId.C1;
        public ChannelId Source1
        {
            get => Model.Source1;
            set
            {
                Model.Source1 = value.Clamp(ActivedChannels);
            }
        }
        //源2
        // private ChannelId _Source2 = ChannelId.C1;
        public ChannelId Source2
        {
            get => Model.Source2;
            set => Model.Source2 = value.Clamp(ActivedChannels);
        }

        public Double Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold1, MaxThreshold1);
        }
        public String Unit => Model.Unit;
        public Double ThresholdBymV
        {
            get => Threshold * 1_000D;
            set => Threshold = value / 1000D;
        }
        public Double MaxThreshold1 => Model.MaxThreshold1;
        public Double MaxThreshold2 => Model.MaxThreshold2;

        public Double MinThreshold1 => Model.MinThreshold1;

        public Double MinThreshold2 => Model.MinThreshold2;
        //信号速率
        public ProtocolFlexRay.SignalRate SignalRate
        {
            get => Model.SignalRate;
            set => Model.SignalRate = value.Clamp();
        }
        //自定义的信号速率
        public Int64 CustomSignalRate
        {
            get => Model.CustomSignalRate;
            set
            {
                Model.CustomSignalRate = value;
            }
        }

        public Int32 MaxSignalRate => FlexRayDecodeModelCPP.MaxSignalRate;


        public Int32 MinSignalRate => FlexRayDecodeModelCPP.MinSignalRate;
        //通道类型
        public ProtocolFlexRay.ChannelType ChannelType
        {
            get => Model.ChannelType;
            set
            {
                Model.ChannelType = value.Clamp();
            }
        }

    }
}
