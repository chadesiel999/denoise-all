using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.Diagnostics.Metrics;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask
{
    public class BatchTaskPart_SaveCaliDataToFile : BatchTaskPartBase
    {
        public override string FuncionDescription
        {
            get => $"保存指定校准数据.";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数：校准数据类型,包括TiAdc_PhaseOffsetGain,TiAdc_SyncSampleClock,PhyChannel,PhyChannelModel2,Misc,CoefficientsTables,DbiAnalogParams,DbiCoefficientsTables,DbiLocalOscillators" + System.Environment.NewLine +
                $"第二个参数：如果是系数表，使用系数名称，如Coefficients1、Coefficients2、Coefficients3";
        }
        public override string Example
        {
            get => "BatchTaskPart_SaveCaliDataToFile PhyChannel";
        }
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
            string typeName = paramList[0].Trim();
            foreach (var v in Enum.GetValues(typeof(CaliDataType)))
            {
                if (v.ToString() == typeName)
                {
                    currCaliDataType = (CaliDataType)v;
                    break;
                }
            }
            if (paramList.Length >= 2)
            {
                coefficientsTableType = paramList[1].Trim();
            }
            return true;
        }
        CaliDataType currCaliDataType = CaliDataType.None;
        string coefficientsTableType = "1";
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
            InstrumentInteract.CaliData_Send(currInstrumentSession, currCaliDataType);
            InstrumentInteract.CaliData_SaveData(currInstrumentSession, currCaliDataType);

            string cmdStr = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData) + " CaliDataChanged," + currCaliDataType;


            if (currCaliDataType == CaliDataType.CoefficientsTables)
            {
                cmdStr = $"{cmdStr},{coefficientsTableType}";
            }
            //else if (currCaliDataType== CaliDataType.DbiCoefficientsTables)
            //{
            //    cmdStr = $"{cmdStr} DbiCoefficientsTables,{(int)coefficientsTableType}";
            //}

            bool bOK = currInstrumentSession?.WriteString(cmdStr) ?? false;
            return BatchTaskPartResult.Succeed;
        }
    }
}

