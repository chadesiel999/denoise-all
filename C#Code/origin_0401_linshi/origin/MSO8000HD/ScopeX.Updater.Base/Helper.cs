using System.Collections.Generic;
using ScopeX.ComModel;

namespace ScopeX.Updater.Base
{
    public class UpdateBaseHelper
    {
        public static HardwareVersionInfo GetVersionFromStr(string version)
        {
            string[] s_array = version.Split('.');
            List<string> s_list = new List<string>(s_array);
            s_list.Add("0");
            s_list.Add("0");
            s_list.Add("0");
            return new HardwareVersionInfo { Major = int.Parse(s_list[0]), Minor = int.Parse(s_list[1]), Build = int.Parse(s_list[2]), Revision = int.Parse(s_list[3]) };
        }

    }
}
