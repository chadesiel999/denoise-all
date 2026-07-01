using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class FlexRayTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override FlexRayTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName { get; } = nameof(Condition);
        public FlexRayTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.FlexRay, view)
        {
            Model = (FlexRayTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.FlexRay);
            LoadEvent();
        }
        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= OnPropertyChanged;
            }
        }
        // private ProtocolFlexRay.Realtion _Relation = ProtocolFlexRay.Realtion.Lteq;

        public ProtocolFlexRay.Realtion Relation
        {
            get => Model.Relation;
            set
            {
                Model.Relation = value.Clamp();
            }
        }
        public ProtocolFlexRay.FrameTail FrameTail
        {
            get => Model.FrameTail;
            set => Model.FrameTail = value.Clamp();
        }
        public ProtocolFlexRay.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }


        public UInt16 ID
        {
            get => Model.ID;
            set => Model.ID = Math.Clamp(value, MinID, MaxID);
        }
        public UInt16 MaxID => Model.MaxID;
        public UInt16 MinID => Model.MinID;

        public UInt64 MinDataL => Model.MinDataL;
        public UInt64 MaxDataL => Model.MaxDataL;
        //  private Byte _Data;


        public UInt64 DataL
        {
            get => Model.DataL;
            set => Model.DataL = Math.Clamp(value, MinDataL, MaxDataL);
        }
        public UInt64 MinDataH => Model.MinDataH;
        public UInt64 MaxDataH => Model.MaxDataH;

        public UInt64 DataH
        {
            get => Model.DataH;
            set => Model.DataH = Math.Clamp(value, MinDataH, MaxDataH);
        }

        public ProtocolFlexRay.FrameError FrameError
        {
            get => Model.FrameError;
            set => Model.FrameError = value.Clamp();
        }
        public Byte MinCycleData => Model.MinCycleData;
        public Byte MaxCycleData => Model.MaxCycleData;
        public Byte CycleData
        {
            get => Model.CycleData;
            set
            {
                Model.CycleData = Math.Clamp(value, MinCycleData, MaxCycleData);
            }
        }

        public UInt16 MinIndicatorData => Model.MinIndicatorData;
        public UInt16 MaxIndicatorData => Model.MaxIndicatorData;
        public UInt16 IndicatorData
        {
            get => Model.IndicatorData;
            set
            {
                Model.IndicatorData = Math.Clamp(value, MinIndicatorData, MaxIndicatorData);
            }
        }

        public UInt16 MinPayload => Model.MinPayload;
        public UInt16 MaxPayload => Model.MaxPayload;
        public UInt16 Payload
        {
            get => Model.Payload;
            set
            {
                Model.Payload = Math.Clamp(value, MinIndicatorData, MaxIndicatorData);
            }
        }

        public UInt16 MinHeaderCRC => Model.MinHeaderCRC;
        public UInt16 MaxHeaderCRC => Model.MaxHeaderCRC;
        public UInt16 HeaderCRC
        {
            get => Model.HeaderCRC;
            set
            {
                Model.HeaderCRC = Math.Clamp(value, MinHeaderCRC, MaxHeaderCRC);
            }
        }

        public Byte MinByteCount => FlexRayTrigSerialModel.MinByteCount;
        public Byte MaxByteCount => FlexRayTrigSerialModel.MaxByteCount;
        public Byte ByteCount
        {
            get => Model.ByteCount;
            set => Model.ByteCount = Math.Clamp(value, MinByteCount, MaxByteCount);
        }
        public ProtocolFlexRay.Indicator Indicator
        {
            get => Model.Indicator;
            set
            {
                Model.Indicator = value.Clamp();
            }
        }

        public Boolean _HasDataOffset = false;
        public Boolean HasDataOffset
        {
            get => Model.HasDataOffset;
            set
            {
                if (Model.HasDataOffset != value)
                {
                    Model.HasDataOffset = value;
                }
            }
        }
        public Byte MaxByteOffset => 0XFD;
        public Byte MinByteOffset => 0;
        //字节偏置（触发条件：数据）
        public Byte ByteOffset
        {
            get => Model.ByteOffset;
            set
            {
                if (Model.ByteOffset != value)
                {
                    Model.ByteOffset = Math.Clamp(value, MinByteOffset, MaxByteOffset);
                }
            }
        }
    }
}
