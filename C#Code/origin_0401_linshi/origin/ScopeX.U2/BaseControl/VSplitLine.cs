using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace ScopeX.U2
{
    //[Designer(typeof(VSplitLineDesigner))]
    public partial class UUVerticalSplitLine : Panel
    {
        public UUVerticalSplitLine()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.None;
        }

        public UUVerticalSplitLine(IContainer container)
        {
            container?.Add(this);

            InitializeComponent();
        }

        private Color _DarkColor = Color.Black;
        public Color DarkColor
        {
            get => _DarkColor;
            set => _DarkColor = value;
        }

        private Color _LightColor = Color.White;
        public Color LightColor
        {
            get => _LightColor;
            set => _LightColor = value;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            /*int halfheight=this.Height/2;

            Rectangle horizontalFillRectangleDn = new Rectangle(0, halfheight, this.Width, this.Height - halfheight);
            LinearGradientBrush myHorizontalGradientDn = new LinearGradientBrush(horizontalFillRectangleDn, LightColor, DarkColor, LinearGradientMode.Vertical);
            pevent.Graphics.FillRectangle(myHorizontalGradientDn, horizontalFillRectangleDn);

            Rectangle horizontalFillRectangleUp = new Rectangle(0, 0, this.Width, halfheight+1);
            LinearGradientBrush myHorizontalGradientUp = new LinearGradientBrush(horizontalFillRectangleUp, DarkColor, LightColor, LinearGradientMode.Vertical);
            pevent.Graphics.FillRectangle(myHorizontalGradientUp, horizontalFillRectangleUp);*/

            Rectangle rect = new(0, 0, this.Width, this.Height);
            using Brush bkbrush = new SolidBrush(DarkColor);
            pevent?.Graphics.FillRectangle(bkbrush, rect);

        }

        /*
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Rectangle horizontalFillRectangleUp = new Rectangle(0, 0, this.Width, this.Height / 2);
            // Create a horizontal linear gradient with four stops.   
            LinearGradientBrush myHorizontalGradientUp = new LinearGradientBrush(horizontalFillRectangleUp, DarkColor, LightColor, LinearGradientMode.Vertical);
            pevent.Graphics.FillRectangle(myHorizontalGradientUp, horizontalFillRectangleUp);

            Rectangle horizontalFillRectangleDn = new Rectangle(0, this.Height / 2, this.Width, this.Height / 2);
            // Create a horizontal linear gradient with four stops.   
            LinearGradientBrush myHorizontalGradientDn = new LinearGradientBrush(horizontalFillRectangleDn, LightColor, DarkColor, LinearGradientMode.Vertical);
            pevent.Graphics.FillRectangle(myHorizontalGradientDn, horizontalFillRectangleDn);
        }
        
        */

        private void VSpliteLine_BackColorChanged(object sender, EventArgs e) => this.Refresh();

        private void VSpliteLine_SizeChanged(object sender, EventArgs e) => this.Refresh();

    }

    /*[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")] 
    public class VSplitLineDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        protected override void OnMouseEnter()
        {
            this.Control.Refresh();
        }

        protected override void OnMouseLeave()
        {
            this.Control.Refresh();
        }        


        protected override void OnPaintAdornments(System.Windows.Forms.PaintEventArgs pe)
        {
            UUVerticalSplitLine ctl = this.Control as UUVerticalSplitLine;
            int height1=ctl.Height /4;
            Rectangle horizontalFillRectangleUp = new Rectangle(0, 0, ctl.Width, height1);
            // Create a horizontal linear gradient with four stops.   
            LinearGradientBrush myHorizontalGradientUp = new LinearGradientBrush(horizontalFillRectangleUp, ctl.DarkColor, ctl.LightColor, LinearGradientMode.Vertical);
            pe.Graphics.FillRectangle(myHorizontalGradientUp, horizontalFillRectangleUp);

            Rectangle horizontalFillRectangleDn = new Rectangle(0, height1, ctl.Width, ctl.Height - height1);
            // Create a horizontal linear gradient with four stops.   
            LinearGradientBrush myHorizontalGradientDn = new LinearGradientBrush(horizontalFillRectangleDn, ctl.LightColor, ctl.DarkColor, LinearGradientMode.Vertical);

            pe.Graphics.FillRectangle(myHorizontalGradientDn, horizontalFillRectangleDn);
        }
    }*/
}
