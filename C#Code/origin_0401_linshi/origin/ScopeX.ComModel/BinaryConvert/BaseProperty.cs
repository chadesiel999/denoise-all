using System;
using System.Text;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private abstract class BaseProperty
        {
            private Byte[] _NameBytes = new Byte[0];
            public BaseProperty(String name, PropertyTypeEnum type = PropertyTypeEnum.NaN):this(Encoding.GetBytes(name),type)
            {
            }
            public BaseProperty(Byte[] name, PropertyTypeEnum type = PropertyTypeEnum.NaN)
            {
                this.PropertyType = type;
                _NameBytes = name;
            }
            public PropertyTypeEnum PropertyType { get; set; }
            public String Name
            { 
                get => Encoding.GetString(_NameBytes);
            }
            public Int32 NameLenght => _NameBytes.Length;
            public override String ToString()
            {
                return Name;
            }

            public virtual void WriteBytes(System.IO.BinaryWriter writer,FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                if (fileType == FileTypeHeader.Extension)
                {
                    writer.Write(NameLenght);
                    writer.Write(_NameBytes);
                }

                writer.Write((Byte)PropertyType);
            }
        }
    }

}