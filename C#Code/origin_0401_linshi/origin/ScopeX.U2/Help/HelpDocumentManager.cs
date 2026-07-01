using EventBus;
using ScopeX.ComModel;
using SharpGen.Runtime.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ScopeX.U2
{
    internal class HelpDocumentManager
    {
        private const string FILE_NAME = "Index";

        public Boolean IsGetDocIndexSuccess => DocIndexList != null && DocIndexList.Count > 0;

        private IReadOnlyList<(String Key, Int32 Value)> DocIndexList { get; set; }

        private IReadOnlyList<(String Key, Int32 Value)> FormIndexMap { get; set; }

        /// <summary>
        /// Gets or sets the Default.
        /// </summary>
        public static HelpDocumentManager Default
        {
            get;
            internal set;
        }

        public HelpDocumentManager()
        {
            LoadFormIndexMap();
            LoadDocumentInfo();
        }

        private void LoadFormIndexMap()
        {
            var filename = PlatformUIManager.Default.Platform.HelperXmlName;
            var xmlfilepath = $"{typeof(HelpDocumentManager).Namespace}.Help.{filename}";
            Stream sm = (typeof(HelpDocumentManager)).Assembly.GetManifestResourceStream(xmlfilepath);
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(sm);
            }
            catch
            {
                EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new LogEventArgs($"The load of {filename} fails!", LogLevel.Error));
#if DEBUG
                throw;
#else
                    return;
#endif
            }
            try
            {
                var list = new List<(String Key, Int32 Value)>();
                reader.ReadToFollowing("Items");
                XmlReader subreader = reader.ReadSubtree();
                while (subreader.Read())
                {
                    if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Form")
                    {
                        String name;
                        String index;
                        try
                        {
                            name = subreader.GetAttribute("Name");
                            index = subreader.GetAttribute("Index");
                            var ok = Int32.TryParse(index, out var value);
                            list.Add((name, value));
                        }
                        catch
                        {
                            subreader.Close();
                            reader.Close();
                            return;
                        }
                    }
                }
                subreader.Close();
                reader.Close();
                FormIndexMap = list.AsReadOnly();
            }
            catch
            {
                reader.Close();
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs($"The format of {filename} is wrong!", LogLevel.Error));
#if DEBUG
                throw;
#else
                return;
#endif
            }
        }

        public void LoadDocumentInfo()
        {
            var tempfiledic = Application.StartupPath;
            var xMLDocument = new XMLDocument();
            var file = Directory.GetFiles(tempfiledic, "*.xml").ToList().Where(x => x.Contains(FILE_NAME)).FirstOrDefault();
            if (!String.IsNullOrEmpty(file))
            {
                DocIndexList = xMLDocument.LoadXML(file, false);
            }
            else
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Failed to obtain information about the help index XML file", EventBus.LogLevel.Info));
            }
        }

        public string GetCommand(String formName)
        {
            if (!IsGetDocIndexSuccess)
            {
                return "";
            }
            (String key, Int32 Index)? current = null;
            foreach (var item in FormIndexMap)
            {
                if (item.Key == formName)
                {
                    current = item;
                    break;
                }
            }
            if (current == null)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"the help index[{formName}] not find", EventBus.LogLevel.Info));
                return "";
            }
            if (current.Value.Index > DocIndexList.Count)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"the help index[{formName}] not find", EventBus.LogLevel.Info));
                return "";
            }
            return DocIndexList[current.Value.Index].Key;
        }
    }

    class XMLDocument
    {
        /// <summary>
        /// 加载文档索引
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ReadAllNodes">是否读取所有节点 true：读取所有节点，false：读取父节点</param>
        /// <returns></returns>
        public List<(String Key, Int32 Value)> LoadXML(String fileName, Boolean ReadAllNodes = true)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(fileName);
            return FromXML(doc.DocumentElement, ReadAllNodes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RootElement"></param>
        /// <param name="ReadAllNodes">是否读取所有节点 true：读取所有节点，false：读取父节点</param>
        /// <returns></returns>
        private List<(String Key, Int32 Value)> FromXML(XmlElement RootElement, Boolean ReadAllNodes)
        {
            var IndexList = new List<(String Key, Int32 Value)>();
            IndexList.Clear();
            foreach (XmlNode node in RootElement.ChildNodes)
            {
                if (node.Name == "Items")
                {
                    NodesFromXML(IndexList, (XmlElement)node, ReadAllNodes);
                }
            }
            return IndexList;
        }

        /// <summary>
        /// xml转为为nodes
        /// </summary>
        /// <param name="IndexList"></param>
        /// <param name="RootElement"></param>
        /// <param name="ReadAllNodes">是否读取所有节点 true：读取所有节点，false：读取父节点</param>
        private void NodesFromXML(List<(String Key, Int32 Value)> IndexList, XmlElement RootElement, Boolean ReadAllNodes)
        {
            foreach (XmlNode node in RootElement.ChildNodes)
            {
                if (node.Name == "Node")
                {
                    var element = (XmlElement)node;

                    var Name = element.GetAttribute("Name");
                    var PageIndex = Int32.Parse(element.GetAttribute("PageIndex"));
                    if (!IndexList.Contains((Name, PageIndex)))
                    {
                        IndexList.Add((Name, PageIndex));
                    }
                    if (ReadAllNodes)
                    {
                        foreach (XmlNode node2 in element.ChildNodes)
                        {
                            if (node2.Name == "Items")
                            {
                                NodesFromXML(IndexList, (System.Xml.XmlElement)node2, ReadAllNodes);
                            }
                        }
                    }
                }
            }
        }
    }
}
