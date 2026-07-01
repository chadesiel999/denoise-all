using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public abstract class BatchTaskBase:IBatchTask
    {
        protected BatchTaskState state = BatchTaskState.Unknow;
        abstract protected void TaskBody();
        public virtual bool CheckPrepareOk(ref string fileMessge, ref string InstrumentationInfo) => false;
        protected string Tag = "";
        protected string _tipMessage = "";
        protected string _description = "";
        protected string _TaskParameterDescription = "";
        public virtual bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg) 
        {
            if (cancelTokenSrc != null)
            {
                cancelTokenSrc.Dispose();
                cancelTokenSrc = null;
            }
            ErrorMsg = "";
            _tipMessage = tipMessage;
            _description = description;
            ourInstrument = instrumentInteract;
            _TaskParameterDescription = "";
            Tag = tag;
            return true;
        }
        public virtual int MaxStepCount
        {
            get => 100;
        }
        public virtual string TipMessage
        {
            get => _tipMessage;
        }
        public virtual string SpecialDescripton
        {
            get =>_description;
        }
        public virtual string ResultTipMessage
        {
            get =>"";
        }
        public virtual string ResultFileName
        {
            get =>"";
        }
        public virtual string TaskParameterDescription(string tag)
        {
            return "";
        }
        public virtual string CurrentProcessingXmlFilename
        {
            get => "";
        }
        protected IInstrumentSession? ourInstrument = null;

        protected Action<int, string, string>? updateAction = null;
        protected CancellationTokenSource? cancelTokenSrc = null;
        public Task<BatchTaskState> RunAsync(Action<int, string, string> update)
        {
            updateAction = update;
            cancelTokenSrc = new CancellationTokenSource();
            return Task.Run(() =>
            {
                TaskBody();
                return state;
            }, cancelTokenSrc.Token);
        }
        public void Cancel()
        {
            cancelTokenSrc?.Cancel();
        }
    }
}
