using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ScopeX.ComModel
{
    [Serializable]
    [XmlRoot("Manufacturer")]
    public class ManufacturerInfo
    {
        [XmlElement("ManufacturerLanguage")]
        public List<ManufacturerLanguage> Languages
        {
            get;
            set;
        } = new List<ManufacturerLanguage>();
    }

    [Serializable]
    public class ManufacturerLanguage
    {
        [XmlAttribute("Language")]
        public String Language { get; set; } = String.Empty;

        [XmlAttribute("Info")]
        public String Info { get; set; } = String.Empty;
    }

    public enum ManufacturerAdatper
    {
        Default,
        TekScope,
        Rigol
    }
}
