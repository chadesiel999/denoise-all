using ScopeX.ComModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class ProbeCaliData
    {
        public static ProbeCaliData Default=new ProbeCaliData();
        private string filename = $@"{AppDomain.CurrentDomain.BaseDirectory}CaliData\ProbeCaliData.xml";


        private ConcurrentDictionary<string, ProbeCaliDataItem> _CaliBuffer;
        public ProbeCaliDataItem this[ChannelId chl, string probeId]
        {

            get
            {
                string key = $@"{chl}-{probeId}";
                if (_CaliBuffer.ContainsKey(key))
                {
                    return _CaliBuffer[key];

                }
                else
                {
                    //
                    return new ProbeCaliDataItem();
                }
            }
            set
            {
                string key = $@"{chl}-{probeId}";
                _CaliBuffer[key] = value;
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        private ProbeCaliData()
        {
            _CaliBuffer = new ConcurrentDictionary<string, ProbeCaliDataItem>();
            Load();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public Boolean Save()
        {
            //提取类型
            Type type = typeof(ProbeCaliDataItem);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            try
            {
                XmlDocument xmlDoc = new XmlDocument(); // 创建文档
                xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));  // 添加声明
                XmlElement root = xmlDoc.CreateElement("root"); // 创建节点(根)
                xmlDoc.AppendChild(root);   // 添加到末尾
                foreach (var item in _CaliBuffer.Values)
                {
                    XmlElement child = xmlDoc.CreateElement("item");
                    root.AppendChild(child);

                    foreach (PropertyInfo property in properties)
                    {
                        if (property.GetIndexParameters().Length > 0)
                        {
                            continue;
                        }
                        object value = property.GetValue(item);
                        if (null == value)
                        {
                            continue;
                        }

                        string valueTxt = "";
                        valueTxt = value.ToString();

                        child.SetAttribute(property.Name, valueTxt); // 设置属性
                    }
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    xmlDoc.Save(writer);    // 将xmldoc保存到指定的写入器中
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 加载
        /// </summary>
        public Boolean Load()
        {
            if (!File.Exists(filename))
            {//文件不存在 不读取
                return false;
            }

            try
            {//解析XML文件

                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode xn = doc.SelectSingleNode("root");
                XmlNodeList xnl = xn.ChildNodes;

                foreach (var xnitem in xnl)
                {
                    XmlElement xle = (XmlElement)xnitem;
                    var record = new ProbeCaliDataItem();

                    record.GainRatio = double.Parse(xle.GetAttribute("GainRatio"));
                    record.Offset    = double.Parse(xle.GetAttribute("Offset"));
                    record.AtChannel = Enum.Parse<ChannelId>(xle.GetAttribute("AtChannel"));

                    record.ProbeId = xle.GetAttribute("ProbeId");

                    string key = $@"{record.AtChannel}-{record.ProbeId}";
                    _CaliBuffer[key] = record;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// 校准数据项
    /// </summary>
    public class ProbeCaliDataItem
    {

        /// <summary>
        /// 增益校准值
        /// </summary>
        private double _GainRatio = 1;
        public double GainRatio { get { return _GainRatio; } set { _GainRatio = value; } }

        /// <summary>
        /// 偏置校准值
        /// </summary>
        private double _Offset = 0;
        public double Offset { get { return _Offset; } set { _Offset = value; } }


        /// <summary>
        /// 校正通道
        /// </summary>
        ChannelId _Channel = ChannelId.None;
        public ChannelId AtChannel { get { return _Channel; } set { _Channel = value; } }


        /// <summary>
        /// 探头ID
        /// </summary>
        private string _ProbeId = "";
        public string ProbeId { get { return _ProbeId; } set { _ProbeId = value; } }


        public ProbeCaliDataItem() { 

        }
    }
}
