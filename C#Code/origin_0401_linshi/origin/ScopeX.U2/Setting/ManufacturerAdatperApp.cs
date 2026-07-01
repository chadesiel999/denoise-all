using System.Collections.Generic;

namespace ScopeX.U2
{
    internal class ManufacturerAdatperApp
    {
        public ManufacturerAdatperApp()
        {
            var Adapters = new List<ScpiAdapter.ScpiAdapter>();
            Adapters = DllLoader<ScpiAdapter.ScpiAdapter>.LoadAdapter();

            Program.Oscilloscope.ScpiAdapters = Adapters.AsReadOnly();
        }

        public static ManufacturerAdatperApp Default
        {
            get;
            internal set;
        }
    }
}
