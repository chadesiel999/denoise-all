using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class PresenterCollection : Collection
    {
        public event EventHandler<CustomEventArg>? PublisherChanged;

        public PresenterCollection(String id, IBroadcaster pub) : base(id, pub)
        {
            IBroadcaster.StaticPublisherChanged += HandleCustomEvent;
        }

        public override void HandleCustomEvent(Object? sender, CustomEventArg e)
        {
            PublisherChanged?.Invoke(sender, e);
            return;
        }
    }
}
