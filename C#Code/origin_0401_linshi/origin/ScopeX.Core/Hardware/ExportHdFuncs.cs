using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

//[assembly: InternalsVisibleToAttribute("Friend2")]
namespace ScopeX.Core.Hardware
{

    public class ExportHdFuncs
    {
        public static Boolean FactoryCaliScpiProc_LogicValue_Set(String param)
        {
            HdSpecial.FactoryCaliScpiProc_LogicValue_Set(param);
            Hardware.HdCmdFactory.Push(HdCmd.TmbPosition | HdCmd.TmbScaleIndex | HdCmd.TmbStorageLen);
            return true;
        }
        public static String FactoryCaliScpiProc_LogicValue_Get()
        {
            return HdSpecial.FactoryCaliScpiProc_LogicValue_Get();
        }
        public static Boolean FactoryCaliScpiProc_SpecialData_Set(String param)
        {
            HdCmd cmd = HdSpecial.FactoryCaliScpiProc_SpecialData_Set(param);
            if (cmd != HdCmd.None)
                Hardware.HdCmdFactory.Push(cmd);
            return true;
        }
        public static String FactoryCaliScpiProc_SpecialData_Get(String param)
        {
            return HdSpecial.FactoryCaliScpiProc_SpecialData_Get(param);
        }

        /// <summary>
        /// 获取所有硬件板的温度信息
        /// </summary>
        /// <returns></returns>
        public static String GetAllBoardTemperature()
        {
            return HdSpecial.FactoryCaliScpiProc_SpecialData_Get("GetAllBoardTemperature");
        }

        /// <summary>
        /// 获取示波器的所有版本信息
        /// </summary>
        /// <returns></returns>
        public static String GetSoftWareVersion()
        {
            return $"Software:{DsoPrsnt.DefaultDsoPrsnt.SoftWareVersion}";
        }

        public static String GetAllFPGAVersionInfo()
        {
            return HdSpecial.GetAllFPGAVersionInfo();
        }
        public static Dictionary<HardwareVersionItem, HardwareVersionInfo?> TryTakeHardwareVersionInfo()
        {
            return Hd.ProductConfig.TryTakeHardwareVersionInfo();
        }
        public static String GetAllFPGAWriteVersionInfo()
        {
            return HdSpecial.GetAllFPGAWriteVersionInfo();
        }
        public static String ReadbackAdcRegisterData()
        {
            return HdSpecial.FactoryCaliScpiProc_ReadbackAdcRegisterValue();
        }
        public static Boolean TakeChannelWaveform(out List<List<UInt16>> waveData)
        {
            return TryGetAllChannelWaveData(out waveData);
        }
        public static Boolean TakeAdcWaveform(out List<List<UInt16>> waveData)
        {
            return TryGetAllAdcWaveDataForScpi(out waveData);
        }
        public static Boolean TakeLAWaveform(out List<UInt16> waveData)
        {
            return TryGetAllLAWaveDataForScpi(out waveData);
        }
        public static String ReadbackAllWritedRegisterValue()
        {
            return HdSpecial.FactoryCaliScpiProc_ReadbackAllDspRegister();
        }

        public static Boolean FPGARegister_WriteValue(UInt32 addr, UInt32 data, Boolean bIsAcq)
        {
            HdSpecial.Test_WriteRegister(addr, data, bIsAcq);
            return true;
        }

