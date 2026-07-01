using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class SubPageBase:UserControl
    {

        private static List<Type> Children = new List<Type>();

        #region Math
        public static Control GetMathChildrenInstantce(string mathtype)
        {
            Control ctl = null;
            var type = GetMathChildren(mathtype);

            if (type != null)
            {
                ctl = (Control)Activator.CreateInstance(type);
            }

            return ctl;
        }

        public static Type? GetMathChildren(string mathtype)
        {
            if (Children == null || Children.Count == 0)
            {
                LoadChildren();
            }

            foreach (var child in Children)
            {
                var descriptionAttribute = child.GetCustomAttribute<DescriptionAttribute>();

                if (descriptionAttribute != null && descriptionAttribute.Description == mathtype)
                {
                    return child;
                }
            }

            return null;
        }
        #endregion

        #region Search

        #endregion


        protected static void LoadChildren()
        {
            // 获取当前应用程序域中加载的所有程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 筛选包含子类的程序集
            var assembliesWithSubPageChildren = assemblies.Where(a =>
            {
                try
                {
                    // 尝试从程序集中获取继承自 SubPageBase 的类型
                    return a.GetTypes().Any(t => t.IsSubclassOf(typeof(SubPageBase)));
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略加载失败的程序集
                    return false;
                }
            });
            foreach (var assembly in assembliesWithSubPageChildren)
            {
                try
                {
                    var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(SubPageBase)));
                    Children.AddRange(types);
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略加载失败的程序集
                }
            }
        }
    }
}
