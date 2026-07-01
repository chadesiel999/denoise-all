using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class PSI5TrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override PSI5TrigSerialModel Model
        {
            get;
        }

        public PSI5TrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SPMI, view)
        {
            Model = (PSI5TrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.PSI5);
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
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolPSI5.Condition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value.Clamp();
            }
        }
        /// <summary>
        /// 数据域A
        /// </summary>
        public Int64 MaxDataAValue => Model.MaxDataAValue;//0xFFFFFFF;
        public Int64 MinDataAValue => Model.MinDataAValue;//0x0;

        public Int64 DataAValue
        {
            get => Model.DataAValue;
            set => Model.DataAValue = Math.Clamp(value, MinDataAValue, MaxDataAValue);
        }
        /// <summary>
        /// 数据域B
        /// </summary>
        public Int64 MaxDataBValue => Model.MaxDataBValue;// 0x3FF;
        public Int64 MinDataBValue => Model.MinDataBValue;//0x0;
        public Int64 DataBValue
        {
            get => Model.DataBValue;
            set => Model.DataBValue = Math.Clamp(value, MinDataBValue, MaxDataBValue);
        }

        /// <summary>
        /// 数据域B
        /// </summary>
        public Int64 MaxBlockID => Model.MaxBlockID;//0x20F;
        public Int64 MinBlockID => Model.MinBlockID;//0x200;
        public Int64 BlockID
        {
            get => Model.BlockID;
            set => Model.BlockID = Math.Clamp(value, MinBlockID, MaxBlockID);
        }


        /// <summary>
        /// 数据域B
        /// </summary>
        public Int64 MaxSensorStatus => Model.MaxSensorStatus;//0x1F4;
        public Int64 MinSensorStatus => Model.MinSensorStatus;//0x1E1;
        public Int64 SensorStatus
        {
            get => Model.SensorStatus;
            set => Model.SensorStatus = Math.Clamp(value, MinSensorStatus, MaxSensorStatus);
        }

    }
    
}
