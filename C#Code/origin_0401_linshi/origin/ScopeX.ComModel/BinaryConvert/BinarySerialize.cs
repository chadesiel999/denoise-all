using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private static class BinarySerialize
        {
            internal static List<BaseProperty> SerializationClassProperties(Object? val)
            {
                List<BaseProperty> _list = new List<BaseProperty>();

                if (val == null)
                    return _list;

                GetMemberInfos(val)
                    .Select(x =>
                    {
                        OrderAttribute? orderattribute = x.GetCustomAttribute<OrderAttribute>();
                        Int32 order = orderattribute == null ? 0 : orderattribute.Order;

                        if (x.MemberType == MemberTypes.Field)
                            return (order, GetMemberName(x), (x as FieldInfo)?.GetValue(val),(x as FieldInfo)?.FieldType);
                        else 
                            return (order,GetMemberName(x), (x as PropertyInfo)?.GetValue(val), (x as PropertyInfo)?.PropertyType);

                    })
                    .OrderBy(x=>x.order)
                    .ToList().ForEach(x =>
                    {
                        List<BaseProperty> result = SerializationValueProperties(x.Item3, x.Item2, x.Item4);

                        _list.AddRange(result);
                    });

                return _list;
            }

            internal static IEnumerable<MemberInfo> GetMemberInfos(Object? value)
            {
                if (value == null)
                    return new List<MemberInfo>();

                return GetMemberInfos(value.GetType());
            }

            internal static IEnumerable<MemberInfo> GetMemberInfos(Type? type)
            {
                if (type == null)
                    return new List<MemberInfo>();                

                return type.GetMembers(PROPERTYSEARCHFLAGS)
                    .Where(x => (x.MemberType & MEMBERSEARCHTYPE) == x.MemberType)
                    .Where(x =>
                    {
                        Boolean ispublic = true;

                        if (x is FieldInfo fieldInfo)
                            ispublic = fieldInfo.IsPublic;                        
                        else if ((x.GetType() as dynamic).DeclaredProperties is PropertyInfo[] properties)
                        {
                            PropertyInfo? propertyInfo = properties.Cast<PropertyInfo>().FirstOrDefault(x => x.Name == "BindingFlags");

                            if (propertyInfo != null && propertyInfo.GetValue(x) is BindingFlags bindingFlags)
                                ispublic = (bindingFlags & BindingFlags.NonPublic) != BindingFlags.NonPublic;
                            else
                                ispublic = false;
                        }

                        SeializableAttribute? fileSeializable = x.GetCustomAttribute<SeializableAttribute>();

                        if (ispublic)
                            return fileSeializable == null || !fileSeializable.Ignore;
                        else
                            return fileSeializable != null && !fileSeializable.Ignore;
                    });
            }

            private static List<BaseProperty> SerializationArray1Properties(Object? value, String name,Type? propertyType)
            {
                List<BaseProperty> _list = new List<BaseProperty>();
                Type? elementtype;

                if (value == null && propertyType ==null) 
                    return _list;

                if (value == null)
                    elementtype = propertyType;
                else
                    elementtype = value.GetType().GetElementType();

                if (elementtype == null)
                    return _list;

                PropertyTypeEnum propertytype = GetPropertyTypeEnum(elementtype);

                switch (propertytype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            if (value is Boolean[] tempvalue)
                            {
                                Byte[] temp = new Byte[(Int32)Math.Ceiling((Single)tempvalue.LongLength / 8)];

                                for (Int32 i = 0; i < tempvalue.LongLength; i++)
                                {
                                    if (tempvalue[i])
                                        temp[i / 8] |= (Byte)(1 << (i % 8));
                                }

                                _list.Add(new Array1Property(name, propertytype, temp)
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            if (value is Byte[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, tempvalue)
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            if (value is SByte[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            if (value is Int16[] tempvalue)
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            if (value is UInt32[] tempvalue)
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            if (value is UInt64[] tempvalue)
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            if (value is UInt16[] tempvalue)
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            if (value is Array array && array.GetType().GetElementType() is Type tempelementtype)
                            {
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>())
                                {
                                    Lenght = array.Length
                                });
                                _list.Add(new ClassProperty(name,GetMemberInfos(tempelementtype).Count()));

                                for (Int32 i = 0; i < array.Length; i++)
                                {
                                    _list.AddRange(SerializationClassProperties(array.GetValue(i)));
                                }
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            if (value is Double[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            if (value is Int32[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue)) { Lenght = tempvalue.LongLength });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            if (value is Int64[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            if (value is Single[] tempvalue)
                            {
                                _list.Add(new Array1Property(name, propertytype, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            if (value is String[] tempvalue)
                            {
                                Byte[] tempbytes = Encoding.GetBytes(String.Join(SEPARATOR, tempvalue));
                                _list.Add(new Array1Property(name, propertytype, tempbytes)
                                {
                                    Lenght = tempvalue.LongLength,
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        _list.Add(new NaNProperty(name));
                        break;
                }
                return _list;
            }

            internal static String GetMemberName(MemberInfo memberInfo)
            {
                SeializableAttribute? fileSeializable = memberInfo.GetCustomAttribute<SeializableAttribute>();

                if (fileSeializable == null) 
                    return memberInfo.Name;
                else
                    return (String.IsNullOrEmpty(fileSeializable.Name))?  memberInfo.Name:fileSeializable.Name;
            }


            private static List<BaseProperty> SerializationListProperties(Object? value, String name)
            {
                List<BaseProperty> _list = new List<BaseProperty>();

                if (value == null)
                    return _list;

                Type elementtype = value.GetType().GenericTypeArguments[0];
                PropertyTypeEnum propertytype = GetPropertyTypeEnum(elementtype);
                switch (propertytype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            if (value is IList<Boolean> tempvalue)
                            {
                                Byte[] temp = new Byte[(Int32)Math.Ceiling((Single)tempvalue.Count() / 8)];

                                for (Int32 i = 0; i < tempvalue.Count(); i++)
                                {
                                    if (tempvalue[i])
                                        temp[i / 8] |= (Byte)(1 << (i % 8));
                                }

                                _list.Add(new ListProperty(name, propertytype, temp)
                                {
                                    Lenght = tempvalue.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            if (value is IList<Byte> tempvalue)
                            {
                                _list.Add(new ListProperty(name, propertytype, tempvalue.ToArray())
                                {
                                    Lenght = tempvalue.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            if (value is IList<SByte> tempvalue)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempvalue))
                                {
                                    Lenght = tempvalue.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            if (value is IList<Int16> tempint16s)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempint16s)) { Lenght = tempint16s.LongCount() });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            if (value is IList<UInt32> tempuint32)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempuint32)) { Lenght = tempuint32.LongCount() });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            if (value is IList<UInt64> tempuint64)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempuint64)) { Lenght = tempuint64.LongCount() });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            if (value is IList<UInt16> tempuint16)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempuint16)) { Lenght = tempuint16.LongCount() });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        {
                            if (value is IList templist)
                            {
                                _list.Add(new ListProperty(name, propertytype, Array.Empty<Byte>())
                                {
                                    Lenght = templist.Count,
                                });
                                foreach (Array array in templist)
                                {
                                    _list.AddRange(SerializationArray1Properties(array, name,array.GetType().GetElementType()));
                                }
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.List:
                        {
                            if (value is IList templist)
                            {
                                _list.Add(new ListProperty(name, propertytype, Array.Empty<Byte>())
                                {
                                    Lenght = templist.Count,
                                });
                                foreach (IList array in templist)
                                {
                                    _list.AddRange(SerializationListProperties(array, name));
                                }
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Array2:
                        {
                            if (value is IList templist)
                            {
                                _list.Add(new ListProperty(name, propertytype, Array.Empty<Byte>())
                                {
                                    Lenght = templist.Count,
                                });
                                foreach (Array array in templist)
                                {
                                    _list.AddRange(SerializationArray2Bytes(array, name,array.GetType().GetElementType()));
                                }
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            if (value is IList templist)
                            {
                                _list.Add(new ListProperty(name, propertytype, Array.Empty<Byte>())
                                {
                                    Lenght = templist.Count
                                });
                                _list.Add(new ClassProperty(name,GetMemberInfos(value.GetType().GenericTypeArguments[0]).Count()));

                                for (Int32 i = 0; i < templist.Count; i++)
                                {
                                    _list.AddRange(SerializationClassProperties(templist[i]));
                                }
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            if (value is IList<Double> tempdouble)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempdouble)) { Lenght = tempdouble.LongCount() });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            if (value is IList<Int32> tempint32)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempint32))
                                {
                                    Lenght = tempint32.LongCount()
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            if (value is IList<Int64> tempint64)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempint64))
                                {
                                    Lenght = tempint64.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            if (value is IList<Single> tempsingle)
                            {
                                _list.Add(new ListProperty(name, propertytype, ListValueToBytes(tempsingle))
                                {
                                    Lenght = tempsingle.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            if (value is IList<String> tempstrs)
                            {
                                Byte[] tempvalue = Encoding.GetBytes(String.Join(SEPARATOR, tempstrs));
                                _list.Add(new ListProperty(name, propertytype, tempvalue)
                                {
                                    Lenght = tempstrs.LongCount(),
                                });
                            }
                            else
                                _list.Add(new Array1Property(name, propertytype, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        _list.Add(new NaNProperty(name));
                        break;
                }
                return _list;
            }

            private static Byte[] ListValueToBytes<T>(IList<T> list) where T:struct
            {
                if (typeof(T).IsClass || list ==null)
                    return Array.Empty<Byte>();

                T[] temparray = list.ToArray();
                Byte[] tempbytes = new Byte[System.Runtime.InteropServices.Marshal.SizeOf<T>() * temparray.LongLength];
                Buffer.BlockCopy(temparray, 0, tempbytes, 0, tempbytes.Length);
                return tempbytes;
            }

            private static Byte[] ArrayValueToBytes<T>(T[] array) where T : struct
            {
                if (typeof(T).IsClass || array == null)
                    return Array.Empty<Byte>();

                Byte[] tempbytes = new Byte[System.Runtime.InteropServices.Marshal.SizeOf<T>() * array.LongLength];
                Buffer.BlockCopy(array, 0, tempbytes, 0, tempbytes.Length);
                return tempbytes;
            }
            private static Byte[] ArrayValueToBytes<T>(T[,] array) where T :struct
            {
                if (typeof(T).IsClass || array == null)
                    return Array.Empty<Byte>();

                Byte[] tempbytes = new Byte[System.Runtime.InteropServices.Marshal.SizeOf<T>() * array.LongLength];
                Buffer.BlockCopy(array, 0, tempbytes, 0, tempbytes.Length);
                return tempbytes;
            }

            private static List<BaseProperty> SerializationArray2Bytes(Object? value, String name,Type? propertyType)
            {
                List<BaseProperty> _list = new List<BaseProperty>();
                Type? elementtype;

                if (value == null && propertyType ==null) 
                    return _list;

                if (value == null)
                    elementtype = propertyType;
                else
                    elementtype = value.GetType().GetElementType();
                PropertyTypeEnum property = GetPropertyTypeEnum(elementtype);
                switch (property)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            if (value is Boolean[,] tempvalue)
                            {
                                Byte[] temp = new Byte[(Int32)Math.Ceiling((Single)tempvalue.LongLength / 8)];
                                for (Int32 i = 0; i < tempvalue.LongLength; i++)
                                {
                                    if (tempvalue[i / tempvalue.GetLength(1), i % tempvalue.GetLength(1)])
                                        temp[i / 8] |= (Byte)(1 << (i % 8));
                                }
                                _list.Add(new Array2Property(name, property, temp)
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            if (value is Byte[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, tempvalue.Cast<Byte>().ToArray())
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            if (value is SByte[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            if (value is Int16[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            if (value is UInt32[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            if (value is UInt64[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            if (value is UInt16[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            if (value is Array array && array.GetType().GetElementType() is Type tempelementtype)
                            {
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>())
                                {
                                    Lenght1 = array.GetLongLength(0),
                                    Lenght2 = array.GetLongLength(1),
                                });
                                _list.Add(new ClassProperty(name, GetMemberInfos(tempelementtype).Count()));
                                array.Cast<Object>().ToList().ForEach(x =>
                                {
                                    _list.AddRange(SerializationClassProperties(x));
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            if (value is Double[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            if (value is Int32[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            if (value is Int64[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            if (value is Single[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, ArrayValueToBytes(tempvalue))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            if (value is String[,] tempvalue)
                            {
                                _list.Add(new Array2Property(name, property, Encoding.GetBytes(String.Join(SEPARATOR, tempvalue.Cast<String>())))
                                {
                                    Lenght1 = tempvalue.GetLongLength(0),
                                    Lenght2 = tempvalue.GetLongLength(1),
                                });
                            }
                            else
                                _list.Add(new Array2Property(name, property, Array.Empty<Byte>()));
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        _list.Add(new NaNProperty(name));
                        break;
                    case PropertyTypeEnum.List:
                        break;
                }
                return _list;
            }
            private static List<BaseProperty> SerializationValueProperties(Object? value, String name,Type? propertyType)
            {
                if(name =="_MeasItems")
                {

                }
                List<BaseProperty> _list = new List<BaseProperty>();
                PropertyTypeEnum propertytype = value==null?GetPropertyTypeEnum(propertyType): GetPropertyTypeEnum(value);
                Boolean isenum = false;

                if (value != null)
                    isenum = value.GetType().IsEnum;

                switch (propertytype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            if (value is Boolean tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, new Byte[1] { Convert.ToByte(tempvalue) }));
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            if(isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, new Byte[1] { (Byte)enumvalue.GetHashCode() }));
                            else if (value is Byte tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, new Byte[1] { tempvalue }));
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, new Byte[1] { (Byte)enumvalue.GetHashCode() }));
                            else if (value is SByte tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, new Byte[1] { (Byte)tempvalue }));
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((Double)enumvalue.GetHashCode())));
                            else if (value is Double tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((Single)enumvalue.GetHashCode())));
                            else if (value is Single tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((Int32)enumvalue.GetHashCode())));
                            else if (value is Int32 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((Int64)enumvalue.GetHashCode())));
                            else if (value is Int64 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((Int16)enumvalue.GetHashCode())));
                            else if (value is Int16 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        _list.Add(new NaNProperty(name));
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((UInt32)enumvalue.GetHashCode())));
                            else if (value is UInt32 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((UInt64)enumvalue.GetHashCode())));
                            else if (value is UInt64 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            if (isenum && value is Enum enumvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes((UInt16)enumvalue.GetHashCode())));
                            else if (value is UInt16 tempvalue)
                                _list.Add(new ValueProperty(name, propertytype, BitConverter.GetBytes(tempvalue)));
                        }
                        break;
                    case PropertyTypeEnum.String:
                        _list.Add(new StringProperty(name, Encoding.GetBytes(value + "")));
                        break;
                    case PropertyTypeEnum.Array1:
                        _list.AddRange(SerializationArray1Properties(value, name,propertyType?.GetElementType()));
                        break;
                    case PropertyTypeEnum.Array2:
                        _list.AddRange(SerializationArray2Bytes(value, name,propertyType?.GetElementType()));
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            if (value is Object tempvalue)
                            {
                                _list.Add(new ClassProperty(name,GetMemberInfos(tempvalue).Count()));
                                _list.AddRange(SerializationClassProperties(value));
                            }
                            else
                            {
                                _list.Add(new NaNProperty(name));
                            }
                        }
                        break;
                    case PropertyTypeEnum.List:
                        _list.AddRange(SerializationListProperties(value, name));
                        break;
                }
                return _list;
            }
        }
    }
}
