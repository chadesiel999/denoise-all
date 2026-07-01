using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScopeX.ComModel
{
    public static partial class Constants
    {
        static Constants()
        {
            var config = AppConfig.GetIntance();
            ENABLE_AWG_PROTECT = false;// Boolean.Parse( AppConfigureHelper.AppSettings[nameof(ENABLE_AWG_PROTECT)]?.ToString() ?? "false");
            BOARD_ATTACHED = config.BOARD_ATTACHED;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(BOARD_ATTACHED)]?.ToString() ?? "false");
            ENABLE_MATLAB = config.ENABLE_MATLAB;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_MATLAB)]?.ToString() ?? "false");
            ENABLE_WEBCORE = config.ENABLE_WEBCORE; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_WEBCORE)]?.ToString() ?? "false");
            ENABLE_LXI = config.ENABLE_LXI;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_LXI)]?.ToString() ?? "false");
            ENABLE_USB = config.ENABLE_USB;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_USB)]?.ToString() ?? "false");
            ENABLE_USB_SUPERSPEED = config.ENABLE_USB_SUPERSPEED;
            ENABLE_STYLE = config.ENABLE_STYLE;//Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_STYLE)]?.ToString() ?? "false");
            ENABLE_RF = false; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_RF)]?.ToString() ?? "false");
            ENABLE_LA = false; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_LA)]?.ToString() ?? "false");
            PROBE_FACT_CALIB = config.PROBE_FACT_CALIB;
            //ENABLE_BUS = config.ENABLE_BUS; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BUS)]?.ToString() ?? "false");
            ENABLE_AWG1 = false; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_AWG1)]?.ToString() ?? "false");
            ENABLE_AWG2 = false; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_AWG2)]?.ToString() ?? "false");
            //ENABLE_SDA = config.ENABLE_SDA; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_SDA)]?.ToString() ?? "false");
            ENABLE_VSA = true; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_VSA)]?.ToString() ?? "false");
            ENABLE_VERSION = config.ENABLE_VERSION; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_VERSION)]?.ToString() ?? "false");
            ENABLE_DEBUG = config.ENABLE_DEBUG; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_DEBUG)]?.ToString() ?? "false");
            ENABLE_BANDWIDTH = config.ENABLE_BANDWIDTH; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BANDWIDTH)]?.ToString() ?? "false");
            ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL = config.ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL;
            #region Component Service

            ENABLE_BUS = false; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BUS)]?.ToString() ?? "false");
            ENABLE_SDA = true;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_SDA)]?.ToString() ?? "false");
            ENABLE_AWG = false;
            ENABLE_Ref = false;
            ENABLE_Math = config.ENABLE_Math;
            ENABLE_Lissajous = false;
            ENABLE_PassFail = true;
            ENABLE_PowerAs = false;
            ENABLE_Measure = config.ENABLE_Measure;
            ENABLE_Search = false;
            ENABLE_Segement = false;

            #endregion Component Service
            //ENABLE_AWG_PROTECT = config.ENABLE_AWG_PROTECT;// Boolean.Parse( AppConfigureHelper.AppSettings[nameof(ENABLE_AWG_PROTECT)]?.ToString() ?? "false");
            //BOARD_ATTACHED = config.BOARD_ATTACHED;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(BOARD_ATTACHED)]?.ToString() ?? "false");
            //ENABLE_MATLAB = config.ENABLE_MATLAB;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_MATLAB)]?.ToString() ?? "false");
            //ENABLE_WEBCORE = config.ENABLE_WEBCORE; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_WEBCORE)]?.ToString() ?? "false");
            //ENABLE_LXI = config.ENABLE_LXI;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_LXI)]?.ToString() ?? "false");
            //ENABLE_USB = config.ENABLE_USB;// Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_USB)]?.ToString() ?? "false");
            //ENABLE_USB_SUPERSPEED = config.ENABLE_USB_SUPERSPEED;
            //ENABLE_STYLE = config.ENABLE_STYLE;//Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_STYLE)]?.ToString() ?? "false");
            //ENABLE_RF = config.ENABLE_RF; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_RF)]?.ToString() ?? "false");
            //ENABLE_LA = config.ENABLE_LA; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_LA)]?.ToString() ?? "false");
            //PROBE_FACT_CALIB = config.PROBE_FACT_CALIB;
            ////ENABLE_BUS = config.ENABLE_BUS; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BUS)]?.ToString() ?? "false");
            //ENABLE_AWG1 = config.ENABLE_AWG1; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_AWG1)]?.ToString() ?? "false");
            //ENABLE_AWG2 = config.ENABLE_AWG2; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_AWG2)]?.ToString() ?? "false");
            ////ENABLE_SDA = config.ENABLE_SDA; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_SDA)]?.ToString() ?? "false");
            //ENABLE_VSA = config.ENABLE_VSA; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_VSA)]?.ToString() ?? "false");
            //ENABLE_VERSION = config.ENABLE_VERSION; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_VERSION)]?.ToString() ?? "false");
            //ENABLE_DEBUG = config.ENABLE_DEBUG; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_DEBUG)]?.ToString() ?? "false");
            //ENABLE_BANDWIDTH = config.ENABLE_BANDWIDTH; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BANDWIDTH)]?.ToString() ?? "false");
            //ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL = config.ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL;
            //#region Component Service

            //ENABLE_BUS = config.ENABLE_BUS; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_BUS)]?.ToString() ?? "false");
            //ENABLE_SDA = config.ENABLE_SDA; //Boolean.Parse(AppConfigureHelper.AppSettings[nameof(ENABLE_SDA)]?.ToString() ?? "false");
            //ENABLE_AWG = config.ENABLE_AWG;
            //ENABLE_Ref = config.ENABLE_Ref;
            //ENABLE_Math = config.ENABLE_Math;
            //ENABLE_Lissajous = config.ENABLE_Lissajous;
            //ENABLE_PassFail = config.ENABLE_PassFail;
            //ENABLE_PowerAs = config.ENABLE_PowerAs;
            //ENABLE_Measure = config.ENABLE_Measure;
            //ENABLE_Search = config.ENABLE_Search;
            //ENABLE_Segement = config.ENABLE_Segement;

            //#endregion Component Service

            PRODUCT = Enum.Parse<ProductType>(config.PRODUCT);// Enum.Parse<ProductType>(AppConfigureHelper.AppSettings[nameof(PRODUCT)]?.ToString() ?? "Base");
            AFC_FREQ_RESPONSE_CALI = config.AFC_FREQ_RESPONSE_CALI;
            PFC_FREQ_RESPONSE_CALI = config.PFC_FREQ_RESPONSE_CALI;
            ANA_CHNL_TYPE = Enum.Parse<AnaChnlType>(config.ANA_CHNL_TYPE);
            SAMPLING_RATE = config.SAMPLING_RATE;// Double.Parse(AppConfigureHelper.AppSettings[nameof(SAMPLING_RATE)]?.ToString() ?? "10E9");
            ACQ_BOARD_NUM = config.ACQ_BOARD_NUM;
            ADC_BITS = config.ADC_BITS;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(ADC_BITS)]?.ToString() ?? "12");
            ADC_NUM = config.ADC_NUM;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(ADC_NUM)]?.ToString() ?? "2");
            ADC_CORE_NUM = config.ADC_CORE_NUM;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(ADC_CORE_NUM)]?.ToString() ?? "4");
            CORE_DATA_NUM = 1000;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(CORE_DATA_NUM)]?.ToString() ?? "1000");
            CHNL_DATA_NUM = config.CHNL_DATA_NUM;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(CHNL_DATA_NUM)]?.ToString() ?? "10000");
            VIS_ADC_RES = config.VIS_ADC_RES;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(VIS_ADC_RES)]?.ToString() ?? "4000");
            INPUT_SOURCE_SELECTABILITY = (PRODUCT == ProductType.B21_HB8G || PRODUCT == ProductType.B21_MD8G);

            SORT_MARKER_SELECTABILITY = !(PRODUCT == ProductType.JiHe_MSO7000X || PRODUCT == ProductType.JiHe_MSO7000A || PRODUCT == ProductType.JiHe_UPO7000L);
            MSO7000 = PRODUCT == ProductType.JiHe_MSO7000X || PRODUCT == ProductType.JiHe_MSO7000A || PRODUCT == ProductType.JiHe_UPO7000L;
            BOAED_INDEX = PRODUCT switch
            {
                ProductType.JiHe_MSO7000X => PROCESS_BOARD_K7,
                ProductType.JiHe_UPO7000L => PCIE_BOARD_INDEX,
                _ => PROCESS_BOARD_K7
            };

            MAX_YDIVS_NUM = config.MAX_YDIVS_NUM;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(MAX_YDIVS_NUM)]?.ToString() ?? "11");
            VIS_YDIVS_NUM = config.VIS_YDIVS_NUM;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(VIS_YDIVS_NUM)]?.ToString() ?? "10");
            IDX_PER_YDIV = config.IDX_PER_YDIV;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(IDX_PER_YDIV)]?.ToString() ?? "1000");

            VIS_XDIVS_NUM = config.VIS_XDIVS_NUM;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(VIS_XDIVS_NUM)]?.ToString() ?? "10");
            IDX_PER_XDIV = config.IDX_PER_XDIV;//Int32.Parse(AppConfigureHelper.AppSettings[nameof(IDX_PER_XDIV)]?.ToString() ?? "1000");
            MIN_XPOS_TIME = config.MIN_XPOS_TIME;// Double.Parse(AppConfigureHelper.AppSettings[nameof(MIN_XPOS_TIME)]?.ToString() ?? "2000.0");

            //HZBANDWIDTH_LV0_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV0_NAME)]?.ToString() ?? "FULL";
            //HZBANDWIDTH_LV1_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV1_NAME)]?.ToString() ?? "HALF";
            //HZBANDWIDTH_LV2_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV2_NAME)]?.ToString() ?? "20MHz";
            //HZBANDWIDTH_LV3_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV3_NAME)]?.ToString() ?? "";
            //HZBANDWIDTH_LV4_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV4_NAME)]?.ToString() ?? "";
            //HZBANDWIDTH_LV5_NAME = AppConfigureHelper.AppSettings[nameof(HZBANDWIDTH_LV5_NAME)]?.ToString() ?? "";

            //LZBANDWIDTH_LV0_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV0_NAME)]?.ToString() ?? "FULL";
            //LZBANDWIDTH_LV1_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV1_NAME)]?.ToString() ?? "HALF";
            //LZBANDWIDTH_LV2_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV2_NAME)]?.ToString() ?? "20MHz";
            //LZBANDWIDTH_LV3_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV3_NAME)]?.ToString() ?? "";
            //LZBANDWIDTH_LV4_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV4_NAME)]?.ToString() ?? "";
            //LZBANDWIDTH_LV5_NAME = AppConfigureHelper.AppSettings[nameof(LZBANDWIDTH_LV5_NAME)]?.ToString() ?? "";


            MEASUREGATE_DIVS = config.MEASUREGATE_DIVS;// Double.Parse(AppConfigureHelper.AppSettings[nameof(MEASUREGATE_DIVS)]?.ToString() ?? "1");

            STP_DESKEW_FS = 10;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(STP_DESKEW_FS)]?.ToString() ?? "10");

            RENDERINGMODE = Enum.Parse<RenderingMode>(config.RenderingMode/*AppConfigureHelper.AppSettings[nameof(RenderingMode)]?.ToString() ?? RenderingMode.Default.ToString()*/);

            TRIGGER_DEFAULT_SENSITIVITY_MDIV = config.TRIGGER_DEFAULT_SENSITIVITY_MDIV;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(TRIGGER_DEFAULT_SENSITIVITY_MDIV)]?.ToString() ?? "500");


            MAX_FIGURE_NUM = config.MAX_FIGURE_NUM;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(MAX_FIGURE_NUM)]?.ToString() ?? "16");

            MAX_ADC_RES = 1 << ADC_BITS;
            SAMPS_PER_YDIV = VIS_ADC_RES / VIS_YDIVS_NUM;

            MAX_YPOS_IDX = DEF_YPOS_IDX + MAX_YDIVS_NUM / 2.0 * IDX_PER_YDIV;
            MIN_YPOS_IDX = DEF_YPOS_IDX - MAX_YDIVS_NUM / 2.0 * IDX_PER_YDIV;

            VIS_CENTRT_ADC = MAX_ADC_RES >> 1;
            VIS_MAX_ADC = (Int32)(VIS_CENTRT_ADC + SAMPS_PER_YDIV * VIS_YDIVS_NUM / 2.0);
            VIS_MIN_ADC = (Int32)(VIS_CENTRT_ADC - SAMPS_PER_YDIV * VIS_YDIVS_NUM / 2.0);

            DEF_XPOS_IDX = VIS_XDIVS_NUM / 2.0 * IDX_PER_XDIV;
            MAX_XPOS_IDX = VIS_XDIVS_NUM * IDX_PER_XDIV;

            EXT_TRIGGER_RES_MV = (EXT_TRIGGER_MAX_MV - EXT_TRIGGER_MIN_MV) / VIS_YDIVS_NUM / IDX_PER_YDIV;
            MAX_TRIGGER_IDX = Constants.VIS_YDIVS_NUM / 2 * Constants.IDX_PER_YDIV;
            MIN_TRIGGER_IDX = -MAX_TRIGGER_IDX;

            MAX_VCURSOR_IDX = DEF_YPOS_IDX + VIS_YDIVS_NUM / 2.0 * IDX_PER_YDIV;
            MIN_VCURSOR_IDX = DEF_YPOS_IDX - VIS_YDIVS_NUM / 2.0 * IDX_PER_YDIV;
            MAX_HCURSOR_IDX = Constants.MAX_XPOS_IDX;

            MAX_BIT_RATE = SAMPLING_RATE / 2;

            ERES_MAX_ADC_RES = 1 << (ADC_BITS + ERES_ADC_BITS);
            ERES_VIS_ADC_RES = VIS_ADC_RES * (1 << ERES_ADC_BITS);
            ERES_SAMPS_PER_YDIV = ERES_VIS_ADC_RES / VIS_YDIVS_NUM;

            COMPORTNUM_KEYBOARD = config.COMPORTNUM_KEYBOARD;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(COMPORTNUM_KEYBOARD)]?.ToString() ?? "1");
            COMPORTNUM_ANALGCHANNEL1 = config.COMPORTNUM_ANALGCHANNEL1;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(COMPORTNUM_ANALGCHANNEL1)]?.ToString() ?? "2");
            COMPORTNUM_ANALGCHANNEL2 = 3;// Int32.Parse(AppConfigureHelper.AppSettings[nameof(COMPORTNUM_ANALGCHANNEL2)]?.ToString() ?? "3");

            ANALOG_TEMPERATURE_COMPENSATE = config.ANALOG_TEMPERATURE_COMPENSATE;
            ANALOGCHANNEL_WORKING_TEMPERATURE = config.ANALOGCHANNEL_WORKING_TEMPERATURE;//Double.Parse(AppConfigureHelper.AppSettings[nameof(ANALOGCHANNEL_WORKING_TEMPERATURE)]?.ToString() ?? "55");
            HARDDISK_WORKING_TEMPERATURE = config.HARDDISK_WORKING_TEMPERATURE;// Double.Parse(AppConfigureHelper.AppSettings[nameof(HARDDISK_WORKING_TEMPERATURE)]?.ToString() ?? "55");
            OPTION_LIMIT = config.OPTION_LIMIT;//Int64.Parse(AppConfigureHelper.AppSettings[nameof(OPTION_LIMIT)]?.ToString() ?? "00000000");

            //ProductModel = AppConfig.GetIntance().ProductModel.AsReadOnly();
            SEGMENT_FRAME_SPAN_COUNT_DEFAULT = PRODUCT == ProductType.JiHe_UPO7000L ? 20 : 40;

            UPO_HEIGHT = PRODUCT switch
            {
                ProductType.JiHe_MSO8000HD => 400,
                ProductType.JiHe_MSO8000X => 400,
                _ => 240
            };
        }
        public static readonly Int32 COMPORTNUM_KEYBOARD = 4;
        public static readonly Int32 COMPORTNUM_ANALGCHANNEL1 = 2;
        public static readonly Int32 COMPORTNUM_ANALGCHANNEL2 = 3;

        public static readonly Boolean INPUT_SOURCE_SELECTABILITY;
        public static readonly Boolean SORT_MARKER_SELECTABILITY;
        public static readonly Boolean MSO7000;

        public static readonly Int32 MAX_FIGURE_NUM;

        public static readonly Boolean BOARD_ATTACHED;
        public static readonly Boolean ENABLE_MATLAB = false;
        public static readonly Boolean ENABLE_WEBCORE = false;
        public static readonly Boolean ENABLE_LXI = false;
        public static readonly Boolean ENABLE_USB = false;
        public static readonly Boolean ENABLE_USB_SUPERSPEED = false;
        public static readonly Boolean ENABLE_STYLE = false;
        public static readonly Boolean ENABLE_RF = false;
        public static readonly Boolean ENABLE_LA = false;
        public static readonly Boolean PROBE_FACT_CALIB = false;
        public static readonly Boolean AFC_FREQ_RESPONSE_CALI = true;
        public static readonly Boolean PFC_FREQ_RESPONSE_CALI = true;

        public static readonly Boolean ENABLE_AWG_PROTECT = false;
        public static readonly Boolean ENABLE_AWG1 = false;
        public static readonly Boolean ENABLE_AWG2 = false;
        public static readonly Boolean ENABLE_VSA = false;
        public static readonly Boolean ENABLE_VERSION = false;
        public static readonly Boolean ENABLE_BANDWIDTH = false;

        public static readonly Int32 PCIE_BOARD_INDEX = 0;
        public static readonly Int32 PROCESS_BOARD_K7 = 2;
        public static readonly Int32 BOAED_INDEX;

        #region Component Enable

        public static readonly Boolean ENABLE_BUS = false;
        public static readonly Boolean ENABLE_SDA = true;//Jitter
        public static readonly Boolean ENABLE_Segement = false;
        public static readonly Boolean ENABLE_Search = false;
        //public static readonly Boolean ENABLE_FFT = false;
        public static readonly Boolean ENABLE_Ref = false;
        public static readonly Boolean ENABLE_Math = false;
        public static readonly Boolean ENABLE_Measure = false;
        public static readonly Boolean ENABLE_PassFail = true;
        public static readonly Boolean ENABLE_PowerAs = false;
        public static readonly Boolean ENABLE_Lissajous = false;
        public static readonly Boolean ENABLE_AWG = false;
        public static readonly Boolean ENABLE_VOLTMETER = true;
        public static readonly Boolean ENABLE_CYMOMETER = true;

        #endregion Component Enable

        public static Boolean ENABLE_DEBUG
        {
            get;
            private set;
        } = false;
        public static Boolean ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL
        {
            get;
            private set;
        } = true;

        public static readonly ProductType PRODUCT;
        public static readonly AnaChnlType ANA_CHNL_TYPE = AnaChnlType.ANA_2D5G;
        public static readonly RenderingMode RENDERINGMODE;

        public static readonly Double SAMPLING_RATE;

        public static readonly Double MEASUREGATE_DIVS;

        public static readonly Int32 TRIGGER_DEFAULT_SENSITIVITY_MDIV;

        public static readonly Int32 ACQ_BOARD_NUM;
        public static readonly Int32 ADC_BITS;
        public static readonly Int32 ADC_NUM;
        public static readonly Int32 ADC_CORE_NUM;
        public static readonly Int32 CORE_DATA_NUM;
        public static readonly Int32 CHNL_DATA_NUM;

        public static readonly Int32 MAX_ADC_RES;
        public static readonly Int32 VIS_ADC_RES;
        public static readonly Int32 VIS_CENTRT_ADC;
        public static readonly Int32 VIS_MAX_ADC;
        public static readonly Int32 VIS_MIN_ADC;

        public static readonly Int32 MAX_YDIVS_NUM = 11;
        public static readonly Int32 VIS_YDIVS_NUM = 10;
        public static readonly Double SAMPS_PER_YDIV;

        public const Int32 ERES_ADC_BITS = 4;
        public static readonly Double ERES_SAMPS_PER_YDIV;
        public static readonly Int32 ERES_MAX_ADC_RES;
        public static readonly Int32 ERES_VIS_ADC_RES;

        public const Double DEF_YPOS_IDX = 0;
        public static readonly Int32 IDX_PER_YDIV = 1000;
        public static readonly Double MAX_YPOS_IDX;
        public static readonly Double MIN_YPOS_IDX;
        public const Double STP_YPOS_IDX = 1;

        public static readonly Int32 VIS_XDIVS_NUM = 10;

        public static readonly Int32 IDX_PER_XDIV = 1000;
        public static readonly Double DEF_XPOS_IDX;
        public static readonly Double MAX_XPOS_IDX;
        public static readonly Double MIN_XPOS_TIME;
        public const Double STP_XPOS_IDX = 1;


        public static readonly Boolean ANALOG_TEMPERATURE_COMPENSATE;
        public static readonly Double ANALOGCHANNEL_WORKING_TEMPERATURE;
        public static readonly Double HARDDISK_WORKING_TEMPERATURE;
        public static readonly Int64 OPTION_LIMIT;

        public const Int32 MIN_AVERAGE_CNT = 2;
        public const Int32 MAX_AVERAGE_CNT = 65536;

        public const Int32 MAX_ENVELOPE_CNT = 65536;
        public const Int32 MIN_ENVELOPE_CNT = 2;

        public const Int64 MAX_BIAS_UV = 16_000_000;
        public const Int64 MIN_BIAS_UV = -MAX_BIAS_UV;
        public const Int64 STP_BIAS_UV = 10;
        public const Int64 FAC_BIAS_50 = 2;
        public const Int64 FAC_BIAS_1M = 25;

        /// <summary>
        /// 最大通道延时时间，单位:s 当前值：800ps
        /// </summary>
        public const Double MAX_CHANNEL_DELAY = 800d / 1000d / 1000d / 1000d / 1000d;

        /// <summary>
        /// 最小通道延时时间，单位:s 当前值：800ps
        /// </summary>
        public const Double MIN_CHANNEL_DELAY = -800d / 1000d / 1000d / 1000d / 1000d;

        public const Int64 MIN_HOLDOFF_PS = 6400;
        public const Int64 STP_HOLDOFF_PS = 4000;
        public const Int64 MAX_HOLDOFF_PS = 10_000_000_000_000;
        public const Int32 MIN_HOLDOFF_EVENT = 1;
        public const Int32 MAX_HOLDOFF_EVENT = 65_536;


        public static readonly Int64 STP_DESKEW_FS = 10;
        public const Int64 MAX_DESKEW_FS = 1_000_000_000_000;
        public const Int64 MIN_DESKEW_FS = -MAX_DESKEW_FS;

        public static readonly Double EXT_TRIGGER_RES_MV;

        public const Double EXT_TRIGGER_MAX_MV = 1000;

        public const Double EXT_TRIGGER_MIN_MV = -1000;

        public const ushort EXT_TRIGGER_ACDC_POSITION_DEFAULT = 0x59D8;
        public const ushort EXT_TRIGGER_NR_POSITION_DEFAULT = 0x6500;

        public static readonly Int32 EXT_TRIGGER_ATTEN_FAC = 10;

        public static readonly Double MAX_TRIGGER_IDX;
        public static readonly Double MIN_TRIGGER_IDX;
        public const Double STP_TRIGGER_IDX = 1;
        public const Double MIN_TRIGGER_GAP = 100;

        public const Int16 MIN_TRIGGER_SENSITIVITY_MDIV = -4000;
        public const Int16 MAX_TRIGGER_SENSITIVITY_MDIV = 4000;

        public const Int64 MIN_PULSEWIDTH_PS = 3200;
        public const Int64 STP_PULSEWIDTH_PS = 400;
        public const Int64 MAX_PULSEWIDTH_PS = 10_000_000_000_000;

        public const Int64 MIN_GLITCH_PS = 200;
        public const Int64 STP_GLITCH_PS = 200;
        public const Int64 MAX_GLITCH_PS = 10_000_000_000_000;

        public const Int64 MIN_TIMEOUT_PS = 3200;
        public const Int64 STP_TIMEOUT_PS = STP_PULSEWIDTH_PS;
        public const Int64 MAX_TIMEOUT_PS = MAX_PULSEWIDTH_PS;

        public const Int64 MIN_MULTISTAGE_TIME_PS = 2000;
        public const Int64 STP_MULTISTAGE_TIME_PS = 4000;
        public const Int64 MAX_MULTISTAGE_TIME_PS = 10_000_000_000_000;
        public const Int32 MIN_MULTISTAGE_EVENT = 1;
        public const Int32 MAX_MULTISTAGE_EVENT = 65_536;

        public const Int32 DEF_WAVE_INTENSITY = 65;
        public const Int32 DEF_GRID_INTENSITY = 10;



        //public static IReadOnlyList<(Boolean Is2GHz, String Model)> ProductModel;

        #region Digital
        public const Int32 DIGI_HYSTE_MIN = -4000;
        public const Int32 DIGI_HYSTE_MAX = 4000;
        public const Int32 DIGI_HYSTE_STP = 10;

        public const Int32 DIGI_THROLDIDX_MIN = DIGI_THROLD_MIN / DIGI_THROLD_STP;
        public const Int32 DIGI_THROLDIDX_MAX = DIGI_THROLD_MAX / DIGI_THROLD_STP;
        public const Int32 DIGI_THROLD_STP = 10;

        public const Int32 DIGI_THROLD_FSTP = 10;
        public const Int32 DIGI_THROLD_CSTP = 100;

        public const Int32 DIGI_THROLD_MIN = -20000;
        public const Int32 DIGI_THROLD_MAX = 20000;

        public const Int32 DIGI_TTL_THROLD = 1400;
        public const Int32 DIGI_TTL_HYSTE = 1000;

        public const Int32 DIGI_CMOS50_THROLD = 2500;
        public const Int32 DIGI_CMOS50_HYSTE = 1000;

        public const Int32 DIGI_CMOS33_THROLD = 1650;
        public const Int32 DIGI_CMOS33_HYSTE = 1000;

        public const Int32 DIGI_CMOS25_THROLD = 1250;
        public const Int32 DIGI_CMOS25_HYSTE = 1000;

        public const Int32 DIGI_CMOS18_THROLD = 900;
        public const Int32 DIGI_CMOS18_HYSTE = 1000;

        public const Int32 DIGI_ECL_THROLD = -1300;
        public const Int32 DIGI_ECL_HYSTE = 60;

        public const Int32 DIGI_PECL_THROLD = 3700;
        public const Int32 DIGI_PECL_HYSTE = 70;

        public const Int32 DIGI_LVDS_THROLD = 1200;
        public const Int32 DIGI_LVDS_HYSTE = 100;
        #endregion Digital

        public static readonly Double MAX_VCURSOR_IDX;
        public static readonly Double MIN_VCURSOR_IDX;

        public static readonly Double MAX_HCURSOR_IDX;
        public static readonly Double MIN_HCURSOR_IDX = 0;

        #region Jitter

        public const Int32 MIN_HIST_BIN_CNT = 25;
        public const Int32 MAX_HIST_BIN_CNT = 2000;
        public const Int32 DEFAULT_HIST_BIN_CNT = 2000;
        public const Int32 JITTER_HIST_MAX_RECORD_NUMBER = 1_000_000;
        public const Boolean LIMITED_EYE_CHART_DISPALY_RANGE = true;

        #endregion

        public const Int32 MAX_TREND_LENGTH = 10_000;

        #region Option 

        public const Int32 OPTIONNAME_MAX_LENGTH = 25;
        public const Int32 OPTIONS_MAX_COUNT = 1000;
        public const Double DEFAULT_REMAININGTIME_BYHOUR = 168.0;

        #endregion

        #region Waveform Generator

        public const Int32 AWG_RES_MAX = 1_000_000; // 1MΩ
        public const Int32 AWG_RES_MIN = 5; // 5 Ω
        public const Int32 AWG_RES_DEF = 50; //50 Ω

        public const Int64 AWG_PERIOD_STP1 = 5;   //5ns
        public const Int64 AWG_PERIOD_STP2 = 10;  //10ns

        public const Int64 AWG_FRQ_STP0 = 1;      //1uHz 产品指标对齐 
        public const Int64 AWG_FRQ_STP1 = 100_000;      //100mHz
        public const Int64 AWG_FRQ_STP2 = 500_000;      //500mHz

        public const Int64 AWG_FRQ_CORNER1 = 1_000_000;   //1Hz
        public const Int64 AWG_FRQ_CORNER2 = 10_000_000_000;   //10kHz
        public const Int64 AWG_AMP_FREQ_HIGH = 30_000_000_000_000; // 30 MHz 高频临界

        /*<Remark>更改人：彭博 
         * 修改日期：2024/4/08 10:46:00 原值：1mHz  原因：指标不应该降低
         * 创建日期：2024/2/29 19:10:00 原值：1uHz  原因：建议最小值为1mHz 
         * </Remark>
         */
        public const Int64 AWG_SIN_FRQ_MIN = 1;   //1mHz
        public const Int64 AWG_SIN_FRQ_MAX = 60_000_000_000_000;   //60MHz 产品指标对齐 
        public const Int64 AWG_SIN_FRQ_DEF = 1_000_000_000;   //1kHz

        public const Int64 AWG_SQUARE_FRQ_MIN = 100_000;   //100mHz
        public const Int64 AWG_SQUARE_FRQ_MAX = 25_000_000_000_000;   //25MHz  产品指标对齐 

        public const Int64 AWG_PULSE_FRQ_MIN = 1;   //1 μHz产品指标对齐 
        public const Int64 AWG_PULSE_FRQ_MAX = 25_000_000_000_000;   //25MHz

        public const Int32 AWG_Pulse_Edge_STP = 1;    //1 ns
        public const Int64 AWG_Pulse_Edge_TIME_NS_MIN = 5_000; //5ns 
        //<Remark>更改人：彭博 创建日期：2023/11/22 14:10:00 原值：2s  原因：与现有设备7025B的上升时间最大值一致 </Remark>
        public const Int64 AWG_Pulse_Edge_TIME_NS_MAX = 10_000_000_000_000; //10 s  

        public const Int64 AWG_RAMP_FRQ_MIN = 100_000;   //100mHz
        public const Int64 AWG_RAMP_FRQ_MAX = 1_000_000_000_000;   //1MHz  产品指标对齐 

        public const Int64 AWG_NOISE_FRQ_MIN = 100_000;   //100mHz
        public const Int64 AWG_NOISE_FRQ_MAX = 400_000_000_000;   //400kHz

        public const Int64 AWG_SWEEP_FRQ_MIN = 2_000;   //2mHz
        public const Int64 AWG_SWEEP_FRQ_MAX = AWG_SIN_FRQ_MAX;   //100kHz

        public const Int32 AWG_SWEEP_TIME_MIN = 1_000;   //1ms
        public const Int32 AWG_SWEEP_TIME_MAX = 500_000_000;   //500s
        public const Int32 AWG_SWEEP_TIME_STP = 1_000;    //1ms

        //public const Int64 AWG_BUILTIN_FRQ_MIN = 100_000;   //100mHz
        //public const Int64 AWG_BUILTIN_FRQ_MAX = 5_000_000_000_000;   //5MHz

        //<Remark>更改人：彭博 创建日期：2023/11/23 9:43:00 原值：1uHz  原因：与技术手册不一致，技术手册要求为100mHz </Remark>
        public const Int64 AWG_ARB_FRQ_MIN = 100_000;   //100mHz
        //<Remark>更改人：彭博 创建日期：2023/11/23 9:42:00 原值：1MHz  原因：与技术手册不一致，技术手册要求为5MHz </Remark>
        public const Int64 AWG_ARB_FRQ_MAX = 5_000_000_000_000;   //5MHz

        public const Int64 AWG_ARB_SA_MAX = 625_000_000_000_000 / 2;   //625MSa

        public const Int32 AWG_DUTY_MIN = 1;    //0.01%
        public const Int32 AWG_DUTY_MAX = 9999;   //99.99%
        public const Int32 AWG_DUTY_STP = 1;    //0.01%, max(1%, 10ns)
        public const Int32 AWG_DUTY_DEF = 5000;    //50%

        public const Int32 AWG_PHASE_MIN = 0;    //0°
        public const Int32 AWG_PHASE_MAX = 35999;   //359.99°
        public const Int32 AWG_PHASE_HALF = 18000;   //180.00°
        public const Int32 AWG_PHASE_STP = 1;    //0.01°
        public const Int32 AWG_PHASE_DEF = 0;    //0°

        public const Int32 AWG_NOISE_MIN = 0;    //0%
        public const Int32 AWG_NOISE_MAX = 10000;  //100.00%
        public const Int32 AWG_NOISE_STP = 1;    //0.01%
        public const Int32 AWG_NOISE_DEF = 2;    //0.02%


        public const Int64 AWG_PULSE_WIDTH_MIN = 20;    //20ns
        public const Int64 AWG_PULSE_WIDTH_STP = 10;    //10ns, max(1%, 10ns)

        public const Int32 AWG_ARB_LEN_MIN = 8;
        public const Int32 AWG_ARB_LEN_MAX = 512 * 1024;
        public const Int32 AWG_ARB_CNT = 10;

        public const Int32 AWG_AMP_1M_MIN = 20;         //20mV(High-Z)
        public const Int32 AWG_AMP_1M_MAX = 6_000;      //6V(High-Z)
        public const Int32 AWG_AMP_1M_DEF = 1_000;      //1V(High-Z)
        public const Int32 AWG_AMP_1M_MAX_HIGH_FREQ = 3_000;      //4V(High-Z)

        public const Int32 AWG_AMP_50_MIN = 10;         //10mV(50)
        public const Int32 AWG_AMP_50_MAX = 3_000;      //3V(50)
        public const Int32 AWG_AMP_50_DEF = 1_000;      //1V(50)
        public const Int32 AWG_AMP_50_MAX_HIGH_FREQ = 1_500;      //2V(High-Z)

        public const Int32 AWG_AMP_STP = 1;     //1mV

        public const Int32 AWG_OFS_1M_MIN = -3_000;     //-3V
        public const Int32 AWG_OFS_1M_MAX = 3_000;      //3V
        public const Int32 AWG_OFS_1M_DEF = 0;          //0V

        public const Int32 AWG_OFS_50_MIN = -1_500;     //-1.50V
        public const Int32 AWG_OFS_50_MAX = 1_500;      //1.50V
        public const Int32 AWG_OFS_50_DEF = 0;          //0V

        public const Int32 AWG_OFS_STP = 1;     //1mV

        public const Int64 AWG_MODEM_FRQ_MIN = 2_000;           //2mHz
        //<Remark>更改人：彭博 创建日期：2023/11/23 10:40:00 原值：50kHz  原因：与技术手册不一致，技术手册要求为200kHz </Remark>
        public const Int64 AWG_MODEM_FRQ_MAX = 200_000_000_000;  //50kHz

        public const Int32 AWG_AM_DEPTH_MIN = 1;            //1%
        public const Int32 AWG_AM_DEPTH_MAX = 120;          //120%
        public const Int32 AWG_AM_DEPTH_STP = 1;            //1%
        public const Int32 AWG_AM_DEPTH_DEF = 50;           //50%

        //<Remark>更改人：彭博 创建日期：2023/11/23 17:16:00 原值：2mHz  原因：与技术手册不一致，技术手册要求为0Hz </Remark>
        public const Int64 AWG_FM_BIAS_MIN = 0;            //0Hz
        //<Remark>更改人：彭博 创建日期：2023/11/23 17:16:00 原值：12.5kHz  原因：与技术手册不一致，技术手册要求为30MHz </Remark>
        public const Int64 AWG_FM_BIAS_MAX = 30_000_000_000_000;          //30MHz
        public const Int64 AWG_FM_BIAS_STP = 2_000;            //2mHz
        public const Int64 AWG_FM_BIAS_DEF = 1_000_000_000;    //1kHz
        public const Int32 AWG_MAX_NUM = 2;     //2 
        #region Regedit
        public const UInt16 AWG1_WORK_MODE = 0x0;
        public const UInt16 AWG1_SIGNAL_SEL = 0x01;
        public const UInt16 AWG1_MODULA_WAVE_SEL = 0x02;

        public const UInt16 AWG1_SIGNAL_PHASE_INC_0 = 0x03;
        public const UInt16 AWG1_SIGNAL_PHASE_INC_8 = 0x04;
        public const UInt16 AWG1_SIGNAL_PHASE_INC_16 = 0x05;
        public const UInt16 AWG1_SIGNAL_PHASE_INC_24 = 0x06;
        public const UInt16 AWG1_SIGNAL_PHASE_INC_32 = 0x07;
        public const UInt16 AWG1_SIGNAL_PHASE_INC_40 = 0x08;

        public const UInt16 AWG1_PHASE_OFFSET_0 = 0x09;
        public const UInt16 AWG1_PHASE_OFFSET_8 = 0x0A;
        public const UInt16 AWG1_PHASE_OFFSET_16 = 0x0B;
        public const UInt16 AWG1_PHASE_OFFSET_24 = 0x0C;

        public const UInt16 AWG1_FREQ_SWEEP_STOP_0 = 0x13;
        public const UInt16 AWG1_FREQ_SWEEP_STOP_8 = 0x14;
        public const UInt16 AWG1_FREQ_SWEEP_STOP_16 = 0x15;
        public const UInt16 AWG1_FREQ_SWEEP_STOP_24 = 0x16;
        public const UInt16 AWG1_FREQ_SWEEP_STOP_32 = 0x17;
        public const UInt16 AWG1_FREQ_SWEEP_STOP_40 = 0x18;

        public const UInt16 AWG1_PULSE_REDGE_OFFSET_0 = 0x2B;
        public const UInt16 AWG1_PULSE_REDGE_OFFSET_8 = 0x2C;
        public const UInt16 AWG1_PULSE_REDGE_OFFSET_16 = 0x2D;

        public const UInt16 AWG1_PULSE_FEDGE_OFFSET_0 = 0x2E;
        public const UInt16 AWG1_PULSE_FEDGE_OFFSET_8 = 0x2F;
        public const UInt16 AWG1_PULSE_FEDGE_OFFSET_16 = 0x30;

        public const UInt16 AWG1_RSTEP_SHIFT = 0x31;
        public const UInt16 AWG1_FSTEP_SHIFT = 0x32;

        public const UInt16 AWG1_CH_SW = 0x40;
        public const UInt16 AWG1_CNT_ATTEN = 0x3A;
        public const UInt16 AWG1_VOL_ADJ_QUOT_L = 0x3C;
        public const UInt16 AWG1_VOL_ADJ_QUOT_H = 0x3D;

        public const UInt16 AWG1_MODULA_INC_0 = 0x19;
        public const UInt16 AWG1_MODULA_INC_8 = 0x1A;
        public const UInt16 AWG1_MODULA_INC_16 = 0x1B;
        public const UInt16 AWG1_MODULA_INC_24 = 0x1C;
        public const UInt16 AWG1_MODULA_INC_32 = 0x1D;
        public const UInt16 AWG1_MODULA_INC_40 = 0x1E;

        public const UInt16 AWG1_MODULA_DEGREE_0 = 0x1F;
        public const UInt16 AWG1_MODULA_DEGREE_8 = 0x20;
        public const UInt16 AWG1_MODULA_DEGREE_16 = 0x21;
        public const UInt16 AWG1_MODULA_DEGREE_24 = 0x22;
        public const UInt16 AWG1_MODULA_DEGREE_32 = 0x23;
        public const UInt16 AWG1_MODULA_DEGREE_40 = 0x24;

        public const UInt16 AWG1_PULSE_DUTY_0 = 0x25;
        public const UInt16 AWG1_PULSE_DUTY_8 = 0x26;
        public const UInt16 AWG1_PULSE_DUTY_16 = 0x27;
        public const UInt16 AWG1_PULSE_DUTY_24 = 0x28;
        public const UInt16 AWG1_PULSE_DUTY_32 = 0x29;
        public const UInt16 AWG1_PULSE_DUTY_40 = 0x2A;

        public const UInt16 AWG1_TRI_DUTY_0 = 0x41;
        public const UInt16 AWG1_TRI_DUTY_8 = 0x42;
        public const UInt16 AWG1_TRI_DUTY_16 = 0x43;
        public const UInt16 AWG1_TRI_DUTY_24 = 0x44;
        public const UInt16 AWG1_TRI_DUTY_32 = 0x45;
        public const UInt16 AWG1_TRI_DUTY_40 = 0x46;

        public const UInt16 AWG1_TRI_RISE_0 = 0x47;
        public const UInt16 AWG1_TRI_RISE_8 = 0x48;
        public const UInt16 AWG1_TRI_RISE_16 = 0x49;

        public const UInt16 AWG1_TRI_FALL_0 = 0x4A;
        public const UInt16 AWG1_TRI_FALL_8 = 0x4B;
        public const UInt16 AWG1_TRI_FALL_16 = 0x4C;

        public const UInt16 AWG1_TRI_RISE_SHIFT = 0x4D;
        public const UInt16 AWG1_TRI_FALL_SHIFT = 0x4E;

        public const UInt16 WRITE_DATA_SEL = 0x61;
        public const UInt16 WRITE_DATA_WE = 0x62;
        public const UInt16 SHARE_FPGA_CONFIG_HDATA_L = 0x63;
        public const UInt16 SHARE_FPGA_CONFIG_HDATA_H = 0x64;
        public const UInt16 DAC124_CONFIG_DATA_L = 0x75;
        public const UInt16 DAC124_CONFIG_DATA_H = 0x76;

        public const UInt16 AWG2_WORK_MODE = 0x80;
        public const UInt16 AWG2_SIGNAL_SEL = 0x81;
        public const UInt16 AWG2_MODULA_WAVE_SEL = 0x82;

        public const UInt16 AWG2_SIGNAL_PHASE_INC_0 = 0x83;
        public const UInt16 AWG2_SIGNAL_PHASE_INC_8 = 0x84;
        public const UInt16 AWG2_SIGNAL_PHASE_INC_16 = 0x85;
        public const UInt16 AWG2_SIGNAL_PHASE_INC_24 = 0x86;
        public const UInt16 AWG2_SIGNAL_PHASE_INC_32 = 0x87;
        public const UInt16 AWG2_SIGNAL_PHASE_INC_40 = 0x88;

        public const UInt16 AWG2_PHASE_OFFSET_0 = 0x89;
        public const UInt16 AWG2_PHASE_OFFSET_8 = 0x8A;
        public const UInt16 AWG2_PHASE_OFFSET_16 = 0x8B;
        public const UInt16 AWG2_PHASE_OFFSET_24 = 0x8C;

        public const UInt16 AWG2_FREQ_SWEEP_STOP_0 = 0x93;
        public const UInt16 AWG2_FREQ_SWEEP_STOP_8 = 0x94;
        public const UInt16 AWG2_FREQ_SWEEP_STOP_16 = 0x95;
        public const UInt16 AWG2_FREQ_SWEEP_STOP_24 = 0x96;
        public const UInt16 AWG2_FREQ_SWEEP_STOP_32 = 0x97;
        public const UInt16 AWG2_FREQ_SWEEP_STOP_40 = 0x98;

        public const UInt16 AWG2_PULSE_REDGE_OFFSET_0 = 0xAB;
        public const UInt16 AWG2_PULSE_REDGE_OFFSET_8 = 0xAC;
        public const UInt16 AWG2_PULSE_REDGE_OFFSET_16 = 0xAD;

        public const UInt16 AWG2_PULSE_FEDGE_OFFSET_0 = 0xAE;
        public const UInt16 AWG2_PULSE_FEDGE_OFFSET_8 = 0xAF;
        public const UInt16 AWG2_PULSE_FEDGE_OFFSET_16 = 0xB0;

        public const UInt16 AWG2_RSTEP_SHIFT = 0xB1;
        public const UInt16 AWG2_FSTEP_SHIFT = 0xB2;

        public const UInt16 AWG2_CH_SW = 0xC0;
        public const UInt16 AWG2_CNT_ATTEN = 0xBA;
        public const UInt16 AWG2_VOL_ADJ_QUOT_L = 0xBC;
        public const UInt16 AWG2_VOL_ADJ_QUOT_H = 0xBD;

        public const UInt16 AWG2_MODULA_INC_0 = 0x99;
        public const UInt16 AWG2_MODULA_INC_8 = 0x9A;
        public const UInt16 AWG2_MODULA_INC_16 = 0x9B;
        public const UInt16 AWG2_MODULA_INC_24 = 0x9C;
        public const UInt16 AWG2_MODULA_INC_32 = 0x9D;
        public const UInt16 AWG2_MODULA_INC_40 = 0x9E;

        public const UInt16 AWG2_MODULA_DEGREE_0 = 0x9F;
        public const UInt16 AWG2_MODULA_DEGREE_8 = 0xA0;
        public const UInt16 AWG2_MODULA_DEGREE_16 = 0xA1;
        public const UInt16 AWG2_MODULA_DEGREE_24 = 0xA2;
        public const UInt16 AWG2_MODULA_DEGREE_32 = 0xA3;
        public const UInt16 AWG2_MODULA_DEGREE_40 = 0xA4;

        public const UInt16 AWG2_PULSE_DUTY_0 = 0xA5;
        public const UInt16 AWG2_PULSE_DUTY_8 = 0xA6;
        public const UInt16 AWG2_PULSE_DUTY_16 = 0xA7;
        public const UInt16 AWG2_PULSE_DUTY_24 = 0xA8;
        public const UInt16 AWG2_PULSE_DUTY_32 = 0xA9;
        public const UInt16 AWG2_PULSE_DUTY_40 = 0xAA;

        public const UInt16 AWG2_TRI_DUTY_0 = 0xC1;
        public const UInt16 AWG2_TRI_DUTY_8 = 0xC2;
        public const UInt16 AWG2_TRI_DUTY_16 = 0xC3;
        public const UInt16 AWG2_TRI_DUTY_24 = 0xC4;
        public const UInt16 AWG2_TRI_DUTY_32 = 0xC5;
        public const UInt16 AWG2_TRI_DUTY_40 = 0xC6;

        public const UInt16 AWG2_TRI_RISE_0 = 0xC7;
        public const UInt16 AWG2_TRI_RISE_8 = 0xC8;
        public const UInt16 AWG2_TRI_RISE_16 = 0xC9;

        public const UInt16 AWG2_TRI_FALL_0 = 0xCA;
        public const UInt16 AWG2_TRI_FALL_8 = 0xCB;
        public const UInt16 AWG2_TRI_FALL_16 = 0xCC;

        public const UInt16 AWG2_TRI_RISE_SHIFT = 0xCD;
        public const UInt16 AWG2_TRI_FALL_SHIFT = 0xCE;

        public const UInt16 AWG1_DATA_SET_CAL = 0x02;
        public const UInt16 AWG2_DATA_SET_CAL = 0x06;
        #endregion Regedit

        #region Calib
        public const Double AWG_CAL_FREQ_1K = 1000.0; //1 k
        public const Double AWG_CAL_FREQ_MIN_RANGE = 150e3; //150 k
                                                            //public const Int32 AWG_CAL_PIONT_ARRAY_MAX = 70;
        public const Int32 AWG_CAL_ARRAY_SCALE_MAX = 1; // todo  =>2
        public const Double AWG_CAL_DC_LOAD_GAIN = 1;
        //粗调校正值
        public const Double AWG_CAL_DAC_FINE_GAIN = (100.0 / (100.0 + 910.0));
        //粗调校正值
        public const Double AWG_CAL_DAC_COARSE_GAIN = (1.0 - AWG_CAL_DAC_FINE_GAIN);
        //衰减档位
        public const Int32 AWG_CAL_ATT_LEVEL_NUM = 4;
        //public const Double AWG_AC_CAL_STEP = (Double)AWG_CAL_MAX_FREQ / AWG_AC_CAL_SIZE;// todo
        public const Double AWG_AC_CAL_STEP = 152.587890625 * 1000 * 2; //Hz - 152.587890625 kHz
        public const Int32 AWG_CAL_MAX_FREQ = (Int32)(AWG_SIN_FRQ_MAX / 1000_000);
        //public const Int32 AWG_AC_CAL_SIZE = 1024; //MAX_FREQ / AC_CAL_STEP; //1024
        public const Int32 AWG_AC_CAL_SIZE = (Int32)(AWG_CAL_MAX_FREQ / AWG_AC_CAL_STEP);
        public const Int32 AMP_CAL_MAX = ((1 << 16) - 1); //todo 23.1.29 =>16
        public const Int32 AMP_DEFAULT = AMP_CAL_MAX;//((ushort)(AMP_CAL_MAX/1.4))  // 20479 = 5v

        #endregion Calib
        #endregion Waveform Generator

        #region File and Directory
        public static readonly String WFM_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\WaveForm";
        public static readonly String SET_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\Setting";
        public static readonly String PRNT_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\Print";
        public static readonly String PIC_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\Picture";
        public static readonly String PASSFAIL_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\PassFail";
        public static readonly String AWG_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\Awg";
        public static readonly String USERCODE_DEF_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\U2 DSO\\UserCode";

        public const String FACTORY_SET_NAME = "U2Factory";
        #endregion

        #region Pass Fail
        public const Int32 MAX_VIOLATION_NUM = 1_000;
        public const Int32 MIN_VIOLATION_NUM = 1;

        public const Int32 MAX_TEST_DURATION_MS = 1_000_000_000;
        public const Int32 MIN_TEST_DURATION_MS = 100;
        public const Int32 STP_TEST_DURATION_MS = 50;

        public const Int32 MAX_TEST_WFMS = 100_000;
        public const Int32 MIN_TEST_WFMS = 1;

        public const Int32 MAX_VERT_TOLERANCE_IDX = 1000;
        public const Int32 MAX_HORZ_TOLERANCE_IDX = 500;
        #endregion Pass Fail

        #region PLL
        public const Double STD_PLL_CUTOFF_DIVISOR = 1667;
        public const Double MAX_PLL_CUTOFF_DIVISOR = 1E6;
        public const Double MIN_PLL_CUTOFF_DIVISOR = 20;

        public const Double STD_PLL_DAMPLING_FACTOR = 0.707;
        public const Double MAX_PLL_DAMPLING_FACTOR = 2;
        public const Double MIN_PLL_DAMPLING_FACTOR = 0.5;
        #endregion

        public static readonly Double MAX_BIT_RATE;
        public const Double MIN_BIT_RATE = 0;

        public const Double MAX_HYSTERESIS_DIV = 30;
        public const Double DEF_HYSTERESIS_DIV = 20;
        public const Double MIN_HYSTERESIS_DIV = 0;

        public const Double MAX_FREQ_FAC = 1000;
        public const Double MIN_FREQ_FAC = 2.5;

        public const Double MAX_REF_CLK_DESKEW_PS = 1000;
        public const Double MIN_REF_CLK_DESKEW_PS = 0;

        public const Double MAX_THRESHOLD_PERCENT = 55;
        public const Double DEF_THRESHOLD_PERCENT = 50;
        public const Double MIN_THRESHOLD_PERCENT = 45;

        public const Double MAX_TOP_THRESHOLD_PERCENT = 95;
        public const Double DEF_TOP_THRESHOLD_PERCENT = 90;
        public const Double MIN_TOP_THRESHOLD_PERCENT = 80;

        public const Double MAX_BASE_THRESHOLD_PERCENT = 20;
        public const Double DEF_BASE_THRESHOLD_PERCENT = 10;
        public const Double MIN_BASE_THRESHOLD_PERCENT = 5;
        public const Int32 MIN_JITTER_DATA_LENGTH = 5_000_000;
        public const Int32 MAX_JITTER_DATA_LENGTH = 25_000_000;


        public const Int32 EYE_PATTERN_DEFAULT_HEIGHT = 256;
        public const Int32 EYE_PATTERN_DEFAULT_WIDTH = 256;



        public const Int32 MAX_LOG10_BER = -14;
        public const Int32 DEF_LOG10_BER = -12;
        public const Int32 MIN_LOG10_BER = -4;

        public const Double MAX_SYMBOL_RATE = 100E6;
        public const Double MIN_SYMBOL_RATE = 1E3;

        public const Int32 MAX_BITS_PER_SYM = 12;
        public const Int32 MIN_BITS_PER_SYM = 1;

        public const Int32 MAX_SAMP_PER_BAUD = 100;
        public const Int32 MIN_SAMP_PER_BAUD = 1;

        public const Double MAX_MIXER_FREQ = 1E9;
        public const Double MIN_MIXER_FREQ = 1;
        public const Double STP_MIXER_FREQ = 1E-3;

        public static readonly Double MIN_ZOOMSCALE_H = 1;
        public static readonly Double MIN_ZOOMSCALE_V = 1;
        public static readonly Double MAX_ZOOMSCALE_H = 1000;
        public static readonly Double MAX_ZOOMSCALE_V = 1000;

        public static readonly Int32 MAX_PERSISTENT_CNT = 1000;
        public static readonly Int32 MIN_PERSISTENT_CNT = 1;

        #region 分段存储
        public const Int32 SEGMENT_OBJ_PERROW = 5;
        public const Int32 SEGMENT_OBJ_ROWSPAN = 30;
        public const Int32 SEGMENT_OBJ_ROWSTART = 65;
        public const Int32 SEGMENT_OBJ_COLSTART = 5;
        public const Int32 SEGMENT_OBJ_COLSPAN = 10;
        public const Int32 SEGMENT_OBJ_WIDTH = 100;
        public const Int32 SEGMENT_OBJ_HEIGHT = 30;
        public const Int32 SEGMENT_LBL_ROWSPAN = 24;

        public const Int32 SEGMENT_FRAME_SPAN_COUNT_MAX = 128;
        public static readonly Int32 SEGMENT_FRAME_SPAN_COUNT_DEFAULT;

        public const Int32 SEGMENT_FRAME_COUNT_MIN = 2;
        public const Int32 SEGMENT_FRAME_COUNT_DEFAULT = 25;

        //FPGA里面计算的分段存储的段数最大值
        //1、DDR总地址长度，这个FPGA里面是个常数：29'h1FFF_FFFF
        //2、存触发地址的起始地址，这个FPGA里面是个常数：29'h1FC0_0000
        //3、存触发地址的最大数目，地址8突发
        //	maxcount =（29'h1FFF_FFFF - 29'h1FC0_0000）/8 = 7FFF;
        public const Int32 SEGMENT_FRAME_COUNT_MAX = 0x7FFFF;

        public const Int32 MAX_SEARCH_CNT = 10;

        #endregion

        public const Double DEF_SOA_LOGY_MAX = 100;
        public const Double MAX_SOA_LOGY_MAX = 10_000;
        public const Double DEF_SOA_LOGY_MIN = 0.1;
        public const Double MIN_SOA_LOGY_MIN = 0.001;
        public const Double DEF_SOA_LINY_MAX = 50;
        public const Double MAX_SOA_LINY_MAX = 10_000;
        public const Double DEF_SOA_LINY_MIN = 0;
        public const Double MIN_SOA_LINY_MIN = -10_000;

        public const Double DEF_SOA_LOGX_MAX = 1_000;
        public const Double MAX_SOA_LOGX_MAX = 10_000;
        public const Double DEF_SOA_LOGX_MIN = 0.1;
        public const Double MIN_SOA_LOGX_MIN = 0.001;
        public const Double DEF_SOA_LINX_MAX = 500;
        public const Double MAX_SOA_LINX_MAX = 10_000;
        public const Double DEF_SOA_LINX_MIN = 0;
        public const Double MIN_SOA_LINX_MIN = -10_000;

        public const Double DELTA_SOA_LIN = 0.01;
        public const Double DELTA_SOA_LOG = 10;

        public const Double DEF_SOA_POWER = 750;
        public const Double MAX_SOA_POWER = 100_000;
        public const Double MIN_SOA_POWER = 0.001;
        public const Double DEF_SOA_CUR = 30;
        public const Double MAX_SOA_CUR = 10_000;
        public const Double MIN_SOA_CUR = 0.001;
        public const Double DEF_SOA_VOL = 300;
        public const Double MAX_SOA_VOL = 10_000;
        public const Double MIN_SOA_VOL = 0.001;

        public const Double S_RELATIVE_TO_MS = 1e3;
        public const Double S_RELATIVE_TO_US = 1e6;
        public const Double S_RELATIVE_TO_NS = 1e9;
        public const Double S_RELATIVE_TO_PS = 1e12;
        public const Double S_RELATIVE_TO_FS = 1e15;

        public const Double MS_RELATIVE_TO_S = 1e-3;
        public const Double US_RELATIVE_TO_S = 1e-6;
        public const Double NS_RELATIVE_TO_S = 1e-9;
        public const Double PS_RELATIVE_TO_S = 1e-12;
        public const Double FS_RELATIVE_TO_S = 1e-15;

        #region DPX
        public static Int32 UPO_WIDTH = 1250;
        public static Int32 UPO_HEIGHT = 240;
        #endregion

        #region 通道面板
        public const byte OUTER_PANNEL_LED_RED = 0xFF;
        public const byte OUTER_PANNEL_LED_GREEN = 0xFF;
        public const byte OUTER_PANNEL_LED_BLUE = 0xFF;
        #endregion 通道面板


        public const String FORMULA = "()";
        public const String DATA = "  ";
        #region 抖动分析
        public const String JITTER_TREND = "JitterTrend";
        public const String JITTER_SPECTRUM = "JitterSpectrum";
        public const String JITTER_HISTOGRAM = "JitterHist";
        public const String JITTER_EYE = "JitterEye";
        public const String JITTER_BATHTUB = "JitterBathWave";
        public const String JITTER_QFACTOR = "JitterQWave";

        public const String JITTER_TREND_FORMULA = JITTER_TREND + FORMULA;
        public const String JITTER_SPECTRUM_FORMULA = JITTER_SPECTRUM + FORMULA;
        public const String JITTER_HISTOGRAM_FORMULA = JITTER_HISTOGRAM + FORMULA;
        public const String JITTER_EYE_FORMULA = JITTER_EYE + FORMULA;
        public const String JITTER_BATHTUB_FORMULA = JITTER_BATHTUB + FORMULA;
        public const String JITTER_QFACTOR_FORMULA = JITTER_QFACTOR + FORMULA;


        public const String DATA_JITTER_TREND = DATA + JITTER_TREND;
        public const String DATA_JITTER_SPECTRUM = DATA + JITTER_SPECTRUM;
        public const String DATA_JITTER_HISTOGRAM = DATA + JITTER_HISTOGRAM;
        public const String DATA_JITTER_EYE = DATA + JITTER_EYE;
        public const String DATA_JITTER_BATHTUB = DATA + JITTER_BATHTUB;
        public const String DATA_JITTER_QFACTOR = DATA + JITTER_QFACTOR;

        #endregion

        #region ERes
        public static readonly Double StepEnhancedBit = 0.5;

        public static readonly Double MaxEnhancedBits = 3.0;

        public static readonly Double MinEnhancedBits = 0.5;
        #endregion

        public static void Set(string name, string value)
        {
            switch (name)
            {
                case "ENABLE_DEBUG":
                    ENABLE_DEBUG = value.ToLower() == "true";
                    break;
            }
        }
    }
}
