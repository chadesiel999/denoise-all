using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    /// <summary>
    /// BatchTaskPart任务的相关Xml文件的JiHe生成器
    /// </summary>
    internal class BatchTaskPartXmlHelperJiHe: BatchTaskPartXmlHelperBase
    {

        //!todo:
        //节点内容参数化

        public BatchTaskPartXmlHelperJiHe()
        {
            // 创建 XmlDocument 对象
            XmlDoc = new XmlDocument();

            // 创建 XML 声明部分（可选）
            XmlDeclaration xmlDeclaration = XmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlDoc.AppendChild(xmlDeclaration);

            // 在根元素下创建注释
            string comment = $"=======region 重要说明====={Environment.NewLine}" +
                $"[1]DriverType 目前只支持 VISA; 我们自己的只支持UU_ETHERNET(JiHe_MSO7000X，JiHe_MSO8000X), UU_USB374(示波表);{Environment.NewLine}" +
                $"[2]CommandType 有2两种:check和scpi，check用于弹出准备就绪与否的对话框;scpi为SCPI命令，需要指定仪器;{Environment.NewLine}" +
                $"[3]AfterWaitSec 表示执行一条scpi命令后，需要等待的时间(以秒为单位);{Environment.NewLine}" +
                $"[4]QueryOvertimeSec 表示执行查询命令时的超时时间(以秒为单位)。对有些操作，仪器端可能需要花费较长的时间后才能查询，比如校准。在此期间如果有比较等待操作，就需要多次执行，直到超时;{Environment.NewLine}" +
                $"[5]基本概念：一个文件称为一个解决方案，一个方案中包含多个任务，一个任务包含一系列的命令;{Environment.NewLine}" +
                $"[6]一个方案中使用的所有仪器在Instrumentations 段定义，在后续的命令中的InstrumentationName 必须使用该段中Name字段 中的内容;{Environment.NewLine}" +
                $"[7]如果使用厂家自用的校准工具，对待校准的仪器（示波器），请在仪器段中对该仪器使用IsOurTargetDso=\"1\"来标识，其地址可以为空;{Environment.NewLine}" +
                $"[8]对使用新的厂家提供的校准工具进行校准时，在Command 段，可以使用程序自定义的函数来校准;{Environment.NewLine}" +
                $"[9]此时，包括3个标签：IsProgramFunc（指示是否该命令是程序函数来完成），ProgramFuncName(程序函数的名称，有程序员提供)，ProgramFuncOverSec(程序函数超时时间，秒);{Environment.NewLine}";
            XmlDoc.AppendChild(XmlDoc.CreateComment(comment));
        }

        public override void SaveXml(string filePath)
        {
            // 保存 XML 文档到文件
            XmlDoc.Save(filePath);
        }
        public override XmlElement CreatScope_ForLoop(string description,string startIndex, string count)
        {
            // 创建ForLoop元素
            XmlElement forLoopElement = XmlDoc.CreateElement("ForeachLoop");

            // 添加属性
            forLoopElement.SetAttribute("Description", description);
            forLoopElement.SetAttribute("StartIndex", startIndex);
            forLoopElement.SetAttribute("Count", count);

            return forLoopElement;
        }

        public override XmlElement CreatScope_Instrument()
        {
             XmlElement InstrumentationsElement = XmlDoc.CreateElement("Instrumentations");
            InstrumentationsElement.SetAttribute("Description", "使用的仪器");
            return InstrumentationsElement;
        }

        public override XmlElement CreatScope_Group(string description)
        {
            XmlElement groupElement = XmlDoc.CreateElement("Group");
            groupElement.SetAttribute("Description", description);
            return groupElement;
        }

        public override XmlElement CreatScope_Solution(string description)
        {
            XmlElement solutionElement = XmlDoc.CreateElement("InstrumentationAutoControlSolution");
            solutionElement.SetAttribute("SolutionDescription", description);
            solutionElement.SetAttribute("version", Assembly.GetExecutingAssembly().GetName().Version!.ToString());
            return solutionElement;
        }

        public override XmlElement CreatScope_Tasks(string description)
        {
            XmlElement tasksElement = XmlDoc.CreateElement("Tasks");
            tasksElement.SetAttribute("Description", description);
            return tasksElement;
        }

        public override XmlElement CreatScope_Task(string description, string importanceTip)
        {
            XmlElement taskElement = XmlDoc.CreateElement("Task");
            taskElement.SetAttribute("Description", description);
            taskElement.SetAttribute("ImportanceTipMessage", importanceTip);
            return taskElement;
        }


        public override XmlElement CreatElement_Instrument(String name, String description, Boolean isOurTargetDso,String addr)
        {
            XmlElement instrumentElement = XmlDoc.CreateElement("Instrumentation");
            instrumentElement.SetAttribute("Name", name);
            instrumentElement.SetAttribute("Description", description);
            instrumentElement.SetAttribute("DriverType", "VISA");
            if(isOurTargetDso)
                instrumentElement.SetAttribute("IsOurTargetDso", "true");
            else
                instrumentElement.SetAttribute("Address", addr);
            instrumentElement.SetAttribute("Timeout", "0");
            return instrumentElement;
        }
        public override XmlElement CreatElement_Check(string description)
        {
            XmlElement checkElement = XmlDoc.CreateElement("Command");
            checkElement.SetAttribute("Description", description);
            checkElement.SetAttribute("CommandType", "check");
            return checkElement;
        }
        public override XmlElement CreatElement_Scpi(String description, String instrumentName, String scpiCmd,int waitSec = 0)
        {
            XmlElement scpiElement = XmlDoc.CreateElement("Command");
            scpiElement.SetAttribute("Description", description);
            scpiElement.SetAttribute("InstrumentationName", instrumentName);
            scpiElement.SetAttribute("CommandType", "scpi");
            scpiElement.SetAttribute("SCPICmd", scpiCmd);
            scpiElement.SetAttribute("AfterWaitSec", waitSec.ToString());
            return scpiElement;
        }

        public override XmlElement CreatElement_Func(String description, String instrumentName, String ProgramFuncName,int waitSec = 0)
        {
            XmlElement funcElement = XmlDoc.CreateElement("Command");
            funcElement.SetAttribute("Description", description);
            funcElement.SetAttribute("InstrumentationName", instrumentName);
            funcElement.SetAttribute("IsProgramFunc", "true");
            funcElement.SetAttribute("ProgramFuncName", ProgramFuncName);
            funcElement.SetAttribute("ProgramFuncOverSec", waitSec.ToString());
            return funcElement;
        }

        public override XmlElement CreatElement_Variant(string name, string valueList)
        {
            XmlElement variantElement = XmlDoc.CreateElement("VarintDefine");
            variantElement.SetAttribute("name", name);
            variantElement.SetAttribute("valuelist", valueList);
            return variantElement;
        }

        public override XmlElement CreatElement_ConstVariant(string name, string value)
        {
            XmlElement ConstVariantElement = XmlDoc.CreateElement("ConstVariantDefine");
            ConstVariantElement.SetAttribute("name", name);
            ConstVariantElement.SetAttribute("value", value);
            return ConstVariantElement;
        }


    }
}
