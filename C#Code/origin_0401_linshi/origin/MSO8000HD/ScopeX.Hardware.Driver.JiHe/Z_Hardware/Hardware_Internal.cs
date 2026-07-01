using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
	public static partial class Hd
	{
		internal static DebugVariants CurrDebugVarints = new DebugVariants();
		internal static bool bPrintDebugInformation = false;
		internal static HdMessage? UIMessage;
		internal static bool bAttachHardware = false;
		internal static bool bAcqedNewData = false;
		internal static bool bAdjustGainByTemperature = false;
		internal static bool bAdcAtTestMode = true;
		internal static bool bAdcAtFlashMode = true;

		private static List<KeyValuePair<Stopwatch, int>> waitMillisecondList = new List<KeyValuePair<Stopwatch, int>>();
		internal static void PushWaitMilliseconds(int milliseconds)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			waitMillisecondList.Add(new KeyValuePair<Stopwatch, int>(stopwatch, milliseconds));
		}
		private static IDriver? DriverFactory(DriverTypes driverType, string id)
		{
			switch (driverType)
			{
				case DriverTypes.DCCardPcie:
					if (!bAttachHardware)
						return new Driver_Simulator();
					else
					{
#if JiHe_MSO7000X || JiHe_MSO8000X
						return new Driver_JiHePcie();
#else
#if RpcPcie
                        return new Driver_RpcPcie();
#else
                        return new Driver_DCCardPcie();
#endif
#endif
					}
				case DriverTypes.CyUsb3_0:
					return new Driver_CyUsb3_0(id);
				default:
					return new Driver_Simulator();
			}
		}
		private static bool bNeedPowerInit = true;
		static bool DoAllBoardInit()
		{
			if (bNeedPowerInit)
			{
				Hd.CurrProduct?.PcieBd?.Init();
				Hd.CurrProduct?.S6Bd?.Init();
				Hd.CurrProduct?.ProcBd?.Init();
				Hd.CurrProduct?.AcqBd?.Init();
			}
			else
				Hd.CurrProduct?.AcqBd?.IsAllPowerOk();//需要获取各个采集板上电的情况，否则采集板寄存器不能发送
			return true;
		}
		static void DoAllTest()
		{
			Hd.CurrProduct?.PcieBd?.Test();
			Hd.CurrProduct?.S6Bd?.Test();
			Hd.CurrProduct?.ProcBd?.Test();
			Hd.CurrProduct?.AcqBd?.Test();
		}
		internal static string ReadSystemTemperatures()
		{
			return "";
		}

		internal static bool CreateProduct()
		{
			CurrProduct = ProductFactory.CreateProduct(CurrProductType);
			return CurrProduct != null;
		}
		static partial void Do_U2KeyDown(string param);
		private static String ResponseU2KeyDown(String param)
		{
			Do_U2KeyDown(param);
			return "";
		}
		static partial void Do_UserAutoCali(String param);
		static string AutoCaliHelper_DoCali(string param)
		{
			Do_UserAutoCali(param);
			return "";
		}
		public static string ResopnseAWGVersion()
		{
			if (Hd.CurrProduct?.ProcBd != null)
			{
				if (AWG.AWG_HD_Data_Send_Locked)
				{
					return "NULL";
				}
				AWG.AWG_HD_InQuery_Protect = true;
				if (Hd.CurrProduct.ProcBd.ReadAWGVersion(out string result))
				{
					return result;
				}
			}
			AWG.AWG_HD_InQuery_Protect = false;
			return "NULL";
		}
		static string ResopnseAWGProtectState()
		{
			if (Hd.CurrProduct?.ProcBd != null)
			{
				if (AWG.AWG_HD_Data_Send_Locked)
				{
					return "NULL";
				}
				AWG.AWG_HD_InQuery_Protect = true;
				if (Hd.CurrProduct.ProcBd.ReadAWGProtectState(out byte state))
				{
					string result = "";
					//if ((state & 0x03) == 0)
					if (state == 0x03)
					{
						result += $"1,";
					}
					else
					{
						result += $"0,";
					}
					if ((state >> 4) == 0x03)
					//if (((state >> 4) & 0x03) == 0)
					{
						result += $"1";
					}
					else
					{
						result += $"0";
					}
					AWG.AWG_HD_InQuery_Protect = false;
					return result;
				}
			}
			AWG.AWG_HD_InQuery_Protect = false;
			return "NULL";
		}
		public static String MiscFunc(String funcName, String param)
		{
			return funcName switch
			{
				"UserAutoCali" => AutoCaliHelper_DoCali(param),
				"Ext10MHzLocked" => Hd.Ext10MHzLocked ? "true" : "false",
				"U2KeyDown" => ResponseU2KeyDown(param),
				"AWGProtectState" => ResopnseAWGProtectState(),

				_ => "",
			};
		}
		internal static string GetExtTrigCompareResult()
		{
			return (HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Ext_CmpResult) & 0x01).ToString();
		}
		#region MiscDataFromFPGA
		private static void Read_EdgeCounter(String masterKey, ref Dictionary<String, UInt64> exists)
		{
            #if JiHe_MSO7000X
			(AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[] readbackRegisters = new (AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[]
			{
			   (AcqBdReg.R.EdgeCounter_Ch1ValueLow16,AcqBdReg.R.EdgeCounter_Ch1ValueMid16,AcqBdReg.R.EdgeCounter_Ch1ValueHigh16),
			   (AcqBdReg.R.EdgeCounter_Ch2ValueLow16,AcqBdReg.R.EdgeCounter_Ch2ValueMid16,AcqBdReg.R.EdgeCounter_Ch2ValueHigh16),
			   (AcqBdReg.R.EdgeCounter_Ch3ValueLow16,AcqBdReg.R.EdgeCounter_Ch3ValueMid16,AcqBdReg.R.EdgeCounter_Ch3ValueHigh16),
			   (AcqBdReg.R.EdgeCounter_Ch4ValueLow16,AcqBdReg.R.EdgeCounter_Ch4ValueMid16,AcqBdReg.R.EdgeCounter_Ch4ValueHigh16),
			};
			Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.EdgeCounter_ReadLock, 0);
			Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.EdgeCounter_ReadLock, 1);
			for (ChannelId channelId = 0; channelId < (ChannelId)ChannelIdExt.AnaChnlNum; channelId++)
			{
				if ((int)channelId < readbackRegisters.Length)
				{
					UInt64 data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].high, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].mid, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].low, AcqBdNo.B5);
					exists.TryAdd($"{masterKey}_{channelId}", data);
				}
			}
