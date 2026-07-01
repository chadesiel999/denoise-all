using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ScopeX.U2
{
    internal static class DesignTimeHelper
    {
        private static Boolean? _IsAssemblyVisualStudio;
        private static Boolean? _IsLicenseDesignTime;
        private static Boolean? _IsProcessDevEnv;
        private static Boolean? _IsDesignerHosted;

        /// <summary>
        ///   Property <see cref="Form.DesignMode"/> does not correctly report if a nested <see cref="UserControl"/>
        ///   is in design mode.  InDesignMode is a corrected that property which .
        ///   (see https://connect.microsoft.com/VisualStudio/feedback/details/553305
        ///   and https://stackoverflow.com/a/2693338/238419 )
        /// </summary>
        public static Boolean InDesignMode(this Control userControl, String source = null)
        {
            return IsLicenseDesignTime
                        || IsProcessDevEnv
                        || IsExecutingAssemblyVisualStudio
                        || IsDesignerHosted(userControl);
        }

        private static Boolean IsExecutingAssemblyVisualStudio
          => _IsAssemblyVisualStudio
             ?? (_IsAssemblyVisualStudio = Assembly
               .GetExecutingAssembly()
               .Location.Contains(value: "VisualStudio"))
             .Value;

        private static Boolean IsLicenseDesignTime
          => _IsLicenseDesignTime
             ?? (_IsLicenseDesignTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime)
             .Value;

        private static Boolean IsDesignerHosted(
          Control control)
        {
            if (_IsDesignerHosted.HasValue)
            {
                return _IsDesignerHosted.Value;
            }

            while (control != null)
            {
                if (control.Site?.DesignMode == true)
                {
                    _IsDesignerHosted = true;
                    return true;
                }

                control = control.Parent;
            }

            _IsDesignerHosted = false;
            return false;
        }

        private static Boolean IsProcessDevEnv
          => _IsProcessDevEnv
             ?? (_IsProcessDevEnv = Process.GetCurrentProcess()
                                      .ProcessName == "devenv")
             .Value;
    }
}
