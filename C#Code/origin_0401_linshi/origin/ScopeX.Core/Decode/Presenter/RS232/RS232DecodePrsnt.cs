using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// RS232的通道解码Prsnt
    /// </summary>
    public class RS232DecodePrsnt : ProtocolPrsnt
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">通道</param>
        /// <param name="view">View</param>
        public RS232DecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (RS232DecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.RS232);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }

        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }
        /// <summary>
        /// Model
        /// </summary>
        private protected override RS232DecodeModel Model
        {
            get;
        }


        /// <summary>
        /// 波特率
        /// </summary>
        public UInt32 BitRate
        {
            get => Model.BitRate;
            set
            {
                Model.BitRate = Math.Clamp(value, MinSignalRate, MaxSignalRate);
            }
        }

        public UInt32 MaxSignalRate => Model.MaxSignalRate;
        public UInt32 MinSignalRate => Model.MinSignalRate;

        /// <summary>
        /// 数据位
        /// </summary>
        public ProtocolRS232.DataBitWidth DataBits
        {
            get => Model.DataBits;
            set
            {
                Model.DataBits = value.Clamp();
            }
        }

        /// <summary>
        /// 校验位
        /// </summary>
        public ProtocolRS232.OddEvenCheck Parity
        {
            get => Model.Parity;
            set
            {
                if (value.Clamp() != Model.Parity)
                {
                    Model.Parity = value.Clamp();
                }
            }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public ProtocolRS232.StopBit StopBits
        {
            get => Model.StopBits;
            set
            {
                Model.StopBits = value.Clamp();
            }
        }

        /// <summary>
        /// 通道1
        /// </summary>
        public ChannelId Source1
        {
            get => Model.Source1;
            set
            {
                Model.Source1 = value.Clamp(ActivedChannels);
            }
        }

        /// <summary>
        /// 字节顺序
        /// </summary>
        public ProtocolRS232.MSB_LSB ByteOrder
        {
            get => Model.ByteOrder;
            set
            {
                Model.ByteOrder = value.Clamp();
            }
        }

        public Double MinThreshold => Model.MinThreshold;
        public Double MaxThreshold => Model.MaxThreshold;
        /// <summary>
        /// 阈值
        /// </summary>
        public Double Threshold
        {
            get => Model.Threshold;
            set
            {
                Model.Threshold = Math.Clamp(value, MinThreshold, MaxThreshold);
            }
        }

        public String Unit => Model.Unit;

        public Double ThresholdBymV
        {
            get => Threshold * 1_000D;
            set => Threshold = value / 1000D;
        }

        /// <summary>
        /// 是否反向
        /// </summary>
        public ProtocolCommon.Polarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value.Clamp();
            }
        }

        /// <summary>
        /// 当为差分信号时的通道2
        /// </summary>
        public ChannelId Source2
        {
            get => Model.Source2;
            set
            {
                Model.Source2 = value.Clamp(ActivedChannels);
            }
        }

        /// <summary>
        /// 信号类型，分为单端和差分
        /// </summary>
        public ProtocolRS232.SignalType ChType
        {
            get => Model.SignalType;
            set
            {
                Model.SignalType = value.Clamp();
            }
        }


        public ProtocolRS232.BPSList Baud
        {
            get => Model.Baud;
            set
            {
                Model.Baud = value;
                Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            }
        }

    }
}
