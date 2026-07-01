using System;
using System.Collections;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private enum PropertyTypeEnum : Byte
        {
            Bool = 1,
            SByte,
            Byte,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            String,
            /// <summary>
            /// 一维数组
            /// </summary>
            Array1 = 200,
            /// <summary>
            /// 二维数组
            /// </summary>
            Array2,
            List,

            Class = 250,
            NaN = 255,
        }

        private static PropertyTypeEnum GetPropertyTypeEnum(Object? value)
        {
            if (value == null)
                return PropertyTypeEnum.NaN;

            return GetPropertyTypeEnum(value.GetType());
        }
        private static PropertyTypeEnum GetPropertyTypeEnum(Type? type)
        {
            if (type == null)
                return PropertyTypeEnum.NaN;

            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            if (type == typeof(SByte)) return PropertyTypeEnum.SByte;
            if (type == typeof(Byte)) return PropertyTypeEnum.Byte;
            if (type == typeof(Boolean)) return PropertyTypeEnum.Bool;
            if (type == typeof(Int16)) return PropertyTypeEnum.Short;
            if (type == typeof(UInt16)) return PropertyTypeEnum.UShort;
            if (type == typeof(Int32)) return PropertyTypeEnum.Int;
            if (type == typeof(UInt32)) return PropertyTypeEnum.UInt;
            if (type == typeof(UInt64)) return PropertyTypeEnum.ULong;
            if (type == typeof(Int64)) return PropertyTypeEnum.Long;
            if (type == typeof(Single)) return PropertyTypeEnum.Float;
            if (type == typeof(Double)) return PropertyTypeEnum.Double;
            if (type == typeof(String)) return PropertyTypeEnum.String;
            if (type.IsArray)
            {
                Int32 rank = type.GetArrayRank();

                if (rank == 1) 
                    return PropertyTypeEnum.Array1;
                if (rank == 2)
                    return PropertyTypeEnum.Array2;
                else 
                    throw new NotSupportedException($"Array Rank Is{rank}");
            }
            if(typeof(IList).FullName is String typefullname && type.GetInterface(typefullname) !=null)
                return PropertyTypeEnum.List;

            if (type.IsAnsiClass)
                return PropertyTypeEnum.Class;

            return PropertyTypeEnum.NaN;
        }
    }

}