using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class FlexRayTrigSerialModel : TriggerSerialModel
    {
        private ProtocolFlexRay.Condition _Condition = ProtocolFlexRay.Condition.Data;
        /// <summary>
        /// 事件类型
        /// </summary>
        //触发条件
        public ProtocolFlexRay.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolFlexRay.Realtion _Relation = ProtocolFlexRay.Realtion.Lteq;

        public ProtocolFlexRay.Realtion Relation
        {
            get { return _Relation; }
            set { UpdateProperty(ref _Relation, value); }
        }

        public static Byte MaxByteCount = 15;
        public static Byte MinByteCount = 0;

        private Byte _ByteCount = 0;

        public Byte ByteCount
        {
            get { return _ByteCount; }
            set { UpdateProperty(ref _ByteCount, value); }
        }

        public Byte[] MaxData
        {
            get
            {
                var temp = new Byte[ByteCount];
                for (Int32 i = 0; i < ByteCount + 1; i++)
                {
                    temp[0] = 0XFF;
                }
                return temp;
            }
        }

        private UInt64 _DataL;

        public UInt64 DataL
        {
            get { return _DataL; }
            set { UpdateProperty(ref _DataL, value); }
        }
        public UInt64 MaxDataL => ByteCount >= 7 ? UInt64.MaxValue : (UInt64)(Math.Pow(2, (ByteCount + 1) * 8) - 1);
        public UInt64 MinDataL => ByteCount >= 7 ? UInt64.MinValue : 0;

        private UInt64 _DataH;
        public UInt64 DataH
        {
            get => _DataH;
            set => UpdateProperty(ref _DataH, value);
        }
        public UInt64 MinDataH => ByteCount != 15 ? 0 : UInt64.MinValue;
        public UInt64 MaxDataH
        {
            get
            {
                if (ByteCount <= 7) return 0;
                if (ByteCount == 15) return Int64.MaxValue;
                return (UInt64)(Math.Pow(2, (ByteCount - 7) * 8) - 1);
            }
        }


        public Boolean _HasDataOffset = false;
        public Boolean HasDataOffset
        {
            get { return _HasDataOffset; }
            set { UpdateProperty(ref _HasDataOffset, value); }
        }
        public Byte MaxByteOffset => 0XFD;
        public Byte MinByteOffset => 0;
        //字节偏置（触发条件：数据）
        private Byte _ByteOffset = 0;
        public Byte ByteOffset
        {
            get { return _ByteOffset; }
            set { UpdateProperty(ref _ByteOffset, value); }
        }

        #region 标头字段

        #region 循环数
        public Byte MaxCycleData => 0X3F;
        public Byte MinCycleData => 0X00;

        private Byte _CycleData;
        public Byte CycleData
        {
            get { return _CycleData; }
            set { UpdateProperty(ref _CycleData, value); }
        }
        #endregion

        #region 指示位
        public UInt16 MaxIndicatorData => 0X1F;
        public UInt16 MinIndicatorData => 0X00;

        private UInt16 _IndicatorData = 0X00;
        public UInt16 IndicatorData
        {
            get { return _IndicatorData; }
            set { UpdateProperty(ref _IndicatorData, value); }
        }
        #endregion

        #region ID
        public UInt16 MaxID => 0X7_FF;
        public UInt16 MinID => 0X0;

        private UInt16 _ID;

        public UInt16 ID
        {
            get { return _ID; }
            set { UpdateProperty(ref _ID, value); }
        }
        #endregion

        #region 载荷长度
        public UInt16 MaxPayload => 0X1F;
        public UInt16 MinPayload => 0X00;

        private UInt16 _Payload = 0X00;
        public UInt16 Payload
        {
            get { return _Payload; }
            set { UpdateProperty(ref _Payload, value); }
        }
        #endregion

        #region 标头CRC
        public UInt16 MaxHeaderCRC => 0X7_FF;
        public UInt16 MinHeaderCRC => 0X00;

        private UInt16 _HeaderCRC = 0X00;
        public UInt16 HeaderCRC
        {
            get { return _HeaderCRC; }
            set { UpdateProperty(ref _HeaderCRC, value); }
        }
        #endregion

        #endregion

        private ProtocolFlexRay.Indicator _Indicator = ProtocolFlexRay.Indicator.SyncFrame;
        public ProtocolFlexRay.Indicator Indicator
        {
            get { return _Indicator; }
            set { UpdateProperty(ref _Indicator, value); }
        }

        private ProtocolFlexRay.FrameError _FrameError = ProtocolFlexRay.FrameError.StartFrame;

        public ProtocolFlexRay.FrameError FrameError
        {
            get { return _FrameError; }
            set { UpdateProperty(ref _FrameError, value); }
        }

        private ProtocolFlexRay.FrameTail _FrameTail = ProtocolFlexRay.FrameTail.Static;

        public ProtocolFlexRay.FrameTail FrameTail
        {
            get { return _FrameTail; }
            set { UpdateProperty(ref _FrameTail, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            List<Byte> tempbytes = new List<Byte>();
            tempbytes.AddRange(BitConverter.GetBytes(DataL));
            tempbytes.AddRange(BitConverter.GetBytes(DataH));
            return new HdMessage.TrigFlexRayConditionOptions(Condition)
            {
                Data = tempbytes.Take(ByteCount + 1).ToArray(),
                HasDataOffset = HasDataOffset,
                ByteOffset = ByteOffset,
                ByteCount = ByteCount,
                CycleData = CycleData,
                IndicatorData = IndicatorData,
                ID = ID,
                Payload = Payload,
                HeaderCRC = HeaderCRC,
                Indicator = Indicator,
                Relation = Relation,
                FrameError = FrameError,
                FrameTail = FrameTail
            };
        }
    }
}
