using System;
using System.Text;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class Array2Property : ValueProperty
        {
            public Array2Property(String name, PropertyTypeEnum valuetype, Byte[] val) : base(name, PropertyTypeEnum.Array2, val)
            {
                ValueType = valuetype;
            }
            public PropertyTypeEnum ValueType { get; set; } = PropertyTypeEnum.Bool;
            public Int64 Lenght1 { get; set; }
            public Int64 Lenght2 { get; set; }
            public override String ToString()
            {
                return Name + $" {Lenght1}:{Lenght2}";
            }
            public override void WriteBytes(BinaryWriter writer, FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                if (fileType == FileTypeHeader.Extension)
                {
                    writer.Write(NameLenght);
                    writer.Write(Encoding.GetBytes(Name));
                }

                writer.Write((Byte)PropertyType);
                writer.Write((Byte)ValueType);
                writer.Write(Lenght1);
                writer.Write(Lenght2);
                writer.Write(Value.LongLength);
                writer.Write(Value);
            }
        }
    }

}