using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;
namespace ScopeX.U2
{
    public partial class ScopeXCheckButton : UserControl, IStylize
    {
        #region 属性

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }


        private Font _BaseFont;
        [Description("字体"), Category("自定义属性")]
        public Font BaseFont
        {
            get => _BaseFont;
            set
            {
                base.Font = BtnCheck.Font = BtnMain.Font = value;
                _BaseFont = value;
            }
        }

        private Color _BaseColor = Color.FromArgb(47, 47, 47);
        [Description("背景色"), Category("自定义属性")]
        public Color BaseColor
        {
            get { return _BaseColor; }
            set
            {
                base.BackColor = BtnCheck.BackColor = BtnMain.BackColor = value;
                _BaseColor = value;
            }
        }

        private Color _FontColor = Color.White;
        [Description("字体颜色"), Category("自定义属性")]
        public Color FontColor
        {
            get { return _FontColor; }
            set
            {
                base.ForeColor = BtnCheck.ForeColor = BtnMain.ForeColor = value;
                _FontColor = value;
            }
        }

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override String Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private String _TextInfo = String.Empty;
        [Description("显示文本"), Category("自定义属性")]
        public String TextInfo
        {
            get => _TextInfo;
            set
            {
                BtnMain.Text = value;
                _TextInfo = value;
            }
        }

        private Image _Icon;
        [Description("按钮图标"), Category("自定义属性")]
        public Image Icon
        {
            get => _Icon;
            set
            {
                BtnMain.Icon = value;
                _Icon = value;
            }
        }

        private Boolean _Checked = false;
        [Description("是否选择"), Category("自定义属性")]
        public Boolean Checked
        {
            get => _Checked;
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    CheckedChangedEvent?.Invoke(this, EventArgs.Empty);
                }
                BtnCheck.Icon = value ? Properties.Resources.Check : null;
            }
        }

        private Cursor _Cursor = Cursors.Default;
        [Description("光标"), Category("自定义属性")]
        public override Cursor Cursor
        {
            get => _Cursor;
            set
            {
                _Cursor = value;
                BtnMain.Cursor = BtnCheck.Cursor = base.Cursor = value;
            }
        }

        private Color pressedBackColor = Color.Gray;
        /// <summary>
        /// 点击时背景颜色
        /// </summary>
        [Browsable(true)]
        [Description("点击时背景颜色"), Category("自定义属性"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedBackColor
        {
            get { return pressedBackColor; }
            set
            {
                BtnCheck.PressedBackColor = BtnMain.PressedBackColor = value;
                pressedBackColor = value;

            }
        }
        private Color mouseinBackColor = Color.Transparent;
        /// <summary>
        /// 鼠标进入时背景颜色
        /// </summary>
        [Browsable(true)]
        [Description("鼠标进入背景颜色"), Category("自定义属性"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color MouseinBackColor
        {
            get { return mouseinBackColor; }
            set
            {
                BtnCheck.MouseinBackColor = BtnMain.MouseinBackColor = value;
                mouseinBackColor = value;
            }
        }

        private Color mouseOutBackColor = Color.Transparent;
        /// <summary>
        /// 鼠标进入时背景颜色
        /// </summary>
        [Browsable(true)]
        [Description("鼠标离开背景颜色"), Category("自定义属性"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color MouseOutBackColor
        {
            get { return mouseOutBackColor; }
            set
            {
                mouseOutBackColor = value;
            }
        }


        #endregion

        /// <summary>
        /// 选中状态改变事件
        /// </summary>
        [Browsable(true), Description("选中状态改变事件")]
        public event EventHandler CheckedChangedEvent;

        public ScopeXCheckButton()
        {
            InitializeComponent();
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            this.Click += (o, e) => { Checked = !Checked; };
        }


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }

        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
                BtnCheck.Click += value;
                BtnMain.Click += value;
            }
            remove
            {
                base.Click -= value;
                BtnMain.Click -= value;
                BtnCheck.Click -= value;
            }
        }
    }
}
