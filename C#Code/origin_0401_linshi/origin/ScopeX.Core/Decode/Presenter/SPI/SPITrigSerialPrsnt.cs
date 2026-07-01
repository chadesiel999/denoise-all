using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class SPITrigSerialPrsnt : TrigSerialPrsnt
    {
        public SPITrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.SPI, view)
        {
            Model = (SPITrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.SPI);
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
        public override String ConditionName => nameof(Condition);

        public ComModel.ProtocolSPI.Condition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value.Clamp();
            }
        }

        public ComModel.ProtocolSPI.FramingMode SpiTrigMode
        {
            get => Model.SpiTrigMode;
            set
            {
                Model.SpiTrigMode = value.Clamp();
            }
        }

        public ComModel.ProtocolSPI.DataTriggerSource DataSource
        {
            get => Model.DataSource;
            set
            {
                Model.DataSource = ProtocolSPI.DataTriggerSource.MOSI;
            }
        }
        public Int32 MaxFrameCount => Model.MaxFrameCount;
        public Int32 MinFrameCount => Model.MinFrameCount;

        public Int32 FrameCount
        {
            get => Model.FrameCount;
            set => Model.FrameCount = Math.Clamp(value, MinFrameCount, MaxFrameCount);
        }
        //public UInt64 MaxFrameData => Model.MaxFrameData;
        //public UInt64 MinFrameData => Model.MinFrameData;
        //public UInt64 FrameData
        //{
        //    get => Model.FrameData;
        //    set => Model.FrameData = Math.Clamp(value, MinFrameData, MaxFrameData);
        //}
        public Int64 MaxFrameDataHigh => Model.MaxFrameDataHigh;
        public Int64 MaxFrameData => Model.MaxFrameData;
        public Int64 MinFrameData => Model.MinFrameData;
        public Int64 FrameData
        {
            get => Model.FrameData;
            set => Model.FrameData = Math.Clamp(value, MinFrameData, MaxFrameData);
        }
        public Int64 FrameDataHigh
        {
            get => Model.FrameDataHigh;
            set => Model.FrameDataHigh = Math.Clamp(value, MinFrameData, MaxFrameData);
        }
        public Boolean LongDataEnable
        {
            get => Model.LongDataEnable;
        }
        public Int32 MaxDataBitWidth => Model.MaxDataBitWidth;
        public Int32 MinDataBitWidth => Model.MinDataBitWidth;
        public Int32 DataBitWidth
        {
            get => Model.DataBitWidth;
            set => Model.DataBitWidth = Math.Clamp(value, MinDataBitWidth, MaxDataBitWidth);
        }

        public Double MaxTimeOutByNs => Model.MaxTimeOutByNs;
        public Double MinTimeOutByNs => Model.MinTimeOutByNs;
        public Double TimeOutByNs
        {
            get => Model.TimeOutByNs;
            set => Model.TimeOutByNs = Math.Clamp(value, MinTimeOutByNs, MaxTimeOutByNs);
        }


        private protected override SPITrigSerialModel Model
        {
            get;
        }
    }
}
