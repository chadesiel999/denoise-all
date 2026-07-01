using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal class AnalogAcquireModel
    {
        enum ADCSyncType : Byte
        {
            AutoSync = 0,
            ClockStop = 1,
        }
        enum ADCSyncEdge : Byte
        {
            PosEdge = 0,
            NegEdge = 1,
        }

        /// <summary>
        /// 该表的使用:
        /// 1、在计算控制哪个Core的数据归集到哪个通道时可用，也就是发送到Address = 0x01时的高8位的值。每个Core分别从A、B、C、D，分配到哪个物理通道，通道从0开始编号，每个core用2位表示分配到的通道号
        /// 2、在发送Gain、Offset、Phase时使用。地址4、5分别发送BA、DC核的Gain.校准数据是按但个核进行保存的，需要拼接成BA、DC的模式
        /// </summary>
        public static List<AcqModeInterleaveDefine> AcqModeInterleaveDefines = new List<AcqModeInterleaveDefine>()
        {
            //5G
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C1C2_5G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode2To1,
                    Channels=((1 << (int)ChannelId.C1)) | ((1 << (int)ChannelId.C2)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C1 | 0x01 << (int)ChannelId.C2) << 4 | 1)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=0,UsedCoreList=new int[]{0,2} },
                        new AcqModeInterleaveDetail() { FixedCore=1,UsedCoreList=new int[]{1,3} },
                    }
                },
            //new AcqModeInterleaveDefine()
            //    {
            //        InterleaveName="C1C4_5G",
            //        AdcInterleaveMode= AdcInterleaveMode.Mode2To1,
            //        Channels=((1 << (int)ChannelId.C1)) | ((1 << (int)ChannelId.C4)),
            //        ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C1 | 0x01 << (int)ChannelId.C4) << 4 | 1)),
            //        Details=new AcqModeInterleaveDetail[]
            //        {
            //            new AcqModeInterleaveDetail() { FixedCore=0,UsedCoreList=new int[]{0,2} },
            //            new AcqModeInterleaveDetail() { FixedCore=3,UsedCoreList=new int[]{1,3} },
            //        }
            //    },
            //new AcqModeInterleaveDefine()
            //    {
            //        InterleaveName="C2C3_5G",
            //        AdcInterleaveMode= AdcInterleaveMode.Mode2To1,
            //        Channels=((1 << (int)ChannelId.C2)) | ((1 << (int)ChannelId.C3)),
            //        ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C2 | 0x01 << (int)ChannelId.C3) << 4 | 1)),
            //        Details=new AcqModeInterleaveDetail[]
            //        {
            //            new AcqModeInterleaveDetail() { FixedCore=1,UsedCoreList=new int[]{1,3} },
            //            new AcqModeInterleaveDetail() { FixedCore=2,UsedCoreList=new int[]{0,2} },
            //        }
            //    },
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C3C4_5G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode2To1,
                    Channels=((1 << (int)ChannelId.C3)) | ((1 << (int)ChannelId.C4)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C3 | 0x01 << (int)ChannelId.C4) << 4 | 1)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=2,UsedCoreList=new int[]{0,2} },
                        new AcqModeInterleaveDetail() { FixedCore=3,UsedCoreList=new int[]{1,3} },
                    }
                },
            //10G
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C1_10G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode4To1,
                    Channels=((1 << (int)ChannelId.C1)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C1) << 4 | 2)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=0,UsedCoreList=new int[]{0,1,2,3} },
                    }
                },
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C2_10G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode4To1,
                    Channels=((1 << (int)ChannelId.C2)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C2) << 4 | 2)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=1,UsedCoreList=new int[]{0,1,2,3} },
                    }
                },
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C3_10G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode4To1,
                    Channels=((1 << (int)ChannelId.C3)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C3) << 4 | 2)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=2,UsedCoreList=new int[]{0,1,2,3} },
                    }
                },
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="C4_10G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode4To1,
                    Channels=((1 << (int)ChannelId.C4)),
                    ChMode_SamplingMode=((uint)((0x01 << (int)ChannelId.C4) << 4 | 2)),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=3,UsedCoreList=new int[]{0,1,2,3} },
                    }
                },
            //2.5G
            new AcqModeInterleaveDefine()
                {
                    InterleaveName="2.5G",
                    AdcInterleaveMode= AdcInterleaveMode.Mode1To1,
                    Channels=((1 << (int)ChannelId.C1)) | ((1 << (int)ChannelId.C2)) | ((1 << (int)ChannelId.C3)) | ((1 << (int)ChannelId.C4)),
                    ChMode_SamplingMode=(0x0f << 4),
                    Details=new AcqModeInterleaveDetail[]
                    {
                        new AcqModeInterleaveDetail() { FixedCore=0,UsedCoreList=new int[]{0} },
                        new AcqModeInterleaveDetail() { FixedCore=1,UsedCoreList=new int[]{1} },
                        new AcqModeInterleaveDetail() { FixedCore=2,UsedCoreList=new int[]{2} },
                        new AcqModeInterleaveDetail() { FixedCore=3,UsedCoreList=new int[]{3} },
                    }
                },
        };

    }

    //DataStruct refactoring: define data class can map to TiadcPhaseOffsetGainParams string
    [Serializable]
    public class AcqModeInterleaveDefine
    {
        public String InterleaveName { get; set; } = "";
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

    [Serializable]
    public class AcqModeInterleaveDefine_MSO8000x
    {
        public String InterleaveName { get; set; } = "";
        public AdcInterleaveMode AdcInterleaveMode { get; set; }
        [JsonPropertyName("Details")]
        public AcqModeInterleaveDetail_MSO8000x[]? Details { get; set; }

        public AcqModeInterleaveDetail_MSO8000x? GetChnlDetail(ChannelId chnlId)
        {
            return Details!.FirstOrDefault(d => d.ChannelId == chnlId);
        }

        public int GetChnlIndex(ChannelId chnlId)
        {
            return Details.ToList().FindIndex(0, del => del.ChannelId == chnlId);
        }
    }

    [Serializable]
    public class AcqModeInterleaveDetail_MSO8000x
    {
        public ChannelId ChannelId { get; set; }

        public int FixedAcqUnitIndex { get; set; }

        [JsonPropertyName("UsedAcqUnits")]
        public AcqUnitInfo[]? UsedAcqUnits { get; set; }
    }

    [Serializable]
    public class AcqUnitInfo
    {
        public AcqUnitInfo(int boardId, int adcId)
        {
            BoardId = boardId;
            AdcId = adcId;
        }

        public int BoardId { get; set; }
        public int AdcId { get; set; }

        public int UnitId => BoardId * Constants.ADC_NUM + AdcId;
    }

}
