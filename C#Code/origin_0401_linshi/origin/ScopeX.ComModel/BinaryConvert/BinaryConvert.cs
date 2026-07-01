using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Reflection;

namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        /// <summary>
        /// 文件头
        /// </summary>
        private const Int32 FILEHEADER = 0x7890;
        /// <summary>
        /// 文件类型
        /// </summary>
        private const UInt16 TOPCLASSNAME = 0xAABB;
        /// <summary>
        /// 字符串分隔符
        /// </summary>
        private const Char SEPARATOR = (Char)0x0A;
        /// <summary>
        /// 属性/字段搜索范围
        /// </summary>
        private const BindingFlags PROPERTYSEARCHFLAGS = BindingFlags.Public | BindingFlags.Instance| BindingFlags.NonPublic;
        /// <summary>
        /// 参数搜索范围
        /// </summary>
        private const MemberTypes MEMBERSEARCHTYPE = MemberTypes.Field | MemberTypes.Property;
        /// <summary>
        /// 设置字符串编码方式,默认为UTF8
        /// </summary>
        public static Encoding Encoding { get; set; } = Encoding.UTF8;
        public static Stream Serialize<T>(T value,FileTypeHeader fileType = FileTypeHeader.Extension) where T:class
        {
            System.IO.MemoryStream memorystream = new System.IO.MemoryStream();
            Serialize(value, memorystream, fileType);
            return memorystream;
        }

        public static async Task<Stream> SerializeAsync<T>(T value, FileTypeHeader fileType = FileTypeHeader.Extension) where T : class
        {
            return await Task.Run(() => Serialize(value, fileType));
        }

        public static void Serialize<T>(T value,Stream stream,FileTypeHeader fileType = FileTypeHeader.Extension) where T :class
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));            

            Type type = typeof(T);

            if (type.IsAnsiClass && !type.IsArray)
            {
                List<BaseProperty> result = BinarySerialize.SerializationClassProperties(value);
                System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(stream);
                result.Insert(0, new ClassProperty(BitConverter.GetBytes(TOPCLASSNAME), type.GetMembers(PROPERTYSEARCHFLAGS).Count(x => (x.MemberType & MEMBERSEARCHTYPE) == x.MemberType), true));
                binaryWriter.Write(FILEHEADER);
                binaryWriter.Write((Int32)fileType);
                result.ForEach(x =>
                {
                    x.WriteBytes(binaryWriter, fileType);
                });
                binaryWriter.Flush();
            }
        }
        public static async void SerializeAsync<T>(T value, Stream stream, FileTypeHeader fileType = FileTypeHeader.Extension) where T :class
        {
            await Task.Run(() => Serialize(value, stream, fileType));
        }

        public static T? Deserialize<T>(Stream stream) where T:class
        {
            T? val = BinaryDeserialize.CreateInstance<T>();

            if (val == null)
                throw new Exception("程序无法创建特定类");

            BinaryReader binaryReader = new BinaryReader(stream);

            if (binaryReader.ReadInt32() == FILEHEADER)
            {
                Int32 filetype = binaryReader.ReadInt32();
                Int32 index = Enum.GetValues<FileTypeHeader>().Cast<Int32>().ToList().FindIndex(x => x == filetype);

                if (index != -1)
                {
                    BinaryDeserialize deserialization = BinaryDeserialize.GetDeserialization(Enum.GetValues<FileTypeHeader>()[index]);

                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        deserialization.DeserializationValue(ref val, binaryReader);
                    }
                }
            }
            return val;
        }

        public static async Task<T?> DeserializeAsync<T>(Stream stream) where T : class
        {
            return await Task.Run(() => Deserialize<T>(stream));
        }
    }

}