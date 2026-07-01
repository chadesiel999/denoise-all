using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class NRZTrigSerialPrsnt : TrigSerialPrsnt
    {
        public override String ConditionName => nameof(Condition);
        public NRZTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.NRZ, view)
        {
            Model = (NRZTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.NRZ);
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
        public ProtocolNRZ.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }
        public Byte MinData => Model.MinData;
        public Byte MaxData => Model.MaxData;
        public Byte Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
        public ProtocolNRZ.DataRelation Relation
        {
            get => Model.Relation;
            set => Model.Relation = value.Clamp();
        }


        private protected override NRZTrigSerialModel Model
        {
            get;
        }
    }
}
