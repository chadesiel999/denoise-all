using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class Mlt3TrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override Mlt3TrigSerialModel Model
        {
            get;
        }

        public override String ConditionName => nameof(Condition);
        public Mlt3TrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.Mlt3, view)
        {
            Model = (Mlt3TrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.Mlt3);
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
        public ProtocolMlt3.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }

    }
}
