using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.PlatForm
{
    /// <summary>
    /// Driver层，提供不同产品平台的公共接口
    /// </summary>
    internal interface IPlatForm
    {
        #region 抽取相关

        /// <summary>
        /// 获取前抽的两级下发参数
        /// </summary>
        /// <param name="extramNum">总的前抽倍率</param>
        /// <returns>(base=下发参数, multiple=抽取倍数)</returns>
        (UInt32, UInt32) GetPreSeperateNum(UInt64 extramNum, Dictionary<String, object> addtions = null);

        /// <summary>
        /// 获取有效的前抽倍率
        /// </summary>
        /// <param name="expectedExtramNum">期望的前抽倍率</param>
        /// <returns></returns>
        UInt64 GetValidPreExtractNum(UInt64 expectedExtramNum, Dictionary<String, object> addtions = null);

        #endregion 抽取相关

        #region 插值相关

        /// <summary>
        /// 获取有效的插值倍率
        /// </summary>
        /// <param name="originInterpolate">初始插值倍率</param>
        /// <returns></returns>
        Int32 GetInterpValideNum(Int32 originInterpolate);

        /// <summary>
        /// 获取对应插值倍率的下发值
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        UInt32 GetInterpValideValue(Int32 num);

        #endregion

        #region UPO相关
        /// <summary>
        /// 计算UPO的插值与抽取
        /// </summary>
        /// <param name="currDotNum">当前点数</param>
        /// <param name="targetDotNums">目标点数，先迭代的数优先匹配</param>
        /// <param name="addtions">额外信息</param>
        /// <returns></returns>
        (Double InterpolateNum, UInt32 UPO_ExtractNum) CalcUpoInterpolateAndExtract(double currDotNum, IEnumerable<double> targetDotNums, Dictionary<String, object> addtions = null);

        /// <summary>
        /// 插值是否过大
        /// </summary>
        /// <param name="currInterpolateNum">当前插值数</param>
        /// <returns></returns>
        Boolean IsInterpolateNumGT100(Double currInterpolateNum);

        #endregion UPO相关

    }
}
