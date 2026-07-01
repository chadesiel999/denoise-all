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
    /// 该批任务，用于通过调用Matlab的DLL来生成相频特性补偿系数(PFC)：
    /// 1、除控制通道使用4G还是8G（如果存在）外，其他的设置全部通过手动设置，比如信号源输出的频率、幅度，示波器通道的幅度档位，时基档等；
    /// 2、生成的系数都需要上传到示波器并保存，以供下次自动使用。同时本次的系数需要上传到fpga，并生效。
    /// </summary>
    class BatchTask_GeneratePFCCoefficients : BatchTaskBase
    {
        List<Int64> scanFrequencyByHzList = new List<long>();
        public override int MaxStepCount
        {
            get
            {
                int result = 0;
                if (otherParameter.IndexOf("4G") >= 0)
                    result += 100;
                if (otherParameter.IndexOf("8G") >= 0)
                    result += 100;

                return (result + 20 + 20);
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

            result = $"1、通道，如Ch1、Ch2、Ch3、Ch4[{inputParameters[0]}]" + System.Environment.NewLine +
                     $"2、扫频数据存放目录--需要传入dll。以该软件的EXE所在的目录为相对路径。或者是绝对路径。相对路径时，第一个字母必须是“.”[{inputParameters[1]}]" + System.Environment.NewLine +
                     $"3、Matlab Dll File Name[{inputParameters[2]}]" + System.Environment.NewLine +
                     $"4、other Parameters。目前约定是 ADC的个数|;[{inputParameters[3]}]" + System.Environment.NewLine;
            return result;
        }
        private int currChannelID = 0;
        private string matLabDLLName = "";
        private string Scan100TimesResultDataPath = "";
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

            if (parameters.Length < 4)
            {
                ErrorMsg = "参数个数不够4个！";
                return false;
            }
            //1 - 当前通道
            currChannelID = parameters[0].Trim().ToUpper() switch
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
            //2 - 扫频数据存放目录--需要传入dll
            Scan100TimesResultDataPath = AppDomain.CurrentDomain.BaseDirectory + parameters[1];
            if (!Directory.Exists(Scan100TimesResultDataPath))
                Directory.CreateDirectory(Scan100TimesResultDataPath);
            //3 - Matlab Dll File Name
            matLabDLLName = parameters[2];
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName))
            {
                ErrorMsg = $"Matlab Dll[{matLabDLLName}]没有找到！";
                return false;
            }
            //4 - other Parameters;
            if (parameters.Length >= 4)
                otherParameter = parameters[3];

            ErrorMsg = "";
            return true;
        }
        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于扫频,请做好如下准备:\r\n1、确认信号源设置到正确的频率、幅度档；\r\n2、示波器软件运行中，并设置好相应的幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            return true;
        }
        protected override void TaskBody()
        {
            state = BatchTaskState.Running;
            int processStep = 0;
            #region 关闭相关数字处理
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " DebugVariant,";
            scpiCmd += "bEnable_DigitTrigger:true,bEnable_AcqbdInterpolation:false,bEnable_AcqBd_Afc:false,bEnable_AcqBd_Pfc:false,bEnable_CorrectTiAdc:true";
            ourInstrument!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            #endregion
            #region Step2 ScanFrequrency
            string[] oldFiles = Directory.GetFiles(Scan100TimesResultDataPath);
            foreach (string fileName in oldFiles)
                File.Delete(fileName);
            FileStream fs;
            StreamWriter sw;
            if (otherParameter.IndexOf("4G") >= 0)
            {
                ourInstrument.WriteString($":FACT:CHAN{currChannelID + 1}:INP BNC");
                Thread.Sleep(500);
                for (int i = 0; i < 100; i++)
                {
                    processStep = i;
                    updateAction?.Invoke(processStep, $"正在采集第{i}次,", "上步处理OK");
                    Thread.Sleep(500);
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                    if (allChannelData == null)
                    {
                        updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                        return;
                    }
                    string fileName = Scan100TimesResultDataPath + $@"\CH{(currChannelID + 1)}_4G_{i}.txt";
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
            }
            if (otherParameter.IndexOf("8G") >= 0)
            {
                ourInstrument.WriteString($":FACT:CHAN{currChannelID + 1}:INP BNC");
                Thread.Sleep(500);

                for (int i = 0; i < 100; i++)
                {
                    processStep = i;
                    updateAction?.Invoke(processStep, $"正在采集第{i}次,", "上步处理OK");
                    Thread.Sleep(500);
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(ourInstrument, 6_000);
                    if (allChannelData == null)
                    {
                        updateAction?.Invoke(processStep, $"严重错误：数据读取错误", "上步处理OK");
                        return;
                    }
                    string fileName = Scan100TimesResultDataPath + $@"\CH{(currChannelID + 1)}_8G_{i}.txt";
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
                MWNumericArray matlabFunc_GeneratedCoefficients = (MWNumericArray)generateCoefficients_MatlabFunction.Invoke(obj, new MWArray[] { Scan100TimesResultDataPath, otherParameter })!;
                Double[] matlabFunc_GeneratedCoefficients_Double = (double[])((MWNumericArray)matlabFunc_GeneratedCoefficients).ToVector(MWArrayComponent.Real);
                #region Step4 系数转换
                int count = matlabFunc_GeneratedCoefficients_Double.Length < CoefficientsTables.Fixed_PerChannelDataCount ? matlabFunc_GeneratedCoefficients_Double.Length : CoefficientsTables.Fixed_PerChannelDataCount;
                for (int i = 0; i < count; i++)
                    CoefficientsTables.Default[coefficientsType, currChannelID, i] = (Int32)(matlabFunc_GeneratedCoefficients_Double[i]);//1000是放大倍数。目的是用整数表示浮点数。在FPGA中也有此约定
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
