using ScopeX.ComModel;
using ScopeX.USBBridge;
using System;
using System.Linq;
using System.Text;

namespace ScopeX.Hardware.Driver
{
    internal class FpgaVersion
    {
        public int MaxNum
        {
            get;
            set;
        }
        public int SubNum
        {
            get;
            set;
        }
        public int MinNum
        {
            get;
            set;
        }
        public string Designer
        {
            get;
            set;
        } = "";
        public string Comment
        {
            get;
            set;
        } = "";
        public DateTime BuildTime
        {
            get;
            set;
        }
        public string BuildTimeString
        {
            get
            {
                return BuildTime.Year + "-" + BuildTime.Month.ToString().PadLeft(2, '0') + "-" + BuildTime.Day.ToString().PadLeft(2, '0') + " " + BuildTime.Hour.ToString().PadLeft(2, '0') + ":" + BuildTime.Minute.ToString().PadLeft(2, '0') + ":" + BuildTime.Second.ToString().PadLeft(2, '0');

            }
        }
        public override string ToString()
        {
            return $"[Version: V{MaxNum}.{SubNum.ToString("D2")}.{MinNum.ToString("D4")},CompileTime={BuildTimeString},Designer={Designer},Comment={Comment}]";
        }
        internal static bool ReadFPGAVersion(FpgaVersionRegs regs, ref FpgaVersion fpgaVersion)
        {
            int findFirstZeroPos(byte[] source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] == 0)
                        return i;
                }
                return source.Length;
            }
            #region 编译时间
            DateTime compileTime = new DateTime(1970, 1, 1, 0, 0, 0);
            try
            {
                Int32 CompileTime_Year = Int32.Parse(HdIO.ReadReg(regs.CompileTime_L16).ToString("X"));
                Int32 tmpData = (Int32)HdIO.ReadReg(regs.CompileTime_H16);
                Int32 CompileTime_Month = Int32.Parse(((tmpData >> 8) & 0xff).ToString("X"));
                Int32 CompileTime_Day = Int32.Parse((tmpData & 0xff).ToString("X"));
                tmpData = (Int32)HdIO.ReadReg(regs.Comment_Words[14]);
                Int32 CompileTime_Hour = Int32.Parse(((tmpData >> 8) & 0xff).ToString("X"));
                Int32 CompileTime_Minute = Int32.Parse((tmpData & 0xff).ToString("X"));
                tmpData = (Int32)HdIO.ReadReg(regs.Comment_Words[15]);
                Int32 CompileTime_Second = Int32.Parse(((tmpData >> 8) & 0xff).ToString("X"));

                compileTime = new DateTime(CompileTime_Year, CompileTime_Month, CompileTime_Day, CompileTime_Hour, CompileTime_Minute, CompileTime_Second);
            }
            catch
            {

            }
            fpgaVersion.BuildTime = compileTime;
            #endregion

            #region version
            UInt32 readBackVersion = 0;
            readBackVersion = HdIO.ReadReg(regs.VerionNo_H16);
            readBackVersion <<= 16;
            readBackVersion |= HdIO.ReadReg(regs.VerionNo_L16);
            fpgaVersion.MinNum = (Int32)(readBackVersion & 0xffff);
            fpgaVersion.SubNum = (Int32)(readBackVersion >> 16 & 0xff);
            fpgaVersion.MaxNum = (Int32)(readBackVersion >> 24 & 0xff);
            #endregion

            #region designer
            UInt16[] readBackDesigner = new UInt16[4];
            for (int i = 0; i < 4; i++)
                readBackDesigner[i] = (UInt16)HdIO.ReadReg(regs.Designer_Words[i]);
            byte[] bytesDesigner = new byte[8];
            Buffer.BlockCopy(readBackDesigner, 0, bytesDesigner, 0, 8);
            byte[] bytesDesigner2 = bytesDesigner.Reverse<byte>().ToArray();
            int firstZeroPos = findFirstZeroPos(bytesDesigner2);
            fpgaVersion.Designer = System.Text.Encoding.UTF8.GetString(bytesDesigner2, 0, firstZeroPos);

            #endregion

            #region comment
            UInt16[] readbackComment = new ushort[16];
            for (int i = 0; i < 16; i++)
                readbackComment[i] = (UInt16)HdIO.ReadReg(regs.Comment_Words[i]);
            byte[] bytesComment = new byte[16 * 2];
            Buffer.BlockCopy(readbackComment, 0, bytesComment, 0, 16 * 2);
            byte[] bytesComment2 = bytesComment.Reverse<byte>().ToArray();
            firstZeroPos = findFirstZeroPos(bytesComment2);
            fpgaVersion.Comment = System.Text.Encoding.UTF8.GetString(bytesComment2, 0, firstZeroPos);
            #endregion
            return true;
        }
        
        /// <summary>
        /// 硬件版本号
        /// </summary>
        private static UInt32 _HdVersion;

        /// <summary>
        /// 读取硬件版本号
        /// </summary>
        internal static void ReadHdVersion()
        {
            _HdVersion = HdIO.ReadReg(PcieBdReg.R.SysMon_HardwareVersion);
        }
        
        public static string GetAllFPGAVersionInfo()
        {
            if (!Constants.BOARD_ATTACHED)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"[PCIE_Board]:{Hd.CurrProduct?.PcieBd?.FpgaVersion?.ToString()}");
            stringBuilder.AppendLine($"[S6_Board]:{Hd.CurrProduct?.S6Bd?.FpgaVersion?.ToString()}");
            stringBuilder.AppendLine($"[Process_Board]:{Hd.CurrProduct?.ProcBd?.FpgaVersion?.ToString()}");

            for (int fpgaIndex = 0; fpgaIndex < AbstractAcqBd.FPGATotalCount; fpgaIndex++)
            {
                FpgaVersion? fpgaVersion = Hd.CurrProduct?.AcqBd?.GetFpgaVersion(fpgaIndex) ?? null;
                if (fpgaVersion != null)
                    stringBuilder.AppendLine($"[Acq_Board{fpgaIndex + 1}]:{fpgaVersion.ToString()}");
            }
            stringBuilder.AppendLine($"[Channel_Board]:{CtrlAnalogChannel_JiHe2d5G.baseObj1.Mcu_GetVersionInfo()?.ToString()}");

            //stringBuilder.AppendLine($"[AWG_Board]:{Hd.ResopnseAWGVersion()}");
            if (Tmc.Default != null)
            {
                stringBuilder.AppendLine($"[TMC_Board]:{Tmc.Default.UsbtmcBridge_GetVersion()}");
            }
            //硬件版本
            uint version = _HdVersion;
            //Hd.CurrDebugVarints.bEnable_IsOpenDDr = version == 2;//固件版本2，默认启动DDr
            Hd.CurrDebugVarints.bEnable_IsOpenDDr = true;//当前所有版本默认使用DDR

            string hardVersion = (version >> 3).ToString() + ".0" + (version & 0b111).ToString();
            stringBuilder.AppendLine($"[Hard_Ware]:{hardVersion}");
            return stringBuilder.ToString();
        }
        private static uint ReverseBits(uint value, int countbit)
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
    internal record FpgaVersionRegs(
    UInt32 CompileTime_L16, UInt32 CompileTime_H16,
    UInt32 VerionNo_L16, UInt32 VerionNo_H16,
    UInt32[/*4*/] Designer_Words,
    UInt32[/*16*/] Comment_Words);
}
