using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Properties;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    /// <summary>
    /// 信号损耗测量
    /// </summary>
    internal class BatchTask_TiAdcParamTest : BatchTaskPartBase
    {
        public override string FuncionDescription
        {
            get => $"通过频谱仪测试射频源的信号损耗和线损";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数，扫频结果存储文件夹{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，扫频结果存储文件名{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，扫频频点文件路径{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，信号源VISA地址{System.Environment.NewLine}";
            }
        }
        public override string Example
        {
            get => @"BatchTask_SignallossMeasurement false,Resources,SignalAnalyer.txt,扫频频率点表\生成TiAdc校准系数扫频频点.txt,USB0::0x1AB1::0x0641::DG4E183302205::INSTR,USB0::0x1AB1::0x0641::DG4E183302205::INSTR";
        }
        private ChannelId CaliChannelId;
        private string Param_ScanFrequenciesFilePath = "";
        private string Param_ScanFrequencyResultDataDir = "";
        private bool Param_UpdateResponse = false;
        private string Param_SignalAddr = "";
        private string Param_SavePath = "";
        private string Param_SignalAnalyzerAddr = "";

        private bool _ParamsValid = false;
        private List<Int64> _ScanFrequenciesList = new List<Int64>();
        private IInstrumentSession? _SigInstrumentSession;

        private bool AnalyParameter(string parameter)
        {
            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 3)
                return _ParamsValid = false;


            //[0]: NeedCaliChannelList
            string channelID = paramList[0];
            switch (channelID)
            {
                case "1": CaliChannelId = ChannelId.C1; break;
                case "2": CaliChannelId = ChannelId.C2; break;
                case "3": CaliChannelId = ChannelId.C3; break;
                case "4": CaliChannelId = ChannelId.C4; break;
            }
            //[0]:Param_ScanFrequencyResultDataDir
            Param_ScanFrequencyResultDataDir = paramList[1].Trim();

            //[1]:Param_SavePath
            Param_SavePath = paramList[2].Trim();

            if (Param_UpdateResponse)
            {
                return _ParamsValid = true;
            }
            if (paramList.Length < 4)
                return _ParamsValid = false;

            //[2]:Param_ScanFrequenciesFilePath
            Param_ScanFrequenciesFilePath = paramList[3].Trim();
            _ParamsValid = InitScanFrequenciesList(Param_ScanFrequenciesFilePath);

            //[3]:Param_SignalAddr
            Param_SignalAddr = paramList[4].Trim();
            _SigInstrumentSession = new VISASession(Param_SignalAddr, 20);
            if (!_SigInstrumentSession?.Open() ?? false)
                return _ParamsValid = false;

            return _ParamsValid = true;
        }

        public void SendScpiCmd(IInstrumentSession instrumentSession, string cmd)
        {
            instrumentSession?.WriteString(cmd);
        }


        private bool InitScanFrequenciesList(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string? lineContent;
                    while ((lineContent = sr.ReadLine()) != null)
                    {
                        _ScanFrequenciesList.Add(Int64.Parse(lineContent));
                    }
                }
            }
            catch (IOException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        public override BatchTaskPartResult Exec(double overtimeBySec, out string outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            outMsg = String.Empty;
            if (!_ParamsValid)
            {
                outMsg = "参数错误！";
                return BatchTaskPartResult.ErrorParameter;
            }

            if (!Directory.Exists(Param_ScanFrequencyResultDataDir))
                Directory.CreateDirectory(Param_ScanFrequencyResultDataDir);
            string filePath = Path.Combine(Param_ScanFrequencyResultDataDir, Param_SavePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"Frequency,Phase,Gain,offset");
            }
            foreach (var frequency in _ScanFrequenciesList)
            {
                _SigInstrumentSession?.WriteString($"FREQ:FIXed {frequency.ToString()}");
                Thread.Sleep(1000);
                try
                {
                    List<ushort[]>? adcData = InstrumentInteract.Factory_WaveData_Adc(currInstrumentSession, 6_000, null, 80000);
                    Dictionary<string, List<ushort>> data = new Dictionary<string, List<ushort>>();
                    if (CaliChannelId == ChannelId.C1)
                    {
                        data.Add("adc1", adcData[0].ToList());
                        data.Add("adc0", adcData[1].ToList());
                    }
                    else
                    {
                        data.Add("adc1", adcData[2].ToList());
                        data.Add("adc0", adcData[3].ToList());
                    }
                    CaliSineFit(data, 10_000, frequency / 1000000, filePath);
                    cancelTokenSrc!.Token.ThrowIfCancellationRequested();
                }
                catch (IOException)
                {
                    outMsg = "输入写入文件失败！";
                    return BatchTaskPartResult.ErrorParameter;
                }
                catch (OperationCanceledException)
                {
                    outMsg = "任务取消！";
                    return BatchTaskPartResult.Cancel;
                }
            }
            return BatchTaskPartResult.Succeed;
        }

        private bool CaliSineFit(Dictionary<string, List<ushort>> keyValuePairs, double sampleByM_Sps, double signalFreqByMHz, string filepath)
        {
            //相位理论误差
            double theoryDelta_pS = 50d;

            Dictionary<int, WaveOffsetGainPhase> keyValueWaveOffsetGainPhases = new Dictionary<int, WaveOffsetGainPhase>();
            WaveOffsetGainPhase[] waveOffsetGainPhasesList = new WaveOffsetGainPhase[keyValuePairs.Count];
            int i = 0;
            foreach (var item in keyValuePairs)
            {
                keyValueWaveOffsetGainPhases.Add(i, SineFitFunc.SineFit(item.Value.ToArray(), sampleByM_Sps, signalFreqByMHz));
                i++;
            }
            foreach (var item in keyValuePairs)
            {
                string fintKey = keyValuePairs.First().Key;
                if (keyValuePairs.Count != 2)
                {
                    fintKey = item.Key.Replace("Adc0", "Adc1");
                }
                else
                {
                    theoryDelta_pS = 50;
                }

                WaveOffsetGainPhase fintWaveOffsetGainPhase = SineFitFunc.SineFit(keyValuePairs[fintKey].ToArray(), sampleByM_Sps, signalFreqByMHz);
                WaveOffsetGainPhase WaveOffsetGainPhase = SineFitFunc.SineFit(keyValuePairs[item.Key].ToArray(), sampleByM_Sps, signalFreqByMHz);
                int adcIndex = WaveOffsetGainPhase.Phase == fintWaveOffsetGainPhase.Phase ? 0 : 1;
                double PhaseError_pS = ((WaveOffsetGainPhase.Phase - fintWaveOffsetGainPhase.Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / signalFreqByMHz / (2 * Math.PI) - adcIndex * theoryDelta_pS;
                double GainError = 100 * (WaveOffsetGainPhase.Gain - fintWaveOffsetGainPhase.Gain) / fintWaveOffsetGainPhase.Gain;
                double OffsetError = (WaveOffsetGainPhase.Offset - fintWaveOffsetGainPhase.Offset);

                if (PhaseError_pS > 1000_000 / signalFreqByMHz / 2)
                    PhaseError_pS -= 1000_000 / signalFreqByMHz;
                else if (PhaseError_pS < -1000_000 / signalFreqByMHz / 2)
                    PhaseError_pS += 1000_000 / signalFreqByMHz;
                if (PhaseError_pS != 0 && GainError != 0 && OffsetError != 0)
                {
                    using (StreamWriter sw = new StreamWriter(filepath, true))
                    {
                        sw.WriteLine($"{signalFreqByMHz.ToString()}Mhz,{PhaseError_pS},{GainError},{OffsetError}");
                    }
                }
            }
            return true;
        }

    }
}
