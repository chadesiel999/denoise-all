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
    internal class BatchTaskPartXml_CaliOffset : BatchTaskPartXml_CaliWithSignal
    {
        protected override void GenerateContent()
        {
            //参数说明
            BatchTaskPart_CaliOffset btp = new BatchTaskPart_CaliOffset();
            string comment = $"======={btp.GetType().Name} 参数说明===== {Environment.NewLine}" +
                $"{btp.ParametersDescription}";
            _Helper.XmlDoc.AppendChild(_Helper.XmlDoc.CreateComment(comment));

            var solutionElement = _Helper.XmlDoc.AppendChild(_Helper.CreatScope_Solution($"{_OscilloType}程控偏置校准方案."));

            //创建设备
            var instrumentComment = solutionElement.AppendChild(_Helper.CreatScope_Instrument());
            instrumentComment!.AppendChild(_Helper.CreatElement_Instrument(
                _SignalSourceName, _SignalSourceName, false, _SignalSourceAddr));
            instrumentComment.AppendChild(_Helper.CreatElement_Instrument(_OscilloType, "示波器", true, ""));

            //Tasks\Task
            var taskElement = solutionElement.AppendChild(_Helper.CreatScope_Tasks("任务列表"))!
                .AppendChild(_Helper.CreatScope_Task("校准通道的偏置.", "该任务是通过程控来校准示波器的偏执."));
            
            //通道循环
            var chnlForElement = taskElement.AppendChild(_Helper.CreatScope_ForLoop("通道循环:",
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Start")!,
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Count")!));
            chnlForElement!.AppendChild(_Helper.CreatElement_Variant(_ChnlKey, (string)ParamSetJson.GetVal(_ChnlKey)!));
            chnlForElement.AppendChild(_Helper.CreatElement_Check($"现在即将对“通道@{_ChnlKey}@” 进行校准，" +
                $"请确保将信号源的输出接入到通道！"));

            ChnlForLoopProcess(chnlForElement);

        }

        internal void ChnlForLoopProcess(XmlNode chnlForElement)
        {
            //示波器及信号源初始化
            var groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("示波器及信号源初始化"));
            string timeScaleByS = (string)ParamSetJson.GetVal("TimeScaleByS")!;
            groupElement!.AppendChild(_Helper.CreatElement_Scpi($"时基档{timeScaleByS}s", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTimeScaleByS")! + " " + timeScaleByS));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"采集模式为：{(string)ParamSetJson.GetVal("AcqMode")!}", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetAcquisitionMode")! + " " + (string)ParamSetJson.GetVal("AcqMode")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("存储深度：25K", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetStore25K")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("源复位", _SignalSourceName, (string)SignalScpiJson.GetVal("Reset")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("源DC", _SignalSourceName, (string)SignalScpiJson.GetVal("SetScopeDC")!));

            //阻抗循环
            var impForElement = chnlForElement.AppendChild(_Helper.CreatScope_ForLoop("阻抗循环: 低阻-》高阻",
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
            impForElement.AppendChild(_Helper.CreatElement_Variant("OffsetChnlYScaleStart", 
                (string)ParamSetJson.GetVal("OffsetChnlYScaleStart")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("OffsetChnlYScaleCount", 
                (string)ParamSetJson.GetVal("OffsetChnlYScaleCount")!));

            if (_SignalSourceName == SignalType.RigolDG4162.ToString())
            {
                impForElement.AppendChild(_Helper.CreatElement_Scpi($"源OUTPut1:@{sourceIMPedanceName}@", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc1")! + " @Rigol_SourceIMPedanceSetting@"));
            }
            else if (_SignalSourceName == SignalType.Fluck9500b.ToString())
            {
                impForElement.AppendChild(_Helper.CreatElement_Scpi($"源OUTPut1:@{sourceIMPedanceName}@", _SignalSourceName,
                    (string)SignalScpiJson.GetVal("SetIMPedanc1")! + " @Fluck_SourceIMPedanceSetting@"));
            }

            //档位循环
            var levelForElement = impForElement.AppendChild(_Helper.CreatScope_ForLoop("垂直档位循环:",
                "@OffsetChnlYScaleStart@", "@OffsetChnlYScaleCount@"));
            string sourceAmpVoltByV = "OffsetSourceAmpVoltByV";
            levelForElement!.AppendChild(_Helper.CreatElement_Variant(sourceAmpVoltByV,
                (string)ParamSetJson.GetVal(sourceAmpVoltByV)!));
            string inputLevelBymV = "OffsetInputLevelBymV";
            levelForElement.AppendChild(_Helper.CreatElement_Variant(inputLevelBymV, 
                (string)ParamSetJson.GetVal(inputLevelBymV)!));

            //偏置校准常量设置
            JObject paramCommonJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Common")!;

            groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("偏置校准常量设置"));
            groupElement.AppendChild(_Helper.CreatElement_ConstVariant("MaxWaitMilliseconds", 
                (string)paramCommonJson.GetVal("MaxWaitMilliseconds")!));

            //示波器通道设置
            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("示波器通道设置"));
            groupElement!.AppendChild(_Helper.CreatElement_Scpi(
                $"所有通道,@{inputLevelBymV}@mV档,@{sourceIMPedanceName}@,偏置@{sourceAmpVoltByV}@V,20MHz带宽限制", _OscilloType,
                $":FACTory:ALLSource:APPLy 1,@{inputLevelBymV}@,0,@IMPedanceOfALLSourceAPPLySetting@,3,0,0,9,0,0", 2));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("通道1 调整偏执", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH1Bias")! + $" @{sourceAmpVoltByV}@"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("通道2 调整偏执", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH2Bias")! + $" @{sourceAmpVoltByV}@"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("通道3 调整偏执", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH3Bias")! + $" @{sourceAmpVoltByV}@"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("通道4 调整偏执", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH4Bias")! +$" @{sourceAmpVoltByV}@"));
            string paramBandwidth = (string)ParamSetJson.GetVal("Bandwidth")!;
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道1 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH1Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道2 {paramBandwidth} 带宽限制",_OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH2Bandwidth")! +  " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道3 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH3Bandwidth")! +  " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道4 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH4Bandwidth")! +  " " + paramBandwidth));
                
            //校准偏置
            levelForElement!.AppendChild(_Helper.CreatElement_Scpi($"源输出电压@{sourceAmpVoltByV}@V"
                , _SignalSourceName, (string)SignalScpiJson.GetVal("SetVoltageByV")! + $" @{sourceAmpVoltByV}@"));
            levelForElement!.AppendChild(_Helper.CreatElement_Scpi("源输出打开"
                , _SignalSourceName, (string)SignalScpiJson.GetVal("SetONOrOFF1")! + " ON"));
            levelForElement.AppendChild(_Helper.CreatElement_Func(
                $"通道(@{_ChnlKey}@),垂直档(@{inputLevelBymV}@V),阻抗(@{sourceIMPedanceName}@):偏置校准"
                ,_OscilloType, 
                $"BatchTaskPart_CaliOffset @{_ChnlKey}@,@BatchTaskIMPedanceSetting@,@{inputLevelBymV}@," +
                    $"@{sourceAmpVoltByV}@,@MaxWaitMilliseconds@", 
                40));

            //校准完成后保存信息
            chnlForElement.AppendChild(_Helper.CreatElement_Func("保存通道校准数据", _OscilloType,
                "BatchTaskPart_SaveCaliDataToFile PhyChannel"));
        }
    }
}
