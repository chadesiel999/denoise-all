using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class AiAnalogChannelParams : ICaliData
    {
        private AiAnalogChannelParams()
        {

        }

        public static AiAnalogChannelParams Default = new();

        private DictionaryParams<AiAnalogChannelItem> _ParamsInfo = new(CaliConstants.KeyStrLen);

        public AiAnalogChannelItem this[String paramName]
        {
            get => _ParamsInfo[paramName];
            set => _ParamsInfo[paramName] = value;
        }

        public String[] AllNames => _ParamsInfo.AllNames;

        public CaliDataType DataType => CaliDataType.AiAnalogParams;

        public int TotalBytes => _ParamsInfo.TotalBytes;

        public int OriginTotleBytes { get; set; }

        public void Deserialize(byte[] content)
        {
            _ParamsInfo.Deserialize(content);
        }

        public byte[] Serialize()
        {
            return _ParamsInfo.Serialize();
        }
    }
}