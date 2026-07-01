using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;

namespace ScopeX.ComModel
{
    /// <summary>    
    /// 支持AppSettings和ConnectionStrings节点，如需其他节点请自行扩展
    /// </summary>    
    public class AppConfigureHelper
    {
        //AppSettings     
        public static Hashtable AppSettings => GetKeyValuePair("appSettings", "key", "value");

        //ConnectionStrings   
        public static Hashtable ConnectionStrings => GetKeyValuePair("connectionStrings", "name", "connectionString");

        /// <summary>
        /// 根据节点名，子节点名，获取指定值           
        /// </summary>           
        /// <param name="section">对应节点</param>           
        /// <param name="key">key或者name</param>           
        /// <param name="value">key或者name的值</param>           
        /// <returns>key或者name对应value值</returns>           
        private static Hashtable GetKeyValuePair(String section, String key, String value)
        {
            Hashtable settings = new(5);

            var asm = typeof(AppConfigureHelper).Assembly;

            //获取当前DLL的路径，添加 .config 后缀，得到配置文件路径  
            String cfgpath = asm.Location + ".config";

            if (!File.Exists(cfgpath))
            {
                //获取进程可执行文件的路径，添加 .config 后缀，得到配置文件路径
                cfgpath = Assembly.GetEntryAssembly()?.Location + ".config";
                if (!File.Exists(cfgpath))
                    return settings;
            }

            var cfg = new XmlDocument();
            FileStream? fs = null;
            try
            {
                fs = new FileStream(cfgpath, FileMode.Open, FileAccess.Read);
                cfg.Load(new XmlTextReader(fs));
                XmlNodeList nodes = cfg.GetElementsByTagName(section);
                foreach (XmlNode node in nodes)
                {
                    foreach (XmlNode subnode in node.ChildNodes)
                    {
                        XmlAttributeCollection? attributes = subnode.Attributes;
                        if (attributes?[key] is not null)
                        {
                            settings.Add(attributes[key]!.Value, attributes[value]?.Value);
                        }
                    }
                }
            }
            //catch (FileNotFoundException es)
            //{
            //    throw es;
            //}
            finally
            {
                fs?.Close();
            }
            return settings;
        }
        
        //public static void SetAppSettings(String key, String value)
        //{
        //    SetNameAndValue("appSettings", "key", key, value);
        //}
       
        //public static void SetConnectionStrings(String key, String value)
        //{
        //    SetNameAndValue("connectionStrings", "name", key, value);
        //}

        //private static void SetNameAndValue(String section, String key, String keyname, String value)
        //{
        //    String cfgpath = typeof(AppConfigureHelper).Assembly.Location + ".config";
                    
        //    try
        //    {
        //        XmlDocument cfgfile = new();
        //        cfgfile.Load(cfgpath);
        //        XmlNodeList nodes = cfgfile.GetElementsByTagName(section);
        //        foreach (XmlNode node in nodes)
        //        {
        //            foreach (XmlNode subnode in node.ChildNodes)
        //            {
        //                XmlAttributeCollection? attributes = subnode.Attributes;
        //                if (attributes != null)
        //                {
        //                    if (attributes.GetNamedItem(key)?.InnerText == keyname)
        //                    {
        //                        var v = attributes.GetNamedItem("value");
        //                        if (v is not null)
        //                            v.InnerText = value;
        //                    }
        //                }
        //            }
        //        }
        //        cfgfile.Save(cfgpath);
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        throw;
        //    }
        //}

        
    }
}
