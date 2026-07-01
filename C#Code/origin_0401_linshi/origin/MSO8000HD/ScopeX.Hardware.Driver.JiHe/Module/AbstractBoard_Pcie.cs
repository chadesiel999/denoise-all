using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal abstract class AbstractPcieBd:IBoard
    {
        public virtual void Init() { }
        public virtual void Test() { }
        public virtual bool IsAllPowerOk() { return false; }
        public virtual bool IsPowerOk() { return true; }
        public virtual void PowerDown() { }
        public virtual FpgaVersion? FpgaVersion { get => fpgaVersion; }
        public virtual string FpgaVerionStr { get => ""; }
        public virtual CaliDataType ChangedCaliDataType { get; set; }
        protected FpgaVersion? fpgaVersion = null;
        public virtual void ReadFpgaVersion()
        {
            FpgaVersionRegs pcieRegs = new FpgaVersionRegs(
                (UInt32)PcieBdReg.R.VersionInfo1_CompileTimeWord0, (UInt32)PcieBdReg.R.VersionInfo1_CompileTimeWord1,
                (UInt32)PcieBdReg.R.VersionInfo_VersionWord0, (UInt32)PcieBdReg.R.VersionInfo_VersionWord1,
                new UInt32[] { (UInt32)PcieBdReg.R.VersionInfo_DesignerWord0, (UInt32)PcieBdReg.R.VersionInfo_DesignerWord1, (UInt32)PcieBdReg.R.VersionInfo_DesignerWord2, (UInt32)PcieBdReg.R.VersionInfo_DesignerWord3 },
                new UInt32[] {
                    (UInt32)PcieBdReg.R.VersionInfo_CommentWord0, (UInt32)PcieBdReg.R.VersionInfo_CommentWord1, (UInt32)PcieBdReg.R.VersionInfo_CommentWord2, (UInt32)PcieBdReg.R.VersionInfo_CommentWord3,
                    (UInt32)PcieBdReg.R.VersionInfo_CommentWord4, (UInt32)PcieBdReg.R.VersionInfo_CommentWord5, (UInt32)PcieBdReg.R.VersionInfo_CommentWord6, (UInt32)PcieBdReg.R.VersionInfo_CommentWord7,
                    (UInt32)PcieBdReg.R.VersionInfo_CommentWord8, (UInt32)PcieBdReg.R.VersionInfo_CommentWord9, (UInt32)PcieBdReg.R.VersionInfo_CommentWord10, (UInt32)PcieBdReg.R.VersionInfo_CommentWord11,
                    (UInt32)PcieBdReg.R.VersionInfo_CommentWord12, (UInt32)PcieBdReg.R.VersionInfo_CommentWord13,(UInt32)PcieBdReg.R.VersionInfo1_CommentWord14, (UInt32)PcieBdReg.R.VersionInfo1_CommentWord15
                });
            if (fpgaVersion == null)
                fpgaVersion = new();
            FpgaVersion.ReadFPGAVersion(pcieRegs, ref fpgaVersion);
        }

        public virtual string GetRegMonitorResult()
        {

            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();
#if !Product_B21_JinHui_PXI
            Type writeEnumType = typeof(PcieBdReg.W);
            Array writeRegs = Enum.GetValues(writeEnumType);
            Type readEnumType = typeof(PcieBdReg.R);
            Array readRegs = Enum.GetValues(readEnumType);
            stringBuilder.AppendLine(("Pcie Board").PadRight(60, '='));
            foreach (PcieBdReg.W reg in writeRegs)
            {
                HdIO.WriteReg(PcieBdReg.W.RegMonitor_RegAddress, (UInt32)reg);
                UInt32 readBackData = HdIO.ReadReg(PcieBdReg.R.RegMonitor_ReadbackValue);

                string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(writeEnumType, reg.ToString());
                stringBuilder.AppendLine(("[W],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("X"));
            }
            foreach (PcieBdReg.R reg in readRegs)
            {
                UInt32 readBackData = HdIO.ReadReg((UInt32)reg);
                string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(readEnumType, reg.ToString());
                stringBuilder.AppendLine(("[R],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("X"));
            }
#endif
            resultStr = stringBuilder.ToString();
            return resultStr;
        }
        public virtual bool ProcBoardAlreadyPowerOn
        {
            get
            {
#if !Product_B21_JinHui_PXI
                UInt32 data = 0x5a5a;
                HdIO.WriteReg(ProcBdReg.W.SysInfo_WorkOKTest, data);
                HdIO.WriteReg(0x8000, 0);
                return ((HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest) & 0xffff) == data);
#else
                return true;
#endif
            }
        }
        public virtual void SendCmdToCD4094(UInt32 lowword, UInt32 highword)
        {
#if !Product_B21_JinHui_PXI
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_4094Data_L32, lowword);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_4094Data_H32, highword);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_4094TransStart, 1);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0x10);
            HdIO.WaitForSpiTransfer(1, 6);
            HdIO.Sleep(5);//延时10ms的道理是什么？
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_4094TransStart, 0);
            HdIO.WriteReg(PcieBdReg.W.AnalogChCtrl_DAC_Sel, 0);
#else
#endif
        }
    }
}
