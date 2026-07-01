using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.Unicode;
using System.Text.Json;
using System.Text.Encodings.Web;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;
using ScopeX.Hardware.Calibration.Data.Base;
using System.ComponentModel;
namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 各个项目此板差异较大。主要是ADC的配置、插入的槽号不同。与运行态的改变有关。有各种系数的发送有关。
    /// 1、ADC 输入端口的动态改变。
    /// 2、ADC 的Phase、Gain的动态变化
    /// 3、校准滤波器系数的动态变化
    /// </summary>
    internal abstract class AbstractAcqBd : IFpga
    {
        public AbstractAcqBd((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)[] existsConfig)
        {
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) bExists in existsConfig)
                ExistsDefines.Add(bExists);
        }
        /// <summary>
        /// 理论上插入的采集板
        /// </summary>
        internal List<AcqBdNo> ExistsAcqBdDefine = new();

        /// <summary>
        /// 通过上电检测的采集板
        /// </summary>
        internal List<AcqBdNo> PowerOkBoardList = new();
        #region 各个采集板掩码及插入定义
        internal static readonly UInt32[] AcqFpgaAddrMarkTable =
        {
            0x40000, //B0-ACQ1
      		0x41000, //B1-ACQ2
      		0x42000, //B2-ACQ3
      		0x43000, //B3-ACQ4
      		0x44000,//B4-ACQ5
      		0x45000, //B5-ACQ6 
            0x46000,//B6-ACQ7 
      		0x47000, //B7-ACQ8
            0x48000, //B8-ACQ9 
      		0x49000, //B9-ACQ10
            0x4A000, //B10-ACQ11 
            0x4B000, //B11-ACQ12
        };
        internal List<(String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)> ExistsDefines = new List<(String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)>();
        #endregion

        internal const int FPGATotalCount = 10;//DBI20G 项目做多10块采集板
        //protected List<int> PowerOkBoardList = new List<int>();
        public virtual void Init() { }
        public virtual void ClearSendHistory()
        {
            AdcAlreadySendDataManager.Default.ClearSendHistory();
            Hd.CurrProduct?.Acquirer_AnalogChannel?.ClearSendHistory();
        }
        public void ClearPowerOkInfo() => PowerOkBoardList.Clear();

        public virtual void Test() { }
        public int ExistAcqBoardCount
        {
            get
            {
                int count = 0;
                foreach (var v in ExistsDefines)
                {
                    if (v.ISENABLE)
                        count++;
                }
                return count;
            }
        }
        protected int FindAdcSignalInputPort(int channelID, int fpgaIndex, int adcIndex)
        {
            return 1;//Port A
        }
        #region 上电检查控制
        public virtual bool IsAllPowerOk()//????
        {
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                return true;
            int boardIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exist in ExistsDefines)
            {
                if (exist.ISENABLE)
                {
                    AcqBdNo acqBdNo = (AcqBdNo)boardIndex;
                    //if (!ChecckPowerOk(acqBdNo))
                    //    return false;
                    //else if (!PowerOkBoardList.Contains(boardIndex))
                    //    PowerOkBoardList.Add(boardIndex);
                    int checkCount = 0;
                    ///已经完成上电可能状态未返回，需要多抓几次状态
                    while (checkCount < 5)
                    {
                        if (ChecckPowerOk(acqBdNo))
                        {
                            if (!PowerOkBoardList.Contains((AcqBdNo)boardIndex))
                                PowerOkBoardList.Add((AcqBdNo)boardIndex);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(300);
                            checkCount++;
                        }
                    }
                    //if (checkCount >= 5)
                    //{
                    //    return false;
                    //}

                }
                boardIndex++;
            }

            //if (boardIndex == ((int[])Enum.GetValues(typeof(AcqBdNo))).Max())
            //{
            //    return false;
            //}

            return true;


            //return false;
        }
        private bool ChecckPowerOk(AcqBdNo acqBdNo)
        {
#if !Product_B21_JinHui_PXI

            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
                return true;
            int fpgaIndex = (int)acqBdNo;
            if (ExistsDefines[fpgaIndex].ISENABLE)
            {
                UInt32 data = 0x5a5a;
                HdIO.WriteReg((UInt32)AcqBdReg.W.SysInfo_WorkOKTest | AcqFpgaAddrMarkTable[fpgaIndex], data);
                HdIO.WriteReg(0x8000 | AcqFpgaAddrMarkTable[fpgaIndex], 0);
                HdIO.Sleep(1);
                if (HdIO.ReadReg((UInt32)AcqBdReg.R.SysInfo_WorkOKTest | AcqFpgaAddrMarkTable[fpgaIndex]) != data)
                    return false;
                return true;
            }
#endif
            return false;
        }
        public virtual bool MiscFunc(string FuncName)
        {
            return false;
        }

        public virtual bool IsPowerOk(AcqBdNo acqBdNo)//????
        {
            return PowerOkBoardList.Contains(acqBdNo);

            //return false;
        }
        #endregion

#pragma warning disable CS8766 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
        public virtual FpgaVersion? FpgaVersion { get; set; } = null;
