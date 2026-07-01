using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.ComModel;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Threading.Channels;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_SaveWaveData : BatchTaskPartBase
    {
        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd?.ProgramFuncName.Trim() ?? "");
            if (myName_ParameterPair != null)
                return AnalyParameter(myName_ParameterPair[1]);
            else
                return false;
        }
        public override string FuncionDescription
        {
            get => $"保存文本格式的波形数据。";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数，MaxDataCount: 每文件最大行数，也就是每通道最大数据个数。{System.Environment.NewLine}" +
                   $"第2个参数，IncludeChannels:通道编号，从1开始。如Ch1|CH2|Ch3，或者Sub1|Sub2|Sub3|Sub4。可以用'-'表示该位置的数据不需要保存。 {System.Environment.NewLine}" +
                   $"第3个参数，SaveFilePathAndNames：保存文件的路径及名称，可以是相对路径或绝对路径，相对路径必须以'.'开始,相对路径该指Tool工具exe程序所在的路径为根路径。可以用'-'表示该位置的数据不需要保存 。 {System.Environment.NewLine}" +
                   $"第4个参数，不是读取长存储时，不用！是否读取长存储原始数据。true,false{System.Environment.NewLine}" +
                   $"第5个参数，不是读取长存储时，不用！分包读取时，每包的点数。UsbTMC和LAN为了速度，可以设置为不同大小。UsbTMC 为2000，LAN可以大一些，比如100000";
        }
        public override string Example
        {
            get => "";
        }
        private int totalDataCount = 10000;
        private List<int> includeChannels = new List<int>();
        private List<string> SaveFilePathAndNames = new List<string>();
        private Boolean bIsSourceData = false;
        Int32 perTimeMaxCount = 2000;
        private bool AnalyParameter(string parameter)
        {
            if (parameter == "")
                return false;
            bIsSourceData = false;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 3)
                return false;
            //1.数据最大个数
            string tmpStr = paramList[0];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            totalDataCount = int.Parse(tmpStr);
            //2.指定通道
            tmpStr = paramList[1];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            includeChannels.Clear();
            string[] tmpList = tmpStr.Split('|');
            foreach (string s in tmpList)
            {
                if (s != "-")
                    includeChannels.Add(int.Parse(s.Substring(s.Length - 1)) - 1);
                else
                    includeChannels.Add(-1);
            }
            //3.保存路径及文件名称
            tmpStr = paramList[2];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            SaveFilePathAndNames.Clear();
            tmpList = tmpStr.Split('|');
            foreach (string s in tmpList)
            {
                string trimStr = s.Trim();
                if (trimStr != "-" && trimStr != "")
                {
                    if (trimStr[0] == '.')
                        SaveFilePathAndNames.Add(AppDomain.CurrentDomain.BaseDirectory + trimStr);
                    else
                        SaveFilePathAndNames.Add(trimStr);
                }
                else
                    SaveFilePathAndNames.Add("");
            }
            if (includeChannels.Count != SaveFilePathAndNames.Count)
                return false;
            if (paramList.Length >= 4)
            {
                bIsSourceData = paramList[3].Trim().ToLower() == "true";
            }
            if (paramList.Length >= 5)
            {
                perTimeMaxCount = Int32.Parse(paramList[4]);
            }
            if (bIsSourceData && includeChannels.Count > 1)
            {
                var first = includeChannels[0];
                includeChannels.Clear();
                includeChannels.Add(first);
            }
            return true;
        }
        private List<ushort[]>? GetSourceData(int channelID, int maxWaitMs, CancellationToken? token = null)
        {
            List<ushort>? sourceData = new List<ushort>();

            Int32 readedDots = 0;
            Int32 reTryTimes1 = 0;
            Int32 reTryTimes2 = 0;
            byte[] perTimeRecvBytes = new byte[perTimeMaxCount * 2];
            do
            {
                int currDots = perTimeMaxCount;
                if ((readedDots + perTimeMaxCount) > totalDataCount)
                    currDots = totalDataCount - readedDots;
                string scpiCommand = $":WAV:SDAT?{channelID + 1},{currDots},{readedDots}";
                currInstrumentSession!.WriteString(scpiCommand);
                //Thread.Sleep(5);
                int recvBytes = 0;
                reTryTimes1 = 0;
                while (reTryTimes1 < 10)
                {
                    recvBytes = currInstrumentSession!.ReadBinData(ref perTimeRecvBytes);
                    if (recvBytes == 0)
                    {
                        reTryTimes1++;
                        Thread.Sleep(2);
                    }
                    else
                        break;
                }
                if (recvBytes >= currDots * 2)
                {
                    int posIndex = 0;
                    for (int i = 0; i < currDots; i++)
                    {
                        sourceData.Add(BitConverter.ToUInt16(perTimeRecvBytes, posIndex));
                        posIndex += 2;
                    }
                }
                else
                {
                    if (recvBytes == 0)
                    {
                        if (reTryTimes2 < 5)
                        {
                            reTryTimes2++;
                            continue;
                        }
                        return null;
                    }
                    ;//error
                }
                readedDots += currDots;
                reTryTimes2 = 0;
            } while (readedDots < totalDataCount);
            if (sourceData.Count != totalDataCount)
                return null;
            else
            {
                return new List<ushort[]>() { sourceData.ToArray() };
            }
        }
        private Int64 GetSourceDataTotalLength()
        {
            string scpiCommand = $":FACT:STOR?";
            currInstrumentSession!.WriteString(scpiCommand);
            string v = currInstrumentSession!.ReadShortString();
            if (double.TryParse(v, out double d))
                return (Int64)d;
            return 0;
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            List<ushort[]>? allChannelData = new List<ushort[]>();
            ServerSpecailData.Load(currInstrumentSession);
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            if (!bIsSourceData)
                allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrumentSession!, 6_000);
            else
            {
                var d = GetSourceDataTotalLength();
                if (d<totalDataCount)
                {
                    message = $"请求读取的数据长度太大！实际只有{d}Pts,请求读取{totalDataCount}Pts";
                    return BatchTaskPartResult.ErrorFatal;
                }
                allChannelData = GetSourceData(includeChannels[0], 6_000);
            }
            if (allChannelData == null)
            {
                message = "数据读出错误！";
                return BatchTaskPartResult.ErrorFatal;
            }
            message = "";
            for (int includeChannelIndex = 0; includeChannelIndex < includeChannels.Count; includeChannelIndex++)
            {
                int currChannelID = includeChannels[includeChannelIndex];
                if (currChannelID >= 0 && SaveFilePathAndNames[includeChannelIndex] != "")
                {
                    string filepath = CalibrationOscilloscopeInfo.Defalut.FileDir + SaveFilePathAndNames[includeChannelIndex];
                    string? path = Path.GetDirectoryName(filepath)!;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    if (!bIsSourceData)
                    {
                        for (int k = 0; k < allChannelData[currChannelID].Length && k < totalDataCount; k++)
                            sw.WriteLine(allChannelData[currChannelID][k]);
                    }
                    else
                    {
                        for (int k = 0; k < allChannelData[0].Length && k < totalDataCount; k++)
                            sw.WriteLine(allChannelData[0][k]);
                    }
                    sw.Close();
                    fs.Close();
                }
            }
            batchTaskPartResult = BatchTaskPartResult.Succeed;
            message = $"OK!";
            return batchTaskPartResult;
        }
    }
}
