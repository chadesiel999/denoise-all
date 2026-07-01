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
    /// 该批任务，用于通过调用Matlab的DLL来生成供TI、幅频特性、插值等处理需要的系数。生成这些系数有共同的流程：
    /// 1、自动控制信号源，扫频，得到扫频结果；
    /// 2、生成的系数都需要上传到示波器并保存，以供下次自动使用。同时本次的系数需要上传到fpga，并生效。
    /// 3、在扫频过程中，可能需要执行一些开关的打开与关闭。
    /// 为此，设计了此通用的调用结构。有如此约定：
    /// 1、所有使用该方法调用的matlab 动态链接库，都必须保持统一的接口约定：返回的是一维的double数据，输入的参数为2个：第一个是扫频结果数据文件存放的绝对路径。
    ///    其中存放的是应该扫频的频点的数据。
    /// 2、扫频频点使用文本文件作为输入。每行一个频率值，频率值得单位是MHz
    /// 3、不管生成的数据多[1 X M]的,还是[M X N]的，不管是double类型，还是复数类型(分实部和虚部)，都为1维的浮点数，并使用C语言的数组存储方式组织。
    ///    究竟每个double是double，还是复数的实部或虚部，与FPGA端的数据使用密切相关，并按其要求进行组织。
    /// 4、返回的数据统一 乘以1000倍。
    /// 5、通过配置ScopeX.Hardware.Calibration.Tool.BatchTaskProcessor.config 文件增加一个新的批任务。
    /// {"Title":"生成幅频特性系数-Ch1","ClassName":"BatchTask_GenerateCoefficientsWithScanFrequencyResult","TipMessage":"","Description":"","Parameters":"USB0::0x0AAD::0x0054::181629::INSTR,Ch1,.\\扫频频率点表\\生成幅频特性系.txt,.\\ScanFrequencyResultData,AmplitudeFrequencyCharacteristic,AmplitudeFrequencyCharacteristic_Generatpr.dll,1|1"}
    ///参数的解析，参见Init 函数
    /// 测试用Matlab DLL 示例代码
    /// function [ data ]=test_dll(data_loc,~)
    /// data=1:1:1000;
    /// %fid=fopen(filter_coe_loc,'w');
    /// %fprintf(fid,'%d\r\n', data);  
    /// %fclose(fid);
    ///    end
    /// </summary>
    class BatchTask_GenerateCoefficientsWithTwiceScanFrequencyResult : BatchTaskBase
    {
        List<Int64> scanFrequencyByHzList8G = new List<long>();
        List<Int64> scanFrequencyByHzList4G = new List<long>();
        public override int MaxStepCount
        {
            get => scanFrequencyByHzList8G.Count + scanFrequencyByHzList4G.Count + 20 + 20;//其中20用于装载Matlab的DLL，第二个20用于生成系数
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

            return  $"1、信号源VISA地址[{inputParameters[0]}]" + System.Environment.NewLine +
                    $"2、通道，如Ch1、Ch2、Ch3、Ch4[{inputParameters[1]}]" + System.Environment.NewLine +
                    $"3、扫频频点表8G[{inputParameters[2]}]" + System.Environment.NewLine +
                    $"4、扫频频点表4G[{inputParameters[3]}]" + System.Environment.NewLine +
                    $"5. 8G扫频数据存放目录--需要传入dll[{inputParameters[4]}]" + System.Environment.NewLine +
                    $"6. 4G扫频数据存放目录--需要传入dll[{inputParameters[5]}]" + System.Environment.NewLine +
                    $"7.系数类型[{inputParameters[6]}]" + System.Environment.NewLine +
                    $"8.Matlab Dll File Name[{inputParameters[7]}]" + System.Environment.NewLine +
                    $"9、时基档，以ns表示的整数。[{inputParameters[8]}]" + System.Environment.NewLine +
                    $"10、 other Parameters[{inputParameters[9]}];";
        }
        private string sgVisaAddress = "";
        private int currChannelID = 0;
        private string ScanFrequencyResultDataPath8G = "";
        private string ScanFrequencyResultDataPath4G = "";
        private string frequencyListFileName8G = "";
        private string frequencyListFileName4G = "";
        private string matLabDLLName = "";
        private Int32 XLevel_ByNs = 10;
        private bool bIsDBI = false;
        private string coefficientsFileName = "";
        private CoefficientsTableType coefficientsType = CoefficientsTableType.Coefficients1;
        private string otherParameter = "";
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            ErrorMsg = "";
            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            this.ourInstrument = instrumentInteract;
            base.Tag = tag;
            string[] parameters = tag.Split(',');
            if (parameters.Length < 6)
            {
                ErrorMsg = "参数个数不够6个！";
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
            //3 - 扫频频点表8G
            frequencyListFileName8G = AppDomain.CurrentDomain.BaseDirectory + parameters[2];
            if (!File.Exists(frequencyListFileName8G))
            {
                ErrorMsg = $"扫频频点数据文件[{parameters[2]}]没有找到！";
                return false;
            }
            //4 - 扫频频点表4G
            frequencyListFileName4G = AppDomain.CurrentDomain.BaseDirectory + parameters[3];
            if (!File.Exists(frequencyListFileName4G))
            {
                ErrorMsg = $"扫频频点数据文件[{parameters[3]}]没有找到！";
                return false;
            }

            scanFrequencyByHzList8G.Clear();
            string[] frequencyByHzList = File.ReadAllLines(frequencyListFileName8G);
            foreach (string s in frequencyByHzList)
            {
                if (s.Trim() != "")
                    scanFrequencyByHzList8G.Add(long.Parse(s));
            }
            scanFrequencyByHzList4G.Clear();
            frequencyByHzList = File.ReadAllLines(frequencyListFileName4G);
            foreach (string s in frequencyByHzList)
            {
                if (s.Trim() != "")
                    scanFrequencyByHzList4G.Add(long.Parse(s));
            }

            //5 - 8G扫频数据存放目录--需要传入dll
            ScanFrequencyResultDataPath8G = AppDomain.CurrentDomain.BaseDirectory + parameters[4];
            if (!Directory.Exists(ScanFrequencyResultDataPath8G))
                Directory.CreateDirectory(ScanFrequencyResultDataPath8G);
            //6 - 4G扫频数据存放目录--需要传入dll
            ScanFrequencyResultDataPath4G = AppDomain.CurrentDomain.BaseDirectory + parameters[5];
            if (!Directory.Exists(ScanFrequencyResultDataPath4G))
                Directory.CreateDirectory(ScanFrequencyResultDataPath4G);

            //7 - 系数类型
            try
            {
                coefficientsType = (CoefficientsTableType)Enum.Parse(typeof(CoefficientsTableType), parameters[6]);
            }
            catch
            {
                ErrorMsg = $"参数类型名称[{parameters[6]}]不正确！";
                return false;
            }
            coefficientsFileName = AppDomain.CurrentDomain.BaseDirectory + parameters[6] + ".txt";//需要传入DLL
            //8 - Matlab Dll File Name
            matLabDLLName = parameters[7];
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName))
            {
                ErrorMsg = $"Matlab Dll[{matLabDLLName}]没有找到！";
                return false;
            }
            //9 - 时基
            XLevel_ByNs = Int32.Parse(parameters[8]);
            //10 - other Parameters;
            if (parameters.Length > 9)
                otherParameter = parameters[9];
            ErrorMsg = "";
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
            int processStep = 0;
            #region 设置为需要状态
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
            scpiCmd += paramStr;
            ourInstrument!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            #endregion
            //设置时基档
            scpiCmd = $":TIM:SCAL {1.0 * XLevel_ByNs / 1000_000_000}";//1000_000_000: ns ==>s
            ourInstrument!.WriteString(scpiCmd);
            #endregion
            #region Step2 ScanFrequrency
            #region Step2.1 ScanFrequrency 8G
            ourInstrument.WriteString($":FACT:CHAN{currChannelID + 1}:INP SMA");
            if (MessageBox.Show("请将信号接入8G通道！准备好了吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                state = BatchTaskState.Canceled;
                return;
            }
            Thread.Sleep(1000);//因为有上面的提示框，已经有一定的延时，不需要再等待太多的延时
            string[] otherParameterList = otherParameter.Split("|");

            string[] oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath8G);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);
            FileStream fs;
            StreamWriter sw;
            for (int i = 0; i < scanFrequencyByHzList8G.Count; i++)
            {
                processStep = i;
                updateAction?.Invoke(processStep, $"正在处理{i},当前频率={scanFrequencyByHzList8G[i]}Hz...", "上步处理OK");
                sgInstrumentSession!.WriteString("SOUR1:FREQ " + scanFrequencyByHzList8G[i].ToString());
                Thread.Sleep(500);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                    return;
                }

                string fileName = ScanFrequencyResultDataPath8G + $@"\CH{(currChannelID + 1)}_{scanFrequencyByHzList8G[i] / 1_000_000}MHz.txt";
                fs = new FileStream(fileName, FileMode.Create);
                sw = new StreamWriter(fs);
                for (int k = 0; k < allChannelData[currChannelID].Length; k++)
                {
                    sw.WriteLine(allChannelData[currChannelID][k]);
                }
                sw.Close();
                fs.Close();
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
            #region Step2.2 ScanFrequrency 4G
            updateAction?.Invoke(processStep, $"即将扫描4G通道的频率...", "上步处理OK");
            ourInstrument.WriteString($":FACT:CHAN{currChannelID + 1}:INP BNC");
            
            if (MessageBox.Show("请将信号接入4G通道！准备好了吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                state = BatchTaskState.Canceled;
                return;
            }
            Thread.Sleep(1000);//因为有上面的提示框，已经有一定的延时，不需要再等待太多的延时
            oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath4G);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);
            for (int i = 0; i < scanFrequencyByHzList4G.Count; i++)
            {
                updateAction?.Invoke(processStep+i, $"正在扫描频率={scanFrequencyByHzList4G[i]}Hz...", "上步处理OK");
                sgInstrumentSession!.WriteString("SOUR1:FREQ " + scanFrequencyByHzList4G[i].ToString());
                Thread.Sleep(500);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                    return;
                }

                string fileName = ScanFrequencyResultDataPath4G + $@"\CH{(currChannelID + 1)}_{scanFrequencyByHzList4G[i] / 1_000_000}MHz.txt";
                fs = new FileStream(fileName, FileMode.Create);
                sw = new StreamWriter(fs);
                for (int k = 0; k < allChannelData[currChannelID].Length; k++)
                {
                    sw.WriteLine(allChannelData[currChannelID][k]);
                }
                sw.Close();
                fs.Close();
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
            processStep+=scanFrequencyByHzList4G.Count;
            #endregion
            #endregion
            #region Step3 计算系数
            try
            {
                updateAction?.Invoke(processStep, $"正在处理装载Matlab动态链接库...", "上步处理OK");

                Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName);

                Type? type = assembly.GetType(Path.GetFileNameWithoutExtension(matLabDLLName) + @".Class1");
                if (type == null)
                {
                    state = BatchTaskState.FinishedFailed;
                    return;
                }
                MethodInfo? generateCoefficients_MatlabFunction = BaseHelper.GetGenerateCoefficientsMatlabDllFunc(type,3);
                if (generateCoefficients_MatlabFunction == null)
                {
                    state = BatchTaskState.FinishedFailed;
                    return;
                }

                object? obj = Activator.CreateInstance(type);
                if (obj==null)
                {
                    state = BatchTaskState.FinishedFailed;
                    return;
                }
                processStep += 20;
                updateAction?.Invoke(processStep, $"正在处理生成系数...", "上步处理OK");
                MWNumericArray matlabFunc_GeneratedCoefficients = (MWNumericArray)generateCoefficients_MatlabFunction.Invoke(obj, 
                    new MWArray[] 
                    { 
                        // file_path1,file_path2,stop_freq1,stop_freq2,fs1,fs2,coe_length1,coe_length2,data_len
                        ScanFrequencyResultDataPath4G, //file_path1
                        ScanFrequencyResultDataPath8G, //file_path2                        
                        otherParameter
                    })!;
                Double[] matlabFunc_GeneratedCoefficients_Double = (double[])((MWNumericArray)matlabFunc_GeneratedCoefficients).ToVector(MWArrayComponent.Real);
                #region Step4 系数转换
                int count = matlabFunc_GeneratedCoefficients_Double.Length < CoefficientsTables.Fixed_PerChannelDataCount ? matlabFunc_GeneratedCoefficients_Double.Length : CoefficientsTables.Fixed_PerChannelDataCount;
                for (int i = 0; i < count; i++)
                    CoefficientsTables.Default[coefficientsType, currChannelID, i] = (Int32)(matlabFunc_GeneratedCoefficients_Double[i]);//1000是放大倍数。目的是用整数表示浮点数。在FPGA中也有此约定
                #endregion
            }
            catch(Exception eee)
            {
                string errorMsg = eee.ToString();
                state = BatchTaskState.FinishedFailed;
                return;
            }
            #endregion
            processStep += 20;
            #region Step5 上传数据并生效

            updateAction?.Invoke(processStep, $"正在上传系数并生效...", "上步处理OK");
            InstrumentInteract.CaliData_SaveData(ourInstrument, CaliDataType.CoefficientsTables);//在保存时，自动进行了数据传输
            //当前的系数类型
            scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " CoefficientsTableType," + (int)coefficientsType;
            ourInstrument.WriteString(scpiCmd);

            processStep = MaxStepCount;

            updateAction?.Invoke(processStep, $"处理完成", "上步处理OK");
            #endregion

            state = BatchTaskState.FinishedOK;
        }
    }
}
