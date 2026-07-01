using System;
using System.Collections.Generic;

namespace ScopeX.Scpi
{
    internal record ScpiTagObj(Object PrsntObj = null, String PropertyName = "", Boolean IsData = false, Boolean IsTimeOrFreq = false, List<String> ParamList = null, Int64 IntOrDoubleMultiplier = 1, Object Tag = null);
}
