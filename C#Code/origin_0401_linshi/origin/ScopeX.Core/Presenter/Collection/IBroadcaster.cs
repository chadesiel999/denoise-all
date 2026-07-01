using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public interface IBroadcaster
    {
        event EventHandler<CustomEventArg>? PublisherChanged;

        static event EventHandler<CustomEventArg>? StaticPublisherChanged;
        virtual void OnRaiseCustomEvent(CustomEventArg e) { }

        static void OnRaiseStaticCustomEvent(CustomEventArg e) 
        {
            EventHandler<CustomEventArg>? handler = StaticPublisherChanged;

            if (handler != null)
            {
                handler(e,e);
            }
        }
    }

    public class CustomEventArg : EventArgs
    {
        public CustomEventArg(String s)
        {
            Message = s;
        }

        private String message = "";

        public String Message
        {
            get { return message; }
            set { message = value; }
        }

    }



}
