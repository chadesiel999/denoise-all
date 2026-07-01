using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
        public class OrderAttribute:Attribute
        {
            public OrderAttribute(Int32 order) => Order = order;
            public Int32 Order { get; }
        }
    }
}
