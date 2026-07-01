using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Updater.Base;

namespace ScopeX.Updater.PackageGenerator
{
    public partial class FormGenerator : Form
    {
        public HardwareVersionInfo packageVersion = new() { Major = 2, Minor = 5, Build = 5, Revision = 0, LastComment = "7000l更新支持", LastModifier = "ljw", LastBuildDateTime = new DateTime(2024, 2, 20, 16, 0, 0) };
        public FormGenerator()
        {
            InitializeComponent();
        }
        private Dictionary<ProductDefine, List<ItemTypeDefine>> AllProductItemDefines;
        private List<ItemTypeDefine> currProductItemTypes;
        private void buttonSetSaveFileName_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "升级包(*.UPD)|*.UPD";
            saveFileDialog1.FileName = $"{comboBoxProductList.Text.Trim()}_UpdatePackage_{DateTime.Now:yyMMdd_HHmmss}.UPD";
            if (this.saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            if (Path.GetExtension(this.saveFileDialog1.FileName).ToUpper() != ".UPD")
                textBoxSaveFileName.Text = this.saveFileDialog1.FileName + @".upd";
            else
                textBoxSaveFileName.Text = this.saveFileDialog1.FileName;
        }
        private enum FileGridColumns
        {
            ItemName = 0,
            BrowserButton = 1,
            FilePathAndName = 2,
            Version = 3,
            FirmwareType = 4,

