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

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    internal abstract class BatchTaskPartXml_CaliWithSignal : BatchTaskPartXml_Cali
    {
        //信号源Scpi指令Json对象
        internal JObject SignalJson;
        internal JObject SignalScpiJson;

        protected string _SignalSourceName;
        protected string _SignalSourceAddr;

        public override bool InitVariables()
        {
            if(base.InitVariables())
            {
                if(SignalJson == null)
                    return false;

                SignalScpiJson = (JObject)SignalJson.GetVal("Scpi")!;

                _SignalSourceName = (string)SignalJson.GetVal("Info")!.GetVal("Name")!;
                _SignalSourceAddr = (string)SignalJson.GetVal("Info")!.GetVal("VisaAddr")!;
                string ext = Path.GetExtension(_XmlFilePath);
                _XmlFilePath = _XmlFilePath.Replace($"{ext}", $"_{_SignalSourceName}{ext}");

                return true;
            }
            return false;
        }
    }
}
