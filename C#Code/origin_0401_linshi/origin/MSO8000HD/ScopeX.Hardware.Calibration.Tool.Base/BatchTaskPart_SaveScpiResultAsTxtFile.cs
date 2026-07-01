using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    internal class BatchTaskPart_SaveScpiResultAsTxtFile : BatchTaskPartBase
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
            get => $"保存文本格式的波形数据。";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数，ScpiCommandStr。SCPI 命令。{System.Environment.NewLine}" +
                   $"第2个参数，DelaySecondBeforeRead。读之前需要等到的时间，以秒为单位的浮点数。{System.Environment.NewLine}" +
                   $"第3个参数，SaveFileName。保存的文本文件的路径及名称。 {System.Environment.NewLine}";
        }
        public override string Example
        {
            get => "";
        }
        private string ScpiCommandStr ="";
        private double DelaySecondBeforeRead = 100;
        private string SaveFileName = "";
        private bool AnalyParameter(string parameter)
        {
            if (parameter == "")
                return false;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 3)
                return false;
            //1.scpi 命令
            string tmpStr = paramList[0];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            ScpiCommandStr = tmpStr;
            if (ScpiCommandStr.Trim() == "")
                return false;
            //2.读之前需要等到的时间，以秒为单位的浮点数。
            tmpStr = paramList[1];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            DelaySecondBeforeRead = double.Parse(tmpStr);
            //3.保存文件的路径及名称
            tmpStr = paramList[2];
            if (tmpStr.IndexOf('=') > 0)
                tmpStr = tmpStr.Split('=')[1];
            SaveFileName=tmpStr;
            if (SaveFileName.Trim()=="")
                return false;
            if (SaveFileName[0] == '.')
                SaveFileName= AppDomain.CurrentDomain.BaseDirectory + SaveFileName;
            return true;
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            message = "";
            currInstrumentSession!.WriteString(ScpiCommandStr);
            Thread.Sleep((int)(DelaySecondBeforeRead*1000));
            string readbackStr=currInstrumentSession.ReadString();
            string? path = Path.GetDirectoryName(SaveFileName)!;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            FileStream fs = new FileStream(SaveFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(readbackStr);
            sw.Close();
            fs.Close();
            batchTaskPartResult = BatchTaskPartResult.Succeed;
            message = $"OK!文件保存为{SaveFileName}";
            return batchTaskPartResult;
        }
    }
}
