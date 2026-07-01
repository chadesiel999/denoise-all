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
    public class BatchTaskPart_GetScreentShot : BatchTaskPartBase
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
            get => $"示波器界面截屏。";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数，FileName：截屏保存的文件的路径及名称。包括路径及文件名称。路径可以是绝对路径；也可以是用@ToolRootPath@表示的当前执行工具exe所在的相对路径。{System.Environment.NewLine}";
        }
        public override string Example
        {
            get => "";
        }
        private string FileName = "";
        private bool AnalyParameter(string parameter)
        {
            if (parameter == "")
                return false;
            string[] paramList = parameter.Split(',');
            if (paramList.Length < 1)
                return false;
            //1.文件名称
            string tmpStr = paramList[0];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            FileName = tmpStr.Trim();
            return true;
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            InstrumentInteract.Factory_GetScreenJpeg(currInstrumentSession,FileName);
            batchTaskPartResult = BatchTaskPartResult.Succeed;
            message = $"OK!";
            return batchTaskPartResult;
        }
    }
}
