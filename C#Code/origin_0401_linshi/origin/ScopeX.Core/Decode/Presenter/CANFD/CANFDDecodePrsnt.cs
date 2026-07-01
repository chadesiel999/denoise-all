using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class CANFDDecodePrsnt : ProtocolPrsnt
    {
        public CANFDDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (CANFDDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.CAN_FD);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }

        public Dictionary<ProtocolCANFD.FDSignalRate, Int64> FDSignalRateMap
        {
            get => Model.FDSignalRateMap;
        }
        public Dictionary<ProtocolCANFD.SDSignalRate, Int64> SDSignalRateMap
        {
            get => Model.SDSignalRateMap;
        }
        public Int32 MaxSamplePoint => Model.MaxSamplePoint;
        public Int32 MinSamplePoint => Model.MinSamplePoint;

        /// <summary>
        /// 仲裁域采样率  采样点(%)
        /// </summary>
        public Int32 SamplePoint
        {
            get => Model.SamplePoint;
            set
            {
                Model.SamplePoint = Math.Clamp(value, MinSamplePoint, MaxSamplePoint);
            }
        }

        public Int32 MaxDataSamplePoint => Model.MaxDataSamplePoint;

        public Int32 MinDataSamplePoint => Model.MinDataSamplePoint;

        /// <summary>
        /// 数据域采样点
        /// </summary>
        public Int32 DataSamplePoint
        {
            get => Model.DataSamplePoint;
            set => Model.DataSamplePoint = value;
        }

        public Double MaxThreshold => Model.MaxThreshold;


        public Double MinThreshold => Model.MinThreshold;
        //数据源的阈值
        public Double SDAThreshold
        {
            get => Model.SDAThreshold;
            set
            {
                Model.SDAThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
            }
        }
        public String SDAUnit => Model.SDAUnit;

        public Double SDAThresholdBymV
        {
            get => SDAThreshold * 1000D;
            set => SDAThreshold = value / 1000D;
        }

        //输入1
        public ChannelId Source1
        {
            get => Model.Source1;
            set
            {
                Model.Source1 = value.Clamp(ActivedChannels);
            }
        }

        //输入2(信号类型选择"差分"时使用)
        public ChannelId Source2
        {
            get => Model.Source2;
            set
            {
                Model.Source2 = value.Clamp(ActivedChannels);
            }
        }

        //信号速率
        public ProtocolCANFD.SDSignalRate SDSignalRate
        {
            get => Model.SDSignalRate;
            set
            {
                Model.SDSignalRate = value.Clamp();
            }
        }
        //自定义的信号速率（当SignalRate == TriggerCAN_FDSignalRate.CAN_FDSignalRate_custom时使用）
        public Int64 SDCustomSignalRate
        {
            get => Model.SDCustomSignalRate;
            set
            {
                Model.SDCustomSignalRate = Math.Clamp(value, MinSDSignalRate, MaxSDSignalRate);
            }
        }
        public Int64 MaxSDSignalRate => CANFDDecodeModelCPP.MaxSDSignalRate;
        public Int64 MinSDSignalRate => CANFDDecodeModelCPP.MinSDSignalRate;

        public ProtocolCANFD.FDSignalRate FDSignalRate
        {
            get => Model.FDSignalRate;
            set
            {
                Model.FDSignalRate = value.Clamp();
            }
        }
        //自定义的信号速率（当SignalRate == TriggerCAN_FDSignalRate.CAN_FDSignalRate_custom时使用）
        public Int64 FDCustomSignalRate
        {
            get => Model.FDCustomSignalRate;
            set
            {
                Model.FDCustomSignalRate = Math.Clamp(value, MinFDSignalRate, MaxFDSignalRate);
            }
        }
        public Int64 MaxFDSignalRate => CANFDDecodeModelCPP.MaxFDSignalRate;
        public Int64 MinFDSignalRate => CANFDDecodeModelCPP.MinFDSignalRate;

        //信号类型
        public ProtocolCANFD.SignalType SignalType
        {
            get => Model.SignalType;
            set
            {
                Model.SignalType = value.Clamp();
            }
        }

        private protected override CANFDDecodeModelCPP Model { get; }
    }
}
