using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.U2
{
    /// <summary>
    /// 单个功能面板接口
    /// </summary>
    public interface IPanel
    {
        /// <summary>
        /// 功能面板的标题栏背景色属性
        /// </summary>
        Color HeaderBackColor { set; get; }

        /// <summary>
        /// 功能面板的标题栏前景色属性
        /// </summary>
        Color HeaderForeColor { set; get; }

        /// <summary>
        /// 功能面板的内容区背景色属性
        /// </summary>
        Color ContentBackColor { set; get; }

        /// <summary>
        /// 功能面板的内容区前景色属性
        /// </summary>
        Color ContentForeColor { set; get; }

        /// <summary>
        /// 功能面板的边框颜色属性
        /// </summary>
        Color BorderColor { set; get; }

        /// <summary>
        /// 设置功能面板的边框粗细
        /// </summary>
        Int32 BorderThickness { set; get; }
    }
}
