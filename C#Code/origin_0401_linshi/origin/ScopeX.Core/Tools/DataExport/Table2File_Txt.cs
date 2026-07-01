using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScopeX.Core.Tools.DataExport
{
    internal class Table2File_Txt : IConvertTable2FileBytes
    {
        public Boolean SupportMultiple => true;

        public Byte[] Convert(DataTable table) => SaveDataTablesToTextFile(table);

        /// <summary>
        /// 将多个表格导出到一个文件中去，类似一个Excel支持多个Sheet
        /// </summary>
        /// <param name="tables">表格数据集合</param>
        /// <returns>文件数据二进制</returns>
        public Byte[] ConvertMultiple(IEnumerable<DataTable> tables)
        {
            if (!SupportMultiple)
                return null;

            return SaveDataTablesToTextFile(tables.ToArray());
        }

        private Byte[] SaveDataTablesToTextFile(params DataTable[] tables)
        {
            // 使用 StringBuilder 构建文本内容
            StringBuilder sb = new StringBuilder();

            foreach (DataTable table in tables)
            {
                sb.AppendLine($"表名：{table.TableName ?? "未知"}");
                // 添加表头
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append($"| {column.ColumnName,-15} ");
                }
                sb.AppendLine();

                // 添加分隔线
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append($"| {'-'.ToString().PadLeft(/*column.ColumnName.Length + 14*/16, '-')} ");
                }
                sb.AppendLine();

                // 添加数据
                foreach (DataRow row in table.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        sb.Append($"| {item,-15} ");
                    }
                    sb.AppendLine();
                }


                // 添加表格分隔符
                sb.AppendLine("---------------------------------------");
                sb.AppendLine("");
                sb.AppendLine("");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
