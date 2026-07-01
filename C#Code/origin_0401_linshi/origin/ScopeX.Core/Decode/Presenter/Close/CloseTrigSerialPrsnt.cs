using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class CloseTrigSerialPrsnt : TrigSerialPrsnt
    {
        public CloseTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view = null) : base(idp, SerialProtocolType.Close, view)
        {
            Model = (CloseTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.Close);
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

        private protected override CloseTrigSerialModel Model
        {
            get;
        }

    }
}
