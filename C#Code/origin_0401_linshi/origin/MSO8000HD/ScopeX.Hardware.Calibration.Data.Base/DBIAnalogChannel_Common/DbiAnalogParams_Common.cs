using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class DbiAnalogParams_Common : ICaliData
    {
        public static DbiAnalogParams_Common Default = new DbiAnalogParams_Common();
        private DictionaryParams<DbiAnalogChannelItem_Common> _ParamsInfo = new(CaliConstants.KeyStrLen);

        public DbiAnalogChannelItem_Common this[String paramName]
        {
            get => _ParamsInfo[paramName];
            set => _ParamsInfo[paramName] = value;
        }
        public void Remove(String key)
        {
            _ParamsInfo.Remove(key.Trim());
        }
        public void Clear()
        {
            _ParamsInfo.Clear();
        }
        public String[] AllNames => _ParamsInfo.AllNames;
        internal const int MaxItemCount = 32;
        public CaliDataType DataType => CaliDataType.DbiAnalogParams_Common;

        public int TotalBytes => _ParamsInfo.AllNames.Length * (CaliConstants.KeyStrLen + MaxItemCount * sizeof(Int64));

        public int OriginTotleBytes { get; set; }

        int _SingleBytes = CaliConstants.KeyStrLen + MaxItemCount * sizeof(Int64);
        int _KeyStrLen = CaliConstants.KeyStrLen;
        public void Deserialize(Byte[] content)
        {
            _ParamsInfo.Clear();
            for (Int32 i = 0; i + _SingleBytes <= content.Length; i += _SingleBytes)
            {
                String namestr = Encoding.ASCII.GetString(content, i, _KeyStrLen).Trim();
                DbiAnalogChannelItem_Common newRow = new DbiAnalogChannelItem_Common();
                for (int j = 0; j < MaxItemCount; j++)
                {
                    Int64? d = Helper.BytesToStruct<Int64>(content, i + CaliConstants.KeyStrLen + j * sizeof(Int64));
                    if (d != null)
                        newRow[j] = d ?? 0;
                }
                _ParamsInfo[namestr] = newRow;
            }
        }

        public Byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            var allKeys = AllNames;
            foreach (var key in allKeys)
            {
                String writename = key.PadLeft(_KeyStrLen, ' ');
                memoryStream.Write(Encoding.ASCII.GetBytes(writename));
                Int64 d;
                for (int i = 0; i < MaxItemCount; i++)
                    memoryStream.Write(Helper.StructToBytes(_ParamsInfo[writename.Trim()][i]));
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
    }
}
