using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool
{
    class ComboxItemObject
    {
        public string Text
        {
            get;
            set;
        } = "";
        public object? Value
        {
            get;
            set;
        } = null;
        public string Tag
        {
            get;
            set;
        } = "";
        public string Description
        {
            get;
            set;
        } = "";
        public string TipMessage
        {
            get;
            set;
        } = "";
        public override string ToString()
        {
            return Text;
        }
    }
}
