using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// Adc交织相关
    /// </summary>
    public interface IAdcInterleave
    {
        /// <summary>
        /// 单路Adc的采集存储深度定义
        /// </summary>
        IReadOnlyList<KeyValuePair<String, Int32>> PerAdcStorageLength { get;}

        /// <summary>
        /// 获取当前的交织模式
        /// </summary>
        /// <returns></returns>
        AdcInterleaveMode GetCurrentMode();
    }
}
