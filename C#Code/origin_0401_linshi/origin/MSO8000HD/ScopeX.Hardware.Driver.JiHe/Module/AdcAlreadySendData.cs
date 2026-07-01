using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal enum AdcConfigDataType
    {
        Gain,
        Phase,
        InputPort
    }
    /// <summary>
    /// 一个ADC同时只能从一个端口接入
    /// </summary>
    internal class AdcAlreadySendData
    {
        public int FPGAIndex;
        public int AdcIndex;
        public int Port;
        public int Data;
    }
    internal class AdcAlreadySendDataManager
    {
        public static AdcAlreadySendDataManager Default = new AdcAlreadySendDataManager();
        Dictionary<AdcConfigDataType, List<AdcAlreadySendData>> sentHistory = new Dictionary<AdcConfigDataType, List<AdcAlreadySendData>>()
        {
            [AdcConfigDataType.InputPort] = new List<AdcAlreadySendData>(),
            [AdcConfigDataType.Gain] = new List<AdcAlreadySendData>(),
            [AdcConfigDataType.Phase] = new List<AdcAlreadySendData>()
        };
        public void ClearSendHistory()
        {
            sentHistory[AdcConfigDataType.InputPort].Clear();
            sentHistory[AdcConfigDataType.Gain].Clear();
            sentHistory[AdcConfigDataType.Phase].Clear();
        }
        public List<AdcAlreadySendData> this[AdcConfigDataType type]
        {
            get
            {
                return sentHistory[type];
            }
        }
        public bool CheckNeedSend(AdcConfigDataType dataType,int fpgaIndex,int adcIndex,int port,int data)
        {
            bool bFound = false;
            bool bNeedSend = false;
            switch(dataType)
            {
                case AdcConfigDataType.InputPort:
                    foreach(var v in sentHistory[AdcConfigDataType.InputPort])
                    {
                        if (fpgaIndex==v.FPGAIndex && adcIndex==v.AdcIndex)
                        {
                            bFound = true;
                            if (port == v.Port)
                                bNeedSend = false;
                            else
                            {
                                v.Port = port;
                                bNeedSend = true;
                            }
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        sentHistory[AdcConfigDataType.InputPort].Add(new AdcAlreadySendData() { FPGAIndex = fpgaIndex, AdcIndex = adcIndex, Port = port });
                        bNeedSend = true;
                    }
                    break;
                case AdcConfigDataType.Gain:
                    foreach (var v in sentHistory[AdcConfigDataType.Gain])
                    {
                        if (fpgaIndex == v.FPGAIndex && adcIndex == v.AdcIndex)
                        {
                            bFound = true;
                            if (port == v.Port)
                            {
                                if (v.Data==data)
                                    bNeedSend = false;
                                else
                                {
                                    v.Data = data;
                                    bNeedSend = true;
                                }
                            }
                            else
                            {
                                v.Port = port;
                                v.Data = data;
                                bNeedSend = true;
                            }
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        sentHistory[AdcConfigDataType.Gain].Add(new AdcAlreadySendData() { FPGAIndex = fpgaIndex, AdcIndex = adcIndex, Port = port ,Data=data});
                        bNeedSend = true;
                    }
                    break;
                case AdcConfigDataType.Phase:
                    foreach (var v in sentHistory[AdcConfigDataType.Phase])
                    {
                        if (fpgaIndex == v.FPGAIndex && adcIndex == v.AdcIndex)
                        {
                            bFound = true;
                            if (port == v.Port)
                            {
                                if (v.Data == data)
                                    bNeedSend = false;
                                else
                                {
                                    v.Data = data;
                                    bNeedSend = true;
                                }
                            }
                            else
                            {
                                v.Port = port;
                                v.Data = data;
                                bNeedSend = true;
                            }
                            break;
                        }
                    }
                    if (!bFound)
                    {
                        sentHistory[AdcConfigDataType.Phase].Add(new AdcAlreadySendData() { FPGAIndex = fpgaIndex, AdcIndex = adcIndex, Port = port, Data = data });
                        bNeedSend = true;
                    }
                    break;
            }
            return bNeedSend;
        }
        public bool CheckNeedSend(AdcConfigDataType dataType, int fpgaIndex, int adcIndex, int port)
        {
            return CheckNeedSend(dataType, fpgaIndex, adcIndex, port,0);
        }
    }
}
