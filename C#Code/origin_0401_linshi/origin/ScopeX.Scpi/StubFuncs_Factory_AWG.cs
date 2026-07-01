using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Hardware;
using CalibrationData = ScopeX.Hardware.Calibration.Data.Base;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading.Channels;

namespace ScopeX.Scpi
{
	partial class StubFunc
	{
		#region 私有枚举
		private enum FPGA_ADDR_REG
		{
			FWORK_MODE = 0x001,
			PHASE_INC = 0x002,
			CNT_ATTEN = 0x02f,
			VOL_ADJ = 0x033,
			CH_OnOff = 0x032,
			DC_Offset = 0x063,
			SN_STRING = 0x800,
			SYS_VER,
			SYS_INFO = 0x900,
			SYS_HTTP,
			SYS_IDN,
			SYS_CVER,
			SYS_RESET_TO_FACTORY,
		};
		private enum ReadyCode
		{
			SYNC_Cal_DC = 0,
			SYNC_Cal_AC,
			SYNC_Cal_WriteFile,//SYNC_Cal_Clear
			SYNC_Cal_save,//SYNC_Cal_save

			SYNC_Verify_DC,
			SYNC_Verify_AC,
			SYNC_Cal_ClearAll
		};

		#endregion 私有枚举

		#region 私有常量
		private const UInt32 _AWG_CAL_ATT_LEVEL_NUM = 4;
		#region Calib
		public const double AWG_CAL_FREQ_1K = 1000.0; //1 k
		public const double AWG_CAL_FREQ_MIN_RANGE = 150e3; //150 k
															//public const Int32 AWG_CAL_PIONT_ARRAY_MAX = 70;
		public const Int32 AWG_CAL_ARRAY_SCALE_MAX = 1; // todo  =>2
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
		public const Int32 AWG_AC_CAL_SIZE = (Int32)(AWG_CAL_MAX_FREQ / AWG_AC_CAL_STEP);
		public const Int32 AMP_CAL_MAX = ((1 << 16) - 1); //todo 23.1.29 =>16
		public const Int32 AMP_DEFAULT = AMP_CAL_MAX;//((ushort)(AMP_CAL_MAX/1.4))  // 20479 = 5v

		#endregion Calib
		#endregion 私有常量

