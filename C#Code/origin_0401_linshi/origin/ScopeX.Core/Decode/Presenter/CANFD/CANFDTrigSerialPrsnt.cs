using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class CANFDTrigSerialPrsnt : TrigSerialPrsnt
    {
        public CANFDTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.CAN_FD, view)
        {
            Model = (CANFDTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.CAN_FD);
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
        public override String ConditionName => nameof(Condition);

        public Int32 MinByteCount => Model.MinByteCount;


        public Int32 MaxByteCount => Model.MaxByteCount;
        //字节数(触发条件选择"数据"或"ID和数据"时使用)
        public Int32 ByteCount
        {
            get => Model.ByteCount;
            set
            {
                Model.ByteCount = Math.Clamp(value, MinByteCount, MaxByteCount);
            }
        }
        public Int32 MaxByteIndex => Model.MaxByteIndex;
        public Int32 MinByteIndex => Model.MinByteIndex;

        //字节号(触发条件选择"数据"或"ID和数据"时使用)
        public Int32 ByteIndex
        {
            get => Model.ByteIndex;
            set
            {
                Model.ByteIndex = Math.Clamp(value, MinByteIndex, MaxByteIndex);
            }
        }

        public ProtocolCANFD.Condition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value.Clamp();
            }
        }

        public UInt64 MaxData => Model.MaxData;


        public UInt64 MinData => Model.MinData;
        //数据(触发条件选择"数据"或"ID和数据"时使用)
        public UInt64 Data
        {
            get => Model.Data;
            set
            {
                Model.Data = (UInt64)Math.Clamp((Int64)value, (Int64)MinData, (Int64)MaxData);
            }
        }

        //数据限定(触发条件选择"数据"或"ID和数据"时使用)
        public ProtocolCANFD.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set
            {
                Model.DataRelation = value.Clamp();
            }
        }

        public Int32 MaxExtendedID => Model.MaxExtendedID;


        public Int32 MinExtendedID => Model.MinExtendedID;
        //扩展ID号(触发条件选择"ID"或"ID和数据"时使用)
        public Int32 ExtendedID
        {
            get => Model.ExtendedID;
            set
            {
                Model.ExtendedID = Math.Clamp(value, MinExtendedID, MaxExtendedID);
            }
        }

        /// <summary>
        /// 数据触发的偏移使能
        /// </summary>
        public Boolean DataOffsetEnabled
        {
            get => Model.DataOffsetEnabled;
            set => Model.DataOffsetEnabled = value;
        }

        /// <summary>
        /// 数据触发的偏移量
        /// </summary>
        public Int32 DataOffset
        {
            get => Model.DataOffset;
            set => Model.DataOffset = value;
        }

        /// <summary>
        /// 数据触发的偏移量  -- 最大值
        /// </summary>
        public Int32 MinDataOffset { get => Model.MinDataOffset; }

        /// <summary>
        /// 数据触发的偏移量  -- 最小值
        /// </summary>
        public Int32 MaxDataOffset { get => Model.MaxDataOffset; }


        //帧类型(触发条件选择"帧类型"时使用)
        public ProtocolCANFD.FrameType FrameType
        {
            get => Model.FrameType;
            set
            {
                Model.FrameType = value.Clamp();
            }
        }
        //public ProtocolCANFD.ErrorPacketType ErrorPacketType
        //{
        //    get => Model.ErrorPacketType;
        //    set => Model.ErrorPacketType = value.Clamp();
        //}
        public ProtocolCANFD.ErrorType ErrorType
        {
            get => Model.ErrorType;
            set => Model.ErrorType = value.Clamp();
        }

        //ID帧类型(触发条件选择"ID"或"ID和数据"时使用)
        public ProtocolCANFD.IDFrameDirection IDFrameDirection
        {
            get => Model.IDFrameDirection;
            set
            {
                Model.IDFrameDirection = value.Clamp();
            }
        }

        //ID标准(触发条件选择"ID"或"ID和数据"时使用)
        public ProtocolCANFD.IDStandard IDStandard
        {
            get => Model.IDStandard;
            set
            {
                Model.IDStandard = value.Clamp();
            }
        }

        public Int32 MinStandardID => Model.MinStandardID;


        public Int32 MaxStandardID => Model.MaxStandardID;
        //标准ID号(触发条件选择"ID"或"ID和数据"时使用)
        public Int32 StandardID
        {
            get => Model.StandardID;
            set
            {
                Model.StandardID = Math.Clamp(value, MinStandardID, MaxStandardID);
            }
        }

        public ProtocolCANFD.Condition TrigCondition
        {
            get => Model.TrigCondition;
            set
            {
                Model.TrigCondition = value.Clamp();
            }
        }


        private protected override CANFDTrigSerialModel Model
        {
            get;
        }
    }
}
