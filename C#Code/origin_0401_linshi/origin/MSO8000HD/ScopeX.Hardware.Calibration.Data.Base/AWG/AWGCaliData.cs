using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.Hardware.Calibration.Data.Base.AWGCaliData.ArbWfmDcCailAmpTable;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class AWGCaliData : ICaliData
    {
        #region Calib
        public const double AWG_CAL_FREQ_1K = 1000.0; //1 k
        public const double AWG_CAL_FREQ_MIN_RANGE = 150e3; //150 k
                                                            //public const Int32 AWG_CAL_PIONT_ARRAY_MAX = 70;
                                                            //public const Int32 AWG_CAL_ARRAY_SCALE_MAX = 1; // todo  =>2
        public const double AWG_CAL_DC_LOAD_GAIN = 1;
        //粗调校正值
        public const double AWG_CAL_DAC_FINE_GAIN = (100.0 / (100.0 + 910.0));
        //粗调校正值
        public const double AWG_CAL_DAC_COARSE_GAIN = (1.0 - AWG_CAL_DAC_FINE_GAIN);
        //衰减档位
        public const Int32 AWG_CAL_ATT_LEVEL_NUM = 4;
        //public const double AWG_AC_CAL_STEP = (double)AWG_CAL_MAX_FREQ / AWG_AC_CAL_SIZE;// todo
        public const double AWG_AC_CAL_STEP = 152.587890625 * 1000 * 2; //Hz - 152.587890625 kHz
        public const Int32 AWG_CAL_MAX_FREQ = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000);
        //public const Int32 AWG_AC_CAL_SIZE = 1024; //MAX_FREQ / AC_CAL_STEP; //1024
        public const Int32 AWG_AC_CAL_SIZE = AWG_AC_CAL_SIZE_MAX;//(Int32)(AWG_CAL_MAX_FREQ / AWG_AC_CAL_STEP);
        public const Int32 AWG_AC_CAL_SIZE_MAX = 2048;
        public const Int32 AMP_CAL_MAX = ((1 << 16) - 1); //todo 23.1.29 =>16
        public const Int32 AMP_DEFAULT = AMP_CAL_MAX;//((ushort)(AMP_CAL_MAX/1.4))  // 20479 = 5v

        public const Int32 AWG_MAX_NUM = 2;
        #endregion Calib

        public static AWGCaliData Default = new AWGCaliData();
        private AWGCaliChannel[] _ChannelData;
        public AWGCaliChannel[] ChannelData { get => _ChannelData; }
        private AWGCaliData()
        {
            _ChannelData = new AWGCaliChannel[AWG_MAX_NUM];

            byte[] buffer = Serialize();

            _TotalBytes = buffer.Length;

        }
        private int _TotalBytes = 0;
        public int TotalBytes => _TotalBytes;
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public CaliDataType DataType => CaliDataType.AWG;

        public void UpdateDatas(ChannelId id, AWGCaliChannel caliChannel)
        {
            _ChannelData[id - ChannelId.AWG1] = caliChannel;
        }

        public void Deserialize(byte[] content)
        {
            if (content.Length < TotalBytes)
                return;
            int byteIndex = 0;
            var AdjustStructSize = 16;
            var _AC_Ponits = ((content.Length - ((sizeof(UInt64) * 2) + (AdjustStructSize * 2) + 8 * AWG_CAL_ATT_LEVEL_NUM) * AWG_MAX_NUM)) / AWG_MAX_NUM / (sizeof(UInt64) * 8);
            for (int ch = 0; ch < AWG_MAX_NUM; ch++)
            {
                if (_ChannelData[ch] == null)
                {
                    _ChannelData[ch] = new();
                }
                _ChannelData[ch].DataMark = Helper.BytesToStruct<UInt64>(content, byteIndex, typeof(UInt64));
                byteIndex += sizeof(UInt64);
                _ChannelData[ch].ECC = Helper.BytesToStruct<UInt64>(content, byteIndex, typeof(UInt64));
                byteIndex += sizeof(UInt64);
                for (int i = 0; i < _ChannelData[ch].DC_CAL.Length; i++)
                    _ChannelData[ch].DC_CAL[i].Deserialize(ref byteIndex, content);
                for (int attLv = 0; attLv < AWG_CAL_ATT_LEVEL_NUM; attLv++)
                {
                    _ChannelData[ch].AC_CAL[attLv].Amp = Helper.BytesToStruct<double>(content, byteIndex, typeof(double));
                    byteIndex += sizeof(double);
                    int readyPoints = 0;
                    foreach (AdjustValue adValue in _ChannelData[ch].AC_CAL[attLv].AcCal)
                    {
                        if (readyPoints < _AC_Ponits)
                        {
                            adValue.Deserialize(ref byteIndex, content);
                        }
                        readyPoints++;
                        if (byteIndex > content.Length)
                        {
                            //数据无效
                            _ChannelData[ch] = new();
                            break;
                        }
                    }

                }
                if (_ChannelData[ch] != null)
                {
                    SetCHAmpData(ChannelId.AWG1 + ch, _ChannelData[ch]);
                }
            }
        }

        public byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            for (int ch = 0; ch < AWG_MAX_NUM; ch++)
            {
                if (_ChannelData[ch] == null)
                {
                    _ChannelData[ch] = new();
                }
                memoryStream.Write(Helper.StructToBytes(_ChannelData[ch].DataMark));
                memoryStream.Write(Helper.StructToBytes(_ChannelData[ch].ECC));
                foreach (var adValue in _ChannelData[ch].DC_CAL)
                {
                    memoryStream.Write(adValue.Serialize());
                }
                for (int attLv = 0; attLv < AWG_CAL_ATT_LEVEL_NUM; attLv++)
                {
                    memoryStream.Write(Helper.StructToBytes(_ChannelData[ch].AC_CAL[attLv].Amp));
                    foreach (var oneAdjustValue in _ChannelData[ch].AC_CAL[attLv].AcCal)
                        memoryStream.Write(oneAdjustValue.Serialize());
                }
                //foreach (var oneList1 in _ChannelData[ch].AC_CAL)
                //{
                //	foreach (var oneACAdjustValue in oneList1)
                //	{
                //		memoryStream.Write(Helper.StructToBytes(oneACAdjustValue.Amp));
                //		foreach (var oneAdjustValue in oneACAdjustValue.AcCal)
                //			memoryStream.Write(oneAdjustValue.Serialize());
                //	}
                //}
                // <Remark>更改人：彭博 创建日期：2023/12/14 15:57:00  原因：AWG模块未校准或者无数据是没有输出波形，设置默认幅度 </Remark>
                if (_ChannelData[ch] != null)
                {
                    SetCHAmpData(ChannelId.AWG1 + ch, _ChannelData[ch]);
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }

        public class AWGCaliChannel
        {
            #region 数据体
            public UInt64 DataMark;
            public UInt64 ECC;
            public AdjustValue[] DC_CAL = new AdjustValue[AWG_CAL_ATT_LEVEL_NUM];
            public AdjustValue[] AC1K_CAL = new AdjustValue[AWG_CAL_ATT_LEVEL_NUM];
            public ACAdjustValue[] AC_CAL = new ACAdjustValue[AWG_CAL_ATT_LEVEL_NUM];
            #endregion  数据体

            public AWGCaliChannel()
            {
                for (int i = 0; i < AWG_CAL_ATT_LEVEL_NUM; i++)
                {
                    DC_CAL[i] = new AdjustValue();
                    AC1K_CAL[i] = new AdjustValue();
                    AC_CAL[i] = new ACAdjustValue();
                }
            }
        }

        public class AdjustValue
        {
            public double FreqOrDcVol = 0;
            public double Rate = 1.0;
            public static int TotalBytes => sizeof(double) * 2;
            internal void Deserialize(ref int startIndex, byte[] content)
            {
                FreqOrDcVol = Helper.BytesToStruct<double>(content, startIndex, typeof(double));
                startIndex += sizeof(double);
                Rate = Helper.BytesToStruct<double>(content, startIndex, typeof(double));
                startIndex += sizeof(double);
            }
            internal byte[] Serialize()
            {
                List<byte> result = new List<byte>();
                result.AddRange(Helper.StructToBytes(FreqOrDcVol));
                result.AddRange(Helper.StructToBytes(Rate));
                return result.ToArray();
            }
        }

        public class ACAdjustValue
        {
            public double Amp;
            public AdjustValue[] AcCal = new AdjustValue[AWG_AC_CAL_SIZE];// AdjustValue[Constants.AWG_AC_CAL_SIZE];

            public int TotalBytes
            {
                get
                {
                    int total = sizeof(double);//Amp
                    int sizeofAdjustValue = sizeof(double) * 2;//FreqOrDcVol+Rate
                    total += sizeofAdjustValue * AcCal.Length;
                    return total;
                }
            }
            public ACAdjustValue()
            {
                for (int i = 0; i < AWG_AC_CAL_SIZE; i++)
                {
                    AcCal[i] = new AdjustValue();

                }
            }
        }

        public static class ArbWfmDcCailAmpTable
        {
            //	{//HizVpp att       refatt maxfreq maxdc  
            //	{0.04, 3, /*1001*/  0xff, DBL_MAX,  1, GetDcCalTablAddr(CH1, CAL_ID_ATT200mV),GetAC1KCalTablAddr(CH1, CAL_ID_ATT200mV),CalTablBuff[CH1].AC_CAL[2]
            //	},
            //	{0.2,  1, /*0101*/  0xff, DBL_MAX,  1, GetDcCalTablAddr(CH1, CAL_ID_ATT800mV),GetAC1KCalTablAddr(CH1, CAL_ID_ATT800mV),CalTablBuff[CH1].AC_CAL[3]
            //},
            //	{ 1,   2,/*0001*/  0xff, DBL_MAX,    5, GetDcCalTablAddr(CH1, CAL_ID_ATT4V),   GetAC1KCalTablAddr(CH1, CAL_ID_ATT4V),   CalTablBuff[CH1].AC_CAL[4]},
            //	{ 5,   0,/*0000*/  0xff, DBL_MAX,  	5, GetDcCalTablAddr(CH1, CAL_ID_ATT20V),  GetAC1KCalTablAddr(CH1, CAL_ID_ATT20V),  CalTablBuff[CH1].AC_CAL[5]} 
            //	},
            public static List<int> CAIL_LV_SHORT = new() { 3, 1, 2, 0 };
            public static List<ArbWfmCailAmpData>[] CailAmpDatas = new List<ArbWfmCailAmpData>[AWG_MAX_NUM]
            {
            new List<ArbWfmCailAmpData>
            {
                new ArbWfmCailAmpData() { MaxVpp = 6     ,AttLV = 0, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 3 },
                new ArbWfmCailAmpData() { MaxVpp = 0.2   ,AttLV = 1, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 0.6 },
                new ArbWfmCailAmpData() { MaxVpp = 1.2     ,AttLV = 2, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 3 },
                new ArbWfmCailAmpData() { MaxVpp = 0.04  ,AttLV = 3, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 0.6 },
            },
            new List<ArbWfmCailAmpData>
            {
                new ArbWfmCailAmpData() { MaxVpp = 6     ,AttLV = 0, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) ,  MaxDC = 3 },
                new ArbWfmCailAmpData() { MaxVpp = 0.2   ,AttLV = 1, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) ,  MaxDC = 0.6 },
                new ArbWfmCailAmpData() { MaxVpp = 1.2     ,AttLV = 2, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000)  , MaxDC = 3 },
                new ArbWfmCailAmpData() { MaxVpp = 0.04  ,AttLV = 3, RefAttEnable = false, MaxFreq = (int)(Constants.AWG_SIN_FRQ_MAX / 1000_000) ,  MaxDC = 0.6 },
             }
                };

            public static void SetCHAmpData(ChannelId id, AWGCaliChannel calData)
            {
                if (id < ChannelId.AWG1 || id > ChannelId.AWG2)
                {
                    return;
                }
                for (int i = 0; i < AWGCaliData.AWG_CAL_ATT_LEVEL_NUM; i++)
                {
                    CailAmpDatas![id - ChannelId.AWG1]![i].AC1K = new();
                    CailAmpDatas![id - ChannelId.AWG1]![i].AC1K!.FreqOrDcVol = calData.AC1K_CAL[i].FreqOrDcVol;
                    CailAmpDatas![id - ChannelId.AWG1]![i].AC1K!.Rate = calData.AC1K_CAL[i].Rate;

                    CailAmpDatas![id - ChannelId.AWG1]![i].DC = new();
                    CailAmpDatas[id - ChannelId.AWG1]![i].DC!.FreqOrDcVol = calData.DC_CAL[i].FreqOrDcVol;
                    CailAmpDatas[id - ChannelId.AWG1][i].DC!.Rate = calData.DC_CAL[i].Rate;

                    CailAmpDatas[id - ChannelId.AWG1][i].AVT = new();
                    CailAmpDatas[id - ChannelId.AWG1][i].AVT = calData.AC_CAL[i];
                    CailAmpDatas[id - ChannelId.AWG1][i].AVT!.AcCal = calData.AC_CAL[i].AcCal;
                    Array.Copy(calData.AC_CAL[i].AcCal, CailAmpDatas[id - ChannelId.AWG1][i].AVT!.AcCal, AWG_AC_CAL_SIZE);

                    //CailAmpDatas[id - ChannelId.AWG1][i].AVT.Amp = calData.AC_CAL[i].Amp;

                    //#region 1K Data
                    //if (CailAmpDatas[id - ChannelId.AWG1][i].AVT[0].AcCal.Count == 0)
                    //{
                    //    CailAmpDatas[id - ChannelId.AWG1][i].AVT[0].Init();
                    //}
                    ////                                  avt index - scale
                    //CailAmpDatas[id - ChannelId.AWG1][i].AVT[0].AcCal[0] = CailAmpDatas[id - ChannelId.AWG1][i].AC1K;
                    //#endregion 1K Data

                }

            }
            public class ArbWfmCailAmpData
            {
                ///// <summary>
                ///// 最大幅度
                ///// </summary>
                //public Int32 HizVpp;
                /// <summary>
                /// 最大幅度 
                /// </summary>
                public double MaxVpp;
                /// <summary>
                /// 衰减档
                /// </summary>
                public ushort AttLV;
                /// <summary>
                /// 开启参考通道
                /// </summary>
                public bool RefAttEnable;
                /// <summary>
                /// 衰减档2
                /// </summary>
                public int RefAttLV;
                /// <summary>
                /// 最大频率
                /// </summary>
                public int MaxFreq;
                /// <summary>
                /// 最大直流
                /// </summary>
                public double MaxDC;

                public AdjustValue? DC;
                public AdjustValue? AC1K;
                //public ACAdjustValue[,] AVT = new ACAdjustValue[0, 0]; //Constants.AWG_CAL_ATT_LEVEL_NUM, Constants.AWG_CAL_ARRAY_SCALE_MAX
                public ACAdjustValue? AVT;

            }

        }
    }
}
