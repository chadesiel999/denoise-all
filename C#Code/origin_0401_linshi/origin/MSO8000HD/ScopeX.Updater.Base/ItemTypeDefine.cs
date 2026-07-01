using System;
using System.Collections.Generic;
using System.Xml;

namespace ScopeX.Updater.Base
{
    /// <summary>
    /// 一个产品的定义涉及到FPGA的操作，使用各个产品编写不同的DLL的方式实现
    /// DLL的名称可以改写。为了隐藏，DLL中必须实现FPGA_Action的类，该类是对FPGA_ActionBase 的重写，其目的是为了规范调用函数名称及参数，也是为了一定的隐藏。
    /// </summary>
    public class ProductDefine
    {
        public string Name
        {
            get;
            set;
        } = "";


        public string FPGA_Action_DllFileName
        {
            get;
            set;
        }
    }

    public enum UpdaterItemType
    {
        Software = 0,
        Fpga = 1,
        Mcu_AnalogChannel = 2,
        Mcu_Keyboard = 3,
        //AWG = 4,
        USBBridge = 5,
        Probe = 6
    }
    public class ItemTypeDefine
    {
        private UpdaterItemType itemType = UpdaterItemType.Software;
        public UpdaterItemType Type
        {
            get { return itemType; }
            set { itemType = value; }
        }
        private string _IDCodeVerify;
        public string IDCodeVerify
        {
            get => _IDCodeVerify;
            set => _IDCodeVerify = value;
        }
        private string itemName;
        public string ItemName
        {
            get => itemName;
            set => itemName = value;
        }
        private int typeID;
        public int TypeID
        {
            get => typeID;
            set => typeID = value;
        }
        private string boradName;
        public string BoradName
        {
            get => boradName;
            set => boradName = value;
        }

        private static string DeleteHexHeader(string source)
        {
            string s = source.ToLower();
            if (s.Substring(0, 2) == "0x")
                return s.Substring(2, s.Length - 2);
            return s;
        }
        public static Dictionary<ProductDefine, List<ItemTypeDefine>> Load(string ConfigurationFile)
        {
            Dictionary<ProductDefine, List<ItemTypeDefine>> productItemDefine = new Dictionary<ProductDefine, List<ItemTypeDefine>>();
            XmlDocument doc = new XmlDocument();
            string configFileName = ConfigurationFile; // AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            doc.Load(configFileName);
            XmlNode node = doc.SelectSingleNode("configuration/Prodects");
            foreach (XmlNode subNode in node.SelectNodes("Prodect"))
            {
                string productName = subNode.Attributes["name"].InnerText;
                ProductDefine productDefine = new ProductDefine();
                productDefine.Name = productName;

                List<ItemTypeDefine> itemTypeDefines = new List<ItemTypeDefine>();
                foreach (XmlNode itemNode in subNode.SelectNodes("ItemDefine"))
                {
                    ItemTypeDefine itemTypeDefine = new ItemTypeDefine();
                    itemTypeDefine.itemType = Enum.Parse<UpdaterItemType>(itemNode.Attributes["ItemType"].InnerText);

                    itemTypeDefine.ItemName = itemNode.Attributes["ItemName"].InnerText;
                    itemTypeDefine.TypeID = int.Parse(itemNode.Attributes["TypeID"].InnerText);
                    itemTypeDefine.boradName = itemNode.Attributes["BoardName"].InnerText;
                    if (itemTypeDefine.itemType == UpdaterItemType.Fpga)
                    {
                        itemTypeDefine.IDCodeVerify = itemNode.Attributes["IDCodeVerify"].InnerText.Trim();
                    }
                    itemTypeDefines.Add(itemTypeDefine);
                }
                productItemDefine.Add(productDefine, itemTypeDefines);
            }

            return productItemDefine;
        }
    }
}
