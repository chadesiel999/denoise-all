using ScopeX.Hardware.Driver.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ScopeX.Hardware.Driver
{
    internal class Adc5200AutoCaliManager
    {
        //在每次Exec开始执行Clear,在执行过程中需要进行ADC校准的时候（目前已知的包括：1、端口改变、2、增益调整、3、相位调整)加入等待列表，最后执行校准等待。
        internal readonly static Adc5200AutoCaliManager Default = new Adc5200AutoCaliManager();

        //最后校准的Adc的端口
        private readonly Dictionary<FPGAAdcPair, Int32> _LastCaliAdcPorts = new Dictionary<FPGAAdcPair, Int32>();

        //需要等待校准的Adc
        private readonly Dictionary<FPGAAdcPair, Int32> _WaitForExecAdcPorts = new Dictionary<FPGAAdcPair, Int32>();

        //当前是否需要等待Adc自校准完成
        public Boolean IsNeedExcute => _WaitForExecAdcPorts.Count > 0;

        //Adc自校准互斥锁
        private object _CaliLock = new();

        //添加需等待的Adc到列表
        public void TryAddWaitForCali(Int32 fpgaIndex, Int32 adcIndex, Int32 portIndex)
        {
            //确认列表中是否存在
            foreach (var waitItem in _WaitForExecAdcPorts)
            {
                if ((waitItem.Key.FPGAIndex == fpgaIndex) && (waitItem.Key.AdcIndex == adcIndex))
                    return;
            }

            //确认最近一次是否发送过
            foreach (var lastItem in _LastCaliAdcPorts)
            {
                if (lastItem.Key.FPGAIndex == fpgaIndex && lastItem.Key.AdcIndex == adcIndex && lastItem.Value == portIndex)
                    return;
            }

            _WaitForExecAdcPorts.Add(new FPGAAdcPair() { FPGAIndex = fpgaIndex, AdcIndex = adcIndex }, portIndex);
        }

        //等待Adc自校准完成
        public void ExecAutoCali(Boolean bNeedSetCaliReg62, UInt32 FinishedSendCaliReg0x62Value)
        {
            if (_WaitForExecAdcPorts.Count == 0)
                return;

            lock(_CaliLock)
            {
                HdDebugLogger.Log($"[{DateTime.Now}]: Adc Auto Calibration Start!");
                //once autocali, then cali the all ports
                ForceAddAll();

                //通道下电
                AbstractController_AnalogChannel.PowerOff();
                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x00);//Close FD

                #region Step1:在之前，已经打开了各个ADC的自动校准(0x6c寄存器 生成上升沿）
                foreach (var waitItem in _WaitForExecAdcPorts)
                {
                    //开始校准
  //                  Boadr_Acq_JiHe_MSO8000X.SendCmdToAD5200_OneFpga((AcqBdNo)waitItem.Key.FPGAIndex, waitItem.Key.AdcIndex, 0x006B, 0x06);
                    Boadr_Acq_JiHe_MSO8000X.Send5200CmdWithCS_OneFpga((AcqBdNo)waitItem.Key.FPGAIndex, waitItem.Key.AdcIndex, 0x006C, 0); //CAL_SOFT_TRIG=0
                    Boadr_Acq_JiHe_MSO8000X.Send5200CmdWithCS_OneFpga((AcqBdNo)waitItem.Key.FPGAIndex, waitItem.Key.AdcIndex, 0x006C, 1); //CAL_SOFT_TRIG=0

                    //记录最后校准的Adc的端口
                    if (_LastCaliAdcPorts.Any(item => item.Key.FPGAIndex == waitItem.Key.FPGAIndex && item.Key.AdcIndex == waitItem.Key.AdcIndex))
                    {
                        var lastitem = _LastCaliAdcPorts.First(item => item.Key.FPGAIndex == waitItem.Key.FPGAIndex && item.Key.AdcIndex == waitItem.Key.AdcIndex);
                        _LastCaliAdcPorts.Remove(lastitem.Key);
                    }
                    _LastCaliAdcPorts.Add(waitItem.Key, waitItem.Value);

                }
                #endregion

                #region Step2:通过读取0x6a 寄存器 检查各个ADC的自动校准是否完成。集中检查全部的。此检查与系统安装的采集卡的个数和位置有关，与通道没有任何关系
                Boolean ballfinished = false;
                Stopwatch stopwatch = new Stopwatch();
                Int64 waitms = 2_000;
                if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                    waitms = 1;
                stopwatch.Start();
                while (!ballfinished && stopwatch.ElapsedMilliseconds < waitms)
                {
                    ballfinished = true;
                    foreach (var waitItemKey in _WaitForExecAdcPorts.Keys)
                    {
                        Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)waitItemKey.FPGAIndex, (UInt32)(waitItemKey.AdcIndex + 1));
                        UInt32 data = Hd.CurrProduct!.AcqBd!.ReadFromAD5200((AcqBdNo)waitItemKey.FPGAIndex, waitItemKey.AdcIndex, 0x6a);
                        Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)waitItemKey.FPGAIndex, 0);
                        if ((data & 0x3) == 0)
                            ballfinished = false;
                    }
                }
                if (!ballfinished)
                {
                    Hd.SysLogger!.Invoke("Adc selfCali failed!", "Warning");
                }
                stopwatch.Stop();
                #endregion

                #region Step3:完毕自动检查 
                if (bNeedSetCaliReg62)
                {
                    foreach (var waitItemKey in _WaitForExecAdcPorts.Keys)
                    {
                        Boadr_Acq_JiHe_MSO8000X.Send5200CmdWithCS_OneFpga((AcqBdNo)waitItemKey.FPGAIndex, waitItemKey.AdcIndex, 0x0062, FinishedSendCaliReg0x62Value);//校准配置
                    }
                }
                #endregion

                _WaitForExecAdcPorts.Clear();

                Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x01);//Open FD //cij

                ////通道上电
                //AbstractController_AnalogChannel.PowerOn();
                //AbstractController_AnalogChannel.Init();
                //AbstractController_AnalogChannel.CtrlAnalogChannelSet();
                HdDebugLogger.Log($"[{DateTime.Now}]: Adc Auto Calibration End!");
            }
        }

        /// <summary>
        /// Force add all ports in the waiting list
        /// </summary>
        private void ForceAddAll()
        {
            _WaitForExecAdcPorts.Clear();

            AcqModeAndInterleaveDefine define = Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            foreach (var dtl in define.Details)
            {
                foreach (var adcInfo in dtl.Value)
                {
                    List<Int32> adcIds = new List<Int32>();
                    for (Int32 i = 0; i < sizeof(Int32); i++)
                    {
                        if (((adcInfo.Adc >> i) & 0x1) == 0x1)
                            adcIds.Add(i);
                    }
                    foreach (var adcid in adcIds)
                    {
                        _WaitForExecAdcPorts.Add(new FPGAAdcPair() { FPGAIndex = (Int32)adcInfo.AcqBdNo, AdcIndex = adcid % 2 }, adcInfo.AdcPorts[adcid%2]);
                    }
                }
            }
        }

        /// <summary>
        /// 强行执行一遍当前模式的Adc自校准
        /// </summary>
        public void ExecForceCali()
        {
            ForceAddAll();
            ExecAutoCali(true, 0x05);
        }
    }
}
