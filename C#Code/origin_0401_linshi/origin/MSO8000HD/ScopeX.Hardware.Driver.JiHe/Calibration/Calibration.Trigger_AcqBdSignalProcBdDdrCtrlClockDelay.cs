using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public partial class Cali
    {
        internal void DoCali_Trigger_AcqBdSignalProcBdDdrCtrlClockDelay()
        {
            HdMessage backHdMessage = Hd.UIMessage! with { };
            #region 自动校准代码
            Controller_Trigger_Standard currTriggerController = (Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!;

            //控制生成触发校准用测试波形
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8700);//[15]：test data [11:8] mask data 1 2 3 4  [0]0-trigger [12]-discard location enable

            //通道开启
            HdMessage.AnalogOptions ch = backHdMessage.Analog[0] with
            {
                Active = true,
                Position = 0,
                PositionIndex = 0,
                Scale = 5000,
                ScaleIndex = 12,
                InterChannelOffset = 0,
                IsInverted = false,
            };

            HdMessage.AnalogOptions[] analogOptions4CH = { ch, ch, ch, ch };
            //时基挡位设置
            HdMessage.TimebaseOptions testModeTimebaseOptions = backHdMessage!.Timebase! with
            {
                TmbPositionIndex = 5000,
                TmbScale = 0.02,
                TmbPosition = 0,
                TmbScaleIndex = 12,
                AcqLength = AnaChnlStorageMode.Long,
                AcqMode = AnaChnlAcqMode.Normal,
                IsScan = false,
            };
            //触发信号设置
            HdMessage.TriggerOptions testModeTriggerOptions = backHdMessage.Trigger with
            {

                Edge = backHdMessage.Trigger.Edge with { Coupling = TriggerCoupling.DC, Slope = EdgeSlope.Rise, Source = ChannelId.C1, Position = 0, PosIndex = 0 },
                Mode = TriggerMode.Normal,
                TrigType = TriggerType.Edge,
            };
            //组装
            HdMessage caliMessage = backHdMessage with
            {
                Timebase = testModeTimebaseOptions,
                Trigger = testModeTriggerOptions,
                Analog = analogOptions4CH
            };

            Hd.Execute(caliMessage);
            GetTestWavePosInInterp();
            var readinfo = new List<ReadInfo>();
            readinfo.Add(new ReadInfo(AcqDataType.AnalogChannel,
                new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 }, 
                new WfmPkgInfo(25000, 0.2, 0.1), 
                ""));
            Dictionary<AcqDataType,double> samplingRate = new Dictionary<AcqDataType,double>();
            Hd.AcqWave(false, false, readinfo,ref samplingRate);

            //实时挡位校正设置
            testModeTimebaseOptions = backHdMessage!.Timebase! with
            {
                TmbPositionIndex = 5000,
                TmbScale = 0.05,
                TmbPosition = 0,
                TmbScaleIndex = 13,
                AcqLength = AnaChnlStorageMode.Long,
                AcqMode = AnaChnlAcqMode.Normal,
                IsScan = false,
            };
            readinfo.Clear();
            readinfo.Add(new ReadInfo(AcqDataType.AnalogChannel,
               new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
               new WfmPkgInfo(25000, 0.5, 0.25),
               ""));

            //关闭数字放大功能
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch1_H, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch1_L, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch2_H, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch2_L, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch3_H, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch3_L, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch4_H, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DigZoom_Gainch4_L, 1);

            //重新修改时基挡位设置
            caliMessage = backHdMessage with
            {
                Timebase = testModeTimebaseOptions,
            };
            Hd.Execute(caliMessage);
            GetTestWavePosInRealTime();
            Hd.AcqWave(false, false, readinfo,ref samplingRate);
            #endregion

            #region 还原
            //关闭测试波形
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x0700);//[15]：test data [11:8] mask data 1 2 3 4  [0]0-trigger [12]-discard location enable
            HdMessage newMessage = backHdMessage with { Command = 0xffff_ffff_ffff_fff };
            Hd.Execute(newMessage);
            Hd.AcqWave(false, false, readinfo, ref samplingRate );

            #endregion
        }


        private async void GetTestWavePosInInterp()
        {
            await Task.Run(() =>
            {
                uint DDRPosReadBack = 0;
                uint FIFOPosReadBack = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                uint InterpNum = 1U;
                while (true)
                {
                    if (InterpNum == 1)
                    {
                        break;
                    }

                    DDRPosReadBack = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_acq_reverse_rd_reg_0, AcqBdNo.B1);
                    FIFOPosReadBack = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B1);

                    if (((DDRPosReadBack & 0X8000) != 0) && ((FIFOPosReadBack & 0x8000) != 0))
                    {
                        break;
                    }

                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                (uint DDRPosSkew, uint FifoPosSkew) = CaculatePosDelay(DDRPosReadBack & 0X7FFF, FIFOPosReadBack & 0X7FFF);
            });
        }

        private (uint, uint) CaculatePosDelay(uint DDRPosReadBack, uint FifoPosReadBack)
        {
            (Int64 xdepth, Int64 waveDepth) = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetTrigXDepth();

            //DDR位置歪斜（FPGA在DDR的输出数据流中寻找触发数据点的时候，会标明触发数据点在数据流的第几列，假如触发点在第8列，则回读的数值为8，实际触发点前面有7列；
            //而软件计算预触发深度的时候是按照数据量计算的，GetAcqBdTrigCtrl_PreDepth返回值为触发点前的数据量，所以这两个概念之间差了1，因此在计算歪斜的时候要协商加1）
            uint DDRPosSkew = ((Hd.CurrProduct!.Acquirer_AnalogChannel!.GetAcqBdTrigCtrl_PreDepth(xdepth) + ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).predepth_offset)) / 8 - DDRPosReadBack + 1;
            //FIFO位置歪斜
            uint FifoPosSkew = ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).defaultFragLength - FifoPosReadBack + 1;

            //由DDR模块前的fifo帮助实现的延迟量
            uint delayByFIFo = (FifoPosSkew - ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).DataAfterTrigInFrag);

            //延迟FIFO的对应值（由于延迟fifo的位宽只有DDR的一半，所以延迟量要乘2）
            ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).TrigDecDelay = delayByFIFo * 2;
            //触发片段全部长度
            ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).defaultFragLength = ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).FragLengthDefinedByInterp + ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).DataAfterTrigInFrag;

            ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).predepth_offset = (((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).FragLengthDefinedByInterp - (delayByFIFo - DDRPosSkew)) * 8;

            //实时挡位的预触发修正量计算
            ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).RealTimeOffset = (DDRPosSkew - delayByFIFo) * 8;

            return (DDRPosSkew, FifoPosSkew);

        }


        private async void GetTestWavePosInRealTime()
        {
            await Task.Run(() =>
            {
                uint RealTimePosReadBack = 0;
                int RealTimePosSkew = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (true)
                {
                    RealTimePosReadBack = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Decoder_TimeDelaycntlock, AcqBdNo.B1);

                    if (((RealTimePosReadBack & 0X8000) != 0))
                    {
                        break;
                    }

                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        break;
                    }
                }
                stopwatch.Stop();

                (long xdepth, _) = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetTrigXDepth();

                RealTimePosSkew = (int)xdepth - (int)(RealTimePosReadBack & 0X7FFF);

                //计算偏移量
                ((Controller_Trigger_Standard)Hd.CurrProduct!.Ctrl_Trigger!).RealTimeFineDiscard = (int)Math.Ceiling((double)RealTimePosSkew / 8);



            });
        }








    }
}
