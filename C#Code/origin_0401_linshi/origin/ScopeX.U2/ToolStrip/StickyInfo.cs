using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    [DefaultEvent("MouseClick")]
    public partial class StickyInfo : UserControl
    {
        private bool _isMouseDown = false;
        private bool _isFlashOver = true;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<object> DataSource
        {
            get => USIFPanel.DataSource;
            set => USIFPanel.DataSource = value;
        }

        public override bool AutoSize
        {
            get => base.AutoSize;
            set
            {
                base.AutoSize = value;
                USIFPanel.AutoSize = value;
            }
        }


        [Editor(), Browsable(true), DefaultValue(typeof(Color), "DarkGray"), Category("CatAppearance"), Description("Sticky Note  BackColor")]
        public Color StickyBackColor
        {
            get => LblName.BackColor;
            set
            {
                LblName.BackColor = value;
                this.BackColor = value;
            }
        }

        private bool _isStickyBackColorChanged = false;
        [Description("背景颜色改变"), Category(Const.Category), Browsable(true)]
        public bool IsStickyBackColorChanged
        {
            get => _isStickyBackColorChanged;
            set
            {
                _isStickyBackColorChanged = value;
                if (value)
                {
                    LblName.BackColor = Color.FromArgb(0, 171, 209);
                    LblName.ForeColor = Color.Black;
                }
                else
                {
                    LblName.BackColor = Color.FromArgb(72, 77, 85);
                    LblName.ForeColor = Color.White;
                }
            }
        }

        [Editor(), Browsable(true), Category("CatAppearance"), Description("Sticky Note  Content BackColor")]
        public Color StickyContentBackColor
        {
            get => USIFPanel.BackColor;
            set => USIFPanel.BackColor = value;
        }

        [Editor(), Browsable(true), Category("CatAppearance"), Description("Sticky Note ForeColor")]
        public Color StickyForeColor
        {
            get => USIFPanel.ForeColor;
            set => USIFPanel.ForeColor = value;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Sticky Note Name")]
        public String StickyTitle
        {
            get => LblName.Text;
            set => LblName.Text = value;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Sticky Note Row Height")]
        public int ContentRowHeight
        {
            get => USIFPanel.ContentRowHeight;
            set => USIFPanel.ContentRowHeight = value;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Sticky Note IconWidth")]
        public int IconWidth
        {
            get => USIFPanel.IconWidth;
            set => USIFPanel.IconWidth = value;
        }

        [Editor(), Browsable(true), DefaultValue(""), Category("CatAppearance"), Description("Sticky Note Font")]
        public new Font Font
        {
            get => USIFPanel.Font;
            set => USIFPanel.Font = value;
        }

        [Description("指示标识的颜色"), Category(Const.Category), Browsable(true)]
        public Color IndicatorColor
        {
            get => LbIndicator.BackColor;
            set => LbIndicator.BackColor = value;
        }

        private bool _isIndicatorShow = false;
        [Description("指示标识是否显示"), Category(Const.Category), Browsable(true)]
        public bool IsIndicatorShow
        {
            get => _isIndicatorShow;
            set
            {
                _isIndicatorShow = value;
                LbIndicator.Visible = value;
                if (value)
                    this.TlpBody.RowStyles[1].Height = 4;   //indicator的高度为4；
                else
                    this.TlpBody.RowStyles[1].Height = 0;
            }
        }

        public StickyInfo()
        {
            InitializeComponent();
            this.USIFPanel.MouseClick += (s, e) => OnMouseClick(e);
            this.LblName.MouseClick += (s, e) => OnMouseClick(e);
            this.LbIndicator.MouseClick += (s, e) => OnMouseClick(e);
            this.USIFPanel.MouseDown += (s, e) => OnMouseDown(e);
            this.LblName.MouseDown += (s, e) => OnMouseDown(e);
            this.LbIndicator.MouseDown += (s, e) => OnMouseDown(e);
            this.USIFPanel.MouseUp += (s, e) => OnMouseUp(e);
            this.LblName.MouseUp += (s, e) => OnMouseUp(e);
            this.LbIndicator.MouseUp += (s, e) => OnMouseUp(e);
        }

        public void BeginUpdate() => USIFPanel.BeginUpdate();
        public void EndUpdate() => USIFPanel.EndUpdate();

        public void SetToolTip(ToolTip toolTip, String msg)
        {
            toolTip?.SetToolTip(this, msg);
            foreach (Control ctrl in Controls)
            {
                toolTip?.SetToolTip(ctrl, msg);
            }
        }

        public void Prompt(DefaultPromptProperty newValue, string token = "")
        {
            USIFPanel.Prompt(newValue, token);
        }
        public void SetStringColorByIndex(Int32 index, Color foreColor)
        {
            USIFPanel.SetStringColorByIndex(index, foreColor);
        }
        public void CancelStringColorByIndex(int index)
        {
            USIFPanel.CancelStringColorByIndex(index);
        }
        public void CancelAllStringColorByIndex()
        {
            USIFPanel.CancelAllStringColorByIndex();
        }
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(33, 33, 40)), 8), this.ClientRectangle);
        //    base.OnPaint(e);
        //}
        private void label1_Paint(object sender, PaintEventArgs e)
        {
           
            if (_isStickyBackColorChanged)
            {
                LblName.BackColor = Color.FromArgb(0, 171, 209);
                LblName.ForeColor = Color.Black;
            }
            else 
            {
                Color color1 = Color.FromArgb(72, 77, 85);
                Color color2 = Color.FromArgb(42, 43, 45);
                LinearGradientBrush brush = new LinearGradientBrush(LblName.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, LblName.ClientRectangle);
                TextRenderer.DrawText(e.Graphics, LblName.Text, LblName.Font, LblName.ClientRectangle, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
           if(!_isMouseDown)
            {
                Padding = new Padding(Padding.Left + 1,
                                          Padding.Top + 1,
                                          Padding.Right + 1,
                                          Padding.Bottom + 1);
                _isMouseDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
           if(_isFlashOver)
            {
                Task.Run(() =>
                {
                    _isFlashOver = false;
                    this?.Invoke(() =>
                    {
                        Padding = new Padding(Padding.Left - 1,
                                              Padding.Top - 1,
                                              Padding.Right - 1,
                                              Padding.Bottom - 1);
                    });

                    System.Threading.Thread.Sleep(100);
                    _isMouseDown = false;
                    _isFlashOver = true;
                    Invalidate();
                });
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_isMouseDown)
            {
                ControlPaint.DrawBorder(
                  e.Graphics,
                  ClientRectangle,
                  Color.FromArgb(0, 191, 255),      // 左侧线颜色
                  1,               // 左侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 右侧线颜色
                  1,               // 右侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 上侧线颜色
                  1,               // 上侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 下侧线颜色
                  1,               // 下侧线宽度
                  ButtonBorderStyle.Solid);
            }
        }
    }
}
