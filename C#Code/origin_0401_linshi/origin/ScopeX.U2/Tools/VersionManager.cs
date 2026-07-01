using EventBus;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ScopeX.U2
{
    internal class VersionManager
    {
        public static String KeyboardVersion;
        public static String HardWareVersion;
        private static String _VersionPrefix = "V";
        public static String ExtraVersionInfo = String.Empty;

        public static Boolean CheckVersion(out String msg)
        {
            msg = "";

            // String hardwareversionshow = "";
            var versioninfolist = Reader();
            if (versioninfolist == null || versioninfolist.List == null || versioninfolist.List.Count < 1)
            {
                return false;
            }

            try
            {
                String pcieversion = "";
                var versiondictionary = Core.Hardware.ExportHdFuncs.TryTakeHardwareVersionInfo();
                foreach (var item in versiondictionary)
                {
                    var infolist = versioninfolist.List.Where(a => a.Type == item.Key).ToList();
                    if (infolist.Count < 1)
                    {
                        ComModel.ErrorCode.ErrorType = ErrorType.S_VersionInfo_Info_Error_0010;
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(
                              item.Key.ToString() + ":" + "VersionInfo.xml Error", LogLevel.Error));
                        return false;
                    }
                    else
                    {
                        if (item.Key == HardwareVersionItem.FPGA_Pcie)
                        {
                            pcieversion = $" {ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuJianBanBen_")}：{ReadFireWareVersionFormXml()} "; /*item.Value.Major + "." + item.Value.Minor + "." + item.Value.Build*/
                        }
                        if (item.Key == HardwareVersionItem.HardWare)
                        {
                            //  HardWareVersion = $"{item.Value.Major}.{item.Value.Minor}.{item.Value.Build}.{item.Value.Revision}";
                            HardWareVersion = $"{item.Value.Major}.{item.Value.Minor:D2}.{item.Value.Build:D4}";
                        }
                        if (CheckVersionByType(infolist.First(), item.Value) == false)
                        {
                            var info = infolist.First();
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(
                                item.Key.ToString() + ":" + item.Value.Major + "." + item.Value.Minor + "." + item.Value.Build + " is lower than " +
                               info.Major + "." + info.Minor + "." + info.Build, LogLevel.Error));

                            ComModel.ErrorCode.ErrorType = ErrorType.S_VersionInfo_Lower_0011;
                            Core.Hardware.ExportHdFuncs.ConfigLed(ErrorType.S_VersionInfo_Lower_0011);
                            String mark = "C";
                            if (item.Key == HardwareVersionItem.FPGA_Pcie)
                            {
                                mark = "P";
                            }
                            else if (item.Key == HardwareVersionItem.AnalogChannel_B1 || item.Key == HardwareVersionItem.AnalogChannel_B2)
                            {
                                mark = "C";
                            }
                            else if (item.Key >= HardwareVersionItem.FPGA_AcqBd1 && item.Key <= HardwareVersionItem.FPGA_AcqBd16)
                            {
                                mark = "A";
                            }
                            else if (item.Key == HardwareVersionItem.AWG)
                            {
                                mark = "G";
                            }
                            else if (item.Key == HardwareVersionItem.FPGA_ProcBd)
                            {
                                mark = "PB";
                            }
                            else if (item.Key == HardwareVersionItem.FPGA_S6)
                            {
                                mark = "S";
                            }

                            msg = "(" + mark + ":" + item.Value.Major + "." + item.Value.Minor + "." + item.Value.Build + "," +
                               info.Major + "." + info.Minor + "." + info.Build + ")";
                            return false;
                        }
                    }
                }
                if (PlatformUIManager.Default.Platform.Attribute.SupportKeyBoard)
                {
                    KeyboardChecked(versioninfolist);
                }
                ReadExtraInfoFormXml();
                msg = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RuanJianBanBen_") + ": " + _VersionPrefix + Application.ProductVersion + (!String.IsNullOrWhiteSpace(ExtraVersionInfo) ? $"-{VersionManager.ExtraVersionInfo}" : String.Empty) + pcieversion + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YingJianBanBen_") + ": " + _VersionPrefix + HardWareVersion;
            }
            catch (Exception ex)
            {
                ComModel.ErrorCode.ErrorType = ErrorType.S_Version_Try_Error_0009;
                Core.Hardware.ExportHdFuncs.ConfigLed(ErrorType.S_Version_Try_Error_0009);

                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                return false;
            }

            return true;
        }

        private static void KeyboardChecked(VersionInfoList versioninfolist)
        {
            if (String.IsNullOrEmpty(KeyboardVersion))
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs("KeyboardVersion Is Null Or Empty", LogLevel.Error));
            }
            else
            {
                HardwareVersionInfo hardinfo = GetKeyboardVersionFormString();
                if (hardinfo != null)
                {
                    var infolist = versioninfolist.List.Where(a => a.Type == HardwareVersionItem.Keyboard).ToList();
                    if (infolist.Count < 1)
                    {
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(
                              HardwareVersionItem.Keyboard.ToString() + ":" + "VersionInfo.xml Error", LogLevel.Error));
                    }
                    else
                    {
                        if (CheckVersionByType(infolist.First(), hardinfo) == false)
                        {
                            var info = infolist.First();
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(
                                HardwareVersionItem.Keyboard.ToString() + ":" + hardinfo.Major + "." + hardinfo.Minor + "." +
                                hardinfo.Build + "." + hardinfo.Revision + " is lower than " + info.Major + "." + info.Minor +
                                "." + info.Build + "." + info.Revision, LogLevel.Error));
                        }
                    }
                }
                else
                {

                }

            }
        }

        private static HardwareVersionInfo GetKeyboardVersionFormString()
        {
            HardwareVersionInfo versioninfo = new HardwareVersionInfo();
            var temp = KeyboardVersion.Split(',').ToList();
            var versiontemp = temp.Last().Split(".");

            if (versiontemp.Length == 4)
            {
                if (Int32.TryParse(versiontemp[0], out var major))
                {
                    versioninfo.Major = major;
                }
                else
                {
                    return null;
                }

                if (Int32.TryParse(versiontemp[1], out var minor))
                {
                    versioninfo.Minor = minor;
                }
                else
                {
                    return null;
                }

                if (Int32.TryParse(versiontemp[2], out var build))
                {
                    versioninfo.Build = build;
                }
                else
                {
                    return null;
                }

                if (Int32.TryParse(versiontemp[3], out var revision))
                {
                    versioninfo.Revision = revision;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return versioninfo;
        }

        private static Boolean CheckVersionByType(VersionInfo info, HardwareVersionInfo? value)
        {
            if (Constants.ENABLE_VERSION == false)
                return true;

            if (value?.Major < info.Major)
            {
                return false;
            }
            if (value?.Major == info.Major)
            {
                if (value?.Minor < info.Minor)
                {
                    return false;
                }
                if (value?.Minor == info.Minor && value?.Build < info.Build)
                {
                    return false;
                }
            }
            return true;
        }

        private static void Writer()
        {
            VersionInfoList versioninfolist = new VersionInfoList();
            versioninfolist.List = new List<VersionInfo>();

            for (int i = 0; i < 14; i++)
            {
                VersionInfo versionInfo = new VersionInfo();
                versionInfo.Type = (HardwareVersionItem)i;
                versioninfolist.List.Add(versionInfo);
            }

            XmlSerializer xmlserializer = new XmlSerializer(typeof(VersionInfoList));

            using (XmlTextWriter writer = new XmlTextWriter("./Resources/VersionInfo.xml", new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                xmlserializer.Serialize(writer, versioninfolist);
            }
        }

        private static VersionInfoList Reader()
        {
            try
            {
                using (XmlTextReader sr = new XmlTextReader("./Resources/VersionInfo.xml"))
                {
                    XmlSerializer xmlserializer = new XmlSerializer(typeof(VersionInfoList));

                    return (VersionInfoList)xmlserializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                ComModel.ErrorCode.ErrorType = ErrorType.S_VersionInfo_Load_Error_0008;
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex.Message, LogLevel.Warn));
                return null;
            }
        }

        private static string ReadFireWareVersionFormXml()
        {
            Type type = typeof(AboutForm);
            try
            {
                Stream sm = System.IO.File.OpenRead("./Resources/AboutInfo.xml");
                XmlReader reader = XmlReader.Create(sm);
                reader.ReadToFollowing("AboutInfo");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        String name = reader.Name;
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            switch (name)
                            {
                                case "Type":
                                    break;
                                case "FireWare":
                                    return _VersionPrefix + reader.Value;
                                case "SN":
                                    break;
                                case "Date":
                                    break;
                                case "Extra":
                                    VersionManager.ExtraVersionInfo = reader.Value.Trim();
                                    break;
                            }
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
                //@todo：提示并记录日志
            }
        }

        private static void ReadExtraInfoFormXml()
        {
            Type type = typeof(AboutForm);
            try
            {
                Stream sm = System.IO.File.OpenRead("./Resources/AboutInfo.xml");
                XmlReader reader = XmlReader.Create(sm);
                reader.ReadToFollowing("AboutInfo");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        String name = reader.Name;
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            switch (name)
                            {
                                case "Extra":
                                    VersionManager.ExtraVersionInfo = reader.Value.Trim();
                                    return;

                            }
                        }
                    }
                }
            }
            catch
            {
                //@todo：提示并记录日志
            }
        }
    }

    [XmlRoot]
    public class VersionInfoList
    {
        [XmlElement("List")]

        public List<VersionInfo> List { get; set; }
    }

    [XmlRoot]
    public class VersionInfo
    {
        [XmlElement("Type")]
        public HardwareVersionItem Type { get; set; }
        [XmlElement("Major")]
        public Int32 Major { get; set; }
        [XmlElement("Minor")]

        public Int32 Minor { get; set; }
        [XmlElement("Build")]

        public Int32 Build { get; set; }
        [XmlElement("Revision")]

        public Int32 Revision { get; set; }
    }
}
