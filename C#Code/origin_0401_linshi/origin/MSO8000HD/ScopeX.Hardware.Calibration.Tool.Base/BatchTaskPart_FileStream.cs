using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.ComModel;
using System.Threading;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class BatchTaskPart_TxtFileStream : BatchTaskPartBase
    {
        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd?.ProgramFuncName.Trim() ?? "");
            if (myName_ParameterPair != null)
                return AnalyParameter(myName_ParameterPair[1]);
            else
                return false;
        }

        public static void ForceCloseFile()
        {
            foreach (var kvp in ExistsFiles)
            {
                kvp.Value.Writer.Close();
                kvp.Value.Stream.Close();
            }
            ExistsFiles.Clear();
        }
        private static Dictionary<string, (FileStream Stream, StreamWriter Writer)> ExistsFiles = new Dictionary<string, (FileStream Stream, StreamWriter Writer)>();



        public override string FuncionDescription
        {
            get => $"文本文件流操作，包括创建、写入和关闭。{System.Environment.NewLine}" +
                    $"其流程包括：先创建，然后多次写入，最后关闭！";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数，FileName：文件名称。包括路径及文件名称。路径可以是绝对路径；也可以是用@ToolRootPath@表示的当前执行工具exe所在的相对路径。{System.Environment.NewLine}" +
                    $"第2个参数，CurrOperation：操作，包括Create,Write,Close{System.Environment.NewLine}" +
                    $"第3个参数，WriteLineContent：写时每行的内容。可以是直接的内容，也可以是用@MeasureItem1@、@MeasureItem2@...@MeasureItem8@表示的第几个参数测量的值。 ";
        }
        public override string Example
        {
            get => "";
        }
        private string FileName = "";
        private string CurrOperation = "";
        private string WriteLineContent = "";
        private bool AnalyParameter(string parameter)
        {
            if (parameter == "")
                return false;
            string[] paramList = parameter.Split(',');
            if (paramList.Length < 2)
                return false;
            //1.文件名称
            string tmpStr = paramList[0];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            FileName = tmpStr.Trim();
            //2.操作，包括Create,Write,Close
            tmpStr = paramList[1];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];

            CurrOperation = tmpStr.Trim();
            //3.WriteLineContent
            if (CurrOperation.ToLower() == "write")
            {
                if (paramList.Length < 3)
                    return false;
                tmpStr = paramList[2];
                if (tmpStr.IndexOf('=') > 0)
                    tmpStr = tmpStr.Split('=')[1];
                WriteLineContent = tmpStr;
            }
            if (CurrOperation.ToLower() == "create")
            {
                if (ExistsFiles.ContainsKey(FileName))
                {
                    ExistsFiles[FileName].Writer.Close();
                    ExistsFiles[FileName].Stream.Close();
                    ExistsFiles.Remove(FileName);
                }
            }
            if (CurrOperation.ToLower() == "write" || CurrOperation.ToLower() == "close")
            {
                if (!ExistsFiles.ContainsKey(FileName))
                    return false;
            }

            return true;
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            if (CurrOperation.ToLower() == "create")
            {
                string fileName = FileName;
                if (FileName[0] == '.')
                    fileName = AppDomain.CurrentDomain.BaseDirectory + FileName;
                if (File.Exists(fileName))
                    File.Delete(fileName);
                string? path = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(path!))
                    Directory.CreateDirectory(path!);
                FileStream currFileStream = new FileStream(fileName, FileMode.Create);
                StreamWriter currStreamWriter = new StreamWriter(currFileStream);
                ExistsFiles.Add(FileName, (currFileStream, currStreamWriter));
            }
            else if (CurrOperation.ToLower() == "close")
            {
                if (ExistsFiles.ContainsKey(FileName))
                {
                    ExistsFiles[FileName].Writer.Close();
                    ExistsFiles[FileName].Stream.Close();
                    ExistsFiles.Remove(FileName);
                }
            }
            else
            {
                List<int> MeasureItems = new List<int>();
                for (int measureID = 1; measureID <= 9; measureID++)
                {
                    if (WriteLineContent.IndexOf($"@MeasureItem{measureID}@") >= 0)
                        MeasureItems.Add(measureID);
                }
                string writeLineContent = WriteLineContent;
                foreach (int measureID in MeasureItems)
                {
                    string measureResultStr = "";
                    while (measureResultStr.Trim() == "")
                    {
                        currInstrumentSession!.WriteString($":MEASure:ITEM{measureID}:VAL?");
                        measureResultStr = currInstrumentSession!.ReadString();
                    }
                    writeLineContent = writeLineContent.Replace($"@MeasureItem{measureID}@", measureResultStr);
                }
                writeLineContent = BaseHelper.ReplaceESCChar(writeLineContent);
                ExistsFiles[FileName].Writer!.WriteLine(writeLineContent);
            }
            batchTaskPartResult = BatchTaskPartResult.Succeed;
            message = $"OK!";
            return batchTaskPartResult;
        }
    }
}
