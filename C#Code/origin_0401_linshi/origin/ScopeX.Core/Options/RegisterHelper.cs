using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScopeX.Core.Options
{
    internal class RegisterHelper
    {
        private const String SubKey = "Software\\ScopeX";
        private RegDomain Domain => RegDomain.CurrentUser;

        public static RegisterHelper Default { get; } = new RegisterHelper();

        private RegisterHelper()
        {
            CreateSubKey();
        }

        #region 创建注册表项  

        /// <summary>
        /// 创建注册表
        /// </summary>
        private void CreateSubKey()
        {
            ///创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            ///要创建的注册表项的节点  
            RegistryKey sKey;
            if (!IsSubKeyExist())
            {
                //var software = key.OpenSubKey(FirstKey, true);
                sKey = key.CreateSubKey(SubKey);
                sKey.Close();
            }
            ///关闭对注册表项的更改  
            key.Close();
        }

        #endregion

        #region 判断注册表项是否存在  

        /// <summary>  
        /// 判断注册表项是否存在，默认是在注册表基项HKEY_LOCAL_MACHINE 下判断（请先设置SubKey 属性）  
        /// 例子：如果设置了Domain 和SubKey 属性，则判断Domain\\SubKey，否则默认判断HKEY_LOCAL_MACHINE\\software\\  
        /// </summary>  
        /// <returns>返回注册表项是否存在，存在返回true，否则返回false</returns>  
        private Boolean IsSubKeyExist()
        {
            ///检索注册表子项  
            ///如果sKey 为null,说明没有该注册表项不存在，否则存在  
            RegistryKey? sKey = OpenSubKey();
            if (sKey == null)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 删除注册表项  

        /// <summary>  
        /// 删除注册表项
        /// </summary>  
        /// <returns>如果删除成功，则返回true，否则为false</returns>  
        public Boolean DeleteSubKey()
        {
            ///返回删除是否成功  如果表项不存在 也返回true
            var bok = true;

            ///创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            if (IsSubKeyExist())
            {
                try
                {
                    ///删除注册表项  
                    key.DeleteSubKey(SubKey);
                    bok = true;
                }
                catch
                {
                    bok = false;
                }
            }
            ///关闭对注册表项的更改  
            key.Close();
            return bok;
        }

        #endregion

        #region 判断键值是否存在  

        /// <summary>  
        /// 判断键值是否存在
        /// 如果SubKey 为空、null 或者SubKey 指定的注册表项不存在，返回false  
        /// </summary>  
        /// <param name="keyName">键值名称</param>  
        /// <returns>返回键值是否存在，存在返回true，否则返回false</returns>  
        private Boolean IsRegeditKeyExist(String keyName)
        {
            ///返回结果  
            var result = false;

            ///判断是否设置键值属性  
            if (String.IsNullOrEmpty(keyName))
            {
                return false;
            }

            ///判断注册表项是否存在  
            if (IsSubKeyExist())
            {
                ///打开注册表项  
                var key = OpenSubKey();
                ///键值集合  
                String[] regeditKeyNames;
                ///获取键值集合  
                regeditKeyNames = key!.GetValueNames();
                ///遍历键值集合，如果存在键值，则退出遍历  
                foreach (var regeditKey in regeditKeyNames)
                {
                    if (String.Compare(regeditKey, keyName, true) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                ///关闭对注册表项的更改  
                key.Close();
            }
            return result;
        }

        #endregion

        #region 设置键值内容  

        /// <summary>  
        /// 设置指定的键值内容，不指定内容数据类型
        /// 存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容  
        /// </summary>  
        /// <param name="keyName">键值名称</param>  
        /// <param name="keyValue">键值内容</param>  
        /// <returns>键值内容设置成功，则返回true，否则返回false</returns>  
        public virtual Boolean WriteRegeditKey(String keyName, Object keyValue)
        {
            ///返回结果  
            var result = false;

            ///判断键值是否存在  
            if (String.IsNullOrEmpty(keyName))
            {
                return false;
            }

            ///判断注册表项是否存在，如果不存在，则直接创建  
            if (!IsSubKeyExist())
            {
                CreateSubKey();
            }

            ///以可写方式打开注册表项  
            RegistryKey? key = OpenSubKey(true);

            ///如果注册表项打开失败，则返回false  
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(keyName, keyValue);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                ///关闭对注册表项的更改  
                key.Close();
            }
            return result;
        }

        /// <summary>  
        /// 设置指定的键值内容，指定内容数据类型（
        /// 存在改键值则修改键值内容，不存在键值则先创建键值，再设置键值内容  
        /// </summary>  
        /// <param name="keyName">键值名称</param>  
        /// <param name="keyContent">键值内容</param>  
        /// <returns>键值内容设置成功，则返回true，否则返回false</returns>  
        public virtual Boolean WriteRegeditKey(String keyName, Object keyContent, RegValueKind kind)
        {
            ///返回结果  
            var result = false;

            if (String.IsNullOrEmpty(keyName))
            {
                return false;
            }

            ///判断注册表项是否存在，如果不存在，则直接创建  
            if (!IsSubKeyExist())
            {
                CreateSubKey();
            }

            ///以可写方式打开注册表项  
            RegistryKey? key = OpenSubKey(true);

            ///如果注册表项打开失败，则返回false  
            if (key == null)
            {
                return false;
            }

            try
            {
                key.SetValue(keyName, keyContent, GetRegValueKind(kind));
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                ///关闭对注册表项的更改  
                key.Close();
            }
            return result;
        }

        #endregion

        #region 读取或删除键值内容  

        /// <summary>
        /// 读取键值内容
        /// 指示的注册表项不存在，返回null,反之，则返回键值内容  
        /// </summary>
        /// <param name="keyName">键值名称</param>  
        /// <returns>返回键值内容</returns>  
        public virtual String? ReadRegeditKey(String keyName)
        {
            ///键值内容结果  
            String? content = null;

            ///判断是否设置键值属性  

            if (String.IsNullOrEmpty(keyName))
            {
                return null;
            }

            ///判断键值是否存在  
            if (IsRegeditKeyExist(keyName))
            {
                ///打开注册表项  
                RegistryKey? key = OpenSubKey();
                //if (key != null)
                //{
                //	var value = key.GetValue(keyName);
                //	content = value == null ? null : value.ToString();
                //	///关闭对注册表项的更改  
                //	key.Close();
                //}
                if (key != null)
                {
                    try
                    {
                        var value = key.GetValue(keyName);
                        content = value == null ? null : value.ToString();
                    }
                    catch
                    {
                        content = null;
                    }
                    finally
                    {
                        ///关闭对注册表项的更改  
                        key.Close();
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 读取所有键值内容
        /// 指示的注册表项不存在，返回null,反之，则返回键值内容 
        /// </summary>
        /// <returns>返回键值集合</returns>
        public Dictionary<String, String?>? ReadAllRegeditKey()
        {
            Dictionary<String, String?> allkey = new Dictionary<String, String?>();
            ///打开注册表项  
            var key = OpenSubKey();
            ///键值集合  
            String[] keynames;
            if (key != null)
            {
                try
                {
                    ///获取键值集合  
                    keynames = key.GetValueNames();
                    foreach (var name in keynames)
                    {
                        var value = key.GetValue(name, null);
                        var content = value == null ? null : value.ToString();
                        allkey.Add(name, content);
                    }
                }
                catch
                {
                    return null;
                }
                finally
                {
                    key.Close();
                }

            }
            else
            {
                return null;
            }
            return allkey;
        }

        /// <summary>  
        /// 删除键值
        /// 如果指示的注册表项不存在，返回true
        /// </summary>  
        /// <param name="keyName">键值名称</param>  
        /// <returns>如果删除成功，返回true，否则返回false</returns>  
        public Boolean DeleteRegeditKey(String keyName)
        {
            ///删除结果  
            var result = true;

            ///判断键值名称是否为空，如果为空，返回true  
            if (String.IsNullOrEmpty(keyName))
            {
                return true;
            }

            ///判断键值是否存在  
            if (IsRegeditKeyExist(keyName))
            {
                ///以可写方式打开注册表项  
                RegistryKey? key = OpenSubKey(true);
                if (key != null)
                {
                    try
                    {
                        ///删除键值  
                        key.DeleteValue(keyName);
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        ///关闭对注册表项的更改  
                        key.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 删除键值，并保留指定的项
        /// 如果指示的注册表项不存在，返回true
        /// </summary>
        /// <param name="shieldkeys">需要屏蔽的项</param>
        /// <returns></returns>
        public Boolean DeleteAllRegeditKey(String[]? shieldkeys = null)
        {
            var bok = true;
            ///打开注册表项  
            var key = OpenSubKey(true);
            if (key != null)
            {
                try
                {
                    ///获取键值集合  
                    var keynames = key.GetValueNames();
                    foreach (var name in keynames)
                    {
                        if (shieldkeys != null)
                        {
                            if (!shieldkeys.Contains(name))
                            {
                                key.DeleteValue(name);
                            }
                        }
                        else
                        {
                            key.DeleteValue(name);
                        }
                    }
                    bok = true;
                }
                catch
                {
                    bok = false;
                }
                finally
                {
                    key.Close();
                }
            }

            return bok;
        }


        #endregion

        /// <summary>  
        /// 获取注册表基项域对应顶级节点  
        /// 例子：如regDomain 是ClassesRoot，则返回Registry.ClassesRoot  
        /// </summary>  
        /// <param name="regDomain">注册表基项域</param>  
        /// <returns>注册表基项域对应顶级节点</returns>  
        private RegistryKey GetRegDomain(RegDomain regDomain)
        {
            ///创建基于注册表基项的节点  
            return regDomain switch
            {
                RegDomain.ClassesRoot => Registry.ClassesRoot,
                RegDomain.CurrentUser => Registry.CurrentUser,
                RegDomain.LocalMachine => Registry.LocalMachine,
                RegDomain.User => Registry.Users,
                RegDomain.CurrentConfig => Registry.CurrentConfig,
                RegDomain.PerformanceData => Registry.PerformanceData,
                _ => Registry.LocalMachine
            };
        }

        /// <summary>  
        /// 获取在注册表中对应的值数据类型  
        /// 例子：如regValueKind 是DWord，则返回RegistryValueKind.DWord  
        /// </summary>  
        /// <param name="regValueKind">注册表数据类型</param>  
        /// <returns>注册表中对应的数据类型</returns>  
        private RegistryValueKind GetRegValueKind(RegValueKind regValueKind)
        {
            return regValueKind switch
            {
                #region 判断注册表数据类型  

                RegValueKind.Unknown => RegistryValueKind.Unknown,
                RegValueKind.String => RegistryValueKind.String,
                RegValueKind.ExpandString => RegistryValueKind.ExpandString,
                RegValueKind.Binary => RegistryValueKind.Binary,
                RegValueKind.DWord => RegistryValueKind.DWord,
                RegValueKind.MultiString => RegistryValueKind.MultiString,
                RegValueKind.QWord => RegistryValueKind.QWord,
                _ => RegistryValueKind.String

                #endregion
            };

        }

        /// <summary>  
        /// 打开注册表项节点，以只读方式检索子项  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        private RegistryKey? OpenSubKey()
        {
            ///创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            /////先打开主Key
            //var firstkey = key.OpenSubKey(FirstKey);


            ///要打开的注册表项的节点  
            RegistryKey? sKey = null;
            ///打开注册表项  
            sKey = key.OpenSubKey(SubKey);
            ///关闭对注册表项的更改  
            key.Close();
            ///返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 打开注册表项节点  
        /// 虚方法，子类可进行重写  
        /// </summary>  
        /// <param name="writable">如果需要项的写访问权限，则设置为 true,否则为只读模式</param>  
        /// <returns>如果SubKey为空、null或者SubKey指示注册表项不存在，则返回null，否则返回注册表节点</returns>  
        private RegistryKey? OpenSubKey(Boolean writable)
        {
            ///创建基于注册表基项的节点  
            RegistryKey key = GetRegDomain(Domain);

            ///要打开的注册表项的节点  
            RegistryKey? sKey = null;
            ///打开注册表项  
            sKey = key.OpenSubKey(SubKey, writable);
            ///关闭对注册表项的更改  
            key.Close();
            ///返回注册表节点  
            return sKey;
        }

        /// <summary>  
        /// 注册表基项静态域  
        ///1.Registry.ClassesRoot 对应于HKEY_CLASSES_ROOT 主键  
        ///2.Registry.CurrentUser 对应于HKEY_CURRENT_USER 主键  
        ///3.Registry.LocalMachine 对应于 HKEY_LOCAL_MACHINE 主键  
        ///4.Registry.User 对应于 HKEY_USER 主键  
        ///5.Registry.CurrentConfig 对应于HEKY_CURRENT_CONFIG 主键  
        ///6.Registry.DynDa 对应于HKEY_DYN_DATA 主键  
        ///7.Registry.PerformanceData 对应于HKEY_PERFORMANCE_DATA 主键  
        /// </summary>  
        internal enum RegDomain
        {
            /// <summary>  
            /// 对应于HKEY_CLASSES_ROOT 主键  
            /// </summary>  
            ClassesRoot = 0,
            /// <summary>  
            /// 对应于HKEY_CURRENT_USER 主键  
            /// </summary>  
            CurrentUser = 1,
            /// <summary>  
            /// 对应于 HKEY_LOCAL_MACHINE 主键  
            /// </summary>  
            LocalMachine = 2,
            /// <summary>  
            /// 对应于 HKEY_USER 主键  
            /// </summary>  
            User = 3,
            /// <summary>  
            /// 对应于HEKY_CURRENT_CONFIG 主键  
            /// </summary>  
            CurrentConfig = 4,
            /// <summary>  
            /// 对应于HKEY_PERFORMANCE_DATA 主键  
            /// </summary>  
            PerformanceData = 5
        }

        /// <summary>  
        /// 指定在注册表中存储值时所用的数据类型，或标识注册表中某个值的数据类型  
        ///1.RegistryValueKind.Unknown  
        ///2.RegistryValueKind.String  
        ///3.RegistryValueKind.ExpandString  
        ///4.RegistryValueKind.Binary  
        ///5.RegistryValueKind.DWord  
        ///6.RegistryValueKind.MultiString  
        ///7.RegistryValueKind.QWord  
        /// </summary>  
        internal enum RegValueKind
        {
            /// <summary>  
            /// 指示一个不受支持的注册表数据类型。例如，不支持 Microsoft Win API 注册表数据类型 REG_RESOURCE_LIST。使用此值指定  
            /// </summary>  
            Unknown = 0,
            /// <summary>  
            /// 指定一个以 Null 结尾的字符串。此值与 Win API 注册表数据类型 REG_SZ 等效。  
            /// </summary>  
            String = 1,
            /// <summary>  
            /// 指定一个以 NULL 结尾的字符串，该字符串中包含对环境变量（如 %PATH%，当值被检索时，就会展开）的未展开的引用。  
            /// 此值与 Win API 注册表数据类型 REG_EXPAND_SZ 等效。  
            /// </summary>  
            ExpandString = 2,
            /// <summary>  
            /// 指定任意格式的二进制数据。此值与 Win API 注册表数据类型 REG_BINARY 等效。  
            /// </summary>  
            Binary = 3,
            /// <summary>  
            /// 指定一个  位二进制数。此值与 Win API 注册表数据类型 REG_DWORD 等效。  
            /// </summary>  
            DWord = 4,
            /// <summary>  
            /// 指定一个以 NULL 结尾的字符串数组，以两个空字符结束。此值与 WinAPI 注册表数据类型 REG_MULTI_SZ 等效。  
            /// </summary>  
            MultiString = 5,
            /// <summary>  
            /// 指定一个  位二进制数。此值与 Win API 注册表数据类型 REG_QWORD 等效。  
            /// </summary>  
            QWord = 6
        }
    }
}
