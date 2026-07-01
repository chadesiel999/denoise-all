using System;
using System.IO;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        private class ClassProperty : BaseProperty
        {
            public ClassProperty(String name, Int32 procount, Boolean isTopClass = false):base(name, PropertyTypeEnum.Class)
            {
                PropertyCount = procount;
                IsTopClass = isTopClass;
            }
            public ClassProperty(Byte[] name,Int32 procount, Boolean isTopClass = false) :base(name, PropertyTypeEnum.Class)
            {
                PropertyCount = procount;
                IsTopClass = isTopClass;
            }
            public Int32 PropertyCount { get; set; }
            public Boolean IsTopClass { get; set; }
            public override void WriteBytes(BinaryWriter writer, FileTypeHeader fileType = FileTypeHeader.Extension)
            {
                base.WriteBytes(writer,fileType);
                writer.Write(IsTopClass);
                writer.Write(PropertyCount);
            }
        }
    }

}