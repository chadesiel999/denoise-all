using System;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
        public class SeializableAttribute : Attribute
        {
            public SeializableAttribute(Boolean ignore = true)
            {
                Ignore = ignore;
            }
            public SeializableAttribute(String name, Boolean ignore = false)
            {
                Name = name;
                Ignore = ignore;
            }
            public String Name { get; } = "";
            public Boolean Ignore { get; } = false;
        }
    }

}