using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        private const Int32 IterationCount = 16;
        internal Boolean IsCaliExttrigger { get; private set; } = false;
        public Boolean AutoCaliExeTrigger_Exec()
        {
            var sb = new StringBuilder();
            AppendCaliLog(sb, "开始外触发校准");
            IsCaliExttrigger = true;
            var bok = false;
            HdMessage backupmsg = Hd.UIMessage! with { };

            var couplinglist = new List<TriggerCoupling>
            {
                TriggerCoupling.DC,
                TriggerCoupling.AC,
                TriggerCoupling.LFR,/*使用AC的校准值*/
                TriggerCoupling.HFR,/*使用DC的校准值*/
                //TriggerCoupling.NR,/*使用DC的校准值*/
            };

            var impedancelist = new List<TriggerImpedance>
            {
                TriggerImpedance.High1M,
                TriggerImpedance.Low50,
            };

            var channellist = new List<ChannelId>
            {
                ChannelId.Ext,
                ChannelId.Ext5 //使用Ext参数
            };

            var ctrlwordpermv = 0xFFFF / (Constants.EXT_TRIGGER_MAX_MV - Constants.EXT_TRIGGER_MIN_MV);
            //通道循环
            foreach (var chnl in channellist)
            {
                //耦合
                foreach (var coupling in couplinglist)
                {
                    //阻抗
                    foreach (var impedance in impedancelist)
                    {
                        /// 配置 通道、耦合、阻抗信息
                        var edge = Hd.UIMessage!.Trigger!.Edge! with
                        {
                            Source = chnl,
                            Coupling = coupling,
                            Impedance = impedance,
                            Slope = EdgeSlope.Rise
                        };
                        var ratio = chnl == ChannelId.Ext ? 1D : 5D;
                        var stepbymv = chnl == ChannelId.Ext ? 3 : 10;
                        Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                        ConfigHardware(Hd.UIMessage, 5);

                        AppendCaliLog(sb, $"{chnl} {coupling} {impedance}开始寻找初始值");

                        var ctrlWord = ToCtrlWordBymV((Constants.EXT_TRIGGER_MAX_MV * ratio + Constants.EXT_TRIGGER_MIN_MV * ratio) / 2, ratio);
                        var comparesesult = CompareResult.ZeroAndOne;
                        //var comparevalueymin = CompareResult.ZeroAndOne;//可以用来确定搜索方向
                        var topCtrlWord = 0D;
                        var bottomCtrlWord = 0D;
                        for (var iteration = 2; iteration <= IterationCount; iteration++)
                        {
                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = Constants.EXT_TRIGGER_MIN_MV * ratio };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();
                            //comparevalueymin = GetCompareValue();

                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(ctrlWord, ratio) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();
                            comparesesult = GetCompareValue();
#if DEBUG
                            Debug.WriteLine($"[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} 初始值：{TomVByCtrlWord(ctrlWord, ratio)}mV {comparesesult} 第{iteration - 2}次迭代");
#endif 
                            AppendCaliLog(sb, $"{chnl} {coupling} {impedance} 初始值：{TomVByCtrlWord(ctrlWord, ratio)}mV 比较结果：{comparesesult} 第{iteration - 2}次迭代");


                            if (comparesesult == CompareResult.One || comparesesult == CompareResult.ZeroAndOne)
                            {
                                ctrlWord = ctrlWord - 0xFFFF / (Int32)Math.Pow(2, iteration);
                            }
                            else if (comparesesult == CompareResult.Zero)
                            {
                                ctrlWord = ctrlWord + 0xFFFF / (Int32)Math.Pow(2, iteration);
                            }
                        }
#if DEBUG
                        Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} 初始值：{TomVByCtrlWord(ctrlWord, ratio)}mV {comparesesult}");
#endif
                        AppendCaliLog(sb, $"{chnl} {coupling} {impedance} 初始值：{TomVByCtrlWord(ctrlWord, ratio)}mV 比较结果：{comparesesult}");

                        var defaultresult = comparesesult;
                        var newctrlword = ctrlWord;
                        if (defaultresult == CompareResult.ZeroAndOne)
                        {
                            defaultresult = CompareResult.One;
                        }
                        while (defaultresult == comparesesult || comparesesult == CompareResult.ZeroAndOne)
                        {
                            if (defaultresult == CompareResult.One)
                                newctrlword -= ctrlwordpermv / ratio * stepbymv;
                            else
                                newctrlword += ctrlwordpermv / ratio * stepbymv;

                            if (newctrlword < 0 || newctrlword > 0xFFFF)
                            {
                                break;
                            }

                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(ctrlWord, ratio) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();

                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(newctrlword, ratio) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();
                            comparesesult = GetCompareValue();

                            if ((defaultresult == CompareResult.One && comparesesult == CompareResult.Zero) || (defaultresult == CompareResult.Zero && comparesesult == CompareResult.One))
                            {
                                ctrlWord = newctrlword;
                                topCtrlWord = ctrlWord;
#if DEBUG
                                Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} Top值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
#endif
                                break;
                            }
#if DEBUG
                            Debug.WriteLine($"[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} Top值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
#endif
                            AppendCaliLog(sb, $"{chnl} {coupling} {impedance} Top值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
                            Thread.Sleep(10);
                        }

                        defaultresult = comparesesult;
                        newctrlword = ctrlWord;

                        while (defaultresult == comparesesult || comparesesult == CompareResult.ZeroAndOne)
                        {
                            if (defaultresult == CompareResult.One)
                                newctrlword -= ctrlwordpermv / ratio * stepbymv;
                            else
                                newctrlword += ctrlwordpermv / ratio * stepbymv;

                            if (newctrlword < 0 || newctrlword > 0xFFFF)
                            {
                                break;
                            }

                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(ctrlWord, ratio) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();

                            edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(newctrlword, ratio) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            //ConfigHardware(Hd.UIMessage, 5);
                            CtrlAnalogChannel_JiHe2d5G.CtrlExtTrig();
                            comparesesult = GetCompareValue();

                            if ((defaultresult == CompareResult.One && comparesesult == CompareResult.Zero) || (defaultresult == CompareResult.Zero && comparesesult == CompareResult.One))
                            {
                                ctrlWord = newctrlword;
                                bottomCtrlWord = ctrlWord;
#if DEBUG
                                Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} Bottom值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
#endif
                                break;
                            }
#if DEBUG
                            Debug.WriteLine($"[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} Bottom值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
#endif
                            AppendCaliLog(sb, $"{chnl} {coupling} {impedance} Bottom值：{TomVByCtrlWord(newctrlword, ratio)}mV {comparesesult}");
                            Thread.Sleep(10);
                        }

#if false

                        for (var newctrlword = ctrlWord - 50 * ctrlWordPermV; newctrlword < 0xFFFF /*ctrlWord - 100 * ctrlWordPermV*/; newctrlword += ctrlWordPermV)
                        {
                            HdMessage.TrigEdgeOptions edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(newctrlword) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            ConfigHardware(Hd.UIMessage, 2);
                            compareResult = GetCompareValue();
                            if (compareResult == CompareResult.One)
                            {
                                ctrlWord = newctrlword;
                                topCtrlWord = ctrlWord;
#if DEBUG
                                Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {impedance} {coupling} Top值：{TomVByCtrlWord(newctrlword)}mV {compareResult}");
#endif
                                break;
                            }
#if DEBUG
                            Debug.WriteLine($"[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {impedance} {coupling} Top值：{TomVByCtrlWord(newctrlword)}mV {compareResult}");
#endif
                        }

                        for (var newctrlword = ctrlWord + 50 * ctrlWordPermV; newctrlword > 0/*ctrlWord + 100 * ctrlWordPermV*/; newctrlword -= ctrlWordPermV)
                        {
                            HdMessage.TrigEdgeOptions edge = Hd.UIMessage!.Trigger!.Edge! with { Position = TomVByCtrlWord(newctrlword) };
                            Hd.UIMessage = Hd.UIMessage with { Trigger = Hd.UIMessage.Trigger with { Edge = edge } };
                            ConfigHardware(Hd.UIMessage, 2);
                            compareResult = GetCompareValue();

                            if (compareResult == CompareResult.Zero)
                            {
                                ctrlWord = newctrlword;
                                bottomCtrlWord = ctrlWord;
#if DEBUG
                                Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {impedance} {coupling} Bottom值：{TomVByCtrlWord(newctrlword)}mV {compareResult}");
#endif
                                break;
                            }
#if DEBUG
                            Debug.WriteLine($"[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {impedance} {coupling} Bottom值：{TomVByCtrlWord(newctrlword)}mV {compareResult}");
#endif
                        }
#endif
                        var midCtrlWord = (bottomCtrlWord + topCtrlWord) / 2;
                        var midmV = TomVByCtrlWord(midCtrlWord, ratio);
#if DEBUG
                        Debug.WriteLine($"Info:[{DateTime.Now.ToString("G")}] [Calibration exttrigger] {chnl} {coupling} {impedance} 校准值：（DAC：{midCtrlWord}，Offset：{midmV}mV）");
#endif
                        AppendCaliLog(sb, $"{chnl} {coupling} {impedance} 校准值：（DAC：{midCtrlWord}，Offset：{midmV}mV）");
                        var index = GetZeroIndex(chnl, impedance, coupling);
                        MiscData.Default[index] = (Int32)(midmV * 1e3);//to uV
                        _FinishedItemCount++;

                    }// end 阻抗

                } // end 耦合

            }// end 通道循环
            AppendCaliLog(sb, $"外触发校准完成，退出流程");
            PrintCaliLog(sb, BaselineCaliLogFileName);
            Helper.GetICaliData(CaliDataType.Misc)?.SaveToFile();
            ConfigHardware(backupmsg, 10);
            IsCaliExttrigger = false;
            _FinishedItemCount++;
            return bok;
        }

        private CompareResult GetCompareValue()
        {
            var curresult = 0u;
            var list = new List<UInt32>();
            var CompareRes = CompareResult.ZeroAndOne;
            for (int i = 0; i < 10; i++)//读取10次值
            {
                var l = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_0);
                var h = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_1);
                var r = (h << 16) | l;
                curresult = r & 0xFFFFFFFF;
                if (curresult == 0xFFFFFFFF)
                {
                    list.Add(1);

                }
                else
                    list.Add(0);
                Thread.Sleep(10);
            }
            var zerocount = list.Where(r => r == 0).ToList().Count();
            var onecount = list.Where(r => r == 1).ToList().Count();
            if (zerocount == list.Count)
            {
                CompareRes = CompareResult.Zero;
            }
            if (onecount == list.Count)
            {
                CompareRes = CompareResult.One;
            }
            return CompareRes;
        }

        private Double ToCtrlWordBymV(Double PositionBymV, Double ratio)
        {
            return (Constants.EXT_TRIGGER_MAX_MV * ratio - PositionBymV) / (Constants.EXT_TRIGGER_MAX_MV * ratio - Constants.EXT_TRIGGER_MIN_MV * ratio) * 0xFFFF;
        }

        private Int32 TomVByCtrlWord(Double CtrlWord, Double ratio)
        {
            return (Int32)(Constants.EXT_TRIGGER_MAX_MV * ratio - CtrlWord * (Constants.EXT_TRIGGER_MAX_MV * ratio - Constants.EXT_TRIGGER_MIN_MV * ratio) / 0xFFFF);
        }

        private Double ToCtrlWordBymDiv(Double SensitivityBymdiv, Double ratio)
        {
            return (Constants.EXT_TRIGGER_MAX_MV * ratio - SensitivityBymdiv) / (Constants.MAX_TRIGGER_SENSITIVITY_MDIV * ratio - Constants.MIN_TRIGGER_SENSITIVITY_MDIV * ratio) * 0xFFFF;
        }
        private Int32 TomDivByCtrlWord(Double CtrlWord, Double ratio)
        {
            return (Int32)(Constants.EXT_TRIGGER_MAX_MV * ratio - CtrlWord * (Constants.MAX_TRIGGER_SENSITIVITY_MDIV * ratio - Constants.MIN_TRIGGER_SENSITIVITY_MDIV * ratio) / 0xFFFF);
        }

        private Int32 GetZeroIndex(ChannelId paramChannelId, TriggerImpedance paramTriggerImpedance, TriggerCoupling paramTriggerCoupling)
        {
            return (paramChannelId, paramTriggerImpedance, paramTriggerCoupling) switch
            {
                //Ext
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.DC) => ((int)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV),
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.DC) => ((int)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV),

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.AC) => ((int)MiscDefine.ExtTrigger_HighImp_AC_ZeroDelta_uV),
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.AC) => ((int)MiscDefine.ExtTrigger_LowImp_AC_ZeroDelta_uV),

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.LFR) => ((int)MiscDefine.ExtTrigger_HighImp_LFR_ZeroDelta_uV),
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.LFR) => ((int)MiscDefine.ExtTrigger_LowImp_LFR_ZeroDelta_uV),

                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.HFR) => ((int)MiscDefine.ExtTrigger_HighImp_HFR_ZeroDelta_uV),
                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.HFR) => ((int)MiscDefine.ExtTrigger_LowImp_HFR_ZeroDelta_uV),


                //Ext5
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.DC) => ((int)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.DC) => ((int)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV),

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.AC) => ((int)MiscDefine.Ext5Trigger_HighImp_AC_ZeroDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.AC) => ((int)MiscDefine.Ext5Trigger_LowImp_AC_ZeroDelta_uV),

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.LFR) => ((int)MiscDefine.Ext5Trigger_HighImp_LFR_ZeroDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.LFR) => ((int)MiscDefine.Ext5Trigger_LowImp_LFR_ZeroDelta_uV),

                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.HFR) => ((int)MiscDefine.Ext5Trigger_HighImp_HFR_ZeroDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.HFR) => ((int)MiscDefine.Ext5Trigger_LowImp_HFR_ZeroDelta_uV),

                (_, _, _) => 0,
            };
        }
        internal enum CompareResult
        {
            Zero = 0,
            One = 1,
            ZeroAndOne = 2
        }
    }
}
