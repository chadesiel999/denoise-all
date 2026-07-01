using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Common.APIs;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public class ListViewNFEx : ListViewNF
    {
        private const Int32 _ContentOffsetX = 10;  //Text内容得偏移X

        public ListViewNFEx()
        {
            OwnerDraw = true;
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            base.OnDrawColumnHeader(e);
            e.Graphics.DrawRectangle(new Pen(AppStyleConfig.DefaultTitleBackColor, 1.0F), e.Bounds);

            Int32 ContentOffsetY = (e.Bounds.Height - TextRenderer.MeasureText(e.Header.Text, e.Font).Height) / 2;
            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                e.Font,
                new Point(e.Bounds.Left + _ContentOffsetX, e.Bounds.Top + ContentOffsetY),
                AppStyleConfig.DefaultTitleForeColor);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            base.OnDrawItem(e);
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            base.OnDrawSubItem(e);

            Color textColor = this.ForeColor;
       
            //值列处理
            if(e.ColumnIndex % 2 == 1)
            {
                textColor = AppStyleConfig.DefaultContextForeColor;
                e.Graphics.FillRectangle(new SolidBrush(AppStyleConfig.DefaultContextDarkBackColor), e.Bounds);
            }

            e.Graphics.DrawRectangle(new Pen(AppStyleConfig.DefaultTitleBackColor, 1.0F), e.Bounds);

            Int32 ContentOffsetY = (e.Bounds.Height - TextRenderer.MeasureText(e.Header.Text, this.Font).Height) / 2;
            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                this.Font,
                new Point(e.Bounds.Left + _ContentOffsetX, e.Bounds.Top + ContentOffsetY),
                textColor);
        }

        protected override void WndProc(ref Message m)
        {
            if(!DesignMode)
            {
                //取消滑动边框的显示
                if (m.Msg == APIsUser32.WM_NCCALCSIZE)
                {
                    Int32 style = (Int32)APIsUser32.GetWindowLong(this.Handle, APIsUser32.GWL_STYLE);
                    UInt32 newStyle = (UInt32)style;
                    if ((style & APIsUser32.WS_VSCROLL) == APIsUser32.WS_VSCROLL)
                        newStyle = (UInt32)(newStyle & ~APIsUser32.WS_VSCROLL);
                    if ((style & APIsUser32.WS_HSCROLL) == APIsUser32.WS_HSCROLL)
                        newStyle = (UInt32)(newStyle & ~APIsUser32.WS_HSCROLL);

                    APIsUser32.SetWindowLong(this.Handle, APIsUser32.GWL_STYLE, newStyle);
                }
            }
         
            base.WndProc(ref m);
        }
    }
}
