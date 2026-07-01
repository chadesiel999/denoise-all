namespace ScopeX.ComModel
{
    public static partial class BinaryConvert
    {
        /// <summary>
        /// 文件类型头
        /// </summary>
        public enum FileTypeHeader
        {
            /// <summary>
            /// 扩展，文件包含属性名称
            /// </summary>
            Extension = 0x1234,
            /// <summary>
            /// 压缩，文件不包含属性名称
            /// </summary>
            Compression = 0x4567,
        }
    }

}