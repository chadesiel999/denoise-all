using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public struct DeocodeDataSourcePacket
    {
        /// <summary>
        /// 通道解码数据
        /// </summary>
        public Byte[] ChannelDataSource;
        /// <summary>
        /// 三状态时低比较电平数据源
        /// </summary>
        public Byte[] ChannelLowDataSource;
        /// <summary>
        /// 解码数据中的通道
        /// <para>防止在暂停后用户修改了通道状态导致解码时分辨通道数据错误</para>
        /// </summary>
        public ChannelId[] Channels;
        /// <summary>
        /// 解码数据中每通道的数据长度
        /// </summary>
        public UInt32 PerChannelDataLength;
        /// <summary>
        /// 读取数据时的时间戳
        /// <para>是由当写入ddr数据时才能修改本参数</para>
        /// <para>软件通道此参数判断ddr中的数据是否被修改</para>
        /// <para>当数据被修改时，软件需要进行重新解码，否者不进行解码</para>
        /// </summary>
        public Int64 TimeStamp;
        /// <summary>
        /// 每通道交织的bit数
        /// <para>可选为8、16、32、64</para>
        /// </summary>
        public Int32 InterwovenBitCount;
        /// <summary>
        /// 实际有效数据长度
        /// </summary>
        public UInt32 MaxByteCount;
        /// <summary>
        /// 采样率,单位为s
        /// <para>模拟通道的采样率是相同的</para>
        /// </summary>
        public Double SampleRate;
        /// <summary>
        /// 标识是否具有有效数据
        /// <para>当标识为<see cref="false"/>时，解码结果为No Decode</para>
        /// </summary>
        public Boolean HasData;
        /// <summary>
        /// 波形触发在数据中的位置
        /// </summary>
        public Int64 TriggerIndex;
        
    }
}
