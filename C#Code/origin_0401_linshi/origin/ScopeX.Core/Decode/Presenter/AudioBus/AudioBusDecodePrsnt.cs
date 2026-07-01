using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class AudioBusDecodePrsnt : ProtocolPrsnt
    {
        public AudioBusDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            //if (isTrigDecode)
            //    Model = (AudioBusDecodeModel)TriggerSerialShareParameter.Default.GetTriggerDecodeModel(SerialProtocolType.AudioBus);
            //else
            //    Model = (AudioBusDecodeModel)((DecodeModel)DsoModel.Default.GetChannel(id)).GetChDecodeModel(SerialProtocolType.AudioBus);
            Model = (AudioBusDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.AudioBus);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }

        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { SCL, SDA,WS };
        }

        //private protected override AudioBusDecodeModel Model { get; }
        private protected override AudioBusDecodeModelCPP Model { get; }
        public ProtocolAudioBus.SubType SubType
        {
            get => Model.SubType;
            set => Model.SubType = value.Clamp();
        }

        public ProtocolCommon.Polarity SyncPolarity
        {
            get => Model.SyncPolarity;
            set => Model.SyncPolarity = value.Clamp();
        }
        public ChannelId SCL
        {
            get => Model.SCL;
            set => Model.SCL = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ChannelId SDA
        {
            get => Model.SDA;
            set => Model.SDA = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public Double MaxThresholdSDA => Model.MaxThresholdSDA;
        public Double MaxThresholdSCL => Model.MaxThresholdSCL;
        public Double MaxThresholdWS => Model.MaxThresholdWS;
        public Double MinThresholdSDA => -MaxThresholdSDA;
        public Double MinThresholdSCL => -MaxThresholdSCL;
        public Double MinThresholdWS => -MaxThresholdWS;

        public Double SDAThreshold
        {
            get => Model.SDAThreshold;
            set => Model.SDAThreshold = Math.Clamp(value, MinThresholdSDA, MaxThresholdSDA);
        }
        public String SDAUnit => Model.SDAUnit;

        public Double SDAThresholdBymV
        {
            get => SDAThreshold * 1_000D;
            set => SDAThreshold = value / 1000D;
        }

        public Double SCLThreshold
        {
            get => Model.SCLThreshold;
            set => Model.SCLThreshold = Math.Clamp(value, MinThresholdSCL, MaxThresholdSCL);
        }

        public Double SCLThresholdBymV
        {
            get => SCLThreshold * 1_000D;
            set => SCLThreshold = value / 1000D;
        }

        public String SCLUnit=>Model.SCLUnit;

        public Double WSThreshold
        {
            get => Model.WSThreshold;
            set => Model.WSThreshold = Math.Clamp(value, MinThresholdWS, MaxThresholdWS);
        }
        public String WSUnit => Model.WSUnit;

        public Double WSThresholdBymV
        {
            get => WSThreshold * 1_000D;
            set => WSThreshold = value / 1000D;
        }

        public ChannelId WS
        {
            get => Model.WS;
            set => Model.WS = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ProtocolCommon.Edge ClockEdge
        {
            get => Model.ClockEdge;
            set => Model.ClockEdge = value.Clamp();
        }
        public ProtocolCommon.Polarity DataPolarity
        {
            get => Model.DataPolarity;
            set => Model.DataPolarity = value.Clamp();
        }
        public ProtocolAudioBus.MSB_LSB MSB_LSB
        {
            get => Model.MSB_LSB;
            set
                => Model.MSB_LSB = value.Clamp();
        }
        public ProtocolAudioBus.SoundChannel SoundChannel
        {
            get => Model.SoundChannel;
            set
                => Model.SoundChannel = value.Clamp();

        }
        public Int32 MaxBitDelayCount => Model.MaxBitDelayCount;
        public Int32 MinBitDelayCount => Model.MinBitDelayCount;
        public Int32 BitDelayCount
        {
            get => Model.BitDelayCount;
            set
            {
                Model.BitDelayCount = Math.Clamp(value, MinBitDelayCount, MaxBitDelayCount);
            }
        }

        public Int32 MaxDataBitCount => Model.MaxDataBitCount;
        public Int32 MinDataBitCount => Model.MinDataBitCount;


        public Int32 DataBitCount
        {
            get => Model.DataBitCount;
            set
            {
                Model.DataBitCount = Math.Clamp(value, MinDataBitCount, MaxDataBitCount);
            }
        }
        public Int32 MinClockBitNumberPerChannel => Model.MinClockBitNumberPerChannel;
        public Int32 MaxClockBitNumberPerChannel => Model.MaxClockBitNumberPerChannel;
        /// <summary>
        /// 每通道时钟位
        /// </summary>
        public Int32 ClockBitNumberPerChannel
        {
            get => Model.ClockBitNumberPerChannel;
            set
            {
                Model.ClockBitNumberPerChannel = Math.Clamp(value, MinClockBitNumberPerChannel, MaxClockBitNumberPerChannel);
            }
        }

        public Int32 MinChannelNumberPerFream => Model.MinChannelNumberPerFream;
        public Int32 MaxChannelNumberPerFream => Model.MaxChannelNumberPerFream;

        /// <summary>
        /// 每帧通道数量
        /// </summary>
        public Int32 ChannelNumberPerFream
        {
            get => Model.ChannelNumberPerFream;
            set
            {
                Model.ChannelNumberPerFream = Math.Clamp(value, MinChannelNumberPerFream, MaxChannelNumberPerFream);
            }
        }

        public Int32 MaxClockBitCount => Model.MaxClockBitCount;
        public Int32 MinClockBitCount => Model.MinClockBitCount;

        public Int32 ClockBitCount
        {
            get => Model.ClockBitCount;
            set
            {
                Model.ClockBitCount = Math.Clamp(value, MinClockBitCount, MaxClockBitCount);
            }
        }
        public Int32 MaxSoundChannelCount => Model.MaxSoundChannelCount;
        public Int32 MinSoundChannelCount => Model.MinSoundChannelCount;

        public Int32 SoundChannelCount
        {
            get => Model.SoundChannelCount;
            set
            {
                Model.SoundChannelCount = Math.Clamp(value, MinSoundChannelCount, MaxSoundChannelCount);
            }
        }
    }
}
