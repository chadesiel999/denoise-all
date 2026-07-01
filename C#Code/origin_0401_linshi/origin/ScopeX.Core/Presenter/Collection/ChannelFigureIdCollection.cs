using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class ChannelFigureIdCollection : Collection
    {
        public event EventHandler FigureIdChanged = delegate { };
        public ChannelFigureIdCollection(String id, IBroadcaster pub):base(id, pub)
        { 
        }

        public override void HandleCustomEvent(Object? sender, CustomEventArg e)
        {
            switch (e.Message)
            {
                case "WindowId":
                    FigureIdChanged(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
}
