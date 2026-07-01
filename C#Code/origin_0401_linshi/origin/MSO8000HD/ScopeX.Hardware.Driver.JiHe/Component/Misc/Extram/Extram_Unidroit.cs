using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 优利德的抽取方案：只支持整数倍的抽取倍数，前抽和后抽共用
    /// </summary>
    internal class Extram_Unidroit : AbstractModule_Extram
    {
        internal Extram_Unidroit()
        {

        }

        internal override UInt64 GetValidExtramNum(UInt64 expectedExtramNum, ExtramType extramType, AnaChnlAcqMode acqMode, UInt32 parallelRoads, Boolean isLower = false)
        {
            UInt32[] smallextramnum = _SmallExtramNumModeTable.Keys.ToArray();
            if (expectedExtramNum <= smallextramnum.Min())
                return smallextramnum.Min();

            UInt32 basenum = GetBaseNum(acqMode, parallelRoads);

            if (expectedExtramNum <= basenum && expectedExtramNum <= smallextramnum.Max())
            {
                if (smallextramnum.Contains((UInt32)expectedExtramNum))
                    return expectedExtramNum;

                for (Int32 i = 0; i < smallextramnum.Length - 1; i++)
                {
                    if (smallextramnum[i] < expectedExtramNum && smallextramnum[i + 1] > expectedExtramNum)
                    {
                        return isLower ? smallextramnum[i] : smallextramnum[i + 1];
                    }
                }
            }

            UInt64 validnum = expectedExtramNum / basenum * basenum;

            if (validnum == expectedExtramNum)
                return expectedExtramNum;

            return isLower ? validnum : (validnum + basenum);
        }

        internal override Boolean Config(UInt64 extramNum, ExtramType extramType, UInt32 parallelRoads, AnaChnlAcqMode acqMode)
        {
            var extramparams = GetExtramNum(extramNum, parallelRoads, acqMode);
            if (extramparams == null)
                return false;

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, extramparams.DecimationMode | (extramType == ExtramType.Posterior ? 0x10u : 0));

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, extramparams.GapX);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, AcqBdReg.W.Decimation_PreGapValueH16, extramparams.GapValue);
            
            // reg缺失，need check
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreHrCoeL, AcqBdReg.W.Decimation_PreHrCoeH, extramParams.HrCore);
            return true;
        }

        private static Dictionary<AnaChnlAcqMode, UInt32> _ExtarmModeTable = new()
        {
            { AnaChnlAcqMode.Normal, 0},
            { AnaChnlAcqMode.Average, 0},
            { AnaChnlAcqMode.Peak, 1},
            { AnaChnlAcqMode.HighRes, 3},
        };

        /// <summary>
        /// 小抽取倍数下不同倍数对应的抽取模式，需按升序列表
        /// </summary>
        private Dictionary<UInt32, UInt32> _SmallExtramNumModeTable = new()
        { 
            // 抽取倍数   下发参数
            {1,             0 },
            {2,             1 },
            {4,             2 },
            {10,            5 },
            {20,            6 },
            {40,            7 },
        };

        /// <summary>
        /// 不同抽取模式下，不同并行路数对应的大倍率基数；
        /// 例如：正常抽取模式下，80的并行路数，大抽取倍率都必须是100的整数倍
        /// </summary>
        private Dictionary<AnaChnlAcqMode, Dictionary<UInt32, UInt32>> _BigExtramNumBaseTable = new()
        {
            [AnaChnlAcqMode.Normal] = new()
            {
                { 80, 100 },
                { 40, 50 },
                { 20, 20 },
            },
            [AnaChnlAcqMode.Average] = new()
            {
                { 80, 100 },
                { 40, 50 },
                { 20, 20 },
            },
            [AnaChnlAcqMode.Peak] = new()
            {
                { 80, 50 },
                { 40, 100 },
                { 20, 20 },
            },
            [AnaChnlAcqMode.HighRes] = new()
            {
                { 80, 100 },
                { 40, 50 },
                { 20, 20 },
            }
        };

        private UInt32 GetBaseNum(AnaChnlAcqMode acqMode, UInt32 parallelRoads)
        {
            if (_BigExtramNumBaseTable.ContainsKey(acqMode) && _BigExtramNumBaseTable[acqMode].ContainsKey(parallelRoads))
                return _BigExtramNumBaseTable[acqMode][parallelRoads];

            return 100;
        }

        /// <summary>
        /// FPGA中将截断位数设置为32
        /// </summary>
        private const UInt64 HrCoreBaseNum = 0x1ul << 32;

        private ExtramUnidroit? GetExtramNum(UInt64 extramNum, UInt32 parallelRoads, AnaChnlAcqMode acqMode)
        {
            if (!_ExtarmModeTable.ContainsKey(acqMode))
                return null;

            UInt32 basenum = GetBaseNum(acqMode, parallelRoads);
            if (extramNum < basenum)
            {
                if (_SmallExtramNumModeTable.ContainsKey((UInt32)extramNum))
                    return new ExtramUnidroit(_ExtarmModeTable[acqMode], _SmallExtramNumModeTable[(UInt32)extramNum], 0, _SmallExtramNumModeTable[(UInt32)extramNum]);
                return null;
            }

            return new ExtramUnidroit(_ExtarmModeTable[acqMode], 8, (UInt32)(extramNum / basenum), (UInt32)(HrCoreBaseNum / extramNum));
        }
    }

    internal record ExtramUnidroit(UInt32 DecimationMode, UInt32 GapX, UInt32 GapValue, UInt32 HrCore);
}
