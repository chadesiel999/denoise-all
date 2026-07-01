using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class TiadcPhaseOffsetGainParams : ICaliData
    {
        private TiadcPhaseOffsetGainParams()
        {
            _VersionParams = new(TiadcGodVersionEnum.Test, _GodParamDefineTable,
                                 TiadcItemVersionEnum.Test, _ItemParamDefineTable);
        }

        public static TiadcPhaseOffsetGainParams Default = new();

        private VersionParams<TiadcGodVersionEnum, TiadcItemVersionEnum> _VersionParams;

        private Dictionary<TiadcGodVersionEnum, Object> _GodParamDefineTable = new()
        {
            [TiadcGodVersionEnum.Test] = new TiadcGodParams_Test(),
            [TiadcGodVersionEnum.Base] = new TiadcGodParams_Base(),
        };

        private Dictionary<TiadcItemVersionEnum, DictionaryParams> _ItemParamDefineTable = new()
        {
            [TiadcItemVersionEnum.Test] = new DictionaryParams(CaliConstants.KeyStrLen, typeof(TiadcPhaseOffsetGainItem_Test), 7),
            [TiadcItemVersionEnum.Base] = new DictionaryParams(CaliConstants.KeyStrLen, typeof(TiadcPhaseOffsetGainItem_Base),16),
        };

        public TiadcGodVersionEnum GodVersion
        {
            get => _VersionParams.GodVersion;
            set => _VersionParams.GodVersion = value;
        }

        public TiadcItemVersionEnum ItemVersion
        {
            get => _VersionParams.ItemVersion;
            set => _VersionParams.ItemVersion = value;
        }

        public String CalcTimeStr
        {
            get => _VersionParams.CalcTimeStr;
            set => _VersionParams.CalcTimeStr = value;
        }

        public Type TiadcGodType => _VersionParams.GodType ?? typeof(TiadcGodParams_Test);

        public Type TiadcItemType => _VersionParams.ItemType ?? typeof(TiadcPhaseOffsetGainItem_Test);

        public Object GodParams
        {
            get => _VersionParams.GodParams ?? new TiadcGodParams_Test();
            set => _VersionParams.GodParams = value;
        }

        public Object this[String paramName]
        {
            get => _VersionParams[paramName] ?? new TiadcPhaseOffsetGainItem_Test();
            set => _VersionParams[paramName] = value;
        }

        public String[] AllNames => _VersionParams.AllItemNames;

        public CaliDataType DataType => CaliDataType.TiadcPhaseOffsetGainParams;

        public int TotalBytes => _VersionParams.TotalBytes;

        public Int32 OriginTotleBytes { get; set; }

        public void Deserialize(Byte[] content) => _VersionParams.Deserialize(content);

        public Byte[] Serialize() => _VersionParams.Serialize();

        public void UpdateGodVersion(TiadcGodVersionEnum destinationVersion)
        {
            MapInfo<TiadcGodVersionEnum> mapinfo = new(_VersionParams.GodVersion, destinationVersion);
            var namemaptable = _GodParamsMapDefine.ContainsKey(mapinfo) ? _GodParamsMapDefine[mapinfo] : null;

            _VersionParams.CopyGodParams(mapinfo, namemaptable);
            _VersionParams.GodVersion = destinationVersion;
        }

        public void UpdateItemVersion(TiadcItemVersionEnum destinationVersion)
        {
            MapInfo<TiadcItemVersionEnum> mapinfo = new(_VersionParams.ItemVersion, destinationVersion);
            var namemaptable = _ItemParamsMapDefine.ContainsKey(mapinfo) ? _ItemParamsMapDefine[mapinfo] : null;

            _VersionParams.CopyItemParams(mapinfo, namemaptable);
            _VersionParams.ItemVersion = destinationVersion;
        }

        /// <summary>
        /// GodParams的结构体名字映射表
        /// </summary>
        private Dictionary<MapInfo<TiadcGodVersionEnum>, Dictionary<String, String>> _GodParamsMapDefine = new()
        {
            [new(TiadcGodVersionEnum.Test, TiadcGodVersionEnum.Base)] = new()
            {
                [nameof(TiadcGodParams_Test.Test0)] = nameof(TiadcGodParams_Base.Reserved0),
                [nameof(TiadcGodParams_Test.Test1)] = nameof(TiadcGodParams_Base.Reserved1),
                [nameof(TiadcGodParams_Test.Test2)] = nameof(TiadcGodParams_Base.Reserved2),
                [nameof(TiadcGodParams_Test.Test3)] = nameof(TiadcGodParams_Base.Reserved3),
                [nameof(TiadcGodParams_Test.Test4)] = nameof(TiadcGodParams_Base.Reserved4),
                [nameof(TiadcGodParams_Test.Test5)] = nameof(TiadcGodParams_Base.Reserved5),
                [nameof(TiadcGodParams_Test.Test6)] = nameof(TiadcGodParams_Base.Reserved6),
                [nameof(TiadcGodParams_Test.Test7)] = nameof(TiadcGodParams_Base.Reserved7),
            },
        };

        /// <summary>
        /// ItemParams的结构体名字映射表
        /// </summary>
        private Dictionary<MapInfo<TiadcItemVersionEnum>, Dictionary<String, String>> _ItemParamsMapDefine = new()
        {
            [new(TiadcItemVersionEnum.Test, TiadcItemVersionEnum.Base)] = new()
            {
                [nameof(TiadcPhaseOffsetGainItem_Test.Reserved0)] = nameof(TiadcPhaseOffsetGainItem_Base.Gain),
                [nameof(TiadcPhaseOffsetGainItem_Test.Reserved1)] = nameof(TiadcPhaseOffsetGainItem_Base.Offset),
                [nameof(TiadcPhaseOffsetGainItem_Test.Reserved2)] = nameof(TiadcPhaseOffsetGainItem_Base.Phase),
            },
        };
    }

    public enum TiadcItemVersionEnum
    {
        Test,
        Base,
    }

    public enum TiadcGodVersionEnum
    {
        Test,
        Base,
    }

    #region GodParams Struct Define
    public struct TiadcGodParams_Test
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

    public struct TiadcGodParams_Base
    {
        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
        public Int32 Reserved2 { get; set; }
        public Int32 Reserved3 { get; set; }
        public Int32 Reserved4 { get; set; }
        public Int32 Reserved5 { get; set; }
        public Int32 Reserved6 { get; set; }
        public Int32 Reserved7 { get; set; }
    }
    #endregion

    #region Item Struct Define
    public struct TiadcPhaseOffsetGainItem_Test
    {
        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
        public Int32 Reserved2 { get; set; }
        public Int32 Reserved3 { get; set; }
        public Int32 Reserved4 { get; set; }
        public Int32 Reserved5 { get; set; }
    }

    public struct TiadcPhaseOffsetGainItem_Base
    {
        private Int32 _Gain;
        /// <summary>
        /// Gain寄存器配置值
        /// </summary>
        public Int32 Gain
        {
            get => _Gain;
            set => _Gain = GetValidData(value);
        }

        private Int32 _Offset;
        public Int32 Offset
        {
            get => _Offset;
            set => _Offset = GetValidData(value);
        }

        private Int32 _Phase;
        public Int32 Phase
        {
            get => _Phase;
            set => _Phase = GetValidData(value);
        }

        private Int32 _Gain_FPGA;
        public Int32 Gain_FPGA
        {
            get => _Gain_FPGA;
            set => _Gain_FPGA = GetValidFpgaData(value);
        }

        private Int32 _Offset_FPGA;
        public Int32 Offset_FPGA
        {
            get => _Offset_FPGA;
            set => _Offset_FPGA = GetValidFpgaData(value);
        }

        private Int32 _Phase_FPGA;
        /// <summary>
        /// Fpga调整Phase配置值
        /// </summary>
        public Int32 Phase_FPGA
        {
            get => _Phase_FPGA;
            set => _Phase_FPGA = GetValidFpgaData(value);
        }

        private Int32 _AdcDelay_FPGA;
        /// <summary>
        /// 相关采集单元Fpga丢点数
        /// </summary>
        public Int32 AdcDelay_FPGA
        {
            get => _AdcDelay_FPGA;
            set => _AdcDelay_FPGA = GetValidFpgaData(value);
        }

        private Int32 _Offset_FPGA_10mv;
        public Int32 Offset_FPGA_10mv
        {
            get => _Offset_FPGA_10mv;
            set => _Offset_FPGA_10mv = GetValidFpgaData(value);
        }

        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
        public Int32 Reserved2 { get; set; }
        public Int32 Reserved3 { get; set; }
        public Int32 Reserved4 { get; set; }
        public Int32 Reserved5 { get; set; }

        private Int32 GetValidData(Int32 data)
        {
            return data < 0 ? 0 : data;
        }

        private Int32 GetValidFpgaData(Int32 data)
        {
            return data;
        }
    }
    #endregion
}
