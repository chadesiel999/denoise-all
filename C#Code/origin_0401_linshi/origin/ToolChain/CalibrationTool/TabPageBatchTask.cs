using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Tool.Utilities;
using ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageBatchTask : UserControl, IMainFormTabPage
    {
        public record ReportInfo(String CurrXmlFileName, String FuncName, String StepInfo, BatchTaskPartResult Result, String WholeInfo);
        private List<ReportInfo> _ReportInfos = new List<ReportInfo>();
        public TabPageBatchTask()
        {
            InitializeComponent();
            this.LvStaticInfo.DrawColumnHeader += LvStaticInfo_DrawColumnHeader;
            this.LvStaticInfo.DrawSubItem += LvStaticInfo_DrawSubItem;
            Init();
            LoadAllTaskPart();

            ControlMsgFilter.Instance.RegistProc(LvStaticInfo.Handle, m =>
            {
                if (m.Msg == 0x0201 /*WM_LBUTTONDOWN*/)
                {
                    unsafe
                    {
                        int intptrBitSize = 32;
                        int lowMask = (int)(Math.Pow(2, intptrBitSize / 2) - 1);

                        int x = (int)m.LParam & lowMask;
                        int y = (int)m.LParam >> (intptrBitSize / 2);
                        LvStaticInfo_MouseClick(LvStaticInfo, new MouseEventArgs(MouseButtons.Left, 1, x, y, 1));
                    }
                    return true;
                }
                return false;
            });
            //_ReportInfos.Add(new ReportInfo("file1.txt", "func1", "step1", BatchTaskPartResult.Succeed, "info1"));
            //_ReportInfos.Add(new ReportInfo("file1.txt", "func12", "step12", BatchTaskPartResult.ErrorGeneral, "info12"));
            //_ReportInfos.Add(new ReportInfo("file1.txt", "func13", "step13", BatchTaskPartResult.ErrorGeneral, "info13"));
            //_ReportInfos.Add(new ReportInfo("file2.txt", "func21", "step21", BatchTaskPartResult.Succeed, "info21"));
            //_ReportInfos.Add(new ReportInfo("file2.txt", "func22", "step22", BatchTaskPartResult.ErrorGeneral, "info22"));
            //RefreshLvStaticInfo();
        }

        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private void LoadAllTaskPart()
        {
            comboBoxTaskParts.Items.Clear();
            if (BaseHelper.AllBatchTaskPartClass != null)
            {
                foreach (Type type in BaseHelper.AllBatchTaskPartClass)
                    comboBoxTaskParts.Items.Add(type.Name);
                comboBoxTaskParts.SelectedIndex = 0;
            }
        }
        //ScopeX.Hardware.Calibration.Tool.BatchTaskProcessor.config 配置示例
        //        [
        //{"Title":"这是一个测试","ClassName":"BatchTask_Demo","TipMessage":"","Description":"","Parameters":" "},

        //{"Title":"通道增益校准","ClassName":"BatchTask_ProcessXMLAndPartTask","TipMessage":"通道增益校准","Description":"同时校准两个通道。","Parameters":" "},
        //{ "Title":"通道校准检查","ClassName":"BatchTask_ProcessXMLAndPartTask","TipMessage":"通道基线及增益检查","Description":"同时对两个通道基线及增益检查，并生成Excel结果。","Parameters":" "},
        //{ "Title":"幅频特性检查","ClassName":"BatchTask_ProcessXMLAndPartTask","TipMessage":"幅频特性检查","Description":"同时对两个通道的幅频特性进行检查，并生成Excel结果。","Parameters":" "},

        //{ "Title":"扫频-Ch1","ClassName":"BatchTask_ScanFrequency","TipMessage":"","Description":"","Parameters":"USB0::0x0AAD::0x0054::181629::INSTR,Ch1"},
        //{ "Title":"扫频-Ch2","ClassName":"BatchTask_ScanFrequency","TipMessage":"","Description":"","Parameters":"USB0::0x0AAD::0x0054::181629::INSTR,Ch2"},

        //{ "Title":"TiADC校准-Ch1","ClassName":"BatchTask_AutoCaliTiAdc","TipMessage":"","Description":"","Parameters":"USB0::0x0AAD::0x0054::181629::INSTR,Ch1"},
        //{ "Title":"TiADC校准-Ch2","ClassName":"BatchTask_AutoCaliTiAdc","TipMessage":"","Description":"","Parameters":"USB0::0x0AAD::0x0054::181629::INSTR,Ch2"},

        //{ "Title":"生成幅频特性系数-Ch1","ClassName":"BatchTask_GenerateCoefficientsWithScanFrequencyResult","TipMessage":"","Description":"","Parameters":"USB0::0x1AB1::0x0641::DG4C140300034::INSTR,Ch1,.\\扫频频率点表\\生成幅频特性扫频频点.txt,.\\ScanFrequencyResultData,AFC,GenerateCoefficientsTable_AFC.dll,1|1"},
        //{ "Title":"生成插值系数-Ch1","ClassName":"BatchTask_GenerateCoefficientsWithScanFrequencyResult","TipMessage":"","Description":"","Parameters":"USB0::0x1AB1::0x0641::DG4C140300034::INSTR,Ch1,.\\扫频频率点表\\生成插值系数扫频频点.txt,.\\ScanFrequencyResultData,Interpolation_Acq,GenerateCoefficientsTable_Interpolation.dll,1|1"},
        //{ "Title":"生成TI校准系数-Ch1","ClassName":"BatchTask_GenerateCoefficientsWithScanFrequencyResult","TipMessage":"","Description":"","Parameters":"USB0::0x1AB1::0x0641::DG4C140300034::INSTR,Ch1,.\\扫频频率点表\\生成TiAdc校准系数扫频频点.txt,.\\ScanFrequencyResultData,TiAdc,GenerateCoefficientsTable_TiAdc.dll,1|1"}
        //]

        List<TitleClassNamePair>? allDefinedProcessors = null;

        private void Init()
        {
            labelCurrentStepMessage.Text = "";
            richTextBoxTaskDescription.Clear();
            richTextBoxResultMessage.Clear();
            buttonCancelTask.Enabled = false;
            comboBoxTasks.Items.Clear();
            string jsonFileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".BatchTaskProcessor.config";
            if (!File.Exists(jsonFileName))
                return;
            string ConfigFileContent = File.ReadAllText(jsonFileName, Encoding.UTF8);
            allDefinedProcessors = JsonSerializer.Deserialize<List<TitleClassNamePair>>(ConfigFileContent);
            FilterTaskList();
        }
        private void FilterTaskList()
        {
            if (allDefinedProcessors == null || allDefinedProcessors.Count == 0)
                return;
            comboBoxTasks.Items.Clear();
            foreach (TitleClassNamePair item in allDefinedProcessors)
            {
                if (ExistsBatchTask(item.ClassName))
                {
                    bool bCanUse = false;
                    if (currInstrument == null)
                        bCanUse = true;
                    else
                    {
                        string[] usedForList = item.UsedFor.Split(',');
                        if (usedForList[0].ToUpper() == "ALL")
                            bCanUse = true;
                        else
                        {
                            foreach (string s in usedForList)
                            {
                                if (s.Trim() == ServerDomainConstants.ProductType.ToString())
                                {
                                    bCanUse = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (bCanUse)
                        comboBoxTasks.Items.Add(new ComboxItemObject { Text = item.Title, Value = item.ClassName, Tag = item.Parameters, TipMessage = item.TipMessage, Description = item.Description });
                }
            }
            if (allDefinedProcessors.Count > 0)
                comboBoxTasks.SelectedIndex = 0;
        }
        IBatchTask? CreateBatchTask(string className)
        {
            if (BaseHelper.AllBatchTaskClass == null)
                return null;
            var q = from t in BaseHelper.AllBatchTaskClass.ToArray()
                    where t.Name == className
                    select t;
            if (q == null)
                return null;
            if (Enumerable.Count(q) > 0)
            {
                object? obj = Activator.CreateInstance((Type)q.ToList()[0]);
                if (obj == null)
                    return null;
                return (IBatchTask)obj;
            }
            return null;
        }
        Boolean ExistsBatchTask(string className)
        {
            var q = from t in BaseHelper.AllBatchTaskClass?.ToArray()
                    where t.Name == className
                    select t;
            return (Enumerable.Count(q) > 0);
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            if (currInstrument != null)
                FilterTaskList();
        }
        public void RefreshData()
        {
        }

        /// <summary>
        /// 使用_ReportInfos信息刷新LvStaticInfo
        /// </summary>
        private void RefreshLvStaticInfo()
        {
            //_ReportInfos
            List<string> fileNames = new List<string>();

            _ReportInfos.ForEach(info =>
            {
                if (!fileNames.Contains(info.CurrXmlFileName))
                    fileNames.Add(info.CurrXmlFileName);
            });

            foreach (var fn in fileNames)
            {
                ListViewItem currItem = LvStaticInfo.Items.Cast<ListViewItem>().FirstOrDefault(item => item.Text == fn);
                if (currItem == null)
                    currItem = LvStaticInfo.Items.Add(fn);

                int sum = _ReportInfos.Count(info => info.CurrXmlFileName == fn);
                int success = _ReportInfos.Count(info => info.CurrXmlFileName == fn && info.Result == BatchTaskPartResult.Succeed);

                if (currItem.SubItems.Count == 4)
                {
                    currItem.SubItems[1].Text = $"{success}/{sum}";
                    currItem.SubItems[2].Text = (success == sum) ? "Success" : "Fail";
                }
                else
                {
                    currItem.SubItems.Add($"{success}/{sum}");
                    currItem.SubItems.Add((success == sum) ? "Success" : "Fail");
                    currItem.SubItems.Add("详情");
                }

            }
        }

        private void LvStaticInfo_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void LvStaticInfo_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                Color bgColor = (e.SubItem.Text == "Success") ? Color.Green : Color.Red;
                e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);
            }
            e.DrawText();
        }

        private void LvStaticInfo_MouseClick(object? sender, MouseEventArgs e)
        {
            ListViewItem item = null;
            ListViewItem.ListViewSubItem subItem = null;
            for (int itemId = 0; itemId < LvStaticInfo.Items.Count; itemId++)
            {
                if (LvStaticInfo.GetItemRect(itemId).Contains(e.Location))
                {
                    item = LvStaticInfo.Items[itemId];
                    subItem = item.GetSubItemAt(e.Location.X, e.Location.Y);
                    break;
                }
            }
            if (subItem == null)
                return;

            if (item.SubItems.IndexOf(subItem) == 3)
            {
                var reportInfos = _ReportInfos.Where(info => info.CurrXmlFileName == item.Text).ToList();
                ReportInfoDetailsForm rForm = new ReportInfoDetailsForm(reportInfos);
                rForm.StartPosition = FormStartPosition.CenterScreen;
                rForm.ShowDialog();
            }
        }


        StringBuilder resultMessages = new StringBuilder();
        StringBuilder resultTempMessages = new StringBuilder();
        private int showedLines = 0;
        string[] showedLinesStr = new string[] { "", "", "", "", "", "", "", "", "", "", "" };
        private async void ProcessBarUpdateAsync(int step, string currStepMessage, string lastStepResultMessage)
        {
            await Task.Run(() =>
            {
                //处理执行函数完成信息
                if (currStepMessage != "")
                {
                    var subStr = currStepMessage.Split(";");
                    if (subStr.Length > 3)
                    {
                        string xmlFile = Path.GetFileName(subStr[0].Trim());
                        string funcInfo = subStr[1].Trim();
                        string funcName = funcInfo.Substring(0, funcInfo.IndexOf(" "));
                        string stepInfo = subStr[2].Trim();

                        BatchTaskPartResult result = (subStr[3].Trim()) switch
                        {
                            nameof(BatchTaskPartResult.Succeed) => BatchTaskPartResult.Succeed,
                            nameof(BatchTaskPartResult.ErrorParameter) => BatchTaskPartResult.ErrorParameter,
                            nameof(BatchTaskPartResult.ErrorFatal) => BatchTaskPartResult.ErrorFatal,
                            nameof(BatchTaskPartResult.Cancel) => BatchTaskPartResult.Cancel,
                            _ => BatchTaskPartResult.ErrorGeneral,
                        };
                        _ReportInfos.Add(new ReportInfo(xmlFile, funcName, stepInfo, result,
                            $"函数信息：{funcInfo} \n结果信息：{lastStepResultMessage}"));
                    }
                }

                if (lastStepResultMessage != "")
                {
                    resultMessages.AppendLine(lastStepResultMessage);

                    showedLines = (showedLines + 1) % 10;
                    showedLinesStr[showedLines] = lastStepResultMessage;
                    resultTempMessages.Clear();
                    int i = (showedLines + 1) % 10;
                    do
                    {
                        if (showedLinesStr[i].Trim() != "")
                            resultTempMessages.AppendLine(showedLinesStr[i]);
                        i = (i + 1) % 10;
                    } while (i != showedLines);
                    if (showedLinesStr[i].Trim() != "")
                        resultTempMessages.AppendLine(showedLinesStr[i]);
                }
                int currStep = step > currBatchTask!.MaxStepCount ? currBatchTask.MaxStepCount : step;
                if (InvokeRequired)
                {
                    try
                    {
                        progressBar1.Invoke(new Action(() => this.  progressBar1.Value = currStep));
                        labelStep.Invoke(new Action(() => this.labelStep.Text = $"{currBatchTask.MaxStepCount}/{currStep}"));

                        labelCurrentStepMessage.Invoke(new Action(() => this.labelCurrentStepMessage.Text = currStepMessage));
                        richTextBoxResultMessage.Invoke(new Action(() => this.richTextBoxResultMessage.Text = resultTempMessages.ToString()));
                        //richTextBoxResultMessage.Invoke(new Action(() =>
                        //{
                        //    this.richTextBoxResultMessage.SelectionStart = richTextBoxResultMessage.TextLength < 0 ? 0 : richTextBoxResultMessage.TextLength;
                        //    this.richTextBoxResultMessage.ScrollToCaret();
                        //}));
                        LvStaticInfo.Invoke(RefreshLvStaticInfo);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    this.progressBar1.Value = currStep;
                    this.labelCurrentStepMessage.Text = currStepMessage;
                    this.richTextBoxResultMessage.Text = resultTempMessages.ToString();
                    this.richTextBoxResultMessage.SelectionStart = richTextBoxResultMessage.TextLength < 0 ? 0 : richTextBoxResultMessage.TextLength;
                    this.richTextBoxResultMessage.ScrollToCaret();
                    LvStaticInfo.Invoke(RefreshLvStaticInfo);
                }
            });
        }
        IBatchTask? currBatchTask = null;
        private void ShowFileMessage(string fileMessage, string InstrumentationInfo)
        {
            dataGridViewFileMessage.RowCount = 0;
            dataGridViewMuliXMLFile_InstrumentationInfo.RowCount = 0;
            if (fileMessage != "")
            {
                string[] lines = fileMessage.Split(System.Environment.NewLine);
                if (lines.Length != 0)
                {
                    foreach (string line in lines)
                    {
                        if (line.Trim() != "")
                        {
                            string[] parts = line.Split(';');
                            dataGridViewFileMessage.Rows.Add(1);
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[0].Value = parts[0];
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[1].Value = parts[1];
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[2].Value = parts[2];
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[3].Value = parts[3];
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[4].Value = parts[4];
                            dataGridViewFileMessage.Rows[dataGridViewFileMessage.RowCount - 1].Cells[5].Value = parts[5];
                        }
                    }
                }
            }
            if (InstrumentationInfo != "")
            {
                string[] lines = InstrumentationInfo.Split(System.Environment.NewLine);
                if (lines.Length > 0)
                {
                    foreach (string line in lines)
                    {
                        if (line.Trim() != "")
                        {
                            string[] parts = line.Split(';');
                            dataGridViewMuliXMLFile_InstrumentationInfo.Rows.Add(1);
                            dataGridViewMuliXMLFile_InstrumentationInfo.Rows[dataGridViewMuliXMLFile_InstrumentationInfo.RowCount - 1].Cells[0].Value = parts[0];
                            dataGridViewMuliXMLFile_InstrumentationInfo.Rows[dataGridViewMuliXMLFile_InstrumentationInfo.RowCount - 1].Cells[1].Value = parts[1];
                        }
                    }
                }
            }
        }
        private async void buttonStartTask_Click(object sender, EventArgs e)
        {
            buttonOpenResultFile.Visible = false;
            if (currBatchTask == null)
                return;
            BatchTaskPart_TxtFileStream.ForceCloseFile();

            _ReportInfos.Clear();
            LvStaticInfo.Items.Clear();
            resultMessages.Clear();
            resultTempMessages.Clear();

            labelCurrentStepMessage.Text = "";
            richTextBoxResultMessage.Clear();
            ComboxItemObject comboxItemObject = (ComboxItemObject)comboBoxTasks.SelectedItem;
            if (!currBatchTask.Init(currInstrument!, comboxItemObject.TipMessage, comboxItemObject.Description, comboxItemObject.Tag, out string ErrorMsg))
            {
                buttonCancelTask.Enabled = false;
                buttonStartTask.Enabled = true;

                MessageBox.Show(ErrorMsg);
                return;
            }
            progressBar1.Maximum = currBatchTask.MaxStepCount;
            string fileMessage = "";
            string InstrumentationInfo = "";
            if (!currBatchTask.CheckPrepareOk(ref fileMessage, ref InstrumentationInfo))
            {
                ShowFileMessage(fileMessage, InstrumentationInfo);
                buttonCancelTask.Enabled = false;
                buttonStartTask.Enabled = true;
                return;
            }
            ShowFileMessage(fileMessage, InstrumentationInfo);
            if (currBatchTask.SpecialDescripton != "")
            {
                resultMessages.Append(currBatchTask.SpecialDescripton);
                resultTempMessages.Append(currBatchTask.SpecialDescripton);
            }
            this.richTextBoxResultMessage.Text = resultTempMessages.ToString();
            buttonCancelTask.Enabled = true;
            buttonStartTask.Enabled = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            labelCurrXmlFileName.Text = currBatchTask.CurrentProcessingXmlFilename;

            if (currInstrument != null)
            {
                //日志信息记录
                Utilities.Logger.WriteLine($"[***BatchTaskStart***]:{labelCurrXmlFileName.Text}");
                currInstrument?.WriteString("*IDN?");
                Utilities.Logger.WriteLine($"Instrument Info:{currInstrument!.ReadShortString() ?? " "}");
            }

            BatchTaskState state = await currBatchTask.RunAsync(ProcessBarUpdateAsync);
            stopwatch.Stop();
            MessageBox.Show(state switch
            {
                BatchTaskState.FinishedFailed => "当前任务 失败了！",
                BatchTaskState.FinishedOK => "当前任务成功结束！",
                BatchTaskState.Canceled => "当前任务被取消！",
                _ => "未知结束状态！"
            });
            if (currBatchTask.ResultTipMessage != "")
            {
                resultMessages.AppendLine();
                resultMessages.AppendLine(currBatchTask.ResultTipMessage);
                resultMessages.AppendLine();
                resultMessages.AppendLine($"当前任务执行花费时间:{stopwatch.ElapsedMilliseconds / 1000 / 60}:{(stopwatch.ElapsedMilliseconds / 1000) % 60}");
            }
            if (currBatchTask.ResultFileName != "")
            {
                buttonOpenResultFile.Visible = true;
                buttonOpenResultFile.Tag = currBatchTask.ResultFileName;
            }
            this.richTextBoxResultMessage.Text = resultMessages.ToString();
            progressBar1.Value = 0;
            buttonCancelTask.Enabled = false;
            buttonStartTask.Enabled = true;

            BatchTaskPart_TxtFileStream.ForceCloseFile();
        }

        private void buttonCancelTask_Click(object sender, EventArgs e)
        {
            buttonOpenResultFile.Visible = false;
            currBatchTask?.Cancel();
            BatchTaskPart_TxtFileStream.ForceCloseFile();
        }

        private void comboBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOpenResultFile.Visible = false;
            ComboxItemObject obj = (ComboxItemObject)comboBoxTasks.SelectedItem;
            currBatchTask = (comboBoxTasks.SelectedIndex < 0) ? null : CreateBatchTask(obj!.Value!.ToString() ?? "");
            if (currBatchTask != null)
            {
                richTextBoxTaskDescription.Text = (comboBoxTasks.SelectedItem! as ComboxItemObject)!.Description;
                richTextBoxParameter.Text = (comboBoxTasks.SelectedItem! as ComboxItemObject)!.Tag;
                richTextBoxCurrTaskTypeParameterDescription.Text = currBatchTask.TaskParameterDescription(obj.Tag);
            }
        }

        private void buttonOpenResultFile_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", buttonOpenResultFile.Tag.ToString() ?? "");
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void comboBoxTaskParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBoxTaskPartHelp.Clear();
            if (comboBoxTaskParts.SelectedIndex < 0)
                return;
            IBatchTaskPart? part = null;
            string className = comboBoxTaskParts!.SelectedItem!.ToString() ?? "";
            var q = from t in BaseHelper.AllBatchTaskPartClass?.ToArray()
                    where t.Name == className
                    select t;
            if (Enumerable.Count(q) > 0)
            {
                part = (IBatchTaskPart?)Activator.CreateInstance((Type)q.ToList()[0]);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("===功能描述===");
                sb.AppendLine(part!.FuncionDescription);
                sb.AppendLine("===参数约定===");
                sb.AppendLine(part!.ParametersDescription);
                sb.AppendLine("=====示例=====");
                sb.AppendLine(part!.Example);
                richTextBoxTaskPartHelp.Text = sb.ToString();
            }
        }

        private void BtnGenerateXml_Click(object sender, EventArgs e)
        {
            BatchTaskPartXml_Generator.Generate(ProductType.JiHe_MSO7000X, CaliType.TiAdc);
            BatchTaskPartXml_Generator.Generate(ProductType.JiHe_MSO7000X, CaliType.Gain);
            BatchTaskPartXml_Generator.Generate(ProductType.JiHe_MSO7000X, CaliType.Baseline);
            BatchTaskPartXml_Generator.Generate(ProductType.JiHe_MSO7000X, CaliType.Offset);
            BatchTaskPartXml_Generator.Generate(ProductType.JiHe_MSO7000X, CaliType.AllType);
        }
    }
}
