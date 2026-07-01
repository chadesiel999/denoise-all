using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal partial class ServerSpecailData
    {
        static public List<AcqModeInterleaveDefine>? JiHe_MSO7000X_AcqModeInterleaveDefines = new List<AcqModeInterleaveDefine>();

        static public List<AcqModeInterleaveDefine>? JiHe_MSO8000X_AcqModeInterleaveDefines = new List<AcqModeInterleaveDefine>();

        static public Dictionary<UInt32, AcqModeAndInterleaveDefine>? JiHe_AcqModeInterleaveDefines = new Dictionary<UInt32, AcqModeAndInterleaveDefine>();

        private static void LoadServerSpecailData_AcqModeInterleaveDefine(IInstrumentSession currInstrument)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "AcqModeInterleaveDefine";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string recvStr = currInstrument.ReadString();
            if (recvStr != "")
            {
                if (JiHe_AcqModeInterleaveDefines.Count > 0)
                {
                    JiHe_AcqModeInterleaveDefines.Clear();
                }
                var obj = JsonConvert.DeserializeObject(recvStr);
                foreach (var data in (obj as JObject))
                {
                    JiHe_AcqModeInterleaveDefines.Add(uint.Parse(data.Key), System.Text.Json.JsonSerializer.Deserialize<AcqModeAndInterleaveDefine>(data.Value.ToString()));
                }
            }
        }
    }
    [Serializable]
    public class AcqModeInterleaveDefine
    {
        public String InterleaveName { get; set; }
        public AdcInterleaveMode AdcInterleaveMode { get; set; }
        public int Channels { get; set; }
        public UInt32 ChMode_SamplingMode { get; set; }
        [JsonPropertyName("Details")]
        public AcqModeInterleaveDetail[]? Details { get; set; }
    }
    [Serializable]
    public class AcqModeInterleaveDetail
    {
        public UInt32 FixedCore { get; set; }
        [JsonPropertyName("UsedCoreList")]
        public int[]? UsedCoreList { get; set; }
    }

    /// <summary>
    /// 一块采集板采集的通道数
    /// </summary>
    internal enum SampleMode
    {
        /// <summary>
        /// 单通道
        /// </summary>
        Single,

        /// <summary>
        /// 双通道
        /// </summary>
        Dual,

        /// <summary>
        /// 四通道
        /// </summary>
        Quad,

        /// <summary>
        /// 八通道
        /// </summary>
        Eight
    }

    internal enum AcqBdNo
    {
        [Description("BoardSync_Io_Delay_Value_Acq1,BoardSync_Pattern_Shift_Value_Acq1")]
        B1 = 0,
        [Description("BoardSync_Io_Delay_Value_Acq2,BoardSync_Pattern_Shift_Value_Acq2")]
        B2 = 1,
        [Description("BoardSync_Io_Delay_Value_Acq3,BoardSync_Pattern_Shift_Value_Acq3")]
        B3 = 2,
        [Description("BoardSync_Io_Delay_Value_Acq4,BoardSync_Pattern_Shift_Value_Acq4")]
        B4 = 3,
        [Description("BoardSync_Io_Delay_Value_Acq5,BoardSync_Pattern_Shift_Value_Acq5")]
        B5 = 4,
        [Description("BoardSync_Io_Delay_Value_Acq6,BoardSync_Pattern_Shift_Value_Acq6")]
        B6 = 5,
        [Description("BoardSync_Io_Delay_Value_Acq7,BoardSync_Pattern_Shift_Value_Acq7")]
        B7 = 6,
        [Description("BoardSync_Io_Delay_Value_Acq8,BoardSync_Pattern_Shift_Value_Acq8")]
        B8 = 7,
        [Description("BoardSync_Io_Delay_Value_Acq9,BoardSync_Pattern_Shift_Value_Acq9")]
        B9 = 8,
        [Description("BoardSync_Io_Delay_Value_Acq10,BoardSync_Pattern_Shift_Value_Acq10")]
        B10 = 9,
        [Description("BoardSync_Io_Delay_Value_Acq11,BoardSync_Pattern_Shift_Value_Acq11")]
        B11 = 10,
        [Description("BoardSync_Io_Delay_Value_Acq12,BoardSync_Pattern_Shift_Value_Acq12")]
        B12 = 11,
    }

    /// <summary>
    /// 当前通道使用的采集板和ADC信息
    /// </summary>
    /// <param name="AcqBdNo">使用的采集板的编号</param>
    /// <param name="Adc">使用ADC的独热码：bit0-adc0，bit1-adc1，……，以此类推</param>
    /// <param name="Core">使用ADC的Core的独热码：bit0-core0，bit1-core1，……，以此类推</param>
    internal record AdcUsedInfo(AcqBdNo AcqBdNo, UInt32 Adc, UInt32 Core)
    {
        /// <summary>
        /// key:表示AdcIndex,value:表示打开的通道号（0-APort,1-BPort）
        /// </summary>
        public Dictionary<int, int> AdcPorts { get; init; } = new();
    }

    internal record AcqModeAndInterleaveDefine(String Name, SampleMode SampleMode, AdcInterleaveMode InterleaveMode)
    {
        public Dictionary<ChannelId, AdcUsedInfo[]> Details { get; init; } = new();
    }
}
