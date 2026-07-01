using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public class DecoderPackage
    {
        public UInt16[] SourceData
        {
            get;
        }
        public UInt32 Addr_First
        {
            get;
            internal set;
        }
        public UInt32 Addr_Last
        {
            get;
            internal set;
        }
        public UInt32 Addr_Trig
        {
            get;
            internal set;
        }
    }
}
