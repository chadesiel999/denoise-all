using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.Windows.Forms;
using ScopeX.Visa32;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class VISA32Session : IInstrumentSession
    {
        private Int32 viDefRm, viDev, viErr;
        private string m_addr;
        private Int32 m_timeout;
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

            if (length - lengthMarkLen - 2 < 0)
                return new Byte[0];
            byte[] resultData = new byte[length - lengthMarkLen - 2];
            Array.Copy(origin, lengthMarkLen + 2, resultData, 0, length - lengthMarkLen - 2);
            return resultData;
        }
        Action<Boolean>? errorAction = null;

        Visa32Stub visa32Stub;

        public VISA32Session(string addr, Int32 timeout)
        {
            viDev = -1;
            viDefRm = -1;
            m_addr = addr;
            m_timeout = timeout;

            //Process[] processes = Process.GetProcesses();
            //string visa32ServerProcessName = "ScopeX.Visa32Entry";
            //bool bNeedCreateVisa32ServerProcess = true;
            //foreach (Process process in processes) 
            //{ 
            //    if (process.ProcessName== visa32ServerProcessName) 
            //    {
            //        bNeedCreateVisa32ServerProcess = false;
            //        break;
            //    }
            //}
            //if (bNeedCreateVisa32ServerProcess)
            //{
            //    ProcessStartInfo startInfo = new();
            //    startInfo.UseShellExecute = true;
            //    startInfo.WorkingDirectory = Environment.CurrentDirectory;
            //    startInfo.FileName = "ScopeX.Visa32Entry.exe";
            //    //设置启动动作,确保以管理员身份运行
            //    startInfo.Verb = "runas";
            //    try
            //    {
            //        Process.Start(startInfo);
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
            visa32Stub = new Visa32Stub(500);
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
            int bOK = visa32Stub.viPrintf(viDev, cmdStr + "\n");
            if (errorAction != null)
                errorAction?.Invoke(bOK == visa32.VI_SUCCESS);
            return bOK == visa32.VI_SUCCESS;
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
            while (remain > 0)
            {
                currLength = remain > perPackageBytes ? perPackageBytes : remain;
                binData = InstrumentInteract.ConvertBinDataToScpiData(data, count - remain, currLength);
                Array.Copy(cmdBuffer, writebuffer, cmdBuffer.Length);
                Array.Copy(binData, 0, writebuffer, cmdBuffer.Length, binData.Length);
                int ok = visa32Stub.viWrite(viDev, writebuffer, cmdBuffer.Length + binData.Length, out outCount);
                errorAction?.Invoke(ok == visa32.VI_SUCCESS);
                if (ok != visa32.VI_SUCCESS)
                    return false;
                Thread.Sleep(1);
                remain -= currLength;
            }
            binData = InstrumentInteract.ConvertBinDataToScpiData(data, 0, 0);
            Array.Copy(cmdBuffer, writebuffer, cmdBuffer.Length);
            Array.Copy(binData, 0, writebuffer, cmdBuffer.Length, binData.Length);
            visa32Stub.viWrite(viDev, writebuffer, cmdBuffer.Length + binData.Length, out outCount);
            return true;
        }
        public bool WriteBinData(byte[] data, int count)
        {
            int outCount = 0;
            visa32Stub.viWrite(viDev, data, count, out outCount);
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

            visa32Stub.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            while (resultCount > 0)
            {
                recvCount += resultCount;
                Range range = 0..resultCount;
                recvBuffer.AddRange(tmpBuffer[range]);
                resultCount = 0;
                //for (int i = 0; i < resultCount; i++)
                //    recvBuffer.Add(tmpBuffer[i]);
                //visa32Stub.viRead(viDev, tmpBuffer, maxLength, out resultCount);
            }
            return System.Text.Encoding.Default.GetString(recvBuffer.ToArray(), 0, recvCount).Trim();
        }
        public string ReadString()
        {
            int resultCount = 0;
            int recvCount = 0;

            List<byte> recvBuffer = new List<byte>();

            int ok = visa32Stub.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            //errorAction?.Invoke(ok == visa32.VI_SUCCESS);
            if (ok != visa32.VI_SUCCESS)
                return "";
            while (resultCount > 0)
            {
                recvCount += resultCount;
                recvBuffer.AddRange(tmpBuffer.Take(resultCount));

                //for (int i = 0; i < resultCount; i++)
                //    recvBuffer.Add(tmpBuffer[i]);
                visa32Stub.viRead(viDev, tmpBuffer, MAX_Length, out resultCount);
            }
            return System.Text.Encoding.Default.GetString(recvBuffer.ToArray(), 0, recvCount).Trim();
        }

        private const Int32 _BuffSize = 4 * 1024;

        public Byte[] ReadBinData()
        {
            List<Byte> result = new();

            Int32 recvbytes = 0;
            Byte[] tmpbuff = new Byte[_BuffSize];
            do
            {
                Int32 ret = visa32Stub.viRead(viDev, tmpBuffer, _BuffSize, out recvbytes);
                if (ret < 0)
                    break;
                Byte[] package = ConvertBinDataFromScpiData(tmpBuffer, recvbytes, out Int32 needrecvbytes);
                result.AddRange(package);
                if (recvbytes >= needrecvbytes)
                    break;
            } while (recvbytes != 0);

            return result.ToArray();
        }

        public int ReadBinData(ref Byte[] recvBuff)
        {
            int currCount = 0;
            byte[] tmpBuffer = new byte[1024 * 4];
            int recvedBytes = 0;
            int result = visa32Stub.viRead(viDev, tmpBuffer, 4 * 1024, out currCount);
            int willRecvTotalBytes = 0;
            if (currCount > BinDataHeaderLength)
            {
                byte[] firstPackage = ConvertBinDataFromScpiData(tmpBuffer, currCount, out willRecvTotalBytes);
                if (firstPackage != null)
                {
                    Array.Copy(firstPackage, 0, recvBuff, 0, firstPackage.Length);
                    recvedBytes = firstPackage.Length;
                }
            }
            if (recvedBytes >= willRecvTotalBytes)
                return recvedBytes;
            while (result >= 0 && currCount > 0)
            {
                result = visa32Stub.viRead(viDev, tmpBuffer, 4 * 1024, out currCount);
                if (result >= 0 && currCount > 0)
                {
                    if ((recvedBytes + currCount) <= recvBuff.Length && ((recvedBytes + currCount) <= willRecvTotalBytes))
                    {
                        Array.Copy(tmpBuffer, 0, recvBuff, recvedBytes, currCount);
                        recvedBytes += currCount;
                    }
                    else
                    {
                        Array.Copy(tmpBuffer, 0, recvBuff, recvedBytes, willRecvTotalBytes - recvedBytes);
                        recvedBytes = willRecvTotalBytes;

                    }
                    if (recvedBytes >= willRecvTotalBytes)
                        break;
                }
                else
                    break;
            }
            return recvedBytes;
        }

        public bool Open(Action<Boolean>? action)
        {
            errorAction = action;
            _bOpened = false;
            if (viDev != -1)
            {
                visa32Stub.viClose(viDev);
                viDev = -1;
            }
            if (viDefRm != -1)
            {
                viDev = -1;
                visa32Stub.viClose(viDefRm);
            }
            viErr = visa32Stub.viOpenDefaultRM(out viDefRm);
            if (viErr != visa32.VI_SUCCESS)
            {
                viDefRm = -1;
                return false;
            }
            viErr = visa32Stub.viOpen(viDefRm, m_addr, 0, m_timeout, out viDev);
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
            visa32Stub.viClose(viDev);
        }
    }
}
