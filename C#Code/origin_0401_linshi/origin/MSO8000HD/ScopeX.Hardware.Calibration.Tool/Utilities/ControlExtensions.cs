using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal static class ControlExtensions
    {
        public static void SaveContentAsCsv(this DataGridView dgv, string prefix)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string filePath = Path.Combine(fbd.SelectedPath, $"{prefix}_{DateTime.Now.ToString("yy_MM_dd_HH_mm_ss")}.csv");
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] content = new UTF8Encoding(true).GetBytes(dgv.GetContentAsCsv());
                    fileStream.Write(content, 0, content.Length);
                    fileStream.Flush();
                }
            }
        }

        public static string GetContentAsCsv(this DataGridView dgv)
        {
            StringBuilder sb = new StringBuilder();
            //保存列内容
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                sb.Append(dgv.Columns[i].HeaderText);
                sb.Append((i != dgv.Columns.Count - 1) ? ",": "\r\n");
            }
            //保存所有row的内容
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    sb.Append(row.Cells[i].Value.ToString());
                    sb.Append((i != row.Cells.Count - 1) ? "," : "\r\n");
                }
            }
            return sb.ToString();
        }
    }
}
