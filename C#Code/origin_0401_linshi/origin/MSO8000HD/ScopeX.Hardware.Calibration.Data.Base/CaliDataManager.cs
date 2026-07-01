using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public static class CaliDataManager
    {
        public static CaliDataType[] FlashCaliTypes =
             new CaliDataType[]
             {
                CaliDataType.AnalogParams,
                //CaliDataType.TiadcPhaseOffsetGainParams,
                CaliDataType.CoefficientsParams,
                CaliDataType.Misc,
                CaliDataType.AWG
            };

        public static void SaveAllToFile(Boolean isSaveAll = true)
        {
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                if (!isSaveAll && FlashCaliTypes.Contains(dataType))
                {
                    continue;
                }
                Helper.GetICaliData(dataType)?.SaveToFile();
            }
        }

        public static void LoadAllFromFile(Boolean isLoadAll = true)
        {
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                if (!isLoadAll && FlashCaliTypes.Contains(dataType))
                {
                    continue;
                }
                Helper.GetICaliData(dataType)?.LoadFromFile();
            }
        }

        public static void LoadAllDefault()
        {
            foreach (CaliDataType dataType in Enum.GetValues(typeof(CaliDataType)))
            {
                Helper.GetICaliData(dataType)?.LoadFromFile();
            }
        }

        public static List<CaliDataType> DataChangedCaliDataType
        {
            get;
        } = new List<CaliDataType>();

        public static List<CaliDataType> DataChangedCaliDataType_Running
        {
            get;
        } = new List<CaliDataType>();

        public static List<CoefficientsTableType> DataChangedCoefficientsTableType
        {
            get;
        } = new List<CoefficientsTableType>();

        public static List<CoefficientsTableType> DataChangedCoefficientsTableType_Running
        {
            get;
        } = new List<CoefficientsTableType>();

        public static object DbiDataChangedLocker = new object();
        public static Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> DataChangedDbiCoefficientsTablesType
        {
            get;
        } = new Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>();
        public static Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> DataChangedDbiCoefficientsTablesType_Running
        {
            get;
        } = new Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>();
        public static DbiCoefficientsTablesType LastChangedDataType = DbiCoefficientsTablesType.InterpolationCoefficients;
        public static bool CheckDataChanged()
        {
            lock (DataChangedCaliDataType)
            {
                try
                {
                    if (DataChangedCoefficientsTableType.Count > 0)
                    {
                        DataChangedCoefficientsTableType_Running.AddRange(DataChangedCoefficientsTableType.ToArray());
                        if (!DataChangedCaliDataType.Contains(CaliDataType.CoefficientsTables))
                            DataChangedCaliDataType.Add(CaliDataType.CoefficientsTables);
                        DataChangedCoefficientsTableType.Clear();
                    }
                    if (DataChangedDbiCoefficientsTablesType.Count > 0)
                    {
                        foreach (var a in DataChangedDbiCoefficientsTablesType)
                            if (!DataChangedDbiCoefficientsTablesType_Running.ContainsKey(a.Key))
                            {
                                DataChangedDbiCoefficientsTablesType_Running.Add(a.Key, a.Value);
                            }
                           
                        if (!DataChangedCaliDataType.Contains(CaliDataType.DbiCoefficientsTables))
                            DataChangedCaliDataType.Add(CaliDataType.DbiCoefficientsTables);
                        DataChangedDbiCoefficientsTablesType.Clear();
                    }
                    if (DataChangedCaliDataType.Count > 0)
                        DataChangedCaliDataType_Running.AddRange(DataChangedCaliDataType.ToArray());
                    DataChangedCaliDataType.Clear();

                }
                catch (Exception)
                {

                }
                    return DataChangedCaliDataType_Running.Count > 0;
            }
        }
        private static Dictionary<ProductType, List<CaliDataType>> allProductIncludedCaliParams = new Dictionary<ProductType, List<CaliDataType>>()
        {
            [ProductType.B21_DBI16G] = new List<CaliDataType>() { CaliDataType.DbiAnalogParams, CaliDataType.DbiCoefficientsTables, CaliDataType.DbiLocalOscillators, CaliDataType.CoefficientsTables, CaliDataType.Misc, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock },
            [ProductType.B21_DBI20G] = new List<CaliDataType>() { CaliDataType.DbiAnalogParams, CaliDataType.DbiCoefficientsTables, CaliDataType.DbiLocalOscillators, CaliDataType.CoefficientsTables, CaliDataType.Misc, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock },

            [ProductType.B21_HB8G] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.PhyChannelModel2, CaliDataType.CoefficientsTables, CaliDataType.Misc, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock },
            [ProductType.B21_MD8G] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.PhyChannelModel2, CaliDataType.CoefficientsTables, CaliDataType.Misc, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock },

            [ProductType.B21_HR1G] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
            [ProductType.B21_HD4G] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
            [ProductType.B21_MS2G] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
            [ProductType.JiHe_MSO7000X] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
            [ProductType.JiHe_MSO7000A] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
            [ProductType.JiHe_MSO8000X] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc, CaliDataType.AnalogParams, CaliDataType.CoefficientsParams },

            [ProductType.B21_JinHui_PXI] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.Misc },

            [ProductType.Base] = new List<CaliDataType>() { CaliDataType.PhyChannel, CaliDataType.CoefficientsTables, CaliDataType.TiadcPhaseOffsetGainParams, CaliDataType.TiAdc_SyncSampleClock, CaliDataType.Misc },
        };
        public static List<CaliDataType>? GetProductIncludedParams(ProductType productType)
        {
            if (!allProductIncludedCaliParams.ContainsKey(productType))
                return null;
            return allProductIncludedCaliParams[productType];
        }

    }
}
