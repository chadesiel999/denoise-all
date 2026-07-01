using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class TlpWaveFormContainer : TableLayoutPanel
    {
        //鼠标移动到子栏的边界时,激活控件动态改变大小事件
        //鼠标MouseDown
        //鼠标MouseMove
        //鼠标MoudeUP
        //鼠标箭头
        //当前窗体预览
        //所有绘制线程停止（暂停）
        //
        //
        //
        //
        //
        //
        //



        public TlpWaveFormContainer()
        {
            InitializeComponent();
        }

        public Boolean IsHorizontal { get; set; } = true;//是否水平分隔


    }
}
