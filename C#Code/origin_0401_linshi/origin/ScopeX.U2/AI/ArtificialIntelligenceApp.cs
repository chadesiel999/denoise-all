using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core;

namespace ScopeX.U2
{
    internal class ArtificialIntelligenceApp
    {
        internal ArtificialIntelligenceApp(ArtificialIntelligencePrsnt prsnt)
        {
            Presenter = prsnt;
        }

        internal ArtificialIntelligencePrsnt Presenter { get; }

        internal static ArtificialIntelligenceApp Default
        {
            get;
            set;
        }

        internal ArticialIntelligenceInfoStrip InfoStrip
        {
            get;
            private set;
        }

        internal void Init()
        {
            InfoStrip = new();

            InfoStrip.Presenter = Presenter;
            Presenter.TryAddView(InfoStrip);
            InfoStrip.Refresh();

        }
    }
}
