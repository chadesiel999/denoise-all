using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.PlatForm
{
    /// <summary>
    /// 实现平台接口的基类
    /// </summary>
    internal class PlatForm_Base : IPlatForm
    {
        public virtual (UInt32, UInt32) GetPreSeperateNum(UInt64 extramNum, Dictionary<String, object> addtions = null)
        {
            return (1, 1);
        }

        public virtual UInt64 GetValidPreExtractNum(UInt64 expectedExtramNum, Dictionary<String, object> addtions = null)
        {
            return 1;
        }

        public virtual Int32 GetInterpValideNum(Int32 originInterpolate)
        {
            return 1;
        }

        public virtual UInt32 GetInterpValideValue(Int32 num)
        {
            return 1U;
        }

        public virtual (Double InterpolateNum, UInt32 UPO_ExtractNum) CalcUpoInterpolateAndExtract(double currDotNum, IEnumerable<double> targetDotNums, Dictionary<String, object> addtions = null)
        {
            foreach (var targetDotNum in targetDotNums)
            {
                if (currDotNum == targetDotNum)
                    return (1, 1);
                else if (currDotNum < targetDotNum)
                {
                    return (Math.Ceiling(targetDotNum / currDotNum), 1);
                }
                else//currDotNum > targetDotNum
                {
                    return (1D, (UInt32)Math.Floor(currDotNum / targetDotNum));
                }
            }
            return (1, 1);
        }

        public virtual Boolean IsInterpolateNumGT100(Double currInterpolateNum)
        {
            return currInterpolateNum > 100D;
        }
    }
}
