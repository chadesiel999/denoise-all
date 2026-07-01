using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScopeX.Hardware.Driver
{
    internal class PcieBd_Standard : AbstractPcieBd
    {

        /// <summary>
        /// 完成基本配置
        /// </summary>
        public override void Init()
        {
            #region 通道底板电源关闭
            AbstractController_AnalogChannel.PowerOff();
            Thread.Sleep(20);
            #endregion

            #region 控制采集板、处理板上电(先断电，避免程序闪退时，总线被二级传输占用)
            if (!ComModel.Constants.ENABLE_DEBUG)
            {
                HdIO.WriteReg(PcieBdReg.W.PowerManager_ProcBoard_Power, 0x00);
                HdIO.WriteReg(PcieBdReg.W.PowerManager_AcqBoard_Power, 0x00);
                HdIO.Sleep(1000);
            }
            HdIO.WriteReg(PcieBdReg.W.PowerManager_ProcBoard_Power, 0xff);
            
            //目的是等待时钟板的锁定状态，硬件上设计bug，导致控制逻辑冲突 CIJ标注
            //HdIO.Sleep(45000);
            Stopwatch sw = Stopwatch.StartNew();
            UInt32 clk_locked = 0;
            while (sw.ElapsedMilliseconds < 60000 && clk_locked==0) {
                clk_locked = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_1);
                Thread.Sleep(1000);
            }
            sw.Stop();
            //var clk_locked = HdIO.ReadReg(ProcBdReg.R.reverse_pro_reverse_rd_reg_1);

            ///    HdIO.WriteReg(PcieBdReg.W.PowerManager_AcqBoard_Power, 0xff);
            #endregion

            //二级传输寄存器复位【PCIE】
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_ActionStart, (UInt32)0);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)1);
            HdIO.WriteReg(PcieBdReg.W.Level2Transmit_Reset, (UInt32)0);

            //采集系统传输回路复位成正常模式(非2级传输模式)
            HdIO.WriteReg(PcieBdReg.W.FlashOperator_ActionCode, 0);
            //控制风扇转速
  //          Hd.SystemMonitor.CtrlFanSpeed(3000);
 //           Hd.SystemMonitor.CtrlFanSpeed(3000, false);

            //2024/4/15 上电后需要延时1s,否则配置可能不能写入
            HdIO.Sleep(1000);
            Stopwatch stopwatch = new Stopwatch();
            long waitPower_ms = 10_000;
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                waitPower_ms = 10;
            if (!Hd.CurrProduct.ProcBd?.IsPowerOk ?? false)
            {
                HdIO.WriteReg(PcieBdReg.W.PowerManager_ProcBoard_Power, 0xff);
                stopwatch.Restart();
                while ((!Hd.CurrProduct.ProcBd?.IsPowerOk ?? false) && stopwatch.ElapsedMilliseconds < waitPower_ms)
                    HdIO.Sleep(10);
                HdIO.Sleep(100);

                if (!Hd.CurrProduct.ProcBd?.IsPowerOk ?? false)
                {
                    Hd.SysLogger?.Invoke("处理板上电失败!", "Error");
                }
            }
            ReadFpgaVersion();

            //while (true)
            //{
            //UInt32 data_back;

            //HdIO.WriteReg(ProcBdReg.W.SysInfo_WorkOKTest, 0x55AA);
            //HdIO.Sleep(1);
            //data_back = HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest);
            //data_back = HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest);
            //if (data_back != 0x55AA)
            //{
            //    Hd.SysLogger?.Invoke("处理板寄存器测试失败!", "Error");
            //}
            //HdIO.Sleep(1);

            //HdIO.WriteReg(ProcBdReg.W.SysInfo_WorkOKTest, 0xAA55);
            //HdIO.Sleep(1);
            //data_back = HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest);
            //data_back = HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest);
            //if (data_back != 0xAA55)
            //{
            //    Hd.SysLogger?.Invoke("处理板寄存器测试失败!", "Error");
            //}
            //HdIO.Sleep(1);

            //HdIO.WriteReg((UInt32)AcqBdReg.W.SysInfo_WorkOKTest, 0x55AA);
            //HdIO.Sleep(1);
            //data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest);
            //data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest);
            //if (data_back != 0x55AA)
            //{
            //    Hd.SysLogger?.Invoke("采集板0寄存器测试失败!", "Error");
            //}
            //HdIO.Sleep(1);

            //HdIO.WriteReg((UInt32)AcqBdReg.W.SysInfo_WorkOKTest, 0xAA55);
            //HdIO.Sleep(1);
            //data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest);
            //data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest);
            //if (data_back != 0xAA55)
            //{
            //    Hd.SysLogger?.Invoke("采集板0寄存器测试失败!", "Error");
            //}
            //HdIO.Sleep(1);

            //    HdIO.WriteReg((UInt32)AcqBdReg.W.SysInfo_WorkOKTest | 0x41000, 0x55AA);
            //    HdIO.Sleep(1);
            //    data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest | 0x41000);
            //    data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest | 0x41000);
            //    if (data_back != 0x55AA)
            //    {
            //        Hd.SysLogger?.Invoke("采集板1寄存器测试失败!", "Error");
            //    }
            //    HdIO.Sleep(1);

            //    HdIO.WriteReg((UInt32)AcqBdReg.W.SysInfo_WorkOKTest | 0x41000, 0xAA55);
            //    HdIO.Sleep(1);
            //    data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest | 0x41000);
            //    data_back = HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest | 0x41000);
            //    if (data_back != 0xAA55)
            //    {
            //        Hd.SysLogger?.Invoke("采集板1寄存器测试失败!", "Error");
            //    }
            //    HdIO.Sleep(1);
            //}
            HdIO.WriteReg(ProcBdReg.W.PowerManager_AcqBoardPowerCtrl, 0xff); //采集板上电流程
            HdIO.Sleep(5000);
            //采集板上电检查(如不通过会进行二次上电)
            stopwatch.Restart();
            Hd.CurrProduct?.AcqBd?.ClearPowerOkInfo();
            while ((!Hd.CurrProduct?.AcqBd?.IsAllPowerOk() ?? false) && stopwatch.ElapsedMilliseconds < waitPower_ms)
            {
                HdIO.Sleep(10);
            }
            HdIO.Sleep(100);
            if (!Hd.CurrProduct?.AcqBd?.IsAllPowerOk() ?? false)
            {
                Hd.SysLogger?.Invoke("采集板上电失败!", "Error");
            }
            HdIO.Sleep(10000);
            //此段代码的目的，是软件启动过后，控制机箱风扇给个初始转速，防止机器过热损坏_CIJ
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty, 0x01);//ctrl_en,1
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, (UInt32)(100)); //fan_pwm
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, 0x01);//tx_en,1
            HdIO.Sleep(5);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, 0x00);//tx_en,1
            HdIO.Sleep(1);


            UInt32 StartAt1 = HdIO.ReadReg((UInt32)PcieBdReg.R.FPGAFlashUpdater_UpdateFlag);
            Hd.SysLogger?.Invoke($"PCIE FPGAFlashUpdater_UpdateFlag = {StartAt1.ToString("X4")}", "Info");

            UInt32 StartAt2 = HdIO.ReadReg((UInt32)PcieBdReg.R.FPGAFlashUpdater_UpdateFlag);
            Hd.SysLogger?.Invoke($"PROC FPGAFlashUpdater_UpdateFlag = {StartAt2.ToString("X4")}", "Info");
            HdIO.Sleep(10);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_AdcCardPowerEnable, 0x01);//ADC_power_en 1：ON, 0: OFF
            HdIO.Sleep(10);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x00);//ACQ_FD_power_en 1：ON, 0: OFF

        }

        public override void PowerDown()
        {
            AbstractController_Misc.AllPowerDown();
        }
        /// <summary>
        /// 板内测试
        /// </summary>
        public override void Test()
        {

        }
    }
}