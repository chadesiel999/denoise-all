using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScopeX.Updater.Base
{
	public class ProductInfo
	{

		/// <summary>
		/// 产品型号
		/// </summary>
		public string ProductModel { get => productModel; set => productModel = value; }
		/// <summary>
		/// SN
		/// </summary>
		public string SerialNumber { get => serialNumber; set => serialNumber = value; }
		/// <summary>
		/// 渠道信息
		/// </summary>
		public string ChannelInfo { get => channelInfo; set => channelInfo = value; }
		/// <summary>
		/// 生产日期
		/// </summary>
		public DateTime ProductionDate { get => productionDate; set => productionDate = value; }
		/// <summary>
		/// 备注
		/// </summary>
		public string OtherInfo { get => otherInfo; set => otherInfo = value; }
		/// <summary>
		/// 协议版本号
		/// </summary>
		public string ProtocolVersion { get => protocolVersion; set => protocolVersion = value; }

		/// <summary>
		/// 硬件版本号
		/// </summary>
		public string HardVersion { get => hardVersion; set => hardVersion = value; }
		/// <summary>
		/// 校验码1
		/// </summary>
		public string CheckCodeMD5 { get => checkCodeMD5; }

		private string checkCodeMD5;
		/// <summary>
		/// 校验码2
		/// </summary>
		public string CheckCodeCRC { get => checkCodeCRC; }

		private string checkCodeCRC;
		private string hardVersion;

		private string protocolVersion;
		private string otherInfo;
		private string channelInfo;

		private DateTime productionDate;

		private string productModel;

		private string serialNumber;

		private string InfosToString()
		{
			return $"{serialNumber}${productModel}${productionDate}${channelInfo}${otherInfo}${protocolVersion}${hardVersion}";
		}
		private byte[] InfosToBytes()
		{
			return Encoding.UTF8.GetBytes(InfosToString());
		}
		private void SumCheckCode()
		{
			if (!string.IsNullOrWhiteSpace(serialNumber))
			{
				checkCodeMD5 = MD5Util.GetMD5_32(serialNumber);
			}

			checkCodeCRC = CRCUtil.CRCCalc(InfosToString());
		}
		public byte[] InfosToBytes(bool isOnlySN)
		{
			List<byte> results = new();
			SumCheckCode();
			results.AddRange(Encoding.UTF8.GetBytes(checkCodeMD5));
			results.Add(Encoding.UTF8.GetBytes("$")[0]);
			results.AddRange(Encoding.UTF8.GetBytes(checkCodeCRC));
			results.Add(Encoding.UTF8.GetBytes("$")[0]);
			if (!isOnlySN)
			{
				results.AddRange(Encoding.UTF8.GetBytes(InfosToString()));
			}
			else
			{
				results.AddRange(Encoding.UTF8.GetBytes(serialNumber));
			}
			var test = Encoding.UTF8.GetString(results.ToArray()).Split('$');
			return results.ToArray();
		}
		public bool BytesToInfos(byte[] bytes)
		{
			if (bytes.Count() == 0)
			{
				return false;
			}
			int byteLen = bytes.Length - 1;
			for (; byteLen > 0; byteLen--)
			{
				if (bytes[byteLen] != 0xFF)
				{
					break;
				}
			}
			string infosStr = Encoding.UTF8.GetString(bytes, 0, byteLen + 1);
			if (string.IsNullOrWhiteSpace(infosStr) || !infosStr.Contains('$'))
			{
				return false;
			}
			var infos = infosStr.Split('$').ToList();
			if (infos.Count < 3)
			{
				return false;
			}
			for (int i = 0; i < infos.Count; i++)
			{
				var info = infos[i];
				switch (i)
				{
					case 0:
						checkCodeMD5 = info;
						break;
					case 1:
						checkCodeCRC = info;
						break;
					case 2:
						serialNumber = info;
						break;
					case 3:
						productModel = info;
						break;
					case 4:
						if (DateTime.TryParse(info, out DateTime result))
						{
							productionDate = result;
						}

						break;
					case 5:
						channelInfo = info;
						break;
					case 6:
						otherInfo = info;
						break;
					case 7:
						protocolVersion = info;
						break;
					case 8:
						hardVersion = info;
						break;

				}
			}

			return true;
		}
		public bool ValidSN()
		{
			if (checkCodeMD5 == null || string.IsNullOrWhiteSpace(checkCodeMD5))
			{
				return false;
			}
			return checkCodeMD5.Trim() == MD5Util.GetMD5_32(serialNumber).Trim();
		}

		public bool ValidInfo()
		{
			if (checkCodeCRC == null || string.IsNullOrWhiteSpace(checkCodeCRC))
			{
				return false;
			}
			return checkCodeCRC.Trim() == CRCUtil.CRCCalc(InfosToString()).Trim();
		}
	}
}
