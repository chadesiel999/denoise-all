using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ListViewNF : System.Windows.Forms.ListView
    {
        public ListViewNF()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014)
                return;

            base.WndProc(ref m);
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;

            e.NewWidth = this.Columns[e.ColumnIndex].Width;

            base.OnColumnWidthChanging(e);
        }

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //public static extern int ShowScrollBar(IntPtr hWnd, int iBar, int bShow);

        //const int SB_HORZ = 0;
        //const int SB_VERT = 1;
        //protected override void WndProc(ref Message m)
        //{
        //    if (this.View == View.Details)
        //    {
        //        ShowScrollBar(this.Handle, SB_VERT, 1);
        //        ShowScrollBar(this.Handle, SB_HORZ, 0);
        //    }
        //    base.WndProc(ref m);
        //}
    }
}
