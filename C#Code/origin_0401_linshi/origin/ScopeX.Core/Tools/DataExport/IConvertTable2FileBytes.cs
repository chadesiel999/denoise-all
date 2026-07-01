using System;
using System.Collections.Generic;
using System.Data;

namespace ScopeX.Core.Tools.DataExport
{
    /// <summary>
    /// 保存表格数据到文件的接口
    /// </summary>
    internal interface IConvertTable2FileBytes
    {
        /// <summary>
        /// 是否支持将多个表格导出到一个文件
        /// </summary>
        public Boolean SupportMultiple { get; }

        /// <summary>
        /// 将一个表格保存为一个文件
        /// </summary>
        /// <param name="table">表格数据</param>
        /// <returns>是否保存成功</returns>
        public Byte[] Convert(DataTable table);

        /// <summary>
        /// 将多个表格导出到一个文件中去，类似一个Excel支持多个Sheet
        /// </summary>
        /// <param name="tables">表格数据集合</param>
        /// <returns>文件数据二进制</returns>
        public Byte[] ConvertMultiple(IEnumerable<DataTable> tables)
        {
            if (!SupportMultiple)
                return null;

            return new Byte[0];
        }
    }
}
