using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class SMBusTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override SMBusTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public SMBusTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SMBus, view)
        {
            Model = (SMBusTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.SMBus);
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
        public ProtocolSPMI.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }


    }
}
