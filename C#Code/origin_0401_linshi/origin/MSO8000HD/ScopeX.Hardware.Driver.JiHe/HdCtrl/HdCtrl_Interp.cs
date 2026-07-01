using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal static class HdCtrl_Interp
    {
        internal static void ConfigNum(UInt32 interpNum)//????
        {
            //if (interpNum == 1)
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterEn, 0);
            //else
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterEn, Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation ? 1U : 0);

            //if (_ValidInterpNumTable.ContainsKey(interpNum))
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterpRate, _ValidInterpNumTable[interpNum]);
            //else
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProMultiInterpRate, _ValidInterpNumTable[1]);// 默认发1插
        }

        private static Dictionary<UInt32, UInt32> _ValidInterpNumTable = new()
        {
            {1, 0x0101 },
            {2, 0x0102 },
            {5, 0x0105 },
            {10, 0x010a },
            {20, 0x020a },
            {50, 0x050a },
            {100, 0x0a0a },
            {200, 0x140a },
        };

        internal static UInt32 GetValidInterpNum(Double expectedNum)
        {
            if (expectedNum < 1.0 || (Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation == false) || !(Hd.UIMessage?.Timebase?.InterplotEnable?? true))
                return 1;
            UInt32[] validNum = _ValidInterpNumTable.Keys.ToArray();
            for (Int32 i = 0; i < validNum.Length - 1; i++)
            { 
                if (validNum[i] <= expectedNum && validNum[i+1] > expectedNum)
                    return validNum[i];
            }
            return validNum.Max();
        }
    }
}
