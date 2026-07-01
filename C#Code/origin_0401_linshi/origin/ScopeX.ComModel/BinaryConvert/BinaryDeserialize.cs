using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class BinaryDeserialize
        {
            private FileTypeHeader _FileType = FileTypeHeader.Extension;
            private IEnumerator<String>? _PropertisNames;
            //public Boolean ReadEnd { get; private set; } = false;
            internal static T? CreateInstance<T>() where T : class
            {
                T? value = default;
                ConstructorInfo? tempinfo = typeof(T).GetConstructors().FirstOrDefault();

                if (tempinfo == null)
                    throw new Exception(typeof(T).FullName + "无构造函数");

                if (tempinfo is ConstructorInfo constructor)
                {
                    Int32 parametercount = constructor.GetParameters().Length;

                    if (parametercount == 0)
                        value = Activator.CreateInstance<T>();
                    else
                        value = (T?)Activator.CreateInstance(typeof(T), new Object[parametercount]);
                }

                return value;
            }
            internal static Object? CreateInstance(Type type)
            {
                Object? value = null;
                ConstructorInfo? tempinfo = type.GetConstructors().FirstOrDefault();

                if (tempinfo == null)
                    throw new Exception(type.FullName + "无构造函数");

                if (tempinfo is ConstructorInfo constructor)
                {
                    Int32 parametercount = constructor.GetParameters().Length;

                    if (parametercount == 0)
                        value = Activator.CreateInstance(type);
                    else
                        value = Activator.CreateInstance(type, new Object[parametercount]);
                }
                return value;
            }
            private BinaryDeserialize(FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                _FileType = fileType;
            }
            static BinaryDeserialize()
            {

            }
            internal static BinaryDeserialize GetDeserialization(FileTypeHeader fileType = FileTypeHeader.Extension) => new BinaryDeserialize(fileType);
            internal void DeserializationValue<T>(ref T? val, BinaryReader binaryReader)
            {
                if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length)
                    return;

                Boolean istopclass = false;
                String name = "";
                PropertyTypeEnum propertytype;

                if (_FileType == FileTypeHeader.Extension)
                {
                    Int32 namelenght = binaryReader.ReadInt32();

                    name = Encoding.GetString(binaryReader.ReadBytes(namelenght));

                    propertytype = (PropertyTypeEnum)binaryReader.ReadByte();

                    if (propertytype == PropertyTypeEnum.Class)
                        istopclass = binaryReader.ReadBoolean();

                    if (namelenght == 0)
                        return;
                }
                else
                {
                    propertytype = (PropertyTypeEnum)binaryReader.ReadByte();

                    if (propertytype == PropertyTypeEnum.Class)
                        istopclass = binaryReader.ReadBoolean();

                    if (!istopclass)
                    {
                        if (_PropertisNames == null)
                            _PropertisNames = GetPropertiesNames(val);

                        if (!_PropertisNames.MoveNext())
                            return;

                        name = _PropertisNames.Current;

                        if (String.IsNullOrEmpty(name))
                            return;
                    }
                }

                if (propertytype == PropertyTypeEnum.List || propertytype == PropertyTypeEnum.Array1)
                {
                    PropertyTypeEnum last = propertytype;
                    propertytype = GetPropertyTypeEnum(GetPropertyType(val, name));

                    if (propertytype == PropertyTypeEnum.NaN)
                        propertytype = last;
                }

                switch (propertytype)
                {
                    case PropertyTypeEnum.Bool:
                        SetPropertyValue(ref val, binaryReader.ReadBoolean(), name);
                        break;
                    case PropertyTypeEnum.String:
                        SetPropertyValue(ref val, Encoding.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32())), name);
                        break;
                    case PropertyTypeEnum.Array1:
                        DeserializationArray1(ref val, binaryReader, name);
                        break;
                    case PropertyTypeEnum.Array2:
                        DeserializationArray2(ref val, binaryReader, name);
                        break;
                    case PropertyTypeEnum.Byte:
                        SetPropertyValue(ref val, binaryReader.ReadByte(), name);
                        break;
                    case PropertyTypeEnum.Class:
                        if (istopclass)
                            val = (T?)DeserializationClass(val?.GetType(), binaryReader, binaryReader.ReadInt32());
                        else
                            SetPropertyValue(ref val, DeserializationClass(GetPropertyType(val, name), binaryReader, binaryReader.ReadInt32()), name);
                        break;
                    case PropertyTypeEnum.Double:
                        SetPropertyValue(ref val, binaryReader.ReadDouble(), name);
                        break;
                    case PropertyTypeEnum.Float:
                        SetPropertyValue(ref val, binaryReader.ReadSingle(), name);
                        break;
                    case PropertyTypeEnum.Int:
                        SetPropertyValue(ref val, binaryReader.ReadInt32(), name);
                        break;
                    case PropertyTypeEnum.Long:
                        SetPropertyValue(ref val, binaryReader.ReadInt64(), name);
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                    case PropertyTypeEnum.SByte:
                        SetPropertyValue(ref val, binaryReader.ReadSByte(), name);
                        break;
                    case PropertyTypeEnum.Short:
                        SetPropertyValue(ref val, binaryReader.ReadInt16(), name);
                        break;
                    case PropertyTypeEnum.UInt:
                        SetPropertyValue(ref val, binaryReader.ReadUInt32(), name);
                        break;
                    case PropertyTypeEnum.ULong:
                        SetPropertyValue(ref val, binaryReader.ReadUInt64(), name);
                        break;
                    case PropertyTypeEnum.UShort:
                        SetPropertyValue(ref val, binaryReader.ReadUInt16(), name);
                        break;
                    case PropertyTypeEnum.List:
                        DeserializationList(ref val, binaryReader, name);
                        break;
                }
            }

            IEnumerator<String> GetPropertiesNames<T>(T? value)
            {
                if (_PropertisNames != null)
                    return _PropertisNames;

                if (value == null)
                    return new List<String>().GetEnumerator();

                return BinarySerialize.GetMemberInfos(value)
                    .Select(x =>
                    {
                        OrderAttribute? order = x.GetCustomAttribute<OrderAttribute>();
                        return (order != null) ? (order.Order, BinarySerialize.GetMemberName(x)) : (0, BinarySerialize.GetMemberName(x));
                    })
                    .OrderBy(x => x.Item1)
                    .Select(x => x.Item2).GetEnumerator();
            }
            private Type? GetPropertyType<T>(T val, String name)
            {
                return val?.GetType().GetMembers(PROPERTYSEARCHFLAGS)
                    .Where(x => BinarySerialize.GetMemberName(x) == name && ((x.MemberType & MEMBERSEARCHTYPE) == x.MemberType)).Select(x =>
                {
                    if (x.MemberType == MemberTypes.Field)
                        return (x as FieldInfo)?.FieldType;
                    else
                        return (x as PropertyInfo)?.PropertyType;
                }).FirstOrDefault();
            }
            private Object? DeserializationClass(Type? type, BinaryReader binaryReader, Int32 procount)
            {
                if (type == null)
                    return null;

                Object? val = CreateInstance(type);

                if (_FileType == FileTypeHeader.Extension)
                {
                    for (Int32 i = 0; i < procount; i++)
                    {
                        DeserializationValue(ref val, binaryReader);
                    }
                }
                else
                {
                    BinaryDeserialize deserialization = new BinaryDeserialize(_FileType);

                    for (Int32 i = 0; i < procount; i++)
                    {
                        deserialization.DeserializationValue(ref val, binaryReader);
                    }
                }

                return val;
            }
            private void DeserializationArray1<T>(ref T val, BinaryReader binaryReader, String name)
            {
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 lenght = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (lenght == 0)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            Boolean[] tempbools = new Boolean[lenght];

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                tempbools[i] = (Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1;
                            }

                            SetPropertyValue(ref val, tempbools, name);
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        SetPropertyValue(ref val, binaryReader.ReadBytes((Int32)lenght), name);
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            SetPropertyValue(ref val, BytesToArray<Double>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            SetPropertyValue(ref val, BytesToArray<SByte>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            SetPropertyValue(ref val, BytesToArray<Int16>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            SetPropertyValue(ref val, BytesToArray<UInt16>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            SetPropertyValue(ref val, BytesToArray<Int32>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            SetPropertyValue(ref val, BytesToArray<UInt32>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            SetPropertyValue(ref val, BytesToArray<Int64>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            SetPropertyValue(ref val, BytesToArray<UInt64>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            SetPropertyValue(ref val, BytesToArray<Single>(binaryReader, lenght), name);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            SetPropertyValue(ref val, Encoding.GetString(tempbytes).Split(SEPARATOR), name);
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            Type? temptype = GetPropertyType(val, name);

                            if (temptype == null || temptype.GetElementType() == null)
                                return;

                            temptype = temptype.GetElementType();

                            if (temptype is Type propertytype)
                            {
                                Array temparray = Array.CreateInstance(propertytype, (Int32)lenght);

                                if (_FileType == FileTypeHeader.Extension)
                                    binaryReader.ReadBytes(binaryReader.ReadInt32());

                                binaryReader.ReadByte();
                                binaryReader.ReadBoolean();
                                Int32 procount = binaryReader.ReadInt32();

                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    temparray.SetValue(DeserializationClass(temptype, binaryReader, procount), i);
                                }

                                SetPropertyValue(ref val, temparray, name);
                            }
                        }
                        break;
                    case PropertyTypeEnum.List:
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }
            private T[] BytesToArray<T>(BinaryReader stream, Int64 lenght) where T : struct
            {
                if (typeof(T).IsClass || stream == null)
                    return Array.Empty<T>();

                Byte[] bytes = stream.ReadBytes((Int32)lenght * System.Runtime.InteropServices.Marshal.SizeOf<T>());
                T[] temparray = new T[lenght];
                Buffer.BlockCopy(bytes, 0, temparray, 0, bytes.Length);
                return temparray;
            }

            private T[,] BytesToArray<T>(BinaryReader stream, Int64 lenght1, Int64 lenght2) where T : struct
            {
                if (typeof(T).IsClass || stream == null)
                    return new T[0, 0];

                Byte[] bytes = stream.ReadBytes((Int32)(lenght1 * lenght2 * System.Runtime.InteropServices.Marshal.SizeOf<T>()));
                T[,] temparray = new T[lenght1, lenght2];
                Buffer.BlockCopy(bytes, 0, temparray, 0, bytes.Length);
                return temparray;
            }

            private void DeserializationArray1(ref Array array, BinaryReader binaryReader)
            {
                if (_FileType == FileTypeHeader.Extension)
                    binaryReader.ReadBytes(binaryReader.ReadInt32());

                binaryReader.ReadByte();
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 arraylenght = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (arraylenght == 0)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            array = new Boolean[arraylenght];

                            for (Int32 i = 0; i < arraylenght; i++)
                            {
                                array.SetValue((Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1, i);
                            }
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        array = binaryReader.ReadBytes((Int32)arraylenght);
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            array = BytesToArray<Double>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            array = BytesToArray<SByte>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            array = BytesToArray<Int16>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            array = BytesToArray<UInt16>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            array = BytesToArray<Int32>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            array = BytesToArray<UInt32>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            array = BytesToArray<Int64>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            array = BytesToArray<UInt64>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            array = BytesToArray<Single>(binaryReader, arraylenght);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            array = Encoding.GetString(binaryReader.ReadBytes((Int32)bytescount)).Split(SEPARATOR);
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            Type? elementType = array.GetType().GetElementType();

                            if (elementType == null)
                                return;

                            array = Array.CreateInstance(elementType, (Int32)arraylenght);

                            if (_FileType == FileTypeHeader.Extension)
                                binaryReader.ReadBytes(binaryReader.ReadInt32());

                            binaryReader.ReadByte();
                            binaryReader.ReadBoolean();
                            Int32 procount = binaryReader.ReadInt32();

                            for (Int32 i = 0; i < arraylenght; i++)
                            {
                                array.SetValue(DeserializationClass(elementType, binaryReader, procount), i);
                            }
                        }
                        break;
                    case PropertyTypeEnum.List:
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }
            private void DeserializationList<T>(ref T val, BinaryReader binaryReader, String name)
            {
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 lenght = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (lenght == 0)
                    return;

                Type? temptype = GetPropertyType(val, name);

                if (temptype == null)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            IList? temp = (IList?)Activator.CreateInstance(temptype);

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                temp?.Add((Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1);
                            }

                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, binaryReader.ReadBytes((Int32)lenght)), name);
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            Double[] temp = BytesToArray<Double>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            SByte[] temp = BytesToArray<SByte>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            Int16[] temp = BytesToArray<Int16>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            UInt16[] temp = BytesToArray<UInt16>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            Int32[] temp = BytesToArray<Int32>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            UInt32[] temp = BytesToArray<UInt32>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            Int64[] temp = BytesToArray<Int64>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            UInt64[] temp = BytesToArray<UInt64>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            Single[] temp = BytesToArray<Single>(binaryReader, lenght);
                            SetPropertyValue(ref val, Activator.CreateInstance(temptype, temp), name);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            SetPropertyValue(ref val, (IList)Encoding.GetString(tempbytes).Split(SEPARATOR).ToList(), name);
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        {
                            IList? templist = (IList?)Activator.CreateInstance(temptype);

                            if (temptype.GenericTypeArguments[0].GetElementType() is Type elementtype)
                            {
                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    Array tempval = Array.CreateInstance(elementtype, 0);
                                    DeserializationArray1(ref tempval, binaryReader);
                                    templist?.Add(tempval);
                                }

                                SetPropertyValue(ref val, templist, name);
                            }
                        }
                        break;
                    case PropertyTypeEnum.Array2:
                        {
                            IList? templist = (IList?)Activator.CreateInstance(temptype);

                            if (temptype.GenericTypeArguments[0].GetElementType() is Type elementtype)
                            {
                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    Array temparray = Array.CreateInstance(elementtype, 0);
                                    DeserializationArray2(ref temparray, binaryReader);
                                    templist?.Add(temparray);
                                }

                                SetPropertyValue(ref val, templist, name);
                            }
                        }
                        break;
                    case PropertyTypeEnum.Class:
                        {

                            if (_FileType == FileTypeHeader.Extension)
                                binaryReader.ReadBytes(binaryReader.ReadInt32());

                            binaryReader.ReadByte();
                            binaryReader.ReadBoolean();
                            Int32 procount = binaryReader.ReadInt32();
                            IList? templist = (IList?)Activator.CreateInstance(temptype);

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                templist?.Add(DeserializationClass(temptype.GenericTypeArguments[0], binaryReader, procount));
                            }

                            SetPropertyValue(ref val, templist, name);
                        }
                        break;
                    case PropertyTypeEnum.List:
                        {
                            IList? temp = (IList?)Activator.CreateInstance(temptype);

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                IList? templist = (IList?)Activator.CreateInstance(temptype.GenericTypeArguments[0]);
                                DeserializationList(ref templist, binaryReader);
                                temp?.Add(templist);
                            }

                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }


            private void DeserializationList(ref IList? list, BinaryReader binaryReader)
            {
                if (_FileType == FileTypeHeader.Extension)
                    binaryReader.ReadBytes(binaryReader.ReadInt32());

                binaryReader.ReadByte();
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 lenght = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (lenght == 0 || list == null)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                list?.Add((Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1);
                            }
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            list = (IList?)Activator.CreateInstance(list.GetType(), binaryReader.ReadBytes((Int32)lenght));
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            Double[] temp = BytesToArray<Double>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            SByte[] temp = BytesToArray<SByte>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            Int16[] temp = BytesToArray<Int16>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            UInt16[] temp = BytesToArray<UInt16>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            Int32[] temp = BytesToArray<Int32>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            UInt32[] temp = BytesToArray<UInt32>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            Int64[] temp = BytesToArray<Int64>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            UInt64[] temp = BytesToArray<UInt64>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            Single[] temp = BytesToArray<Single>(binaryReader, lenght);
                            list = (IList?)Activator.CreateInstance(list.GetType(), temp);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            list = (IList?)Activator.CreateInstance(list.GetType(), Encoding.GetString(binaryReader.ReadBytes((Int32)bytescount)).Split(SEPARATOR));
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        {
                            if (list.GetType().GenericTypeArguments[0].GetElementType() is Type elementtype)
                            {
                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    Array temparray = Array.CreateInstance(elementtype, 0);
                                    DeserializationArray1(ref temparray, binaryReader);
                                    list?.Add(temparray);
                                }
                            }
                        }
                        break;
                    case PropertyTypeEnum.Array2:
                        {
                            if (list.GetType().GenericTypeArguments[0].GetElementType() is Type elementtype)
                            {
                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    Array temparray = Array.CreateInstance(elementtype, 0);
                                    DeserializationArray1(ref temparray, binaryReader);
                                    list?.Add(temparray);
                                }
                            }
                        }
                        break;
                    case PropertyTypeEnum.Class:
                        {

                            if (_FileType == FileTypeHeader.Extension)
                                binaryReader.ReadBytes(binaryReader.ReadInt32());

                            binaryReader.ReadByte();
                            binaryReader.ReadBoolean();
                            Int32 procount = binaryReader.ReadInt32();

                            for (Int32 i = 0; i < lenght; i++)
                            {
                                list?.Add(DeserializationClass(list.GetType().GenericTypeArguments[0], binaryReader, procount));
                            }
                        }
                        break;
                    case PropertyTypeEnum.List:
                        {
                            if (list is IList temp)
                            {
                                for (Int32 i = 0; i < lenght; i++)
                                {
                                    IList? templist = (IList?)Activator.CreateInstance(temp.GetType().GenericTypeArguments[0]);
                                    DeserializationList(ref templist, binaryReader);
                                    list?.Add(templist);
                                }
                            }
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }

            private void DeserializationArray2<T>(ref T val, BinaryReader binaryReader, String name)
            {
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 lenght1 = binaryReader.ReadInt64();
                Int64 lenght2 = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (lenght1 == 0 || lenght2 == 0)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            Boolean[,] booltemparray = new Boolean[lenght1, lenght2];
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);

                            for (Int32 i = 0; i < lenght1 * lenght2; i++)
                            {
                                booltemparray[i / lenght2, i % lenght2] = (Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1;
                            }

                            SetPropertyValue(ref val, booltemparray, name);
                        }
                        break;
                    case PropertyTypeEnum.List:
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            SByte[,] temp = BytesToArray<SByte>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            Byte[,] temp = BytesToArray<Byte>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            Int16[,] temp = BytesToArray<Int16>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            UInt16[,] temp = BytesToArray<UInt16>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            Int32[,] temp = BytesToArray<Int32>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            UInt32[,] temp = BytesToArray<UInt32>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            Int64[,] temp = BytesToArray<Int64>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            UInt64[,] temp = BytesToArray<UInt64>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            Single[,] temp = BytesToArray<Single>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            Double[,] temp = BytesToArray<Double>(binaryReader, lenght1, lenght2);
                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            String[] strs = Encoding.GetString(tempbytes).Split(SEPARATOR);
                            String[,] temp = new String[lenght1, lenght2];

                            for (Int32 i = 0; i < lenght1; i++)
                            {
                                for (Int32 j = 0; j < lenght2; j++)
                                    temp[i, j] = strs[i * lenght2 + j];
                            }

                            SetPropertyValue(ref val, temp, name);
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            Type? temppropertytype = GetPropertyType(val, name);

                            if (temppropertytype == null || temppropertytype.GetElementType() == null)
                                return;

                            if (temppropertytype.GetElementType() is Type elementtype)
                            {
                                Array temparray = Array.CreateInstance(elementtype, (Int32)lenght1, (Int32)lenght2);

                                if (_FileType == FileTypeHeader.Extension)
                                    binaryReader.ReadBytes(binaryReader.ReadInt32());

                                binaryReader.ReadByte();
                                binaryReader.ReadBoolean();
                                Int32 procount = binaryReader.ReadInt32();

                                for (Int32 i = 0; i < lenght1; i++)
                                {
                                    for (Int32 j = 0; j < lenght2; j++)
                                    {
                                        temparray.SetValue(DeserializationClass(elementtype, binaryReader, procount), i, j);
                                    }
                                }

                                SetPropertyValue(ref val, temparray, name);
                            }
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }


            private void DeserializationArray2(ref Array array, BinaryReader binaryReader)
            {
                if (_FileType == FileTypeHeader.Extension)
                    binaryReader.ReadBytes(binaryReader.ReadInt32());

                binaryReader.ReadByte();
                PropertyTypeEnum valuetype = (PropertyTypeEnum)binaryReader.ReadByte();

                if (valuetype == PropertyTypeEnum.NaN)
                    return;

                Int64 lenght1 = binaryReader.ReadInt64();
                Int64 lenght2 = binaryReader.ReadInt64();
                Int64 bytescount = binaryReader.ReadInt64();

                if (lenght1 == 0 || lenght2 == 0)
                    return;

                switch (valuetype)
                {
                    case PropertyTypeEnum.Bool:
                        {
                            array = BytesToArray<Boolean>(binaryReader, lenght1, lenght2);
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);

                            for (Int32 i = 0; i < lenght1 * lenght2; i++)
                            {
                                array.SetValue((Byte)(tempbytes[i / 8] << (7 - (i % 8))) >> 7 == 1, i / lenght2, i % lenght2);
                            }
                        }
                        break;
                    case PropertyTypeEnum.List:
                        break;
                    case PropertyTypeEnum.SByte:
                        {
                            array = BytesToArray<SByte>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Byte:
                        {
                            array = BytesToArray<Byte>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Short:
                        {
                            array = BytesToArray<Int16>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.UShort:
                        {
                            array = BytesToArray<UInt16>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Int:
                        {
                            array = BytesToArray<Int32>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.UInt:
                        {
                            array = BytesToArray<UInt32>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Long:
                        {
                            array = BytesToArray<Int64>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.ULong:
                        {
                            array = BytesToArray<UInt64>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Float:
                        {
                            array = BytesToArray<Single>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.Double:
                        {
                            array = BytesToArray<Double>(binaryReader, lenght1, lenght2);
                        }
                        break;
                    case PropertyTypeEnum.String:
                        {
                            Byte[] tempbytes = binaryReader.ReadBytes((Int32)bytescount);
                            String[] strs = Encoding.GetString(tempbytes).Split(SEPARATOR);
                            array = new String[lenght1, lenght2];

                            for (Int32 i = 0; i < lenght1; i++)
                            {
                                for (Int32 j = 0; j < lenght2; j++)
                                    array.SetValue(strs[i * lenght2 + j], i, j);
                            }
                        }
                        break;
                    case PropertyTypeEnum.Array1:
                        break;
                    case PropertyTypeEnum.Array2:
                        break;
                    case PropertyTypeEnum.Class:
                        {
                            Type? temppropertytype = array.GetType().GetElementType();

                            if (temppropertytype == null)
                                return;

                            array = Array.CreateInstance(temppropertytype, (Int32)lenght1, (Int32)lenght2);


                            if (_FileType == FileTypeHeader.Extension)
                                binaryReader.ReadBytes(binaryReader.ReadInt32());

                            binaryReader.ReadByte();
                            binaryReader.ReadBoolean();

                            Int32 procount = binaryReader.ReadInt32();

                            for (Int32 i = 0; i < lenght1; i++)
                            {
                                for (Int32 j = 0; j < lenght2; j++)
                                {
                                    array.SetValue(DeserializationClass(temppropertytype, binaryReader, procount), i, j);
                                }
                            };
                        }
                        break;
                    case PropertyTypeEnum.NaN:
                        break;
                }
            }
            private void SetPropertyValue<T, V>(ref T value, V val, String name)
            {
                if (value == null)
                    return;

                try
                {

                    MemberInfo? memberinfo = value.GetType().GetMembers(PROPERTYSEARCHFLAGS)
                        .Where(x => name == BinarySerialize.GetMemberName(x) && (x.MemberType & MEMBERSEARCHTYPE) == x.MemberType)
                        .Where(x =>
                        {
                            SeializableAttribute? fileSeializable = x.GetCustomAttribute<SeializableAttribute>();

                            if (fileSeializable == null)
                                return true;
                            else
                                return !fileSeializable.Ignore;

                        }).FirstOrDefault();

                    if (memberinfo != null)
                    {
                        if (memberinfo.MemberType == MemberTypes.Field)
                        {
                            if (memberinfo is FieldInfo fi)
                            {
                                try
                                {
                                    fi.SetValue(value, val);
                                }
                                catch (ArgumentException)
                                {
                                    Type ftp = fi.FieldType;
                                    Type? underlyingType = Nullable.GetUnderlyingType(fi.FieldType);

                                    if (underlyingType != null)
                                    {
                                        ftp = underlyingType;
                                    }

                                    if (val != null && ftp.IsEnum && val.GetType() != ftp)
                                    {
                                        if (Enum.TryParse(ftp, val.ToString(), out object? result))
                                        {
                                            fi.SetValue(value, result);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        else
                        {
                            if (memberinfo is PropertyInfo propertyInfo)
                            {
                                if (propertyInfo.CanWrite)
                                    propertyInfo.SetValue(value, val);
                                else
                                    value.GetType().GetField($"<{memberinfo.Name}>k__BackingField", PROPERTYSEARCHFLAGS | BindingFlags.NonPublic)?.SetValue(value, val);
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
}
