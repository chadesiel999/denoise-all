using ScopeX.Controls.Language;

namespace ScopeX.Updater.Core
{
    internal class LanguageFactory
    {
        private static ILanguage _english;

        public static ILanguage EnglishLang
        {
            get
            {
                if (_english == null)
                {
                    XMLLanguage xmllanguage = new XMLLanguage("english.xml");
                    //if (Constants.PRODUCT == ProductType.JiHe_UPO7000L)
                    //{
                    //    xmllanguage.AppendOrUpdate("english_UPO7000L.xml");
                    //}
                    _english = xmllanguage;
                }
                return _english;
            }
        }

        private static ILanguage _chinese;

        public static ILanguage ChineseLang
        {
            get
            {
                if (_chinese == null)
                {
                    XMLLanguage xmllanguage = new XMLLanguage("chinese.xml");
                    //if (Constants.PRODUCT == ProductType.JiHe_UPO7000L)
                    //{
                    //    xmllanguage.AppendOrUpdate("Tip_UPO7000L.xml");
                    //}
                    _chinese = xmllanguage;
                }
                return _chinese;
            }
        }

        /// <summary>
        /// 当前使用的语言
        /// </summary>
        public static Language Current { get; private set; } = Language.简体中文;

        public static ILanguage GetLanguage(Language language)
        {
            Current = language;
            return language switch
            {
                Language.English => EnglishLang,
                _ => ChineseLang
            };
        }
    }
}
