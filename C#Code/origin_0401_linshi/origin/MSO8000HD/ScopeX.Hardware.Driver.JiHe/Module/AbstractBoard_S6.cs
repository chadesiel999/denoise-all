using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    //本板与校准数据无关
    //不同项目的7044,7043配置可能不同，通过Product配置来实现。
    internal abstract class AbstractS6Bd : IBoard
    {
        public virtual void Init()
        {
            #region PLL
            //Init7044();
            //HdIO.Sleep(50);
            //PllSync();
            //PllWrite7044(0x0104, 0xB3); // DCLKOUT6
            //PllWrite7044(0x0122, 0xB3); // SCLKOUT9
            //PllWrite7044(0x0136, 0xB3); // SCLKOUT11
            //PllWrite7044(0x014A, 0xB2); // SCLKOUT13
            //                            // 10MHz
            //PllWrite7044(0x00FA, 0xB3); // SCLKOUT5
            //PllWrite7044(0x0118, 0xB3); // DCLKOUT8

            //Init7043();
            //HdIO.Sleep(50);
            //PllSync();
            //PllWrite7043(0x0118, 0xB3); // DCLKOUT8
            //PllWrite7043(0x0140, 0xB3); // DCLKOUT12
            //Config7044_A();
            //HdIO.Sleep(500);
            //Config7044_B();
            //HdIO.Sleep(500);
            //Config7044_C();
            //HdIO.Sleep(500);
            #endregion

            ReadFpgaVersion();
        }
        public virtual void Test() { }
        public virtual bool IsPowerOk { get => true; }
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
            Type writeEnumType = typeof(S6BdReg.W);
            Array writeRegs = Enum.GetValues(writeEnumType);
            Type readEnumType = typeof(S6BdReg.R);
            Array readRegs = Enum.GetValues(readEnumType);
            stringBuilder.AppendLine(("S6 Board").PadRight(60, '='));

            foreach (S6BdReg.W reg in writeRegs)
            {
                HdIO.WriteReg(S6BdReg.W.RegMonitor_RegAddress, (UInt32)reg);
                UInt32 readBackData = HdIO.ReadReg(S6BdReg.R.RegMonitor_ReadbackValue);

                string? addr = ",address=" + HdIO.GetFPGARegisterDescription?.Invoke(writeEnumType, reg.ToString());
                stringBuilder.AppendLine(("[W],Name=" + reg.ToString()).PadRight(50) + addr.PadRight(70) + ",value=0x" + readBackData.ToString("x"));
            }
            foreach (S6BdReg.R reg in readRegs)
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
                (UInt32)S6BdReg.R.VersionInfo1_CompileTimeWord0, (UInt32)S6BdReg.R.VersionInfo1_CompileTimeWord1,
                (UInt32)S6BdReg.R.VersionInfo_VersionWord0, (UInt32)S6BdReg.R.VersionInfo_VersionWord1,
                new UInt32[] { (UInt32)S6BdReg.R.VersionInfo_DesignerWord0, (UInt32)S6BdReg.R.VersionInfo_DesignerWord1, (UInt32)S6BdReg.R.VersionInfo_DesignerWord2, (UInt32)S6BdReg.R.VersionInfo_DesignerWord3 },
                new UInt32[] {
                    (UInt32)S6BdReg.R.VersionInfo_CommentWord0, (UInt32)S6BdReg.R.VersionInfo_CommentWord1, (UInt32)S6BdReg.R.VersionInfo_CommentWord2, (UInt32)S6BdReg.R.VersionInfo_CommentWord3,
                    (UInt32)S6BdReg.R.VersionInfo_CommentWord4, (UInt32)S6BdReg.R.VersionInfo_CommentWord5, (UInt32)S6BdReg.R.VersionInfo_CommentWord6, (UInt32)S6BdReg.R.VersionInfo_CommentWord7,
                    (UInt32)S6BdReg.R.VersionInfo_CommentWord8, (UInt32)S6BdReg.R.VersionInfo_CommentWord9, (UInt32)S6BdReg.R.VersionInfo_CommentWord10, (UInt32)S6BdReg.R.VersionInfo_CommentWord11,
                    (UInt32)S6BdReg.R.VersionInfo_CommentWord12, (UInt32)S6BdReg.R.VersionInfo_CommentWord13, (UInt32)S6BdReg.R.VersionInfo1_CommentWord14, (UInt32)S6BdReg.R.VersionInfo1_CommentWord15
                });
            if (fpgaVersions == null)
                fpgaVersions = new();
            FpgaVersion.ReadFPGAVersion(procRegs, ref fpgaVersions);
#endif
        }


        protected PLLRegAddrValuePair[]? _Chip7044ConfigData;
        protected PLLRegAddrValuePair[]? _Chip7044ConfigDataSpecial;

        protected PLLRegAddrValuePair[]? _Chip7043ConfigData;
        protected PLLRegAddrValuePair[]? _Chip7043ConfigDataSpecail;
        protected PLLRegAddrValuePair[]? _Chip7043ConfigData_2;


        public virtual void PllSync()
        {
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 1);
            HdIO.Sleep(1);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044Sync, 0);
        }

        #region 7044 为采集板提供10MHz 参考
        protected virtual void PllWrite7044(UInt32 addr, UInt32 data)
        {
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
        }
        protected record PLLRegAddrValuePair(UInt32 address, UInt32 value);
#if JiHe_MSO8000X
        protected PLLRegAddrValuePair[] Chip7044ConfigData_SchameJiHe8000X =
        {
            //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x04),
            new (0x0003, 0x37),
            new (0x0004, 0x7F),
            new (0x0005, 0x81),// SYNC MODE
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x0009, 0x01),
            //-----------------------
            //      保留寄存器
            //-------------------------
            new (0x0096, 0x00),
            new (0x0097, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x4D),
            new (0x00A0, 0xDF),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00A5, 0x06),
            new (0x00A6, 0x1C),
            new (0x00A7, 0x00),
            new (0x00A8, 0x06),
            new (0x00A9, 0x00),
            new (0x00AB, 0x00),
            new (0x00AC, 0x20),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x04),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-----------------------
            //      PLL2设置
            //-------------------------
            new (0x0031, 0x01),
            new (0x0032, 0x01), // DOUBLE R
            new (0x0033, 0x01), // R2
            new (0x0034, 0x00),
            new (0x0035, 0x19), // N2
            new (0x0036, 0x00),
            new (0x0037, 0x0F),
            new (0x0038, 0x18),
            new (0x0039, 0x01),
            new (0x003A, 0x35),
            new (0x003B, 0x35),
            //-----------------------
            //      PLL1设置
            //-------------------------
            new (0x0046, 0x00),
            new (0x0047, 0x00),
            new (0x0048, 0x08),
            new (0x0049, 0x10),
            new (0x0050, 0x37),
            new (0x0051, 0x2B),
            new (0x0052, 0x37),
            new (0x0053, 0x33),
            new (0x0054, 0x03),
            new (0x005A, 0x01),
            new (0x005B, 0x04),
            new (0x005C, 0x80),//CIJ_NEW
            new (0x005D, 0x02),//CIJ_NEW
            new (0x0064, 0x00),
            new (0x0065, 0x00),
            new (0x0070, 0xE0), // alarm
            new (0x0071, 0x19),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007B, 0x00),
            new (0x007C, 0x00),
            new (0x007D, 0x00),
            new (0x007E, 0x00),
            new (0x0082, 0x00),
            new (0x0083, 0x00),
            new (0x0084, 0x00),
            new (0x0085, 0x00),
            new (0x0086, 0x00),
            new (0x008C, 0x00),
            new (0x008D, 0x00),
            new (0x008E, 0x00),
            new (0x008F, 0x00),
            new (0x0091, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
        //-----------------------
            //      Output 模式
            //-------------------------
            // NC
            //new (0x00C8, 0x00), // DCLKOUT0
            //new (0x00D2, 0x00), // SCLKOUT1
            //new (0x00DC, 0x00), // DCLKOUT2
            //new (0x00F0, 0x00), // DCLKOUT4
            //new (0x012C, 0x00), // DCLKOUT10
            //// KEEP
            //new (0x0104, 0xF3), // DCLKOUT6
            //new (0x0122, 0xF3), // SCLKOUT9
            //new (0x0136, 0xF3), // SCLKOUT11
            //new (0x014A, 0xF2), // SCLKOUT13
            //// 10MHz
            //new (0x00FA, 0xF3), // SCLKOUT5
            //new (0x0118, 0xF3), // DCLKOUT8
            //// Pluse
            //new (0x010E, 0x5D), // SCLKOUT7
            //new (0x00E6, 0x5D), // SCLKOUT3  //acq rfsync
            //new (0x0140, 0x5D), // DCLKOUT12 //acq rfsync
                     // NC
            
            new (0x00D2, 0x5D), // SCLKOUT1//
            new (0x00DC, 0xF3), // DCLKOUT2 //FPGA1 GT ZM
            new (0x0122, 0xF3), // SCLKOUT9 //FPGA3 GT1 ZM
            new (0x012C, 0xF3), // DCLKOUT10 //FPGA3 GT2 ZM
            new (0x0104, 0xF3), // DCLKOUT6 //AWG CLK
            new (0x0136, 0xF3), // SCLKOUT11 //FPGA4 PCIE GT ZM
            new (0x014A, 0xF3), // SCLKOUT13
            new (0x00FA, 0x5D), // SCLKOUT5//
            new (0x0118, 0xF3), // DCLKOUT8 //AWG FPGA CLK
            new (0x010E, 0x00), // SCLKOUT7 //AWG REF CLK ,NOT USE
            //new (0x00E6, 0x5D), // SCLKOUT3  //acq rfsync
            new (0x00E6, 0xF3), // SCLKOUT3  //FPGA2 GT ZM
            new (0x0140, 0xF3), // DCLKOUT12 //FPGA3 PRO GT ZM
           
            // 10MHz
            new (0x00c8, 0xF3), // DCLKOUT0 to acq1_10M
            new (0x00f0, 0xF3), // DCLKOUT4 to acq3_10M
           

            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new (0x00C9, 0xFA), // [7:0]LSB
            new (0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @ pro_to_acq_10MHz
            new (0x00D3, 0x80), // [7:0]LSB 
            new (0x00D4, 0x02), // [3:0]MSB SCLKOUT1 @ pro_to_acq_sync
            new (0x00DD, 0x08), // [7:0]LSB  //CIJ_NEW
            new (0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @ adc1_gth_refclk_@312.5Mhz
            new (0x00F1, 0xFA), // [7:0]LSB
            new (0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @ pro_to_acq_10MHz
            new (0x012D, 0x08), // [7:0]LSB
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @PRO adc2_gth_refclk_@312.5Mhz
            // KEEP
            new (0x0105, 0x04), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ 625MHz AWG CLK
            new (0x0123, 0x08), // [7:0]LSB
            new (0x0124, 0x00), // [3:0]MSB SCLKOUT9 @PRO adc1_gth_refclk_@312.5Mhz
            new (0x0137, 0x08), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ PCIE gth_refclk_@312.5Mhz
            new (0x014B, 0x08), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ TRIG //CIJ_NEW
            new (0x00FB, 0x80), // [7:0]LSB  
            new (0x00FC, 0x02), // [3:0]MSB SCLKOUT5 @pro_to_acq_sync  CIJ_NEW
            //new (0x0119, 0x00), // [7:0]LSB
            new (0x0119, 0x04), // [7:0]LSB
            new (0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ 625MHz AWG FPGA CLK
            // Pluse
            new (0x010F, 0x04), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 @ 625MHz AWG REF CLK
            new (0x00E7, 0x08), // [7:0]LSB
            new (0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ adc2_gth_refclk_@312.5Mhz
            new (0x0141, 0x08), // [7:0]LSB  8(312.5MHz)  2024/04/15 zm改
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ 
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE
            new (0x00CB, 0x00), // DCLKOUT0
            new (0x00D5, 0x00), // SCLKOUT1
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0125, 0x00), // SCLKOUT9
            new (0x012F, 0x00), // DCLKOUT10
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // 10MHz
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0143, 0x00), // DCLKOUT12
            // Pluse
            new (0x0111, 0x00), // SCLKOUT7
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4    
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0130, 0x00), // DCLKOUT10
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // 10MHz
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0144, 0x00), // DCLKOUT12
            // Pluse
            new (0x0112, 0x00), // SCLKOUT7 
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2     
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4           
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6          
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            // Pluse
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // Pluse
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x01), // DCLKOUT0
            new (0x00D9, 0x01), // SCLKOUT1
            new (0x00E3, 0x01), // DCLKOUT2
            new (0x00ED, 0x01), // SCLKOUT3
            new (0x00F7, 0x01), // DCLKOUT4
            new (0x0101, 0x01), // SCLKOUT5
            new (0x010B, 0x01), // DCLKOUT6
            new (0x0115, 0x01), // SCLKOUT7
            new (0x011F, 0x01), // DCLKOUT8
            new (0x0129, 0x01), // SCLKOUT9
            new (0x0133, 0x01), // DCLKOUT10
            new (0x013D, 0x01), // SCLKOUT11
            new (0x0147, 0x01), // DCLKOUT12
            new (0x0151, 0x01), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new (0x00D0, 0x08), // DCLKOUT0 LVPECL
            new (0x00DA, 0x88), // SCLKOUT1 LVPECL sync 0X89
            new (0x00E4, 0x10), // DCLKOUT2 LVDS
            new (0x00EE, 0x10), // SCLKOUT3 LVDS
            new (0x00F8, 0x08), // DCLKOUT4 LVPECL
            new (0x0102, 0x88), // SCLKOUT5 LVPECL sync 0X89
            new (0x010C, 0x10), // DCLKOUT6 LVDS
            new (0x0116, 0x10), // SCLKOUT7 LVDS
            new (0x0120, 0x10), // DCLKOUT8 LVDS
            new (0x012A, 0x10), // SCLKOUT9 LVDS
            new (0x0134, 0x10), // DCLKOUT10 LVDS
            new (0x013E, 0x10), // SCLKOUT11 LVDS
            new (0x0148, 0x10), // DCLKOUT12 LVDS
            new (0x0152, 0x10), // SCLKOUT13 LVDS
            //-----------------------------//
            // 		Input buffer
            //---------------------------//
            new (0x000A, 0x07), // CLKIN0/RFSYNCIN
            new (0x000B, 0x07), // CLKIN1
            new (0x000C, 0x07), // CLKIN2
            new (0x000D, 0x07), // CLKIN3
            new (0x000E, 0x07), // OSCIN
            //-----------------------------//
            // 		Other
            //---------------------------//
            new (0x0001, 0x02),
            new (0x0001, 0x00),
            new (0x0014, 0x00),
            new (0x0015, 0x03),
            new (0x0016, 0x0C),
            new (0x0017, 0x00),
            new (0x0018, 0x04),
            new (0x0019, 0x03),
            new (0x001A, 0x08),
            new (0x001B, 0x18),
            new (0x001C, 0x01),
            new (0x001D, 0x01),
            new (0x001E, 0x01),
            new (0x001F, 0x01),
            new (0x0020, 0x01),
            new (0x0021, 0x01), // R1 
            new (0x0022, 0x00),
            new (0x0026, 0x0A), // N1
            new (0x0027, 0x00),
            new (0x0028, 0x0F),
            new (0x0029, 0x07),
            new (0x002A, 0x0F),
        };
