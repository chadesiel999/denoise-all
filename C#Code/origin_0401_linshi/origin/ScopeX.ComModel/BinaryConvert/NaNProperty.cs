using System;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class NaNProperty : BaseProperty
        {
            public NaNProperty(String name):base(name, PropertyTypeEnum.NaN)
            {
            }
            public override void WriteBytes(BinaryWriter writer, FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                if(fileType == FileTypeHeader.Extension)
                {
                    writer.Write(NameLenght);
                    writer.Write(Encoding.GetBytes(Name));
                }

                writer.Write((Byte)PropertyType);
            }
        }
    }

}