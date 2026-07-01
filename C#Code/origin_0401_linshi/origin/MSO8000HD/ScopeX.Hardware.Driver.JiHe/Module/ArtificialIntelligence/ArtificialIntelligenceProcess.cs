using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class ArtificialIntelligenceProcess
    {
        private ArtificialIntelligenceProcess()
        {
            EmdProcess = new EmdProcess();
            ReconfigDBIProcess = new ReconfigDBIProcess();
            TemplateTriggerProcess = new TemplateTriggerProcess();
            AutoFilterProcess= new AutoFilterProcess();
            PrecisionProcess = new PrecisionProcess();
            MultiDomainProcess = new MultiDomainProcess();
        }

        internal static ArtificialIntelligenceProcess Default = new ArtificialIntelligenceProcess();

        internal EmdProcess EmdProcess { get; init; }

        internal ReconfigDBIProcess ReconfigDBIProcess { get; init; }

        internal TemplateTriggerProcess TemplateTriggerProcess { get; init; }

        internal PrecisionProcess PrecisionProcess { get; init; }

        internal AutoFilterProcess AutoFilterProcess { get; init; }

        internal MultiDomainProcess MultiDomainProcess { get; init; }

        internal void Run()
        {
            ReconfigDBIProcess.Run();
            TemplateTriggerProcess.Run();
            PrecisionProcess.Run();
        }

        internal void Init()
        {
            EmdProcess.Init();
            //ReconfigDBIProcess.Init();
            PrecisionProcess.Init();
            AutoFilterProcess.Init();
            ReconfigDBIProcess.InitDetectModel();
            MultiDomainProcess.Init();
        }

        internal List<UInt16>? GetProcessedData(ChannelId chnlId, out Int32 dataType)
        {
            if (ReconfigDBIProcess.DataPool.ContainsKey(chnlId) && ReconfigDBIProcess.DataPool[chnlId] != null)
            {
                dataType = 1;
                return ReconfigDBIProcess.DataPool[chnlId]!.ToList();
            }
                

            //var exceptiondata = EmdProcess.TryGetAbnormalData(chnlId);
            //if (exceptiondata != null)
            //{
            //    dataType = 2;
            //    return exceptiondata;
            //}

            dataType = 0;
            return null;
        }
    }
}
