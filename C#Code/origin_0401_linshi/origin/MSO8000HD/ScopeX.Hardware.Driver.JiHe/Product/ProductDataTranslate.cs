using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 产品校准数据转化
    /// </summary>
    internal class ProductDataTranslate_MSO8000X
    {
        #region TiadcPhaseOffsetGainParams
        /// <summary>
        /// TiadcPhaseOffsetGainParams的访问key所对应的类
        /// </summary>
        /// <param name="interleaveName"></param>
        /// <param name="chnlId"></param>
        /// <param name="adcId"></param>
        public record TiadcParamsKeyMap(String interleaveName, ChannelId chnlId, UInt32 adcId);
        public record TiadcParamsKeyMapWithBoard(String interleaveName, ChannelId chnlId,AcqBdNo aqbd, UInt32 adcId);

        /// <summary>
        /// 通过TiadcParamsKeyMap获取TiadcPhaseOffsetGainParams的访问key
        /// </summary>
        /// <param name="tiadcMap"></param>
        /// <returns></returns>
        public static String GenerateTiadcParamsKey(TiadcParamsKeyMap tiadcMap)
        {
            return $"{tiadcMap.interleaveName}{CaliConstants.NameSpiltChar}{tiadcMap.chnlId}{CaliConstants.NameSpiltChar}Adc{tiadcMap.adcId}";
        }
        public static String GenerateTiadcParamsKeyWithBoard(TiadcParamsKeyMapWithBoard tiadcMap)
        {
            return $"{tiadcMap.interleaveName}{CaliConstants.NameSpiltChar}{tiadcMap.chnlId}{CaliConstants.NameSpiltChar}{tiadcMap.aqbd}{CaliConstants.NameSpiltChar}Adc{tiadcMap.adcId}";
        }
        /// <summary>
        /// 获取当前项目所有的TiadcPhaseOffsetGainParams的访问key
        /// </summary>
        /// <returns></returns>
        public static List<String> GetAllTiadcParamsKeys(AnalogAcquireModule_Jihe_MSO8000X analogAcquireModel)
        {
            var keys = new List<String>();
            //遍历模式，通道，Adc
            foreach (var modeDefine in analogAcquireModel.AcqModeAndInterleaveDefineTable)
            {
                foreach (var dtl in modeDefine.Value.Details)
                {
                    //Int32 num = 0;
                    foreach (var item in dtl.Value)
                    {
                        var acqInfos = analogAcquireModel.GetUsedAdcs(item);
                        foreach (var adcId in acqInfos)
                        {
                            var mapKey = new TiadcParamsKeyMap(modeDefine.Value.Name, dtl.Key, adcId);
                            string key = GenerateTiadcParamsKey(mapKey);
                            keys.Add(key);
                        }
                        //num += acqInfos.Count();
                    }
                  
                }
            }
            return keys;
        }

        /// <summary>
        /// 通过TiadcParamsKeyMap获取对应的TiadcPhaseOffsetGainItem_Base
        /// </summary>
        /// <param name="tiadcMap"></param>
        /// <returns></returns>
        public static TiadcPhaseOffsetGainItem_Base? GetTiadcParamsItem(TiadcParamsKeyMap tiadcMap)
        {
            if (TiadcPhaseOffsetGainParams.Default.TiadcItemType != typeof(TiadcPhaseOffsetGainItem_Base))
                return null;

            string tiadcKey = GenerateTiadcParamsKey(tiadcMap);
            return (TiadcPhaseOffsetGainItem_Base)TiadcPhaseOffsetGainParams.Default[tiadcKey];
        }

        public static TiadcPhaseOffsetGainItem_Base? GetTiadcParamsItemWithBoard(TiadcParamsKeyMapWithBoard tiadcMap)
        {
            if (TiadcPhaseOffsetGainParams.Default.TiadcItemType != typeof(TiadcPhaseOffsetGainItem_Base))
                return null;

            string tiadcKey = GenerateTiadcParamsKeyWithBoard(tiadcMap);
            switch (tiadcKey)
            {
                case "01_C1_B0_Adc0": tiadcKey = "All-20G_C1_Adc0"; break;
                case "01_C1_B0_Adc1": tiadcKey = "All-20G_C1_Adc1"; break;
   
                case "01_C1_B1_Adc0": tiadcKey = "All-20G_C2_Adc0"; break;
                case "01_C1_B1_Adc1": tiadcKey = "All-20G_C2_Adc1"; break;
 
                case "01_C1_B2_Adc0": tiadcKey = "All-20G_C3_Adc0"; break;
                case "01_C1_B2_Adc1": tiadcKey = "All-20G_C3_Adc1"; break;
          
                case "01_C1_B3_Adc0": tiadcKey = "All-20G_C4_Adc0"; break;
                case "01_C1_B3_Adc1": tiadcKey = "All-20G_C4_Adc1"; break;

                case "DBI_Disable_C1_B0_Adc0": tiadcKey = "All-20G_C1_Adc0"; break;
                case "DBI_Disable_C1_B0_Adc1": tiadcKey = "All-20G_C1_Adc1"; break;

                case "DBI_Disable_C1_B1_Adc0": tiadcKey = "All-20G_C2_Adc0"; break;
                case "DBI_Disable_C1_B1_Adc1": tiadcKey = "All-20G_C2_Adc1"; break;

                case "DBI_Disable_C1_B2_Adc0": tiadcKey = "All-20G_C3_Adc0"; break;
                case "DBI_Disable_C1_B2_Adc1": tiadcKey = "All-20G_C3_Adc1"; break;

                case "DBI_Disable_C1_B3_Adc0": tiadcKey = "All-20G_C4_Adc0"; break;
                case "DBI_Disable_C1_B3_Adc1": tiadcKey = "All-20G_C4_Adc1"; break;

                //case "All-20G_C1_B0_Adc0": tiadcKey = "All-20G_C1_Adc0"; break;
                //case "All-20G_C1_B0_Adc1": tiadcKey = "All-20G_C1_Adc1"; break;
                //case "All-20G_C1_B1_Adc0": tiadcKey = "All-20G_C1_Adc2"; break;
                //case "All-20G_C1_B1_Adc1": tiadcKey = "All-20G_C1_Adc3"; break;
                //case "All-20G_C2_B1_Adc0": tiadcKey = "All-20G_C2_Adc0"; break;
                //case "All-20G_C2_B1_Adc1": tiadcKey = "All-20G_C2_Adc1"; break;
                //case "All-20G_C2_B3_Adc0": tiadcKey = "All-20G_C2_Adc2"; break;
                //case "All-20G_C2_B3_Adc1": tiadcKey = "All-20G_C2_Adc3"; break;
                //case "All-20G_C3_B2_Adc0": tiadcKey = "All-20G_C3_Adc0"; break;
                //case "All-20G_C3_B2_Adc1": tiadcKey = "All-20G_C3_Adc1"; break;
                //case "All-20G_C3_B5_Adc0": tiadcKey = "All-20G_C3_Adc2"; break;
                //case "All-20G_C3_B5_Adc1": tiadcKey = "All-20G_C3_Adc3"; break;
                //case "All-20G_C4_B3_Adc0": tiadcKey = "All-20G_C4_Adc0"; break;
                //case "All-20G_C4_B3_Adc1": tiadcKey = "All-20G_C4_Adc1"; break;
                //case "All-20G_C4_B7_Adc0": tiadcKey = "All-20G_C4_Adc2"; break;
                //case "All-20G_C4_B7_Adc1": tiadcKey = "All-20G_C4_Adc3"; break;
                default:
                    break;
            }
            
            return (TiadcPhaseOffsetGainItem_Base)TiadcPhaseOffsetGainParams.Default[tiadcKey];
        }

        /// <summary>
        /// 通过TiadcParamsKeyMap设置对应的TiadcPhaseOffsetGainItem_Base
        /// </summary>
        /// <param name="tiadcMap"></param>
        /// <param name="value"></param>
        public static void SetTiadcParamsItem(TiadcParamsKeyMap tiadcMap, TiadcPhaseOffsetGainItem_Base value)
        {
            string tiadcKey = GenerateTiadcParamsKey(tiadcMap);
            TiadcPhaseOffsetGainParams.Default[tiadcKey] = value;
        }
        public static void SetTiadcParamsItemWithBoard(TiadcParamsKeyMapWithBoard tiadcMap, TiadcPhaseOffsetGainItem_Base value)
        {
            string tiadcKey = GenerateTiadcParamsKeyWithBoard(tiadcMap);
            switch (tiadcKey)
            {
                case "DBI_Disable_C1_B0_Adc0": tiadcKey = "All-20G_C1_Adc0"; break;
                case "DBI_Disable_C1_B0_Adc1": tiadcKey = "All-20G_C1_Adc1"; break;
                case "DBI_Disable_C1_B1_Adc0": tiadcKey = "All-20G_C2_Adc0"; break;
                case "DBI_Disable_C1_B1_Adc1": tiadcKey = "All-20G_C2_Adc1"; break;
                case "DBI_Disable_C1_B2_Adc0": tiadcKey = "All-20G_C3_Adc0"; break;
                case "DBI_Disable_C1_B2_Adc1": tiadcKey = "All-20G_C3_Adc1"; break;
                case "DBI_Disable_C1_B3_Adc0": tiadcKey = "All-20G_C4_Adc0"; break;
                case "DBI_Disable_C1_B3_Adc1": tiadcKey = "All-20G_C4_Adc1"; break;
               
                default:
                    break;
            }
            TiadcPhaseOffsetGainParams.Default[tiadcKey] = value;
        }
        #endregion 

        #region AnalogChannelParams

        /// <summary>
        /// AnalogChannelParams的访问key所对应的类
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="isHighImp"></param>
        /// <param name="scaleValueByMv"></param>
        public record ChnlParamsKeyMap(ChannelId chnlId, Boolean isHighImp,UInt32 scaleValueByMv);

        /// <summary>
        /// 通过ChnlParamsKeyMap获取AnalogChannelParams的访问key
        /// </summary>
        /// <param name="chnlMap"></param>
        /// <returns></returns>
        public static String GenerateChnlParamsKey(ChnlParamsKeyMap chnlMap)
        {
            string impedance = chnlMap.isHighImp ? "High": "Low";
            return $"{chnlMap.chnlId}{CaliConstants.NameSpiltChar}{impedance}{CaliConstants.NameSpiltChar}{chnlMap.scaleValueByMv}Mv";
        }

        /// <summary>
        /// 获取当前项目所有的AnalogChannelParams的访问key
        /// </summary>
        /// <returns></returns>
        public static List<String> GetAllChnlParamsKeys()
        {
            List<bool> isHighImps = new List<bool>() { false, true};
            var keys = new List<String>();

            foreach (ChannelId chnlId in ChannelIdExt.GetAnalogs())
            {
                foreach (bool isHighImp in isHighImps)
                {
                    var maxScaleIndex = (int)(isHighImp ? AnaChnlScaleIndex.Lv10 : AnaChnlScaleIndex.Lv1);
                    var minScaleIndex = (int)(AnaChnlScaleIndex.Lv1m);
                    for (int scaleIndex = minScaleIndex; scaleIndex <= maxScaleIndex; scaleIndex++)
                    {
                        var scaleMv = (UInt32)(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[scaleIndex] / 1000);
                        var mapKey = new ChnlParamsKeyMap(chnlId, isHighImp, scaleMv);
                        string key = GenerateChnlParamsKey(mapKey);
                        keys.Add(key);
                    }

                }
            }
            return keys;
        }

        /// <summary>
        /// 通过ChnlParamsKeyMap获取对应的AnalogChannelItem_Base
        /// </summary>
        /// <param name="chnlMap"></param>
        /// <returns></returns>
        public static AnalogChannelItem_Base? GetChnlParamsItem(ChnlParamsKeyMap chnlMap)
        {
            if (AnalogChannelParams.Default.ItemType != typeof(AnalogChannelItem_Base))
                return null;

            string chnlKey = GenerateChnlParamsKey(chnlMap);
            return (AnalogChannelItem_Base)AnalogChannelParams.Default[chnlKey];
        }

        /// <summary>
        /// 通过ChnlParamsKeyMap设置对应的AnalogChannelItem_Base
        /// </summary>
        /// <param name="chnlMap"></param>
        /// <param name="value"></param>
        public static void SetChnlParamsItem(ChnlParamsKeyMap chnlMap, AnalogChannelItem_Base value)
        {
            string chnlKey = GenerateChnlParamsKey(chnlMap);
            AnalogChannelParams.Default[chnlKey] = value;
        }

        #endregion 

    }
}
