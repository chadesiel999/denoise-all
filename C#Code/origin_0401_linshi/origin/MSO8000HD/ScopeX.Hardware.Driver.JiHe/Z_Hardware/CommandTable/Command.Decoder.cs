using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Decoder : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region 解码
                //[HdCmd.DecodeDisabled] = new Action[] { AbstractController_Decoder.DisableDecode },//????
                [HdCmd.DecodeProtocal] = new Action[] { AbstractController_Decoder.Config },
                #endregion
            };
        }
    }
}
