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
    internal class BatchTask_SignallossMeasurement : BatchTaskPartBase
    {
        public override String FuncionDescription
        {
            get => $"通过频谱仪测试射频源的信号损耗和线损";
        }
        public override String ParametersDescription
        {
            get
            {
                Int32 argindex = 1;
                return $"第{argindex++}个参数：是否更新频响设计曲线{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，扫频结果存储文件夹{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，扫频结果存储文件名{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，扫频频点文件路径{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，信号源VISA地址{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，频谱仪VISA地址{System.Environment.NewLine}";
            }
        }
        public override String Example
        {
            get => @"BatchTask_SignallossMeasurement false,Resources,SignalAnalyer.txt,扫频频率点表\生成TiAdc校准系数扫频频点.txt,USB0::0x1AB1::0x0641::DG4E183302205::INSTR,USB0::0x1AB1::0x0641::DG4E183302205::INSTR";
        }

        private String _Param_ScanFrequenciesFilePath = "";
        private String _Param_ScanFrequencyResultDataDir = "";
        private Boolean _Param_UpdateResponse = false;
        private String _Param_SignalAddr = "";
        private String _Param_SavePath = "";
        private String _Param_SignalAnalyzerAddr = "";

        private Boolean _ParamsValid = false;
        private List<Int64> _ScanFrequenciesList = new List<Int64>();
        private IInstrumentSession? _SigInstrumentSession;
        private IInstrumentSession? _MeasureInstrumentSession;

        private Boolean AnalyParameter(String parameter)
        {
            parameterStr = parameter;
            String[] paramlist = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramlist.Length < 3)
                return _ParamsValid = false;
            //[0]:Param_UpdateResponse
            _Param_UpdateResponse = Boolean.Parse(paramlist[0].Trim());

            //[1]:Param_ScanFrequencyResultDataDir
            _Param_ScanFrequencyResultDataDir = paramlist[1].Trim();

            //[2]:Param_SavePath
            _Param_SavePath = paramlist[2].Trim();

            if (_Param_UpdateResponse)
            {
                return _ParamsValid = true;
            }
            if (paramlist.Length < 6)
                return _ParamsValid = false;

            //[3]:Param_ScanFrequenciesFilePath
            _Param_ScanFrequenciesFilePath = paramlist[3].Trim();
            _ParamsValid = InitScanFrequenciesList(_Param_ScanFrequenciesFilePath);

            //[4]:Param_SignalAddr
            _Param_SignalAddr = paramlist[4].Trim();
            _SigInstrumentSession = new VISASession(_Param_SignalAddr, 20);
            if (!_SigInstrumentSession?.Open() ?? false)
                return _ParamsValid = false;

            //[5]:Param_SignalAnalyzerAddr
            _Param_SignalAnalyzerAddr = paramlist[5].Trim();
            _MeasureInstrumentSession = new VISASession(_Param_SignalAnalyzerAddr, 20);
            if (!_MeasureInstrumentSession?.Open() ?? false)
                return _ParamsValid = false;

            return _ParamsValid = true;
        }

        public void SendScpiCmd(IInstrumentSession instrumentSession, String cmd)
        {
            instrumentSession?.WriteString(cmd);
        }


        private Boolean InitScanFrequenciesList(String filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    String? lineContent;
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

        public override Boolean SetParameter(XmlScpiCmd? xmlScpiCmd, String parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            String[]? myname_parameterpair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myname_parameterpair == null)
                return false;
            return AnalyParameter(myname_parameterpair[1]);
        }

        public override BatchTaskPartResult Exec(Double overtimeBySec, out String outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            outMsg = String.Empty;
            if (!_ParamsValid)
            {
                outMsg = "参数错误！";
                return BatchTaskPartResult.ErrorParameter;
            }
            if (_Param_UpdateResponse)
            {
                if (UpdateResponse(out outMsg))
                {
                    return BatchTaskPartResult.Succeed;
                }
                else
                {
                    return BatchTaskPartResult.ErrorFatal;
                }
            }
            else
            {
                #region 频谱仪设置
                _MeasureInstrumentSession?.WriteString(":SYST:PRES");
                _MeasureInstrumentSession?.WriteString(":DISP:WIND:TRAC:Y:PDIV 10DB");
                _MeasureInstrumentSession?.WriteString(":DISP:WIND:TRAC:Y:RLEV 10dBm");
                //_AnalyzerInstrumentSession?.WriteString(":FREQ:SPAN 10 MHZ");
                #endregion
                if (!Directory.Exists(_Param_ScanFrequencyResultDataDir))
                    Directory.CreateDirectory(_Param_ScanFrequencyResultDataDir);
                String filepath = Path.Combine(_Param_ScanFrequencyResultDataDir, _Param_SavePath);
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                foreach (var frequency in _ScanFrequenciesList)
                {
                    String result = "0";
                    Double data = 0;
                    if (_Param_ScanFrequencyResultDataDir == "SignalAnalyer")//频谱仪
                    {
                        _SigInstrumentSession?.WriteString($"FREQ:FIXed {frequency.ToString()}");
                        //_AnalyzerInstrumentSession?.WriteString($":FREQ:CENT {frequency.ToString()}");
                        Thread.Sleep(1000);
                        _MeasureInstrumentSession?.WriteString($":FREQ:TUNE:IMM");//自动调谐
                        Thread.Sleep(6000);
                        _MeasureInstrumentSession?.WriteString($":CALC:MARK1:MAX");
                        Thread.Sleep(1000);
                        _MeasureInstrumentSession?.WriteString($":CALC:MARK1:MAX");
                        _MeasureInstrumentSession?.WriteString($":CALC:MARK1:Y?");
                        result = _MeasureInstrumentSession?.ReadString();
                        Double.TryParse(result, out data);
                    }
                    else if (_Param_ScanFrequencyResultDataDir == "PowerMeter")//功率计
                    {
                        _SigInstrumentSession?.WriteString($"FREQ:FIXed {frequency.ToString()}");
                        Thread.Sleep(1000);
                        _MeasureInstrumentSession?.WriteString($"FETCh?");
                        result = _MeasureInstrumentSession?.ReadString();
                        Double.TryParse(result, out data);
                    }
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(filepath, true))
                        {
                            sw.WriteLine(frequency.ToString() + "," + data);
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
                return BatchTaskPartResult.Succeed;
            }
        }

        /// <summary>
        /// 更新频响设计曲线
        /// </summary>
        public bool UpdateResponse(out String msg)
        {
            msg = String.Empty;
            try
            {
                String[] fileinfos = Directory.GetFiles("Resources");
                Dictionary<Double, Double> data = new Dictionary<Double, Double>();
                String signalanalyerfilepath = _Param_ScanFrequencyResultDataDir + "\\" + _Param_SavePath;
                using (StreamReader sr = new StreamReader(signalanalyerfilepath))
                {
                    while (!sr.EndOfStream)
                    {
                        String[] txts = sr.ReadLine().Split(",");
                        data.Add(Double.Parse(txts[0]), Double.Parse(txts[1]));
                    }
                }
                foreach (String fileInfo in fileinfos)
                {
                    String filename = Path.GetFileName(fileInfo);
                    if (filename.StartsWith("Response") && filename.EndsWith(".txt"))
                    {
                        String filepath = "Resources\\Merged" + filename;

                        Dictionary<Double, Double> data2 = new Dictionary<Double, Double>();
                        using (StreamReader sr = new StreamReader(fileInfo))
                        {
                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    string readtxt = sr.ReadLine();
                                    if (!String.IsNullOrEmpty(readtxt))
                                    {
                                        String[] txts = readtxt.Split(" ");
                                        txts = txts.Where(x => x != " "&&!String.IsNullOrEmpty(x)).ToArray();
                                        data2.Add(Double.Parse(txts[0]), Double.Parse(txts[1]));
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                        if (data2.Count > 0)
                        {
                            if (File.Exists(filepath))
                            {
                                File.Delete(filepath);
                            }
                            foreach (var item in data2)
                            {
                                try
                                {
                                    if (data.TryGetValue(item.Key, out Double param))
                                    {
                                        using (StreamWriter sw = new StreamWriter(filepath, true))
                                        {
                                            var tempvalue = item.Value + param;
                                            sw.WriteLine(item.Key.ToString() + " " + Math.Round(tempvalue, 4));
                                        }
                                    }
                                    else
                                    {
                                        using (StreamWriter sw = new StreamWriter(filepath, true))
                                        {
                                            sw.WriteLine(item.Key.ToString() + " " + Math.Round(item.Value, 4));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.StackTrace + Environment.NewLine + ex.Message;
                return false;
            }
        }

    }
}
