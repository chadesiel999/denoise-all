using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    /// <summary>
    /// 构造函数中的itemType参数必须是值类型
    /// </summary>
    internal class DictionaryParams
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyStrLen"></param>
        /// <param name="itemType">必须是值类型</param>
        internal DictionaryParams(Int32 keyStrLen, Type itemType, int itemNums)
        {
            _KeyStrLen = keyStrLen;
            ItemType = itemType;
            _SingleBytes = keyStrLen + Marshal.SizeOf(itemType);
            _ItemNums = itemNums;
        }

        internal Int32 _ItemNums;

        private Int32 _KeyStrLen;
        private Int32 _SingleBytes;
        private Dictionary<String, Object> _ParamsTable = new();

        internal Type ItemType { get; init; }

        internal Int32 TotalBytes => _ParamsTable.Count * _SingleBytes;

        internal Object? this[String paramName]
        {
            get => _ParamsTable.ContainsKey(paramName) ? _ParamsTable[paramName] : null;
            set
            {
                if (value != null && value.GetType() == ItemType)
                {
                    _ParamsTable[paramName] = value;
                }
            }

        }

        internal String[] AllNames => _ParamsTable.Keys.ToArray();

        internal void Deserialize(Byte[] content)
        {
            int itemcount = 0;
            for (int i = 0; i + _SingleBytes <= content.Length; i += _SingleBytes)
            {
                itemcount++;
                if (itemcount <= _ItemNums)
                {
                    String namestr = Encoding.ASCII.GetString(content, i, _KeyStrLen).Trim();
                    Object? info = Helper.BytesToStruct(content, i + _KeyStrLen, ItemType);
                    if (info != null) _ParamsTable[namestr] = info;
                }
                else
                {
                    String namestr = Encoding.ASCII.GetString(content, i, _KeyStrLen).Trim();
                    _ParamsTable.Remove(namestr);
                }
            }
        }

        internal Byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            foreach ((String paramname, Object paraminfo) in _ParamsTable)
            {
                String writename = paramname.PadLeft(_KeyStrLen, ' ');
                memoryStream.Write(Encoding.ASCII.GetBytes(writename));
                memoryStream.Write(Helper.StructToBytes(paraminfo));
            }
            Byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
    }

    //internal class DictionaryParams<T> where T : struct
    //{
    //    internal DictionaryParams(int keyStrLen)
    //    {
    //        _ParamsTable = new Dictionary<string, T>();
    //        _KeyStrLen = keyStrLen;
    //        _SingleBytes = keyStrLen + Marshal.SizeOf<T>();
    //    }

    //    private Dictionary<string, T> _ParamsTable;

    //    private int _KeyStrLen;

    //    private int _SingleBytes;

    //    internal int TotalBytes => _ParamsTable.Count * _SingleBytes;

    //    internal T this[string paramName]
    //    {
    //        get => _ParamsTable.ContainsKey(paramName) ? _ParamsTable[paramName] : new T();
    //        set => _ParamsTable[paramName] = value;
    //    }

    //    internal string[] AllNames => _ParamsTable.Keys.ToArray();

    //    internal void Deserialize(byte[] content)
    //    {
    //        for (int i = 0; i + _SingleBytes <= content.Length; i += _SingleBytes)
    //        {
    //            string namestr = Encoding.ASCII.GetString(content, i, _KeyStrLen).Trim();
    //            T? info = Helper.BytesToStruct<T>(content, i + _KeyStrLen);
    //            if (info != null)
    //                _ParamsTable[namestr] = (T)info;
    //        }
    //    }

    //    internal byte[] Serialize()
    //    {
    //        MemoryStream memoryStream = new MemoryStream();
    //        foreach ((string paramname, T paraminfo) in _ParamsTable)
    //        {
    //            string writename = paramname.PadLeft(_KeyStrLen, ' ');
    //            memoryStream.Write(Encoding.ASCII.GetBytes(writename));
    //            memoryStream.Write(Helper.StructToBytes(paraminfo));
    //        }
    //        byte[] result = memoryStream.ToArray();
    //        memoryStream.Close();
    //        return result;
    //    }
    //}
}
