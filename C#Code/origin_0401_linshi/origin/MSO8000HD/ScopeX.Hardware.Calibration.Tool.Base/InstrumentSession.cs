using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Visa32;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public interface IInstrumentSession
    {
        string Address
        {
            get;
        }
        bool WriteString(string cmdStr);
        bool WriteBinDataWithMultiPackage(string cmd, byte[] data, int count);
        string ReadString();
        string ReadShortString();
        bool bOpened
        {
            get;
        }
        bool Open(Action<Boolean>? action = null);
        void Close();
        int ReadBinData(ref Byte[] recvBuff);
        bool WriteBinData(byte[] data, int count);
    }
    public class VISASession : IInstrumentSession
    {
        private Int32 viDefRm, viDev, viErr;
        private string m_addr;
        private Int32 m_timeout = 500;
        private Boolean _bOpened = false;
        public const int BinDataHeaderLength = 11;
        public byte[] PerWriteBuffer = new byte[8 * 1024];
        private static byte[] ConvertBinDataFromScpiData(byte[] origin, int length, out int totalBytes)
        {
            if (origin[0] != 0x23)
            {
                totalBytes = origin.Length;
                return origin;
            }
            int lengthMarkLen = origin[1] - '0';
            StringBuilder lenStr = new StringBuilder("");
            lenStr.Append(Encoding.ASCII.GetChars(origin, 2, lengthMarkLen));

            totalBytes = int.Parse(lenStr.ToString());

            byte[] resultData = new byte[length - lengthMarkLen - 2];
            Array.Copy(origin, lengthMarkLen + 2, resultData, 0, length - lengthMarkLen - 2);
            return resultData;
        }
        Action<Boolean>? errorAction = null;
        public VISASession(string addr, Int32 timeout)
        {
            viDev = -1;
            viDefRm = -1;
            m_addr = addr;
            m_timeout = timeout;
        }
        public string Address
        {
            get => m_addr;
        }
        public bool bOpened
        {
            get => _bOpened;
        }
        public bool WriteString(string cmdStr)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(cmdStr);
            WriteBinData(bytes, bytes.Count());
            return true;
        }
        public bool WriteBinDataWithMultiPackage(string cmd, byte[] data, int count)
        {
            //注意：7000这个数与网络传输的包大小有关。如果有问题，请改小
            int perPackageBytes = ((7000 - 32 - cmd.Length - VISASession.BinDataHeaderLength) / 8) * 8;
            byte[] writebuffer = new byte[8192];
            string newFormatCmd;
            if (cmd.Last<char>() != ' ')
                newFormatCmd = cmd + " ";
            else
                newFormatCmd = cmd;
            int binDataStartIndex = newFormatCmd.Length;
            byte[] cmdBuffer = System.Text.Encoding.UTF8.GetBytes(newFormatCmd);
            int remain = count;
            int outCount = 0;
            int currLength = 0;
            byte[] binData;
            //File.WriteAllBytes($@"d:\send.bin", data);
            int sendnum = 0;
            while (remain > 0)
            {

                currLength = remain > perPackageBytes ? perPackageBytes : remain;
                binData = InstrumentInteract.ConvertBinDataToScpiData(data, count - remain, currLength);
                Array.Copy(cmdBuffer, writebuffer, cmdBuffer.Length);
                Array.Copy(binData, 0, writebuffer, cmdBuffer.Length, binData.Length);
                sendnum++;
                int ok = visa32.viWrite(viDev, writebuffer, cmdBuffer.Length + binData.Length, out outCount);
                if (ok != visa32.VI_SUCCESS && errorAction != null)
                    errorAction?.Invoke(ok == visa32.VI_SUCCESS);
                if (ok != visa32.VI_SUCCESS)
                    return false;
                Thread.Sleep(1);
                remain -= currLength;
            }
            binData = InstrumentInteract.ConvertBinDataToScpiData(data, 0, 0);
            Array.Copy(cmdBuffer, writebuffer, cmdBuffer.Length);
            Array.Copy(binData, 0, writebuffer, cmdBuffer.Length, binData.Length);
            visa32.viWrite(viDev, writebuffer, cmdBuffer.Length + binData.Length, out outCount);
            return true;
        }
        public bool WriteBinData(byte[] data, int count)
        {
            int outCount = 0;
            visa32.viWrite(viDev, data, count, out outCount);
            //https://zone.ni.com/reference/en-XX/help/370131S-01/ni-visa/datablocks/
            //int v=visa32.viPrintf(viDev, "%*b", data); 
            //visa32.viBufWrite(viDev, data, count, out outCount);
            //int outCount = 0;
            //try
            //{
            //    int result=visa32.viPrintf(viDev,"%s", data);

            //    //int remainBytes = count;
            //    //int currBytes = 0;
            //    //while(remainBytes>0)
            //    //{
            //    //    currBytes = remainBytes > PerWriteBuffer.Length ? PerWriteBuffer.Length : remainBytes;
            //    //    Array.Copy(data, count - remainBytes, PerWriteBuffer, 0, currBytes);
            //    //    visa32.viWrite(viDev, PerWriteBuffer, currBytes, out outCount);
            //    //    remainBytes-= currBytes;
            //    //}
            //}
            //catch (Exception e)
            //{
            //}
            return true;
        }
        const int MAX_Length = 1024 * 1024;
        byte[] tmpBuffer = new byte[MAX_Length];

        public string ReadShortString()
        {
            int resultCount = 0;
            int recvCount = 0;

            List<byte> recvBuffer = new List<byte>();

            visa32.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            while (resultCount > 0)
            {
                recvCount += resultCount;
                Range range = 0..resultCount;
                recvBuffer.AddRange(tmpBuffer[range]);
                resultCount = 0;
                //for (int i = 0; i < resultCount; i++)
                //    recvBuffer.Add(tmpBuffer[i]);
                //visa32.viRead(viDev, tmpBuffer, maxLength, out resultCount);
            }
            return System.Text.Encoding.Default.GetString(recvBuffer.ToArray(), 0, recvCount).Trim();
        }
        public string ReadString()
        {
            int resultCount = 0;
            int recvCount = 0;

            List<byte> recvBuffer = new List<byte>();

            int ok = visa32.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            if (ok != visa32.VI_SUCCESS)
                errorAction?.Invoke(ok == visa32.VI_SUCCESS);
            if (ok != visa32.VI_SUCCESS)
                return "";
            while (resultCount > 0)
            {
                recvCount += resultCount;
                Range range = 0..resultCount;
                recvBuffer.AddRange(tmpBuffer[range]);

                //for (int i = 0; i < resultCount; i++)
                //    recvBuffer.Add(tmpBuffer[i]);
                visa32.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            }
            return System.Text.Encoding.Default.GetString(recvBuffer.ToArray(), 0, recvCount).Trim();
        }
        private int TMCHeaderBytes = 24;
        public int ReadBinData(ref Byte[] recvBuff)
        {
            int currCount = 0;
            byte[] tmpBuffer = new byte[recvBuff.Length + TMCHeaderBytes];
            int recvedBytes = 0;
            int result = visa32.viRead(viDev, tmpBuffer, tmpBuffer.Length, out currCount);
            int willRecvTotalBytes = 0;
            if (currCount > BinDataHeaderLength)
            {
                byte[] firstPackage = ConvertBinDataFromScpiData(tmpBuffer, currCount, out willRecvTotalBytes);
                if (firstPackage != null)
                {
                    if (firstPackage.Length > recvBuff.Length)
                    {
                        recvBuff = new byte[firstPackage.Length];
                    }
                    Array.Copy(firstPackage, 0, recvBuff, 0, firstPackage.Length);
                    recvedBytes = firstPackage.Length;
                }
            }
            if (recvedBytes >= willRecvTotalBytes)
                return recvedBytes;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 500)
            {
                result = visa32.viRead(viDev, tmpBuffer, tmpBuffer.Length, out currCount);
                if (result >= 0 && currCount > 0)
                {
                    if ((recvedBytes + currCount) <= recvBuff.Length && ((recvedBytes + currCount) <= willRecvTotalBytes))
                    {
                        int residueLength = recvBuff.Length - recvedBytes;
                        if (currCount > residueLength)
                        {
                            Array.Copy(tmpBuffer, 0, recvBuff, recvedBytes, residueLength);
                        }
                        else
                        {
                            Array.Copy(tmpBuffer, 0, recvBuff, recvedBytes, currCount);
                        }
                        recvedBytes += currCount;
                    }
                    else
                    {
                        Array.Copy(tmpBuffer, 0, recvBuff, recvedBytes, willRecvTotalBytes - recvedBytes);
                        recvedBytes = willRecvTotalBytes;

                    }
                    if (recvedBytes >= willRecvTotalBytes)
                        break;
                    stopwatch.Restart();
                }
                else
                {
                }
            }
            return recvedBytes;
        }

        public bool Open(Action<Boolean>? action)
        {
            errorAction = action;
            _bOpened = false;
            if (viDev != -1)
            {
                visa32.viClose(viDev);
                viDev = -1;
            }
            if (viDefRm != -1)
            {
                viDev = -1;
                visa32.viClose(viDefRm);
            }
            viErr = visa32.viOpenDefaultRM(out viDefRm);
            if (viErr != visa32.VI_SUCCESS)
            {
                viDefRm = -1;
                return false;
            }
            viErr = visa32.viOpen(viDefRm, m_addr, 0, m_timeout, out viDev);
            if (viErr != visa32.VI_SUCCESS)
            {
                viDev = -1;
                return false;
            }
            _bOpened = true;
            return true;
        }
        public void Close()
        {
            visa32.viClose(viDev);
        }
    }
    public class InstrumentSessionEngine
    {
        private static bool TryOpenInstrument(IInstrumentSession session, Action<Boolean>? errorAction, out string message)
        {
            string ret_message;
            if (session != null)
            {
                if (session.Open(errorAction))
                {
                    //visaSession.Close();
                    ret_message = "OK!";
                    message = ret_message;
                    return true;
                }
                else
                {
                    ret_message = "Defeated!!!!";
                    message = ret_message;
                    return false;
                }
            }
            else
            {
                ret_message = "未知仪器，其驱动目前不支持！！！！";
                message = ret_message;
                return false;
            }
        }
        public static IInstrumentSession? TryGetSession(string addr, string timeout, Action<bool>? errorAction, out string message)
        {
            message = "";
            if (addr.ToLower().IndexOf("gpib") >= 0)
            {
                IInstrumentSession returnSession = new VISA32Session(addr, Convert.ToInt32(timeout));
                if (TryOpenInstrument(returnSession, errorAction, out message))
                {
                    return returnSession;
                }
                else
                    return null;

            }
            else
            {
                IInstrumentSession returnSession = new VISASession(addr, Convert.ToInt32(timeout));
                if (TryOpenInstrument(returnSession, errorAction, out message))
                {
                    return returnSession;
                }
                else
                    return null;
            }
        }
        public static List<string> GetAllExistsResource()
        {
            Visa32Stub visa32Stub = new Visa32Stub(1000);
            return visa32Stub.GetAllExistsResource();
        }
        public static List<string> GetAllExistsResourceEx()
        {
            List<string> list = new List<string>();
            try
            {
                InstrumentEx.InstrumentManager instrumentManager = new InstrumentEx.InstrumentManager();
                Logger.Defualt.WriteLine($"开始获取资源信息");
                string[] arr = instrumentManager.FindAllResource();
                if (arr != null && arr.Length > 0)
                    list.AddRange(arr.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            catch (Exception ex)
            {
                Logger.Defualt.WriteLine($"通过InstrumentEx获取资源信息异常：{ex.Message}");
            }

            // 某些场景（如U2先启动后）InstrumentEx可能返回空，这里增加VISA32兜底查询。
            if (list.Count == 0)
            {
                try
                {
                    Logger.Defualt.WriteLine("InstrumentEx返回空，尝试使用VISA32兜底获取资源信息");
                    List<string> fallback = GetAllExistsResource();
                    if (fallback.Count > 0)
                        list.AddRange(fallback.Where(x => !string.IsNullOrWhiteSpace(x)));
                }
                catch (Exception ex)
                {
                    Logger.Defualt.WriteLine($"通过VISA32兜底获取资源信息异常：{ex.Message}");
                }
            }

            return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static bool TryProbeResource(string addr, int timeout)
        {
            try
            {
                IInstrumentSession? session;
                if (addr.ToLower().Contains("gpib"))
                    session = new VISA32Session(addr, timeout);
                else
                    session = new VISASession(addr, timeout);

                bool opened = session.Open();
                if (opened)
                    session.Close();
                return opened;
            }
            catch (Exception ex)
            {
                Logger.Defualt.WriteLine($"探测资源失败：{addr}，异常：{ex.Message}");
                return false;
            }
        }

        public static List<string> GetReachableResourceEx(int probeTimeout = 400)
        {
            List<string> all = GetAllExistsResourceEx();
            List<string> reachable = new List<string>();
            foreach (string addr in all)
            {
                if (TryProbeResource(addr, probeTimeout))
                    reachable.Add(addr);
                else
                    Logger.Defualt.WriteLine($"资源当前不可达，已过滤：{addr}");
            }
            return reachable;
        }

    }

}
