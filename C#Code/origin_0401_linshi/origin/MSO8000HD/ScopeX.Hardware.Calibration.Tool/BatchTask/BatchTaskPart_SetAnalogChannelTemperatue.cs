using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_SetAnalogChannelTemperatue:BatchTaskPartBase
    {
        public override string FuncionDescription
        {
            get => $"获取并设置模拟通道的温度数据到当前模拟通道的校准数据中";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数：温度用于何种用途。有Gain和Baseline 两个选项";
        }
        public override string Example
        {
            get => "BatchTaskPart_SetAnalogChannelTemperatue Gain";
        }
        private string usedForWhat = "GAIN";
        private bool AnalyParameter(string parameter)
        {
            if (parameterStr == parameter)
                return false;

            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 1)
            {
                return false;
            }
            string _usedForWhat = paramList[0].Trim();
            if (_usedForWhat != "")
                usedForWhat = _usedForWhat.ToUpper();
            return true;
        }
        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            message = "Ok!";
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $"? GetPhyChannelTemperatures{1}";
            currInstrumentSession!.WriteString(scpiCmd);
            Thread.Sleep(100);
            string readBack = currInstrumentSession.ReadString();
            if (readBack != "")
            {
                switch(usedForWhat.ToUpper())
                {
                    case "Gain":
                        ChannelParams.Default.TemperatureAtCaliGain_mCelsius = (Int32)(double.Parse(readBack) * 1000);
                        break;
                    case "BASELINE":
                        ChannelParams.Default.TemperatureAtCaliBaseline_mCelsius = (Int32)(double.Parse(readBack) * 1000);
                        break ;
                }
            }
            return BatchTaskPartResult.Succeed;
        }
    }
}   