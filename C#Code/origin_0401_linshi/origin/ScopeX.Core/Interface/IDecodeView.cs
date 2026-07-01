using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Decode;

namespace ScopeX.Core
{
    public interface IDecodeView
    {
        public IProtocolView ProtocolView { get; }

        public ITriggerSerialView TriggerSerialView { get; }

        public SerialProtocolType ProtocolType { get; }

        //public void DrawDecodeInfo(Object graphics, System.Drawing.Rectangle rectangle, IProtocolPrsnt prsnt,System.Drawing.Color ForeColor, Font font);
    }
}
