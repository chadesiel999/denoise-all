using ScopeX.U2.BaseControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal static class DataHelper
    {
        public static DataTable GetDataTable(this IDataExportView dataview, UserControls.ScopeXListViewEx listView, String title)
        {
            if (listView == null)
            {
                return null;
            }

            DataTable dataTable = new DataTable() { TableName = title };

            //2025.04.14 不需要再增加标题列
            //dataTable.Columns.Add(new DataColumn(title)); // 补充标题列 
            foreach (ColumnHeader item in listView.Columns)
            {
                if (item == null || String.IsNullOrEmpty(item.Text))
                {
                    dataTable.Columns.Add(" ");
                }
                else
                {
                    dataTable.Columns.Add(item.Text);
                }
            }

            DataRow row = null;
            foreach (ListViewItem item in listView.Items)
            {
                if (item == null || item.SubItems == null || item.SubItems.Count <= 0)
                    continue;

                row = dataTable.NewRow();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (i >= item.SubItems.Count)
                        continue;
                    row[i] = item.SubItems[i]?.Text.ToString();
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
