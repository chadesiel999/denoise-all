using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolMIL;

namespace ScopeX.Core.Decode
{

    internal sealed class PSI5TrigSerialModel : TriggerSerialModel
    {
        
        /// <summary>
        /// 事件类型
        /// </summary>
        private ProtocolPSI5.Condition _Condition = ProtocolPSI5.Condition.DataA;
        public ProtocolPSI5.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        /// <summary>
        /// 数据域A
        /// </summary>
        public Int64 MaxDataAValue = 0xFFFFFFF;
        public Int64 MinDataAValue = 0x0;
        private Int64 _DataAValue;
        public Int64 DataAValue
        {
            get { return _DataAValue; }
            set { UpdateProperty(ref _DataAValue, value); }
        }

        /// <summary>
        /// 数据域B
        /// </summary>
        public Int64 MaxDataBValue = 0x3FF;
        public Int64 MinDataBValue = 0x0;
        private Int64 _DataBValue;
        public Int64 DataBValue
        {
            get { return _DataBValue; }
            set { UpdateProperty(ref _DataBValue, value); }
        }


        /// <summary>
        /// BlockID
        /// </summary>
        public Int64 MaxBlockID = (Int64)DataARange.STATUS_DATA_1111;//0x21F;
        public Int64 MinBlockID = (Int64)DataARange.BLOCK_ID_1;//0x200;
        private Int64 _BlockID;
        public Int64 BlockID
        {
            get { return _BlockID; }
            set { UpdateProperty(ref _BlockID, value); }
        }


        /// <summary>
        /// Sensor Status
        /// </summary>
        public Int64 MaxSensorStatus = (Int64)DataARange.SENSOR_DEFECT;//0x1F4;
        public Int64 MinSensorStatus = (Int64)DataARange.BIDIRECTIONAL_COMMUNICATION_RC_OK;//0x1E1;
        private Int64 _SensorStatus;
        public Int64 SensorStatus
        {
            get { return _SensorStatus; }
            set { UpdateProperty(ref _SensorStatus, value); }
        }
        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigPSI5ConditionsOptions(Condition)
            {
                DataAValue = DataAValue,
                DataBValue = DataBValue,
                BlockID = BlockID,
                SensorStatus = SensorStatus,
            };
        }
    }
}
