using System;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class ListProperty :Array1Property
        {
            public ListProperty(String name, PropertyTypeEnum valuetype, Byte[] val):base(name,valuetype,val)
            {
                PropertyType = PropertyTypeEnum.List;
            }
        }
    }

}