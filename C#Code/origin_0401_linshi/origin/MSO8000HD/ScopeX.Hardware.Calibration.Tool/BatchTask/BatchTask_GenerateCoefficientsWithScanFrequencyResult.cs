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
    /// </summary>
    class BatchTask_GenerateCoefficientsWithScanFrequencyResult : BatchTaskBase
    {
        List<Int64> scanFrequencyByHzList = new List<long>();
        public override int MaxStepCount
        {
            get => scanFrequencyByHzList.Count + 20 + 20;//其中20用于装载Matlab的DLL，第二个20用于生成系数
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
                    $"2、通道，如Ch1、Ch2、Ch3、Ch4[{inputParameters[1]}]" + System.Environment.NewLine +
                    $"3、扫频频点表文件名称，以该软件的exe所在的目录为相对路径 表示的频点表文件名称，频点表为文本文件，一行以Hz为单位表示的频率数，每行没有“,”号。[{inputParameters[2]}]" + System.Environment.NewLine +
                    $"4、扫频数据存放目录--需要传入dll。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[3]}]" + System.Environment.NewLine +
                    $"5. 系数类型。如果不是DBI，请输入产品配置表中的Coefficients1、Coefficients2等字样。如果是DBI，请输入TiAdc等字样。[{inputParameters[4]}]" + Environment.NewLine +
                    $"6 - Matlab Dll File Name[{inputParameters[5]}]" + System.Environment.NewLine +
                    $"7、时基档，以ns表示的整数。[{inputParameters[6]}]" + System.Environment.NewLine +
                    $"8、 other Parameters。目前约定是 ADC的个数|总采样率|本振频率(以MHz为单位)|±1。如果是非DBI项目，本振频率为0，最后一个是1。如果是DBI，第一个子带的本振频率是0，其余子带按设计输入，最后一个参数依子带顺序分别为1,-1,1,1。 ;[{inputParameters[7]}]" + System.Environment.NewLine;
            if (inputParameters.Length >= 9)
                result = result + $"9、如果是DBI项目中需要同时存4个子带，输入IsDBI4SubBand，否则没有此参数[{inputParameters[8]}]" + System.Environment.NewLine;
            if (inputParameters.Length >= 10)
                result=result+$"10、DBI的那个子带,从0开始。整数.[{inputParameters[9]}]" + System.Environment.NewLine;
            return result;
        }
        private string sgVisaAddress = "";
        private int currChannelID = 0;
        private string ScanFrequencyResultDataPath = "";
        private string coefficientsFileName = "";
        private Int32 XLevel_ByNs = 10;
        private string matLabDLLName = "";
        private bool bIsDBI = false;
        private int dbiSubIndex = 0;
        private string dbiCoefficientsType = "";
        private CoefficientsTableType coefficientsType = CoefficientsTableType.Coefficients1;
        private string otherParameter = "";
        string[] inputParameters = new string[20];
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
            scanFrequencyByHzList.Clear();
            string[] frequencyByHzList = File.ReadAllLines(frequencyListFileName);
            foreach (string s in frequencyByHzList)
            {
                if (s.Trim() != "")
                    scanFrequencyByHzList.Add(long.Parse(s));
            }
            //4 - 扫频数据存放目录--需要传入dll
            ScanFrequencyResultDataPath = AppDomain.CurrentDomain.BaseDirectory + parameters[3];
            if (!Directory.Exists(ScanFrequencyResultDataPath))
                Directory.CreateDirectory(ScanFrequencyResultDataPath);
            //6 - Matlab Dll File Name
            matLabDLLName = parameters[5];
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName))
            {
                ErrorMsg = $"Matlab Dll[{matLabDLLName}]没有找到！";
                return false;
            }
            //7 -时基，以ns为单位
            XLevel_ByNs = Int32.Parse(parameters[6]);
            //8 - other Parameters;
            if (parameters.Length >= 8)
                otherParameter = parameters[7];
            //9 -是否是DBI
            if (parameters.Length >= 9)
                bIsDBI = parameters[8].ToUpper().Trim() == "ISDBI4SUBBAND" ? true : false;
            if (parameters.Length >= 10)
                dbiSubIndex = int.Parse(parameters[9]);

            ErrorMsg = "";

            //5 - 系数类型
            if (!bIsDBI)
            {
                try
                {
                    coefficientsType = (CoefficientsTableType)Enum.Parse(typeof(CoefficientsTableType), parameters[4]);
                }
                catch
                {
                    ErrorMsg = $"参数类型名称[{parameters[4]}]不正确！";
                    return false;
                }
                coefficientsFileName = AppDomain.CurrentDomain.BaseDirectory + parameters[4]+"_CH"+ (currChannelID+1).ToString() + ".txt";//需要传入DLL
            }
            else
            {
                dbiCoefficientsType = parameters[4].Trim().ToUpper();
                coefficientsFileName = AppDomain.CurrentDomain.BaseDirectory + dbiCoefficientsType + "_CH" + (currChannelID + 1).ToString() +"_Sub"+(dbiSubIndex+1).ToString() + ".txt";//需要传入DLL
            }
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
            //DBI特殊处理
            if (bIsDBI)
            {
                if (dbiCoefficientsType != "TIADC")
                {
                    updateAction?.Invoke(1, $"严重错误！！！", "错误！没有指定DBI系数类型");
                    state = BatchTaskState.FinishedFailed;
                    return;
                }
            }



            int processStep = 0;
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
            scpiCmd += paramStr;
            ourInstrument!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            #endregion
            //设置时基档
            scpiCmd = $":TIM:SCAL {1.0 * XLevel_ByNs / 1000_000_000}";//1000_000_000: ns ==>s
            ourInstrument!.WriteString(scpiCmd);
            #endregion
            #region Step2 ScanFrequrency
            string[] oldFiles = Directory.GetFiles(ScanFrequencyResultDataPath);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);
            FileStream fs;
            StreamWriter sw;
            for (int i = 0; i < scanFrequencyByHzList.Count; i++)
            {
                processStep = i;
                updateAction?.Invoke(processStep, $"正在处理{i},当前频率={scanFrequencyByHzList[i]}Hz...", "上步处理OK");
                sgInstrumentSession!.WriteString("SOUR1:FREQ " + scanFrequencyByHzList[i].ToString());
                Thread.Sleep(500);
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                if (allChannelData == null)
                {
                    updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                    return;
                }
                if (!bIsDBI)
                {
                    string fileName = ScanFrequencyResultDataPath + $@"\CH{(currChannelID + 1)}_{scanFrequencyByHzList[i] / 1_000_000}MHz.txt";
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
                    string fileName = ScanFrequencyResultDataPath + $@"\CH{(currChannelID + 1)}_{scanFrequencyByHzList[i] / 1_000_000}MHz.txt";
                    fs = new FileStream(fileName, FileMode.Create);
                    sw = new StreamWriter(fs);
                    for (int k = 0; k < allChannelData[dbiSubIndex].Length; k++)
                    {
                        sw.WriteLine(allChannelData[dbiSubIndex][k]);
                    }
                    sw.Close();
                    fs.Close();
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
                MethodInfo? generateCoefficients_MatlabFunction = BaseHelper.GetGenerateCoefficientsMatlabDllFunc(type, 2);
                if (generateCoefficients_MatlabFunction == null)
                {
                    state = BatchTaskState.FinishedFailed;
                    return;
                }

                object? obj = Activator.CreateInstance(type);
                if (obj == null)
                {
                    state = BatchTaskState.FinishedFailed;
                    return;
                }
                processStep += 20;
                updateAction?.Invoke(processStep, $"正在处理生成系数...", "上步处理OK");
                MWNumericArray matlabFunc_GeneratedCoefficients = (MWNumericArray)generateCoefficients_MatlabFunction.Invoke(obj, new MWArray[] { ScanFrequencyResultDataPath, otherParameter })!;
                Double[] matlabFunc_GeneratedCoefficients_Double = (double[])((MWNumericArray)matlabFunc_GeneratedCoefficients).ToVector(MWArrayComponent.Real);
                #region Step4 系数转换
                int count = matlabFunc_GeneratedCoefficients_Double.Length < CoefficientsTables.Fixed_PerChannelDataCount ? matlabFunc_GeneratedCoefficients_Double.Length : CoefficientsTables.Fixed_PerChannelDataCount;
                if (!bIsDBI)
                {
                    for (int i = 0; i < count; i++)
                        CoefficientsTables.Default[coefficientsType, currChannelID, i] = (Int32)(matlabFunc_GeneratedCoefficients_Double[i]);//1000是放大倍数。目的是用整数表示浮点数。在FPGA中也有此约定
                }
                else
                {
                    //Dbi专用
                    if (dbiCoefficientsType == "TIADC")
                    {
                        for (int i = 0; i < count; i++)
                            DbiCoefficientsTables.Default[DbiCoefficientsTablesType.TiAdc, i/*dataIndex*/, 0/*bandMode*/, currChannelID, dbiSubIndex, 0/*currFilterBandMode*/] = (Int32)(matlabFunc_GeneratedCoefficients_Double[i]);
                    }
                    else
                    {
                        updateAction?.Invoke(processStep, $"处理完成", "错误！没有指定DBI系数类型");
                        state = BatchTaskState.FinishedFailed;
                        return;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                updateAction?.Invoke(processStep, $"Matlab 处理异常！！", e.ToString());
                state = BatchTaskState.FinishedFailed;
                return;
            }
            #endregion
            processStep += 20;
            #region Step5 上传数据并生效

            updateAction?.Invoke(processStep, $"正在上传系数并生效...", "上步处理OK");
            if (!bIsDBI)
            {
                InstrumentInteract.CaliData_SaveData(ourInstrument, CaliDataType.CoefficientsTables);//在保存时，自动进行了数据传输
                //当前的系数类型
                scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
                scpiCmd += " CoefficientsTableType," + (int)coefficientsType;
                ourInstrument.WriteString(scpiCmd);
            }
            else
            {
                if (dbiCoefficientsType == "TIADC")
                    InstrumentInteract.DbiCoefficientsTable_SaveToFile(ourInstrument, DbiCoefficientsTablesType.TiAdc, 0/*currBandMode*/, currChannelID, dbiSubIndex /*currSubBandIndex*/, 0/*currFilterBandMode*/);//在保存时，自动进行了数据传输
            }

            processStep = MaxStepCount;

            updateAction?.Invoke(processStep, $"处理完成", "上步处理OK");
            #endregion
            state = BatchTaskState.FinishedOK;
        }
    }
}
