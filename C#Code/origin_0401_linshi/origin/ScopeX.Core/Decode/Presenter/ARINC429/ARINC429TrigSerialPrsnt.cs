using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class ARINC429TrigSerialPrsnt : TrigSerialPrsnt
    {
        public ARINC429TrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.ARINC429, view)
        {
            Model = (ARINC429TrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.ARINC429);
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
        public ProtocolARINC429.Condition Condition
        {
            get => Model.Condition;
            set => Model.Condition = value.Clamp();
        }
        public override String ConditionName => nameof(Condition);
        public UInt32 Data
        {
            get => Model.Data;
            set
            {
                Model.Data = Math.Clamp(value, MinData, MaxData);
            }
        }

        public ProtocolARINC429.DecodeMode DecodeMode
        {
            get => Model.DecodeMode;
            internal set => Model.DecodeMode = value.Clamp();
        }

        public ProtocolARINC429.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set
            {
                Model.DataRelation = value.Clamp();
            }
        }


        public UInt32 Label
        {
            get => Model.Label;
            set
            {
                Model.Label = Math.Clamp(value, MinLabel, MaxLabel);
            }
        }

        public UInt32 SDI
        {
            get => Model.SDI;
            set
            {
                Model.SDI = Math.Clamp(value, MinSDI, MaxSDI);
            }
        }

        public UInt32 SSM
        {
            get => Model.SSM;
            set
            {
                Model.SSM = Math.Clamp(value, MinSSM, MaxSSM);
            }
        }
        public ProtocolARINC429.ErrorType ErrorType
        {
            get => Model.ErrorType;
            set
            {
                Model.ErrorType = value.Clamp();
            }
        }

        public UInt32 MinData => Model.MinData;
        public UInt32 MaxData => Model.MaxData;
        public UInt32 MinSSM => Model.MinSSM;
        public UInt32 MaxSSM => Model.MaxSSM;
        public UInt32 MinSDI => Model.MinSDI;
        public UInt32 MaxSDI => Model.MaxSDI;
        public UInt32 MinLabel => Model.MinLabel;
        public UInt32 MaxLabel => Model.MaxLabel;


        private protected override ARINC429TrigSerialModel Model
        {
            get;
        }
    }
}
