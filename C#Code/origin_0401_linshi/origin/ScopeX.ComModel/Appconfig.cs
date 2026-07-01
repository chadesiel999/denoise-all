using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ScopeX.ComModel
{
    public class AppConfig
    {

        //public const String _VER = "U2Core221118";
        //[BinaryConvert.Seializable(false)]
        //public readonly String _Version = _VER;

        [BinaryConvert.Seializable(false)] public Boolean BOARD_ATTACHED { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_MATLAB { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_WEBCORE { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_LXI { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_USB { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_STYLE { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_RF { get; set; } = false;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_AWG1 { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_AWG2 { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_VSA { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_VERSION { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_DEBUG { get; set; } = false;

        //用于控制校准数据反序列化失败或读取失败后是否重写Flash
        [BinaryConvert.Seializable(false)] public Boolean ENABLE_CALIBDATA_REWRITE_AT_LOADFAIL { get; set; } = false;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_BANDWIDTH { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_AWG_PROTECT { get; set; } = true;

        [BinaryConvert.Seializable(false)] public string PRODUCT { get; set; } = "JiHe_MSO7000X";
        [BinaryConvert.Seializable(false)] public Boolean AFC_FREQ_RESPONSE_CALI { get; set; } = true;
        [BinaryConvert.Seializable(false)] public Boolean PFC_FREQ_RESPONSE_CALI { get; set; } = true;

        [BinaryConvert.Seializable(false)]
        public String ANA_CHNL_TYPE { get; set; } = AnaChnlType.ANA_2D5G.ToString();


        [BinaryConvert.Seializable(false)] public Int32 TRIGGER_DEFAULT_SENSITIVITY_MDIV { get; set; } = 400;


        /// <summary>
        /// 多语言ID具体见枚举：ScopeX.U2.LanguageSupoort.Language. 0=简体中文，1 =英文
        /// </summary>
        public Int32? LANGUAGEID { get; set; } = 0;


        [BinaryConvert.Seializable(false)]
        public Int32 ACQ_BOARD_NUM { get; set; } = 1;

        [BinaryConvert.Seializable(false)]
        public Int32 ADC_BITS { get; set; } = 8;
        [BinaryConvert.Seializable(false)] public Double SAMPLING_RATE { get; set; } = 10E9;


        [BinaryConvert.Seializable(false)]
        public Int32 ADC_CORE_NUM { get; set; } = 4;

        [BinaryConvert.Seializable(false)]
        public Int32 CHNL_DATA_NUM { get; set; } = 100000;
        [BinaryConvert.Seializable(false)] public Int32 ADC_NUM { get; set; } = 1;



        [BinaryConvert.Seializable(false)] public Int32 VIS_ADC_RES { get; set; } = 240;



        [BinaryConvert.Seializable(false)] public Int32 MAX_YDIVS_NUM { get; set; } = 11;

        [BinaryConvert.Seializable(false)] public Int32 VIS_YDIVS_NUM { get; set; } = 8;

        [BinaryConvert.Seializable(false)] public Int32 IDX_PER_YDIV { get; set; } = 1000;



        [BinaryConvert.Seializable(false)] public Int32 VIS_XDIVS_NUM { get; set; } = 10;

        [BinaryConvert.Seializable(false)] public Int32 IDX_PER_XDIV { get; set; } = 1000;

        [BinaryConvert.Seializable(false)] public Double MIN_XPOS_TIME { get; set; } = 5000;



        [BinaryConvert.Seializable(false)] public Boolean LOG_ENABLED { get; set; } = true;


        /// <summary>
        /// Max Analogy Id = C4
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxAChId { get; set; } = "C4";


        /// <summary>
        /// Max Digital Id = D15
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxDChId { get; set; } = "D15";


        /// <summary>
        /// Max Math Id = M8
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxMChId { get; set; } = "M8";


        /// <summary>
        /// Max Reference Id = R4
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxRChId { get; set; } = "R4";


        /// <summary>
        /// Max Bus Id = B2
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxBChId { get; set; } = "B2";


        /// <summary>
        /// Max Parameter Id = P8
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxPChId { get; set; } = "P8";


        /// <summary>
        /// Max Awg Id = AWG2
        /// </summary>
        [BinaryConvert.Seializable(false)]
        public string MaxAwgId { get; set; } = "AWG2";


        [BinaryConvert.Seializable(false)] public string MaxRFChId { get; set; } = "RF1";



        [BinaryConvert.Seializable(false)] public Int32 MaxPAMNum { get; set; } = 8;

        [BinaryConvert.Seializable(false)] public Int32 MaxJMNum { get; set; } = 8;

        [BinaryConvert.Seializable(false)] public Int32 MaxVSAMNum { get; set; } = 8;
        [BinaryConvert.Seializable(false)] public Int32 MaxIChartMNum { get; set; } = 8;

        [BinaryConvert.Seializable(false)] public Int32 MaxCEMNum { get; set; } = 4;

        [BinaryConvert.Seializable(false)] public Int32 MaxIRMNum { get; set; } = 4;

        [BinaryConvert.Seializable(false)] public Int32 MaxMDNum { get; set; } = 8;

        [BinaryConvert.Seializable(false)] public Double C1PosIndex { get; set; } = 3000;

        [BinaryConvert.Seializable(false)] public Double C2PosIndex { get; set; } = 1000;

        [BinaryConvert.Seializable(false)] public Double C3PosIndex { get; set; } = -1000;

        [BinaryConvert.Seializable(false)] public Double C4PosIndex { get; set; } = -3000;



        [BinaryConvert.Seializable(false)] public String C1ScaleMaxIndex { get; set; } = "Lv10";

        [BinaryConvert.Seializable(false)] public String C2ScaleMaxIndex { get; set; } = "Lv10";

        [BinaryConvert.Seializable(false)] public String C3ScaleMaxIndex { get; set; } = "Lv10";

        [BinaryConvert.Seializable(false)] public String C4ScaleMaxIndex { get; set; } = "Lv10";



        [BinaryConvert.Seializable(false)] public String C1ScaleMinIndex { get; set; } = "Lv1m";

        [BinaryConvert.Seializable(false)] public String C2ScaleMinIndex { get; set; } = "Lv1m";

        [BinaryConvert.Seializable(false)] public String C3ScaleMinIndex { get; set; } = "Lv1m";

        [BinaryConvert.Seializable(false)] public String C4ScaleMinIndex { get; set; } = "Lv1m";



        [BinaryConvert.Seializable(false)] public String TimebaseMaxIndex { get; set; } = "Lv1k";

        [BinaryConvert.Seializable(false)] public String TimebaseMinIndex { get; set; } = "Lv100p";

        [BinaryConvert.Seializable(false)] public String TimebaseMinScanIndex { get; set; } = "Lv50m";

        [BinaryConvert.Seializable(false)] public String TimebaseMaxItplIndex { get; set; } = "Lv50n";



        [BinaryConvert.Seializable(false)] public Double AWG1SI { get; set; } = 4E-6;

        [BinaryConvert.Seializable(false)] public Double AWG2SI { get; set; } = 4E-6;

        [BinaryConvert.Seializable(false)] public Double AWG3SI { get; set; } = 4E-6;

        [BinaryConvert.Seializable(false)] public Double AWG4SI { get; set; } = 4E-6;



        [BinaryConvert.Seializable(false)] public Int32 AWG1Len { get; set; } = 10000;

        [BinaryConvert.Seializable(false)] public Int32 AWG2Len { get; set; } = 10000;

        [BinaryConvert.Seializable(false)] public Int32 AWG3Len { get; set; } = 10000;

        [BinaryConvert.Seializable(false)] public Int32 AWG4Len { get; set; } = 10000;



        [BinaryConvert.Seializable(false)] public Boolean AdaptiveLength { get; set; } = true;

        [BinaryConvert.Seializable(false)] public Int32 MAX_FIGURE_NUM { get; set; } = 16;

        [BinaryConvert.Seializable(false)] public string RenderingMode { get; set; } = "GPU";

        [BinaryConvert.Seializable(false)] public Int32 LOG_SAVE_DAYS { get; set; } = 7;

        [BinaryConvert.Seializable(false)] public Int32 COMPORTNUM_KEYBOARD { get; set; } = 4;

        [BinaryConvert.Seializable(false)] public Int32 COMPORTNUM_ANALGCHANNEL1 { get; set; } = 2;

        [BinaryConvert.Seializable(false)] public Double MEASUREGATE_DIVS { get; set; } = 0.33;

        [BinaryConvert.Seializable(false)] public Double ANALOGCHANNEL_WORKING_TEMPERATURE { get; set; } = 55;
        [BinaryConvert.Seializable(false)] public Double HARDDISK_WORKING_TEMPERATURE { get; set; } = 55;

        [BinaryConvert.Seializable(false)] public Int64 OPTION_LIMIT { get; set; } = 0;

        [BinaryConvert.Seializable(false)] public Boolean TEMPERATURE_MONITOR_ENABLE { get; set; } = false;

        [BinaryConvert.Seializable(false)] public Boolean ANALOG_TEMPERATURE_COMPENSATE { get; set; } = true;

        [BinaryConvert.Seializable(false)]
        public Double MaxGainCaliValue { get; set; } = 0.1D;//最大幅度自校准范围

        [BinaryConvert.Seializable(false)]
        public Double MinGainCaliValue { get; set; } = 0.005D;//最小幅度自校准范围

        [BinaryConvert.Seializable(false)]
        public Int32 AdcCalibrationModel { get; set; } = 1;//开机TiAdc自动校准 0:关闭 1:打开 2：采集板1 3:采集板2

        [BinaryConvert.Seializable(false)]
        public Int32 DspModel { get; set; } = 15;// [3:0] 3bit： ; [0]：0关闭DSP；1打开DSP； [1]：0频域系数 1时域系数 [2]：打开DMA下发DSP系数; [3]：1打开探头dsp 0关闭探头dsp

		[BinaryConvert.Seializable(false)] 
		public Boolean ENABLE_USB_SUPERSPEED { get; set; } = true;
        #region 通道延迟功能配置项

        /// <summary>
        /// 通道一延迟点数
        /// </summary>
        public int ChannelDelay_C1 { get; set; } = 1;


        /// <summary>
        /// 通道二延迟点数
        /// </summary>
        public int ChannelDelay_C2 { get; set; } = 1;


        /// <summary>
        /// 通道三延迟点数
        /// </summary>
        public int ChannelDelay_C3 { get; set; }


        /// <summary>
        /// 通道四延迟点数
        /// </summary>
        public int ChannelDelay_C4 { get; set; }

        #endregion 通道延迟功能配置项

        #region 通道号和ADC 读取的通道配置关系映射

        //#if UPO7000L
        //        /// <summary>
        //        /// 通道和ADC关系映射，
        //        /// 7000L需要默认映射为： 3,2,1,0 
        //        /// </summary>
        //        [BinaryConvert.Seializable(false)]
        //        public string ChannalMap { get; set; } = "3,2,1,0";
        //#else
        /// <summary>
        /// 通道和ADC关系映射
        /// 7000X的正确关系0,1,2,3
        /// </summary>
        //[BinaryConvert.Seializable(false)]
        private string channalmap { get; set; } = "0,1,2,3";
        //#endif


        /// <summary>
        /// 获取ADC的Core和通道号的映射关系
        /// </summary>
        /// <returns></returns>
        public List<int> GetAdc_Channal_Map()
        {
            if (string.IsNullOrEmpty(channalmap))
                return new List<int> { 0, 1, 2, 3 };

            List<int> result = new List<int>();
            var ids = channalmap.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int val))
                    result.Add(val);
            }

            return result;
        }

        #endregion

        #region Component service

        [BinaryConvert.Seializable(false)] public Boolean ENABLE_LA { get; set; } = true;

        /// <summary>
        /// 用于控制示波器软件侧 能否进行探头得基础校准
        /// </summary>
        [BinaryConvert.Seializable(false)] public Boolean PROBE_FACT_CALIB { get; set; } = false;

        [Description("AWG")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_AWG { get; set; } = true;


        [Description("BUS")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_BUS { get; set; } = true;


        [Description("SDA")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_SDA { get; set; } = true;


        [Description("Ref")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Ref { get; set; } = true;


        [Description("Math")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Math { get; set; } = true;


        [Description("Segement")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Segement { get; set; } = true;


        [Description("Search")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Search { get; set; } = true;


        [Description("Measure")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Measure { get; set; } = true;


        [Description("PassFail")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_PassFail { get; set; } = true;


        [Description("PowerAs")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_PowerAs { get; set; } = true;


        [Description("Lissajus")]
        [BinaryConvert.Seializable(false)]
        public Boolean ENABLE_Lissajous { get; set; } = true;




        #endregion Component service

        //public List<(Boolean Is2GHz, String Model)> ProductModel = new List<(bool Is2GHz, string Model)>();

        private static AppConfig instance = null;
        private static object objlock = new object();
        public static AppConfig GetIntance(string configPath = "")
        {
            lock (objlock)
            {
                if (instance is null)
                {
                    instance = LoadConfig(configPath);
                    switch (instance.PRODUCT)
                    {
                        case "JiHe_MSO7000X":
                            instance.channalmap = "0,1,2,3";
                            Config7000X(instance);
                            break;
                        case "JiHe_UPO7000L":
                            instance.channalmap = "3,2,1,0";
                            Config7000L(instance);
                            break;
                        default:
                            instance.channalmap = "0,1,2,3";
                            Config7000X(instance);
                            break;
                    }
                }
                return instance;
            }
        }

        private static void Config7000L(AppConfig app)
        {
            app.ENABLE_LA = false;
            app.ENABLE_AWG2 = false;
            app.MaxAwgId = "AWG1";
            app.MaxBChId = "B1";
            //app.ProductModel = new List<(bool Is2GHz, string Model)>
            //{
            //    (true, "UPO7204L"),
            //    (false, "UPO7104L")
            //};
        }

        private static void Config7000X(AppConfig app)
        {
            app.MaxAwgId = "AWG2";
            app.MaxBChId = "B2";
            app.ENABLE_AWG = true;
            app.ENABLE_AWG2 = true;
            app.ENABLE_BUS = true;
            app.ENABLE_LA = true;
            app.ENABLE_Lissajous = true;
            app.ENABLE_PassFail = true;
            app.ENABLE_PowerAs = true;
            app.ENABLE_Ref = true;
            app.ENABLE_Measure = true;
            app.ENABLE_SDA = true;
            app.ENABLE_Math = true;
            app.ENABLE_Search = true;
            app.ENABLE_Segement = true;

            //app.ProductModel = new List<(bool Is2GHz, string Model)>
            //{
            //    (true, "MSO7204X"),
            //    (false, "MSO7104X")
            //};
        }

        public static void UpdateInstance()
        {
            lock (objlock)
            {

                instance = LoadConfig();
                switch (instance.PRODUCT)
                {
                    case "JiHe_MSO7000X":
                        instance.channalmap = "0,1,2,3";
                        Config7000X(instance);
                        break;
                    case "JiHe_UPO7000L":
                        instance.channalmap = "3,2,1,0";
                        Config7000L(instance);
                        break;
                    default:
                        instance.channalmap = "0,1,2,3";
                        Config7000X(instance);
                        break;
                }
            }
        }

        private static AppConfig LoadConfig(string configPath = "")
        {
            string filename = "Config.set";
            if (!string.IsNullOrWhiteSpace(configPath) && Directory.Exists(configPath))
            {
                filename = @$"{configPath}\Config.set";
            }
            try
            {
                if (!File.Exists(filename))
                {
                    var appconfig = new AppConfig();
                    appconfig.SaveConfig();
                    return appconfig;
                }

                using MemoryStream memorystream = new(File.ReadAllBytes(filename));
                var config = BinaryConvert.Deserialize<AppConfig>(memorystream);
                return config ?? new AppConfig();
            }
            catch (Exception e)
            {
                var appconfig = new AppConfig();
                appconfig.SaveConfig();
                return appconfig;
                //  EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
        }

        public void SaveConfig()
        {
            string filename = "" +
                "Config.set";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using var fs = new FileStream("Config.set", FileMode.Create, FileAccess.Write);
            BinaryConvert.Serialize(this, fs);
        }
    }
}
    