using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface IDsoPrsnt
    {
        IDsoView? View
        {
            get;
        }

        Boolean TryGetChannel(ChannelId id, out IChnlPrsnt? chnl);

        //Boolean TryAddChannel(ChannelId id, IChnlPrsnt chnl);

        ITriggerPrsnt CurrentTrigger { get; }

        ITimebasePrsnt Timebase { get; }

        //Dictionary<String, IApp> Apps { get; }
    }
}
