using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public partial class InstrumentInteract
    {
        public static List<ushort[]>? Factory_WaveData_Adc(IInstrumentSession? instrument, int maxWaitMs, CancellationToken? token = null, int _needrecvedByteCount = 28000 * 4 * sizeof(ushort), int ACQ_BOARD_NUM = 2, int AdcNum = 2)
        {
            if (instrument == null)
                return null;
            int needrecvedByteCount = sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount * ServerDomainConstants.AdcCount;
            if (ServerDomainConstants.ProductType == ProductType.B21_DBI16G || ServerDomainConstants.ProductType == ProductType.B21_DBI20G)
            {
                //DBI项目，通道数与子带数是两个概念，在此模式下，只能是4个子带。
                needrecvedByteCount = 4 * ServerDomainConstants.PerAnaChannelAdcCount * ServerDomainConstants.PerAdcCoreCount * sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount;
            }
            else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X || ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000A)
            {
                needrecvedByteCount = _needrecvedByteCount;
            }
            else if ((int)ServerDomainConstants.ProductType == (int)ProductType.JiHe_MSO8000X)
            {
                needrecvedByteCount = _needrecvedByteCount;
            }

            byte[] recvedByteData = new byte[needrecvedByteCount];
            int recvedBytes = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (recvedBytes < needrecvedByteCount && stopwatch.ElapsedMilliseconds < (maxWaitMs + 100))
            {
                try
                {
                    token?.ThrowIfCancellationRequested();
                }
                catch
                {
                    return null;
                }
                string scpiCmd = GetCmdStr(ScpiCmd.Factory_WaveData_Adc);
                scpiCmd += " ?";
                if (!instrument.WriteString(scpiCmd))
                    return null;
                Thread.Sleep(20);

                recvedBytes = instrument.ReadBinData(ref recvedByteData);
                if (recvedBytes < recvedByteData.Length)
                    continue;
                List<ushort[]> returnData = new List<ushort[]>();
                int posIndex = 0;
                if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X || ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000A)
                {
                    for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                    {
                        ushort[] coreData = new ushort[needrecvedByteCount / sizeof(ushort) / ServerDomainConstants.PerAdcCoreCount];
                        for (int i = 0; i < coreData.Length; i++)
                        {
                            coreData[i] = BitConverter.ToUInt16(recvedByteData, posIndex);
                            posIndex += sizeof(ushort);
                        }
                        returnData.Add(coreData);
                    }
                }
                else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X)
                {
                    for (int boardId = 0; boardId < ACQ_BOARD_NUM; boardId++)
                    {
                        for (int adcId = 0; adcId < AdcNum; adcId++)
                        {
                            //按adc来分
                            int allAdcSum = ACQ_BOARD_NUM * AdcNum;
                            ushort[] adcData = new ushort[needrecvedByteCount / sizeof(ushort) / allAdcSum];
                            for (int i = 0; i < adcData.Length; i++)
                            {
                                adcData[i] = BitConverter.ToUInt16(recvedByteData, posIndex);
                                posIndex += sizeof(ushort);
                            }
                            returnData.Add(adcData);
                        }
                    }
                }
                else
                {
                    for (int adcIndex = 0; adcIndex < ServerDomainConstants.PerAnaChannelAdcCount * ServerDomainConstants.AnalogChannelCount; adcIndex++)
                    {
                        for (int coreIndex = 0; coreIndex < ServerDomainConstants.PerAdcCoreCount; coreIndex++)
                        {
                            ushort[] coreData = new ushort[ServerDomainConstants.PerAdcCoreDataCount];
                            for (int i = 0; i < ServerDomainConstants.PerAdcCoreDataCount; i++)
                            {
                                coreData[i] = BitConverter.ToUInt16(recvedByteData, posIndex);
                                posIndex += sizeof(ushort);
                            }
                            returnData.Add(coreData);
                        }
                    }
                }
                return returnData;
            }
            stopwatch.Stop();
            return null;
        }

        public static List<UInt16>? Factory_WaveData_Adc(IInstrumentSession? instrument, int maxWaitMs, ChannelId chnlId, Int32 adcId, Int32 dataLen, CancellationToken? token = null)
        {
            if (instrument == null)
                return null;

            Int32 needrecvedByteCount = dataLen * sizeof(UInt16);
            byte[] recvedByteData = new byte[needrecvedByteCount];

            int recvedBytes = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (recvedBytes < needrecvedByteCount && stopwatch.ElapsedMilliseconds < (maxWaitMs + 100))
            {
                if (token?.IsCancellationRequested ?? false)
                    return null;

                string scpiCmd = GetCmdStr(ScpiCmd.Factory_WaveData_Adc);
                scpiCmd += $" ? {chnlId},{adcId},{dataLen}";
                if (!instrument.WriteString(scpiCmd))
                    return null;

                Thread.Sleep(20);

                recvedBytes = instrument.ReadBinData(ref recvedByteData);

                List<UInt16> returnData = new List<UInt16>();
                Int32 posid = 0;
                for (Int32 dotid = 0; dotid < dataLen; dotid++)
                {
                    returnData.Add(BitConverter.ToUInt16(recvedByteData, posid));
                    posid += sizeof(UInt16);
                }
                return returnData;
            }
            stopwatch.Stop();

            return null;
        }

        public static List<UInt16>? Factory_WaveData_Adc(IInstrumentSession? instrument, int maxWaitMs, String dataName, CancellationToken? token = null)
        {
            if (instrument == null) return null;

            Int32 bytesize = 0;
            Boolean getbytesizeflag = Factory_WaveData_ByteSize(instrument, dataName, ref bytesize, token);
            if (!getbytesizeflag) return null;

            string scpiCmd = GetCmdStr(ScpiCmd.Factory_WaveData_Adc);
            scpiCmd += $" ? {dataName}";
            if (!instrument.WriteString(scpiCmd))
                return null;

            Thread.Sleep(20);

            byte[] recvedByteData = new byte[bytesize];
            int recvedBytes = 0;
            List<UInt16> returnData = new List<UInt16>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (recvedBytes < bytesize && stopwatch.ElapsedMilliseconds < (maxWaitMs + 100))
            {
                if (token?.IsCancellationRequested ?? false)
                    return null;

                recvedBytes = instrument.ReadBinData(ref recvedByteData);

                for (Int32 posid = 0; posid < recvedBytes; posid += sizeof(UInt16))
                {
                    returnData.Add(BitConverter.ToUInt16(recvedByteData, posid));
                }
            }
            stopwatch.Stop();

            return returnData;
        }

        private static Boolean Factory_WaveData_ByteSize(IInstrumentSession? instrument, String dataName, ref Int32 byteSize, CancellationToken? token = null)
        {
            if (instrument == null) return false;

            string cmdStr = GetCmdStr(ScpiCmd.Factory_WaveData_ByteSize) + " ?" + dataName;
            instrument.WriteString(cmdStr);
            Thread.Sleep(100);

            byte[] data = new byte[_ByteSizeBuffLen];
            int readedDataBytes = instrument.ReadBinData(ref data);

            String datastr = Encoding.ASCII.GetString(data.Take(readedDataBytes).ToArray());
            Boolean flag = Int32.TryParse(datastr, out Int32 bytesize);
            if (flag)
            {
                byteSize = bytesize;
            }

            return flag;
        }

        public static List<Byte[]>? DoFactory_WaveData_LA(IInstrumentSession? instrument, int maxWaitMs, CancellationToken? token = null)
        {
            if (instrument == null)
                return null;
            Int32 recvedbytes = 0;
            Int32 needrecvedbytecount = ServerDomainConstants.ProductType switch
            {
                ProductType.JiHe_MSO8000HD => sizeof(UInt16) * 50_000,
                ProductType.JiHe_MSO8000X => sizeof(UInt16) * 50_000,
                _ => sizeof(UInt16) * 25_000,
            } ;
            Byte[] recvedbytedata = new Byte[needrecvedbytecount];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (recvedbytes < needrecvedbytecount && stopwatch.ElapsedMilliseconds < (maxWaitMs + 100))
            {
                try
                {
                    token?.ThrowIfCancellationRequested();
                }
                catch
                {
                    return null;
                }
                String scpicmd = GetCmdStr(ScpiCmd.Factory_WaveData_LA);
                scpicmd += " ?";
                if (!instrument.WriteString(scpicmd))
                    return null;

                recvedbytes = instrument.ReadBinData(ref recvedbytedata);
                if (recvedbytes < recvedbytedata.Length)
                    continue;

                //*数据解析
                List<Byte[]> returndata = new List<Byte[]>();
                List<Byte>[] chnldatas = new List<Byte>[16];    //todo:数字通道固定16个

                for (Int32 i = 0; i < recvedbytedata.Length / 2; i++)
                {
                    var pointdata = BitConverter.ToUInt16(recvedbytedata, i * 2);
                    //一个ushort值包含了所有通道的一个点的值
                    for (Int32 chnlId = 0; chnlId < chnldatas.Length; chnlId++)
                    {
                        if (chnldatas[chnlId] == null)
                            chnldatas[chnlId] = new List<Byte>();

                        //获取对应通道的值
                        Int32 bitPos = 15 - chnlId;
                        Byte value = (Byte)((pointdata >> bitPos) % 2);
                        chnldatas[chnlId].Add(value);

                        //数据取完时，赋值到输出
                        if (i == recvedbytedata.Length / 2 - 1)
                            returndata.Add(chnldatas[chnlId].ToArray());
                    }
                }
                return returndata;
            }
            stopwatch.Stop();
            return null;
        }


        private static List<UInt16[]>? DoFactory_WaveData_Channel(IInstrumentSession? instrument, Int32 maxWaitMs, Int32 channelCount, CancellationToken? token = null)
        {
            if (instrument == null)
                return null;
            Int32 recvedbytes = 0;
            Int32 needrecvedbytevount = channelCount * sizeof(UInt16) * ServerDomainConstants.PerAnaChannelDataCount;
            //Boolean getbytesizeflag = Factory_WaveData_ByteSize(instrument, dataName, ref needrecvedByteCount, token);
            byte[] recvedbytedata = new byte[needrecvedbytevount];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (recvedbytes < needrecvedbytevount && stopwatch.ElapsedMilliseconds < (maxWaitMs + 100))
            {
                try
                {
                    token?.ThrowIfCancellationRequested();
                }
                catch
                {
                    return null;
                }
                String scpicmd = GetCmdStr(ScpiCmd.Factory_WaveData_Channel);
                scpicmd += " ?";
                if (!instrument.WriteString(scpicmd))
                    return null;
                Thread.Sleep(100);
                recvedbytes = instrument.ReadBinData(ref recvedbytedata);
                if (recvedbytes < recvedbytedata.Length)
                    continue;

                List<UInt16[]> returnData = new List<UInt16[]>();
                Int32 posIndex = 0;
                for (Int32 channelIndex = 0; channelIndex < channelCount; channelIndex++)
                {
                    UInt16[] perChannelData = new UInt16[ServerDomainConstants.PerAnaChannelDataCount];
                    for (Int32 i = 0; i < ServerDomainConstants.PerAnaChannelDataCount; i++)
                    {
                        perChannelData[i] = BitConverter.ToUInt16(recvedbytedata, posIndex);
                        posIndex += 2;
                    }
                    returnData.Add(perChannelData);
                }
                return returnData;
            }
            stopwatch.Stop();
            return null;
        }
        public static List<ushort[]>? Factory_WaveData_Channel(IInstrumentSession? instrument, int maxWaitMs, int channelCount, CancellationToken? token = null)
        {
            return DoFactory_WaveData_Channel(instrument, maxWaitMs, channelCount, token);
        }
        public static List<ushort[]>? Factory_WaveData_Channel(IInstrumentSession? instrument, int maxWaitMs, CancellationToken? token = null)
        {
            return DoFactory_WaveData_Channel(instrument, maxWaitMs, ServerDomainConstants.AnalogChannelCount, token);
        }
        public static List<AdcRegisterDataFormat>? Factory_Cali_Specail_ReadbackAdcRegisterValue(IInstrumentSession instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_Cali_Special_ReadbackAdcRegisterValue);
            scpiCmd += " ?";
            instrument.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readback = instrument.ReadString();
            if (readback.Trim() != "")
                return System.Text.Json.JsonSerializer.Deserialize<List<AdcRegisterDataFormat>>(readback);
            else
                return null;
        }
        public static string Factory_ReadbackFPGAVersionInfo(IInstrumentSession? instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_FPGAVersionInfo);
            scpiCmd += " ?";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(200);
            return instrument.ReadString();
        }
        public static string Factory_ReadbackFPGAWriteVersionInfo(IInstrumentSession? instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_FPGAWriteVersionInfo);
            scpiCmd += " ?";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(200);
            return instrument.ReadString();
        }
        public static string Factory_ReadbackFPGAWritedRegisterValue(IInstrumentSession? instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_FPGAAllWriteRegisterValueReadback);
            scpiCmd += " ?";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string result = instrument.ReadString();
            return result;
        }

        public static string Factory_ReadbackFPGAVersionInfoAtFlash(IInstrumentSession? instrument)
        {
            //需要比较长的读写时间，避免与正常采集的读写冲突，先停止采集
            string scpiCmd = "";
            scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? FpgaVersionInfoAtFlashIsReadback";
            instrument!.WriteString(scpiCmd);
            string result = instrument.ReadString();
            if (result != "1")
            {
                scpiCmd = ":STOP";
                instrument!.WriteString(scpiCmd);
                Thread.Sleep(500);

                scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
                scpiCmd += " ReadbackAllFPGAVersionInfoAtFlash";
                instrument!.WriteString(scpiCmd);
                Thread.Sleep(10_000);

                scpiCmd = ":RUN";
                instrument!.WriteString(scpiCmd);
            }
            scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? FpgaVersionInfoAtFlash";
            instrument!.WriteString(scpiCmd);
            result = instrument.ReadString();
            return result;
        }
        public static ServersTestLogicValue Factory_ReadbackLogicValue(IInstrumentSession? instrument)
        {
            string logicValueScpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_CaliLogicValue) + " ?";
            instrument!.WriteString(logicValueScpiCmd);
            string readBack = instrument.ReadString();
            string[] pairs = readBack.Split(',');
            ServersTestLogicValue serversTestLogicValue = new ServersTestLogicValue();
            foreach (string s in pairs)
            {
                string[] pair = s.Split(":");
                _ = (pair[0]) switch
                {
                    "DigitTrigger" => serversTestLogicValue.DigitTrigger = pair[1] == "on",
                    "AdcTestMode" => serversTestLogicValue.AdcTestMode = pair[1] == "on",
                    "AdcFlashMode" => serversTestLogicValue.bAdcAtFlashMode = pair[1] == "on",
                    "AdjustGainByTemperature" => serversTestLogicValue.bAdjustGainByTemperature = pair[1] == "on",
                    "bForceReFind5200AdcSyncWindow" => serversTestLogicValue.bForceReFind5200AdcSyncWindow = pair[1] == "on",
                    "bAcqStatisticsRunning" => serversTestLogicValue.bAcqStatisticsRunning = pair[1] == "on",
                    "bAFCOpened" => serversTestLogicValue.bAFCOpened = pair[1] == "on",
                    "bDBIOpened" => serversTestLogicValue.bDBIOpened = pair[1] == "on",
                    "bTiAdcOpened" => serversTestLogicValue.bTiAdcOpened = pair[1] == "on",
                    _ => serversTestLogicValue.bNoUsed = false,
                };
            }
            return serversTestLogicValue;
        }
        public static bool Factory_ReadbackSoftLAIsFull(IInstrumentSession? instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? GetSoftLA_FifoFull";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readBack = instrument.ReadString();
            return readBack switch
            {
                "true" or "TRUE" or "1" => true,
                _ => false,
            };
        }
        public static int Factory_ReadbackSoftLACaptureData(IInstrumentSession? instrument, ref byte[] readBackBytes)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_TakeSpecialBinData);
            scpiCmd += " ? TakeFPGASoftLACaptureBinData";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            byte[] binData = new byte[16 * 1024];
            return instrument.ReadBinData(ref readBackBytes);
        }

        public static string Factory_ReadbackProductInfoAtFlash(IInstrumentSession? instrument)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? ProductInfo";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(3000);
            string result = instrument.ReadString();
            return result;
        }
        public static void Factory_WriteProductInfoAtFlash(IInstrumentSession? instrument, string productInfo)
        {
            string scpiCmd = GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ProductInfo," + productInfo;
            instrument!.WriteString(scpiCmd);
        }
        public static bool Factory_GetScreenJpeg(IInstrumentSession? instrument, string pathAndFileName)
        {
            string scpiCmd = ":FACT:SCR:JPG?";
            instrument!.WriteString(scpiCmd);
            string result = instrument.ReadString();
            if (result != "")
            {
                File.WriteAllBytes(pathAndFileName, Convert.FromBase64String(result));
                return false;
            }
            return false;
        }
    }
    public class ServersTestLogicValue
    {
        public bool DigitTrigger = false;
        public bool AdcTestMode = false;
        public bool bAdcAtFlashMode = false;
        public bool bAdjustGainByTemperature = false;
        public bool UsingMatlab = false;
        public bool bForceReFind5200AdcSyncWindow = false;
        public bool bAcqStatisticsRunning = false;
        public bool bAFCOpened = false;
        public bool bTiAdcOpened = false;
        public bool bDBIOpened = false;
        public bool bNoUsed = false;
    }
}
