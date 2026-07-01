using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class CoefficientsParams : ICaliData
    {
        public CoefficientsParams()
        {

        }

        public static CoefficientsParams Default = new();

        private const Int32 _KeyStrLen = 64;
        private Dictionary<String, Double[]> _ParamsTable = new();

        public Dictionary<String, Double[]> ParamsTable
        {
            get { return _ParamsTable; }
        }

        public CaliDataType DataType => CaliDataType.CoefficientsParams;

        public Int32 TotalBytes
        {
            get
            {
                Int32 size = 0;
                foreach (Double[] coe in _ParamsTable.Values)
                {
                    size += _KeyStrLen + sizeof(Int32) + coe.Length * sizeof(Double);
                }
                return size;
            }
        }

        public Int32 OriginTotleBytes
        {
            get;
            set;
        }

        public String[] AllNames => _ParamsTable.Keys.ToArray();

        public Double[] this[String paramName]
        {
            get => _ParamsTable.ContainsKey(paramName) ? _ParamsTable[paramName] : new Double[0];
            set => _ParamsTable[paramName] = value;
        }

        public Byte[] Serialize()
        {
            MemoryStream memorystream = new MemoryStream();
            foreach ((String paramname, Double[] paraminfo) in _ParamsTable)
            {
                if(paramname.Contains("?"))
                {
                    continue;
                }
                String writename = paramname.PadLeft(_KeyStrLen, ' ');
                memorystream.Write(Encoding.ASCII.GetBytes(writename));

                memorystream.Write(Helper.StructToBytes(paraminfo.Length));
                for (Int32 i = 0; i < paraminfo.Length; i++)
                {
                    memorystream.Write(Helper.StructToBytes(paraminfo[i]));
                }
            }
            Byte[] result = memorystream.ToArray();
            memorystream.Close();
            return result;
        }

        public Byte[] SerializeOne(String Key)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (_ParamsTable.ContainsKey(Key))
            {
                String paramname = Key.PadLeft(_KeyStrLen, ' ');
                memoryStream.Write(Encoding.ASCII.GetBytes(paramname));
                Double[] paraminfo = _ParamsTable[Key];
                memoryStream.Write(Helper.StructToBytes(paraminfo.Length));
                for (Int32 i = 0; i < paraminfo.Length; i++)
                {
                    memoryStream.Write(Helper.StructToBytes(paraminfo[i]));
                }
            }
            Byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }

        public void Deserialize(Byte[] content)
        {
            if (!DeserializeDouble(content))
            {
                DeserializeInt(content);
            }
            CheckParams();
        }

        /// <summary>
        /// 反序列化Double类型的数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Boolean DeserializeDouble(Byte[] content)
        {
            try
            {
                Int32 id = 0;
                while (id + _KeyStrLen < content.Length)
                {
                    String namestr = Encoding.ASCII.GetString(content, id, _KeyStrLen).Trim();
                    id += _KeyStrLen;

                    Int32 datalen = Helper.BytesToStruct<Int32>(content, id) ?? 0;
                    Double[] data = new Double[datalen];

                    id += sizeof(Int32);
                    for (Int32 i = 0; i < data.Length; i++)
                    {
                        data[i] = Helper.BytesToStruct<Double>(content, id) ?? 0;
                        id += sizeof(Double);
                    }
                    _ParamsTable[namestr] = data;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 反序列化之前版本的数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private void DeserializeInt(Byte[] content)
        {
            try
            {
                Int32 id = 0;
                while (id + _KeyStrLen < content.Length)
                {
                    String namestr = Encoding.ASCII.GetString(content, id, _KeyStrLen).Trim();
                    id += _KeyStrLen;

                    Int32 datalen = Helper.BytesToStruct<Int32>(content, id) ?? 0;
                    Double[] data = new Double[datalen];

                    id += sizeof(Int32);
                    for (Int32 i = 0; i < data.Length; i++)
                    {
                        data[i] = Helper.BytesToStruct<Int32>(content, id) ?? 0;
                        id += sizeof(Int32);
                    }
                    _ParamsTable[namestr] = data;
                    Serialize();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 单条数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Boolean DeserializeTheOne(Byte[] content)
        {
            Int32 id = 0;
            try
            {
                String namestr = Encoding.ASCII.GetString(content, id, _KeyStrLen).Trim();
                id += _KeyStrLen;

                Int32 datalen = Helper.BytesToStruct<Int32>(content, id) ?? 0;
                Double[] data = new Double[datalen];

                id += sizeof(Int32);
                if ((id + datalen * 8) == content.Length)
                {
                    for (Int32 i = 0; i < data.Length; i++)
                    {
                        data[i] = Helper.BytesToStruct<Double>(content, id) ?? 0;
                        id += sizeof(Double);
                    }
                    _ParamsTable[namestr] = data;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public Boolean CheckParams()
        {
            Boolean isremove = false;
            String[] keys = _ParamsTable.Keys.ToArray();
            foreach (var item in keys)
            {
                if (item.ToUpper().Contains("@") || item.ToUpper().Contains(" "))
                {
                    _ParamsTable.Remove(item);
                    isremove = true;
                }
            }
            return isremove;
        }

        /// <summary>
        /// 移除系数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveCoefficientsParam(String key)
        {
            if (key != null && _ParamsTable.ContainsKey(key))
            {
                _ParamsTable.Remove(key);
            }
            return _ParamsTable.ContainsKey(key);
        }

        public bool ClearCoefficientsParam()
        {
            _ParamsTable.Clear();
            Default["AmpFreq_C1_100mV"] = new Double[] { 1, 2, 3 };
            return true;
        }

    }
}
