
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ScopeX.Hardware.Driver
{
	internal abstract class AbstractProcBd : IBoard
	{
		public virtual void Init() { }
		public virtual void Test() { }
		protected virtual void IniAverageModule()
		{
			//平均模式初始化
			//comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Average_RamReset, 1);
			//comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Average_RamReset, 0);
			//comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Average_Number, 4);
			//comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
			//comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 10000);
		}
		public virtual bool IsPowerOk
		{
			get
			{
#if !Product_B21_JinHui_PXI
				if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen)
					return true;
				UInt32 data = 0x55aa;
				HdIO.WriteReg(ProcBdReg.W.SysInfo_WorkOKTest, data);
				HdIO.Sleep(1);
				HdIO.WriteReg(0x8800, 0);
				HdIO.Sleep(1);
				return (HdIO.ReadReg(ProcBdReg.R.SysInfo_WorkOKTest) == data);
#else
                return false;
#endif
			}
		}
		public virtual FpgaVersion? FpgaVersion
		{
			get
			{
				if (fpgaVersions == null)
					return new FpgaVersion();
				else
					return fpgaVersions;
			}
		}
		public virtual string GetRegMonitorResult()
		{
			string resultStr = "";
			StringBuilder stringBuilder = new StringBuilder();
#if !Product_B21_JinHui_PXI
			Type writeEnumType = typeof(ProcBdReg.W);
			Array writeRegs = Enum.GetValues(writeEnumType);
			Type readEnumType = typeof(ProcBdReg.R);
			Array readRegs = Enum.GetValues(readEnumType);
			stringBuilder.AppendLine(("Proc Board").PadRight(60, '='));

			foreach (ProcBdReg.W reg in writeRegs)
			{
				HdIO.WriteReg(ProcBdReg.W.RegMonitor_RegAddress, (UInt32)reg);
				UInt32 readBackData = HdIO.ReadReg(ProcBdReg.R.RegMonitor_ReadbackValue);

				string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(writeEnumType, reg.ToString());
				stringBuilder.AppendLine(("[W],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("x"));
			}
			foreach (ProcBdReg.R reg in readRegs)
			{
				UInt32 readBackData = HdIO.ReadReg((UInt32)reg);
				string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(readEnumType, reg.ToString());
				stringBuilder.AppendLine(("[R],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("x"));
			}
#endif
			resultStr = stringBuilder.ToString();
			return resultStr;
		}
		protected FpgaVersion? fpgaVersions = null;

		public virtual void ReadFpgaVersion()
		{
#if !Product_B21_JinHui_PXI
			FpgaVersionRegs procRegs = new FpgaVersionRegs(
				(UInt32)ProcBdReg.R.VersionInfo1_CompileTimeWord0, (UInt32)ProcBdReg.R.VersionInfo1_CompileTimeWord1,
				(UInt32)ProcBdReg.R.VersionInfo_VersionWord0, (UInt32)ProcBdReg.R.VersionInfo_VersionWord1,
				new UInt32[] { (UInt32)ProcBdReg.R.VersionInfo_DesignerWord0, (UInt32)ProcBdReg.R.VersionInfo_DesignerWord1, (UInt32)ProcBdReg.R.VersionInfo_DesignerWord2, (UInt32)ProcBdReg.R.VersionInfo_DesignerWord3 },
				new UInt32[] {
					(UInt32)ProcBdReg.R.VersionInfo_CommentWord0, (UInt32)ProcBdReg.R.VersionInfo_CommentWord1, (UInt32)ProcBdReg.R.VersionInfo_CommentWord2, (UInt32)ProcBdReg.R.VersionInfo_CommentWord3,
					(UInt32)ProcBdReg.R.VersionInfo_CommentWord4, (UInt32)ProcBdReg.R.VersionInfo_CommentWord5, (UInt32)ProcBdReg.R.VersionInfo_CommentWord6, (UInt32)ProcBdReg.R.VersionInfo_CommentWord7,
					(UInt32)ProcBdReg.R.VersionInfo_CommentWord8, (UInt32)ProcBdReg.R.VersionInfo_CommentWord9, (UInt32)ProcBdReg.R.VersionInfo_CommentWord10, (UInt32)ProcBdReg.R.VersionInfo_CommentWord11,
					(UInt32)ProcBdReg.R.VersionInfo_CommentWord12, (UInt32)ProcBdReg.R.VersionInfo_CommentWord13, (UInt32)ProcBdReg.R.VersionInfo1_CommentWord14, (UInt32)ProcBdReg.R.VersionInfo1_CommentWord15
				});
			if (fpgaVersions == null)
				fpgaVersions = new();
			FpgaVersion.ReadFPGAVersion(procRegs, ref fpgaVersions);
#endif
		}

		public bool ReadAWGProtectState(out byte state)
		{
			UInt16 addrCtrl;

			#region read
			var regAddr = 0xF4;
			var readDataReg = (UInt32)ProcBdReg.R.Awg_data_awg_rd;
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x04);
			// 低8位地址
			addrCtrl = (UInt16)(regAddr & 0xffff);
			// cs为0时，发送一次，赋初值
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			// cs置1，再发送一次
			addrCtrl = (UInt16)(regAddr | (0x0001 << 8));
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
			Thread.Sleep(1);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x06);
			state = (byte)HdIO.ReadReg(readDataReg);
			//HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
			//state = (byte)HdIO.ReadReg(readDataReg);
			#endregion read

			return true;
		}
		public bool ReadAWGVersion(out string versionStr)
		{
			UInt16 addrCtrl;

			#region read
			var regAddr = 0xF2;
			var readDataReg = (UInt32)ProcBdReg.R.Awg_data_awg_rd;
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x04);
			// 低8位地址
			addrCtrl = (UInt16)(regAddr & 0xffff);
			// cs为0时，发送一次，赋初值
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			// cs置1，再发送一次
			addrCtrl = (UInt16)(regAddr | (0x0001 << 8));
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
			Thread.Sleep(1);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x06);
			var bytes1 = (byte)HdIO.ReadReg(readDataReg);
			//HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
			//state = (byte)HdIO.ReadReg(readDataReg);
			regAddr = 0xF3;
			addrCtrl = (UInt16)(regAddr & 0xffff);
			// cs为0时，发送一次，赋初值
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			// cs置1，再发送一次
			addrCtrl = (UInt16)(regAddr | (0x0001 << 8));
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_addr_ctrl, addrCtrl);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x05);
			Thread.Sleep(1);
			HdIO.WriteReg(ProcBdReg.W.Awg_awg_wr_ctrl, 0x06);
			var bytes2 = (byte)HdIO.ReadReg(readDataReg);
			#endregion read
			versionStr = $"{bytes1 / 16}.{bytes1 % 16}.{bytes2}";
			return true;
		}
		public virtual void ConfigRecvDelay()
		{
		}
		#region Dpo
		public virtual void Dpo_ConfigMode()
		{
			//HdCommand.PCIX_WriteRegister32(HdCommand.DPO_TEST_VIEW, 0);
			//HdCommand.PCIX_WriteRegister32(HdCommand.DPO_TEST_WAVE, 0);
		}
		#endregion
	}
}
