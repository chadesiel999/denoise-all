using ScopeX.Hardware.Calibration.Tool.Base;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Nodes;
using CSScripting;
using ScopeX.ComModel;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    internal class BatchTaskPartXml_CaliGain : BatchTaskPartXml_CaliWithSignal
    {
        protected override void GenerateContent()
        {
            //参数说明
            BatchTaskPartBase btp = new BatchTaskPart_CaliChannelGain();
            string comment = $"======={btp.GetType().Name} 参数说明===== {Environment.NewLine}" +
                $"{btp.ParametersDescription}";
            _Helper.XmlDoc.AppendChild(_Helper.XmlDoc.CreateComment(comment));

            var solutionElement = _Helper.XmlDoc.AppendChild(_Helper.CreatScope_Solution($"{_OscilloType}程控幅度校准方案."));

            //创建设备
            var instrumentComment = solutionElement.AppendChild(_Helper.CreatScope_Instrument());
            instrumentComment!.AppendChild(_Helper.CreatElement_Instrument(
                _SignalSourceName, _SignalSourceName, false, _SignalSourceAddr));
            instrumentComment.AppendChild(_Helper.CreatElement_Instrument(_OscilloType, "示波器", true, ""));

            //Tasks\Task
            var taskElement = solutionElement.AppendChild(_Helper.CreatScope_Tasks("任务列表"))!
                .AppendChild(_Helper.CreatScope_Task("校准通道高阻、低阻全档位增益.", "该任务是通过程控来校准示波器的通道增益。"));

            //通道循环
            var chanelForElement = taskElement.AppendChild(_Helper.CreatScope_ForLoop("通道循环:",
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Start")!,
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Count")!));
            chanelForElement!.AppendChild(_Helper.CreatElement_Variant(_ChnlKey, (string)ParamSetJson.GetVal(_ChnlKey)!));
            chanelForElement.AppendChild(_Helper.CreatElement_Check($"现在即将对“通道@{_ChnlKey}@” " +
                $"进行校准，请确保将信号源的输出接入到通道！"));

            ChnlForLoopProcess(chanelForElement);
        }

        internal void ChnlForLoopProcess(XmlNode chanelForElement)
        {
            //示波器及信号源初始化
            string timeScaleByS = (string)ParamSetJson.GetVal("TimeScaleByS")!;
            var groupElement = chanelForElement.AppendChild(_Helper.CreatScope_Group("示波器及信号源初始化"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi($"时基档{timeScaleByS}s", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTimeScaleByS")! + " " + timeScaleByS));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"采集模式为：{(string)ParamSetJson.GetVal("AcqMode")!}", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetAcquisitionMode")! + " " + (string)ParamSetJson.GetVal("AcqMode")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"触发模式为：{(string)ParamSetJson.GetVal("TriggerType")!}", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTrigTyp")! + " " + (string)ParamSetJson.GetVal("TriggerType")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"触发源设置为：CH@{_ChnlKey}@", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTrigSour")! + $" C@{_ChnlKey}@"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("触发电平设置为：0", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTrigLev")! + " 0"));

            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出1打开", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " ON"));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出2打开", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetONOrOFF2")! + " ON", 1));
            }
            else if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源复位", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("Reset")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出1打开", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetOutputChnl1")!, 1));
            }
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出方波", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetScopeSqu")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi($"源输出{(string)ParamSetJson.GetVal("SourceFrequency")!}Hz",
                    _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetFrequency")! + " " + (string)ParamSetJson.GetVal("SourceFrequency")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("方波设置(SYMM)", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetSquPos")!, 1));
            }


            //阻抗循环
            var impForElement = chanelForElement.AppendChild(_Helper.CreatScope_ForLoop("阻抗循环: 低阻-》高阻",
                (string)ParamSetJson.GetVal("ImpForLoop")!.GetVal("Start")!, 
                (string)ParamSetJson.GetVal("ImpForLoop")!.GetVal("Count")!));
            string sourceIMPedanceName = "SourceIMPedanceName";
            impForElement!.AppendChild(_Helper.CreatElement_Variant(sourceIMPedanceName,
                (string)ParamSetJson.GetVal(sourceIMPedanceName)!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("Rigol_SourceIMPedanceSetting",
                (string)ParamSetJson.GetVal("Rigol_SourceIMPedanceSetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("Fluck_SourceIMPedanceSetting",
                (string)ParamSetJson.GetVal("Fluck_SourceIMPedanceSetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("BatchTaskIMPedanceSetting",
                (string)ParamSetJson.GetVal("BatchTaskIMPedanceSetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("IMPedanceOfALLSourceAPPLySetting",
                (string)ParamSetJson.GetVal("IMPedanceOfALLSourceAPPLySetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("ChnlYScaleStart", 
                (string)ParamSetJson.GetVal("ChnlYScaleStart")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("ChnlYScaleCount", 
                (string)ParamSetJson.GetVal("ChnlYScaleCount")!));

            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                impForElement.AppendChild(_Helper.CreatElement_Scpi($"源OUTPut1:@{sourceIMPedanceName}@", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc1")! + " @Rigol_SourceIMPedanceSetting@", 1));
                impForElement.AppendChild(_Helper.CreatElement_Scpi($"源OUTPut2:@{sourceIMPedanceName}@", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc2")! + " @Rigol_SourceIMPedanceSetting@", 1));
            }
            else if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                impForElement.AppendChild(_Helper.CreatElement_Scpi($"源OUTPut1:@{sourceIMPedanceName}@", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc1")! + " @Fluck_SourceIMPedanceSetting@", 1));
            }

            //档位循环
            var levelForElement = impForElement.AppendChild(_Helper.CreatScope_ForLoop("垂直档位循环:",
                "@ChnlYScaleStart@", "@ChnlYScaleCount@"));
            levelForElement!.AppendChild(_Helper.CreatElement_Variant("SourceAmpVoltByV", 
                (string)ParamSetJson.GetVal("SourceAmpVoltByV")!));
            string inputLevelBymV = "InputLevelBymV";
            levelForElement.AppendChild(_Helper.CreatElement_Variant(inputLevelBymV, (string)ParamSetJson.GetVal(inputLevelBymV)!));
            levelForElement.AppendChild(_Helper.CreatElement_Variant("GainStage", (string)ParamSetJson.GetVal("GainStage")!));

            //基线，幅度校准常量设置
            JObject paramCommonJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Common")!;

            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("基线校准常量设置"));
            JObject paramBaselineJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("BaseLine")!;
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("DACCtrlWord", 
                (string)paramBaselineJson.GetVal("DACCtrlWord")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("GainErrorByLsb",
                (string)paramBaselineJson.GetVal("GainErrorByLsb")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("StaticTimes",
                (string)paramCommonJson.GetVal("StaticTimes")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("HardwareValidMs", 
                (string)paramBaselineJson.GetVal("HardwareValidMs")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("GainMaxWaitMilliseconds", 
                (string)paramBaselineJson.GetVal("GainMaxWaitMilliseconds")!));

            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("幅度校准常量设置"));
            JObject paramGainJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Gain")!;
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("GainHardwareValidMs",
                (string)paramGainJson.GetVal("GainHardwareValidMs")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("StaticTimes",
                (string)paramCommonJson.GetVal("StaticTimes")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("ErrorByPercent", 
                (string)paramGainJson.GetVal("ErrorByPercent")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("DSAStepPercent", 
                (string)paramGainJson.GetVal("DSAStepPercent")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("ADCStepPercent", 
                (string)paramGainJson.GetVal("ADCStepPercent")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("FPGAYLevelID",
                (string)paramGainJson.GetVal("FPGAYLevelID")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("DSACtrlWord",
                (string)paramGainJson.GetVal("DSACtrlWord")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("ADCCtrlWord",
                (string)paramGainJson.GetVal("ADCCtrlWord")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("FpgaCtrlWord",
                (string)paramGainJson.GetVal("FpgaCtrlWord")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("AmpMeasureType",
                (string)paramGainJson.GetVal("AmpMeasureType")!));


            //示波器通道设置
            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("示波器通道设置"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi(
                $"所有通道,@{inputLevelBymV}@mV档,@{sourceIMPedanceName}@,20MHz带宽限制", _OscilloType,
                $":FACTory:ALLSource:APPLy 1,@{inputLevelBymV}@,0,@IMPedanceOfALLSourceAPPLySetting@,3,0,0,9,0,0", 2));
            string paramBandwidth = (string)ParamSetJson.GetVal("Bandwidth")!;
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道1 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH1Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道2 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH2Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道3 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH3Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道4 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH4Bandwidth")! + " " + paramBandwidth, 1));


            //基线校准
            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("先校准基线0Div"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi("源输出1关闭", _SignalSourceName, 
                (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " OFF", 1));
            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出2关闭", _SignalSourceName, 
                    (string)SignalScpiJson.GetVal("SetONOrOFF2")! + " OFF", 1));
            }
            groupElement.AppendChild(_Helper.CreatElement_Func(
                $"@{sourceIMPedanceName}@基线0Div校准、@{inputLevelBymV}@mV档",
                _OscilloType, 
                $"BatchTaskPart_CaliChannelBaseLine @{_ChnlKey}@,@BatchTaskIMPedanceSetting@,@{inputLevelBymV}@,0," +
                    $"@DACCtrlWord@,100,@GainErrorByLsb@,@StaticTimes@,@HardwareValidMs@," +
                    $"@GainMaxWaitMilliseconds@,true",
                (int)paramBaselineJson.GetVal("GainMaxWaitMilliseconds")!));

            groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出1打开", _SignalSourceName, 
                (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " ON", 1));
            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出2打开", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetONOrOFF2")! + " ON", 1));
            }

            //校准幅度
            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("再校准幅度"));
            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                groupElement!.AppendChild(_Helper.CreatElement_Scpi("源输出1:1k,@SourceAmpVoltByV@V", _SignalSourceName, 
                    $"{(string)SignalScpiJson.GetVal("SetAPPLy1")!} 1000,@SourceAmpVoltByV@,0.0,0", 1));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出2:1k,@SourceAmpVoltByV@V", _SignalSourceName, 
                    $"{(string)SignalScpiJson.GetVal("SetAPPLy2")!} 1000,@SourceAmpVoltByV@,0.0,0", 1));
            }
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement!.AppendChild(_Helper.CreatElement_Scpi("源输出:@SourceAmpVoltByV@V", _SignalSourceName, 
                    $"{(string)SignalScpiJson.GetVal("SetVoltageByV")!} @SourceAmpVoltByV@", 1));
            }
            groupElement.AppendChild(_Helper.CreatElement_Func(
                $"通道@{_ChnlKey}@_@{inputLevelBymV}@mV档_@{sourceIMPedanceName}@:幅度校准",
                _OscilloType, 
                $"BatchTaskPart_CaliChannelGain @{_ChnlKey}@,@BatchTaskIMPedanceSetting@,@{inputLevelBymV}@,@SourceAmpVoltByV@," +
                    $"@GainStage@,@GainHardwareValidMs@,@StaticTimes@,@ErrorByPercent@,@DSAStepPercent@,@ADCStepPercent@," +
                    $"@FPGAYLevelID@,@DSACtrlWord@,@ADCCtrlWord@,@FpgaCtrlWord@,@AmpMeasureType@", 
                (int)paramCommonJson.GetVal("MaxWaitMilliseconds")!));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi("源输出1关闭", _SignalSourceName, 
                (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " OFF", 1));
            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出2关闭", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetONOrOFF2")! + " OFF", 1));
            }

            chanelForElement.AppendChild(_Helper.CreatElement_Scpi("源复位", _SignalSourceName, (string)SignalScpiJson.GetVal("Reset")!));

            //校准完成后保存信息
            chanelForElement.AppendChild(_Helper.CreatElement_Func("保存校准时的温度", _OscilloType,
                "BatchTaskPart_SetAnalogChannelTemperatue Gain"));
            chanelForElement.AppendChild(_Helper.CreatElement_Func("保存校准数据", _OscilloType,
                "BatchTaskPart_SaveCaliDataToFile PhyChannel"));
        }
    }
}
