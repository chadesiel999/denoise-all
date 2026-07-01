using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public static partial class Constants
    {
        #region 协议触发/解码控制字
        public const UInt32 PROT_USED4_TRIG = 0;
        public const UInt32 PROT_USED4_DECODE = 1;
        public const UInt32 PROT_SYS_CLOCK_HZ = 312500000;
        public const UInt32 PROT_JTAG_SYS_CLOCK_HZ = 250000000;
        public const Byte PROT_PACKET_HEADER = 0xA5;
        #endregion 
    }


    public static class CovertExtension
    {
        public static String GetASCIIStr(this byte[] data)
        {
            String str = String.Empty;
            if(data!=null&&data.Length>0)
            {
                str= string.Join(' ', data.Select(x => (Int32)x switch
                {
                    0 => "NUL",
                    1 => "SOH",
                    2 => "STX",
                    3 => "ETX",
                    4 => "EOT",
                    5 => "ENQ",
                    6 => "ACK",
                    7 => "BEL",
                    8 => "BS",
                    9 => "HT",
                    10 => "LF",
                    11 => "VT",
                    12 => "FF",
                    13 => "CR",
                    14 => "SO",
                    15 => "SI",
                    16 => "DLE",
                    17 => "DC1",
                    18 => "DC2",
                    19 => "DC3",
                    20 => "DC4",
                    21 => "NAK",
                    22 => "SYN",
                    23 => "ETB",
                    24 => "CAN",
                    25 => "EM",
                    26 => "SUB",
                    27 => "ESC",
                    28 => "FS",
                    29 => "GS",
                    30 => "RS",
                    31 => "US",
                    32 => "SP",
                    int ch when (ch >= 33 && ch <= 126) => System.Text.Encoding.ASCII.GetString(new byte[1] { (byte)ch }),
                    127 => "DEL",
                    _ => Encoding.ASCII.GetString(new byte[1] { x }),
                }));
            }

            return str;
        }
    }
}
