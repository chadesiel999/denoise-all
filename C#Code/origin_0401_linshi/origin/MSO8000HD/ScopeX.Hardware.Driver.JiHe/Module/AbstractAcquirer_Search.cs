using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public class AbstractAcquirer_Search : AbstractAcquirer
    {
        internal override void CreateAcquireAttribute() { }
        internal override bool bDataVaild { get; set; }
        /// <summary>
        /// 上电缺省初始化，与系统的环境变量无关
        /// </summary>
        internal override void Init() { }
        /// <summary>
        /// 开启下一次新的采集
        /// </summary>
        internal override void InitAcq() { }
        /// <summary>
        /// 读取采集到的数据
        /// </summary>
        /// <returns></returns>
        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            samplingRateByus = 1.0;
            return false; 
        }
        /// <summary>
        /// 采集数据的后处理
        /// </summary>
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken) { }

        public virtual bool TryTakeResult([NotNullWhen(true)] out Dictionary<long, (Double[,] Result, Int32 ResultCount)> result)
        {
            result = new();

            Monitor.Enter(SearchData.UpdateDataLock);

            foreach (var item in AcqedDataPool.SearchData.Data)
                result.Add(item.Key, item.Value);

            Monitor.Exit(SearchData.UpdateDataLock);

            return Hd.bAcqedNewData;
        }
    }
}
