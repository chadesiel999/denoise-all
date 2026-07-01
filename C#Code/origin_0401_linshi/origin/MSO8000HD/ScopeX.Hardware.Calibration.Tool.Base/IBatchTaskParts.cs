using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public enum BatchTaskPartResult
    {
        ErrorGeneral,
        ErrorFatal,
        ErrorParameter,
        ErrorOvertime,
        Cancel,
        Succeed
    }
    public interface IBatchTaskPart
    {
        void SetInstrument(IInstrumentSession instrumentSession);
        bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter);
        void SetDebugVariantStatus(bool status);
        BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null);
        int ErrorCount
        {
            get;
        }
        string FuncionDescription
        {
            get;
        }
        string ParametersDescription
        {
            get;
        }
        string Example
        {
            get;
        }
    }
    public abstract class BatchTaskPartBase : IBatchTaskPart
    {
        protected IInstrumentSession? currInstrumentSession = null;
        public void SetInstrument(IInstrumentSession instrumentSession)
        {
            currInstrumentSession = instrumentSession;
        }
        protected string parameterStr = "";
        protected int errorCount = 0;
        protected XmlScpiCmd? xmlScpiCMD = null;
        public virtual bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            xmlScpiCMD = xmlScpiCmd;
            parameterStr = parameter;
            return true;
        }
        public virtual int ErrorCount
        {
            get => errorCount;
        }
        public virtual string FuncionDescription
        {
            get => "";
        }
        public virtual string ParametersDescription
        {
            get => "";
        }
        public virtual string Example
        {
            get => "";
        }

        public abstract BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null);

        public virtual void SetDebugVariantStatus(bool status)
        {
        }
    }
    public class BatchTaskPartFactory
    {
        public static IBatchTaskPart? Create(XmlScpiCmd xmlScpiCmd, string[]? name_ParameterPair)
        {
            IBatchTaskPart? part = null;
            string className = "";
            string otherParams = "";
            if (xmlScpiCmd != null)
            {
                if (xmlScpiCmd.ProgramFuncName != "")
                {
                    string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd!.ProgramFuncName.Trim());
                    if (myName_ParameterPair == null)
                        return null;
                    className = myName_ParameterPair[0];
                }
            }
            if (className == "")
            {
                if (name_ParameterPair != null)
                {
                    className = name_ParameterPair[0];
                    if (name_ParameterPair.Length > 1)
                        otherParams = name_ParameterPair[1];
                }
            }
            var q = from t in BaseHelper.AllBatchTaskPartClass?.ToArray()
                    where t.Name == className
                    select t;
            if (Enumerable.Count(q) > 0)
                part = (IBatchTaskPart?)Activator.CreateInstance((Type)q.ToList()[0]);
            else
                part = null;
            if (part != null)
                part.SetParameter(xmlScpiCmd, otherParams);
            return part;
        }
    }
}
