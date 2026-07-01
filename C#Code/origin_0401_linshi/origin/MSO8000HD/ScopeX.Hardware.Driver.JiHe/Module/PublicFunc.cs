using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal static class PublicFunc
    {
        /// <summary>
        /// 将每个元素对应的bit位 置1
        /// </summary>
        /// <param name="enableId"></param>
        /// <returns></returns>
        internal static UInt32 ConvertToUniqueHotCode(IEnumerable<Int32> enableId)
        {
            UInt32 result = 0;
            foreach (Int32 id in enableId)
            {
                result |= 0x1u << id;
            }
            return result;
        }

        /// <summary>
        /// 提取独热码中每个为1的bit位
        /// </summary>
        /// <param name="hotCode"></param>
        /// <returns></returns>
        internal static IEnumerable<Int32> ConvertFromUniqueHotCode(UInt32 hotCode)
        {
            for (Int32 bit = 0; bit < 32; bit++)
            {
                if (((0x1u << bit) & hotCode) != 0)
                    yield return bit;
            }
        }

        /// <summary>
        /// 获取二进制中，1的个数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static Int32 GetBinaryBitCnt(UInt32 data)
        {
            int count = 0;
            while (data != 0)
            {
                count++;
                data &= (data - 1);
            }
            return count;
        }

        internal static void WriteLog(String logInfo)
        {
            // Driver层打印开关
            if (true)
            {
                Trace.WriteLine(logInfo);
            }
        }

        internal static void SaveDataToFile(String fileName, List<UInt16> data)
        {
            StreamWriter sw = new StreamWriter(fileName);
            for (Int32 i = 0; i < data.Count; i++)
            {
                sw.WriteLine(data[i]);
            }
            sw.Flush();
            sw.Close();
        }

        internal static void SaveDataToFile<T>(String fileName, List<T> data, Boolean append = false)
        {
            StreamWriter sw = new StreamWriter(fileName, append);
            for (Int32 i = 0; i < data.Count; i++)
            {
                sw.WriteLine(data[i]);
            }
            sw.Flush();
            sw.Close();
        }

        internal static Boolean LoadDataFormFile(String fileName, out List<Double> data)
        {
            data = new List<Double>();
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                while (!sr.EndOfStream)
                {
                    String? tmpstr = sr.ReadLine();
                    if (Double.TryParse(tmpstr, out Double tmpdata))
                    {
                        data.Add(tmpdata);
                    }
                }
                return true;
            }
            return false;
        }

        internal static Boolean CheckDictionaryEqual<T, V>(Dictionary<T, V> source1, Dictionary<T, V> source2) where T : struct where V : struct
        {
            foreach (T source1key in source1.Keys)
            {
                if (!source2.ContainsKey(source1key) || !source2[source1key].Equals(source1[source1key]))
                {
                    return false;
                }
            }

            foreach (T source2key in source2.Keys)
            {
                if (!source1.ContainsKey(source2key) || !source2[source2key].Equals(source1[source2key]))
                {
                    return false;
                }
            }

            return true;
        }

        internal static void CopyDictionary<T, V>(Dictionary<T, V> source, Dictionary<T, V> destination) where T : struct where V: struct
        {
            destination.Clear();
            foreach (T sourcekey in source.Keys)
            {
                destination[sourcekey] = source[sourcekey];
            }
        }
    }
}
