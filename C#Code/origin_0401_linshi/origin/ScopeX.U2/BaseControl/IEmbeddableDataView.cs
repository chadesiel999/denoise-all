using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal interface IEmbeddableDataView
    {
        Control GetDataView
        {
            get;
        }

        void IndependentControl(Control control);
        public Size LastSize { get; set; }
    }
}
