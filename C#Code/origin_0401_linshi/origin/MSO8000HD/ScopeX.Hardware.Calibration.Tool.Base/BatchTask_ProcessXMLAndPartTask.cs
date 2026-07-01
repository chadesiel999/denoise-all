using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class BatchTask_ProcessXMLAndPartTask : BatchTaskBase
    {
        string taskDescription = "";
        string taskImportanceTipMessage = "";
        string ExcelFile = "";
        List<XmlScpiCmd> allScpiCmd = new List<XmlScpiCmd>();
        Dictionary<string, IInstrumentSession> OtherInstruments = new Dictionary<string, IInstrumentSession>();
        Dictionary<string, InstrumentDefine> OtherInstrumentsDefine = new Dictionary<string, InstrumentDefine>();
        Dictionary<string, string> ConstVariantDefine = new Dictionary<string, string>();
        string ourInstrumentName = "";
        string xmlFileErrorMessage = "";
        string currXmlFilename = "";
        string currSubXmlFilename = "";
        public override string CurrentProcessingXmlFilename
        {
            get => currXmlFilename;
        }
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            base.Init(instrumentInteract, tipMessage, description, tag, out ErrorMsg);
            this.ourInstrument = instrumentInteract;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = $"选择{TipMessage}方案XML文件。";
            openFileDialog.Filter = "方案XML|*.xml";
            allScpiCmd.Clear();
            totalStep = 0;
            stepIndex = 0;
            totalErrorCount = 0;
            batchXmlFlieList.Clear();
            RuntimeForeachXmlScpiCmd.Clear();
            RuntimeForeachNowIndex.Clear();
            RuntimeForeachVariantDefine.Clear();
            OtherInstruments.Clear();
            OtherInstrumentsDefine.Clear();
            currXmlFilename = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ConstVariantDefine.Clear();
                string xmlFileName = openFileDialog.FileName;
                currXmlFilename = xmlFileName;
                xmlFileErrorMessage = onOpenXmlFile(xmlFileName);
                if (xmlFileErrorMessage != "")
                {
                    MessageBox.Show(xmlFileErrorMessage);
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string SpecialDescripton
        {
            get => taskDescription;
        }
        List<string> batchXmlFlieList = new List<string>();
        int totalStep = 0;
        private string onOpenXmlFile(string selectedFileName)
        {
            batchXmlFlieList.Clear();
            XmlReader reader;
            reader = XmlReader.Create(selectedFileName);
            try
            {
                if (reader.ReadToFollowing("MultiXmlBatchExecuteSolution"))
                {
                    XmlReader MultiXmlBatchNode = reader.ReadSubtree();
                    XmlReader mainSub = MultiXmlBatchNode;
                    string fileRootPath = "";
                    while (mainSub.Read())
                    {
                        if (mainSub.NodeType == XmlNodeType.Element && mainSub.Name == "FileRootPath")
                        {
                            fileRootPath = mainSub.GetAttribute("Value") ?? "";
                            break;
                        }
                    }
                    fileRootPath = fileRootPath.Replace($@"@ToolRootPath@", AppDomain.CurrentDomain.BaseDirectory);
                    if (fileRootPath.Trim() == "")
                        fileRootPath = Path.GetDirectoryName(selectedFileName)!;
                    if (fileRootPath[0] == '.')
                        fileRootPath = Path.GetDirectoryName(selectedFileName)!;
                    if (fileRootPath[fileRootPath.Length - 1] != '\\')
                        fileRootPath = Path.GetDirectoryName(selectedFileName) + $@"\";
                    XmlReader filesSub = MultiXmlBatchNode;
                    while (filesSub.Read())
                    {
                        if (filesSub.NodeType == XmlNodeType.Element && filesSub.Name == "XmlFile")
                        {
                            string fileName = filesSub.GetAttribute("Value") ?? "";
                            if (fileName.Trim() != "")
                            {
                                if (fileName[0] == '.')
                                {
                                    if (fileRootPath.Trim() == "")
                                    {
                                        reader.Close();
                                        return $"{fileName}路径设置不正确";
                                    }
                                    if (fileRootPath[fileRootPath.Length - 1] != '\\')
                                        fileRootPath += @"\";
                                    if (!File.Exists(fileRootPath + fileName))
                                    {
                                        reader.Close();
                                        return $"{fileRootPath + fileName}不存在！";
                                    }
                                    batchXmlFlieList.Add(fileRootPath + fileName);
                                }
                                else
                                {
                                    if (!File.Exists(fileRootPath + fileName))
                                    {
                                        reader.Close();
                                        return $"{fileName}不存在！";
                                    }
                                    batchXmlFlieList.Add(fileName);
                                }
                            }
                        }
                    }
                }
                else
                {
                    batchXmlFlieList.Add(selectedFileName);
                }
            }
            catch (Exception e)
            {
                reader.Close();
                return e.ToString();
            }
            reader.Close();
            if (batchXmlFlieList.Count == 0)
                return "没有可执行的XML文件！";
            totalStep = 0;
            stepIndex = 0;
            foreach (string xmlFileName in batchXmlFlieList)
            {
                string analyResult = AnalyOneXmlFile(xmlFileName);

                if (analyResult != "")
                    return analyResult;
                totalStep += GetStepCount();
            }
            return "";
        }
        private int GetStepCount()
        {
            stepIndex = 0;
            RuntimeForeachXmlScpiCmd.Clear();
            RuntimeForeachNowIndex.Clear();
            foreach (XmlScpiCmd cmd in allScpiCmd)
            {
                ProcessOneCommand(cmd, true);
            }
            return stepIndex + 1;
        }
        private XmlScpiCmd ResolveXmlScpiCmd(XmlReader commandReader, bool bExistsExcelFile)
        {
            XmlScpiCmd xmlScpiCmd = new XmlScpiCmd();

            xmlScpiCmd.InstrumentationName = BaseHelper.TryConvertToString(commandReader.GetAttribute("InstrumentationName") ?? "");
            xmlScpiCmd.Description = BaseHelper.TryConvertToString(commandReader.GetAttribute("Description") ?? "");
            xmlScpiCmd.CmdType = (commandReader.GetAttribute("CommandType") ?? "").ToUpper() switch
            {
                "SCPI" => XmlScpiCmdType.SCPI,
                "CHECK" => XmlScpiCmdType.Check,
                "UIFUNCTION" => XmlScpiCmdType.UIFunction,
                _ => XmlScpiCmdType.Unknow
            };
            xmlScpiCmd.bIsProgramFunc = BaseHelper.TryConvertToBoolean(commandReader.GetAttribute("IsProgramFunc") ?? "false");
            if (xmlScpiCmd.bIsProgramFunc)
            {
                xmlScpiCmd.ProgramFuncName = BaseHelper.TryConvertToString(commandReader.GetAttribute("ProgramFuncName") ?? "");
                xmlScpiCmd.ProgramFuncOverSec = BaseHelper.TryConvertToDouble(commandReader.GetAttribute("ProgramFuncOverSec") ?? "0");
                xmlScpiCmd.ProgramFuncStepCount = BaseHelper.TryConvertToInt(commandReader.GetAttribute("ProgramFuncStepCount") ?? "1");
            }
            xmlScpiCmd.AfterWaitSec = BaseHelper.TryConvertToDouble(commandReader.GetAttribute("AfterWaitSec") ?? "0");

            xmlScpiCmd.SCPICmd = BaseHelper.TryConvertToString(commandReader.GetAttribute("SCPICmd") ?? "");
            xmlScpiCmd.EqualValue = BaseHelper.TryConvertToString(commandReader.GetAttribute("EqualValue") ?? "");

            xmlScpiCmd.Excel_WriteContent = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_WriteContent") ?? "");
            xmlScpiCmd.Excel_WriteAtCell = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_WriteAtCell") ?? "");
            xmlScpiCmd.BooleanCheckFormula = BaseHelper.TryConvertToString(commandReader.GetAttribute("BooleanCheckFormula") ?? "");
            if (xmlScpiCmd.BooleanCheckFormula != "")
                xmlScpiCmd.BooleanCheckFormula = BaseHelper.ReplaceESCChar(xmlScpiCmd.BooleanCheckFormula);

            xmlScpiCmd.Excel_WriteContent2 = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_WriteContent2") ?? "");
            xmlScpiCmd.Excel_WriteAtCell2 = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_WriteAtCell2") ?? "");
            xmlScpiCmd.BooleanCheckFormula2 = BaseHelper.TryConvertToString(commandReader.GetAttribute("BooleanCheckFormula2") ?? "");
            if (xmlScpiCmd.BooleanCheckFormula2 != "")
                xmlScpiCmd.BooleanCheckFormula2 = BaseHelper.ReplaceESCChar(xmlScpiCmd.BooleanCheckFormula2);

            xmlScpiCmd.Excel_InsertPicture = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_InsertPicture") ?? "");
            xmlScpiCmd.Excel_InsertHyperlinks = BaseHelper.TryConvertToString(commandReader.GetAttribute("Excel_InsertHyperlinks") ?? "");

            if (!bExistsExcelFile)
            {
                xmlScpiCmd.Excel_WriteAtCell = "";
                xmlScpiCmd.Excel_WriteAtCell2 = "";
                xmlScpiCmd.Excel_InsertPicture = "";
                xmlScpiCmd.Excel_InsertHyperlinks = "";
            }
            if (xmlScpiCmd.CmdType == XmlScpiCmdType.SCPI)
            {
                if (xmlScpiCmd.SCPICmd.IndexOf('?') >= 0)
                {
                    xmlScpiCmd.QueryOvertimeSec = BaseHelper.TryConvertToDouble(commandReader.GetAttribute("QueryOvertimeSec") ?? "0");
                    xmlScpiCmd.bQueryWaitWhenNotEmpty = BaseHelper.TryConvertToBoolean(commandReader.GetAttribute("QueryWaitWhenNotEmpty") ?? "false");
                }
            }
            return xmlScpiCmd;
        }

        private string AnalyOneXmlFile(string xmlFileName)
        {
            OtherInstrumentsDefine.Clear();
            allScpiCmd.Clear();
            ExcelFile = "";

            XmlReader reader;
            reader = XmlReader.Create(xmlFileName);
            reader.ReadToFollowing("InstrumentationAutoControlSolution");
            string version = reader.GetAttribute("version") ?? "";
            string creator = reader.GetAttribute("creator") ?? "";
            string lastModifier = reader.GetAttribute("LastModifier") ?? "";
            string description = reader.GetAttribute("SolutionDescription") ?? "";
            var d = File.GetLastWriteTime(xmlFileName);
            sbFileMessage.AppendLine($"{xmlFileName};{creator};{lastModifier};{d.Year}.{d.Month.ToString().PadLeft(2, '0')}.{d.Day.ToString().PadLeft(2, '0')} {d.Hour.ToString().PadLeft(2, '0')}:{d.Minute.ToString().PadLeft(2, '0')}:{d.Second.ToString().PadLeft(2, '0')};{version};{description}");
            XmlReader mainSub = reader.ReadSubtree();
            mainSub.ReadToFollowing("Instrumentations");
            XmlReader InstrumentationReader = mainSub.ReadSubtree();

            string errorMsg = "";
            try
            {
                while (InstrumentationReader.Read())
                {
                    if (InstrumentationReader.NodeType == XmlNodeType.Element && InstrumentationReader.Name == "Instrumentation")
                    {
                        InstrumentDefine instrumentDefine = new InstrumentDefine();
                        instrumentDefine.Name = BaseHelper.TryConvertToString(InstrumentationReader.GetAttribute("Name") ?? "");
                        instrumentDefine.Description = BaseHelper.TryConvertToString(InstrumentationReader.GetAttribute("Description") ?? "");
                        instrumentDefine.Address = BaseHelper.TryConvertToString(InstrumentationReader.GetAttribute("Address") ?? "");
                        instrumentDefine.bIsOurTarget = BaseHelper.TryConvertToBoolean(InstrumentationReader.GetAttribute("IsOurTargetDso") ?? "false");
                        if (!instrumentDefine.bIsOurTarget)
                            OtherInstrumentsDefine.Add(instrumentDefine.Name, instrumentDefine);
                        else
                            ourInstrumentName = instrumentDefine.Name;
                    }
                }

                mainSub.ReadToFollowing("Tasks");
                XmlReader TaskReader = mainSub.ReadSubtree();
                string taskDescription;
                bool bExistsExcelFile = false;
                while (TaskReader.Read())
                {
                    if (TaskReader.NodeType == XmlNodeType.Element && TaskReader.Name == "Task")
                    {
                        taskImportanceTipMessage = BaseHelper.TryConvertToString(TaskReader.GetAttribute("ImportanceTipMessage") ?? "");
                        taskDescription = BaseHelper.TryConvertToString(TaskReader.GetAttribute("Description") ?? "");
                        ExcelFile = BaseHelper.TryConvertToString(TaskReader.GetAttribute("ExcelFile") ?? "");
                        if (ExcelFile.Trim() != "")
                        {
                            if (ExcelFile[0] == '.')
                            {

                                ExcelFile = Path.GetDirectoryName(xmlFileName) + $@"\" + ExcelFile;
                            }
                            if (!File.Exists(ExcelFile))
                            {
                                errorMsg = $"指定的Excel File[{ExcelFile}]不存在，不能工作！。";
                                break;
                            }
                            Type? type = Type.GetTypeFromProgID("Excel.Application");
                            if (type == null)
                            {
                                errorMsg = $"没有安装Excel，不能工作！。";
                                break;
                            }
                            bExistsExcelFile = true;
                        }
                        else
                            bExistsExcelFile = false;
                        XmlReader commandReader = TaskReader.ReadSubtree();
                        List<XmlScpiCmd> ForeachLevelXmlScpiCmd = new List<XmlScpiCmd>();//标识Foreach的层次，用于表明command所在的foreach(loop或FileLine)

                        while (commandReader.Read())
                        {
                            if (commandReader.NodeType != XmlNodeType.Element)
                            {
                                if (commandReader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (commandReader.Name == "ForeachLoop" || commandReader.Name == "ForeachByFileLine")
                                        ForeachLevelXmlScpiCmd.RemoveAt(ForeachLevelXmlScpiCmd.Count - 1);
                                }
                                continue;
                            }
                            if (commandReader.Name == "ForeachLoop")
                            {
                                XmlScpiCmd xmlScpiCmd = new XmlScpiCmd();
                                xmlScpiCmd.CmdType = XmlScpiCmdType.ForeachLoop;
                                xmlScpiCmd.ForeachLoop_StartIndexStr = BaseHelper.TryConvertToString(commandReader.GetAttribute("StartIndex") ?? "0");
                                xmlScpiCmd.ForeachLoop_CountStr = BaseHelper.TryConvertToString(commandReader.GetAttribute("Count") ?? "0");
                                xmlScpiCmd.Description = BaseHelper.TryConvertToString(commandReader.GetAttribute("Description") ?? "");
                                //归属
                                if (ForeachLevelXmlScpiCmd.Count > 0)
                                    ForeachLevelXmlScpiCmd[ForeachLevelXmlScpiCmd.Count - 1].LoopXmlScpiCmds.Add(xmlScpiCmd);
                                else
                                    allScpiCmd.Add(xmlScpiCmd);

                                ForeachLevelXmlScpiCmd.Add(xmlScpiCmd);
                            }
                            else if (commandReader.Name == "ForeachByFileLine")
                            {
                                XmlScpiCmd xmlScpiCmd = new XmlScpiCmd();
                                xmlScpiCmd.CmdType = XmlScpiCmdType.ForeachByFileLine;

                                xmlScpiCmd.ForeachByFileLine_FileName = BaseHelper.TryConvertToString(commandReader.GetAttribute("FileName") ?? "");
                                xmlScpiCmd.ForeachByFileLine_FileID = BaseHelper.TryConvertToString(commandReader.GetAttribute("FileID") ?? "");
                                xmlScpiCmd.Description = BaseHelper.TryConvertToString(commandReader.GetAttribute("Description") ?? "");
                                //归属
                                if (ForeachLevelXmlScpiCmd.Count > 0)
                                    ForeachLevelXmlScpiCmd[ForeachLevelXmlScpiCmd.Count - 1].LoopXmlScpiCmds.Add(xmlScpiCmd);
                                else
                                    allScpiCmd.Add(xmlScpiCmd);

                                ForeachLevelXmlScpiCmd.Add(xmlScpiCmd);

                            }
                            else if (commandReader.Name == "Command")
                            {
                                XmlScpiCmd xmlScpiCmd = ResolveXmlScpiCmd(commandReader, bExistsExcelFile);
                                //归属
                                if (ForeachLevelXmlScpiCmd.Count > 0)
                                    ForeachLevelXmlScpiCmd[ForeachLevelXmlScpiCmd.Count - 1].LoopXmlScpiCmds.Add(xmlScpiCmd);
                                else
                                    allScpiCmd.Add(xmlScpiCmd);
                            }
                            else if (commandReader.Name == "VarintDefine")
                            {
                                if (ForeachLevelXmlScpiCmd.Count > 0 && ForeachLevelXmlScpiCmd[ForeachLevelXmlScpiCmd.Count - 1].CmdType == XmlScpiCmdType.ForeachLoop)
                                {
                                    string varintName = BaseHelper.TryConvertToString(commandReader.GetAttribute("name") ?? "");
                                    string valueDefine = BaseHelper.TryConvertToString(commandReader.GetAttribute("valuelist") ?? "");
                                    string[] valueArray = valueDefine.Split(',');
                                    List<string> valueList = new List<string>();
                                    foreach (string value in valueArray)
                                    {
                                        valueList.Add(value.Trim());
                                    }
                                    ForeachLevelXmlScpiCmd[ForeachLevelXmlScpiCmd.Count - 1].ForeachLoop_VariantDefine.Add(varintName.Trim(), valueList);
                                }
                            }
                            else if (commandReader.Name == "ConstVariantDefine")
                            {
                                string varintName = BaseHelper.TryConvertToString(commandReader.GetAttribute("name") ?? "");
                                string valueDefine = BaseHelper.TryConvertToString(commandReader.GetAttribute("value") ?? "");
                                if (varintName.Trim() != "" && valueDefine.Trim() != "")
                                {
                                    if (!ConstVariantDefine.ContainsKey(varintName))
                                        ConstVariantDefine.Add(varintName, valueDefine);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                errorMsg = "方案文件格式有问题，不能处理！" + ee.ToString();
            }
            finally
            {
                reader.Close();
            }
            return errorMsg;
        }
        private string resultTipMessage = "";
        public override string ResultTipMessage
        {
            get => resultTipMessage;
        }
        public override string ResultFileName
        {
            get => ExcelFile;
        }
        StringBuilder sbFileMessage = new StringBuilder();
        StringBuilder sbInstrumentationInfo = new StringBuilder();
        List<string> existsInstrumentationInfo = new List<string>();

        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            sbFileMessage.Clear();
            sbInstrumentationInfo.Clear();
            existsInstrumentationInfo.Clear();
            fileMessage = "";
            if (batchXmlFlieList.Count == 0)
            {
                MessageBox.Show("请先选中需要校准的xml文件！");
                return false;
            }
            foreach (string xmlFile in batchXmlFlieList)
            {
                AnalyOneXmlFile(xmlFile);
                OtherInstruments.Clear();
                foreach (KeyValuePair<string, InstrumentDefine> kvp in OtherInstrumentsDefine)
                {
                    string message = "";
                    string instrumentationInfo = $"{kvp.Value.Name}@@@@@{kvp.Value.Address}";
                    if (!existsInstrumentationInfo.Contains(instrumentationInfo))
                    {
                        existsInstrumentationInfo.Add(instrumentationInfo);
                        sbInstrumentationInfo.AppendLine($"{kvp.Value.Name};{kvp.Value.Address}");
                    }
                    IInstrumentSession? instrument = InstrumentSessionEngine.TryGetSession(kvp.Value.Address, "500", null, out message);
                    if (instrument == null)
                    {
                        MessageBox.Show($"仪器{kvp.Value.Name}[{kvp.Value.Address}]无效！");
                        return false;
                    }
                    OtherInstruments.Add(kvp.Key, instrument);
                }
                foreach (XmlScpiCmd cmd in allScpiCmd)
                {
                    if (cmd.bIsProgramFunc)
                    {
                        if (cmd.ProgramFuncName == "")
                        {
                            MessageBox.Show($"{cmd.Description}中没有定义ProgramFuncName！");
                            return false;
                        }
                    }
                }
            }
            fileMessage = sbFileMessage.ToString();
            InstrumentationInfo = sbInstrumentationInfo.ToString();
            string msg = (batchXmlFlieList.Count > 1) ? "" : taskImportanceTipMessage;
            if (msg != "")
                msg += System.Environment.NewLine + System.Environment.NewLine;
            msg += "你确认要执行该任务吗？";
            return MessageBox.Show(msg, "提示", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
        public override int MaxStepCount
        {
            get => totalStep;
        }
        private IInstrumentSession? GetInstrumentSessionByName(string InstrumentationName)
        {
            if (InstrumentationName == ourInstrumentName)
                return ourInstrument;
            else if (OtherInstruments.ContainsKey(InstrumentationName))
                return OtherInstruments[InstrumentationName];
            else
                return null;
        }
        private void SpecialWriteString(string InstrumentationName, string SCPICmd, double AfterWaitSec)
        {
            IInstrumentSession? instrumentSession = GetInstrumentSessionByName(InstrumentationName);
            string[] cmdList = SCPICmd.Split(';');
            foreach (string cmd in cmdList)
            {
                if (cmd.Trim() != "")
                    instrumentSession?.WriteString(cmd.Trim() + "\r\n");
            }

            if (AfterWaitSec > 0)
                Thread.Sleep((int)(AfterWaitSec * 1000));
        }
        private string SpecialReadString(string InstrumentationName)
        {
            if (InstrumentationName == ourInstrumentName)
                return ourInstrument!.ReadString();
            else
                return OtherInstruments[InstrumentationName].ReadString();
        }
        private void PreprocessForeachByFileLine(XmlScpiCmd xmlScpiCmd, string fileName)
        {
            xmlScpiCmd.ForeachByFileLine_LineCount = 0;
            xmlScpiCmd.ForeachByFileLine_FileColumnRows.Clear();
            if (fileName != "")
            {
                if (fileName[0] == '.')
                    fileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
                if (File.Exists(fileName))
                {
                    string[] fileLines = File.ReadAllLines(fileName);
                    string firstValidLine = "";
                    foreach (string fileLine in fileLines)
                    {
                        string line = fileLine.Trim();
                        if (!line.StartsWith("#") && !line.StartsWith("//") && line != "")
                        {
                            firstValidLine = line;
                            break;
                        }
                    }
                    if (firstValidLine != "")
                    {
                        xmlScpiCmd.ForeachByFileLine_ColumnCount = firstValidLine.Split(',').Length;
                        xmlScpiCmd.ForeachByFileLine_FileColumnRows.Clear();

                        for (int i = 0; i < xmlScpiCmd.ForeachByFileLine_ColumnCount; i++)
                            xmlScpiCmd.ForeachByFileLine_FileColumnRows.Add(new List<string>());
                        foreach (string fileLine in fileLines)
                        {
                            string line = fileLine.Trim();

                            if (!line.StartsWith("#") && !line.StartsWith("//") && line != "")
                            {
                                string[] columns = line.Split(',');
                                for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
                                    xmlScpiCmd.ForeachByFileLine_FileColumnRows[columnIndex].Add(columns[columnIndex].Trim());
                            }

                        }
                        xmlScpiCmd.ForeachByFileLine_LineCount = xmlScpiCmd.ForeachByFileLine_FileColumnRows[0].Count;
                    }
                }
            }
        }
        private void ReplaceVarint(ref MaybeReplaceElement maybeReplaceElement)
        {
            if (maybeReplaceElement.ToString().IndexOf('@') < 0)
                return;
            if (RuntimeForeachXmlScpiCmd.Count > 0)
            {
                RuntimeForeachVariantDefine.Clear();

                for (int foreachLevelIndex = 0; foreachLevelIndex < RuntimeForeachXmlScpiCmd.Count; foreachLevelIndex++)
                {
                    if (RuntimeForeachXmlScpiCmd[foreachLevelIndex].CmdType == XmlScpiCmdType.ForeachLoop)
                    {
                        Dictionary<string, List<string>> currVariantDefine = new Dictionary<string, List<string>>();
                        foreach (var v in RuntimeForeachXmlScpiCmd[foreachLevelIndex].ForeachLoop_VariantDefine)
                        {
                            List<string> tmpDefineList = new List<string>();
                            foreach (var v2 in v.Value)
                                tmpDefineList.Add(v2);
                            currVariantDefine.Add(v.Key, tmpDefineList);
                        }
                        RuntimeForeachVariantDefine.Add(foreachLevelIndex, currVariantDefine);
                    }
                }
                foreach (var v in RuntimeForeachVariantDefine)
                {
                    foreach (var name_Values in v.Value)
                    {
                        string name = name_Values.Key;
                        string replaceValue = name_Values.Value[RuntimeForeachNowIndex[v.Key]];
                        foreach (var replaceItem in RuntimeForeachVariantDefine)
                        {
                            if (replaceItem.Key > v.Key)
                            {
                                foreach (var n_v in replaceItem.Value)
                                {
                                    for (int i = 0; i < n_v.Value.Count; i++)
                                        n_v.Value[i] = n_v.Value[i].Replace($"@{name}@", replaceValue);
                                }
                            }
                        }
                    }
                }

                for (int foreachLevelIndex = 0; foreachLevelIndex < RuntimeForeachXmlScpiCmd.Count; foreachLevelIndex++)
                {
                    if (RuntimeForeachXmlScpiCmd[foreachLevelIndex].CmdType == XmlScpiCmdType.ForeachLoop)
                    {
                        int currLoopIndex = RuntimeForeachNowIndex[foreachLevelIndex];
                        foreach (var v in RuntimeForeachVariantDefine[foreachLevelIndex])
                        {
                            maybeReplaceElement.ProgramFuncName = maybeReplaceElement.ProgramFuncName.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Description = maybeReplaceElement.Description.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.SCPICmd = maybeReplaceElement.SCPICmd.Replace($"@{v.Key}@", v.Value[currLoopIndex]);

                            maybeReplaceElement.BooleanCheckFormula = maybeReplaceElement.BooleanCheckFormula.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Excel_WriteAtCell = maybeReplaceElement.Excel_WriteAtCell.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Excel_WriteContent = maybeReplaceElement.Excel_WriteContent.Replace($"@{v.Key}@", v.Value[currLoopIndex]);

                            maybeReplaceElement.BooleanCheckFormula2 = maybeReplaceElement.BooleanCheckFormula2.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Excel_WriteAtCell2 = maybeReplaceElement.Excel_WriteAtCell2.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Excel_WriteContent2 = maybeReplaceElement.Excel_WriteContent2.Replace($"@{v.Key}@", v.Value[currLoopIndex]);

                            maybeReplaceElement.Excel_InsertPicture = maybeReplaceElement.Excel_InsertPicture.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.Excel_InsertHyperlinks = maybeReplaceElement.Excel_InsertHyperlinks.Replace($"@{v.Key}@", v.Value[currLoopIndex]);

                            maybeReplaceElement.EqualValue = maybeReplaceElement.EqualValue.Replace($"@{v.Key}@", v.Value[currLoopIndex]);

                            maybeReplaceElement.ForeachLoop_StartIndexStr = maybeReplaceElement.ForeachLoop_StartIndexStr.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.ForeachLoop_CountStr = maybeReplaceElement.ForeachLoop_CountStr.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                            maybeReplaceElement.ForeachByFileLine_FileName = maybeReplaceElement.ForeachByFileLine_FileName.Replace($"@{v.Key}@", v.Value[currLoopIndex]);
                        }
                    }
                    else if (RuntimeForeachXmlScpiCmd[foreachLevelIndex].CmdType == XmlScpiCmdType.ForeachByFileLine)
                    {
                        int currRowIndex = RuntimeForeachNowIndex[foreachLevelIndex];
                        for (int columnIndex = 0; columnIndex < RuntimeForeachXmlScpiCmd[foreachLevelIndex].ForeachByFileLine_ColumnCount; columnIndex++)
                        {
                            string key = $"@FID:{RuntimeForeachXmlScpiCmd[foreachLevelIndex].ForeachByFileLine_FileID}:Column{columnIndex + 1}@";
                            string value = RuntimeForeachXmlScpiCmd[foreachLevelIndex].ForeachByFileLine_FileColumnRows[columnIndex][currRowIndex];
                            maybeReplaceElement.ProgramFuncName = maybeReplaceElement.ProgramFuncName.Replace(key, value);
                            maybeReplaceElement.Description = maybeReplaceElement.Description.Replace(key, value);
                            maybeReplaceElement.SCPICmd = maybeReplaceElement.SCPICmd.Replace(key, value);

                            maybeReplaceElement.BooleanCheckFormula = maybeReplaceElement.BooleanCheckFormula.Replace(key, value);
                            maybeReplaceElement.Excel_WriteAtCell = maybeReplaceElement.Excel_WriteAtCell.Replace(key, value);
                            maybeReplaceElement.Excel_WriteContent = maybeReplaceElement.Excel_WriteContent.Replace(key, value);

                            maybeReplaceElement.BooleanCheckFormula2 = maybeReplaceElement.BooleanCheckFormula2.Replace(key, value);
                            maybeReplaceElement.Excel_WriteAtCell2 = maybeReplaceElement.Excel_WriteAtCell2.Replace(key, value);
                            maybeReplaceElement.Excel_WriteContent2 = maybeReplaceElement.Excel_WriteContent2.Replace(key, value);

                            maybeReplaceElement.Excel_InsertPicture = maybeReplaceElement.Excel_InsertPicture.Replace(key, value);
                            maybeReplaceElement.Excel_InsertHyperlinks = maybeReplaceElement.Excel_InsertHyperlinks.Replace(key, value);

                            maybeReplaceElement.EqualValue = maybeReplaceElement.EqualValue.Replace(key, value);

                            maybeReplaceElement.ForeachLoop_StartIndexStr = maybeReplaceElement.ForeachLoop_StartIndexStr.Replace(key, value);
                            maybeReplaceElement.ForeachLoop_CountStr = maybeReplaceElement.ForeachLoop_CountStr.Replace(key, value);
                            maybeReplaceElement.ForeachByFileLine_FileName = maybeReplaceElement.ForeachByFileLine_FileName.Replace(key, value);
                        }
                    }
                }
            }
            if (ConstVariantDefine.Count > 0)
            {
                foreach (var variant in ConstVariantDefine)
                {
                    string key = $"@{variant.Key}@";
                    string value = variant.Value;
                    maybeReplaceElement.ProgramFuncName = maybeReplaceElement.ProgramFuncName.Replace(key, value);
                    maybeReplaceElement.Description = maybeReplaceElement.Description.Replace(key, value);
                    maybeReplaceElement.SCPICmd = maybeReplaceElement.SCPICmd.Replace(key, value);

                    maybeReplaceElement.BooleanCheckFormula = maybeReplaceElement.BooleanCheckFormula.Replace(key, value);
                    maybeReplaceElement.Excel_WriteAtCell = maybeReplaceElement.Excel_WriteAtCell.Replace(key, value);
                    maybeReplaceElement.Excel_WriteContent = maybeReplaceElement.Excel_WriteContent.Replace(key, value);

                    maybeReplaceElement.BooleanCheckFormula2 = maybeReplaceElement.BooleanCheckFormula2.Replace(key, value);
                    maybeReplaceElement.Excel_WriteAtCell2 = maybeReplaceElement.Excel_WriteAtCell2.Replace(key, value);
                    maybeReplaceElement.Excel_WriteContent2 = maybeReplaceElement.Excel_WriteContent2.Replace(key, value);

                    maybeReplaceElement.Excel_InsertPicture = maybeReplaceElement.Excel_InsertPicture.Replace(key, value);
                    maybeReplaceElement.Excel_InsertHyperlinks = maybeReplaceElement.Excel_InsertHyperlinks.Replace(key, value);

                    maybeReplaceElement.EqualValue = maybeReplaceElement.EqualValue.Replace(key, value);

                    maybeReplaceElement.ForeachLoop_StartIndexStr = maybeReplaceElement.ForeachLoop_StartIndexStr.Replace(key, value);
                    maybeReplaceElement.ForeachLoop_CountStr = maybeReplaceElement.ForeachLoop_CountStr.Replace(key, value);
                    maybeReplaceElement.ForeachByFileLine_FileName = maybeReplaceElement.ForeachByFileLine_FileName.Replace(key, value);
                }
            }
            SystemFunctions.Replace(ref maybeReplaceElement);
            maybeReplaceElement.Excel_InsertHyperlinks = maybeReplaceElement.Excel_InsertHyperlinks.Replace("@ToolRootPath@", AppDomain.CurrentDomain.BaseDirectory);
            maybeReplaceElement.Excel_InsertPicture = maybeReplaceElement.Excel_InsertPicture.Replace("@ToolRootPath@", AppDomain.CurrentDomain.BaseDirectory);
        }
        private MaybeReplaceElement CreateMaybeReplaceElement(XmlScpiCmd cmd)
        {
            return new()
            {
                Description = cmd.Description,
                SCPICmd = cmd.SCPICmd,
                EqualValue = cmd.EqualValue,
                ProgramFuncName = cmd.ProgramFuncName,
                ForeachLoop_CountStr = cmd.ForeachLoop_CountStr,
                ForeachLoop_StartIndexStr = cmd.ForeachLoop_StartIndexStr,
                ForeachByFileLine_FileName = cmd.ForeachByFileLine_FileName,
                Excel_WriteAtCell = cmd.Excel_WriteAtCell,
                Excel_WriteContent = cmd.Excel_WriteContent,
                BooleanCheckFormula = cmd.BooleanCheckFormula,
                Excel_WriteAtCell2 = cmd.Excel_WriteAtCell2,
                Excel_WriteContent2 = cmd.Excel_WriteContent2,
                BooleanCheckFormula2 = cmd.BooleanCheckFormula2,

                Excel_InsertPicture = cmd.Excel_InsertPicture,
                Excel_InsertHyperlinks = cmd.Excel_InsertHyperlinks,
            };
        }
        private BatchTaskState ProcessOneCommand(XmlScpiCmd cmd, bool bSimulatedExecute = false)
        {
            MaybeReplaceElement maybeReplaceElement = CreateMaybeReplaceElement(cmd);
            ReplaceVarint(ref maybeReplaceElement);
            if (cmd.CmdType == XmlScpiCmdType.ForeachLoop)
            {
                int startIndex = int.Parse(maybeReplaceElement.ForeachLoop_StartIndexStr);
                int count = int.Parse(maybeReplaceElement.ForeachLoop_CountStr);

                RuntimeForeachXmlScpiCmd.Add(cmd);
                RuntimeForeachNowIndex.Add(startIndex);
                for (int loopIndex = startIndex; loopIndex < startIndex + count; loopIndex++)
                {
                    RuntimeForeachNowIndex[RuntimeForeachNowIndex.Count - 1] = loopIndex;
                    List<XmlScpiCmd> includeCommands = RuntimeForeachXmlScpiCmd[RuntimeForeachXmlScpiCmd.Count - 1].LoopXmlScpiCmds;
                    foreach (XmlScpiCmd xmlScpiCmd in includeCommands)
                    {
                        if (ProcessOneCommand(xmlScpiCmd, bSimulatedExecute) == BatchTaskState.Canceled)
                            return BatchTaskState.Canceled;
                        if (cancelTokenSrc != null)
                        {
                            try
                            {
                                cancelTokenSrc.Token.ThrowIfCancellationRequested();
                            }
                            catch
                            {
                                return BatchTaskState.Canceled;
                            }
                        }
                    }

                }
                RuntimeForeachNowIndex.RemoveAt(RuntimeForeachNowIndex.Count - 1);
                RuntimeForeachXmlScpiCmd.RemoveAt(RuntimeForeachXmlScpiCmd.Count - 1);
                stepIndex++;
                return BatchTaskState.FinishedOK;
            }
            if (cmd.CmdType == XmlScpiCmdType.ForeachByFileLine)
            {
                PreprocessForeachByFileLine(cmd, maybeReplaceElement.ForeachByFileLine_FileName);

                RuntimeForeachXmlScpiCmd.Add(cmd);
                RuntimeForeachNowIndex.Add(0);

                for (int loopIndex = 0; loopIndex < cmd.ForeachByFileLine_LineCount; loopIndex++)
                {
                    RuntimeForeachNowIndex[RuntimeForeachNowIndex.Count - 1] = loopIndex;
                    List<XmlScpiCmd> includeCommands = RuntimeForeachXmlScpiCmd[RuntimeForeachXmlScpiCmd.Count - 1].LoopXmlScpiCmds;

                    foreach (XmlScpiCmd xmlScpiCmd in includeCommands)
                    {
                        if (ProcessOneCommand(xmlScpiCmd, bSimulatedExecute) == BatchTaskState.Canceled)
                            return BatchTaskState.Canceled;
                        if (cancelTokenSrc != null)
                        {
                            try
                            {
                                cancelTokenSrc.Token.ThrowIfCancellationRequested();
                            }
                            catch
                            {
                                return BatchTaskState.Canceled;
                            }
                        }
                    }

                }
                RuntimeForeachNowIndex.RemoveAt(RuntimeForeachNowIndex.Count - 1);
                RuntimeForeachXmlScpiCmd.RemoveAt(RuntimeForeachXmlScpiCmd.Count - 1);

                stepIndex++;
                return BatchTaskState.FinishedOK;
            }

            stepIndex++;
            if (bSimulatedExecute)
                return BatchTaskState.FinishedOK;

            updateAction?.Invoke(stepIndex, $"正在处理{maybeReplaceElement.Description}...", "");
            if (cancelTokenSrc != null)
            {
                try
                {
                    cancelTokenSrc.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    return BatchTaskState.Canceled;
                }
            }
            string scanStr = "";

            if (cmd.bIsProgramFunc)
            {
                if (maybeReplaceElement.ProgramFuncName.Trim() != "")
                {
                    XmlScpiCmd partCmd = new XmlScpiCmd();
                    partCmd.AfterWaitSec = cmd.AfterWaitSec;
                    partCmd.bIsProgramFunc = cmd.bIsProgramFunc;
                    partCmd.BooleanCheckFormula = maybeReplaceElement.BooleanCheckFormula;
                    partCmd.Excel_WriteContent = maybeReplaceElement.Excel_WriteContent;
                    partCmd.Excel_WriteAtCell = maybeReplaceElement.Excel_WriteAtCell;

                    partCmd.BooleanCheckFormula2 = maybeReplaceElement.BooleanCheckFormula2;
                    partCmd.Excel_WriteContent2 = maybeReplaceElement.Excel_WriteContent2;
                    partCmd.Excel_WriteAtCell2 = maybeReplaceElement.Excel_WriteAtCell2;

                    partCmd.bQueryWaitWhenNotEmpty = cmd.bQueryWaitWhenNotEmpty;
                    partCmd.CmdType = cmd.CmdType;
                    partCmd.Description = maybeReplaceElement.Description;
                    partCmd.EqualValue = maybeReplaceElement.EqualValue;
                    partCmd.InstrumentationName = cmd.InstrumentationName;
                    partCmd.ProgramFuncName = maybeReplaceElement.ProgramFuncName;
                    partCmd.ProgramFuncOverSec = cmd.ProgramFuncOverSec;
                    partCmd.QueryOvertimeSec = cmd.QueryOvertimeSec;
                    partCmd.SCPICmd = maybeReplaceElement.SCPICmd;
                    if (cmd.CmdType != XmlScpiCmdType.UIFunction)
                    {
                        IBatchTaskPart? batchTaskPart = BatchTaskPartFactory.Create(partCmd, null);

                        if (batchTaskPart != null)
                        {
                            batchTaskPart.SetInstrument(ourInstrument!);
                            BatchTaskPartResult batchTaskPartResult = batchTaskPart.Exec(cmd.ProgramFuncOverSec, out string resultMsg, cancelTokenSrc);
                            if (batchTaskPartResult == BatchTaskPartResult.Cancel)
                            {
                                state = BatchTaskState.Canceled;
                                ExcelAction.Default.Close();
                                return state;
                            }
                            totalErrorCount += batchTaskPart.ErrorCount;
                            updateAction?.Invoke(
                                stepIndex,
                                $"{currSubXmlFilename};{partCmd.ProgramFuncName};{maybeReplaceElement.Description};{batchTaskPartResult}",
                                $"[{maybeReplaceElement.Description}]的处理结果为:{resultMsg}");
                            if (partCmd.AfterWaitSec > 0)
                                Thread.Sleep((int)(partCmd.AfterWaitSec * 1000));
                        }
                    }
                    else
                    {
                        string[] programFuncNameAndParamterss = partCmd.ProgramFuncName.Split(' ');
                        if (programFuncNameAndParamterss.Length > 0 && programFuncNameAndParamterss[0].Trim() != "")
                        {
                            ProcessXMLAndPartTask_UIFunc? processXMLAndPartTask_UIFunc = BaseHelper.ProcessXMLAndPartTask_UIFuncList.FirstOrDefault(v => v.GetMethodInfo().Name == programFuncNameAndParamterss[0].Trim());
                            if (processXMLAndPartTask_UIFunc != null)
                            {
                                IInstrumentSession? instrumentSession = GetInstrumentSessionByName(partCmd.InstrumentationName);
                                BatchTaskPartResult batchTaskPartResult = processXMLAndPartTask_UIFunc!.Invoke(partCmd, instrumentSession, out string resultMsg, updateAction, cancelTokenSrc?.Token ?? null);
                                if (batchTaskPartResult == BatchTaskPartResult.Cancel)
                                {
                                    state = BatchTaskState.Canceled;
                                    ExcelAction.Default.Close();
                                    return state;
                                }
                                totalErrorCount += (batchTaskPartResult != BatchTaskPartResult.Succeed && batchTaskPartResult != BatchTaskPartResult.Cancel) ? 1 : 0;
                                updateAction?.Invoke(
                                    stepIndex,
                                    $"{currSubXmlFilename};{partCmd.ProgramFuncName};{maybeReplaceElement.Description};{batchTaskPartResult}",
                                    $"[{maybeReplaceElement.Description}]的处理结果为:{resultMsg}");
                                if (partCmd.AfterWaitSec > 0)
                                    Thread.Sleep((int)(partCmd.AfterWaitSec * 1000));
                            }
                        }
                        IInstrumentSession? instrumentSession_tmp = GetInstrumentSessionByName(partCmd.InstrumentationName);
                        IBatchTaskPart? batchTaskPart = BatchTaskPartFactory.Create(partCmd, null);

                        if (batchTaskPart != null)
                        {
                            batchTaskPart.SetInstrument(ourInstrument!);
                            BatchTaskPartResult batchTaskPartResult = batchTaskPart.Exec(cmd.ProgramFuncOverSec, out string resultMsg, cancelTokenSrc);
                            if (batchTaskPartResult == BatchTaskPartResult.Cancel)
                            {
                                state = BatchTaskState.Canceled;
                                ExcelAction.Default.Close();
                                return state;
                            }
                            totalErrorCount += batchTaskPart.ErrorCount;
                            updateAction?.Invoke(
                                stepIndex,
                                $"{currSubXmlFilename};{partCmd.ProgramFuncName};{maybeReplaceElement.Description};{batchTaskPartResult}",
                                $"[{maybeReplaceElement.Description}]的处理结果为:{resultMsg}");
                            if (partCmd.AfterWaitSec > 0)
                                Thread.Sleep((int)(partCmd.AfterWaitSec * 1000));
                        }
                    }
                }
            }
            else if (cmd.CmdType == XmlScpiCmdType.Check)
            {
                if (MessageBox.Show(maybeReplaceElement.Description, "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    state = BatchTaskState.Canceled;
                    ExcelAction.Default.Close();
                    return state;
                }
            }
            else if (cmd.CmdType == XmlScpiCmdType.SCPI)
            {
                SpecialWriteString(cmd.InstrumentationName, maybeReplaceElement.SCPICmd, cmd.AfterWaitSec);
                if (cmd.SCPICmd.IndexOf('?') >= 0)
                {
                    scanStr = SpecialReadString(cmd.InstrumentationName);
                    if (scanStr.Trim() == "")
                    {
                        if (cmd.bQueryWaitWhenNotEmpty)
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            while (scanStr.Trim() == "" || stopwatch.ElapsedMilliseconds < 1000 * 50)
                            {
                                //Application.DoEvents();
                                scanStr = SpecialReadString(cmd.InstrumentationName);
                            }
                        }
                    }
                    if (cmd.EqualValue != "")
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (maybeReplaceElement.EqualValue != scanStr.ToUpper() || stopwatch.ElapsedMilliseconds < cmd.QueryOvertimeSec * 1000)
                        {
                            //Application.DoEvents();
                            scanStr = SpecialReadString(cmd.InstrumentationName);
                        }
                        if (cmd.EqualValue != scanStr.ToUpper())
                        {
                            updateAction?.Invoke(stepIndex, "", $"{maybeReplaceElement.Description}错误返回。期望返回[{maybeReplaceElement.EqualValue}],实际返回[{scanStr}]");
                        }
                        else
                            updateAction?.Invoke(stepIndex, "", $"{maybeReplaceElement.Description}正确返回,返回[{scanStr}]");
                    }
                    else
                    {

                    }
                }
                else
                {
                    SpecialWriteString(cmd.InstrumentationName, maybeReplaceElement.SCPICmd, cmd.AfterWaitSec);
                    updateAction?.Invoke(stepIndex, $"{maybeReplaceElement.Description}完毕", $"{maybeReplaceElement.Description}完毕");
                }
            }

            if (cmd.Excel_WriteAtCell != "")
            {
                if (cmd.Excel_WriteContent == "@scpi_result@")
                {
                    if (scanStr.Trim() != "")
                        ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell, scanStr);
                }
                else if (cmd.Excel_WriteContent == "@bool_formula_result@" && cmd.BooleanCheckFormula != "")
                {
                    if (scanStr.Trim() != "")
                    {
                        bool boolCheckResult = BaseHelper.CalcBooleanFormula(maybeReplaceElement.BooleanCheckFormula, scanStr);
                        ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell, boolCheckResult ? "√" : "×");
                        string descriptionMssage = $"{maybeReplaceElement.Description}正确返回.返回{scanStr},逻辑检查结果为{boolCheckResult}";
                    }
                }
                else
                    ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell, maybeReplaceElement.Excel_WriteContent);
            }
            if (cmd.Excel_WriteAtCell2 != "")
            {
                if (cmd.Excel_WriteContent2 == "@scpi_result@")
                {
                    if (scanStr.Trim() != "")
                        ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell2, scanStr);
                }
                else if (cmd.Excel_WriteContent2 == "@bool_formula_result@" && cmd.BooleanCheckFormula2 != "")
                {
                    if (scanStr.Trim() != "")
                    {
                        bool boolCheckResult = BaseHelper.CalcBooleanFormula(maybeReplaceElement.BooleanCheckFormula2, scanStr);
                        ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell2, boolCheckResult ? "√" : "×");
                        string descriptionMssage = $"{maybeReplaceElement.Description}正确返回.返回{scanStr},逻辑检查结果为{boolCheckResult}";
                        updateAction?.Invoke(stepIndex, "", descriptionMssage);
                    }
                }
                else
                    ExcelAction.Default.WriteCell(maybeReplaceElement.Excel_WriteAtCell2, maybeReplaceElement.Excel_WriteContent2);
            }
            if (cmd.Excel_InsertPicture != "")
            {
                string[] paramList = maybeReplaceElement.Excel_InsertPicture.Split(',');
                if (paramList.Length == 4)
                {
                    string fileName = paramList[0];
                    if (fileName.StartsWith('.'))
                        fileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    if (File.Exists(fileName))
                    {
                        float picWidth = float.Parse(paramList[2]);
                        float picHeight = float.Parse(paramList[3]);
                        ExcelAction.Default.InsertPicture(fileName, paramList[1], picWidth, picHeight);
                    }
                }
            }
            if (cmd.Excel_InsertHyperlinks != "")
            {
                string[] paramList = maybeReplaceElement.Excel_InsertHyperlinks.Split(',');
                if (paramList.Length == 3)
                {
                    ExcelAction.Default.InsertHyperlinks(paramList[0]/*cell*/, paramList[1]/*title*/, paramList[2]/*Hyperlinks*/);
                }
            }
            return BatchTaskState.FinishedOK;
        }
        private int totalErrorCount = 0;
        int stepIndex = 0;
        private List<XmlScpiCmd> RuntimeForeachXmlScpiCmd = new List<XmlScpiCmd>();
        private List<int> RuntimeForeachNowIndex = new List<int>();
        private Dictionary<int/*Level Index*/, Dictionary<string, List<string>>> RuntimeForeachVariantDefine = new Dictionary<int, Dictionary<string, List<string>>>();
        protected override void TaskBody()
        {
            state = BatchTaskState.Running;
            stepIndex = 0;
            totalErrorCount = 0;
            foreach (string xmlFile in batchXmlFlieList)
            {
                updateAction?.Invoke(stepIndex, "", $"正在处理文件:{xmlFile}");
                currSubXmlFilename = xmlFile;

                AnalyOneXmlFile(xmlFile);
                OtherInstruments.Clear();
                foreach (KeyValuePair<string, InstrumentDefine> kvp in OtherInstrumentsDefine)
                {
                    string message = "";
                    IInstrumentSession? instrument = InstrumentSessionEngine.TryGetSession(kvp.Value.Address, "20", null, out message);
                    if (instrument == null)
                        return;
                    OtherInstruments.Add(kvp.Key, instrument);
                }
                if (ExcelFile != "")
                    ExcelAction.Default.OpenFile(ExcelFile);
                //ForeachLoop只能在一个XML文件中存在
                RuntimeForeachXmlScpiCmd.Clear();
                RuntimeForeachNowIndex.Clear();
                foreach (XmlScpiCmd cmd in allScpiCmd)
                {
                    if (ProcessOneCommand(cmd) == BatchTaskState.Canceled)
                    {
                        state = BatchTaskState.Canceled;
                        ExcelAction.Default.Close();
                        return;
                    }
                    if (cancelTokenSrc != null)
                    {
                        try
                        {
                            cancelTokenSrc.Token.ThrowIfCancellationRequested();
                        }
                        catch
                        {
                            state = BatchTaskState.Canceled;
                            ExcelAction.Default.Close();
                            return;
                        }
                    }
                }
                if (totalErrorCount == 0)
                {
                    if (ExcelFile != "")
                        resultTipMessage = $"该任务全部成功！结果文件为：[{ExcelFile}]";
                    else
                        resultTipMessage = "该任务全部成功！";
                    state = BatchTaskState.FinishedOK;
                }
                else
                {
                    if (ExcelFile != "")
                        resultTipMessage = $"该任务完成，但有{totalErrorCount}个错误。结果文件为：[{ExcelFile}]";
                    else
                        resultTipMessage = $"该任务完成，但有{totalErrorCount}个错误。";
                    state = BatchTaskState.FinishedFailed;
                }
                ExcelAction.Default.Close();
            }
        }
    }
    #region Other Class
    public enum XmlScpiCmdType
    {
        Unknow,
        Check,
        SCPI,
        ForeachLoop,
        ForeachByFileLine,
        UIFunction,
    }
    public class XmlScpiCmd
    {
        public XmlScpiCmdType CmdType
        {
            get;
            set;
        } = XmlScpiCmdType.Unknow;
        public bool bIsProgramFunc
        {
            get;
            set;
        } = false;
        public string ProgramFuncName
        {
            get;
            set;
        } = "";
        public int ProgramFuncStepCount
        {
            get;
            set;
        } = 1;
        public double ProgramFuncOverSec
        {
            get;
            set;
        } = 0;
        public String Description
        {
            get;
            set;
        } = "";
        public string InstrumentationName
        {
            get;
            set;
        } = "";
        public string SCPICmd
        {
            get;
            set;
        } = "";
        public double AfterWaitSec
        {
            get;
            set;
        } = 0;
        public string EqualValue
        {
            get;
            set;
        } = "";
        public double QueryOvertimeSec
        {
            get;
            set;
        } = 0;
        public bool bQueryWaitWhenNotEmpty
        {
            get;
            set;
        } = false;
        /// <summary>
        /// 约定：1、scpi读回结果:@scpi_result@
        ///       2、系统函数：如@Datetime.Now.Date(.)@,@Datetime.Now.Date(-)@,@Datetime.Now.Time()@等
        ///       3、逻辑表达式结果：@bool_formula_result@
        ///       3、固定内容
        /// </summary>
        public string Excel_WriteContent
        {
            get;
            set;
        } = "";
        public string Excel_WriteAtCell
        {
            get;
            set;
        } = "";
        public string BooleanCheckFormula
        {
            get;
            set;
        } = "";
        /// <summary>
        /// 约定：1、scpi读回结果:@scpi_result@
        ///       2、系统函数：如@Datetime.Now.Date(.)@,@Datetime.Now.Date(-)@,@Datetime.Now.Time()@等
        ///       3、逻辑表达式结果：@bool_formula_result@
        ///       3、固定内容
        /// </summary>
        public string Excel_WriteContent2
        {
            get;
            set;
        } = "";
        public string Excel_WriteAtCell2
        {
            get;
            set;
        } = "";
        public string BooleanCheckFormula2
        {
            get;
            set;
        } = "";
        //-----------------------
        public string Excel_InsertPicture
        {
            get;
            set;
        } = "";
        public string Excel_InsertHyperlinks
        {
            get;
            set;
        } = "";

        public List<XmlScpiCmd> LoopXmlScpiCmds = new List<XmlScpiCmd>();
        #region ForeachLoop
        public Dictionary<string, List<string>> ForeachLoop_VariantDefine = new Dictionary<string, List<string>>();
        public string ForeachLoop_StartIndexStr
        {
            get;
            set;
        } = "";
        public string ForeachLoop_CountStr
        {
            get;
            set;
        } = "";
        public int ForeachLoop_StartIndex
        {
            get;
            set;
        } = 0;
        public int ForeachLoop_Count
        {
            get;
            set;
        } = 0;
        #endregion

        #region ForeachByFileLine
        public string ForeachByFileLine_FileName
        {
            get;
            set;
        } = "";
        public string ForeachByFileLine_FileID
        {
            get;
            set;
        } = "";

        public int ForeachByFileLine_LineCount = 0;
        public int ForeachByFileLine_ColumnCount = 0;
        public List<List<string>> ForeachByFileLine_FileColumnRows = new List<List<string>>();
        #endregion

    }
    class InstrumentDefine
    {
        public string Name
        {
            get;
            set;
        } = "";
        public string Description
        {
            get;
            set;
        } = "";
        public string Address
        {
            get;
            set;
        } = "";
        public bool bIsOurTarget
        {
            get;
            set;
        } = false;
    }
    class MaybeReplaceElement
    {
        public string Description = "";
        public string SCPICmd = "";
        public string Excel_WriteAtCell = "";
        public string Excel_WriteContent = "";
        public string BooleanCheckFormula = "";

        public string Excel_WriteAtCell2 = "";
        public string Excel_WriteContent2 = "";
        public string BooleanCheckFormula2 = "";

        public string Excel_InsertPicture = "";
        public string Excel_InsertHyperlinks = "";
        public string EqualValue = "";
        public string ProgramFuncName = "";

        public string ForeachLoop_StartIndexStr = "";
        public string ForeachLoop_CountStr = "";

        public string ForeachByFileLine_FileName = "";
        public override string ToString()
        {
            return $"{Description}{SCPICmd}{EqualValue}{BooleanCheckFormula}{Excel_WriteAtCell}{Excel_WriteContent}{BooleanCheckFormula2}{Excel_WriteAtCell2}{Excel_WriteContent2}{Excel_InsertPicture}{Excel_InsertHyperlinks}{ProgramFuncName}{ForeachLoop_StartIndexStr}{ForeachLoop_CountStr}{ForeachByFileLine_FileName}";
        }
    }
    static class SystemFunctions
    {
        static Dictionary<string, Func<String>> funcions = new Dictionary<string, Func<String>>()
        {
            ["Datetime.Now.Date(.)"] = GetNowDateStr_WithDot,
            ["Datetime.Now.Date(-)"] = GetNowDateStr_WithBridge,
            ["Datetime.Now.Time()"] = GetNowTimeStr,
        };
        static string GetNowDateStr_WithDot()
        {
            return DateTime.Now.ToString("yyyy.MM.dd");
        }
        static string GetNowDateStr_WithBridge()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        static string GetNowTimeStr()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
        static internal void Replace(ref MaybeReplaceElement maybeReplaceElement)
        {
            if (maybeReplaceElement.ToString().IndexOf('@') < 0)
                return;
            foreach (var variant in funcions)
            {
                string key = $"@{variant.Key}@";
                string value = variant.Value.Invoke();
                maybeReplaceElement.ProgramFuncName = maybeReplaceElement.ProgramFuncName.Replace(key, value);
                maybeReplaceElement.Description = maybeReplaceElement.Description.Replace(key, value);
                maybeReplaceElement.SCPICmd = maybeReplaceElement.SCPICmd.Replace(key, value);
                maybeReplaceElement.Excel_WriteAtCell = maybeReplaceElement.Excel_WriteAtCell.Replace(key, value);
                maybeReplaceElement.Excel_WriteContent = maybeReplaceElement.Excel_WriteContent.Replace(key, value);
                maybeReplaceElement.BooleanCheckFormula = maybeReplaceElement.BooleanCheckFormula.Replace(key, value);

                maybeReplaceElement.Excel_WriteAtCell2 = maybeReplaceElement.Excel_WriteAtCell2.Replace(key, value);
                maybeReplaceElement.Excel_WriteContent2 = maybeReplaceElement.Excel_WriteContent2.Replace(key, value);
                maybeReplaceElement.BooleanCheckFormula2 = maybeReplaceElement.BooleanCheckFormula2.Replace(key, value);

                maybeReplaceElement.Excel_InsertPicture = maybeReplaceElement.Excel_InsertPicture.Replace(key, value);
                maybeReplaceElement.Excel_InsertHyperlinks = maybeReplaceElement.Excel_InsertHyperlinks.Replace(key, value);

                maybeReplaceElement.EqualValue = maybeReplaceElement.EqualValue.Replace(key, value);
                maybeReplaceElement.ForeachLoop_StartIndexStr = maybeReplaceElement.ForeachLoop_StartIndexStr.Replace(key, value);
                maybeReplaceElement.ForeachLoop_CountStr = maybeReplaceElement.ForeachLoop_CountStr.Replace(key, value);
                maybeReplaceElement.ForeachByFileLine_FileName = maybeReplaceElement.ForeachByFileLine_FileName.Replace(key, value);
            }
        }
    }
    #endregion
}
