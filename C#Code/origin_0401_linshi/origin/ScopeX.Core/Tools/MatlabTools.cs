using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ScopeX.Core.Tools
{
    public class MatlabTools
    {
        public static Boolean IsMatlabInstalled()
        {
            return GetMatlabInstallPathFromRegistry();
        }

        private static Boolean GetMatlabInstallPathFromRegistry()
        {
            try
            {
                const String matlabkey = @"SOFTWARE\MathWorks\MATLAB";

                #region 检查ClassesRoot
                // 尝试打开 64 位注册表视图
                using (var basekey64 = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64))
                using (var matlabkey64 = basekey64.OpenSubKey(matlabkey))
                {
                    if (matlabkey64 != null && IsMatlabVersionInstalled(matlabkey64))
                        return true;
                }

                // 打开 32 位注册表视图
                using (var basekey32 = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32))
                using (var matlabkey32 = basekey32.OpenSubKey(matlabkey))
                {
                    if (matlabkey32 != null && IsMatlabVersionInstalled(matlabkey32))
                        return true;
                }
                #endregion

                #region 检查CurrentUser
                // 尝试打开 64 位注册表视图
                using (var basekey64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var matlabkey64 = basekey64.OpenSubKey(matlabkey))
                {
                    if (matlabkey64 != null && IsMatlabVersionInstalled(matlabkey64))
                        return true;
                }

                // 打开 32 位注册表视图
                using (var basekey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var matlabkey32 = basekey32.OpenSubKey(matlabkey))
                {
                    if (matlabkey32 != null && IsMatlabVersionInstalled(matlabkey32))
                        return true;
                }
                #endregion

                #region 检查LocalMachine
                // 尝试打开 64 位注册表视图
                using (var basekey64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var matlabkey64 = basekey64.OpenSubKey(matlabkey))
                {
                    if (matlabkey64 != null && IsMatlabVersionInstalled(matlabkey64))
                        return true;
                }

                // 打开 32 位注册表视图
                using (var basekey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var matlabkey32 = basekey32.OpenSubKey(matlabkey))
                {
                    if (matlabkey32 != null && IsMatlabVersionInstalled(matlabkey32))
                        return true;
                }
                #endregion

                #region 检查Users
                // 尝试打开 64 位注册表视图
                using (var basekey64 = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64))
                using (var matlabkey64 = basekey64.OpenSubKey(matlabkey))
                {
                    if (matlabkey64 != null && IsMatlabVersionInstalled(matlabkey64))
                        return true;
                }

                // 打开 32 位注册表视图
                using (var basekey32 = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry32))
                using (var matlabkey32 = basekey32.OpenSubKey(matlabkey))
                {
                    if (matlabkey32 != null && IsMatlabVersionInstalled(matlabkey32))
                        return true;
                }
                #endregion

                #region 检查CurrentConfig
                // 尝试打开 64 位注册表视图
                using (var basekey64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64))
                using (var matlabkey64 = basekey64.OpenSubKey(matlabkey))
                {
                    if (matlabkey64 != null && IsMatlabVersionInstalled(matlabkey64))
                        return true;
                }

                // 打开 32 位注册表视图
                using (var basekey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry32))
                using (var matlabkey32 = basekey32.OpenSubKey(matlabkey))
                {
                    if (matlabkey32 != null && IsMatlabVersionInstalled(matlabkey32))
                        return true;
                }
                #endregion

                return false;
            }
            catch
            {
            }

            return false;
        }

        private static Boolean IsMatlabVersionInstalled(RegistryKey matlabKey)
        {
            // 获取安装的 Matlab 版本信息
            String[] subKeyNames = matlabKey.GetSubKeyNames();
            foreach (String subKeyName in subKeyNames)
            {
                if (Version.TryParse(subKeyName, out Version matlabVersion))
                {
                    // 检查 Matlab 版本是否大于等于 2015
                    if (matlabVersion.Major >= 9) // Matlab 2015 对应的主版本号是 9
                        return true;
                }
            }
            return false;
        }


        public Boolean WriteMatFile()
        {


            return false;
        }
    }
}
