using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_CaliDataChanged : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region 校准数据改变
                [HdCmd.CaliDataChanged] = new Action[] { AbstractController_Misc.CaliDataChanged },
                [HdCmd.SystemCtrl] = new Action[] { Hd.CfgFansSpeed },
                #endregion
            };
        }
    }
}
