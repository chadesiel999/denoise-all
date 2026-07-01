using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class Adc5200WaitForAutoCaliFinished
    {
        //在每次Exec开始执行Clear,在执行过程中需要进行ADC校准的时候（目前已知的包括：1、端口改变、2、增益调整、3、相位调整)加入等待列表，最后执行校准等待。
        internal readonly static Adc5200WaitForAutoCaliFinished Default = new Adc5200WaitForAutoCaliFinished();
        private readonly List<FPGAAdcPair> fpgaAdcPairList = new List<FPGAAdcPair>();
        public void TryAdd(int in_FPGAIndex,int in_AdcIndex)
        {
            bool bAlreadyExist = false;
            foreach (var v in fpgaAdcPairList)
            {
                if ((v.FPGAIndex== in_FPGAIndex) && (v.AdcIndex == in_AdcIndex))
                {
                    bAlreadyExist = true;
                    break;
                }
            }
            if (bAlreadyExist)
                return;
            fpgaAdcPairList.Add(new() { FPGAIndex = in_FPGAIndex, AdcIndex = in_AdcIndex });
        }
        public void Init()
        {
            fpgaAdcPairList.Clear();
        }
        public bool bNeedWaitfor => fpgaAdcPairList.Count > 0;
        public void ExecWaitForFinished(bool bNeedSetCaliReg62, UInt32 FinishedSendCaliReg0x62Value)
        {
            if (fpgaAdcPairList.Count == 0)
                return;
            #region Step1:在之前，已经打开了各个ADC的自动校准(0x6c寄存器 生成上升沿）
            foreach (var v in fpgaAdcPairList)
            {
                //开始校准
                AcqBd_DBI13G.Send5200CmdWithCS_OneFpga((AcqBdNo)v.FPGAIndex, v.AdcIndex, 0x006C, 0); //CAL_SOFT_TRIG=0
                AcqBd_DBI13G.Send5200CmdWithCS_OneFpga((AcqBdNo)v.FPGAIndex, v.AdcIndex, 0x006C, 1); //CAL_SOFT_TRIG=0
            }
            #endregion

            #region Step2:通过读取0x6a 寄存器 检查各个ADC的自动校准是否完成。集中检查全部的。此检查与系统安装的采集卡的个数和位置有关，与通道没有任何关系
            bool bAllFinished = false;
            Stopwatch stopwatch = new Stopwatch();
            long waitMs = 2_000;
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                waitMs = 1;
            stopwatch.Start();
            while (!bAllFinished && stopwatch.ElapsedMilliseconds < waitMs)
            {
                bAllFinished = true;
                foreach (var v in fpgaAdcPairList)
                {
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)v.FPGAIndex, (UInt32)(v.AdcIndex + 1));
                    uint data = Hd.CurrProduct!.AcqBd!.ReadFromAD5200((AcqBdNo)v.FPGAIndex, v.AdcIndex, 0x6a);
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)v.FPGAIndex, 0);
                    if ((data & 0x3) == 0)
                        bAllFinished = false;
                }
            }
            stopwatch.Stop();
            #endregion

            #region Step3:完毕自动检查 
            if (bNeedSetCaliReg62)
            {
                foreach (var v in fpgaAdcPairList)
                {
                    AcqBd_DBI13G.Send5200CmdWithCS_OneFpga((AcqBdNo)v.FPGAIndex, v.AdcIndex, 0x0062, FinishedSendCaliReg0x62Value);//校准配置
                }
            }
            //if (ReSendOffsetValue)
            //Hd.CurrProduct?.AcqBd?.ResendAdcOffsetValue();

            #endregion
        }
    }
}
