using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper
{
    /// <summary>
    /// 校准类型
    /// </summary>
    public enum CaliType
    {
        AllType,
        TiAdc,
        Gain,
        Baseline,
        Offset,
    }

    /// <summary>
    /// 校准信号源类型
    /// </summary>
    public enum SignalType
    {
        RigolDG4162,
        Fluck9500b,
    }

    internal abstract class BatchTaskPartXml_Cali
    {
        //xml生成器
        protected BatchTaskPartXmlHelperBase _Helper;

        //校准参数Json对象
        internal JObject ParamJson;
        protected JObject ParamSetJson;
        

        //目标（示波器）Scpi指令Json对象
        internal JObject OscilloscopeJson;
        protected JObject OscilloScpiJson;

        protected string _OscilloType;
        protected string _XmlFilePath;

        //Json通用名称
        protected const string _ChnlKey = "Channels";

        /// <summary>
        /// 生成自动校准的Xml文件
        /// </summary>
        internal void GenerateXml()
        {
            if(Init())
            {
                _Helper = new BatchTaskPartXmlHelperJiHe();
                GenerateContent();
                _Helper.SaveXml(_XmlFilePath);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private bool Init()
        {
            if(InitVariables())
            {
                // 创建输出xml的文件夹
                string fileDir = Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(_XmlFilePath));
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                return true;
            }
            return false;
        }

        public void SetXmlHelper(BatchTaskPartXmlHelperBase helper)
        {
            _Helper = helper;
        }

        /// <summary>
        /// 初始化类的变量
        /// </summary>
        /// <returns></returns>
        public virtual bool InitVariables()
        {
            if (ParamJson == null || OscilloscopeJson == null)
                return false;

            ParamSetJson = (JObject)ParamJson.GetVal("SettingParam")!;
            OscilloScpiJson = (JObject)OscilloscopeJson.GetVal("Scpi")!;

            _OscilloType = (string)ParamSetJson.GetVal("OscilloType")!;
            _XmlFilePath = (string)ParamSetJson.GetVal("FilePath")!;
            return true;
        }


        /// <summary>
        /// 生成自动校准的内容
        /// </summary>
        protected abstract void GenerateContent();

    }
}
