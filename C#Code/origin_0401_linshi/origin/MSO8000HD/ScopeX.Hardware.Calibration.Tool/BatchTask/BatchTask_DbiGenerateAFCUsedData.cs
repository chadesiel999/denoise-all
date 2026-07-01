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
    class BatchTask_DbiGenerateAFCUsedData : BatchTaskBase
    {
        record FrequencyByHzXScaleByPS_Pair ( UInt64 FrequencyByHz, UInt64 XScaleByPs);
        List<FrequencyByHzXScaleByPS_Pair> scanFrequencyXScalePairList = new List<FrequencyByHzXScaleByPS_Pair>();
        public override int MaxStepCount
        {
            get => scanFrequencyXScalePairList.Count + 20 + 20;//其中20用于装载Matlab的DLL，第二个20用于生成系数
        }
        public override string ResultTipMessage
        {
            get => "文件保存在该程序执行文件所在的目录下的[ScanFrequencyResult]目录下。";
        }
        public override string TaskParameterDescription(string tag)
        {
            string[] inputParameters = new string[20];
            for (int i = 0; i < 20; i++)
                inputParameters[i] = "";
            string[] parameters = tag.Split(',');
            for (int i = 0; i < parameters.Length; i++)
                inputParameters[i] = parameters[i];
            string result = "";

            result = $"1、信号源VISA地址[{inputParameters[0]}]" + System.Environment.NewLine +
                    $"2、通道，如Ch1、Ch2[{inputParameters[1]}]" + System.Environment.NewLine +
                    $"3、扫频频点表文件名称，以该软件的exe所在的目录为相对路径 表示的频点表文件名称，频点表为文本文件，每行以Hz为单位表示的频率数和扫描该频率时的时基档(以pS为单位的数),如：{System.Environment.NewLine}100000000,5000{System.Environment.NewLine}10000000000,2000{System.Environment.NewLine}每行结尾没有“,”号。[{inputParameters[2]}]" + System.Environment.NewLine +
                    $"4、扫频数据存放目录。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[3]}]" + System.Environment.NewLine;
            return result;
        }
        private string sgVisaAddress = "";
        private int currChannelID = 0;
        private string ScanFrequencyResultDataPath = "";
        string[] inputParameters = new string[20];
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            ErrorMsg = "";

            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            this.ourInstrument = instrumentInteract;
            base.Tag = tag;
            string[] parameters = tag.Split(',');

            if (parameters.Length < 4)
            {
                ErrorMsg = "参数个数不够4个！";
                return false;
            }
            //1 - 远程控制之信号源的VISA访问地址
            sgVisaAddress = parameters[0].Trim();
            //2 - 当前通道
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
            string frequencyListFileName = "";
            if (parameters[2].Trim()[0] == '.')
                frequencyListFileName = AppDomain.CurrentDomain.BaseDirectory + parameters[2];
            else
                frequencyListFileName = parameters[2];

            if (!File.Exists(frequencyListFileName))
            {
                ErrorMsg = $"扫频频点数据文件[{parameters[2]}]没有找到！";
                return false;
            }
            scanFrequencyXScalePairList.Clear();
            string[] frequencyByHzList = File.ReadAllLines(frequencyListFileName);
            foreach (string s in frequencyByHzList)
            {
                if (s.Trim() != "")
                {
                    string[] pair = s.Split(',');
                    scanFrequencyXScalePairList.Add(new(ulong.Parse(pair[0]),ulong.Parse(pair[1])));
                }
            }
            //4 - 扫频数据存放目录--需要传入dll
            ScanFrequencyResultDataPath = AppDomain.CurrentDomain.BaseDirectory + parameters[3];
            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);
            //6 - Matlab Dll File Name
            return true;
        }
        IInstrumentSession? sgInstrumentSession = null;//信号源仪器
        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于扫频,请做好如下准备:\r\n1、确认信号源[" + sgVisaAddress + "]已经连接到示波器的通道" + (currChannelID + 1) + "，并打开输出，设置好相应的幅度；\r\n2、示波器软件运行中，并设置好扫频的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            sgInstrumentSession = new VISASession(sgVisaAddress, 500);

            if (!sgInstrumentSession.Open(null))
            {
                MessageBox.Show("对应的仪器不能打开！");
                return false;
            }
            return true;
        }
        protected override void TaskBody()
        {
            state = BatchTaskState.Running;
            int processStep = 0;
            string scpiCmd = "";
            #region 设置基本状态
            //打开通道
            //string scpiCmd = $":CHAN{currChannelID + 1}:DISP ON";
            //ourInstrument!.WriteString(scpiCmd);
            //#region 关闭相关数字处理
            //scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            //scpiCmd += " DebugVariant,";
            //string paramStr = "bEnable_DigitTrigger:true,bEnable_AcqbdInterpolation:false,bEnable_AcqBd_Afc:false,bEnable_AcqBd_Pfc:false,bEnable_CorrectTiAdc:true,bEnable_ProcBd_Average:false";
            //paramStr += $",bEnable_Dbi_IntDelay:true,bEnable_Dbi_AmpFreqCoef:false,bEnable_Dbi_AntiImageCoef:true,bEnable_Dbi_FractionaryDelayCoef:true,bEnable_Dbi_LocalOscillatorCoef:true,bEnable_Dbi_MultiRadioInterpolation:true,bEnable_Dbi_OverlapPhaseFreqDelayCoef:true";
            //paramStr += $",bEnable_Dbi_PhaseFreqCoef:false,bEnable_Dbi_IsSubbandMergeMode:true,iDbi_DebugChannelID:{currChannelID}";
            //scpiCmd += paramStr;
            //ourInstrument!.WriteString(scpiCmd);
            //Thread.Sleep(1000);
            //#endregion
            //设置时基档
            scpiCmd = $":TIM:SCAL {1.0 * 10 / 1000_000_000}";//1000_000_000: ns ==>s,10-10ns
            ourInstrument!.WriteString(scpiCmd);
            ourInstrument?.WriteString($":MEASure:ITEM1:SOURce C{currChannelID + 1}");
            ourInstrument?.WriteString($":MEASure:ITEM1:TYPe AMPL");

            #endregion
            #region Step2 ScanFrequrency
            string[] oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);
            FileStream fsAcquiredData;
            StreamWriter swAcquiredData;

            FileStream fsAmpValueFile;
            StreamWriter swAmpValueFile;
            fsAmpValueFile = new FileStream(ScanFrequencyResultDataPath + $@"\CH{currChannelID + 1}_AmpValues.txt", FileMode.Create);
            swAmpValueFile = new StreamWriter(fsAmpValueFile);
            for (int i = 0; i < scanFrequencyXScalePairList.Count; i++)
            {
                scpiCmd = $":TIM:SCAL {1.0 * scanFrequencyXScalePairList[i].XScaleByPs / 1000_000_000_000}";//1000_000_000_000: ps ==>s
                ourInstrument!.WriteString(scpiCmd);
                processStep = i;
                updateAction?.Invoke(processStep, $"正在处理{i},当前频率={scanFrequencyXScalePairList[i].FrequencyByHz}Hz...", "上步处理OK");
                sgInstrumentSession!.WriteString("SOUR1:FREQ " + scanFrequencyXScalePairList[i].FrequencyByHz.ToString());
                Thread.Sleep(500);
                Thread.Sleep(1000);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                    return;
                }
                string measureResultStr = "";
                while (measureResultStr.Trim() == "")
                {
                    ourInstrument!.WriteString($":MEASure:ITEM1:VAL?");
                    measureResultStr = ourInstrument!.ReadShortString();
                }
                //参数测量结果
                swAmpValueFile.WriteLine($"{scanFrequencyXScalePairList[i].FrequencyByHz / 1_000_000},{measureResultStr}");
                //采样数据
                string acqDatafileName = ScanFrequencyResultDataPath + $@"\CH{(currChannelID + 1)}_{scanFrequencyXScalePairList[i].FrequencyByHz / 1_000_000}MHz.txt";

                fsAcquiredData = new FileStream(acqDatafileName, FileMode.Create);
                swAcquiredData = new StreamWriter(fsAcquiredData);
                for (int k = 0; k < allChannelData[currChannelID].Length; k++)
                {
                    swAcquiredData.WriteLine(allChannelData[currChannelID][k]);
                }
                swAcquiredData.Close();
                fsAcquiredData.Close();
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
            #endregion
            swAmpValueFile.Close();
            fsAmpValueFile.Close();

            processStep += 20;
            updateAction?.Invoke(processStep, $"处理完成", "上步处理OK");
            processStep = MaxStepCount;
            updateAction?.Invoke(processStep, $"处理完成", "上步处理OK");
            state = BatchTaskState.FinishedOK;
        }
    }
}
