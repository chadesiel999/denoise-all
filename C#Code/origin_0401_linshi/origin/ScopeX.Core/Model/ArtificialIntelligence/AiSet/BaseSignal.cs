using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Model.ArtificialIntelligence.AiSet
{
    internal abstract class BaseSignal
    {
        internal String? type
        {
            get; set;
        }

        protected String NormalizedType => type?.Trim() ?? String.Empty;

        protected static void AppendAiTip(String message)
        {
            if (!String.IsNullOrWhiteSpace(message))
                DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo(message);
        }

        protected static ChannelId ResolveAnalogSource()
        {
            ChannelId source = DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis.Source;
            if (source.IsAnalog()
                && DsoModel.Default.TryGetChannel(source, out ChannelModel? sourceChannel)
                && sourceChannel.Active)
            {
                return source;
            }

            ChannelModel? firstActive = DsoModel.Default.Channels.FirstOrDefault(ch => ch.Id.IsAnalog() && ch.Active);
            return firstActive?.Id ?? ChannelId.C1;
        }

        public abstract void AiSet();
    }
}
