using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class PSI5DecodePrsnt : ProtocolPrsnt
    {
        public PSI5DecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (PSI5DecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.PSI5);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        private protected override PSI5DecodeModel Model { get; }

        public ProtocolPSI5.Psi5BaudMode BaudMode
        {
            get => Model.BaudMode;
            set
            {
                Model.BaudMode = value.Clamp();
            }
        }

        public Single MaxFrameControl => 4;
        public Single MinFrameControl => 0;
        public ProtocolPSI5.Psi5FrameControl FrameControl
        {
            get => Model.FrameControl;
            set
            {
                Model.FrameControl = value.Clamp();
            }
        }

        public ProtocolPSI5.Psi5SerialMessage SerialMessage
        {
            get => Model.SerialMessage;
            set
            {
                Model.SerialMessage = value.Clamp();
            }
        }

        public ProtocolPSI5.Psi5Status Status
        {
            get => Model.Status;
            set
            {
                Model.Status = value.Clamp();
            }
        }


        public ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = value.Clamp(ActivedChannels);
        }

        public Single MaxThreshold => Model.MaxThreshold;
        public Single MinThreshold => Model.MinThreshold;
        public Single Threshold1
        {
            get => Model.Threshold1;
            set => Model.Threshold1 = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String DataUnit => Model.Unit;

        public String BitUnit => Model.BitUnit;
        public Single MaxDataABits => Model.MaxDataABitsSize;
        public Single MinDataABits => Model.MinDataABitsSize;
        public UInt16 DataABitsSize
        { 
            get => (UInt16)Model.DataABitsSize;
            set => Model.DataABitsSize = value;
        }

        public Single MaxDataBBits => Model.MaxDataBBitsSize;
        public Single MinDataBBits => Model.MinDataBBitsSize;
        public UInt16 DataBBitsSize
        {
            get => (UInt16)Model.DataBBitsSize;
            set => Model.DataBBitsSize = value;
        }

        public Single MaxPayload => Model.MaxPayloadSize;
        public Single MinPayload => Model.MinPayloadSize;
        public UInt16 PayloadSize
        {
            get => (UInt16)Model.PayloadSize;
            //set => Model.PayloadSize = value;
        }
    }
}
