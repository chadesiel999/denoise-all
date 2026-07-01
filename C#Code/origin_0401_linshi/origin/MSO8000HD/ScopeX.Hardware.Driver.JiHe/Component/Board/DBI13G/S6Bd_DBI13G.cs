using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class S6Bd_DBI13G : AbstractS6Bd
    {
        internal S6Bd_DBI13G()
        {
            _Chip7044ConfigData = ConfigData7044_SchameC;
            _Chip7044ConfigDataSpecial = SpecialConfigData7044_SchameC;

            _Chip7043ConfigData = ConfigData7043_SchameC;
            _Chip7043ConfigDataSpecail = SpecialConfigData7043_SchameC;
        //调试GT的时候将下表开放，并将ConfigData7044_SchameC、SpecialConfigData7044_SchameC对应时钟参考进行设置
        //  _Chip7043ConfigData_2 = ConfigData7043_2_SchameA;
        }

        protected PLLRegAddrValuePair[] ConfigData7044_SchameC =
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
            new (0x005C, 0x80),
            new (0x005D, 0x02),//cij_new
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
          
            // KEEP
            new (0x0104, 0x00), // DCLKOUT6  //NC
            new (0x0136, 0x5d), // SCLKOUT11 //7043A_SYNC
            new (0x014A, 0x00), // SCLKOUT13
            // 10MHz
            
            // Pluse
            new (0x00C8, 0x5d), // DCLKOUT0  //acq1 rfsync
            new (0x00D2, 0x5d), // SCLKOUT1  //acq5 rfsync
            new (0x00FA, 0x00), // SCLKOUT5  //7043B_clk_2.5G  //开启：F3:,关闭：00
            new (0x0122, 0x5d), // SCLKOUT9  //acq4 rfsync
            new (0x00DC, 0x00), // DCLKOUT2  //vu9p_clk
            new (0x00F0, 0x00), // DCLKOUT4  //7043B_sync      //开启：5D:,关闭：00  

            new (0x012C, 0xF3), // DCLKOUT10 //7043A_clk_2.5G

            new (0x00E6, 0x5d), // SCLKOUT3 //acq3 rfsync
            new (0x0140, 0x00), // DCLKOUT12 //NC
            new (0x0118, 0x5d), // DCLKOUT8  //acq2 rfsync

            new (0x010E, 0x5d), // SCLKOUT7  //acq6 rfsync
           

            //-----------------------
            //      Output 分频
            //-------------------------
            // NC
            new (0x00DD, 0x80), // [7:0]LSB
            new (0x00DE, 0x02), // [3:0]MSB DCLKOUT2 @ //acq rfsync
            new (0x00F1, 0x80), // [7:0]LSB
            new (0x00F2, 0x02), // [3:0]MSB DCLKOUT4 @ //acq rfsync
            new (0x012D, 0x01), // [7:0]LSB
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @ //acq rfsync//2.5GHz
            // KEEP
            new (0x0105, 0x01), // [7:0]LSB
            new (0x0106, 0x00), // [3:0]MSB DCLKOUT6 @ KEEP
            new (0x0137, 0x60), // [7:0]LSB
            new (0x0138, 0x01), // [3:0]MSB SCLKOUT11 @ KEEP

            new (0x014B, 0x19), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ KEEP
            // 10MHz
            new (0x0119, 0x80), // [7:0]LSB
            new (0x011A, 0x02), // [3:0]MSB DCLKOUT8 @ //acq rfsync
            // Pluse
            new (0x00C9, 0x80), // [7:0]LSB
            new (0x00CA, 0x02), // [3:0]MSB DCLKOUT0 @ //acq rfsync
            new (0x00D3, 0x80), // [7:0]LSB
            new (0x00D4, 0x02), // [3:0]MSB SCLKOUT1 @ //acq rfsync
            new (0x00E7, 0x80), // [7:0]LSB
            new (0x00E8, 0x02), // [3:0]MSB SCLKOUT3 @ //acq rfsync
            new (0x00FB, 0x05), // [7:0]LSB
            new (0x00FC, 0x00), // [3:0]MSB SCLKOUT5 @ //7043B_2.5G
            new (0x0123, 0x80), // [7:0]LSB
            new (0x0124, 0x02), // [3:0]MSB SCLKOUT9 @ //acq rfsync

            new (0x010F, 0x80), // [7:0]LSB
            new (0x0110, 0x02), // [3:0]MSB SCLKOUT7 @ pluse
            new (0x0141, 0x80), // [7:0]LSB
            new (0x0142, 0x02), // [3:0]MSB DCLKOUT12 @ //acq rfsync
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
            new (0x00DA, 0x89), // SCLKOUT1
            new (0x00E4, 0x89), // DCLKOUT2
            new (0x00EE, 0x89), // SCLKOUT3
            new (0x00F8, 0x10), // DCLKOUT4
            new (0x0102, 0x10), // SCLKOUT5
            new (0x010C, 0x09), // DCLKOUT6
            new (0x0116, 0x89), // SCLKOUT7
            new (0x0120, 0x89), // DCLKOUT8
            new (0x012A, 0x89), // SCLKOUT9
            new (0x0134, 0x89), // DCLKOUT10
            new (0x013E, 0x09), // SCLKOUT11
            new (0x0148, 0x89), // DCLKOUT12
            new (0x0152, 0x89), // SCLKOUT13
            //new (0x0152, 0x08), // SCLKOUT13
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

        protected PLLRegAddrValuePair[] SpecialConfigData7044_SchameC =
        {
            new(0x012C, 0xb3), // DCLKOUT10 //7043A_clk_2.5G
        };

        protected PLLRegAddrValuePair[] ConfigData7043_SchameC =
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
            new (0x012C, 0xF3), // DCLKOUT10 //acq1 7044 clk_10M  
            new (0x0136, 0xF3), // SCLKOUT11 //acq3 7044 clk_10M  
            new (0x0140, 0x00), // DCLKOUT12 //pcie_pro_gt1
            new (0x014A, 0x00), // SCLKOUT13 //pcie_pro_gt2
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
            new (0x012D, 0xFA), // [7:0]LSB
            new (0x012E, 0x00), // [3:0]MSB DCLKOUT10 @ acq1 7044 clk_10M 
            new (0x0137, 0xFA), // [7:0]LSB
            new (0x0138, 0x00), // [3:0]MSB SCLKOUT11 @ acq3 7044 clk_10M  
            new (0x0141, 0xFA), // [7:0]LSB
            new (0x0142, 0x00), // [3:0]MSB DCLKOUT12 @ pcie_pro_gt1
            new (0x014B, 0x00), // [7:0]LSB
            new (0x014C, 0x00), // [3:0]MSB SCLKOUT13 @ pcie_pro_gt2
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

        protected PLLRegAddrValuePair[] SpecialConfigData7043_SchameC =
        {
            //寄存器，值
            new(0x00F0, 0xb3), // DCLKOUT4  //acq5 7044 clk_10M
            new(0x0104, 0xb3), // DCLKOUT6  //acq6 7044 clk_10M
            new(0x010E, 0xb3), // SCLKOUT7  //acq4 7044 clk_10M
            new(0x0118, 0xb3), // DCLKOUT8  //acq2 7044 clk_10M
            new(0x012C, 0xb3), // DCLKOUT10 //acq1 7044 clk_10M  
            new(0x0136, 0xb3), // SCLKOUT11 //acq3 7044 clk_10M  
        };
    }
}