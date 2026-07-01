using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 所有协议解码Prsnt的基类接口
    /// </summary>
    /// <remarks>所有的解码Prsnt的接口</remarks>
    public interface IProtocolPrsnt : IPresenter<IProtocolView>
    {
        /// <summary>
        /// 协议类型
        /// </summary>
        public SerialProtocolType ProtocolType { get; set; }


        /// <summary>
        /// 是否为触发解码
        /// </summary>
        public Boolean IsTrigger { get; set; }


        /// <summary>
        /// 获取系统中当前的触发中的解码Prsnt
        /// </summary>
        public IProtocolPrsnt GetDecodePrsnt(ChannelId id);

        /// <summary>
        /// 获取系统中所有通道中的协议解码Prsnt
        /// </summary>
        public List<IProtocolPrsnt> GetChannlesDecodePrsnt();
    }
}
