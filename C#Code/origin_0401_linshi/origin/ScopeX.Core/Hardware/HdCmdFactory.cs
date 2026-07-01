using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.Hardware
{
    internal static class HdCmdFactory
    {
        private static UInt64 _Command;
        
        public static void Push(HdCmd cmd)
        {
            Tools.Widgets.InterlockedOr(ref _Command, (UInt64)cmd);
        }

        public static UInt64 Command => Interlocked.Exchange(ref _Command, 0);

        static HdCmdFactory()
        {
            Push((HdCmd)~0L);
        }
    }
}
