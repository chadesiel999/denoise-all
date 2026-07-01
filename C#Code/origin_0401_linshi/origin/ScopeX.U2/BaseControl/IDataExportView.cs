using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2.BaseControl
{
    public interface IDataExportView
    {
        List<DataTable> GetDataTables();
    }
}
