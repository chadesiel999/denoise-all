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
    /// 供DBI 使用：生成用于生成相频特性系数的采样数据。
    /// </summary>
    class BatchTask_DbiGeneratePFCUsedData : BatchTaskBase
    {
        public override int MaxStepCount
        {
            get => AcquireTimes + 10;
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

            return $"1、当前通道。如CH1、Ch2。[{inputParameters[0]}]" + System.Environment.NewLine +
                    $"2、采集次数。[{inputParameters[1]}]" + System.Environment.NewLine +
                    $"3、扫频数据存放目录。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[2]}]";

        }
        private int currChannelID = 0;
        private int AcquireTimes = 100;
        private string ScanFrequencyResultDataPath = "";
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            string[] parameters = tag.Split(',');

            if (parameters.Length < 3)
            {
                ErrorMsg = "参数个数不能少于3个！";
                return false;
            }
            //1:通道
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
            //2:采集次数
            AcquireTimes = int.Parse(parameters[1].Trim());
            //2 - 扫频数据存放目录
            if (parameters[1][0] == '.')
                ScanFrequencyResultDataPath = AppDomain.CurrentDomain.BaseDirectory + parameters[1];
            else
                ScanFrequencyResultDataPath = parameters[2];
            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);
            return true;
        }
        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于生成供相频特性系数的采集数据,请做好如下准备:\r\n1、确认信号源已经连接到示波器的通道" + (currChannelID + 1) + "，并打开输出，设置好相应的频率和幅度；\r\n2、示波器软件运行中，并设置好扫频的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
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
            paramStr += $",bEnable_Dbi_IntDelay:false,bEnable_Dbi_AmpFreqCoef:false,bEnable_Dbi_AntiImageCoef:false,bEnable_Dbi_FractionaryDelayCoef:false,bEnable_Dbi_LocalOscillatorCoef:false,bEnable_Dbi_MultiRadioInterpolation:false,bEnable_Dbi_OverlapPhaseFreqDelayCoef:false,bEnable_Dbi_PhaseFreqCoef:false,bEnable_Dbi_IsSubbandMergeMode:false,iDbi_DebugChannelID:{currChannelID}";
            ourInstrument!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            #endregion
            //设置时基档
            scpiCmd = $":TIM:SCAL {1.0 * 5 / 1000_000_000}";//1000_000_000: ns ==>s，5ns
            ourInstrument!.WriteString(scpiCmd);
            #endregion
            string[] oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);

            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);

            FileStream fs;
            StreamWriter sw;

            for (int i = 0; i < AcquireTimes; i++)
            {
                updateAction?.Invoke(i, $"正在处理{i}...", "上步处理OK");
                Thread.Sleep(100);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument!, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(i, $"严重错误：数据读取错误！", "上步处理OK");
                    return;
                }
                for (int subbandIndex = 0; subbandIndex < allChannelData.Count; subbandIndex++)
                {
                    if (allChannelData[subbandIndex].Length > 100)
                    {
                        string fileName = ScanFrequencyResultDataPath + $@"\Ch{currChannelID+1}_sub{ (subbandIndex + 1)}_{i}.txt";
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
