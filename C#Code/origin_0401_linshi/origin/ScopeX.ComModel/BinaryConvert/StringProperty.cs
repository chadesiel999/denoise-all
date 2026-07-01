using System;
using System.Text;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class StringProperty :ValueProperty
        {
            public StringProperty(String name, Byte[] val) : base(name, PropertyTypeEnum.String, val)
            {
                StringLenght = val.Length;
            }
            public Int32 StringLenght { get; set; }

            public override String ToString()
            {
                return base.ToString() +" "+ Encoding.GetString(Value);
            }
            public override void WriteBytes(BinaryWriter writer, FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                if (fileType == FileTypeHeader.Extension)
                {
                    writer.Write(NameLenght);
                    writer.Write(Encoding.GetBytes(Name));
                }

                writer.Write((Byte)PropertyType);
                writer.Write(StringLenght);
                writer.Write(Value);

            }
        }
    }

}