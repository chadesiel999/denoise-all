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
    internal class BatchTaskPartXml_CaliTiAdc : BatchTaskPartXml_CaliWithSignal
    {
        protected override void GenerateContent()
        {
            //参数说明
            //BatchTaskPart_TiAdc_JiHe_MSO7000X btp = new BatchTaskPart_TiAdc_JiHe_MSO7000X();
            //string comment = $"======={btp.GetType().Name} 参数说明===== {Environment.NewLine}" +
            //    $"{btp.ParametersDescription}";
            //_Helper.XmlDoc.AppendChild(_Helper.XmlDoc.CreateComment(comment));

            var solutionElement = _Helper.XmlDoc.AppendChild(_Helper.CreatScope_Solution($"{_OscilloType}程控TiAdc校准方案."));

            //创建设备
            var instrumentComment = solutionElement.AppendChild(_Helper.CreatScope_Instrument());
            instrumentComment!.AppendChild(_Helper.CreatElement_Instrument(_SignalSourceName,
                _SignalSourceName, false, _SignalSourceAddr));
            instrumentComment.AppendChild(_Helper.CreatElement_Instrument(_OscilloType, "示波器", true, ""));

            //Tasks\Task
            var taskElement = solutionElement.AppendChild(_Helper.CreatScope_Tasks("任务列表"))!
                .AppendChild(_Helper.CreatScope_Task("校准各交织方式下各通道TiAdc.", 
                    "该任务是通过程控来校准示波器的各交织方式下各通道TiAdc."));

            //通道循环
            var chanelForElement = taskElement.AppendChild(_Helper.CreatScope_ForLoop("通道循环:",
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Start")!,
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Count")!));
            chanelForElement!.AppendChild(_Helper.CreatElement_Variant(_ChnlKey, (string)ParamSetJson.GetVal(_ChnlKey)!));
            string modelKey = "ModesStart";
            chanelForElement!.AppendChild(_Helper.CreatElement_Variant(modelKey, (string)ParamSetJson.GetVal(modelKey)!));
            chanelForElement.AppendChild(_Helper.CreatElement_Check($"现在即将对“通道@{_ChnlKey}@” 进行校准，" +
                $"请确保将信号源的输出接入到通道！"));

            ChnlForLoopProcess(chanelForElement);
        }

        internal void ChnlForLoopProcess(XmlNode chnlForElement)
        {
            //模式循环
            var modeForElement = chnlForElement.AppendChild(_Helper.CreatScope_ForLoop("模式循环:",
                (string)ParamSetJson.GetVal("ModesForLoop")!.GetVal("Start")!,
                (string)ParamSetJson.GetVal("ModesForLoop")!.GetVal("Count")!));
            string modeKey = "ModeName";
            modeForElement!.AppendChild(_Helper.CreatElement_Variant(modeKey, (string)ParamSetJson.GetVal(modeKey)!));

            //示波器及信号源初始化
            string timeScaleByS = (string)ParamSetJson.GetVal("TiAdcTimeScaleByS")!;
            var groupElement = modeForElement.AppendChild(_Helper.CreatScope_Group("示波器及信号源初始化"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi($"时基档{timeScaleByS}s", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTimeScaleByS")! + " " + timeScaleByS));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"采集模式为：{(string)ParamSetJson.GetVal("TiAdcAcqMode")!}",
                _OscilloType, (string)OscilloScpiJson.GetVal("SetAcquisitionMode")! + " " + (string)ParamSetJson.GetVal("TiAdcAcqMode")!));
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源复位", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("Reset")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源设置输出", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetOutputChnl1")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源正弦", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetScopeSin")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi($"源频率{(string)ParamSetJson.GetVal("TiAdcSourceFrequency")!}Hz",
                    _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetFrequency")! + " " + (string)ParamSetJson.GetVal("TiAdcSourceFrequency")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi($"源阻抗{(string)ParamSetJson.GetVal("SourceImp")!}欧姆",
                    _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc1")! + " " + (string)ParamSetJson.GetVal("SourceImp")!));
                groupElement.AppendChild(_Helper.CreatElement_Scpi($"源电压{(string)ParamSetJson.GetVal("SourceVoltageByV")!}V",
                    _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetVoltageByV")! + " " + (string)ParamSetJson.GetVal("SourceVoltageByV")!));
            }

            //基线，幅度校准常量设置
            JObject paramCommonJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Common")!;

            groupElement = modeForElement.AppendChild(_Helper.CreatScope_Group("基线校准常量设置"));
            JObject paramBaselineJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("BaseLine")!;
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("DACCtrlWord",
                (string)paramBaselineJson.GetVal("DACCtrlWord")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("TiAdcErrorByLsb", 
                (string)paramBaselineJson.GetVal("TiAdcErrorByLsb")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("StaticTimes", 
                (string)paramCommonJson.GetVal("StaticTimes")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("HardwareValidMs",
                (string)paramBaselineJson.GetVal("HardwareValidMs")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("TiAdcMaxWaitMilliseconds",
                (string)paramBaselineJson.GetVal("TiAdcMaxWaitMilliseconds")!));

            groupElement = modeForElement.AppendChild(_Helper.CreatScope_Group("幅度校准常量设置"));
            JObject paramGainJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Gain")!;
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("GainHardwareValidMs", 
                (string)paramGainJson.GetVal("GainHardwareValidMs")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("StaticTimes",
                (string)paramCommonJson.GetVal("StaticTimes")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("TiAdcErrorByPercent",
                (string)paramGainJson.GetVal("TiAdcErrorByPercent")!));
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
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("TiAdcGainAmpMeasureType",
                (string)paramGainJson.GetVal("TiAdcGainAmpMeasureType")!));
            groupElement!.AppendChild(_Helper.CreatElement_ConstVariant("TiAdcMaxWaitMilliseconds",
                (string)paramGainJson.GetVal("TiAdcMaxWaitMilliseconds")!));

            groupElement = modeForElement.AppendChild(_Helper.CreatScope_Group("TiAdc校准常量设置"));
            JObject paramTiAdcJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("TiAdc")!;
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("SourceFrequencyByMHz",
                (string)paramTiAdcJson.GetVal("SourceFrequencyByMHz")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("GainErrorByPercent",
                (string)paramTiAdcJson.GetVal("GainErrorByPercent")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("OffsetErrorByLSB",
                (string)paramTiAdcJson.GetVal("OffsetErrorByLSB")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("PhaseErrorByPs",
                (string)paramTiAdcJson.GetVal("PhaseErrorByPs")!));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("AdjustStep", 
                (string)paramTiAdcJson.GetVal("AdjustStep")!));

            //示波器校准基线和幅度
            groupElement = modeForElement.AppendChild(_Helper.CreatScope_Group("示波器校准基线和幅度"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi(
                $"所有通道,50mV档,低阻", _OscilloType, $":FACTory:ALLSource:APPLy 1,50,0,2,1,0,0,9,0,0", 2));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("存储深度：25K", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetStore25K")!));
            string paramBandwidth = (string)ParamSetJson.GetVal("TiAdcBandwidth")!;
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道1 {paramBandwidth} 带宽限制", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH1Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道2 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH2Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道3 {paramBandwidth} 带宽限制", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH3Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道4 {paramBandwidth} 带宽限制", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH4Bandwidth")! + " " + paramBandwidth, 1));
            
            //基线校准
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement!.AppendChild(_Helper.CreatElement_Scpi("源输出关闭", _SignalSourceName, 
                    (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " OFF"));
            }
            groupElement.AppendChild(_Helper.CreatElement_Func(
                $"通道@{_ChnlKey}@，低阻档，50mv档:基线0Div校准",
                _OscilloType,
                $"BatchTaskPart_CaliChannelBaseLine @{_ChnlKey}@,Low,50,0,@DACCtrlWord@,100,@TiAdcErrorByLsb@," +
                    $"@StaticTimes@,@HardwareValidMs@,@TiAdcMaxWaitMilliseconds@,true", 
                (int)paramBaselineJson.GetVal("TiAdcMaxWaitMilliseconds")!));
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                groupElement.AppendChild(_Helper.CreatElement_Scpi("源输出打开", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " ON"));
            }
            //校准幅度
            groupElement.AppendChild(_Helper.CreatElement_Func(
                $"通道(@{_ChnlKey}@)，低阻档，50mv档:幅度校准",
                _OscilloType, 
                $"BatchTaskPart_CaliChannelGain @{_ChnlKey}@,Low,50,{(string)ParamSetJson.GetVal("SourceVoltageByV")!},Dsa_Adc_Fpga," +
                    $"@GainHardwareValidMs@,@StaticTimes@, @TiAdcErrorByPercent@,@DSAStepPercent@,@ADCStepPercent@," +
                    $"@FPGAYLevelID@,@DSACtrlWord@,@ADCCtrlWord@,@FpgaCtrlWord@,@TiAdcGainAmpMeasureType@",
                (int)paramGainJson.GetVal("TiAdcMaxWaitMilliseconds")!));

            //校准TiAdc
            modeForElement.AppendChild(_Helper.CreatElement_Func(
                $"模式(@{modeKey}@),通道(@{_ChnlKey}@):Tiadc校准", 
                _OscilloType,
                $"BatchTaskPart_TiAdc_JiHe_MSO7000X @{modeKey}@,@{_ChnlKey}@,Low,50,@SourceFrequencyByMHz@," +
                    $"2500,@GainErrorByPercent@,@OffsetErrorByLSB@,@PhaseErrorByPs@,@AdjustStep@", 
                (int)paramCommonJson.GetVal("MaxWaitMilliseconds")!));
            if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                modeForElement!.AppendChild(_Helper.CreatElement_Scpi("源输出关闭", _SignalSourceName, 
                    (string)SignalScpiJson["SetONOrOFF1"]! + " OFF"));
            }

            //校准完成后保存信息
            chnlForElement.AppendChild(_Helper.CreatElement_Func("保存校准数据", _OscilloType,
                "BatchTaskPart_SaveCaliDataToFile TiAdc_PhaseOffsetGain_JiHe_MSO7000X"));
        }

    }
}
