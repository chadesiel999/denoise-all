using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class SATATrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override SATATrigSerialModel Model
        {
            get;
        }


        public SATATrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SATA, view)
        {
            Model = (SATATrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.SATA);
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
        public Int64 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
        public ProtocolSATA.DataRelation Relation
        {
            get => Model.Relation;
            set => Model.Relation = value.Clamp();
        }
        public ProtocolSATA.FISTypeFlag FISType
        {
            get => Model.FISType;
            set => Model.FISType = value.Clamp();
        }
        public Byte MaxDataCount => Model.MaxDataCount;
        public Byte MinDataCount => Model.MinDataCount;
        public Byte DataCount
        {
            get => Model.DataCount;
            set => Model.DataCount = Math.Clamp(value, MinDataCount, MaxDataCount);
        }
        public Int64 MaxData => Model.MaxData;
        public Int64 MinData => Model.MinData;
        public override String ConditionName => nameof(Condition);
        public ProtocolSATA.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }
    }
}
