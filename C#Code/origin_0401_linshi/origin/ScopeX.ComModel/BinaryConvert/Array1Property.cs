using System;
using System.Text;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class Array1Property : ValueProperty
        {
            public Array1Property(String name, PropertyTypeEnum valuetype,Byte[] val) : base(name, PropertyTypeEnum.Array1,val)
            {
                ValueType = valuetype;
            }
            public PropertyTypeEnum ValueType { get; set; } = PropertyTypeEnum.Bool;
            public Int64 Lenght { get; set; }

            public override String ToString()
            {
                return Name+" "+Lenght;
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
                writer.Write(Lenght);
                writer.Write(Value.LongLength);
                writer.Write(Value);
            }
        }
    }

}