            MinDevVer = 5,
            Comment = 6,
            FileCreateDatetime = 7,
            FileSize = 8,
            //FileRelatedPath = 10,
            DeleteButton = 9
        }
        private readonly int GenMcsFile2DataFileThreadCount = 10;
        private byte[] GenMcsFile2DataFile(string DataFileName)
        {
            string targetFileName = DataFileName + ".dat";
            if (File.Exists(targetFileName))
                File.Delete(targetFileName);
            BinaryWriter binWriter = new BinaryWriter(new FileStream(targetFileName, FileMode.CreateNew));

            int i, j;
            RichTextBox richTextBoxTmp = new RichTextBox();
            richTextBoxTmp.Clear();
            richTextBoxTmp.LoadFile(DataFileName, RichTextBoxStreamType.PlainText);

            int lineCount = richTextBoxTmp.Lines.Length;

            string[] allLines = richTextBoxTmp.Lines;
            int perTheardLineCount = (lineCount / GenMcsFile2DataFileThreadCount);
            int bytes;

            LineConverter[] converters = new LineConverter[GenMcsFile2DataFileThreadCount];
            for (i = 0; i < GenMcsFile2DataFileThreadCount - 1; i++)
            {
                converters[i] = new LineConverter();
                converters[i].lineCount = perTheardLineCount;
                converters[i].allLines = new string[perTheardLineCount];
                converters[i].startLineNo = i * perTheardLineCount;
                bytes = 0;
                for (j = 0; j < perTheardLineCount; j++)
                {
                    bytes += 16;
                    converters[i].allLines[j] = (allLines[i * perTheardLineCount + j]);
                }
                converters[i].ByteCount = bytes + 16;
            }
            converters[GenMcsFile2DataFileThreadCount - 1] = new LineConverter();
            converters[GenMcsFile2DataFileThreadCount - 1].lineCount = lineCount - perTheardLineCount * (GenMcsFile2DataFileThreadCount - 1);
            converters[GenMcsFile2DataFileThreadCount - 1].allLines = new string[lineCount - perTheardLineCount * (GenMcsFile2DataFileThreadCount - 1)];
            converters[GenMcsFile2DataFileThreadCount - 1].startLineNo = perTheardLineCount * (GenMcsFile2DataFileThreadCount - 1);
            bytes = 0;
            j = 0;
            for (i = perTheardLineCount * (GenMcsFile2DataFileThreadCount - 1); i < lineCount; i++)
            {
                bytes += 16;
                converters[GenMcsFile2DataFileThreadCount - 1].allLines[j++] = (allLines[i]);
            }
            converters[GenMcsFile2DataFileThreadCount - 1].ByteCount = bytes + 16;

            Thread[] threads = new Thread[GenMcsFile2DataFileThreadCount];
            for (i = 0; i < GenMcsFile2DataFileThreadCount; i++)
            {
                threads[i] = new Thread(new ThreadStart(converters[i].ExecConvert));
                threads[i].Start();
            }
            int[] states = new int[GenMcsFile2DataFileThreadCount];
            for (i = 0; i < GenMcsFile2DataFileThreadCount; i++)
                states[i] = 0;
            for (i = 0; i < GenMcsFile2DataFileThreadCount; i++)
            {
                do
                {
                    Application.DoEvents();
                } while (converters[i].state == 0);
            }
            for (i = 0; i < GenMcsFile2DataFileThreadCount; i++)
            {
                for (j = 0; j < converters[i].actualByteCount; j++)
                    binWriter.Write(converters[i].byteContent[j]);
                threads[i].Join();
            }

            binWriter.Flush();
            binWriter.Close();
            return File.ReadAllBytes(DataFileName + ".dat");
        }
        private bool CheckFpgaIDCode(string idCode, byte[] content)
        {
            uint nullCount = 0;
            for (; nullCount < 1024; nullCount++)
            {
                if (content[nullCount] != 0xFF)
                {
                    break;
                }
            }
            //对齐
            if (nullCount > 0 && nullCount % 2 != 0)
            {
                nullCount--;
            }
            uint searchHeaderBytes = nullCount + 256;//通过观察得到的，并没有找到文档支持 ljw 
            string HeaderStr = "";
            for (uint i = nullCount; i < searchHeaderBytes; i++)
                HeaderStr += $"_{content[i].ToString("X").PadLeft(2, '0')}";
            return HeaderStr.IndexOf(idCode) > 0;
        }
        private void buttonGeneratePackage_Click(object sender, EventArgs e)
        {
            if (textBoxSaveFileName.Text.Trim() == "")
            {
                progressBar1.Visible = false;
                MessageBox.Show("请设置保存文件的路径及名称！");
                return;
            }
            #region Check
            progressBar1.Visible = true;

            int rowCount = dataGridViewUpdatePackageFiles.RowCount;
            int itemCount = 0;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow row = dataGridViewUpdatePackageFiles.Rows[rowIndex];

                if (string.IsNullOrWhiteSpace($"{row.Cells[(int)FileGridColumns.ItemName].Value}"))
                {
                    if (rowIndex == rowCount - 1)
                        break;
                    else
                    {
                        row.Selected = true;
                        progressBar1.Visible = false;

                        MessageBox.Show("当前行的信息不完整，不能生成！");
                        return;
                    }
                }
                if (row.Cells[(int)FileGridColumns.FilePathAndName].Value.ToString() == "")
                {
                    row.Selected = true;
                    progressBar1.Visible = false;

                    MessageBox.Show("当前行的文件没有确定，不能生成！");
                    return;
                }

                #region 版本输入检查
                bool versionCheck = !string.IsNullOrWhiteSpace($"{row.Cells[(int)FileGridColumns.Version].Value}");
                if (!versionCheck)
                {
                    progressBar1.Visible = false;

                    row.Selected = true;
                    MessageBox.Show("当前行的版本号异常 (格式\"1.0.0\"或\"1.0.0.0\")，不能生成！");
                    return;
                }
                var versionStr = $"{row.Cells[(int)FileGridColumns.Version].Value}";
                versionCheck &= !string.IsNullOrWhiteSpace(versionStr);
                Regex versionRegex = new Regex(@"((\d)+\.){2,4}(\d)");
                versionCheck &= !versionRegex.IsMatch(versionStr);

                if (versionCheck)
                {
                    row.Selected = true;
                    progressBar1.Visible = false;

                    MessageBox.Show("当前行的版本号异常 (格式\"1.0.0\"或\"1.0.0.0\")，不能生成！");
                    return;
                }
                #endregion  版本输入检查

                ItemTypeDefine currItemTypeDefine = getCurrentSelectItemTypeDefine(row.Cells[(int)FileGridColumns.ItemName].Value.ToString());
                if (currItemTypeDefine.Type == UpdaterItemType.Software)
                {
                    if (!Directory.Exists(textBoxSoftwareRelatedPath.Text))
                    {
                        progressBar1.Visible = false;

                        row.Selected = true;
                        MessageBox.Show("当前行的软件保存相对路径不正确！请设置正确后再生成！");
                        return;
                    }
                }
                else if (currItemTypeDefine.Type == UpdaterItemType.Fpga)
                {
                    var fileExt = Path.GetExtension(row.Cells[(int)FileGridColumns.FilePathAndName].Value.ToString()).ToUpper();
                    if (fileExt != ".MCS" && fileExt != ".BIN")
                    {
                        progressBar1.Visible = false;

                        row.Selected = true;
                        MessageBox.Show("当前行的选中的文件与类型不匹配！请设置正确后再生成！");
                        return;
                    }
                    if (row.Cells[(int)FileGridColumns.FirmwareType].Value.ToString() == "无效")
                    {
                        progressBar1.Visible = false;

                        row.Selected = true;
                        MessageBox.Show("固件类型未选择或不匹配！请设置正确后再生成！");
                        return;
                    }
                }
                itemCount++;
            }
            if (itemCount == 0)
            {
                progressBar1.Visible = false;

                MessageBox.Show("请先设置需要更新的内容！");
                return;
            }
            #endregion Check

