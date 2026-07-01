using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 218阶段采用的抽取方案：除数+余数，支持的抽取倍数更加多样化
    /// </summary>
    internal class Module_Extram218 : AbstractModule_Extram
    {
        internal override UInt64 GetValidExtramNum(UInt64 expectedExtramNum, ExtramType extramType, AnaChnlAcqMode acqMode, UInt32 parallelRoads, Boolean isLower = false)
        {
            UInt64[] validextramtable = _PreExtramModeTable.Keys.ToArray();
            if (expectedExtramNum <= validextramtable.Min())
            {
                return validextramtable.Min();
            }
            if (expectedExtramNum <= validextramtable.Max())
            {
                if (_PreExtramModeTable.ContainsKey(expectedExtramNum))
                    return expectedExtramNum;

                for (Int32 i = 0; i < validextramtable.Length; i++)
                {
                    if (validextramtable[i] < expectedExtramNum && validextramtable[i + 1] > expectedExtramNum)
                    {
                        return isLower ? validextramtable[i] : validextramtable[i + 1];
                    }
                }
            }

            return GetValidNumBy124(expectedExtramNum, isLower);
        }

        internal override bool Config(ulong extramNum, ExtramType extramType, uint parallelRoads, AnaChnlAcqMode acqMode)
        {
            Extram218? extram = GetPreExtramPara(extramNum, parallelRoads, acqMode);
            if (extram != null)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, extram.DecimationMode);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, extram.Multiple_Pattern);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, AcqBdReg.W.Decimation_PreGapValueH16, (UInt32)extram.DecimationQuotient);

                return true;
            }
            return false;
        }

        private const UInt64 _Step = 10;
        /// <summary>
        /// 获取以1-2-4步进的合法值
        /// </summary>
        /// <param name="expectedExtramNum"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        private UInt64 GetValidNumBy124(UInt64 expectedExtramNum, Boolean isLower = true)
        {
            UInt64 basevalue = _Step;
            while (expectedExtramNum > basevalue)
            {
                basevalue *= _Step;
            }

            UInt64 basevalue1 = basevalue / _Step;
            UInt64 basevalue2 = basevalue1 * 2;
            UInt64 basevalue4 = basevalue1 * 4;
            if (expectedExtramNum == basevalue1 || expectedExtramNum == basevalue2 || expectedExtramNum == basevalue4 || expectedExtramNum == basevalue)
                return expectedExtramNum;

            if (basevalue1 < expectedExtramNum && basevalue2 > expectedExtramNum)
                return isLower ? basevalue1 : basevalue2;

            if (basevalue2 < expectedExtramNum && basevalue4 > expectedExtramNum)
                return isLower ? basevalue2 : basevalue4;

            return isLower ? basevalue4 : basevalue;
        }

        /// <summary>
        /// 小抽取倍数下的发送的抽取模式，需要已经从小到大进行排序
        /// </summary>
        private Dictionary<UInt64, UInt32> _PreExtramModeTable = new Dictionary<UInt64, UInt32>()
        {
            // 抽取倍数   下发参数
            {1,             0 },
            {2,             1 },
            {4,             2 },
            {10,            5 },
            {20,            6 },
            {40,            7 },
        };

        private Dictionary<AnaChnlAcqMode, UInt32> _PreExtarmMode = new()
        {
            { AnaChnlAcqMode.Normal, 0},
        };

        private Extram218? GetPreExtramPara(UInt64 extramNum, UInt32 parallelRoads, AnaChnlAcqMode extramMode)
        {
            if (!_PreExtarmMode.ContainsKey(extramMode))
                return null;

            Extram218 ectram = new Extram218();
            ectram.DecimationMode = _PreExtarmMode[extramMode];
            if (_PreExtramModeTable.ContainsKey(extramNum) && extramNum < parallelRoads)
            {
                ectram.Multiple_Pattern = _PreExtramModeTable[extramNum];
                ectram.RemainderAdditional = 0;
                ectram.DecimationQuotient = 0;
            }
            else
            {
                ectram.Multiple_Pattern = 8;
                switch (extramMode)
                {
                    case AnaChnlAcqMode.Normal:
                        ectram.DecimationQuotient = extramNum / parallelRoads;
                        ectram.RemainderAdditional = (extramNum / (parallelRoads / 4)) % 4;
                        break;
                    case AnaChnlAcqMode.Peak:
                        ectram.DecimationQuotient = extramNum * 2 / parallelRoads;
                        if (extramNum * 2 % parallelRoads == 0)
                            ectram.RemainderAdditional = 0x1 << 7;
                        else
                            ectram.RemainderAdditional = parallelRoads / (extramNum * 2 % parallelRoads);
                        break;
                    case AnaChnlAcqMode.HighRes:
                        ectram.DecimationQuotient = extramNum / parallelRoads;
                        if (extramNum % parallelRoads == 0)
                            ectram.RemainderAdditional = 0x1 << 7;
                        else
                            ectram.RemainderAdditional = parallelRoads / (extramNum % parallelRoads);
                        break;
                    default:
                        break;
                }
            }
            return ectram;
        }
    }

    internal record Extram218
    {
        internal UInt32 DecimationMode;
        internal UInt32 Multiple_Pattern;
        internal UInt64 RemainderAdditional;
        internal UInt64 DecimationQuotient;
    }
}
