using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class RS232TrigSerialPrsnt : TrigSerialPrsnt
    {
        public RS232TrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.RS232, view)
        {
            Model = (RS232TrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.RS232);
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
        public override String ConditionName => nameof(Conditions);


        private protected override RS232TrigSerialModel Model
        {
            get;
        }


        public PulseCondition Compare
        {
            get => Model.Compare;
            set
            {
                Model.Compare = value.Clamp();
            }
        }

        public ProtocolRS232.Conditions Conditions
        {
            get => Model.Conditions;
            set
            {
                Model.Conditions = value.Clamp();
            }
        }
        public UInt32 MinData => Model.MinData;
        public UInt32 MaxData => Model.MaxData;
        public UInt32 Data
        {
            get => Model.Data;
            set
            {
                Model.Data = value;
            }
        }
        public UInt32 MinDataLength => Model.MinDataLength;
        public UInt32 MaxDataLength => Model.MaxDataLength;
        private UInt32 _DataLength = 1;
        /// <summary>
        /// 数据
        /// </summary>
        public UInt32 DataLength
        {
            get => Model.DataLength;
            set 
            {
               Model.DataLength = value;
            }
        }


        //public RS232TrigDataIndex DataIndex 
        //{ 
        //    get => Model.DataIndex; 
        //    set => Model.DataIndex = value;
        //}

        public Char MaxEOP => Model.MaxEOP;
        public Char MinEOP => Model.MinEOP;
        public Char EOPChar
        {
            get => Model.EOPChar;
            set
            {
                Model.EOPChar = (Char)Math.Clamp(value, MinEOP, MaxEOP);
            }
        }
    }
}
