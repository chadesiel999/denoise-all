using Newtonsoft.Json;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Hardware.Driver
{
    internal abstract class AbstractAnalogAcquireModel
    {
        internal AbstractAnalogAcquireModel()
        {
            AcqModeAndInterleaveDefineTable = new();
        }
        internal Int32 SubbandCnt { get; init; } = 1;

        public string GetAcqModeInterleaveDefineString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            return JsonConvert.SerializeObject(AcqModeAndInterleaveDefineTable);
        }
        /// <summary>
        /// Key:所有通道的使能状态，独热码，每个bit表示对应通道的使能状态，0-关闭，1-开启
        /// </summary>
        protected Dictionary<UInt32, AcqModeAndInterleaveDefine> AcqModeAndInterleaveDefineTable { get; init; }

        /// <summary>
        /// 获取当前通道使能状态下对应的采样模式信息
        /// </summary>
        /// <param name="chnlActiveState">通道使能状态，每个通道使用一个bit表示对应的使能状态：0-关闭，1-开启</param>
        /// <returns></returns>
        internal virtual AcqModeAndInterleaveDefine? GetAcqModeInterleaveByChnlState(UInt32 chnlActiveState)
        {
            if (AcqModeAndInterleaveDefineTable.Keys.Contains(chnlActiveState))
            {
                return AcqModeAndInterleaveDefineTable[chnlActiveState];
            }

            if (AcqModeAndInterleaveDefineTable.ContainsKey(DeafultChannelState))
            {
                return AcqModeAndInterleaveDefineTable[DeafultChannelState];
            }

            return null;
        }
      
        /// <summary>
        /// 获取当前的交织模式
        /// </summary>
        /// <returns></returns>
        internal virtual AcqModeAndInterleaveDefine? GetCurrentAcqModeInterleave()
        {
            return null;
        }

        /// <summary>
        /// 获取采集单元序号
        /// </summary>
        /// <param name="adcInfo"></param>
        /// <returns></returns>
        internal virtual UInt32? GetAcqUintIndex(AdcUsedInfo adcInfo,int acqId)
        {
            return null;
        }

        /// <summary>
        /// 获取AdcUsedInfo所使用的Adc集合
        /// </summary>
        /// <param name="adcInfo"></param>
        /// <returns></returns>
        internal virtual List<uint> GetUsedAdcs(AdcUsedInfo adcInfo)
        {
            List<uint> usedAdcs = new List<uint>();
            for (int i = 0; i < sizeof(UInt32); i++)
            {
                bool usedFlag = ((adcInfo.Adc >> i) & 0x1) == 0x1;
                if (usedFlag)
                    usedAdcs.Add((uint)i);
            }
            return usedAdcs;
        }

        //internal virtual IEnumerable<AdcUsedInfo>? GetAdcUsedInfo(UInt32 chnlActiveState, ChannelId chnlId)
        //{
        //    if (AcqModeAndInterleaveDefineTable.ContainsKey(chnlActiveState) && AcqModeAndInterleaveDefineTable[chnlActiveState].Details.ContainsKey(chnlId))
        //    {
        //        return AcqModeAndInterleaveDefineTable[chnlActiveState].Details[chnlId];
        //    }

        //    if (AcqModeAndInterleaveDefineTable.ContainsKey(DeafultChannelState) && AcqModeAndInterleaveDefineTable[DeafultChannelState].Details.ContainsKey(chnlId))
        //    {
        //        return AcqModeAndInterleaveDefineTable[DeafultChannelState].Details[chnlId];
        //    }

        //    return null;
        //}
        internal virtual AdcUsedInfo[]? GetAdcUsedInfo(UInt32 chnlActiveState, ChannelId chnlId)
        {
            AcqModeAndInterleaveDefine? acqdefine = GetAcqModeInterleaveByChnlState(chnlActiveState);
            if (acqdefine != null && acqdefine.Details.ContainsKey(chnlId))
            {
                return acqdefine.Details[chnlId];
            }
            return null;
        }

        internal virtual AdcUsedInfo? GetAdcUsedInfo(UInt32 chnlActiveState, ChannelId chnlId, Int32 usedInfoId)
        {
            AdcUsedInfo[]? adcusedinfos = GetAdcUsedInfo(chnlActiveState, chnlId);
            if (adcusedinfos != null && adcusedinfos.Length > usedInfoId)
            { 
                return adcusedinfos[usedInfoId];
            }

            return null;
        }

        internal virtual UInt32 GetActuallActiveState(UInt32 activeState)
        {
            return activeState;
        }

        /// <summary>
        /// 查询在指定通道使能状态下，是否可以开启DBI拼合模式
        /// </summary>
        /// <param name="activeState"></param>
        /// <returns></returns>
        internal virtual Boolean GetDbiMergeState(UInt32 activeState)
        {
            return false;
        }

        /// <summary>
        /// 查询在指定通道使能状态下，是否处于拼合模式
        /// </summary>
        /// <param name="activeState"></param>
        /// <returns></returns>
        internal virtual Boolean GetMergeState(UInt32 activeState) => false;

        /// <summary>
        /// 默认的通道使能模式，当表中没有对应的通道状态时，返回本模式对应的AcqModeAndInterleaveDefine
        /// </summary>
        internal UInt32 DeafultChannelState
        {
            get;
            init;
        }

        /// <summary>
        /// 指定开启通道情况下，获取对应每个通道使用了多少个ADC的Core
        /// </summary>
        /// <param name="activeChnls"></param>
        /// <returns></returns>
        internal virtual Dictionary<ChannelId, Int32> GetUsedCoreCntPerChnl(ChannelId[] activeChnls)
        {
            Dictionary<ChannelId, Int32> ans = new();
            UInt32 chnlActiveState = PublicFunc.ConvertToUniqueHotCode(activeChnls.Select(o => (Int32)o));
            foreach(ChannelId chnlid in activeChnls)
            {
                AdcUsedInfo[]? adcusedinfos = GetAdcUsedInfo(chnlActiveState, chnlid);
                if (adcusedinfos != null)
                {
                    ans[chnlid] = 0;
                    foreach (AdcUsedInfo adcusedinfo in adcusedinfos)
                    {
                        ans[chnlid] += PublicFunc.GetBinaryBitCnt(adcusedinfo.Core) * PublicFunc.GetBinaryBitCnt(adcusedinfo.Adc);
                    }
                }
            }
            return ans;
        }

        internal List<AcqBdNo> GetUsedAcqBd(UInt32 chnlActiveState, ChannelId chnlId)
        {
            List<AcqBdNo> ans = new();

            var adcusdinfos = GetAdcUsedInfo(chnlActiveState, chnlId);
            if (adcusdinfos != null)
            {
                for (Int32 i = 0; i < adcusdinfos.Length; i++)
                {
                    if (!ans.Contains(adcusdinfos[i].AcqBdNo))
                    {
                        ans.Add(adcusdinfos[i].AcqBdNo);
                    }
                }
            }

            return ans;
        }

        /// <summary>
        /// 根据通道开启状态，获取每个通道分别使用了多少个ADC的Core
        /// </summary>
        /// <param name="activeChnl">开启的通道</param>
        /// <returns></returns>
        internal Int32[] GetUsedCoreCntOfAdc(ChannelId[] activeChnl)
        {
            Int32[] ans = new Int32[activeChnl.Length];
            UInt32 chnlActiveState = PublicFunc.ConvertToUniqueHotCode(activeChnl.Select(o => (Int32)o));

            AcqModeAndInterleaveDefine? acqModeAndInterleaveDefine = GetAcqModeInterleaveByChnlState(chnlActiveState);

            if (acqModeAndInterleaveDefine == null || acqModeAndInterleaveDefine.Details.Count == 0)
                return ans;

            for (Int32 i = 0; i < activeChnl.Length; i++)
            {
                if (acqModeAndInterleaveDefine.Details.ContainsKey(activeChnl[i]))
                {
                    foreach (AdcUsedInfo usedInfo in acqModeAndInterleaveDefine.Details[activeChnl[i]])
                    {
                        ans[i] += PublicFunc.GetBinaryBitCnt(usedInfo.Core) * PublicFunc.GetBinaryBitCnt(usedInfo.Adc);
                    }
                }
            }

            return ans;
        }
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