#pragma warning restore CS8766 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
        public virtual FpgaVersion? GetFpgaVersion(int boardIndex)
        {
            if (boardIndex < ExistsDefines.Count)
            {
                return fpgaVersions[boardIndex];
            }
            else
                return null;
        }
        public virtual string GetRegMonitorResult()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            Type writeEnumType = typeof(AcqBdReg.W);
            Array writeRegs = Enum.GetValues(writeEnumType);
            Type readEnumType = typeof(AcqBdReg.R);
            Array readRegs = Enum.GetValues(typeof(AcqBdReg.R));
            int boardIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exists in ExistsDefines)
            {
                if (exists.ISENABLE)
                {
                    stringBuilder.AppendLine(("AcqBoard" + (boardIndex + 1).ToString()).PadRight(60, '='));
                    foreach (AcqBdReg.W reg in writeRegs)
                    {
                        WriteReg(AcqBdReg.W.RegMonitor_RegAddress, (AcqBdNo)boardIndex, (UInt32)((UInt32)reg | AcqFpgaAddrMarkTable[(int)boardIndex]));
                        UInt32 readBackData = ReadReg(AcqBdReg.R.RegMonitor_ReadbackValue, (AcqBdNo)boardIndex);
                        string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(writeEnumType, reg.ToString());
                        stringBuilder.AppendLine("B" + (boardIndex + 1).ToString() + (",[W],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("X"));
                    }
                    foreach (AcqBdReg.R reg in readRegs)
                    {
                        UInt32 readBackData = ReadReg(reg, (AcqBdNo)boardIndex);
                        string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(readEnumType, reg.ToString());
                        stringBuilder.AppendLine("B" + (boardIndex + 1).ToString() + (",[R] ,Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("X"));
                    }
                }
                boardIndex++;
            }
            resultStr = stringBuilder.ToString();
            return resultStr;
        }

        public virtual bool BoardExists(int boardIndex) //????
        {
            bool iscontains=PowerOkBoardList.Contains((AcqBdNo)boardIndex);

            return iscontains;
        }
        public virtual CaliDataType ChangedCaliDataType { get; set; }
        protected FpgaVersion[] fpgaVersions = new FpgaVersion[FPGATotalCount];
        public virtual void CtrlFineGain(ChannelId channelIndex) { }
        public virtual void TiAdc_ApplayAdc_SyncSampleClock() { }
        public virtual void AdjustAdc_Gain(bool widthAdcCS, int channelID, AcqBdNo fpgaIndex, int adcIndex) { }
        public virtual void AdjustAdc_Phase(bool withAdcCS, int channelID, AcqBdNo fpgaIndex, int adcIndex) { }
        public virtual string ReadADC5200SyncWindowRegValue() => "";
        public virtual void TiAdc_ApplyAdc_Phase_Offset_Gain() { }
        #region FpgaVerion
        public virtual string AllFpgaVerionStr
        {
            get
            {
                string result = "";
                foreach (int boardIndex in PowerOkBoardList)
                {
                    if (result != "")
                        result = result + ",";
                    result = result + "{" + (boardIndex + 1).ToString() + ":" + fpgaVersions[boardIndex].ToString() + "}";
                }
                return result;
            }
        }

        public virtual void ReadFpgaVersion()
        {
            for (int boardIndex = 0; boardIndex < FPGATotalCount; boardIndex++)
            {
                if (IsPowerOk((AcqBdNo)boardIndex))
                {
                    fpgaVersions[boardIndex] = new FpgaVersion();
                    UInt32 mark = AcqFpgaAddrMarkTable[boardIndex];
                    FpgaVersionRegs acqBoardRegs = new FpgaVersionRegs(
                            (UInt32)AcqBdReg.R.VersionInfo1_CompileTimeWord0| mark, (UInt32)AcqBdReg.R.VersionInfo1_CompileTimeWord1 | mark,
                            (UInt32)AcqBdReg.R.VersionInfo_VersionWord0 | mark, (UInt32)AcqBdReg.R.VersionInfo_VersionWord1 | mark,
                            new UInt32[] { (UInt32)AcqBdReg.R.VersionInfo_DesignerWord0 | mark, (UInt32)AcqBdReg.R.VersionInfo_DesignerWord1 | mark, (UInt32)AcqBdReg.R.VersionInfo_DesignerWord2 | mark, (UInt32)AcqBdReg.R.VersionInfo_DesignerWord3 | mark },
                            new UInt32[] {
                            (UInt32)AcqBdReg.R.VersionInfo_CommentWord0 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord1 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord2 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord3 | mark,
                            (UInt32)AcqBdReg.R.VersionInfo_CommentWord4 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord5 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord6 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord7 | mark,
                            (UInt32)AcqBdReg.R.VersionInfo_CommentWord8 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord9 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord10 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord11 | mark,
                            (UInt32)AcqBdReg.R.VersionInfo_CommentWord12 | mark, (UInt32)AcqBdReg.R.VersionInfo_CommentWord13 | mark, (UInt32)AcqBdReg.R.VersionInfo1_CommentWord14| mark, (UInt32)AcqBdReg.R.VersionInfo1_CommentWord15 | mark
                    });
                    FpgaVersion.ReadFPGAVersion(acqBoardRegs, ref fpgaVersions[boardIndex]);
                }
            }
        }
        #endregion

        #region PllWrite
        //protected void PllWrite(AcqBdNo fpgaIndex, UInt32 addr, UInt32 data)
        //{
        //    UInt32 temp = ((0x000 << 21) | (addr << 8) | data);

        //    WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
        //    WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_L16, fpgaIndex, temp & 0xffff);
        //    WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_H8, fpgaIndex, (temp >> 16) & 0xff);
        //    WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 1);
        //    HdIO.WaitForSpiTransfer(1, 4);
        //    HdIO.Sleep(1);
        //    WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
        //    HdIO.Sleep(1);
        //}
        //protected void PllAllWrite(UInt32 addr, UInt32 data)
        //{
        //    UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
        //    foreach (AcqBdNo fpgaIndex in Enum.GetValues(typeof(AcqBdNo)))
        //    {
        //        WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
        //        WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_L16, fpgaIndex, temp & 0xffff);
        //        WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_H8, fpgaIndex, (temp >> 16) & 0xff);
        //        WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 1);
        //    }
        //    HdIO.WaitForSpiTransfer(1, 4);//并行等待法。
        //    HdIO.Sleep(1);
        //    foreach (AcqBdNo fpgaIndex in Enum.GetValues(typeof(AcqBdNo)))
        //    {
        //        WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
        //    }
        //    HdIO.Sleep(1);
        //}
        #endregion

        #region 寄存器读写基础函数

        public void WriteReg(UInt32 regAddr, AcqBdNo fpgaIndex, UInt32 data)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                HdIO.AppendLineMessage?.Invoke("write to Acq Board[" + fpgaIndex.ToString() + "]& ,address=0x" + regAddr.ToString("X").PadLeft(4, '0') + ",data=0x" + data.ToString("X").PadLeft(4, '0'));
                HdIO.WriteReg(regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex], data);
            }
        }
        public void WriteReg(AcqBdReg.W regAddrLow, AcqBdReg.W regAddrHigh, AcqBdNo fpgaIndex, UInt32 data)
        {
            WriteReg(regAddrLow, fpgaIndex, data & 0xffff);
            WriteReg(regAddrHigh, fpgaIndex, (data >> 16) & 0xffff);
        }
        public void WriteReg(UInt32 regAddr, AcqBdNo fpgaIndex, UInt32 data, int delayUs)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                HdIO.AppendLineMessage?.Invoke("write to Acq Board[" + fpgaIndex.ToString() + "]& ,address=0x" + regAddr.ToString("X").PadLeft(4, '0') + ",data=0x" + data.ToString("X").PadLeft(4, '0') + ",delayUs=" + delayUs.ToString());

                HdIO.WriteReg(regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex], data, delayUs);
            }
        }
        public void WriteReg(AcqBdReg.W regAddr, AcqBdNo fpgaIndex, UInt32 data)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                HdIO.AppendLineMessage?.Invoke("write to Acq Board[" + fpgaIndex.ToString() + "]& ,name=" + regAddr.ToString() + ", address=" + HdIO.GetFPGARegisterDescription?.Invoke(typeof(AcqBdReg.W), regAddr.ToString()) + ",data=0x" + data.ToString("X").PadLeft(4, '0'));

                HdIO.WriteReg((uint)regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex], data);
            }
        }
        public void WriteReg(AcqBdReg.W regAddr, AcqBdNo fpgaIndex, UInt32 data, int delayUs)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                HdIO.AppendLineMessage?.Invoke("write to Acq Board[" + fpgaIndex.ToString() + "]& ,name=" + regAddr.ToString() + ", address=" + HdIO.GetFPGARegisterDescription?.Invoke(typeof(AcqBdReg.W), regAddr.ToString()) + ",data=0x" + data.ToString("X").PadLeft(4, '0') + ",delayUs=" + delayUs.ToString());
                HdIO.WriteReg((uint)regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex], data, delayUs);
            }
        }
        public UInt32 ReadReg(AcqBdReg.R regAddr, AcqBdNo fpgaIndex)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                UInt32 data = HdIO.ReadRegNoDebug((UInt32)regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex]);

                HdIO.AppendLineMessage?.Invoke("read from Acq Board[" + fpgaIndex.ToString() + "]& ,name=" + regAddr.ToString() + ", address=" + HdIO.GetFPGARegisterDescription?.Invoke(typeof(AcqBdReg.R), regAddr.ToString()) + ",value=0x" + data.ToString("X").PadLeft(4, '0'));

                return data;
            }
            else
                return 0;
        }

        public UInt32 ReadReg(UInt32 regAddr, AcqBdNo fpgaIndex)
        {
            if ((int)fpgaIndex < FPGATotalCount && BoardExists((int)fpgaIndex))
            {
                UInt32 data = HdIO.ReadRegNoDebug(regAddr | AcqFpgaAddrMarkTable[(int)fpgaIndex]);

                HdIO.AppendLineMessage?.Invoke("read from Acq Board[" + fpgaIndex.ToString() + "]& ,address=" + regAddr.ToString("X").PadLeft(4, '0') + ",readback=0x" + data.ToString("X").PadLeft(4, '0'));
                return data;
            }
            else
                return 0;
        }
        public void WriteToAllFpga(UInt32 regAddr, UInt32 data)
        {
            foreach (int fpgaIndex in PowerOkBoardList)
                HdIO.WriteReg(regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
        }
        public void WriteToAllFpga(UInt32 regAddr, UInt32 data, int delayUs)
        {
            foreach (int fpgaIndex in PowerOkBoardList)
            {
                HdIO.WriteReg(regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
                HdIO.DelayByUs(delayUs);
            }
        }
        public void WriteToAllFpga(AcqBdReg.W regAddr, UInt32 data)
        {
            //foreach (int fpgaIndex in PowerOkBoardList)
            //先直接发,现在上电标志不对;
            for (int fpgaIndex = 0; fpgaIndex < AcqFpgaAddrMarkTable.Length; fpgaIndex++)
            {
                HdIO.WriteReg((UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
            }
        }

        public void WriteToAllFpgaForRst(AcqBdReg.W regAddr, UInt32 data)
        {
            //foreach (int fpgaIndex in PowerOkBoardList)
            //先直接发,现在上电标志不对;

            for (int fpgaIndex = 0; fpgaIndex < AcqFpgaAddrMarkTable.Length; fpgaIndex++)
            {
                HdIO.WriteReg((UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);

                while ((fpgaIndex == 6) | (fpgaIndex == 4))
                {
                    WriteReg(AcqBdReg.W.RegMonitor_RegAddress, (AcqBdNo)fpgaIndex, (UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex]);
                    var regReadBackValue = ReadReg(AcqBdReg.R.RegMonitor_ReadbackValue, (AcqBdNo)fpgaIndex);

                    if (regReadBackValue != data)
                    {
                        HdIO.WriteReg((UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public UInt32 UartTest(AcqBdReg.W regAddr, UInt32 testLong)
        {
            UInt32 data = 0;
            UInt32 errTimes = 0;
            UInt32 correctTimes = 0;
            UInt32 fpgaIndex = 4;
            for (UInt32 testTimes = 0; testTimes < testLong; testTimes++)
            {

                HdIO.WriteReg((UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
                WriteReg(AcqBdReg.W.RegMonitor_RegAddress, (AcqBdNo)fpgaIndex, (UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex]);
                var regReadBackValue = ReadReg(AcqBdReg.R.RegMonitor_ReadbackValue, (AcqBdNo)fpgaIndex);

                if (regReadBackValue != data)
                {
                    errTimes = errTimes + 1;
                }
                else
                {
                    correctTimes = correctTimes + 1;
                }

                data = data + 1;
            }
            return errTimes;
        }

        public UInt32 UartTest2(AcqBdReg.W regAddr, UInt32 testLong)
        {
            UInt32 errTimes = 0;
            UInt32 errTimes2 = 0;
            for (UInt32 testTimes = 0; testTimes < testLong; testTimes++)
            {
                errTimes2 = UartTest(AcqBdReg.W.Decimation_PosGapValueH16, 60_000);
                errTimes = errTimes + errTimes2;
            }

            return errTimes;
        }

        public void WriteToAllFpga(AcqBdReg.W regAddr, UInt32 data, int delayUs)
        {
            foreach (int fpgaIndex in PowerOkBoardList)
            {
                HdIO.WriteReg((UInt32)regAddr | AcqFpgaAddrMarkTable[fpgaIndex], data);
                HdIO.DelayByUs(delayUs);
            }
        }
        public void WriteToAllFpga(AcqBdReg.W regAddrLow, AcqBdReg.W regAddrHigh, UInt32 data)
        {
            WriteToAllFpga(regAddrLow, data & 0xffff);
            WriteToAllFpga(regAddrHigh, (data >> 16) & 0xffff);
        }
        #endregion

        public virtual void ResendAdcOffsetValue() { }
        internal virtual void Wait5200AdcAutoCaliFinished(List<FPGAAdcPair>? fpgaAdcPairList)
        {
            if (fpgaAdcPairList == null)
                return;

        }
        #region 各种系数表数据的下发
#region Interpolation

#region IFIR
#if IFIR
        private static readonly UInt32[] _IFirCoe =
        {
            //0x00014,
            //0x1FFDA,
            //0x1FFCB,
            //0x1FFAF,
            //0x1FF88,
            //0x1FF5A,
            //0x1FF22,
            //0x1FEE5,
            //0x1FEA6,
            //0x1FE61,
            //0x1FE21,
            //0x1FDE4,
            //0x1FDB1,
            //0x1FD8B,
            //0x1FD77,
            //0x1FD77,
            //0x1FD8E,
            //0x1FDBB,
            //0x1FE00,
            //0x1FE5E,
            //0x1FECE,
            //0x1FF4B,
            //0x1FFD5,
            //0x0005C,
            //0x000E0,
            //0x00155,
            //0x001B6,
            //0x001F8,
            //0x00219,
            //0x00217,
            //0x001EE,
            //0x0019F,
            //0x0012F,
            //0x000A3,
            //0x00008,
            //0x1FF62,
            //0x1FEBF,
            //0x1FE2E,
            //0x1FDB6,
            //0x1FD65,
            //0x1FD3F,
            //0x1FD4E,
            //0x1FD90,
            //0x1FE03,
            //0x1FEA3,
            //0x1FF67,
            //0x00042,
            //0x00122,
            //0x001FB,
            //0x002B7,
            //0x0034D,
            //0x003A9,
            //0x003C8,
            //0x0039C,
            //0x0032A,
            //0x00272,
            //0x00183,
            //0x00068,
            //0x1FF37,
            //0x1FE05,
            //0x1FCE6,
            //0x1FBF4,
            //0x1FB44,
            //0x1FAE3,
            //0x1FAE1,
            //0x1FB44,
            //0x1FC06,
            //0x1FD20,
            //0x1FE82,
            //0x00012,
            //0x001B8,
            //0x00355,
            //0x004C6,
            //0x005EE,
            //0x006B4,
            //0x00700,
            //0x006CB,
            //0x0060C,
            //0x004CE,
            //0x00322,
            //0x00122,
            //0x1FEF0,
            //0x1FCB3,
            //0x1FA97,
            //0x1F8CA,
            //0x1F76D,
            //0x1F6A9,
            //0x1F68D,
            //0x1F72B,
            //0x1F87E,
            //0x1FA73,
            //0x1FCF5,
            //0x1FFD5,
            //0x002E0,
            //0x005DE,
            //0x00895,
            //0x00ACB,
            //0x00C4E,
            //0x00CF6,
            //0x00CA7,
            //0x00B5C,
            //0x0091F,
            //0x0060C,
            //0x00254,
            //0x1FE33,
            //0x1F9F6,
            //0x1F5ED,
            //0x1F26A,
            //0x1EFBD,
            //0x1EE23,
            //0x1EDCF,
            //0x1EEDD,
            //0x1F152,
            //0x1F512,
            //0x1F9EF,
            //0x1FF9A,
            //0x005B5,
            //0x00BD3,
            //0x0117A,
            //0x01636,
            //0x01995,
            //0x01B3B,
            //0x01AE7,
            //0x01870,
            //0x013DA,
            //0x00D4A,
            //0x00513,
            //0x1FBAA,
            //0x1F1A3,
            //0x1E7B1,
            //0x1DE92,
            //0x1D70D,
            //0x1D1E1,
            //0x1CFB9,
            //0x1D123,
            //0x1D684,
            //0x1E013,
            //0x1EDCD,
            //0x1FF77,
            //0x0149E,
            //0x02C99,
            //0x04694,
            //0x0618F,
            //0x07C79,
            //0x09637,
            //0x0ADAD,
            //0x0C1D9,
            //0x0D1D2,
            //0x0DCE4,
            //0x0E28F,
            //0x0E28F,
            //0x0DCE4,
            //0x0D1D2,
            //0x0C1D9,
            //0x0ADAD,
            //0x09637,
            //0x07C79,
            //0x0618F,
            //0x04694,
            //0x02C99,
            //0x0149E,
            //0x1FF77,
            //0x1EDCD,
            //0x1E013,
            //0x1D684,
            //0x1D123,
            //0x1CFB9,
            //0x1D1E1,
            //0x1D70D,
            //0x1DE92,
            //0x1E7B1,
            //0x1F1A3,
            //0x1FBAA,
            //0x00513,
            //0x00D4A,
            //0x013DA,
            //0x01870,
            //0x01AE7,
            //0x01B3B,
            //0x01995,
            //0x01636,
            //0x0117A,
            //0x00BD3,
            //0x005B5,
            //0x1FF9A,
            //0x1F9EF,
            //0x1F512,
            //0x1F152,
            //0x1EEDD,
            //0x1EDCF,
            //0x1EE23,
            //0x1EFBD,
            //0x1F26A,
            //0x1F5ED,
            //0x1F9F6,
            //0x1FE33,
            //0x00254,
            //0x0060C,
            //0x0091F,
            //0x00B5C,
            //0x00CA7,
            //0x00CF6,
            //0x00C4E,
            //0x00ACB,
            //0x00895,
            //0x005DE,
            //0x002E0,
            //0x1FFD5,
            //0x1FCF5,
            //0x1FA73,
            //0x1F87E,
            //0x1F72B,
            //0x1F68D,
            //0x1F6A9,
            //0x1F76D,
            //0x1F8CA,
            //0x1FA97,
            //0x1FCB3,
            //0x1FEF0,
            //0x00122,
            //0x00322,
            //0x004CE,
            //0x0060C,
            //0x006CB,
            //0x00700,
            //0x006B4,
            //0x005EE,
            //0x004C6,
            //0x00355,
            //0x001B8,
            //0x00012,
            //0x1FE82,
            //0x1FD20,
            //0x1FC06,
            //0x1FB44,
            //0x1FAE1,
            //0x1FAE3,
            //0x1FB44,
            //0x1FBF4,
            //0x1FCE6,
            //0x1FE05,
            //0x1FF37,
            //0x00068,
            //0x00183,
            //0x00272,
            //0x0032A,
            //0x0039C,
            //0x003C8,
            //0x003A9,
            //0x0034D,
            //0x002B7,
            //0x001FB,
            //0x00122,
            //0x00042,
            //0x1FF67,
            //0x1FEA3,
            //0x1FE03,
            //0x1FD90,
            //0x1FD4E,
            //0x1FD3F,
            //0x1FD65,
            //0x1FDB6,
            //0x1FE2E,
            //0x1FEBF,
            //0x1FF62,
            //0x00008,
            //0x000A3,
            //0x0012F,
            //0x0019F,
            //0x001EE,
            //0x00217,
            //0x00219,
            //0x001F8,
            //0x001B6,
            //0x00155,
            //0x000E0,
            //0x0005C,
            //0x1FFD5,
            //0x1FF4B,
            //0x1FECE,
            //0x1FE5E,
            //0x1FE00,
            //0x1FDBB,
            //0x1FD8E,
            //0x1FD77,
            //0x1FD77,
            //0x1FD8B,
            //0x1FDB1,
            //0x1FDE4,
            //0x1FE21,
            //0x1FE61,
            //0x1FEA6,
            //0x1FEE5,
            //0x1FF22,
            //0x1FF5A,
            //0x1FF88,
            //0x1FFAF,
            //0x1FFCB,
            //0x1FFDA,
            //0x00014,
            //0x0007C,
            //0x000B2,
            //0x0012D,
            //0x001D7,
            //0x002BD,
            //0x003E9,
            //0x00566,
            //0x0073C,
            //0x00978,
            //0x00C1F,
            //0x00F38,
            //0x012C5,
            //0x016C8,
            //0x01B3C,
            //0x0201C,
            //0x0255B,
            //0x02AEA,
            //0x030B8,
            //0x036B0,
            //0x03CB8,
            //0x042B4,
            //0x04889,
            //0x04E19,
            //0x05345,
            //0x057F2,
            //0x05C05,
            //0x05F66,
            //0x06202,
            //0x063C7,
            //0x064AD,
            //0x064AD,
            //0x063C7,
            //0x06202,
            //0x05F66,
            //0x05C05,
            //0x057F2,
            //0x05345,
            //0x04E19,
            //0x04889,
            //0x042B4,
            //0x03CB8,
            //0x036B0,
            //0x030B8,
            //0x02AEA,
            //0x0255B,
            //0x0201C,
            //0x01B3C,
            //0x016C8,
            //0x012C5,
            //0x00F38,
            //0x00C1F,
            //0x00978,
            //0x0073C,
            //0x00566,
            //0x003E9,
            //0x002BD,
            //0x001D7,
            //0x0012D,
            //0x000B2,
            //0x0007C
//100dB
            0x0000A,
            0x00014,
            0x0001E,
            0x00028,
            0x00046,
            0x00064,
            0x0008C,
            0x000B4,
            0x000F0,
            0x0012C,
            0x00172,
            0x001B8,
            0x00208,
            0x0024E,
            0x00294,
            0x002D0,
            0x002F8,
            0x0030C,
            0x00302,
            0x002DA,
            0x0028A,
            0x0021C,
            0x0017C,
            0x000B4,
            0x1FFC4,
            0x1FEAC,
            0x1FD80,
            0x1FC36,
            0x1FAEC,
            0x1F9A2,
            0x1F862,
            0x1F740,
            0x1F650,
            0x1F592,
            0x1F51A,
            0x1F4E8,
            0x1F510,
            0x1F588,
            0x1F650,
            0x1F768,
            0x1F8BC,
            0x1FA42,
            0x1FBE6,
            0x1FD94,
            0x1FF38,
            0x000BE,
            0x00212,
            0x00316,
            0x003CA,
            0x00424,
            0x00410,
            0x00398,
            0x002C6,
            0x001AE,
            0x00050,
            0x1FED4,
            0x1FD44,
            0x1FBD2,
            0x1FA7E,
            0x1F97A,
            0x1F8D0,
            0x1F88A,
            0x1F8BC,
            0x1F95C,
            0x1FA74,
            0x1FBE6,
            0x1FDA8,
            0x1FF92,
            0x00190,
            0x0037A,
            0x00532,
            0x00690,
            0x00780,
            0x007E4,
            0x007BC,
            0x006F4,
            0x005AA,
            0x003DE,
            0x001AE,
            0x1FF4C,
            0x1FCCC,
            0x1FA74,
            0x1F858,
            0x1F6AA,
            0x1F59C,
            0x1F538,
            0x1F592,
            0x1F6AA,
            0x1F880,
            0x1FAF6,
            0x1FDDA,
            0x00118,
            0x00456,
            0x00776,
            0x00A32,
            0x00C4E,
            0x00DA2,
            0x00E10,
            0x00D70,
            0x00BD6,
            0x00942,
            0x005DC,
            0x001E0,
            0x1FD80,
            0x1F90C,
            0x1F4DE,
            0x1F146,
            0x1EE80,
            0x1ECDC,
            0x1EC82,
            0x1ED9A,
            0x1F010,
            0x1F3DA,
            0x1F8C6,
            0x1FE98,
            0x004E2,
            0x00B40,
            0x0113A,
            0x01658,
            0x01A40,
            0x01C7A,
            0x01CC0,
            0x01AF4,
            0x016F8,
            0x010F4,
            0x00924,
            0x1FFEC,
            0x1F5D8,
            0x1EB7E,
            0x1E19C,
            0x1D8F0,
            0x1D242,
            0x1CE32,
            0x1CD6A,
            0x1D062,
            0x1D76A,
            0x1E2AA,
            0x1F1FA,
            0x0051E,
            0x01B94,
            0x034A8,
            0x04F7E,
            0x06B26,
            0x08688,
            0x0A08C,
            0x0B824,
            0x0CC60,
            0x0DC5A,
            0x0E768,
            0x0ED08,
            0x0ED08,
            0x0E768,
            0x0DC5A,
            0x0CC60,
            0x0B824,
            0x0A08C,
            0x08688,
            0x06B26,
            0x04F7E,
            0x034A8,
            0x01B94,
            0x0051E,
            0x1F1FA,
            0x1E2AA,
            0x1D76A,
            0x1D062,
            0x1CD6A,
            0x1CE32,
            0x1D242,
            0x1D8F0,
            0x1E19C,
            0x1EB7E,
            0x1F5D8,
            0x1FFEC,
            0x00924,
            0x010F4,
            0x016F8,
            0x01AF4,
            0x01CC0,
            0x01C7A,
            0x01A40,
            0x01658,
            0x0113A,
            0x00B40,
            0x004E2,
            0x1FE98,
            0x1F8C6,
            0x1F3DA,
            0x1F010,
            0x1ED9A,
            0x1EC82,
            0x1ECDC,
            0x1EE80,
            0x1F146,
            0x1F4DE,
            0x1F90C,
            0x1FD80,
            0x001E0,
            0x005DC,
            0x00942,
            0x00BD6,
            0x00D70,
            0x00E10,
            0x00DA2,
            0x00C4E,
            0x00A32,
            0x00776,
            0x00456,
            0x00118,
            0x1FDDA,
            0x1FAF6,
            0x1F880,
            0x1F6AA,
            0x1F592,
            0x1F538,
            0x1F59C,
            0x1F6AA,
            0x1F858,
            0x1FA74,
            0x1FCCC,
            0x1FF4C,
            0x001AE,
            0x003DE,
            0x005AA,
            0x006F4,
            0x007BC,
            0x007E4,
            0x00780,
            0x00690,
            0x00532,
            0x0037A,
            0x00190,
            0x1FF92,
            0x1FDA8,
            0x1FBE6,
            0x1FA74,
            0x1F95C,
            0x1F8BC,
            0x1F88A,
            0x1F8D0,
            0x1F97A,
            0x1FA7E,
            0x1FBD2,
            0x1FD44,
            0x1FED4,
            0x00050,
            0x001AE,
            0x002C6,
            0x00398,
            0x00410,
            0x00424,
            0x003CA,
            0x00316,
            0x00212,
            0x000BE,
            0x1FF38,
            0x1FD94,
            0x1FBE6,
            0x1FA42,
            0x1F8BC,
            0x1F768,
            0x1F650,
            0x1F588,
            0x1F510,
            0x1F4E8,
            0x1F51A,
            0x1F592,
            0x1F650,
            0x1F740,
            0x1F862,
            0x1F9A2,
            0x1FAEC,
            0x1FC36,
            0x1FD80,
            0x1FEAC,
            0x1FFC4,
            0x000B4,
            0x0017C,
            0x0021C,
            0x0028A,
            0x002DA,
            0x00302,
            0x0030C,
            0x002F8,
            0x002D0,
            0x00294,
            0x0024E,
            0x00208,
            0x001B8,
            0x00172,
            0x0012C,
            0x000F0,
            0x000B4,
            0x0008C,
            0x00064,
            0x00046,
            0x00028,
            0x0001E,
            0x00014,
            0x0000A,
            0x00078,
            0x000B4,
            0x0012C,
            0x001D6,
            0x002BC,
            0x003E8,
            0x00564,
            0x0073A,
            0x00974,
            0x00C1C,
            0x00F3C,
            0x012CA,
            0x016C6,
            0x01B3A,
            0x0201C,
            0x02558,
            0x02AEE,
            0x030B6,
            0x036B0,
            0x03CB4,
            0x042B8,
            0x0488A,
            0x04E16,
            0x05348,
            0x057EE,
            0x05C08,
            0x05F64,
            0x06202,
            0x063C4,
            0x064AA,
            0x064AA,
            0x063C4,
            0x06202,
            0x05F64,
            0x05C08,
            0x057EE,
            0x05348,
            0x04E16,
            0x0488A,
            0x042B8,
            0x03CB4,
            0x036B0,
            0x030B6,
            0x02AEE,
            0x02558,
            0x0201C,
            0x01B3A,
            0x016C6,
            0x012CA,
            0x00F3C,
            0x00C1C,
            0x00974,
            0x0073A,
            0x00564,
            0x003E8,
            0x002BC,
            0x001D6,
            0x0012C,
            0x000B4,
            0x00078

        };

        public virtual void ConfigIFIRCoefficients()
        {
            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableReset, 1);
            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableReset, 0);


            uint[] coefficientsData = _IFirCoe;


            for (int j = 0; j < coefficientsData.Length; j++)
            {
                UInt32 CoeData = (UInt32)coefficientsData[j];
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 0);
                if (j < 300)
                    WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteAddr, (UInt32)j);
                else
                    WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteAddr, (UInt32)(j | 0x8000));
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteData_H, (UInt32)(CoeData >> 16) & 0x1);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteData_L16, (UInt32)CoeData & 0xFFFF);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 1);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 0);
            }

        }
#endif
#endregion
        internal virtual bool SendCoefficientsByDMAMode_Interpolation(CoefficientsTableType coefficientsTableType)
        {
            //预设 各个通道的插值系数是一样的
            //Byte[] Coefficients = CoefficientsTables.Default.SerializeByFpgaFormat(coefficientsTableType, 0);
            //for (int fpgaIndex = 0; fpgaIndex < FPGATotalCount; fpgaIndex++)
            //{
            //    if (BoardExists((int)fpgaIndex))
            //    {
            //        bool bOk = HdIO.DMAWrite(0, Coefficients, (UInt32)Coefficients.Length);
            //    }
            //}
            return true;
        }
        /// <summary>
        /// IFC的下发，每个通道的系数需要下发到哪块采集板，还需要进一步调试（pengbo 2024/08/30）
        /// </summary>
        /// <param name="coefficientsTableType"></param>
        /// <param name="bForce"></param>
        /// <returns></returns>
        internal virtual bool SendCoefficientsByRegisterMode_IFC(bool bForce)
        {
            return true;
        }

        internal virtual bool SendCoefficientsByRegisterMode_IFC_Delay(bool bForce)
        {
            return true;
        }
        internal virtual bool SendCoefficientsByRegisterMode_ADCTI(bool bForce)
        {
            return true;
        }

        internal virtual bool SendCoefficientsByRegisterMode_Interpolation(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            //预设 各个通道的插值系数是一样的
            Int32[] Coefficients = CoefficientsTables.Default[coefficientsTableType, 0];
            #region 方法A：每个板单独发送
            for (int fpgaIndex = 0; fpgaIndex < ExistsDefines.Count; fpgaIndex++)
            {
                if (ExistsDefines[fpgaIndex].ISENABLE)
                {
                    HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteEnable | AcqFpgaAddrMarkTable[fpgaIndex], 0);
                    HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableReset | AcqFpgaAddrMarkTable[fpgaIndex], 0);
                    HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableReset | AcqFpgaAddrMarkTable[fpgaIndex], 1);
                    HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableReset | AcqFpgaAddrMarkTable[fpgaIndex], 0);

                    HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteEnable , 0);
                    HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableReset, 0x00); //仿照下面代码，无需选择片选 zwj
                    HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableReset, 0x01);
                    HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableReset, 0x00);
                    //HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableReset, 0x00); //处理板插值系数复位 zwj

                    for (int i = 0; i < Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[coefficientsTableType].Length; i++)
                    {
                        HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteEnable | AcqFpgaAddrMarkTable[fpgaIndex], 0);
                        UInt32 addr = 0;
                        if (i < Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[coefficientsTableType].LengthOfPartA)
                            addr = (UInt32)i;
                        else
                            addr = (UInt32)(0x8000) | (uint)(i - Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[coefficientsTableType].LengthOfPartA);

                        HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteAddr | AcqFpgaAddrMarkTable[fpgaIndex], (UInt32)addr);

                        Int32 data = Coefficients[i];
                        HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteData_L16 | AcqFpgaAddrMarkTable[fpgaIndex], (UInt32)data & 0xffff);
                        HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteData_H | AcqFpgaAddrMarkTable[fpgaIndex], (UInt32)(data >> 16) & 0xffff);

                        HdIO.WriteReg((UInt32)AcqBdReg.W.Interpolate_FactorTableWriteEnable | AcqFpgaAddrMarkTable[fpgaIndex], 1);

                        //to pro
                        HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteAddr, (UInt32)addr);

                        HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteData_L16, (UInt32)data & 0xffff);
                        HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteData_H, (UInt32)(data >> 16) & 0xffff);

                        HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteEnable, 1);
                    }
                }
            }
            #endregion
            return true;
        }
