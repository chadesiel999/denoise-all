using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    /// <summary>
    /// 作为IPanel集合的管理
    /// </summary>
    public interface IPanelManager : IEnumerable<IPanel>
    {
        /// <summary>
        /// 添加一个IPanel到集合的尾部
        /// </summary>
        /// <param name="p"></param>
        /// InvalidOperationException : IPanelManager已经包含IPanel实例了
        void Add(IPanel p);

        /// <summary>
        /// 移除一个指定的IPanel实例
        /// </summary>
        /// <param name="p"></param>
        /// <exception>
        /// ArgumentException : 找不到指定移除的IPanel实例
        /// </exception>
        void Remove(IPanel p);

        /// <summary>
        ///  移除最后一个IPanel实例
        /// </summary>
        /// <exception>
        /// InvalidOperationException : IPanelManager已经没有IPanel实例了
        /// </exception>
        void Remove();

        /// <summary>
        /// 移除一个指定index的IPanel实例
        /// </summary>
        /// <param name="index"></param>
        /// <exception>
        /// ArgumentOutOfRangeException
        /// </exception>
        void RemoveAt(Int32 index);

        /// <summary>
        /// 插入一个指定的IPanel实例到特定位置
        /// </summary>
        /// <param name="p"></param>
        /// <param name="index"></param>
        /// <exception>
        /// ArgumentOutOfRangeException
        /// </exception>
        void Insert(Int32 index, IPanel p);

        /// <summary>
        /// 清空IPanel集合
        /// </summary>
        void Clear();
    }
}
