using ScopeX.ComModel;
using System;

namespace ScopeX.Core.Decode
{
    internal sealed class AudioBusTrigSerialModel : TriggerSerialModel
    {

        private ProtocolAudioBus.Condition _Condition = ProtocolAudioBus.Condition.Data;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolAudioBus.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolAudioBus.Condition4TDM _ConditionTDM;

        public ProtocolAudioBus.Condition4TDM ConditionTDM
        {
            get { return _ConditionTDM; }
            set { UpdateProperty(ref _ConditionTDM, value); }
        }

        private ProtocolAudioBus.DataRelation _DataRelation;
        public ProtocolAudioBus.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }
        public UInt32 MinData => 0;
        public UInt32 MaxData => UInt32.MaxValue;
        private UInt32 _Data;

        public UInt32 Data
        {
            get { return _Data; }
            set 
            {
                _ByteCount = (Byte)Convert.ToString(value, 16).Length;
                UpdateProperty(ref _Data, value);

            }
        }

        public UInt32 MinTDMChannelID => 1;
        public UInt32 MaxTDMChannelID => 64;

        private UInt32 _TDMChannelID;
        /// <summary>
        /// TDM时选择的音频通道
        /// </summary>
        public UInt32 TDMChannelID
        {
            get { return _TDMChannelID; }
            set { UpdateProperty(ref _TDMChannelID, value); }
        }

        public Byte _ByteCount = 1;

        public Byte ByteCount
        {
            get { return _ByteCount; }
            set { UpdateProperty(ref _ByteCount, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigI2SConditionsOptions(Condition)
            {
                SoundChannel = SoundChannel,
                ByteCount = ByteCount,
                Data = Data,
                DataRelation = DataRelation,
                ConditionTDM = ConditionTDM,
                TDMChannelID = TDMChannelID,
            };
        }

        private ProtocolAudioBus.SoundChannel _SoundChannel;
        public ProtocolAudioBus.SoundChannel SoundChannel
        {
            get { return _SoundChannel; }
            set { UpdateProperty(ref _SoundChannel, value); }
        }
        
    }
}
