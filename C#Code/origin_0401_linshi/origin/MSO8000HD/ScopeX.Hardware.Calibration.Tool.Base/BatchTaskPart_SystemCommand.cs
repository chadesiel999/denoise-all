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
    public class BatchTaskPart_SystemCommand : BatchTaskPartBase
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
        public override string FuncionDescription
        {
            get => $"执行系统命令。{System.Environment.NewLine}。";
        }
        public override string ParametersDescription
        {
            get => $"第一个参数，windows命令。可以用@ToolRootPath@ 表示当前工具所在的路径为起始路径。";
        }
        public override string Example
        {
            get => "";
        }
        //参数样式为：
        //第一个参数，命令
        private string command = "";
        private bool AnalyParameter(string parameter)
        {
            if (parameter == "")
                return false;
            parameter=parameter.Trim();
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 1)
                return false;
            command = paramList[0].Trim();
            if (command.IndexOf('=') > 0)
                command = command.Split('=')[1];
            return true;
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            message = "";
            if (command.IndexOf("@ToolRootPath@") >= 0)
                command = command.Replace("@ToolRootPath@", AppDomain.CurrentDomain.BaseDirectory);
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "tmp.bat";
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, command);
            Process p = new();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = "";
            p.Start();
            p.WaitForExit();
            message = $"OK!";
            batchTaskPartResult = BatchTaskPartResult.Succeed;
            return batchTaskPartResult;
        }
    }
}
