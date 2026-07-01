using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public abstract class Collection
    {
        public String ID;
        public Collection(String id, IBroadcaster pub)
        {
            ID = id;
            if (pub != null)
                pub.PublisherChanged += HandleCustomEvent;
        }
        public abstract void HandleCustomEvent(Object? sender, CustomEventArg e);

        public void Add(IBroadcaster pub)
        {
            pub.PublisherChanged += HandleCustomEvent;
        }

        public void Remove(IBroadcaster pub)
        {
            try
            {
                pub.PublisherChanged -= HandleCustomEvent;
            }
            catch (Exception)
            {
            }
        }

        public event EventHandler? ExternalEvents;

        public void Add(IBroadcaster pub, EventHandler e)
        {
            pub.PublisherChanged += HandleCustomEvent;
            ExternalEvents += e;
        }

        public void Remove(IBroadcaster pub, EventHandler e)
        {
            try
            {
                pub.PublisherChanged -= HandleCustomEvent;
                ExternalEvents -= e;
            }
            catch (Exception)
            {
            }
        }

    }
}
