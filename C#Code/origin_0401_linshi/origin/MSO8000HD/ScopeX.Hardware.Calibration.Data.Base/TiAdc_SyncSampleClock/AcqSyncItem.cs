using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public struct AcqSyncItem
    {
        public UInt32 Sample10GClockDelay
        {
            get;
            set;
        }
        public UInt32 Sample20GClockDelay
        {
            get;
            set;
        }

        public UInt32 SyncResetDelay
        {
            get;
            set;
        }
        public UInt32 RMDelay
        {
            get;
            set;
        }
        public UInt32 SerdesDelay
        {
            get;
            set;
        }
        public UInt32 WriteEnableDelay
        {
            get;
            set;
        }
        public UInt32 Reserve1
        {
            get;
            set;
        }
        public UInt32 Reserve2
        {
            get;
            set;
        }
        public UInt32 Reserve3
        {
            get;
            set;
        }
        public UInt32 Reserve4
        {
            get;
            set;
        }
    }
}
