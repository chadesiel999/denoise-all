using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Hardware
{
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

		public static List<ArbWfmCailAmpData>[] CailAmpDatas = new List<ArbWfmCailAmpData>[Constants.AWG_MAX_NUM]
		{
			new List<ArbWfmCailAmpData>
			{
				new ArbWfmCailAmpData() { MaxVpp = 5     ,AttLV = 0, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 5 },
				new ArbWfmCailAmpData() { MaxVpp = 0.2   ,AttLV = 1, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 1 },
				new ArbWfmCailAmpData() { MaxVpp = 1     ,AttLV = 2, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 5 },
				new ArbWfmCailAmpData() { MaxVpp = 0.04  ,AttLV = 3, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 1 },
			},
			new List<ArbWfmCailAmpData>
			{
				new ArbWfmCailAmpData() { MaxVpp = 5     ,AttLV = 0, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 5 },
				new ArbWfmCailAmpData() { MaxVpp = 0.2   ,AttLV = 1, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 1 },
				new ArbWfmCailAmpData() { MaxVpp = 1     ,AttLV = 2, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 5 },
				new ArbWfmCailAmpData() { MaxVpp = 0.04  ,AttLV = 3, RefAttEnable = false, MaxFreq = (Int32)(Constants.AWG_SIN_FRQ_MAX / 1000_000) , MaxDC = 1 },
			 }
			};

		public static void SetCHAmpData(ChannelId id, CalDataBuffer calData)
		{
			if (id < ChannelId.AWG1 || id > ChannelId.AWG2)
			{
				return;
			}
			for (Int32 i = 0; i < Constants.AWG_CAL_ATT_LEVEL_NUM; i++)
			{
				CailAmpDatas[id - ChannelId.AWG1][i].AC1K = calData.AC1K_CAL[i];
				CailAmpDatas[id - ChannelId.AWG1][i].DC = calData.DC_CAL[i];
				CailAmpDatas[id - ChannelId.AWG1][i].AVT = calData.AC_CAL[i];

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
			public Double MaxVpp;
			/// <summary>
			/// 衰减档
			/// </summary>
			public UInt16 AttLV;
			/// <summary>
			/// 开启参考通道
			/// </summary>
			public Boolean RefAttEnable;
			/// <summary>
			/// 衰减档2
			/// </summary>
			public Int32 RefAttLV;
			/// <summary>
			/// 最大频率
			/// </summary>
			public Int32 MaxFreq;
			/// <summary>
			/// 最大直流
			/// </summary>
			public Int32 MaxDC;

			public AdjustValue DC;
			public AdjustValue AC1K;
			//public ACAdjustValue[,] AVT = new ACAdjustValue[0, 0]; //Constants.AWG_CAL_ATT_LEVEL_NUM, Constants.AWG_CAL_ARRAY_SCALE_MAX
			public List<ACAdjustValue> AVT = new();
			public ArbWfmCailAmpData()
			{
				//for (Int32 att = 0; att < Constants.AWG_CAL_ATT_LEVEL_NUM; att++)
				//{
				//    var scaleList = new List<ACAdjustValue>();
				//    for (Int32 scale = 0; scale < Constants.AWG_CAL_ARRAY_SCALE_MAX; scale++)
				//    {
				//        var tmp = new ACAdjustValue();
				//        tmp.Init();
				//        scaleList.Add(tmp);
				//    }
				//    AVT.Add(scaleList);
				//}
				for (Int32 scale = 0; scale < Constants.AWG_CAL_ARRAY_SCALE_MAX; scale++)
				{
					var tmp = new ACAdjustValue();
					tmp.Init();
					AVT.Add(tmp);
				}
			}
			//public AdjustValue DC = new() { A = 1, B = 0 };
			//public AdjustValue AC1K = new() { A = 1, B = 0 };
			//public AdjustValue AVT = new() { A = 1, B = 0 };
		}

	}
}
