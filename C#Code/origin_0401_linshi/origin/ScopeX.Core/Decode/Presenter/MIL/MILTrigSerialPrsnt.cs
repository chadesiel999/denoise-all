using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class MILTrigSerialPrsnt : TrigSerialPrsnt
    {
        public MILTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.MIL, view)
        {
            Model = (MILTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.MIL);
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

        private protected override MILTrigSerialModel Model
        {
            get;
        }

        public ProtocolMIL.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }
        public override String ConditionName => nameof(Condition);

        public Int64 MaxRTA => Model.MaxRTA;
        public Int64 MinRTA => Model.MinRTA;
        public Int32 RTA
        {
            get => Model.RTA;
            set => Model.RTA = value;
        }

        public Int64 MaxData => Model.MaxData;
        public Int64 MinData => Model.MinData;
        public Int32 Data
        {
            get => Model.Data;
            set => Model.Data = value;
        }

        public Int64 MaxPairty => Model.MaxPairty;
        public Int64 MinPairty => Model.MinPairty;
        public Int32 Pairty
        {
            get => Model.Pairty;
            set => Model.Pairty = value;
        }

        public ProtocolMIL.ErrorType ErrorType
        {
            get => Model.ErrorType;
            set
            {
                Model.ErrorType = value.Clamp();
            }
        }

    }
}
