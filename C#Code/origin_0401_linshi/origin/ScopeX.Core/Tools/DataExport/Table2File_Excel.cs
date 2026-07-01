using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ScopeX.Core.Tools.DataExport
{
    internal class Table2File_Excel : IConvertTable2FileBytes
    {
        public Boolean SupportMultiple => true;

        public Byte[] Convert(DataTable table) => ExcelHelper.ExportWorkbook(table);

        /// <summary>
        /// 将多个表格导出到一个文件中去，类似一个Excel支持多个Sheet
        /// </summary>
        /// <param name="tables">表格数据集合</param>
        /// <returns>文件数据二进制</returns>
        public Byte[] ConvertMultiple(IEnumerable<DataTable> tables)
        {
            if (tables == null)
                return null;

            return ExcelHelper.ExportDT2Excel(tables.ToArray());
        }
    }
}
