using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public interface ISearchTypePrsnt
    {
        TriggerPrsnt Trigger { get; }
        ChannelId Source { get; set; }

        Int32 ResultCount { get; set; }
        (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        void LoadTriggerPrsnt();

        Boolean ReadFromTrigger();

        Boolean SetToTrigger();
    }
}
