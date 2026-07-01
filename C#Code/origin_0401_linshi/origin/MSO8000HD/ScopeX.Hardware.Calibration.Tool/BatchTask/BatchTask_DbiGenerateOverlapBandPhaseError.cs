using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathWorks.MATLAB.NET.Arrays;
using ScopeX.Hardware.Calibration.Data.Base;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    /// <summary>
    /// 该批任务，用于通过调用Matlab的DLL来：
    /// 1、除控制通道使用4G还是8G（如果存在）外，其他的设置全部通过手动设置，比如信号源输出的频率、幅度，示波器通道的幅度档位，时基档等；
    /// 2、生成的系数都需要上传到示波器并保存，以供下次自动使用。同时本次的系数需要上传到fpga，并生效。
    /// </summary>
    class BatchTask_DbiGenerateOverlapBandPhaseError : BatchTaskBase
    {
        List<Int64> scanFrequencyByHzList = new List<long>();
        public override int MaxStepCount
        {
            get
            {
                return (scanFrequencyByHzList.Count + 20 + 20);
            }
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
                     $"2、计算相位差的Matlab Dll Name，需放在本软件exe所在的目录下。[{inputParameters[1]}]" + System.Environment.NewLine +
                     $"2、扫频频点表文件名称，以该软件的exe所在的目录为相对路径 表示的频点表文件名称，频点表为文本文件，一行以Hz为单位表示的频率数，每行没有“,”号。[{inputParameters[2]}]" + System.Environment.NewLine +
                     $"3、数据结果存放目录及文件名称。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[3]}]" + System.Environment.NewLine +
                     $"4、每频点计算次数。[{inputParameters[4]}]" + System.Environment.NewLine;

            return result;
        }
        private string sgVisaAddress = "";
        private string matLabDLLName = "";
        private string resultFileName = "";
        private int perFreqScanTimes = 10;
        string[] inputParameters = new string[20];
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            ErrorMsg = "";

            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            this.ourInstrument = instrumentInteract;
            base.Tag = tag;
            string[] parameters = tag.Split(',');

            if (parameters.Length < 5)
            {
                ErrorMsg = "参数个数不够5个！";
                return false;
            }
            //1：信号源VISA地址
            sgVisaAddress = parameters[0].Trim();
            //2 - Matlab Dll File Name
            matLabDLLName = parameters[1];
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName))
            {
                ErrorMsg = $"Matlab Dll[{matLabDLLName}]没有找到！";
                return false;
            }

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
            //4 - resultFileName;
            resultFileName = parameters[3].Trim();
            //5 - perFreqScanTimes
            perFreqScanTimes = int.Parse(parameters[4].Trim());
            ErrorMsg = "";
            return true;
        }
        IInstrumentSession? sgInstrumentSession = null;//信号源仪器
        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于Dbi交叠带相位多次扫描,请做好如下准备:\r\n1、确认信号源设置到幅度档,并保证输出到被测试仪器；\r\n2、示波器软件运行中，并设置好相应的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
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
            #region 关闭相关数字处理
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            //scpiCmd += " DebugVariant,";
            //scpiCmd += "bEnable_DigitTrigger:true,bEnable_AcqbdInterpolation:false,bEnable_AcqBd_Afc:false,bEnable_AcqBd_Pfc:false,bEnable_CorrectTiAdc:true";
            //ourInstrument!.WriteString(scpiCmd);
            //Thread.Sleep(1000);
            #endregion
            #region Create MatLab dll Instance
            Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName);

            Type? type = assembly.GetType(Path.GetFileNameWithoutExtension(matLabDLLName) + @".Class1");
            if (type == null)
            {
                state = BatchTaskState.FinishedFailed;
                return;
            }
            MethodInfo? curr_MatlabFunction = BaseHelper.GetGenerateCoefficientsMatlabDllFunc(type, 2);
            if (curr_MatlabFunction == null)
            {
                state = BatchTaskState.FinishedFailed;
                return;
            }

            object? matlabDllInstance = Activator.CreateInstance(type);
            if (matlabDllInstance == null)
            {
                state = BatchTaskState.FinishedFailed;
                return;
            }
            #endregion
            if (File.Exists(resultFileName))
                File.Delete(resultFileName);
            FileStream fs;
            StreamWriter sw;
            fs = new FileStream(resultFileName, FileMode.Create);
            sw = new StreamWriter(fs);
            for (int freqIndex = 0; freqIndex < scanFrequencyByHzList.Count; freqIndex++)
            {
                processStep = freqIndex;
                updateAction?.Invoke(processStep, $"正在采集频率点:{scanFrequencyByHzList[freqIndex] / 1000}MHz,", "上步处理OK");
                sgInstrumentSession!.WriteString("SOUR1:FREQ " + scanFrequencyByHzList[freqIndex].ToString());
                Thread.Sleep(500);
                string oneFreqResultLineStr = $"{scanFrequencyByHzList[freqIndex]},";
                for (int times = 0; times < perFreqScanTimes; times++)
                {
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                    if (allChannelData == null)
                    {
                        updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                        return;
                    }
                    double[][] sourceData = new double[4][];
                    for(int i=0;i<4;i++)
                        sourceData[i]= new double[allChannelData[0].Length];
                    for (int i = 0; i < allChannelData[0].Length; i++)
                    {
                        for(int j=0;j<4;j++)
                            sourceData[j][i] = allChannelData[j][i];
                    }
                    //MWNumericArray matlabSrcData1 = new MWNumericArray(sourceData1);
                    //MWNumericArray matlabSrcData2 = new MWNumericArray(sourceData2);
                    //MWNumericArray matlabSrcData3 = new MWNumericArray(sourceData3);
                    //MWNumericArray matlabSrcData4 = new MWNumericArray(sourceData4);
                    for(int loop=0; loop<3;loop++)
                    { 
                        MWNumericArray? dataout_dbi = (MWNumericArray?)curr_MatlabFunction.Invoke(matlabDllInstance, new MWArray[] { new MWNumericArray(sourceData[loop]), new MWNumericArray(sourceData[loop+1])});
                        if (dataout_dbi != null)
                        {
                            Double[] matlabFunc_Result_Double = (double[])((MWNumericArray)dataout_dbi).ToVector(MWArrayComponent.Real);
                            oneFreqResultLineStr += $"{ matlabFunc_Result_Double[0]},";
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
                            sw.Close();
                            fs.Close();
                            return;
                        }
                    }
                }
                sw.WriteLine(oneFreqResultLineStr);
                Thread.Sleep(100);//保证采集到新一次的数据
            }
            sw.Close();
            fs.Close();
            state = BatchTaskState.FinishedOK;
        }
    }
}
