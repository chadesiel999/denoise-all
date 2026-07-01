using System;
using System.Collections.Generic;

namespace ScopeX.Updater.Base.Infos
{
    [Serializable]
    public class DataBlock
    {
        public string Name { get; set; }
        public Dictionary<UInt32, UInt32> Addrs { get; set; }// StartAddr,SizeBytes;
    }
}
