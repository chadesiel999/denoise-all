using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class AdcRegisterDataFormat
    {
        [JsonPropertyName("BoardIndex")]
        public int BoardIndex
        {
            get;
            set;
        }
        public Adc[]? AllAdc
        {
            get;
            set;
        }
    }
    public class Adc
    {
        [JsonPropertyName("AdcIndex")]
        public int AdcIndex
        {
            get;
            set;
        }
        public RegisterAddrValuePair[]? RegisterValuePair
        {
            get;
            set;
        }
    }
    public class RegisterAddrValuePair
    {
        [JsonPropertyName("RegAddress")]
        public uint RegAddress
        {
            get;
            set;
        }
        [JsonPropertyName("RegValue")]
        public uint RegValue
        {
            get;
            set;
        }
    }
}
