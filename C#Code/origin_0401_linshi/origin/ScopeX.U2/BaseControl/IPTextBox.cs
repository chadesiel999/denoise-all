using ScopeX.Controls.Common.Default;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.U2.Tools;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class IPTextBox : Control, IStylize, ILanguageControl
    {
        #region Const

        private const Int32 MIN_NUM = 0;//最小值
        private const Int32 MAX_NUM = 255;//最大值
        private const Int32 OFFSET = 5;

        #endregion Const

        #region 字段

        private readonly TextItemInfo _IP1;
        private readonly TextItemInfo _IP2;
        private readonly TextItemInfo _IP3;
        private readonly TextItemInfo _IP4;
        private readonly TextItemInfo _Dot1;
        private readonly TextItemInfo _Dot2;
        private readonly TextItemInfo _Dot3;
        private readonly TextItemInfo _CursorLine;

        private readonly Timer _Timer;
        private List<TextItemInfo> _TextInfos;

        private Boolean _ShowCursor = true;
        private Boolean _ArgToCtrl = false;
        private Boolean _IsDiposed = true;//防止冗余调用

        #endregion 字段

        #region 属性

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public LanguagePattern LanguagePattern { get => LanguagePattern.Ignore; set => _ = value; }

        private IPAddress iPAddress;

        [Browsable(true)]
        [Description("返回System.Net.IPAddress类型的IP地址"), DefaultValue(typeof(IPAddress))]
        public IPAddress IPAddress
        {
            get
            {
                if (iPAddress == null)
                {
                    iPAddress = new IPAddress();
                }

                iPAddress.IP1 = _IP1.Text;
                iPAddress.IP2 = _IP2.Text;
                iPAddress.IP3 = _IP3.Text;
                iPAddress.IP4 = _IP4.Text;
                return iPAddress;
            }
            set { iPAddress = value; }
        }

        [Browsable(true)]
        [Description("string类型的用户在IPBox控件中输入的IP地址"), DefaultValue(typeof(IPAddress))]
        public override String Text
        {
            get
            {
                return IPAddress.ToString();
            }
            set
            {
                try
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        SetDefaultValue();
                    }
                    else
                    {
                        String[] str = new String[4];
                        str = value.Split(Char.Parse("."));
                        if (str.Length >= 4)
                        {
                            for (Int32 i = 0; i < 4; i++)
                            {
                                if (Int32.TryParse(str[i], out int i2) && i2 > -1 && i2 < 256)
                                {
                                    _IP1.Text = str[0];
                                    _IP2.Text = str[1];
                                    _IP3.Text = str[2];
                                    _IP4.Text = str[3];
                                }
                            }
                        }
                        else
                        {
                            SetDefaultValue();
                        }
                    }
                }
                catch
                {
                    SetDefaultValue();
                }
                Refresh();
            }
        }

        private void SetDefaultValue()
        {
            _IP1.Text = "0";
            _IP2.Text = "0";
            _IP3.Text = "0";
            _IP4.Text = "0";
        }

        [Browsable(true)]
        [Description("背景颜色"), DefaultValue(typeof(Color))]
        public override Color BackColor { get; set; } = Color.FromArgb(53, 54, 58);

        private Color _ForeColor = Color.White;

        [Browsable(true)]
        [Description("字体颜色"), DefaultValue(typeof(Color))]
        public override Color ForeColor { get; set; } = Color.White;

        [Browsable(true)]
        [Description("文本字体"), DefaultValue(typeof(Font))]
        public override Font Font
        {
            get => base.Font; set
            {
                if (base.Font != value)
                {
                    CalcDotRegion();
                    CalcCursorRegion();
                    CalcTextInfoRegion();
                    Refresh();
                }
                base.Font = value;
            }
        }

        [Browsable(true)]
        [Description("光标样式"), DefaultValue(typeof(Cursors))]
        public override Cursor Cursor { get; set; } = Cursors.IBeam;

        [Browsable(true)]
        [Description("选中区域颜色"), DefaultValue(typeof(Color))]
        public Color SelectedColor { get; set; } = Color.FromArgb(40, 71, 193);

        /// <summary>
        /// 选中区域
        /// </summary>
        private Rectangle SelectedRegion { get; set; }

        private Color BackupBackColor;
        private Color BackupForeColor;

        public new Boolean Enabled
        {
            get => base.Enabled;
            set
            {

                BackColor = value ? BackupBackColor : Color.FromArgb(30, 33, 36);
                ForeColor = value ? BackupForeColor : Color.FromArgb(150, 153, 156);
                base.Enabled = value;
            }
        }

        #endregion 属性

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public IPTextBox()
        {
            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            BackupBackColor = BackColor;
            BackupForeColor = ForeColor;
            _IP1 = new() { ItemIndex = 1 };
            _IP2 = new() { ItemIndex = 2 };
            _IP3 = new() { ItemIndex = 3 };
            _IP4 = new() { ItemIndex = 4 };
            _Dot1 = new() { Text = "." };
            _Dot2 = new() { Text = "." };
            _Dot3 = new() { Text = "." };
            _CursorLine = new() { Text = "|" };

            _Timer = new();
            _TextInfos = new();

            _IP1.TextChanged += (_, _) =>
                  {
                      var itemwidth = Width / 4;
                      //_IP1
                      var textwidth = TextRenderer.MeasureText(_IP1.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
                      _IP1.Bound = new Rectangle(itemwidth * 0, 0, itemwidth, Height);
                      _IP1.DrawRegion = new Rectangle(itemwidth - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);
                  };

            _IP2.TextChanged += (_, _) =>
            {
                var itemwidth = Width / 4;
                //_IP2
                var textwidth = TextRenderer.MeasureText(_IP2.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
                _IP2.Bound = new Rectangle(itemwidth * 1, 0, itemwidth, Height);
                _IP2.DrawRegion = new Rectangle(itemwidth * 2 - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);
            };

            _IP3.TextChanged += (_, _) =>
            {
                var itemwidth = Width / 4;
                //_IP3
                var textwidth = TextRenderer.MeasureText(_IP3.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
                _IP3.Bound = new Rectangle(itemwidth * 2, 0, itemwidth, Height);
                _IP3.DrawRegion = new Rectangle(itemwidth * 3 - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);
            };

            _IP4.TextChanged += (_, _) =>
            {
                var itemwidth = Width / 4;
                //_IP4
                var textwidth = TextRenderer.MeasureText(_IP4.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
                _IP4.Bound = new Rectangle(itemwidth * 3, 0, itemwidth, Height);
                _IP4.DrawRegion = new Rectangle(Width - textwidth - OFFSET * 3, 0, textwidth + OFFSET, Height);
            };

            _TextInfos.Add(_IP1);
            _TextInfos.Add(_IP2);
            _TextInfos.Add(_IP3);
            _TextInfos.Add(_IP4);

            _Timer.Interval = 1000;
            _Timer.Tick += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    _ShowCursor = !_ShowCursor;
                    Refresh();
                }
            };
            _Timer.Start();
            _Timer.Enabled = false;
        }

        #region override

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            CalcDotRegion();
            CalcTextInfoRegion();
            CalcCursorRegion();
            Draw(e.Graphics);
        }

        public override void Refresh()
        {
            try
            {
                Draw(Graphics.FromHwnd(Handle));
            }
            catch (Exception)
            { }
            base.Refresh();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            _ArgToCtrl = true;
            var mousepoint = this.PointToClient(Control.MousePosition);
            var item = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Bound.Contains(mousepoint));
            if (item != null)
            {
                item.Focused = true;
                var otheritem = _TextInfos.Where((TextItemInfo x) => x.ItemIndex != item.ItemIndex).ToList();
                foreach (var other in otheritem)
                {
                    other.Focused = !item.Focused;
                }
            }
            else
            {
                SetAllFocused(false);
            }
            _ArgToCtrl = false;

            if (IsAnyFocused())
            {
                SetAllselected(false);
                _ShowCursor = true;
                Refresh();//先刷新一次 光标立即显示
                _Timer.Enabled = true;
                this.Focus();
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            _ArgToCtrl = true;
            var mousepoint = this.PointToClient(Control.MousePosition);
            var item = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Bound.Contains(mousepoint));
            if (item != null)
            {
                item.Selected = true;
                var otheritem = _TextInfos.Where((TextItemInfo x) => x.ItemIndex != item.ItemIndex).ToList();
                foreach (var other in otheritem)
                {
                    other.Selected = !item.Selected;
                }
            }
            else
            {
                SetAllselected(false);
            }

            _ArgToCtrl = false;

            if (IsAnySelected())
            {
                _ShowCursor = _Timer.Enabled = false;
                CalcSelectedRegion();
                Refresh();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                _ArgToCtrl = true;
                if (_IP1.Selected || _IP1.Focused)
                {
                    _IP1.Focused = _IP1.Selected = false;
                    _IP2.Focused = _IP2.Selected = true;
                }
                else if (_IP2.Selected || _IP2.Focused)
                {
                    _IP2.Focused = _IP2.Selected = false;
                    _IP3.Focused = _IP3.Selected = true;
                }
                else if (_IP3.Selected || _IP3.Focused)
                {
                    _IP3.Focused = _IP3.Selected = false;
                    _IP4.Focused = _IP4.Selected = true;
                }
                else if (_IP4.Selected == true || _IP4.Focused)
                {
                    _IP4.Focused = _IP4.Selected = false;
                    _Timer.Enabled = false;
                }
                _ArgToCtrl = false;
                if (IsAnySelected())
                {
                    CalcSelectedRegion();
                    _ShowCursor = _Timer.Enabled = false;
                }
                Refresh();
            }
            return keyData == Keys.Tab;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // 阻止事件冒泡
            e.Handled = true;
            //如果输入的不是退格和数字，则屏蔽输入
            if (!(e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
            {
                if (e.KeyChar != '.')
                {
                    return;
                }
            }
            if (_IP1.Selected || _IP1.Focused)//IP1
            {
                KeyFilter(_IP1, e);
            }
            else if (_IP2.Selected || _IP2.Focused)
            {
                KeyFilter(_IP2, e);
            }
            else if (_IP3.Selected || _IP3.Focused)
            {
                KeyFilter(_IP3, e);
            }
            else if (_IP4.Selected || _IP4.Focused)
            {
                KeyFilter(_IP4, e);
            }

            if (!IsAnySelected())
            {
                if (IsAnyFocused())
                {
                    _Timer.Enabled = _ShowCursor = true;
                }
                else
                {
                    _Timer.Enabled = _ShowCursor = false;
                }
            }
            else
            {
                CalcSelectedRegion();
                _Timer.Enabled = _ShowCursor = false;
            }

            Refresh();
        }

        private void KeyFilter(TextItemInfo info, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (info.Selected)//选中状态
                {
                    if (e.KeyChar != '.')
                    {
                        info.Selected = false;
                        info.Text = e.KeyChar.ToString();
                    }
                    else
                    {
                        switch (info.ItemIndex)
                        {
                            case 1:
                                _IP2.Focused = !(info.Focused = false);
                                _IP2.Selected = !(info.Selected = false);
                                break;

                            case 2:
                                _IP3.Focused = !(info.Focused = false);
                                _IP3.Selected = !(info.Selected = false);
                                break;

                            case 3:
                                _IP4.Focused = !(info.Focused = false);
                                _IP4.Selected = !(info.Selected = false);
                                break;

                            case 4:
                                info.Focused = info.Selected = false;
                                SendKeys.SendWait("{TAB}");
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    if ((info.Text.Length == 3) || e.KeyChar == '.')
                    {
                        ProcessText(info);
                    }
                    else
                    {
                        info.Text += e.KeyChar;
                        if (info.Text.Length == 3)
                        {
                            ProcessText(info);
                        }
                    }
                }
            }//end if e.KeyChar != '\b'
            else
            {
                if (info.Selected)
                {
                    info.Focused = !(info.Selected = false);
                    info.Text = "";
                }
                else
                {
                    if (info.Text.Length == 0)
                    {
                        info.Text = "";
                    }
                    else
                    {
                        info.Text = info.Text.Substring(0, info.Text.Length - 1);
                    }
                }
            }
        }

        private void ProcessText(TextItemInfo info)
        {
            if (Int32.TryParse(info.Text, out Int32 i) && (i < MIN_NUM || i > MAX_NUM))
            {
                info.Text = MAX_NUM.ToString();
            }
            else if (i == MIN_NUM)
            {
                info.Text = MIN_NUM.ToString();
            }
            info.Text = info.Text.TrimStart('0') == "" ? "0" : info.Text.TrimStart('0');
            switch (info.ItemIndex)
            {
                case 1:
                    _IP2.Focused = !(_IP1.Focused = false);
                    _IP2.Selected = !(_IP1.Selected = false);

                    break;

                case 2:
                    _IP3.Focused = !(_IP2.Focused = false);
                    _IP3.Selected = !(_IP2.Selected = false);

                    break;

                case 3:
                    _IP4.Focused = !(_IP3.Focused = false);
                    _IP4.Selected = !(_IP4.Selected = false);

                    break;

                case 4:
                    _IP4.Focused = _IP4.Selected = false;
                    SendKeys.SendWait("{TAB}");

                    break;

                default:
                    break;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!IsAnySelected() && !IsAnyFocused())
            {
                _IP1.Selected = true;
                CalcSelectedRegion();
                Refresh();
            }
            else
            {
                SystemSoftKeyboard.Show();
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SetAllselected(false);
            SetAllFocused(false);
            _ShowCursor = _Timer.Enabled = false;
            Refresh();
            //SystemSoftKeyboard.Close();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (_IsDiposed)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    _Timer.Enabled = false;
                    _Timer?.Dispose();
                    if (Font != null)
                    {
                        Font = null;
                    }
                }

                _IsDiposed = false;
            }
        }

        #endregion override

        #region private

        private void Draw(Graphics graphics)
        {
            graphics.Clear(BackColor);
            if (IsAnySelected())
            {
                graphics.FillRectangle(new SolidBrush(SelectedColor), SelectedRegion);
            }
            else
            {
                graphics.FillRectangle(new SolidBrush(BackColor), SelectedRegion);
            }
            TextRenderer.DrawText(graphics, _IP1.Text, Font, _IP1.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw IP1

            TextRenderer.DrawText(graphics, _Dot1.Text, Font, _Dot1.DrawRegion, ForeColor, TextFormatFlags.Left | TextFormatFlags.Bottom);//draw "."

            TextRenderer.DrawText(graphics, _IP2.Text, Font, _IP2.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw IP2

            TextRenderer.DrawText(graphics, _Dot1.Text, Font, _Dot2.DrawRegion, ForeColor, TextFormatFlags.Left | TextFormatFlags.Bottom);//draw "."

            TextRenderer.DrawText(graphics, _IP3.Text, Font, _IP3.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw IP3

            TextRenderer.DrawText(graphics, _Dot1.Text, Font, _Dot3.DrawRegion, ForeColor, TextFormatFlags.Left | TextFormatFlags.Bottom);//draw "."

            TextRenderer.DrawText(graphics, _IP4.Text, Font, _IP4.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw IP3

            if (_ShowCursor)
            {
                TextRenderer.DrawText(graphics, _CursorLine.Text, Font, _CursorLine.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw Cursor
            }
        }

        /// <summary>
        /// 计算IPAddress绘制区域
        /// </summary>
        private void CalcTextInfoRegion()
        {
            var itemwidth = Width / 4;

            //_IP1
            var textwidth = TextRenderer.MeasureText(_IP1.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _IP1.Bound = new Rectangle(itemwidth * 0, 0, itemwidth, Height);
            _IP1.DrawRegion = new Rectangle(itemwidth - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);

            //_IP2
            textwidth = TextRenderer.MeasureText(_IP2.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _IP2.Bound = new Rectangle(itemwidth * 1, 0, itemwidth, Height);
            _IP2.DrawRegion = new Rectangle(itemwidth * 2 - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);

            //_IP3
            textwidth = TextRenderer.MeasureText(_IP3.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _IP3.Bound = new Rectangle(itemwidth * 2, 0, itemwidth, Height);
            _IP3.DrawRegion = new Rectangle(itemwidth * 3 - textwidth - OFFSET * 2, 0, textwidth + OFFSET, Height);

            //_IP4
            textwidth = TextRenderer.MeasureText(_IP4.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _IP4.Bound = new Rectangle(itemwidth * 3, 0, itemwidth, Height);
            _IP4.DrawRegion = new Rectangle(Width - textwidth - OFFSET * 3, 0, textwidth + OFFSET, Height);
        }

        /// <summary>
        /// 计算Dot绘制区域
        /// </summary>
        private void CalcDotRegion()
        {
            var itemwidth = Width / 4;
            //_Dot1
            Int32 dotwidth = TextRenderer.MeasureText(_Dot1.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Dot1.DrawRegion = new Rectangle(itemwidth - OFFSET, 0, dotwidth + OFFSET, Height);

            //_Dot2
            _Dot2.DrawRegion = new Rectangle(itemwidth * 2 - OFFSET, 0, dotwidth + OFFSET, Height);

            //_Dot3
            _Dot3.DrawRegion = new Rectangle(itemwidth * 3 - OFFSET, 0, dotwidth + OFFSET, Height);
        }

        /// <summary>
        /// 计算光标绘制区域
        /// </summary>
        private void CalcCursorRegion()
        {
            var length = TextRenderer.MeasureText(_CursorLine.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            var focusedtext = GetFocusedText();
            if (focusedtext != null)
            {
                _CursorLine.DrawRegion = new Rectangle(focusedtext.DrawRegion.X + focusedtext.DrawRegion.Width, 0, length, Height);
            }
        }

        /// <summary>
        /// 计算选中文本绘制区域
        /// </summary>
        private void CalcSelectedRegion()
        {
            var textinfo = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Selected == true);
            if (textinfo == null)
            {
                return;
            }
            var size = TextRenderer.MeasureText(textinfo.Text, Font);
            SelectedRegion = new Rectangle(textinfo.DrawRegion.X, textinfo.DrawRegion.Y + (textinfo.DrawRegion.Height - size.Height) / 2, size.Width, size.Height);
        }

        /// <summary>
        /// 获取一个值 指示是否有区域被选中
        /// </summary>
        /// <returns></returns>
        private Boolean IsAnySelected()
        {
            if (_IP1.Selected || _IP2.Selected || _IP3.Selected || _IP4.Selected)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个值 指示是否有区域获取到输入焦点
        /// </summary>
        /// <returns></returns>
        private Boolean IsAnyFocused()
        {
            if (_IP1.Focused || _IP2.Focused || _IP3.Focused || _IP4.Focused)
            {
                return true;
            }
            return false;
        }

        private void SetAllselected(Boolean selected = true)
        {
            foreach (var item in _TextInfos)
            {
                item.Selected = selected;
            }
        }

        private void SetAllFocused(Boolean focused = true)
        {
            foreach (var item in _TextInfos)
            {
                item.Focused = focused;
            }
        }

        /// <summary>
        /// 获取得到输入焦点的TextInfo
        /// </summary>
        /// <returns></returns>
        private TextItemInfo GetFocusedText()
        {
            var textifo = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Focused == true);
            return textifo;
        }

        #endregion private

        public void Clear()
        {
            Text = "0,0,0,0";
        }
    }

    public class TextItemInfo
    {
        public event EventHandler TextChanged = delegate { };

        private String _Text = "0";

        /// <summary>
        /// 文本
        /// </summary>
        public String Text
        {
            get => _Text;
            set
            {
                if (_Text != value)
                {
                    TextChanged(new Object(), new EventArgs());
                }
                _Text = value;
            }
        }

        /// <summary>
        /// 边界
        /// </summary>
        public Rectangle Bound { get; set; }

        /// <summary>
        /// 绘制区域
        /// </summary>
        public Rectangle DrawRegion { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public Boolean Selected { get; set; } = false;

        /// <summary>
        /// 是否获取输入焦点
        /// </summary>
        public Boolean Focused { get; set; }

        public Int32 ItemIndex { get; set; }
    }

    public class IPAddress
    {
        public String IP1 { get; set; }
        public String IP2 { get; set; }
        public String IP3 { get; set; }
        public String IP4 { get; set; }

        public override String ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}", IP1, IP2, IP3, IP4);
        }
    }
}
