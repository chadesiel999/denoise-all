using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veldrid.Common.Tools
{
    internal static class ArrayTools
    {
        public static Boolean Equals<T>(this T[] oldvalue, T[] newvalue)
        {
            if (newvalue == null || newvalue.Length != oldvalue.Length) return false;
            return !Enumerable.Range(0, oldvalue.Length).Any(x => !Object.Equals(oldvalue[x], newvalue[x]));
        }
    }
}
