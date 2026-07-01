using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using System.Reflection;
using System.ComponentModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.ComModel;

namespace ScopeX.Scpi
{
	partial class StubFunc
	{
		//================= AWG =================================================================================================

		public static bool scpiQuy_AWGModeType(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
		{
			bool returnResult = false;
			if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
			{
				ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
				if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
				{
					if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
					{
						sendMessage.UsingScientificNotation = usingScientific;
						sendMessage.SendData = decodeStr(outputString);
						returnResult = true;
					}
				}
			}
			return returnResult;
		}
		public static bool scpiSet_AWGModeType(SCPICommandProcessFuncParam analyResult)
		{
			bool returnResult = false;
			if (!scpiSet_ParamCheck(analyResult))
			{
				return false;
			}
			if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
			{
				ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
				if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
				{
					List<string> param = ParamListToStrList(analyResult.Params);
					if (param.Count > 0)
					{
						if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
							returnResult = true;
					}
				}
			}
			return returnResult;
		}
	}
}
