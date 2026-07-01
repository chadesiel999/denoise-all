using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum CPHYPacketType
    {
        Sync,
        VirtualChannel,
        DataType,
        WordCount,
        Data,
        PDCRC,
        PHCRC,
        ERROR
    }
}