#endif
		}
		private static void Read_DVM(String masterKey, ref Dictionary<String, UInt64> exists)
		{
            #if JiHe_MSO7000X
			(AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[] readbackRegisters_TimeCounter = new (AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[]
			{
				(AcqBdReg.R.DVM_Ch1TimeCounterLow16,AcqBdReg.R.DVM_Ch1TimeCounterMid16,AcqBdReg.R.DVM_Ch1TimeCounterHigh16),
				(AcqBdReg.R.DVM_Ch2TimeCounterLow16,AcqBdReg.R.DVM_Ch2TimeCounterMid16,AcqBdReg.R.DVM_Ch2TimeCounterHigh16),
				(AcqBdReg.R.DVM_Ch3TimeCounterLow16,AcqBdReg.R.DVM_Ch3TimeCounterMid16,AcqBdReg.R.DVM_Ch3TimeCounterHigh16),
				(AcqBdReg.R.DVM_Ch4TimeCounterLow16,AcqBdReg.R.DVM_Ch4TimeCounterMid16,AcqBdReg.R.DVM_Ch4TimeCounterHigh16),
			};
			(AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[] readbackRegisters_QuadraticSum = new (AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[]
			{
				(AcqBdReg.R.DVM_Ch1QuadraticSumLow16,AcqBdReg.R.DVM_Ch1QuadraticSumMid16,AcqBdReg.R.DVM_Ch1QuadraticSumHigh16),
				(AcqBdReg.R.DVM_Ch2QuadraticSumLow16,AcqBdReg.R.DVM_Ch2QuadraticSumMid16,AcqBdReg.R.DVM_Ch2QuadraticSumHigh16),
				(AcqBdReg.R.DVM_Ch3QuadraticSumLow16,AcqBdReg.R.DVM_Ch3QuadraticSumMid16,AcqBdReg.R.DVM_Ch3QuadraticSumHigh16),
				(AcqBdReg.R.DVM_Ch4QuadraticSumLow16,AcqBdReg.R.DVM_Ch4QuadraticSumMid16,AcqBdReg.R.DVM_Ch4QuadraticSumHigh16),
			};
			(AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[] readbackRegisters_Sum = new (AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[]
			{
				(AcqBdReg.R.DVM_Ch1SumLow16,AcqBdReg.R.DVM_Ch1SumMid16,AcqBdReg.R.DVM_Ch1SumHigh16),
				(AcqBdReg.R.DVM_Ch2SumLow16,AcqBdReg.R.DVM_Ch2SumMid16,AcqBdReg.R.DVM_Ch2SumHigh16),
				(AcqBdReg.R.DVM_Ch3SumLow16,AcqBdReg.R.DVM_Ch3SumMid16,AcqBdReg.R.DVM_Ch3SumHigh16),
				(AcqBdReg.R.DVM_Ch4SumLow16,AcqBdReg.R.DVM_Ch4SumMid16,AcqBdReg.R.DVM_Ch4SumHigh16),
			};
			for (ChannelId channelId = 0; channelId < (ChannelId)ChannelIdExt.AnaChnlNum; channelId++)
			{
				if ((int)channelId < readbackRegisters_TimeCounter.Length)
				{
					UInt64 data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_TimeCounter[(int)channelId].high, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_TimeCounter[(int)channelId].mid, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_TimeCounter[(int)channelId].low, AcqBdNo.B5);
					exists.TryAdd($"{masterKey}_TimeCounter_{channelId}", data);
				}
				if ((int)channelId < readbackRegisters_QuadraticSum.Length)
				{
					UInt64 data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_QuadraticSum[(int)channelId].high, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_QuadraticSum[(int)channelId].mid, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_QuadraticSum[(int)channelId].low, AcqBdNo.B5);
					exists.TryAdd($"{masterKey}_QuadraticSum_{channelId}", data);
				}
				if ((int)channelId < readbackRegisters_Sum.Length)
				{
					UInt64 data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_Sum[(int)channelId].high, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_Sum[(int)channelId].mid, AcqBdNo.B5);
					data <<= 16;
					data |= Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters_Sum[(int)channelId].low, AcqBdNo.B5);
					exists.TryAdd($"{masterKey}_Sum_{channelId}", data);
				}
			}
#endif
		}
		private static void Read_AcqWaveStatistics(String masterKey, ref Dictionary<String, UInt64> exists)
		{
            #if JiHe_MSO7000X
			(AcqBdReg.R Max, AcqBdReg.R Min, AcqBdReg.R DC)[] readbackRegisters = new (AcqBdReg.R low, AcqBdReg.R mid, AcqBdReg.R high)[]
			{
				(AcqBdReg.R.AcqWaveStatistics_Ch1Max,AcqBdReg.R.AcqWaveStatistics_Ch1Min,AcqBdReg.R.AcqWaveStatistics_Ch1DC),
				(AcqBdReg.R.AcqWaveStatistics_Ch2Max,AcqBdReg.R.AcqWaveStatistics_Ch2Min,AcqBdReg.R.AcqWaveStatistics_Ch2DC),
				(AcqBdReg.R.AcqWaveStatistics_Ch3Max,AcqBdReg.R.AcqWaveStatistics_Ch3Min,AcqBdReg.R.AcqWaveStatistics_Ch3DC),
				(AcqBdReg.R.AcqWaveStatistics_Ch4Max,AcqBdReg.R.AcqWaveStatistics_Ch4Min,AcqBdReg.R.AcqWaveStatistics_Ch4DC),
			};
			for (ChannelId channelId = 0; channelId < (ChannelId)ChannelIdExt.AnaChnlNum; channelId++)
			{
				if ((int)channelId < readbackRegisters.Length)
				{
					UInt64 data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].Max, AcqBdNo.B5) & 0xffff;
					exists.TryAdd($"{masterKey}_Max_{channelId}", data);
					data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].Min, AcqBdNo.B5) & 0xffff;
					exists.TryAdd($"{masterKey}_Min_{channelId}", data);
					data = Hd.CurrProduct!.AcqBd!.ReadReg(readbackRegisters[(int)channelId].DC, AcqBdNo.B5);
					exists.TryAdd($"{masterKey}_DC_{channelId}", data);
				}
			}
#endif
		}
		#endregion
	}
}
