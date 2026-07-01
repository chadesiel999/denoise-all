using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using CyUSB;

namespace ScopeX.Hardware.Driver
{
    internal class Driver_CyUsb3_0 : IDriver
    {
        private CyUSBDevice? CurrDevice;//USB设备
        private USBDeviceList? CurrDeviceList;//设备列表
        private CyControlEndPoint? CtrlEndPt;//控制端点
        private CyBulkEndPoint? inEndpoint;//输入端点
        private CyBulkEndPoint? outEndpoint;//输出端点
        private bool bOpened = false;
        private bool Config(string id)
        {
            bool isOpen = false;

            if (CurrDevice != null)
            {
                outEndpoint = (CyBulkEndPoint)CurrDevice.EndPointOf(0x01);
                inEndpoint = (CyBulkEndPoint)CurrDevice.EndPointOf(0x81);
                CtrlEndPt = (CyControlEndPoint)CurrDevice.EndPointOf(0x00);
                if ((outEndpoint != null) && (inEndpoint != null))
                {
                    //make sure that the device configuration doesn't contain the other than bulk endpoint
                    if ((outEndpoint.Attributes & 0x03/*0,1 bit for type of transfer*/) != 0x02/*Bulk endpoint*/|| (inEndpoint.Attributes & 0x03) != 0x02)
                    {
                        //isOpen = false;
                    }
                    outEndpoint.TimeOut = 1000;
                    inEndpoint.TimeOut = 1000;
                    isOpen = true;
                }
                else
                {
                    //isOpen = false;
                }
            }
            return isOpen;
        }
        public bool bOpen
        {
            get => bOpened;
            set => bOpened = value;
        }
        public Driver_CyUsb3_0(string id)
        {
            CurrDeviceList = new USBDeviceList(CyConst.DEVICES_CYUSB);//设备列表，普通USB设备
            for (int j = 0; j < CurrDeviceList.Count; j++)
            {
                if (CurrDeviceList[j].VendorID.Equals(0X04B4) && CurrDeviceList[j].ProductID.Equals(0X00F1) && CurrDeviceList[j].SerialNumber.Contains(id))
                {
                    CurrDevice = (CyUSBDevice)CurrDeviceList[j];
                    //bandname = Regex.Match(MyDevice.SerialNumber, @"ES7336[A-Z]\d[A-Z]\d{7}").Value;
                    Console.WriteLine(CurrDevice.SerialNumber);
                    Open(id);
                }
                else
                {
                    CurrDevice = null;
                }
            }
        }
        public bool Open(string id)
        {
            bOpened = Config(id);
            return bOpened;
        }
        public void Close()
        {
            CurrDevice = null;
        }
        public void WriterRegister(UInt32 registerAddress, UInt32 data)
        {
            bool result = false;
            if (outEndpoint == null)
            {
                return;
            }
            int len = 4;
            byte[] sendbuffer = new byte[len];
            //FPGA寄存器地址高16位，数据低16位。
            sendbuffer = BitConverter.GetBytes((registerAddress << 16) | (data & 0x0000ffff));
            result = outEndpoint.XferData(ref sendbuffer, ref len);
        }
        public UInt32 ReadRegister(UInt32 registerAddress)
        {
            bool result = false;
            if (inEndpoint == null || outEndpoint == null)
            {
                return 0;
            }
            int len = 4;
            int len1 = 2048;
            byte[] addrbuffer = new byte[len];
            byte[] databuffer = new byte[len1];
            addrbuffer = BitConverter.GetBytes((registerAddress << 16) | 0x80000000);
            lock (inEndpoint)
            {
                outEndpoint.XferData(ref addrbuffer, ref len);
                //Thread.Sleep(5);
                result = inEndpoint.XferData(ref databuffer, ref len1);
            }
            //Marshal.Copy(databuffer, 0, (IntPtr)pdest, len);
            //usbdSta = Convert.ToString(inEndpoint.UsbdStatus, 16);
            return BitConverter.ToUInt32(databuffer, 0);
        }
        public bool DMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            bool result = false;
            if (inEndpoint == null || outEndpoint == null)
            {
                return result;
            }
            int len = 4;
            byte[] sendbuffer = new byte[len];

            sendbuffer = BitConverter.GetBytes(((fromAddress << 16) | 0xC0000000)/*|bytecount/4*/);
            lock (inEndpoint)
            {
                outEndpoint.XferData(ref sendbuffer, ref len);
                //Thread.Sleep(5);
                //////同步接收。使用同步接收，如果USB驱动端没有做pingpong，则易导致数据没有完写入USB芯片就开始接收，可能会导致端点挂起或接收数据长度不对
                int singlereceivebytes = (int)needReadBytes;
                byte[] inbuffer = new byte[singlereceivebytes];
                result = inEndpoint.XferData(ref receiveData, ref singlereceivebytes);
            }
            return result;
        }

        //直接读Card数据
        public Boolean RawDMARead(UInt32 fromAddress, UInt32 needReadBytes, ref Byte[] receiveData)
        {
            return false;
        }

        public bool DMAWrite(byte[] data, UInt32 byteCount)
        {
            bool result = false;
            if (inEndpoint == null || outEndpoint == null)
            {
                return result;
            }
            //int len = 4;
            int sendlength = (int)byteCount;
            ////byte[] addrbuffer = new byte[len];
            //byte[] databuffer = new byte[sendlength];
            ////addrbuffer = BitConverter.GetBytes(((addr << 16) | 0x40000000)|bytecount);
            ////outEndpoint.XferData(ref addrbuffer, ref len);
            //Marshal.Copy((IntPtr)pdestbuffer, databuffer, 0, (int)byteCount);
            //databuffer[3] = (byte)(0x40 | databuffer[3]);
            result = outEndpoint.XferData(ref data, ref sendlength);
            return result;
        }
        public bool DMAWrite(UInt32 startAddress, byte[] data, UInt32 byteCount)
        {
            return false;
        }
        #region 
        #endregion
    }
}
