using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal abstract class AbstractModule_Extram
    {
        internal virtual UInt64 GetValidExtramNum(UInt64 expectedExtramNum, ExtramType extramType, AnaChnlAcqMode acqMode, UInt32 parallelRoads, Boolean isLower = false)
        {
            return expectedExtramNum;
        }

        internal virtual Boolean Config(UInt64 extramNum, ExtramType extramType, UInt32 parallelRoads, AnaChnlAcqMode acqMode)
        {
            return false;
        }
    }

    /// <summary>
    /// 抽取类型：前抽、后抽
    /// </summary>
    internal enum ExtramType
    {
        /// <summary>
        /// 前抽
        /// </summary>
        Preceding,

        /// <summary>
        /// 后抽
        /// </summary>
        Posterior,
    }
}
