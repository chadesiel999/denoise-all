using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class EthernetDecodePrsnt : ProtocolPrsnt
    {
        public EthernetDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (EthernetDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.Ethernet);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        public ProtocolEthernet.EthernetVersion Version
        {
            get => Model.Version;
            set => Model.Version = value.Clamp();
        }

        public Single MinThreshold1 => Model.MinThreshold1;
        public Single MaxThreshold2 => Model.MaxThreshold2;
        public Single MinThreshold2 => Model.MinThreshold2;
        public Single MaxThreshold1 => Model.MaxThreshold1;
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Single Source1ThresholdH
        {
            get => Model.Source1ThresholdH;
            set => Model.Source1ThresholdH = Math.Clamp(value, MinThreshold1, MaxThreshold1);
        }

        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Single Source1ThresholdL
        {
            get => Model.Source1ThresholdL;
            set => Model.Source1ThresholdL = Math.Clamp(value, MinThreshold2, MaxThreshold2);
        }

        public ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = value.Clamp(ActivedChannels);
        }

        public ChannelId Source2
        {
            get => Model.Source2;
            set => Model.Source2 = value.Clamp(ActivedChannels);
        }

        public ProtocolEthernet.SignalType SignalType
        {
            get => Model.SignalType;
            set => Model.SignalType = value.Clamp();
        }

        public ProtocolEthernet.EthernetSpeed Speed
        {
            get => Model.Speed;
            set => Model.Speed = value.Clamp();
        }
        //public Single Source1Threshold
        //{
        //    get => Model.Source1Threshold;
        //    set => Model.Source1Threshold = Math.Clamp(value, MinThreshold1, MaxThreshold1);
        //}
        public String Source1Unit => Model.Source1Unit;
        //public Single Source2Threshold
        //{
        //    get => Model.Source2Threshold;
        //    set => Model.Source2Threshold = Math.Clamp(value, MinThreshold2, MaxThreshold2);
        //}
        public String Source2Unit => Model.Source2Unit;

        public Boolean QFlag
        {
            get => Model.QFlag;
            set => Model.QFlag = value;
        }
        private protected override EthernetDecodeModel Model { get; }
    }
}
