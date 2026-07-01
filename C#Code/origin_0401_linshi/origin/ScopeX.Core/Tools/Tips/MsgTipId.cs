using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Tools
{
    public enum MessageType
    {
        /// <summary>
        /// 提示
        /// </summary>
        Information,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 询问
        /// </summary>
        Asking,
    }

    public enum MsgTipId
    {
        None,
        GreatethanMax,
        LessthanMin,
        Unused1,
        Unused2,
        InvalidData,
        Undefined,
        NoPermissionToAccessThisPath,
        UnSupportedFormat,
        SavingFailed,
        ReadingFailed,
        SDABitRateFault,
        ThisItemDisabled,
        FormulaEndlessLoopDef,
        FormulaUndef,
        SavingSuccess,
        ReadingSuccess,
        SetFreqFail,
        CreateMaskFail,
        InputFormatError,
        OnlyOneAnalysis,
        SDAPrepare,
        SDAAnalysis,
        Unused3,
        USBDeviceOpened,
        USBDeviceClosed,
        NetCommunicateOpened,
        NetCommunicateClosed,
        SdaInputError,
        ErrorHappenedCheckIt,
        OutPutTenMLockSuccess,
        OutPutTenMLockFail,
        OpenMeasureFuction,
        InputError,
        FeatureUnsupported,
        NoMoreChannels,
        NoMoreMeasuerLabel,
        MeasuerLabelExisted,
        NoMoreRefChannels,
        CalibrationFault,
        CalibrationSuccess,
        CalibrationCancel,
        NotInstallExcel,
        ChannelHadOpened,
        IllegalName,
        Information,
        Asking,
        Warning,
        Error,
        ResolutionMismatch,
        FileExisted,
        FileOccupied,
        NoMoreDiskSpace,
        CheckDiskSpaceError,
        RestoreFactoryDefault,
        UndoFactoryDefault,
        UndoFactoryDefaultErr,
        SwitchSettingFile,
        AppClose,
        ComputerShutDown,
        ComputerRestart,
        UserLogOut,
        AnotherRFChannelExisted,
        AutoGetIPFail,
        AutoGetIPSuccess,
        SetIPAddressFail,
        SetGatewayFail,
        SetDNSFail,
        SetIPSuccess,

        GetBrigtnessFail,
        SetBrigtnessFail,
        GetContrastFail,
        SetContrastFail,
        RemovableStorageDiscovered,
        RemovableStorageRemoved,
        UnknownRemovableStorage,
        AdministrtorAuthorityRequired,

        PrintPageSucceeded,
        PrintPageFailed,
        SDADataDeficient,
        FunctionUnused,
        SDADataAmplitudeDiffTooSmall,
        FewUINumber,
        SDADataCyclesTooFew,
        TIEResultError,
        CreateEyeError,
        ClearingSuccess,
        AccessToFolderFailed,
        DefaultingFailed,
        NoSignal,
        DecodeNotSupportedMoreThan250MStorageDepth,
        AutoSetFail,
        SOASaveError,
        SOALoadMaskError,
        SOALoadMaskSuccess,
        FilePathNotExist,
        AutoCalibration,
        DataLengthIsInsufficient,
        DataLengthIsTooLarge,
        AUXOutIsRequired,
        AUXInIsRequired,
        AWGProtected1,
        AWGProtected2,
        AWGProtected3,
        AWGProtected4,
        //<Remark>更改人：彭博 创建日期：2023/12/4 11:51:00  原因：若无法选择此工作方式，给出提示 </Remark>
        /// <summary>
        /// 调制模式
        /// </summary>
        AWGModeModulation,
        /// <summary>
        /// 扫频模式
        /// </summary>
        AWGModeSweep,
        DigitalOccupyAnalog,
        StopBanDigital,
        FunctionBanDigital,
        DigitalMutex,
        ChannelUITooLess,
        CurrentChannelNoData,
        CurrentChannelAmpTooLow,
        SegementIsNotSupportedInScan,
        SegementIsNotSupportedInThisAcqMode,
        MathIsNotSupportedInScan,
        PowerAnalysisIsNotSupportedInScan,
        JitterIsNotSupportedInScan,
        PassFailIsNotSupportedInScan,
        DecodeIsNotSupportedInScan,
        DecodeIsSupportedMinTimebase,
        JitterIsSupportedMinTimebase,
        SegementIsNotSupportedInPowerAnalysis,
        SegementIsNotSupportedInJitter,
        SegementIsNotSupportedInPassFail,
        SegementIsNotSupportedInDecode,
        SegementIsNotSupportedInThisTriggerMode,
        SingleTriggerIsNotSupportedInSegement,
        PowerAnalysisIsNotSupportedInJitter,
        PowerAnalysisIsNotSupportedInPassFail,
        PowerAnalysisIsNotSupportedInDecode,
        PowerAnalysisIsNotSupportedInSegment,
        PwrAnalysisIsNotSupportedInLA,
        JitterIsNotSupportedInPowerAnalysis,
        JitterIsNotSupportedInPassFail,
        JitterIsNotSupportedInDecode,
        JitterIsNotSupportedInSegment,
        JitterIsNotSupportedInFFT,
        JitterIsNotSupportedInLA,
        FFTIsNotSupportedInJitter,
        PassFailIsNotSupportedInPowerAnalysis,
        PassFailIsNotSupportedInJitter,
        PassFailIsNotSupportedInDecode,
        PassFailIsNotSupportedInSegment,
        DecodeIsNotSupportedInPowerAnalysis,
        DecodeIsNotSupportedInJitter,
        DecodeIsNotSupportedInPassFail,
        DecodeIsNotSupportedInSegment,
        DecodeIsNotSupportedInLA,
        MathIsNotSupportedInLA,
        DigitalIsNotSupportedInDecode,
        DigitalIsNotSupportedInMath,
        DigitalIsNotSupportedInPowerAnalysis,
        DigitalIsNotSupportedInJitter,
        DigitalIsNotSupportedInLissajous,
        LissajousIsNotSupportedInDigital,
        ScanIsNotSupportedInFast,
        DemoIsNotSupportedInFast,
        FastIsNotSupportedInScan,
        //解码阈值范围弱提示 ljw 24.6
        DecodingThresholdRangeWeakHints,
        PurchaseOptions,
        OptionActiveSuccess,
        OptionActiveFail,
        OptionActived,
        LicenseFileFormatError,
        LicenseRemoveYesOrNo,
        LicenseRemoveSuccess,

        TimeMinScaleBy2_5G,
        TimeMinScaleBy5G,
        ChannelClosed,
        PassFailChannelClosed,
        RunningLoopAnalysisTip,
        CompletedLoopAnalysisTip,
        RunningPSRRTip,
        CompletedPSRRTip,
        FileSaveCheck,
        HistBinNumberTooFew,
        BathWaveFitError,

        RefFileError1,
        RefFileError2,
        PassFaileStop,

        #region Filter
        FilterTip1,
        FilterTip2,
        #endregion

        MatlabUninstalled,

        RestartingProgram,
        FunctionDisabled,
        FinetuningofamplitudeON,
        FinetuningofamplitudeOFF,

        /// <summary>
        /// 拖拽开始绘制矩形
        /// </summary>
        Drag2DrawRectangle,

        /// <summary>
        /// 没有通道开启
        /// </summary>
        NoActivedChannel,

        /// <summary>
        /// 只能开启两个区域触发
        /// </summary>
        OnlyEnableTwoVisualTrigger,
        
        /// <summary>
        /// 深存储数据导出失败
        /// </summary>
        UnknownErrorSaveData,

        /// <summary>
        /// 保存超时
        /// </summary>
        TimeOutSaveData,

        ProbeConnected,
        ProbeRemoved,
        ProbeUnavailable,

        /// <summary>
        /// 探头校准参考信息未接入(增益校准时)
        /// </summary>
        /// <Remark>
        /// Added by lihuijun
        /// </Remark>
        ProbeCaliRefSigUnInput,

        /// <summary>
        /// 探头校准参考信号移除(偏置校准时)
        /// </summary>
        /// <Remark>
        /// Added by xulintao
        /// </Remark>
        ProbeCaliRefSigRemove,

        /// <summary>
        /// 探头校准不能有信号输入(偏置校准时)
        /// </summary>
        /// <Remark>
        /// Added by lihuijun
        /// </Remark>
        ProbeCaliCannotInput,

        /// <summary>
        /// 探头校准检测到噪声过大(偏置校准时)
        /// </summary>
        /// <Remark>
        /// Added by xulintao
        /// </Remark>
        ProbeCaliNoiseChecked,

        /// <summary>
        /// 搜索项达到最大值
        /// </summary>
        SearchItemsMaximum,
        /// <summary>
        /// 快采模式下采集模式不能为平均或者包络
        /// </summary>
        /// <Remark>
        /// Added by luhao
        /// </Remark>
        AcqModeCannotSet,
        JitterSourceClose,
        AuxOutputSetPassFail,
        PowerAnalysisInrushCurrent,
        PowerAnalysisTurnOnTime,
        PowerAnalysisTurnOffTime,
        PowerAnalysisCloseSourceAndClickNext,
        PowerAnalysisOpenSourceAndClickNext,
        PowerAnalysisNoSignal,
        PowerAnalysisWaitForTriggerReady,
        PowerAnalysisWaitForTriggerCompleted,
        AC2DCTurnOnCompleted,
        AC2DCTurnOffCompleted,
        DC2DCTurnOnCompleted,
        DC2DCTurnOffCompleted,

        /// <summary>
        /// 打开触屏功能
        /// </summary>
        EnableTouch,

        /// <summary>
        /// 关闭触屏功能
        /// </summary>
        DisableTouch,

        //临时使用
        TempUse,
		
		 /// <summary>
        /// 清除所有测量项
        /// </summary>
        DeleteMeasureItems,
        /// <summary>
        /// 垂直档位不支持所设带宽 ljw 25.4
        /// </summary>
        VerticalLvNotSupportThisSetBandwidth,

        ExCaptureWfmStudy,
        ExCaptureWfmStudyExcute,
        ExCaptureWfmStudySuccess,
        ExCaptureWfmStudyFail
    }

    public static class EnumOperateMethod
    {
        public static String GetEnumFullName(Enum tip)
        {
            return tip.GetType().Name + "." + tip.ToString();
        }
    }
}
