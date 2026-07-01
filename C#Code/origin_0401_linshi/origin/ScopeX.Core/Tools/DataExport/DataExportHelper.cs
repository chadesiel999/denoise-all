using ScopeX.ComModel;
using ScopeX.Core.Decode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static ScopeX.Controls.Common.APIs.APIsStructs;
using System.Text.RegularExpressions;

namespace ScopeX.Core.Tools.DataExport
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        CSV,

        Text,

        Excel,
    }

    /// <summary>
    /// 数据导出帮助类，导出为指定文件格式的 二进制数据(同时兼容UI界面和SCPI指令等)
    /// </summary>
    public static class DataExportHelper
    {
        /// <summary>
        /// 将单个表格保存为指定格式的文件二进制数据(一个表格一个文件)
        /// </summary>
        /// <param name="tables">表格数据</param>
        /// <param name="fileType">存储文件类型</param>
        /// <returns>是否保存成功</returns>
        public static Byte[] ConvertTables2FileBytes(FileType fileType = FileType.CSV, params DataTable[] tables)
        {
            if (tables == null || !tables.Any())
                return null;

            /*var has_datas_tables = tables.Where(c => c != null && c.Rows.Count > 0).ToList();
            if (has_datas_tables == null || !has_datas_tables.Any())
                return null;*/

            IConvertTable2FileBytes handler = null;
            switch (fileType)
            {
                case FileType.CSV:
                default:
                    handler = new Table2File_CSV();
                    break;
                case FileType.Text:
                    handler = new Table2File_Txt();
                    break;
                case FileType.Excel:
                    handler = new Table2File_Excel();
                    break;
            }

            if (handler == null)
                return null;

            if (tables.Count() == 1)
            {
                return handler.Convert(tables.FirstOrDefault()!);

            }
            else
            {
                if (handler.SupportMultiple)
                {
                    return handler.ConvertMultiple(tables);
                }
                else
                {
                    // 多文件，但不支持多文件转换
                    throw new Exception("Data export cannot be stored in multiple tables.");
                }
            }
        }
        public static String? GetDecodeData(List<ProtocolEventInfo> eventInfos, Int32 colunmcount, (Int64 start, Int64 end) range)
        {
            var result = new List<String[]>();
            //提取更新信息
            if (eventInfos == null || eventInfos.Count == 0)
            {
                return null;
            }
            else
            {
                result.AddRange(eventInfos
                 .Skip((Int32)range.start) // 跳过开始的N个元素
                 .Take((Int32)range.end - (Int32)range.start).Select(info =>
                 {
                     var itemcontent = new List<String>();
                     itemcontent.Add((info.Index + 1).ToString());
                     if (Double.IsNaN(info.StartTimeByPs))
                     {
                         itemcontent.Add($"{Double.MaxValue}");
                     }
                     else
                     {
                         //itemcontent.Add(ScopeX.Controls.Common.Helper.SIHelper.ValueChangeToSI(info.TimeByPs / 1E+12, 2, "s"));
                         itemcontent.Add((info.StartTimeByPs / 1E+12).ToString("E5"));
                         info.EventInofs.ForEach(datas =>
                         {
                             if (datas.BitCount == 0)//文字
                             {
                                 String res = System.Text.Encoding.Default.GetString(datas.Data);
                                 res = res.Replace("--", $"{Double.NaN}");
                                 itemcontent.Add(res);
                             }
                             else if (datas.BitCount == 1)//True or False
                             {
                                 itemcontent.Add(Convert.ToBoolean(datas.Data[0]).ToString());
                             }
                             else//数字(数据量大的时候容易界面卡死)
                             {
                                 var temp = GetHexString(datas.Data, datas.BitCount);
                                 itemcontent.Add(temp);
                             }
                         });
                     }
                     //检查表头数量和内容子项长度是否一致,不足填零
                     for (Int32 i = itemcontent.Count; i < colunmcount; i++)
                     {
                         itemcontent.Add(String.Empty);
                     }
                     return itemcontent.ToArray();
                 }));
            }
            String outputstring = String.Empty;
            foreach (var data in result)
            {
                outputstring += String.Join(",", data) + Environment.NewLine;
            }
            if (!String.IsNullOrEmpty(outputstring))
            {
                return outputstring;
            }
            else
            {
                return null;
            }
        }

        private static String GetHexString(Byte[] data, UInt32 bitcount)
        {
            String temp = "";
            Int32 bytecount = (Int32)Math.Ceiling(bitcount / 8f);
            List<Byte> tempbytes = new List<Byte>();
            if (data.Length < bytecount)
            {
                tempbytes.AddRange(Enumerable.Repeat<Byte>(0, bytecount - data.Length));
            }

            tempbytes.AddRange(data);
            for (Int32 i = 0; i < bytecount; i++)
            {
                Byte lenght = 0;
                if (i == 0)
                {
                    lenght = (Byte)(bitcount % 8);
                }
                else
                {
                    lenght = 7;
                }
                if (lenght == 0)
                {
                    lenght = 7;
                }
                Byte tempvalue = (Byte)(tempbytes[i] & (Byte)(Math.Pow(2, lenght + 1) - 1));
                temp += Convert.ToString(tempvalue, 16).PadLeft(2, '0') + " ";
            }
            return temp.ToUpper().Trim();
        }

    }
}
