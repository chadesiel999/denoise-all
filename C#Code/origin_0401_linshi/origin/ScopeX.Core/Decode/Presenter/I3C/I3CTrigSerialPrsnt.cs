using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class I3CTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override I3CTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public I3CTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.I3C, view)
        {
            Model = (I3CTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.I3C);
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
        public ProtocolI3C.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }


    }
}
