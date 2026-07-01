using System;
using System.Data;
using System.IO;
using System.Linq;

namespace ScopeX.Core.Tools.DataExport
{
    /// <summary>
    /// 将Table保存为CSV文件
    /// </summary>
    internal class Table2File_CSV : IConvertTable2FileBytes
    {
        public Boolean SupportMultiple => false;

        public Byte[] Convert(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(ms))
                    {
                        // Write header
                        String[] header = table.Columns.Cast<DataColumn>().Select(column => QuoteCsvField(column.ColumnName)).ToArray();
                        writer.WriteLine(String.Join(",", header));

                        // Write rows
                        foreach (DataRow row in table.Rows)
                        {
                            String[] fields = row.ItemArray.Select(field => QuoteCsvField((field ?? "").ToString())).ToArray();
                            writer.WriteLine(String.Join(",", fields));
                        }
                    }

                    return ms.GetBuffer();
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                return null;
            }
        }

        /// <summary>
        /// 分隔符替换，以免CSV格式错乱
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private String QuoteCsvField(String field)
        {
            if (field.Contains(",") || field.Contains("\""))
            {
                field = field.Replace("\"", "\"\"");
                field = "\"" + field + "\"";
            }
            return field;
        }

        private String UnquoteCsvField(String field)
        {
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field.Substring(1, field.Length - 2);
                field = field.Replace("\"\"", "\"");
            }
            return field;
        }
    }
}