        public static Boolean TakeSpecialBinData(String param, out Byte[] binData)
        {
            return HdSpecial.TakeSpecialBinData(param, out binData);
        }
        #region Scpi Special
        private static Boolean _BNeedReadAllChannelWaveDataForScpi = false;
        private static List<List<UInt16>> _WaveDataOfAllChannelForScpi = new List<List<UInt16>>();
        private static Boolean TryGetAllChannelWaveData(out List<List<UInt16>> waveData)
        {
            System.Threading.Volatile.Write(ref _BNeedReadAllChannelWaveDataForScpi, true);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_BNeedReadAllChannelWaveDataForScpi && sw.ElapsedMilliseconds < 500)
                Thread.Sleep(2);
            waveData = _WaveDataOfAllChannelForScpi;
            if (_BNeedReadAllChannelWaveDataForScpi)
                return false;
            else
                return true;
        }
        private static Boolean _BNeedReadAllAdcWaveDataForSCPI = false;
        private static List<List<UInt16>> _WaveDataAllAdcForSCPI = new List<List<UInt16>>();
        private static Boolean TryGetAllAdcWaveDataForScpi(out List<List<UInt16>> waveData)
        {
            Volatile.Write(ref _BNeedReadAllAdcWaveDataForSCPI, true);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_BNeedReadAllAdcWaveDataForSCPI && sw.ElapsedMilliseconds < 500)
                Thread.Sleep(2);
            waveData = _WaveDataAllAdcForSCPI;
            if (_BNeedReadAllAdcWaveDataForSCPI)
                return false;
            else
                return true;
        }
        private static Boolean _BNeedReadAllLAWaveDataForSCPI = false;
        private static List<UInt16> _WaveDataAllLAForSCPI = new List<UInt16>();
        private static Boolean TryGetAllLAWaveDataForScpi(out List<UInt16> waveData)
        {
            Volatile.Write(ref _BNeedReadAllLAWaveDataForSCPI, true);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (_BNeedReadAllLAWaveDataForSCPI && sw.ElapsedMilliseconds < 500)
                Thread.Sleep(2);
            waveData = _WaveDataAllLAForSCPI;
            if (_BNeedReadAllLAWaveDataForSCPI)
                return false;
            else
                return true;
        }

        public static Boolean TryGetWaveformByteSize(String waveName, ref Int32 byteSize)
        {
            var info = Hd.TryGetData(ChannelType.Analog, $"{AnalogParamEnum.WaveByteSize}_{waveName}", out Object? data);
            if (data != null && data is Int32)
            {
                byteSize = (Int32)data;
            }
            return true;
        }

        public static List<UInt16>? TryGetAdcWaveform(String waveName)
        {
            var info = Hd.TryGetData(ChannelType.Analog, $"{AnalogParamEnum.AdcWaveData}_{waveName}", out Object? data);
            if (data != null && data is List<UInt16>)
            {
                return (List<UInt16>)data;
            }
            return null;
        }

        public static void ProcessScpiSpecial()
        {
            if (_BNeedReadAllChannelWaveDataForScpi && Hd.AnalogChannel!.TakeAllChannelWaveform(out _WaveDataOfAllChannelForScpi))
                Volatile.Write(ref _BNeedReadAllChannelWaveDataForScpi, false);
            if (_BNeedReadAllAdcWaveDataForSCPI && Hd.AnalogChannel!.TakeAdcWaveform(out _WaveDataAllAdcForSCPI))
                Volatile.Write(ref _BNeedReadAllAdcWaveDataForSCPI, false);
            if (_BNeedReadAllLAWaveDataForSCPI && Hd.LA!.TakeAllChannelWaveform(out _WaveDataAllLAForSCPI))
                Volatile.Write(ref _BNeedReadAllLAWaveDataForSCPI, false);
        }

        public static void SetUpdateFlashCaliDataFlag(Boolean value)
        {
            Hd.UpdateFlashCaliDataFlag = value;
        }

        public static Boolean GetUpdateFlashCaliDataFlag => Hd.UpdateFlashCaliDataFlag;

        public static void SetReadFlashCaliDataFlag(Boolean value)
        {
            Hd.ReadlashCaliDataFlag = value;
        }

        public static Boolean GetReadFlashCaliDataFlag => Hd.ReadlashCaliDataFlag;

        internal static UInt32 FlashCaliType { get; set; }

        public static UInt32 SetFlashCaliType(UInt32 type) => FlashCaliType = type;
        public static UInt32 GetFlashCaliType() => FlashCaliType;

        #endregion
        #region

        public static Boolean TryTakeSourceData(ChannelId channelId, ReadInfo readInfo, out List<UInt16> wavedata, CancellationToken? softResetToken)
        {
            var ans = Hd.AnalogChannel!.TryTakeSourceWave(channelId, readInfo, out wavedata, softResetToken);
            return ans;
        }

        public static Boolean TryReadSourceData(ChannelId channelId, Int32 dots, Double startDotIndex, Int32 frameIndex, out UInt16[] waveData, WfmSampleInfo wfmSampleInfo, CancellationToken? softResetToken)
        {
            Boolean ans = true;
            wfmSampleInfo = new();
            WfmPkgInfo wfmPkgInfo = new(dots, Double.MaxValue, startDotIndex);
            ReadInfo requstReadSourceData_ReadInfo = new(AcqDataType.AnalogChannel, new() { channelId }, wfmPkgInfo, nameof(DataRole.SourceData));
            ans = Hd.AnalogChannel!.TryTakeSegmentWave(channelId, requstReadSourceData_ReadInfo!, frameIndex, 1, out UInt16[,] waveData_source, out wfmSampleInfo, out var secondByps, softResetToken, true);
            if (!ans)
            {
                ans = Hd.AnalogChannel!.TryTakeSegmentWave(channelId, requstReadSourceData_ReadInfo!, frameIndex, 1, out UInt16[,] waveData_source1, out wfmSampleInfo, out var secondByps1, softResetToken, true);
                waveData_source = waveData_source1;
            }
            if (ans)
            {
                waveData = new UInt16[waveData_source.GetLength(1)];
                //if (waveData_source.GetLength(1) != 2000)
                //  ;
                //Debug.WriteLine(waveData.Length);
                Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref waveData[0]), ref Unsafe.As<UInt16, Byte>(ref waveData_source[(Int32)0, 0]), (UInt32)(Unsafe.SizeOf<UInt16>() * waveData_source.GetLength(1)));
                return true;
            }
            else
                waveData = new UInt16[0];
            return ans;
        }
        #endregion

        public static void ConfigLed(ErrorType type)
        {
            Hd.AnalogChannel?.ConfigLed(type);
        }
        public static void CloseAllLed()
        {
            Hd.AnalogChannel?.CloseAllLed();
        }
    }
}
