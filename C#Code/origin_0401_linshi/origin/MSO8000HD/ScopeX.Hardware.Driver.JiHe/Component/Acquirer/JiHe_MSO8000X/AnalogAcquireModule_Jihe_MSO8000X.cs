using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.Hardware.Driver.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class AnalogAcquireModule_Jihe_MSO8000X : AbstractAnalogAcquireModel
    {
        internal AnalogAcquireModule_Jihe_MSO8000X() : base()
        {
            DeafultChannelState = _AllChnlActive;

            #region 通道全开
            AcqModeAndInterleaveDefineTable[_AllChnlActive] = new("All-20G", SampleMode.Quad, AdcInterleaveMode.Mode2To1);
            AcqModeAndInterleaveDefineTable[_AllChnlActive].Details[ChannelId.C1] = new AdcUsedInfo[]
            {
                //端口号为0，表示PortA；端口号为1，表示PortB;
                //new AdcUsedInfo(AcqBdNo.B0, 0b10, 0b0001)
                new AdcUsedInfo(AcqBdNo.B0, 0b11, 0b0011)
                {
                    AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }                       
                },
                //new AdcUsedInfo(AcqBdNo.B1, 0b1100, 0b0011)
                //{
                //    AdcPorts = new()
                //    {
                //        {0,1},
                //        {1,1},
                //    }
                //},
            };
            AcqModeAndInterleaveDefineTable[_AllChnlActive].Details[ChannelId.C2] = new AdcUsedInfo[]
            {
                //new AdcUsedInfo(AcqBdNo.B0, 0b01, 0b0001)
                new AdcUsedInfo(AcqBdNo.B1, 0b11, 0b0011)
                { 
                    AdcPorts = new ()
                    {
                        {0,1},
                        {1,1},
                    }
                },
                //new AdcUsedInfo(AcqBdNo.B3, 0b1100, 0b0011)
                //{
                //    AdcPorts = new ()
                //    {
                //         {0,1},
                //        {1,1},
                //    }
                //},
            };
            AcqModeAndInterleaveDefineTable[_AllChnlActive].Details[ChannelId.C3] = new AdcUsedInfo[]
            {
                //new AdcUsedInfo(AcqBdNo.B1, 0b10, 0b0001)
                new AdcUsedInfo(AcqBdNo.B2, 0b11, 0b0011)
                {
                    AdcPorts = new ()
                    {
                        {0,1},
                        {1,1},
                    }
                },
                //new AdcUsedInfo(AcqBdNo.B5, 0b1100, 0b0011)
                //{
                //    AdcPorts = new ()
                //    {
                //       {0,1},
                //        {1,1},
                //    }
                //},
            };
            AcqModeAndInterleaveDefineTable[_AllChnlActive].Details[ChannelId.C4] = new AdcUsedInfo[]
            {
                //new AdcUsedInfo(AcqBdNo.B1, 0b1, 0b0001)
                new AdcUsedInfo(AcqBdNo.B3, 0b11, 0b0011)
                {
                    AdcPorts = new ()
                    {
                         {0,1},
                        {1,1},
                    }
                },
                //new AdcUsedInfo(AcqBdNo.B7, 0b1100, 0b0011)
                //{
                //    AdcPorts = new ()
                //    {
                //        {0,1},
                //        {1,1},
                //    }
                //},
            };
            #endregion

            //#region 双通道模式
            //_AcqModeAndInterleaveDefineTable[_ActiveC1C3] = new("C1C3-20G", SampleMode.Dual, AdcInterleaveMode.Mode2To1);
            //_AcqModeAndInterleaveDefineTable[_ActiveC1C3].Details[ChannelId.C1] = new AdcUsedInfo[]
            //{
            //    new AdcUsedInfo(AcqBdNo.B0, 0b11, 0b0011)
            //    {
            //        AdcPorts = new ()
            //        {
            //            { 0, 1},
            //            { 1, 0}
            //        }
            //    },
            //};
            //_AcqModeAndInterleaveDefineTable[_ActiveC1C3].Details[ChannelId.C3] = new AdcUsedInfo[]
            //{
            //    new AdcUsedInfo(AcqBdNo.B1, 0b11, 0b0011)
            //    {
            //        AdcPorts = new ()
            //        {
            //            { 0, 1},
            //            { 1, 0}
            //        }
            //    },
            //};

            //_AcqModeAndInterleaveDefineTable[_ActiveC1] = new("C1-20G", SampleMode.Dual, AdcInterleaveMode.Mode2To1);
            //_AcqModeAndInterleaveDefineTable[_ActiveC1].Details[ChannelId.C1] = new AdcUsedInfo[]
            //{
            //    new AdcUsedInfo(AcqBdNo.B0, 0b11, 0b0011)
            //    {
            //        AdcPorts = new ()
            //        {
            //            { 0, 1},
            //            { 1, 0}
            //        }
            //    },
            //};
            //_AcqModeAndInterleaveDefineTable[_ActiveC3] = new("C3-20G", SampleMode.Dual, AdcInterleaveMode.Mode2To1);
            //_AcqModeAndInterleaveDefineTable[_ActiveC3].Details[ChannelId.C3] = new AdcUsedInfo[]
            //{
            //    new AdcUsedInfo(AcqBdNo.B1, 0b11, 0b0011)
            //    {
            //        AdcPorts = new ()
            //        {
            //            { 0, 1},
            //            { 1, 0}
            //        }
            //    },
            //};
            //#endregion
        }

        internal override AcqModeAndInterleaveDefine? GetCurrentAcqModeInterleave()
        {
            UInt32 chnlActiveState = GetChnlActiveState();
            return GetAcqModeInterleaveByChnlState(chnlActiveState);
        }

        internal override AcqModeAndInterleaveDefine? GetAcqModeInterleaveByChnlState(UInt32 chnlActiveState)
        {
            //开启LA，通道状态为默认
            var laisopen = Hd.UIMessage?.Digital?.Any(d => d.Active) ?? false;
            if (laisopen)
                chnlActiveState = DeafultChannelState;

            if (AcqModeAndInterleaveDefineTable.Keys.Contains(chnlActiveState))
            {
                return AcqModeAndInterleaveDefineTable[chnlActiveState];
            }

            if (AcqModeAndInterleaveDefineTable.ContainsKey(DeafultChannelState))
            {
                return AcqModeAndInterleaveDefineTable[DeafultChannelState];
            }
//???cij
            return AcqModeAndInterleaveDefineTable[_AllChnlActive];
        }


        internal override Dictionary<ChannelId, Int32> GetUsedCoreCntPerChnl(ChannelId[] activeChnls)
        {
            Dictionary<ChannelId, Int32> ans = new();
            UInt32 chnlActiveState = PublicFunc.ConvertToUniqueHotCode(activeChnls.Select(o => (Int32)o));
            var currentDefine = GetAcqModeInterleaveByChnlState(chnlActiveState);
            foreach (ChannelId chnlid in activeChnls)
            {
                
                ans[chnlid] = currentDefine.InterleaveMode switch
                {
                    AdcInterleaveMode.Mode2To1 => 2,
                    _ => 1,
                };
            }
            return ans;
        }

        internal override UInt32? GetAcqUintIndex(AdcUsedInfo adcInfo,int acqId)
        {
            var adcId = GetUsedAdcs(adcInfo)[acqId%2];
            return (UInt32)((int)adcInfo.AcqBdNo * Constants.ADC_NUM + adcId);
        }

        /// <summary>
        /// 获取交织定义表
        /// </summary>
        internal Dictionary<UInt32, AcqModeAndInterleaveDefine> AcqModeAndInterleaveDefineTable;
        //{
        //    get => AcqModeAndInterleaveDefineTable; 
        //}


        /// <summary>
        /// 获取当前打开通道的状态码
        /// </summary>
        /// <returns></returns>
        private UInt32 GetChnlActiveState()
        {
            UInt32 chnlActiveState = 0;
            HdMessage.AnalogOptions[]? analogs = Hd.UIMessage?.Analog;
            if (analogs == null)
                return _AllChnlActive;

            for (int anaIndex = 0; anaIndex < analogs.Length; anaIndex++)
            {
                if (analogs[anaIndex].Active)
                    chnlActiveState |= (UInt32)(0x1 << anaIndex);
            }
            return chnlActiveState;
        }

        private const uint _ActiveC1 = 0b0001;
        private const uint _ActiveC3 = 0b0100;

        private const uint _ActiveC1C3 = 0b0101;

        private const uint _AllChnlActive = 0b1111;

    }
}