		private static byte[] CByteCovert2CSharpByte(byte[] data)
		{
			byte[] tmp = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				//如果是负数转化成无符号数据
				if (int.Parse(data[i].ToString()) < 0)
					tmp[i] = (byte)(0xff & int.Parse(data[i].ToString()));
				else
					tmp[i] = byte.Parse(data[i].ToString());
			}
			return tmp;
		}
		#region 厂家AWG 校准
		/// <summary>
		/// 查询校准IDN，适配旧软件
		/// </summary>
		/// <param name="analyResult"></param>
		/// <param name="sendMessage"></param>
		/// <returns></returns>
		public static bool scpiQuy_FactoryAWGCailIDN(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
		{
			sendMessage.SendData = Encoding.UTF8.GetBytes(ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Get($"CalibAWG,CailIDN"));
			return true;
		}
		/// <summary>
		/// 查询寄存器 RP<n>:ADDR<a>
		/// </summary>
		/// <param name="analyResult"></param>
		/// <param name="sendMessage"></param>
		/// <returns></returns>
		public static bool scpiQuy_FactoryAWGCailRP(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
		{
			if (analyResult.ChannelIndexs.Count != 2)
			{
				return false;
			}
			int addr = analyResult.ChannelIndexs[1];
			int channel = analyResult.ChannelIndexs[0];

			if (!Enum.IsDefined(typeof(FPGA_ADDR_REG), addr))
			{
				return false;
			}
			sendMessage = new();


			switch ((FPGA_ADDR_REG)addr)
			{
				case FPGA_ADDR_REG.SN_STRING://获取SN

					//sendMessage.SendData = prsnt.HdGetResults("CailSN");
					sendMessage.SendData = Encoding.UTF8.GetBytes(ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Get($"CalibAWG{channel},CailSN"));
					return true;
				case FPGA_ADDR_REG.SYS_VER://获取协议版本
					break;
				case FPGA_ADDR_REG.SYS_HTTP://获取网址
					break;

				case FPGA_ADDR_REG.SYS_INFO://获取信息

					//sendMessage.SendData = prsnt.HdGetResults("CailSYSINFO");
					sendMessage.SendData = Encoding.UTF8.GetBytes(ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Get($"CalibAWG{channel},CailSYSINFO"));
					return true;

				default: return false;
			}
			return false;
		}
		/// <summary>
		/// 设置寄存器 WP<n>:ADDR<a>
		/// </summary>
		/// <param name="analyResult"></param>
		/// <returns></returns>
		public static bool scpiSet_FactoryAWGCailWP(SCPICommandProcessFuncParam analyResult)
		{
			if (analyResult.ChannelIndexs.Count != 2 || analyResult.Params.Count == 0)
			{
				return false;
			}
			int addr = analyResult.ChannelIndexs[1];
			int channel = analyResult.ChannelIndexs[0];
			var dataStr = Encoding.UTF8.GetString(analyResult.Params[0]).Trim();
			var dataValue = (int)analyResult.Params[0][0];
			if (!Enum.IsDefined(typeof(FPGA_ADDR_REG), addr))
			{
				return false;
			}
			var channelVaild = !(channel < 1 || channel > 2);
			if (!channelVaild)
			{
				return false;
			}
			//if (!TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
			//{
			//	return false;
			//}
			if (string.IsNullOrWhiteSpace(dataStr))
			{
				return false;
			}

			switch ((FPGA_ADDR_REG)addr)
			{
				case FPGA_ADDR_REG.SYS_INFO://设置系统信息 版本，带宽，版本，系统型号
											//todo
					break;
				case FPGA_ADDR_REG.SYS_HTTP://设置网址
											//todo
					break;
				case FPGA_ADDR_REG.SN_STRING://设置SN号
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},CAIL_SN_SET,{dataStr}");
					break;
				case FPGA_ADDR_REG.FWORK_MODE://设置工作模式
											  //if (dataStr == "0")
					if (dataValue == 0)
					{
						//prsnt.WfmGenCalSetOffset(0);
						//prsnt.WfmType = ArbWfmType.Sinusoid;
						//prsnt.SendMsgToHD($"OFFSET,0");
						//prsnt.SendMsgToHD($"TYPE,{(ushort)ArbWfmType.Sinusoid}");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},OFFSET,0");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},TYPE,{(ushort)ArbWfmType.Sinusoid}");
					}
					else // dataStr == "1"
					{
						//prsnt.WfmType = ArbWfmType.DC;
						//prsnt.SendMsgToHD($"TYPE,{(ushort)ArbWfmType.DC}");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},TYPE,{(ushort)ArbWfmType.DC}");
					}
					break;
				case FPGA_ADDR_REG.CNT_ATTEN://设置档位信息
					Debug.WriteLine($"===Debug:Get CMD [CNT_ATTEN]  val:{dataValue}");
					//if (ushort.TryParse(dataStr, out ushort att))
					{
						//prsnt.Active = false;
						//prsnt.WfmGenCalSetOffset(0);
						//prsnt.CalATT = (ushort)dataValue;
						//prsnt.CalAMP = 0;//new 修复可能校准烧毁探头问题
						//prsnt.Active = true;
						//prsnt.SendMsgToHD($"ACT,0");
						//prsnt.SendMsgToHD($"OFFSET,0");
						//prsnt.SendMsgToHD($"ATT,{(ushort)dataValue}");
						//prsnt.SendMsgToHD($"AMP,0");//修复可能校准烧毁探头问题
						//prsnt.SendMsgToHD($"ACT,1");

						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},ACT,0");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},OFFSET,0");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},ATT,{(ushort)dataValue}");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},AMP,0");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},ACT,1");
					}

					break;
				case FPGA_ADDR_REG.CH_OnOff://通道开关
					Debug.WriteLine($"===Debug:Get CMD [CH_OnOff]  val:{dataValue}");
					//prsnt.Active = dataValue == 1;
					//prsnt.SendMsgToHD($"ACT,{dataValue}");
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},ACT,{dataValue}");
					break;
				case FPGA_ADDR_REG.PHASE_INC://设置频率

					double freq = BitConverter.ToDouble(CByteCovert2CSharpByte(analyResult.Params[0]), 0);
					Debug.WriteLine($"===Debug:Get CMD [FREQ_PHASE_INC]  val:{freq}");
					//if (double.TryParse(dataStr, out double freq))
					{
						//prsnt.Frequency = (Int64)(freq * 1000_000);
						//prsnt.SendMsgToHD($"FREQ,{(Int64)(freq * 1000_000)}");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},FREQ,{(Int64)(freq * 1000_000)}");
					}
					break;
				case FPGA_ADDR_REG.DC_Offset:   //set offset

					double offset = BitConverter.ToDouble(CByteCovert2CSharpByte(analyResult.Params[0]), 0);
					Debug.WriteLine($"===Debug:Get CMD [DC_Offset]  val:{offset}");
					//if (double.TryParse(dataStr, out double offset))
					{
						//prsnt.CalATT = 1;
						//prsnt.WfmGenCalSetOffset(offset);
						//prsnt.SendMsgToHD($"OFFSET,{offset}");
						ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},OFFSET,{offset}");
					}
					break;
				case FPGA_ADDR_REG.VOL_ADJ:   //设置幅度

					double amp = BitConverter.ToDouble(CByteCovert2CSharpByte(analyResult.Params[0]), 0);
					Debug.WriteLine($"===Debug:Get CMD [VOL_ADJ]  val:{amp}");

					//prsnt.CalATT = prsnt.CalATT; //重设一遍ATT
					//prsnt.CalAMP = amp;
					//prsnt.SendMsgToHD($"AMP,{amp}");
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},AMP,{amp}");
					break;
				default:
					return false;
			}

			return true;
		}
		/// <summary>
		/// 设置同步进度 SYNC<n>:CMD<a>
		/// </summary>
		/// <param name="analyResult"></param>
		/// <returns></returns>
		public static bool scpiSet_FactoryAWGCailSYNC(SCPICommandProcessFuncParam analyResult)
		{
			if (analyResult.ChannelIndexs.Count != 2)
			{
				return false;
			}
			int cmd = analyResult.ChannelIndexs[1];
			int channel = analyResult.ChannelIndexs[0];
			if (!Enum.IsDefined(typeof(ReadyCode), cmd))
			{
				return false;
			}
			if (channel != 0xff && (channel < 1 || channel > 2))
			{
				return false;
			}
			ArbWfmGenPrsnt prsnt;

			switch ((ReadyCode)cmd)
			{
				case ReadyCode.SYNC_Cal_DC:

					if (TryGetAWGChannelPrsnt(analyResult, out prsnt))
					{
						//prsnt.CalStatus = WfmGenCalStatus.Cal_DC;
						//prsnt.SendMsgToHD($"STATUS,1");
						prsnt.Impedance = WfmGenImpedance.Low50;
					}
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},STATUS,1");
					break;

				case ReadyCode.SYNC_Cal_AC:

					if (TryGetAWGChannelPrsnt(analyResult, out prsnt))
					{
						//prsnt.CalStatus = WfmGenCalStatus.Cal_AC;
						//prsnt.SendMsgToHD($"STATUS,2");
						prsnt.Impedance = WfmGenImpedance.Low50;
					}
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},STATUS,2");
					break;

				case ReadyCode.SYNC_Cal_WriteFile:
					//			LoadCalData();
					break;

				case ReadyCode.SYNC_Cal_save:
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},CAIL_SAVE_DATA,1");
					break;

				case ReadyCode.SYNC_Verify_DC:
					if (TryGetAWGChannelPrsnt(analyResult, out prsnt))
					{
						//prsnt.SendMsgToHD($"STATUS,3");
						prsnt.Impedance = WfmGenImpedance.Low50;
					}
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},STATUS,3");
					break;

				case ReadyCode.SYNC_Verify_AC:
					if (TryGetAWGChannelPrsnt(analyResult, out prsnt))
					{
						//prsnt.SendMsgToHD($"STATUS,4");
						prsnt.Impedance = WfmGenImpedance.Low50;
					}
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},STATUS,4");
					break;

				case ReadyCode.SYNC_Cal_ClearAll:
					if (TryGetAWGChannelPrsnt(analyResult, out prsnt))
					{
						//prsnt.CalStatus = WfmGenCalStatus.OFF;
						//prsnt.SendMsgToHD($"STATUS,0");
						//prsnt.SendMsgToHD($"ClearCalTempData,1");

						//prsnt.WfmGenCalClearAll();
					}
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},STATUS,0");
					ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel},CLEAR_CAL_TEMP_DATA,1");
					break;
				default:
					break;
			}
			return true;
		}
		/// <summary>
		/// 设置校准数据 WFILE
		/// </summary>
		/// <param name="analyResult"></param>
		/// <returns></returns>
		public static bool scpiSet_FactoryAWGCailFile(SCPICommandProcessFuncParam analyResult)
		{
			////todo
			if (analyResult.Params != null && analyResult.Params.Count > 0)
			{
				var datas = Encoding.UTF8.GetString(analyResult.Params[0]);
				Debug.WriteLine($"WFILE:{datas}");

				#region 初始化
				if (string.IsNullOrWhiteSpace(datas))
				{
					return false;
				}
				var lines = datas.Split('\n');
				if (lines == null || lines.Count() < 2)
				{
					return false;
				}
				string? type = null;
				//ArbWfmGenPrsnt prsnt = null;
				int attLevel = 0;
				double voltage = 0;
				//int freqPointACAny = 0;
				Regex regexNum = new Regex(@"(-?\d+[.][0-9]*|\d+)");
				Regex regexFreqAmp = new Regex(@"\[\d+\:(-?\d+[.][0-9]*|\d+)\]"); //[200000:0.484463]  200k
				var channel = ChannelId.AWG1;
				#endregion 初始化

				foreach (var dataLine in lines)
				{
					if (String.IsNullOrWhiteSpace(dataLine))
					{
						continue;
					}
					if (dataLine.IndexOf("CH:") > 0)
					{
						if (dataLine.Contains("END"))
						{
							ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel - ChannelId.AWG1 + 1},CAIL_SET_AMP_DATA,0,0,0");
							////保存校准数据
							//prsnt.WfmGenSaveCalData();
							//prsnt = null;
						}
						else if (regexNum.IsMatch(dataLine))
						{
							var match = regexNum.Match(dataLine);
							var value = int.Parse(match.Value);
							channel = ChannelId.AWG1 + value;
							//TryGetAWGChannelPrsnt(channel, out prsnt);
						}
					}
					else if (dataLine.IndexOf("T:") > 0)
					{
						if (dataLine.Contains("END"))
						{
							type = null;
						}
						else if (dataLine.Contains("DC"))
						{
							type = "DC";
						}
						else if (dataLine.Contains("AC"))
						{
							type = "AC";
						}
					}
					else if (dataLine.IndexOf("R:") > 0 && dataLine.IndexOf("A:") > 0 && dataLine.IndexOf("B:") > 0 && type == "DC")
					{//DC
						if (type == null)
						{
							return false;
						}
						var tmpItems = dataLine.Split(']');
						foreach (var item in tmpItems)
						{
							if (item.IndexOf("R:") > 0 && regexNum.IsMatch(item))
							{
								var match = regexNum.Match(item);
								attLevel = int.Parse(match.Value);
								if (attLevel < 0 || attLevel > _AWG_CAL_ATT_LEVEL_NUM)
								{
									return false;
								}

							}
							else if (item.IndexOf("A:") > 0 && regexNum.IsMatch(item))
							{
								var match = regexNum.Match(item);
								//AdjustValue buffer = prsnt.CalTablBuff.DC_CAL[attLevel];
								//buffer.FreqOrDcVol = double.Parse(match.Value);
								//prsnt.CalTablBuff.DC_CAL[attLevel] = buffer;
								//prsnt.SendMsgToHD($"DC_CAL,{attLevel},0,{double.Parse(match.Value)}"); //DC_CAL,<att>,<paramId>,<value>
								Debug.WriteLine($"       DC Cal Data: LV: {attLevel} ,VOL:{double.Parse(match.Value)}");
								ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel - ChannelId.AWG1 + 1},DC_CAL,{attLevel},0,{double.Parse(match.Value)}");
							}
							else if (item.IndexOf("B:") > 0 && regexNum.IsMatch(item))
							{
								var match = regexNum.Match(item);
								//AdjustValue buffer = prsnt.CalTablBuff.DC_CAL[attLevel];
								//buffer.Rate = double.Parse(match.Value);
								//prsnt.CalTablBuff.DC_CAL[attLevel] = buffer;
								//prsnt.SendMsgToHD($"DC_CAL,{attLevel},1,{double.Parse(match.Value)}"); //DC_CAL,<att>,<paramId>,<value>
								Debug.WriteLine($"       DC Cal Data: LV: {attLevel} ,Rate:{double.Parse(match.Value)}");
								ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel - ChannelId.AWG1 + 1},DC_CAL,{attLevel},1,{double.Parse(match.Value)}");
								break;
							}
						}
					}
					else if (dataLine.IndexOf("V:") > 0 && type == "AC" && regexNum.IsMatch(dataLine))
					{//AC V
						var match = regexNum.Match(dataLine);
						voltage = double.Parse(match.Value);
					}
					else if (dataLine.IndexOf("R:") > 0 && type == "AC" && regexNum.IsMatch(dataLine))
					{//AC R
						var match = regexNum.Match(dataLine);
						attLevel = int.Parse(match.Value);
						//freqPointACAny = 0;
					}
					else if (regexFreqAmp.IsMatch(dataLine) && type == "AC")
					{// AC Freq Amp

						// freq by Hz
						var match = regexNum.Match(dataLine.Split(":")[0]);
						var freqReal = double.Parse(match.Value);
						match = regexNum.Match(dataLine.Split(":")[1]);
						var rate = double.Parse(match.Value);

						if (freqReal <= AWG_CAL_FREQ_1K)
						{ //1K
							if (attLevel >= AWG_CAL_ATT_LEVEL_NUM)
							{
								Debug.WriteLine($"      ERROR: AC Cal Data: attLevel = {attLevel}");
								return false;
							}
							//AdjustValue calDataBuf = new();
							//calDataBuf.FreqOrDcVol = freqReal;
							//calDataBuf.Rate = rate;
							//prsnt.CalTablBuff.AC1K_CAL[attLevel] = calDataBuf;
							//prsnt.SendMsgToHD($"AC1K_CAL,{attLevel},{freqReal},{rate}");
							ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel - ChannelId.AWG1 + 1},AC1K_CAL,{attLevel},{freqReal},{rate}");
							Debug.WriteLine($"       AC 1K Cal Data: LV: {attLevel},RealFreq:{freqReal} Hz ,Rate:{rate}");
						}
						else
						{ //AC Any
						  //todo maxScale =>2
							if (attLevel >= AWG_CAL_ATT_LEVEL_NUM)
							{
								Debug.WriteLine($"      ERROR: AC Cal Data: attLevel = {attLevel}");
								return false;
							}
							//prsnt.SendMsgToHD($"AC_CAL,{attLevel},{freqReal},{rate},{voltage}");
							ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"CalibAWG{channel - ChannelId.AWG1 + 1},AC_CAL,{attLevel},{freqReal},{rate},{voltage}");
						}
					}
				}
			}
			else
			{
				Debug.WriteLine($"WFILE: (NULL)");
			}

			return true;
		}
		#endregion  厂家AWG 校准
	}
}
