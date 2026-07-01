using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class ChannelActiveCollection : Collection
    {
        public event EventHandler ActiveChanged = delegate { };
        public ChannelActiveCollection(String id, IBroadcaster pub):base(id, pub)
        { 
        }

        public override void HandleCustomEvent(Object? sender, CustomEventArg e)
        {
            switch (e.Message)
            {
                case "Active":
                    ActiveChanged(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
}
