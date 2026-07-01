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
    /// <summary>
    /// I2C的通道解码Prsnt
    /// </summary>
    public class I2CDecodePrsnt : ProtocolPrsnt
    {
        public I2CDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (I2CDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.I2C);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { SCLK,SDA };
        }

        /// <summary>
        /// Model
        /// </summary>
        private protected override I2CDecodeModel Model { get; }


        /// <summary>
        /// 时钟源
        /// </summary>
        public ChannelId SCLK
        {
            get => Model.SCLK;
            set
            {
                Model.SCLK = value.Clamp(ActivedChannels);
            }
        }

        public Double MinThresholdSDA => Model.MinThresholdSDA;
        public Double MinThresholdSCL => Model.MinThresholdSCL;
        public Double MaxThresholdSDA => Model.MaxThresholdSDA;
        public Double MaxThresholdSCL => Model.MaxThresholdSCL;
        /// <summary>
        /// 时钟源的阈值
        /// </summary>
        public Double SCLKThreshold
        {
            get => Model.SCLKThreshold;
            set
            {
                Model.SCLKThreshold = Math.Clamp(value, MinThresholdSCL, MaxThresholdSCL);
            }
        }
        public String SCLKUnit => Model.SCLKUnit;

        public Double SCLKThresholdBymV
        {
            get => SCLKThreshold * 1_000D;
            set => SCLKThreshold = value / 1000D;
        }

        public ProtocolI2C.AddrBitWidth BitWidth
        {
            get => Model.BitWidth;
            set
            {
                Model.BitWidth = value.Clamp();
                if (TriggerSerialShareParameter.Default.ProtocolType == SerialProtocolType.I2C && IsTrigger)
                {
                    if (TriggerPrsnt.GetOrMakeTriggerSerial(Dso, TriggerSerialShareParameter.Default.ProtocolType) is I2CTrigSerialPrsnt i2c)
                    {
                        i2c.BitWidth = Model.BitWidth;
                    }
                }
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public ChannelId SDA
        {
            get => Model.SDA;
            set
            {
                Model.SDA = value.Clamp(ActivedChannels);
            }
        }
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Double SDAThreshold
        {
            get => Model.SDAThreshold;
            set
            {
                Model.SDAThreshold = Math.Clamp(value, MinThresholdSDA, MaxThresholdSDA);
            }
        }

        public String SDAUnit => Model.SDAUnit;

        public Double SDAThresholdBymV
        {
            get => SDAThreshold * 1_000D;
            set => SDAThreshold = value / 1000D;
        }
    }
}
