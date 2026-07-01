using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 处理进度
    /// </summary>
    public class TaskProgress
    {
        private UInt32 now;

        private UInt32 total;

        private UInt32 typeID;
        /*当前进度*/
        public UInt32 Now { get => now; set => now = value; }
        /*总进度*/
        public UInt32 Total { get => total; set => total = value; }
        /*TypeID*/
        public UInt32 TypeID { get => typeID; set => typeID = value; }

        public TaskProgress()
        {

        }

        public TaskProgress(UInt32 boardID)
        {
            TypeID = boardID;
        }
    }
}
