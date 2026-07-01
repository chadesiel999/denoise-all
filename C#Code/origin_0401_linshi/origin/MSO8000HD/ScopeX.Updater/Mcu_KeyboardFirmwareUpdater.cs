using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ScopeX.ComModel;
using ScopeX.Updater;
using ScopeX.Updater.Base;

namespace WindowsDSO_Updater
{
    internal class Mcu_KeyboardFirmwareUpdater
    {
        private static Action<string>? AddLog;
        private static void WriteLog(string message)
        {
            AddLog?.Invoke(message);
        }
        public static void BindLogFunc(Action<string>? logFunc)
        {
            AddLog = logFunc;
        }

        private const UInt16 BSL_START_ADDR = 0x0C22;
        private static readonly Byte[] BSL_PASSWORD = Enumerable.Repeat((Byte)0xFF, 32).ToArray();
        private const String BS_150S_14X = "@0220\n" +
            "24 02 00 00 C0 43 E3 FF B2 40 10 A5 2C 01 B2 40\n" +
            "00 A5 28 01 F2 C0 3A 00 08 02 09 43 B0 12 A2 03\n" +
            "B0 12 BA 0D 55 42 0B 02 75 90 12 00 37 24 B0 12\n" +
            "5C 03 55 42 0B 02 75 90 20 00 17 24 75 90 14 00\n" +
            "12 24 75 90 1A 00 0B 24 75 90 1C 00 1A 24 04 3C\n" +
            "B0 12 94 0E D9 3F 21 53 B0 12 8C 0E D5 3F B0 12\n" +
            "94 0E 10 42 0E 02 30 40 76 0D B0 12 94 0E D2 42\n" +
            "0E 02 56 00 D2 42 0F 02 57 00 D2 42 10 02 09 02\n" +
            "C3 3F 16 42 0E 02 17 42 10 02 35 43 75 96 03 20\n" +
            "17 83 FC 23 DD 3F 82 46 1E 02 DE 3F 16 42 0E 02\n" +
            "17 42 10 02 E2 B2 08 02 42 24 F2 D0 10 00 08 02\n" +
            "B0 12 10 0F 36 90 00 10 07 2C 36 90 00 01 0A 2C\n" +
            "D6 42 06 02 00 00 2D 3C B2 40 00 A5 2C 01 B2 40\n" +
            "40 A5 28 01 16 B3 04 20 D2 42 06 02 1C 02 21 3C\n" +
            "D2 42 06 02 1D 02 86 9A FD FF 0B 24 E2 D3 08 02\n" +
            "B2 B0 20 00 08 02 05 20 B2 D0 20 00 08 02 82 46\n" +
            "1E 02 36 90 01 02 06 28 F2 D2 08 02 F2 B0 10 00\n" +
            "08 02 03 24 F2 C0 32 00 08 02 1A 42 1C 02 86 4A\n" +
            "FF FF 16 53 17 83 C4 23 B0 12 5C 03 91 3F B0 12\n" +
            "10 0F 17 83 FC 23 B0 12 5C 03 8E 3F 18 42 12 02\n" +
            "B0 12 10 0F D2 42 06 02 12 02 B0 12 10 0F D2 42\n" +
            "06 02 13 02 38 E3 F2 B2 08 02 0C 24 86 9A FE FF\n" +
            "09 24 B2 B0 20 00 08 02 05 20 16 53 82 46 1E 02\n" +
            "E2 D3 08 02 18 92 12 02 6E 23 E2 B3 08 02 6B 23\n" +
            "30 41 E2 B2 28 00 FD 27 E2 B2 28 00 FD 23 B2 40\n" +
            "24 02 60 01 E2 B2 28 00 FD 27 15 42 70 01 05 11\n" +
            "05 11 05 11 82 45 00 02 05 11 82 45 02 02 B2 80\n" +
            "1E 00 02 02 57 42 09 02 37 80 03 00 05 11 05 11\n" +
            "17 53 FD 23 30 40 64 0E\n" +
            "q";

