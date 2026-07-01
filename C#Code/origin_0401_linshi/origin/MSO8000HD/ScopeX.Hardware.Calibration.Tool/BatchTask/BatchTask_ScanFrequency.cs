using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    /// <summary>
    /// </summary>
    class BatchTask_ScanFrequency : BatchTaskBase
    {
        List<Int64> scanFrequencyByHzList = new List<long>();
        public override int MaxStepCount
        {
            get => scanFrequencyByHzList.Count;
        }
        public override string ResultTipMessage
        {
            get => "文件保存在该程序执行文件所在的目录下的[" + ScanFrequencyResultDataPath + "]目录下。";
        }
        public override string TaskParameterDescription(string tag)
        {
            string[] inputParameters = new string[20];
            for (int i = 0; i < 20; i++)
                inputParameters[i] = "";
            string[] parameters = tag.Split(',');
            for (int i = 0; i < parameters.Length; i++)
                inputParameters[i] = parameters[i];

            return  $"1、信号源VISA地址[{inputParameters[0]}]" + System.Environment.NewLine +
                    $"2、通道，如Ch1、Ch2、Ch3、Ch4[{inputParameters[1]}]" + System.Environment.NewLine +
                    $"3、扫频频点表文件名称，以该软件的exe所在的目录为相对路径 表示的频点表文件名称，频点表为文本文件，一行以Hz为单位表示的频率数，每行没有“,”号。[{inputParameters[2]}]" + System.Environment.NewLine +
                    $"4、扫频数据存放目录--需要传入dll。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[3]}]" + System.Environment.NewLine +
                    $"5、时基档，以ns表示的整数。[{inputParameters[4]}]" + System.Environment.NewLine +
                    $"6、如果是DBI项目同时4个子带，输入IsDBI4SubBand，否则没有此参数[{inputParameters[5]}]";
        }
        private string sgVisaAddress = "";
        private int currChannelID = 0;
        private string ScanFrequencyResultDataPath = "";
        private Int32 XLevel_ByNs = 10;
        private bool bIsDBI = false;
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            string[] parameters = tag.Split(',');

            if (parameters.Length < 4)
            {
                ErrorMsg = $"参数个数必须至少4个！请参考参数描述！";
                return false;
            }
            //1：信号源VISA地址
            sgVisaAddress = parameters[0].Trim();
            //2:通道
            currChannelID = parameters[1].Trim().ToUpper() switch
            {
                "CH1" => 0,
                "CH2" => 1,
                "CH3" => 2,
                "CH4" => 3,
                "CH5" => 4,
                "CH6" => 5,
                "CH7" => 6,
                "CH8" => 7,
                _ => 0,
            };
            //3 - 扫频频点表
            string frequencyListFileName = AppDomain.CurrentDomain.BaseDirectory + parameters[2];
            if (!File.Exists(frequencyListFileName))
            {
                ErrorMsg = $"文件【{frequencyListFileName}】不存在！请参考参数描述及检测文件目录及名称！";
                return false;
            }
            scanFrequencyByHzList.Clear();
            string[] frequencyByHzList = File.ReadAllLines(frequencyListFileName);
            foreach (string s in frequencyByHzList)
            {
                if (s.Trim() != "")
                    scanFrequencyByHzList.Add(long.Parse(s));
            }
            //4 - 扫频数据存放目录--需要传入dll
            if (parameters[3][0] == '.')
                ScanFrequencyResultDataPath = AppDomain.CurrentDomain.BaseDirectory + parameters[3];
            else
                ScanFrequencyResultDataPath = parameters[3];
            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);
            //5 - 时基档
            XLevel_ByNs = Int32.Parse(parameters[4]);
            //6 - IsDBI
            if (parameters.Length >= 6)
                bIsDBI = parameters[5].ToUpper().Trim() == "ISDBI4SUBBAND" ? true : false;
            return true;
        }
        IInstrumentSession? sgInstrumentSession = null;//信号源仪器
        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于扫频,请做好如下准备:\r\n1、确认信号源[" + sgVisaAddress + "]已经连接到示波器的通道" + (currChannelID + 1) + "，并打开输出，设置好相应的幅度；\r\n2、示波器软件运行中，并设置好扫频的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            sgInstrumentSession = new VISASession(sgVisaAddress, 500);

            if (!sgInstrumentSession.Open())
            {
                MessageBox.Show("对应的仪器不能打开！");
                return false;
            }
            return true;
        }
        protected override void TaskBody()
        {
            state = BatchTaskState.Running;

            #region 设置基本状态
            //打开通道
            string scpiCmd = $":CHAN{currChannelID + 1}:DISP ON";
            ourInstrument!.WriteString(scpiCmd);
            #region 关闭相关数字处理
            scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " DebugVariant,";
            string paramStr = "bEnable_DigitTrigger:false,bEnable_AcqbdInterpolation:false,bEnable_AcqBd_Afc:false,bEnable_AcqBd_Pfc:false,bEnable_CorrectTiAdc:false,bEnable_ProcBd_Average:false";
            if (bIsDBI)
            {
                paramStr += $",bEnable_Dbi_IntDelay:false,bEnable_Dbi_AmpFreqCoef:false,bEnable_Dbi_AntiImageCoef:false,bEnable_Dbi_FractionaryDelayCoef:false,bEnable_Dbi_LocalOscillatorCoef:false,bEnable_Dbi_MultiRadioInterpolation:false,bEnable_Dbi_OverlapPhaseFreqDelayCoef:false,bEnable_Dbi_PhaseFreqCoef:false,bEnable_Dbi_IsSubbandMergeMode:false,iDbi_DebugChannelID:{currChannelID}";
            }
            ourInstrument!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            #endregion
            //设置时基档
            scpiCmd = $":TIM:SCAL {1.0 * XLevel_ByNs / 1000_000_000}";//1000_000_000: ns ==>s
            ourInstrument!.WriteString(scpiCmd);
            #endregion
            string[] oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);

            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);

            FileStream fs;
            StreamWriter sw;

            for (int i = 0; i < scanFrequencyByHzList.Count; i++)
            {
                updateAction?.Invoke(i, $"正在处理{i},当前频率={scanFrequencyByHzList[i]}Hz...", "上步处理OK");
                sgInstrumentSession?.WriteString("SOUR1:FREQ " + scanFrequencyByHzList[i].ToString());
                Thread.Sleep(1000);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument!, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(i, $"严重错误：数据读取错误！", "上步处理OK");
                    return;
                }
                if (!bIsDBI)
                {
                    string fileName = ScanFrequencyResultDataPath + $@"\CH{ (currChannelID + 1)}_{(scanFrequencyByHzList[i] / 1_000_000)}MHz.txt";
                    fs = new FileStream(fileName, FileMode.Create);
                    sw = new StreamWriter(fs);
                    for (int k = 0; k < allChannelData[currChannelID].Length; k++)
                    {
                        sw.WriteLine(allChannelData[currChannelID][k]);
                    }
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    //DBI 专用
                    for (int subbandIndex = 0; subbandIndex < allChannelData.Count; subbandIndex++)
                    {
                        if (allChannelData[subbandIndex].Length > 100)
                        {
                            string fileName = ScanFrequencyResultDataPath + $@"\sub{ (subbandIndex + 1)}_{(scanFrequencyByHzList[i] / 1_000_000)}MHz.txt";
                            fs = new FileStream(fileName, FileMode.Create);
                            sw = new StreamWriter(fs);
                            for (int k = 0; k < allChannelData[subbandIndex].Length; k++)
                            {
                                sw.WriteLine(allChannelData[subbandIndex][k]);
                            }
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
                if (cancelTokenSrc != null)
                {
                    try
                    {
                        cancelTokenSrc.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        state = BatchTaskState.Canceled;
                        return;
                    }
                }
            }
            state = BatchTaskState.FinishedOK;
        }
    }
}
