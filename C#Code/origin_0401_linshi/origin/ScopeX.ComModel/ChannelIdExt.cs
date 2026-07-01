using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ScopeX.ComModel
{
    public enum ChannelId
    {
        C1 = 0, C2, C3, C4, C5, C6, C7, C8,
        JITTER = 100,//抖动分析目前唯一
        M1 = 200, M2, M3, M4, M5, M6, M7, M8,
        M9, M10, M11, M12, M13, M14, M15, M16,
        M17, M18, M19, M20, M21, M22, M23, M24,
        M25, M26, M27, M28, M29, M30, M31, M32,
        M33, M34, M35, M36, M37, M38, M39, M40,
        M41, M42, M43, M44, M45, M46, M47, M48,
        M49, M50, M51, M52, M53, M54, M55, M56,
        M57, M58, M59, M60, M61, M62, M63, M64,
        M65, M66, M67, M68, M69, M70, M71, M72,
        M73, M74, M75, M76, M77, M78, M79, M80,
        R1 = 300, R2, R3, R4, R5, R6, R7, R8,
        R9, R10, R11, R12, R13, R14, R15, R16,
        B1 = 400, B2,
        D0 = 500, D1, D2, D3, D4, D5, D6, D7, D8,
        D9, D10, D11, D12, D13, D14, D15, D16,
        D17, D18, D19, D20, D21, D22, D23, D24,
        D25, D26, D27, D28, D29, D30, D31, D32,
        D33, D34, D35, D36, D37, D38, D39, D40,
        D41, D42, D43, D44, D45, D46, D47, D48,
        D49, D50, D51, D52, D53, D54, D55, D56,
        D57, D58, D59, D60, D61, D62, D63,// D64,
        P1 = 1100, P2, P3, P4, P5, P6, P7, P8,
        P9, P10, P11, P12, P13, P14, P15, P16,
        AWG1 = 1200, AWG2, AWG3, AWG4,
        DVM = 1240,
        CYM = 1245,
        Ext = 1250,
        [Alias("Ext/5")]
        Ext5,
        AC,
        AuxIn,
        //Assign them to logical RF channel like math
        RF1 = 1300, RF2, RF3, RF4, RF5, RF6, RF7, RF8,
        //Assign 'RF' to independent phisical RF channel
        RF,
        AVT1 = 1400, AVT2, AVT3, AVT4, AVT5, AVT6, AVT7, AVT8, AVT,
        PVT1 = 1500, PVT2, PVT3, PVT4, PVT5, PVT6, PVT7, PVT8, PVT,
        PVF1 = 1600, PVF2, PVF3, PVF4, PVF5, PVF6, PVF7, PVF8, PVF,
        PGD1 = 1700, PGD2, PGD3, PGD4, PGD5, PGD6, PGD7, PGD8, PGD,
        TVF1 = 1800, TVF2, TVF3, TVF4, TVF5, TVF6, TVF7, TVF8, TVF,
        FVT1 = 1900, FVT2, FVT3, FVT4, FVT5, FVT6, FVT7, FVT8, FVT,
        USER = 2000,
        POWER1 = 2100, POWER2, POWER3, POWER4, /*POWER5, POWER6, POWER7, POWER8,*/

        MAKER1=2200,MAKER2, MAKER3,MAKER4, MAKER5,MAKER6, MAKER7,MAKER8, MAKER9,MAKER10,
        MAKER11,MAKER12, MAKER13,MAKER14, MAKER15,MAKER16, MAKER17,MAKER18, MAKER19,MAKER20,
        MAKER21,MAKER22, MAKER23,MAKER24, MAKER25,MAKER26, MAKER27,MAKER28, MAKER29,MAKER30,
        MAKER31,MAKER32, MAKER33,MAKER34, MAKER35,MAKER36, MAKER37,MAKER38, MAKER39,MAKER40,
        MAKER41,MAKER42, MAKER43,MAKER44, MAKER45,MAKER46, MAKER47,MAKER48,
        // None channel,means no channel selected.
        None = Int32.MaxValue,
        ExceptionCapture = 600,//异常捕获通道
    }

    public static class ChannelIdExt
    {
        public static ChannelId Clamp(this ChannelId id, IEnumerable<ChannelId> allows)
        {
            if (allows == null || allows.Count() == 0) return id;
            if (allows.Contains(id)) return id;
            else return allows.First();
        }
        static ChannelIdExt()
        {
            var config = AppConfig.GetIntance();
            MaxAChId = Enum.Parse<ChannelId>(config.MaxAChId/*AppConfigureHelper.AppSettings[nameof(MaxAChId)]?.ToString() ?? "C4"*/);
            MaxMChId = Enum.Parse<ChannelId>(config.MaxMChId/*AppConfigureHelper.AppSettings[nameof(MaxMChId)]?.ToString() ?? "M8"*/);
            MaxRChId = Enum.Parse<ChannelId>(config.MaxRChId/*AppConfigureHelper.AppSettings[nameof(MaxRChId)]?.ToString() ?? "R4"*/);
            MaxDChId = Enum.Parse<ChannelId>(config.MaxDChId/*AppConfigureHelper.AppSettings[nameof(MaxDChId)]?.ToString() ?? "D15"*/);
            MaxBChId = Enum.Parse<ChannelId>(config.MaxBChId/*AppConfigureHelper.AppSettings[nameof(MaxBChId)]?.ToString() ?? "B2"*/);
            MaxPChId = Enum.Parse<ChannelId>(config.MaxPChId/*AppConfigureHelper.AppSettings[nameof(MaxPChId)]?.ToString() ?? "P8"*/);
            MaxAwgId = Enum.Parse<ChannelId>(config.MaxAwgId/*AppConfigureHelper.AppSettings[nameof(MaxAwgId)]?.ToString() ?? "AWG1"*/);
            MaxRFChId = Enum.Parse<ChannelId>(config.MaxRFChId/*AppConfigureHelper.AppSettings[nameof(MaxRFChId)]?.ToString() ?? "RF4"*/);
            //MaxAVTChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxAVTChId)]?.ToString() ?? "AVT4");
            //MaxPVTChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxPVTChId)]?.ToString() ?? "PVT4");
            //MaxPVFChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxPVFChId)]?.ToString() ?? "PVF4");
            //MaxPGDChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxPGDChId)]?.ToString() ?? "PGD4");
            //MaxTVFChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxPGDChId)]?.ToString() ?? "TVF4");
            //MaxFVTChId = Enum.Parse<ChannelId>(AppConfigureHelper.AppSettings[nameof(MaxPGDChId)]?.ToString() ?? "FVT4");
            MaxPAMNum = config.MaxPAMNum;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(MaxPAMNum)]?.ToString() ?? "8");
            MaxJMNum = config.MaxJMNum;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(MaxJMNum)]?.ToString() ?? "8");
            MaxVSAMNum = config.MaxVSAMNum;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(MaxVSAMNum)]?.ToString() ?? "8");
            MaxIChartMNum = config.MaxIChartMNum;
            MaxCEMNum = config.MaxCEMNum;
            MaxIRMNum = config.MaxIRMNum;
            MaxMDNum = config.MaxMDNum;

            AnaChnlNum = MaxAChId - MinAChId + 1;
            DigiChnlNum = MaxDChId - MinDChId + 1;
            MathChnlNum = MaxMChId - MinMChId + 1;
            RefChnlNum = MaxRChId - MinRChId + 1;
            BusChnlNum = MaxBChId - MinBChId + 1;
            MeasChnlNum = MaxPChId - MinPChId + 1;
            AwgNum = MaxAwgId - MinAwgId + 1;
            RFChnlNum = MaxRFChId - MinRFChId + 1;

            MaxAVTChId = MinAVTChId + RFChnlNum - 1;
            MaxPVTChId = MinPVTChId + RFChnlNum - 1;
            MaxPVFChId = MinPVFChId + RFChnlNum - 1;
            MaxPGDChId = MinPGDChId + RFChnlNum - 1;
            MaxTVFChId = MinTVFChId + RFChnlNum - 1;
            MaxFVTChId = MinFVTChId + RFChnlNum - 1;

            MinPAMChId = MaxMChId + 1;
            MaxPAMChId = MaxMChId + MaxPAMNum;
            MinJMChId = MaxPAMChId + 1;
            MaxJMChId = MaxPAMChId + MaxJMNum;
            MinVSAMChId = MaxJMChId + 1;
            MaxVSAMChId = MaxJMChId + MaxVSAMNum;
            MinIChartMChId = MaxVSAMChId + 1;
            MaxIChartMChId = MaxVSAMChId + MaxIChartMNum;
            MinCEMChId = MaxIChartMChId + 1;
            MaxCEMChId = MaxIChartMChId + MaxCEMNum;
            MinIRMChId = MaxCEMChId + 1;
            MaxIRMChId = MaxCEMChId + MaxIRMNum;
            MinMDChId = MaxIRMChId + 1;
            MaxMDChId = MaxIRMChId + MaxMDNum;

        }

        public static readonly Int32 AnaChnlNum;
        public static readonly Int32 DigiChnlNum;
        public static readonly Int32 MathChnlNum;
        public static readonly Int32 RefChnlNum;
        public static readonly Int32 BusChnlNum;
        public static readonly Int32 MeasChnlNum;
        public static readonly Int32 AwgNum;
        public static readonly Int32 DvmNum = 1;
        public static readonly Int32 RFChnlNum;
        public static readonly Int32 AVTChnlNum;
        public static readonly Int32 PVTChnlNum;
        public static readonly Int32 PVFChnlNum;
        public static readonly Int32 PGDChnlNum;
        public static readonly Int32 TVFChnlNum;
        public static readonly Int32 FVTChnlNum;

        public static readonly Int32 MaxJMNum;
        public static readonly Int32 MaxPAMNum;
        public static readonly Int32 MaxVSAMNum;
        public static readonly Int32 MaxIChartMNum;
        public static readonly Int32 MaxCEMNum;
        public static readonly Int32 MaxIRMNum;
        public static readonly int MaxMDNum;

        public static readonly ChannelId MinAChId = ChannelId.C1;

        public static readonly ChannelId MaxAChId;

        public static Boolean IsAnalog(this ChannelId id) => id >= MinAChId && id <= MaxAChId;

        public static IEnumerable<ChannelId> GetAnalogs()
        {
            for (ChannelId id = MinAChId; id <= MaxAChId; id++)
            {
                yield return id;
            }

            //return Enum.GetValues<ChannelId>().Where(id => id.IsAnalog());
        }


        public static readonly ChannelId MinMChId = ChannelId.M1;

        public static readonly ChannelId MaxMChId;

        public static Boolean IsMath(this ChannelId id) => id >= ChannelId.M1 && id <= ChannelId.M80;

        public static Boolean IsAdvancedMath(this ChannelId id) => id > MaxMChId && id <= ChannelId.M80;

        public static Boolean IsBaseMath(this ChannelId id) => id >= ChannelId.M1 && id <= MaxMChId;

        public static IEnumerable<ChannelId> GetMaths()
        {
            for (ChannelId id = MinMChId; id <= MaxMChId; id++)
            {
                yield return id;
            }
        }

        public static readonly ChannelId MinPAMChId;

        public static readonly ChannelId MaxPAMChId;

        public static IEnumerable<ChannelId> GetPowerAnalysisMaths()
        {
            for (ChannelId id = MinPAMChId; id <= MaxPAMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M48)
                    yield return id;
            }
        }

        public static Boolean IsPowerAnalysisMath(this ChannelId id)
        {
            return id >= MinPAMChId && id <= MaxPAMChId;
        }

        public static readonly ChannelId MinJMChId;

        public static readonly ChannelId MaxJMChId;

        public static IEnumerable<ChannelId> GetJitterMaths()
        {
            for (ChannelId id = MinJMChId; id <= MaxJMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M48)
                    yield return id;
            }
        }

        public static Boolean IsJitterMath(this ChannelId id)
        {
            return id >= MinJMChId && id <= MaxJMChId;
        }

        public static readonly ChannelId MinVSAMChId;

        public static readonly ChannelId MaxVSAMChId;

        public static IEnumerable<ChannelId> GetVSAMaths()
        {
            for (ChannelId id = MinVSAMChId; id <= MaxVSAMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M48)
                    yield return id;
            }
        }
		public static readonly ChannelId MinIChartMChId;

        public static readonly ChannelId MaxIChartMChId;

        public static IEnumerable<ChannelId> GetIChartMaths()
        {
            for (ChannelId id = MinIChartMChId; id <= MaxIChartMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M64)
                    yield return id;
            }
        }

        public static readonly ChannelId MinCEMChId;   //CE:Capture Exception

        public static readonly ChannelId MaxCEMChId;

        public static IEnumerable<ChannelId> GetCEMaths()
        {
            for (ChannelId id = MinCEMChId; id <= MaxCEMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M80)
                    yield return id;
            }
        }

        public static readonly ChannelId MinIRMChId;   //智能降噪

        public static readonly ChannelId MaxIRMChId;

        public static IEnumerable<ChannelId> GetIRMaths()
        {
            for (ChannelId id = MinIRMChId; id <= MaxIRMChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M80)
                    yield return id;
            }
        }

        public static readonly ChannelId MinMDChId;

        public static readonly ChannelId MaxMDChId;

        public static IEnumerable<ChannelId> GetMDMaths()
        {
            for (ChannelId id = MinMDChId; id <= MaxMDChId; id++)
            {
                if (id >= ChannelId.M1 && id <= ChannelId.M80)
                {
                    yield return id;
                }
            }
        }

        public static readonly ChannelId MinRChId = ChannelId.R1;

        public static readonly ChannelId MaxRChId;

        public static Boolean IsReference(this ChannelId id) => id >= MinRChId && id <= MaxRChId;

        public static IEnumerable<ChannelId> GetReferences()
        {
            for (ChannelId id = MinRChId; id <= MaxRChId; id++)
            {
                yield return id;
            }
        }

        public static readonly ChannelId MinDChId = ChannelId.D0;

        public static readonly ChannelId MaxDChId;

        public static Boolean IsDigital(this ChannelId id) => id >= MinDChId && id <= MaxDChId;

        public static IEnumerable<ChannelId> GetDigitals()
        {
            for (ChannelId id = MinDChId; id <= MaxDChId; id++)
            {
                yield return id;
            }
        }


        public static readonly ChannelId MinBChId = ChannelId.B1;

        public static readonly ChannelId MaxBChId;

        public static Boolean IsDecode(this ChannelId id) => id >= MinBChId && id <= MaxBChId;

        public static IEnumerable<ChannelId> GetDecodes()
        {
            for (ChannelId id = MinBChId; id <= MaxBChId; id++)
            {
                yield return id;
            }
        }


        public static readonly ChannelId MinPChId = ChannelId.P1;

        public static readonly ChannelId MaxPChId;

        public static Boolean IsMeasure(this ChannelId id) => id >= MinPChId && id <= MaxPChId;

        public static IEnumerable<ChannelId> GetMeasures()
        {
            for (ChannelId id = MinPChId; id <= MaxPChId; id++)
            {
                yield return id;
            }
        }

        public static readonly ChannelId MinAwgId = ChannelId.AWG1;

        public static readonly ChannelId MaxAwgId;

        public static Boolean IsAWG(this ChannelId id) => id >= MinAwgId && id <= MaxAwgId;

        public static IEnumerable<ChannelId> GetAWGs()
        {
            for (ChannelId id = MinAwgId; id <= MaxAwgId; id++)
            {
                yield return id;
            }
        }

        //public static Boolean IsExtTrigger(this ChannelId id) => id >= ChannelId.Ext && id < ChannelId.USER;

        public static Boolean IsExtTrigger(this ChannelId id) => id == ChannelId.Ext;

        public static IEnumerable<ChannelId> GetTriggerSources()
        {

            for (ChannelId id = MinAChId; id <= MaxAChId; id++)
            {
                yield return id;
            }
            yield return ChannelId.Ext;
            yield return ChannelId.AC;
            yield return ChannelId.AuxIn;
            for (ChannelId id = MinDChId; id <= MaxDChId; id++)
            {
                yield return id;
            }
        }

        public static IEnumerable<ChannelId> GetTriggerSourcesWithoutDigital()
        {
            for (ChannelId id = MinAChId; id <= MaxAChId; id++)
            {
                yield return id;
            }
            yield return ChannelId.Ext;
            yield return ChannelId.AC;
        }

        public static readonly ChannelId MinRFChId = ChannelId.RF1;

        public static readonly ChannelId MaxRFChId;

        public static Boolean IsRadioFrequency(this ChannelId id) => id >= MinRFChId && id <= MaxRFChId;

        public static IEnumerable<ChannelId> GetRadioFrequencies()
        {
            for (ChannelId id = MinRFChId; id <= MaxRFChId; id++)
            {
                yield return id;
            }
        }

        public static readonly ChannelId MinAVTChId = ChannelId.AVT1;

        public static readonly ChannelId MaxAVTChId;

        public static Boolean IsAmpVSTime(this ChannelId id) => id >= MinAVTChId && id <= MaxAVTChId;

        public static IEnumerable<ChannelId> GetAmpVSTimes()
        {
            for (ChannelId id = MinAVTChId; id <= MaxAVTChId; id++)
                yield return id;
        }

        public static readonly ChannelId MinPVTChId = ChannelId.PVT1;

        public static readonly ChannelId MaxPVTChId;

        public static Boolean IsPhaseVSTime(this ChannelId id) => id >= MinPVTChId && id <= MaxPVTChId;

        public static IEnumerable<ChannelId> GetPhaseVSTimes()
        {
            for (ChannelId id = MinPVTChId; id <= MaxPVTChId; id++)
                yield return id;
        }

        public static readonly ChannelId MinPVFChId = ChannelId.PVF1;

        public static readonly ChannelId MaxPVFChId;

        public static Boolean IsPhaseVSFrequency(this ChannelId id) => id >= MinPVFChId && id <= MaxPVFChId;

        public static IEnumerable<ChannelId> GetPhaseVSFrequencies()
        {
            for (ChannelId id = MinPVFChId; id <= MaxPVFChId; id++)
                yield return id;
        }

        public static readonly ChannelId MinPGDChId = ChannelId.PGD1;

        public static readonly ChannelId MaxPGDChId;

        public static Boolean IsPhaseGroupDelay(this ChannelId id) => id >= MinPGDChId && id <= MaxPGDChId;

        public static IEnumerable<ChannelId> GetPhaseGroupDelaies()
        {
            for (ChannelId id = MinPGDChId; id <= MaxPGDChId; id++)
                yield return id;
        }

        public static readonly ChannelId MinTVFChId = ChannelId.TVF1;

        public static readonly ChannelId MaxTVFChId;

        public static Boolean IsTimeVSFrequency(this ChannelId id) => id >= MinTVFChId && id <= MaxTVFChId;

        public static IEnumerable<ChannelId> GetTimeVSFrequencies()
        {
            for (ChannelId id = MinTVFChId; id <= MaxTVFChId; id++)
                yield return id;
        }

        public static readonly ChannelId MinFVTChId = ChannelId.FVT1;

        public static readonly ChannelId MaxFVTChId;

        public static Boolean IsFrequencyVSTime(this ChannelId id) => id >= MinFVTChId && id <= MaxFVTChId;

        public static IEnumerable<ChannelId> GetFrequencyVSTimes()
        {
            for (ChannelId id = MinFVTChId; id <= MaxFVTChId; id++)
                yield return id;
        }
        public static readonly ChannelId MinPowerChId = ChannelId.POWER1;

        public static readonly ChannelId MaxPowerChId = ChannelId.POWER4;
        public static IEnumerable<ChannelId> GetPowers()
        {
            for (ChannelId id = ChannelId.POWER1; id <= ChannelId.POWER4; id++)
            {
                yield return id;
            }
        }

        public static IEnumerable<ChannelId> GetMarkers()
        {
            for (ChannelId id = ChannelId.MAKER1; id <= ChannelId.MAKER2; id++)
            {
                yield return id;
            }
        }

        /// <summary>
        /// 获取解码信源键值对列表
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="hasRef">是否包括参考通道</param>
        /// <returns></returns>
        public static List<KeyValuePair<String, Object>> GetDecodeChannels(this ChannelId[] ids, Boolean hasRef = false)
        {
            if (ids == null)
                return new List<KeyValuePair<string, object>>();
            if (hasRef)
                return ids.Where(x => x.IsAnalog() || x.IsReference()).Select(x => new KeyValuePair<string, object>(x.ToString(), x)).ToList();
            else
                return ids.Where(x => x.IsAnalog()).Select(x => new KeyValuePair<string, object>(x.ToString(), x)).ToList();
        }

        public static Boolean IsPowers(this ChannelId id) => id >= ChannelId.POWER1 && id <= ChannelId.POWER4;

        // 正则表达式用于匹配字符串中的数字
        static Regex regex = new Regex(@"\d+");
        public static Int64 GetIdNumByString(this String str)
        {
            Int64 id = -1;
            // 使用正则表达式找到匹配的数字
            Match match = regex.Match(str);
            if (match.Success)
            {
                // 将匹配到的字符串转换为整数
                id = Int64.Parse(match.Value);
            }

            return id;
        }
    }
}
