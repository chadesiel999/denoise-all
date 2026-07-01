using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Data;
using System.IO;
using System.Text;
namespace ScopeX.Core.Tools
{
    /// <summary>
    /// Excel帮助类
    /// </summary>
    public static class ExcelHelper
    {
        #region 从datatable中将数据导出到excel

        private const String _ReplaceChar = "_";
        private static String CharReplace(String str) => str.Replace("\\", _ReplaceChar).Replace("/", _ReplaceChar).Replace(":", _ReplaceChar).Replace("?", _ReplaceChar).Replace("*", _ReplaceChar).Replace("<", _ReplaceChar).Replace(">", _ReplaceChar).Replace("[", _ReplaceChar).Replace("]", _ReplaceChar);

        public static Byte[] ExportWorkbook(DataTable dtSource, String ColumnProperty = null)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            String SheetName = "Sheet1";
            if (!String.IsNullOrEmpty(dtSource.TableName)) SheetName = CharReplace(dtSource.TableName);
            HSSFSheet sheet = workbook.CreateSheet(SheetName) as HSSFSheet;

            // 标题样式
            HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.VerticalAlignment = VerticalAlignment.Center;
            headStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.WrapText = true;
            HSSFFont font = workbook.CreateFont() as HSSFFont;
            font.Boldweight = (short)FontBoldWeight.Bold;
            headStyle.SetFont(font);

            //内容样式
            HSSFCellStyle cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.CloneStyleFrom(headStyle);
            cellStyle.Alignment = HorizontalAlignment.Center;
            font = workbook.CreateFont() as HSSFFont;
            font.Boldweight = (short)FontBoldWeight.Normal;
            cellStyle.SetFont(font);
            HSSFCellStyle cellStyleLeft = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyleLeft.CloneStyleFrom(cellStyle);
            cellStyleLeft.Alignment = HorizontalAlignment.Left;
            HSSFCellStyle cellStyleRight = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyleRight.CloneStyleFrom(cellStyle);
            cellStyleRight.Alignment = HorizontalAlignment.Right;

            Int32 rowIndex = 0;
            #region 列头及样式
            HSSFRow headerRow = sheet.CreateRow(rowIndex) as HSSFRow;
            foreach (DataColumn column in dtSource.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(CharReplace(column.ColumnName));
                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
            }
            #endregion
            rowIndex++;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                    newCell.CellStyle = cellStyle;
                    AutoSetType(newCell, column, row);
                }