            #region Generate

            UpdatePackage.Default.Items.Clear();

            progressBar1.Visible = true;
            progressBar1.Value = 1;
            progressBar1.Update();
            progressBar1.Maximum = itemCount + 1 + 1;
            int processItemCount = 0;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow row = dataGridViewUpdatePackageFiles.Rows[rowIndex];
                if ((row.Cells[(int)FileGridColumns.ItemName].Value == null) && (rowIndex == rowCount - 1))
                {
                    break;
                }
                ItemTypeDefine currItemTypeDefine = getCurrentSelectItemTypeDefine(row.Cells[(int)FileGridColumns.ItemName].Value.ToString());
                string fileName = row.Cells[(int)FileGridColumns.FilePathAndName].Value.ToString();
                var fileExt = Path.GetExtension(fileName).ToUpper();
                UpdateItem updateItem;
                string versionStr = $"{row.Cells[(int)FileGridColumns.Version].Value}";
                string firmwareType = $"{row.Cells[(int)FileGridColumns.FirmwareType].Value}";
                string comment = $"{row.Cells[(int)FileGridColumns.Comment].Value}";
                string minDevVer = $"{row.Cells[(int)FileGridColumns.MinDevVer].Value}";

                string boardName = $"{currItemTypeDefine.BoradName}";

                BaseDataBlock info;
                var fileData = File.GetLastWriteTime(fileName);

