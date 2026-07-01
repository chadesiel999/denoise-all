using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public enum MiscDefine
    {
        ExtTrigger_HighImp_DC_ZeroDelta_uV = 1,
        ExtTrigger_HighImp_DC_100VDelta_uV = 2,
        ExtTrigger_HighImp_AC_ZeroDelta_uV = 3,
        ExtTrigger_HighImp_AC_100VDelta_uV = 4,
        ExtTrigger_HighImp_HFR_ZeroDelta_uV = 5,
        ExtTrigger_HighImp_HFR_100VDelta_uV = 6,
        ExtTrigger_HighImp_LFR_ZeroDelta_uV = 7,
        ExtTrigger_HighImp_LFR_100VDelta_uV = 8,

        ExtTrigger_LowImp_DC_ZeroDelta_uV = 21,
        ExtTrigger_LowImp_DC_100VDelta_uV = 22,
        ExtTrigger_LowImp_AC_ZeroDelta_uV = 23,
        ExtTrigger_LowImp_AC_100VDelta_uV = 24,
        ExtTrigger_LowImp_HFR_ZeroDelta_uV = 25,
        ExtTrigger_LowImp_HFR_100VDelta_uV = 26,
        ExtTrigger_LowImp_LFR_ZeroDelta_uV = 27,
        ExtTrigger_LowImp_LFR_100VDelta_uV = 28,

        Ext5Trigger_HighImp_DC_ZeroDelta_uV = 41,
        Ext5Trigger_HighImp_DC_100VDelta_uV = 42,
        Ext5Trigger_HighImp_AC_ZeroDelta_uV = 43,
        Ext5Trigger_HighImp_AC_100VDelta_uV = 44,
        Ext5Trigger_HighImp_HFR_ZeroDelta_uV = 45,
        Ext5Trigger_HighImp_HFR_100VDelta_uV = 46,
        Ext5Trigger_HighImp_LFR_ZeroDelta_uV = 47,
        Ext5Trigger_HighImp_LFR_100VDelta_uV = 48,

        Ext5Trigger_LowImp_DC_ZeroDelta_uV = 61,
        Ext5Trigger_LowImp_DC_100VDelta_uV = 62,
        Ext5Trigger_LowImp_AC_ZeroDelta_uV = 63,
        Ext5Trigger_LowImp_AC_100VDelta_uV = 64,
        Ext5Trigger_LowImp_HFR_ZeroDelta_uV = 65,
        Ext5Trigger_LowImp_HFR_100VDelta_uV = 66,
        Ext5Trigger_LowImp_LFR_ZeroDelta_uV = 67,
        Ext5Trigger_LowImp_LFR_100VDelta_uV = 68,

        TrigTypeDelay_Edge = 201,       //边沿触发
        TrigTypeDelay_PulseWidth,     //脉宽触发		    
        TrigTypeDelay_Video,          //视频触发
        TrigTypeDelay_Pattern,        //码型触发
        TrigTypeDelay_State,          //状态触发
        TrigTypeDelay_TimeOut,        //超时触发，跌落触发
        TrigTypeDelay_SetupHold,      //建立保持触发
        TrigTypeDelay_Runt,           //欠幅
        TrigTypeDelay_Transition,     //过渡
        TrigTypeDelay_Glitch,         //毛刺
        TrigTypeDelay_Window,         //窗口
        TrigTypeDelay_Interval,       //间隔
        TrigTypeDelay_MultiQulified,  //级联触发
        TrigTypeDelay_Serial,         //串行触发

        TrigBoardDelay=220,

        LA_CaliDataBegin=250,
        LA_CaliDataBlock1_VotageBymV=250,
        LA_CaliDataBlock1_RadioMulti1000,//251
        LA_CaliDataBlock2_VotageBymV,//252
        LA_CaliDataBlock2_RadioMulti1000,//253
        LA_CaliDataBlock3_VotageBymV,//
        LA_CaliDataBlock3_RadioMulti1000,
        LA_CaliDataBlock4_VotageBymV,
        LA_CaliDataBlock4_RadioMulti1000,

        Reserved1 = 500,
        Reserved2 = 501,
    }
}
