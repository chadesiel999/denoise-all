using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class ServerDomainConstants
    {
        public static bool HardwareAttached = false;
        public static AnaChnlType AnaChnlType;
        public static ProductType ProductType = ProductType.Base;
        public static Int32 AdcBits = 12;
        public static Int32 AdcCount = 4;
        public static Int32 AnalogChannelCount = 4;
        public static Int32 LAChannelCount = 16;
        public static Int32 PerAnaChannelDataCount = 10_000;
        public static Int32 PerAdcCoreDataCount = 1_000;
        public static Int32 PerAnaChannelAdcCount = CaliConstants.Fixed_PerChannelMergeAdcMaxCount;
        public static Int32 PerAdcCoreCount = CaliConstants.Fixed_PerAdcCoreMaxCount;
        public static double SAMPS_PER_YDIV = Constants.SAMPS_PER_YDIV;
        public static Int32 AnalogChannelType = 0;
        public static Int32 AnalogChannelLevelMin = 0;
        public static Int32 AnalogChannelLevelMax = 0;
        public static List<CaliDataType>? CurrProductIncludeCaliDataTypes = new List<CaliDataType>();
        public static List<Int32>? AcqBdNoChannelCorrespondence=new List<int>() { 0,0,1,1,2,2,3,3,0,0};
        public static Dictionary<Calibration.Data.Base.CoefficientsTableType, string> ProductCoefficientsTableTypeDefine
        {
            get;
            set; 
        } = new Dictionary<Data.Base.CoefficientsTableType, string>();
        public static void Convert(string fromServer)
        {
            //HardwareAttached:1|ProductType:5|AdcBits:12|AnalogChannelCount:4|PerAnaChannelAdcCount:2|PerAdcCoreCount:2|PerAnaChannelDataCount:10000|PerAdcCoreDataCount:10000|AnalogChannelType:4|CoefficientsTableTypeDefine_1:TiAdc_Acq
            string[] paramList = fromServer.Split('|');
            foreach (string s in paramList)
            {
                if (s.Trim() == "")
                    continue;
                if (s.IndexOf("CoefficientsTableTypeDefine_") < 0 && s.IndexOf("AcqBdNoChannelCorrespondence")<0)
                {
                    string[] perParam = s.Split(':');
                    Int32.TryParse(perParam[1],out int value);
                    if (perParam[0] == "HardwareAttached")
                        ServerDomainConstants.HardwareAttached = (value == 1);
                    else if (perParam[0] == "ProductType")
                    {
                        ServerDomainConstants.ProductType = Enum.Parse<ProductType>(perParam[1]);
                        CurrProductIncludeCaliDataTypes = CaliDataManager.GetProductIncludedParams(ServerDomainConstants.ProductType);
                        if (CurrProductIncludeCaliDataTypes == null)
                            CurrProductIncludeCaliDataTypes = new List<CaliDataType>();
                    }
                    else if (perParam[0] == "AnaChnlType")
                    {
                        ServerDomainConstants.AnaChnlType = (AnaChnlType)value;
                    }
                    else if (perParam[0] == "AdcBits")
                        ServerDomainConstants.AdcBits = value;
                    else if (perParam[0] == "AdcCount")
                        ServerDomainConstants.AdcCount = value;
                    else if (perParam[0] == "AnalogChannelLevelMin")
                        ServerDomainConstants.AnalogChannelLevelMin = value;
                    else if (perParam[0] == "AnalogChannelLevelMax")
                        ServerDomainConstants.AnalogChannelLevelMax = value;
                    else if (perParam[0] == "PerAdcCoreCount")
                        ServerDomainConstants.PerAdcCoreCount = value;
                    else if (perParam[0] == "PerAnaChannelAdcCount")
                        ServerDomainConstants.PerAnaChannelAdcCount = value;
                    else if (perParam[0] == "AnalogChannelCount")
                        ServerDomainConstants.AnalogChannelCount = value;
                    else if (perParam[0] == "PerAnaChannelDataCount")
                        ServerDomainConstants.PerAnaChannelDataCount = value;
                    else if (perParam[0]== "PerAdcCoreDataCount")
                        ServerDomainConstants.PerAdcCoreDataCount = value;
                    else if (perParam[0] == "SAMPS_PER_YDIV")
                        ServerDomainConstants.SAMPS_PER_YDIV = value;
                    else if (perParam[0] == "AnalogChannelType")
                        AnalogChannelType = value;
                }
                else if (s.IndexOf("AcqBdNoChannelCorrespondence") >= 0)
                {
                    string ss = s.Replace("AcqBdNoChannelCorrespondence:", "");
                    List<Int32> ints = new List<Int32>();
                    string tmp = ss.Trim().PadRight(10, '0');
                    for (int i = 0; i < 10; i++)
                        ints.Add(Int32.Parse(tmp.Substring(i, 1)));
                    AcqBdNoChannelCorrespondence = ints;
                }
                else
                {
                    string coefficientsTableTypeDefine = s.Replace("CoefficientsTableTypeDefine_", "");
                    string[] defineList = coefficientsTableTypeDefine.Split(',');
                    ProductCoefficientsTableTypeDefine.Clear();
                    foreach(string define in defineList)
                    {
                        string[] valveNamePair = define.Split(':');
                        ProductCoefficientsTableTypeDefine.Add((Data.Base.CoefficientsTableType)int.Parse(valveNamePair[0]), valveNamePair[1]);
                    }
                }
            }
        }
        public static Dictionary<AnaChnlScaleIndex, int/*by_uV*/> AnalyChannelYScaleTable = new Dictionary<AnaChnlScaleIndex, int>()
        {
            [AnaChnlScaleIndex.Lv500u] = 500,
            [AnaChnlScaleIndex.Lv1m] = 1_000,
            [AnaChnlScaleIndex.Lv2m] = 2_000,
            [AnaChnlScaleIndex.Lv5m] = 5000,
            [AnaChnlScaleIndex.Lv10m] = 10_000,
            [AnaChnlScaleIndex.Lv20m] = 20_000,
            [AnaChnlScaleIndex.Lv50m] = 50_000,
            [AnaChnlScaleIndex.Lv100m] = 100_000,
            [AnaChnlScaleIndex.Lv200m] = 200_000,
            [AnaChnlScaleIndex.Lv500m] = 500_000,
            [AnaChnlScaleIndex.Lv1] = 1_000_000,
            [AnaChnlScaleIndex.Lv2] = 2_000_000,
            [AnaChnlScaleIndex.Lv5] = 5_000_000,
            [AnaChnlScaleIndex.Lv10] = 10_000_000,
            [AnaChnlScaleIndex.Lv20] = 20_000_000,
        };
    }
}
