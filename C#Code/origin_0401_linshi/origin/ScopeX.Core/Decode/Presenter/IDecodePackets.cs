using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 协议解析后结果关键信息，主要是提供给其他功能模块使用，如搜索等
    /// </summary>
    public interface IDecodePackets
    {
        public Single Start { get; }
        public Single Lenght { get; }
        public Boolean IsInfoPacket { get; }
        public ComModel.SerialProtocolType ProtocolType { get; }
        public Single End => Start + Lenght;
    }
}