#endregion Interpolation
        #region Adc_INL
        public virtual void ConfigAdcINLCoefficients(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            for(int i=0;i<256;i++)
            {
                int core1Data = CoefficientsTables.Default[coefficientsTableType, 0][i] & 0xff;
                int core2Data = CoefficientsTables.Default[coefficientsTableType, 1][i] & 0xff;
                UInt32 core12Data=(UInt32)((core2Data << 8) | core1Data);
                int core3Data = CoefficientsTables.Default[coefficientsTableType, 2][i] & 0xff;
                int core4Data = CoefficientsTables.Default[coefficientsTableType, 3][i] & 0xff;
                UInt32 core34Data = (UInt32)((core4Data << 8) | core3Data);
                WriteToAllFpga(AcqBdReg.W.ADC_INL_Address , (UInt32)i);
                WriteToAllFpga(AcqBdReg.W.ADC_INL_DataCore1Core2, core12Data);
                WriteToAllFpga(AcqBdReg.W.ADC_INL_DataCore3Core4, core34Data);
                WriteToAllFpga(AcqBdReg.W.ADC_INL_Latch, 0);
                WriteToAllFpga(AcqBdReg.W.ADC_INL_Latch, 1);
            }
            WriteToAllFpga(AcqBdReg.W.ADC_INL_ModuleEn,(UInt32) (Hd.CurrDebugVarints.bEnable_AdcINL? 1:0));
        }
        #endregion
