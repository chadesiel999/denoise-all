using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;
using System.Text.RegularExpressions;
namespace ScopeX.Hardware.Calibration.Tool
{
    interface IMainFormTabPage
    {
        void SetInstrumentInteract(IInstrumentSession? instrumentInteract);
        void RefreshData();
        CaliDataType CaliDataType { get=> CaliDataType.None; }
        List<ProductType> Used4ProductTypes { get; }
        bool CheckMatch(String keyID, String filter)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            if (filter.IndexOf('*') < 0)
                return keyID.StartsWith(filter);
            string pattern = Regex.Escape("*");
            int asteriskCharCount = Regex.Matches(filter, pattern).Count;
            if (asteriskCharCount == 1)
            {
                if (filter[0] == '*')
                {
                    filter = filter.Remove(0, 1);
                    return keyID.EndsWith(filter);
                }
                else
                {
                    filter = filter.Remove(filter.Length - 1, 1);
                    return keyID.StartsWith(filter);
                }
            }
            pattern = filter.Replace("*", "(.*)");
            return Regex.Match(keyID, pattern).Success;
        }
    }
}
