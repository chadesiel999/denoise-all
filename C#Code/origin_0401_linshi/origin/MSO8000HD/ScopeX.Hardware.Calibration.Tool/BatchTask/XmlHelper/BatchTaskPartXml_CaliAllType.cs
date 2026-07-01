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
    internal class BatchTaskPartXml_CaliAllType : BatchTaskPartXml_CaliWithSignal
    {
        protected override void GenerateContent()
        {
            //参数说明
            List<BatchTaskPartBase> btps = new List<BatchTaskPartBase>() 
            {
                 new BatchTaskPart_CaliChannelGain(),
                 new BatchTaskPart_CaliChannelBaseLine(),
                 new BatchTaskPart_CaliOffset()
            };
            btps.ForEach(btp => 
            {
                string comment = $"======={btp.GetType().Name} 参数说明===== {Environment.NewLine}" +
                    $"{btp.ParametersDescription}";
                _Helper.XmlDoc.AppendChild(_Helper.XmlDoc.CreateComment(comment));
            });

            var solutionElement = _Helper.XmlDoc.AppendChild(_Helper.CreatScope_Solution($"{_OscilloType}程控AllType校准项方案."));
            //创建设备
            var instrumentComment = solutionElement.AppendChild(_Helper.CreatScope_Instrument());
            instrumentComment!.AppendChild(_Helper.CreatElement_Instrument(_SignalSourceName,
                _SignalSourceName, false, _SignalSourceAddr));
            instrumentComment.AppendChild(_Helper.CreatElement_Instrument(_OscilloType, "示波器", true, ""));

            //Tasks\Task
            var taskElement = solutionElement.AppendChild(_Helper.CreatScope_Tasks("任务列表"))!
                .AppendChild(_Helper.CreatScope_Task("校准AllType校准项.", 
                    "该任务是通过程控来校准示波器的AllType校准项."));

            //通道循环
            var chnlForElement = taskElement.AppendChild(_Helper.CreatScope_ForLoop("通道循环:",
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Start")!,
                (string)ParamSetJson.GetVal("ChannelsForLoop")!.GetVal("Count")!));
            chnlForElement!.AppendChild(_Helper.CreatElement_Variant(_ChnlKey, (string)ParamSetJson.GetVal(_ChnlKey)!));
            string modelKey = "ModesStart";
            chnlForElement!.AppendChild(_Helper.CreatElement_Variant(modelKey, (string)ParamSetJson.GetVal(modelKey)!));
            chnlForElement.AppendChild(_Helper.CreatElement_Check($"现在即将对“通道@{_ChnlKey}@” 进行校准，" +
                $"请确保将信号源的输出接入到通道！"));

            ChnlForLoopProcess(chnlForElement);
        }

        internal void ChnlForLoopProcess(XmlNode chnlForElement)
        {
            //TiAdc校准
            var groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("TiAdc校准"));
            BatchTaskPartXml_CaliTiAdc tiAdcBtpx = new BatchTaskPartXml_CaliTiAdc()
            {
                SignalJson = this.SignalJson,
                ParamJson = this.ParamJson,
                OscilloscopeJson = this.OscilloscopeJson
            };
            tiAdcBtpx.InitVariables();
            tiAdcBtpx.SetXmlHelper(_Helper);
            tiAdcBtpx.ChnlForLoopProcess(groupElement);

            //增益校准
            groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("增益校准"));
            BatchTaskPartXml_CaliGain gainBtpx = new BatchTaskPartXml_CaliGain()
            {
                SignalJson = this.SignalJson,
                ParamJson = this.ParamJson,
                OscilloscopeJson = this.OscilloscopeJson
            };
            gainBtpx.InitVariables();
            gainBtpx.SetXmlHelper(_Helper);
            gainBtpx.ChnlForLoopProcess(groupElement);

            //基线校准
            groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("基线校准"));
            BatchTaskPartXml_CaliBaseline baselineBtpx = new BatchTaskPartXml_CaliBaseline()
            {
                ParamJson = this.ParamJson,
                OscilloscopeJson = this.OscilloscopeJson
            };
            baselineBtpx.InitVariables();
            baselineBtpx.SetXmlHelper(_Helper);
            baselineBtpx.ChnlForLoopProcess(groupElement);

            //偏置校准
            groupElement = chnlForElement.AppendChild(_Helper.CreatScope_Group("偏置校准"));
            BatchTaskPartXml_CaliOffset offsetBtpx = new BatchTaskPartXml_CaliOffset()
            {
                SignalJson = this.SignalJson,
                ParamJson = this.ParamJson,
                OscilloscopeJson = this.OscilloscopeJson
            };
            offsetBtpx.InitVariables();
            offsetBtpx.SetXmlHelper(_Helper);
            offsetBtpx.ChnlForLoopProcess(groupElement);
        }

    }
}
