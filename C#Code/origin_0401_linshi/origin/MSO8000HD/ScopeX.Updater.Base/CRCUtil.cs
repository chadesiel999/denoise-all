using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScopeX.Updater.Base
{
    internal class CRCUtil
    {
        public static string CRCCalc(string data)
        {

            List<byte> bytedata = Encoding.UTF8.GetBytes(data).ToList();

            byte[] crcbuf = bytedata.ToArray();
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }

            }
            string[] redata = new string[2];
            redata[1] = Convert.ToString((byte)((crc >> 8) & 0xff), 16);
            redata[0] = Convert.ToString((byte)((crc & 0xff)), 16);
            return redata[0] + " " + redata[1];
        }
    }
}
