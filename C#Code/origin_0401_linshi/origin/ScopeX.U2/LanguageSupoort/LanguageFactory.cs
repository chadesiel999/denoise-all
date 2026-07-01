using ScopeX.Controls.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScopeX.ComModel;

namespace ScopeX.U2.LanguageSupoort
{
    internal static class LanguageFactory
    {
        /// <summary>
        ///  控件多语言配置缓存信息
        /// </summary>
        /// <param name="key">唯一标识</param>
        /// <param name="feild">字段名称</param>
        /// <param name="val">缓存时的属性值</param>
        internal record CacheEntity(string key, string feild, string val)
        {
            /// <summary>
            /// 对空间的弱引用
            /// </summary>
            public WeakReference<Control> Reference { get; set; }
        };

        internal static Dictionary<string, List<CacheEntity>> AllPageLuanguage = new Dictionary<string, List<CacheEntity>>();

        /// <summary>
        /// 缓存窗体下的所有子控件，如果已有缓存，则覆盖。
        /// </summary>
        /// <param name="form"></param>
        internal static void CacheFormLanguageControls(Form form)
        {
            try
            {
                if (string.IsNullOrEmpty(form.Name))
                    throw new System.Exception(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuangTiMingChengBuNengWeiKong"));

                var temp = GetFormCurrentLanguageInfo(form);
                if (temp == null)
                    return;

                if (AllPageLuanguage.ContainsKey(form.Name))
                {
                    AllPageLuanguage[form.Name] = temp;
                }
                else
                {
                    AllPageLuanguage.Add(form.Name, temp);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 更新窗体控件缓存，如果已存在，不处理，不存在的追加。
        /// </summary>
        /// <param name="form"></param>
        internal static void UpdateFormCache(Form form)
        {
            try
            {
                if (string.IsNullOrEmpty(form.Name))
                    throw new System.Exception(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuangTiMingChengBuNengWeiKong"));

                var temp = GetFormCurrentLanguageInfo(form);
                if (temp == null)
                    return;

                if (AllPageLuanguage.ContainsKey(form.Name))
                {
                    var formCaches = AllPageLuanguage[form.Name];
                    foreach (var item in temp)
                    {
                        var cacheitem = formCaches.FirstOrDefault(c => c.key.Equals(item.key, StringComparison.OrdinalIgnoreCase));
                        if (cacheitem != null)
                            continue;

                        formCaches.Add(item);
                    }
                }
                else
                {
                    AllPageLuanguage.Add(form.Name, temp);
                }
            }
            catch
            {

            }
        }

        internal static void ChangeLanguage()
        {
            CacheEntity cache;
            foreach (var formLang in AllPageLuanguage)
            {
                List<CacheEntity> tempItemControls = formLang.Value;
                if (tempItemControls == null || tempItemControls.Count <= 0)
                    continue;

                for (int i = tempItemControls.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        cache = tempItemControls[i];
                        if (cache == null || cache.Reference == null || string.IsNullOrEmpty(cache.key))
                            continue;

                        if (!cache.Reference.TryGetTarget(out Control target))
                        {
                            // 弱引用无效了，删除缓存
                            tempItemControls.Remove(cache);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(cache.feild) && !cache.feild.Equals("text", StringComparison.OrdinalIgnoreCase))
                        {
                            var targetProp = target.GetType().GetProperty(cache.feild, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                            if (targetProp != null)
                            {
                                var temp = LanguageManger.Instance.GetIDMessage(cache.key, targetProp.Name);
                                if (temp != cache.key)
                                    targetProp.SetValue(target, temp, null);
                            }
                        }
                        else
                        {
                            var temp = LanguageManger.Instance.GetIDMessage(cache.key);
                            if (temp != cache.key)
                                target.Text = temp;
                        }

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

#if DEBUG && SaveLanguage
        public static void Save2Disk(string savepath)
        {
            if (string.IsNullOrEmpty(savepath))
                return;

            StringBuilder sb = new();
            var temp = AllPageLuanguage.SelectMany(c => c.Value).GroupBy(c => c.key);
            foreach (var item in temp)
            {
                sb.Append($"<Item name=\"{item.Key}\"");
                bool hastext = false; // 一个临时标志，用于去除同一个key，多个text字段问题
                foreach (var item2 in item)
                {
                    if (item2.feild.Equals("text", StringComparison.OrdinalIgnoreCase))
                    {
                        if (hastext)
                            continue;
                        hastext = true;
                        sb.Append($" text=\"{item2.val}\"");
                    }
                    else
                        sb.Append($" {item2.feild}=\"{item2.val}\"");
                }
                sb.Append(" />\r\n");
                //sb.AppendLine($"<Item name=\"{item.key}\" text=\"{item.val}\" />");
            }

            System.IO.File.WriteAllText(savepath, sb.ToString());
        }

#endif
        /// <summary>
        /// 当前使用的语言
        /// </summary>
        public static Language Current { get; private set; } = Language.简体中文;

        public static ILanguage GetLanguage(Language language)
        {
            Current = language;
            return PlatformUIManager.Default.Platform.GetLanguage(language);
        }

        /// <summary>
        /// 获取指定窗体的具有中文字符的控件资源信息
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private static List<CacheEntity> GetFormCurrentLanguageInfo(Form form)
        {
            List<CacheEntity> templist = new List<CacheEntity>();
            var formcontrols = Toolkit.GetFormControls(form);
            if (formcontrols == null)
                return null;

            //string pattern = @"[\u4e00-\u9fa5]+";
            foreach (var ctlInfos in formcontrols)
            {
                // 跳过空数据
                if (ctlInfos.Item1 == null || string.IsNullOrEmpty(ctlInfos.Item2) || string.IsNullOrEmpty(ctlInfos.Item3) || string.IsNullOrWhiteSpace(ctlInfos.Item4))
                    continue;

                /*// 正则表达式匹配
                MatchCollection matches = Regex.Matches(ctlInfos.Item4, pattern);
                if (matches == null || matches.Count <= 0)
                    continue;*/

                templist.Add(new CacheEntity(ctlInfos.Item2, ctlInfos.Item3, ctlInfos.Item4) { Reference = ctlInfos.Item1 });
            }

            return templist;
        }
    }
}
