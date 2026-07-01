using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class CPHYTrigSerialPrsnt : TrigSerialPrsnt
    {
        public CPHYTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.CPHY, view)
        {
            Model = (CPHYTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.CPHY);
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

        private protected override CPHYTrigSerialModel Model
        {
            get;
        }


        //public override String ConditionName => nameof(Condition);
        public Int64 MaxMACAddress => Model.MaxMACAddress;
        public Int64 MinMACAddress => Model.MinMACAddress;
        public Int32 MACByteCount => Model.MACByteCount;
        //public ProtocolCPHY.Condition Condition
        //{
        //    get => Model.Condition;
        //    set => Model.Condition = value.Clamp();
        //}
        public Byte MinData => Model.MinData;
        public Byte MaxData => Model.MaxData;
        public Byte Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
        public Byte[] SrcMAC
        {
            get => Model.SrcMAC;
            set => Model.SrcMAC = value.Take(MACByteCount).ToArray();
        }
        public Byte[] DestMAC
        {
            get => Model.DestMAC;
            set => Model.DestMAC = value.Take(MACByteCount).ToArray();
        }
        //public ProtocolCPHY.DataRelation Relation
        //{
        //    get => Model.Relation;
        //    set => Model.Relation = value.Clamp();
        //}
    }
}
