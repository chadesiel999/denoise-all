using System;

namespace ScopeX.ComModel
{



    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(String alias)
        {
            Alias = alias;
        }

        public String Alias 
        { 
            get; 
            private set; 
        }
    }

    public class DisplayAttribute :Attribute
    {
        public DisplayAttribute(String display)
        {
            Diplay = display;
        }

        public String Diplay
        {
            get;
            private set;
        }
    }

}
