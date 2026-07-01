using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class SPMITrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override SPMITrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public SPMITrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SPMI, view)
        {
            Model = (SPMITrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.SPMI);
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
