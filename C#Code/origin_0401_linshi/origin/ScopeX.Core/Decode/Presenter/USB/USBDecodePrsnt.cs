using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using static ScopeX.Core.Decode.DecoderTypes;

namespace ScopeX.Core.Decode
{
    public class USBDecodePrsnt : ProtocolPrsnt
    {

        public USBDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            //if (isTrigDecode)
            //    Model = (USBDecodeModel)TriggerSerialShareParameter.Default.GetTriggerDecodeModel(SerialProtocolType.USB);
            //else

            Model = (USBDecodeModelCPP)((DecodeModel)DsoModel.Default.GetChannel(id)).GetChDecodeModel(SerialProtocolType.USB);

            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        private protected override USBDecodeModelCPP Model { get; }


        public String Source1Unit => Model.Source1Unit;
        public ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = value.Clamp(ActivedChannels);
        }
        public String Source2Unit => Model.Source2Unit;
        public ChannelId Source2
        {
            get => Model.Source2;
            set => Model.Source2 = value.Clamp(ActivedChannels);
        }

        public Single MaxThresholdH => Model.MaxThresholdH;
        public Single MinThresholdH => Model.MinThresholdH;

        public Single ThresholdH
        {
            get => Model.ThresholdH;
            set => Model.ThresholdH = Math.Clamp(value, MinThresholdH, MaxThresholdH);
        }
        public Single MaxThresholdL => Model.MaxThresholdL;
        public Single MinThresholdL => Model.MinThresholdL;
        public Single ThresholdL
        {
            get => Model.ThresholdL;
            set => Model.ThresholdL = Math.Clamp(value, MinThresholdL, MaxThresholdL);
        }

        public Single MaxThreshold2 => Model.MaxThreshold2;
        public Single MinThreshold2 => Model.MinThreshold2;
        public Single Threshold2
        {
            get => Model.Threshold2;
            set => Model.Threshold2 = Math.Clamp(value, MinThreshold2, MaxThreshold2);
        }

        public ProtocolUSB.USBSpeed BaudMode
        { 
            get => Model.BaudMode;
            set => Model.BaudMode = value;
        }

        public ProtocolUSB.USBInputType InputType
        {
            get => Model.InputType;
            set => Model.InputType = value;
        }

    }
}
