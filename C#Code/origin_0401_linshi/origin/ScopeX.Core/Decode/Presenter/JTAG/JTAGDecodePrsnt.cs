using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class JTAGDecodePrsnt : ProtocolPrsnt
    {
        public JTAGDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (JTAGDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.JTAG);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { TCK, TMS,TDI,TDO };
        }

        // <summary>
        /// Model
        /// </summary>
        private protected override JTAGDecodeModel Model
        {
            get;
        }


        public ChannelId TCK
        {
            get => Model.TCK;
            set => Model.TCK = value.Clamp(ActivedChannels);
        }


        public Single TCKThreshold
        {
            get => Model.TCKThreshold;
            set => Model.TCKThreshold = Math.Clamp(value, MinThresholdTCK, MaxThresholdTCK);
        }
        public String TCKUnit => Model.TCKUnit;

        public Single MaxThresholdTDO => Model.MaxThresholdTDO;
        public Single MaxThresholdTCK => Model.MaxThresholdTCK;
        public Single MaxThresholdTDI => Model.MaxThresholdTDI;
        public Single MaxThresholdTMS => Model.MaxThresholdTMS;
        public Single MinThresholdTDO => Model.MinThresholdTDO;
        public Single MinThresholdTCK => Model.MinThresholdTCK;
        public Single MinThresholdTDI => Model.MinThresholdTDI;
        public Single MinThresholdTMS => Model.MinThresholdTMS;

        public ChannelId TMS
        {
            get => Model.TMS;
            set => Model.TMS = value.Clamp(ActivedChannels);
        }


        public Single TMSThreshold
        {
            get => Model.TMSThreshold;
            set => Model.TMSThreshold = Math.Clamp(value, MinThresholdTMS, MaxThresholdTMS);
        }


        public ChannelId TDI
        {
            get => Model.TDI;
            set => Model.TDI = value.Clamp(ActivedChannels);
        }


        public Single TDIThreshold
        {
            get => Model.TDIThreshold;
            set => Model.TDIThreshold = Math.Clamp(value, MinThresholdTDI, MaxThresholdTDI);
        }
        public String TDIUnit => Model.TDIUnit;


        public ChannelId TDO
        {
            get => Model.TDO;
            set => Model.TDO = value.Clamp(ActivedChannels);
        }


        public Single TDOThreshold
        {
            get => Model.TDOThreshold;
            set => Model.TDOThreshold = Math.Clamp(value, MinThresholdTDO, MaxThresholdTDO);
        }

        public String TMSUnit => Model.TMSUnit;
        public String TDOUnit => Model.TDOUnit;

        public ComModel.ProtocolJTAG.DecodeChannel DecodeChannel
        {
            get => Model.DecodeChannel;
            set => Model.DecodeChannel = value.Clamp();
        }
        public UInt32 MaxBitRate => Model.MaxBitRate;
        public UInt32 MinBitRate => Model.MinBitRate;

        public UInt32 BitRate
        {
            get => Model.BitRate;
            set => Model.BitRate = Math.Clamp(value, MinBitRate, MaxBitRate);
        }
    }
}