                #endregion
                rowIndex++;
            }
            AutoSizeColumns(sheet);

            if (ColumnProperty != null)
            {
                String[] ColumnArray = ColumnProperty.Split('|');
                foreach (String Column in ColumnArray)
                {
                    String[] ValueArray = Column.Split(',');
                    if (ValueArray.Length > 2)
                    {
                        Int32 cols = Int32.Parse(ValueArray[1]);
                        switch (ValueArray[0].ToString())
                        {
                            case "Merge":
                                Int32 precols = 0;
                                Int32.TryParse(ValueArray[2], out precols);
                                GroupCol(sheet, 1, cols, precols);
                                break;
                            case "Width":
                                Int32 columnWidth = 0;
                                Int32.TryParse(ValueArray[2], out columnWidth);
                                sheet.SetColumnWidth(cols, columnWidth * 256);
                                break;
                            case "Align":
                                String Value = ValueArray[2].ToString();
                                for (Int32 j = 1; j <= sheet.LastRowNum; j++)
                                {
                                    ICell currentCell = sheet.GetRow(j).GetCell(cols);
                                    switch (Value)
                                    {
                                        case "center":
                                            currentCell.CellStyle = cellStyle;
                                            break;
                                        case "left":
                                            currentCell.CellStyle = cellStyleLeft;
                                            break;
                                        case "right":
                                            currentCell.CellStyle = cellStyleRight;
                                            break;
                                    }
                                }
                                break;
                            case "DataType":
                                String Value2 = ValueArray[2].ToString();
                                for (Int32 j = 1; j <= sheet.LastRowNum; j++)
                                {
                                    ICell currentCell = sheet.GetRow(j).GetCell(cols);
                                    String drValue = currentCell.ToString();
                                    switch (Value2)
                                    {
                                        case "Double":
                                            Double result;
                                            if (Double.TryParse(drValue, out result))
                                                currentCell.SetCellValue(result);
                                            break;
                                        case "String":
                                            currentCell.SetCellValue(drValue.ToString());
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 导出EXCEL,可以导出多个sheet(不设置列宽)
        /// </summary>
        /// <param name="dtSources">原始数据表</param>
        /// <param name="NeedMargeColumns">需要合并的列，如：3,5</param>
        public static Byte[] ExportDT2ExcelNoWidth(DataTable[] dtSources, String NeedMargeColumns = "")
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            if (NeedMargeColumns.Length > 0)
            {
                NeedMargeColumns = "," + NeedMargeColumns + ",";
            }
            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            for (Int32 k = 0; k < dtSources.Length; k++)
            {
                Int32[] margeColIndex = new Int32[dtSources[k].Columns.Count];//保存合并列的起始行
                String[] margeColValue = new String[dtSources[k].Columns.Count];//保存合并列对应的上一个值      
                HSSFSheet sheet = workbook.CreateSheet(CharReplace(dtSources[k].TableName.ToString())) as HSSFSheet;
                //填充表头
                HSSFRow dataRow = sheet.CreateRow(0) as HSSFRow;
                ICellStyle Headstyle = workbook.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                Headstyle.Alignment = HorizontalAlignment.Center;
                //新建一个字体样式对象
                IFont font = workbook.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                Headstyle.SetFont(font);
                foreach (DataColumn column in dtSources[k].Columns)
                {
                    margeColIndex[column.Ordinal] = 1;
                    margeColValue[column.Ordinal] = "";
                    ICell cell = dataRow.CreateCell(column.Ordinal);//创建单元格
                    cell.SetCellValue(CharReplace(column.ColumnName));//设置单元格的值
                    dataRow.GetCell(column.Ordinal).CellStyle = Headstyle; //将新的样式赋给单元格
                }
                //填充内容
                for (Int32 i = 0; i < dtSources[k].Rows.Count; i++)
                {
                    dataRow = sheet.CreateRow(i + 1) as HSSFRow;
                    for (Int32 j = 0; j < dtSources[k].Columns.Count; j++)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        Boolean canSetValue = false;
                        if (NeedMargeColumns.Length > 0 && (NeedMargeColumns.IndexOf("," + j.ToString() + ",") != -1)) //有需要合并的列,并且当前列需要合并              
                        {
                            if (margeColValue[j] == "")
                            {
                                canSetValue = true;
                                margeColIndex[j] = i + 1;
                                margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                            }
                            else if (margeColValue[j] != dtSources[k].Rows[i][j].ToString())//不同的时候需要更新原来储存的值，并重新设置合并
                            {
                                canSetValue = true;
                                //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                                sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i, j, j));
                                margeColIndex[j] = i + 1;
                                margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                            }
                        }
                        else
                        {
                            canSetValue = true;
                        }
                        if (canSetValue)
                        {
                            //需要合并的列，1.第一行/最后一行,2.上一行值不同，下一行值相同，则给当前单元格赋值
                            switch (dtSources[k].Columns[j].DataType.ToString())
                            {
                                case "System.String"://字符串类型   
                                    newCell.SetCellValue(dtSources[k].Rows[i][j].ToString());
                                    break;
                                case "System.DateTime"://日期类型   
                                    DateTime dateV;
                                    DateTime.TryParse(dtSources[k].Rows[i][j].ToString(), out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle;//格式化显示   
                                    break;
                                case "System.Boolean"://布尔型   
                                    Boolean boolV = false;
                                    Boolean.TryParse(dtSources[k].Rows[i][j].ToString(), out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16"://整型   
                                case "System.Int32":
                                case "System.Int64":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                    Int32 intV = 0;
                                    Int32.TryParse(dtSources[k].Rows[i][j].ToString(), out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal"://浮点型   
                                case "System.Double":
                                    Double doubV = 0;
                                    Double.TryParse(dtSources[k].Rows[i][j].ToString(), out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull"://空值处理   
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }
                        }
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 导出EXCEL,可以导出多个sheet
        /// </summary>
        /// <param name="dtSources">原始数据表</param>
        /// <param name="NeedMargeColumns">需要合并的列，如：3,5（表示第三列和第五列各自合并单元格）</param>
        public static Byte[] ExportDT2Excel(DataTable[] dtSources, String NeedMargeColumns = "")
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            if (NeedMargeColumns.Length > 0)
            {
                NeedMargeColumns = "," + NeedMargeColumns + ",";
            }
            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            for (Int32 k = 0; k < dtSources.Length; k++)
            {
                Int32[] margeColIndex = new Int32[dtSources[k].Columns.Count];//保存合并列的起始行
                String[] margeColValue = new String[dtSources[k].Columns.Count];//保存合并列对应的上一个值      
                HSSFSheet sheet = workbook.CreateSheet(CharReplace(dtSources[k].TableName.ToString())) as HSSFSheet;
                //填充表头
                HSSFRow dataRow = sheet.CreateRow(0) as HSSFRow;
                ICellStyle Headstyle = workbook.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                Headstyle.Alignment = HorizontalAlignment.Center;
                //新建一个字体样式对象
                IFont font = workbook.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                Headstyle.SetFont(font);

                ICellStyle Rowstyle = workbook.CreateCellStyle();
                //设置单元格的样式：垂直对齐居中
                Rowstyle.VerticalAlignment = VerticalAlignment.Justify;

                //取得列宽
                Int32[] arrColWidth = new Int32[dtSources[k].Columns.Count];
                foreach (DataColumn item in dtSources[k].Columns)
                {
                    arrColWidth[item.Ordinal] = String.IsNullOrWhiteSpace(item.ColumnName) ? 1 : Encoding.UTF8.GetBytes(CharReplace(item.ColumnName.ToString())).Length;
                    margeColIndex[item.Ordinal] = 1;
                    margeColValue[item.Ordinal] = "";
                }
                for (Int32 i = 0; i < dtSources[k].Rows.Count; i++)
                {
                    for (Int32 j = 0; j < dtSources[k].Columns.Count; j++)
                    {
                        Int32 intTemp = Encoding.UTF8.GetBytes(dtSources[k].Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                foreach (DataColumn column in dtSources[k].Columns)
                {
                    ICell cell = dataRow.CreateCell(column.Ordinal);//创建单元格
                    cell.SetCellValue(CharReplace(column.ColumnName));//设置单元格的值
                    dataRow.GetCell(column.Ordinal).CellStyle = Headstyle; //将新的样式赋给单元格
                                                                           //设置列宽
                                                                           //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                    if (arrColWidth[column.Ordinal] > 255)
                    {
                        arrColWidth[column.Ordinal] = 254;
                    }
                    else
                    {
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                    }
                }
                //填充内容
                for (Int32 i = 0; i < dtSources[k].Rows.Count; i++)
                {
                    dataRow = sheet.CreateRow(i + 1) as HSSFRow;
                    for (Int32 j = 0; j < dtSources[k].Columns.Count; j++)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        Boolean canSetValue = false;
                        if (NeedMargeColumns.Length > 0 && (NeedMargeColumns.IndexOf("," + j.ToString() + ",") != -1)) //有需要合并的列,并且当前列需要合并              
                        {
                            if (margeColValue[j] == "")
                            {
                                canSetValue = true;
                                margeColIndex[j] = i + 1;
                                margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                            }
                            else if (margeColValue[j] != dtSources[k].Rows[i][j].ToString())//不同的时候需要更新原来储存的值，并重新设置合并
                            {
                                canSetValue = true;
                                //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                                sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i, j, j));
                                margeColIndex[j] = i + 1;
                                margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                                newCell.CellStyle = Rowstyle;
                            }
                            else if (i == dtSources[k].Rows.Count - 1 && i != 0)//最后一行强制合并
                            {
                                //值相同本来是不用做任何事，但是最后一行，则需要合并
                                sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i + 1, j, j));
                                newCell.CellStyle = Rowstyle;
                            }
                        }
                        else
                        {
                            canSetValue = true;
                        }
                        if (canSetValue)
                        {
                            //需要合并的列，1.第一行/最后一行,2.上一行值不同，下一行值相同，则给当前单元格赋值
                            switch (dtSources[k].Columns[j].DataType.ToString())
                            {
                                case "System.String"://字符串类型   
                                    newCell.SetCellValue(dtSources[k].Rows[i][j].ToString());
                                    break;
                                case "System.DateTime"://日期类型   
                                    DateTime dateV;
                                    DateTime.TryParse(dtSources[k].Rows[i][j].ToString(), out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle;//格式化显示   
                                    break;
                                case "System.Boolean"://布尔型   
                                    Boolean boolV = false;
                                    Boolean.TryParse(dtSources[k].Rows[i][j].ToString(), out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16"://整型   
                                case "System.Int32":
                                case "System.Int64":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                    Int32 intV = 0;
                                    Int32.TryParse(dtSources[k].Rows[i][j].ToString(), out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal"://浮点型   
                                case "System.Double":
                                    Double doubV = 0;
                                    Double.TryParse(dtSources[k].Rows[i][j].ToString(), out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull"://空值处理   
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }
                        }
                    }
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }


        /// <summary>
        /// 导出EXCEL,可以导出多个sheet
        /// </summary>
        /// <param name="dtSources">原始数据表</param>
        /// <param name="NeedMargeColumns">需要合并的列，如：3,5（表示第三列和第五列各自合并单元格）或者3,3:5（表示第三列合并，然后第五列合并时参考第三列再合并）</param>
        public static Byte[] ExportDT2ExcelExt(DataTable[] dtSources, String NeedMargeColumns)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            if (NeedMargeColumns.Length > 0)
            {
                NeedMargeColumns = "," + NeedMargeColumns + ",";
            }
            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            for (Int32 k = 0; k < dtSources.Length; k++)
            {
                Int32[] margeColIndex = new Int32[dtSources[k].Columns.Count];//保存合并列的起始行
                String[] margeColValue = new String[dtSources[k].Columns.Count];//保存合并列对应的上一个值      
                HSSFSheet sheet = workbook.CreateSheet(CharReplace(dtSources[k].TableName.ToString())) as HSSFSheet;
                //填充表头
                HSSFRow dataRow = sheet.CreateRow(0) as HSSFRow;
                ICellStyle Headstyle = workbook.CreateCellStyle();
                //设置单元格的样式：水平对齐居中
                Headstyle.Alignment = HorizontalAlignment.Center;
                //新建一个字体样式对象
                IFont font = workbook.CreateFont();
                //设置字体加粗样式
                font.Boldweight = short.MaxValue;
                //使用SetFont方法将字体样式添加到单元格样式中 
                Headstyle.SetFont(font);

                ICellStyle Rowstyle = workbook.CreateCellStyle();
                //设置单元格的样式：垂直对齐居中
                Rowstyle.VerticalAlignment = VerticalAlignment.Justify;

                //取得列宽
                Int32[] arrColWidth = new Int32[dtSources[k].Columns.Count];
                foreach (DataColumn item in dtSources[k].Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.UTF8.GetBytes(CharReplace(item.ColumnName.ToString())).Length;
                    margeColIndex[item.Ordinal] = 1;
                    margeColValue[item.Ordinal] = "";
                }
                for (Int32 i = 0; i < dtSources[k].Rows.Count; i++)
                {
                    for (Int32 j = 0; j < dtSources[k].Columns.Count; j++)
                    {
                        Int32 intTemp = Encoding.UTF8.GetBytes(dtSources[k].Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                foreach (DataColumn column in dtSources[k].Columns)
                {
                    ICell cell = dataRow.CreateCell(column.Ordinal);//创建单元格
                    cell.SetCellValue(CharReplace(column.ColumnName));//设置单元格的值
                    dataRow.GetCell(column.Ordinal).CellStyle = Headstyle; //将新的样式赋给单元格
                                                                           //设置列宽
                                                                           //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                    if (arrColWidth[column.Ordinal] >= 255)
                    {
                        arrColWidth[column.Ordinal] = 254;
                    }
                    else
                    {
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                    }
                }
                //填充内容
                for (Int32 i = 0; i < dtSources[k].Rows.Count; i++)
                {
                    dataRow = sheet.CreateRow(i + 1) as HSSFRow;
                    for (Int32 j = 0; j < dtSources[k].Columns.Count; j++)
                    {
                        ICell newCell = dataRow.CreateCell(j);
                        Boolean canSetValue = false;
                        if (NeedMargeColumns.Length > 0 && (NeedMargeColumns.IndexOf("," + j.ToString() + ",") != -1 || NeedMargeColumns.IndexOf(":" + j.ToString() + ",") != -1)) //有需要合并的列,并且当前列需要合并              
                        {
                            if (margeColValue[j] == "")
                            {
                                canSetValue = true;
                                margeColIndex[j] = i + 1;
                                margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                            }
                            else if (NeedMargeColumns.IndexOf("," + j.ToString() + ",") != -1)//单独列合并
                            {
                                if (margeColValue[j] != dtSources[k].Rows[i][j].ToString())//不同的时候需要更新原来储存的值，并重新设置合并
                                {
                                    canSetValue = true;
                                    //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                                    sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i, j, j));
                                    margeColIndex[j] = i + 1;
                                    margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                                    newCell.CellStyle = Rowstyle;
                                }
                                else if (i == dtSources[k].Rows.Count - 1 && i != 0)//最后一行强制合并
                                {
                                    //值相同本来是不用做任何事，但是最后一行，则需要合并
                                    sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i + 1, j, j));
                                    newCell.CellStyle = Rowstyle;
                                }
                            }
                            else//参考前列合并
                            {
                                //获取前列
                                Int32 refercol = 0;
                                String[] MargeCols = NeedMargeColumns.Split(',');
                                for (Int32 dd = 0; dd < MargeCols.Length; dd++)
                                {
                                    if (MargeCols[dd].Length > 0 && ("," + MargeCols[dd] + ",").IndexOf(":" + j.ToString() + ",") != -1)
                                    {
                                        refercol = Convert.ToInt32(MargeCols[dd].Substring(0, MargeCols[dd].IndexOf(":")));
                                        break;
                                    }
                                }
                                if (dtSources[k].Rows[i - 1][refercol].ToString() != dtSources[k].Rows[i][refercol].ToString())
                                {
                                    canSetValue = true;
                                    //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                                    sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i, j, j));
                                    margeColIndex[j] = i + 1;
                                    margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                                    newCell.CellStyle = Rowstyle;
                                }
                                else if (margeColValue[j] != dtSources[k].Rows[i][j].ToString())//不同的时候需要更新原来储存的值，并重新设置合并
                                {
                                    canSetValue = true;
                                    //CellRangeAddress四个参数为：起始行，结束行，起始列，结束列
                                    sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i, j, j));
                                    margeColIndex[j] = i + 1;
                                    margeColValue[j] = dtSources[k].Rows[i][j].ToString();
                                    newCell.CellStyle = Rowstyle;
                                }
                                else if (i == dtSources[k].Rows.Count - 1 && i != 0)//最后一行强制合并
                                {
                                    //值相同本来是不用做任何事，但是最后一行，则需要合并
                                    sheet.AddMergedRegion(new CellRangeAddress(margeColIndex[j], i + 1, j, j));
                                    newCell.CellStyle = Rowstyle;
                                }
                            }
                        }
                        else
                        {
                            canSetValue = true;
                        }
                        if (canSetValue)
                        {
                            #region 需要合并的列，1.第一行/最后一行,2.上一行值不同，下一行值相同，则给当前单元格赋值
                            switch (dtSources[k].Columns[j].DataType.ToString())
                            {
                                case "System.String"://字符串类型   
                                    newCell.SetCellValue(dtSources[k].Rows[i][j].ToString());
                                    break;
                                case "System.DateTime"://日期类型   
                                    DateTime dateV;
                                    DateTime.TryParse(dtSources[k].Rows[i][j].ToString(), out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle;//格式化显示   
                                    break;
                                case "System.Boolean"://布尔型   
                                    Boolean boolV = false;
                                    Boolean.TryParse(dtSources[k].Rows[i][j].ToString(), out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16"://整型   
                                case "System.Int32":
                                case "System.Int64":
                                case "System.UInt32":
                                case "System.UInt64":
                                case "System.Byte":
                                    Int32 intV = 0;
                                    Int32.TryParse(dtSources[k].Rows[i][j].ToString(), out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal"://浮点型   
                                case "System.Double":
                                    Double doubV = 0;
                                    Double.TryParse(dtSources[k].Rows[i][j].ToString(), out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull"://空值处理   
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }
                            #endregion
                        }
                    }
                }
            }
            //保存
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }
        #endregion

        /// <summary>
        /// 根据其他行的内容合并ISheet中某列相同信息的行（单元格）,2列相等
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="StartRow">合并的开始行</param>
        /// <param name="cols">合并列</param>
        /// <param name="precols">参考列</param>
        private static void GroupCol(ISheet sheet, Int32 StartRow, Int32 cols, Int32 precols)
        {
            if (sheet.PhysicalNumberOfRows < StartRow + 1 || cols > sheet.GetRow(0).PhysicalNumberOfCells - 1)
            {
                return;
            }
            Int32 RowNum = 0;
            ICell oldPreCell = sheet.GetRow(StartRow).GetCell(precols);
            ICell oldCell = sheet.GetRow(StartRow).GetCell(cols);
            Int32 i = StartRow + 1;
            while (i < sheet.PhysicalNumberOfRows)
            {

                ICell PreCell = sheet.GetRow(i).GetCell(precols);
                ICell Cell = sheet.GetRow(i).GetCell(cols);
                if (oldCell.ToString() == Cell.ToString() && oldPreCell.ToString() == PreCell.ToString())
                {
                    //Cell.SetCellValue("");
                    RowNum++;
                }
                else
                {
                    if (RowNum > 0)
                    {
                        CellRangeAddress region = new CellRangeAddress(StartRow, StartRow + RowNum, cols, cols);
                        sheet.AddMergedRegion(region);
                        StartRow += RowNum;
                    }
                    oldPreCell = PreCell;
                    oldCell = Cell;
                    StartRow += 1;
                    RowNum = 0;
                }
                i++;
            }
            if (RowNum > 0 && i == sheet.PhysicalNumberOfRows)
            {
                CellRangeAddress region = new CellRangeAddress(StartRow, StartRow + RowNum, cols, cols);
                sheet.AddMergedRegion(region);
                StartRow += RowNum;
            }
        }

        /// <summary>
        /// 自动设置Excel列宽，行高
        /// </summary>
        /// <param name="sheet">Excel表</param>
        private static void AutoSizeColumns(ISheet sheet)
        {
            Int32 RowNum = sheet.LastRowNum;
            Int32 ColumnNum = sheet.GetRow(0).LastCellNum;
            Int32 HeightValue = 0;
            for (Int32 i = 0; i < ColumnNum; i++)
            {
                Int32 columnWidth = sheet.GetColumnWidth(i) / 256;//获取当前列宽度  
                for (Int32 j = 1; j <= RowNum; j++)//在这一列上循环行  
                {
                    IRow currentRow = sheet.GetRow(j);
                    ICell currentCell = currentRow.GetCell(i);
                    Int32 length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;//获取当前单元格的内容宽度  
                    if (columnWidth < length + 1)
                        columnWidth = length + 1;
                    if (columnWidth > 40)
                        columnWidth = 40;

                    if (length > HeightValue) HeightValue = length;
                    currentRow.HeightInPoints = 20 * (HeightValue / 60 + 1);//高度自适应
                }
                sheet.SetColumnWidth(i, columnWidth * 256);
            }
        }

        private static void AutoSetType(ICell newCell, DataColumn column, DataRow row)
        {
            String drValue = row[column].ToString();
            switch (column.DataType.ToString())
            {
                case "System.String"://字符串类型
                                     //Double result;
                                     //if (Double.TryParse(drValue, out result) && result.ToString().Length != 15)
                                     //    newCell.SetCellValue(result);
                                     //else
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型   
                    DateTime dateV;
                    DateTime.TryParse(drValue, out dateV);
                    newCell.SetCellValue(dateV);
                    break;
                case "System.Boolean"://布尔型   
                    Boolean boolV = false;
                    Boolean.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型   
                case "System.Int32":
                case "System.Int64":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Byte":
                    Int32 intV = 0;
                    Int32.TryParse(drValue, out intV);
                    if (intV != 0)
                        newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型   
                case "System.Double":
                    Double doubV = 0;
                    Double.TryParse(drValue, out doubV);
                    if (doubV != 0)
                        newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理   
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        public static Byte[] ExportDT_2Title(DataTable dt_Title, DataTable dtSource, String SheetName, String ColumnProperty = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                HSSFWorkbook workbook = ExportWorkbook_2Title(dt_Title, dtSource, SheetName, ColumnProperty);
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }
        private static HSSFWorkbook ExportWorkbook_2Title(DataTable dt_Title, DataTable dtSource, String SheetName, String ColumnProperty = null)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            if (SheetName == null || SheetName == "") SheetName = "Sheet1";
            HSSFSheet sheet = workbook.CreateSheet(SheetName) as HSSFSheet;

            // 标题样式
            HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            headStyle.Alignment = HorizontalAlignment.Center;
            headStyle.VerticalAlignment = VerticalAlignment.Center;
            headStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            headStyle.WrapText = true;
            HSSFFont font = workbook.CreateFont() as HSSFFont;
            font.Boldweight = (short)FontBoldWeight.Bold;
            headStyle.SetFont(font);

            //内容样式
            HSSFCellStyle cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.CloneStyleFrom(headStyle);
            cellStyle.Alignment = HorizontalAlignment.Center;
            font = workbook.CreateFont() as HSSFFont;
            font.Boldweight = (short)FontBoldWeight.Normal;
            cellStyle.SetFont(font);
            HSSFCellStyle cellStyleLeft = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyleLeft.CloneStyleFrom(cellStyle);
            cellStyleLeft.Alignment = HorizontalAlignment.Left;
            HSSFCellStyle cellStyleRight = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyleRight.CloneStyleFrom(cellStyle);
            cellStyleRight.Alignment = HorizontalAlignment.Right;

            Int32 rowIndex = 0;
            #region 列头及样式

            for (Int32 i = 0; i < dt_Title.Rows.Count; i++)
            {
                HSSFRow headerRow = sheet.CreateRow(rowIndex) as HSSFRow;
                for (Int32 j = 0; j < dt_Title.Columns.Count; j++)
                {
                    headerRow.CreateCell(dt_Title.Columns[j].Ordinal).SetCellValue(dt_Title.Rows[i][j].ToString());
                    headerRow.GetCell(dt_Title.Columns[j].Ordinal).CellStyle = headStyle;

                }
                rowIndex++;
            }
            for (Int32 i = 0; i < dt_Title.Rows.Count; i++)
            {
                for (Int32 m = 0; m < dt_Title.Columns.Count; m++)
                {
                    Int32 n = m + 1;
                    while (n < dt_Title.Columns.Count)
                    {
                        if (dt_Title.Rows[i][m].ToString() == dt_Title.Rows[i][n].ToString())
                        {
                            n++;
                        }
                        else if (n == dt_Title.Columns.Count)
                        {
                            break;
                        }
                        else
                        {
                            //合并同一行相同的列表头
                            sheet.AddMergedRegion(new CellRangeAddress(i, i, m, n - 1));
                            m = n - 1;
                            break;
                        }
                    }
                    if (n == dt_Title.Columns.Count)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(i, i, m, n - 1));

                    }
                }

            }

            for (Int32 i = 0; i < dt_Title.Columns.Count; i++)
            {
                for (Int32 m = 0; m < dt_Title.Rows.Count; m++)
                {
                    Int32 n = m + 1;
                    while (n < dt_Title.Rows.Count)
                    {
                        if (dt_Title.Rows[m][i].ToString() == dt_Title.Rows[n][i].ToString())
                        {
                            n++;
                        }
                        else if (n == dt_Title.Rows.Count)
                        {
                            break;
                        }
                        else
                        {
                            //合并同一列相同的行表头
                            sheet.AddMergedRegion(new CellRangeAddress(m, n - 1, i, i));
                            m = n - 1;
                            break;
                        }
                    }
                    if (n == dt_Title.Rows.Count)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(m, n - 1, i, i));

                    }
                }
            }

            #endregion

            foreach (DataRow row in dtSource.Rows)
            {
                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                    newCell.CellStyle = cellStyle;
                    AutoSetType(newCell, column, row);
                }

                #endregion
                rowIndex++;
            }
            AutoSizeColumns(sheet);

            if (ColumnProperty != null)
            {
                String[] ColumnArray = ColumnProperty.Split('|');
                foreach (String Column in ColumnArray)
                {
                    String[] ValueArray = Column.Split(',');
                    if (ValueArray.Length > 2)
                    {
                        Int32 cols = Int32.Parse(ValueArray[1]);
                        switch (ValueArray[0].ToString())
                        {
                            case "Merge":
                                Int32 precols = 0;
                                Int32.TryParse(ValueArray[2], out precols);
                                GroupCol(sheet, 1, cols, precols);
                                break;
                            case "Width":
                                Int32 columnWidth = 0;
                                Int32.TryParse(ValueArray[2], out columnWidth);
                                sheet.SetColumnWidth(cols, columnWidth * 256);
                                break;
                            case "Align":
                                String Value = ValueArray[2].ToString();
                                for (Int32 j = 1; j <= sheet.LastRowNum; j++)
                                {
                                    ICell currentCell = sheet.GetRow(j).GetCell(cols);
                                    switch (Value)
                                    {
                                        case "center":
                                            currentCell.CellStyle = cellStyle;
                                            break;
                                        case "left":
                                            currentCell.CellStyle = cellStyleLeft;
                                            break;
                                        case "right":
                                            currentCell.CellStyle = cellStyleRight;
                                            break;
                                    }
                                }
                                break;
                            case "DataType":
                                String Value2 = ValueArray[2].ToString();
                                for (Int32 j = 1; j <= sheet.LastRowNum; j++)
                                {
                                    ICell currentCell = sheet.GetRow(j).GetCell(cols);
                                    String drValue = currentCell.ToString();
                                    switch (Value2)
                                    {
                                        case "Double":
                                            Double result;
                                            if (Double.TryParse(drValue, out result))
                                                currentCell.SetCellValue(result);
                                            break;
                                        case "String":
                                            currentCell.SetCellValue(drValue.ToString());
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            return workbook;
        }
    }
}