#endregion
        #region MSO8000配置
        protected void PllWrite(AcqBdNo fpgaIndex, UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);

            WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
            WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_L16, fpgaIndex, temp & 0xffff);
            WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_H8, fpgaIndex, (temp >> 16) & 0xff);
            WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, fpgaIndex, 0);
            HdIO.Sleep(1);
        }
        internal virtual UInt32 ReadFromAD5200(AcqBdNo fpgaIndex, Int32 ADCSel, UInt32 Address_15bit)
        {
            AcqBdReg.W reg1, reg2, reg3;
            reg1 = AcqBdReg.W.Adc_DataCmdL8;// ADC_CONFIG_L | CTRL_INDEPENT_REG[fpgaIndex]; //DC_CONFIG1_L;
            reg2 = AcqBdReg.W.Adc_DataCmdH16;// ADC_CONFIG_H | CTRL_INDEPENT_REG[fpgaIndex]; //ADC_CONFIG1_H;
            reg3 = AcqBdReg.W.Adc_ConfigEnable;// ADC5200_CONFIG_EN | CTRL_INDEPENT_REG[fpgaIndex]; //AD9689_CONFIG_EN1;

            UInt32 tmp = ((0x001 << 23) | (Address_15bit << 8) | 0x00);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)

            WriteReg(reg1, fpgaIndex, (UInt32)((UInt16)(tmp))); //10'h00:ADC_SPI_data[7:0] <= adsp_databus_wr[7:0];
            WriteReg(reg2, fpgaIndex, (UInt32)((UInt16)(tmp >> 8)));//10'h01:ADC_SPI_data[23:8] <= adsp_databus_wr[15:0];
            WriteReg(reg3, fpgaIndex, 0xC0);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            WriteReg(reg3, fpgaIndex, 0xE0);

            HdIO.Sleep(1);
            return ReadReg(AcqBdReg.R.Adc_Reg_Rd_Data, fpgaIndex);
        }

        public Dictionary<AcqBdNo, UInt32> ReadFromAllFpga(AcqBdReg.R regAddr, UInt32 validBits)
        {
            Dictionary<AcqBdNo, UInt32> ans = new();
            foreach (AcqBdNo acqBdNo in Enum.GetValues<AcqBdNo>())
            {
                if (BoardExists((int)acqBdNo))
                    ans.Add(acqBdNo, ReadReg(regAddr, acqBdNo) & validBits);
            }

            return ans;
        }
        #endregion

        public virtual void ConfigAdc() { }
        public virtual void SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes dataTypes,UInt32 totalBytes)
        {
            //PCIE 收数据长度
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, totalBytes * 8);//8=8bit

            //路径切换
            uint muxvalue = dataTypes switch
            {
                DMAReadDataTypes.AnalogChannelDdr => 0,
                DMAReadDataTypes.LADdr => 1,
                DMAReadDataTypes.Dpx => 2,
                DMAReadDataTypes.MeasureHist => 4,
                _ => 0,
            };
            HdIO.WriteReg(ProcBdReg.W.DataPath_ProDataMux, muxvalue);

            //PCIE收数据复位
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);
        }
        public virtual void ExecMiscFunc(string param) { }
    }
    internal enum AcqBdNo
    {
        [Description("BoardSync_Io_Delay_Value_Acq1,BoardSync_Pattern_Shift_Value_Acq1")]
        B0 = 0,
        [Description("BoardSync_Io_Delay_Value_Acq2,BoardSync_Pattern_Shift_Value_Acq2")]
        B1 = 1,
        [Description("BoardSync_Io_Delay_Value_Acq3,BoardSync_Pattern_Shift_Value_Acq3")]
        B2 = 2,
        [Description("BoardSync_Io_Delay_Value_Acq4,BoardSync_Pattern_Shift_Value_Acq4")]
        B3 = 3,
        [Description("BoardSync_Io_Delay_Value_Acq5,BoardSync_Pattern_Shift_Value_Acq5")]
        B4 = 4,
        [Description("BoardSync_Io_Delay_Value_Acq6,BoardSync_Pattern_Shift_Value_Acq6")]
        B5 = 5,
        [Description("BoardSync_Io_Delay_Value_Acq7,BoardSync_Pattern_Shift_Value_Acq7")]
        B6 = 6,
        [Description("BoardSync_Io_Delay_Value_Acq8,BoardSync_Pattern_Shift_Value_Acq8")]
        B7 = 7,
        [Description("BoardSync_Io_Delay_Value_Acq9,BoardSync_Pattern_Shift_Value_Acq9")]
        B8 = 8,
        [Description("BoardSync_Io_Delay_Value_Acq10,BoardSync_Pattern_Shift_Value_Acq10")]
        B9 = 9,
        [Description("BoardSync_Io_Delay_Value_Acq11,BoardSync_Pattern_Shift_Value_Acq11")]
        B10 = 10,
        [Description("BoardSync_Io_Delay_Value_Acq12,BoardSync_Pattern_Shift_Value_Acq12")]
        B11 = 11,
        [Description("BoardSync_Io_Delay_Value_Acq13,BoardSync_Pattern_Shift_Value_Acq13")]
        B12 = 12,
        [Description("BoardSync_Io_Delay_Value_Acq14,BoardSync_Pattern_Shift_Value_Acq14")]
        B13 = 13,
        [Description("BoardSync_Io_Delay_Value_Acq15,BoardSync_Pattern_Shift_Value_Acq15")]
        B14 = 14,
        [Description("BoardSync_Io_Delay_Value_Acq16,BoardSync_Pattern_Shift_Value_Acq16")]
        B15 = 15,
    }
    internal record FPGAIndexMarkPair
    {
        public Int32 FpgaIndex
        {
            get;
            init;
        }
        public UInt32 FpgaMark
        {
            get;
            init;
        }

        public List<UInt32> AdcList
        {
            get;
            set;
        } = new List<uint>();
    }
    internal class FPGAAdcPair
    {
        public int FPGAIndex
        {
            get;
            set;
        }
        public int AdcIndex
        {
            get;
            set;
        }
    }
    class TapRegion
    {
        public UInt32 Tap_Start = 0;
        public UInt32 Tap_End = 0;
    }
}
