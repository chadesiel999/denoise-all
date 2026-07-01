using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public partial class InstrumentInteract
    {
        public static byte[] ConvertBinDataToScpiData(byte[] origin, int startIndex, int count)
        {
            byte[] resultData = new byte[count + VISASession.BinDataHeaderLength];

            resultData[0] = 0x23;
            resultData[1] = 0x39;//9
            string lenStr = count.ToString().PadLeft(9, '0');
            for (int i = 0; i < 9; i++)
                resultData[2 + i] = (byte)lenStr[i];
            if (count > 0)
                Array.Copy(origin, startIndex, resultData, VISASession.BinDataHeaderLength, count);
            return resultData;
        }
        public static string GetCmdStr(ScpiCmd scpiCmd)
        {
            if (ScpiCmdTable.TryGetValue(scpiCmd, out string? cmdStr))
                return cmdStr;
            else
                return "";
        }
    }
}
