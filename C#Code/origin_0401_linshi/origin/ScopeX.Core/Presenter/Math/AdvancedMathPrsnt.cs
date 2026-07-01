using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    /// <summary>
    /// 用于强占数学通道的表示层抽象类
    /// </summary>
    public abstract class AdvancedMathPrsnt
    {
       

        internal AdvancedMathPrsnt(AdvancedMathModel formula)
        {
            GraphModel = formula;
        }

        internal AdvancedMathModel GraphModel
        {
            get;
            private set;
        }

        public Boolean Enabled
        {
            get => GraphModel.Enabled;
            set
            {
                if (GraphModel.Enabled == value)
                    return;
                GraphModel.Enabled = value;
            }
        }

        /// <summary>
        /// 用来显示波形的数学通道
        /// </summary>
        public ChannelId DestMathChannel
        {
            get => GraphModel.MathChannelId;
            set
            {
                GraphModel.MathChannelId = value;
            }
        }
    }
}
