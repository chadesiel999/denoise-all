using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal class BatchTaskPart_ScanFrequency_MSO8000X : BatchTaskPartBase
    {
        public override string FuncionDescription
        {
            get => $"MSO8000X的扫频";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数：通道组合构造成的交织方式，请使用Driver端代码中的AnalogAcquireModel.cs 之 AcqModeInterleaveDefines 之InterleaveName{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，需要校准的通道，Ch1、Ch2、Ch3、Ch4{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，阻抗，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，频点文件路径{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，保存该次扫频数据的文件夹{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，固定保存扫频数据的文件夹{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，信号源VISA地址{System.Environment.NewLine}";
            }
        }
        public override string Example
        {
            get => "BatchTaskPart_ScanFrequency_MSO8000X C1C3_20G,Ch1,HIGH,50,USB0::0x1AB1::0x0641::DG4E183302205::INSTR";
        }

        private string Param_InterleaveName = "";
        private ChannelId Param_Chnl = ChannelId.C1;
        private string Param_ImpedanceName = "";
        private int Param_Impedance = 0;  //0为高，1为低；
        private double Param_InputLevelBymV = 50;
        private string Param_ScanFrequenciesFilePath = "";
        private string Param_ScanFrequencyResultDataDir = "";
        private string Param_SignalAddr = "";
        private string Param_ScanFreqDataPath = "";//固定存储数据路径，不断更新

        private bool _ParamsValid = false;
        private List<Int64> _ScanFrequenciesList = new List<Int64>();
        private IInstrumentSession? _SigInstrumentSession;

        private bool AnalyParameter(string parameter)
        {
            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 7)
                return _ParamsValid = false;

            //[0]Param_InterleaveName
            Param_InterleaveName = paramList[0].Trim();
            //if (!ServerSpecailData.JiHe_MSO8000X_AcqModeInterleaveDefines?.Exists(def => def.InterleaveName == Param_InterleaveName) ?? false)
            //    return _ParamsValid = false;

            //[1]: NeedCaliChannelList
            int chnl = (paramList[1].Trim().Last<char>() - '1');
            if (chnl >= (int)ChannelId.C1 && chnl <= (int)ChannelId.C4)
                Param_Chnl = (ChannelId)chnl;
            else
                return _ParamsValid = false;

            //[2]:Impedance
            Param_ImpedanceName = paramList[2].Trim().ToUpper();
            Param_Impedance = Param_ImpedanceName switch
            {
                "HIGH" => 0,
                _ => 1
            };

            //[3]:Param_InputLevelBymV
            Param_InputLevelBymV = BaseHelper.TryConvertToDouble(paramList[3].Trim());

            //[4]:Param_ScanFrequenciesFilePath
            Param_ScanFrequenciesFilePath = paramList[4].Trim();
            _ParamsValid = InitScanFrequenciesList(Param_ScanFrequenciesFilePath);

            //[5]:Param_ScanFrequencyResultDataDir
            Param_ScanFrequencyResultDataDir = paramList[5].Trim();

            //[5]:Param_ScanFrequencyResultDataDir
            Param_ScanFreqDataPath = paramList[6].Trim();


            //[6]:Param_SignalAddr
            Param_SignalAddr = paramList[7].Trim();
            _SigInstrumentSession = new VISASession(Param_SignalAddr, 20);
            if (!_SigInstrumentSession?.Open() ?? false)
                return _ParamsValid = false;

            return _ParamsValid = true;
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
            ServerSpecailData.Load(currInstrumentSession);
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            var currModeDef = ServerSpecailData.JiHe_AcqModeInterleaveDefines!.First(o => o.Value.Name == Param_InterleaveName);

            string fileDir = CalibrationOscilloscopeInfo.Defalut!.FileDir + Path.Combine(Param_ScanFrequencyResultDataDir,
                $"{Param_InterleaveName}_CH{(int)(Param_Chnl + 1)}_{Param_ImpedanceName}_{Param_InputLevelBymV}Mv");
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);
            if (Directory.Exists(Param_ScanFreqDataPath))
                Directory.Delete(Param_ScanFreqDataPath, true);
            Directory.CreateDirectory(Param_ScanFreqDataPath);

            foreach (var frequency in _ScanFrequenciesList)
            {
                _SigInstrumentSession?.WriteString("SOUR1:FREQ " + frequency.ToString());
                Thread.Sleep(1000);
                List<ushort[]>? allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession!, 10000); //ServerDomainConstants.PerAdcCoreDataCount); 0321 htf  less than 20000 
                if (allChannelData == null)
                {
                    outMsg = "读取波形数据错误！";
                    return BatchTaskPartResult.ErrorParameter;
                }
                #region sitongdao

                for (int k = 0; k < 4; k++)
                {
                    string filePath = Path.Combine(fileDir, $"Data_{k}_{(frequency / 1_000_000)}MHz.txt");
                    string DataPath = Path.Combine(Param_ScanFreqDataPath, $"Data_{k}_{(frequency / 1_000_000)}MHz.txt");
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {
                            for (int i = 0; i < allChannelData[k].Length; i++)
                            {
                                sw.WriteLine(allChannelData[k][i]);
                            }
                        }
                        using (StreamWriter sw = new StreamWriter(DataPath))
                        {
                            for (int i = 0; i < allChannelData[k].Length; i++)
                            {
                                sw.WriteLine(allChannelData[k][i]);
                            }
                        }
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
                #endregion


                #region dantongdao

                //string filePath = Path.Combine(fileDir, $"Data_{(frequency / 1_000_000)}MHz.txt");
                //string DataPath = Path.Combine(Param_ScanFreqDataPath, $"Data_{(frequency / 1_000_000)}MHz.txt");
                //try
                //{
                //    if (File.Exists(filePath))
                //    {
                //        File.Delete(filePath);
                //    }
                //    using (StreamWriter sw = new StreamWriter(filePath))
                //    {
                //        for (int i = 0; i < allChannelData[(int)Param_Chnl].Length; i++)
                //        {
                //            sw.WriteLine(allChannelData[(int)Param_Chnl][i]);
                //        }
                //    }
                //    using (StreamWriter sw = new StreamWriter(DataPath))
                //    {
                //        for (int i = 0; i < allChannelData[(int)Param_Chnl].Length; i++)
                //        {
                //            sw.WriteLine(allChannelData[(int)Param_Chnl][i]);
                //        }
                //    }
                //    cancelTokenSrc!.Token.ThrowIfCancellationRequested();
                //}
                //catch (IOException)
                //{
                //    outMsg = "输入写入文件失败！";
                //    return BatchTaskPartResult.ErrorParameter;
                //}
                //catch (OperationCanceledException)
                //{
                //    outMsg = "任务取消！";
                //    return BatchTaskPartResult.Cancel;
                //}
                #endregion
            }
            return BatchTaskPartResult.Succeed;
        }
    }
}
