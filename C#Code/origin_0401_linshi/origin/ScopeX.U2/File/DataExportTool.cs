using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventBus;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core.Tools;
using ScopeX.Core.Tools.DataExport;
using ScopeX.U2.BaseControl;

namespace ScopeX.U2.File
{
    public static class DataExportTool
    {
        private static String _LastSavePath = String.Empty;
        private static Int32 _LastSaveType = 0;
        public static Boolean DataExport(this IDataExportView dataview)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt|Excel(*.xls)|*.xls|csv(*.csv)|*.csv"; // csv(*.csv)|*.csv|
            dialog.SupportMultiDottedExtensions = false;
            dialog.OverwritePrompt = true;
            // 设置上次保存的文件夹路径作为默认路径
            if (!String.IsNullOrEmpty(_LastSavePath) && Directory.Exists(Path.GetDirectoryName(_LastSavePath)))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_LastSavePath);
            }
            else
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            dialog.FilterIndex = _LastSaveType;
            dialog.SetWindowInCenter();
            if (dialog.ShowDialog() != DialogResult.OK || String.IsNullOrWhiteSpace(dialog.FileName))
                return false;

            var ext = System.IO.Path.GetExtension(dialog.FileName).ToUpper();
            FileType fileType;
            switch (ext)
            {
                case ".TXT":
                    fileType = FileType.Text;
                    _LastSaveType = 1;
                    break;
                case ".XLS":
                    fileType = FileType.Excel;
                    _LastSaveType = 2;
                    break;
                case ".CSV":
                    fileType = FileType.CSV;
                    _LastSaveType = 3;
                    break;
                default:
                    _LastSaveType = 1;
                    StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.UnSupportedFormat, MessageType.Warning);
                    return false;
            }
            _LastSavePath = dialog.FileName;
            try
            {
                var tables = dataview.GetDataTables();

                // 使用 UTF-8 编码来处理 CSV 文件
                if (fileType == FileType.CSV)
                {
                    // 先将 DataTable 转换为 CSV 字符串
                    var csvContent = ConvertDataTablesToCsv(tables);

                    // 保存 CSV 文件时使用 UTF-8 编码
                    System.IO.File.WriteAllText(dialog.FileName, csvContent, Encoding.UTF8);
                }
                else
                {
                    var bytes = DataExportHelper.ConvertTables2FileBytes(fileType, tables.ToArray());
                    System.IO.File.WriteAllBytes(_LastSavePath, bytes);
                }

                WeakTip.Default.Write("Export", MsgTipId.SavingSuccess, false, Path.GetDirectoryName(dialog.FileName));
                return true;
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                WeakTip.Default.Write("Export", MsgTipId.SavingFailed);
                return false;
            }
        }

        private static String ConvertDataTablesToCsv(List<DataTable> tables)
        {
            var sb = new StringBuilder();

            foreach (DataTable table in tables)
            {
                // 添加列标题
                var columns = table.Columns.Cast<DataColumn>();
                sb.AppendLine(String.Join(",", columns.Select(c => c.ColumnName)));

                // 添加数据行
                foreach (DataRow row in table.Rows)
                {
                    sb.AppendLine(String.Join(",", row.ItemArray.Select(field => $"\"{field.ToString().Replace("\"", "\"\"")}\"")));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
