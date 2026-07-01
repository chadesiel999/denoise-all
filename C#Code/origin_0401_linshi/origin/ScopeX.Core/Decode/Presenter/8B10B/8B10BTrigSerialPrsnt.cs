using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class D8B10BTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override D8B10BTrigSerialModel Model
        {
            get;
        }

        public override String ConditionName => nameof(Condition);
        public D8B10BTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.Common_8b10b, view)
        {
            Model = (D8B10BTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.Common_8b10b);
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
        public Protocol8B10B.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }

    }
}