                if (currItemTypeDefine.Type == UpdaterItemType.Fpga)
                {
                    byte[]? fpgaContent;
                    if (fileExt.ToUpper() == ".MCS")
                        fpgaContent = GenMcsFile2DataFile(fileName);
                    else
                        fpgaContent = File.ReadAllBytes(fileName);
                    #region 检查IDCODE
                    if (!CheckFpgaIDCode(currItemTypeDefine.IDCodeVerify, fpgaContent))
                    {
                        progressBar1.Visible = false;

                        MessageBox.Show("固件与产品型号载板不匹配！");
                        return;
                    }
                    #endregion  检查IDCODE

                    //todo
                    info = new ImageBlock()
                    {
                        BoardID = (uint)currItemTypeDefine.TypeID,
                        Date = fileData,
                        Remarks = comment,
                        SizeBytes = (uint)fpgaContent.Length,
                    };
                    ((ImageBlock)info).Version = UpdateBaseHelper.GetVersionFromStr(versionStr);
                    ((ImageBlock)info).RequiredDriveVersion = UpdateBaseHelper.GetVersionFromStr(minDevVer);
                    switch (firmwareType)
                    {
                        default:
                            ((ImageBlock)info).FirmwareType = Base.FirmwareType.None;
                            break;
                        case "Golden":
                            ((ImageBlock)info).FirmwareType = Base.FirmwareType.GoldenImage;
                            break;
                        case "Application":
                            ((ImageBlock)info).FirmwareType = Base.FirmwareType.AppImage;
                            break;
                    }

                    updateItem = new UpdateItem(currItemTypeDefine.TypeID, currItemTypeDefine.Type,
                        "",
                        fileData,
                        fpgaContent,
                        info,
                        boardName
                        );

                }
                else if (currItemTypeDefine.Type == UpdaterItemType.Mcu_AnalogChannel || currItemTypeDefine.Type == UpdaterItemType.Mcu_Keyboard)
                {
                    byte[]? appContent = File.ReadAllBytes(fileName);
                    //todo
                    info = new ImageBlock()
                    {
                        BoardID = (uint)currItemTypeDefine.TypeID,
                        Date = fileData,
                        Remarks = comment,
                        SizeBytes = (uint)appContent.Length,
                    };
                    ((ImageBlock)info).Version = UpdateBaseHelper.GetVersionFromStr(versionStr);
                    ((ImageBlock)info).RequiredDriveVersion = UpdateBaseHelper.GetVersionFromStr(minDevVer);
                    ((ImageBlock)info).FirmwareType = Base.FirmwareType.None;

                    updateItem = new UpdateItem(currItemTypeDefine.TypeID, currItemTypeDefine.Type,
                        "",
                        fileData,
                        appContent,
                        info,
                        boardName
                        );

                }
                else if (currItemTypeDefine.Type is UpdaterItemType.Probe or UpdaterItemType.Probe)
                {
                    byte[]? appContent = File.ReadAllBytes(fileName);
                    //todo
                    info = new ImageBlock()
                    {
                        BoardID = (uint)currItemTypeDefine.TypeID,
                        Date = fileData,
                        Remarks = comment,
                        SizeBytes = (uint)appContent.Length,
                    };
                    ((ImageBlock)info).Version = UpdateBaseHelper.GetVersionFromStr(versionStr);
                    ((ImageBlock)info).RequiredDriveVersion = UpdateBaseHelper.GetVersionFromStr(minDevVer);
                    ((ImageBlock)info).FirmwareType = Base.FirmwareType.None;

                    updateItem = new UpdateItem(currItemTypeDefine.TypeID, currItemTypeDefine.Type,
                        "",
                        fileData,
                        appContent,
                        info,
                        boardName
                    );

                }
                else
                {
                    //todo 后期选件、软件再扩展类型
                    info = new BaseDataBlock()
                    {
                        BoardID = (uint)currItemTypeDefine.TypeID,
                        Date = fileData,
                        Remarks = comment,
                    };
                    updateItem = new UpdateItem(currItemTypeDefine.TypeID, currItemTypeDefine.Type,
                         Path.GetFileName(fileName),
                         File.GetCreationTime(fileName),
                         File.ReadAllBytes(fileName),
                         info,
                         boardName
                         );
                }
                //if (currItemTypeDefine.bIsFPGA)
                //{
                //    haveFPGA = true;
                //}

                UpdatePackage.Default.Items.Add(updateItem);
                processItemCount++;
                progressBar1.Value = processItemCount;
            }
            //if (haveFPGA)
            //{
            //    string hardwareDriverDllFileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + currProductDefine.FPGA_Action_DllFileName;
            //    if (File.Exists(hardwareDriverDllFileName))
            //    {
            //        UpdatePackage.Default.HardwareDriver = File.ReadAllBytes(hardwareDriverDllFileName);
            //    }
            //}

