using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using SixLabors.ImageSharp.ColorSpaces;

namespace ScopeX.Core.Decode
{
    public class LINDecodePrsnt : ProtocolPrsnt
    {
        public LINDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (LINDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.LIN);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }

        //private protected override LINDecodeModel Model { get; }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }
        private protected override LINDecodeModelCPP Model { get; }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }


        public ProtocolLIN.Standard Standard
        {
            get => Model.Standard;
            set => Model.Standard = value.Clamp();
        }

        public Boolean CheckPIDParity
        {
            get => Model.CheckPIDParity;
            set => Model.CheckPIDParity = value;
        }

        public ProtocolCommon.Polarity Polarity
        {
            get => Model.Polarity;
            set => Model.Polarity = value.Clamp();
        }

        public ProtocolLIN.PIncludeOddEven PIncludeOddEven
        {
            get => Model.PIncludeOddEven;
            set => Model.PIncludeOddEven = value.Clamp();
        }

        public Double Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }

        public Boolean ParityWithId
        {
            get => Model.ParityWithId;
            set => Model.ParityWithId = value;
        }

        public String Unit => Model.Unit;

        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;

        public Int32 DataCount
        {
            get => Model.DataCount;
            set => Model.DataCount = Math.Clamp(value,MinDataCount,MaxDataCount);
        }
        public Int32 MaxDataCount => Model.MaxDataCount;
        public Int32 MinDataCount => Model.MinDataCount;


        public ProtocolLIN.BPS_ID BPS
        {
            get => Model.BPS;
            set => Model.BPS = value.Clamp();
        }

        public Int32 CustomBPS
        {
            get => Model.CustomBPS;
            set => Model.CustomBPS = Math.Clamp(value, MinBPS, MaxBPS);
        }
        public Int32 MaxBPS => LINDecodeModelCPP.MaxBPS;
        public Int32 MinBPS => LINDecodeModelCPP.MinBPS;

        public Int32 SamplePoint
        {
            get => Model.SamplePoint;
            set
            {
                Model.SamplePoint = Math.Clamp(value, MinSamplePoint, MaxSamplePoint);
            }
        }
        public Int32 MaxSamplePoint => Model.MaxSamplePoint;
        public Int32 MinSamplePoint => Model.MinSamplePoint;
    }
}
