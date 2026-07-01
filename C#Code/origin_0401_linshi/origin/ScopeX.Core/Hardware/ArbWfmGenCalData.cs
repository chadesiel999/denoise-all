using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Hardware
{
	[Serializable]
	public struct AdjustValue
	{
		public Double FreqOrDcVol;
		public Double Rate;
	}

	[Serializable]
	public class ACAdjustValue
	{
		public Double Amp;
		public List<AdjustValue> AcCal = new List<AdjustValue>();// AdjustValue[Constants.AWG_AC_CAL_SIZE];
		public void Init()
		{
			AcCal = new List<AdjustValue>();
			for (Int32 i = 0; i < Constants.AWG_AC_CAL_SIZE; i++)
			{
				AcCal.Add(new AdjustValue());
			}
		}
	}

	[Serializable]
	public class CalDataBuffer
	{
		public UInt64 DataMark;
		public UInt64 ECC;
		//public AdjustValue[] DC_CAL = new AdjustValue[Constants.AWG_CAL_ATT_LEVEL_NUM];
		//public AdjustValue[] AC1K_CAL = new AdjustValue[Constants.AWG_CAL_ATT_LEVEL_NUM];
		//public ACAdjustValue[,] AC_CAL = new ACAdjustValue[Constants.AWG_CAL_ATT_LEVEL_NUM, Constants.AWG_CAL_ARRAY_SCALE_MAX];
		public List<AdjustValue> DC_CAL = new();
		public List<AdjustValue> AC1K_CAL = new();
		public List<List<ACAdjustValue>> AC_CAL = new();//Constants.AWG_CAL_ATT_LEVEL_NUM, Constants.AWG_CAL_ARRAY_SCALE_MAX
		public void Init()
		{
			DC_CAL = new();
			AC1K_CAL = new();
			AC_CAL = new();
			for (Int32 att = 0; att < Constants.AWG_CAL_ATT_LEVEL_NUM; att++)
			{
				DC_CAL.Add(new());
				AC1K_CAL.Add(new());
				var scaleList = new List<ACAdjustValue>();
				for (Int32 scale = 0; scale < Constants.AWG_CAL_ARRAY_SCALE_MAX; scale++)
				{
					var tmp = new ACAdjustValue();
					tmp.Init();
					scaleList.Add(tmp);
				}
				AC_CAL.Add(scaleList);
			}

		}
	}
}
