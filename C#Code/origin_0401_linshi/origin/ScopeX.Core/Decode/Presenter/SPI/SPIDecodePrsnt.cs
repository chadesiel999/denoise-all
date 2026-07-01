using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;

namespace ScopeX.Core.Decode
{
    public class SPIDecodePrsnt : ProtocolPrsnt
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">通道</param>
        /// <param name="view">View</param>
        public SPIDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (SPIDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.SPI);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { CLK, CS, MISO };
        }
        public ComModel.ProtocolSPI.MSB_LSB ByteOrder
        {
            get => Model.ByteOrder;
            set
            {
                Model.ByteOrder = value.Clamp();
            }
        }

        public ComModel.ChannelId CLK
        {
            get => Model.CLK;
            set
            {
                Model.CLK = value.Clamp(ActivedChannels);
            }
        }

        public ComModel.ProtocolCommon.Edge CLKState
        {
            get => Model.CLKState;
            set
            {
                Model.CLKState = value.Clamp();
            }
        }

        public Double CLKThreshold
        {
            get => Model.CLKThreshold;
            set
            {
                Model.CLKThreshold = Math.Clamp(value, MinThresholdCLK, MaxThresholdCLK);
            }
        }

        public Double CLKThresholdBymV
        {
            get => CLKThreshold * 1_000D;
            set => CLKThreshold = value / 1000D;
        }
        public String CLKUnit => Model.CLKUnit;

        public ComModel.ChannelId CS
        {
            get => Model.CS;
            set
            {
                Model.CS = value.Clamp(ActivedChannels);
            }
        }

        public ComModel.ProtocolSPI.LevelState CSLevelState
        {
            get => Model.CSLevelState;
            set
            {
                Model.CSLevelState = value.Clamp();
            }
        }

        public Double MinThresholdCLK => Model.MinThresholdCLK;
        public Double MaxThresholdCLK => Model.MaxThresholdCLK;
        public Double MinThresholdMOSI => Model.MinThresholdMOSI;
        public Double MaxThresholdMOSI => Model.MaxThresholdMOSI;
        public Double MinThresholdMISO => Model.MinThresholdMISO;
        public Double MaxThresholdMISO => Model.MaxThresholdMISO;

        public Double MinThresholdCS => Model.MinThresholdCS;
        public Double MaxThresholdCS => Model.MaxThresholdCS;
        public Double CSThreshold
        {
            get => Model.CSThreshold;
            set => Model.CSThreshold = Math.Clamp(value, MinThresholdCS, MaxThresholdCS);
        }
        public String CSUnit => Model.CSUnit;

        public Double CSThresholdBymV
        {
            get => CSThreshold * 1_000D;
            set => CSThreshold = value / 1000D;
        }

        public Int32 MaxFrameCount => Model.MaxFrameCount;
        public Int32 MinFrameCount => Model.MinFrameCount;
        public Int32 FrameCount
        {
            get => Model.FrameCount;
            set => Model.FrameCount = Math.Clamp(value, MinFrameCount, MaxFrameCount);
        }

        public ComModel.ProtocolSPI.DecodeChannel DecodeChannel
        {
            get => Model.DecodeChannel;
            set
            {
                Model.DecodeChannel = ProtocolSPI.DecodeChannel.MOSI;
            }
        }

        public ComModel.ProtocolSPI.FramingMode FramingMode
        {
            get => Model.FramingMode;
            set
            {
                Model.FramingMode = value.Clamp();
            }
        }
        public Double MaxOutTime => Model.MaxOutTime;
        public Double MinOutTime => Model.MinOutTime;

        public Double OutTime
        {
            get => Model.OutTime;
            set
            {
                Model.OutTime = Math.Clamp(value, MinOutTime, MaxOutTime);
            }
        }

        public ComModel.ChannelId MISO
        {
            get => Model.MISO;
            set
            {
                Model.MISO = value.Clamp(ActivedChannels);
            }
        }

        public ComModel.ProtocolCommon.Polarity MISOPolarity
        {
            get => Model.MISOPolarity;
            set
            {
                Model.MISOPolarity = value.Clamp();
            }
        }

        public Double MISOThreshold
        {
            get => Model.MISOThreshold;
            set
            {
                Model.MISOThreshold = Math.Clamp(value, MinThresholdMISO, MaxThresholdMISO);
            }
        }
        public String MISOUnit => Model.MISOUnit;

        public Double MISOThresholdBymV
        {
            get => MISOThreshold * 1_000D;
            set => MISOThreshold = value / 1000D;
        }

        public ComModel.ChannelId MOSI
        {
            get => Model.MOSI;
            set
            {
                Model.MOSI = value.Clamp(ActivedChannels);
            }
        }

        public ComModel.ProtocolCommon.Polarity MOSIPolarity
        {
            get => Model.MOSIPolarity;
            set
            {
                Model.MOSIPolarity = value.Clamp();
            }
        }

        public Double MOSIThreshold
        {
            get => Model.MOSIThreshold;
            set
            {
                Model.MOSIThreshold = Math.Clamp(value, MinThresholdMOSI, MaxThresholdMOSI);
            }
        }
        public String MOSIUnit => Model.MOSIUnit;

        public Double MOSIThresholdBymV
        {
            get => MOSIThreshold * 1_000D;
            set => MOSIThreshold = value / 1000D;
        }

        /// <summary>
        /// Model
        /// </summary>
        private protected override SPIDecodeModel Model
        {
            get;
        }
    }
}
