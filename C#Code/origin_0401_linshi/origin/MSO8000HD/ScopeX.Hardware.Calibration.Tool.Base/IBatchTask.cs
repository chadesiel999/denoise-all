using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public interface IBatchTask
    {
        bool Init(IInstrumentSession instrumentInteract, string tipMessage,string description ,string tag , out string ErrorMsg);
        string TipMessage
        {
            get;
        }
        string SpecialDescripton
        {
            get;
        }
        string ResultTipMessage
        {
            get;
        }
        int MaxStepCount
        {
            get;
        }
        string ResultFileName
        {
            get;
        }
        string CurrentProcessingXmlFilename
        {
            get;
        }
        string TaskParameterDescription(string tag);
        bool CheckPrepareOk(ref string fileMessge, ref string InstrumentationInfo);
        Task<BatchTaskState> RunAsync(Action<int/*step*/,string/*current step process message*/,string/*last step process result*/> update);
        void Cancel();
    }
    public enum BatchTaskState
    {
        Unknow,
        Running,
        Canceled,
        FinishedOK,
        FinishedFailed
    }
}
