using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    internal class DictionaryParams<T> where T : struct
    {
        internal DictionaryParams(Int32 keyStrLen)                                                                        
        {
            _ParamsTable = new Dictionary<String, T>();
            _KeyStrLen = keyStrLen;
            _SingleBytes = keyStrLen + Marshal.SizeOf<T>();
        }

        private Dictionary<String, T> _ParamsTable;

        private Int32 _KeyStrLen;

        private Int32 _SingleBytes;

        internal Int32 TotalBytes => _ParamsTable.Count * _SingleBytes;

        internal T this[String paramName]
        { 
            get => _ParamsTable.ContainsKey(paramName) ? _ParamsTable[paramName] : new T();
            set => _ParamsTable[paramName] = value;
        }
        internal void Remove(String key)
        {
            if (_ParamsTable.ContainsKey(key))
                _ParamsTable.Remove(key);
        }
        internal void Clear()
        {
            _ParamsTable.Clear();
        }
        internal String[] AllNames => _ParamsTable.Keys.ToArray();

        internal void Deserialize(Byte[] content)
        {
            for (Int32 i = 0; i + _SingleBytes <= content.Length; i += _SingleBytes)
            {
                String namestr = Encoding.ASCII.GetString(content, i, _KeyStrLen).Trim();
                T? info = Helper.BytesToStruct<T>(content, i + _KeyStrLen);
                if (info != null)
                    _ParamsTable[namestr] = (T)info;
            }
        }

        internal Byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            foreach ((String paramname, T paraminfo) in _ParamsTable)
            {
                String writename = paramname.PadLeft(_KeyStrLen, ' ');
                memoryStream.Write(Encoding.ASCII.GetBytes(writename));
                memoryStream.Write(Helper.StructToBytes(paraminfo));
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
    }
}