            UpdatePackage.Default.Save(this.textBoxSaveFileName.Text.Trim(),
                (ProductType)Enum.Parse(typeof(ProductType),
                $"{comboBoxProductList.Text}"),
                packageVersion
                );
            progressBar1.Value = progressBar1.Maximum;
            #endregion Generate
            MessageBox.Show("生成成功！文件保存在[" + this.textBoxSaveFileName.Text.Trim() + "]。");
            progressBar1.Visible = false;
        }
        private string getVersionStr(HardwareVersionInfo version)
        {
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
        private void FormGenerator_Load(object sender, EventArgs e)
        {
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            Text += $" V{fileInfo.FileVersion}";
            //Text += $" V{getVersionStr(packageVersion)}";
            openFileDialog1.FileName = "";
            progressBar1.Visible = false;
            string configFileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".dll.config";
            AllProductItemDefines = ItemTypeDefine.Load(configFileName);
            foreach (KeyValuePair<ProductDefine, List<ItemTypeDefine>> product in AllProductItemDefines)
            {
                comboBoxProductList.Items.Add(product.Key.Name);
            }
        }
        ProductDefine currProductDefine = null;
        private void comboBoxProductList_SelectedIndexChanged(object sender, EventArgs e)
        {
            currProductItemTypes = null;
            dataGridViewUpdatePackageFiles.Rows.Clear();
            foreach (KeyValuePair<ProductDefine, List<ItemTypeDefine>> product in AllProductItemDefines)
            {
                if (product.Key.Name == comboBoxProductList.SelectedItem.ToString())
                {
                    currProductItemTypes = product.Value;
                    currProductDefine = product.Key;
                    break;
                }
            }
            if (currProductItemTypes == null)
                return;
            DataGridViewComboBoxColumn partTypeColumn = dataGridViewUpdatePackageFiles.Columns[(int)FileGridColumns.ItemName] as DataGridViewComboBoxColumn;
            partTypeColumn.Items.Clear();
            foreach (ItemTypeDefine itemTypeDefine in currProductItemTypes)
            {
                partTypeColumn.Items.Add(itemTypeDefine.ItemName);
            }
            dataGridViewUpdatePackageFiles.RowCount = 1;
            SetEmptyRow(0);
        }
        private ItemTypeDefine getCurrentSelectItemTypeDefine(string ItemName)
        {
            ItemTypeDefine currItemTypeDefine = null;
            foreach (ItemTypeDefine itemTypeDefine in currProductItemTypes)
            {
                if (itemTypeDefine.ItemName == ItemName)
                {
                    currItemTypeDefine = itemTypeDefine;
                    break;
                }
            }
            return currItemTypeDefine;
        }
        private string GetLastModifiedDatetime(string fileName)
        {
            DateTime dateTime = File.GetLastWriteTime(fileName);
            return string.Format("{0}.{1:d2}.{2:d2} {3:d2}:{4:d2}:{5:d2}", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
        bool bClearSelectFile = true;
        private string getRelatedFilePath(string fileName)
        {
            string result = "";
            string path = Path.GetDirectoryName(fileName);
            if (textBoxSoftwareRelatedPath.Text == path)
                result = @".\";
            else
            {
                if (textBoxSoftwareRelatedPath.Text[textBoxSoftwareRelatedPath.Text.Length - 1] == '\\')
                    result = @".\" + path.Substring(textBoxSoftwareRelatedPath.Text.Length);
                else
                    result = @"." + path.Substring(textBoxSoftwareRelatedPath.Text.Length);
            }
            if (result[result.Length - 1] != '\\')
                result += @"\";
            return result;
        }
        private void SetEmptyRow(int rowIndex)
        {
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FilePathAndName].Value = "";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.BrowserButton].Value = "浏览";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.DeleteButton].Value = "删除";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FileSize].Value = "";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FirmwareType].Value = "无效";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.MinDevVer].Value = "0.0.0";
            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.Version].Value = "";

            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FileCreateDatetime].Value = "";

        }
        private void OnSelectFile(DataGridViewCellEventArgs e, ItemTypeDefine currItemTypeDefine)
        {
            string fileFilter;
            if (currItemTypeDefine.Type == UpdaterItemType.Fpga)
            {
                fileFilter = $"{currItemTypeDefine.ItemName}|*.bin;*.mcs|{currItemTypeDefine.ItemName}|*.bin|{currItemTypeDefine.ItemName}|*.mcs";
                openFileDialog1.Multiselect = false;
            }
            else if (currItemTypeDefine.Type == UpdaterItemType.Mcu_AnalogChannel)
            {
                fileFilter = $"{currItemTypeDefine.ItemName}|*.bin;{currItemTypeDefine.ItemName}";
                openFileDialog1.Multiselect = false;
            }
            else if (currItemTypeDefine.Type == UpdaterItemType.Mcu_Keyboard)
            {
                fileFilter = $"{currItemTypeDefine.ItemName}|*.bin;{currItemTypeDefine.ItemName}";
                openFileDialog1.Multiselect = false;
            }
            else
            {
                if (textBoxSoftwareRelatedPath.Text.Trim() == "")
                {
                    MessageBox.Show("请先设置软件的相对路径！");
                    return;
                }
                fileFilter = currItemTypeDefine.ItemName + "|*.*";
                if (textBoxSoftwareRelatedPath.Text.Trim() != "")
                    openFileDialog1.InitialDirectory = textBoxSoftwareRelatedPath.Text;
                openFileDialog1.Multiselect = true;
            }
            openFileDialog1.Filter = fileFilter;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            bool bIsLastRow = (e.RowIndex == dataGridViewUpdatePackageFiles.RowCount - 1);
            int beginRowIndex = e.RowIndex;
            string nowType = dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.ItemName].Value.ToString();
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.FilePathAndName].Value = openFileDialog1.FileNames[0];
            FileInfo fileInfo = new FileInfo(openFileDialog1.FileNames[0]);
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.FileSize].Value = fileInfo.Length;
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.Version].Value = "1.0.0";
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.FileCreateDatetime].Value = GetLastModifiedDatetime(openFileDialog1.FileNames[0]);

            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.Comment].Value = "";
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.FirmwareType].Value = "Application";
            dataGridViewUpdatePackageFiles.Rows[beginRowIndex].Cells[(int)FileGridColumns.MinDevVer].Value = "0.0.0";

            for (int fileIndex = 1; fileIndex < openFileDialog1.FileNames.Length; fileIndex++)
            {
                string fileName = openFileDialog1.FileNames[fileIndex];
                long fileSize = (new FileInfo(fileName)).Length;
                string fileCreatetime = GetLastModifiedDatetime(fileName);
                string fileRelatedPath = getRelatedFilePath(fileName);
                DataGridViewRow newRow = new DataGridViewRow();

                newRow.CreateCells(dataGridViewUpdatePackageFiles);
                newRow.Cells[(int)FileGridColumns.ItemName].Value = nowType;

                newRow.Cells[(int)FileGridColumns.Version].Value = "1.0.0";

                newRow.Cells[(int)FileGridColumns.BrowserButton].Value = "浏览";
                newRow.Cells[(int)FileGridColumns.FilePathAndName].Value = fileName;
                newRow.Cells[(int)FileGridColumns.FileSize].Value = fileSize.ToString();

                newRow.Cells[(int)FileGridColumns.FileCreateDatetime].Value = fileCreatetime;
                newRow.Cells[(int)FileGridColumns.DeleteButton].Value = "删除";

                newRow.Cells[(int)FileGridColumns.Comment].Value = "";
                newRow.Cells[(int)FileGridColumns.FirmwareType].Value = "无效";
                newRow.Cells[(int)FileGridColumns.MinDevVer].Value = "0.0.0";


                dataGridViewUpdatePackageFiles.Rows.Insert(beginRowIndex, newRow);
            }
            if (bIsLastRow)
            {
                //add new row
                dataGridViewUpdatePackageFiles.RowCount = dataGridViewUpdatePackageFiles.RowCount + 1;
                SetEmptyRow(dataGridViewUpdatePackageFiles.RowCount - 1);
            }
        }
        private void dataGridViewUpdatePackageFiles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (currProductItemTypes == null)
                return;
            if (e.RowIndex < 0)
                return;
            if (dataGridViewUpdatePackageFiles.Rows[e.RowIndex].Cells[(int)FileGridColumns.ItemName].Value == null)
                return;
            if ((e.ColumnIndex != (int)FileGridColumns.BrowserButton) && (e.ColumnIndex == (int)FileGridColumns.BrowserButton))
                return;

            ItemTypeDefine currItemTypeDefine = getCurrentSelectItemTypeDefine(dataGridViewUpdatePackageFiles.Rows[e.RowIndex].Cells[(int)FileGridColumns.ItemName].Value.ToString());
            if (currItemTypeDefine == null)
                return;
            bClearSelectFile = false;
            if (e.ColumnIndex == (int)FileGridColumns.BrowserButton)
            {
                OnSelectFile(e, currItemTypeDefine);
            }
            else if (e.ColumnIndex == (int)FileGridColumns.DeleteButton)
            {
                int currRowIndex = 1;
                if (dataGridViewUpdatePackageFiles.RowCount > 1)
                {
                    currRowIndex = e.RowIndex;
                    if (e.RowIndex == dataGridViewUpdatePackageFiles.RowCount - 1)
                    {
                        currRowIndex = dataGridViewUpdatePackageFiles.RowCount - 1;
                        dataGridViewUpdatePackageFiles.RowCount = dataGridViewUpdatePackageFiles.RowCount + 1;
                        SetEmptyRow(dataGridViewUpdatePackageFiles.RowCount - 1);
                    }
                    else
                    {
                        for (int rowIndex = currRowIndex; rowIndex < dataGridViewUpdatePackageFiles.RowCount - 1; rowIndex++)
                        {
                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.ItemName].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.ItemName].Value;
                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FilePathAndName].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.FilePathAndName].Value;
                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.BrowserButton].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.BrowserButton].Value;
                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.DeleteButton].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.DeleteButton].Value;
                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FileSize].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.FileSize].Value;

                            dataGridViewUpdatePackageFiles.Rows[rowIndex].Cells[(int)FileGridColumns.FileCreateDatetime].Value = dataGridViewUpdatePackageFiles.Rows[rowIndex + 1].Cells[(int)FileGridColumns.FileCreateDatetime].Value;
                        }
                        dataGridViewUpdatePackageFiles.RowCount = dataGridViewUpdatePackageFiles.RowCount - 1;
                    }
                }
                else
                {
                    SetEmptyRow(0);
                }
            }
            bClearSelectFile = true;
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            dataGridViewUpdatePackageFiles.RowCount = 1;
            SetEmptyRow(0);
        }

        private void dataGridViewUpdatePackageFiles_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!bClearSelectFile)
                return;
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == (int)FileGridColumns.ItemName)
            {
                SetEmptyRow(e.RowIndex);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSetSoftwareRelatedPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialogSetRelatedPath.ShowDialog() == DialogResult.OK)
            {
                textBoxSoftwareRelatedPath.Text = folderBrowserDialogSetRelatedPath.SelectedPath;
            }
        }

        private void dataGridViewUpdatePackageFiles_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }
    }
}
