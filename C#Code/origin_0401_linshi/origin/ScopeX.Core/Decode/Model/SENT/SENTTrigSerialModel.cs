using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class SENTTrigSerialModel : TriggerSerialModel
    {
        private ProtocolSENT.ChannelMode _ChannelMode;
        public ProtocolSENT.ChannelMode ChannelMode
        {
            get { return _ChannelMode; }
            set { UpdateProperty(ref _ChannelMode, value); }
        }

        #region 快速通道触发参数
        private ProtocolSENT.FastCondition _FastCondition;

        public ProtocolSENT.FastCondition FastCondition
        {
            get { return _FastCondition; }
            set { UpdateProperty(ref _FastCondition, value); }
        }

        private ProtocolSENT.FastChannelMode _FastChannelMode;

        public ProtocolSENT.FastChannelMode FastChannelMode
        {
            get { return _FastChannelMode; }
            set
            {
                if (_FastChannelMode != value)
                {
                    UpdateProperty(ref _FastChannelMode, value);
                    FastChannelData = Math.Clamp(_FastChannelData, MinFastChannelData, MaxFastChannelData);
                }
            }
        }

        private ProtocolSENT.DataLength _DataLength;//根据解码长度自适应

        public ProtocolSENT.DataLength DataLength
        {
            get { return _FastChannelMode == ProtocolSENT.FastChannelMode.Nibbles ? _DataLength : ProtocolSENT.DataLength.Nibbles_6; }
            set
            {
                if (_DataLength != value)
                {
                    UpdateProperty(ref _DataLength, value);
                    FastChannelData = Math.Clamp(_FastChannelData, MinFastChannelData, MaxFastChannelData);
                }
            }
        }
        public Int64 MaxFastChannelData => (Int64)(Math.Pow(2, ((Int32)DataLength + 1) * 4) - 1);
        public Int64 MinFastChannelData => 0X00;

        private Int64 _FastChannelData;

        public Int64 FastChannelData
        {
            get { return _FastChannelData; }
            set { UpdateProperty(ref _FastChannelData, value); }
        }

        public Int32 MaxFastChannelStatus => 0X0F;
        public Int32 MinFastChannelStatus => 0X00;

        private Int32 _FastChannelStatus;

        public Int32 FastChannelStatus
        {
            get { return _FastChannelStatus; }
            set { UpdateProperty(ref _FastChannelStatus, value); }
        }

        public Int32 MaxFastChannelCRC => 0X0F;
        public Int32 MinFastChannelCRC => 0X00;

        private Int32 _FastChannelCRC;

        public Int32 FastChannelCRC
        {
            get { return _FastChannelCRC; }
            set { UpdateProperty(ref _FastChannelCRC, value); }
        }

        private ProtocolSENT.FastError _FastError;

        public ProtocolSENT.FastError FastError
        {
            get { return _FastError; }
            set { UpdateProperty(ref _FastError, value); }
        }

        #endregion

        #region 慢速通道触发参数
        private ProtocolSENT.SlowCondition _SlowCondition;

        public ProtocolSENT.SlowCondition SlowCondition
        {
            get { return _SlowCondition; }
            set
            {
                if (_SlowCondition != value)
                {
                    UpdateProperty(ref _SlowCondition, value);
                    SlowChannelID = Math.Clamp(_SlowChannelID, MinSlowChannelID, MaxSlowChannelID);
                    SlowChannelData = Math.Clamp(_SlowChannelData, MinSlowChannelData, MaxSlowChannelData);
                    SlowChannelCRC = Math.Clamp(_SlowChannelCRC, MinSlowChannelCRC, MaxSlowChannelCRC);
                }
            }
        }

        private ProtocolSENT.SlowMessageCondition _SlowMessageCondition;

        public ProtocolSENT.SlowMessageCondition SlowMessageCondition
        {
            get { return _SlowMessageCondition; }
            set
            {
                if (_SlowMessageCondition != value)
                {
                    UpdateProperty(ref _SlowMessageCondition, value);
                    SlowChannelData = Math.Clamp(_SlowChannelData, MinSlowChannelData, MaxSlowChannelData);
                }
            }
        }


        private ProtocolSENT.SlowEnhancedMessageType _SlowEnhancedMessageType;

        public ProtocolSENT.SlowEnhancedMessageType SlowEnhancedMessageType
        {
            get { return _SlowEnhancedMessageType; }
            set 
            {
                if (_SlowEnhancedMessageType != value)
                {
                    UpdateProperty(ref _SlowEnhancedMessageType, value);
                    SlowChannelID = Math.Clamp(_SlowChannelID, MinSlowChannelID, MaxSlowChannelID);
                    SlowChannelData = Math.Clamp(_SlowChannelData, MinSlowChannelData, MaxSlowChannelData);
                    SlowChannelCRC = Math.Clamp(_SlowChannelCRC, MinSlowChannelCRC, MaxSlowChannelCRC);
                }
            }
        }
        public Int32 MaxSlowChannelID => _SlowMessageCondition == ProtocolSENT.SlowMessageCondition.EnhancedMessage ? 0XFF : 0X0F;
        public Int32 MinSlowChannelID => 0X00;

        private Int32 _SlowChannelID;

        public Int32 SlowChannelID
        {
            get { return _SlowChannelID; }
            set { UpdateProperty(ref _SlowChannelID, value); }
        }


        public Int32 MaxSlowChannelData => _SlowMessageCondition == ProtocolSENT.SlowMessageCondition.EnhancedMessage ? (SlowEnhancedMessageType == ProtocolSENT.SlowEnhancedMessageType.Enhanced8BitID ? 0XF_FF : 0XFF_FF) : 0XFF;
        public Int32 MinSlowChannelData => 0X00;

        private Int32 _SlowChannelData;

        public Int32 SlowChannelData
        {
            get { return _SlowChannelData; }
            set { UpdateProperty(ref _SlowChannelData, value); }
        }

        public Int32 MaxSlowChannelCRC => _SlowMessageCondition == ProtocolSENT.SlowMessageCondition.EnhancedMessage ? 0X3F : 0X0F;
        public Int32 MinSlowChannelCRC => 0X00;

        private Int32 _SlowChannelCRC;

        public Int32 SlowChannelCRC
        {
            get { return _SlowChannelCRC; }
            set { UpdateProperty(ref _SlowChannelCRC, value); }
        }

        #endregion

        private ProtocolSENT.DataRelation _DataRelation;

        public ProtocolSENT.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }



        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigSENTConditionsOptions(ChannelMode, FastCondition, SlowCondition)
            {
                channelMode = ChannelMode,
                fastCondition = FastCondition,
                slowCondition = SlowCondition,
                FastChannelData = FastChannelData,
                FastChannelStatus = FastChannelStatus,
                FastChannelCRC = FastChannelCRC,
                FastError = FastError,
                SlowMessageCondition = SlowMessageCondition,
                SlowEnhancedMessageType = SlowEnhancedMessageType,
                SlowChannelData = SlowChannelData,
                SlowChannelID = SlowChannelID,
                SlowChannelCRC = SlowChannelCRC,
                DataRelation = DataRelation,
                DataLength = DataLength
            };
        }
    }
}
