using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal class ControlMsgFilter : IMessageFilter
    {
        public static ControlMsgFilter Instance => _Instance;
        private static ControlMsgFilter _Instance = new ControlMsgFilter();

        ConcurrentDictionary<IntPtr, Func<Message, bool>> ControlProcTable;
        private ControlMsgFilter()
        {
            ControlProcTable = new ConcurrentDictionary<IntPtr, Func<Message, bool>>();
        }

        public bool PreFilterMessage(ref Message m)
        {
            foreach (var pair in ControlProcTable)
            {
                if (pair.Key == m.HWnd)
                    return pair.Value.Invoke(m);
            }
            return false;
        }

        public bool RegistProc(IntPtr controlHandle, Func<Message, bool> procMethod)
        {
            return ControlProcTable.TryAdd(controlHandle, procMethod);
        }
    }
}
