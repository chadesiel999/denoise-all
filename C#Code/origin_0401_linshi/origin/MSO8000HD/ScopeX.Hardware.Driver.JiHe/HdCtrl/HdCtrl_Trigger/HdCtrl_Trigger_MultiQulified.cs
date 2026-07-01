#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Hardware.Driver
{
    internal partial class HdCtrl_Trigger
    {
        static Dictionary<string, KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>> MultiQulifiedDefine = new Dictionary<string, KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>>()
        {
            ["Edge"]=new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(0, Config_Edge_MultiQulified),
            ["PulseWidth"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(1, Config_PulseWidth_MultiQulified),
            ["Transition"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(2, Config_Transition_MultiQulified),
            ["Runt"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(3, Config_Runt_MultiQulified),
            ["TimeOut"] = new KeyValuePair<uint, Action<ITriggerTypeOptions?, int>>(4, Config_TimeOut_MultiQulified),
        };
        private static UInt32 _forceCnt = 0;
        internal static void Config_MultiQulified()//????
        {
            long DurationByps = 0;
            DelayOpt delayType_A = DelayOpt.Time;

            int EventA_type = 0;
            ChannelId EventA_source = ChannelId.C1;
            int EventB_type = 0;
            ChannelId EventB_source = ChannelId.C2;

            EdgeSlope EventA_polar = EdgeSlope.Rise;
            EdgeSlope EventB_polar = EdgeSlope.Rise;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, 1);

            var EventOpts = Hd.UIMessage?.Trigger?.TrigMultiQualified?.EventOptions;
            if (EventOpts?[0] != null)
            {
                string triggerType_A = EventOpts[0]!.Value.Name;
                string triggerType_B = EventOpts[1]!.Value.Name;

                switch (triggerType_A)//判断事件A的触发类型
                {
                    case "Edge":
                        Config_Edge_MultiQulified(Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions[0]!.Value.TriggerOption, 1);//执行触发类型对应的Config_Edge_MultiQulified方法，在方法里面下发对应参数
                        TrigEdgeOptions EdgeOptions = (TrigEdgeOptions)EventOpts[0]!.Value!.TriggerOption!;
                        EventA_type = 0;
                        EventA_source = EdgeOptions.Source;
                        EventA_polar = EdgeOptions.Slope;
                        break;
                    case "PulseWidth":
                        Config_PulseWidth_MultiQulified(Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions[0]!.Value.TriggerOption, 1);//执行触发类型对应的Config_Edge_MultiQulified方法，在方法里面下发对应参数
                        TrigPulseOptions PulseWidthOptions = (TrigPulseOptions)EventOpts[0]!.Value!.TriggerOption!;
                        EventA_type = 1;
                        EventA_source = PulseWidthOptions.Source;
                        break;
                }

                switch (triggerType_B)//判断事件B的触发类型
                {
                    case "Edge":
                        Config_Edge_MultiQulified(Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions[1]!.Value.TriggerOption, 1);//执行触发类型对应的Config_Edge_MultiQulified方法，在方法里面下发对应参数
                        TrigEdgeOptions EdgeOptions = (TrigEdgeOptions)EventOpts[1]!.Value!.TriggerOption!;
                        EventB_type = 0;
                        EventB_source = EdgeOptions.Source;
                        EventB_polar = EdgeOptions.Slope;
                        break;
                    case "PulseWidth":
                        Config_PulseWidth_MultiQulified(Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions[1]!.Value.TriggerOption, 1);//执行触发类型对应的Config_Edge_MultiQulified方法，在方法里面下发对应参数
                        TrigPulseOptions PulseWidthOptions = (TrigPulseOptions)EventOpts[1]!.Value!.TriggerOption!;
                        EventB_type = 1;
                        EventB_source = PulseWidthOptions.Source;
                        break;
                }

                delayType_A = EventOpts[0]!.Value.DelayType;
                DurationByps = EventOpts[0]!.Value.DurationByps;
            }
            bool timeRstEnable = Hd.UIMessage?.Trigger?.TrigMultiQualified?.TimeRstEnable ?? false;
            bool stateRstEnable = Hd.UIMessage?.Trigger?.TrigMultiQualified?.StateRstEnable ?? false;
            bool transRstEnable = Hd.UIMessage?.Trigger?.TrigMultiQualified?.StateRiseRstEnable ?? false;
            Int16 Timerst = 0;
            Int16 Staterst = 0;
            Int16 Tranrst = 0;
            if (timeRstEnable == true)
            {
                Timerst = 0b1;
            }
            if (stateRstEnable == true)
            {
                Staterst = 0b1;
            }
            if (transRstEnable == true)
            {
                Tranrst = 0b1;
            }

            Int32 RstCtrlWords = 0x0000;
            RstCtrlWords = (Timerst << 2 | Staterst << 1 | Tranrst) & 0xFFFF;
            double timeRst = (Hd.UIMessage?.Trigger?.TrigMultiQualified?.TimeRst ?? 0) / 1000;


            UInt32 currCnt = Hd.UIMessage?.Trigger?.TrigMultiQualified?.ForceRstCnt ?? 0;
            if (currCnt != _forceCnt)
            {
                //发010
                _forceCnt = currCnt;
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0018);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
            }
            if (Staterst == 0b1)
            {
                //发010
                _forceCnt = currCnt;
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0018);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
            }
            if (Tranrst == 0b1)
            {
                //发010
                _forceCnt = currCnt;
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0018);
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, 0x0000);
            }
            //UInt32 CascadeStatus = HdIO.ReadReg(ProcBdReg.R.TrigAdv_CascadeStatusRead);

            Int32 Event_type = 0x0000;
            Int32 Event_source = 0x0000;
            Int32 Event_polar = 0x0000;
            Event_type = (((Int16)EventB_type) << 4) | (Int16)EventA_type;
            Event_source = (((Int16)EventB_source) << 4) | (Int16)EventA_source;
            if (EventA_polar == EdgeSlope.Rise)
                Event_polar = (((Int16)EventA_polar) ^ 0x1) | ((((Int16)EventB_polar) ^ 0x1) << 1);

            UInt64 extramNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.ExtractNumFromAdc ?? 1;


            Int64 Delay_A = 0;
            Int64 Delay_A_2nd = 0;
            if (delayType_A == DelayOpt.Time)
            {
                Delay_A = (0b1 << 15) | (DurationByps) / 4000;
                Delay_A_2nd = (0b1 << 15) | (DurationByps) / 50 / (Int32)extramNum;
            }
            else
            {
                Delay_A = (uint)(Hd.UIMessage?.Trigger?.TrigMultiQualified?.EventOptions?[0].Value.Counts ?? 0);
                Delay_A_2nd = Delay_A;
            }

            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeSet, (uint)RstCtrlWords);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeEventType, (uint)Event_type);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeEventSource, (uint)Event_source);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeDelayA, (uint)Delay_A);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_CascadeResetTimeSet, (uint)timeRst);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvCodeCascadeSet, (uint)Event_polar);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvCodeCascadeEventType, (uint)Event_type);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvCodeCascadeEventSource, (uint)Event_source);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvCodeCascadeSetDelayA, (uint)Delay_A_2nd);
        }
    }
}
#endif
