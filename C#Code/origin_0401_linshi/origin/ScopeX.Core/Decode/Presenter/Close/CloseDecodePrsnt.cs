using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// Close的通道解码Prsnt
    /// </summary>
    public class CloseDecodePrsnt : ProtocolPrsnt
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">通道</param>
        /// <param name="view">View</param>
        public CloseDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (CloseDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.Close);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>();
        }

        /// <summary>
        /// Model
        /// </summary>
        private protected override CloseDecodeModel Model
        {
            get;
        }
    }
}
