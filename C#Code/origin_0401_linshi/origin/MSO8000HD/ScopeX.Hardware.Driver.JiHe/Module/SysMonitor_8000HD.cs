using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    internal class SysMonitor_8000HD : SystemMonitor
    {
        public SysMonitor_8000HD()
        {
            Type = ProductType.JiHe_MSO8000HD;
            MinFanSpeed = 500;
            MaxFanSpeed = 5000;
        }

        internal override AcqBdNo AcqBdNo => AcqBdNo.B0;
        internal AcqBdNo AcqBdNo1 => AcqBdNo.B1;

        internal override void InitFanSpeed()
        {
            CtrlFanSpeed(1500);
            CtrlCaseFanSpeed(50);
        }

        private void CtrlFanSpeed(UInt32 speed)
        {
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty3, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty4, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty5, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty6, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty7, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty8, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty9, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty10, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty11, speed);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty12, speed);
        }

        /// <summary>
        /// 控制示波器左右侧所有风扇的转速
        /// 即所有风扇转速保持一致
        /// </summary>
        /// <param name="speed">转速，范围[0,4000]RPM</param>
        /// <param name="isRight">是否是右侧风扇</param>
        internal override void CtrlFanSpeed(Double speed, Boolean isRight = true)
        {
            //hc20241009:value取值范围为[0,12500]，对应占空比为[0%,100%],对应转速speed为[0,5000]RPM；
            //风扇占空比10%起转 对应风扇最低转速为500转
            //          speed与value为线性关系，且"value = 2.5 * speed"
            speed = Math.Clamp(speed, (UInt32)MinFanSpeed, (UInt16)MaxFanSpeed);
            UInt32 value = (UInt32)(2.5 * speed);
            if (isRight)
            {
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty3, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty5, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty4, value);
            }
            else
            {
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty6, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty7, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty8, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty9, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty10, value);
                HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty11, value);
            }
        }

        /// <summary>
        /// 控制示波器左右侧单个风扇的转速
        /// 左上角开始、右下角结束 {{1,2},{3,4},{5,6},}
        /// </summary>
        /// <param name="speed">转速，范围[0,4000]RPM</param>
        /// <param name="fanIndex">风扇编号</param>
        /// <param name="isRight">是否是右侧风扇</param>
        internal override void CtrlFanSpeed(Double speed, Int32 fanIndex, Boolean isRight = true)
        {
            var index = 0;
            index = Math.Clamp(fanIndex, 0, 5);
            var register = PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1;//对应 1
            if (isRight)
            {
                register = index switch
                {
                    ///Cpu
                    0 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1,//对应 1
                    1 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty,//对应 2

                    ///Pcie
                    2 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty5,//对应 3
                    3 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2,//对应 4

                    ///Chnl
                    4 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty3,//对应 5
                    5 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty4,//对应 6

                    _ => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1,
                };
            }
            else
            {
                register = index switch
                {
                    0 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty6,//对应  1
                    1 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty7,//对应  2
                    2 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty11,//对应  3
                    3 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty10,//对应  4
                    4 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty9,//对应 5
                    5 => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty8,//对应 6

                    _ => PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty6,
                };
            }
            //hc20241009:value取值范围为[0,12500]，对应占空比为[0%,100%],对应转速speed为[0,5000]RPM；
            //风扇占空比10%起转 对应风扇最低转速为500转
            //          speed与value为线性关系，且"value = 2.5 * speed"
            speed = Math.Clamp(speed, (UInt32)MinFanSpeed, (UInt16)MaxFanSpeed);
            UInt32 value = (UInt32)(2.5 * speed);
            CtrlFanSpeed(register, value);
        }

        internal override void CtrlFanSpeed(PcieBdReg.W register, UInt32 speed)
        {
            //hc20241009:value取值范围为[0,12500]，对应占空比为[0%,100%],对应转速speed为[0,5000]RPM；
            //风扇占空比10%起转 对应风扇最低转速为500转
            //          speed与value为线性关系，且"value = 2.5 * speed"
            speed = Math.Clamp(speed, (UInt32)MinFanSpeed, (UInt16)MaxFanSpeed);
            UInt32 value = (UInt32)(2.5 * speed);
            HdIO.WriteReg(register, value);
        }

        private static void CtrlCaseFanSpeed(Int32 speed)
        {
            //HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegFanCtrlPwmRst, 0x02);//复位控制 FanCtrlPwm_RegFanCtrlPwmRst
            //HdIO.WriteReg(PcieBdReg.W.FanCtrlPwm_RegPwmThreshold, (UInt32)(speed * 100));//风扇转速控制 10%:0X03E8 ，20%:0X07D0， 30%：0x0BB8， 40%：0x0FA0，50%：0x1388，60%：0x1770，70%：0x1B58，80%：0x1F40，90%：0x2328, 100%：0x2710
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty, 0x01);//上位机单片机风扇控制使能，上位机接入有效位至1
            //HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, (UInt32)(speed));//上位机风扇转速控制，0~100
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, (UInt32)(100));//上位机风扇转速控制，0~100
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, 0x01);//uart发送使能状态,有效位至1
            HdIO.Sleep(10);
            HdIO.WriteReg(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, 0x00);//uart发送使能状态,有效位至1


            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_5, 0x02);
            Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_2, (uint)speed*100);
        }
        internal override String Read()
        {
            StringBuilder stringbuilder = new StringBuilder();
            #region Pcie board
            String boardName = "PcieBoard:";
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            Thread.Sleep(5);
            UInt32 data = 0;
            Double temprature = 0.0;

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
            temprature = (data * 501.3743 / 1024) - 273.15;
            stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_temp={temprature.ToString("0.00")}℃");
            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccaux);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccaux={temprature.ToString()}");

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccbram);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccbram={temprature.ToString()}");

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccint);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccint={temprature.ToString()}");

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data);
            temprature = data * 0.0625;
            stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data={temprature.ToString("0.00")}℃");

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_pro);
            temprature = data * 0.0625;
            stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data_pro={temprature.ToString("0.00")}℃");

            ReadAnalogChannelTemperatures();
            stringbuilder.AppendLine($"{boardName}PhyChannelTemperatures={AnalogChannelTemperatures[0].ToString()}");
            stringbuilder.AppendLine($"{boardName}PhyChannelTemperatures_8G={AnalogChannelTemperatures[1].ToString()}");

            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            #endregion
            stringbuilder.AppendLine("======================================");

            #region S6Board

            #endregion
            stringbuilder.AppendLine("======================================");
            #region ProcessBoard
            boardName = "ProcBoard:";
            HdIO.WriteReg(ProcBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(ProcBdReg.W.SysMon_pro_sysmon_rst, 1);
            Thread.Sleep(5);
            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Temperature);
            data &= 0xfff;
            temprature = (int)data;
            temprature = (temprature * 503.975 / 1024) - 273.15;
            stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_temp={temprature.ToString("0.00")}℃");

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccaux);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccaux={temprature.ToString()}");

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccbram);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccbram={temprature.ToString()}");

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccint);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccint={temprature.ToString()}");

            //HdIO.WriteReg(ProcBdReg.W.SysMon_pro_sysmon_rst, 0);
            #endregion
            stringbuilder.AppendLine("======================================");
            #region AcqBoard
            for (var acqBdIndex = 0; acqBdIndex < Hd.CurrProduct!.AcqBd!.ExistsDefines.Count; acqBdIndex++)
            {
                if (Hd.CurrProduct!.AcqBd!.ExistsDefines[acqBdIndex].ISENABLE)
                {
                    AcqBdNo acbdno = (AcqBdNo)acqBdIndex;
                    boardName = "AcqBoard_" + acbdno.ToString() + ":";

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = (temprature * 501.3743 / 1024) - 273.6777;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_temp={temprature.ToString("0.00")}℃");
                    //data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_Ct_1820_Driver_Temp_Data, acqBdNo);
                    //temprature = data * 0.0625;
                    //StringBuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data={temprature.ToString("0.00")}℃");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccaux, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccaux={temprature.ToString()}");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccbram, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccbram={temprature.ToString()}");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccint, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccint={temprature.ToString()}");
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc1, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_adc1_temperature={temprature.ToString("0.00")}℃");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_adc2_temperature={temprature.ToString("0.00")}℃");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb1, acbdno);
                    data &= 0x7FF;
                    temprature = (int)data * 0.125;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb1_temperature={temprature.ToString("0.00")}℃");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb2_temperature={temprature.ToString("0.00")}℃");
                }
            }
            #endregion
            return stringbuilder.ToString();
        }

        /// <summary>
        /// PCIE_FPGA系统监视器片上温度
        /// </summary>
        /// <returns></returns>
        internal override List<(String Description, Double Temperature)> GetPcieFpgaTemperatureBymCelsius()
        {
            var temprature = 0.0;
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            Thread.Sleep(5);
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
            temprature = (data * 501.3743 / 1024) - 273.15;
            temprature *= 1000;
            temprature = Math.Round(temprature);

            return new List<(String Description, Double Temperature)>() { ("Pcie_Fpga", temprature) };
        }

        /// <summary>
        /// PCIE_Board CT1820温度传感器板上温度
        /// </summary>
        /// <returns></returns>
        internal override List<(String Description, Double Temperature)> GetPcieBoardTemperatureBymCelsius()
        {
            var temprature = 0.0;
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data);
            temprature = data * 0.0625;
            temprature *= 1000;

            return new List<(String Description, Double Temperature)>() { ("Pcie_Board", temprature) };
        }

        internal List<(String Description, Double Temperature)> GetProFpgaTemperatureBymCelsius()
        {
            var temprature = 0.0;
            var data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Temperature);
            data &= 0xfff;
            temprature = (int)data;
            temprature = (temprature * 503.975 / 1024) - 273.15;
            temprature *= 1000;
            temprature = Math.Round(temprature);

            return new List<(String Description, Double Temperature)>() { ("Pro_Fpga", temprature) };
        }

        /// <summary>
        /// PRO_Board CT1820温度传感器板上温度
        /// </summary>
        /// <returns></returns>
        internal List<(String Description, Double Temperature)> GetProBoardTemperatureBymCelsius()
        {
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_pro);
            var temprature = data * 0.0625;
            temprature *= 1000;
            return new List<(String Description, Double Temperature)>() { ("Pro_Board", temprature) };
        }

        internal override List<(String Description, Double Temperature)> GetAcqFpgaTemperatureBymCelsius(AcqBdNo acqBdNo)
        {
            var temprature1 = 0.0;
            var temprature2 = 0.0;
            var data = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, AcqBdNo.B0);
            data &= 0x3ff;
            temprature1 = (int)data;
            temprature1 = (temprature1 * 501.3743 / 1024) - 273.6777;
            temprature1 *= 1000;
            temprature1 = Math.Round(temprature1);

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, AcqBdNo.B1);
            data &= 0x3ff;
            temprature2 = (int)data;
            temprature2 = (temprature2 * 501.3743 / 1024) - 273.6777;
            temprature2 *= 1000;
            temprature2 = Math.Round(temprature2);

            return new List<(String Description, Double Temperature)>() { ("Acq_Fpga1", temprature1), ("Acq_Fpga2", temprature2) };

            //List<(String Description, Double Temperature)> trmplist = new();
            //for (int i = 0; i < (int)AcqBdNo.B7; i++)
            //{
            //    var temprature = 0.0;
            //    uint data = 0;
            //    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, (AcqBdNo)i);
            //    data &= 0x3ff;
            //    temprature = (int)data;
            //    temprature = (temprature * 501.3743 / 1024) - 273.6777;
            //    temprature *= 1000;
            //    temprature = Math.Round(temprature);
            //    trmplist.Add(("Acq_Fpga" + i.ToString(), temprature));
            //}
            //return trmplist;
        }

        internal override List<(String Description, Double Temperature)> GetAcqBoardTemperatureBymCelsius(AcqBdNo acqBdNo)
        {
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_acq1);
            var temprature1 = data * 0.0625;
            temprature1 *= 1000;

            data = 0;
            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_acq2);
            var temprature2 = data * 0.0625;
            temprature2 *= 1000;

            return new List<(String Description, Double Temperature)>() { ("Acq_Board1", temprature1), ("Acq_Board2", temprature2) };
        }

        /// <summary>
        /// ACQ1_FPGA系统监视器片上温度
        /// ACQ2_FPGA系统监视器片上温度
        /// </summary>
        /// <returns></returns>
        internal override List<(String Description, Double Temperature)> GetAcqFpgaTemperatureBymCelsius()
        {
            var temprature1 = 0.0;
            var temprature2 = 0.0;
            var data = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, AcqBdNo);
            data &= 0x3ff;
            temprature1 = (int)data;
            temprature1 = (temprature1 * 501.3743 / 1024) - 273.6777;
            temprature1 *= 1000;
            temprature1 = Math.Round(temprature1);

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, AcqBdNo1);
            data &= 0x3ff;
            temprature2 = (int)data;
            temprature2 = (temprature2 * 501.3743 / 1024) - 273.6777;
            temprature2 *= 1000;
            temprature2 = Math.Round(temprature2);

            return new List<(String Description, Double Temperature)>() { ("Acq_Fpga1", temprature1), ("Acq_Fpga2", temprature2) };

            //List<(String Description, Double Temperature)> trmplist = new();
            //for (int i = 0; i < (int)AcqBdNo.B7; i++)
            //{
            //    var temprature = 0.0;
            //    uint data = 0;
            //    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, (AcqBdNo)i);
            //    data &= 0x3ff;
            //    temprature = (int)data;
            //    temprature = (temprature * 501.3743 / 1024) - 273.6777;
            //    temprature *= 1000;
            //    temprature = Math.Round(temprature);
            //    trmplist.Add(("Acq_Fpga" + i.ToString(), temprature));
            //}
            //return trmplist;
        }

        /// <summary>
        /// ACQ1_Board CT1820温度传感器板上温度
        /// ACQ2_Board CT1820温度传感器板上温度
        /// </summary>
        /// <returns></returns>
        internal override List<(String Description, Double Temperature)> GetAcqBoardTemperatureBymCelsius()
        {
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_acq1);
            var temprature1 = data * 0.0625;
            temprature1 *= 1000;

            data = 0;
            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_acq2);
            var temprature2 = data * 0.0625;
            temprature2 *= 1000;
            return new List<(String Description, Double Temperature)>() { ("Acq_Board1", temprature1), ("Acq_Board2", temprature2) };
        }

        /// <summary>
        /// ADC_Board_R CT1820温度传感器板上温度
        /// ADC_Board_M CT1820温度传感器板上温度
        /// </summary>
        /// <returns></returns>
        internal List<(String Description, Double Temperature)> GetADCBoardTemperatureBymCelsius()
        {
            var data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_adc_r);
            var temprature1 = data * 0.0625;
            temprature1 *= 1000;

            data = 0;
            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_adc_m);
            var temprature2 = data * 0.0625;
            temprature2 *= 1000;
            return new List<(String Description, Double Temperature)>() { ("Adc_Board_R", temprature1), ("Adc_Board_M", temprature2) };
        }

        /// <summary>
        /// ACQ1_ADC1 LM95233温度传感器ADC1片上温度
        /// ACQ1_ADC2 LM95233温度传感器ADC2片上温度
        /// ACQ2_ADC3 LM95233温度传感器ADC3片上温度
        /// ACQ2_ADC4 LM95233温度传感器ADC4片上温度
        /// </summary>
        internal List<(String Description, Double Temperature)> GetADCChipsTemperatureBymCelsius()
        {
            var list = new List<(String Description, Double Temperature)>();
            var data = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc1, AcqBdNo);
            data &= 0x7FF;
            var chip1 = (int)data * 0.125;
            chip1 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc2, AcqBdNo);
            data &= 0x7FF;
            var chip2 = (int)data * 0.125;
            chip2 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc1, AcqBdNo1);
            data &= 0x7FF;
            var chip3 = (int)data * 0.125;
            chip3 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc2, AcqBdNo1);
            data &= 0x7FF;
            var chip4 = (int)data * 0.125;
            chip4 *= 1000;

            list.Add(("Adc1_Chip", chip1));
            list.Add(("Adc2_Chip", chip2));
            list.Add(("Adc3_Chip", chip3));
            list.Add(("Adc4_Chip", chip4));
            return list;
        }

        /// <summary>
        /// ACQ1_ADC1_Board LM95233温度传感器ADC1板上温度
        /// ACQ1_ADC2_Board LM95233温度传感器ADC2板上温度
        /// ACQ2_ADC3_Board LM95233温度传感器ADC3板上温度
        /// ACQ2_ADC4_Board LM95233温度传感器ADC4板上温度
        /// </summary>
        /// <returns></returns>
        internal List<(String Description, Double Temperature)> GetADCBoradsTemperatureBymCelsius()
        {
            var list = new List<(String Description, Double Temperature)>();
            var data = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb1, AcqBdNo);
            data &= 0x7FF;
            var chip1 = (int)data * 0.125;
            chip1 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb2, AcqBdNo);
            data &= 0x7FF;
            var chip2 = (int)data * 0.125;
            chip2 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb1, AcqBdNo1);
            data &= 0x7FF;
            var chip3 = (int)data * 0.125;
            chip3 *= 1000;

            data = 0;
            data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb2, AcqBdNo1);
            data &= 0x7FF;
            var chip4 = (int)data * 0.125;
            chip4 *= 1000;

            list.Add(("Adc1_Board", chip1));
            list.Add(("Adc2_Board", chip2));
            list.Add(("Adc3_Board", chip3));
            list.Add(("Adc4_Board", chip4));
            return list;
        }

        internal override String GetChannelTemperaturesByCelsius()
        {
            var tempstr = String.Empty;
            foreach (var chnlid in ChannelIdExt.GetAnalogs())
            {
                var isreadsuccess = CtrlAnalogChannel_JiHe2d5G.AllChannelTemperatures.TryGetValue(chnlid, out var tempvalue);
                var temp = isreadsuccess ? tempvalue : InvalidTemperature;
                tempstr += $"{temp},";
            }
            return tempstr;
        }

        public override Double CtrollerPcieFanSpeedTempertuare => _HddTemperature;

        internal Double _Pcie_Fpag_TempBymCelsius = 0.0;
        internal Double _Pcie_Board_TempBymCelsius = 0.0;

        internal Double _Pro_Fpga_TempBymCelsius = 0.0;
        internal Double _Pro_Board_TempBymCelsius = 0.0;

        internal Double _Acq1_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq2_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq3_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq4_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq5_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq6_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq7_Fpag_TempBymCelsius = 0.0;
        internal Double _Acq8_Fpag_TempBymCelsius = 0.0;


        internal Double _Adc_Board_R_TempBymCelsius = 0.0;
        internal Double _Adc_Board_M_TempBymCelsius = 0.0;

        internal Double _Adc1_Chip_TempBymCelsius = 0.0;
        internal Double _Adc2_Chip_TempBymCelsius = 0.0;
        internal Double _Adc3_Chip_TempBymCelsius = 0.0;
        internal Double _Adc4_Chip_TempBymCelsius = 0.0;
        internal Double _Adc5_Chip_TempBymCelsius = 0.0;
        internal Double _Adc6_Chip_TempBymCelsius = 0.0;
        internal Double _Adc7_Chip_TempBymCelsius = 0.0;
        internal Double _Adc8_Chip_TempBymCelsius = 0.0;
        internal Double _Adc9_Chip_TempBymCelsius = 0.0;
        internal Double _Adc10_Chip_TempBymCelsius = 0.0;
        internal Double _Adc11_Chip_TempBymCelsius = 0.0;
        internal Double _Adc12_Chip_TempBymCelsius = 0.0;
        internal Double _Adc13_Chip_TempBymCelsius = 0.0;
        internal Double _Adc14_Chip_TempBymCelsius = 0.0;
        internal Double _Adc15_Chip_TempBymCelsius = 0.0;
        internal Double _Adc16_Chip_TempBymCelsius = 0.0;

        internal Double _Acq1_Board_TempBymCelsius = 0.0;
        internal Double _Acq2_Board_TempBymCelsius = 0.0;
        internal Double _Acq3_Board_TempBymCelsius = 0.0;
        internal Double _Acq4_Board_TempBymCelsius = 0.0;
        internal Double _Acq5_Board_TempBymCelsius = 0.0;
        internal Double _Acq6_Board_TempBymCelsius = 0.0;
        internal Double _Acq7_Board_TempBymCelsius = 0.0;
        internal Double _Acq8_Board_TempBymCelsius = 0.0;

        internal Double _Adc1_Board_TempBymCelsius = 0.0;
        internal Double _Adc2_Board_TempBymCelsius = 0.0;
        internal Double _Adc3_Board_TempBymCelsius = 0.0;
        internal Double _Adc4_Board_TempBymCelsius = 0.0;
        internal Double _Adc5_Board_TempBymCelsius = 0.0;
        internal Double _Adc6_Board_TempBymCelsius = 0.0;
        internal Double _Adc7_Board_TempBymCelsius = 0.0;
        internal Double _Adc8_Board_TempBymCelsius = 0.0;
        internal Double _Adc9_Board_TempBymCelsius = 0.0;
        internal Double _Adc10_Board_TempBymCelsius = 0.0;
        internal Double _Adc11_Board_TempBymCelsius = 0.0;
        internal Double _Adc12_Board_TempBymCelsius = 0.0;
        internal Double _Adc13_Board_TempBymCelsius = 0.0;
        internal Double _Adc14_Board_TempBymCelsius = 0.0;
        internal Double _Adc15_Board_TempBymCelsius = 0.0;
        internal Double _Adc16_Board_TempBymCelsius = 0.0;

        public override List<(String Description, Double Temperature)> GetAllTemperature()
        {

            var alltemps = new List<(String Description, Double Temperature)>
            {
                ("CH1", _Ch1Temperature),
                ("CH2", _Ch2Temperature),
                ("CH3", _Ch3Temperature),
                ("CH4", _Ch4Temperature),

                ("Pcie_FPGA", _Pcie_Fpag_TempBymCelsius / 1000D),
                ("Pcie_Board", _Pcie_Board_TempBymCelsius / 1000D),

                ("Pro_FPGA", _Pro_Fpga_TempBymCelsius / 1000D),
                ("Pro_Board", _Pro_Board_TempBymCelsius / 1000D),


                ("Acq1_FPGA", _Acq1_Fpag_TempBymCelsius / 1000D),
                ("Acq2_FPGA", _Acq2_Fpag_TempBymCelsius / 1000D),

                ("Acq1_Board", _Acq1_Board_TempBymCelsius / 1000D),
                ("Acq2_Board", _Acq2_Board_TempBymCelsius / 1000D),

                ("Adc_Board_R", _Adc_Board_R_TempBymCelsius / 1000D),
                ("Adc_Board_M", _Adc_Board_M_TempBymCelsius / 1000D),

                ("Adc1_Chip", _Adc1_Chip_TempBymCelsius / 1000D),
                ("Adc2_Chip", _Adc2_Chip_TempBymCelsius / 1000D),
                ("Adc3_Chip", _Adc3_Chip_TempBymCelsius / 1000D),
                ("Adc4_Chip", _Adc4_Chip_TempBymCelsius / 1000D),

                ("Adc1_Board", _Adc1_Board_TempBymCelsius / 1000D),
                ("Adc2_Board", _Adc2_Board_TempBymCelsius / 1000D),
                ("Adc3_Board", _Adc3_Board_TempBymCelsius / 1000D),
                ("Adc4_Board", _Adc4_Board_TempBymCelsius / 1000D),

                //("HDD", HddTemperature)
            };

            return alltemps;
        }

        public override List<(String Description, Int32 Speed)> GetAllFanSpeed()
        {
            var alltemps = new List<(String Description, Int32 Temperature)>
            {
                ("Chnl Speed", _ChnlFanSpeed),
                ("机箱两侧", ChannelFanSpeed),
                ("Pcie Speed", PcieSpeed),
                ("Pro Speed", _ProFanSpeed),
                //("Acq1 Speed", _Acq1FanSpeed),
                //("Acq2 Speed", _Acq2FanSpeed),
                //("Adc12 Speed", _Adc12FanSpeed),
                ("Adc34 Speed", _Adc34FanSpeed),
            };

            return alltemps;
        }

        internal override void SetSysTemperatures()
        {
            var temp = GetPcieFpgaTemperatureBymCelsius();
            Volatile.Write(ref _Pcie_Fpag_TempBymCelsius, temp.FirstOrDefault().Temperature);
            var pcieboard = GetPcieBoardTemperatureBymCelsius();
            Volatile.Write(ref _Pcie_Board_TempBymCelsius, pcieboard.FirstOrDefault().Temperature);

            temp = GetProFpgaTemperatureBymCelsius();
            Volatile.Write(ref _Pro_Fpga_TempBymCelsius, temp.FirstOrDefault().Temperature);
            temp = GetProBoardTemperatureBymCelsius();
            Volatile.Write(ref _Pro_Board_TempBymCelsius, temp.FirstOrDefault().Temperature);


            var twotemp = GetAcqFpgaTemperatureBymCelsius();
            Volatile.Write(ref _Acq1_Fpag_TempBymCelsius, twotemp[0].Temperature);
            Volatile.Write(ref _Acq2_Fpag_TempBymCelsius, twotemp[1].Temperature);
            

            twotemp = GetAcqBoardTemperatureBymCelsius();
            Volatile.Write(ref _Acq1_Board_TempBymCelsius, twotemp[0].Temperature);
            Volatile.Write(ref _Acq2_Board_TempBymCelsius, twotemp[1].Temperature);
            Volatile.Write(ref _Adc3_Board_TempBymCelsius, twotemp[2].Temperature);
            Volatile.Write(ref _Adc4_Board_TempBymCelsius, twotemp[3].Temperature);
        

            var adctemp = GetADCBoardTemperatureBymCelsius();
            Volatile.Write(ref _Adc_Board_R_TempBymCelsius, adctemp[0].Temperature);
            Volatile.Write(ref _Adc_Board_M_TempBymCelsius, adctemp[1].Temperature);


            var list = GetADCChipsTemperatureBymCelsius();
            Volatile.Write(ref _Adc1_Chip_TempBymCelsius, list[0].Temperature);
            Volatile.Write(ref _Adc2_Chip_TempBymCelsius, list[1].Temperature);
            Volatile.Write(ref _Adc3_Chip_TempBymCelsius, list[2].Temperature);
            Volatile.Write(ref _Adc4_Chip_TempBymCelsius, list[3].Temperature);
         

            list = GetADCBoradsTemperatureBymCelsius();
            Volatile.Write(ref _Adc1_Board_TempBymCelsius, list[0].Temperature);
            Volatile.Write(ref _Adc2_Board_TempBymCelsius, list[1].Temperature);
      

            var channeltemperature = CtrlAnalogChannel_JiHe2d5G.AllChannelTemperatures.TryGetValue(ChannelId.C1, out var value1);
            var channeltemp = channeltemperature ? value1 : 0.0D;
            Volatile.Write(ref _Ch1Temperature, channeltemp);

            channeltemp = 0.0;
            channeltemperature = CtrlAnalogChannel_JiHe2d5G.AllChannelTemperatures.TryGetValue(ChannelId.C2, out var value2);
            channeltemp = channeltemperature ? value2 : 0.0D;
            Volatile.Write(ref _Ch2Temperature, channeltemp);

            channeltemp = 0.0;
            channeltemperature = CtrlAnalogChannel_JiHe2d5G.AllChannelTemperatures.TryGetValue(ChannelId.C3, out var value3);
            channeltemp = channeltemperature ? value3 : 0.0D;
            Volatile.Write(ref _Ch3Temperature, channeltemp);

            channeltemp = 0.0;
            channeltemperature = CtrlAnalogChannel_JiHe2d5G.AllChannelTemperatures.TryGetValue(ChannelId.C4, out var value4);
            channeltemp = channeltemperature ? value4 : 0.0D;
            Volatile.Write(ref _Ch4Temperature, channeltemp);
        }

        private volatile Int32 _Acq1FanSpeed;
        private volatile Int32 _Acq2FanSpeed;
        private volatile Int32 _ProFanSpeed;
        private volatile Int32 _Adc12FanSpeed;
        private volatile Int32 _Adc34FanSpeed;
        private volatile Int32 _ChnlFanSpeed;

        private PIDContoller _Acq1FanController = new() { Kp = 500, Ki = 30, Kd = 0 };
        private PIDContoller _Acq2FanController = new() { Kp = 500, Ki = 30, Kd = 0 };
        private PIDContoller _ProFanController = new() { Kp = 500, Ki = 30, Kd = 0 };
        private PIDContoller _Adc12FanController = new() { Kp = 500, Ki = 30, Kd = 0 };
        private PIDContoller _Adc34FanController = new() { Kp = 500, Ki = 30, Kd = 0 };
        private PIDContoller _ChnlController = new() { Kp = 500, Ki = 30, Kd = 0 };


        private Double GetMaxTemperature()
        {
            var alltemps = new List<Double>
            {
                _Ch1Temperature,
                _Ch2Temperature,
                _Ch3Temperature,
                _Ch4Temperature,

                _Pcie_Fpag_TempBymCelsius / 1000D,

                _Pro_Fpga_TempBymCelsius / 1000D,


                _Acq1_Fpag_TempBymCelsius / 1000D,
                _Acq2_Fpag_TempBymCelsius / 1000D,

                _Adc1_Chip_TempBymCelsius / 1000D,
                _Adc2_Chip_TempBymCelsius / 1000D,
                _Adc3_Chip_TempBymCelsius / 1000D,
                _Adc4_Chip_TempBymCelsius / 1000D,
            };
            return alltemps.Max();
        }
        internal double GetMaxTemp()
        {
            List<double> temps = new();
            #region Pcie board
            String boardName = "PcieBoard:";
            HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 1);
            Thread.Sleep(5);
            UInt32 data = 0;
            Double temprature = 0.0;

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Temperature);
            temprature = (data * 501.3743 / 1024) - 273.15;
            //stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_temp={temprature.ToString("0.00")}℃");
            temps.Add(temprature);
            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccaux);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccaux={temprature.ToString()}");
            temps.Add(temprature);

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccbram);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccbram={temprature.ToString()}");
            temps.Add(temprature);

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Sysmon_Vccint);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pcie_fpga_vccint={temprature.ToString()}");
            temps.Add(temprature);

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data);
            temprature = data * 0.0625;
            //stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data={temprature.ToString("0.00")}℃");
            temps.Add(temprature);

            data = HdIO.ReadReg(PcieBdReg.R.SysMon_Ct_1820_Driver_Temp_Data_pro);
            temprature = data * 0.0625;
            //stringbuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data_pro={temprature.ToString("0.00")}℃");
            temps.Add(temprature);

            ReadAnalogChannelTemperatures();
            //stringbuilder.AppendLine($"{boardName}PhyChannelTemperatures={AnalogChannelTemperatures[0].ToString()}");
            temps.Add(AnalogChannelTemperatures[0]);
            temps.Add(AnalogChannelTemperatures[1]);
            //stringbuilder.AppendLine($"{boardName}PhyChannelTemperatures_8G={AnalogChannelTemperatures[1].ToString()}");

            //HdIO.WriteReg(PcieBdReg.W.SysMon_Sysmon_Rst, 0);
            #endregion
            //stringbuilder.AppendLine("======================================");

            #region S6Board

            #endregion
            //stringbuilder.AppendLine("======================================");
            #region ProcessBoard
            boardName = "ProcBoard:";
            HdIO.WriteReg(ProcBdReg.W.SysMon_Sysmon_Rst, 0);
            //HdIO.WriteReg(ProcBdReg.W.SysMon_pro_sysmon_rst, 1);
            Thread.Sleep(5);
            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Temperature);
            data &= 0xfff;
            temprature = (int)data;
            temprature = (temprature * 503.975 / 1024) - 273.15;
            //stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_temp={temprature.ToString("0.00")}℃");
            temps.Add(temprature);

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccaux);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccaux={temprature.ToString()}");
            temps.Add(temprature);

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccbram);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccbram={temprature.ToString()}");
            temps.Add(temprature);

            data = HdIO.ReadReg(ProcBdReg.R.SysMon_Sysmon_Vccint);
            data &= 0xfff;
            temprature = (int)data;
            temprature = temprature / 1024 * 3;
            //stringbuilder.AppendLine($"{boardName}SysMon_pro_fpga_vccint={temprature.ToString()}");
            temps.Add(temprature);

            //HdIO.WriteReg(ProcBdReg.W.SysMon_pro_sysmon_rst, 0);
            #endregion
            //stringbuilder.AppendLine("======================================");
            #region AcqBoard
            for (var acqBdIndex = 0; acqBdIndex < Hd.CurrProduct!.AcqBd!.ExistsDefines.Count; acqBdIndex++)
            {
                if (Hd.CurrProduct!.AcqBd!.ExistsDefines[acqBdIndex].ISENABLE)
                {
                    AcqBdNo acbdno = (AcqBdNo)acqBdIndex;
                    boardName = "AcqBoard_" + acbdno.ToString() + ":";

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_temp, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = (temprature * 501.3743 / 1024) - 273.6777;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_temp={temprature.ToString("0.00")}℃");
                    temps.Add(temprature);
                    //data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_Ct_1820_Driver_Temp_Data, acqBdNo);
                    //temprature = data * 0.0625;
                    //StringBuilder.AppendLine($"{boardName}SysMon_Ct_1820_Driver_Temp_Data={temprature.ToString("0.00")}℃");

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccaux, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccaux={temprature.ToString()}");
                    temps.Add(temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccbram, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccbram={temprature.ToString()}");
                    temps.Add(temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_fpga_vccint, acbdno);
                    data &= 0x3ff;
                    temprature = (Int32)data;
                    temprature = temprature / 1024 * 3;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_fpga_vccint={temprature.ToString()}");
                    temps.Add(temprature);
                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc1, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_adc1_temperature={temprature.ToString("0.00")}℃");
                    temps.Add(temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_adc2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_adc2_temperature={temprature.ToString("0.00")}℃");
                    temps.Add(temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb1, acbdno);
                    data &= 0x7FF;
                    temprature = (int)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb1_temperature={temprature.ToString("0.00")}℃");
                    temps.Add(temprature);

                    data = Hd.CurrProduct!.AcqBd.ReadReg(AcqBdReg.R.SysMon_acq_temp_pcb2, acbdno);
                    data &= 0x7FF;
                    temprature = (Int32)data * 0.125;
                    //stringbuilder.AppendLine($"{boardName}SysMon_acq_pcb2_temperature={temprature.ToString("0.00")}℃");
                    temps.Add(temprature);
                }
            }
            #endregion
            //return stringbuilder.ToString();
            return temps.Max();
        }
        internal override void AutoFanControl()
        {
            //if (!Hd.CurrDebugVarints.bEnableAutoFanControl)
            //{
            //    return;
            //}
            //if (UsingUIParam)
            //{
            //    if (DateTime.Now.AddMilliseconds(0 - HoldoffBymsAutoFanControl) >= LastCtrlFanTimestamp)
            //    {
            //        LastCtrlFanTimestamp = DateTime.Now;
            //        if (HDDMonitor.Default == null)
            //        {
            //            HDDMonitor.Default = new HDDMonitor();
            //        }
            //        HDDMonitor.Default.SendMsg();
            //        SetSysTemperatures();

            //        CtrlFanSpeed(ChannelFanSpeed, 4);
            //        CtrlFanSpeed(ChannelFanSpeed, 5);
            //        CtrlFanSpeed(ChannelFanSpeed, 4, false);
            //        CtrlFanSpeed(ChannelFanSpeed, 5, false);

            //        CtrlFanSpeed(PcieSpeed, 2);
            //        CtrlFanSpeed(PcieSpeed, 3);
            //        CtrlFanSpeed(PcieSpeed, 2, false);
            //        CtrlFanSpeed(PcieSpeed, 3, false);

            //        CtrlFanSpeed(CpuSpeed, 0);
            //        CtrlFanSpeed(CpuSpeed, 1);
            //        CtrlFanSpeed(CpuSpeed, 0, false);
            //        CtrlFanSpeed(CpuSpeed, 1, false);
            //    }
            //    return;
            //}

            if (DateTime.Now.AddMilliseconds(0 - HoldoffBymsAutoFanControl) >= _LastCtrlFanTimestamp)
            {
                _LastCtrlFanTimestamp = DateTime.Now;

                double max = GetMaxTemp();
                int speed = (int)(0.5 * max )+ 55;
                speed = speed > 100 ? 100 : speed;
                speed = speed < 50 ? 50 : speed;
                CtrlCaseFanSpeed(speed);


                //ReadAnalogChannelTemperatures();

                //if (AnalogChannelTemperatures[0] > InvalidTemperature)
                //{
                //    Hd.SysLogger?.Invoke("Temperature reading error", "Debug");
                //    AnalogChannelTemperatures[0] = Constants.ANALOGCHANNEL_WORKING_TEMPERATURE;
                //}

                //var chnlspeed = _ChnlController.CaculateOriginal(AnalogChannelTemperatures[0], Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 800, 6500, -2500, 2500, 2500);
                //_ChnlFanSpeed = (Int32)chnlspeed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty10, (UInt32)_ChnlFanSpeed, 6500, 800); //V3硬件通道的风扇

                ////to do,何晨要调整此处
                ////机箱左右两侧风扇采用CH1,CH2,CH3,CH4,Adc1_Chip,Adc2_Chip,Adc3_Chip,Adc4_Chip,Acq1_FPGA,,Acq2_FPGA,Pro_FPGA,Pcie_FPGA温度最大值调节
                //var maxtemp = GetMaxTemperature();
                //var speed = _ChannelPID.CaculateOriginal(maxtemp, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 800, 4000, -1600, 1600, 1600);
                ////if (maxtemp < 70)
                ////{
                ////    speed = 2500;
                ////}
                //ChannelFanSpeed = (Int32)speed;
                //ChannelFanSpeed = (Int32)2000;
                ////机箱两侧风扇最大转速4000转 最小800转 对于占空比 100% - 30%
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty11, (UInt32)ChannelFanSpeed, 4000, 800);//右侧上风扇
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty, (UInt32)ChannelFanSpeed, 4000, 800);//右侧下风扇
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty12, (UInt32)ChannelFanSpeed, 4000, 800);//左侧上风扇
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty1, (UInt32)ChannelFanSpeed, 4000, 800);//左侧下风扇

                ////Pcie内嵌风扇最大4000转 静音风扇可全速转
                //speed = 4000;// _PciePID.Caculate(Pcie_Fpag_TempBymCelsius / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 800, 4000, -1600, 1600, 1600);
                //PcieSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty8, (UInt32)speed, 4000U, 800U);//Pcie风扇

                //SetSysTemperatures();

                ////Pro内嵌风扇最大4000转  静音风扇可全速转
                //speed = 4000;// _ProFanController.CaculateOriginal(Pro_Fpga_TempBymCelsius / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 800, 4000, -1600, 1600, 1600);
                //_ProFanSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty4, (UInt32)_ProFanSpeed, 4000U, 800U);//Pro风扇

                //speed = _Acq1FanController.CaculateOriginal(_Acq1_Fpag_TempBymCelsius / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, MinFanSpeed, MaxFanSpeed, -2500, 2500, 2000);
                //_Acq1FanSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty9, (UInt32)_Acq1FanSpeed);//Acq1风扇


                //speed = _Acq2FanController.CaculateOriginal(_Acq2_Fpag_TempBymCelsius / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, MinFanSpeed, MaxFanSpeed, -2500, 2500, 2000);
                //_Acq2FanSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty5, (UInt32)_Acq2FanSpeed);//Acq2风扇

                ////Adc风扇最大转速12000转 最小3000转 对于占空比 100% - 30%
                //speed = _Adc12FanController.CaculateOriginal((_Adc1_Chip_TempBymCelsius + _Adc2_Chip_TempBymCelsius) / 2 / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 3000U, 12000U, -6000, 6000, 600);
                //_Adc12FanSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty6, (UInt32)_Adc12FanSpeed, 12000U, 3000U);//Adc1风扇
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty7, (UInt32)_Adc12FanSpeed, 12000U, 3000U);//Adc2风扇


                //speed = _Adc34FanController.CaculateOriginal((_Adc3_Chip_TempBymCelsius + _Adc4_Chip_TempBymCelsius) / 2 / 1000D, Constants.ANALOGCHANNEL_WORKING_TEMPERATURE, 3000U, 12000U, -60000, 6000, 6000);
                //_Adc34FanSpeed = (Int32)speed;
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty2, (UInt32)_Adc34FanSpeed, 12000U, 3000U);//Adc3风扇
                //CtrlFanSpeed(PcieBdReg.W.CaseFanCtrl_Case_Fan_PWM_Duty3, (UInt32)_Adc34FanSpeed, 12000U, 3000U);//Adc4风扇
            }
        }
    }
}
