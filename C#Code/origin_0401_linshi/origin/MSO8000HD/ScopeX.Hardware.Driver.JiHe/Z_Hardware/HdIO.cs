//#define FPGA_Debug
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
namespace ScopeX.Hardware.Driver
{
    internal static class HdIO
    {
        internal static int PerWriteRegUsedNs
        {
            get;
            private set;
        } = 1;
        internal static void QueryPerWriteRegUsedNs()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10_000; i++)
                HdIO.WriteReg(0x8000, 0);
            stopwatch.Stop();
            PerWriteRegUsedNs = (int)(stopwatch.ElapsedMilliseconds * 1000 * 1000 / 10_000);
        }
        internal static void Sleep(int ms)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
                Thread.Sleep(ms);
        }
        internal static IDriver? CurrDriver = null;
        internal static IDriver? CurrUsb30Driver = null;
        internal static void WriteReg(UInt32 regAddr, UInt32 data)
        {
            AppendLineMessage?.Invoke("Write to register& ,address=" + regAddr.ToString("X").PadLeft(4, '0') + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister(regAddr, data);
        }
        internal static void WriteReg(UInt32 regAddr, UInt32 data, int delayUs)
        {
            AppendLineMessage?.Invoke("Write to register& ,address=" + regAddr.ToString("X").PadLeft(4, '0') + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister(regAddr, data);
            DelayByUs(delayUs);
        }
        internal static UInt32 ReadRegNoDebug(UInt32 regAddr)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
            {
                UInt32 data = CurrDriver.ReadRegister((uint)regAddr);
                return CurrDriver.ReadRegister(regAddr);
            }
            return 0;
        }
        internal static UInt32 ReadReg(UInt32 regAddr)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
            {
                UInt32 data = CurrDriver.ReadRegister((uint)regAddr);
                AppendLineMessage?.Invoke("Read From& 0x" + regAddr.ToString("X").PadLeft(4, '0') + "=0x" + data.ToString("X").PadLeft(4, '0'));
                return data;

            }
            return 0;
        }

