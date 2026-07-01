using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                [HdCmd.ArtificialIntelligence] = new Action[]
                {
                    ArtificialIntelligenceProcess.Default.EmdProcess.PropertiesChanged,
                    ArtificialIntelligenceProcess.Default.TemplateTriggerProcess.TemplateDataChanged,
                    ArtificialIntelligenceProcess.Default.AutoFilterProcess.PropertyChanged,
                    ArtificialIntelligenceProcess.Default.PrecisionProcess.PropertyChanged,
                    ArtificialIntelligenceProcess.Default.MultiDomainProcess.PropertyChanged,//????
                    AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength,
                },
            };
        }
    }
}