        private List<HexData> ParsHex(String hexstring)
        {
            List<HexData> datas = new List<HexData>();
            if (String.IsNullOrEmpty(hexstring)) return datas;
            try
            {
                var lines = hexstring.Split('\n').Select(x => x.Trim());
                String addrregstr = @"^@([0-9a-fA-F]{4})$";
                foreach (var line in lines)
                {
                    if (line.Contains('q')) break;
                    if (Regex.IsMatch(line, addrregstr, RegexOptions.IgnoreCase))
                    {
                        HexData data = new HexData();
                        var match = Regex.Match(line, addrregstr, RegexOptions.IgnoreCase);
                        data.StartAddr = Convert.ToUInt16(match.Groups[^1].Value.Trim(), 16);
                        datas.Add(data);
                    }
                    else
                    {
                        if (datas.Count == 0)
                        {
                            return new List<HexData>();
                        }
                        datas[^1].Datas.AddRange(line.Trim().Split(' ').Select(x => Convert.ToByte(x.Trim(), 16)));
                    }

                }
                return datas;
            }
            catch
            {
                return new List<HexData>();
            }
        }
        class HexData
        {
            public UInt16 StartAddr { get; set; }
            public List<Byte> Datas { get; } = new List<Byte>();
        }
        [AllowNull]
        private Action ProgressChanged { get; set; }
        [AllowNull]
        private Action<String> MessageChanged { get; set; }
        private enum Cmd : byte
        {
            RX_Data_Block = 0x12,
            RX_PassWord = 0x10,
            Erase_Segment = 0x16,
            Erase_Main_Or_Info = 0x16,
            Mass_Erase = 0x18,
            Erase_Check = 0x1c,
            Change_Baud_Rate = 0x20,
            Set_Mem_Offset = 0x21,
            Load_PC = 0x1a,
            TX_Data_Block = 0x14,
            TX_BSL_Version = 0x1e,
        }
        public Int32 Progress
        {
            get => _Progress;
            private set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    ProgressChanged?.Invoke();
                }
            }
        }
        public String Message
        {
            get => _Message;
            private set
            {
                if (_Message != value)
                {
                    _Message = value;
                    MessageChanged?.Invoke(value);
                }
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BSLData
        {
            [MarshalAs(UnmanagedType.I1)]
            public Byte HDR;
            [MarshalAs(UnmanagedType.I1)]
            public Cmd Cmd;
            [MarshalAs(UnmanagedType.I1)]
            public Byte L1;
            [MarshalAs(UnmanagedType.I1)]
            public Byte L2;
            [MarshalAs(UnmanagedType.I1)]
            public Byte AL;
            [MarshalAs(UnmanagedType.I1)]
            public Byte AH;
            [MarshalAs(UnmanagedType.I1)]
            public Byte LL;
            [MarshalAs(UnmanagedType.I1)]
            public Byte LH;
            //public Byte[] Data;
        }
        private const Byte DATA_ACK = 0x90;
        private const Byte SYNC = 0x80;
        private const Byte MAX_FRAME_LENGHT = 0xF0;
        private System.IO.Ports.SerialPort _SerialPort;
        private Int32 _Progress;
        private String _Message = String.Empty;

        private Byte[] GetBytes(BSLData data, Byte[] datas)
        {
            if (datas == null) datas = new Byte[0];
            Byte[] bytes = new Byte[8 + datas.Length + 2];
            Unsafe.CopyBlock(ref bytes[0], ref Unsafe.As<BSLData, Byte>(ref data), 8);
            if (datas.Length > 0) Unsafe.CopyBlock(ref bytes[8], ref datas[0], (UInt32)datas.Length);

            for (Int32 j = 0; j < bytes.Length - 2;)
            {
                bytes[^2] ^= bytes[j++];
                bytes[^1] ^= bytes[j++];
            }

            bytes[^1] = (Byte)~bytes[^1];
            bytes[^2] = (Byte)~bytes[^2];
            return bytes;
        }

        private string _comname = "COM4";
        private Mcu_KeyboardFirmwareUpdater(String comport = $"COM4", int baudRate = 19200)
        {
            _comname = comport;
            _SerialPort = new System.IO.Ports.SerialPort(comport);
            _SerialPort.ReadBufferSize = 4096;
            _SerialPort.WriteBufferSize = 4096;
            _SerialPort.ReadTimeout = 250;
            _SerialPort.WriteTimeout = -1;
            _SerialPort.StopBits = System.IO.Ports.StopBits.One;
            _SerialPort.Parity = System.IO.Ports.Parity.Even;
            _SerialPort.BaudRate = baudRate;
            _SerialPort.DataBits = 8;
        }
        public void Clear()
        {
            try
            {
                _SerialPort.ReadExisting();
            }
            catch { }
        }
        private (BSLData, Byte[]) CreateRXPassword()
        {
            BSLData data = new BSLData();
            data.HDR = SYNC;
            data.Cmd = Cmd.RX_PassWord;
            data.L1 = data.L2 = 0x24;
            data.AL = 0xe0;
            data.AH = 0xff;
            data.LL = 0x20;
            return (data, BSL_PASSWORD);
        }
        private (BSLData, Byte[]) CreateMasserase()
        {
            BSLData data = new BSLData();
            data.HDR = SYNC;
            data.Cmd = Cmd.Mass_Erase;
            data.L1 = data.L2 = 0x04;
            data.LL = 0x06;
            data.LH = 0xa5;
            return (data, new Byte[0]);
        }

        private (BSLData, Byte[]) CreateRXDataBlock(UInt16 addr, Byte[] data)
        {
            BSLData bsldata = new BSLData();
            bsldata.HDR = SYNC;
            bsldata.Cmd = Cmd.RX_Data_Block;
            bsldata.L1 = bsldata.L2 = (Byte)(data.Length + 4);
            bsldata.AL = (Byte)(addr & 0xFF);
            bsldata.AH = (Byte)((addr >> 8) & 0xFF);
            bsldata.LL = (Byte)(bsldata.L1 - 4);
            bsldata.LH = 0;
            return (bsldata, data);
        }
        private Boolean SendPassWord()
        {
            if (!Sync()) return false;
            var result = CreateRXPassword();
            var data = GetBytes(result.Item1, result.Item2);
            _SerialPort.Write(data, 0, data.Length);
            return _SerialPort.ReadByte() == DATA_ACK;
        }
        private Boolean SendMassErase()
        {
            if (!Sync()) return false;
            var result = CreateMasserase();
            var data = GetBytes(result.Item1, result.Item2);
            _SerialPort.Write(data, 0, data.Length);
            return _SerialPort.ReadByte() == DATA_ACK;
        }
        private Boolean SendLoadPC(UInt16 addr)
        {
            if (!Sync()) return false;
            var result = CreateLoadPC(addr);
            var data = GetBytes(result.Item1, result.Item2);
            _SerialPort.Write(data, 0, data.Length);
            return _SerialPort.ReadByte() == DATA_ACK;
        }
        private Boolean SendRXDataBlock(UInt16 addr, Byte[] blockdata)
        {
            if (!Sync()) return false;
            var result = CreateRXDataBlock(addr, blockdata);
            var data = GetBytes(result.Item1, result.Item2);
            _SerialPort.Write(data, 0, data.Length);
            return _SerialPort.ReadByte() == DATA_ACK;
        }
        private Boolean SendTXDataBlock(UInt16 addr, Byte len, out Byte[] datas)
        {
            datas = new Byte[len];
            if (!Sync()) return false;
            var result = CreateTXDataBlock(addr, len);
            var data = GetBytes(result.Item1, result.Item2);
            _SerialPort.Write(data, 0, data.Length);
            var ack = _SerialPort.ReadByte();
            if (ack != SYNC) return false;
            data = ReadBytes(3);
            datas = ReadBytes((Byte)(data[^1] + 2));
            return true;
        }
        private Byte ReadByte() => (Byte)_SerialPort.ReadByte();
        private Byte[] ReadBytes(Byte len)
        {
            Byte[] result = new Byte[len];
            _SerialPort.Read(result, 0, len);
            return result;
        }
        private (BSLData, Byte[]) CreateLoadPC(UInt16 addr)
        {
            BSLData bsldata = new BSLData();
            bsldata.HDR = SYNC;
            bsldata.Cmd = Cmd.Load_PC;
            bsldata.L1 = bsldata.L2 = 0x04;
            bsldata.AL = (Byte)(addr & 0xFF);
            bsldata.AH = (Byte)(addr >> 8);
            bsldata.LL = 0;
            bsldata.LH = 0;
            return (bsldata, new Byte[0]);
        }
        private (BSLData, Byte[]) CreateTXDataBlock(UInt16 addr, Byte len)
        {
            BSLData bsldata = new BSLData();
            bsldata.HDR = SYNC;
            bsldata.Cmd = Cmd.TX_Data_Block;
            bsldata.L1 = bsldata.L2 = 0x04;
            bsldata.AL = (Byte)(addr & 0xFF);
            bsldata.AH = (Byte)(addr >> 8);
            bsldata.LL = len;
            bsldata.LH = 0;
            return (bsldata, new Byte[0]);
        }
        private void CommRts(Boolean state)
        {
            _SerialPort.RtsEnable = state;
        }
        private void CommDtr(Boolean state)
        {
            _SerialPort.DtrEnable = state;
        }
        private void StartBSL()
        {
            CommDtr(false);
            CommRts(true);

            Task.Delay(5).Wait();

            CommDtr(false);
            CommRts(false);

            Task.Delay(200).Wait();
            CommDtr(true);
            CommRts(false);

            Task.Delay(10).Wait();
            CommDtr(true);
            CommRts(true);

            Task.Delay(15).Wait();
            CommDtr(true);
            CommRts(false);

            Task.Delay(15).Wait();
            CommDtr(true);
            CommRts(true);

            Task.Delay(15).Wait();
            CommDtr(false);
            CommRts(true);

            Task.Delay(15).Wait();
            CommDtr(false);
            CommRts(false);

            Task.Delay(15).Wait();
        }

        private Boolean Sync()
        {
            _SerialPort.Write(new Byte[] { SYNC }, 0, 1);
            return _SerialPort.ReadByte() == DATA_ACK;
        }
        private void Close()
        {
            if (!_SerialPort.IsOpen) return;
            _SerialPort.Close();
        }
        private void Reset()
        {
            CommDtr(false);
            CommRts(false);
            Thread.Sleep(300);
            CommDtr(true);
            CommRts(false);
            Thread.Sleep(20);
            CommDtr(false);
            CommRts(false);
            Thread.Sleep(20);
        }
        private void Open()
        {
            if (_SerialPort.IsOpen) return;
            _SerialPort.Open();
        }
        public Boolean Flash(Byte[] hexbuffer)
        {
            _Progress = 0;
            _Message = String.Empty;
            Message = "开始解析固件";
            HexData bsl = ParsHex(BS_150S_14X).FirstOrDefault();
            if (bsl == null)
            {
                Message = "固件解析失败";
                Progress = 100;
                return false;
            }
            List<HexData> hexdatas = ParsHex(System.Text.Encoding.Default.GetString(hexbuffer));
            if (hexdatas.Count == 0)
            {
                Message = "固件解析失败";
                Progress = 100;
                return false;
            }
            Message = "固件解析成功";
            Progress += 5;
            try
            {
                Open();
                _SerialPort.StopBits = System.IO.Ports.StopBits.One;
                _SerialPort.Parity = System.IO.Ports.Parity.Even;
                _SerialPort.BaudRate = 9600;
                _SerialPort.DataBits = 8;
                Message = "通信句柄打开成功";
                Progress += 2;
            }
            catch
            {
                Message = "通信句柄打开失败";
                Progress = 100;
                return false;
            }
            Message = "开始烧录固件";
            try
            {
                StartBSL();
                Clear();
                Message = "开始擦除固件";
                Progress += 3;
                Boolean result = SendMassErase();
                if (!result)
                {
                    Message = "固件擦除失败";
                    Progress = 100;
                    Reset();
                    Close();
                    return false;
                }
                Message = "开始写入固件";
                result = SendPassWord();
                if (!result)
                {
                    Message = "固件写入失败";
                    Progress = 100;
                    Reset();
                    Close();
                    return false;
                }
                Progress += 5;
                result = SendLoadPC(BSL_START_ADDR);
                if (!result)
                {
                    Message = "固件写入失败";
                    Progress = 100;
                    Reset();
                    Close();
                    return false;
                }
                Progress += 5;
                result = SendPassWord();
                if (!result)
                {
                    Message = "固件写入失败";
                    Progress = 100;
                    Reset();
                    Close();
                    return false;
                }
                Progress += 5;
                Int32 framecount = (Int32)Math.Ceiling(bsl.Datas.Count / (Double)(MAX_FRAME_LENGHT));
                UInt16 addr = bsl.StartAddr;
                Int32 startpro = Progress;
                Int32 sendedlen = 0;
                Double temppro = (40 - Progress) / (Double)(bsl.Datas.Count);
                for (Int32 j = 0; j < framecount; j++)
                {
                    Byte[] senddata = bsl.Datas.Skip(j * MAX_FRAME_LENGHT).Take(Math.Min(MAX_FRAME_LENGHT, bsl.Datas.Count - j * MAX_FRAME_LENGHT)).ToArray();
                    result = SendRXDataBlock(addr, senddata);
                    if (!result)
                    {
                        Message = "固件写入失败";
                        Progress = 100;
                        Reset();
                        Close();
                        return false;
                    }
                    addr += (UInt16)senddata.Length;
                    sendedlen += senddata.Length;
                    Progress = startpro + (Int32)(sendedlen * temppro);
                }

                result = SendTXDataBlock(bsl.StartAddr, 2, out var bytes);
                result = SendPassWord();
                result = SendLoadPC((UInt16)((bytes[1] << 8) | bytes[0]));
                startpro = Progress;
                sendedlen = 0;
                temppro = (100 - Progress) / (Double)(hexdatas.Sum(x => x.Datas.Count));
                for (Int32 i = 0; i < hexdatas.Count; i++)
                {
                    framecount = (Int32)Math.Ceiling(hexdatas[i].Datas.Count / (Double)(MAX_FRAME_LENGHT));
                    addr = hexdatas[i].StartAddr;
                    for (Int32 j = 0; j < framecount; j++)
                    {
                        Thread.Sleep(10);
                        Byte[] senddata = hexdatas[i].Datas.Skip(j * MAX_FRAME_LENGHT).Take(Math.Min(MAX_FRAME_LENGHT, hexdatas[i].Datas.Count - j * MAX_FRAME_LENGHT)).ToArray();
                        result = SendRXDataBlock(addr, senddata);
                        if (!result)
                        {
                            Message = "固件写入失败";
                            Progress = 100;
                            Reset();
                            Close();
                            return false;
                        }
                        addr += (UInt16)senddata.Length;
                        sendedlen += senddata.Length;
                        Progress = startpro + (Int32)(sendedlen * temppro);
                    }
                }
                Reset();
                Close();
                Message = "固件烧录完成";
            }
            catch
            {
                Close();
                Message = "固件烧录失败";
                Progress = 100;
                return false;
            }
            return true;
        }
        private String ReadVersion()
        {
            try
            {
                if (!_SerialPort.IsOpen)
                {
                    _SerialPort.Open();
                }
                /*CommDtr(false);
                CommRts(false);
                Thread.Sleep(1000);*/
                _SerialPort.BaudRate = 19200;
                _SerialPort.Parity = System.IO.Ports.Parity.None;
                Byte[] cmd = Encoding.ASCII.GetBytes("*IDN?");
                _SerialPort.Write(cmd, 0, cmd.Length);
                _SerialPort.BaseStream.Flush();
                Thread.Sleep(200);
                if (_SerialPort.BytesToRead > 0)
                {
                    Byte[] resultbuffer = new Byte[_SerialPort.BytesToRead];
                    _SerialPort.Read(resultbuffer, 0, resultbuffer.Length);
                    return Encoding.Default.GetString(resultbuffer);
                }
                else
                {
                    return String.Empty;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        internal static bool DoUpdate(List<UpdateItem> updateMcuItems, Action<string>? logWriter)
        {
            bool bResult = true;
            foreach (var item in updateMcuItems)
            {
                if (item.Type != UpdaterItemType.Mcu_Keyboard)
                    continue;
                string BoardName = item.BoardName.Trim();
                string comPortNum = BoardName.Substring(BoardName.Length - 1, 1);
                Mcu_KeyboardFirmwareUpdater updater = new Mcu_KeyboardFirmwareUpdater($"COM{comPortNum}");
                updater.MessageChanged = logWriter;

                Boolean needflash = true;
                if (item.BaseInfo is ImageBlock imageBlock)
                {
                    String version = updater.ReadVersion();
                    String[] versions = version.Split(",");
                    String regstr = "([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)";
                    if (versions.Length >= 4 && Regex.IsMatch(versions[3], regstr, RegexOptions.IgnoreCase))
                    {
                        Match match = Regex.Match(versions[3], regstr, RegexOptions.IgnoreCase);
                        HardwareVersionInfo imageversion = new HardwareVersionInfo();
                        imageversion.Major = Int32.Parse(match.Groups[1].Value);
                        imageversion.Minor = Int32.Parse(match.Groups[2].Value);
                        imageversion.Build = Int32.Parse(match.Groups[3].Value);
                        imageversion.Revision = Int32.Parse(match.Groups[4].Value);
                        needflash = imageBlock.Version.CompareTo(imageversion) != 0;
                    }
                }
                if (needflash)
                {
                    bResult = updater.Flash(item.Content);
                }
                updater.Close();
            }
            return bResult;
        }

        internal static async Task<bool> UpdateFormBinFile(List<UpdateItem> updateMcuItems, Action<string>? logWriter)
        {
            if (updateMcuItems == null || updateMcuItems.Count <= 0)
                return false;

            bool bResult = false;
            foreach (var item in updateMcuItems)
            {
                if (item.Type != UpdaterItemType.Mcu_Keyboard)
                    continue;

                Mcu_KeyboardFirmwareUpdater updater = null;
                try
                {
                    string BoardName = item.BoardName.Trim();
                    string comPortNum = BoardName.Substring(BoardName.Length - 1, 1);
                    updater = new Mcu_KeyboardFirmwareUpdater();
                    updater.MessageChanged = logWriter;

                    Boolean needflash = true;
                    if (item.BaseInfo is ImageBlock imageBlock)
                    {
                        String version = updater.ReadVersion();
                        updater?.Close();

                        // 未能读到版本号。
                        if (string.IsNullOrEmpty(version))
                            return true;

                        String[] versions = version.Split(",");
                        String regstr = "([0-9]+)\\.([0-9]+)\\.([0-9]+)\\.([0-9]+)";
                        if (versions.Length >= 4 && Regex.IsMatch(versions[3], regstr, RegexOptions.IgnoreCase))
                        {
                            Match match = Regex.Match(versions[3], regstr, RegexOptions.IgnoreCase);
                            HardwareVersionInfo imageversion = new HardwareVersionInfo();
                            imageversion.Major = Int32.Parse(match.Groups[1].Value);
                            imageversion.Minor = Int32.Parse(match.Groups[2].Value);
                            imageversion.Build = Int32.Parse(match.Groups[3].Value);
                            imageversion.Revision = Int32.Parse(match.Groups[4].Value);
                            WriteLog($"按键板升级前版本:{imageversion.ToString()}");
                            HardwareVersionInfo oldHDVersion = new HardwareVersionInfo();
                            oldHDVersion.Major = 1;
                            oldHDVersion.Minor = 0;
                            oldHDVersion.Build = 0;
                            oldHDVersion.Revision = 4;

                            if (imageversion.CompareTo(oldHDVersion) <= 0)
                            {
                                // 1.0.0.4为旧板子，是MSP的MCU，1.0.0.5以后得为STM32的板子，旧板子不支持升级
                                //needflash = false;
                                WriteLog($"按键板当前版本:{imageversion.ToString()}不支持升级，跳过");
                                return true;// 认为升级成功，以免升级程序提示升级错误。
                            }
                            else
                            {
                                needflash = imageBlock.Version.CompareTo(imageversion) != 0;
                            }
                        }
                    }
                    if (needflash)
                    {
                        bResult = await updater.UpdateBinBytes2Keyboard(item.Content);
                    }
                }
                catch (Exception e)
                {

                }
            }
            return bResult;
        }

        private int FindPatternIndex(List<byte> byteList, byte[] pattern)
        {
            for (int i = 0; i <= byteList.Count - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (byteList[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 将键盘板的bin文件的二进制数据升级到键盘板中去。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private async Task<bool> UpdateBinBytes2Keyboard(byte[] bytes)
        {
            bool result = false;
            if (bytes == null)
                return result;

            SerialPort serialPort = null;
            try
            {
                bool isover = false;
                List<CommandInfo> _commands = new List<CommandInfo>()
                {
                    new CommandInfo()
                    {
                        CommandType = Command.Data,
                        ID = Encoding.ASCII.GetBytes("DATA"),
                        Length = 10,
                    },
                    new CommandInfo()
                    {
                        CommandType = Command.OK,
                        ID = Encoding.ASCII.GetBytes("OK"),
                        Length = 3,
                    }
                };
                int _minchecklenth = _commands.Min(c => c.Length);

                List<byte> _recevedBytes = new List<byte>();
                serialPort = new SerialPort(_comname);
                serialPort.StopBits = System.IO.Ports.StopBits.One;
                serialPort.Parity = System.IO.Ports.Parity.Even;
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.DataReceived += (s, e) =>
                {
                    try
                    {
                        SerialPort serialPort = s as SerialPort;
                        if (serialPort == null || serialPort.BytesToRead <= 0)
                            return;

                        byte[] buffer = new byte[serialPort.BytesToRead];
                        var readbyteslength = serialPort.Read(buffer, 0, buffer.Length);
                        if (readbyteslength <= 0)
                            return;

                        _recevedBytes.AddRange(buffer.Take(readbyteslength));

                        if (_recevedBytes.Count < _minchecklenth)
                            return;

                        Dictionary<Command, int> indexs = new Dictionary<Command, int>();

                        do
                        {
                            foreach (var command in _commands)
                            {
                                indexs.Add(command.CommandType, FindPatternIndex(_recevedBytes, command.ID));
                            }

                            var firsthandle = indexs.OrderBy(c => c.Value).FirstOrDefault(c => c.Value != -1);
                            if (firsthandle.Key == Command.None || firsthandle.Value == -1)
                            {
                                // 未找到可以识别的命令
                                return;
                            }
                            else if (firsthandle.Value > 0)
                            {
                                // 移除掉前面的无法识别的数据。
                                _recevedBytes.RemoveRange(0, firsthandle.Value);
                            }

                            /*// 版本号查询命令单独处理，所有本次接收到的内容都当做字符串处理。
                            if (firsthandle.Key == Command.Version)
                            {
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    MessageItems.Add(Encoding.ASCII.GetString(_recevedBytes.ToArray()));
                                }));
                                _recevedBytes.Clear();
                                return;
                            }*/

                            var commanditem = _commands.First(c => c.CommandType == firsthandle.Key);
                            if (_recevedBytes.Count < commanditem.Length)
                                return; // 数据长度不够

                            switch (firsthandle.Key)
                            {
                                case Command.Data:
                                    // data
                                    var needSize = _recevedBytes.Skip(4).Take(1).First();
                                    var startindexbytes = _recevedBytes.Skip(commanditem.ID.Length + 1).Take(4).Reverse();
                                    var startindex = BitConverter.ToUInt32(startindexbytes.ToArray(), 0);
                                    ResponseData(serialPort, bytes, needSize, startindex);
                                    _recevedBytes.RemoveRange(0, commanditem.Length);
                                    break;
                                case Command.OK:
                                    // ok
                                    Message = "键盘板升级成功。且已自动重启";
                                    Progress = 100;
                                    _recevedBytes.RemoveRange(0, commanditem.Length);
                                    result = true;
                                    isover = true;
                                    break;
                                case Command.Error:
                                    Message = "单片机遇到未知异常，已软件重启";
                                    _recevedBytes.RemoveRange(0, commanditem.Length);
                                    isover = true;
                                    break;
                            }

                            indexs.Clear();
                        } while (true);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog($"key board received exception:{e}");
                    }
                };
                serialPort.Open();
                _Progress = 0;
                _Message = String.Empty;
                Message = "开始解析固件";

                var _startbyte = Encoding.ASCII.GetBytes("START");
                List<byte> temp = new List<byte>();
                temp.AddRange(_startbyte);
                var lengthbytes = BitConverter.GetBytes((uint)(bytes.Length & 0xFFFFFFFF));
                temp.AddRange(lengthbytes.Reverse());
                var startbytes = temp.ToArray();
                temp.Add(GetSum(startbytes));
                serialPort.Write(temp.ToArray(), 0, temp.Count);
                serialPort.BaseStream.Flush();

                while (!isover)
                {
                    await Task.Delay(250);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog($"key board exception:{e}");
            }
            finally
            {
                serialPort?.Close();
            }

            return result;
        }

        /// <summary>
        /// 请求数据响应
        /// </summary>
        /// <param name="size"></param>
        /// <param name="startindex"></param>
        private void ResponseData(SerialPort serialPort, byte[] sourcebytes, byte size, uint startindex)
        {
            if (serialPort == null || sourcebytes == null)
                return;

            byte[] datas = sourcebytes.Skip((int)startindex).Take(size).ToArray();

            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes("DATA"));
            bytes.Add((byte)(size & 0xFF));

            var lengthbytes = BitConverter.GetBytes(startindex);
            bytes.AddRange(lengthbytes.Reverse());

            bytes.AddRange(datas);
            if (datas.Length < size)
                bytes.AddRange(Enumerable.Repeat((byte)0xFF, size - datas.Length));

            var sumbyte = GetSum(bytes.ToArray());
            bytes.Add(sumbyte);

            _Progress = (int)Math.Max(0, Math.Min(((double)(startindex + 1) / sourcebytes.Length) * 100, 100));
            //LogHelper.WriteLog($"startindex={startindex} get size={size},Progress={_Progress}");
            serialPort.Write(bytes.ToArray(), 0, bytes.Count);
            serialPort.BaseStream.Flush();
        }

        private byte GetSum(byte[] bytes)
        {
            int result = 0;
            foreach (var item in bytes)
                result += item;

            return (byte)(result & 0xFF);
        }

        private enum Command
        {
            None,
            Data,
            OK,
            Error,
            Key,
            Version
        }

        private class CommandInfo
        {
            public Command CommandType { get; set; }
            public byte[] ID { get; set; }

            public int Length { get; set; }
        }
    }
}
