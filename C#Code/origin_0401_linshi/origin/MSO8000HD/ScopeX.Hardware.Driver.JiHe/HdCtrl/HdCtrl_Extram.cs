using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal static class HdCtrl_Extram
    {
        /// <summary>
        /// 教研室218项目抽取的下发函数
        /// </summary>
        /// <param name="ectramPara"></param>
        internal static void ConfigPreExtramNum(Extram218 ectramPara)
        {
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, ectramPara.DecimationMode);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, ectramPara.Multiple_Pattern);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, AcqBdReg.W.Decimation_PreGapValueH16, (UInt32)ectramPara.DecimationQuotient);
        }

        /// <summary>
        /// 优利德抽取方案的下发函数
        /// </summary>
        /// <param name="extramParams"></param>
        /// <param name="extramType"></param>
        internal static void ConfigExtramNum(ExtramUnidroit extramParams, ExtramType extramType)
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W./*DataPath_RstAfifoFromPC*/, 0x1);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, extramParams.DecimationMode | (extramType == ExtramType.Posterior ? 0x10u : 0));

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, (extramType == ExtramType.Posterior && Hd.UIMessage!.bAcquireStopped ? 0 : extramParams.GapX));//extramParams.GapX);
            //zlj20250225
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, (extramType == ExtramType.Posterior && Hd.UIMessage!.bAcquireStopped ? extramParams.GapX : extramParams.GapX));//extramParams.GapX);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, AcqBdReg.W.Decimation_PreGapValueH16, extramParams.GapValue);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreHrCoeL, AcqBdReg.W.Decimation_PreHrCoeH, extramParams.HrCore);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_RstAfifoFromPC, 0x0);
        }//????

        internal static void ConfigAverageCnt(Int32 averageCnt)
        {
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, AcquingParameters.bIsLongStorageMode ? 1 : 0U);
            //if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            //{
            //    HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
            //}
            //else
            //{//dyh_new
            //    Double timescale_AVE_1 = (Hd.UIMessage?.Timebase?.TmbScale ?? 0.02);//us单位
            //    if (timescale_AVE_1 > 0.02)//平均功能
            //    {
            //        HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
            //    }
            //    else
            //    {
            //        HdIO.WriteReg(ProcBdReg.W.Average_Enable, Hd.CurrDebugVarints.bEnable_ProcBd_Average ? 1u : 0);
            //    }
            //    //HdIO.WriteReg(ProcBdReg.W.Average_Enable, Hd.CurrDebugVarints.bEnable_ProcBd_Average ? 1u : 0);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0, (UInt32)1000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0Cnt, (UInt32)3000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1, (UInt32)1000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1Cnt, (UInt32)3000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2, (UInt32)(4 << 12) + 2048);
            //    HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2Cnt, (UInt32)1000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_Number, (UInt32)averageCnt);
            //    HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
            //    HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
            //    HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
            //}
        }//????
    } 
}
