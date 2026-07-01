using System;
using System.Collections.Generic;
using System.Text;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class AbstractController_Misc
    {
        protected delegate void delegateAction();

        protected Action? _AfterFirstInitAction;
        protected delegateAction? _CaliDataChanged = null;
        protected Func<bool>? _AcqIsFulled = null;
        protected Func<UInt32>? _ReadTrigStatus = null;
        protected Action? _AnalogChannelActiveChanged = null;
        protected Action? _AllPowerDown = null;
        protected Action? _ConfigDspCoefficients = null;

        public static void ConfigLongStorage() { Hd.CurrProduct?.Acquirer_AnalogChannel?.ConfigLongStorage(); }
        public static void AfterFirstInitAction() { Hd.CurrProduct?.Ctrl_Misc?._AfterFirstInitAction?.Invoke(); }
        public static void CaliDataChanged() { Hd.CurrProduct?.Ctrl_Misc?._CaliDataChanged?.Invoke(); }
        public static void AnalogChannelActiveChanged() { Hd.CurrProduct?.Ctrl_Misc?._AnalogChannelActiveChanged?.Invoke(); }
        public static void DPX_Config() { AbstractAcquirer_DPX.Config(); }
        public static void ConfigExtractProcessRoadParameters() { Hd.CurrProduct?.Acquirer_AnalogChannel?.ConfigExtractProcessRoadParameters(); }
        public static bool AcqIsFulled() { return Hd.CurrProduct?.Ctrl_Misc?._AcqIsFulled?.Invoke() ?? false; }
        public static UInt32 ReadTrigStatus() { return Hd.CurrProduct?.Ctrl_Misc?._ReadTrigStatus?.Invoke() ?? 0; }
        public static void AllPowerDown() { Hd.CurrProduct?.Ctrl_Misc?._AllPowerDown?.Invoke(); }
        public static void ConfigDspCoefficients() { Hd.CurrProduct?.Ctrl_Misc?._ConfigDspCoefficients?.Invoke(); }

        /// <summary>
        /// 配置插值模式
        /// </summary>
        public static void ConfigInterpolateMode()
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Mode, (UInt32)(1 - (int)(Hd.UIMessage?.Timebase?.InterpolateType ?? AnaChnlItplType.Sinx)));
        }
   /// <summary>
        /// 写完成判断
        /// </summary>
        /// <returns></returns>
        protected bool ourAcqIsFulled()
        {
            if (Hd.UIMessage!.Timebase!.IsScan && !Hd.UIMessage!.bAcquireStopped)
            {
                return HdCtrl_AnalogFifo.CanRead();
            }

            if (Hd.UIMessage!.Timebase!.IsScan && !Hd.UIMessage!.bAcquireStopped)
            {
                ////ZQB_SCAN
                //HdIO.WriteReg(ProcBdReg.W.ScanCtrl_CountLatchEn, 0x0);
                //HdIO.WriteReg(ProcBdReg.W.ScanCtrl_CountLatchEn, 0x1);
                //Acquisition.ScanRunningNewDataPerChannelExistsDotCount = (int)Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.ScanCtrl_WrDataCount, AcqBdNo.B5);
                //Acquisition.ScanRunningNewDataPerChannelExistsDotCount = (int)HdIO.ReadReg(ProcBdReg.R.ScanCtrl_WrDataCount1);
                //Trace.WriteLine($"[ourAcqIsFulled]{Acquisition.ScanRunningNewDataPerChannelExistsDotCount}");

                Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay += Acquisition.ScanRunningNewDataPerChannelExistsDotCount * Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.Scan2ExtractNum_Total;
                if (Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay > Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt)
                    Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt;
                return Acquisition.ScanRunningNewDataPerChannelExistsDotCount != 0;
            }
            else
            {
                if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.bIsLongStorageMode ?? false)
                    return HdCtrl_AnalogDDR.WriteFinished();
                if (Hd.UIMessage?.Display?.IsFast ?? false)
                    //return (HdIO.ReadReg(ProcBdReg.R.Upo_WriteFinished) & 0x01) == 1;
                    return true;
                return (HdIO.ReadReg(PcieBdReg.R.Xdma_XdmaWrFinish) & 0x01) == 1;
            }
            //return false;
        }//????

        protected UInt32 ourReadTrigStatus()
        {
            var trig_status2 = (uint)HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Status) & 0x07;
            //return HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Status) & 0x07;
            return trig_status2;
        }//????
    }
}
