using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class AnalogChannelParams : ICaliData
    {
        private AnalogChannelParams()
        {
            _VersionParams = new(AnalogGodVersionEnum.Test, _GodParamDefineTable,
                                 AnalogItemVersionEnum.Test, _ItemParamDefineTable);
        }

        public static AnalogChannelParams Default = new();

        private VersionParams<AnalogGodVersionEnum, AnalogItemVersionEnum> _VersionParams;

        private Dictionary<AnalogGodVersionEnum, Object> _GodParamDefineTable = new()
        {
            [AnalogGodVersionEnum.Test] = new AnalogGodParams_Test(),
            [AnalogGodVersionEnum.Base] = new AnalogGodParams_Base(),
        };

        private Dictionary<AnalogItemVersionEnum, DictionaryParams> _ItemParamDefineTable = new()
        {
            [AnalogItemVersionEnum.Test] = new(CaliConstants.KeyStrLen, typeof(AnalogChannelItem_Test), 7),
            [AnalogItemVersionEnum.Base] = new(CaliConstants.KeyStrLen, typeof(AnalogChannelItem_Base), ProductDataTranslate_MSO8000X.GetAllChnlParamsKeys().Count),
        };

        public AnalogGodVersionEnum GodVersion
        {
            get => _VersionParams.GodVersion;
            set => _VersionParams.GodVersion = value;
        }
        public AnalogItemVersionEnum ItemVersion
        {
            get => _VersionParams.ItemVersion;
            set => _VersionParams.ItemVersion = value;
        }

        public String CalcTimeStr
        {
            get => _VersionParams.CalcTimeStr;
            set => _VersionParams.CalcTimeStr = value;
        }

        public Type GodType => _VersionParams.GodType ?? typeof(AnalogGodParams_Test);

        public Type ItemType => _VersionParams.ItemType ?? typeof(AnalogChannelItem_Test);

        public Object GodParams
        {
            get => _VersionParams.GodParams ?? new AnalogGodParams_Test();
            set => _VersionParams.GodParams = value;
        }

        public Object this[String paramName]
        {
            get => _VersionParams[paramName] ?? new AnalogChannelItem_Test();
            set => _VersionParams[paramName] = value;
        }

        public String[] AllNames => _VersionParams.AllItemNames;

        public CaliDataType DataType => CaliDataType.AnalogParams;

        public Int32 TotalBytes => _VersionParams.TotalBytes;

        public Int32 OriginTotleBytes
        {
            get;
            set;
        }
        internal string CaliDataPath
        {
            get => AppDomain.CurrentDomain.BaseDirectory + @"CaliData\";
        }
        /// <summary>
        /// Tool端调用接口
        /// </summary>
        public void SaveToFile()
        {
            string fileName = CaliDataPath + DataType.ToString() + ".bin";
            if (!Directory.Exists(CaliDataPath))
                Directory.CreateDirectory(CaliDataPath);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, Serialize());

            AutoCaliParams.Default?.SaveToFile();
        }

        public void Deserialize(Byte[] content) => _VersionParams.Deserialize(content);

        public Byte[] Serialize() => _VersionParams.Serialize();

        public void UpdateGodVersion(AnalogGodVersionEnum destinationVersion)
        {
            MapInfo<AnalogGodVersionEnum> mapinfo = new(_VersionParams.GodVersion, destinationVersion);
            var namemaptable = _GodParamsMapDefine.ContainsKey(mapinfo) ? _GodParamsMapDefine[mapinfo] : null;

            _VersionParams.CopyGodParams(mapinfo, namemaptable);
            _VersionParams.GodVersion = destinationVersion;
        }

        public void UpdateItemVersion(AnalogItemVersionEnum destinationVersion)
        {
            MapInfo<AnalogItemVersionEnum> mapinfo = new(_VersionParams.ItemVersion, destinationVersion);
            var namemaptable = _ItemParamsMapDefine.ContainsKey(mapinfo) ? _ItemParamsMapDefine[mapinfo] : null;

            _VersionParams.CopyItemParams(mapinfo, namemaptable);
            _VersionParams.ItemVersion = destinationVersion;
        }

        /// <summary>
        /// GodParams的结构体名字映射表
        /// </summary>
        private Dictionary<MapInfo<AnalogGodVersionEnum>, Dictionary<String, String>> _GodParamsMapDefine = new()
        {
            [new(AnalogGodVersionEnum.Test, AnalogGodVersionEnum.Base)] = new()
            {
                [nameof(AnalogGodParams_Test.Test0)] = nameof(AnalogGodParams_Base.Reserved0),
                [nameof(AnalogGodParams_Test.Test1)] = nameof(AnalogGodParams_Base.Reserved1),
            },
        };

        /// <summary>
        /// ItemParams的结构体名字映射表
        /// </summary>
        private Dictionary<MapInfo<AnalogItemVersionEnum>, Dictionary<String, String>> _ItemParamsMapDefine = new()
        {
            [new(AnalogItemVersionEnum.Test, AnalogItemVersionEnum.Base)] = new()
            {
                [nameof(AnalogChannelItem_Test.Test0)] = nameof(AnalogChannelItem_Base.Reserved0),
                [nameof(AnalogChannelItem_Test.Test1)] = nameof(AnalogChannelItem_Base.Reserved1),
                [nameof(AnalogChannelItem_Test.Test2)] = nameof(AnalogChannelItem_Base.Reserved2),
                [nameof(AnalogChannelItem_Test.Test3)] = nameof(AnalogChannelItem_Base.Reserved3),
            },
        };
    }

    public enum AnalogGodVersionEnum
    {
        Test,
        Base,
    }

    public enum AnalogItemVersionEnum
    {
        Test,
        Base,
    }

    #region GodParams Struct Define
    public struct AnalogGodParams_Test
    {
        //AnalogGodParams_Base Base;
        public Int32 Test0 { get; set; }
        public Int32 Test1 { get; set; }
    }

    public struct AnalogGodParams_Base
    {
        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
    }
    #endregion

    #region Item Struct Define
    public struct AnalogChannelItem_Test
    {
        public Int32 Test0 { get; set; }
        public Int32 Test1 { get; set; }
        public Int32 Test2 { get; set; }
        public Int32 Test3 { get; set; }
        public Int32 Test4 { get; set; }
        public Int32 Test5 { get; set; }
        public Int32 Test6 { get; set; }
        public Int32 Test7 { get; set; }
    }

    public struct AnalogChannelItem_Base
    {
        /// <summary>
        /// 偏置DAC零点值
        /// </summary>
        public Int32 Bias { get; set; }

        /// <summary>
        /// 偏置DAC 正斜率值
        /// </summary>
        public Int32 Bias_Pos3Div { get; set; }

        /// <summary>
        /// 偏置DAC 负斜率值
        /// </summary>
        public Int32 Bias_Neg3Div { get; set; }

        /// <summary>
        /// 偏移DAC零点值
        /// </summary>
        public Int32 Offset { get; set; }

        /// <summary>
        /// 偏移DAC 正斜率值
        /// </summary>
        public Int32 Offset_Pos3Div { get; set; }

        /// <summary>
        /// 偏移DAC 负斜率值
        /// </summary>
        public Int32 Offset_Neg3Div { get; set; }

        /// <summary>
        /// 增益粗调（DVGA）值
        /// </summary>
        public Int32 Gain { get; set; }

        /// <summary>
        /// Fpga的数字细调值
        /// </summary>
        public Int32 Gain_FineByFpgaThousand { get; set; }

        /// <summary>
        /// Adc1的细调值
        /// </summary>
        public Int32 Gain_FineByTenThousandByAdc1 { get; set; }
        public Int32 Gain_FineByTenThousandByAdc2 { get; set; }
        public Int32 DCTrigZero { get; set; }
        public Int32 DCTrigZero_3Div { get; set; }
        public Int32 DiscardDotsBefore { get; set; }
        public Int32 DiscardDotsAfter { get; set; }

        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
        public Int32 Reserved2 { get; set; }
        public Int32 Reserved3 { get; set; }
    }
    #endregion
}
