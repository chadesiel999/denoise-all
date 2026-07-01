using System;

namespace ScopeX.Updater.Base
{
    [Serializable]
    public class BaseDataBlock
    {
        public UInt32 BoardID { get; set; }
        public DateTime Date { get; set; }
        public string CRC16 { get; set; }
        public string Remarks { get; set; }
    }
}
