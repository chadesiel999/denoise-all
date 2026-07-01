using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum SPMIFieldType
    {
        FIELD_ARBITRATION_START,
        FIELD_CONNECT_BIT,
        FIELD_ALERT_BIT,
        FIELD_SR_BIT,
        FIELD_MASTER_ID,
        FIELD_PRIMARY_LEVEL,
        FIELD_SECONDARY_LEVEL,
        FIELD_SLAVE_ADDRESS,
        FIELD_COMMAND_START,
        FIELD_COMMAND_TYPE,
        FIELD_COMMAND_ADDRESS,
        FIELD_DATA,
        FIELD_PARITY,
        FIELD_ACKNACK,
        FIELD_ERROR,
    };
}
