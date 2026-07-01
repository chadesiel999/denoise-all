using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_FileCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            return scpiQuy_CommonByUsingDeclareTable(analyResult, ref sendMessage);
        }
        public static bool scpiSet_FileCommon(SCPICommandProcessFuncParam analyResult)
        {
            return scpiSet_CommonByUsingDeclareTable(analyResult);
        }

        public static bool scpiQuy_FileSave(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.Tag == null)
                return false;
            if (scpiTagObj.ParamList.Count != 2)
            {
                return false;
            }
            var result = false;
            switch (scpiTagObj.Tag.ToString())
            {
                case "WFMSave":
                    result = Presenter.File.SaveWaveform(false);
                    break;
                case "PICSave":
                    result = Presenter.File.SaveImage(false);
                    break;
                default:
                    result = false;
                    break;
            }
            var msg = result ? scpiTagObj.ParamList[0] : scpiTagObj.ParamList[1];
            sendMessage.SendData = decodeStr(msg);
            return true;
        }
    }
}
