using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class I3CDecodePrsnt : ProtocolPrsnt
    {
        public I3CDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (I3CDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.I3C);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { SData,SCLK };
        }
        private protected override I3CDecodeModel Model { get; }

        public ProtocolSPMI.CheckType CheckType
        {
            get => Model.CheckType;
            set
            {
                Model.CheckType = value.Clamp();
            }
        }

        public ChannelId SData
        {
            get => Model.SData;
            set => Model.SData = value.Clamp(ActivedChannels);
        }
        public ChannelId SCLK
        {
            get => Model.SCLK;
            set => Model.SCLK = value.Clamp(ActivedChannels);
        }
        public Single MaxThreshold => Model.MaxThreshold;
        public Single MinThreshold => Model.MinThreshold;
        public Single DataThreshold
        {
            get => Model.DataThreshold;
            set => Model.DataThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String DataUnit => Model.DataUnit;
        public Single CLKThreshold
        {
            get => Model.CLKThreshold;
            set => Model.CLKThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String CLKUnit => Model.CLKUnit;

    }
}
