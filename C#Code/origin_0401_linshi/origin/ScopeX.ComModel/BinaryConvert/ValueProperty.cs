using System;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class ValueProperty : BaseProperty
        {
            public ValueProperty(String name, PropertyTypeEnum type,Byte[] val):base(name,type)
            {
                Value = val;
            }
            public Byte[] Value { get; set; } = new Byte[0];
            public override String ToString()
            {
                return base.ToString() +" " + Value.Length; 
            }
            public override void WriteBytes(BinaryWriter writer, FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                base.WriteBytes(writer,fileType);
                writer.Write(Value);
            }
        }
    }

}