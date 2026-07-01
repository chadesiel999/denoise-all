using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class LINTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override LINTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public LINTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.LIN, view)
        {
            Model = (LINTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.LIN);
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
        public ProtocolLIN.Condition Condition
        {
            get => Model.Condition;
            set => Model.Condition = value.Clamp();
        }

        public ProtocolLIN.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set => Model.DataRelation = value.Clamp();
        }


        public Byte ID
        {
            get => Model.ID;
            set => Model.ID = Math.Clamp(value, MinID, MaxID);
        }
        public Byte MaxID => Model.MaxID;
        public Byte MinID => Model.MinID;


        public Int64 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
        public Int64 MaxData => Model.MaxData;
        public Int64 MinData => Model.MinData;

        public Byte ByteCount
        {
            get => Model.ByteCount;
            set => Model.ByteCount = value;
        }
    }
}
