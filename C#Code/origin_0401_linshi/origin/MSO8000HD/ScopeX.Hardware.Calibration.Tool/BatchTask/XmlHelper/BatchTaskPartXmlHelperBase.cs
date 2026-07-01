using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    /// <summary>
    /// BatchTaskPart任务的相关Xml文件的生成器
    /// </summary>
    internal abstract class BatchTaskPartXmlHelperBase
    {
        internal XmlDocument XmlDoc;    //XmlDocument 对象

        /// <summary>
        /// 保存Xml文件
        /// </summary>
        /// <param name="filePath"></param>
        public abstract void SaveXml(string filePath);
        public abstract XmlElement CreatScope_ForLoop(string description,string startIndex, string count);
       public abstract XmlElement CreatScope_Instrument();
       public abstract XmlElement CreatScope_Group(string description);
        public abstract XmlElement CreatScope_Solution(string description);

        public abstract XmlElement CreatScope_Tasks(string description);
        public abstract XmlElement CreatScope_Task(string description,string importanceTip);

        public abstract XmlElement CreatElement_Instrument(String name, String description, Boolean isOurTargetDso,String addr);
       public abstract XmlElement CreatElement_Check(string description);
       public abstract XmlElement CreatElement_Scpi(String description, String instrumentName, String scpiCmd,int waitSec = 0);
       public abstract XmlElement CreatElement_Func(String description, String instrumentName, String ProgramFuncName,int waitSec = 0);

        public abstract XmlElement CreatElement_Variant(string name, string valueList);
        public abstract XmlElement CreatElement_ConstVariant(string name, string value);


    }
}
