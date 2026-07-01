using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class SMBusDecodePrsnt : ProtocolPrsnt
    {
        public SMBusDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (SMBusDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.SMBus);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { SMBData, SMBClk };
            //return new List<ChannelId>() { SData, SCLK };
        }
        private protected override SMBusDecodeModel Model { get; }

        //public ProtocolSMBus.CheckType CheckType
        //{
        //    get => Model.CheckType;
        //    set
        //    {
        //        Model.CheckType = value.Clamp();
        //    }
        //}
        public ProtocolSMBus.PECByte PECByte
        {
            get => Model.PECByte;
            set
            {
                Model.PECByte = value;
                //Model.PECByte = value.Clamp();
            }
        }

        public ChannelId SMBData
        {
            get => Model.SMBData;
            set => Model.SMBData = value.Clamp(ActivedChannels);
        }
        public ChannelId SMBClk
        {
            get => Model.SMBClk;
            set => Model.SMBClk = value.Clamp(ActivedChannels);
        }
        //public Single MinThreshold => Model.MinThreshold;
        //public Single MaxThreshold => Model.MaxThreshold;
        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;
        public Double DataThreshold //Single
        {
            get => Model.DataThreshold;
            set => Model.DataThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String DataUnit => Model.DataUnit;
        public Double CLKThreshold
        {
            get => Model.CLKThreshold;
            set => Model.CLKThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String CLKUnit => Model.CLKUnit;

    }
}
