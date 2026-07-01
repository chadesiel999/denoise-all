using ScopeX.ComModel;
using System;
using System.Collections.Generic;

namespace ScopeX.Hardware.Driver
{
    public class ProductConfig
    {
        public static ProductConfig Defalut = new ProductConfig();

        public Dictionary<HardwareVersionItem, HardwareVersionInfo?> TryTakeHardwareVersionInfo()
        {
            Dictionary<HardwareVersionItem, HardwareVersionInfo?> result = new Dictionary<HardwareVersionItem, HardwareVersionInfo?>();
            FpgaVersion? fpgaVersion = Hd.CurrProduct?.PcieBd?.FpgaVersion ?? null;
            if (fpgaVersion == null)
                result.Add(HardwareVersionItem.FPGA_Pcie, null);
            else
                result.Add(HardwareVersionItem.FPGA_Pcie, new HardwareVersionInfo() { Major = fpgaVersion.MaxNum, Minor = fpgaVersion.SubNum, Build = fpgaVersion.MinNum, LastBuildDateTime = fpgaVersion.BuildTime, LastModifier = fpgaVersion.Designer, LastComment = fpgaVersion.Comment });

            fpgaVersion = Hd.CurrProduct?.S6Bd?.FpgaVersion ?? null;
            if (fpgaVersion == null)
                result.Add(HardwareVersionItem.FPGA_S6, null);
            else
                result.Add(HardwareVersionItem.FPGA_S6, new HardwareVersionInfo() { Major = fpgaVersion.MaxNum, Minor = fpgaVersion.SubNum, Build = fpgaVersion.MinNum, LastBuildDateTime = fpgaVersion.BuildTime, LastModifier = fpgaVersion.Designer, LastComment = fpgaVersion.Comment });

            fpgaVersion = Hd.CurrProduct?.ProcBd?.FpgaVersion ?? null;
            if (fpgaVersion == null)
                result.Add(HardwareVersionItem.FPGA_ProcBd, null);
            else
                result.Add(HardwareVersionItem.FPGA_ProcBd, new HardwareVersionInfo() { Major = fpgaVersion.MaxNum, Minor = fpgaVersion.SubNum, Build = fpgaVersion.MinNum, LastBuildDateTime = fpgaVersion.BuildTime, LastModifier = fpgaVersion.Designer, LastComment = fpgaVersion.Comment });
            for (int acqBoardIndex = 0; acqBoardIndex < 8; acqBoardIndex++)
            {
                fpgaVersion = Hd.CurrProduct?.AcqBd?.GetFpgaVersion(acqBoardIndex) ?? null;
                if (fpgaVersion != null)
                {
                    result.Add(HardwareVersionItem.FPGA_AcqBd1 + acqBoardIndex, new HardwareVersionInfo() { Major = fpgaVersion.MaxNum, Minor = fpgaVersion.SubNum, Build = fpgaVersion.MinNum, LastBuildDateTime = fpgaVersion.BuildTime, LastModifier = fpgaVersion.Designer, LastComment = fpgaVersion.Comment });
                }
            }
            var ananlogChannelVersion = CtrlAnalogChannel_JiHe2d5G.baseObj1.Mcu_GetVersionInfo();
            if (ananlogChannelVersion != null)
                result.Add(HardwareVersionItem.AnalogChannel_B1, ananlogChannelVersion);
            var ananlogChannelBootVersion = CtrlAnalogChannel_JiHe2d5G.baseObj1.Mcu_GetBootVersionInfo();
            if (ananlogChannelVersion != null)
                result.Add(HardwareVersionItem.AnalogChannel_B1_Boot, ananlogChannelVersion);
            //以后添加模拟通道

            //以后添加键盘板
            var awgVersion = Hd.ResopnseAWGVersion();
            if (!string.IsNullOrWhiteSpace(awgVersion))
            {
                var awgVerSplit = awgVersion.Split('.');
                var hardVersion = new HardwareVersionInfo();
                hardVersion.Minor = int.Parse(awgVerSplit[2]);
                hardVersion.Major = int.Parse(awgVerSplit[0]);
                hardVersion.Revision = int.Parse(awgVerSplit[1]);
                result.Add(HardwareVersionItem.AWG, hardVersion);
            }

            //添加硬件版本
            //硬件版本
            uint version = 0;//HdIO.ReadReg(PcieBdReg.R.SysMon_HardwareVersion);
            version = ReverseBits(version, 3);
            var info = version == 0x6 ? new HardwareVersionInfo() { Major = 1, Minor = 1 ,Build=0, Revision =0} : new HardwareVersionInfo() { Major = 1, Minor = 0, Build = 0, Revision = 0 };
            result.Add(HardwareVersionItem.HardWare, info);
            return result;
        }

        private uint ReverseBits(uint value, int countbit)
        {
            uint result = 0;

            for (int i = 0; i < countbit; i++)
            {
                result = (result << 1) | (value & 1);
                value >>= 1;
            }

            return result;
        }
    }
}
