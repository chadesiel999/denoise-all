using ScopeX.ComModel;
using ScopeX.Controls.LanguageDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScopeX.U2.LanguageSupoort
{
    internal static class LanguageHelper
    {
        /// <summary>
        /// 获取枚举的多语言描述值
        /// </summary>
        /// <param name="enum">枚举值</param>
        /// <returns></returns>
        public static string GetDescription_Lang(this Enum @enum)
        {
            string key = $"Enum_{@enum.GetType().Name}_{@enum}"; // 格式： Enum_XXXEnumType_XXXEnumItemName
            var tempstr = Controls.Language.LanguageManger.Instance.GetIDMessage(key);
            if (tempstr != key)
                return tempstr;

            return @enum.GetDescription();
        }

        public static string GetPowerAnalysisString(string @key)
        {
            string result = "";
            result = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(@key);
            //if (result == @key)
            //    result = LanguageHelper.GetPowerAnalysisString(@key);

            return result;
        }

        public static string GetAliaLangString(this Enum @enum)
        {
            string key = $"Enum_{@enum.GetType().Name}_{@enum}"; // 格式： Enum_XXXEnumType_XXXEnumItemName
            var tempstr = Controls.Language.LanguageManger.Instance.GetIDMessage(key);
            if (tempstr != key)
                return tempstr;

            return @enum.GetAlias();
        }
    }
}
