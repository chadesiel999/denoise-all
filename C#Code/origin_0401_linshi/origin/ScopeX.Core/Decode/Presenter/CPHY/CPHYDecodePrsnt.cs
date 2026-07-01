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
    public class CPHYDecodePrsnt : ProtocolPrsnt
    {
        public CPHYDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (CPHYDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.CPHY);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { A, B, C};
        }

        private protected override CPHYDecodeModelCPP Model { get; }
        public ProtocolCPHY.SubType SubType
        {
            get => Model.SubType;
            set => Model.SubType = value.Clamp();
        }

        public ProtocolCPHY.SignalType SignalType
        {
            get => Model.SignalType;
            set => Model.SignalType = value.Clamp();
        }

        public Int64 BitRate
        {
            get => Model.BitRate;
            set => Model.BitRate = Math.Clamp(value, MinBitRate, MaxBitRate);
        }

        public ChannelId A
        {
            get => Model.A;
            set => Model.A = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ChannelId B
        {
            get => Model.B;
            set => Model.B = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ChannelId C
        {
            get => Model.C;
            set => Model.C = ChannelIdExt.Clamp(value, ActivedChannels);
        }

        public Double MaxThreshold => 8;
        public Double MinThreshold => -8;
        public Int64 MinBitRate => 4_000_000;
        public Int64 MaxBitRate => 10_000_000_000;
        public Double AThreshold
        {
            get => Model.AThreshold;
            set => Model.AThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String AUnit => Model.AUnit;

        public Double AThresholdBymV
        {
            get => AThreshold * 1_000D;
            set => AThreshold = value / 1000D;
        }

        public Double BThreshold
        {
            get => Model.BThreshold;
            set => Model.BThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }

        public Double BThresholdBymV
        {
            get => BThreshold * 1_000D;
            set => BThreshold = value / 1000D;
        }

        public String BUnit => Model.BUnit;

        public Double CThreshold
        {
            get => Model.CThreshold;
            set => Model.CThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String CUnit => Model.CUnit;

        public Double CThresholdBymV
        {
            get => CThreshold * 1_000D;
            set => CThreshold = value / 1000D;
        }

        public Double LPAThreshold
        {
            get => Model.LPAThreshold;
            set => Model.LPAThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String LPAUnit => Model.AUnit;

        public Double LPAThresholdBymV
        {
            get => LPAThreshold * 1_000D;
            set => LPAThreshold = value / 1000D;
        }

        public Double LPCThreshold
        {
            get => Model.LPCThreshold;
            set => Model.LPCThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String LPCUnit => Model.CUnit;

        public Double LPCThresholdBymV
        {
            get => LPCThreshold * 1_000D;
            set => LPCThreshold = value / 1000D;
        }

    }
}