#endif
        // cij_jiuyuan_new_clk
        protected PLLRegAddrValuePair[] ConfigData7044A_Schame40G12BIT =
{
            //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x04),
            new (0x0003, 0x37),
            new (0x0004, 0x7F),
            new (0x0005, 0x82),// SYNC MODE  //clkin0 0x81 //clkin1 0x82
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x0009, 0x01),
            //-----------------------
            //      保留寄存器
            //-------------------------
            new (0x0096, 0x00),
            new (0x0097, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x4D),
            new (0x00A0, 0xDF),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00A5, 0x06),
            new (0x00A6, 0x1C),
            new (0x00A7, 0x00),
            new (0x00A8, 0x06),
            new (0x00A9, 0x00),
            new (0x00AB, 0x00),
            new (0x00AC, 0x20),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x04),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-----------------------
            //      PLL2设置
            //-------------------------
            new (0x0031, 0x01),
            new (0x0032, 0x01), // DOUBLE R
            new (0x0033, 0x01), // R2
            new (0x0034, 0x00),
            new (0x0035, 0x19), // N2
            new (0x0036, 0x00),
            new (0x0037, 0x0F),
            new (0x0038, 0x18),
            new (0x0039, 0x01),
            new (0x003A, 0x35),
            new (0x003B, 0x35),
            //-----------------------
            //      PLL1设置
            //-------------------------
            new (0x0046, 0x00),
            new (0x0047, 0x00),
            new (0x0048, 0x08),
            new (0x0049, 0x10),
            new (0x0050, 0x37),
            new (0x0051, 0x2B),
            new (0x0052, 0x37),
            new (0x0053, 0x33),
            new (0x0054, 0x03),
            new (0x005A, 0x01),
            new (0x005B, 0x04),
            new (0x005C, 0x80),
            new (0x005D, 0x02),  //cij 0160
            new (0x0064, 0x00),
            new (0x0065, 0x00),
            new (0x0070, 0xE0), // alarm
            new (0x0071, 0x19),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007B, 0x00),
            new (0x007C, 0x00),
            new (0x007D, 0x00),
            new (0x007E, 0x00),
            new (0x0082, 0x00),
            new (0x0083, 0x00),
            new (0x0084, 0x00),
            new (0x0085, 0x00),
            new (0x0086, 0x00),
            new (0x008C, 0x00),
            new (0x008D, 0x00),
            new (0x008E, 0x00),
            new (0x008F, 0x00),
            new (0x0091, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
                   
            //开启：5D:,关闭：00 
            //开启：F3:,关闭：00
            
            new (0x00C8, 0x5d), // DCLKOUT0     @0511 HXY 修改     //ADC1_SYNC_SCLK
            new (0x00D2, 0x5d), // SCLKOUT1     @0511 HXY 修改     //ADC3_SYNC_SCLK
            new (0x00DC, 0x5D), // DCLKOUT2     @0511 HXY 修改     //ADC4_SYNC_SCLK
            new (0x00E6, 0x5d), // SCLKOUT3     @0511 HXY 修改     //ADC2_SYNC_SCLK
            new (0x00F0, 0x5d), // DCLKOUT4     @0511 HXY 修改     //HMC7044B_SYNC      
            new (0x00FA, 0xF3), // SCLKOUT5     @0511 HXY 修改     //HMC7044B_CLKIN     
            new (0x0104, 0x5d), // DCLKOUT6     @0511 HXY 修改     //ADC6_SYNC_SCLK
            new (0x010E, 0xF3), // SCLKOUT7     @0511 HXY 修改     //HMC7044C_CLKIN
            new (0x0118, 0x5d), // DCLKOUT8     @0511 HXY 修改     //HMC7044C_SYNC
            new (0x0122, 0x5d), // SCLKOUT9     @0511 HXY 修改     //ADC8_SYNC_SCLK
            new (0x012C, 0x5D), // DCLKOUT10    @0511 HXY 修改     //ADC5_SYNC_SCLK
            new (0x0136, 0x5d), // SCLKOUT11    @0511 HXY 修改     //ADC7_SYNC_SCLK
            new (0x0140, 0x5d), // DCLKOUT12    @0511 HXY 修改     //PRO2_OUT_SYNC
            new (0x014A, 0x00), // SCLKOUT13    @0511 HXY 修改     //KU035_KU035_GT_CLK1
      

            //-----------------------
            //      Output 分频
            //-------------------------
     

            // Pluse
            new (0x00C9, 0x80), // [7:0]LSB
            new (0x00CA, 0x02), // [3:0]MSB DCLKOUT0 修改 //ADC1_SYNC_SCLK  2500/704=3.55
            new (0x00D3, 0x80), // [7:0]LSB
            new (0x00D4, 0x02), // [3:0]MSB SCLKOUT1 修改 //ADC3_SYNC_SCLK 2500/704=3.55
            new (0x00DD, 0x80), // [7:0]LSB
            new (0x00DE, 0x02), // [3:0]MSB DCLKOUT2 修改 //ADC4_SYNC_SCLK 2500/704=3.55
            new (0x00E7, 0x80), // [7:0]LSB
            new (0x00E8, 0x02), // [3:0]MSB SCLKOUT3 修改 //ADC2_SYNC_SCLK 2500/704=3.55
            new (0x00F1, 0x80), // [7:0]LSB
            new (0x00F2, 0x02), // [3:0]MSB DCLKOUT4 修改 //HMC7044B_SYNC 2500/704=3.55
            new (0x00FB, 0xFA), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 修改 //HMC7044B_CLKIN  10MHz
            new (0x0105, 0x80), // [7:0]LSB
            new (0x0106, 0x02), // [3:0]MSB DCLKOUT6 修改 //ADC6_SYNC_SCLK  2500/704=3.55
            new (0x010F, 0xFA), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 修改 //HMC7044C_CLKIN  10MHz
            new (0x0119, 0x80), // [7:0]LSB
            new (0x011A, 0x02), // [3:0]MSB DCLKOUT8 修改 //HMC7044C_SYNC 2500/704=3.55
            new (0x0123, 0x80), // [7:0]LSB
            new (0x0124, 0x02), // [3:0]MSB SCLKOUT9 修改 //ADC8_SYNC_SCLK 2500/704=3.55
            new (0x012D, 0x80), // [7:0]LSB
            new (0x012E, 0x02), // [3:0]MSB DCLKOUT10 修改//ADC5_SYNC_SCLK 2500/704=3.55
            new (0x0137, 0x80), // [7:0]LSB
            new (0x0138, 0x02), // [3:0]MSB SCLKOUT11 修改//ADC7_SYNC_SCLK 2500/704=3.55
            new (0x0141, 0x80), // [7:0]LSB
            new (0x0142, 0x02), // [3:0]MSB DCLKOUT12 修改//PRO2_OUT_SYNC 2500/704=3.55
            new (0x014B, 0x0A), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 修改//KU035_KU035_GT_CLK1 250M


            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE
            new (0x00CB, 0x02), // DCLKOUT0
            new (0x00D5, 0x02), // SCLKOUT1
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0125, 0x00), // SCLKOUT9
            new (0x012F, 0x00), // DCLKOUT10
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // 10MHz
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0143, 0x00), // DCLKOUT12
            // Pluse
            new (0x0111, 0x00), // SCLKOUT7
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4    
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0130, 0x00), // DCLKOUT10
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // 10MHz
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0144, 0x00), // DCLKOUT12
            // Pluse
            new (0x0112, 0x00), // SCLKOUT7 
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2     
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4           
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6          
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13
            // 10MHz
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            // Pluse
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // Pluse
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x00), // DCLKOUT0
            new (0x00D9, 0x00), // SCLKOUT1
            new (0x00E3, 0x00), // DCLKOUT2
            new (0x00ED, 0x00), // SCLKOUT3
            new (0x00F7, 0x00), // DCLKOUT4
            new (0x0101, 0x00), // SCLKOUT5
            new (0x010B, 0x00), // DCLKOUT6
            new (0x0115, 0x00), // SCLKOUT7
            new (0x011F, 0x00), // DCLKOUT8
            new (0x0129, 0x00), // SCLKOUT9
            new (0x0133, 0x00), // DCLKOUT10
            new (0x013D, 0x00), // SCLKOUT11
            new (0x0147, 0x00), // DCLKOUT12
            new (0x0151, 0x00), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------

                        // Output driver
            //90:Force to Logic 0.;                LVDS mode.   ;Internal resistor disable.
            //89;Force to Logic 0.;                LVPECL mode. ;Internal 100 Ω resistor enable per output pin.
            //10:Normal mode (selection for DCLK).;LVDS mode.   ;Internal resistor disable.
            //80:Force to Logic 0.;                CML mode     ;Internal resistor disable.
            //81:Force to Logic 0.;                CML mode     ;Internal 100 Ω resistor enable per output pin.
            //82:Force to Logic 0.;                CML mode     ;Reserved.
            //83:Force to Logic 0.;                CML mode     ;Internal 50 Ω resistor enable per output pin.
            //08:Normal mode (selection for DCLK).;LVPECL mode. ;Internal resistor disable.
            //09;Normal mode (selection for DCLK).;LVPECL mode. ;Internal 100 Ω resistor enable per output pin.

            new (0x00D0, 0x88), // DCLKOUT0 @0511 HXY 修改     //ADC1_SYNC_SCLK           LVPECL  
            new (0x00DA, 0x88), // SCLKOUT1 @0511 HXY 修改     //ADC3_SYNC_SCLK           LVPECL  
            new (0x00E4, 0x88), // DCLKOUT2 @0511 HXY 修改     //ADC4_SYNC_SCLK           LVPECL  
            new (0x00EE, 0x88), // SCLKOUT3 @0511 HXY 修改     //ADC2_SYNC_SCLK           LVPECL  
            new (0x00F8, 0x88), // DCLKOUT4 @0511 HXY 修改     //HMC7044B_SYNC            LVPECL  
            new (0x0102, 0x08), // SCLKOUT5 @0511 HXY 修改     //HMC7044B_CLKIN           LVPECL  
            new (0x010C, 0x88), // DCLKOUT6 @0511 HXY 修改     //ADC6_SYNC_SCLK           LVPECL  
            new (0x0116, 0x08), // SCLKOUT7 @0511 HXY 修改     //HMC7044C_CLKIN           LVPECL  
            new (0x0120, 0x88), // DCLKOUT8 @0511 HXY 修改     //HMC7044C_SYNC            LVPECL 
            new (0x012A, 0x88), // SCLKOUT9 @0511 HXY 修改     //ADC8_SYNC_SCLK           LVPECL  
            new (0x0134, 0x88), // DCLKOUT10 @0511 HXY 修改    //ADC5_SYNC_SCLK           LVPECL   
            new (0x013E, 0x88), // SCLKOUT11 @0511 HXY 修改    //ADC7_SYNC_SCLK           LVPECL  
            new (0x0148, 0x88), // DCLKOUT12 @0511 HXY 修改    //PRO2_OUT_SYNC            LVPECL  
            new (0x0152, 0x08), // SCLKOUT13 @0511 HXY 修改    //KU035_KU035_GT_CLK1      LVPECL  
            //new (0x0152, 0x08), // SCLKOUT13
            //-----------------------------//
            // 		Input buffer
            //---------------------------//
            new (0x000A, 0x07), // CLKIN0/RFSYNCIN @0408 HXY 修改 PRO2_IN_SYNC
            new (0x000B, 0x07), // CLKIN1          @0408 HXY 修改 10MHz
            new (0x000C, 0x07), // CLKIN2
            new (0x000D, 0x07), // CLKIN3
            new (0x000E, 0x07), // OSCIN
            //-----------------------------//
            // 		Other
            //---------------------------//
            new (0x0001, 0x02),
            new (0x0001, 0x00),
            new (0x0014, 0xF5),//选择输入接口优先级 // 00//@0408 HXY 修改 F5（1111 0101） CLKIN1为最高优先级输入（系统10MHz），其次为CLKIN3输入（外部座子输入）
            new (0x0015, 0x03),
            new (0x0016, 0x0C),
            new (0x0017, 0x00),
            new (0x0018, 0x04),
            new (0x0019, 0x03),
            new (0x001A, 0x08),
            new (0x001B, 0x18),
            new (0x001C, 0x01),
            new (0x001D, 0x01),
            new (0x001E, 0x01),
            new (0x001F, 0x01),
            new (0x0020, 0x01),
            new (0x0021, 0x01), // R1 
            new (0x0022, 0x00),
            new (0x0026, 0x0A), // N1
            new (0x0027, 0x00),
            new (0x0028, 0x0F),
            new (0x0029, 0x07),
            new (0x002A, 0x0F),

        };
        protected PLLRegAddrValuePair[] SpecialConfigData7044A_Schame40G12BIT =
        {
            new (0x00FA, 0xB3), // SCLKOUT5     @0511 HXY 修改     //HMC7044B_CLKIN
            new (0x010E, 0xB3), // SCLKOUT7     @0511 HXY 修改     //HMC7044C_CLKIN

        };
        protected PLLRegAddrValuePair[] ConfigData7044B_Schame40G12BIT =
{
            //    地址 , 值
            /**********register_config***********/
            new(0x0000, 0x01),
            new(0x0000, 0x00),
            new(0x0001, 0x40),
            new(0x0002, 0x04),
            new(0x0003, 0x37),
            new(0x0004, 0x7F),
            new(0x0005, 0x98), // SYNC MODE (82、58) 98
            new(0x0006, 0x00),
            new(0x0007, 0x00),
            new(0x0009, 0x01),
            //-----------------------------//
            // 		保留寄存器
            //---------------------------//
            new(0x0096, 0x00),
            new(0x0097, 0x00),
            new(0x0098, 0x00),
            new(0x0099, 0x00),
            new(0x009A, 0x00),
            new(0x009B, 0xAA),
            new(0x009C, 0xAA),
            new(0x009D, 0xAA),
            new(0x009E, 0xAA),
            new(0x009F, 0x4D),
            new(0x00A0, 0xDF),
            new(0x00A1, 0x97),
            new(0x00A2, 0x03),
            new(0x00A3, 0x00),
            new(0x00A4, 0x00),
            new(0x00A5, 0x06),
            new(0x00A6, 0x1C),
            new(0x00A7, 0x00),
            new(0x00A8, 0x06),
            new(0x00A9, 0x00),
            new(0x00AB, 0x00),
            new(0x00AC, 0x20),
            new(0x00AD, 0x00),
            new(0x00AE, 0x08),
            new(0x00AF, 0x50),
            new(0x00B0, 0x04),
            new(0x00B1, 0x0D),
            new(0x00B2, 0x00),
            new(0x00B3, 0x00),
            new(0x00B5, 0x00),
            new(0x00B6, 0x00),
            new(0x00B7, 0x00),
            new(0x00B8, 0x00),
            //-----------------------------//
            // 		PLL2配置
            //---------------------------//
            new(0x0031, 0x01),
            new(0x0032, 0x01), // DOUBLE R
            new(0x0033, 0x01), // R2
            new(0x0034, 0x00),
            new(0x0035, 0x19), // N2
            new(0x0036, 0x00),
            new(0x0037, 0x0F),
            new(0x0038, 0x18),
            new(0x0039, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Path Control  输出分频器和路径使能
            new(0x003A, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Driver Control输出驱动配置
            new(0x003B, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Driver Control输出驱动配置
            //-----------------------------//
            // 		PLL1配置
            //---------------------------//
            new(0x0046, 0x00),
            new(0x0047, 0x00),
            new(0x0048, 0x08),
            new(0x0049, 0x10),
            new(0x0050, 0x1F),
            new(0x0051, 0x2B),
            new(0x0052, 0x37),
            //HMC7044read(0x0050, 0x7f),
            //new(0x0050, 0x7f),//1F
            //new(0x0051, 0x7f),
            //new(0x0052, 0x7f),

            new(0x0053, 0x33),
            new(0x0054, 0x03),
            //// significant start
            //new(3, 0x005B, 0x00),
            //new(3, 0x005C, 0x80),
            //new(3, 0x005D, 0x00),
            //// significant end
            new(0x0064, 0x00),
            new(0x0065, 0x00),
            new(0x0070, 0xE0), // alarm
            new(0x0071, 0x19),
            new(0x0078, 0x00),
            new(0x0079, 0x00),
            new(0x007A, 0x00),
            new(0x007B, 0x00),
            new(0x007C, 0x00),
            new(0x007D, 0x00),
            new(0x007E, 0x00),
            new(0x0082, 0x00),
            new(0x0083, 0x00),
            new(0x0084, 0x00),
            new(0x0085, 0x00),
            new(0x0086, 0x00),
            new(0x008C, 0x00),
            new(0x008D, 0x00),
            new(0x008E, 0x00),
            new(0x008F, 0x00),
            new(0x0091, 0x00),
            //-----------------------------//
            // 		Sysref Timer
            //---------------------------//
            new(0x005A, 0x01),//07
            new(0x005B, 0x04), // 04
            new(0x005C, 0x00), // [7:0]LSB
            new(0x005D, 0x02), // [3:0]MSB\\05// @0408 HXY 修改 0200 2500/512=4.88MHz
            //-----------------------------//
            // 		Clock Output Channel
            //---------------------------//
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            new(0x00C8, 0xF3), // DCLKOUT0   @0511 HXY 修改  PRO4_GT_CLK  250MHz  LVPECL    
            new(0x00D2, 0xF3), // SCLKOUT1   @0511 HXY 修改  PRO3_GT_CLK  250MHz  LVPECL    
            new(0x00DC, 0xF3), // DCLKOUT2   @0511 HXY 修改  PRO2_GT_CLK  250MHz  LVPECL    
            new(0x00E6, 0xF3), // SCLKOUT3   @0511 HXY 修改  PRO1_GT_CLK  250MHz  LVPECL    
            new(0x00F0, 0xF3), // DCLKOUT4   @0511 HXY 修改  ACQ3_GT_CLK  250MHz  LVPECL    
            new(0x00FA, 0xF3), // SCLKOUT5   @0511 HXY 修改  ADC3_SCLK    10MHz   LVPECL    
            new(0x0104, 0xF3), // DCLKOUT6   @0511 HXY 修改  ADC1_SCLK    10MHz   LVPECL    
            new(0x010E, 0xF3), // SCLKOUT7   @0511 HXY 修改  ACQ1_GT_CLK  250MHz  LVPECL    
            new(0x0118, 0xF3), // DCLKOUT8   @0511 HXY 修改  ACQ4_GT_CLK  250MHz  LVPECL    
            new(0x0122, 0xF3), // SCLKOUT9   @0511 HXY 修改  ADC4_SCLK    10MHz   LVPECL    
            new(0x012C, 0xF3), // DCLKOUT10  @0511 HXY 修改  ADC2_SCLK    10MHz   LVPECL    
            new(0x0136, 0xF3), // SCLKOUT11  @0511 HXY 修改  ACQ2_GT_CLK  250MHz  LVPECL    
            new(0x0140, 0x00), // DCLKOUT12  @0511 HXY 修改  LA_GT_CLK2   250MHz  LVDS    
            new(0x014A, 0x00), // SCLKOUT13  @0511 HXY 修改  LA_GT_CLK1   250MHz  LVDS    

            // Output divider
            // Even divide ratios from 2 to 4094
            // Odd divide ratios are 1、3、5
            new(0x00C9, 0x0A), // [7:0]LSB
            new(0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @0511 HXY 修改  PRO4_GT_CLK  250MHz  LVDS     
            new(0x00D3, 0x0A), // [7:0]LSB
            new(0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @0511 HXY 修改  PRO3_GT_CLK  250MHz  LVDS      

            new(0x00DD, 0x0A), // [7:0]LSB
            new(0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @0511 HXY 修改  PRO2_GT_CLK   250MHz  LVDS
            new(0x00E7, 0x0A), // [7:0]LSB
            new(0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @0511 HXY 修改  PRO1_GT_CLK   250MHz  LVDS

            new(0x00F1, 0x0A), // [7:0]LSB
            new(0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @0511 HXY 修改  ACQ3_GT_CLK  250MHz  LVDS  
            new(0x00FB, 0xFA), // [7:0]LSB
            new(0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @0511 HXY 修改  ADC3_SCLK    10MHz   LVPECL  

            new(0x0105, 0xFA), // [7:0]LSB
            new(0x0106, 0x00), // [3:0]MSB DCLKOUT6 @0511 HXY 修改  ADC1_SCLK    10MHz  LVPECL     
            new(0x010F, 0x0A), // [7:0]LSB                                            
            new(0x0110, 0x00), // [3:0]MSB SCLKOUT7 @0511 HXY 修改  ACQ1_GT_CLK    250MHz   LVDS     ??
            new(0x0119, 0x0A), // [7:0]LSB                                            
            new(0x011A, 0x00), // [3:0]MSB DCLKOUT8 @0511 HXY 修改  ACQ4_GT_CLK  250MHz  LVDS     
            new(0x0123, 0xFA), // [7:0]LSB                                            
            new(0x0124, 0x00), // [3:0]MSB SCLKOUT9 @0511 HXY 修改  ADC4_SCLK    10MHz   LVPECL     

            new(0x012D, 0xFA), // [7:0]LSB
            new(0x012E, 0x00), // [3:0]MSB DCLKOUT10 @0511 HXY 修改  ADC2_SCLK    10MHz   LVPECL   
            new(0x0137, 0x0A), // [7:0]LSB
            new(0x0138, 0x00), // [3:0]MSB SCLKOUT11 @0511 HXY 修改  ACQ2_GT_CLK  250MHz  LVDS    
            new(0x0141, 0x0A), // [7:0]LSB
            new(0x0142, 0x00), // [3:0]MSB DCLKOUT12 @0511 HXY 修改  LA_GT_CLK2  250MHz  LVDS   
            new(0x014B, 0x0A), // [7:0]LSB
            new(0x014C, 0x00), // [3:0]MSB SCLKOUT13 @0511 HXY 修改  LA_GT_CLK1  250MHz  LVDS    

            // Fine analog delay
            // Step size 25ps
            // 0~23 effective
            new(0x00CB, 0x00), // DCLKOUT0 
            new(0x00DF, 0x00), // DCLKOUT2 
            new(0x00F3, 0x00), // DCLKOUT4  
            new(0x0107, 0x00), // DCLKOUT6 
            new(0x011B, 0x00), // DCLKOUT8
            new(0x012F, 0x00), // DCLKOUT10//08 lwq 0708
            new(0x0143, 0x00), // DCLKOUT12//0F
            new(0x00D5, 0x00), // SCLKOUT1
            new(0x00E9, 0x00), // SCLKOUT3//0F
            new(0x00FD, 0x00), // SCLKOUT5
            new(0x0111, 0x00), // SCLKOUT7
            new(0x0125, 0x00), // SCLKOUT9
            new(0x0139, 0x00), // SCLKOUT11
            new(0x014D, 0x00), // SCLKOUT13//02//04

            // Coarse digital deladelay
            // Step size 1/2 VCO cyclk//10G-100ps-50ps
            // 0~17 effective
            new(0x00CC, 0x00), // DCLKOUT0
            new(0x00D6, 0x00), // SCLKOUT1
            new(0x00E0, 0x00), // DCLKOUT2
            new(0x00EA, 0x00), // SCLKOUT3
            new(0x00F4, 0x00), // DCLKOUT4
            new(0x00FE, 0x00), // SCLKOUT5
            new(0x0108, 0x00), // DCLKOUT6
            new(0x0112, 0x00), // SCLKOUT7
            new(0x011C, 0x00), // DCLKOUT8
            new(0x0126, 0x00), // SCLKOUT9
            new(0x0130, 0x00), // DCLKOUT10
            new(0x013A, 0x00), // SCLKOUT11
            new(0x0144, 0x00), // DCLKOUT12
            new(0x014E, 0x00), // SCLKOUT13
            // Multislip digital delay
            // Step size : amount * VCO cycles
            new(0x00CD, 0x00), // [7:0]LSB
            new(0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new(0x00D7, 0x00), // [7:0]LSB
            new(0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new(0x00E1, 0x00), // [7:0]LSB
            new(0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new(0x00EB, 0x00), // [7:0]LSB
            new(0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new(0x00F5, 0x00), // [7:0]LSB
            new(0x00F6, 0x00), // [3:0]MSB DCLKOUT4
            new(0x00FF, 0x00), // [7:0]LSB
            new(0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new(0x0109, 0x00), // [7:0]LSB
            new(0x010A, 0x00), // [3:0]MSB DCLKOUT6
            new(0x0113, 0x00), // [7:0]LSB
            new(0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new(0x011D, 0x00), // [7:0]LSB
            new(0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new(0x0127, 0x00), // [7:0]LSB
            new(0x0128, 0x00), // [3:0]MSB SCLKOUT9
            new(0x0131, 0x00), // [7:0]LSB
            new(0x0132, 0x00), // [3:0]MSB DCLKOUT10
            new(0x013B, 0x00), // [7:0]LSB
            new(0x013C, 0x00), // [3:0]MSB SCLKOUT11
            new(0x0145, 0x00), // [7:0]LSB
            new(0x0146, 0x00), // [3:0]MSB DCLKOUT12
            new(0x014F, 0x00), // [7:0]LSB
            new(0x0150, 0x00), // [3:0]MSB SCLKOUT13
            // Output mux slelction
            new(0x00CF, 0x00), // DCLKOUT0
            new(0x00D9, 0x00), // SCLKOUT1
            new(0x00E3, 0x00), // DCLKOUT2
            new(0x00ED, 0x00), // SCLKOUT3
            new(0x00F7, 0x00), // DCLKOUT4
            new(0x0101, 0x00), // SCLKOUT5
            new(0x010B, 0x00), // DCLKOUT6
            new(0x0115, 0x00), // SCLKOUT7
            new(0x011F, 0x00), // DCLKOUT8
            new(0x0129, 0x00), // SCLKOUT9
            new(0x0133, 0x00), // DCLKOUT10
            new(0x013D, 0x00), // SCLKOUT11
            new(0x0147, 0x00), // DCLKOUT12
            new(0x0151, 0x00), // SCLKOUT13

            // Output driver
            //90:Force to Logic 0.,                LVDS mode.   ,Internal resistor disable.
            //89,Force to Logic 0.,                LVPECL mode. ,Internal 100 Ω resistor enable per output pin.
            //10:Normal mode (selection for DCLK).,LVDS mode.   ,Internal resistor disable.
            //80:Force to Logic 0.,                CML mode     ,Internal resistor disable.
            //81:Force to Logic 0.,                CML mode     ,Internal 100 Ω resistor enable per output pin.
            //82:Force to Logic 0.,                CML mode     ,Reserved.
            //83:Force to Logic 0.,                CML mode     ,Internal 50 Ω resistor enable per output pin.
            //08:Normal mode (selection for DCLK).,LVPECL mode. ,Internal resistor disable.
            //09;Normal mode (selection for DCLK).;LVPECL mode. ;Internal 100 Ω resistor enable per output pin.

            //sysref:LVPECL输出只有100mv(单端)过完00304有200mv(单端)
            //sysref:LVDS可以被00304正常识别,00304输出500mv(单端)
            new(0x00D0, 0x08), // DCLKOUT0  @0511 HXY 修改  PRO4_GT_CLK  250MHz  LVPECL 
            new(0x00DA, 0x08), // SCLKOUT1  @0511 HXY 修改  PRO3_GT_CLK  250MHz  LVPECL 
            new(0x00E4, 0x08), // DCLKOUT2  @0511 HXY 修改  PRO2_GT_CLK  250MHz  LVPECL  
            new(0x00EE, 0x08), // SCLKOUT3  @0511 HXY 修改  PRO1_GT_CLK  250MHz  LVPECL 
            new(0x00F8, 0x08), // DCLKOUT4  @0511 HXY 修改  ACQ3_GT_CLK  250MHz  LVPECL  
            new(0x0102, 0x08), // SCLKOUT5  @0511 HXY 修改  ADC3_SCLK    10MHz   LVPECL      
            new(0x010C, 0x08), // DCLKOUT6  @0511 HXY 修改  ADC1_SCLK    10MHz   LVPECL                                    
            new(0x0116, 0x08), // SCLKOUT7  @0511 HXY 修改  ACQ1_GT_CLK  250MHz  LVPECL
            new(0x0120, 0x08), // DCLKOUT8  @0511 HXY 修改  ACQ4_GT_CLK  250MHz  LVPECL 
            new(0x012A, 0x08), // SCLKOUT9  @0511 HXY 修改  ADC4_SCLK    10MHz   LVPECL
            new(0x0134, 0x08), // DCLKOUT10 @0511 HXY 修改  ADC2_SCLK    10MHz   LVPECL
            new(0x013E, 0x08), // SCLKOUT11 @0511 HXY 修改  ACQ2_GT_CLK  250MHz  LVPECL  
            new(0x0148, 0x08), // DCLKOUT12 @0511 HXY 修改  LA_GT_CLK2   250MHz  LVPECL  
            new(0x0152, 0x08), // SCLKOUT13 @0511 HXY 修改  LA_GT_CLK1   250MHz  LVPECL  
            //-----------------------------//
            // 		Input buffer
            //---------------------------//
            new(0x000A, 0x0B), // CLKIN0/RFSYNCIN   // 来自7044A lvpecl:09 lvds:07 lwq or:09
            new(0x000B, 0x07), // CLKIN1
            new(0x000C, 0x07), // CLKIN2
            new(0x000D, 0x0B), // CLKIN3   //0X03   // 来自7044A lwq
            new(0x000E, 0x07), // OSCIN
            //-----------------------------//
            // 		Other
            //---------------------------//
            new(0x0001, 0x02),
            new(0x0001, 0x00),
            new(0x0014, 0x27), //clkin_priority
            new(0x0015, 0x03),
            new(0x0016, 0x0C),
            new(0x0017, 0x00),
            new(0x0018, 0x04),
            new(0x0019, 0x03),
            new(0x001A, 0x08),
            new(0x001B, 0x18),
            new(0x001C, 0x01),
            new(0x001D, 0x01),
            new(0x001E, 0x01),
            new(0x001F, 0x01),
            new(0x0020, 0x0A),
            new(0x0021, 0x01),
            new(0x0022, 0x00),
            new(0x0026, 0x0A),
            new(0x0027, 0x00),
            new(0x0028, 0x13),
            new(0x0029, 0x07),
            new(0x002A, 0x0F),


        };
        protected PLLRegAddrValuePair[] SpecialConfigData7044B_Schame40G12BIT =
        {
            new(0x00FA, 0xB3), // SCLKOUT5   @0511 HXY 修改  ADC3_SCLK    10MHz   LVPECL
            new(0x0104, 0xB3), // DCLKOUT6   @0511 HXY 修改  ADC1_SCLK    10MHz   LVPECL
            new(0x0122, 0xB3), // SCLKOUT9   @0511 HXY 修改  ADC4_SCLK    10MHz   LVPECL
            new(0x012C, 0xB3), // DCLKOUT10  @0511 HXY 修改  ADC2_SCLK    10MHz   LVPECL

        };
        protected PLLRegAddrValuePair[] ConfigData7044C_Schame40G12BIT =
{
            //    地址 , 值
            /**********register_config***********/
            new(0x0000, 0x01),
            new(0x0000, 0x00),
            new(0x0001, 0x40),
            new(0x0002, 0x04),
            new(0x0003, 0x37),
            new(0x0004, 0x7F),
            new(0x0005, 0x98), // SYNC MODE (82、58) 98
            new(0x0006, 0x00),
            new(0x0007, 0x00),
            new(0x0009, 0x01),
            //-----------------------------//
            // 		保留寄存器
            //---------------------------//
            new(0x0096, 0x00),
            new(0x0097, 0x00),
            new(0x0098, 0x00),
            new(0x0099, 0x00),
            new(0x009A, 0x00),
            new(0x009B, 0xAA),
            new(0x009C, 0xAA),
            new(0x009D, 0xAA),
            new(0x009E, 0xAA),
            new(0x009F, 0x4D),
            new(0x00A0, 0xDF),
            new(0x00A1, 0x97),
            new(0x00A2, 0x03),
            new(0x00A3, 0x00),
            new(0x00A4, 0x00),
            new(0x00A5, 0x06),
            new(0x00A6, 0x1C),
            new(0x00A7, 0x00),
            new(0x00A8, 0x06),
            new(0x00A9, 0x00),
            new(0x00AB, 0x00),
            new(0x00AC, 0x20),
            new(0x00AD, 0x00),
            new(0x00AE, 0x08),
            new(0x00AF, 0x50),
            new(0x00B0, 0x04),
            new(0x00B1, 0x0D),
            new(0x00B2, 0x00),
            new(0x00B3, 0x00),
            new(0x00B5, 0x00),
            new(0x00B6, 0x00),
            new(0x00B7, 0x00),
            new(0x00B8, 0x00),
            //-----------------------------//
            // 		PLL2配置
            //---------------------------//
            new(0x0031, 0x01),
            new(0x0032, 0x01), // DOUBLE R
            new(0x0033, 0x01), // R2
            new(0x0034, 0x00),
            new(0x0035, 0x19), // N2
            new(0x0036, 0x00),
            new(0x0037, 0x0F),
            new(0x0038, 0x18),
            new(0x0039, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Path Control  输出分频器和路径使能
            new(0x003A, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Driver Control输出驱动配置
            new(0x003B, 0x00),//   @0408 HXY 添加注释    OSCOUTx/OSCOUTx Driver Control输出驱动配置
            //-----------------------------//
            // 		PLL1配置
            //---------------------------//
            new(0x0046, 0x00),
            new(0x0047, 0x00),
            new(0x0048, 0x08),
            new(0x0049, 0x10),
            new(0x0050, 0x1F),
            new(0x0051, 0x2B),
            new(0x0052, 0x37),
            //HMC7044read(0x0050, 0x7f),
            //new(0x0050, 0x7f),//1F
            //new(0x0051, 0x7f),
            //new(0x0052, 0x7f),

            new(0x0053, 0x33),
            new(0x0054, 0x03),
            //// significant start
            //new(3, 0x005B, 0x00),
            //new(3, 0x005C, 0x80),
            //new(3, 0x005D, 0x00),
            //// significant end
            new(0x0064, 0x00),
            new(0x0065, 0x00),
            new(0x0070, 0xE0), // alarm
            new(0x0071, 0x19),
            new(0x0078, 0x00),
            new(0x0079, 0x00),
            new(0x007A, 0x00),
            new(0x007B, 0x00),
            new(0x007C, 0x00),
            new(0x007D, 0x00),
            new(0x007E, 0x00),
            new(0x0082, 0x00),
            new(0x0083, 0x00),
            new(0x0084, 0x00),
            new(0x0085, 0x00),
            new(0x0086, 0x00),
            new(0x008C, 0x00),
            new(0x008D, 0x00),
            new(0x008E, 0x00),
            new(0x008F, 0x00),
            new(0x0091, 0x00),
            //-----------------------------//
            // 		Sysref Timer
            //---------------------------//
            new(0x005A, 0x01),//07
            new(0x005B, 0x04), // 04
            new(0x005C, 0x00), // [7:0]LSB
            new(0x005D, 0x02), // [3:0]MSB\\05// @0408 HXY 修改 0200 2500/512=4.88MHz
            //-----------------------------//
            // 		Clock Output Channel
            //---------------------------//
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            new(0x00C8, 0xF3), // DCLKOUT0   @0511 HXY 修改  ACQ7_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x00D2, 0xF3), // SCLKOUT1   @0511 HXY 修改  ADC5_SCLK            10MHz   LVPECL    
            new(0x00DC, 0x00), // DCLKOUT2   @0511 HXY 修改  BUS_VU9P_GT_CLK      250MHz  差分（100ΩNC）    
            new(0x00E6, 0xF3), // SCLKOUT3   @0511 HXY 修改  ACQ5_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x00F0, 0xF3), // DCLKOUT4   @0511 HXY 修改  PRO5_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x00FA, 0xF3), // SCLKOUT5   @0511 HXY 修改  PRO6_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x0104, 0xF3), // DCLKOUT6   @0511 HXY 修改  PRO8_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x010E, 0xF3), // SCLKOUT7   @0511 HXY 修改  PRO7_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x0118, 0xF3), // DCLKOUT8   @0511 HXY 修改  ACQ6_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x0122, 0xF3), // SCLKOUT9   @0511 HXY 修改  ADC6_SCLK            10MHz   LVPECL    
            new(0x012C, 0xF3), // DCLKOUT10  @0511 HXY 修改  ACQ8_GT_CLK          250MHz  差分（100ΩNC）    
            new(0x0136, 0xF3), // SCLKOUT11  @0511 HXY 修改  ADC8_SCLK            10MHz   LVPECL    
            new(0x0140, 0xF3), // DCLKOUT12  @0511 HXY 修改  ADC7_SCLK            10MHz   LVPECL    
            new(0x014A, 0xF3), // SCLKOUT13  @0511 HXY 修改  KU035_VU9P_GT_CLK    250MHz  差分（100ΩNC）    

            // Output divider
            // Even divide ratios from 2 to 4094
            // Odd divide ratios are 1、3、5
            new(0x00C9, 0x0A), // [7:0]LSB
            new(0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @0511 HXY 修改  ACQ7_GT_CLK          250MHz  差分（100ΩNC）     
            new(0x00D3, 0xFA), // [7:0]LSB
            new(0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @0511 HXY 修改  ADC5_SCLK            10MHz  LVPECL      

            new(0x00DD, 0x0A), // [7:0]LSB
            new(0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @0511 HXY 修改  BUS_VU9P_GT_CLK      250MHz  差分（100ΩNC）
            new(0x00E7, 0x0A), // [7:0]LSB
            new(0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @0511 HXY 修改  ACQ5_GT_CLK          250MHz  差分（100ΩNC）

            new(0x00F1, 0x0A), // [7:0]LSB
            new(0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @0511 HXY 修改  PRO5_GT_CLK          250MHz  差分（100ΩNC）  
            new(0x00FB, 0x0A), // [7:0]LSB
            new(0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @0511 HXY 修改  PRO6_GT_CLK          250MHz  差分（100ΩNC）  

            new(0x0105, 0x0A), // [7:0]LSB
            new(0x0106, 0x00), // [3:0]MSB DCLKOUT6 @0511 HXY 修改  PRO8_GT_CLK          250MHz  差分（100ΩNC）     
            new(0x010F, 0x0A), // [7:0]LSB                                            
            new(0x0110, 0x00), // [3:0]MSB SCLKOUT7 @0511 HXY 修改  PRO7_GT_CLK          250MHz   LVPECL     
            new(0x0119, 0x0A), // [7:0]LSB                                            
            new(0x011A, 0x00), // [3:0]MSB DCLKOUT8 @0511 HXY 修改  ACQ6_GT_CLK          250MHz  差分（100ΩNC）     
            new(0x0123, 0xFA), // [7:0]LSB                                            
            new(0x0124, 0x00), // [3:0]MSB SCLKOUT9 @0511 HXY 修改  ADC6_SCLK            10MHz   LVPECL     

            new(0x012D, 0x0A), // [7:0]LSB
            new(0x012E, 0x00), // [3:0]MSB DCLKOUT10 @0511 HXY 修改  ACQ8_GT_CLK          250MHz  差分（100ΩNC）   
            new(0x0137, 0xFA), // [7:0]LSB
            new(0x0138, 0x00), // [3:0]MSB SCLKOUT11 @0511 HXY 修改  ADC8_SCLK            10MHz   LVPECL    
            new(0x0141, 0xFA), // [7:0]LSB
            new(0x0142, 0x00), // [3:0]MSB DCLKOUT12 @0511 HXY 修改  ADC7_SCLK            10MHz   LVPECL   
            new(0x014B, 0x0A), // [7:0]LSB
            new(0x014C, 0x00), // [3:0]MSB SCLKOUT13 @0511 HXY 修改  KU035_VU9P_GT_CLK    250MHz  差分（100ΩNC）    

            // Fine analog delay
            // Step size 25ps
            // 0~23 effective
            new(0x00CB, 0x00), // DCLKOUT0 
            new(0x00DF, 0x00), // DCLKOUT2 
            new(0x00F3, 0x00), // DCLKOUT4  
            new(0x0107, 0x00), // DCLKOUT6 
            new(0x011B, 0x00), // DCLKOUT8
            new(0x012F, 0x00), // DCLKOUT10//02
            new(0x0143, 0x00), // DCLKOUT12//0F
            new(0x00D5, 0x00), // SCLKOUT1 //lwq0709 0x10
            new(0x00E9, 0x00), // SCLKOUT3//0F
            new(0x00FD, 0x00), // SCLKOUT5
            new(0x0111, 0x00), // SCLKOUT7
            new(0x0125, 0x00), // SCLKOUT9
            new(0x0139, 0x00), // SCLKOUT11
            new(0x014D, 0x00), // SCLKOUT13//02//04

            // Coarse digital deladelay
            // Step size 1/2 VCO cyclk//10G-100ps-50ps
            // 0~17 effective
            new(0x00CC, 0x00), // DCLKOUT0
            new(0x00D6, 0x00), // SCLKOUT1
            new(0x00E0, 0x00), // DCLKOUT2
            new(0x00EA, 0x00), // SCLKOUT3
            new(0x00F4, 0x00), // DCLKOUT4
            new(0x00FE, 0x00), // SCLKOUT5
            new(0x0108, 0x00), // DCLKOUT6
            new(0x0112, 0x00), // SCLKOUT7
            new(0x011C, 0x00), // DCLKOUT8
            new(0x0126, 0x00), // SCLKOUT9
            new(0x0130, 0x00), // DCLKOUT10
            new(0x013A, 0x00), // SCLKOUT11
            new(0x0144, 0x00), // DCLKOUT12
            new(0x014E, 0x00), // SCLKOUT13
            // Multislip digital delay
            // Step size : amount * VCO cycles
            new(0x00CD, 0x00), // [7:0]LSB
            new(0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new(0x00D7, 0x00), // [7:0]LSB
            new(0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new(0x00E1, 0x00), // [7:0]LSB
            new(0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new(0x00EB, 0x00), // [7:0]LSB
            new(0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new(0x00F5, 0x00), // [7:0]LSB
            new(0x00F6, 0x00), // [3:0]MSB DCLKOUT4
            new(0x00FF, 0x00), // [7:0]LSB
            new(0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new(0x0109, 0x00), // [7:0]LSB
            new(0x010A, 0x00), // [3:0]MSB DCLKOUT6
            new(0x0113, 0x00), // [7:0]LSB
            new(0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new(0x011D, 0x00), // [7:0]LSB
            new(0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new(0x0127, 0x00), // [7:0]LSB
            new(0x0128, 0x00), // [3:0]MSB SCLKOUT9
            new(0x0131, 0x00), // [7:0]LSB
            new(0x0132, 0x00), // [3:0]MSB DCLKOUT10
            new(0x013B, 0x00), // [7:0]LSB
            new(0x013C, 0x00), // [3:0]MSB SCLKOUT11
            new(0x0145, 0x00), // [7:0]LSB
            new(0x0146, 0x00), // [3:0]MSB DCLKOUT12
            new(0x014F, 0x00), // [7:0]LSB
            new(0x0150, 0x00), // [3:0]MSB SCLKOUT13
            // Output mux slelction
            new(0x00CF, 0x00), // DCLKOUT0
            new(0x00D9, 0x00), // SCLKOUT1
            new(0x00E3, 0x00), // DCLKOUT2
            new(0x00ED, 0x00), // SCLKOUT3
            new(0x00F7, 0x00), // DCLKOUT4
            new(0x0101, 0x00), // SCLKOUT5
            new(0x010B, 0x00), // DCLKOUT6
            new(0x0115, 0x00), // SCLKOUT7
            new(0x011F, 0x00), // DCLKOUT8
            new(0x0129, 0x00), // SCLKOUT9
            new(0x0133, 0x00), // DCLKOUT10
            new(0x013D, 0x00), // SCLKOUT11
            new(0x0147, 0x00), // DCLKOUT12
            new(0x0151, 0x00), // SCLKOUT13

            // Output driver
            //90:Force to Logic 0.,                LVDS mode.   ,Internal resistor disable.
            //89,Force to Logic 0.,                LVPECL mode. ,Internal 100 Ω resistor enable per output pin.
            //10:Normal mode (selection for DCLK).,LVDS mode.   ,Internal resistor disable.
            //80:Force to Logic 0.,                CML mode     ,Internal resistor disable.
            //81:Force to Logic 0.,                CML mode     ,Internal 100 Ω resistor enable per output pin.
            //82:Force to Logic 0.,                CML mode     ,Reserved.
            //83:Force to Logic 0.,                CML mode     ,Internal 50 Ω resistor enable per output pin.
            //08:Normal mode (selection for DCLK).,LVPECL mode. ,Internal resistor disable.
            //09;Normal mode (selection for DCLK).;LVPECL mode. ;Internal 100 Ω resistor enable per output pin.

            //sysref:LVPECL输出只有100mv(单端)过完00304有200mv(单端)
            //sysref:LVDS可以被00304正常识别,00304输出500mv(单端)
            new(0x00D0, 0x08), // DCLKOUT0    ACQ7_GT_CLK          250MHz  LVPECL 
            new(0x00DA, 0x08), // SCLKOUT1    ADC5_SCLK            10MHz   LVPECL    
            new(0x00E4, 0x08), // DCLKOUT2    BUS_VU9P_GT_CLK      250MHz  LVPECL 
            new(0x00EE, 0x08), // SCLKOUT3    ACQ5_GT_CLK          250MHz  LVPECL 
            new(0x00F8, 0x08), // DCLKOUT4    PRO5_GT_CLK          250MHz  LVPECL 
            new(0x0102, 0x08), // SCLKOUT5    PRO6_GT_CLK          250MHz  LVPECL       
            new(0x010C, 0x08), // DCLKOUT6    PRO8_GT_CLK          250MHz  LVPECL                                     
            new(0x0116, 0x08), // SCLKOUT7    PRO7_GT_CLK          250MHz  LVPECL 
            new(0x0120, 0x08), // DCLKOUT8    ACQ6_GT_CLK          250MHz  LVPECL 
            new(0x012A, 0x08), // SCLKOUT9    ADC6_SCLK            10MHz   LVPECL    
            new(0x0134, 0x08), // DCLKOUT10   ACQ8_GT_CLK          250MHz  LVPECL 
            new(0x013E, 0x08), // SCLKOUT11   ADC8_SCLK            10MHz   LVPECL    
            new(0x0148, 0x08), // DCLKOUT12   ADC7_SCLK            10MHz   LVPECL    
            new(0x0152, 0x08), // SCLKOUT13   KU035_VU9P_GT_CLK    LVPECL 
            //-----------------------------//
            // 		Input buffer
            //---------------------------//
            new(0x000A, 0x0B), // CLKIN0/RFSYNCIN   //来自7044A
            new(0x000B, 0x07), // CLKIN1
            new(0x000C, 0x07), // CLKIN2
            new(0x000D, 0x0B), // CLKIN3   //0X03   //来自7044A
            new(0x000E, 0x07), // OSCIN
            //-----------------------------//
            // 		Other
            //---------------------------//
            new(0x0001, 0x02),
            new(0x0001, 0x00),
            new(0x0014, 0x27), //clkin_priority
            new(0x0015, 0x03),
            new(0x0016, 0x0C),
            new(0x0017, 0x00),
            new(0x0018, 0x04),
            new(0x0019, 0x03),
            new(0x001A, 0x08),
            new(0x001B, 0x18),
            new(0x001C, 0x01),
            new(0x001D, 0x01),
            new(0x001E, 0x01),
            new(0x001F, 0x01),
            new(0x0020, 0x0A),
            new(0x0021, 0x01),
            new(0x0022, 0x00),
            new(0x0026, 0x0A),
            new(0x0027, 0x00),
            new(0x0028, 0x13),
            new(0x0029, 0x07),
            new(0x002A, 0x0F),


        };
        protected PLLRegAddrValuePair[] SpecialConfigData7044C_Schame40G12BIT =
        {
            new(0x00D2, 0xB3), // SCLKOUT1   @0511 HXY 修改  ADC5_SCLK            10MHz   LVPECL
            new(0x0122, 0xB3), // SCLKOUT9   @0511 HXY 修改  ADC6_SCLK            10MHz   LVPECL
            new(0x0136, 0xB3), // SCLKOUT11  @0511 HXY 修改  ADC8_SCLK            10MHz   LVPECL
            new(0x0140, 0xB3), // DCLKOUT12  @0511 HXY 修改  ADC7_SCLK            10MHz   LVPECL
        };
        //cij_jiuyuan_new_clk
        protected virtual void Init7044()
        {
            PLL7044Mode pll7044Mode = Hd.CurrProduct?.HardwareConfig?.PLL7044Mode ?? PLL7044Mode.SchemeA;

            PLLRegAddrValuePair[] currConfigAddrValuePairs = pll7044Mode switch
            {
#if JiHe_MSO8000X
                PLL7044Mode.Schame_JiHe_MSO8000X => Chip7044ConfigData_SchameJiHe8000X,
#endif
                _ => null,
            };
            if (currConfigAddrValuePairs == null)
                throw new ArgumentException("7044 Config Table Is Null!");
            foreach (PLLRegAddrValuePair regAddrValuePair in currConfigAddrValuePairs)
                PllWrite7044(regAddrValuePair.address, regAddrValuePair.value);
        }
        #endregion
        #region 7043 为采集板提供采样钟同步复位信号
        protected virtual void PllWrite7043(UInt32 addr, UInt32 data)
        {
#if !Product_B21_JinHui_PXI
            UInt32 temp = ((0x000 << 21) | (addr << 8) | data);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 0);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_L16, temp & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_H8, (temp >> 16) & 0xff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 1);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
#endif
        }
        protected PLLRegAddrValuePair[] Chip7043ConfigData_SchameA =
        {
            //    地址 , 值
            new(0x0000, 0x01),
            new(0x0000, 0x00),
            new(0x0001, 0x00),
            new(0x0002, 0x00),
            new(0x0003, 0x34),
            new(0x0004, 0x7F),
            new(0x0006, 0x00),
            new(0x0007, 0x00),
            new(0x000A, 0x07),
            new(0x000B, 0x07),
            new(0x0046, 0x00),
            new(0x0050, 0x28),
            new(0x0050, 0x2F),
            new(0x0050, 0x28),
            new(0x0054, 0x03),
            new(0x005A, 0x01), // PULSE NUM
            new(0x005B, 0x04),
            new(0x005C, 0x00), // PULSE CNT LSB
            new(0x005D, 0x02), // PULSE CNT HSB
            new(0x0064, 0x01),
            new(0x0065, 0x00),
            new(0x0071, 0x10),
            new(0x0078, 0x00),
            new(0x0079, 0x00),
            new(0x007A, 0x00),
            new(0x007D, 0x00),
            new(0x0091, 0x00),
            new(0x0098, 0x00),
            new(0x0099, 0x00),
            new(0x009A, 0x00),
            new(0x009B, 0xAA),
            new(0x009C, 0xAA),
            new(0x009D, 0xAA),
            new(0x009E, 0xAA),
            new(0x009F, 0x55),
            new(0x00A0, 0x56),
            new(0x00A1, 0x97),
            new(0x00A2, 0x03),
            new(0x00A3, 0x00),
            new(0x00A4, 0x00),
            new(0x00AD, 0x00),
            new(0x00AE, 0x08),
            new(0x00AF, 0x50),
            new(0x00B0, 0x09),
            new(0x00B1, 0x0D),
            new(0x00B2, 0x00),
            new(0x00B3, 0x00),
            new(0x00B5, 0x00),
            new(0x00B6, 0x00),
            new(0x00B7, 0x00),
            new(0x00B8, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
            // NC
            new(0x00D2, 0x00), // SCLKOUT1
            new(0x00E6, 0x00), // SCLKOUT3
            new(0x00FA, 0x00), // SCLKOUT5
            new(0x0104, 0x00), // DCLKOUT6
            new(0x010E, 0x00), // SCLKOUT7
            new(0x0122, 0x00), // SCLKOUT9
            // KEEP
            new(0x0136, 0xF3), // SCLKOUT11
            new(0x014A, 0xF3), // SCLKOUT13

            new(0x0118, 0xF3), // DCLKOUT8
            new(0x0140, 0xF3), // DCLKOUT12
            // Pluse
            new(0x00C8, 0x5D), // DCLKOUT0
            new(0x00DC, 0x5D), // DCLKOUT2
            new(0x00F0, 0x5D), // DCLKOUT4
            new(0x012C, 0x5D), // DCLKOUT10
            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new(0x00D3, 0x00), // [7:0]LSB
            new(0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @ NC
            new(0x00E7, 0x00), // [7:0]LSB
            new(0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ NC
            new(0x00FB, 0x00), // [7:0]LSB
            new(0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ NC
            new(0x0105, 0x00), // [7:0]LSB
            new(0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ NC
            new(0x010F, 0x00), // [7:0]LSB
            new(0x0110, 0x00), // [3:0]MSB SCLKOUT7 @ NC
            new(0x0119, 0xFA), // [7:0]LSB
            new(0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ NC
            new(0x0123, 0x00), // [7:0]LSB
            new(0x0124, 0x00), // [3:0]MSB SCLKOUT9 @ NC
            new(0x0141, 0xFA), // [7:0]LSB
            new(0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ NC
            // KEEP
            new(0x0137, 0x08), // [7:0]LSB
            new(0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ KEEP
            new(0x014B, 0x08), // [7:0]LSB
            new(0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ KEEP
            // Pluse
            new(0x00C9, 0x00), // [7:0]LSB
            new(0x00CA, 0x02), // [3:0]MSB DCLKOUT0 @ NC
            new(0x00DD, 0x00), // [7:0]LSB
            new(0x00DE, 0x02), // [3:0]MSB DCLKOUT2 @ NC
            new(0x00F1, 0x00), // [7:0]LSB
            new(0x00F2, 0x02), // [3:0]MSB DCLKOUT4 @ NC
            new(0x012D, 0x00), // [7:0]LSB
            new(0x012E, 0x02), // [3:0]MSB DCLKOUT10 @ NC
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE      
            new(0x00D5, 0x00), // SCLKOUT1  
            new(0x00E9, 0x00), // SCLKOUT3
            new(0x00FD, 0x00), // SCLKOUT5
            new(0x0107, 0x00), // DCLKOUT6
            new(0x0111, 0x00), // SCLKOUT7
            new(0x011B, 0x00), // DCLKOUT8
            new(0x0125, 0x00), // SCLKOUT9
            new(0x0143, 0x00), // DCLKOUT12
            // KEEP
            new(0x0139, 0x00), // SCLKOUT11
            new(0x014D, 0x00), // SCLKOUT13
            // Pluse
            new(0x00CB, 0x00), // DCLKOUT0
            new(0x00DF, 0x00), // DCLKOUT2
            new(0x00F3, 0x00), // DCLKOUT4
            new(0x012F, 0x00), // DCLKOUT10
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new(0x00D6, 0x00), // SCLKOUT1
            new(0x00EA, 0x00), // SCLKOUT3
            new(0x00FE, 0x00), // SCLKOUT5
            new(0x0108, 0x00), // DCLKOUT6
            new(0x0112, 0x00), // SCLKOUT7 
            new(0x011C, 0x00), // DCLKOUT8
            new(0x0126, 0x00), // SCLKOUT9
            new(0x0144, 0x00), // DCLKOUT12
            // KEEP
            new(0x013A, 0x00), // SCLKOUT11
            new(0x014E, 0x00), // SCLKOUT13
            // Pluse
            new(0x00CC, 0x00), // DCLKOUT0
            new(0x00E0, 0x00), // DCLKOUT2
            new(0x00F4, 0x00), // DCLKOUT4
            new(0x0130, 0x00), // DCLKOUT10
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new(0x00D7, 0x00), // [7:0]LSB
            new(0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new(0x00EB, 0x00), // [7:0]LSB
            new(0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new(0x00FF, 0x00), // [7:0]LSB
            new(0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new(0x0109, 0x00), // [7:0]LSB
            new(0x010A, 0x00), // [3:0]MSB DCLKOUT6  
            new(0x0113, 0x00), // [7:0]LSB
            new(0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new(0x011D, 0x00), // [7:0]LSB
            new(0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new(0x0127, 0x00), // [7:0]LSB
            new(0x0128, 0x00), // [3:0]MSB SCLKOUT9                 
            new(0x0145, 0x00), // [7:0]LSB
            new(0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // KEEP
            new(0x013B, 0x00), // [7:0]LSB
            new(0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new(0x014F, 0x00), // [7:0]LSB
            new(0x0150, 0x00), // [3:0]MSB SCLKOUT13 
            // Pluse
            new(0x00CD, 0x00), // [7:0]LSB
            new(0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new(0x00E1, 0x00), // [7:0]LSB
            new(0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new(0x00F5, 0x00), // [7:0]LSB
            new(0x00F6, 0x00), // [3:0]MSB DCLKOUT4 
            new(0x0131, 0x00), // [7:0]LSB
            new(0x0132, 0x00), // [3:0]MSB DCLKOUT10
            //-----------------------
            //      Output mux
            //-------------------------
            new(0x00CF, 0x01), // DCLKOUT0
            new(0x00D9, 0x01), // SCLKOUT1
            new(0x00E3, 0x01), // DCLKOUT2
            new(0x00ED, 0x01), // SCLKOUT3
            new(0x00F7, 0x01), // DCLKOUT4
            new(0x0101, 0x01), // SCLKOUT5
            new(0x010B, 0x01), // DCLKOUT6
            new(0x0115, 0x01), // SCLKOUT7
            new(0x011F, 0x01), // DCLKOUT8
            new(0x0129, 0x01), // SCLKOUT9
            new(0x0133, 0x01), // DCLKOUT10
            new(0x013D, 0x01), // SCLKOUT11
            new(0x0147, 0x01), // DCLKOUT12
            new(0x0151, 0x01), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new(0x00D0, 0x89), // DCLKOUT0
            new(0x00DA, 0x08), // SCLKOUT1
            new(0x00E4, 0x89), // DCLKOUT2
            new(0x00EE, 0x89), // SCLKOUT3
            new(0x00F8, 0x89), // DCLKOUT4
            new(0x0102, 0x89), // SCLKOUT5
            new(0x010C, 0x89), // DCLKOUT6
            new(0x0116, 0x08), // SCLKOUT7
            new(0x0120, 0x09), // DCLKOUT8
            new(0x012A, 0x08), // SCLKOUT9
            new(0x0134, 0x89), // DCLKOUT10
            new(0x013E, 0x09), // SCLKOUT11
            new(0x0148, 0x09), // DCLKOUT12
            new(0x0152, 0x09), // SCLKOUT13


            // test
            new(0x14A, 0x51),
            new(0x14F, 0x00),
            new(0x150, 0x00),
            new(0x14B, 0x08),
            new(0x14C, 0x00),
            new(0x14E, 0x00),
            new(0x14D, 0x00),
            new(0x151, 0x01),
            new(0x152, 0x89),

            new(0x136, 0x51),
            new(0x13B, 0x00),
            new(0x13C, 0x00),
            new(0x137, 0x08),
            new(0x138, 0x00),
            new(0x13A, 0x00),
            new(0x139, 0x00),
            new(0x13D, 0x01),
            new(0x13E, 0x89),

            //-----------------------
            //      重启分频器
            //-------------------------
            new(0001, 0x02),
            new(0001, 0x00), // 重启分频器

        };
        protected PLLRegAddrValuePair[] Chip7043ConfigData_SchameB =
            {
            //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x00),
            new (0x0003, 0x34),
            new (0x0004, 0x7F),
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x000A, 0x07),
            new (0x000B, 0x07),
            new (0x0046, 0x00),
            new (0x0050, 0x28),
            new (0x0050, 0x2F),
            new (0x0050, 0x28),
            new (0x0054, 0x03),
            new (0x005A, 0x01), // PULSE NUM
            new (0x005B, 0x04),
            new (0x005C, 0x00), // PULSE CNT LSB
            new (0x005D, 0x02), // PULSE CNT HSB
            new (0x0064, 0x01),
            new (0x0065, 0x00),
            new (0x0071, 0x10),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007D, 0x00),
            new (0x0091, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x55),
            new (0x00A0, 0x56),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x09),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
            new (0x00C8, 0x5D), // DCLKOUT0
            new (0x00D2, 0xF3), // SCLKOUT1  //acq 7044 clk
            new (0x00DC, 0xF3), // DCLKOUT2  //acq 7044 clk
            new (0x00E6, 0x00), // SCLKOUT3
            new (0x00F0, 0xF3), // DCLKOUT4  //acq 7044 clk
            new (0x00FA, 0x00), // SCLKOUT5
            new (0x0104, 0x00), // DCLKOUT6
            new (0x010E, 0x00), // SCLKOUT7
            new (0x0118, 0xF3), // DCLKOUT8
            new (0x0122, 0x00), // SCLKOUT9
            new (0x012C, 0x5D), // DCLKOUT10 
            new (0x0136, 0x00), // SCLKOUT11
            new (0x0140, 0xF3), // DCLKOUT12 //acq 7044 clk
            new (0x014A, 0x00), // SCLKOUT13
            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new (0x00C9, 0x00), // [7:0]LSB
            new (0x00CA, 0x02), // [3:0]MSB DCLKOUT0 @ NC
            new (0x00D3, 0xFA), // [7:0]LSB
            new (0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @ acq 7044 clk
            new (0x00DD, 0xFA), // [7:0]LSB
            new (0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @ acq 7044 clk
            new (0x00E7, 0x00), // [7:0]LSB
            new (0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ NC
            new (0x00F1, 0xFA), // [7:0]LSB 
            new (0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @ acq 7044 clk
            new (0x00FB, 0x00), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ NC
            new (0x0105, 0x00), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ NC
            new (0x010F, 0x00), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 @ NC
            new (0x0119, 0xFA), // [7:0]LSB
            new (0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ NC
            new (0x0123, 0x00), // [7:0]LSB
            new (0x0124, 0x00), // [3:0]MSB SCLKOUT9 @ NC
            new (0x012D, 0x00), // [7:0]LSB
            new (0x012E, 0x02), // [3:0]MSB DCLKOUT10 @ NC
            new (0x0137, 0x00), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ KEEP
            new (0x0141, 0xFA), // [7:0]LSB
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ acq 7044 clk
            new (0x014B, 0x00), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ KEEP
            // KEEP
            // Pluse
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE      
            new (0x00D5, 0x00), // SCLKOUT1  
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0111, 0x00), // SCLKOUT7
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0125, 0x00), // SCLKOUT9
            new (0x0143, 0x00), // DCLKOUT12
            // KEEP
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CB, 0x00), // DCLKOUT0
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x012F, 0x00), // DCLKOUT10
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0112, 0x00), // SCLKOUT7 
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0144, 0x00), // DCLKOUT12
            // KEEP
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4
            new (0x0130, 0x00), // DCLKOUT10
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6  
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9                 
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // KEEP
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13 
            // Pluse
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4 
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x01), // DCLKOUT0
            new (0x00D9, 0x01), // SCLKOUT1
            new (0x00E3, 0x01), // DCLKOUT2
            new (0x00ED, 0x01), // SCLKOUT3
            new (0x00F7, 0x01), // DCLKOUT4
            new (0x0101, 0x01), // SCLKOUT5
            new (0x010B, 0x01), // DCLKOUT6
            new (0x0115, 0x01), // SCLKOUT7
            new (0x011F, 0x01), // DCLKOUT8
            new (0x0129, 0x01), // SCLKOUT9
            new (0x0133, 0x01), // DCLKOUT10
            new (0x013D, 0x01), // SCLKOUT11
            new (0x0147, 0x01), // DCLKOUT12
            new (0x0151, 0x01), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new (0x00D0, 0x89), // DCLKOUT0
            new (0x00DA, 0x09), // SCLKOUT1 @ acq 7044 clk
            new (0x00E4, 0x09), // DCLKOUT2 @ acq 7044 clk
            new (0x00EE, 0x89), // SCLKOUT3
            new (0x00F8, 0x09), // DCLKOUT4 @ acq 7044 clk
            new (0x0102, 0x89), // SCLKOUT5
            new (0x010C, 0x89), // DCLKOUT6
            new (0x0116, 0x08), // SCLKOUT7
            new (0x0120, 0x09), // DCLKOUT8
            new (0x012A, 0x08), // SCLKOUT9
            new (0x0134, 0x89), // DCLKOUT10
            new (0x013E, 0x08), // SCLKOUT11
            new (0x0148, 0x09), // DCLKOUT12 @ acq 7044 clk
            new (0x0152, 0x89), // SCLKOUT13
            //-----------------------
            //      重启分频器
            //-------------------------
            new (0001, 0x02),
            new (0001, 0x00), // 重启分频器

        };
        protected PLLRegAddrValuePair[] Chip7043ConfigData_SchameC =
{
            //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x00),
            new (0x0003, 0x34),
            new (0x0004, 0x7F),
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x000A, 0x07),
            new (0x000B, 0x07),
            new (0x0046, 0x00),
            new (0x0050, 0x28),
            new (0x0050, 0x2F),
            new (0x0050, 0x28),
            new (0x0054, 0x03),
            new (0x005A, 0x01), // PULSE NUM
            new (0x005B, 0x04),
            new (0x005C, 0x00), // PULSE CNT LSB
            new (0x005D, 0x02), // PULSE CNT HSB
            new (0x0064, 0x01),
            new (0x0065, 0x00),
            new (0x0071, 0x10),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007D, 0x00),
            new (0x0091, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x55),
            new (0x00A0, 0x56),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x09),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
            new (0x00C8, 0xF3), // DCLKOUT0  //acq 7044 clk
            new (0x00D2, 0xF3), // SCLKOUT1  //acq 7044 clk
            new (0x00DC, 0xF3), // DCLKOUT2  //acq 7044 clk
            new (0x00E6, 0xF3), // SCLKOUT3  //acq 7044 clk
            new (0x00F0, 0xF3), // DCLKOUT4  //acq 7044 clk
            new (0x00FA, 0xF3), // SCLKOUT5  //acq 7044 clk
            new (0x0104, 0xF3), // DCLKOUT6  //acq 7044 clk
            new (0x010E, 0x00), // SCLKOUT7
            new (0x0118, 0xF3), // DCLKOUT8  //acq 7044 clk
            new (0x0122, 0x00), // SCLKOUT9
            new (0x012C, 0xF3), // DCLKOUT10 //acq 7044 clk
            new (0x0136, 0x00), // SCLKOUT11
            new (0x0140, 0xF3), // DCLKOUT12 //acq 7044 clk
            new (0x014A, 0x00), // SCLKOUT13
            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new (0x00C9, 0xFA), // [7:0]LSB
            new (0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @ acq 7044 clk
            new (0x00D3, 0xFA), // [7:0]LSB
            new (0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @ acq 7044 clk
            new (0x00DD, 0xFA), // [7:0]LSB
            new (0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @ acq 7044 clk
            new (0x00E7, 0xFA), // [7:0]LSB
            new (0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ acq 7044 clk
            new (0x00F1, 0xFA), // [7:0]LSB 
            new (0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @ acq 7044 clk
            new (0x00FB, 0xFA), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ acq 7044 clk
            new (0x0105, 0xFA), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ acq 7044 clk
            new (0x010F, 0x00), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 @ NC
            new (0x0119, 0xFA), // [7:0]LSB
            new (0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ acq 7044 clk
            new (0x0123, 0x00), // [7:0]LSB
            new (0x0124, 0x00), // [3:0]MSB SCLKOUT9 @ NC
            new (0x012D, 0xFA), // [7:0]LSB
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @ acq 7044 clk
            new (0x0137, 0x00), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ KEEP
            new (0x0141, 0xFA), // [7:0]LSB
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ acq 7044 clk
            new (0x014B, 0x00), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ KEEP
            // KEEP
            // Pluse
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE      
            new (0x00D5, 0x00), // SCLKOUT1  
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0111, 0x00), // SCLKOUT7
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0125, 0x00), // SCLKOUT9
            new (0x0143, 0x00), // DCLKOUT12
            // KEEP
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CB, 0x00), // DCLKOUT0
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x012F, 0x00), // DCLKOUT10
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0112, 0x00), // SCLKOUT7 
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0144, 0x00), // DCLKOUT12
            // KEEP
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4
            new (0x0130, 0x00), // DCLKOUT10
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6  
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9                 
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // KEEP
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13 
            // Pluse
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4 
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x01), // DCLKOUT0
            new (0x00D9, 0x01), // SCLKOUT1
            new (0x00E3, 0x01), // DCLKOUT2
            new (0x00ED, 0x01), // SCLKOUT3
            new (0x00F7, 0x01), // DCLKOUT4
            new (0x0101, 0x01), // SCLKOUT5
            new (0x010B, 0x01), // DCLKOUT6
            new (0x0115, 0x01), // SCLKOUT7
            new (0x011F, 0x01), // DCLKOUT8
            new (0x0129, 0x01), // SCLKOUT9
            new (0x0133, 0x01), // DCLKOUT10
            new (0x013D, 0x01), // SCLKOUT11
            new (0x0147, 0x01), // DCLKOUT12
            new (0x0151, 0x01), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new (0x00D0, 0x89), // DCLKOUT0 @ acq 7044 clk
            new (0x00DA, 0x89), // SCLKOUT1 @ acq 7044 clk
            new (0x00E4, 0x89), // DCLKOUT2 @ acq 7044 clk
            new (0x00EE, 0x89), // SCLKOUT3 @ acq 7044 clk
            new (0x00F8, 0x89), // DCLKOUT4 @ acq 7044 clk
            new (0x0102, 0x89), // SCLKOUT5 @ acq 7044 clk
            new (0x010C, 0x89), // DCLKOUT6 @ acq 7044 clk
            new (0x0116, 0x08), // SCLKOUT7
            new (0x0120, 0x89), // DCLKOUT8 @ acq 7044 clk
            new (0x012A, 0x08), // SCLKOUT9
            new (0x0134, 0x89), // DCLKOUT10 @ acq 7044 clk
            new (0x013E, 0x08), // SCLKOUT11
            new (0x0148, 0x89), // DCLKOUT12 @ acq 7044 clk
            new (0x0152, 0x89), // SCLKOUT13
            //-----------------------
            //      重启分频器
            //-------------------------
            new (0001, 0x02),
            new (0001, 0x00), // 重启分频器

        };
        protected PLLRegAddrValuePair[] Chip7043ConfigData_Schame_JiHe_MSO7000X =
{
            //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x00),
            new (0x0003, 0x34),
            new (0x0004, 0x7F),
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x000A, 0x07),
            new (0x000B, 0x07),
            new (0x0046, 0x00),
            new (0x0050, 0x28),
            new (0x0050, 0x2F),
            new (0x0050, 0x28),
            new (0x0054, 0x03),
            new (0x005A, 0x01), // PULSE NUM
            new (0x005B, 0x04),
            new (0x005C, 0x00), // PULSE CNT LSB
            new (0x005D, 0x02), // PULSE CNT HSB
            new (0x0064, 0x01),
            new (0x0065, 0x00),
            new (0x0071, 0x10),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007D, 0x00),
            new (0x0091, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x55),
            new (0x00A0, 0x56),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x09),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
            new (0x00C8, 0xF3), // DCLKOUT0  //TRIG
            new (0x00D2, 0xF3), // SCLKOUT1  
            new (0x00DC, 0xF3), // DCLKOUT2  
            new (0x00E6, 0xF3), // SCLKOUT3  
            new (0x00F0, 0xF3), // DCLKOUT4  
            new (0x00FA, 0xF3), // SCLKOUT5  
            new (0x0104, 0xF3), // DCLKOUT6  //PCIE
            new (0x010E, 0xF3), // SCLKOUT7
            new (0x0118, 0xF3), // DCLKOUT8  //PRO 7043 clk
            new (0x0122, 0xF3), // SCLKOUT9
            new (0x012C, 0xF3), // DCLKOUT10 //ADC1 GTHREFCLK
            new (0x0136, 0xF3), // SCLKOUT11 //ADC2 GTHREFCLK
            new (0x0140, 0xF3), // DCLKOUT12 //ADC4 GTHREFCLK
            new (0x014A, 0xF3), // SCLKOUT13 //ADC3 GTHREFCLK
            //-----------------------
            //      Output 分频
            //-------------------------
            // PRO PCIE
            new (0x00C9, 0xFA), // [7:0]LSB
            new (0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @ TRIG
            new (0x00D3, 0xFA), // [7:0]LSB
            new (0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @ NC
            new (0x00DD, 0xFA), // [7:0]LSB
            new (0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @ NC
            new (0x00E7, 0xFA), // [7:0]LSB
            new (0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ NC
            new (0x00F1, 0xFA), // [7:0]LSB 
            new (0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @ NC
            new (0x00FB, 0xFA), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ NC
            new (0x0105, 0xFA), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ PCIE
            new (0x010F, 0x08), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 @ NC
            new (0x0119, 0x08), // [7:0]LSB
            new (0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ PRO 7043 clk
            new (0x0123, 0x08), // [7:0]LSB
            new (0x0124, 0x00), // [3:0]MSB SCLKOUT9 @ NC
            new (0x012D, 0x02), // [7:0]LSB
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @ ADC1 GTHREFCLK 312.5MHz
            new (0x0137, 0x02), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ ADC2 GTHREFCLK 312.5MHz
            new (0x0141, 0x02), // [7:0]LSB
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ ADC4 GTHREFCLK 312.5MHz
            new (0x014B, 0x02), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ ADC3 GTHREFCLK 312.5MHz
            // KEEP
            // Pluse
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE      
            new (0x00D5, 0x00), // SCLKOUT1  
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0111, 0x00), // SCLKOUT7
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0125, 0x00), // SCLKOUT9
            new (0x0143, 0x00), // DCLKOUT12
            // KEEP
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CB, 0x00), // DCLKOUT0
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x012F, 0x00), // DCLKOUT10
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0112, 0x00), // SCLKOUT7 
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0144, 0x00), // DCLKOUT12
            // KEEP
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4
            new (0x0130, 0x00), // DCLKOUT10
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6  
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9                 
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // KEEP
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13 
            // Pluse
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4 
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x01), // DCLKOUT0
            new (0x00D9, 0x01), // SCLKOUT1
            new (0x00E3, 0x01), // DCLKOUT2
            new (0x00ED, 0x01), // SCLKOUT3
            new (0x00F7, 0x01), // DCLKOUT4
            new (0x0101, 0x01), // SCLKOUT5
            new (0x010B, 0x01), // DCLKOUT6
            new (0x0115, 0x01), // SCLKOUT7
            new (0x011F, 0x01), // DCLKOUT8
            new (0x0129, 0x01), // SCLKOUT9
            new (0x0133, 0x00), // DCLKOUT10
            new (0x013D, 0x00), // SCLKOUT11
            new (0x0147, 0x00), // DCLKOUT12
            new (0x0151, 0x00), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new (0x00D0, 0x89), // DCLKOUT0 @ acq 7044 clk
            new (0x00DA, 0x89), // SCLKOUT1 @ acq 7044 clk
            new (0x00E4, 0x89), // DCLKOUT2 @ acq 7044 clk
            new (0x00EE, 0x89), // SCLKOUT3 @ acq 7044 clk
            new (0x00F8, 0x89), // DCLKOUT4 @ acq 7044 clk
            new (0x0102, 0x89), // SCLKOUT5 @ acq 7044 clk
            new (0x010C, 0x89), // DCLKOUT6 @ acq 7044 clk
            new (0x0116, 0x08), // SCLKOUT7
            new (0x0120, 0x89), // DCLKOUT8 @ acq 7044 clk
            new (0x012A, 0x08), // SCLKOUT9
            new (0x0134, 0x01), // DCLKOUT10 CML 100ohm
            new (0x013E, 0x01), // SCLKOUT11 CML 100ohm
            new (0x0148, 0x01), // DCLKOUT12 CML 100ohm
            new (0x0152, 0x01), // SCLKOUT13 CML 100ohm
            //-----------------------
            //      重启分频器
            //-------------------------
            new (0001, 0x02),
            new (0001, 0x00), // 重启分频器

        };

        protected PLLRegAddrValuePair[] Chip7043ConfigData_Schame_JiHe_MSO8000X =
        {
             //    地址 , 值
            new (0x0000, 0x01),
            new (0x0000, 0x00),
            new (0x0001, 0x00),
            new (0x0002, 0x00),
            new (0x0003, 0x34),
            new (0x0004, 0x7F),
            new (0x0006, 0x00),
            new (0x0007, 0x00),
            new (0x000A, 0x07),
            new (0x000B, 0x07),
            new (0x0046, 0x00),
            new (0x0050, 0x28),
            new (0x0050, 0x2F),
            new (0x0050, 0x28),
            new (0x0054, 0x03),
            new (0x005A, 0x01), // PULSE NUM
            new (0x005B, 0x04),
            new (0x005C, 0x00), // PULSE CNT LSB
            new (0x005D, 0x02), // PULSE CNT HSB
            new (0x0064, 0x01),
            new (0x0065, 0x00),
            new (0x0071, 0x10),
            new (0x0078, 0x00),
            new (0x0079, 0x00),
            new (0x007A, 0x00),
            new (0x007D, 0x00),
            new (0x0091, 0x00),
            new (0x0098, 0x00),
            new (0x0099, 0x00),
            new (0x009A, 0x00),
            new (0x009B, 0xAA),
            new (0x009C, 0xAA),
            new (0x009D, 0xAA),
            new (0x009E, 0xAA),
            new (0x009F, 0x55),
            new (0x00A0, 0x56),
            new (0x00A1, 0x97),
            new (0x00A2, 0x03),
            new (0x00A3, 0x00),
            new (0x00A4, 0x00),
            new (0x00AD, 0x00),
            new (0x00AE, 0x08),
            new (0x00AF, 0x50),
            new (0x00B0, 0x09),
            new (0x00B1, 0x0D),
            new (0x00B2, 0x00),
            new (0x00B3, 0x00),
            new (0x00B5, 0x00),
            new (0x00B6, 0x00),
            new (0x00B7, 0x00),
            new (0x00B8, 0x00),
            //-------------------------------------------------
            //                  通道设置
            //--------------------------------------------
            //-----------------------
            //      Output 模式
            //-------------------------
            new (0x00C8, 0x00), // DCLKOUT0  //LA_CLK4
            new (0x00D2, 0x00), // SCLKOUT1  //LA_CLK3
            new (0x00DC, 0x00), // DCLKOUT2  //LA_CLK2
            new (0x00E6, 0x00), // SCLKOUT3  //LA_CLK1
            new (0x00F0, 0xF3), // DCLKOUT4  //acq5 7044 clk_10M
            new (0x00FA, 0x00), // SCLKOUT5  //VU9P_TRIG
            new (0x0104, 0xF3), // DCLKOUT6  //acq6 7044 clk_10M
            new (0x010E, 0xF3), // SCLKOUT7  //acq4 7044 clk_10M
            new (0x0118, 0xF3), // DCLKOUT8  //acq2 7044 clk_10M
            new (0x0122, 0x00), // SCLKOUT9  //DBI_clk
            new (0x012C, 0xF3), // DCLKOUT10 //PRO_KU035_ADC1_GTH_REFCLK 
            new (0x0136, 0xF3), // SCLKOUT11 //acq3 7044 clk_10M  
            new (0x0140, 0x00), // DCLKOUT12 //pcie_pro_gt1
            new (0x014A, 0xF3), // SCLKOUT13 //PRO_KU035_ADC3_GTH_REFCLK
            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new (0x00C9, 0xFA), // [7:0]LSB
            new (0x00CA, 0x00), // [3:0]MSB DCLKOUT0 @ LA_CLK4
            new (0x00D3, 0xFA), // [7:0]LSB
            new (0x00D4, 0x00), // [3:0]MSB SCLKOUT1 @ LA_CLK3
            new (0x00DD, 0xFA), // [7:0]LSB
            new (0x00DE, 0x00), // [3:0]MSB DCLKOUT2 @ LA_CLK2
            new (0x00E7, 0xFA), // [7:0]LSB
            new (0x00E8, 0x00), // [3:0]MSB SCLKOUT3 @ LA_CLK1
            new (0x00F1, 0xFA), // [7:0]LSB 
            new (0x00F2, 0x00), // [3:0]MSB DCLKOUT4 @ acq5 7044 clk_10M
            new (0x00FB, 0xFA), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ VU9P_TRIG
            new (0x0105, 0xFA), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ acq6 7044 clk_10M
            new (0x010F, 0xFA), // [7:0]LSB
            new (0x0110, 0x00), // [3:0]MSB SCLKOUT7 @  acq4 7044 clk_10M
            new (0x0119, 0xFA), // [7:0]LSB
            new (0x011A, 0x00), // [3:0]MSB DCLKOUT8 @ acq2 7044 clk_10M
            new (0x0123, 0x00), // [7:0]LSB
            new (0x0124, 0x00), // [3:0]MSB SCLKOUT9 @ DBI_clk
            new (0x012D, 0x01), // [7:0]LSB 8(312.5MHz)  2024/04/15 zm改
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @ PRO_KU035_ADC1_GTH_REFCLK 
            new (0x0137, 0xFA), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ acq3 7044 clk_10M  
            new (0x0141, 0xFA), // [7:0]LSB
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ pcie_pro_gt1
            new (0x014B, 0x01), // [7:0]LSB 8(312.5MHz)  2024/04/15 zm改
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ PRO_KU035_ADC3_GTH_REFCLK 
            // KEEP
            // Pluse
            //-----------------------
            //      模拟延迟
            //-------------------------
            // NO USE      
            new (0x00D5, 0x00), // SCLKOUT1  
            new (0x00E9, 0x00), // SCLKOUT3
            new (0x00FD, 0x00), // SCLKOUT5
            new (0x0107, 0x00), // DCLKOUT6
            new (0x0111, 0x00), // SCLKOUT7
            new (0x011B, 0x00), // DCLKOUT8
            new (0x0125, 0x00), // SCLKOUT9
            new (0x0143, 0x00), // DCLKOUT12
            // KEEP
            new (0x0139, 0x00), // SCLKOUT11
            new (0x014D, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CB, 0x00), // DCLKOUT0
            new (0x00DF, 0x00), // DCLKOUT2
            new (0x00F3, 0x00), // DCLKOUT4
            new (0x012F, 0x00), // DCLKOUT10
            //-----------------------
            //      数字延迟
            //-------------------------
            // NO USE
            new (0x00D6, 0x00), // SCLKOUT1
            new (0x00EA, 0x00), // SCLKOUT3
            new (0x00FE, 0x00), // SCLKOUT5
            new (0x0108, 0x00), // DCLKOUT6
            new (0x0112, 0x00), // SCLKOUT7 
            new (0x011C, 0x00), // DCLKOUT8
            new (0x0126, 0x00), // SCLKOUT9
            new (0x0144, 0x00), // DCLKOUT12
            // KEEP
            new (0x013A, 0x00), // SCLKOUT11
            new (0x014E, 0x00), // SCLKOUT13
            // Pluse
            new (0x00CC, 0x00), // DCLKOUT0
            new (0x00E0, 0x00), // DCLKOUT2
            new (0x00F4, 0x00), // DCLKOUT4
            new (0x0130, 0x00), // DCLKOUT10
            //-----------------------
            //      多跳数字延迟
            //-------------------------
            // NO USE
            new (0x00D7, 0x00), // [7:0]LSB
            new (0x00D8, 0x00), // [3:0]MSB SCLKOUT1
            new (0x00EB, 0x00), // [7:0]LSB
            new (0x00EC, 0x00), // [3:0]MSB SCLKOUT3
            new (0x00FF, 0x00), // [7:0]LSB
            new (0x0100, 0x00), // [3:0]MSB SCLKOUT5
            new (0x0109, 0x00), // [7:0]LSB
            new (0x010A, 0x00), // [3:0]MSB DCLKOUT6  
            new (0x0113, 0x00), // [7:0]LSB
            new (0x0114, 0x00), // [3:0]MSB SCLKOUT7
            new (0x011D, 0x00), // [7:0]LSB
            new (0x011E, 0x00), // [3:0]MSB DCLKOUT8
            new (0x0127, 0x00), // [7:0]LSB
            new (0x0128, 0x00), // [3:0]MSB SCLKOUT9                 
            new (0x0145, 0x00), // [7:0]LSB
            new (0x0146, 0x00), // [3:0]MSB DCLKOUT12
            // KEEP
            new (0x013B, 0x00), // [7:0]LSB
            new (0x013C, 0x00), // [3:0]MSB SCLKOUT11    
            new (0x014F, 0x00), // [7:0]LSB
            new (0x0150, 0x00), // [3:0]MSB SCLKOUT13 
            // Pluse
            new (0x00CD, 0x00), // [7:0]LSB
            new (0x00CE, 0x00), // [3:0]MSB DCLKOUT0
            new (0x00E1, 0x00), // [7:0]LSB
            new (0x00E2, 0x00), // [3:0]MSB DCLKOUT2
            new (0x00F5, 0x00), // [7:0]LSB
            new (0x00F6, 0x00), // [3:0]MSB DCLKOUT4 
            new (0x0131, 0x00), // [7:0]LSB
            new (0x0132, 0x00), // [3:0]MSB DCLKOUT10
            //-----------------------
            //      Output mux
            //-------------------------
            new (0x00CF, 0x01), // DCLKOUT0
            new (0x00D9, 0x01), // SCLKOUT1
            new (0x00E3, 0x01), // DCLKOUT2
            new (0x00ED, 0x01), // SCLKOUT3
            new (0x00F7, 0x01), // DCLKOUT4
            new (0x0101, 0x01), // SCLKOUT5
            new (0x010B, 0x01), // DCLKOUT6
            new (0x0115, 0x01), // SCLKOUT7
            new (0x011F, 0x01), // DCLKOUT8
            new (0x0129, 0x01), // SCLKOUT9
            new (0x0133, 0x01), // DCLKOUT10
            new (0x013D, 0x01), // SCLKOUT11
            new (0x0147, 0x01), // DCLKOUT12
            new (0x0151, 0x01), // SCLKOUT13
            //-----------------------
            //      输出驱动
            //-------------------------
            new (0x00D0, 0x08), // DCLKOUT0 @ acq 7044 clk
            new (0x00DA, 0x89), // SCLKOUT1 @ acq 7044 clk
            new (0x00E4, 0x89), // DCLKOUT2 @ acq 7044 clk
            new (0x00EE, 0x89), // SCLKOUT3 @ acq 7044 clk
            new (0x00F8, 0x09), // DCLKOUT4 @ acq 7044 clk
            new (0x0102, 0x89), // SCLKOUT5 @ acq 7044 clk
            new (0x010C, 0x89), // DCLKOUT6 @ acq 7044 clk
            new (0x0116, 0x89), // SCLKOUT7
            new (0x0120, 0x89), // DCLKOUT8 @ acq 7044 clk
            new (0x012A, 0x08), // SCLKOUT9
            new (0x0134, 0x89), // DCLKOUT10 @ acq 7044 clk
            new (0x013E, 0x89), // SCLKOUT11
            new (0x0148, 0x89), // DCLKOUT12 @ acq 7044 clk
            new (0x0152, 0x89), // SCLKOUT13
            //-----------------------
            //      重启分频器
            //-------------------------
            new (0001, 0x02),
            new (0001, 0x00), // 重启分频器
        };

        protected virtual void Init7043()
        {
            PLL7043Mode pll7043Mode = Hd.CurrProduct?.HardwareConfig?.PLL7043Mode ?? PLL7043Mode.SchemeA;
            PLLRegAddrValuePair[] currConfigAddrValuePairs = pll7043Mode switch
            {
                PLL7043Mode.SchemeB => Chip7043ConfigData_SchameB,
                PLL7043Mode.SchemeC => Chip7043ConfigData_SchameC,
                PLL7043Mode.Schame_JiHe_MSO7000X => Chip7043ConfigData_Schame_JiHe_MSO7000X,
                PLL7043Mode.Schame_JiHe_MSO8000X => Chip7043ConfigData_Schame_JiHe_MSO8000X,
                _ => Chip7043ConfigData_SchameA,
            };
            foreach (PLLRegAddrValuePair regAddrValuePair in currConfigAddrValuePairs)
                PllWrite7043(regAddrValuePair.address, regAddrValuePair.value);
        }
        #endregion


        private AnaChnlClkSrc? _LastClkSrc;
        /// <summary>
        /// 设置7044 Pll的10M参考时钟的输入
        /// </summary>
        /// <param name="clkSrc">时钟源类型</param>
        public virtual void SetPll10MSyncClk(AnaChnlClkSrc clkSrc)
        {
            if (_LastClkSrc == clkSrc)
                return;
            _LastClkSrc = clkSrc;

            HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Ext_10m_Sel, clkSrc == AnaChnlClkSrc.Inner ? 0U : 1U);

            (UInt32 addr05Value, UInt32 addr14Value) Value = clkSrc switch
            {
                AnaChnlClkSrc.Outter => (0x82, 0x55),
                _ => (0x81, 0x00),   //默认使用内部时钟
            };
            PllWrite7044(0x0005, Value.addr05Value);
            PllWrite7044(0x0014, Value.addr14Value);
        }
    }
}
