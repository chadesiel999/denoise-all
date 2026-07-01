using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 6块采集板（3块大板）的13G
    /// </summary>
    internal class AnalogAcquireModel_DBI13G_AcqBd6 : AbstractAnalogAcquireModel
    {
        internal AnalogAcquireModel_DBI13G_AcqBd6() : base()
        {
            DeafultChannelState = DbiDisableActive;

            #region DBI模式
            foreach (UInt32 activestate in _DbiMergeDefine.Keys)
            {
                AcqModeAndInterleaveDefineTable[activestate] = new(activestate.ToString("X2"), SampleMode.Single, AdcInterleaveMode.Mode4To1);
                foreach (ChannelId chnlid in _DbiMergeDefine[activestate])
                {
                    if (_AdcUsedInfoDefine.ContainsKey(chnlid) && _AdcUsedInfoDefine[chnlid].ContainsKey(DbiMergeMode.Enable))
                    {
                        AcqModeAndInterleaveDefineTable[activestate].Details[chnlid] = _AdcUsedInfoDefine[chnlid][DbiMergeMode.Enable].Values.ToArray();
                    }
                }
            }
            #endregion

            #region 非DBI模式
            AcqModeAndInterleaveDefineTable[DbiDisableActive] = new("DBI_Disable", SampleMode.Single, AdcInterleaveMode.Mode4To1);
            foreach (ChannelId chnlid in _AdcUsedInfoDefine.Keys)
            {
                if (_AdcUsedInfoDefine[chnlid].ContainsKey(DbiMergeMode.Disable))
                {
                    AcqModeAndInterleaveDefineTable[DbiDisableActive].Details[chnlid] = _AdcUsedInfoDefine[chnlid][DbiMergeMode.Disable].Values.ToArray();
                }
            }
            #endregion

            SubbandCnt = 4;
        }

        internal const UInt32 OnlyC1 = 0x1;
        internal const UInt32 OnlyC2 = 0x2;
        internal const UInt32 OnlyC3 = 0x4;
        internal const UInt32 OnlyC4 = 0x8;

        internal const UInt32 C1_C3 = (OnlyC1 | OnlyC3);
        internal const UInt32 C1_C4 = (OnlyC1 | OnlyC4);
        internal const UInt32 C2_C3 = (OnlyC2 | OnlyC3);
        internal const UInt32 C2_C4 = (OnlyC2 | OnlyC4);

        internal const UInt32 DbiDisableActive = (OnlyC1 | OnlyC2 | OnlyC3 | OnlyC4);

        private readonly Dictionary<ChannelId, Dictionary<DbiMergeMode, Dictionary<Int32, AdcUsedInfo>>> _AdcUsedInfoDefine = new()
        {
            [ChannelId.C1] = new()
            {
                [DbiMergeMode.Enable] = new()
                //{
                //    [0] = new AdcUsedInfo(AcqBdNo.B10, 0b11, 0b1111),
                //    [1] = new AdcUsedInfo(AcqBdNo.B7, 0b11, 0b1111),
                //    [2] = new AdcUsedInfo(AcqBdNo.B12, 0b11, 0b1111),
                //    [3] = new AdcUsedInfo(AcqBdNo.B11, 0b11, 0b1111),
                //},
                {
                    [0] = new AdcUsedInfo(AcqBdNo.B0, 0b11, 0b1111) {
                        AdcPorts = new()
                        {
                            {0,1},
                            {1,1},
                        }
                    },
                    [1] = new AdcUsedInfo(AcqBdNo.B1, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                    [2] = new AdcUsedInfo(AcqBdNo.B2, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                    [3] = new AdcUsedInfo(AcqBdNo.B3, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                },
                [DbiMergeMode.Disable] = new()
                {
                    [0] = new AdcUsedInfo(AcqBdNo.B0, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                    [1] = new AdcUsedInfo(AcqBdNo.B1, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                    [2] = new AdcUsedInfo(AcqBdNo.B2, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                    [3] = new AdcUsedInfo(AcqBdNo.B3, 0b11, 0b1111)
                    {
                        AdcPorts = new()
                    {
                        {0,1},
                        {1,1},
                    }
                    },
                },
            },

            //[ChannelId.C2] = new()
            //{
            //    [DbiMergeMode.Enable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B9, 0x3, 0xf),
            //        [1] = new AdcUsedInfo(AcqBdNo.B8, 0x3, 0xf),
            //        [2] = new AdcUsedInfo(AcqBdNo.B7, 0x3, 0xf),
            //    },
            //    [DbiMergeMode.Disable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B9, 0x3, 0xf),
            //    },
            //},

            //[ChannelId.C3] = new()
            //{
            //    [DbiMergeMode.Enable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B10, 0x3, 0xf),
            //        [1] = new AdcUsedInfo(AcqBdNo.B11, 0x3, 0xf),
            //        [2] = new AdcUsedInfo(AcqBdNo.B12, 0x3, 0xf),
            //    },
            //    [DbiMergeMode.Disable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B10, 0x3, 0xf),
            //    },
            //},

            //[ChannelId.C4] = new()
            //{
            //    [DbiMergeMode.Enable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B12, 0x3, 0xf),
            //        [1] = new AdcUsedInfo(AcqBdNo.B11, 0x3, 0xf),
            //        [2] = new AdcUsedInfo(AcqBdNo.B10, 0x3, 0xf),
            //    },
            //    [DbiMergeMode.Disable] = new()
            //    {
            //        [0] = new AdcUsedInfo(AcqBdNo.B12, 0x3, 0xf),
            //    },
            //},
        };
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
        private const uint _AllChnlActive = 0b1111;
        private readonly Dictionary<UInt32, ChannelId[]> _DbiMergeDefine = new()
        {
            [OnlyC1] = new ChannelId[] { ChannelId.C1, ChannelId.C3 },
            [OnlyC2] = new ChannelId[] { ChannelId.C2, ChannelId.C4 },
            [OnlyC3] = new ChannelId[] { ChannelId.C2, ChannelId.C3 },
            [OnlyC4] = new ChannelId[] { ChannelId.C1, ChannelId.C4 },
            [C1_C3] =  new ChannelId[] { ChannelId.C1, ChannelId.C3 },
            [C1_C4] =  new ChannelId[] { ChannelId.C1, ChannelId.C4 },
            [C2_C3] =  new ChannelId[] { ChannelId.C2, ChannelId.C3 },
            [C2_C4] =  new ChannelId[] { ChannelId.C2, ChannelId.C4 },
        };

        internal override AdcUsedInfo? GetAdcUsedInfo(UInt32 chnlActiveState, ChannelId chnlId, Int32 subbandId)
        {
            if (_DbiMergeDefine.ContainsKey(chnlActiveState))
            {
                if (_AdcUsedInfoDefine.ContainsKey(chnlId) && _AdcUsedInfoDefine[chnlId].ContainsKey(DbiMergeMode.Enable)
                    && _AdcUsedInfoDefine[chnlId][DbiMergeMode.Enable].ContainsKey(subbandId) && _DbiMergeDefine[chnlActiveState].Contains(chnlId))
                    return _AdcUsedInfoDefine[chnlId][DbiMergeMode.Enable][subbandId];

                return null;
            }

            if (_AdcUsedInfoDefine.ContainsKey(chnlId) && _AdcUsedInfoDefine[chnlId].ContainsKey(DbiMergeMode.Disable)
                && _AdcUsedInfoDefine[chnlId][DbiMergeMode.Disable].ContainsKey(subbandId))
                return _AdcUsedInfoDefine[chnlId][DbiMergeMode.Disable][subbandId];

            return null;
        }

        internal override AdcUsedInfo[]? GetAdcUsedInfo(UInt32 chnlActiveState, ChannelId chnlId)
        {
            if (_DbiMergeDefine.ContainsKey(chnlActiveState))
            {
                if (_AdcUsedInfoDefine.ContainsKey(chnlId) && _AdcUsedInfoDefine[chnlId].ContainsKey(DbiMergeMode.Enable)
                    && _DbiMergeDefine[chnlActiveState].Contains(chnlId))
                    return _AdcUsedInfoDefine[chnlId][DbiMergeMode.Enable].Values.ToArray();

                return null;
            }

            if (_AdcUsedInfoDefine.ContainsKey(chnlId) && _AdcUsedInfoDefine[chnlId].ContainsKey(DbiMergeMode.Disable))
            {
                return _AdcUsedInfoDefine[chnlId][DbiMergeMode.Disable].Values.ToArray();
            }

            return null;
        }

        internal override UInt32 GetActuallActiveState(UInt32 activeState)
        {
            if (_DbiMergeDefine.ContainsKey(activeState))
                return PublicFunc.ConvertToUniqueHotCode(_DbiMergeDefine[activeState].Select(o => (Int32)o));
            return DbiDisableActive;
        }

        /// <summary>
        /// 查询在指定通道使能状态下，是否可以开启真正的DBI拼合模式
        /// </summary>
        /// <param name="activeState"></param>
        /// <returns></returns>
        internal override Boolean GetDbiMergeState(UInt32 activeState)
        {
            return _DbiMergeDefine.ContainsKey(activeState);
        }
    }

    /// <summary>
    /// DBI拼合模式是否开启
    /// </summary>
    internal enum DbiMergeMode
    { 
        Enable,
        Disable,
    }
}
