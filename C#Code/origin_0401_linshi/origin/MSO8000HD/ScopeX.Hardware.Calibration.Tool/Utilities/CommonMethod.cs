using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal class CommonMethod
    {
        /// <summary>
        /// 说明：把32位bit的数值，转化为数组，数组的值为位的下标；
        /// 例如：0b01000011 => {1，2，7}；
        /// </summary>
        /// <param name="bitSet"></param>
        /// <returns></returns>
        public static Int32[] BitsToArray(Int32 bitSet)
        {
            List<Int32> result = new List<Int32>();
            for (int i = 0; i < 32; i++)
            {
                if ((bitSet & (1 << i)) != 0)
                    result.Add(i + 1);
            }
            return result.ToArray();
        }
        /// <summary>
        /// 关闭数字处理功能
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="isEnable"></param>
        public static void Set_CorrectTiAdc(IInstrumentSession instrument, Boolean isEnable)
        {
            string scpiCMD = $":FACT:CALI:SPEC:DATA DebugVariant,bEnable_CorrectTiAdc:{isEnable}";
            instrument.WriteString(scpiCMD);
        }

        public static void SetDigitTrigger(IInstrumentSession instrument, Boolean isEnable)
        {
            string scpiCMD = $":FACT:CALI:SPEC:DATA DebugVariant,bEnable_DigitTrigger:{isEnable}";
            instrument.WriteString(scpiCMD);
        }

        public static void SetTemperatureCompensate(IInstrumentSession instrument, Boolean isEnable)
        {
            string scpiCMD = $":FACT:CALI:SPEC:DATA DebugVariant,bEnableAnalogTemperatureCompensate:{isEnable}";
            instrument.WriteString(scpiCMD);
        }

        public static void RefreshConstDataFromServer(IInstrumentSession instrument)
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "GetComModelConstData";
            instrument!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string recvStr = instrument.ReadString();
            if (recvStr == "")
                return;
            ServerDomainConstants.Convert(recvStr);
        }

        public static void SetChannelDelay(IInstrumentSession instrument, Boolean isEnable)
        {
            string scpiCMD = $":FACT:CALI:SPEC:DATA DebugVariant,bEnable_ChannelDelay:{isEnable}";
            instrument.WriteString(scpiCMD);
        }

        /// <summary>
        /// 获取通道的数据
        /// </summary>
        /// <returns></returns>
        public static List<ushort[]>? Factory_WaveData_Channel(IInstrumentSession instrument, int chnlDotCount = 25000)
        {
            //获取波形数据
            List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(instrument, 6_000);
            if (allChannelData == null)
                return null;

            //每个元素取25K
            for (int dataIndex = 0; dataIndex < allChannelData.Count; dataIndex++)
            {
                int newDataLength = chnlDotCount;
                ushort[] oldData = allChannelData[dataIndex];
                ushort[] newData = new ushort[newDataLength];
                Array.Copy(oldData, newData, newDataLength);
                allChannelData[dataIndex] = newData;
            }
            return allChannelData;
        }

        /// <summary>
        /// 中值过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="reserveNum"></param>
        /// <returns></returns>
        public static List<T> MiddleDataFilter<T>(List<T> data, int reserveNum)
            where T : IComparable<T>
        {
            List<T> objectList = new List<T>();
            data.Sort();

            int startIndex = (data.Count - reserveNum) / 2;
            for (int i = startIndex; i < startIndex + reserveNum; i++)
                objectList.Add(data[i]);

            //打印信息
            //StringBuilder msgSb = new StringBuilder();
            //msgSb.Append("srcData:");
            //data.ForEach(d => msgSb.Append(d.ToString() + ","));
            //msgSb.Append(";objectList:");
            //objectList.ForEach(d => msgSb.Append(d.ToString() + ","));
            //Logger.WriteLine($"MiddleDataFilter:{msgSb.ToString()};");

            return objectList;
        }
    }
}
