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
using ScopeX.ComModel;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    internal partial class BatchTaskPartXml_CaliBaseline : BatchTaskPartXml_Cali
    {
        protected override void GenerateContent()
        {
            //参数说明
            BatchTaskPartBase btp = new BatchTaskPart_CaliChannelBaseLine();
            string comment = $"======={btp.GetType().Name} 参数说明===== {Environment.NewLine}" +
                $"{btp.ParametersDescription}";
            _Helper.XmlDoc.AppendChild(_Helper.XmlDoc.CreateComment(comment));

            //创建xml内容
            var solutionElement = _Helper.XmlDoc.AppendChild(_Helper.CreatScope_Solution($"{_OscilloType}程控基线校准方案."));

            //创建设备
            solutionElement.AppendChild(_Helper.CreatScope_Instrument())
                .AppendChild(_Helper.CreatElement_Instrument(_OscilloType, "示波器", true, ""));

            //Tasks\Task
            var taskElement = solutionElement.AppendChild(_Helper.CreatScope_Tasks("任务列表"))
                .AppendChild(_Helper.CreatScope_Task("校准通道高阻、低阻全档位基线.", 
                    "该任务是通过程控来校准示波器的通道基线."));
            taskElement.AppendChild(_Helper.CreatElement_Check("现在即将对通道基线进行校准，请确保将无输入信号接入通道!"));

            ChnlForLoopProcess(taskElement);
        }

        internal void ChnlForLoopProcess(XmlNode chanelForElement)
        {
            //示波器通道设置
            var groupElement = chanelForElement.AppendChild(_Helper.CreatScope_Group("示波器状态初始化设置"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("时基档1ms", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetTimeScaleByS")! + " " + (string)ParamSetJson.GetVal("TimeScaleByS")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("打开高分辨率模式", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetAcquisitionMode")! + " " + (string)ParamSetJson.GetVal("AcqMode")!));
            groupElement.AppendChild(_Helper.CreatElement_Scpi("存储深度：25K", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetStore25K")!));

            //阻抗循环
            var impForElement = chanelForElement.AppendChild(_Helper.CreatScope_ForLoop("阻抗循环: 低阻-》高阻",
                (string)ParamSetJson.GetVal("ImpForLoop")!.GetVal("Start")!, 
                (string)ParamSetJson.GetVal("ImpForLoop")!.GetVal("Count")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("SourceIMPedanceName",
                (string)ParamSetJson.GetVal("SourceIMPedanceName")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("BatchTaskIMPedanceSetting", 
                (string)ParamSetJson.GetVal("BatchTaskIMPedanceSetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("IMPedanceOfALLSourceAPPLySetting",
                (string)ParamSetJson.GetVal("IMPedanceOfALLSourceAPPLySetting")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("ChnlYScaleStart", 
                (string)ParamSetJson.GetVal("ChnlYScaleStart")!));
            impForElement.AppendChild(_Helper.CreatElement_Variant("ChnlYScaleCount", 
                (string)ParamSetJson.GetVal("ChnlYScaleCount")!));

            //档位循环
            var levelForElement = impForElement.AppendChild(_Helper.CreatScope_ForLoop("垂直档位循环: 小-》大",
                "@ChnlYScaleStart@", "@ChnlYScaleCount@"));
            levelForElement.AppendChild(_Helper.CreatElement_Variant("inputLevelBymV", 
                (string)ParamSetJson.GetVal("InputLevelBymV")!));
            levelForElement.AppendChild(_Helper.CreatElement_Variant("BaselineErrorByLsb", 
                (string)ParamSetJson.GetVal("BaselineErrorByLsb")!));

            //示波器通道设置
            groupElement = levelForElement.AppendChild(_Helper.CreatScope_Group("示波器通道设置"));
            groupElement.AppendChild(_Helper.CreatElement_Scpi(
                "所有通道,@inputLevelBymV@mV档,@SourceIMPedanceName@,20MHz带宽限制", _OscilloType,
                ":FACTory:ALLSource:APPLy 1,@inputLevelBymV@,0,@IMPedanceOfALLSourceAPPLySetting@,3,0,0,9,0,0"));
            string paramBandwidth = (string)ParamSetJson.GetVal("Bandwidth")!;
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道1 {paramBandwidth} 带宽限制", _OscilloType, 
                (string)OscilloScpiJson.GetVal("SetCH1Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道2 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH2Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道3 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH3Bandwidth")! + " " + paramBandwidth));
            groupElement.AppendChild(_Helper.CreatElement_Scpi($"通道4 {paramBandwidth} 带宽限制", _OscilloType,
                (string)OscilloScpiJson.GetVal("SetCH4Bandwidth")! + " " + paramBandwidth, 1));

            //执行BatchTaskPart
            JObject paramCommonJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("Common")!;
            JObject paramBaselineJson = (JObject)ParamJson.GetVal("FuncParam")!.GetVal("BaseLine")!;
            var destDivs = paramBaselineJson.GetVal("DestDiv")!.ToObject<int[]>();
            for (int i = 0; i < destDivs!.Length; i++)
            {
                int ctrlWordMin = (int)paramBaselineJson.GetVal("CtrlWord")!.GetVal("Min")![i]!;
                int ctrlWordMax = (int)paramBaselineJson.GetVal("CtrlWord")!.GetVal("Max")![i]!;
                int hardwareValidMs = (int)paramBaselineJson.GetVal("HardwareValidMs")!;
                string caliChnls = (string)paramBaselineJson.GetVal("CaliChnls")!;
                int staticTimes = (int)paramCommonJson.GetVal("StaticTimes")!;
                int maxWaitMilliseconds = (int)paramCommonJson.GetVal("MaxWaitMilliseconds")!;

                levelForElement.AppendChild(_Helper.CreatElement_Func(
                    $"@inputLevelBymV@mV档、@SourceIMPedanceName@:基线{destDivs[i]}Div校准",_OscilloType,
                    $"BatchTaskPart_CaliChannelBaseLine {caliChnls},@BatchTaskIMPedanceSetting@,@inputLevelBymV@," +
                        $"{destDivs[i]},{ctrlWordMin},{ctrlWordMax},50,@BaselineErrorByLsb@,{staticTimes}," +
                        $"{hardwareValidMs},{maxWaitMilliseconds},false", 
                    40));
            }

            //校准完成后保存信息
            chanelForElement.AppendChild(_Helper.CreatElement_Func("读取设置基线校准完成后通道温度", _OscilloType,
                "BatchTaskPart_SetAnalogChannelTemperatue BaseLine"));
            chanelForElement.AppendChild(_Helper.CreatElement_Func("保存校准数据", _OscilloType,
                "BatchTaskPart_SaveCaliDataToFile PhyChannel"));
        }

    }
}
