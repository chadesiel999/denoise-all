using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Hardware;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
using System.Configuration;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static readonly List<string> AnalogInputSource = new List<string>() { "BNC", "SMA" };

        private static string ConvertParamToString(SCPICommandProcessFuncParam analyResult)
        {
            StringBuilder sb = new StringBuilder();
            if (analyResult.Params != null)
            {
                bool bFirst = true;
                foreach (byte[] param in analyResult.Params)
                {
                    if (!bFirst)
                        sb.Append(",");
                    if (param != null)
                    {
                        foreach (byte b in param)
                        {
                            char c = (char)b;
                            if (c != ' ' && c != '\r' && c != '\n')
                                sb.Append(c);
                        }
                    }
                    bFirst = false;
                }
            }
            return sb.ToString();
        }
        private static void PushCaliDataChangedHdCmd(CaliDataType caliDataType)
        {
            lock (CaliDataManager.DataChangedCaliDataType)
            {
                if (!CaliDataManager.DataChangedCaliDataType.Contains(caliDataType))
                    CaliDataManager.DataChangedCaliDataType.Add(caliDataType);
            }
            CalibrationPrsnt.Push(HdCmd.CaliDataChanged);
        }
        public static bool scpiQuy_FPGA_Version(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.GetAllFPGAVersionInfo());

            return true;
        }
        public static bool scpiQuy_FPGA_WriteVersion(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.GetAllFPGAVersionInfo());

            return true;
        }
        public static bool scpiQuy_FPGA_AllWriteRegisterValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.ReadbackAllWritedRegisterValue());

            return true;
        }

        //public static bool TEST(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage) //测试0508
        //{
        //    sendMessage.IsDataBlock = false;
        //    double lo_addr_tmp2 = HdIO.ReadReg(ProcBdReg.R.Acq_Pro_Main_State);
        //    sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes("1");

        //    return true;
        //}
        public static bool scpiSet_SerialNumber(SCPICommandProcessFuncParam analyResult)
        {
            if (Presenter.OptionsManager != null && analyResult.Params != null && analyResult.Params.Count > 0)
            {
                Presenter.OptionsManager.SerialNumber = Encoding.UTF8.GetString(analyResult.Params[0]).Trim().Replace("\"", "");
            }
            return true;
        }
        public static bool scpiQuy_SerialNumber(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            if (Presenter.OptionsManager != null)
            {
                sendMessage.SendData = Encoding.UTF8.GetBytes(Presenter.OptionsManager.SerialNumber);
            }
            return true;
        }

        public static bool scpiQuy_Temperatures(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            sendMessage.SendData = Encoding.UTF8.GetBytes(ExportHdFuncs.GetAllBoardTemperature());
            return true;
        }

        public static bool scpiQuy_SoftWareVersion(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;
            sendMessage.SendData = Encoding.UTF8.GetBytes(ExportHdFuncs.GetSoftWareVersion());
            return true;
        }


        public static bool scpiSet_FPGA_WriteRegister(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            string[] groups = paramStr.Split('|');
            foreach (string group in groups)
            {
                string[] paramList = group.Split(',');
                if (paramList.Length < 4)
                    return false;
                if (!UInt32.TryParse(paramList[0], out UInt32 addr))
                    return false;
                if (!UInt32.TryParse(paramList[1], out UInt32 data))
                    return false;
                if (!Int32.TryParse(paramList[2], out Int32 DelayMs))
                    return false;
                bool bIsAcq = paramList[3] == "1" ? true : false;
                ExportHdFuncs.FPGARegister_WriteValue(addr, data, bIsAcq);
                if (DelayMs > 0)
                    Thread.Sleep(DelayMs);
            }
            return true;
        }

        #region 校准数据
        public static bool scpiQuy_CaliData_Get(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = true;
            sendMessage.SendData = CalibrationData.Helper.GetICaliData((CaliDataType)analyResult.ChannelIndex).Serialize();

            PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
            return true;
        }
        private static byte[] bigCaliData = new byte[16 * 1024 * 1024];
        private static CaliDataType lastCaliDataType = CaliDataType.None;
        private static int bigCaliDataIndex = 0;

        private static Dictionary<Int32, List<Byte>> CaliDatas = new Dictionary<Int32, List<Byte>>();
        private static Int16 ReceiveStatus;//接收数据状态 0.未接收过数据 1.接收数据正常，未停止接收数据 2.接收数据异常，未停止接收数据 3.完成接收数据

        public static bool scpiSet_CaliData_Set(SCPICommandProcessFuncParam analyResult)
        {
            PrintDebug(ConvertInputData(analyResult), null);
            if (lastCaliDataType == CaliDataType.None)
            {
                lastCaliDataType = (CaliDataType)analyResult.ChannelIndex;
                if (CaliDatas.ContainsKey(analyResult.ChannelIndex))
                {
                    CaliDatas[analyResult.ChannelIndex].Clear();
                }

            }
            if (analyResult.Params[0].Length == 0)
            {
                //File.WriteAllBytes($@"d:\recved.bin", bigCaliData);
                var datas = CaliDatas[analyResult.ChannelIndex].ToArray();
                CaliDataType caliDataType = (CaliDataType)analyResult.ChannelIndex;
                if (caliDataType != CaliDataType.DbiCoefficientsTables)
                {
                    CalibrationData.Helper.GetICaliData(caliDataType)?.Deserialize(datas);
                }
                else
                {
                    DbiCoefficientsTables.Default.Deserialize(CaliDataManager.LastChangedDataType, datas);
                }
                PushCaliDataChangedHdCmd(caliDataType);
                CaliDatas[analyResult.ChannelIndex].Clear();
                lastCaliDataType = CaliDataType.None;
                bigCaliDataIndex = 0;
            }
            else
            {
                byte[] data = analyResult.Params[0];// ConvertBinDataFromScpiData(analyResult.Params[0]);
                if (CaliDatas.ContainsKey(analyResult.ChannelIndex))
                {
                    CaliDatas[analyResult.ChannelIndex].AddRange(data);
                }
                else
                {
                    CaliDatas.Add(analyResult.ChannelIndex, data.ToList());
                }
            }
            return true;
        }

        public static bool scpiSet_CaliData_SaveToFile(SCPICommandProcessFuncParam analyResult)
        {
            PrintDebug(ConvertInputData(analyResult), null);
            CaliDataType caliDataType = (CaliDataType)analyResult.ChannelIndex;
            CalibrationData.Helper.GetICaliData(caliDataType).SaveToFile();
            CalibrationData.Helper.GetICaliData(CaliDataType.AutoCalibration)?.SaveToFile();
            //PushCaliDataChangedHdCmd(caliDataType);
            return true;
        }

        public static bool scpiSet_CaliData_LoadFromFile(SCPICommandProcessFuncParam analyResult)
        {
            PrintDebug(ConvertInputData(analyResult), null);
            CaliDataType caliDataType = (CaliDataType)analyResult.ChannelIndex;
            CalibrationData.Helper.GetICaliData(caliDataType).LoadFromFile();
            CalibrationData.Helper.GetICaliData(CaliDataType.AutoCalibration)?.LoadFromFile();
            PushCaliDataChangedHdCmd(caliDataType);
            return true;
        }
        public static bool scpiQuy_CaliData_GetByteSize(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = true;
            Int32 totlebytes = Helper.GetICaliData((CaliDataType)analyResult.ChannelIndex)?.TotalBytes ?? 0;
            sendMessage.SendData = decodeStr(totlebytes.ToString());

            PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
            return true;
        }

        public static bool scpiSet_CaliData_SetByteSize(SCPICommandProcessFuncParam analyResult)
        {
            PrintDebug(ConvertInputData(analyResult), new List<byte>());
            CaliDataType caliDataType = (CaliDataType)analyResult.ChannelIndex;
            ICaliData? icalidata = Helper.GetICaliData(caliDataType);
            if (analyResult.Params.Count < 1 || icalidata == null)
                return false;
            String bytesizestr = Encoding.ASCII.GetString(analyResult.Params[0]).Trim();
            //wangcj240806, 注意：此处用反射兼容7000 & 8000不同的ICaliData接口
            icalidata.GetType().GetProperty("OriginTotleBytes", System.Reflection.BindingFlags.Instance)?.SetValue(icalidata, Int32.Parse(bytesizestr));
            return true;
        }

        public static bool scpiSet_SingalCaliData_Set(SCPICommandProcessFuncParam analyResult)
        {
            PrintDebug(ConvertInputData(analyResult), null);

            byte[] datas = ConvertBinDataFromScpiData(analyResult.Params[0]);
            CaliDataType caliDataType = (CaliDataType)analyResult.ChannelIndex;
            if (caliDataType == CaliDataType.CoefficientsParams)
            {
                CoefficientsParams.Default.DeserializeTheOne(datas);
            }
            return true;
        }
        #endregion

        #region 波形数据

        public static Boolean scpiQuy_Factory_GetWaveData_Adc(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            String paramStr = ConvertParamToString(analyResult);
            if (String.IsNullOrEmpty(paramStr) == false)
            {
                var wavedata = ExportHdFuncs.TryGetAdcWaveform(paramStr);
                if (wavedata != null)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    for (Int32 dotid = 0; dotid < wavedata.Count; dotid++)
                    {
                        memoryStream.Write(BitConverter.GetBytes(wavedata[dotid]));
                    }
                    sendMessage.IsDataBlock = true;
                    sendMessage.SendData = memoryStream.ToArray();
                    memoryStream.Close();

                    PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
                    return true;
                }

                return false;
            }
            if (ExportHdFuncs.TakeAdcWaveform(out List<List<ushort>> waveData))
            {
                MemoryStream memoryStream = new MemoryStream();
                for (int coreIndex = 0; coreIndex < waveData.Count; coreIndex++)
                {
                    foreach (ushort s in waveData[coreIndex])
                        memoryStream.Write(BitConverter.GetBytes(s));
                }

                sendMessage.IsDataBlock = true;
                sendMessage.SendData = memoryStream.ToArray();
                memoryStream.Close();

                PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
                return true;
            }
            else
                return false;
        }
        public static bool scpiQuy_Factory_TakeSpecialBinData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string paramStr = ConvertParamToString(analyResult);
            if (ExportHdFuncs.TakeSpecialBinData(paramStr, out byte[] CapturedData))
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(CapturedData);

                sendMessage.IsDataBlock = true;
                sendMessage.SendData = memoryStream.ToArray();
                memoryStream.Close();
                return true;
            }
            else
                return false;
        }
        public static bool scpiQuy_Factory_GetWaveData_Channel(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (ExportHdFuncs.TakeChannelWaveform(out List<List<ushort>> waveData))
            {
                MemoryStream memoryStream = new MemoryStream();
                for (int channelIndex = 0; channelIndex < waveData.Count; channelIndex++)
                {
                    foreach (ushort s in waveData[channelIndex])
                        memoryStream.Write(BitConverter.GetBytes(s));
                }

                sendMessage.IsDataBlock = true;
                sendMessage.SendData = memoryStream.ToArray();
                memoryStream.Close();

                PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
                return true;
            }
            else
                return false;
        }

        public static bool scpiQuy_Factory_GetWaveData_LA(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (ExportHdFuncs.TakeLAWaveform(out List<ushort> waveData))
            {
                MemoryStream memoryStream = new MemoryStream();
                foreach (var pt in waveData)
                {
                    memoryStream.Write(BitConverter.GetBytes(pt));
                }
                sendMessage.IsDataBlock = true;
                sendMessage.SendData = memoryStream.ToArray();
                memoryStream.Close();

                PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
                return true;
            }
            else
                return false;
        }
        public static bool scpiQuy_Factory_GetWaveData_GetByteSize(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Params.Count < 1) return false;
            sendMessage.IsDataBlock = true;
            Int32 bytesize = 0;
            Boolean getbytesizeflag = ExportHdFuncs.TryGetWaveformByteSize(Encoding.ASCII.GetString(analyResult.Params[0]), ref bytesize);
            if (!getbytesizeflag) return false;

            sendMessage.SendData = decodeStr(bytesize.ToString());

            PrintDebug(ConvertInputData(analyResult), ConvertOutputData(sendMessage.SendData, true));
            return true;
        }
        #endregion
        #region 厂家校准用逻辑参数
        public static bool scpiQuy_FactoryCaliLogicValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.FactoryCaliScpiProc_LogicValue_Get());
            return true;
        }
        public static bool scpiSet_FactoryCaliLogicValue(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            return ExportHdFuncs.FactoryCaliScpiProc_LogicValue_Set(paramStr);
        }
        #endregion 厂家校准用逻辑参数

        #region 厂家校准用特殊数据

        public static bool scpiQuy_FactoryFlashCaliDataWriteFlaga(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var calitype = ExportHdFuncs.GetFlashCaliType();
            var res = ExportHdFuncs.GetUpdateFlashCaliDataFlag ? "1" : "0";
            sendMessage.UsingScientificNotation = false;
            sendMessage.SendData = Encoding.UTF8.GetBytes($"{res},{calitype}");
            return true;
        }

        public static bool scpiSet_FactoryFlashCaliDataWriteFlag(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            ExportHdFuncs.SetFlashCaliType((UInt32)analyResult.ChannelIndex);
            var value = false;
            if (paramStr == "0" || paramStr.ToUpper() == Boolean.FalseString.ToUpper())
            {
                value = false;
            }
            if (paramStr == "1" || paramStr.ToUpper() == Boolean.TrueString.ToUpper())
            {
                value = true;
            }
            ExportHdFuncs.SetUpdateFlashCaliDataFlag(value);
            if (Enum.IsDefined(typeof(CaliDataType), analyResult.ChannelIndex))
            {
                if ((CaliDataType)analyResult.ChannelIndex == CaliDataType.AnalogParams || (CaliDataType)analyResult.ChannelIndex == CaliDataType.PhyChannel)
                    Helper.GetICaliData(CaliDataType.AutoCalibration)?.SaveToFile();
            }
            return true;
        }

        public static bool scpiQuy_FactoryFlashCaliDataReadFlaga(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var calitype = ExportHdFuncs.GetFlashCaliType();
            var res = ExportHdFuncs.GetReadFlashCaliDataFlag ? "1" : "0";
            sendMessage.UsingScientificNotation = false;
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes($"{res},{calitype}");
            return true;
        }

        public static bool scpiSet_FactoryFlashCaliDataReadFlag(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            ExportHdFuncs.SetFlashCaliType((UInt32)analyResult.ChannelIndex);
            var value = false;
            if (paramStr == "0" || paramStr.ToUpper() == Boolean.FalseString.ToUpper())
            {
                value = false;
            }
            if (paramStr == "1" || paramStr.ToUpper() == Boolean.TrueString.ToUpper())
            {
                value = true;
            }
            ExportHdFuncs.SetReadFlashCaliDataFlag(value);
            return true;
        }

        public static bool scpiQuy_FactoryCaliSpecialData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string paramStr = ConvertParamToString(analyResult);
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Get(paramStr));
            return true;
        }
        public static bool scpiSet_FactoryCaliSpecialData(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            return ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set(paramStr);
        }
        public static bool scpiQuy_FactoryCaliSpecialReadBackAdcRegister(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string paramStr = ConvertParamToString(analyResult);
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(ExportHdFuncs.ReadbackAdcRegisterData());
            return true;
        }
        public static bool scpiSet_FactoryCaliApplySource(SCPICommandProcessFuncParam analyResult)
        {
            bool TryGetScaleIndex(int valueByuV, out Core.AnaChnlScaleIndex scaleIndex)
            {
                scaleIndex = Core.AnaChnlScaleIndex.Lv1;
                for (int i = 0; i < AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.Count; i++)
                {
                    if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[i] == valueByuV)
                    {
                        scaleIndex = (Core.AnaChnlScaleIndex)i;
                        return true;
                    }
                }
                return false;
            }
            string paramStr = ConvertParamToString(analyResult);
            string[] paramList = paramStr.Split(',');
            //打开[0,1],幅度档(mv数),基线位置[0],耦合[0=DC，1=AC,2=DC50],阻抗[0=1M，1=50],带宽限制[0=关闭,1=20M,2=100M],偏置[mv数],探头倍率[枚举],反向[0=off,1=on]，单位[0=V,1=A]
            //    0        1            2          3                          4                    5                            6               7           8                9               
            if (Presenter.TryGetChannel((ChannelId)(analyResult.ChannelIndex - 1), out var ch))
            {
                AnalogPrsnt anaChnlPrsnt = (AnalogPrsnt)ch;
                anaChnlPrsnt.Active = paramList[0] == "1";
                double scaleBymV = double.Parse(paramList[1]);
                if (TryGetScaleIndex((int)(scaleBymV * 1_000), out Core.AnaChnlScaleIndex scaleIndex))
                    anaChnlPrsnt.ScaleIndex = (int)scaleIndex;
                anaChnlPrsnt.Coupling = (AnaChnlCoupling)int.Parse(paramList[3]);
                anaChnlPrsnt.PosIndexBymDiv = double.Parse(paramList[2]);

                anaChnlPrsnt.Bias = double.Parse(paramList[6]) * 1000; //mV==>uV
                anaChnlPrsnt.IsInverted = (paramList[8] == "1");

                anaChnlPrsnt.Bandwidth = paramList[5] switch
                {
                    "0" => 0,//AnaChnlBandwidth.Full,
                    "1" => 1,//AnaChnlBandwidth.1G,
                    "2" => 2,//AnaChnlBandwidth.Bw500MHz,
                    "3" => 3,//AnaChnlBandwidth.Bw20MHz,
                    _ => 0//AnaChnlBandwidth.Full
                };
            }
            else
                return false;
            return true;
        }
        public static bool scpiSet_FactoryCaliApplyAllSource(SCPICommandProcessFuncParam analyResult)
        {
            bool TryGetScaleIndex(int valueByuV, out Core.AnaChnlScaleIndex scaleIndex)
            {
                scaleIndex = Core.AnaChnlScaleIndex.Lv1;
                for (int i = 0; i < AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.Count; i++)
                {
                    if (AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[i] == valueByuV)
                    {

                        scaleIndex = (Core.AnaChnlScaleIndex)i;
                        return true;
                    }
                }
                return false;
            }
            string paramStr = ConvertParamToString(analyResult);
            string[] paramList = paramStr.Split(',');
            //打开[0,1],幅度档(mv数),基线位置[0],耦合[直流1M、交流、DC50OM,枚举],阻抗[0=1M_om，1=5O_om],带宽限制[0=关闭,1=20M,2=100M],偏置[mv数],探头倍率[枚举],反向[0=off,1=on]，单位[0=V,1=A]
            //    0        1            2          3                          4                    5                            6               7           8                9               
            double scaleBymV = double.Parse(paramList[1]);
            if (!TryGetScaleIndex((int)(scaleBymV * 1_000), out Core.AnaChnlScaleIndex scaleIndex))
                return false;
            AnaChnlCoupling anaChnlCoupling = (AnaChnlCoupling)int.Parse(paramList[3]);
            Int32 anaChnlBandwidth = paramList[5] switch
            {
                "0" => 0,//AnaChnlBandwidth.Full,
                "1" => 1,//AnaChnlBandwidth.1G,
                "2" => 2,//AnaChnlBandwidth.Bw500MHz,
                "3" => 3,//AnaChnlBandwidth.Bw20MHz,
                _ => 0//AnaChnlBandwidth.Full
            };
            double bias = double.Parse(paramList[6]) * 1000; //mV==>uV
            bool IsInverted = (paramList[8] == "1");
            bool bActive = paramList[0] == "1";
            double posIndex = double.Parse(paramList[2]);
            for (ChannelId channelId = ChannelId.C1; channelId < ChannelId.C1 + ChannelIdExt.AnaChnlNum; channelId++)
            {
                if (Presenter.TryGetChannel(channelId, out var ch))
                {
                    AnalogPrsnt anaChnlPrsnt = (AnalogPrsnt)ch;
                    try
                    {
                        anaChnlPrsnt.Active = bActive;
                        anaChnlPrsnt.ScaleIndex = (int)scaleIndex;
                        anaChnlPrsnt.Coupling = anaChnlCoupling;

                        anaChnlPrsnt.PosIndexBymDiv = posIndex;

                        anaChnlPrsnt.Bias = bias;
                        anaChnlPrsnt.IsInverted = IsInverted;

                        anaChnlPrsnt.Bandwidth = anaChnlBandwidth;
                    }
                    catch
                    {

                    }
                }
            }
            return true;
        }

        public static bool scpiSet_FactoryActiveChannel(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            for (ChannelId channelId = ChannelId.C1; channelId < ChannelId.C1 + ChannelIdExt.AnaChnlNum; channelId++)
            {
                if (Presenter.TryGetChannel(channelId, out var ch))
                {
                    AnalogPrsnt anaChnlPrsnt = (AnalogPrsnt)ch;
                    try
                    {
                        anaChnlPrsnt.Active = (paramStr.IndexOf((channelId).ToString()) >= 0) || paramStr == "All-10G" ? true : false;
                    }
                    catch
                    {

                    }
                }
            }
            return true;
        }

        public static bool scpiSet_FactoryLAChannel(SCPICommandProcessFuncParam analyResult)
        {
            List<string> param = ParamListToStrList(analyResult.Params);
            if (param.Count <= 0)
            {
                return false;
            }
            var valueString = param[0];
            if (TryGetLAChannelPrsnt(analyResult, out DigitalPrsnt prsnt))
            {
                if (valueString == "ON" || valueString == "1")
                {
                    prsnt.SetCalibrationDigital();
                    return true;
                }
                else if (valueString == "OFF" || valueString == "0")
                {
                    prsnt.CloseAllDigital();
                    return true;
                }
            }
            return false;
        }

        public static bool scpiQuy_FactoryScreenJpeg(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string? paramStr = FilePrsnt.GetImageBase64String();
            if (string.IsNullOrEmpty(paramStr))
            {
                paramStr = "";
            }
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(paramStr);
            return true;
        }

        #endregion 厂家校准用特殊数据
        public static bool scpiQuy_Factory_StorageDeep(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            sendMessage.IsDataBlock = false;

            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(Presenter.Timebase.StorageWaveDotsCnt.ToString());
            return true;
        }
        public static bool scpiSet_Factory_StorageDeep(SCPICommandProcessFuncParam analyResult)
        {
            string paramStr = ConvertParamToString(analyResult);
            switch (paramStr)
            {
                case "Auto":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Auto;
                    break;
                case "Of25KDots":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Of25KDots;
                    break;
                case "Of250KDots":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Of250KDots;
                    break;
                case "Of2_5MDots":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Of2_5MDots;
                    break;
                case "Of25MDots":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Of25MDots;
                    break;
                case "Of250MDots":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Of250MDots;
                    break;
                case "Full":
                    Presenter.Timebase.StorageDepthOpt = (int)AnaChnlLengthOpt.Full;
                    break;
            }
            return true;
        }

        #region Web


        private enum WebLanguage
        {
            zh_CN,
            en_US,
        }

        private static WebLanguage _WebLanguage = WebLanguage.zh_CN;

        private static List<(String? Language, String? Info)> ManufacturerInfo = new List<(string? Language, string? Info)>()
        {
            ("zh_CN","优利德科技（中国）股份有限公司"),
            ("en_US","Uni-Trend Technology (China) Co., Ltd.")
        };


        //private static ManufacturerInfo ManufacturerInfo = new ManufacturerInfo()
        //{
        //    Languages = new List<ManufacturerLanguage>
        //    {
        //        new ManufacturerLanguage{Language="zh_CN",Info="优利德科技（中国）股份有限公司"},
        //        new ManufacturerLanguage{Language="en_US",Info="Uni-Trend Technology (China) Co., Ltd."},
        //    }
        //};

        internal static void LoadManufacturerInfo()
        {
            string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScopXConfig");
            string xmlPath = Path.Combine(dirPath, "ManufacturerInfo.xml");

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                System.IO.File.SetAttributes(dirPath, System.IO.File.GetAttributes(dirPath) | FileAttributes.Hidden);
            }

            // 检查文件是否存在
            if (!File.Exists(xmlPath))
            {
                var resourcexml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ManufacturerInfo.xml");
                if (File.Exists(resourcexml))
                {
                    System.IO.File.Copy(resourcexml, xmlPath, true);
                }
                else
                    CreateDefaultXmlFile(xmlPath);// 如果文件不存在，根据默认的 ManufacturerInfo 列表生成 XML 文件
            }

            //读取xml
            using (FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(fs))
                    {
                        var list = new List<(string? Language, string? Info)>();

                        // 移动到 "Items" 元素
                        reader.ReadToFollowing("Items");
                        using (XmlReader subreader = reader.ReadSubtree())
                        {
                            while (subreader.Read())
                            {
                                if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Language")
                                {
                                    string? language = subreader.GetAttribute("Value");
                                    string? info = subreader.GetAttribute("Info");
                                    list.Add((language, info));
                                }
                            }
                        }
                        ManufacturerInfo.Clear();
                        ManufacturerInfo = list;
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        internal static void LoadPrivateManufacturerInfo()
        {
            string xmlfilepath = $"{typeof(StubFunc).Namespace}.ManufacturerInfo.xml";
            using (Stream? sm = (typeof(StubFunc)).Assembly.GetManifestResourceStream(xmlfilepath))
            {
                if (sm == null)
                {
                    return;
                }

                try
                {
                    using (XmlReader reader = XmlReader.Create(sm))
                    {
                        var list = new List<(string? Language, string? Info)>();

                        // 定位到 "Items" 元素
                        reader.ReadToFollowing("Items");

                        using (XmlReader subreader = reader.ReadSubtree())
                        {
                            while (subreader.Read())
                            {
                                if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Language")
                                {
                                    string? language = subreader.GetAttribute("Value");
                                    string? info = subreader.GetAttribute("Info");
                                    list.Add((language, info));
                                }
                            }
                        }
                        ManufacturerInfo.Clear();
                        ManufacturerInfo = list;
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private static void CreateDefaultXmlFile(string xmlPath)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(xmlPath, new XmlWriterSettings { Indent = true }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Items");

                    foreach (var item in ManufacturerInfo)
                    {
                        writer.WriteStartElement("Language");
                        writer.WriteAttributeString("Value", item.Language);
                        writer.WriteAttributeString("Info", item.Info);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                // 处理创建文件时的异常
                return;
            }
        }

        public static bool scpiQuy_WebManufacturerinfo(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string paramStr = string.Empty;
            if (ManufacturerInfo == null || ManufacturerInfo.Count == 0)
            {

            }
            else
            {
                var mi = ManufacturerInfo.FirstOrDefault(manuf => manuf.Language == _WebLanguage.ToString());
                paramStr = mi.Info ?? string.Empty;
            }
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(paramStr);
            sendMessage.UsingScientificNotation = false;
            return true;
        }
        public static bool scpiQuy_WebLanguage(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string paramStr = _WebLanguage.ToString();
            sendMessage.SendData = System.Text.Encoding.UTF8.GetBytes(paramStr);
            sendMessage.UsingScientificNotation = false;
            return true;
        }

        public static bool scpiSet_WebLanguage(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult, out string param))
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var paramlist = scpiTagObj.ParamList;
            if (paramlist == null || paramlist.Count <= 0)
            {
                return false;
            }
            int foundIndex = paramlist.FindIndex(s => s == param);
            foundIndex = foundIndex == -1 ? 0 : foundIndex;
            _WebLanguage = (WebLanguage)foundIndex;

            return false;
        }

        //public static void SerializeToBinaryFile(ManufacturerInfo items, string filePath)
        //{
        //    var xmlSerializer = new XmlSerializer(typeof(ManufacturerInfo));
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        // 序列化到内存流
        //        xmlSerializer.Serialize(memoryStream, items);
        //        // 将内存流中的数据写入二进制文件
        //        File.WriteAllBytes(filePath, memoryStream.ToArray());
        //    }
        //}

        //public static ManufacturerInfo DeserializeFromBinaryFile(string filePath)
        //{
        //    var xmlSerializer = new XmlSerializer(typeof(ManufacturerInfo));
        //    byte[] bytes = File.ReadAllBytes(filePath);
        //    using (var memoryStream = new MemoryStream(bytes))
        //    {
        //        return (ManufacturerInfo)xmlSerializer.Deserialize(memoryStream);
        //    }
        //}

        #endregion
    }
}
