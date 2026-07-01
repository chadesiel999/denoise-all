using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.PlatForm
{
    internal class PlatForm_DBI13G:PlatForm_Base
    {
        #region 抽取相关

        /*前抽相关*/
        /// <summary>
        ///  FPGA前抽模块，20G抽取倍数及下发参数定义表
        /// </summary>
        private Dictionary<UInt32, UInt32> _PreExtractTable = new()
        { 
            // 抽取倍数   下发参数
            {1,             1},
            {2,             2},
            {4,             4},
            {8,             8},
            {16,            0x10},
            {32,            0x20},
            {64,            0x40},
        };

        public override UInt64 GetValidPreExtractNum(UInt64 expectedExtramNum, Dictionary<String, object> addtions = null)
        {
            (Double DdrSampGhz, UInt32 baseExt) maxConfig = (20, _PreExtractTable.Keys.Last());//前抽base的最大配置,20G对应最后一个抽取倍率
            double currDdrSampGhz = 20;//当前的ddr采样率，默认20G

            if (addtions != null)
            {
                if (addtions.Keys.Contains(nameof(AdcInterleaveMode)))
                {
                    var interleave = (AdcInterleaveMode)addtions[nameof(AdcInterleaveMode)];
                    if (interleave == AdcInterleaveMode.Mode1To1)//10G模式
                    {
                        currDdrSampGhz = 10;
                    }
                }
            }
            var newBase = _PreExtractTable.Keys.Where(b =>
            {
                return b <= (maxConfig.baseExt / (maxConfig.DdrSampGhz / currDdrSampGhz));
            }).ToArray();
            return GetValidExtractNum(expectedExtramNum, newBase, newBase.Last());
        }

        /// <summary>
        /// 获取有效的前抽倍数
        /// </summary>
        /// <param name="expectedExtramNum"></param>
        /// <param name="baseExtractNum"></param>
        /// <param name="baseMum"></param>
        /// <returns></returns>
        private UInt64 GetValidExtractNum(UInt64 expectedExtramNum, UInt32[] baseExtractNum, UInt32 baseMum)
        {
            if (expectedExtramNum <= baseExtractNum.Min())
                return baseExtractNum.Min();

            if (expectedExtramNum <= baseMum)
            {
                if (baseExtractNum.Contains((UInt32)expectedExtramNum))
                    return expectedExtramNum;

                for (Int32 i = 0; i < baseExtractNum.Length - 1; i++)
                {
                    if (baseExtractNum[i] < expectedExtramNum && baseExtractNum[i + 1] > expectedExtramNum)
                    {
                        return baseExtractNum[i + 1];
                    }
                }
            }

            /*第二级抽取为1，2，5步进*/
            UInt32 secondExtram = (UInt32)(Math.Ceiling(((double)expectedExtramNum) / baseMum));
            var digitNum = Math.Pow(10, secondExtram.ToString().Length - 1);
            if (secondExtram <= 1 * digitNum)
            {
                secondExtram = (UInt32)(1 * digitNum);
            }
            else if (secondExtram <= 2 * digitNum)
            {
                secondExtram = (UInt32)(2 * digitNum);
            }
            else if (secondExtram <= 5 * digitNum)
            {
                secondExtram = (UInt32)(5 * digitNum);
            }
            else if (secondExtram <= 10 * digitNum)
            {
                secondExtram = (UInt32)(10 * digitNum);
            }

            //Debug.WriteLine($"[{DateTime.Now}]需要第二级前抽：" +
            //    $"expectedExtramNum_{expectedExtramNum}," +
            //    $"baseMum_{baseMum}," +
            //    $"secondExtram_{secondExtram}", "Debug");
            return secondExtram * baseMum;
        }

        /// <summary>
        /// 获取前抽的两级下发参数
        /// </summary>
        /// <param name="extramNum">总的前抽倍率</param>
        /// <returns>(base=下发参数, multiple=抽取倍数)</returns>
        public override (UInt32, UInt32) GetPreSeperateNum(UInt64 extramNum, Dictionary<String, object> addtions = null)
        {
            (Double DdrSampGhz, UInt32 baseExt) maxConfig = (20, _PreExtractTable.Keys.Last());//前抽base的最大配置,20G对应最后一个抽取倍率
            double currDdrSampGhz = 20;//当前的ddr采样率，默认20G

            if (addtions != null)
            {
                if (addtions.Keys.Contains(nameof(DdrData4What)))
                {
                    if ((DdrData4What)addtions[nameof(DdrData4What)] == DdrData4What.Dso)
                    {
                        if (addtions.Keys.Contains(nameof(AdcInterleaveMode)))
                        {
                            var interleave = (AdcInterleaveMode)addtions[nameof(AdcInterleaveMode)];
                            if (interleave == AdcInterleaveMode.Mode1To1)//10G模式
                            {
                                currDdrSampGhz = 10;
                            }
                        }
                    }
                    else if ((DdrData4What)addtions[nameof(DdrData4What)] == DdrData4What.LA)
                    {
                        currDdrSampGhz = 1.25;//LA固定//todo:待确认
                    }
                }
            }

            var preBaseNum = (UInt32)(maxConfig.baseExt / (maxConfig.DdrSampGhz / currDdrSampGhz));
            if (extramNum < preBaseNum)
            {
                return (_PreExtractTable[(UInt32)extramNum], 1);
            }
            return (_PreExtractTable[preBaseNum], (UInt32)(extramNum / preBaseNum));
        }

        #endregion 抽取相关

        #region 插值相关
        private static Dictionary<Int32, UInt32> _InterpNumTable = new()
        {
            {1, 100 },
            {2, 50 },
            {5, 20 },
            {10, 10 },
            {20, 5 },
            {50, 2 },
            {100, 1 },
        };

        public override Int32 GetInterpValideNum(Int32 originInterpolate)
        {
            foreach (var item in _InterpNumTable)
            {
                if (item.Key >= originInterpolate)
                    return item.Key;
            }
            return _InterpNumTable.Last().Key;
        }

        public override UInt32 GetInterpValideValue(Int32 num)
        {
            if (_InterpNumTable.ContainsKey(num))
            {
                return _InterpNumTable[num];
            }
            return _InterpNumTable.First().Value;
        }

        #endregion 插值相关

        #region UPO相关
        public override (Double InterpolateNum, UInt32 UPO_ExtractNum) CalcUpoInterpolateAndExtract(double currDotNum, IEnumerable<double> targetDotNums, Dictionary<String, object> addtions = null)
        {
            //插值表，从小到大
            List<int> interpNums = _InterpNumTable.Keys.OrderBy(n => n).ToList();

            /*优先匹配目标列表*/
            foreach (var targetDotNum in targetDotNums)
            {
                if (currDotNum == targetDotNum)
                    return (1, 1);

                //说明：1）插值只能在插值表中找有效的值；2）UPO抽取的值是任意正整数;
                else if (currDotNum < targetDotNum)
                {
                    int maxExtraNum = interpNums.Max();
                    for (int extraNum = 1; extraNum <= maxExtraNum; extraNum++)
                    {
                        var interpNum = (int)Math.Ceiling(targetDotNum / (currDotNum / extraNum));
                        interpNum = GetInterpValideNum(interpNum);
                        if (targetDotNum == currDotNum * interpNum)
                        {
                            return (interpNum, (UInt32)extraNum);
                        }
                    }
                }
                else//currDotNum > targetDotNum
                {
                    foreach (var interpNum in interpNums)
                    {
                        double intextraNum_double = (currDotNum * interpNum / targetDotNum);
                        if (intextraNum_double.CompareTo((int)intextraNum_double) == 0)
                        {
                            return (interpNum, (UInt32)intextraNum_double);
                        }
                    }
                }
            }

            /*没匹配到*/
            double maxDotNum = targetDotNums.Max();
            if (currDotNum <= maxDotNum)
            {//插值
                for (int i = 0; i < interpNums.Count; i++)
                {
                    if (currDotNum * interpNums[i] > maxDotNum)
                    {
                        if (i == 0)
                        {
                            //这种情况理论上不可能
                            Hd.SysLogger?.Invoke($"UPO计算抽取插值异常!", "Error");
                            return (1, 1);
                        }
                        return (interpNums[i - 1], 1);
                    }
                }
                return (interpNums.Last(), 1);
            }
            else//currDotNum > targetDotNum
            {//抽取
                return (1D, (UInt32)Math.Ceiling(currDotNum / maxDotNum));
            }
        }
        #endregion UPO相关

    }
}
