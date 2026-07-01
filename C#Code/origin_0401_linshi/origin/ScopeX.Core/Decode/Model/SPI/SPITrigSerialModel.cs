using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolRS232;

namespace ScopeX.Core.Decode
{
    internal sealed class SPITrigSerialModel : TriggerSerialModel
    {
        private const int _MAX_DATA_WIDTH = 64;

        private ComModel.ProtocolSPI.Condition _Condition;

        public ComModel.ProtocolSPI.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }
        private ComModel.ProtocolSPI.FramingMode _SpiTrigMode;

        public ComModel.ProtocolSPI.FramingMode SpiTrigMode
        {
            get { return _SpiTrigMode; }
            set { UpdateProperty(ref _SpiTrigMode, value); }
        }
        private ComModel.ProtocolSPI.DataTriggerSource _DataSource;

        public ComModel.ProtocolSPI.DataTriggerSource DataSource
        {
            get { return _DataSource; }
            set { UpdateProperty(ref _DataSource, value); }
        }

        private ProtocolSPI.FramingMode _TrigMode;
        public ProtocolSPI.FramingMode TrigMode
        {
            get { return _TrigMode; }
            set { UpdateProperty(ref _TrigMode, value); }
        }

        //帧长
        public Int32 MaxFrameCount => 64;
        public Int32 MinFrameCount => 1;

        private Int32 _FrameCount = 1;

        public Int32 FrameCount
        {
            get { return _FrameCount; }
            set
            {
                if (_FrameCount == value || value < 0)
                {
                    return;
                }
                _FrameCount = Math.Clamp(value, MinFrameCount, MaxFrameCount);
                //更新位宽一致
                if (value * DataBitWidth > _MAX_DATA_WIDTH)
                {
                    Int32 _tmpDataBitWidth = (Int32)(_MAX_DATA_WIDTH / value);
                    if (_tmpDataBitWidth != _DataBitWidth)
                    {
                        UpdateProperty(ref _DataBitWidth, _tmpDataBitWidth);
                    }
                }
                Int64 mask = (Int64)(1L << (value * DataBitWidth)) - 1;
                Int64 _tempFrameData = _FrameData & mask;//根据位宽和帧长设置更新数据
                _tempFrameData = Math.Clamp(_tempFrameData, MinFrameData, MaxFrameData);
                UpdateProperty(ref _FrameData, _tempFrameData);
                UpdateProperty(ref _FrameCount, value);
            }
        }


        //数据位宽
        public Int32 MaxDataBitWidth => 64;
        public Int32 MinDataBitWidth => 1;

        private Int32 _DataBitWidth = 8;

        public Int32 DataBitWidth
        {
            get { return _DataBitWidth; }
            set
            {
                if (_DataBitWidth == value || value < 0)
                {
                    return;
                }

                //更新帧长一致
                if (value * _FrameCount > _MAX_DATA_WIDTH)
                {
                    Int32 _tmpFrameCount = (Int32)(_MAX_DATA_WIDTH / value);
                    if (_tmpFrameCount != _FrameCount)
                    {
                        UpdateProperty(ref _FrameCount, _tmpFrameCount);
                    }
                }
                Int64 mask = (Int64)(1L << (value * FrameCount)) - 1;
                Int64 _tempFrameData = _FrameData & mask;//根据位宽和帧长设置更新数据
                _tempFrameData = Math.Clamp(_tempFrameData, MinFrameData, MaxFrameData);
                UpdateProperty(ref _FrameData, _tempFrameData);
                UpdateProperty(ref _DataBitWidth, value);
            }
        }

        public Double _TimeOutByNs;
        public Double TimeOutByNs
        {
            get { return _TimeOutByNs; }
            set
            {
                _TimeOutByNs = Math.Clamp(_TimeOutByNs, MinTimeOutByNs, MaxTimeOutByNs);
                UpdateProperty(ref _TimeOutByNs, value);
            }
        }
        public Int64 MaxTimeOutByNs => 500; //临时占位  具体待定 待调整
        public Int64 MinTimeOutByNs => 0;

        //public UInt64 MaxFrameData => _DataBitWidth > 0 ? (UInt64)Math.Pow(2, _DataBitWidth) : (UInt64)Math.Pow(2, MaxDataBitWidth);
        //public UInt64 MinFrameData => 1;

        //private UInt64 _FrameData;

        //public UInt64 FrameData
        //{
        //    get { return _FrameData; }
        //    set { UpdateProperty(ref _FrameData, value); }
        //}

        public Int64 MaxFrameDataHigh => LongDataEnable ? (Int64)Math.Pow(2, _DataBitWidth - 64) - 1 : 0;
        public Int64 MaxFrameData => _DataBitWidth > 0 ? (Int64)Math.Pow(2, (_FrameCount * _DataBitWidth)) - 1 : (Int64)Math.Pow(2, 64);//最大数据应同时考虑帧长与位宽
        public Int64 MinFrameData => 0;

        public Boolean LongDataEnable
        {
            get => _DataBitWidth > 64;
        }
        private Int64 _FrameData;

        public Int64 FrameData
        {
            get { return _FrameData; }
            set { UpdateProperty(ref _FrameData, value); }
        }

        private Int64 _FrameDataHigh;

        public Int64 FrameDataHigh
        {
            get { return _FrameDataHigh; }
            set { UpdateProperty(ref _FrameDataHigh, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigSPIConditionsOptions(Condition)
            {
                DataSource = DataSource,
                FrameCount = FrameCount,
                FrameData = FrameData,
                FrameDataHigh = FrameDataHigh,
                DataBitWidth = (UInt64)DataBitWidth,
            };
        }
    }
}