#if !Product_B21_JinHui_PXI
        internal static void WriteReg(ProcBdReg.W regAddr, UInt32 data)
        {
            AppendLineMessage?.Invoke("Write To proc Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(ProcBdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister((uint)regAddr, data);
            //UInt32 readBackData = CurrDriver.ReadRegister((uint)regAddr);

            // HdIO.Sleep(1);
        }
        internal static void WriteReg(ProcBdReg.W regAddr, UInt32 data, int delayUs)
        {
            AppendLineMessage?.Invoke("Write To proc Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(ProcBdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister((uint)regAddr, data);
            DelayByUs(delayUs);
  //          HdIO.Sleep(1);
        }

        internal static void WriteReg(S6BdReg.W regAddr, UInt32 data)
        {
            AppendLineMessage?.Invoke("Write To proc Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(S6BdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister((uint)regAddr, data);
       //     HdIO.Sleep(1);
        }
        internal static void WriteReg(S6BdReg.W regAddr, UInt32 data, int delayUs)
        {
            AppendLineMessage?.Invoke("Write To proc Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(S6BdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));

            CurrDriver?.WriterRegister((uint)regAddr, data);
            DelayByUs(delayUs);
  //          HdIO.Sleep(1);
        }

        internal static void WriteReg(PcieBdReg.W regAddr, UInt32 data)
        {
            AppendLineMessage?.Invoke("Write To Pcie Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(PcieBdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));
            CurrDriver?.WriterRegister((uint)regAddr, data);
       //     HdIO.Sleep(1);
        }
        internal static void WriteReg(PcieBdReg.W regAddr, UInt32 data, int delayUs)
        {

            AppendLineMessage?.Invoke("Write To Pcie Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(PcieBdReg.W), regAddr.ToString()) + ", value=0x" + data.ToString("X").PadLeft(4, '0'));
            CurrDriver?.WriterRegister((uint)regAddr, data);
            DelayByUs(delayUs);
        }

        internal static UInt32 ReadReg(ProcBdReg.R regAddr)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
            {
                UInt32 readBackData= CurrDriver.ReadRegister((uint)regAddr);
        //        HdIO.Sleep(1);
                readBackData = CurrDriver.ReadRegister((uint)regAddr);
                AppendLineMessage?.Invoke("Read from proc Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(ProcBdReg.W), regAddr.ToString()+",readBackData="+readBackData.ToString()));
                return readBackData;
            }
            return 0;
        }
        internal static UInt32 ReadReg(S6BdReg.R regAddr)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
            {
                UInt32 data = CurrDriver.ReadRegister((uint)regAddr);
                data = CurrDriver.ReadRegister((uint)regAddr);
                AppendLineMessage?.Invoke("Read Pcie Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(S6BdReg.R), regAddr.ToString()) + ".return value=0x" + data.ToString("X").PadLeft(4, '0'));
                return data;
            }
            return 0;
        }
        internal static UInt32 ReadReg(PcieBdReg.R regAddr)
        {
            if (CurrDriver != null && CurrDriver.bOpen)
            {
                UInt32 data = CurrDriver.ReadRegister((uint)regAddr);
                data = CurrDriver.ReadRegister((uint)regAddr);
                AppendLineMessage?.Invoke("Read Pcie Board&,name=" + regAddr.ToString() + ",address=" + GetFPGARegisterDescription?.Invoke(typeof(PcieBdReg.R), regAddr.ToString()) + ".return value=0x" + data.ToString("X").PadLeft(4, '0'));
                return data;
            }
            return 0;
        }
#endif
        internal static bool CheckRegisterValue(uint address,UInt32 markCode, UInt32 eqValue,int overtimeByMs, CancellationToken? softResetToken = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            UInt32 v = ReadReg(address) & markCode;
            while(v!=eqValue && stopwatch.ElapsedMilliseconds< overtimeByMs && ((softResetToken?.IsCancellationRequested ?? false) == false))
                v = ReadReg(address) & markCode;
            return v == eqValue;
        }
        internal static bool CheckRegisterValue(AcqBdReg.R address, UInt32 markCode ,UInt32 eqValue, int overtimeByMs, CancellationToken? softResetToken = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool bError = false;
            while (stopwatch.ElapsedMilliseconds < overtimeByMs && ((softResetToken?.IsCancellationRequested ?? false) == false))
            {
                bError = false;
                for (int fpgaIndex=0; fpgaIndex< Hd.CurrProduct!.AcqBd!.ExistsDefines.Count ; fpgaIndex++)
                {
                    if (Hd.CurrProduct.AcqBd.ExistsDefines[fpgaIndex].ISENABLE)
                    {
                        if ((Hd.CurrProduct.AcqBd.ReadReg(address, (AcqBdNo)fpgaIndex) & markCode) != eqValue)
                        {
                            bError = true;
                        }
                    }
                }
                if (!bError)
                    break;
            }
            return !bError;
        }
        internal static bool DMARead(UInt32 needReadBytes, ref Byte[] receiveData)
        {
            bool bOK = false;
            CurrDriver?.WriterRegister((uint)PcieBdReg.W.ReadTotalBytes, needReadBytes);
            bOK = CurrDriver?.DMARead(0, needReadBytes, ref receiveData) ?? false;

            return bOK;
        }
        internal static bool DMARead(UInt32 startAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            bool bOK = false;
            CurrDriver?.WriterRegister((uint)PcieBdReg.W.ReadTotalBytes, needReadBytes);
            bOK = CurrDriver?.DMARead(startAddress, needReadBytes, ref receiveData) ?? false;

            return bOK;
        }

        internal static bool RawDMARead(UInt32 startAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            CurrDriver?.WriterRegister((uint)PcieBdReg.W.ReadTotalBytes, needReadBytes);
            bool bOK = CurrDriver?.RawDMARead(startAddress, needReadBytes, ref receiveData) ?? false;
            return bOK;
        }



        internal static Boolean DMAWrite(UInt32 startAddr, Byte[] data, UInt32 byteCount)
        {
            return CurrDriver?.DMAWrite(startAddr, data, byteCount) ?? false;
        }
#region us级别的延迟函数
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);
        //定义延迟函数
        internal static void DelayByUs(long delay_Time_us)
        {
            long stop_Value = 0;
            long start_Value = 0;
            long freq = 0;
            long n = 0;

            QueryPerformanceFrequency(ref freq);  //获取CPU频率
            long count = delay_Time_us * freq / 1_000_000;   //这里写成1_000_000就是微秒，写成1_000就是毫秒
            QueryPerformanceCounter(ref start_Value); //获取初始前值

            while (n < count) //不能精确判定
            {
                QueryPerformanceCounter(ref stop_Value);//获取终止变量值
                n = stop_Value - start_Value;
            }
        }
        internal static void WaitForSpiTransfer(int spiClockWithMHz, int byteCount)
        {
            long delay_Time_ns = 1_000 * 8 * byteCount / spiClockWithMHz;//ns
            if ((delay_Time_ns * 3 / 2) < PerWriteRegUsedNs) // *3/2,将需要的时间 * 1.5，避免PerWriteRegUsedNs统计误差
                return;
            long delay_Time_us = delay_Time_ns / 1_000;//=>us
            delay_Time_us++;
            DelayByUs(delay_Time_us);
        }
#endregion
#region FPGA_RegisterDebug
        private static void localAppendLineMessageFunc(string message)
        {
            stringBuilder?.AppendLine(message);
        }
        private static void SavetoDiskFunc()
        {
            string datetimeStr = DateTime.Now.ToString().Replace('/', '_');
            datetimeStr = datetimeStr.Replace(':', '_');
            string fileName = $"{AppDomain.CurrentDomain.BaseDirectory} FPGA_RegisterDebug_log{datetimeStr}.txt";
            File.WriteAllText(fileName, stringBuilder?.ToString());
        }

        private static string GetFPGARegisterDescriptionFunc(Type type, string propertyName)
        {
            FieldInfo[] arryProperty = type.GetFields();
            if (arryProperty != null)
            {
                foreach (FieldInfo fieldInfo in arryProperty)
                {
                    if (fieldInfo.Name == propertyName)
                    {
                        var regAttribute = fieldInfo.GetCustomAttribute(typeof(RegAttribute));
                        if (regAttribute != null)
                            return ((RegAttribute)regAttribute).Description;
                        else
                            return "Unknow";
                    }
                }
            }
            return string.Empty;
        }
        private static string FindFPGARegisterDescription(Type type, string propertyName)
        {
            if (allRegistersDescription.ContainsKey(type))
            {
                if (allRegistersDescription[type].ContainsKey(propertyName))
                    return allRegistersDescription[type][propertyName];
            }
            return string.Empty;
        }
        private static void BuildOneType(Type type)
        {
            if (allRegistersDescription.ContainsKey(type))
                return;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            FieldInfo[] arryProperty = type.GetFields();
            foreach (FieldInfo fieldInfo in arryProperty)
            {
                Attribute? attribute = fieldInfo.GetCustomAttribute(typeof(RegAttribute));
                if (attribute != null)
                    dictionary.Add(fieldInfo.Name, ((RegAttribute)attribute).Description);
            }
            allRegistersDescription.Add(type, dictionary);
        }
        private static void BuildAllRegisterDescription()
        {
            if (!Hd.BFirstPower)
                return;
            BuildOneType(typeof(AcqBdReg.W));
            BuildOneType(typeof(AcqBdReg.R));

            BuildOneType(typeof(ProcBdReg.W));
            BuildOneType(typeof(ProcBdReg.R));

            BuildOneType(typeof(PcieBdReg.W));
            BuildOneType(typeof(PcieBdReg.R));
        }
        internal static Func<Type, String, String>? GetFPGARegisterDescription = GetFPGARegisterDescriptionFunc;
        internal static Action? Init = BuildAllRegisterDescription;
        internal static Dictionary<Type, Dictionary<string, string>> allRegistersDescription = new Dictionary<Type, Dictionary<string, string>>();
#if FPGA_Debug
        internal static StringBuilder? stringBuilder = new StringBuilder();
        internal static Action? SaveLogToDisk = SavetoDiskFunc;
        internal static Action<string>? AppendLineMessage = localAppendLineMessageFunc;
#else
        internal static Action<string>? AppendLineMessage;
        internal static StringBuilder? stringBuilder;
        internal static Action? SaveLogToDisk;
#endif
#endregion
    }
    internal class RegAttribute : Attribute
    {
        public string Description { get; set; }
        public RegAttribute()
        {
            Description = "";
        }
    }
}
