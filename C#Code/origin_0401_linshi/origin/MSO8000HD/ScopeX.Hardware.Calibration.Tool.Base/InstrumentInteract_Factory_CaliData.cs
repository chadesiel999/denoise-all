using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public partial class InstrumentInteract
    {
        private enum CaliDataOperateType
        {
            GetSet,
            LoadFromFile,
            SaveToFile,
            ByteSize,
            SaveToFlash,
            LoadFromFlash,
        }
        private static string GetCaliDataCmd(CaliDataType caliDataType, CaliDataOperateType operateType)
        {
            string cmdStr = operateType switch
            {
                CaliDataOperateType.GetSet => GetCmdStr(ScpiCmd.Factory_CaliData_GetSet),
                CaliDataOperateType.LoadFromFile => GetCmdStr(ScpiCmd.Factory_CaliData_LoadFromFile),
                CaliDataOperateType.LoadFromFlash => GetCmdStr(ScpiCmd.Factory_CaliData_LoadFromFlash),
                CaliDataOperateType.SaveToFile => GetCmdStr(ScpiCmd.Factory_CaliData_SaveToFile),
                CaliDataOperateType.SaveToFlash => GetCmdStr(ScpiCmd.Factory_FlashCaliData_Save),
                CaliDataOperateType.ByteSize => GetCmdStr(ScpiCmd.Factory_CaliData_ByteSize),
                _ => ""
            };

            cmdStr = cmdStr.Insert(cmdStr.LastIndexOf(':'), ((int)caliDataType).ToString());
            if (operateType == CaliDataOperateType.SaveToFlash || operateType == CaliDataOperateType.LoadFromFlash)
            {
                cmdStr += " 1";
            }
            return cmdStr;
        }
        private const Int32 _ByteSizeBuffLen = 1024;
        private static Boolean CaliData_ByteSizeGet(IInstrumentSession instrument, CaliDataType caliDataType)
        {
            ICaliData? iCaliData = Helper.GetICaliData(caliDataType);
            if (iCaliData == null) return false;

            string cmdStr = GetCaliDataCmd(caliDataType, CaliDataOperateType.ByteSize) + " ?";
            instrument.WriteString(cmdStr);
            Thread.Sleep(100);

            byte[] data = new byte[_ByteSizeBuffLen];
            int readedDataBytes = instrument.ReadBinData(ref data);

            var datastr = Encoding.ASCII.GetString(data.Take(readedDataBytes).ToArray());
            Boolean flag = Int32.TryParse(datastr, out var bytesize);
            if (flag)
            {
                iCaliData.OriginTotleBytes = bytesize;
            }

            return flag;
        }

        public static bool CaliData_Get(IInstrumentSession instrument, CaliDataType caliDataType)
        {
            if (instrument == null) return false;

            ICaliData? iCaliData = Helper.GetICaliData(caliDataType);
            if (iCaliData == null) return false;

            Boolean getbytesizeflag = CaliData_ByteSizeGet(instrument, caliDataType);
            if (!getbytesizeflag) return false;

            string cmdStr = GetCaliDataCmd(caliDataType, CaliDataOperateType.GetSet) + " ?";
            instrument.WriteString(cmdStr);
            Thread.Sleep(100);

            byte[] data = new byte[iCaliData.OriginTotleBytes];
            int readedDataBytes = instrument.ReadBinData(ref data);

            if (readedDataBytes < iCaliData.OriginTotleBytes)
                return false;
            iCaliData.Deserialize(data);

            return true;
        }

        public static bool CaliData_Send(IInstrumentSession? instrument, CaliDataType caliDataType)
        {
            if (instrument == null)
                return false;
            ICaliData? iCaliData = Helper.GetICaliData(caliDataType);
            if (iCaliData == null)
                return false;
            //传输到远端
            byte[] caliData = iCaliData.Serialize();
            string cmdStr = GetCaliDataCmd(caliDataType, CaliDataOperateType.GetSet) + " ";
            return instrument.WriteBinDataWithMultiPackage(cmdStr, caliData, caliData.Length);
        }

        public static bool CaliData_LoadFromFile(IInstrumentSession? instrument, CaliDataType caliDataType)
        {
            if (instrument == null)
                return false;
            String cmdstr = GetCaliDataCmd(caliDataType, CaliDataOperateType.LoadFromFile);
            //switch (caliDataType)
            //{
            //    case CaliDataType.Misc:
            //    case CaliDataType.CoefficientsTables://7000X
            //    case CaliDataType.AWG:
            //    case CaliDataType.AnalogParams:
            //    case CaliDataType.CoefficientsParams:
            //        if (CheckLoadStatus(instrument))
            //        {
            //            cmdstr = GetCaliDataCmd(caliDataType, CaliDataOperateType.LoadFromFlash);
            //            if (!instrument.WriteString(cmdstr) && !CheckLoadStatus(instrument))
            //                return false;
            //        }
            //        else
            //        {
            //            return false;
            //        }

            //        break;
            //    default:
            //        if (!instrument.WriteString(cmdstr))
            //            return false;
            //        break;
            //}
            //Thread.Sleep(100);

            return CaliData_Get(instrument, caliDataType);
        }
        private static Boolean CheckLoadStatus(IInstrumentSession? instrument)
        {
            Int32 timeout = 0;
            Boolean operateflash = true;
            if (instrument != null)
            {
                while (operateflash && timeout < 15)
                {
                    String readflashcmd = ":FACT:FLASh:READ ?";
                    instrument.WriteString(readflashcmd);
                    String result = instrument.ReadString();
                    if (result != null && result.Contains(","))
                    {
                        operateflash = result.Split(',')[0] == "1" ? true : false;
                    }
                    Thread.Sleep(1000);
                    timeout++;
                }
            }
            return !operateflash;
        }
        public static bool CaliData_SaveToFile(IInstrumentSession? instrument, CaliDataType caliDataType)
        {
            if (instrument == null)
                return false;

            Boolean sendflag = CaliData_Send(instrument, caliDataType);
            if (!sendflag) return false;
            
            Thread.Sleep(100);
            //发送保存命令
            String cmdStr = GetCaliDataCmd(caliDataType,CaliDataOperateType.SaveToFile);
            return instrument.WriteString(cmdStr);
        }
        public static bool CaliData_SaveData(IInstrumentSession? instrument, CaliDataType caliDataType)
        {
            if (instrument == null)
                return false;

            Boolean sendflag = CaliData_Send(instrument, caliDataType);
            if (!sendflag) return false;

            Thread.Sleep(100);
            //发送保存命令
            String cmdstr = GetCaliDataCmd(caliDataType, CaliDataOperateType.SaveToFile);
            CaliDataManager.SaveAllToFile();
            if (Directory.Exists("CaliData"))
            {
                String discalidatadic = CalibrationOscilloscopeInfo.Defalut.FileDir;
                foreach (String fileInfo in Directory.GetFiles("CaliData"))
                {
                    String sourcepath = AppDomain.CurrentDomain.BaseDirectory + fileInfo;
                    String disfilepath = discalidatadic + fileInfo;
                    String disdic = Path.GetDirectoryName(disfilepath);
                    if (disdic != null && !Directory.Exists(disdic))
                    {
                        Directory.CreateDirectory(disdic);
                    }
                    File.Copy(sourcepath, disfilepath, true);
                }
            }
            instrument.WriteString(cmdstr);
            //switch (caliDataType)
            //{
            //    case CaliDataType.Misc:
            //    case CaliDataType.CoefficientsTables://7000X
            //    case CaliDataType.AWG:
            //    case CaliDataType.AnalogParams:
            //    case CaliDataType.CoefficientsParams:
            //        if (CheckSaveStatus(instrument))
            //        {
            //            cmdstr = GetCaliDataCmd(caliDataType, CaliDataOperateType.SaveToFlash);
            //            if (instrument.WriteString(cmdstr))
            //            {
            //                return CheckSaveStatus(instrument);
            //            }
            //            else
            //            {
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //}
            return instrument.WriteString(cmdstr);
        }

        private static Boolean CheckSaveStatus(IInstrumentSession? instrument)
        {
            Boolean operateflash = true;
            Int32 timeOut = 0;
            if (instrument != null)
            {
                while (operateflash && timeOut < 15)
                {
                    String readflashcmd = ":FACT:FLASh:SAV ?";
                    instrument.WriteString(readflashcmd);
                    String result = instrument.ReadString();
                    if (result != null && result.Contains(","))
                    {
                        operateflash = result.Split(',')[0] == "1" ? true : false;
                    }
                    Thread.Sleep(1000);
                    timeOut++;
                }
            }
            return !operateflash;
        }

        public static bool DbiCoefficientsTable_SaveToFile(IInstrumentSession? instrument, DbiCoefficientsTablesType dbiCoefficientsTablesType, int currBandMode, int currChannelIndex, int currSubBandIndex, int currFilterBandMode)
        {
            if (instrument == null)
                return false;
            //传输到远端
            //step1:数据类型
            String cmdstr = GetCmdStr(ScpiCmd.Factory_SpecailData) + $" DbiCoefficientsTableChanged,{(int)dbiCoefficientsTablesType}";
            bool bOK = instrument.WriteString(cmdstr);
            if (!bOK)
                return false;
            //数据传输
            byte[] caliData = DbiCoefficientsTables.Default.Serialize(dbiCoefficientsTablesType);
            cmdstr = GetCaliDataCmd(CaliDataType.DbiCoefficientsTables, CaliDataOperateType.GetSet) + " ";
            bOK = instrument.WriteBinDataWithMultiPackage(cmdstr, caliData, caliData.Length);
            if (!bOK)
                return false;
            Thread.Sleep(100);
            //发送保存命令
            cmdstr = GetCaliDataCmd(CaliDataType.DbiCoefficientsTables, CaliDataOperateType.SaveToFile);
            bOK = instrument.WriteString(cmdstr);
            if (!bOK)
                return false;

            //发布生效命令
            Thread.Sleep(500);
            cmdstr = GetCmdStr(ScpiCmd.Factory_SpecailData) + $" DbiCoefficientsTables,{(int)dbiCoefficientsTablesType}_{currBandMode}_{currChannelIndex}_{currSubBandIndex}_{currFilterBandMode}";
            return instrument.WriteString(cmdstr);
        }
        public static bool DbiCoefficientsTable_GetFromServer(IInstrumentSession instrument, DbiCoefficientsTablesType dbiCoefficientsTablesType)
        {
            //传输过来
            string cmdStr = GetCmdStr(ScpiCmd.Factory_TakeSpecialBinData) + $" ? DbiCoefficientsTables,{(int)dbiCoefficientsTablesType}";
            instrument.WriteString(cmdStr);
            //数据处理
            Thread.Sleep(100);
            byte[] data = new byte[DbiCoefficientsTables.Default.TotalBytesOfType(dbiCoefficientsTablesType)];
            int readedDataBytes = instrument.ReadBinData(ref data);
            if (readedDataBytes == 0)
                return false;
            DbiCoefficientsTables.Default.Deserialize(dbiCoefficientsTablesType, data);
            return true;
        }
    }
}
