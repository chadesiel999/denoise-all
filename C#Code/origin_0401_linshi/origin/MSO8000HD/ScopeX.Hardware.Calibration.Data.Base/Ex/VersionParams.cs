using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    internal class VersionParams<G,I> where G : Enum where I : Enum
    {
        internal VersionParams(
            G defaultGodVersion, Dictionary<G, Object> godParams,
            I defaultItemVersion, Dictionary<I, DictionaryParams> itemParams,
            Int32 godVersionStrLen = CaliConstants.KeyStrLen,
            Int32 itemVersionStrLen = CaliConstants.KeyStrLen,
            Int32 calcTimeStrLen = CaliConstants.KeyStrLen)
        { 
            GodVersion = defaultGodVersion;
            _GodParamsDefine = godParams;

            ItemVersion = defaultItemVersion;
            _ItemParamsDefine = itemParams;

            _GodVersionStrLen = godVersionStrLen;
            _ItemVersionStrLen = itemVersionStrLen;
            _CalcTimeStrLen = calcTimeStrLen;

            CalcTimeStr = String.Empty;
        }

        private Int32 _GodVersionStrLen;
        private Int32 _ItemVersionStrLen;
        private Int32 _CalcTimeStrLen;
        private Dictionary<G, Object> _GodParamsDefine;
        private Dictionary<I, DictionaryParams> _ItemParamsDefine;

        internal G GodVersion { get; set; }
        internal I ItemVersion { get; set; }
        internal String CalcTimeStr { get; set; }
        internal Type? GodType
        {
            get 
            {
                if (_GodParamsDefine.ContainsKey(GodVersion))
                    return _GodParamsDefine[GodVersion].GetType();
                return null;
            }
        }
        internal Type? ItemType
        {
            get
            {
                if (_ItemParamsDefine.ContainsKey(ItemVersion))
                    return _ItemParamsDefine[ItemVersion].ItemType;
                return null;
            }
        }
        internal Object? GodParams
        {
            get
            {
                if (_GodParamsDefine.ContainsKey(GodVersion))
                    return _GodParamsDefine[GodVersion];
                return null;
            }
            set
            {
                // 类型强保护，避免被乱改
                if (_GodParamsDefine.ContainsKey(GodVersion) && value != null &&
                    value.GetType() == _GodParamsDefine[GodVersion].GetType())
                {
                    _GodParamsDefine[GodVersion] = value;
                }
            }
        }

        internal Object? this[String paramName]
        {
            get
            {
                if (_ItemParamsDefine.ContainsKey(ItemVersion))
                {
                    return _ItemParamsDefine[ItemVersion][paramName];
                }
                return null;
            }
            set
            {
                // 类型强保护，避免被乱改
                if (_ItemParamsDefine.ContainsKey(ItemVersion) && value != null &&
                    value.GetType() == _ItemParamsDefine[ItemVersion].ItemType)
                {
                    _ItemParamsDefine[ItemVersion][paramName] = value;
                }
            }
        }

        internal String[] AllItemNames
        {
            get
            {
                if (_ItemParamsDefine.ContainsKey(ItemVersion))
                    return _ItemParamsDefine[ItemVersion].AllNames;
                return new String[0];
            }
        }

        internal Int32 TotalBytes
        {
            get
            {
                Int32 bytescnt = _GodVersionStrLen + _ItemVersionStrLen + _CalcTimeStrLen;

                if (GodType != null)
                    bytescnt += Marshal.SizeOf(GodType);

                if (_ItemParamsDefine.ContainsKey(ItemVersion))
                    bytescnt += _ItemParamsDefine[ItemVersion].TotalBytes;

                return bytescnt;
            }
        }

        internal void Deserialize(Byte[] content)
        {
            Int32 posid = 0;

            String godversionstr = Encoding.ASCII.GetString(content, posid, _GodVersionStrLen).Trim();
            Boolean godversionflag = Enum.TryParse(typeof(G), godversionstr, out Object? godversion);
            if (godversionflag && godversion != null && godversion is G)
                GodVersion = (G)godversion;
            posid += _GodVersionStrLen;

            String itemversionstr = Encoding.ASCII.GetString(content, posid, _ItemVersionStrLen).Trim();
            Boolean itemversionflag = Enum.TryParse(typeof(I), itemversionstr, out Object? itemversion);
            if (itemversionflag && itemversion != null && itemversion is I)
                ItemVersion = (I)itemversion;
            posid += _ItemVersionStrLen;

            CalcTimeStr = Encoding.ASCII.GetString(content, posid, _CalcTimeStrLen).Trim();
            posid += _CalcTimeStrLen;

            if (GodType != null)
            {
                var godparams = Helper.BytesToStruct(content, posid, GodType);
                if (godparams != null && godparams.GetType() == GodType)
                {
                    _GodParamsDefine[GodVersion] = godparams;
                    posid += Marshal.SizeOf(GodType);
                }
            }

            if (_ItemParamsDefine.ContainsKey(ItemVersion))
            {
                _ItemParamsDefine[ItemVersion].Deserialize(content.Skip(posid).ToArray());
            }
        }

        internal Byte[] Serialize()
        {
            List<Byte> result = new();

            String godversionstr = GodVersion.ToString().PadLeft(_GodVersionStrLen, ' ');
            result.AddRange(Encoding.ASCII.GetBytes(godversionstr));

            String itemversionstr = ItemVersion.ToString().PadLeft(_ItemVersionStrLen, ' ');
            result.AddRange(Encoding.ASCII.GetBytes(itemversionstr));

            String calctimestr = CalcTimeStr.PadLeft(_CalcTimeStrLen, ' ');
            result.AddRange(Encoding.ASCII.GetBytes(calctimestr));

            if (GodParams != null)
                result.AddRange(Helper.StructToBytes(GodParams));

            if (_ItemParamsDefine.ContainsKey(ItemVersion))
            {
                result.AddRange(_ItemParamsDefine[ItemVersion].Serialize());
            }

            return result.ToArray();
        }

        /// <summary>
        /// 相同类型的属性进行拷贝，属性名使用映射表，若表中没有，则使用原名拷贝
        /// </summary>
        /// <param name="mapInfo"></param>
        /// <param name="nameMapTable"></param>
        internal void CopyGodParams(MapInfo<G> mapInfo, Dictionary<String, String>? nameMapTable = null)
        {
            if (_GodParamsDefine.ContainsKey(mapInfo.source) && _GodParamsDefine.ContainsKey(mapInfo.destination))
            {
                // 值类型到引用类型的强转，实现栈内存到堆内存的拷贝，确保CopyPropertyInfo反射的正确运行
                Object destinationparams = _GodParamsDefine[mapInfo.destination];
                CopyPropertyValue(_GodParamsDefine[mapInfo.source], destinationparams, nameMapTable);
                _GodParamsDefine[mapInfo.destination] = destinationparams;
            }
        }

        /// <summary>
        /// 将原表中的结构体拷贝到新结构体中，属性类型必须相同，属性名使用映射表
        /// </summary>
        /// <param name="sourceVersion"></param>
        /// <param name="destinationVersion"></param>
        /// <param name="destinationParams"></param>
        /// <param name="nameMapTable"></param>
        internal void CopyGodParams(G sourceVersion, G destinationVersion, Object destinationParams, Dictionary<String, String>? nameMapTable = null)
        {
            if (_GodParamsDefine.ContainsKey(sourceVersion))
            {
                CopyPropertyValue(_GodParamsDefine[sourceVersion], destinationParams, nameMapTable);
                _GodParamsDefine[destinationVersion] = destinationParams;
            }
        }

        /// <summary>
        /// 相同类型的属性进行拷贝，属性名使用映射表，若表中没有，则使用原名拷贝
        /// </summary>
        /// <param name="mapInfo"></param>
        /// <param name="nameMapTable"></param>
        internal void CopyItemParams(MapInfo<I> mapInfo, Dictionary<String, String>? nameMapTable = null)
        {
            if (_ItemParamsDefine.ContainsKey(mapInfo.source) && _ItemParamsDefine.ContainsKey(mapInfo.destination))
            {
                DictionaryParams sourceparams = _ItemParamsDefine[mapInfo.source];
                DictionaryParams desyinationparams = _ItemParamsDefine[mapInfo.destination];

                foreach (String itemname in sourceparams.AllNames)
                {
                    Object? sourceitem = sourceparams[itemname];
                    Object? destinationitem = desyinationparams[itemname] ?? Activator.CreateInstance(desyinationparams.ItemType);
                    if (sourceitem != null && destinationitem != null)
                    {
                        CopyPropertyValue(sourceitem, destinationitem, nameMapTable);
                        desyinationparams[itemname] = destinationitem;
                    }
                }
            }
        }

        /// <summary>
        /// todo，这个函数是否有存在的必要
        /// </summary>
        /// <param name="sourceVersion"></param>
        /// <param name="destinationVersion"></param>
        /// <param name="destinationItemType"></param>
        /// <param name="nameMapTable"></param>
        internal void CopyItemParams(I sourceVersion, I destinationVersion, Type destinationItemType, Dictionary<String, String>? nameMapTable = null)
        { 
            
        }

        /// <summary>
        /// 属性拷贝，类型必须相同，名字进行映射，若映射表中没有，则尝试使用原名字进行拷贝；
        /// static的原因是该函数与具体的实例没有关系，只需要在内存中保留一份即可
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="nameMapTable">属性名字映射表，key：源结构体的属性名字，value：目标结构体的属性名字</param>
        internal static void CopyPropertyValue(Object source, Object destination, Dictionary<String, String>? nameMapTable)
        {
            PropertyInfo[] sourceinfos = source.GetType().GetProperties();
            foreach (PropertyInfo sourceinfo in sourceinfos)
            {
                String namestr = (nameMapTable?.ContainsKey(sourceinfo.Name) ?? false) ? nameMapTable[sourceinfo.Name] : sourceinfo.Name;
                PropertyInfo? destinationinfo = destination.GetType().GetProperty(namestr, sourceinfo.PropertyType);
                if (destinationinfo != null)
                {
                    destinationinfo.SetValue(destination, sourceinfo.GetValue(source));
                }
            }
        }
    }

    internal record MapInfo<T>(T source, T destination) where T : Enum;
}
