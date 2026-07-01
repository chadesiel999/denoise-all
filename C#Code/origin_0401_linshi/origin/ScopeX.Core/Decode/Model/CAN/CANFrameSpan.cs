using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode;

internal class CANFrameSpan
{
    public Int32 StartIndex;
    public Int32 EndIndex;
    Int32 bitLenByTime;
    public Int32 FrameSpanIndexLen => EndIndex - StartIndex;

    public Int32 BitLen
    {
        get
        {
            if (bitLenByTime <= 0)
            {
                return 0;
            }
            return FrameSpanIndexLen / bitLenByTime;
        }
    }
    public CANFrameSpan(Int32 startIndex, Int32 endIndex
        , Int32 bitLenByTime)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        this.bitLenByTime = bitLenByTime;
    }
}
