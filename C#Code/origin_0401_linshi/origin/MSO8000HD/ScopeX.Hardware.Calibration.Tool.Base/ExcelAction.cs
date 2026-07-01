using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    using Excel = Microsoft.Office.Interop.Excel;
    public class ExcelAction
    {
        private bool bOpened = false;
        public static ExcelAction Default = new ExcelAction();
        private Excel.Application? currApplication = null;
        private Excel.Workbook? currWorkbook = null;
        private Excel.Worksheet? currWorksheet = null;
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processid);

        public bool OpenFile(string fileName)
        {
            if (currApplication != null)
            {
                if (currWorkbook != null)
                    currWorkbook.Close();
                currApplication.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(currApplication);
                currApplication = null;
            }
            currWorkbook = null;
            currApplication = null;
            try
            {
                currApplication = new Excel.Application();
                currApplication.Visible = false;//设置调用引用的 Excel文件是否可见
                currApplication.DisplayAlerts = false;
                currWorkbook = currApplication.Workbooks.Open(fileName);
                currApplication.AlertBeforeOverwriting = false;
                bOpened = true;
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static bool IsValidCell(string cell)
        {
            string cc = cell;
            int markPos = cell.IndexOf('!');
            if (markPos >= 0)
                cc = cell.Substring(markPos + 1);
            int digitStartPos = 0;
            cc = cc.ToUpper();
            foreach(char c in cc)
            {
                if (!(c >= 'A' && c <= 'Z'))
                    break;
                digitStartPos++;
            }
            if (digitStartPos == 0 || digitStartPos == cc.Length)
                return false;
            for(int index= digitStartPos;index<cc.Length;index++)
            {
                if (!(cc[index] >= '0' && cc[index] <= '9'))
                    return false;
            }
            return true;
        }
        private string Address(string inAddress)
        {
            string address = inAddress.ToUpper().Trim();
            if (address.IndexOf("ADDRESS(") >= 0)
            {
                address = address.Substring(address.IndexOf("ADDRESS(") + "ADDRESS(".Length);
                address = address.TrimEnd(')');
                string[] colRow= address.Split(',');
                object? obj= BaseHelper.CalcCSharpFormat(colRow[1]);
                int intCol = 1;
                if (obj != null)
                    intCol = (int)obj;
                obj = BaseHelper.CalcCSharpFormat(colRow[0]);
                int intRow = 1;
                if(obj!= null)
                    intRow = (int)obj;

                int intFirstLetter = ((intCol) / 676) + 64;
                int intSecondLetter = ((intCol % 676) / 26) + 64;
                int intThirdLetter = (intCol % 26) + 64;

                char FirstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
                char SecondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
                char ThirdLetter = (char)intThirdLetter;
                string ColStr = string.Concat(FirstLetter, SecondLetter, ThirdLetter).Trim();
                return $"${ColStr}${intRow}";
            }
            else
                return address;
        }
        public void WriteCell(string sheet_cell, string cellValue)
        {
            if (!bOpened)
                return;
            if (currWorkbook==null)
                return;
            string[] sheetAndCell = sheet_cell.Split('!');
            string cell;
            if (sheetAndCell.Length == 1)
            {
                currWorksheet = (Excel.Worksheet)currWorkbook.Worksheets[1]; //索引从1开始 
                cell = sheetAndCell[0];
            }
            else
            {
                currWorksheet = null;
                foreach (Excel.Worksheet sheet in currWorkbook.Worksheets)
                {
                    if (sheet.Name == sheetAndCell[0])
                        currWorksheet = sheet;
                }
                if (currWorksheet == null)
                {
                    Logger.Defualt.WriteLine("指定的Sheet [" + sheetAndCell[0] + "]不存在！");
                    return;
                }
                cell = sheetAndCell[1];
            }
            if (currWorksheet != null)
            {
                cell = Address(cell);
                Excel.Range? range = (Excel.Range)currWorksheet.get_Range(cell, cell);
                range.Value2 = cellValue;
                range = null;
            }
        }
        public void InsertPicture(string pictureName,string sheet_cell, float pictureWidth, float pictureHeight)
        {
            if (!bOpened)
                return;
            if (currWorkbook == null)
                return;
            string[] sheetAndCell = sheet_cell.Split('!');
            string cell;
            if (sheetAndCell.Length == 1)
            {
                currWorksheet = (Excel.Worksheet)currWorkbook.Worksheets[1]; //索引从1开始 
                cell = sheetAndCell[0];
            }
            else
            {
                currWorksheet = null;
                foreach (Excel.Worksheet sheet in currWorkbook.Worksheets)
                {
                    if (sheet.Name == sheetAndCell[0])
                        currWorksheet = sheet;
                }
                if (currWorksheet == null)
                {
                    Logger.Defualt.WriteLine("指定的Sheet [" + sheetAndCell[0] + "]不存在！");
                    return;
                }
                cell = sheetAndCell[1];
            }
            if (currWorksheet != null)
            {
                Excel.Range? range = (Excel.Range)currWorksheet.get_Range(cell, cell);
                range.Select();
                float PicLeft = Convert.ToSingle(range.Left);
                float PicTop = Convert.ToSingle(range.Top);
                currWorksheet.Shapes.AddPicture(pictureName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, PicLeft, PicTop, pictureWidth, pictureHeight);

                range = null;
            }
        }
        public void InsertHyperlinks(string sheet_cell,string title,string Hyperlinks)
        {
            if (!bOpened)
                return;
            if (currWorkbook == null)
                return;
            string[] sheetAndCell = sheet_cell.Split('!');
            string cell;
            if (sheetAndCell.Length == 1)
            {
                currWorksheet = (Excel.Worksheet)currWorkbook.Worksheets[1]; //索引从1开始 
                cell = sheetAndCell[0];
            }
            else
            {
                currWorksheet = null;
                foreach (Excel.Worksheet sheet in currWorkbook.Worksheets)
                {
                    if (sheet.Name == sheetAndCell[0])
                        currWorksheet = sheet;
                }
                if (currWorksheet == null)
                {
                    Logger.Defualt.WriteLine("指定的Sheet [" + sheetAndCell[0] + "]不存在！");
                    return;
                }
                cell = sheetAndCell[1];
            }
            if (currWorksheet != null)
            {
                Excel.Range? range = (Excel.Range)currWorksheet.get_Range(cell, cell);
                range.Select();
                range.Value2 = title;
                currWorksheet.Hyperlinks.Add(range, Hyperlinks);
                range = null;
            }
        }
        public void Close()
        {
            bOpened = false;
            if (currApplication == null)
                return;
            if (currWorkbook == null)
            {
                currApplication.Quit();
                currApplication = null;
                return;
            }
            currWorkbook.Save();
            currWorkbook.Close();
            currWorkbook = null;
            currApplication.Workbooks.Close();
            currApplication.Quit();
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(currApplication);
            GetWindowThreadProcessId(new IntPtr(currApplication.Hwnd), out int pid);
            System.Diagnostics.Process.GetProcessById(pid).Kill();
            currApplication = null;
        }
    }
}
