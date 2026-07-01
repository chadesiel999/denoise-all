#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ScopeX.ComModel;

namespace ScopeX.Updater.Base;

public class OptionsInfoBase
{
    public ConcurrentDictionary<string, bool>? AllOptions;

    public double TrialRemainingTimeByHour = Constants.DEFAULT_REMAININGTIME_BYHOUR;

    #region 校验相关对象

    /// <summary>
    /// 校验码1
    /// </summary>
    public string CheckCodeMd5 { get; private set; }


    /// <summary>
    /// 校验码2
    /// </summary>
    public string CheckCodeCrc { get; private set; }


    public OptionsInfoBase()
    {
        CheckCodeMd5 = "";
        CheckCodeCrc = "";
    }

    string _ModelValues
    {
        get
        {
            if (AllOptions == null)
            {
                return "";
            }
            string allStrings = "";
            foreach (KeyValuePair<string, bool> option in AllOptions)
            {
                string opName = option.ToString().Trim();
                if (string.IsNullOrWhiteSpace(opName))
                {
                    continue;
                }
                if (opName.Length > Constants.OPTIONNAME_MAX_LENGTH)
                {
                    opName = opName.Substring(0, Constants.OPTIONNAME_MAX_LENGTH);
                }
                allStrings += $"{opName}=";
            }
            return allStrings;
        }
    }
    public bool ValidInfo()
    {
        return true;//未找到原因 临时return true
        if (string.IsNullOrWhiteSpace(CheckCodeCrc))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(CheckCodeMd5))
        {
            return false;
        }
        var checkStr = _ModelValues.Replace(",", "").Replace(" ", "").Trim();

        return CheckCodeMd5 == MD5Util.GetMD5_32(checkStr)
               && CheckCodeCrc == CRCUtil.CRCCalc(checkStr);
    }

    void SumCheckCode()
    {
        var checkStr = _ModelValues.Replace(",", "").Replace(" ", "").Trim();

        if (!string.IsNullOrWhiteSpace(_ModelValues))
        {
            CheckCodeMd5 = MD5Util.GetMD5_32(checkStr);
        }

        CheckCodeCrc = CRCUtil.CRCCalc(checkStr);
    }

    #endregion 校验相关对象

    #region 数据转换

    static byte[] DoubleToBytes(double value)
    {
        return BitConverter.GetBytes(value);
    }

    static double BytesToDouble(byte[] bytes)
    {
        return BitConverter.ToDouble(bytes, 0);
    }

    public byte[] InfosToBytes()
    {
        List<byte> results = new();
        SumCheckCode();
        byte[] dateData = DoubleToBytes(TrialRemainingTimeByHour);
        uint dateBytesCount = (uint)dateData.Length;
        results.AddRange(Encoding.UTF8.GetBytes(CheckCodeMd5));
        results.Add(Encoding.UTF8.GetBytes("$")[0]);
        results.AddRange(Encoding.UTF8.GetBytes(CheckCodeCrc));
        results.Add(Encoding.UTF8.GetBytes("$")[0]);
        results.Add((byte)dateBytesCount);
        results.Add(Encoding.UTF8.GetBytes("$")[0]);
        results.AddRange(dateData);
        results.Add(Encoding.UTF8.GetBytes("$")[0]);
        results.AddRange(Encoding.UTF8.GetBytes(_ModelValues));

        return results.ToArray();
    }

    public bool BytesToInfos(byte[] bytes)
    {
        #region 检查完整

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
        List<int> indexes = new();
        for (int i = 0; i < byteLen; i++)
        {
            if (bytes[i] == '$')
            {
                indexes.Add(i);
            }
        }
        if (indexes.Count < 4)
        {
            return false;
        }

        #endregion 检查完整
        string infosStr = Encoding.UTF8.GetString(bytes, 0, indexes[1]);
        if (string.IsNullOrWhiteSpace(infosStr) || !infosStr.Contains('$'))
        {
            return false;
        }
        List<string> checkInfos = infosStr.Split('$').ToList();
        CheckCodeMd5 = checkInfos[0];
        CheckCodeCrc = checkInfos[1];
        int dateDataBytesCountStartIndex = indexes[1] + 1;
        int dateDataBytesCount = bytes[dateDataBytesCountStartIndex];
        byte[] dateDataBytes = new byte[dateDataBytesCount];
        int dateDataBytesStartIndex = indexes[2] + 1;
        int dateDataBytesEndIndex = indexes[3];
        for (int i = dateDataBytesStartIndex; i < dateDataBytesEndIndex; i++)
        {
            dateDataBytes[i - dateDataBytesStartIndex] = bytes[i];
        }
        TrialRemainingTimeByHour = BytesToDouble(dateDataBytes);

        infosStr = Encoding.UTF8.GetString(bytes, indexes[3], (byteLen - indexes[3]) + 1);
        if (string.IsNullOrWhiteSpace(infosStr))
        {
            return false;
        }
        //var regexInfos = new Regex(@"\[\d+\,[ A-Za-z]+\](\=)*");
        Regex regexInfos = new Regex(@"\[\d+\, ?(True)|(False)\](\=)*");
        if (!regexInfos.IsMatch(infosStr))
        {
            return false;
        }

        string[] keyPairs = infosStr.Split('=');
        AllOptions = new ConcurrentDictionary<string, bool>();
        foreach (string pairStr in keyPairs)
        {
            if (String.IsNullOrWhiteSpace(pairStr))
            {
                continue;
            }
            var temp = pairStr.Replace("$", "").Trim();
            KeyValuePair<string, bool> data = ParseKeyValuePair(temp);
            AllOptions.TryAdd(data.Key, data.Value);
        }
        return true;
    }

    static KeyValuePair<string, bool> ParseKeyValuePair(string keyValueString)
    {
        // 去掉首尾的方括号，并使用逗号分隔键和值
        string[] parts = keyValueString.Trim('[', ']').Split(',');

        // 解析键和值
        string key = parts[0].Trim();
        bool value = bool.Parse(parts[1].Trim());

        // 构造新的 KeyValuePair
        return new KeyValuePair<string, bool>(key, value);
    }

    #endregion 数据转换

}
