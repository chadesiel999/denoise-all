using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolSENT;

namespace ScopeX.Core.Decode
{
    public class SENTTrigSerialPrsnt : TrigSerialPrsnt
    {
        public SENTTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SENT, view)
        {
            Model = (SENTTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.SENT);
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
        public override String ConditionName => ChannelMode== ProtocolSENT.ChannelMode.FastChannel? nameof(FastCondition):nameof(SlowCondition);

        public ProtocolSENT.ChannelMode ChannelMode
        {
            get => Model.ChannelMode;
            set => Model.ChannelMode = value;
        }

        #region 快速通道参数
        public ProtocolSENT.FastCondition FastCondition
        {
            get => Model.FastCondition;
            set => Model.FastCondition = value;
        }
        public ProtocolSENT.FastChannelMode FastChannelMode
        {
            get => Model.FastChannelMode;
            set => Model.FastChannelMode = value;
        }
        public ProtocolSENT.DataLength DataLength
        {
            get => Model.DataLength;
            set => Model.DataLength = value;
        }
        public Int64 MaxFastChannelData => Model.MaxFastChannelData;
        public Int64 MinFastChannelData => Model.MinFastChannelData;
        public Int64 FastChannelData
        {
            get => Model.FastChannelData;
            set => Model.FastChannelData = Math.Clamp(value, MinFastChannelData, MaxFastChannelData);
        }

        public Int32 MaxFastChannelState => Model.MaxFastChannelStatus;
        public Int32 MinFastChannelState => Model.MinFastChannelStatus;

        public Int32 FastChannelStatus
        {
            get => Model.FastChannelStatus;
            set => Model.FastChannelStatus = Math.Clamp(value, MinFastChannelState, MaxFastChannelState);
        }

        public Int32 MaxFastChannelCRC => Model.MaxFastChannelCRC;
        public Int32 MinFastChannelCRC => Model.MinFastChannelCRC;

        public Int32 FastChannelCRC
        {
            get => Model.FastChannelCRC;
            set => Model.FastChannelCRC = Math.Clamp(value, MinFastChannelCRC, MaxFastChannelCRC);
        }

        public ProtocolSENT.FastError FastError
        {
            get => Model.FastError;
            set => Model.FastError = value;
        }
        #endregion

        #region 慢速通道触发参数

        public ProtocolSENT.SlowCondition SlowCondition
        {
            get => Model.SlowCondition;
            set => Model.SlowCondition = value;
        }

        public ProtocolSENT.SlowMessageCondition SlowMessageCondition
        {
            get => Model.SlowMessageCondition;
            set => Model.SlowMessageCondition = value;
        }

        public ProtocolSENT.SlowEnhancedMessageType SlowEnhancedMessageType
        {
            get => Model.SlowEnhancedMessageType;
            set => Model.SlowEnhancedMessageType = value;
        }

        public Int32 MaxSlowChannelID => Model.MaxSlowChannelID;
        public Int32 MinSlowChannelID => Model.MinSlowChannelID;

        public Int32 SlowChannelID
        {
            get => Model.SlowChannelID;
            set => Model.SlowChannelID = Math.Clamp(value, MinSlowChannelID, MaxSlowChannelID);
        }


        public Int32 MaxSlowChannelData => Model.MaxSlowChannelData;
        public Int32 MinSlowChannelData => Model.MinSlowChannelData;

        public Int32 SlowChannelData
        {
            get => Model.SlowChannelData;
            set => Model.SlowChannelData = Math.Clamp(value, MinSlowChannelData, MaxSlowChannelData);
        }

        public Int32 MaxSlowChannelCRC => Model.MaxSlowChannelCRC;
        public Int32 MinSlowChannelCRC => Model.MinSlowChannelCRC;

        public Int32 SlowChannelCRC
        {
            get => Model.SlowChannelCRC;
            set => Model.SlowChannelCRC = Math.Clamp(value, MinSlowChannelCRC, MaxSlowChannelCRC);
        }

        #endregion

        public ProtocolSENT.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set => Model.DataRelation = value;
        }


        private protected override SENTTrigSerialModel Model
        {
            get;
        }
    }
}
