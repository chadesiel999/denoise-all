using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum PSI5FieldType
    {
        FIELD_START_BIT = 0,
        FIELD_SERIAL_MESSAGE,
        FIELD_FRAME_CONTROL,
        FIELD_STATUS,
        FIELD_DATA_B,
        FIELD_DATA_A,
        FIELD_DATA_A_REST,
        FIELD_DATA_A_INIT,
        FIELD_VERIFY_CRC
    };
}
